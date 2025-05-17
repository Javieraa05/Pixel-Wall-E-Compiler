public class Environment
{
    private readonly Dictionary<string, object> values = new();

    public object Get(string name)
    {
        if (values.TryGetValue(name, out var val)) return val;
        throw new RuntimeError(new Token(TokenType.Identifier, name, 0, 0),
            $"Variable '{name}' no definida.");
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
