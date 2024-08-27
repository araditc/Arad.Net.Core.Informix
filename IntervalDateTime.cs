using System;
using System.Text;


namespace Arad.Net.Core.Informix;

internal struct IntervalDateTime
{
    private static string[] FieldFormats = new string[11]
    {
        "0000", "00", "00", "00", "00", "00", "0", "00", "000", "0000",
        "00000"
    };

    internal int Year;

    internal int Month;

    internal int Day;

    internal int Hour;

    internal int Minute;

    internal int Second;

    internal int Fraction;

    internal double FractionalSeconds;

    internal InformixTimeUnit startTU;

    internal InformixTimeUnit endTU;

    internal bool isNull;

    internal static readonly IntervalDateTime Null = new IntervalDateTime(t: true);

    private static char[] TimeUnitSeparators = new char[6] { '-', '-', ' ', ':', ':', '.' };

    internal IntervalDateTime(bool t)
    {
        isNull = t;
        Year = 0;
        Month = 0;
        Day = 0;
        Hour = 0;
        Minute = 0;
        Second = 0;
        Fraction = 0;
        FractionalSeconds = 0.0;
        startTU = InformixTimeUnit.Year;
        endTU = InformixTimeUnit.Month;
    }

    internal IntervalDateTime(InformixTimeUnit start, InformixTimeUnit end, params object[] obj)
    {
        Year = 0;
        Month = 0;
        Day = 0;
        Hour = 0;
        Minute = 0;
        Second = 0;
        Fraction = 0;
        isNull = false;
        FractionalSeconds = 0.0;
        short num = Qualifier.TimeUnitToOffset[(short)start];
        short num2 = Qualifier.TimeUnitToOffset[(short)end];
        startTU = start;
        endTU = end;
        if (obj.Length != num2 - num + 1)
        {
            throw new ArgumentException();
        }
        short num3 = 0;
        while (num3 < obj.Length)
        {
            switch (num)
            {
                case 0:
                    Year = (int)obj.GetValue(num3);
                    break;
                case 1:
                    Month = (int)obj.GetValue(num3);
                    break;
                case 2:
                    Day = (int)obj.GetValue(num3);
                    break;
                case 3:
                    Hour = (int)obj.GetValue(num3);
                    break;
                case 4:
                    Minute = (int)obj.GetValue(num3);
                    break;
                case 5:
                    Second = (int)obj.GetValue(num3);
                    break;
                case 6:
                    Fraction = (int)obj.GetValue(num3);
                    FractionalSeconds = CalculateFractionalSeconds(Fraction, end);
                    break;
            }
            num3++;
            num++;
        }
    }

    internal IntervalDateTime(DateTime dt)
    {
        startTU = InformixTimeUnit.Year;
        endTU = InformixTimeUnit.Fraction5;
        Year = dt.Year;
        Month = dt.Month;
        Day = dt.Day;
        Hour = dt.Hour;
        Minute = dt.Minute;
        Second = dt.Second;
        isNull = false;
        long num = dt.Ticks % 10000000;
        Fraction = (int)(num / 100);
        FractionalSeconds = CalculateFractionalSeconds(Fraction, endTU);
    }

    internal IntervalDateTime(InformixTimeUnit start, InformixTimeUnit end, decimal ticks)
    {
        short num = Qualifier.TimeUnitToOffset[(short)start];
        short num2 = Qualifier.TimeUnitToOffset[(short)end];
        Year = 0;
        Month = 0;
        Day = 0;
        Hour = 0;
        Minute = 0;
        Second = 0;
        Fraction = 0;
        isNull = false;
        FractionalSeconds = 0.0;
        startTU = start;
        endTU = end;
        short num3 = num;
        decimal num4 = ticks;
        while (num3 <= num2)
        {
            if (num3 == Qualifier.TimeUnitToOffset[4])
            {
                Day = (int)decimal.Divide(num4, 864000000000m);
                num4 = decimal.Remainder(num4, 864000000000m);
            }
            else if (num3 == Qualifier.TimeUnitToOffset[6])
            {
                Hour = (int)decimal.Divide(num4, 36000000000m);
                num4 = decimal.Remainder(num4, 36000000000m);
            }
            else if (num3 == Qualifier.TimeUnitToOffset[8])
            {
                Minute = (int)decimal.Divide(num4, 600000000m);
                num4 = decimal.Remainder(num4, 600000000m);
            }
            else if (num3 == Qualifier.TimeUnitToOffset[10])
            {
                Second = (int)decimal.Divide(num4, 10000000m);
                num4 = decimal.Remainder(num4, 10000000m);
            }
            else if (num3 == Qualifier.TimeUnitToOffset[16])
            {
                Fraction = (int)(num4 / (int)Math.Pow(10.0, (double)(15 - endTU + 2)));
                FractionalSeconds = CalculateFractionalSeconds(Fraction, endTU);
                if ((short)endTU == 15 && Fraction > 99999)
                {
                    throw new OverflowException();
                }
                if ((short)endTU == 14 && Fraction > 9999)
                {
                    throw new OverflowException();
                }
                if ((short)endTU == 13 && Fraction > 999)
                {
                    throw new OverflowException();
                }
                if ((short)endTU == 12 && Fraction > 99)
                {
                    throw new OverflowException();
                }
                if ((short)endTU == 11 && Fraction > 9)
                {
                    throw new OverflowException();
                }
            }
            num3++;
        }
    }

    internal IntervalDateTime(string _szTime, InformixTimeUnit leading, InformixTimeUnit trailing)
    {
        Year = 0;
        Month = 0;
        Day = 0;
        Hour = 0;
        Minute = 0;
        Second = 0;
        Fraction = 0;
        isNull = false;
        FractionalSeconds = 0.0;
        startTU = leading;
        endTU = trailing;
        string[] array = null;
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        int num6 = 0;
        string text = _szTime.Trim();
        array = text.Split('-');
        bool flag;
        if (array.Length > 1 && array[0].CompareTo("") == 0)
        {
            flag = true;
            text = text.TrimStart('-');
        }
        else
        {
            flag = false;
        }
        short num7 = Qualifier.TimeUnitToOffset[(short)leading];
        short num8 = Qualifier.TimeUnitToOffset[(short)trailing];
        if (num7 != num8)
        {
            while (num7 <= num8)
            {
                switch (num7)
                {
                    case 0:
                        num5 = text.IndexOf('-');
                        if (num5 < 0)
                        {
                            throw new FormatException();
                        }
                        Year = int.Parse(text.Substring(0, num5));
                        if (Year < 0)
                        {
                            throw new FormatException();
                        }
                        break;
                    case 1:
                        if (num7 != Qualifier.TimeUnitToOffset[(short)trailing])
                        {
                            num6 = text.IndexOf('-', num5 + 1);
                            if (num6 < 0)
                            {
                                throw new FormatException();
                            }
                            Month = int.Parse(text.Substring(num5 != 0 ? num5 + 1 : 0, num6 - (num5 != 0 ? num5 + 1 : 0)));
                        }
                        else
                        {
                            Month = int.Parse(text.Substring(num5 != 0 ? num5 + 1 : 0, text.Length - (num5 != 0 ? num5 + 1 : 0)));
                        }
                        if (Month < 0)
                        {
                            throw new FormatException();
                        }
                        break;
                    case 2:
                        if (num7 != Qualifier.TimeUnitToOffset[(short)trailing])
                        {
                            num3 = text.IndexOf(' ');
                            if (num3 < 0)
                            {
                                throw new FormatException();
                            }
                            Day = int.Parse(text.Substring(num6 != 0 ? num6 + 1 : 0, num3 - (num6 != 0 ? num6 + 1 : 0)));
                        }
                        else
                        {
                            Day = int.Parse(text.Substring(num6 != 0 ? num6 + 1 : 0, text.Length - (num6 != 0 ? num6 + 1 : 0)));
                        }
                        if (Day < 0)
                        {
                            throw new FormatException();
                        }
                        break;
                    case 3:
                        if (num7 != Qualifier.TimeUnitToOffset[(short)trailing])
                        {
                            num = text.IndexOf(':');
                            if (num < 0)
                            {
                                throw new FormatException();
                            }
                            Hour = int.Parse(text.Substring(num3 != 0 ? num3 + 1 : 0, num - (num3 != 0 ? num3 + 1 : 0)));
                        }
                        else
                        {
                            Hour = int.Parse(text.Substring(num3 != 0 ? num3 + 1 : 0, text.Length - (num3 != 0 ? num3 + 1 : 0)));
                        }
                        if (Hour < 0)
                        {
                            throw new FormatException();
                        }
                        break;
                    case 4:
                        if (num7 != Qualifier.TimeUnitToOffset[(short)trailing])
                        {
                            num2 = text.IndexOf(':', num + 1);
                            if (num2 < 0)
                            {
                                throw new FormatException();
                            }
                            Minute = int.Parse(text.Substring(num != 0 ? num + 1 : 0, num2 - (num != 0 ? num + 1 : 0)));
                        }
                        else
                        {
                            Minute = int.Parse(text.Substring(num != 0 ? num + 1 : 0, text.Length - (num != 0 ? num + 1 : 0)));
                        }
                        if (Minute < 0)
                        {
                            throw new FormatException();
                        }
                        break;
                    case 5:
                        if (num7 != Qualifier.TimeUnitToOffset[(short)trailing])
                        {
                            num4 = text.IndexOf('.', num2 + 1);
                            if (num4 < 0)
                            {
                                throw new FormatException();
                            }
                            Second = int.Parse(text.Substring(num2 != 0 ? num2 + 1 : 0, num4 - (num2 != 0 ? num2 + 1 : 0)));
                        }
                        else
                        {
                            Second = int.Parse(text.Substring(num2 != 0 ? num2 + 1 : 0, text.Length - (num2 != 0 ? num2 + 1 : 0)));
                        }
                        if (Second < 0)
                        {
                            throw new FormatException();
                        }
                        break;
                    case 6:
                        {
                            int num9 = num4 != 0 ? num4 + 1 : 0;
                            string s = text.Substring(num9, text.Length - num9);
                            Fraction = int.Parse(s);
                            if (Fraction < 0)
                            {
                                throw new FormatException();
                            }
                            Fraction = AdjustFractionVal(Fraction, text.Length - num9, endTU);
                            FractionalSeconds = CalculateFractionalSeconds(Fraction, endTU);
                            break;
                        }
                }
                num7++;
            }
        }
        else
        {
            switch (num7)
            {
                case 0:
                    Year = int.Parse(text);
                    if (Year < 0)
                    {
                        throw new FormatException();
                    }
                    break;
                case 1:
                    Month = int.Parse(text);
                    if (Month < 0)
                    {
                        throw new FormatException();
                    }
                    break;
                case 2:
                    Day = int.Parse(text);
                    if (Day < 0)
                    {
                        throw new FormatException();
                    }
                    break;
                case 3:
                    Hour = int.Parse(text);
                    if (Hour < 0)
                    {
                        throw new FormatException();
                    }
                    break;
                case 4:
                    Minute = int.Parse(text);
                    if (Minute < 0)
                    {
                        throw new FormatException();
                    }
                    break;
                case 5:
                    Second = int.Parse(text);
                    if (Second < 0)
                    {
                        throw new FormatException();
                    }
                    break;
                case 6:
                    {
                        string text2 = text;
                        string[] array2 = text.Split('.');
                        text2 = array2[1];
                        Fraction = int.Parse(text2);
                        if (Fraction < 0)
                        {
                            throw new FormatException();
                        }
                        Fraction = AdjustFractionVal(Fraction, text2.Length, endTU);
                        FractionalSeconds = CalculateFractionalSeconds(Fraction, endTU);
                        break;
                    }
            }
        }
        if (flag)
        {
            Year = -1 * Year;
            Month = -1 * Month;
            Day = -1 * Day;
            Hour = -1 * Hour;
            Minute = -1 * Minute;
            Second = -1 * Second;
            Fraction = -1 * Fraction;
            FractionalSeconds *= -1.0;
        }
    }

    private static int AdjustFractionVal(int _fraction, int precision, InformixTimeUnit end)
    {
        int num = Qualifier.FractionPrecision(end) - precision;
        _fraction *= (int)Math.Pow(10.0, num);
        return _fraction;
    }

    internal decimal ToTicks()
    {
        if (Qualifier.TimeUnitToOffset[(short)startTU] < Qualifier.TimeUnitToOffset[4])
        {
            throw new InvalidOperationException();
        }
        try
        {
            decimal d = decimal.Multiply(Fraction, (decimal)Math.Pow(10.0, ((short)endTU - 10) * -1));
            return decimal.Multiply(Day, 864000000000m) + decimal.Multiply(Hour, 36000000000m) + decimal.Multiply(Minute, 600000000m) + decimal.Multiply(Second, 10000000m) + decimal.Multiply(d, 10000000m);
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder(25);
        short num = Qualifier.TimeUnitToOffset[(short)startTU];
        short num2 = Qualifier.TimeUnitToOffset[(short)endTU];
        short num3 = Qualifier.TimeUnitToOffset[16];
        if (Year < 0 || Month < 0 || Day < 0 || Hour < 0 || Minute < 0 || Second < 0 || Fraction < 0)
        {
            stringBuilder.Append("-");
        }
        int num4 = 10;
        for (short num5 = num; num5 <= num2; num5++)
        {
            switch (num5)
            {
                case 0:
                    num4 = Math.Abs(Year);
                    break;
                case 1:
                    num4 = Math.Abs(Month);
                    break;
                case 2:
                    num4 = Math.Abs(Day);
                    break;
                case 3:
                    num4 = Math.Abs(Hour);
                    break;
                case 4:
                    num4 = Math.Abs(Minute);
                    break;
                case 5:
                    num4 = Math.Abs(Second);
                    break;
                case 6:
                    {
                        int num6 = Math.Abs(Fraction);
                        string value = (short)endTU == 15 ? num6.ToString("00000") : (short)endTU == 14 ? num6.ToString("0000") : (short)endTU == 13 || (short)endTU == 16 ? num6.ToString("000") : (short)endTU != 12 ? num6.ToString("0") : num6.ToString("00");
                        if ((short)startTU >= 11)
                        {
                            stringBuilder.Append(".");
                        }
                        stringBuilder.Append(value);
                        break;
                    }
            }
            if (num4 < 10 && num5 != num && num5 != num3)
            {
                stringBuilder.Append('0');
            }
            if (num5 != num3)
            {
                stringBuilder.Append(num4);
            }
            if (num5 != num2)
            {
                stringBuilder.Append(TimeUnitSeparators[num5]);
            }
        }
        return stringBuilder.ToString();
    }

    internal string DateTimeToString()
    {
        StringBuilder stringBuilder = new StringBuilder(25);
        short num = Qualifier.TimeUnitToOffset[(short)startTU];
        short num2 = Qualifier.TimeUnitToOffset[(short)endTU];
        if (Year < 0 || Month < 0 || Day < 0 || Hour < 0 || Minute < 0 || Second < 0 || Fraction < 0)
        {
            stringBuilder.Append("-");
        }
        while (num <= num2)
        {
            string text = FieldFormats[num];
            switch (num)
            {
                case 0:
                    stringBuilder.Append(Math.Abs(Year).ToString(text));
                    break;
                case 1:
                    stringBuilder.Append(Math.Abs(Month).ToString(text));
                    break;
                case 2:
                    stringBuilder.Append(Math.Abs(Day).ToString(text));
                    break;
                case 3:
                    stringBuilder.Append(Math.Abs(Hour).ToString(text));
                    break;
                case 4:
                    stringBuilder.Append(Math.Abs(Minute).ToString(text));
                    break;
                case 5:
                    stringBuilder.Append(Math.Abs(Second).ToString(text));
                    break;
                case 6:
                    {
                        int num3 = Math.Abs(Fraction);
                        text = FieldFormats[num + (short)endTU - 11];
                        string value = num3.ToString(text);
                        if ((short)startTU >= 11)
                        {
                            stringBuilder.Append(".");
                        }
                        stringBuilder.Append(value);
                        break;
                    }
            }
            if (num != num2)
            {
                stringBuilder.Append(TimeUnitSeparators[num]);
            }
            num++;
        }
        return stringBuilder.ToString();
    }

    private static double CalculateFractionalSeconds(int Fraction_, InformixTimeUnit endTU_)
    {
        double num = Fraction_;
        switch ((short)endTU_)
        {
            case 11:
                num /= 10.0;
                break;
            case 12:
                num /= 100.0;
                break;
            case 13:
            case 16:
                num /= 1000.0;
                break;
            case 14:
                num /= 10000.0;
                break;
            case 15:
                num /= 100000.0;
                break;
        }
        return num;
    }
}
