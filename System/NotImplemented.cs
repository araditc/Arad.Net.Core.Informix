using System;

namespace Arad.Net.Core.Informix.System;

internal static class NotImplemented
{
	internal static Exception ByDesign => new NotImplementedException();

	internal static Exception ByDesignWithMessage(string message)
	{
		return new NotImplementedException(message);
	}
}
