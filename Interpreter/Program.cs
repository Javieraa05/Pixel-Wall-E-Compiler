using System;

class Program
{
    static void Main(string[] args)
    {
        //ProbarLexer();
        //ProbarAST();
        ProbarParser();
    }

    static void ProbarLexer()
    {
        string source = @"  Spawn(0, 0) 
                            Color(Black)
                            n <- 5
                            k <- 3 + 3 * 10
                            n <- k * 2
                            actual-x <- GetActualX()
                            i <- 0

                            loop-1
                            DrawLine(1, 0, 1)
                            i <- i + 1
                            is-brush-color-blue <- IsBrushColor(""Blue"")
                            Goto [loop-ends-here] (is-brush-color-blue == 1)
                            GoTo [loop1] (i < 10)

                            Color(""Blue"")
                            GoTo [loop1] (1 == 1)

                            loop-ends-here";

        Lexer lexer = new Lexer(source);

        List<Token> tokens = lexer.Lex();

        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
            Console.WriteLine();
        }
    }

    static void ProbarAST()
    {
        // Construir un programa con una sola expresión: (1 + 2) * 3
        var prog = new ProgramNode();
        Expr expr = new Binary(
            new Grouping(
                new Binary(new Literal(1), new Token(TokenType.Plus,"+",0,0), new Literal(2))),
            new Token(TokenType.Star, "*", 0, 0),
            new Literal(3)
        );
        prog.Statements.Add(new ExpressionStatement(expr));

        var printer = new AstPrinter();
        Console.WriteLine(printer.Print(prog));
        // Salida: (program (* (group (+ 1 2)) 3))
    }

    static void ProbarParser()
    {
        string source = @"n <- 3 + 4 * 2";

        // Paso 1: Analizar léxicamente el código fuente
        Lexer lexer = new Lexer(source);
        List<Token> tokens = lexer.Lex();

        // Paso 2: Analizar sintácticamente los tokens
        Parser parser = new Parser(tokens);
        Expr expression = parser.Parse();

        // Paso 3: Imprimir el árbol sintáctico (AST)
        AstPrinter printer = new AstPrinter();
        string output = printer.Print(expression);

        Console.WriteLine("Árbol de sintaxis abstracta:");
        Console.WriteLine(output);
    }



}
