using System;
using System.Collections.Generic;
using System.IO;

namespace Lox
{
    public static class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();

        static bool hadError = false;
        static bool hadRuntimeError = false;

        public static int RunFile(string path)
        {
            StreamReader sr = new StreamReader(path);

            Run(sr.ReadToEnd());

            if (hadError) return 65;
            if (hadRuntimeError) return 70;

            return 0;
        }

        public static void RunPrompt()
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
