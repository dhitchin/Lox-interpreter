using System;
using System.Collections.Generic;
using System.Text;

namespace Lox
{
    class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };

        public Scanner(string source)
        {
            this.source = source;
        }

        /// <summary>
        /// Scans and parses the tokens.
        /// </summary>
        /// <returns>List of parsed tokens</returns>
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        /// <summary>
        /// Scans the next token and adds it to the list.
        /// </summary>
        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(Match('-') ? TokenType.MINUS_MINUS : TokenType.MINUS); break;
                case '+': AddToken(Match('+') ? TokenType.PLUS_PLUS : TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                //case '?': AddToken(TokenType.QUESTION_MARK); break;
                //case ':': AddToken(TokenType.COLON); break;

                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    //ignore whitespace
                    break;

                case '\n':
                    line++;
                    break;

                case '"': Lox_string(); break;

                default:
                    if (IsDigit(c))
                    {
                        Number();
                    } 
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected character.");
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// Adds the token.
        /// </summary>
        /// <param name="type">The token type.</param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Adds the token.
        /// </summary>
        /// <param name="type">The token type.</param>
        /// <param name="literal">The token literal.</param>
        private void AddToken(TokenType type, object literal)
        {
            string text = source[start..current];
            tokens.Add(new Token(type, text, literal, line));
        }



        /// <summary>
        /// If the current character matches the expected character, then consume it and return true.
        /// </summary>
        /// <param name="expected">The expected character.</param>
        /// <returns>true if match, false otherwise</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        /// <summary>
        /// Peeks at the next character, without advancing the counter.
        /// </summary>
        /// <returns></returns>
        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        /// <summary>
        /// Peeks at the next, next character, without advancing the counter.
        /// </summary>
        /// <returns></returns>
        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        /// <summary>
        /// Consumes the current character and advances the index.
        /// </summary>
        /// <returns>Character consumed</returns>
        private char Advance()
        {
            current++;
            return source[current - 1];
        }



        /// <summary>
        /// Parses and adds a string token.
        /// </summary>
        private void Lox_string()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            //Unterminated string
            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            //consume closing "
            Advance();

            string value = source.Substring(start+1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        /// <summary>
        /// Parses and adds a number token to the list.
        /// </summary>
        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            //look for decimal part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                //consume the '.'
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, double.Parse(source[start..current]));
        }

        /// <summary>
        /// Parses and adds an identifier OR keyword token to the list.
        /// </summary>
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = source[start..current];

            if (!keywords.TryGetValue(text, out TokenType type))
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }



        /// <summary>
        /// Determines whether the specified char is alpha.
        /// </summary>
        /// <param name="c">The char.</param>
        /// <returns>
        ///   <c>true</c> if the specified char is alpha; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   (c == '_');
        }

        /// <summary>
        /// Determines whether the specifed char is alpha numeric.
        /// </summary>
        /// <param name="c">The char.</param>
        /// <returns>
        ///   <c>true</c> if the specified char is alpha numeric; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        /// <summary>
        /// Determines whether the specified char is a digit.
        /// </summary>
        /// <param name="c">The char.</param>
        /// <returns>
        ///   <c>true</c> if the specified char is digit; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Determines whether [is at end] of source.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is at end]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }
    }
}
