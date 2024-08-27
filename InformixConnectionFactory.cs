using System.Data.Common;
using Arad.Net.Core.Informix.System.Data.ProviderBase;
using System.IO;
using System.Reflection;
using Arad.Net.Core.Informix.System.Data.Common;


namespace Arad.Net.Core.Informix;
internal sealed class InformixConnectionFactory : DbConnectionFactory
{
    public static readonly InformixConnectionFactory SingletonInstance = new InformixConnectionFactory();

    public override DbProviderFactory ProviderFactory => InformixClientFactory.Instance;

    private InformixConnectionFactory()
    {
    }

    protected override DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningObject)
    {
        return new InformixConnectionOpen(owningObject as InformixConnection, options as InformixConnectionString);
    }

    protected override DbConnectionOptions CreateConnectionOptions(string connectionString, DbConnectionOptions previous)
    {
        return new InformixConnectionString(connectionString, previous != null);
    }

    protected override DbConnectionPoolGroupOptions CreateConnectionPoolGroupOptions(DbConnectionOptions connectionOptions)
    {
        return null;
    }

    internal override DbConnectionPoolGroupProviderInfo CreateConnectionPoolGroupProviderInfo(DbConnectionOptions connectionOptions)
    {
        return new InformixConnectionPoolGroupProviderInfo();
    }

    protected override DbMetaDataFactory CreateMetaDataFactory(DbConnectionInternal internalConnection, out bool cacheMetaDataFactory)
    {
        cacheMetaDataFactory = false;
        InformixConnection outerConnection = ((InformixConnectionOpen)internalConnection).OuterConnection;
        object obj = null;
        string infoStringUnhandled = outerConnection.GetInfoStringUnhandled(Informix32.SQL_INFO.DRIVER_NAME);
        if (infoStringUnhandled != null)
        {
            obj = infoStringUnhandled;
        }
        Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Arad.Net.Core.Informix.OdbcMetaData.xml");
        cacheMetaDataFactory = true;
        string infoStringUnhandled2 = outerConnection.GetInfoStringUnhandled(Informix32.SQL_INFO.DBMS_VER);
        return new OdbcMetaDataFactory(manifestResourceStream, infoStringUnhandled2, infoStringUnhandled2, outerConnection);
    }

    internal override DbConnectionPoolGroup GetConnectionPoolGroup(DbConnection connection)
    {
        if (connection is InformixConnection ifxConnection)
        {
            return ifxConnection.PoolGroup;
        }
        return null;
    }

    internal override DbConnectionInternal GetInnerConnection(DbConnection connection)
    {
        if (connection is InformixConnection ifxConnection)
        {
            return ifxConnection.InnerConnection;
        }
        return null;
    }

    internal override void PermissionDemand(DbConnection outerConnection)
    {
        if (outerConnection is InformixConnection ifxConnection)
        {
            ifxConnection.PermissionDemand();
        }
    }

    internal override void SetConnectionPoolGroup(DbConnection outerConnection, DbConnectionPoolGroup poolGroup)
    {
        if (outerConnection is InformixConnection ifxConnection)
        {
            ifxConnection.PoolGroup = poolGroup;
        }
    }

    internal override void SetInnerConnectionEvent(DbConnection owningObject, DbConnectionInternal to)
    {
        if (owningObject is InformixConnection ifxConnection)
        {
            ifxConnection.SetInnerConnectionEvent(to);
        }
    }

    internal override bool SetInnerConnectionFrom(DbConnection owningObject, DbConnectionInternal to, DbConnectionInternal from)
    {
        if (owningObject is InformixConnection ifxConnection)
        {
            return ifxConnection.SetInnerConnectionFrom(to, from);
        }
        return false;
    }

    internal override void SetInnerConnectionTo(DbConnection owningObject, DbConnectionInternal to)
    {
        if (owningObject is InformixConnection ifxConnection)
        {
            ifxConnection.SetInnerConnectionTo(to);
        }
    }
}
