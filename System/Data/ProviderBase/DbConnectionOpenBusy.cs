using System.Data;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal sealed class DbConnectionOpenBusy : DbConnectionBusy
{
	internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionOpenBusy();

	private DbConnectionOpenBusy()
		: base(ConnectionState.Open)
	{
	}
}
