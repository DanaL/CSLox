namespace CSLox;

class Lox
{
  static bool hadError = false;

  static void RunFile(string path)
  {
    byte[] bytes = File.ReadAllBytes(path);

    if (hadError)
      Environment.Exit(65);
  }

  static void Run(string source)
  {
    Console.WriteLine(source);
    var scanner = new Scanner(source);

    var tokens = scanner.ScanTokens();
    tokens.Add(new Token(TokenType.EOF, "", null, 0));

    var parser = new Parser(tokens);
    Expr? expression = parser.Parse();

    if (hadError || expression is null)
    {
      return;
    }

    Console.WriteLine(new AstPrinter().Print(expression));
  }

  public static void Error(Token token, string message)
  {
    if (token.Type == TokenType.EOF)
      Report(token.Line, "at end", message);
    else
      Report(token.Line, $"at '{token.Lexeme}'", message);
  }

  public static void Error(int line, string message)
  {
    Report(line, "", message);
  }

  static void Report(int line, string where, string message)
  {
    Console.WriteLine($"[line {line}] Error {where}: {message}");
    hadError = true;
  }

  static void RunPrompt()
  {
    for (; ; )
    {
      hadError = false;
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
        Console.WriteLine(args[0]);
        break;
      default:
        RunPrompt();
        break;
    }
  }
}