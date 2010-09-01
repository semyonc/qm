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
    public class ULongFactory: ValueProxyFactory
    {
        public const int Code = 20;

        public override ValueProxy Create(object value)
        {
            return new ULong((ulong)value);
        }

        public override int GetValueCode()
        {
            return Code;
        }

        public override Type GetValueType()
        {
            return typeof(UInt64);
        }

        public override Type GetResultType()
        {
            return typeof(Integer);
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override int Compare(ValueProxyFactory other)
        {
            switch (other.GetValueCode())
            {
                case SByteProxyFactory.Code:
                case ByteProxyFactory.Code:
                case UShortFactory.Code:
                case UIntFactory.Code:
                case ShortFactory.Code:
                case IntFactory.Code:
                case LongFactory.Code:
                    return 1;

                case ULongFactory.Code:
                    return 0;

                case IntegerProxyFactory.Code:
                case DecimalProxyFactory.Code:
                case FloatFactory.Code:
                case DoubleProxyFactory.Code:
                    return -1;

                default:
                    return -2;
            }
        }
    }
}
