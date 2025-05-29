using System;
using System.Collections.Generic;
using System.Globalization;


public class Parser
{
    private readonly List<Token> tokens;
    private int current = 0;
    public static bool hadError = false;
    
    private static readonly Dictionary<TokenType, int> arity = new()
    {
        { TokenType.Spawn,        2 },
        { TokenType.Color,        1 },
        { TokenType.Size,         1 },
        { TokenType.DrawLine,     3 },
        { TokenType.DrawCircle,   3 },
        { TokenType.DrawRectangle,5 },
        { TokenType.Fill,         0 },
        
    };

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new List<Stmt>();

        statements.Add(ParseSpawnStmt());

        while (!IsAtEnd())
        {
            try
            {
                statements.Add(Statement());
            }
            catch (ParseError)
            {
                Synchronize();
                hadError = true;
            }
        }
    
        return statements;
    }

    private Stmt Statement()
    {
         if (Match(TokenType.Spawn, TokenType.Color, TokenType.Size,
                  TokenType.DrawLine, TokenType.DrawCircle, TokenType.DrawRectangle, TokenType.Fill))
        {
            return CallStmt(Previous());
        }
        return ExpressionStatement();
    }

    private Stmt ParseSpawnStmt()
    {
        Token keyword = Consume(TokenType.Spawn,"Se esperaba Spawn");
        
        Consume(TokenType.LeftParen, "Esperaba '(' después de 'Spawn'.");

        Expr x = Expression();
        Consume(TokenType.Comma, "Esperaba ',' entre los parámetros de Spawn.");
        Expr y = Expression();

        Consume(TokenType.RightParen, "Esperaba ')' después de los parámetros de Spawn.");

        if(Peek().Type is TokenType.EOF) return new SpawnStmt(keyword, x, y);
        
        Consume(TokenType.EOL, "Esperaba un salto de línea después de la instrucción Spawn.");

        return new SpawnStmt(keyword, x, y);
    }
    private Stmt CallStmt(Token keyword)
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
            throw Error(keyword,
                $"'{keyword.Lexeme}' espera {expected} argumentos, pero recibió {args.Count}.");
        }

        // 5. Consumir EOL opcional
        if (!IsAtEnd() && Peek().Type == TokenType.EOL)
            Advance();

        // 6. Construir el Stmt adecuado
        return keyword.Type switch
        {
            TokenType.Spawn => new SpawnStmt(keyword, args[0], args[1]),
            TokenType.Color => new ColorStmt(args[0]),
            TokenType.Size => new SizeStmt(args[0]),
            TokenType.DrawLine => new DrawLineStmt(args[0], args[1], args[2]),
            TokenType.DrawCircle => new DrawCircleStmt(args[0], args[1], args[2]),
            TokenType.DrawRectangle => new DrawRectangleStmt(args[0], args[1], args[2], args[3], args[4]),
            TokenType.Fill => new FillStmt(),
            _ => throw Error(keyword, $"Instrucción desconocida: {keyword.Lexeme}")
        };
    }
    private Stmt ExpressionStatement()
    {
        Expr expr = Expression();
        if (IsAtEnd())
            return new ExpressionStmt(expr);
        // Si no es el final, se espera un salto de línea    
        Consume(TokenType.EOL, "Esperaba un salto de línea después de la expresión.");
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

            throw Error(op, "Invalid assignment target.");
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
            return new Literal(Previous().Lexeme);
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
        throw Error(Peek(), "Expected expression.");
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

    private Token Previous() => tokens[current - 1];

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        ReportError(token, message);
        return new ParseError();
    }

    private void ReportError(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
            Console.Error.WriteLine($"[line {token.Line}] Error at end: {message}");
        else
            Console.Error.WriteLine($"[line {token.Line}] Error at '{token.Lexeme}': {message}");
    }

    private void Synchronize()
{
    // Descartar el token que provocó el error
    Advance();

    while (!IsAtEnd())
    {
        // Si vemos el inicio de una nueva instrucción o control,
        // consideramos que ya estamos sincronizados
        switch (Peek().Type)
        {
            // Instrucciones del lenguaje
            case TokenType.Spawn:
            case TokenType.Color:
            case TokenType.Size:
            case TokenType.DrawLine:
            case TokenType.DrawCircle:
            case TokenType.DrawRectangle:
            case TokenType.Fill:

            // Declaración de variable
            case TokenType.Identifier:

            // Salto condicional
            case TokenType.GoTo:

                return;
        }
        Advance();
    }
}
}

// Excepción usada para errores de parseo
public class ParseError : Exception {}
