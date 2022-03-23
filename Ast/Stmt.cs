using System.Collections.Generic;

public abstract class Stmt {
	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		T VisitBlockStmt(Block stmt);
		T VisitClassStmt(Class stmt);
		T VisitExpressionStmt(Expression stmt);
		T VisitFunctionStmt(Function stmt);
		T VisitIfStmt(If stmt);
		T VisitReturnStmt(Return stmt);
		T VisitVarStmt(Var stmt);
		T VisitWhileStmt(While stmt);
	}
	public class Block : Stmt {
		public List<Stmt> statements;

		public Block(List<Stmt> statements) {
			this.statements = statements;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitBlockStmt(this);
		}
	}
	public class Class : Stmt {
		public Token name;
		public List<Stmt.Function> methods;

		public Class(Token name, List<Stmt.Function> methods) {
			this.name = name;
			this.methods = methods;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitClassStmt(this);
		}
	}
	public class Expression : Stmt {
		public Expr expression;

		public Expression(Expr expression) {
			this.expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitExpressionStmt(this);
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
			return visitor.VisitFunctionStmt(this);
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
			return visitor.VisitIfStmt(this);
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
			return visitor.VisitReturnStmt(this);
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
			return visitor.VisitVarStmt(this);
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
			return visitor.VisitWhileStmt(this);
		}
	}
}
