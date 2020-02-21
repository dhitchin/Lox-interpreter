using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.NativeFunctions
{
    class Clock : ICallable
    {
        public int Arity { get { return 0; } set { } }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return DateTime.Now.Second + DateTime.Now.Millisecond / 1000.0;
        }
    }
}
