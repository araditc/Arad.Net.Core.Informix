using System.Globalization;


namespace Arad.Net.Core.Informix;

internal sealed class Res
{
    internal const string DataCategory_Behavior = "DataCategory_Behavior";

    internal const string DataCategory_Data = "DataCategory_Data";

    internal const string DataCategory_Fill = "DataCategory_Fill";

    internal const string DataCategory_InfoMessage = "DataCategory_InfoMessage";

    internal const string DataCategory_StateChange = "DataCategory_StateChange";

    internal const string DataCategory_Update = "DataCategory_Update";

    internal const string ADP_NotAPermissionElement = "ADP_NotAPermissionElement";

    internal const string ADP_InvalidXMLBadVersion = "ADP_InvalidXMLBadVersion";

    internal const string ADP_WrongType = "ADP_WrongType";

    internal const string ADP_NoQuoteChange = "ADP_NoQuoteChange";

    internal const string ADP_MissingSourceCommand = "ADP_MissingSourceCommand";

    internal const string ADP_MissingSourceCommandConnection = "ADP_MissingSourceCommandConnection";

    internal const string ADP_CollectionUniqueValue = "ADP_CollectionUniqueValue";

    internal const string ADP_CollectionIsNotParent = "ADP_CollectionIsNotParent";

    internal const string ADP_CollectionIsParent = "ADP_CollectionIsParent";

    internal const string ADP_CollectionRemoveInvalidObject = "ADP_CollectionRemoveInvalidObject";

    internal const string ADP_CollectionIndexInt32 = "ADP_CollectionIndexInt32";

    internal const string ADP_CollectionIndexString = "ADP_CollectionIndexString";

    internal const string ADP_CollectionNullValue = "ADP_CollectionNullValue";

    internal const string ADP_CollectionInvalidType = "ADP_CollectionInvalidType";

    internal const string ADP_NullDataTable = "ADP_NullDataTable";

    internal const string ADP_ColumnSchemaExpression = "ADP_ColumnSchemaExpression";

    internal const string ADP_ColumnSchemaMismatch = "ADP_ColumnSchemaMismatch";

    internal const string ADP_ColumnSchemaMissing1 = "ADP_ColumnSchemaMissing1";

    internal const string ADP_ColumnSchemaMissing2 = "ADP_ColumnSchemaMissing2";

    internal const string ADP_InvalidSourceColumn = "ADP_InvalidSourceColumn";

    internal const string ADP_MissingColumnMapping = "ADP_MissingColumnMapping";

    internal const string ADP_InvalidAction = "ADP_InvalidAction";

    internal const string ADP_NullDataSet = "ADP_NullDataSet";

    internal const string ADP_MissingTableSchema = "ADP_MissingTableSchema";

    internal const string ADP_InvalidSourceTable = "ADP_InvalidSourceTable";

    internal const string ADP_MissingTableMapping = "ADP_MissingTableMapping";

    internal const string ADP_InvalidCommandType = "ADP_InvalidCommandType";

    internal const string ADP_InvalidUpdateRowSource = "ADP_InvalidUpdateRowSource";

    internal const string ADP_CommandTextRequired = "ADP_CommandTextRequired";

    internal const string ADP_ConnectionRequired = "ADP_ConnectionRequired";

    internal const string ADP_NoStoredProcedureExists = "ADP_NoStoredProcedureExists";

    internal const string ADP_OpenConnectionRequired = "ADP_OpenConnectionRequired";

    internal const string ADP_TransactionConnectionMismatch = "ADP_TransactionConnectionMismatch";

    internal const string ADP_TransactionRequired_Execute = "ADP_TransactionRequired_Execute";

    internal const string ADP_TransactionCompleted = "ADP_TransactionCompleted";

    internal const string ADP_OpenReaderExists = "ADP_OpenReaderExists";

    internal const string ADP_DeriveParametersNotSupported = "ADP_DeriveParametersNotSupported";

    internal const string ADP_MissingSelectCommand = "ADP_MissingSelectCommand";

    internal const string ADP_InvalidSchemaType = "ADP_InvalidSchemaType";

    internal const string ADP_FillSchemaRequiresSourceTableName = "ADP_FillSchemaRequiresSourceTableName";

    internal const string ADP_InvalidMaxRecords = "ADP_InvalidMaxRecords";

    internal const string ADP_InvalidStartRecord = "ADP_InvalidStartRecord";

    internal const string ADP_FillRequiresSourceTableName = "ADP_FillRequiresSourceTableName";

    internal const string ADP_FillChapterAutoIncrement = "ADP_FillChapterAutoIncrement";

    internal const string ADP_UpdateRequiresSourceTable = "ADP_UpdateRequiresSourceTable";

    internal const string ADP_InvalidUpdateStatus = "ADP_InvalidUpdateStatus";

    internal const string ADP_UpdateRequiresSourceTableName = "ADP_UpdateRequiresSourceTableName";

    internal const string ADP_MissingTableMappingDestination = "ADP_MissingTableMappingDestination";

    internal const string ADP_UpdateRequiresCommandClone = "ADP_UpdateRequiresCommandClone";

    internal const string ADP_UpdateRequiresCommandSelect = "ADP_UpdateRequiresCommandSelect";

    internal const string ADP_UpdateRequiresCommandInsert = "ADP_UpdateRequiresCommandInsert";

    internal const string ADP_UpdateRequiresCommandUpdate = "ADP_UpdateRequiresCommandUpdate";

    internal const string ADP_UpdateRequiresCommandDelete = "ADP_UpdateRequiresCommandDelete";

    internal const string ADP_UpdateNullRow = "ADP_UpdateNullRow";

    internal const string ADP_UpdateNullRowTable = "ADP_UpdateNullRowTable";

    internal const string ADP_UpdateMismatchRowTable = "ADP_UpdateMismatchRowTable";

    internal const string ADP_RowUpdateErrors = "ADP_RowUpdateErrors";

    internal const string ADP_UpdateConcurrencyViolation = "ADP_UpdateConcurrencyViolation";

    internal const string ADP_UpdateUnknownRowState = "ADP_UpdateUnknownRowState";

    internal const string ADP_InvalidCommandTimeout = "ADP_InvalidCommandTimeout";

    internal const string ADP_CommandIsActive = "ADP_CommandIsActive";

    internal const string ADP_UninitializedParameterSize = "ADP_UninitializedParameterSize";

    internal const string ADP_PrepareParameterType = "ADP_PrepareParameterType";

    internal const string ADP_PrepareParameterSize = "ADP_PrepareParameterSize";

    internal const string ADP_PrepareParameterScale = "ADP_PrepareParameterScale";

    internal const string ADP_ClosedConnectionError = "ADP_ClosedConnectionError";

    internal const string ADP_ConnectionAlreadyOpen = "ADP_ConnectionAlreadyOpen";

    internal const string ADP_NoConnectionString = "ADP_NoConnectionString";

    internal const string ADP_OpenConnectionPropertySet = "ADP_OpenConnectionPropertySet";

    internal const string ADP_EmptyDatabaseName = "ADP_EmptyDatabaseName";

    internal const string ADP_InvalidConnectTimeoutValue = "ADP_InvalidConnectTimeoutValue";

    internal const string ADP_ConnectTimeoutExpired = "ADP_ConnectTimeoutExpired";

    internal const string ADP_InvalidIsolationLevel = "ADP_InvalidIsolationLevel";

    internal const string ADP_InvalidPersistSecurityInfo = "ADP_InvalidPersistSecurityInfo";

    internal const string ADP_PoolFull = "ADP_PoolFull";

    internal const string ADP_InvalidSourceBufferIndex = "ADP_InvalidSourceBufferIndex";

    internal const string ADP_InvalidDestinationBufferIndex = "ADP_InvalidDestinationBufferIndex";

    internal const string ADP_DataReaderNoData = "ADP_DataReaderNoData";

    internal const string ADP_DataReaderClosed = "ADP_DataReaderClosed";

    internal const string ADP_StreamClosed = "ADP_StreamClosed";

    internal const string ADP_DynamicSQLJoinUnsupported = "ADP_DynamicSQLJoinUnsupported";

    internal const string ADP_DynamicSQLNoTableInfo = "ADP_DynamicSQLNoTableInfo";

    internal const string ADP_DynamicSQLNoKeyInfo = "ADP_DynamicSQLNoKeyInfo";

    internal const string ADP_DynamicSQLReadOnly = "ADP_DynamicSQLReadOnly";

    internal const string ADP_DynamicSQLNestedQuote = "ADP_DynamicSQLNestedQuote";

    internal const string ADP_NonSequentialColumnAccess = "ADP_NonSequentialColumnAccess";

    internal const string ADP_InvalidDataRowVersion = "ADP_InvalidDataRowVersion";

    internal const string ADP_InvalidParameterDirection = "ADP_InvalidParameterDirection";

    internal const string ADP_InvalidDataType = "ADP_InvalidDataType";

    internal const string ADP_UnknownDataType = "ADP_UnknownDataType";

    internal const string ADP_UnknownDataTypeCode = "ADP_UnknownDataTypeCode";

    internal const string ADP_InvalidSizeValue = "ADP_InvalidSizeValue";

    internal const string ADP_WhereClauseUnspecifiedValue = "ADP_WhereClauseUnspecifiedValue";

    internal const string ADP_TruncatedBytes = "ADP_TruncatedBytes";

    internal const string ADP_TruncatedString = "ADP_TruncatedString";

    internal const string ADP_ParallelTransactionsNotSupported = "ADP_ParallelTransactionsNotSupported";

    internal const string ADP_TransactionZombied = "ADP_TransactionZombied";

    internal const string DbConnection_InfoMessage = "DbConnection_InfoMessage";

    internal const string DbConnection_StateChange = "DbConnection_StateChange";

    internal const string DbConnection_State = "DbConnection_State";

    internal const string ADP_NonSeqByteAccess = "ADP_NonSeqByteAccess";

    internal const string DbCommand_CommandText = "DbCommand_CommandText";

    internal const string DbCommand_CommandTimeout = "DbCommand_CommandTimeout";

    internal const string DbCommand_CommandType = "DbCommand_CommandType";

    internal const string DbCommand_Connection = "DbCommand_Connection";

    internal const string DbCommand_Parameters = "DbCommand_Parameters";

    internal const string DbCommand_Transaction = "DbCommand_Transaction";

    internal const string DbCommand_UpdatedRowSource = "DbCommand_UpdatedRowSource";

    internal const string DbDataAdapter_DeleteCommand = "DbDataAdapter_DeleteCommand";

    internal const string DbDataAdapter_InsertCommand = "DbDataAdapter_InsertCommand";

    internal const string DbDataAdapter_SelectCommand = "DbDataAdapter_SelectCommand";

    internal const string DbDataAdapter_UpdateCommand = "DbDataAdapter_UpdateCommand";

    internal const string DataParameter_DbType = "DataParameter_DbType";

    internal const string DataParameter_Direction = "DataParameter_Direction";

    internal const string DataParameter_IsNullable = "DataParameter_IsNullable";

    internal const string DataParameter_ParameterName = "DataParameter_ParameterName";

    internal const string DataParameter_SourceColumn = "DataParameter_SourceColumn";

    internal const string DataParameter_SourceVersion = "DataParameter_SourceVersion";

    internal const string DataParameter_Value = "DataParameter_Value";

    internal const string DbDataParameter_Precision = "DbDataParameter_Precision";

    internal const string DbDataParameter_Scale = "DbDataParameter_Scale";

    internal const string DbDataParameter_Size = "DbDataParameter_Size";

    internal const string Ifx_UnknownSQLType = "Ifx_UnknownSQLType";

    internal const string Ifx_UnknownURTType = "Ifx_UnknownURTType";

    internal const string Ifx_InvalidHandle = "Ifx_InvalidHandle";

    internal const string Ifx_UnsupportedCommandTypeTableDirect = "Ifx_UnsupportedCommandTypeTableDirect";

    internal const string Ifx_NegativeArgument = "Ifx_NegativeArgument";

    internal const string Ifx_InvalidArgument = "Ifx_InvalidArgument";

    internal const string Ifx_CantSetPropertyOnOpenConnection = "Ifx_CantSetPropertyOnOpenConnection";

    internal const string Ifx_UnsupportedIsolationLevel = "Ifx_UnsupportedIsolationLevel";

    internal const string Ifx_CantEnableConnectionpooling = "Ifx_CantEnableConnectionpooling";

    internal const string Ifx_CantAllocateEnvironmentHandle = "Ifx_CantAllocateEnvironmentHandle";

    internal const string Ifx_NotInTransaction = "Ifx_NotInTransaction";

    internal const string Ifx_UnknownSQLCType = "Ifx_UnknownSQLCType";

    internal const string Ifx_ExceptionMessage = "Ifx_ExceptionMessage";

    internal const string Ifx_ExceptionNoInfoMsg = "Ifx_ExceptionNoInfoMsg";

    internal const string IfxConnection_ConnectionStringTooLong = "IfxConnection_ConnectionStringTooLong";

    internal const string Ifx_UnknownIfxType = "Ifx_UnknownIfxType";

    internal const string IfxCommandBuilder_DataAdapter = "IfxCommandBuilder_DataAdapter";

    internal const string IfxCommandBuilder_QuotePrefix = "IfxCommandBuilder_QuotePrefix";

    internal const string IfxCommandBuilder_QuoteSuffix = "IfxCommandBuilder_QuoteSuffix";

    internal const string IfxConnection_ConnectionString = "IfxConnection_ConnectionString";

    internal const string IfxConnection_ConnectionTimeout = "IfxConnection_ConnectionTimeout";

    internal const string IfxConnection_Database = "IfxConnection_Database";

    internal const string IfxConnection_DataSource = "IfxConnection_DataSource";

    internal const string IfxConnection_Server = "IfxConnection_Server";

    internal const string IfxConnection_ServerVersion = "IfxConnection_ServerVersion";

    internal const string IfxConnection_ServerType = "IfxConnection_ServerType";

    internal const string IfxConnection_Client_Locale = "IfxConnection_Client_Locale";

    internal const string IfxConnection_Database_Locale = "IfxConnection_Database_Locale";

    internal const string IfxConnection_FetchBufferSize = "IfxConnection_FetchBufferSize";

    internal const string IfxConnection_PacketSize = "IfxConnection_PacketSize";

    internal const string IfxConnection_LeaveTrailingSpaces = "IfxConnection_LeaveTrailingSpaces";

    internal const string IfxParameter_IfxType = "IfxParameter_IfxType";

    internal const string IfxParameter_DirectionNotSupported = "IfxParameter_DirectionNotSupported";

    internal const string IfxNullString = "Null";

    internal static Res loader;

    private Res()
    {
    }

    private static Res GetLoader()
    {
        if (loader == null && loader == null)
        {
            loader = new Res();
        }
        return loader;
    }

    internal static string GetString(string name, params string[] args)
    {
        Res res = GetLoader();
        return null;
    }

    internal static string GetString(CultureInfo culture, string name, params string[] args)
    {
        Res res = GetLoader();
        if (res == null)
        {
            return null;
        }
        return null;
    }

    internal static string GetString(string name)
    {
        Res res = GetLoader();
        return null;
    }

    internal static string GetString(CultureInfo culture, string name)
    {
        Res res = GetLoader();
        if (res == null)
        {
            return null;
        }
        return null;
    }
}
