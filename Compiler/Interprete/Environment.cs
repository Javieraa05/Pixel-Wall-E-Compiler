using System.Collections.Generic;

namespace Wall_E.Compiler
{
    public class Environment
    {
        private readonly Dictionary<string, object> values;

        public Environment()
        {
            values = new Dictionary<string, object>()
        {
            {"Black", "Black"},
            {"Red", "Red"},
            {"Blue", "Blue"},
            {"Green", "Green"},
            {"Yellow", "Yellow"},
            {"Orange", "Orange"},
            {"Purple", "Purple"},
            {"White", "White"},
            {"Transparent", "Transparent"},
        };
        }
        public object Get(Token id)
        {
            if (values.TryGetValue(id.Lexeme, out var val)) return val;
            throw new RuntimeError(id.Line, id.Column,
                $"Variable '{id.Lexeme}' no definida.");
        }

        public void Assign(string name, object value)
        {
            if (values.ContainsKey(name))
            {
                values[name] = value;
                return;
            }
            // Si prefieres que asignar a variable no existente cree una nueva:
            values[name] = value;
            // O, si prefieres error:
            // throw new RuntimeError(…, $"Variable '{name}' no definida.");
        }
    }
}