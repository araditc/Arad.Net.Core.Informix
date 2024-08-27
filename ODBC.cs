using System;
using System.Data;
using Arad.Net.Core.Informix.System;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Globalization;



namespace Arad.Net.Core.Informix;
internal static class ODBC
{
    internal const string Pwd = "pwd";

    internal static Exception ConnectionClosed()
    {
        return ADP.InvalidOperation(SR.GetString(SR.Odbc_ConnectionClosed));
    }

    internal static Exception OpenConnectionNoOwner()
    {
        return ADP.InvalidOperation(SR.GetString(SR.Odbc_OpenConnectionNoOwner));
    }

    internal static Exception UnknownSQLType(Informix32.SQL_TYPE sqltype)
    {
        return ADP.Argument(SR.GetString(SR.Odbc_UnknownSQLType, sqltype.ToString()));
    }

    internal static Exception ConnectionStringTooLong()
    {
        return ADP.Argument(SR.GetString(SR.OdbcConnection_ConnectionStringTooLong, 1024));
    }

    internal static ArgumentException GetSchemaRestrictionRequired()
    {
        return ADP.Argument(SR.GetString(SR.ODBC_GetSchemaRestrictionRequired));
    }

    internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, int value)
    {
        return ADP.ArgumentOutOfRange(SR.GetString(SR.ODBC_NotSupportedEnumerationValue, type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
    }

    internal static ArgumentOutOfRangeException NotSupportedCommandType(CommandType value)
    {
        return NotSupportedEnumerationValue(typeof(CommandType), (int)value);
    }

    internal static ArgumentOutOfRangeException NotSupportedIsolationLevel(IsolationLevel value)
    {
        return NotSupportedEnumerationValue(typeof(IsolationLevel), (int)value);
    }

    internal static InvalidOperationException NoMappingForSqlTransactionLevel(int value)
    {
        return ADP.DataAdapter(SR.GetString(SR.Odbc_NoMappingForSqlTransactionLevel, value.ToString(CultureInfo.InvariantCulture)));
    }

    internal static Exception NegativeArgument()
    {
        return ADP.Argument(SR.GetString(SR.Odbc_NegativeArgument));
    }

    internal static Exception CantSetPropertyOnOpenConnection()
    {
        return ADP.InvalidOperation(SR.GetString(SR.Odbc_CantSetPropertyOnOpenConnection));
    }

    internal static Exception CantEnableConnectionpooling(Informix32.RetCode retcode)
    {
        return ADP.DataAdapter(SR.GetString(SR.Odbc_CantEnableConnectionpooling, Informix32.RetcodeToString(retcode)));
    }

    internal static Exception CantAllocateEnvironmentHandle(Informix32.RetCode retcode)
    {
        return ADP.DataAdapter(SR.GetString(SR.Odbc_CantAllocateEnvironmentHandle, Informix32.RetcodeToString(retcode)));
    }

    internal static Exception FailedToGetDescriptorHandle(Informix32.RetCode retcode)
    {
        return ADP.DataAdapter(SR.GetString(SR.Odbc_FailedToGetDescriptorHandle, Informix32.RetcodeToString(retcode)));
    }

    internal static Exception NotInTransaction()
    {
        return ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
    }

    internal static Exception UnknownOdbcType(InformixType odbctype)
    {
        return ADP.InvalidEnumerationValue(typeof(InformixType), (int)odbctype);
    }

    internal static void TraceODBC(int level, string method, Informix32.RetCode retcode)
    {
    }

    internal static short ShortStringLength(string inputString)
    {
        return checked((short)ADP.StringLength(inputString));
    }
}
