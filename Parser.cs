
using System.ComponentModel.Design;
using System.Reflection;

namespace CSLox;

class Parser(List<Token> tokens)
{
  List<Token> Tokens { get; set ;} = tokens;
  int Current = 0;

  Token Peek() => Tokens[Current];
  Token Previous() => Tokens[Current - 1];
  bool IsAtEnd() => Peek().Type == TokenType.EOF;

  Token Advance()
  {
    if (!IsAtEnd())
      ++Current;
    return Previous();
  }

  bool Check(TokenType type)
  {
    if (IsAtEnd())
      return false;
    return Peek().Type == type;
  }

  bool Match(params TokenType[] types)
  {
    foreach (var type in types)
    {
      if (Check(type))
      {
        Advance();

        return true;
      }
    }

    return false;
  }

  Expr Expression() => Equality();

  Expr Equality()
  {
    Expr expr = Comparison();

    while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
    {
      Token op = Previous();
      Expr right = Comparison();
      expr = new Binary(expr, op, right);
    }
  }

  Expr Comparison()
  {
    Expr expr = Term();

    while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
    {
      Token op = Previous();
      Expr right = Term();
      expr = new Binary(expr, op, right);
    }

    return expr;
  }
}