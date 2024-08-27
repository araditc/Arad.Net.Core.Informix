using System;
using System.Text;



namespace Arad.Net.Core.Informix;
internal sealed class CStringTokenizer
{
    private readonly StringBuilder _token;

    private readonly string _sqlstatement;

    private readonly char _quote;

    private readonly char _escape;

    private int _len;

    private int _idx;

    internal int CurrentPosition => _idx;

    internal CStringTokenizer(string text, char quote, char escape)
    {
        _token = new StringBuilder();
        _quote = quote;
        _escape = escape;
        _sqlstatement = text;
        if (text != null)
        {
            int num = text.IndexOf('\0');
            _len = 0 > num ? text.Length : num;
        }
        else
        {
            _len = 0;
        }
    }

    internal string NextToken()
    {
        if (_token.Length != 0)
        {
            _idx += _token.Length;
            _token.Remove(0, _token.Length);
        }
        while (_idx < _len && char.IsWhiteSpace(_sqlstatement[_idx]))
        {
            _idx++;
        }
        if (_idx == _len)
        {
            return string.Empty;
        }
        int i = _idx;
        bool flag = false;
        while (!flag && i < _len)
        {
            if (IsValidNameChar(_sqlstatement[i]))
            {
                for (; i < _len && IsValidNameChar(_sqlstatement[i]); i++)
                {
                    _token.Append(_sqlstatement[i]);
                }
                continue;
            }
            char c = _sqlstatement[i];
            if (c == '[')
            {
                i = GetTokenFromBracket(i);
                continue;
            }
            if (' ' != _quote && c == _quote)
            {
                i = GetTokenFromQuote(i);
                continue;
            }
            if (!char.IsWhiteSpace(c))
            {
                if (c == ',')
                {
                    if (i == _idx)
                    {
                        _token.Append(c);
                    }
                }
                else
                {
                    _token.Append(c);
                }
            }
            flag = true;
            break;
        }
        if (_token.Length <= 0)
        {
            return string.Empty;
        }
        return _token.ToString();
    }

    private int GetTokenFromBracket(int curidx)
    {
        while (curidx < _len)
        {
            _token.Append(_sqlstatement[curidx]);
            curidx++;
            if (_sqlstatement[curidx - 1] == ']')
            {
                break;
            }
        }
        return curidx;
    }

    private int GetTokenFromQuote(int curidx)
    {
        int i;
        for (i = curidx; i < _len; i++)
        {
            _token.Append(_sqlstatement[i]);
            if (_sqlstatement[i] == _quote && i > curidx && _sqlstatement[i - 1] != _escape && i + 1 < _len && _sqlstatement[i + 1] != _quote)
            {
                return i + 1;
            }
        }
        return i;
    }

    private bool IsValidNameChar(char ch)
    {
        if (!char.IsLetterOrDigit(ch) && ch != '_' && ch != '-' && ch != '.' && ch != '$' && ch != '#' && ch != '@' && ch != '~' && ch != '`' && ch != '%' && ch != '^' && ch != '&')
        {
            return ch == '|';
        }
        return true;
    }

    internal int FindTokenIndex(string tokenString)
    {
        while (true)
        {
            string text = NextToken();
            if (_idx == _len || string.IsNullOrEmpty(text))
            {
                break;
            }
            if (string.Equals(tokenString, text, StringComparison.OrdinalIgnoreCase))
            {
                return _idx;
            }
        }
        return -1;
    }

    internal bool StartsWith(string tokenString)
    {
        int i;
        for (i = 0; i < _len && char.IsWhiteSpace(_sqlstatement[i]); i++)
        {
        }
        if (_len - i < tokenString.Length)
        {
            return false;
        }
        if (string.Compare(_sqlstatement, i, tokenString, 0, tokenString.Length, StringComparison.OrdinalIgnoreCase) == 0)
        {
            _idx = 0;
            NextToken();
            return true;
        }
        return false;
    }
}
