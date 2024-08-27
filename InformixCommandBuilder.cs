using Arad.Net.Core.Informix.System;
using Arad.Net.Core.Informix.System.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;



namespace Arad.Net.Core.Informix;
public sealed class InformixCommandBuilder : DbCommandBuilder
{
    public new InformixDataAdapter DataAdapter
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return base.DataAdapter as InformixDataAdapter;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            base.DataAdapter = value;
            ifxTrace?.ApiExit();
        }
    }

    public InformixCommandBuilder()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        GC.SuppressFinalize(this);
        ifxTrace?.ApiExit();
    }

    public InformixCommandBuilder(InformixDataAdapter adapter)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        DataAdapter = adapter;
        ifxTrace?.ApiExit();
    }

    private void IfxRowUpdatingHandler(object sender, InformixRowUpdatingEventArgs ruevent)
    {
        RowUpdatingHandler(ruevent);
    }

    public new InformixCommand GetInsertCommand()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (InformixCommand)base.GetInsertCommand();
    }

    public new InformixCommand GetInsertCommand(bool useColumnsForParameterNames)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (InformixCommand)base.GetInsertCommand(useColumnsForParameterNames);
    }

    public new InformixCommand GetUpdateCommand()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (InformixCommand)base.GetUpdateCommand();
    }

    public new InformixCommand GetUpdateCommand(bool useColumnsForParameterNames)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (InformixCommand)base.GetUpdateCommand(useColumnsForParameterNames);
    }

    public new InformixCommand GetDeleteCommand()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (InformixCommand)base.GetDeleteCommand();
    }

    public new InformixCommand GetDeleteCommand(bool useColumnsForParameterNames)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return (InformixCommand)base.GetDeleteCommand(useColumnsForParameterNames);
    }

    protected override string GetParameterName(int parameterOrdinal)
    {
        return "p" + parameterOrdinal.ToString(CultureInfo.InvariantCulture);
    }

    protected override string GetParameterName(string parameterName)
    {
        return parameterName;
    }

    protected override string GetParameterPlaceholder(int parameterOrdinal)
    {
        return "?";
    }

    protected override void ApplyParameterInfo(DbParameter parameter, DataRow datarow, StatementType statementType, bool whereClause)
    {
        InformixParameter ifxParameter = (InformixParameter)parameter;
        object obj = datarow[SchemaTableColumn.ProviderType];
        ifxParameter.IfxType = (InformixType)obj;
        object obj2 = datarow[SchemaTableColumn.NumericPrecision];
        if (DBNull.Value != obj2)
        {
            byte b = (byte)(short)obj2;
            ifxParameter.PrecisionInternal = (byte)(byte.MaxValue != b ? b : 0);
        }
        obj2 = datarow[SchemaTableColumn.NumericScale];
        if (DBNull.Value != obj2)
        {
            byte b2 = (byte)(short)obj2;
            ifxParameter.ScaleInternal = (byte)(byte.MaxValue != b2 ? b2 : 0);
        }
    }

    public static void DeriveParameters(InformixCommand command)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (command == null)
        {
            throw ADP.ArgumentNull("command");
        }
        switch (command.CommandType)
        {
            case CommandType.Text:
                throw ADP.DeriveParametersNotSupported(command);
            case CommandType.TableDirect:
                throw ADP.DeriveParametersNotSupported(command);
            default:
                throw ADP.InvalidCommandType(command.CommandType);
            case CommandType.StoredProcedure:
                {
                    if (string.IsNullOrEmpty(command.CommandText))
                    {
                        throw ADP.CommandTextRequired("DeriveParameters");
                    }
                    InformixConnection connection = command.Connection;
                    if (connection == null)
                    {
                        throw ADP.ConnectionRequired("DeriveParameters");
                    }
                    ConnectionState state = connection.State;
                    if (ConnectionState.Open != state)
                    {
                        throw ADP.OpenConnectionRequired("DeriveParameters", state);
                    }
                    InformixParameter[] array = DeriveParametersFromStoredProcedure(connection, command);
                    InformixParameterCollection parameters = command.Parameters;
                    parameters.Clear();
                    int num = array.Length;
                    if (0 < num)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            parameters.Add(array[i]);
                        }
                    }
                    ifxTrace?.ApiExit();
                    break;
                }
        }
    }

    private static InformixParameter[] DeriveParametersFromStoredProcedure(InformixConnection connection, InformixCommand command)
    {
        List<InformixParameter> list = new List<InformixParameter>();
        CMDWrapper statementHandle = command.GetStatementHandle();
        OdbcStatementHandle statementHandle2 = statementHandle.StatementHandle;
        string text = connection.QuoteChar("DeriveParameters");
        string[] array = MultipartIdentifier.ParseMultipartIdentifier(command.CommandText, text, text, '.', 4, removequotes: true, SR.ODBC_ODBCCommandText, ThrowOnEmptyMultipartName: false);
        if (array[3] == null)
        {
            array[3] = command.CommandText;
        }
        Informix32.RetCode retCode = statementHandle2.ProcedureColumns(array[1], array[2], array[3], null);
        if (retCode != 0)
        {
            connection.HandleError(statementHandle2, retCode);
        }
        using (InformixDataReader ifxDataReader = new InformixDataReader(command, statementHandle, CommandBehavior.Default))
        {
            ifxDataReader.FirstResult();
            int fieldCount = ifxDataReader.FieldCount;
            while (ifxDataReader.Read())
            {
                InformixParameter ifxParameter = new InformixParameter();
                ifxParameter.ParameterName = ifxDataReader.GetString(3);
                switch ((Informix32.SQL_PARAM)ifxDataReader.GetInt16(4))
                {
                    case Informix32.SQL_PARAM.INPUT:
                        ifxParameter.Direction = ParameterDirection.Input;
                        break;
                    case Informix32.SQL_PARAM.OUTPUT:
                        ifxParameter.Direction = ParameterDirection.Output;
                        break;
                    case Informix32.SQL_PARAM.INPUT_OUTPUT:
                        ifxParameter.Direction = ParameterDirection.InputOutput;
                        break;
                    case Informix32.SQL_PARAM.RESULT_COL:
                    case Informix32.SQL_PARAM.RETURN_VALUE:
                        ifxParameter.Direction = ParameterDirection.ReturnValue;
                        break;
                }
                Informix32.SQL_TYPE sQL_TYPE = (Informix32.SQL_TYPE)ifxDataReader.GetInt16(5);
                if (sQL_TYPE == Informix32.SQL_TYPE.TIMESTAMP)
                {
                    sQL_TYPE = Informix32.SQL_TYPE.TYPE_TIMESTAMP;
                }
                ifxParameter.IfxType = TypeMap.FromSqlType(sQL_TYPE)._odbcType;
                ifxParameter.Size = ifxDataReader.GetInt32(7);
                InformixType ifxType = ifxParameter.IfxType;
                if ((uint)(ifxType - 6) <= 1u)
                {
                    ifxParameter.ScaleInternal = (byte)ifxDataReader.GetInt16(9);
                    ifxParameter.PrecisionInternal = (byte)ifxDataReader.GetInt32(7);
                }
                list.Add(ifxParameter);
            }
        }
        retCode = statementHandle2.CloseCursor();
        return list.ToArray();
    }

    public override string QuoteIdentifier(string unquotedIdentifier)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return QuoteIdentifier(unquotedIdentifier, null);
    }

    public string QuoteIdentifier(string unquotedIdentifier, InformixConnection connection)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ADP.CheckArgumentNull(unquotedIdentifier, "unquotedIdentifier");
        string text = QuotePrefix;
        string quoteSuffix = QuoteSuffix;
        if (string.IsNullOrEmpty(text))
        {
            if (connection == null)
            {
                connection = DataAdapter?.SelectCommand?.Connection;
                if (connection == null)
                {
                    throw ADP.QuotePrefixNotSet("QuoteIdentifier");
                }
            }
            text = connection.QuoteChar("QuoteIdentifier");
            quoteSuffix = text;
        }
        if (!string.IsNullOrEmpty(text) && text != " ")
        {
            ifxTrace?.ApiExit();
            return ADP.BuildQuotedString(text, quoteSuffix, unquotedIdentifier);
        }
        ifxTrace?.ApiExit();
        return unquotedIdentifier;
    }

    protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
    {
        if (adapter == base.DataAdapter)
        {
            ((InformixDataAdapter)adapter).RowUpdating -= IfxRowUpdatingHandler;
        }
        else
        {
            ((InformixDataAdapter)adapter).RowUpdating += IfxRowUpdatingHandler;
        }
    }

    public override string UnquoteIdentifier(string quotedIdentifier)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return UnquoteIdentifier(quotedIdentifier, null);
    }

    public string UnquoteIdentifier(string quotedIdentifier, InformixConnection connection)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ADP.CheckArgumentNull(quotedIdentifier, "quotedIdentifier");
        string text = QuotePrefix;
        string quoteSuffix = QuoteSuffix;
        if (string.IsNullOrEmpty(text))
        {
            if (connection == null)
            {
                connection = DataAdapter?.SelectCommand?.Connection;
                if (connection == null)
                {
                    throw ADP.QuotePrefixNotSet("UnquoteIdentifier");
                }
            }
            text = connection.QuoteChar("UnquoteIdentifier");
            quoteSuffix = text;
        }
        string unquotedString;
        if (!string.IsNullOrEmpty(text) || text != " ")
        {
            ADP.RemoveStringQuotes(text, quoteSuffix, quotedIdentifier, out unquotedString);
        }
        else
        {
            unquotedString = quotedIdentifier;
        }
        ifxTrace?.ApiExit();
        return unquotedString;
    }
}
