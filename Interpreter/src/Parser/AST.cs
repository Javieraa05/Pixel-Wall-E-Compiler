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


// 2) Clase abstracta base Expr
public abstract class Expr : ASTNode
{
    public abstract override T Accept<T>(IVisitor<T> visitor);
}

// 3) Subclases concretas de Expr
public class Binary : Expr
{
    public Expr Left { get; }
    public Token Operator { get; }
    public Expr Right { get; }

    public Binary(Expr left, Token op, Expr right)
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

public class Identifier : Expr {
    
    public Token Name { get; }
    public Identifier(Token name) { Name = name; }
    public override T Accept<T>(IVisitor<T> visitor) {
        return visitor.VisitIdentifier(this);
    }
}

public class Assign : Expr
{
    public Token Name { get; }
    public Expr Value { get; }
    public Assign(Token name, Expr value)
    {
        Name = name;
        Value = value;
    }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitAssignExpr(this);
}


public class Logical : Expr
{
    public Expr Left { get; }
    public Token Operator { get; }
    public Expr Right { get; }
    public Logical(Expr left, Token op, Expr right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitLogicalExpr(this);
}






