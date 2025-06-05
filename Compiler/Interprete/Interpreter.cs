using System;
using System.Collections.Generic;
using Godot;

namespace Wall_E.Compiler
{

    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        // Flag para código de salida
        public bool hadRuntimeError = false;
        private readonly Environment env = new Environment();
        Canvas canvas;

        // Lista para acumular errores en tiempo de ejecución
        private readonly List<RuntimeError> runtimeErrors = new List<RuntimeError>();

        // Lista de instrucciones gráficas que deben enviarse a Godot (en orden)
        private readonly List<Instruction> instructions = new List<Instruction>();

        public Interpreter(int sizeCanvas)
        {
            canvas = new Canvas(sizeCanvas);
        }
        public Interpreter(Canvas canvas)
        {
            this.canvas = canvas;
        }
        /// <summary>
        /// Punto de entrada del intérprete.
        /// </summary>
        public void Interpret(ProgramNode programNode)
        {
            hadRuntimeError = false;
            try
            {
                foreach (var stmt in programNode.Statements)
                {
                    stmt.Accept(this);
                }
            }
            catch (RuntimeError error)
            {
                GD.Print($"Capturamos {error.Message}");
                hadRuntimeError = true;
                // Si llega aquí, agregamos a la lista (aunque idealmente cada Visit protegerá sus propias llamadas)
                runtimeErrors.Add(error);
            }
        }
        /// <summary>
        /// Acceso desde Core para obtener todos los errores de runtime
        /// </summary>
        public List<RuntimeError> RuntimeErrors => runtimeErrors;

        /// <summary>
        /// Lista de instrucciones a animar en Godot
        /// </summary>
        public List<Instruction> Instructions => instructions;

        /// <summary>
        /// El canvas interno, desde el que luego sacamos la matriz de píxeles
        /// </summary>
        public Canvas Canvas => canvas;

        // Método que inicia la interpretación de un programa completo:
        public object VisitProgramNode(ProgramNode program)
        {
            object last = null;
            foreach (var stmt in program.Statements)
            {
                try
                {
                    last = stmt.Accept(this);
                }
                catch (RuntimeError rte)
                {
                    // Capturamos cualquier RuntimeError para seguir interpretando
                    runtimeErrors.Add(rte);
                    hadRuntimeError = true;
                }
            }
            return last;
        }

        public object VisitSpawnStmt(SpawnStmt spawnStmt)
        {
            var X = (int)SafeEvaluate(spawnStmt.ExprX);
            var Y = (int)SafeEvaluate(spawnStmt.ExprY);

            ValidateCoords(spawnStmt.Keyword, X, Y);


            canvas.SpawnWallE(X, Y);
            // Agregamos instrucción
            instructions.Add(new Instruction(
                InstructionType.Spawn,
                X, Y
            ));

            return null;
        }

        public object VisitColorStmt(ColorStmt colorStmt)
        {
            if (!(colorStmt.Color is StringLiteral))
            {
                throw new RuntimeError(colorStmt.Keyword.Line, colorStmt.Keyword.Column, "El color debe ser una cadena de texto.");
            }

            var color = (string)SafeEvaluate(colorStmt.Color);

            try
            {
                // Verifica si el color está definido en el entorno
                env.Get(new Token(TokenType.Identifier, color, colorStmt.Keyword.Line, colorStmt.Keyword.Column));
            }           
            catch
            {
                throw new RuntimeError(colorStmt.Keyword.Line, colorStmt.Keyword.Column, $"Color '{color}' no definido.");
            }
            canvas.SetColor(color);
            instructions.Add(new Instruction(
                InstructionType.SetColor,
                color
            ));
            
            return null;
        }
        public object VisitSizeStmt(SizeStmt sizeStmt)
        {
            var size = (int)SafeEvaluate(sizeStmt.Size);
            if (size < 1 || size > canvas.Size)
                throw new RuntimeError(sizeStmt.Keyword.Line, sizeStmt.Keyword.Column, "Tamanno de brocha invalido");
           
            canvas.SetSize(size);
            instructions.Add(new Instruction(
                InstructionType.SetSize,
                size
                ));

            return null;
        }
        public object VisitDrawLineStmt(DrawLineStmt drawLineStmt)
        {
            var dirX = (int)SafeEvaluate(drawLineStmt.DirX);
            var dirY = (int)SafeEvaluate(drawLineStmt.DirY);
            var distance = (int)SafeEvaluate(drawLineStmt.Distance);
            CheckValidDirection(drawLineStmt.Keyword, dirX, dirY);
            if (distance < 1) throw new RuntimeError(drawLineStmt.Keyword.Line, drawLineStmt.Keyword.Column, "La distancia debe ser mayor que 1");
           
            canvas.DrawLine(dirX, dirY, distance);
            instructions.Add(new Instruction(
                InstructionType.DrawLine,
                dirX, dirY, distance
            ));
    

            return null;

        }
        public object VisitDrawCircleStmt(DrawCircleStmt drawCircleStmt)
        {
            var dirX = (int)SafeEvaluate(drawCircleStmt.DirX);
            var dirY = (int)SafeEvaluate(drawCircleStmt.DirY);
            var radius = (int)SafeEvaluate(drawCircleStmt.Radius);
            CheckValidDirection(drawCircleStmt.Keyword, dirX, dirY);
            
            if(radius < 1 ) throw new RuntimeError(drawCircleStmt.Keyword.Line, drawCircleStmt.Keyword.Column, "El radio debe ser mayor que 1");
            canvas.DrawCircle(dirX, dirY, radius);
            instructions.Add(new Instruction(
                InstructionType.DrawCircle,
                dirX, dirY, radius
            ));

            return null;
        }
        public object VisitDrawRectangleStmt(DrawRectangleStmt drawRectangleStmt)
        {
            var dirX = (int)SafeEvaluate(drawRectangleStmt.DirX);
            var dirY = (int)SafeEvaluate(drawRectangleStmt.DirY);
            var distance = (int)SafeEvaluate(drawRectangleStmt.Distance);
            var width = (int)SafeEvaluate(drawRectangleStmt.Width);
            var height = (int)SafeEvaluate(drawRectangleStmt.Height);
            

            CheckValidDirection(drawRectangleStmt.Keyword, dirX, dirY);
            if (width < 1 || height < 1 || distance < 0)
                throw new RuntimeError(drawRectangleStmt.Keyword.Line, drawRectangleStmt.Keyword.Column, "Alto, ancho o distancia fuera de rango");
           
            canvas.DrawRectangle(dirX, dirY, distance, width, height);

            instructions.Add(new Instruction(
                InstructionType.DrawRectangle,
                dirX, dirY, width, height
            ));
           

            return null;

        }
        public object VisitFillStmt(FillStmt fillStmt)
        {
            canvas.Fill();
            instructions.Add(new Instruction(
            InstructionType.Fill
            ));
            
            return null;
        }
        public object VisitGetActualXExpr(GetActualXExpr getActualXNode)
        {
            return canvas.GetWallEPosX();
        }
        public object VisitGetActualYExpr(GetActualYExpr getActualYNode)
        {
            return canvas.GetWallEPosY();
        }
        public object VisitGetCanvasSizeExpr(GetCanvasSizeExpr getCanvasSizeNode)
        {
            return canvas.GetPixels().GetLength(0);
        }
        public object VisitGetColorCountExpr(GetColorCountExpr getColorCountNode)
        {
            var x1 = (int)SafeEvaluate(getColorCountNode.X1);
            var y1 = (int)SafeEvaluate(getColorCountNode.Y1);
            var x2 = (int)SafeEvaluate(getColorCountNode.X2);
            var y2 = (int)SafeEvaluate(getColorCountNode.Y2);

            try
            {
                ValidateCoords(getColorCountNode.Keyword, x1, y1);
                ValidateCoords(getColorCountNode.Keyword, x2, y2);
            }
            catch
            {
                return 0;
            }

            if (!(getColorCountNode.Color is StringLiteral))
            {
                GD.Print($"Entro {getColorCountNode.Color}");
                throw new RuntimeError(getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column, "El color debe ser una cadena de texto.");
            }

            var color = (string)SafeEvaluate(getColorCountNode.Color);
            
            try
            {
                // Verifica si el color está definido en el entorno
                env.Get(new Token(TokenType.Identifier, color, getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column));
            }           
            catch
            {
                throw new RuntimeError(getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column, $"Color '{color}' no definido.");
            }

            
            return canvas.GetColorCount(color, x1, y1, x2, y2);
        }
        public object VisitIsBrushColorExpr(IsBrushColorExpr isBrushColorNode)
        {
            return canvas.GetPixelColor(
                canvas.GetWallEPosX(),
                canvas.GetWallEPosY()
            ) == (string)SafeEvaluate(isBrushColorNode.Color);
        }
        public object VisitIsBrushSizeExpr(IsBrushSizeExpr isBrushSizeNode)
        {
            return canvas.GetWallEPosX() == (int)SafeEvaluate(isBrushSizeNode.Size);
        }
        public object VisitIsCanvasColorExpr(IsCanvasColorExpr isCanvasColorNode)
        {
            var color = (string)SafeEvaluate(isCanvasColorNode.Color);
            var v = (int)SafeEvaluate(isCanvasColorNode.Vertical);
            var h = (int)SafeEvaluate(isCanvasColorNode.Horizontal);
            return canvas.GetPixelColor(h, v) == color;
        }
        public object VisitGoToStmt(GoToStmt GoToNode) => string.Empty;

        // Ejecutar una sentencia de expresión:
        public object VisitExpressionStmt(ExpressionStmt stmt)
        {
            object value = SafeEvaluate(stmt.Expression);
            return value;
        }

        public object VisitAssignExpr(Assign expr)
        {
            // Evalúa el lado derecho
            object value = SafeEvaluate(expr.Value);
            // Guarda en el entorno
            env.Assign(expr.Name, value);
            return value;
        }

        public object VisitLogicalExpr(Logical expr)
        {
            object left = SafeEvaluate(expr.Left);

            if (expr.Operator.Type == TokenType.Or)
            {
                // OR: si el izquierdo es "true", retorna left sin evaluar right
                if (IsTruthy(left)) return left;
            }
            else
            {
                // AND: si el izquierdo es "false", retorna left sin evaluar right
                if (!IsTruthy(left)) return left;
            }

            // Si no cortocircuitó, evalúa y retorna right
            return SafeEvaluate(expr.Right);
        }
        public object VisitLiteralExpr(Literal literal)
        {
            return literal.Value;
        }
        public object VisitStringLiteralExpr(StringLiteral stringLiteral)
        {
            return stringLiteral.Value;
        }

        public object VisitGroupingExpr(Grouping grouping)
        {
            return SafeEvaluate(grouping.Expression);
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = SafeEvaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Plus:
                    CheckNumberOperand(expr.Operator, right);
                    return (int)right;

                case TokenType.Minus:
                    CheckNumberOperand(expr.Operator, right);
                    return -(int)right;

                default:
                    throw new RuntimeError(
                        expr.Operator.Line, expr.Operator.Column,
                        $"Operador no soportado '{expr.Operator.Lexeme}'."
                    );
            }
        }
        public object VisitBinaryExpr(Binary expr)
        {
            object left = SafeEvaluate(expr.Left);
            object right = SafeEvaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.Plus:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left + (int)right;

                case TokenType.Minus:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left - (int)right;

                case TokenType.Star:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left * (int)right;

                case TokenType.Power:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)Math.Pow((int)left, (int)right);

                case TokenType.Modulo:
                    CheckNumberOperands(expr.Operator, left, right);
                    if ((int)right == 0)
                        throw new RuntimeError(expr.Operator.Line, expr.Operator.Column, "Módulo por cero.");
                    return (int)left % (int)right;

                case TokenType.Slash:
                    CheckNumberOperands(expr.Operator, left, right);
                    if ((int)right == 0)
                        throw new RuntimeError(expr.Operator.Line, expr.Operator.Column, "División por cero.");
                    return (int)left / (int)right;

                case TokenType.Greater:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left > (int)right;

                case TokenType.Less:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left < (int)right;

                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left >= (int)right;

                case TokenType.LessEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (int)left <= (int)right;

                case TokenType.EqualEqual:
                    return left.Equals(right);

                case TokenType.And:
                    return (bool)left && (bool)right;

                case TokenType.Or:
                    return (bool)left || (bool)right;

                default:
                    throw new RuntimeError(
                        expr.Operator.Line, expr.Operator.Column,
                        $"Operador no soportado '{expr.Operator.Lexeme}'."
                    );
            }

        }
        public object VisitEmptyStmt(EmptyStmt emptyNode)
        {
            // No hace nada, simplemente retorna null
            return null;
        }

        public object VisitIdentifier(Identifier id)
        {
            // Recupera el valor de la variable
            return env.Get(id.Name);
        }
        public object VisitEmptyExpr(EmptyExpr expr)
        {
            // Expresión vacía, no hace nada
            return null;
        }

        /// <summary>
        /// Evalúa el árbol y traduce cualquier InvalidCastException en RuntimeError.
        /// </summary>
        private object SafeEvaluate(Expr expr)
        {
            try
            {
                return expr.Accept(this);
            }
            catch (InvalidCastException)
            {
                Token t = ExtractToken(expr);
                var rte = new RuntimeError(t.Line, t.Column, "Operación con tipos inválidos.");
                runtimeErrors.Add(rte);
                hadRuntimeError = true;
                // Como no podemos devolver un int válido, relanzamos RuntimeError:
                throw rte;
            }
            catch (RuntimeError rte)
            {
                hadRuntimeError = true;
                // Relanzamos para que el VisitXXXStmt lo capture antes de castear:
                throw;
            }
        }

        private Token ExtractToken(Expr expr)
        {
            switch (expr)
            {
                case Binary b: return b.Operator;
                case Unary u: return u.Operator;
                case Grouping g: return ExtractToken(g.Expression);
                case Identifier i: return i.Name;
                default: return new Token(TokenType.None, "", 0, 0);
            }
        }
        private void CheckNumberOperand(Token operatorToken, object operand)
        {
            if (!(operand is int))
                throw new RuntimeError(operatorToken.Line, operatorToken.Column, "El operando debe ser un número.");
        }

        private void CheckNumberOperands(Token operatorToken, object left, object right)
        {
            if (!(left is int) || !(right is int))
                throw new RuntimeError(operatorToken.Line, operatorToken.Column, "Ambos operandos deben ser números.");
        }

        private string Stringify(object obj)
        {
            return obj == null ? "null" : obj.ToString();
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool b) return b;
            // Otros tipos (números, strings, etc.) siempre "verdaderos"
            return true;
        }
        private void CheckValidDirection(Token operatorToken, int X, int Y)
        {
            if (!(X == 1 || X == -1 || X == 0) || !(Y == 1 || Y == -1 || Y == 0))
            {
                throw new RuntimeError(operatorToken.Line, operatorToken.Column, "Direccion invalida");
            }
        }

        public void ClearErrors()
        {
            runtimeErrors.Clear();
            hadRuntimeError = false;
        }

        public void ClearInstructions()
        {
            instructions.Clear();
        }
        public void ValidateCoords(Token Keyword, int X, int Y)
        {
            if (X < 0 || X >= canvas.Size || Y < 0 || Y >= canvas.Size)
                throw new RuntimeError(Keyword.Line, Keyword.Column, $"Coordenadas fuera de rango: ({X}, {Y})");
        }
    }


    public class RuntimeError : Exception
    {
        public int Line { get; }
        public int Column { get; }
        
        public RuntimeError(int line, int column, string message)
            : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}