using System;
using System.Collections.Generic;

namespace Wall_E.Compiler
{
    public class Core
    {
        /// <summary>
        /// Ejecuta todo el pipeline (Lexer → Parser → Interpreter) sobre el código dado.
        /// Devuelve RunResult con: la matriz de píxeles, lista de errores y lista de instrucciones.
        /// </summary>
        public RunResult Run(string source, int sizeCanvas = 32)
        {
            var errores = new List<ErrorInfo>();
            List<Token> tokens = null;
            ProgramNode program = null;
            string AST = "";

            // 1) LEXER
           
                var lexer = new Lexer(source);
                tokens = lexer.Lex();

            if (lexer.HadError)
            {
                // Si el lexer tiene errores, los recogemos
                foreach (var lexErr in lexer.LexicalErrors)
                {
                    errores.Add(new ErrorInfo(
                        ErrorKind.Lexical,
                        line: lexErr.Line,
                        column: lexErr.Column,
                        message: lexErr.Message
                    ));
                }

                return new RunResult(
                canvas: new Canvas(sizeCanvas),
                errors: errores,
                instructions: new List<Instruction>(),
                ast: ""
                );
            }
            

            AST += "Tokens: \n";
            foreach (var token in tokens)
            {
                AST += $"Type: {token.Type}   ";
                AST += $"Lexeme: {token.Lexeme}   ";
                AST += $"Line: {token.Line}   ";
                AST += $"Column: {token.Column} \n   ";

            }

            // 2) PARSER
            List<Stmt> statements = null;
            var parser = new Parser(tokens);
            statements = parser.Parse();
            if(parser.hadError)
            {
                // Si el parser tiene errores, los recogemos
                foreach (var parseErr in parser._parseErrors)
                {
                    errores.Add(new ErrorInfo(
                        ErrorKind.Syntactic,
                        line: parseErr.Line,
                        column: parseErr.Column,
                        message: parseErr.Message
                    ));
                }
                
                return new RunResult(
                    canvas: new Canvas(sizeCanvas),
                    errors: errores,
                    instructions: new List<Instruction>(),
                    ast: AST
                );
            }
            
            program = new ProgramNode();
            program.Statements.AddRange(statements);
            AstTreePrinter astTreePrinter = new AstTreePrinter();
            AST += "\n" + astTreePrinter.Print(program); 


            // 3) INTERPRETACIÓN
            var interpreter = new Interpreter(sizeCanvas);
            interpreter.ClearErrors();
            interpreter.ClearInstructions();
           
            interpreter.Interpret(program);

            // 4) RECOGEMOS ERRORES DE RUNTIME
            if (interpreter.hadRuntimeError)
            {
                foreach (var runtimeError in interpreter.RuntimeErrors)
                {
                    errores.Add(new ErrorInfo(
                        ErrorKind.Runtime,
                        line: runtimeError.Line,
                        column: runtimeError.Column,
                        message: runtimeError.Message
                    ));
                }
                return new RunResult(
                    canvas: new Canvas(sizeCanvas),
                    errors: errores,
                    instructions: new List<Instruction>(),
                    ast: AST
                );
            }
            

            // 5) Construimos la matriz de píxeles y la lista de instrucciones
            var canvas = interpreter.Canvas;
            var instrucciones = interpreter.Instructions;

            return new RunResult(
            canvas: canvas,
            errors: errores,
            instructions: instrucciones,
            AST);
        }
    }
}
