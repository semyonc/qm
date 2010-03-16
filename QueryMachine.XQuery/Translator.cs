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
using System.Net;

using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.XQuery.Parser;
using System.Globalization;
using DataEngine.XQuery.Util;
using System.Text.RegularExpressions;


namespace DataEngine.XQuery
{
    public class Translator
    {
        protected class FunctionDecl
        {
            public object f;
            public XmlQualifiedName identity;
            public FunctionParameter[] parameters;
            public XQuerySequenceType returnType;

            public FunctionDecl(object f, XmlQualifiedName identity, 
                FunctionParameter[] parameters)
            {
                this.f = f;
                this.identity = identity;
                this.parameters = parameters;
            }
        }

        protected struct FunctionParameter
        {
            public object id;
            public XQuerySequenceType type;
        }

        protected struct FLWORItem
        {
            public Descriptor desc;
            public object var;
            public XQuerySequenceType varType;
            public object pos;
            public object assignExpr;
            public bool convert;
        }

        private XQueryContext _context;
        private List<String> _moduleNs;
        private List<String> _schemaNs;
        private VarTable _varTable;
        private Dictionary<object, FunctionDecl> _fdecl;
        
        private bool _baseUriDecl;
        private bool _defaultCollationDecl;
        private bool _defaultOrderDecl;
        private bool _defaultOrderingDecl;
        private bool _defaultElementNsDecl;
        private bool _defaultFunctionNsDecl;
        private bool _boundarySpaceDecl;
        private bool _copyNamespacesDecl;
        private bool _constructionDecl;

        public Translator(XQueryContext context)
        {
            _context = context;
            _varTable = new VarTable();
            _fdecl = new Dictionary<object, FunctionDecl>();
            _moduleNs = new List<string>();
            _schemaNs = new List<string>();
        }

        public void PreProcess(Notation notation)
        {
            if (!String.IsNullOrEmpty(_context.DefaultElementNS))
                _context.NamespaceManager.AddNamespace("", _context.DefaultElementNS);
            // Phase 1. Namespace declaration, default function namespace and declare option statement.
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            if (recs.Length == 0)
                throw new InvalidOperationException();
            Notation.Record[] recs1 = notation.Select(recs[0].sym, Descriptor.Version, 2);
            if (recs1.Length > 0)
            {
                Literal ver = (Literal)recs1[0].Arg0;
                if (ver.Data != "1.0")
                    throw new XQueryException(Properties.Resources.XQST0031, ver.Data);
                if (recs1[0].Arg1 != null)
                {
                    Literal enc = (Literal)recs1[0].Arg1;
                    if (!Regex.IsMatch(enc.Data, "^[A-Za-z]([A-Za-z0-9._]|-)*$"))
                        throw new XQueryException(Properties.Resources.XQST0087, enc.Data);
                }
            }
            Notation.Record[] recs_c = notation.Select(recs[0].Arg0,
                new Descriptor[] { Descriptor.Query, Descriptor.Library });
            if (recs_c.Length > 0)
                if (recs_c[0].descriptor == Descriptor.Query)
                {
                    if (_context.slave)
                        throw new XQueryException(Properties.Resources.ExpectedModuleDecl);
                    ProcessProlog(notation, recs_c[0].args[0]);
                }
                else
                {
                    ProcessModuleDecl(notation, recs_c[0].Arg0);
                    ProcessProlog(notation, recs_c[0].args[1]);
                }
        }

        public XQueryExprBase Process(Notation notation)
        { // Phase 2. Generation
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            if (recs.Length == 0)
                throw new InvalidOperationException();
            Notation.Record[] recs_c = notation.Select(recs[0].Arg0,
                new Descriptor[] { Descriptor.Query, Descriptor.Library });
            if (recs_c.Length > 0)
                if (recs_c[0].descriptor == Descriptor.Query)
                {
                    ProcessSchemaImport(notation, recs_c[0].args[0]);
                    ProcessImportModule(notation, recs_c[0].args[0]);
                    ProcessFunctionDecl(notation, recs_c[0].args[0]);
                    PostProcess(notation, recs_c[0].args[0]);
                    return ProcessExpr(notation, recs_c[0].args[1]);
                }
                else
                {
                    ProcessSchemaImport(notation, recs_c[0].args[1]);
                    ProcessImportModule(notation, recs_c[0].args[1]);
                    ProcessFunctionDecl(notation, recs_c[0].args[1]);
                    PostProcess(notation, recs_c[0].args[1]);
                }
            return null;
        }

        private void ProcessModuleDecl(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.ModuleNamespace, 2);
            if (recs.Length > 0)
            {
                Qname name = (Qname)recs[0].Arg0;
                Literal ns = (Literal)recs[0].Arg1;
                _context.AddNamespace(name.Name, ns.Data);
            }            
        }

        private void ProcessProlog(Notation notation, object prolog)
        {
            if (prolog != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0)
                    {
                        switch (recs[0].descriptor)
                        {
                            case Descriptor.DefaultFunction:
                                ProcessDefaultFunctionNS(notation, recs[0]);
                                break;

                            case Descriptor.Namespace:
                                ProcessNamespace(notation, recs[0]);
                                break;

                            case Descriptor.BaseUri:
                                ProcessBaseUri(notation, recs[0]);
                                break;

                            case Descriptor.ConstructionDecl:
                                ProcessConstructionDecl(notation, recs[0]);
                                break;

                            case Descriptor.BoundarySpace:
                                ProcessBoundarySpace(notation, recs[0]);
                                break;

                            case Descriptor.Ordering:
                                ProcessOrdering(notation, recs[0]);
                                break;

                            case Descriptor.DefaultOrder:
                                ProcessDefaultOrder(notation, recs[0]);
                                break;

                            case Descriptor.DefaultElement:
                                ProcessDefaultElementNS(notation, recs[0]);
                                break;

                            case Descriptor.CopyNamespace:
                                ProcessCopyNamespace(notation, recs[0]);
                                break;
                        }
                    }
                }
                _context.InitNamespaces();
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0)
                    {
                        switch (recs[0].descriptor)
                        {
                            case Descriptor.OptionDecl:
                                ProcessOptionDecl(notation, recs[0]);
                                break;

                            case Descriptor.DefaultCollation:
                                ProcessDefaultCollation(notation, recs[0]);
                                break;
                        }
                    }
                }
            }
            else
                _context.InitNamespaces();
        }

        private void ProcessConstructionDecl(Notation notation, Notation.Record rec)
        {
            if (_constructionDecl)
                throw new XQueryException(Properties.Resources.XQST0067);
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.PRESERVE:
                    _context.ConstructionMode = ElementConstructionMode.Preserve;
                    break;

                case Token.STRIP:
                    _context.ConstructionMode = ElementConstructionMode.Strip;
                    break;
            }
            _constructionDecl = true;
        }

        private void ProcessSchemaImport(Notation notation, object prolog)
        {
            if (prolog != null)
            {
                bool compile_flag = false;
                Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0 &&
                        recs[0].descriptor == Descriptor.ImportSchema)
                    {
                        compile_flag = true;
                        ProcessImportSchema(notation, recs[0]);
                    }
                }
                if (compile_flag)
                    _context.schemaSet.Compile(); 
            }
        }

        private void ProcessImportModule(Notation notation, object prolog)
        {
            if (prolog != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0 &&
                        recs[0].descriptor == Descriptor.ImportModule)
                        ProcessImportModule(notation, recs[0]);
                }
            }
        }

        private void ProcessFunctionDecl(Notation notation, object prolog)
        {
            if (prolog != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0 &&
                        recs[0].descriptor == Descriptor.DeclareFunction)
                        PreProcessFuncDecl(notation, recs[0]);
                }
            }
        }

        private void PostProcess(Notation notation, object prolog)
        {
            if (prolog != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
                foreach (Symbol sym in arr)
                {
                    Notation.Record[] recs = notation.Select(sym);
                    if (recs.Length > 0)
                        switch (recs[0].descriptor)
                        {
                            case Descriptor.DeclareFunction:
                                ProcessFuncDecl(notation, recs[0]);
                                break;

                            case Descriptor.VarDecl:
                                ProcessVarDecl(notation, recs[0]);
                                break;
                        }
                }
            }
        }

        //private void ProcessVarDecl(Notation notation, object prolog)
        //{
        //    if (prolog != null)
        //    {
        //        Symbol[] arr = Lisp.ToArray<Symbol>(prolog);
        //        foreach (Symbol sym in arr)
        //        {
        //            Notation.Record[] recs = notation.Select(sym);
        //            if (recs.Length > 0 &&
        //                recs[0].descriptor == Descriptor.VarDecl)
        //                ProcessVarDecl(notation, recs[0]);
        //        }
        //    }
        //}

        private void ProcessBoundarySpace(Notation notation, Notation.Record rec)
        {
            if (_boundarySpaceDecl)
                throw new XQueryException(Properties.Resources.XQST0068);
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.PRESERVE:
                    _context.PreserveBoundarySpace = true;
                    break;

                case Token.STRIP:
                    _context.PreserveBoundarySpace = false;
                    break;
            }
            _boundarySpaceDecl = true;
        }

        private void ProcessCopyNamespace(Notation notation, Notation.Record rec)
        {
            if (_copyNamespacesDecl)
                throw new XQueryException(Properties.Resources.XQST0055);
            TokenWrapper wrapper1 = (TokenWrapper)rec.Arg0;
            TokenWrapper wrapper2 = (TokenWrapper)rec.Arg1;
            switch (wrapper1.Data)
            {
                case Token.PRESERVE:
                    _context.NamespacePreserveMode = NamespacePreserveMode.Preserve;
                    break;

                case Token.NO_PRESERVE:
                    _context.NamespacePreserveMode = NamespacePreserveMode.NoPreserve;
                    break;
            }
            switch (wrapper2.Data)
            {
                case Token.INHERIT:
                    _context.NamespaceInheritanceMode = NamespaceInheritanceMode.Inherit;
                    break;

                case Token.NO_INHERIT:
                    _context.NamespaceInheritanceMode = NamespaceInheritanceMode.NoInherit;
                    break;
            }
            _copyNamespacesDecl = true;
        }

        private void ProcessBaseUri(Notation notation, Notation.Record rec)
        {
            if (_baseUriDecl)
                throw new XQueryException(Properties.Resources.XQST0032);
            Literal baseUri = (Literal)rec.Arg0;
            _context.BaseUri = baseUri.Data;
            _baseUriDecl = true;
        }

        private void ProcessImportModule(Notation notation, Notation.Record rec)
        {
            Literal[] arr = null;
            Literal targetNamespace;
            if (rec.args.Length == 2)
            {
                targetNamespace = (Literal)rec.Arg0;
                if (targetNamespace.Data == "")
                    throw new XQueryException(Properties.Resources.XQST0088);
                if (_moduleNs.Contains(targetNamespace.Data))
                    throw new XQueryException(Properties.Resources.XQST0047, targetNamespace.Data);
                if (rec.args[1] != null)
                    arr = Lisp.ToArray<Literal>(rec.args[1]);
                else
                    arr = _context.ResolveModuleImport(String.Empty, targetNamespace.Data);
            }
            else
            {
                Qname prefix = (Qname)rec.Arg0;
                targetNamespace = (Literal)rec.Arg1;
                if (targetNamespace.Data == "")
                    throw new XQueryException(Properties.Resources.XQST0088);
                if (_moduleNs.Contains(targetNamespace.Data))
                    throw new XQueryException(Properties.Resources.XQST0047, targetNamespace.Data);
                if (prefix.Name == "xml" || prefix.Name == "xmlns")
                    throw new XQueryException(Properties.Resources.XQST0070, targetNamespace.Data);
                if (rec.args[2] != null)
                    arr = Lisp.ToArray<Literal>(rec.args[2]);
                else
                    arr = _context.ResolveModuleImport(prefix.Name, targetNamespace.Data);
                if (_context.NamespaceManager.HasNamespace(prefix.Name))
                    throw new XQueryException(Properties.Resources.XQST0033, prefix.Name);
                _context.AddNamespace(prefix.Name, targetNamespace.Data);
            }
            for (int k = 0; k < arr.Length; k++)
            {
                Literal filename = (Literal)arr[k];
                TextReader reader;
                string absolutePath;
                if (Uri.IsWellFormedUriString(filename.Data, UriKind.Absolute) ||
                    (_context.BaseUri != null && Uri.IsWellFormedUriString(_context.BaseUri, UriKind.Absolute)))
                {
                    Uri uri;
                    WebClient client = new WebClient();
                    client.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    if (!Uri.TryCreate(filename.Data, UriKind.Absolute, out uri))
                        uri = new Uri(new Uri(_context.BaseUri), filename.Data);
                    absolutePath = uri.AbsoluteUri;
                    reader = new StringReader(client.DownloadString(uri));
                }
                else
                {
                    if (_context.BaseUri != null)
                        absolutePath = Path.Combine(_context.BaseUri, filename.Data);
                    else
                        absolutePath = filename.Data;
                    reader = new StreamReader(new FileStream(absolutePath,
                        FileMode.Open, FileAccess.Read));
                }
                TokenizerBase tok = new Tokenizer(reader.ReadToEnd());
                reader.Close();
                Notation notation2 = new Notation();
                YYParser parser = new YYParser(notation2);
                parser.yyparseSafe(tok);
                XQueryContext context2 = new XQueryContext(targetNamespace.Data, absolutePath, _context);
                Translator translator = new Translator(context2);
                translator.PreProcess(notation2);
                translator.Process(notation2);
                _varTable.ImportVariables(context2);
                _context.FunctionTable.CopyFrom(context2.FunctionTable, context2);
                context2.Close();
            }
            _moduleNs.Add(targetNamespace.Data);
        }

        private void ProcessDefaultCollation(Notation notation, Notation.Record rec)
        {
            if (_defaultCollationDecl)
                throw new XQueryException(Properties.Resources.XQST0038);
            Literal lit = (Literal)rec.Arg0;
            if (!String.IsNullOrEmpty(_context.BaseUri))
                _context.DefaultCollation = new Uri(new Uri(_context.BaseUri), lit.Data).ToString();
            else
                _context.DefaultCollation = lit.Data;
            _context.GetCulture(_context.DefaultCollation);
            _defaultCollationDecl = true;
        }

        private void ProcessDefaultOrder(Notation notation, Notation.Record rec)
        {
            if (_defaultOrderDecl)
                throw new XQueryException(Properties.Resources.XQST0069);
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.EMPTY_GREATEST:
                    _context.EmptyOrderSpec = XQueryEmptyOrderSpec.Greatest;
                    break;
                case Token.EMPTY_LEAST:
                    _context.EmptyOrderSpec = XQueryEmptyOrderSpec.Least;
                    break;
            }
            _defaultOrderDecl = true;
        }

        private void ProcessOrdering(Notation notation, Notation.Record rec)
        {
            if (_defaultOrderingDecl)
                throw new XQueryException(Properties.Resources.XQST0065);
            TokenWrapper wrapper = (TokenWrapper)rec.Arg0;
            switch (wrapper.Data)
            {
                case Token.ORDERED:
                    _context.DefaultOrdering = XQueryOrder.Ordered;
                    break;

                case Token.UNORDERED:
                    _context.DefaultOrdering = XQueryOrder.Unordered;
                    break;
            }
            _defaultOrderingDecl = true;
        }

        private void ProcessDefaultElementNS(Notation notation, Notation.Record rec)
        {
            if (_defaultElementNsDecl)
                throw new XQueryException(Properties.Resources.XQST0066);
            Literal uri = (Literal)rec.Arg0;
            _context.NamespaceManager.AddNamespace("", uri.Data);
            _defaultElementNsDecl = true;
        }

        private void ProcessDefaultFunctionNS(Notation notation, Notation.Record rec)
        {
            if (_defaultFunctionNsDecl)
                throw new XQueryException(Properties.Resources.XQST0066);
            Literal uri = (Literal)rec.Arg0;
            _context.DefaultFunctionNS = uri.Data;
            _defaultFunctionNsDecl = true;
        }

        private void ProcessNamespace(Notation notation, Notation.Record rec)
        {
            Qname prefix = (Qname)rec.Arg0;
            Literal uri = (Literal)rec.Arg1;
            if (prefix.Name == "xml" || prefix.Name == "xmlns" || uri.Data == XmlReservedNs.NsXml)
                throw new XQueryException(Properties.Resources.XQST0070, uri.Data);
            if (_context.NamespaceManager.HasNamespace(prefix.Name))
                throw new XQueryException(Properties.Resources.XQST0033, prefix.Name);            
            _context.AddNamespace(prefix.Name, uri.Data);
        }

        private void ProcessImportSchema(Notation notation, Notation.Record rec)
        {
            _context.NeedValidatedParser = true;
            Literal uri = (Literal)rec.Arg1;
            if (rec.Arg0 != null)
            {
                Notation.Record[] recs = notation.Select(rec.Arg0, Descriptor.Namespace, 1);
                if (recs.Length > 0)
                {
                    Qname prefix = (Qname)recs[0].Arg0;
                    if (_context.NamespaceManager.HasNamespace(prefix.Name))
                        throw new XQueryException(Properties.Resources.XQST0033, prefix.Name);
                    _context.AddNamespace(prefix.Name, uri.Data);
                }
                else
                {
                    recs = notation.Select(rec.Arg0, Descriptor.DefaultElement, 0);
                    if (recs.Length > 0)
                    {
                        if (_context.NamespaceManager.DefaultNamespace != "")
                            throw new XQueryException(Properties.Resources.XQST0066);
                        _context.NamespaceManager.AddNamespace("", uri.Data);
                    }
                    else
                        throw new InvalidOperationException();
                }
            }
            if (_schemaNs.IndexOf(uri.Data) != -1)
                throw new XQueryException(Properties.Resources.XQST0058, uri.Data);
            _schemaNs.Add(uri.Data);
            Literal[] arr;
            if (rec.args[2] != null)
                arr = Lisp.ToArray<Literal>(rec.args[2]);
            else
                arr = _context.ResolveSchemaImport(String.Empty, uri.Data);
            for (int k = 0; k < arr.Length; k++)
            {
                string filename;
                if (_context.uri != null)
                {
                    Uri targetUri = new Uri(new Uri(_context.uri), arr[k].Data);
                    if (targetUri.Scheme == "file")
                        filename = targetUri.LocalPath;
                    else
                        filename = targetUri.AbsoluteUri;
                }
                else
                {
                    filename = _context.GetFileName(arr[k].Data);
                    if (filename == null)
                        throw new XQueryException(Properties.Resources.FileNotFound, filename);
                }
                try
                {
                    _context.schemaSet.Add(uri.Data, filename);
                }
                catch (WebException ex)
                {
                    throw new XQueryException(ex.Message, ex);
                }
            }
        }

        private void ProcessVarDecl(Notation notation, Notation.Record rec)
        {
            VarName name = (VarName)rec.Arg0;
            object var = ProcessVarName(name);
            if (_context.Engine.TryGet(var, true, false) != null)
                throw new XQueryException(Properties.Resources.XQST0049, name);            
            XQuerySequenceType varType;            
            if (rec.Arg1 == null)
                varType = XQuerySequenceType.Item;
            else
                varType = ProcessTypeDecl(notation, rec.Arg1);
            bool external = false;
            SymbolLink valueLink;
            if (rec.args.Length > 2)
            {
                object valueExpr = ProcessExprSingle(notation, rec.Arg2);
                if (varType == XQuerySequenceType.Item)
                {
                    XQuerySequenceType type = EvalExprType(valueExpr);
                    if (type.Cardinality == XmlTypeCardinality.One)
                        varType = type;
                }
                valueLink = new SymbolLink(varType.ValueType);
                _context.AddVariable(var, valueExpr, varType, valueLink);
            }
            else
            {
                valueLink = new SymbolLink(varType.ValueType);                
                _context.AddExternalVariable(var, varType);
                external = true;
            }
            valueLink.Value = Undefined.Value;
            _context.Engine.Set(var, valueLink);
            _varTable.PushVar(var, varType, external);
        }

        private void PreProcessFuncDecl(Notation notation, Notation.Record rec)
        {
            Qname qname = (Qname)rec.Arg0;
            XmlQualifiedName identity = QNameParser.Parse(qname.Name, _context.NamespaceManager, _context.nameTable);
            string ns = identity.Namespace;
            if (ns == String.Empty)
            {
                if (_context.DefaultFunctionNS == null)
                    ns = XmlReservedNs.NsXQueryFunc;
                else
                    ns = _context.DefaultFunctionNS;
            }
            if (ns == String.Empty ||
                ns == XmlReservedNs.NsXml ||
                ns == XmlReservedNs.NsXQueryFunc ||
                ns == XmlReservedNs.NsXs ||
                ns == XmlReservedNs.NsXsi)
                throw new XQueryException(Properties.Resources.XQST0045, identity.Name, identity.Namespace);
            object f = Lisp.Defatom(ns, new string[] { identity.Name }, false);
            FunctionParameter[] parameters = ProcessParamList(notation, rec.args[1]);
            if (_context.FunctionTable.IsRegistered(f, parameters.Length))
                throw new XQueryException(Properties.Resources.XQST0034, identity.Name, identity.Namespace);            
            XQuerySequenceType[] parameterTypes = new XQuerySequenceType[parameters.Length];
            for (int k = 0; k < parameters.Length; k++)
                parameterTypes[k] = parameters[k].type;
            FunctionDecl decl = new FunctionDecl(f, identity, parameters); 
            if (rec.args.Length > 3)
            {
                decl.returnType = new XQuerySequenceType(ProcessTypeDecl(notation, rec.Arg2));
                if (rec.args[3] == null) // external
                    throw new NotImplementedException();
                else
                    _context.FunctionTable.Register(f, _context, parameterTypes, decl.returnType);
            }
            else
            {
                if (rec.args[2] == null) // external
                    throw new NotImplementedException();
                else
                    decl.returnType = XQuerySequenceType.Item;
                _context.FunctionTable.Register(f, _context, parameterTypes, decl.returnType);
            }
            _fdecl.Add(rec.sym, decl);
        }

        private void ProcessFuncDecl(Notation notation, Notation.Record rec)
        {
            int stack_pos = _varTable.BeginFrame();
            FunctionDecl decl = _fdecl[rec.sym];
            Executive.Parameter[] executiveParameters = new Executive.Parameter[decl.parameters.Length];
            for (int k = 0; k < decl.parameters.Length; k++)
            {
                FunctionParameter p = decl.parameters[k];                
                executiveParameters[k].ID = p.id;
                executiveParameters[k].Type = p.type.ValueType;
                executiveParameters[k].VariableParam = false;
                _varTable.PushVar(p.id, p.type);
            }
            if (rec.args.Length > 3)
            {
                if (rec.args[3] == null) // external
                    throw new NotImplementedException();
                else
                {
                    XQueryExpr expr = (XQueryExpr)ProcessExpr(notation, rec.args[3]);
                    object body = expr.ToLispFunction();
                    if (decl.returnType != XQuerySequenceType.Item)
                    {
                        XQuerySequenceType type = EvalExprType(body);
                        if (!decl.returnType.Equals(type))
                            body = Lisp.List(ID.CastArg, body, decl.returnType);
                    }
                    LambdaExpr lambdaExpr = new LambdaExpr(decl.f, executiveParameters, typeof(System.Object), body);
                    _context.Engine.Defun(lambdaExpr);
                }
            }
            else
            {
                if (rec.args[2] == null) // external
                    throw new NotImplementedException();
                else
                {
                    XQueryExpr expr = (XQueryExpr)ProcessExpr(notation, rec.args[2]);
                    LambdaExpr lambdaExpr = new LambdaExpr(decl.f, executiveParameters, typeof(System.Object),
                        expr.ToLispFunction());
                    _context.Engine.Defun(lambdaExpr);
                }

            }
            _varTable.EndFrame(stack_pos);
        }

        private FunctionParameter[] ProcessParamList(Notation notation, object p)
        {
            List<FunctionParameter> res = new List<FunctionParameter>();            
            if (p != null)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(p);
                HashSet<VarName> hs = new HashSet<VarName>();
                for (int k = 0; k < arr.Length; k++)
                {
                    FunctionParameter parameter = new FunctionParameter();
                    VarName name = (VarName)arr[k];
                    if (hs.Contains(name))
                        throw new XQueryException(Properties.Resources.XQST0039, name);
                    hs.Add(name);
                    parameter.id = ProcessVarName(name);
                    Notation.Record[] recs = notation.Select(arr[k], Descriptor.TypeDecl, 1);
                    if (recs.Length > 0)
                        parameter.type = ProcessTypeDecl(notation, recs[0].Arg0);
                    else
                        parameter.type = XQuerySequenceType.Item;
                    res.Add(parameter);
                }
            }
            return res.ToArray();
        }

        private void ProcessOptionDecl(Notation notation, Notation.Record rec)
        {
            QNameValue qn = QNameValue.Parse(((Qname)rec.Arg0).Name, _context.NamespaceManager, "");            
            if (qn.Prefix == "")
                throw new XQueryException(Properties.Resources.ExpectedQNamePrefix, "option", qn.ToString());
            Literal lit = (Literal)rec.Arg1;
            _context.Option[qn.ToQualifiedName()] = lit.Data;
        }

        private XQueryExprBase ProcessExpr(Notation notation, object p)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(p);
            object[] expr = new object[arr.Length];
            for (int k = 0; k < arr.Length; k++)
                expr[k] = ProcessExprSingle(notation, arr[k]);
            if (expr.Length == 1 && Lisp.IsFunctor(expr[0], ID.DynExecuteExpr, 1))
                return (XQueryExprBase)Lisp.Arg1(expr[0]);
            return new XQueryExpr(_context, expr);
        }

        private object ProcessExprSingle(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym);
            switch (recs[0].descriptor)
            {
                case Descriptor.FLWORExpr:
                    return ProcessFLORExpr(notation, recs[0]);

                case Descriptor.Some:
                case Descriptor.Every:
                    return ProcessQuantifiedExpr(notation, recs[0]);

                case Descriptor.Typeswitch:
                    return ProcessTypeswitchExpr(notation, recs[0]);

                case Descriptor.If:
                    return ProcessIfExpr(notation, recs[0]);                    

                default:
                    return ProcessOrExpr(notation, sym);
            }            
        }

        private object ProcessFLORExpr(Notation notation, Notation.Record rec)
        {                        
            int stack_pos = _varTable.BeginFrame();
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            List<FLWORItem> flworItems = new List<FLWORItem>();
            bool stable = false;
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k]);
                switch (recs[0].descriptor)
                {
                    case Descriptor.For:
                        {
                            Symbol[] arr2 = Lisp.ToArray<Symbol>(recs[0].args[0]);
                            for (int s = 0; s < arr2.Length; s++)
                            {
                                Notation.Record[] recs2 = notation.Select(arr2[s], 
                                    Descriptor.ForClauseOperator, 4);
                                if (recs.Length > 0)
                                {
                                    FLWORItem item = new FLWORItem();
                                    item.desc = Descriptor.For;
                                    VarName name = (VarName)recs2[0].Arg0;
                                    item.var = ProcessVarName(name);
                                    item.assignExpr = ProcessExprSingle(notation, recs2[0].Arg3);
                                    item.convert = true;
                                    if (recs2[0].Arg1 == null)
                                    {
                                        XQuerySequenceType type = EvalExprType(item.assignExpr);
                                        if (type.TypeCode != XmlTypeCode.Item)
                                        {
                                            item.varType = new XQuerySequenceType(type.TypeCode, XmlTypeCardinality.One);
                                            item.convert = false;
                                        }
                                        else
                                            item.varType = XQuerySequenceType.Item;
                                    }
                                    else
                                        item.varType = ProcessTypeDecl(notation, recs2[0].Arg1);
                                    item.pos = null;
                                    if (recs2[0].Arg2 != null)
                                    {
                                        VarName posname = (VarName)recs2[0].Arg2;
                                        if (posname.Name == name.Name)
                                            throw new XQueryException(Properties.Resources.XQST0089, posname.Name);
                                        item.pos = ProcessVarName(posname);
                                    }                                    
                                    _varTable.PushVar(item.var, item.varType);
                                    if (item.pos != null)
                                        _varTable.PushVar(item.pos, new XQuerySequenceType(XmlTypeCode.Integer));
                                    flworItems.Add(item);
                                }
                            }
                        }
                        break;

                    case Descriptor.Let:
                        {
                            Symbol[] arr2 = Lisp.ToArray<Symbol>(recs[0].args[0]);
                            for (int s = 0; s < arr2.Length; s++)
                            {
                                Notation.Record[] recs2 = notation.Select(arr2[s],
                                    Descriptor.LetClauseOperator, 3);
                                if (recs.Length > 0)
                                {
                                    FLWORItem item = new FLWORItem();
                                    item.desc = Descriptor.Let;
                                    item.var = ProcessVarName((VarName)recs2[0].Arg0);
                                    item.assignExpr = ProcessExprSingle(notation, recs2[0].Arg2);
                                    item.convert = true;                                    
                                    if (recs2[0].Arg1 == null)
                                    {
                                        XQuerySequenceType type = EvalExprType(item.assignExpr);
                                        if (type.Cardinality == XmlTypeCardinality.One)
                                        {
                                            item.varType = type;
                                            item.convert = false;
                                        }
                                        else
                                            item.varType = XQuerySequenceType.Item;
                                    }
                                    else
                                        item.varType = ProcessTypeDecl(notation, recs2[0].Arg1);
                                    _varTable.PushVar(item.var, item.varType);
                                    flworItems.Add(item);
                                }
                            }
                        }
                        break;
                }
            }
            XQueryOrderSpec[] orderSpec = null;
            XQueryExprBase expr = new XQueryExpr(_context, 
                new object[] { ProcessExprSingle(notation, rec.Arg3) });
            if (rec.Arg2 != null)
            {
                Notation.Record[] recs = notation.Select(rec.Arg2, new Descriptor[] { 
                    Descriptor.OrderBy, Descriptor.StableOrderBy });
                if (recs.Length > 0)
                {
                    stable = (recs[0].descriptor == Descriptor.StableOrderBy);
                    Symbol[] arr3 = Lisp.ToArray<Symbol>(recs[0].args[0]);
                    orderSpec = new XQueryOrderSpec[arr3.Length];
                    object[] sortKey = new object[arr3.Length];
                    for (int k = 0; k < arr3.Length; k++)
                    {
                        sortKey[k] = ProcessExprSingle(notation, arr3[k]);
                        orderSpec[k].emptySpec = _context.EmptyOrderSpec;
                        orderSpec[k].collation = _context.DefaultCollation;
                        Notation.Record[] recs1 = notation.Select(arr3[k], Descriptor.Modifier, 1);
                        if (recs1.Length > 0)
                        {
                            Symbol[] modifier = Lisp.ToArray<Symbol>(recs1[0].args[0]);
                            if (modifier[0] != null)
                                switch (((TokenWrapper)modifier[0]).Data)
                                {
                                    case Token.ASCENDING:
                                        orderSpec[k].direction = XQueryOrderDirection.Ascending;
                                        break;
                                    case Token.DESCENDING:
                                        orderSpec[k].direction = XQueryOrderDirection.Descending;
                                        break;
                                }
                            if (modifier[1] != null)
                                switch (((TokenWrapper)modifier[1]).Data)
                                {
                                    case Token.EMPTY_GREATEST:
                                        orderSpec[k].emptySpec = XQueryEmptyOrderSpec.Greatest;
                                        break;
                                    case Token.EMPTY_LEAST:
                                        orderSpec[k].emptySpec = XQueryEmptyOrderSpec.Least;
                                        break;
                                }
                            if (modifier[2] != null)
                                orderSpec[k].collation = ((Literal)modifier[2]).Data;
                        }
                    }
                    ((XQueryExpr)expr).Annotation = sortKey;
                }
            }
            object whereExpr = null;
            if (rec.Arg1 != null)
            {
                Notation.Record[] recs3 = notation.Select(rec.Arg1, Descriptor.Where, 1);
                if (recs3.Length > 0)
                    whereExpr = ProcessExprSingle(notation, recs3[0].Arg0);
            }
            for (int k = flworItems.Count - 1; k >= 0; k--)
            {
                FLWORItem item = flworItems[k];                
                switch (item.desc)
                {
                    case Descriptor.For:
                        expr = new XQueryFLWOR(_context, item.var, item.varType, item.pos, item.assignExpr, expr, item.convert);
                        break;

                    case Descriptor.Let:
                        expr = new XQueryLET(_context, item.var, item.varType, item.assignExpr, expr, item.convert);
                        break;
                }
                if (k == flworItems.Count - 1)
                {
                    XQueryFLWORBase flworExpr = (XQueryFLWORBase)expr;
                    flworExpr.ConditionExpr = whereExpr;
                }
            }
            _varTable.EndFrame(stack_pos);
            if (orderSpec != null)
                expr = new XQuerySorter(_context, orderSpec, stable, expr);
            return Lisp.List(ID.DynExecuteExpr, expr, ID.Context, Lisp.ARGV);
        }

        private object ProcessQuantifiedExpr(Notation notation, Notation.Record rec)
        {
            int stack_pos = _varTable.BeginFrame();
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            List<FLWORItem> flworItems = new List<FLWORItem>();
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k], Descriptor.QuantifiedExprOper, 3);
                if (recs.Length > 0)
                {
                    FLWORItem item = new FLWORItem();
                    item.var = ProcessVarName((VarName)recs[0].Arg0);
                    item.assignExpr = ProcessExprSingle(notation, recs[0].Arg2);
                    item.convert = true;
                    if (recs[0].Arg1 == null)
                    {
                        XQuerySequenceType type = EvalExprType(item.assignExpr);
                        if (type.TypeCode != XmlTypeCode.Item)
                        {
                            item.varType = new XQuerySequenceType(type.TypeCode, XmlTypeCardinality.One);
                            item.convert = false;
                        }
                        else
                            item.varType = XQuerySequenceType.Item;
                    }
                    else
                        item.varType = ProcessTypeDecl(notation, recs[0].Arg1);                    
                    _varTable.PushVar(item.var, item.varType);
                    flworItems.Add(item);
                }
            }
            XQueryExprBase expr = new XQueryExpr(_context, 
                new object[] { Lisp.List(ID.BooleanValue, ProcessExprSingle(notation, rec.Arg1)) });
            for (int k = flworItems.Count - 1; k >= 0; k--)
            {
                FLWORItem item = flworItems[k];
                expr = new XQueryFLWOR(_context, item.var, item.varType, null, item.assignExpr, expr, item.convert);
            }
            _varTable.EndFrame(stack_pos);
            switch (rec.descriptor)
            {
                case Descriptor.Some:
                    return Lisp.List(ID.Some, expr.ToLispFunction());

                case Descriptor.Every:
                    return Lisp.List(ID.Every, expr.ToLispFunction());

                default:
                    throw new InvalidOperationException();
            }            
        }

        private object ProcessIfExpr(Notation notation, Notation.Record rec)
        {
            object cond = ProcessExprList(notation, rec.args[0]);
            if (!IsBooleanFunctor(cond))
                cond = Lisp.List(ID.BooleanValue, cond);
            return Lisp.List(Funcs.If, cond,
                    ProcessExprSingle(notation, rec.Arg1), 
                    ProcessExprSingle(notation, rec.Arg2));
        }

        private object ProcessTypeswitchExpr(Notation notation, Notation.Record rec)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[1]);
            object[] branch = new object[arr.Length + 1];
            object x = Lisp.Defatom("_expr");
            for (int k = 0; k < arr.Length; k++)
            {
                Notation.Record[] recs = notation.Select(arr[k], new Descriptor[] { Descriptor.Case });
                if (recs.Length > 0)
                {
                    if (recs[0].args.Length > 2)
                    {
                        object var = ProcessVarName((VarName)recs[0].Arg0);
                        XQuerySequenceType seqtype = ProcessTypeDecl(notation, recs[0].Arg1);
                        int stack_pos = _varTable.BeginFrame();
                        _varTable.PushVar(var, seqtype);
                        XQueryExprBase expr = new XQueryExpr(_context, 
                            new object[] { ProcessExprSingle(notation, recs[0].Arg2) });
                        _varTable.EndFrame(stack_pos);
                        branch[k] = Lisp.List(Lisp.List(ID.InstanceOf, x, seqtype),
                            Lisp.List(ID.DynExecuteExpr, new XQueryLET(_context, var, seqtype, x, expr, true), ID.Context, Lisp.ARGV));
                    }
                    else
                        branch[k] = Lisp.List(Lisp.List(ID.InstanceOf, x, ProcessTypeDecl(notation, recs[0].Arg0)),
                            ProcessExprSingle(notation, recs[0].Arg1));
                }
            }
            if (rec.args.Length > 3)
            {
                object var = ProcessVarName((VarName)rec.Arg2);
                int stack_pos = _varTable.BeginFrame();
                _varTable.PushVar(var, XQuerySequenceType.Item);
                XQueryExprBase expr = new XQueryExpr(_context, 
                    new object[] { ProcessExprSingle(notation, rec.Arg3) });
                _varTable.EndFrame(stack_pos);
                branch[arr.Length] = Lisp.List(Lisp.T, Lisp.List(ID.DynExecuteExpr, 
                    new XQueryLET(_context, var, XQuerySequenceType.Item, x, expr, true), ID.Context, Lisp.ARGV));
            }
            else
                branch[arr.Length] = Lisp.List(Lisp.T, ProcessExprSingle(notation, rec.Arg2));
            object res = new XQueryLET(_context, x, XQuerySequenceType.Item, 
                ProcessExpr(notation, rec.args[0]).ToLispFunction(), 
                    new XQueryExpr(_context, new object[] { Lisp.Append(Lisp.Cons(Funcs.Cond), Lisp.List(branch)) }), true).ToLispFunction();
            return res;
        }

        private object ProcessOrExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Or, 2);
            if (recs.Length > 0)
                return Lisp.List(Funcs.Or, CompileBooleanConvertion(ProcessOrExpr(notation, recs[0].Arg0)), 
                    CompileBooleanConvertion(ProcessAndExpr(notation, recs[0].Arg1)));
            else
                return ProcessAndExpr(notation, sym);
        }

        private object ProcessAndExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.And, 2);
            if (recs.Length > 0)
                return Lisp.List(Funcs.And, CompileBooleanConvertion(ProcessOrExpr(notation, recs[0].Arg0)),
                    CompileBooleanConvertion(ProcessComparisonExpr(notation, recs[0].Arg1)));
            else
                return ProcessComparisonExpr(notation, sym);
        }

        private object ProcessComparisonExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.ValueComp, 
                Descriptor.GeneralComp, Descriptor.NodeComp }, 3);
            if (recs.Length > 0)
            {
                object arg0 = ProcessRangeExpr(notation, recs[0].Arg0);
                switch (recs[0].descriptor)
                {
                    case Descriptor.ValueComp:
                        {
                            TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                            switch (w.Data)
                            {
                                case Token.EQ:
                                    return Lisp.List(Funcs.Eq, CompileAtomizeValueExpr(arg0),
                                        CompileAtomizeValueExpr(ProcessRangeExpr(notation, recs[0].Arg2)));

                                case Token.NE:
                                    return Lisp.List(Funcs.Ne, CompileAtomizeValueExpr(arg0),
                                        CompileAtomizeValueExpr(ProcessRangeExpr(notation, recs[0].Arg2)));

                                case Token.LT:
                                    return Lisp.List(Funcs.Lt, CompileAtomizeValueExpr(arg0),
                                        CompileAtomizeValueExpr(ProcessRangeExpr(notation, recs[0].Arg2)));

                                case Token.LE:
                                    return Lisp.List(Funcs.Le, CompileAtomizeValueExpr(arg0),
                                        CompileAtomizeValueExpr(ProcessRangeExpr(notation, recs[0].Arg2)));

                                case Token.GT:
                                    return Lisp.List(Funcs.Gt, CompileAtomizeValueExpr(arg0),
                                        CompileAtomizeValueExpr(ProcessRangeExpr(notation, recs[0].Arg2)));

                                case Token.GE:
                                    return Lisp.List(Funcs.Ge, CompileAtomizeValueExpr(arg0),
                                        CompileAtomizeValueExpr(ProcessRangeExpr(notation, recs[0].Arg2)));

                                default:
                                    throw new InvalidOperationException();
                            }
                        }

                    case Descriptor.GeneralComp:
                        {
                            Literal lit = (Literal)recs[0].Arg1;
                            if (lit.Data == "=")
                                return Lisp.List(ID.GeneralEQ, arg0,
                                    ProcessRangeExpr(notation, recs[0].Arg2));
                            else if (lit.Data == "!=")
                                return Lisp.List(ID.GeneralNE, arg0,
                                    ProcessRangeExpr(notation, recs[0].Arg2));
                            else if (lit.Data == "<")
                                return Lisp.List(ID.GeneralLT, arg0,
                                    ProcessRangeExpr(notation, recs[0].Arg2));
                            else if (lit.Data == "<=")
                                return Lisp.List(ID.GeneralLE, arg0,
                                    ProcessRangeExpr(notation, recs[0].Arg2));
                            else if (lit.Data == ">")
                                return Lisp.List(ID.GeneralGT, arg0,
                                    ProcessRangeExpr(notation, recs[0].Arg2));
                            else if (lit.Data == ">=")
                                return Lisp.List(ID.GeneralGE, arg0,
                                    ProcessRangeExpr(notation, recs[0].Arg2));
                            else
                                throw new InvalidOperationException();
                        }                        

                    case Descriptor.NodeComp:
                        {
                            if (recs[0].Arg1.Tag == Tag.TokenWrapper)
                            {
                                if (((TokenWrapper)recs[0].Arg1).Data == Token.IS)
                                    return Lisp.List(ID.SameNode, Lisp.List(ID.NodeValue, arg0),
                                        Lisp.List(ID.NodeValue, ProcessRangeExpr(notation, recs[0].Arg2)));
                            }
                            else
                            {
                                Literal lit = (Literal)recs[0].Arg1;
                                if (lit.Data == "<<")
                                    return Lisp.List(ID.PrecedingNode, Lisp.List(ID.NodeValue, arg0),
                                        Lisp.List(ID.NodeValue, ProcessRangeExpr(notation, recs[0].Arg2)));
                                else if (lit.Data == ">>")
                                    return Lisp.List(ID.FollowingNode, Lisp.List(ID.NodeValue, arg0),
                                        Lisp.List(ID.NodeValue, ProcessRangeExpr(notation, recs[0].Arg2)));
                            }
                        }
                        goto default;

                    default:
                        throw new InvalidOperationException();               
                }
            }
            else
                return ProcessRangeExpr(notation, sym);
        }

        private object ProcessRangeExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Range, 2);
            if (recs.Length > 0)
                return Lisp.List(ID.Range,
                    ProcessAdditiveExpr(notation, recs[0].Arg0),
                    ProcessAdditiveExpr(notation, recs[0].Arg1));
            else
                return ProcessAdditiveExpr(notation, sym);
        }

        private object ProcessAdditiveExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Add, 3);
            if (recs.Length > 0)
            {
                TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                if (w.Data == '+')
                    return Lisp.List(Funcs.Add, 
                        CompileAtomizeNumberExpr(ProcessAdditiveExpr(notation, recs[0].Arg0)),
                        CompileAtomizeNumberExpr(ProcessMultiplicativeExpr(notation, recs[0].Arg2)));
                else
                    return Lisp.List(Funcs.Sub,
                        CompileAtomizeNumberExpr(ProcessAdditiveExpr(notation, recs[0].Arg0)),
                        CompileAtomizeNumberExpr(ProcessMultiplicativeExpr(notation, recs[0].Arg2)));
            }
            else
                return ProcessMultiplicativeExpr(notation, sym);
        }

        private object ProcessMultiplicativeExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Mul, 3);
            if (recs.Length > 0)
            {
                TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                switch (w.Data)
                {
                    case Token.ML:
                        return Lisp.List(Funcs.Mul, CompileAtomizeNumberExpr(ProcessMultiplicativeExpr(notation, recs[0].Arg0)),
                            CompileAtomizeNumberExpr(ProcessUnionExpr(notation, recs[0].Arg2)));

                    case Token.DIV:
                        return Lisp.List(Funcs.Div, CompileAtomizeNumberExpr(ProcessMultiplicativeExpr(notation, recs[0].Arg0)),
                            CompileAtomizeNumberExpr(ProcessUnionExpr(notation, recs[0].Arg2)));

                    case Token.IDIV:
                        return Lisp.List(Funcs.IDiv, CompileAtomizeNumberExpr(ProcessMultiplicativeExpr(notation, recs[0].Arg0)),
                            CompileAtomizeNumberExpr(ProcessUnionExpr(notation, recs[0].Arg2)));

                    case Token.MOD:
                        return Lisp.List(Funcs.Mod, CompileAtomizeNumberExpr(ProcessMultiplicativeExpr(notation, recs[0].Arg0)),
                            CompileAtomizeNumberExpr(ProcessUnionExpr(notation, recs[0].Arg2)));

                    default:
                        throw new InvalidOperationException();
                }            
            }
            else
                return ProcessUnionExpr(notation, sym);
        }

        private object ProcessUnionExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, 
                new Descriptor[] { Descriptor.Union, Descriptor.Concatenate },  2);
            if (recs.Length > 0)
                return Lisp.List(ID.Union, _context.IsOrdered, ProcessUnionExpr(notation, recs[0].Arg0),
                    ProcessIntersectExceptExpr(notation, recs[0].Arg1));
            else
                return ProcessIntersectExceptExpr(notation, sym);
        }

        private object ProcessIntersectExceptExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.IntersectExcept, 3);
            if (recs.Length > 0)
            {                
                TokenWrapper w = (TokenWrapper)recs[0].Arg1;
                switch (w.Data)
                {
                    case Token.INTERSECT:
                        return Lisp.List(ID.Intersect, _context.IsOrdered, ProcessIntersectExceptExpr(notation, recs[0].Arg0), 
                            ProcessInstanceofExpr(notation, recs[0].Arg2));

                    case Token.EXCEPT:
                        return Lisp.List(ID.Except, _context.IsOrdered, ProcessIntersectExceptExpr(notation, recs[0].Arg0), 
                            ProcessInstanceofExpr(notation, recs[0].Arg2));
                    
                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                return ProcessInstanceofExpr(notation, sym);
        }

        private object ProcessInstanceofExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.InstanceOf, 2);
            if (recs.Length > 0)
                return Lisp.List(ID.InstanceOf,
                    Lisp.List(Funcs.LambdaQuote, ProcessTreatExpr(notation, recs[0].Arg0)), ProcessTypeDecl(notation, recs[0].Arg1));
            else
                return ProcessTreatExpr(notation, sym);
        }

        private object ProcessTreatExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.TreatAs, 2);
            if (recs.Length > 0)
            {
                XQuerySequenceType destType = ProcessTypeDecl(notation, recs[0].Arg1);
                object expr = Lisp.List(ID.TreatAs, 
                    ProcessCastableExpr(notation, recs[0].Arg0), destType);
                Type type = destType.ValueType;
                if (type != typeof(System.Object))
                    return Lisp.List(Funcs.Cast, expr, type);
                return expr;
            }
            else
                return ProcessCastableExpr(notation, sym);
        }

        private object ProcessCastableExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.CastableAs, 2);
            if (recs.Length > 0)
            {
                XQuerySequenceType destType = ProcessTypeDecl(notation, recs[0].Arg1);
                if (destType == null)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:untyped");
                if (destType.SchemaType == XQuerySequenceType.XmlSchema.AnyType)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:anyType");
                if (destType.SchemaType == XQuerySequenceType.XmlSchema.AnySimpleType)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:anySimpleType");
                if (destType.TypeCode == XmlTypeCode.AnyAtomicType)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:anyAtomicType");
                if (destType.TypeCode == XmlTypeCode.Notation)
                    throw new XQueryException(Properties.Resources.XPST0080, destType);
                if (destType.Cardinality == XmlTypeCardinality.ZeroOrMore || destType.Cardinality == XmlTypeCardinality.OneOrMore)
                    throw new XQueryException(Properties.Resources.XPST0080, destType);
                object expr = ProcessCastExpr(notation, recs[0].Arg0);
                return Lisp.List(ID.Castable, expr, destType, expr.GetType());
            }
            else
                return ProcessCastExpr(notation, sym);
        }

        private object ProcessCastExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.CastAs, 2);
            if (recs.Length > 0)
            {
                XQuerySequenceType destType = ProcessTypeDecl(notation, recs[0].Arg1);
                if (destType == null)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:untyped");
                if (destType.SchemaType == XQuerySequenceType.XmlSchema.AnyType)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:anyType");
                if (destType.SchemaType == XQuerySequenceType.XmlSchema.AnySimpleType)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:anySimpleType");
                if (destType.TypeCode == XmlTypeCode.AnyAtomicType)
                    throw new XQueryException(Properties.Resources.XPST0051, "xs:anyAtomicType");
                if (destType.TypeCode == XmlTypeCode.Notation)
                    throw new XQueryException(Properties.Resources.XPST0080, destType);
                if (destType.Cardinality == XmlTypeCardinality.ZeroOrMore || destType.Cardinality == XmlTypeCardinality.OneOrMore)
                    throw new XQueryException(Properties.Resources.XPST0080, destType);
                object expr = ProcessUnaryExpr(notation, recs[0].Arg0);
                return Lisp.List(ID.CastTo, expr, destType, expr.GetType());
            }
            else
                return ProcessUnaryExpr(notation, sym);
        }

        private object ProcessUnaryExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.Unary, 2);
            if (recs.Length > 0)
            {
                TokenWrapper[] arr = Lisp.ToArray<TokenWrapper>(recs[0].args[0]);
                object res = ProcessValueExpr(notation, recs[0].Arg1);
                foreach (TokenWrapper w in arr)
                    if (w.Data == '-')
                        res = Lisp.List(Funcs.Neg, CompileAtomizeNumberExpr(res));
                    else
                        res = Lisp.List(Funcs.Add, 0, CompileAtomizeNumberExpr(res));
                return res;
            }
            else
                return ProcessValueExpr(notation, sym);
        }

        private object ProcessValueExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym);
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.ExtensionExpr:
                        return ProcessExtensionExpr(notation, recs[0]);
                    
                    case Descriptor.Validate:
                        return ProcessValidateExpr(notation, recs[0]);

                    default:
                        return ProcessPathExpr(notation, sym);
                }
            }
            else
                throw new InvalidOperationException();
        }

        private object ProcessExtensionExpr(Notation notation, Notation.Record rec)
        {
            Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[0]);
            foreach (Symbol sym in arr)
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.Pragma, 2);
                if (recs.Length > 0)
                {                    
                    QNameValue qn = QNameValue.Parse(((Qname)recs[0].Arg0).Name, _context.NamespaceManager, "");
                    if (qn.Prefix == "")
                        throw new XQueryException(Properties.Resources.ExpectedQNamePrefix, "pragma", qn.ToString());
                    // ???
                    Literal lit = (Literal)recs[0].Arg1;
                }
            }            
            return ProcessExpr(notation, rec.args[1]).ToLispFunction();            
        }

        private object ProcessValidateExpr(Notation notation, Notation.Record rec)
        {
            bool lax = false;
            if (rec.Arg0 != null)
            {
                TokenWrapper w = (TokenWrapper)rec.Arg0;
                switch (w.Data)
                {
                    case Token.LAX:
                        lax = true;
                        break;

                    case Token.STRICT:
                        lax = false;
                        break;
                }
            }
            return Lisp.List(ID.Validate, Lisp.List(ID.DynExecuteExpr, 
                ProcessExpr(notation, rec.args[1]), ID.Context, Lisp.ARGV), lax);
        }

        private object ProcessPathExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Child, 
                Descriptor.Descendant }, 1);
            List<XQueryExprBase> steps = new List<XQueryExprBase>();
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.Child:
                        if (recs[0].Arg0 == null)
                            return Lisp.List(ID.Seq, Lisp.List(ID.Root, Lisp.List(ID.ContextNode, ID.Context)));
                        break;

                    case Descriptor.Descendant:
                        steps.Add(new XQueryStepExpr(XQuerySequenceType.Node,
                            XQueryPathExprType.DescendantOrSelf, _context));
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                ProcessRelativePathExpr(notation, recs[0].Arg0, steps);
            }
            else
                ProcessRelativePathExpr(notation, sym, steps);
            if (steps.Count == 1 && steps[0] is XQueryExpr)
            {
                XQueryExpr expr = (XQueryExpr)steps[0];
                return expr.ToLispFunction();
            }
            else
                return Lisp.List(ID.DynExecuteExpr,
                    new XQueryPathExpr(_context, steps.ToArray(), _context.IsOrdered), ID.Context, Lisp.ARGV);
        }

        private void ProcessRelativePathExpr(Notation notation, Symbol sym, List<XQueryExprBase> steps)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Child, 
                Descriptor.Descendant }, 2);
            if (recs.Length > 0)
            {
                ProcessRelativePathExpr(notation, recs[0].Arg0, steps);
                if (recs[0].descriptor == Descriptor.Descendant)
                  steps.Add(new XQueryStepExpr(XQuerySequenceType.Node,
                    XQueryPathExprType.DescendantOrSelf, _context));
                steps.Add(ProcessStepExpr(notation, recs[0].Arg1));
            }
            else
                steps.Add(ProcessStepExpr(notation, sym));
        }

        private XQueryExprBase ProcessStepExpr(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.AxisStep, 
                Descriptor.FilterExpr }, 1);
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.AxisStep:
                        return ProcessAxisStep(notation, recs[0]);

                    case Descriptor.FilterExpr:
                        return ProcessFilterExpr(notation, recs[0]);

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                throw new InvalidOperationException();
        }

        private XQueryExprBase ProcessAxisStep(Notation notation, Notation.Record rec)
        {
            if (rec.Arg0.Tag == Tag.TokenWrapper && ((TokenWrapper)rec.Arg0).Data == Token.DOUBLE_PERIOD)
                return new XQueryStepExpr(XQueryPathExprType.Parent, _context);
            else
            {
                Notation.Record[] recs = notation.Select(rec.Arg0, new Descriptor[] { Descriptor.ForwardStep,
                    Descriptor.AbbrevForward, Descriptor.ReverseStep });
                if (recs.Length > 0)
                {
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.ForwardStep:
                            return ProcessPredicateList(notation, rec.Arg0, 
                                ProcessForwardStep(notation, recs[0]));

                        case Descriptor.AbbrevForward:
                            return ProcessPredicateList(notation, rec.Arg0,
                                ProcessAbbrevForward(notation, recs[0]));

                        case Descriptor.ReverseStep:
                            return ProcessPredicateList(notation, rec.Arg0,
                                ProcessReverseStep(notation, recs[0]));

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                    return ProcessPredicateList(notation, rec.Arg0, new XQueryStepExpr(
                        ProcessNodeTest(notation, rec.Arg0, false), XQueryPathExprType.Child, _context));
            }
        }

        private XQueryExprBase ProcessForwardStep(Notation notation, Notation.Record rec)
        {
            TokenWrapper w = (TokenWrapper)rec.Arg0;
            switch (w.Data)
            {
                case Token.AXIS_CHILD:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false), 
                        XQueryPathExprType.Child, _context);

                case Token.AXIS_DESCENDANT:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false), 
                        XQueryPathExprType.Descendant, _context);

                case Token.AXIS_ATTRIBUTE:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, true), 
                        XQueryPathExprType.Attribute, _context);

                case Token.AXIS_SELF:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false), 
                        XQueryPathExprType.Self, _context);

                case Token.AXIS_DESCENDANT_OR_SELF:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false), 
                        XQueryPathExprType.DescendantOrSelf, _context);

                case Token.AXIS_FOLLOWING_SIBLING:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false), 
                        XQueryPathExprType.FollowingSibling, _context);

                case Token.AXIS_FOLLOWING:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false), 
                        XQueryPathExprType.Following, _context);

                case Token.AXIS_NAMESPACE:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, true), 
                        XQueryPathExprType.Namespace, _context);

                default:
                    throw new InvalidOperationException();
            }            
        }

        private XQueryExprBase ProcessAbbrevForward(Notation notation, Notation.Record rec)
        {
            return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg0, true), 
                XQueryPathExprType.Attribute, _context);
        }

        private XQueryExprBase ProcessReverseStep(Notation notation, Notation.Record rec)
        {
            TokenWrapper w = (TokenWrapper)rec.Arg0;
            switch (w.Data)
            {
                case Token.AXIS_PARENT:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false),
                        XQueryPathExprType.Parent, _context);

                case Token.AXIS_ANCESTOR:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false),
                        XQueryPathExprType.Ancestor, _context);

                case Token.AXIS_PRECEDING_SIBLING:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false),
                        XQueryPathExprType.PrecedingSibling, _context);

                case Token.AXIS_PRECEDING:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false),
                        XQueryPathExprType.Preceding, _context);

                case Token.AXIS_ANCESTOR_OR_SELF:
                    return new XQueryStepExpr(ProcessNodeTest(notation, rec.Arg1, false),
                        XQueryPathExprType.AncestorOrSelf, _context);
                
                default:
                    throw new InvalidOperationException();
            }            
        }
       
        private object ProcessNodeTest(Notation notation, Symbol sym, bool attr)
        {
            if (sym.Tag == Tag.TokenWrapper)
                return XmlQualifiedNameTest.New(null, null);
            else if (sym.Tag == Tag.Qname)
            {
                Qname qn = (Qname)sym;
                XmlQualifiedName qualifiedName = QNameParser.Parse(qn.Name, _context.NamespaceManager, 
                    attr ?  "" : _context.NamespaceManager.DefaultNamespace, _context.nameTable);
                return XmlQualifiedNameTest.New(qualifiedName.Name, qualifiedName.Namespace);
            }
            else
            {
                Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.Wildcard1, Descriptor.Wildcard2 }, 1);
                if (recs.Length > 0)
                {
                    Qname qname = (Qname)recs[0].Arg0;
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.Wildcard1:
                            {
                                string ns = _context.NamespaceManager.LookupNamespace(qname.Name);
                                if (ns == null)
                                    throw new XQueryException(Properties.Resources.XPST0081, qname.Name);
                                return XmlQualifiedNameTest.New(null, ns);
                            }

                        case Descriptor.Wildcard2:
                            return XmlQualifiedNameTest.New(_context.nameTable.Add(qname.Name), null);

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                    return ProcessKindTest(notation, sym);
            }
        }

        private XQueryExprBase ProcessPredicateList(Notation notation, Symbol sym, XQueryExprBase ancestor)
        {       
            Notation.Record[] recs = notation.Select(sym, Descriptor.PredicateList, 1);
            if (recs.Length > 0)
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[0]);
                List<XQueryExpr> filter = new List<XQueryExpr>();
                for (int k = 0; k < arr.Length; k++)
                {
                    Notation.Record[] recs1 = notation.Select(arr[k], Descriptor.Predicate, 1);
                    if (recs1.Length > 0)
                        filter.Add((XQueryExpr)ProcessExpr(notation, recs1[0].args[0]));
                }
                //XQueryStepExpr stepExpr = ancestor as XQueryStepExpr;
                //if (stepExpr != null && filter.Count == 1)
                //    stepExpr.Filter = filter[0];
                //else
                //{
                //    XQueryFilterExpr filterExpr = new XQueryFilterExpr(_context, filter.ToArray());
                //    filterExpr.SourceExpr = ancestor;
                //    return filterExpr;
                //}
                XQueryFilterExpr filterExpr = new XQueryFilterExpr(_context, filter.ToArray());
                filterExpr.SourceExpr = ancestor;
                return filterExpr;
            }
            return ancestor;
        }
        
        private XQueryExprBase ProcessFilterExpr(Notation notation, Notation.Record rec)
        {
            return ProcessPredicateList(notation, rec.Arg0, 
                new XQueryExpr(_context, new object[] { ProcessPrimaryExpr(notation, rec.Arg0) }));
        }

        private object ProcessPrimaryExpr(Notation notation, Symbol sym)
        {
            if (sym.Tag == Tag.Literal)
                return ((Literal)sym).Data;
            else if (sym.Tag == Tag.Integer)
                return ((IntegerValue)sym).Data;
            else if (sym.Tag == Tag.Double)
                return ((DoublelValue)sym).Data;
            else if (sym.Tag == Tag.Decimal)
                return ((DecimalValue)sym).Data;
            else if (sym.Tag == Tag.VarName)
            {
                XQuerySequenceType type;
                bool external;
                object var = ProcessVarName((VarName)sym);
                if (!_varTable.GetType(var, out type, out external))
                    throw new XQueryException(Properties.Resources.XPST0008, sym);
                if (external)
                    _context.MarkExternalVariable(var);
                return var;
            }
            else if (sym.Tag == Tag.TokenWrapper)
            {
                if (((TokenWrapper)sym).Data == '.')
                    return Lisp.List(ID.ContextNode, ID.Context);
                else
                    throw new InvalidOperationException();
            }
            else
            {
                Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.ParenthesizedExpr,
                    Descriptor.Ordered, Descriptor.Unordered, Descriptor.Funcall });
                if (recs.Length > 0)
                {
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.ParenthesizedExpr:
                            return ProcessParenthesizedExpr(notation, recs[0]);

                        case Descriptor.Ordered:
                        case Descriptor.Unordered:
                            return ProcessOrderedExpr(notation, recs[0]);

                        case Descriptor.Funcall:
                            return ProcessFuncallExpr(notation, recs[0]);

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                    return ProcessDirectConstructor(notation, sym);
            }
        }

        private object ProcessParenthesizedExpr(Notation notation, Notation.Record rec)
        {
            if (rec.args[0] == null)
                return Lisp.List(ID.DynExecuteExpr, 
                    new XQueryExpr(_context, new object[0]), ID.Context, Lisp.ARGV);
            else
                return Lisp.List(ID.Par, ProcessExprList(notation, rec.args[0]));
        }

        private object ProcessOrderedExpr(Notation notation, Notation.Record rec)
        {
            XQueryOrder order;
            switch (rec.descriptor)
            {
                case Descriptor.Unordered:
                    order = XQueryOrder.Unordered;
                    break;

                default:
                    order = XQueryOrder.Ordered;
                    break;
            }
            _context.EnterOrdering(order);
            try
            {
                return Lisp.List(ID.DynExecuteExpr, ProcessExpr(notation, rec.args[0]),
                    ID.Context, Lisp.ARGV);
            }
            finally
            {
                _context.LeaveOrdering();
            }
        }

        private object ProcessFuncallExpr(Notation notation, Notation.Record rec)
        {
            Qname qname = (Qname)rec.Arg0;
            XmlQualifiedName identity = QNameParser.Parse(qname.Name, _context.NamespaceManager, _context.nameTable);
            string ns = identity.Namespace;
            if (identity.Namespace == String.Empty)            
            {
                if (_context.DefaultFunctionNS == null)
                    ns = XmlReservedNs.NsXQueryFunc;
                else
                    ns = _context.DefaultFunctionNS;
            }
            object f = Lisp.Defatom(ns, new string[] { identity.Name }, false);
            object[] arg;
            if (rec.args[1] == null)
                arg = new object[1];
            else
            {
                Symbol[] arr = Lisp.ToArray<Symbol>(rec.args[1]);
                arg = new object[arr.Length + 1];
                for (int k = 0; k < arr.Length; k++)
                    arg[k + 1] = ProcessExprSingle(notation, arr[k]);                
            }
            if (arg.Length == 2)
            {
                XmlSchemaObject schemaType;
                if (TryProcessTypeName(notation, qname, false, out schemaType))
                {
                    XQuerySequenceType seqtype =
                        new XQuerySequenceType((XmlSchemaSimpleType)schemaType, XmlTypeCardinality.One, null);
                    if (seqtype == null)
                        throw new XQueryException(Properties.Resources.XPST0051, "untyped");
                    if (seqtype.TypeCode == XmlTypeCode.Notation)
                        throw new XQueryException(Properties.Resources.XPST0051, "NOTATION");
                    return Lisp.List(ID.CastToItem, arg[1], seqtype);
                }
            }
            arg[0] = f;
            XQueryFunctionRecord fr = _context.FunctionTable.GetRecord(f, arg.Length - 1);
            if (fr == null)
                throw new XQueryException(Properties.Resources.XPST0017,
                    identity.Name, arg.Length - 1, ns);
            if (fr.validationReader)
                _context.NeedValidatedParser = true;
            for (int s = 1; s < arg.Length; s++)
            {
                XQuerySequenceType seqtype;
                if (fr.variableParams)
                    seqtype = fr.parameters[0];
                else
                    seqtype = fr.parameters[s - 1];
                arg[s] = CompileConversion(arg[s], seqtype);
            }
            return Lisp.List(arg);
        }

        private object ProcessDirectConstructor(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.DirElemConstructor,
                Descriptor.DirCommentConstructor, Descriptor.DirPIConstructor });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.DirElemConstructor:
                        {
                            bool mapping = false;                            
                            object builder = Lisp.Defatom("b");
                            List<object> stmt = new List<object>();
                            stmt.Add(Funcs.Progn);
                            WriteDirElemConstructor(notation, recs[0], builder, stmt, ref mapping);
                            stmt.Add(Lisp.List(ID.CreateNavigator, builder));                            
                            object body = Lisp.List(stmt.ToArray());
                            object res;
                            if (mapping)
                                res = new XQueryLET(_context, builder, XQuerySequenceType.Item,
                                    Lisp.Cons(ID.CreateBuilder), new XQueryExpr(_context, new object[] { body }), false);
                            else
                                res = Lisp.List(Funcs.Let1, Lisp.Cons(Lisp.List(builder, Lisp.Cons(ID.CreateBuilder))), body);
                            Notation.Record[] recs1 = notation.Select(sym, Descriptor.MappingExpr, 1);
                            if (recs1.Length > 0) // Mapping extension support
                            {
                                if (!mapping)
                                    res = new XQueryExpr(_context, new object[] { res });
                                return new XQueryMapping(_context, ProcessPathExpr(notation, recs1[0].Arg0), 
                                    (XQueryExprBase)res, true).ToLispFunction();
                            }
                            else
                            {
                                if (mapping)
                                    return ((XQueryExprBase)res).ToLispFunction();
                                return res;
                            }
                        }

                    case Descriptor.DirCommentConstructor:
                        {
                            Literal lit = (Literal)recs[0].Arg0;
                            return Lisp.List(ID.CreateNavigator, Lisp.List(ID.WriteComment, Lisp.Cons(ID.CreateBuilder), lit.Data));
                        }

                    case Descriptor.DirPIConstructor:
                        {
                            Literal name = (Literal)recs[0].Arg0;
                            Literal data = (Literal)recs[0].Arg1;
                            return Lisp.List(ID.CreateNavigator, Lisp.List(ID.WritePi, Lisp.Cons(ID.CreateBuilder), name.Data,
                                data != null ? data.Data : String.Empty));
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                return ProcessComputedConstructor(notation, sym);
        }

        private void WriteDirElemConstructor(Notation notation, Notation.Record rec, object builder, List<object> stmt, ref bool mapping)
        {           
            string prefix;
            string localName;
            string ns;
            bool openScope = false;
            HashSet<XmlQualifiedName> hs = new HashSet<XmlQualifiedName>();
            foreach (Symbol sym in Lisp.getIterator<Symbol>(rec.args[1]))
                if (sym.Tag != Tag.Literal)
                {
                    Notation.Record[] recs = notation.Select(sym, Descriptor.DirAttribute, 5);
                    if (recs.Length > 0)
                    {
                        Qname name = (Qname)recs[0].Arg0;
                        QNameParser.Split(name.Name, out prefix, out localName);
                        if (prefix == "xmlns" || (prefix == "" && localName == "xmlns"))
                        {
                            if (!openScope)
                            {
                                _context.NamespaceManager.PushScope();
                                openScope = true;
                            }
                            Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[4]);
                            if (arr.Length != 1 || !(arr[0] is Literal))
                                throw new XQueryException(Properties.Resources.XQST0022, localName);
                            Literal targetNamespace = (Literal)arr[0];
                            if (prefix != "" && targetNamespace.Data == "")
                                throw new XQueryException(Properties.Resources.XQST0088);
                            if (localName == "xml" || (prefix == "xmlns" && localName == "xmlns"))
                                throw new XQueryException(Properties.Resources.XQST0070, targetNamespace.Data);
                            if (prefix == "" && localName == "xmlns")
                                _context.NamespaceManager.AddNamespace("", targetNamespace.Data);
                            else
                                _context.NamespaceManager.AddNamespace(localName, targetNamespace.Data);
                        }
                    }
                }
            ProcessElementQName((Qname)rec.Arg0, out prefix, out localName, out ns);
            stmt.Add(Lisp.List(ID.WriteBeginElement, builder, 
                new QNameValue(prefix, localName, ns, _context.nameTable)));
            if (rec.args[1] != null)
            {
                foreach (Symbol sym in Lisp.getIterator<Symbol>(rec.args[1]))
                {
                    if (sym.Tag != Tag.Literal)
                    {
                        Notation.Record[] recs = notation.Select(sym, Descriptor.DirAttribute, 5);
                        if (recs.Length > 0)
                        {
                            Qname name = (Qname)recs[0].Arg0;
                            ProcessAttributeQName(name, out prefix, out localName, out ns);
                            XmlQualifiedName qn = new XmlQualifiedName(localName, ns);
                            if (hs.Contains(qn))
                                throw new XQueryException(Properties.Resources.XQST0040, name);
                            hs.Add(qn);
                            if (prefix != "xmlns" && (prefix != "" || localName != "xmlns"))
                            {
                                stmt.Add(Lisp.List(ID.WriteBeginAttribute, builder, 
                                    new QNameValue(prefix, localName, ns, _context.nameTable)));
                                WriteDirAttributeValue(notation, recs[0].args[4], builder, stmt);
                                stmt.Add(Lisp.List(ID.WriteEndAttribute, builder));
                            }
                            else
                            {
                                Symbol[] arr = Lisp.ToArray<Symbol>(recs[0].args[4]);
                                Literal targetNamespace = (Literal)arr[0];
                                stmt.Add(Lisp.List(ID.WriteBeginAttribute, builder, 
                                    new QNameValue(prefix, localName, XmlReservedNs.NsXmlNs, _context.nameTable)));
                                stmt.Add(Lisp.List(ID.WriteString, builder, targetNamespace.Data));
                                stmt.Add(Lisp.List(ID.WriteEndAttribute, builder));
                            }
                        }
                    }
                }
            }
            if (rec.args.Length > 2 && rec.args[2] != null)
                foreach (Symbol sym in Lisp.getIterator<Symbol>(rec.args[2]))
                    WriteDirElemContent(notation, sym, builder, stmt, ref mapping);
            stmt.Add(Lisp.List(ID.WriteEndElement, builder));
            if (openScope)
                _context.NamespaceManager.PopScope();
        }

        private void WriteDirAttributeValue(Notation notation, object expr, object builder, List<object> stmt)
        {
            foreach (Symbol sym in Lisp.getIterator<Symbol>(expr))
                if (sym.Tag == Tag.TokenWrapper)
                {
                    TokenWrapper w = (TokenWrapper)sym;
                    switch (w.Data)
                    {
                        case Token.EscapeApos:
                            stmt.Add(Lisp.List(ID.WriteString, builder, "\'"));
                            break;

                        case Token.EscapeQuot:
                            stmt.Add(Lisp.List(ID.WriteString, builder, "\""));
                            break;
                    }
                }
                else
                    WriteCommonContent(notation, sym, builder, stmt, true);
        }

        private void WriteDirElemContent(Notation notation, Symbol sym, object builder, List<object> stmt, ref bool mapping)
        {
            if (sym.Tag == Tag.Literal)
            {
                Literal lit = (Literal)sym;
                if (XmlCharType.Instance.IsOnlyWhitespace(lit.Data))
                {
                    if (_context.PreserveBoundarySpace)
                        stmt.Add(Lisp.List(ID.WriteString, builder, lit.Data));
                }
                else
                    stmt.Add(Lisp.List(ID.WriteString, builder, lit.Data));
            }
            else if (sym.Tag == Tag.Constructor)
            {
                Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.DirElemConstructor,
                Descriptor.DirCommentConstructor, Descriptor.DirPIConstructor });
                if (recs.Length > 0)
                {
                    switch (recs[0].descriptor)
                    {
                        case Descriptor.DirElemConstructor:
                            {
                                Notation.Record[] recs1 = notation.Select(recs[0].sym, Descriptor.MappingExpr, 1);
                                if (recs1.Length > 0) // Mapping extension support
                                {
                                    List<object> group = new List<object>();
                                    group.Add(Funcs.Progn);
                                    WriteDirElemConstructor(notation, recs[0], builder, group, ref mapping);
                                    stmt.Add(new XQueryMapping(_context, ProcessPathExpr(notation, recs1[0].Arg0),
                                        new XQueryExpr(_context, new object[] { Lisp.List(group.ToArray()) }), false).ToLispFunction());
                                    mapping = true;
                                }
                                else
                                    WriteDirElemConstructor(notation, recs[0], builder, stmt, ref mapping);
                            }
                            break;

                        case Descriptor.DirCommentConstructor:
                            {
                                Literal lit = (Literal)recs[0].Arg0;
                                stmt.Add(Lisp.List(ID.WriteComment, builder, lit.Data));
                            }
                            break;

                        case Descriptor.DirPIConstructor:
                            {
                                Literal name = (Literal)recs[0].Arg0;
                                Literal data = (Literal)recs[0].Arg1;
                                stmt.Add(Lisp.List(ID.WritePi, builder, name.Data,
                                    data != null ? data.Data : String.Empty));
                            }
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                    throw new InvalidOperationException();
            }
            else
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.CDataSection, 1);
                if (recs.Length > 0)
                    stmt.Add(Lisp.List(ID.WriteCdata, builder, ((Literal)recs[0].Arg0).Data));
                else
                    WriteCommonContent(notation, sym, builder, stmt, false);
            }
        }

        private void WriteCommonContent(Notation notation, Symbol sym, object builder, List<object> stmt, bool attr)
        {
            if (sym.Tag == Tag.Literal)
            {
                string value = ((Literal)sym).Data;
                stmt.Add(Lisp.List(ID.WriteString, builder, value));
            }
            else if (sym.Tag == Tag.PredefinedEntityRef)
                stmt.Add(Lisp.List(ID.WriteString, builder, ((PredefinedEntityRef)sym).Data));
            else if (sym.Tag == Tag.CharRef)
            {
                if (sym is CharRefHex)
                {
                    CharRefHex charRef = (CharRefHex)sym;
                    stmt.Add(Lisp.List(ID.WriteString, builder, String.Format("&#x{0};", charRef.Data)));
                }
                else
                {
                    CharRef charRef = (CharRef)sym;
                    stmt.Add(Lisp.List(ID.WriteString, builder, String.Format("&#{0};", charRef.Data)));
                }
            }
            else
            {
                Notation.Record[] recs = notation.Select(sym, Descriptor.EnclosedExpr, 1);
                if (recs.Length > 0)
                {
                    object expr = ProcessExprList(notation, recs[0].args[0]);
                    if (attr)
                        stmt.Add(Lisp.List(ID.WriteString, builder, Lisp.List(ID.FormatValue, expr)));
                    else
                        stmt.Add(Lisp.List(ID.WriteNode, builder, expr));
                }
                else
                    throw new InvalidOperationException();
            }
        }

        private object ProcessComputedConstructor(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, new Descriptor[] { Descriptor.CompDocConstructor,
                Descriptor.CompElemConstructor, Descriptor.CompAttrConstructor, Descriptor.CompTextConstructor,
                Descriptor.CompCommentConstructor, Descriptor.CompPIConstructor });
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.CompDocConstructor:
                        return Lisp.List(ID.DynCreateDocument, 
                            ProcessExprList(notation, recs[0].args[0]));

                    case Descriptor.CompElemConstructor:
                    case Descriptor.CompAttrConstructor:
                        {
                            object expr = EmptyIterator.Shared;
                            if (recs[0].args[1] != null)
                                expr = ProcessExprList(notation, recs[0].args[1]);
                            object name;
                            if (recs[0].args[0] is Qname)
                            {
                                Qname qn = (Qname)recs[0].args[0];
                                if (recs[0].descriptor == Descriptor.CompElemConstructor)
                                    name = QNameValue.Parse(qn.Name, _context.NamespaceManager, _context.NamespaceManager.DefaultNamespace);
                                else
                                    name = QNameValue.Parse(qn.Name, _context.NamespaceManager, String.Empty);
                                if (recs[0].descriptor == Descriptor.CompElemConstructor)
                                    return Lisp.List(ID.DynCreateElement, name, expr);
                                else
                                    return Lisp.List(ID.DynCreateAttribute, name, Lisp.List(ID.FormatValue, expr));
                            }
                            else
                            {
                                XmlNamespaceManager nsmgr = new XmlNamespaceManager(_context.nameTable);
                                foreach (KeyValuePair<string, string> k in _context.NamespaceManager.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml))
                                    nsmgr.AddNamespace(k.Key, k.Value);
                                name = Lisp.List(ID.AtomizeX, ProcessExprList(notation, recs[0].args[0]));
                                if (recs[0].descriptor == Descriptor.CompElemConstructor)
                                    return Lisp.List(ID.DynCreateElement, name, nsmgr, expr);
                                else
                                    return Lisp.List(ID.DynCreateAttribute, name, nsmgr, Lisp.List(ID.FormatValue, expr));
                            }
                        }

                    case Descriptor.CompTextConstructor:
                        return Lisp.List(ID.DynCreateText, Lisp.List(ID.FormatValue, 
                            ProcessExprList(notation, recs[0].args[0])));

                    case Descriptor.CompCommentConstructor:
                        return Lisp.List(ID.DynCreateComment, Lisp.List(ID.FormatValue, 
                            ProcessExprList(notation, recs[0].args[0])));

                    case Descriptor.CompPIConstructor:
                        {
                            object name;
                            if (recs[0].args[0] is Qname)
                                name = ((Qname)recs[0].args[0]).Name;
                            else
                                name = Lisp.List(ID.FormatValue, ProcessExprList(notation, recs[0].args[0]));
                            return Lisp.List(ID.DynCreatePi, name, Lisp.List(ID.FormatValue, 
                                ProcessExprList(notation, recs[0].args[1])));
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
                throw new InvalidOperationException();
        }

        private XQuerySequenceType ProcessKindTest(Notation notation, Symbol sym)
        {
            Notation.Record[] recs = notation.Select(sym, Descriptor.KindTest, 1);
            if (recs.Length > 0)
            {
                if (recs[0].Arg0.Tag == Tag.TokenWrapper)
                {
                    TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                    switch (w.Data)
                    {
                        case Token.NODE:
                            return XQuerySequenceType.Node;

                        case Token.TEXT:
                            return XQuerySequenceType.Text;

                        case Token.COMMENT:
                            return XQuerySequenceType.Comment;

                        case Token.ELEMENT:
                            return XQuerySequenceType.Element;

                        case Token.ATTRIBUTE:
                            return XQuerySequenceType.Attribute;

                        case Token.DOCUMENT_NODE:
                            return XQuerySequenceType.Document;

                        case Token.PROCESSING_INSTRUCTION:
                            return XQuerySequenceType.ProcessingInstruction;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    Notation.Record[] recs1 = notation.Select(recs[0].Arg0);
                    switch (recs1[0].descriptor)
                    {
                        case Descriptor.DocumentNode:
                            return ProcessDocumentTest(notation, recs1[0]);

                        case Descriptor.ProcessingInstruction:
                            return ProcessPITest(notation, recs1[0]);

                        case Descriptor.Element:
                            return ProcessElementTest(notation, recs1[0]);

                        case Descriptor.Attribute:
                            return ProcessAttributeTest(notation, recs1[0]);

                        case Descriptor.SchemaElement:
                            return ProcessSchemaElementTest(notation, recs1[0]);

                        case Descriptor.SchemaAttribute:
                            return ProcessSchemaAttributeTest(notation, recs1[0]);

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
            else
                throw new InvalidOperationException();
        }

        private XQuerySequenceType ProcessDocumentTest(Notation notation, Notation.Record rec)
        {
            XQuerySequenceType typeTest = null;
            Notation.Record[] recs = notation.Select(rec.Arg0);            
            if (recs.Length > 0)
            {
                switch (recs[0].descriptor)
                {
                    case Descriptor.Element:
                        typeTest = ProcessElementTest(notation, recs[0]);
                        break;

                    case Descriptor.SchemaElement:
                        typeTest = ProcessSchemaElementTest(notation, recs[0]);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                typeTest.TypeCode = XmlTypeCode.Document;
            }
            return typeTest;
        }

        private XQuerySequenceType ProcessPITest(Notation notation, Notation.Record rec)
        {
            XmlQualifiedNameTest nameTest;
            if (rec.Arg0.Tag == Tag.Qname)
            {
                Qname qn = (Qname)rec.Arg0;
                nameTest = XmlQualifiedNameTest.New(qn.Name, null);
            }
            else
            {
                Literal lit = (Literal)rec.Arg0;
                nameTest = XmlQualifiedNameTest.New(lit.Data, null);
            }
            return new XQuerySequenceType(XmlTypeCode.ProcessingInstruction, nameTest);
        }

        private XQuerySequenceType ProcessElementTest(Notation notation, Notation.Record rec)
        {            
            XmlQualifiedNameTest nameTest;
            if (rec.Arg0.Tag == Tag.TokenWrapper)
                nameTest = XmlQualifiedNameTest.New(null, null);
            else
                nameTest = XmlQualifiedNameTest.New((XmlQualifiedName)ProcessQName(notation, 
                    (Qname)rec.Arg0, _context.NamespaceManager.DefaultNamespace));
            if (rec.args.Length > 1)
            {
                XmlSchemaType schemaType = (XmlSchemaType)ProcessTypeName(notation, (Qname)rec.Arg1);
                if (rec.args.Length > 2)
                    return new XQuerySequenceType(XmlTypeCode.Element, nameTest, schemaType, true);
                else
                    return new XQuerySequenceType(XmlTypeCode.Element, nameTest, schemaType, false);                         
            }
            else
                return new XQuerySequenceType(XmlTypeCode.Element, nameTest);
        }

        private XQuerySequenceType ProcessAttributeTest(Notation notation, Notation.Record rec)
        {
            XmlQualifiedNameTest nameTest;
            if (rec.Arg0.Tag == Tag.TokenWrapper)
                nameTest = XmlQualifiedNameTest.New(null, null);
            else
                nameTest = XmlQualifiedNameTest.New((XmlQualifiedName)ProcessQName(notation, 
                    (Qname)rec.Arg0, String.Empty)); 
            if (rec.args.Length > 1)
                return new XQuerySequenceType(XmlTypeCode.Attribute, nameTest, 
                    (XmlSchemaType)ProcessTypeName(notation, (Qname)rec.Arg1));
            else
                return new XQuerySequenceType(XmlTypeCode.Attribute, nameTest);
        }

        private XQuerySequenceType ProcessSchemaElementTest(Notation notation, Notation.Record rec)
        {
            XmlQualifiedName qname = (XmlQualifiedName)ProcessQName(notation, (Qname)rec.Arg0, _context.NamespaceManager.DefaultNamespace);
            XmlSchemaElement schemaElement = (XmlSchemaElement)_context.SchemaSet.GlobalElements[qname];
            if (schemaElement == null)
                throw new XQueryException(Properties.Resources.XPST0008, qname);
            return new XQuerySequenceType(schemaElement);
        }

        private XQuerySequenceType ProcessSchemaAttributeTest(Notation notation, Notation.Record rec)
        {
            XmlQualifiedName qname = (XmlQualifiedName)ProcessQName(notation, (Qname)rec.Arg0, String.Empty);
            XmlSchemaAttribute schemaAttribute = (XmlSchemaAttribute)_context.SchemaSet.GlobalAttributes[qname];
            if (schemaAttribute == null)
                throw new XQueryException(Properties.Resources.XPST0008, qname);
            return new XQuerySequenceType(schemaAttribute);
        }

        private XQuerySequenceType ProcessTypeDecl(Notation notation, Symbol sym)
        {
            if (sym.Tag == Tag.TokenWrapper &&
                ((TokenWrapper)sym).Data == Token.EMPTY_SEQUENCE)
                return XQuerySequenceType.Void;
            else
            {
                XQuerySequenceType type = ProcessItemType(notation, sym);
                Notation.Record[] recs = notation.Select(sym, Descriptor.Occurrence, 1);
                if (recs.Length > 0)
                {
                    type = new XQuerySequenceType(type);
                    TokenWrapper w = (TokenWrapper)recs[0].Arg0;
                    switch (w.Data)
                    {
                        case Token.Indicator1: // (*)
                            type.Cardinality = XmlTypeCardinality.ZeroOrMore; 
                            break;

                        case Token.Indicator2: // (+)
                            type.Cardinality = XmlTypeCardinality.OneOrMore;
                            break;

                        case Token.Indicator3: // (?)
                            type.Cardinality = XmlTypeCardinality.ZeroOrOne;
                            break;
                    }
                }
                return type;
            }
        }

        private XQuerySequenceType ProcessItemType(Notation notation, Symbol sym)
        {
            if (sym.Tag == Tag.TokenWrapper)
            {
                TokenWrapper w = (TokenWrapper)sym;
                if (w.Data == Token.ITEM)
                    return new XQuerySequenceType(XmlTypeCode.Item);
                else
                    throw new InvalidOperationException();
            }
            else if (sym.Tag == Tag.Qname)
            {
                XmlSchemaType xmlType = (XmlSchemaType)ProcessTypeName(notation, (Qname)sym);
                if (xmlType == null)
                    return null;
                return new XQuerySequenceType(xmlType, XmlTypeCardinality.One, null);
            }
            else
                return ProcessKindTest(notation, sym);
        }

        private bool TryProcessTypeName(Notation notation, Qname qname, bool raise, out XmlSchemaObject schemaObject)
        {
            schemaObject = null;
            XmlQualifiedName qualifiedName =
                (XmlQualifiedName)ProcessQName(notation, qname, _context.NamespaceManager.DefaultNamespace);
            if (qualifiedName.Name == "anyAtomicType" && qualifiedName.Namespace == XmlReservedNs.NsXs)
            {
                schemaObject = XQuerySequenceType.XmlSchema.AnyAtomicType;
                return true;
            }
            if (qualifiedName.Name == "untypedAtomic" && qualifiedName.Namespace == XmlReservedNs.NsXs)
            {
                schemaObject = XQuerySequenceType.XmlSchema.UntypedAtomic;
                return true;
            }
            if (qualifiedName.Name == "anyType" && qualifiedName.Namespace == XmlReservedNs.NsXs)
            {
                schemaObject = XQuerySequenceType.XmlSchema.AnyType;
                return true;
            }
            if (qualifiedName.Name == "untyped" && qualifiedName.Namespace == XmlReservedNs.NsXs)            
                return true;
            if (qualifiedName.Name == "yearMonthDuration" && qualifiedName.Namespace == XmlReservedNs.NsXs)
            {
                schemaObject = XQuerySequenceType.XmlSchema.YearMonthDuration;
                return true;
            }
            if (qualifiedName.Name == "dayTimeDuration" && qualifiedName.Namespace == XmlReservedNs.NsXs)
            {
                schemaObject = XQuerySequenceType.XmlSchema.DayTimeDuration;
                return true;
            }
            if (qualifiedName.Namespace == XmlReservedNs.NsXs)
                schemaObject = XmlSchemaType.GetBuiltInSimpleType(qualifiedName);
            else
                schemaObject = _context.SchemaSet.GlobalTypes[qualifiedName];
            if (schemaObject == null && raise)
                throw new XQueryException(Properties.Resources.XPST0008, qualifiedName);            
            return schemaObject != null;
        }

        private XmlSchemaObject ProcessTypeName(Notation notation, Qname qname)
        {
            XmlSchemaObject schemaObject;
            TryProcessTypeName(notation, qname, true, out schemaObject);
            return schemaObject;
        }

        private object ProcessQName(Notation notation, Qname qname, String defaultNS)
        {
            return QNameParser.Parse(qname.Name, _context.NamespaceManager, defaultNS, _context.nameTable);
        }

        private void ProcessElementQName(Qname qname, out string prefix, out string localName, out string ns)
        {
            QNameParser.Split(qname.Name, out prefix, out localName);
             ns = _context.NamespaceManager.LookupNamespace(prefix);
             if (ns == null && !String.IsNullOrEmpty(prefix))             
                 throw new XQueryException(Properties.Resources.XPST0081, prefix);
            prefix = _context.nameTable.Add(prefix);
            localName = _context.nameTable.Add(localName);
        }

        private void ProcessAttributeQName(Qname qname, out string prefix, out string localName, out string ns)
        {
            QNameParser.Split(qname.Name, out prefix, out localName);
            if (String.IsNullOrEmpty(prefix))
                ns = String.Empty;
            else
            {
                ns = _context.NamespaceManager.LookupNamespace(prefix);
                if (ns == null)
                    throw new XQueryException(Properties.Resources.XPST0081, prefix);
            }
            prefix = _context.nameTable.Add(prefix);
            localName = _context.nameTable.Add(localName);
        }

        private object ProcessVarName(VarName varName)
        {
            string ns = null;
            if (varName.Prefix != String.Empty)
            {
                ns = _context.NamespaceManager.LookupNamespace(varName.Prefix);
                if (ns == null)
                    throw new XQueryException(Properties.Resources.XPST0081, varName.Prefix);
            }
            return Lisp.Defatom(ns, new string[] { "$", varName.LocalName }, false);
        }

        internal static object GetVarName(string localName, string ns)
        {
            return Lisp.Defatom(ns, new string[] { "$", localName }, false);
        }

        private bool IsBooleanFunctor(object expr)
        {
            return Lisp.IsFunctor(expr, Funcs.Eq) ||
                Lisp.IsFunctor(expr, Funcs.Ne) ||
                Lisp.IsFunctor(expr, Funcs.Gt) ||
                Lisp.IsFunctor(expr, Funcs.Ge) ||
                Lisp.IsFunctor(expr, Funcs.Lt) ||
                Lisp.IsFunctor(expr, Funcs.Le) ||
                Lisp.IsFunctor(expr, ID.GeneralEQ) ||
                Lisp.IsFunctor(expr, ID.GeneralNE) ||
                Lisp.IsFunctor(expr, ID.GeneralGT) ||
                Lisp.IsFunctor(expr, ID.GeneralGE) ||
                Lisp.IsFunctor(expr, ID.GeneralLT) ||
                Lisp.IsFunctor(expr, ID.GeneralLE) ||
                Lisp.IsFunctor(expr, Funcs.And) ||
                Lisp.IsFunctor(expr, Funcs.Or) ||
                Lisp.IsFunctor(expr, ID.True) ||
                Lisp.IsFunctor(expr, ID.False) ||
                Lisp.IsFunctor(expr, ID.Not) ||
                Lisp.IsFunctor(expr, ID.SameNode) ||
                Lisp.IsFunctor(expr, ID.PrecedingNode) ||
                Lisp.IsFunctor(expr, ID.FollowingNode) ||
                Lisp.IsFunctor(expr, ID.InstanceOf) ||
                Lisp.IsFunctor(expr, ID.Castable) ||
                Lisp.IsFunctor(expr, ID.Some) ||
                Lisp.IsFunctor(expr, ID.Every) ||
                Lisp.IsFunctor(expr, ID.BooleanValue);
        }

        private bool IsXPathFunctor(object expr)
        {
            if (Lisp.IsFunctor(expr, ID.Validate) ||
                Lisp.IsFunctor(expr, ID.CastTo) ||
                Lisp.IsFunctor(expr, Funcs.Let1) ||
                Lisp.IsFunctor(expr, Funcs.Let2) ||
                Lisp.IsFunctor(expr, ID.DynExecuteExpr))
                return true;
            return false;
        }

        private bool IsNumberFunctor(object expr)
        {
            return
                Lisp.IsFunctor(expr, Funcs.Add) ||
                Lisp.IsFunctor(expr, Funcs.Sub) ||
                Lisp.IsFunctor(expr, Funcs.Mul) ||
                Lisp.IsFunctor(expr, Funcs.Div) ||
                Lisp.IsFunctor(expr, Funcs.IDiv) ||
                Lisp.IsFunctor(expr, Funcs.Mod) ||
                Lisp.IsFunctor(expr, Funcs.Neg);
        }

        private XQuerySequenceType GetXPathValueType(object expr)
        {            
            if (Lisp.IsFunctor(expr, Funcs.Let1) ||
                Lisp.IsFunctor(expr, Funcs.Let2))
                return new XQuerySequenceType(XmlTypeCode.Node);
            if (Lisp.IsFunctor(expr, ID.DynExecuteExpr))
            {
                XQueryPathExpr pathExpr = Lisp.Arg1(expr) as XQueryPathExpr;
                if (pathExpr != null)
                {
                    XQueryStepExpr lastStep = pathExpr.LastStep as XQueryStepExpr;
                    if (lastStep != null)
                    {
                        if (lastStep.TypeTest != null)
                            return lastStep.TypeTest;
                        else if (lastStep.NameTest != null)
                        {
                            if (_context.SchemaProcessing != SchemaProcessingMode.Force && !_context.NeedValidatedParser)
                                return new XQuerySequenceType(XmlTypeCode.Node);
                        }
                    }
                }
            }
            return XQuerySequenceType.Item;
        }

        private object CompileBooleanConvertion(object expr)
        {
            if (Lisp.IsFunctor(expr))
            {
                if (IsBooleanFunctor(expr))
                    return expr;
            }
            return Lisp.List(ID.BooleanValue, expr);
        }

        private object CompileAtomizeNumberExpr(object expr, XQuerySequenceType type)
        {
            if (type.Equals(XQuerySequenceType.Item))
                return Lisp.List(ID.CastToNumber1, Lisp.List(ID.Atomize, expr));
            else if (type.IsNode || type.Cardinality != XmlTypeCardinality.One)
            {
                Type resType = type.AtomizedValueType;
                if (resType == typeof(UntypedAtomic) &&
                    (_context.SchemaProcessing == SchemaProcessingMode.Force || _context.NeedValidatedParser))
                    resType = typeof(System.Object);
                if (type.Cardinality == XmlTypeCardinality.One ||
                    type.Cardinality == XmlTypeCardinality.OneOrMore)
                {
                    if (TypeConverter.IsNumberType(resType))
                        return Lisp.List(Funcs.Cast, Lisp.List(ID.AtomizeX, expr), resType);
                    else if (resType == typeof(UntypedAtomic))
                        return Lisp.List(ID.CastToNumber2, Lisp.List(ID.AtomizeX, expr));
                    else
                        return Lisp.List(ID.CastToNumber1, Lisp.List(ID.AtomizeX, expr));
                }
                else
                {
                    if (TypeConverter.IsNumberType(resType))
                        return Lisp.List(Funcs.Cast, Lisp.List(ID.Atomize, expr), resType);
                    else if (resType == typeof(UntypedAtomic))
                        return Lisp.List(ID.CastToNumber2, Lisp.List(ID.Atomize, expr));
                    else
                        return Lisp.List(ID.CastToNumber1, Lisp.List(ID.Atomize, expr));
                }
            }
            else
            {
                if (type.IsNumeric)
                    return Lisp.List(Funcs.Cast, expr, type.ItemType);
                else if (type.IsUntypedAtomic)
                    return Lisp.List(ID.CastToNumber2, expr);
                else
                    return Lisp.List(ID.CastToNumber1, expr);
            }
        }

        private object CompileAtomizeValueExpr(object expr, XQuerySequenceType type)
        {
            if (type.Equals(XQuerySequenceType.Item))
                return Lisp.List(ID.Atomize, expr);
            else if (type.IsNode || type.Cardinality != XmlTypeCardinality.One)
            {
                Type resType = type.AtomizedValueType;
                if (type.Cardinality == XmlTypeCardinality.One ||
                    type.Cardinality == XmlTypeCardinality.OneOrMore)
                {
                    if (resType != typeof(System.Object))
                        return Lisp.List(Funcs.Cast, Lisp.List(ID.AtomizeX, expr), resType);
                    else
                        return Lisp.List(ID.AtomizeX, expr);
                }
                else
                {
                    if (resType != typeof(System.Object))
                        return Lisp.List(Funcs.Cast, Lisp.List(ID.Atomize, expr), resType);
                    else
                        return Lisp.List(ID.Atomize, expr);
                }
            }
            else
            {
                if (type.ItemType != typeof(System.Object))
                    return Lisp.List(Funcs.Cast, expr, type.ItemType);
                else
                    return expr;
            }
        }

        private object CompileAtomizeNumberExpr(object expr)
        {
            if (Lisp.IsFunctor(expr))
            {
                if (IsNumberFunctor(expr) ||
                    IsBooleanFunctor(expr) ||
                    Lisp.IsFunctor(expr, ID.CastToNumber1) ||
                    Lisp.IsFunctor(expr, ID.CastToNumber2) ||
                    Lisp.IsFunctor(expr, ID.Number))
                    return expr;
                if (Lisp.IsFunctor(expr, ID.Par))
                    return CompileAtomizeNumberExpr(Lisp.Second(expr));
                if (Lisp.IsFunctor(expr, ID.CastToItem))
                {
                    XQuerySequenceType seqType = (XQuerySequenceType)Lisp.Arg2(expr);
                    if (seqType.TypeCode == XmlTypeCode.UntypedAtomic)
                        return Lisp.List(ID.CastToNumber2, Lisp.List(ID.Atomize, expr));                        
                    else
                        return Lisp.List(Funcs.Cast, 
                            Lisp.List(ID.Atomize, expr), seqType.ValueType);
                }
                else if (IsXPathFunctor(expr))
                    return CompileAtomizeNumberExpr(expr, GetXPathValueType(expr));
                else
                {
                    XQueryFunctionRecord fr =
                        _context.FunctionTable.GetRecord(Lisp.Car(expr), Lisp.Length(expr) - 1);
                    if (fr != null)
                    {
                        if (fr.returnType.ParameterType != null &&
                            TypeConverter.IsNumberType(fr.returnType.ParameterType))
                            return expr;
                        return CompileAtomizeNumberExpr(expr, fr.returnType);
                    }
                    else
                        return Lisp.List(ID.CastToNumber1, Lisp.List(ID.Atomize, expr));
                }
            }
            else
                if (Lisp.IsAtom(expr))
                {
                    XQuerySequenceType type = _varTable.GetType(expr);
                    if (type == null)
                        throw new XQueryException(Properties.Resources.XPST0008, expr);
                    return CompileAtomizeNumberExpr(expr, type);
                }
            return expr;
        }

        private object CompileAtomizeValueExpr(object expr)
        {
            if (Lisp.IsFunctor(expr))
            {
                if (IsNumberFunctor(expr) ||
                    IsBooleanFunctor(expr) ||
                    Lisp.IsFunctor(expr, ID.CastToNumber1) ||
                    Lisp.IsFunctor(expr, ID.CastToNumber2) ||
                    Lisp.IsFunctor(expr, ID.Number))
                    return expr;
                if (Lisp.IsFunctor(expr, ID.Par))
                    return CompileAtomizeValueExpr(Lisp.Second(expr));
                if (Lisp.IsFunctor(expr, ID.CastToItem))
                {
                    XQuerySequenceType seqType = (XQuerySequenceType)Lisp.Arg2(expr);
                    if (seqType.ValueType != typeof(System.Object))
                        return Lisp.List(Funcs.Cast,
                            Lisp.List(ID.Atomize, expr), seqType.ValueType);
                }
                else if (IsXPathFunctor(expr))
                    return CompileAtomizeValueExpr(expr, GetXPathValueType(expr));
                else
                {
                    XQueryFunctionRecord fr =
                        _context.FunctionTable.GetRecord(Lisp.Car(expr), Lisp.Length(expr) - 1);
                    if (fr != null)
                    {
                        if (fr.returnType.ParameterType != null &&
                            TypeConverter.IsNumberType(fr.returnType.ParameterType))
                            return expr;
                        return CompileAtomizeValueExpr(expr, fr.returnType);
                    }
                    else
                        return Lisp.List(ID.Atomize, expr);
                }
            }
            else
                if (Lisp.IsAtom(expr))
                {
                    XQuerySequenceType type = _varTable.GetType(expr);
                    if (type == null)
                        throw new XQueryException(Properties.Resources.XPST0008, expr);
                    return CompileAtomizeValueExpr(expr, type);
                }
            return expr;
        }

        private object ProcessExprList(Notation notation, object lval)
        {
            if (lval == null || Lisp.Length(lval) > 1)
                return Lisp.List(ID.DynExecuteExpr, ProcessExpr(notation, lval), ID.Context, Lisp.ARGV);
            else
                return ProcessExprSingle(notation, (Symbol)Lisp.First(lval));
        }

        private XQuerySequenceType GetExprType(object expr)
        {
            if (Lisp.IsFunctor(expr))
            {
                if (IsXPathFunctor(expr))
                    return XQuerySequenceType.ItemS;
                else if (IsBooleanFunctor(expr))
                    return XQuerySequenceType.Boolean;
                else if (Lisp.IsFunctor(expr, Funcs.Cast))
                    return new XQuerySequenceType((Type)Lisp.Arg2(expr), XmlTypeCardinality.One);
                else if (Lisp.IsFunctor(expr, ID.Par))
                    return GetExprType(Lisp.Second(expr));
                else if (Lisp.IsFunctor(expr, ID.CastToNumber2) ||
                         Lisp.IsFunctor(expr, ID.Number))
                    return new XQuerySequenceType(XmlTypeCode.Double);
                else
                {
                    XQueryFunctionRecord rec = _context.FunctionTable.GetRecord(expr);
                    if (rec != null)
                        return rec.returnType;
                }
            }
            else
            {
                if (Lisp.IsAtom(expr))
                {
                    XQuerySequenceType type = _varTable.GetType(expr);
                    if (type != null)
                        return type;
                }
                else
                    return new XQuerySequenceType(expr.GetType(),
                        XmlTypeCardinality.One);
            }
            return XQuerySequenceType.Item;
        }

        private XQuerySequenceType EvalExprType(object expr)
        {
            if (Lisp.IsFunctor(expr))
            {
                if (IsBooleanFunctor(expr))
                    return new XQuerySequenceType(XmlTypeCode.Boolean, XmlTypeCardinality.ZeroOrOne);
                else if (Lisp.IsFunctor(expr, Funcs.Neg))
                    return EvalExprType(Lisp.Arg1(expr));
                else if (IsNumberFunctor(expr))
                {
                    XQuerySequenceType typ1 = EvalExprType(Lisp.Arg1(expr));
                    XQuerySequenceType typ2 = EvalExprType(Lisp.Arg2(expr));
                    if (typ1.IsUntypedAtomic)
                        typ1 = new XQuerySequenceType(XmlTypeCode.Double, typ1.Cardinality);
                    if (typ2.IsUntypedAtomic)
                        typ2 = new XQuerySequenceType(XmlTypeCode.Double, typ2.Cardinality);
                    XmlTypeCardinality cardinality;
                    if (typ1.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                        typ2.Cardinality == XmlTypeCardinality.ZeroOrOne)
                        cardinality = XmlTypeCardinality.ZeroOrOne;
                    else
                        cardinality = XmlTypeCardinality.One;
                    if (Lisp.IsFunctor(expr, Funcs.IDiv))
                        return new XQuerySequenceType(typeof(CoreServices.Integer), cardinality);
                    else if (Lisp.IsFunctor(expr, Funcs.Div))
                    {
                        if (typ1.ItemType == typeof(System.Double) || typ2.ItemType == typeof(System.Double))
                            return new XQuerySequenceType(XmlTypeCode.Double, cardinality);
                        if (typ1.ItemType == typeof(System.Single) || typ2.ItemType == typeof(System.Single))
                            return new XQuerySequenceType(XmlTypeCode.Float, cardinality);
                        return new XQuerySequenceType(XmlTypeCode.Decimal, cardinality);
                    }
                    else
                    {
                        Type typ3 = TypeConverter.GetType(typ1.ItemType, typ2.ItemType);
                        return new XQuerySequenceType(typ3, cardinality);
                    }
                }
                else if (Lisp.IsFunctor(expr, ID.CastToItem))
                {
                    XQuerySequenceType type = (XQuerySequenceType)Lisp.Arg2(expr);
                    if (type.Cardinality == XmlTypeCardinality.One)
                        return new XQuerySequenceType(type.SchemaType, XmlTypeCardinality.ZeroOrOne, type.ParameterType);
                    return new XQuerySequenceType(type);
                }
                else if (Lisp.IsFunctor(expr, Funcs.Cast))
                {
                    object arg = Lisp.Arg2(expr);
                    if ((Type)arg == typeof(XQuerySequenceType))
                        return new XQuerySequenceType(XQuerySequenceType.ItemS);
                    else
                        return new XQuerySequenceType((Type)arg, XmlTypeCardinality.One);
                }
                else if (Lisp.IsFunctor(expr, ID.Number) ||
                    Lisp.IsFunctor(expr, ID.CastToNumber2))
                    return new XQuerySequenceType(XmlTypeCode.Double);
                else if (Lisp.IsFunctor(expr, ID.CastToNumber1))
                    return EvalExprType(Lisp.Second(expr));
                else if (Lisp.IsFunctor(expr, Funcs.Cast))
                    return EvalExprType(Lisp.Second(expr));
                else if (Lisp.IsFunctor(expr, ID.Atomize) ||
                    Lisp.IsFunctor(expr, ID.AtomizeBody))
                {
                    XQuerySequenceType typ = EvalExprType(Lisp.Second(expr));
                    XmlTypeCardinality cardinality;
                    if (typ.Cardinality == XmlTypeCardinality.One ||
                        typ.Cardinality == XmlTypeCardinality.OneOrMore)
                        cardinality = XmlTypeCardinality.One;
                    else
                        cardinality = XmlTypeCardinality.ZeroOrOne;
                    if (typ.IsNode)
                    {
                        if (typ.SchemaType != null)
                            return new XQuerySequenceType(typ.SchemaType, cardinality, typeof(System.Object));
                        else
                            return new XQuerySequenceType(XmlTypeCode.AnyAtomicType, cardinality);
                    }
                    else
                        return new XQuerySequenceType(typ.TypeCode, cardinality);
                }
                else if (Lisp.IsFunctor(expr, ID.CastTo))
                    return (XQuerySequenceType)Lisp.Arg2(expr);
                else if (Lisp.IsFunctor(expr, ID.Par))
                    return EvalExprType(Lisp.Second(expr));
                else if (Lisp.IsFunctor(expr, ID.Range))
                    return new XQuerySequenceType(XmlTypeCode.Integer, XmlTypeCardinality.ZeroOrMore);
                else if (IsXPathFunctor(expr))
                    return XQuerySequenceType.ItemS;
                else if (Lisp.IsFunctor(expr, Funcs.If))
                {
                    XQuerySequenceType typ1 = EvalExprType(Lisp.Arg2(expr));
                    XQuerySequenceType typ2 = EvalExprType(Lisp.Arg3(expr));
                    if (typ1.Equals(typ2))
                        return typ1;
                }
                else
                {
                    XQueryFunctionRecord rec = _context.FunctionTable.GetRecord(expr);
                    if (rec != null)
                        return rec.returnType;
                }
            }
            else if (expr is XQueryExpr)
            {
                XQueryExpr dynExpr = (XQueryExpr)expr;
                XQuerySequenceType typ = new XQuerySequenceType(EvalExprType(dynExpr.m_expr[0]));
                for (int k = 1; k < dynExpr.m_expr.Length; k++)
                {
                    XQuerySequenceType typ2 = EvalExprType(dynExpr.m_expr[k]);
                    if (typ.TypeCode != typ2.TypeCode)
                        return XQuerySequenceType.ItemS;
                    if (typ.Cardinality == XmlTypeCardinality.One)
                        typ.Cardinality = XmlTypeCardinality.OneOrMore;
                    else if (typ.Cardinality == XmlTypeCardinality.ZeroOrOne)
                    {
                        if (typ2.Cardinality == XmlTypeCardinality.One ||
                            typ2.Cardinality == XmlTypeCardinality.OneOrMore)
                            typ.Cardinality = XmlTypeCardinality.OneOrMore;
                        else
                            typ.Cardinality = XmlTypeCardinality.ZeroOrMore;
                    }
                    else if (typ.Cardinality == XmlTypeCardinality.ZeroOrMore)
                    {
                        if (typ2.Cardinality == XmlTypeCardinality.One ||
                            typ2.Cardinality == XmlTypeCardinality.OneOrMore)
                            typ.Cardinality = XmlTypeCardinality.OneOrMore;
                    }
                }
                return typ;
            }
            else
            {
                if (Lisp.IsAtom(expr))
                {
                    XQuerySequenceType type = _varTable.GetType(expr);
                    if (type != null)
                        return type;
                }
                else
                    return new XQuerySequenceType(expr.GetType(), XmlTypeCardinality.One);
            }
            return XQuerySequenceType.Item;
        }

        private object CompileConversion(object expr, XQuerySequenceType destType)
        {
            if (destType == XQuerySequenceType.Item)
                return expr;
            XQuerySequenceType type = EvalExprType(expr);
            if (destType.TypeCode == XmlTypeCode.None)
            {
                if (type != XQuerySequenceType.Item &&
                    (type.Cardinality == XmlTypeCardinality.One ||
                     type.Cardinality == XmlTypeCardinality.OneOrMore))
                  throw new XQueryException(Properties.Resources.XPTY0004, type, destType);
                return Lisp.List(ID.CastArg, expr, destType);
            }
            if ((destType.Cardinality == XmlTypeCardinality.ZeroOrMore ||
                 destType.Cardinality == XmlTypeCardinality.OneOrMore) &&
                (type.Cardinality == XmlTypeCardinality.One ||
                 type.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                 Lisp.IsFunctor(expr, Funcs.Let1) || Lisp.IsFunctor(expr, Funcs.Let2)))
                expr = Lisp.List(ID.Seq, Lisp.List(Funcs.LambdaQuote, expr)); // !!
            Type clrType = destType.ParameterType;
            if (clrType != null && clrType.IsArray)
                clrType = clrType.GetElementType();
            if (clrType == null)
                clrType = destType.ValueType;
            if ((destType.Cardinality == XmlTypeCardinality.One ||
                 destType.Cardinality == XmlTypeCardinality.ZeroOrOne) &&
                   (type.Cardinality == XmlTypeCardinality.ZeroOrOne ||
                    type.Cardinality == XmlTypeCardinality.ZeroOrMore ||
                    type.Cardinality == XmlTypeCardinality.OneOrMore ||                    
                    type.Equals(XQuerySequenceType.Item)))
            {
                if (destType.IsNode)
                {
                    if (clrType == typeof(XPathNavigator))
                    {
                        if (destType.Cardinality == XmlTypeCardinality.One)
                            return Lisp.List(ID.NodeValueX, expr);
                        else
                            return Lisp.List(ID.NodeValue, expr);
                    }
                    else
                        return Lisp.List(ID.NodeValueBody,
                            Lisp.List(Funcs.LambdaQuote, expr));
                }
                else
                {
                    if (destType.Cardinality == XmlTypeCardinality.One)
                    {
                        if (IsNumberFunctor(expr))
                            expr = Lisp.List(ID.ExactlyOne, 
                                Lisp.List(Funcs.LambdaQuote, expr));
                        else
                            expr = Lisp.List(ID.AtomizeX, expr);
                    }
                    else
                    {
                        if (clrType == typeof(System.Object))
                            expr = Lisp.List(ID.AtomizeBody, expr);
                        else
                            expr = Lisp.List(ID.Atomize, expr);
                    }
                }
            }
            if (clrType == typeof(XPathNavigator) && !type.IsNode)
                throw new XQueryException(Properties.Resources.XPTY0004, type, destType);
            if (destType.ItemType != typeof(System.Object) &&
                 type.ItemType != destType.ItemType)
            {
                if (destType.TypeCode != XmlTypeCode.AnyAtomicType)
                    expr = Lisp.List(ID.CastArg, expr, destType);
            }
            if (clrType != typeof(System.Object))
                expr = Lisp.List(Funcs.Cast, expr, clrType);
            return expr;
        }
    }
}
