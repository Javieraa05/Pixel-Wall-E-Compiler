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
    EOF,
    None
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
