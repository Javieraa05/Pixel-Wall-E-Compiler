
using System;

namespace Wall_E.Compiler
{
    public enum ErrorKind
    {
        Lexical,
        Syntactic,
        Runtime
    }

    public class ErrorInfo
    {
        public ErrorKind Kind { get; }
        public int Line { get; }
        public int Column { get; }
        public string Message { get; }

        public ErrorInfo(ErrorKind kind, int line, int column, string message)
        {
            Kind = kind;
            Line = line;
            Column = column;
            Message = message;
        }

        public override string ToString()
        {
            return $"[Error] Línea {Line}, Col {Column}: {Message}";
        }
    }
}
