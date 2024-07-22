namespace CSLox;

public interface IStmtVisitor
{
  public void VisitExprStmt(ExprStmt stmt);
  public void VisitPrintStmt(PrintStmt stmt);
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

