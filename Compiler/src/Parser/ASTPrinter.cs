using System;
using System.Collections.Generic;

public class AstPrinter : IVisitor<string>
{
    public string Print(ASTNode node)
    {
        return node.Accept(this);
    }

    // Implementación para ProgramNode
    public string VisitProgramNode(ProgramNode program)
    {
        var parts = new List<string> { "(program" };
        foreach (var stmt in program.Statements)
        {
            parts.Add(" ");
            parts.Add(stmt.Accept(this));
        }
        parts.Add(")");
        return string.Join("", parts);
    }

    // Implementación para ExpressionStatement
    public string VisitExpressionStatement(ExpressionStatement stmt)
    {
        return stmt.Expression.Accept(this);
    }

    // Métodos para expresiones
    public string VisitBinaryExpr(Binary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Literal expr)
    {
        return expr.Value?.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Unary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    public string VisitIdentifier(Identifier identifier)
    {
        return identifier.Name.Lexeme;
    }

    public string VisitLogicalExpr(Logical expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitAssignExpr(Assign expr)
    {
        return Parenthesize("=", new Identifier(expr.Name), expr.Value);
    }

    // Método auxiliar para formatear en notación prefija
    private string Parenthesize(string name, params Expr[] exprs)
    {
        var parts = new List<string> { "(" + name };
        foreach (var e in exprs)
        {
            parts.Add(" ");
            parts.Add(e.Accept(this));
        }
        parts.Add(")");
        return string.Join("", parts);
    }
}
