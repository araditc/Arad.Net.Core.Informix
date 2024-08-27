using System.Threading;


namespace Arad.Net.Core.Informix;

internal sealed class OdbcEnvironment
{
    private static object s_globalEnvironmentHandle;

    private static object s_globalEnvironmentHandleLock = new object();

    private OdbcEnvironment()
    {
    }

    internal static OdbcEnvironmentHandle GetGlobalEnvironmentHandle()
    {
        OdbcEnvironmentHandle odbcEnvironmentHandle = s_globalEnvironmentHandle as OdbcEnvironmentHandle;
        if (odbcEnvironmentHandle == null)
        {
            lock (s_globalEnvironmentHandleLock)
            {
                odbcEnvironmentHandle = s_globalEnvironmentHandle as OdbcEnvironmentHandle;
                if (odbcEnvironmentHandle == null)
                {
                    odbcEnvironmentHandle = (OdbcEnvironmentHandle)(s_globalEnvironmentHandle = new OdbcEnvironmentHandle());
                }
            }
        }
        return odbcEnvironmentHandle;
    }

    internal static void ReleaseObjectPool()
    {
        object obj = Interlocked.Exchange(ref s_globalEnvironmentHandle, null);
        if (obj != null)
        {
            (obj as OdbcEnvironmentHandle).Dispose();
        }
    }
}
