/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/
using System; 
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace DataEngine.CoreServices.Generation
{
#if DEBUG
    public class DebugILGen : ILGen {
        private readonly TextWriter _txt;
        
        public DebugILGen(ILGenerator ilg, TextWriter txt)
            : base(ilg) {
            _txt = txt;
        }

        #region ILGen overrides

        /// <summary>
        /// Begins a catch block.
        /// </summary>
        public override void BeginCatchBlock(Type exceptionType) {
            if (exceptionType == null) {
                Write("} catch {");
            } else {
                Write("}} catch ({0}) {{", exceptionType.FullName);
            }
            base.BeginCatchBlock(exceptionType);
        }

        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        public override void BeginExceptFilterBlock()
        {
            Write("} filter {");
            base.BeginExceptFilterBlock();
        }

        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <returns></returns>
        public override Label BeginExceptionBlock()
        {
            Write(" try {");
            return base.BeginExceptionBlock();
        }

        /// <summary>
        /// Begins an exception fault block
        /// </summary>
        public override void BeginFaultBlock()
        {
            Write("} fault {");
            base.BeginFaultBlock();
        }

        /// <summary>
        /// Begins a finally block
        /// </summary>
        public override void BeginFinallyBlock()
        {
            Write("} finally {");
            base.BeginFinallyBlock();
        }

        /// <summary>
        /// Ends an exception block.
        /// </summary>
        public override void EndExceptionBlock()
        {
            Write("}");
            base.EndExceptionBlock();
        }

        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        public override void BeginScope()
        {
            Write("{");
            base.BeginScope();
        }

        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        public override void EndScope()
        {
            Write("}");
            base.EndScope();
        }

        /// <summary>
        /// Declares a local variable of the specified type.
        /// </summary>
        public override LocalBuilder DeclareLocal(Type localType)
        {
            LocalBuilder lb = base.DeclareLocal(localType);
            Write(".local {0}: {1}", lb.LocalIndex, lb.LocalType.FullName);
            return lb;
        }

        /// <summary>
        /// Declares a local variable of the specified type, optionally
        /// pinning the object referred to by the variable.
        /// </summary>
        public override LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            LocalBuilder lb = base.DeclareLocal(localType, pinned);
            Write(".local {0}: {1}{2}", lb.LocalIndex, lb.LocalType.FullName, pinned ? " (pinned)" : "");
            return lb;
        }

        /// <summary>
        /// Declares a new label.
        /// </summary>
        public override Label DefineLabel()
        {
            return base.DefineLabel();
        }

        /// <summary>
        /// Marks the label at the current position.
        /// </summary>
        public override void MarkLabel(Label loc)
        {
            Write(".label_{0}:", GetLabelId(loc));
            base.MarkLabel(loc);
        }

        /// <summary>
        /// Emits an instruction.
        /// </summary>
        public override void Emit(OpCode opcode)
        {
            Write(opcode.ToString());
            base.Emit(opcode);
        }

        /// <summary>
        /// Emits an instruction with a byte argument.
        /// </summary>
        public override void Emit(OpCode opcode, byte arg)
        {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

        /// <summary>
        /// Emits an instruction with the metadata token for the specified contructor.
        /// </summary>
        public override void Emit(OpCode opcode, ConstructorInfo con)
        {
            Write("{0}\t{1}", opcode, con.FormatSignature());
            base.Emit(opcode, con);
        }

        /// <summary>
        /// Emits an instruction with a double argument.
        /// </summary>
        public override void Emit(OpCode opcode, double arg)
        {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

        /// <summary>
        /// Emits an instruction with the metadata token for the specified field.
        /// </summary>
        public override void Emit(OpCode opcode, FieldInfo field) {
            Write("{0}\t{1}.{2}", opcode, field.DeclaringType.FormatTypeName(), field.Name);
            base.Emit(opcode, field);
        }

        /// <summary>
        /// Emits an instruction with a float argument.
        /// </summary>
        public override void Emit(OpCode opcode, float arg) {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

        /// <summary>
        /// Emits an instruction with an int argument.
        /// </summary>
        public override void Emit(OpCode opcode, int arg) {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

        /// <summary>
        /// Emits an instruction with a label argument.
        /// </summary>
        public override void Emit(OpCode opcode, Label label) {
            Write("{0}\t.label_{1}", opcode, GetLabelId(label));
            base.Emit(opcode, label);
        }

        /// <summary>
        /// Emits an instruction with multiple target labels (switch).
        /// </summary>
        public override void Emit(OpCode opcode, Label[] labels) {
            StringBuilder sb = new StringBuilder();
            sb.Append(opcode.ToString());
            sb.Append("\t[");
            for (int i = 0; i < labels.Length; i++) {
                if (i != 0) {
                    sb.Append(", ");
                }
                sb.Append("label_" + GetLabelId(labels[i]).ToString(CultureInfo.CurrentCulture));
            }
            sb.Append("]");

            Write(sb.ToString());

            base.Emit(opcode, labels);
        }

        /// <summary>
        /// Emits an instruction with a reference to a local variable.
        /// </summary>
        public override void Emit(OpCode opcode, LocalBuilder local) {
            Write("{0}\t{1}", opcode, local);
            base.Emit(opcode, local);
        }

        /// <summary>
        /// Emits an instruction with a long argument.
        /// </summary>
        public override void Emit(OpCode opcode, long arg) {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

        /// <summary>
        /// Emits an instruction with the metadata token for a specified method.
        /// </summary>
        public override void Emit(OpCode opcode, MethodInfo meth) {
            Write("{0}\t{1}", opcode, meth.FormatSignature());
            base.Emit(opcode, meth);
        }

        /// <summary>
        /// Emits an instruction with a signed byte argument.
        /// </summary>
        public override void Emit(OpCode opcode, sbyte arg) {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

        /// <summary>
        /// Emits an instruction with a short argument.
        /// </summary>
        public override void Emit(OpCode opcode, short arg) {
            Write("{0}\t{1}", opcode, arg);
            base.Emit(opcode, arg);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Emits an instruction with a signature token.
        /// </summary>
        public override void Emit(OpCode opcode, SignatureHelper signature) {
            Write("{0}\t{1}", opcode, signature);
            base.Emit(opcode, signature);
        }
#endif

        /// <summary>
        /// Emits an instruction with a string argument.
        /// </summary>
        public override void Emit(OpCode opcode, string str) {
            Write("{0}\t\"{1}\"", opcode, str);
            base.Emit(opcode, str);
        }

        /// <summary>
        /// Emits an instruction with the metadata token for a specified type argument.
        /// </summary>
        public override void Emit(OpCode opcode, Type cls) {
            Write("{0}\t{1}", opcode, cls.FormatTypeName());
            base.Emit(opcode, cls);
        }

        /// <summary>
        /// Emits a call or a virtual call to the varargs method.
        /// </summary>
        public override void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) {
            Write("{0}\t{1}", opcode, methodInfo.FormatSignature());
            base.EmitCall(opcode, methodInfo, optionalParameterTypes);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Emits an unmanaged indirect call instruction.
        /// </summary>
        public override void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes) {
            StringBuilder sb = new StringBuilder();
            sb.Append(opcode.ToString());
            sb.Append('\t');
            sb.Append(unmanagedCallConv.ToString());
            sb.Append(' ');
            sb.Append(returnType.FormatTypeName());
            sb.Append('(');
            AppendTypeNames(sb, parameterTypes);
            sb.Append(')');

            Write(sb.ToString());

            base.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
        }

        /// <summary>
        /// Emits a managed indirect call instruction.
        /// </summary>
        public override void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) {
            StringBuilder sb = new StringBuilder();
            sb.Append(opcode.ToString());
            sb.Append('\t');
            sb.Append(callingConvention.ToString());
            sb.Append(' ');
            sb.Append(returnType.FormatTypeName());
            sb.Append('(');
            AppendTypeNames(sb, parameterTypes);
            sb.Append(") [(");
            AppendTypeNames(sb, optionalParameterTypes);
            sb.Append(")]");

            Write(sb.ToString());

            base.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
        }
#endif

        /// <summary>
        /// Marks a sequence point.
        /// </summary>
        public override void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn) {
            WriteImpl(String.Format(CultureInfo.CurrentCulture, ".seq {0}:{1}-{2}:{3}", startLine, startColumn, endLine, endColumn));
            base.MarkSequencePoint(document, startLine, startColumn, endLine, endColumn);
        }

        /// <summary>
        /// Specifies the namespace to be used in evaluating locals and watches for the
        ///     current active lexical scope.
        /// </summary>
        public override void UsingNamespace(string usingNamespace) {
            base.UsingNamespace(usingNamespace);
        }

        #endregion

        public void WriteLine(string str) {
            WriteImpl(str);
        }

        #region IL Output Support

        private void Write(string str) {
            WriteImpl(str);
        }

        private void WriteImpl(string str) {
            if (_txt == null) {
                return;
            }

#if !SILVERLIGHT
            if (_txt == Console.Out) {
                ConsoleColor color = Console.ForegroundColor;
                try {
                    if (Console.BackgroundColor == ConsoleColor.White) {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                    } else {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    _txt.WriteLine(str);
                    _txt.Flush();
                } finally {
                    Console.ForegroundColor = color;
                }
                return;
            }
#endif

            _txt.WriteLine(str);
            _txt.Flush();
        }


        private void Write(string format, object arg0) {
            Write(String.Format(CultureInfo.CurrentCulture, format, arg0));
        }

        private void Write(string format, object arg0, object arg1) {
            Write(String.Format(CultureInfo.CurrentCulture, format, arg0, arg1));
        }

        private void Write(string format, object arg0, object arg1, object arg2) {
            Write(String.Format(CultureInfo.CurrentCulture, format, arg0, arg1, arg2));
        }

        private static int GetLabelId(Label label) {
            return label.GetHashCode();
        }

        private static void AppendTypeNames(StringBuilder sb, Type[] types) {
            Debug.Assert(sb != null);

            if (types != null) {
                for (int i = 0; i < types.Length; i++) {
                    if (i > 0) {
                        sb.Append(", ");
                    }
                    sb.Append(types[i].FormatTypeName());
                }
            }
        }

        #endregion
    }
#endif
}
