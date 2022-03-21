using System;
using System.Collections.Generic;
public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object> {
    private static Environment globals = new Environment();
    private Environment environment = globals;
    public Interpreter() {
        new BuiltInClock().DefineInEnvironment(globals, "clock");
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
    
    public object visitBlockStmt(Stmt.Block stmt) {
        ExecuteBlock(stmt.statements, new Environment(environment));
        return null;
    }
    private void ExecuteBlock(List<Stmt>statements, Environment environment) {
        Environment previous = this.environment;
        try {
            this.environment = environment;
            foreach(var statement in statements) {
                Execute(statement);
            }
        } finally {
            this.environment = previous;
        }
    }
    public object visitVarStmt(Stmt.Var stmt) {
        object value = null;
        if(stmt.initializer != null) {
            value = Evaluate(stmt.initializer);
        }
        environment.Define(stmt.name.lexeme, value);
        return null;
    }

    public object visitExpressionStmt(Stmt.Expression stmt) {
        Evaluate(stmt.expression);
        return null;
    }

    public object visitWhileStmt(Stmt.While stmt) {
        while(IsTruthy(Evaluate(stmt.condition))) {
            Execute(stmt.body);
        }
        return null;
    }
    public object visitIfStmt(Stmt.If stmt) {
        if(IsTruthy(Evaluate(stmt.condition))) {
            Execute(stmt.thenBranch);
        } else if(stmt.elseBranch != null) {
            Execute(stmt.elseBranch);
        }
        return null;
    }
    public object visitPrintStmt(Stmt.Print stmt) {
        object result = Evaluate(stmt.expression);
        Console.WriteLine(result == null ? "nil" : result);
        return null;
    }
    public object visitAssignExpr(Expr.Assign expr) {
        object value = Evaluate(expr.value);
        environment.Assign(expr.name, value);
        return value;
    }
    public object visitBinaryExpr(Expr.Binary expr) {
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

    public object visitLogicalExpr(Expr.Logical expr) {
        object left = Evaluate(expr.left);
        if(expr.op.type == TokenType.OR) {
            if(IsTruthy(left)) return left;
        } else {
            if(!IsTruthy(left)) return left;
        }
        return Evaluate(expr.right);
    }

    public object visitUnaryExpr(Expr.Unary expr) {
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

    public object visitCallExpr(Expr.Call expr) {
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

    public object visitVariableExpr(Expr.Variable variable) {
        return environment.Get(variable.name); 
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
    public object visitGroupingExpr(Expr.Grouping expr) {
        return Evaluate(expr.expression);
    }

    private object Evaluate(Expr expr) {
        return expr.Accept(this);
    }
    public object visitLiteralExpr(Expr.Literal expr) {
        return expr.value;
    }
}