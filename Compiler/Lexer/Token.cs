namespace Wall_E.Compiler
{
    /// <summary>
    /// Enumeración que define los tipos de tokens, incluyendo comandos, operadores, literales y símbolos especiales.
    /// </summary>
    public enum TokenType
    {
        // Comandos / Palabras reservadas
        Spawn,
        ReSpawn,
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

    /// <summary>
    /// Clase que representa un token obtenido durante el análisis léxico.
    /// Almacena el tipo del token, su lexema (contenido textual) y la posición (línea y columna) en el código fuente.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Tipo del token, según lo definido en <see cref="TokenType"/>.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Lexema o contenido textual del token.
        /// </summary>
        public string Lexeme { get; }

        /// <summary>
        /// Línea del código fuente donde se encontró el token.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Columna del código fuente donde se encontró el token.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Token"/> con sus propiedades.
        /// </summary>
        /// <param name="type">Tipo del token.</param>
        /// <param name="lexeme">Contenido textual del token.</param>
        /// <param name="line">Línea en la que se encontró el token.</param>
        /// <param name="column">Columna en la que se encontró el token.</param>
        public Token(TokenType type, string lexeme, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Devuelve una representación en cadena del token, que incluye su tipo, lexema y posición.
        /// </summary>
        /// <returns>Cadena que representa el token.</returns>
        public override string ToString()
            => $"[{Type}] '{Lexeme}' (Línea: {Line}, Col: {Column})";
    }
}