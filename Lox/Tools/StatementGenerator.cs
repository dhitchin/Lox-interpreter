 
using System.Collections.Generic;
namespace Lox
{
	public abstract class Stmt
	{


	public interface IVisitor<T> 
		{
            T Visit(Block _block);
            T Visit(Expression _expression);
            T Visit(Function _function);
            T Visit(If _if);
            T Visit(Print _print);
            T Visit(Return _return);
            T Visit(Var _var);
            T Visit(While _while);
		}
 
        public class Block : Stmt
        {
            public List<Stmt> statements;
             
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Expression : Stmt
        {
            public Expr expression;
             
            public Expression(Expr expression)
            {
                this.expression = expression;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Function : Stmt
        {
            public Token name;
            public List<Token> parameters;
            public List<Stmt> body;
             
            public Function(Token name, List<Token> parameters, List<Stmt> body)
            {
                this.name = name;
                this.parameters = parameters;
                this.body = body;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class If : Stmt
        {
            public Expr condition;
            public Stmt thenBranch;
            public Stmt elseBranch;
             
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Print : Stmt
        {
            public Expr expression;
             
            public Print(Expr expression)
            {
                this.expression = expression;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Return : Stmt
        {
            public Token keyword;
            public Expr value;
             
            public Return(Token keyword, Expr value)
            {
                this.keyword = keyword;
                this.value = value;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Var : Stmt
        {
            public Token name;
            public Expr initializer;
             
            public Var(Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class While : Stmt
        {
            public Expr condition;
            public Stmt body;
             
            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
		/// <summary>
		/// Base function for visiting our trees.
		/// </summary> 
		public abstract T Accept<T>(IVisitor<T> visitor);
	}
}


