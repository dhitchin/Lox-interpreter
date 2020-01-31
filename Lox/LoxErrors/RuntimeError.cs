using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class RuntimeError : LoxError
    {
        public readonly Token token;
        private readonly string _msg;

        public RuntimeError(string message) : base(message)
        {
        }

        public RuntimeError(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RuntimeError(Token token, string message)
        {
            this.token = token;
            this._msg = message;
        }

        public string GetMessage()
        {
            return _msg;
        }

        public RuntimeError() : base()
        {
        }
    }
}
