using System;
using Arad.Net.Core.Informix.System.Data.Common;
using Arad.Net.Core.Informix.System;
using System.Data.SqlTypes;
using System.IO;
using System.Runtime.InteropServices;


namespace Arad.Net.Core.Informix;

public abstract class InformixSmartLOB : INullable
{
    internal const short LOCATOR_PTR_SIZE = 72;

    internal const short SPEC_PTR_SIZE = 596;

    internal const short STATUS_PTR_SIZE = 644;

    private Informix32.RetCode rc;

    internal InformixCommand command;

    internal OdbcStatementHandle stmt;

    internal InformixConnectionHandle hdbc;

    internal InformixSmartLOBLocator locator;

    internal InformixConnection connection;

    internal int lofd = -1;

    internal bool isUsrLOFDProvided;

    internal int usrProvLOFD = -1;

    internal byte[] lostat;

    internal byte[] lospec;

    internal ParamBufs paramBufs = new ParamBufs();

    internal CNativeBuffer varLenDataBuf1;

    internal CNativeBuffer varLenDataBuf2;

    internal short lobType;

    internal bool isNull;

    internal string tableName;

    internal string colName;

    public bool IsOpen
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            bool result = lofd != -1;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public bool IsNull
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            bool result = false;
            if (isNull)
            {
                result = true;
            }
            else if (locator == null)
            {
                result = true;
            }
            ifxTrace?.ApiExit();
            return result;
        }
    }

    public long EstimatedSize
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specget_estbytes(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (long)paramBufs.dataBuf8_1.MarshalToManaged(0, Informix32.SQL_C.SBIGINT, 0);
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            paramBufs.dataBuf8_1.MarshalToNative(0, value, Informix32.SQL_C.SBIGINT, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
            rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specset_estbytes(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
            ifxTrace?.ApiExit();
        }
    }

    public int ExtentSize
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specget_extsz(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            paramBufs.dataBuf4_1.MarshalToNative(0, value, Informix32.SQL_C.SLONG, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specset_extsz(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
            ifxTrace?.ApiExit();
        }
    }

    public int Flags
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specget_flags(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            paramBufs.dataBuf4_1.MarshalToNative(0, value, Informix32.SQL_C.SLONG, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specset_flags(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
            ifxTrace?.ApiExit();
        }
    }

    public long MaxBytes
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specget_maxbytes(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (long)paramBufs.dataBuf8_1.MarshalToManaged(0, Informix32.SQL_C.SBIGINT, 0);
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            paramBufs.dataBuf8_1.MarshalToNative(0, value, Informix32.SQL_C.SBIGINT, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
            rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specset_maxbytes(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
            ifxTrace?.ApiExit();
        }
    }

    public string SBSpace
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            varLenDataBuf2 = paramBufs.GetVarLenDataBuf2(258);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 258L);
            rgbValue = varLenDataBuf2.PtrOffset(0, varLenDataBuf2.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.WCHAR, 1, 0, 0, rgbValue, varLenDataBuf2.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specget_sbspace(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (string)varLenDataBuf2.MarshalToManaged(0, Informix32.SQL_C.WCHAR, (int)paramBufs.indBuf_2.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0));
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetDefaultLOSpec();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            varLenDataBuf2 = paramBufs.GetVarLenDataBuf2(258);
            varLenDataBuf2.MarshalToNative(0, value, Informix32.SQL_C.WCHAR, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -3L);
            rgbValue = varLenDataBuf2.PtrOffset(0, varLenDataBuf2.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.WCHAR, 1, 0, 0, rgbValue, varLenDataBuf2.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_specset_sbspace(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
            ifxTrace?.ApiExit();
        }
    }

    public long Position
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            long result = internalPosition;
            ifxTrace?.ApiExit();
            return result;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            internalPosition = value;
            ifxTrace?.ApiExit();
        }
    }

    internal long internalPosition
    {
        get
        {
            if (lofd == -1)
            {
                throw ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
            }
            paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
            HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
            rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 2, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_tell(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            return (long)paramBufs.dataBuf8_1.MarshalToManaged(0, Informix32.SQL_C.SBIGINT, 0);
        }
        set
        {
            internalSeek(value, InformixSmartLOBWhence.Begin);
        }
    }

    public int LastAccessTime
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetLoStatusInfo();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(644);
            varLenDataBuf1.MarshalToNative(0, lostat, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 644L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 644, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_stat_atime(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
    }

    public int LastChangeTime
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetLoStatusInfo();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(644);
            varLenDataBuf1.MarshalToNative(0, lostat, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 644L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 644, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_stat_ctime(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
    }

    public int LastModificationTime
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetLoStatusInfo();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(644);
            varLenDataBuf1.MarshalToNative(0, lostat, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 644L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 644, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_stat_mtime_sec(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
    }

    public int ReferenceCount
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            GetLoStatusInfo();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(644);
            varLenDataBuf1.MarshalToNative(0, lostat, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 644L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 644, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_stat_refcnt(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            ifxTrace?.ApiExit();
            return (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
    }

    public long Size
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (isNull)
            {
                throw new InvalidOperationException();
            }
            long result = internalSize;
            ifxTrace?.ApiExit();
            return result;
        }
    }

    internal long internalSize
    {
        get
        {
            GetLoStatusInfo();
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(644);
            varLenDataBuf1.MarshalToNative(0, lostat, Informix32.SQL_C.BINARY, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 644L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 644, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
            rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_stat_size(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            return (long)paramBufs.dataBuf8_1.MarshalToManaged(0, Informix32.SQL_C.SBIGINT, 0);
        }
    }

    internal void InitSmartLOB(InformixSmartLOBLocator _locator, InformixConnection _connection, SmartLOBType _lobType)
    {
        if (_connection == null)
        {
            throw new ArgumentNullException();
        }
        locator = _locator;
        lobType = (short)_lobType;
        connection = _connection;
        hdbc = connection.ConnectionHandle;
        lostat = null;
        lospec = null;
        lofd = -1;
        isNull = false;
        tableName = string.Empty;
        colName = string.Empty;
        command = new InformixCommand();
        command.Connection = connection;
        CMDWrapper statementHandle = command.GetStatementHandle();
        stmt = statementHandle.StatementHandle;
        if (stmt == null)
        {
            return;
        }
        rc = Interop.Odbc.SQLSetStmtAttrW(stmt, 2262, 1, -6);
        if (rc != 0)
        {
            connection.HandleError(hdbc, Informix32.RetCode.ERROR);
        }
        if (connection.connSettings.skipParsing)
        {
            rc = Interop.Odbc.SQLSetStmtAttrW(stmt, 2278, 0, -7);
            if (rc != 0)
            {
                connection.HandleError(hdbc, Informix32.RetCode.ERROR);
            }
        }
    }

    internal void InitSmartLOB(bool _isNull)
    {
        isNull = _isNull;
    }

    internal void DeinitSmartLOB()
    {
        try
        {
            if (lofd == -1)
            {
                internalClose();
            }
        }
        catch (Exception)
        {
        }
    }

    internal void CloseSTMTHandle()
    {
        if (stmt != null)
        {
            Interop.Odbc.SQLFreeStmt(stmt, Informix32.STMT.DROP);
        }
    }

    internal void GetLoStatusInfo()
    {
        if (lofd == -1)
        {
            throw ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
        }
        if (lostat == null)
        {
            paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
            HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(644);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
            rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 2, Informix32.SQL_C.BINARY, -100, 644, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_stat(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lostat = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 644);
        }
    }

    internal void GetDefaultLOSpec()
    {
        if (lospec == null)
        {
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 2, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_def_create_spec(?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
        }
    }

    internal void GetColumnSpecificLOSpec(string tableName, string columnName)
    {
        string text = tableName + "." + columnName;
        varLenDataBuf2 = paramBufs.GetVarLenDataBuf2(text.Length);
        varLenDataBuf2.MarshalToNative(0, text, Informix32.SQL_C.WCHAR, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_2.Address, -3L);
        HandleRef rgbValue = varLenDataBuf2.PtrOffset(0, varLenDataBuf2.Length);
        HandleRef strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.WCHAR, 1, text.Length, 0, rgbValue, varLenDataBuf2.Length, strLen_or_Ind);
        Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
        rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
        strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 2, 2, Informix32.SQL_C.BINARY, -100, varLenDataBuf1.Length, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_col_info(?, ?)}", -3);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        lospec = (byte[])varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, 596);
    }

    public long Seek(long offset, InformixSmartLOBWhence whence)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(offset, whence);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalSeek(offset, whence);
        ifxTrace?.ApiExit();
        return result;
    }

    internal long internalSeek(long offset, InformixSmartLOBWhence whence)
    {
        if (lofd == -1)
        {
            throw ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
        }
        paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
        HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
        HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf8_1.MarshalToNative(0, offset, Informix32.SQL_C.SBIGINT, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
        rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
        strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf4_2.MarshalToNative(0, (int)whence, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
        rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf4_2.Length);
        strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 3, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        Marshal.WriteInt64(paramBufs.indBuf_4.Address, 0L);
        rgbValue = paramBufs.dataBuf8_2.PtrOffset(0, paramBufs.dataBuf8_2.Length);
        strLen_or_Ind = paramBufs.indBuf_4.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 4, 2, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_seek(?, ?, ?, ?)}", -3);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        return (long)paramBufs.dataBuf8_2.MarshalToManaged(0, Informix32.SQL_C.SBIGINT, 0);
    }

    public void Close()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (!isNull)
        {
            internalClose();
            CloseSTMTHandle();
        }
        ifxTrace?.ApiExit();
    }

    internal void internalClose()
    {
        if (lofd != -1)
        {
            paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
            HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_close(?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lofd = -1;
        }
    }

    public InformixSmartLOBLocator GetLocator()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        ifxTrace?.ApiExit();
        return locator;
    }

    public void Lock(long smartLOBOffset, InformixSmartLOBWhence whence, long range, InformixSmartLOBLockMode lockMode)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(smartLOBOffset, whence, range, lockMode);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        if (lofd == -1)
        {
            throw ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
        }
        paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
        HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
        HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf8_1.MarshalToNative(0, smartLOBOffset, Informix32.SQL_C.SBIGINT, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
        rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
        strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf4_2.MarshalToNative(0, (int)whence, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
        rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf4_2.Length);
        strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 3, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf8_2.MarshalToNative(0, range, Informix32.SQL_C.SBIGINT, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_4.Address, 0L);
        rgbValue = paramBufs.dataBuf8_2.PtrOffset(0, paramBufs.dataBuf8_2.Length);
        strLen_or_Ind = paramBufs.indBuf_4.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 4, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf4_3.MarshalToNative(0, (int)lockMode, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_5.Address, 0L);
        rgbValue = paramBufs.dataBuf4_3.PtrOffset(0, paramBufs.dataBuf4_3.Length);
        strLen_or_Ind = paramBufs.indBuf_5.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 5, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_3.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_lock(?, ?, ?, ?, ?)}", -3);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        ifxTrace?.ApiExit();
    }

    public void Open(InformixSmartLOBOpenMode mode)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(mode);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        if (locator == null)
        {
            CreateSmartLOB(mode);
        }
        else
        {
            internalOpen(mode);
        }
        ifxTrace?.ApiExit();
    }

    internal void internalOpen(InformixSmartLOBOpenMode mode)
    {
        if (lofd == -1)
        {
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, -1L);
            HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            Marshal.WriteInt64(paramBufs.dataBufVarLen1.Address, 0L);
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(72);
            varLenDataBuf1.MarshalToNative(0, locator, Informix32.SQL_C.SB_LOCATOR, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_2.Address, 72L);
            rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
            paramBufs.indBuf_2.WriteInt16(0, 72);
            rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SB_LOCATOR, -100, 72, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            paramBufs.dataBuf4_2.MarshalToNative(0, (int)mode, Informix32.SQL_C.SLONG, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
            rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf4_2.Length);
            strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 3, 1, Informix32.SQL_C.SLONG, 4, 1, 2, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{? = call ifx_lo_open(?, ?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            lofd = (int)paramBufs.dataBuf4_1.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
        }
    }

    internal void CreateSmartLOB(InformixSmartLOBOpenMode mode)
    {
        GetDefaultLOSpec();
        rc = Interop.Odbc.SQLFreeStmt(stmt, Informix32.STMT.RESET_PARAMS);
        if (tableName.Length > 0 && colName.Length > 0)
        {
            GetColumnSpecificLOSpec(tableName, colName);
        }
        varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(596);
        varLenDataBuf1.MarshalToNative(0, lospec, Informix32.SQL_C.BINARY, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_1.Address, 596L);
        HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
        HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.BINARY, -100, 596, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf4_1.MarshalToNative(0, (int)mode, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
        rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
        strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        varLenDataBuf2 = paramBufs.GetVarLenDataBuf2(72);
        Marshal.WriteInt64(paramBufs.indBuf_3.Address, 72L);
        rgbValue = varLenDataBuf2.PtrOffset(0, varLenDataBuf2.Length);
        strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 3, 2, Informix32.SQL_C.SB_LOCATOR, -100, 72, 0, rgbValue, varLenDataBuf2.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        Marshal.WriteInt64(paramBufs.indBuf_4.Address, 0L);
        rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf4_2.Length);
        strLen_or_Ind = paramBufs.indBuf_4.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 4, 4, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_create(?, ?, ?, ?)}", -3);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        byte[] array = (byte[])varLenDataBuf2.MarshalToManaged(0, Informix32.SQL_C.BINARY, 72);
        locator = new InformixSmartLOBLocator(array);
        lofd = (int)paramBufs.dataBuf4_2.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
    }

    internal long internalRead(object buff)
    {
        long val = internalSize - internalPosition;
        val = 1 != lobType ? Math.Min(((byte[])buff).Length, val) : Math.Min(((char[])buff).Length, val);
        return internalRead(buff, 0L, val, 0L, InformixSmartLOBWhence.Current);
    }

    internal long internalRead(object buff, long plofd)
    {
        long numBytesToRead = ((byte[])buff).Length;
        isUsrLOFDProvided = true;
        usrProvLOFD = (int)plofd;
        return internalRead(buff, 0L, numBytesToRead, 0L, InformixSmartLOBWhence.Current);
    }

    internal long internalRead(object buff, long buffOffset, long numBytesToRead, long smartLOBOffset, InformixSmartLOBWhence whence)
    {
        object obj = null;
        bool flag = false;
        long num = 0L;
        if (numBytesToRead > 0)
        {
            while (true)
            {
                int num2;
                if (numBytesToRead > int.MaxValue)
                {
                    if (flag)
                    {
                        num2 = (int)(numBytesToRead - int.MaxValue);
                        smartLOBOffset = 0L;
                        whence = InformixSmartLOBWhence.Current;
                    }
                    else
                    {
                        num2 = int.MaxValue;
                    }
                }
                else
                {
                    num2 = (int)numBytesToRead;
                }
                if (isUsrLOFDProvided)
                {
                    paramBufs.dataBuf4_1.MarshalToNative(0, usrProvLOFD, Informix32.SQL_C.SLONG, 0, 0);
                }
                else
                {
                    paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
                }
                Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
                HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
                HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
                rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                if (1 == lobType)
                {
                    varLenDataBuf1 = paramBufs.GetVarLenDataBuf1((num2 + 1) * 2);
                }
                else
                {
                    varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(num2);
                }
                Marshal.WriteInt64(paramBufs.indBuf_2.Address, -1L);
                Marshal.WriteInt64(varLenDataBuf1.Address, -1L);
                rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
                strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
                rc = Interop.Odbc.SQLBindParameter(stmt, 2, 4, 1 == lobType ? Informix32.SQL_C.WCHAR : Informix32.SQL_C.BINARY, 1, 0, 0, rgbValue, 1 == lobType ? (num2 + 1) * 2 : num2, strLen_or_Ind);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                if (isUsrLOFDProvided)
                {
                    isUsrLOFDProvided = false;
                    rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_read(?, ?)}", -3);
                    if (rc != 0 && Informix32.RetCode.SUCCESS_WITH_INFO != rc)
                    {
                        connection.HandleError(hdbc, rc);
                    }
                }
                else
                {
                    paramBufs.dataBuf8_1.MarshalToNative(0, smartLOBOffset, Informix32.SQL_C.SBIGINT, 0, 0);
                    Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
                    rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
                    strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
                    rc = Interop.Odbc.SQLBindParameter(stmt, 3, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
                    if (rc != 0)
                    {
                        connection.HandleError(hdbc, rc);
                    }
                    paramBufs.dataBuf4_2.MarshalToNative(0, (int)whence, Informix32.SQL_C.SLONG, 0, 0);
                    Marshal.WriteInt64(paramBufs.indBuf_4.Address, 0L);
                    rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf4_2.Length);
                    strLen_or_Ind = paramBufs.indBuf_4.PtrOffset(0, 4);
                    rc = Interop.Odbc.SQLBindParameter(stmt, 4, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
                    if (rc != 0)
                    {
                        connection.HandleError(hdbc, rc);
                    }
                    rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_readwithseek(?, ?, ?, ?)}", -3);
                    if (rc != 0 && Informix32.RetCode.SUCCESS_WITH_INFO != rc)
                    {
                        connection.HandleError(hdbc, rc);
                    }
                }
                int num3 = (int)paramBufs.indBuf_2.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
                if (num3 == 0)
                {
                    break;
                }
                if (1 == lobType)
                {
                    num3 = Math.Min(num3, (num2 + 1) * 2);
                    obj = varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.WCHAR, num3);
                    num3 /= 2;
                }
                else
                {
                    num3 = Math.Min(num3, num2);
                    obj = varLenDataBuf1.MarshalToManaged(0, Informix32.SQL_C.BINARY, num3);
                }
                if (numBytesToRead > int.MaxValue && num3 < num2)
                {
                    flag = true;
                }
                num += num3;
                if (1 == lobType)
                {
                    Array.Copy(((string)obj).ToCharArray(), 0L, (char[])buff, buffOffset, num3);
                }
                else
                {
                    Array.Copy((byte[])obj, 0L, (byte[])buff, buffOffset, num3);
                }
                if (!flag)
                {
                    break;
                }
                buffOffset += num3;
            }
        }
        return num;
    }

    public void Release()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        if (!isNull && locator != null)
        {
            varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(72);
            varLenDataBuf1.MarshalToNative(0, locator, Informix32.SQL_C.SB_LOCATOR, 0, 0);
            Marshal.WriteInt64(paramBufs.indBuf_1.Address, 72L);
            HandleRef rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
            HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
            rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SB_LOCATOR, -100, 72, 0, rgbValue, varLenDataBuf1.Length, strLen_or_Ind);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
            rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_release(?)}", -3);
            if (rc != 0)
            {
                connection.HandleError(hdbc, rc);
            }
        }
        ifxTrace?.ApiExit();
    }

    public void Truncate(long offset)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(offset);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        if (lofd == -1)
        {
            throw ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
        }
        paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
        HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
        HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf8_1.MarshalToNative(0, offset, Informix32.SQL_C.SBIGINT, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
        rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
        strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_truncate(?, ?)}", -3);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        ifxTrace?.ApiExit();
    }

    public void Unlock(long smartLOBOffset, InformixSmartLOBWhence whence, long range)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(smartLOBOffset, whence, range);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        if (lofd == -1)
        {
            throw ADP.InvalidOperation(SR.GetString(SR.Odbc_NotInTransaction));
        }
        paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
        HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
        HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf8_1.MarshalToNative(0, smartLOBOffset, Informix32.SQL_C.SBIGINT, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
        rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
        strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf4_2.MarshalToNative(0, (int)whence, Informix32.SQL_C.SLONG, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
        rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf4_2.Length);
        strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 3, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        paramBufs.dataBuf8_2.MarshalToNative(0, range, Informix32.SQL_C.SBIGINT, 0, 0);
        Marshal.WriteInt64(paramBufs.indBuf_4.Address, 0L);
        rgbValue = paramBufs.dataBuf8_2.PtrOffset(0, paramBufs.dataBuf8_2.Length);
        strLen_or_Ind = paramBufs.indBuf_4.PtrOffset(0, 4);
        rc = Interop.Odbc.SQLBindParameter(stmt, 4, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_unlock(?, ?, ?, ?)}", -3);
        if (rc != 0)
        {
            connection.HandleError(hdbc, rc);
        }
        ifxTrace?.ApiExit();
    }

    internal long internalWrite(object buff)
    {
        long numBytesToWrite = 1 != lobType ? ((byte[])buff).Length : ((char[])buff).Length;
        return internalWrite(buff, 0L, numBytesToWrite, 0L, InformixSmartLOBWhence.Current);
    }

    internal long internalWrite(object buff, long buffOffset, long numBytesToWrite, long smartLOBOffset, InformixSmartLOBWhence whence)
    {
        object obj = null;
        bool flag = false;
        long num = 0L;
        if (numBytesToWrite > 0)
        {
            while (true)
            {
                int num2;
                if (numBytesToWrite > int.MaxValue)
                {
                    if (flag)
                    {
                        num2 = (int)(numBytesToWrite - int.MaxValue);
                        smartLOBOffset = 0L;
                        whence = InformixSmartLOBWhence.Current;
                    }
                    else
                    {
                        num2 = int.MaxValue;
                    }
                }
                else
                {
                    num2 = (int)numBytesToWrite;
                }
                paramBufs.dataBuf4_1.MarshalToNative(0, lofd, Informix32.SQL_C.SLONG, 0, 0);
                Marshal.WriteInt64(paramBufs.indBuf_1.Address, 0L);
                HandleRef rgbValue = paramBufs.dataBuf4_1.PtrOffset(0, paramBufs.dataBuf4_1.Length);
                HandleRef strLen_or_Ind = paramBufs.indBuf_1.PtrOffset(0, 4);
                rc = Interop.Odbc.SQLBindParameter(stmt, 1, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_1.Length, strLen_or_Ind);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                if (1 == lobType)
                {
                    obj = new char[num2];
                    varLenDataBuf1 = paramBufs.GetVarLenDataBuf1((num2 + 1) * 2);
                    Array.Copy((char[])buff, buffOffset, (char[])obj, 0L, num2);
                    varLenDataBuf1.MarshalToNative(0, obj, Informix32.SQL_C.CHAR, 0, 0);
                }
                else
                {
                    obj = new byte[num2];
                    varLenDataBuf1 = paramBufs.GetVarLenDataBuf1(num2);
                    Array.Copy((byte[])buff, buffOffset, (byte[])obj, 0L, num2);
                    varLenDataBuf1.MarshalToNative(0, obj, Informix32.SQL_C.BINARY, 0, 0);
                }
                Marshal.WriteInt64(paramBufs.indBuf_2.Address, 0L);
                rgbValue = varLenDataBuf1.PtrOffset(0, varLenDataBuf1.Length);
                strLen_or_Ind = paramBufs.indBuf_2.PtrOffset(0, 4);
                rc = Interop.Odbc.SQLBindParameter(stmt, 2, 1, 1 == lobType ? Informix32.SQL_C.WCHAR : Informix32.SQL_C.BINARY, 1, 0, 0, rgbValue, 1 == lobType ? num2 * 2 : num2, strLen_or_Ind);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                paramBufs.dataBuf8_1.MarshalToNative(0, smartLOBOffset, Informix32.SQL_C.SBIGINT, 0, 0);
                Marshal.WriteInt64(paramBufs.indBuf_3.Address, 0L);
                rgbValue = paramBufs.dataBuf8_1.PtrOffset(0, paramBufs.dataBuf8_1.Length);
                strLen_or_Ind = paramBufs.indBuf_3.PtrOffset(0, 4);
                rc = Interop.Odbc.SQLBindParameter(stmt, 3, 1, Informix32.SQL_C.SBIGINT, -5, 0, 0, rgbValue, 0, strLen_or_Ind);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                paramBufs.dataBuf4_2.MarshalToNative(0, (int)whence, Informix32.SQL_C.SLONG, 0, 0);
                Marshal.WriteInt64(paramBufs.indBuf_4.Address, 0L);
                rgbValue = paramBufs.dataBuf4_2.PtrOffset(0, paramBufs.dataBuf8_1.Length);
                strLen_or_Ind = paramBufs.indBuf_4.PtrOffset(0, 4);
                rc = Interop.Odbc.SQLBindParameter(stmt, 4, 1, Informix32.SQL_C.SLONG, 4, 0, 0, rgbValue, paramBufs.dataBuf4_2.Length, strLen_or_Ind);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                rc = Interop.Odbc.SQLExecDirectW(stmt, "{call ifx_lo_writewithseek(?, ?, ?, ?)}", -3);
                if (rc != 0)
                {
                    connection.HandleError(hdbc, rc);
                }
                int num3 = (int)paramBufs.indBuf_2.MarshalToManaged(0, Informix32.SQL_C.SLONG, 0);
                if (num3 == 0)
                {
                    break;
                }
                num3 = 1 == lobType ? num3 / 2 : num3;
                if (numBytesToWrite > int.MaxValue && num3 < num2)
                {
                    flag = true;
                }
                num += num3;
                if (!flag)
                {
                    break;
                }
                buffOffset += num3;
            }
        }
        return num;
    }

    public string ToFile(string filename, FileMode mode, InformixSmartLOBFileLocation fileLocation)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(filename, mode, fileLocation);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        StreamWriter streamWriter = null;
        FileStream fileStream = null;
        if (1 == (short)fileLocation)
        {
            throw ADP.NotSupported();
        }
        if (1 == lobType)
        {
            char[] array = new char[524287];
            bool append = mode.CompareTo(FileMode.Append) == 0;
            try
            {
                streamWriter = new StreamWriter(filename, append);
                bool flag = true;
                InformixSmartLOBWhence whence = InformixSmartLOBWhence.Begin;
                long num;
                do
                {
                    num = internalRead(array, 0L, array.Length, 0L, whence);
                    if (num != 0L)
                    {
                        streamWriter.Write(array, 0, (int)num);
                        if (flag)
                        {
                            whence = InformixSmartLOBWhence.Current;
                            flag = false;
                        }
                    }
                }
                while (num != 0L);
            }
            catch
            {
                return null;
            }
            finally
            {
                streamWriter.Close();
            }
        }
        else
        {
            try
            {
                byte[] array2 = new byte[524587];
                fileStream = new FileStream(filename, mode);
                bool flag = true;
                InformixSmartLOBWhence whence = InformixSmartLOBWhence.Begin;
                long num;
                do
                {
                    num = internalRead(array2, 0L, array2.Length, 0L, whence);
                    if (num != 0L)
                    {
                        fileStream.Write(array2, 0, (int)num);
                        if (flag)
                        {
                            whence = InformixSmartLOBWhence.Current;
                            flag = false;
                        }
                    }
                }
                while (num != 0L);
            }
            catch
            {
                return null;
            }
            finally
            {
                fileStream.Close();
            }
        }
        ifxTrace?.ApiExit();
        return filename;
    }

    public void FromFile(string filename, bool appendToSmartLOB, InformixSmartLOBFileLocation fileLocation)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(filename, appendToSmartLOB, fileLocation);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        StreamReader streamReader = null;
        FileStream fileStream = null;
        if (1 == (short)fileLocation)
        {
            throw ADP.NotSupported();
        }
        if (1 == lobType)
        {
            try
            {
                char[] array = new char[524287];
                streamReader = new StreamReader(filename);
                bool flag = true;
                InformixSmartLOBWhence whence = appendToSmartLOB ? InformixSmartLOBWhence.End : InformixSmartLOBWhence.Begin;
                int num;
                do
                {
                    num = streamReader.Read(array, 0, 524287);
                    long num2 = internalWrite(array, 0L, num, 0L, whence);
                    if (flag)
                    {
                        whence = InformixSmartLOBWhence.Current;
                        flag = false;
                    }
                }
                while (num != 0);
            }
            catch
            {
                return;
            }
            finally
            {
                streamReader.Close();
            }
        }
        else
        {
            try
            {
                byte[] array2 = new byte[524287];
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                bool flag = true;
                InformixSmartLOBWhence whence = appendToSmartLOB ? InformixSmartLOBWhence.End : InformixSmartLOBWhence.Begin;
                int num;
                do
                {
                    num = fileStream.Read(array2, 0, 524287);
                    long num2 = internalWrite(array2, 0L, num, 0L, whence);
                    if (flag)
                    {
                        whence = InformixSmartLOBWhence.Current;
                        flag = false;
                    }
                }
                while (num != 0);
            }
            catch
            {
                return;
            }
            finally
            {
                fileStream.Close();
            }
        }
        ifxTrace?.ApiExit();
    }
}
