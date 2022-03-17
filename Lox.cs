﻿using System;
using System.Collections.Generic;
using System.IO;
public class Lox {
    private static Interpreter interpreter = new Interpreter();
    private static bool hadError = false;
    private static bool hadRuntimeError = false;
    public static void Main(string[] args) {
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
        if(hadRuntimeError) Environment.Exit(70);
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

        Parser parser = new Parser(tokens);
        Expr expression = parser.Parse();

        if(hadError) return;

        interpreter.Interpret(expression);
    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    public static void RuntimeError(RuntimeException e) {
        Console.WriteLine(e.Message + "\n[line " + e.token.line + "]");
        hadRuntimeError = true;
    }

    public static void Error(Token token, string message) {
        if(token.type == TokenType.EOF) {
            Report(token.line, " at end", message);
        } else {
            Report(token.line, " at '" + token.lexeme + "'", message);
        }
    }

    private static void Report(int line, string where, string message) {
        Console.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}
