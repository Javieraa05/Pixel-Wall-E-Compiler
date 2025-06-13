namespace Wall_E.Compiler
{
    public abstract class Stmt : ASTNode
    {
        // Método Accept genérico para el patrón Visitor
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    public class ExpressionStmt : Stmt
    {
        public Expr Expression { get; }

        public ExpressionStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
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

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitSpawnStmt(this);
        }
    }
    public class ReSpawnStmt : Stmt
    {
        public Token Keyword;
        public Expr ExprX;
        public Expr ExprY;

        public ReSpawnStmt(Token token, Expr exprX, Expr exprY)
        {
            Keyword = token;
            ExprX = exprX;
            ExprY = exprY;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitReSpawnStmt(this);
        }
    }

    public class ColorStmt : Stmt
    {
        public Token Keyword;

        public Expr Color;

        public ColorStmt(Token keyword, Expr color)
        {
            Keyword = keyword;
            Color = color;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitColorStmt(this);
        }
    }

    public class SizeStmt : Stmt
    {
        public Token Keyword;
        public Expr Size;

        public SizeStmt(Token keyword, Expr expr)
        {
            Keyword = keyword;
            Size = expr;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitSizeStmt(this);
        }
    }
    public class DrawLineStmt : Stmt
    {
        public Token Keyword;
        public Expr DirX;
        public Expr DirY;
        public Expr Distance;

        public DrawLineStmt(Token keyword, Expr dirX, Expr dirY, Expr distance)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawLineStmt(this);
        }
    }
    public class DrawCircleStmt : Stmt
    {
        public Token Keyword;
        public Expr DirX;
        public Expr DirY;
        public Expr Radius;

        public DrawCircleStmt(Token keyword, Expr dirX, Expr dirY, Expr radius)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Radius = radius;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawCircleStmt(this);
        }
    }

    public class DrawRectangleStmt : Stmt
    {
        public Token Keyword;
        public Expr DirX;
        public Expr DirY;
        public Expr Distance;
        public Expr Width;
        public Expr Height;

        public DrawRectangleStmt(Token keyword, Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawRectangleStmt(this);
        }
    }

    public class FillStmt : Stmt
    {
        public Token Keyword;

        public FillStmt(Token token)
        {
            Keyword = token;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFillStmt(this);
        }
    }

     public class LabelStmt : Stmt
    {
        public Token Name { get; }

        public LabelStmt(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitLabelStmt(this);
        }
    }
    
    public class GoToStmt : Stmt
    {
        public Token Keyword;
        public Expr Label;
        public Expr Condition;

        public GoToStmt(Token keyword, Expr label, Expr condition)
        {
            Keyword = keyword;
            Label = label;
            Condition = condition;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGoToStmt(this);
        }
    }
    public class EmptyStmt : Stmt
    {
        public EmptyStmt() { }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitEmptyStmt(this);
        }
    }
}