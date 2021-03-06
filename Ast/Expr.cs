using System.Collections.Generic;

public abstract class Expr {
	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		T VisitAssignExpr(Assign expr);
		T VisitBinaryExpr(Binary expr);
		T VisitCallExpr(Call expr);
		T VisitGetExpr(Get expr);
		T VisitGroupingExpr(Grouping expr);
		T VisitLiteralExpr(Literal expr);
		T VisitLogicalExpr(Logical expr);
		T VisitSetExpr(Set expr);
		T VisitSuperExpr(Super expr);
		T VisitThisExpr(This expr);
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

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitAssignExpr(this);
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
			return visitor.VisitBinaryExpr(this);
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

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitCallExpr(this);
		}
	}
	public class Get : Expr {
		public Expr obj;
		public Token name;

		public Get(Expr obj, Token name) {
			this.obj = obj;
			this.name = name;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitGetExpr(this);
		}
	}
	public class Grouping : Expr {
		public Expr expression;

		public Grouping(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitGroupingExpr(this);
		}
	}
	public class Literal : Expr {
		public object value;

		public Literal(object value) {
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitLiteralExpr(this);
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
			return visitor.VisitLogicalExpr(this);
		}
	}
	public class Set : Expr {
		public Expr obj;
		public Token name;
		public Expr value;

		public Set(Expr obj, Token name, Expr value) {
			this.obj = obj;
			this.name = name;
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitSetExpr(this);
		}
	}
	public class Super : Expr {
		public Token keyword;
		public Token method;

		public Super(Token keyword, Token method) {
			this.keyword = keyword;
			this.method = method;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitSuperExpr(this);
		}
	}
	public class This : Expr {
		public Token keyword;

		public This(Token keyword) {
			this.keyword = keyword;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitThisExpr(this);
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
			return visitor.VisitUnaryExpr(this);
		}
	}
	public class Variable : Expr {
		public Token name;

		public Variable(Token name) {
			this.name = name;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitVariableExpr(this);
		}
	}
}
