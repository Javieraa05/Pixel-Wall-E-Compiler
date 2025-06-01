using System;
using System.Collections.Generic;
using System.Text;

namespace Wall_E.Compiler
{
    // Definición de nodos del Árbol de Sintaxis Abstracta (AST)
    public abstract class ASTNode
    {

    }

    // Nodo raíz que contiene una lista de sentencias
    public class ProgramNode : ASTNode
    {
        public List<Stmt> Statements { get; } = new List<Stmt>();
        public void Execute(Interpreter interpreter)
        {
            foreach (var stmt in Statements)
            {
                stmt.Accept(interpreter);
            }
        }
    }

    public class AstTreePrinter
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
                    children.Add(sizeStmt.Size);
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
                case GetColorCountStmt getColorCountStmt:
                    children.Add(getColorCountStmt.Color);
                    children.Add(getColorCountStmt.X1);
                    children.Add(getColorCountStmt.Y1);
                    children.Add(getColorCountStmt.X2);
                    children.Add(getColorCountStmt.Y2);
                    break;
                case IsBrushColorStmt isBrushColorStmt:
                    children.Add(isBrushColorStmt.Color);
                    break;
                case IsBrushSizeStmt isBrushSizeStmt:
                    children.Add(isBrushSizeStmt.Size);
                    break;
                case IsCanvasColorStmt isCanvasColorStmt:
                    children.Add(isCanvasColorStmt.Color);
                    children.Add(isCanvasColorStmt.Vertical);
                    children.Add(isCanvasColorStmt.Horizontal);
                    break;
                case GoToStmt goToStmt:
                    children.Add(goToStmt.Label);
                    children.Add(goToStmt.Condition);
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
                Literal l => $"Literal({l.Value})",
                Unary u => $"Unary({u.Operator.Lexeme})",
                Logical lo => $"Logical({lo.Operator.Lexeme})",
                Assign a => $"Assign({a.Name.Lexeme})",
                Identifier id => $"Identifier({id.Name.Lexeme})",
                _ => node.GetType().Name
            };
        }

    }
}






