using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    public enum XmlValueOption
    {
        NullOnNull,
        EmptyOnNull,
        AbsentOnNull,
        NilOnNull,
        NilOnNoContent
    }

    [Flags]
    enum XmlValueParseFlags
    {
        StripWhitespace = 0,
        PreserveWhitespace = 1,
        ParseDocument = 0,
        ParseContent = 2
    };

    public partial class ID
    {
        public static readonly object XmlForest = ATOM.Create("XmlForest");
        public static readonly object XmlForestAtt = ATOM.Create("XmlForestAtt");
        public static readonly object XmlValueAtt = ATOM.Create("XmlValueAtt");
        public static readonly object XmlValueCdata = ATOM.Create("XmlValueCdata");
        public static readonly object XmlValueComment = ATOM.Create("XmlValueComment");
        public static readonly object XmlValuePi = ATOM.Create("XmlValuePi");
        public static readonly object XmlValueConcat = ATOM.Create("XmlValueConcat");
        public static readonly object XmlValueElem = ATOM.Create("XmlValueElem");
        public static readonly object XmlValueRoot = ATOM.Create("XmlValueRoot");
        public static readonly object XmlValueNs = ATOM.Create("XmlValueNs");
        public static readonly object XmlValueParse = ATOM.Create("XmlValueParse");
        public static readonly object XmlQuery = ATOM.Create("XmlQuery");
    }

    public class SQLX
    {
        static SQLX()
        {
            GlobalSymbols.Shared.Defmacro(new SQLX.XmlForest());
            GlobalSymbols.Shared.Defmacro(new SQLX.XmlForestAtt());
            GlobalSymbols.DefineStaticOperator(ID.XmlValueAtt, typeof(SQLX), "XmlValueAtt");
            GlobalSymbols.DefineStaticOperator(ID.XmlValueCdata, typeof(SQLX), "XmlValueCdata");
            GlobalSymbols.DefineStaticOperator(ID.XmlValueComment, typeof(SQLX), "XmlValueComment");
            GlobalSymbols.DefineStaticOperator(ID.XmlValuePi, typeof(SQLX), "XmlValuePi");            
            GlobalSymbols.DefineStaticOperator(ID.XmlValueElem, typeof(SQLX), "XmlValueElem");
            GlobalSymbols.DefineStaticOperator(ID.XmlValueRoot, typeof(SQLX), "XmlValueRoot");
            //GlobalSymbols.DefineStaticOperator(ID.XmlValueNs, typeof(SQLX), "XmlValueNs");
            GlobalSymbols.DefineStaticOperator(ID.XmlValueConcat, typeof(SQLX), "XmlValueConcat");
            GlobalSymbols.DefineStaticOperator(ID.XmlQuery, typeof(SQLX), "XmlQuery");
        }

        public static void Initialize()
        {
            return;
        }
        
        public class XmlForest : MacroFuncBase
        {
            public XmlForest()
            {
                Name = new MacroFuncName(ID.XmlForest);
            }

            public override object Execute(Executive engine, object[] lval, out bool proceed)
            {
                DataResolver resolver = (DataResolver)engine.CurrentResolver();
                object res = Lisp.Cons(ID.XmlValueConcat);
                object tail = res;
                if (lval.Length == 0)
                {
                    foreach (ColumnBinding b in resolver.Bindings)
                    {
                        Lisp.Rplacd(tail, Lisp.Cons(Lisp.List(ID.XmlValueElem, b.Name,
                            Lisp.List(Funcs.Weak, ATOM.Create(null, new string[] { b.TableName, b.Name }, false)), XmlValueOption.EmptyOnNull)));
                        tail = Lisp.Cdr(tail);
                    }
                }
                else
                    foreach (object o in lval)
                    {
                        if (Lisp.IsFunctor(o, DataSelector.Table))
                        {
                            string tableName = (string)Lisp.Second(o);
                            XmlValueOption option = (XmlValueOption)Lisp.Third(o);
                            foreach (ColumnBinding b in resolver.Bindings)
                                if (b.TableName.Equals(tableName))
                                {
                                    Lisp.Rplacd(tail, Lisp.Cons(Lisp.List(ID.XmlValueElem, b.Name,
                                        Lisp.List(Funcs.Weak,  ATOM.Create(null, new string[] { b.TableName, b.Name }, false)), option)));
                                    tail = Lisp.Cdr(tail);
                                }
                        }
                        else
                        {
                            Lisp.Rplacd(tail, Lisp.Cons(o));
                            tail = Lisp.Cdr(tail);
                        }
                    }
                proceed = true;
                return res;
            }
        }

        public class XmlForestAtt : MacroFuncBase
        {
            public XmlForestAtt()
            {
                Name = new MacroFuncName(ID.XmlForestAtt);
            }

            public override object Execute(Executive engine, object[] lval, out bool proceed)
            {
                DataResolver resolver = (DataResolver)engine.CurrentResolver();
                object elem = lval[0];
                string tableName = (string)lval[1];
                XmlValueOption option = (XmlValueOption)lval[2];
                foreach (ColumnBinding b in resolver.Bindings)
                    if (String.IsNullOrEmpty(tableName) || b.TableName.Equals(tableName))
                        elem = Lisp.List(ID.XmlValueAtt, elem, b.Name,
                            Lisp.List(Funcs.Weak, ATOM.Create(null, new string[] { b.TableName, b.Name }, false)), option);
                proceed = true;
                return elem;
            }
        }

        public static object XmlValueAtt([Implict] Executive engine, object node, string name, object value, XmlValueOption option)        
        {
            XmlElement elem = (XmlElement)node;
            if (value == null || value == Undefined.Value || 
                (value.Equals(String.Empty) && option == XmlValueOption.NilOnNoContent))
                switch (option)
                {
                    case XmlValueOption.AbsentOnNull:
                        break;

                    case XmlValueOption.EmptyOnNull:
                        elem.SetAttribute(name, "");
                        break;

                    default:
                        throw new ESQLException(Properties.Resources.InvalidOptionForXmlAttribute, name);
                }
            else
                elem.SetAttribute(XmlConvert.EncodeLocalName(name), XmlDataAccessor.Serialize(value));
            return elem;
        }

        private static String GetString(object value)
        {
            if (value == null || value == Undefined.Value)
                return String.Empty;
            else
                return value.ToString();
        }

        public static object XmlValueCdata([Implict] Executive engine, object value)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            return owner.QueryContext.XmlResult.CreateCDataSection(GetString(value));
        }

        public static object XmlValueComment([Implict] Executive engine, object value)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            return owner.QueryContext.XmlResult.CreateComment(GetString(value));
        }

        public static object XmlValuePi([Implict] Executive engine, string target, object value)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            return owner.QueryContext.XmlResult.CreateProcessingInstruction(target, GetString(value));
        }

        public static object XmlValueConcat([Implict] Executive engine, params object[] args)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            XmlDataAccessor.NodeList nodes = new XmlDataAccessor.NodeList();
            for (int i = 0; i < args.Length; i++)
            {
                object val = args[i];
                if (val != null && val != Undefined.Value)
                {
                    if (val is XmlNode)
                        nodes.Add((XmlNode)val);
                    else if (val is XmlNodeList)
                        foreach (XmlNode n in (XmlNodeList)val)
                            nodes.Add(n);
                    else
                        nodes.Add(XmlDataAccessor.Serialize(owner.QueryContext.XmlResult, val));
                }
            }
            return nodes;
        }

        private static XmlElement CreateNullElement(string name, XmlValueOption option, XmlDocument xmlResult)
        {
            switch (option)
            {
                case XmlValueOption.AbsentOnNull:
                    return null;

                case XmlValueOption.EmptyOnNull:
                    return xmlResult.CreateElement(name);

                case XmlValueOption.NilOnNoContent:
                case XmlValueOption.NilOnNull:
                    {
                        XmlElement elem = xmlResult.CreateElement(name);
                        elem.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                        elem.SetAttribute("xsi:nil", "true");
                        return elem;
                    }

                case XmlValueOption.NullOnNull:
                    {
                        XmlElement elem = xmlResult.CreateElement(name);
                        elem.InnerText = "NULL"; // ???????
                        return elem;
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static object XmlValueElem([Implict] Executive engine, String name, object value, XmlValueOption option)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            name = XmlConvert.EncodeLocalName(name);
            XmlDocument xmlResult = owner.QueryContext.XmlResult;
            if (value == null || value == Undefined.Value)
                return CreateNullElement(name, option, xmlResult);
            else if (value is XmlNodeList)
            {
                XmlNodeList nodeList = (XmlNodeList)value;
                if (nodeList.Count == 0)
                    return CreateNullElement(name, option, xmlResult);
                else
                {
                    XmlElement elem = xmlResult.CreateElement(name);
                    foreach (XmlNode node in nodeList)
                    {
                        XmlNode child;
                        if (node.OwnerDocument != elem.OwnerDocument)
                            child = xmlResult.ImportNode(node, true);
                        else
                            child = node.CloneNode(true);
                        elem.AppendChild(child);
                    }
                    return elem;
                }
            }
            else 
            {
                XmlNode node;
                if (value is XmlNode)
                    node = (XmlNode)value;
                else
                    node = XmlDataAccessor.Serialize(xmlResult, value);
                XmlElement elem = xmlResult.CreateElement(name);
                XmlNode child;
                if (node.OwnerDocument != elem.OwnerDocument)
                    child = xmlResult.ImportNode(node, true);
                else
                    child = node.CloneNode(true);
                elem.AppendChild(child);
                return elem;
            }
        }

        public static object XmlValueRoot([Implict] Executive engine, string version, 
            object arg, Object root)
        {
            string standalone = null;
            if (arg != null)
                standalone = arg.ToString().ToLowerInvariant();
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            XmlDocument xmldoc = new XmlDocument(owner.QueryContext.NameTable);
            XmlDeclaration decl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", standalone);
            xmldoc.AppendChild(decl);
            if (root is XmlNodeList)
            {
                foreach (XmlNode n in (XmlNodeList)root)
                    xmldoc.AppendChild(xmldoc.ImportNode(n, true));
            }
            else
                xmldoc.AppendChild(xmldoc.ImportNode((XmlNode)root, true));
            return xmldoc;
        }

        public static object XmlQuery([Implict] Executive engine, object command, object context_item, object arg)
        {
            QueryNode.LispProcessingContext owner = (QueryNode.LispProcessingContext)engine.Owner;
            XQueryAdapter adapter = (XQueryAdapter)command;
            lock (adapter)
            {
                object[] vargs = null;
                if (arg != null)
                    vargs = Lisp.ToArray(arg);
                return adapter.Execute(context_item, vargs, owner.QueryContext.XmlResult);
            }
        }
    }
}
