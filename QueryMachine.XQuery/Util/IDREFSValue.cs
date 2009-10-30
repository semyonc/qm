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

namespace DataEngine.XQuery.Util
{
    public class IDREFSValue
    {
        public IDREFSValue(string[] value)
        {
            if (value == null)
                throw new ArgumentException("value");
            ValueList = value;
        }

        public string[] ValueList { get; private set; }

        public override bool Equals(object obj)
        {
            IDREFSValue other = obj as IDREFSValue;
            if (other != null && other.ValueList.Length == ValueList.Length)
            {
                for (int k = 0; k < ValueList.Length; k++)
                    if (ValueList[k] != other.ValueList[k])
                        return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int k = 0; k < ValueList.Length; k++)
                hashCode = hashCode << 7 ^ ValueList[k].GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < ValueList.Length; k++)
            {
                if (k > 0)
                    sb.Append(" ");
                sb.Append(ValueList[k]);
            }
            return sb.ToString();
        }
    }
}
