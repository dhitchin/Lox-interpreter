using System;
using System.Collections.Generic;
using System.Text;

namespace Lox.NativeFunctions
{
    class Exit : ICallable
    {
        public int Arity { get => 0; set => throw new NotImplementedException(); }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Lox.Exit(1);

            return null;
        }
    }
}
