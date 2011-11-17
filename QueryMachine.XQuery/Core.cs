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
using System.IO;
using System.Diagnostics;
using System.Globalization;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.XQuery.Util;
using System.Threading.Tasks;

namespace DataEngine.XQuery
{
    public partial class ID
    {
        public static readonly object DynExecuteExpr = ATOM.Create("dyn_execute");       
        public static readonly object Doc = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "doc" }, true);
        public static readonly object Root = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "root" }, true);        
        public static readonly object Position = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "position" }, true);
        public static readonly object Last = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "last" }, true);
        public static readonly object ContextNode = ATOM.Create("context-node");

        public static readonly object BooleanValue = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "boolean" }, true);
        public static readonly object True = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "true" }, true);
        public static readonly object False = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "false" }, true);
        public static readonly object Not = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "not" }, true);

        public static readonly object String = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "string" }, true);
        public static readonly object Number = ATOM.Create(XmlReservedNs.NsXQueryFunc, new string[] { "number" }, true);

        public static readonly object DynCreateDocument = ATOM.Create("dyn_root");
        public static readonly object DynCreateElement = ATOM.Create("dyn_element");
        public static readonly object DynCreateAttribute = ATOM.Create("dyn_attribute");
        public static readonly object DynCreateCData = ATOM.Create("dyn_cdata");
        public static readonly object DynCreateText = ATOM.Create("dyn_text");
        public static readonly object DynCreateComment = ATOM.Create("dyn_comment");
        public static readonly object DynCreatePi = ATOM.Create("dyn_pi");
        public static readonly object DynZeroOrOne = ATOM.Create("dyn_zero_or_one");
        
        public static readonly object CreateBuilder = ATOM.Create("create-builder");
        public static readonly object CreateNavigator = ATOM.Create("create-navigator");
        public static readonly object WriteBeginElement = ATOM.Create("begin-element");
        public static readonly object WriteEndElement = ATOM.Create("end-element");
        public static readonly object WriteBeginAttribute = ATOM.Create("begin-attribute");
        public static readonly object WriteEndAttribute = ATOM.Create("end-attribute");
        public static readonly object WriteRaw = ATOM.Create("write-raw");
        public static readonly object WriteNode = ATOM.Create("write-node");
        public static readonly object WriteComment = ATOM.Create("write-comment");
        public static readonly object WritePi = ATOM.Create("write-pi");
        public static readonly object WriteString = ATOM.Create("write-string");
        public static readonly object WriteWhitespace = ATOM.Create("write-ws");
        public static readonly object WriteCdata = ATOM.Create("write-cdata");
                
        public static readonly object Atomize = ATOM.Create("atomize");
        public static readonly object AtomizeX = ATOM.Create("atomize_x");
        public static readonly object AtomizeBody = ATOM.Create("atomize#");
        public static readonly object NodeValue = ATOM.Create("node");
        public static readonly object NodeValueX = ATOM.Create("node_x");
        public static readonly object NodeValueBody = ATOM.Create("node#");        
        public static readonly object FormatValue = ATOM.Create("format-value");

        public static readonly object InstanceOf = ATOM.Create("instance-of");
        public static readonly object CastTo = ATOM.Create("cast-to");
        public static readonly object CastToItem = ATOM.Create("cast-to-item");
        public static readonly object Castable = ATOM.Create("castable");
        public static readonly object TreatAs = ATOM.Create("treat-as");
        public static readonly object CastArg = ATOM.Create("cast-arg");

        public static readonly object GeneralEQ = ATOM.Create("general-eq");
        public static readonly object GeneralNE = ATOM.Create("general-ne");
        public static readonly object GeneralLT = ATOM.Create("general-lt");
        public static readonly object GeneralGT = ATOM.Create("general-gt");
        public static readonly object GeneralGE = ATOM.Create("general-ge");
        public static readonly object GeneralLE = ATOM.Create("general-le");

        public static readonly object Some = ATOM.Create("some");
        public static readonly object Every = ATOM.Create("every");

        public static readonly object SameNode = ATOM.Create("is-same-node");
        public static readonly object PrecedingNode = ATOM.Create("is-preceding-node");
        public static readonly object FollowingNode = ATOM.Create("is-following-node");

        public static readonly object Range = ATOM.Create("range");
        public static readonly object Except = ATOM.Create("except");
        public static readonly object Intersect = ATOM.Create("intersect");
        public static readonly object Union = ATOM.Create("union");

        public static readonly object Context = ATOM.Create("$context");
        public static readonly object Seq = ATOM.Create("$seq");
        public static readonly object IsUnknown = ATOM.Create("is-unknown");
        
        public static readonly object Par = ATOM.Create("par");
        public static readonly object ExactlyOne = ATOM.Create("dyn-exactly-one");
        public static readonly object RaiseUnknown = ATOM.Create("raise-unknown");
        public static readonly object Validate = ATOM.Create("validate");
        public static readonly object CastToNumber1 = ATOM.Create("cast-to-number1");
        public static readonly object CastToNumber2 = ATOM.Create("cast-to-number2");
        public static readonly object CastToNumber3 = ATOM.Create("cast-to-number3");
    }
    
    public static class Core
    {        
        static Core()
        {
            GlobalSymbols.DefineStaticOperator(ID.CreateNavigator, typeof(Core), "CreateNavigator");
            GlobalSymbols.DefineStaticOperator(ID.DynExecuteExpr, typeof(Core), "DynExecuteExpr");
            GlobalSymbols.DefineStaticOperator(ID.DynCreateDocument, typeof(Core), "DynCreateDocument");
            GlobalSymbols.DefineStaticOperator(ID.DynCreateElement, typeof(Core), "DynCreateElement");
            GlobalSymbols.DefineStaticOperator(ID.DynCreateAttribute, typeof(Core), "DynCreateAttribute");
            GlobalSymbols.DefineStaticOperator(ID.DynCreateCData, typeof(Core), "DynCreateCData");
            GlobalSymbols.DefineStaticOperator(ID.DynCreateText, typeof(Core), "DynCreateText");
            GlobalSymbols.DefineStaticOperator(ID.DynCreateComment, typeof(Core), "DynCreateComment");
            GlobalSymbols.DefineStaticOperator(ID.DynCreatePi, typeof(Core), "DynCreatePi");
            GlobalSymbols.DefineStaticOperator(ID.CreateBuilder, typeof(Core), "CreateBuilder");
            GlobalSymbols.DefineStaticOperator(ID.WriteBeginElement, typeof(Core), "BeginElement");
            GlobalSymbols.DefineStaticOperator(ID.WriteEndElement, typeof(Core), "EndElement");
            GlobalSymbols.DefineStaticOperator(ID.WriteBeginAttribute, typeof(Core), "BeginAttribute");
            GlobalSymbols.DefineStaticOperator(ID.WriteEndAttribute, typeof(Core), "EndAttribute");            
            GlobalSymbols.DefineStaticOperator(ID.WriteComment, typeof(Core), "CreateComment");
            GlobalSymbols.DefineStaticOperator(ID.WritePi, typeof(Core), "CreatePi");
            GlobalSymbols.DefineStaticOperator(ID.WriteCdata, typeof(Core), "CreateCdata");
            GlobalSymbols.DefineStaticOperator(ID.WriteNode, typeof(Core), "WriteNode");
            GlobalSymbols.DefineStaticOperator(ID.WriteString, typeof(Core), "WriteString");
            GlobalSymbols.DefineStaticOperator(ID.WriteWhitespace, typeof(Core), "WriteWhitespace");
            
            GlobalSymbols.DefineStaticOperator(ID.FormatValue, typeof(Core), "FormatValue");
            GlobalSymbols.DefineStaticOperator(ID.AtomizeBody, typeof(Core), "Atomize");
            GlobalSymbols.DefineStaticOperator(ID.NodeValueBody, typeof(Core), "NodeValue");
            GlobalSymbols.DefineStaticOperator(ID.ContextNode, typeof(Core), "ContextNode");
            GlobalSymbols.DefineStaticOperator(ID.Seq, typeof(Core), "CreateSequence");
            GlobalSymbols.DefineStaticOperator(ID.IsUnknown, typeof(Core), "IsUnknown");
            GlobalSymbols.DefineStaticOperator(ID.RaiseUnknown, typeof(Core), "RaiseUnknown");

            GlobalSymbols.DefineStaticOperator(ID.InstanceOf, typeof(Core), "InstanceOf");
            GlobalSymbols.DefineStaticOperator(ID.Castable, typeof(Core), "Castable");
            GlobalSymbols.DefineStaticOperator(ID.CastTo, typeof(Core), "CastTo");
            GlobalSymbols.DefineStaticOperator(ID.CastToItem, typeof(Core), "CastToItem");            
            GlobalSymbols.DefineStaticOperator(ID.TreatAs, typeof(Core), "TreatAs");
            GlobalSymbols.DefineStaticOperator(ID.CastArg, typeof(Core), "CastArg");

            GlobalSymbols.DefineStaticOperator(ID.GeneralEQ, typeof(Core), "GeneralEQ");
            GlobalSymbols.DefineStaticOperator(ID.GeneralNE, typeof(Core), "GeneralNE");
            GlobalSymbols.DefineStaticOperator(ID.GeneralGT, typeof(Core), "GeneralGT");
            GlobalSymbols.DefineStaticOperator(ID.GeneralGE, typeof(Core), "GeneralGE");
            GlobalSymbols.DefineStaticOperator(ID.GeneralLT, typeof(Core), "GeneralLT");
            GlobalSymbols.DefineStaticOperator(ID.GeneralLE, typeof(Core), "GeneralLE");

            GlobalSymbols.DefineStaticOperator(ID.Some, typeof(Core), "Some");
            GlobalSymbols.DefineStaticOperator(ID.Every, typeof(Core), "Every");

            GlobalSymbols.DefineStaticOperator(ID.SameNode, typeof(Core), "SameNode");
            GlobalSymbols.DefineStaticOperator(ID.PrecedingNode, typeof(Core), "PrecedingNode");
            GlobalSymbols.DefineStaticOperator(ID.FollowingNode, typeof(Core), "FollowingNode");

          
            GlobalSymbols.DefineStaticOperator(ID.Range, typeof(Core), "GetRange");
            GlobalSymbols.DefineStaticOperator(ID.Except, typeof(Core), "Except");
            GlobalSymbols.DefineStaticOperator(ID.Intersect, typeof(Core), "Intersect");
            GlobalSymbols.DefineStaticOperator(ID.Union, typeof(Core), "Union");            
            GlobalSymbols.DefineStaticOperator(ID.Validate, typeof(Core), "Validate");
            GlobalSymbols.DefineStaticOperator(ID.CastToNumber1, typeof(Core), "CastToNumber1");
            GlobalSymbols.DefineStaticOperator(ID.CastToNumber2, typeof(Core), "CastToNumber2");
            GlobalSymbols.DefineStaticOperator(ID.CastToNumber3, typeof(Core), "CastToNumber3");

            XQueryFunctionTable.Register(ID.Position, typeof(Core), "CurrentPosition");
            XQueryFunctionTable.Register(ID.Last, typeof(Core), "LastPosition");

            XQueryFunctionTable.Register(ID.Doc, typeof(Core), "GetDocument");
            XQueryFunctionTable.Register(ID.Root, typeof(Core), "GetRoot");
            XQueryFunctionTable.Register(ID.BooleanValue, typeof(Core), "BooleanValue");
            XQueryFunctionTable.Register(ID.True, typeof(Core), "True");
            XQueryFunctionTable.Register(ID.False, typeof(Core), "False");
            XQueryFunctionTable.Register(ID.Not, typeof(Core), "Not");
            XQueryFunctionTable.Register(ID.Number, typeof(Core), "Number");
            XQueryFunctionTable.Register(ID.String, typeof(Core), "StringValue");

            ValueProxy.AddFactory(
                new ValueProxyFactory[] { 
                    new DateTimeValue.ProxyFactory(),
                    new DateValue.ProxyFactory(),
                    new TimeValue.ProxyFactory(),
                    new DurationValue.ProxyFactory(),
                    new YearMonthDurationValue.ProxyFactory(),
                    new DayTimeDurationValue.ProxyFactory()
            });

            GlobalSymbols.Defmacro(ID.Atomize, "(x)", 
                @"(list 'let (list (list 'y (list 'atomize# x))) 
                    (list 'cond (list (list 'is-unknown 'y) (list 'trap 'unknown)) (list 't 'y)))");
            GlobalSymbols.Defmacro(ID.AtomizeX, "(x)",
                @"(list 'let (list (list 'y (list 'atomize# x))) 
                    (list 'cond (list (list 'is-unknown 'y) (list 'raise-unknown)) (list 't 'y)))");
            GlobalSymbols.Defmacro(ID.NodeValue, "(x)",
                @"(list 'cast (list 'let (list (list 'y (list 'node# x))) 
                    (list 'cond (list (list 'is-unknown 'y) (list 'trap 'unknown)) (list 't 'y))) node#type)");
            GlobalSymbols.Defmacro(ID.NodeValueX, "(x)",
                @"(list 'cast (list 'let (list (list 'y (list 'node# x))) 
                    (list 'cond (list (list 'is-unknown 'y) (list 'raise-unknown)) (list 't 'y))) node#type)");
            GlobalSymbols.Defmacro(ID.DynZeroOrOne, "(x)",
                @"(list 'let (list (list 'y x)) 
                    (list 'cond (list (list 'is-unknown 'y) (list 'trap 'unknown)) (list 't 'y)))");                        
            GlobalSymbols.Defmacro(ID.Par, "(x)", "x");
            GlobalSymbols.Defmacro(ID.ExactlyOne, "(x)", 
                "(list 'if (list 'is-unknown x) (list 'raise-unknown) x)");
        }        

        internal static void Init()
        {
        }

        public static bool IsUnknown(object value)
        {
            return value == Undefined.Value;
        }

        public static void RaiseUnknown()
        {
            throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", "item()");
        }

        private static QNameValue GetQualifiedName(object name, XmlNamespaceManager nsmgr, string defaultNamespace)
        {
            if (name is QNameValue)
                return (QNameValue)name;
            else if (name is String || name is UntypedAtomic)
                return QNameValue.Parse(name.ToString(), nsmgr, defaultNamespace);
            else
                throw new XQueryException(Properties.Resources.XPST0004,
                    "xs:string | xs:untypedAtomic | xs:QName");
        }

        private static XQueryDocumentBuilder GetBuilder(object builder)
        {
            return builder as XQueryDocumentBuilder;
        }

        [XQuerySignature("doc", Return=XmlTypeCode.Node, Cardinality=XmlTypeCardinality.ZeroOrOne)]
        public static object GetDocument([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.String, Cardinality=XmlTypeCardinality.ZeroOrOne)] object name)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            if (name == Undefined.Value)
                return Undefined.Value;
            string fileName;
            try
            {
                fileName = context.GetFileName((string)name);
                if (fileName == null)
                    throw new XQueryException(Properties.Resources.FileNotFound, name);
            }
            catch (ArgumentException ex)
            {
                throw new XQueryException(ex.Message, ex);
            }
            IXPathNavigable doc = context.OpenDocument(fileName);
            return doc.CreateNavigator();
        }

        public static XQueryNodeIterator CreateSequence(object value)
        {
            return XQueryNodeIterator.Create(value);
        }

        public static object DynExecuteExpr(object obj, IContextProvider provider, object[] args, MemoryPool pool)
        {
            XQueryExprBase expr = (XQueryExprBase)obj;
            return expr.Execute(provider, args, pool);
        }               

        public static XPathItem ContextNode(IContextProvider provider)
        {
            XPathItem item = provider.Context;
            if (item == null)
                throw new XQueryException(Properties.Resources.XPDY0002);
            return item;
        }

        public static int CurrentPosition(IContextProvider provider)
        {
            return provider.CurrentPosition;
        }

        public static int LastPosition(IContextProvider provider)
        {
            return provider.LastPosition;
        }

        public static object DynCreateDocument([Implict] Executive executive, object body)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteStartDocument();
            if (body != null)
                WriteNode(executive, builder, body);
            return doc.CreateNavigator();
        }

        public static object DynCreateElement([Implict] Executive executive, QNameValue name, object body)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteStartElement(name.Prefix, name.LocalName, name.NamespaceUri);
            if (body != null)
                WriteNode(executive, builder, body);
            builder.WriteEndElement();
            return doc.CreateNavigator();
        }

        public static object DynCreateElement([Implict] Executive executive, object name, XmlNamespaceManager nsmgr, object body)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            QNameValue qname = GetQualifiedName(name, nsmgr, nsmgr.DefaultNamespace);
            builder.WriteStartElement(qname.Prefix, qname.LocalName, qname.NamespaceUri);
            if (body != null)
                WriteNode(executive, builder, body);
            builder.WriteEndElement();
            return doc.CreateNavigator();
        }

        public static object DynCreateAttribute([Implict] Executive executive, QNameValue name, object value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            if (name.Prefix == "xmlns" || (name.Prefix == "" && name.LocalName == "xmlns"))
                throw new XQueryException(Properties.Resources.XQDY0044);
            builder.WriteStartElement("dummy");
            builder.WriteStartAttribute(name.Prefix, name.LocalName, name.NamespaceUri);
            string text = value == Undefined.Value ? "" : (string)value;
            builder.WriteString(text);
            builder.WriteEndAttribute();
            builder.WriteEndElement();
            XPathNavigator nav = doc.CreateNavigator();
            return new NodeIterator(XPathFactory.DynAttributeIterator(nav));
        }

        public static object DynCreateAttribute([Implict] Executive executive, object name, XmlNamespaceManager nsmgr, object value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            QNameValue qname = GetQualifiedName(name, nsmgr, "");
            if (qname.Prefix == "xmlns" || (qname.Prefix == "" && qname.LocalName == "xmlns"))
                throw new XQueryException(Properties.Resources.XQDY0044);
            builder.WriteStartElement("dummy");
            builder.WriteStartAttribute(qname.Prefix, qname.LocalName, qname.NamespaceUri);
            string text = value == Undefined.Value ? "" : (string)value;
            builder.WriteString(text);
            builder.WriteEndAttribute();
            builder.WriteEndElement();
            XPathNavigator nav = doc.CreateNavigator();
            return new NodeIterator(XPathFactory.DynAttributeIterator(nav));
        }

        public static object DynCreateCData([Implict] Executive executive, object value)
        {
            if (value == Undefined.Value)
                return Undefined.Value;
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteString((string)value);
            XPathFactory.XQueryDynNodeNavigator nav = new XPathFactory.XQueryDynNodeNavigator(doc);
            nav.MoveTo(doc.CreateNavigator());
            return nav;
        }

        public static object DynCreateText([Implict] Executive executive, object value)
        {
            if (value == Undefined.Value)
                return Undefined.Value;
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteString((string)value);
            XPathFactory.XQueryDynNodeNavigator nav = new XPathFactory.XQueryDynNodeNavigator(doc);
            nav.MoveTo(doc.CreateNavigator());
            return nav;
        }

        public static object DynCreateComment([Implict] Executive executive, object value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            string text = value == Undefined.Value ? "" : NormalizeStringValue((string)value, false, true);
            if (text.EndsWith("-") || text.Contains("--"))
                throw new XQueryException(Properties.Resources.XQDY0072);
            builder.WriteComment(text);
            XPathFactory.XQueryDynNodeNavigator nav = new XPathFactory.XQueryDynNodeNavigator(doc);
            nav.MoveTo(doc.CreateNavigator());
            return nav;
        }

        public static object DynCreatePi([Implict] Executive executive, object name, object value)
        {
            if (name == Undefined.Value)
                throw new XQueryException(Properties.Resources.XPTY0004, "()", "xs:string | xs:untypedAtomic");
            string ncname = (string)name;
            if (String.Compare(ncname, "xml", true) == 0)
                throw new XQueryException(Properties.Resources.XQDY0064);
            string text = value == Undefined.Value ? "" : NormalizeStringValue(value.ToString(), false, true).Trim();
            if (text.Contains("?>"))
                throw new XQueryException(Properties.Resources.XQDY0026);
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            try
            {
                builder.WriteProcessingInstruction(XmlConvert.VerifyNCName(ncname), text);
            }
            catch (XmlException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, name, "xs:NCName");
            }
            XPathFactory.XQueryDynNodeNavigator nav = new XPathFactory.XQueryDynNodeNavigator(doc);
            nav.MoveTo(doc.CreateNavigator());
            return nav;
        }

        public static object CreateBuilder([Implict] Executive executive)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.NamespaceInheritanceMode = context.NamespaceInheritanceMode;
            return builder;
        }

        public static object CreateNavigator(object o)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            return builder.Document.CreateNavigator();
        }

        public static object BeginElement([Implict] Executive executive, object o, QNameValue name)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteStartElement(name.Prefix, name.LocalName, name.NamespaceUri);
            return builder;
        }

        public static object EndElement(object o)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteEndElement();
            return builder;
        }

        public static object BeginAttribute([Implict] Executive executive, object o, QNameValue name)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteStartAttribute(name.Prefix, name.LocalName, name.NamespaceUri);
            return builder;
        }

        public static object EndAttribute(object o)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteEndAttribute();
            return builder;
        }

        public static object CreateComment(object o, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteComment(text);
            return builder;
        }

        public static object CreatePi(object o, string name, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteProcessingInstruction(name, text);
            return builder;
        }

        public static string NormalizeStringValue(string value, bool attr, bool raiseException)
        {
            StringBuilder sb = new StringBuilder(value);
            int i = 0;
            while (i < sb.Length)
            {
                switch (sb[i])
                {                    
                    case '\t':
                        if (attr)
                            sb[i] = ' ';
                        i++;                 
                        break;

                    case '\n':
                        if (i < sb.Length - 1 && sb[i + 1] == '\r')
                            sb.Remove(i + 1, 1);
                        if (attr)
                            sb[i] = ' ';
                        i++;
                        break;

                    case '\r':
                        if (i < sb.Length - 1 && sb[i + 1] == '\n')
                            sb.Remove(i + 1, 1);
                        if (attr)
                            sb[i] = ' ';
                        else
                            sb[i] = '\n';
                        i++;
                        break;

                    case '&':
                        bool process = false;
                        for (int j = i + 1; j < sb.Length; j++)
                            if (sb[j] == ';')
                            {
                                string entity = sb.ToString(i + 1, j - i - 1);
                                string entity_value = null;
                                if (entity.StartsWith("#"))
                                {
                                    int n;
                                    if (entity.StartsWith("#x"))
                                    {
                                        if (entity.Length > 2 && Int32.TryParse(entity.Substring(2, entity.Length - 2),
                                                System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out n))
                                          entity_value = Convert.ToString(Convert.ToChar(n));
                                     }
                                    else
                                    {
                                        if (entity.Length > 1 && Int32.TryParse(entity.Substring(1, entity.Length - 1), out n))
                                          entity_value = Convert.ToString(Convert.ToChar(n));
                                    }
                                }
                                else if (entity == "gt")
                                    entity_value = ">";
                                else if (entity == "lt")
                                    entity_value = "<";
                                else if (entity == "amp")
                                    entity_value = "&";
                                else if (entity == "quot")
                                    entity_value = "\"";
                                else if (entity == "apos")
                                    entity_value = "\'";
                                if (entity_value != null)
                                {
                                    sb.Remove(i, j - i + 1);
                                    sb.Insert(i, entity_value);
                                    i += entity_value.Length;
                                    process = true;
                                    break;
                                }
                                else
                                    if (raiseException)
                                        throw new XQueryException(Properties.Resources.XPST0003, String.Format("Entity reference '&{0};' was not recognized.", entity_value));
                            }
                        if (!process)
                        {
                            if (raiseException)
                                throw new XQueryException(Properties.Resources.XPST0003, "Entity reference '&' was not terminated by a semi-colon.");
                            i++;
                        }
                        break;

                    default:
                        i++;
                        break;
                }
            }
            return sb.ToString();
        }

        public static object WriteString(object o, object text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            if (text != Undefined.Value)
            {
                string value = NormalizeStringValue((string)text,
                    builder.WriteState == WriteState.Attribute, false);
                if (value != "")
                    builder.WriteString(value);
            }
            return builder;
        }

        public static object WriteWhitespace(object o, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            if (text != "")
                builder.WriteWhitespace(text);
            return builder;
        }

        public static object CreateCdata(object o, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            if (text != "")
                builder.WriteString(text);
            return builder;
        }

        public static object FormatValue(object value)
        {
            StringBuilder sb = new StringBuilder();
            if (value is XPathItem)
                sb.Append(((XPathItem)value).Value);
            else if (value is XQueryNodeIterator)
            {
                XQueryNodeIterator iter = (XQueryNodeIterator)value;
                bool begin = true;
                foreach (XPathItem item in iter)
                {
                    if (begin)
                        begin = false;
                    else
                        sb.Append(" ");
                    sb.Append(item.Value);
                }
                if (begin)
                    return Undefined.Value;
            }
            else
                sb.Append(XQueryConvert.ToString(value));
            return sb.ToString();
        }

        public static object WriteNode([Implict] Executive executive, object o, object node)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (node is XPathNavigator)
            {
                XPathNavigator nav = (XPathNavigator)node;
                if (nav.NodeType == XPathNodeType.Attribute)
                {
                    if (context.ConstructionMode == ElementConstructionMode.Preserve)
                        builder.SchemaInfo = nav.SchemaInfo;
                    builder.WriteStartAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                    builder.WriteString(nav.Value);
                    builder.WriteEndAttribute();
                    builder.SchemaInfo = null;
                }
                else
                {
                    if (nav.NodeType != XPathNodeType.Text || nav.Value != "")
                        builder.WriteNode(nav, context.NamespacePreserveMode, context.ConstructionMode);
                }
            }
            else if (node is XPathItem)
            {
                XPathItem item = (XPathItem)node;
                string value = item.Value;
                if (value != "")
                    builder.WriteString(value);
            }
            else if (node is XQueryNodeIterator)
            {
                XQueryNodeIterator iter = (XQueryNodeIterator)node;
                bool string_flag = false;
                foreach (XPathItem item in iter)
                {
                    XPathNavigator nav = item as XPathNavigator;
                    if (nav != null)
                    {
                        if (nav.NodeType == XPathNodeType.Attribute)
                        {
                            if (context.ConstructionMode == ElementConstructionMode.Preserve)
                                builder.SchemaInfo = nav.SchemaInfo;
                            builder.WriteStartAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                            builder.WriteString(nav.Value);
                            builder.WriteEndAttribute();
                            builder.SchemaInfo = null;
                        }
                        else
                        {
                            if (nav.NodeType == XPathNodeType.Text && nav.Value == "")
                                continue;
                            builder.WriteNode(nav, context.NamespacePreserveMode, context.ConstructionMode);
                        }
                        string_flag = false;
                    }
                    else
                    {
                        if (string_flag)
                            builder.WriteString(" ");
                        builder.WriteString(item.Value);
                        string_flag = true;
                    }
                }
            }
            else
                if (node != Undefined.Value)
                {
                    string value = XQueryConvert.ToString(node); // !!!
                    if (value != "")
                        builder.WriteString(value);
                }
            return builder;
        }

        public static bool BooleanValue([XQueryParameter(XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)] object value)
        {
            if (value == null ||
                value == DataEngine.CoreServices.Generation.RuntimeOps.False ||
                value == Undefined.Value)
                return false;
            XPathItem item;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                if (!iter.MoveNext())
                    return false;
                item = iter.Current.Clone();
                if (item.IsNode)
                    return true;
                if (iter.MoveNext())
                    throw new XQueryException(Properties.Resources.FORG0006, "fn:boolean()",
                        new XQuerySequenceType(XmlTypeCode.AnyAtomicType, XmlTypeCardinality.OneOrMore));
            }
            else
                item = value as XPathItem;
            if (item != null)
                switch (item.XmlType.TypeCode)
                {
                    case XmlTypeCode.Boolean:
                        return item.ValueAsBoolean;

                    case XmlTypeCode.String:
                    case XmlTypeCode.AnyUri:
                    case XmlTypeCode.UntypedAtomic:
                        return item.Value != String.Empty;

                    case XmlTypeCode.Float:
                    case XmlTypeCode.Double:
                        return !Double.IsNaN(item.ValueAsDouble) && item.ValueAsDouble != 0.0;

                    case XmlTypeCode.Decimal:
                    case XmlTypeCode.Integer:
                    case XmlTypeCode.NonPositiveInteger:
                    case XmlTypeCode.NegativeInteger:
                    case XmlTypeCode.Long:
                    case XmlTypeCode.Int:
                    case XmlTypeCode.Short:
                    case XmlTypeCode.Byte:
                    case XmlTypeCode.UnsignedInt:
                    case XmlTypeCode.UnsignedShort:
                    case XmlTypeCode.UnsignedByte:
                    case XmlTypeCode.NonNegativeInteger:
                    case XmlTypeCode.UnsignedLong:
                    case XmlTypeCode.PositiveInteger:
                        return (decimal)(item.ValueAs(typeof(Decimal))) != 0;

                    default:
                        throw new XQueryException(Properties.Resources.FORG0006, "fn:boolean()",
                            new XQuerySequenceType(item.XmlType.TypeCode, XmlTypeCardinality.One));
                }

            else
            {
                TypeCode typeCode;
                IConvertible conv = value as IConvertible;
                if (conv != null)
                    typeCode = conv.GetTypeCode();
                else
                    typeCode = Type.GetTypeCode(value.GetType());
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return Convert.ToBoolean(value, CultureInfo.InvariantCulture);

                    case TypeCode.String:
                        return Convert.ToString(value, CultureInfo.InvariantCulture) != String.Empty;

                    case TypeCode.Single:
                    case TypeCode.Double:
                        return Convert.ToDouble(value, CultureInfo.InvariantCulture) != 0.0 &&
                            !Double.IsNaN(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                    default:
                        {
                            if (value is AnyUriValue || value is UntypedAtomic)
                                return value.ToString() != String.Empty;
                            if (ValueProxy.IsNumeric(value.GetType()))
                                return Convert.ToDecimal(value) != 0;
                            throw new XQueryException(Properties.Resources.FORG0006, "fn:boolean()",
                                new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One));
                        }
                }
            }
        }

        public static object Atomize(object value)
        {
            XPathItem item = value as XPathItem;
            if (item != null)
                return item.TypedValue;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                iter = iter.Clone();
                if (!iter.MoveNext())
                    return Undefined.Value;
                object res = iter.Current.TypedValue;
                if (iter.MoveNext())
                    throw new XQueryException(Properties.Resources.MoreThanOneItem);
                return res;
            }
            return value;
        }

        public static object NodeValue(object value)
        {
            if (value == Undefined.Value)
                return Undefined.Value;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                iter = iter.Clone();
                if (!iter.MoveNext())
                    return Undefined.Value;
                XPathItem res = iter.Current.Clone();
                if (iter.MoveNext())
                    throw new XQueryException(Properties.Resources.MoreThanOneItem);
                XPathNavigator nav = res as XPathNavigator;
                if (nav == null)
                    throw new XQueryException(Properties.Resources.XPST0004, "node()");
                return nav;
            }
            else
            {
                XPathNavigator nav = value as XPathNavigator;
                if (nav == null)
                    throw new XQueryException(Properties.Resources.XPST0004, "node()");
                return nav.Clone();
            }
        }

        public static bool Some([Implict] Executive executive, object expr)
        {
            XQueryNodeIterator iter = expr as XQueryNodeIterator;
            if (iter != null)
            {
                while (iter.MoveNext())
                    if (iter.Current.ValueAsBoolean)
                        return true;
            }
            return false;
        }

        public static bool Every([Implict] Executive executive, object expr)
        {
            XQueryNodeIterator iter = expr as XQueryNodeIterator;
            if (iter != null)
            {
                while (iter.MoveNext())
                    if (!iter.Current.ValueAsBoolean)
                        return false;
            }
            return true;            
        }

        public static object CastTo([Implict] Executive engine, object value, XQuerySequenceType destType, Type exprType)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (destType == XQuerySequenceType.Item)
                return value;
            if (value == Undefined.Value)
            {
                if (destType.Cardinality == XmlTypeCardinality.ZeroOrMore)
                    return EmptyIterator.Shared;
                if (destType.TypeCode != XmlTypeCode.None && destType.Cardinality != XmlTypeCardinality.ZeroOrOne)
                    throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", destType);
                return Undefined.Value;
            }
            if (destType.Cardinality == XmlTypeCardinality.One ||
                destType.Cardinality == XmlTypeCardinality.ZeroOrOne)
            {
                XPathItem res;
                XQueryNodeIterator iter = value as XQueryNodeIterator;
                if (iter != null)
                {
                    iter = iter.Clone();
                    if (!iter.MoveNext())
                    {
                        if (destType.TypeCode != XmlTypeCode.None && 
                            (destType.Cardinality == XmlTypeCardinality.One || destType.Cardinality == XmlTypeCardinality.OneOrMore))
                            throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", destType);
                        return Undefined.Value;
                    }
                    if (exprType != null && exprType != typeof(System.String))
                    {
                        if ((destType.TypeCode == XmlTypeCode.QName && iter.Current.XmlType.TypeCode != XmlTypeCode.QName) ||
                            (destType.TypeCode == XmlTypeCode.Notation && iter.Current.XmlType.TypeCode != XmlTypeCode.Notation))
                            throw new XQueryException(Properties.Resources.XPTY0004_CAST, destType);
                    }
                    res = iter.Current.ChangeType(destType, context);
                    if (iter.MoveNext())
                        throw new XQueryException(Properties.Resources.MoreThanOneItem);
                    if (destType.IsNode)
                        return res;                    
                    return res.TypedValue;
                }
                XPathItem item = value as XPathItem;
                if (item == null)
                    item = new XQueryItem(value);
                if (exprType != null && exprType != typeof(System.String))
                {
                    if ((destType.TypeCode == XmlTypeCode.QName && item.XmlType.TypeCode != XmlTypeCode.QName) ||
                        (destType.TypeCode == XmlTypeCode.Notation && item.XmlType.TypeCode != XmlTypeCode.Notation))
                        throw new XQueryException(Properties.Resources.XPTY0004_CAST, destType);
                }
                res = item.ChangeType(destType, context);
                if (destType.IsNode)
                    return res;
                return res.TypedValue;
            }
            else
                return new NodeIterator(XPathFactory.ConvertIterator(XQueryNodeIterator.Create(value), destType, context));
        }

        public static object CastArg([Implict] Executive engine, object value, XQuerySequenceType destType)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (destType == XQuerySequenceType.Item)
                return value;
            if (value == Undefined.Value)
            {
                if (destType.Cardinality == XmlTypeCardinality.ZeroOrMore)
                    return EmptyIterator.Shared;
                if (destType.TypeCode != XmlTypeCode.None && destType.Cardinality != XmlTypeCardinality.ZeroOrOne)
                    throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", destType);
                return Undefined.Value;
            }
            if (destType.Cardinality == XmlTypeCardinality.One ||
                destType.Cardinality == XmlTypeCardinality.ZeroOrOne)
            {
                object res;
                XQueryNodeIterator iter = value as XQueryNodeIterator;
                if (iter != null)
                {
                    iter = iter.Clone();
                    if (!iter.MoveNext())
                    {
                        if (destType.TypeCode != XmlTypeCode.None &&
                            (destType.Cardinality == XmlTypeCardinality.One || destType.Cardinality == XmlTypeCardinality.OneOrMore))
                            throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", destType);
                        return Undefined.Value;
                    }
                    if (destType.IsNode)
                    {
                        if (!destType.Match(iter.Current, context))
                            throw new XQueryException(Properties.Resources.XPTY0004,
                                new XQuerySequenceType(iter.Current.XmlType, XmlTypeCardinality.OneOrMore, null), destType);
                        res = iter.Current.Clone();
                    }
                    else
                        res = XQueryConvert.ValueAs(iter.Current.TypedValue, destType, context.nameTable, context.NamespaceManager);
                    if (iter.MoveNext())
                        throw new XQueryException(Properties.Resources.MoreThanOneItem);
                    return res;
                }
                else
                {
                    XPathItem item = value as XPathItem;
                    if (item != null)
                    {
                        if (item.IsNode)
                        {
                            if (!destType.Match(item, context))
                                throw new XQueryException(Properties.Resources.XPTY0004,
                                    new XQuerySequenceType(item.XmlType, XmlTypeCardinality.OneOrMore, null), destType);
                            return item;
                        }
                        else
                            return XQueryConvert.ValueAs(item.TypedValue, destType,
                                context.nameTable, context.NamespaceManager);
                    }
                    return XQueryConvert.ValueAs(value, destType, context.nameTable, context.NamespaceManager);
                }
            }
            else
                return new NodeIterator(XPathFactory.ValueIterator(XQueryNodeIterator.Create(value), destType, context));
        }

        public static object TreatAs([Implict] Executive engine, object value, XQuerySequenceType destType)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (destType == XQuerySequenceType.Item)
                return value;
            if (value == Undefined.Value)
            {
                if (destType.Cardinality == XmlTypeCardinality.ZeroOrMore)
                    return EmptyIterator.Shared;
                if (destType.TypeCode != XmlTypeCode.None && destType.Cardinality != XmlTypeCardinality.ZeroOrOne)
                    throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", destType);
                return Undefined.Value;
            }            
            if (destType.Cardinality == XmlTypeCardinality.One ||
                destType.Cardinality == XmlTypeCardinality.ZeroOrOne)
            {
                object res;
                XQueryNodeIterator iter = value as XQueryNodeIterator;
                if (iter != null)
                {
                    iter = iter.Clone();
                    if (!iter.MoveNext())
                    {
                        if (destType.TypeCode != XmlTypeCode.None &&
                            (destType.Cardinality == XmlTypeCardinality.One || destType.Cardinality == XmlTypeCardinality.OneOrMore))
                            throw new XQueryException(Properties.Resources.XPTY0004, "empty-sequence()", destType);
                        return Undefined.Value;
                    }
                    if (destType.TypeCode == XmlTypeCode.None)
                        throw new XQueryException(Properties.Resources.XPTY0004, 
                            new XQuerySequenceType(iter.Current.XmlType, XmlTypeCardinality.OneOrMore, null), "empty-sequence()");
                    if (destType.IsNode)
                    {
                        if (!destType.Match(iter.Current, context))
                            throw new XQueryException(Properties.Resources.XPTY0004,
                                new XQuerySequenceType(iter.Current.XmlType, XmlTypeCardinality.OneOrMore, null), destType);
                        res = iter.Current.Clone();
                    }
                    else
                        res = XQueryConvert.TreatValueAs(iter.Current.TypedValue, destType);
                    if (iter.MoveNext())
                        throw new XQueryException(Properties.Resources.MoreThanOneItem);
                    return res;
                }
                else
                {
                    XPathItem item = value as XPathItem;
                    if (item != null)
                    {
                        if (item.IsNode)
                        {
                            if (!destType.Match(item, context))
                                throw new XQueryException(Properties.Resources.XPTY0004,
                                    new XQuerySequenceType(item.XmlType, XmlTypeCardinality.OneOrMore, null), destType);
                            return item;
                        }
                        else
                            return XQueryConvert.TreatValueAs(item.TypedValue, destType);
                    }
                    return XQueryConvert.TreatValueAs(value, destType); 
                }
            }
            else
                return new NodeIterator(XPathFactory.TreatIterator(
                    XQueryNodeIterator.Create(value), destType, context));
        }

        public static object CastToItem([Implict] Executive executive, 
            object value, XQuerySequenceType destType)
        {            
            XQueryContext context = (XQueryContext)executive.Owner;
            if (value == null)
                value = CoreServices.Generation.RuntimeOps.False;
            else
            {
                value = Atomize(value);
                if (value == Undefined.Value)
                {
                    if (destType.TypeCode == XmlTypeCode.String)
                        return String.Empty;
                    return value;
                }
            }
            XmlTypeCode typeCode = XQuerySequenceType.GetXmlTypeCode(value.GetType());
            XmlSchemaType xmlType = XmlSchemaSimpleType.GetBuiltInSimpleType(typeCode);
            return XQueryConvert.ChangeType(xmlType, value, 
                destType, context.nameTable, context.NamespaceManager);
        }

        public static bool InstanceOf([Implict] Executive engine, object value, XQuerySequenceType destType)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (value == Undefined.Value)
                return destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                    destType.Cardinality == XmlTypeCardinality.ZeroOrMore;
            if (value == null)
                value = CoreServices.Generation.RuntimeOps.False;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                int num = 0;
                foreach (XPathItem item in iter)
                {
                    if (num == 1)
                    {
                        if (destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                            destType.Cardinality == XmlTypeCardinality.One)
                            return false;
                    }
                    if (!destType.Match(item, context))
                        return false;
                    num++;
                }
                if (num == 0)
                {
                    if (destType.TypeCode != XmlTypeCode.None && (destType.Cardinality == XmlTypeCardinality.One ||
                         destType.Cardinality == XmlTypeCardinality.OneOrMore))
                        return false;
                }
                return true;
            }
            else                
            {
                if (destType.ItemType == value.GetType())
                    return true;
                XPathItem item = value as XPathItem;
                if (item == null)
                    item = new XQueryItem(value);
                return destType.Match(item, context);
            }
        }

        public static bool Castable([Implict] Executive engine, object value, XQuerySequenceType destType, Type exprType)
        {
            try
            {
                CastTo(engine, value, destType, exprType);
                return true;
            }
            catch(XQueryException)
            {
                return false;
            }
        }

        public static bool SameNode(object a, object b)
        {
            XPathNavigator nav1 = (XPathNavigator)a;
            XPathNavigator nav2 = (XPathNavigator)b;
            return nav1.ComparePosition(nav2) == XmlNodeOrder.Same;
        }

        public static bool PrecedingNode(object a, object b)
        {
            XPathNavigator nav1 = (XPathNavigator)a;
            XPathNavigator nav2 = (XPathNavigator)b;
            XPathComparer comp = new XPathComparer();
            return comp.Compare(nav1, nav2) == -1;
        }

        public static bool FollowingNode(object a, object b)
        {
            XPathNavigator nav1 = (XPathNavigator)a;
            XPathNavigator nav2 = (XPathNavigator)b;
            XPathComparer comp = new XPathComparer();
            return comp.Compare(nav1, nav2) == 1;
        }

        private static void MagnitudeRelationship(XQueryContext context, XPathItem item1, XPathItem item2, 
            out object x, out object y)
        {
            x = item1.TypedValue;
            y = item2.TypedValue;
            if (x is UntypedAtomic) 
            {
                if (ValueProxy.IsNumeric(y.GetType()))
                    x = Convert.ToDouble(x, CultureInfo.InvariantCulture);
                else
                    if (y is String)
                        x = x.ToString();
                    else if (!(y is UntypedAtomic))
                        x = item1.ChangeType(new XQuerySequenceType(item2.XmlType.TypeCode), context).TypedValue;
            }
            if (y is UntypedAtomic)
            {
                if (ValueProxy.IsNumeric(x.GetType()))
                    y = Convert.ToDouble(y, CultureInfo.InvariantCulture);
                else
                    if (x is String)
                        y = y.ToString();
                    else if (!(x is UntypedAtomic))
                        y = item2.ChangeType(new XQuerySequenceType(item1.XmlType.TypeCode), context).TypedValue;
            }
        }

        public static bool GeneralEQ([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            while (iter1.MoveNext())
            {
                XQueryNodeIterator iter = iter2.Clone();
                while (iter.MoveNext())
                {
                    object x;
                    object y;
                    MagnitudeRelationship(context, iter1.Current, iter.Current, out x, out y);
                    if (executive.OperatorEq(x, y) != null)
                        return true;
                }
            }
            return false;
        }

        public static bool GeneralGT([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            while (iter1.MoveNext())
            {
                XQueryNodeIterator iter = iter2.Clone();
                while (iter.MoveNext())
                {
                    object x;
                    object y;
                    MagnitudeRelationship(context, iter1.Current, iter.Current, out x, out y);
                    if (executive.OperatorGt(x, y) != null)
                        return true;
                }
            }
            return false;
        }

        public static bool GeneralNE([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            while (iter1.MoveNext())
            {
                XQueryNodeIterator iter = iter2.Clone();
                while (iter.MoveNext())
                {
                    object x;
                    object y;
                    MagnitudeRelationship(context, iter1.Current, iter.Current, out x, out y);
                    if (executive.OperatorEq(x, y) == null)
                        return true;
                }
            }
            return false;
        }

        public static bool GeneralGE([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            while (iter1.MoveNext())
            {
                XQueryNodeIterator iter = iter2.Clone();
                while (iter.MoveNext())
                {
                    object x;
                    object y;
                    MagnitudeRelationship(context, iter1.Current, iter.Current, out x, out y);
                    if (executive.OperatorEq(x, y) != null || executive.OperatorGt(x, y) != null)
                        return true;
                }
            }
            return false;
        }

        public static bool GeneralLT([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            while (iter1.MoveNext())
            {
                XQueryNodeIterator iter = iter2.Clone();
                while (iter.MoveNext())
                {
                    object x;
                    object y;
                    MagnitudeRelationship(context, iter1.Current, iter.Current, out x, out y);
                    if (executive.OperatorGt(y, x) != null)
                        return true;
                }
            }
            return false;
        }

        public static bool GeneralLE([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            while (iter1.MoveNext())
            {
                XQueryNodeIterator iter = iter2.Clone();
                while (iter.MoveNext())
                {
                    object x;
                    object y;
                    MagnitudeRelationship(context, iter1.Current, iter.Current, out x, out y);
                    if (executive.OperatorEq(x, y) != null || executive.OperatorGt(y, x) != null)
                        return true;
                }
            }
            return false;
        }

        public static XQueryNodeIterator GetRange(object arg1, object arg2)
        {
            object lo = Atomize(arg1);
            if (lo == Undefined.Value)
                return EmptyIterator.Shared;
            if (lo is UntypedAtomic)
            {
                int i;
                if (!Int32.TryParse(lo.ToString(), out i))
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(lo.GetType(), XmlTypeCardinality.One), "xs:integer in first argument op:range");
                lo = i;
            }
            object high = Atomize(arg2);
            if (high == Undefined.Value)
                return EmptyIterator.Shared;
            if (high is UntypedAtomic)
            {
                int i;
                if (!Int32.TryParse(high.ToString(), out i))
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(lo.GetType(), XmlTypeCardinality.One), "xs:integer in second argument op:range");
                high = i;
            }
            if (!Integer.IsDerivedSubtype(lo))
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(lo.GetType(), XmlTypeCardinality.One), "xs:integer in first argument op:range");
            if (!Integer.IsDerivedSubtype(high))
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(high.GetType(), XmlTypeCardinality.One), "xs:integer in second argument op:range");
            return new RangeIterator(Convert.ToInt32(lo), Convert.ToInt32(high));
        }

        public static XQueryNodeIterator Union([Implict] Executive executive, bool isOrdered, object a, object b)
        {
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (isOrdered)
                return new NodeIterator(XPathFactory.UnionIterator1(iter1, iter2));
            else
                return new NodeIterator(XPathFactory.UnionIterator2(iter1, iter2));
        }

        public static XQueryNodeIterator Except([Implict] Executive executive, bool isOrdered, object a, object b)
        {
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (isOrdered)
                return new NodeIterator(XPathFactory.IntersectExceptIterator1(true, iter1, iter2));
            else
                return new NodeIterator(XPathFactory.IntersectExceptIterator2(true, iter1, iter2));
        }

        public static XQueryNodeIterator Intersect([Implict] Executive executive, bool isOrdered, object a, object b)
        {
            XQueryNodeIterator iter1 = XQueryNodeIterator.Create(a);
            XQueryNodeIterator iter2 = XQueryNodeIterator.Create(b);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (isOrdered)
                return new NodeIterator(XPathFactory.IntersectExceptIterator1(false, iter1, iter2));
            else
                return new NodeIterator(XPathFactory.IntersectExceptIterator2(false, iter1, iter2));
        }

        [XQuerySignature("root", Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetRoot(IContextProvider provider)
        {
            return GetRoot(NodeValue(ContextNode(provider)));
        }

        [XQuerySignature("root", Return = XmlTypeCode.Node, Cardinality = XmlTypeCardinality.ZeroOrOne)]
        public static object GetRoot([XQueryParameter(XmlTypeCode.Node,
            Cardinality = XmlTypeCardinality.ZeroOrOne)] object node)
        {
            if (node == Undefined.Value)
                return node;
            XPathNavigator nav = node as XPathNavigator;
            if (nav == null)
                throw new XQueryException(Properties.Resources.XPTY0004,
                    new XQuerySequenceType(node.GetType(), XmlTypeCardinality.ZeroOrOne), "node()? in fn:root()");
            XPathNavigator curr = nav.Clone();
            curr.MoveToRoot();
            return curr;
        }

        public static bool True()
        {
            return true;
        }

        public static bool False()
        {
            return false;
        }

        public static bool Not([XQueryParameter(XmlTypeCode.Item, Cardinality = XmlTypeCardinality.ZeroOrMore)] object value)
        {
            return !BooleanValue(value);
        }

        public static double Number([Implict] Executive engine, IContextProvider provider)
        {
            return Number(engine, Core.Atomize(Core.ContextNode(provider)));
        }

        public static double Number([Implict] Executive engine, 
            [XQueryParameter(XmlTypeCode.AnyAtomicType, Cardinality=XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value || !(value is IConvertible))
                return Double.NaN;
            XQueryContext context = (XQueryContext)engine.Owner;
            try
            {
                return (double)Convert.ChangeType(value, TypeCode.Double, context.DefaultCulture);
            }
            catch (FormatException)
            {
                return Double.NaN;
            }
            catch (InvalidCastException)
            {
                return Double.NaN;
            }
        }

        public static object CastToNumber1([Implict] Executive engine, object value)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            try
            {
                if (value is UntypedAtomic)
                    return Convert.ToDouble(value, context.DefaultCulture);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, value, "xs:double?");
            }
            catch (InvalidCastException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, value, "xs:double?");
            }
            return value;
        }

        public static double CastToNumber2([Implict] Executive engine, object value)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            try
            {
                if (!(value is UntypedAtomic))
                    throw new XQueryException(Properties.Resources.XPTY0004,
                        new XQuerySequenceType(value.GetType(), XmlTypeCardinality.One), "xs:untypedAtomic?");
                return Convert.ToDouble(value, context.DefaultCulture);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, value, "xs:double?");
            }
            catch (InvalidCastException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, value, "xs:double?");
            }
        }

        public static double CastToNumber3([Implict] Executive engine, object value)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            try
            {
                return Convert.ToDouble(value, context.DefaultCulture);
            }
            catch (FormatException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, value, "xs:double?");
            }
            catch (InvalidCastException)
            {
                throw new XQueryException(Properties.Resources.FORG0001, value, "xs:double?");
            }
        }

        public static string StringValue([Implict] Executive executive, IContextProvider provider)
        {
            return StringValue(executive, ContextNode(provider));
        }

        public static string StringValue([Implict] Executive executive, 
            [XQueryParameter(XmlTypeCode.Item, Cardinality=XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == Undefined.Value)
                return "";
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                iter = iter.Clone();
                if (!iter.MoveNext())
                    return "";
                string res = iter.Current.Value;
                if (iter.MoveNext())
                    throw new XQueryException(Properties.Resources.MoreThanOneItem);
                return res;
            }
            XPathItem item = value as XPathItem;
            if (item != null)
                return item.Value;
            return XQueryConvert.ToString(value);
        }

        public static XQueryNodeIterator Validate([Implict] Executive executive, object val, bool lax)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            return new NodeIterator(XPathFactory.ValidateIterator(
                XQueryNodeIterator.Create(val), context.schemaSet, lax));
        }
    }
}
