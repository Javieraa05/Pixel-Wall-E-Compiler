using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Lexer: realiza el análisis léxico transformando el código fuente en una lista de tokens.
    /// Detecta errores léxicos y soporta palabras reservadas, operadores y símbolos especiales.
    /// </summary>
    public class Lexer
    {
        private readonly string _source;
        private int _position;
        private int _line;
        private int _column;
        
        /// <summary>
        /// Lista de excepciones léxicas detectadas durante el análisis.
        /// </summary>
        private List<LexicalException> _lexicalErrors = new List<LexicalException>();

        /// <summary>
        /// Propiedad para acceder a la lista de errores léxicos.
        /// </summary>
        public List<LexicalException> LexicalErrors => _lexicalErrors;

        /// <summary>
        /// Indica si se detectó al menos un error léxico.
        /// </summary>
        public bool HadError => _lexicalErrors.Count > 0;

        /// <summary>
        /// Diccionario de palabras reservadas, con comparación sensible a mayúsculas/minúsculas.
        /// </summary>
        private static readonly Dictionary<string, TokenType> _keywords =
            new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            {"Spawn", TokenType.Spawn },
            {"ReSpawn", TokenType.ReSpawn },
            {"Color", TokenType.Color },
            {"Size", TokenType.Size },
            {"DrawLine", TokenType.DrawLine },
            {"DrawCircle", TokenType.DrawCircle },
            {"DrawRectangle", TokenType.DrawRectangle },
            {"Fill", TokenType.Fill },
            {"GoTo", TokenType.GoTo },
            {"GetActualX", TokenType.GetActualX },
            {"GetActualY", TokenType.GetActualY },
            {"GetCanvasSize", TokenType.GetCanvasSize },
            {"GetColorCount", TokenType.GetColorCount },
            {"IsBrushColor", TokenType.IsBrushColor },
            {"IsBrushSize", TokenType.IsBrushSize },
            {"IsCanvasColor", TokenType.IsCanvasColor }
        };

        /// <summary>
        /// Inicializa una nueva instancia del Lexer con el código fuente proporcionado.
        /// </summary>
        /// <param name="source">El código fuente a analizar.</param>
        public Lexer(string source)
        {
            _source = source;
            _position = 0;
            _line = 1;
            _column = 1;
        }

        /// <summary>
        /// Propiedad que devuelve el caracter actual o '\0' si se llegó al final.
        /// </summary>
        private char Current => _position < _source.Length ? _source[_position] : '\0';

        /// <summary>
        /// Realiza el análisis léxico del código fuente y devuelve la lista de tokens encontrados.
        /// Agrega errores a la lista <c>LexicalErrors</c> cuando encuentra caracteres o secuencias inválidas.
        /// </summary>
        /// <returns>Lista de tokens generados.</returns>
        public List<Token> Lex()
        {
            var tokens = new List<Token>();

            while (_position < _source.Length)
            {
                SkipWhitespace();
                int startLine = _line, startCol = _column;
                if (_position >= _source.Length)
                    break;

                char c = Current;

                // Número
                if (char.IsDigit(c))
                {
                    string num = ReadNumber();
                    tokens.Add(new Token(TokenType.Number, num, startLine, startCol));
                    continue;
                }

                // Identificador o palabra reservada (debe empezar con letra española o subrayado)
                if (IsSpanishLetter(c) || c == '_')
                {
                    string word = ReadIdentifier();
                    if (_keywords.TryGetValue(word, out var kwType))
                        tokens.Add(new Token(kwType, word, startLine, startCol));
                    else
                        tokens.Add(new Token(TokenType.Identifier, word, startLine, startCol));
                    continue;
                }

                // Cadena
                if (c == '"')
                {
                    string str = ReadString(startLine, startCol);
                    tokens.Add(new Token(TokenType.String, str, startLine, startCol));
                    continue;
                }

                // Operadores y símbolos
                switch (c)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus, "+", startLine, startCol));
                        Advance();
                        break;

                    case '-':
                        tokens.Add(new Token(TokenType.Minus, "-", startLine, startCol));
                        Advance();
                        break;

                    case '*':
                        if (Peek() == '*')
                        {
                            tokens.Add(new Token(TokenType.Power, "**", startLine, startCol));
                            Advance(); 
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Star, "*", startLine, startCol));
                            Advance();
                        }
                        break;

                    case '/':
                        tokens.Add(new Token(TokenType.Slash, "/", startLine, startCol));
                        Advance();
                        break;

                    case '%':
                        tokens.Add(new Token(TokenType.Modulo, "%", startLine, startCol));
                        Advance();
                        break;

                    case '&':
                        if (Peek() == '&')
                        {
                            Advance(); 
                            Advance();
                            tokens.Add(new Token(TokenType.And, "&&", startLine, startCol));
                        }
                        else
                        {
                            _lexicalErrors.Add(new LexicalException($"Carácter inesperado '&'", _line, _column));
                            Advance();
                        }
                        break;

                    case '|':
                        if (Peek() == '|')
                        {
                            Advance(); 
                            Advance();
                            tokens.Add(new Token(TokenType.Or, "||", startLine, startCol));
                        }
                        else
                        {
                            _lexicalErrors.Add(new LexicalException($"Carácter inesperado '|'", _line, _column));
                            Advance();
                        }
                        break;

                    case '=':
                        if (Peek() == '=')
                        {
                            Advance(); 
                            Advance();
                            tokens.Add(new Token(TokenType.EqualEqual, "==", startLine, startCol));
                        }
                        else
                        {
                            _lexicalErrors.Add(new LexicalException($"Carácter inesperado '='", _line, _column));
                            Advance();
                        }
                        break;

                    case '>':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.GreaterEqual, ">=", startLine, startCol));
                            Advance(); 
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Greater, ">", startLine, startCol));
                            Advance();
                        }
                        break;

                    case '<':
                        if (Peek() == '=')
                        {
                            tokens.Add(new Token(TokenType.LessEqual, "<=", startLine, startCol));
                            Advance(); 
                            Advance();
                        }
                        else if (Peek() == '-')
                        {
                            tokens.Add(new Token(TokenType.Assign, "<-", startLine, startCol));
                            Advance(); 
                            Advance();
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Less, "<", startLine, startCol));
                            Advance();
                        }
                        break;

                    case '←':
                        tokens.Add(new Token(TokenType.Assign, "←", startLine, startCol));
                        Advance();
                        break;

                    case '(':
                        tokens.Add(new Token(TokenType.LeftParen, "(", startLine, startCol));
                        Advance();
                        break;

                    case ')':
                        tokens.Add(new Token(TokenType.RightParen, ")", startLine, startCol));
                        Advance();
                        break;

                    case '[':
                        tokens.Add(new Token(TokenType.LeftBracket, "[", startLine, startCol));
                        Advance();
                        break;

                    case ']':
                        tokens.Add(new Token(TokenType.RightBracket, "]", startLine, startCol));
                        Advance();
                        break;

                    case ',':
                        tokens.Add(new Token(TokenType.Comma, ",", startLine, startCol));
                        Advance();
                        break;

                    case '\n':
                        tokens.Add(new Token(TokenType.EOL, "\\n", startLine, startCol));
                        Advance();
                        break;
                    case '\r':
                        tokens.Add(new Token(TokenType.EOL, "\\r", startLine, startCol));
                        Advance();
                        break;
                    default:
                        _lexicalErrors.Add(new LexicalException($"Carácter inesperado '{Current}'", _line, _column));
                        Advance();
                        break;
                }
            }

            tokens.Add(new Token(TokenType.EOF, "", _line, _column));
            return tokens;
        }

        /// <summary>
        /// Avanza el cursor en el código fuente. Actualiza el contador de línea y columna.
        /// Si el caracter es un salto de línea, incrementa la línea y reinicia la columna.
        /// </summary>
        private void Advance()
        {
            if (Current == '\r' || Current == '\n')
            {
                _line++;
                _column = 1;
                do
                {
                    _position++;
                }
                while (Current == ' ');
                return;
            }
            else
            {
                _column++;
            }
            _position++;
        }

        /// <summary>
        /// Devuelve el caracter en la posición actual + offset sin avanzar el cursor.
        /// </summary>
        /// <param name="offset">Desplazamiento desde la posición actual (por defecto 1).</param>
        /// <returns>El caracter encontrado o '\0' si se sale del rango.</returns>
        private char Peek(int offset = 1)
        {
            int pos = _position + offset;
            return pos < _source.Length ? _source[pos] : '\0';
        }

        /// <summary>
        /// Omite espacios y tabulaciones, sin afectar saltos de línea.
        /// </summary>
        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(Current) && Current != '\n')
            {
                _column++;
                _position++;
            }
        }

        /// <summary>
        /// Lee y devuelve una secuencia numérica, incluyendo puntos decimales.
        /// Agrega un error si el número tiene un formato inválido.
        /// </summary>
        /// <returns>Representación en cadena del número leído.</returns>
        private string ReadNumber()
        {
            int start = _position;
            while (char.IsDigit(Current) || Current == '.' || IsSpanishLetter(Current))
            {
                Advance();
            }
            string numStr = _source[start.._position];
            if (!int.TryParse(numStr, out int n))
            {
                _lexicalErrors.Add(new LexicalException($"Número inválido: '{numStr}'", _line, _column));
            }
            return numStr;
        }

        /// <summary>
        /// Lee y devuelve un identificador o palabra reservada.
        /// Considera como válido letras españolas, dígitos y el subrayado.
        /// </summary>
        /// <returns>El identificador leído.</returns>
        private string ReadIdentifier()
        {
            int start = _position;
            // Primer carácter ya validado como letra española o subrayado
            Advance();
            while (char.IsDigit(Current) || IsSpanishLetter(Current) || Current == '_')
                Advance();
            return _source[start.._position];
        }

        /// <summary>
        /// Lee y devuelve una cadena encerrada en comillas dobles.
        /// Si la cadena no se cierra correctamente, registra un error.
        /// </summary>
        /// <param name="startLine">La línea de inicio de la cadena.</param>
        /// <param name="startCol">La columna de inicio de la cadena.</param>
        /// <returns>El contenido de la cadena (sin las comillas).</returns>
        private string ReadString(int startLine, int startCol)
        {
            Advance(); // omite la comilla inicial
            int start = _position;
            while (Current != '"' && Current != '\n' && Current != '\0')
                Advance();
            if (Current != '"')
            {
                _lexicalErrors.Add(new LexicalException($"Cadena sin cerrar", startLine, startCol));
                return string.Empty; // Devuelve cadena vacía si hay error
            }
            string str = _source[start.._position];
            Advance(); // omite la comilla final
            return str;
        }

        /// <summary>
        /// Determina si el caractéres es una letra válida del alfabeto español (incluyendo Ñ/ñ).
        /// </summary>
        /// <param name="c">El caractére a evaluar.</param>
        /// <returns>Verdadero si es una letra válida; de lo contrario, falso.</returns>
        private static bool IsSpanishLetter(char c)
        {
            return (c >= 'A' && c <= 'Z')
                || (c >= 'a' && c <= 'z')
                || c == 'Ñ'
                || c == 'ñ';
        }
    }

    /// <summary>
    /// Excepción específica para errores léxicos, que almacena el mensaje de error junto con la línea y columna.
    /// </summary>
    public class LexicalException : Exception
    {   
        /// <summary>
        /// Mensaje descriptivo del error léxico.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Número de línea en el que ocurrió el error.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Número de columna en el que ocurrió el error.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <c>LexicalException</c> con el mensaje y la ubicación del error.
        /// </summary>
        /// <param name="message">Descripción del error.</param>
        /// <param name="line">Línea donde ocurrió el error.</param>
        /// <param name="column">Columna donde ocurrió el error.</param>
        public LexicalException(string message, int line, int column)
        { 
            Message = message;
            Line = line;
            Column = column;
        }
    }
}