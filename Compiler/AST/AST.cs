using System;
using System.Collections.Generic;
using System.Text;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Clase base abstracta para todos los nodos del Árbol de Sintaxis Abstracta (AST).
    /// </summary>
    public abstract class ASTNode
    {
    }

    /// <summary>
    /// Nodo raíz del AST que contiene la lista de sentencias del programa.
    /// </summary>
    public class ProgramNode : ASTNode
    {
        /// <summary>
        /// Lista de sentencias que conforman el programa.
        /// </summary>
        public List<Stmt> Statements { get; } = new List<Stmt>();
    }

    /// <summary>
    /// Clase para imprimir el Árbol de Sintaxis Abstracta (AST) en forma de árbol legible.
    /// </summary>
    public class AstTreePrinter
    {
        private StringBuilder? _builder;
        private const string IndentStep = "│   ";
        private const string Branch = "├── ";
        private const string LastBranch = "└── ";

        /// <summary>
        /// Imprime el AST a partir de la raíz proporcionada.
        /// </summary>
        /// <param name="root">Nodo raíz del AST.</param>
        /// <returns>Una cadena de texto que representa el AST.</returns>
        public string Print(ASTNode root)
        {
            _builder = new StringBuilder();
            VisitNode(root, "", true);
            return _builder.ToString();
        }

        /// <summary>
        /// Visita recursivamente los nodos del AST y agrega cada nodo al StringBuilder con la indentación adecuada.
        /// </summary>
        /// <param name="node">Nodo actual a visitar.</param>
        /// <param name="indent">Indentación acumulada para el nodo actual.</param>
        /// <param name="isLast">Indica si el nodo es el último hijo en su nivel.</param>
        private void VisitNode(ASTNode node, string indent, bool isLast)
        {
            var connector = isLast ? LastBranch : Branch;
            _builder.AppendLine(indent + connector + GetLabel(node));

            var children = new List<ASTNode>();

            // Se agregan los hijos del nodo actual según su tipo.
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
                case GetColorCountExpr getColorCountStmt:
                    children.Add(getColorCountStmt.Color);
                    children.Add(getColorCountStmt.X1);
                    children.Add(getColorCountStmt.Y1);
                    children.Add(getColorCountStmt.X2);
                    children.Add(getColorCountStmt.Y2);
                    break;
                case IsBrushColorExpr isBrushColorStmt:
                    children.Add(isBrushColorStmt.Color);
                    break;
                case IsBrushSizeExpr isBrushSizeStmt:
                    children.Add(isBrushSizeStmt.Size);
                    break;
                case IsCanvasColorExpr isCanvasColorStmt:
                    children.Add(isCanvasColorStmt.Color);
                    children.Add(isCanvasColorStmt.Vertical);
                    children.Add(isCanvasColorStmt.Horizontal);
                    break;
                case GoToStmt goToStmt:
                    children.Add(goToStmt.Label);
                    children.Add(goToStmt.Condition);
                    break;
                case ReSpawnStmt reSpawnStmt:
                    children.Add(reSpawnStmt.ExprX);
                    children.Add(reSpawnStmt.ExprY);
                    break;
            }

            // Visita recursivamente cada uno de los nodos hijos.
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                var last = i == children.Count - 1;
                var childIndent = indent + (isLast ? "    " : IndentStep);
                VisitNode(child, childIndent, last);
            }
        }

        /// <summary>
        /// Devuelve una etiqueta descriptiva para el nodo del AST.
        /// </summary>
        /// <param name="node">Nodo del AST.</param>
        /// <returns>Una cadena que describe el tipo y algunos atributos del nodo.</returns>
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