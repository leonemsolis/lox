public class Interpreter : Expr.Visitor<object> {

    public void Interpret(Expr expr) {
        try {
            object value = Evaluate(expr);
            System.Console.WriteLine(value.ToString());
        } catch(RuntimeException e) {
            Lox.RuntimeError(e);
        }
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
        return expr.accept(this);
    }
    public object visitLiteralExpr(Expr.Literal expr) {
        return expr.value;
    }
}