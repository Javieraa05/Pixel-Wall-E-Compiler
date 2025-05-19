public abstract class Stmt : ASTNode
{
    // Método Accept genérico para el patrón Visitor
    public abstract override T Accept<T>(IVisitor<T> visitor);
}

public class ExpressionStmt : Stmt
{
    public Expr Expression { get; }

    public ExpressionStmt(Expr expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitExpressionStmt(this);
    }

}

public class SpawnStmt : Stmt
{
    public Token Keyword;
    public Expr ExprX;
    public Expr ExprY;

    public SpawnStmt(Token token, Expr exprX, Expr exprY)
    {
        Keyword = token;
        ExprX = exprX;
        ExprY = exprY;

    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitSpawnStmt(this);
    }
}



