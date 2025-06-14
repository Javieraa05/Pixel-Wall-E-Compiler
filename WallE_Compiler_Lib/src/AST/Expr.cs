using System.Collections.Generic;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Representa el nodo base para todas las expresiones en el Árbol de Sintaxis Abstracta (AST).
    /// </summary>
    public abstract class Expr : ASTNode
    {
        /// <summary>
        /// Método para aceptar un visitante que implementa la interfaz IExprVisitor.
        /// Permite la ejecución de la operación definida en el visitante sobre el nodo de expresión.
        /// </summary>
        /// <typeparam name="T">El tipo de resultado que devuelve la operación del visitante.</typeparam>
        /// <param name="visitor">El visitante que implementa IExprVisitor.</param>
        /// <returns>El resultado de aplicar el visitante sobre este nodo.</returns>
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    /// <summary>
    /// Representa una expresión binaria que contiene una operación entre dos expresiones.
    /// </summary>
    public class Binary : Expr
    {
        /// <summary>
        /// Expresión de la parte izquierda de la operación binaria.
        /// </summary>
        public Expr Left { get; }
        /// <summary>
        /// Token que representa el operador de la operación binaria.
        /// </summary>
        public Token Operator { get; }
        /// <summary>
        /// Expresión de la parte derecha de la operación binaria.
        /// </summary>
        public Expr Right { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Binary.
        /// </summary>
        /// <param name="left">Expresión izquierda.</param>
        /// <param name="op">Token del operador.</param>
        /// <param name="right">Expresión derecha.</param>
        public Binary(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    /// <summary>
    /// Representa una agrupación de una expresión, normalmente para establecer precedencia de operaciones.
    /// </summary>
    public class Grouping : Expr
    {
        /// <summary>
        /// La expresión agrupada.
        /// </summary>
        public Expr Expression { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Grouping.
        /// </summary>
        /// <param name="expression">La expresión a agrupar.</param>
        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión literal numérica.
    /// </summary>
    public class Literal : Expr
    {
        /// <summary>
        /// Valor entero de la literal.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Literal.
        /// </summary>
        /// <param name="value">El valor numérico literal.</param>
        public Literal(int value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión literal de cadena.
    /// </summary>
    public class StringLiteral : Expr
    {
        /// <summary>
        /// Valor de la cadena.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase StringLiteral.
        /// </summary>
        /// <param name="value">La cadena literal.</param>
        public StringLiteral(string value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitStringLiteralExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión unaria, como la negación.
    /// </summary>
    public class Unary : Expr
    {
        /// <summary>
        /// Token que representa el operador unario.
        /// </summary>
        public Token Operator { get; }
        /// <summary>
        /// La expresión a la que se aplica el operador unario.
        /// </summary>
        public Expr Right { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Unary.
        /// </summary>
        /// <param name="op">Token del operador unario.</param>
        /// <param name="right">Expresión a evaluar.</param>
        public Unary(Token op, Expr right)
        {
            Operator = op;
            Right = right;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión que consiste en un identificador.
    /// </summary>
    public class Identifier : Expr
    {
        /// <summary>
        /// Token que representa el nombre del identificador.
        /// </summary>
        public Token Name { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Identifier.
        /// </summary>
        /// <param name="name">Token del identificador.</param>
        public Identifier(Token name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIdentifier(this);
        }
    }

    /// <summary>
    /// Representa una expresión de asignación, donde se asigna un valor a una variable.
    /// </summary>
    public class Assign : Expr
    {
        /// <summary>
        /// Token que representa el nombre de la variable.
        /// </summary>
        public Token Name { get; }
        /// <summary>
        /// Expresión cuyo resultado se asigna a la variable.
        /// </summary>
        public Expr Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Assign.
        /// </summary>
        /// <param name="name">Token del nombre de la variable.</param>
        /// <param name="value">Expresión con el nuevo valor.</param>
        public Assign(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAssignExpr(this);
    }

    /// <summary>
    /// Representa una expresión lógica que combina dos expresiones mediante un operador lógico.
    /// </summary>
    public class Logical : Expr
    {
        /// <summary>
        /// Expresión de la parte izquierda.
        /// </summary>
        public Expr Left { get; }
        /// <summary>
        /// Token que representa el operador lógico.
        /// </summary>
        public Token Operator { get; }
        /// <summary>
        /// Expresión de la parte derecha.
        /// </summary>
        public Expr Right { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Logical.
        /// </summary>
        /// <param name="left">Expresión izquierda.</param>
        /// <param name="op">Token del operador lógico.</param>
        /// <param name="right">Expresión derecha.</param>
        public Logical(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLogicalExpr(this);
    }

    /// <summary>
    /// Representa una expresión que obtiene el valor actual de la coordenada X.
    /// </summary>
    public class GetActualXExpr : Expr
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase GetActualXExpr.
        /// </summary>
        public GetActualXExpr() { }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetActualXExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión que obtiene el valor actual de la coordenada Y.
    /// </summary>
    public class GetActualYExpr : Expr
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase GetActualYExpr.
        /// </summary>
        public GetActualYExpr() { }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetActualYExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión que obtiene el tamaño del canvas.
    /// </summary>
    public class GetCanvasSizeExpr : Expr
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase GetCanvasSizeExpr.
        /// </summary>
        public GetCanvasSizeExpr() { }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetCanvasSizeExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión para obtener el conteo de colores en un área.
    /// </summary>
    public class GetColorCountExpr : Expr
    {
        /// <summary>
        /// Token del comando asociado a la expresión.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que representa el color.
        /// </summary>
        public Expr Color { get; }
        /// <summary>
        /// Expresión que representa la coordenada X1.
        /// </summary>
        public Expr X1 { get; }
        /// <summary>
        /// Expresión que representa la coordenada Y1.
        /// </summary>
        public Expr Y1 { get; }
        /// <summary>
        /// Expresión que representa la coordenada X2.
        /// </summary>
        public Expr X2 { get; }
        /// <summary>
        /// Expresión que representa la coordenada Y2.
        /// </summary>
        public Expr Y2 { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase GetColorCountExpr.
        /// </summary>
        /// <param name="keyword">Token del comando.</param>
        /// <param name="color">Expresión del color.</param>
        /// <param name="x1">Expresión de la coordenada X1.</param>
        /// <param name="y1">Expresión de la coordenada Y1.</param>
        /// <param name="x2">Expresión de la coordenada X2.</param>
        /// <param name="y2">Expresión de la coordenada Y2.</param>
        public GetColorCountExpr(Token keyword, Expr color, Expr x1, Expr y1, Expr x2, Expr y2)
        {
            Keyword = keyword;
            Color = color;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGetColorCountExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión que verifica si un color es el color del pincel.
    /// </summary>
    public class IsBrushColorExpr : Expr
    {
        /// <summary>
        /// Token del comando asociado.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que representa el color a comparar.
        /// </summary>
        public Expr Color { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase IsBrushColorExpr.
        /// </summary>
        /// <param name="keyword">Token del comando.</param>
        /// <param name="color">Expresión del color.</param>
        public IsBrushColorExpr(Token keyword, Expr color)
        {
            Keyword = keyword;
            Color = color;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIsBrushColorExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión que verifica si un tamaño corresponde al tamaño del pincel.
    /// </summary>
    public class IsBrushSizeExpr : Expr
    {
        /// <summary>
        /// Token del comando asociado.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que representa el tamaño a comparar.
        /// </summary>
        public Expr Size { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase IsBrushSizeExpr.
        /// </summary>
        /// <param name="keyword">Token del comando.</param>
        /// <param name="size">Expresión del tamaño.</param>
        public IsBrushSizeExpr(Token keyword, Expr size)
        {
            Keyword = keyword;
            Size = size;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIsBrushSizeExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión que verifica si un color es igual al color del canvas en una posición específica.
    /// </summary>
    public class IsCanvasColorExpr : Expr
    {
        /// <summary>
        /// Token del comando asociado.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que representa el color del canvas.
        /// </summary>
        public Expr Color { get; }
        /// <summary>
        /// Expresión que representa el desplazamiento vertical.
        /// </summary>
        public Expr Vertical { get; }
        /// <summary>
        /// Expresión que representa el desplazamiento horizontal.
        /// </summary>
        public Expr Horizontal { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase IsCanvasColorExpr.
        /// </summary>
        /// <param name="keyword">Token del comando.</param>
        /// <param name="color">Expresión del color.</param>
        /// <param name="vertical">Expresión del desplazamiento vertical.</param>
        /// <param name="horizontal">Expresión del desplazamiento horizontal.</param>
        public IsCanvasColorExpr(Token keyword, Expr color, Expr vertical, Expr horizontal)
        {
            Keyword = keyword;
            Color = color;
            Vertical = vertical;
            Horizontal = horizontal;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitIsCanvasColorExpr(this);
        }
    }

    /// <summary>
    /// Representa una expresión vacía, usada cuando no se tiene una expresión concreta.
    /// </summary>
    public class EmptyExpr : Expr
    {
        /// <inheritdoc/>
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitEmptyExpr(this);
        }
    }
}