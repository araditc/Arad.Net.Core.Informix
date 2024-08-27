using Microsoft.Win32;



namespace Arad.Net.Core.Informix;
internal class InformixRegEnumerator
{
    private string[] ifxSQLServers;

    private int itemNumber;

    private InformixDSRecord currentItem;

    public InformixDSRecord CurrentItem
    {
        get
        {
            return currentItem;
        }
        set
        {
            currentItem = value;
        }
    }

    public int InitInstance()
    {
        int result = 0;
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Informix\\SQLHOSTS");
        if (registryKey != null)
        {
            ifxSQLServers = registryKey.GetSubKeyNames();
            result = ifxSQLServers.Length;
        }
        return result;
    }

    public InformixDSRecord MoveNext()
    {
        InformixDSRecord ifxDSRecord = null;
        if (itemNumber < ifxSQLServers.Length)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Informix\\SQLHOSTS\\" + ifxSQLServers[itemNumber]);
            if (registryKey != null)
            {
                object obj = null;
                ifxDSRecord = new InformixDSRecord();
                ifxDSRecord.InformixServer = ifxSQLServers[itemNumber];
                obj = registryKey.GetValue("HOST");
                ifxDSRecord.Host = obj == null ? string.Empty : obj.ToString();
                obj = registryKey.GetValue("OPTIONS");
                ifxDSRecord.Options = obj == null ? string.Empty : obj.ToString();
                obj = registryKey.GetValue("PROTOCOL");
                ifxDSRecord.Protocol = obj == null ? string.Empty : obj.ToString();
                obj = registryKey.GetValue("SERVICE");
                ifxDSRecord.Service = obj == null ? string.Empty : obj.ToString();
                registryKey.Close();
                RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Software\\Informix\\netrc\\" + ifxDSRecord.Host);
                if (registryKey2 != null)
                {
                    obj = registryKey2.GetValue("USER");
                    ifxDSRecord.UserName = obj == null ? string.Empty : obj.ToString();
                    obj = registryKey2.GetValue("AskPassword");
                    ifxDSRecord.PasswordOption = obj == null ? string.Empty : obj.ToString();
                    registryKey2.Close();
                }
            }
            itemNumber++;
        }
        CurrentItem = ifxDSRecord;
        return ifxDSRecord;
    }

    public void Reset()
    {
        itemNumber = 0;
    }
}
