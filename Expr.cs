namespace CSLox;

public interface IVisitor<T>
{
	public T VisitBinaryExpr(Binary expr);
	public T VisitGroupingExpr(Grouping expr);
	public T VisitLiteralExpr(Literal expr);
	public T VisitUnaryExpr(Unary expr);
}

public abstract class Expr 
{
	public abstract T Accept<T>(IVisitor<T> visitor);

}

public class Binary : Expr
{
	public Expr Left;
	public Token Op;
	public Expr Right;

	public Binary(Expr left, Token op, Expr right)
	{
		Left = left;
		Op = op;
		Right = right;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitBinaryExpr(this);
	}

}

public class Grouping : Expr
{
	public Expr Expression;

	public Grouping(Expr expression)
	{
		Expression = expression;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitGroupingExpr(this);
	}

}

public class Literal : Expr
{
	public Object Value;

	public Literal(Object value)
	{
		Value = value;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitLiteralExpr(this);
	}

}

public class Unary : Expr
{
	public Token Op;
	public Expr Right;

	public Unary(Token op, Expr right)
	{
		Op = op;
		Right = right;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitUnaryExpr(this);
	}

}

