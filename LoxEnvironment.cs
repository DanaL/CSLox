
namespace CSLox
{
  internal class LoxEnvironment
  {
    LoxEnvironment? EnclosingScope;
    Dictionary<string, object?> Values = [];

    public LoxEnvironment() => EnclosingScope = null;
    public LoxEnvironment(LoxEnvironment enclosing) => EnclosingScope = enclosing;
    
    public object? Get(Token name)
    {
      if (Values.TryGetValue(name.Lexeme, out object? value))
      {
        return value;
      }

      if (EnclosingScope is not null)
      {
        return EnclosingScope.Get(name);
      }

      throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
      if (Values.ContainsKey(name.Lexeme))
      {
        Values[name.Lexeme] = value;
        return;
      }

      if (EnclosingScope is not null)
      {
        EnclosingScope.Assign(name, value);
        return;
      }

      throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Define(string name, object? value)
    {
      Values.Add(name, value);
    }
  }
}
