//        Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace DataEngine.CoreServices
{
    public enum NumericCode
    {        
        Byte = 0,
        unsignedByte = 1,
        Short = 2,
        unsignedShort = 3,        
        Int = 4,
        unsignedInt = 5,
        Long = 6,
        unsignedLong = 7,
        Integer = 8,
        Decimal = 9,
        Float = 10,
        Double = 11,
        Unknown = 12
    }

    public class TypeConverter
    {
        private static NumericCode[] typeCodeToNumericCode;

        static TypeConverter()
        {
            BuildMap();
        }

        private static void BuildMap()
        {
            Array values = Enum.GetValues(typeof(TypeCode));
            int maxval = 0;
            for (int i = 0; i < values.Length; i++)
            {
                int val = Convert.ToInt32(values.GetValue(i));
                if (val > maxval)
                    maxval = val;
            }
            typeCodeToNumericCode = new NumericCode[maxval + 1];
            for (int i = 0; i < values.Length; i++)
            {
                NumericCode code;
                switch ((TypeCode)values.GetValue(i))
                {
                    case TypeCode.Byte:
                        code = NumericCode.Byte;
                        break;

                    case TypeCode.SByte:
                        code = NumericCode.unsignedByte;
                        break;

                    case TypeCode.Int16:
                        code = NumericCode.Short;
                        break;

                    case TypeCode.UInt16:
                        code = NumericCode.unsignedShort;
                        break;

                    case TypeCode.Int32:
                        code = NumericCode.Int;
                        break;

                    case TypeCode.UInt32:
                        code = NumericCode.unsignedInt;
                        break;

                    case TypeCode.Int64:
                        code = NumericCode.Long;
                        break;

                    case TypeCode.UInt64:
                        code = NumericCode.unsignedLong;
                        break;

                    case TypeCode.Decimal:
                        code = NumericCode.Decimal;
                        break;

                    case TypeCode.Single:
                        code = NumericCode.Float;
                        break;

                    case TypeCode.Double:
                        code = NumericCode.Double;
                        break;

                    default:
                        code = NumericCode.Unknown;
                        break;
                }
                typeCodeToNumericCode[Convert.ToInt32(values.GetValue(i))] = code;
            }
        }

        public static NumericCode GetNumericCode(Type type)
        {
            if (type == typeof(Integer))
                return NumericCode.Integer;
            return typeCodeToNumericCode[(int)Type.GetTypeCode(type)];
        }

        public static NumericCode GetNumericCode(TypeCode typecode)
        {
            return typeCodeToNumericCode[(int)typecode];
        }

        public static NumericCode GetNumericCode(NumericCode code1, NumericCode code2)
        {
            int nc1 = (int)code1;
            int nc2 = (int)code2;
            if (nc1 > nc2)
                return code1;
            else
                return code2;
        }

        private static TypeCode GetValueTypeCode(Object value)
        {
            IConvertible convert = value as IConvertible;
            if (convert != null)
                return convert.GetTypeCode();
            else
                return Type.GetTypeCode(value.GetType());
        }

        public static NumericCode GetNumericCode(Object object1, Object object2)
        {
            return GetNumericCode(GetNumericCode(object1), GetNumericCode(object2));
        }

        private static NumericCode GetNumericCode(object obj)
        {
            return GetNumericCode(obj.GetType());
        }

        public static Type GetTypeByNumericCode(NumericCode code)
        {
            switch (code)
            {
                case NumericCode.Byte:
                    return typeof(System.SByte);
                case NumericCode.unsignedByte:
                    return typeof(System.Byte);
                case NumericCode.Short:
                    return typeof(System.Int16);
                case NumericCode.unsignedShort:
                    return typeof(System.UInt16);
                case NumericCode.Int:
                    return typeof(System.Int32);
                case NumericCode.unsignedInt:
                    return typeof(System.UInt32);
                case NumericCode.Long:
                    return typeof(System.Int64);
                case NumericCode.unsignedLong:
                    return typeof(System.UInt64);
                case NumericCode.Integer:
                    return typeof(Integer);
                case NumericCode.Decimal:
                    return typeof(System.Decimal);
                case NumericCode.Float:
                    return typeof(System.Single);
                case NumericCode.Double:
                    return typeof(System.Double);
                default:
                    return typeof(System.Object);
            }
        }

        public static Type GetType(Type type1, Type type2)
        {
            return GetTypeByNumericCode(GetNumericCode(
                GetNumericCode(type1), GetNumericCode(type2)));
        }

        public static int GetValueSize(Object value)
        {
            if (value is String)
                return ((String)value).Length;
            else
                return 0;
        }

        public static bool IsNumberType(Type type)
        {
            return GetNumericCode(type) != NumericCode.Unknown;
        }

        public static object ChangeType(object value, NumericCode code)
        {
            if (code == NumericCode.Integer)
                return (Integer)((Decimal)Convert.ChangeType(value, TypeCode.Decimal));
            return Convert.ChangeType(value, GetTypeByNumericCode(code));
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
    }
}
