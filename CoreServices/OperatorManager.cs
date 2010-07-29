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
using System.Collections.Concurrent;
using System.Text;
using System.Globalization;

namespace DataEngine.CoreServices
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

    public abstract class TypeProxy
    {
        public abstract bool Eq(object arg1, object arg2);

        public abstract bool Gt(object arg1, object arg2);

        public abstract object Promote(object arg1);

        public abstract object Neg(object arg1);

        public abstract object Add(object arg1, object arg2);

        public abstract object Sub(object arg1, object arg2);
        
        public abstract object Mul(object arg1, object arg2);

        public abstract object Div(object arg1, object arg2);

        public abstract Integer IDiv(object arg1, object arg2);
        
        public abstract object Mod(object arg1, object arg2);
    }

    public class Int16Proxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) == Convert.ToInt16(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) > Convert.ToInt16(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToInt16(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToInt16(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) + Convert.ToInt16(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) - Convert.ToInt16(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) * Convert.ToInt16(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Convert.ToInt16(arg1) / Convert.ToInt16(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) % Convert.ToInt16(arg2);
        }
    }

    public class Int32Proxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) == Convert.ToInt32(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) > Convert.ToInt32(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToInt32(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToInt32(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) + Convert.ToInt32(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) - Convert.ToInt32(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) * Convert.ToInt32(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Convert.ToInt32(arg1) / Convert.ToInt32(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) % Convert.ToInt32(arg2);
        }
    }

    public class Int64Proxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) == Convert.ToInt64(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) > Convert.ToInt64(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToInt64(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToInt64(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) + Convert.ToInt64(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) - Convert.ToInt64(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) * Convert.ToInt64(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Convert.ToInt64(arg1) / Convert.ToInt64(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) % Convert.ToInt64(arg2);
        }
    }

    public class IntegerProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) == Integer.ToInteger(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) > Integer.ToInteger(arg2);
        }

        public override object Promote(object arg1)
        {
            return Integer.ToInteger(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Integer.ToInteger(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) + Integer.ToInteger(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) - Integer.ToInteger(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) * Integer.ToInteger(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) / Integer.ToInteger(arg2);
        }

        public override object Mod(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) % Integer.ToInteger(arg2);
        }
    }

    public class SingleProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) == Convert.ToSingle(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) > Convert.ToSingle(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToSingle(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToSingle(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) + Convert.ToSingle(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) - Convert.ToSingle(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) * Convert.ToSingle(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) / Convert.ToSingle(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Math.Truncate(Convert.ToSingle(arg1) / Convert.ToSingle(arg2)));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) % Convert.ToSingle(arg2);
        }
    }

    public class DoubleProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) == Convert.ToDouble(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) > Convert.ToDouble(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToDouble(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToDouble(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) + Convert.ToDouble(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) - Convert.ToDouble(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) * Convert.ToDouble(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) / Convert.ToDouble(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Math.Truncate(Convert.ToDouble(arg1) / Convert.ToDouble(arg2)));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) % Convert.ToDouble(arg2);
        }
    }

    public class DecimalProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) == Convert.ToDecimal(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) > Convert.ToDecimal(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToDecimal(arg1);
        }

        public override object Neg(object arg1)
        {
            return 0 - Convert.ToDecimal(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) + Convert.ToDecimal(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) - Convert.ToDecimal(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) * Convert.ToDecimal(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Math.Truncate(Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) % Convert.ToDecimal(arg2);
        }
    }

    public class StringProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return String.CompareOrdinal(arg1.ToString(), arg2.ToString()) == 0;
        }

        public override bool Gt(object arg1, object arg2)
        {
            return String.CompareOrdinal(arg1.ToString(), arg2.ToString()) > 0;
        }

        public override object Promote(object arg1)
        {
            return arg1.ToString();
        }

        public override object Neg(object arg1)
        {
            throw new OperatorMismatchException(Funcs.Neg, arg1, null);
        }

        public override object Add(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);
        }

        public override object Mod(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mod, arg1, arg2);
        }
    }

    public class CultureStringProxy : TypeProxy
    {
        private CultureInfo _cultureInfo;

        public CultureStringProxy(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public override bool Eq(object arg1, object arg2)
        {
            return String.Compare(arg1.ToString(), arg2.ToString(), false, _cultureInfo) == 0;
        }

        public override bool Gt(object arg1, object arg2)
        {
            return String.Compare(arg1.ToString(), arg2.ToString(), false, _cultureInfo) > 0;
        }

        public override object Promote(object arg1)
        {
            return arg1.ToString();
        }

        public override object Neg(object arg1)
        {
            throw new OperatorMismatchException(Funcs.Neg, arg1, null);
        }

        public override object Add(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);
        }

        public override object Mod(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mod, arg1, arg2);
        }
    }

    public class OperatorManager
    {
        private class Key
        {
            public Type type1;
            public Type type2;

            public Key()
            {
            }

            public Key(Type t1, Type t2)
            {
                type1 = t1;
                type2 = t2;
            }

            public override bool Equals(object obj)
            {
                Key other = obj as Key;
                if (other != null)
                    return type1 == other.type1 &&
                        type2 == other.type2;
                return false;
            }

            public override int GetHashCode()
            {
                return type1.GetHashCode() ^ (type2.GetHashCode() << 16);
            }

#if DEBUG
            public override string ToString()
            {
                return String.Format("Key({0},{1})", type1, type2);
            }
#endif
        }

        private Dictionary<Key, TypeProxy> _op1 = new Dictionary<Key, TypeProxy>();
        private Dictionary<Key, TypeProxy> _op2 = new Dictionary<Key, TypeProxy>();
        //private Key _key = new Key();

        public OperatorManager()
        {
        }

        private TypeProxy Get(object arg1, object arg2)
        {
            Key _key = new Key();
            _key.type1 = arg1.GetType();
            _key.type2 = arg2.GetType();
            TypeProxy oper;
            if (_op1.TryGetValue(_key, out oper))
                return oper;
            if (_op2.TryGetValue(_key, out oper))
                return oper;
            return null;
        }

        public void DefineProxy(Type type1, Type type2, TypeProxy proxy)
        {
            _op1.Add(new Key(type1, type2), proxy);
        }

        public void DefineProxy(Type type1, Type[] type2, TypeProxy proxy)
        {
            _op1[new Key(type1, type1)] = proxy;
            for (int k = 0; k < type2.Length; k++)
            {
                _op1[new Key(type1, type2[k])] = proxy;
                _op1[new Key(type2[k], type1)] = proxy;
            }
        }

        public void DefineProxy2(Type type1, Type type2, TypeProxy proxy)
        {
            _op2.Add(new Key(type1, type2), proxy);
        }

        public void DefineProxy2(Type type1, Type[] type2, TypeProxy proxy)
        {
            _op2[new Key(type1, type1)] = proxy;
            for (int k = 0; k < type2.Length; k++)
            {
                _op2[new Key(type1, type2[k])] = proxy;
                _op2[new Key(type2[k], type1)] = proxy;
            }
        }

        protected virtual object HandleUnsupported(object id, object arg1, object arg2)
        {
            throw new OperatorMismatchException(id, arg1, arg2);
        }

        public bool Eq(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return (bool)HandleUnsupported(Funcs.Eq, arg1, arg2);
            return oper.Eq(arg1, arg2);
        }

        public bool Eq(object arg1, object arg2, out object res)
        {
            res = null;
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return false;
            if (oper.Eq(arg1, arg2))
                res = true;
            return true;
        }

        public bool Gt(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return (bool)HandleUnsupported(Funcs.Eq, arg1, arg2);
            return oper.Gt(arg1, arg2);
        }

        public bool Gt(object arg1, object arg2, out object res)
        {
            res = null;
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return false;
            if (oper.Gt(arg1, arg2))
                res = true;
            return true;
        }

        public object Promote(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                throw new InvalidCastException();
            return oper.Promote(arg1);
        }

        public object Neg(object arg1)
        {
            TypeProxy oper = Get(arg1, arg1);
            if (oper == null)
                return HandleUnsupported(Funcs.Neg, arg1, null);
            return oper.Neg(arg1);
        }

        public object Add(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return HandleUnsupported(Funcs.Add, arg1, arg2);
            return oper.Add(arg1, arg2);
        }

        public object Sub(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return HandleUnsupported(Funcs.Sub, arg1, arg2);
            return oper.Sub(arg1, arg2);
        }

        public object Mul(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return HandleUnsupported(Funcs.Mul, arg1, arg2);
            return oper.Mul(arg1, arg2);
        }

        public object Div(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return HandleUnsupported(Funcs.Div, arg1, arg2);
            return oper.Div(arg1, arg2);
        }

        public Integer IDiv(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return (Integer)HandleUnsupported(Funcs.IDiv, arg1, arg2);
            return oper.IDiv(arg1, arg2);
        }

        public object Mod(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return HandleUnsupported(Funcs.Mod, arg1, arg2);
            return oper.Mod(arg1, arg2);
        }

    }
}
