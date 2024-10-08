using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal sealed class DbConnectionClosedPreviouslyOpened : DbConnectionClosed
{
	internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedPreviouslyOpened();

	private DbConnectionClosedPreviouslyOpened()
		: base(ConnectionState.Closed, hidePassword: true, allowSetConnectionString: true)
	{
	}

	internal override bool TryReplaceConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		return TryOpenConnection(outerConnection, connectionFactory, retry, userOptions);
	}
}
