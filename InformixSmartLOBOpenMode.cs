using System;


namespace Arad.Net.Core.Informix;

[Flags]
public enum InformixSmartLOBOpenMode
{
    None = 0,
    ReadOnly = 4,
    DirtyRead = 0x10,
    WriteOnly = 2,
    Append = 1,
    ReadWrite = 8,
    Buffer = 0x200,
    Nobuffer = 0x400,
    LockAll = 0x1000,
    LockRange = 0x2000
}
