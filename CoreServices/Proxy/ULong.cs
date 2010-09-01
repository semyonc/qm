//        Copyright (c) 2009-2010, Semyon A. Chertkov (semyonc@gmail.com)
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

namespace DataEngine.CoreServices.Proxy
{
    public sealed class ULong: ValueProxy
    {
        private ulong _value;

        public ULong(ulong value)
        {
            _value = value;
        }

        public override int GetValueCode()
        {
            return ULongFactory.Code;
        }

        public override object Value
        {
            get 
            {
                return _value;
            }
        }

        public override bool IsNumeric()
        {
            return true;
        }

        protected override bool Eq(ValueProxy val)
        {
            return _value == ((ULong)val)._value;
        }

        protected override bool Gt(ValueProxy val)
        {
            return _value > ((ULong)val)._value;
        }

        protected override ValueProxy Promote(ValueProxy val)
        {
            return new ULong(Convert.ToUInt64(val));
        }

        protected override ValueProxy Neg()
        {
            return new IntegerProxy(-(Integer)_value);
        }

        protected override ValueProxy Add(ValueProxy val)
        {
            return new IntegerProxy(_value + ((ULong)val)._value);
        }

        protected override ValueProxy Sub(ValueProxy val)
        {
            return new IntegerProxy(_value - ((ULong)val)._value);
        }

        protected override ValueProxy Mul(ValueProxy val)
        {
            return new IntegerProxy(_value * ((ULong)val)._value);
        }

        protected override ValueProxy Div(ValueProxy val)
        {
            return new DecimalProxy(Convert.ToDecimal(_value) / Convert.ToDecimal(val));
        }

        protected override Integer IDiv(ValueProxy val)
        {
            return (Integer)Convert.ToDecimal(_value / Convert.ToUInt64(val));
        }

        protected override ValueProxy Mod(ValueProxy val)
        {
            return new IntegerProxy(_value % ((ULong)val)._value);
        }

        public override TypeCode GetTypeCode()
        {
            return TypeCode.UInt64;
        }

        public override bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_value, provider);
        }

        public override byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value, provider);
        }

        public override char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value, provider);
        }

        public override DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(_value, provider);
        }

        public override decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(_value, provider);
        }

        public override double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(_value, provider);
        }

        public override short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value, provider);
        }

        public override int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(_value, provider);
        }

        public override long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(_value, provider);
        }

        public override sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(_value, provider);
        }

        public override float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(_value, provider);
        }

        public override string ToString(IFormatProvider provider)
        {
            return Convert.ToString(_value, provider);
        }

        public override object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_value, conversionType, provider);
        }

        public override ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value, provider);
        }

        public override uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value, provider);
        }

        public override ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(_value, provider);
        }
    }
}
