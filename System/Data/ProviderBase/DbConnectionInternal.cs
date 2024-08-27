using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arad.Net.Core.Informix.System.Data.ProviderBase;

internal abstract class DbConnectionInternal
{
	internal static readonly StateChangeEventArgs StateChangeClosed = new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed);

	internal static readonly StateChangeEventArgs StateChangeOpen = new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open);

	private readonly bool _allowSetConnectionString;

	private readonly bool _hidePassword;

	private readonly ConnectionState _state;

	private readonly WeakReference _owningObject = new WeakReference(null, trackResurrection: false);

	private DbConnectionPool _connectionPool;

	private DbReferenceCollection _referenceCollection;

	private int _pooledCount;

	private bool _connectionIsDoomed;

	private bool _cannotBePooled;

	private DateTime _createTime;

	internal bool AllowSetConnectionString => _allowSetConnectionString;

	internal bool CanBePooled => !_connectionIsDoomed && !_cannotBePooled && !_owningObject.IsAlive;

	protected internal bool IsConnectionDoomed => _connectionIsDoomed;

	internal bool IsEmancipated => _pooledCount < 1 && !_owningObject.IsAlive;

	internal bool IsInPool => _pooledCount == 1;

	protected internal object Owner => _owningObject.Target;

	internal DbConnectionPool Pool => _connectionPool;

	protected internal DbReferenceCollection ReferenceCollection => _referenceCollection;

	public abstract string ServerVersion { get; }

	public virtual string ServerVersionNormalized
	{
		get
		{
			throw System.Data.Common.ADP.NotSupported();
		}
	}

	public bool ShouldHidePassword => _hidePassword;

	public ConnectionState State => _state;

	protected DbConnectionInternal()
		: this(ConnectionState.Open, hidePassword: true, allowSetConnectionString: false)
	{
	}

	internal DbConnectionInternal(ConnectionState state, bool hidePassword, bool allowSetConnectionString)
	{
		_allowSetConnectionString = allowSetConnectionString;
		_hidePassword = hidePassword;
		_state = state;
	}

	internal void AddWeakReference(object value, int tag)
	{
		if (_referenceCollection == null)
		{
			_referenceCollection = CreateReferenceCollection();
			if (_referenceCollection == null)
			{
				throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.CreateReferenceCollectionReturnedNull);
			}
		}
		_referenceCollection.Add(value, tag);
	}

	public abstract DbTransaction BeginTransaction(IsolationLevel il);

	public virtual void ChangeDatabase(string value)
	{
		throw System.Data.Common.ADP.MethodNotImplemented("ChangeDatabase");
	}

	internal virtual void PrepareForReplaceConnection()
	{
	}

	protected virtual void PrepareForCloseConnection()
	{
	}

	protected virtual object ObtainAdditionalLocksForClose()
	{
		return null;
	}

	protected virtual void ReleaseAdditionalLocksForClose(object lockToken)
	{
	}

	protected virtual DbReferenceCollection CreateReferenceCollection()
	{
		throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.AttemptingToConstructReferenceCollectionOnStaticObject);
	}

	protected abstract void Deactivate();

	internal void DeactivateConnection()
	{
		if (!_connectionIsDoomed && Pool.UseLoadBalancing && DateTime.UtcNow.Ticks - _createTime.Ticks > Pool.LoadBalanceTimeout.Ticks)
		{
			DoNotPoolThisConnection();
		}
		Deactivate();
	}

	protected internal void DoNotPoolThisConnection()
	{
		_cannotBePooled = true;
	}

	protected internal void DoomThisConnection()
	{
		_connectionIsDoomed = true;
	}

	protected internal virtual DataTable GetSchema(DbConnectionFactory factory, DbConnectionPoolGroup poolGroup, DbConnection outerConnection, string collectionName, string[] restrictions)
	{
		DbMetaDataFactory metaDataFactory = factory.GetMetaDataFactory(poolGroup, this);
		return metaDataFactory.GetSchema(outerConnection, collectionName, restrictions);
	}

	internal void MakeNonPooledObject(object owningObject)
	{
		_connectionPool = null;
		_owningObject.Target = owningObject;
		_pooledCount = -1;
	}

	internal void MakePooledConnection(DbConnectionPool connectionPool)
	{
		_createTime = DateTime.UtcNow;
		_connectionPool = connectionPool;
	}

	internal void NotifyWeakReference(int message)
	{
		ReferenceCollection?.Notify(message);
	}

	internal virtual void OpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory)
	{
		if (!TryOpenConnection(outerConnection, connectionFactory, null, null))
		{
			throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.SynchronousConnectReturnedPending);
		}
	}

	internal virtual bool TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		throw System.Data.Common.ADP.ConnectionAlreadyOpen(State);
	}

	internal virtual bool TryReplaceConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		throw System.Data.Common.ADP.MethodNotImplemented("TryReplaceConnection");
	}

	protected bool TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource<DbConnectionInternal> retry, System.Data.Common.DbConnectionOptions userOptions)
	{
		if (connectionFactory.SetInnerConnectionFrom(outerConnection, DbConnectionClosedConnecting.SingletonInstance, this))
		{
			DbConnectionInternal connection = null;
			try
			{
				connectionFactory.PermissionDemand(outerConnection);
				if (!connectionFactory.TryGetConnection(outerConnection, retry, userOptions, this, out connection))
				{
					return false;
				}
			}
			catch
			{
				connectionFactory.SetInnerConnectionTo(outerConnection, this);
				throw;
			}
			if (connection == null)
			{
				connectionFactory.SetInnerConnectionTo(outerConnection, this);
				throw System.Data.Common.ADP.InternalConnectionError(System.Data.Common.ADP.ConnectionError.GetConnectionReturnsNull);
			}
			connectionFactory.SetInnerConnectionEvent(outerConnection, connection);
		}
		return true;
	}

	internal void PrePush(object expectedOwner)
	{
		if (expectedOwner == null)
		{
			if (_owningObject.Target != null)
			{
				throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.UnpooledObjectHasOwner);
			}
		}
		else if (_owningObject.Target != expectedOwner)
		{
			throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.UnpooledObjectHasWrongOwner);
		}
		if (_pooledCount != 0)
		{
			throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.PushingObjectSecondTime);
		}
		_pooledCount++;
		_owningObject.Target = null;
	}

	internal void PostPop(object newOwner)
	{
		if (_owningObject.Target != null)
		{
			throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.PooledObjectHasOwner);
		}
		_owningObject.Target = newOwner;
		_pooledCount--;
		if (Pool != null)
		{
			if (_pooledCount != 0)
			{
				throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.PooledObjectInPoolMoreThanOnce);
			}
		}
		else if (-1 != _pooledCount)
		{
			throw System.Data.Common.ADP.InternalError(System.Data.Common.ADP.InternalErrorCode.NonPooledObjectUsedMoreThanOnce);
		}
	}

	internal void RemoveWeakReference(object value)
	{
		ReferenceCollection?.Remove(value);
	}

	internal virtual bool IsConnectionAlive(bool throwOnException = false)
	{
		return true;
	}

	protected abstract void Activate();

	internal void ActivateConnection()
	{
		Activate();
	}

	internal virtual void CloseConnection(DbConnection owningObject, DbConnectionFactory connectionFactory)
	{
		if (!connectionFactory.SetInnerConnectionFrom(owningObject, DbConnectionOpenBusy.SingletonInstance, this))
		{
			return;
		}
		lock (this)
		{
			object lockToken = ObtainAdditionalLocksForClose();
			try
			{
				PrepareForCloseConnection();
				DbConnectionPool pool = Pool;
				if (pool != null)
				{
					pool.PutObject(this, owningObject);
					return;
				}
				Deactivate();
				_owningObject.Target = null;
				Dispose();
			}
			finally
			{
				ReleaseAdditionalLocksForClose(lockToken);
				connectionFactory.SetInnerConnectionEvent(owningObject, DbConnectionClosedPreviouslyOpened.SingletonInstance);
			}
		}
	}

	public virtual void Dispose()
	{
		_connectionPool = null;
		_connectionIsDoomed = true;
	}
}
