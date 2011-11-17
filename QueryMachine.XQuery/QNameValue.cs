//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

using DataEngine.XQuery.Util;

namespace DataEngine.XQuery
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
