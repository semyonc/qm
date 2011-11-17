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
using System.Security;

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

        internal class ConverterKey
        {
            private readonly Type _src;
            private readonly Type _dest;

            public ConverterKey(Type src, Type dest)
            {
                _src = src;
                _dest = dest;
            }

            public override bool Equals(object obj)
            {
                ConverterKey other = obj as ConverterKey;
                if (other == null)
                    return false;
                return other._src == _src && other._dest == _dest;
            }

            public override int GetHashCode()
            {
                return _src.GetHashCode() ^ _dest.GetHashCode() << 6;
            }
        }

        private Dictionary<object, SymbolLink> m_value;
        private Dictionary<object, ControlFormBase> m_control;
        private Dictionary<object, FuncDef> m_func;
        private Dictionary<ConverterKey, ValueConverter> m_converter;
        private Dictionary<object, MacroFuncDef> m_macro;
        private Stack<Resolver> m_resolvers = new Stack<Resolver>();
        private int m_resolvers_lock = 0;
        private MethodInfo m_memoryPoolGetData;
        private MemoryPool m_defaultPool;

        public Executive(object owner)
        {
            m_owner = owner;
            
            m_control = GlobalSymbols.Shared.CreateControls();
            m_func = GlobalSymbols.Shared.CreateFuncs();
            m_converter = GlobalSymbols.Shared.CreateConverters();
            m_macro = GlobalSymbols.Shared.CreateMacros();
            
            m_value = new Dictionary<object, SymbolLink>();
            m_memoryPoolGetData = typeof(MemoryPool).GetMethod("GetData", BindingFlags.Instance | BindingFlags.Public);
        }

        public virtual void Prepare()
        {
            m_defaultPool = new MemoryPool();
            m_value.Clear();
            Set(Lisp.T, Generation.RuntimeOps.True);
            Set(Lisp.NIL, null);
            Set(Lisp.UNKNOWN, Undefined.Value);
        }

        public SymbolLink Set(string Name, object value)
        {
            return Set(ATOM.Create(null, new string[] {Name}, false), value);            
        }

        public SymbolLink Set(object atom, object value)
        {
            SymbolLink link;
            if (!m_value.TryGetValue(atom, out link))
            {
                if (!Lisp.IsAtom(atom))
                    throw new ArgumentException();
                link = new SymbolLink();
                m_defaultPool.Bind(link);
                m_value.Add(atom, link);
            }
            m_defaultPool.SetData(link, value);
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
            resolver.Init(m_defaultPool);
            m_resolvers.Push(resolver);
        }

        public void Leave()
        {
            m_resolvers.Pop();
        }

        public Resolver Resolver
        {
            get
            {
                if (m_resolvers.Count > 0)
                    return m_resolvers.Peek();
                return null;
            }
        }

        internal void EnterMacro()
        {
            m_resolvers_lock++;
        }

        public void LeaveMacro()
        {
            m_resolvers_lock--;
        }

        public Resolver CurrentResolver()
        {
            return m_resolvers.Peek();
        }

        public SymbolLink TryGet(object atom, bool bindings, bool resolvers)
        {
            SymbolLink link;
            if (resolvers && m_resolvers_lock == 0 && m_resolvers.Count > 0)
            {
                if (m_resolvers.Peek().Get(atom, out link))
                    return link;
            }
            if (bindings && m_value.TryGetValue(atom, out link))
                return link;
            return null;
        }

        public SymbolLink Get(object atom)
        {
            SymbolLink link = TryGet(atom, true, true);
            if (link == null)
                throw new ArgumentException("Value not defined", atom.ToString());
            return link;
        }

        public object GetValue(object atom)
        {
            return m_defaultPool.GetData(Get(atom));
        }

        public void DefineStaticOperator(object id, Type type, string name)
        {
            bool bfound = false;
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
                if (method.Name == name && !method.IsGenericMethod)
                {
                    Defun(new InvokeStatic(id, method));
                    bfound = true;
                }
            if (!bfound)
                throw new ArgumentException("Static method not found in class", name);
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
                ValueConverter res;
                if (m_converter.TryGetValue(new ConverterKey(source, dest), out res))
                    return res;
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
        
        public Type Compile(Parameter[] parameters, object expr, FunctionLink dynamicFunc)
        {
            try
            {
                CompiledLambda lambda = Compile(parameters, expr);
                if (dynamicFunc != null)
                    dynamicFunc.Value = lambda;
                return lambda.ReturnType;
            }
            catch (Exception ex)
            {
                HandleRuntimeException(ex);
                throw;
            }
        }

        public bool IsParameterSensitive(FunctionLink dynamicFunc)
        {
            if (dynamicFunc.Value == null)
                throw new ArgumentNullException();
            return dynamicFunc.Value.ParametersBinded;
        }

        internal Type Compile(LambdaExpr expr)
        {
            Compile(expr._parameters, expr._body, expr._compiledBody);
            return expr._retType;
        }

        private void GetValueDependences(HashSet<Object> hs, Parameter[] parameters, 
            CompiledLambda compiledExpr, List<SymbolLink> res, bool reviewLambdaExpr)
        {
            foreach (SymbolLink value in compiledExpr.Values)
                res.Add(value);
            if (compiledExpr.Consts != null)
            {
                foreach (object cn in compiledExpr.Consts)
                {
                    IBindableObject bo = cn as IBindableObject;
                    if (bo != null)
                        bo.GetValueDependences(hs, parameters, reviewLambdaExpr, 
                            (SymbolLink value) => res.Add(value));
                        //foreach (FunctionLink dynFunc in bo.EnumDynamicFuncs())
                        //    GetValueDependences(hs, dynFunc.Value, res, reviewLambdaExpr);
                }
            }
            foreach (LambdaExpr expr in compiledExpr.Dependences)
                if (reviewLambdaExpr || !expr.Isolate)
                {
                    if (hs != null)
                    {
                        if (hs.Contains(expr))
                            continue;
                        hs.Add(expr);
                    }
                    if (expr._compiledBody.Value == null)
                        Compile(expr);
                    GetValueDependences(hs, expr._parameters, expr._compiledBody.Value, res, reviewLambdaExpr);
                }
        }

        public SymbolLink[] GetValueDependences(HashSet<Object> hs, Parameter[] parameters, object expr, 
            FunctionLink dynamicFunc, bool reviewLambdaExpr)
        {
            if (dynamicFunc.Value == null)
                Compile(parameters, expr, dynamicFunc);
            List<SymbolLink> res = new List<SymbolLink>();
            GetValueDependences(hs, parameters, dynamicFunc.Value, res, reviewLambdaExpr);
            return res.ToArray();
        }

        public IBindableObject[] GetBindableObjects(Parameter[] parameters, object expr, FunctionLink dynamicFunc)
        {
            List<IBindableObject> res = new List<IBindableObject>();
            if (dynamicFunc.Value == null)
                Compile(parameters, expr, dynamicFunc);
            foreach (object cn in dynamicFunc.Value.Consts)
            {
                IBindableObject bo = cn as IBindableObject;
                if (bo != null)
                    res.Add(bo);
            }
            return res.ToArray();
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
                LocalAccess localAccess = new LocalAccess(il, parameters);
                foreach (Parameter p in parameters)
                    localAccess.BindParameter(p.ID, p.Type);
                CreateVariables(il, localAccess, expr);
                WriteEpilog(il, parameters, localAccess);
                CompileExpr(il, localAccess, st, expr);
                lambda.Values = localAccess.GetValues();
                lambda.Consts = localAccess.GetConsts();
                lambda.Dependences = localAccess.GetDependences();
                lambda.ParametersBinded = localAccess.ParametersBinded;
                Type retType = st.Pop();
                if (ValueProxy.IsProxyType(retType))
                {
                    il.EmitPropertyGet(typeof(ValueProxy), "Value");
                    retType = typeof(System.Object);
                }                
                else 
                    if (retType.IsValueType)
                        il.EmitBoxing(retType);
                il.Emit(OpCodes.Ret);
                lambda.ReturnType = retType;
                if (lambda.Consts != null)
                {
                    foreach (object obj in lambda.Consts)
                    {
                        // Notify object about compile
                        IBindableObject cs = obj as IBindableObject;
                        if (cs != null)
                            cs.Bind(parameters, m_defaultPool);
                    }
                }
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
                if (Lisp.IsAtom(lval) && lval != Lisp.NIL && lval != Lisp.T && lval != Lisp.INST && lval != Lisp.ARGV && lval != Lisp.MPOOL)
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

        private void WriteEpilog(ILGen il, Parameter[] parameters, LocalAccess locals)
        {
            if (locals.HasLocals || parameters.Length > 0)
            {
                Label label = il.DefineLabel();
                if (parameters.Length > 0)
                {
                    for (int k = 0; k < parameters.Length; k++)
                    {
                        il.Emit(OpCodes.Ldarg_2);
                        il.EmitInt(k);
                        il.Emit(OpCodes.Ldelem_Ref);
                        if (parameters[k].Type.IsValueType)
                        {
                            il.Emit(OpCodes.Isinst, parameters[k].Type);
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brfalse, label);
                            if (locals.IsParameterBinded(parameters[k].ID))
                            {
                                LocalBuilder local = locals.GetLocal(parameters[k].ID);
                                il.EmitUnbox(local.LocalType);
                                il.Emit(OpCodes.Stloc, local);
                            }
                            else
                                il.Emit(OpCodes.Pop);
                        }
                        else
                        {
                            Label label2 = il.DefineLabel();
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brfalse_S, label2);
                            il.Emit(OpCodes.Isinst, parameters[k].Type);
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Brfalse, label);
                            il.MarkLabel(label2);
                            if (locals.IsParameterBinded(parameters[k].ID))
                            {                                                                
                                LocalBuilder local = locals.GetLocal(parameters[k].ID);
                                il.Emit(OpCodes.Stloc, local);
                            }
                            else
                                il.Emit(OpCodes.Pop);
                        }
                    }
                }
                if (locals.HasLocals)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.EmitPropertyGet(typeof(CompiledLambda), "Values");
                    LocalBuilder values = il.DeclareLocal(typeof(SymbolLink[]));
                    il.Emit(OpCodes.Stloc, values);
                    int k = 0;
                    foreach (LocalAccess.LocalBinding b in locals)
                    {
                        il.Emit(OpCodes.Ldarg_3);                        
                        il.Emit(OpCodes.Ldloc, values);
                        il.EmitInt(k++);
                        il.Emit(OpCodes.Ldelem_Ref);
                        il.EmitCall(m_memoryPoolGetData);
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
                    il.FreeLocal(values);
                }
                Label end = il.DefineLabel();
                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(label);
                il.Emit(OpCodes.Pop); // Isinst
                il.EmitPropertyGet(typeof(Undefined), "Value");
                il.Emit(OpCodes.Ret);
                il.MarkLabel(end);                
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
                else if (lval == Lisp.ARGV)
                {
                    il.Emit(OpCodes.Ldarg_2);
                    st.Push(typeof(object[]));
                }
                else if (lval == Lisp.MPOOL)
                {
                    il.Emit(OpCodes.Ldarg_3);
                    st.Push(typeof(MemoryPool));
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
                        if (type == typeof(Integer))
                        {
                            il.EmitDecimal((Decimal)((Integer)lval));
                            il.EmitNew(typeof(Integer).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                                new Type[] { typeof(Decimal) }, null));
                        }
                        else if (!il.TryEmitConstant(lval, type))
                        {
                            il.Emit(OpCodes.Ldarg_1);
                            il.EmitInt(locals.DefineConstant(lval));
                            il.Emit(OpCodes.Ldelem_Ref);
                            if (type.IsValueType)
                                il.EmitUnbox(type);
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
                            Type castType = ValueProxy.GetType(parameterTypes[0], parameterTypes[1]);
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
                                    if (body.Name.GetParameterType(k) != parameterTypes[k])                                         
                                    {
                                        if (parameterTypes[k].IsValueType)
                                            il.EmitBoxing(parameterTypes[k]);
                                        else if (ValueProxy.IsProxyType(parameterTypes[k]))
                                            il.EmitPropertyGet(typeof(ValueProxy), "Value");
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
        
        [SecuritySafeCritical]
        public object Apply(object id, Parameter[] parameters, object lval, object[] args, FunctionLink dynamicFunc, MemoryPool pool)
        {
            object res = Undefined.Value;            
            if (dynamicFunc != null && dynamicFunc.Value != null)
            {
                CompiledLambda lambda = dynamicFunc.Value;
                res = lambda.Invoke(pool, args);
                return res;
            }
            else
            {
                CompiledLambda lambda = Compile(parameters, lval);
                if (dynamicFunc != null)
                    dynamicFunc.Value = lambda;
                res = lambda.Invoke(pool, args);
            }            
            return res;
        }

        public virtual void HandleRuntimeException(Exception exception)
        {
            throw exception;
        }

        public virtual object OperatorEq(object arg1, object arg2)
        {
            if (arg1 == arg2)
                return true;
            else 
            {
                if (arg1 == null)
                    arg1 = Generation.RuntimeOps.False;
                if (arg2 == null)
                    arg2 = Generation.RuntimeOps.False;
                if (ValueProxy.New(arg1) == ValueProxy.New(arg2))
                    return true;
            }
            return null;
        }

        public virtual object OperatorGt(object arg1, object arg2)
        {
            if (arg1 == null)
                arg1 = Generation.RuntimeOps.False;
            if (arg2 == null)
                arg2 = Generation.RuntimeOps.False;
            if (ValueProxy.New(arg1) > ValueProxy.New(arg2))
                return true;
            return null;
        }

        public object Owner
        {
            get { return m_owner; }
        }

        public MemoryPool DefaultPool
        {
            get { return m_defaultPool; }
        }
    }
}
