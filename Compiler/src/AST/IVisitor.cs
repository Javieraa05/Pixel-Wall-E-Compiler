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

    // Métodos para nodos de Program
    T VisitProgramNode(ProgramNode program);
}   