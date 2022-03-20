public abstract class Stmt {
	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		T visitExpressionStmt(Expression stmt);
		T visitPrintStmt(Print stmt);
		T visitVarStmt(Var stmt);
	}
	public class Expression : Stmt {
		public Expr expression;

		public Expression(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitExpressionStmt(this);
		}
	}
	public class Print : Stmt {
		public Expr expression;

		public Print(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitPrintStmt(this);
		}
	}
	public class Var : Stmt {
		public Token name;
		public Expr initializer;

		public Var(Token name, Expr initializer) {
			this.name = name;
			this.initializer = initializer;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitVarStmt(this);
		}
	}
}
