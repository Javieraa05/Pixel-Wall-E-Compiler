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

![Pantalla principal](docs/img/main_screen.png)
![Ejemplo de dibujo](docs/img/example_art.png)

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

¡Controla a Wall-E con estos comandos!  
Cada instrucción va en una línea diferente. Los comandos se ejecutan en orden, de arriba hacia abajo.

### 1. `Spawn(int x, int y)`
Posiciona a Wall-E en la coordenada `(x, y)` del canvas.  
**¡Debe ser el primer comando del programa y solo puede aparecer una vez!**

```pw
Spawn(10, 10)
```

---

### 2. `Color(string color)`
Cambia el color del pincel de Wall-E.  
Colores permitidos (insensible a mayúsculas, pero se recomienda usar como abajo):

- `"Red"`
- `"Blue"`
- `"Green"`
- `"Yellow"`
- `"Orange"`
- `"Purple"`
- `"Black"`
- `"White"` (puede usarse como "borrador")
- `"Transparent"` (no pinta, sirve para “levantar” el pincel)

```pw
Color("Blue")
```

---

### 3. `Size(int k)`
Cambia el grosor del pincel a `k` (en píxeles).  
- Solo acepta números impares mayores que 0.
- Si escribes un número par, se usará el impar menor inmediato.

```pw
Size(5)
```

---

### 4. `DrawLine(int dirX, int dirY, int distance)`
Dibuja una línea desde la posición actual de Wall-E.  
Parámetros:
- `dirX` y `dirY`: Dirección del trazo (`-1`, `0`, `1`), pueden usarse para diagonales.
- `distance`: Longitud de la línea en píxeles.

Direcciones:
- `(1, 0)`: Derecha
- `(-1, 0)`: Izquierda
- `(0, 1)`: Abajo
- `(0, -1)`: Arriba
- `(1, 1)`: Diagonal abajo-derecha
- `(-1, 1)`: Diagonal abajo-izquierda
- `(1, -1)`: Diagonal arriba-derecha
- `(-1, -1)`: Diagonal arriba-izquierda

```pw
DrawLine(1, 0, 10)       // Derecha 10 píxeles
DrawLine(0, 1, 5)        // Abajo 5 píxeles
DrawLine(-1, -1, 7)      // Diagonal arriba-izquierda
```

---

### 5. `DrawCircle(int dirX, int dirY, int radius)`
Dibuja un círculo centrado en la posición actual de Wall-E desplazada por `(dirX, dirY)`.  
- `radius`: radio del círculo.

```pw
DrawCircle(0, 0, 8)
```

---

### 6. `DrawRectangle(int dirX, int dirY, int distance, int width, int height)`
Dibuja un rectángulo con la esquina superior izquierda desplazada desde la posición actual de Wall-E.
- `distance`: cuántos píxeles mover antes de comenzar a dibujar.
- `width`: ancho del rectángulo.
- `height`: alto del rectángulo.

```pw
DrawRectangle(1, 0, 5, 12, 8)
```

---

### 7. Asignaciones y Variables

Puedes crear y usar variables para almacenar valores numéricos o cadenas:

```pw
let x = 20
let color = "Red"
Color(color)
Spawn(x, x)
```

---

### 8. Funciones, Etiquetas y Saltos Condicionales

#### Funciones

Define bloques de código reutilizables:

```pw
function Cuadrado() {
  Size(3)
  DrawLine(1, 0, 10)
  DrawLine(0, 1, 10)
  DrawLine(-1, 0, 10)
  DrawLine(0, -1, 10)
}
Cuadrado()
```

#### Etiquetas y saltos

Puedes marcar y saltar a etiquetas bajo ciertas condiciones:

```pw
label Repetir
DrawLine(1, 0, 5)
if (x < 10) goto Repetir
```

---

### 9. Comentarios

Agrega comentarios a tu código usando `//`:

```pw
// Esto dibuja una línea azul
Color("Blue")
DrawLine(1, 0, 15)
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

## 📝 Ejemplo Completo

```pw
// Dibuja un cuadrado rojo en el centro
Spawn(16, 16)
Color("Red")
Size(3)
DrawLine(1, 0, 8)
DrawLine(0, 1, 8)
DrawLine(-1, 0, 8)
DrawLine(0, -1, 8)
```

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
