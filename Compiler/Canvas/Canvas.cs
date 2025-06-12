using System.Drawing;
using System;
using System.Collections.Generic;
using Godot;

namespace Wall_E.Compiler
{
    public class Canvas
    {
        Pixel[,] pixels;
        Wall_E Wall_E;
        public int Size { get; }

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
        
        public string GetBrushColor()
        {
            return Wall_E.GetBrushColor();
        }
        
        public string GetPixelColor(int x, int y)
        {
            return pixels[y, x].Color;
        }

        public int GetWallEPosX()
        {
            return Wall_E.PosX;
        }
        public int GetWallEPosY()
        {
            return Wall_E.PosY;
        }



        /// <summary>
        /// Sitúa a Wall-E en (x, y).
        /// </summary>
        public void SpawnWallE(int x, int y)
        {
            Wall_E.MoveTo(x, y);
        }
        

        /// <summary>
        /// Dibuja una línea desde la posición actual de Wall-E, 
        /// en la dirección (dirX, dirY) (cada uno debe ser -1, 0 o 1), 
        /// con longitud 'distance'. 
        /// La línea usa el color y tamaño actuales de la brocha.
        /// </summary>
        public void DrawLine(int dirX, int dirY, int distance)
        {

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

            Wall_E.MoveTo(x + Wall_E.BrushSize/2, y + Wall_E.BrushSize/2);

        }

        /// <summary>
        /// Dibuja un círculo con centro en la posición actual de Wall-E 
        /// desplazado por (dirX, dirY), con radio 'radius'.
        /// Usa color y tamaño de brocha para “engrosar” el contorno.
        /// </summary>
        public void DrawCircle(int dirX, int dirY, int radius)
        {
            for (int i = 0; i < radius; i++)
            {
                Wall_E.MoveTo(Wall_E.PosX + dirX, Wall_E.PosY + dirY);
            }

            // Usamos el algoritmo de Bresenham para círculo o aproximación:
            int x0 = Wall_E.PosX;
            int y0 = Wall_E.PosY;
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
        }

        /// <summary>
        /// Dibuja un rectángulo con la esquina superior izquierda en:
        /// posición actual de Wall-E + (dirX, dirY), 
        /// de ancho 'width' y alto 'height'.
        /// Bordes: usan tamaño y color de brocha.
        /// </summary>
        public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            for (int i = 0; i < distance; i++)
            {
                // Mover a Wall-E en la dirección indicada
                int newX = Wall_E.PosX + dirX;
                int newY = Wall_E.PosY + dirY;
                Wall_E.MoveTo(newX,newY);
            }

            int startX = Wall_E.PosX - width / 2;
            int startY = Wall_E.PosY - height / 2;

            GD.Print($"Empezar rectangulo en ({startX},{startY})");


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

        }
        
        public int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            int count = 0;
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    if (pixels[i, j].Color == color)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Rellena TODO el canvas con el color actual de la brocha.
        /// </summary>
        public void Fill()
        {
            List<(int x, int y)> visited = new List<(int, int)>();

            Queue<(int x, int y)> ToVisit = new Queue<(int, int)>();

            int startX = Wall_E.PosX;
            int startY = Wall_E.PosY;

            string current = Wall_E.BrushColor;

            string color = pixels[startX, startY].Color;

            // Si ya es del mismo color, no hay nada que hacer
            if (current == color) return;

            ToVisit.Enqueue((startX, startY));

            pixels[startX, startY].Color = current;


            int[] dirX = { 0, 1, 0, -1 };
            int[] dirY = { 1, 0, -1, 0 };

            while (ToVisit.Count > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int newX = ToVisit.Peek().x + dirX[i];
                    int newY = ToVisit.Peek().y + dirY[i];

                    if (Valid(newX, newY, color))
                    {

                        pixels[newX, newY].Color = current;
                        ToVisit.Enqueue((newX, newY));
                    }
                }
                ToVisit.Dequeue();
            }


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

        public bool Valid(int x, int y, string color)
        {
            return x >= 0 && x < Size && y >= 0 && y < Size && pixels[x, y].Color == color;
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
            BrushSize = size;
        }
        public void MoveTo(int x, int y)
        {
            PosX = x;
            PosY = y;
        }
        public string GetBrushColor()
        {
            return BrushColor;
        }
        
    }
}