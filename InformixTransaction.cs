using System;
using System.Data;
using System.Data.Common;
using Arad.Net.Core.Informix.System.Data.Common;


namespace Arad.Net.Core.Informix;
public sealed class InformixTransaction : DbTransaction
{
    private InformixConnection _connection;

    private IsolationLevel _isolevel = IsolationLevel.Unspecified;

    private InformixConnectionHandle _handle;

    public new InformixConnection Connection
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _connection;
        }
    }

    protected override DbConnection DbConnection => Connection;

    public override IsolationLevel IsolationLevel
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            InformixConnection connection = _connection;
            if (connection == null)
            {
                throw ADP.TransactionZombied(this);
            }
            if (IsolationLevel.Unspecified == _isolevel)
            {
                int connectAttr = connection.GetConnectAttr(Informix32.SQL_ATTR.TXN_ISOLATION, Informix32.HANDLER.THROW);
                switch ((Informix32.SQL_TRANSACTION)connectAttr)
                {
                    case Informix32.SQL_TRANSACTION.READ_UNCOMMITTED:
                        _isolevel = IsolationLevel.ReadUncommitted;
                        break;
                    case Informix32.SQL_TRANSACTION.READ_COMMITTED:
                        _isolevel = IsolationLevel.ReadCommitted;
                        break;
                    case Informix32.SQL_TRANSACTION.REPEATABLE_READ:
                        _isolevel = IsolationLevel.RepeatableRead;
                        break;
                    case Informix32.SQL_TRANSACTION.SERIALIZABLE:
                        _isolevel = IsolationLevel.Serializable;
                        break;
                    case Informix32.SQL_TRANSACTION.SNAPSHOT:
                        _isolevel = IsolationLevel.Snapshot;
                        break;
                    default:
                        throw ODBC.NoMappingForSqlTransactionLevel(connectAttr);
                }
            }
            ifxTrace?.ApiExit();
            return _isolevel;
        }
    }

    internal InformixTransaction(InformixConnection connection, IsolationLevel isolevel, InformixConnectionHandle handle)
    {
        _connection = connection;
        _isolevel = isolevel;
        _handle = handle;
    }

    public override void Commit()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixConnection connection = _connection;
        if (connection == null)
        {
            throw ADP.TransactionZombied(this);
        }
        connection.CheckState("CommitTransaction");
        if (_handle == null)
        {
            throw ODBC.NotInTransaction();
        }
        Informix32.RetCode retCode = _handle.CompleteTransaction(0);
        if (retCode == Informix32.RetCode.ERROR)
        {
            connection.HandleError(_handle, retCode);
        }
        connection.LocalTransaction = null;
        _connection = null;
        _handle = null;
        ifxTrace?.ApiExit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InformixConnectionHandle handle = _handle;
            _handle = null;
            if (handle != null)
            {
                try
                {
                    Informix32.RetCode retCode = handle.CompleteTransaction(1);
                    if (retCode == Informix32.RetCode.ERROR && _connection != null)
                    {
                        Exception e = _connection.HandleErrorNoThrow(handle, retCode);
                        ADP.TraceExceptionWithoutRethrow(e);
                    }
                }
                catch (Exception e2)
                {
                    if (!ADP.IsCatchableExceptionType(e2))
                    {
                        throw;
                    }
                }
            }
            if (_connection != null && _connection.IsOpen)
            {
                _connection.LocalTransaction = null;
            }
            _connection = null;
            _isolevel = IsolationLevel.Unspecified;
        }
        base.Dispose(disposing);
    }

    public override void Rollback()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixConnection connection = _connection;
        if (connection == null)
        {
            throw ADP.TransactionZombied(this);
        }
        connection.CheckState("RollbackTransaction");
        if (_handle == null)
        {
            throw ODBC.NotInTransaction();
        }
        Informix32.RetCode retCode = _handle.CompleteTransaction(1);
        if (retCode == Informix32.RetCode.ERROR)
        {
            connection.HandleError(_handle, retCode);
        }
        connection.LocalTransaction = null;
        _connection = null;
        _handle = null;
        ifxTrace?.ApiExit();
    }
}
