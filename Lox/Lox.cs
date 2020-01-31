using System;
using System.Collections.Generic;
using System.IO;

namespace Lox
{
    class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();

        static bool hadError = false;
        static bool hadRuntimeError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                System.Environment.Exit(64);
            } else if (args.Length == 1)
            {
                RunFile(args[0]);
            } else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            StreamReader sr = new StreamReader(path);

            Run(sr.ReadToEnd());

            if (hadError) System.Environment.Exit(65);
            if (hadRuntimeError) System.Environment.Exit(70);
        }

        private static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                Run(Console.ReadLine());
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            if (hadError) return;

            interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                Report(token.line, " at end", message);
            } else
            {
                Report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeError runtimeError)
        {
            Console.Error.WriteLine(runtimeError.GetMessage() + "\n[line " + runtimeError.token.line + "]");
            hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line.ToString() + "] Error" + where + ": " + message);
            hadError = true;
        }

        public static void Exit(int code)
        {
            System.Environment.Exit(code);
        }
    }
}
