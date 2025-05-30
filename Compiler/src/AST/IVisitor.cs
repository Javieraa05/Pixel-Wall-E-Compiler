public interface IVisitor<T>
{
    // Métodos para nodos de Expr
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);
    T VisitIdentifier(Identifier id);
    T VisitAssignExpr(Assign expr);
    T VisitLogicalExpr(Logical expr);

    // Métodos para nodos de Stmt
    T VisitExpressionStmt(ExpressionStmt stmt);
    T VisitSpawnStmt(SpawnStmt spawn);
    T VisitColorStmt(ColorStmt color);
    T VisitSizeStmt(SizeStmt size);
    T VisitDrawLineStmt(DrawLineStmt drawLine);
    T VisitDrawCircleStmt(DrawCircleStmt drawCircleStmt);
    T VisitDrawRectangleStmt(DrawRectangleStmt drawRectangleleStmt);
    T VisitFillStmt(FillStmt fillStmt);
    T VisitGetActualXStmt(GetActualXStmt getActualXNode);
    T VisitGetActualYStmt(GetActualYStmt getActualYNode);
    T VisitGetCanvasSizeStmt(GetCanvasSizeStmt getCanvasSizeNode);
    T VisitGetColorCountStmt(GetColorCountStmt getColorCountNode);
    T VisitIsBrushColorStmt(IsBrushColorStmt isBrushColorNode);
    T VisitIsBrushSizeStmt(IsBrushSizeStmt isBrushSizeNode);
    T VisitIsCanvasColorStmt(IsCanvasColorStmt isCanvasColorNode);
    T VisitGoToStmt(GoToStmt GoToNode);
    


    // Métodos para nodos de Program
    T VisitProgramNode(ProgramNode program);
}   