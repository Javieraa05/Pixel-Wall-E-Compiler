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

            // 1) LEXER
            try
            {
                var lexer = new Lexer(source);
                tokens = lexer.Lex();
            }
            catch (LexicalException lexEx)
            {
                errores.Add(new ErrorInfo(
                    ErrorKind.Lexical,
                    line: lexEx.Line,
                    column: lexEx.Column,
                    message: lexEx.Message
                ));
                return new RunResult(
                    canvas: new Canvas(sizeCanvas),
                    errors: errores,
                    instructions: new List<Instruction>()
                );
            }



            // 2) PARSER
            List<Stmt> statements = null;
            var parser = new Parser(tokens);
            statements = parser.Parse();
            if(parser.hadError)
            {
                // Si el parser tiene errores, los recogemos
                foreach (var parseErr in parser.ParseErrors)
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
                    instructions: new List<Instruction>()
                );
            }
            program = new ProgramNode();
            program.Statements.AddRange(statements);
            


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
                    instructions: new List<Instruction>()
                );
            }
            

            // 5) Construimos la matriz de píxeles y la lista de instrucciones
            var canvas = interpreter.Canvas;
            var instrucciones = interpreter.Instructions;

            return new RunResult(
            canvas: canvas,
            errors: errores,
            instructions: instrucciones);
        }
    }
}
