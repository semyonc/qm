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
using System.Data;
using System.Data.Common;
using System.Globalization;

using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine
{
    /// <summary>
    /// Implement universal table accessor for .NET data provider
    /// </summary>
    public class DataProviderTableAccessor: DataProviderAccessor
    {
        public class SortColumn
        {
            public String ColumnName { get; set; }
            public SortDirection Direction { get; set; }

            public SortColumn()
            {
            }

            public SortColumn(string columnName)
            {
                ColumnName = columnName;
            }

            public SortColumn(string columnName, SortDirection direction)
            {
                ColumnName = columnName;
                Direction = direction;
            }
        }

        public TableType TableType { get; private set; }

        public SortColumn[] SortColumns { get; set; }
       
        public String[] AccessPredicate { get; set; }
        public Object[][] AccessPredicateValues { get; set; }

        public DataProviderTableAccessor(TableType tableType)
            : base()
        {
            TableType = tableType;
            _providerInvariantName = TableType.DataSource.ProviderInvariantName;
            _connectionString = TableType.DataSource.ConnectionString;
        }

        public override Resultset Get(QueryContext queryContext, object[] parameters)
        {
            object[] parameterValues = null;
            if (_parameterBindings != null)
            {
                parameterValues = new object[_parameterBindings.Length];
                for (int k = 0; k < _parameterBindings.Length; k++)
                    parameterValues[k] = parameters[_parameterBindings[k]];
            }
            if (queryContext != null)
            {
                RowCache cache = queryContext.DataCache.Get(this, parameterValues);
                if (cache == null)
                {
                    Resultset rs = base.Get(queryContext, parameters);
                    if (queryContext.CacheEnabled && AccessPredicate == null)
                        queryContext.DataCache.Add(this, parameterValues, rs);
                    return rs;
                }
                else
                    return cache.GetResultset();
            }
            else
                return base.Get(queryContext, parameters);
        }

        protected override RowType CreateRowType()
        {
            DataTable dt = TableType.TableRowType.GetSchemaTable();
            return new RowType(dt);
        }

        protected override void PrepareCommand(RowType.TypeInfo[] fields, DbCommand command, Object[] parameters)
        {            
            DataProviderHelper helper = new DataProviderHelper(_providerInvariantName, _connectionString);
            Binder binder = new Binder(fields);
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            for (int k = 0; k < fields.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                if (fields[k].ProviderColumnName != null)
                    sb.Append(helper.FormatIdentifier(fields[k].ProviderColumnName));
                else
                    sb.Append(helper.FormatIdentifier(fields[k].Name));
            }
            sb.AppendLine();
            sb.Append(" FROM ");
            sb.Append(TableType.ToString(helper));
            if (TableType.Smart)
            {
                if (FilterPredicate != null || AccessPredicate != null)
                    sb.AppendLine(" WHERE ");
                if (FilterPredicate != null)
                {
                    if (AccessPredicate != null)
                        sb.Append("(");
                    command.Parameters.Clear();
                    sb.AppendLine(WriteExpr(binder, helper, FilterPredicate, 
                        parameters, command));
                    if (AccessPredicate != null)
                        sb.Append(")");
                }
                if (AccessPredicate != null)
                {
                    if (FilterPredicate != null)
                        sb.AppendLine(" AND (");
                    for (int s = 0; s < AccessPredicateValues.Length; s++)
                    {
                        if (s > 0)
                            sb.Append(" OR ");
                        sb.Append("(");
                        for (int k = 0; k < AccessPredicate.Length; k++)
                        {
                            if (k > 0)
                                sb.Append(" AND ");
                            sb.Append(helper.FormatIdentifier(AccessPredicate[k]));
                            Object predicateValue = AccessPredicateValues[s][k];
                            if (predicateValue == DBNull.Value)
                                sb.Append(" IS NULL");
                            else
                            {
                                sb.Append("=");
                                sb.Append(WriteLiteral(helper, predicateValue));
                            }
                        }
                        sb.Append(")");
                    }
                    if (FilterPredicate != null)
                        sb.Append(")");
                }
            }
            if (SortColumns != null && SortColumns.Length > 0)
            {
                sb.AppendLine(" ORDER BY ");
                for (int k = 0; k < SortColumns.Length; k++)               
                    {
                        if (k > 0)
                            sb.Append(", ");
                        sb.Append(helper.FormatIdentifier(SortColumns[k].ColumnName));
                        if (SortColumns[k].Direction == SortDirection.Descending)
                            sb.Append(" DESC");
                    }
                sb.AppendLine();
            }
            command.CommandText = sb.ToString();            
        }

        private String WriteLiteral(DataProviderHelper helper, Object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.DateTime:
                    return helper.FormatDateTime((DateTime)value);                    

                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    {
                        decimal n = Convert.ToDecimal(value);
                        return n.ToString(CultureInfo.InvariantCulture);
                    }

                default:
                    return helper.FormatLiteral(value.ToString());
            }
        }

        private String WriteList(Binder binder, DataProviderHelper helper, object[] list,
            Object[] parameters, DbCommand command)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            for (int k = 0; k < list.Length; k++)
            {
                if (k > 0)
                    sb.Append(", ");
                sb.Append(WriteExpr(binder, helper, list[k], parameters, command));
            }
            sb.Append(")");
            return sb.ToString();
        }

        private String WriteExpr(Binder binder, DataProviderHelper helper, object lval, 
            Object[] parameters, DbCommand command)
        {
            if (Lisp.IsNode(lval))
            {
                if (Lisp.IsAtom(lval))
                {
                    ATOM a = (ATOM)lval;
                    string name;
                    if (a.parts.Length == 1)
                        name = a.parts[0];
                    else
                        name = a.parts[1];
                    ColumnBinding b = binder.Get(name);
                    if (b == null)
                        throw new ESQLException(Properties.Resources.InvalidIdentifier, name);
                    RowType.TypeInfo ti = TableType.TableRowType.Fields[b.rnum];
                    if (ti.ProviderColumnName != null)
                        return helper.FormatIdentifier(ti.ProviderColumnName);
                    else
                        return helper.FormatIdentifier(ti.Name);
                }
                else
                    return WriteLiteral(helper, lval);
            }
            else
            {
                object head = Lisp.Car(lval);
                if (Lisp.IsCons(head))
                {
                    object[] list = Lisp.ToArray(lval);
                    return WriteList(binder, helper, list, parameters, command);
                }
                else
                {
                    object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                    if (head.Equals(ID.Like1))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("{0} LIKE {1}", WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                        if (args.Length > 2)
                            sb.AppendFormat(" ESCAPE {0}", WriteExpr(binder, helper, args[2], 
                                parameters, command));
                        return sb.ToString();
                    }
                    else if (head.Equals(Funcs.List))
                        return WriteList(binder, helper, args, parameters, command);
                    else if (head.Equals(ID.Member))
                        return String.Format(" {0} IN {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.Between))
                        return String.Format(" BETWEEN {0} AND {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(Funcs.Not))
                        return String.Format(" NOT {0}", WriteExpr(binder, helper, args[0], parameters, command));
                    else if (head.Equals(ID.IsNull))
                        return String.Format(" {0} IS NULL", WriteExpr(binder, helper, args[0], parameters, command));
                    else if (head.Equals(Funcs.And))
                        return String.Format(" ({0}) AND ({1})",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(Funcs.Or))
                        return String.Format(" ({0}) OR ({1})",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.LT))
                        return String.Format(" {0} < {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.GT))
                        return String.Format(" {0} > {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.EQ))
                        return String.Format(" {0} = {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.NE))
                        return String.Format(" {0} <> {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.LE))
                        return String.Format(" {0} <= {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.GE))
                        return String.Format(" {0} >= {1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.Concat))
                        return String.Format("CONCAT({0},{1})",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(Funcs.Add))
                        return String.Format(" {0}+{1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(Funcs.Sub))
                        return String.Format(" {0}-{1}",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(Funcs.Mul))
                        return String.Format(" ({0})*({1})",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(Funcs.Div))
                        return String.Format(" ({0})/({1})",
                            WriteExpr(binder, helper, args[0], parameters, command),
                            WriteExpr(binder, helper, args[1], parameters, command));
                    else if (head.Equals(ID.ParamRef))
                    {
                        DbParameter parameter = command.CreateParameter();
                        parameter.ParameterName = helper.FormatParameter(
                            String.Format("p{0}", command.Parameters.Count));
                        parameter.Value = parameters[(int)args[0] - 1];
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);
                        return parameter.ParameterName;
                    }
                    else
                        throw new UnproperlyFormatedExpr(Lisp.Format(lval));
                }
            }
        }

        private object _filterPredicate;
        private int[] _parameterBindings;

        public Object FilterPredicate
        {
            get
            {
                return _filterPredicate;
            }
            set
            {
                _filterPredicate = value;
                List<int> bindings = new List<int>();
                GetParameterBindings(value, bindings);
                if (bindings.Count == 0)
                    _parameterBindings = null;
                else
                    _parameterBindings = bindings.ToArray();
            }
        }

        private void GetParameterBindings(object lval, List<int> bindings)
        {
            if (!Lisp.IsNode(lval))
            {
                object head = Lisp.Car(lval);
                if (Lisp.IsCons(head))
                    foreach (object o in Lisp.getIterator(lval))
                        GetParameterBindings(o, bindings);
                else
                {
                    object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                    if (head.Equals(ID.ParamRef))
                        bindings.Add((int)args[0] -1);
                    else if (head.Equals(ID.Like1))
                    {
                        GetParameterBindings(args[0], bindings);
                        GetParameterBindings(args[1], bindings);
                        if (args.Length > 2)
                            GetParameterBindings(args[2], bindings);
                    }
                    else if (head.Equals(ID.Member) || head.Equals(ID.Between) || head.Equals(Funcs.Div) ||
                        head.Equals(Funcs.And) || head.Equals(Funcs.Or) || head.Equals(ID.LT) ||
                        head.Equals(ID.GT) || head.Equals(ID.EQ) || head.Equals(ID.NE) ||
                        head.Equals(ID.LE) || head.Equals(ID.GE) || head.Equals(ID.Concat) ||
                        head.Equals(Funcs.Add) || head.Equals(Funcs.Sub) || head.Equals(Funcs.Mul))
                    {
                        GetParameterBindings(args[0], bindings);
                        GetParameterBindings(args[1], bindings);
                    }
                    else if (head.Equals(Funcs.List))
                        foreach (object arg in args)
                            GetParameterBindings(args[0], bindings);
                    else
                        throw new UnproperlyFormatedExpr(Lisp.Format(lval));
                }
            }
        }

#if DEBUG
        protected override void Dump(System.IO.TextWriter w, int padding)
        {
            OutlineNode(w, padding);
            DataProviderHelper helper = new DataProviderHelper(TableType);
            w.Write("DataProviderTableAccessor {0},{1}",
                TableType.DataSource.ProviderInvariantName, TableType.ToString(helper));
            if (_filterPredicate != null)
            {
                w.Write(" ");
                w.Write(Lisp.Format(_filterPredicate));
            }
            w.WriteLine();
        }
#endif
    }
}
