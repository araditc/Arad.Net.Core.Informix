using System.Data;
using System.Data.Common;


namespace Arad.Net.Core.Informix;

public sealed class InformixRowUpdatedEventArgs : RowUpdatedEventArgs
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
    }

    public InformixRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        : base(row, command, statementType, tableMapping)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(row, command, statementType, tableMapping);
        ifxTrace?.ApiExit();
    }
}
