using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;



namespace Arad.Net.Core.Informix;
public struct InformixDecimal : IComparable, INullable
{
    public static readonly InformixDecimal MaxValue = new InformixDecimal(DecimalConstants("MAX_DEC"));

    public static readonly InformixDecimal MinValue = new InformixDecimal(DecimalConstants("MIN_DEC"));

    public static readonly InformixDecimal MinusOne = new InformixDecimal(-1);

    public static readonly InformixDecimal Null = new InformixDecimal(NullFlag: true);

    public static readonly InformixDecimal One = new InformixDecimal(1);

    public static readonly InformixDecimal Zero = new InformixDecimal(0);

    public static readonly InformixDecimal Pi = new InformixDecimal(DecimalConstants("PI"));

    public static readonly InformixDecimal E = new InformixDecimal(DecimalConstants("EXP"));

    public static readonly byte MaxPrecision = 32;

    private const int SizeofDectStruct = 22;

    private const int MaxDecStringBufSize = 256;

    private short exp;

    private short pos;

    private short ndgts;

    internal byte[] DectVal;

    public bool IsNull
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            if (pos == -1)
            {
                return true;
            }
            return false;
        }
    }

    public bool IsFloating
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new ArgumentNullException();
            }
            ifxTrace?.ApiExit();
            return true;
        }
    }

    public bool IsPositive
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new ArgumentNullException();
            }
            ifxTrace?.ApiExit();
            if (pos == 1)
            {
                return true;
            }
            return false;
        }
    }

    internal InformixDecimal(byte[] Dect)
    {
        exp = 0;
        pos = 0;
        ndgts = 0;
        DectVal = Dect;
        SyncLoclVarWithDect();
    }

    private InformixDecimal(string s)
    {
        exp = 0;
        pos = 0;
        ndgts = 0;
        DectVal = StringToDecT(s);
        SyncLoclVarWithDect();
    }

    private InformixDecimal(bool NullFlag)
    {
        exp = 0;
        pos = 0;
        ndgts = 0;
        DectVal = new byte[22];
        pos = -1;
        SyncDectValWithLoclVar();
    }

    public InformixDecimal(int i32)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i32);
        exp = 0;
        pos = 0;
        ndgts = 0;
        string s = i32.ToString();
        DectVal = StringToDecT(s);
        SyncLoclVarWithDect();
        ifxTrace?.ApiExit();
    }

    public InformixDecimal(decimal d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        exp = 0;
        pos = 0;
        ndgts = 0;
        string s = d.ToString();
        DectVal = StringToDecT(s);
        SyncLoclVarWithDect();
        ifxTrace?.ApiExit();
    }

    public InformixDecimal(long d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        exp = 0;
        pos = 0;
        ndgts = 0;
        string s = d.ToString();
        DectVal = StringToDecT(s);
        SyncLoclVarWithDect();
        ifxTrace?.ApiExit();
    }

    public InformixDecimal(double d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        exp = 0;
        pos = 0;
        ndgts = 0;
        string s = d.ToString();
        DectVal = StringToDecT(s);
        SyncLoclVarWithDect();
        ifxTrace?.ApiExit();
    }

    private static byte[] DecimalConstants(string constString)
    {
        byte[] array = new byte[22];
        int num = 0;
        int num2 = -1;
        int num3 = 0;
        char[] array2 = new char[16];
        switch (constString)
        {
            case "MAX_DEC":
                num = 62;
                num2 = 1;
                num3 = 16;
                array2 = "cccccccccccccccc".ToCharArray();
                break;
            case "MIN_DEC":
                num = 62;
                num2 = 0;
                num3 = 16;
                array2 = "cccccccccccccccc".ToCharArray();
                break;
            case "PI":
                {
                    num = 1;
                    num2 = 1;
                    num3 = 16;
                    string text2 = "03141592653589793238462643383279";
                    for (int j = 0; j < 16; j++)
                    {
                        array2[j] = (char)int.Parse(text2.Substring(j * 2, 2));
                    }
                    break;
                }
            case "EXP":
                {
                    num = 1;
                    num2 = 1;
                    num3 = 16;
                    string text = "02718281828459045235360287471352";
                    for (int i = 0; i < 16; i++)
                    {
                        array2[i] = (char)int.Parse(text.Substring(i * 2, 2));
                    }
                    break;
                }
        }
        short num4 = Interop.Odbc.decload(array, num2, num, array2, num3);
        if (num4 != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num4));
        }
        return array;
    }

    private unsafe void SyncLoclVarWithDect()
    {
        fixed (byte* ptr = &DectVal[0])
        {
            short* ptr2 = (short*)ptr;
            exp = *ptr2;
            pos = ptr2[1];
            ndgts = ptr2[2];
        }
    }

    private unsafe void SyncDectValWithLoclVar()
    {
        fixed (byte* ptr = &DectVal[0])
        {
            short* ptr2 = (short*)ptr;
            *ptr2 = exp;
            ptr2[1] = pos;
            ptr2[2] = ndgts;
        }
    }

    private static short DecCmp(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        short num = 0;
        if (IfxDec1.IsNull && IfxDec2.IsNull)
        {
            return 0;
        }
        if (IfxDec1.IsNull)
        {
            return -1;
        }
        if (IfxDec2.IsNull)
        {
            return 1;
        }
        num = Interop.Odbc.deccmp(IfxDec1.DectVal, IfxDec2.DectVal);
        if (num == -2)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        return num;
    }

    private static byte[] StringToDecT(string s)
    {
        byte[] array = new byte[22];
        int num = (short)s.Length;
        byte[] array2 = new byte[num + 1];
        Informix32.encoder.GetBytes(s.ToCharArray(), 0, num, array2, 0);
        Informix32.RetCode retCode = Interop.Odbc.deccvasc(array2, num, array);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        return array;
    }

    public static int Compare(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        int result = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        return result;
    }

    public int CompareTo(object obj)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        if (GetType() != obj.GetType())
        {
            throw new ArgumentException();
        }
        InformixDecimal ifxDec = (InformixDecimal)obj;
        int result = DecCmp(this, ifxDec);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDecimal Clone()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new InvalidOperationException();
        }
        byte[] array = new byte[22];
        for (int i = 0; i < 22; i++)
        {
            array[i] = DectVal[i];
        }
        InformixDecimal result = new InformixDecimal(array);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Abs(InformixDecimal IfxDec_)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec_);
        if (IfxDec_.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixDecimal result = IfxDec_.Clone();
        if (result.pos != 1)
        {
            result.pos = 1;
            result.SyncDectValWithLoclVar();
        }
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Add(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        if (IfxDec1.IsNull || IfxDec2.IsNull)
        {
            throw new ArgumentNullException();
        }
        byte[] array = new byte[22];
        short num = Interop.Odbc.decadd(IfxDec1.DectVal, IfxDec2.DectVal, array);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        InformixDecimal result = new InformixDecimal(array);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Subtract(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        if (IfxDec1.IsNull || IfxDec2.IsNull)
        {
            throw new ArgumentNullException();
        }
        byte[] array = new byte[22];
        short num = Interop.Odbc.decsub(IfxDec1.DectVal, IfxDec2.DectVal, array);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        InformixDecimal result = new InformixDecimal(array);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Modulo(InformixDecimal a, InformixDecimal b)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(a, b);
        InformixDecimal result = Remainder(a, b);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Remainder(InformixDecimal a, InformixDecimal b)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(a, b);
        if (a.IsNull || b.IsNull)
        {
            throw new ArgumentNullException();
        }
        if (DecCmp(b, Zero) == 0)
        {
            throw new DivideByZeroException();
        }
        InformixDecimal ifxDec = a / b;
        ifxDec = Truncate(ifxDec, 0);
        InformixDecimal result = Subtract(a, Multiply(b, ifxDec));
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Divide(InformixDecimal Dividend, InformixDecimal Divisor)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(Dividend, Divisor);
        if (Dividend.IsNull || Divisor.IsNull)
        {
            throw new ArgumentNullException();
        }
        if (Divisor == Zero)
        {
            throw new DivideByZeroException();
        }
        byte[] array = new byte[22];
        short num = Interop.Odbc.decdiv(Dividend.DectVal, Divisor.DectVal, array);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        InformixDecimal result = new InformixDecimal(array);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Multiply(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        if (IfxDec1.IsNull || IfxDec2.IsNull)
        {
            throw new ArgumentNullException();
        }
        byte[] array = new byte[22];
        short num = Interop.Odbc.decmul(IfxDec1.DectVal, IfxDec2.DectVal, array);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        InformixDecimal result = new InformixDecimal(array);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Round(InformixDecimal IfxDec1, int FractionDigits)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, FractionDigits);
        if (IfxDec1.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixDecimal result = IfxDec1.Clone();
        Interop.Odbc.decround(result.DectVal, FractionDigits);
        result.SyncLoclVarWithDect();
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Truncate(InformixDecimal IfxDec1, int FractionDigits)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, FractionDigits);
        if (IfxDec1.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixDecimal result = IfxDec1.Clone();
        Interop.Odbc.dectrunc(result.DectVal, FractionDigits);
        result.SyncLoclVarWithDect();
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Min(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        if (IfxDec1.IsNull || IfxDec2.IsNull)
        {
            return Null;
        }
        short num = DecCmp(IfxDec1, IfxDec2);
        InformixDecimal result = num <= 0 ? IfxDec1 : IfxDec2;
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Max(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        if (IfxDec1.IsNull || IfxDec2.IsNull)
        {
            return Null;
        }
        short num = DecCmp(IfxDec1, IfxDec2);
        InformixDecimal result = num >= 0 ? IfxDec1 : IfxDec2;
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Negate(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixDecimal result = Multiply(IfxDec, MinusOne);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal operator -(InformixDecimal IfxDec)
    {
        return Negate(IfxDec);
    }

    public static bool Equals(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        short num = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        if (num == 0)
        {
            return true;
        }
        return false;
    }

    public static bool NotEquals(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        short num = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        if (num != 0)
        {
            return true;
        }
        return false;
    }

    public static bool GreaterThan(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        short num = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        if (num > 0)
        {
            return true;
        }
        return false;
    }

    public static bool GreaterThanOrEqual(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        short num = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        if (num >= 0)
        {
            return true;
        }
        return false;
    }

    public static bool LessThan(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        short num = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        if (num < 0)
        {
            return true;
        }
        return false;
    }

    public static bool LessThanOrEqual(InformixDecimal IfxDec1, InformixDecimal IfxDec2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec1, IfxDec2);
        short num = DecCmp(IfxDec1, IfxDec2);
        ifxTrace?.ApiExit();
        if (num <= 0)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        string text = "Null";
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            return "Null";
        }
        byte[] dectVal = DectVal;
        byte[] array = new byte[256];
        Informix32.RetCode retCode = Interop.Odbc.dectoasc(dectVal, array, 256, -1);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        string @string = Informix32.encoder.GetString(array);
        text = @string.TrimEnd(" \0\t".ToCharArray());
        ifxTrace?.ApiExit();
        return text;
    }

    public string ToString(string format)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(format);
        if (IsNull)
        {
            throw new InvalidOperationException();
        }
        string text = ToString();
        byte[] bytes = Informix32.encoder.GetBytes(format);
        byte[] array = new byte[256];
        byte[] dectVal = DectVal;
        short num = Interop.Odbc.rfmtdec(dectVal, bytes, array);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        string @string = Informix32.encoder.GetString(array);
        text = @string.TrimEnd(" \0\t".ToCharArray());
        ifxTrace?.ApiExit();
        return text;
    }

    public static InformixDecimal Ceiling(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixDecimal ifxDecimal = Truncate(IfxDec, 0);
        if (ifxDecimal < IfxDec)
        {
            ifxDecimal += One;
        }
        ifxTrace?.ApiExit();
        return ifxDecimal;
    }

    public static InformixDecimal Floor(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixDecimal ifxDecimal = Truncate(IfxDec, 0);
        if (ifxDecimal > IfxDec)
        {
            ifxDecimal -= One;
        }
        ifxTrace?.ApiExit();
        return ifxDecimal;
    }

    public static explicit operator double(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        CNativeBuffer cNativeBuffer = new CNativeBuffer(8);
        short num = Interop.Odbc.dectodbl(IfxDec.DectVal, cNativeBuffer.Address);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        object obj = cNativeBuffer.MarshalToManaged(Informix32.SQL_C.DOUBLE, 8);
        double result = (double)obj;
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixDecimal(double d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        InformixDecimal result = new InformixDecimal(d);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixDecimal(decimal d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        string s = d.ToString();
        InformixDecimal result = Parse(s);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator decimal(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        string s = IfxDec.ToString();
        decimal result = decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator int(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        CNativeBuffer cNativeBuffer = new CNativeBuffer(4);
        short num = Interop.Odbc.dectolong(IfxDec.DectVal, cNativeBuffer.Address);
        if (num != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, (Informix32.RetCode)num));
        }
        object obj = cNativeBuffer.MarshalToManaged(Informix32.SQL_C.SLONG, 4);
        int result = (int)obj;
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator long(InformixDecimal IfxDec)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(IfxDec);
        if (IfxDec.IsNull)
        {
            throw new ArgumentNullException();
        }
        string text = IfxDec.ToString();
        char[] array = text.ToCharArray();
        int length = text.Length;
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < length && array[i] >= '0' && array[i] <= '9'; i++)
        {
            stringBuilder.Append(array[i]);
        }
        string text2 = stringBuilder.ToString();
        long result = long.Parse(text2.Trim());
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixDecimal(int d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        InformixDecimal result = new InformixDecimal(d);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixDecimal(long d)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(d);
        InformixDecimal result = new InformixDecimal(d);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixDecimal(string s)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(s);
        InformixDecimal result = Parse(s);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal Parse(string s)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(s);
        InformixDecimal result = new InformixDecimal(s);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDecimal operator +(InformixDecimal x, InformixDecimal y)
    {
        return Add(x, y);
    }

    public static InformixDecimal operator -(InformixDecimal x, InformixDecimal y)
    {
        return Subtract(x, y);
    }

    public static InformixDecimal operator *(InformixDecimal x, InformixDecimal y)
    {
        return Multiply(x, y);
    }

    public static InformixDecimal operator /(InformixDecimal x, InformixDecimal y)
    {
        return Divide(x, y);
    }

    public static InformixDecimal operator %(InformixDecimal x, InformixDecimal y)
    {
        return Remainder(x, y);
    }

    public static bool operator >(InformixDecimal x, InformixDecimal y)
    {
        return GreaterThan(x, y);
    }

    public static bool operator >=(InformixDecimal x, InformixDecimal y)
    {
        return GreaterThanOrEqual(x, y);
    }

    public static bool operator <(InformixDecimal x, InformixDecimal y)
    {
        return LessThan(x, y);
    }

    public static bool operator <=(InformixDecimal x, InformixDecimal y)
    {
        return LessThanOrEqual(x, y);
    }

    public static bool operator ==(InformixDecimal x, InformixDecimal y)
    {
        return Equals(x, y);
    }

    public static bool operator !=(InformixDecimal x, InformixDecimal y)
    {
        return NotEquals(x, y);
    }

    public override bool Equals(object obj)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        if (obj == null || GetType() != obj.GetType())
        {
            ifxTrace?.ApiExit();
            return false;
        }
        InformixDecimal ifxDecimal = (InformixDecimal)obj;
        ifxTrace?.ApiExit();
        return this == ifxDecimal;
    }

    public override int GetHashCode()
    {
        return GetHashCode();
    }
}
