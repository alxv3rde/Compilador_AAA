using Compilador_AAA.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compilador_AAA.Traductor
{
    public enum TokenType
    {
        Public,
        Private,
        Protected,
        Internal,
        Keyword,
        Identifier,
        Literal,
        NumericLiteral,
        StringLiteral,
        Operator,
        BinaryOperator,
        Equals,
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        Semicolon,
        Comma,
        SingleLineComment,
        MultiLineComment,
        EOF,
        UnrecognizedCharacter,
    }

    public class Lexer
    {
        private string _code; // El código fuente
        private int _position; // Posición actual en el código

        private Dictionary<TokenType, string> _tokenPatterns = new Dictionary<TokenType, string>()
    {
        { TokenType.SingleLineComment, @"//.*" },
        { TokenType.MultiLineComment, @"/\*[\s\S]*?\*/" },
        { TokenType.Public, @"\bpublic\b" },
        { TokenType.Private, @"\bprivate\b" },
        { TokenType.Protected, @"\bprotected\b" },
        { TokenType.Internal, @"\binternal\b" },
        { TokenType.Keyword, @"\b(int|if|for|while|return|string|char|double)\b" },
        { TokenType.NumericLiteral, @"(?<!\w)[+-]?((\d+(?:\.\d+)*)|(\.\d+))(e[+-]?\d+)?(?!\w)"},
        { TokenType.Identifier, @"\b[a-zA-Z0-9_]+\b" },
        { TokenType.Equals, @"=" },
        { TokenType.Operator, @"==|[+\-*/%&|^!=<>]=?|&&|\|\|" },
        { TokenType.BinaryOperator, @"[+\-*/%&|^!=<>]" },
        { TokenType.OpenParen, @"\(" },
        { TokenType.CloseParen, @"\)" },
        { TokenType.OpenBrace, @"\{" },
        { TokenType.CloseBrace, @"\}" },
        { TokenType.OpenBracket, @"\[" },
        { TokenType.CloseBracket, @"\]" },
        { TokenType.Comma, @"," },
        { TokenType.Semicolon, @";" },


    };

        public Lexer(string code)
        {
            _code = code;
            _position = 0;
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();

            while (_position < _code.Length)
            {
                // Ignorar espacios en blanco
                if (char.IsWhiteSpace(_code[_position]))
                {
                    _position++;
                    continue;
                }

                // Tratar de encontrar un token válido
                Token token = GetNextToken();
                if (token != null)
                {
                    // Verificar identificadores que comienzan con un número
                    if (token.Type == TokenType.Identifier && char.IsDigit(token.Value[0]))
                    {
                        TranslatorView.HandleError($"Identificador no puede comenzar con un número en la posición {_position}: '{token.Value}'", 1);
                    }

                    // Verificar números mal formateados con múltiples puntos decimales
                    if (token.Type == TokenType.NumericLiteral && token.Value.Count(c => c == '.') > 1)
                    {
                        TranslatorView.HandleError($"Número mal formateado en la posición {_position}: '{token.Value}'", 2);
                    }

                    tokens.Add(token);
                }
                else
                {
                    TranslatorView.HandleError($"Token no reconocido en la posición {_position}: '{_code[_position]}'", _position);
                    _position++;
                }
            }
            return tokens;
        }

        private Token GetNextToken()
        {
            // Contadores para la línea y la posición en la línea
            int startLine = 1;
            int startColumn = 1;

            // Contar líneas y columnas hasta la posición actual
            for (int i = 0; i < _position; i++)
            {
                if (_code[i] == '\n')
                {
                    startLine++;
                    startColumn = 1;
                }
                else
                {
                    startColumn++;
                }
            }

            // Verificar manualmente los literales de cadena
            if (_code[_position] == '\"' || _code[_position] == '\'')
            {
                char quoteChar = _code[_position];
                int start = _position++;
                while (_position < _code.Length && _code[_position] != quoteChar)
                {
                    if (_code[_position] == '\n') // Incrementar la línea si encontramos un salto de línea
                    {
                        startLine++;
                        startColumn = 1;
                    }
                    else
                    {
                        startColumn++;
                    }
                    _position++;
                }

                // Si alcanzamos el final sin encontrar la comilla de cierre
                if (_position >= _code.Length || _code[_position] != quoteChar)
                {
                    TranslatorView.HandleError($"Literal de cadena sin cerrar que comienza en la posición {start}.", 1);
                }
                else
                {
                    // Avanzar la posición para incluir la comilla de cierre
                    _position++;
                }

                string value = _code.Substring(start, _position - start);
                return new Token(TokenType.StringLiteral, value, start, _position - 1, startLine, startLine); // Fin es _position - 1
            }

            // Buscar otros tokens mediante patrones
            foreach (var pattern in _tokenPatterns)
            {
                Match match = Regex.Match(_code.Substring(_position), pattern.Value);
                if (match.Success && match.Index == 0) // El token debe comenzar en la posición actual
                {
                    int start = _position;
                    int end = _position + match.Length - 1; // Fin del token
                    _position += match.Length; // Avanzamos la posición

                    // Calcular la línea de fin
                    int endLine = startLine;
                    int endColumn = startColumn + match.Length - 1;

                    // Actualizar endLine si se ha cruzado un salto de línea
                    for (int i = start; i <= end; i++)
                    {
                        if (_code[i] == '\n')
                        {
                            endLine++;
                            endColumn = 1;
                        }
                        else
                        {
                            endColumn++;
                        }
                    }

                    return new Token(pattern.Key, match.Value, start, end, startLine, endLine); // Fin es _position - 1
                }
            }

            return null; // Si no se encuentra un token válido
        }
    }

}
