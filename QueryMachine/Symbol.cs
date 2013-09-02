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

using DataEngine.CoreServices;
using DataEngine.Parser;

namespace DataEngine
{

    public enum Tag
    {
        Stmt,
        QueryExp,
        QueryTerm,
        SQuery,
        Join,
        ExplictTable,
        TableFields,
        RowValue,
        TableConstructor,
        BooleanExpr,
        Expr,
        CExpr,
        CaseExpr,
        AggExpr,
        Predicate,
        ValueList,
        Funcall,
        Dynatable,
        Tuple,
        Binding,
        Literal,
        Integer,
        Decimal,
        Double,
        DateTime,
        Qname,
        Parameter,
        Placeholder,
        TokenWrapper,
        Dref,
        SQLX
    }

    public enum Axis
    {
        Self
    }

    public class AxisParser
    {
        public static Axis Parse(string axis)
        {
            if (axis.Equals("self", StringComparison.InvariantCultureIgnoreCase))
                return Axis.Self;
            else
                throw new ESQLException(Properties.Resources.InvalidAxisToken, axis);
        }
    }

    public class Symbol: ICloneable
    {
        protected Tag tag;

        public Symbol(Tag tag)
        {
            this.tag = tag;
#if DEBUG
            int index;
            if (!itab.TryGetValue(tag, out index))
                index = 1;
            itab[tag] = index + 1;
            name = String.Format("_{0}{1}", tag, index);
#endif
        }

        public virtual bool IsAbstract
        {
            get
            {
                return true;
            }
        }

        public Tag Tag
        {
            get
            {
                return tag;
            }
        }

#if DEBUG
        public static Dictionary<Tag, int> itab = new Dictionary<Tag, int>();
        private readonly string name;

        public override string ToString()
        {
            return name;
        }
#endif

        #region ICloneable Members

        public virtual object Clone()
        {
            return new Symbol(tag);
        }

        #endregion
    }

    public class Value : Symbol
    {
        protected object data;

        public Value(object data) :
            base(Tag.Expr)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public override bool IsAbstract
        {
            get
            {
                return false;
            }
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Value)
            {
                Value value = (Value)obj;
                return data.Equals(value.data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }
    }

    public class Literal : Value
    {
        public Literal(object data)
            : base(data)
        {
            tag = Tag.Literal;
        }

        public override string ToString()
        {
            return "'" + data.ToString() + "'";
        }

        new public string Data
        {
            get
            {
                return (string)data;
            }
        }

        public override object Clone()
        {
            return new Literal(Data);
        }        
    }

    public class IntegerValue : Value
    {
        public IntegerValue(object data)
            : base(data)
        {
            tag = Tag.Integer;
        }

        new public int Data
        {
            get
            {
                return (int)data;
            }
        }

        public override object Clone()
        {
            return new IntegerValue(Data);
        }
    }

    public class DecimalValue : Value
    {
        public DecimalValue(object data)
            : base(data)
        {
            tag = Tag.Decimal;
        }

        new public decimal Data
        {
            get
            {
                return (decimal)data;
            }
        }

        public override object Clone()
        {
            return new DecimalValue(Data);
        }
    }

    public class DoubleValue : Value
    {
        public DoubleValue(object data)
            : base(data)
        {
            tag = Tag.Double;
        }

        new public double Data
        {
            get
            {
                return (double)data;
            }
        }

        public override object Clone()
        {
            return new DoubleValue(Data);
        }
    }

    public class DateTimeValue : Value
    {
        public DateTimeValue(object data)
            : base(data)
        {
            tag = Tag.DateTime;
        }

        new public DateTime Data
        {
            get
            {
                return (DateTime)data;
            }
        }

        public override object Clone()
        {
            return new DateTimeValue(Data);
        }
    }

    public class Qname : Value
    {
        public Qname()
            : base(null)
        {
            tag = Tag.Qname;
        }

        public Qname(object name)
            : this()
        {
            data = Lisp.Cons(name);
        }

        public Qname(string[] identifierPart)
            : base(null)
        {
            tag = Tag.Qname;
            data = Lisp.List(identifierPart);
        }

        public void Append(string name)
        {
            data = Lisp.Append(data, Lisp.Cons(name));
        }

        public string[] ToArray()
        {
            return Lisp.ToArray<string>(data);
        }

        public string ToString(string delimiter)
        {
            string[] parts = ToArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                    sb.Append(delimiter);
                sb.Append(Util.UnquoteName(parts[i]));
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(".");
        }

        public int Length
        {
            get
            {
                return Lisp.Length(data);
            }
        }

        public Boolean IsNonQualifiedName
        {
            get
            {
                return Lisp.Cdr(data) == null;
            }
        }

        public String Qualifier
        {
            get
            {
                string[] arr = ToArray();
                if (arr.Length > 1)
                    return arr[0];
                else
                    return null;
            }
        }

        public String Name
        {
            get
            {
                string[] arr = ToArray();
                return arr[arr.Length -1];
            }
        }

        public String UnqoutedName
        {
            get
            {
                return Util.UnquoteName(Name);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Qname)
            {
                Qname dest = (Qname)obj;
                return Lisp.IsEqual(data, dest.data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override object Clone()
        {
            Qname qname = new Qname();
            qname.data = data;
            return qname;
        }
    }

    public class Parameter : Value
    {
        public Parameter(string parameterName) :
            base(parameterName)
        {
            tag = Tag.Parameter;
        }

        public string ParameterName
        {
            get
            {
                return data.ToString();
            }
        }

        public override string ToString()
        {
            return ParameterName;
        }

        public override object Clone()
        {
            return new Parameter(ParameterName);
        }
    }

    public class Placeholder : Value
    {
        public Placeholder(int value) :
            base(value)
        {
            tag = Tag.Placeholder;
        }

        public int Index
        {
            get
            {
                return (int)data;
            }
        }

        public override string ToString()
        {
            return String.Format("${0}", Index);
        }

        public override object Clone()
        {
            return new Placeholder(Index);
        }
    }

    public class TokenWrapper : Value
    {
        public TokenWrapper(object data)
            : base(data)
        {
            tag = Tag.TokenWrapper;
        }

        new public int Data
        {
            get
            {
                return (int)data;
            }
        }

        public override string ToString()
        {
            return "Token." + YYParser.yyname(Data);
        }

        public override object Clone()
        {
            return new TokenWrapper(data);
        }
    }

    //public class FieldDescriptor : Value
    //{
    //    public string Name;
    //    public Type FieldType;
    //    public int Size;
    //    public short Precision;
    //    public byte Scale;
    //    public bool IsLong;
    //    public bool AllowDbNull;
    //    public bool IsUnique;
    //    public bool IsKey;
    //    public string BaseSchemaName;
    //    public string BaseTableName;
    //    public string BaseColumnName;
    //}

}
