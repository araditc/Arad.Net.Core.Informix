using System;
using System.Data.SqlTypes;



namespace Arad.Net.Core.Informix;
public struct InformixDateTime : IComparable, INullable
{
    internal enum Limits
    {
        YearMax = 9999,
        YearMin = 1,
        MonthMax = 12,
        MonthMin = 1,
        DayMax = 31,
        DayMin = 1,
        HourMax = 23,
        HourMin = 0,
        MinuteMax = 59,
        MinuteMin = 0,
        SecondMax = 59,
        SecondMin = 0
    }

    internal IntervalDateTime intervalDateTime;

    internal bool isNull;

    internal string toStringVal;

    private const short DTIME_T_SIZE = 24;

    internal const short MAX_DATETIME_STR_LEN = 26;

    private static CNativeBuffer genlibDTVal1 = new CNativeBuffer(24);

    private static CNativeBuffer genlibDTVal2 = new CNativeBuffer(24);

    private static CNativeBuffer genlibInvVal1 = new CNativeBuffer(24);

    internal long ticks;

    public static readonly InformixDateTime MaxValue = new InformixDateTime(9999, 12, 31, 23, 59, 59, 99999, InformixTimeUnit.Fraction5);

    public static readonly InformixDateTime MinValue = new InformixDateTime(1, 1, 1, 0, 0, 0, 0, InformixTimeUnit.Fraction5);

    public static readonly InformixDateTime Null = new InformixDateTime(nullFlag: true);

    private static readonly InformixDateTime Default = new InformixDateTime(1200, 1, 1, 0, 0, 0, 0, InformixTimeUnit.Fraction5);

    public InformixDateTime Date
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            InformixDateTime result = new InformixDateTime(intervalDateTime.Year, intervalDateTime.Month, intervalDateTime.Day, InformixTimeUnit.Day);
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public int Day
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.Day;
        }
    }

    public InformixTimeUnit EndTimeUnit
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.endTU;
        }
    }

    public int Hour
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.Hour;
        }
    }

    public bool IsNull
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return isNull;
        }
    }

    public int Millisecond
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            int result = (int)decimal.Multiply(intervalDateTime.Fraction, (decimal)Math.Pow(10.0, 13 - (short)intervalDateTime.endTU));
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public int Minute
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.Minute;
        }
    }

    public int Month
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.Month;
        }
    }

    public static InformixDateTime Now
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            InformixDateTime result = new InformixDateTime(DateTime.Now);
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public int Second
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.Second;
        }
    }

    public InformixTimeUnit StartTimeUnit
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.startTU;
        }
    }

    public long Ticks
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            if (long.MinValue == ticks)
            {
                long num = (int)decimal.Multiply(intervalDateTime.Fraction, (decimal)Math.Pow(10.0, 16 - (short)intervalDateTime.endTU + 1));
                DateTime dateTime = new DateTime(intervalDateTime.Year, intervalDateTime.Month, intervalDateTime.Day, intervalDateTime.Hour, intervalDateTime.Minute, intervalDateTime.Second);
                if (num > 0)
                {
                    dateTime = dateTime.AddTicks(num);
                }
                ticks = dateTime.Ticks;
            }
            ifxTrace?.ApiExit();
            return ticks;
        }
    }

    public static InformixDateTime Today
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            DateTime today = DateTime.Today;
            InformixDateTime result = new InformixDateTime(today.Year, today.Month, today.Day, InformixTimeUnit.Day);
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public int Year
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            ifxTrace?.ApiExit();
            return intervalDateTime.Year;
        }
    }

    public InformixDateTime(long ticks)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ticks);
        DateTime dt = new DateTime(ticks);
        intervalDateTime = new IntervalDateTime(dt);
        isNull = false;
        toStringVal = null;
        this.ticks = ticks;
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(DateTime dt)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(dt);
        intervalDateTime = new IntervalDateTime(dt);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits, InformixTimeUnit unit)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits, unit);
        InformixTimeUnit end;
        InformixTimeUnit start = end = unit;
        if (unit >= InformixTimeUnit.Fraction1)
        {
            start = InformixTimeUnit.Fraction;
            if (unit == InformixTimeUnit.Fraction)
            {
                end = InformixTimeUnit.Fraction3;
            }
        }
        Qualifier.Validate(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, start, end);
        intervalDateTime = new IntervalDateTime(start, end, new object[1] { numUnits });
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits1, int numUnits2, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits1, numUnits2, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, 2, end);
        intervalDateTime = new IntervalDateTime(start, end, numUnits1, numUnits2);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits1, int numUnits2, int numUnits3, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits1, numUnits2, numUnits3, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, 3, end);
        intervalDateTime = new IntervalDateTime(start, end, numUnits1, numUnits2, numUnits3);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits1, int numUnits2, int numUnits3, int numUnits4, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits1, numUnits2, numUnits3, numUnits4, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, 4, end);
        intervalDateTime = new IntervalDateTime(start, end, numUnits1, numUnits2, numUnits3, numUnits4);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits1, int numUnits2, int numUnits3, int numUnits4, int numUnits5, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits1, numUnits2, numUnits3, numUnits4, numUnits5, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, 5, end);
        intervalDateTime = new IntervalDateTime(start, end, numUnits1, numUnits2, numUnits3, numUnits4, numUnits5);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits1, int numUnits2, int numUnits3, int numUnits4, int numUnits5, int numUnits6, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits1, numUnits2, numUnits3, numUnits4, numUnits5, numUnits6, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, 6, end);
        intervalDateTime = new IntervalDateTime(start, end, numUnits1, numUnits2, numUnits3, numUnits4, numUnits5, numUnits6);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    public InformixDateTime(int numUnits1, int numUnits2, int numUnits3, int numUnits4, int numUnits5, int numUnits6, int numUnits7, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(numUnits1, numUnits2, numUnits3, numUnits4, numUnits5, numUnits6, numUnits7, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        InformixTimeUnit start = Qualifier.ValidateEndTUDetermineStartTU(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, 7, end);
        intervalDateTime = new IntervalDateTime(start, end, numUnits1, numUnits2, numUnits3, numUnits4, numUnits5, numUnits6, numUnits7);
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
        ifxTrace?.ApiExit();
    }

    internal InformixDateTime(bool nullFlag)
    {
        isNull = nullFlag;
        if (nullFlag)
        {
            toStringVal = "Null";
        }
        else
        {
            toStringVal = null;
        }
        intervalDateTime = new IntervalDateTime(t: false);
        ticks = long.MinValue;
    }

    internal InformixDateTime(IntervalDateTime idt)
    {
        intervalDateTime = idt;
        isNull = false;
        toStringVal = null;
        ticks = long.MinValue;
        ValidateRange();
        AssignDefaultValues();
        ValidateConsistency();
    }

    public InformixDateTime Add(InformixTimeSpan ifxTS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxTS);
        if (isNull || ifxTS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(this, ifxTS);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime Add(InformixMonthSpan ifxMS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxMS);
        if (isNull || ifxMS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(this, ifxMS);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddDays(double days)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(days);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeUnit start = InformixTimeUnit.Day;
        decimal num = (decimal)days * 864000000000m;
        if (intervalDateTime.startTU > InformixTimeUnit.Day)
        {
            start = intervalDateTime.startTU;
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixTimeSpan(num, start, intervalDateTime.endTU), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddHours(double hours)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(hours);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeUnit start = InformixTimeUnit.Day;
        decimal num = (decimal)hours * 36000000000m;
        if (intervalDateTime.startTU > InformixTimeUnit.Day)
        {
            start = intervalDateTime.startTU;
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixTimeSpan(num, start, intervalDateTime.endTU), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddMilliseconds(double milliseconds)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(milliseconds);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeUnit start = InformixTimeUnit.Day;
        decimal num = (decimal)milliseconds * 10000m;
        if (intervalDateTime.startTU > InformixTimeUnit.Day)
        {
            start = intervalDateTime.startTU;
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixTimeSpan(num, start, intervalDateTime.endTU), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddMinutes(double minutes)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(minutes);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeUnit start = InformixTimeUnit.Day;
        decimal num = (decimal)minutes * 600000000m;
        if (intervalDateTime.startTU > InformixTimeUnit.Day)
        {
            start = intervalDateTime.startTU;
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixTimeSpan(num, start, intervalDateTime.endTU), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddMonths(int months)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(months);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixMonthSpan(months, InformixTimeUnit.Month), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddSeconds(double seconds)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(seconds);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeUnit start = InformixTimeUnit.Day;
        decimal num = (decimal)seconds * 10000000m;
        if (intervalDateTime.startTU > InformixTimeUnit.Day)
        {
            start = intervalDateTime.startTU;
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixTimeSpan(num, start, intervalDateTime.endTU), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddTicks(long ticks)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ticks);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeUnit start = InformixTimeUnit.Day;
        if (intervalDateTime.startTU > InformixTimeUnit.Day)
        {
            start = intervalDateTime.startTU;
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixTimeSpan(ticks, start, intervalDateTime.endTU), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime AddYears(int years)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(years);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(intervalVal: new InformixMonthSpan(years, InformixTimeUnit.Year), ifxDT: this);
        ifxTrace?.ApiExit();
        return result;
    }

    public static int Compare(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int result = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        ifxTrace?.ApiExit();
        return result;
    }

    public int CompareTo(object obj)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        if (obj.GetType() != typeof(InformixDateTime))
        {
            throw new ArgumentException();
        }
        int result = internalCompare(intervalDateTime, ((InformixDateTime)obj).intervalDateTime);
        ifxTrace?.ApiExit();
        return result;
    }

    public static int DaysInMonth(int year, int month)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(year, month);
        int result = DateTime.DaysInMonth(year, month);
        ifxTrace?.ApiExit();
        return result;
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(obj);
        flag = obj != null && !(typeof(InformixDateTime) != obj.GetType()) && internalCompare(intervalDateTime, ((InformixDateTime)obj).intervalDateTime) == 0;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool Equals(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        bool result = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime) == 0;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool GreaterThan(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        flag = num == 1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool GreaterThanOrEqual(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        flag = num != -1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool LessThan(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        flag = num == -1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool LessThanOrEqual(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        flag = num != 1;
        ifxTrace?.ApiExit();
        return flag;
    }

    public static bool NotEquals(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        bool flag = false;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        flag = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime) != 0 ? true : false;
        ifxTrace?.ApiExit();
        return flag;
    }

    public override int GetHashCode()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        int hashCode = Ticks.GetHashCode();
        ifxTrace?.ApiExit();
        return hashCode;
    }

    public static InformixDateTime Parse(string dateTimeString)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(dateTimeString);
        IntervalDateTime idt = AnsiDateTime2IntervalDateTime(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, dateTimeString);
        InformixDateTime result = new InformixDateTime(idt);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime Parse(string dateTimeString, InformixTimeUnit start, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(dateTimeString, start, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Qualifier.Validate(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, start, end);
        IntervalDateTime idt = AnsiDateTime2IntervalDateTime(start, end, dateTimeString);
        InformixDateTime result = new InformixDateTime(idt);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime Parse(string dateTimeString, string format, InformixTimeUnit start, InformixTimeUnit end)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(dateTimeString, format, start, end);
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        Qualifier.Validate(InformixTimeUnit.Year, InformixTimeUnit.Fraction5, start, end);
        IntervalDateTime idt = FormatDateTime2IntervalDateTime(start, end, dateTimeString, format);
        InformixDateTime result = new InformixDateTime(idt);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixTimeSpan Subtract(InformixDateTime ifxDT)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT);
        if (isNull || ifxDT.isNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeSpan @null = InformixTimeSpan.Null;
        @null = (InformixTimeSpan)Subtract2DateTimeValues(this, ifxDT, @null.GetType());
        ifxTrace?.ApiExit();
        return @null;
    }

    public InformixMonthSpan SubtractYearMonth(InformixDateTime ifxDT)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT);
        if (isNull || ifxDT.isNull)
        {
            throw new InvalidOperationException();
        }
        InformixMonthSpan @null = InformixMonthSpan.Null;
        @null = (InformixMonthSpan)Subtract2DateTimeValues(this, ifxDT, @null.GetType());
        ifxTrace?.ApiExit();
        return @null;
    }

    public InformixDateTime Subtract(InformixTimeSpan ifxTS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxTS);
        if (isNull || ifxTS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = SubtractIntervalFromDateTime(this, ifxTS);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixDateTime Subtract(InformixMonthSpan ifxMS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxMS);
        if (isNull || ifxMS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = SubtractIntervalFromDateTime(this, ifxMS);
        ifxTrace?.ApiExit();
        return result;
    }

    public override string ToString()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (toStringVal == null)
        {
            toStringVal = intervalDateTime.DateTimeToString();
        }
        ifxTrace?.ApiExit();
        return toStringVal;
    }

    public string ToString(string format)
    {
        string text = "";
        byte[] array = new byte[format.Length + 26];
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(format);
        if (isNull)
        {
            text = "Null";
        }
        else
        {
            string inbuf = intervalDateTime.ToString();
            short num = Qualifier.DateTimeEncode(intervalDateTime.startTU, intervalDateTime.endTU);
            genlibDTVal1.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
            retCode = Interop.Odbc.dtcvasc(inbuf, genlibDTVal1.Address);
            if (retCode != 0)
            {
                throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
            }
            retCode = Interop.Odbc.dttofmtasc(genlibDTVal1.Address, array, array.Length, format);
            if (retCode != 0)
            {
                throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
            }
            text = Informix32.encoder.GetString(array, 0, array.Length);
            text = text.Trim('\0');
        }
        ifxTrace?.ApiExit();
        return text;
    }

    private static InformixDateTime AddIntervalToDateTime(InformixDateTime ifxDT, object intervalVal)
    {
        Type type = intervalVal.GetType();
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        IntervalDateTime intervalDateTime;
        if (type == typeof(InformixMonthSpan))
        {
            intervalDateTime = ((InformixMonthSpan)intervalVal).IDT;
        }
        else
        {
            if (!(type == typeof(InformixTimeSpan)))
            {
                throw new ArgumentException();
            }
            intervalDateTime = ((InformixTimeSpan)intervalVal).IDTUserQualified;
        }
        genlibDTVal1.MarshalToNative(ifxDT.intervalDateTime, Informix32.GENLIB_TYPE.DTTIME_T);
        genlibInvVal1.MarshalToNative(intervalDateTime, Informix32.GENLIB_TYPE.INTRVL_T);
        retCode = Interop.Odbc.dtaddinv(genlibDTVal1.Address, genlibInvVal1.Address, genlibDTVal2.Address);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        IntervalDateTime idt = (IntervalDateTime)genlibDTVal2.MarshalToManaged(Informix32.GENLIB_TYPE.DTTIME_T);
        return new InformixDateTime(idt);
    }

    private void AssignDefaultValues()
    {
        if (intervalDateTime.Year == 0)
        {
            intervalDateTime.Year = Default.Year;
        }
        if (intervalDateTime.Month == 0)
        {
            intervalDateTime.Month = Default.Month;
        }
        if (intervalDateTime.Day == 0)
        {
            intervalDateTime.Day = Default.Day;
        }
    }

    private static IntervalDateTime AnsiDateTime2IntervalDateTime(InformixTimeUnit start, InformixTimeUnit end, string dateTimeString)
    {
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        short num = Qualifier.DateTimeEncode(start, end);
        genlibDTVal1.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
        retCode = Interop.Odbc.dtcvasc(dateTimeString, genlibDTVal1.Address);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        return (IntervalDateTime)genlibDTVal1.MarshalToManaged(Informix32.GENLIB_TYPE.DTTIME_T);
    }

    private static IntervalDateTime FormatDateTime2IntervalDateTime(InformixTimeUnit start, InformixTimeUnit end, string dateTimeString, string format)
    {
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        short num = Qualifier.DateTimeEncode(start, end);
        genlibDTVal1.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
        retCode = Interop.Odbc.dtcvfmtasc(dateTimeString, format, genlibDTVal1.Address);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        return (IntervalDateTime)genlibDTVal1.MarshalToManaged(Informix32.GENLIB_TYPE.DTTIME_T);
    }

    private static int internalCompare(IntervalDateTime idt1, IntervalDateTime idt2)
    {
        int result = 0;
        if (idt1.isNull && idt2.isNull)
        {
            result = 0;
        }
        else if (idt1.isNull)
        {
            result = -1;
        }
        else if (idt2.isNull)
        {
            result = 1;
        }
        else if (idt1.Year < idt2.Year)
        {
            result = -1;
        }
        else if (idt1.Year > idt2.Year)
        {
            result = 1;
        }
        else if (idt1.Month < idt2.Month)
        {
            result = -1;
        }
        else if (idt1.Month > idt2.Month)
        {
            result = 1;
        }
        else if (idt1.Day < idt2.Day)
        {
            result = -1;
        }
        else if (idt1.Day > idt2.Day)
        {
            result = 1;
        }
        else if (idt1.Hour < idt2.Hour)
        {
            result = -1;
        }
        else if (idt1.Hour > idt2.Hour)
        {
            result = 1;
        }
        else if (idt1.Minute < idt2.Minute)
        {
            result = -1;
        }
        else if (idt1.Minute > idt2.Minute)
        {
            result = 1;
        }
        else if (idt1.Second < idt2.Second)
        {
            result = -1;
        }
        else if (idt1.Second > idt2.Second)
        {
            result = 1;
        }
        else if (idt1.FractionalSeconds < idt2.FractionalSeconds)
        {
            result = -1;
        }
        else if (idt1.FractionalSeconds > idt2.FractionalSeconds)
        {
            result = 1;
        }
        return result;
    }

    private static InformixDateTime SubtractIntervalFromDateTime(InformixDateTime ifxDT, object intervalVal)
    {
        Type type = intervalVal.GetType();
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        IntervalDateTime intervalDateTime;
        if (type == typeof(InformixMonthSpan))
        {
            intervalDateTime = ((InformixMonthSpan)intervalVal).IDT;
        }
        else
        {
            if (!(type == typeof(InformixTimeSpan)))
            {
                throw new ArgumentException();
            }
            intervalDateTime = ((InformixTimeSpan)intervalVal).IDTUserQualified;
        }
        genlibDTVal1.MarshalToNative(ifxDT.intervalDateTime, Informix32.GENLIB_TYPE.DTTIME_T);
        genlibInvVal1.MarshalToNative(intervalDateTime, Informix32.GENLIB_TYPE.INTRVL_T);
        retCode = Interop.Odbc.dtsubinv(genlibDTVal1.Address, genlibInvVal1.Address, genlibDTVal2.Address);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        IntervalDateTime idt = (IntervalDateTime)genlibDTVal2.MarshalToManaged(Informix32.GENLIB_TYPE.DTTIME_T);
        return new InformixDateTime(idt);
    }

    private static object Subtract2DateTimeValues(InformixDateTime ifxDT1, InformixDateTime ifxDT2, Type intervalType)
    {
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        object result = null;
        short num;
        if (intervalType == typeof(InformixMonthSpan))
        {
            num = Qualifier.IntervalEncode(InformixTimeUnit.Year, InformixTimeUnit.Month);
        }
        else
        {
            if (!(intervalType == typeof(InformixTimeSpan)))
            {
                throw new ArgumentException();
            }
            num = Qualifier.IntervalEncode(InformixTimeUnit.Day, InformixTimeUnit.Fraction5);
        }
        IntervalDateTime intervalDateTime = ifxDT1.intervalDateTime;
        IntervalDateTime intervalDateTime2 = ifxDT2.intervalDateTime;
        if (intervalDateTime.startTU < intervalDateTime2.startTU)
        {
            intervalDateTime2.startTU = intervalDateTime.startTU;
        }
        genlibDTVal1.MarshalToNative(intervalDateTime, Informix32.GENLIB_TYPE.DTTIME_T);
        genlibDTVal2.MarshalToNative(intervalDateTime2, Informix32.GENLIB_TYPE.DTTIME_T);
        genlibInvVal1.MarshalToNative(num, Informix32.SQL_C.SSHORT, 0);
        retCode = Interop.Odbc.dtsub(genlibDTVal1.Address, genlibDTVal2.Address, genlibInvVal1.Address);
        if (retCode != 0)
        {
            throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
        }
        IntervalDateTime intervalDateTime3 = (IntervalDateTime)genlibInvVal1.MarshalToManaged(Informix32.GENLIB_TYPE.INTRVL_T);
        if (intervalType == typeof(InformixMonthSpan))
        {
            result = new InformixMonthSpan(intervalDateTime3);
        }
        else if (intervalType == typeof(InformixTimeSpan))
        {
            result = new InformixTimeSpan(intervalDateTime3);
        }
        return result;
    }

    private void ValidateRange()
    {
        int[] array = new int[5] { 9, 99, 999, 9999, 99999 };
        if (intervalDateTime.startTU <= InformixTimeUnit.Year && intervalDateTime.endTU >= InformixTimeUnit.Year && (intervalDateTime.Year < 1 || intervalDateTime.Year > 9999))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (intervalDateTime.startTU <= InformixTimeUnit.Month && intervalDateTime.endTU >= InformixTimeUnit.Month && (intervalDateTime.Month < 1 || intervalDateTime.Month > 12))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (intervalDateTime.startTU <= InformixTimeUnit.Day && intervalDateTime.endTU >= InformixTimeUnit.Day && (intervalDateTime.Day < 1 || intervalDateTime.Day > 31))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (intervalDateTime.startTU <= InformixTimeUnit.Hour && intervalDateTime.endTU >= InformixTimeUnit.Hour && (intervalDateTime.Hour < 0 || intervalDateTime.Hour > 23))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (intervalDateTime.startTU <= InformixTimeUnit.Minute && intervalDateTime.endTU >= InformixTimeUnit.Minute && (intervalDateTime.Minute < 0 || intervalDateTime.Minute > 59))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (intervalDateTime.startTU <= InformixTimeUnit.Second && intervalDateTime.endTU >= InformixTimeUnit.Second && (intervalDateTime.Second < 0 || intervalDateTime.Second > 59))
        {
            throw new ArgumentOutOfRangeException();
        }
        int num = (short)intervalDateTime.endTU - 11;
        if (num >= 0 && (intervalDateTime.Fraction < 0 || intervalDateTime.Fraction > array[num]))
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    private void ValidateConsistency()
    {
        try
        {
            DateTime dateTime = new DateTime(intervalDateTime.Year, intervalDateTime.Month, intervalDateTime.Day);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException();
        }
        catch (ArgumentException)
        {
            throw new ArgumentException();
        }
    }

    public static bool operator ==(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        bool result = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime) == 0;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator !=(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        bool result = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime) != 0 ? true : false;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator >(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        bool result = num == 1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator >=(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        bool result = num != -1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator <(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        bool result = num == -1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static bool operator <=(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        int num = internalCompare(ifxDT1.intervalDateTime, ifxDT2.intervalDateTime);
        bool result = num != 1;
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime operator +(InformixDateTime ifxDT, InformixTimeSpan ifxTS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT, ifxTS);
        if (ifxDT.isNull || ifxTS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(ifxDT, ifxTS);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime operator +(InformixTimeSpan ifxTS, InformixDateTime ifxDT)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxTS, ifxDT);
        if (ifxDT.isNull || ifxTS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(ifxDT, ifxTS);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime operator +(InformixDateTime ifxDT, InformixMonthSpan ifxMS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT, ifxMS);
        if (ifxDT.isNull || ifxMS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(ifxDT, ifxMS);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime operator +(InformixMonthSpan ifxMS, InformixDateTime ifxDT)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxMS, ifxDT);
        if (ifxDT.isNull || ifxMS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = AddIntervalToDateTime(ifxDT, ifxMS);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixTimeSpan operator -(InformixDateTime ifxDT1, InformixDateTime ifxDT2)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT1, ifxDT2);
        if (ifxDT1.isNull || ifxDT2.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixTimeSpan @null = InformixTimeSpan.Null;
        @null = (InformixTimeSpan)Subtract2DateTimeValues(ifxDT1, ifxDT2, @null.GetType());
        ifxTrace?.ApiExit();
        return @null;
    }

    public static InformixDateTime operator -(InformixDateTime ifxDT, InformixTimeSpan ifxTS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT, ifxTS);
        if (ifxDT.isNull || ifxTS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = SubtractIntervalFromDateTime(ifxDT, ifxTS);
        ifxTrace?.ApiExit();
        return result;
    }

    public static InformixDateTime operator -(InformixDateTime ifxDT, InformixMonthSpan ifxMS)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(ifxDT, ifxMS);
        if (ifxDT.isNull || ifxMS.IsNull)
        {
            throw new InvalidOperationException();
        }
        InformixDateTime result = SubtractIntervalFromDateTime(ifxDT, ifxMS);
        ifxTrace?.ApiExit();
        return result;
    }

    public static explicit operator DateTime(InformixDateTime x)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (x.isNull)
        {
            throw new InvalidOperationException();
        }
        DateTime result = new DateTime(x.Ticks);
        ifxTrace?.ApiExit();
        return result;
    }
}
