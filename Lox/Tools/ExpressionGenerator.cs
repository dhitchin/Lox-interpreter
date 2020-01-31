 
using System.Collections.Generic;
namespace Lox
{
	public abstract class Expr
	{

		public interface IVisitor<T> 
		{
            T Visit(Assign _assign);
            T Visit(Binary _binary);
            T Visit(Call _call);
            T Visit(Get _get);
            T Visit(Grouping _grouping);
            T Visit(Literal _literal);
            T Visit(Logical _logical);
            T Visit(Set _set);
            T Visit(Super _super);
            T Visit(This _this);
            T Visit(Prefix _prefix);
            T Visit(Postfix _postfix);
            T Visit(Conditional _conditional);
            T Visit(Variable _variable);
		}
 
        public class Assign : Expr
        {
            public Token name;
            public Expr value;
             
            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Binary : Expr
        {
            public Expr left;
            public Token opp;
            public Expr right;
             
            public Binary(Expr left, Token opp, Expr right)
            {
                this.left = left;
                this.opp = opp;
                this.right = right;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Call : Expr
        {
            public Expr callee;
            public Token paren;
            public List<Expr> arguments;
             
            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Get : Expr
        {
            public Expr target;
            public Token name;
             
            public Get(Expr target, Token name)
            {
                this.target = target;
                this.name = name;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr expression;
             
            public Grouping(Expr expression)
            {
                this.expression = expression;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Literal : Expr
        {
            public object value;
             
            public Literal(object value)
            {
                this.value = value;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Logical : Expr
        {
            public Expr left;
            public Token opp;
            public Expr right;
             
            public Logical(Expr left, Token opp, Expr right)
            {
                this.left = left;
                this.opp = opp;
                this.right = right;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Set : Expr
        {
            public Expr target;
            public Token name;
            public Expr value;
             
            public Set(Expr target, Token name, Expr value)
            {
                this.target = target;
                this.name = name;
                this.value = value;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Super : Expr
        {
            public Token keyword;
            public Token method;
             
            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class This : Expr
        {
            public Token keyword;
             
            public This(Token keyword)
            {
                this.keyword = keyword;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Prefix : Expr
        {
            public Token opp;
            public Expr right;
             
            public Prefix(Token opp, Expr right)
            {
                this.opp = opp;
                this.right = right;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Postfix : Expr
        {
            public Token opp;
            public Expr left;
             
            public Postfix(Token opp, Expr left)
            {
                this.opp = opp;
                this.left = left;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Conditional : Expr
        {
            public Expr expression;
            public Expr thenBranch;
            public Expr elseBranch;
             
            public Conditional(Expr expression, Expr thenBranch, Expr elseBranch)
            {
                this.expression = expression;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }
             
            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Variable : Expr
        {
            public Token name;
             
            public Variable(Token name)
            {
                this.name = name;
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


