using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;


namespace Arad.Net.Core.Informix;

internal sealed class InformixConnPoolManager
{
    private const int POOL_CLEANING_INTERVAL = 60000;

    internal static Hashtable connPools;

    internal static OdbcEnvironmentHandle hEnv;

    internal static InformixConnectionHandle hDbc;

    private static Thread poolCleaner = null;

    private Mutex connMgrMutex;

    internal short dlmtDefault;

    internal static TypeMap mappingTable = new TypeMap();

    private static int refCount;

    private static int numConns;

    private static int failedConns;

    private static int peakPooledConns;

    private static bool perfCounters = true;

    private static PerformanceCounter perfCounterPooledAndNonPooledConnections;

    private static PerformanceCounter perfCounterPooledConnections;

    private static PerformanceCounter perfCounterNumberOfPools;

    private static PerformanceCounter perfCounterMaxPooledConnections;

    private static PerformanceCounter perfCounterTotalFailedConnects;

    internal InformixConnPoolManager()
    {
        CNativeBuffer cNativeBuffer = new CNativeBuffer(1024);
        connPools = Hashtable.Synchronized(new Hashtable());
        connMgrMutex = new Mutex();
        hEnv = new OdbcEnvironmentHandle();
        hDbc = new InformixConnectionHandle(hEnv);
        int cbActual = 0;
        byte[] array = new byte[100];
        if (hDbc.GetConnectionAttribute(Informix32.SQL_ATTR.INFX_DELIMIDENT, array, out cbActual) != 0)
        {
            Exception ex = null;
            throw ex;
        }
        dlmtDefault = Convert.ToInt16(array);
        try
        {
            hDbc.FreeConnectHandle();
        }
        catch (Exception)
        {
        }
        lock (this)
        {
            refCount = 0;
            numConns = 0;
            failedConns = 0;
            peakPooledConns = 0;
            PerformanceCounterPermission performanceCounterPermission = new PerformanceCounterPermission();
            performanceCounterPermission.Assert();
            try
            {
                perfCounterPooledAndNonPooledConnections = new PerformanceCounter(".NET CLR Data", "IBM Informix Client: Current # pooled and nonpooled connections", "defaultdomain", readOnly: false);
            }
            catch
            {
                perfCounters = false;
            }
            if (perfCounters)
            {
                perfCounterPooledConnections = new PerformanceCounter(".NET CLR Data", "IBM Informix Client: Current # pooled connections", "defaultdomain", readOnly: false);
                perfCounterNumberOfPools = new PerformanceCounter(".NET CLR Data", "IBM Informix Client: Current # connection pools", "defaultdomain", readOnly: false);
                perfCounterMaxPooledConnections = new PerformanceCounter(".NET CLR Data", "IBM Informix Client: Peak # pooled connections", "defaultdomain", readOnly: false);
                perfCounterTotalFailedConnects = new PerformanceCounter(".NET CLR Data", "IBM Informix Client: Total # failed connects", "defaultdomain", readOnly: false);
                perfCounterPooledAndNonPooledConnections.RawValue = 0L;
                perfCounterPooledConnections.RawValue = 0L;
                perfCounterNumberOfPools.RawValue = 0L;
                perfCounterMaxPooledConnections.RawValue = 0L;
                perfCounterTotalFailedConnects.RawValue = 0L;
            }
            poolCleaner = new Thread(InvokeCleanPool);
            poolCleaner.Name = "IfxConnPoolCleaner";
            poolCleaner.IsBackground = true;
            poolCleaner.Start();
        }
        Interlocked.Increment(ref refCount);
    }

    ~InformixConnPoolManager()
    {
        lock (this)
        {
            Interlocked.Decrement(ref refCount);
            if (refCount == 0)
            {
                if (perfCounterPooledAndNonPooledConnections != null)
                {
                    perfCounterPooledAndNonPooledConnections.Close();
                    perfCounterPooledAndNonPooledConnections = null;
                }
                if (perfCounterPooledConnections != null)
                {
                    perfCounterPooledConnections.Close();
                    perfCounterPooledConnections = null;
                }
                if (perfCounterNumberOfPools != null)
                {
                    perfCounterNumberOfPools.Close();
                    perfCounterNumberOfPools = null;
                }
                if (perfCounterMaxPooledConnections != null)
                {
                    perfCounterMaxPooledConnections.Close();
                    perfCounterMaxPooledConnections = null;
                }
                if (perfCounterTotalFailedConnects != null)
                {
                    perfCounterTotalFailedConnects.Close();
                    perfCounterTotalFailedConnects = null;
                }
            }
        }
        try
        {
            hEnv.FreeEnvHandle();
        }
        catch (Exception)
        {
        }
    }

    private void InvokeCleanPool()
    {
        while (true)
        {
            Thread.Sleep(60000);
            connMgrMutex.WaitOne();
            try
            {
                IDictionaryEnumerator enumerator = connPools.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    InformixConnectionPool ifxConnectionPool = (InformixConnectionPool)enumerator.Value;
                    InformixConnSettings key = (InformixConnSettings)enumerator.Key;
                    ifxConnectionPool.CleanPool();
                    if (ifxConnectionPool.TotalNodes == 0)
                    {
                        connPools.Remove(key);
                        if (perfCounters)
                        {
                            perfCounterNumberOfPools.RawValue = connPools.Count;
                        }
                        enumerator = connPools.GetEnumerator();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                connMgrMutex.ReleaseMutex();
            }
        }
    }

    internal InformixConnectionPool GetPool(InformixConnSettings key)
    {
        connMgrMutex.WaitOne();
        InformixConnectionPool ifxConnectionPool;
        try
        {
            ifxConnectionPool = (InformixConnectionPool)connPools[key];
            if (ifxConnectionPool == null)
            {
                ifxConnectionPool = new InformixConnectionPool(key);
                connPools[key] = ifxConnectionPool;
                if (perfCounters)
                {
                    perfCounterNumberOfPools.RawValue = connPools.Count;
                }
            }
        }
        finally
        {
            connMgrMutex.ReleaseMutex();
        }
        return ifxConnectionPool;
    }

    internal void Open(InformixConnection connection)
    {
        InformixConnectionPool pool = GetPool(connection.connSettingsAtOpen);
        try
        {
            pool.Open(connection);
            if (perfCounters)
            {
                Interlocked.Increment(ref numConns);
                perfCounterPooledConnections.RawValue = numConns;
                if (numConns > peakPooledConns)
                {
                    peakPooledConns = numConns;
                    perfCounterMaxPooledConnections.RawValue = peakPooledConns;
                }
            }
        }
        catch
        {
            Interlocked.Increment(ref failedConns);
            if (perfCounters)
            {
                perfCounterTotalFailedConnects.RawValue = failedConns;
            }
            throw;
        }
    }

    internal Informix32.RETCODE Close(InformixConnection connection)
    {
        Informix32.RETCODE rETCODE = Informix32.RETCODE.SUCCESS;
        InformixConnectionPool pool = GetPool(connection.connSettingsAtOpen);
        rETCODE = pool.Close(connection);
        if (perfCounters)
        {
            Interlocked.Decrement(ref numConns);
            perfCounterPooledAndNonPooledConnections.RawValue = numConns;
        }
        return rETCODE;
    }
}
