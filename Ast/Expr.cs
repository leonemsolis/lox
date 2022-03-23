using System.Collections.Generic;

public abstract class Expr {
	public abstract T Accept<T>(Visitor<T> Visitor);

	public interface Visitor<T> {
		T VisitAssignExpr(Assign expr);
		T VisitBinaryExpr(Binary expr);
		T VisitCallExpr(Call expr);
		T VisitGroupingExpr(Grouping expr);
		T VisitLiteralExpr(Literal expr);
		T VisitLogicalExpr(Logical expr);
		T VisitUnaryExpr(Unary expr);
		T VisitVariableExpr(Variable expr);
	}
	public class Assign : Expr {
		public Token name;
		public Expr value;

		public Assign(Token name, Expr value) {
			this.name = name;
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitAssignExpr(this);
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

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitBinaryExpr(this);
		}
	}
	public class Call : Expr {
		public Expr callee;
		public Token paren;
		public List<Expr> arguments;

		public Call(Expr callee, Token paren, List<Expr> arguments) {
			this.callee = callee;
			this.paren = paren;
			this.arguments = arguments;
		}

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitCallExpr(this);
		}
	}
	public class Grouping : Expr {
		public Expr expression;

		public Grouping(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitGroupingExpr(this);
		}
	}
	public class Literal : Expr {
		public object value;

		public Literal(object value) {
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitLiteralExpr(this);
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

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitLogicalExpr(this);
		}
	}
	public class Unary : Expr {
		public Token op;
		public Expr right;

		public Unary(Token op, Expr right) {
			this.op = op;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitUnaryExpr(this);
		}
	}
	public class Variable : Expr {
		public Token name;

		public Variable(Token name) {
			this.name = name;
		}

		public override T Accept<T>(Visitor<T> Visitor) {
			return Visitor.VisitVariableExpr(this);
		}
	}
}
