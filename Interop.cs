using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;


namespace Arad.Net.Core.Informix;
internal static class Interop
{
	internal static class Odbc
	{
		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLAllocHandle(Informix32.SQL_HANDLE HandleType, IntPtr InputHandle, out IntPtr OutputHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLAllocHandle(Informix32.SQL_HANDLE HandleType, OdbcHandle InputHandle, out IntPtr OutputHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLBindCol(OdbcStatementHandle StatementHandle, ushort ColumnNumber, Informix32.SQL_C TargetType, HandleRef TargetValue, IntPtr BufferLength, IntPtr StrLen_or_Ind);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLBindCol(OdbcStatementHandle StatementHandle, ushort ColumnNumber, Informix32.SQL_C TargetType, IntPtr TargetValue, IntPtr BufferLength, IntPtr StrLen_or_Ind);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLBindParameter(OdbcStatementHandle StatementHandle, ushort ParameterNumber, short ParamDirection, Informix32.SQL_C SQLCType, short SQLType, IntPtr cbColDef, IntPtr ibScale, HandleRef rgbValue, IntPtr BufferLength, HandleRef StrLen_or_Ind);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLCancel(OdbcStatementHandle StatementHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLCloseCursor(OdbcStatementHandle StatementHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLColAttributeW(OdbcStatementHandle StatementHandle, short ColumnNumber, short FieldIdentifier, CNativeBuffer CharacterAttribute, short BufferLength, out short StringLength, out IntPtr NumericAttribute);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLColumnsW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string TableName, short NameLen3, [In][MarshalAs(UnmanagedType.LPWStr)] string ColumnName, short NameLen4);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLDisconnect(IntPtr ConnectionHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode IFMX_IsInTransaction(IntPtr ConnectionHandle);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLDriverConnectW(InformixConnectionHandle hdbc, IntPtr hwnd, [In][MarshalAs(UnmanagedType.LPWStr)] string connectionstring, short cbConnectionstring, IntPtr connectionstringout, short cbConnectionstringoutMax, out short cbConnectionstringout, short fDriverCompletion);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLEndTran(Informix32.SQL_HANDLE HandleType, IntPtr Handle, short CompletionType);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLExecDirectW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string StatementText, int TextLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLExecute(OdbcStatementHandle StatementHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLFetch(OdbcStatementHandle StatementHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLFreeHandle(Informix32.SQL_HANDLE HandleType, IntPtr StatementHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLFreeStmt(OdbcStatementHandle StatementHandle, Informix32.STMT Option);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetConnectAttrW(InformixConnectionHandle ConnectionHandle, Informix32.SQL_ATTR Attribute, byte[] Value, int BufferLength, out int StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetData(OdbcStatementHandle StatementHandle, ushort ColumnNumber, Informix32.SQL_C TargetType, CNativeBuffer TargetValue, IntPtr BufferLength, out IntPtr StrLen_or_Ind);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetDescFieldW(OdbcDescriptorHandle StatementHandle, short RecNumber, Informix32.SQL_DESC FieldIdentifier, CNativeBuffer ValuePointer, int BufferLength, out int StringLength);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLGetDiagRecW(Informix32.SQL_HANDLE HandleType, OdbcHandle Handle, short RecNumber, [Out] StringBuilder rchState, out int NativeError, [Out] StringBuilder MessageText, short BufferLength, out short TextLength);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLGetDiagFieldW(Informix32.SQL_HANDLE HandleType, OdbcHandle Handle, short RecNumber, short DiagIdentifier, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder rchState, short BufferLength, out short StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetFunctions(InformixConnectionHandle hdbc, Informix32.SQL_API fFunction, out short pfExists);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetInfoW(InformixConnectionHandle hdbc, Informix32.SQL_INFO fInfoType, byte[] rgbInfoValue, short cbInfoValueMax, out short pcbInfoValue);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetInfoW(InformixConnectionHandle hdbc, Informix32.SQL_INFO fInfoType, byte[] rgbInfoValue, short cbInfoValueMax, IntPtr pcbInfoValue);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetStmtAttrW(OdbcStatementHandle StatementHandle, Informix32.SQL_ATTR Attribute, out IntPtr Value, int BufferLength, out int StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetTypeInfo(OdbcStatementHandle StatementHandle, short fSqlType);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLMoreResults(OdbcStatementHandle StatementHandle);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLNumResultCols(OdbcStatementHandle StatementHandle, out short ColumnCount);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLPrepareW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string StatementText, int TextLength);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLPrimaryKeysW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string TableName, short NameLen3);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLProcedureColumnsW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string ProcName, short NameLen3, [In][MarshalAs(UnmanagedType.LPWStr)] string ColumnName, short NameLen4);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLProceduresW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string ProcName, short NameLen3);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLRowCount(OdbcStatementHandle StatementHandle, out IntPtr RowCount);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetConnectAttrW(InformixConnectionHandle ConnectionHandle, Informix32.SQL_ATTR Attribute, IDtcTransaction Value, int StringLength);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLSetConnectAttrW(InformixConnectionHandle ConnectionHandle, Informix32.SQL_ATTR Attribute, string Value, int StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetConnectAttrW(InformixConnectionHandle ConnectionHandle, Informix32.SQL_ATTR Attribute, IntPtr Value, int StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetConnectAttrW(IntPtr ConnectionHandle, Informix32.SQL_ATTR Attribute, IntPtr Value, int StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetDescFieldW(OdbcDescriptorHandle StatementHandle, short ColumnNumber, Informix32.SQL_DESC FieldIdentifier, HandleRef CharacterAttribute, int BufferLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetDescFieldW(OdbcDescriptorHandle StatementHandle, short ColumnNumber, Informix32.SQL_DESC FieldIdentifier, IntPtr CharacterAttribute, int BufferLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetEnvAttr(OdbcEnvironmentHandle EnvironmentHandle, Informix32.SQL_ATTR Attribute, IntPtr Value, Informix32.SQL_IS StringLength);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLGetEnvAttr(OdbcEnvironmentHandle EnvironmentHandle, Informix32.SQL_ATTR Attribute, out IntPtr Value, int StringLength, out int StringLengthPtr);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLSetStmtAttrW(OdbcStatementHandle StatementHandle, int Attribute, IntPtr Value, int StringLength);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLSpecialColumnsW(OdbcStatementHandle StatementHandle, Informix32.SQL_SPECIALCOLS IdentifierType, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string TableName, short NameLen3, Informix32.SQL_SCOPE Scope, Informix32.SQL_NULLABILITY Nullable);

		[DllImport("iclit09b.dll", CharSet = CharSet.Unicode)]
		internal static extern Informix32.RetCode SQLStatisticsW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string TableName, short NameLen3, short Unique, short Reserved);

		[DllImport("iclit09b.dll")]
		internal static extern Informix32.RetCode SQLTablesW(OdbcStatementHandle StatementHandle, [In][MarshalAs(UnmanagedType.LPWStr)] string CatalogName, short NameLen1, [In][MarshalAs(UnmanagedType.LPWStr)] string SchemaName, short NameLen2, [In][MarshalAs(UnmanagedType.LPWStr)] string TableName, short NameLen3, [In][MarshalAs(UnmanagedType.LPWStr)] string TableType, short NameLen4);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dtaddinv(IntPtr dtValue, IntPtr intrvl, IntPtr res);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dtcvasc([In][MarshalAs(UnmanagedType.LPStr)] string inbuf, IntPtr intrvl);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode incvasc([In][MarshalAs(UnmanagedType.LPStr)] string inbuf, IntPtr intrvl);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dttofmtasc(IntPtr dtValue, byte[] dateTimeString, int buffLen, [In][MarshalAs(UnmanagedType.LPStr)] string fmtString);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode intoasc(IntPtr intrvl, byte[] outbuf);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dttoasc(IntPtr dtValue, byte[] dateTimeString);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dtcvfmtasc([In][MarshalAs(UnmanagedType.LPStr)] string inbuf, [In][MarshalAs(UnmanagedType.LPStr)] string fmtString, IntPtr dtValue);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dtsubinv(IntPtr dtValue, IntPtr intrvl, IntPtr res);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dtsub(IntPtr dtValue1, IntPtr dtValue2, IntPtr res);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode deccvasc(byte[] pByteStr, int len, byte[] pByteDec_t);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode dectoasc(byte[] pByteSrcDecimalVal, byte[] pByteTargStr, int len, int right);

		[DllImport("isqlt09a.dll")]
		internal static extern short deccmp(byte[] pByte1, byte[] pByte2);

		[DllImport("isqlt09a.dll")]
		internal static extern short decload(byte[] pByteDec_t, int pos, int exp, char[] dgts, int ndgts);

		[DllImport("isqlt09a.dll")]
		internal static extern short dectodbl(byte[] pByteSrcDecimalVal, IntPtr pByteTargDblVal);

		[DllImport("isqlt09a.dll")]
		internal static extern short dectolong(byte[] pByteSrcDecimalVal, IntPtr pByteTargDblVal);

		[DllImport("isqlt09a.dll")]
		internal static extern short decadd(byte[] pByte1, byte[] pByte2, byte[] pByteRes);

		[DllImport("isqlt09a.dll")]
		internal static extern short decsub(byte[] pByte1, byte[] pByte2, byte[] pByteRes);

		[DllImport("isqlt09a.dll")]
		internal static extern short decdiv(byte[] pByte1, byte[] pByte2, byte[] pBytRes);

		[DllImport("isqlt09a.dll")]
		internal static extern short decmul(byte[] pByte1, byte[] pByte2, byte[] pByteRes);

		[DllImport("isqlt09a.dll")]
		internal static extern void decround(byte[] pByteDecimalVal, int NumFraction);

		[DllImport("isqlt09a.dll")]
		internal static extern void dectrunc(byte[] pByteDecimalVal, int NumFraction);

		[DllImport("isqlt09a.dll")]
		internal static extern short rfmtdec(byte[] decval, byte[] fmtstring, byte[] outbuf);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode incvfmtasc([In][MarshalAs(UnmanagedType.LPStr)] string inbuf1, [In][MarshalAs(UnmanagedType.LPStr)] string inbuf2, IntPtr intrvl);

		[DllImport("isqlt09a.dll")]
		internal static extern Informix32.RetCode intofmtasc(IntPtr intrvl, byte[] outbuf, short outbuf_len, [In][MarshalAs(UnmanagedType.LPStr)] string format);
	}

	internal static class Libraries
	{
		internal const string Advapi32 = "advapi32.dll";

		internal const string BCrypt = "BCrypt.dll";

		internal const string CoreComm_L1_1_1 = "api-ms-win-core-comm-l1-1-1.dll";

		internal const string CoreComm_L1_1_2 = "api-ms-win-core-comm-l1-1-2.dll";

		internal const string Crypt32 = "crypt32.dll";

		internal const string CryptUI = "cryptui.dll";

		internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";

		internal const string Gdi32 = "gdi32.dll";

		internal const string HttpApi = "httpapi.dll";

		internal const string IpHlpApi = "iphlpapi.dll";

		internal const string Kernel32 = "kernel32.dll";

		internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";

		internal const string Mswsock = "mswsock.dll";

		internal const string NCrypt = "ncrypt.dll";

		internal const string NtDll = "ntdll.dll";

		internal const string Odbc32 = "odbc32.dll";

		internal const string Ifx32 = "iclit09b.dll";

		internal const string Genlib = "isqlt09a.dll";

		internal const string Ole32 = "ole32.dll";

		internal const string OleAut32 = "oleaut32.dll";

		internal const string PerfCounter = "perfcounter.dll";

		internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";

		internal const string Secur32 = "secur32.dll";

		internal const string Shell32 = "shell32.dll";

		internal const string SspiCli = "sspicli.dll";

		internal const string User32 = "user32.dll";

		internal const string Version = "version.dll";

		internal const string WebSocket = "websocket.dll";

		internal const string WinHttp = "winhttp.dll";

		internal const string WinMM = "winmm.dll";

		internal const string Ws2_32 = "ws2_32.dll";

		internal const string Wtsapi32 = "wtsapi32.dll";

		internal const string CompressionNative = "clrcompression.dll";

		internal const string CoreWinRT = "api-ms-win-core-winrt-l1-1-0.dll";
	}
}
