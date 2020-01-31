using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    interface ICallable
    {
        public int Arity { get; set; }
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
