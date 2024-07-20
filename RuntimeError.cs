using CSLox;

class RuntimeError(Token token, string message) : Exception(message)
{
  public Token Token { get; set; } = token;
}