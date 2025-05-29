using System;
using System.Collections.Generic;
using System.Text;


// Definición de nodos del Árbol de Sintaxis Abstracta (AST)
public abstract class ASTNode
{
    // Método Accept genérico para el patrón Visitor
    public abstract T Accept<T>(IVisitor<T> visitor);
}

// Nodo raíz que contiene una lista de sentencias
public class ProgramNode : ASTNode
{
    public List<Stmt> Statements { get; } = new List<Stmt>();

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitProgramNode(this);
    }
}

public class AstTreePrinter : IVisitor<string>
{
    private StringBuilder? _builder;
    private const string IndentStep = "│   ";
    private const string Branch = "├── ";
    private const string LastBranch = "└── ";

    public string Print(ASTNode root)
    {
        _builder = new StringBuilder();
        VisitNode(root, "", true);
        return _builder.ToString();
    }

    private void VisitNode(ASTNode node, string indent, bool isLast)
    {
        var connector = isLast ? LastBranch : Branch;
        _builder.AppendLine(indent + connector + GetLabel(node));

        var children = new List<ASTNode>();

        switch (node)
        {
            case ProgramNode program:
                children.AddRange(program.Statements);
                break;
            case ExpressionStmt exprStmt:
                children.Add(exprStmt.Expression);
                break;
            case Binary binary:
                children.Add(binary.Left);
                children.Add(binary.Right);
                break;
            case Grouping grouping:
                children.Add(grouping.Expression);
                break;
            case Unary unary:
                children.Add(unary.Right);
                break;
            case Logical logical:
                children.Add(logical.Left);
                children.Add(logical.Right);
                break;
            case Assign assign:
                children.Add(assign.Value);
                break;
            case SpawnStmt spawnStmt:
                children.Add(spawnStmt.ExprX);
                children.Add(spawnStmt.ExprY);
                break;
            case ColorStmt colorStmt:
                children.Add(colorStmt.Color);
                break;
            case SizeStmt sizeStmt:
                children.Add(sizeStmt.Expr);
                break;
            case DrawLineStmt drawLineStmt:
                children.Add(drawLineStmt.DirX);
                children.Add(drawLineStmt.DirY);
                children.Add(drawLineStmt.Distance);
                break;
            case DrawCircleStmt drawLineStmt:
                children.Add(drawLineStmt.DirX);
                children.Add(drawLineStmt.DirY);
                children.Add(drawLineStmt.Radius);
                break;
            case DrawRectangleStmt drawRectangleStmt:
                children.Add(drawRectangleStmt.DirX);
                children.Add(drawRectangleStmt.DirY);
                children.Add(drawRectangleStmt.Width);
                children.Add(drawRectangleStmt.Height);
                break;
        }

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var last = i == children.Count - 1;
            var childIndent = indent + (isLast ? "    " : IndentStep);
            VisitNode(child, childIndent, last);
        }
    }

    private string GetLabel(ASTNode node)
    {
        return node switch
        {
            ProgramNode _ => "Program",
            ExpressionStmt _ => "ExpressionStmt",
            Binary b => $"Binary({b.Operator.Lexeme})",
            Grouping _ => "Grouping",
            Literal l => $"Literal({l.Value ?? "nil"})",
            Unary u => $"Unary({u.Operator.Lexeme})",
            Logical lo => $"Logical({lo.Operator.Lexeme})",
            Assign a => $"Assign({a.Name.Lexeme})",
            Identifier id => $"Identifier({id.Name.Lexeme})",
            _ => node.GetType().Name
        };
    }

    // Implementación vacía del visitor, solo para cumplir la interfaz
    public string VisitProgramNode(ProgramNode program) => Print(program);
    public string VisitExpressionStmt(ExpressionStmt stmt) => string.Empty;
    public string VisitBinaryExpr(Binary expr) => string.Empty;
    public string VisitGroupingExpr(Grouping expr) => string.Empty;
    public string VisitLiteralExpr(Literal expr) => string.Empty;
    public string VisitUnaryExpr(Unary expr) => string.Empty;
    public string VisitIdentifier(Identifier identifier) => string.Empty;
    public string VisitLogicalExpr(Logical expr) => string.Empty;
    public string VisitAssignExpr(Assign expr) => string.Empty;
    public string VisitSpawnStmt(SpawnStmt expr) => string.Empty;
    public string VisitColorStmt(ColorStmt expr) => string.Empty;
    public string VisitSizeStmt(SizeStmt expr) => string.Empty;
    public string VisitDrawLineStmt(DrawLineStmt expr) => string.Empty;
    public string VisitDrawCircleStmt(DrawCircleStmt expr) => string.Empty;
    public string VisitDrawRectangleStmt(DrawRectangleStmt expr) => string.Empty;
    public string VisitFillStmt(FillStmt expr) => string.Empty;
    public string VisitGetActualXStmt(GetActualXStmt getActualXNode) => string.Empty;
    public string VisitGetActualYStmt(GetActualYStmt getActualYNode) => string.Empty;
    public string VisitGetCanvasSizeStmt(GetCanvasSizeStmt getCanvasSizeNode) => string.Empty;
    public string VisitGetColorCountStmt(GetColorCountStmt getColorCountNode) => string.Empty;
    public string VisitIsBrushColorStmt(IsBrushColorStmt isBrushColorNode) => string.Empty;
    public string VisitIsBrushSizeStmt(IsBrushSizeStmt isBrushSizeNode) => string.Empty;
    public string VisitIsCanvasColorStmt(IsCanvasColorStmt isCanvasColorNode) => string.Empty;

}








