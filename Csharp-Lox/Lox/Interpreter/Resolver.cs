using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        private readonly Interpreter _interpreter;
        private readonly Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType _currentFunction = FunctionType.NONE;
        private ClassType _currentClass = ClassType.NONE;

        public Resolver(Interpreter interpreter)
        {
            this._interpreter = interpreter;
        }

        #region "Private Helpers"
        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }


        public void Resolve(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = _scopes.Count - 1; i >=0; i--)
            {
                if (_scopes.ToArray()[i].ContainsKey(name.lexeme))
                {
                    _interpreter.Resolve(expr, _scopes.Count - 1 - i);
                    return;
                }
            }

            //Not found locally.  Assume global.
        }

        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = _currentFunction;
            _currentFunction = type;
            
            BeginScope();
            foreach (Token param in function.parameters)
            {
                Declare(param);
                Define(param);
            }

            Resolve(function.body);
            EndScope();

            _currentFunction = enclosingFunction;
        }

        private void Declare(Token name)
        {
            if (_scopes.Count == 0) return;

            Dictionary<string, bool> scope = _scopes.Peek();
            if (scope.ContainsKey(name.lexeme))
            {
                Lox.Error(name, "Variable with this name already declared in this scope.");
            }
            else
            {
                scope.Add(name.lexeme, false);
            }            
        }

        private void Define(Token name)
        {
            if (_scopes.Count == 0) return;

            if (_scopes.Peek().ContainsKey(name.lexeme))
            {
                _scopes.Peek()[name.lexeme] = true;
            }
            else
            {
                Lox.Error(name, "Variable has not been declared.");
            }
            
        }

        private void EndScope()
        {
            _scopes.Pop();
        }


        #endregion

        #region "Statement Visitors"
        public object Visit(Stmt.Block _block)
        {
            BeginScope();
            Resolve(_block.statements);
            EndScope();

            return null;
        }

        public object Visit(Stmt.Class _class)
        {
            ClassType enclosingClass = _currentClass;
            _currentClass = ClassType.CLASS;

            Declare(_class.name);
            Define(_class.name);

            if ((_class.superclass != null) && 
                _class.name.lexeme.Equals(_class.superclass.name.lexeme)) {
                Lox.Error(_class.superclass.name, "A class cannot inherit from itself.");
            }

            if (_class.superclass != null)
            {
                _currentClass = ClassType.SUBCLASS;
                Resolve(_class.superclass);
            }

            if (_class.superclass != null)
            {
                BeginScope();
                _scopes.Peek().Add("super", true);
            }

            BeginScope();
            _scopes.Peek().Add("this", true);

            foreach (Stmt.Function method in _class.methods)
            {
                FunctionType declaration = (method.name.lexeme.Equals("init")) ? FunctionType.INITIALIZER : FunctionType.METHOD;
                ResolveFunction(method, declaration);
            }

            EndScope();

            if (_class.superclass != null) EndScope();

            _currentClass = enclosingClass;
            return null;
        }

        public object Visit(Stmt.Expression _expression)
        {
            Resolve(_expression.expression);
            
            return null;
        }

        public object Visit(Stmt.Function _function)
        {
            Declare(_function.name);
            Define(_function.name);

            ResolveFunction(_function, FunctionType.FUNCTION);
            
            return null;
        }

        public object Visit(Stmt.If _if)
        {
            Resolve(_if.condition);
            Resolve(_if.thenBranch);
            if (_if.elseBranch != null) Resolve(_if.elseBranch);

            return null;
        }

        public object Visit(Stmt.Print _print)
        {
            Resolve(_print.expression);

            return null;
        }

        public object Visit(Stmt.Return _return)
        {
            if (_currentFunction == FunctionType.NONE)
            {
                Lox.Error(_return.keyword, "Cannot return from top-level code.");
            }

            if  (_return.value != null)
            {
                if (_currentFunction == FunctionType.INITIALIZER)
                {
                    Lox.Error(_return.keyword, "Cannot return a value from an initializer.");
                }
                Resolve(_return.value);
            }

            return null;
        }

        public object Visit(Stmt.Var _var)
        {
            Declare(_var.name);
            if (_var.initializer != null)
            {
                Resolve(_var.initializer);
            }
            Define(_var.name);

            return null;
        }

        public object Visit(Stmt.While _while)
        {
            Resolve(_while.condition);
            Resolve(_while.body);

            return null;
        }
#endregion

        #region "Expression Visitors"
        public object Visit(Expr.Assign _assign)
        {
            Resolve(_assign.value);
            ResolveLocal(_assign, _assign.name);

            return null;
        }

        public object Visit(Expr.Binary _binary)
        {
            Resolve(_binary.left);
            Resolve(_binary.right);

            return null;
        }

        public object Visit(Expr.Call _call)
        {
            Resolve(_call.callee);

            foreach(Expr argument in _call.arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public object Visit(Expr.Get _get)
        {
            Resolve(_get.target);

            return null;
        }

        public object Visit(Expr.Grouping _grouping)
        {
            Resolve(_grouping.expression);

            return null;
        }

        public object Visit(Expr.Literal _literal)
        {
            return null;
        }

        public object Visit(Expr.Logical _logical)
        {
            Resolve(_logical.left);
            Resolve(_logical.right);

            return null;
        }

        public object Visit(Expr.Set _set)
        {
            Resolve(_set.value);
            Resolve(_set.target);

            return null;
        }

        public object Visit(Expr.Super _super)
        {
            if (_currentClass == ClassType.NONE)
            {
                Lox.Error(_super.keyword, "Cannot use 'super' outside of a class.");
            } 
            else if (_currentClass != ClassType.SUBCLASS)
            {
                Lox.Error(_super.keyword, "Cannot use 'super' in a class with no superclass.");
            }
            
            
            ResolveLocal(_super, _super.keyword);

            return null;
        }

        public object Visit(Expr.This _this)
        {
            ResolveLocal(_this, _this.keyword);

            return null;
        }

        public object Visit(Expr.Prefix _prefix)
        {
            Resolve(_prefix.right);

            return null;
        }

        public object Visit(Expr.Postfix _postfix)
        {
            Resolve(_postfix.left);

            return null;
        }

        public object Visit(Expr.Conditional _conditional)
        {
            throw new NotImplementedException();
        }

        public object Visit(Expr.Variable _variable)
        {
            if (_scopes.Count != 0 && !_scopes.Peek()[_variable.name.lexeme])
            {
                Lox.Error(_variable.name, "Cannot read local variable in its own initializer.");
            }

            ResolveLocal(_variable, _variable.name);

            return null;
        }
        #endregion
    }
}
