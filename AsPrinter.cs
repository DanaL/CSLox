
using System.Text;

namespace CSLox;

class AstPrinter : IVisitor<string>
{
  public string Print(Expr expr)
  {
    return expr.Accept<string>(this);
  }

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
}

class AstPrinterRPN : IVisitor<string>
{
  public string Print(Expr expr)
  {
    return expr.Accept<string>(this);
  }

  public string VisitBinaryExpr(Binary expr)
  {
    return expr.Left.Accept<string>(this) + " " +
            expr.Right.Accept<string>(this) + " " + expr.Op.Lexeme;
  }

  public string VisitGroupingExpr(Grouping expr) => expr.Expression.Accept<string>(this);
  public string VisitLiteralExpr(Literal expr) => expr.Value.ToString() ?? "";
  public string VisitUnaryExpr(Unary expr) =>  $"{expr.Right.Accept<string>(this)} {expr.Op.Lexeme}";
}
