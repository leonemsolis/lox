using System.Collections.Generic;

public abstract class Stmt {
	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		T visitBlockStmt(Block stmt);
		T visitExpressionStmt(Expression stmt);
		T visitFunctionStmt(Function stmt);
		T visitIfStmt(If stmt);
		T visitPrintStmt(Print stmt);
		T visitReturnStmt(Return stmt);
		T visitVarStmt(Var stmt);
		T visitWhileStmt(While stmt);
	}
	public class Block : Stmt {
		public List<Stmt> statements;

		public Block(List<Stmt> statements) {
			this.statements = statements;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitBlockStmt(this);
		}
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
	public class Function : Stmt {
		public Token name;
		public List<Token> parameters;
		public List<Stmt> body;

		public Function(Token name, List<Token> parameters, List<Stmt> body) {
			this.name = name;
			this.parameters = parameters;
			this.body = body;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitFunctionStmt(this);
		}
	}
	public class If : Stmt {
		public Expr condition;
		public Stmt thenBranch;
		public Stmt elseBranch;

		public If(Expr condition, Stmt thenBranch, Stmt elseBranch) {
			this.condition = condition;
			this.thenBranch = thenBranch;
			this.elseBranch = elseBranch;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitIfStmt(this);
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
	public class Return : Stmt {
		public Token keyword;
		public Expr value;

		public Return(Token keyword, Expr value) {
			this.keyword = keyword;
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitReturnStmt(this);
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
	public class While : Stmt {
		public Expr condition;
		public Stmt body;

		public While(Expr condition, Stmt body) {
			this.condition = condition;
			this.body = body;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.visitWhileStmt(this);
		}
	}
}
