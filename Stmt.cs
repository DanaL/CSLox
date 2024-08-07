namespace CSLox;

public interface IStmtVisitor
{
  public void VisitExprStmt(ExprStmt stmt);
  public void VisitPrintStmt(PrintStmt stmt);
	public void VisitVarStmt(VarStmt stmt);
	public void VisitBlockStmt(BlockStmt stmt);
	public void VisitIfStmt(IfStatement stmt);
  public void VisitWhileStmt(WhileStmt stmt);
	public void VisitFunction(Function stmt);
	public void VisitReturnStmt(ReturnStmt stmt);
}

public abstract class Stmt 
{
	public abstract void Accept(IStmtVisitor visitor);
}

public class ExprStmt(Expr expr) : Stmt
{
	public Expr Expression = expr;

	public override void Accept(IStmtVisitor visitor) => visitor.VisitExprStmt(this);
}

public class PrintStmt(Expr expr) : Stmt
{
	public Expr Expression = expr;

	public override void Accept(IStmtVisitor visitor) => visitor.VisitPrintStmt(this);
}

public class VarStmt(Token name, Expr? expr) : Stmt
{
	public Expr? Initializer = expr;
	public Token Name = name;

  public override void Accept(IStmtVisitor visitor) => visitor.VisitVarStmt(this);
}

public class BlockStmt(List<Stmt> statements) : Stmt
{
	public List<Stmt> Statements = statements;

	public override void Accept(IStmtVisitor visitor) => visitor.VisitBlockStmt(this);
}

public class IfStatement(Expr condition, Stmt thenBranch, Stmt? elseBranch) : Stmt
{
	public Expr Condition { get; set; } = condition;
	public Stmt ThenBranch { get; set; } = thenBranch;
	public Stmt? ElseBranch { get; set; } = elseBranch;

	public override void Accept(IStmtVisitor visitor) => visitor.VisitIfStmt(this);
}

public class WhileStmt(Expr condition, Stmt body) : Stmt
{
  public Expr Condition { get; set; } = condition;
  public Stmt Body { get; set; } = body;

  public override void Accept(IStmtVisitor visitor) => visitor.VisitWhileStmt(this);
}

public class Function(Token name, List<Token> args, List<Stmt> body) : Stmt
{
	public Token Name { get; set; } = name;
	public List<Token> Params { get; set; } = args;
	public List<Stmt> Body { get; set; } = body;

  public override void Accept(IStmtVisitor visitor) => visitor.VisitFunction(this);
}

public class ReturnStmt(Token keyword, Expr? val) : Stmt
{
	public Token Keyword { get; set; } = keyword;
	public Expr? Value { get; set; } = val;

	public override void Accept(IStmtVisitor visitor) => visitor.VisitReturnStmt(this);
}