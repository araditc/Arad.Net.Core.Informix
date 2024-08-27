using System;
using System.Globalization;

namespace Arad.Net.Core.Informix.System.Data.Common;

internal static class DbConnectionStringBuilderUtil
{
	internal static bool ConvertToBoolean(object value)
	{
		if (value is string text)
		{
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "true") || StringComparer.OrdinalIgnoreCase.Equals(text, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "false") || StringComparer.OrdinalIgnoreCase.Equals(text, "no"))
			{
				return false;
			}
			string x = text.Trim();
			if (StringComparer.OrdinalIgnoreCase.Equals(x, "true") || StringComparer.OrdinalIgnoreCase.Equals(x, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(x, "false") || StringComparer.OrdinalIgnoreCase.Equals(x, "no"))
			{
				return false;
			}
			return bool.Parse(text);
		}
		try
		{
			return ((IConvertible)value).ToBoolean(CultureInfo.InvariantCulture);
		}
		catch (InvalidCastException innerException)
		{
			throw System.Data.Common.ADP.ConvertFailed(value.GetType(), typeof(bool), innerException);
		}
	}

	internal static bool ConvertToIntegratedSecurity(object value)
	{
		if (value is string text)
		{
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(text, "true") || StringComparer.OrdinalIgnoreCase.Equals(text, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "false") || StringComparer.OrdinalIgnoreCase.Equals(text, "no"))
			{
				return false;
			}
			string x = text.Trim();
			if (StringComparer.OrdinalIgnoreCase.Equals(x, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(x, "true") || StringComparer.OrdinalIgnoreCase.Equals(x, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(x, "false") || StringComparer.OrdinalIgnoreCase.Equals(x, "no"))
			{
				return false;
			}
			return bool.Parse(text);
		}
		try
		{
			return ((IConvertible)value).ToBoolean(CultureInfo.InvariantCulture);
		}
		catch (InvalidCastException innerException)
		{
			throw System.Data.Common.ADP.ConvertFailed(value.GetType(), typeof(bool), innerException);
		}
	}

	internal static int ConvertToInt32(object value)
	{
		try
		{
			return ((IConvertible)value).ToInt32(CultureInfo.InvariantCulture);
		}
		catch (InvalidCastException innerException)
		{
			throw System.Data.Common.ADP.ConvertFailed(value.GetType(), typeof(int), innerException);
		}
	}

	internal static string ConvertToString(object value)
	{
		try
		{
			return ((IConvertible)value).ToString(CultureInfo.InvariantCulture);
		}
		catch (InvalidCastException innerException)
		{
			throw System.Data.Common.ADP.ConvertFailed(value.GetType(), typeof(string), innerException);
		}
	}
}
