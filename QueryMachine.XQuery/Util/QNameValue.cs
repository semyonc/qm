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
using System.Xml;
using System.Xml.Schema;

namespace DataEngine.XQuery.Util
{
    public class QNameValue: IXmlConvertable
    {
        public QNameValue()
        {
            Prefix = LocalName = NamespaceUri = String.Empty;
        }

        public QNameValue(string prefix, string localName, string ns, XmlNameTable nameTable)
        {
            if (prefix == null)
                throw new NullReferenceException("prefix");
            if (localName == null)
                throw new NullReferenceException("localName");                
            if (ns == null)
                throw new NullReferenceException("ns");
            if (nameTable == null)
                throw new NullReferenceException("nameTable");
            if (prefix != "" && ns == "")
                throw new XQueryException(Properties.Resources.FOCA0002,
                    String.Format("{0}:{1}", prefix, localName));
            try
            {
                localName = XmlConvert.VerifyNCName(localName);
            }
            catch(XmlException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, localName, "xs:QName");
            }
            Prefix = nameTable.Add(prefix);
            LocalName = nameTable.Add(localName);
            NamespaceUri = nameTable.Add(ns);
        }

        public QNameValue(XmlQualifiedName qname)
        {
            Prefix = String.Empty;
            LocalName = qname.Name;
            NamespaceUri = qname.Namespace;
        }

        public String Prefix { get; private set; }
        public String LocalName { get; private set; }
        public String NamespaceUri { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return LocalName != "";
            }
        }

        public XmlQualifiedName ToQualifiedName()
        {
            return new XmlQualifiedName(LocalName, NamespaceUri); 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Prefix != "")
            {
                sb.Append(Prefix);
                sb.Append(':');
            }
            sb.Append(LocalName);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            QNameValue other = obj as QNameValue;
            if (other != null)
            {
                if (LocalName == other.LocalName &&
                    NamespaceUri == other.NamespaceUri)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return LocalName.GetHashCode() ^ NamespaceUri.GetHashCode() << 8; 
        }

        #region IXmlConvertable Members

        object IXmlConvertable.ValueAs(XQuerySequenceType type, XmlNamespaceManager nsmgr)
        {
            switch (type.TypeCode)
            {
                case XmlTypeCode.AnyAtomicType:
                case XmlTypeCode.QName:
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

        public static QNameValue Parse(string qname, string ns, XmlNameTable nameTable)
        {
            string prefix;
            string localName;
            QNameParser.Split(qname.Trim(), out prefix, out localName);
            if (localName == null)
                throw new XQueryException(Properties.Resources.FORG0001, qname, "xs:QName");
            return new QNameValue(prefix, localName, ns, nameTable);
        }

        public static QNameValue Parse(string qname, XmlNamespaceManager resolver)
        {
            return Parse(qname, resolver, resolver.DefaultNamespace);
        }

        public static QNameValue Parse(string qname, XmlNamespaceManager resolver, string defaultNs)
        {
            string prefix;
            string localName;
            QNameParser.Split(qname.Trim(), out prefix, out localName);
            if (localName == null)
                throw new XQueryException(Properties.Resources.FORG0001, qname, "xs:QName");
            if (defaultNs == null)
                defaultNs = String.Empty;
            if (!String.IsNullOrEmpty(prefix))
            {
                string ns = resolver.LookupNamespace(prefix);
                if (ns == null)
                    throw new XQueryException(Properties.Resources.XPST0081, prefix);
                return new QNameValue(prefix, localName, ns, resolver.NameTable);
            }
            else
                return new QNameValue("", localName, defaultNs, resolver.NameTable);
        }        
    }
}
