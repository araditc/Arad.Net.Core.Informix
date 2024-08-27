using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Arad.Net.Core.Informix.System.Data.Common;



namespace Arad.Net.Core.Informix;
public sealed class InformixCommand : DbCommand, ICloneable
{
    private static int s_objectTypeCount;

    internal readonly int ObjectID = Interlocked.Increment(ref s_objectTypeCount);

    private string _commandText;

    private CommandType _commandType;

    private int _commandTimeout = 30;

    private UpdateRowSource _updatedRowSource = UpdateRowSource.Both;

    private bool _designTimeInvisible;

    private bool _isPrepared;

    internal bool _loautoset;

    private InformixConnection _connection;

    private TypeMap mappingTable;

    private string userDefinedTypeFormat;

    private InformixTransaction _transaction;

    private WeakReference _weakDataReaderReference;

    private CMDWrapper _cmdWrapper;

    private InformixParameterCollection _parameterCollection;

    private ConnectionState _cmdState;

    internal TypeMap MappingTable => mappingTable;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UserDefinedTypeFormat
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return userDefinedTypeFormat;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            userDefinedTypeFormat = value;
            mappingTable.SetUserDefinedTypeMaps(value);
            ifxTrace?.ApiExit();
        }
    }

    internal bool Canceling => _cmdWrapper.Canceling;

    public override string CommandText
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            string commandText = _commandText;
            if (commandText == null)
            {
                return "";
            }
            return commandText;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (_commandText != value)
            {
                PropertyChanging();
                _commandText = value;
            }
            ifxTrace?.ApiExit();
        }
    }

    public override int CommandTimeout
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _commandTimeout;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (value < 0)
            {
                throw ADP.InvalidCommandTimeout(value);
            }
            if (value != _commandTimeout)
            {
                PropertyChanging();
                _commandTimeout = value;
            }
            ifxTrace?.ApiExit();
        }
    }

    [DefaultValue(CommandType.Text)]
    public override CommandType CommandType
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            CommandType commandType = _commandType;
            if (commandType == 0)
            {
                return CommandType.Text;
            }
            return commandType;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            switch (value)
            {
                case CommandType.Text:
                case CommandType.StoredProcedure:
                    PropertyChanging();
                    _commandType = value;
                    ifxTrace?.ApiExit();
                    break;
                case CommandType.TableDirect:
                    throw ODBC.NotSupportedCommandType(value);
                default:
                    throw ADP.InvalidCommandType(value);
            }
        }
    }

    public new InformixConnection Connection
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _connection;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (value != _connection)
            {
                PropertyChanging();
                if (_connection != null)
                {
                    DisconnectFromDataReaderAndConnection();
                }
                _connection = value;
                if (_connection != null)
                {
                    mappingTable = _connection.MappingTable.Clone();
                }
            }
            ifxTrace?.ApiExit();
        }
    }

    protected override DbConnection DbConnection
    {
        get
        {
            return Connection;
        }
        set
        {
            Connection = (InformixConnection)value;
        }
    }

    protected override DbParameterCollection DbParameterCollection => Parameters;

    protected override DbTransaction DbTransaction
    {
        get
        {
            return Transaction;
        }
        set
        {
            Transaction = (InformixTransaction)value;
        }
    }

    [DefaultValue(true)]
    [DesignOnly(true)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool DesignTimeVisible
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return !_designTimeInvisible;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            _designTimeInvisible = !value;
            TypeDescriptor.Refresh(this);
            ifxTrace?.ApiExit();
        }
    }

    internal bool HasParameters => _parameterCollection != null;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public new InformixParameterCollection Parameters
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (_parameterCollection == null)
            {
                _parameterCollection = new InformixParameterCollection();
            }
            ifxTrace?.ApiExit();
            return _parameterCollection;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new InformixTransaction Transaction
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (_transaction != null && _transaction.Connection == null)
            {
                _transaction = null;
            }
            ifxTrace?.ApiExit();
            return _transaction;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (_transaction != value)
            {
                PropertyChanging();
                _transaction = value;
            }
            ifxTrace?.ApiExit();
        }
    }

    [DefaultValue(UpdateRowSource.Both)]
    public override UpdateRowSource UpdatedRowSource
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _updatedRowSource;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if ((uint)value <= 3u)
            {
                _updatedRowSource = value;
                ifxTrace?.ApiExit();
                return;
            }
            throw ADP.InvalidUpdateRowSource(value);
        }
    }

    internal string AdjustedCommandText
    {
        get
        {
            if (_commandType == CommandType.TableDirect)
            {
                return "SELECT * FROM " + _commandText + ";";
            }
            if (_commandType == CommandType.StoredProcedure)
            {
                if (_parameterCollection != null && _parameterCollection.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    if (_parameterCollection[0].Direction == ParameterDirection.ReturnValue)
                    {
                        stringBuilder.Append("{?=CALL " + _commandText + "(");
                    }
                    else
                    {
                        stringBuilder.Append("{CALL " + _commandText + "(");
                    }
                    for (int i = 0; i < _parameterCollection.Count; i++)
                    {
                        if (_parameterCollection[i].Direction != ParameterDirection.ReturnValue)
                        {
                            if (string.Compare(stringBuilder[stringBuilder.Length - 1].ToString(), "?") == 0)
                            {
                                stringBuilder.Append(",");
                            }
                            stringBuilder.Append("?");
                        }
                    }
                    stringBuilder.Append(")}");
                    return stringBuilder.ToString();
                }
                return "EXECUTE PROCEDURE " + _commandText + "();";
            }
            return _commandText;
        }
    }

    public InformixCommand()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        GC.SuppressFinalize(this);
        ifxTrace?.ApiExit();
    }

    public InformixCommand(string cmdText)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(cmdText);
        CommandText = cmdText;
        ifxTrace?.ApiExit();
    }

    public InformixCommand(string cmdText, InformixConnection connection)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(cmdText, connection.ToString());
        CommandText = cmdText;
        Connection = connection;
        ifxTrace?.ApiExit();
    }

    public InformixCommand(string cmdText, InformixConnection connection, InformixTransaction transaction)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(cmdText, connection, transaction);
        CommandText = cmdText;
        Connection = connection;
        Transaction = transaction;
        ifxTrace?.ApiExit();
    }

    private void DisposeDeadDataReader()
    {
        if (ConnectionState.Fetching == _cmdState && _weakDataReaderReference != null && !_weakDataReaderReference.IsAlive)
        {
            if (_cmdWrapper != null)
            {
                _cmdWrapper.FreeKeyInfoStatementHandle(Informix32.STMT.CLOSE);
                _cmdWrapper.FreeStatementHandle(Informix32.STMT.CLOSE);
            }
            CloseFromDataReader();
        }
    }

    private void DisposeDataReader()
    {
        if (_weakDataReaderReference != null)
        {
            IDisposable disposable = (IDisposable)_weakDataReaderReference.Target;
            if (disposable != null && _weakDataReaderReference.IsAlive)
            {
                disposable.Dispose();
            }
            CloseFromDataReader();
        }
    }

    internal void DisconnectFromDataReaderAndConnection()
    {
        InformixDataReader ifxDataReader = null;
        if (_weakDataReaderReference != null)
        {
            InformixDataReader ifxDataReader2 = (InformixDataReader)_weakDataReaderReference.Target;
            if (_weakDataReaderReference.IsAlive)
            {
                ifxDataReader = ifxDataReader2;
            }
        }
        if (ifxDataReader != null)
        {
            ifxDataReader.Command = null;
        }
        _transaction = null;
        if (_connection != null)
        {
            _connection.RemoveWeakReference(this);
            _connection = null;
        }
        if (ifxDataReader == null)
        {
            CloseCommandWrapper();
        }
        _cmdWrapper = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisconnectFromDataReaderAndConnection();
            _parameterCollection = null;
            CommandText = null;
        }
        _cmdWrapper = null;
        _isPrepared = false;
        base.Dispose(disposing);
    }

    public void ResetCommandTimeout()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (30 != _commandTimeout)
        {
            PropertyChanging();
            _commandTimeout = 30;
        }
        ifxTrace?.ApiExit();
    }

    private bool ShouldSerializeCommandTimeout()
    {
        return 30 != _commandTimeout;
    }

    internal OdbcDescriptorHandle GetDescriptorHandle(Informix32.SQL_ATTR attribute)
    {
        return _cmdWrapper.GetDescriptorHandle(attribute);
    }

    internal CMDWrapper GetStatementHandle()
    {
        if (_cmdWrapper == null)
        {
            _cmdWrapper = new CMDWrapper(_connection);
            _connection.AddWeakReference(this, 1);
        }
        if (_cmdWrapper._dataReaderBuf == null)
        {
            _cmdWrapper._dataReaderBuf = new CNativeBuffer(65536);
        }
        if (_cmdWrapper.StatementHandle == null)
        {
            _isPrepared = false;
            _cmdWrapper.CreateStatementHandle();
        }
        else if (_parameterCollection != null && _parameterCollection.RebindCollection)
        {
            _cmdWrapper.FreeStatementHandle(Informix32.STMT.RESET_PARAMS);
        }
        return _cmdWrapper;
    }

    public override void Cancel()
    {
        CMDWrapper cmdWrapper = _cmdWrapper;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (cmdWrapper != null)
        {
            cmdWrapper.Canceling = true;
            OdbcStatementHandle statementHandle = cmdWrapper.StatementHandle;
            if (statementHandle != null)
            {
                lock (statementHandle)
                {
                    Informix32.RetCode retCode = statementHandle.Cancel();
                    if ((uint)retCode > 1u)
                    {
                        throw cmdWrapper.Connection.HandleErrorNoThrow(statementHandle, retCode);
                    }
                }
            }
        }
        ifxTrace?.ApiExit();
    }

    object ICloneable.Clone()
    {
        InformixCommand ifxCommand = new InformixCommand();
        ifxCommand.CommandText = CommandText;
        ifxCommand.CommandTimeout = CommandTimeout;
        ifxCommand.CommandType = CommandType;
        ifxCommand.Connection = Connection;
        ifxCommand.Transaction = Transaction;
        ifxCommand.UpdatedRowSource = UpdatedRowSource;
        ifxCommand.userDefinedTypeFormat = userDefinedTypeFormat;
        ifxCommand.mappingTable = mappingTable.Clone();
        if (_parameterCollection != null && 0 < Parameters.Count)
        {
            InformixParameterCollection parameters = ifxCommand.Parameters;
            foreach (ICloneable parameter in Parameters)
            {
                parameters.Add(parameter.Clone());
            }
        }
        return ifxCommand;
    }

    internal bool RecoverFromConnection()
    {
        DisposeDeadDataReader();
        return _cmdState == ConnectionState.Closed;
    }

    private void CloseCommandWrapper()
    {
        CMDWrapper cmdWrapper = _cmdWrapper;
        if (cmdWrapper == null)
        {
            return;
        }
        try
        {
            cmdWrapper.Dispose();
            if (_connection != null)
            {
                _connection.RemoveWeakReference(this);
            }
        }
        finally
        {
            _cmdWrapper = null;
        }
    }

    internal void CloseFromConnection()
    {
        if (_parameterCollection != null)
        {
            _parameterCollection.RebindCollection = true;
        }
        DisposeDataReader();
        CloseCommandWrapper();
        _isPrepared = false;
        _transaction = null;
    }

    internal void CloseFromDataReader()
    {
        _weakDataReaderReference = null;
        if (_connection != null)
        {
            if (ConnectionState.Fetching == _cmdState)
            {
                _connection.SetStateFetchingFalse();
            }
            else
            {
                _connection.SetStateExecutingFalse();
            }
        }
        _cmdState = ConnectionState.Closed;
    }

    public new InformixParameter CreateParameter()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return new InformixParameter();
    }

    protected override DbParameter CreateDbParameter()
    {
        return CreateParameter();
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        return ExecuteReader(behavior);
    }

    public override int ExecuteNonQuery()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        using InformixDataReader ifxDataReader = ExecuteReaderObject(CommandBehavior.Default, "ExecuteNonQuery", needReader: false);
        ifxDataReader.Close();
        ifxTrace?.ApiExit();
        return ifxDataReader.RecordsAffected;
    }

    public new InformixDataReader ExecuteReader()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return ExecuteReader(CommandBehavior.Default);
    }

    public new InformixDataReader ExecuteReader(CommandBehavior behavior)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return ExecuteReaderObject(behavior, "ExecuteReader", needReader: true);
    }

    internal InformixDataReader ExecuteReaderFromSQLMethod(object[] methodArguments, Informix32.SQL_API method)
    {
        return ExecuteReaderObject(CommandBehavior.Default, method.ToString(), needReader: true, methodArguments, method);
    }

    private InformixDataReader ExecuteReaderObject(CommandBehavior behavior, string method, bool needReader)
    {
        if (CommandText == null || CommandText.Length == 0)
        {
            throw ADP.CommandTextRequired(method);
        }
        return ExecuteReaderObject(behavior, method, needReader, null, Informix32.SQL_API.SQLEXECDIRECT);
    }

    private InformixDataReader ExecuteReaderObject(CommandBehavior behavior, string method, bool needReader, object[] methodArguments, Informix32.SQL_API odbcApiMethod)
    {
        InformixDataReader ifxDataReader = null;
        try
        {
            DisposeDeadDataReader();
            ValidateConnectionAndTransaction(method);
            if ((CommandBehavior.SingleRow & behavior) != 0)
            {
                behavior |= CommandBehavior.SingleResult;
            }
            OdbcStatementHandle statementHandle = GetStatementHandle().StatementHandle;
            _cmdWrapper.Canceling = false;
            if (_weakDataReaderReference != null && _weakDataReaderReference.IsAlive)
            {
                object target = _weakDataReaderReference.Target;
                if (target != null && _weakDataReaderReference.IsAlive && !((InformixDataReader)target).IsClosed)
                {
                    throw ADP.OpenReaderExists();
                }
            }
            ifxDataReader = new InformixDataReader(this, _cmdWrapper, behavior);
            if (!Connection.ProviderInfo.NoQueryTimeout)
            {
                TrySetStatementAttribute(statementHandle, Informix32.SQL_ATTR.QUERY_TIMEOUT, CommandTimeout);
            }
            if (ifxDataReader.IsBehavior(CommandBehavior.KeyInfo) || ifxDataReader.IsBehavior(CommandBehavior.SchemaOnly))
            {
                Informix32.RetCode retCode = statementHandle.Prepare(AdjustedCommandText);
                if (retCode != 0)
                {
                    _connection.HandleError(statementHandle, retCode);
                }
            }
            bool success = false;
            CNativeBuffer cNativeBuffer = _cmdWrapper._nativeParameterBuffer;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                if (_parameterCollection != null && 0 < _parameterCollection.Count)
                {
                    int num = _parameterCollection.CalcParameterBufferSize(this);
                    if (cNativeBuffer == null || cNativeBuffer.Length < num)
                    {
                        cNativeBuffer?.Dispose();
                        cNativeBuffer = new CNativeBuffer(num);
                        _cmdWrapper._nativeParameterBuffer = cNativeBuffer;
                    }
                    else
                    {
                        cNativeBuffer.ZeroMemory();
                    }
                    cNativeBuffer.DangerousAddRef(ref success);
                    int count = _parameterCollection.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (!_loautoset && (_parameterCollection[i].IfxType == InformixType.Blob || _parameterCollection[i].IfxType == InformixType.Clob))
                        {
                            Informix32.RetCode retCode = statementHandle.SetStatementAttribute(Informix32.SQL_ATTR.LO_AUTOMATIC, 1, Informix32.SQL_IS.INTEGER);
                            _loautoset = true;
                            if (retCode != 0)
                            {
                                _connection.HandleError(statementHandle, retCode);
                            }
                        }
                    }
                    _parameterCollection.Bind(this, _cmdWrapper, cNativeBuffer);
                }
                if (!ifxDataReader.IsBehavior(CommandBehavior.SchemaOnly))
                {
                    Informix32.RetCode retCode;
                    if ((ifxDataReader.IsBehavior(CommandBehavior.KeyInfo) || ifxDataReader.IsBehavior(CommandBehavior.SchemaOnly)) && CommandType != CommandType.StoredProcedure)
                    {
                        retCode = statementHandle.NumberOfResultColumns(out var columnsAffected);
                        switch (retCode)
                        {
                            case Informix32.RetCode.SUCCESS:
                            case Informix32.RetCode.SUCCESS_WITH_INFO:
                                if (columnsAffected > 0)
                                {
                                    ifxDataReader.GetSchemaTable();
                                }
                                break;
                            default:
                                _connection.HandleError(statementHandle, retCode);
                                break;
                            case Informix32.RetCode.NO_DATA:
                                break;
                        }
                    }
                    retCode = odbcApiMethod switch
                    {
                        Informix32.SQL_API.SQLEXECDIRECT => !ifxDataReader.IsBehavior(CommandBehavior.KeyInfo) && !_isPrepared ? statementHandle.ExecuteDirect(AdjustedCommandText) : statementHandle.Execute(),
                        Informix32.SQL_API.SQLTABLES => statementHandle.Tables((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (string)methodArguments[3]),
                        Informix32.SQL_API.SQLCOLUMNS => statementHandle.Columns((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (string)methodArguments[3]),
                        Informix32.SQL_API.SQLPROCEDURES => statementHandle.Procedures((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2]),
                        Informix32.SQL_API.SQLPROCEDURECOLUMNS => statementHandle.ProcedureColumns((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (string)methodArguments[3]),
                        Informix32.SQL_API.SQLSTATISTICS => statementHandle.Statistics((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (short)methodArguments[3], (short)methodArguments[4]),
                        Informix32.SQL_API.SQLGETTYPEINFO => statementHandle.GetTypeInfo((short)methodArguments[0]),
                        _ => throw ADP.InvalidOperation(method.ToString()),
                    };
                    if (retCode != 0 && Informix32.RetCode.NO_DATA != retCode)
                    {
                        _connection.HandleError(statementHandle, retCode);
                    }
                }
            }
            finally
            {
                if (success)
                {
                    cNativeBuffer.DangerousRelease();
                }
            }
            _weakDataReaderReference = new WeakReference(ifxDataReader);
            _connection.SetStateFetchingTrue();
            if (!ifxDataReader.IsBehavior(CommandBehavior.SchemaOnly))
            {
                ifxDataReader.FirstResult();
            }
            _cmdState = ConnectionState.Fetching;
        }
        finally
        {
            if (ConnectionState.Fetching != _cmdState)
            {
                if (ifxDataReader != null)
                {
                    if (_parameterCollection != null)
                    {
                        _parameterCollection.ClearBindings();
                    }
                    ifxDataReader.Dispose();
                }
                if (_cmdState != 0)
                {
                    _cmdState = ConnectionState.Closed;
                    _connection.SetStateExecutingFalse();
                }
            }
        }
        return ifxDataReader;
    }

    public override object ExecuteScalar()
    {
        object result = null;
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        using (IDataReader dataReader = ExecuteReaderObject(CommandBehavior.Default, "ExecuteScalar", needReader: false))
        {
            if (dataReader.Read() && 0 < dataReader.FieldCount)
            {
                result = dataReader.GetValue(0);
            }
            dataReader.Close();
        }
        ifxTrace?.ApiExit();
        return result;
    }

    internal string GetDiagSqlState()
    {
        return _cmdWrapper.GetDiagSqlState();
    }

    private void PropertyChanging()
    {
        _isPrepared = false;
    }

    public override void Prepare()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ValidateOpenConnection("Prepare");
        if ((ConnectionState.Fetching & _connection.InternalState) != 0)
        {
            throw ADP.OpenReaderExists();
        }
        if (CommandType == CommandType.TableDirect)
        {
            ifxTrace?.ApiExit();
            return;
        }
        DisposeDeadDataReader();
        GetStatementHandle();
        OdbcStatementHandle statementHandle = _cmdWrapper.StatementHandle;
        Informix32.RetCode retCode = statementHandle.Prepare(AdjustedCommandText);
        if (retCode != 0)
        {
            _connection.HandleError(statementHandle, retCode);
        }
        _isPrepared = true;
        ifxTrace?.ApiExit();
    }

    private void TrySetStatementAttribute(OdbcStatementHandle stmt, Informix32.SQL_ATTR stmtAttribute, nint value)
    {
        Informix32.RetCode retCode = stmt.SetStatementAttribute(stmtAttribute, value, Informix32.SQL_IS.UINTEGER);
        if (retCode == Informix32.RetCode.ERROR)
        {
            stmt.GetDiagnosticField(out var sqlState);
            if (sqlState == "HYC00" || sqlState == "HY092")
            {
                Connection.FlagUnsupportedStmtAttr(stmtAttribute);
            }
        }
    }

    private void ValidateOpenConnection(string methodName)
    {
        InformixConnection connection = Connection;
        if (connection == null)
        {
            throw ADP.ConnectionRequired(methodName);
        }
        ConnectionState state = connection.State;
        if (ConnectionState.Open != state)
        {
            throw ADP.OpenConnectionRequired(methodName, state);
        }
    }

    private void ValidateConnectionAndTransaction(string method)
    {
        if (_connection == null)
        {
            throw ADP.ConnectionRequired(method);
        }
        _transaction = _connection.SetStateExecuting(method, Transaction);
        _cmdState = ConnectionState.Executing;
    }
}
