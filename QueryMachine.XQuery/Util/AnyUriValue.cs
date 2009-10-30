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
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace DataEngine.XQuery.Util
{
    public class AnyUriValue: IXmlConvertable, IComparable
    {
        public AnyUriValue(string value)
        {
            if (value == null)
                throw new InvalidOperationException();
            Value = value;
        }

        public AnyUriValue(Uri uri)
        {
            Value = uri.OriginalString;
        }

        public string Value { get; private set; }

        public override bool Equals(object obj)
        {
            AnyUriValue other = obj as AnyUriValue;
            if (other != null)
                return Value == other.Value;
            if (obj is String)
                return Value == obj.ToString();
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        #region IXmlConvertable Members

        public object ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.AnyUri:
                    return this;
                case XmlTypeCode.String:
                    return ToString();
                case XmlTypeCode.UntypedAtomic:
                    return new UntypedAtomic(ToString());
                default:
                    throw new InvalidCastException();
            }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            AnyUriValue other = obj as AnyUriValue;
            if (other == null)
                throw new ArgumentException();
            return String.CompareOrdinal(Value, other.Value);
        }

        #endregion
    }
}
