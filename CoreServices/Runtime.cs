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
using System.Linq;
using System.Text;
using System.Globalization;

namespace DataEngine.CoreServices
{
    public class Runtime
    {
        public static object DynamicAdd(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetTypeCode(arg1, arg2))
                    {
                        case TypeCode.Int32:
                            return Convert.ToInt32(arg1) + Convert.ToInt32(arg2);

                        case TypeCode.UInt32:
                            return Convert.ToUInt32(arg1) + Convert.ToUInt32(arg2);

                        case TypeCode.Int64:
                            return Convert.ToInt64(arg1) + Convert.ToInt64(arg2);

                        case TypeCode.UInt64:
                            return Convert.ToUInt64(arg1) + Convert.ToUInt64(arg2);

                        case TypeCode.Single:
                            return Convert.ToSingle(arg1) + Convert.ToSingle(arg2);

                        case TypeCode.Double:
                            return Convert.ToDouble(arg1) + Convert.ToDouble(arg2);

                        case TypeCode.Decimal:
                            return Convert.ToDecimal(arg1) + Convert.ToDecimal(arg2);

                        case TypeCode.String:
                            return Convert.ToDouble(arg1, CultureInfo.InvariantCulture) +
                                Convert.ToDouble(arg2, CultureInfo.InvariantCulture);

                        default:
                            throw new InvalidCastException();
                    }
                }
                else
                    throw new InvalidCastException();
        }

        public static object DynamicSub(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetTypeCode(arg1, arg2))
                    {
                        case TypeCode.Int32:
                            return Convert.ToInt32(arg1) - Convert.ToInt32(arg2);

                        case TypeCode.UInt32:
                            return Convert.ToUInt32(arg1) - Convert.ToUInt32(arg2);

                        case TypeCode.Int64:
                            return Convert.ToInt64(arg1) - Convert.ToInt64(arg2);

                        case TypeCode.UInt64:
                            return Convert.ToUInt64(arg1) - Convert.ToUInt64(arg2);

                        case TypeCode.Single:
                            return Convert.ToSingle(arg1) - Convert.ToSingle(arg2);

                        case TypeCode.Double:
                            return Convert.ToDouble(arg1) - Convert.ToDouble(arg2);

                        case TypeCode.Decimal:
                            return Convert.ToDecimal(arg1) - Convert.ToDecimal(arg2);

                        case TypeCode.String:
                            return Convert.ToDouble(arg1, CultureInfo.InvariantCulture) -
                                Convert.ToDouble(arg2, CultureInfo.InvariantCulture);

                        default:
                            throw new InvalidCastException();
                    }
                }
                else
                    throw new InvalidCastException();
        }

        public static object DynamicMul(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetTypeCode(arg1, arg2))
                    {
                        case TypeCode.Int32:
                            return Convert.ToInt32(arg1) * Convert.ToInt32(arg2);

                        case TypeCode.UInt32:
                            return Convert.ToUInt32(arg1) * Convert.ToUInt32(arg2);

                        case TypeCode.Int64:
                            return Convert.ToInt64(arg1) * Convert.ToInt64(arg2);

                        case TypeCode.UInt64:
                            return Convert.ToUInt64(arg1) * Convert.ToUInt64(arg2);

                        case TypeCode.Single:
                            return Convert.ToSingle(arg1) * Convert.ToSingle(arg2);

                        case TypeCode.Double:
                            return Convert.ToDouble(arg1) * Convert.ToDouble(arg2);

                        case TypeCode.Decimal:
                            return Convert.ToDecimal(arg1) * Convert.ToDecimal(arg2);

                        case TypeCode.String:
                            return Convert.ToDouble(arg1, CultureInfo.InvariantCulture) * 
                                Convert.ToDouble(arg2, CultureInfo.InvariantCulture);

                        default:
                            throw new InvalidCastException();
                    }
                }
                else
                    throw new InvalidCastException();
        }

        public static object DynamicDiv(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetTypeCode(arg1, arg2))
                    {
                        case TypeCode.Int32:
                            return Convert.ToInt32(arg1) / Convert.ToInt32(arg2);

                        case TypeCode.UInt32:
                            return Convert.ToUInt32(arg1) / Convert.ToUInt32(arg2);

                        case TypeCode.Int64:
                            return Convert.ToInt64(arg1) / Convert.ToInt64(arg2);

                        case TypeCode.UInt64:
                            return Convert.ToUInt64(arg1) / Convert.ToUInt64(arg2);

                        case TypeCode.Single:
                            return Convert.ToSingle(arg1) / Convert.ToSingle(arg2);

                        case TypeCode.Double:
                            return Convert.ToDouble(arg1) / Convert.ToDouble(arg2);

                        case TypeCode.Decimal:
                            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);

                        case TypeCode.String:
                            return Convert.ToDouble(arg1, CultureInfo.InvariantCulture) / 
                                Convert.ToDouble(arg2, CultureInfo.InvariantCulture);

                        default:
                            throw new InvalidCastException();
                    }
                }
                else
                    throw new InvalidCastException();
        }

        public static object DynamicEq(object arg1, object arg2)
        {
            if (arg1 == arg2)
                return true;
            else if (arg1 == null || arg2 == null)
                return null;
            else if (arg1 is IComparable && arg2 is IComparable)
            {
                TypeCode typecode = TypeConverter.GetTypeCode(arg1, arg2);
                object val1 = TypeConverter.ChangeType(arg1, typecode);
                object val2 = TypeConverter.ChangeType(arg2, typecode);
                if (((IComparable)val1).CompareTo(val2) == 0)
                    return true;
            }
            return null;
        }

        public static object DynamicGt(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IComparable && arg2 is IComparable)
                {
                    TypeCode typecode = TypeConverter.GetTypeCode(arg1, arg2);
                    object val1 = TypeConverter.ChangeType(arg1, typecode);
                    object val2 = TypeConverter.ChangeType(arg2, typecode);
                    if (((IComparable)val1).CompareTo(val2) > 0)
                        return true;
                }
                else
                    throw new InvalidOperationException();
            return null;
        }

        public static bool IsAtom(object o)
        {
            return Lisp.IsNode(o);
        }

        public static bool IsDBNull(object o)
        {
            return o == DBNull.Value;
        }
    }
}
