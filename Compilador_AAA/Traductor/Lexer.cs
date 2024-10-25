using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compilador_AAA.Traductor
{
    public enum TokenType
    {
        Keyword,       // Palabras clave como "int", "if", "for"
        Identifier,    // Nombres de variables, métodos, etc.
        Operator,      // Operadores como "+", "=", "=="
        Literal,       // Literales como números, cadenas
        Punctuation   
    }

    public class Lexer
    {
        private string _code; // El código fuente
        private int _position; // Posición actual en el código

        // Expresiones regulares para diferentes tipos de tokens
        private Dictionary<TokenType, string> _tokenPatterns = new Dictionary<TokenType, string>()
    {
        { TokenType.Keyword, @"\b(int|if|for|while|return)\b" },     // Palabras clave
        { TokenType.Identifier, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b" },     // Identificadores
        { TokenType.Operator, @"[+\-*/=<>!]=?|&&|\|\|" },            // Operadores
        { TokenType.Literal, @"\b\d+\b" },                           // Literales (números)
        { TokenType.Punctuation, @"[;,.()\[\]\{\}]" }                          // Espacios en blanco
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
                    tokens.Add(token);
                }
                else
                {
                    throw new Exception($"Token no reconocido en la posición {_position}: '{_code[_position]}'");
                }
            }

            return tokens;
        }

        private Token GetNextToken()
        {
            foreach (var pattern in _tokenPatterns)
            {
                Match match = Regex.Match(_code.Substring(_position), pattern.Value);
                if (match.Success && match.Index == 0) // El token debe comenzar en la posición actual
                {
                    _position += match.Length; // Avanzamos la posición
                    return new Token(pattern.Key, match.Value);
                }
            }

            return null; // Si no se encuentra un token válido
        }

    }
}
