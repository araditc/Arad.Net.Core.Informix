using Arad.Net.Core.Informix.System.Data.ProviderBase;
using Arad.Net.Core.Informix.System.Data.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;


namespace Arad.Net.Core.Informix;

public sealed class InformixDataReader : DbDataReader
{
    private enum HasRowsStatus
    {
        DontKnow,
        HasRows,
        HasNoRows
    }

    internal sealed class QualifiedTableName
    {
        private string _catalogName;

        private string _schemaName;

        private string _tableName;

        private string _quotedTableName;

        private string _quoteChar;

        internal string Catalog => _catalogName;

        internal string Schema => _schemaName;

        internal string Table
        {
            get
            {
                return _tableName;
            }
            set
            {
                _quotedTableName = value;
                _tableName = UnQuote(value);
            }
        }

        internal string QuotedTable => _quotedTableName;

        internal string GetTable(bool flag)
        {
            if (!flag)
            {
                return Table;
            }
            return QuotedTable;
        }

        internal QualifiedTableName(string quoteChar)
        {
            _quoteChar = quoteChar;
        }

        internal QualifiedTableName(string quoteChar, string qualifiedname)
        {
            _quoteChar = quoteChar;
            string[] array = ParseProcedureName(qualifiedname, quoteChar, quoteChar);
            _catalogName = UnQuote(array[1]);
            _schemaName = UnQuote(array[2]);
            _quotedTableName = array[3];
            _tableName = UnQuote(array[3]);
        }

        private string UnQuote(string str)
        {
            if (str != null && str.Length > 0)
            {
                char c = _quoteChar[0];
                if (str[0] == c && str.Length > 1 && str[str.Length - 1] == c)
                {
                    str = str.Substring(1, str.Length - 2);
                }
            }
            return str;
        }

        internal static string[] ParseProcedureName(string name, string quotePrefix, string quoteSuffix)
        {
            string[] array = new string[4];
            if (!string.IsNullOrEmpty(name))
            {
                bool flag = !string.IsNullOrEmpty(quotePrefix) && !string.IsNullOrEmpty(quoteSuffix);
                int i = 0;
                int j;
                for (j = 0; j < array.Length; j++)
                {
                    if (i >= name.Length)
                    {
                        break;
                    }
                    int num = i;
                    if (flag && name.IndexOf(quotePrefix, i, quotePrefix.Length, StringComparison.Ordinal) == i)
                    {
                        for (i += quotePrefix.Length; i < name.Length; i += quoteSuffix.Length)
                        {
                            i = name.IndexOf(quoteSuffix, i, StringComparison.Ordinal);
                            if (i < 0)
                            {
                                i = name.Length;
                                break;
                            }
                            i += quoteSuffix.Length;
                            if (i >= name.Length || name.IndexOf(quoteSuffix, i, quoteSuffix.Length, StringComparison.Ordinal) != i)
                            {
                                break;
                            }
                        }
                    }
                    if (i < name.Length)
                    {
                        i = name.IndexOf(".", i, StringComparison.Ordinal);
                        if (i < 0 || j == array.Length - 1)
                        {
                            i = name.Length;
                        }
                    }
                    array[j] = name.Substring(num, i - num);
                    i += ".".Length;
                }
                int num2 = array.Length - 1;
                while (0 <= num2)
                {
                    array[num2] = 0 < j ? array[--j] : null;
                    num2--;
                }
            }
            return array;
        }
    }

    private sealed class MetaData
    {
        internal int ordinal;

        internal TypeMap typemap;

        internal SQLLEN size;

        internal byte precision;

        internal byte scale;

        internal bool isAutoIncrement;

        internal bool isUnique;

        internal bool isReadOnly;

        internal bool isNullable;

        internal bool isRowVersion;

        internal bool isLong;

        internal bool isKeyColumn;

        internal string baseSchemaName;

        internal string baseCatalogName;

        internal string baseTableName;

        internal string baseColumnName;
    }

    private InformixCommand _command;

    private int _recordAffected = -1;

    private FieldNameLookup _fieldNameLookup;

    private DbCache _dataCache;

    private HasRowsStatus _hasRows;

    private bool _isClosed;

    private bool _isRead;

    private bool _isValidResult;

    private bool _noMoreResults;

    private bool _noMoreRows;

    private bool _skipReadOnce;

    private int _hiddenColumns;

    internal CommandBehavior _commandBehavior;

    private int _row = -1;

    private int _column = -1;

    private long _sequentialBytesRead;

    private static int s_objectTypeCount;

    internal readonly int ObjectID = Interlocked.Increment(ref s_objectTypeCount);

    private MetaData[] _metadata;

    private DataTable _schemaTable;

    private string _cmdText;

    private CMDWrapper _cmdWrapper;

    private static bool _validResult;

    private static bool IsValidResult => _validResult;

    private CNativeBuffer Buffer
    {
        get
        {
            CNativeBuffer dataReaderBuf = _cmdWrapper._dataReaderBuf;
            if (dataReaderBuf == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            return dataReaderBuf;
        }
        set
        {
            _cmdWrapper._dataReaderBuf = value;
        }
    }

    private InformixConnection Connection
    {
        get
        {
            if (_cmdWrapper != null)
            {
                return _cmdWrapper.Connection;
            }
            return null;
        }
    }

    internal InformixCommand Command
    {
        get
        {
            return _command;
        }
        set
        {
            _command = value;
        }
    }

    private OdbcStatementHandle StatementHandle => _cmdWrapper.StatementHandle;

    private OdbcStatementHandle KeyInfoStatementHandle => _cmdWrapper.KeyInfoStatement;

    internal bool IsCancelingCommand
    {
        get
        {
            if (_command != null)
            {
                return _command.Canceling;
            }
            return false;
        }
    }

    internal bool IsNonCancelingCommand
    {
        get
        {
            if (_command != null)
            {
                return !_command.Canceling;
            }
            return false;
        }
    }

    public override int Depth
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsClosed)
            {
                throw ADP.DataReaderClosed("Depth");
            }
            ifxTrace?.ApiExit();
            return 0;
        }
    }

    public override int FieldCount
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsClosed)
            {
                throw ADP.DataReaderClosed("FieldCount");
            }
            if (_noMoreResults)
            {
                ifxTrace?.ApiExit();
                return 0;
            }
            if (_dataCache == null)
            {
                short cColsAffected;
                Informix32.RetCode retCode = FieldCountNoThrow(out cColsAffected);
                if (retCode != 0)
                {
                    Connection.HandleError(StatementHandle, retCode);
                }
            }
            ifxTrace?.ApiExit();
            if (_dataCache == null)
            {
                return 0;
            }
            return _dataCache._count;
        }
    }

    public override bool HasRows
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (IsClosed)
            {
                throw ADP.DataReaderClosed("HasRows");
            }
            if (_hasRows == HasRowsStatus.DontKnow)
            {
                Read();
                _skipReadOnce = true;
            }
            ifxTrace?.ApiExit();
            return _hasRows == HasRowsStatus.HasRows;
        }
    }

    public override bool IsClosed
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _isClosed;
        }
    }

    public override int RecordsAffected
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _recordAffected;
        }
    }

    public override object this[int i]
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return GetValue(i);
        }
    }

    public override object this[string value]
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return GetValue(GetOrdinal(value));
        }
    }

    internal InformixDataReader(InformixCommand command, CMDWrapper cmdWrapper, CommandBehavior commandbehavior)
    {
        _command = command;
        _commandBehavior = commandbehavior;
        _cmdText = command.CommandText;
        _cmdWrapper = cmdWrapper;
    }

    internal bool IsBehavior(CommandBehavior behavior)
    {
        return IsCommandBehavior(behavior);
    }

    internal Informix32.RetCode FieldCountNoThrow(out short cColsAffected)
    {
        if (IsCancelingCommand)
        {
            cColsAffected = 0;
            return Informix32.RetCode.ERROR;
        }
        Informix32.RetCode retCode = StatementHandle.NumberOfResultColumns(out cColsAffected);
        if (retCode == Informix32.RetCode.SUCCESS)
        {
            _hiddenColumns = 0;
            if (IsCommandBehavior(CommandBehavior.KeyInfo) && !Connection.ProviderInfo.NoSqlSoptSSNoBrowseTable && !Connection.ProviderInfo.NoSqlSoptSSHiddenColumns)
            {
                for (int i = 0; i < cColsAffected; i++)
                {
                    if (GetColAttribute(i, (Informix32.SQL_DESC)1211, (Informix32.SQL_COLUMN)(-1), Informix32.HANDLER.IGNORE).ToInt64() == 1)
                    {
                        _hiddenColumns = cColsAffected - i;
                        cColsAffected = (short)i;
                        break;
                    }
                }
            }
            _dataCache = new DbCache(this, cColsAffected);
        }
        else
        {
            cColsAffected = 0;
        }
        return retCode;
    }

    private SQLLEN GetRowCount()
    {
        if (!IsClosed)
        {
            SQLLEN rowCount;
            Informix32.RetCode retCode = StatementHandle.RowCount(out rowCount);
            if (retCode == Informix32.RetCode.SUCCESS || Informix32.RetCode.SUCCESS_WITH_INFO == retCode)
            {
                return rowCount;
            }
        }
        return -1;
    }

    internal int CalculateRecordsAffected(int cRowsAffected)
    {
        if (0 <= cRowsAffected)
        {
            if (-1 == _recordAffected)
            {
                _recordAffected = cRowsAffected;
            }
            else
            {
                _recordAffected += cRowsAffected;
            }
        }
        return _recordAffected;
    }

    public override void Close()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        Close(disposing: false);
    }

    private void Close(bool disposing)
    {
        Exception ex = null;
        CMDWrapper cmdWrapper = _cmdWrapper;
        if (cmdWrapper != null && cmdWrapper.StatementHandle != null)
        {
            if (IsNonCancelingCommand)
            {
                NextResult(disposing, !disposing);
                if (_command != null)
                {
                    if (_command.HasParameters)
                    {
                        _command.Parameters.GetOutputValues(_cmdWrapper);
                    }
                    cmdWrapper.FreeStatementHandle(Informix32.STMT.CLOSE);
                    _command.CloseFromDataReader();
                }
            }
            cmdWrapper.FreeKeyInfoStatementHandle(Informix32.STMT.CLOSE);
        }
        if (_command != null)
        {
            _command.CloseFromDataReader();
            if (IsCommandBehavior(CommandBehavior.CloseConnection))
            {
                _command.Parameters.RebindCollection = true;
                Connection.Close();
            }
        }
        else
        {
            cmdWrapper?.Dispose();
        }
        _command = null;
        _isClosed = true;
        _dataCache = null;
        _metadata = null;
        _schemaTable = null;
        _isRead = false;
        _hasRows = HasRowsStatus.DontKnow;
        _isValidResult = false;
        _noMoreResults = true;
        _noMoreRows = true;
        _fieldNameLookup = null;
        SetCurrentRowColumnInfo(-1, 0);
        if (ex != null && !disposing)
        {
            throw ex;
        }
        _cmdWrapper = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Close(disposing: true);
        }
    }

    public override string GetDataTypeName(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._typename == null)
            {
                schema._typename = GetColAttributeStr(i, Informix32.SQL_DESC.TYPE_NAME, Informix32.SQL_COLUMN.TYPE_NAME, Informix32.HANDLER.THROW);
            }
            ifxTrace?.ApiExit();
            return schema._typename;
        }
        throw ADP.DataReaderNoData();
    }

    public override IEnumerator GetEnumerator()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return new DbEnumerator((IDataReader)this, IsCommandBehavior(CommandBehavior.CloseConnection));
    }

    public override Type GetFieldType(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._type == null)
            {
                schema._type = GetSqlType(i)._type;
            }
            ifxTrace?.ApiExit();
            return schema._type;
        }
        throw ADP.DataReaderNoData();
    }

    public override string GetName(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._name == null)
            {
                schema._name = GetColAttributeStr(i, Informix32.SQL_DESC.NAME, Informix32.SQL_COLUMN.NAME, Informix32.HANDLER.THROW);
                if (schema._name == null)
                {
                    schema._name = "";
                }
            }
            ifxTrace?.ApiExit();
            return schema._name;
        }
        throw ADP.DataReaderNoData();
    }

    public override int GetOrdinal(string value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_fieldNameLookup == null)
        {
            if (_dataCache == null)
            {
                throw ADP.DataReaderNoData();
            }
            _fieldNameLookup = new FieldNameLookup(this, -1);
        }
        ifxTrace?.ApiExit();
        return _fieldNameLookup.GetOrdinal(value);
    }

    private int IndexOf(string value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_fieldNameLookup == null)
        {
            if (_dataCache == null)
            {
                throw ADP.DataReaderNoData();
            }
            _fieldNameLookup = new FieldNameLookup(this, -1);
        }
        ifxTrace?.ApiExit();
        return _fieldNameLookup.IndexOf(value);
    }

    private bool IsCommandBehavior(CommandBehavior condition)
    {
        return condition == (condition & _commandBehavior);
    }

    internal object GetValue(int i, TypeMap typemap)
    {
        switch (typemap._sql_type)
        {
            case Informix32.SQL_TYPE.WLONGVARCHAR:
            case Informix32.SQL_TYPE.WVARCHAR:
            case Informix32.SQL_TYPE.WCHAR:
            case Informix32.SQL_TYPE.LONGVARCHAR:
            case Informix32.SQL_TYPE.CHAR:
            case Informix32.SQL_TYPE.VARCHAR:
                return internalGetString(i);
            case Informix32.SQL_TYPE.INTERVAL_YEAR_TO_MONTH:
                return internalGetIfxMonthSpan(i);
            case Informix32.SQL_TYPE.INTERVAL_DAY_TO_SECOND:
                return internalGetIfxTimeSpan(i);
            case Informix32.SQL_TYPE.RC_MULTISET:
            case Informix32.SQL_TYPE.RC_SET:
            case Informix32.SQL_TYPE.RC_LIST:
            case Informix32.SQL_TYPE.RC_ROW:
            case Informix32.SQL_TYPE.UDT_VAR:
            case Informix32.SQL_TYPE.UDT_FIXED:
                if (typemap._dbType == DbType.String)
                {
                    return internalGetString(i, typemap._odbcType);
                }
                return internalGetBytes(i);
            case Informix32.SQL_TYPE.NUMERIC:
            case Informix32.SQL_TYPE.DECIMAL:
                return internalGetDecimal(i);
            case Informix32.SQL_TYPE.SMALLINT:
                return internalGetInt16(i);
            case Informix32.SQL_TYPE.INTEGER:
                return internalGetInt32(i);
            case Informix32.SQL_TYPE.REAL:
                return internalGetFloat(i);
            case Informix32.SQL_TYPE.FLOAT:
            case Informix32.SQL_TYPE.DOUBLE:
                return internalGetDouble(i);
            case Informix32.SQL_TYPE.BIT:
                return internalGetBoolean(i);
            case Informix32.SQL_TYPE.TINYINT:
                return internalGetByte(i);
            case Informix32.SQL_TYPE.BIGINT:
                return internalGetInt64(i);
            case Informix32.SQL_TYPE.LONGVARBINARY:
            case Informix32.SQL_TYPE.VARBINARY:
            case Informix32.SQL_TYPE.BINARY:
                return internalGetBytes(i);
            case Informix32.SQL_TYPE.TYPE_DATE:
                return internalGetIfxDateTime(i);
            case Informix32.SQL_TYPE.TYPE_TIME:
                return internalGetTime(i);
            case Informix32.SQL_TYPE.TYPE_TIMESTAMP:
                return internalGetIfxDateTime(i);
            case Informix32.SQL_TYPE.GUID:
                return internalGetGuid(i);
            case Informix32.SQL_TYPE.SS_VARIANT:
                if (_isRead)
                {
                    if (_dataCache.AccessIndex(i) == null && QueryFieldInfo(i, Informix32.SQL_C.BINARY, out var _))
                    {
                        Informix32.SQL_TYPE sqltype = (Informix32.SQL_TYPE)(int)GetColAttribute(i, (Informix32.SQL_DESC)1216, (Informix32.SQL_COLUMN)(-1), Informix32.HANDLER.THROW);
                        return GetValue(i, TypeMap.FromSqlType(sqltype));
                    }
                    return _dataCache[i];
                }
                throw ADP.DataReaderNoData();
            default:
                return internalGetBytes(i);
        }
    }

    private static object GetSytemTypeFromCacheType(Type systemType, Type cacheEntryType, object cacheEntry)
    {
        object result = null;
        if (InformixParameterConverter.IsNull(cacheEntry))
        {
            result = DBNull.Value;
        }
        else if (typeof(string) == systemType)
        {
            result = cacheEntry.ToString();
        }
        else if (typeof(char) == systemType)
        {
            result = cacheEntry.ToString().ToCharArray(0, 1)[0];
        }
        else if (typeof(byte[]) == systemType)
        {
            result = cacheEntry;
        }
        else if (typeof(InformixTimeSpan) == cacheEntryType && typeof(TimeSpan) == systemType)
        {
            try
            {
                result = (TimeSpan)(InformixTimeSpan)cacheEntry;
            }
            catch
            {
                return null;
            }
        }
        else if (typeof(InformixDecimal) == cacheEntryType)
        {
            InformixDecimal ifxDecimal = (InformixDecimal)cacheEntry;
            if (typeof(decimal) == systemType)
            {
                result = (decimal)ifxDecimal;
            }
            else if (typeof(double) == systemType)
            {
                result = (double)ifxDecimal;
            }
        }
        else if (typeof(InformixDateTime) == cacheEntryType)
        {
            result = (DateTime)(InformixDateTime)cacheEntry;
        }
        else
        {
            if (!(typeof(string) == cacheEntryType))
            {
                throw new InvalidCastException();
            }
            string s = (string)cacheEntry;
            if (typeof(decimal) == systemType)
            {
                result = decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent);
            }
            else if (typeof(InformixDecimal) == systemType)
            {
                result = InformixDecimal.Parse(s);
            }
            else if (typeof(double) == systemType)
            {
                result = double.Parse(s);
            }
        }
        return result;
    }

    public override object GetValue(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        object result = null;
        if (_isRead)
        {
            object obj = null;
            if (_dataCache.AccessIndex(i) == null)
            {
                _dataCache[i] = GetValue(i, GetSqlType(i));
            }
            obj = _dataCache[i];
            if (obj != null)
            {
                Type type = obj.GetType();
                Type fieldType = GetFieldType(i);
                result = !(type != fieldType) ? obj : GetSytemTypeFromCacheType(fieldType, type, obj);
            }
            ifxTrace?.ApiExit();
            return result;
        }
        throw ADP.DataReaderNoData();
    }

    public override int GetValues(object[] values)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_isRead)
        {
            int num = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < num; i++)
            {
                values[i] = GetValue(i);
            }
            ifxTrace?.ApiExit();
            return num;
        }
        throw ADP.DataReaderNoData();
    }

    private TypeMap GetSqlType(int i)
    {
        DbSchemaInfo schema = _dataCache.GetSchema(i);
        TypeMap typeMap;
        if (!schema._dbtype.HasValue)
        {
            InformixType ifxType = GetIfxType(i);
            schema._dbtype = (Informix32.SQL_TYPE)(int)GetColAttribute(i, Informix32.SQL_DESC.CONCISE_TYPE, Informix32.SQL_COLUMN.TYPE, Informix32.HANDLER.THROW);
            typeMap = TypeMap.FromSqlType(schema._dbtype.Value);
            if (typeMap._signType)
            {
                bool unsigned = GetColAttribute(i, Informix32.SQL_DESC.UNSIGNED, Informix32.SQL_COLUMN.UNSIGNED, Informix32.HANDLER.THROW).ToInt64() != 0;
                typeMap = TypeMap.UpgradeSignedType(typeMap, unsigned);
                schema._dbtype = typeMap._sql_type;
            }
            if (typeMap._odbcType == InformixType.IntervalYearMonth || typeMap._odbcType == InformixType.IntervalDayFraction)
            {
                SetIntervalPrecision(i, ifxType);
            }
        }
        else
        {
            typeMap = TypeMap.FromSqlType(schema._dbtype.Value);
        }
        Connection.SetSupportedType(schema._dbtype.Value);
        return typeMap;
    }

    public override bool IsDBNull(int i)
    {
        if (!IsCommandBehavior(CommandBehavior.SequentialAccess))
        {
            object value = GetValue(i);
            if (value == null && IsValidResult)
            {
                return false;
            }
            return InformixParameterConverter.IsNull(value);
        }
        object obj = _dataCache[i];
        if (obj != null)
        {
            return InformixParameterConverter.IsNull(obj);
        }
        TypeMap sqlType = GetSqlType(i);
        if (sqlType._bufferSize > 0)
        {
            object value = GetValue(i);
            if (value == null && IsValidResult)
            {
                return false;
            }
            return InformixParameterConverter.IsNull(value);
        }
        int cbLengthOrIndicator;
        return !QueryFieldInfo(i, sqlType._sql_c, out cbLengthOrIndicator);
    }

    public override byte GetByte(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (byte)internalGetByte(i);
    }

    private object internalGetByte(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.UTINYINT))
            {
                _dataCache[i] = Buffer.ReadByte(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override char GetChar(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (char)internalGetChar(i);
    }

    private object internalGetChar(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.WCHAR))
            {
                _dataCache[i] = Buffer.ReadChar(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override short GetInt16(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (GetIfxType(i) != InformixType.SmallInt)
        {
            throw new InvalidCastException();
        }
        short result = (short)internalGetInt16(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetInt16(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.SSHORT))
            {
                _dataCache[i] = Buffer.ReadInt16(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public long GetBigInt(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        if (GetIfxType(i) != InformixType.BigInt && GetIfxType(i) != InformixType.BigSerial)
        {
            throw new InvalidCastException();
        }
        long result = (long)internalGetBigInt(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetBigInt(int i)
    {
        object obj = null;
        if (_isRead)
        {
            obj = _dataCache[i];
            if (obj == null)
            {
                if (GetData(i, Informix32.SQL_C.SBIGINT))
                {
                    object obj3 = _dataCache[i] = Buffer.ReadInt64(0);
                    obj = obj3;
                }
                else
                {
                    obj = _dataCache[i];
                }
            }
            return obj;
        }
        throw ADP.DataReaderNoData();
    }

    private static decimal ReadIntervalStruct(CNativeBuffer buffer)
    {
        switch ((Informix32.SQL_TYPE)(short)(buffer.ReadInt32(0) + 100))
        {
            case Informix32.SQL_TYPE.INTERVAL_YEAR:
            case Informix32.SQL_TYPE.INTERVAL_MONTH:
            case Informix32.SQL_TYPE.INTERVAL_YEAR_TO_MONTH:
                {
                    int num = buffer.ReadInt32(4);
                    long num2 = buffer.ReadInt32(8);
                    num2 *= 12;
                    num2 += buffer.ReadInt32(12);
                    if (num != 0)
                    {
                        num2 *= -1;
                    }
                    return num2;
                }
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
                {
                    int num = buffer.ReadInt32(4);
                    decimal result = buffer.ReadInt32(8) * 864000000000m;
                    result += buffer.ReadInt32(12) * 36000000000m;
                    result += buffer.ReadInt32(16) * 600000000m;
                    result += buffer.ReadInt32(20) * 10000000m;
                    result += buffer.ReadInt32(24);
                    if (num != 0)
                    {
                        result *= -1m;
                    }
                    return result;
                }
            default:
                return default;
        }
    }

    public TimeSpan GetTimeSpan(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (InformixType.IntervalDayFraction != ifxType)
        {
            throw new InvalidCastException();
        }
        TimeSpan result;
        try
        {
            InformixTimeSpan ifxTimeSpan = (InformixTimeSpan)internalGetIfxTimeSpan(i);
            result = (TimeSpan)ifxTimeSpan;
        }
        catch (OverflowException)
        {
            throw new OverflowException();
        }
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixTimeSpan GetIfxTimeSpan(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (InformixType.IntervalDayFraction != ifxType)
        {
            throw new InvalidCastException();
        }
        SetIntervalPrecision(i, ifxType);
        InformixTimeSpan result = (InformixTimeSpan)internalGetIfxTimeSpan(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetIfxTimeSpan(int i)
    {
        object obj = null;
        if (_isRead)
        {
            obj = _dataCache[i];
            if (obj == null)
            {
                if (GetData(i, Informix32.SQL_C.ARD_TYPE))
                {
                    decimal ticks = ReadIntervalStruct(Buffer);
                    Qualifier.Decode(_dataCache.GetSchema(i)._qualifier, out var start, out var end);
                    object obj3 = _dataCache[i] = new InformixTimeSpan(ticks, start, end);
                    obj = obj3;
                }
                else
                {
                    object obj3 = _dataCache[i] = InformixTimeSpan.Null;
                    obj = obj3;
                }
            }
            return obj;
        }
        throw ADP.DataReaderNoData();
    }

    public InformixMonthSpan GetIfxMonthSpan(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (InformixType.IntervalYearMonth != ifxType)
        {
            throw new InvalidCastException();
        }
        SetIntervalPrecision(i, ifxType);
        InformixMonthSpan result = (InformixMonthSpan)internalGetIfxMonthSpan(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetIfxMonthSpan(int i)
    {
        object obj = null;
        if (_isRead)
        {
            obj = _dataCache[i];
            if (obj == null)
            {
                int qualifier = _dataCache.GetSchema(i)._qualifier;
                if (GetData(i, Informix32.SQL_C.ARD_TYPE))
                {
                    long months = (long)ReadIntervalStruct(Buffer);
                    Qualifier.Decode(qualifier, out var start, out var end);
                    object obj3 = _dataCache[i] = new InformixMonthSpan(months, start, end);
                    obj = obj3;
                }
                else
                {
                    object obj3 = _dataCache[i] = InformixMonthSpan.Null;
                    obj = obj3;
                }
            }
            return obj;
        }
        throw ADP.DataReaderNoData();
    }

    private static IntervalDateTime ReadTimestampStruct(CNativeBuffer buffer, InformixType ifxType)
    {
        IntervalDateTime result = new IntervalDateTime(t: false);
        result.Year = buffer.ReadInt16(0);
        result.Month = buffer.ReadInt16(2);
        result.Day = buffer.ReadInt16(4);
        if (ifxType == InformixType.DateTime)
        {
            result.Hour = buffer.ReadInt16(6);
            result.Minute = buffer.ReadInt16(8);
            result.Second = buffer.ReadInt16(10);
            result.Fraction = buffer.ReadInt32(12);
        }
        return result;
    }

    public InformixDateTime GetIfxDateTime(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (ifxType != InformixType.DateTime && ifxType != InformixType.Date)
        {
            throw new InvalidCastException();
        }
        InformixDateTime result = (InformixDateTime)internalGetIfxDateTime(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetIfxDateTime(int i)
    {
        if (_isRead)
        {
            object obj = _dataCache.AccessIndex(i);
            if (obj != null && obj.GetType() != typeof(InformixDateTime))
            {
                obj = null;
            }
            if (obj == null)
            {
                InformixType ifxType = GetIfxType(i);
                InformixTimeUnit start;
                InformixTimeUnit end;
                if (ifxType == InformixType.Date)
                {
                    start = InformixTimeUnit.Year;
                    end = InformixTimeUnit.Day;
                }
                else
                {
                    int descFieldInt = GetDescFieldInt32(i, Informix32.SQL_DESC.INFX_QUALIFIER, Informix32.HANDLER.IGNORE);
                    Qualifier.Decode(descFieldInt, out start, out end);
                }
                if (GetData(i, Informix32.SQL_C.TYPE_TIMESTAMP))
                {
                    IntervalDateTime idt = ReadTimestampStruct(Buffer, ifxType);
                    idt.startTU = start;
                    idt.endTU = end;
                    if (end >= InformixTimeUnit.Fraction1)
                    {
                        idt.Fraction /= (int)Math.Pow(10.0, (double)(8 - (end - 11)));
                    }
                    InformixDateTime ifxDateTime = new InformixDateTime(idt);
                    _dataCache[i] = ifxDateTime;
                }
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    private object internalGetString(int i, InformixType iType)
    {
        object obj = _dataCache[i];
        if (_isRead)
        {
            if (obj == null)
            {
                obj = iType != InformixType.Clob ? internalGetStringData(i) : internalGetClobData(i);
            }
            return obj;
        }
        throw ADP.DataReaderNoData();
    }

    private object internalGetClobData(int i)
    {
        object obj = _dataCache[i];
        if (_isRead)
        {
            if (obj == null)
            {
                if (GetData(i, Informix32.SQL_C.SB_LOCATOR))
                {
                    byte[] array = new byte[72];
                    Marshal.Copy(Buffer.Address, array, 0, array.Length);
                    InformixSmartLOBLocator locator = new InformixSmartLOBLocator(array);
                    InformixClob ifxClob = new InformixClob(_command.Connection, locator);
                    ifxClob.Open(InformixSmartLOBOpenMode.ReadOnly);
                    long internalSize = ifxClob.internalSize;
                    char[] array2 = new char[(int)internalSize];
                    long num = ifxClob.internalRead(array2);
                    ifxClob.Close();
                    object obj2 = _dataCache[i] = new string(array2);
                    obj = obj2;
                }
                else
                {
                    obj = _dataCache[i];
                }
            }
            return obj;
        }
        throw ADP.DataReaderNoData();
    }

    private object internalGetStringData(int i)
    {
        object obj = _dataCache[i];
        int num = Buffer.Length - 2;
        string text = null;
        int cbLengthOrIndicator = 0;
        if (GetData(i, Informix32.SQL_C.WCHAR, num, out cbLengthOrIndicator))
        {
            if (cbLengthOrIndicator <= num)
            {
                text = (string)Buffer.MarshalToManaged(0, Informix32.SQL_C.WCHAR, -3);
                return _dataCache[i] = text;
            }
            int num2 = cbLengthOrIndicator - num;
            int num3 = num2 / 2;
            CNativeBuffer buffer = new CNativeBuffer(65536 + num2 + 2);
            char[] array = new char[cbLengthOrIndicator + 2];
            StringBuilder stringBuilder = new StringBuilder(cbLengthOrIndicator / 2);
            Marshal.Copy(Buffer.Address, array, 0, num / 2);
            stringBuilder.Append(array, 0, num / 2);
            Buffer = buffer;
            num = Buffer.Length - 2;
            while (cbLengthOrIndicator > 0)
            {
                bool data = GetData(i, Informix32.SQL_C.WCHAR, num, out cbLengthOrIndicator);
                int num4 = Math.Min(cbLengthOrIndicator, num) / 2;
                Marshal.Copy(Buffer.Address, array, 0, num4);
                stringBuilder.Append(array, 0, num4);
                if (cbLengthOrIndicator <= num)
                {
                    cbLengthOrIndicator = 0;
                }
            }
            return _dataCache[i] = stringBuilder.ToString();
        }
        return _dataCache[i];
    }

    public InformixDecimal GetIfxDecimal(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (ifxType != InformixType.Decimal && ifxType != InformixType.Money)
        {
            throw new InvalidCastException();
        }
        object obj = internalGetIfxDecimal(i);
        Type type = obj.GetType();
        InformixDecimal result;
        if (obj == DBNull.Value)
        {
            result = InformixDecimal.Null;
        }
        else if (!(typeof(string) == type))
        {
            result = !(typeof(decimal) == type) ? (InformixDecimal)obj : (InformixDecimal)(decimal)obj;
        }
        else
        {
            try
            {
                result = InformixDecimal.Parse((string)obj);
            }
            catch (OverflowException ex)
            {
                throw ex;
            }
        }
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetIfxDecimal(int i)
    {
        if (_isRead)
        {
            object obj = _dataCache[i];
            if (obj == null)
            {
                obj = internalGetString(i, InformixType.Decimal);
            }
            return obj;
        }
        throw ADP.DataReaderNoData();
    }

    public override int GetInt32(int i)
    {
        InformixType ifxType = GetIfxType(i);
        if (ifxType != InformixType.Integer && ifxType != InformixType.Serial)
        {
            throw new InvalidCastException();
        }
        int num = 0;
        try
        {
            return Convert.ToInt32(internalGetInt32(i));
        }
        catch (InvalidCastException)
        {
            return 0;
        }
    }

    private object internalGetInt32(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.SLONG))
            {
                _dataCache[i] = Buffer.ReadInt32(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override long GetInt64(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        if (GetIfxType(i) != InformixType.Int8 && GetIfxType(i) != InformixType.Serial8 && GetIfxType(i) != InformixType.BigInt)
        {
            throw new InvalidCastException();
        }
        long result = (long)internalGetInt64(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetInt64(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.WCHAR))
            {
                string s = (string)Buffer.MarshalToManaged(0, Informix32.SQL_C.WCHAR, -3);
                _dataCache[i] = long.Parse(s, CultureInfo.InvariantCulture);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override bool GetBoolean(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        if (GetIfxType(i) != InformixType.Boolean)
        {
            throw new InvalidCastException();
        }
        ifxTrace?.ApiExit();
        return (bool)internalGetBoolean(i);
    }

    private object internalGetBoolean(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.BIT))
            {
                _dataCache[i] = Buffer.MarshalToManaged(0, Informix32.SQL_C.BIT, -1);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override float GetFloat(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        if (GetIfxType(i) != InformixType.SmallFloat)
        {
            throw new InvalidCastException();
        }
        float result = (float)internalGetFloat(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetFloat(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.REAL))
            {
                _dataCache[i] = Buffer.ReadSingle(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public DateTime GetDate(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        ifxTrace?.ApiExit();
        return (DateTime)internalGetDate(i);
    }

    private object internalGetDate(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.TYPE_DATE))
            {
                _dataCache[i] = Buffer.MarshalToManaged(0, Informix32.SQL_C.TYPE_DATE, -1);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override DateTime GetDateTime(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (ifxType != InformixType.DateTime && ifxType != InformixType.Date)
        {
            throw new InvalidCastException();
        }
        DateTime result = _dataCache.AccessIndex(i) == null ? (DateTime)(InformixDateTime)internalGetIfxDateTime(i) : !(typeof(InformixDateTime) == _dataCache[i].GetType()) ? (DateTime)_dataCache[i] : (DateTime)(InformixDateTime)_dataCache[i];
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetDateTime(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.TYPE_TIMESTAMP))
            {
                _dataCache[i] = Buffer.MarshalToManaged(0, Informix32.SQL_C.TYPE_TIMESTAMP, -1);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override decimal GetDecimal(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if (ifxType != InformixType.Decimal && ifxType != InformixType.Money)
        {
            throw new InvalidCastException();
        }
        decimal num = default;
        object obj = internalGetDecimal(i);
        Type type = obj.GetType();
        if (typeof(string) == type)
        {
            try
            {
                num = decimal.Parse((string)obj, CultureInfo.InvariantCulture);
            }
            catch (OverflowException ex)
            {
                throw ex;
            }
        }
        else
        {
            num = (decimal)obj;
        }
        ifxTrace?.ApiExit();
        return num;
    }

    private object internalGetDecimal(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.WCHAR))
            {
                string text = null;
                try
                {
                    text = (string)Buffer.MarshalToManaged(0, Informix32.SQL_C.WCHAR, -3);
                    _dataCache[i] = decimal.Parse(text, CultureInfo.InvariantCulture);
                }
                catch
                {
                    _dataCache[i] = text;
                    return null;
                }
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override double GetDouble(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        InformixType ifxType = GetIfxType(i);
        if ((InformixType.Decimal != ifxType || GetScale(i) != DbSchemaInfo.Floating) && InformixType.Float != ifxType)
        {
            throw new InvalidCastException();
        }
        double result = (double)internalGetDouble(i);
        ifxTrace?.ApiExit();
        return result;
    }

    private object internalGetDouble(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.DOUBLE))
            {
                _dataCache[i] = Buffer.ReadDouble(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override Guid GetGuid(int i)
    {
        InformixTrace.GetIfxTrace()?.ApiEntry(i);
        throw new InvalidCastException();
    }

    private object internalGetGuid(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.GUID))
            {
                _dataCache[i] = Buffer.ReadGuid(0);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    public override string GetString(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        ifxTrace?.ApiExit();
        return (string)internalGetString(i);
    }

    private object internalGetString(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null)
            {
                CNativeBuffer buffer = Buffer;
                int num = buffer.Length - 4;
                if (GetData(i, Informix32.SQL_C.WCHAR, buffer.Length - 2, out var cbLengthOrIndicator))
                {
                    if (cbLengthOrIndicator <= num && -4 != cbLengthOrIndicator)
                    {
                        string text = buffer.PtrToStringUni(0, Math.Min(cbLengthOrIndicator, num) / 2);
                        _dataCache[i] = text;
                        return text;
                    }
                    char[] array = new char[num / 2];
                    int num2 = cbLengthOrIndicator == -4 ? num : cbLengthOrIndicator;
                    StringBuilder stringBuilder = new StringBuilder(num2 / 2);
                    int num3 = num;
                    int num4 = -4 == cbLengthOrIndicator ? -1 : cbLengthOrIndicator - num3;
                    bool data;
                    do
                    {
                        int num5 = num3 / 2;
                        buffer.ReadChars(0, array, 0, num5);
                        stringBuilder.Append(array, 0, num5);
                        if (num4 == 0)
                        {
                            break;
                        }
                        data = GetData(i, Informix32.SQL_C.WCHAR, buffer.Length - 2, out cbLengthOrIndicator);
                        if (-4 != cbLengthOrIndicator)
                        {
                            num3 = Math.Min(cbLengthOrIndicator, num);
                            num4 = 0 < num4 ? num4 - num3 : 0;
                        }
                    }
                    while (data);
                    _dataCache[i] = stringBuilder.ToString();
                }
            }
            if (_dataCache[i] == DBNull.Value)
            {
                return _dataCache[i];
            }
            return _dataCache[i].ToString();
        }
        throw ADP.DataReaderNoData();
    }

    public TimeSpan GetTime(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(i);
        ifxTrace?.ApiExit();
        return (TimeSpan)internalGetTime(i);
    }

    private object internalGetTime(int i)
    {
        if (_isRead)
        {
            if (_dataCache.AccessIndex(i) == null && GetData(i, Informix32.SQL_C.TYPE_TIME))
            {
                _dataCache[i] = Buffer.MarshalToManaged(0, Informix32.SQL_C.TYPE_TIME, -1);
            }
            return _dataCache[i];
        }
        throw ADP.DataReaderNoData();
    }

    private void SetCurrentRowColumnInfo(int row, int column)
    {
        if (_row != row || _column != column)
        {
            _row = row;
            _column = column;
            _sequentialBytesRead = 0L;
        }
    }

    public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return GetBytesOrChars(i, dataIndex, buffer, isCharsBuffer: false, bufferIndex, length);
    }

    public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return GetBytesOrChars(i, dataIndex, buffer, isCharsBuffer: true, bufferIndex, length);
    }

    private long GetBytesOrChars(int i, long dataIndex, Array buffer, bool isCharsBuffer, int bufferIndex, int length)
    {
        if (IsClosed)
        {
            throw ADP.DataReaderNoData();
        }
        if (!_isRead)
        {
            throw ADP.DataReaderNoData();
        }
        if (dataIndex < 0)
        {
            throw ADP.ArgumentOutOfRange("dataIndex");
        }
        if (bufferIndex < 0)
        {
            throw ADP.ArgumentOutOfRange("bufferIndex");
        }
        if (length < 0)
        {
            throw ADP.ArgumentOutOfRange("length");
        }
        string method = isCharsBuffer ? "GetChars" : "GetBytes";
        SetCurrentRowColumnInfo(_row, i);
        object obj = null;
        if (!IsCommandBehavior(CommandBehavior.SequentialAccess))
        {
            if (int.MaxValue < dataIndex)
            {
                throw ADP.ArgumentOutOfRange("dataIndex");
            }
            if (obj == null)
            {
                obj = !isCharsBuffer ? (byte[])internalGetBytes(i) : (string)internalGetString(i);
            }
            int num = isCharsBuffer ? ((string)obj).Length : ((byte[])obj).Length;
            if (buffer == null)
            {
                return num;
            }
            if (length == 0)
            {
                return 0L;
            }
            if (dataIndex >= num)
            {
                return 0L;
            }
            int val = num - (int)dataIndex;
            int val2 = Math.Min(val, length);
            val2 = Math.Min(val2, buffer.Length - bufferIndex);
            if (val2 <= 0)
            {
                return 0L;
            }
            if (isCharsBuffer)
            {
                ((string)obj).CopyTo((int)dataIndex, (char[])buffer, bufferIndex, val2);
            }
            else
            {
                Array.Copy((byte[])obj, (int)dataIndex, (byte[])buffer, bufferIndex, val2);
            }
            return val2;
        }
        if (buffer == null)
        {
            Informix32.SQL_C sqlctype = isCharsBuffer ? Informix32.SQL_C.WCHAR : Informix32.SQL_C.BINARY;
            if (!QueryFieldInfo(i, sqlctype, out var cbLengthOrIndicator))
            {
                if (isCharsBuffer)
                {
                    throw ADP.InvalidCast();
                }
                return -1L;
            }
            if (isCharsBuffer)
            {
                return cbLengthOrIndicator / 2;
            }
            return cbLengthOrIndicator;
        }
        if (isCharsBuffer && dataIndex < _sequentialBytesRead / 2 || !isCharsBuffer && dataIndex < _sequentialBytesRead)
        {
            throw ADP.NonSeqByteAccess(dataIndex, _sequentialBytesRead, method);
        }
        dataIndex = !isCharsBuffer ? dataIndex - _sequentialBytesRead : dataIndex - _sequentialBytesRead / 2;
        if (dataIndex > 0)
        {
            int num2 = readBytesOrCharsSequentialAccess(i, null, isCharsBuffer, 0, dataIndex);
            if (num2 < dataIndex)
            {
                return 0L;
            }
        }
        length = Math.Min(length, buffer.Length - bufferIndex);
        if (length <= 0)
        {
            if (isCharsBuffer && !QueryFieldInfo(i, Informix32.SQL_C.WCHAR, out var _))
            {
                throw ADP.InvalidCast();
            }
            return 0L;
        }
        return readBytesOrCharsSequentialAccess(i, buffer, isCharsBuffer, bufferIndex, length);
    }

    private int readBytesOrCharsSequentialAccess(int i, Array buffer, bool isCharsBuffer, int bufferIndex, long bytesOrCharsLength)
    {
        int num = 0;
        string text = isCharsBuffer ? "GetChars" : "GetBytes";
        long num2 = isCharsBuffer ? checked(bytesOrCharsLength * 2) : bytesOrCharsLength;
        CNativeBuffer buffer2 = Buffer;
        while (num2 > 0)
        {
            int num3;
            bool data;
            int cbLengthOrIndicator;
            if (isCharsBuffer)
            {
                num3 = (int)Math.Min(num2, buffer2.Length - 4);
                data = GetData(i, Informix32.SQL_C.WCHAR, num3 + 2, out cbLengthOrIndicator);
            }
            else
            {
                num3 = (int)Math.Min(num2, buffer2.Length - 2);
                data = GetData(i, Informix32.SQL_C.BINARY, num3, out cbLengthOrIndicator);
            }
            if (!data)
            {
                throw ADP.InvalidCast();
            }
            bool flag = false;
            if (cbLengthOrIndicator == 0)
            {
                break;
            }
            int num4;
            if (-4 == cbLengthOrIndicator)
            {
                num4 = num3;
            }
            else if (cbLengthOrIndicator > num3)
            {
                num4 = num3;
            }
            else
            {
                num4 = cbLengthOrIndicator;
                flag = true;
            }
            _sequentialBytesRead += num4;
            if (isCharsBuffer)
            {
                int num5 = num4 / 2;
                if (buffer != null)
                {
                    buffer2.ReadChars(0, (char[])buffer, bufferIndex, num5);
                    bufferIndex += num5;
                }
                num += num5;
            }
            else
            {
                if (buffer != null)
                {
                    buffer2.ReadBytes(0, (byte[])buffer, bufferIndex, num4);
                    bufferIndex += num4;
                }
                num += num4;
            }
            num2 -= num4;
            if (flag)
            {
                break;
            }
        }
        return num;
    }

    private object internalGetBytes(int i)
    {
        if (_dataCache.AccessIndex(i) == null)
        {
            int num = Buffer.Length - 4;
            int num2 = 0;
            if (GetData(i, Informix32.SQL_C.BINARY, num, out var cbLengthOrIndicator))
            {
                CNativeBuffer buffer = Buffer;
                byte[] array;
                if (-4 != cbLengthOrIndicator)
                {
                    array = new byte[cbLengthOrIndicator];
                    Buffer.ReadBytes(0, array, num2, Math.Min(cbLengthOrIndicator, num));
                    while (cbLengthOrIndicator > num)
                    {
                        bool data = GetData(i, Informix32.SQL_C.BINARY, num, out cbLengthOrIndicator);
                        num2 += num;
                        buffer.ReadBytes(0, array, num2, Math.Min(cbLengthOrIndicator, num));
                    }
                }
                else
                {
                    List<byte[]> list = new List<byte[]>();
                    int num3 = 0;
                    do
                    {
                        int num4 = -4 != cbLengthOrIndicator ? cbLengthOrIndicator : num;
                        array = new byte[num4];
                        num3 += num4;
                        buffer.ReadBytes(0, array, 0, num4);
                        list.Add(array);
                    }
                    while (-4 == cbLengthOrIndicator && GetData(i, Informix32.SQL_C.BINARY, num, out cbLengthOrIndicator));
                    array = new byte[num3];
                    foreach (byte[] item in list)
                    {
                        item.CopyTo(array, num2);
                        num2 += item.Length;
                    }
                }
                _dataCache[i] = array;
            }
        }
        return _dataCache[i];
    }

    private SQLLEN GetColAttribute(int iColumn, Informix32.SQL_DESC v3FieldId, Informix32.SQL_COLUMN v2FieldId, Informix32.HANDLER handler)
    {
        short stringLength = 0;
        if (Connection == null || _cmdWrapper.Canceling)
        {
            return -1;
        }
        OdbcStatementHandle statementHandle = StatementHandle;
        Informix32.RetCode retCode;
        SQLLEN numericAttribute;
        if (Connection.IsV3Driver)
        {
            retCode = statementHandle.ColumnAttribute(iColumn + 1, (short)v3FieldId, Buffer, out stringLength, out numericAttribute);
        }
        else
        {
            if (v2FieldId == (Informix32.SQL_COLUMN)(-1))
            {
                return 0;
            }
            retCode = statementHandle.ColumnAttribute(iColumn + 1, (short)v2FieldId, Buffer, out stringLength, out numericAttribute);
        }
        if (retCode != 0)
        {
            if (retCode == Informix32.RetCode.ERROR && "HY091" == Command.GetDiagSqlState())
            {
                Connection.FlagUnsupportedColAttr(v3FieldId, v2FieldId);
            }
            if (handler == Informix32.HANDLER.THROW)
            {
                Connection.HandleError(statementHandle, retCode);
            }
            return -1;
        }
        return numericAttribute;
    }

    private string GetColAttributeStr(int i, Informix32.SQL_DESC v3FieldId, Informix32.SQL_COLUMN v2FieldId, Informix32.HANDLER handler)
    {
        short stringLength = 0;
        CNativeBuffer buffer = Buffer;
        buffer.WriteInt16(0, 0);
        OdbcStatementHandle statementHandle = StatementHandle;
        if (Connection == null || _cmdWrapper.Canceling || statementHandle == null)
        {
            return "";
        }
        Informix32.RetCode retCode;
        SQLLEN numericAttribute;
        if (Connection.IsV3Driver)
        {
            retCode = statementHandle.ColumnAttribute(i + 1, (short)v3FieldId, buffer, out stringLength, out numericAttribute);
        }
        else
        {
            if (v2FieldId == (Informix32.SQL_COLUMN)(-1))
            {
                return null;
            }
            retCode = statementHandle.ColumnAttribute(i + 1, (short)v2FieldId, buffer, out stringLength, out numericAttribute);
        }
        if (retCode != 0 || stringLength == 0)
        {
            if (retCode == Informix32.RetCode.ERROR && "HY091" == Command.GetDiagSqlState())
            {
                Connection.FlagUnsupportedColAttr(v3FieldId, v2FieldId);
            }
            if (handler == Informix32.HANDLER.THROW)
            {
                Connection.HandleError(statementHandle, retCode);
            }
            return null;
        }
        return buffer.PtrToStringUni(0, stringLength / 2);
    }

    private string GetDescFieldStr(int i, Informix32.SQL_DESC attribute, Informix32.HANDLER handler)
    {
        int numericAttribute = 0;
        if (Connection == null || _cmdWrapper.Canceling)
        {
            return "";
        }
        if (!Connection.IsV3Driver)
        {
            return null;
        }
        CNativeBuffer buffer = Buffer;
        using (OdbcDescriptorHandle odbcDescriptorHandle = new OdbcDescriptorHandle(StatementHandle, Informix32.SQL_ATTR.APP_PARAM_DESC))
        {
            Informix32.RetCode descriptionField = odbcDescriptorHandle.GetDescriptionField(i + 1, attribute, buffer, out numericAttribute);
            if (descriptionField != 0 || numericAttribute == 0)
            {
                if (descriptionField == Informix32.RetCode.ERROR && "HY091" == Command.GetDiagSqlState())
                {
                    Connection.FlagUnsupportedColAttr(attribute, Informix32.SQL_COLUMN.COUNT);
                }
                if (handler == Informix32.HANDLER.THROW)
                {
                    Connection.HandleError(StatementHandle, descriptionField);
                }
                return null;
            }
        }
        return buffer.PtrToStringUni(0, numericAttribute / 2);
    }

    private bool QueryFieldInfo(int i, Informix32.SQL_C sqlctype, out int cbLengthOrIndicator)
    {
        int cb = 0;
        if (sqlctype == Informix32.SQL_C.WCHAR)
        {
            cb = 2;
        }
        return GetData(i, sqlctype, cb, out cbLengthOrIndicator);
    }

    private bool GetData(int i, Informix32.SQL_C sqlctype)
    {
        int cbLengthOrIndicator;
        return GetData(i, sqlctype, Buffer.Length - 4, out cbLengthOrIndicator);
    }

    private bool GetData(int i, Informix32.SQL_C sqlctype, int cb, out int cbLengthOrIndicator)
    {
        nint cbActual = nint.Zero;
        if (IsCancelingCommand)
        {
            throw ADP.DataReaderNoData();
        }
        CNativeBuffer buffer = Buffer;
        Informix32.RetCode data = StatementHandle.GetData(i + 1, sqlctype, buffer, cb, out cbActual);
        switch (data)
        {
            case Informix32.RetCode.SUCCESS_WITH_INFO:
                if ((int)cbActual != -4)
                {
                }
                break;
            case Informix32.RetCode.NO_DATA:
                if (sqlctype != Informix32.SQL_C.WCHAR && sqlctype != Informix32.SQL_C.BINARY)
                {
                    Connection.HandleError(StatementHandle, data);
                }
                if (cbActual == -4)
                {
                    cbActual = 0;
                }
                break;
            default:
                Connection.HandleError(StatementHandle, data);
                break;
            case Informix32.RetCode.SUCCESS:
                break;
        }
        SetCurrentRowColumnInfo(_row, i);
        if (cbActual == -1)
        {
            _dataCache[i] = DBNull.Value;
            cbLengthOrIndicator = 0;
            _validResult = false;
            return false;
        }
        cbLengthOrIndicator = (int)cbActual;
        _validResult = true;
        return true;
    }

    public override bool Read()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsClosed)
        {
            throw ADP.DataReaderClosed("Read");
        }
        if (IsCancelingCommand)
        {
            _isRead = false;
            ifxTrace?.ApiExit();
            return false;
        }
        if (_skipReadOnce)
        {
            _skipReadOnce = false;
            ifxTrace?.ApiExit();
            return _isRead;
        }
        if (_noMoreRows || _noMoreResults || IsCommandBehavior(CommandBehavior.SchemaOnly))
        {
            ifxTrace?.ApiExit();
            return false;
        }
        if (!_isValidResult)
        {
            ifxTrace?.ApiExit();
            return false;
        }
        Informix32.RetCode retCode = StatementHandle.Fetch();
        switch (retCode)
        {
            case Informix32.RetCode.SUCCESS_WITH_INFO:
                Connection.HandleErrorNoThrow(StatementHandle, retCode);
                _hasRows = HasRowsStatus.HasRows;
                _isRead = true;
                break;
            case Informix32.RetCode.SUCCESS:
                _hasRows = HasRowsStatus.HasRows;
                _isRead = true;
                break;
            case Informix32.RetCode.NO_DATA:
                _isRead = false;
                if (_hasRows == HasRowsStatus.DontKnow)
                {
                    _hasRows = HasRowsStatus.HasNoRows;
                }
                break;
            default:
                Connection.HandleError(StatementHandle, retCode);
                break;
        }
        _dataCache.FlushValues();
        if (IsCommandBehavior(CommandBehavior.SingleRow))
        {
            _noMoreRows = true;
            SetCurrentRowColumnInfo(-1, 0);
        }
        else
        {
            SetCurrentRowColumnInfo(_row + 1, 0);
        }
        ifxTrace?.ApiExit();
        return _isRead;
    }

    internal void FirstResult()
    {
        SQLLEN rowCount = GetRowCount();
        CalculateRecordsAffected(rowCount);
        if (FieldCountNoThrow(out var cColsAffected) == Informix32.RetCode.SUCCESS && cColsAffected == 0)
        {
            NextResult();
        }
        else
        {
            _isValidResult = true;
        }
    }

    public override bool NextResult()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return NextResult(disposing: false, allresults: false);
    }

    private bool NextResult(bool disposing, bool allresults)
    {
        Informix32.RetCode retcode = Informix32.RetCode.SUCCESS;
        bool flag = false;
        bool flag2 = IsCommandBehavior(CommandBehavior.SingleResult);
        if (IsClosed)
        {
            throw ADP.DataReaderClosed("NextResult");
        }
        _fieldNameLookup = null;
        if (IsCancelingCommand || _noMoreResults)
        {
            return false;
        }
        _isRead = false;
        _hasRows = HasRowsStatus.DontKnow;
        _fieldNameLookup = null;
        _metadata = null;
        _schemaTable = null;
        int num = 0;
        InformixErrorCollection ifxErrorCollection = null;
        Informix32.RetCode retCode;
        bool flag3;
        do
        {
            _isValidResult = false;
            retCode = StatementHandle.MoreResults();
            flag3 = retCode == Informix32.RetCode.SUCCESS || retCode == Informix32.RetCode.SUCCESS_WITH_INFO;
            if (retCode == Informix32.RetCode.SUCCESS_WITH_INFO)
            {
                Connection.HandleErrorNoThrow(StatementHandle, retCode);
            }
            else if (!disposing && retCode != Informix32.RetCode.NO_DATA && retCode != 0)
            {
                if (ifxErrorCollection == null)
                {
                    retcode = retCode;
                    ifxErrorCollection = new InformixErrorCollection();
                }
                Informix32.GetDiagErrors(ifxErrorCollection, null, StatementHandle, retCode);
                num++;
            }
            if (!disposing && flag3)
            {
                num = 0;
                SQLLEN rowCount = GetRowCount();
                CalculateRecordsAffected(rowCount);
                if (!flag2)
                {
                    FieldCountNoThrow(out var cColsAffected);
                    flag = _isValidResult = cColsAffected != 0;
                }
            }
        }
        while (!flag2 && flag3 && !flag || Informix32.RetCode.NO_DATA != retCode && allresults && num < 2000 || flag2 && flag3);
        if (retCode == Informix32.RetCode.NO_DATA)
        {
            _dataCache = null;
            _noMoreResults = true;
        }
        if (ifxErrorCollection != null)
        {
            ifxErrorCollection.SetSource(Connection.Driver);
            InformixException ex = InformixException.CreateException(ifxErrorCollection, retcode);
            Connection.ConnectionIsAlive();
            throw ex;
        }
        return flag3;
    }

    private void BuildMetaDataInfo()
    {
        int fieldCount = FieldCount;
        MetaData[] array = new MetaData[fieldCount];
        bool flag = IsCommandBehavior(CommandBehavior.KeyInfo);
        List<string> list = !flag ? null : new List<string>();
        for (int i = 0; i < fieldCount; i++)
        {
            array[i] = new MetaData();
            array[i].ordinal = i;
            InformixType ifxType = GetIfxType(i);
            TypeMap typeMap = TypeMap.FromOdbcType(ifxType);
            if (typeMap._signType)
            {
                bool unsigned = GetColAttribute(i, Informix32.SQL_DESC.UNSIGNED, Informix32.SQL_COLUMN.UNSIGNED, Informix32.HANDLER.THROW).ToInt64() != 0;
                typeMap = TypeMap.UpgradeSignedType(typeMap, unsigned);
            }
            array[i].typemap = typeMap;
            array[i].size = GetColAttribute(i, Informix32.SQL_DESC.OCTET_LENGTH, Informix32.SQL_COLUMN.LENGTH, Informix32.HANDLER.IGNORE);
            Informix32.SQL_TYPE sql_type = array[i].typemap._sql_type;
            if ((uint)(sql_type - -10) <= 2u)
            {
                MetaData obj = array[i];
                obj.size = (int)obj.size / 2;
            }
            array[i].precision = (byte)(int)GetColAttribute(i, (Informix32.SQL_DESC)4, Informix32.SQL_COLUMN.PRECISION, Informix32.HANDLER.IGNORE);
            array[i].scale = (byte)(int)GetColAttribute(i, (Informix32.SQL_DESC)5, Informix32.SQL_COLUMN.SCALE, Informix32.HANDLER.IGNORE);
            array[i].isAutoIncrement = (int)GetColAttribute(i, Informix32.SQL_DESC.AUTO_UNIQUE_VALUE, Informix32.SQL_COLUMN.AUTO_INCREMENT, Informix32.HANDLER.IGNORE) == 1;
            array[i].isReadOnly = (int)GetColAttribute(i, Informix32.SQL_DESC.UPDATABLE, Informix32.SQL_COLUMN.UPDATABLE, Informix32.HANDLER.IGNORE) == 0;
            Informix32.SQL_NULLABILITY sQL_NULLABILITY = (Informix32.SQL_NULLABILITY)(int)GetColAttribute(i, Informix32.SQL_DESC.NULLABLE, Informix32.SQL_COLUMN.NULLABLE, Informix32.HANDLER.IGNORE);
            array[i].isNullable = sQL_NULLABILITY == Informix32.SQL_NULLABILITY.NULLABLE;
            Informix32.SQL_TYPE sql_type2 = array[i].typemap._sql_type;
            if (sql_type2 == Informix32.SQL_TYPE.WLONGVARCHAR || sql_type2 == Informix32.SQL_TYPE.LONGVARBINARY || sql_type2 == Informix32.SQL_TYPE.LONGVARCHAR)
            {
                array[i].isLong = true;
            }
            else
            {
                array[i].isLong = false;
            }
            if (IsCommandBehavior(CommandBehavior.KeyInfo))
            {
                if (!Connection.ProviderInfo.NoSqlCASSColumnKey)
                {
                    bool flag2 = (int)GetColAttribute(i, (Informix32.SQL_DESC)1212, (Informix32.SQL_COLUMN)(-1), Informix32.HANDLER.IGNORE) == 1;
                    if (flag2)
                    {
                        array[i].isKeyColumn = flag2;
                        array[i].isUnique = true;
                        flag = false;
                    }
                }
                array[i].baseSchemaName = GetColAttributeStr(i, Informix32.SQL_DESC.SCHEMA_NAME, Informix32.SQL_COLUMN.OWNER_NAME, Informix32.HANDLER.IGNORE);
                array[i].baseCatalogName = GetColAttributeStr(i, Informix32.SQL_DESC.CATALOG_NAME, (Informix32.SQL_COLUMN)(-1), Informix32.HANDLER.IGNORE);
                array[i].baseTableName = GetColAttributeStr(i, Informix32.SQL_DESC.BASE_TABLE_NAME, Informix32.SQL_COLUMN.TABLE_NAME, Informix32.HANDLER.IGNORE);
                array[i].baseColumnName = GetColAttributeStr(i, Informix32.SQL_DESC.BASE_COLUMN_NAME, Informix32.SQL_COLUMN.NAME, Informix32.HANDLER.IGNORE);
                if (Connection.IsV3Driver)
                {
                    if (array[i].baseTableName == null || array[i].baseTableName.Length == 0)
                    {
                        array[i].baseTableName = GetDescFieldStr(i, Informix32.SQL_DESC.BASE_TABLE_NAME, Informix32.HANDLER.IGNORE);
                    }
                    if (array[i].baseColumnName == null || array[i].baseColumnName.Length == 0)
                    {
                        array[i].baseColumnName = GetDescFieldStr(i, Informix32.SQL_DESC.BASE_COLUMN_NAME, Informix32.HANDLER.IGNORE);
                    }
                }
                if (array[i].baseTableName != null && !list.Contains(array[i].baseTableName))
                {
                    list.Add(array[i].baseTableName);
                }
            }
            if ((array[i].isKeyColumn || array[i].isAutoIncrement) && sQL_NULLABILITY == Informix32.SQL_NULLABILITY.UNKNOWN)
            {
                array[i].isNullable = false;
            }
        }
        if (!Connection.ProviderInfo.NoSqlCASSColumnKey)
        {
            for (int j = fieldCount; j < fieldCount + _hiddenColumns; j++)
            {
                if ((int)GetColAttribute(j, (Informix32.SQL_DESC)1212, (Informix32.SQL_COLUMN)(-1), Informix32.HANDLER.IGNORE) == 1 && (int)GetColAttribute(j, (Informix32.SQL_DESC)1211, (Informix32.SQL_COLUMN)(-1), Informix32.HANDLER.IGNORE) == 1)
                {
                    for (int k = 0; k < fieldCount; k++)
                    {
                        array[k].isKeyColumn = false;
                        array[k].isUnique = false;
                    }
                }
            }
        }
        _metadata = array;
        if (!IsCommandBehavior(CommandBehavior.KeyInfo))
        {
            return;
        }
        if (list != null && list.Count > 0)
        {
            List<string>.Enumerator enumerator = list.GetEnumerator();
            QualifiedTableName qualifiedTableName = new QualifiedTableName(Connection.QuoteChar("GetSchemaTable"));
            while (enumerator.MoveNext())
            {
                qualifiedTableName.Table = enumerator.Current;
                if (RetrieveKeyInfo(flag, qualifiedTableName, quoted: false) <= 0)
                {
                    RetrieveKeyInfo(flag, qualifiedTableName, quoted: true);
                }
            }
            return;
        }
        QualifiedTableName qualifiedTableName2 = new QualifiedTableName(Connection.QuoteChar("GetSchemaTable"), GetTableNameFromCommandText());
        if (!string.IsNullOrEmpty(qualifiedTableName2.Table))
        {
            SetBaseTableNames(qualifiedTableName2);
            if (RetrieveKeyInfo(flag, qualifiedTableName2, quoted: false) <= 0)
            {
                RetrieveKeyInfo(flag, qualifiedTableName2, quoted: true);
            }
        }
    }

    private DataTable NewSchemaTable()
    {
        DataTable dataTable = new DataTable("SchemaTable");
        dataTable.Locale = CultureInfo.InvariantCulture;
        dataTable.MinimumCapacity = FieldCount;
        DataColumnCollection columns = dataTable.Columns;
        columns.Add(new DataColumn("ColumnName", typeof(string)));
        columns.Add(new DataColumn("ColumnOrdinal", typeof(int)));
        columns.Add(new DataColumn("ColumnSize", typeof(int)));
        columns.Add(new DataColumn("NumericPrecision", typeof(short)));
        columns.Add(new DataColumn("NumericScale", typeof(short)));
        columns.Add(new DataColumn("DataType", typeof(object)));
        columns.Add(new DataColumn("ProviderType", typeof(int)));
        columns.Add(new DataColumn("IsLong", typeof(bool)));
        columns.Add(new DataColumn("AllowDBNull", typeof(bool)));
        columns.Add(new DataColumn("IsReadOnly", typeof(bool)));
        columns.Add(new DataColumn("IsRowVersion", typeof(bool)));
        columns.Add(new DataColumn("IsUnique", typeof(bool)));
        columns.Add(new DataColumn("IsKey", typeof(bool)));
        columns.Add(new DataColumn("IsAutoIncrement", typeof(bool)));
        columns.Add(new DataColumn("BaseSchemaName", typeof(string)));
        columns.Add(new DataColumn("BaseCatalogName", typeof(string)));
        columns.Add(new DataColumn("BaseTableName", typeof(string)));
        columns.Add(new DataColumn("BaseColumnName", typeof(string)));
        foreach (DataColumn item in columns)
        {
            item.ReadOnly = true;
        }
        return dataTable;
    }

    public override DataTable GetSchemaTable()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (IsClosed)
        {
            throw ADP.DataReaderClosed("GetSchemaTable");
        }
        if (_noMoreResults)
        {
            ifxTrace?.ApiExit();
            return null;
        }
        if (_schemaTable != null)
        {
            ifxTrace?.ApiExit();
            return _schemaTable;
        }
        DataTable dataTable = NewSchemaTable();
        if (FieldCount == 0)
        {
            ifxTrace?.ApiExit();
            return dataTable;
        }
        if (_metadata == null)
        {
            BuildMetaDataInfo();
        }
        DataColumn column = dataTable.Columns["ColumnName"];
        DataColumn column2 = dataTable.Columns["ColumnOrdinal"];
        DataColumn column3 = dataTable.Columns["ColumnSize"];
        DataColumn column4 = dataTable.Columns["NumericPrecision"];
        DataColumn column5 = dataTable.Columns["NumericScale"];
        DataColumn column6 = dataTable.Columns["DataType"];
        DataColumn column7 = dataTable.Columns["ProviderType"];
        DataColumn column8 = dataTable.Columns["IsLong"];
        DataColumn column9 = dataTable.Columns["AllowDBNull"];
        DataColumn column10 = dataTable.Columns["IsReadOnly"];
        DataColumn column11 = dataTable.Columns["IsRowVersion"];
        DataColumn column12 = dataTable.Columns["IsUnique"];
        DataColumn column13 = dataTable.Columns["IsKey"];
        DataColumn column14 = dataTable.Columns["IsAutoIncrement"];
        DataColumn column15 = dataTable.Columns["BaseSchemaName"];
        DataColumn column16 = dataTable.Columns["BaseCatalogName"];
        DataColumn column17 = dataTable.Columns["BaseTableName"];
        DataColumn column18 = dataTable.Columns["BaseColumnName"];
        int fieldCount = FieldCount;
        for (int i = 0; i < fieldCount; i++)
        {
            DataRow dataRow = dataTable.NewRow();
            dataRow[column] = GetName(i);
            dataRow[column2] = i;
            dataRow[column3] = (int)Math.Min(Math.Max(-2147483648L, _metadata[i].size.ToInt64()), 2147483647L);
            dataRow[column4] = (short)_metadata[i].precision;
            dataRow[column5] = (short)_metadata[i].scale;
            dataRow[column6] = _metadata[i].typemap._type;
            dataRow[column7] = _metadata[i].typemap._odbcType;
            dataRow[column8] = _metadata[i].isLong;
            dataRow[column9] = _metadata[i].isNullable;
            dataRow[column10] = _metadata[i].isReadOnly;
            dataRow[column11] = _metadata[i].isRowVersion;
            dataRow[column12] = _metadata[i].isUnique;
            dataRow[column13] = _metadata[i].isKeyColumn;
            dataRow[column14] = _metadata[i].isAutoIncrement;
            dataRow[column15] = _metadata[i].baseSchemaName;
            dataRow[column16] = _metadata[i].baseCatalogName;
            dataRow[column17] = _metadata[i].baseTableName;
            dataRow[column18] = _metadata[i].baseColumnName;
            dataTable.Rows.Add(dataRow);
            dataRow.AcceptChanges();
        }
        _schemaTable = dataTable;
        ifxTrace?.ApiExit();
        return dataTable;
    }

    internal int RetrieveKeyInfo(bool needkeyinfo, QualifiedTableName qualifiedTableName, bool quoted)
    {
        int num = 0;
        nint zero = nint.Zero;
        if (IsClosed || _cmdWrapper == null)
        {
            return 0;
        }
        _cmdWrapper.CreateKeyInfoStatementHandle();
        CNativeBuffer buffer = Buffer;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            buffer.DangerousAddRef(ref success);
            Informix32.RetCode retCode;
            if (needkeyinfo)
            {
                if (!Connection.ProviderInfo.NoSqlPrimaryKeys)
                {
                    retCode = KeyInfoStatementHandle.PrimaryKeys(qualifiedTableName.Catalog, qualifiedTableName.Schema, qualifiedTableName.GetTable(quoted));
                    if (retCode == Informix32.RetCode.SUCCESS || retCode == Informix32.RetCode.SUCCESS_WITH_INFO)
                    {
                        bool flag = false;
                        buffer.WriteInt16(0, 0);
                        retCode = KeyInfoStatementHandle.BindColumn2(4, Informix32.SQL_C.WCHAR, buffer.PtrOffset(0, 256), 256, buffer.PtrOffset(256, nint.Size).Handle);
                        while ((retCode = KeyInfoStatementHandle.Fetch()) == Informix32.RetCode.SUCCESS)
                        {
                            zero = buffer.ReadIntPtr(256);
                            string text = buffer.PtrToStringUni(0, (int)zero / 2);
                            int ordinalFromBaseColName = GetOrdinalFromBaseColName(text);
                            if (ordinalFromBaseColName != -1)
                            {
                                num++;
                                _metadata[ordinalFromBaseColName].isKeyColumn = true;
                                _metadata[ordinalFromBaseColName].isUnique = true;
                                _metadata[ordinalFromBaseColName].isNullable = false;
                                _metadata[ordinalFromBaseColName].baseTableName = qualifiedTableName.Table;
                                if (_metadata[ordinalFromBaseColName].baseColumnName == null)
                                {
                                    _metadata[ordinalFromBaseColName].baseColumnName = text;
                                }
                                continue;
                            }
                            flag = true;
                            break;
                        }
                        if (flag)
                        {
                            MetaData[] metadata = _metadata;
                            foreach (MetaData metaData in metadata)
                            {
                                metaData.isKeyColumn = false;
                            }
                        }
                        retCode = KeyInfoStatementHandle.BindColumn3(4, Informix32.SQL_C.WCHAR, buffer.DangerousGetHandle());
                    }
                    else if ("IM001" == Command.GetDiagSqlState())
                    {
                        Connection.ProviderInfo.NoSqlPrimaryKeys = true;
                    }
                }
                if (num == 0)
                {
                    KeyInfoStatementHandle.MoreResults();
                    num += RetrieveKeyInfoFromStatistics(qualifiedTableName, quoted);
                }
                KeyInfoStatementHandle.MoreResults();
            }
            retCode = KeyInfoStatementHandle.SpecialColumns(qualifiedTableName.GetTable(quoted));
            if (retCode == Informix32.RetCode.SUCCESS || retCode == Informix32.RetCode.SUCCESS_WITH_INFO)
            {
                zero = nint.Zero;
                buffer.WriteInt16(0, 0);
                retCode = KeyInfoStatementHandle.BindColumn2(2, Informix32.SQL_C.WCHAR, buffer.PtrOffset(0, 256), 256, buffer.PtrOffset(256, nint.Size).Handle);
                while ((retCode = KeyInfoStatementHandle.Fetch()) == Informix32.RetCode.SUCCESS)
                {
                    zero = buffer.ReadIntPtr(256);
                    string text = buffer.PtrToStringUni(0, (int)zero / 2);
                    int ordinalFromBaseColName = GetOrdinalFromBaseColName(text);
                    if (ordinalFromBaseColName != -1)
                    {
                        _metadata[ordinalFromBaseColName].isRowVersion = true;
                        if (_metadata[ordinalFromBaseColName].baseColumnName == null)
                        {
                            _metadata[ordinalFromBaseColName].baseColumnName = text;
                        }
                    }
                }
                retCode = KeyInfoStatementHandle.BindColumn3(2, Informix32.SQL_C.WCHAR, buffer.DangerousGetHandle());
                retCode = KeyInfoStatementHandle.MoreResults();
            }
        }
        finally
        {
            if (success)
            {
                buffer.DangerousRelease();
            }
        }
        return num;
    }

    private int RetrieveKeyInfoFromStatistics(QualifiedTableName qualifiedTableName, bool quoted)
    {
        string text = string.Empty;
        string empty = string.Empty;
        string currentindexname = string.Empty;
        int[] array = new int[16];
        int[] array2 = new int[16];
        int num = 0;
        int num2 = 0;
        bool flag = false;
        nint zero = nint.Zero;
        nint zero2 = nint.Zero;
        int num3 = 0;
        string tableName = new string(qualifiedTableName.GetTable(quoted));
        if (KeyInfoStatementHandle.Statistics(tableName) != 0)
        {
            return 0;
        }
        CNativeBuffer buffer = Buffer;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            buffer.DangerousAddRef(ref success);
            HandleRef buffer2 = buffer.PtrOffset(0, 256);
            HandleRef buffer3 = buffer.PtrOffset(256, 256);
            HandleRef buffer4 = buffer.PtrOffset(512, 4);
            nint handle = buffer.PtrOffset(520, nint.Size).Handle;
            nint handle2 = buffer.PtrOffset(528, nint.Size).Handle;
            nint handle3 = buffer.PtrOffset(536, nint.Size).Handle;
            buffer.WriteInt16(256, 0);
            Informix32.RetCode retCode = KeyInfoStatementHandle.BindColumn2(6, Informix32.SQL_C.WCHAR, buffer3, 256, handle2);
            retCode = KeyInfoStatementHandle.BindColumn2(8, Informix32.SQL_C.SSHORT, buffer4, 4, handle3);
            buffer.WriteInt16(512, 0);
            retCode = KeyInfoStatementHandle.BindColumn2(9, Informix32.SQL_C.WCHAR, buffer2, 256, handle);
            while ((retCode = KeyInfoStatementHandle.Fetch()) == Informix32.RetCode.SUCCESS)
            {
                zero2 = buffer.ReadIntPtr(520);
                zero = buffer.ReadIntPtr(528);
                if (buffer.ReadInt16(256) == 0)
                {
                    continue;
                }
                text = buffer.PtrToStringUni(0, (int)zero2 / 2);
                empty = buffer.PtrToStringUni(256, (int)zero / 2);
                int ordinal = buffer.ReadInt16(512);
                if (SameIndexColumn(currentindexname, empty, ordinal, num2))
                {
                    if (!flag)
                    {
                        ordinal = GetOrdinalFromBaseColName(text, qualifiedTableName.Table);
                        if (ordinal == -1)
                        {
                            flag = true;
                        }
                        else if (num2 < 16)
                        {
                            array[num2++] = ordinal;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    continue;
                }
                if (!flag && num2 != 0 && (num == 0 || num > num2))
                {
                    num = num2;
                    for (int i = 0; i < num2; i++)
                    {
                        array2[i] = array[i];
                    }
                }
                num2 = 0;
                currentindexname = empty;
                flag = false;
                ordinal = GetOrdinalFromBaseColName(text, qualifiedTableName.Table);
                if (ordinal == -1)
                {
                    flag = true;
                }
                else
                {
                    array[num2++] = ordinal;
                }
            }
            if (!flag && num2 != 0 && (num == 0 || num > num2))
            {
                num = num2;
                for (int j = 0; j < num2; j++)
                {
                    array2[j] = array[j];
                }
            }
            if (num != 0)
            {
                for (int k = 0; k < num; k++)
                {
                    int num4 = array2[k];
                    num3++;
                    _metadata[num4].isKeyColumn = true;
                    _metadata[num4].isNullable = false;
                    _metadata[num4].isUnique = true;
                    if (_metadata[num4].baseTableName == null)
                    {
                        _metadata[num4].baseTableName = qualifiedTableName.Table;
                    }
                    if (_metadata[num4].baseColumnName == null)
                    {
                        _metadata[num4].baseColumnName = text;
                    }
                }
            }
            _cmdWrapper.FreeKeyInfoStatementHandle(Informix32.STMT.UNBIND);
            return num3;
        }
        finally
        {
            if (success)
            {
                buffer.DangerousRelease();
            }
        }
    }

    internal bool SameIndexColumn(string currentindexname, string indexname, int ordinal, int ncols)
    {
        if (string.IsNullOrEmpty(currentindexname))
        {
            return false;
        }
        if (currentindexname == indexname && ordinal == ncols + 1)
        {
            return true;
        }
        return false;
    }

    internal int GetOrdinalFromBaseColName(string columnname)
    {
        return GetOrdinalFromBaseColName(columnname, null);
    }

    internal int GetOrdinalFromBaseColName(string columnname, string tablename)
    {
        if (string.IsNullOrEmpty(columnname))
        {
            return -1;
        }
        if (_metadata != null)
        {
            int fieldCount = FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                if (_metadata[i].baseColumnName != null && columnname == _metadata[i].baseColumnName)
                {
                    if (string.IsNullOrEmpty(tablename))
                    {
                        return i;
                    }
                    if (tablename == _metadata[i].baseTableName)
                    {
                        return i;
                    }
                }
            }
        }
        return IndexOf(columnname);
    }

    internal string GetTableNameFromCommandText()
    {
        if (_command == null)
        {
            return null;
        }
        string cmdText = _cmdText;
        if (string.IsNullOrEmpty(cmdText))
        {
            return null;
        }
        CStringTokenizer cStringTokenizer = new CStringTokenizer(cmdText, Connection.QuoteChar("GetSchemaTable")[0], Connection.EscapeChar("GetSchemaTable"));
        int num = cStringTokenizer.StartsWith("select") ? cStringTokenizer.FindTokenIndex("from") : !cStringTokenizer.StartsWith("insert") && !cStringTokenizer.StartsWith("update") && !cStringTokenizer.StartsWith("delete") ? -1 : cStringTokenizer.CurrentPosition;
        if (num == -1)
        {
            return null;
        }
        string result = cStringTokenizer.NextToken();
        cmdText = cStringTokenizer.NextToken();
        if (cmdText.Length > 0 && cmdText[0] == ',')
        {
            return null;
        }
        if (cmdText.Length == 2 && (cmdText[0] == 'a' || cmdText[0] == 'A') && (cmdText[1] == 's' || cmdText[1] == 'S'))
        {
            cmdText = cStringTokenizer.NextToken();
            cmdText = cStringTokenizer.NextToken();
            if (cmdText.Length > 0 && cmdText[0] == ',')
            {
                return null;
            }
        }
        return result;
    }

    internal void SetBaseTableNames(QualifiedTableName qualifiedTableName)
    {
        int fieldCount = FieldCount;
        for (int i = 0; i < fieldCount; i++)
        {
            if (_metadata[i].baseTableName == null)
            {
                _metadata[i].baseTableName = qualifiedTableName.Table;
                _metadata[i].baseSchemaName = qualifiedTableName.Schema;
                _metadata[i].baseCatalogName = qualifiedTableName.Catalog;
            }
        }
    }

    public string GetBaseTableName(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._baseTableName == null)
            {
                schema._baseTableName = GetColAttributeStr(i, Informix32.SQL_DESC.INFX_BASE_TABLE_NAME, Informix32.SQL_COLUMN.TABLE_NAME, Informix32.HANDLER.THROW);
                if (schema._name == null)
                {
                    schema._baseTableName = "";
                }
            }
            ifxTrace?.ApiExit();
            return schema._baseTableName;
        }
        throw ADP.DataReaderNoData();
    }

    private int GetDescFieldInt32(int i, Informix32.SQL_DESC attribute, Informix32.HANDLER handler)
    {
        int num = 0;
        if (Command != null && Command.Canceling)
        {
            return -1;
        }
        Buffer.WriteInt16(0, 0);
        num = 0;
        CNativeBuffer buffer = Buffer;
        using (OdbcDescriptorHandle odbcDescriptorHandle = new OdbcDescriptorHandle(StatementHandle, Informix32.SQL_ATTR.IMP_ROW_DESC))
        {
            Informix32.RetCode descriptionField = odbcDescriptorHandle.GetDescriptionField(i + 1, attribute, buffer, out num);
            if (descriptionField != 0)
            {
                if (descriptionField == Informix32.RetCode.ERROR && "HY091" == Command.GetDiagSqlState())
                {
                    Connection.FlagUnsupportedColAttr(attribute, Informix32.SQL_COLUMN.COUNT);
                }
                if (handler == Informix32.HANDLER.THROW)
                {
                    Connection.HandleError(StatementHandle, descriptionField);
                }
                return -1;
            }
            odbcDescriptorHandle.Dispose();
        }
        int result = Buffer.ReadInt32(0);
        GC.KeepAlive(this);
        return result;
    }

    private void SetIntervalPrecision(int i, InformixType type)
    {
        int descFieldInt = GetDescFieldInt32(i, Informix32.SQL_DESC.INFX_QUALIFIER, Informix32.HANDLER.IGNORE);
        _dataCache.GetSchema(i)._qualifier = descFieldInt;
        Qualifier.Decode(descFieldInt, out var _, out var end);
        Informix32.SQL_C sQL_C = type != InformixType.IntervalYearMonth ? Informix32.SQL_C.INTERVAL_DAY_TO_SECOND : Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH;
        OdbcDescriptorHandle descriptorHandle = Command.GetDescriptorHandle(Informix32.SQL_ATTR.APP_ROW_DESC);
        Informix32.RetCode retCode = descriptorHandle.SetDescriptionField1((short)(i + 1), Informix32.SQL_DESC.CONCISE_TYPE, (int)sQL_C);
        if (retCode != 0)
        {
            Command.Connection.HandleError(descriptorHandle, retCode);
        }
        retCode = descriptorHandle.SetDescriptionField1((short)(i + 1), Informix32.SQL_DESC.DATETIME_INTERVAL_CODE, (int)(sQL_C - 100));
        if (retCode != 0)
        {
            Command.Connection.HandleError(descriptorHandle, retCode);
        }
        retCode = descriptorHandle.SetDescriptionField1((short)(i + 1), Informix32.SQL_DESC.DATETIME_INTERVAL_PRECISION, 9);
        if (retCode != 0)
        {
            Command.Connection.HandleError(descriptorHandle, retCode);
        }
        if (end > InformixTimeUnit.Second)
        {
            retCode = descriptorHandle.SetDescriptionField1((short)(i + 1), Informix32.SQL_DESC.PRECISION, 7);
            if (retCode != 0)
            {
                Command.Connection.HandleError(descriptorHandle, retCode);
            }
        }
    }

    internal int GetPrecision(int i)
    {
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._precision == null)
            {
                schema._precision = (int)GetColAttribute(i, Informix32.SQL_DESC.PRECISION, Informix32.SQL_COLUMN.PRECISION, Informix32.HANDLER.THROW);
            }
            return (int)schema._precision;
        }
        throw ADP.DataReaderNoData();
    }

    internal int GetScale(int i)
    {
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._scale == null)
            {
                schema._scale = (int)GetColAttribute(i, Informix32.SQL_DESC.SCALE, Informix32.SQL_COLUMN.SCALE, Informix32.HANDLER.THROW);
            }
            return (int)schema._scale;
        }
        throw ADP.DataReaderNoData();
    }

    internal InformixType GetIfxType(int i)
    {
        if (_dataCache != null)
        {
            DbSchemaInfo schema = _dataCache.GetSchema(i);
            if (schema._ifxtype == null)
            {
                schema._typename = GetDataTypeName(i);
                schema._precision = GetPrecision(i);
                schema._ifxtype = _command.MappingTable.IfxNameToIfxType(schema._typename, (int)schema._precision);
            }
            return (InformixType)schema._ifxtype;
        }
        throw ADP.DataReaderNoData();
    }

    public InformixBlob GetIfxBlob(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (GetIfxType(i) != InformixType.Blob)
        {
            throw new InvalidCastException();
        }
        InformixBlob result = (InformixBlob)internalGetIfxSmartLOB(i, SmartLOBType.BLOB);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixClob GetIfxClob(int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (GetIfxType(i) != InformixType.Clob)
        {
            throw new InvalidCastException();
        }
        InformixClob result = (InformixClob)internalGetIfxSmartLOB(i, SmartLOBType.CLOB);
        ifxTrace?.ApiExit();
        return result;
    }

    internal object internalGetIfxSmartLOB(int i, SmartLOBType lobtype)
    {
        InformixClob result = null;
        InformixBlob result2 = null;
        InformixSmartLOBLocator ifxSmartLOBLocator = null;
        if (_isRead)
        {
            if (GetData(i, Informix32.SQL_C.SB_LOCATOR))
            {
                byte[] array = new byte[72];
                Buffer.ReadBytes(0, array, 0, 72);
                ifxSmartLOBLocator = new InformixSmartLOBLocator(array);
                if (SmartLOBType.CLOB == lobtype)
                {
                    result = new InformixClob(_command.Connection, ifxSmartLOBLocator);
                }
                else
                {
                    result2 = new InformixBlob(_command.Connection, ifxSmartLOBLocator);
                }
            }
            if (1 == (short)lobtype)
            {
                return result;
            }
            return result2;
        }
        throw ADP.DataReaderNoData();
    }
}
