namespace CSLox;

public interface IStmtVisitor
{
  public void VisitExprStmt(ExprStmt stmt);
  public void VisitPrintStmt(PrintStmt stmt);
	public void VisitVarStmt(VarStmt stmt);
	public void VisitBlockStmt(BlockStmt stmt);
	public void VisitIfStmt(IfStatement stmt);
}

public abstract class Stmt 
{
	public abstract void Accept(IStmtVisitor visitor);
}

public class ExprStmt(Expr expr) : Stmt
{
	public Expr Expression = expr;

	public override void Accept(IStmtVisitor visitor) 
	{ 
		visitor.VisitExprStmt(this);
	}
}

public class PrintStmt(Expr expr) : Stmt
{
	public Expr Expression = expr;

	public override void Accept(IStmtVisitor visitor) 
	{ 
		visitor.VisitPrintStmt(this);
	}
}

public class VarStmt(Token name, Expr? expr) : Stmt
{
	public Expr? Initializer = expr;
	public Token Name = name;

  public override void Accept(IStmtVisitor visitor)
  {
    visitor.VisitVarStmt(this);
  }
}

public class BlockStmt(List<Stmt> statements) : Stmt
{
	public List<Stmt> Statements = statements;

	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitBlockStmt(this);
	}
}

public class IfStatement(Expr condition, Stmt thenBranch, Stmt? elseBranch) : Stmt
{
	public Expr Condition { get; set; } = condition;
	public Stmt ThenBranch { get; set; } = thenBranch;
	public Stmt? ElseBranch { get; set; } = elseBranch;

	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitIfStmt(this);
	}
}
