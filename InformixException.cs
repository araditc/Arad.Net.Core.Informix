using Arad.Net.Core.Informix.System;
using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;


namespace Arad.Net.Core.Informix;

[Serializable]
[TypeForwardedFrom("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class InformixException : DbException
{
    private InformixErrorCollection _odbcErrors = new InformixErrorCollection();

    public InformixErrorCollection Errors
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _odbcErrors;
        }
    }

    public override string Source
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(Source);
            if (0 < Errors.Count)
            {
                string source = Errors[0].Source;
                ifxTrace?.ApiExit();
                if (!string.IsNullOrEmpty(source))
                {
                    return source;
                }
                return "";
            }
            ifxTrace?.ApiExit();
            return "";
        }
    }

    internal static InformixException CreateException(InformixErrorCollection errors, Informix32.RetCode retcode)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (InformixError error in errors)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(Environment.NewLine);
            }
            stringBuilder.Append(SR.GetString(SR.Odbc_ExceptionMessage, Informix32.RetcodeToString(retcode), error.SQLState, error.Message));
        }
        return new InformixException(stringBuilder.ToString(), errors);
    }

    internal InformixException(string message, InformixErrorCollection errors)
        : base(message)
    {
        _odbcErrors = errors;
        HResult = -2146232009;
    }

    private InformixException(SerializationInfo si, StreamingContext sc)
        : base(si, sc)
    {
        _odbcErrors = (InformixErrorCollection)si.GetValue("odbcErrors", typeof(InformixErrorCollection));
        HResult = -2146232009;
    }

    public override void GetObjectData(SerializationInfo si, StreamingContext context)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(si, context);
        base.GetObjectData(si, context);
        si.AddValue("odbcRetcode", Informix32.RETCODE.SUCCESS, typeof(Informix32.RETCODE));
        si.AddValue("odbcErrors", _odbcErrors, typeof(InformixErrorCollection));
        ifxTrace?.ApiExit();
    }
}
