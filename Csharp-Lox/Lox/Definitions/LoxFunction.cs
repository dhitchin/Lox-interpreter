using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class LoxFunction : ICallable
    {
        private readonly Stmt.Function _declaration;
        private readonly Environment _closure;
        private readonly bool _isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            _closure = closure;
            _declaration = declaration;
            _isInitializer = isInitializer;
        }

        public int Arity { get => _declaration.parameters.Count; }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(_closure);

            for (int i = 0; i<_declaration.parameters.Count; i++)
            {
                environment.Define(_declaration.parameters[i].lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.body, environment);
            } catch(Return returnValue)
            {
                if (_isInitializer) return _closure.GetAt(0, "this");

                return returnValue.Value;
            }

            if (_isInitializer) return _closure.GetAt(0, "this");

            return null;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new Environment(_closure);
            environment.Define("this", instance);

            return new LoxFunction(_declaration, environment, _isInitializer);
        }

        public override string ToString()
        {
            return "<fn " + _declaration.name.lexeme + ">";
        }
    }
}
