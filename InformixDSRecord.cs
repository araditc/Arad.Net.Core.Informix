
namespace Arad.Net.Core.Informix;
internal class InformixDSRecord
{
    internal string szInformixServer = string.Empty;

    internal string szHost = string.Empty;

    internal string szUserName = string.Empty;

    internal string szPasswordOption = string.Empty;

    internal string szPassword = string.Empty;

    internal string szProtocol = string.Empty;

    internal string szService = string.Empty;

    internal string szOption = string.Empty;

    public string InformixServer
    {
        get
        {
            return szInformixServer;
        }
        set
        {
            szInformixServer = value;
        }
    }

    public string Host
    {
        get
        {
            return szHost;
        }
        set
        {
            szHost = value;
        }
    }

    public string UserName
    {
        get
        {
            return szUserName;
        }
        set
        {
            szUserName = value;
        }
    }

    public string PasswordOption
    {
        get
        {
            return szPasswordOption;
        }
        set
        {
            szPasswordOption = value;
        }
    }

    public string Password
    {
        get
        {
            return szPassword;
        }
        set
        {
            szPassword = value;
        }
    }

    public string Protocol
    {
        get
        {
            return szProtocol;
        }
        set
        {
            szProtocol = value;
        }
    }

    public string Service
    {
        get
        {
            return szService;
        }
        set
        {
            szService = value;
        }
    }

    public string Options
    {
        get
        {
            return szOption;
        }
        set
        {
            szOption = value;
        }
    }
}
