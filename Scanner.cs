namespace CSLox;

public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _start;
    private int _current;
    private int _line = 1;
   
    static Dictionary<string, TokenType> _keywords = new()
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "fn", TokenType.FUN },
        { "for", TokenType.FOR },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE }
    };
    
    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': 
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                
                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                _line++;
                break;
            case '"':
                String();
                break;
            default:
                if (IsDigit(c))
                    Number();
                else if (IsAlpha(c))
                    Identifier();
                else
                    Lox.Error(_line, $"Unexpected character: {c}.");

                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
            Advance();

        string text = _source.Substring(_start, _current - _start - 1);
        TokenType type = _keywords.ContainsKey(text) ? _keywords[text] : TokenType.IDENTIFIER;
        AddToken(type);
    }
    
    private bool IsAlpha(char c)
    {
        return char.IsLetter(c) || c == '_';
    }
    
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }
    
    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private void ConsumeDigits()
    {
        while (IsDigit(Peek()))
            Advance();
    }
    
    private void Number()
    {
        ConsumeDigits();
        if (Peek() == '.' && IsDigit(Peeek()))
        {
            Advance();
            ConsumeDigits();
        }

        AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
    }
    
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, "Unterminated string.");
            return;
        }
        
        // skip closing "
        Advance();

        string? value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.STRING, value);
    }

    private char Peeek()
    {
        if (_current + 1 >= _source.Length)
            return '\0';

        return _source[_current + 1];
    }
    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_current];
    }
    
    private bool Match(char expected)
    {
        if (IsAtEnd())
            return false;
        if (_source[_current] != expected)
            return false;

        _current++;
        return true;
    }
    
    private char Advance()
    {
        return _source[_current++];
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = _source.Substring(_start, _current - _start);
        _tokens.Add(new(type, text, literal, _line));
    }
    
    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }
}