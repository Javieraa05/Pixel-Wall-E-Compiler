using System;
using System.Collections.Generic;
using System.Globalization;

// Excepción específica para errores léxicos
public class LexicalException : Exception
{
    public LexicalException(string message) : base(message) { }
}

// Enum que define los tipos de tokens, incluyendo palabras reservadas, operadores y símbolos.
public enum TokenType
{
    // Comandos / Palabras reservadas
    Spawn,
    Color,
    Size,
    DrawLine,
    DrawCircle,
    DrawRectangle,
    Fill,
    GoTo,
    GetActualX,
    GetActualY,
    GetCanvasSize,
    GetColorCount,
    IsBrushColor,
    IsBrushSize,
    IsCanvasColor,

    // Operadores aritméticos y símbolos
    Plus,           // +
    Minus,          // -
    Star,           // *
    Slash,          // /
    Power,          // ** (potencia)
    Modulo,         // % (módulo)
    And,            // &&
    Or,             // ||
    EqualEqual,     // ==
    GreaterEqual,   // >=
    LessEqual,      // <=
    Greater,        // >
    Less,           // <
    Assign,         // ← o <-
    LeftParen,      // (
    RightParen,     // )
    LeftBracket,    // [
    RightBracket,   // ]
    Comma,          // ,

    // Literales y identificadores
    Number,
    String,
    Identifier,

    // Tokens especiales
    EOL,
    EOF
}

// Clase Token: almacena el tipo, lexema y posición del token.
public class Token
{
    public TokenType Type { get; }
    public string Lexeme { get; }
    public int Line { get; }
    public int Column { get; }

    public Token(TokenType type, string lexeme, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Line = line;
        Column = column;
    }

    public override string ToString()
        => $"[{Type}] '{Lexeme}' (Línea: {Line}, Col: {Column})";
}

// Lexer: realiza el análisis léxico transformando el código fuente en una lista de tokens.
public class Lexer
{
    private readonly string _source;
    private int _position;
    private int _line;
    private int _column;

    // Palabras reservadas, comparación case‑sensitive
    private static readonly Dictionary<string, TokenType> _keywords =
        new Dictionary<string, TokenType>(StringComparer.Ordinal)
    {
        {"Spawn", TokenType.Spawn },
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

    public Lexer(string source)
    {
        _source = source;
        _position = 0;
        _line = 1;
        _column = 1;
    }

    private char Current => _position < _source.Length ? _source[_position] : '\0';

    private void Advance()
    {
        if (Current == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }
        _position++;
    }

    private char Peek(int offset = 1)
    {
        int pos = _position + offset;
        return pos < _source.Length ? _source[pos] : '\0';
    }

    public List<Token> Lex()
    {
        var tokens = new List<Token>();

        while (_position < _source.Length)
        {
            SkipWhitespace();
            int startLine = _line, startCol = _column;
            if (_position >= _source.Length) break;

            char c = Current;

            // Número
            if (char.IsDigit(c))
            {
                string num = ReadNumber();
                tokens.Add(new Token(TokenType.Number, num, startLine, startCol));
                continue;
            }

            // Identificador o palabra reservada (debe empezar con letra española)
            if (IsSpanishLetter(c))
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
                        Advance(); Advance();
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
                        Advance(); Advance();
                        tokens.Add(new Token(TokenType.And, "&&", startLine, startCol));
                    }
                    else throw new LexicalException($"Carácter inesperado '&' en línea {_line}, columna {_column}");
                    break;

                case '|':
                    if (Peek() == '|')
                    {
                        Advance(); Advance();
                        tokens.Add(new Token(TokenType.Or, "||", startLine, startCol));
                    }
                    else throw new LexicalException($"Carácter inesperado '|' en línea {_line}, columna {_column}");
                    break;

                case '=':
                    if (Peek() == '=')
                    {
                        Advance(); Advance();
                        tokens.Add(new Token(TokenType.EqualEqual, "==", startLine, startCol));
                    }
                    else throw new LexicalException($"Carácter inesperado '=' en línea {_line}, columna {_column}");
                    break;

                case '>':
                    if (Peek() == '=')
                    {
                        tokens.Add(new Token(TokenType.GreaterEqual, ">=", startLine, startCol));
                        Advance(); Advance();
                    }
                    else tokens.Add(new Token(TokenType.Greater, ">", startLine, startCol));
                    Advance();
                    break;

                case '<':
                    if (Peek() == '=')
                    {
                        tokens.Add(new Token(TokenType.LessEqual, "<=", startLine, startCol));
                        Advance(); Advance();
                    }
                    else if (Peek() == '-')
                    {
                        tokens.Add(new Token(TokenType.Assign, "<-", startLine, startCol));
                        Advance(); Advance();
                    }
                    else tokens.Add(new Token(TokenType.Less, "<", startLine, startCol));
                    Advance();
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

                default:
                    throw new LexicalException($"Carácter inesperado '{Current}' en línea {_line}, columna {_column}");
            }
        }

        tokens.Add(new Token(TokenType.EOF, "", _line, _column));
        return tokens;
    }

    // Omite espacios y tabulaciones, pero no saltos de línea
    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(Current) && Current != '\n')
            Advance();
    }

    private string ReadNumber()
    {
        int start = _position;
        while (char.IsDigit(Current))
            Advance();
        return _source[start.._position];
    }

    private string ReadIdentifier()
    {
        int start = _position;
        // Primer carácter ya validado como letra española
        Advance();
        while (char.IsDigit(Current) || IsSpanishLetter(Current) || Current == '-')
            Advance();
        return _source[start.._position];
    }

    private string ReadString(int startLine, int startCol)
    {
        Advance(); // omite la comilla inicial
        int start = _position;
        while (Current != '"' && Current != '\0')
            Advance();
        if (Current != '"')
            throw new LexicalException($"Cadena sin cerrar iniciada en línea {startLine}, columna {startCol}");
        string str = _source[start.._position];
        Advance(); // omite la comilla final
        return str;
    }

    // Sólo letras A–Z, a–z y Ñ/ñ
    private static bool IsSpanishLetter(char c)
    {
        return (c >= 'A' && c <= 'Z')
            || (c >= 'a' && c <= 'z')
            || c == 'Ñ'
            || c == 'ñ';
    }
}
