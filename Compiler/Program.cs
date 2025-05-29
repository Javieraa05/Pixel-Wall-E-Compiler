using System;

class Program
{
    static void Main(string[] args)
    {
        //ProbarInterprete();
        string source = @"Spawn(4,5)
                          GetActualX()
                          GetActualY()
                          GetCanvasSize()
                          GetColorCount(1,2,3,4,5)
                          IsBrushColor(1)
                          IsBrushSize(1)
                          IsCanvasColor(1,2,3)
";   

        // Paso 1: Analizar léxicamente el código fuente
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.Lex();   
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
            Console.WriteLine();
        }

        // Paso 2: Analizar sintácticamente los tokens
        Parser parser = new Parser(tokens);
        List<Stmt> stmts = parser.Parse();

        ProgramNode program = new ProgramNode();
        program.Statements.AddRange(stmts);

        // Paso 3: Imprimir el árbol sintáctico (AST)
        AstTreePrinter printer = new AstTreePrinter();
        Console.WriteLine(printer.Print(program));
        

        /*// Paso 4: Interpretar el AST
        Interpreter interpreter = new Interpreter();
        interpreter.Interpret(expression);
        // Salida: 14

        // Paso 5: Analisis de errores*/
        
    }

}