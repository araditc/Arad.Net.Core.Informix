using System.Text;



namespace Arad.Net.Core.Informix;
public class InformixSmartLOBLocator
{
    internal byte[] locator;

    public static int Length => 72;

    public InformixSmartLOBLocator(byte[] locator)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(locator);
        this.locator = locator;
        ifxTrace?.ApiExit();
    }

    public InformixSmartLOBLocator(string locator)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(locator);
        this.locator = new byte[72];
        ifxTrace?.ApiExit();
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        for (int i = 0; i < locator.Length; i++)
        {
            stringBuilder.AppendFormat("{0:X02}", locator[i]);
        }
        return stringBuilder.ToString();
    }

    public byte[] ToBytes()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return locator;
    }
}
