using Godot;
using System;
using Wall_E.Compiler;
using System.Collections.Generic;
public partial class Main : Control
{
    // Botones
    private Button runButton;
    private Button loadButton;
    private Button saveButton;  
    
    // Nodo del editor de código
    private CodeEdit codeEdit;
    private TextEdit textOut;
    
    // FileDialogs para guardar y cargar archivos
    private FileDialog fileDialogSave;
    private FileDialog fileDialogLoad;

    // Referencias a otros nodos de la interfaz
    private SpinBox boardSizeSpinBox;
    private GridOverlay gridOverlay;
    private TextureRect canvasTextureRect;

    // Tamaño fijo del canvas en píxeles
    private const int BoardPixelSize = 900;

    // Imagen y textura para el canvas
    private Image canvasImage;
    private ImageTexture canvasTexture;

    // Almacena el número actual de divisiones de la cuadrícula
    private int currentGridDivisions = 32;


    public override void _Ready()
    {
        // Obtén las referencias a los nodos hijos
        boardSizeSpinBox = GetNode<SpinBox>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/SpinBox");
        gridOverlay = GetNode<GridOverlay>("HBoxContainer/DisplayContainer/CanvasContainer/GridOverlay");
        canvasTextureRect = GetNode<TextureRect>("HBoxContainer/DisplayContainer/CanvasContainer/TextureRect");
        saveButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Save");
        loadButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Load");
        runButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Run");
        codeEdit = GetNode<CodeEdit>("HBoxContainer/EditContainer/MarginContainer/CodeEdit");
        textOut = GetNode<TextEdit>("HBoxContainer/EditContainer/MarginContainer3/TextEdit");

        // Conectar señales de botones
        boardSizeSpinBox.ValueChanged += OnBoardSizeChanged;
        runButton.Pressed += OnRunButtonPressed;
        saveButton.Pressed += OnSaveButtonPressed;
        loadButton.Pressed += OnLoadButtonPressed;

        // Inicializa el canvas (imagen) y la textura
        InicializarCanvas();
        currentGridDivisions = (int)boardSizeSpinBox.Value;

        // Crear y configurar el FileDialog para guardar archivos
        fileDialogSave = new FileDialog();
        // Usamos Filesystem en lugar de User
        fileDialogSave.Access = FileDialog.AccessEnum.Filesystem;
        // No se asigna la propiedad Mode, ya que produce error en esta versión
        fileDialogSave.Filters = new string[] { "*.gw" };
        AddChild(fileDialogSave);
        fileDialogSave.Connect("file_selected", new Callable(this, nameof(_OnFileDialogSaveFileSelected)));

        // Crear y configurar el FileDialog para cargar archivos
        fileDialogLoad = new FileDialog();
        fileDialogLoad.Access = FileDialog.AccessEnum.Filesystem;
        // No se asigna la propiedad Mode, ya que produce error en esta versión
        fileDialogLoad.Filters = new string[] { "*.gw" };
        AddChild(fileDialogLoad);
        fileDialogLoad.Connect("file_selected", new Callable(this, nameof(_OnFileDialogLoadFileSelected)));

       

    }

    private void OnSaveButtonPressed()
    {
        GD.Print("Guardar Código");
        fileDialogSave.PopupCentered();
        fileDialogSave.Size = new Vector2I(600, 400); // ancho x alto
        fileDialogLoad.Size = new Vector2I(600, 400);

    }

    private void OnLoadButtonPressed()
    {
        GD.Print("Cargar Código");
        fileDialogLoad.PopupCentered();
        fileDialogSave.Size = new Vector2I(600, 400); // ancho x alto
        fileDialogLoad.Size = new Vector2I(600, 400);

    }

    private void OnRunButtonPressed()
    {
        GD.Print("Ejecutar Código");
        string codigo = codeEdit.GetText();

        GD.Print("Código a ejecutar: " + codigo);
        
        if (string.IsNullOrWhiteSpace(codigo))
        {
            PrintConsole("El código está vacío. Por favor, escribe algo antes de ejecutar.");
            return;
        }
        Compiler(codigo);
    }
    private void Compiler(string message)
    {
        string codigo = codeEdit.GetText();
        var core = new Core();
        RunResult resultado = core.Run(codigo, currentGridDivisions);

        // 1) Si hay errores, los mostramos en pantalla (p. ej. en un Panel o Label):
        if (resultado.Errors.Count > 0)
        {
            GD.Print($"{resultado.Errors.Count} Errores de compilación encontrados:");
            foreach (var err in resultado.Errors)
            {
                PrintConsole(err.ToString());
            }
            return;
        }

        // Si llegamos aquí, significa que no hay errores de compilación.
        GD.Print("Código compilado correctamente. Ejecutando...");
        GD.Print("AST");
        GD.Print(resultado.AST);
        // 2) No hay errores -> obtenemos la matriz de píxeles y la lista de instrucciones:
        Canvas canvas = resultado.Canvas;
        List<Instruction> instrucciones = resultado.Instructions;

        // 3) Limpiamos el canvas antes de pintar:
        Reset();
        // Pintamos la matriz de píxeles en el canvas
        Print(canvas);
    }

    private void Print(Canvas canvas)
    {
        Pixel[,] pixels = canvas.GetPixels();

        for (int y = 0; y < currentGridDivisions; y++)
        {
            for (int x = 0; x < currentGridDivisions; x++)
            {
                string color = pixels[y, x].ToString();

                if (canvas.GetWallEPosX() == y && canvas.GetWallEPosY() == x)
                {
                    // Pintar Wall-E en la posición actual
                    PintarCelda(y, x, new Color("#FFD700")); // Color dorado para Wall-E
                    GD.Print($"Pintando Wall-E en la celda ({y}, {x})");
                    continue;
                }

                if (color == "Transparent")
                {
                    GD.Print($"Celda ({x}, {y}) es transparente, no se pintará.");
                    continue; // No pintar celdas transparentes
                }

                PintarCelda(y, x, new Color(color));
            }
        }
    }
    private void PrintConsole(string message)
    {
        textOut.Text = message;
    }
    private void Reset()
    {
        canvasImage.Fill(Colors.White);
        canvasTexture.Update(canvasImage);
        textOut.Text = "";
    }
    private void _OnFileDialogSaveFileSelected(string ruta)
    {
        GuardarArchivo(ruta);
        GD.Print("Archivo guardado en: " + ruta);
    }

    private void _OnFileDialogLoadFileSelected(string ruta)
    {
        CargarArchivo(ruta);
        GD.Print("Archivo cargado desde: " + ruta);
    }

    private void OnBoardSizeChanged(double newValue)
    {
        // Limpiar el canvas antes de pintar
        Reset();
        int newSize = (int)newValue;
        currentGridDivisions = newSize;
        gridOverlay.SetGridDivisions(newSize);
        GD.Print("Nuevo número de divisiones: " + newSize);
    }

    private void InicializarCanvas()
    {
        canvasImage = Image.CreateEmpty(BoardPixelSize, BoardPixelSize, false, Image.Format.Rgba8);
        canvasImage.Fill(Colors.White);
        canvasTexture = ImageTexture.CreateFromImage(canvasImage);
        canvasTextureRect.Texture = canvasTexture;
    }

    // Función para pintar una celda completa en el canvas
    public void PintarCelda(int cellX, int cellY, Color color)
    {
        float cellSize = (float)BoardPixelSize / currentGridDivisions;
        int startX = (int)(cellX * cellSize);
        int startY = (int)(cellY * cellSize);
        int cellPixelSize = (int)cellSize;

        for (int x = startX; x < startX + cellPixelSize; x++)
        {
            for (int y = startY; y < startY + cellPixelSize; y++)
            {
                if (x < BoardPixelSize && y < BoardPixelSize)
                {
                    canvasImage.SetPixel(x, y, color);
                }
            }
        }
        canvasTexture.Update(canvasImage);
    }

    // Función para guardar el contenido del editor en un archivo
    private void GuardarArchivo(string ruta)
    {
        var archivo = Godot.FileAccess.Open(ruta, Godot.FileAccess.ModeFlags.Write);
        archivo.StoreString(codeEdit.GetText());
        archivo.Close();
    }

    // Función para cargar el contenido de un archivo en el editor
    private void CargarArchivo(string ruta)
    {
        Reset();
        var archivo = Godot.FileAccess.Open(ruta, Godot.FileAccess.ModeFlags.Read);
        string contenido = archivo.GetAsText();
        archivo.Close();
        codeEdit.SetText(contenido);
    }
}
