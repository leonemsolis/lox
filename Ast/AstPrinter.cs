using System.Collections.Generic;
public class AstPrinter : Stmt.Visitor<string>, Expr.Visitor<string> {
    public void Print(List<Stmt> statements) {
        try{ 
            foreach(var statement in statements) {
                System.Console.WriteLine(GetString(statement));
            }
        } catch(RuntimeException e) {
            Lox.RuntimeError(e);
        }
    }

    private string GetString(Stmt statement) {
        return statement.Accept(this);
    }

    public string visitVarStmt(Stmt.Var stmt) {
        string value = null;
        if(stmt.initializer != null) {
            value = stmt.initializer.Accept(this);
        }
        return $"{stmt.name.lexeme} is {value}";
    }

    // TODO: if
    public string visitIfStmt(Stmt.If stmt) {
        return "IF";
    }
    // TODO: while
    public string visitWhileStmt(Stmt.While stmt) {
        return "WHILE";
    }
    public string visitExpressionStmt(Stmt.Expression stmt) {
        return stmt.expression.Accept(this);
    }

    public string visitBlockStmt(Stmt.Block stmt) {
        var result = "{";
        foreach(var statement in stmt.statements) {
            result += statement.Accept(this) + ";";
        }
        return result + "}";
    }

    public string visitPrintStmt(Stmt.Print stmt) {
        return "Print " + stmt.expression.Accept(this);
    }

    public string visitBinaryExpr(Expr.Binary expr) {
        return Parenthesize(expr.op.lexeme, expr.left, expr.right);
    }
    public string visitGroupingExpr(Expr.Grouping expr) {
        return Parenthesize("group", expr.expression);
    }
    public string visitLiteralExpr(Expr.Literal expr) {
        if(expr.value == null) return "nil";
        return expr.value.ToString();
    }
    // TODO: logical
    public string visitLogicalExpr(Expr.Logical expr) {
        return "";
    }
    public string visitUnaryExpr(Expr.Unary expr) {
        return Parenthesize(expr.op.lexeme, expr.right);
    }
    
    public string visitAssignExpr(Expr.Assign expr) {
        return $"Assign {expr.value.Accept(this)} to {expr.name.lexeme}";
    }

    public string visitVariableExpr(Expr.Variable expr) {
        return expr.name.lexeme;
    }

    private string Parenthesize(string name, params Expr[] exprs) {
        var result = "";
        result += "(" + name;
        foreach(var expr in exprs) {
            result += " ";
            result += expr.Accept(this);
        }
        result += ")";
        return result;
    }
}