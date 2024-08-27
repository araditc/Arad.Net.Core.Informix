using Arad.Net.Core.Informix;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using IsolationLevel = System.Data.IsolationLevel;

namespace Arad.Net.Core.Informix.System.Data.Common;

internal static class ADP
{
	internal enum InternalErrorCode
	{
		UnpooledObjectHasOwner = 0,
		UnpooledObjectHasWrongOwner = 1,
		PushingObjectSecondTime = 2,
		PooledObjectHasOwner = 3,
		PooledObjectInPoolMoreThanOnce = 4,
		CreateObjectReturnedNull = 5,
		NewObjectCannotBePooled = 6,
		NonPooledObjectUsedMoreThanOnce = 7,
		AttemptingToPoolOnRestrictedToken = 8,
		ConvertSidToStringSidWReturnedNull = 10,
		AttemptingToConstructReferenceCollectionOnStaticObject = 12,
		AttemptingToEnlistTwice = 13,
		CreateReferenceCollectionReturnedNull = 14,
		PooledObjectWithoutPool = 15,
		UnexpectedWaitAnyResult = 16,
		SynchronousConnectReturnedPending = 17,
		CompletedConnectReturnedPending = 18,
		NameValuePairNext = 20,
		InvalidParserState1 = 21,
		InvalidParserState2 = 22,
		InvalidParserState3 = 23,
		InvalidBuffer = 30,
		UnimplementedSMIMethod = 40,
		InvalidSmiCall = 41,
		SqlDependencyObtainProcessDispatcherFailureObjectHandle = 50,
		SqlDependencyProcessDispatcherFailureCreateInstance = 51,
		SqlDependencyProcessDispatcherFailureAppDomain = 52,
		SqlDependencyCommandHashIsNotAssociatedWithNotification = 53,
		UnknownTransactionFailure = 60
	}

	internal enum ConnectionError
	{
		BeginGetConnectionReturnsNull,
		GetConnectionReturnsNull,
		ConnectionOptionsMissing,
		CouldNotSwitchToClosedPreviouslyOpenedState
	}

	private static Task<bool> _trueTask;

	private static Task<bool> _falseTask;

	internal const CompareOptions DefaultCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

	internal const int DefaultConnectionTimeout = 15;

	private static readonly Type s_stackOverflowType = typeof(StackOverflowException);

	private static readonly Type s_outOfMemoryType = typeof(OutOfMemoryException);

	private static readonly Type s_threadAbortType = typeof(ThreadAbortException);

	private static readonly Type s_nullReferenceType = typeof(NullReferenceException);

	private static readonly Type s_accessViolationType = typeof(AccessViolationException);

	private static readonly Type s_securityType = typeof(SecurityException);

	internal const string BeginTransaction = "BeginTransaction";

	internal const string ChangeDatabase = "ChangeDatabase";

	internal const string CommitTransaction = "CommitTransaction";

	internal const string CommandTimeout = "CommandTimeout";

	internal const string DeriveParameters = "DeriveParameters";

	internal const string ExecuteReader = "ExecuteReader";

	internal const string ExecuteNonQuery = "ExecuteNonQuery";

	internal const string ExecuteScalar = "ExecuteScalar";

	internal const string GetSchema = "GetSchema";

	internal const string GetSchemaTable = "GetSchemaTable";

	internal const string Parameter = "Parameter";

	internal const string ParameterName = "ParameterName";

	internal const string Prepare = "Prepare";

	internal const string RollbackTransaction = "RollbackTransaction";

	internal const string QuoteIdentifier = "QuoteIdentifier";

	internal const string UnquoteIdentifier = "UnquoteIdentifier";

	internal const int DecimalMaxPrecision = 29;

	internal const int DecimalMaxPrecision28 = 28;

	internal const int DefaultCommandTimeout = 30;

	internal const string StrEmpty = "";

	internal static readonly IntPtr PtrZero = new IntPtr(0);

	internal static readonly int PtrSize = IntPtr.Size;

	internal static Task<bool> TrueTask => _trueTask ?? (_trueTask = Task.FromResult(result: true));

	internal static Task<bool> FalseTask => _falseTask ?? (_falseTask = Task.FromResult(result: false));

	internal static void TraceExceptionAsReturnValue(Exception e)
	{
	}

	internal static void TraceExceptionWithoutRethrow(Exception e)
	{
	}

	internal static ArgumentException Argument(string error)
	{
		ArgumentException ex = new ArgumentException(error);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentException Argument(string error, Exception inner)
	{
		ArgumentException ex = new ArgumentException(error, inner);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentException Argument(string error, string parameter)
	{
		ArgumentException ex = new ArgumentException(error, parameter);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentNullException ArgumentNull(string parameter)
	{
		ArgumentNullException ex = new ArgumentNullException(parameter);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentNullException ArgumentNull(string parameter, string error)
	{
		ArgumentNullException ex = new ArgumentNullException(parameter, error);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentOutOfRangeException ArgumentOutOfRange(string parameterName)
	{
		ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(parameterName);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
	{
		ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException(parameterName, message);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static IndexOutOfRangeException IndexOutOfRange(string error)
	{
		IndexOutOfRangeException ex = new IndexOutOfRangeException(error);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static InvalidCastException InvalidCast(string error)
	{
		return InvalidCast(error, null);
	}

	internal static InvalidCastException InvalidCast(string error, Exception inner)
	{
		InvalidCastException ex = new InvalidCastException(error, inner);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static InvalidOperationException InvalidOperation(string error)
	{
		InvalidOperationException ex = new InvalidOperationException(error);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static NotSupportedException NotSupported()
	{
		NotSupportedException ex = new NotSupportedException();
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static NotSupportedException NotSupported(string error)
	{
		NotSupportedException ex = new NotSupportedException(error);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static bool RemoveStringQuotes(string quotePrefix, string quoteSuffix, string quotedString, out string unquotedString)
	{
		int num = quotePrefix?.Length ?? 0;
		int num2 = quoteSuffix?.Length ?? 0;
		if (num2 + num == 0)
		{
			unquotedString = quotedString;
			return true;
		}
		if (quotedString == null)
		{
			unquotedString = quotedString;
			return false;
		}
		int length = quotedString.Length;
		if (length < num + num2)
		{
			unquotedString = quotedString;
			return false;
		}
		if (num > 0 && !quotedString.StartsWith(quotePrefix, StringComparison.Ordinal))
		{
			unquotedString = quotedString;
			return false;
		}
		if (num2 > 0)
		{
			if (!quotedString.EndsWith(quoteSuffix, StringComparison.Ordinal))
			{
				unquotedString = quotedString;
				return false;
			}
			unquotedString = quotedString.Substring(num, length - (num + num2)).Replace(quoteSuffix + quoteSuffix, quoteSuffix);
		}
		else
		{
			unquotedString = quotedString.Substring(num, length - num);
		}
		return true;
	}

	internal static ArgumentOutOfRangeException NotSupportedEnumerationValue(Type type, string value, string method)
	{
		return ArgumentOutOfRange(System.SR.Format(System.SR.ADP_NotSupportedEnumerationValue, type.Name, value, method), type.Name);
	}

	internal static InvalidOperationException DataAdapter(string error)
	{
		return InvalidOperation(error);
	}

	private static InvalidOperationException Provider(string error)
	{
		return InvalidOperation(error);
	}

	internal static ArgumentException InvalidMultipartName(string property, string value)
	{
		ArgumentException ex = new ArgumentException(System.SR.Format(System.SR.ADP_InvalidMultipartName, property, value));
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentException InvalidMultipartNameIncorrectUsageOfQuotes(string property, string value)
	{
		ArgumentException ex = new ArgumentException(System.SR.Format(System.SR.ADP_InvalidMultipartNameQuoteUsage, property, value));
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static ArgumentException InvalidMultipartNameToManyParts(string property, string value, int limit)
	{
		ArgumentException ex = new ArgumentException(System.SR.Format(System.SR.ADP_InvalidMultipartNameToManyParts, property, value, limit));
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static void CheckArgumentNull(object value, string parameterName)
	{
		if (value == null)
		{
			throw ArgumentNull(parameterName);
		}
	}

	internal static bool IsCatchableExceptionType(Exception e)
	{
		Type type = e.GetType();
		if (type != s_stackOverflowType && type != s_outOfMemoryType && type != s_threadAbortType && type != s_nullReferenceType && type != s_accessViolationType)
		{
			return !s_securityType.IsAssignableFrom(type);
		}
		return false;
	}

	internal static bool IsCatchableOrSecurityExceptionType(Exception e)
	{
		Type type = e.GetType();
		if (type != s_stackOverflowType && type != s_outOfMemoryType && type != s_threadAbortType && type != s_nullReferenceType)
		{
			return type != s_accessViolationType;
		}
		return false;
	}

	internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
	{
		return ArgumentOutOfRange(System.SR.Format(System.SR.ADP_InvalidEnumerationValue, type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
	}

	internal static ArgumentException ConnectionStringSyntax(int index)
	{
		return Argument(System.SR.Format(System.SR.ADP_ConnectionStringSyntax, index));
	}

	internal static ArgumentException KeywordNotSupported(string keyword)
	{
		return Argument(System.SR.Format(System.SR.ADP_KeywordNotSupported, keyword));
	}

	internal static ArgumentException ConvertFailed(Type fromType, Type toType, Exception innerException)
	{
		return Argument(System.SR.Format(System.SR.SqlConvert_ConvertFailed, fromType.FullName, toType.FullName), innerException);
	}

	internal static Exception InvalidConnectionOptionValue(string key)
	{
		return InvalidConnectionOptionValue(key, null);
	}

	internal static Exception InvalidConnectionOptionValue(string key, Exception inner)
	{
		return Argument(System.SR.Format(System.SR.ADP_InvalidConnectionOptionValue, key), inner);
	}

	internal static ArgumentException CollectionRemoveInvalidObject(Type itemType, ICollection collection)
	{
		return Argument(System.SR.Format(System.SR.ADP_CollectionRemoveInvalidObject, itemType.Name, collection.GetType().Name));
	}

	internal static ArgumentNullException CollectionNullValue(string parameter, Type collection, Type itemType)
	{
		return ArgumentNull(parameter, System.SR.Format(System.SR.ADP_CollectionNullValue, collection.Name, itemType.Name));
	}

	internal static IndexOutOfRangeException CollectionIndexInt32(int index, Type collection, int count)
	{
		return IndexOutOfRange(System.SR.Format(System.SR.ADP_CollectionIndexInt32, index.ToString(CultureInfo.InvariantCulture), collection.Name, count.ToString(CultureInfo.InvariantCulture)));
	}

	internal static IndexOutOfRangeException CollectionIndexString(Type itemType, string propertyName, string propertyValue, Type collection)
	{
		return IndexOutOfRange(System.SR.Format(System.SR.ADP_CollectionIndexString, itemType.Name, propertyName, propertyValue, collection.Name));
	}

	internal static InvalidCastException CollectionInvalidType(Type collection, Type itemType, object invalidValue)
	{
		return InvalidCast(System.SR.Format(System.SR.ADP_CollectionInvalidType, collection.Name, itemType.Name, invalidValue.GetType().Name));
	}

	private static string ConnectionStateMsg(ConnectionState state)
	{
		switch (state)
		{
		case ConnectionState.Closed:
		case ConnectionState.Connecting | ConnectionState.Broken:
			return System.SR.ADP_ConnectionStateMsg_Closed;
		case ConnectionState.Connecting:
			return System.SR.ADP_ConnectionStateMsg_Connecting;
		case ConnectionState.Open:
			return System.SR.ADP_ConnectionStateMsg_Open;
		case ConnectionState.Open | ConnectionState.Executing:
			return System.SR.ADP_ConnectionStateMsg_OpenExecuting;
		case ConnectionState.Open | ConnectionState.Fetching:
			return System.SR.ADP_ConnectionStateMsg_OpenFetching;
		default:
			return System.SR.Format(System.SR.ADP_ConnectionStateMsg, state.ToString());
		}
	}

	internal static Exception StreamClosed([CallerMemberName] string method = "")
	{
		return InvalidOperation(System.SR.Format(System.SR.ADP_StreamClosed, method));
	}

	internal static string BuildQuotedString(string quotePrefix, string quoteSuffix, string unQuotedString)
	{
		StringBuilder stringBuilder = new StringBuilder(unQuotedString.Length + quoteSuffix.Length + quoteSuffix.Length);
		AppendQuotedString(stringBuilder, quotePrefix, quoteSuffix, unQuotedString);
		return stringBuilder.ToString();
	}

	internal static string AppendQuotedString(StringBuilder buffer, string quotePrefix, string quoteSuffix, string unQuotedString)
	{
		if (!string.IsNullOrEmpty(quotePrefix))
		{
			buffer.Append(quotePrefix);
		}
		if (!string.IsNullOrEmpty(quoteSuffix))
		{
			int length = buffer.Length;
			buffer.Append(unQuotedString);
			buffer.Replace(quoteSuffix, quoteSuffix + quoteSuffix, length, unQuotedString.Length);
			buffer.Append(quoteSuffix);
		}
		else
		{
			buffer.Append(unQuotedString);
		}
		return buffer.ToString();
	}

	internal static ArgumentException ParametersIsNotParent(Type parameterType, ICollection collection)
	{
		return Argument(System.SR.Format(System.SR.ADP_CollectionIsNotParent, parameterType.Name, collection.GetType().Name));
	}

	internal static ArgumentException ParametersIsParent(Type parameterType, ICollection collection)
	{
		return Argument(System.SR.Format(System.SR.ADP_CollectionIsNotParent, parameterType.Name, collection.GetType().Name));
	}

	internal static Exception InternalError(InternalErrorCode internalError)
	{
		return InvalidOperation(System.SR.Format(System.SR.ADP_InternalProviderError, (int)internalError));
	}

	internal static Exception DataReaderClosed([CallerMemberName] string method = "")
	{
		return InvalidOperation(System.SR.Format(System.SR.ADP_DataReaderClosed, method));
	}

	internal static ArgumentOutOfRangeException InvalidSourceBufferIndex(int maxLen, long srcOffset, string parameterName)
	{
		return ArgumentOutOfRange(System.SR.Format(System.SR.ADP_InvalidSourceBufferIndex, maxLen.ToString(CultureInfo.InvariantCulture), srcOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
	}

	internal static ArgumentOutOfRangeException InvalidDestinationBufferIndex(int maxLen, int dstOffset, string parameterName)
	{
		return ArgumentOutOfRange(System.SR.Format(System.SR.ADP_InvalidDestinationBufferIndex, maxLen.ToString(CultureInfo.InvariantCulture), dstOffset.ToString(CultureInfo.InvariantCulture)), parameterName);
	}

	internal static IndexOutOfRangeException InvalidBufferSizeOrIndex(int numBytes, int bufferIndex)
	{
		return IndexOutOfRange(System.SR.Format(System.SR.SQL_InvalidBufferSizeOrIndex, numBytes.ToString(CultureInfo.InvariantCulture), bufferIndex.ToString(CultureInfo.InvariantCulture)));
	}

	internal static Exception InvalidDataLength(long length)
	{
		return IndexOutOfRange(System.SR.Format(System.SR.SQL_InvalidDataLength, length.ToString(CultureInfo.InvariantCulture)));
	}

	internal static bool CompareInsensitiveInvariant(string strvalue, string strconst)
	{
		return CultureInfo.InvariantCulture.CompareInfo.Compare(strvalue, strconst, CompareOptions.IgnoreCase) == 0;
	}

	internal static int DstCompare(string strA, string strB)
	{
		return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
	}

	internal static bool IsEmptyArray(string[] array)
	{
		if (array != null)
		{
			return array.Length == 0;
		}
		return true;
	}

	internal static bool IsNull(object value)
	{
		if (value == null || DBNull.Value == value)
		{
			return true;
		}
		if (value is INullable nullable)
		{
			return nullable.IsNull;
		}
		return false;
	}

	internal static Exception InvalidSeekOrigin(string parameterName)
	{
		return ArgumentOutOfRange(System.SR.ADP_InvalidSeekOrigin, parameterName);
	}

	internal static void SetCurrentTransaction(Transaction transaction)
	{
		Transaction.Current = transaction;
	}

	internal static Timer UnsafeCreateTimer(TimerCallback callback, object state, int dueTime, int period)
	{
		bool flag = false;
		try
		{
			if (!ExecutionContext.IsFlowSuppressed())
			{
				ExecutionContext.SuppressFlow();
				flag = true;
			}
			return new Timer(callback, state, dueTime, period);
		}
		finally
		{
			if (flag)
			{
				ExecutionContext.RestoreFlow();
			}
		}
	}

	internal static Exception ExceptionWithStackTrace(Exception e)
	{
		try
		{
			throw e;
		}
		catch (Exception result)
		{
			return result;
		}
	}

	internal static Exception WrongType(Type type)
	{
		return Argument(Res.GetString("ADP_WrongType", type.FullName));
	}

	internal static Exception NotAPermissionElement()
	{
		return Argument(Res.GetString("ADP_NotAPermissionElement"));
	}

	internal static Exception InvalidXMLBadVersion()
	{
		return Argument(Res.GetString("ADP_InvalidXMLBadVersion"));
	}

	internal static TimeoutException TimeoutException(string error)
	{
		TimeoutException ex = new TimeoutException(error);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static InvalidOperationException InvalidOperation(string error, Exception inner)
	{
		InvalidOperationException ex = new InvalidOperationException(error, inner);
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static InvalidCastException InvalidCast()
	{
		InvalidCastException ex = new InvalidCastException();
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static void CheckArgumentLength(string value, string parameterName)
	{
		CheckArgumentNull(value, parameterName);
		if (value.Length == 0)
		{
			throw Argument(System.SR.GetString(System.SR.ADP_EmptyString, parameterName));
		}
	}

	internal static ArgumentOutOfRangeException InvalidCommandType(CommandType value)
	{
		return InvalidEnumerationValue(typeof(CommandType), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidDataRowVersion(DataRowVersion value)
	{
		return InvalidEnumerationValue(typeof(DataRowVersion), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidIsolationLevel(IsolationLevel value)
	{
		return InvalidEnumerationValue(typeof(IsolationLevel), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidKeyRestrictionBehavior(KeyRestrictionBehavior value)
	{
		return InvalidEnumerationValue(typeof(KeyRestrictionBehavior), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidParameterDirection(ParameterDirection value)
	{
		return InvalidEnumerationValue(typeof(ParameterDirection), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidUpdateRowSource(UpdateRowSource value)
	{
		return InvalidEnumerationValue(typeof(UpdateRowSource), (int)value);
	}

	internal static InvalidOperationException InvalidDataDirectory()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_InvalidDataDirectory));
	}

	internal static ArgumentException InvalidKeyname(string parameterName)
	{
		return Argument(System.SR.GetString(System.SR.ADP_InvalidKey), parameterName);
	}

	internal static ArgumentException InvalidValue(string parameterName)
	{
		return Argument(System.SR.GetString(System.SR.ADP_InvalidValue), parameterName);
	}

	internal static InvalidOperationException NoConnectionString()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_NoConnectionString));
	}

	internal static Exception MethodNotImplemented([CallerMemberName] string methodName = "")
	{
		return System.NotImplemented.ByDesignWithMessage(methodName);
	}

	internal static Exception OdbcNoTypesFromProvider()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_OdbcNoTypesFromProvider));
	}

	internal static Exception PooledOpenTimeout()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_PooledOpenTimeout));
	}

	internal static Exception NonPooledOpenTimeout()
	{
		return TimeoutException(System.SR.GetString(System.SR.ADP_NonPooledOpenTimeout));
	}

	internal static InvalidOperationException TransactionConnectionMismatch()
	{
		return Provider(System.SR.GetString(System.SR.ADP_TransactionConnectionMismatch));
	}

	internal static InvalidOperationException TransactionRequired(string method)
	{
		return Provider(System.SR.GetString(System.SR.ADP_TransactionRequired, method));
	}

	internal static Exception CommandTextRequired(string method)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_CommandTextRequired, method));
	}

	internal static InvalidOperationException ConnectionRequired(string method)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_ConnectionRequired, method));
	}

	internal static InvalidOperationException OpenConnectionRequired(string method, ConnectionState state)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_OpenConnectionRequired, method, ConnectionStateMsg(state)));
	}

	internal static Exception OpenReaderExists()
	{
		return OpenReaderExists(null);
	}

	internal static Exception OpenReaderExists(Exception e)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_OpenReaderExists), e);
	}

	internal static Exception NonSeqByteAccess(long badIndex, long currIndex, string method)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_NonSeqByteAccess, badIndex.ToString(CultureInfo.InvariantCulture), currIndex.ToString(CultureInfo.InvariantCulture), method));
	}

	internal static Exception NumericToDecimalOverflow()
	{
		return InvalidCast(System.SR.GetString(System.SR.ADP_NumericToDecimalOverflow));
	}

	internal static Exception InvalidCommandTimeout(int value)
	{
		return Argument(System.SR.GetString(System.SR.ADP_InvalidCommandTimeout, value.ToString(CultureInfo.InvariantCulture)), "CommandTimeout");
	}

	internal static Exception DeriveParametersNotSupported(IDbCommand value)
	{
		return DataAdapter(System.SR.GetString(System.SR.ADP_DeriveParametersNotSupported, value.GetType().Name, value.CommandType.ToString()));
	}

	internal static Exception UninitializedParameterSize(int index, Type dataType)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_UninitializedParameterSize, index.ToString(CultureInfo.InvariantCulture), dataType.Name));
	}

	internal static InvalidOperationException QuotePrefixNotSet(string method)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_QuotePrefixNotSet, method));
	}

	internal static Exception ConnectionIsDisabled(Exception InnerException)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_ConnectionIsDisabled), InnerException);
	}

	internal static Exception ClosedConnectionError()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_ClosedConnectionError));
	}

	internal static Exception ConnectionAlreadyOpen(ConnectionState state)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_ConnectionAlreadyOpen, ConnectionStateMsg(state)));
	}

	internal static Exception OpenConnectionPropertySet(string property, ConnectionState state)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_OpenConnectionPropertySet, property, ConnectionStateMsg(state)));
	}

	internal static Exception EmptyDatabaseName()
	{
		return Argument(System.SR.GetString(System.SR.ADP_EmptyDatabaseName));
	}

	internal static Exception DatabaseNameTooLong()
	{
		return Argument(System.SR.GetString(System.SR.ADP_DatabaseNameTooLong));
	}

	internal static Exception InternalConnectionError(ConnectionError internalError)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_InternalConnectionError, (int)internalError));
	}

	internal static Exception DataReaderNoData()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_DataReaderNoData));
	}

	internal static ArgumentException InvalidDataType(TypeCode typecode)
	{
		return Argument(System.SR.GetString(System.SR.ADP_InvalidDataType, typecode.ToString()));
	}

	internal static ArgumentException UnknownDataType(Type dataType)
	{
		return Argument(System.SR.GetString(System.SR.ADP_UnknownDataType, dataType.FullName));
	}

	internal static ArgumentException DbTypeNotSupported(DbType type, Type enumtype)
	{
		return Argument(System.SR.GetString(System.SR.ADP_DbTypeNotSupported, type.ToString(), enumtype.Name));
	}

	internal static ArgumentException UnknownDataTypeCode(Type dataType, TypeCode typeCode)
	{
		string aDP_UnknownDataTypeCode = System.SR.ADP_UnknownDataTypeCode;
		object[] array = new object[2];
		int num = (int)typeCode;
		array[0] = num.ToString(CultureInfo.InvariantCulture);
		array[1] = dataType.FullName;
		return Argument(System.SR.GetString(aDP_UnknownDataTypeCode, array));
	}

	internal static ArgumentException InvalidOffsetValue(int value)
	{
		return Argument(System.SR.GetString(System.SR.ADP_InvalidOffsetValue, value.ToString(CultureInfo.InvariantCulture)));
	}

	internal static ArgumentException InvalidSizeValue(int value)
	{
		return Argument(System.SR.GetString(System.SR.ADP_InvalidSizeValue, value.ToString(CultureInfo.InvariantCulture)));
	}

	internal static Exception ParameterConversionFailed(object value, Type destType, Exception inner)
	{
		string @string = System.SR.GetString(System.SR.ADP_ParameterConversionFailed, value.GetType().Name, destType.Name);
		Exception ex = ((inner is ArgumentException) ? new ArgumentException(@string, inner) : ((inner is FormatException) ? new FormatException(@string, inner) : ((inner is InvalidCastException) ? new InvalidCastException(@string, inner) : ((!(inner is OverflowException)) ? inner : new OverflowException(@string, inner)))));
		TraceExceptionAsReturnValue(ex);
		return ex;
	}

	internal static Exception ParametersMappingIndex(int index, IDataParameterCollection collection)
	{
		return CollectionIndexInt32(index, collection.GetType(), collection.Count);
	}

	internal static Exception ParametersSourceIndex(string parameterName, IDataParameterCollection collection, Type parameterType)
	{
		return CollectionIndexString(parameterType, "ParameterName", parameterName, collection.GetType());
	}

	internal static Exception ParameterNull(string parameter, IDataParameterCollection collection, Type parameterType)
	{
		return CollectionNullValue(parameter, collection.GetType(), parameterType);
	}

	internal static Exception InvalidParameterType(IDataParameterCollection collection, Type parameterType, object invalidValue)
	{
		return CollectionInvalidType(collection.GetType(), parameterType, invalidValue);
	}

	internal static Exception ParallelTransactionsNotSupported(IDbConnection obj)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_ParallelTransactionsNotSupported, obj.GetType().Name));
	}

	internal static Exception TransactionZombied(IDbTransaction obj)
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_TransactionZombied, obj.GetType().Name));
	}

	internal static Exception OffsetOutOfRangeException()
	{
		return InvalidOperation(System.SR.GetString(System.SR.ADP_OffsetOutOfRangeException));
	}

	internal static Exception AmbigousCollectionName(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_AmbigousCollectionName, collectionName));
	}

	internal static Exception CollectionNameIsNotUnique(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_CollectionNameISNotUnique, collectionName));
	}

	internal static Exception DataTableDoesNotExist(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_DataTableDoesNotExist, collectionName));
	}

	internal static Exception IncorrectNumberOfDataSourceInformationRows()
	{
		return Argument(System.SR.GetString(System.SR.MDF_IncorrectNumberOfDataSourceInformationRows));
	}

	internal static ArgumentException InvalidRestrictionValue(string collectionName, string restrictionName, string restrictionValue)
	{
		return Argument(System.SR.GetString(System.SR.MDF_InvalidRestrictionValue, collectionName, restrictionName, restrictionValue));
	}

	internal static Exception InvalidXml()
	{
		return Argument(System.SR.GetString(System.SR.MDF_InvalidXml));
	}

	internal static Exception InvalidXmlMissingColumn(string collectionName, string columnName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_InvalidXmlMissingColumn, collectionName, columnName));
	}

	internal static Exception InvalidXmlInvalidValue(string collectionName, string columnName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_InvalidXmlInvalidValue, collectionName, columnName));
	}

	internal static Exception MissingDataSourceInformationColumn()
	{
		return Argument(System.SR.GetString(System.SR.MDF_MissingDataSourceInformationColumn));
	}

	internal static Exception MissingRestrictionColumn()
	{
		return Argument(System.SR.GetString(System.SR.MDF_MissingRestrictionColumn));
	}

	internal static Exception MissingRestrictionRow()
	{
		return Argument(System.SR.GetString(System.SR.MDF_MissingRestrictionRow));
	}

	internal static Exception NoColumns()
	{
		return Argument(System.SR.GetString(System.SR.MDF_NoColumns));
	}

	internal static Exception QueryFailed(string collectionName, Exception e)
	{
		return InvalidOperation(System.SR.GetString(System.SR.MDF_QueryFailed, collectionName), e);
	}

	internal static Exception TooManyRestrictions(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_TooManyRestrictions, collectionName));
	}

	internal static Exception UnableToBuildCollection(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_UnableToBuildCollection, collectionName));
	}

	internal static Exception UndefinedCollection(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_UndefinedCollection, collectionName));
	}

	internal static Exception UndefinedPopulationMechanism(string populationMechanism)
	{
		return Argument(System.SR.GetString(System.SR.MDF_UndefinedPopulationMechanism, populationMechanism));
	}

	internal static Exception UnsupportedVersion(string collectionName)
	{
		return Argument(System.SR.GetString(System.SR.MDF_UnsupportedVersion, collectionName));
	}

	internal static Delegate FindBuilder(MulticastDelegate mcd)
	{
		if ((object)mcd != null)
		{
			Delegate[] invocationList = mcd.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				if (invocationList[i].Target is DbCommandBuilder)
				{
					return invocationList[i];
				}
			}
		}
		return null;
	}

	internal static bool NeedManualEnlistment()
	{
		return false;
	}

	internal static long TimerCurrent()
	{
		return DateTime.UtcNow.ToFileTimeUtc();
	}

	internal static long TimerFromSeconds(int seconds)
	{
		checked
		{
			return unchecked((long)seconds) * 10000000L;
		}
	}

	internal static bool TimerHasExpired(long timerExpire)
	{
		return TimerCurrent() > timerExpire;
	}

	internal static long TimerRemaining(long timerExpire)
	{
		long num = TimerCurrent();
		return checked(timerExpire - num);
	}

	internal static long TimerRemainingMilliseconds(long timerExpire)
	{
		return TimerToMilliseconds(TimerRemaining(timerExpire));
	}

	internal static long TimerToMilliseconds(long timerValue)
	{
		return timerValue / 10000;
	}

	internal static void EscapeSpecialCharacters(string unescapedString, StringBuilder escapedString)
	{
		foreach (char value in unescapedString)
		{
			if (".$^{[(|)*+?\\]".IndexOf(value) >= 0)
			{
				escapedString.Append("\\");
			}
			escapedString.Append(value);
		}
	}

	internal static string GetFullPath(string filename)
	{
		return Path.GetFullPath(filename);
	}

	internal static int StringLength(string inputString)
	{
		return inputString?.Length ?? 0;
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	internal static IntPtr IntPtrOffset(IntPtr pbase, int offset)
	{
		checked
		{
			if (4 == PtrSize)
			{
				return (IntPtr)(pbase.ToInt32() + offset);
			}
			return (IntPtr)(pbase.ToInt64() + offset);
		}
	}

	internal static bool IsEmpty(string str)
	{
		if (str != null)
		{
			return str.Length == 0;
		}
		return true;
	}
}
