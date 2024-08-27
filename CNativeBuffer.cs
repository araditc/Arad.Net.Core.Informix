using System;
using Arad.Net.Core.Informix.System.Data.Common;
using Arad.Net.Core.Informix.System.Data.ProviderBase;
using System.Runtime.InteropServices;



namespace Arad.Net.Core.Informix;
internal sealed class CNativeBuffer : DbBuffer
{
    internal nint _buffer;

    internal int _bufferlen;

    internal bool _memowner;

    internal short ShortLength => checked((short)Length);

    internal nint Address
    {
        get
        {
            if (_buffer == nint.Zero)
            {
                _buffer = Marshal.AllocCoTaskMem(_bufferlen);
                _memowner = true;
            }
            return _buffer;
        }
    }

    internal CNativeBuffer(int initialSize)
        : base(initialSize)
    {
        _buffer = nint.Zero;
        _bufferlen = initialSize;
        _memowner = false;
    }

    internal object MarshalToManaged(Informix32.GENLIB_TYPE genlibType)
    {
        object result = null;
        nint address = Address;
        switch (genlibType)
        {
            case Informix32.GENLIB_TYPE.DTTIME_T:
                {
                    Informix32.RetCode retCode2 = Informix32.RetCode.SUCCESS;
                    byte[] array2 = new byte[26];
                    retCode2 = Interop.Odbc.dttoasc(address, array2);
                    if (retCode2 != 0)
                    {
                        throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode2));
                    }
                    string string2 = Informix32.encoder.GetString(array2, 0, 26);
                    string2 = string2.Trim('\0');
                    short qualifier2 = Marshal.ReadInt16(address, 0);
                    Qualifier.Decode(qualifier2, out var start2, out var end2);
                    IntervalDateTime intervalDateTime2 = new IntervalDateTime(string2, start2, end2);
                    result = intervalDateTime2;
                    break;
                }
            case Informix32.GENLIB_TYPE.INTRVL_T:
                {
                    Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
                    byte[] array = new byte[26];
                    retCode = Interop.Odbc.intoasc(address, array);
                    if (retCode != 0)
                    {
                        throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
                    }
                    string @string = Informix32.encoder.GetString(array, 0, 26);
                    @string = @string.Trim('\0');
                    short qualifier = Marshal.ReadInt16(address, 0);
                    Qualifier.Decode(qualifier, out var start, out var end);
                    IntervalDateTime intervalDateTime = new IntervalDateTime(@string, start, end);
                    result = intervalDateTime;
                    break;
                }
        }
        return result;
    }

    internal object MarshalToManaged(Informix32.SQL_C sqlctype, int cb)
    {
        nint address = Address;
        switch (sqlctype)
        {
            case Informix32.SQL_C.WCHAR:
                if (cb < 0)
                {
                    return Marshal.PtrToStringUni(address);
                }
                cb = Math.Min(cb / 2, (Length - 2) / 2);
                return Marshal.PtrToStringUni(address, cb);
            case Informix32.SQL_C.BINARY:
            case Informix32.SQL_C.CHAR:
                {
                    cb = Math.Min(cb, Length);
                    byte[] array2 = new byte[cb];
                    Marshal.Copy(address, array2, 0, cb);
                    return array2;
                }
            case Informix32.SQL_C.SSHORT:
                return Marshal.PtrToStructure(address, typeof(short));
            case Informix32.SQL_C.SLONG:
                return Marshal.PtrToStructure(address, typeof(int));
            case Informix32.SQL_C.SBIGINT:
                return Marshal.PtrToStructure(address, typeof(long));
            case Informix32.SQL_C.BIT:
                {
                    byte b = Marshal.ReadByte(address);
                    return b != 0;
                }
            case Informix32.SQL_C.REAL:
                return Marshal.PtrToStructure(address, typeof(float));
            case Informix32.SQL_C.DOUBLE:
                return Marshal.PtrToStructure(address, typeof(double));
            case Informix32.SQL_C.UTINYINT:
                return Marshal.ReadByte(address);
            case Informix32.SQL_C.GUID:
                return Marshal.PtrToStructure(address, typeof(Guid));
            case Informix32.SQL_C.INTERVAL_DAY_TO_SECOND:
                {
                    Informix32.SQL_TYPE sQL_TYPE2 = (Informix32.SQL_TYPE)(Marshal.ReadInt32(address, 0) + 100);
                    int num3 = Marshal.ReadInt32(address, 4);
                    decimal num4 = Marshal.ReadInt32(address, 8) * 864000000000m;
                    num4 += Marshal.ReadInt32(address, 12) * 36000000000m;
                    num4 += Marshal.ReadInt32(address, 16) * 600000000m;
                    num4 += Marshal.ReadInt32(address, 20) * 10000000m;
                    num4 += Marshal.ReadInt32(address, 24);
                    if (num3 != 0)
                    {
                        num4 *= -1m;
                    }
                    return num4;
                }
            case Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH:
                {
                    Informix32.SQL_TYPE sQL_TYPE = (Informix32.SQL_TYPE)(Marshal.ReadInt32(address, 0) + 100);
                    int num = Marshal.ReadInt32(address, 4);
                    long num2 = Marshal.ReadInt32(address, 8);
                    num2 *= 12;
                    num2 += Marshal.ReadInt32(address, 12);
                    if (num != 0)
                    {
                        num2 *= -1;
                    }
                    return num2;
                }
            case Informix32.SQL_C.TYPE_TIMESTAMP:
                {
                    IntervalDateTime intervalDateTime = new IntervalDateTime(t: false);
                    intervalDateTime.Year = Marshal.ReadInt16(address, 0);
                    intervalDateTime.Month = Marshal.ReadInt16(address, 2);
                    intervalDateTime.Day = Marshal.ReadInt16(address, 4);
                    intervalDateTime.Hour = Marshal.ReadInt16(address, 6);
                    intervalDateTime.Minute = Marshal.ReadInt16(address, 8);
                    intervalDateTime.Second = Marshal.ReadInt16(address, 10);
                    intervalDateTime.Fraction = Marshal.ReadInt32(address, 12);
                    return intervalDateTime;
                }
            case Informix32.SQL_C.TYPE_DATE:
                return new DateTime(Marshal.ReadInt16(address, 0), Marshal.ReadInt16(address, 2), Marshal.ReadInt16(address, 4));
            case Informix32.SQL_C.TYPE_TIME:
                return new TimeSpan(Marshal.ReadInt16(address, 0), Marshal.ReadInt16(address, 2), Marshal.ReadInt16(address, 4));
            case Informix32.SQL_C.INFX_DECIMAL:
                {
                    byte[] array = new byte[cb];
                    Marshal.Copy(address, array, 0, array.Length);
                    return new InformixDecimal(array);
                }
            case Informix32.SQL_C.NUMERIC:
                return new decimal(Marshal.ReadInt32(address, 3), Marshal.ReadInt32(address, 7), Marshal.ReadInt32(address, 11), Marshal.ReadByte(address, 2) == 0, Marshal.ReadByte(address, 1));
            default:
                throw new ArgumentException();
        }
    }

    internal object MarshalToManaged(int offset, Informix32.SQL_C sqlctype, int cb)
    {
        switch (sqlctype)
        {
            case Informix32.SQL_C.WCHAR:
                if (cb == -3)
                {
                    return PtrToStringUni(offset);
                }
                cb = Math.Min(cb / 2, (Length - 2) / 2);
                return PtrToStringUni(offset, cb);
            case Informix32.SQL_C.BINARY:
            case Informix32.SQL_C.CHAR:
                cb = Math.Min(cb, Length);
                return ReadBytes(offset, cb);
            case Informix32.SQL_C.SSHORT:
                return ReadInt16(offset);
            case Informix32.SQL_C.SLONG:
                return ReadInt32(offset);
            case Informix32.SQL_C.SBIGINT:
                return ReadInt64(offset);
            case Informix32.SQL_C.BIT:
                {
                    byte b = ReadByte(offset);
                    return b != 0;
                }
            case Informix32.SQL_C.REAL:
                return ReadSingle(offset);
            case Informix32.SQL_C.DOUBLE:
                return ReadDouble(offset);
            case Informix32.SQL_C.UTINYINT:
                return ReadByte(offset);
            case Informix32.SQL_C.GUID:
                return ReadGuid(offset);
            case Informix32.SQL_C.TYPE_TIMESTAMP:
                return ReadDateTime(offset);
            case Informix32.SQL_C.TYPE_DATE:
                return ReadDate(offset);
            case Informix32.SQL_C.TYPE_TIME:
                return ReadTime(offset);
            case Informix32.SQL_C.NUMERIC:
                return ReadNumeric(offset);
            default:
                return null;
        }
    }

    internal void MarshalToNative(int offset, object value, Informix32.SQL_C sqlctype, int sizeorprecision, int valueOffset)
    {
        nint address = Address;
        switch (sqlctype)
        {
            case Informix32.SQL_C.WCHAR:
                if (value is string)
                {
                    int num4 = Math.Max(0, ((string)value).Length - valueOffset);
                    if (sizeorprecision > 0 && sizeorprecision < num4)
                    {
                        num4 = sizeorprecision;
                    }
                    char[] array = ((string)value).ToCharArray(valueOffset, num4);
                    WriteCharArray(offset, array, 0, array.Length);
                    WriteInt16(offset + array.Length * 2, 0);
                }
                else
                {
                    int num4 = Math.Max(0, ((char[])value).Length - valueOffset);
                    if (sizeorprecision > 0 && sizeorprecision < num4)
                    {
                        num4 = sizeorprecision;
                    }
                    char[] array = (char[])value;
                    WriteCharArray(offset, array, valueOffset, num4);
                    WriteInt16(offset + array.Length * 2, 0);
                }
                break;
            case Informix32.SQL_C.BINARY:
            case Informix32.SQL_C.CHAR:
                {
                    byte[] array2 = (byte[])value;
                    int num5 = array2.Length;
                    num5 -= valueOffset;
                    if (sizeorprecision > 0 && sizeorprecision < num5)
                    {
                        num5 = sizeorprecision;
                    }
                    WriteBytes(offset, array2, valueOffset, num5);
                    break;
                }
            case Informix32.SQL_C.SB_LOCATOR:
                WriteBytes(offset, ((InformixSmartLOBLocator)value).ToBytes(), valueOffset, 72);
                break;
            case Informix32.SQL_C.UTINYINT:
                WriteByte(offset, (byte)value);
                break;
            case Informix32.SQL_C.SSHORT:
                WriteInt16(offset, (short)value);
                break;
            case Informix32.SQL_C.SLONG:
                WriteInt32(offset, (int)value);
                break;
            case Informix32.SQL_C.REAL:
                WriteSingle(offset, (float)value);
                break;
            case Informix32.SQL_C.SINFXBIGINT:
            case Informix32.SQL_C.SBIGINT:
                WriteInt64(offset, (long)value);
                break;
            case Informix32.SQL_C.DOUBLE:
                WriteDouble(offset, (double)value);
                break;
            case Informix32.SQL_C.GUID:
                WriteGuid(offset, (Guid)value);
                break;
            case Informix32.SQL_C.BIT:
                WriteByte(offset, (byte)((bool)value ? 1u : 0u));
                break;
            case Informix32.SQL_C.TYPE_TIMESTAMP:
                {
                    int value2 = 0;
                    IntervalDateTime intervalDateTime = !(typeof(InformixDateTime) == value.GetType()) ? new IntervalDateTime((DateTime)value) : ((InformixDateTime)value).intervalDateTime;
                    WriteInt16(offset, (short)intervalDateTime.Year);
                    WriteInt16(offset + 2, (short)intervalDateTime.Month);
                    WriteInt16(offset + 4, (short)intervalDateTime.Day);
                    WriteInt16(offset + 6, (short)intervalDateTime.Hour);
                    WriteInt16(offset + 8, (short)intervalDateTime.Minute);
                    WriteInt16(offset + 10, (short)intervalDateTime.Second);
                    if (intervalDateTime.endTU >= InformixTimeUnit.Fraction1)
                    {
                        value2 = intervalDateTime.Fraction * (int)Math.Pow(10.0, (double)(9 - (intervalDateTime.endTU - 11 + 1)));
                    }
                    WriteInt32(offset + 12, value2);
                    break;
                }
            case Informix32.SQL_C.TYPE_DATE:
                {
                    DateTime dateTime = (DateTime)value;
                    WriteInt16(offset, (short)dateTime.Year);
                    WriteInt16(offset + 2, (short)dateTime.Month);
                    WriteInt16(offset + 4, (short)dateTime.Day);
                    break;
                }
            case Informix32.SQL_C.TYPE_TIME:
                {
                    TimeSpan timeSpan2 = (TimeSpan)value;
                    WriteInt16(offset, (short)timeSpan2.Hours);
                    WriteInt16(offset + 2, (short)timeSpan2.Minutes);
                    WriteInt16(offset + 4, (short)timeSpan2.Seconds);
                    break;
                }
            case Informix32.SQL_C.NUMERIC:
                WriteNumeric(offset, Convert.ToDecimal(value), checked((byte)sizeorprecision));
                break;
            case Informix32.SQL_C.INFX_DECIMAL:
                {
                    byte[] array3 = ((InformixDecimal)value).DectVal != null ? ((InformixDecimal)value).DectVal : InformixDecimal.Zero.DectVal;
                    WriteBytes(offset, array3, valueOffset, array3.Length);
                    break;
                }
            case Informix32.SQL_C.INTERVAL_DAY_TO_SECOND:
                if (typeof(InformixTimeSpan) == value.GetType())
                {
                    InformixTimeSpan ifxTimeSpan = (InformixTimeSpan)value;
                    if (ifxTimeSpan.IsNull)
                    {
                        Marshal.WriteInt32(address, 0, -1);
                        break;
                    }
                    WriteInt32(offset, 10);
                    int num6 = ifxTimeSpan.Ticks < 0m ? 1 : 0;
                    WriteInt32(offset + 4, num6);
                    if (1 == num6)
                    {
                        ifxTimeSpan = new InformixTimeSpan(-ifxTimeSpan.Ticks);
                    }
                    WriteInt32(offset + 8, ifxTimeSpan.Days);
                    WriteInt32(offset + 12, ifxTimeSpan.Hours);
                    WriteInt32(offset + 16, ifxTimeSpan.Minutes);
                    WriteInt32(offset + 20, ifxTimeSpan.Seconds);
                    WriteInt32(offset + 24, (int)(ifxTimeSpan.Ticks % 10000000m));
                }
                else
                {
                    TimeSpan timeSpan = (TimeSpan)value;
                    WriteInt32(offset, 10);
                    int num7 = timeSpan.Ticks < 0 ? 1 : 0;
                    WriteInt32(offset + 4, num7);
                    if (1 == num7)
                    {
                        timeSpan = timeSpan.Negate();
                    }
                    WriteInt32(offset + 8, timeSpan.Days);
                    WriteInt32(offset + 12, timeSpan.Hours);
                    WriteInt32(offset + 16, timeSpan.Minutes);
                    WriteInt32(offset + 20, timeSpan.Seconds);
                    WriteInt32(offset + 24, (int)(timeSpan.Ticks % 10000000));
                }
                break;
            case Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH:
                {
                    int num = 0;
                    int num2 = 0;
                    InformixMonthSpan ifxMonthSpan = (InformixMonthSpan)value;
                    WriteInt32(offset, 7);
                    int num3 = ifxMonthSpan.TotalMonths < 0 ? 1 : 0;
                    WriteInt32(offset + 4, num3);
                    if (1 == num3)
                    {
                        num = ifxMonthSpan.Years >= 0 ? ifxMonthSpan.Years : -1 * ifxMonthSpan.Years;
                        num2 = ifxMonthSpan.Months >= 0 ? ifxMonthSpan.Months : -1 * ifxMonthSpan.Months;
                    }
                    else
                    {
                        num = ifxMonthSpan.Years;
                        num2 = ifxMonthSpan.Months;
                    }
                    WriteInt32(offset + 8, num);
                    WriteInt32(offset + 12, num2);
                    break;
                }
        }
    }

    internal void MarshalToNative(object value, Informix32.GENLIB_TYPE genlibType)
    {
        nint address = Address;
        switch (genlibType)
        {
            case Informix32.GENLIB_TYPE.DTTIME_T:
                {
                    IntervalDateTime intervalDateTime2 = (IntervalDateTime)value;
                    short val2 = Qualifier.DateTimeEncode(intervalDateTime2.startTU, intervalDateTime2.endTU);
                    Marshal.WriteInt16(address, 0, val2);
                    string inbuf2 = intervalDateTime2.DateTimeToString();
                    Informix32.RetCode retCode2 = Interop.Odbc.dtcvasc(inbuf2, address);
                    if (retCode2 != 0)
                    {
                        throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode2));
                    }
                    break;
                }
            case Informix32.GENLIB_TYPE.INTRVL_T:
                {
                    IntervalDateTime intervalDateTime = (IntervalDateTime)value;
                    short val = Qualifier.IntervalEncode(intervalDateTime.startTU, intervalDateTime.endTU);
                    Marshal.WriteInt16(address, 0, val);
                    string inbuf = intervalDateTime.ToString();
                    Informix32.RetCode retCode = Interop.Odbc.incvasc(inbuf, address);
                    if (retCode != 0)
                    {
                        throw new InformixException("Error", Informix32.GetDiagErrors("Error", null, retCode));
                    }
                    break;
                }
        }
    }

    internal void MarshalToNative(object value, Informix32.SQL_C sqlctype, int precision)
    {
        nint address = Address;
        switch (sqlctype)
        {
            case Informix32.SQL_C.WCHAR:
                {
                    char[] array4 = !(value is char[]) ? value.ToString().ToCharArray() : (char[])value;
                    EnsureAlloc((array4.Length + 1) * 2);
                    address = Address;
                    Marshal.Copy(array4, 0, address, array4.Length);
                    Marshal.WriteInt16(address, array4.Length * 2, 0);
                    break;
                }
            case Informix32.SQL_C.BINARY:
                {
                    byte[] array = (byte[])value;
                    EnsureAlloc(array.Length);
                    address = Address;
                    Marshal.Copy(array, 0, address, array.Length);
                    break;
                }
            case Informix32.SQL_C.CHAR:
                {
                    char[] array3 = (char[])value;
                    EnsureAlloc(array3.Length + 1);
                    address = Address;
                    Marshal.Copy(array3, 0, address, array3.Length);
                    break;
                }
            case Informix32.SQL_C.SB_LOCATOR:
                Marshal.Copy(((InformixSmartLOBLocator)value).ToBytes(), 0, Address, 72);
                break;
            case Informix32.SQL_C.UTINYINT:
                Marshal.WriteByte(address, (byte)value);
                break;
            case Informix32.SQL_C.SSHORT:
                Marshal.WriteInt16(address, (short)value);
                break;
            case Informix32.SQL_C.SLONG:
            case Informix32.SQL_C.REAL:
                Marshal.StructureToPtr(value, address, fDeleteOld: false);
                break;
            case Informix32.SQL_C.SINFXBIGINT:
            case Informix32.SQL_C.SBIGINT:
            case Informix32.SQL_C.DOUBLE:
                Marshal.StructureToPtr(value, address, fDeleteOld: false);
                break;
            case Informix32.SQL_C.GUID:
                Marshal.StructureToPtr(value, address, fDeleteOld: false);
                break;
            case Informix32.SQL_C.BIT:
                Marshal.WriteByte(address, (byte)((bool)value ? 1u : 0u));
                break;
            case Informix32.SQL_C.INTERVAL_DAY_TO_SECOND:
                if (typeof(InformixTimeSpan) == value.GetType())
                {
                    InformixTimeSpan ifxTimeSpan = (InformixTimeSpan)value;
                    Marshal.WriteInt32(address, 0, 10);
                    int num = ifxTimeSpan.Ticks < 0m ? 1 : 0;
                    Marshal.WriteInt32(address, 4, num);
                    if (1 == num)
                    {
                        ifxTimeSpan = new InformixTimeSpan(-ifxTimeSpan.Ticks);
                    }
                    Marshal.WriteInt32(address, 8, ifxTimeSpan.Days);
                    Marshal.WriteInt32(address, 12, ifxTimeSpan.Hours);
                    Marshal.WriteInt32(address, 16, ifxTimeSpan.Minutes);
                    Marshal.WriteInt32(address, 20, ifxTimeSpan.Seconds);
                    Marshal.WriteInt32(address, 24, (int)(ifxTimeSpan.Ticks % 10000000m));
                }
                else
                {
                    TimeSpan timeSpan = (TimeSpan)value;
                    Marshal.WriteInt32(address, 0, 10);
                    int num2 = timeSpan.Ticks < 0 ? 1 : 0;
                    Marshal.WriteInt32(address, 4, num2);
                    if (1 == num2)
                    {
                        timeSpan = timeSpan.Negate();
                    }
                    Marshal.WriteInt32(address, 8, timeSpan.Days);
                    Marshal.WriteInt32(address, 12, timeSpan.Hours);
                    Marshal.WriteInt32(address, 16, timeSpan.Minutes);
                    Marshal.WriteInt32(address, 20, timeSpan.Seconds);
                    Marshal.WriteInt32(address, 24, (int)(timeSpan.Ticks % 10000000));
                }
                break;
            case Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH:
                {
                    int num3 = 0;
                    int num4 = 0;
                    InformixMonthSpan ifxMonthSpan = (InformixMonthSpan)value;
                    Marshal.WriteInt32(address, 0, 7);
                    int num5 = ifxMonthSpan.TotalMonths < 0 ? 1 : 0;
                    Marshal.WriteInt32(address, 4, num5);
                    if (1 == num5)
                    {
                        num3 = ifxMonthSpan.Years >= 0 ? ifxMonthSpan.Years : -1 * ifxMonthSpan.Years;
                        num4 = ifxMonthSpan.Months >= 0 ? ifxMonthSpan.Months : -1 * ifxMonthSpan.Months;
                    }
                    else
                    {
                        num3 = ifxMonthSpan.Years;
                        num4 = ifxMonthSpan.Months;
                    }
                    Marshal.WriteInt32(address, 8, num3);
                    Marshal.WriteInt32(address, 12, num4);
                    break;
                }
            case Informix32.SQL_C.TYPE_TIMESTAMP:
                {
                    int val = 0;
                    IntervalDateTime intervalDateTime = !(typeof(InformixDateTime) == value.GetType()) ? new IntervalDateTime((DateTime)value) : ((InformixDateTime)value).intervalDateTime;
                    Marshal.WriteInt16(address, 0, (short)intervalDateTime.Year);
                    Marshal.WriteInt16(address, 2, (short)intervalDateTime.Month);
                    Marshal.WriteInt16(address, 4, (short)intervalDateTime.Day);
                    Marshal.WriteInt16(address, 6, (short)intervalDateTime.Hour);
                    Marshal.WriteInt16(address, 8, (short)intervalDateTime.Minute);
                    Marshal.WriteInt16(address, 10, (short)intervalDateTime.Second);
                    if (intervalDateTime.endTU >= InformixTimeUnit.Fraction1)
                    {
                        val = intervalDateTime.Fraction * (int)Math.Pow(10.0, (double)(9 - (intervalDateTime.endTU - 11 + 1)));
                    }
                    Marshal.WriteInt32(address, 12, val);
                    break;
                }
            case Informix32.SQL_C.TYPE_DATE:
                {
                    DateTime dateTime = (DateTime)value;
                    Marshal.WriteInt16(address, 0, (short)dateTime.Year);
                    Marshal.WriteInt16(address, 2, (short)dateTime.Month);
                    Marshal.WriteInt16(address, 4, (short)dateTime.Day);
                    break;
                }
            case Informix32.SQL_C.TYPE_TIME:
                {
                    TimeSpan timeSpan2 = (TimeSpan)value;
                    Marshal.WriteInt16(address, 0, (short)timeSpan2.Hours);
                    Marshal.WriteInt16(address, 2, (short)timeSpan2.Minutes);
                    Marshal.WriteInt16(address, 4, (short)timeSpan2.Seconds);
                    break;
                }
            case Informix32.SQL_C.INFX_DECIMAL:
                {
                    byte[] array2 = ((InformixDecimal)value).DectVal != null ? ((InformixDecimal)value).DectVal : InformixDecimal.Zero.DectVal;
                    Marshal.Copy(array2, 0, address, array2.Length);
                    break;
                }
            case Informix32.SQL_C.NUMERIC:
                {
                    int[] bits = decimal.GetBits((decimal)value);
                    byte[] bytes = BitConverter.GetBytes(bits[3]);
                    Marshal.WriteByte(address, 0, (byte)precision);
                    Marshal.WriteByte(address, 1, bytes[2]);
                    Marshal.WriteByte(address, 2, bytes[3] == 0 ? (byte)1 : (byte)0);
                    Marshal.WriteInt32(address, 3, bits[0]);
                    Marshal.WriteInt32(address, 7, bits[1]);
                    Marshal.WriteInt32(address, 11, bits[2]);
                    Marshal.WriteInt32(address, 15, 0);
                    break;
                }
            default:
                throw new ArgumentException();
        }
    }

    internal HandleRef PtrOffset(int offset, int length)
    {
        Validate(offset, length);
        nint intPtr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
        return new HandleRef(this, intPtr);
    }

    internal void EnsureAlloc(int cb)
    {
        if (_bufferlen >= cb)
        {
            return;
        }
        if (nint.Zero != _buffer)
        {
            if (_memowner)
            {
                _buffer = Marshal.ReAllocCoTaskMem(_buffer, cb);
            }
            else
            {
                _buffer = nint.Zero;
            }
        }
        _bufferlen = cb;
        Length = _bufferlen;
    }

    internal void WriteODBCDateTime(int offset, DateTime value)
    {
        short[] source = new short[6]
        {
            (short)value.Year,
            (short)value.Month,
            (short)value.Day,
            (short)value.Hour,
            (short)value.Minute,
            (short)value.Second
        };
        WriteInt16Array(offset, source, 0, 6);
        WriteInt32(offset + 12, value.Millisecond * 1000000);
    }

    internal void FreeBuffer()
    {
        if (_memowner && nint.Zero != _buffer)
        {
            Marshal.FreeCoTaskMem(_buffer);
            _memowner = false;
            _buffer = nint.Zero;
        }
    }
}
