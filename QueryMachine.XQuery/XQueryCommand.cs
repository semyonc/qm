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

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.XQuery.Parser;

namespace DataEngine.XQuery
{
    public class ResolveCollectionArgs : EventArgs
    {
        public String CollectionName { get; internal set; }
        public XQueryNodeIterator Collection { get; set; }
    }

    public delegate void ResolveCollectionEvent(object sender, ResolveCollectionArgs args);

    public class XQueryCommand : IDisposable, IContextProvider
    {
        protected XQueryContext m_context;
        protected Dictionary<string,string> m_modules;
        protected Dictionary<string,string> m_schemas;
        protected bool m_compiled;
        protected XQueryExprBase m_res;
        protected FunctionLink[] m_vars;
        protected bool m_disposed = false;        

        protected class WorkContext : XQueryContext
        {
            private XQueryCommand m_command;
            private Dictionary<string, XQueryNodeIterator> m_dict = new Dictionary<string, XQueryNodeIterator>();

            public WorkContext(XQueryCommand command, XmlNameTable nameTable)
                : base(nameTable)
            {
                m_command = command;
            }

            public override XQueryNodeIterator CreateCollection(string collection_name)
            {
                lock (m_dict)
                {
                    XQueryNodeIterator res;
                    if (m_dict.TryGetValue(collection_name, out res))
                        return res.Clone();
                    ResolveCollectionArgs args = new ResolveCollectionArgs();
                    args.CollectionName = collection_name;
                    if (m_command.OnResolveCollection != null)
                    {
                        m_command.OnResolveCollection(m_command, args);
                        if (args.Collection != null)
                        {
                            m_dict.Add(collection_name, args.Collection.Clone());
                            return args.Collection;
                        }
                    }
                    throw new XQueryException(Properties.Resources.FODC0004, collection_name);
                }
            }

            public override Literal[] ResolveModuleImport(string prefix, string targetNamespace)
            {
                String uri;
                if (m_command.m_modules != null && m_command.m_modules.TryGetValue(targetNamespace, out uri))
                    return new Literal[] { new Literal(uri) };
                return base.ResolveModuleImport(prefix, targetNamespace);
            }

            public override Literal[] ResolveSchemaImport(string prefix, string targetNamespace)
            {
                String uri;
                if (m_command.m_schemas != null && m_command.m_schemas.TryGetValue(targetNamespace, out uri))
                    return new Literal[] { new Literal(uri) };
                return base.ResolveSchemaImport(prefix, targetNamespace);
            }

            public override void Close()
            {
                base.Close();
                m_dict = null;
            }

            protected override void ValidationError(XmlReader sender, ValidationEventArgs e)
            {
                if (m_command.OnInputValidation != null)
                    m_command.OnInputValidation(sender, e);
            }
        }


        public XQueryCommand(XQueryContext context)
        {
            m_context = context;
            Parameters = new XQueryParameterCollection();
        }
        

        public XQueryCommand(XmlNameTable nameTable)
        {
            m_context = new WorkContext(this, nameTable);
            BaseUri = null;
            SearchPath = null;
            Parameters = new XQueryParameterCollection();
        }

        public XQueryCommand()
            : this(new NameTable())
        {
        }

        ~XQueryCommand()
        {
            Dispose(false);
        }

        protected void CheckDisposed()
        {
            if (m_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {                
                m_context.Close();
                m_disposed = true;
            }
        }

        public XQueryContext DetachContext()
        {
            m_disposed = true;
            return m_context;
        }

        public void Compile()
        {
            CheckDisposed();
            if (String.IsNullOrEmpty(CommandText))
                throw new XQueryException("CommandText is empty string");
            TokenizerBase tok = new Tokenizer(CommandText);
            Notation notation = new Notation();
            YYParser parser = new YYParser(notation);
            parser.yyparseSafe(tok);
            if (BaseUri != null)
                m_context.BaseUri = BaseUri;
            if (SearchPath != null)
                m_context.SearchPath = SearchPath;
            m_context.SchemaProcessing = SchemaProcessing;
            m_context.Engine.Prepare();
            SymbolLink contextLink = new SymbolLink(typeof(IContextProvider));
            m_context.Engine.DefaultPool.Bind(contextLink);
            m_context.Engine.DefaultPool.SetData(contextLink, this);
            m_context.Resolver.SetValue(ID.Context, contextLink);
            Translator translator = new Translator(m_context);
            translator.PreProcess(notation);
            if (OnPreProcess != null) // Chance to process custom declare option statement
                OnPreProcess(this, EventArgs.Empty);
            m_res = translator.Process(notation);
            if (m_res == null)
                throw new XQueryException("Can't run XQuery function module");
            m_res.Bind(null, m_context.Engine.DefaultPool);
            // Compile variables and check it for XQST0054
            m_vars = new FunctionLink[m_context.variables.Count];
            Dictionary<SymbolLink, SymbolLink[]> dependences = new Dictionary<SymbolLink, SymbolLink[]>();
            for (int k = 0; k < m_context.variables.Count; k++)
            {
                XQueryContext.VariableRecord rec = m_context.variables[k];
                m_vars[k] = new FunctionLink();
                dependences.Add(rec.link, m_context.Engine.GetValueDependences(null, rec.expr, m_vars[k], true));
            }
            int i = 0;
            foreach (KeyValuePair<SymbolLink, SymbolLink[]> kvp in dependences)
            {
                List<SymbolLink> closure = new List<SymbolLink>();
                HashSet<SymbolLink> hs = new HashSet<SymbolLink>();
                hs.Add(kvp.Key);
                closure.AddRange(kvp.Value);
                bool expand;
                do
                {
                    expand = false;
                    for (int k = 0; k < closure.Count; k++)
                    {
                        SymbolLink[] values;
                        if (dependences.TryGetValue(closure[k], out values))
                        {
                            if (hs.Contains(closure[k]))
                                throw new XQueryException(Properties.Resources.XQST0054, m_context.variables[i].id);
                            hs.Add(closure[k]);
                            closure.RemoveAt(k);
                            closure.AddRange(values);
                            expand = true;
                            break;
                        }
                    }
                }
                while (expand);
                i++;
            }
            m_compiled = true;
        }        

        public XQueryNodeIterator Execute()
        {
            CheckDisposed();
            if (!m_compiled)
                Compile();
            foreach (XQueryParameter param in Parameters)
            {
                if (param.ID == null)
                    param.ID = Translator.GetVarName(param.LocalName, param.NamespaceUri);
                m_context.SetExternalVariable(param.ID, param.Value);
            }
            MemoryPool pool = m_context.Engine.DefaultPool;
            for (int k = 0; k < m_context.variables.Count; k++)
            {
                XQueryContext.VariableRecord rec = m_context.variables[k];
                object value = m_context.Engine.Apply(null, null, rec.expr, null, m_vars[k], pool);
                if (rec.varType != XQuerySequenceType.Item)
                    value = Core.TreatAs(m_context.Engine, value, rec.varType);
                XQueryNodeIterator iter = value as XQueryNodeIterator;
                if (iter != null)
                    value = iter.CreateBufferedIterator();
                pool.SetData(rec.link, value);
            }            
            m_context.CheckExternalVariables();
            XQueryNodeIterator res = XQueryNodeIterator.Create(m_res.Execute(this, null, pool));
            return res;
        }

        public void DefineModuleNamespace(string targetNamespace, string uri)
        {
            if (m_modules == null)
                m_modules = new Dictionary<string, string>();
            m_modules.Add(targetNamespace, uri);
        }

        public void DefineSchemaNamespace(string targetNamespace, string uri)
        {
            if (m_schemas == null)
                m_schemas = new Dictionary<string, string>();
            m_schemas.Add(targetNamespace, uri);
        }

        public void AddSchema(string targetNamespace, XmlReader reader)
        {
            m_context.SchemaSet.Add(targetNamespace, reader);
        }

        public void AddSchema(string targetNamespace, string schemaUri)
        {
            m_context.SchemaSet.Add(targetNamespace, schemaUri);
        }

        public void AddSchema(XmlSchema schema)
        {
            m_context.SchemaSet.Add(schema);
        }

        public void AddSchemaSet(XmlSchemaSet schemaSet)
        {
            m_context.SchemaSet.Add(schemaSet);
        }

        public String CommandText { get; set; }
        public String BaseUri { get; set; }
        public String SearchPath { get; set; }
        public SchemaProcessingMode SchemaProcessing { get; set; }
        public XQueryParameterCollection Parameters { get; private set; }
        public XPathNavigator ContextItem { get; set; }
        
        public XmlNameTable NameTable
        {
            get
            {
                return m_context.nameTable;
            }
        }
        
        public XQueryContext Context
        {
            get
            {
                return m_context;
            }
        }

        public event EventHandler OnPreProcess;

        public event ResolveCollectionEvent OnResolveCollection;

        public event ValidationEventHandler OnInputValidation;

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region IContextProvider Members

        XPathItem IContextProvider.Context
        {
            get 
            {
                if (ContextItem == null)
                    return null;
                if (ContextItem is XQueryNavigator)
                    return ContextItem;
                else
                    return new XQueryNavigatorWrapper(ContextItem.Clone());
            }
        }

        public int CurrentPosition
        {
            get 
            {
                if (ContextItem == null)
                    throw new XQueryException(Properties.Resources.XPDY0002);
                return 1; 
            }
        }

        public int LastPosition
        {
            get 
            {
                if (ContextItem == null)
                    throw new XQueryException(Properties.Resources.XPDY0002);
                return 1; 
            }
        }

        #endregion
    }
}
