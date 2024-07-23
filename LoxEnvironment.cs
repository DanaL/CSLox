
namespace CSLox
{
  internal class LoxEnvironment
  {
    Dictionary<string, object?> Values = [];

    public LoxEnvironment() { }

    public object? Get(Token name)
    {
      if (Values.TryGetValue(name.Lexeme, out object? value))
      {
        return value;
      }

      throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Define(string name, object? value)
    {
      Values.Add(name, value);
    }
  }
}
