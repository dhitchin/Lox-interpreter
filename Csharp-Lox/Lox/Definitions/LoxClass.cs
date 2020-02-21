using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class LoxClass : ICallable
    {
        private readonly Dictionary<string, LoxFunction> _methods;
        
        public LoxClass Superclass { get; }
        public string Name { get; }
        public int Arity { 
            get {
                LoxFunction initializer = FindMethod("init");
                return (initializer == null) ? 0 : initializer.Arity;
            }
        }

        public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            Superclass = superclass;
            _methods = methods;
        }

        public LoxFunction FindMethod(string name)
        {
            if (_methods.ContainsKey(name))
            {
                return _methods[name];
            }

            if (Superclass != null)
            {
                return Superclass.FindMethod(name);
            }

            return null;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction initializer = FindMethod("init");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
