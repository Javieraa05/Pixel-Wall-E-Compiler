using System;
using System.Collections.Generic;
using System.Globalization;

// Parser con soporte de asignación, lógica y potencia
public class Parser
{
    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    /// <summary>
    /// Entrada principal: comienza con expresión de asignación.
    /// </summary>
    public Expr Parse()
    {
        return Expression();
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
            return new Unary(op.Lexeme, right);
        }
        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.Number))
        {
            double value = double.Parse(Previous().Lexeme, CultureInfo.InvariantCulture);
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
}

// Excepción usada para errores de parseo
public class ParseError : Exception {}
