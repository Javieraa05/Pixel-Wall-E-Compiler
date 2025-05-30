using System;
using System.Collections.Generic;

public class Interpreter : IVisitor<object>
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
        }
        catch (RuntimeError error)
        {
            ReportRuntimeError(error);
        }
    }
    // Método que inicia la interpretación de un programa completo:
    public object VisitProgramNode(ProgramNode program)
    {
        object last = null;
        foreach (var stmt in program.Statements)
        {
            last = stmt.Accept(this);
            Console.WriteLine(Stringify(last));
        }
        return last;
    }

    public object VisitIdentifier(Identifier id)
    {
        // Recupera el valor de la variable
        return env.Get(id.Name.Lexeme);
    }

    // Ejecutar una sentencia de expresión:
    public object VisitExpressionStmt(ExpressionStmt stmt)
    {
        object value = SafeEvaluate(stmt.Expression);
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

    public object VisitSpawnStmt(SpawnStmt spawnStmt)
    {
        var X = (int)SafeEvaluate(spawnStmt.ExprX);
        var Y = (int)SafeEvaluate(spawnStmt.ExprY);

        return $"Wall-E comienza en ({X}, {Y}) ";
    }

    public object VisitColorStmt(ColorStmt colorStmt)
    {
        var Color = (string)SafeEvaluate(colorStmt.Color);
        return $"Brocha cambiada a {Color}";
    }
    public object VisitSizeStmt(SizeStmt sizeStmt)
    {
        var Size = (int)SafeEvaluate(sizeStmt.Size);
        return $"Tamanno de brocha cambiado a {Size}";
    }
    public object VisitDrawLineStmt(DrawLineStmt drawLineStmt)
    {
        var X = (int)SafeEvaluate(drawLineStmt.DirX);
        var Y = (int)SafeEvaluate(drawLineStmt.DirY);
        var Distance = (int)SafeEvaluate(drawLineStmt.Distance);

        CheckValidDirection(drawLineStmt.Keyword, X, Y);

        return $"Wall-E pinta linea en ({X}, {Y}) de distancia {Distance}";
    }
    public object VisitDrawCircleStmt(DrawCircleStmt drawCircleStmt)
    {
        var X = (int)SafeEvaluate(drawCircleStmt.DirX);
        var Y = (int)SafeEvaluate(drawCircleStmt.DirY);
        var Radius = (int)SafeEvaluate(drawCircleStmt.Radius);

        CheckValidDirection(drawCircleStmt.Keyword, X, Y);

        return $"Wall-E pinta circulo en ({X}, {Y}) de radio {Radius}";
    }
    public object VisitDrawRectangleStmt(DrawRectangleStmt drawRectangleStmt)
    {
        var X = (int)SafeEvaluate(drawRectangleStmt.DirX);
        var Y = (int)SafeEvaluate(drawRectangleStmt.DirY);
        var Distance = (int)SafeEvaluate(drawRectangleStmt.Distance);
        var Width = (int)SafeEvaluate(drawRectangleStmt.Width);
        var Height = (int)SafeEvaluate(drawRectangleStmt.Height);

        CheckValidDirection(drawRectangleStmt.Keyword, X, Y);

        return $"Wall-E pinta rectangulo en ({X}, {Y}) a {Distance} de ancho {Width} y largo {Height}";

    }
    public object VisitFillStmt(FillStmt fillStmt)
    {
        return "Fill";
    }
    public object VisitGetActualXStmt(GetActualXStmt getActualXNode)
    {
        return "X actual";
    }
    public object VisitGetActualYStmt(GetActualYStmt getActualYNode)
    {
        return "Y actual";
    }
    public object VisitGetCanvasSizeStmt(GetCanvasSizeStmt getCanvasSizeNode)
    {
        return "Tamanno del canvas";
    }
    public object VisitGetColorCountStmt(GetColorCountStmt getColorCountNode)
    {
        var Color = (string)SafeEvaluate(getColorCountNode.Color);
        var X1 = (int)SafeEvaluate(getColorCountNode.X1);
        var Y1 = (int)SafeEvaluate(getColorCountNode.Y1);
        var X2 = (int)SafeEvaluate(getColorCountNode.X2);
        var Y2 = (int)SafeEvaluate(getColorCountNode.Y2);

        return $"Cantidad de {Color} de ({X1}, {X2}) a ({X2},{Y2}) ";
    }
    public object VisitIsBrushColorStmt(IsBrushColorStmt isBrushColorNode)
    {
        var Color = (string)SafeEvaluate(isBrushColorNode.Color);
        return $"Es la brocha {Color}?";
    }
    public object VisitIsBrushSizeStmt(IsBrushSizeStmt isBrushSizeNode)
    {
        var Size = (int)SafeEvaluate(isBrushSizeNode.Size);

        return $"Es la brocha de tamanno {Size}?";
    }
    public object VisitIsCanvasColorStmt(IsCanvasColorStmt isCanvasColorNode)
    {
        var Color = (string)SafeEvaluate(isCanvasColorNode.Color);
        var Vertical = (int)SafeEvaluate(isCanvasColorNode.Vertical);
        var Horizontal = (int)SafeEvaluate(isCanvasColorNode.Horizontal);

        return $"Es la casilla en {Vertical} y {Horizontal} {Color}?";
    }
    public object VisitGoToStmt(GoToStmt GoToNode) => string.Empty;


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
            case Identifier i: return i.Name;
            default: return new Token(TokenType.None, "", 0, 0);
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
    private void CheckValidDirection(Token operatorToken, int X, int Y)
    {
        if (!(X == 1 || X == -1) || !(Y == 1 || Y == -1))
        {
            throw new RuntimeError(operatorToken, "Direccion invalida");
        }     
    }
}


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
