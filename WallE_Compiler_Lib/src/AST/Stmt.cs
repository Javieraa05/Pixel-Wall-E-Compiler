namespace Wall_E.Compiler
{
    /// <summary>
    /// Clase base abstracta para todos los nodos de sentencias en el Árbol de Sintaxis Abstracta (AST).
    /// Implementa el patrón Visitor para recorrer las sentencias.
    /// </summary>
    public abstract class Stmt : ASTNode
    {
        /// <summary>
        /// Método abstracto para aceptar un visitante que implementa la interfaz IStmtVisitor.
        /// </summary>
        /// <typeparam name="T">El tipo de resultado que retorna el visitante.</typeparam>
        /// <param name="visitor">El visitante que procesa la sentencia.</param>
        /// <returns>El resultado del procesamiento.</returns>
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    /// <summary>
    /// Representa una sentencia de expresión.
    /// </summary>
    public class ExpressionStmt : Stmt
    {
        /// <summary>
        /// Token que representa el tipo de expresión, generalmente una palabra clave.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// La expresión que se evaluará en esta sentencia.
        /// </summary>
        public Expr Expression { get; }

        /// <summary>
        /// Inicializa una nueva instancia de ExpressionStmt.
        /// </summary>
        /// <param name="expression">La expresión asociada a esta sentencia.</param>
        public ExpressionStmt(Token keyword, Expr expression)
        {
            Keyword = keyword;
            Expression = expression;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia Spawn, que inicia algún proceso o crea una entidad en base a dos expresiones.
    /// </summary>
    public class SpawnStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando Spawn.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que define la coordenada X o el primer parámetro de Spawn.
        /// </summary>
        public Expr ExprX;
        /// <summary>
        /// Expresión que define la coordenada Y o el segundo parámetro de Spawn.
        /// </summary>
        public Expr ExprY;

        /// <summary>
        /// Inicializa una nueva instancia de SpawnStmt.
        /// </summary>
        /// <param name="token">Token del comando Spawn.</param>
        /// <param name="exprX">Expresión para el primer parámetro (X).</param>
        /// <param name="exprY">Expresión para el segundo parámetro (Y).</param>
        public SpawnStmt(Token token, Expr exprX, Expr exprY)
        {
            Keyword = token;
            ExprX = exprX;
            ExprY = exprY;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitSpawnStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia ReSpawn, similar a Spawn, pero normalmente para reinicios.
    /// </summary>
    public class ReSpawnStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando ReSpawn.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión para el primer parámetro.
        /// </summary>
        public Expr ExprX;
        /// <summary>
        /// Expresión para el segundo parámetro.
        /// </summary>
        public Expr ExprY;

        /// <summary>
        /// Inicializa una nueva instancia de ReSpawnStmt.
        /// </summary>
        /// <param name="token">Token del comando ReSpawn.</param>
        /// <param name="exprX">Expresión para el primer parámetro.</param>
        /// <param name="exprY">Expresión para el segundo parámetro.</param>
        public ReSpawnStmt(Token token, Expr exprX, Expr exprY)
        {
            Keyword = token;
            ExprX = exprX;
            ExprY = exprY;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitReSpawnStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia Color, que establece un color basado en una expresión.
    /// </summary>
    public class ColorStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando Color.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que define el color.
        /// </summary>
        public Expr Color;

        /// <summary>
        /// Inicializa una nueva instancia de ColorStmt.
        /// </summary>
        /// <param name="keyword">Token del comando Color.</param>
        /// <param name="color">Expresión que define el color.</param>
        public ColorStmt(Token keyword, Expr color)
        {
            Keyword = keyword;
            Color = color;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitColorStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia Size, que establece un tamaño basado en una expresión.
    /// </summary>
    public class SizeStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando Size.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que define el tamaño.
        /// </summary>
        public Expr Size;

        /// <summary>
        /// Inicializa una nueva instancia de SizeStmt.
        /// </summary>
        /// <param name="keyword">Token del comando Size.</param>
        /// <param name="expr">Expresión que define el tamaño.</param>
        public SizeStmt(Token keyword, Expr expr)
        {
            Keyword = keyword;
            Size = expr;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitSizeStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia DrawLine, que dibuja una línea utilizando tres expresiones: dirección en X, dirección en Y y distancia.
    /// </summary>
    public class DrawLineStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando DrawLine.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que define la dirección en X.
        /// </summary>
        public Expr DirX;
        /// <summary>
        /// Expresión que define la dirección en Y.
        /// </summary>
        public Expr DirY;
        /// <summary>
        /// Expresión que define la distancia de la línea.
        /// </summary>
        public Expr Distance;

        /// <summary>
        /// Inicializa una nueva instancia de DrawLineStmt.
        /// </summary>
        /// <param name="keyword">Token del comando DrawLine.</param>
        /// <param name="dirX">Expresión para la dirección en X.</param>
        /// <param name="dirY">Expresión para la dirección en Y.</param>
        /// <param name="distance">Expresión para la distancia.</param>
        public DrawLineStmt(Token keyword, Expr dirX, Expr dirY, Expr distance)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawLineStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia DrawCircle, que dibuja un círculo basado en dirección X, dirección Y y radio.
    /// </summary>
    public class DrawCircleStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando DrawCircle.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que define la dirección en X.
        /// </summary>
        public Expr DirX;
        /// <summary>
        /// Expresión que define la dirección en Y.
        /// </summary>
        public Expr DirY;
        /// <summary>
        /// Expresión que define el radio del círculo.
        /// </summary>
        public Expr Radius;

        /// <summary>
        /// Inicializa una nueva instancia de DrawCircleStmt.
        /// </summary>
        /// <param name="keyword">Token del comando DrawCircle.</param>
        /// <param name="dirX">Expresión para la dirección en X.</param>
        /// <param name="dirY">Expresión para la dirección en Y.</param>
        /// <param name="radius">Expresión para el radio.</param>
        public DrawCircleStmt(Token keyword, Expr dirX, Expr dirY, Expr radius)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Radius = radius;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawCircleStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia DrawRectangle, que dibuja un rectángulo utilizando cinco expresiones:
    /// dirección en X, dirección en Y, distancia (o desplazamiento), ancho y alto.
    /// </summary>
    public class DrawRectangleStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando DrawRectangle.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que define la dirección en X.
        /// </summary>
        public Expr DirX;
        /// <summary>
        /// Expresión que define la dirección en Y.
        /// </summary>
        public Expr DirY;
        /// <summary>
        /// Expresión que define la distancia o desplazamiento.
        /// </summary>
        public Expr Distance;
        /// <summary>
        /// Expresión que define el ancho del rectángulo.
        /// </summary>
        public Expr Width;
        /// <summary>
        /// Expresión que define el alto del rectángulo.
        /// </summary>
        public Expr Height;

        /// <summary>
        /// Inicializa una nueva instancia de DrawRectangleStmt.
        /// </summary>
        /// <param name="keyword">Token del comando DrawRectangle.</param>
        /// <param name="dirX">Expresión para la dirección en X.</param>
        /// <param name="dirY">Expresión para la dirección en Y.</param>
        /// <param name="distance">Expresión para la distancia o desplazamiento.</param>
        /// <param name="width">Expresión para el ancho.</param>
        /// <param name="height">Expresión para el alto.</param>
        public DrawRectangleStmt(Token keyword, Expr dirX, Expr dirY, Expr distance, Expr width, Expr height)
        {
            Keyword = keyword;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitDrawRectangleStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia Fill, que generalmente se utiliza para rellenar una figura o el canvas.
    /// </summary>
    public class FillStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando Fill.
        /// </summary>
        public Token Keyword;

        /// <summary>
        /// Inicializa una nueva instancia de FillStmt.
        /// </summary>
        /// <param name="token">Token del comando Fill.</param>
        public FillStmt(Token token)
        {
            Keyword = token;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFillStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia de etiqueta, que define un identificador para referenciar en otras sentencias.
    /// </summary>
    public class LabelStmt : Stmt
    {
        /// <summary>
        /// Token que representa el nombre de la etiqueta.
        /// </summary>
        public Token Name { get; }

        /// <summary>
        /// Inicializa una nueva instancia de LabelStmt.
        /// </summary>
        /// <param name="name">Token del nombre de la etiqueta.</param>
        public LabelStmt(Token name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitLabelStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia GoTo, utilizada para saltar a una etiqueta determinada bajo cierta condición.
    /// </summary>
    public class GoToStmt : Stmt
    {
        /// <summary>
        /// Token que representa el comando GoTo.
        /// </summary>
        public Token Keyword;
        /// <summary>
        /// Expresión que representa la etiqueta a la que se saltará.
        /// </summary>
        public Expr Label;
        /// <summary>
        /// Expresión que define la condición para el salto.
        /// </summary>
        public Expr Condition;

        /// <summary>
        /// Inicializa una nueva instancia de GoToStmt.
        /// </summary>
        /// <param name="keyword">Token del comando GoTo.</param>
        /// <param name="label">Expresión que identifica la etiqueta destino.</param>
        /// <param name="condition">Expresión que define la condición para el salto.</param>
        public GoToStmt(Token keyword, Expr label, Expr condition)
        {
            Keyword = keyword;
            Label = label;
            Condition = condition;
        }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitGoToStmt(this);
        }
    }

    /// <summary>
    /// Representa una sentencia vacía, que no realiza ninguna acción.
    /// </summary>
    public class EmptyStmt : Stmt
    {
        /// <summary>
        /// Inicializa una nueva instancia de EmptyStmt.
        /// </summary>
        public EmptyStmt() { }

        /// <inheritdoc/>
        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitEmptyStmt(this);
        }
    }
}