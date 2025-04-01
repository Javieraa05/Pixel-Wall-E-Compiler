using Godot;
using System;

public partial class Main : Control
{
    // Referencias a los nodos de la interfaz
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
        // Obtén las referencias a los nodos hijos (ajusta las rutas según tu escena)
        boardSizeSpinBox = GetNode<SpinBox>("HBoxContainer/EditContainer/MarginContainer2/ButtonContainer/SpinBox");
        gridOverlay = GetNode<GridOverlay>("HBoxContainer/DisplayContainer/CanvasContainer/GridOverlay");
        canvasTextureRect = GetNode<TextureRect>("HBoxContainer/DisplayContainer/CanvasContainer/TextureRect");

        // Conectar la señal del SpinBox para detectar cambios en la cantidad de divisiones
        boardSizeSpinBox.ValueChanged += OnBoardSizeChanged;

        // Inicializa el canvas (imagen) y la textura
        InicializarCanvas();

        // Almacena el valor inicial del SpinBox
        currentGridDivisions = (int)boardSizeSpinBox.Value;


    }

    private void OnBoardSizeChanged(double newValue)
    {
        int newSize = (int)newValue;
        currentGridDivisions = newSize;

        // Actualiza la cuadrícula con el nuevo número de divisiones
        gridOverlay.SetGridDivisions(newSize);
        GD.Print("Nuevo número de divisiones: " + newSize);
		
    }

    private void InicializarCanvas()
	{
    	// Crea la Image de 900x900 píxeles usando CreateEmpty, ya que Create está obsoleto
    	canvasImage = Image.CreateEmpty(BoardPixelSize, BoardPixelSize, false, Image.Format.Rgba8);
    	canvasImage.Fill(Colors.White);

    	// Usa el método estático CreateFromImage para crear la textura a partir de la imagen
    	canvasTexture = ImageTexture.CreateFromImage(canvasImage);
    	canvasTextureRect.Texture = canvasTexture;
	}



    // Función para pintar una celda completa (de la cuadrícula) en el canvas
    // cellX y cellY indican la posición de la celda en la cuadrícula (desde 0)
    public void PintarCelda(int cellX, int cellY, Color color)
    {
        // Calcula el tamaño de cada celda en píxeles
        float cellSize = (float)BoardPixelSize / currentGridDivisions;
        int startX = (int)(cellX * cellSize);
        int startY = (int)(cellY * cellSize);
        int cellPixelSize = (int)cellSize;

        // Recorre cada píxel de la celda y le asigna el color
        for (int x = startX; x < startX + cellPixelSize; x++)
        {
            for (int y = startY; y < startY + cellPixelSize; y++)
            {
                // Asegúrate de que las coordenadas estén dentro del canvas
                if (x < BoardPixelSize && y < BoardPixelSize)
                {
                    canvasImage.SetPixel(x, y, color);
                }
            }
        }
        // Actualiza la textura para reflejar los cambios en pantalla
        canvasTexture.Update(canvasImage);
    }
}
