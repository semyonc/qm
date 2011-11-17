//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using DataEngine.CoreServices;
using DataEngine.XQuery.Parser;

namespace DataEngine.XQuery
{

    public enum Tag
    {
        Stmt,
        Expr,
        Constructor,        
        Module,
        Operator,
        Modifier,
        Literal,
        Integer,
        Double,
        Decimal,
        Qname,
        VarName,
        Parameter,
        Placeholder,
        TokenWrapper,
        PredefinedEntityRef,
        CharRef,
        CommonContent,
        CData,
        CompConstructor
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
        public Char Quote { get; private set; }

        public Literal(object data)
            : base(data)
        {
            tag = Tag.Literal;            
        }

        public Literal(object data, char quote)
            : base(data)
        {
            tag = Tag.Literal;
            Quote = quote;
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
            return new Literal(Data, Quote);
        }        
    }

    public class IntegerValue : Value
    {
        public IntegerValue(object data)
            : base(data)
        {
            tag = Tag.Integer;
        }

        public override object Clone()
        {
            return new IntegerValue(Data);
        }
    }

    public class DoublelValue : Value
    {
        public DoublelValue(object data)
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
            return new DoublelValue(Data);
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
            return new DecimalValue(data);
        }
    }

    public class Qname : Value
    {
        public Qname()
            : base(null)
        {
            tag = Tag.Qname;
        }

        public Qname(string name)
            : this()
        {
            data = name;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public String Name
        {
            get
            {
                return (String)data;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Qname)
            {
                Qname dest = (Qname)obj;
                return dest.data.Equals(data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override object Clone()
        {
            Qname qname = new Qname();
            qname.data = data;
            return qname;
        }
    }

    public class VarName : Value
    {
        public VarName(string prefix,string localName)
            : base(prefix + ":" + localName)
        {
            Prefix = prefix;
            LocalName = localName;
            tag = Tag.VarName;
        }

        public override string ToString()
        {
            return '$' + data.ToString();
        }

        public String Prefix { get; private set; }

        public String LocalName { get; private set; }

        public String Name
        {
            get
            {
                return (String)data;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is VarName)
            {
                VarName dest = (VarName)obj;
                return dest.data.Equals(data);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }

        public override object Clone()
        {
            return new VarName(Prefix, Name);
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
                return Convert.ToInt32(data);
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

    public class PredefinedEntityRef : Value
    {
        public PredefinedEntityRef(string data)
            : base(data)
        {
            tag = Tag.PredefinedEntityRef;
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
            return new PredefinedEntityRef(Data);
        }
    }

    public class CharRef : Value
    {
        public CharRef(string data)
            : base(data)
        {
            tag = Tag.CharRef;
        }

        new public string Data
        {
            get
            {
                return (string)data;
            }
        }

        public override string ToString()
        {
            return String.Format("&{0};", data);
        }

        public override object Clone()
        {
            return new CharRef(Data);
        }
    }

    public class CharRefHex : Value
    {
        public CharRefHex(string data)
            : base(data)
        {
            tag = Tag.CharRef;
        }

        new public string Data
        {
            get
            {
                return (string)data;
            }
        }

        public override string ToString()
        {
            return String.Format("&x{0};", data);
        }

        public override object Clone()
        {
            return new CharRefHex(Data);
        }
    }    
}
