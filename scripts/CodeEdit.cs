using Godot;
using System;

public partial class CodeEdit : Godot.CodeEdit
{
    public override void _Ready()
    {
        // Configurar los colores básicos del editor para simular Dark+ de VS Code:
        // Fondo: #1e1e1e, Fuente: #d4d4d4
        AddThemeColorOverride("font_color", Colors.White);

        // Opcionales: colores para caret y selección
        AddThemeColorOverride("caret_color", Colors.White);
        AddThemeColorOverride("selection_color", new Color(0.2647f, 0.529f, 0.882f, 0.5f)); // Azul semitransparente

        // Crear el CodeHighlighter y asignarlo mediante la propiedad SyntaxHighlighter
        CodeHighlighter highlighter = new CodeHighlighter();
        SyntaxHighlighter = highlighter;

        // Configurar los colores de las palabras reservadas (inspirados en VS Code Dark+)
        // Usamos un azul brillante (#569CD6) para los keywords
        highlighter.AddKeywordColor("Spawn", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("Color", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("Size", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("DrawLine", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("DrawCircle", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("DrawRectangle", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("Fill", Color.FromHtml("569CD6"));
        highlighter.AddKeywordColor("Goto", Color.FromHtml("569CD6"));

        // Para las funciones integradas, se usa un tono amarillento claro (#DCDCAA)
        highlighter.AddKeywordColor("GetActualX", Color.FromHtml("DCDCAA"));
        highlighter.AddKeywordColor("GetActualY", Color.FromHtml("DCDCAA"));
        highlighter.AddKeywordColor("GetCanvasSize", Color.FromHtml("DCDCAA"));
        highlighter.AddKeywordColor("GetColorCount", Color.FromHtml("DCDCAA"));
        highlighter.AddKeywordColor("IsBrushColor", Color.FromHtml("DCDCAA"));
        highlighter.AddKeywordColor("IsBrushSize", Color.FromHtml("DCDCAA"));
        highlighter.AddKeywordColor("IsCanvasColor", Color.FromHtml("DCDCAA"));

       
    }
}
