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

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;


namespace DataEngine
{
    public delegate bool DemandProcessingDelegate(Resultset rs);

    /// <summary>
    /// Base class for query plan node
    /// </summary>
    public abstract class QueryNode
    {

        #region DemandProcessingContext
        /// <summary>
        /// Utility helper class
        /// </summary>
        public abstract class DemandProcessingContext: IDisposable
        {
            protected bool _disposed = false;
            private DemandProcessingContext[] _childs = null;

            public int RecordLimit { get; set; }

            public DemandProcessingContext(Resultset[] src)
            {
                RecordLimit = -1;
                if (src != null)
                {
                    _childs = new DemandProcessingContext[src.Length];
                    for (int k = 0; k < src.Length; k++)
                        if (src[k] != null)
                            _childs[k] = src[k]._context;
                }
            }

            ~DemandProcessingContext()
            {
                Dispose(false);
            }

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                if (_childs != null)
                {
                    foreach (DemandProcessingContext context in _childs)
                        if (context != null)
                            context.Dispose();
                }
                GC.SuppressFinalize(this);
            }

            #endregion

            protected virtual void Dispose(bool disposing)
            {
                _disposed = true;
            }

            protected void CheckDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
            }

            public virtual void CheckCancelled()
            {
                return;
            }

            public abstract bool ProcessNextPiece(Resultset rs);
        }
        #endregion

        public abstract class LispProcessingContext : DemandProcessingContext
        {
            private Executive _executive;
            private QueryNode _node;
            private QueryContext _queryContext;
            private object[] _parameters;
            private int _rangeLength;            

            public LispProcessingContext(QueryNode node, Resultset src, QueryContext queryContext, Object[] parameters)
                : this(node, new Resultset[] { src }, queryContext, parameters)
            {
            }

            public LispProcessingContext(QueryNode node, Resultset[] src, QueryContext queryContext, Object[] parameters)
                : base(src)
            {
                _node = node;
                _parameters = parameters;
                _queryContext = queryContext;
                _executive = queryContext.CreateExecutive(this);
            }

            public override void CheckCancelled()
            {
                _queryContext.Token.ThrowIfCancellationRequested();
            }

            public Executive LispExecutive
            {
                get
                {
                    return _executive;
                }
            }

            public Object[] Parameters
            {
                get
                {
                    return _parameters;
                }
            }

            public QueryNode Node
            {
                get
                {
                    return _node;
                }
            }

            public QueryContext QueryContext
            {
                get
                {
                    return _queryContext;
                }
            }

            public int RangeLength
            {
                get
                {
                    return _rangeLength;
                }

                protected set
                {
                    _rangeLength = value;
                }
            }            
        }

        public class EnumeratorProcessingContext : DemandProcessingContext
        {
            public IEnumerator<Row> Iterator { get; set; }

            public EnumeratorProcessingContext(Resultset[] src)
                : base(src)
            {
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (Iterator.MoveNext())
                {
                    rs.Enqueue(Iterator.Current);
                    return true;
                }
                else
                    return false;
            }
        }

        protected class DelegateProcessingContext : DemandProcessingContext
        {
            private DemandProcessingDelegate _delegate;

            public DelegateProcessingContext(Resultset[] src, DemandProcessingDelegate del)
                : base(src)
            {
                _delegate = del;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                return _delegate(rs);
            }
        }

        protected QueryNodeCollection _childs;
        internal QueryNode _parent = null;
        
        public QueryNode()
        {
        }

        public virtual void Prepare()
        {
            if (_childs != null)
                foreach (QueryNode qnode in _childs)
                    qnode.Prepare();
        }

        public abstract Resultset Get(QueryContext queryContext, object[] parameters);        
        
        public String[][] Namespaces { get; set; }

        public XmlTypeManager TypeManager { get; set; }

        public QueryNode Parent
        {
            get { return _parent; }
        }
    
        public QueryNodeCollection ChildNodes
        {
            get { return _childs; }
        }

        public QueryNode GetNodeByID(object ID)
        {
            for (int k = 0; k < _childs.Count; k++)
                if (_childs[k].NodeID == ID)
                    return _childs[k];
            throw new IndexOutOfRangeException();
        }

        public object NodeID { get; set; }


        public static Resultset CreateResultset(RowType rt, DemandProcessingDelegate callback)
        {
            return new Resultset(rt, new DelegateProcessingContext(null, callback));
        }

        public static Resultset CreateResultset(RowType rt, Resultset[] src, DemandProcessingDelegate callback)
        {
            return new Resultset(rt, new DelegateProcessingContext(src, callback));
        }

#if DEBUG
        protected void OutlineNode(System.IO.TextWriter w, int padding)
        {
            for (int k = 0; k < padding; k++)
                w.Write('\t');
        }

        protected virtual void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            w.WriteLine(GetType().Name);
        }

        private void DumpNode(System.IO.TextWriter w, int padding)
        {
            Dump(w, padding);
            if (_childs != null)
                foreach (QueryNode node in _childs)
                    node.DumpNode(w, padding + 1);
        }

        public void Dump(System.IO.TextWriter w)
        {
            DumpNode(w, 0);
        }
#endif
    }    
}
