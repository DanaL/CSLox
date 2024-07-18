namespace CSLox; 

public enum TokenType
{
    // Single-character tokens
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT,
    MINUS, PLUS, SEMICOLON, SLASH, STAR,
    
    // One or two character tokens
    BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL,
    LESS, LESS_EQUAL,
    
    // Literals
    IDENTIFIER, STRING, NUMBER,
    
    // Keywords
    AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR, PRINT, RETURN,
    SUPER, THIS, TRUE, VAR, WHILE,
    
    EOF 
}

public class Token(TokenType type, string lexeme, object? literal, int line)
{
  public TokenType Type { get; set; } = type;
  public string Lexeme { get; set; } = lexeme;
  public object? Literal { get; set; } = literal;
  public int Line { get; set; } = line;

  public override string ToString() => $"{Type} {Lexeme} {Literal}";
}