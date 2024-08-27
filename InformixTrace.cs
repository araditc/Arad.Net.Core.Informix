using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;



namespace Arad.Net.Core.Informix;
internal class InformixTrace
{
    internal static int envTraceLevel = -1;

    internal static string envTraceFile = null;

    private static StreamWriter traceWriter = null;

    private StringBuilder methodName = new StringBuilder();

    private ParameterInfo[] paramInfo;

    internal static string GetStringValue(object Obj)
    {
        if (Obj != null)
        {
            return Obj.ToString();
        }
        return "Null";
    }

    private void traceEntryHelper()
    {
        WriteToFile(Process.GetCurrentProcess().Id + ":" + Thread.CurrentThread.GetHashCode() + "  Entry: " + methodName.ToString() + "()");
        if (paramInfo.Length != 0)
        {
            WriteToFile(" Mismatch in actual number of parameters & supplied number of parameters");
        }
    }

    internal void ApiEntry()
    {
        traceEntryHelper();
    }

    private void traceEntryHelper(object paramValue)
    {
        WriteToFile(Process.GetCurrentProcess().Id + ":" + Thread.CurrentThread.GetHashCode() + "  Entry: " + methodName.ToString());
        if (1 != paramInfo.Length)
        {
            WriteToFile(" Mismatch in actual number of parameters & supplied number of parameters");
            return;
        }
        switch (envTraceLevel)
        {
            case 1:
                WriteToFile("(" + paramInfo[0].ParameterType.Name + ")");
                break;
            case 2:
                WriteToFile("(");
                WriteToFile("  " + paramInfo[0].ParameterType.Name + "  " + GetStringValue(paramValue));
                WriteToFile(")");
                break;
            case 0:
                break;
        }
    }

    internal void ApiEntry(object paramValue)
    {
        traceEntryHelper(paramValue);
    }

    private void traceHelper(params object[] obj)
    {
        traceHelper("Entry: ", obj);
    }

    private void traceHelper(string header, params object[] obj)
    {
        try
        {
            WriteToFile(Process.GetCurrentProcess().Id + ":" + Thread.CurrentThread.GetHashCode() + "  " + header + methodName.ToString());
            if (obj.Length != paramInfo.Length && (paramInfo.Length != 1 || !(paramInfo[0].ParameterType == typeof(object[]))))
            {
                WriteToFile("Error in trace: Mismatch in actual number of parameters & supplied number of parameters");
                return;
            }
            switch (envTraceLevel)
            {
                case 1:
                    {
                        StringBuilder stringBuilder = new StringBuilder("");
                        for (int i = 0; i < obj.Length; i++)
                        {
                            if (i != 0)
                            {
                                stringBuilder.Append(", ");
                            }
                            stringBuilder.Append(paramInfo[i].ParameterType.Name);
                        }
                        WriteToFile("(" + stringBuilder.ToString() + ")");
                        break;
                    }
                case 2:
                    WriteParameters(obj);
                    break;
                case 0:
                    break;
            }
        }
        catch
        {
        }
    }

    private void WriteParameters(object[] obj)
    {
        WriteToFile("(");
        for (int i = 0; i < paramInfo.Length; i++)
        {
            if (!paramInfo[0].ParameterType.IsArray)
            {
                WriteToFile(paramInfo[i].ParameterType.Name + "  " + GetStringValue(obj.GetValue(i)));
                continue;
            }
            WriteToFile(paramInfo[i].ParameterType.Name);
            for (int j = 0; j < obj.Length; j++)
            {
                WriteToFile("    " + GetStringValue(obj.GetValue(j)));
            }
        }
        WriteToFile(")");
    }

    internal void ApiEntry(params object[] obj)
    {
        traceHelper(obj);
    }

    internal void ApiExit()
    {
        WriteToFile(Process.GetCurrentProcess().Id + ":" + Thread.CurrentThread.GetHashCode() + "  Exit: " + methodName.ToString() + " ");
    }

    internal void ApiExit(params object[] obj)
    {
        traceHelper("Exit: ", obj);
    }

    internal static void WriteToFile(string data)
    {
        if (envTraceLevel <= 2)
        {
            traceWriter.WriteLine(data);
        }
    }

    private static bool IsSelfCall(StackTrace stackTrace)
    {
        for (int i = 2; i < stackTrace.FrameCount - 1; i++)
        {
            MethodBase method = stackTrace.GetFrame(i).GetMethod();
            Type declaringType = method.DeclaringType;
            string name = Assembly.GetAssembly(declaringType).GetName().Name;
            if (name.Equals("IBM.Data.Informix"))
            {
                return true;
            }
        }
        return false;
    }

    internal static InformixTrace GetIfxTrace()
    {
        InformixTrace ifxTrace = null;
        if (envTraceLevel == -1)
        {
            InitTrace();
        }
        if (envTraceLevel > 0)
        {
            StackTrace stackTrace = new StackTrace();
            if (envTraceLevel > 2 || !IsSelfCall(stackTrace))
            {
                MethodBase method = stackTrace.GetFrame(1).GetMethod();
                ifxTrace = new InformixTrace();
                ifxTrace.methodName.Append(method.DeclaringType.Name);
                ifxTrace.methodName.Append('.');
                ifxTrace.methodName.Append(method.Name);
                ifxTrace.paramInfo = method.GetParameters();
            }
        }
        return ifxTrace;
    }

    internal static void InitTrace()
    {
        try
        {
            string environmentVariable = Environment.GetEnvironmentVariable("IFXDOTNETTRACE");
            if (environmentVariable == null)
            {
                envTraceLevel = 0;
            }
            else
            {
                envTraceLevel = int.Parse(environmentVariable);
                if (envTraceLevel < 1 || envTraceLevel > 4)
                {
                    envTraceLevel = 0;
                }
            }
            envTraceFile = Environment.GetEnvironmentVariable("IFXDOTNETTRACEFILE");
            if (envTraceFile == null)
            {
                envTraceLevel = 0;
            }
            if (envTraceLevel != 0 && envTraceFile != null && traceWriter == null && envTraceLevel < 3)
            {
                traceWriter = new StreamWriter(envTraceFile, append: true);
                traceWriter.AutoFlush = true;
                SystemInformation.LogAll();
            }
        }
        catch (Exception)
        {
            envTraceFile = null;
            envTraceLevel = 0;
        }
    }
}
