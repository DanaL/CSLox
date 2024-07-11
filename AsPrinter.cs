
using System.Text;

namespace CSLox;

class AstPrinter : IVisitor<string>
{
  public string Print(Expr expr)
  {
    return expr.Accept<string>(this);
  }

  public string Parenthesize(string name, params Expr[] args)
  {
    var sb = new StringBuilder();

    sb.Append("(");
    sb.Append(name);
    foreach (var expr in args)
    {
      sb.Append(" ");
      sb.Append(expr.Accept<string>(this));
    }
    sb.Append(")");

    return sb.ToString();
  }

  public string VisitBinaryExpr(Binary expr)
  {
    return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
  }

  public string VisitGroupingExpr(Grouping expr)
  {
    return Parenthesize("group", expr.Expression);
  }

  public string VisitLiteralExpr(Literal expr)
  {
    return expr.Value.ToString() ?? "";
  }

  public string VisitUnaryExpr(Unary expr)
  {
    return Parenthesize(expr.Op.Lexeme, expr.Right);
  }
}
