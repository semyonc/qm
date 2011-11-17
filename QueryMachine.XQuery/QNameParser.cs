//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

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
