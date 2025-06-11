# Pixel Wall-E

2do Proyecto de Programación  
**Pixel Wall-E**  
MATCOM - 2024-2025

## Introducción

Pixel Wall-E es una aplicación educativa que permite crear arte digital en un canvas pixelado mediante un lenguaje de programación simple y personalizado. El usuario ayuda a Wall-E, nuestro robot pixel-artista, a ejecutar comandos que modifican un lienzo cuadrado, permitiendo dibujar, colorear y manipular píxeles de manera programática.

## Características principales

- **Editor de código:** Escribe comandos en un editor de texto integrado.
- **Ejecución visual:** Al pulsar un botón, los comandos se interpretan y el resultado se muestra en el canvas.
- **Importación y exportación:** Permite cargar archivos `.pw` y exportar el contenido del editor.
- **Lenguaje propio:** Admite instrucciones para manipular la posición, color, tamaño y dibujo de Wall-E sobre el canvas.
- **Detección y manejo de errores:** El sistema identifica errores sintácticos, semánticos y de ejecución.

## Formato de entrada

- **Desde el editor:** Escribe comandos directamente en la aplicación.
- **Desde archivos:** Importa archivos `.pw` para cargar código.
- **Exportar:** Guarda el código del editor en archivos `.pw`.

## Resumen del Lenguaje

El lenguaje de Pixel Wall-E consta de instrucciones separadas por saltos de línea. Los comandos incluyen:

### 1. Spawn(int x, int y)
Inicializa a Wall-E en el canvas en la posición `(x, y)`.  
**Obligatorio** como primer comando y solo se permite una vez.  
Ejemplo:
```
Spawn(0, 0)
```

### 2. Color(string color)
Cambia el color del pincel. Colores permitidos:
`"Red"`, `"Blue"`, `"Green"`, `"Yellow"`, `"Orange"`, `"Purple"`, `"Black"`, `"White"`, `"Transparent"`  
El color por defecto es `"Transparent"`.

### 3. Size(int k)
Cambia el grosor del pincel a `k` píxeles (debe ser impar). El valor por defecto es 1 píxel.

### 4. DrawLine(int dirX, int dirY, int distance)
Dibuja una línea desde la posición actual de Wall-E, en la dirección `(dirX, dirY)` y longitud `distance` píxeles. Las direcciones válidas para `dirX` y `dirY` son -1, 0, 1.

Ejemplo:
```
DrawLine(1, 0, 5)
```
Esto dibuja una línea horizontal hacia la derecha.

## Ejemplo de uso

```pw
Spawn(10, 10)
Color("Blue")
Size(3)
DrawLine(1, 0, 5)
```

## Instalación y uso

1. Clona el repositorio.
2. Abre el proyecto en Godot (versión 4.x recomendada).
3. Ejecuta la escena principal.
4. Escribe tus comandos en el editor o importa un archivo `.pw`.
5. Pulsa "Run" para ver el resultado en el canvas.
6. Guarda tu trabajo cuando lo desees.

## Licencia

[Especificar licencia aquí]

## Créditos

Desarrollado como proyecto académico para MATCOM 2024-2025.
