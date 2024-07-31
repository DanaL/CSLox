namespace CSLox;

class Interpreter : IExprVisitor<object?>, IStmtVisitor
{
  public bool EchoExpressionResult { get; set; } = false;

  LoxEnvironment Environment { get; set; } = new LoxEnvironment();

  object? Evaluate(Expr expr) => expr.Accept(this);
  
  static string Stringify(object? obj)
  {
    if (obj is null)
      return "nil";

    if (obj is double)
    {
      string txt = obj.ToString();
      if (txt.EndsWith(".0"))
        txt = txt[..^2];
      return txt;
    }

    return obj.ToString();
  }

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

  static void CheckNumericOperand(Token op, object? operand)
  {
    if (operand is double)
      return;

    throw new RuntimeError(op, "Operand must be a number.");
  }

  static void CheckNumericOperands(Token op, object? left, object? right)
  {
    if (left is double && right is double)
      return;

    throw new RuntimeError(op, "Operands must be numbers.");
  }
  
  public object? VisitBinaryExpr(Binary expr)
  {
    object? left = Evaluate(expr.Left);
    object? right = Evaluate(expr.Right);

    switch (expr.Op.Type)
    {
      case TokenType.MINUS:
        CheckNumericOperands(expr.Op, left, right);
        return (double)left - (double)right;
      case TokenType.SLASH:
        CheckNumericOperands(expr.Op, left, right);
        double denominator = (double)right;
        if (Math.Abs(0.0 - denominator) < 0.0000000000001)
          throw new RuntimeError(expr.Op, "Division by zero.");
        return (double)left / denominator;
      case TokenType.STAR:
        CheckNumericOperands(expr.Op, left, right);
        return (double)left * (double)right;
      case TokenType.PLUS:
        if (left is double lval && right is double rval)
          return lval + rval;

        if (left is string && right is string)
          return $"{left}{right}";
        throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.");
      case TokenType.GREATER:
        CheckNumericOperands(expr.Op, left, right);
        return (double)left > (double)right;
      case TokenType.GREATER_EQUAL:
        CheckNumericOperands(expr.Op, left, right);
        return (double)left >= (double)right;
      case TokenType.LESS:
        CheckNumericOperands(expr.Op, left, right);
        return (double)left < (double)right;
      case TokenType.LESS_EQUAL:
        CheckNumericOperands(expr.Op, left, right);
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

    throw new RuntimeError(null, "Expected boolean test.");
  }

  public object? VisitUnaryExpr(Unary expr)
  {
    object? right = Evaluate(expr.Right) ?? 
      throw new RuntimeError(expr.Op, "Expected operand.");

    switch (expr.Op.Type)
    {
      case TokenType.MINUS:
        CheckNumericOperand(expr.Op, right);
        return -(double)right;
      case TokenType.BANG:
        return IsTruthy(right);
    }

    return null;
  }

  public object? VisitVariableExpr(Variable expr) => Environment.Get(expr.Name);

  public object? VisitAssignExpr(Assign expr)
  {
    object? value = Evaluate(expr.Value);
    Environment.Assign(expr.Name, value);

    return value;
  }

  public void VisitExprStmt(ExprStmt stmt)
  {
    var result = Evaluate(stmt.Expression);
    if (EchoExpressionResult)
      Console.WriteLine(Stringify(result));    
  }

  public void VisitPrintStmt(PrintStmt stmt)
  {
    object? result = Evaluate(stmt.Expression);
    Console.WriteLine(Stringify(result));
  }

  public void VisitVarStmt(VarStmt stmt)
  {
    object? value = null;
    if (stmt.Initializer != null)
    {
      value = Evaluate(stmt.Initializer);
    }

    if (stmt.Initializer is null)
      Environment.Define(stmt.Name.Lexeme);
    else
      Environment.Define(stmt.Name.Lexeme, value);
  }

  void Execute(Stmt stmt)
  {
    stmt.Accept(this);
  }

  void ExecuteBlock(List<Stmt> statements, LoxEnvironment env)
  {
    LoxEnvironment previous = Environment;

    try
    {
      Environment = env;

      foreach (var stmt in statements)
      {
        Execute(stmt);
      }
    }
    finally
    {
      Environment = previous;
    }
  }

  public void VisitBlockStmt(BlockStmt stmt)
  {
    ExecuteBlock(stmt.Statements, new LoxEnvironment(Environment));
  }

  public void VisitWhileStmt(WhileStmt stmt)
  {
    while (IsTruthy(Evaluate(stmt.Condition)))
    {
      Execute(stmt.Body);
    }
  }

  public void Interpret(List<Stmt> statements)
  {
    try
    {
      foreach (var statement in statements)
      {
        Execute(statement);
      }
    }
    catch (RuntimeError error)
    {
      Lox.ReportRuntimeError(error);
    }
  }
}