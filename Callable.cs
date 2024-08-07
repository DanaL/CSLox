
namespace CSLox;

interface ICallable
{
  int Arity();
  object? Call(Interpreter interpreter, List<object> arguments);
}

class ClockFunction : ICallable
{
  public int Arity() => 0;

  public object? Call(Interpreter interpreter, List<object> arguments)
  {
    return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
  }
}

class LoxFunction(Function declaration, LoxEnvironment closure) : ICallable
{
  Function Declaration { get; set; } = declaration;
  LoxEnvironment Closure { get; set; } = closure;

  public int Arity() => Declaration.Params.Count;

  public object? Call(Interpreter interpreter, List<object> arguments)
  {
    var env = new LoxEnvironment(Closure);

    for (int i = 0; i < Declaration.Params.Count; i++)
    {
      env.Define(Declaration.Params[i].Lexeme, arguments[i]);
    }

    try
    {
      interpreter.ExecuteBlock(Declaration.Body, env);
    }
    catch (Return returnValue)
    {
      return returnValue.Value;
    }

    return null;
  }

  public override string ToString() => $"<fn {Declaration.Name.Lexeme}>";
}