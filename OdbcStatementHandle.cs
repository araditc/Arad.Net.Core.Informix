using Arad.Net.Core.Informix.System.Data.Common;
using System.Runtime.InteropServices;



namespace Arad.Net.Core.Informix;
internal sealed class OdbcStatementHandle : OdbcHandle
{
    internal OdbcStatementHandle(InformixConnectionHandle connectionHandle)
        : base(Informix32.SQL_HANDLE.STMT, connectionHandle)
    {
    }

    internal Informix32.RetCode BindColumn2(int columnNumber, Informix32.SQL_C targetType, HandleRef buffer, nint length, nint srLen_or_Ind)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLBindCol(this, checked((ushort)columnNumber), targetType, buffer, length, srLen_or_Ind);
        ODBC.TraceODBC(3, "SQLBindCol", retCode);
        return retCode;
    }

    internal Informix32.RetCode BindColumn3(int columnNumber, Informix32.SQL_C targetType, nint srLen_or_Ind)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLBindCol(this, checked((ushort)columnNumber), targetType, ADP.PtrZero, ADP.PtrZero, srLen_or_Ind);
        ODBC.TraceODBC(3, "SQLBindCol", retCode);
        return retCode;
    }

    internal Informix32.RetCode BindParameter(short ordinal, short parameterDirection, Informix32.SQL_C sqlctype, Informix32.SQL_TYPE sqltype, nint cchSize, nint scale, HandleRef buffer, nint bufferLength, HandleRef intbuffer)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLBindParameter(this, checked((ushort)ordinal), parameterDirection, sqlctype, (short)sqltype, cchSize, scale, buffer, bufferLength, intbuffer);
        ODBC.TraceODBC(3, "SQLBindParameter", retCode);
        return retCode;
    }

    internal Informix32.RetCode Cancel()
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLCancel(this);
        ODBC.TraceODBC(3, "SQLCancel", retCode);
        return retCode;
    }

    internal Informix32.RetCode CloseCursor()
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLCloseCursor(this);
        ODBC.TraceODBC(3, "SQLCloseCursor", retCode);
        return retCode;
    }

    internal Informix32.RetCode ColumnAttribute(int columnNumber, short fieldIdentifier, CNativeBuffer characterAttribute, out short stringLength, out SQLLEN numericAttribute)
    {
        nint NumericAttribute;
        Informix32.RetCode retCode = Interop.Odbc.SQLColAttributeW(this, checked((short)columnNumber), fieldIdentifier, characterAttribute, (short)(characterAttribute.Length > 32767 ? 32767 : characterAttribute.Length), out stringLength, out NumericAttribute);
        numericAttribute = new SQLLEN(NumericAttribute);
        ODBC.TraceODBC(3, "SQLColAttributeW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Columns(string tableCatalog, string tableSchema, string tableName, string columnName)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLColumnsW(this, tableCatalog, ODBC.ShortStringLength(tableCatalog), tableSchema, ODBC.ShortStringLength(tableSchema), tableName, ODBC.ShortStringLength(tableName), columnName, ODBC.ShortStringLength(columnName));
        ODBC.TraceODBC(3, "SQLColumnsW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Execute()
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLExecute(this);
        ODBC.TraceODBC(3, "SQLExecute", retCode);
        return retCode;
    }

    internal Informix32.RetCode ExecuteDirect(string commandText)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLExecDirectW(this, commandText, -3);
        ODBC.TraceODBC(3, "SQLExecDirectW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Fetch()
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLFetch(this);
        ODBC.TraceODBC(3, "SQLFetch", retCode);
        return retCode;
    }

    internal Informix32.RetCode FreeStatement(Informix32.STMT stmt)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLFreeStmt(this, stmt);
        ODBC.TraceODBC(3, "SQLFreeStmt", retCode);
        return retCode;
    }

    internal Informix32.RetCode GetData(int index, Informix32.SQL_C sqlctype, CNativeBuffer buffer, int cb, out nint cbActual)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLGetData(this, checked((ushort)index), sqlctype, buffer, new nint(cb), out cbActual);
        ODBC.TraceODBC(3, "SQLGetData", retCode);
        return retCode;
    }

    internal Informix32.RetCode GetStatementAttribute(Informix32.SQL_ATTR attribute, out nint value, out int stringLength)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLGetStmtAttrW(this, attribute, out value, ADP.PtrSize, out stringLength);
        ODBC.TraceODBC(3, "SQLGetStmtAttrW", retCode);
        return retCode;
    }

    internal Informix32.RetCode GetTypeInfo(short fSqlType)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLGetTypeInfo(this, fSqlType);
        ODBC.TraceODBC(3, "SQLGetTypeInfo", retCode);
        return retCode;
    }

    internal Informix32.RetCode MoreResults()
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLMoreResults(this);
        ODBC.TraceODBC(3, "SQLMoreResults", retCode);
        return retCode;
    }

    internal Informix32.RetCode NumberOfResultColumns(out short columnsAffected)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLNumResultCols(this, out columnsAffected);
        ODBC.TraceODBC(3, "SQLNumResultCols", retCode);
        return retCode;
    }

    internal Informix32.RetCode Prepare(string commandText)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLPrepareW(this, commandText, -3);
        ODBC.TraceODBC(3, "SQLPrepareW", retCode);
        return retCode;
    }

    internal Informix32.RetCode PrimaryKeys(string catalogName, string schemaName, string tableName)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLPrimaryKeysW(this, catalogName, ODBC.ShortStringLength(catalogName), schemaName, ODBC.ShortStringLength(schemaName), tableName, ODBC.ShortStringLength(tableName));
        ODBC.TraceODBC(3, "SQLPrimaryKeysW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Procedures(string procedureCatalog, string procedureSchema, string procedureName)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLProceduresW(this, procedureCatalog, ODBC.ShortStringLength(procedureCatalog), procedureSchema, ODBC.ShortStringLength(procedureSchema), procedureName, ODBC.ShortStringLength(procedureName));
        ODBC.TraceODBC(3, "SQLProceduresW", retCode);
        return retCode;
    }

    internal Informix32.RetCode ProcedureColumns(string procedureCatalog, string procedureSchema, string procedureName, string columnName)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLProcedureColumnsW(this, procedureCatalog, ODBC.ShortStringLength(procedureCatalog), procedureSchema, ODBC.ShortStringLength(procedureSchema), procedureName, ODBC.ShortStringLength(procedureName), columnName, ODBC.ShortStringLength(columnName));
        ODBC.TraceODBC(3, "SQLProcedureColumnsW", retCode);
        return retCode;
    }

    internal Informix32.RetCode RowCount(out SQLLEN rowCount)
    {
        nint RowCount;
        Informix32.RetCode retCode = Interop.Odbc.SQLRowCount(this, out RowCount);
        rowCount = new SQLLEN(RowCount);
        ODBC.TraceODBC(3, "SQLRowCount", retCode);
        return retCode;
    }

    internal Informix32.RetCode SetStatementAttribute(Informix32.SQL_ATTR attribute, nint value, Informix32.SQL_IS stringLength)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSetStmtAttrW(this, (int)attribute, value, (int)stringLength);
        ODBC.TraceODBC(3, "SQLSetStmtAttrW", retCode);
        return retCode;
    }

    internal Informix32.RetCode SpecialColumns(string quotedTable)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSpecialColumnsW(this, Informix32.SQL_SPECIALCOLS.ROWVER, null, 0, null, 0, quotedTable, ODBC.ShortStringLength(quotedTable), Informix32.SQL_SCOPE.SESSION, Informix32.SQL_NULLABILITY.NO_NULLS);
        ODBC.TraceODBC(3, "SQLSpecialColumnsW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Statistics(string tableCatalog, string tableSchema, string tableName, short unique, short accuracy)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLStatisticsW(this, tableCatalog, ODBC.ShortStringLength(tableCatalog), tableSchema, ODBC.ShortStringLength(tableSchema), tableName, ODBC.ShortStringLength(tableName), unique, accuracy);
        ODBC.TraceODBC(3, "SQLStatisticsW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Statistics(string tableName)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLStatisticsW(this, null, 0, null, 0, tableName, ODBC.ShortStringLength(tableName), 0, 1);
        ODBC.TraceODBC(3, "SQLStatisticsW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Tables(string tableCatalog, string tableSchema, string tableName, string tableType)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLTablesW(this, tableCatalog, ODBC.ShortStringLength(tableCatalog), tableSchema, ODBC.ShortStringLength(tableSchema), tableName, ODBC.ShortStringLength(tableName), tableType, ODBC.ShortStringLength(tableType));
        ODBC.TraceODBC(3, "SQLTablesW", retCode);
        return retCode;
    }
}
