namespace CSLox;

public interface IVisitor
{
	public T VisitBinaryExpr<T>(Binary expr);
	public T VisitGroupingExpr<T>(Grouping expr);
	public T VisitLiteralExpr<T>(Literal expr);
	public T VisitUnaryExpr<T>(Unary expr);
}

public abstract class Expr 
{
	public abstract T Accept<T>(IVisitor visitor);

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

	public override T Accept<T>(IVisitor visitor)
	{
		return visitor.VisitBinaryExpr<T>(this);
	}

}

public class Grouping : Expr
{
	public Expr Expression;

	public Grouping(Expr expression)
	{
		Expression = expression;
	}

	public override T Accept<T>(IVisitor visitor)
	{
		return visitor.VisitGroupingExpr<T>(this);
	}

}

public class Literal : Expr
{
	public Object Value;

	public Literal(Object value)
	{
		Value = value;
	}

	public override T Accept<T>(IVisitor visitor)
	{
		return visitor.VisitLiteralExpr<T>(this);
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

	public override T Accept<T>(IVisitor visitor)
	{
		return visitor.VisitUnaryExpr<T>(this);
	}

}

