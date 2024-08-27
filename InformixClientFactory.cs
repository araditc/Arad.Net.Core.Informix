using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;


namespace Arad.Net.Core.Informix;
public sealed class InformixClientFactory : DbProviderFactory
{
    public static readonly InformixClientFactory Instance = new InformixClientFactory();

    public override bool CanCreateDataSourceEnumerator => true;

    private InformixClientFactory()
    {
    }

    public override DbCommand CreateCommand()
    {
        return new InformixCommand();
    }

    public override DbCommandBuilder CreateCommandBuilder()
    {
        return new InformixCommandBuilder();
    }

    public override DbConnection CreateConnection()
    {
        return new InformixConnection();
    }

    public override DbConnectionStringBuilder CreateConnectionStringBuilder()
    {
        return new InformixConnectionStringBuilder();
    }

    public override DbDataAdapter CreateDataAdapter()
    {
        return new InformixDataAdapter();
    }

    public override DbParameter CreateParameter()
    {
        return new InformixParameter();
    }

    public override DbDataSourceEnumerator CreateDataSourceEnumerator()
    {
        DbDataSourceEnumerator instance = InformixDataSourceEnumerator.Instance;
        if (instance == null)
        {
            throw new ArgumentNullException();
        }
        return instance;
    }

    public CodeAccessPermission CreatePermission(PermissionState state)
    {
        return new InformixPermission(state);
    }
}
