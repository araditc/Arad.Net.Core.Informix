using System;


namespace Arad.Net.Core.Informix;

[Flags]
public enum InformixSmartLOBCreateTimeFlags
{
    None = 0,
    Log = 1,
    NoLog = 2,
    KeepAccessTime = 8,
    DontKeepAccessTime = 0x10
}
