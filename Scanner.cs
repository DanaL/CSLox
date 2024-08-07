using System.Text;

namespace CSLox;

public class Scanner(string src)
{
  private readonly string source = src;
  private readonly List<Token> tokens = [];
  private int start;
  private int current;
  private int line = 1;

  static readonly Dictionary<string, TokenType> keywords = new()
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "fn", TokenType.FUN },
        { "for", TokenType.FOR },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      start = current;
      ScanToken();
    }

    return tokens;
  }

  private void ScanToken()
  {
    char c = Advance();
    switch (c)
    {
      case '(':
        AddToken(TokenType.LEFT_PAREN);
        break;
      case ')':
        AddToken(TokenType.RIGHT_PAREN);
        break;
      case '{':
        AddToken(TokenType.LEFT_BRACE);
        break;
      case '}':
        AddToken(TokenType.RIGHT_BRACE);
        break;
      case ',':
        AddToken(TokenType.COMMA);
        break;
      case '.':
        AddToken(TokenType.DOT);
        break;
      case '-':
        AddToken(TokenType.MINUS);
        break;
      case '+':
        AddToken(TokenType.PLUS);
        break;
      case ';':
        AddToken(TokenType.SEMICOLON);
        break;
      case '*':
        AddToken(TokenType.STAR);
        break;
      case '?':
        AddToken(TokenType.QUESTION_MARK);
        break;
      case ':':
        AddToken(TokenType.COLON);
        break;
      case '!':
        AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
        break;
      case '=':
        AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
        break;
      case '<':
        AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
        break;
      case '>':
        AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
        break;
      case '/':
        if (Match('/'))
        {
          while (Peek() != '\n' && !IsAtEnd())
            Advance();
        }
        else
        {
          AddToken(TokenType.SLASH);
        }

        break;
      case ' ':
      case '\r':
      case '\t':
        break;
      case '\n':
        line++;
        break;
      case '"':
        String();
        break;
      default:
        if (IsDigit(c))
          Number();
        else if (IsAlpha(c))
          Identifier();
        else
          Lox.Error(line, $"Unexpected character: {c}.");

        break;
    }
  }

  private void Identifier()
  {
    while (IsAlphaNumeric(Peek()))
      Advance();

    string text = source[start..current];
    TokenType type = keywords.TryGetValue(text, out TokenType value) ? value : TokenType.IDENTIFIER;
    AddToken(type);
  }

  private static bool IsAlpha(char c) => char.IsLetter(c) || c == '_';
  private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
  private static bool IsDigit(char c) => c >= '0' && c <= '9';

  private void ConsumeDigits()
  {
    while (true)
    {
      char ch = Peek();
      if (IsDigit(ch) || (ch == '_' && IsDigit(Peeek())))
        Advance();
      else
        break;
    }
  }

  private void Number()
  {
    ConsumeDigits();

    if (Peek() == '.')
    {
      if (!IsDigit(Peeek()))
      {
        Lox.Error(line, "Invalid numeric constant.");
        return;
      }

      Advance();
      ConsumeDigits();
    }

    var sb = new StringBuilder();
    for (int j = start; j < current; j++)
    {
      if (source[j] != '_')
        sb.Append(source[j]);
    }
    
    AddToken(TokenType.NUMBER, double.Parse(sb.ToString()));
  }

  private void String()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\n')
        line++;
      Advance();
    }

    if (IsAtEnd())
    {
      Lox.Error(line, "Unterminated string.");
      return;
    }

    // skip closing "
    Advance();

    string? value = source.Substring(start + 1, current - start - 2);
    AddToken(TokenType.STRING, value);
  }

  private char Peek() => IsAtEnd() ? '\0' : source[current];

  private char Peeek()
  {
    if (current + 1 >= source.Length)
      return '\0';

    return source[current + 1];
  }

  private bool Match(char expected)
  {
    if (IsAtEnd())
      return false;
    if (source[current] != expected)
      return false;

    current++;

    return true;
  }

  private char Advance() => source[current++];

  private void AddToken(TokenType type, object? literal = null)
  {
    string text = source[start..current];
    tokens.Add(new(type, text, literal, line));
  }

  private bool IsAtEnd() => current >= source.Length;
}