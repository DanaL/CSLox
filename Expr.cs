namespace CSLox;

public abstract class Expr {}

public class Binary : Expr
{
	Expr Left;
	Token Op;
	Expr Right;

	public Binary(Expr left, Token op, Expr right)
	{
		Left = left;
		Op = op;
		Right = right;
	}
}

public class Grouping : Expr
{
	Expr Expression;

	public Grouping(Expr expression)
	{
		Expression = expression;
	}
}

public class Literal : Expr
{
	Object Value;

	public Literal(Object value)
	{
		Value = value;
	}
}

public class Unary : Expr
{
	Token Op;
	Expr Right;

	public Unary(Token op, Expr right)
	{
		Op = op;
		Right = right;
	}
}

