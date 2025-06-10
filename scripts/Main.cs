using Godot;
using System;
using Wall_E.Compiler;
using System.Collections.Generic;
using System.IO;
public partial class Main : Control
{
    // Botones
    private Button runButton;
    private Button loadButton;
    private Button saveButton;
    private Button checkWall_E;
    private Button resetButton;
    private Button documentationButton;
    private bool Wall_E_Paint = false;
    
    // Nodo del editor de código
    private CodeEdit codeEdit;
    private TextEdit textOut;
    private TextEdit textPosition;
    
    // FileDialogs para guardar y cargar archivos
    private FileDialog fileDialogSave;
    private FileDialog fileDialogLoad;

    // Referencias a otros nodos de la interfaz
    private SpinBox boardSizeSpinBox;

    // Tamaño fijo del canvas en píxeles
    private const int BoardPixelSize = 900;

    // Imagen y textura para el canvas
    private Image canvasImage;
    private ImageTexture canvasTexture;
    private TextureRect canvasTextureRect;
    // Almacena el número actual de divisiones de la cuadrícula
    private int currentGridDivisions = 32;


    public override void _Ready()
    {
        // Obtén las referencias a los nodos hijos
        boardSizeSpinBox = GetNode<SpinBox>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/SpinBox");
        canvasTextureRect = GetNode<TextureRect>("HBoxContainer/CanvasContainer/Canvas/TextureRect");
        saveButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Save");
        loadButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Load");
        runButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Run");
        resetButton = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/Reset");
        checkWall_E = GetNode<Button>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/CheckWallE");
        documentationButton = GetNode<Button>("HBoxContainer/CanvasContainer/HContainer/MarginButtom/Documentation");
        codeEdit = GetNode<CodeEdit>("HBoxContainer/EditContainer/MarginContainer/CodeEdit");
        textOut = GetNode<TextEdit>("HBoxContainer/EditContainer/MarginContainer3/TextEdit");
        textPosition = GetNode<TextEdit>("HBoxContainer/CanvasContainer/HContainer/MarginText/TextPosition");

        

        // Conectar señales de botones
        boardSizeSpinBox.ValueChanged += OnBoardSizeChanged;
        runButton.Pressed += OnRunButtonPressed;
        saveButton.Pressed += OnSaveButtonPressed;
        loadButton.Pressed += OnLoadButtonPressed;
        checkWall_E.Pressed += OnCheckWallEPressed;
        resetButton.Pressed += OnResetButtonPressed;
        documentationButton.Pressed += OnDocumentationButtonPressed;

        // Inicializa el canvas (imagen) y la textura
        InicializarCanvas();
        currentGridDivisions = (int)boardSizeSpinBox.Value;

        // Crear y configurar el FileDialog para guardar archivos
        fileDialogSave = new FileDialog();
        fileDialogSave.FileMode = FileDialog.FileModeEnum.SaveFile;
        // Usamos Filesystem en lugar de User
        fileDialogSave.Access = FileDialog.AccessEnum.Filesystem;
        // No se asigna la propiedad Mode, ya que produce error en esta versión
        fileDialogSave.Filters = new string[] { "*.gw" };
        AddChild(fileDialogSave);
        fileDialogSave.FileSelected += _OnFileDialogSaveFileSelected;

        // Crear y configurar el FileDialog para cargar archivos
        fileDialogLoad = new FileDialog();
        fileDialogLoad.FileMode = FileDialog.FileModeEnum.OpenAny;
        fileDialogLoad.Access = FileDialog.AccessEnum.Filesystem;
        // No se asigna la propiedad Mode, ya que produce error en esta versión
        fileDialogLoad.Filters = new string[] { "*.gw" };
        AddChild(fileDialogLoad);
        fileDialogLoad.FileSelected += _OnFileDialogLoadFileSelected;
    }

  
    private void Compiler()
    {
        // Obtener el código del editor
        string codigo = codeEdit.GetText();
        // Imprimir el código en la consola para depuración
        GD.Print("Ejecutar Código");
        GD.Print("Código a ejecutar: " + codigo);
        if (string.IsNullOrWhiteSpace(codigo))
        {
            PrintConsole("El código está vacío. Por favor, escribe algo antes de ejecutar.");
            return;
        }
        Core core = new Core();

        RunResult resultado = core.Run(codigo, currentGridDivisions);


        GD.Print(resultado.AST);
        // 1) Si hay errores, los mostramos en pantalla (p. ej. en un Panel o Label):
        if (resultado.Errors.Count > 0)
        {
            GD.Print($"{resultado.Errors.Count} Errores de compilación encontrados:");
            string errorMessage = "";
            foreach (var err in resultado.Errors)
            {
                errorMessage += err.ToString() + "\n";
            }
            PrintConsole(errorMessage);
            return;
        }

        // Si llegamos aquí, significa que no hay errores de compilación.
        GD.Print("Código compilado correctamente. Ejecutando...");

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
        int divs = currentGridDivisions;
        int baseW = BoardPixelSize / divs;
        int remW = BoardPixelSize % divs;
        int baseH = BoardPixelSize / divs;
        int remH = BoardPixelSize % divs;

        int yPos = 0;
        for (int y = 0; y < divs; y++)
        {
            // Altura de esta fila:
            int rowHeight = (y < remH) ? baseH + 1 : baseH;

            int xPos = 0;
            for (int x = 0; x < divs; x++)
            {
                int colWidth = (x < remW) ? baseW + 1 : baseW;
                var rect = new Rect2I(new Vector2I(xPos, yPos), new Vector2I(colWidth, rowHeight));

                string color = pixels[y, x].ToString();
                if ((canvas.GetWallEPosX() == x && canvas.GetWallEPosY() == y) && Wall_E_Paint)
                {

                    Texture2D iconTexture = GD.Load<Texture2D>("res://Img/WallE.png");
                    Image imageWallE= iconTexture.GetImage();
                    imageWallE.Convert(Image.Format.Rgba8);
                    canvasImage.BlitRect(imageWallE, new Rect2I(Vector2I.Zero, imageWallE.GetSize()), rect.Position);
                    GD.Print($"Walle: ({y},{x})");
                }
                else if (color != "Transparent")
                {
                    canvasImage.FillRect(rect, new Color(color));
                }

                xPos += colWidth;
            }

            yPos += rowHeight;
        }



        canvasTexture.Update(canvasImage);
        canvasTextureRect.Texture = canvasTexture;
        PintarCuadrícula();
        ChangeTextPosition(canvas.GetWallEPosX(), canvas.GetWallEPosY());
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
        ChangeTextPosition(0, 0);
    }
    private void InicializarCanvas()
    {
        canvasImage = Image.CreateEmpty(BoardPixelSize, BoardPixelSize, false, Image.Format.Rgba8);
        canvasImage.Fill(Colors.White);
        canvasTexture = ImageTexture.CreateFromImage(canvasImage);
        canvasTextureRect.Texture = canvasTexture;
        PintarCuadrícula();
        ChangeTextPosition(0, 0);
    }
    public void PintarCuadrícula()
    {
        Color gridColor = Colors.Black;
        int divs = currentGridDivisions;
        int baseW = BoardPixelSize / divs;
        int remW = BoardPixelSize % divs;
        int baseH = BoardPixelSize / divs;
        int remH = BoardPixelSize % divs;

        // Acumulamos posiciones de línea en X
        int xPos = 0;
        for (int i = 0; i <= divs; i++)
        {
            // Dibujo una línea vertical 1px de ancho en xPos
            var lineV = new Rect2I(new Vector2I(xPos, 0), new Vector2I(1, BoardPixelSize));
            canvasImage.FillRect(lineV, gridColor);

            // Incremento xPos: para i<divs avanzo ancho de celda i
            if (i < divs)
                xPos += (i < remW) ? (baseW + 1) : baseW;
        }

        // Acumulamos posiciones de línea en Y
        int yPos = 0;
        for (int j = 0; j <= divs; j++)
        {
            var lineH = new Rect2I(new Vector2I(0, yPos), new Vector2I(BoardPixelSize, 1));
            canvasImage.FillRect(lineH, gridColor);

            if (j < divs)
                yPos += (j < remH) ? (baseH + 1) : baseH;
        }

        canvasTexture.Update(canvasImage);
        canvasTextureRect.Texture = canvasTexture;
    }
     private void OnRunButtonPressed()
    {
        Compiler();
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
    private void OnResetButtonPressed()
    {
        GD.Print("Reiniciar Canvas");
        Reset();
        PintarCuadrícula();
    }
    private void OnCheckWallEPressed()
    {
        Wall_E_Paint = !Wall_E_Paint;
    }
    private void OnDocumentationButtonPressed()
    {
        GD.Print("Abrir Documentación");
        // Aquí puedes abrir la documentación en un navegador o mostrarla en un panel
        // Por ejemplo, abrir un enlace web:
        OS.ShellOpen("www.google.com");
    }
    private void _OnFileDialogSaveFileSelected(string ruta)
    {
        GuardarArchivo(ruta);
        PintarCuadrícula();
        GD.Print("Archivo guardado en: " + ruta);
    }
    private void _OnFileDialogLoadFileSelected(string ruta)
    {
        CargarArchivo(ruta);
        PintarCuadrícula();
        GD.Print("Archivo cargado desde: " + ruta);
    }
    private void ChangeTextPosition(int x, int y)
    {
        // Actualizar la posición del texto en el TextEdit
        textPosition.Text = $"({x}, {y})";
        
    }
    private void OnBoardSizeChanged(double newValue)
    {
        // Limpiar el canvas antes de pintar
        Reset();
        int newSize = (int)newValue;
        currentGridDivisions = newSize;
        PintarCuadrícula();
        GD.Print("Nuevo número de divisiones: " + newSize);
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
