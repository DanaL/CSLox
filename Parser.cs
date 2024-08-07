namespace CSLox;

class ParserError : Exception {}

class Parser(List<Token> tokens)
{
  static readonly int MAX_ARGUMENTS = 255;

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
      if (Match(TokenType.FUN))
        return FunDeclaration("function");

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
    if (Match(TokenType.FOR))
      return ForStatement();
    if (Match(TokenType.IF))
      return IfStatement();
    if (Match(TokenType.PRINT))
      return PrintStatement();
    if (Match(TokenType.RETURN))
      return ReturnStatement();
    if (Match(TokenType.WHILE))
      return WhileStatement();
    if (Match(TokenType.LEFT_BRACE))
      return new BlockStmt(Block());

    return ExpressionStatement();
  }

  Stmt ForStatement()
  {
    Consume(TokenType.LEFT_PAREN, "Expected '(' after 'for'.");

    Stmt? initializer;
    if (Match(TokenType.SEMICOLON))
      initializer = null;
    else if (Match(TokenType.VAR))
      initializer = VarDeclaration();
    else
      initializer = ExpressionStatement();

    Expr? condition = null;
    if (!Check(TokenType.SEMICOLON))
      condition = Expression();
    Consume(TokenType.SEMICOLON, "Expected ';' after loop condition.");

    Expr? increment = null;
    if (!Check(TokenType.RIGHT_PAREN))
      increment = Expression();
    Consume(TokenType.RIGHT_PAREN, "Expected ')' after for clause.");

    Stmt body = Statement();

    if (increment is not null)
    {
      body = new BlockStmt([body, new ExprStmt(increment)]);
    }

    condition ??= new Literal(true);
    
    body = new WhileStmt(condition, body);

    if (initializer is not null)
      body = new BlockStmt([initializer, body]);

    return body;
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

  Stmt ReturnStatement()
  {
    Token keyword = Previous();
    Expr? val = null;
    if (!Check(TokenType.SEMICOLON))
    {
      val = Expression();
    }

    Consume(TokenType.SEMICOLON, "Expected ';' after return value.");

    return new ReturnStmt(keyword, val);
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

  Stmt FunDeclaration(string kind)
  {
    Token name = Consume(TokenType.IDENTIFIER, $"Execpted {kind} name.");
    Consume(TokenType.LEFT_PAREN, $"Expected '(' after {kind} name.");
    List<Token> parameters = [];
    if (!Check(TokenType.RIGHT_PAREN))
    {
      do
      {
        if (parameters.Count >= MAX_ARGUMENTS)
          Error(Peek(), $"Can't have more than {MAX_ARGUMENTS} parameters.");

        parameters.Add(Consume(TokenType.IDENTIFIER, "Expected parameter name"));
      }
      while (Match(TokenType.COMMA));
    }
    Consume(TokenType.RIGHT_PAREN, "Expected ')' after parameters.");

    Consume(TokenType.LEFT_BRACE, $"Expected '{{' before {kind} body.");
    List<Stmt> body = Block();

    return new Function(name, parameters, body);
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

    return Call();
  }

  Expr FinishCall(Expr callee)
  {
    List<Expr> arguments = [];

    if (!Check(TokenType.RIGHT_PAREN))
    {
      do
      {
        if (arguments.Count > MAX_ARGUMENTS)
          Error(Peek(), $"Cannot have more than {MAX_ARGUMENTS} arguments.");
        arguments.Add(Expression());
      }
      while (Match(TokenType.COMMA));
    }

    Token paren = Consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments.");

    return new Call(callee, paren, arguments);
  }

  Expr Call()
  {
    Expr expr = Primary();

    while (true)
    {
      if (Match(TokenType.LEFT_PAREN))
        expr = FinishCall(expr);
      else
        break;
    }

    return expr;
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