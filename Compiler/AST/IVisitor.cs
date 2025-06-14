namespace Wall_E.Compiler
{
    /// <summary>
    /// Interfaz para implementar el patrón Visitor para nodos de expresión (Expr).
    /// Proporciona métodos para visitar cada tipo concreto de nodo de expresión.
    /// </summary>
    /// <typeparam name="T">El tipo de valor que retorna el visitante.</typeparam>
    public interface IExprVisitor<T>
    {
        /// <summary>
        /// Visita un nodo de expresión binaria.
        /// </summary>
        /// <param name="expr">La expresión binaria a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitBinaryExpr(Binary expr);

        /// <summary>
        /// Visita un nodo de agrupación de expresión.
        /// </summary>
        /// <param name="expr">El nodo de agrupación a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitGroupingExpr(Grouping expr);

        /// <summary>
        /// Visita un nodo de literal numérico.
        /// </summary>
        /// <param name="expr">El nodo literal a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitLiteralExpr(Literal expr);

        /// <summary>
        /// Visita un nodo de expresión unaria.
        /// </summary>
        /// <param name="expr">El nodo unario a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitUnaryExpr(Unary expr);

        /// <summary>
        /// Visita un nodo de identificador.
        /// </summary>
        /// <param name="id">El nodo identificador a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitIdentifier(Identifier id);

        /// <summary>
        /// Visita un nodo de asignación.
        /// </summary>
        /// <param name="expr">El nodo de asignación a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitAssignExpr(Assign expr);

        /// <summary>
        /// Visita un nodo de expresión lógica.
        /// </summary>
        /// <param name="expr">El nodo lógico a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitLogicalExpr(Logical expr);

        /// <summary>
        /// Visita un nodo de expresión vacía.
        /// </summary>
        /// <param name="expr">El nodo de expresión vacía a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitEmptyExpr(EmptyExpr expr);

        /// <summary>
        /// Visita un nodo de literal de cadena.
        /// </summary>
        /// <param name="stringLiteral">El nodo de cadena a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitStringLiteralExpr(StringLiteral stringLiteral);

        /// <summary>
        /// Visita un nodo para obtener la coordenada X actual.
        /// </summary>
        /// <param name="getActualXNode">El nodo GetActualX a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitGetActualXExpr(GetActualXExpr getActualXNode);

        /// <summary>
        /// Visita un nodo para obtener la coordenada Y actual.
        /// </summary>
        /// <param name="getActualYNode">El nodo GetActualY a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitGetActualYExpr(GetActualYExpr getActualYNode);

        /// <summary>
        /// Visita un nodo para obtener el tamaño del canvas.
        /// </summary>
        /// <param name="getCanvasSizeNode">El nodo GetCanvasSize a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitGetCanvasSizeExpr(GetCanvasSizeExpr getCanvasSizeNode);

        /// <summary>
        /// Visita un nodo para obtener el conteo de colores en un área específica.
        /// </summary>
        /// <param name="getColorCountNode">El nodo GetColorCount a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitGetColorCountExpr(GetColorCountExpr getColorCountNode);

        /// <summary>
        /// Visita un nodo que verifica si un color corresponde al color del pincel.
        /// </summary>
        /// <param name="isBrushColorNode">El nodo IsBrushColor a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitIsBrushColorExpr(IsBrushColorExpr isBrushColorNode);

        /// <summary>
        /// Visita un nodo que verifica si el tamaño corresponde al tamaño del pincel.
        /// </summary>
        /// <param name="isBrushSizeNode">El nodo IsBrushSize a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitIsBrushSizeExpr(IsBrushSizeExpr isBrushSizeNode);

        /// <summary>
        /// Visita un nodo que verifica si un color es igual al color del canvas en una posición determinada.
        /// </summary>
        /// <param name="isCanvasColorNode">El nodo IsCanvasColor a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitIsCanvasColorExpr(IsCanvasColorExpr isCanvasColorNode);
    }

    /// <summary>
    /// Interfaz para implementar el patrón Visitor para nodos de sentencias (Stmt).
    /// Proporciona métodos para visitar cada tipo concreto de sentencia y el nodo del programa.
    /// </summary>
    /// <typeparam name="T">El tipo de valor que retorna el visitante.</typeparam>
    public interface IStmtVisitor<T>
    {
        /// <summary>
        /// Visita un nodo de sentencia de expresión.
        /// </summary>
        /// <param name="stmt">La sentencia de expresión a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitExpressionStmt(ExpressionStmt stmt);

        /// <summary>
        /// Visita un nodo de sentencia de Spawn.
        /// </summary>
        /// <param name="spawn">El nodo SpawnStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitSpawnStmt(SpawnStmt spawn);

        /// <summary>
        /// Visita un nodo de sentencia de Color.
        /// </summary>
        /// <param name="color">El nodo ColorStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitColorStmt(ColorStmt color);

        /// <summary>
        /// Visita un nodo de sentencia de Size.
        /// </summary>
        /// <param name="size">El nodo SizeStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitSizeStmt(SizeStmt size);

        /// <summary>
        /// Visita un nodo de sentencia de DrawLine.
        /// </summary>
        /// <param name="drawLine">El nodo DrawLineStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitDrawLineStmt(DrawLineStmt drawLine);

        /// <summary>
        /// Visita un nodo de sentencia de DrawCircle.
        /// </summary>
        /// <param name="drawCircleStmt">El nodo DrawCircleStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitDrawCircleStmt(DrawCircleStmt drawCircleStmt);

        /// <summary>
        /// Visita un nodo de sentencia de DrawRectangle.
        /// </summary>
        /// <param name="drawRectangleleStmt">El nodo DrawRectangleStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitDrawRectangleStmt(DrawRectangleStmt drawRectangleleStmt);

        /// <summary>
        /// Visita un nodo de sentencia de Fill.
        /// </summary>
        /// <param name="fillStmt">El nodo FillStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitFillStmt(FillStmt fillStmt);

        /// <summary>
        /// Visita un nodo de sentencia de GoTo.
        /// </summary>
        /// <param name="goToNode">El nodo GoToStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitGoToStmt(GoToStmt goToNode);

        /// <summary>
        /// Visita un nodo de sentencia vacía.
        /// </summary>
        /// <param name="emptyNode">El nodo EmptyStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitEmptyStmt(EmptyStmt emptyNode);

        /// <summary>
        /// Visita un nodo de sentencia de etiqueta.
        /// </summary>
        /// <param name="labelNode">El nodo LabelStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitLabelStmt(LabelStmt labelNode);

        /// <summary>
        /// Visita un nodo de sentencia de ReSpawn.
        /// </summary>
        /// <param name="reSpawn">El nodo ReSpawnStmt a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitReSpawnStmt(ReSpawnStmt reSpawn);

        /// <summary>
        /// Visita el nodo raíz del programa.
        /// </summary>
        /// <param name="program">El nodo ProgramNode a visitar.</param>
        /// <returns>El resultado de la operación del visitante.</returns>
        T VisitProgramNode(ProgramNode program);
    }
}