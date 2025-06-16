# Informe Técnico: Wall_E Compiler Library

Este informe describe el flujo y el diseño técnico de la librería Wall_E Compiler, desde el análisis léxico (Lexer), análisis sintáctico (Parser), interpretación (Interpreter), manejo gráfico (Canvas), y sus clases auxiliares (Tokens, Expr, Stmt, etc). Además, se justifica la estructura y se revisan los modificadores de acceso empleados.

---

## 1. Flujo general del programa

La clase principal de entrada es `Core` (ver `WallE_Compiler_Lib/src/Core/Core.cs`), que implementa el método público `Run(string source, int sizeCanvas = 32)`. El flujo es:

1. **Lexer**: Convierte el código fuente en una lista de tokens.
2. **Parser**: Convierte la lista de tokens en una lista de sentencias (AST).
3. **Interpreter**: Ejecuta el AST, actualizando el entorno y el canvas.
4. **Canvas**: Gestiona la representación gráfica y el estado del robot Wall-E.

### 1.1 Lexer

- **Responsabilidad:** Analiza el texto fuente y produce una lista de tokens.
- **Justificación:** Separar el análisis léxico facilita la modularidad y el manejo de errores léxicos.
- **Accesibilidad:** El Lexer es instanciado como objeto local en `Core.Run` (no expuesto innecesariamente).
- **Errores:** Si hay errores, se almacenan y se retorna inmediatamente.

### 1.2 Parser

- **Responsabilidad:** Convierte los tokens en un Árbol de Sintaxis Abstracta (AST) compuesto por nodos Expr y Stmt.
- **Justificación:** Separar parsing permite validar la sintaxis y construir una representación estructurada para la interpretación.
- **Accesibilidad:** El Parser es una clase pública, pero sus métodos de parsing suelen ser privados o protegidos, limitando el acceso externo.
- **Manejo de Errores:** Si hay errores sintácticos, se recogen y se retorna.

### 1.3 Interpreter

- **Responsabilidad:** Recorre el AST y ejecuta el programa, implementando los patrones de diseño Visitor para separar lógica de ejecución de la estructura del AST.
- **Justificación:** El patrón Visitor desacopla la lógica de ejecución de la estructura de datos, facilitando extensiones futuras.
- **Accesibilidad:** La clase `Interpreter` es pública, pero mantiene tablas de símbolos y métodos auxiliares privados.
- **Manejo de Errores:** Gestiona errores en tiempo de ejecución y los reporta.

### 1.4 Canvas

- **Responsabilidad:** Representa la "pantalla" y el entorno donde Wall-E dibuja.
- **Justificación:** Encapsular el estado gráfico y la interacción con el robot permite separar la lógica de interpretación de la representación visual.
- **Accesibilidad:** El Canvas es público, pero su estado interno (`pixels`, `Wall_E`) se mantiene privado o protegido.

---

## 2. Clases auxiliares y diseño técnico

### 2.1 Tokens

- **Motivo de existencia:** Los tokens encapsulan la información mínima necesaria para que el parser comprenda la estructura del programa (tipo, lexema, línea, columna).
- **Justificación:** Facilita el manejo de errores y el mapeo del código fuente a estructuras de alto nivel.

### 2.2 Expr y Stmt

- **Separación Expr/Stmt:** 
  - **Expr** (Expresiones): Representan valores y operaciones que retornan un resultado, p.ej. operaciones matemáticas, consultas al entorno (`GetActualXExpr`).
  - **Stmt** (Sentencias): Representan acciones que alteran el estado pero no retornan valores, p.ej. dibujar, mover, asignar.
- **Justificación:** Esta división sigue el diseño tradicional de lenguajes interpretados y permite una clara distinción entre acciones y cálculos, lo que simplifica el parser, el intérprete y la extensibilidad del lenguaje.

### 2.3 Nodos del AST

Cada nodo (por ejemplo, `DrawRectangleStmt`, `SpawnStmt`, `GetColorCountExpr`) es una clase derivada de `Stmt` o `Expr`, con atributos que capturan los argumentos necesarios.

- **Ventajas:**
  - Facilita la implementación del patrón Visitor.
  - Permite agregar nuevos comandos o expresiones simplemente añadiendo nuevos nodos derivados.
  - Mejora la mantenibilidad y el testeo.

#### Ejemplo: Nodo DrawRectangleStmt

```csharp
public class DrawRectangleStmt : Stmt
{
    public DrawRectangleStmt(Token keyword, Expr dirX, Expr dirY, Expr distance, Expr width, Expr height) { ... }
    // Campos públicos: Keyword, DirX, DirY, Distance, Width, Height
}
```
- **Justificación:** Los campos son públicos para facilitar el acceso desde el Visitor, pero si solo se accede internamente, podrían ser `internal` o `private` con getters.
- **Constructor:** Recibe todos los componentes sintácticos necesarios.

#### Ejemplo: Nodos Expr

Los nodos de expresión como `GetActualXExpr`, `Assign`, etc., siguen un patrón similar, permitiendo la evaluación de expresiones durante la interpretación.

---

## 3. Diseño de métodos y acceso

### 3.1 Métodos públicos y privados

- **Clases principales (`Core`, `Interpreter`, `Parser`, `Canvas`)**: Solo exponen como públicos los métodos necesarios para la operación desde fuera de la clase (`Run`, `Interpret`, `Parse`, etc.).
- **Métodos auxiliares:** Son privados o protegidos para evitar el acceso externo y preservar la encapsulación.
- **Campos de AST nodes:** Tienden a ser públicos para facilitar la manipulación durante el recorrido del árbol. Sin embargo, para mayor robustez, se podrían hacer `readonly` o exponer solo getters si no se modifican después de la construcción.

### 3.2 Justificación de accesos

- **Público:** Solo para métodos de interfaz principal y nodos de AST que necesitan ser accedidos por el Visitor.
- **Privado/protegido:** Para métodos internos de cada componente (ej. métodos de parsing, helpers de interpretación, detalles de la implementación de Canvas).

---

## 4. Canvas y clases auxiliares gráficas

### 4.1 Canvas

- **Responsabilidad:** Gestiona la cuadrícula de píxeles, el estado del robot y operaciones de consulta y modificación.
- **Métodos representativos:**
  - `GetPixels()`: Devuelve la matriz de píxeles.
  - `GetBrushColor()`, `GetPixelColor(x, y)`, `GetWallEPosX()`, `GetWallEPosY()`: Permiten consultar el estado.
- **Modificadores de acceso:** Los métodos son públicos para permitir interacción desde el intérprete. Los campos internos como la matriz de píxeles y el objeto Wall_E son privados, garantizando encapsulamiento.

### 4.2 Pixel y Wall_E

- **Pixel:** Representa el color de cada celda.
- **Wall_E:** Representa la posición, color del pincel y tamaño del robot.

---

## 5. Conclusión y recomendaciones

El diseño de la librería Wall_E Compiler es modular y sigue las mejores prácticas de compiladores sencillos:

- **Separación de responsabilidades** clara (Lexer, Parser, Interpreter, Canvas).
- **AST estructurado** con Expr y Stmt, facilitando la extensibilidad.
- **Uso de patrones:** El patrón Visitor es adecuado para la interpretación y recorrido del AST.
- **Encapsulamiento:** El Canvas y las clases auxiliares mantienen el estado interno protegido, exponiendo solo lo necesario.

**Recomendación:** Revisar si los campos públicos de los nodos AST pueden ser más restrictivos (ej. solo getters o `internal`), especialmente si la manipulación solo ocurre en el constructor y el visitor. Esto puede mejorar la robustez ante cambios futuros.

---

## 6. Detalle y Comportamiento de las Clases Principales

A continuación se analiza en profundidad el comportamiento, métodos clave y justificación de diseño de las clases principales: `Lexer`, `Parser`, `Interpreter`, `Canvas`, y las auxiliares relevantes.

---

### 6.1 Lexer

**Responsabilidad:**  
Convierte el texto fuente en una secuencia de tokens, identificando palabras clave, identificadores, literales, operadores y delimitadores.

**Métodos principales:**
- `Lex()`: Método principal que recorre el texto fuente carácter a carácter, identifica patrones y construye la lista de tokens.
- Maneja errores léxicos (caracteres inválidos, literales no terminados, etc.), almacenando mensajes de error y posición.

**Justificación de diseño:**
- El Lexer es **local a la ejecución** (no es singleton ni global), lo que permite hilos o ejecuciones paralelas.
- Separa claramente la detección de errores léxicos antes de entrar al parser.
- Los tokens contienen metadata útil (línea, columna), imprescindible para el reporte de errores y depuración.

---

### 6.2 Parser

**Responsabilidad:**  
Transforma la secuencia de tokens en un Árbol de Sintaxis Abstracta (AST), determinando la estructura jerárquica del programa.

**Métodos principales:**
- `Parse()`: Método público que inicia el proceso de análisis sintáctico, devolviendo una lista de sentencias (`List<Stmt>`).
- Métodos privados para cada regla gramatical (`ParseStatement`, `Expression`, `Assignment`, etc.).
- `ConsumeEOLorEOF()`: Verifica que las expresiones/sentencias terminan correctamente.
- Manejo de errores sintácticos: recolecta errores y permite la recuperación para seguir parseando hasta el final.

**Justificación de diseño:**
- El parser utiliza un **mapa de aridad** para cada instrucción, lo que simplifica la validación de argumentos (ver `Parser.cs`).
- El parser implementa el patrón **recursive descent**, lo que facilita la comprensión y modificación de la gramática.
- La división entre **Expr** y **Stmt** permite expresar operaciones puras (expresiones) y efectos secundarios (sentencias).

**Comportamiento avanzado:**
- Soporte para expresiones complejas, agrupaciones, operaciones lógicas y aritméticas.
- Capacidad de parsear instrucciones personalizadas fácilmente extendiendo el mapa de aridad y agregando nuevos nodos.

---

### 6.3 Interpreter

**Responsabilidad:**  
Recorre el AST y ejecuta las acciones definidas, modificando el entorno (variables, canvas, etc.) según corresponda.

**Métodos principales:**
- `Interpret(ProgramNode)`: Método principal que recorre las sentencias del programa.
- Métodos `VisitXStmt`, `VisitXExpr`: Para cada tipo de nodo del AST, mediante el patrón **Visitor**, se implementa la lógica de ejecución.
  - Ejemplo: `VisitDrawRectangleStmt`, `VisitAssignExpr`, etc.
- Manejo de entorno: tabla de variables, tabla de etiquetas para saltos, manejo de control de flujo.
- Manejo de errores de ejecución (runtime): lanza y recoge excepciones customizadas con información rica de contexto.

**Justificación de diseño:**
- El uso del patrón **Visitor** desacopla la lógica de recorrido del AST y permite añadir nuevas operaciones sin modificar los nodos.
- La separación de entorno, instrucciones y manejo de errores facilita la extensión y el mantenimiento.
- La interpretación es **estadoful**: mantiene el estado del canvas, variables y posición de Wall-E a lo largo de la ejecución.

---

### 6.4 Canvas

**Responsabilidad:**  
Modela la cuadrícula de píxeles y el estado del robot Wall-E, permitiendo operaciones de dibujo, consulta y modificación.

**Atributos destacados:**
- `Pixel[,] pixels`: Matriz de píxeles, cada uno con su color.
- `Wall_E Wall_E`: Instancia que almacena la posición y estado del robot (color y tamaño de brocha).
- `int Size`: Tamaño del canvas.

**Métodos principales:**
- **Consultas:**  
  - `GetPixels()`, `GetBrushColor()`, `GetPixelColor(x, y)`, `GetWallEPosX()`, `GetWallEPosY()`, `GetBrushSize()`
- **Acciones:**  
  - `SpawnWallE(x, y)`: Coloca a Wall-E en una posición.
  - `DrawLine(dirX, dirY, distance)`, `DrawCircle(dirX, dirY, radius)`, `DrawRectangle(dirX, dirY, dist, w, h)`: Dibujo de figuras.
  - `Fill()`: Rellena el canvas desde la posición actual con el color de la brocha.
  - `SetColor(color)`, `SetSize(size)`: Cambia el estado de la brocha de Wall-E.
  - **Auxiliares:** Métodos privados para pintar áreas, validar posiciones, etc.

**Justificación de diseño:**
- La separación entre `Canvas` y `Wall_E` permite distinguir entre el estado global (canvas) y el agente (robot) que actúa sobre él.
- Los métodos públicos exponen solo lo necesario para la interpretación y visualización, manteniendo los detalles internos ocultos.
- El diseño soporta **operaciones atómicas y compuestas** sobre el canvas, permitiendo futuras extensiones (nuevas figuras, efectos, etc.).

---

### 6.5 Clases auxiliares: Pixel y Wall_E

**Pixel:**
- Encapsula el color de un pixel.  
- Es simple por diseño, para eficiencia y claridad.

**Wall_E:**
- Mantiene el estado del robot: posición actual (`PosX`, `PosY`), color y tamaño de la brocha.
- Expone métodos públicos para modificar el estado (`MoveTo`, `SetBrushColor`, `SetBrushSize`), pero restringe la modificación directa usando `private set` en las propiedades.

**Justificación de diseño:**
- Permite un modelo claro y controlado del agente y su interacción con el canvas.
- Facilita la extensión para agregar más atributos o funcionalidades al robot si fuera necesario.

---

## 7. Observaciones sobre extensibilidad y mantenimiento

- **Extensibilidad:**  
  El diseño modular facilita agregar nuevos comandos, expresiones o figuras. Solo requiere añadir nuevos nodos, métodos Visitor y, en su caso, lógica en el Canvas.
- **Mantenibilidad:**  
  La clara separación de responsabilidades y el uso de patrones clásicos de compiladores (Lexer, Parser, AST, Visitor) facilitan la depuración y el testeo.
---

## 8. Resumen

Cada clase principal está cuidadosamente estructurada para cumplir con su responsabilidad específica dentro del ciclo de vida del lenguaje. El diseño sigue patrones estándar de la literatura de lenguajes de programación, lo cual asegura claridad, extensibilidad y mantenibilidad.

---