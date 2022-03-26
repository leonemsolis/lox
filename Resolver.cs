/**
Resolves closure variables:

var a = "global";
{
    fun showA() {
        print(a);
    }
    showA();
    a = "block";
    showA();
}
**/

using System;
using System.Collections.Generic;
public class Resolver : Expr.Visitor<object>, Stmt.Visitor<object> {
    private enum FunctionType {NONE, FUNCTION, METHOD};
    private enum ClassType {NONE, CLASS};
    private ClassType currentClass = ClassType.NONE;
    private Interpreter interpreter;

    // bool - variable has been resolved
    private Stack<Dictionary<string, bool>>scopes = new Stack<Dictionary<string, bool>>();
    private FunctionType currentFunction = FunctionType.NONE;
    public Resolver(Interpreter interpreter) {
        this.interpreter = interpreter;
    }

    // Nodes with var/fun declaration/usage

    public object VisitBlockStmt(Stmt.Block stmt) {
        BeginScope();
        Resolve(stmt.statements);
        EndScope();
        return null;
    }

    public object VisitClassStmt(Stmt.Class stmt) {
        ClassType enclosingClass = currentClass;
        currentClass = ClassType.CLASS;
        Declare(stmt.name);
        BeginScope();
        scopes.Peek()["this"] = true;
        foreach(var method in stmt.methods) {
            FunctionType declaration = FunctionType.METHOD;
            ResolveFunction(method, declaration);
        }
        EndScope();
        currentClass = enclosingClass;
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt) {
        Declare(stmt.name);
        if(stmt.initializer != null) {
            Resolve(stmt.initializer);
        }
        Define(stmt.name);
        return null;
    }

    public object VisitVariableExpr(Expr.Variable expr) {
        if(scopes.Count != 0) {
            var scope = scopes.Peek();
            if(scope.ContainsKey(expr.name.lexeme) && scope[expr.name.lexeme] == false) {
                Lox.Error(expr.name, "Can't read local variable in it's own initializer.");
            }
        }
        ResolveLocal(expr, expr.name);
        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr) {
        Resolve(expr.value);
        ResolveLocal(expr, expr.name);
        return null;
    }

    public object VisitFunctionStmt(Stmt.Function stmt) {
        Declare(stmt.name);
        Define(stmt.name);
        ResolveFunction(stmt, FunctionType.FUNCTION);

        return null;
    } 


    // Helper methods

    private void ResolveFunction(Stmt.Function function, FunctionType functionType) {
        FunctionType enclosingFunction = currentFunction;
        currentFunction = functionType;
        BeginScope();
        foreach(var param in function.parameters) {
            Declare(param);
            Define(param);
        }
        Resolve(function.body);
        EndScope();
        currentFunction = enclosingFunction;
    }

    private void ResolveLocal(Expr expr, Token name) {
        var s = scopes.ToArray();
        for(int i = s.Length - 1; i >=0; i--) {
            if(s[i].ContainsKey(name.lexeme)) {
                interpreter.Resolve(expr, s.Length - 1 - i);
                return;
            }
        }
    }

    private void Declare(Token name) {
        if(scopes.Count == 0) return;
        // Variable start resolving
        var scope = scopes.Peek();
        if(scope.ContainsKey(name.lexeme)) {
            Lox.Error(name, "Already a variable with this name in this scope.");
        }
        scope[name.lexeme] = false;
    }

    private void Define(Token name) {
        if(scopes.Count == 0) return;
        // Variable finished resolving
        scopes.Peek()[name.lexeme] = true;
    }

    public void Resolve(List<Stmt> statements) {
        foreach(var statement in statements) {
            Resolve(statement);
        }
    }

    private void Resolve(Stmt statement) {
        statement.Accept(this);
    }

    private void Resolve(Expr expression) {
        expression.Accept(this);
    }

    private void BeginScope() {
        scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope() {
        scopes.Pop();
    }

    // Other statement nodes
    public object VisitExpressionStmt(Stmt.Expression stmt) {
        Resolve(stmt.expression);
        return null;
    }

    public object VisitIfStmt(Stmt.If stmt) {
        Resolve(stmt.condition);
        Resolve(stmt.thenBranch);
        if(stmt.elseBranch != null) Resolve(stmt.elseBranch);
        return null;
    }

    public object VisitReturnStmt(Stmt.Return stmt) {
        if(currentFunction == FunctionType.NONE) Lox.Error(stmt.keyword, "Can't return from top-level code.");
        if(stmt.value != null) Resolve(stmt.value);
        return null;
    }

    public object VisitWhileStmt(Stmt.While stmt) {
        Resolve(stmt.condition);
        Resolve(stmt.body);
        return null;
    }

    // Other expression nodes
    public object VisitBinaryExpr(Expr.Binary expr) {
        Resolve(expr.left);
        Resolve(expr.right);
        return null;
    }

    public object VisitCallExpr(Expr.Call expr) {
        Resolve(expr.callee);
        foreach(var argument in expr.arguments) Resolve(argument);
        return null;
    }

    public object VisitGetExpr(Expr.Get expr) {
        Resolve(expr.obj);
        return null;
    }

    public object VisitGroupingExpr(Expr.Grouping expr) {
        Resolve(expr.expression);
        return null;
    }

    public object VisitLiteralExpr(Expr.Literal expr) {
        return null;
    }

    public object VisitLogicalExpr(Expr.Logical expr) {
        Resolve(expr.left);
        Resolve(expr.right);
        return null;
    }

    public object VisitSetExpr(Expr.Set expr) {
        Resolve(expr.value);
        Resolve(expr.obj);
        return null;
    }

    public object VisitThisExpr(Expr.This expr) {
        if(currentClass == ClassType.NONE) {
            Lox.Error(expr.keyword, "Can't use 'this' outside of a class.");
            return null;
        }
        ResolveLocal(expr, expr.keyword);
        return null;
    }

    public object VisitUnaryExpr(Expr.Unary expr) {
        Resolve(expr.right);
        return null;
    }


}
