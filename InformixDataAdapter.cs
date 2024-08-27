using System;
using System.Data;
using System.Data.Common;
using Arad.Net.Core.Informix.System.Data.Common;


namespace Arad.Net.Core.Informix;
public sealed class InformixDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
{
    private static readonly object s_eventRowUpdated = new object();

    private static readonly object s_eventRowUpdating = new object();

    private InformixCommand _deleteCommand;

    private InformixCommand _insertCommand;

    private InformixCommand _selectCommand;

    private InformixCommand _updateCommand;

    public new InformixCommand DeleteCommand
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _deleteCommand;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            _deleteCommand = value;
            ifxTrace?.ApiExit();
        }
    }

    IDbCommand IDbDataAdapter.DeleteCommand
    {
        get
        {
            return _deleteCommand;
        }
        set
        {
            _deleteCommand = (InformixCommand)value;
        }
    }

    public new InformixCommand InsertCommand
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _insertCommand;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            _insertCommand = value;
            ifxTrace?.ApiExit();
        }
    }

    IDbCommand IDbDataAdapter.InsertCommand
    {
        get
        {
            return _insertCommand;
        }
        set
        {
            _insertCommand = (InformixCommand)value;
        }
    }

    public new InformixCommand SelectCommand
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _selectCommand;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            _selectCommand = value;
            ifxTrace?.ApiExit();
        }
    }

    IDbCommand IDbDataAdapter.SelectCommand
    {
        get
        {
            return _selectCommand;
        }
        set
        {
            _selectCommand = (InformixCommand)value;
        }
    }

    public new InformixCommand UpdateCommand
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _updateCommand;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            _updateCommand = value;
            ifxTrace?.ApiExit();
        }
    }

    IDbCommand IDbDataAdapter.UpdateCommand
    {
        get
        {
            return _updateCommand;
        }
        set
        {
            _updateCommand = (InformixCommand)value;
        }
    }

    public event InformixRowUpdatedEventHandler RowUpdated
    {
        add
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            Events.AddHandler(s_eventRowUpdated, value);
            ifxTrace?.ApiExit();
        }
        remove
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            Events.RemoveHandler(s_eventRowUpdated, value);
            ifxTrace?.ApiExit();
        }
    }

    public event InformixRowUpdatingEventHandler RowUpdating
    {
        add
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            InformixRowUpdatingEventHandler ifxRowUpdatingEventHandler = (InformixRowUpdatingEventHandler)Events[s_eventRowUpdating];
            if (ifxRowUpdatingEventHandler != null && value.Target is InformixCommandBuilder)
            {
                InformixRowUpdatingEventHandler ifxRowUpdatingEventHandler2 = (InformixRowUpdatingEventHandler)ADP.FindBuilder(ifxRowUpdatingEventHandler);
                if (ifxRowUpdatingEventHandler2 != null)
                {
                    Events.RemoveHandler(s_eventRowUpdating, ifxRowUpdatingEventHandler2);
                }
            }
            Events.AddHandler(s_eventRowUpdating, value);
            ifxTrace?.ApiExit();
        }
        remove
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            Events.RemoveHandler(s_eventRowUpdating, value);
            ifxTrace?.ApiExit();
        }
    }

    public InformixDataAdapter()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        GC.SuppressFinalize(this);
        ifxTrace?.ApiExit();
    }

    public InformixDataAdapter(InformixCommand selectCommand)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        SelectCommand = selectCommand;
        ifxTrace?.ApiExit();
    }

    public InformixDataAdapter(string selectCommandText, InformixConnection selectConnection)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        SelectCommand = new InformixCommand(selectCommandText, selectConnection);
        ifxTrace?.ApiExit();
    }

    public InformixDataAdapter(string selectCommandText, string selectConnectionString)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixConnection connection = new InformixConnection(selectConnectionString);
        SelectCommand = new InformixCommand(selectCommandText, connection);
        ifxTrace?.ApiExit();
    }

    private InformixDataAdapter(InformixDataAdapter from)
        : base(from)
    {
        GC.SuppressFinalize(this);
    }

    object ICloneable.Clone()
    {
        return new InformixDataAdapter(this);
    }

    protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
    {
        return new InformixRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
    }

    protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
    {
        return new InformixRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
    }

    protected override void OnRowUpdated(RowUpdatedEventArgs value)
    {
        InformixRowUpdatedEventHandler ifxRowUpdatedEventHandler = (InformixRowUpdatedEventHandler)Events[s_eventRowUpdated];
        if (ifxRowUpdatedEventHandler != null && value is InformixRowUpdatedEventArgs)
        {
            ifxRowUpdatedEventHandler(this, (InformixRowUpdatedEventArgs)value);
        }
        base.OnRowUpdated(value);
    }

    protected override void OnRowUpdating(RowUpdatingEventArgs value)
    {
        InformixRowUpdatingEventHandler ifxRowUpdatingEventHandler = (InformixRowUpdatingEventHandler)Events[s_eventRowUpdating];
        if (ifxRowUpdatingEventHandler != null && value is InformixRowUpdatingEventArgs)
        {
            ifxRowUpdatingEventHandler(this, (InformixRowUpdatingEventArgs)value);
        }
        base.OnRowUpdating(value);
    }
}
