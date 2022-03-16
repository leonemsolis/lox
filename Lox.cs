public class Lox {
    static bool hadError = false;
    public static void Main(string[] args) {
        if(args.Length > 1) {
            Console.WriteLine("Usage: jlox [script]");
            Environment.Exit(64);
        } else if(args.Length == 1) {
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
