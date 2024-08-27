using System;
using System.Runtime.InteropServices;


namespace Arad.Net.Core.Informix;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct Qualifier
{
    internal static short[] TimeUnitToOffset = new short[17]
    {
        0, -1, 1, -1, 2, -1, 3, -1, 4, -1,
        5, 6, 6, 6, 6, 6, 6
    };

    internal static InformixTimeUnit[] OffsetToTimeUnit = new InformixTimeUnit[7]
    {
        InformixTimeUnit.Year,
        InformixTimeUnit.Month,
        InformixTimeUnit.Day,
        InformixTimeUnit.Hour,
        InformixTimeUnit.Minute,
        InformixTimeUnit.Second,
        InformixTimeUnit.Fraction
    };

    internal static short FractionPrecision(InformixTimeUnit end)
    {
        if (end == InformixTimeUnit.Fraction)
        {
            end = InformixTimeUnit.Fraction3;
        }
        return (short)(end - 10);
    }

    internal static void Decode(int qualifier, out InformixTimeUnit start, out InformixTimeUnit end)
    {
        start = (InformixTimeUnit)(qualifier >> 4 & 0xF);
        end = (InformixTimeUnit)(qualifier & 0xF);
        if (start == InformixTimeUnit.Fraction2)
        {
            start = InformixTimeUnit.Fraction;
        }
    }

    internal static short IntervalEncode(InformixTimeUnit start, InformixTimeUnit end)
    {
        if (start == InformixTimeUnit.Fraction)
        {
            start = InformixTimeUnit.Fraction2;
        }
        ushort num = (ushort)(start == InformixTimeUnit.Fraction2 ? 2u : 9u);
        num = (ushort)(end - start + num);
        return (short)((ushort)(num << 8) | (ushort)start << 4 | (ushort)end);
    }

    internal static short DateTimeEncode(InformixTimeUnit start, InformixTimeUnit end)
    {
        if (start == InformixTimeUnit.Fraction)
        {
            start = InformixTimeUnit.Fraction2;
        }
        ushort num = (ushort)((ushort)end - (ushort)start + (start == InformixTimeUnit.Year ? 4 : 2));
        return (short)((ushort)(num << 8) | (ushort)start << 4 | (ushort)end);
    }

    internal static void Validate(InformixTimeUnit MaxQual, InformixTimeUnit MinQual, InformixTimeUnit start, InformixTimeUnit end)
    {
        short num = TimeUnitToOffset[(short)start];
        short num2 = TimeUnitToOffset[(short)end];
        if (!Enum.IsDefined(start.GetType(), start) || !Enum.IsDefined(end.GetType(), end))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (num > num2)
        {
            throw new ArgumentException();
        }
        if (num < TimeUnitToOffset[(short)MaxQual] || num2 > TimeUnitToOffset[(short)MinQual])
        {
            throw new ArgumentException();
        }
        if (num == 6 && start != InformixTimeUnit.Fraction)
        {
            throw new ArgumentException();
        }
    }

    internal static bool GreaterThan(InformixTimeUnit unit1, InformixTimeUnit unit2)
    {
        return (short)unit1 < (short)unit2;
    }

    internal static InformixTimeUnit ValidateEndTUDetermineStartTU(InformixTimeUnit MaxQual, InformixTimeUnit MinQual, int numUnits, InformixTimeUnit end)
    {
        int num = 100;
        if (!Enum.IsDefined(end.GetType(), end))
        {
            throw new ArgumentOutOfRangeException();
        }
        short num2 = TimeUnitToOffset[(short)end];
        if (num2 > TimeUnitToOffset[(short)MinQual])
        {
            throw new ArgumentException();
        }
        num = num2 - numUnits + 1;
        if (num < 0)
        {
            throw new ArgumentException();
        }
        if (num < TimeUnitToOffset[(short)MaxQual])
        {
            throw new ArgumentException();
        }
        return OffsetToTimeUnit[num];
    }
}
