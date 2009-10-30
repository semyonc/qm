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
using System.Reflection;
using System.Net;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;
using System.Diagnostics;
using System.Globalization;

using DataEngine.XQuery.Util;

namespace DataEngine.XQuery
{
    public enum SchemaProcessingMode
    {
        Default,
        Disable,
        Force
    }

    public enum CopyNamespaceMode
    {
        PreserveNoInherit,
        PreserveInherit,
        NoPreserveNoInherit,
        NoPreserveInherit
    }

    public enum NamespacePreserveMode
    {
        Preserve,
        NoPreserve
    }

    public enum NamespaceInheritanceMode
    {
        NoInherit,
        Inherit        
    }

    public enum ElementConstructionMode
    {
        Strip,
        Preserve        
    }

    public class XQueryContext
    {
        public class VariableRecord
        {
            public object id;
            public object expr;
            public XQuerySequenceType varType;
            public SymbolLink link;
            public XQueryContext module;
        }

        public class ExternalVariableRecord
        {
            public XQuerySequenceType varType;
            public bool requred;
            public bool initialized;
        }

        private class InnerExecutive : Executive
        {
            public InnerExecutive(object owner)
                : base(owner)
            {
            }

            public override void HandleRuntimeException(Exception exception)
            {
                if (exception is Runtime.OperatorMismatchException)
                {
                    Runtime.OperatorMismatchException ex = (Runtime.OperatorMismatchException)exception;
                    if (ex.ID == Funcs.Neg)
                        throw new XQueryException(Properties.Resources.UnaryOperatorNotDefined, "fn:unary-minus",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One));
                    else if (ex.ID == Funcs.Add)
                        throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:add",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One),
                            new XQuerySequenceType(ex.Arg2.GetType(), XmlTypeCardinality.One));
                    else if (ex.ID == Funcs.Sub)
                        throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:sub",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One),
                            new XQuerySequenceType(ex.Arg2.GetType(), XmlTypeCardinality.One));
                    else if (ex.ID == Funcs.Mul)
                        throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:mul",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One),
                            new XQuerySequenceType(ex.Arg2.GetType(), XmlTypeCardinality.One));
                    else if (ex.ID == Funcs.Div)
                        throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:div",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One),
                            new XQuerySequenceType(ex.Arg2.GetType(), XmlTypeCardinality.One));
                    else if (ex.ID == Funcs.IDiv)
                        throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:idiv",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One),
                            new XQuerySequenceType(ex.Arg2.GetType(), XmlTypeCardinality.One));
                    else if (ex.ID == Funcs.Mod)
                        throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:mod",
                            new XQuerySequenceType(ex.Arg1.GetType(), XmlTypeCardinality.One),
                            new XQuerySequenceType(ex.Arg2.GetType(), XmlTypeCardinality.One));
                }
                else if (exception is System.DivideByZeroException)
                    throw new XQueryException(Properties.Resources.FOAR0001);
                else if (exception is System.OverflowException)
                    throw new XQueryException(Properties.Resources.FOAR0002);
                throw exception;
            }


            public override object OperatorEq(object arg1, object arg2)
            {
                if (Object.ReferenceEquals(arg1, arg2))
                    return true;
                else
                {
                    if (arg1 == null)
                        arg1 = false;
                    if (arg2 == null)
                        arg2 = false;
                    if (arg1.Equals(arg2))
                        return true;
                    else
                    {
                        if (TypeConverter.IsNumberType(arg1.GetType()) &&
                            TypeConverter.IsNumberType(arg2.GetType()))
                        {
                            NumericCode code = TypeConverter.GetNumericCode(arg1, arg2);
                            if (code == NumericCode.Unknown)
                                throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:eq",
                                    new XQuerySequenceType(arg1.GetType(), XmlTypeCardinality.One),
                                    new XQuerySequenceType(arg2.GetType(), XmlTypeCardinality.One));
                            object val1 = TypeConverter.ChangeType(arg1, code);
                            object val2 = TypeConverter.ChangeType(arg2, code);
                            if (((IComparable)val1).CompareTo(val2) == 0)
                                return true;
                        }
                        else
                        {
                            object a = arg1;
                            object b = arg2;
                            if (arg1 is UntypedAtomic || arg1 is AnyUriValue)
                                a = arg1.ToString();
                            if (arg2 is UntypedAtomic || arg2 is AnyUriValue)
                                b = arg2.ToString();
                            if (a.GetType() == b.GetType() || 
                                (a is DurationValue && b is DurationValue))
                            {
                                if (a.Equals(b))
                                    return true;
                            }
                            else
                                throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:eq",
                                    new XQuerySequenceType(arg1.GetType(), XmlTypeCardinality.One),
                                    new XQuerySequenceType(arg2.GetType(), XmlTypeCardinality.One));
                        }
                    }
                }
                return null;
            }

            public override object OperatorGt(object arg1, object arg2)
            {
                if (arg1 == null)
                    arg1 = false;
                if (arg2 == null)
                    arg2 = false;
                if (arg1 is IComparable && arg2 is IComparable)
                {
                    if (TypeConverter.IsNumberType(arg1.GetType()) &&
                        TypeConverter.IsNumberType(arg2.GetType()))
                    {
                        NumericCode code = TypeConverter.GetNumericCode(arg1, arg2);
                        if (code == NumericCode.Unknown)
                            throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                                new XQuerySequenceType(arg1.GetType(), XmlTypeCardinality.One),
                                new XQuerySequenceType(arg2.GetType(), XmlTypeCardinality.One));
                        object val1 = TypeConverter.ChangeType(arg1, code);
                        object val2 = TypeConverter.ChangeType(arg2, code);
                        if (((IComparable)val1).CompareTo(val2) > 0)
                            return true;
                    }
                    else
                    {
                        object a = arg1;
                        object b = arg2;
                        if (arg1 is UntypedAtomic || arg1 is AnyUriValue)
                            a = arg1.ToString();
                        if (arg2 is UntypedAtomic || arg2 is AnyUriValue)
                            b = arg2.ToString();
                        if (a.GetType() == b.GetType())
                        {
                            if (((IComparable)a).CompareTo(b) > 0)
                                return true;
                        }
                        else
                            throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                                new XQuerySequenceType(arg1.GetType(), XmlTypeCardinality.One),
                                new XQuerySequenceType(arg2.GetType(), XmlTypeCardinality.One));
                    }
                }
                else
                    throw new XQueryException(Properties.Resources.BinaryOperatorNotDefined, "op:gt",
                        new XQuerySequenceType(arg1.GetType(), XmlTypeCardinality.One),
                        new XQuerySequenceType(arg2.GetType(), XmlTypeCardinality.One));
                return null;
            }
        }

        internal string moduleNamespace;
        internal string uri;
        internal bool slave;
        internal bool needValidationParser;
        internal XQueryContext master;
        internal NameTable nameTable;
        internal XmlSchemaSet schemaSet;
        internal XQueryNodeInfoTable nodeInfoTable;
        internal XQuerySchemaInfoTable schemaInfoTable;
        internal XmlNamespaceManager nsManager;
        internal Executive lispEngine;
        internal List<XQueryDocument> worklist;
        internal Stack<XQueryOrder> queryOrdering;
        internal List<String> moduleList;
        internal List<VariableRecord> variables;
        internal Dictionary<object, ExternalVariableRecord> externalVars;
        internal Dictionary<XmlQualifiedName, string> option;

        public XQueryContext()
        {
            DefaultOrdering = XQueryOrder.Ordered;
            EmptyOrderSpec = XQueryEmptyOrderSpec.Least;
            SearchPath = String.Empty;
            SchemaProcessing = SchemaProcessingMode.Default;
            
            nameTable = new NameTable();
            schemaSet = new XmlSchemaSet(nameTable);
            nodeInfoTable = new XQueryNodeInfoTable(nameTable);
            schemaInfoTable = new XQuerySchemaInfoTable();
            nsManager = new XmlNamespaceManager(nameTable);
            worklist = new List<XQueryDocument>();
            queryOrdering = new Stack<XQueryOrder>();  
            Resolver = new XQueryResolver();
            moduleList = new List<string>();
            variables = new List<VariableRecord>();
            externalVars = new Dictionary<object, ExternalVariableRecord>();
            option = new Dictionary<XmlQualifiedName, string>();
            
            Core.Init();
            FunctionTable = XQueryFunctionTable.CreateInstance();

            lispEngine = new InnerExecutive(this);
            lispEngine.Set("node#type", typeof(XPathNavigator));
            lispEngine.Enter(Resolver);
            //lispEngine.m_traceOutput = Console.Out;
            
            AddNamespace("xml", XmlReservedNs.NsXml);

            DefaultCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DefaultCulture.NumberFormat.CurrencyGroupSeparator = "";
            DefaultCulture.NumberFormat.NumberGroupSeparator = "";
        }

        internal XQueryContext(string moduleNamespace, string uri, XQueryContext master)
        {
            DefaultOrdering = XQueryOrder.Ordered;
            EmptyOrderSpec = XQueryEmptyOrderSpec.Least;
            SearchPath = master.SearchPath;
            BaseUri = master.BaseUri;

            nameTable = master.nameTable;
            schemaSet = new XmlSchemaSet(nameTable);
            nodeInfoTable = master.nodeInfoTable;
            schemaInfoTable = master.schemaInfoTable;
            variables = master.variables;
            externalVars = master.externalVars;
            option = new Dictionary<XmlQualifiedName, string>();

            nsManager = new XmlNamespaceManager(nameTable);
            nsManager.AddNamespace("xml", XmlReservedNs.NsXml);

            worklist = master.worklist;
            queryOrdering = master.queryOrdering;
            moduleList = new List<string>();

            FunctionTable = XQueryFunctionTable.CreateInstance();
            lispEngine = master.lispEngine;
            Resolver = master.Resolver;
            
            this.master = master;
            slave = true;
            this.moduleNamespace = moduleNamespace;
            this.uri = uri;
        }

        public virtual XmlReaderSettings GetSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.NameTable = nameTable;
            settings.Schemas = schemaSet;
            settings.ProhibitDtd = false;
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = CredentialCache.DefaultCredentials;
            settings.XmlResolver = resolver;
            settings.ValidationEventHandler += new ValidationEventHandler(settings_ValidationEventHandler);
            if (SchemaProcessing == SchemaProcessingMode.Force || 
                (SchemaProcessing != SchemaProcessingMode.Disable && NeedValidatedParser))
            {
                settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation |
                    XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationType = ValidationType.Schema;
            }
            return settings;
        }

        public virtual void InitNamespaces()
        {
            if (nsManager.LookupNamespace("xs") == null)
                AddNamespace("xs", XmlReservedNs.NsXs);
            if (nsManager.LookupNamespace("xsi") == null)
                AddNamespace("xsi", XmlReservedNs.NsXsi);
            if (nsManager.LookupNamespace("fn") == null)
                AddNamespace("fn", XmlReservedNs.NsXQueryFunc);
            if (nsManager.LookupNamespace("local") == null)
                AddNamespace("local", XmlReservedNs.NsXQueryLocalFunc);            
        }

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            ValidationError((XmlReader)sender, e);
        }

        protected virtual void ValidationError(XmlReader sender, ValidationEventArgs e)
        {
            Trace.WriteLine(e.Message);
        }

        public XmlReader CreateReader(Stream stream)
        {            
            return XmlReader.Create(stream, GetSettings());
        }

        public XmlReader CreateReader(String uri)
        {
            return XmlReader.Create(uri, GetSettings());
        }

        public virtual XPathItem CreateItem(object value)
        {
            if (value == null)
                return new XQueryAtomicValue(false, nsManager);
            else if (value is XQueryDocumentBuilder)
            {
                XQueryDocumentBuilder builder = (XQueryDocumentBuilder)value;
                return builder.m_document.CreateNavigator();
            }
            else
                return new XQueryAtomicValue(value, nsManager);
        }

        public virtual string GetFileName(string name)
        {
            if (Uri.IsWellFormedUriString(name, UriKind.Absolute))
                return name;
            else
            {
                if (BaseUri != null)
                {
                    if (Uri.IsWellFormedUriString(BaseUri, UriKind.Absolute))
                        return new Uri(new Uri(BaseUri), name).ToString();
                    else
                    {
                        string fileName = Path.Combine(BaseUri, name);
                        if (File.Exists(fileName))
                            return fileName;
                    }
                }
                string[] pathset = SearchPath.Split(new char[] { ';' });
                foreach (string baseDir in pathset)
                {
                    string fileName = Path.Combine(baseDir, name);
                    if (File.Exists(fileName))
                        return fileName;
                }
            }
            return null;
        }

        public virtual Literal[] ResolveModuleImport(string prefix, string targetNamespace)
        {
            if (slave)
                return master.ResolveModuleImport(prefix, targetNamespace);
            else
                throw new XQueryException(Properties.Resources.XQST0059, targetNamespace);
        }
        
        public XQueryDocument CreateDocument()
        {
            XQueryDocument doc = new XQueryDocument(nameTable, nodeInfoTable, schemaInfoTable);
            worklist.Add(doc);
            return doc;
        }

        public void AddDocument(XQueryDocument doc)
        {
            worklist.Add(doc);
        }

        public IXPathNavigable OpenDocument(string fileName)
        {
            if (slave)
                return master.OpenDocument(fileName);
            else
            {
                Uri uri = new Uri(fileName);
                return OpenDocument(uri);
            }
        }

        public virtual IXPathNavigable OpenDocument(Uri uri)
        {
            if (slave)
                return master.OpenDocument(uri);
            else
            {
                foreach (XQueryDocument doc in worklist)
                {
                    if (doc.baseUri == uri.AbsoluteUri)
                        return doc;
                }
                XQueryDocument ndoc = CreateDocument();
                ndoc.Open(uri, GetSettings(), XmlSpace.Default);
                return ndoc;
            }
        }

        public virtual void Close()
        {
            if (!slave)
            {
                lispEngine.Leave();
                foreach (XQueryDocument doc in worklist)
                    doc.Close();
            }
        }

        public void AddNamespace(string prefix, string ns)
        {
            nsManager.AddNamespace(nameTable.Add(prefix), nameTable.Add(ns));
        }

        public void CopyNamespaces(IXmlNamespaceResolver nsmgr)
        {
            IDictionary<string, string> nss = nsmgr.GetNamespacesInScope(XmlNamespaceScope.All);
            foreach (KeyValuePair<string, string> kvp in nss)
                AddNamespace(kvp.Key, kvp.Value);
        }

        public virtual XQueryNodeIterator CreateCollection(string collection_name)
        {
            if (slave)
                return master.CreateCollection(collection_name);
            else
                throw new XQueryException(Properties.Resources.FODC0004, collection_name);
        }

        public bool IsOrdered
        {
            get
            {
                if (queryOrdering.Count == 0)
                    return DefaultOrdering == XQueryOrder.Ordered;
                else
                    return queryOrdering.Peek() == XQueryOrder.Ordered;
            }
        }

        public void EnterOrdering(XQueryOrder order)
        {
            if (order == XQueryOrder.Default)
                throw new ArgumentException();
            queryOrdering.Push(order);
        }

        public CultureInfo GetCulture(string collationName)
        {
            if (String.IsNullOrEmpty(collationName) ||
                collationName == XmlReservedNs.NsCollationCodepoint)
                return null;
            try
            {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(collationName);
            }
            catch (ArgumentException)
            {
                throw new XQueryException(Properties.Resources.XQST0076, collationName);
            }
        }

        public void LeaveOrdering()
        {
            queryOrdering.Pop();
        }

        internal void AddExternalVariable(object var, XQuerySequenceType varType)
        {
            ExternalVariableRecord rec = new ExternalVariableRecord();
            rec.varType = varType;
            rec.requred = false;
            externalVars.Add(var, rec);
        }

        internal void MarkExternalVariable(object var)
        {
            externalVars[var].requred = true;
        }

        internal void AddVariable(object id, object expr, XQuerySequenceType varType, SymbolLink link)
        {
            VariableRecord rec = new VariableRecord();
            rec.id = id;
            rec.expr = expr;
            rec.varType = varType;
            rec.link = link;
            rec.module = this;
            variables.Add(rec);
        }

        public VariableRecord[] GetVariables()
        {
            return variables.ToArray();
        }

        public void SetExternalVariable(object var, object value)
        {
            ExternalVariableRecord rec;
            if (externalVars.TryGetValue(var, out rec))
            {
                rec.initialized = true;
                SymbolLink link = lispEngine.Get(var);
                if (rec.varType == XQuerySequenceType.Item)
                    link.Value = value;
                else
                    link.Value = Core.CastTo(lispEngine, value, rec.varType);                
            }
        }

        public void CheckExternalVariables()
        {
            foreach (KeyValuePair<object, ExternalVariableRecord> kvp in externalVars)
                if (kvp.Value.requred && !kvp.Value.initialized)
                    throw new XQueryException(Properties.Resources.ExternalVariableNotSet, kvp.Key);
        }

        public XmlSchemaSet SchemaSet
        {
            get
            {
                return schemaSet;
            }
        }

        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                return nsManager;
            }
        }

        public Executive Engine
        {
            get
            {
                return lispEngine;
            }
        }

        public String SearchPath { get; set; }

        public String DefaultCollation { get; set; }

        public XQueryOrder DefaultOrdering { get; set; }

        public String DefaultElementNS { get; set; }

        public String DefaultFunctionNS { get; set; }

        public bool PreserveBoundarySpace { get; set; }

        public String BaseUri { get; set; }

        public ElementConstructionMode ConstructionMode { get; set; }

        public XQueryEmptyOrderSpec EmptyOrderSpec { get; set; }

        public NamespacePreserveMode NamespacePreserveMode { get; set; }

        public NamespaceInheritanceMode NamespaceInheritanceMode { get; set; }

        public XQueryFunctionTable FunctionTable { get; private set; }

        public XQueryResolver Resolver { get; private set; }

        public SchemaProcessingMode SchemaProcessing { get; set; }

        public IDictionary<XmlQualifiedName, string> Option
        {
            get
            {
                return option;
            }
        }

        public CultureInfo DefaultCulture { get; private set; }

        internal bool NeedValidatedParser
        {
            get
            {
                if (slave)
                    return master.NeedValidatedParser;
                else
                    return needValidationParser;
            }

            set
            {
                if (slave)
                    master.NeedValidatedParser = value;
                else
                    needValidationParser = value;
            }
        }
    }

    public class XPathContext : XQueryContext
    {
        private Dictionary<String, IXPathNavigable> docs;

        public XPathContext()
        {
            docs = new Dictionary<String, IXPathNavigable>();
        }

        public override XmlReaderSettings GetSettings()
        {
            XmlReaderSettings settings = base.GetSettings();
            settings.ValidationFlags = XmlSchemaValidationFlags.None;
            settings.ValidationType = ValidationType.None;
            return settings;
        }

        public override IXPathNavigable OpenDocument(Uri uri)
        {
            IXPathNavigable doc;
            if (docs.TryGetValue(uri.AbsoluteUri, out doc))
                return doc;
            else
            {
                XmlReader reader = XmlReader.Create(uri.AbsoluteUri, GetSettings());
                doc = new XPathNavigableWrapper(new XPathDocument(reader, XmlSpace.Default));
                reader.Close();
                docs.Add(uri.AbsoluteUri, doc);
                return doc;
            }
        }
    }
}
