using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AAA.Traductor
{
    public class Parser
    {
        private Dictionary<int, List<Token>> _tokensByLine;
        private int _currentLine;
        private int _currentTokenIndex;
        private List<Token> _currentTokens;

        public Parser(Dictionary<int, List<Token>> tokensByLine)
        {
            _tokensByLine = tokensByLine;
            _currentLine = 1;
            _currentTokenIndex = 0;
            _currentTokens = _tokensByLine[_currentLine];
        }

        public Program Parse()
        {
            var program = new Program();

            while (_currentLine <= _tokensByLine.Count)
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    program.children.Add(statement);
                }
                else
                {
                    AdvanceToNextLine();
                }
            }

            return program;
        }

        private Stmt ParseStatement()
        {
            if (Match(TokenType.Public, TokenType.Private, TokenType.Protected, TokenType.Internal))
            {
                return ParseClassDeclaration();
            }
            else if (Match(TokenType.Constant, TokenType.Keyword))
            {
                return ParseVarDeclaration();
            }
            else
            {
                // Aquí puedes agregar más declaraciones como funciones o expresiones
                return null;
            }
        }

        private ClassDeclaration ParseClassDeclaration()
        {
            var accessModifier = Previous().Type; // Almacena el modificador de acceso
            Consume(TokenType.Identifier, "Se esperaba un nombre de clase después del modificador de acceso.");
            string className = Previous().Value;

            Consume(TokenType.OpenBrace, "Se esperaba '{' después del nombre de la clase.");

            List<Stmt> children = new List<Stmt>();
            while (!Check(TokenType.CloseBrace) && !IsAtEnd())
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    children.Add(statement);
                }
            }

            Consume(TokenType.CloseBrace, "Se esperaba '}' al final de la declaración de la clase.");
            return new ClassDeclaration(className, new List<string>(), children);
        }

        private VarDeclaration ParseVarDeclaration()
        {
            bool isConstant = Match(TokenType.Constant);
            Consume(TokenType.Identifier, "Se esperaba un identificador para la variable.");
            string identifier = Previous().Value;

            Expr value = null;
            if (Match(TokenType.Equals))
            {
                value = ParseExpression();
            }

            Consume(TokenType.Semicolon, "Se esperaba ';' al final de la declaración de variable.");
            return new VarDeclaration(isConstant, identifier, value);
        }

        private Expr ParseExpression()
        {
            // Aquí puedes implementar el análisis de expresiones, como expresiones binarias
            return null;
        }

        private void AdvanceToNextLine()
        {
            _currentLine++;
            _currentTokenIndex = 0;

            if (_currentLine <= _tokensByLine.Count)
            {
                _currentTokens = _tokensByLine[_currentLine];
            }
        }

        private bool Match(params TokenType[] tokenTypes)
        {
            foreach (var type in tokenTypes)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _currentTokenIndex++;
            return Previous();
        }

        private Token Peek()
        {
            return _currentTokens[_currentTokenIndex];
        }

        private Token Previous()
        {
            return _currentTokens[_currentTokenIndex - 1];
        }

        private bool IsAtEnd()
        {
            return _currentTokenIndex >= _currentTokens.Count || Peek().Type == TokenType.EOF;
        }
        private Token Consume(TokenType expectedType, string errorMessage)
        {
            if (Check(expectedType))
            {
                return Advance(); // Avanza al siguiente token si coincide
            }

            // Si no coincide, lanza un error
            throw new Exception($"Error de sintaxis: {errorMessage}. Se esperaba '{expectedType}', pero se encontró '{Peek().Type}'.");
        }
    }
}
