using System;
using System.Runtime.InteropServices;


namespace Arad.Net.Core.Informix;

internal sealed class OdbcDescriptorHandle : OdbcHandle
{
    internal OdbcDescriptorHandle(OdbcStatementHandle statementHandle, Informix32.SQL_ATTR attribute)
        : base(statementHandle, attribute)
    {
    }

    internal Informix32.RetCode GetDescriptionField(int i, Informix32.SQL_DESC attribute, CNativeBuffer buffer, out int numericAttribute)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLGetDescFieldW(this, checked((short)i), attribute, buffer, (short)(buffer.Length > 32767 ? 32767 : buffer.Length), out numericAttribute);
        ODBC.TraceODBC(3, "SQLGetDescFieldW", retCode);
        return retCode;
    }

    internal Informix32.RetCode SetDescriptionField1(short ordinal, Informix32.SQL_DESC type, nint value)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSetDescFieldW(this, ordinal, type, value, 0);
        ODBC.TraceODBC(3, "SQLSetDescFieldW", retCode);
        return retCode;
    }

    internal Informix32.RetCode SetDescriptionField2(short ordinal, Informix32.SQL_DESC type, HandleRef value)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSetDescFieldW(this, ordinal, type, value, 0);
        ODBC.TraceODBC(3, "SQLSetDescFieldW", retCode);
        return retCode;
    }
}
