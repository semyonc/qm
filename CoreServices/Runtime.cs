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
    public class Runtime
    {
        public class OperatorMismatchException : Exception
        {
            public OperatorMismatchException(object id, object arg1, object arg2)
            {
                ID = id;
                Arg1 = arg1;
                Arg2 = arg2;
            }

            public OperatorMismatchException(object id, object arg)
                : this(id, arg, null)
            {
            }

            public object ID { get; private set; }

            public object Arg1 { get; private set; }

            public object Arg2 { get; private set; }
        }

        public static object DynamicNeg(object arg)
        {
            if (arg == null)
                return null;
            switch (TypeConverter.GetNumericCode(arg.GetType()))
            {
                case NumericCode.Byte:
                    return -(SByte)arg;

                case NumericCode.Short:
                    return -(Int16)arg;

                case NumericCode.unsignedByte:
                case NumericCode.unsignedShort:
                case NumericCode.Int:
                    return -Convert.ToInt32(arg);

                case NumericCode.Long:
                case NumericCode.unsignedInt:
                    return -Convert.ToInt64(arg);

                case NumericCode.unsignedLong:
                    return -(Integer)Convert.ToDecimal(arg);

                case NumericCode.Integer:
                    return -Integer.ToInteger(arg);

                case NumericCode.Float:
                    return -(Single)arg;

                case NumericCode.Double:
                    return -(Double)arg;

                case NumericCode.Decimal:
                    return 0 -(Decimal)arg;

                default:
                    throw new OperatorMismatchException(Funcs.Neg, arg);
            }
        }

        public static object DynamicAdd(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
            {
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetNumericCode(arg1, arg2))
                    {

                        case NumericCode.Byte:
                            return Convert.ToSByte(arg1) + Convert.ToSByte(arg2);

                        case NumericCode.unsignedByte:
                            return Convert.ToByte(arg1) + Convert.ToByte(arg2);

                        case NumericCode.Short:
                            return Convert.ToInt16(arg1) + Convert.ToInt16(arg2);

                        case NumericCode.unsignedShort:
                            return Convert.ToUInt16(arg1) + Convert.ToUInt16(arg2);

                        case NumericCode.Int:
                            return Convert.ToInt32(arg1) + Convert.ToInt32(arg2);

                        case NumericCode.unsignedInt:
                            return Convert.ToUInt32(arg1) + Convert.ToUInt32(arg2);

                        case NumericCode.Long:
                            return Convert.ToInt64(arg1) + Convert.ToInt64(arg2);

                        case NumericCode.unsignedLong:
                            return Convert.ToUInt64(arg1) + Convert.ToUInt64(arg2);

                        case NumericCode.Integer:
                            return (Integer)Convert.ToDecimal(arg1) + (Integer)Convert.ToDecimal(arg2);

                        case NumericCode.Decimal:
                            return Convert.ToDecimal(arg1) + Convert.ToDecimal(arg2);

                        case NumericCode.Float:
                            return Convert.ToSingle(arg1) + Convert.ToSingle(arg2);

                        case NumericCode.Double:
                            return Convert.ToDouble(arg1) + Convert.ToDouble(arg2);
                    }
                }
            }
            IRuntimeExtension ext = arg1 as IRuntimeExtension;
            if (ext != null)
                return ext.OperatorAdd(arg1, arg2);
            ext = arg2 as IRuntimeExtension;
            if (ext != null)
                return ext.OperatorAdd(arg1, arg2);
            else
                throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
        }

        public static object DynamicSub(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
            {
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetNumericCode(arg1, arg2))
                    {

                        case NumericCode.Byte:
                            return Convert.ToSByte(arg1) - Convert.ToSByte(arg2);

                        case NumericCode.unsignedByte:
                            return Convert.ToByte(arg1) - Convert.ToByte(arg2);

                        case NumericCode.Short:
                            return Convert.ToInt16(arg1) - Convert.ToInt16(arg2);

                        case NumericCode.unsignedShort:
                            return Convert.ToUInt16(arg1) - Convert.ToUInt16(arg2);

                        case NumericCode.Int:
                            return Convert.ToInt32(arg1) - Convert.ToInt32(arg2);

                        case NumericCode.unsignedInt:
                            return Convert.ToUInt32(arg1) - Convert.ToUInt32(arg2);

                        case NumericCode.Long:
                            return Convert.ToInt64(arg1) - Convert.ToInt64(arg2);

                        case NumericCode.unsignedLong:
                            return Convert.ToUInt64(arg1) - Convert.ToUInt64(arg2);

                        case NumericCode.Integer:
                            return (Integer)Convert.ToDecimal(arg1) - (Integer)Convert.ToDecimal(arg2);

                        case NumericCode.Decimal:
                            return Convert.ToDecimal(arg1) - Convert.ToDecimal(arg2);

                        case NumericCode.Float:
                            return Convert.ToSingle(arg1) - Convert.ToSingle(arg2);

                        case NumericCode.Double:
                            return Convert.ToDouble(arg1) - Convert.ToDouble(arg2);
                    }
                }
                IRuntimeExtension ext = arg1 as IRuntimeExtension;
                if (ext != null)
                    return ext.OperatorSub(arg1, arg2);
                ext = arg2 as IRuntimeExtension;
                if (ext != null)
                    return ext.OperatorSub(arg1, arg2);
                else
                    throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
            }
        }

        public static object DynamicMul(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
            {
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetNumericCode(arg1, arg2))
                    {

                        case NumericCode.Byte:
                            return Convert.ToSByte(arg1) * Convert.ToSByte(arg2);

                        case NumericCode.unsignedByte:
                            return Convert.ToByte(arg1) * Convert.ToByte(arg2);

                        case NumericCode.Short:
                            return Convert.ToInt16(arg1) * Convert.ToInt16(arg2);

                        case NumericCode.unsignedShort:
                            return Convert.ToUInt16(arg1) * Convert.ToUInt16(arg2);

                        case NumericCode.Int:
                            return Convert.ToInt32(arg1) * Convert.ToInt32(arg2);

                        case NumericCode.unsignedInt:
                            return Convert.ToUInt32(arg1) * Convert.ToUInt32(arg2);

                        case NumericCode.Long:
                            return Convert.ToInt64(arg1) * Convert.ToInt64(arg2);

                        case NumericCode.unsignedLong:
                            return Convert.ToUInt64(arg1) * Convert.ToUInt64(arg2);

                        case NumericCode.Integer:
                            return (Integer)Convert.ToDecimal(arg1) * (Integer)Convert.ToDecimal(arg2);

                        case NumericCode.Decimal:
                            return Convert.ToDecimal(arg1) * Convert.ToDecimal(arg2);

                        case NumericCode.Float:
                            return Convert.ToSingle(arg1) * Convert.ToSingle(arg2);

                        case NumericCode.Double:
                            return Convert.ToDouble(arg1) * Convert.ToDouble(arg2);
                    }
                }
                IRuntimeExtension ext = arg1 as IRuntimeExtension;
                if (ext != null)
                    return ext.OperatorMul(arg1, arg2);
                ext = arg2 as IRuntimeExtension;
                if (ext != null)
                    return ext.OperatorMul(arg1, arg2);
                else
                    throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
            }
        }

        public static object DynamicDiv(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
            {
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetNumericCode(arg1, arg2))
                    {
                        case NumericCode.Float:
                            return Convert.ToSingle(arg1) / Convert.ToSingle(arg2);

                        case NumericCode.Double:
                            return Convert.ToDouble(arg1) / Convert.ToDouble(arg2);

                        case NumericCode.Unknown:
                            {
                                IRuntimeExtension ext = arg1 as IRuntimeExtension;
                                if (ext != null)
                                    return ext.OperatorDiv(arg1, arg2);
                                ext = arg2 as IRuntimeExtension;
                                if (ext != null)
                                    return ext.OperatorDiv(arg1, arg2);
                                else
                                    throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
                            }

                        default:
                            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
                    }
                }
                else
                {
                    IRuntimeExtension ext = arg1 as IRuntimeExtension;
                    if (ext != null)
                        return ext.OperatorDiv(arg1, arg2);
                    ext = arg2 as IRuntimeExtension;
                    if (ext != null)
                        return ext.OperatorDiv(arg1, arg2);
                    else
                        throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
                }
            }
        }

        public static object DynamicMod(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetNumericCode(arg1, arg2))
                    {

                        case NumericCode.Byte:
                            return Convert.ToSByte(arg1) % Convert.ToSByte(arg2);

                        case NumericCode.unsignedByte:
                            return Convert.ToByte(arg1) % Convert.ToByte(arg2);

                        case NumericCode.Short:
                            return Convert.ToInt16(arg1) % Convert.ToInt16(arg2);

                        case NumericCode.unsignedShort:
                            return Convert.ToUInt16(arg1) % Convert.ToUInt16(arg2);

                        case NumericCode.Int:
                            return Convert.ToInt32(arg1) % Convert.ToInt32(arg2);

                        case NumericCode.unsignedInt:
                            return Convert.ToUInt32(arg1) % Convert.ToUInt32(arg2);

                        case NumericCode.Long:
                            return Convert.ToInt64(arg1) % Convert.ToInt64(arg2);

                        case NumericCode.unsignedLong:
                            return Convert.ToUInt64(arg1) % Convert.ToUInt64(arg2);

                        case NumericCode.Integer:
                            return (Integer)Convert.ToDecimal(arg1) % (Integer)Convert.ToDecimal(arg2);

                        case NumericCode.Decimal:
                            return Convert.ToDecimal(arg1) % Convert.ToDecimal(arg2);

                        case NumericCode.Float:
                            return Convert.ToSingle(arg1) % Convert.ToSingle(arg2);

                        case NumericCode.Double:
                            return Convert.ToDouble(arg1) % Convert.ToDouble(arg2);

                        default:
                            throw new OperatorMismatchException(Funcs.Mod, arg1, arg2);
                    }
                }
                else
                    throw new OperatorMismatchException(Funcs.Mod, arg1, arg2);            
        }

        public static Integer DynamicIDiv(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                throw new InvalidOperationException();
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetNumericCode(arg1, arg2))
                    {
                        case NumericCode.Byte:
                            return (Integer)Convert.ToDecimal(Convert.ToSByte(arg1) / Convert.ToSByte(arg2));

                        case NumericCode.unsignedByte:
                            return (Integer)Convert.ToDecimal(Convert.ToByte(arg1) / Convert.ToByte(arg2));

                        case NumericCode.Short:
                            return (Integer)Convert.ToDecimal(Convert.ToInt16(arg1) / Convert.ToInt16(arg2));

                        case NumericCode.unsignedShort:
                            return (Integer)Convert.ToDecimal(Convert.ToUInt16(arg1) / Convert.ToUInt16(arg2));

                        case NumericCode.Int:
                            return (Integer)Convert.ToDecimal(Convert.ToInt32(arg1) / Convert.ToInt32(arg2));

                        case NumericCode.unsignedInt:
                            return (Integer)Convert.ToDecimal(Convert.ToUInt32(arg1) / Convert.ToUInt32(arg2));

                        case NumericCode.Long:
                            return (Integer)Convert.ToDecimal(Convert.ToInt64(arg1) / Convert.ToInt64(arg2));

                        case NumericCode.unsignedLong:
                            return (Integer)Convert.ToDecimal(Convert.ToUInt64(arg1) / Convert.ToUInt64(arg2));

                        case NumericCode.Decimal:
                            return (Integer)Convert.ToDecimal(Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2));

                        case NumericCode.Float:
                            return (Integer)Convert.ToDecimal(Math.Truncate(Convert.ToSingle(arg1) / Convert.ToSingle(arg2)));

                        case NumericCode.Double:
                            return (Integer)Convert.ToDecimal(Math.Truncate(Convert.ToDouble(arg1) / Convert.ToDouble(arg2)));

                        default:
                            throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);
                    }
                }
                else
                    throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);            
        }

        public static object DynamicEq([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.OperatorEq(arg1, arg2);
        }

        public static object DynamicGt([Implict] Executive engine, object arg1, object arg2)
        {
            return engine.OperatorGt(arg1, arg2);
        }

        public static bool IsAtom(object o)
        {
            return Lisp.IsNode(o);
        }

        public static bool IsDBNull(object o)
        {
            return o == DBNull.Value;
        }

        public static double ConvertToDouble(object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        // preventing xs:float(-0.0) = -0
        public static decimal op_UnaryNegation(decimal a)
        {
            return 0 - a;
        }

        public static decimal op_Divide(sbyte a, sbyte b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(byte a, byte b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(short a, short b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(ushort a, ushort b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(int a, int b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(uint a, uint b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(long a, long b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(ulong a, ulong b)
        {
            return Convert.ToDecimal(a) / Convert.ToDecimal(b);
        }

        public static decimal op_Divide(Integer a, Integer b)
        {
            return (Decimal)a / (Decimal)b;
        }

        public static Integer op_IntegerDivide(sbyte a, sbyte b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(byte a, byte b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(short a, short b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(ushort a, ushort b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(int a, int b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(uint a, uint b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(long a, long b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(ulong a, ulong b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(float a, float b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(double a, double b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }

        public static Integer op_IntegerDivide(decimal a, decimal b)
        {
            return (Integer)Convert.ToDecimal(a / b);
        }
    }
}
