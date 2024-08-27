using System;



namespace Arad.Net.Core.Informix;
internal struct SQLLEN
{
    private nint _value;

    internal SQLLEN(int value)
    {
        _value = new nint(value);
    }

    internal SQLLEN(long value)
    {
        _value = new nint(value);
    }

    internal SQLLEN(nint value)
    {
        _value = value;
    }

    public static implicit operator SQLLEN(int value)
    {
        return new SQLLEN(value);
    }

    public static explicit operator SQLLEN(long value)
    {
        return new SQLLEN(value);
    }

    public static implicit operator int(SQLLEN value)
    {
        long num = value._value.ToInt64();
        return checked((int)num);
    }

    public static explicit operator long(SQLLEN value)
    {
        return value._value.ToInt64();
    }

    public long ToInt64()
    {
        return _value.ToInt64();
    }
}
