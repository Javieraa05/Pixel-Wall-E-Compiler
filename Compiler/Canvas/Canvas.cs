using System.Drawing;
using System;
using System.Collections.Generic;

namespace Wall_E.Compiler
{
    public class Canvas
    {
        Pixel[,] pixels;
        Wall_E Wall_E;
        int Size;

        public Canvas(int sizeCanvas)
        {
            Size = sizeCanvas;
            pixels = new Pixel[sizeCanvas, sizeCanvas];
            for (int i = 0; i < sizeCanvas; i++)
            {
                for (int j = 0; j < sizeCanvas; j++)
                {
                    pixels[i, j] = new Pixel("White");
                }
            }
            Wall_E = new Wall_E();
        }
        public Pixel[,] GetPixels()
        {
            return pixels;
        }
        public void PrintCanvas()
        {
            Console.WriteLine();

            for (int i = 0; i < pixels.GetLength(0); i++)
            {
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    if (Wall_E.PosX == i && Wall_E.PosY == j)
                    {
                        Console.Write("E ");
                    }
                    else
                    {
                        Console.Write(pixels[i, j].Color[0] + " ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Color: " + Wall_E.BrushColor + "\nSize: " + Wall_E.BrushSize + "\n(" + Wall_E.PosX + ", " + Wall_E.PosY + ")");
        }
        public string GetPixelColor(int x, int y)
        {
            ValidateCoords(x, y);
            return pixels[y, x].Color;
        }

        // Cambiar color de un píxel
        private void SetPixelColor(int x, int y, string color)
        {
            ValidateCoords(x, y);
            pixels[y, x].Color = color;
        }

        private void ValidateCoords(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size)
                throw new InvalidOperationException($"Coordenadas fuera de rango: ({x}, {y})");
        }

        /// <summary>
        /// Sitúa a Wall-E en (x, y).
        /// </summary>
        public void SpawnWallE(int x, int y)
        {
            ValidateCoords(x, y);
            Wall_E.MoveTo(x, y);
            PrintCanvas();
        }

        /// <summary>
        /// Cambia el color de la brocha de Wall-E.
        /// </summary>
        public void SetColor(string color)
        {
            Wall_E.SetBrushColor(color);
        }

        /// <summary>
        /// Cambia el tamaño de la brocha de Wall-E.
        /// </summary>
        public void SetSize(int size)
        {
            Wall_E.SetBrushSize(size);
        }

        /// <summary>
        /// Dibuja una línea desde la posición actual de Wall-E, 
        /// en la dirección (dirX, dirY) (cada uno debe ser -1, 0 o 1), 
        /// con longitud 'distance'. 
        /// La línea usa el color y tamaño actuales de la brocha.
        /// </summary>
        public void DrawLine(int dirX, int dirY, int distance)
        {
            // Validar dirección: al menos uno de dirX o dirY debe ser -1,0,1
            if (!(Math.Abs(dirX) == 1 && (dirY == 0)
               || (Math.Abs(dirY) == 1 && (dirX == 0))
               || (Math.Abs(dirX) == 1 && Math.Abs(dirY) == 1)))
            {
                throw new InvalidOperationException($"Dirección inválida para línea: ({dirX}, {dirY})");
            }
            if (distance < 1)
                throw new InvalidOperationException("Distance debe ser >= 1");

            int x = Wall_E.PosX;
            int y = Wall_E.PosY;

            PaintAt(x, y, Wall_E.BrushSize, Wall_E.BrushColor);

            for (int step = 0; step < distance; step++)
            {
                // Avanzar un paso en la dirección
                x += dirY;
                y += dirX;

                // Pintar un bloque de BrushSize × BrushSize centrado en (x,y)
                PaintAt(x, y, Wall_E.BrushSize, Wall_E.BrushColor);
            }

            ValidateCoords(y, x);
            Wall_E.MoveTo(x, y);
            PrintCanvas();
        }

        /// <summary>
        /// Dibuja un círculo con centro en la posición actual de Wall-E 
        /// desplazado por (dirX, dirY), con radio 'radius'.
        /// Usa color y tamaño de brocha para “engrosar” el contorno.
        /// </summary>
        public void DrawCircle(int dirX, int dirY, int radius)
        {
            if (radius < 0) throw new InvalidOperationException("Radius debe ser >= 0");

            // Centro del círculo = posición actual de Wall-E + (dirX, dirY)
            int centerX = Wall_E.PosX + dirX;
            int centerY = Wall_E.PosY + dirY;
            ValidateCoords(centerX, centerY);

            // Usamos el algoritmo de Bresenham para círculo o aproximación:
            int x0 = centerX;
            int y0 = centerY;
            int r = radius;

            int x = r;
            int y = 0;
            int err = 0;

            while (x >= y)
            {
                PlotCirclePoints(x0, y0, x, y, Wall_E.BrushColor, Wall_E.BrushSize);
                y++;
                err += 1 + 2 * y;
                if (2 * (err - x) + 1 > 0)
                {
                    x--;
                    err += 1 - 2 * x;
                }
            }
            Wall_E.MoveTo(centerX, centerY);
            PrintCanvas();
        }

        /// <summary>
        /// Dibuja un rectángulo con la esquina superior izquierda en:
        /// posición actual de Wall-E + (dirX, dirY), 
        /// de ancho 'width' y alto 'height'.
        /// Bordes: usan tamaño y color de brocha.
        /// </summary>
        public void DrawRectangle(int dirX, int dirY, int width, int height)
        {
            if (width < 1 || height < 1)
                throw new InvalidOperationException("Width y Height deben ser >= 1.");

            int startX = Wall_E.PosX + dirX;
            int startY = Wall_E.PosY + dirY;
            ValidateCoords(startX, startY);

            // Dibujar bordes: 4 líneas
            // Línea superior
            for (int dx = 0; dx < width; dx++)
                PaintAt(startX + dx, startY, Wall_E.BrushSize, Wall_E.BrushColor);
            // Línea inferior
            for (int dx = 0; dx < width; dx++)
                PaintAt(startX + dx, startY + height - 1, Wall_E.BrushSize, Wall_E.BrushColor);
            // Lado izquierdo
            for (int dy = 0; dy < height; dy++)
                PaintAt(startX, startY + dy, Wall_E.BrushSize, Wall_E.BrushColor);
            // Lado derecho
            for (int dy = 0; dy < height; dy++)
                PaintAt(startX + width - 1, startY + dy, Wall_E.BrushSize, Wall_E.BrushColor);
            // NOTA: no movemos a Wall-E
        }

        /// <summary>
        /// Rellena TODO el canvas con el color actual de la brocha.
        /// </summary>
        public void Fill()
        {
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    pixels[y, x].Color = Wall_E.BrushColor;
            // La posición de Wall-E no cambia.
        }

        // ===== Métodos auxiliares internos =====

        /// <summary>
        /// Pinta un bloque de “brushSize × brushSize” centrado en (cx, cy).
        /// Si brushSize = 1, se pinta solo (cx, cy). 
        /// Si brushSize > 1, pinta un cuadrado de lado brushSize centrado en el píxel
        /// (redondeando la posición para índices pares).
        /// </summary>
        private void PaintAt(int cx, int cy, int brushSize, string color)
        {
            // Si brushSize = 1, simplemente asignar el píxel (cx, cy):
            if (brushSize == 1)
            {
                if (IsInside(cx, cy))
                    pixels[cx, cy].Color = color;
                return;
            }

            // Para brushSize > 1, definimos offsets alrededor de (cx, cy)
            int half = brushSize / 2;
            for (int dy = -half; dy <= half; dy++)
            {
                for (int dx = -half; dx <= half; dx++)
                {
                    int px = cx + dx;
                    int py = cy + dy;
                    if (IsInside(px, py))
                    {
                        pixels[px, py].Color = color;
                    }
                }
            }
        }

        /// <summary>
        /// Herramienta para dibujar los 8 octantes del círculo usando el algoritmo de Bresenham.
        /// (Implementación simplificada: pinta puntos aislados, sin engrosar bordes).
        /// </summary>
        private void PlotCirclePoints(int x0, int y0, int x, int y, string color, int brushSize)
        {
            void PaintSafe(int px, int py)
            {
                if (IsInside(px, py))
                    PaintAt(px, py, brushSize, color);
            }

            PaintSafe(x0 + x, y0 + y);
            PaintSafe(x0 - x, y0 + y);
            PaintSafe(x0 + x, y0 - y);
            PaintSafe(x0 - x, y0 - y);
            PaintSafe(x0 + y, y0 + x);
            PaintSafe(x0 - y, y0 + x);
            PaintSafe(x0 + y, y0 - x);
            PaintSafe(x0 - y, y0 - x);
        }

        /// <summary>
        /// Verifica si las coordenadas (x, y) están dentro de los límites del canvas.
        /// </summary>
        private bool IsInside(int x, int y)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size;
        }

    }

    public class Pixel
    {
        public string Color;

        public Pixel(string color)
        {
            Color = color;
        }
        public override string ToString()
        {
            return Color;
        }
    }

    public class Wall_E
    {
        public string BrushColor { get; private set; }
        public int BrushSize { get; private set; }
        public int PosX { get; private set; }
        public int PosY { get; private set; }
        public Wall_E()
        {
            BrushColor = "Transparent";
            BrushSize = 1;
            PosX = 0;
            PosY = 0;
        }
        public void SetBrushColor(string color)
        {
            BrushColor = color;
        }
        public void SetBrushSize(int size)
        {
            if (size < 1) throw new ArgumentException("El tamaño de brocha debe ser >= 1.");
            BrushSize = size;
        }
        public void MoveTo(int x, int y)
        {
            PosX = x;
            PosY = y;
        }
    }
}