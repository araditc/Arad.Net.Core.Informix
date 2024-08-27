using System;
using System.ComponentModel;
using Arad.Net.Core.Informix.System.Data.ProviderBase;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Data.Common;
using System.Data;
using Arad.Net.Core.Informix.System;

using IsolationLevel = System.Data.IsolationLevel;

namespace Arad.Net.Core.Informix;

public sealed class InformixConnection : DbConnection, ICloneable
{
    private int _connectionTimeout = 15;

    internal static readonly InformixPermission IfxPermission = new InformixPermission();

    private InformixInfoMessageEventHandler _infoMessageEventHandler;

    private WeakReference _weakTransaction;

    private InformixConnectionHandle _connectionHandle;

    internal InformixConnSettings connSettingsAtOpen;

    private long objectId;

    private static object CSLock_ConnID = new object();

    private static long ConnObjSerialID = 0L;

    internal string connString = "";

    internal StringBuilder parsedConnString = new StringBuilder("");

    internal int supportedSQLTypes;

    internal int testedSQLTypes;

    internal TypeMap MappingTable = new TypeMap();

    private static Regex parserConnectionString;

    internal ConnectionState state;

    internal bool wasEverOpened;

    internal DBCWrapper _dbcWrapper;

    private CNativeBuffer _buffer;

    private Mutex ConnMutex;

    private ConnectionState _extraState;

    internal InformixConnSettings connSettings = new InformixConnSettings();

    private static readonly DbConnectionFactory s_connectionFactory = InformixConnectionFactory.SingletonInstance;

    private DbConnectionOptions _userConnectionOptions;

    private DbConnectionPoolGroup _poolGroup;

    private DbConnectionInternal _innerConnection;

    private int _closeCount;

    internal InformixConnectionHandle ConnectionHandle
    {
        get
        {
            return _connectionHandle;
        }
        set
        {
            _connectionHandle = value;
        }
    }

    internal long ObjectId => objectId;

    public string UserDefinedTypeFormat
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return connSettings.userDefinedTypeFormat;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            MappingTable.SetUserDefinedTypeMaps(value);
            connSettings.userDefinedTypeFormat = value;
            ifxTrace?.ApiExit();
        }
    }

    public override string ConnectionString
    {
        get
        {
            return ConnectionString_Get();
        }
        set
        {
            if (State != 0)
            {
                throw ADP.OpenConnectionPropertySet("Connection already closed", State);
            }
            if (value != null && value.Length != 0)
            {
                if (1024 < value.Length)
                {
                    throw ODBC.ConnectionStringTooLong();
                }
                InformixConnSettings ifxConnSettings = new InformixConnSettings();
                string value2 = ReplaceConnectionStringParms(value, ref ifxConnSettings);
                connSettings = ifxConnSettings;
                parsedConnString.Length = 0;
                parsedConnString.Append(value2);
            }
            connString = value;
            MappingTable.SetUserDefinedTypeMaps(connSettings.userDefinedTypeFormat);
            supportedSQLTypes = 0;
            testedSQLTypes = 0;
            ConnectionString_Set(connString);
        }
    }

    [DefaultValue(15)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int ConnectionTimeout
    {
        get
        {
            return _connectionTimeout;
        }
        set
        {
            if (value < 0)
            {
                throw ODBC.NegativeArgument();
            }
            if (IsOpen)
            {
                throw ODBC.CantSetPropertyOnOpenConnection();
            }
            _connectionTimeout = value;
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Database
    {
        get
        {
            if (IsOpen && !ProviderInfo.NoCurrentCatalog)
            {
                return GetConnectAttrString(Informix32.SQL_ATTR.CURRENT_CATALOG);
            }
            return string.Empty;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string DataSource
    {
        get
        {
            if (IsOpen)
            {
                return GetInfoStringUnhandled(Informix32.SQL_INFO.SERVER_NAME, handleError: true);
            }
            return string.Empty;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string ServerVersion => InnerConnection.ServerVersion;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Server => GetServerName();

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ServerType
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            string result = null;
            if (IsOpen)
            {
                short cbActual = 0;
                byte[] array = new byte[100];
                InformixConnectionHandle connectionHandle = ConnectionHandle;
                if (connectionHandle != null)
                {
                    Informix32.RetCode info = connectionHandle.GetInfo2(Informix32.SQL_INFO.DBMS_NAME, array, out cbActual);
                    if (array.Length < cbActual - 2)
                    {
                        array = new byte[cbActual + 2];
                        info = connectionHandle.GetInfo2(Informix32.SQL_INFO.DBMS_NAME, array, out cbActual);
                    }
                    if (info == Informix32.RetCode.SUCCESS || info == Informix32.RetCode.SUCCESS_WITH_INFO)
                    {
                        result = Encoding.Unicode.GetString(array, 0, Math.Min(cbActual, array.Length));
                    }
                    else
                    {
                        HandleError(connectionHandle, info);
                    }
                }
                else
                {
                    result = "";
                }
                ifxTrace?.ApiExit();
                return result;
            }
            throw ADP.ClosedConnectionError();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ClientLocale
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return connSettings.cl_loc;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (!IsOpen)
            {
                connSettings.cl_loc = value;
                parsedConnString.Append("cloc=" + value + ";");
                ifxTrace?.ApiExit();
                return;
            }
            throw ADP.OpenConnectionPropertySet(value, State);
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string DatabaseLocale
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return connSettings.db_loc;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (!IsOpen)
            {
                connSettings.db_loc = value;
                int num = parsedConnString.ToString().IndexOf("dloc=");
                if (num >= 0)
                {
                    int num2 = parsedConnString.ToString().IndexOf(";", num);
                    parsedConnString.Remove(num, num2 - num + 1);
                }
                parsedConnString.Append("dloc=" + value + ";");
                ifxTrace?.ApiExit();
                return;
            }
            throw ADP.OpenConnectionPropertySet(value, State);
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PacketSize
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            int fetchBufferSize = GetFetchBufferSize();
            ifxTrace?.ApiExit();
            return fetchBufferSize;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            SetFetchBufferSize(value);
            ifxTrace?.ApiExit();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int FetchBufferSize
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            int fetchBufferSize = GetFetchBufferSize();
            ifxTrace?.ApiExit();
            return fetchBufferSize;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            SetFetchBufferSize(value);
            ifxTrace?.ApiExit();
        }
    }

    public bool Pooling
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            bool pooling = connSettings.pooling;
            ifxTrace?.ApiExit();
            return pooling;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ConnectionState State
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return ConnectionState.Open & state;
        }
    }

    internal InformixConnectionPoolGroupProviderInfo ProviderInfo => (InformixConnectionPoolGroupProviderInfo)PoolGroup.ProviderInfo;

    internal ConnectionState InternalState => State | _extraState;

    internal bool IsOpen => InnerConnection is InformixConnectionOpen;

    internal InformixTransaction LocalTransaction
    {
        get
        {
            InformixTransaction result = null;
            if (_weakTransaction != null)
            {
                result = (InformixTransaction)_weakTransaction.Target;
            }
            return result;
        }
        set
        {
            _weakTransaction = null;
            if (value != null)
            {
                _weakTransaction = new WeakReference(value);
            }
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Driver
    {
        get
        {
            if (IsOpen)
            {
                if (ProviderInfo.DriverName == null)
                {
                    ProviderInfo.DriverName = GetInfoStringUnhandled(Informix32.SQL_INFO.DRIVER_NAME);
                }
                return ProviderInfo.DriverName;
            }
            return "";
        }
    }

    internal bool IsV3Driver
    {
        get
        {
            if (ProviderInfo.DriverVersion == null)
            {
                ProviderInfo.DriverVersion = "03.51";
                if (ProviderInfo.DriverVersion != null && ProviderInfo.DriverVersion.Length >= 2)
                {
                    try
                    {
                        ProviderInfo.IsV3Driver = int.Parse(ProviderInfo.DriverVersion.Substring(0, 2), CultureInfo.InvariantCulture) >= 3;
                    }
                    catch (FormatException e)
                    {
                        ProviderInfo.IsV3Driver = false;
                        ADP.TraceExceptionWithoutRethrow(e);
                    }
                }
                else
                {
                    ProviderInfo.DriverVersion = "";
                }
            }
            return ProviderInfo.IsV3Driver;
        }
    }

    public int GetIdleConnectionsCount
    {
        get
        {
            if (connSettings.pooling)
            {
                OdbcEnvironmentHandle globalEnvironmentHandle = OdbcEnvironment.GetGlobalEnvironmentHandle();
                if (Interop.Odbc.SQLGetEnvAttr(globalEnvironmentHandle, Informix32.SQL_ATTR.SQL_INFX_ATTR_CP_TOTAL_IDLE, out var Value, 4, out var _) != 0)
                {
                    return -1;
                }
                return Value.ToInt32();
            }
            return 0;
        }
    }

    public int GetActiveConnectionsCount
    {
        get
        {
            if (connSettings.pooling)
            {
                OdbcEnvironmentHandle globalEnvironmentHandle = OdbcEnvironment.GetGlobalEnvironmentHandle();
                if (Interop.Odbc.SQLGetEnvAttr(globalEnvironmentHandle, Informix32.SQL_ATTR.SQL_INFX_ATTR_CP_TOTAL_ACTIVE, out var Value, 4, out var _) != 0)
                {
                    return -1;
                }
                return Value.ToInt32();
            }
            return 0;
        }
    }

    internal int CloseCount => _closeCount;

    internal DbConnectionFactory ConnectionFactory => s_connectionFactory;

    internal DbConnectionOptions ConnectionOptions => PoolGroup?.ConnectionOptions;

    internal DbConnectionInternal InnerConnection => _innerConnection;

    internal DbConnectionPoolGroup PoolGroup
    {
        get
        {
            return _poolGroup;
        }
        set
        {
            _poolGroup = value;
        }
    }

    internal DbConnectionOptions UserConnectionOptions => _userConnectionOptions;

    public event InformixInfoMessageEventHandler InfoMessage
    {
        add
        {
            _infoMessageEventHandler = (InformixInfoMessageEventHandler)Delegate.Combine(_infoMessageEventHandler, value);
        }
        remove
        {
            _infoMessageEventHandler = (InformixInfoMessageEventHandler)Delegate.Remove(_infoMessageEventHandler, value);
        }
    }

    private void Initialize()
    {
        connString = "";
        parsedConnString = new StringBuilder("");
    }

    public InformixConnection()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        lock (CSLock_ConnID)
        {
            if (long.MaxValue == ConnObjSerialID)
            {
                ConnObjSerialID = 0L;
            }
            objectId = ++ConnObjSerialID;
        }
        GC.SuppressFinalize(this);
        Initialize();
        ConnMutex = new Mutex();
        ifxTrace?.ApiExit();
        _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
    }

    public InformixConnection(string connectionString)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(connectionString == null ? null : RemoveConnectionStringPassword(connectionString));
        ConnectionString = connectionString;
        ifxTrace?.ApiExit();
    }

    private InformixConnection(InformixConnection connection)
        : this()
    {
        CopyFrom(connection);
        _connectionTimeout = connection._connectionTimeout;
    }

    internal static string RemoveConnectionStringPassword(string value)
    {
        StringBuilder stringBuilder = new StringBuilder("");
        Regex parser = GetParser();
        Match match = parser.Match(value);
        while (match != null && Match.Empty != match)
        {
            if (match.Success)
            {
                string value2 = match.Groups["key"].Value;
                string value3 = match.Groups["value"].Value;
                if ("password" != value2.ToLower(CultureInfo.InvariantCulture) && "pwd" != value2.ToLower(CultureInfo.InvariantCulture))
                {
                    stringBuilder.Append(value2 + "=");
                    stringBuilder.Append(value3 + ";");
                }
            }
            match = match.NextMatch();
        }
        return stringBuilder.ToString();
    }

    internal static Regex GetParser()
    {
        Regex regex = parserConnectionString;
        if (regex == null)
        {
            regex = parserConnectionString;
            if (regex == null)
            {
                regex = parserConnectionString = new Regex("[\\s;]*(?<key>([^=\\s]|\\s+[^=\\s]|\\s+==|==)+)\\s*=(?!=)\\s*(?<value>((\"([^\"]|\"\")*\")|('([^']|'')*')|({([^{]|{})*})|((?![\"'])([^\\s;]|\\s+[^\\s;])*(?<![\"']))))[\\s;]*", RegexOptions.ExplicitCapture);
            }
        }
        return regex;
    }

    internal static string ReplaceConnectionStringParms(string szValue, ref InformixConnSettings connSettings)
    {
        StringBuilder stringBuilder = new StringBuilder("");
        string text = null;
        string text2 = null;
        bool flag = false;
        Regex parser = GetParser();
        Match match = parser.Match(szValue);
        while (match != null && Match.Empty != match)
        {
            if (match.Success)
            {
                string value = match.Groups["key"].Value;
                string text3 = value.ToLower(CultureInfo.InvariantCulture);
                string text4 = match.Groups["value"].Value;
                if (text4.StartsWith("\"") && text4.EndsWith("\"") || text4.StartsWith("'") && text4.EndsWith("'"))
                {
                    text4 = text4.Substring(1, text4.Length - 2);
                }
                string text5 = text4.ToLower(CultureInfo.InvariantCulture).Trim();
                if ("server" == text3)
                {
                    stringBuilder.Append("server=" + text4 + ";");
                    text2 = text4;
                }
                else if ("host" == text3)
                {
                    stringBuilder.Append("host=" + text4 + ";");
                    connSettings.host = text4;
                }
                else if ("service" == text3)
                {
                    stringBuilder.Append("service=" + text4 + ";");
                    connSettings.service = text4;
                }
                else if ("protocol" == text3 || "pro" == text3)
                {
                    stringBuilder.Append("protocol=" + text4 + ";");
                    connSettings.protocol = text4;
                }
                else if ("database" == text3 || "db" == text3)
                {
                    stringBuilder.Append("database=" + text4 + ";");
                    text = text4;
                }
                else if ("user id" == text3 || "uid" == text3)
                {
                    stringBuilder.Append("uid=" + text4 + ";");
                    connSettings.uid = text4;
                }
                else if ("password" == text3 || "pwd" == text3)
                {
                    stringBuilder.Append("pwd=" + text4 + ";");
                    connSettings.pwd = text4;
                }
                else if ("client locale" == text3 || "client_locale" == text3)
                {
                    stringBuilder.Append("cloc=" + text4 + ";");
                    connSettings.cl_loc = text4;
                }
                else if ("database locale" == text3 || "db_locale" == text3)
                {
                    stringBuilder.Append("dloc=" + text4 + ";");
                    connSettings.db_loc = text4;
                }
                else if ("fetch buffer size" == text3 || "fbs" == text3 || "packet size" == text3)
                {
                    if (text4.Length == 0)
                    {
                        connSettings.fbs = 32767;
                    }
                    else
                    {
                        int num;
                        try
                        {
                            num = int.Parse(text4);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException();
                        }
                        if (num < 0)
                        {
                            throw new ArgumentException();
                        }
                        stringBuilder.Append("fetchbuffersize=" + text4 + ";");
                        connSettings.fbs = num;
                    }
                }
                else if ("optimize openfetchclose" == text3 || "optofc" == text3)
                {
                    stringBuilder.Append("optofc=" + text4 + ";");
                    connSettings.optofc = text4;
                }
                else if ("exclusive" == text3 || "xcl" == text3)
                {
                    stringBuilder.Append("exclusive=" + text4 + ";");
                    connSettings.exclusive = text4;
                }
                else if ("connect timeout" == text3 || "timeout" == text3 || "connection timeout" == text3)
                {
                    if (text4.Length != 0)
                    {
                        int num;
                        try
                        {
                            num = int.Parse(text4);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException();
                        }
                        if (num < 0)
                        {
                            throw new ArgumentException();
                        }
                        connSettings.connTimeOut = num;
                    }
                    else
                    {
                        connSettings.connTimeOut = 15;
                    }
                }
                else if ("pooling" == text3)
                {
                    if (text5.Length == 0 || "true" == text5 || "yes" == text5 || "1" == text5)
                    {
                        connSettings.pooling = true;
                    }
                    else
                    {
                        if (!("false" == text5) && !("no" == text5) && !("0" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.pooling = false;
                    }
                }
                else if ("min pool size" == text3 || "minpoolsize" == text3)
                {
                    if (text4.Length == 0)
                    {
                        connSettings.minPoolSize = 0;
                    }
                    else
                    {
                        int num;
                        try
                        {
                            num = int.Parse(text4);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException();
                        }
                        if (num < 0)
                        {
                            throw new ArgumentException();
                        }
                        connSettings.minPoolSize = num;
                        stringBuilder.Append("MinPoolSize=" + num + ";");
                    }
                }
                else if ("max pool size" == text3 || "maxpoolsize" == text3)
                {
                    if (text4.Length == 0)
                    {
                        connSettings.maxPoolSize = 100;
                    }
                    else
                    {
                        int num;
                        try
                        {
                            num = int.Parse(text4);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException();
                        }
                        if (num < 0)
                        {
                            throw new ArgumentException();
                        }
                        connSettings.maxPoolSize = num;
                        stringBuilder.Append("MaxConnLimit=" + num + ";");
                    }
                }
                else if ("connection reset" == text3)
                {
                    if ("true" == text5 || "yes" == text5 || "1" == text5)
                    {
                        connSettings.connReset = true;
                    }
                    else
                    {
                        if (text5.Length != 0 && !("false" == text5) && !("no" == text5) && !("0" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.connReset = false;
                    }
                }
                else if ("connection lifetime" == text3)
                {
                    if (text4.Length == 0)
                    {
                        connSettings.connLifeTime = 0;
                    }
                    else
                    {
                        int num;
                        try
                        {
                            num = int.Parse(text4);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException();
                        }
                        if (num < 0)
                        {
                            throw new ArgumentException();
                        }
                        connSettings.connLifeTime = num;
                    }
                }
                else if ("enlist" == text3)
                {
                    if (text5.Length == 0 || "true" == text5 || "yes" == text5 || "1" == text5)
                    {
                        connSettings.enlist = true;
                    }
                    else
                    {
                        if (!("false" == text5) && !("no" == text5) && !("0" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.enlist = false;
                    }
                }
                else if ("persist security info" == text3)
                {
                    if ("true" == text5 || "yes" == text5 || "1" == text5)
                    {
                        connSettings.persistSecurityInfo = true;
                    }
                    else
                    {
                        if (text5.Length != 0 && !("false" == text5) && !("no" == text5) && !("0" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.persistSecurityInfo = false;
                    }
                }
                else if ("delimident" == text3)
                {
                    flag = true;
                    if (text5.Length == 0 || "true" == text5 || "yes" == text5 || "1" == text5 || "y" == text5)
                    {
                        connSettings.delimIdent = true;
                        stringBuilder.Append(";delimident=y;");
                    }
                    else
                    {
                        if (!("false" == text5) && !("no" == text5) && !("0" == text5) && !("n" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.delimIdent = false;
                        stringBuilder.Append(";delimident=n;");
                    }
                }
                else if ("fetchextendeddatatypesas" == text3 || "userdefinedtypeformat" == text3 || "udtformat" == text3)
                {
                    if (!("string" == text5) && text5.Length != 0 && !("bytes" == text5))
                    {
                        throw new ArgumentException();
                    }
                    connSettings.userDefinedTypeFormat = text5;
                }
                else if ("skip parsing" == text3)
                {
                    if ("true" == text5 || "yes" == text5 || "1" == text5 || "y" == text5)
                    {
                        connSettings.skipParsing = true;
                        stringBuilder.Append(";SKIPP=1;");
                    }
                    else
                    {
                        if (!("false" == text5) && !("no" == text5) && !("0" == text5) && !("n" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.skipParsing = false;
                        stringBuilder.Append(";SKIPP=0;");
                    }
                }
                else if ("single threaded" == text3)
                {
                    if ("true" == text5 || "yes" == text5 || "1" == text5 || "y" == text5)
                    {
                        stringBuilder.Append(";SINGLETH=1;");
                    }
                    else
                    {
                        if (!("false" == text5) && !("no" == text5) && !("0" == text5) && !("n" == text5))
                        {
                            throw new ArgumentException();
                        }
                        stringBuilder.Append(";SINGLETH=0;");
                    }
                }
                else if ("leavetrailingspaces" == text3 || "leave trailing spaces" == text3)
                {
                    if (text5.Length == 0 || "true" == text5 || "yes" == text5 || "1" == text5)
                    {
                        connSettings.leaveTrailingSpaces = true;
                    }
                    else
                    {
                        if (!("false" == text5) && !("no" == text5) && !("0" == text5))
                        {
                            throw new ArgumentException();
                        }
                        connSettings.leaveTrailingSpaces = false;
                    }
                }
                else if ("options" == text3)
                {
                    stringBuilder.Append("OPTIONS=" + text5);
                }
                else if ("dsn" == text3)
                {
                    stringBuilder.Append("dsn=" + text4 + ";");
                    connSettings.dsn = text4;
                }
                else if ("driver" == text3)
                {
                    stringBuilder.Append("driver=" + text4 + ";");
                    connSettings.driver = text4;
                }
                else if ("sqlhosts" == text3)
                {
                    stringBuilder.Append("sqlhosts=" + text4 + ";");
                    connSettings.driver = text4;
                }
                else
                {
                    if (!("connectdatabase" == text3) && !("connect database" == text3))
                    {
                        throw new ArgumentException();
                    }
                    stringBuilder.Append("connectdatabase=" + text4 + ";");
                    connSettings.connectDatabase = text4;
                }
            }
            match = match.NextMatch();
        }
        if (connSettings.dsn.ToString().Equals("") && (text == null || text.Length == 0))
        {
            stringBuilder.Append(";connectdatabase=no;");
        }
        if (!flag)
        {
            stringBuilder.Append(";delimident=y;");
        }
        if (connSettings.dsn.ToString().Equals("") && (text2 == null || text2.Length == 0))
        {
            throw new ArgumentException();
        }
        if (text != null)
        {
            connSettings.database = text;
        }
        if (text2 != null)
        {
            connSettings.server = text2;
        }
        return stringBuilder.ToString();
    }

    internal int GetFetchBufferSize()
    {
        return connSettings.fbs;
    }

    internal void SetFetchBufferSize(int fbs)
    {
        if (!IsOpen)
        {
            connSettings.fbs = fbs;
            parsedConnString.Append("fbs=" + fbs + ";");
            return;
        }
        throw ADP.OpenConnectionPropertySet(fbs.ToString(), State);
    }

    internal string GetServerName()
    {
        string result = string.Empty;
        if (IsOpen)
        {
            short cbActual = 0;
            byte[] array = new byte[100];
            InformixConnectionHandle connectionHandle = ConnectionHandle;
            if (connectionHandle != null)
            {
                Informix32.RetCode info = connectionHandle.GetInfo2(Informix32.SQL_INFO.SERVER_NAME, array, out cbActual);
                if (array.Length < cbActual - 2)
                {
                    array = new byte[cbActual + 2];
                    info = connectionHandle.GetInfo2(Informix32.SQL_INFO.SERVER_NAME, array, out cbActual);
                }
                if (info == Informix32.RetCode.SUCCESS || info == Informix32.RetCode.SUCCESS_WITH_INFO)
                {
                    result = Encoding.Unicode.GetString(array, 0, Math.Min(cbActual, array.Length));
                }
                else
                {
                    HandleError(connectionHandle, info);
                }
            }
            else
            {
                result = "";
            }
            return result;
        }
        return result;
    }

    internal int GetInfo(Informix32.SQL_INFO info)
    {
        nint zero = nint.Zero;
        short pcbInfoValue = 0;
        byte[] rgbInfoValue = new byte[129];
        Informix32.RetCode retCode = ConnectionHandle == null ? Informix32.RetCode.INVALID_HANDLE : Interop.Odbc.SQLGetInfoW(ConnectionHandle, info, rgbInfoValue, (short)_buffer.Length, out pcbInfoValue);
        if (retCode != 0)
        {
            HandleError(ConnectionHandle, retCode);
        }
        return pcbInfoValue;
    }

    internal char EscapeChar(string method)
    {
        CheckState(method);
        if (!ProviderInfo.HasEscapeChar)
        {
            string infoStringUnhandled = GetInfoStringUnhandled(Informix32.SQL_INFO.SEARCH_PATTERN_ESCAPE);
            ProviderInfo.EscapeChar = infoStringUnhandled.Length == 1 ? infoStringUnhandled[0] : QuoteChar(method)[0];
        }
        return ProviderInfo.EscapeChar;
    }

    internal string QuoteChar(string method)
    {
        CheckState(method);
        if (!ProviderInfo.HasQuoteChar)
        {
            string infoStringUnhandled = GetInfoStringUnhandled(Informix32.SQL_INFO.IDENTIFIER_QUOTE_CHAR);
            ProviderInfo.QuoteChar = 1 == infoStringUnhandled.Length ? infoStringUnhandled : "\0";
        }
        return ProviderInfo.QuoteChar;
    }

    public new InformixTransaction BeginTransaction()
    {
        return BeginTransaction(IsolationLevel.Unspecified);
    }

    public new InformixTransaction BeginTransaction(IsolationLevel isolevel)
    {
        return (InformixTransaction)InnerConnection.BeginTransaction(isolevel);
    }

    private void RollbackDeadTransaction()
    {
        WeakReference weakTransaction = _weakTransaction;
        if (weakTransaction != null && !weakTransaction.IsAlive)
        {
            _weakTransaction = null;
            ConnectionHandle.CompleteTransaction(1);
        }
    }

    public override void ChangeDatabase(string value)
    {
        InnerConnection.ChangeDatabase(value);
    }

    internal void CheckState(string method)
    {
        ConnectionState internalState = InternalState;
        if (ConnectionState.Open != internalState)
        {
            throw ADP.OpenConnectionRequired(method, internalState);
        }
    }

    object ICloneable.Clone()
    {
        InformixConnection ifxConnection = new InformixConnection(this);
        ifxConnection.connSettings = (InformixConnSettings)connSettings.Clone();
        ifxConnection.connString = connString;
        ifxConnection.parsedConnString = parsedConnString;
        ifxConnection.supportedSQLTypes = supportedSQLTypes;
        ifxConnection.testedSQLTypes = testedSQLTypes;
        ifxConnection.state = ConnectionState.Closed;
        ifxConnection.wasEverOpened = false;
        if (_buffer != null)
        {
            ifxConnection._buffer = new CNativeBuffer(1024);
        }
        if (connSettingsAtOpen != null)
        {
            ifxConnection.connSettingsAtOpen = (InformixConnSettings)connSettingsAtOpen.Clone();
        }
        return ifxConnection;
    }

    internal bool ConnectionIsAlive()
    {
        if (IsOpen)
        {
            if (!ProviderInfo.NoConnectionDead)
            {
                int connectAttr = GetConnectAttr(Informix32.SQL_ATTR.CONNECTION_DEAD, Informix32.HANDLER.IGNORE);
                if (1 == connectAttr)
                {
                    Close();
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public new InformixCommand CreateCommand()
    {
        return new InformixCommand(string.Empty, this);
    }

    internal OdbcStatementHandle CreateStatementHandle()
    {
        return new OdbcStatementHandle(ConnectionHandle);
    }

    public override void Close()
    {
        InnerConnection.CloseConnection(this, ConnectionFactory);
        state = ConnectionState.Closed;
        InformixConnectionHandle connectionHandle = _connectionHandle;
        if (connectionHandle == null)
        {
            return;
        }
        _connectionHandle = null;
        WeakReference weakTransaction = _weakTransaction;
        if (weakTransaction != null)
        {
            _weakTransaction = null;
            IDisposable disposable = weakTransaction.Target as InformixTransaction;
            if (disposable != null && weakTransaction.IsAlive)
            {
                disposable.Dispose();
            }
        }
        connectionHandle.Dispose();
    }

    private void DisposeMe(bool disposing)
    {
    }

    internal string GetConnectAttrString(Informix32.SQL_ATTR attribute)
    {
        string result = "";
        int cbActual = 0;
        byte[] array = new byte[100];
        InformixConnectionHandle connectionHandle = ConnectionHandle;
        if (connectionHandle != null)
        {
            Informix32.RetCode connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out cbActual);
            if (array.Length + 2 <= cbActual)
            {
                array = new byte[cbActual + 2];
                connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out cbActual);
            }
            if (connectionAttribute == Informix32.RetCode.SUCCESS || Informix32.RetCode.SUCCESS_WITH_INFO == connectionAttribute)
            {
                result = Encoding.Unicode.GetString(array, 0, Math.Min(cbActual, array.Length));
            }
            else if (connectionAttribute == Informix32.RetCode.ERROR)
            {
                string diagSqlState = GetDiagSqlState();
                if ("HYC00" == diagSqlState || "HY092" == diagSqlState || "IM001" == diagSqlState)
                {
                    FlagUnsupportedConnectAttr(attribute);
                }
            }
        }
        return result;
    }

    internal int GetConnectAttr(Informix32.SQL_ATTR attribute, Informix32.HANDLER handler)
    {
        int result = -1;
        int cbActual = 0;
        byte[] array = new byte[4];
        InformixConnectionHandle connectionHandle = ConnectionHandle;
        if (connectionHandle != null)
        {
            Informix32.RetCode connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out cbActual);
            if (connectionAttribute == Informix32.RetCode.SUCCESS || Informix32.RetCode.SUCCESS_WITH_INFO == connectionAttribute)
            {
                result = BitConverter.ToInt32(array, 0);
            }
            else
            {
                if (connectionAttribute == Informix32.RetCode.ERROR)
                {
                    string diagSqlState = GetDiagSqlState();
                    if ("HYC00" == diagSqlState || "HY092" == diagSqlState || "IM001" == diagSqlState)
                    {
                        FlagUnsupportedConnectAttr(attribute);
                    }
                }
                if (handler == Informix32.HANDLER.THROW)
                {
                    HandleError(connectionHandle, connectionAttribute);
                }
            }
        }
        return result;
    }

    public InformixClob GetIfxClob(string tabname, string colname)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixClob result = new InformixClob(this, tabname, colname);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixBlob GetIfxBlob(string tabname, string colname)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixBlob result = new InformixBlob(this, tabname, colname);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixClob GetIfxClob()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixClob result = new InformixClob(this);
        ifxTrace?.ApiExit();
        return result;
    }

    public InformixBlob GetIfxBlob()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixBlob result = new InformixBlob(this);
        ifxTrace?.ApiExit();
        return result;
    }

    private string GetDiagSqlState()
    {
        InformixConnectionHandle connectionHandle = ConnectionHandle;
        connectionHandle.GetDiagnosticField(out var sqlState);
        return sqlState;
    }

    internal Informix32.RetCode GetInfoInt16Unhandled(Informix32.SQL_INFO info, out short resultValue)
    {
        byte[] array = new byte[2];
        Informix32.RetCode info2 = ConnectionHandle.GetInfo1(info, array);
        resultValue = BitConverter.ToInt16(array, 0);
        return info2;
    }

    internal Informix32.RetCode GetInfoInt32Unhandled(Informix32.SQL_INFO info, out int resultValue)
    {
        byte[] array = new byte[4];
        Informix32.RetCode info2 = ConnectionHandle.GetInfo1(info, array);
        resultValue = BitConverter.ToInt32(array, 0);
        return info2;
    }

    private int GetInfoInt32Unhandled(Informix32.SQL_INFO infotype)
    {
        byte[] array = new byte[4];
        ConnectionHandle.GetInfo1(infotype, array);
        return BitConverter.ToInt32(array, 0);
    }

    internal string GetInfoStringUnhandled(Informix32.SQL_INFO info)
    {
        return GetInfoStringUnhandled(info, handleError: false);
    }

    private string GetInfoStringUnhandled(Informix32.SQL_INFO info, bool handleError)
    {
        string result = null;
        short cbActual = 0;
        byte[] array = new byte[100];
        InformixConnectionHandle connectionHandle = ConnectionHandle;
        if (connectionHandle != null)
        {
            Informix32.RetCode info2 = connectionHandle.GetInfo2(info, array, out cbActual);
            if (array.Length < cbActual - 2)
            {
                array = new byte[cbActual + 2];
                info2 = connectionHandle.GetInfo2(info, array, out cbActual);
            }
            if (info2 == Informix32.RetCode.SUCCESS || info2 == Informix32.RetCode.SUCCESS_WITH_INFO)
            {
                result = Encoding.Unicode.GetString(array, 0, Math.Min(cbActual, array.Length));
            }
            else if (handleError)
            {
                HandleError(connectionHandle, info2);
            }
        }
        else if (handleError)
        {
            result = "";
        }
        return result;
    }

    internal Exception HandleErrorNoThrow(OdbcHandle hrHandle, Informix32.RetCode retcode)
    {
        switch (retcode)
        {
            case Informix32.RetCode.SUCCESS_WITH_INFO:
                if (_infoMessageEventHandler != null)
                {
                    InformixErrorCollection diagErrors = Informix32.GetDiagErrors(null, hrHandle, retcode);
                    diagErrors.SetSource(Driver);
                    OnInfoMessage(new InformixInfoMessageEventArgs(diagErrors));
                }
                break;
            default:
                {
                    InformixException ex = InformixException.CreateException(Informix32.GetDiagErrors(null, hrHandle, retcode), retcode);
                    ex?.Errors.SetSource(Driver);
                    ConnectionIsAlive();
                    return ex;
                }
            case Informix32.RetCode.SUCCESS:
                break;
        }
        return null;
    }

    internal void HandleError(OdbcHandle hrHandle, Informix32.RetCode retcode)
    {
        Exception ex = HandleErrorNoThrow(hrHandle, retcode);
        if ((uint)retcode > 1u)
        {
            throw ex;
        }
    }

    public override void Open()
    {
        try
        {
            InnerConnection.OpenConnection(this, ConnectionFactory);
        }
        catch (DllNotFoundException ex) when (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new DllNotFoundException(SR.Odbc_UnixOdbcNotFound + Environment.NewLine + ex.Message);
        }
        if (ADP.NeedManualEnlistment())
        {
            EnlistTransaction(Transaction.Current);
        }
    }

    private void OnInfoMessage(InformixInfoMessageEventArgs args)
    {
        if (_infoMessageEventHandler == null)
        {
            return;
        }
        try
        {
            _infoMessageEventHandler(this, args);
        }
        catch (Exception e)
        {
            if (!ADP.IsCatchableOrSecurityExceptionType(e))
            {
                throw;
            }
            ADP.TraceExceptionWithoutRethrow(e);
        }
    }

    public static void ReleaseObjectPool()
    {
        OdbcEnvironment.ReleaseObjectPool();
    }

    internal InformixTransaction SetStateExecuting(string method, InformixTransaction transaction)
    {
        if (_weakTransaction != null)
        {
            InformixTransaction ifxTransaction = _weakTransaction.Target as InformixTransaction;
            if (transaction != ifxTransaction)
            {
                if (transaction == null)
                {
                    throw ADP.TransactionRequired(method);
                }
                if (this != transaction.Connection)
                {
                    throw ADP.TransactionConnectionMismatch();
                }
                transaction = null;
            }
        }
        else if (transaction != null)
        {
            if (transaction.Connection != null)
            {
                throw ADP.TransactionConnectionMismatch();
            }
            transaction = null;
        }
        ConnectionState internalState = InternalState;
        if (ConnectionState.Open != internalState)
        {
            NotifyWeakReference(1);
            internalState = InternalState;
            if (ConnectionState.Open != internalState)
            {
                if ((ConnectionState.Fetching & internalState) != 0)
                {
                    throw ADP.OpenReaderExists();
                }
                throw ADP.OpenConnectionRequired(method, internalState);
            }
        }
        if ((ConnectionState.Open | ConnectionState.Fetching) != internalState)
        {
            internalState |= ConnectionState.Executing;
        }
        else
        {
            internalState = (internalState | ConnectionState.Executing) & ~ConnectionState.Fetching;
        }
        return transaction;
    }

    internal void SetStateExecutingFalse()
    {
        state &= ~ConnectionState.Executing;
    }

    internal void SetStateFetchingTrue()
    {
        state = (state | ConnectionState.Fetching) & ~ConnectionState.Executing;
    }

    internal void SetStateFetchingFalse()
    {
        state &= ~ConnectionState.Fetching;
    }

    internal void SetSupportedType(Informix32.SQL_TYPE sqltype)
    {
        Informix32.SQL_CVT sQL_CVT;
        switch (sqltype)
        {
            case Informix32.SQL_TYPE.NUMERIC:
                sQL_CVT = Informix32.SQL_CVT.NUMERIC;
                break;
            case Informix32.SQL_TYPE.WCHAR:
                sQL_CVT = Informix32.SQL_CVT.WCHAR;
                break;
            case Informix32.SQL_TYPE.WVARCHAR:
                sQL_CVT = Informix32.SQL_CVT.WVARCHAR;
                break;
            case Informix32.SQL_TYPE.WLONGVARCHAR:
                sQL_CVT = Informix32.SQL_CVT.WLONGVARCHAR;
                break;
            default:
                return;
        }
        ProviderInfo.TestedSQLTypes |= (int)sQL_CVT;
        ProviderInfo.SupportedSQLTypes |= (int)sQL_CVT;
    }

    internal void FlagRestrictedSqlBindType(Informix32.SQL_TYPE sqltype)
    {
        Informix32.SQL_CVT sQL_CVT;
        switch (sqltype)
        {
            default:
                return;
            case Informix32.SQL_TYPE.NUMERIC:
                sQL_CVT = Informix32.SQL_CVT.NUMERIC;
                break;
            case Informix32.SQL_TYPE.DECIMAL:
                sQL_CVT = Informix32.SQL_CVT.DECIMAL;
                break;
        }
        ProviderInfo.RestrictedSQLBindTypes |= (int)sQL_CVT;
    }

    internal void FlagUnsupportedConnectAttr(Informix32.SQL_ATTR Attribute)
    {
        switch (Attribute)
        {
            case Informix32.SQL_ATTR.CURRENT_CATALOG:
                ProviderInfo.NoCurrentCatalog = true;
                break;
            case Informix32.SQL_ATTR.CONNECTION_DEAD:
                ProviderInfo.NoConnectionDead = true;
                break;
        }
    }

    internal void FlagUnsupportedStmtAttr(Informix32.SQL_ATTR Attribute)
    {
        switch (Attribute)
        {
            case Informix32.SQL_ATTR.QUERY_TIMEOUT:
                ProviderInfo.NoQueryTimeout = true;
                break;
            case (Informix32.SQL_ATTR)1228:
                ProviderInfo.NoSqlSoptSSNoBrowseTable = true;
                break;
            case Informix32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION:
                ProviderInfo.NoSqlSoptSSHiddenColumns = true;
                break;
        }
    }

    internal void FlagUnsupportedColAttr(Informix32.SQL_DESC v3FieldId, Informix32.SQL_COLUMN v2FieldId)
    {
        if (IsV3Driver)
        {
            switch (v3FieldId)
            {
                case (Informix32.SQL_DESC)1212:
                    ProviderInfo.NoSqlCASSColumnKey = true;
                    break;
                case (Informix32.SQL_DESC)1211:
                    ProviderInfo.NoSqlSoptSSHiddenColumns = true;
                    break;
            }
        }
    }

    internal bool SQLGetFunctions(Informix32.SQL_API odbcFunction)
    {
        InformixConnectionHandle connectionHandle = ConnectionHandle;
        if (connectionHandle != null)
        {
            short fExists;
            Informix32.RetCode functions = connectionHandle.GetFunctions(odbcFunction, out fExists);
            if (functions != 0)
            {
                HandleError(connectionHandle, functions);
            }
            if (fExists == 0)
            {
                return false;
            }
            return true;
        }
        throw ODBC.ConnectionClosed();
    }

    internal bool TestTypeSupport(Informix32.SQL_TYPE sqltype)
    {
        Informix32.SQL_CONVERT infotype;
        Informix32.SQL_CVT sQL_CVT;
        switch (sqltype)
        {
            case Informix32.SQL_TYPE.NUMERIC:
                infotype = Informix32.SQL_CONVERT.NUMERIC;
                sQL_CVT = Informix32.SQL_CVT.NUMERIC;
                break;
            case Informix32.SQL_TYPE.WCHAR:
                infotype = Informix32.SQL_CONVERT.CHAR;
                sQL_CVT = Informix32.SQL_CVT.WCHAR;
                break;
            case Informix32.SQL_TYPE.WVARCHAR:
                infotype = Informix32.SQL_CONVERT.VARCHAR;
                sQL_CVT = Informix32.SQL_CVT.WVARCHAR;
                break;
            case Informix32.SQL_TYPE.WLONGVARCHAR:
                infotype = Informix32.SQL_CONVERT.LONGVARCHAR;
                sQL_CVT = Informix32.SQL_CVT.WLONGVARCHAR;
                break;
            default:
                return false;
        }
        if (((uint)ProviderInfo.TestedSQLTypes & (uint)sQL_CVT) == 0)
        {
            int infoInt32Unhandled = GetInfoInt32Unhandled((Informix32.SQL_INFO)infotype);
            infoInt32Unhandled &= (int)sQL_CVT;
            ProviderInfo.TestedSQLTypes |= (int)sQL_CVT;
            ProviderInfo.SupportedSQLTypes |= infoInt32Unhandled;
        }
        return ((uint)ProviderInfo.SupportedSQLTypes & (uint)sQL_CVT) != 0;
    }

    internal bool TestRestrictedSqlBindType(Informix32.SQL_TYPE sqltype)
    {
        Informix32.SQL_CVT sQL_CVT;
        switch (sqltype)
        {
            case Informix32.SQL_TYPE.NUMERIC:
                sQL_CVT = Informix32.SQL_CVT.NUMERIC;
                break;
            case Informix32.SQL_TYPE.DECIMAL:
                sQL_CVT = Informix32.SQL_CVT.DECIMAL;
                break;
            default:
                return false;
        }
        return ((uint)ProviderInfo.RestrictedSQLBindTypes & (uint)sQL_CVT) != 0;
    }

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        DbTransaction result = InnerConnection.BeginTransaction(isolationLevel);
        GC.KeepAlive(this);
        return result;
    }

    internal InformixTransaction Open_BeginTransaction(IsolationLevel isolevel)
    {
        CheckState("BeginTransaction");
        RollbackDeadTransaction();
        if (_weakTransaction != null && _weakTransaction.IsAlive)
        {
            throw ADP.ParallelTransactionsNotSupported(this);
        }
        switch (isolevel)
        {
            case IsolationLevel.Chaos:
                throw ODBC.NotSupportedIsolationLevel(isolevel);
            default:
                throw ADP.InvalidIsolationLevel(isolevel);
            case IsolationLevel.Unspecified:
            case IsolationLevel.ReadUncommitted:
            case IsolationLevel.ReadCommitted:
            case IsolationLevel.RepeatableRead:
            case IsolationLevel.Serializable:
            case IsolationLevel.Snapshot:
                {
                    InformixConnectionHandle connectionHandle = ConnectionHandle;
                    Informix32.RetCode retCode = connectionHandle.BeginTransaction(ref isolevel);
                    if (retCode == Informix32.RetCode.ERROR)
                    {
                        HandleError(connectionHandle, retCode);
                    }
                    InformixTransaction ifxTransaction = new InformixTransaction(this, isolevel, connectionHandle);
                    _weakTransaction = new WeakReference(ifxTransaction);
                    return ifxTransaction;
                }
        }
    }

    internal void Open_ChangeDatabase(string value)
    {
        CheckState("ChangeDatabase");
        if (value == null || value.Trim().Length == 0)
        {
            throw ADP.EmptyDatabaseName();
        }
        if (1024 < value.Length * 2 + 2)
        {
            throw ADP.DatabaseNameTooLong();
        }
        RollbackDeadTransaction();
        InformixConnectionHandle connectionHandle = ConnectionHandle;
        Informix32.RetCode retCode = connectionHandle.SetConnectionAttribute3(Informix32.SQL_ATTR.CURRENT_CATALOG, value, checked(value.Length * 2));
        if (retCode != 0)
        {
            HandleError(connectionHandle, retCode);
        }
    }

    internal string Open_GetServerVersion()
    {
        return GetInfoStringUnhandled(Informix32.SQL_INFO.DBMS_VER, handleError: true);
    }

    private void CopyFrom(InformixConnection connection)
    {
        ADP.CheckArgumentNull(connection, "connection");
        _userConnectionOptions = connection.UserConnectionOptions;
        _poolGroup = connection.PoolGroup;
        if (DbConnectionClosedNeverOpened.SingletonInstance == connection._innerConnection)
        {
            _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
        }
        else
        {
            _innerConnection = DbConnectionClosedPreviouslyOpened.SingletonInstance;
        }
    }

    private string ConnectionString_Get()
    {
        bool shouldHidePassword = InnerConnection.ShouldHidePassword;
        DbConnectionOptions userConnectionOptions = UserConnectionOptions;
        if (userConnectionOptions == null)
        {
            return "";
        }
        return userConnectionOptions.UsersConnectionString(shouldHidePassword);
    }

    private void ConnectionString_Set(string value)
    {
        DbConnectionPoolKey key = new DbConnectionPoolKey(value);
        ConnectionString_Set(key);
    }

    private void ConnectionString_Set(DbConnectionPoolKey key)
    {
        DbConnectionOptions userConnectionOptions = null;
        DbConnectionPoolGroup connectionPoolGroup = ConnectionFactory.GetConnectionPoolGroup(key, null, ref userConnectionOptions);
        DbConnectionInternal innerConnection = InnerConnection;
        bool flag = innerConnection.AllowSetConnectionString;
        if (flag)
        {
            flag = SetInnerConnectionFrom(DbConnectionClosedBusy.SingletonInstance, innerConnection);
            if (flag)
            {
                _userConnectionOptions = userConnectionOptions;
                _poolGroup = connectionPoolGroup;
                _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
            }
        }
        if (!flag)
        {
            throw ADP.OpenConnectionPropertySet("ConnectionString", innerConnection.State);
        }
    }

    internal void Abort(Exception e)
    {
        DbConnectionInternal innerConnection = _innerConnection;
        if (ConnectionState.Open == innerConnection.State)
        {
            Interlocked.CompareExchange(ref _innerConnection, DbConnectionClosedPreviouslyOpened.SingletonInstance, innerConnection);
            innerConnection.DoomThisConnection();
        }
    }

    internal void AddWeakReference(object value, int tag)
    {
        InnerConnection.AddWeakReference(value, tag);
    }

    protected override DbCommand CreateDbCommand()
    {
        DbCommand dbCommand = null;
        DbProviderFactory providerFactory = ConnectionFactory.ProviderFactory;
        dbCommand = providerFactory.CreateCommand();
        dbCommand.Connection = this;
        return dbCommand;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _userConnectionOptions = null;
            _poolGroup = null;
            Close();
        }
        DisposeMe(disposing);
        base.Dispose(disposing);
    }

    public override DataTable GetSchema()
    {
        return GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);
    }

    public override DataTable GetSchema(string collectionName)
    {
        return GetSchema(collectionName, null);
    }

    public override DataTable GetSchema(string collectionName, string[] restrictionValues)
    {
        return InnerConnection.GetSchema(ConnectionFactory, PoolGroup, this, collectionName, restrictionValues);
    }

    internal void NotifyWeakReference(int message)
    {
        InnerConnection.NotifyWeakReference(message);
    }

    internal void PermissionDemand()
    {
        DbConnectionOptions dbConnectionOptions = PoolGroup?.ConnectionOptions;
        if (dbConnectionOptions == null || dbConnectionOptions.IsEmpty)
        {
            throw ADP.NoConnectionString();
        }
        DbConnectionOptions userConnectionOptions = UserConnectionOptions;
    }

    internal void RemoveWeakReference(object value)
    {
        InnerConnection.RemoveWeakReference(value);
    }

    internal void SetInnerConnectionEvent(DbConnectionInternal to)
    {
        ConnectionState connectionState = _innerConnection.State & ConnectionState.Open;
        ConnectionState connectionState2 = to.State & ConnectionState.Open;
        if (connectionState != connectionState2 && connectionState2 == ConnectionState.Closed)
        {
            _closeCount++;
        }
        _innerConnection = to;
        if (connectionState == ConnectionState.Closed && ConnectionState.Open == connectionState2)
        {
            OnStateChange(DbConnectionInternal.StateChangeOpen);
        }
        else if (ConnectionState.Open == connectionState && connectionState2 == ConnectionState.Closed)
        {
            OnStateChange(DbConnectionInternal.StateChangeClosed);
        }
        else if (connectionState != connectionState2)
        {
            OnStateChange(new StateChangeEventArgs(connectionState, connectionState2));
        }
    }

    internal bool SetInnerConnectionFrom(DbConnectionInternal to, DbConnectionInternal from)
    {
        return from == Interlocked.CompareExchange(ref _innerConnection, to, from);
    }

    internal void SetInnerConnectionTo(DbConnectionInternal to)
    {
        _innerConnection = to;
    }
}
