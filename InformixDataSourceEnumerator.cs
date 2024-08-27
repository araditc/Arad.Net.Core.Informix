using System.Data;
using System.Data.Common;



namespace Arad.Net.Core.Informix;
public sealed class InformixDataSourceEnumerator : DbDataSourceEnumerator
{
    private const string DATASOURCE_TABLENAME = "InformixDataSources";

    private const string DATABASE_SERVER = "IfxDatabaseServer";

    private const string HOST_COMPUTER = "HostComputer";

    private const string USER_NAME = "UserName";

    private const string PASSWORD_OPTION = "PasswordOption";

    private const string PASSWORD = "Password";

    private const string PROTOCOL = "Protocol";

    private const string SERVICE = "Service";

    private const string OPTION = "Option";

    public static InformixDataSourceEnumerator Instance = new InformixDataSourceEnumerator();

    private InformixDataSourceEnumerator()
    {
    }

    public override DataTable GetDataSources()
    {
        InformixRegEnumerator ifxRegEnumerator = new InformixRegEnumerator();
        ifxRegEnumerator.InitInstance();
        DataTable dataTable = new DataTable("InformixDataSources");
        try
        {
            DataColumnCollection columns = dataTable.Columns;
            columns.Add(new DataColumn("IfxDatabaseServer", typeof(string)));
            columns.Add(new DataColumn("HostComputer", typeof(string)));
            columns.Add(new DataColumn("UserName", typeof(string)));
            columns.Add(new DataColumn("PasswordOption", typeof(string)));
            columns.Add(new DataColumn("Password", typeof(string)));
            columns.Add(new DataColumn("Protocol", typeof(string)));
            columns.Add(new DataColumn("Service", typeof(string)));
            columns.Add(new DataColumn("Option", typeof(string)));
            DataRow dataRow = null;
            InformixDSRecord ifxDSRecord = null;
            while ((ifxDSRecord = ifxRegEnumerator.MoveNext()) != null)
            {
                dataRow = dataTable.NewRow();
                dataRow["IfxDatabaseServer"] = ifxDSRecord.InformixServer;
                dataRow["HostComputer"] = ifxDSRecord.Host;
                dataRow["UserName"] = ifxDSRecord.UserName;
                dataRow["PasswordOption"] = ifxDSRecord.PasswordOption;
                dataRow["Password"] = ifxDSRecord.Password;
                dataRow["Protocol"] = ifxDSRecord.Protocol;
                dataRow["Service"] = ifxDSRecord.Service;
                dataRow["Option"] = ifxDSRecord.Options;
                dataTable.Rows.Add(dataRow);
                dataRow.AcceptChanges();
            }
            dataTable.AcceptChanges();
            return dataTable;
        }
        catch
        {
            return null;
        }
    }
}
