/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataEngine.CoreServices
{
    public class TypeConverter
    {       
        private static TypeCode[] _typeSeniority;

        static TypeConverter()
        {
            _typeSeniority = new TypeCode[]  
                { TypeCode.String,
                  TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16,
                  TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64,
                  TypeCode.Single, TypeCode.Double, TypeCode.Decimal, TypeCode.DateTime };
        }

        public static TypeCode GetTypeCode(Object object1, Object object2)
        {
            return GetTypeCode(object1.GetType(), object2.GetType());
        }

        public static TypeCode GetTypeCode(Type t1, Type t2)
        {
            return GetTypeCode(Type.GetTypeCode(t1), Type.GetTypeCode(t2));
        }

        public static TypeCode GetTypeCode(TypeCode typecode1, TypeCode typecode2)
        {
            if (typecode1 == typecode2)
                return typecode1;
            else if (typecode1 == TypeCode.Object || typecode2 == TypeCode.Object)
                return TypeCode.Object;
            else
            {
                int index1 = -1;
                for(int k = 0; k < _typeSeniority.Length; k++)
                    if (_typeSeniority[k] == typecode1)
                    {
                        index1 = k;
                        break;
                    }
                if (index1 == -1)
                    throw new InvalidCastException();
                
                int index2 = -1;
                for (int k = 0; k < _typeSeniority.Length; k++)
                    if (_typeSeniority[k] == typecode2)
                    {
                        index2 = k;
                        break;
                    }
                if (index2 == -1)
                    throw new InvalidCastException();

                TypeCode typecode;
                if (index1 > index2)
                    typecode = typecode1;
                else
                    typecode = typecode2;

                switch (typecode)
                {
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        return TypeCode.Int32;

                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                        return TypeCode.UInt32;                    

                    default:
                        return typecode;
                }
            }
        }

        public static Type GetTypeByTypeCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Empty:
                    return null;
                case TypeCode.DBNull:
                    return typeof(DBNull);
                case TypeCode.Boolean:
                    return typeof(System.Boolean);
                case TypeCode.Int16:
                    return typeof(System.Int16);
                case TypeCode.Int32:
                    return typeof(System.Int32);
                case TypeCode.Int64:
                    return typeof(System.Int64);
                case TypeCode.UInt16:
                    return typeof(System.UInt16);
                case TypeCode.UInt32:
                    return typeof(System.UInt32);
                case TypeCode.UInt64:
                    return typeof(System.UInt64);
                case TypeCode.SByte:
                    return typeof(System.SByte);
                case TypeCode.Byte:
                    return typeof(System.Byte);
                case TypeCode.Single:
                    return typeof(System.Single);
                case TypeCode.Decimal:
                    return typeof(System.Decimal);
                case TypeCode.Double:
                    return typeof(System.Double);
                case TypeCode.Char:
                case TypeCode.String:
                    return typeof(System.String);
                case TypeCode.DateTime:
                    return typeof(System.DateTime);
                case TypeCode.Object:
                    return typeof(System.Object);
                default:
                    throw new InvalidCastException();
            }
        }

        public static Type GetType(Type type1, Type type2)
        {
            return GetTypeByTypeCode(GetTypeCode(type1, type2));
        }

        public static int GetValueSize(Object value)
        {
            if (value is String)
                return ((String)value).Length;
            else
                return 0;
        }

        public static object ChangeType(object value, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Empty:
                    return null;
                case TypeCode.DBNull:
                    return typeof(DBNull);
                case TypeCode.Boolean:
                    return Convert.ChangeType(value, typeof(System.Boolean));
                case TypeCode.Int16:
                    return Convert.ChangeType(value, typeof(System.Int16));
                case TypeCode.Int32:
                    return Convert.ChangeType(value, typeof(System.Int32));
                case TypeCode.Int64:
                    return Convert.ChangeType(value, typeof(System.Int64));
                case TypeCode.UInt16:
                    return Convert.ChangeType(value, typeof(System.UInt16));
                case TypeCode.UInt32:
                    return Convert.ChangeType(value, typeof(System.UInt32));
                case TypeCode.UInt64:
                    return Convert.ChangeType(value, typeof(System.UInt64));
                case TypeCode.SByte:
                    return Convert.ChangeType(value, typeof(System.SByte));
                case TypeCode.Byte:
                    return Convert.ChangeType(value, typeof(System.Byte));
                case TypeCode.Single:
                    return Convert.ChangeType(value, typeof(System.Single));
                case TypeCode.Decimal:
                    return Convert.ChangeType(value, typeof(System.Decimal));
                case TypeCode.Double:
                    return Convert.ChangeType(value, typeof(System.Double));
                case TypeCode.Char:
                    return Convert.ChangeType(value, typeof(System.Char));
                case TypeCode.String:
                    return Convert.ChangeType(value, typeof(System.String));
                case TypeCode.DateTime:
                    return Convert.ChangeType(value, typeof(System.DateTime));
                case TypeCode.Object:
                    return value;
                default:
                    throw new InvalidCastException();
            }
        }

    }
}
