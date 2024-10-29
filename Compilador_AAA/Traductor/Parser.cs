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
            IsAtEndOfLine();
            if (Check(TokenType.Keyword, new string[] { "class" }))
            {
                Advance(); //avanza una posicion imitando el metodo Match()
                return ParseClassDeclaration();
            }
            else if (Check(TokenType.Keyword, new string[] { "int", "bool", "double", "string" }))
            {
                return ParseVarDeclaration(true);

            }
            else if (Match(TokenType.Identifier))
            {
                return ParseIdentifier();
            }
            else if (Match(TokenType.NumericLiteral))
            {
                return ParseNumericLiteral();
            }
            else
            {
                return null;
            }

        }
        private Expr ParseIdentifier()
        {

            string identifier = Previous().Value;
            return new Identifier(identifier,_currentLine);
        }
        private Expr ParseNumericLiteral()
        {
            string numericL = Previous().Value;
            return new Identifier(numericL,_currentLine);
        }
        private ClassDeclaration ParseClassDeclaration()
        {
            var accessModifier = TokenType.Public; // Almacena el modificador de acceso

            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            // Ahora, consumimos el identificador del nombre de la clase
            string className = null;

            if (Consume(TokenType.Identifier, "Se esperaba un nombre de clase después de la palabra clave 'class'.", "SIN001"))
            {
                className = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp).Value
                            : Previous().Value;
            }

            if (IsAtEndOfLine() && !IsAtEndOfFile())
            {
                Consume(TokenType.OpenBrace, "Se esperaba '{' ", "SIN004");
            }
            else
            {
                Consume(TokenType.OpenBrace, "Se esperaba '{' ", "SIN004");
            }
            List<Stmt> children = new List<Stmt>();
            while (!Check(TokenType.CloseBrace) && !IsAtEndOfFile())
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    children.Add(statement);
                }
                //else
                //{
                //    Advance();
                //}
            }

            Consume(TokenType.CloseBrace, "Se esperaba '}' al final de la declaración de la clase.", "SIN005");
            return new ClassDeclaration(className, new List<string>(), children, accessModifier,currentLineTemp);
        }
        //private FunctionDeclaration ParseFunctionDeclaration()
        //{
        //    var accessModifier = TokenType.Public; // Almacena el modificador de acceso

        //    //// Se espera que el siguiente token sea la palabra clave 'class'
        //    //Consume(TokenType.Keyword, "class", "Se esperaba la palabra clave 'class' después del modificador de acceso.", "SIN001");
        //    int currentLineTemp = _currentLine;
        //    int currentTokenIndexTemp = _currentTokenIndex;
        //    // Ahora, consumimos el identificador del nombre de la clase
        //    string funcName = null;

        //    if (Consume(TokenType.Identifier, "Se esperaba un identificador después de la palabra clave 'func'.", "SIN001"))
        //    {
        //        funcName = _currentLine > currentLineTemp
        //                    ? Previous(currentLineTemp, currentTokenIndexTemp).Value
        //                    : Previous().Value;
        //    }
        //    Consume(TokenType.OpenBrace, "Se esperaba '(' ", "SIN003");
        //    List<Stmt> parameters = new List<Stmt>();
        //    while (!Check(TokenType.CloseParen) && !IsAtEndOfFile())
        //    {
        //        if (Consume(TokenType.Keyword, "Se esperaba un un tipo de dato", "SIN007"))
        //        {
        //            parameters.Add(ParseVarDeclaration(true));
        //        }
        //        else
        //        {

        //        }
        //    }
        //    Consume(TokenType.OpenBrace, "Se esperaba ')' ", "SIN003");

        //    if (IsAtEndOfLine() && !IsAtEndOfFile())
        //    {
        //        Consume(TokenType.OpenBrace, "Se esperaba '{' ", "SIN004");
        //    }
        //    else
        //    {
        //        Consume(TokenType.OpenBrace, "Se esperaba '{' ", "SIN004");
        //    }
        //    List<Stmt> children = new List<Stmt>();
        //    while (!Check(TokenType.CloseBrace) && !IsAtEndOfFile())
        //    {
        //        var statement = ParseStatement();
        //        if (statement != null)
        //        {
        //            children.Add(statement);
        //        }
        //    }

        //    Consume(TokenType.CloseBrace, "Se esperaba '}' al final de la declaración de la clase.", "SIN005");
        //    return new FunctionDeclaration();
        //}

        private VarDeclaration ParseVarDeclaration(bool constant)
        {
            bool isConstant = constant;
            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            string identifier = null;
            string keyword = null;
            if (Consume(TokenType.Keyword, "Se esperaba un un tipo de dato", "SIN007"))
            {
                keyword = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp).Value
                            : Previous().Value;
            }
            if (Consume(TokenType.Identifier, "Se esperaba un identificador para la variable.", "SIN006"))
            {
                identifier = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp).Value
                            : Previous().Value;
            }
            string value = null;
            if (Match(TokenType.Equals))
            {
                value = ParseExpression();
            }

            Consume(TokenType.Semicolon, "Se esperaba ';' al final de la declaración de variable.", "SIN007");
            return new VarDeclaration(currentLineTemp, isConstant, identifier, value);
        }

        private string ParseExpression()
        {
            string keyword = null;
            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            string expr = Previous().Value;
           
            if  (Consume(new TokenType[] {TokenType.NumericLiteral,TokenType.StringLiteral}, "Se esperaba una expresión ", "SIN008"))
            {
                expr = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp).Value
                            : Previous().Value;
            }
            return expr;

        }
        private Token Previous(int previousLine, int previousIndexToken)
        {
            List<Token> tempTokens;
            tempTokens = _tokensByLine[previousLine];
            return tempTokens[previousLine - 1];
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
                if (CheckNoHop(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
        private bool CheckNoHop(TokenType type)
        {
            if (IsAtEndOfLineNoHop()) return false;
            return Peek().Type == type;
        }
        private bool Check(TokenType type)
        {
            if (IsAtEndOfLine()) return false;
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
                        return true;
                    }
                }
            }

            return false;
        }

        private Token Advance()
        {
            if (!IsAtEndOfLine()) _currentTokenIndex++;
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

        private bool IsAtEndOfFile()
        {
            return Peek().Type == TokenType.EOF;
        }
        private bool IsAtEndOfLineNoHop()
        {
            if (_currentTokenIndex >= _currentTokens.Count)
            {
                return true;
            }

            return false;
        }
        private bool IsAtEndOfLine()
        {
            if (_currentTokenIndex >= _currentTokens.Count)
            {
                AdvanceToNextLine();
                return true;
            }

            return false;
        }
        private bool Consume(TokenType expectedType, string errorMessage, string errorCode)
        {
            string errorMsg;
            if (Check(expectedType))
            {
                Advance();
                return true;
            }
            else if (!IsAtEndOfLine())
            {
                errorMsg = "Error de sintaxis: " + errorMessage;
                TranslatorView.HandleError(errorMsg, expectedType == TokenType.OpenBrace ? Peek().StartLine - 1 : Peek().StartLine, errorCode);
                return false;
            }

            errorMsg = "Error de sintaxis: " + errorMessage;
            TranslatorView.HandleError(errorMsg, Previous().StartLine, errorCode);
            return false;

        }


        private bool Consume(TokenType expectedType, string keyword, string errorMessage, string errorCode)
        {
            string errorMsg;
            if (Check(expectedType) && Peek().Value == keyword)
            {
                Advance(); // Avanza al siguiente token si coincide
                return true;
            }
            else if (!IsAtEndOfLine())
            {
                errorMsg = "Error de sintaxis: " + errorMessage;
                TranslatorView.HandleError(errorMsg, Peek().StartLine, errorCode);
                return false;
            }

            errorMsg = "Error de sintaxis: " + errorMessage;
            TranslatorView.HandleError(errorMsg, Previous().StartLine, errorCode);
            return false;
        }
        private bool Consume(TokenType[] expectedType, string errorMessage, string errorCode)
        {
            foreach (var type in expectedType)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            string errorMsg = "Error de sintaxis: " + errorMessage;
            int errorLine = IsAtEndOfLine() ? Previous().StartLine : Peek().StartLine;

            // Manejar el error
            TranslatorView.HandleError(errorMsg, errorLine, errorCode);
            return false;
        }
    }
}

