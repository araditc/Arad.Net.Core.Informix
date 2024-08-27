using System.Data;
using System.Data.Common;


namespace Arad.Net.Core.Informix;

public sealed class InformixRowUpdatingEventArgs : RowUpdatingEventArgs
{
    public new InformixCommand Command
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return (InformixCommand)base.Command;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            base.Command = value;
            ifxTrace?.ApiExit();
        }
    }

    public InformixRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        : base(row, command, statementType, tableMapping)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(row, command, statementType, tableMapping);
        ifxTrace?.ApiExit();
    }
}
