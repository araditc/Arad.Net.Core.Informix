using System;
using System.Data;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Globalization;


namespace Arad.Net.Core.Informix;

internal sealed class TypeMap
{
    internal InformixType _odbcType;

    internal DbType _dbType;

    internal Type _type;

    internal readonly string _ifxTypeName;

    internal readonly Informix32.DATA_TYPE_INDEX _index;

    internal Informix32.SQL_TYPE _sql_type;

    internal Informix32.SQL_C _sql_c;

    internal Informix32.SQL_C _param_sql_c;

    internal int _bufferSize;

    internal int _columnSize;

    internal bool _signType;

    private static readonly TypeMap _SmallInt = new TypeMap("SMALLINT", InformixType.SmallInt, DbType.Int16, typeof(short), Informix32.SQL_TYPE.SMALLINT, Informix32.SQL_C.SSHORT, Informix32.SQL_C.SSHORT, 2, 5, signType: true, Informix32.DATA_TYPE_INDEX.SMALLINT);

    private static readonly TypeMap _Integer = new TypeMap("INT", InformixType.Integer, DbType.Int32, typeof(int), Informix32.SQL_TYPE.INTEGER, Informix32.SQL_C.SLONG, Informix32.SQL_C.SLONG, 4, 10, signType: true, Informix32.DATA_TYPE_INDEX.INTEGER);

    private static readonly TypeMap _Serial = new TypeMap("SERIAL", InformixType.Serial, DbType.Int32, typeof(int), Informix32.SQL_TYPE.INTEGER, Informix32.SQL_C.SLONG, Informix32.SQL_C.SLONG, 4, 10, signType: true, Informix32.DATA_TYPE_INDEX.SERIAL);

    private static readonly TypeMap _Int8 = new TypeMap("INT8", InformixType.Int8, DbType.Int64, typeof(long), Informix32.SQL_TYPE.BIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.INT8);

    private static readonly TypeMap _BigInt = new TypeMap("BIGINT", InformixType.BigInt, DbType.Int64, typeof(long), Informix32.SQL_TYPE.INFXBIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SINFXBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.BIGINT);

    private static readonly TypeMap _Serial8 = new TypeMap("SERIAL8", InformixType.Serial8, DbType.Int64, typeof(long), Informix32.SQL_TYPE.BIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.SERIAL8);

    private static readonly TypeMap _BigSerial = new TypeMap("BIGSERIAL", InformixType.BigSerial, DbType.Int64, typeof(long), Informix32.SQL_TYPE.INFXBIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SINFXBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.BIGSERIAL);

    private static readonly TypeMap _SmallFloat = new TypeMap("SMALLFLOAT", InformixType.SmallFloat, DbType.Single, typeof(float), Informix32.SQL_TYPE.REAL, Informix32.SQL_C.REAL, Informix32.SQL_C.REAL, 4, 7, signType: true, Informix32.DATA_TYPE_INDEX.SMALLFLOAT);

    private static readonly TypeMap _Double = new TypeMap("DOUBLE PRECISION", InformixType.Float, DbType.Double, typeof(double), Informix32.SQL_TYPE.DOUBLE, Informix32.SQL_C.DOUBLE, Informix32.SQL_C.DOUBLE, 8, 15, signType: true, Informix32.DATA_TYPE_INDEX.DOUBLE);

    private static readonly TypeMap _Decimal = new TypeMap("DECIMAL({0},{1})", InformixType.Decimal, DbType.Decimal, typeof(decimal), Informix32.SQL_TYPE.INFX_DECIMAL, Informix32.SQL_C.WCHAR, Informix32.SQL_C.INFX_DECIMAL, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.DECIMAL);

    private static readonly TypeMap _IfxDecimal = new TypeMap("DECIMAL({0},{1})", InformixType.Decimal, DbType.Decimal, typeof(decimal), Informix32.SQL_TYPE.INFX_DECIMAL, Informix32.SQL_C.INFX_DECIMAL, Informix32.SQL_C.INFX_DECIMAL, 22, -1, signType: false, Informix32.DATA_TYPE_INDEX.IFXDECIMAL);

    private static readonly TypeMap _Money = new TypeMap("MONEY({0},{1})", InformixType.Money, DbType.Currency, typeof(decimal), Informix32.SQL_TYPE.INFX_DECIMAL, Informix32.SQL_C.WCHAR, Informix32.SQL_C.INFX_DECIMAL, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.MONEY);

    private static readonly TypeMap _Date = new TypeMap("DATE", InformixType.Date, DbType.Date, typeof(DateTime), Informix32.SQL_TYPE.TYPE_DATE, Informix32.SQL_C.TYPE_DATE, Informix32.SQL_C.TYPE_DATE, 6, 10, signType: false, Informix32.DATA_TYPE_INDEX.DATE);

    private static readonly TypeMap _IfxDateTime = new TypeMap("DATETIME YEAR TO FRACTION(5)", InformixType.DateTime, DbType.DateTime, typeof(DateTime), Informix32.SQL_TYPE.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, 16, -1, signType: false, Informix32.DATA_TYPE_INDEX.IFXDATETIME);

    private static readonly TypeMap _NChar = new TypeMap("NCHAR({0})", InformixType.NChar, DbType.StringFixedLength, typeof(string), Informix32.SQL_TYPE.CHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.NCHAR);

    private static readonly TypeMap _Char1 = new TypeMap("CHAR", InformixType.Char1, DbType.StringFixedLength, typeof(string), Informix32.SQL_TYPE.CHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.CHAR1);

    private static readonly TypeMap _VarChar = new TypeMap("VARCHAR({0})", InformixType.VarChar, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.VARCHAR);

    private static readonly TypeMap _LVarChar = new TypeMap("LVARCHAR({0})", InformixType.LVarChar, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.LVARCHAR);

    private static readonly TypeMap _Row = new TypeMap("ROW", InformixType.Row, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, 65478, 32739, signType: false, Informix32.DATA_TYPE_INDEX.ROW);

    private static readonly TypeMap _Set = new TypeMap("SET", InformixType.Set, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, 65478, 32739, signType: false, Informix32.DATA_TYPE_INDEX.SET);

    private static readonly TypeMap _Multiset = new TypeMap("MULTISET", InformixType.MultiSet, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, 65478, 32739, signType: false, Informix32.DATA_TYPE_INDEX.MULTISET);

    private static readonly TypeMap _List = new TypeMap("LIST", InformixType.List, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, 65478, 32739, signType: false, Informix32.DATA_TYPE_INDEX.LIST);

    private static readonly TypeMap _Byte = new TypeMap("BYTE", InformixType.Byte, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.LONGVARBINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, int.MaxValue, signType: false, Informix32.DATA_TYPE_INDEX.BYTE);

    private static readonly TypeMap _Boolean = new TypeMap("BOOLEAN", InformixType.Boolean, DbType.Boolean, typeof(bool), Informix32.SQL_TYPE.BIT, Informix32.SQL_C.BIT, Informix32.SQL_C.BIT, 2, 1, signType: false, Informix32.DATA_TYPE_INDEX.BOOLEAN);

    private static readonly TypeMap _SQLUDTFixed = new TypeMap("SQLUDTFIXED", InformixType.SQLUDTFixed, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, 65478, 32739, signType: false, Informix32.DATA_TYPE_INDEX.SQLUDTFIXED);

    private static readonly TypeMap _SQLUDTVar = new TypeMap("SQLUDTVAR", InformixType.SQLUDTVar, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, 65478, 32739, signType: false, Informix32.DATA_TYPE_INDEX.SQLUDTVAR);

    private static readonly TypeMap _Unknown = new TypeMap("Other", InformixType.Other, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.BINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.UNKNOWN);

    private static readonly TypeMap _IntervalYearMonth = new TypeMap("INTERVAL {0}({1}) TO {2}", InformixType.IntervalYearMonth, DbType.String, typeof(string), Informix32.SQL_TYPE.INTERVAL_YEAR_TO_MONTH, Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH, Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH, 28, -1, signType: true, Informix32.DATA_TYPE_INDEX.INTERVALYEARMONTH);

    private static readonly TypeMap _IntervalDayFraction = new TypeMap("INTERVAL {0}({1}) TO {2}", InformixType.IntervalDayFraction, DbType.String, typeof(TimeSpan), Informix32.SQL_TYPE.INTERVAL_DAY_TO_SECOND, Informix32.SQL_C.INTERVAL_DAY_TO_SECOND, Informix32.SQL_C.INTERVAL_DAY_TO_SECOND, 28, -1, signType: true, Informix32.DATA_TYPE_INDEX.INTERVALDAYFRACTION);

    private static readonly TypeMap s_bigInt = new TypeMap(InformixType.BigInt, DbType.Int64, typeof(long), Informix32.SQL_TYPE.BIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SBIGINT, 8, 20, signType: true);

    private static readonly TypeMap s_binary = new TypeMap(InformixType.Binary, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.BINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false);

    private static readonly TypeMap s_bit = new TypeMap(InformixType.Bit, DbType.Boolean, typeof(bool), Informix32.SQL_TYPE.BIT, Informix32.SQL_C.BIT, Informix32.SQL_C.BIT, 1, 1, signType: false);

    internal static readonly TypeMap _Char = new TypeMap(InformixType.Char, DbType.AnsiStringFixedLength, typeof(string), Informix32.SQL_TYPE.CHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false);

    private static readonly TypeMap s_dateTime = new TypeMap(InformixType.DateTime, DbType.DateTime, typeof(DateTime), Informix32.SQL_TYPE.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, 16, 23, signType: false);

    private static readonly TypeMap s_date = new TypeMap(InformixType.Date, DbType.Date, typeof(DateTime), Informix32.SQL_TYPE.TYPE_DATE, Informix32.SQL_C.TYPE_DATE, Informix32.SQL_C.TYPE_DATE, 6, 10, signType: false);

    private static readonly TypeMap s_time = new TypeMap(InformixType.Time, DbType.Time, typeof(TimeSpan), Informix32.SQL_TYPE.TYPE_TIME, Informix32.SQL_C.TYPE_TIME, Informix32.SQL_C.TYPE_TIME, 6, 12, signType: false);

    private static readonly TypeMap s_decimal = new TypeMap(InformixType.Decimal, DbType.Decimal, typeof(decimal), Informix32.SQL_TYPE.DECIMAL, Informix32.SQL_C.NUMERIC, Informix32.SQL_C.NUMERIC, 19, 28, signType: false);

    private static readonly TypeMap s_double = new TypeMap(InformixType.Double, DbType.Double, typeof(double), Informix32.SQL_TYPE.DOUBLE, Informix32.SQL_C.DOUBLE, Informix32.SQL_C.DOUBLE, 8, 15, signType: false);

    internal static readonly TypeMap _Image = new TypeMap(InformixType.Image, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.LONGVARBINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false);

    private static readonly TypeMap s_int = new TypeMap(InformixType.Int, DbType.Int32, typeof(int), Informix32.SQL_TYPE.INTEGER, Informix32.SQL_C.SLONG, Informix32.SQL_C.SLONG, 4, 10, signType: true);

    private static readonly TypeMap s_NChar = new TypeMap(InformixType.NChar, DbType.StringFixedLength, typeof(string), Informix32.SQL_TYPE.WCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.WCHAR, -1, -1, signType: false);

    internal static readonly TypeMap _NText = new TypeMap(InformixType.NText, DbType.String, typeof(string), Informix32.SQL_TYPE.WLONGVARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.WCHAR, -1, -1, signType: false);

    private static readonly TypeMap s_numeric = new TypeMap(InformixType.Numeric, DbType.Decimal, typeof(decimal), Informix32.SQL_TYPE.NUMERIC, Informix32.SQL_C.NUMERIC, Informix32.SQL_C.NUMERIC, 19, 28, signType: false);

    internal static readonly TypeMap _NVarChar = new TypeMap(InformixType.NVarChar, DbType.String, typeof(string), Informix32.SQL_TYPE.WVARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.WCHAR, -1, -1, signType: false);

    private static readonly TypeMap s_real = new TypeMap(InformixType.Real, DbType.Single, typeof(float), Informix32.SQL_TYPE.REAL, Informix32.SQL_C.REAL, Informix32.SQL_C.REAL, 4, 7, signType: false);

    private static readonly TypeMap s_uniqueId = new TypeMap(InformixType.UniqueIdentifier, DbType.Guid, typeof(Guid), Informix32.SQL_TYPE.GUID, Informix32.SQL_C.GUID, Informix32.SQL_C.GUID, 16, 36, signType: false);

    private static readonly TypeMap s_smallDT = new TypeMap(InformixType.SmallDateTime, DbType.DateTime, typeof(DateTime), Informix32.SQL_TYPE.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, 16, 23, signType: false);

    private static readonly TypeMap s_smallInt = new TypeMap(InformixType.SmallInt, DbType.Int16, typeof(short), Informix32.SQL_TYPE.SMALLINT, Informix32.SQL_C.SSHORT, Informix32.SQL_C.SSHORT, 2, 5, signType: true);

    internal static readonly TypeMap _Text = new TypeMap(InformixType.Text, DbType.AnsiString, typeof(string), Informix32.SQL_TYPE.LONGVARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false);

    private static readonly TypeMap s_timestamp = new TypeMap(InformixType.Timestamp, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.BINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false);

    private static readonly TypeMap s_tinyInt = new TypeMap(InformixType.TinyInt, DbType.Byte, typeof(byte), Informix32.SQL_TYPE.TINYINT, Informix32.SQL_C.UTINYINT, Informix32.SQL_C.UTINYINT, 1, 3, signType: true);

    private static readonly TypeMap s_varBinary = new TypeMap(InformixType.VarBinary, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.VARBINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false);

    internal static readonly TypeMap s_VarChar = new TypeMap(InformixType.VarChar, DbType.AnsiString, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false);

    private static readonly TypeMap s_LVarChar = new TypeMap("LVARCHAR({0})", InformixType.LVarChar, DbType.String, typeof(string), Informix32.SQL_TYPE.VARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.LVARCHAR);

    private static readonly TypeMap s_variant = new TypeMap(InformixType.Binary, DbType.Binary, typeof(object), Informix32.SQL_TYPE.SS_VARIANT, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false);

    private static readonly TypeMap s_UDT = new TypeMap(InformixType.Binary, DbType.Binary, typeof(object), Informix32.SQL_TYPE.SS_UDT, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false);

    private static readonly TypeMap s_XML = new TypeMap(InformixType.Text, DbType.AnsiString, typeof(string), Informix32.SQL_TYPE.LONGVARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false);

    private static readonly TypeMap _Clob = new TypeMap("CLOB", InformixType.Clob, DbType.String, typeof(string), Informix32.SQL_TYPE.LONGVARCHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.CLOB);

    private static readonly TypeMap _Blob = new TypeMap("BLOB", InformixType.Blob, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.LONGVARBINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.BLOB);

    private static readonly TypeMap _SmartLOBLocator = new TypeMap("", InformixType.SmartLOBLocator, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.UDT_FIXED, Informix32.SQL_C.SB_LOCATOR, Informix32.SQL_C.SB_LOCATOR, InformixSmartLOBLocator.Length, InformixSmartLOBLocator.Length, signType: false, Informix32.DATA_TYPE_INDEX.SMARTLOBLOCATOR);

    private static readonly TypeMap s_IntervalYearMonth = new TypeMap("INTERVAL {0}({1}) TO {2}", InformixType.IntervalYearMonth, DbType.String, typeof(string), Informix32.SQL_TYPE.INTERVAL_YEAR_TO_MONTH, Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH, Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH, 28, -1, signType: true, Informix32.DATA_TYPE_INDEX.INTERVALYEARMONTH);

    private static readonly TypeMap s_IntervalDayFraction = new TypeMap("INTERVAL {0}({1}) TO {2}", InformixType.IntervalDayFraction, DbType.String, typeof(TimeSpan), Informix32.SQL_TYPE.INTERVAL_DAY_TO_SECOND, Informix32.SQL_C.INTERVAL_DAY_TO_SECOND, Informix32.SQL_C.INTERVAL_DAY_TO_SECOND, 28, -1, signType: true, Informix32.DATA_TYPE_INDEX.INTERVALDAYFRACTION);

    private static readonly TypeMap s_Decimal = new TypeMap("DECIMAL({0},{1})", InformixType.Decimal, DbType.Decimal, typeof(decimal), Informix32.SQL_TYPE.INFX_DECIMAL, Informix32.SQL_C.WCHAR, Informix32.SQL_C.INFX_DECIMAL, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.DECIMAL);

    private static readonly TypeMap s_IfxDecimal = new TypeMap("DECIMAL({0},{1})", InformixType.Decimal, DbType.Decimal, typeof(decimal), Informix32.SQL_TYPE.INFX_DECIMAL, Informix32.SQL_C.INFX_DECIMAL, Informix32.SQL_C.INFX_DECIMAL, 22, -1, signType: false, Informix32.DATA_TYPE_INDEX.IFXDECIMAL);

    private static readonly TypeMap s_IfxDateTime = new TypeMap("DATETIME YEAR TO FRACTION(5)", InformixType.DateTime, DbType.DateTime, typeof(DateTime), Informix32.SQL_TYPE.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, Informix32.SQL_C.TYPE_TIMESTAMP, 16, -1, signType: false, Informix32.DATA_TYPE_INDEX.IFXDATETIME);

    private static readonly TypeMap s_Byte = new TypeMap("BYTE", InformixType.Byte, DbType.Binary, typeof(byte[]), Informix32.SQL_TYPE.LONGVARBINARY, Informix32.SQL_C.BINARY, Informix32.SQL_C.BINARY, -1, int.MaxValue, signType: false, Informix32.DATA_TYPE_INDEX.BYTE);

    private static readonly TypeMap s_Money = new TypeMap("MONEY({0},{1})", InformixType.Money, DbType.Currency, typeof(decimal), Informix32.SQL_TYPE.INFX_DECIMAL, Informix32.SQL_C.WCHAR, Informix32.SQL_C.INFX_DECIMAL, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.MONEY);

    private static readonly TypeMap s_SmallFloat = new TypeMap("SMALLFLOAT", InformixType.SmallFloat, DbType.Single, typeof(float), Informix32.SQL_TYPE.REAL, Informix32.SQL_C.REAL, Informix32.SQL_C.REAL, 4, 7, signType: true, Informix32.DATA_TYPE_INDEX.SMALLFLOAT);

    private static readonly TypeMap s_Int8 = new TypeMap("INT8", InformixType.Int8, DbType.Int64, typeof(long), Informix32.SQL_TYPE.BIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.INT8);

    private static readonly TypeMap s_Serial8 = new TypeMap("SERIAL8", InformixType.Serial8, DbType.Int64, typeof(long), Informix32.SQL_TYPE.BIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.SERIAL8);

    private static readonly TypeMap s_BigSerial = new TypeMap("BIGSERIAL", InformixType.BigSerial, DbType.Int64, typeof(long), Informix32.SQL_TYPE.INFXBIGINT, Informix32.SQL_C.SBIGINT, Informix32.SQL_C.SINFXBIGINT, 8, 20, signType: true, Informix32.DATA_TYPE_INDEX.BIGSERIAL);

    private static readonly TypeMap s_Serial = new TypeMap("SERIAL", InformixType.Serial, DbType.Int32, typeof(int), Informix32.SQL_TYPE.INTEGER, Informix32.SQL_C.SLONG, Informix32.SQL_C.SLONG, 4, 10, signType: true, Informix32.DATA_TYPE_INDEX.SERIAL);

    private static readonly TypeMap s_Char1 = new TypeMap("CHAR", InformixType.Char1, DbType.StringFixedLength, typeof(string), Informix32.SQL_TYPE.CHAR, Informix32.SQL_C.WCHAR, Informix32.SQL_C.CHAR, -1, -1, signType: false, Informix32.DATA_TYPE_INDEX.CHAR1);

    internal TypeMap()
    {
    }

    private TypeMap(string ifxTypeName, InformixType ifxType, DbType dbType, Type type, Informix32.SQL_TYPE sql_type, Informix32.SQL_C sql_c, Informix32.SQL_C param_sql_c, int bsize, int csize, bool signType, Informix32.DATA_TYPE_INDEX index)
    {
        _ifxTypeName = ifxTypeName;
        _odbcType = ifxType;
        _dbType = dbType;
        _type = type;
        _sql_type = sql_type;
        _sql_c = sql_c;
        _param_sql_c = param_sql_c;
        _bufferSize = bsize;
        _columnSize = csize;
        _signType = signType;
        _index = index;
    }

    private void SetDefaults()
    {
    }

    internal TypeMap Clone()
    {
        return new TypeMap();
    }

    internal InformixType IfxNameToIfxType(string name, int precision)
    {
        switch (name)
        {
            case "CHAR":
                return 1 != precision ? InformixType.Char : InformixType.Char1;
            case "NCHAR":
                return InformixType.NChar;
            case "DATE":
                return InformixType.Date;
            case "INTEGER":
                return InformixType.Integer;
            case "INT8":
                return InformixType.Int8;
            case "SMALLINT":
                return InformixType.SmallInt;
            default:
                if (name.StartsWith("DATETIME"))
                {
                    return InformixType.DateTime;
                }
                switch (name)
                {
                    case "SERIAL":
                        return InformixType.Serial;
                    case "SERIAL8":
                        return InformixType.Serial8;
                    case "FLOAT":
                        return InformixType.Float;
                    case "SMALLFLOAT":
                        return InformixType.SmallFloat;
                    case "LVARCHAR":
                        return InformixType.LVarChar;
                    case "VARCHAR":
                        return InformixType.VarChar;
                    case "NVARCHAR":
                        return InformixType.NVarChar;
                    case "BOOLEAN":
                        return InformixType.Boolean;
                    case "BYTE":
                        return InformixType.Byte;
                    case "BLOB":
                        return InformixType.Blob;
                    case "TEXT":
                        return InformixType.Text;
                    case "CLOB":
                        return InformixType.Clob;
                    case "DECIMAL":
                        return InformixType.Decimal;
                    case "MONEY":
                        return InformixType.Money;
                    default:
                        if (name.StartsWith("INTERVAL"))
                        {
                            return GetIntervalSubType(name);
                        }
                        return name switch
                        {
                            "LIST" => InformixType.List,
                            "MULTISET" => InformixType.MultiSet,
                            "SET" => InformixType.Set,
                            "ROW" => InformixType.Row,
                            "SQLUDTFIXED" => InformixType.SQLUDTFixed,
                            "SQLUDTVAR" => InformixType.SQLUDTVar,
                            "BIGINT" => InformixType.BigInt,
                            "BIGSERIAL" => InformixType.BigSerial,
                            _ => InformixType.Other,
                        };
                }
        }
    }

    internal static InformixType GetIntervalSubType(string typeName)
    {
        if (typeName.StartsWith("INTERVAL YEAR") || typeName.StartsWith("INTERVAL MONTH"))
        {
            return InformixType.IntervalYearMonth;
        }
        return InformixType.IntervalDayFraction;
    }

    private TypeMap(InformixType odbcType, DbType dbType, Type type, Informix32.SQL_TYPE sql_type, Informix32.SQL_C sql_c, Informix32.SQL_C param_sql_c, int bsize, int csize, bool signType)
    {
        _odbcType = odbcType;
        _dbType = dbType;
        _type = type;
        _sql_type = sql_type;
        _sql_c = sql_c;
        _param_sql_c = param_sql_c;
        _bufferSize = bsize;
        _columnSize = csize;
        _signType = signType;
    }

    internal static TypeMap FromOdbcType(InformixType odbcType)
    {
        return odbcType switch
        {
            InformixType.BigInt => s_bigInt,
            InformixType.Binary => s_binary,
            InformixType.Bit => s_bit,
            InformixType.Char => _Char,
            InformixType.DateTime => s_dateTime,
            InformixType.Decimal => s_decimal,
            InformixType.Numeric => s_numeric,
            InformixType.Double => s_double,
            InformixType.Image => _Image,
            InformixType.Int => s_int,
            InformixType.NChar => s_NChar,
            InformixType.NText => _NText,
            InformixType.NVarChar => _NVarChar,
            InformixType.Real => s_real,
            InformixType.UniqueIdentifier => s_uniqueId,
            InformixType.SmallDateTime => s_smallDT,
            InformixType.SmallInt => s_smallInt,
            InformixType.Text => _Text,
            InformixType.Timestamp => s_timestamp,
            InformixType.TinyInt => s_tinyInt,
            InformixType.VarBinary => s_varBinary,
            InformixType.VarChar => s_VarChar,
            InformixType.Date => s_date,
            InformixType.Time => s_time,
            InformixType.Integer => s_int,
            InformixType.Float => s_double,
            InformixType.SmallFloat => s_SmallFloat,
            InformixType.Serial => s_Serial,
            InformixType.Money => s_Money,
            InformixType.Byte => s_Byte,
            InformixType.Int8 => s_Int8,
            InformixType.Serial8 => s_Serial8,
            InformixType.BigSerial => s_BigSerial,
            InformixType.LVarChar => s_LVarChar,
            InformixType.Blob => _Blob,
            InformixType.Clob => _Clob,
            InformixType.Char1 => s_Char1,
            InformixType.IntervalYearMonth => s_IntervalYearMonth,
            InformixType.IntervalDayFraction => s_IntervalDayFraction,
            _ => throw ODBC.UnknownOdbcType(odbcType),
        };
    }

    internal static TypeMap FromDbType(DbType dbType)
    {
        return dbType switch
        {
            DbType.AnsiString => s_VarChar,
            DbType.Binary => s_varBinary,
            DbType.Byte => s_tinyInt,
            DbType.Boolean => s_bit,
            DbType.Currency => s_decimal,
            DbType.Date => s_date,
            DbType.DateTime => s_dateTime,
            DbType.Decimal => s_decimal,
            DbType.Double => s_double,
            DbType.Guid => s_uniqueId,
            DbType.Int16 => s_smallInt,
            DbType.Int32 => s_int,
            DbType.Int64 => s_bigInt,
            DbType.Single => s_real,
            DbType.String => _NVarChar,
            DbType.Time => s_time,
            DbType.AnsiStringFixedLength => _Char,
            DbType.StringFixedLength => s_NChar,
            _ => throw ADP.DbTypeNotSupported(dbType, typeof(InformixType)),
        };
    }

    internal static TypeMap FromSystemType(Type dataType)
    {
        switch (Type.GetTypeCode(dataType))
        {
            case TypeCode.Empty:
                throw ADP.InvalidDataType(TypeCode.Empty);
            case TypeCode.Object:
                if (dataType == typeof(byte[]))
                {
                    return _Blob;
                }
                if (dataType == typeof(char[]))
                {
                    return _Clob;
                }
                if (dataType == typeof(TimeSpan) || dataType == typeof(InformixTimeSpan))
                {
                    return s_IntervalDayFraction;
                }
                if (dataType == typeof(InformixMonthSpan))
                {
                    return s_IntervalYearMonth;
                }
                if (dataType == typeof(InformixClob))
                {
                    return _Clob;
                }
                if (dataType == typeof(InformixBlob))
                {
                    return _Blob;
                }
                if (dataType == typeof(InformixSmartLOBLocator))
                {
                    return _SmartLOBLocator;
                }
                if (dataType == typeof(decimal))
                {
                    return s_Decimal;
                }
                if (dataType == typeof(InformixDecimal))
                {
                    return s_IfxDecimal;
                }
                if (dataType == typeof(InformixDateTime))
                {
                    return s_IfxDateTime;
                }
                throw ADP.UnknownDataType(dataType);
            case TypeCode.DBNull:
                throw ADP.InvalidDataType(TypeCode.DBNull);
            case TypeCode.Boolean:
                return s_bit;
            case TypeCode.Char:
            case TypeCode.String:
                return _NVarChar;
            case TypeCode.SByte:
                return s_smallInt;
            case TypeCode.Byte:
                return s_tinyInt;
            case TypeCode.Int16:
                return s_smallInt;
            case TypeCode.UInt16:
                return s_int;
            case TypeCode.Int32:
                return s_int;
            case TypeCode.UInt32:
                return s_bigInt;
            case TypeCode.Int64:
                return s_bigInt;
            case TypeCode.UInt64:
                return s_numeric;
            case TypeCode.Single:
                return s_real;
            case TypeCode.Double:
                return s_double;
            case TypeCode.Decimal:
                return s_numeric;
            case TypeCode.DateTime:
                return s_dateTime;
            default:
                throw ADP.UnknownDataTypeCode(dataType, Type.GetTypeCode(dataType));
        }
    }

    internal TypeMap FromIfxType(InformixType ifxType)
    {
        return ifxType switch
        {
            InformixType.BigInt => _BigInt,
            InformixType.Char => _Char,
            InformixType.DateTime => _IfxDateTime,
            InformixType.Decimal => _Decimal,
            InformixType.NChar => _NChar,
            InformixType.NVarChar => _NVarChar,
            InformixType.SmallInt => _SmallInt,
            InformixType.Text => _Text,
            InformixType.VarChar => s_VarChar,
            InformixType.Date => _Date,
            InformixType.Integer => _Integer,
            InformixType.Float => _Double,
            InformixType.SmallFloat => _SmallFloat,
            InformixType.Serial => _Serial,
            InformixType.Money => _Money,
            InformixType.Byte => _Byte,
            InformixType.Int8 => _Int8,
            InformixType.Serial8 => _Serial8,
            InformixType.Set => _Set,
            InformixType.MultiSet => _Multiset,
            InformixType.List => _List,
            InformixType.Row => _Row,
            InformixType.SQLUDTVar => _SQLUDTVar,
            InformixType.SQLUDTFixed => _SQLUDTFixed,
            InformixType.BigSerial => _BigSerial,
            InformixType.Other => _Unknown,
            InformixType.LVarChar => _LVarChar,
            InformixType.Blob => _Blob,
            InformixType.Clob => _Clob,
            InformixType.SmartLOBLocator => _SmartLOBLocator,
            InformixType.Boolean => _Boolean,
            InformixType.Char1 => _Char1,
            InformixType.IntervalYearMonth => _IntervalYearMonth,
            InformixType.IntervalDayFraction => _IntervalDayFraction,
            _ => throw ODBC.UnknownOdbcType(ifxType),
        };
    }

    internal TypeMap FromObjectType(Type dataType)
    {
        return FromObjectType(dataType, 0);
    }

    internal TypeMap FromObjectType(object datum)
    {
        if (DBNull.Value != datum && datum != null)
        {
            return FromObjectType(datum.GetType());
        }
        return _VarChar;
    }

    internal TypeMap FromObjectType(Type dataType, int length)
    {
        switch (Type.GetTypeCode(dataType))
        {
            case TypeCode.Empty:
                throw ADP.InvalidDataType(TypeCode.Empty);
            case TypeCode.Object:
                if (dataType == typeof(byte[]))
                {
                    return _Blob;
                }
                if (dataType == typeof(char[]))
                {
                    return _Clob;
                }
                if (dataType == typeof(TimeSpan) || dataType == typeof(InformixTimeSpan))
                {
                    return _IntervalDayFraction;
                }
                if (dataType == typeof(InformixMonthSpan))
                {
                    return _IntervalYearMonth;
                }
                if (dataType == typeof(InformixClob))
                {
                    return _Clob;
                }
                if (dataType == typeof(InformixBlob))
                {
                    return _Blob;
                }
                if (dataType == typeof(InformixSmartLOBLocator))
                {
                    return _SmartLOBLocator;
                }
                if (dataType == typeof(decimal))
                {
                    return _Decimal;
                }
                if (dataType == typeof(InformixDecimal))
                {
                    return _IfxDecimal;
                }
                if (dataType == typeof(InformixDateTime))
                {
                    return _IfxDateTime;
                }
                throw ADP.UnknownDataType(dataType);
            case TypeCode.DBNull:
                return _VarChar;
            case TypeCode.Boolean:
                return _Boolean;
            case TypeCode.Char:
                return _Char1;
            case TypeCode.SByte:
                throw ADP.InvalidDataType(TypeCode.Byte);
            case TypeCode.Byte:
                throw ADP.InvalidDataType(TypeCode.Byte);
            case TypeCode.Int16:
                return _SmallInt;
            case TypeCode.UInt16:
                return _Integer;
            case TypeCode.Int32:
                return _Integer;
            case TypeCode.UInt32:
                return _Int8;
            case TypeCode.Int64:
                return _Int8;
            case TypeCode.UInt64:
                return _Decimal;
            case TypeCode.Single:
                return _SmallFloat;
            case TypeCode.Double:
                return _Double;
            case TypeCode.Decimal:
                return _Decimal;
            case TypeCode.DateTime:
                return _IfxDateTime;
            case TypeCode.String:
                if (length > 32767)
                {
                    return _Text;
                }
                if (length > 4096)
                {
                    return _Char;
                }
                if (length <= 255)
                {
                    return s_VarChar;
                }
                return _LVarChar;
            default:
                throw ADP.UnknownDataTypeCode(dataType, Type.GetTypeCode(dataType));
        }
    }

    internal static TypeMap FromSqlType(Informix32.SQL_TYPE sqltype)
    {
        switch (sqltype)
        {
            case Informix32.SQL_TYPE.SS_TIME_EX:
            case Informix32.SQL_TYPE.SS_UTCDATETIME:
                throw ODBC.UnknownSQLType(sqltype);
            case Informix32.SQL_TYPE.SS_XML:
                return s_XML;
            case Informix32.SQL_TYPE.SS_UDT:
                return s_UDT;
            case Informix32.SQL_TYPE.SS_VARIANT:
                return s_variant;
            case Informix32.SQL_TYPE.INFXBIGINT:
                return s_bigInt;
            case Informix32.SQL_TYPE.RC_MULTISET:
                return _Text;
            case Informix32.SQL_TYPE.RC_SET:
                return _Text;
            case Informix32.SQL_TYPE.RC_LIST:
                return _Text;
            case Informix32.SQL_TYPE.RC_COLLECTION:
                return _Text;
            case Informix32.SQL_TYPE.RC_ROW:
                return _Text;
            case Informix32.SQL_TYPE.UDT_CLOB:
                return _Clob;
            case Informix32.SQL_TYPE.UDT_BLOB:
                return _Blob;
            case Informix32.SQL_TYPE.GUID:
                return s_uniqueId;
            case Informix32.SQL_TYPE.WLONGVARCHAR:
                return _Text;
            case Informix32.SQL_TYPE.WVARCHAR:
                return _NVarChar;
            case Informix32.SQL_TYPE.WCHAR:
                return s_NChar;
            case Informix32.SQL_TYPE.BIT:
                return s_bit;
            case Informix32.SQL_TYPE.TINYINT:
                return s_tinyInt;
            case Informix32.SQL_TYPE.BIGINT:
                return s_bigInt;
            case Informix32.SQL_TYPE.LONGVARBINARY:
                return _Image;
            case Informix32.SQL_TYPE.VARBINARY:
                return s_varBinary;
            case Informix32.SQL_TYPE.BINARY:
                return _Blob;
            case Informix32.SQL_TYPE.LONGVARCHAR:
                return _Clob;
            case Informix32.SQL_TYPE.CHAR:
                return _Char;
            case Informix32.SQL_TYPE.NUMERIC:
                return s_numeric;
            case Informix32.SQL_TYPE.DECIMAL:
                return s_decimal;
            case Informix32.SQL_TYPE.INTEGER:
                return s_int;
            case Informix32.SQL_TYPE.SMALLINT:
                return s_smallInt;
            case Informix32.SQL_TYPE.FLOAT:
                return s_double;
            case Informix32.SQL_TYPE.REAL:
                return s_real;
            case Informix32.SQL_TYPE.DOUBLE:
                return s_double;
            case Informix32.SQL_TYPE.TIMESTAMP:
            case Informix32.SQL_TYPE.TYPE_TIMESTAMP:
                return s_IfxDateTime;
            case Informix32.SQL_TYPE.VARCHAR:
                return s_VarChar;
            case Informix32.SQL_TYPE.TYPE_DATE:
                return s_date;
            case Informix32.SQL_TYPE.TYPE_TIME:
                return s_IfxDateTime;
            case Informix32.SQL_TYPE.INTERVAL_YEAR:
            case Informix32.SQL_TYPE.INTERVAL_MONTH:
            case Informix32.SQL_TYPE.INTERVAL_YEAR_TO_MONTH:
                return s_IntervalYearMonth;
            case Informix32.SQL_TYPE.INTERVAL_DAY:
            case Informix32.SQL_TYPE.INTERVAL_HOUR:
            case Informix32.SQL_TYPE.INTERVAL_MINUTE:
            case Informix32.SQL_TYPE.INTERVAL_SECOND:
            case Informix32.SQL_TYPE.INTERVAL_DAY_TO_HOUR:
            case Informix32.SQL_TYPE.INTERVAL_DAY_TO_MINUTE:
            case Informix32.SQL_TYPE.INTERVAL_DAY_TO_SECOND:
            case Informix32.SQL_TYPE.INTERVAL_HOUR_TO_MINUTE:
            case Informix32.SQL_TYPE.INTERVAL_HOUR_TO_SECOND:
            case Informix32.SQL_TYPE.INTERVAL_MINUTE_TO_SECOND:
                return s_IntervalDayFraction;
            default:
                throw ODBC.UnknownSQLType(sqltype);
        }
    }

    internal static TypeMap UpgradeSignedType(TypeMap typeMap, bool unsigned)
    {
        if (unsigned)
        {
            return typeMap._dbType switch
            {
                DbType.Int16 => s_int,
                DbType.Int32 => s_bigInt,
                DbType.Int64 => s_decimal,
                _ => typeMap,
            };
        }
        if (typeMap._dbType != DbType.Byte)
        {
            return typeMap;
        }
        return s_smallInt;
    }

    internal void SetUserDefinedTypeMaps(string format)
    {
        if (string.Compare(format, "bytes", ignoreCase: true) == 0)
        {
            _SQLUDTVar._dbType = _SQLUDTFixed._dbType = DbType.Binary;
            _SQLUDTVar._type = _SQLUDTFixed._type = typeof(byte[]);
            _SQLUDTFixed._sql_type = Informix32.SQL_TYPE.UDT_FIXED;
            _SQLUDTVar._sql_type = Informix32.SQL_TYPE.UDT_VAR;
            _SQLUDTVar._sql_c = _SQLUDTFixed._sql_c = Informix32.SQL_C.BINARY;
            _SQLUDTVar._param_sql_c = _SQLUDTFixed._param_sql_c = Informix32.SQL_C.BINARY;
            _SQLUDTFixed._columnSize = -1;
            _SQLUDTFixed._bufferSize = -1;
            _SQLUDTVar._columnSize = 32739;
            _SQLUDTVar._bufferSize = _SQLUDTVar._columnSize;
        }
        else
        {
            if (format != null && format.Length != 0 && !(format.ToLower(CultureInfo.InvariantCulture) == "string"))
            {
                throw new ArgumentException();
            }
            _SQLUDTVar._dbType = _SQLUDTFixed._dbType = DbType.String;
            _SQLUDTVar._type = _SQLUDTFixed._type = typeof(string);
            _SQLUDTVar._sql_type = _SQLUDTFixed._sql_type = Informix32.SQL_TYPE.VARCHAR;
            _SQLUDTVar._sql_c = _SQLUDTFixed._sql_c = Informix32.SQL_C.WCHAR;
            _SQLUDTVar._param_sql_c = _SQLUDTFixed._param_sql_c = Informix32.SQL_C.WCHAR;
            _SQLUDTVar._columnSize = _SQLUDTFixed._columnSize = 32739;
            _SQLUDTVar._bufferSize = _SQLUDTFixed._bufferSize = _SQLUDTFixed._columnSize * 2;
        }
    }
}
