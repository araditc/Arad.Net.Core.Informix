using System;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;


namespace Arad.Net.Core.Informix;

[TypeForwardedFrom("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public static class Informix32
{
    internal enum SQL_HANDLE : short
    {
        ENV = 1,
        DBC,
        STMT,
        DESC
    }

    internal enum SQL_ISOLATION
    {
        READ_UNCOMMITTED = 1,
        READ_COMMITTED = 2,
        REPEATABLE_READ = 4,
        SERIALIZABLE = 8
    }

    [TypeForwardedFrom("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public enum RETCODE
    {
        SUCCESS = 0,
        SUCCESS_WITH_INFO = 1,
        ERROR = -1,
        INVALID_HANDLE = -2,
        NO_DATA = 100
    }

    internal enum RetCode : short
    {
        SUCCESS = 0,
        SUCCESS_WITH_INFO = 1,
        ERROR = -1,
        INVALID_HANDLE = -2,
        NO_DATA = 100
    }

    internal enum SQL_ATTR
    {
        APP_ROW_DESC = 10010,
        APP_PARAM_DESC = 10011,
        IMP_ROW_DESC = 10012,
        IMP_PARAM_DESC = 10013,
        METADATA_ID = 10014,
        ODBC_VERSION = 200,
        CONNECTION_POOLING = 201,
        AUTOCOMMIT = 102,
        TXN_ISOLATION = 108,
        CURRENT_CATALOG = 109,
        LOGIN_TIMEOUT = 103,
        QUERY_TIMEOUT = 0,
        CONNECTION_DEAD = 1209,
        PREPARE_BATCH_ONLY = 2510,
        ENLIST_IN_DTC = 1207,
        LO_AUTOMATIC = 2262,
        INFX_DELIMIDENT = 2273,
        TRANSACTION_ID = 2274,
        DBLOCALE = 2275,
        LEAVE_TRAILING_SPACES = 2252,
        ODBC_TYPES_ONLY = 2257,
        LOCALIZE_DECIMALS = 2276,
        DEFAULT_DECIMAL = 2277,
        SKIP_PARSING = 2278,
        CALL_FROM_DOTNET = 2279,
        SINGLE_THREADED = 2280,
        ROW_BIND_TYPE = 5,
        ROW_ARRAY_SIZE = 27,
        ROW_STATUS_PTR = 25,
        ROWS_FETCHED_PTR = 26,
        MAX_FET_ARR_SIZE = 2326,
        SQL_COPT_SS_BASE = 1200,
        SQL_COPT_SS_ENLIST_IN_DTC = 1207,
        SQL_COPT_SS_TXN_ISOLATION = 1227,
        SQL_INFX_ATTR_CP_TIMEOUT = 2292,
        SQL_INFX_ATTR_CONNECTION_POOLING = 2293,
        SQL_INFX_ATTR_CP_MATCH = 2294,
        SQL_INFX_ATTR_CP_TOTAL_CONNECTIONS = 2295,
        SQL_INFX_ATTR_CP_TOTAL_ACTIVE = 2296,
        SQL_INFX_ATTR_CP_TOTAL_IDLE = 2297,
        SQL_INFX_ATTR_CLIENT_LABEL = 2298,
        SQL_INFX_ATTR_MIN_CONN_POOL_SIZE = 2299,
        SQL_INFX_ATTR_MAX_CONN_LIMIT = 2300
    }

    internal enum SQL_TYPE : short
    {
        CHAR = 1,
        VARCHAR = 12,
        LONGVARCHAR = -1,
        WCHAR = -8,
        WVARCHAR = -9,
        WLONGVARCHAR = -10,
        DECIMAL = 3,
        NUMERIC = 2,
        INFX_DECIMAL = -113,
        SMALLINT = 5,
        INTEGER = 4,
        REAL = 7,
        FLOAT = 6,
        DOUBLE = 8,
        BIT = -7,
        TINYINT = -6,
        BIGINT = -5,
        BINARY = -2,
        VARBINARY = -3,
        LONGVARBINARY = -4,
        INFXBIGINT = -114,
        TYPE_DATE = 91,
        TYPE_TIME = 92,
        TIMESTAMP = 11,
        TYPE_TIMESTAMP = 93,
        INTERVAL_YEAR = 101,
        INTERVAL_MONTH = 102,
        INTERVAL_DAY = 103,
        INTERVAL_HOUR = 104,
        INTERVAL_MINUTE = 105,
        INTERVAL_SECOND = 106,
        INTERVAL_YEAR_TO_MONTH = 107,
        INTERVAL_DAY_TO_HOUR = 108,
        INTERVAL_DAY_TO_MINUTE = 109,
        INTERVAL_DAY_TO_SECOND = 110,
        INTERVAL_HOUR_TO_MINUTE = 111,
        INTERVAL_HOUR_TO_SECOND = 112,
        INTERVAL_MINUTE_TO_SECOND = 113,
        UDT_FIXED = -100,
        UDT_VAR = -101,
        UDT_BLOB = -102,
        UDT_CLOB = -103,
        RC_ROW = -105,
        RC_COLLECTION = -106,
        RC_LIST = -107,
        RC_SET = -108,
        RC_MULTISET = -109,
        GUID = -11,
        SS_VARIANT = -150,
        SS_UDT = -151,
        SS_XML = -152,
        SS_UTCDATETIME = -153,
        SS_TIME_EX = -154
    }

    internal enum SQL_CONVERT : ushort
    {
        BIGINT = 53,
        BINARY,
        BIT,
        CHAR,
        DATE,
        DECIMAL,
        DOUBLE,
        FLOAT,
        INTEGER,
        LONGVARCHAR,
        NUMERIC,
        REAL,
        SMALLINT,
        TIME,
        TIMESTAMP,
        TINYINT,
        VARBINARY,
        VARCHAR,
        LONGVARBINARY
    }

    [Flags]
    internal enum SQL_CVT
    {
        CHAR = 1,
        NUMERIC = 2,
        DECIMAL = 4,
        INTEGER = 8,
        SMALLINT = 0x10,
        FLOAT = 0x20,
        REAL = 0x40,
        DOUBLE = 0x80,
        VARCHAR = 0x100,
        LONGVARCHAR = 0x200,
        BINARY = 0x400,
        VARBINARY = 0x800,
        BIT = 0x1000,
        TINYINT = 0x2000,
        BIGINT = 0x4000,
        DATE = 0x8000,
        TIME = 0x10000,
        TIMESTAMP = 0x20000,
        LONGVARBINARY = 0x40000,
        INTERVAL_YEAR_MONTH = 0x80000,
        INTERVAL_DAY_TIME = 0x100000,
        WCHAR = 0x200000,
        WLONGVARCHAR = 0x400000,
        WVARCHAR = 0x800000,
        GUID = 0x1000000
    }

    internal enum STMT : short
    {
        CLOSE,
        DROP,
        UNBIND,
        RESET_PARAMS
    }

    internal enum SQL_MAX
    {
        NUMERIC_LEN = 0x10
    }

    internal enum SQL_IS
    {
        POINTER = -4,
        INTEGER = -6,
        UINTEGER = -5,
        USMALLINT = -7,
        SMALLINT = -8
    }

    internal enum SQL_HC
    {
        OFF,
        ON
    }

    internal enum SQL_NB
    {
        OFF,
        ON
    }

    internal enum SQL_CA_SS
    {
        BASE = 1200,
        COLUMN_HIDDEN = 1211,
        COLUMN_KEY = 1212,
        VARIANT_TYPE = 1215,
        VARIANT_SQL_TYPE = 1216,
        VARIANT_SERVER_TYPE = 1217
    }

    internal enum SQL_SOPT_SS
    {
        BASE = 1225,
        HIDDEN_COLUMNS = 1227,
        NOBROWSETABLE = 1228
    }

    internal enum SQL_TXN
    {
        COMMIT,
        ROLLBACK
    }

    internal enum SQL_TRANSACTION
    {
        READ_UNCOMMITTED = 1,
        READ_COMMITTED = 2,
        REPEATABLE_READ = 4,
        SERIALIZABLE = 8,
        SNAPSHOT = 0x20
    }

    internal enum SQL_PARAM
    {
        INPUT = 1,
        INPUT_OUTPUT,
        RESULT_COL,
        OUTPUT,
        RETURN_VALUE
    }

    internal enum SQL_API : ushort
    {
        SQLCOLUMNS = 40,
        SQLEXECDIRECT = 11,
        SQLGETTYPEINFO = 47,
        SQLPROCEDURECOLUMNS = 66,
        SQLPROCEDURES = 67,
        SQLSTATISTICS = 53,
        SQLTABLES = 54
    }

    internal enum SQL_DESC : short
    {
        COUNT = 1001,
        TYPE = 1002,
        LENGTH = 1003,
        OCTET_LENGTH_PTR = 1004,
        PRECISION = 1005,
        SCALE = 1006,
        DATETIME_INTERVAL_CODE = 1007,
        DATETIME_INTERVAL_PRECISION = 26,
        NULLABLE = 1008,
        INDICATOR_PTR = 1009,
        DATA_PTR = 1010,
        NAME = 1011,
        UNNAMED = 1012,
        OCTET_LENGTH = 1013,
        ALLOC_TYPE = 1099,
        CONCISE_TYPE = 2,
        DISPLAY_SIZE = 6,
        UNSIGNED = 8,
        UPDATABLE = 10,
        AUTO_UNIQUE_VALUE = 11,
        TYPE_NAME = 14,
        TABLE_NAME = 15,
        SCHEMA_NAME = 16,
        CATALOG_NAME = 17,
        BASE_COLUMN_NAME = 22,
        BASE_TABLE_NAME = 23,
        INFX_BASE_TABLE_NAME = 35,
        INFX_QUALIFIER = -112
    }

    internal enum SQL_COLUMN
    {
        COUNT,
        NAME,
        TYPE,
        LENGTH,
        PRECISION,
        SCALE,
        DISPLAY_SIZE,
        NULLABLE,
        UNSIGNED,
        MONEY,
        UPDATABLE,
        AUTO_INCREMENT,
        CASE_SENSITIVE,
        SEARCHABLE,
        TYPE_NAME,
        TABLE_NAME,
        OWNER_NAME,
        QUALIFIER_NAME,
        LABEL
    }

    internal enum SQL_GROUP_BY
    {
        NOT_SUPPORTED,
        GROUP_BY_EQUALS_SELECT,
        GROUP_BY_CONTAINS_SELECT,
        NO_RELATION,
        COLLATE
    }

    internal enum SQL_SQL92_RELATIONAL_JOIN_OPERATORS
    {
        CORRESPONDING_CLAUSE = 1,
        CROSS_JOIN = 2,
        EXCEPT_JOIN = 4,
        FULL_OUTER_JOIN = 8,
        INNER_JOIN = 0x10,
        INTERSECT_JOIN = 0x20,
        LEFT_OUTER_JOIN = 0x40,
        NATURAL_JOIN = 0x80,
        RIGHT_OUTER_JOIN = 0x100,
        UNION_JOIN = 0x200
    }

    internal enum SQL_OJ_CAPABILITIES
    {
        LEFT = 1,
        RIGHT = 2,
        FULL = 4,
        NESTED = 8,
        NOT_ORDERED = 0x10,
        INNER = 0x20,
        ALL_COMPARISON_OPS = 0x40
    }

    internal enum SQL_UPDATABLE
    {
        READONLY,
        WRITE,
        READWRITE_UNKNOWN
    }

    internal enum SQL_IDENTIFIER_CASE
    {
        UPPER = 1,
        LOWER,
        SENSITIVE,
        MIXED
    }

    internal enum SQL_INDEX : short
    {
        UNIQUE,
        ALL
    }

    internal enum SQL_STATISTICS_RESERVED : short
    {
        QUICK,
        ENSURE
    }

    internal enum SQL_SPECIALCOLS : ushort
    {
        BEST_ROWID = 1,
        ROWVER
    }

    internal enum SQL_SCOPE : ushort
    {
        CURROW,
        TRANSACTION,
        SESSION
    }

    internal enum SQL_NULLABILITY : ushort
    {
        NO_NULLS,
        NULLABLE,
        UNKNOWN
    }

    internal enum SQL_SEARCHABLE
    {
        UNSEARCHABLE,
        LIKE_ONLY,
        ALL_EXCEPT_LIKE,
        SEARCHABLE
    }

    internal enum SQL_UNNAMED
    {
        NAMED,
        UNNAMED
    }

    internal enum HANDLER
    {
        IGNORE,
        THROW
    }

    internal enum SQL_STATISTICSTYPE
    {
        TABLE_STAT,
        INDEX_CLUSTERED,
        INDEX_HASHED,
        INDEX_OTHER
    }

    internal enum SQL_PROCEDURETYPE
    {
        UNKNOWN,
        PROCEDURE,
        FUNCTION
    }

    internal enum GENLIB_TYPE
    {
        DTTIME_T = 1,
        INTRVL_T,
        DEC_T
    }

    internal enum SQL_C : short
    {
        CHAR = 1,
        WCHAR = -8,
        SLONG = -16,
        SSHORT = -15,
        REAL = 7,
        DOUBLE = 8,
        BIT = -7,
        UTINYINT = -28,
        SBIGINT = -25,
        UBIGINT = -27,
        BINARY = -2,
        TIMESTAMP = 11,
        TYPE_DATE = 91,
        TYPE_TIME = 92,
        TYPE_TIMESTAMP = 93,
        INTERVAL_YEAR = 101,
        INTERVAL_MONTH = 102,
        INTERVAL_DAY = 103,
        INTERVAL_HOUR = 104,
        INTERVAL_MINUTE = 105,
        INTERVAL_SECOND = 106,
        INTERVAL_YEAR_TO_MONTH = 107,
        INTERVAL_DAY_TO_HOUR = 108,
        INTERVAL_DAY_TO_MINUTE = 109,
        INTERVAL_DAY_TO_SECOND = 110,
        INTERVAL_HOUR_TO_MINUTE = 111,
        INTERVAL_HOUR_TO_SECOND = 112,
        INTERVAL_MINUTE_TO_SECOND = 113,
        SB_LOCATOR = -111,
        NUMERIC = 2,
        INFX_DECIMAL = -113,
        GUID = -11,
        DEFAULT = 99,
        ARD_TYPE = -99,
        SINFXBIGINT = -114
    }

    internal enum SQL_INFO : ushort
    {
        DATA_SOURCE_NAME = 2,
        SERVER_NAME = 13,
        DRIVER_NAME = 6,
        DRIVER_VER = 7,
        ODBC_VER = 10,
        SEARCH_PATTERN_ESCAPE = 14,
        DBMS_VER = 18,
        DBMS_NAME = 17,
        IDENTIFIER_CASE = 28,
        IDENTIFIER_QUOTE_CHAR = 29,
        CATALOG_NAME_SEPARATOR = 41,
        DRIVER_ODBC_VER = 77,
        GROUP_BY = 88,
        KEYWORDS = 89,
        ORDER_BY_COLUMNS_IN_SELECT = 90,
        QUOTED_IDENTIFIER_CASE = 93,
        SQL_OJ_CAPABILITIES_30 = 115,
        SQL_OJ_CAPABILITIES_20 = 65003,
        SQL_SQL92_RELATIONAL_JOIN_OPERATORS = 161
    }

    internal enum DATA_TYPE_INDEX
    {
        SMALLINT = 1,
        INTEGER,
        SERIAL,
        INT8,
        BIGINT,
        SERIAL8,
        BIGSERIAL,
        SMALLFLOAT,
        DOUBLE,
        DECIMAL,
        IFXDECIMAL,
        MONEY,
        DATE,
        IFXDATETIME,
        CHAR,
        NCHAR,
        CHAR1,
        VARCHAR,
        NVARCHAR,
        LVARCHAR,
        ROW,
        SET,
        MULTISET,
        LIST,
        TEXT,
        BYTE,
        SMARTLOBLOCATOR,
        BOOLEAN,
        SQLUDTFIXED,
        SQLUDTVAR,
        CLOB,
        BLOB,
        UNKNOWN,
        INTERVALYEARMONTH,
        INTERVALDAYFRACTION
    }

    internal enum SQL_DRIVER
    {
        NOPROMPT,
        COMPLETE,
        PROMPT,
        COMPLETE_REQUIRED
    }

    internal enum SQL_PRIMARYKEYS : short
    {
        COLUMNNAME = 4
    }

    internal enum SQL_STATISTICS : short
    {
        INDEXNAME = 6,
        ORDINAL_POSITION = 8,
        COLUMN_NAME = 9
    }

    internal enum SQL_SPECIALCOLUMNSET : short
    {
        COLUMN_NAME = 2
    }

    internal static ASCIIEncoding encoder = new ASCIIEncoding();

    internal const short SQL_COMMIT = 0;

    internal const short SQL_ROLLBACK = 1;

    internal const int SQL_FALSE = 0;

    internal const int SQL_TRUE = 1;

    internal static readonly nint SQL_AUTOCOMMIT_OFF = ADP.PtrZero;

    internal static readonly nint SQL_AUTOCOMMIT_ON = new nint(1);

    private const int SIGNED_OFFSET = -20;

    private const int UNSIGNED_OFFSET = -22;

    internal const short SQL_ALL_TYPES = 0;

    internal static readonly nint SQL_HANDLE_NULL = ADP.PtrZero;

    internal const int SQL_NULL_DATA = -1;

    internal const int SQL_NO_TOTAL = -4;

    internal const int SQL_DEFAULT_PARAM = -5;

    internal const int COLUMN_NAME = 4;

    internal const int COLUMN_TYPE = 5;

    internal const int DATA_TYPE = 6;

    internal const int COLUMN_SIZE = 8;

    internal const int DECIMAL_DIGITS = 10;

    internal const int NUM_PREC_RADIX = 11;

    internal static readonly nint SQL_OV_ODBC3 = new nint(3);

    internal const int SQL_NTS = -3;

    internal static readonly nint SQL_CP_OFF = new nint(0);

    internal static readonly nint SQL_CP_ONE_PER_DRIVER = new nint(1);

    internal static readonly nint SQL_CP_ONE_PER_HENV = new nint(2);

    internal static readonly nint SQL_INFX_CP_STRICT_MATCH = new nint(1);

    internal static readonly nint SQL_INFX_CP_RELAXED_MATCH = new nint(2);

    internal static readonly nint SQL_INFX_CP_OFF = new nint(1);

    internal static readonly nint SQL_INFX_CP_ON = new nint(2);

    internal const int SQL_CD_TRUE = 1;

    internal const int SQL_CD_FALSE = 0;

    internal const int SQL_DTC_DONE = 0;

    internal const int SQL_IS_POINTER = -4;

    internal const int SQL_IS_PTR = 1;

    internal const int MAX_CONNECTION_STRING_LENGTH = 1024;

    internal const short SQL_DIAG_SQLSTATE = 4;

    internal const short SQL_RESULT_COL = 3;

    internal static string RetcodeToString(RetCode retcode)
    {
        return retcode switch
        {
            RetCode.SUCCESS => "SUCCESS",
            RetCode.SUCCESS_WITH_INFO => "SUCCESS_WITH_INFO",
            RetCode.INVALID_HANDLE => "INVALID_HANDLE",
            RetCode.NO_DATA => "NO_DATA",
            _ => "ERROR",
        };
    }

    internal static InformixErrorCollection GetDiagErrors(string source, OdbcHandle hrHandle, RetCode retcode)
    {
        InformixErrorCollection ifxErrorCollection = new InformixErrorCollection();
        GetDiagErrors(ifxErrorCollection, source, hrHandle, retcode);
        return ifxErrorCollection;
    }

    internal static void GetDiagErrors(InformixErrorCollection errors, string source, OdbcHandle hrHandle, RetCode retcode)
    {
        if (retcode == RetCode.SUCCESS)
        {
            return;
        }
        short num = 0;
        short cchActual = 0;
        StringBuilder stringBuilder = new StringBuilder(1024);
        bool flag = true;
        while (flag)
        {
            num++;
            retcode = hrHandle.GetDiagnosticRecord(num, out var sqlState, stringBuilder, out var nativeError, out cchActual);
            if (RetCode.SUCCESS_WITH_INFO == retcode && stringBuilder.Capacity - 1 < cchActual)
            {
                stringBuilder.Capacity = cchActual + 1;
                retcode = hrHandle.GetDiagnosticRecord(num, out sqlState, stringBuilder, out nativeError, out cchActual);
            }
            flag = retcode == RetCode.SUCCESS || retcode == RetCode.SUCCESS_WITH_INFO;
            if (flag)
            {
                errors.Add(new InformixError(source, stringBuilder.ToString(), sqlState, nativeError));
            }
        }
    }
}
