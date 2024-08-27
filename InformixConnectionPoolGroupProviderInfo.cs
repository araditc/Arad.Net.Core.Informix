using Arad.Net.Core.Informix.System.Data.ProviderBase;



namespace Arad.Net.Core.Informix;
internal sealed class InformixConnectionPoolGroupProviderInfo : DbConnectionPoolGroupProviderInfo
{
    private string _driverName;

    private string _driverVersion;

    private string _quoteChar;

    private char _escapeChar;

    private bool _hasQuoteChar;

    private bool _hasEscapeChar;

    private bool _isV3Driver;

    private int _supportedSQLTypes;

    private int _testedSQLTypes;

    private int _restrictedSQLBindTypes;

    private bool _noCurrentCatalog;

    private bool _noConnectionDead;

    private bool _noQueryTimeout;

    private bool _noSqlSoptSSNoBrowseTable;

    private bool _noSqlSoptSSHiddenColumns;

    private bool _noSqlCASSColumnKey;

    private bool _noSqlPrimaryKeys;

    internal string DriverName
    {
        get
        {
            return _driverName;
        }
        set
        {
            _driverName = value;
        }
    }

    internal string DriverVersion
    {
        get
        {
            return _driverVersion;
        }
        set
        {
            _driverVersion = value;
        }
    }

    internal bool HasQuoteChar => _hasQuoteChar;

    internal bool HasEscapeChar => _hasEscapeChar;

    internal string QuoteChar
    {
        get
        {
            return _quoteChar;
        }
        set
        {
            _quoteChar = value;
            _hasQuoteChar = true;
        }
    }

    internal char EscapeChar
    {
        get
        {
            return _escapeChar;
        }
        set
        {
            _escapeChar = value;
            _hasEscapeChar = true;
        }
    }

    internal bool IsV3Driver
    {
        get
        {
            return _isV3Driver;
        }
        set
        {
            _isV3Driver = value;
        }
    }

    internal int SupportedSQLTypes
    {
        get
        {
            return _supportedSQLTypes;
        }
        set
        {
            _supportedSQLTypes = value;
        }
    }

    internal int TestedSQLTypes
    {
        get
        {
            return _testedSQLTypes;
        }
        set
        {
            _testedSQLTypes = value;
        }
    }

    internal int RestrictedSQLBindTypes
    {
        get
        {
            return _restrictedSQLBindTypes;
        }
        set
        {
            _restrictedSQLBindTypes = value;
        }
    }

    internal bool NoCurrentCatalog
    {
        get
        {
            return _noCurrentCatalog;
        }
        set
        {
            _noCurrentCatalog = value;
        }
    }

    internal bool NoConnectionDead
    {
        get
        {
            return _noConnectionDead;
        }
        set
        {
            _noConnectionDead = value;
        }
    }

    internal bool NoQueryTimeout
    {
        get
        {
            return _noQueryTimeout;
        }
        set
        {
            _noQueryTimeout = value;
        }
    }

    internal bool NoSqlSoptSSNoBrowseTable
    {
        get
        {
            return _noSqlSoptSSNoBrowseTable;
        }
        set
        {
            _noSqlSoptSSNoBrowseTable = value;
        }
    }

    internal bool NoSqlSoptSSHiddenColumns
    {
        get
        {
            return _noSqlSoptSSHiddenColumns;
        }
        set
        {
            _noSqlSoptSSHiddenColumns = value;
        }
    }

    internal bool NoSqlCASSColumnKey
    {
        get
        {
            return _noSqlCASSColumnKey;
        }
        set
        {
            _noSqlCASSColumnKey = value;
        }
    }

    internal bool NoSqlPrimaryKeys
    {
        get
        {
            return _noSqlPrimaryKeys;
        }
        set
        {
            _noSqlPrimaryKeys = value;
        }
    }
}
