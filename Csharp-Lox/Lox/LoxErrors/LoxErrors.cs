using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    public abstract class LoxError : Exception
    {
        protected LoxError() : base()
        {
        }

        protected LoxError(string message) : base(message)
        {
        }

        protected LoxError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ParseError : LoxError
    {
        public ParseError(string message) : base(message)
        {
        }

        public ParseError(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParseError() : base()
        {
        }
    }

}
