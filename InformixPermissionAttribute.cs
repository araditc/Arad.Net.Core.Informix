using System;
using System.Security;
using System.Security.Permissions;



namespace Arad.Net.Core.Informix;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class InformixPermissionAttribute : CodeAccessSecurityAttribute
{
    public InformixPermissionAttribute(SecurityAction action)
        : base(action)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
    }

    public override IPermission CreatePermission()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        InformixPermission result = new InformixPermission(Unrestricted ? PermissionState.Unrestricted : PermissionState.None);
        ifxTrace?.ApiExit();
        return result;
    }
}
