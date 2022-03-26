using System;
using System.Collections.Generic;
public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object> {
    public static Environment globals = new Environment();
    private Environment environment = globals;
    private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();
    public Interpreter() {
        globals.Define("clock", new BuiltInClock());
        globals.Define("print", new BuiltInPrint());
    }
    public void Interpret(List<Stmt> statements) {
        try {
            foreach(var statement in statements) {
                Execute(statement);
            }
        } catch(RuntimeException e) {
            Lox.RuntimeError(e);
        }
    }

    private void Execute(Stmt statement) {
        statement.Accept(this);
    }

    public void Resolve(Expr expr, int depth) {
        locals.Add(expr, depth);
    }
    
    public object VisitBlockStmt(Stmt.Block stmt) {
        ExecuteBlock(stmt.statements, new Environment(environment));
        return null;
    }

    public object VisitClassStmt(Stmt.Class stmt) {
        environment.Define(stmt.name.lexeme, null);

        Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
        foreach(var method in stmt.methods) {
            LoxFunction function = new LoxFunction(method, environment, method.name.lexeme == "init");
            methods[method.name.lexeme] = function;
        }

        LoxClass klass = new LoxClass(stmt.name.lexeme, methods);
        environment.Assign(stmt.name, klass);
        return null;
    }
    public void ExecuteBlock(List<Stmt>statements, Environment env) {
        Environment previous = environment;
        try {
            environment = env;
            foreach(var statement in statements) {
                Execute(statement);
            }
        } finally {
            environment = previous;
        }
    }
    public object VisitFunctionStmt(Stmt.Function stmt) {
        LoxFunction function = new LoxFunction(stmt, environment, false);
        environment.Define(stmt.name.lexeme, function);
        return null;
    }
    public object VisitVarStmt(Stmt.Var stmt) {
        object value = null;
        if(stmt.initializer != null) {
            value = Evaluate(stmt.initializer);
        }
        environment.Define(stmt.name.lexeme, value);
        return null;
    }

    public object VisitExpressionStmt(Stmt.Expression stmt) {
        Evaluate(stmt.expression);
        return null;
    }

    public object VisitWhileStmt(Stmt.While stmt) {
        while(IsTruthy(Evaluate(stmt.condition))) {
            Execute(stmt.body);
        }
        return null;
    }
    public object VisitIfStmt(Stmt.If stmt) {
        if(IsTruthy(Evaluate(stmt.condition))) {
            Execute(stmt.thenBranch);
        } else if(stmt.elseBranch != null) {
            Execute(stmt.elseBranch);
        }
        return null;
    }
    public object VisitReturnStmt(Stmt.Return stmt) {
        Object value = null;
        if(stmt.value != null) value = Evaluate(stmt.value);
        throw new Return(value);
    }
    public object VisitAssignExpr(Expr.Assign expr) {
        object value = Evaluate(expr.value);
        if(locals.ContainsKey(expr)) {
            environment.AssignAt(locals[expr], expr.name, value);
        } else {
            globals.Assign(expr.name, value);
        }
        return value;
    }
    public object VisitBinaryExpr(Expr.Binary expr) {
        object left = Evaluate(expr.left);
        object right = Evaluate(expr.right);

        switch(expr.op.type) {
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.GREATER:
                CheckNumberOperands(expr.op, left, right);
                return (double) left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.op, left, right);
                return (double) left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperands(expr.op, left, right);
                return (double) left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.op, left, right);
                return (double) left <= (double)right;
            case TokenType.MINUS:
                CheckNumberOperands(expr.op, left, right);
                return (double)left - (double)right;
            case TokenType.SLASH:
                CheckNumberOperands(expr.op, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperands(expr.op, left, right);
                return (double)left * (double)right;
            case TokenType.PLUS:
                if(left is double && right is double) {
                    return (double)left + (double)right;
                }
                if(left is string && right is string) {
                    return (string)left + (string)right;
                }
                throw new RuntimeException(expr.op, "Operands must be two numbers or two strings.");
        }

        // Unreachable
        return null;
    }
    public bool IsEqual(object left, object right) {
        if(left == null && right == null) return true;
        if(left == null) return false;
        return left.Equals(right);
    }

    public object VisitLogicalExpr(Expr.Logical expr) {
        object left = Evaluate(expr.left);
        if(expr.op.type == TokenType.OR) {
            if(IsTruthy(left)) return left;
        } else {
            if(!IsTruthy(left)) return left;
        }
        return Evaluate(expr.right);
    }

    public object VisitSetExpr(Expr.Set expr) {
        object obj = Evaluate(expr.obj);
        if(!(obj is LoxInstance)) {
            throw new RuntimeException(expr.name, "Only instances have fields.");
        }

        object value = Evaluate(expr.value);
        (obj as LoxInstance).Set(expr.name, value);
        return value;
    }

    public object VisitThisExpr(Expr.This expr) {
        return LookUpVariable(expr.keyword, expr);
    }

    public object VisitUnaryExpr(Expr.Unary expr) {
        object right = Evaluate(expr.right);
        switch(expr.op.type) {
            case TokenType.MINUS:
                CheckNumberOperand(expr.op, right);
                return -(double)right;
            case TokenType.BANG:
                return !IsTruthy(right);
        }
        return null;
    }

    public object VisitCallExpr(Expr.Call expr) {
        object callee = Evaluate(expr.callee);
        List<object> arguments = new List<object>();
        foreach(var argument in expr.arguments) {
            arguments.Add(Evaluate(argument));
        } 
        if(!(callee is LoxCallable)) {
            throw new RuntimeException(expr.paren, "Can only call functions and constructors.");
        }
        LoxCallable function = (LoxCallable)callee;
        if(arguments.Count != function.Arity()) {
            throw new RuntimeException(expr.paren, $"Expected {function.Arity()} arguments, but got {arguments.Count}.");
        }
        return function.Call(this, arguments);
    }

    public object VisitGetExpr(Expr.Get expr) {
        object obj = Evaluate(expr.obj);
        if(obj is LoxInstance) {
            return (obj as LoxInstance).Get(expr.name);
        }
        throw new RuntimeException(expr.name, "Only instances have properties.");
    }

    public object VisitVariableExpr(Expr.Variable expr) {
        return LookUpVariable(expr.name, expr);
    }

    private object LookUpVariable(Token name, Expr expr) {
        if(locals.ContainsKey(expr)) {
            return environment.GetAt(locals[expr], name.lexeme);
        } else {
            return globals.Get(name);
        }
    }

    private void CheckNumberOperands(Token op, object left, object right) {
        if(left is double && right is double) return;
        throw new RuntimeException(op, "Operands must be numbers.");
    }
    private void CheckNumberOperand(Token op, object operand) {
        if(operand is double) return;
        throw new RuntimeException(op, "Operand must be a number.");
    }

    private bool IsTruthy(object obj) {
        if(obj == null) return false;
        if(obj is bool) return (bool)obj;
        return true;
    }
    public object VisitGroupingExpr(Expr.Grouping expr) {
        return Evaluate(expr.expression);
    }

    private object Evaluate(Expr expr) {
        return expr.Accept(this);
    }
    public object VisitLiteralExpr(Expr.Literal expr) {
        return expr.value;
    }
}