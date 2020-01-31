using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class ConsoleApp
    {


        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                Lox.RunFile(args[0]);
            }
            else
            {
                Lox.RunPrompt();
            }
        }
    }
}
