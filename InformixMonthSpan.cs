using System;
using System.Data.SqlTypes;
using System.Text;



namespace Arad.Net.Core.Informix;
public struct InformixMonthSpan : IComparable, INullable
{
    public static readonly InformixMonthSpan MaxValue = new InformixMonthSpan(11999999999L, InformixTimeUnit.Year, InformixTimeUnit.Month);

    public static readonly InformixMonthSpan MinValue = new InformixMonthSpan(-11999999999L, InformixTimeUnit.Year, InformixTimeUnit.Month);

    public static readonly InformixMonthSpan Null = new InformixMonthSpan(t: true);

    public static readonly InformixMonthSpan Zero = new InformixMonthSpan(0L, InformixTimeUnit.Year, InformixTimeUnit.Month);

    private long months;

    private bool isNull;

    private InformixTimeUnit startTimeUnit;

    private InformixTimeUnit endTimeUnit;

    internal IntervalDateTime IDT;

    public InformixTimeUnit StartTimeUnit
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            InformixTimeUnit result = startTimeUnit;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public InformixTimeUnit EndTimeUnit
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            InformixTimeUnit result = endTimeUnit;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public bool IsNull
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            bool result = isNull;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public int Years
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int result = (int)(months / 12);
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public int Months
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int result = (int)(months % 12);
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public long TotalMonths
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            long result = months;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    private InformixMonthSpan(bool t)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(t);
        months = 0L;
        isNull = t;
        startTimeUnit = InformixTimeUnit.Year;
        endTimeUnit = InformixTimeUnit.Month;
        IDT = IntervalDateTime.Null;
        ifxTrace?.ApiExit();
    }

    internal InformixMonthSpan(long _months, InformixTimeUnit start, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_months, start, end);
        months = _months;
        isNull = false;
        IDT = IntervalDateTime.Null;
        startTimeUnit = start;
        endTimeUnit = end;
        int num = (int)(months / 12);
        int num2 = (int)(months % 12);
        if (start == InformixTimeUnit.Year && start == end)
        {
            IDT = new IntervalDateTime(startTimeUnit, endTimeUnit, new object[1] { num });
        }
        else if (start == InformixTimeUnit.Month && start == end)
        {
            IDT = new IntervalDateTime(startTimeUnit, endTimeUnit, new object[1] { num2 });
        }
        else
        {
            IDT = new IntervalDateTime(startTimeUnit, endTimeUnit, num, num2);
        }
        ifxTrace?.ApiExit();
    }

    public InformixMonthSpan(int val, InformixTimeUnit timeUnit)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val, timeUnit);
        startTimeUnit = timeUnit;
        endTimeUnit = timeUnit;
        Qualifier.Validate(InformixTimeUnit.Year, InformixTimeUnit.Month, timeUnit, timeUnit);
        IDT = new IntervalDateTime(timeUnit, timeUnit, new object[1] { val });
        months = ValidateMonths(IDT);
        isNull = false;
        ifxTrace?.ApiExit();
    }

    public InformixMonthSpan(int _years, long _months)
    {
        int num = _years;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_years, _months);
        num += (int)(_months / 12);
        int num2 = (int)(_months % 12);
        startTimeUnit = InformixTimeUnit.Year;
        endTimeUnit = InformixTimeUnit.Month;
        IDT = new IntervalDateTime(InformixTimeUnit.Year, InformixTimeUnit.Month, num, num2);
        months = ValidateMonths(IDT);
        isNull = false;
        ifxTrace?.ApiExit();
    }

    internal InformixMonthSpan(IntervalDateTime _idt)
    {
        startTimeUnit = _idt.startTU;
        endTimeUnit = _idt.endTU;
        IDT = _idt;
        months = ValidateMonths(IDT);
        isNull = false;
    }

    public InformixMonthSpan Add(InformixMonthSpan ms)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms);
        InformixMonthSpan result = this + ms;
        ifxTrace?.ApiExit();
        return result;
    }

    public static int Compare(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        int num = 0;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        num = internalCompare(ms1, ms2);
        ifxTrace?.ApiExit();
        return num;
    }

    public int CompareTo(object obj)
    {
        int num = 0;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        if (obj == null)
        {
            throw new ArgumentNullException();
        }
        if (typeof(InformixMonthSpan) != obj.GetType())
        {
            throw new ArgumentException();
        }
        InformixMonthSpan ms = (InformixMonthSpan)obj;
        num = internalCompare(this, ms);
        ifxTrace?.ApiExit();
        return num;
    }

    public InformixMonthSpan Divide(decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        if (val == 0m)
        {
            throw new DivideByZeroException();
        }
        InformixMonthSpan result = this / val;
        ifxTrace?.ApiExit();
        return result;
    }

    public decimal Divide(InformixMonthSpan ms)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms);
        decimal result = this / ms;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool Equals(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        flag = internalCompare(ms1, ms2) == 0;
        ifxTrace?.ApiExit();
        return flag;
    }

    public override int GetHashCode()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new ArgumentNullException();
        }
        int hashCode = months.GetHashCode();
        ifxTrace?.ApiExit();
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        if (obj == null || typeof(InformixMonthSpan) != obj.GetType())
        {
            flag = false;
        }
        else
        {
            InformixMonthSpan ms = (InformixMonthSpan)obj;
            flag = internalCompare(ms, this) == 0;
        }
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool GreaterThan(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        flag = num == 1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool GreaterThanOrEqual(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        flag = num != -1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool LessThan(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        flag = num == -1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool LessThanOrEqual(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        flag = num != 1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool NotEquals(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        flag = internalCompare(ms1, ms2) != 0 ? true : false;
        ifxTrace?.ApiExit();
        return flag;
    }

    public InformixMonthSpan Multiply(decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        InformixMonthSpan result = this * val;
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixMonthSpan Negate()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixMonthSpan result = new InformixMonthSpan(-months, StartTimeUnit, EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan Parse(string val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        InformixMonthSpan result = Parse(val, InformixTimeUnit.Year, InformixTimeUnit.Month);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan Parse(string val, InformixTimeUnit start, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val, start, end);
        InformixMonthSpan yearMonthVal = getYearMonthVal(val, start, end);
        ifxTrace?.ApiExit();
        return yearMonthVal;
    }

    public static InformixMonthSpan Parse(string val, string format, InformixTimeUnit start, InformixTimeUnit end)
    {
        byte[] array = new byte[25];
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val, format, start, end);
        short num = Qualifier.IntervalEncode(start, end);
        CNativeBuffer cNativeBuffer = new CNativeBuffer(25);
        cNativeBuffer.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
        Informix32.RetCode retCode = Interop.Odbc.incvfmtasc(val, format, cNativeBuffer.Address);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        retCode = Interop.Odbc.intoasc(cNativeBuffer.Address, array);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
        string text = aSCIIEncoding.GetString(array, 0, array.Length);
        string[] array2 = text.Split('\0');
        if (array2.Length != 0)
        {
            text = array2[0];
        }
        InformixMonthSpan yearMonthVal = getYearMonthVal(text, start, end);
        ifxTrace?.ApiExit();
        return yearMonthVal;
    }

    public InformixMonthSpan Subtract(InformixMonthSpan ms)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms);
        if (IsNull || ms.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixMonthSpan result = this - ms;
        ifxTrace?.ApiExit();
        return result;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            stringBuilder.Append("Null");
        }
        else
        {
            long num = Math.Abs(months);
            if (months < 0)
            {
                stringBuilder.Append("-");
            }
            long result = num;
            if (startTimeUnit == InformixTimeUnit.Year)
            {
                long value = Math.DivRem(num, 12L, out result);
                stringBuilder.Append(value);
                if (endTimeUnit == InformixTimeUnit.Month)
                {
                    stringBuilder.Append("-");
                    if (result < 10)
                    {
                        stringBuilder.Append("0");
                    }
                }
            }
            if (endTimeUnit == InformixTimeUnit.Month)
            {
                stringBuilder.Append(result);
            }
        }
        ifxTrace?.ApiExit();
        return stringBuilder.ToString();
    }

    public string ToString(string format)
    {
        string text = "";
        byte[] array = new byte[format.Length + 50];
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(format);
        if (IsNull)
        {
            text = "Null";
        }
        else
        {
            string inbuf = ToString();
            short num = Qualifier.IntervalEncode(startTimeUnit, endTimeUnit);
            CNativeBuffer cNativeBuffer = new CNativeBuffer(25);
            cNativeBuffer.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
            Informix32.RetCode retCode = Interop.Odbc.incvasc(inbuf, cNativeBuffer.Address);
            if (retCode != 0)
            {
                throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
            }
            retCode = Interop.Odbc.intofmtasc(cNativeBuffer.Address, array, (short)(format.Length + 50), format);
            if (retCode != 0)
            {
                throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
            }
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            text = aSCIIEncoding.GetString(array, 0, array.Length);
            string[] array2 = text.Split('\0');
            if (array2.Length != 0)
            {
                text = array2[0];
            }
        }
        ifxTrace?.ApiExit();
        return text;
    }

    public InformixMonthSpan Duration()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixMonthSpan result = new InformixMonthSpan(Math.Abs(months), StartTimeUnit, EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan operator +(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        if (ms1.IsNull || ms2.IsNull)
        {
            throw new ArgumentNullException();
        }
        long num;
        try
        {
            num = ms1.months + ms2.months;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        InformixTimeUnit start = Qualifier.GreaterThan(ms1.StartTimeUnit, ms2.StartTimeUnit) ? ms1.StartTimeUnit : ms2.StartTimeUnit;
        InformixTimeUnit end = Qualifier.GreaterThan(ms2.EndTimeUnit, ms1.EndTimeUnit) ? ms1.EndTimeUnit : ms2.EndTimeUnit;
        ValidateMonths(num, start, end);
        InformixMonthSpan result = new InformixMonthSpan(num, start, end);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan operator /(InformixMonthSpan ms1, decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, val);
        if (ms1.IsNull)
        {
            throw new ArgumentNullException();
        }
        if (val == 0m)
        {
            throw new DivideByZeroException();
        }
        long num;
        try
        {
            num = (long)(ms1.months / val);
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        ValidateMonths(num, ms1.StartTimeUnit, ms1.EndTimeUnit);
        InformixMonthSpan result = new InformixMonthSpan(num, ms1.StartTimeUnit, ms1.EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan operator *(InformixMonthSpan ms1, decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, val);
        if (ms1.IsNull)
        {
            throw new ArgumentNullException();
        }
        long num;
        try
        {
            num = (long)(ms1.months * val);
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        ValidateMonths(num, ms1.StartTimeUnit, ms1.EndTimeUnit);
        InformixMonthSpan result = new InformixMonthSpan(num, ms1.StartTimeUnit, ms1.EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan operator -(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        if (ms1.IsNull || ms2.IsNull)
        {
            throw new ArgumentNullException();
        }
        long num;
        try
        {
            num = ms1.months - ms2.months;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        InformixTimeUnit start = Qualifier.GreaterThan(ms1.StartTimeUnit, ms2.StartTimeUnit) ? ms1.StartTimeUnit : ms2.StartTimeUnit;
        InformixTimeUnit end = Qualifier.GreaterThan(ms2.EndTimeUnit, ms1.EndTimeUnit) ? ms1.EndTimeUnit : ms2.EndTimeUnit;
        ValidateMonths(num, start, end);
        InformixMonthSpan result = new InformixMonthSpan(num, start, end);
        ifxTrace?.ApiExit();
        return result;
    }

    public static decimal operator /(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        if (ms1.IsNull || ms2.IsNull)
        {
            throw new ArgumentNullException();
        }
        decimal result = ms1.months / (decimal)ms2.months;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator ==(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        bool result = internalCompare(ms1, ms2) == 0;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator >(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        bool result = num == 1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator >=(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        bool result = num != -1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator !=(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        bool result = internalCompare(ms1, ms2) != 0 ? true : false;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator <(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        bool result = num == -1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator <=(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms1, ms2);
        int num = internalCompare(ms1, ms2);
        bool result = num != 1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan operator +(InformixMonthSpan ms)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms);
        if (ms.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixMonthSpan result = new InformixMonthSpan(ms.months, ms.StartTimeUnit, ms.EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixMonthSpan operator -(InformixMonthSpan ms)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ms);
        if (ms.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixMonthSpan result = new InformixMonthSpan(-ms.months, ms.StartTimeUnit, ms.EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixMonthSpan(string val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        InformixMonthSpan result = Parse(val, InformixTimeUnit.Year, InformixTimeUnit.Month);
        ifxTrace?.ApiExit();
        return result;
    }

    private static InformixMonthSpan getYearMonthVal(string val, InformixTimeUnit start, InformixTimeUnit end)
    {
        int num = 0;
        int num2 = 0;
        string[] array = null;
        short num3 = 0;
        if (val == null)
        {
            throw new ArgumentNullException();
        }
        val = val.Trim();
        InformixMonthSpan result;
        if (start == InformixTimeUnit.Year && end == InformixTimeUnit.Month)
        {
            array = val.Split('-');
            if (array.Length == 3)
            {
                num3 = 1;
            }
            int num4 = array.Length;
            if ((uint)(num4 - 2) > 1u)
            {
                throw new FormatException();
            }
            try
            {
                num2 = int.Parse(array[num3]);
                num = int.Parse(array[1 + num3]);
                if (num3 == 1)
                {
                    num2 *= -1;
                    num *= -1;
                }
            }
            catch (OverflowException)
            {
                throw new OverflowException();
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
            if (num2 > 999999999 || num2 < -999999999 || num > 11 || num < -11)
            {
                throw new OverflowException();
            }
            result = new InformixMonthSpan(num2, num);
        }
        else
        {
            if (start != end)
            {
                throw new ArgumentOutOfRangeException();
            }
            try
            {
                num = int.Parse(val);
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
            result = new InformixMonthSpan(num, start);
        }
        return result;
    }

    private static long ValidateMonths(IntervalDateTime intervalDateTimeVal)
    {
        long result;
        try
        {
            result = intervalDateTimeVal.Year * 12L + intervalDateTimeVal.Month;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        ValidateMonths(result, intervalDateTimeVal.startTU, intervalDateTimeVal.endTU);
        return result;
    }

    private static void ValidateMonths(long _months, InformixTimeUnit start, InformixTimeUnit end)
    {
        if (start == InformixTimeUnit.Year && end == InformixTimeUnit.Year)
        {
            int num = (int)(_months / 12);
            if (num > 999999999 || num < -999999999)
            {
                throw new OverflowException();
            }
        }
        else if (start == InformixTimeUnit.Month && end == InformixTimeUnit.Month)
        {
            if (_months > 999999999 || _months < -999999999)
            {
                throw new OverflowException();
            }
        }
        else if (_months > 11999999999L || _months < -11999999999L)
        {
            throw new OverflowException();
        }
    }

    private static int internalCompare(InformixMonthSpan ms1, InformixMonthSpan ms2)
    {
        int result = 0;
        if (ms1.IsNull && ms2.IsNull)
        {
            result = 0;
        }
        else if (ms1.IsNull)
        {
            result = -1;
        }
        else if (ms2.IsNull)
        {
            result = 1;
        }
        else if (ms1.months > ms2.months)
        {
            result = 1;
        }
        else if (ms1.months < ms2.months)
        {
            result = -1;
        }
        else if (ms1.months == ms2.months)
        {
            result = 0;
        }
        return result;
    }
}
