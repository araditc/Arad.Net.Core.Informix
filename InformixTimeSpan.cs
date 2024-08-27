using System;
using System.Data.SqlTypes;
using System.Text;


namespace Arad.Net.Core.Informix;

public struct InformixTimeSpan : IComparable, INullable
{
    public static readonly InformixTimeSpan MaxValue = new InformixTimeSpan(864000000000m * 1000000000m - 100m, InformixTimeUnit.Day, InformixTimeUnit.Fraction5);

    public static readonly InformixTimeSpan MinValue = new InformixTimeSpan(864000000000m * -1000000000m + 100m, InformixTimeUnit.Day, InformixTimeUnit.Fraction5);

    public static readonly InformixTimeSpan Null = new InformixTimeSpan(t: true);

    public static readonly InformixTimeSpan Zero = new InformixTimeSpan(0m, InformixTimeUnit.Day, InformixTimeUnit.Fraction5);

    public static readonly int MaxScale = 5;

    private decimal ticks;

    private bool isNull;

    internal IntervalDateTime IDTFullyQualified;

    internal IntervalDateTime IDTUserQualified;

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
            InformixTimeUnit startTU = IDTUserQualified.startTU;
            ifxTrace?.ApiExit();
            return startTU;
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
            InformixTimeUnit endTU = IDTUserQualified.endTU;
            ifxTrace?.ApiExit();
            return endTU;
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

    public int Days
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int day = IDTFullyQualified.Day;
            ifxTrace?.ApiExit();
            return day;
        }
    }

    public int Hours
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int hour = IDTFullyQualified.Hour;
            ifxTrace?.ApiExit();
            return hour;
        }
    }

    public int Minutes
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int minute = IDTFullyQualified.Minute;
            ifxTrace?.ApiExit();
            return minute;
        }
    }

    public int Seconds
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int second = IDTFullyQualified.Second;
            ifxTrace?.ApiExit();
            return second;
        }
    }

    public int Milliseconds
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            int result = (int)decimal.Multiply(IDTUserQualified.Fraction, (decimal)Math.Pow(10.0, 13 - (short)IDTUserQualified.endTU));
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public decimal Ticks
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsNull)
            {
                throw new InvalidOperationException();
            }
            decimal result = ticks;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    private void Initialize(InformixTimeUnit end, params object[] obj)
    {
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, obj.Length, end);
        ticks = new IntervalDateTime(start, end, obj).ToTicks();
        IDTUserQualified = new IntervalDateTime(start, end, ticks);
        ValidateValue(IDTUserQualified);
        IDTFullyQualified = new IntervalDateTime(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, ticks);
    }

    internal InformixTimeSpan(IntervalDateTime IDT)
    {
        isNull = false;
        IDTUserQualified = IDT;
        ticks = IDT.ToTicks();
        IDTFullyQualified = new IntervalDateTime(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, ticks);
    }

    private InformixTimeSpan(bool t)
    {
        ticks = default;
        isNull = t;
        IDTUserQualified = new IntervalDateTime(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, ticks);
        IDTFullyQualified = IDTUserQualified;
    }

    internal InformixTimeSpan(decimal _ticks, InformixTimeUnit start, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_ticks, start, end);
        isNull = false;
        ticks = _ticks;
        IDTUserQualified = new IntervalDateTime(start, end, ticks);
        IDTFullyQualified = IDTUserQualified;
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(long _ticks)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_ticks);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = _ticks;
        if (ticks > MaxValue.ticks || ticks < MinValue.ticks)
        {
            throw new OverflowException();
        }
        IDTUserQualified = new IntervalDateTime(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, ticks);
        ValidateValue(IDTUserQualified);
        IDTFullyQualified = IDTUserQualified;
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(TimeSpan ts)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = ts.Ticks;
        if (ticks > MaxValue.ticks || ticks < MinValue.ticks)
        {
            throw new OverflowException();
        }
        IDTUserQualified = new IntervalDateTime(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, ticks);
        IDTFullyQualified = IDTUserQualified;
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(decimal _ticks)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_ticks);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = _ticks;
        if (ticks > MaxValue.ticks || ticks < MinValue.ticks)
        {
            throw new OverflowException();
        }
        IDTUserQualified = new IntervalDateTime(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, ticks);
        ValidateValue(IDTUserQualified);
        IDTFullyQualified = IDTUserQualified;
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(int val, InformixTimeUnit timeUnit)
    {
        InformixTimeUnit end = timeUnit;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val, timeUnit);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = default;
        if (timeUnit == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Initialize(end, val);
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(int val1, int val2, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val1, val2, end);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = default;
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Initialize(end, val1, val2);
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(int val1, int val2, int val3, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val1, val2, val3, end);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = default;
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Initialize(end, val1, val2, val3);
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(int val1, int val2, int val3, int val4, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val1, val2, val3, val4, end);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = default;
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Initialize(end, val1, val2, val3, val4);
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan(int val1, int val2, int val3, int val4, int val5, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val1, val2, val3, val4, val5, end);
        isNull = false;
        IDTUserQualified = IntervalDateTime.Null;
        IDTFullyQualified = IntervalDateTime.Null;
        ticks = default;
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Initialize(end, val1, val2, val3, val4, val5);
        ifxTrace?.ApiExit();
    }

    public InformixTimeSpan Add(InformixTimeSpan ts)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts);
        InformixTimeSpan result = this + ts;
        ifxTrace?.ApiExit();
        return result;
    }

    public static int Compare(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        int num = 0;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        num = internalCompare(ts1, ts2);
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
        if (typeof(InformixTimeSpan) != obj.GetType())
        {
            throw new ArgumentException();
        }
        InformixTimeSpan ts = (InformixTimeSpan)obj;
        num = internalCompare(this, ts);
        ifxTrace?.ApiExit();
        return num;
    }

    public InformixTimeSpan Divide(decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        if (val == 0m)
        {
            throw new DivideByZeroException();
        }
        InformixTimeSpan result = this / val;
        ifxTrace?.ApiExit();
        return result;
    }

    public decimal Divide(InformixTimeSpan ts)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts);
        decimal result = this / ts;
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixTimeSpan Duration()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixTimeSpan result = new InformixTimeSpan(Math.Abs(ticks), StartTimeUnit, EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public override int GetHashCode()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new ArgumentNullException();
        }
        int hashCode = ticks.GetHashCode();
        ifxTrace?.ApiExit();
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        if (obj == null || typeof(InformixTimeSpan) != obj.GetType())
        {
            flag = false;
        }
        else
        {
            InformixTimeSpan ts = (InformixTimeSpan)obj;
            flag = internalCompare(ts, this) == 0;
        }
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool Equals(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        flag = internalCompare(ts1, ts2) == 0;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool GreaterThan(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        flag = num == 1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool GreaterThanOrEqual(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        flag = num != -1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool LessThan(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        flag = num == -1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool LessThanOrEqual(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        flag = num != 1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool NotEquals(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        flag = internalCompare(ts1, ts2) != 0 ? true : false;
        ifxTrace?.ApiExit();
        return flag;
    }

    public InformixTimeSpan Multiply(decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        InformixTimeSpan result = this * val;
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixTimeSpan Negate()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixTimeSpan result = new InformixTimeSpan(-ticks, StartTimeUnit, EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan Parse(string _szTime)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_szTime);
        InformixTimeSpan result = Parse(_szTime, InformixTimeUnit.Day, InformixTimeUnit.Fraction5);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan Parse(string _szTime, InformixTimeUnit start, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_szTime, start, end);
        InformixTimeUnit ifxTimeUnit = end;
        if (end == InformixTimeUnit.Fraction)
        {
            ifxTimeUnit = InformixTimeUnit.Fraction3;
        }
        Qualifier.Validate(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, start, ifxTimeUnit);
        IntervalDateTime iDT = new IntervalDateTime(_szTime, start, ifxTimeUnit);
        ValidateANSIFormatForParse(iDT);
        InformixTimeSpan result = new InformixTimeSpan(iDT);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan Parse(string _szTime, string format, InformixTimeUnit start, InformixTimeUnit end)
    {
        byte[] array = new byte[25];
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(_szTime, format, start, end);
        InformixTimeUnit ifxTimeUnit = end;
        if (end == InformixTimeUnit.Fraction)
        {
            ifxTimeUnit = InformixTimeUnit.Fraction3;
        }
        Qualifier.Validate(InformixTimeUnit.Day, InformixTimeUnit.Fraction5, start, ifxTimeUnit);
        short num = Qualifier.IntervalEncode(start, ifxTimeUnit);
        CNativeBuffer cNativeBuffer = new CNativeBuffer(25);
        cNativeBuffer.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
        Informix32.RetCode retCode = Interop.Odbc.incvfmtasc(_szTime, format, cNativeBuffer.Address);
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
        IntervalDateTime iDT = new IntervalDateTime(text, start, ifxTimeUnit);
        InformixTimeSpan result = new InformixTimeSpan(iDT);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixTimeSpan Subtract(InformixTimeSpan ts)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts);
        if (IsNull || ts.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixTimeSpan result = this - ts;
        ifxTrace?.ApiExit();
        return result;
    }

    public override string ToString()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsNull)
        {
            return "Null";
        }
        if (IsNull)
        {
            return null;
        }
        string result = IDTUserQualified.ToString();
        ifxTrace?.ApiExit();
        return result;
    }

    public string ToString(string format)
    {
        byte[] array = new byte[format.Length + 50];
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(format);
        if (IsNull)
        {
            return "Null";
        }
        if (IsNull)
        {
            return null;
        }
        string inbuf = IDTUserQualified.ToString();
        short num = Qualifier.IntervalEncode(IDTUserQualified.startTU, IDTUserQualified.endTU);
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
        string text = aSCIIEncoding.GetString(array, 0, array.Length);
        string[] array2 = text.Split('\0');
        if (array2.Length != 0)
        {
            text = array2[0];
        }
        ifxTrace?.ApiExit();
        return text;
    }

    public static InformixTimeSpan operator +(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        if (ts1.IsNull || ts2.IsNull)
        {
            throw new ArgumentNullException();
        }
        decimal num;
        try
        {
            num = ts1.ticks + ts2.ticks;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        InformixTimeUnit start = Qualifier.GreaterThan(ts1.StartTimeUnit, ts2.StartTimeUnit) ? ts1.StartTimeUnit : ts2.StartTimeUnit;
        InformixTimeUnit end = Qualifier.GreaterThan(ts2.EndTimeUnit, ts1.EndTimeUnit) ? ts1.EndTimeUnit : ts2.EndTimeUnit;
        IntervalDateTime iDT = new IntervalDateTime(start, end, num);
        ValidateValue(iDT);
        InformixTimeSpan result = new InformixTimeSpan(iDT);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan operator /(InformixTimeSpan ts, decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts, val);
        if (ts.IsNull)
        {
            throw new ArgumentNullException();
        }
        if (val == 0m)
        {
            throw new DivideByZeroException();
        }
        decimal num;
        try
        {
            num = ts.ticks / val;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        IntervalDateTime iDT = new IntervalDateTime(ts.StartTimeUnit, ts.EndTimeUnit, num);
        ValidateValue(iDT);
        InformixTimeSpan result = new InformixTimeSpan(iDT);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan operator *(InformixTimeSpan ts, decimal val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts, val);
        if (ts.IsNull)
        {
            throw new ArgumentNullException();
        }
        decimal num;
        try
        {
            num = ts.ticks * val;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        IntervalDateTime iDT = new IntervalDateTime(ts.StartTimeUnit, ts.EndTimeUnit, num);
        ValidateValue(iDT);
        InformixTimeSpan result = new InformixTimeSpan(iDT);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan operator -(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        if (ts1.IsNull || ts2.IsNull)
        {
            throw new ArgumentNullException();
        }
        decimal num;
        try
        {
            num = ts1.ticks - ts2.ticks;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        InformixTimeUnit start = Qualifier.GreaterThan(ts1.StartTimeUnit, ts2.StartTimeUnit) ? ts1.StartTimeUnit : ts2.StartTimeUnit;
        InformixTimeUnit end = Qualifier.GreaterThan(ts2.EndTimeUnit, ts1.EndTimeUnit) ? ts1.EndTimeUnit : ts2.EndTimeUnit;
        IntervalDateTime iDT = new IntervalDateTime(start, end, num);
        ValidateValue(iDT);
        InformixTimeSpan result = new InformixTimeSpan(iDT);
        ifxTrace?.ApiExit();
        return result;
    }

    public static decimal operator /(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        if (ts1.IsNull || ts2.IsNull)
        {
            throw new ArgumentNullException();
        }
        decimal result = ts1.ticks / ts2.ticks;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator ==(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        bool result = internalCompare(ts1, ts2) == 0;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator >(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        bool result = num == 1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator >=(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        bool result = num != -1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator !=(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        bool result = internalCompare(ts1, ts2) != 0 ? true : false;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator <(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        bool result = num == -1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator <=(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts1, ts2);
        int num = internalCompare(ts1, ts2);
        bool result = num != 1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan operator -(InformixTimeSpan ts)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts);
        if (ts.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixTimeSpan result = new InformixTimeSpan(-ts.ticks, ts.StartTimeUnit, ts.EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan operator +(InformixTimeSpan ts)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ts);
        if (ts.IsNull)
        {
            throw new ArgumentNullException();
        }
        InformixTimeSpan result = new InformixTimeSpan(ts.ticks, ts.StartTimeUnit, ts.EndTimeUnit);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator InformixTimeSpan(string val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        InformixTimeSpan result = Parse(val, InformixTimeUnit.Day, InformixTimeUnit.Fraction5);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator TimeSpan(InformixTimeSpan x)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(x);
        TimeSpan result = new TimeSpan((long)x.ticks);
        ifxTrace?.ApiExit();
        return result;
    }

    private static void ValidateValue(IntervalDateTime IDT)
    {
        int[] array = new int[4] { 999999999, -999999999, 99999, -99999 };
        switch (Qualifier.TimeUnitToOffset[(short)IDT.startTU])
        {
            case 6:
                if (IDT.Fraction > array[2] || IDT.Fraction < array[3])
                {
                    throw new OverflowException();
                }
                break;
            case 2:
                if (IDT.Day > array[0] || IDT.Day < array[1])
                {
                    throw new OverflowException();
                }
                break;
            case 3:
                if (IDT.Hour > array[0] || IDT.Hour < array[1])
                {
                    throw new OverflowException();
                }
                break;
            case 4:
                if (IDT.Minute > array[0] || IDT.Minute < array[1])
                {
                    throw new OverflowException();
                }
                break;
            case 5:
                if (IDT.Second > array[0] || IDT.Second < array[1])
                {
                    throw new OverflowException();
                }
                break;
        }
    }

    private static void ValidateANSIFormatForParse(IntervalDateTime IDT)
    {
        int[] array = new int[4] { 999999999, -999999999, 99999, -99999 };
        short num = Qualifier.TimeUnitToOffset[(short)IDT.startTU];
        short num2 = Qualifier.TimeUnitToOffset[(short)IDT.endTU];
        while (num <= num2)
        {
            switch (num)
            {
                case 2:
                    if (IDT.Day > array[0] || IDT.Day < array[1])
                    {
                        throw new OverflowException();
                    }
                    break;
                case 3:
                    if (num == Qualifier.TimeUnitToOffset[(short)IDT.startTU])
                    {
                        if (IDT.Hour > array[0] || IDT.Hour < array[1])
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (IDT.Hour > 23 || IDT.Hour < -23)
                    {
                        throw new OverflowException();
                    }
                    break;
                case 4:
                    if (num == Qualifier.TimeUnitToOffset[(short)IDT.startTU])
                    {
                        if (IDT.Minute > array[0] || IDT.Minute < array[1])
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (IDT.Minute > 59 || IDT.Minute < -59)
                    {
                        throw new OverflowException();
                    }
                    break;
                case 5:
                    if (num == Qualifier.TimeUnitToOffset[(short)IDT.startTU])
                    {
                        if (IDT.Second > array[0] || IDT.Second < array[1])
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (IDT.Second > 59 || IDT.Second < -59)
                    {
                        throw new OverflowException();
                    }
                    break;
                case 6:
                    if (15 == (short)IDT.endTU)
                    {
                        if (IDT.Fraction > array[2] || IDT.Fraction < array[3])
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (14 == (short)IDT.endTU)
                    {
                        if (IDT.Fraction > 9999 || IDT.Fraction < -9999)
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (13 == (short)IDT.endTU || 16 == (short)IDT.endTU)
                    {
                        if (IDT.Fraction > 999 || IDT.Fraction < -999)
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (12 == (short)IDT.endTU)
                    {
                        if (IDT.Fraction > 99 || IDT.Fraction < -99)
                        {
                            throw new OverflowException();
                        }
                    }
                    else if (11 == (short)IDT.endTU && (IDT.Fraction > 9 || IDT.Fraction < -9))
                    {
                        throw new OverflowException();
                    }
                    break;
            }
            num++;
        }
    }

    private static int internalCompare(InformixTimeSpan ts1, InformixTimeSpan ts2)
    {
        int result = 0;
        if (ts1.IsNull && ts2.IsNull)
        {
            result = 0;
        }
        else if (ts1.IsNull)
        {
            result = -1;
        }
        else if (ts2.IsNull)
        {
            result = 1;
        }
        else if (ts1.ticks > ts2.ticks)
        {
            result = 1;
        }
        else if (ts1.ticks < ts2.ticks)
        {
            result = -1;
        }
        else if (ts1.ticks == ts2.ticks)
        {
            result = 0;
        }
        return result;
    }
}
