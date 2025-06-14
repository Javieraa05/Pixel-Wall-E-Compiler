using System;
using System.Collections.Generic;
using System.Globalization;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Clase Parser que transforma una lista de tokens en un árbol de sintaxis abstracta (AST).
    /// Se encarga de detectar errores de parseo y generar las sentencias (Stmt) adecuadas.
    /// </summary>
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        /// <summary>
        /// Indica si se ha encontrado algún error de parseo.
        /// </summary>
        public bool hadError = false;
        private bool Spawned = false;
        /// <summary>
        /// Lista de excepciones de parseo encontradas durante el proceso.
        /// </summary>
        public List<ParseException> _parseErrors { get; } = new List<ParseException>();

        /// <summary>
        /// Diccionario que define la cantidad de argumentos (aridad) para cada comando.
        /// </summary>
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

        /// <summary>
        /// Inicializa una nueva instancia del Parser con la lista de tokens proporcionada.
        /// </summary>
        /// <param name="tokens">Lista de tokens generada por el Lexer.</param>
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        /// <summary>
        /// Método principal para parsear la lista de tokens y generar una lista de sentencias (Stmt).
        /// Captura errores de parseo y sincroniza la lectura en caso de error.
        /// </summary>
        /// <returns>Lista de sentencias parseadas.</returns>
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

        /// <summary>
        /// Parsea una sentencia individual según el token actual.
        /// </summary>
        /// <returns>Una sentencia (Stmt) parseada o nula si se trata de un salto de línea.</returns>
        private Stmt Statement()
        {
            // Si es una etiqueta que finaliza en EOL o EOF
            if (Check(TokenType.Identifier) && (PeekNext().Type == TokenType.EOL || PeekNext().Type == TokenType.EOF))
            {
                Token labelToken = Advance();       // Consume el IDENTIFIER
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
            if (Match(TokenType.ReSpawn, TokenType.Color, TokenType.Size,
                       TokenType.DrawLine, TokenType.DrawCircle, TokenType.DrawRectangle,
                       TokenType.Fill))
            {
                return (Stmt)CallStmt(Previous());
            }
            // Si empieza con "GoTo"
            if (Match(TokenType.GoTo))
            {
                Stmt gotoStmt = ParseGoToStmt();
                return gotoStmt;
            }

            Stmt exprStmt = ExpressionStatement();
            return exprStmt;
        }

        /// <summary>
        /// Parsea la sentencia "Spawn", validando la aridad y estructura de la misma.
        /// </summary>
        /// <returns>Una instancia de SpawnStmt.</returns>
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

        /// <summary>
        /// Parsea la sentencia "GoTo" validando su sintaxis y estructura.
        /// </summary>
        /// <returns>Una instancia de GoToStmt.</returns>
        private Stmt ParseGoToStmt()
        {
            Token keyword = Previous();
            Consume(TokenType.LeftBracket, "Esperaba '[' después de 'GoTo'.");
            Expr Label = Expression();
            Consume(TokenType.RightBracket, $"Esperaba ']' después de Label.");

            Consume(TokenType.LeftParen, "Esperaba '(' después de 'GoTo'.");
            Expr Condition = Expression();

            Consume(TokenType.RightParen, "Esperaba ')' después de Condition.");

            if (Peek().Type is TokenType.EOF) return new GoToStmt(keyword, Label, Condition);

            Consume(TokenType.EOL, "Esperaba un salto de línea después de 'GoTo'.");

            return new GoToStmt(keyword, Label, Condition);
        }

        /// <summary>
        /// Parsea una sentencia de llamada a función/comando.
        /// Consume los paréntesis y separadores, y valida la cantidad de argumentos de acuerdo a la aridad definida.
        /// </summary>
        /// <param name="keyword">El token de la instrucción a la que se hará la llamada.</param>
        /// <returns>Una instancia de Stmt o Expr según corresponda.</returns>
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

            // 5. Construir el Stmt o Expr adecuado según el tipo de comando
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

        /// <summary>
        /// Parsea una sentencia de expresión.
        /// Valida que la expresión termine con un EOL o EOF.
        /// </summary>
        /// <returns>Una instancia de ExpressionStmt.</returns>
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();   
            ConsumeEOLorEOF($"Esperaba un salto de línea después de la expresión. {Previous().Lexeme}" );
            return new ExpressionStmt(Previous(),expr);
        }

        /// <summary>
        /// Comienza a parsear una expresión.
        /// </summary>
        /// <returns>La expresión parseada.</returns>
        private Expr Expression() => Assignment();

        /// <summary>
        /// Parsea una asignación. Si se encuentra un token de asignación, se crea un nodo de asignación.
        /// </summary>
        /// <returns>La expresión resultante, que puede ser una asignación o una expresión lógica.</returns>
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

        /// <summary>
        /// Parsea expresiones lógicas OR.
        /// </summary>
        /// <returns>La expresión OR parseada.</returns>
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

        /// <summary>
        /// Parsea expresiones lógicas AND.
        /// </summary>
        /// <returns>La expresión AND parseada.</returns>
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

        /// <summary>
        /// Parsea comparaciones de igualdad (==).
        /// </summary>
        /// <returns>La expresión de igualdad parseada.</returns>
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

        /// <summary>
        /// Parsea expresiones de comparación (>, >=, <, <=).
        /// </summary>
        /// <returns>La expresión de comparación parseada.</returns>
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

        /// <summary>
        /// Parsea términos (operaciones de suma y resta).
        /// </summary>
        /// <returns>La expresión del término parseada.</returns>
        private Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.Plus, TokenType.Minus))
            {
                Token op = Previous();
                Expr right = Factor();
                if (right is EmptyExpr)
                {
                    throw Error(Peek(), "No se puede aplicar operacion a una expresión vacía.");
                }
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        /// <summary>
        /// Parsea factores (operaciones de multiplicación, división y módulo).
        /// </summary>
        /// <returns>La expresión del factor parseada.</returns>
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

        /// <summary>
        /// Parsea operaciones de potencia.
        /// </summary>
        /// <returns>La expresión de potencia parseada.</returns>
        private Expr Power()
        {
            Expr expr = Unary();
            if (expr is EmptyExpr)
            {
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

        /// <summary>
        /// Parsea expresiones unarias (por ejemplo, negación).
        /// </summary>
        /// <returns>La expresión unaria parseada.</returns>
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

        /// <summary>
        /// Parsea los elementos primarios: números, cadenas, agrupaciones, identificadores y llamadas.
        /// </summary>
        /// <returns>La expresión primaria parseada.</returns>
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
                Consume(TokenType.RightParen, "Esperaba ')' después de la expresión.");
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

        // Métodos auxiliares de parseo

        /// <summary>
        /// Comprueba si el token actual es de alguno de los tipos especificados. Si es así, lo consume.
        /// </summary>
        /// <param name="types">Tipos de token a comparar.</param>
        /// <returns>Verdadero si se consumió alguno de los tokens; de lo contrario, falso.</returns>
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

        /// <summary>
        /// Verifica si el token actual coincide con el tipo especificado.
        /// </summary>
        /// <param name="type">El tipo de token a verificar.</param>
        /// <returns>Verdadero si coincide; de lo contrario, falso.</returns>
        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;

        /// <summary>
        /// Avanza el cursor y retorna el token previamente consumido.
        /// </summary>
        /// <returns>El token consumido anteriormente.</returns>
        private Token Advance()
        {
            if (!IsAtEnd())
                current++;
            return Previous();
        }

        /// <summary>
        /// Indica si se alcanzó el final de la lista de tokens.
        /// </summary>
        /// <returns>Verdadero si el token actual es EOF; de lo contrario, falso.</returns>
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;

        /// <summary>
        /// Retorna el token actual sin consumirlo.
        /// </summary>
        /// <returns>El token actual.</returns>
        private Token Peek() => tokens[current];

        /// <summary>
        /// Retorna el token siguiente al actual.
        /// </summary>
        /// <returns>El token siguiente.</returns>
        private Token PeekNext() => tokens[current + 1];

        /// <summary>
        /// Retorna el token inmediatamente anterior al token actual.
        /// </summary>
        /// <returns>El token anterior.</returns>
        private Token Previous() => tokens[current - 1];

        /// <summary>
        /// Consume el token actual si coincide con el tipo especificado; de lo contrario, lanza un ParseException con el mensaje dado.
        /// </summary>
        /// <param name="type">El tipo de token esperado.</param>
        /// <param name="message">Mensaje de error a mostrar.</param>
        /// <returns>El token consumido.</returns>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
                return Advance();
            throw Error(Peek(), message);
        }

        /// <summary>
        /// Crea y retorna un ParseException basado en el token y mensaje proporcionados.
        /// </summary>
        /// <param name="token">El token en el que ocurrió el error.</param>
        /// <param name="message">Descripción del error.</param>
        /// <returns>Una instancia de ParseException.</returns>
        private ParseException Error(Token token, string message)
        {
            return new ParseException(message, token.Line, token.Column);
        }

        /// <summary>
        /// Sincroniza el parser para continuar parseando después de un error.
        /// Avanza hasta encontrar un salto de línea o una palabra clave que indique el comienzo de una nueva sentencia.
        /// </summary>
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
        /// Si es EOF, se da por válido.
        /// En caso contrario, lanza un ParseException con el mensaje indicado.
        /// </summary>
        /// <param name="errorMessage">Mensaje de error si no se cumple la condición.</param>
        private void ConsumeEOLorEOF(string errorMessage)
        {
            if (Peek().Type == TokenType.EOL)
            {
                Advance(); // consumir el EOL
            }
            else if (Peek().Type == TokenType.EOF)
            {
                // Fin del archivo, es aceptable
            }
            else
            {
                throw Error(Peek(), errorMessage);
            }
        }
    }

    /// <summary>
    /// Excepción utilizada para errores de parseo.
    /// Almacena el mensaje, línea y columna donde ocurrió el error.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Mensaje descriptivo del error de parseo.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Línea en la que ocurrió el error.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Columna en la que ocurrió el error.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <c>ParseException</c> con un mensaje y la ubicación del error.
        /// </summary>
        /// <param name="message">Descripción del error.</param>
        /// <param name="line">Línea donde se detectó el error.</param>
        /// <param name="column">Columna donde se detectó el error.</param>
        public ParseException(string message = "Error de parseo", int line = 0, int column = 0)
        {
            Message = message;
            Line = line;
            Column = column;
        }       
    }
}