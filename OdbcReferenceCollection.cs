using Arad.Net.Core.Informix.System.Data.ProviderBase;



namespace Arad.Net.Core.Informix;
internal sealed class OdbcReferenceCollection : DbReferenceCollection
{
    internal const int Closing = 0;

    internal const int Recover = 1;

    internal const int CommandTag = 1;

    public override void Add(object value, int tag)
    {
        AddItem(value, tag);
    }

    protected override void NotifyItem(int message, int tag, object value)
    {
        switch (message)
        {
            case 1:
                if (1 == tag)
                {
                    ((InformixCommand)value).RecoverFromConnection();
                }
                break;
            case 0:
                if (1 == tag)
                {
                    ((InformixCommand)value).CloseFromConnection();
                }
                break;
        }
    }

    public override void Remove(object value)
    {
        RemoveItem(value);
    }
}
