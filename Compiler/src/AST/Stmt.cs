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

