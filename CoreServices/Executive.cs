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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using DataEngine.CoreServices.Generation;
using System.Threading;
using System.IO;

namespace DataEngine.CoreServices
{
    /// <summary>
    /// Compact lisp engine implementation
    /// </summary>
    public class Executive
    {
        private object m_owner;
#if DEBUG
        public TextWriter m_traceOutput = null;
#endif
      
        public struct Parameter
        {
            public object ID;
            public Type Type;
            public bool VariableParam;
        }

        public delegate object UnaryOperatorDelegate(object arg1);
        public delegate object BinaryOperatorDelegate(object arg1, object arg2);

        private Dictionary<object, SymbolLink> m_value;
        private Dictionary<object, ControlFormBase> m_control;
        private Dictionary<object, FuncDef> m_func;
        private List<ValueConverter> m_converter;
        private Dictionary<object, MacroFuncDef> m_macro;
        private Stack<Resolver> m_resolvers = new Stack<Resolver>();

        public Executive(object owner)
        {
            m_owner = owner;
            
            m_control = GlobalSymbols.Shared.CreateControls();
            m_func = GlobalSymbols.Shared.CreateFuncs();
            m_converter = GlobalSymbols.Shared.CreateConverters();
            m_macro = GlobalSymbols.Shared.CreateMacros();
            
            m_value = new Dictionary<object, SymbolLink>();

            Set(Lisp.T, Generation.RuntimeOps.True);
            Set(Lisp.NIL, null);
            Set(Lisp.UNKNOWN, Undefined.Value);
        }

        public SymbolLink Set(string Name, object value)
        {
            return Set(Lisp.Defatom(new string[] {Name}), value);            
        }

        public SymbolLink Set(object atom, object value)
        {
            SymbolLink link;
            if (!m_value.TryGetValue(atom, out link))
            {
                if (!Lisp.IsAtom(atom))
                    throw new ArgumentException();
                link = new SymbolLink();
                m_value.Add(atom, link);
            }
            link.Value = value;
            return link;
        }

        public void Set(object atom, SymbolLink link)
        {
            SymbolLink link2;
            if (m_value.TryGetValue(atom, out link2))
                throw new ArgumentException("Value already defined");
            m_value.Add(atom, link);
        }

        public void Unbind(object atom)
        {
            m_value.Remove(atom);
        }

        public void Enter(Resolver resolver)
        {
            m_resolvers.Push(resolver);
        }

        public void Leave()
        {
            m_resolvers.Pop();
        }

        public Resolver CurrentResolver()
        {
            return m_resolvers.Peek();
        }

        public SymbolLink TryGet(object atom)
        {
            SymbolLink link;
            if (!m_value.TryGetValue(atom, out link))
            {
                Resolver[] rls = m_resolvers.ToArray();
                for (int k = 0; k < rls.Length; k++)
                    if (rls[k].Get(atom, out link))
                        return link;
                return null;
            }
            return link;
        }

        public SymbolLink Get(object atom)
        {
            SymbolLink link = TryGet(atom);
            if (link == null)
                throw new ArgumentException("Value not defined", atom.ToString());
            return link;
        }

        public object GetValue(object atom)
        {
            return Get(atom).Value;
        }
   
        public void Defun(FuncBase func)
        {
            FuncDef def;
            if (!m_func.TryGetValue(func.Name.ID, out def))
            {
                def = new FuncDef(func);
                m_func.Add(def.ID, def);
            }
            else
                def.Overload(func);
        }        

        private ValueConverter FindConverter(Type source, Type dest)
        {
            if (source == dest)
                return new ILConverter();
            else
            {
                foreach (ValueConverter conv in m_converter)
                    if (conv.Destination == dest)
                    {
                        foreach (Type type in conv.Source)
                            if (type == source)
                                return conv;
                    }
                return null;
            }
        }

        private FuncBase GetFunc(FuncName name, bool anyType)
        {
            FuncDef def;
            if (m_func.TryGetValue(name.ID, out def))
                return def.FindMatched(name, anyType);
            else
                throw new UnknownFuncCall(name.ToString());
        }

        public void Defmacro(MacroFuncBase func)
        {
            MacroFuncDef def;
            if (!m_macro.TryGetValue(func.Name.ID, out def))
            {
                def = new MacroFuncDef(func);
                m_macro.Add(def.ID, def);
            }
            else
                def.Overload(func);
        }

        public object MacroExpand(object lval, ref bool proceed)
        {
            if (Lisp.IsFunctor(lval))
            {
                object head = Lisp.Car(lval);
                object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                MacroFuncName name = new MacroFuncName(head, args.Length);
                MacroFuncDef def;
                if (m_macro.TryGetValue(head, out def))
                {
                    foreach (MacroFuncBase macro in def)
                    {
                        bool completed = false;
                        object res = macro.Execute(this, args, out completed);
                        if (completed)
                        {
                            proceed = true;
                            return res;
                        }
                    }
                }
                ControlFormBase control;
                if (m_control.TryGetValue(head, out control))
                    return control.MacroExpand(this, args, ref proceed);
                else
                    return MacroExpand(head, args, ref proceed);
            }
            else
                return lval;
        }

        internal object MacroExpand(object head, object[] lval, ref bool proceed)
        {
            object[] form = new object[lval.Length + 1];
            form[0] = head;
            for (int k = 0; k < lval.Length; k++)
                form[k + 1] = MacroExpand(lval[k], ref proceed);
            return Lisp.List(form);
        }

        public Type Compile(Parameter[] parameters, object expr, SymbolLink dynamicFunc)
        {
            CompiledLambda lambda = Compile(parameters, expr);
            if (dynamicFunc != null)
                dynamicFunc.Value = lambda;
            return lambda.ReturnType;
        }

        internal CompiledLambda Compile(Parameter[] parameters, object expr)
        {
#if DEBUG
            if (m_traceOutput != null)
            {
                Monitor.Enter(m_traceOutput);
                m_traceOutput.WriteLine("------ Begin snipped ---------");
                m_traceOutput.WriteLine("Body: {0}", expr);
                if (parameters != null)
                {
                    foreach (Parameter p in parameters)
                    {
                        m_traceOutput.Write("\t{0}:{1}", p.ID, p.Type);
                        if (p.VariableParam)
                            m_traceOutput.Write("[&REST]");
                        m_traceOutput.WriteLine();
                    }
                }
            }
            try
            {
                //Console.WriteLine("[{1}] Compile: {0}", expr, expr.GetHashCode());
#endif
                while (true)
                {
                    bool proceed = false;
                    expr = MacroExpand(expr, ref proceed);
                    if (!proceed)
                        break;
                }
#if DEBUG
                if (m_traceOutput != null)
                    m_traceOutput.WriteLine("Expanded: {0}", expr);
#endif
                Stack<Type> st = new Stack<Type>();
                if (parameters == null)
                    parameters = new Parameter[0];
                CompiledLambda lambda = new CompiledLambda(this, parameters);
#if DEBUG
                ILGen il = lambda.CreateILGen(m_traceOutput);
#else
                ILGen il = lambda.CreateILGen();
#endif
                LocalAccess localAccess = new LocalAccess(il);
                foreach (Parameter p in parameters)
                    localAccess.BindParameter(p.ID, p.Type);
                CreateVariables(il, localAccess, expr);
                WriteEpilog(il, localAccess);
                CompileExpr(il, localAccess, st, expr);
                lambda.Values = localAccess.GetValues();
                lambda.Consts = localAccess.GetConsts();
                lambda.ReturnType = st.Pop();
                if (lambda.ReturnType.IsValueType)
                    il.EmitBoxing(lambda.ReturnType);
                il.Emit(OpCodes.Ret);
                return lambda;
#if DEBUG
            }
            finally
            {
                if (m_traceOutput != null)
                {
                    m_traceOutput.WriteLine();
                    Monitor.Exit(m_traceOutput);
                }
            }
#endif
        }

        private Parameter[] CreateParameterTypes(object[] param, object[] value)
        {
            if (param != null && value != null)
            {
                if (param.Length == value.Length)
                {                    
                    Parameter[] res = new Parameter[param.Length];
                    for (int k = 0; k < param.Length; k++)
                    {
                        res[k].ID = param[k];
                        if (value[k] != null && !Lisp.IsAtom(value[k]))
                            res[k].Type = value[k].GetType();
                        else
                            res[k].Type = typeof(System.Object);
                    }
                    return res;
                }
                else
                    throw new ArgumentException("Lambda parameters does not match values", "args");
            }
            else
                if (param == null && value == null)
                    return new Parameter[0];
                else
                    throw new ArgumentException("Lambda parameters does not match values", "args");            
        }

        internal void CreateVariables(ILGen il, LocalAccess locals, object lval)
        {
            if (Lisp.IsNode(lval))
            {
                if (Lisp.IsAtom(lval) && lval != Lisp.NIL && lval != Lisp.T && lval != Lisp.INST)
                    locals.Bind(this, lval);
            }
            else if (Lisp.IsFunctor(lval))
            {
                object head = Lisp.Car(lval);
                object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                ControlFormBase control;
                if (m_control.TryGetValue(head, out control))
                    control.CreateVariables(this, lval, il, locals, args);
                else
                {
                    foreach (object arg in args)
                        CreateVariables(il, locals, arg);
                }
            }
            else
            {
                object[] args = Lisp.ToArray(lval);
                if (Lisp.IsFunctor(Lisp.Car(lval), Lisp.LAMBDA))
                {
                    for (int k = 1; k < args.Length; k++)
                        CreateVariables(il, locals, args[k]);
                }
                else
                    foreach (object arg in args)
                        CreateVariables(il, locals, arg);
            }
        }

        private void WriteEpilog(ILGen il, LocalAccess locals)
        {
            if (locals.HasLocals)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.EmitPropertyGet(typeof(CompiledLambda), "Values");
                LocalBuilder values = il.DeclareLocal(typeof(SymbolLink[]));
                il.Emit(OpCodes.Stloc, values);
                int k = 0;
                Label label = il.DefineLabel();
                foreach (LocalAccess.LocalBinding b in locals)
                {
                    il.Emit(OpCodes.Ldloc, values);
                    il.EmitInt(k++);
                    il.Emit(OpCodes.Ldelem_Ref);
                    il.EmitPropertyGet(typeof(SymbolLink), "Value");
                    if (b.Local.LocalType.IsValueType)
                    {
                        il.Emit(OpCodes.Isinst, b.Local.LocalType);
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Brfalse, label);
                        il.EmitUnbox(b.Local.LocalType);
                        il.Emit(OpCodes.Stloc, b.Local);
                    }
                    else
                    {
                        Label label2 = il.DefineLabel();
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Brfalse_S, label2);
                        il.Emit(OpCodes.Isinst, b.Local.LocalType);
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Brfalse, label);
                        il.MarkLabel(label2);
                        il.Emit(OpCodes.Stloc, b.Local);
                    }
                }
                Label end = il.DefineLabel();
                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(label);
                il.Emit(OpCodes.Pop); // Isinst
                il.EmitPropertyGet(typeof(Undefined), "Value");
                il.Emit(OpCodes.Ret);
                il.MarkLabel(end);
                il.FreeLocal(values);
            }
        }

        internal void CompileExpr(ILGen il, LocalAccess locals, Stack<Type> st, object lval)
        {
            if (lval == null)
            {
                il.EmitNull();
                st.Push(typeof(System.Object));
            }
            else if (Lisp.IsNode(lval))
            {
                if (lval == Lisp.NIL || lval == Lisp.T)
                {
                    if (lval == Lisp.NIL)
                        il.EmitNull();
                    else
                        il.Emit(OpCodes.Ldsfld, typeof(RuntimeOps).GetField("True"));
                    st.Push(typeof(System.Object));
                }
                else if (lval == Lisp.INST)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.EmitPropertyGet(typeof(CompiledLambda), "Engine");
                    st.Push(typeof(Executive));
                }
                else
                {
                    LocalBuilder localVar = locals.GetLocal(lval);
                    if (localVar != null)
                    {
                        il.Emit(OpCodes.Ldloc, localVar);
                        st.Push(localVar.LocalType);
                    }
                    else
                    {
                        Type type = lval.GetType();
                        if (!il.TryEmitConstant(lval, type))
                        {
                            il.Emit(OpCodes.Ldarg_1);
                            il.EmitInt(locals.DefineConstant(lval));
                            il.Emit(OpCodes.Ldelem_Ref);
                            il.Emit(OpCodes.Isinst, type);
                        }
                        st.Push(type);
                    }
                }
            }
            else if (Lisp.IsFunctor(lval))
            {
                object head = Lisp.Car(lval);
                object[] args = Lisp.ToArray(Lisp.Cdr(lval));
                ControlFormBase control;
                if (m_control.TryGetValue(head, out control))
                    control.Compile(this, lval, il, locals, st, args);
                else
                {
                    foreach (object a in args)
                        CompileExpr(il, locals, st, a);
                    Type[] parameterTypes = new Type[args.Length];
                    for (int k = args.Length - 1; k >= 0; k--)
                        parameterTypes[k] = st.Pop();
                    FuncName name = new FuncName(head, parameterTypes);
                    FuncBase body = GetFunc(name, false);
                    if (body == null)
                    {
                        bool successed = false;
                        if (parameterTypes.Length == 2)
                        {
                            Type castType = TypeConverter.GetType(parameterTypes[0], parameterTypes[1]);
                            ValueConverter converter1 = FindConverter(parameterTypes[0], castType);
                            ValueConverter converter2 = FindConverter(parameterTypes[1], castType);
                            body = GetFunc(new FuncName(name.ID, new Type[] { castType, castType }), false);
                            if (body != null && converter1 != null && converter2 != null)
                            {
                                LocalBuilder localVar = il.DeclareLocal(parameterTypes[1]);
                                il.Emit(OpCodes.Stloc, localVar);
                                converter1.Compile(this, il, locals, parameterTypes[0]);
                                il.Emit(OpCodes.Ldloc, localVar);
                                il.FreeLocal(localVar);
                                converter2.Compile(this, il, locals, parameterTypes[1]);
                                successed = true;
                            }
                        }
                        if (!successed)
                        {
                            body = GetFunc(name, true);
                            if (body == null)
                                throw new UnknownFuncCall(name.ToString());
                            else
                            {
                                LocalBuilder[] localVar = new LocalBuilder[parameterTypes.Length];
                                for (int k = localVar.Length - 1; k >= 0; k--)
                                {
                                    localVar[k] = il.DeclareLocal(parameterTypes[k]);
                                    il.Emit(OpCodes.Stloc, localVar[k]);
                                }
                                Type[] new_parameter_types = new Type[parameterTypes.Length];
                                for (int k = 0; k < localVar.Length; k++)
                                {
                                    il.Emit(OpCodes.Ldloc, localVar[k]);
                                    if (body.Name.GetParameterType(k) != parameterTypes[k] &&
                                        parameterTypes[k].IsValueType)
                                    {
                                        il.EmitBoxing(parameterTypes[k]);
                                        new_parameter_types[k] = typeof(System.Object);
                                    }
                                    else
                                        new_parameter_types[k] = parameterTypes[k];
                                    il.FreeLocal(localVar[k]);                                    
                                }
                                parameterTypes = new_parameter_types;
                            }
                        }
                    }
                    Type resType = body.Compile(this, il, locals, parameterTypes);
                    if (resType == typeof(System.Boolean))
                    { // Implict boolean convertion
                        Label l_false = il.DefineLabel();
                        Label end = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse_S, l_false);
                        il.Emit(OpCodes.Ldsfld, typeof(RuntimeOps).GetField("True"));
                        il.Emit(OpCodes.Br_S, end);
                        il.MarkLabel(l_false);
                        il.EmitNull();
                        il.MarkLabel(end);
                        st.Push(typeof(System.Object));
                    }
                    else if (resType == typeof(void))
                    { // assume that all procedures returns t
                        il.Emit(OpCodes.Ldsfld, typeof(RuntimeOps).GetField("True"));
                        st.Push(typeof(System.Object));
                    }
                    else
                        st.Push(resType);
                }
            }
            else
                if (Lisp.IsFunctor(Lisp.Car(lval), Lisp.LAMBDA))
                {
                    object form = Lisp.Car(lval);
                    object body = Lisp.Car(Lisp.Cddr(form));
                    object tail = Lisp.Cdr(lval);
                    LambdaExpr lambda = new LambdaExpr(null, CreateParameters(Lisp.Arg1(form)), 
                        typeof(System.Object), body);
                    List<Type> parameterTypesList = new List<Type>();
                    foreach (object a in Lisp.getIterator(tail))
                    {
                        CompileExpr(il, locals, st, a);
                        parameterTypesList.Add(st.Pop());
                    }
                    Type[] parameterTypes = parameterTypesList.ToArray();
                    FuncName name = new FuncName(null, parameterTypes);
                    if (!lambda.Name.Match(name, true))
                        throw new InvalidOperationException("Lambda parameters does not match");
                    st.Push(lambda.Compile(this, il, locals, parameterTypes)); 
                }
                else
                    throw new ArgumentException("Unproperly formated expression");
        }

        public static Parameter[] CreateParameters(string param)
        {
            return CreateParameters(LispParser.Parse(param));
        }

        public static Parameter[] CreateParameters(object param)
        {
            List<Parameter> parameters = new List<Parameter>();
            if (param != Lisp.NIL)
            {
                object[] paramid = Lisp.ToArray(param);
                bool variableParam = false;
                for (int k = 0; k < paramid.Length; k++)
                {
                    if (!Lisp.IsAtom(paramid[k]) || (variableParam && k != paramid.Length - 1))
                        throw new ArgumentException("Bad argument", "param");
                    if (paramid[k].Equals(Funcs.Rest))
                    {
                        variableParam = true;
                        continue;
                    }
                    Parameter p = new Parameter();
                    p.ID = paramid[k];
                    p.Type = typeof(System.Object);
                    p.VariableParam = variableParam;
                    parameters.Add(p);
                }
            }
            return parameters.ToArray();
        }

        public object Eval(object lval)
        {
            return Apply(null, null, lval, null, null);
        }

        public object Apply(object id, Parameter[] parameters, object lval, object[] args, SymbolLink dynamicFunc)
        {
            object res = Undefined.Value;
            if (dynamicFunc != null && dynamicFunc.Value != null)
            {
                CompiledLambda lambda = (CompiledLambda)dynamicFunc.Value;
                res = lambda.Invoke(args);
                return res;
            }
            else
            {
                CompiledLambda lambda = Compile(parameters, lval);
                if (dynamicFunc != null)
                    dynamicFunc.Value = lambda;
                res = lambda.Invoke(args);
            }
            return res;
        }

        public object Owner
        {
            get { return m_owner; }
        }

        public CompiledLambda CurrentLambda { get; internal set; }
    }
}
