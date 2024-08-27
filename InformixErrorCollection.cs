using System;
using System.Collections;
using System.Runtime.CompilerServices;



namespace Arad.Net.Core.Informix;

[Serializable]
[TypeForwardedFrom("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class InformixErrorCollection : ICollection, IEnumerable
{
    private ArrayList _items = new ArrayList();

    object ICollection.SyncRoot => this;

    bool ICollection.IsSynchronized => false;

    public int Count
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _items.Count;
        }
    }

    public InformixError this[int i]
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return (InformixError)_items[i];
        }
    }

    internal InformixErrorCollection()
    {
    }

    internal void Add(InformixError error)
    {
        _items.Add(error);
    }

    public void CopyTo(Array array, int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(array, i);
        ifxTrace?.ApiExit();
        _items.CopyTo(array, i);
    }

    public void CopyTo(InformixError[] array, int i)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(array, i);
        ifxTrace?.ApiExit();
        _items.CopyTo(array, i);
    }

    public IEnumerator GetEnumerator()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return _items.GetEnumerator();
    }

    internal void SetSource(string Source)
    {
        foreach (object item in _items)
        {
            ((InformixError)item).SetSource(Source);
        }
    }
}
