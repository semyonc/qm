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

namespace DataEngine.CoreServices
{
    public class Util
    {
        public static bool IsRowConstuctor(object lval)
        {
            return Lisp.IsFunctor(lval, Funcs.List);
        }
        
        public static string CreateUniqueName(List<string> fieldNames, string name)
        {
            bool quoted = false;
            if (String.IsNullOrEmpty(name))
                name = "Unknown";
            else
                if (name.StartsWith("\"") && name.EndsWith("\""))
                    quoted = true;
            string curr = name;
            for (int n = 1; ; n++)
            {
                if (!fieldNames.Contains(curr))
                    break;
                if (quoted)
                    curr = QuoteName(String.Format("{0}({1})", UnquoteName(name), n));
                else
                    curr = String.Format("{0}{{{1}}}", name, n);
            }
            fieldNames.Add(curr);
            return curr;
        }

        public static void ParseCollectionName(string name, out string prefix, out string[] identifierPart)
        {
            string[] parts = name.Split(':');
            if (parts.Length == 1)
            {
                prefix = String.Empty;
                identifierPart = new string[] { parts[0] };
            }
            else
            {
                prefix = parts[0];
                identifierPart = SplitName(parts[1], '[', ']');
                for (int k = 0; k < identifierPart.Length; k++)
                    if (identifierPart[k].StartsWith("[") && identifierPart[k].EndsWith("]"))
                        identifierPart[k] = identifierPart[k].Substring(1, identifierPart[k].Length - 2);
            }
        }

        public static String[] SplitName(String name)
        {
            List<string> res = new List<string>();
            int anchor = 0;
            bool isLiteral = false;
            for (int k = 0; k < name.Length; k++)
            {
                if (name[k] == '.' && !isLiteral)
                {
                    res.Add(name.Substring(0, k - anchor));
                    anchor = k + 1;
                }
                else if (name[k] == '\"')
                {
                    if (isLiteral)
                    {
                        if (!(k < name.Length - 1 && name[k + 1] == '\"'))
                            isLiteral = false;
                    }
                    else
                        isLiteral = true;
                }
            }
            if (anchor < name.Length)
                res.Add(name.Substring(anchor));
            return res.ToArray();
        }

        public static String[] SplitName(String name, char leftPar, char rightPar)
        {
            List<string> res = new List<string>();
            int anchor = 0;
            bool isLiteral = false;
            for (int k = 0; k < name.Length; k++)
            {
                if (name[k] == '.' && !isLiteral)
                {
                    res.Add(name.Substring(anchor, k - anchor));
                    anchor = k + 1;
                }
                else if (name[k] == leftPar)
                    isLiteral = true;
                else if (name[k] == rightPar && isLiteral)
                {
                     if (!(k < name.Length - 1 && name[k + 1] == rightPar))
                         isLiteral = false;
                }
            }
            if (anchor < name.Length)
                res.Add(name.Substring(anchor));
            return res.ToArray();
        }

        public static bool EqualsName(String name1, String name2, bool caseSensitive)
        {
            bool ignoreCase;
            if ((name1.StartsWith("\"") && name1.EndsWith("\"")) ||
                (name2.StartsWith("\"") && name2.EndsWith("\"")))
                ignoreCase = false;
            else
                ignoreCase = !caseSensitive;
            return String.Compare(UnquoteName(name1), UnquoteName(name2), ignoreCase) == 0;
        }

        public static String QuoteName(String name)
        {
            return String.Format("\"{0}\"", name.Replace("\"", "\"\""));
        }

        public static bool IsQuotedName(String name)
        {
            return name.StartsWith("\"") && name.EndsWith("\"");
        }

        public static String UnquoteName(String name)
        {
            if (IsQuotedName(name))
                return name.Substring(1, name.Length - 2).Replace("\"\"", "\"");
            else
                return name;
        }

        public static String[] UnquoteName(String[] parts)
        {
            string[] res = new string[parts.Length];
            for (int k = 0; k < res.Length; k++)
                res[k] = UnquoteName(parts[k]);
            return res;
        }

        public static bool IsNull(Object value)
        {
            return value == null || value == DBNull.Value;
        }

        public static String Substring(String str, int index, int length)
        {
            if (str.Length < index)
                return String.Empty;
            else
                if (str.Length < index + length)
                    return str.Substring(index);
                else
                    return str.Substring(index, length);
        }

        public static bool ParseBool(string val, bool defval)
        {
            if (val == "1" || val == "T" || val == "True" ||
                            val == "t" || val == "true")
                return true;
            else
                if (val == "0" || val == "F" || val == "False" ||
                    val == "f" || val == "false")
                    return false;
            return defval;
        }

        private static TypeCode[] _typeSeniority = 
            new TypeCode[]  
                { TypeCode.String,
                  TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16,
                  TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64,
                  TypeCode.Single, TypeCode.Double, TypeCode.Decimal, TypeCode.DateTime };

        public static TypeCode GetTypeCode(Object object1, Object object2)
        {
            return GetTypeCode(object1.GetType(), object2.GetType());
        }

        public static TypeCode GetTypeCode(Type t1, Type t2)
        {
            return GetTypeCode(Type.GetTypeCode(t1), Type.GetTypeCode(t2));
        }

        public static TypeCode GetTypeCode(TypeCode typecode1, TypeCode typecode2)
        {
            if (typecode1 == typecode2)
                return typecode1;
            else if (typecode1 == TypeCode.Object || typecode2 == TypeCode.Object)
                return TypeCode.Object;
            else
            {
                int index1 = -1;
                for (int k = 0; k < _typeSeniority.Length; k++)
                    if (_typeSeniority[k] == typecode1)
                    {
                        index1 = k;
                        break;
                    }
                if (index1 == -1)
                    throw new InvalidCastException();

                int index2 = -1;
                for (int k = 0; k < _typeSeniority.Length; k++)
                    if (_typeSeniority[k] == typecode2)
                    {
                        index2 = k;
                        break;
                    }
                if (index2 == -1)
                    throw new InvalidCastException();

                TypeCode typecode;
                if (index1 > index2)
                    typecode = typecode1;
                else
                    typecode = typecode2;

                switch (typecode)
                {
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        return TypeCode.Int32;

                    case TypeCode.Byte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                        return TypeCode.UInt32;

                    default:
                        return typecode;
                }
            }
        }        
    }
}
