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
                { "Red", "Red" },
                { "Green", "Green" },
                { "Blue", "Blue" },
                { "Yellow" , "Yellow"},
                { "Black", "Black" },
                { "White", "White" },
                { "Gray", "Gray" },
                { "Cyan", "Cyan" },
                { "Magenta", "Magenta" },
                { "Orange", "Orange" },
                { "Purple", "Purple" },
                { "Pink", "Pink" },
                { "Brown", "Brown" }
            };
        }
        public object Get(Token id)
        {
            if (values.TryGetValue(id.Lexeme, out var val)) return val;
            throw new RuntimeError(id.Line, id.Column,
                $"Variable '{id.Lexeme}' no definida.");
        }

        public void Assign(Token name, object value)
        {
            values[name.Lexeme] = value;
        }
    }
}