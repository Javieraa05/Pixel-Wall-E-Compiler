using System.Collections.Generic;

namespace Wall_E.Compiler
{
    public enum InstructionType
    {
        Spawn,
        ReSpawn,
        SetColor,
        SetSize,
        DrawLine,
        DrawCircle,
        DrawRectangle,
        Fill
        // (Puedes extender con otros tipos de instrucción según sea necesario)
    }

    
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
