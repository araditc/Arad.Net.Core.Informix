using System;
using System.Text;



namespace Arad.Net.Core.Informix;
internal sealed class InformixConnSettings : ICloneable
{
    internal const string DEFAULT_STRING = "";

    internal const int DEFAULT_INTEGER = 0;

    internal const int DEFAULT_FETCH_BUFFER_SIZE = 32767;

    internal const int DEFAULT_MAX_CONN_POOL_SIZE = 100;

    internal const int DEFAULT_CONN_TIMEOUT = 15;

    internal int connTimeOut = 15;

    internal int connLifeTime;

    internal bool pooling;

    internal int minPoolSize;

    internal int maxPoolSize = 100;

    internal string host;

    internal string uid;

    internal string pwd;

    internal string server;

    internal string service;

    internal string protocol;

    internal string database;

    internal string cl_loc;

    internal string db_loc;

    internal string optofc;

    internal string exclusive;

    internal int fbs;

    internal bool connReset;

    internal bool enlist;

    internal bool persistSecurityInfo;

    internal bool delimIdent;

    internal string userDefinedTypeFormat;

    internal bool skipParsing;

    internal bool leaveTrailingSpaces;

    internal string dsn;

    internal string driver;

    internal string connectDatabase;

    internal InformixConnSettings()
    {
        connTimeOut = 15;
        connLifeTime = 0;
        pooling = true;
        minPoolSize = 0;
        maxPoolSize = 100;
        host = "";
        uid = "";
        pwd = "";
        server = "";
        service = "";
        protocol = "";
        database = "";
        cl_loc = "en_us.CP1252";
        db_loc = "en_US.819";
        optofc = "";
        exclusive = "";
        fbs = 32767;
        connReset = false;
        enlist = true;
        persistSecurityInfo = false;
        delimIdent = true;
        userDefinedTypeFormat = "";
        skipParsing = false;
        leaveTrailingSpaces = false;
        dsn = "";
        driver = "";
        connectDatabase = "";
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder(1024);
        stringBuilder.Append("Client Locale=");
        stringBuilder.Append(cl_loc);
        stringBuilder.Append(";Connection Lifetime=");
        stringBuilder.Append(connLifeTime);
        stringBuilder.Append(";Connection Reset=");
        stringBuilder.Append(connReset);
        stringBuilder.Append(";Connection Timeout=");
        stringBuilder.Append(connTimeOut);
        stringBuilder.Append(";Database=");
        stringBuilder.Append(database);
        stringBuilder.Append(";DB Locale=");
        stringBuilder.Append(db_loc);
        stringBuilder.Append(";Delimident=");
        stringBuilder.Append(delimIdent);
        stringBuilder.Append(";UserDefinedTypeFormat=");
        stringBuilder.Append(userDefinedTypeFormat);
        stringBuilder.Append(";Enlist=");
        stringBuilder.Append(enlist);
        stringBuilder.Append(";Exclusive=");
        stringBuilder.Append(exclusive);
        stringBuilder.Append(";Fetch Buffer Size=");
        stringBuilder.Append(fbs);
        stringBuilder.Append(";Host=");
        stringBuilder.Append(host);
        stringBuilder.Append(";Max Pool Size=");
        stringBuilder.Append(maxPoolSize);
        stringBuilder.Append(";Min Pool Size=");
        stringBuilder.Append(minPoolSize);
        stringBuilder.Append(";Optofc=");
        stringBuilder.Append(optofc);
        stringBuilder.Append(";Persist Security Info=");
        stringBuilder.Append(persistSecurityInfo);
        stringBuilder.Append(";Pooling=");
        stringBuilder.Append(pooling);
        stringBuilder.Append(";Protocol=");
        stringBuilder.Append(protocol);
        stringBuilder.Append(";Pwd=");
        stringBuilder.Append(pwd);
        stringBuilder.Append(";Server=");
        stringBuilder.Append(server);
        stringBuilder.Append(";Service=");
        stringBuilder.Append(service);
        stringBuilder.Append(";User ID=");
        stringBuilder.Append(uid);
        stringBuilder.Append(";LeaveTrailingSpaces=");
        stringBuilder.Append(leaveTrailingSpaces);
        stringBuilder.Append(";Dsn=");
        stringBuilder.Append(dsn);
        stringBuilder.Append(";Driver=");
        stringBuilder.Append(driver);
        stringBuilder.Append(";ConnectDatabase=");
        stringBuilder.Append(connectDatabase);
        return stringBuilder.ToString();
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        if (obj == null || typeof(InformixConnSettings) != obj.GetType())
        {
            flag = false;
        }
        else
        {
            InformixConnSettings ifxConnSettings = (InformixConnSettings)obj;
            flag = ifxConnSettings.connTimeOut == connTimeOut && ifxConnSettings.connLifeTime == connLifeTime && ifxConnSettings.pooling == pooling && ifxConnSettings.minPoolSize == minPoolSize && ifxConnSettings.maxPoolSize == maxPoolSize && ifxConnSettings.host == host && ifxConnSettings.uid == uid && ifxConnSettings.pwd == pwd && ifxConnSettings.server == server && ifxConnSettings.service == service && ifxConnSettings.protocol == protocol && ifxConnSettings.database == database && ifxConnSettings.cl_loc == cl_loc && ifxConnSettings.optofc == optofc && ifxConnSettings.exclusive == exclusive && ifxConnSettings.fbs == fbs && ifxConnSettings.connReset == connReset && ifxConnSettings.enlist == enlist && ifxConnSettings.persistSecurityInfo == persistSecurityInfo && ifxConnSettings.delimIdent == delimIdent && ifxConnSettings.userDefinedTypeFormat == userDefinedTypeFormat && ifxConnSettings.leaveTrailingSpaces == leaveTrailingSpaces && ifxConnSettings.dsn == dsn && ifxConnSettings.driver == driver;
            if (flag && ifxConnSettings.db_loc != "" && ifxConnSettings.db_loc != db_loc)
            {
                flag = false;
            }
        }
        return flag;
    }

    public override int GetHashCode()
    {
        return connTimeOut.GetHashCode() ^ connLifeTime.GetHashCode() ^ pooling.GetHashCode() ^ minPoolSize.GetHashCode() ^ maxPoolSize.GetHashCode() ^ host.GetHashCode() ^ uid.GetHashCode() ^ pwd.GetHashCode() ^ server.GetHashCode() ^ service.GetHashCode() ^ protocol.GetHashCode() ^ database.GetHashCode() ^ cl_loc.GetHashCode() ^ optofc.GetHashCode() ^ exclusive.GetHashCode() ^ fbs.GetHashCode() ^ connReset.GetHashCode() ^ enlist.GetHashCode() ^ persistSecurityInfo.GetHashCode() ^ delimIdent.GetHashCode() ^ userDefinedTypeFormat.GetHashCode() ^ leaveTrailingSpaces.GetHashCode() ^ dsn.GetHashCode() ^ driver.GetHashCode();
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
