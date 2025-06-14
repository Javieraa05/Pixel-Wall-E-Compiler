using System.Collections.Generic;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Contiene el resultado completo de ejecutar el compilador sobre un string.
    /// Incluye:
    ///  1) La matriz final de píxeles (Canvas).
    ///  2) La lista de errores (léxicos, sintácticos y de tiempo de ejecución).
    ///  3) La lista de instrucciones ordenadas para animar en Godot.
    ///  4) Una representación textual del árbol de sintaxis abstracta (AST).
    /// </summary>
    public class RunResult
    {
        /// <summary>
        /// Canvas final tras la ejecución del código.
        /// </summary>
        public Canvas Canvas { get; }

        /// <summary>
        /// Lista de errores detectados durante la ejecución (léxicos, sintácticos o de runtime).
        /// </summary>
        public List<ErrorInfo> Errors { get; }

        /// <summary>
        /// Lista de instrucciones generadas para animación/visualización en Godot.
        /// </summary>
        public List<Instruction> Instructions { get; }

        /// <summary>
        /// Representación en texto del árbol de sintaxis abstracta (AST).
        /// </summary>
        public string AST { get; }

        /// <summary>
        /// Inicializa un nuevo resultado de ejecución.
        /// </summary>
        /// <param name="canvas">Canvas resultante.</param>
        /// <param name="errors">Lista de errores encontrados.</param>
        /// <param name="instructions">Lista de instrucciones para animar.</param>
        /// <param name="ast">Texto del AST generado.</param>
        public RunResult(Canvas canvas, List<ErrorInfo> errors, List<Instruction> instructions, string ast)
        {
            Canvas = canvas;
            Errors = errors;
            Instructions = instructions;
            AST = ast;
        }
    }
}