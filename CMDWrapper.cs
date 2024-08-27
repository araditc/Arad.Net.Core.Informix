using System;
using Arad.Net.Core.Informix.System.Data.Common;

namespace Arad.Net.Core.Informix;
internal sealed class CMDWrapper
{
    private OdbcStatementHandle _stmt;

    private OdbcStatementHandle _keyinfostmt;

    internal OdbcDescriptorHandle _hdesc;

    internal CNativeBuffer _nativeParameterBuffer;

    internal CNativeBuffer _dataReaderBuf;

    private readonly InformixConnection _connection;

    private bool _canceling;

    internal bool _hasBoundColumns;

    internal bool _ssKeyInfoModeOn;

    internal bool _ssKeyInfoModeOff;

    internal bool Canceling
    {
        get
        {
            return _canceling;
        }
        set
        {
            _canceling = value;
        }
    }

    internal InformixConnection Connection => _connection;

    internal bool HasBoundColumns
    {
        set
        {
            _hasBoundColumns = value;
        }
    }

    internal OdbcStatementHandle StatementHandle => _stmt;

    internal OdbcStatementHandle KeyInfoStatement => _keyinfostmt;

    internal CMDWrapper(InformixConnection connection)
    {
        _connection = connection;
    }

    internal void CreateKeyInfoStatementHandle()
    {
        DisposeKeyInfoStatementHandle();
        _keyinfostmt = _connection.CreateStatementHandle();
    }

    internal void CreateStatementHandle()
    {
        DisposeStatementHandle();
        _stmt = _connection.CreateStatementHandle();
    }

    internal void Dispose()
    {
        if (_dataReaderBuf != null)
        {
            _dataReaderBuf.Dispose();
            _dataReaderBuf = null;
        }
        DisposeStatementHandle();
        CNativeBuffer nativeParameterBuffer = _nativeParameterBuffer;
        _nativeParameterBuffer = null;
        if (nativeParameterBuffer != null)
        {
            nativeParameterBuffer.FreeBuffer();
            nativeParameterBuffer.Dispose();
        }
        _ssKeyInfoModeOn = false;
        _ssKeyInfoModeOff = false;
    }

    private void DisposeDescriptorHandle()
    {
        OdbcDescriptorHandle hdesc = _hdesc;
        if (hdesc != null)
        {
            _hdesc = null;
            hdesc.Dispose();
        }
    }

    internal void DisposeStatementHandle()
    {
        DisposeKeyInfoStatementHandle();
        DisposeDescriptorHandle();
        OdbcStatementHandle stmt = _stmt;
        if (stmt != null)
        {
            _stmt = null;
            stmt.Dispose();
        }
    }

    internal void DisposeKeyInfoStatementHandle()
    {
        OdbcStatementHandle keyinfostmt = _keyinfostmt;
        if (keyinfostmt != null)
        {
            _keyinfostmt = null;
            keyinfostmt.Dispose();
        }
    }

    internal void FreeStatementHandle(Informix32.STMT stmt)
    {
        DisposeDescriptorHandle();
        OdbcStatementHandle stmt2 = _stmt;
        if (stmt2 == null)
        {
            return;
        }
        try
        {
            Informix32.RetCode retcode = stmt2.FreeStatement(stmt);
            StatementErrorHandler(retcode);
        }
        catch (Exception e)
        {
            if (ADP.IsCatchableExceptionType(e))
            {
                _stmt = null;
                stmt2.Dispose();
            }
            throw;
        }
    }

    internal void FreeKeyInfoStatementHandle(Informix32.STMT stmt)
    {
        OdbcStatementHandle keyinfostmt = _keyinfostmt;
        if (keyinfostmt == null)
        {
            return;
        }
        try
        {
            keyinfostmt.FreeStatement(stmt);
        }
        catch (Exception e)
        {
            if (ADP.IsCatchableExceptionType(e))
            {
                _keyinfostmt = null;
                keyinfostmt.Dispose();
            }
            throw;
        }
    }

    internal OdbcDescriptorHandle GetDescriptorHandle(Informix32.SQL_ATTR attribute)
    {
        OdbcDescriptorHandle hdesc = _hdesc;
        return _hdesc = new OdbcDescriptorHandle(_stmt, attribute);
    }

    internal string GetDiagSqlState()
    {
        _stmt.GetDiagnosticField(out var sqlState);
        return sqlState;
    }

    internal void StatementErrorHandler(Informix32.RetCode retcode)
    {
        if ((uint)retcode <= 1u)
        {
            _connection.HandleErrorNoThrow(_stmt, retcode);
            return;
        }
        throw _connection.HandleErrorNoThrow(_stmt, retcode);
    }

    internal void UnbindStmtColumns()
    {
        if (_hasBoundColumns)
        {
            FreeStatementHandle(Informix32.STMT.UNBIND);
            _hasBoundColumns = false;
        }
    }
}
