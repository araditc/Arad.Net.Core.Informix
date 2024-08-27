using Arad.Net.Core.Informix.System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;


namespace Arad.Net.Core.Informix;

internal abstract class OdbcHandle : SafeHandle
{
    private Informix32.SQL_HANDLE _handleType;

    private OdbcHandle _parentHandle;

    private bool IsConnPool = true;

    internal Informix32.SQL_HANDLE HandleType => _handleType;

    public override bool IsInvalid => nint.Zero == handle;

    internal bool IsConnPoolEnabled
    {
        get
        {
            return IsConnPool;
        }
        set
        {
            IsConnPool = value;
        }
    }

    internal OdbcHandle(Informix32.SQL_HANDLE handleType, OdbcHandle parentHandle)
        : base(nint.Zero, ownsHandle: true)
    {
        _handleType = handleType;
        bool success = false;
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            switch (handleType)
            {
                case Informix32.SQL_HANDLE.ENV:
                    retCode = Interop.Odbc.SQLAllocHandle(handleType, nint.Zero, out handle);
                    break;
                case Informix32.SQL_HANDLE.DBC:
                case Informix32.SQL_HANDLE.STMT:
                    parentHandle.DangerousAddRef(ref success);
                    retCode = Interop.Odbc.SQLAllocHandle(handleType, parentHandle, out handle);
                    if (retCode == Informix32.RetCode.SUCCESS && handleType == Informix32.SQL_HANDLE.DBC)
                    {
                        retCode = Interop.Odbc.SQLSetConnectAttrW(handle, Informix32.SQL_ATTR.CALL_FROM_DOTNET, 1, -6);
                        retCode = Interop.Odbc.SQLSetConnectAttrW(handle, Informix32.SQL_ATTR.ODBC_TYPES_ONLY, 1, -6);
                    }
                    break;
            }
        }
        finally
        {
            if (success && (uint)(handleType - 2) <= 1u)
            {
                if (nint.Zero != handle)
                {
                    _parentHandle = parentHandle;
                }
                else
                {
                    parentHandle.DangerousRelease();
                }
            }
        }
        if (ADP.PtrZero == handle || retCode != 0)
        {
            throw ODBC.CantAllocateEnvironmentHandle(retCode);
        }
    }

    internal OdbcHandle(OdbcStatementHandle parentHandle, Informix32.SQL_ATTR attribute)
        : base(nint.Zero, ownsHandle: true)
    {
        _handleType = Informix32.SQL_HANDLE.DESC;
        Informix32.RetCode retcode = Informix32.RetCode.SUCCESS;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            parentHandle.DangerousAddRef(ref success);
            retcode = parentHandle.GetStatementAttribute(attribute, out handle, out var _);
        }
        finally
        {
            if (success)
            {
                if (nint.Zero != handle)
                {
                    _parentHandle = parentHandle;
                }
                else
                {
                    parentHandle.DangerousRelease();
                }
            }
        }
        if (ADP.PtrZero == handle)
        {
            throw ODBC.FailedToGetDescriptorHandle(retcode);
        }
    }

    internal bool FreeConnectHandle()
    {
        if (Interop.Odbc.SQLFreeHandle(Informix32.SQL_HANDLE.DBC, handle) != 0)
        {
            return false;
        }
        return true;
    }

    protected override bool ReleaseHandle()
    {
        nint intPtr = handle;
        handle = nint.Zero;
        if (nint.Zero != intPtr)
        {
            Informix32.SQL_HANDLE handleType = HandleType;
            Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
            switch (handleType)
            {
                case Informix32.SQL_HANDLE.DBC:
                    retCode = Interop.Odbc.SQLFreeHandle(handleType, intPtr);
                    break;
                case Informix32.SQL_HANDLE.ENV:
                case Informix32.SQL_HANDLE.STMT:
                    retCode = Interop.Odbc.SQLFreeHandle(handleType, intPtr);
                    break;
            }
        }
        OdbcHandle parentHandle = _parentHandle;
        _parentHandle = null;
        if (parentHandle != null)
        {
            parentHandle.DangerousRelease();
            parentHandle = null;
        }
        return true;
    }

    internal Informix32.RetCode GetDiagnosticField(out string sqlState)
    {
        StringBuilder stringBuilder = new StringBuilder(6);
        short StringLength;
        Informix32.RetCode retCode = Interop.Odbc.SQLGetDiagFieldW(HandleType, this, 1, 4, stringBuilder, checked((short)(2 * stringBuilder.Capacity)), out StringLength);
        ODBC.TraceODBC(3, "SQLGetDiagFieldW", retCode);
        if (retCode == Informix32.RetCode.SUCCESS || retCode == Informix32.RetCode.SUCCESS_WITH_INFO)
        {
            sqlState = stringBuilder.ToString();
        }
        else
        {
            sqlState = "";
        }
        return retCode;
    }

    internal Informix32.RetCode GetDiagnosticRecord(short record, out string sqlState, StringBuilder message, out int nativeError, out short cchActual)
    {
        StringBuilder stringBuilder = new StringBuilder(5);
        Informix32.RetCode retCode = Interop.Odbc.SQLGetDiagRecW(HandleType, this, record, stringBuilder, out nativeError, message, checked((short)message.Capacity), out cchActual);
        ODBC.TraceODBC(3, "SQLGetDiagRecW", retCode);
        if (retCode == Informix32.RetCode.SUCCESS || retCode == Informix32.RetCode.SUCCESS_WITH_INFO)
        {
            sqlState = stringBuilder.ToString();
        }
        else
        {
            sqlState = "";
        }
        return retCode;
    }
}
