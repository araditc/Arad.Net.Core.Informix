using System;


namespace Arad.Net.Core.Informix;

internal class ParamBufs
{
    internal CNativeBuffer dataBuf4_1;

    internal CNativeBuffer dataBuf4_2;

    internal CNativeBuffer dataBuf4_3;

    internal CNativeBuffer dataBuf8_1;

    internal CNativeBuffer dataBuf8_2;

    internal CNativeBuffer dataBufVarLen1;

    internal CNativeBuffer dataBufVarLen2;

    internal CNativeBuffer indBuf_1;

    internal CNativeBuffer indBuf_2;

    internal CNativeBuffer indBuf_3;

    internal CNativeBuffer indBuf_4;

    internal CNativeBuffer indBuf_5;

    internal ParamBufs()
    {
        int size = nint.Size;
        dataBuf4_1 = new CNativeBuffer(size);
        dataBuf4_2 = new CNativeBuffer(size);
        dataBuf4_3 = new CNativeBuffer(size);
        dataBuf8_1 = new CNativeBuffer(8);
        dataBuf8_2 = new CNativeBuffer(8);
        dataBufVarLen1 = new CNativeBuffer(1);
        dataBufVarLen2 = new CNativeBuffer(1);
        indBuf_1 = new CNativeBuffer(size);
        indBuf_2 = new CNativeBuffer(size);
        indBuf_3 = new CNativeBuffer(size);
        indBuf_4 = new CNativeBuffer(size);
        indBuf_5 = new CNativeBuffer(size);
    }

    internal CNativeBuffer GetVarLenDataBuf1(int length)
    {
        dataBufVarLen1.EnsureAlloc(length);
        return dataBufVarLen1;
    }

    internal CNativeBuffer GetVarLenDataBuf2(int length)
    {
        dataBufVarLen2.EnsureAlloc(length);
        return dataBufVarLen2;
    }
}
