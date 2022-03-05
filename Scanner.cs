using System.Data.Common;

namespace CSLox;

public class Scanner
{
    private string source;
    private List<Token> tokens;
    private int start;
    private int current;
    private int line = 1;
   
    static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
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
        this.source = source;
        tokens = new List<Token>();
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        return tokens;
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
                line++;
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
                    Lox.Error(line, $"Unexpected character: {c}.");

                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
            Advance();

        string text = source.Substring(start, current - start - 1);
        TokenType type = keywords.ContainsKey(text) ? keywords[text] : TokenType.IDENTIFIER;
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

        AddToken(TokenType.NUMBER, double.Parse(source.Substring(start, current - start)));
    }
    
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
                line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }
        
        // skip closing "
        Advance();

        string value = source.Substring(start + 1, current - start - 2);
        AddToken(TokenType.STRING, value);
    }

    private char Peeek()
    {
        if (current + 1 >= source.Length)
            return '\0';

        return source[current + 1];
    }
    private char Peek()
    {
        return IsAtEnd() ? '\0' : source[current];
    }
    
    private bool Match(char expected)
    {
        if (IsAtEnd())
            return false;
        if (source[current] != expected)
            return false;

        current++;
        return true;
    }
    
    private char Advance()
    {
        return source[current++];
    }

    private void AddToken(TokenType type, object literal = null)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }
    
    private bool IsAtEnd()
    {
        return current >= source.Length;
    }
}