using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private Environment _environment;
        private readonly Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();

        public Environment Globals { get; }

        public Interpreter()
        {
            Globals = new Environment();
            _environment = Globals;
            DefineNativeFunctions();
        }

        private void DefineNativeFunctions()
        {
            Globals.Define("clock", new NativeFunctions.Clock());
            Globals.Define("exit", new NativeFunctions.Exit());
        }


        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            } catch (RuntimeError err)
            {
                Lox.RuntimeError(err);
            }
        }

        public void Resolve(Expr expr, int depth)
        {
            _locals.Add(expr, depth);
        }


        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }


        object Expr.IVisitor<object>.Visit(Expr.Assign _assign)
        {
            object value = Evaluate(_assign.value);

            if (_locals.TryGetValue(_assign, out int distance))
            {
                _environment.AssignAt(distance, _assign.name, value);
            }
            else
            {
                Globals.Assign(_assign.name, value);
            }

            return value;
        }

        object Expr.IVisitor<object>.Visit(Expr.Binary _binary)
        {
            object left = Evaluate(_binary.left);
            object right = Evaluate(_binary.right);

            switch (_binary.opp.type)
            {
                case TokenType.BANG_EQUAL:
                    CheckNumberOperand(_binary.opp, right);
                    return !IsEqual(left, right);

                case TokenType.EQUAL_EQUAL:
                    CheckNumberOperand(_binary.opp, right);
                    return IsEqual(left, right);

                case TokenType.GREATER:
                    CheckNumberOperand(_binary.opp, right);
                    return (double)left > (double)right;

                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(_binary.opp, right);
                    return (double)left >= (double)right;

                case TokenType.LESS:
                    CheckNumberOperand(_binary.opp, right);
                    return (double)left < (double)right;

                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(_binary.opp, right);
                    return (double)left <= (double)right;

                case TokenType.MINUS:
                    CheckNumberOperands(_binary.opp, left, right);
                    return (double)left - (double)right;

                case TokenType.PLUS:
                    if (left is double && right is double) return (double)left + (double)right;
                    if (left is string && right is string) return (string)left + (string)right;
                    if (left is string && right is double) return (string)left + ((double)right).ToString();
                    throw new RuntimeError(_binary.opp, "Operands must be two numbers or two strings.");

                case TokenType.SLASH:
                    CheckNumberOperand(_binary.opp, right);
                    if ((double)right == 0) throw new RuntimeError(_binary.opp, "Cannot divide by zero.");
                    return (double)left / (double)right;

                case TokenType.STAR:
                    CheckNumberOperand(_binary.opp, right);
                    return (double)left * (double)right;
            }

            //unreachable
            return null;
        }

        object Expr.IVisitor<object>.Visit(Expr.Call _call)
        {
            object callee = Evaluate(_call.callee);

            List<object> arguments = new List<object>();
            foreach (Expr argument in _call.arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeError(_call.paren, "Can only call functions and classes.");
            }

            ICallable function = (ICallable)callee;
            if (arguments.Count != function.Arity)
            {
                throw new RuntimeError(_call.paren, "Expected " + function.Arity + " arguments, but got " + arguments.Count + ".");
            }



            return function.Call(this, arguments);
        }

         object Expr.IVisitor<object>.Visit(Expr.Get _get)
        {
            object target = Evaluate(_get.target);
            if (target is LoxInstance)
            {
                return ((LoxInstance)target).Get(_get.name);
            }

            throw new RuntimeError(_get.name, "Only instances have properties.");
        }

         object Expr.IVisitor<object>.Visit(Expr.Grouping _grouping)
        {
            return Evaluate(_grouping.expression);
        }

         object Expr.IVisitor<object>.Visit(Expr.Literal _literal)
        {
            return _literal.value;
        }

         object Expr.IVisitor<object>.Visit(Expr.Logical _logical)
        {
            object left = Evaluate(_logical.left);

            if (_logical.opp.type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            } else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(_logical.right);
        }

         object Expr.IVisitor<object>.Visit(Expr.Set _set)
        {
            object target = Evaluate(_set.target);

            if (!(target is LoxInstance))
            {
                throw new RuntimeError(_set.name, "Only instances have fields.");
            }

            object value = Evaluate(_set.value);
            ((LoxInstance)target).Set(_set.name, value);

            return value;
        }

         object Expr.IVisitor<object>.Visit(Expr.Super _super)
        {
            throw new NotImplementedException();
        }

         object Expr.IVisitor<object>.Visit(Expr.This _this)
        {
            return LookUpVariable(_this.keyword, _this);
        }

         object Expr.IVisitor<object>.Visit(Expr.Prefix _prefix)
        {
            object right = Evaluate(_prefix.right);

            switch (_prefix.opp.type)
            {
                case TokenType.MINUS:
                    CheckNumberOperand(_prefix.opp, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
                default:
                    break;
            }

            //unreachable
            return null;
        }

         object Expr.IVisitor<object>.Visit(Expr.Postfix _postfix)
        {
            object left = Evaluate(_postfix.left);

            CheckNumberOperand(_postfix.opp, left);

            switch (_postfix.opp.type)
            {
                case TokenType.PLUS_PLUS:
                    return (double)left + 1;
                case TokenType.MINUS_MINUS:
                    return (double)left - 1;
                default:
                    break;
            }

            return null;
        }

         object Expr.IVisitor<object>.Visit(Expr.Conditional _conditional)
        {
            throw new NotImplementedException();
        }

         object Expr.IVisitor<object>.Visit(Expr.Variable _variable)
        {
            return LookUpVariable(_variable.name, _variable);
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            if (_locals.TryGetValue(expr, out int distance))
            {
                return _environment.GetAt(distance, name.lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text[0..(text.Length - 2)];
                }
                return text;
            }

            return obj.ToString();
        }

        private void CheckNumberOperand(Token opp, object operand)
        {
            if (operand is double) return;

            throw new RuntimeError(opp, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token opp, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeError(opp, "Operands must be numbers.");
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Expression _expression)
        {
            Evaluate(_expression.expression);
            
            return null;
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Print _print)
        {
            object value = Evaluate(_print.expression);
            Console.WriteLine(Stringify(value));

            return null;
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Var _var)
        {
            object value = null;
            if (_var.initializer != null)
            {
                value = Evaluate(_var.initializer);
            }

            _environment.Define(_var.name.lexeme, value);
            return null;
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Block _block)
        {
            ExecuteBlock(_block.statements, new Environment(_environment));
            return null;
        }

        object Stmt.IVisitor<object>.Visit(Stmt.If _if)
        {
            if (IsTruthy(Evaluate(_if.condition)))
            {
                Execute(_if.thenBranch);
            }
            else if (_if.elseBranch != null)
            {
                Execute(_if.elseBranch);
            }
            return null;
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this._environment;

            try
            {
                _environment = environment;

                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            } 
            finally
            {
                _environment = previous;
            }
        }

        object Stmt.IVisitor<object>.Visit(Stmt.While _while)
        {
            while (IsTruthy(Evaluate(_while.condition)))
            {
                Execute(_while.body);
            }

            return null;
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Function _function)
        {
            LoxFunction function = new LoxFunction(_function, _environment, false);
            _environment.Define(_function.name.lexeme, function);

            return null;
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Return _return)
        {
            object value = null;

            if (_return.value != null) value = Evaluate(_return.value);

            throw new Return(value);
        }

        object Stmt.IVisitor<object>.Visit(Stmt.Class _class)
        {
            _environment.Define(_class.name.lexeme, null);

            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            foreach (Stmt.Function method in _class.methods)
            {
                LoxFunction function = new LoxFunction(method, _environment, method.name.lexeme.Equals("init"));
                methods.Add(method.name.lexeme, function);
            }


            LoxClass klass = new LoxClass(_class.name.lexeme, methods);
            _environment.Assign(_class.name, klass);

            return null;
        }
    }
}
