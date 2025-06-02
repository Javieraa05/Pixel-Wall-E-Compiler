using System.Collections.Generic;

namespace Wall_E.Compiler
{
    public enum InstructionType
    {
        Spawn,
        SetColor,
        SetSize,
        DrawLine,
        DrawCircle,
        DrawRectangle,
        Fill
        // (Puedes extender con otros tipos de instrucción según sea necesario)
    }

    /// <summary>
    /// Representa una “instrucción gráfica” que Godot deberá animar paso a paso.
    /// </summary>
    public class Instruction
    {
        public InstructionType Type { get; }
        public List<object> Parameters { get; }

        public Instruction(InstructionType type, params object[] parameters)
        {
            Type = type;
            Parameters = new List<object>(parameters);
        }
    }
}
