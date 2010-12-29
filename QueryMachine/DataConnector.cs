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

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;
using System.Data;

namespace DataEngine
{
    public enum ConnectorType
    {    
        ComplexUnion,
        Union,
        Intersect,
        Except
    }

    public class DataConnector : QueryNode
    {

        protected class DataConcateContext : DemandProcessingContext
        {
            private Resultset _source1;
            private Resultset _source2;

            public DataConcateContext(Resultset source1, Resultset source2)
                : base(new Resultset[] { source1, source2 })
            {
                _source1 = source1;
                _source2 = source2;
            }

            public override bool ProcessNextPiece(Resultset rs)
            {
                if (_source1.Begin != null)
                {
                    rs.Enqueue(_source1.Dequeue());
                    return true;
                }
                else if (_source2.Begin != null)
                {
                    rs.Enqueue(_source2.Dequeue());
                    return true;
                }
                return false;
            }
        }

        private ConnectorType _type;
        private bool _allTag;

        public ConnectorType Type
        {
            get
            {
                return _type;
            }
        }

        public DataConnector(ConnectorType type, bool allTag)
        {
            _type = type;
            _allTag = allTag;
            _childs = new QueryNodeCollection(this);
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            if (ChildNodes.Count != 2)
                throw new InvalidOperationException();

            Resultset rs1 = null;
            Resultset rs2 = null;

            Iterator.Invoke(new Action[] {
                () => rs1 = ChildNodes[0].Get(queryContext, parameters),
                () => rs2 = ChildNodes[1].Get(queryContext, parameters)
            });

            if (!rs1.RowType.RowTypeEquals(rs2.RowType))
                throw new ESQLException(Properties.Resources.IncorrectSubqueries);

            RowType.TypeInfo[] typeInfo = new RowType.TypeInfo[rs1.RowType.Fields.Length];
            for (int k = 0; k < typeInfo.Length; k++)
                if (_type == ConnectorType.ComplexUnion)
                    typeInfo[k] = rs1.RowType.Fields[k];
                else
                    typeInfo[k] = new RowType.TypeInfo(k,
                        rs1.RowType.Fields[k], rs2.RowType.Fields[k]);

            DemandProcessingContext context;
            if (_type == ConnectorType.Union && _allTag)
            {
                context = new DataConcateContext(rs1, rs2);
                return new Resultset(new RowType(typeInfo), context);
            }
            else
            {
                if (_allTag)
                    throw new NotImplementedException();
                Resultset rs = new Resultset(new RowType(typeInfo), null);
                IEqualityComparer<Row> comp;
                if (_type == ConnectorType.ComplexUnion)
                    comp = new RowEqualityComplexComparer();
                else
                    comp = new RowEqualityComparer();
                HashSet<Row> set1 = new HashSet<Row>(comp);
                HashSet<Row> set2 = new HashSet<Row>(comp);
                Iterator.Invoke(new Action[] {
                () => {
                    while (rs1.Begin != null)
                        set1.Add(rs1.Dequeue()); },
                () => {
                    while (rs2.Begin != null)
                        set2.Add(rs2.Dequeue()); }});
                switch (_type)
                {
                    case ConnectorType.Union:
                    case ConnectorType.ComplexUnion:
                        set1.UnionWith(set2);
                        break;

                    case ConnectorType.Except:
                        set1.ExceptWith(set2);
                        break;

                    case ConnectorType.Intersect:
                        set1.IntersectWith(set2);
                        break;
                }
                foreach (Row row in set1)
                    rs.Enqueue(row);
                return rs;
            }

        }
    }
}
