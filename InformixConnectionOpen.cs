using System.Data;
using System.Data.Common;
using Arad.Net.Core.Informix.System.Data.ProviderBase;


namespace Arad.Net.Core.Informix;
internal sealed class InformixConnectionOpen : DbConnectionInternal
{
    internal InformixConnection OuterConnection
    {
        get
        {
            InformixConnection ifxConnection = (InformixConnection)Owner;
            if (ifxConnection == null)
            {
                throw ODBC.OpenConnectionNoOwner();
            }
            return ifxConnection;
        }
    }

    public override string ServerVersion => OuterConnection.Open_GetServerVersion();

    internal InformixConnectionOpen(InformixConnection outerConnection, InformixConnectionString connectionOptions)
    {
        OdbcEnvironmentHandle globalEnvironmentHandle = OdbcEnvironment.GetGlobalEnvironmentHandle();
        if (outerConnection.Pooling)
        {
            Informix32.RetCode retCode = Interop.Odbc.SQLSetEnvAttr(globalEnvironmentHandle, Informix32.SQL_ATTR.SQL_INFX_ATTR_CONNECTION_POOLING, Informix32.SQL_INFX_CP_ON, Informix32.SQL_IS.INTEGER);
            if ((uint)retCode > 1u)
            {
                Dispose();
                throw ODBC.CantEnableConnectionpooling(retCode);
            }
        }
        outerConnection.ConnectionHandle = new InformixConnectionHandle(outerConnection, connectionOptions, globalEnvironmentHandle);
    }

    protected override void Activate()
    {
    }

    public override DbTransaction BeginTransaction(IsolationLevel isolevel)
    {
        return BeginOdbcTransaction(isolevel);
    }

    internal InformixTransaction BeginOdbcTransaction(IsolationLevel isolevel)
    {
        return OuterConnection.Open_BeginTransaction(isolevel);
    }

    public override void ChangeDatabase(string value)
    {
        OuterConnection.Open_ChangeDatabase(value);
    }

    protected override DbReferenceCollection CreateReferenceCollection()
    {
        return new OdbcReferenceCollection();
    }

    protected override void Deactivate()
    {
        NotifyWeakReference(0);
    }
}
