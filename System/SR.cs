using System;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Arad.Net.Core.Informix.System;

internal static class SR
{
	private static ResourceManager s_resourceManager;

	internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(SR)));

	internal static string ADP_CollectionIndexInt32 => GetResourceString("ADP_CollectionIndexInt32");

	internal static string ADP_CollectionIndexString => GetResourceString("ADP_CollectionIndexString");

	internal static string ADP_CollectionInvalidType => GetResourceString("ADP_CollectionInvalidType");

	internal static string ADP_CollectionIsNotParent => GetResourceString("ADP_CollectionIsNotParent");

	internal static string ADP_CollectionIsParent => GetResourceString("ADP_CollectionIsParent");

	internal static string ADP_CollectionNullValue => GetResourceString("ADP_CollectionNullValue");

	internal static string ADP_CollectionRemoveInvalidObject => GetResourceString("ADP_CollectionRemoveInvalidObject");

	internal static string ADP_CollectionUniqueValue => GetResourceString("ADP_CollectionUniqueValue");

	internal static string ADP_ConnectionAlreadyOpen => GetResourceString("ADP_ConnectionAlreadyOpen");

	internal static string ADP_ConnectionStateMsg_Closed => GetResourceString("ADP_ConnectionStateMsg_Closed");

	internal static string ADP_ConnectionStateMsg_Connecting => GetResourceString("ADP_ConnectionStateMsg_Connecting");

	internal static string ADP_ConnectionStateMsg_Open => GetResourceString("ADP_ConnectionStateMsg_Open");

	internal static string ADP_ConnectionStateMsg_OpenExecuting => GetResourceString("ADP_ConnectionStateMsg_OpenExecuting");

	internal static string ADP_ConnectionStateMsg_OpenFetching => GetResourceString("ADP_ConnectionStateMsg_OpenFetching");

	internal static string ADP_ConnectionStateMsg => GetResourceString("ADP_ConnectionStateMsg");

	internal static string ADP_ConnectionStringSyntax => GetResourceString("ADP_ConnectionStringSyntax");

	internal static string ADP_DataReaderClosed => GetResourceString("ADP_DataReaderClosed");

	internal static string ADP_EmptyString => GetResourceString("ADP_EmptyString");

	internal static string ADP_InternalConnectionError => GetResourceString("ADP_InternalConnectionError");

	internal static string ADP_InvalidDataDirectory => GetResourceString("ADP_InvalidDataDirectory");

	internal static string ADP_InvalidEnumerationValue => GetResourceString("ADP_InvalidEnumerationValue");

	internal static string ADP_InvalidKey => GetResourceString("ADP_InvalidKey");

	internal static string ADP_InvalidOffsetValue => GetResourceString("ADP_InvalidOffsetValue");

	internal static string ADP_InvalidValue => GetResourceString("ADP_InvalidValue");

	internal static string ADP_NoConnectionString => GetResourceString("ADP_NoConnectionString");

	internal static string ADP_OpenConnectionPropertySet => GetResourceString("ADP_OpenConnectionPropertySet");

	internal static string ADP_PooledOpenTimeout => GetResourceString("ADP_PooledOpenTimeout");

	internal static string ADP_NonPooledOpenTimeout => GetResourceString("ADP_NonPooledOpenTimeout");

	internal static string ADP_QuotePrefixNotSet => GetResourceString("ADP_QuotePrefixNotSet");

	internal static string MDF_QueryFailed => GetResourceString("MDF_QueryFailed");

	internal static string MDF_TooManyRestrictions => GetResourceString("MDF_TooManyRestrictions");

	internal static string MDF_InvalidRestrictionValue => GetResourceString("MDF_InvalidRestrictionValue");

	internal static string MDF_UndefinedCollection => GetResourceString("MDF_UndefinedCollection");

	internal static string MDF_UndefinedPopulationMechanism => GetResourceString("MDF_UndefinedPopulationMechanism");

	internal static string MDF_UnsupportedVersion => GetResourceString("MDF_UnsupportedVersion");

	internal static string MDF_MissingDataSourceInformationColumn => GetResourceString("MDF_MissingDataSourceInformationColumn");

	internal static string MDF_IncorrectNumberOfDataSourceInformationRows => GetResourceString("MDF_IncorrectNumberOfDataSourceInformationRows");

	internal static string MDF_MissingRestrictionColumn => GetResourceString("MDF_MissingRestrictionColumn");

	internal static string MDF_MissingRestrictionRow => GetResourceString("MDF_MissingRestrictionRow");

	internal static string MDF_NoColumns => GetResourceString("MDF_NoColumns");

	internal static string MDF_UnableToBuildCollection => GetResourceString("MDF_UnableToBuildCollection");

	internal static string MDF_AmbigousCollectionName => GetResourceString("MDF_AmbigousCollectionName");

	internal static string MDF_CollectionNameISNotUnique => GetResourceString("MDF_CollectionNameISNotUnique");

	internal static string MDF_DataTableDoesNotExist => GetResourceString("MDF_DataTableDoesNotExist");

	internal static string MDF_InvalidXml => GetResourceString("MDF_InvalidXml");

	internal static string MDF_InvalidXmlMissingColumn => GetResourceString("MDF_InvalidXmlMissingColumn");

	internal static string MDF_InvalidXmlInvalidValue => GetResourceString("MDF_InvalidXmlInvalidValue");

	internal static string SqlConvert_ConvertFailed => GetResourceString("SqlConvert_ConvertFailed");

	internal static string ADP_InvalidConnectionOptionValue => GetResourceString("ADP_InvalidConnectionOptionValue");

	internal static string ADP_KeywordNotSupported => GetResourceString("ADP_KeywordNotSupported");

	internal static string ADP_InternalProviderError => GetResourceString("ADP_InternalProviderError");

	internal static string ADP_InvalidMultipartName => GetResourceString("ADP_InvalidMultipartName");

	internal static string ADP_InvalidMultipartNameQuoteUsage => GetResourceString("ADP_InvalidMultipartNameQuoteUsage");

	internal static string ADP_InvalidMultipartNameToManyParts => GetResourceString("ADP_InvalidMultipartNameToManyParts");

	internal static string ADP_NotSupportedEnumerationValue => GetResourceString("ADP_NotSupportedEnumerationValue");

	internal static string ADP_StreamClosed => GetResourceString("ADP_StreamClosed");

	internal static string ADP_InvalidSourceBufferIndex => GetResourceString("ADP_InvalidSourceBufferIndex");

	internal static string ADP_InvalidDestinationBufferIndex => GetResourceString("ADP_InvalidDestinationBufferIndex");

	internal static string SQL_InvalidBufferSizeOrIndex => GetResourceString("SQL_InvalidBufferSizeOrIndex");

	internal static string SQL_InvalidDataLength => GetResourceString("SQL_InvalidDataLength");

	internal static string ADP_InvalidSeekOrigin => GetResourceString("ADP_InvalidSeekOrigin");

	internal static string SQL_WrongType => GetResourceString("SQL_WrongType");

	internal static string ODBC_ODBCCommandText => GetResourceString("ODBC_ODBCCommandText");

	internal static string ODBC_NotSupportedEnumerationValue => GetResourceString("ODBC_NotSupportedEnumerationValue");

	internal static string ADP_CommandTextRequired => GetResourceString("ADP_CommandTextRequired");

	internal static string ADP_ConnectionRequired => GetResourceString("ADP_ConnectionRequired");

	internal static string ADP_OpenConnectionRequired => GetResourceString("ADP_OpenConnectionRequired");

	internal static string ADP_TransactionConnectionMismatch => GetResourceString("ADP_TransactionConnectionMismatch");

	internal static string ADP_TransactionRequired => GetResourceString("ADP_TransactionRequired");

	internal static string ADP_OpenReaderExists => GetResourceString("ADP_OpenReaderExists");

	internal static string ADP_DeriveParametersNotSupported => GetResourceString("ADP_DeriveParametersNotSupported");

	internal static string ADP_InvalidCommandTimeout => GetResourceString("ADP_InvalidCommandTimeout");

	internal static string ADP_UninitializedParameterSize => GetResourceString("ADP_UninitializedParameterSize");

	internal static string ADP_ClosedConnectionError => GetResourceString("ADP_ClosedConnectionError");

	internal static string ADP_ConnectionIsDisabled => GetResourceString("ADP_ConnectionIsDisabled");

	internal static string ADP_EmptyDatabaseName => GetResourceString("ADP_EmptyDatabaseName");

	internal static string ADP_DatabaseNameTooLong => GetResourceString("ADP_DatabaseNameTooLong");

	internal static string ADP_DataReaderNoData => GetResourceString("ADP_DataReaderNoData");

	internal static string ADP_NumericToDecimalOverflow => GetResourceString("ADP_NumericToDecimalOverflow");

	internal static string ADP_InvalidDataType => GetResourceString("ADP_InvalidDataType");

	internal static string ADP_UnknownDataType => GetResourceString("ADP_UnknownDataType");

	internal static string ADP_UnknownDataTypeCode => GetResourceString("ADP_UnknownDataTypeCode");

	internal static string ADP_DbTypeNotSupported => GetResourceString("ADP_DbTypeNotSupported");

	internal static string ADP_InvalidSizeValue => GetResourceString("ADP_InvalidSizeValue");

	internal static string ADP_ParameterConversionFailed => GetResourceString("ADP_ParameterConversionFailed");

	internal static string ADP_ParallelTransactionsNotSupported => GetResourceString("ADP_ParallelTransactionsNotSupported");

	internal static string ADP_TransactionZombied => GetResourceString("ADP_TransactionZombied");

	internal static string ADP_DbRecordReadOnly => GetResourceString("ADP_DbRecordReadOnly");

	internal static string ADP_NonSeqByteAccess => GetResourceString("ADP_NonSeqByteAccess");

	internal static string ADP_OffsetOutOfRangeException => GetResourceString("ADP_OffsetOutOfRangeException");

	internal static string ODBC_GetSchemaRestrictionRequired => GetResourceString("ODBC_GetSchemaRestrictionRequired");

	internal static string ADP_OdbcNoTypesFromProvider => GetResourceString("ADP_OdbcNoTypesFromProvider");

	internal static string OdbcConnection_ConnectionStringTooLong => GetResourceString("OdbcConnection_ConnectionStringTooLong");

	internal static string Odbc_UnknownSQLType => GetResourceString("Odbc_UnknownSQLType");

	internal static string Odbc_NegativeArgument => GetResourceString("Odbc_NegativeArgument");

	internal static string Odbc_CantSetPropertyOnOpenConnection => GetResourceString("Odbc_CantSetPropertyOnOpenConnection");

	internal static string Odbc_NoMappingForSqlTransactionLevel => GetResourceString("Odbc_NoMappingForSqlTransactionLevel");

	internal static string Odbc_CantEnableConnectionpooling => GetResourceString("Odbc_CantEnableConnectionpooling");

	internal static string Odbc_CantAllocateEnvironmentHandle => GetResourceString("Odbc_CantAllocateEnvironmentHandle");

	internal static string Odbc_FailedToGetDescriptorHandle => GetResourceString("Odbc_FailedToGetDescriptorHandle");

	internal static string Odbc_NotInTransaction => GetResourceString("Odbc_NotInTransaction");

	internal static string Odbc_ExceptionMessage => GetResourceString("Odbc_ExceptionMessage");

	internal static string Odbc_ConnectionClosed => GetResourceString("Odbc_ConnectionClosed");

	internal static string Odbc_OpenConnectionNoOwner => GetResourceString("Odbc_OpenConnectionNoOwner");

	internal static string Odbc_PlatformNotSupported => GetResourceString("Odbc_PlatformNotSupported");

	internal static string Odbc_UnixOdbcNotFound => GetResourceString("Odbc_UnixOdbcNotFound");

	internal static string GetString(string value)
	{
		return value;
	}

	internal static string GetString(string format, params object[] args)
	{
		return Format(format, args);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool UsingResourceKeys()
	{
		return false;
	}

	internal static string GetResourceString(string resourceKey, string defaultString = null)
	{
		if (UsingResourceKeys())
		{
			return defaultString ?? resourceKey;
		}
		string text = null;
		try
		{
			text = ResourceManager.GetString(resourceKey);
		}
		catch (MissingManifestResourceException)
		{
		}
		if (defaultString != null && resourceKey.Equals(text))
		{
			return defaultString;
		}
		return text;
	}

	internal static string Format(string resourceFormat, object p1)
	{
		if (UsingResourceKeys())
		{
			return string.Join(", ", resourceFormat, p1);
		}
		return string.Format(resourceFormat, p1);
	}

	internal static string Format(string resourceFormat, object p1, object p2)
	{
		if (UsingResourceKeys())
		{
			return string.Join(", ", resourceFormat, p1, p2);
		}
		return string.Format(resourceFormat, p1, p2);
	}

	internal static string Format(string resourceFormat, object p1, object p2, object p3)
	{
		if (UsingResourceKeys())
		{
			return string.Join(", ", resourceFormat, p1, p2, p3);
		}
		return string.Format(resourceFormat, p1, p2, p3);
	}

	internal static string Format(string resourceFormat, params object[] args)
	{
		if (args != null)
		{
			if (UsingResourceKeys())
			{
				return resourceFormat + ", " + string.Join(", ", args);
			}
			return string.Format(resourceFormat, args);
		}
		return resourceFormat;
	}

	internal static string Format(IFormatProvider provider, string resourceFormat, object p1)
	{
		if (UsingResourceKeys())
		{
			return string.Join(", ", resourceFormat, p1);
		}
		return string.Format(provider, resourceFormat, p1);
	}

	internal static string Format(IFormatProvider provider, string resourceFormat, object p1, object p2)
	{
		if (UsingResourceKeys())
		{
			return string.Join(", ", resourceFormat, p1, p2);
		}
		return string.Format(provider, resourceFormat, p1, p2);
	}

	internal static string Format(IFormatProvider provider, string resourceFormat, object p1, object p2, object p3)
	{
		if (UsingResourceKeys())
		{
			return string.Join(", ", resourceFormat, p1, p2, p3);
		}
		return string.Format(provider, resourceFormat, p1, p2, p3);
	}

	internal static string Format(IFormatProvider provider, string resourceFormat, params object[] args)
	{
		if (args != null)
		{
			if (UsingResourceKeys())
			{
				return resourceFormat + ", " + string.Join(", ", args);
			}
			return string.Format(provider, resourceFormat, args);
		}
		return resourceFormat;
	}
}
