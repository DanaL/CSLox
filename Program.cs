using System.Text;

namespace CSLox
{
    class Lox
    {
        static bool _hadError = false;

        static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var contents = Encoding.Default.GetString(bytes);
            Run(contents);

            if (_hadError)
                Environment.Exit(65);
        }

        static void Run(string source)
        {
            Console.WriteLine(source);
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
                _hadError = false;
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            _hadError = true;
        }

        static void RunPrompt()
        {
            for (;;)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == null)
                    break;
                Run(line);
            }
        }

        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case > 1:
                    Console.WriteLine("Usage: cslox [script]");
                    return;
                case 1:
                    RunFile(args[0]);
                    break;
                default:
                    RunPrompt();
                    break;
            }
        }
    }
}