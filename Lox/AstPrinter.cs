using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    public class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }

        string Expr.IVisitor<string>.Visit(Expr.Assign _assign)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Binary _binary)
        {
            return Parenthesize(_binary.opp.lexeme, _binary.left, _binary.right);
        }

        string Expr.IVisitor<string>.Visit(Expr.Call _call)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Get _get)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Grouping _grouping)
        {
            return Parenthesize("group", _grouping.expression);
        }

        string Expr.IVisitor<string>.Visit(Expr.Literal _literal)
        {
            if (_literal.value == null) return "nil";
            return _literal.value.ToString();
        }

        string Expr.IVisitor<string>.Visit(Expr.Logical _logical)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Set _set)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Super _super)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.This _this)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Prefix _prefix)
        {
            return Parenthesize(_prefix.opp.lexeme, _prefix.right);
        }

        string Expr.IVisitor<string>.Visit(Expr.Postfix _postfix)
        {
            return Parenthesize(_postfix.opp.lexeme);
        }

        string Expr.IVisitor<string>.Visit(Expr.Conditional _conditional)
        {
            throw new NotImplementedException();
        }

        string Expr.IVisitor<string>.Visit(Expr.Variable _variable)
        {
            throw new NotImplementedException();
        }
    }
}
