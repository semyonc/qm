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
using System.Xml;

namespace DataEngine.XQuery
{
    internal class QNameParser
    {
        public static XmlQualifiedName Parse(string name, IXmlNamespaceResolver resolver, XmlNameTable nameTable)
        {
            return Parse(name, resolver, String.Empty, nameTable);
        }

        public static XmlQualifiedName Parse(string name, IXmlNamespaceResolver resolver, string defaultNamespace, XmlNameTable nameTable)
        {
            string prefix;
            string localName;
            Split(name, out prefix, out localName);
            if (nameTable != null)
            {
                if (prefix != null)
                    prefix = nameTable.Add(prefix);
                if (localName != null)
                    localName = nameTable.Add(localName);
            }
            if (!String.IsNullOrEmpty(prefix))
            {
                string ns = resolver.LookupNamespace(prefix);
                if (ns == null)
                    throw new XQueryException(Properties.Resources.XPST0081, prefix);
                return new XmlQualifiedName(localName, ns);
            }
            else
            {
                if (defaultNamespace == null)
                    defaultNamespace = String.Empty;
                return new XmlQualifiedName(localName, defaultNamespace);
            }
        }

        public static int ParseNCName(string s, int offset)
        {
            int num = offset;
            XmlCharType instance = XmlCharType.Instance;
            if (offset < s.Length && (instance.charProperties[s[offset]] & 4) != 0)
            {
                offset++;
                while (offset < s.Length)
                {
                    if ((instance.charProperties[s[offset]] & 8) == 0)
                        break;
                    offset++;
                }
            }
            return offset - num;
        }

        public static int ParseQName(string s, int offset, out int colonOffset)
        {
            colonOffset = 0;
            int num = ParseNCName(s, offset);
            if (num != 0)
            {
                offset += num;
                if (offset < s.Length && s[offset] == ':')
                {
                    int num2 = ParseNCName(s, offset + 1);
                    if (num2 != 0)
                    {
                        colonOffset = offset;
                        num += num2 + 1;
                    }
                }
            }
            return num;
        }

        public static void Split(string value, out string prefix, out string localName)
        {
            prefix = String.Empty;
            int num;
            int num2 = ParseQName(value, 0, out num);
            if (num2 == 0 || num2 != value.Length)
                localName = null;
            else
            {
                if (num != 0)
                {
                    prefix = value.Substring(0, num);
                    localName = value.Substring(num + 1);
                }
                else
                    localName = value;
            }
        }
    }
}
