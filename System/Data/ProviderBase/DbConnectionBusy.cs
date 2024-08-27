using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal abstract class DbConnectionBusy : DbConnectionClosed
{
	protected DbConnectionBusy(ConnectionState state)
		: base(state, hidePassword: true, allowSetConnectionString: false)
	{
	}

	internal override bool TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		throw Common.ADP.ConnectionAlreadyOpen(State);
	}
}
