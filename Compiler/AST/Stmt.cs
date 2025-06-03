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
        public FillStmt()
        { }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFillStmt(this);
        }
    }

    public class GetActualXStmt : Stmt
    {
        public GetActualXStmt() { }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGetActualXStmt(this);
        }
    }

    public class GetActualYStmt : Stmt
    {
        public GetActualYStmt() { }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGetActualYStmt(this);
        }
    }

    public class GetCanvasSizeStmt : Stmt
    {
        public GetCanvasSizeStmt() { }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGetCanvasSizeStmt(this);
        }
    }

    public class GetColorCountStmt : Stmt
    {
        public Token Keyword;
        public Expr Color { get; }
        public Expr X1 { get; }
        public Expr Y1 { get; }
        public Expr X2 { get; }
        public Expr Y2 { get; }

        public GetColorCountStmt(Token keyword, Expr color, Expr x1, Expr y1, Expr x2, Expr y2)
        {
            Keyword = keyword;
            Color = color;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGetColorCountStmt(this);
        }
    }

    public class IsBrushColorStmt : Stmt
    {
        public Token Keyword;
        public Expr Color { get; }

        public IsBrushColorStmt(Token keyword, Expr color)
        {
            Keyword = keyword;
            Color = color;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIsBrushColorStmt(this);
        }
    }

    public class IsBrushSizeStmt : Stmt
    {
        public Token Keyword;
        public Expr Size { get; }

        public IsBrushSizeStmt(Token keyword, Expr size)
        {
            Keyword = keyword;
            Size = size;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIsBrushSizeStmt(this);
        }
    }

    public class IsCanvasColorStmt : Stmt
    {
        public Token Keyword;
        public Expr Color { get; }
        public Expr Vertical { get; }
        public Expr Horizontal { get; }

        public IsCanvasColorStmt(Token keyword, Expr color, Expr vertical, Expr horizontal)
        {
            Keyword = keyword;
            Color = color;
            Vertical = vertical;
            Horizontal = horizontal;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIsCanvasColorStmt(this);
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