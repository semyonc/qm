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

namespace DataEngine.CoreServices
{
    public class FuncName
    {
        public Object ID { get; private set; }
        public Type[] Signature { get; private set; }
        public bool VariableLength { get; private set; }

        public int Arity
        {
            get
            {
                 return Signature.Length;
            }
        }

        public Type VariableParamType
        {
            get
            {
                if (!VariableLength)
                    return null;
                return Signature[Arity - 1];
            }
        }

        public FuncName(Object id)
        {
            ID = id;
            Signature = new Type[0];
            VariableLength = false;
        }

        public FuncName(Object id, Type[] signature)
        {
            ID = id;
            Signature = signature;
            VariableLength = false;
        }

        public FuncName(Object id, Executive.Parameter[] parameters)
        {
            ID = id;
            int len = parameters.Length;
            Signature = new Type[len];
            for (int k = 0; k < len; k++)
                Signature[k] = parameters[k].Type;
            if (len > 0 && parameters[len - 1].VariableParam)
                VariableLength = true;
        }        

        private void FormatTypeName(StringBuilder result, Type type)
        {
            if (type.IsGenericType)
            {
                string genericName = type.GetGenericTypeDefinition().FullName.Replace('+', '.');
                int tickIndex = genericName.IndexOf('`');
                result.Append(tickIndex != -1 ? genericName.Substring(0, tickIndex) : genericName);
                FormatTypeArgs(result, type.GetGenericArguments());
            }
            else if (type.IsGenericParameter)
            {
                result.Append(type.Name);
            }
            else
            {
                result.Append(type.FullName.Replace('+', '.'));
            }
        }

        private void FormatTypeArgs(StringBuilder result, Type[] types)
        {
            if (types.Length > 0)
            {
                result.Append("<");

                for (int i = 0; i < types.Length; i++)
                {
                    if (i > 0) result.Append(", ");
                    FormatTypeName(result, types[i]);
                }

                result.Append(">");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Arity != -1)
            {
                sb.AppendFormat("{0}/{1}(", ID, Arity);
                FormatTypeArgs(sb, Signature);
                sb.Append(")");
            }
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            int hashCode = ID.GetHashCode();
            foreach (Type type in Signature)
                hashCode = hashCode * 37 + type.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is FuncName)
                return Equals((FuncName)obj);
            else
                return base.Equals(obj);
        }

        public bool Equals(FuncName name)
        {
            if (name.ID == ID && name.Arity == Arity && 
                name.VariableLength == VariableLength)
            {
                for (int k = 0; k < Signature.Length; k++)
                    if (Signature[k] != name.Signature[k])
                        return false;
                return true;
            }
            else
                return false;
        }

        public Type GetParameterType(int index)
        {
            if (!VariableLength || index < Arity - 1)
                return Signature[index];
            else
                return Signature[Arity - 1];
        }

        public bool Match(FuncName name, bool anyType)
        {
            if (name.ID == ID)
            {
                if (name.Arity == Arity || (VariableLength && Arity < name.Arity))
                {
                    for (int k = 0; k < Signature.Length; k++)
                        if (anyType)
                        {
                            if (!Signature[k].IsAssignableFrom(name.Signature[k]))
                                return false;
                        }
                        else
                        {
                            if (Signature[k] != name.Signature[k])
                                return false;
                        }
                    if (VariableLength && Arity < name.Arity)
                    {
                        int last = Signature.Length - 1;
                        for (int k = Signature.Length; k < name.Arity; k++)
                            if (anyType)
                            {
                                if (!Signature[last].IsAssignableFrom(name.Signature[k]))
                                    return false;
                            }
                            else
                            {
                                if (Signature[last] != name.Signature[k])
                                    return false;
                            }
                    }
                    return true;
                }
                if (VariableLength && Arity == 1 && name.Arity == 0)
                    return true;
            }
            return false;
        }
    }

    public class MacroFuncName
    {
        public object ID { get; private set; }

        public int Arity { get; private set; }

        public bool VariableLength { get; private set; }

        public MacroFuncName(Object id, int arity)
        {
            ID = id;
            Arity = arity;
            VariableLength = false;
        }

        public MacroFuncName(Object id)
        {
            ID = id;
            Arity = -1;
            VariableLength = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Arity != -1)
            {
                if (VariableLength)
                    sb.Append(ID);
                else
                    sb.AppendFormat("{0}/{1}", ID, Arity);
                sb.Append(")");
            }
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is MacroFuncName)
                return Equals((MacroFuncName)obj);
            else
                return base.Equals(obj);
        }

        public bool Equals(MacroFuncName name)
        {
            if (name.ID == ID && name.Arity == Arity)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() * 37 + Arity.GetHashCode();
        }

        public bool Match(MacroFuncName name)
        {
            return ID == name.ID && (VariableLength || Arity == name.Arity);
        }

    }
}
