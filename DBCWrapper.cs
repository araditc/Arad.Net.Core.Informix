using System;
using System.Runtime.InteropServices;



namespace Arad.Net.Core.Informix;
internal sealed class DBCWrapper
{
    internal InformixConnectionHandle connectionHandle;

    internal nint hdbc = nint.Zero;

    internal int posInPool = -1;

    internal bool _isOpen;

    internal bool isConnectionDead;

    internal bool _isInTransaction;

    internal bool enlisted;

    internal Guid TransactionId
    {
        get
        {
            byte[] array = new byte[100];
            int StringLength = 16;
            if (Interop.Odbc.SQLGetConnectAttrW(connectionHandle, Informix32.SQL_ATTR.TRANSACTION_ID, array, 16, out StringLength) != 0)
            {
                isConnectionDead = true;
                return Guid.Empty;
            }
            return new Guid(array);
        }
    }

    internal DBCWrapper(InformixConnection connection)
    {
        connectionHandle = connection.ConnectionHandle;
        connection._dbcWrapper = this;
        Informix32.RetCode retCode = connectionHandle.Connect(connection.ConnectionString);
        if (retCode != 0)
        {
            Exception ex = connection.HandleErrorNoThrow(connectionHandle, retCode);
            if (ex != null)
            {
                throw ex;
            }
        }
        else
        {
            _isOpen = true;
            retCode = Interop.Odbc.SQLSetConnectAttrW(connectionHandle, Informix32.SQL_ATTR.LOCALIZE_DECIMALS, 1, -6);
            if (retCode != 0)
            {
                connection.HandleError(connectionHandle, retCode);
            }
            retCode = Interop.Odbc.SQLSetConnectAttrW(connectionHandle, Informix32.SQL_ATTR.CALL_FROM_DOTNET, 1, -6);
            if (retCode != 0)
            {
                connection.HandleError(connectionHandle, retCode);
            }
            if (connection.connSettings.leaveTrailingSpaces)
            {
                retCode = Interop.Odbc.SQLSetConnectAttrW(connectionHandle, Informix32.SQL_ATTR.LEAVE_TRAILING_SPACES, 1, -6);
                if (retCode != 0)
                {
                    connection.HandleError(connectionHandle, retCode);
                }
            }
        }
        GC.KeepAlive(this);
    }

    internal Informix32.RetCode Close()
    {
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        if (!isConnectionDead)
        {
            retCode = connectionHandle.Disconnect();
            if (retCode != 0)
            {
                _ = 1;
            }
            try
            {
                connectionHandle.FreeConnectHandle();
            }
            catch (Exception)
            {
            }
        }
        hdbc = nint.Zero;
        _isOpen = false;
        CloseAndRelease();
        return retCode;
    }

    ~DBCWrapper()
    {
        if (_isInTransaction)
        {
            RollbackDeadTransaction();
        }
        CloseAndRelease();
    }

    internal void EnlistAsRequired(InformixConnection connection)
    {
        if (!connection.connSettingsAtOpen.enlist)
        {
            return;
        }
        nint zero = nint.Zero;
        try
        {
            zero = nint.Zero;
            Informix32.RetCode retCode = Interop.Odbc.SQLSetConnectAttrW(connectionHandle, Informix32.SQL_ATTR.ENLIST_IN_DTC, zero, 0);
            if (retCode != 0)
            {
                Exception ex = connection.HandleErrorNoThrow(connectionHandle, retCode);
                _ = 1;
                if (ex != null)
                {
                    throw ex;
                }
            }
        }
        finally
        {
            if (zero != nint.Zero)
            {
                Marshal.Release(zero);
            }
        }
        enlisted = true;
        _isInTransaction = true;
    }

    public override int GetHashCode()
    {
        return hdbc.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        if (obj == null || typeof(DBCWrapper) != obj.GetType())
        {
            return false;
        }
        return hdbc.Equals(((DBCWrapper)obj).hdbc);
    }

    internal void RollbackDeadTransaction()
    {
        Informix32.RetCode retCode = connectionHandle.CompleteTransaction(1);
        _isInTransaction = false;
        GC.KeepAlive(this);
    }

    internal Informix32.RETCODE CloseAndRelease()
    {
        Informix32.RETCODE result = Informix32.RETCODE.SUCCESS;
        if (hdbc != nint.Zero)
        {
            GC.KeepAlive(this);
        }
        return result;
    }
}
