// public class RPNPrintner : Expr.Visitor<string> {
//     public string Print(Expr expr) {
//         return expr.Accept(this);
//     }

//     public string visi

//     public string visitBinaryExpr(Expr.Binary expr) {
//         return Parenthesize(expr.op.lexeme, expr.left, expr.right);
//     }

//     public string visitGroupingExpr(Expr.Grouping expr) {
//         return Parenthesize("group", expr.expression);
//     }

//     public string visitLiteralExpr(Expr.Literal expr) {
//         if(expr.value == null) return "nil";
//         return expr.value.ToString();
//     }

//     public string visitUnaryExpr(Expr.Unary expr) {
//         return Parenthesize(expr.op.lexeme, expr.right);
//     }

//     public string visitVariableExpr(Expr.Variable expr) {
//         return expr.name.lexeme;
//     }

//     public string Parenthesize(string name, params Expr[] exprs) {
//         var result = "";
//         // result += "(";
//         foreach(var expr in exprs) {
//             result += " ";
//             result += expr.Accept(this);
//         }
//         // result += " " + name + ")";
//         result += " " + name;
//         return result;
//     }
// }