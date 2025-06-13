using System;
using System.Collections.Generic;
using System.Globalization;


namespace Wall_E.Compiler
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        public bool hadError = false;
        private bool Spawned = false;
        public List<ParseException> _parseErrors { get; } = new List<ParseException>();

        private static readonly Dictionary<TokenType, int> arity = new()
    {
        { TokenType.Spawn,        2 },
        { TokenType.ReSpawn,      2 },
        { TokenType.Color,        1 },
        { TokenType.Size,         1 },
        { TokenType.DrawLine,     3 },
        { TokenType.DrawCircle,   3 },
        { TokenType.DrawRectangle,5 },
        { TokenType.Fill,         0 },
        { TokenType.GetActualX,   0 },
        { TokenType.GetActualY,   0 },
        { TokenType.GetCanvasSize,0 },
        { TokenType.GetColorCount,5 },
        { TokenType.IsBrushColor, 1 },
        { TokenType.IsBrushSize,  1 },
        { TokenType.IsCanvasColor,3 },
    };

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!IsAtEnd())
            {
                try
                {
                    var stmt = Statement();
                    if (stmt != null)
                        statements.Add(stmt);
                }
                catch (ParseException ex)
                {
                    // Capturar el error de parseo y agregarlo a la lista
                    _parseErrors.Add(ex);
                    hadError = true;
                    Synchronize();

                }
            }

            return statements;
        }

        private Stmt Statement()
        {
            if (Check(TokenType.Identifier) && (PeekNext().Type == TokenType.EOL || PeekNext().Type == TokenType.EOF))
            {
                Token labelToken = Advance();       // consume IDENTIFIER
                return new LabelStmt(labelToken);
            }
            if (Match(TokenType.EOL))
            {
                return null;
            }
            if (Match(TokenType.Spawn))
            {
                if (Spawned)
                {
                    throw Error(Previous(), "Solo se puede usar Spawn una vez.");
                }
                else
                {
                    Spawned = true;
                    return ParseSpawnStmt();
                }
            }
            if (Match(TokenType.ReSpawn,TokenType.Color, TokenType.Size,
                       TokenType.DrawLine, TokenType.DrawCircle, TokenType.DrawRectangle,
                       TokenType.Fill))
            {
                return (Stmt)CallStmt(Previous());
            }
            // 3) Si empieza con "GoTo"
            if (Match(TokenType.GoTo))
            {
                Stmt gotoStmt = ParseGoToStmt();
                return gotoStmt;
            }

            Stmt exprStmt = ExpressionStatement();
            return exprStmt;
    
        }

        private Stmt ParseSpawnStmt()
        {
            Token keyword = Previous();

            Consume(TokenType.LeftParen, "Esperaba '(' después de 'Spawn'.");

            Expr x = Expression();

            if (Check(TokenType.RightParen))
            {
                // Construimos un ParseException con mensaje más claro:
                throw Error(Peek(), "Falta el segundo parámetro de Spawn.");
            }

            Consume(TokenType.Comma, "Esperaba ',' entre los parámetros de Spawn.");
            Expr y = Expression();

            Consume(TokenType.RightParen, "Esperaba ')' después de los parámetros de Spawn.");    

            if (Peek().Type is TokenType.EOF) return new SpawnStmt(keyword, x, y);

            Consume(TokenType.EOL, "Esperaba un salto de línea después de la instrucción Spawn.");

            return new SpawnStmt(keyword, x, y);
        }

        private Stmt ParseGoToStmt()
        {
            Token keyword = Previous();
            Consume(TokenType.LeftBracket, "Esperaba '[' después de 'GoTo'.");
            Expr Label = Expression();
            Consume(TokenType.RightBracket, $"Esperaba ']' despues de Label.");

            Consume(TokenType.LeftParen, "Esperaba '(' después de 'GoTo'.");
            Expr Condition = Expression();

            Consume(TokenType.RightParen, "Esperaba ')' después de Condition.");

            if (Peek().Type is TokenType.EOF) return new GoToStmt(keyword, Label, Condition);

            Consume(TokenType.EOL, "Esperaba un salto de línea después de 'GoTo'.");

            return new GoToStmt(keyword, Label, Condition);
        }

        private ASTNode CallStmt(Token keyword)
        {
            // 1. Consumir '('
            Consume(TokenType.LeftParen, $"Esperaba '(' después de '{keyword.Lexeme}'.");

            // 2. Parsear lista de argumentos
            var args = new List<Expr>();
            if (!Check(TokenType.RightParen))
            {
                do
                {
                    args.Add(Expression());
                } while (Match(TokenType.Comma));
            }

            // 3. Consumir ')'
            Consume(TokenType.RightParen, $"Esperaba ')' para cerrar parámetros de '{keyword.Lexeme}'.");

            // 4. Verificar aridad
            int expected = arity[keyword.Type];
            if (args.Count != expected)
            {
                throw Error(keyword, $"'{keyword.Lexeme}' espera {expected} argumentos, pero recibió {args.Count}.");
            }

            // 6. Construir el Stmt adecuado
            return keyword.Type switch
            {
                TokenType.Spawn => new SpawnStmt(keyword, args[0], args[1]),
                TokenType.ReSpawn => new ReSpawnStmt(keyword, args[0], args[1]),
                TokenType.Color => new ColorStmt(keyword, args[0]),
                TokenType.Size => new SizeStmt(keyword, args[0]),
                TokenType.DrawLine => new DrawLineStmt(keyword, args[0], args[1], args[2]),
                TokenType.DrawCircle => new DrawCircleStmt(keyword, args[0], args[1], args[2]),
                TokenType.DrawRectangle => new DrawRectangleStmt(keyword, args[0], args[1], args[2], args[3], args[4]),
                TokenType.Fill => new FillStmt(keyword),
                TokenType.GetActualX => new GetActualXExpr(),
                TokenType.GetActualY => new GetActualYExpr(),
                TokenType.GetCanvasSize => new GetCanvasSizeExpr(),
                TokenType.GetColorCount => new GetColorCountExpr(keyword, args[0], args[1], args[2], args[3], args[4]),
                TokenType.IsBrushColor => new IsBrushColorExpr(keyword, args[0]),
                TokenType.IsBrushSize => new IsBrushSizeExpr(keyword, args[0]),
                TokenType.IsCanvasColor => new IsCanvasColorExpr(keyword, args[0], args[1], args[2]),
                _ => throw Error(keyword, $"Instrucción desconocida: {keyword.Lexeme}")
            };
        }
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();   
            ConsumeEOLorEOF($"Esperaba un salto de línea después de la expresión. {Previous().Lexeme}" );
            return new ExpressionStmt(expr);
        }

        private Expr Expression() => Assignment();

        private Expr Assignment()
        {
            Expr expr = Or();

            if (Match(TokenType.Assign))
            {
                Token op = Previous();
                Expr value = Assignment();

                if (expr is Identifier id)
                    return new Assign(id.Name, value);

                ConsumeEOLorEOF("Esperaba un salto de línea después de la asignación.");
                throw Error(op, "Objetivo de asignación no válido.");
            }

            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();
            while (Match(TokenType.Or))
            {
                Token op = Previous();
                Expr right = And();
                expr = new Logical(expr, op, right);
            }
            return expr;
        }

        private Expr And()
        {
            Expr expr = Equality();
            while (Match(TokenType.And))
            {
                Token op = Previous();
                Expr right = Equality();
                expr = new Logical(expr, op, right);
            }
            return expr;
        }

        private Expr Equality()
        {
            Expr expr = Comparison();
            while (Match(TokenType.EqualEqual))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();
            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.Plus, TokenType.Minus))
            {
                Token op = Previous();
                Expr right = Factor();
                if (right is EmptyExpr)
                {
                    // Si la expresión es EmptyExpr, no podemos aplicar Term
                    throw Error(Peek(), "No se puede aplicar operacion a una expresión vacía.");
                }
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Power();
            while (Match(TokenType.Star, TokenType.Slash, TokenType.Modulo))
            {
                Token op = Previous();
                Expr right = Power();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Power()
        {
            Expr expr = Unary();
            if(expr is EmptyExpr)
            {
                // Si la expresión es EmptyExpr, no podemos aplicar Power
                throw Error(Peek(), "No se puede aplicar operacion a una expresión vacía.");
            }
            if (Match(TokenType.Power))
            {
                Token op = Previous();
                Expr right = Power();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(TokenType.Minus))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Unary(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.Number))
            {
                int value = int.Parse(Previous().Lexeme, CultureInfo.InvariantCulture);
                return new Literal(value);
            }
            if (Match(TokenType.String))
            {
                return new StringLiteral(Previous().Lexeme);
            }
            if (Match(TokenType.LeftParen))
            {
                Expr expr = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression.");
                return new Grouping(expr);
            }
            if (Match(TokenType.Identifier))
            {
                Token name = Previous();
                return new Identifier(name);
            }
            if (Match(TokenType.GetActualX, TokenType.GetActualY, TokenType.GetCanvasSize,
                       TokenType.GetColorCount, TokenType.IsBrushColor, TokenType.IsBrushSize,
                       TokenType.IsCanvasColor))
            {
                return (Expr)CallStmt(Previous());            
            }

            

            throw Error(Peek(), $"{Peek().Lexeme} no es una expresión válida.");

        }

        // Métodos auxiliares
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        private Token Peek() => tokens[current];
        private Token PeekNext() => tokens[current+1];

        private Token Previous() => tokens[current - 1];

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, string message)
        {
            return new ParseException(message, token.Line, token.Column);
        }

        private void Synchronize()
        {
             Advance();
    while (!IsAtEnd())
    {
        if (Previous().Type == TokenType.EOL)
            return;
        switch (Peek().Type)
        {
            case TokenType.Spawn:
            case TokenType.Color:
            case TokenType.Size:
            case TokenType.DrawLine:
            case TokenType.DrawCircle:
            case TokenType.DrawRectangle:
            case TokenType.Fill:
            case TokenType.GoTo:
                return;
        }
        Advance();
    }

        }
        /// <summary>
        /// Verifica que el token actual sea EOL; si es así, lo consume. 
        /// Si es EOF, también se da por válido. 
        /// En caso contrario, lanza ParseException con el mensaje indicado.
        /// </summary>
        private void ConsumeEOLorEOF(string errorMessage)
        {
            if (Peek().Type == TokenType.EOL)
            {
                Advance(); // consumir el EOL
            }
            else if (Peek().Type == TokenType.EOF)
            {
                // No hacemos nada, ya acabó el archivo
            }
            else
            {
                throw Error(Peek(), errorMessage);
            }

        }


    }

    // Excepción usada para errores de parseo
    public class ParseException : Exception
    {
        public string Message { get; }
        public int Line { get; }
        public int Column { get; }
        public ParseException(string message = "Error de parseo", int line = 0, int column = 0)
        {
            Message = message;
            Line = line;
            Column = column;
        }       
    }
     
    
}