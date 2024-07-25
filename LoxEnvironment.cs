
namespace CSLox
{
  class EnvVariable(bool assigned, object? val)
  {
    public bool Assigned { get; set; } = assigned;
    public object? Value { get; set; } = val;
  }

  class LoxEnvironment
  {
    LoxEnvironment? EnclosingScope;
    Dictionary<string, EnvVariable> Values = [];

    public LoxEnvironment() => EnclosingScope = null;
    public LoxEnvironment(LoxEnvironment enclosing) => EnclosingScope = enclosing;
    
    public object? Get(Token name)
    {
      if (Values.TryGetValue(name.Lexeme, out var envVar))
      {
        if (envVar.Assigned)
          return envVar.Value;

        throw new RuntimeError(name, $"Use of assigned var '{name.Lexeme}'.");
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
        Values[name.Lexeme] = new EnvVariable(true, value);
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
      Values.Add(name, new EnvVariable(true, value));
    }

    public void Define(string name)
    {
      Values.Add(name, new EnvVariable(false, null));
    }
  }
}
