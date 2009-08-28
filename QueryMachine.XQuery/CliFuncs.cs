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

namespace DataEngine.XQuery
{
    public class CliFuncs
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

        [XQuerySignature("node-name", Return = XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetNodeName([Implict] Executive executive, [XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return EmptyIterator.Shared;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:node-name()");
            return CreateQName(executive, nav.NamespaceURI, nav.Name);
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
        public static string GetNamespaceUri(IContextProvider provider)
        {
            return GetNamespaceUri(Core.NodeValue(Core.ContextNode(provider)));
        }

        [XQuerySignature("namespace-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static string GetNamespaceUri([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return String.Empty;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:local-name()");
            return nav.NamespaceURI;
        }

        [XQuerySignature("nilled", Return = XmlTypeCode.Boolean, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static bool GetNilled([XQueryParameter(XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)] XPathNavigator nav)
        {
            if (nav.SchemaInfo != null)
                return nav.SchemaInfo.IsNil;
            return false;
        }

        [XQuerySignature("base-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetBaseUri([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return node;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:base-uri()");
            Uri uri = new Uri(nav.BaseURI);
            if (uri.IsFile)
                return uri.LocalPath;
            else
                return nav.BaseURI;
        }

        [XQuerySignature("base-uri", Return = XmlTypeCode.AnyUri, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetBaseUri(IContextProvider provider)
        {
            return GetBaseUri(Core.NodeValue(Core.ContextNode(provider)));
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
            if (nav.NodeType != XPathNodeType.Root)
                return Undefined.Value;
            Uri uri = new Uri(nav.BaseURI);
            if (uri.IsFile)
                return uri.LocalPath;
            else
                return nav.BaseURI;            
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
                sb.AppendFormat("{0}:", label);
            foreach (XPathItem item in iter)
            {
                sb.Append(" ");
                if (item.IsNode)
                    sb.Append(((XPathNavigator)item).OuterXml);
                else
                    sb.Append(item.Value);
            }
            Console.Out.WriteLine(sb.ToString());
            return EmptyIterator.Shared;
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
            foreach (object arg in args)
                if (arg != Undefined.Value)
                    sb.Append(arg.ToString());
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
            int pos = Convert.ToInt32(Math.Round(startingLoc)) - 1;
            if (pos < value.Length)
            {
                int len = Convert.ToInt32(Math.Round(length));
                if (pos + len > value.Length)
                    len = value.Length - pos;
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
        public static int StringLength(IContextProvider provider)
        {
            return StringLength(Core.StringValue(Core.Atomize(Core.ContextNode(provider))));
        }

        [XQuerySignature("normalize-space")]
        public static string NormalizeSpace([XQueryParameter(XmlTypeCode.String,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object item)
        {
            if (item == Undefined.Value)
                return String.Empty;
            string value = (string)item;
            StringBuilder builder;
            int length = 0;
            while (length < value.Length)
            {
                if (XmlCharType.Instance.IsWhiteSpace(value[length]))
                    break;
                length++;
            }
            if (length == value.Length)
                return value;
            if (length != 0)
                builder = new StringBuilder(value.Substring(0, length), value.Length);
            else
            {
                length++;
                while (length < value.Length)
                {
                    if (!XmlCharType.Instance.IsWhiteSpace(value[length]))
                        break;
                    length++;
                }
                if (length == value.Length)
                    return String.Empty;
                int startIndex = length++;
                while (length < value.Length)
                {
                    if (XmlCharType.Instance.IsWhiteSpace(value[length]))
                        break;
                    length++;
                }
                if (length == value.Length)
                    return value.Substring(startIndex, length - startIndex);
                builder = new StringBuilder(value.Substring(startIndex, length - startIndex), value.Length - startIndex);
            }
            bool flag = true;
            while (length < value.Length)
            {
                char character = value[length];
                if (XmlCharType.Instance.IsWhiteSpace(character))
                    flag = true;
                else
                {
                    if (flag)
                    {
                        builder.Append(' ');
                        flag = false;
                    }
                    builder.Append(character);
                }
                length++;
            }
            return builder.ToString();
        }

        [XQuerySignature("normalize-space")]
        public static string NormalizeSpace(IContextProvider provider)
        {
            return NormalizeSpace(Core.StringValue(Core.Atomize(Core.ContextNode(provider))));
        }

        [XQuerySignature("normalize-unicode")]
        public static string NormalizeUnicode(string value, string form)
        {
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
            return Uri.EscapeDataString((string)value);
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
            StringBuilder builder = new StringBuilder();
            foreach (byte num in Encoding.UTF8.GetBytes(value))
            {
                if (num == 0x20)
                    builder.Append("%20");
                else if ((((num < 0x7f && num >= 0x20) && (num != 60 && num != 0x3e)) && 
                    ((num != 0x22 && num != 0x7b) && (num != 0x7d && num != 0x7c))) && (((num != 0x5c) && (num != 0x5e)) && (num != 0x60)))
                    builder.Append((char)num);
                else
                {
                    int num2 = num;
                    int num3 = num2 / 0x10;
                    int num4 = num2 % 0x10;
                    chArray[1] = (num3 >= 10) ? ((char)(0x41 + (num3 - 10))) : ((char)(0x30 + num3));
                    chArray[2] = (num4 >= 10) ? ((char)(0x41 + (num4 - 10))) : ((char)(0x30 + num4));
                    builder.Append(chArray);
                }
            }
            return builder.ToString();
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
        public static bool Contains(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
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
        public static bool StartsWith(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
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
        public static bool EndsWith(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
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
        public static string SubstringBefore(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
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
        public static string SubstringAfter(
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg1,
            [XQueryParameter(XmlTypeCode.String,
                Cardinality = XmlTypeCardinality.ZeroOrOne)] object arg2,
            string collation)
        {
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
                        flags |= RegexOptions.IgnoreCase;
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
            return Regex.IsMatch(input, pattern, flags);
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
            return Regex.IsMatch(input, pattern);
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
                yield return new XQueryAtomicValue(str);
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

        [XQuerySignature("years-from-duration", Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static int YearsFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)]TimeSpan value)
        {
            return value.Days / 360;
        }

        [XQuerySignature("months-from-duration", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MonthsFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)]TimeSpan value)
        {
            return value.Days / 30;
        }

        [XQuerySignature("days-from-duration", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int DaysFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)]TimeSpan value)
        {
            return value.Days;
        }

        [XQuerySignature("hours-from-duration", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int HoursFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)]TimeSpan value)
        {
            return value.Hours;
        }

        [XQuerySignature("minutes-from-duration", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MinutesFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)]TimeSpan value)
        {
            return value.Minutes;
        }

        [XQuerySignature("seconds-from-duration", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int SecondsFromDuration([XQueryParameter(XmlTypeCode.Duration, Cardinality = XmlTypeCardinality.ZeroOrOne)]TimeSpan value)
        {
            return value.Seconds;
        }

        [XQuerySignature("year-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int YearFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)]DateTime value)
        {
            return value.Year;
        }

        [XQuerySignature("month-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MonthFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Month;
        }

        [XQuerySignature("day-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int DayFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Day;
        }

        [XQuerySignature("hours-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int HoursFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Hour;
        }

        [XQuerySignature("minutes-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MinutesFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Minute;
        }

        [XQuerySignature("seconds-from-dateTime", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int SecondsFromDateTime([XQueryParameter(XmlTypeCode.DateTime, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Second;
        }

        [XQuerySignature("year-from-date", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int YearFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Year;
        }

        [XQuerySignature("month-from-date", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int MonthFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Month;
        }

        [XQuerySignature("day-from-date", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int DayFromDate([XQueryParameter(XmlTypeCode.Date, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Day;
        }

        [XQuerySignature("hours-from-time", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int HoursFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Hour;
        }

        [XQuerySignature("minutes-from-time", Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static int MinutesFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Minute;
        }

        [XQuerySignature("seconds-from-time", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int SecondsFromTime([XQueryParameter(XmlTypeCode.Time, Cardinality = XmlTypeCardinality.ZeroOrOne)] DateTime value)
        {
            return value.Second;
        }

        [XQuerySignature("abs", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static double GetAbs([XQueryParameter(XmlTypeCode.Double, Cardinality = XmlTypeCardinality.ZeroOrOne)] double value)
        {
            return Math.Abs(value);
        }

        [XQuerySignature("ceiling", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static double GetCeiling([XQueryParameter(XmlTypeCode.Double, Cardinality = XmlTypeCardinality.ZeroOrOne)] double value)
        {
            return Math.Ceiling(value);
        }

        [XQuerySignature("floor", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static double GetFloor([XQueryParameter(XmlTypeCode.Double, Cardinality = XmlTypeCardinality.ZeroOrOne)] double value)
        {
            return Math.Floor(value);
        }

        [XQuerySignature("round", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static double GetRound([XQueryParameter(XmlTypeCode.Double, Cardinality = XmlTypeCardinality.ZeroOrOne)] double value)
        {
            return Math.Round(value, MidpointRounding.AwayFromZero);
        }

        [XQuerySignature("round-half-to-even", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static double GetRoundHalfToEven([XQueryParameter(XmlTypeCode.Double, Cardinality = XmlTypeCardinality.ZeroOrOne)] double value)
        {
            return Math.Round(value, MidpointRounding.ToEven);
        }

        [XQuerySignature("compare", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int Compare([XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string a,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string b, string collation)
        {
            return Compare(a, b);
        }

        [XQuerySignature("compare", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static int Compare([XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string a,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string b)
        {
            return String.Compare(a, b);
        }

        [XQuerySignature("codepoint-equal", Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static bool CodepointEqual([XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string a,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] string b)
        {
            return String.Compare(a, b) == 0;
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
        public static object IndexOfSequence([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType)] object value)
        {
            int pos = 1;
            foreach (XPathItem item in iter)
            {
                if (Runtime.DynamicEq(item.TypedValue, value) != null)
                    return pos;
                pos++;
            }
            return Undefined.Value;
        }

        [XQuerySignature("remove")]
        public static XQueryNodeIterator Remove(XQueryNodeIterator iter, int index)
        {
            return new NodeIterator(XPath.RemoveIterator(iter, index));
        }

        [XQuerySignature("insert-before")]
        public static XQueryNodeIterator InsertBefore(XQueryNodeIterator iter, int index, XQueryNodeIterator iter2)
        {
            return new NodeIterator(XPath.InsertIterator(iter, index, iter2));
        }

        [XQuerySignature("subsequence")]
        public static XQueryNodeIterator Subsequence(XQueryNodeIterator iter, double startingLoc)
        {
            return new NodeIterator(XPath.SubsequenceIterator(iter, Convert.ToInt32(Math.Round(startingLoc))));
        }

        [XQuerySignature("subsequence")]
        public static XQueryNodeIterator Subsequence(XQueryNodeIterator iter, double startingLoc, double length)
        {
            return new NodeIterator(XPath.SubsequenceIterator(iter, Convert.ToInt32(Math.Round(startingLoc)),
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
        public static object IndexOfSequence([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter,
            [XQueryParameter(XmlTypeCode.AnyAtomicType)] object value, string collation)
        {
            return IndexOfSequence(iter, value);
        }

        [XQuerySignature("distinct-values", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator DistinctValues([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            HashSet<XPathItem> hs = new HashSet<XPathItem>(new XPathItemEqualityComparer());
            foreach (XPathItem item in iter)
                if (item.Value != String.Empty)
                {
                    if (item.IsNode)
                    {
                        XPathItem tmp = new XQueryAtomicValue(item.TypedValue, item.XmlType);
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
        public static XQueryNodeIterator DistinctValues([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, string collation)
        {
            return DistinctValues(iter);
        }

        private static bool NodeEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.NodeType != nav2.NodeType)
                return false;
            switch (nav1.NodeType)
            {
                case XPathNodeType.Element:
                    return ElementEqual(nav1, nav2);

                case XPathNodeType.Attribute:
                    return AttributeEqual(nav1, nav2);

                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                case XPathNodeType.Comment:
                    return nav1.Value == nav2.Value;

                case XPathNodeType.ProcessingInstruction:
                    return ProcessingInstructionEqual(nav1, nav2);

                default:
                    return DeepEqual(nav1, nav2);
            }            
        }

        private static bool ElementEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.LocalName != nav2.LocalName || nav1.NamespaceURI != nav2.NamespaceURI)
                return false;
            return ElementAttributesEqual(nav1.Clone(), nav2.Clone()) && DeepEqual(nav1, nav2);
        }

        private static bool ElementAttributesEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.HasAttributes != nav2.HasAttributes)
                return false;
            if (nav1.HasAttributes)
            {
                bool flag1 = nav1.MoveToFirstAttribute();
                bool flag2 = nav2.MoveToFirstAttribute();
                while (flag1 && flag2)
                {
                    flag1 = nav1.MoveToNextAttribute();
                    flag2 = nav2.MoveToNextAttribute();
                }
                nav1.MoveToParent();
                nav2.MoveToParent();
                if (flag1 != flag2)
                    return false;
                for (bool flag3 = nav1.MoveToFirstAttribute(); flag3; flag3 = nav1.MoveToNextAttribute())
                {
                    bool flag4 = nav2.MoveToFirstAttribute();
                    while (flag4)
                    {
                        if (AttributeEqual(nav1, nav2))
                            break;
                        flag4 = nav2.MoveToNextAttribute();
                    }
                    nav2.MoveToParent();
                    if (!flag4)
                    {
                        nav1.MoveToParent();
                        return false;
                    }
                }
                nav1.MoveToParent();
            }
            return true;
        }

        private static bool ProcessingInstructionEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            return nav1.LocalName == nav2.LocalName &&
                nav1.Value == nav2.Value;
        }

        private static bool AttributeEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            if (nav1.LocalName != nav2.LocalName || nav1.NamespaceURI != nav2.NamespaceURI)
                return false;
            return Runtime.DynamicEq(nav1.TypedValue, nav2.TypedValue) != null;
        }

        private static bool DeepEqual(XPathNavigator nav1, XPathNavigator nav2)
        {
            bool flag1;
            bool flag2;
            XPathNodeIterator iter1 = nav1.SelectChildren(XPathNodeType.All);
            XPathNodeIterator iter2 = nav2.SelectChildren(XPathNodeType.All);
            do
            {
                flag1 = iter1.MoveNext();
            }
            while ((flag1 && iter1.Current.NodeType != XPathNodeType.Element) &&
                iter1.Current.NodeType != XPathNodeType.Text);
            do
            {
                flag2 = iter2.MoveNext();
                if (!flag2 || iter2.Current.NodeType == XPathNodeType.Element)
                    break;
            }
            while (iter2.Current.NodeType != XPathNodeType.Text);
            while (flag1 && flag2)
            {
                if (!NodeEqual(iter1.Current, iter2.Current))
                    return false;
                do
                {
                    flag1 = iter1.MoveNext();
                }
                while ((flag1 && iter1.Current.NodeType != XPathNodeType.Element) &&
                    iter1.Current.NodeType != XPathNodeType.Text);
                do
                {
                    flag2 = iter2.MoveNext();
                }
                while ((flag2 && iter2.Current.NodeType != XPathNodeType.Element) &&
                    iter2.Current.NodeType != XPathNodeType.Text);
            }
            return flag1 == flag2;
        }

        [XQuerySignature("deep-equal")]
        public static bool DeepEqual(XQueryNodeIterator iter1, XQueryNodeIterator iter2)
        {
            iter1 = iter1.Clone();
            iter2 = iter2.Clone();
            bool flag1;
            bool flag2;
            do
            {
                flag1 = iter1.MoveNext();
                flag2 = iter2.MoveNext();
                if (flag1 != flag2)
                    return false;
                else
                    if (flag1 && flag2)
                    {
                        if (iter1.Current.IsNode != iter2.Current.IsNode)
                            return false;
                        else
                        {
                            if (iter1.Current.IsNode && iter2.Current.IsNode)
                            {
                                return NodeEqual((XPathNavigator)iter1.Current,
                                    (XPathNavigator)iter2.Current);
                            }
                            else
                                return Runtime.DynamicEq(iter1.Current.TypedValue,
                                    iter2.Current.TypedValue) != null;
                        }
                    }
            }
            while (flag1 && flag2);
            return true;
        }        

        [XQuerySignature("deep-equal")]
        public static bool DeepEqual(XQueryNodeIterator iter1, XQueryNodeIterator iter2, string collation)
        {
            return DeepEqual(iter1, iter2);
        }

        [XQuerySignature("count")]
        public static int CountValues([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            return iter.Count;
        }

        [XQuerySignature("max", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MaxValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            object value = null;
            foreach (XPathItem item in iter)
            {
                object curr;
                if (item.XmlType == null || 
                    item.XmlType == XQuerySequenceType.UntypedAtomic)
                    curr = Core.Number(item.Value);
                else
                    curr = item.TypedValue;
                if (value == null || Runtime.DynamicGt(curr, value) != null)
                    value = curr;
            }
            if (value == null)
                return Undefined.Value;
            return value;
        }

        [XQuerySignature("max", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MaxValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, string collation)
        {            
            return MaxValue(iter);
        }

        [XQuerySignature("min", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MinValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            object value = null;
            foreach (XPathItem item in iter)
            {
                object curr;
                if (item.XmlType == null || 
                    item.XmlType == XQuerySequenceType.UntypedAtomic)
                    curr = Core.Number(item.Value);
                else
                    curr = item.TypedValue;
                if (value == null || Runtime.DynamicGt(value, curr) != null)
                    value = curr;
            }
            if (value == null)
                return Undefined.Value;
            return value;
        }

        [XQuerySignature("min", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object MinValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, string collation)
        {
            return MinValue(iter);
        }

        [XQuerySignature("sum")]
        public static object SumValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            return SumValue(iter, 0);
        }

        private static object DynConvert(object value)
        {
            if (value is string)
                return Double.Parse((string)value, CultureInfo.InvariantCulture);
            else
                return value;
        }

        [XQuerySignature("sum")]
        public static object SumValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter, object zero)
        {
            object value = null;
            foreach (XPathItem item in iter)
            {
                if (value == null)
                    value = DynConvert(item.TypedValue);
                else
                    value = DynConvert(Runtime.DynamicAdd(value, item.TypedValue));
            }
            return value != null ? value : zero;
        }

        [XQuerySignature("avg", Return = XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object AvgValue([XQueryParameter(XmlTypeCode.AnyAtomicType,
            Cardinality = XmlTypeCardinality.ZeroOrMore)] XQueryNodeIterator iter)
        {
            object value = null;
            double count = 0.0;
            foreach (XPathItem item in iter)
            {
                if (value == null)
                    value = DynConvert(item.TypedValue);
                else
                    value = DynConvert(Runtime.DynamicAdd(value, item.TypedValue));
                count = count + 1;
            }
            if (value == null)
                return Undefined.Value;
            return Runtime.DynamicDiv(value, count);
        }

        [XQuerySignature("collection")]
        public static XPathNavigator GetCollection([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality = XmlTypeCardinality.ZeroOrOne)] object collection_name)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            //if (collection_name == Undefined.Value)
            //    return context.CreateCollection(Core.StringValue(Core.Atomize(Core.Context(executive))));
            //else
            return context.CreateCollection((string)collection_name);
        }

        [XQuerySignature("collection")]
        public static XPathNavigator GetCollection([Implict] Executive executive)
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

        [XQuerySignature("dateTime")]
        public static XQueryNodeIterator CreateDateTime([XQueryParameter(XmlTypeCode.Date)] DateTime date,
            [XQueryParameter(XmlTypeCode.Date)] DateTime time)
        {
            DateTime dateTime = new DateTime(date.Year, date.Month, 
                date.Day, time.Hour, time.Minute, time.Second);
            XPathItem[] item = new XPathItem[] { new XQueryAtomicValue(dateTime, 
                XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime)) };
            return new NodeIterator(item);
        }

        [XQuerySignature("current-dateTime", Return = XmlTypeCode.DateTime, 
            Cardinality=XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetCurrentDateTime()
        {
            XPathItem[] item = new XPathItem [] { new XQueryAtomicValue(DateTime.Now, 
                XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime)) };
            return new NodeIterator(item);
        }

        [XQuerySignature("current-date", Return = XmlTypeCode.Date,
            Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetCurrentDate()
        {
            XPathItem[] item = new XPathItem[] { new XQueryAtomicValue(DateTime.Today, 
                XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Date)) };
            return new NodeIterator(item);
        }

        [XQuerySignature("current-time", Return = XmlTypeCode.Time,
            Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetCurrentTime()
        {
            XPathItem[] item = new XPathItem[] { new XQueryAtomicValue(DateTime.Now, 
                XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Time)) };
            return new NodeIterator(item);
        }

        [XQuerySignature("static-base-uri")]
        public static String GetStaticBaseUri([Implict] Executive executive)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            if (context.BaseUri == null)
                return String.Empty;
            return context.BaseUri;
        }

        private static void ScanLocalNamespaces(XmlNamespaceManager nsmgr, XPathNavigator node)
        {
            XPathNavigator parent = node.Clone();
            if (parent.MoveToParent())
                ScanLocalNamespaces(nsmgr, parent);
            if (node.NodeType == XPathNodeType.Root)
                node.MoveToChild(XPathNodeType.Element);
            if (node.MoveToFirstNamespace(XPathNamespaceScope.Local))
            {
                nsmgr.PushScope();
                do
                {
                    nsmgr.AddNamespace(node.Name, node.Value);
                }
                while (node.MoveToNextNamespace(XPathNamespaceScope.Local));
            }
        }

        private static IEnumerable<XPathItem> PrefixEnumerator(XPathNavigator nav, XmlNameTable nameTable)
        {            
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);
            ScanLocalNamespaces(nsmgr, nav.Clone());
            foreach (KeyValuePair<string, string> kvp in nsmgr.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml))
                yield return new XQueryAtomicValue(nsmgr.LookupPrefix(kvp.Value));
        }

        [XQuerySignature("in-scope-prefixes", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetInScopePrefixes([Implict] Executive executive, XPathNavigator nav)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            return new NodeIterator(PrefixEnumerator(nav, context.nameTable));
        }

        [XQuerySignature("namespace-uri-for-prefix", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator GetNamespaceUriForPrefix([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrOne)] object prefix, XPathNavigator nav)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(context.nameTable);
            ScanLocalNamespaces(nsmgr, nav.Clone());
            string ns;
            if (prefix == Undefined.Value)
                ns = nsmgr.DefaultNamespace;
            else
                ns = nsmgr.LookupNamespace((string)prefix);
            if (ns == null)
                return EmptyIterator.Shared;
            XPathItem[] item = new XPathItem[] { new XQueryAtomicValue(ns) };
            return new NodeIterator(item);            
        }

        [XQuerySignature("resolve-QName", Return = XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator ResolveQName([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname, XPathNavigator nav)
        {
            if (qname == Undefined.Value)
                return EmptyIterator.Shared;
            XQueryContext context = (XQueryContext)executive.Owner;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(context.nameTable);
            ScanLocalNamespaces(nsmgr, nav.Clone());
            XmlSchemaType simpleType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.QName);
            XPathItem[] item = new XPathItem[] { new XQueryAtomicValue(simpleType.Datatype.ParseValue(
                (string)qname, context.nameTable, nsmgr), simpleType) };
            return new NodeIterator(item);            
        }

        [XQuerySignature("QName", Return = XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrMore)]
        public static XQueryNodeIterator CreateQName([Implict] Executive executive,
            [XQueryParameter(XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)] object ns, string qname)
        {
            if (ns == Undefined.Value)
                ns = String.Empty;
            XQueryContext context = (XQueryContext)executive.Owner;
            string prefix;
            string localName;
            QNameParser.Split(qname, out prefix, out localName);
            if (prefix != "" && ((string)ns) == "")
                throw new XQueryException(Properties.Resources.FOCA0002, qname);
            XmlQualifiedName qualifiedName = new XmlQualifiedName(qname, (string)ns);
            XmlSchemaType simpleType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.QName);
            XPathItem[] item = new XPathItem[] { new XQueryAtomicValue(qualifiedName, simpleType) };
            return new NodeIterator(item);            
        }

        [XQuerySignature("prefix-from-QName", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object PrefixFromQName([XQueryParameter(XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname)        
        {
            if (qname == Undefined.Value)
                return qname;
            if (!(qname is XmlQualifiedName))
                throw new XQueryException(Properties.Resources.XPTY0004, 
                    new XQuerySequenceType(qname.GetType(), XmlTypeCardinality.One), "QName");
            XmlQualifiedName qualifiedName = (XmlQualifiedName)qname;
            string prefix;
            string localName;
            QNameParser.Split(qualifiedName.Name, out prefix, out localName);
            return prefix;
        }

        [XQuerySignature("local-name-from-QName", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object LocalNameFromQName([XQueryParameter(XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname)
        {
            if (qname == Undefined.Value)
                return qname;
            if (!(qname is XmlQualifiedName))
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(qname.GetType(), XmlTypeCardinality.One), "QName");
            XmlQualifiedName qualifiedName = (XmlQualifiedName)qname;
            string prefix;
            string localName;
            QNameParser.Split(qualifiedName.Name, out prefix, out localName);
            return localName;
        }

        [XQuerySignature("namespace-uri-from-QName", Return = XmlTypeCode.String, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object NamespaceUriFromQName([XQueryParameter(XmlTypeCode.QName, Cardinality = XmlTypeCardinality.ZeroOrOne)] object qname)
        {
            if (qname == Undefined.Value)
                return qname;
            if (!(qname is XmlQualifiedName))
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(qname.GetType(), XmlTypeCardinality.One), "QName");
            XmlQualifiedName qualifiedName = (XmlQualifiedName)qname;
            return qualifiedName.Namespace;
        }
    }
}
