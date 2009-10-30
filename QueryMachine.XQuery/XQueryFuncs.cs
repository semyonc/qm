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
using System.Xml.XPath;
using System.Xml.Schema;

using DataEngine.CoreServices;
using System.Text.RegularExpressions;
using System.IO;

using DataEngine.XQuery.Util;

namespace DataEngine.XQuery
{
    public class XQueryFuncs
    {
        [XQuerySignature("name", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static string GetName(IContextProvider provider)
        {
            return GetName(Core.NodeValue(Core.ContextNode(provider)));
        }

        [XQuerySignature("name", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static string GetName([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return String.Empty;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:name()");
            return nav.Name;
        }

        [XQuerySignature("node-name", Return = XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetNodeName([Implict] Executive executive, [XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node != Undefined.Value)
            {
                XPathNavigator nav = node as XPathNavigator;
                if (nav == null)
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:node-name()");
                if (nav.NodeType == XPathNodeType.Element || nav.NodeType == XPathNodeType.Attribute)
                    return new QNameValue(nav.Prefix, nav.LocalName, nav.NamespaceURI, nav.NameTable);
                else if (nav.NodeType == XPathNodeType.ProcessingInstruction || nav.NodeType == XPathNodeType.Namespace)
                    return new QNameValue("", nav.Name, "", nav.NameTable);
            }
            return Undefined.Value;
        }

        [XQuerySignature("local-name", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static string GetLocalName(IContextProvider provider)
        {
            return GetLocalName(Core.NodeValue(Core.ContextNode(provider)));
        }

        [XQuerySignature("local-name", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static string GetLocalName([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return String.Empty;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:local-name()");
            return nav.LocalName;
        }

        [XQuerySignature("namespace-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetNamespaceUri(IContextProvider provider)
        {
            return GetNamespaceUri(Core.NodeValue(Core.ContextNode(provider)));
        }

        [XQuerySignature("namespace-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetNamespaceUri([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return String.Empty;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:local-name()");
            return new AnyUriValue(nav.NamespaceURI);
        }

        [XQuerySignature("nilled", Return = XmlTypeCode.Boolean, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetNilled([XQueryParameter(XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return node;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:local-name()");
            if (nav.NodeType != XPathNodeType.Element)
                return Undefined.Value;
            if (nav.SchemaInfo != null)
                return nav.SchemaInfo.IsNil;
            return false;
        }

        [XQuerySignature("base-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetBaseUri([Implict] Executive engine, [XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return node;
            XQueryContext context = (XQueryContext)engine.Owner;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:base-uri()");
            
            if (!(nav.NodeType == XPathNodeType.Element ||
                  nav.NodeType == XPathNodeType.Attribute ||
                  nav.NodeType == XPathNodeType.Root ||
                  nav.NodeType == XPathNodeType.Namespace))
                return Undefined.Value;

            nav = nav.Clone();
            List<string> uri = new List<string>();
            do
            {
                string baseUri = nav.BaseURI;
                if (baseUri != "")
                    uri.Add(baseUri);
            }
            while (nav.MoveToParent());
            Uri res = null;
            if (context.BaseUri != null)
                res = new Uri(context.BaseUri);
            for (int k = uri.Count - 1; k >= 0; k--)
            {
                if (res != null)
                    res = new Uri(res, uri[k]);
                else
                    res = new Uri(uri[k]);
            }
            if (res == null)
                return Undefined.Value;
            else
                return new AnyUriValue(res);
        }

        [XQuerySignature("base-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetBaseUri([Implict] Executive engine, IContextProvider provider)
        {
            return GetBaseUri(engine, Core.NodeValue(Core.ContextNode(provider)));
        }

        [XQuerySignature("document-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object DocumentUri([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return node;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:document-uri()");
            if (nav.NodeType != XPathNodeType.Root || nav.BaseURI == "")
                return Undefined.Value;
            return new AnyUriValue(nav.BaseURI);
        }

        [XQuerySignature("doc-available")]
        public static bool IsDocAvailable([Implict] Executive engine, [XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object name)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (name == Undefined.Value)
                return false;
            return context.GetFileName((String)name) != null;
        }

        [XQuerySignature("trace")]
        public static XQueryNodeIterator WriteTrace([Implict] Executive engine, XQueryNodeIterator iter, string label)
        {
            StringBuilder sb = new StringBuilder();
            if (label != "")
                sb.AppendFormat("{0}: ", label);
            bool first = true;
            foreach (XPathItem item in iter)
            {
                if (!first)
                    sb.Append(", ");
                else
                    first = false;
                if (item.IsNode)
                    sb.Append(((XPathNavigator)item).OuterXml);
                else
                    sb.Append(item.Value);
            }
            Console.Out.WriteLine(sb.ToString());
            return iter;
        }

        [XQuerySignature("error", Return = XmlTypeCode.None)]
        public static XQueryNodeIterator WriteError([Implict] Executive engine)
        {
            throw new XQueryException(Properties.Resources.FOER0000);
        }

        [XQuerySignature("error", Return = XmlTypeCode.None)]
        public static XQueryNodeIterator WriteError([Implict] Executive engine, [XQueryParameter(XmlTypeCode.QName)] QNameValue qname)
        {
            throw new XQueryException(qname.ToString());
        }

        [XQuerySignature("error", Return = XmlTypeCode.None)]
        public static XQueryNodeIterator WriteError([Implict] Executive engine, [XQueryParameter(XmlTypeCode.QName)] QNameValue qname, 
            string description)
        {
            throw new XQueryException("{0}: {1}", qname, description);
        }

        [XQuerySignature("error", Return=XmlTypeCode.None)]
        public static XQueryNodeIterator WriteError([Implict] Executive engine, [XQueryParameter(XmlTypeCode.QName)] QNameValue qname, 
            string description, XQueryNodeIterator errobj)
        {
            throw new XQueryException("{0}: {1}", qname, description);
        }

        private static IEnumerable<XPathItem> AtomizeIterator(XQueryContext context, XQueryNodeIterator iter)
        {
            foreach (XPathItem item in iter)
                yield return context.CreateItem(Core.Atomize(item));
        }

        [XQuerySignature("data", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetData([Implict] Executive engine, XQueryNodeIterator iter)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            return new NodeIterator(AtomizeIterator(context, iter));
        }

        [XQuerySignature("concat", VariableParams = true)]
        public static string Concat([XQueryParameter(XmlTypeCode.AnyAtomicType, 
            Cardinality=XmlTypeCardinality.ZeroOrOne)] params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            if (args.Length < 2)
                throw new XQueryException(Properties.Resources.XPST0017, "concat", 
                    args.Length, XmlReservedNs.NsXQueryFunc);
            foreach (object arg in args)
                if (arg != Undefined.Value)
                    sb.Append(XQueryConvert.ToString(arg));
            return sb.ToString();
        }

        [XQuerySignature("string-join")]
        public static string StringJoin(XQueryNodeIterator iter, string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (XPathItem item in iter)
            {
                if (sb.Length > 0 && s != "")
                    sb.Append(s);
                sb.Append(item.Value);
            }
            return sb.ToString();
        }
         
        [XQuerySignature("substring")]
        public static string Substring([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item, double startingLoc)
        {
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            int pos = Convert.ToInt32(Math.Round(startingLoc)) - 1;
            if (pos <= 0)
                pos = 0;
            if (pos < value.Length)
                return value.Substring(pos);
            else
                return String.Empty;
        }

        [XQuerySignature("substring")]
        public static string Substring([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item, double startingLoc, double length)
        {
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            if (Double.IsInfinity(startingLoc) || Double.IsNaN(startingLoc) ||
                Double.IsNegativeInfinity(length) || Double.IsNaN(length))
                return String.Empty;
            int pos = Convert.ToInt32(Math.Round(startingLoc)) -1;
            int len;
            if (Double.IsPositiveInfinity(length))
                len = Int32.MaxValue;
            else
                len = Convert.ToInt32(Math.Round(length));
            if (pos < 0)
            {
                len = len + pos;
                pos = 0;
            }
            if (pos < value.Length)
            {
                if (pos + len > value.Length)
                    len = value.Length - pos;
                if (len > 0)
                    return value.Substring(pos, len);
            }
            return String.Empty;
        }

        [XQuerySignature("string-length")]
        public static int StringLength([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object source)
        {
            if (source == Undefined.Value)
                return 0;
            return ((string)source).Length;
        }

        [XQuerySignature("string-length")]
        public static int StringLength([Implict] Executive engine, IContextProvider provider)
        {
            return StringLength(Core.StringValue(engine, Core.Atomize(Core.ContextNode(provider))));
        }

        [XQuerySignature("normalize-space")]
        public static string NormalizeSpace([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item)
        {
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            // Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
            // Original source is XsltFunctions.cs (System.Xml.Xsl.Runtime)
            XmlCharType xmlCharType = XmlCharType.Instance;
            StringBuilder sb = null;
            int idx, idxStart = 0, idxSpace = 0;

            for (idx = 0; idx < value.Length; idx++) {
                if (xmlCharType.IsWhiteSpace(value[idx])) {
                    if (idx == idxStart) {
                        // Previous character was a whitespace character, so discard this character
                        idxStart++;
                    }
                    else if (value[idx] != ' ' || idxSpace == idx) {
                        // Space was previous character or this is a non-space character
                        if (sb == null)
                            sb = new StringBuilder(value.Length);
                        else
                            sb.Append(' ');

                        // Copy non-space characters into string builder
                        if (idxSpace == idx)
                            sb.Append(value, idxStart, idx - idxStart - 1);
                        else
                            sb.Append(value, idxStart, idx - idxStart);

                        idxStart = idx + 1;
                    }
                    else {
                        // Single whitespace character doesn't cause normalization, but mark its position
                        idxSpace = idx + 1;
                    }
                }
            }

            if (sb == null) {
                // Check for string that is entirely composed of whitespace
                if (idxStart == idx) return string.Empty;

                // If string does not end with a space, then it must already be normalized
                if (idxStart == 0 && idxSpace != idx) return value;

                sb = new StringBuilder(value.Length);
            }
            else if (idx != idxStart) {
                sb.Append(' ');
            }

            // Copy non-space characters into string builder
            if (idxSpace == idx)
                sb.Append(value, idxStart, idx - idxStart - 1);
            else
                sb.Append(value, idxStart, idx - idxStart);

            return sb.ToString();
        }

        [XQuerySignature("normalize-space")]
        public static string NormalizeSpace([Implict] Executive engine, IContextProvider provider)
        {
            return NormalizeSpace(Core.StringValue(engine, Core.Atomize(Core.ContextNode(provider))));
        }

        [XQuerySignature("normalize-unicode")]
        public static string NormalizeUnicode([XQueryParameter(XmlTypeCode.String, 
            Cardinality=XmlTypeCardinality.ZeroOrOne)] object arg, string form)
        {
            if (arg == Undefined.Value)
                return String.Empty;
            string value = (String)arg;
            form = form.Trim();
            if (String.Equals(form, "NFC", StringComparison.OrdinalIgnoreCase))
                return value.Normalize(NormalizationForm.FormC);
            if (String.Equals(form, "NFD", StringComparison.OrdinalIgnoreCase))
                return value.Normalize(NormalizationForm.FormD);
            if (String.Equals(form, "NFKC", StringComparison.OrdinalIgnoreCase))
                return value.Normalize(NormalizationForm.FormKC);
            if (String.Equals(form, "NFKD", StringComparison.OrdinalIgnoreCase))
                return value.Normalize(NormalizationForm.FormKD);
            if (form.Length != 0)
                throw new XQueryException(Properties.Resources.UnsupportedNormalizationForm, form);
            return value;
        }

        [XQuerySignature("normalize-unicode")]
        public static string NormalizeUnicode([XQueryParameter(XmlTypeCode.String, 
            Cardinality=XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return String.Empty;
            string value = (string)arg;
            return value.Normalize(NormalizationForm.FormC);
        }

        [XQuerySignature("upper-case")]
        public static string UpperCase([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return String.Empty;
            return ((string)value).ToUpper();
        }

        [XQuerySignature("lower-case")]
        public static string LowerCase([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return String.Empty;
            return ((string)value).ToLower();
        }

        private static Dictionary<int, int> CreateMapping(string mapString, string translateString)
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            int index = 0;
            int num2 = 0;
            while (index < mapString.Length && num2 < translateString.Length)
            {
                int num3;
                if (Char.IsSurrogate(mapString[index]))
                {
                    num3 = Char.ConvertToUtf32(mapString, index);
                    index++;
                }
                else
                    num3 = mapString[index];
                if (!dictionary.ContainsKey(num3))
                {
                    int num4;
                    if (Char.IsSurrogate(translateString[num2]))
                    {
                        num4 = char.ConvertToUtf32(translateString, num2);
                        num2++;
                    }
                        num4 = translateString[num2];
                    dictionary[num3] = num4;
                }
                num2++;
                index++;
            }
            while (index < mapString.Length)
            {
                int num5;
                if (Char.IsSurrogate(mapString[index]))
                {
                    num5 = char.ConvertToUtf32(mapString, index);
                    index++;
                }
                else
                    num5 = mapString[index];
                if (!dictionary.ContainsKey(num5))
                    dictionary[num5] = 0;
                index++;
            }
            return dictionary;
        }

        [XQuerySignature("translate")]
        public static string Translate([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item, string mapString, string transString)
        {
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            StringBuilder builder = new StringBuilder(value.Length);
            Dictionary<int, int> mapping = CreateMapping(mapString, transString);
            for (int i = 0; i < value.Length; i++)
            {
                int num3;
                int key = Char.ConvertToUtf32(value, i);
                if (Char.IsSurrogate(value, i))
                    i++;
                if (!mapping.TryGetValue(key, out num3))
                    num3 = key;
                if (num3 != 0)
                {
                    if (num3 < 0x10000)
                        builder.Append((char)num3);
                    else
                    {
                        num3 -= 0x10000;
                        builder.Append((char)((num3 >> 10) + 0xd800));
                        builder.Append((char)((num3 % 0x400) + 0xdc00));
                    }
                }
            }
            return builder.ToString();
        }

        [XQuerySignature("encode-for-uri")]
        public static string EncodeForUri([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return String.Empty;
            char[] chArray = new char[3];
            chArray[0] = '%';
            StringBuilder sb = new StringBuilder();
            foreach (byte c in Encoding.UTF8.GetBytes((string)value))
            {                
                if (Char.IsDigit((char)c) || ('a' <= c && c <= 'z') ||
                     ('A' <= c && c <= 'Z') || c == '-' || c == '_' || c == '.' || c == '~')
                    sb.Append((char)c);
                else
                {
                    int num = c;
                    int num3 = num / 0x10;
                    int num4 = num % 0x10;
                    chArray[1] = (num3 >= 10) ? ((char)(0x41 + (num3 - 10))) : ((char)(0x30 + num3));
                    chArray[2] = (num4 >= 10) ? ((char)(0x41 + (num4 - 10))) : ((char)(0x30 + num4));
                    sb.Append(chArray);
                }
            }
            return sb.ToString();
        }

        [XQuerySignature("iri-to-uri")]
        public static string IriToUri([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item)
        {
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            char[] chArray = new char[3];
            chArray[0] = '%';
            StringBuilder sb = new StringBuilder();
            foreach (byte num in Encoding.UTF8.GetBytes(value))
            {
                if (num == 0x20)
                    sb.Append("%20");
                else if ((((num < 0x7f && num >= 0x20) && (num != 60 && num != 0x3e)) && 
                    ((num != 0x22 && num != 0x7b) && (num != 0x7d && num != 0x7c))) && (((num != 0x5c) && (num != 0x5e)) && (num != 0x60)))
                    sb.Append((char)num);
                else
                {
                    int num2 = num;
                    int num3 = num2 / 0x10;
                    int num4 = num2 % 0x10;
                    chArray[1] = (num3 >= 10) ? ((char)(0x41 + (num3 - 10))) : ((char)(0x30 + num3));
                    chArray[2] = (num4 >= 10) ? ((char)(0x41 + (num4 - 10))) : ((char)(0x30 + num4));
                    sb.Append(chArray);
                }
            }
            return sb.ToString();
        }

        [XQuerySignature("escape-html-uri")]
        public static string EscapeHtmlUri([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item)
        {            
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            StringBuilder builder = new StringBuilder(value.Length);
            foreach (byte num in Encoding.UTF8.GetBytes(value))
            {
                if (num >= 0x20 && num < 0x7f)
                    builder.Append((char)num);
                else
                {
                    int num2 = num / 0x10;
                    int num3 = num % 0x10;
                    char ch = (num2 >= 10) ? ((char)(0x41 + (num2 - 10))) : ((char)(0x30 + num2));
                    char ch2 = (num3 >= 10) ? ((char)(0x41 + (num3 - 10))) : ((char)(0x30 + num3));
                    builder.Append('%');
                    builder.Append(ch);
                    builder.Append(ch2);
                }
            }
            return builder.ToString();
        }

        [XQuerySignature("contains")]
        public static bool Contains(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string str;
            if (arg1 == Undefined.Value)
                str = String.Empty;
            else
                str = (string)arg1;
            string substr;
            if (arg2 == Undefined.Value)
                substr = String.Empty;
            else
                substr = (string)arg2;
            return str.Contains(substr);
        }

        [XQuerySignature("contains")]
        public static bool Contains([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            CultureInfo culture = context.GetCulture(collation);
            return Contains(arg1, arg2);
        }

        [XQuerySignature("starts-with")]
        public static bool StartsWith(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string str;
            if (arg1 == Undefined.Value)
                str = String.Empty;
            else
                str = (string)arg1;
            string substr;
            if (arg2 == Undefined.Value)
                substr = String.Empty;
            else
                substr = (string)arg2;
            return str.StartsWith(substr);
        }

        [XQuerySignature("starts-with")]
        public static bool StartsWith([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            CultureInfo culture = context.GetCulture(collation);
            return StartsWith(arg1, arg2);
        }

        [XQuerySignature("ends-with")]
        public static bool EndsWith(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string str;
            if (arg1 == Undefined.Value)
                str = String.Empty;
            else
                str = (string)arg1;
            string substr;
            if (arg2 == Undefined.Value)
                substr = String.Empty;
            else
                substr = (string)arg2;
            return str.EndsWith(substr);
        }

        [XQuerySignature("ends-with")]
        public static bool EndsWith([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            CultureInfo culture = context.GetCulture(collation);
            return EndsWith(arg1, arg2);
        }

        [XQuerySignature("substring-before")]
        public static string SubstringBefore(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string str;
            if (arg1 == Undefined.Value)
                str = String.Empty;
            else
                str = (string)arg1;
            string substr;
            if (arg2 == Undefined.Value)
                substr = String.Empty;
            else
                substr = (string)arg2;
            int index = str.IndexOf(substr);
            if (index >= 0)
                return str.Substring(0, index);
            return String.Empty;
        }

        [XQuerySignature("substring-before")]
        public static string SubstringBefore([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            CultureInfo culture = context.GetCulture(collation);
            return SubstringBefore(arg1, arg2);
        }

        [XQuerySignature("substring-after")]
        public static string SubstringAfter(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string str;
            if (arg1 == Undefined.Value)
                str = String.Empty;
            else
                str = (string)arg1;
            string substr;
            if (arg2 == Undefined.Value)
                substr = String.Empty;
            else
                substr = (string)arg2;
            int index = str.IndexOf(substr);
            if (index >= 0)
                return str.Substring(index + substr.Length);
            return String.Empty;
        }

        [XQuerySignature("substring-after")]
        public static string SubstringAfter([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            CultureInfo culture = context.GetCulture(collation);
            return SubstringAfter(arg1, arg2);
        }

        private static bool ParseFlags(string flagString, out RegexOptions flags)
        {
            flags = RegexOptions.None;
            foreach (char ch in flagString)
            {
                switch (ch)
                {
                    case 's':
                        flags |= RegexOptions.Singleline;
                        break;

                    case 'x':
                        flags |= RegexOptions.IgnorePatternWhitespace;
                        break;

                    case 'i':
                        flags |= RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
                        break;

                    case 'm':
                        flags |= RegexOptions.Multiline;
                        break;

                    default:
                        return false;
                }
            }
            return true;
        }

        [XQuerySignature("matches")]
        public static bool Matches(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string flagString)
        {
            string input;
            if (arg1 == Undefined.Value)
                input = String.Empty;
            else
                input = (string)arg1;
            string pattern;
            if (arg2 == Undefined.Value)
                pattern = String.Empty;
            else
                pattern = (string)arg2;
            RegexOptions flags;
            if (!ParseFlags(flagString, out flags))
                throw new XQueryException(Properties.Resources.InvalidRegularExpressionFlags, flagString);
            try
            {
                return Regex.IsMatch(input, pattern, flags);
            }
            catch (ArgumentException)
            {
                throw new XQueryException(Properties.Resources.InvalidRegularExpr, pattern);
            }
        }

        [XQuerySignature("matches")]
        public static bool Matches(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string input;
            if (arg1 == Undefined.Value)
                input = String.Empty;
            else
                input = (string)arg1;
            string pattern;
            if (arg2 == Undefined.Value)
                pattern = String.Empty;
            else
                pattern = (string)arg2;
            try
            {
                return Regex.IsMatch(input, pattern);
            }
            catch (ArgumentException)
            {
                throw new XQueryException(Properties.Resources.InvalidRegularExpr, pattern);
            }
        }

        [XQuerySignature("replace")]
        public static string Replace(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string replacement,
            string flagString)
        {
            string input;
            if (arg1 == Undefined.Value)
                input = String.Empty;
            else
                input = (string)arg1;
            string pattern;
            if (arg2 == Undefined.Value)
                pattern = String.Empty;
            else
                pattern = (string)arg2;
            RegexOptions flags;
            if (!ParseFlags(flagString, out flags))
                throw new XQueryException(Properties.Resources.InvalidRegularExpressionFlags, flagString);
            return Regex.Replace(input, pattern, replacement, flags);
        }

        [XQuerySignature("replace")]
        public static string Replace(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string replacement)
        {
            string input;
            if (arg1 == Undefined.Value)
                input = String.Empty;
            else
                input = (string)arg1;
            string pattern;
            if (arg2 == Undefined.Value)
                pattern = String.Empty;
            else
                pattern = (string)arg2;
            return Regex.Replace(input, pattern, replacement);
        }

        private static IEnumerable<XPathItem> StringEnumerator(string[] s)
        {
            foreach (string str in s)
                yield return new XQueryAtomicValue(str, null);
        }

        [XQuerySignature("tokenize", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator Tokenize(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string flagString)
        {
            string input;
            if (arg1 == Undefined.Value)
                input = String.Empty;
            else
                input = (string)arg1;
            string pattern;
            if (arg2 == Undefined.Value)
                pattern = String.Empty;
            else
                pattern = (string)arg2;
            RegexOptions flags;
            if (!ParseFlags(flagString, out flags))
                throw new XQueryException(Properties.Resources.InvalidRegularExpressionFlags, flagString);
            string[] res = Regex.Split(input, pattern, flags);
            return new NodeIterator(StringEnumerator(res));
        }

        [XQuerySignature("tokenize", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator Tokenize(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2)
        {
            string input;
            if (arg1 == Undefined.Value)
                input = String.Empty;
            else
                input = (string)arg1;
            string pattern;
            if (arg2 == Undefined.Value)
                pattern = String.Empty;
            else
                pattern = (string)arg2;
            string[] res = Regex.Split(input, pattern);
            return new NodeIterator(StringEnumerator(res));
        }

        [XQuerySignature("years-from-duration", Return=XmlTypeCode.Integer, Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static object YearsFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DurationValue duration = (DurationValue)arg;
            return (Integer)duration.Years;
        }

        [XQuerySignature("months-from-duration", Return = XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MonthsFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DurationValue duration = (DurationValue)arg;
            return (Integer)duration.Months;
        }

        [XQuerySignature("days-from-duration", Return = XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object DaysFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DurationValue duration = (DurationValue)arg;
            return (Integer)duration.Days;
        }

        [XQuerySignature("hours-from-duration", Return = XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object HoursFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DurationValue duration = (DurationValue)arg;
            return (Integer)duration.Hours;
        }

        [XQuerySignature("minutes-from-duration", Return = XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MinutesFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DurationValue duration = (DurationValue)arg;
            return (Integer)duration.Minutes;
        }

        [XQuerySignature("seconds-from-duration", Return = XmlTypeCode.Decimal, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object SecondsFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DurationValue duration = (DurationValue)arg;
            return (decimal)duration.Seconds + (decimal)duration.Milliseconds / 1000;
        }

        [XQuerySignature("year-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int YearFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            if (dateTime.S)
                return -dateTime.Value.Year;
            else
                return dateTime.Value.Year;
        }

        [XQuerySignature("month-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MonthFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            return dateTime.Value.Month;
        }

        [XQuerySignature("day-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int DayFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            return dateTime.Value.Day;
        }

        [XQuerySignature("hours-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int HoursFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            return dateTime.Value.Hour;
        }

        [XQuerySignature("minutes-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MinutesFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            return dateTime.Value.Minute;
        }

        [XQuerySignature("seconds-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static decimal SecondsFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            return (decimal)dateTime.Value.Second + (decimal)dateTime.Value.Millisecond / 1000;
        }

        [XQuerySignature("timezone-from-dateTime", Return=XmlTypeCode.DayTimeDuration, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object TimezoneFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DateTimeValue dateTime = (DateTimeValue)arg;
            if (dateTime.IsLocal)
                return Undefined.Value;
            return new DayTimeDurationValue(dateTime.Value.Offset);
        }

        [XQuerySignature("year-from-date", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int YearFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateValue date)
        {
            if (date.S)
                return -date.Value.Year;
            else
                return date.Value.Year;
        }

        [XQuerySignature("month-from-date", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MonthFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateValue date)
        {
            return date.Value.Month;
        }

        [XQuerySignature("day-from-date", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int DayFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateValue date)
        {
            return date.Value.Day;
        }

        [XQuerySignature("timezone-from-date", Return = XmlTypeCode.DayTimeDuration, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object TimezoneFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            DateValue date = (DateValue)arg;
            if (date.IsLocal)
                return Undefined.Value;
            return new DayTimeDurationValue(date.Value.Offset);
        }

        [XQuerySignature("hours-from-time", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int HoursFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] TimeValue time)
        {
            return time.Value.Hour;
        }

        [XQuerySignature("minutes-from-time", Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static int MinutesFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] TimeValue time)
        {
            return time.Value.Minute;
        }

        [XQuerySignature("seconds-from-time", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static decimal SecondsFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] TimeValue time)
        {
            return (decimal)time.Value.Second + (decimal)time.Value.Millisecond / 1000;
        }

        [XQuerySignature("timezone-from-time", Return = XmlTypeCode.DayTimeDuration, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object TimezoneFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg)
        {
            if (arg == Undefined.Value)
                return Undefined.Value;
            TimeValue time = (TimeValue)arg;
            if (time.IsLocal)
                return Undefined.Value;
            return new DayTimeDurationValue(time.Value.Offset);
        }

        [XQuerySignature("adjust-dateTime-to-timezone", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static DateTimeValue AdjustDateTimeToTimezone(
            [XQueryParameter(XmlTypeCode.DateTime, Cardinality=XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime)
        {
            return new DateTimeValue(dateTime.S, TimeZoneInfo.ConvertTime(dateTime.Value, TimeZoneInfo.Local));
        }

        [XQuerySignature("adjust-dateTime-to-timezone", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static DateTimeValue AdjustDateTimeToTimezone(
            [XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTimeValue dateTime,
            [XQueryParameter(XmlTypeCode.DayTimeDuration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object tz)
        {
            if (tz == Undefined.Value)
                return new DateTimeValue(dateTime.S, dateTime.Value.DateTime);
            DayTimeDurationValue _tz = (DayTimeDurationValue)tz;
            try
            {
                if (dateTime.IsLocal)
                    return new DateTimeValue(dateTime.S, new DateTimeOffset(dateTime.Value.DateTime, _tz.LowPartValue));
                else
                    return new DateTimeValue(dateTime.S, dateTime.Value.ToOffset(_tz.LowPartValue));
            }
            catch (ArgumentException)
            {
                throw new XQueryException(Properties.Resources.FODT0003, _tz.ToString());
            }
        }

        [XQuerySignature("adjust-date-to-timezone", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static DateValue AdjustDateToTimezone(
            [XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateValue date)
        {
            return new DateValue(date.S, TimeZoneInfo.ConvertTime(date.Value, TimeZoneInfo.Local));
        }

        [XQuerySignature("adjust-date-to-timezone", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static DateValue AdjustDateToTimezone(
            [XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateValue date, 
            [XQueryParameter(XmlTypeCode.DayTimeDuration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object tz)
        {
            if (tz == Undefined.Value)
                return new DateValue(date.S, date.Value.Date);
            DayTimeDurationValue _tz = (DayTimeDurationValue)tz;
            try
            {
                if (date.IsLocal)
                    return new DateValue(date.S, new DateTimeOffset(date.Value.Date, _tz.LowPartValue));
                else
                {
                    DateTimeOffset offs = date.Value.ToOffset(_tz.LowPartValue);
                    return new DateValue(date.S, new DateTimeOffset(offs.Date, offs.Offset));
                }
            }
            catch (ArgumentException)
            {
                throw new XQueryException(Properties.Resources.FODT0003, _tz.ToString());
            }
        }

        [XQuerySignature("adjust-time-to-timezone", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static TimeValue AdjustTimeToTimezone(
            [XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] TimeValue time)
        {
            return new TimeValue(TimeZoneInfo.ConvertTime(time.Value, TimeZoneInfo.Local));
        }

        [XQuerySignature("adjust-time-to-timezone", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static TimeValue AdjustTimeToTimezone(
            [XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] TimeValue time, 
            [XQueryParameter(XmlTypeCode.DayTimeDuration, Cardinality = XmlTypeCardinality.ZeroOrOne)] object tz)
        {
            if (tz == Undefined.Value)
                return new TimeValue(time.Value.DateTime);
            DayTimeDurationValue _tz = (DayTimeDurationValue)tz;
            try
            {
                if (time.IsLocal)
                    return new TimeValue(new DateTimeOffset(time.Value.DateTime, _tz.LowPartValue));
                else
                    return new TimeValue(time.Value.ToOffset(_tz.LowPartValue)); 
            }
            catch (ArgumentException)
            {
                throw new XQueryException(Properties.Resources.FODT0003, _tz.ToString());
            }
        }

        [XQuerySignature("abs", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetAbs([XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return value;
            if (Integer.IsDerivedSubtype(value))
                value = Integer.ToInteger(value);
            if (value is Double)
                return Math.Abs((double)value);
            else if (value is Decimal)
                return Math.Abs((decimal)value);
            else if (value is Integer)
                return (Integer)Math.Abs((decimal)(Integer)value);
            else if (value is Single)
                return Math.Abs((float)value);
            else
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                    "xs:float | xs:double | xs:decimal | xs:integer in fn:abs()");
        }

        [XQuerySignature("ceiling", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetCeiling([XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return value;
            if (Integer.IsDerivedSubtype(value))
                value = Integer.ToInteger(value);
            if (value is Double)
                return Math.Ceiling((double)value);
            else if (value is Decimal)
                return Math.Ceiling((decimal)value);
            else if (value is Integer)
                return (Integer)Math.Ceiling((decimal)(Integer)value);
            else if (value is Single)
                return (float)Math.Ceiling((float)value);
            else
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                    "xs:float | xs:double | xs:decimal | xs:integer in fn:ceiling()");            
        }

        [XQuerySignature("floor", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetFloor([XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return value;
            if (Integer.IsDerivedSubtype(value))
                value = Integer.ToInteger(value);
            if (value is Double)
                return Math.Floor((double)value);
            else if (value is Decimal)
                return Math.Floor((decimal)value);
            else if (value is Integer)
                return (Integer)Math.Floor((decimal)(Integer)value);
            else if (value is Single)
                return (float)Math.Floor((float)value);
            else
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                    "xs:float | xs:double | xs:decimal | xs:integer in fn:floor()");                        
        }

        [XQuerySignature("round", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetRound([XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return value;
            if (Integer.IsDerivedSubtype(value))
                value = Integer.ToInteger(value);
            if (value is Double)
                return Math.Round((double)value);
            else if (value is Decimal)
                return Math.Round((decimal)value);
            else if (value is Integer)
                return (Integer)Math.Round((decimal)(Integer)value);
            else if (value is Single)
                return (float)Math.Round((float)value);
            else
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                    "xs:float | xs:double | xs:decimal | xs:integer in fn:round()");
        }

        [XQuerySignature("round-half-to-even", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetRoundHalfToEven([XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return value;
            if (Integer.IsDerivedSubtype(value))
                value = Integer.ToInteger(value);
            if (value is Double)
                return Math.Round((double)value, MidpointRounding.ToEven);
            else if (value is Decimal)
                return Math.Round((decimal)value, MidpointRounding.ToEven);
            else if (value is Integer)
                return (Integer)Math.Round((decimal)(Integer)value, MidpointRounding.ToEven);
            else if (value is Single)
                return (float)Math.Round((float)value, MidpointRounding.ToEven);
            else
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                    "xs:float | xs:double | xs:decimal | xs:integer in fn:round-half-to-even()");
        }

        [XQuerySignature("round-half-to-even", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetRoundHalfToEven(
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object value,
            [XQueryParameter(XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)] object prec)
        {
            if (value == Undefined.Value || prec == Undefined.Value)
                return Undefined.Value;
            if (Integer.IsDerivedSubtype(value))
                value = Integer.ToInteger(value);
            int p = (int)(Integer)prec;
            if (p < 0)
            {
                int pow = 1;
                for (int k = 1; k <= -p; k++)
                    pow = pow * 10;
                if (value is Double)
                    return pow * Math.Round((double)value / pow, MidpointRounding.ToEven);
                else if (value is Decimal)
                    return pow * Math.Round((decimal)value / pow, MidpointRounding.ToEven);
                else if (value is Integer)
                    return pow * (Integer)Math.Round((decimal)(Integer)value / pow, MidpointRounding.ToEven);
                else if (value is Single)
                    return pow * Math.Round((float)value / pow, MidpointRounding.ToEven);
                else
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                        "xs:float | xs:double | xs:decimal | xs:integer in fn:round-half-to-even()");
            }
            else
            {
                if (value is Double)
                    return Math.Round((double)value, p, MidpointRounding.ToEven);
                else if (value is Decimal)
                    return Math.Round((decimal)value, p, MidpointRounding.ToEven);
                else if (value is Integer)
                    return (Integer)Math.Round((decimal)(Integer)value, p, MidpointRounding.ToEven);
                else if (value is Single)
                    return Math.Round((float)value, p, MidpointRounding.ToEven);
                else
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One),
                        "xs:float | xs:double | xs:decimal | xs:integer in fn:round-half-to-even()");
            }
        }

        [XQuerySignature("compare", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int Compare([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string a,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string b, string collation)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            CultureInfo culture = context.GetCulture(collation);
            return String.Compare(a, b, false, culture);
        }

        [XQuerySignature("compare", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int Compare(
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string a,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string b)
        {
            int res = String.CompareOrdinal(a, b);
            if (res > 0)
                return 1;
            else if (res < 0)
                return -1;
            return 0;
        }

        [XQuerySignature("codepoint-equal", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static bool CodepointEqual([XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string a,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string b)
        {
            return String.CompareOrdinal(a, b) == 0;
        }

        [XQuerySignature("empty")]
        public static bool EmptySequence(XQueryNodeIterator iter)
        {
            XQueryNodeIterator probe = iter.Clone();
            return !probe.MoveNext();
        }

        [XQuerySignature("exists")]
        public static bool ExistsSequence(XQueryNodeIterator iter)
        {
            return !EmptySequence(iter);
        }

        private static IEnumerable<XPathItem> ReverseIterator(LinkedList<XPathItem> list)
        {
            LinkedListNode<XPathItem> node = list.Last;
            while (node != null)
            {
                yield return node.Value;
                node = node.Previous;
            }
        }

        [XQuerySignature("reverse")]
        public static XQueryNodeIterator ReverseSequence(XQueryNodeIterator iter)
        {
            LinkedList<XPathItem> list = new LinkedList<XPathItem>();
            foreach (XPathItem item in iter)
                list.AddLast(item);
            return new NodeIterator(ReverseIterator(list));
        }

        [XQuerySignature("index-of", Return = XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object IndexOfSequence([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType)] object value)
        {
            int pos = 1;
            foreach (XPathItem item in iter)
            {
                if (engine.OperatorEq(item.TypedValue, value) != null)
                    return pos;
                pos++;
            }
            return Undefined.Value;
        }

        [XQuerySignature("remove")]
        public static XQueryNodeIterator Remove(XQueryNodeIterator iter, int index)
        {
            return new NodeIterator(XPathFactory.RemoveIterator(iter, index));
        }

        [XQuerySignature("insert-before")]
        public static XQueryNodeIterator InsertBefore(XQueryNodeIterator iter, int index, XQueryNodeIterator iter2)
        {
            return new NodeIterator(XPathFactory.InsertIterator(iter, index, iter2));
        }

        [XQuerySignature("subsequence")]
        public static XQueryNodeIterator Subsequence(XQueryNodeIterator iter, double startingLoc)
        {
            return new NodeIterator(XPathFactory.SubsequenceIterator(iter, Convert.ToInt32(Math.Round(startingLoc))));
        }

        [XQuerySignature("subsequence")]
        public static XQueryNodeIterator Subsequence(XQueryNodeIterator iter, double startingLoc, double length)
        {
            return new NodeIterator(XPathFactory.SubsequenceIterator(iter, Convert.ToInt32(Math.Round(startingLoc)),
                Convert.ToInt32(Math.Round(length))));
        }

        [XQuerySignature("unordered")]
        public static XQueryNodeIterator Unordered(XQueryNodeIterator iter)
        {
            return iter.Clone();
        }

        [XQuerySignature("zero-or-one")]
        public static XQueryNodeIterator ZeroOrOne(XQueryNodeIterator iter)
        {
            XQueryNodeIterator probe = iter.Clone();
            if (probe.MoveNext())
            {
                if (probe.MoveNext())
                    throw new XQueryException(Properties.Resources.FORG0003);
            }
            return iter.Clone();
        }

        [XQuerySignature("one-or-more")]
        public static XQueryNodeIterator OneOrMore(XQueryNodeIterator iter)
        {
            XQueryNodeIterator probe = iter.Clone();
            if (!probe.MoveNext())
                throw new XQueryException(Properties.Resources.FORG0004);
            return iter.Clone();
        }

        [XQuerySignature("exactly-one")]
        public static XQueryNodeIterator ExactlyOne(XQueryNodeIterator iter)
        {
            XQueryNodeIterator probe = iter.Clone();
            if (!probe.MoveNext())
                throw new XQueryException(Properties.Resources.FORG0005);
            if (probe.MoveNext())
                throw new XQueryException(Properties.Resources.FORG0005);
            return iter.Clone();
        }

        [XQuerySignature("index-of", Return = XmlTypeCode.Integer, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object IndexOfSequence([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter,
            [XQueryParameter(XmlTypeCode.AnyAtomicType)] object value, string collation)
        {
            return IndexOfSequence(engine, iter, value);
        }

        [XQuerySignature("distinct-values", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator DistinctValues([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            HashSet<XPathItem> hs = new HashSet<XPathItem>(new XPathItemEqualityComparer());
            foreach (XPathItem item in iter)
                if (item.Value != String.Empty)
                {
                    if (item.IsNode)
                    {
                        XPathItem tmp = new XQueryAtomicValue(item.TypedValue, item.XmlType, context.nsManager);
                        if (!hs.Contains(tmp))
                            hs.Add(tmp);
                    }
                    else
                        if (!hs.Contains(item))
                            hs.Add(item);
                }
            return new NodeIterator(hs);
        }

        [XQuerySignature("distinct-values", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator DistinctValues([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, string collation)
        {
            return DistinctValues(executive, iter);
        }

        [XQuerySignature("deep-equal")]
        public static bool DeepEqual(XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            TreeComparer comparer = new TreeComparer();
            return comparer.DeepEqual(iter1, iter2);
        }        

        [XQuerySignature("deep-equal")]
        public static bool DeepEqual(XQueryNodeIterator iter1, XQueryNodeIterator iter2, string collation)
        {
            TreeComparer comparer = new TreeComparer(collation);
            return comparer.DeepEqual(iter1, iter2);
        }

        [XQuerySignature("count")]
        public static int CountValues([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            return iter.Count;
        }

        [XQuerySignature("max", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MaxValue([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            object value = null;
            iter = iter.Clone();
            while (iter.MoveNext())
            {
                object curr;
                XPathItem item = iter.Current;
                if (item.TypedValue is UntypedAtomic)
                    curr = Convert.ToDouble(item.TypedValue, context.DefaultCulture);
                else
                    curr = item.TypedValue;
                if (value == null || engine.OperatorGt(curr, value) != null)
                    value = curr;
            }
            if (value == null)
                return Undefined.Value;
            return value;
        }

        [XQuerySignature("max", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MaxValue([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, string collation)
        {            
            return MaxValue(engine, iter);
        }

        [XQuerySignature("min", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MinValue([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            object value = null;
            iter = iter.Clone();
            while (iter.MoveNext())
            {
                object curr;
                XPathItem item = iter.Current;
                if (item.TypedValue is UntypedAtomic)
                    curr = Convert.ToDouble(item.TypedValue, context.DefaultCulture);
                else
                    curr = item.TypedValue;
                if (value == null || engine.OperatorGt(value, curr) != null)
                    value = curr;
            }
            if (value == null)
                return Undefined.Value;
            return value;
        }

        [XQuerySignature("min", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MinValue([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, string collation)
        {
            return MinValue(engine, iter);
        }

        [XQuerySignature("sum")]
        public static object SumValue([Implict] Executive engine,
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            return SumValue(engine, iter, 0);
        }

        private static object DynConvert(CultureInfo culture, object value)
        {
            if (value is string || value is UntypedAtomic)
                return Convert.ToDouble(value, culture);
            else
                return value;
        }

        [XQuerySignature("sum")]
        public static object SumValue([Implict]Executive engine, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType,Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, object zero)
        {
            object value = null;
            XQueryContext context = (XQueryContext)engine.Owner;
            iter = iter.Clone();
            while (iter.MoveNext())
            {
                if (value == null)
                    value = DynConvert(context.DefaultCulture, iter.Current.TypedValue);
                else
                    value = Runtime.DynamicAdd(value,
                        DynConvert(context.DefaultCulture, iter.Current.TypedValue));
            }
            return value != null ? value : zero;
        }

        [XQuerySignature("avg", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object AvgValue([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType,Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            object value = null;
            int count = 0;
            XQueryContext context = (XQueryContext)engine.Owner;
            foreach (XPathItem item in iter)
            {
                if (value == null)
                    value = DynConvert(context.DefaultCulture, item.TypedValue);
                else
                    value = Runtime.DynamicAdd(value,
                        DynConvert(context.DefaultCulture, item.TypedValue));
                count = count + 1;
            }
            if (value == null)
                return Undefined.Value;
            return Runtime.DynamicDiv(value, count);
        }

        [XQuerySignature("collection")]
        public static XQueryNodeIterator GetCollection([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object collection_name)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            if (collection_name == Undefined.Value)
                collection_name = String.Empty;
            return context.CreateCollection((string)collection_name);
        }

        [XQuerySignature("collection")]
        public static XQueryNodeIterator GetCollection([Implict] Executive executive)
        {
            return GetCollection(executive, Undefined.Value);
        }

        [XQuerySignature("id")]
        public static XQueryNodeIterator GetNodesById([Implict] Executive executive, IContextProvider provider, 
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator arg)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XPathItem item = Core.ContextNode(provider);
            if (!item.IsNode)
                throw new XQueryException(Properties.Resources.XPTY0004, "xs:anyAtomicValue", "node() in fn:id(string*)");
            return GetNodesById(arg, (XPathNavigator)item);
        }

        private static IEnumerable<XPathItem> NodesEnumerator(XQueryNodeIterator arg, XPathNavigator node)
        {
            foreach (XPathItem item in arg)
            {
                XPathNavigator curr = node.Clone();
                if (curr.MoveToId(item.Value))
                    yield return curr;
            }
        }

        [XQuerySignature("id")]
        public static XQueryNodeIterator GetNodesById([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator arg, XPathNavigator node)
        {
            return new NodeIterator(NodesEnumerator(arg, node));
        }

        [XQuerySignature("dateTime", Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static DateTimeValue CreateDateTime(
            [XQueryParameter(XmlTypeCode.Date, Cardinality=XmlTypeCardinality.ZeroOrOne)]  DateValue date,
            [XQueryParameter(XmlTypeCode.Time, Cardinality=XmlTypeCardinality.ZeroOrOne)]  TimeValue time)
        {
            if (!date.IsLocal || !time.IsLocal)
            {
                TimeSpan offset;
                if (!date.IsLocal && !time.IsLocal)
                {
                    if (date.Value.Offset != time.Value.Offset)
                        throw new XQueryException(Properties.Resources.FORG0008);
                    offset = date.Value.Offset;
                }
                else
                    if (time.IsLocal)
                        offset = date.Value.Offset;
                    else
                        offset = time.Value.Offset;
                return new DateTimeValue(date.S, new DateTimeOffset(date.Value.Year, date.Value.Month,
                    date.Value.Day, time.Value.Hour, time.Value.Minute, time.Value.Second, offset));
            }
            else
                return new DateTimeValue(date.S, date.Value.Date, time.Value.DateTime);
        }

        [XQuerySignature("current-dateTime")]
        public static DateTimeValue GetCurrentDateTime()
        {
            return new DateTimeValue(false, DateTime.Now);
        }

        [XQuerySignature("current-date")]
        public static DateValue GetCurrentDate()
        {
            return new DateValue(false, DateTime.Now);
        }

        [XQuerySignature("current-time")]
        public static TimeValue GetCurrentTime()
        {
            return new TimeValue(DateTime.Now);
        }

        internal static void ScanLocalNamespaces(XmlNamespaceManager nsmgr, XPathNavigator node)
        {
            if (node.NodeType == XPathNodeType.Root)
                node.MoveToChild(XPathNodeType.Element);
            bool defaultNS = false;
            if (node.MoveToFirstNamespace(XPathNamespaceScope.Local))
            {                
                nsmgr.PushScope();
                do
                {                    
                    nsmgr.AddNamespace(node.Name, node.Value);
                    if (node.Name == "")
                        defaultNS = true;
                }
                while (node.MoveToNextNamespace(XPathNamespaceScope.Local));
            }
            if (!defaultNS && node.NamespaceURI != "")
                nsmgr.AddNamespace("", node.NamespaceURI);
        }

        private static IEnumerable<XPathItem> PrefixEnumerator(XPathNavigator nav, XQueryContext context)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(context.nameTable);
            ScanLocalNamespaces(nsmgr, nav.Clone());            
            foreach (KeyValuePair<string, string> kvp in nsmgr.GetNamespacesInScope(XmlNamespaceScope.All))
                yield return context.CreateItem(kvp.Key);
        }

        [XQuerySignature("in-scope-prefixes", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetInScopePrefixes([Implict] Executive executive, XPathNavigator nav)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            return new NodeIterator(PrefixEnumerator(nav, context));
        }

        [XQuerySignature("namespace-uri-for-prefix", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetNamespaceUriForPrefix([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrOne)] object prefix, XPathNavigator nav)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            string ns;
            if (prefix == Undefined.Value || (string)prefix == String.Empty)
                ns = nav.NamespaceURI;
            else
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(context.nameTable);
                ScanLocalNamespaces(nsmgr, nav.Clone());
                ns = nsmgr.LookupNamespace((string)prefix);
            }
            if (ns == null)
                return Undefined.Value;
            return new AnyUriValue(ns);
        }

        [XQuerySignature("resolve-QName", Return = XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object ResolveQName([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname, XPathNavigator nav)
        {
            if (qname == Undefined.Value)
                return Undefined.Value;
            XQueryContext context = (XQueryContext)executive.Owner;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(context.nameTable);
            ScanLocalNamespaces(nsmgr, nav.Clone());
            return new XQueryAtomicValue(QNameValue.Parse((string)qname, nsmgr), nsmgr);
        }

        [XQuerySignature("QName")]
        public static QNameValue CreateQName([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] object ns, string qname)
        {
            if (ns == Undefined.Value)
                ns = String.Empty;
            XQueryContext context = (XQueryContext)executive.Owner;
            return QNameValue.Parse(qname, (string)ns, context.nameTable);
        }

        [XQuerySignature("prefix-from-QName", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object PrefixFromQName([XQueryParameter(XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname)        
        {
            if (qname == Undefined.Value)
                return qname;
            QNameValue qnameValue = qname as QNameValue;
            if (qname == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(qname.GetType(), XmlTypeCardinality.One), "QName");
            if (qnameValue.Prefix == "")
                return Undefined.Value;
            return qnameValue.Prefix;
        }

        [XQuerySignature("local-name-from-QName", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object LocalNameFromQName([XQueryParameter(XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname)
        {
            if (qname == Undefined.Value)
                return qname;
            QNameValue qnameValue = qname as QNameValue;
            if (qname == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(qname.GetType(), XmlTypeCardinality.One), "QName");
            return qnameValue.LocalName;
        }

        [XQuerySignature("namespace-uri-from-QName", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object NamespaceUriFromQName([XQueryParameter(XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname)
        {
            if (qname == Undefined.Value)
                return Undefined.Value;
            QNameValue qnameValue = qname as QNameValue;
            if (qname == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(qname.GetType(), XmlTypeCardinality.One), "QName");
            return new AnyUriValue(qnameValue.NamespaceUri);
        }

        [XQuerySignature("string-to-codepoints", Return = XmlTypeCode.Int, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator StringToCodepoint([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] object text)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (text == Undefined.Value)
                return EmptyIterator.Shared;
            return new NodeIterator(XPathFactory.CodepointIterator(context, (string)text));
        }

        [XQuerySignature("codepoints-to-string")]
        public static string CodepointToString([XQueryParameter(XmlTypeCode.Integer, 
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (XPathItem item in iter)
            {
                int codepoint = item.ValueAsInt;
                if (!XmlCharType.Instance.IsCharData((char)codepoint))
                    throw new XQueryException(Properties.Resources.FOCH0001, codepoint.ToString("X"));
                try
                {
                    sb.Append(Convert.ToChar(codepoint));
                }
                catch
                {
                    throw new XQueryException(Properties.Resources.FOCH0001, codepoint.ToString("X"));
                }
            }
            return sb.ToString();
        }

        [XQuerySignature("default-collation")]
        public static string DefaultCollation([Implict] Executive engine)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (context.DefaultCollation == null)
                return XmlReservedNs.NsCollationCodepoint;
            return context.DefaultCollation;
        }

        [XQuerySignature("resolve-uri", Return = XmlTypeCode.AnyUri, Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static AnyUriValue ResolveUri([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrOne)] string relative)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (context.BaseUri == null)
                throw new XQueryException(Properties.Resources.FONS0005);
            try
            {                
                return new AnyUriValue(new Uri(new Uri(context.BaseUri), relative));
            }
            catch (UriFormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0009);
            }
        }

        [XQuerySignature("resolve-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static AnyUriValue ResolveUri(
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string relative,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string baseUri)
        {
            if (baseUri == "")
            {
                if (!Uri.IsWellFormedUriString(relative, UriKind.Absolute))
                    throw new XQueryException(Properties.Resources.FORG0009);
                return new AnyUriValue(relative);
            }
            try
            {
                return new AnyUriValue(new Uri(new Uri(baseUri), relative));
            }
            catch (UriFormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0009);
            }
        }

        [XQuerySignature("static-base-uri", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object StaticBaseUri([Implict] Executive engine)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (context.BaseUri == null)
                return Undefined.Value;
            return new AnyUriValue(context.BaseUri);
        }

        [XQuerySignature("implicit-timezone")]
        public static DayTimeDurationValue ImplicitTimezone()
        {
            return new DayTimeDurationValue(TimeZoneInfo.Local.BaseUtcOffset);
        }
    }
}
