using Microsoft.VisualBasic;

namespace CSLox;

class ParserError : Exception {}

class Parser(List<Token> tokens)
{
  List<Token> Tokens { get; set ;} = tokens;
  int Current = 0;

  public List<Stmt> Parse()
  {
    List<Stmt> statements = [];
    while (!IsAtEnd())
    {
      statements.Add(Declaration());
    }

    return statements;
  }

  Token Peek() => Tokens[Current];
  Token Previous() => Tokens[Current - 1];
  bool IsAtEnd() => Peek().Type == TokenType.EOF;

  static ParserError Error(Token token, string message)
  {
    Lox.Error(token, message);

    return new ParserError();
  }

  // After a moderately serious error is thrown, this method
  // tries to skip ahead to the start of the next statement
  // so we can continue parsing. (There might be an error but
  // we don't want to report errors to the user one at a time)
  void Synchronize()
  {
    Advance();

    while (!IsAtEnd())
    {
      if (Previous().Type == TokenType.SEMICOLON)
        return;

      switch (Peek().Type)
      {
        case TokenType.CLASS:
        case TokenType.FOR:
        case TokenType.FUN:
        case TokenType.IF:
        case TokenType.PRINT:
        case TokenType.RETURN:
        case TokenType.VAR:
        case TokenType.WHILE:
          return;
      }

      Advance();
    }
  }

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

  Token Consume(TokenType type, string message)
  {
    if (Check(type))
      return Advance();

    throw Error(Peek(), message);
  }

  Stmt? Declaration()
  {
    try
    {
      if (Match(TokenType.VAR))
        return VarDeclaration();

      return Statement();
    }
    catch (ParserError error)
    {
      Synchronize();
      return null;
    }
  }

  Stmt Statement()
  {
    if (Match(TokenType.IF))
      return IfStatement();
    if (Match(TokenType.PRINT))
      return PrintStatement();
    if (Match(TokenType.WHILE))
      return WhileStatement();
    if (Match(TokenType.LEFT_BRACE))
      return new BlockStmt(Block());

    return ExpressionStatement();
  }

  Stmt IfStatement()
  {
    Consume(TokenType.LEFT_PAREN, "Expected '(' after 'if' statement.");
    Expr condition = Expression();
    Consume(TokenType.RIGHT_PAREN, "Expected ')' after 'if' condition.");

    Stmt thenBranch = Statement();
    Stmt? elseBranch = null;
    if (Match(TokenType.ELSE))
    {
      elseBranch = Statement();
    }

    return new IfStatement(condition, thenBranch, elseBranch);
  }

  Stmt PrintStatement()
  {
    Expr value = Expression();
    Consume(TokenType.SEMICOLON, "Expected ';' after value.");

    return new PrintStmt(value);
  }

  Stmt VarDeclaration()
  {
    Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

    Expr? initializer = null;
    if (Match(TokenType.EQUAL))
    {
      initializer = Expression();
    }

    Consume(TokenType.SEMICOLON, "Expected ';' after variable declaration");

    return new VarStmt(name, initializer);
  }

  Stmt ExpressionStatement()
  {
    Expr expr = Expression();
    Consume(TokenType.SEMICOLON, "Expected ';' after value.");

    return new ExprStmt(expr);
  }

  Stmt WhileStatement()
  {
    Consume(TokenType.LEFT_PAREN, "Expected '(' after 'while'.");
    Expr condition = Expression();
    Consume(TokenType.RIGHT_PAREN, "Expected ')' after condition.");
    Stmt body = Statement();

    return new WhileStmt(condition, body);
  }

  List<Stmt> Block()
  {
    List<Stmt> statements = [];

    while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) 
    {
      var stmt = Declaration();
      if (stmt is not null)
        statements.Add(stmt);
    }

    Consume(TokenType.RIGHT_BRACE, "Expected '}' after block.");

    return statements;
  }

  Expr Expression() => Assignment();

  Expr Assignment()
  {
    Expr expr = Ternary();

    if (Match(TokenType.EQUAL))
    {
      Token equals = Previous();
      Expr value = Assignment();

      if (expr is Variable var)
      {
        Token name = var.Name;
        return new Assign(name, value);
      }

      Error(equals, "Invalid assignment target");
    }

    return expr;
  }

  Expr Ternary()
  {
    Expr expr = Or();

    while (Match(TokenType.QUESTION_MARK))
    {
      Expr pass = Expression();
      Consume(TokenType.COLON, "Expected ':' in ternary expression.");
      Expr fail = Expression();
      expr = new Ternary(expr, pass, fail);
    }

    return expr;
  }

  Expr Or()
  {
    Expr expr = And();

    while (Match(TokenType.OR))
    {
      Token op = Previous();
      Expr right = And();
      expr = new Logical(expr, op, right);
    }

    return expr;
  }

  Expr And()
  {
    Expr expr = Equality();

    while (Match(TokenType.AND))
    {
      Token op = Previous();
      Expr right = Equality();
      expr = new Logical(expr, op, right);
    }

    return expr;
  }

  Expr Equality()
  {
    Expr expr = Comparison();

    while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
    {
      Token op = Previous();
      Expr right = Comparison();
      expr = new Binary(expr, op, right);
    }

    return expr;
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

  // This handles addition or subtraction
  Expr Term()
  {
    Expr expr = Factor();

    while (Match(TokenType.MINUS, TokenType.PLUS))
    {
      Token op = Previous();
      Expr right = Factor();
      expr = new Binary(expr, op, right);
    }

    return expr;
  }

  // For multiplication or division
  Expr Factor()
  {
    Expr expr = Unary();

    while (Match(TokenType.SLASH, TokenType.STAR))
    {
      Token op = Previous();
      Expr right = Unary();
      expr = new Binary(expr, op, right); 
    }

    return expr;
  }
  
  Expr Unary()
  {
    if (Match(TokenType.BANG, TokenType.MINUS))
    {
      Token op = Previous();
      Expr right = Unary();
      return new Unary(op, right);
    }

    return Primary();
  }

  Expr Primary()
  {
    if (Match(TokenType.FALSE))
      return new Literal(false);
    if (Match(TokenType.TRUE))
      return new Literal(true);
    if (Match(TokenType.NIL))
      return new Literal(null);

    if (Match(TokenType.NUMBER, TokenType.STRING))
      return new Literal(Previous().Literal);

    if (Match(TokenType.IDENTIFIER))
      return new Variable(Previous());

    if (Match(TokenType.LEFT_PAREN))
    {
      Expr expr = Expression();
      Consume(TokenType.RIGHT_PAREN, "Expected ')' after expression.");
      return new Grouping(expr);
    }

    throw Error(Peek(), "Expected expression.");
  }
}