using System;
using System.Data;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Transactions;

using IsolationLevel = System.Data.IsolationLevel;


namespace Arad.Net.Core.Informix;
internal sealed class InformixConnectionHandle : OdbcHandle
{
    private enum HandleState
    {
        Allocated,
        Connected,
        Transacted,
        TransactionInProgress
    }

    private HandleState _handleState;

    internal InformixConnectionHandle(OdbcEnvironmentHandle environmentHandle)
        : base(Informix32.SQL_HANDLE.DBC, environmentHandle)
    {
    }

    internal InformixConnectionHandle(InformixConnection connection, InformixConnectionString constr, OdbcEnvironmentHandle environmentHandle)
        : base(Informix32.SQL_HANDLE.DBC, environmentHandle)
    {
        if (connection == null)
        {
            throw ADP.ArgumentNull("connection");
        }
        if (constr == null)
        {
            throw ADP.ArgumentNull("constr");
        }
        int connTimeOut = connection.connSettings.connTimeOut;
        Informix32.RetCode retCode = SetConnectionAttribute2(Informix32.SQL_ATTR.LOGIN_TIMEOUT, connTimeOut, -5);
        string connectionString = connection.parsedConnString.ToString();
        retCode = Connect(connectionString);
        connection.HandleError(this, retCode);
        if (retCode == Informix32.RetCode.SUCCESS || retCode == Informix32.RetCode.SUCCESS_WITH_INFO)
        {
            connection.state = ConnectionState.Open;
            IsConnPoolEnabled = connection.connSettings.pooling;
        }
    }

    private Informix32.RetCode AutoCommitOff()
    {
        RuntimeHelpers.PrepareConstrainedRegions();
        Informix32.RetCode retCode;
        try
        {
        }
        finally
        {
            retCode = Interop.Odbc.SQLSetConnectAttrW(this, Informix32.SQL_ATTR.AUTOCOMMIT, Informix32.SQL_AUTOCOMMIT_OFF, -5);
            if ((uint)retCode <= 1u)
            {
                _handleState = HandleState.Transacted;
            }
        }
        ODBC.TraceODBC(3, "SQLSetConnectAttrW", retCode);
        return retCode;
    }

    internal Informix32.RetCode BeginTransaction(ref IsolationLevel isolevel)
    {
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        if (IsolationLevel.Unspecified != isolevel)
        {
            Informix32.SQL_TRANSACTION sQL_TRANSACTION;
            Informix32.SQL_ATTR attribute;
            switch (isolevel)
            {
                case IsolationLevel.ReadUncommitted:
                    sQL_TRANSACTION = Informix32.SQL_TRANSACTION.READ_UNCOMMITTED;
                    attribute = Informix32.SQL_ATTR.TXN_ISOLATION;
                    break;
                case IsolationLevel.ReadCommitted:
                    sQL_TRANSACTION = Informix32.SQL_TRANSACTION.READ_COMMITTED;
                    attribute = Informix32.SQL_ATTR.TXN_ISOLATION;
                    break;
                case IsolationLevel.RepeatableRead:
                    sQL_TRANSACTION = Informix32.SQL_TRANSACTION.REPEATABLE_READ;
                    attribute = Informix32.SQL_ATTR.TXN_ISOLATION;
                    break;
                case IsolationLevel.Serializable:
                    sQL_TRANSACTION = Informix32.SQL_TRANSACTION.SERIALIZABLE;
                    attribute = Informix32.SQL_ATTR.TXN_ISOLATION;
                    break;
                case IsolationLevel.Snapshot:
                    sQL_TRANSACTION = Informix32.SQL_TRANSACTION.SNAPSHOT;
                    attribute = Informix32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION;
                    break;
                case IsolationLevel.Chaos:
                    throw ODBC.NotSupportedIsolationLevel(isolevel);
                default:
                    throw ADP.InvalidIsolationLevel(isolevel);
            }
            retCode = SetConnectionAttribute2(attribute, (int)sQL_TRANSACTION, -6);
            if (Informix32.RetCode.SUCCESS_WITH_INFO == retCode)
            {
                isolevel = IsolationLevel.Unspecified;
            }
        }
        if ((uint)retCode <= 1u)
        {
            retCode = AutoCommitOff();
            _handleState = HandleState.TransactionInProgress;
        }
        return retCode;
    }

    internal Informix32.RetCode CompleteTransaction(short transactionOperation)
    {
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
            DangerousAddRef(ref success);
            return CompleteTransaction(transactionOperation, handle);
        }
        finally
        {
            if (success)
            {
                DangerousRelease();
            }
        }
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    private Informix32.RetCode CompleteTransaction(short transactionOperation, nint handle)
    {
        Informix32.RetCode retCode = Informix32.RetCode.SUCCESS;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
            if (HandleState.TransactionInProgress == _handleState)
            {
                retCode = Interop.Odbc.SQLEndTran(HandleType, handle, transactionOperation);
                if (retCode == Informix32.RetCode.SUCCESS || Informix32.RetCode.SUCCESS_WITH_INFO == retCode)
                {
                    _handleState = HandleState.Transacted;
                }
            }
            if (HandleState.Transacted == _handleState)
            {
                retCode = Interop.Odbc.SQLSetConnectAttrW(handle, Informix32.SQL_ATTR.AUTOCOMMIT, Informix32.SQL_AUTOCOMMIT_ON, -5);
                _handleState = HandleState.Connected;
            }
        }
        return retCode;
    }

    internal Informix32.RetCode Connect(string connectionString)
    {
        RuntimeHelpers.PrepareConstrainedRegions();
        Informix32.RetCode retCode;
        try
        {
        }
        finally
        {
            retCode = Interop.Odbc.SQLDriverConnectW(this, ADP.PtrZero, connectionString, -3, ADP.PtrZero, 0, out var _, 0);
            if ((uint)retCode <= 1u)
            {
                _handleState = HandleState.Connected;
            }
        }
        ODBC.TraceODBC(3, "SQLDriverConnectW", retCode);
        return retCode;
    }

    internal Informix32.RetCode Disconnect()
    {
        Informix32.RetCode result = CompleteTransaction(1, handle);
        if (HandleState.Connected == _handleState || HandleState.TransactionInProgress == _handleState)
        {
            result = Interop.Odbc.SQLDisconnect(handle);
            _handleState = HandleState.Allocated;
        }
        return result;
    }

    protected override bool ReleaseHandle()
    {
        Informix32.RetCode retCode = CompleteTransaction(1, handle);
        if (HandleState.Connected == _handleState || HandleState.TransactionInProgress == _handleState)
        {
            retCode = Interop.Odbc.SQLDisconnect(handle);
            _handleState = HandleState.Allocated;
        }
        return base.ReleaseHandle();
    }

    internal Informix32.RetCode GetConnectionAttribute(Informix32.SQL_ATTR attribute, byte[] buffer, out int cbActual)
    {
        return Interop.Odbc.SQLGetConnectAttrW(this, attribute, buffer, buffer.Length, out cbActual);
    }

    internal Informix32.RetCode GetFunctions(Informix32.SQL_API fFunction, out short fExists)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLGetFunctions(this, fFunction, out fExists);
        ODBC.TraceODBC(3, "SQLGetFunctions", retCode);
        return retCode;
    }

    internal Informix32.RetCode GetInfo2(Informix32.SQL_INFO info, byte[] buffer, out short cbActual)
    {
        return Interop.Odbc.SQLGetInfoW(this, info, buffer, checked((short)buffer.Length), out cbActual);
    }

    internal Informix32.RetCode GetInfo1(Informix32.SQL_INFO info, byte[] buffer)
    {
        return Interop.Odbc.SQLGetInfoW(this, info, buffer, checked((short)buffer.Length), ADP.PtrZero);
    }

    internal Informix32.RetCode SetConnectionAttribute2(Informix32.SQL_ATTR attribute, nint value, int length)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSetConnectAttrW(this, attribute, value, length);
        ODBC.TraceODBC(3, "SQLSetConnectAttrW", retCode);
        return retCode;
    }

    internal Informix32.RetCode SetConnectionAttribute3(Informix32.SQL_ATTR attribute, string buffer, int length)
    {
        return Interop.Odbc.SQLSetConnectAttrW(this, attribute, buffer, length);
    }

    internal Informix32.RetCode SetConnectionAttribute4(Informix32.SQL_ATTR attribute, IDtcTransaction transaction, int length)
    {
        Informix32.RetCode retCode = Interop.Odbc.SQLSetConnectAttrW(this, attribute, transaction, length);
        ODBC.TraceODBC(3, "SQLSetConnectAttrW", retCode);
        return retCode;
    }
}
