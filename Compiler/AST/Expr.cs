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
   
    public class EmptyExpr : Expr
    {
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitEmptyExpr(this);
        }
    }
}