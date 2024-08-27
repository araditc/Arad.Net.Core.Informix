using System;



namespace Arad.Net.Core.Informix;
public class InformixBlob : InformixSmartLOB
{
    public static readonly InformixBlob Null = new InformixBlob(isNull: true);

    public InformixBlob(InformixConnection connection)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(connection);
        InitSmartLOB(null, connection, SmartLOBType.BLOB);
        ifxTrace?.ApiExit();
    }

    public InformixBlob(InformixConnection connection, InformixSmartLOBLocator locator)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(connection, locator);
        InitSmartLOB(locator, connection, SmartLOBType.BLOB);
        ifxTrace?.ApiExit();
    }

    internal InformixBlob(bool isNull)
    {
        InitSmartLOB(isNull);
    }

    public InformixBlob(InformixConnection connection, string table, string column)
        : this(connection)
    {
        tableName = table;
        colName = column;
    }

    public InformixBlob(InformixConnection connection, InformixSmartLOBLocator locator, string table, string column)
        : this(connection, locator)
    {
        tableName = table;
        colName = column;
    }

    public long Read(byte[] buff)
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

    public long Read(long plofd, byte[] buff)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalRead(buff, plofd);
        ifxTrace?.ApiExit();
        return result;
    }

    public long Read(byte[] buff, long buffOffset, long numBytesToRead, long smartLOBOffset, InformixSmartLOBWhence whence)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff, buffOffset, numBytesToRead, smartLOBOffset, whence);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalRead(buff, buffOffset, numBytesToRead, smartLOBOffset, whence);
        ifxTrace?.ApiExit();
        return result;
    }

    public long Write(byte[] buff)
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

    public long Write(byte[] buff, long buffOffset, long numBytesToWrite, long smartLOBOffset, InformixSmartLOBWhence whence)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(buff, buffOffset, numBytesToWrite, smartLOBOffset, whence);
        if (isNull)
        {
            throw new InvalidOperationException();
        }
        long result = internalWrite(buff, buffOffset, numBytesToWrite, smartLOBOffset, whence);
        ifxTrace?.ApiExit();
        return result;
    }
}
