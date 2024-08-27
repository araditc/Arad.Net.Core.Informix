using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal sealed class DbConnectionClosedConnecting : DbConnectionBusy
{
	internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedConnecting();

	private DbConnectionClosedConnecting()
		: base(ConnectionState.Connecting)
	{
	}

	internal override void CloseConnection(DbConnection owningObject, DbConnectionFactory connectionFactory)
	{
		connectionFactory.SetInnerConnectionTo(owningObject, DbConnectionClosedPreviouslyOpened.SingletonInstance);
	}

	internal override bool TryReplaceConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		return TryOpenConnection(outerConnection, connectionFactory, retry, userOptions);
	}

	internal override bool TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		if (retry == null || !retry.Task.IsCompleted)
		{
			throw System.Data.Common.ADP.ConnectionAlreadyOpen(base.State);
		}
		DbConnectionInternal result = retry.Task.Result;
		if (result == null)
		{
			connectionFactory.SetInnerConnectionTo(outerConnection, this);
			throw System.Data.Common.ADP.InternalConnectionError(System.Data.Common.ADP.ConnectionError.GetConnectionReturnsNull);
		}
		connectionFactory.SetInnerConnectionEvent(outerConnection, result);
		return true;
	}
}
