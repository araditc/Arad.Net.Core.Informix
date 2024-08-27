using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Common;
using Arad.Net.Core.Informix.System.Data.Common;


namespace Arad.Net.Core.Informix;
public sealed class InformixConnectionStringBuilder : DbConnectionStringBuilder
{
    private enum Keywords
    {
        Dsn,
        Driver,
        ClientLocale,
        ConnLifetime,
        ConnReset,
        ConnTimeout,
        Database,
        DbLocale,
        DelimIdent,
        UserDefinedTypeFormat,
        Enlist,
        Exclusive,
        FetchBufferSize,
        Host,
        MaxPoolSize,
        MinPoolSize,
        Optofc,
        PersistSecurityInfo,
        Pooling,
        Protocol,
        Pwd,
        Server,
        Service,
        UID,
        SkipParsing,
        LeaveTrailingSpaces,
        ConnectDatabase,
        ClientLocale1,
        ConnTimeout1,
        ConnTimeout2,
        Database1,
        DbLocale1,
        DbLocale2,
        UserDefinedTypeFormat1,
        UserDefinedTypeFormat2,
        Exclusive1,
        FetchBufferSize1,
        FetchBufferSize2,
        MaxPoolSize1,
        MinPoolSize1,
        Optofc1,
        Pwd1,
        UID1,
        LeaveTrailingSpaces1,
        ConnectDatabase1
    }

    private enum SwitchKeywordAction
    {
        SetKeyword,
        Reset,
        GetKeywordValue
    }

    private static StringCollection _keysCollection = null;

    private static readonly string[] s_validKeywords = InitializeKeywords();

    private static readonly Dictionary<string, Keywords> s_keywords = InitializeDictionary();

    private string[] _knownKeywords;

    private string _dsn = "";

    private string _driver = "";

    private int _connTimeOut = 15;

    private int _connLifeTime;

    private bool _pooling = true;

    private int _minPoolSize;

    private int _maxPoolSize = 100;

    private string _host = "localhost";

    private string _uid = "";

    private string _pwd = "";

    private string _server = "";

    private string _service = "";

    private string _protocol = "onsoctcp";

    private string _database = "";

    private string _cl_loc = "en_us.CP1252";

    private string _db_loc = "";

    private string _optofc = "";

    private string _exclusive = "no";

    private int _fbs = 32767;

    private bool _connReset;

    private bool _enlist = true;

    private bool _persistSecurityInfo;

    private bool _delimIdent = true;

    private string _userDefinedTypeFormat = "string";

    private bool _skipParsing;

    private bool _leaveTrailingSpaces;

    private string _connectDatabase = "";

    public override object this[string keyword]
    {
        get
        {
            Keywords index = GetIndex(keyword);
            return GetKeywordValue(index);
        }
        set
        {
            if (value != null)
            {
                if (value is string text && text.Trim().Length == 0)
                {
                    Remove(keyword);
                    return;
                }
                Keywords index = GetIndex(keyword);
                SwitchOnKeywords(index, value, SwitchKeywordAction.SetKeyword);
            }
            else
            {
                Remove(keyword);
            }
        }
    }

    public override ICollection Keys
    {
        get
        {
            if (_keysCollection == null)
            {
                _keysCollection = new StringCollection();
                _keysCollection.AddRange(s_validKeywords);
            }
            return _keysCollection;
        }
    }

    public override bool IsFixedSize => true;

    public override ICollection Values
    {
        get
        {
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < s_validKeywords.Length; i++)
            {
                arrayList.Add(GetKeywordValue((Keywords)i));
            }
            return arrayList;
        }
    }

    [DisplayName("Driver")]
    [RefreshProperties(RefreshProperties.All)]
    public string Driver
    {
        get
        {
            return _driver;
        }
        set
        {
            SetValue("Driver", value);
            _driver = value;
        }
    }

    [DisplayName("Dsn")]
    [RefreshProperties(RefreshProperties.All)]
    public string Dsn
    {
        get
        {
            return _dsn;
        }
        set
        {
            SetValue("Dsn", value);
            _dsn = value;
        }
    }

    [DisplayName("Client Locale")]
    [RefreshProperties(RefreshProperties.All)]
    public string ClientLocale
    {
        get
        {
            return _cl_loc;
        }
        set
        {
            SetValue("Client Locale", value);
            _cl_loc = value;
        }
    }

    [DisplayName("Client_Locale")]
    [RefreshProperties(RefreshProperties.All)]
    public string ClientLocale1
    {
        get
        {
            return _cl_loc;
        }
        set
        {
            SetValue("Client Locale", value);
            _cl_loc = value;
        }
    }

    [DisplayName("Connection Lifetime")]
    [RefreshProperties(RefreshProperties.All)]
    public int ConnLifetime
    {
        get
        {
            return _connLifeTime;
        }
        set
        {
            SetValue("Connection Lifetime", value);
            _connLifeTime = value;
        }
    }

    [DisplayName("Connection Reset")]
    [RefreshProperties(RefreshProperties.All)]
    public bool ConnReset
    {
        get
        {
            return _connReset;
        }
        set
        {
            SetValue("Connection Reset", value);
            _connReset = value;
        }
    }

    [DisplayName("Connection Timeout")]
    [RefreshProperties(RefreshProperties.All)]
    public int ConnTimeout
    {
        get
        {
            return _connTimeOut;
        }
        set
        {
            SetValue("Connection Timeout", value);
            _connTimeOut = value;
        }
    }

    [DisplayName("Connect Timeout")]
    [RefreshProperties(RefreshProperties.All)]
    public int ConnTimeout1
    {
        get
        {
            return _connTimeOut;
        }
        set
        {
            SetValue("Connection Timeout", value);
            _connTimeOut = value;
        }
    }

    [DisplayName("Timeout")]
    [RefreshProperties(RefreshProperties.All)]
    public int ConnTimeout2
    {
        get
        {
            return _connTimeOut;
        }
        set
        {
            SetValue("Connection Timeout", value);
            _connTimeOut = value;
        }
    }

    [DisplayName("Database")]
    [RefreshProperties(RefreshProperties.All)]
    public string Database
    {
        get
        {
            return _database;
        }
        set
        {
            SetValue("Database", value);
            _database = value;
        }
    }

    [DisplayName("Db")]
    [RefreshProperties(RefreshProperties.All)]
    public string Database1
    {
        get
        {
            return _database;
        }
        set
        {
            SetValue("Database", value);
            _database = value;
        }
    }

    [DisplayName("Database Locale")]
    [RefreshProperties(RefreshProperties.All)]
    public string DbLocale
    {
        get
        {
            return _db_loc;
        }
        set
        {
            SetValue("Database Locale", value);
            _db_loc = value;
        }
    }

    [DisplayName("Db_Locale")]
    [RefreshProperties(RefreshProperties.All)]
    public string DbLocale1
    {
        get
        {
            return _db_loc;
        }
        set
        {
            SetValue("Database Locale", value);
            _db_loc = value;
        }
    }

    [DisplayName("Dblocale")]
    [RefreshProperties(RefreshProperties.All)]
    public string DbLocale2
    {
        get
        {
            return _db_loc;
        }
        set
        {
            SetValue("Database Locale", value);
            _db_loc = value;
        }
    }

    [DisplayName("Delimident")]
    [RefreshProperties(RefreshProperties.All)]
    public bool DelimIdent
    {
        get
        {
            return _delimIdent;
        }
        set
        {
            SetValue("Delimident", value);
            _delimIdent = value;
        }
    }

    [DisplayName("UserDefinedTypeFormat")]
    [RefreshProperties(RefreshProperties.All)]
    public string UserDefinedTypeFormat
    {
        get
        {
            return _userDefinedTypeFormat;
        }
        set
        {
            SetValue("UserDefinedTypeFormat", value);
            _userDefinedTypeFormat = value;
        }
    }

    [DisplayName("FetchExtendedDataTypesAs")]
    [RefreshProperties(RefreshProperties.All)]
    public string UserDefinedTypeFormat1
    {
        get
        {
            return _userDefinedTypeFormat;
        }
        set
        {
            SetValue("UserDefinedTypeFormat", value);
            _userDefinedTypeFormat = value;
        }
    }

    [DisplayName("UdtFormat")]
    [RefreshProperties(RefreshProperties.All)]
    public string UserDefinedTypeFormat2
    {
        get
        {
            return _userDefinedTypeFormat;
        }
        set
        {
            SetValue("UserDefinedTypeFormat", value);
            _userDefinedTypeFormat = value;
        }
    }

    [DisplayName("Enlist")]
    [RefreshProperties(RefreshProperties.All)]
    public bool Enlist
    {
        get
        {
            return _enlist;
        }
        set
        {
            SetValue("Enlist", value);
            _enlist = value;
        }
    }

    [DisplayName("Exclusive")]
    [RefreshProperties(RefreshProperties.All)]
    public string Exclusive
    {
        get
        {
            return _exclusive;
        }
        set
        {
            SetValue("Exclusive", value);
            _exclusive = value;
        }
    }

    [DisplayName("Xcl")]
    [RefreshProperties(RefreshProperties.All)]
    public string Exclusive1
    {
        get
        {
            return _exclusive;
        }
        set
        {
            SetValue("Exclusive", value);
            _exclusive = value;
        }
    }

    [DisplayName("Fetch Buffer Size")]
    [RefreshProperties(RefreshProperties.All)]
    public int FetchBufferSize
    {
        get
        {
            return _fbs;
        }
        set
        {
            SetValue("Fetch Buffer Size", value);
            _fbs = value;
        }
    }

    [DisplayName("Fbs")]
    [RefreshProperties(RefreshProperties.All)]
    public int FetchBufferSize1
    {
        get
        {
            return _fbs;
        }
        set
        {
            SetValue("Fetch Buffer Size", value);
            _fbs = value;
        }
    }

    [DisplayName("Packet Size")]
    [RefreshProperties(RefreshProperties.All)]
    public int FetchBufferSize2
    {
        get
        {
            return _fbs;
        }
        set
        {
            SetValue("Fetch Buffer Size", value);
            _fbs = value;
        }
    }

    [DisplayName("Host")]
    [RefreshProperties(RefreshProperties.All)]
    public string Host
    {
        get
        {
            return _host;
        }
        set
        {
            SetValue("Host", value);
            _host = value;
        }
    }

    [DisplayName("Max Pool Size")]
    [RefreshProperties(RefreshProperties.All)]
    public int MaxPoolSize
    {
        get
        {
            return _maxPoolSize;
        }
        set
        {
            SetValue("Max Pool Size", value);
            _maxPoolSize = value;
        }
    }

    [DisplayName("MaxPoolSize")]
    [RefreshProperties(RefreshProperties.All)]
    public int MaxPoolSize1
    {
        get
        {
            return _maxPoolSize;
        }
        set
        {
            SetValue("Max Pool Size", value);
            _maxPoolSize = value;
        }
    }

    [DisplayName("Min Pool Size")]
    [RefreshProperties(RefreshProperties.All)]
    public int MinPoolSize
    {
        get
        {
            return _minPoolSize;
        }
        set
        {
            SetValue("Min Pool Size", value);
            _minPoolSize = value;
        }
    }

    [DisplayName("MinPoolSize")]
    [RefreshProperties(RefreshProperties.All)]
    public int MinPoolSize1
    {
        get
        {
            return _minPoolSize;
        }
        set
        {
            SetValue("Min Pool Size", value);
            _minPoolSize = value;
        }
    }

    [DisplayName("optofc")]
    [RefreshProperties(RefreshProperties.All)]
    public string Optofc
    {
        get
        {
            return _optofc;
        }
        set
        {
            SetValue("optofc", value);
            _optofc = value;
        }
    }

    [DisplayName("Optimize Openfetchclose")]
    [RefreshProperties(RefreshProperties.All)]
    public string Optofc1
    {
        get
        {
            return _optofc;
        }
        set
        {
            SetValue("optofc", value);
            _optofc = value;
        }
    }

    [DisplayName("Persist Security Info")]
    [RefreshProperties(RefreshProperties.All)]
    public bool PersistSecurityInfo
    {
        get
        {
            return _persistSecurityInfo;
        }
        set
        {
            SetValue("Persist Security Info", value);
            _persistSecurityInfo = value;
        }
    }

    [DisplayName("Pooling")]
    [RefreshProperties(RefreshProperties.All)]
    public bool Pooling
    {
        get
        {
            return _pooling;
        }
        set
        {
            SetValue("Pooling", value);
            _pooling = value;
        }
    }

    [DisplayName("Protocol")]
    [RefreshProperties(RefreshProperties.All)]
    public string Protocol
    {
        get
        {
            return _protocol;
        }
        set
        {
            SetValue("Protocol", value);
            _protocol = value;
        }
    }

    [DisplayName("Password")]
    [RefreshProperties(RefreshProperties.All)]
    public string Pwd
    {
        get
        {
            return _pwd;
        }
        set
        {
            SetValue("Password", value);
            _pwd = value;
        }
    }

    [DisplayName("Pwd")]
    [RefreshProperties(RefreshProperties.All)]
    public string Pwd1
    {
        get
        {
            return _pwd;
        }
        set
        {
            SetValue("Password", value);
            _pwd = value;
        }
    }

    [DisplayName("Server")]
    [RefreshProperties(RefreshProperties.All)]
    public string Server
    {
        get
        {
            return _server;
        }
        set
        {
            SetValue("Server", value);
            _server = value;
        }
    }

    [DisplayName("Service")]
    [RefreshProperties(RefreshProperties.All)]
    public string Service
    {
        get
        {
            return _service;
        }
        set
        {
            SetValue("Service", value);
            _service = value;
        }
    }

    [DisplayName("User ID")]
    [RefreshProperties(RefreshProperties.All)]
    public string UID
    {
        get
        {
            return _uid;
        }
        set
        {
            SetValue("User ID", value);
            _uid = value;
        }
    }

    [DisplayName("UID")]
    [RefreshProperties(RefreshProperties.All)]
    public string UID1
    {
        get
        {
            return _uid;
        }
        set
        {
            SetValue("User ID", value);
            _uid = value;
        }
    }

    [DisplayName("Skip Parsing")]
    [RefreshProperties(RefreshProperties.All)]
    public bool SkipParsing
    {
        get
        {
            return _skipParsing;
        }
        set
        {
            SetValue("Skip Parsing", value);
            _skipParsing = value;
        }
    }

    [DisplayName("Leave Trailing Spaces")]
    [RefreshProperties(RefreshProperties.All)]
    public bool LeaveTrailingSpaces
    {
        get
        {
            return _leaveTrailingSpaces;
        }
        set
        {
            SetValue("Leave Trailing Spaces", value);
            _leaveTrailingSpaces = value;
        }
    }

    [DisplayName("LeaveTrailingSpaces")]
    [RefreshProperties(RefreshProperties.All)]
    public bool LeaveTrailingSpaces1
    {
        get
        {
            return _leaveTrailingSpaces;
        }
        set
        {
            SetValue("Leave Trailing Spaces", value);
            _leaveTrailingSpaces = value;
        }
    }

    [DisplayName("Connect Database")]
    [RefreshProperties(RefreshProperties.All)]
    public string ConnectDatabase
    {
        get
        {
            return _connectDatabase;
        }
        set
        {
            SetValue("Connect Database", value);
            _connectDatabase = value;
        }
    }

    [DisplayName("ConnectDatabase")]
    [RefreshProperties(RefreshProperties.All)]
    public string ConnectDatabase1
    {
        get
        {
            return _connectDatabase;
        }
        set
        {
            SetValue("Connect Database", value);
            _connectDatabase = value;
        }
    }

    private static string[] InitializeKeywords()
    {
        string[] array = new string[45];
        array[0] = "Dsn";
        array[1] = "Driver";
        array[2] = "Client Locale";
        array[3] = "Connection Lifetime";
        array[4] = "Connection Reset";
        array[5] = "Connection Timeout";
        array[6] = "Database";
        array[7] = "Database Locale";
        array[8] = "Delimident";
        array[9] = "UserDefinedTypeFormat";
        array[10] = "Enlist";
        array[11] = "Exclusive";
        array[12] = "Fetch Buffer Size";
        array[13] = "Host";
        array[14] = "Max Pool Size";
        array[15] = "Min Pool Size";
        array[16] = "optofc";
        array[17] = "Persist Security Info";
        array[18] = "Pooling";
        array[19] = "Protocol";
        array[20] = "Password";
        array[21] = "Server";
        array[22] = "Service";
        array[23] = "User ID";
        array[24] = "Skip Parsing";
        array[25] = "Leave Trailing Spaces";
        array[26] = "Connect Database";
        array[30] = "Db";
        array[42] = "UID";
        array[41] = "Pwd";
        array[27] = "Client_Locale";
        array[31] = "Db_Locale";
        array[32] = "Dblocale";
        array[36] = "Fbs";
        array[37] = "Packet Size";
        array[40] = "Optimize Openfetchclose";
        array[28] = "Connect Timeout";
        array[29] = "Timeout";
        array[35] = "Xcl";
        array[38] = "MaxPoolSize";
        array[39] = "MinPoolSize";
        array[33] = "FetchExtendedDataTypesAs";
        array[34] = "UdtFormat";
        array[43] = "LeaveTrailingSpaces";
        array[44] = "ConnectDatabase";
        return array;
    }

    private static Dictionary<string, Keywords> InitializeDictionary()
    {
        Dictionary<string, Keywords> dictionary = new Dictionary<string, Keywords>(45, StringComparer.InvariantCultureIgnoreCase);
        dictionary.Add("Dsn", Keywords.Dsn);
        dictionary.Add("Driver", Keywords.Driver);
        dictionary.Add("Client Locale", Keywords.ClientLocale);
        dictionary.Add("Connection Lifetime", Keywords.ConnLifetime);
        dictionary.Add("Connection Reset", Keywords.ConnReset);
        dictionary.Add("Connection Timeout", Keywords.ConnTimeout);
        dictionary.Add("Database", Keywords.Database);
        dictionary.Add("Database Locale", Keywords.DbLocale);
        dictionary.Add("Delimident", Keywords.DelimIdent);
        dictionary.Add("UserDefinedTypeFormat", Keywords.UserDefinedTypeFormat);
        dictionary.Add("Enlist", Keywords.Enlist);
        dictionary.Add("Exclusive", Keywords.Exclusive);
        dictionary.Add("Fetch Buffer Size", Keywords.FetchBufferSize);
        dictionary.Add("Host", Keywords.Host);
        dictionary.Add("Max Pool Size", Keywords.MaxPoolSize);
        dictionary.Add("Min Pool Size", Keywords.MinPoolSize);
        dictionary.Add("optofc", Keywords.Optofc);
        dictionary.Add("Persist Security Info", Keywords.PersistSecurityInfo);
        dictionary.Add("Pooling", Keywords.Pooling);
        dictionary.Add("Protocol", Keywords.Protocol);
        dictionary.Add("Password", Keywords.Pwd);
        dictionary.Add("Server", Keywords.Server);
        dictionary.Add("Service", Keywords.Service);
        dictionary.Add("User ID", Keywords.UID);
        dictionary.Add("Skip Parsing", Keywords.SkipParsing);
        dictionary.Add("Leave Trailing Spaces", Keywords.LeaveTrailingSpaces);
        dictionary.Add("Connect Database", Keywords.ConnectDatabase);
        dictionary.Add("Db", Keywords.Database1);
        dictionary.Add("UID", Keywords.UID1);
        dictionary.Add("Pwd", Keywords.Pwd1);
        dictionary.Add("Client_Locale", Keywords.ClientLocale1);
        dictionary.Add("Db_Locale", Keywords.DbLocale1);
        dictionary.Add("Dblocale", Keywords.DbLocale2);
        dictionary.Add("Fbs", Keywords.FetchBufferSize1);
        dictionary.Add("Packet Size", Keywords.FetchBufferSize2);
        dictionary.Add("Optimize Openfetchclose", Keywords.Optofc1);
        dictionary.Add("Connect Timeout", Keywords.ConnTimeout1);
        dictionary.Add("Timeout", Keywords.ConnTimeout2);
        dictionary.Add("Xcl", Keywords.Exclusive1);
        dictionary.Add("MaxPoolSize", Keywords.MaxPoolSize1);
        dictionary.Add("MinPoolSize", Keywords.MinPoolSize1);
        dictionary.Add("FetchExtendedDataTypesAs", Keywords.UserDefinedTypeFormat1);
        dictionary.Add("UdtFormat", Keywords.UserDefinedTypeFormat2);
        dictionary.Add("LeaveTrailingSpaces", Keywords.LeaveTrailingSpaces1);
        dictionary.Add("ConnectDatabase", Keywords.ConnectDatabase1);
        return dictionary;
    }

    public InformixConnectionStringBuilder()
        : this(null)
    {
    }

    public InformixConnectionStringBuilder(string connectionString)
        : base(useOdbcRules: true)
    {
        SwitchOnKeywords(Keywords.Dsn, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Driver, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ClientLocale, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnLifetime, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnReset, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnTimeout, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Database, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.DbLocale, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.DelimIdent, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.UserDefinedTypeFormat, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Enlist, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Exclusive, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.FetchBufferSize, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Host, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.MaxPoolSize, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.MinPoolSize, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Optofc, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.PersistSecurityInfo, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Pooling, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Protocol, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Pwd, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Server, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Service, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.UID, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.SkipParsing, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.LeaveTrailingSpaces, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnectDatabase, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Database1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.UID1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Pwd1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ClientLocale1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.DbLocale1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.DbLocale2, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.FetchBufferSize1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.FetchBufferSize2, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Optofc1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnTimeout1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnTimeout2, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.Exclusive1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.MaxPoolSize1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.MinPoolSize1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.UserDefinedTypeFormat1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.UserDefinedTypeFormat2, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.LeaveTrailingSpaces1, null, SwitchKeywordAction.Reset);
        SwitchOnKeywords(Keywords.ConnectDatabase1, null, SwitchKeywordAction.Reset);
        if (connectionString != null && connectionString.Length > 0)
        {
            ConnectionString = connectionString;
        }
    }

    public override void Clear()
    {
        base.Clear();
        for (int i = 0; i < s_validKeywords.Length; i++)
        {
            Reset((Keywords)i);
        }
        _knownKeywords = s_validKeywords;
    }

    private object GetAt(Keywords index)
    {
        switch (index)
        {
            case Keywords.Driver:
                return Driver;
            case Keywords.Dsn:
                return Dsn;
            case Keywords.ClientLocale:
            case Keywords.ClientLocale1:
                return ClientLocale;
            case Keywords.ConnLifetime:
                return ConnLifetime;
            case Keywords.ConnReset:
                return ConnReset;
            case Keywords.ConnTimeout:
            case Keywords.ConnTimeout1:
            case Keywords.ConnTimeout2:
                return ConnTimeout;
            case Keywords.Database:
            case Keywords.Database1:
                return Database;
            case Keywords.DbLocale:
            case Keywords.DbLocale1:
            case Keywords.DbLocale2:
                return DbLocale;
            case Keywords.DelimIdent:
                return DelimIdent;
            case Keywords.UserDefinedTypeFormat:
            case Keywords.UserDefinedTypeFormat1:
            case Keywords.UserDefinedTypeFormat2:
                return UserDefinedTypeFormat;
            case Keywords.Enlist:
                return Enlist;
            case Keywords.Exclusive:
            case Keywords.Exclusive1:
                return Exclusive;
            case Keywords.FetchBufferSize:
            case Keywords.FetchBufferSize1:
            case Keywords.FetchBufferSize2:
                return FetchBufferSize;
            case Keywords.Host:
                return Host;
            case Keywords.MaxPoolSize:
            case Keywords.MaxPoolSize1:
                return MaxPoolSize;
            case Keywords.MinPoolSize:
            case Keywords.MinPoolSize1:
                return MinPoolSize;
            case Keywords.Optofc:
            case Keywords.Optofc1:
                return Optofc;
            case Keywords.PersistSecurityInfo:
                return PersistSecurityInfo;
            case Keywords.Pooling:
                return Pooling;
            case Keywords.Protocol:
                return Protocol;
            case Keywords.Pwd:
            case Keywords.Pwd1:
                return Pwd;
            case Keywords.Server:
                return Server;
            case Keywords.Service:
                return Service;
            case Keywords.UID:
            case Keywords.UID1:
                return UID;
            case Keywords.SkipParsing:
                return SkipParsing;
            case Keywords.LeaveTrailingSpaces:
            case Keywords.LeaveTrailingSpaces1:
                return LeaveTrailingSpaces;
            case Keywords.ConnectDatabase:
            case Keywords.ConnectDatabase1:
                return ConnectDatabase;
            default:
                throw ADP.KeywordNotSupported(s_validKeywords[(int)index]);
        }
    }

    public override bool Remove(string keyword)
    {
        if (keyword == null)
        {
            return false;
        }
        if (s_keywords.TryGetValue(keyword, out var value))
        {
            base.Remove(s_validKeywords[(int)value]);
            Reset(value);
            return true;
        }
        return false;
    }

    public override bool ContainsKey(string keyword)
    {
        if (keyword == null)
        {
            return false;
        }
        return s_keywords.ContainsKey(keyword);
    }

    private void Reset(Keywords index)
    {
        SwitchOnKeywords(index, null, SwitchKeywordAction.Reset);
    }

    private void SetValue(string keyword, string value)
    {
        ADP.CheckArgumentNull(value, keyword);
        base[keyword] = value;
    }

    private void SetValue(string keyword, bool value)
    {
        base[keyword] = value.ToString(null);
    }

    private void SetValue(string keyword, int value)
    {
        base[keyword] = value.ToString((IFormatProvider)null);
    }

    private static Keywords GetIndex(string keyword)
    {
        if (keyword != null && s_keywords.TryGetValue(keyword, out var value))
        {
            return value;
        }
        throw new ArgumentException();
    }

    private object GetKeywordValue(Keywords index)
    {
        return SwitchOnKeywords(index, null, SwitchKeywordAction.GetKeywordValue);
    }

    private static string ConvertToString(object value)
    {
        if (value == null)
        {
            throw new ArgumentException();
        }
        if (value is string)
        {
            return value as string;
        }
        return Convert.ToString(value);
    }

    private static int ConvertToInt32(object value)
    {
        if (value == null)
        {
            throw new ArgumentException();
        }
        if (value is int)
        {
            return (int)value;
        }
        return Convert.ToInt32(value);
    }

    private static bool ConvertToBoolean(object value)
    {
        if (value == null)
        {
            throw new ArgumentException();
        }
        if (value is bool)
        {
            return (bool)value;
        }
        return Convert.ToBoolean(value);
    }

    public override bool TryGetValue(string keyword, out object value)
    {
        ADP.CheckArgumentNull(keyword, "keyword");
        if (s_keywords.TryGetValue(keyword, out var value2))
        {
            value = GetAt(value2);
            return true;
        }
        return base.TryGetValue(keyword, out value);
    }

    private object SwitchOnKeywords(Keywords index, object value, SwitchKeywordAction action)
    {
        switch (index)
        {
            case Keywords.Dsn:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Dsn = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _dsn;
                    default:
                        _dsn = "";
                        break;
                }
                break;
            case Keywords.Driver:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Driver = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _driver;
                    default:
                        _driver = "";
                        break;
                }
                break;
            case Keywords.ClientLocale:
            case Keywords.ClientLocale1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        ClientLocale = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _cl_loc;
                    default:
                        _cl_loc = "en_us.CP1252";
                        break;
                }
                break;
            case Keywords.ConnLifetime:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        ConnLifetime = ConvertToInt32(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _connLifeTime;
                    default:
                        _connLifeTime = 0;
                        break;
                }
                break;
            case Keywords.ConnReset:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        ConnReset = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _connReset;
                    default:
                        _connReset = false;
                        break;
                }
                break;
            case Keywords.ConnTimeout:
            case Keywords.ConnTimeout1:
            case Keywords.ConnTimeout2:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        ConnTimeout = ConvertToInt32(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _connTimeOut;
                    default:
                        _connTimeOut = 15;
                        break;
                }
                break;
            case Keywords.Database:
            case Keywords.Database1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Database = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _database;
                    default:
                        _database = "";
                        break;
                }
                break;
            case Keywords.DbLocale:
            case Keywords.DbLocale1:
            case Keywords.DbLocale2:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        DbLocale = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _db_loc;
                    default:
                        _db_loc = "";
                        break;
                }
                break;
            case Keywords.DelimIdent:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        DelimIdent = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _delimIdent;
                    default:
                        _delimIdent = true;
                        break;
                }
                break;
            case Keywords.UserDefinedTypeFormat:
            case Keywords.UserDefinedTypeFormat1:
            case Keywords.UserDefinedTypeFormat2:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        UserDefinedTypeFormat = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _userDefinedTypeFormat;
                    default:
                        _userDefinedTypeFormat = "string";
                        break;
                }
                break;
            case Keywords.Enlist:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Enlist = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _enlist;
                    default:
                        _enlist = true;
                        break;
                }
                break;
            case Keywords.Exclusive:
            case Keywords.Exclusive1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Exclusive = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _exclusive;
                    default:
                        _exclusive = "no";
                        break;
                }
                break;
            case Keywords.FetchBufferSize:
            case Keywords.FetchBufferSize1:
            case Keywords.FetchBufferSize2:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        FetchBufferSize = ConvertToInt32(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _fbs;
                    default:
                        _fbs = 32767;
                        break;
                }
                break;
            case Keywords.Host:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Host = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _host;
                    default:
                        _host = "localhost";
                        break;
                }
                break;
            case Keywords.MaxPoolSize:
            case Keywords.MaxPoolSize1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        MaxPoolSize = ConvertToInt32(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _maxPoolSize;
                    default:
                        _maxPoolSize = 100;
                        break;
                }
                break;
            case Keywords.MinPoolSize:
            case Keywords.MinPoolSize1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        MinPoolSize = ConvertToInt32(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _minPoolSize;
                    default:
                        _minPoolSize = 0;
                        break;
                }
                break;
            case Keywords.Optofc:
            case Keywords.Optofc1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Optofc = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _optofc;
                    default:
                        _optofc = "";
                        break;
                }
                break;
            case Keywords.PersistSecurityInfo:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        PersistSecurityInfo = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _persistSecurityInfo;
                    default:
                        _persistSecurityInfo = false;
                        break;
                }
                break;
            case Keywords.Pooling:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Pooling = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _pooling;
                    default:
                        _pooling = true;
                        break;
                }
                break;
            case Keywords.Protocol:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Protocol = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _protocol;
                    default:
                        _protocol = "onsoctcp";
                        break;
                }
                break;
            case Keywords.Pwd:
            case Keywords.Pwd1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Pwd = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _pwd;
                    default:
                        _pwd = "";
                        break;
                }
                break;
            case Keywords.Server:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Server = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _server;
                    default:
                        _server = "";
                        break;
                }
                break;
            case Keywords.Service:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        Service = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _service;
                    default:
                        _service = "";
                        break;
                }
                break;
            case Keywords.UID:
            case Keywords.UID1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        UID = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _uid;
                    default:
                        _uid = "";
                        break;
                }
                break;
            case Keywords.SkipParsing:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        SkipParsing = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _skipParsing;
                    default:
                        _skipParsing = false;
                        break;
                }
                break;
            case Keywords.LeaveTrailingSpaces:
            case Keywords.LeaveTrailingSpaces1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        LeaveTrailingSpaces = ConvertToBoolean(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _leaveTrailingSpaces;
                    default:
                        _leaveTrailingSpaces = false;
                        break;
                }
                break;
            case Keywords.ConnectDatabase:
            case Keywords.ConnectDatabase1:
                switch (action)
                {
                    case SwitchKeywordAction.SetKeyword:
                        _connectDatabase = ConvertToString(value);
                        break;
                    case SwitchKeywordAction.GetKeywordValue:
                        return _connectDatabase;
                    default:
                        _connectDatabase = "";
                        break;
                }
                break;
            default:
                throw new ArgumentException();
        }
        return null;
    }
}
