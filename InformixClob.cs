using System;



namespace Arad.Net.Core.Informix;
public class InformixClob : InformixSmartLOB
{
    public static readonly InformixClob Null = new InformixClob(isNull: true);

    public InformixClob(InformixConnection connection)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(connection);
        InitSmartLOB(null, connection, SmartLOBType.CLOB);
        ifxTrace?.ApiExit();
    }

    public InformixClob(InformixConnection connection, InformixSmartLOBLocator locator)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(connection, locator);
        InitSmartLOB(locator, connection, SmartLOBType.CLOB);
        ifxTrace?.ApiExit();
    }

    internal InformixClob(bool isNull)
    {
        InitSmartLOB(isNull);
    }

    public InformixClob(InformixConnection connection, string table, string column)
        : this(connection)
    {
        tableName = table;
        colName = column;
    }

    public InformixClob(InformixConnection connection, InformixSmartLOBLocator locator, string table, string column)
        : this(connection, locator)
    {
        tableName = table;
        colName = column;
    }

    public long Read(char[] buff)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalRead(buff);
        ifxTrace?.ApiExit();
        return result;
    }

    public long Read(char[] buff, long buffOffset, long numCharsToRead, long smartLOBOffset, InformixSmartLOBWhence whence)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff, buffOffset, numCharsToRead, smartLOBOffset, whence);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalRead(buff, buffOffset, numCharsToRead, smartLOBOffset, whence);
        ifxTrace?.ApiExit();
        return result;
    }

    public long Write(char[] buff)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalWrite(buff);
        ifxTrace?.ApiExit();
        return result;
    }

    public long Write(char[] buff, long buffOffset, long numCharsToWrite, long smartLOBOffset, InformixSmartLOBWhence whence)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff, buffOffset, numCharsToWrite, smartLOBOffset, whence);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalWrite(buff, buffOffset, numCharsToWrite, smartLOBOffset, whence);
        ifxTrace?.ApiExit();
        return result;
    }
}
