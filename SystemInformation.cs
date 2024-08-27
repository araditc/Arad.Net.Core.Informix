using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;


namespace Arad.Net.Core.Informix;

internal class SystemInformation
{
    internal static void LogOSInfo()
    {
        OperatingSystem oSVersion = Environment.OSVersion;
        string text = "Operating System:  \t";
        switch (oSVersion.Platform)
        {
            case PlatformID.Win32Windows:
                switch (oSVersion.Version.Minor)
                {
                    case 0:
                        text += "Windows 95";
                        break;
                    case 10:
                        text = !(oSVersion.Version.Revision.ToString() == "2222A") ? text + "Windows 98" : text + "Windows 98 Second Edition";
                        break;
                    case 90:
                        text += "Windows Me";
                        break;
                }
                break;
            case PlatformID.Win32NT:
                switch (oSVersion.Version.Major)
                {
                    case 3:
                        text += "Windows NT 3.51";
                        break;
                    case 4:
                        text += "Windows NT 4.0";
                        break;
                    case 5:
                        text = oSVersion.Version.Minor != 0 ? text + "Windows XP" : text + "Windows 2000";
                        break;
                }
                break;
        }
        InformixTrace.WriteToFile(text);
    }

    internal static void LogFrameworkInfo()
    {
        InformixTrace.WriteToFile("Framework Version:  \t" + Environment.Version.ToString());
    }

    internal static void LogProviderInfo()
    {
        LogAssemblyInfo("Arad.Net.Core.Informix.dll");
    }

    internal static void LogAllAssemblyInfo()
    {
        Assembly[] assemblies = Thread.GetDomain().GetAssemblies();
        foreach (Assembly asm in assemblies)
        {
            LogAssemblyInfo(asm);
        }
    }

    internal static void LogAssemblyName(AssemblyName asmName)
    {
        InformixTrace.WriteToFile("");
        InformixTrace.WriteToFile("Assembly:  " + asmName.Name);
        InformixTrace.WriteToFile("\tFull Name:\t\t" + asmName.FullName);
        InformixTrace.WriteToFile("\tCodeBase:  \t\t" + asmName.CodeBase);
        InformixTrace.WriteToFile("\tVersion:  \t\t" + asmName.Version);
    }

    internal static void LogAssemblyInfo(Assembly asm)
    {
        LogAssemblyName(asm.GetName());
        InformixTrace.WriteToFile("\tLoaded from GAC:  \t" + asm.GlobalAssemblyCache);
        InformixTrace.WriteToFile("\tImage Runtime Version:  " + asm.ImageRuntimeVersion);
        InformixTrace.WriteToFile("\tLocation:\t\t" + asm.Location);
        InformixTrace.WriteToFile("\tModules:");
        Module[] modules = asm.GetModules(getResourceModules: true);
        foreach (Module module in modules)
        {
            string fullyQualifiedName = module.FullyQualifiedName;
            string text = "\t\t" + fullyQualifiedName;
            if (File.Exists(fullyQualifiedName))
            {
                FileInfo fileInfo = new FileInfo(fullyQualifiedName);
                text = text + "\t" + fileInfo.LastWriteTime;
                text = text + "\t" + fileInfo.Length;
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fullyQualifiedName);
                if (versionInfo.FileVersion.Length > 0)
                {
                    text = text + "\n\t\t\tFile Version:  " + versionInfo.FileVersion;
                }
            }
            InformixTrace.WriteToFile(text);
        }
        InformixTrace.WriteToFile("\tReferences:");
        AssemblyName[] referencedAssemblies = asm.GetReferencedAssemblies();
        foreach (AssemblyName assemblyName in referencedAssemblies)
        {
            InformixTrace.WriteToFile("\t\t" + assemblyName.Name);
        }
        InformixTrace.WriteToFile("");
    }

    internal static void LogAssemblyInfo(string assemblyName)
    {
        Assembly asm = Assembly.LoadFrom(assemblyName);
        LogAssemblyInfo(asm);
    }

    internal static void LogAll()
    {
        LogOSInfo();
        LogFrameworkInfo();
        LogAllAssemblyInfo();
    }
}
