using System;
using System.Text;



namespace Arad.Net.Core.Informix;
public sealed class InformixInfoMessageEventArgs : EventArgs
{
    private InformixErrorCollection _errors;

    public InformixErrorCollection Errors => _errors;

    public string Message
    {
        get
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (InformixError error in Errors)
            {
                if (0 < stringBuilder.Length)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
                stringBuilder.Append(error.Message);
            }
            return stringBuilder.ToString();
        }
    }

    internal InformixInfoMessageEventArgs(InformixErrorCollection errors)
    {
        _errors = errors;
    }

    public override string ToString()
    {
        return Message;
    }
}
