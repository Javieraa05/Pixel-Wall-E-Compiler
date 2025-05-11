// Definición de nodos del Árbol de Sintaxis Abstracta (AST)
public abstract class ASTNode
{
    // Método Accept genérico para el patrón Visitor
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public abstract class Statement : ASTNode { }

// Nodo raíz que contiene una lista de sentencias
public class ProgramNode : ASTNode
{
    public List<Statement> Statements { get; } = new List<Statement>();

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitProgramNode(this);
    }
}

// Ejemplo de una sentencia concreta: expresión como sentencia
public class ExpressionStatement : Statement
{
    public Expr Expression { get; }

    public ExpressionStatement(Expr expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitExpressionStatement(this);
    }
}

// 1) Interfaz de visitante con tipo de retorno genérico
public interface IVisitor<T>
{
    // Métodos para nodos de expresión
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);

    // Métodos para nodos de AST general
    T VisitProgramNode(ProgramNode program);
    T VisitExpressionStatement(ExpressionStatement stmt);
    // Añadir más VisitX para nuevas clases de Statement u otros nodos
}

// 2) Clase abstracta base Expr
public abstract class Expr : ASTNode
{
    public abstract override T Accept<T>(IVisitor<T> visitor);
}

// 3) Subclases concretas de Expr
public class Binary : Expr
{
    public Expr Left { get; }
    public string Operator { get; }
    public Expr Right { get; }

    public Binary(Expr left, string op, Expr right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpr(this);
    }
}

public class Grouping : Expr
{
    public Expr Expression { get; }

    public Grouping(Expr expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpr(this);
    }
}

public class Literal : Expr
{
    public object Value { get; }

    public Literal(object value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}

public class Unary : Expr
{
    public string Operator { get; }
    public Expr Right { get; }

    public Unary(string op, Expr right)
    {
        Operator = op;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}

// 4) Ejemplo: visitor que genera una representación en texto del AST
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
        return Parenthesize(expr.Operator, expr.Left, expr.Right);
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
        return Parenthesize(expr.Operator, expr.Right);
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

