
namespace Arad.Net.Core.Informix;

internal sealed class OdbcEnvironmentHandle : OdbcHandle
{
    internal OdbcEnvironmentHandle()
        : base(Informix32.SQL_HANDLE.ENV, null)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSetEnvAttr(this, Informix32.SQL_ATTR.ODBC_VERSION, Informix32.SQL_OV_ODBC3, Informix32.SQL_IS.INTEGER);
    }

    internal bool FreeEnvHandle()
    {
        if (Interop.Odbc.SQLFreeHandle(Informix32.SQL_HANDLE.ENV, handle) != 0)
        {
            return false;
        }
        return true;
    }
}
