# 🤖🎨 Pixel Wall-E: ¡Programa y Dibuja con tu Robot Favorito! 🎨🤖

¡Bienvenido/a a **Pixel Wall-E**, el proyecto donde la programación y el arte digital se encuentran!  
¿Te gustaría ver a Wall-E crear pixel-art siguiendo tus instrucciones? Con Pixel Wall-E puedes escribir código sencillo, ejecutar tus comandos y observar cómo Wall-E da vida a tus creaciones píxel a píxel.

---

## 🚀 ¿Qué es Pixel Wall-E?

Pixel Wall-E es una aplicación educativa y divertida para aprender conceptos de programación, lógica y arte digital. Usando un pequeño lenguaje de comandos, puedes controlar a Wall-E para que pinte, borre, y cree figuras y patrones en un lienzo cuadrado.

---

## 🖼️ Capturas de Pantalla

<!--
Aquí puedes insertar tus capturas de pantalla:
1. Interfaz principal
2. Ejemplo de código y resultado
3. Mensajes de error amigables
-->

![Pantalla principal](Img/Captura de pantalla3.png)
![Ejemplo de dibujo](Img/Captura de pantalla1.png)
![Ejemplo de dibujo](Img/Captura de pantalla2.png)

---

## 🕹️ ¿Cómo empezar?

### 1. Descarga y ejecuta

1. Clona este repositorio.
2. Ve a la carpeta `release/` y descomprime el archivo `.zip` que contiene el ejecutable (`PixelWallE.exe`).
3. ¡Haz doble clic en `PixelWallE.exe` y comienza a crear tu arte!

> **Nota:** No necesitas instalar nada adicional. ¡Solo clona, descomprime y ejecuta!

---

## ✨ Características Principales

- **Editor de código integrado:** Escribe tus comandos y ejecútalos al instante.
- **Soporte para archivos:** Guarda tus creaciones o carga archivos `.pw` para editarlos y mejorarlos.
- **Lenguaje propio fácil de aprender:** Con instrucciones intuitivas y mensajes de error claros.
- **Visualización en tiempo real:** Observa cómo Wall-E interpreta y dibuja tu código.
- **Errores detallados:** El compilador te ayuda a encontrar y corregir tus errores.

---

## 📚 Sintaxis Completa del Lenguaje Pixel Wall-E

El lenguaje de Pixel Wall-E está compuesto por **instrucciones**, **asignaciones**, **expresiones**, **funciones**, **etiquetas** y **saltos condicionales**. Todas las instrucciones van en líneas separadas y se ejecutan de arriba hacia abajo.

---

### 🔧 Instrucciones

#### `Spawn(int x, int y)`
Inicializa a Wall-E en la posición `(x, y)` del canvas.  
✅ **Debe ser la primera instrucción del programa y solo puede aparecer una vez.**

#### `ReSpawn(int x, int y)`
Mueve a Wall-E a la posición `(x, y)` del canvas.  

#### `Color(string color)`
Cambia el color del pincel. Valores válidos:

`"Red"`, `"Blue"`, `"Green"`, `"Yellow"`, `"Orange"`, `"Purple"`, `"Black"`, `"White"`, `"Transparent"`

- `"White"` puede usarse como borrador.
- `"Transparent"` implica no pintar.

#### `Size(int k)`
Modifica el tamaño de la brocha.  
- `k` debe ser un número impar positivo.  
- Si es par, se usa el número impar inmediatamente menor.  
- Valor por defecto: 1.

#### `DrawLine(int dirX, int dirY, int distance)`
Dibuja una línea desde la posición actual. Wall-E se mueve hasta el último píxel dibujado.

Direcciones válidas:
```
(-1, -1) Diagonal arriba izquierda
(-1,  0) Izquierda
(-1,  1) Diagonal abajo izquierda
( 0,  1) Abajo
( 1,  1) Diagonal abajo derecha
( 1,  0) Derecha
( 1, -1) Diagonal arriba derecha
( 0, -1) Arriba
```

#### `DrawCircle(int dirX, int dirY, int radius)`
Dibuja un círculo con centro a `radius` de la posición actual en la dirección `(dirX, dirY)`.

#### `DrawRectangle(int dirX, int dirY, int distance, int width, int height)`
Dibuja un rectángulo. Wall-E se mueve `distance` en `(dirX, dirY)` y esa es la posición central del rectángulo.

#### `Fill()`
Rellena de color los píxeles contiguos al actual que tienen el mismo color original y no estén bloqueados por otros colores.

---

### 🧮 Asignaciones

```pw
variable <- expresión
```

- El nombre puede tener letras, números y espacios.
- No puede empezar con número ni con `-`.
- La expresión puede ser aritmética o booleana.

---

### ➕ Expresiones Aritméticas

Componentes válidos:

- Literales enteros
- Variables numéricas
- Funciones numéricas
- Operaciones: `+`, `-`, `*`, `/`, `**`, `%`

---

### 🔁 Expresiones Booleanas

- Comparaciones: `==`, `>=`, `<=`, `>`, `<`
- Operadores:
  - `&&` (AND)
  - `||` (OR) → **tiene mayor precedencia que AND**

---

### 🧩 Funciones

```pw
x <- GetActualX()
```

Funciones disponibles:

- `GetActualX()` → Coordenada X actual
- `GetActualY()` → Coordenada Y actual
- `GetCanvasSize()` → Lado del canvas
- `GetColorCount(string color, int x1, y1, x2, y2)` → Cuántos píxeles del color hay dentro del cuadrado formado por las dos esquinas
- `IsBrushColor(string color)` → `1` si el color actual es ese
- `IsBrushSize(int size)` → `1` si el tamaño actual coincide
- `IsCanvasColor(string color, int vertical, int horizontal)` → Verifica el color de una casilla relativa a Wall-E

---

### 🏷️ Etiquetas y Saltos Condicionales

#### Etiquetas

Marcan una línea del código. No hacen nada por sí mismas, pero permiten hacer saltos.

```pw
loop_start
```

#### Saltos

```pw
GoTo [label] (condición)
```

- `label` debe existir en el código.
- `condición` puede ser:
  - Variable booleana
  - Comparación entre literales o variables numéricas

Si la condición es verdadera, el flujo salta a la etiqueta. Si es falsa, se continúa a la línea siguiente.

---

### 🧾 Ejemplo de Código

```pw
Spawn(0, 0)
Color("Black")
n <- 5
k <- 3 + 3 * 10
n <- k * 2
actual_x <- GetActualX()
i <- 0

loop1
DrawLine(1, 0, 1)
i <- i + 1
is_blue <- IsBrushColor("Blue")
GoTo [loop_end] (is_blue == 1)
GoTo [loop1] (i < 10)

Color("Blue")
GoTo [loop1] (1 == 1)

loop_end
```

---

## 🛠️ Guía Rápida de Uso

1. **Abre PixelWallE.exe**  
   (¡Recuerda haberlo descomprimido desde el ZIP!)
2. **Escribe tu código** en el editor o importa un archivo `.pw`.
3. **Pulsa el botón "Run"** para ejecutar tus comandos.
4. **Observa a Wall-E en acción** en el canvas.
5. **Guarda tu arte** o exporta tu código para compartirlo.

---

## 📂 Carga/Guarda tus proyectos

- **Guardar:** Usa el botón “Save” para exportar tu código actual a un archivo `.pw`.
- **Cargar:** Usa el botón “Load” para abrir un archivo `.pw` existente y continuar editando.

---

## 💡 Consejos y Trucos

- Usa colores y tamaños de pincel para lograr efectos de sombra o volumen.
- Puedes crear figuras complejas combinando funciones y variables.
- Si ocurre un error (por ejemplo, si te sales del canvas), Pixel Wall-E te avisará con un mensaje claro.

---

## 🏷️ Créditos

- Desarrollado por Javier Aristigui Aguilar.
- 2do Proyecto de Programacion, Ciencias de la Computación, MATCOM 2024-2025.

---

## 🪪 Licencia

[MIT]

---

## 📬 Contacto

¿Dudas, sugerencias o quieres mostrar tu arte?  
¡Abre un issue o contacta a Javieraa05 aquí en GitHub!

---

¡Diviértete programando y creando arte con Wall-E!
