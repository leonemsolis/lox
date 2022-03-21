using System.Collections.Generic;

public abstract class Expr {
	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		T visitAssignExpr(Assign expr);
		T visitBinaryExpr(Binary expr);
		T visitGroupingExpr(Grouping expr);
		T visitLiteralExpr(Literal expr);
		T visitLogicalExpr(Logical expr);
		T visitUnaryExpr(Unary expr);
		T visitVariableExpr(Variable expr);
	}
	public class Assign : Expr {
		public Token name;
		public Expr value;

		public Assign(Token name, Expr value) {
			this.name = name;
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitAssignExpr(this);
		}
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

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitBinaryExpr(this);
		}
	}
	public class Grouping : Expr {
		public Expr expression;

		public Grouping(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitGroupingExpr(this);
		}
	}
	public class Literal : Expr {
		public object value;

		public Literal(object value) {
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitLiteralExpr(this);
		}
	}
	public class Logical : Expr {
		public Expr left;
		public Token op;
		public Expr right;

		public Logical(Expr left, Token op, Expr right) {
			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitLogicalExpr(this);
		}
	}
	public class Unary : Expr {
		public Token op;
		public Expr right;

		public Unary(Token op, Expr right) {
			this.op = op;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitUnaryExpr(this);
		}
	}
	public class Variable : Expr {
		public Token name;

		public Variable(Token name) {
			this.name = name;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitVariableExpr(this);
		}
	}
}
