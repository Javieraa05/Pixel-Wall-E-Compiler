public interface IVisitor<T>
{
    // Métodos para nodos de expresión
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);
    T VisitIdentifier(Identifier id);
    T VisitAssignExpr(Assign expr);
    T VisitLogicalExpr(Logical expr);

    // Métodos para nodos de AST general
    T VisitProgramNode(ProgramNode program);
    T VisitExpressionStatement(ExpressionStatement stmt);
}   