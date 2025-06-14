using System;
using System.Collections.Generic;
using Godot;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Clase Interpreter que implementa los patrones Visitor para evaluar expresiones y sentencias.
    /// Se encarga de interpretar el árbol de sintaxis abstracta (AST), gestionar errores en tiempo de ejecución,
    /// actualizar el entorno y enviar instrucciones gráficas al canvas.
    /// </summary>
    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        /// <summary>
        /// Flag que indica si se produjo algún error en tiempo de ejecución.
        /// </summary>
        public bool hadRuntimeError = false;

        /// <summary>
        /// Entorno utilizado para almacenar variables y sus valores durante la ejecución.
        /// </summary>
        private readonly Environment env = new Environment();

        /// <summary>
        /// Instancia del canvas sobre el que se realizan las operaciones gráficas.
        /// </summary>
        Canvas canvas;

        /// <summary>
        /// Lista para acumular los errores en tiempo de ejecución detectados durante la interpretación.
        /// </summary>
        private readonly List<RuntimeError> runtimeErrors = new List<RuntimeError>();

        /// <summary>
        /// Lista de instrucciones gráficas, en el orden en el que deben enviarse a Godot.
        /// </summary>
        private readonly List<Instruction> instructions = new List<Instruction>();

        /// <summary>
        /// Tabla de etiquetas que mapea nombres de etiquetas a su índice de sentencia en el programa.
        /// </summary>
        private Dictionary<string, int> labelTable = new Dictionary<string, int>();

        /// <summary>
        /// Constructor que inicializa el intérprete con un canvas de un tamaño específico.
        /// </summary>
        /// <param name="sizeCanvas">Tamaño del canvas.</param>
        public Interpreter(int sizeCanvas)
        {
            canvas = new Canvas(sizeCanvas);
        }

        /// <summary>
        /// Constructor que inicializa el intérprete utilizando un canvas proporcionado.
        /// </summary>
        /// <param name="canvas">Instancia de Canvas ya configurada.</param>
        public Interpreter(Canvas canvas)
        {
            this.canvas = canvas;
        }

        /// <summary>
        /// Método principal que inicia la interpretación del programa.
        /// Realiza dos pasadas:
        /// 1) Indexa todas las etiquetas del programa.
        /// 2) Ejecuta las sentencias mediante un bucle basado en índices, permitiendo saltos (GoTo).
        /// </summary>
        /// <param name="programNode">Nodo raíz del programa (AST).</param>
        public void Interpret(ProgramNode programNode)
        {
            hadRuntimeError = false;
            labelTable.Clear();
            
            // 1) Primer pase: indexar todas las etiquetas del programa.
            var stmts = programNode.Statements;
            for (int i = 0; i < stmts.Count; i++)
            {
                if (stmts[i] is LabelStmt label)
                {
                    string name = label.Name.Lexeme;
                    if (labelTable.ContainsKey(name))
                    {
                        // Etiqueta duplicada, se lanza error (podría tratarse también como error en tiempo de ejecución)
                        throw new RuntimeError(label.Name.Line, label.Name.Column,
                            $"Etiqueta '{name}' ya definida en otra línea.");
                    }
                    labelTable[name] = i;
                }
            }

            // 2) Segundo pase: ejecución de sentencias por índice.
            int current = 0;
            int steps = 0;
            const int maxSteps = 10000; // Límite de pasos para prevenir bucles infinitos

            while (current < stmts.Count)
            {
                Stmt stmt = stmts[current];

                try
                {
                    if (++steps > maxSteps)
                        throw new RuntimeError(0, 0, $"Límite de pasos ({maxSteps}) alcanzado. Posible bucle infinito.");

                    // Manejo del GoTo, que permite saltar a una etiqueta si se cumple la condición.
                    if (stmt is GoToStmt goTo)
                    {
                        // Se evalúa la condición del GoTo
                        object condValue = SafeEvaluate(goTo.Condition);
                        // La condición debe ser un booleano; si no, se lanza un error.
                        if (!(condValue is bool))
                        {
                            throw new RuntimeError(
                                goTo.Keyword.Line,
                                 goTo.Keyword.Column,
                                  "La condición del GoTo debe evaluar a un valor booleano."
                                    );
                        }
                        bool truthy = (bool)condValue;

                        if (truthy)
                        {
                            // La etiqueta a saltar debe ser un Identifier
                            if (!(goTo.Label is Identifier id))
                            {
                                throw new RuntimeError(goTo.Keyword.Line, goTo.Keyword.Column,
                                    "La etiqueta debe ser un identificador simple.");
                            }

                            string labelName = id.Name.Lexeme;
                            if (!labelTable.TryGetValue(labelName, out int targetIndex))
                            {
                                throw new RuntimeError(id.Name.Line, id.Name.Column,
                                    $"Etiqueta '{labelName}' no encontrada.");
                            }
                            // Salto: se actualiza el índice actual con el índice destino de la etiqueta.
                            current = targetIndex;
                            continue; // Se continúa sin incrementar current, pues ya se actualizó.
                        }
                        else
                        {
                            current++;
                        }
                    }
                    else if (stmt is LabelStmt)
                    {
                        // Las etiquetas se ignoran en ejecución; simplemente se avanza.
                        current++;
                    }
                    else
                    {
                        // Para el resto de sentencias se invoca el método Accept del visitor.
                        stmt.Accept(this);
                        current++;
                    }
                }
                catch (RuntimeError rte)
                {
                    // Se captura el error en tiempo de ejecución, se registra y se continúa con la siguiente sentencia.
                    hadRuntimeError = true;
                    runtimeErrors.Add(rte);
                    current++;
                }
            }
        }

        /// <summary>
        /// Propiedad para acceder a la lista de errores en tiempo de ejecución.
        /// </summary>
        public List<RuntimeError> RuntimeErrors => runtimeErrors;

        /// <summary>
        /// Propiedad para acceder a la lista de instrucciones que serán enviadas a Godot.
        /// </summary>
        public List<Instruction> Instructions => instructions;

        /// <summary>
        /// Propiedad para acceder al canvas utilizado por el intérprete.
        /// </summary>
        public Canvas Canvas => canvas;

        /// <summary>
        /// Visitor para el nodo ProgramNode.
        /// Recorre cada sentencia del programa y retorna el valor de la última expresión evaluada.
        /// </summary>
        /// <param name="program">Nodo raíz del programa.</param>
        /// <returns>Resultado de la última sentencia ejecutada (o null si no hay).</returns>
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
                    // Capturamos cualquier error en runtime para seguir interpretando
                    runtimeErrors.Add(rte);
                    hadRuntimeError = true;
                }
            }
            return last;
        }

        /// <summary>
        /// Visitor para la sentencia SpawnStmt.
        /// Evalúa las coordenadas, verifica su validez y ejecuta el spawn en el canvas.
        /// También agrega la instrucción correspondiente a la lista de instrucciones.
        /// </summary>
        public object VisitSpawnStmt(SpawnStmt spawnStmt)
        {
            try
            {
                var X = (int)SafeEvaluate(spawnStmt.ExprX);
                var Y = (int)SafeEvaluate(spawnStmt.ExprY);
                ValidateCoords(spawnStmt.Keyword, X, Y);

                canvas.SpawnWallE(X, Y);
                instructions.Add(new Instruction(
                    InstructionType.Spawn,
                    X, Y
                )); 
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(spawnStmt.Keyword.Line, spawnStmt.Keyword.Column, "Las coordenadas deben ser números enteros.");
            }

            return null;
        }

        /// <summary>
        /// Visitor para la sentencia ReSpawnStmt.
        /// Realiza un reinicio o "respawn" en el canvas en las coordenadas evaluadas.
        /// Agrega la instrucción adecuada a la lista.
        /// </summary>
        public object VisitReSpawnStmt(ReSpawnStmt reSpawnStmt)
        {
            try
            {
                var X = (int)SafeEvaluate(reSpawnStmt.ExprX);
                var Y = (int)SafeEvaluate(reSpawnStmt.ExprY);

                ValidateCoords(reSpawnStmt.Keyword, X, Y);

                canvas.SpawnWallE(X, Y);
                instructions.Add(new Instruction(
                    InstructionType.ReSpawn,
                    X, Y
                ));
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(reSpawnStmt.Keyword.Line, reSpawnStmt.Keyword.Column, "Las coordenadas deben ser números enteros.");
            }
            return null;
        }

        /// <summary>
        /// Visitor para la sentencia ColorStmt.
        /// Verifica que el color sea una cadena de texto, lo busca en el entorno y actualiza el color del canvas.
        /// También agrega la instrucción de cambio de color.
        /// </summary>
        public object VisitColorStmt(ColorStmt colorStmt)
        {
            if (!(colorStmt.Color is StringLiteral) && 
                !(colorStmt.Color is Identifier))
            {
                throw new RuntimeError(colorStmt.Keyword.Line, colorStmt.Keyword.Column, "El color debe ser una variable o cadena de texto.");
            }

            var color = (string)SafeEvaluate(colorStmt.Color);

            try
            {
                // Verifica si el color está definido en el entorno.
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

        /// <summary>
        /// Visitor para la sentencia SizeStmt.
        /// Evalúa el tamaño, lo verifica y actualiza el tamaño en el canvas.
        /// Se agrega la instrucción correspondiente.
        /// </summary>
        public object VisitSizeStmt(SizeStmt sizeStmt)
        {
            try
            { 
            var size = (int)SafeEvaluate(sizeStmt.Size);
            if (size < 1 || size > canvas.Size)
                throw new RuntimeError(sizeStmt.Keyword.Line, sizeStmt.Keyword.Column, "Tamanno de brocha invalido");

            canvas.SetSize(size);
            instructions.Add(new Instruction(
                InstructionType.SetSize,
                size
            ));
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(sizeStmt.Keyword.Line, sizeStmt.Keyword.Column, "El tamaño debe ser un número entero.");
            }
            return null;
        }

        /// <summary>
        /// Visitor para la sentencia DrawLineStmt.
        /// Evalúa la dirección y distancia, verifica los parámetros y ordena al canvas dibujar una línea.
        /// Se agrega la instrucción de dibujo de línea.
        /// </summary>
        public object VisitDrawLineStmt(DrawLineStmt drawLineStmt)
        {
            try
            {
                var dirX = (int)SafeEvaluate(drawLineStmt.DirX);
                var dirY = (int)SafeEvaluate(drawLineStmt.DirY);
                var distance = (int)SafeEvaluate(drawLineStmt.Distance);
                CheckValidDirection(drawLineStmt.Keyword, dirX, dirY);
                if (distance < 0)
                    throw new RuntimeError(drawLineStmt.Keyword.Line, drawLineStmt.Keyword.Column, "La distancia debe ser mayor que 0");
                CheckValidMove(drawLineStmt.Keyword, dirX, dirY, distance);

                GD.Print($"Estoy aumentando {dirX} en X y {dirY} en Y");
                canvas.DrawLine(dirX, dirY, distance);
                instructions.Add(new Instruction(
                    InstructionType.DrawLine,
                    dirX, dirY, distance
                ));
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(drawLineStmt.Keyword.Line, drawLineStmt.Keyword.Column, "Las coordenadas y distancia deben ser números enteros.");
            }
            return null;
        }

        /// <summary>
        /// Visitor para la sentencia DrawCircleStmt.
        /// Evalúa la dirección y el radio; verifica la validez y solicita al canvas dibujar un círculo.
        /// Se agrega la instrucción correspondiente.
        /// </summary>
        public object VisitDrawCircleStmt(DrawCircleStmt drawCircleStmt)
        {
            try
            {
                var dirX = (int)SafeEvaluate(drawCircleStmt.DirX);
                var dirY = (int)SafeEvaluate(drawCircleStmt.DirY);
                var radius = (int)SafeEvaluate(drawCircleStmt.Radius);
                CheckValidDirection(drawCircleStmt.Keyword, dirX, dirY);

                if (radius < 1)
                    throw new RuntimeError(drawCircleStmt.Keyword.Line, drawCircleStmt.Keyword.Column, "El radio debe ser mayor que 1");
                CheckValidMove(drawCircleStmt.Keyword, dirX, dirY, radius);

                canvas.DrawCircle(dirX, dirY, radius);
                instructions.Add(new Instruction(
                    InstructionType.DrawCircle,
                    dirX, dirY, radius
                ));
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(drawCircleStmt.Keyword.Line, drawCircleStmt.Keyword.Column, "Las coordenadas y radio deben ser números enteros.");
            }
            return null;
        }

        /// <summary>
        /// Visitor para la sentencia DrawRectangleStmt.
        /// Evalúa y verifica dirección, desplazamiento, ancho y alto; ordena al canvas dibujar un rectángulo.
        /// Se agrega la instrucción de dibujo de rectángulo.
        /// </summary>
        public object VisitDrawRectangleStmt(DrawRectangleStmt drawRectangleStmt)
        {
            try
            {
                var dirX = (int)SafeEvaluate(drawRectangleStmt.DirX);
                var dirY = (int)SafeEvaluate(drawRectangleStmt.DirY);
                var distance = (int)SafeEvaluate(drawRectangleStmt.Distance);
                var width = (int)SafeEvaluate(drawRectangleStmt.Width);
                var height = (int)SafeEvaluate(drawRectangleStmt.Height);

                CheckValidDirection(drawRectangleStmt.Keyword, dirX, dirY);
                if (width < 1 || height < 1 || distance < 0)
                    throw new RuntimeError(drawRectangleStmt.Keyword.Line, drawRectangleStmt.Keyword.Column, "Alto, ancho o distancia fuera de rango");

                CheckValidMove(drawRectangleStmt.Keyword, dirX, dirY, distance);

                canvas.DrawRectangle(dirX, dirY, distance, width, height);
                instructions.Add(new Instruction(
                    InstructionType.DrawRectangle,
                    dirX, dirY, width, height
                ));
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(drawRectangleStmt.Keyword.Line, drawRectangleStmt.Keyword.Column, "Las coordenadas, ancho, alto y distancia deben ser números enteros.");
            }
            return null;
        }

        /// <summary>
        /// Visitor para la sentencia FillStmt.
        /// Intenta llenar el canvas; en caso de error, lanza una excepción con el mensaje adecuado.
        /// Se agrega la instrucción de relleno.
        /// </summary>
        public object VisitFillStmt(FillStmt fillStmt)
        {
            try
            {
                canvas.Fill();
                instructions.Add(new Instruction(
                    InstructionType.Fill
                ));
            }
            catch
            {
                throw new RuntimeError(fillStmt.Keyword.Line, fillStmt.Keyword.Column, " Coordenadas invalidas");
            }
            
            return null;
        }

        /// <summary>
        /// Visitor para la expresión GetActualXExpr. Retorna la posición actual en X de Wall-E.
        /// </summary>
        public object VisitGetActualXExpr(GetActualXExpr getActualXNode)
        {
            return canvas.GetWallEPosX();
        }

        /// <summary>
        /// Visitor para la expresión GetActualYExpr. Retorna la posición actual en Y de Wall-E.
        /// </summary>
        public object VisitGetActualYExpr(GetActualYExpr getActualYNode)
        {
            return canvas.GetWallEPosY();
        }

        /// <summary>
        /// Visitor para la expresión GetCanvasSizeExpr. Retorna el tamaño del canvas.
        /// </summary>
        public object VisitGetCanvasSizeExpr(GetCanvasSizeExpr getCanvasSizeNode)
        {
            return canvas.GetPixels().GetLength(0);
        }

        /// <summary>
        /// Visitor para la expresión GetColorCountExpr.
        /// Evalúa las coordenadas de un área y un color, retorna la cantidad de píxeles que coinciden con el color.
        /// </summary>
        public object VisitGetColorCountExpr(GetColorCountExpr getColorCountNode)
        {
            try
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

                if (!(getColorCountNode.Color is StringLiteral) && 
                    !(getColorCountNode.Color is Identifier))
                {
                    throw new RuntimeError(getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column, "El color debe ser una cadena de texto.");
                }

                var color = (string)SafeEvaluate(getColorCountNode.Color);

                try
                {
                    // Verifica si el color existe en el entorno.
                    env.Get(new Token(TokenType.Identifier, color, getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column));
                }
                catch
                {
                    throw new RuntimeError(getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column, $"Color '{color}' no definido.");
                }

                return canvas.GetColorCount(color, x1, y1, x2, y2);
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(getColorCountNode.Keyword.Line, getColorCountNode.Keyword.Column, "Las coordenadas deben ser números enteros.");
            }
        }

        /// <summary>
        /// Visitor para la expresión IsBrushColorExpr.
        /// Retorna verdadero si el color del pincel coincide con el color especificado.
        /// </summary>
        public object VisitIsBrushColorExpr(IsBrushColorExpr isBrushColorNode)
        {
            if (!(isBrushColorNode.Color is StringLiteral) && 
                !(isBrushColorNode.Color is Identifier))
            {
                throw new RuntimeError(isBrushColorNode.Keyword.Line, isBrushColorNode.Keyword.Column, "El color debe ser una cadena de texto.");
            }
            var color = (string)SafeEvaluate(isBrushColorNode.Color);

            try
            {
                env.Get(new Token(TokenType.Identifier, color, isBrushColorNode.Keyword.Line, isBrushColorNode.Keyword.Column));
            }
            catch
            {
                throw new RuntimeError(isBrushColorNode.Keyword.Line, isBrushColorNode.Keyword.Column, $"Color '{color}' no definido.");
            }

            return (canvas.GetBrushColor() == color) ? 1 : 0;
            
            
               
        }

        /// <summary>
        /// Visitor para la expresión IsBrushSizeExpr.
        /// Retorna verdadero si el tamaño del pincel coincide con el tamaño especificado.
        /// </summary>
        public object VisitIsBrushSizeExpr(IsBrushSizeExpr isBrushSizeNode)
        {
            try
            {
                return (canvas.GetBrushSize() == (int)SafeEvaluate(isBrushSizeNode.Size)) ? 1 : 0;
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(isBrushSizeNode.Keyword.Line, isBrushSizeNode.Keyword.Column, "El tamaño debe ser un número entero.");
            }
        }

        /// <summary>
        /// Visitor para la expresión IsCanvasColorExpr.
        /// Evalúa las coordenadas y retorna verdadero si el color del canvas en esa posición coincide con el especificado.
        /// </summary>
        public object VisitIsCanvasColorExpr(IsCanvasColorExpr isCanvasColorNode)
        {
            try
            {
                var v = (int)SafeEvaluate(isCanvasColorNode.Vertical) + canvas.GetWallEPosY();
                var h = (int)SafeEvaluate(isCanvasColorNode.Horizontal) + canvas.GetWallEPosX();

                try
                {
                    ValidateCoords(isCanvasColorNode.Keyword, h, v);
                }
                catch
                {
                    return 0;
                }

                if (!(isCanvasColorNode.Color is StringLiteral) && 
                    !(isCanvasColorNode.Color is Identifier))
                {
                    throw new RuntimeError(isCanvasColorNode.Keyword.Line, isCanvasColorNode.Keyword.Column, "El color debe ser una cadena de texto.");
                }

                var color = (string)SafeEvaluate(isCanvasColorNode.Color);
                try
                {
                    // Verifica si el color está definido en el entorno.
                    env.Get(new Token(TokenType.Identifier, color, isCanvasColorNode.Keyword.Line, isCanvasColorNode.Keyword.Column));
                }
                catch
                {
                    throw new RuntimeError(isCanvasColorNode.Keyword.Line, isCanvasColorNode.Keyword.Column, $"Color '{color}' no definido.");
                }


                return (canvas.GetPixelColor(h, v) == color) ? 1 : 0;
            }
            catch (InvalidCastException)
            {
                throw new RuntimeError(isCanvasColorNode.Keyword.Line, isCanvasColorNode.Keyword.Column, "Las coordenadas deben ser números enteros.");
            }
        }

        /// <summary>
        /// Visitor para la sentencia GoToStmt.
        /// Retorna una cadena vacía ya que la lógica de salto se gestiona en el método Interpret.
        /// </summary>
        public object VisitGoToStmt(GoToStmt GoToNode) => string.Empty;

        /// <summary>
        /// Visitor para la sentencia LabelStmt.
        /// Las etiquetas no realizan acción en tiempo de ejecución, por ello se retorna cadena vacía.
        /// </summary>
        public object VisitLabelStmt(LabelStmt labelStmt) => string.Empty;

        /// <summary>
        /// Visitor para la sentencia ExpressionStmt.
        /// Evalúa la expresión contenida y retorna su valor.
        /// </summary>
        public object VisitExpressionStmt(ExpressionStmt stmt)
        {
            if (stmt.Expression is Binary)
            {
                throw new RuntimeError(stmt.Keyword.Line, stmt.Keyword.Column,
                    "Sentencia de expresión no válida.");
            }
            

            object value = SafeEvaluate(stmt.Expression);
            return value;
        }

        /// <summary>
        /// Visitor para la expresión Assign.
        /// Evalúa la parte derecha de la asignación, actualiza el entorno y retorna el valor asignado.
        /// </summary>
        public object VisitAssignExpr(Assign expr)
        {
            object value = SafeEvaluate(expr.Value);
            env.Assign(expr.Name, value);
            return value;
        }

        /// <summary>
        /// Visitor para la expresión Logical.
        /// Implementa la evaluación de corto circuito para operadores lógicos AND y OR.
        /// </summary>
        public object VisitLogicalExpr(Logical expr)
        {
            object left = SafeEvaluate(expr.Left);

            if (expr.Operator.Type == TokenType.Or)
            {
                // Si con OR el valor izquierdo ya es verdadero, se retorna sin evaluar la derecha.
                if (IsTruthy(left))
                    return left;
            }
            else
            {
                // Si con AND el valor izquierdo es falso, se retorna sin evaluar la derecha.
                if (!IsTruthy(left))
                    return left;
            }

            return SafeEvaluate(expr.Right);
        }

        /// <summary>
        /// Visitor para la expresión Literal.
        /// Retorna el valor literal (número entero).
        /// </summary>
        public object VisitLiteralExpr(Literal literal)
        {
            return literal.Value;
        }

        /// <summary>
        /// Visitor para la expresión StringLiteral.
        /// Retorna la cadena de texto literal.
        /// </summary>
        public object VisitStringLiteralExpr(StringLiteral stringLiteral)
        {
            return stringLiteral.Value;
        }

        /// <summary>
        /// Visitor para la expresión Grouping.
        /// Evalúa la expresión agrupada y retorna su resultado.
        /// </summary>
        public object VisitGroupingExpr(Grouping grouping)
        {
            return SafeEvaluate(grouping.Expression);
        }

        /// <summary>
        /// Visitor para la expresión Unary.
        /// Evalúa el operador unario y retorna el resultado (por ejemplo, cambio de signo).
        /// </summary>
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
                    throw new RuntimeError(expr.Operator.Line, expr.Operator.Column,
                        $"Operador no soportado '{expr.Operator.Lexeme}'.");
            }
        }

        /// <summary>
        /// Visitor para la expresión Binary.
        /// Evalúa la operación binaria entre dos operandos y retorna el resultado.
        /// </summary>
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
                    throw new RuntimeError(expr.Operator.Line, expr.Operator.Column,
                        $"Operador no soportado '{expr.Operator.Lexeme}'.");
            }
        }

        /// <summary>
        /// Visitor para la sentencia EmptyStmt.
        /// Retorna null puesto que no realiza ninguna acción.
        /// </summary>
        public object VisitEmptyStmt(EmptyStmt emptyNode)
        {
            return null;
        }

        /// <summary>
        /// Visitor para la expresión Identifier.
        /// Recupera el valor asociado a la variable desde el entorno.
        /// </summary>
        public object VisitIdentifier(Identifier id)
        {
            return env.Get(id.Name);
        }

        /// <summary>
        /// Visitor para la expresión EmptyExpr.
        /// Retorna null ya que representa una expresión vacía.
        /// </summary>
        public object VisitEmptyExpr(EmptyExpr expr)
        {
            return null;
        }

        /// <summary>
        /// Evalúa una expresión de forma segura, capturando excepciones de tipo InvalidCastException y RuntimeError.
        /// Si ocurre un error de conversión, se registra y relanza como RuntimeError.
        /// </summary>
        /// <param name="expr">Expresión a evaluar.</param>
        /// <returns>El resultado de la evaluación de la expresión.</returns>
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
                throw rte;
            }
            catch (RuntimeError)
            {
                hadRuntimeError = true;
                throw;
            }
        }

        /// <summary>
        /// Extrae un token representativo de una expresión para propósitos de notificación de errores.
        /// Se determina a partir de la expresión (por ejemplo, en Binary, Unary o Grouping).
        /// </summary>
        /// <param name="expr">Expresión de la que se extrae el token.</param>
        /// <returns>El token extraído o un token vacío si no se encuentra.</returns>
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

        /// <summary>
        /// Verifica que el operando sea un número; si no, lanza un RuntimeError.
        /// </summary>
        private void CheckNumberOperand(Token operatorToken, object operand)
        {
            if (!(operand is int))
                throw new RuntimeError(operatorToken.Line, operatorToken.Column, "El operando debe ser un número.");
        }

        /// <summary>
        /// Verifica que ambos operandos sean números; si no, lanza un RuntimeError.
        /// </summary>
        private void CheckNumberOperands(Token operatorToken, object left, object right)
        {
            if (!(left is int) || !(right is int))
                throw new RuntimeError(operatorToken.Line, operatorToken.Column, "Ambos operandos deben ser números.");
        }

        /// <summary>
        /// Determina si un valor es considerado "verdadero".
        /// Si la expresión es null, se considera falsa; si es booleano, se evalúa su valor.
        /// Otros tipos se consideran verdaderos.
        /// </summary>
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool b) return b;
            return true;
        }

        /// <summary>
        /// Verifica que la dirección especificada sea válida.
        /// Solo se permiten valores -1, 0 o 1 para cada componente.
        /// </summary>
        private void CheckValidDirection(Token operatorToken, int X, int Y)
        {
            if (!(X == 1 || X == -1 || X == 0) || !(Y == 1 || Y == -1 || Y == 0))
            {
                throw new RuntimeError(operatorToken.Line, operatorToken.Column, "Direccion invalida");
            }
        }

        /// <summary>
        /// Verifica que el movimiento resultante de aplicar la dirección y la distancia esté dentro de los límites del canvas.
        /// Se calcula la posición teórica y se valida con ValidateCoords.
        /// </summary>
        private void CheckValidMove(Token operatorToken, int dirX, int dirY, int distance)
        {
            int temX = canvas.GetWallEPosX() + dirX * distance;
            int temY = canvas.GetWallEPosY() + dirY * distance;
            ValidateCoords(operatorToken, temX, temY);
        }

        /// <summary>
        /// Limpia la lista de errores de runtime y resetea la bandera de error.
        /// </summary>
        public void ClearErrors()
        {
            runtimeErrors.Clear();
            hadRuntimeError = false;
        }

        /// <summary>
        /// Limpia la lista de instrucciones pendientes.
        /// </summary>
        public void ClearInstructions()
        {
            instructions.Clear();
        }

        /// <summary>
        /// Verifica que unas coordenadas se encuentren dentro de los límites del canvas.
        /// Si las coordenadas están fuera del rango, lanza un RuntimeError.
        /// </summary>
        /// <param name="Keyword">Token para referencia de error.</param>
        /// <param name="X">Coordenada X.</param>
        /// <param name="Y">Coordenada Y.</param>
        public void ValidateCoords(Token Keyword, int X, int Y)
        {
            if (X < 0 || X >= canvas.Size || Y < 0 || Y >= canvas.Size)
                throw new RuntimeError(Keyword.Line, Keyword.Column, $"Coordenadas fuera de rango: ({X}, {Y})");
        }
    }

    /// <summary>
    /// Excepción que representa un error en tiempo de ejecución.
    /// Incluye detalles de la línea y columna donde se produjo el error.
    /// </summary>
    public class RuntimeError : Exception
    {
        /// <summary>
        /// Línea donde ocurrió el error.
        /// </summary>
        public int Line { get; }
        /// <summary>
        /// Columna en la que ocurrió el error.
        /// </summary>
        public int Column { get; }
        
        /// <summary>
        /// Inicializa una nueva instancia de RuntimeError con la línea, columna y mensaje especificados.
        /// </summary>
        /// <param name="line">Línea del error.</param>
        /// <param name="column">Columna del error.</param>
        /// <param name="message">Descripción del error.</param>
        public RuntimeError(int line, int column, string message)
            : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}