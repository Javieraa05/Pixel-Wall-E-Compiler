using System;
using System.Collections.Generic;

/*public class Interpreter : IVisitor<object>
{
    // Flag para código de salida
    public static bool hadRuntimeError = false;

    private readonly Environment env = new Environment();

    /// <summary>
    /// Punto de entrada del intérprete.
    /// </summary>
    public void Interpret(Expr expr)
    {
        // Reiniciar flags antes de cada ejecución
        hadRuntimeError = false;

        try
        {
            object result = SafeEvaluate(expr);
            Console.WriteLine(Stringify(result));
        }
        catch (RuntimeError error)
        {
            ReportRuntimeError(error);
        }
    }

    public object VisitIdentifier(Identifier id)
    {
        // Recupera el valor de la variable
        return env.Get(id.Name.Lexeme);
    }
    
     // Método que inicia la interpretación de un programa completo:
    public object VisitProgramNode(ProgramNode program)
    {
        object last = null;
        foreach (var stmt in program.Statements)
        {
            last = stmt.Accept(this);
        }
        return last;
    }

    // Ejecutar una sentencia de expresión:
    public object VisitExpressionStmt(ExpressionStmt stmt)
    {
        object value = SafeEvaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));  // opcional, si quieres imprimir cada resultado
        return value;
    }

    public object VisitAssignExpr(Assign expr)
    {
        // Evalúa el lado derecho
        object value = SafeEvaluate(expr.Value);
        // Guarda en el entorno
        env.Assign(expr.Name.Lexeme, value);
        return value;
    }

    public object VisitLogicalExpr(Logical expr)
    {
        object left = SafeEvaluate(expr.Left);

        if (expr.Operator.Type == TokenType.Or)
        {
            // OR: si el izquierdo es "true", retorna left sin evaluar right
            if (IsTruthy(left)) return left;
        }
        else
        {
            // AND: si el izquierdo es "false", retorna left sin evaluar right
            if (!IsTruthy(left)) return left;
        }

        // Si no cortocircuitó, evalúa y retorna right
        return SafeEvaluate(expr.Right);
    }

    /// <summary>
    /// Evalúa el árbol y traduce cualquier InvalidCastException en RuntimeError.
    /// </summary>
    private object SafeEvaluate(Expr expr)
    {
        try
        {
            return expr.Accept(this);
        }
        catch (InvalidCastException)
        {
            Token token = ExtractToken(expr);
            throw new RuntimeError(token, "Operación con tipos inválidos.");
        }
    }

    private Token ExtractToken(Expr expr)
    {
        switch (expr)
        {
            case Binary b: return b.Operator;
            case Unary u: return u.Operator;
            case Grouping g: return ExtractToken(g.Expression);
            case Identifier l: return l.Name;
            default: return new Token(TokenType.None, "", 0, 0);
        }
    }

    public object VisitLiteralExpr(Literal literal)
    {
        return literal.Value;
    }

    public object VisitGroupingExpr(Grouping grouping)
    {
        return SafeEvaluate(grouping.Expression);
    }

    public object VisitUnaryExpr(Unary expr)
    {
        object right = SafeEvaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Plus:
                CheckNumberOperand(expr.Operator, right);
                return (int)right;

            case TokenType.Minus:
                CheckNumberOperand(expr.Operator, right);
                return -(int)right;

            default:
                throw new RuntimeError(
                    expr.Operator,
                    $"Operador no soportado '{expr.Operator.Lexeme}'."
                );
        }
    }

    public object VisitBinaryExpr(Binary expr)
    {
        object left = SafeEvaluate(expr.Left);
        object right = SafeEvaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.Plus:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left + (int)right;

            case TokenType.Minus:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left - (int)right;

            case TokenType.Star:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left * (int)right;

            case TokenType.Power:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)Math.Pow((int)left, (int)right);

            case TokenType.Modulo:
                CheckNumberOperands(expr.Operator, left, right);
                if ((int)right == 0)
                    throw new RuntimeError(expr.Operator, "Módulo por cero.");
                return (int)left % (int)right;

            case TokenType.Slash:
                CheckNumberOperands(expr.Operator, left, right);
                if ((int)right == 0)
                    throw new RuntimeError(expr.Operator, "División por cero.");
                return (int)left / (int)right;

            case TokenType.Greater:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left > (int)right;

            case TokenType.Less:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left < (int)right;

            case TokenType.GreaterEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left >= (int)right;

            case TokenType.LessEqual:
                CheckNumberOperands(expr.Operator, left, right);
                return (int)left <= (int)right;

            case TokenType.EqualEqual:
                return left.Equals(right);

            case TokenType.And:
                return (bool)left && (bool)right;

            case TokenType.Or:
                return (bool)left || (bool)right;

            default:
                throw new RuntimeError(
                    expr.Operator,
                    $"Operador no soportado '{expr.Operator.Lexeme}'."
                );
        }
    }

    private void CheckNumberOperand(Token operatorToken, object operand)
    {
        if (!(operand is int))
            throw new RuntimeError(operatorToken, "El operando debe ser un número.");
    }

    private void CheckNumberOperands(Token operatorToken, object left, object right)
    {
        if (!(left is int) || !(right is int))
            throw new RuntimeError(operatorToken, "Ambos operandos deben ser números.");
    }

    private string Stringify(object obj)
    {
        return obj == null ? "null" : obj.ToString();
    }


    private void ReportRuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[línea {error.Token.Line}]");
        foreach (var t in error.CallStack)
        {
            Console.Error.WriteLine($"  at línea {t.Line} ('{t.Lexeme}')");
        }
        hadRuntimeError = true;
    }
    
        private bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        if (obj is bool b) return b;
        // Otros tipos (números, strings, etc.) siempre "verdaderos"
        return true;
    }

}

*/
public class RuntimeError : Exception
{
    public Token Token { get; }
    public List<Token> CallStack { get; } = new List<Token>();

    public RuntimeError(Token token, string message)
        : base(message)
    {
        Token = token;
        CallStack.Add(token);
    }

    
    public void Push(Token token) => CallStack.Add(token);
}
