using System;
using System.Collections.Generic;
using System.IO;
public class Lox {
    static bool hadError = false;
    public static void Main(string[] args) {
        Expr expression = new Expr.Binary(
            new Expr.Unary(
                new Token(TokenType.MINUS, "-", null, 1),
                new Expr.Literal(123)),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)));
        
        expression = new Expr.Binary(
            new Expr.Binary(
                new Expr.Literal(1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Expr.Literal(2)
            ),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Binary(
                new Expr.Literal(4),
                new Token(TokenType.MINUS, "-", null, 1),
                new Expr.Literal(3)
            )
        );
        
        Console.WriteLine(new RPNPrintner().Print(expression));
        return;
        if(args.Length > 2) {
            Console.WriteLine("Usage: jlox [script]");
            Console.WriteLine("       jlox generate <output directory>");
            Environment.Exit(64);
        }  else if(args.Length == 2)  {
            if(args[0] == "generate") {
                var outputDir = args[1];
                GenerateAst.DefineAst(outputDir, "Expr", new List<string>() {
                    "Binary     : Expr left, Token op, Expr right",
                    "Grouping   : Expr expression",
                    "Literal    : object value",
                    "Unary      : Token op, Expr right"
                });
            } else {
                Console.WriteLine("Usage: jlox [script]");
                Console.WriteLine("       jlox generate <output directory>");
                Environment.Exit(64);
            }
        }
        else if(args.Length == 1) {
            RunFile(args[0]);
        } else {
            RunPrompt();
        }
    }

    private static void RunFile(string path) {
        Run(File.ReadAllText(path));

        if(hadError) Environment.Exit(65);
    }

    private static void RunPrompt() {
        while(true) {
            Console.Write("> ");
            var line = Console.ReadLine();
            if(string.IsNullOrEmpty(line)) break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source) {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach(var token in tokens) {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message) {
        Console.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}
