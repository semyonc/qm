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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading;

using DataEngine;
using DataEngine.Parser;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine.ADO
{

    public delegate void QueryExecuteDelegate(Command source, Notation notation, 
        Optimizer optimizer, QueryContext context);

    public class Command: DbCommand
    {
        private String _commandText;
        private String _searchPath;
        
        private bool _designTimeVisible;        
        private ParameterCollection _parameters = new ParameterCollection();

        public Command()
            : base()
        {
        }

        public Command(DatabaseDictionary dictionary)
            : base()
        {
            DatabaseDictionary = dictionary;
        }

        public override void Cancel()
        {
            return;
        }

        public override string CommandText
        {
            get
            {
                return _commandText;
            }
            set
            {
                _commandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override CommandType CommandType
        {
            get
            {
                return CommandType.Text;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string SearchPath
        {
            get
            {
                return _searchPath;
            }
            set
            {
                _searchPath = value;
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return new Parameter();
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _parameters; }
        }

        public new ParameterCollection Parameters
        {
            get { return _parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                return _designTimeVisible;
            }
            set
            {
                _designTimeVisible = value;
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (behavior != CommandBehavior.Default)
                throw new NotSupportedException();
            Notation notation = new Notation();
            YYParser parser = new YYParser(notation);
            object result = parser.yyparseSafe(new Tokenizer(_commandText));
            QueryContext context = new QueryContext(DatabaseDictionary);
            context.DatabaseDictionary.SearchPath = _searchPath;
            Optimizer optimizer = new Optimizer(context);
            optimizer.Process(notation);
            QueryBinder binder = new QueryBinder();
            binder.IsServerQuery = optimizer.IsServerQuery;
            binder.Process(notation);
            optimizer.PostProcess(notation);
            if (BeforeExecute != null)
                BeforeExecute(this, notation, optimizer, context);
            Notation.Record[] recs = notation.Select(Descriptor.Root, 1);
            Object[] raw_parameters = null;
            if (recs.Length > 0)
            {
                Notation.Record[] recsd = notation.Select(recs[0].Arg0, Descriptor.Binding, 1);
                if (recsd.Length > 0)
                {
                    Symbol[] bindings = Lisp.ToArray<Symbol>(recsd[0].args[0]);
                    raw_parameters = new object[bindings.Length];
                    for (int k = 0; k < bindings.Length; k++)
                    {
                        recsd = notation.Select(bindings[k], Descriptor.Link, 1);
                        if (recsd.Length > 0)
                        {
                            DataEngine.Parameter p = (DataEngine.Parameter)recsd[0].Arg0;
                            raw_parameters[k] = _parameters[p.ParameterName].Value;
                        }
                    }
                }
            }
            Translator translator = new Translator(context);
            QueryNode rootNode = translator.Process(notation);
            Resultset rs = rootNode.Get(context, raw_parameters);
            optimizer.ProcessResults(rs);
            return new DataReader(rs, context);
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            DbDataReader reader = ExecuteDbDataReader(CommandBehavior.Default);
            try
            {
                if (reader.Read())
                    return reader[0];
                else
                    return null;
            }
            finally
            {
                reader.Close();
            }
        }

        public override void Prepare()
        {
            return;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return UpdateRowSource.None;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DatabaseDictionary DatabaseDictionary { get; set; }

        public event QueryExecuteDelegate BeforeExecute;
    }
}

