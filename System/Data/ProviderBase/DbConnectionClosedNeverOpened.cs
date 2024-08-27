using System.Data;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal sealed class DbConnectionClosedNeverOpened : DbConnectionClosed
{
	internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedNeverOpened();

	private DbConnectionClosedNeverOpened()
		: base(ConnectionState.Closed, hidePassword: false, allowSetConnectionString: true)
	{
	}
}
