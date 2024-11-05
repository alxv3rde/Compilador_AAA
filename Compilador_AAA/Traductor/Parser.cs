﻿using Compilador_AAA.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

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
            if (Check(TokenType.Keyword, ["class"]))
            {
                Advance(); //avanza una posicion imitando el metodo Match()
                return ParseClassDeclaration();
            }
            else if (Check(TokenType.Keyword, ["int", "bool", "double", "string" ]))
            {
                return ParseVarDeclaration(true);

            }
            else if (Match(TokenType.Identifier))
            {
                return ParseIdentifier();
            }
            else
            {
                return null;
            }

        }

        private Identifier ParseIdentifier()
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

           
            Consume(TokenType.OpenBrace, "Se esperaba '{' ", "SIN004");

            List<Stmt> children = new List<Stmt>();
            while (!Check(TokenType.CloseBrace) && !IsAtEndOfFile())
            {
                var statement = ParseStatement();
                if (statement != null)
                {
                    children.Add(statement);
                }
                else
                {
                    Advance();
                }
                
            }

            Consume(TokenType.CloseBrace, "Se esperaba '}' al final de la declaración de la clase.", "SIN005");
            return new ClassDeclaration(className, new List<string>(), children, accessModifier,currentLineTemp);
        }

        private VarDeclaration ParseVarDeclaration(bool constant)
        {
            bool isConstant = constant;
            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            Token tokenIdentifier = null;
            Token tokenKeyword = null;
            if (Consume(TokenType.Keyword, "Se esperaba un tipo de dato.", "SIN007"))
            {
                tokenKeyword = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp)
                            : Previous();
            }
            if (Consume(TokenType.Identifier, "Se esperaba un identificador para la variable.", "SIN006"))
            {
                tokenIdentifier = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp)
                            : Previous();
            }
            AssignmentExpr value = null;
            if (Match(TokenType.Equals))
            {
                value = ParseAssignmentExpression(new Identifier(tokenIdentifier.Value, tokenIdentifier.StartLine));
            }

            Consume(TokenType.Semicolon, "Se esperaba ';' al final de la declaración de variable.", "SIN007");
            if(tokenIdentifier != null)
                return new VarDeclaration(tokenKeyword.Value, tokenKeyword.StartLine, isConstant, new Identifier(tokenIdentifier.Value, tokenIdentifier.StartLine), value);
            return null;
        }

        private AssignmentExpr ParseAssignmentExpression(Identifier id)
        {
            Expr expresion = ParseAddExpr();
            if(expresion != null )
                return new AssignmentExpr(id, expresion, _currentLine);
            return null;
            

        }
        private Expr PrimaryExpr()
        {
            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            Token token = null;
            if (Consume([TokenType.IntegerLiteral, TokenType.StringLiteral, TokenType.DoubleLiteral, TokenType.OpenParen], "Se esperaba una expresión ", "SIN008"))
            {
                token = _currentLine > currentLineTemp
                            ? Previous(currentLineTemp, currentTokenIndexTemp)
                            : Previous();
            }
            if (token != null)
            {
                switch (token.Type)
                {
                    case TokenType.Identifier:
                        return new Identifier(token.Value, currentLineTemp);
                    case TokenType.IntegerLiteral:
                        return new IntegerLiteral(Convert.ToInt32(token.Value),currentLineTemp);
                    case TokenType.DoubleLiteral:
                        return new DoubleLiteral(Convert.ToDouble(token.Value), currentLineTemp);
                    //case TokenType.StringLiteral:
                    //    expression = ParseStringLiteral();
                    //    break;
                    //case TokenType.OpenParen:
                    //    expression = ParseBinaryExpr();
                    //    Consume(TokenType.CloseParen, "Se esperaba el cierre del parentesis", "SIN007");
                }
            }
            return null;
        }
        private Expr ParseAddExpr()
        {
            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            Expr left = ParseMultExpr();  // Comienza con una expresión de multiplicación

            while (true)
            {
                Token token = AdvancePeek();
                if (token.Type == TokenType.Operator && (token.Value == "+" || token.Value == "-"))
                {
                    Advance();  // Consume el operador
                    Expr right = ParseMultExpr();  // Obtiene la siguiente expresión de multiplicación
                    left = new BinaryExpr(left, right, token.Value, currentLineTemp);  // Crea una nueva expresión binaria
                }
                else
                {
                    break;  // Sale del bucle si no hay más operadores de suma o resta
                }
            }
            return left;
        }

        private Expr ParseMultExpr()
        {
            int currentLineTemp = _currentLine;
            int currentTokenIndexTemp = _currentTokenIndex;
            Expr left = PrimaryExpr();  // Comienza con una expresión primaria

            while (true)
            {
                Token token = AdvancePeek();
                if (token.Type == TokenType.Operator && (token.Value == "*" || token.Value == "/"))
                {
                    Advance();  // Consume el operador
                    Expr right = PrimaryExpr();  // Obtiene la siguiente expresión primaria
                    left = new BinaryExpr(left, right, token.Value, currentLineTemp);  // Crea una nueva expresión binaria
                }
                else
                {
                    break;  // Sale del bucle si no hay más operadores de multiplicación o división
                }
            }
            return left;
        }
        private StringLiteral ParseStringLiteral()
        {
            Token stringLiteral = Previous();
            return new StringLiteral(stringLiteral.Value,stringLiteral.StartLine);
        }
        private IntegerLiteral ParseIntegerLiteral()
        {
            Token integerLiteral = Previous();
            return new IntegerLiteral(Convert.ToInt32(integerLiteral.Value), integerLiteral.StartLine);
        }
        private DoubleLiteral ParseDoubleLiteral()
        {
            Token doubleLiteral = Previous();
            return new DoubleLiteral(Convert.ToDouble( doubleLiteral.Value), doubleLiteral.StartLine);
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
            IsAtEndOfLine();
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
        private Token AdvancePeek()
        {
            IsAtEndOfLine();
            return Peek();
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
                if(errorCode !="")
                TranslatorView.HandleError(errorMsg, expectedType == TokenType.OpenBrace ? Peek().StartLine - 1 : Peek().StartLine, errorCode);
                Advance();
                return false;
            }

            errorMsg = "Error de sintaxis: " + errorMessage;
            if (errorCode != "")
                TranslatorView.HandleError(errorMsg, Previous().StartLine, errorCode);
            Advance();
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
                if (errorCode != "")
                    TranslatorView.HandleError(errorMsg, Peek().StartLine, errorCode);
                Advance();
                return false;
            }

            errorMsg = "Error de sintaxis: " + errorMessage;
            if (errorCode != "")
                TranslatorView.HandleError(errorMsg, Previous().StartLine, errorCode);
            Advance();
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
            if (errorCode != "")
                TranslatorView.HandleError(errorMsg, errorLine, errorCode);
            Advance();
            return false;
        }
    }
}

