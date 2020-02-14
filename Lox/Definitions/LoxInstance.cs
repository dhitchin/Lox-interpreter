using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{ 
    class LoxInstance
    {
        private readonly LoxClass _class;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public LoxInstance(LoxClass _class)
        {
            this._class = _class;
        }

        public object Get(Token name)
        {
            if (_fields.ContainsKey(name.lexeme))
            {
                return _fields[name.lexeme];
            }

            LoxFunction method = _class.FindMethod(name.lexeme);
            if (method != null) return method.Bind(this);

            throw new RuntimeError(name, "Undefined property '" + name.lexeme + "'.");
        }

        public void Set(Token name, object value)
        {
            if (_fields.ContainsKey(name.lexeme))
            {
                _fields[name.lexeme] = value;
            }
            else
            {
                _fields.Add(name.lexeme, value);
            }
        }

        public override string ToString()
        {
            return _class.Name + " instance.";
        }
    }
}
