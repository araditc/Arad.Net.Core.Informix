using System;



namespace Arad.Net.Core.Informix;
internal sealed class DbSchemaInfo
{
    public static byte Floating = byte.MaxValue;

    public Type _type;

    public string _name;

    public string _baseTableName;

    public string _typename;

    internal Informix32.SQL_TYPE? _dbtype;

    public object _ifxtype;

    public object _scale;

    public object _precision;

    public int _qualifier;

    internal DbSchemaInfo()
    {
    }
}
