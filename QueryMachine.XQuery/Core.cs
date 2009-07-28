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
using System.Diagnostics;
using System.Globalization;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;

namespace DataEngine.XQuery
{
    public partial class ID
    {
        public static readonly object DynExecuteExpr = Lisp.Defatom("dyn_execute");
        public static readonly object DynOrdering = Lisp.Defatom("dyn_ordering");
        
        public static readonly object Doc = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "doc" }, true);
        public static readonly object Root = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "root" }, true);
        public static readonly object Position = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "position" }, true);
        public static readonly object Last = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "last" }, true);

        public static readonly object BooleanValue = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "boolean" }, true);
        public static readonly object True = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "true" }, true);
        public static readonly object False = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "false" }, true);
        public static readonly object Not = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "not" }, true);

        public static readonly object String = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "string" }, true);
        public static readonly object Number = Lisp.Defatom(XmlReservedNs.NsXQueryFunc, new string[] { "number" }, true);

        public static readonly object DynCreateDocument = Lisp.Defatom("dyn_root");
        public static readonly object DynCreateElement = Lisp.Defatom("dyn_element");
        public static readonly object DynCreateAttribute = Lisp.Defatom("dyn_attribute");
        public static readonly object DynCreateCData = Lisp.Defatom("dyn_cdata");
        public static readonly object DynCreateText = Lisp.Defatom("dyn_text");
        public static readonly object DynCreateComment = Lisp.Defatom("dyn_comment");
        public static readonly object DynCreatePi = Lisp.Defatom("dyn_pi");
        public static readonly object DynZeroOrOne = Lisp.Defatom("dyn_zero_or_one");
        
        public static readonly object CreateBuilder = Lisp.Defatom("create-builder");
        public static readonly object CreateNavigator = Lisp.Defatom("create-navigator");
        public static readonly object WriteBeginElement = Lisp.Defatom("begin-element");
        public static readonly object WriteEndElement = Lisp.Defatom("end-element");
        public static readonly object WriteBeginAttribute = Lisp.Defatom("begin-attribute");
        public static readonly object WriteEndAttribute = Lisp.Defatom("end-attribute");
        public static readonly object WriteRaw = Lisp.Defatom("write-raw");
        public static readonly object WriteNode = Lisp.Defatom("write-node");
        public static readonly object WriteComment = Lisp.Defatom("write-comment");
        public static readonly object WritePi = Lisp.Defatom("write-pi");
        public static readonly object WriteString = Lisp.Defatom("write-string");
        public static readonly object WriteWhitespace = Lisp.Defatom("write-ws");
        public static readonly object WriteEntityRef = Lisp.Defatom("write-entityref");
        public static readonly object WriteCdata = Lisp.Defatom("write-cdata");
                
        public static readonly object Atomize = Lisp.Defatom("atomize");
        public static readonly object AtomizeBody = Lisp.Defatom("atomize#");
        public static readonly object NodeValue = Lisp.Defatom("node");
        public static readonly object DateTimeValue = Lisp.Defatom("dateTime");
        public static readonly object NodeValueBody = Lisp.Defatom("node#");        
        public static readonly object FormatValue = Lisp.Defatom("format-value");

        public static readonly object InstanceOf = Lisp.Defatom("instance-of");
        public static readonly object CastTo = Lisp.Defatom("cast-to");
        public static readonly object CastToItem = Lisp.Defatom("cast-to-item");
        public static readonly object Castable = Lisp.Defatom("castable");
        public static readonly object TreatAs = Lisp.Defatom("treat-as");

        public static readonly object GeneralEQ = Lisp.Defatom("general-eq");
        public static readonly object GeneralNE = Lisp.Defatom("general-ne");
        public static readonly object GeneralLT = Lisp.Defatom("general-lt");
        public static readonly object GeneralGT = Lisp.Defatom("general-gt");
        public static readonly object GeneralGE = Lisp.Defatom("general-ge");
        public static readonly object GeneralLE = Lisp.Defatom("general-le");

        public static readonly object Some = Lisp.Defatom("some");
        public static readonly object Every = Lisp.Defatom("every");

        public static readonly object SameNode = Lisp.Defatom("is-same-node");
        public static readonly object PrecedingNode = Lisp.Defatom("is-preceding-node");
        public static readonly object FollowingNode = Lisp.Defatom("is-following-node");

        public static readonly object Div = Lisp.Defatom("div");
        public static readonly object IDiv = Lisp.Defatom("idiv");
        public static readonly object Mod = Lisp.Defatom("mod");

        public static readonly object Range = Lisp.Defatom("range");
        public static readonly object Except = Lisp.Defatom("except");
        public static readonly object Intersect = Lisp.Defatom("intersect");
        public static readonly object Union = Lisp.Defatom("union");

        public static readonly object Context = Lisp.Defatom("$context");
        public static readonly object Seq = Lisp.Defatom("$seq");
        public static readonly object CheckIsNode = Lisp.Defatom("check-is-node");        
        
        public static readonly object Child = Lisp.Defatom("child");
        public static readonly object Descendant = Lisp.Defatom("descendant");
        public static readonly object Attribute = Lisp.Defatom("attribute");
        public static readonly object Namespace = Lisp.Defatom("namespace");
        public static readonly object Self = Lisp.Defatom("self");
        public static readonly object DescendantOrSelf = Lisp.Defatom("descendant-or-self");
        public static readonly object FollowingSibling = Lisp.Defatom("following-sibling");
        public static readonly object Following = Lisp.Defatom("following");
        public static readonly object Parent = Lisp.Defatom("parent");
        public static readonly object Ancestor = Lisp.Defatom("ancestor");
        public static readonly object PrecedingSibling = Lisp.Defatom("preceding-sibling");
        public static readonly object Preceding = Lisp.Defatom("preceding");
        public static readonly object AncestorOrSelf = Lisp.Defatom("ancestor-or-self");

        public static readonly object NameTest = Lisp.Defatom("test");
        public static readonly object TypeTest = Lisp.Defatom("test-type");
        public static readonly object Par = Lisp.Defatom("par");
    }    

    public static class Core
    {        
        static Core()
        {
            GlobalSymbols.DefineStaticOperator(ID.CreateNavigator, typeof(Core), "CreateNavigator");
            GlobalSymbols.DefineStaticOperator(ID.DynExecuteExpr, typeof(Core), "DynExecuteExpr");
            GlobalSymbols.DefineStaticOperator(ID.DynOrdering, typeof(Core), "DynOrdering");
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
            GlobalSymbols.DefineStaticOperator(ID.DateTimeValue, typeof(Core), "DateTimeValue");
            GlobalSymbols.DefineStaticOperator(ID.Context, typeof(Core), "Context");
            GlobalSymbols.DefineStaticOperator(ID.Seq, typeof(Core), "CreateSequence");
            GlobalSymbols.DefineStaticOperator(ID.CheckIsNode, typeof(Core), "CheckIsNode");

            GlobalSymbols.DefineStaticOperator(ID.InstanceOf, typeof(Core), "InstanceOf");
            GlobalSymbols.DefineStaticOperator(ID.Castable, typeof(Core), "Castable");
            GlobalSymbols.DefineStaticOperator(ID.CastTo, typeof(Core), "CastTo");
            GlobalSymbols.DefineStaticOperator(ID.CastToItem, typeof(Core), "CastToItem");            

            GlobalSymbols.DefineStaticOperator(ID.GeneralEQ, typeof(Core), "GeneralEQ");
            GlobalSymbols.DefineStaticOperator(ID.GeneralNE, typeof(Core), "GeneralNE");
            GlobalSymbols.DefineStaticOperator(ID.GeneralGT, typeof(Core), "GeneralGT");
            GlobalSymbols.DefineStaticOperator(ID.GeneralGE, typeof(Core), "GeneralGE");
            GlobalSymbols.DefineStaticOperator(ID.GeneralLT, typeof(Core), "GeneralLT");
            GlobalSymbols.DefineStaticOperator(ID.GeneralLE, typeof(Core), "GeneralLE");

            GlobalSymbols.DefineStaticOperator(ID.Some, typeof(Core), "Some");
            GlobalSymbols.DefineStaticOperator(ID.Every, typeof(Core), "Every");

            GlobalSymbols.DefineStaticOperator(ID.Mod, typeof(Core), "Mod");
            GlobalSymbols.DefineStaticOperator(ID.Div, typeof(Core), "Div");
            GlobalSymbols.DefineStaticOperator(ID.IDiv, typeof(Core), "IDiv");

            GlobalSymbols.DefineStaticOperator(ID.SameNode, typeof(Core), "SameNode");
            GlobalSymbols.DefineStaticOperator(ID.PrecedingNode, typeof(Core), "PrecedingNode");
            GlobalSymbols.DefineStaticOperator(ID.FollowingNode, typeof(Core), "FollowingNode");

            GlobalSymbols.DefineStaticOperator(ID.NameTest, typeof(Core), "NameTest");
            GlobalSymbols.DefineStaticOperator(ID.TypeTest, typeof(Core), "TypeTest");           
            
            GlobalSymbols.DefineStaticOperator(ID.Range, typeof(Core), "GetRange");
            GlobalSymbols.DefineStaticOperator(ID.Except, typeof(Core), "Except");
            GlobalSymbols.DefineStaticOperator(ID.Intersect, typeof(Core), "Intersect");
            GlobalSymbols.DefineStaticOperator(ID.Union, typeof(Core), "Union");
            
            GlobalSymbols.DefineStaticOperator(ID.Child, typeof(Core), "AxisChild");
            GlobalSymbols.DefineStaticOperator(ID.Self, typeof(Core), "AxisSelf");
            GlobalSymbols.DefineStaticOperator(ID.Attribute, typeof(Core), "AxisAttribute");
            GlobalSymbols.DefineStaticOperator(ID.Namespace, typeof(Core), "AxisNamespace");
            GlobalSymbols.DefineStaticOperator(ID.Ancestor, typeof(Core), "AxisAncestor");
            GlobalSymbols.DefineStaticOperator(ID.AncestorOrSelf, typeof(Core), "AxisAncestorOrSelf");
            GlobalSymbols.DefineStaticOperator(ID.Parent, typeof(Core), "AxisParent");
            GlobalSymbols.DefineStaticOperator(ID.Descendant, typeof(Core), "AxisDescendant");
            GlobalSymbols.DefineStaticOperator(ID.DescendantOrSelf, typeof(Core), "AxisDescendantOrSelf");
            GlobalSymbols.DefineStaticOperator(ID.Following, typeof(Core), "AxisFollowing");
            GlobalSymbols.DefineStaticOperator(ID.FollowingSibling, typeof(Core), "AxisFollowingSibling");
            GlobalSymbols.DefineStaticOperator(ID.Preceding, typeof(Core), "AxisPreceding");
            GlobalSymbols.DefineStaticOperator(ID.PrecedingSibling, typeof(Core), "AxisPrecedingSibling");

            XQueryFunctionTable.Register(ID.Doc, typeof(Core), "GetDocument");
            XQueryFunctionTable.Register(ID.Root, typeof(Core), "GetRoot");
            XQueryFunctionTable.Register(ID.Position, typeof(Core), "CurrentPosition");
            XQueryFunctionTable.Register(ID.Last, typeof(Core), "LastPosition");
            XQueryFunctionTable.Register(ID.BooleanValue, typeof(Core), "BooleanValue");
            XQueryFunctionTable.Register(ID.True, typeof(Core), "True");
            XQueryFunctionTable.Register(ID.False, typeof(Core), "False");
            XQueryFunctionTable.Register(ID.Not, typeof(Core), "Not");
            XQueryFunctionTable.Register(ID.Number, typeof(Core), "Number");
            XQueryFunctionTable.Register(ID.String, typeof(Core), "StringValue");

            GlobalSymbols.Defmacro(ID.Atomize, "(x)", 
                @"(list 'let (list (list 'y (list 'atomize# x))) 
                    (list 'cond (list (list 'null 'y) (list 'trap 'unknown)) (list 't 'y)))");
            GlobalSymbols.Defmacro(ID.NodeValue, "(x)",
                @"(list 'cast (list 'let (list (list 'y (list 'node# x))) 
                    (list 'cond (list (list 'null 'y) (list 'trap 'unknown)) (list 't 'y))) node#type)");
            GlobalSymbols.Defmacro(ID.DynZeroOrOne, "(x)",
                @"(list 'let (list (list 'y x)) 
                    (list 'cond (list (list 'null 'y) (list 'trap 'unknown)) (list 't 'y)))");
            GlobalSymbols.Defmacro(ID.TreatAs, "(x y)", "x");
            GlobalSymbols.Defmacro(ID.Par, "(x)", "x");
        }        

        internal static void Init()
        {
        }

        private static XmlQualifiedName GetQualifiedName(object name, XQueryContext context)
        {
            if (name is XmlQualifiedName)
                return (XmlQualifiedName)name;
            else if (name is String)
                return QNameParser.Parse((string)name, context.NamespaceManager);
            else
                throw new XQueryException(Properties.Resources.XPST0004,
                    "xsd:string | xsd:untypedAtomic | xsd:QName");
        }

        private static XQueryDocumentBuilder GetBuilder(object builder)
        {
            return builder as XQueryDocumentBuilder;
        }

        public static XPathNavigator GetDocument([Implict] Executive executive, String name)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            string fileName = context.GetFileName(name);
            if (fileName == null)
                throw new XQueryException(Properties.Resources.FileNotFound, name);
            IXPathNavigable doc = context.OpenDocument(context.GetFileName(name));
            return doc.CreateNavigator();
        }

        public static XQueryNodeIterator CreateSequence([Implict] Executive executive, object value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            if (value == null)
                return EmptyIterator.Shared;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
                return iter.Clone();
            XPathItem item = value as XPathItem;
            if (item == null)
                item = context.CreateItem(value);
            XPathItem[] items = new XPathItem[1];
            items[0] = item;
            return new NodeIterator(items);
        }

        public static object CheckIsNode([Implict] Executive executive, object value)
        {
            if (value is XPathNavigator || value is XQueryNodeIterator)
                return value;
            throw new XQueryException(Properties.Resources.XPTY0019, value);
        }

        public static XQueryNodeIterator DynExecuteExpr(object obj)
        {
            XQueryExprBase expr = (XQueryExprBase)obj;
            return expr.Execute(null);
        }       

        public static XQueryNodeIterator DynExecuteExpr(object obj, params object[] parameters)
        {
            XQueryExprBase expr = (XQueryExprBase)obj;
            return expr.Execute(parameters);
        }

        public static XQueryNodeIterator DynOrdering([Implict] Executive executive, XQueryOrder order, object obj)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryExpr expr = (XQueryExpr)obj;
            expr.QueryOrder = order;
            XQueryNodeIterator iter = expr.Execute(null);
            return iter;
        }

        public static XPathItem Context([Implict] Executive executive)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            IContextProvider provider = context.ContextProvider;
            return provider.Context;
        }

        public static int CurrentPosition([Implict] Executive executive)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            IContextProvider provider = context.ContextProvider;
            return provider.CurrentPosition;
        }

        public static int LastPosition([Implict] Executive executive)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            IContextProvider provider = context.ContextProvider;
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

        public static object DynCreateElement([Implict] Executive executive, object name, object body)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XmlQualifiedName qname = GetQualifiedName(name, context);
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteStartElement(context.NamespaceManager.LookupPrefix(qname.Namespace), 
                qname.Name, qname.Namespace);
            if (body != null)
                WriteNode(executive, builder, body);
            builder.WriteEndElement();
            return doc.CreateNavigator();
        }

        public static object DynCreateAttribute([Implict] Executive executive, object name, string value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XmlQualifiedName qname = GetQualifiedName(name, context);
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteStartElement("dummy");
            builder.WriteStartAttribute(context.NamespaceManager.LookupPrefix(qname.Namespace), 
                qname.Name, qname.Namespace);
            builder.WriteString(value);
            builder.WriteEndAttribute();
            builder.WriteEndElement();
            XPathNavigator nav = doc.CreateNavigator();
            return new NodeIterator(XPath.AttributeIterator(CreateSequence(executive, nav)));
        }

        public static object DynCreateCData([Implict] Executive executive, string value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteCData(value);
            return doc.CreateNavigator();
        }

        public static object DynCreateText([Implict] Executive executive, string value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteString(value);
            return doc.CreateNavigator();
        }

        public static object DynCreateComment([Implict] Executive executive, string value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteComment(value);
            return doc.CreateNavigator();
        }

        public static object DynCreatePi([Implict] Executive executive, object name, string value)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XmlQualifiedName qname = GetQualifiedName(name, context);
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            builder.WriteProcessingInstruction(qname.Name, value);
            return doc.CreateNavigator();
        }

        public static object CreateBuilder([Implict] Executive executive)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocument doc = context.CreateDocument();
            XQueryDocumentBuilder builder = new XQueryDocumentBuilder(doc);
            return builder;
        }

        public static object CreateNavigator(object o)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            return builder.m_document.CreateNavigator();
        }

        public static object BeginElement([Implict] Executive executive, object o, String prefix, String localName, String ns)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteStartElement(prefix, localName, ns);
            return builder;
        }

        public static object EndElement(object o)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteEndElement();
            return builder;
        }

        public static object BeginAttribute([Implict] Executive executive, object o, String prefix, String localName, String ns)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteStartAttribute(prefix, localName, ns);
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

        public static object WriteString(object o, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteString(text);
            return builder;
        }

        public static object WriteWhitespace(object o, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteWhitespace(text);
            return builder;
        }

        public static object CreateCdata(object o, string text)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            builder.WriteCData(text);
            return builder;
        }

        public static string FormatValue([Implict] Executive executive, object value)
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
            }
            else
            {
                XQueryContext context = (XQueryContext)executive.Owner;
                sb.Append(context.CreateItem(value).Value);
            }
            return sb.ToString();
        }

        public static object WriteNode([Implict] Executive executive, object o, object node)
        {
            XQueryDocumentBuilder builder = GetBuilder(o);
            if (node is XPathNavigator)
            {
                XPathNavigator nav = (XPathNavigator)node;
                if (nav.NodeType == XPathNodeType.Attribute)
                {
                    builder.WriteStartAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                    builder.WriteString(nav.Value);
                    builder.WriteEndAttribute();
                }
                else
                    builder.WriteNode(nav, false);
            }
            else if (node is XPathItem)
                builder.WriteString(((XPathItem)node).Value);
            else if (node is XQueryNodeIterator)
            {
                XQueryNodeIterator iter = (XQueryNodeIterator)node;
                foreach (XPathItem item in iter)
                {
                    XPathNavigator nav = item as XPathNavigator;
                    if (nav != null)
                    {
                        if (nav.NodeType == XPathNodeType.Attribute)
                        {
                            builder.WriteStartAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                            builder.WriteString(nav.Value);
                            builder.WriteEndAttribute();
                        }
                        else
                            builder.WriteNode(nav, false);
                    }
                    else
                        builder.WriteString(item.Value);
                }
            }
            else
                if (node != null)
                {
                    XQueryContext context = (XQueryContext)executive.Owner;
                    builder.WriteString(context.CreateItem(node).Value);
                }
            return builder;
        }       

        public static bool BooleanValue(object value)        
        {
            if (value == null)
                return false;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                if (!iter.MoveNext())
                    return false;
                XPathItem v = iter.Current;
                if (v is XPathNavigator)
                    return true;
                if (iter.MoveNext())
                    return true;
                switch (v.XmlType.TypeCode)
                {
                    case XmlTypeCode.Boolean:
                        return v.ValueAsBoolean;
                    case XmlTypeCode.String:
                    case XmlTypeCode.UntypedAtomic:
                        return v.Value != String.Empty;
                    case XmlTypeCode.Float:
                    case XmlTypeCode.Double:
                    case XmlTypeCode.Decimal:
                        return v.ValueAsDouble != Double.NaN &&
                            v.ValueAsDouble != 0.0;
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
                        return v.ValueAsLong != 0;
                    case XmlTypeCode.NonNegativeInteger:
                    case XmlTypeCode.UnsignedLong:
                    case XmlTypeCode.PositiveInteger:
                        return (ulong)(v.ValueAs(typeof(ulong))) != 0;
                }
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
                    case TypeCode.Decimal:
                        return Convert.ToDouble(value, CultureInfo.InvariantCulture) != 0.0 &&
                            Convert.ToDouble(value, CultureInfo.InvariantCulture) != Double.NaN;
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return Convert.ToUInt64(value, CultureInfo.InvariantCulture) != 0;
                }
            }
            // otherwise, return true
            return true;
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
                    return null;
                object res = iter.Current.TypedValue;
                if (iter.MoveNext())
                    throw new XQueryException(Properties.Resources.MoreThanOneItem);
                return res;
            }
            return value;
        }

        public static XPathNavigator NodeValue(object value)
        {
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                iter = iter.Clone();
                if (!iter.MoveNext())
                    return null;
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

        public static DateTime DateTimeValue(object value)
        {
            if (value is DateTime)
                return (DateTime)value;
            else if (value is XQueryAtomicValue)
                return ((XQueryAtomicValue)value).ValueAsDateTime;
            else
                throw new InvalidCastException(String.Format("Can't cast value {0} to dateTime", value));
        }

        public static bool Some([Implict] Executive executive, object expr)
        {
            XQueryNodeIterator iter = expr as XQueryNodeIterator;
            if (iter != null)
            {
                while (iter.MoveNext())
                    if (BooleanValue(iter.Current))
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
                    if (!BooleanValue(iter.Current))
                        return false;
            }
            return true;            
        }

        public static object Mod(object arg1, object arg2)
        {
            if (arg1 == null || arg2 == null)
                return null;
            else
                if (arg1 is IConvertible && arg2 is IConvertible)
                {
                    switch (TypeConverter.GetTypeCode(arg1, arg2))
                    {
                        case TypeCode.Int32:
                            return Convert.ToInt32(arg1) % Convert.ToInt32(arg2);

                        case TypeCode.UInt32:
                            return Convert.ToUInt32(arg1) % Convert.ToUInt32(arg2);

                        case TypeCode.Int64:
                            return Convert.ToInt64(arg1) % Convert.ToInt64(arg2);

                        case TypeCode.UInt64:
                            return Convert.ToUInt64(arg1) % Convert.ToUInt64(arg2);

                        case TypeCode.Single:
                            return Convert.ToSingle(arg1) % Convert.ToSingle(arg2);

                        case TypeCode.Double:
                            return Convert.ToDouble(arg1) % Convert.ToDouble(arg2);

                        case TypeCode.Decimal:
                            return Convert.ToDecimal(arg1) % Convert.ToDecimal(arg2);

                        case TypeCode.String:
                            return Convert.ToDouble(arg1, CultureInfo.InvariantCulture) % 
                                Convert.ToDouble(arg2, CultureInfo.InvariantCulture);

                        default:
                            throw new InvalidCastException();
                    }
                }
                else
                    throw new InvalidCastException();
        }

        public static object Div(object arg1, object arg2)
        {
            if (arg1 is System.Int32 && arg2 is System.Int32)
                return Convert.ToDouble(arg1) / Convert.ToDouble(arg2);
            else
                return Runtime.DynamicDiv(arg1, arg2);
        }

        public static object IDiv(object arg1, object arg2)
        {
            object res = Div(arg1, arg2);
            if (res == null)
                return null;
            return Convert.ToInt32(res);
        }

        public static object CastTo([Implict] Executive engine, object value, XQuerySequenceType destType)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            if (destType == XQuerySequenceType.Item)
                return value;
            if (destType.Cardinality == XmlTypeCardinality.One ||
                destType.Cardinality == XmlTypeCardinality.ZeroOrOne)
            {
                XQueryNodeIterator iter = value as XQueryNodeIterator;
                if (iter != null)
                {
                    iter = iter.Clone();
                    if (!iter.MoveNext())
                    {
                        if (destType.Cardinality == XmlTypeCardinality.One)
                            throw new XQueryException(Properties.Resources.XPTY0004, "item()?", destType);
                        return null;
                    }
                    XPathItem res = iter.Current.ChangeType(destType, context.nameTable, context.nsManager);
                    if (iter.MoveNext())
                        throw new XQueryException(Properties.Resources.MoreThanOneItem);
                    return res;
                }
                XPathItem item = value as XPathItem;
                if (item == null)
                    item = context.CreateItem(value);
                return item.ChangeType(destType, context.nameTable, context.nsManager).TypedValue;
            }
            else
            {
                XQueryNodeIterator iter = CreateSequence(engine, value);
                return new NodeIterator(XPath.ConvertIterator(iter, destType, 
                    context.nameTable, context.nsManager));
            }
        }

        public static XQueryNodeIterator CastToItem([Implict] Executive executive, 
            object value, XQuerySequenceType destType)
        {
            XPathItem[] res = new XPathItem[1];
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                iter = iter.Clone();
                if (!iter.MoveNext())
                {
                    if (destType.Cardinality == XmlTypeCardinality.One)
                        throw new XQueryException(Properties.Resources.XPTY0004, "item()?", destType);
                    return null;
                }
                res[0] = iter.Current.ChangeType(destType, context.nameTable, context.nsManager);
                if (iter.MoveNext())
                    throw new XQueryException(Properties.Resources.MoreThanOneItem);
            }
            XPathItem item = value as XPathItem;
            if (item == null)
                item = context.CreateItem(value);
            res[0] = item.ChangeType(destType, context.nameTable, context.nsManager);
            return new NodeIterator(res);
        }

        public static bool InstanceOf([Implict] Executive engine, object value, XQuerySequenceType destType)
        {
            XQueryContext context = (XQueryContext)engine.Owner;
            XQueryNodeIterator iter = value as XQueryNodeIterator;
            if (iter != null)
            {
                int num = 0;
                foreach (XPathItem item in iter)
                {
                    if (num == 2)
                    {
                        if (destType.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                            destType.Cardinality == XmlTypeCardinality.One)
                            return false;
                    }
                    if (!destType.Match(item))
                        return false;
                    num++;
                }
                if (num == 0)
                {
                    if (destType.Cardinality == XmlTypeCardinality.One ||
                        destType.Cardinality == XmlTypeCardinality.OneOrMore)
                        return false;
                }
                return true;
            }
            else                
            {
                XPathItem item = value as XPathItem;
                if (item == null)
                    item = context.CreateItem(value);
                return destType.Match(item);
            }
        }

        public static bool Castable(object value, XQuerySequenceType destType)
        {
            if (value is XQueryNodeIterator)
            {
                XQueryNodeIterator iter = (XQueryNodeIterator)value;
                iter = iter.Clone();
                if (!iter.MoveNext())
                    return destType.Cardinality == XmlTypeCardinality.ZeroOrOne;
                XPathItem curr = iter.Current.Clone();
                if (iter.MoveNext())
                    return false;
                XmlTypeCode src = XQuerySequenceType.GetXmlTypeCode(curr);
                return XQuerySequenceType.CanConvert(src, destType.TypeCode);
            }
            else
            {
                XmlTypeCode src = XQuerySequenceType.GetXmlTypeCode(value);
                return XQuerySequenceType.CanConvert(src, destType.TypeCode);
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
            return nav1.ComparePosition(nav2) == XmlNodeOrder.Before;
        }

        public static bool FollowingNode(object a, object b)
        {
            XPathNavigator nav1 = (XPathNavigator)a;
            XPathNavigator nav2 = (XPathNavigator)b;
            return nav1.ComparePosition(nav2) == XmlNodeOrder.After;
        }

        public static bool GeneralEQ([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = CreateSequence(executive, a);
            XQueryNodeIterator iter2 = CreateSequence(executive, b);
            foreach (XPathItem item1 in iter1)
                foreach (XPathItem item2 in iter2)
                    if (Runtime.DynamicEq(Atomize(item1), Atomize(item2)) != null)
                        return true;
            return false;
        }

        public static bool GeneralGT([Implict] Executive executive, object a, object b)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            XQueryNodeIterator iter1 = CreateSequence(executive, a);
            XQueryNodeIterator iter2 = CreateSequence(executive, b);
            foreach (XPathItem item1 in iter1)
                foreach (XPathItem item2 in iter2)
                    if (Runtime.DynamicGt(Atomize(item1), Atomize(item2)) != null)
                        return true;
            return false;
        }

        public static bool GeneralNE([Implict] Executive executive, object a, object b)
        {
            return !GeneralEQ(executive, a, b);
        }

        public static bool GeneralGE([Implict] Executive executive, object a, object b)
        {
            return GeneralGT(executive, a, b) || GeneralEQ(executive, a, b);
        }

        public static bool GeneralLT([Implict] Executive executive, object a, object b)
        {
            return GeneralGT(executive, b, a);
        }

        public static bool GeneralLE([Implict] Executive executive, object a, object b)
        {
            return GeneralLT(executive, a, b) || GeneralEQ(executive, a, b);
        }

        public static XQueryNodeIterator GetRange([Implict] Executive executive, object lo, object high)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            return new NodeIterator(XPath.RangeIterator(context, Convert.ToInt32(lo), Convert.ToInt32(high)));
        }

        public static XQueryNodeIterator NameTest(XmlQualifiedNameTest nameTest, XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.NameTestIterator(nameTest, iter));
        }

        public static XQueryNodeIterator TypeTest(XQuerySequenceType typeTest, XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.TypeTestIterator(typeTest, iter));
        }

        public static XQueryNodeIterator AxisChild(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.ChildIterator(iter));
        }

        public static XQueryNodeIterator AxisAttribute(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.AttributeIterator(iter));
        }
        
        public static XQueryNodeIterator AxisDescendant(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.DescendantIterator(iter, false));
        }
        
        public static XQueryNodeIterator AxisDescendantOrSelf(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.DescendantIterator(iter, true));
        }

        public static XQueryNodeIterator AxisParent(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.ParentIterator(iter));
        }

        public static XQueryNodeIterator AxisAncestor(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.AncestorIterator(iter, false));
        }

        public static XQueryNodeIterator AxisAncestorOrSelf(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.AncestorIterator(iter, true));
        }

        public static XQueryNodeIterator AxisFollowingSibling(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.FollowingSiblingIterator(iter));
        }

        public static XQueryNodeIterator AxisPrecedingSibling(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.PrecedingSiblingIterator(iter));
        }

        public static XQueryNodeIterator AxisFollowing(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.FollowingIterator(iter));
        }

        public static XQueryNodeIterator AxisPreceding(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.PrecedingIterator(iter));
        }

        public static XQueryNodeIterator AxisSelf(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.SelfIterator(iter));
        }

        public static XQueryNodeIterator AxisNamespace(XQueryNodeIterator iter)
        {
            return new NodeIterator(XPath.NamespaceIterator(iter));
        }

        public static XQueryNodeIterator Union([Implict] Executive executive, object a, object b)
        {
            XQueryNodeIterator iter1 = Core.CreateSequence(executive, a);
            XQueryNodeIterator iter2 = Core.CreateSequence(executive, b);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (context.IsOrdered)
                return new NodeIterator(XPath.UnionIterator1(iter1, iter2));
            else
                return new NodeIterator(XPath.UnionIterator2(iter1, iter2));
        }

        public static XQueryNodeIterator Except([Implict] Executive executive, object a, object b)
        {
            XQueryNodeIterator iter1 = Core.CreateSequence(executive, a);
            XQueryNodeIterator iter2 = Core.CreateSequence(executive, b);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (context.IsOrdered)
                return new NodeIterator(XPath.IntersectExceptIterator1(true, iter1, iter2));
            else
                return new NodeIterator(XPath.IntersectExceptIterator2(true, iter1, iter2));
        }

        public static XQueryNodeIterator Intersect([Implict] Executive executive, object a, object b)
        {
            XQueryNodeIterator iter1 = Core.CreateSequence(executive, a);
            XQueryNodeIterator iter2 = Core.CreateSequence(executive, b);
            XQueryContext context = (XQueryContext)executive.Owner;
            if (context.IsOrdered)
                return new NodeIterator(XPath.IntersectExceptIterator1(false, iter1, iter2));
            else
                return new NodeIterator(XPath.IntersectExceptIterator2(false, iter1, iter2));
        }

        public static XPathNavigator GetRoot([Implict] Executive executive)
        {
            return GetRoot(NodeValue(Context(executive)));
        }

        public static XPathNavigator GetRoot(XPathNavigator nav)
        {
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

        public static bool Not(object value)
        {
            return !BooleanValue(value);
        }
        
        public static double Number([Implict] Executive executive)
        {
            return Number(Core.Atomize(Core.Context(executive)));
        }

        public static double Number([XQueryParameter(XmlTypeCode.AnyAtomicType)] object value)
        {
            try
            {
                return (double)Convert.ChangeType(value, TypeCode.Double,
                    CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                return Double.NaN;
            }
        }

        public static string StringValue([Implict] Executive executive)
        {
            return StringValue(Core.Context(executive));
        }

        public static string StringValue([XQueryParameter(XmlTypeCode.AnyAtomicType, 
            Cardinality=XmlTypeCardinality.ZeroOrOne)] object value)
        {
            if (value == null)
                return null;
            return Core.Atomize(value).ToString();
        }
    }
}
