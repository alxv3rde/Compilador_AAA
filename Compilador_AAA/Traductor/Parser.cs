using Compilador_AAA.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

            while (_currentLine < _tokensByLine.Count)
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
            //TokenType AccessMod = TokenType.Public;
            //if (Match(TokenType.Public, TokenType.Private, TokenType.Protected, TokenType.Internal))
            //{

            //}
            if (Check(TokenType.Keyword, new string[] { "class" }))
            {
                return ParseClassDeclaration();
            }
            else if (Match(TokenType.Constant) is var matchResult && Check(TokenType.Keyword, new string[] { "int", "bool", "double", "string" }))
            {
                return ParseVarDeclaration(matchResult);
            }
            else
            {
                // Aquí puedes agregar más declaraciones como funciones o expresiones
                return null;
            }

        }

        private ClassDeclaration ParseClassDeclaration()
        {
            var accessModifier = TokenType.Public; // Almacena el modificador de acceso

            //// Se espera que el siguiente token sea la palabra clave 'class'
            //Consume(TokenType.Keyword, "class", "Se esperaba la palabra clave 'class' después del modificador de acceso.", "SIN001");

            // Ahora, consumimos el identificador del nombre de la clase
            Consume(TokenType.Identifier, "Se esperaba un nombre de clase después de la palabra clave 'class'.", "SIN002");
            string className = Previous().Value;

            Consume(TokenType.OpenParen, "Se esperaba '(' ", "SIN002");
            //parametros aqui
            Consume(TokenType.CloseParen, "Se esperaba ')' ", "SIN003");

            Consume(TokenType.OpenBrace, "Se esperaba '{' ", "SIN004");

            List<Stmt> children = new List<Stmt>();
            while (!Check(TokenType.CloseBrace) && !IsAtEnd())
            {

                var statement = ParseStatement();
                if (statement != null)
                {
                    children.Add(statement);
                }
            }

            Consume(TokenType.CloseBrace, "Se esperaba '}' al final de la declaración de la clase.", "SIN005");
            return new ClassDeclaration(className, new List<string>(), children, accessModifier);
        }

        private VarDeclaration ParseVarDeclaration(bool constant)
        {
            bool isConstant = constant;
            Consume(TokenType.Identifier, "Se esperaba un identificador para la variable.", "SIN006");
            string identifier = Previous().Value;

            Expr value = null;
            if (Match(TokenType.Equals))
            {
                value = ParseExpression();
            }

            Consume(TokenType.Semicolon, "Se esperaba ';' al final de la declaración de variable.", "SIN007");
            return new VarDeclaration(isConstant, identifier, value);
        }
        private VarDeclaration ParseGlobalVarDeclaration()
        {
            bool isConstant = Match(TokenType.Constant);
            Consume(TokenType.Identifier, "Se esperaba un identificador para la variable.", "SIN006");
            string identifier = Previous().Value;

            Expr value = null;
            if (Match(TokenType.Equals))
            {
                value = ParseExpression();
            }

            Consume(TokenType.Semicolon, "Se esperaba ';' al final de la declaración de variable.", "SIN007");
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
        private bool MatchNotAdv(params TokenType[] tokenTypes)
        {
            foreach (var type in tokenTypes)
            {
                if (Check(type))
                {
                    return true;
                }
            }
            return false;
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
        private bool Check(TokenType token, string[] keywords)
        {
            if (Check(token))
            {
                foreach (var k in keywords)
                {
                    if (Peek().Value == k)
                    {
                        Advance();
                        return true;
                    }
                }
            }

            return false;
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
            if (_currentTokenIndex >= _currentTokens.Count || Peek().Type == TokenType.EOF)
            {
                return true;
            }
            return false;
        }
        private void Consume(TokenType expectedType, string errorMessage, string errorCode)
        {
            string errorMsg;
            if (Check(expectedType))
            {
                Advance(); // Avanza al siguiente token si coincide
            }else if (!IsAtEnd())
            {
                errorMsg = "Error de sintaxis: " + errorMessage + "pero se encontró" + Peek().Value+".";
                TranslatorView.HandleError(errorMsg, Peek().StartLine, errorCode);
            }
            else
            {
                errorMsg = "Error de sintaxis: "+ errorMessage;
                TranslatorView.HandleError(errorMsg, Previous().StartLine, errorCode);
            }
            
        }


        private void Consume(TokenType expectedType, string keyword, string errorMessage, string errorCode)
        {
            string errorMsg;
            if (Check(expectedType) && Peek().Value == keyword)
            {
                 Advance(); // Avanza al siguiente token si coincide
                 
            }else if (!IsAtEnd())
            {
                errorMsg = "Error de sintaxis: " + errorMessage + "pero se encontró" + Peek().Value + ".";
                TranslatorView.HandleError(errorMsg, Peek().StartLine, errorCode);
            }
            else
            {
                errorMsg = "Error de sintaxis: " + errorMessage;
                TranslatorView.HandleError(errorMsg, Previous().StartLine, errorCode);
            }
        }
    }
}

