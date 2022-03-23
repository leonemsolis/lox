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

    public string VisitVarStmt(Stmt.Var stmt) {
        string value = null;
        if(stmt.initializer != null) {
            value = stmt.initializer.Accept(this);
        }
        return $"{stmt.name.lexeme} is {value}";
    }

    // TODO: if
    public string VisitIfStmt(Stmt.If stmt) {
        return "IF";
    }
    // TODO: while
    public string VisitWhileStmt(Stmt.While stmt) {
        return "WHILE";
    }
    // TODO: function
    public string VisitFunctionStmt(Stmt.Function stmt) {
        return "FUNC";
    }
    // TODO: return
    public string VisitReturnStmt(Stmt.Return stmt) {
        return "RETURN";
    }
    public string VisitExpressionStmt(Stmt.Expression stmt) {
        return stmt.expression.Accept(this);
    }

    public string VisitBlockStmt(Stmt.Block stmt) {
        var result = "{";
        foreach(var statement in stmt.statements) {
            result += statement.Accept(this) + ";";
        }
        return result + "}";
    }

    // TODO: class
    public string VisitClassStmt(Stmt.Class stmt) {
        return "CLASS";
    } 

    public string VisitBinaryExpr(Expr.Binary expr) {
        return Parenthesize(expr.op.lexeme, expr.left, expr.right);
    }
    public string VisitGroupingExpr(Expr.Grouping expr) {
        return Parenthesize("group", expr.expression);
    }
    public string VisitLiteralExpr(Expr.Literal expr) {
        if(expr.value == null) return "nil";
        return expr.value.ToString();
    }
    // TODO: logical
    public string VisitLogicalExpr(Expr.Logical expr) {
        return "";
    }
    // TODO: Call
    public string VisitCallExpr(Expr.Call expr) {
        return "";
    }
    // TODO: Get
    public string VisitGetExpr(Expr.Get expr) {
        return "GET";
    }
    public string VisitUnaryExpr(Expr.Unary expr) {
        return Parenthesize(expr.op.lexeme, expr.right);
    }
    
    public string VisitAssignExpr(Expr.Assign expr) {
        return $"Assign {expr.value.Accept(this)} to {expr.name.lexeme}";
    }

    public string VisitVariableExpr(Expr.Variable expr) {
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