using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal abstract class DbConnectionClosed : DbConnectionInternal
{
	public override string ServerVersion
	{
		get
		{
			throw System.Data.Common.ADP.ClosedConnectionError();
		}
	}

	protected DbConnectionClosed(ConnectionState state, bool hidePassword, bool allowSetConnectionString)
		: base(state, hidePassword, allowSetConnectionString)
	{
	}

	public override DbTransaction BeginTransaction(IsolationLevel il)
	{
		throw System.Data.Common.ADP.ClosedConnectionError();
	}

	public override void ChangeDatabase(string database)
	{
		throw System.Data.Common.ADP.ClosedConnectionError();
	}

	internal override void CloseConnection(DbConnection owningObject, DbConnectionFactory connectionFactory)
	{
	}

	protected override void Deactivate()
	{
		System.Data.Common.ADP.ClosedConnectionError();
	}

	protected internal override DataTable GetSchema(DbConnectionFactory factory, DbConnectionPoolGroup poolGroup, DbConnection outerConnection, string collectionName, string[] restrictions)
	{
		throw System.Data.Common.ADP.ClosedConnectionError();
	}

	protected override DbReferenceCollection CreateReferenceCollection()
	{
		throw System.Data.Common.ADP.ClosedConnectionError();
	}

	internal override bool TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		return TryOpenConnectionInternal(outerConnection, connectionFactory, retry, userOptions);
	}

	protected override void Activate()
	{
		throw System.Data.Common.ADP.ClosedConnectionError();
	}
}
