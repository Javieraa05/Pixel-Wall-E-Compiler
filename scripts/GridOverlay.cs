using Godot;

public partial class GridOverlay : Control
{
    // Tamaño fijo del tablero en píxeles.
    private const int BoardPixelSize = 900;

    // Número de divisiones en cada eje (por defecto, 32 celdas por lado)
    public int GridDivisions = 32;

    // Tamaño de cada celda calculado dinámicamente
    private float CellSize => (float)BoardPixelSize / GridDivisions;

    public override void _Ready()
    {
        // Fijamos el tamaño del nodo a 900x900 píxeles.
        Size = new Vector2(BoardPixelSize, BoardPixelSize);
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Dibuja las líneas verticales
        for (int i = 0; i <= GridDivisions; i++)
        {
            float xPos = i * CellSize;
            DrawLine(new Vector2(xPos, 0), new Vector2(xPos, BoardPixelSize), Colors.Black, 1);
        }
        
        // Dibuja las líneas horizontales
        for (int i = 0; i <= GridDivisions; i++)
        {
            float yPos = i * CellSize;
            DrawLine(new Vector2(0, yPos), new Vector2(BoardPixelSize, yPos), Colors.Black, 1);
        }
    }

    // Este método se llamará para actualizar la cantidad de divisiones y redibujar la cuadrícula.
    public void SetGridDivisions(int newDivisions)
    {
        GridDivisions = newDivisions;
        QueueRedraw();
    }
}
