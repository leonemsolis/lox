public abstract class Expr {
	public abstract T accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		T visitBinaryExpr(Binary expr);
		T visitGroupingExpr(Grouping expr);
		T visitLiteralExpr(Literal expr);
		T visitUnaryExpr(Unary expr);
	}
	public class Binary : Expr {
		public Expr left;
		public Token op;
		public Expr right;

		public Binary(Expr left, Token op, Expr right) {
			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override T accept<T>(Visitor<T> visitor) {
			return visitor.visitBinaryExpr(this);
		}
	}
	public class Grouping : Expr {
		public Expr expression;

		public Grouping(Expr expression) {
			this.expression = expression;
		}

		public override T accept<T>(Visitor<T> visitor) {
			return visitor.visitGroupingExpr(this);
		}
	}
	public class Literal : Expr {
		public object value;

		public Literal(object value) {
			this.value = value;
		}

		public override T accept<T>(Visitor<T> visitor) {
			return visitor.visitLiteralExpr(this);
		}
	}
	public class Unary : Expr {
		public Token op;
		public Expr right;

		public Unary(Token op, Expr right) {
			this.op = op;
			this.right = right;
		}

		public override T accept<T>(Visitor<T> visitor) {
			return visitor.visitUnaryExpr(this);
		}
	}
}
