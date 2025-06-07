namespace Wall_E.Compiler
{
    public interface IExprVisitor<T>
    {
        // Métodos para nodos de Expr
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
        T VisitIdentifier(Identifier id);
        T VisitAssignExpr(Assign expr);
        T VisitLogicalExpr(Logical expr);
        T VisitEmptyExpr(EmptyExpr expr);
        T VisitStringLiteralExpr(StringLiteral stringLiteral);
        T VisitGetActualXExpr(GetActualXExpr getActualXNode);
        T VisitGetActualYExpr(GetActualYExpr getActualYNode);
        T VisitGetCanvasSizeExpr(GetCanvasSizeExpr getCanvasSizeNode);
        T VisitGetColorCountExpr(GetColorCountExpr getColorCountNode);
        T VisitIsBrushColorExpr(IsBrushColorExpr isBrushColorNode);
        T VisitIsBrushSizeExpr(IsBrushSizeExpr isBrushSizeNode);
        T VisitIsCanvasColorExpr(IsCanvasColorExpr isCanvasColorNode);
    }

    public interface IStmtVisitor<T>
    {
        // Métodos para nodos de Stmt
        T VisitExpressionStmt(ExpressionStmt stmt);
        T VisitSpawnStmt(SpawnStmt spawn);
        T VisitColorStmt(ColorStmt color);
        T VisitSizeStmt(SizeStmt size);
        T VisitDrawLineStmt(DrawLineStmt drawLine);
        T VisitDrawCircleStmt(DrawCircleStmt drawCircleStmt);
        T VisitDrawRectangleStmt(DrawRectangleStmt drawRectangleleStmt);
        T VisitFillStmt(FillStmt fillStmt);
        T VisitGoToStmt(GoToStmt GoToNode);
        T VisitEmptyStmt(EmptyStmt emptyNode);
        T VisitLabelStmt(LabelStmt labelNode);
        T VisitReSpawnStmt(ReSpawnStmt reSpawn);

        // Métodos para nodos de Program
        T VisitProgramNode(ProgramNode program);
    }
}