
using System.Text;

namespace CSLox;

class AstPrinter : IExprVisitor<string>
{
  public string Print(Expr expr) => expr.Accept<string>(this);

  string Parenthesize(string name, params Expr[] args)
  {
    var sb = new StringBuilder();

    sb.Append('(');
    sb.Append(name);
    foreach (var expr in args)
    {
      sb.Append(' ');
      sb.Append(expr.Accept<string>(this));
    }
    sb.Append(')');

    return sb.ToString();
  }

  public string VisitBinaryExpr(Binary expr) => Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
  public string VisitGroupingExpr(Grouping expr) => Parenthesize("group", expr.Expression);
  public string VisitLiteralExpr(Literal expr) => expr.Value.ToString() ?? "";
  public string VisitUnaryExpr(Unary expr) => Parenthesize(expr.Op.Lexeme, expr.Right);
  public string VisitTernaryExpr(Ternary expr) => Parenthesize("tern", expr.Test, expr.Pass, expr.Fail);
  public string VisitAssignExpr(Assign expr) => throw new NotImplementedException();

  public string VisitExprStmt(ExprStmt stmt)
  {
    throw new NotImplementedException();
  }

  public string VisitPrintStmt(PrintStmt stmt)
  {
    throw new NotImplementedException();
  }

  public string VisitVariableExpr(Variable expr) 
  { 
    throw new NotImplementedException(); 
  }

  public string VisitLogicalExpr(Logical expr)
  {
    throw new NotImplementedException();
  }
}

