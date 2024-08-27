namespace Arad.Net.Core.Informix.System.Data.Common;

internal sealed class NameValuePair
{
	private readonly string _name;

	private readonly string _value;

	private readonly int _length;

	private System.Data.Common.NameValuePair _next;

	internal int Length => _length;

	internal string Name => _name;

	internal string Value => _value;

	internal System.Data.Common.NameValuePair Next
	{
		get
		{
			return _next;
		}
		set
		{
			if (_next != null || value == null)
			{
				throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.NameValuePairNext);
			}
			_next = value;
		}
	}

	internal NameValuePair(string name, string value, int length)
	{
		_name = name;
		_value = value;
		_length = length;
	}
}
