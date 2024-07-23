namespace CSLox;

public interface IExprVisitor<T>
{
	public T VisitBinaryExpr(Binary expr);
	public T VisitGroupingExpr(Grouping expr);
	public T VisitLiteralExpr(Literal expr);
	public T VisitUnaryExpr(Unary expr);  
  public T VisitTernaryExpr(Ternary expr);
	public T VisitVariableExpr(Variable expr);
}

public abstract class Expr 
{
	public abstract T Accept<T>(IExprVisitor<T> visitor);
}

public class Ternary(Expr test, Expr pass, Expr fail) : Expr
{
  public Expr Test { get; set; } = test;
  public Expr Pass { get; set; } = pass;
  public Expr Fail { get; set; } = fail;

  public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitTernaryExpr(this);
}

public class Binary(Expr left, Token op, Expr right) : Expr
{
	public Expr Left = left;
	public Token Op = op;
	public Expr Right = right;

  public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
}

public class Grouping(Expr expression) : Expr
{
	public Expr Expression = expression;

  public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
}

public class Literal(object value) : Expr
{
	public object Value = value;

  public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
}

public class Unary(Token op, Expr right) : Expr
{
	public Token Op = op;
	public Expr Right = right;

  public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
}

public class Variable(Token name) : Expr
{
	public Token Name = name;

	public override T Accept<T>(IExprVisitor <T> visitor) => visitor.VisitVariableExpr(this);	
}

