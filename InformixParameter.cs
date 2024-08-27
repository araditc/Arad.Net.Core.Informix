using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Arad.Net.Core.Informix.System.Data.Common;


namespace Arad.Net.Core.Informix;
public sealed class InformixParameter : DbParameter, ICloneable, IDataParameter, IDbDataParameter
{
    private const int MaxParameterSize = int.MaxValue;

    private bool _hasChanged;

    private bool _userSpecifiedType;

    private TypeMap _typemap;

    private TypeMap _bindtype;

    private string _parameterName;

    private byte _precision;

    private byte _scale;

    private bool _hasScale;

    internal TypeMap MappingTable = new TypeMap();

    private Informix32.SQL_C _boundSqlCType;

    private Informix32.SQL_TYPE _boundParameterType;

    private int _boundSize;

    private int _boundScale;

    private nint _boundBuffer;

    private nint _boundIntbuffer;

    private TypeMap _originalbindtype;

    private byte _internalPrecision;

    private bool _internalShouldSerializeSize;

    private int _internalSize;

    private ParameterDirection _internalDirection;

    private byte _internalScale;

    private int _internalOffset;

    internal bool _internalUserSpecifiedType;

    private object _internalValue;

    private int _preparedOffset;

    private int _preparedSize;

    private int _preparedBufferSize;

    private object _preparedValue;

    private int _preparedIntOffset;

    private int _preparedValueOffset;

    private Informix32.SQL_C _prepared_Sql_C_Type;

    private object _value;

    private object _parent;

    private ParameterDirection _direction;

    private int _size;

    private int _offset;

    private string _sourceColumn;

    private DataRowVersion _sourceVersion;

    private bool _sourceColumnNullMapping;

    private bool _isNullable;

    private object _coercedValue;

    public override DbType DbType
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (_userSpecifiedType)
            {
                ifxTrace?.ApiExit();
                return _typemap._dbType;
            }
            ifxTrace?.ApiExit();
            return TypeMap._NVarChar._dbType;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (_typemap == null || _typemap._dbType != value)
            {
                PropertyTypeChanging();
                _typemap = TypeMap.FromDbType(value);
                _userSpecifiedType = true;
            }
            ifxTrace?.ApiExit();
        }
    }

    [DefaultValue(InformixType.NChar)]
    [DbProviderSpecificTypeProperty(true)]
    public InformixType IfxType
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            if (_userSpecifiedType)
            {
                ifxTrace?.ApiExit();
                return _typemap._odbcType;
            }
            ifxTrace?.ApiExit();
            return TypeMap._NVarChar._odbcType;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (_typemap == null || _typemap._odbcType != value)
            {
                PropertyTypeChanging();
                _typemap = TypeMap.FromOdbcType(value);
                _userSpecifiedType = true;
            }
            ifxTrace?.ApiExit();
        }
    }

    internal bool HasChanged
    {
        set
        {
            _hasChanged = value;
        }
    }

    internal bool UserSpecifiedType => _userSpecifiedType;

    public override string ParameterName
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            string parameterName = _parameterName;
            ifxTrace?.ApiExit();
            if (parameterName == null)
            {
                return "";
            }
            return parameterName;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            if (_parameterName != value)
            {
                PropertyChanging();
                _parameterName = value;
            }
            ifxTrace?.ApiExit();
        }
    }

    public new byte Precision
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return PrecisionInternal;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            PrecisionInternal = value;
            ifxTrace?.ApiExit();
        }
    }

    internal byte PrecisionInternal
    {
        get
        {
            byte b = _precision;
            if (b == 0)
            {
                b = ValuePrecision(Value);
            }
            return b;
        }
        set
        {
            if (_precision != value)
            {
                PropertyChanging();
                _precision = value;
            }
        }
    }

    public new byte Scale
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return ScaleInternal;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            ScaleInternal = value;
            ifxTrace?.ApiExit();
        }
    }

    internal byte ScaleInternal
    {
        get
        {
            byte b = _scale;
            if (!ShouldSerializeScale(b))
            {
                b = ValueScale(Value);
            }
            return b;
        }
        set
        {
            if (_scale != value || !_hasScale)
            {
                PropertyChanging();
                _scale = value;
                _hasScale = true;
            }
        }
    }

    public override object Value
    {
        get
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry();
            ifxTrace?.ApiExit();
            return _value;
        }
        set
        {
            InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
            ifxTrace?.ApiEntry(value);
            _coercedValue = null;
            _value = value;
            ifxTrace?.ApiExit();
        }
    }

    private object CoercedValue
    {
        get
        {
            return _coercedValue;
        }
        set
        {
            _coercedValue = value;
        }
    }

    public override ParameterDirection Direction
    {
        get
        {
            ParameterDirection direction = _direction;
            if (direction == 0)
            {
                return ParameterDirection.Input;
            }
            return direction;
        }
        set
        {
            if (_direction != value)
            {
                if ((uint)(value - 1) > 2u && value != ParameterDirection.ReturnValue)
                {
                    throw ADP.InvalidParameterDirection(value);
                }
                PropertyChanging();
                _direction = value;
            }
        }
    }

    public override bool IsNullable
    {
        get
        {
            return _isNullable;
        }
        set
        {
            _isNullable = value;
        }
    }

    public int Offset
    {
        get
        {
            return _offset;
        }
        set
        {
            if (value < 0)
            {
                throw ADP.InvalidOffsetValue(value);
            }
            _offset = value;
        }
    }

    public override int Size
    {
        get
        {
            int num = _size;
            if (num == 0)
            {
                num = ValueSize(Value);
            }
            return num;
        }
        set
        {
            if (_size != value)
            {
                if (value < -1)
                {
                    throw ADP.InvalidSizeValue(value);
                }
                PropertyChanging();
                _size = value;
            }
        }
    }

    public override string SourceColumn
    {
        get
        {
            string sourceColumn = _sourceColumn;
            if (sourceColumn == null)
            {
                return "";
            }
            return sourceColumn;
        }
        set
        {
            _sourceColumn = value;
        }
    }

    public override bool SourceColumnNullMapping
    {
        get
        {
            return _sourceColumnNullMapping;
        }
        set
        {
            _sourceColumnNullMapping = value;
        }
    }

    public override DataRowVersion SourceVersion
    {
        get
        {
            DataRowVersion sourceVersion = _sourceVersion;
            if (sourceVersion == 0)
            {
                return DataRowVersion.Current;
            }
            return sourceVersion;
        }
        set
        {
            switch (value)
            {
                case DataRowVersion.Original:
                case DataRowVersion.Current:
                case DataRowVersion.Proposed:
                case DataRowVersion.Default:
                    _sourceVersion = value;
                    break;
                default:
                    throw ADP.InvalidDataRowVersion(value);
            }
        }
    }

    public InformixParameter()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ifxTrace?.ApiExit();
    }

    public InformixParameter(string name, object value)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(name, value);
        ParameterName = name;
        Value = value;
        ifxTrace?.ApiExit();
    }

    public InformixParameter(string name, InformixType type)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(name, type);
        ParameterName = name;
        IfxType = type;
        ifxTrace?.ApiExit();
    }

    public InformixParameter(string name, InformixType type, int size)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(name, type, size);
        ParameterName = name;
        IfxType = type;
        Size = size;
        ifxTrace?.ApiExit();
    }

    public InformixParameter(string name, InformixType type, int size, string sourcecolumn)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(name, type, size, sourcecolumn);
        ParameterName = name;
        IfxType = type;
        Size = size;
        SourceColumn = sourcecolumn;
        ifxTrace?.ApiExit();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public InformixParameter(string parameterName, InformixType odbcType, int size, ParameterDirection parameterDirection, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, odbcType, size, parameterDirection, isNullable, precision, scale, srcColumn, srcVersion, value);
        ParameterName = parameterName;
        IfxType = odbcType;
        Size = size;
        Direction = parameterDirection;
        IsNullable = isNullable;
        PrecisionInternal = precision;
        ScaleInternal = scale;
        SourceColumn = srcColumn;
        SourceVersion = srcVersion;
        Value = value;
        ifxTrace?.ApiExit();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public InformixParameter(string parameterName, InformixType odbcType, int size, ParameterDirection parameterDirection, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
        : this()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(parameterName, odbcType, size, parameterDirection, precision, scale, sourceColumn, sourceVersion, sourceColumnNullMapping, value);
        ParameterName = parameterName;
        IfxType = odbcType;
        Size = size;
        Direction = parameterDirection;
        PrecisionInternal = precision;
        ScaleInternal = scale;
        SourceColumn = sourceColumn;
        SourceVersion = sourceVersion;
        SourceColumnNullMapping = sourceColumnNullMapping;
        Value = value;
        ifxTrace?.ApiExit();
    }

    public override void ResetDbType()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        ResetIfxType();
        ifxTrace?.ApiExit();
    }

    public void ResetIfxType()
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry();
        PropertyTypeChanging();
        _typemap = null;
        _userSpecifiedType = false;
        ifxTrace?.ApiExit();
    }

    private bool ShouldSerializePrecision()
    {
        return _precision != 0;
    }

    private bool ShouldSerializeScale()
    {
        return ShouldSerializeScale(_scale);
    }

    private bool ShouldSerializeScale(byte scale)
    {
        if (_hasScale)
        {
            if (scale == 0)
            {
                return ShouldSerializePrecision();
            }
            return true;
        }
        return false;
    }

    private int GetColumnSize(object value, TypeMap typeMap)
    {
        int num = typeMap._columnSize;
        if (0 >= num)
        {
            num = Size;
            if (num <= 0 || int.MaxValue <= num)
            {
                if (num <= 0 && (ParameterDirection.Output & _internalDirection) != 0 && typeMap._columnSize != -1)
                {
                    throw ADP.UninitializedParameterSize(1, typeMap._type);
                }
                if (InformixParameterConverter.IsNull(value))
                {
                    num = 0;
                }
                else if (value is string)
                {
                    num = ((string)value).Length;
                    if ((ParameterDirection.Output & _internalDirection) != 0 && int.MaxValue <= Size)
                    {
                        num = Math.Max(num, 4096);
                    }
                    if (Informix32.SQL_TYPE.CHAR == typeMap._sql_type || Informix32.SQL_TYPE.VARCHAR == typeMap._sql_type || Informix32.SQL_TYPE.LONGVARCHAR == typeMap._sql_type)
                    {
                        num = Encoding.Default.GetMaxByteCount(num);
                    }
                }
                else if (value is byte[])
                {
                    num = ((byte[])value).Length;
                    if ((ParameterDirection.Output & _internalDirection) != 0 && int.MaxValue <= Size)
                    {
                        num = Math.Max(num, 8192);
                    }
                }
                else if (value is char[])
                {
                    num = ((char[])value).Length;
                    if ((ParameterDirection.Output & _internalDirection) != 0 && int.MaxValue <= Size)
                    {
                        num = Math.Max(num, 8192);
                    }
                }
                num = Math.Max(2, num);
            }
        }
        return num;
    }

    private int GetColumnSize(object value, int offset, int ordinal)
    {
        if (Informix32.SQL_C.NUMERIC == _bindtype._sql_c && _internalPrecision != 0)
        {
            return Math.Min((int)_internalPrecision, 29);
        }
        int num = _bindtype._columnSize;
        if (0 >= num)
        {
            if (Informix32.SQL_C.NUMERIC == _typemap._sql_c)
            {
                num = 62;
            }
            else
            {
                num = _internalSize;
                if (!_internalShouldSerializeSize || 1073741823 <= num || num < 0)
                {
                    if (!_internalShouldSerializeSize && (ParameterDirection.Output & _internalDirection) != 0)
                    {
                        throw ADP.UninitializedParameterSize(ordinal, _bindtype._type);
                    }
                    if (value == null || Convert.IsDBNull(value))
                    {
                        num = 0;
                    }
                    else if (value is string)
                    {
                        num = ((string)value).Length - offset;
                        if ((ParameterDirection.Output & _internalDirection) != 0 && 1073741823 <= _internalSize)
                        {
                            num = Math.Max(num, 4096);
                        }
                        if (Informix32.SQL_TYPE.CHAR == _bindtype._sql_type || Informix32.SQL_TYPE.VARCHAR == _bindtype._sql_type || Informix32.SQL_TYPE.LONGVARCHAR == _bindtype._sql_type)
                        {
                            num = Encoding.Default.GetMaxByteCount(num);
                        }
                    }
                    else if (value is char[])
                    {
                        num = ((char[])value).Length - offset;
                        if ((ParameterDirection.Output & _internalDirection) != 0 && 1073741823 <= _internalSize)
                        {
                            num = Math.Max(num, 4096);
                        }
                        if (Informix32.SQL_TYPE.CHAR == _bindtype._sql_type || Informix32.SQL_TYPE.VARCHAR == _bindtype._sql_type || Informix32.SQL_TYPE.LONGVARCHAR == _bindtype._sql_type)
                        {
                            num = Encoding.Default.GetMaxByteCount(num);
                        }
                    }
                    else if (value is byte[])
                    {
                        num = ((byte[])value).Length - offset;
                        if ((ParameterDirection.Output & _internalDirection) != 0 && 1073741823 <= _internalSize)
                        {
                            num = Math.Max(num, 8192);
                        }
                    }
                    num = Math.Max(2, num);
                }
            }
        }
        return num;
    }

    private int GetValueSize(object value, int offset)
    {
        int num = 0;
        if (Informix32.SQL_C.NUMERIC == _bindtype._sql_c && _internalPrecision != 0)
        {
            return Math.Min((int)_internalPrecision, 29);
        }
        Informix32.DATA_TYPE_INDEX index = _typemap._index;
        num = (uint)(index - 21) > 3u ? _typemap._bufferSize : -1;
        if (0 >= num)
        {
            bool flag = false;
            if (value is string)
            {
                num = ((string)value).Length - offset;
                flag = true;
            }
            else if (!(value is char[]))
            {
                num = value is byte[]? ((byte[])value).Length - offset : 0;
            }
            else
            {
                num = ((char[])value).Length - offset;
                flag = true;
            }
            if (_internalShouldSerializeSize && _internalSize >= 0 && _internalSize < num && _bindtype == _originalbindtype)
            {
                num = _internalSize;
            }
            if (flag)
            {
                num *= 2;
            }
        }
        return num;
    }

    private int GetParameterSize(object value, int offset, int ordinal)
    {
        int num = _bindtype._bufferSize;
        if (0 >= num)
        {
            if (Informix32.SQL_C.NUMERIC == _typemap._sql_c)
            {
                num = 518;
            }
            else
            {
                num = _internalSize;
                if (!_internalShouldSerializeSize || 1073741823 <= num || num < 0)
                {
                    if (num <= 0 && (ParameterDirection.Output & _internalDirection) != 0)
                    {
                        throw ADP.UninitializedParameterSize(ordinal, _bindtype._type);
                    }
                    if (value == null || Convert.IsDBNull(value) || InformixParameterConverter.IsNull(value))
                    {
                        num = _bindtype._sql_c == Informix32.SQL_C.WCHAR ? 2 : 0;
                    }
                    else if (value is string)
                    {
                        num = (((string)value).Length - offset) * 2 + 2;
                    }
                    else if (value is char[])
                    {
                        num = (((char[])value).Length - offset) * 2 + 2;
                    }
                    else if (value is byte[])
                    {
                        num = ((byte[])value).Length - offset;
                    }
                    if ((ParameterDirection.Output & _internalDirection) != 0 && 1073741823 <= _internalSize)
                    {
                        num = Math.Max(num, 8192);
                    }
                }
                else if (Informix32.SQL_C.WCHAR == _bindtype._sql_c)
                {
                    if (value is string && num < ((string)value).Length && _bindtype == _originalbindtype)
                    {
                        num = ((string)value).Length;
                    }
                    num = num * 2 + 2;
                }
                else if (value is byte[] && num < ((byte[])value).Length && _bindtype == _originalbindtype)
                {
                    num = ((byte[])value).Length;
                }
            }
        }
        return num;
    }

    private byte GetParameterPrecision(object value)
    {
        if (value is InformixDecimal)
        {
            return 32;
        }
        if (value is TimeSpan || value is InformixTimeSpan)
        {
            return 7;
        }
        if (_internalPrecision != 0 && value is decimal)
        {
            if (_internalPrecision < 29)
            {
                if (_internalPrecision != 0)
                {
                    byte precision = ((SqlDecimal)(decimal)value).Precision;
                    _internalPrecision = Math.Max(_internalPrecision, precision);
                }
                return _internalPrecision;
            }
            return 29;
        }
        if (value == null || value is decimal || Convert.IsDBNull(value))
        {
            return 28;
        }
        return 0;
    }

    private byte GetParameterScale(object value)
    {
        if (!(value is decimal))
        {
            return _internalScale;
        }
        byte b = (byte)((decimal.GetBits((decimal)value)[3] & 0xFF0000) >> 16);
        if (_internalScale > 0 && _internalScale < b)
        {
            return _internalScale;
        }
        return b;
    }

    object ICloneable.Clone()
    {
        return new InformixParameter(this);
    }

    private void CopyParameterInternal()
    {
        _internalValue = Value;
        _internalPrecision = ShouldSerializePrecision() ? PrecisionInternal : ValuePrecision(_internalValue);
        _internalShouldSerializeSize = ShouldSerializeSize();
        _internalSize = _internalShouldSerializeSize ? Size : ValueSize(_internalValue);
        _internalDirection = Direction;
        _internalScale = ShouldSerializeScale() ? ScaleInternal : ValueScale(_internalValue);
        _internalOffset = Offset;
        _internalUserSpecifiedType = UserSpecifiedType;
    }

    private void CloneHelper(InformixParameter destination)
    {
        CloneHelperCore(destination);
        destination._userSpecifiedType = _userSpecifiedType;
        destination._typemap = _typemap;
        destination._parameterName = _parameterName;
        destination._precision = _precision;
        destination._scale = _scale;
        destination._hasScale = _hasScale;
    }

    internal void ClearBinding()
    {
        if (!_userSpecifiedType)
        {
            _typemap = null;
        }
        _bindtype = null;
    }

    internal void PrepareForBind(InformixCommand command, short ordinal, ref int parameterBufferSize)
    {
        CopyParameterInternal();
        object obj = ProcessAndGetParameterValue();
        int num = _internalOffset;
        int num2 = _internalSize;
        if (num > 0)
        {
            if (obj is string)
            {
                if (num > ((string)obj).Length)
                {
                    throw ADP.OffsetOutOfRangeException();
                }
            }
            else if (obj is char[])
            {
                if (num > ((char[])obj).Length)
                {
                    throw ADP.OffsetOutOfRangeException();
                }
            }
            else if (obj is byte[])
            {
                if (num > ((byte[])obj).Length)
                {
                    throw ADP.OffsetOutOfRangeException();
                }
            }
            else
            {
                num = 0;
            }
        }
        switch (_bindtype._sql_type)
        {
            case Informix32.SQL_TYPE.INFX_DECIMAL:
            case Informix32.SQL_TYPE.NUMERIC:
            case Informix32.SQL_TYPE.DECIMAL:
                if (obj is decimal)
                {
                    obj = _bindtype._odbcType != InformixType.Decimal && _bindtype._odbcType != InformixType.Money ? (InformixDecimal)(decimal)obj : ((decimal)obj).ToString();
                }
                else if (InformixParameterConverter.IsNull(obj))
                {
                    obj = InformixDecimal.Null;
                }
                break;
            case Informix32.SQL_TYPE.BIGINT:
                if (!command.Connection.IsV3Driver)
                {
                    _bindtype = TypeMap.s_VarChar;
                    if (obj != null && !Convert.IsDBNull(obj))
                    {
                        obj = ((long)obj).ToString(CultureInfo.CurrentCulture);
                        num2 = ((string)obj).Length;
                        num = 0;
                    }
                }
                break;
            case Informix32.SQL_TYPE.WLONGVARCHAR:
            case Informix32.SQL_TYPE.WVARCHAR:
            case Informix32.SQL_TYPE.WCHAR:
                if (obj is char)
                {
                    obj = obj.ToString();
                    num2 = ((string)obj).Length;
                    num = 0;
                }
                if (!command.Connection.TestTypeSupport(_bindtype._sql_type))
                {
                    if (Informix32.SQL_TYPE.WCHAR == _bindtype._sql_type)
                    {
                        _bindtype = TypeMap._Char;
                    }
                    else if (Informix32.SQL_TYPE.WVARCHAR == _bindtype._sql_type)
                    {
                        _bindtype = TypeMap.s_VarChar;
                    }
                    else if (Informix32.SQL_TYPE.WLONGVARCHAR == _bindtype._sql_type)
                    {
                        _bindtype = TypeMap._Text;
                    }
                }
                break;
        }
        Informix32.SQL_C sQL_C = _bindtype._sql_c;
        if (!command.Connection.IsV3Driver && sQL_C == Informix32.SQL_C.WCHAR)
        {
            sQL_C = Informix32.SQL_C.CHAR;
            if (obj != null && !Convert.IsDBNull(obj) && obj is string)
            {
                int lCID = CultureInfo.CurrentCulture.LCID;
                CultureInfo cultureInfo = new CultureInfo(lCID);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding encoding = Encoding.GetEncoding(cultureInfo.TextInfo.ANSICodePage);
                obj = encoding.GetBytes(obj.ToString());
                num2 = ((byte[])obj).Length;
            }
        }
        int parameterSize = GetParameterSize(obj, num, ordinal);
        switch (_bindtype._sql_type)
        {
            case Informix32.SQL_TYPE.VARBINARY:
                if (num2 > 8000)
                {
                    _bindtype = TypeMap._Image;
                }
                break;
            case Informix32.SQL_TYPE.VARCHAR:
                if (num2 > 8000)
                {
                    _bindtype = TypeMap._Text;
                }
                break;
            case Informix32.SQL_TYPE.WVARCHAR:
                if (num2 > 4000)
                {
                    _bindtype = TypeMap._NText;
                }
                break;
        }
        _prepared_Sql_C_Type = sQL_C;
        _preparedOffset = num;
        _preparedSize = num2;
        _preparedValue = obj;
        _preparedBufferSize = parameterSize;
        _preparedIntOffset = parameterBufferSize;
        _preparedValueOffset = _preparedIntOffset + nint.Size;
        parameterBufferSize += parameterSize + nint.Size;
    }

    internal void Bind(OdbcStatementHandle hstmt, InformixCommand command, short ordinal, CNativeBuffer parameterBuffer, bool allowReentrance)
    {
        Informix32.SQL_C sQL_C = _prepared_Sql_C_Type;
        Informix32.SQL_PARAM sQL_PARAM = SqlDirectionFromParameterDirection();
        int preparedOffset = _preparedOffset;
        int preparedSize = _preparedSize;
        object obj = _preparedValue;
        int valueSize = GetValueSize(obj, preparedOffset);
        int num = GetColumnSize(obj, _typemap);
        byte parameterPrecision = GetParameterPrecision(obj);
        byte b = GetParameterScale(obj);
        if (valueSize > num)
        {
            num = valueSize;
        }
        HandleRef handleRef = parameterBuffer.PtrOffset(_preparedValueOffset, _preparedBufferSize);
        HandleRef intbuffer = parameterBuffer.PtrOffset(_preparedIntOffset, nint.Size);
        if (Informix32.SQL_C.NUMERIC == sQL_C)
        {
            if (Informix32.SQL_PARAM.INPUT_OUTPUT == sQL_PARAM && obj is decimal && b < _internalScale)
            {
                while (b < _internalScale)
                {
                    obj = (decimal)obj * 10m;
                    b++;
                }
            }
            SetInputValue(obj, sQL_C, valueSize, parameterPrecision, 0, parameterBuffer);
            if (Informix32.SQL_PARAM.INPUT != sQL_PARAM)
            {
                parameterBuffer.WriteInt16(_preparedValueOffset, (short)(b << 8 | parameterPrecision));
            }
        }
        else
        {
            SetInputValue(obj, sQL_C, valueSize, preparedSize, preparedOffset, parameterBuffer);
        }
        if (!_hasChanged && _boundSqlCType == sQL_C && _boundParameterType == _bindtype._sql_type && _boundSize == num && _boundScale == b && _boundBuffer == handleRef.Handle && _boundIntbuffer == intbuffer.Handle)
        {
            return;
        }
        if (Informix32.SQL_C.SB_LOCATOR == sQL_C)
        {
            sQL_C = !(obj.GetType() == typeof(InformixClob)) ? (Informix32.SQL_C)(-102) : (Informix32.SQL_C)(-103);
        }
        Informix32.RetCode retCode = hstmt.BindParameter(ordinal, (short)sQL_PARAM, sQL_C, _bindtype._sql_type, num, b, handleRef, _preparedBufferSize, intbuffer);
        if (retCode != 0)
        {
            if ("07006" == command.GetDiagSqlState())
            {
                command.Connection.FlagRestrictedSqlBindType(_bindtype._sql_type);
                if (allowReentrance)
                {
                    Bind(hstmt, command, ordinal, parameterBuffer, allowReentrance: false);
                    return;
                }
            }
            command.Connection.HandleError(hstmt, retCode);
        }
        _hasChanged = false;
        _boundSqlCType = sQL_C;
        _boundParameterType = _bindtype._sql_type;
        _boundSize = num;
        _boundScale = b;
        _boundBuffer = handleRef.Handle;
        _boundIntbuffer = intbuffer.Handle;
        if (Informix32.SQL_C.INTERVAL_DAY_TO_SECOND == sQL_C && !InformixParameterConverter.IsNull(obj))
        {
            int num2 = 0;
            num2 = !(typeof(InformixTimeSpan) == obj.GetType()) ? Math.Abs((int)(((TimeSpan)obj).Ticks % 10000000)) : Math.Abs((int)(((InformixTimeSpan)obj).Ticks % 10000000m));
            OdbcDescriptorHandle descriptorHandle = command.GetDescriptorHandle(Informix32.SQL_ATTR.IMP_PARAM_DESC);
            retCode = descriptorHandle.SetDescriptionField1(ordinal, Informix32.SQL_DESC.DATETIME_INTERVAL_PRECISION, 9);
            if (retCode != 0)
            {
                command.Connection.HandleError(hstmt, retCode);
            }
            if (num2 > 0)
            {
                OdbcDescriptorHandle descriptorHandle2 = command.GetDescriptorHandle(Informix32.SQL_ATTR.APP_PARAM_DESC);
                retCode = descriptorHandle2.SetDescriptionField1(ordinal, Informix32.SQL_DESC.PRECISION, parameterPrecision);
                if (retCode != 0)
                {
                    command.Connection.HandleError(hstmt, retCode);
                }
            }
        }
        if (Informix32.SQL_C.INTERVAL_YEAR_TO_MONTH == sQL_C && !InformixParameterConverter.IsNull(obj))
        {
            InformixMonthSpan ifxMonthSpan = (InformixMonthSpan)obj;
            OdbcDescriptorHandle descriptorHandle3 = command.GetDescriptorHandle(Informix32.SQL_ATTR.IMP_PARAM_DESC);
            retCode = descriptorHandle3.SetDescriptionField1(ordinal, Informix32.SQL_DESC.DATETIME_INTERVAL_PRECISION, 9);
            if (retCode != 0)
            {
                command.Connection.HandleError(hstmt, retCode);
            }
        }
        if (Informix32.SQL_C.NUMERIC == sQL_C)
        {
            OdbcDescriptorHandle descriptorHandle4 = command.GetDescriptorHandle(Informix32.SQL_ATTR.APP_PARAM_DESC);
            retCode = descriptorHandle4.SetDescriptionField1(ordinal, Informix32.SQL_DESC.TYPE, 2);
            if (retCode != 0)
            {
                command.Connection.HandleError(hstmt, retCode);
            }
            int num3 = parameterPrecision;
            retCode = descriptorHandle4.SetDescriptionField1(ordinal, Informix32.SQL_DESC.PRECISION, num3);
            if (retCode != 0)
            {
                command.Connection.HandleError(hstmt, retCode);
            }
            num3 = b;
            retCode = descriptorHandle4.SetDescriptionField1(ordinal, Informix32.SQL_DESC.SCALE, num3);
            if (retCode != 0)
            {
                command.Connection.HandleError(hstmt, retCode);
            }
            retCode = descriptorHandle4.SetDescriptionField2(ordinal, Informix32.SQL_DESC.DATA_PTR, handleRef);
            if (retCode != 0)
            {
                command.Connection.HandleError(hstmt, retCode);
            }
        }
    }

    internal void GetOutputValue(CNativeBuffer parameterBuffer)
    {
        if (_hasChanged || _bindtype == null || _internalDirection == ParameterDirection.Input)
        {
            return;
        }
        TypeMap bindtype = _bindtype;
        _bindtype = null;
        int num = (int)parameterBuffer.ReadIntPtr(_preparedIntOffset);
        if (-1 == num)
        {
            Value = DBNull.Value;
        }
        else if (0 <= num || num == -3)
        {
            Value = parameterBuffer.MarshalToManaged(_preparedValueOffset, _boundSqlCType, num);
            if (_boundSqlCType == Informix32.SQL_C.CHAR && Value != null && !Convert.IsDBNull(Value))
            {
                int lCID = CultureInfo.CurrentCulture.LCID;
                CultureInfo cultureInfo = new CultureInfo(lCID);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding encoding = Encoding.GetEncoding(cultureInfo.TextInfo.ANSICodePage);
                Value = encoding.GetString((byte[])Value);
            }
            if (bindtype != _typemap && Value != null && !Convert.IsDBNull(Value) && Value.GetType() != _typemap._type)
            {
                Value = decimal.Parse((string)Value, CultureInfo.CurrentCulture);
            }
        }
    }

    private object ProcessAndGetParameterValue()
    {
        object obj = _internalValue;
        if (_internalUserSpecifiedType)
        {
            if (obj != null && !Convert.IsDBNull(obj))
            {
                Type type = obj.GetType();
                if (!type.IsArray)
                {
                    if (type != _typemap._type)
                    {
                        try
                        {
                            _typemap = TypeMap.FromSystemType(type);
                        }
                        catch (Exception ex)
                        {
                            if (!ADP.IsCatchableExceptionType(ex))
                            {
                                throw;
                            }
                            throw ADP.ParameterConversionFailed(obj, _typemap._type, ex);
                        }
                    }
                }
                else if (type == typeof(char[]))
                {
                    obj = new string((char[])obj);
                }
            }
        }
        else if (_typemap == null)
        {
            if (obj == null || Convert.IsDBNull(obj))
            {
                _typemap = TypeMap._NVarChar;
            }
            else
            {
                Type type2 = obj.GetType();
                _typemap = TypeMap.FromSystemType(type2);
            }
        }
        _originalbindtype = _bindtype = _typemap;
        return obj;
    }

    private void PropertyChanging()
    {
        _hasChanged = true;
    }

    private void PropertyTypeChanging()
    {
        PropertyChanging();
    }

    internal void SetInputValue(object value, Informix32.SQL_C sql_c_type, int cbsize, int sizeorprecision, int offset, CNativeBuffer parameterBuffer)
    {
        if (ParameterDirection.Input == _internalDirection || ParameterDirection.InputOutput == _internalDirection)
        {
            if (value == null)
            {
                parameterBuffer.WriteIntPtr(_preparedIntOffset, -1);
                return;
            }
            if (Convert.IsDBNull(value) || InformixParameterConverter.IsNull(value))
            {
                parameterBuffer.WriteIntPtr(_preparedIntOffset, -1);
                return;
            }
            if (sql_c_type == Informix32.SQL_C.WCHAR || sql_c_type == Informix32.SQL_C.BINARY || sql_c_type == Informix32.SQL_C.CHAR)
            {
                parameterBuffer.WriteIntPtr(_preparedIntOffset, cbsize);
            }
            else
            {
                parameterBuffer.WriteIntPtr(_preparedIntOffset, nint.Zero);
            }
            parameterBuffer.MarshalToNative(_preparedValueOffset, value, sql_c_type, sizeorprecision, offset);
        }
        else
        {
            _internalValue = null;
            parameterBuffer.WriteIntPtr(_preparedIntOffset, -1);
        }
    }

    private Informix32.SQL_PARAM SqlDirectionFromParameterDirection()
    {
        switch (_internalDirection)
        {
            case ParameterDirection.Input:
                return Informix32.SQL_PARAM.INPUT;
            case ParameterDirection.Output:
            case ParameterDirection.ReturnValue:
                return Informix32.SQL_PARAM.OUTPUT;
            case ParameterDirection.InputOutput:
                return Informix32.SQL_PARAM.INPUT_OUTPUT;
            default:
                return Informix32.SQL_PARAM.INPUT;
        }
    }

    private byte ValuePrecision(object value)
    {
        return ValuePrecisionCore(value);
    }

    private byte ValueScale(object value)
    {
        return ValueScaleCore(value);
    }

    private int ValueSize(object value)
    {
        return ValueSizeCore(value);
    }

    private InformixParameter(InformixParameter source)
        : this()
    {
        ADP.CheckArgumentNull(source, "source");
        source.CloneHelper(this);
        if (_value is ICloneable cloneable)
        {
            _value = cloneable.Clone();
        }
    }

    private bool ShouldSerializeSize()
    {
        return _size != 0;
    }

    private void CloneHelperCore(InformixParameter destination)
    {
        destination._value = _value;
        destination._direction = _direction;
        destination._size = _size;
        destination._offset = _offset;
        destination._sourceColumn = _sourceColumn;
        destination._sourceVersion = _sourceVersion;
        destination._sourceColumnNullMapping = _sourceColumnNullMapping;
        destination._isNullable = _isNullable;
    }

    internal object CompareExchangeParent(object value, object comparand)
    {
        object parent = _parent;
        if (comparand == parent)
        {
            _parent = value;
        }
        return parent;
    }

    internal void ResetParent()
    {
        _parent = null;
    }

    public override string ToString()
    {
        return ParameterName;
    }

    private byte ValuePrecisionCore(object value)
    {
        if (value is decimal)
        {
            return ((SqlDecimal)(decimal)value).Precision;
        }
        return 0;
    }

    private byte ValueScaleCore(object value)
    {
        if (value is decimal)
        {
            return (byte)((decimal.GetBits((decimal)value)[3] & 0xFF0000) >> 16);
        }
        return 0;
    }

    private int ValueSizeCore(object value)
    {
        if (!ADP.IsNull(value))
        {
            if (value is string text)
            {
                return text.Length;
            }
            if (value is byte[] array)
            {
                return array.Length;
            }
            if (value is char[] array2)
            {
                return array2.Length;
            }
            if (value is byte || value is char)
            {
                return 1;
            }
        }
        return 0;
    }
}
