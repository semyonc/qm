/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Data;
using System.Data.Common;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;
using System.Threading;

namespace DataEngine
{
    public class QueryContext
    {        
        private class InnerExecutive: Executive
        {
            public InnerExecutive(object owner)
                : base(owner)
            {
            }

            public override object OperatorEq(object arg1, object arg2)
            {
                if (arg1 == arg2)
                    return true;
                else if (arg1 == null || arg2 == null)
                    return null;
                else if (arg1 is IComparable && arg2 is IComparable)
                {
                    TypeCode typecode = Util.GetTypeCode(arg1, arg2);
                    object val1 = TypeConverter.ChangeType(arg1, typecode);
                    object val2 = TypeConverter.ChangeType(arg2, typecode);
                    if (((IComparable)val1).CompareTo(val2) == 0)
                        return true;
                }
                return null;
            }

            public override object OperatorGt(object arg1, object arg2)
            {
                if (arg1 == null || arg2 == null)
                    return null;
                else
                    if (arg1 is IComparable && arg2 is IComparable)
                    {
                        TypeCode typecode = Util.GetTypeCode(arg1, arg2);
                        object val1 = TypeConverter.ChangeType(arg1, typecode);
                        object val2 = TypeConverter.ChangeType(arg2, typecode);
                        if (((IComparable)val1).CompareTo(val2) > 0)
                            return true;
                    }
                    else
                        throw new InvalidOperationException();
                return null;
            }

            public override void HandleRuntimeException(Exception exception)
            {
                if (exception is ValueNotDefined)
                    throw new ESQLException(Properties.Resources.InvalidIdentifier, 
                        ((ValueNotDefined)exception).Name);
                throw exception;
            }
        }

        private class DocumentContext
        {
            public XmlDocument document;
            public XmlNamespaceManager nsManager;
        }

        private class TableEstimate
        {
            public int count;
            public int threshold;
        }

        private Dictionary<XmlDocument, DocumentContext> context;
        private Dictionary<TableType, TableEstimate> estimate;
        private List<IDisposable> resources;
        private CancellationTokenSource cancelSource;

        [ThreadStatic]
        private XmlDocument _xml_res = null;

        public QueryContext(DatabaseDictionary dictionary)
        {
            context = new Dictionary<XmlDocument, DocumentContext>();
            estimate = new Dictionary<TableType, TableEstimate>();
            resources = new List<IDisposable>();
            cancelSource = new CancellationTokenSource();
            NameTable = new NameTable();
            Output = new StringWriter();
            DataCache = new ResultsetCache();
            DatabaseDictionary = dictionary;
            LdapSearchLimit = 0;
            CacheEnabled = true;
            LimitInputQuery = -1;
        }

        public DatabaseDictionary DatabaseDictionary { get; private set; }

        public XmlNameTable NameTable { get; private set; }

        public XmlDocument XmlResult
        {
            get
            {
                if (_xml_res == null)
                    _xml_res = new XmlDocument(NameTable);
                return _xml_res;
            }
        }

        public void SetNsManager(XmlDocument xmldoc, XmlNamespaceManager nsManager)
        {            
            lock (context)
            {
                DocumentContext docContext;
                if (!context.TryGetValue(xmldoc, out docContext))
                {
                    docContext = new DocumentContext();
                    docContext.document = xmldoc;
                    context.Add(xmldoc, docContext);
                }
                docContext.nsManager = nsManager;
            }
        }

        public XmlNamespaceManager GetNsManager(XmlDocument xmldoc)
        {            
            lock (context)
            {
                DocumentContext docContext;
                if (context.TryGetValue(xmldoc, out docContext))
                    return docContext.nsManager;
                else
                    return null;
            }
        }

        public int GetTableEstimate(SmartTableAccessor smartAccesstor, int threshold)
        {
            TableEstimate tableEstimate;
            lock (estimate)
                if (!estimate.TryGetValue(smartAccesstor.TableType, out tableEstimate))
                {
                    tableEstimate = new TableEstimate();
                    estimate.Add(smartAccesstor.TableType, tableEstimate);
                }
            lock (tableEstimate)
            {
                if (tableEstimate.count == tableEstimate.threshold || tableEstimate.threshold < threshold)
                {
                    tableEstimate.count = smartAccesstor.GetTableEstimate(threshold);
                    tableEstimate.threshold = threshold;
                }
                return tableEstimate.count;
            }
        }

#if DEBUG
        public TextWriter _traceWriter = null;
#endif

        public virtual Executive CreateExecutive(object owner)
        {
            Service.Initialize();
            SQLX.Initialize();
            Executive executive = new InnerExecutive(owner);
            executive.Prepare();
#if DEBUG
            executive.m_traceOutput = _traceWriter;
#endif
            return executive;
        }

        public virtual TextFileDataFormat GetTextFileDataFormat()
        {
            return TextDataAccessor.GetCurrentTextDataFormat();
        }

        public void AddResource(IDisposable obj)
        {
            resources.Add(obj);
        }

        public void Cancel()
        {
            cancelSource.Cancel();
        }

        public void Close()
        {
            if (EnableHPC)
                cancelSource.Cancel();
            foreach (IDisposable disp in resources)
                disp.Dispose();
        }

        public StringWriter Output { get; private set; }

        public bool CacheEnabled { get; set; }

        public ResultsetCache DataCache { get; private set; }

        public int LdapSearchLimit { get; set; }

        public TimeSpan LdapClientTimeout { get; set; }

        public bool UseSampleData { get; set; }

#if DEBUG
        public String TraceFileName { get; set; }
#endif

        public bool EnableHPC { get; set; }

        public int LimitInputQuery { get; set; }

        public bool DisableLimitInput { get; set; }

        public CancellationToken Token
        {
            get
            {
                return cancelSource.Token;
            }
        }
    }
}
