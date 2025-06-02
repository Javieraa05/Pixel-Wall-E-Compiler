using System.Collections.Generic;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Contiene el resultado completo de ejecutar el compilador sobre un string:
    ///  1) La matriz final de píxeles.
    ///  2) La lista de errores (léxicos, sintácticos y de tiempo de ejecución).
    ///  3) La lista de instrucciones ordenadas para animar en Godot.
    /// </summary>
    public class RunResult
    {
        public Canvas Canvas { get; }
        public List<ErrorInfo> Errors { get; }
        public List<Instruction> Instructions { get; }

        public RunResult(Canvas canvas, List<ErrorInfo> errors, List<Instruction> instructions)
        {
            Canvas = canvas;
            Errors = errors;
            Instructions = instructions;
        }
    }
}
