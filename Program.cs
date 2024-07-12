using System.Text;
using System.Text.RegularExpressions;

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

        foreach (var token in tokens)
        {
          Console.WriteLine(token);
          hadError = false;
        }
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
      Expr expression = new Binary(
        new Unary(new Token(TokenType.MINUS, "-", null, 1),
          new Literal(123)),
        new Token(TokenType.STAR, "*", null, 1),
        new Grouping(new Literal(45.67))
      );

      Console.WriteLine(new AstPrinter().Print(expression));
      Console.WriteLine(new AstPrinterRPN().Print(expression));

      Expr exp2 = new Binary(
        new Grouping(
          new Binary(new Literal(1), new Token(TokenType.STAR, "+", null, 1), new Literal(2))
        ),
        new Token(TokenType.STAR, "*", null, 1),
        new Grouping(
          new Binary(new Literal(4), new Token(TokenType.STAR, "-", null, 1), new Literal(3))
        )
      );
      Console.WriteLine(new AstPrinter().Print(exp2));
      Console.WriteLine(new AstPrinterRPN().Print(exp2));
        // switch (args.Length)
        // {
        //   case > 1:
        //       Console.WriteLine("Usage: cslox [script]");
        //       return;
        //   case 1:
        //       Console.WriteLine(args[0]);
        //       break;
        //   default:
        //       RunPrompt();
        //       break;
        // }
    }
}