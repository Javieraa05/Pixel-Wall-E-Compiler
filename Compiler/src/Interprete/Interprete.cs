
public class Interprete : IVisitor<Object>
{
    public Object VisitLiteralExpr(Literal literal)
    {
        return literal.Value;
    }

    public Object VisitGroupingExpr(Grouping grouping)
    {
        return Evaluate(grouping.Expression);
    }

    public Object VisitUnaryExpr(Unary expr)
    {
        Object right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Plus:
                checkedNumberOperand(expr.Operator, right);
                return (double)right;

            case TokenType.Minus:
                checkedNumberOperand(expr.Operator, right);
                return -(double)right;
            default:
                throw new RuntimeError(expr.Operator, $"Operador no soportado: {expr.Operator}");

        }
    }

    public Object VisitBinaryExpr(Binary expr)
    {
        Object left = Evaluate(expr.Left);
        Object right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Plus:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left + (double)right;

            case TokenType.Minus:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left - (double)right;

            case TokenType.Star:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left * (double)right;

            case TokenType.Slash:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left / (double)right;

            case TokenType.Greater:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left > (double)right;

            case TokenType.Less:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left < (double)right;

            case TokenType.GreaterEqual:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left >= (double)right;

            case TokenType.LessEqual:
                checkedNumberOperands(expr.Operator, left, right);
                return (double)left <= (double)right;

            case TokenType.EqualEqual:
                return left.Equals(right);

            case TokenType.And:
                return (bool)left && (bool)right;

            case TokenType.Or:
                return (bool)left || (bool)right;

            default:
                throw new RuntimeError(expr.Operator, $"Operador no soportado: {expr.Operator}");
        }
    }

    private void checkedNumberOperand(Token operatorToken, Object operand)
    {
        if (!(operand is double))
        {
            throw new RuntimeError(operatorToken, "Operador tiene que ser de tipo numero.");
        }
    }

    private void checkedNumberOperands(Token operatorToken, Object left, Object right)
    {
        if (!(left is double) || !(right is double))
        {
            throw new RuntimeError(operatorToken, "Both operands must be numbers.");
        }
    }

    private Object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

}

class RuntimeError : Exception
{
    public Token Token { get; }

    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
} 
