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

public class ColorStmt : Stmt
{
    public Expr Color;

    public ColorStmt(Expr color)
    {
        Color = color;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitColorStmt(this);
    }
}

public class SizeStmt : Stmt
{
    public Expr Expr;

    public SizeStmt(Expr expr)
    {
        Expr = expr;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitSizeStmt(this);
    }
}
public class DrawLineStmt : Stmt
{
    public Expr DirX;
    public Expr DirY;
    public Expr Distance;

    public DrawLineStmt(Expr dirX, Expr dirY, Expr distance)
    {
        DirX = dirX;
        DirY = dirY;
        Distance = distance;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitDrawLineStmt(this);
    }
}
public class DrawCircleStmt : Stmt
{
    public Expr DirX;
    public Expr DirY;
    public Expr Radius;

    public DrawCircleStmt(Expr dirX, Expr dirY, Expr radius)
    {
        DirX = dirX;
        DirY = dirY;
        Radius = radius;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitDrawCircleStmt(this);
    }
}

public class DrawRectangleStmt : Stmt
    {
        public Expr DirX;
        public Expr DirY;
        public Expr Distance;
        public Expr Width;
        public Expr Height;

        public DrawRectangleStmt(Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
        {
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitDrawRectangleStmt(this);
        }
    }

public class FillStmt : Stmt
{
            public FillStmt()
            {
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitFillStmt(this);
            }
}





