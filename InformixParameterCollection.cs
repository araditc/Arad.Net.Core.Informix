using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using Arad.Net.Core.Informix.System.Data.Common;


namespace Arad.Net.Core.Informix;
public sealed class InformixParameterCollection : DbParameterCollection
{
    private bool _rebindCollection;

    private static Type s_itemType = typeof(InformixParameter);

    private List<InformixParameter> _items;

    internal bool RebindCollection
    {
        get
        {
            return _rebindCollection;
        }
        set
        {
            _rebindCollection = value;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new InformixParameter this[int index]
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return (InformixParameter)GetParameter(index);
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            SetParameter(index, value);
            ifxTrace?.ApiExit();
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new InformixParameter this[string parameterName]
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return (InformixParameter)GetParameter(parameterName);
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            SetParameter(parameterName, value);
            ifxTrace?.ApiExit();
        }
    }

    public override int Count
    {
        get
        {
            if (_items == null)
            {
                return 0;
            }
            return _items.Count;
        }
    }

    private List<InformixParameter> InnerList
    {
        get
        {
            List<InformixParameter> list = _items;
            if (list == null)
            {
                list = _items = new List<InformixParameter>();
            }
            return list;
        }
    }

    public override bool IsFixedSize => ((IList)InnerList).IsFixedSize;

    public override bool IsReadOnly => ((IList)InnerList).IsReadOnly;

    public override bool IsSynchronized => ((ICollection)InnerList).IsSynchronized;

    public override object SyncRoot => ((ICollection)InnerList).SyncRoot;

    internal InformixParameterCollection()
    {
    }

    public InformixParameter Add(InformixParameter value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        Add((object)value);
        ifxTrace?.ApiExit();
        return value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).  https://go.microsoft.com/fwlink/?linkid=14202", false)]
    public InformixParameter Add(string parameterName, object value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, value);
        ifxTrace?.ApiExit();
        return Add(new InformixParameter(parameterName, value));
    }

    public InformixParameter AddWithValue(string parameterName, object value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, value);
        ifxTrace?.ApiExit();
        return Add(new InformixParameter(parameterName, value));
    }

    public InformixParameter Add(string parameterName, InformixType odbcType)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, odbcType);
        ifxTrace?.ApiExit();
        return Add(new InformixParameter(parameterName, odbcType));
    }

    public InformixParameter Add(string parameterName, InformixType odbcType, int size)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, odbcType, size);
        ifxTrace?.ApiExit();
        return Add(new InformixParameter(parameterName, odbcType, size));
    }

    public InformixParameter Add(string parameterName, InformixType odbcType, int size, string sourceColumn)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, odbcType, size, sourceColumn);
        ifxTrace?.ApiExit();
        return Add(new InformixParameter(parameterName, odbcType, size, sourceColumn));
    }

    public void AddRange(InformixParameter[] values)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        AddRange((Array)values);
    }

    internal void Bind(InformixCommand command, CMDWrapper cmdWrapper, CNativeBuffer parameterBuffer)
    {
        for (int i = 0; i < Count; i++)
        {
            this[i].Bind(cmdWrapper.StatementHandle, command, checked((short)(i + 1)), parameterBuffer, allowReentrance: true);
        }
        _rebindCollection = false;
    }

    internal int CalcParameterBufferSize(InformixCommand command)
    {
        int parameterBufferSize = 0;
        for (int i = 0; i < Count; i++)
        {
            if (_rebindCollection)
            {
                this[i].HasChanged = true;
            }
            this[i].PrepareForBind(command, (short)(i + 1), ref parameterBufferSize);
            parameterBufferSize = parameterBufferSize + (nint.Size - 1) & ~(nint.Size - 1);
        }
        return parameterBufferSize;
    }

    internal void ClearBindings()
    {
        for (int i = 0; i < Count; i++)
        {
            this[i].ClearBinding();
        }
    }

    public override bool Contains(string value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(value);
        ifxTrace?.ApiExit();
        return -1 != IndexOf(value);
    }

    public bool Contains(InformixParameter value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(value);
        ifxTrace?.ApiExit();
        return -1 != IndexOf(value);
    }

    public void CopyTo(InformixParameter[] array, int index)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(array, index);
        ifxTrace?.ApiExit();
        CopyTo((Array)array, index);
    }

    private void OnChange()
    {
        _rebindCollection = true;
    }

    internal void GetOutputValues(CMDWrapper cmdWrapper)
    {
        if (!_rebindCollection)
        {
            CNativeBuffer nativeParameterBuffer = cmdWrapper._nativeParameterBuffer;
            for (int i = 0; i < Count; i++)
            {
                this[i].GetOutputValue(nativeParameterBuffer);
            }
        }
    }

    public int IndexOf(InformixParameter value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(value);
        ifxTrace?.ApiExit();
        return IndexOf((object)value);
    }

    public void Insert(int index, InformixParameter value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(index, value);
        ifxTrace?.ApiExit();
        Insert(index, (object)value);
    }

    public void Remove(InformixParameter value)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(value);
        ifxTrace?.ApiExit();
        Remove((object)value);
    }

    public override int Add(object value)
    {
        OnChange();
        ValidateType(value);
        Validate(-1, value);
        InnerList.Add((InformixParameter)value);
        return Count - 1;
    }

    public override void AddRange(Array values)
    {
        OnChange();
        if (values == null)
        {
            throw ADP.ArgumentNull("values");
        }
        foreach (object value in values)
        {
            ValidateType(value);
        }
        foreach (InformixParameter value2 in values)
        {
            Validate(-1, value2);
            InnerList.Add(value2);
        }
    }

    private int CheckName(string parameterName)
    {
        int num = IndexOf(parameterName);
        if (num < 0)
        {
            throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
        }
        return num;
    }

    public override void Clear()
    {
        OnChange();
        List<InformixParameter> innerList = InnerList;
        if (innerList == null)
        {
            return;
        }
        foreach (InformixParameter item in innerList)
        {
            item.ResetParent();
        }
        innerList.Clear();
    }

    public override bool Contains(object value)
    {
        return -1 != IndexOf(value);
    }

    public override void CopyTo(Array array, int index)
    {
        ((ICollection)InnerList).CopyTo(array, index);
    }

    public override IEnumerator GetEnumerator()
    {
        return ((IEnumerable)InnerList).GetEnumerator();
    }

    protected override DbParameter GetParameter(int index)
    {
        RangeCheck(index);
        return InnerList[index];
    }

    protected override DbParameter GetParameter(string parameterName)
    {
        int num = IndexOf(parameterName);
        if (num < 0)
        {
            throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
        }
        return InnerList[num];
    }

    private static int IndexOf(IEnumerable items, string parameterName)
    {
        if (items != null)
        {
            int num = 0;
            foreach (InformixParameter item in items)
            {
                if (parameterName == item.ParameterName)
                {
                    return num;
                }
                num++;
            }
            num = 0;
            foreach (InformixParameter item2 in items)
            {
                if (ADP.DstCompare(parameterName, item2.ParameterName) == 0)
                {
                    return num;
                }
                num++;
            }
        }
        return -1;
    }

    public override int IndexOf(string parameterName)
    {
        return IndexOf(InnerList, parameterName);
    }

    public override int IndexOf(object value)
    {
        if (value != null)
        {
            ValidateType(value);
            List<InformixParameter> innerList = InnerList;
            if (innerList != null)
            {
                int count = innerList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (value == innerList[i])
                    {
                        return i;
                    }
                }
            }
        }
        return -1;
    }

    public override void Insert(int index, object value)
    {
        OnChange();
        ValidateType(value);
        Validate(-1, (InformixParameter)value);
        InnerList.Insert(index, (InformixParameter)value);
    }

    private void RangeCheck(int index)
    {
        if (index < 0 || Count <= index)
        {
            throw ADP.ParametersMappingIndex(index, this);
        }
    }

    public override void Remove(object value)
    {
        OnChange();
        ValidateType(value);
        int num = IndexOf(value);
        if (-1 != num)
        {
            RemoveIndex(num);
        }
        else if (this != ((InformixParameter)value).CompareExchangeParent(null, this))
        {
            throw ADP.CollectionRemoveInvalidObject(s_itemType, this);
        }
    }

    public override void RemoveAt(int index)
    {
        OnChange();
        RangeCheck(index);
        RemoveIndex(index);
    }

    public override void RemoveAt(string parameterName)
    {
        OnChange();
        int index = CheckName(parameterName);
        RemoveIndex(index);
    }

    private void RemoveIndex(int index)
    {
        List<InformixParameter> innerList = InnerList;
        InformixParameter ifxParameter = innerList[index];
        innerList.RemoveAt(index);
        ifxParameter.ResetParent();
    }

    private void Replace(int index, object newValue)
    {
        List<InformixParameter> innerList = InnerList;
        ValidateType(newValue);
        Validate(index, newValue);
        InformixParameter ifxParameter = innerList[index];
        innerList[index] = (InformixParameter)newValue;
        ifxParameter.ResetParent();
    }

    protected override void SetParameter(int index, DbParameter value)
    {
        OnChange();
        RangeCheck(index);
        Replace(index, value);
    }

    protected override void SetParameter(string parameterName, DbParameter value)
    {
        OnChange();
        int num = IndexOf(parameterName);
        if (num < 0)
        {
            throw ADP.ParametersSourceIndex(parameterName, this, s_itemType);
        }
        Replace(num, value);
    }

    private void Validate(int index, object value)
    {
        if (value == null)
        {
            throw ADP.ParameterNull("value", this, s_itemType);
        }
        object obj = ((InformixParameter)value).CompareExchangeParent(this, null);
        if (obj != null)
        {
            if (this != obj)
            {
                throw ADP.ParametersIsNotParent(s_itemType, this);
            }
            if (index != IndexOf(value))
            {
                throw ADP.ParametersIsParent(s_itemType, this);
            }
        }
        string parameterName = ((InformixParameter)value).ParameterName;
        if (parameterName.Length == 0)
        {
            index = 1;
            do
            {
                parameterName = "Parameter" + index.ToString(CultureInfo.CurrentCulture);
                index++;
            }
            while (-1 != IndexOf(parameterName));
            ((InformixParameter)value).ParameterName = parameterName;
        }
    }

    private void ValidateType(object value)
    {
        if (value == null)
        {
            throw ADP.ParameterNull("value", this, s_itemType);
        }
        if (!s_itemType.IsInstanceOfType(value))
        {
            throw ADP.InvalidParameterType(this, s_itemType, value);
        }
    }
}
