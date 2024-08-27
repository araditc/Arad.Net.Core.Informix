using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using Arad.Net.Core.Informix.System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.Reflection;



namespace Arad.Net.Core.Informix;
internal sealed class InformixParameterConverter : ExpandableObjectConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        if (destinationType == typeof(InstanceDescriptor))
        {
            return true;
        }
        return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        object obj = null;
        if (destinationType == null)
        {
            throw ADP.ArgumentNull("destinationType");
        }
        if (destinationType == typeof(InstanceDescriptor) && value is InformixParameter)
        {
            InformixParameter ifxParameter = (InformixParameter)value;
            int num = 0;
            if (InformixType.Char != ifxParameter.IfxType)
            {
                num |= 1;
            }
            if (ifxParameter.Size != 0)
            {
                num |= 2;
            }
            if (!ADP.IsEmpty(ifxParameter.SourceColumn))
            {
                num |= 4;
            }
            if (ifxParameter.Value != null)
            {
                num |= 8;
            }
            if (ParameterDirection.Input != ifxParameter.Direction || ifxParameter.IsNullable || ifxParameter.Precision != 0 || ifxParameter.Scale != 0 || DataRowVersion.Current != ifxParameter.SourceVersion)
            {
                num |= 0x10;
            }
            Type[] types;
            object[] arguments;
            switch (num)
            {
                case 0:
                case 1:
                    types = new Type[2]
                    {
                    typeof(string),
                    typeof(InformixType)
                    };
                    arguments = new object[2] { ifxParameter.ParameterName, ifxParameter.IfxType };
                    break;
                case 2:
                case 3:
                    types = new Type[3]
                    {
                    typeof(string),
                    typeof(InformixType),
                    typeof(int)
                    };
                    arguments = new object[3] { ifxParameter.ParameterName, ifxParameter.IfxType, ifxParameter.Size };
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    types = new Type[4]
                    {
                    typeof(string),
                    typeof(InformixType),
                    typeof(int),
                    typeof(string)
                    };
                    arguments = new object[4] { ifxParameter.ParameterName, ifxParameter.IfxType, ifxParameter.Size, ifxParameter.SourceColumn };
                    break;
                case 8:
                    types = new Type[2]
                    {
                    typeof(string),
                    typeof(object)
                    };
                    arguments = new object[2] { ifxParameter.ParameterName, ifxParameter.Value };
                    break;
                default:
                    types = new Type[10]
                    {
                    typeof(string),
                    typeof(InformixType),
                    typeof(int),
                    typeof(ParameterDirection),
                    typeof(bool),
                    typeof(byte),
                    typeof(byte),
                    typeof(string),
                    typeof(DataRowVersion),
                    typeof(object)
                    };
                    arguments = new object[10] { ifxParameter.ParameterName, ifxParameter.IfxType, ifxParameter.Size, ifxParameter.Direction, ifxParameter.IsNullable, ifxParameter.Precision, ifxParameter.Scale, ifxParameter.SourceColumn, ifxParameter.SourceVersion, ifxParameter.Value };
                    break;
            }
            ConstructorInfo constructor = typeof(InformixParameter).GetConstructor(types);
            if (null != constructor)
            {
                obj = new InstanceDescriptor(constructor, arguments);
            }
        }
        if (obj == null)
        {
            obj = base.ConvertTo(context, culture, value, destinationType);
        }
        return obj;
    }

    internal static bool IsNull(object val)
    {
        InformixTrace ifxTrace = InformixTrace.GetIfxTrace();
        ifxTrace?.ApiEntry(val);
        bool result = false;
        if (val == null)
        {
            result = true;
        }
        else if (val == DBNull.Value)
        {
            result = true;
        }
        else
        {
            Type type = val.GetType();
            if (typeof(InformixTimeSpan) == type || typeof(InformixMonthSpan) == type || typeof(InformixDateTime) == type || typeof(InformixDecimal) == type || typeof(InformixClob) == type || typeof(InformixBlob) == type)
            {
                result = ((INullable)val).IsNull;
            }
        }
        ifxTrace?.ApiExit();
        return result;
    }
}
