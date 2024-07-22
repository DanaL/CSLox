namespace CSLox;

class Lox
{
  static Interpreter interpreter = new();
  static bool hadError = false;
  static bool hadRunetimeError = false;

  static void RunFile(string path)
  {
    byte[] bytes = File.ReadAllBytes(path);

    if (hadError)
      Environment.Exit(65);
    if (hadRunetimeError)
      Environment.Exit(70);
  }

  static void Run(string source)
  {
    var scanner = new Scanner(source);
    var tokens = scanner.ScanTokens();
    tokens.Add(new Token(TokenType.EOF, "", null, 1));

    var parser = new Parser(tokens);
    Expr? expression = parser.Parse();

    if (hadError || expression is null)
    {
      return;
    }

    interpreter.Interpret(expression);

    //Console.WriteLine(new AstPrinter().Print(expression));
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

  public static void ReportRuntimeError(RuntimeError error)
  {
    string msg = error.Message;
    if (error.Token is not null)
      msg += $"\n[line {error.Token.Line}]";

    Console.Error.WriteLine(msg);
    hadRunetimeError = true;
  }

  static void RunPrompt()
  {
    while (true)
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