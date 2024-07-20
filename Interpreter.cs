namespace CSLox;

class Interpreter : IVisitor<object?>
{
  object? Evaluate(Expr expr) => expr.Accept(this);
  
  static bool IsEqual(object? a, object? b)
  {
    if (a is null && b is null)
      return true;
    if (a == null)
      return false;

    return a.Equals(b);
  }

  static bool IsTruthy(object? obj)
  {
    if (obj is null)
      return false;
    if (obj is bool b)
      return b;

    return true;
  }

  public object? VisitBinaryExpr(Binary expr)
  {
    object? left = Evaluate(expr.Left);
    object? right = Evaluate(expr.Right);

    switch (expr.Op.Type)
    {
      case TokenType.MINUS:
        return (double)left - (double)right;
      case TokenType.SLASH:
        return (double)left / (double)right;
      case TokenType.STAR:
        return (double)left * (double)right;
      case TokenType.PLUS:
        if (left is double lval && right is double rval)
          return lval + rval;

        if (left is string && right is string)
          return $"{left}{right}";
        break;
      case TokenType.GREATER:
        return (double)left > (double)right;
      case TokenType.GREATER_EQUAL:
        return (double)left >= (double)right;
      case TokenType.LESS:
        return (double)left < (double)right;
      case TokenType.LESS_EQUAL:
        return (double)left <= (double)right;
      case TokenType.EQUAL_EQUAL:
        return IsEqual(left, right);
      case TokenType.BANG_EQUAL:
        return !IsEqual(left, right);
    }
    
    return null;
  }

  public object? VisitGroupingExpr(Grouping expr) => Evaluate(expr.Expression);
  public object? VisitLiteralExpr(Literal expr) => expr.Value;

  public object? VisitTernaryExpr(Ternary expr)
  {
    object? result = Evaluate(expr.Test);
    
    if (result is bool b)
    {
      return b ? Evaluate(expr.Pass) : Evaluate(expr.Fail);
    }
    else
    {
      // I'll need to log a runtime error here
    }

    return null;
  }

  public object? VisitUnaryExpr(Unary expr)
  {
    object? right = Evaluate(expr.Right);

    return expr.Op.Type switch
    {
      TokenType.MINUS => -(double)right,
      TokenType.BANG => IsTruthy(right),
      _ => null
    };
  }
}