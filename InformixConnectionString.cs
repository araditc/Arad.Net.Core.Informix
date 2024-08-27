using Arad.Net.Core.Informix.System.Data.Common;



namespace Arad.Net.Core.Informix;
internal sealed class InformixConnectionString : DbConnectionOptions
{
    internal static class KEYWORDS
    {
        internal const string Dsn = "Dsn";

        internal const string Driver = "Driver";

        internal const string ClientLocale = "Client Locale";

        internal const string ConnLifetime = "Connection Lifetime";

        internal const string ConnReset = "Connection Reset";

        internal const string ConnTimeout = "Connection Timeout";

        internal const string Database = "Database";

        internal const string DbLocale = "Database Locale";

        internal const string DelimIdent = "Delimident";

        internal const string UserDefinedTypeFormat = "UserDefinedTypeFormat";

        internal const string Enlist = "Enlist";

        internal const string Exclusive = "Exclusive";

        internal const string FetchBufferSize = "Fetch Buffer Size";

        internal const string Host = "Host";

        internal const string MaxPoolSize = "Max Pool Size";

        internal const string MinPoolSize = "Min Pool Size";

        internal const string Optofc = "optofc";

        internal const string PersistSecurityInfo = "Persist Security Info";

        internal const string Pooling = "Pooling";

        internal const string Protocol = "Protocol";

        internal const string Pwd = "Password";

        internal const string Server = "Server";

        internal const string Service = "Service";

        internal const string UID = "User ID";

        internal const string SkipParsing = "Skip Parsing";

        internal const string LeaveTrailingSpaces = "Leave Trailing Spaces";

        internal const string connectDatabase = "Connect Database";

        internal const string Database1 = "Db";

        internal const string UID1 = "UID";

        internal const string Pwd1 = "Pwd";

        internal const string ClientLocale1 = "Client_Locale";

        internal const string DbLocale1 = "Db_Locale";

        internal const string DbLocale2 = "Dblocale";

        internal const string FetchBufferSize1 = "Fbs";

        internal const string FetchBufferSize2 = "Packet Size";

        internal const string Optofc1 = "Optimize Openfetchclose";

        internal const string ConnTimeout1 = "Connect Timeout";

        internal const string ConnTimeout2 = "Timeout";

        internal const string Exclusive1 = "Xcl";

        internal const string MaxPoolSize1 = "MaxPoolSize";

        internal const string MinPoolSize1 = "MinPoolSize";

        internal const string UserDefinedTypeFormat1 = "FetchExtendedDataTypesAs";

        internal const string UserDefinedTypeFormat2 = "UdtFormat";

        internal const string LeaveTrailingSpaces1 = "LeaveTrailingSpaces";

        internal const string connectDatabase1 = "ConnectDatabase";

        internal const int Count = 45;
    }

    internal static class DEFAULT
    {
        internal const string Dsn = "";

        internal const string Driver = "";

        internal const int connTimeOut = 15;

        internal const int connLifeTime = 0;

        internal const bool pooling = true;

        internal const int minPoolSize = 0;

        internal const int maxPoolSize = 100;

        internal const string host = "localhost";

        internal const string uid = "";

        internal const string pwd = "";

        internal const string server = "";

        internal const string service = "";

        internal const string protocol = "onsoctcp";

        internal const string database = "";

        internal const string cl_loc = "en_us.CP1252";

        internal const string db_loc = "";

        internal const string optofc = "";

        internal const string exclusive = "no";

        internal const int fbs = 32767;

        internal const bool connReset = false;

        internal const bool enlist = true;

        internal const bool persistSecurityInfo = false;

        internal const bool delimIdent = true;

        internal const string userDefinedTypeFormat = "string";

        internal const bool skipParsing = false;

        internal const bool leaveTrailingSpaces = false;

        internal const string connectDatabase = "";
    }

    private readonly string _expandedConnectionString;

    internal InformixConnectionString(string connectionString, bool validate)
        : base(connectionString, null, useOdbcRules: true)
    {
        if (!validate)
        {
            string filename = null;
            int position = 0;
            _expandedConnectionString = ExpandDataDirectories(ref filename, ref position);
        }
        if ((validate || _expandedConnectionString == null) && connectionString != null && 1024 < connectionString.Length)
        {
            throw ODBC.ConnectionStringTooLong();
        }
    }
}
