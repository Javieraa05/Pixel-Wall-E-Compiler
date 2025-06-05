using System.Collections.Generic;
namespace Wall_E.Compiler
{
    public abstract class Expr : ASTNode
    {
        public abstract T Accept<T>(IExprVisitor<T> visitor);
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

        public override T Accept<T>(IExprVisitor<T> visitor)
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

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    public class Literal : Expr
    {
        public int Value { get; }

        public Literal(int value)
        {
            Value = value;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }
    public class StringLiteral : Expr
    {
        public string Value { get; }
        public StringLiteral(string value)
        {
            Value = value;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitStringLiteralExpr(this);
        }
    }


    public class Unary : Expr
    {
        public Token Operator { get; }
        public Expr Right { get; }

        public Unary(Token op, Expr right)
        {
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    public class Identifier : Expr
    {
        public Token Name { get; }
        public Identifier(Token name)
        {
            Name = name;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
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
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAssignExpr(this);
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
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLogicalExpr(this);
    }
    public class GetActualXExpr : Expr
    {
        public GetActualXExpr() { }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetActualXExpr(this);
        }
    }

    public class GetActualYExpr : Expr
    {
        public GetActualYExpr() { }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetActualYExpr(this);
        }
    }

    public class GetCanvasSizeExpr : Expr
    {
        public GetCanvasSizeExpr() { }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetCanvasSizeExpr(this);
        }
    }

    public class GetColorCountExpr : Expr
    {
        public Token Keyword;
        public Expr Color { get; }
        public Expr X1 { get; }
        public Expr Y1 { get; }
        public Expr X2 { get; }
        public Expr Y2 { get; }

        public GetColorCountExpr(Token keyword, Expr color, Expr x1, Expr y1, Expr x2, Expr y2)
        {
            Keyword = keyword;
            Color = color;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetColorCountExpr(this);
        }
    }

    public class IsBrushColorExpr : Expr
    {
        public Token Keyword;
        public Expr Color { get; }

        public IsBrushColorExpr(Token keyword, Expr color)
        {
            Keyword = keyword;
            Color = color;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIsBrushColorExpr(this);
        }
    }

    public class IsBrushSizeExpr : Expr
    {
        public Token Keyword;
        public Expr Size { get; }

        public IsBrushSizeExpr(Token keyword, Expr size)
        {
            Keyword = keyword;
            Size = size;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIsBrushSizeExpr(this);
        }
    }

    public class IsCanvasColorExpr : Expr
    {
        public Token Keyword;
        public Expr Color { get; }
        public Expr Vertical { get; }
        public Expr Horizontal { get; }

        public IsCanvasColorExpr(Token keyword, Expr color, Expr vertical, Expr horizontal)
        {
            Keyword = keyword;
            Color = color;
            Vertical = vertical;
            Horizontal = horizontal;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIsCanvasColorExpr(this);
        }
    }

   
    public class EmptyExpr : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitEmptyExpr(this);
        }
    }
}