using System;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Security;
using System.Security.Permissions;



namespace Arad.Net.Core.Informix;

[Serializable]
public sealed class InformixPermission : CodeAccessPermission, IUnrestrictedPermission
{
    private sealed class XmlStr
    {
        internal const string _class = "class";

        internal const string _IPermission = "IPermission";

        internal const string _Permission = "Permission";

        internal const string _Unrestricted = "Unrestricted";

        internal const string _true = "true";

        internal const string _Version = "version";

        internal const string _VersionNumber = "1";

        internal const string _add = "add";

        internal const string _keyword = "keyword";

        internal const string _name = "name";

        internal const string _value = "value";
    }

    private bool isUnrestricted;

    public InformixPermission()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
    }

    public InformixPermission(PermissionState state)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(state);
        switch (state)
        {
            case PermissionState.Unrestricted:
                isUnrestricted = true;
                break;
            case PermissionState.None:
                isUnrestricted = false;
                break;
            default:
                throw ADP.Argument("state");
        }
        ifxTrace?.ApiExit();
    }

    public override IPermission Copy()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixPermission ifxPermission = null;
        ifxPermission = CreateInstance();
        ifxPermission.isUnrestricted = isUnrestricted;
        ifxTrace?.ApiExit();
        return ifxPermission;
    }

    internal InformixPermission CreateInstance()
    {
        return new InformixPermission();
    }

    public override IPermission Intersect(IPermission target)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(target);
        if (target == null)
        {
            return null;
        }
        if (target.GetType() != GetType())
        {
            throw ADP.Argument("target");
        }
        InformixPermission ifxPermission = (InformixPermission)target;
        if (IsUnrestricted())
        {
            return ifxPermission.Copy();
        }
        ifxTrace?.ApiExit();
        return Copy();
    }

    public override bool IsSubsetOf(IPermission target)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(target);
        if (target == null)
        {
            ifxTrace?.ApiExit();
            return false;
        }
        if (target.GetType() != GetType())
        {
            throw ADP.WrongType(GetType());
        }
        bool result = ((InformixPermission)target).IsUnrestricted() || !IsUnrestricted();
        ifxTrace?.ApiExit();
        return result;
    }

    public bool IsUnrestricted()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
        return isUnrestricted;
    }

    public override IPermission Union(IPermission target)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(target);
        if (target == null)
        {
            return Copy();
        }
        if (target.GetType() != GetType())
        {
            throw ADP.Argument("target");
        }
        InformixPermission ifxPermission = (InformixPermission)target;
        InformixPermission ifxPermission2 = CreateInstance();
        ifxPermission2.isUnrestricted = IsUnrestricted() || ifxPermission.IsUnrestricted();
        ifxTrace?.ApiExit();
        return ifxPermission2;
    }

    public override void FromXml(SecurityElement securityElement)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(securityElement);
        if (securityElement == null)
        {
            throw ADP.ArgumentNull("securityElement");
        }
        string tag = securityElement.Tag;
        if (!tag.Equals("Permission") && !tag.Equals("IPermission"))
        {
            throw ADP.NotAPermissionElement();
        }
        string text = securityElement.Attribute("version");
        if (text != null && !text.Equals("1"))
        {
            throw ADP.InvalidXMLBadVersion();
        }
        try
        {
            string text2 = securityElement.Attribute("Unrestricted");
            isUnrestricted = text2 != null && bool.Parse(text2);
        }
        catch (Exception)
        {
        }
        ifxTrace?.ApiExit();
    }

    public override SecurityElement ToXml()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        Type type = GetType();
        SecurityElement securityElement = new SecurityElement("IPermission");
        securityElement.AddAttribute("class", type.FullName + ", " + type.Module.Assembly.FullName.Replace('"', '\''));
        securityElement.AddAttribute("version", "1");
        if (isUnrestricted)
        {
            securityElement.AddAttribute("Unrestricted", "true");
        }
        ifxTrace?.ApiExit();
        return securityElement;
    }
}
