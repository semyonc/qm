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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DataEngine.CoreServices.Generation
{
    // Extensions on System.Type and friends
    internal static class TypeExtensions 
    {
        /// <summary>
        /// Creates an open delegate for the given (dynamic)method.
        /// </summary>
        internal static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType) {
            Debug.Assert(methodInfo != null && delegateType != null);

            var dm = methodInfo as DynamicMethod;
            if (dm != null) {
                return dm.CreateDelegate(delegateType);
            } else {
                return Delegate.CreateDelegate(delegateType, methodInfo);
            }
        }

        /// <summary>
        /// Creates a closed delegate for the given (dynamic)method.
        /// </summary>
        internal static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target) {
            Debug.Assert(methodInfo != null && delegateType != null);

            var dm = methodInfo as DynamicMethod;
            if (dm != null) {
                return dm.CreateDelegate(delegateType, target);
            } else {
                return Delegate.CreateDelegate(delegateType, target, methodInfo);
            }
        }

        internal static T CreateDelegate<T>(this MethodInfo methodInfo) {
            return (T)(object)methodInfo.CreateDelegate(typeof(T));
        }
        internal static T CreateDelegate<T>(this MethodInfo methodInfo, object target) {
            return (T)(object)methodInfo.CreateDelegate(typeof(T), target);
        }


        internal static Type GetReturnType(this MethodBase mi) {
            return (mi.IsConstructor) ? mi.DeclaringType : ((MethodInfo)mi).ReturnType;
        }
               

        /// <summary>
        /// Returns <c>true</c> if the specified parameter is mandatory, i.e. is not optional and doesn't have a default value.
        /// </summary>
        internal static bool IsMandatoryParameter(this ParameterInfo pi) {
            return (pi.Attributes & (ParameterAttributes.Optional | ParameterAttributes.HasDefault)) == 0;
        }

        internal static bool HasDefaultValue(this ParameterInfo pi) {
            return (pi.Attributes & ParameterAttributes.HasDefault) != 0;
        }

        internal static bool IsByRefParameter(this ParameterInfo pi) {
            // not using IsIn/IsOut properties as they are not available in Silverlight:
            if (pi.ParameterType.IsByRef) return true;

            return (pi.Attributes & (ParameterAttributes.Out)) == ParameterAttributes.Out;
        }

        internal static string FormatTypeName(this Type type) {
            Debug.Assert(type != null);
            var str = new StringBuilder();
            FormatTypeName(str, type);
            return str.ToString();
        }

        internal static string FormatSignature(this MethodBase method) {
            var str = new StringBuilder();
            FormatSignature(str, method);
            return str.ToString();
        }

        private static void FormatSignature(StringBuilder result, MethodBase method) {
            MethodInfo methodInfo = method as MethodInfo;
            if (methodInfo != null) {
                FormatTypeName(result, methodInfo.ReturnType);
                result.Append(' ');
            }

            MethodBuilder builder = method as MethodBuilder;
            if (builder != null) {
                result.Append(builder.Signature);
                return;
            }

            ConstructorBuilder cb = method as ConstructorBuilder;
            if (cb != null) {
                result.Append(cb.Signature);
                return;
            }

            FormatTypeName(result, method.DeclaringType);
            result.Append("::");
            result.Append(method.Name);

            if (!method.IsConstructor) {
                FormatTypeArgs(result, method.GetGenericArguments());
            }

            result.Append("(");

            if (!method.ContainsGenericParameters) {
                ParameterInfo[] ps = method.GetParameters();
                for (int i = 0; i < ps.Length; i++) {
                    if (i > 0) result.Append(", ");
                    FormatTypeName(result, ps[i].ParameterType);
                    if (!System.String.IsNullOrEmpty(ps[i].Name)) {
                        result.Append(" ");
                        result.Append(ps[i].Name);
                    }
                }
            } else {
                result.Append("?");
            }

            result.Append(")");
        }

        private static void FormatTypeName(StringBuilder result, Type type) {
            if (type.IsGenericType) {
                string genericName = type.GetGenericTypeDefinition().FullName.Replace('+', '.');
                int tickIndex = genericName.IndexOf('`');
                result.Append(tickIndex != -1 ? genericName.Substring(0, tickIndex) : genericName);
                FormatTypeArgs(result, type.GetGenericArguments());
            } else if (type.IsGenericParameter) {
                result.Append(type.Name);
            } else {
                result.Append(type.FullName.Replace('+', '.'));
            }
        }

        private static void FormatTypeArgs(StringBuilder result, Type[] types) {
            if (types.Length > 0) {
                result.Append("<");

                for (int i = 0; i < types.Length; i++) {
                    if (i > 0) result.Append(", ");
                    FormatTypeName(result, types[i]);
                }

                result.Append(">");
            }
        }
    }
}
