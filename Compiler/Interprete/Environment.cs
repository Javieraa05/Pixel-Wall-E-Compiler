using System.Collections.Generic;

namespace Wall_E.Compiler
{
    /// <summary>
    /// Representa el entorno de ejecución que almacena variables y sus valores.
    /// Proporciona métodos para obtener y asignar valores a las variables.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// Diccionario que mapea nombres de variables (lexemas) a sus valores correspondientes.
        /// </summary>
        private readonly Dictionary<string, object> values;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Environment"/> con un conjunto de variables predefinidas.
        /// </summary>
        public Environment()
        {
            values = new Dictionary<string, object>()
            {
                { "AntiqueWhite", "AntiqueWhite" },
                { "AliceBlue", "AliceBlue" },
                { "Aqua", "Aqua" },
                { "Aquamarine", "Aquamarine" },
                { "Azure", "Azure" },
                { "Beige", "Beige" },
                { "Bisque", "Bisque" },
                { "Black", "Black" },
                { "BlanchedAlmond", "BlanchedAlmond" },
                { "Blue", "Blue" },
                { "BlueViolet", "BlueViolet" },
                { "Brown", "Brown" },
                { "Burlywood", "Burlywood" },
                { "CadetBlue", "CadetBlue" },
                { "Chartreuse", "Chartreuse" },
                { "Chocolate", "Chocolate" },
                { "Coral", "Coral" },
                { "CornflowerBlue", "CornflowerBlue" },
                { "Cornsilk", "Cornsilk" },
                { "Crimson", "Crimson" },
                { "Cyan", "Cyan" },
                { "DarkBlue", "DarkBlue" },
                { "DarkCyan", "DarkCyan" },
                { "DarkGoldenrod", "DarkGoldenrod" },
                { "DarkGray", "DarkGray" },
                { "DarkGreen", "DarkGreen" },
                { "DarkKhaki", "DarkKhaki" },
                { "DarkMagenta", "DarkMagenta" },
                { "DarkOliveGreen", "DarkOliveGreen" },
                { "DarkOrange", "DarkOrange" },
                { "DarkOrchid", "DarkOrchid" },
                { "DarkRed", "DarkRed" },
                { "DarkSalmon", "DarkSalmon" },
                { "DarkSeaGreen", "DarkSeaGreen" },
                { "DarkSlateBlue", "DarkSlateBlue" },
                { "DarkSlateGray", "DarkSlateGray" },
                { "DarkTurquoise", "DarkTurquoise" },
                { "DarkViolet", "DarkViolet" },
                { "DeepPink", "DeepPink" },
                { "DeepSkyBlue", "DeepSkyBlue" },
                { "DimGray", "DimGray" },
                { "DodgerBlue", "DodgerBlue" },
                { "Firebrick", "Firebrick" },
                { "FloralWhite", "FloralWhite" },
                { "ForestGreen", "ForestGreen" },
                { "Fuchsia", "Fuchsia" },
                { "Gainsboro", "Gainsboro" },
                { "GhostWhite", "GhostWhite" },
                { "Gold", "Gold" },
                { "Goldenrod", "Goldenrod" },
                { "Gray", "Gray" },
                { "Green", "Green" },
                { "GreenYellow", "GreenYellow" },
                { "Honeydew", "Honeydew" },
                { "HotPink", "HotPink" },
                { "IndianRed", "IndianRed" },
                { "Indigo", "Indigo" },
                { "Ivory", "Ivory" },
                { "Khaki", "Khaki" },
                { "Lavender", "Lavender" },
                { "LavenderBlush", "LavenderBlush" },
                { "LawnGreen", "LawnGreen" },
                { "LemonChiffon", "LemonChiffon" },
                { "LightBlue", "LightBlue" },
                { "LightCoral", "LightCoral" },
                { "LightCyan", "LightCyan" },
                { "LightGoldenrod", "LightGoldenrod" },
                { "LightGray", "LightGray" },
                { "LightGreen", "LightGreen" },
                { "LightPink", "LightPink" },
                { "LightSalmon", "LightSalmon" },
                { "LightSeaGreen", "LightSeaGreen" },
                { "LightSkyBlue", "LightSkyBlue" },
                { "LightSlateGray", "LightSlateGray" },
                { "LightSteelBlue", "LightSteelBlue" },
                { "LightYellow", "LightYellow" },
                { "Lime", "Lime" },
                { "LimeGreen", "LimeGreen" },
                { "Linen", "Linen" },
                { "Magenta", "Magenta" },
                { "Maroon", "Maroon" },
                { "MediumAquamarine", "MediumAquamarine" },
                { "MediumBlue", "MediumBlue" },
                { "MediumOrchid", "MediumOrchid" },
                { "MediumPurple", "MediumPurple" },
                { "MediumSeaGreen", "MediumSeaGreen" },
                { "MediumSlateBlue", "MediumSlateBlue" },
                { "MediumSpringGreen", "MediumSpringGreen" },
                { "MediumTurquoise", "MediumTurquoise" },
                { "MediumVioletRed", "MediumVioletRed" },
                { "MidnightBlue", "MidnightBlue" },
                { "MintCream", "MintCream" },
                { "MistyRose", "MistyRose" },
                { "Moccasin", "Moccasin" },
                { "NavajoWhite", "NavajoWhite" },
                { "NavyBlue", "NavyBlue" },
                { "OldLace", "OldLace" },
                { "Olive", "Olive" },
                { "OliveDrab", "OliveDrab" },
                { "Orange", "Orange" },
                { "OrangeRed", "OrangeRed" },
                { "Orchid", "Orchid" },
                { "PaleGoldenrod", "PaleGoldenrod" },
                { "PaleGreen", "PaleGreen" },
                { "PaleTurquoise", "PaleTurquoise" },
                { "PaleVioletRed", "PaleVioletRed" },
                { "PapayaWhip", "PapayaWhip" },
                { "PeachPuff", "PeachPuff" },
                { "Peru", "Peru" },
                { "Pink", "Pink" },
                { "Plum", "Plum" },
                { "PowderBlue", "PowderBlue" },
                { "Purple", "Purple" },
                { "RebeccaPurple", "RebeccaPurple" },
                { "Red", "Red" },
                { "RosyBrown", "RosyBrown" },
                { "RoyalBlue", "RoyalBlue" },
                { "SaddleBrown", "SaddleBrown" },
                { "Salmon", "Salmon" },
                { "SandyBrown", "SandyBrown" },
                { "SeaGreen", "SeaGreen" },
                { "Seashell", "Seashell" },
                { "Sienna", "Sienna" },
                { "Silver", "Silver" },
                { "SkyBlue", "SkyBlue" },
                { "SlateBlue", "SlateBlue" },
                { "Snow", "Snow" },
                { "SlateGray", "SlateGray" },
                { "SteelBlue", "SteelBlue" },
                { "SpringGreen", "SpringGreen" },
                { "Teal", "Teal" },
                { "Tan", "Tan" },
                { "Tomato", "Tomato" },
                { "Thistle", "Thistle" },
                { "Turquoise", "Turquoise" },
                { "Transparent", "Transparent" },
                { "WebGray", "WebGray" },
                { "Violet", "Violet" },
                { "WebMaroon", "WebMaroon" },
                { "WebGreen", "WebGreen" },
                { "Wheat", "Wheat" },
                { "WebPurple", "WebPurple" },
                { "WhiteSmoke", "WhiteSmoke" },
                { "White", "White" },
                { "YellowGreen", "YellowGreen" },
                { "Yellow", "Yellow" },
            };
        }

        /// <summary>
        /// Obtiene el valor de una variable definida en el entorno.
        /// </summary>
        /// <param name="id">El token que representa el identificador de la variable.</param>
        /// <returns>El valor asociado a la variable.</returns>
        /// <exception cref="RuntimeError">
        /// Se lanza si la variable solicitada no está definida en el entorno.
        /// </exception>
        public object Get(Token id)
        {
            if (values.TryGetValue(id.Lexeme, out var val))
                return val;
            throw new RuntimeError(id.Line, id.Column,
                $"Variable '{id.Lexeme}' no definida.");
        }

        /// <summary>
        /// Asigna un valor a una variable en el entorno.
        /// Si la variable no existía, se crea.
        /// </summary>
        /// <param name="name">El token que representa el nombre de la variable.</param>
        /// <param name="value">El valor a asignar a la variable.</param>
        public void Assign(Token name, object value)
        {
            values[name.Lexeme] = value;
        }
    }
}