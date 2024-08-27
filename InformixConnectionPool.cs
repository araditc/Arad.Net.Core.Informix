using System;
using System.Collections;
using System.Threading;



namespace Arad.Net.Core.Informix;
internal class InformixConnectionPool
{
    private enum ConnectionPoolType
    {
        Idle,
        Active
    }

    private class IfxConnPoolNode : IComparable
    {
        internal DateTime poolEntryTime;

        internal bool connInUse;

        internal DBCWrapper dbcWrapper;

        internal static long PoolNodeObjSerialID;

        private long objectId;

        internal long ObjectId => objectId;

        internal IfxConnPoolNode(InformixConnection connection)
        {
            lock (this)
            {
                PoolNodeObjSerialID++;
                objectId = PoolNodeObjSerialID;
            }
            dbcWrapper = new DBCWrapper(connection);
            poolEntryTime = DateTime.Now;
        }

        internal IfxConnPoolNode()
        {
            lock (this)
            {
                PoolNodeObjSerialID++;
                objectId = PoolNodeObjSerialID;
            }
            Clear();
        }

        internal void Clear()
        {
            connInUse = false;
            dbcWrapper = null;
        }

        public override int GetHashCode()
        {
            return dbcWrapper.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return dbcWrapper.Equals(obj);
        }

        public int CompareTo(object obj)
        {
            if (obj is IfxConnPoolNode)
            {
                IfxConnPoolNode value = (IfxConnPoolNode)obj;
                return poolEntryTime.CompareTo(value);
            }
            throw new ArgumentException();
        }

        internal void Reset()
        {
            dbcWrapper.Close();
            Clear();
        }
    }

    private const int ConnIdleTimeout = 120;

    private InformixConnSettings connSettings;

    private Mutex connPoolMutex;

    private ManualResetEvent NewConnectionEvent;

    private Hashtable idleNodes;

    private Hashtable activeNodes;

    private Timer stateTimer;

    internal int TotalNodes => idleNodes.Count + activeNodes.Count;

    internal int IdleNodes => idleNodes.Count;

    internal int ActiveNodes => activeNodes.Count;

    internal InformixConnectionPool(InformixConnSettings settings)
    {
        connPoolMutex = new Mutex();
        NewConnectionEvent = new ManualResetEvent(initialState: true);
        connSettings = settings;
        idleNodes = new Hashtable(settings.maxPoolSize);
        activeNodes = new Hashtable(settings.maxPoolSize);
        if (connSettings.connLifeTime != 0)
        {
            int dueTime = connSettings.connLifeTime * 1000;
            TimerCallback callback = InvokeCleanPoolonConnLifetimeExceed;
            stateTimer = new Timer(callback, idleNodes, dueTime, 1000);
        }
    }

    ~InformixConnectionPool()
    {
        try
        {
            if (idleNodes != null && idleNodes.Count > 0)
            {
                IDictionaryEnumerator enumerator = idleNodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IfxConnPoolNode ifxConnPoolNode = (IfxConnPoolNode)enumerator.Value;
                    object key = enumerator.Key;
                    try
                    {
                        ifxConnPoolNode.Reset();
                        idleNodes.Remove(key);
                        enumerator = idleNodes.GetEnumerator();
                    }
                    catch
                    {
                    }
                }
            }
        }
        catch
        {
        }
        try
        {
            if (activeNodes != null && activeNodes.Count > 0)
            {
                IDictionaryEnumerator enumerator2 = activeNodes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    IfxConnPoolNode ifxConnPoolNode = (IfxConnPoolNode)enumerator2.Value;
                    object key2 = enumerator2.Key;
                    try
                    {
                        ifxConnPoolNode.Reset();
                        activeNodes.Remove(key2);
                        enumerator2 = activeNodes.GetEnumerator();
                    }
                    catch
                    {
                    }
                }
            }
        }
        catch
        {
        }
        try
        {
            if (connPoolMutex != null)
            {
                connPoolMutex.Close();
                connPoolMutex = null;
            }
        }
        catch
        {
        }
    }

    private IfxConnPoolNode GetIdleNode(InformixConnection connection)
    {
        IfxConnPoolNode ifxConnPoolNode = null;
        IfxConnPoolNode ifxConnPoolNode2 = null;
        int num = -1;
        if (idleNodes.Count > 0)
        {
            IDictionaryEnumerator enumerator = idleNodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ifxConnPoolNode = (IfxConnPoolNode)enumerator.Value;
                num++;
                if (connection.connSettingsAtOpen.enlist)
                {
                    if (!ifxConnPoolNode.dbcWrapper.enlisted)
                    {
                        ifxConnPoolNode2 = ifxConnPoolNode;
                        break;
                    }
                    if (true)
                    {
                        ifxConnPoolNode2 = ifxConnPoolNode;
                        break;
                    }
                    if (Interop.Odbc.IFMX_IsInTransaction(ifxConnPoolNode.dbcWrapper.hdbc) == Informix32.RetCode.SUCCESS)
                    {
                        ifxConnPoolNode.dbcWrapper._isInTransaction = false;
                    }
                    if (!ifxConnPoolNode.dbcWrapper._isInTransaction)
                    {
                        ifxConnPoolNode2 = ifxConnPoolNode;
                        break;
                    }
                }
                else if (!ifxConnPoolNode.dbcWrapper.enlisted)
                {
                    ifxConnPoolNode2 = ifxConnPoolNode;
                    break;
                }
            }
        }
        if (ifxConnPoolNode2 != null)
        {
            idleNodes.Remove(num);
        }
        return ifxConnPoolNode2;
    }

    private int BindNodeToConnection(IfxConnPoolNode poolNode, InformixConnection connection, ConnectionPoolType ConnPoolType)
    {
        int result = 0;
        connection._dbcWrapper = poolNode.dbcWrapper;
        if (!connection.ConnectionIsAlive())
        {
            connection._dbcWrapper = null;
            result = -1;
        }
        else if (ConnPoolType == ConnectionPoolType.Active)
        {
            poolNode.connInUse = true;
            activeNodes.Add(connection.ObjectId, poolNode);
        }
        else
        {
            poolNode.connInUse = false;
            poolNode.poolEntryTime = DateTime.Now;
            idleNodes.Add(poolNode.ObjectId, poolNode);
        }
        return result;
    }

    internal void Open(InformixConnection connection)
    {
        DateTime dateTime = DateTime.Now.AddSeconds(connSettings.connTimeOut);
        IfxConnPoolNode ifxConnPoolNode = null;
        while (true)
        {
            if (!CanGiveNewConnection())
            {
                TimeSpan timeout;
                TimeSpan timeSpan = timeout = dateTime.Subtract(DateTime.Now);
                if (timeSpan.TotalMilliseconds <= 0.0)
                {
                    ReportOpenTimeOut();
                }
                if (!NewConnectionEvent.WaitOne(timeout, exitContext: false))
                {
                    ReportOpenTimeOut();
                }
            }
            try
            {
                if (connSettings.connTimeOut == 0)
                {
                    connPoolMutex.WaitOne();
                }
                else
                {
                    TimeSpan timeout;
                    TimeSpan timeSpan = timeout = dateTime.Subtract(DateTime.Now);
                    if (timeSpan.TotalMilliseconds <= 0.0)
                    {
                        ReportOpenTimeOut();
                    }
                    if (!connPoolMutex.WaitOne(timeout, exitContext: false))
                    {
                        ReportOpenTimeOut();
                    }
                }
            }
            catch
            {
                ReportOpenTimeOut();
            }
            if (CanGiveNewConnection())
            {
                break;
            }
            connPoolMutex.ReleaseMutex();
            Thread.Sleep(0);
        }
        try
        {
            ifxConnPoolNode = GetIdleNode(connection);
            if (ifxConnPoolNode != null && BindNodeToConnection(ifxConnPoolNode, connection, ConnectionPoolType.Active) == 0)
            {
                connection._dbcWrapper.EnlistAsRequired(connection);
            }
            else
            {
                if (connSettings.connTimeOut < 0)
                {
                    return;
                }
                bool flag = idleNodes.Count + activeNodes.Count == 0;
                bool flag2 = true;
                bool flag3 = false;
                if (connSettings.connTimeOut == 0)
                {
                    flag3 = true;
                }
                while (DateTime.Now < dateTime || flag3)
                {
                    if (idleNodes.Count + activeNodes.Count > connSettings.maxPoolSize)
                    {
                        throw new ArgumentException();
                    }
                    if (!CanGiveNewConnection())
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    flag2 = false;
                    OpenNewConnection(connection, ConnectionPoolType.Active);
                    break;
                }
                if (flag2)
                {
                    throw new ArgumentException();
                }
                if (flag)
                {
                    FillPool(connection);
                }
            }
        }
        finally
        {
            connPoolMutex.ReleaseMutex();
        }
    }

    private bool CanGiveNewConnection()
    {
        if (idleNodes.Count == 0 && idleNodes.Count + activeNodes.Count == connSettings.maxPoolSize)
        {
            NewConnectionEvent.Reset();
            return false;
        }
        NewConnectionEvent.Set();
        return true;
    }

    private void ReportOpenTimeOut()
    {
        CanGiveNewConnection();
        throw new ArgumentException();
    }

    private int FillPool(InformixConnection RefConn)
    {
        int num = 0;
        for (int i = idleNodes.Count + activeNodes.Count; i < connSettings.minPoolSize; i++)
        {
            InformixConnection connection = (InformixConnection)((ICloneable)RefConn).Clone();
            OpenNewConnection(connection, ConnectionPoolType.Idle);
            num++;
        }
        return num;
    }

    private int OpenNewConnection(InformixConnection connection, ConnectionPoolType ConnPoolType)
    {
        int result = 0;
        IfxConnPoolNode ifxConnPoolNode = null;
        ifxConnPoolNode = new IfxConnPoolNode(connection);
        if (BindNodeToConnection(ifxConnPoolNode, connection, ConnPoolType) == 0 && ConnPoolType == ConnectionPoolType.Active)
        {
            connection._dbcWrapper.EnlistAsRequired(connection);
        }
        return result;
    }

    internal Informix32.RETCODE Close(InformixConnection conn)
    {
        Informix32.RETCODE result = Informix32.RETCODE.SUCCESS;
        IfxConnPoolNode ifxConnPoolNode;
        lock (activeNodes.SyncRoot)
        {
            ifxConnPoolNode = (IfxConnPoolNode)activeNodes[conn.ObjectId];
        }
        connPoolMutex.WaitOne();
        try
        {
            if (connSettings.connLifeTime != 0 && ifxConnPoolNode != null && !ifxConnPoolNode.connInUse)
            {
                double totalSeconds = DateTime.Now.Subtract(ifxConnPoolNode.poolEntryTime).TotalSeconds;
                if (totalSeconds > connSettings.connLifeTime && TotalNodes > connSettings.minPoolSize)
                {
                    ifxConnPoolNode.Reset();
                    ifxConnPoolNode = null;
                }
            }
            if (activeNodes.ContainsKey(conn.ObjectId))
            {
                activeNodes.Remove(conn.ObjectId);
                if (ifxConnPoolNode != null)
                {
                    ifxConnPoolNode.poolEntryTime = DateTime.Now;
                    idleNodes.Add(ifxConnPoolNode.ObjectId, ifxConnPoolNode);
                }
            }
        }
        finally
        {
            CanGiveNewConnection();
            connPoolMutex.ReleaseMutex();
        }
        return result;
    }

    private void InvokeCleanPoolonConnLifetimeExceed(object idlnodes)
    {
        if (idleNodes != null && idleNodes.Count != 0)
        {
            CleanPool();
        }
        else if (stateTimer != null)
        {
            stateTimer.Dispose();
            stateTimer = null;
        }
    }

    internal void CleanPool()
    {
        connPoolMutex.WaitOne();
        int num = -1;
        try
        {
            IDictionaryEnumerator enumerator = idleNodes.GetEnumerator();
            while (enumerator.MoveNext() && TotalNodes > connSettings.minPoolSize)
            {
                IfxConnPoolNode ifxConnPoolNode = (IfxConnPoolNode)enumerator.Value;
                num++;
                if (ifxConnPoolNode.dbcWrapper._isInTransaction)
                {
                    short num2 = 1;
                    if (Interop.Odbc.IFMX_IsInTransaction(ifxConnPoolNode.dbcWrapper.hdbc) != 0)
                    {
                        continue;
                    }
                }
                if (ifxConnPoolNode != null && (HasIdleTimeOutExpired(ifxConnPoolNode) || HasConnLifeTimeExceeded(ifxConnPoolNode)))
                {
                    ifxConnPoolNode.Reset();
                    idleNodes.Remove(num);
                    enumerator = idleNodes.GetEnumerator();
                    num = -1;
                }
            }
        }
        finally
        {
            CanGiveNewConnection();
            connPoolMutex.ReleaseMutex();
        }
    }

    private static bool HasIdleTimeOutExpired(IfxConnPoolNode poolNode)
    {
        DateTime dateTime = poolNode.poolEntryTime.AddSeconds(120.0);
        if (DateTime.Now > dateTime)
        {
            return true;
        }
        return false;
    }

    private bool HasConnLifeTimeExceeded(IfxConnPoolNode poolNode)
    {
        if (connSettings.connLifeTime != 0)
        {
            DateTime dateTime = poolNode.poolEntryTime.AddSeconds(connSettings.connLifeTime);
            if (DateTime.Now > dateTime)
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
