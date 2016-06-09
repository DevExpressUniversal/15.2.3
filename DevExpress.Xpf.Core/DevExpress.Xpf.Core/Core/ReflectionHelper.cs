#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace DevExpress.Xpf.Core.Internal {
	public class ReflectionHelper {
		#region inner classes
		struct HelperKey {
			public bool Equals(HelperKey other) {
				var simpleattr = type == other.type
					&& string.Equals(handlerName, other.handlerName)
					&& handlerType == other.handlerType
					&& parametersCount == other.parametersCount
					&& callVirtIfNeeded == other.callVirtIfNeeded
					&& hasTypeParameters == other.hasTypeParameters;
				if (!simpleattr)
					return false;
				if (hasTypeParameters) {
					if (typeParameters.Length != other.typeParameters.Length)
						return false;
					for (int i = 0; i < typeParameters.Length; i++) {
						if (typeParameters[i] != other.typeParameters[i])
							return false;
					}
				}
				return true;
			}
			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				return obj is HelperKey && Equals((HelperKey)obj); 
			}
			public override int GetHashCode() {
				return getHashCode;
			}
			int GetHashCodeInternal() {
				unchecked {
					int hashCode = (type != null ? type.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (handlerName != null ? handlerName.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (handlerType != null ? handlerType.GetHashCode() : 0);
					if (typeParameters != null)
						foreach (var element in typeParameters)
							hashCode = (hashCode * 397) ^ (element != null ? element.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ callVirtIfNeeded.GetHashCode();
					hashCode = (hashCode * 397) ^ parametersCount.GetHashCode();
					return hashCode;
				}
			}
			public HelperKey(Type type, string handlerName, Type handlerType, int? parametersCount, Type[] typeParameters, bool callVirtIfNeeded) {
				this.type = type;
				this.handlerName = handlerName;
				this.handlerType = handlerType;
				this.parametersCount = parametersCount;
				this.typeParameters = typeParameters;
				this.callVirtIfNeeded = callVirtIfNeeded;
				this.hasTypeParameters = typeParameters != null;
				this.getHashCode = 0;
				this.getHashCode = GetHashCodeInternal();
			}
			public static bool operator ==(HelperKey left, HelperKey right) {
				return left.Equals(right);
			}
			public static bool operator !=(HelperKey left, HelperKey right) {
				return !left.Equals(right);
			}
			readonly Type type;
			readonly string handlerName;
			readonly Type handlerType;
			readonly int? parametersCount;
			readonly int getHashCode;
			readonly Type[] typeParameters;
			readonly bool hasTypeParameters;
			readonly bool callVirtIfNeeded;
		}
		#endregion
		Dictionary<HelperKey, object> InvokeInfo { get; set; }
		Dictionary<HelperKey, Type> PropertyTypeInfo { get; set; }
		public bool HasContent { get { return InvokeInfo.Count > 0; } }
		public ReflectionHelper() {
			InvokeInfo = new Dictionary<HelperKey, object>();
			PropertyTypeInfo = new Dictionary<HelperKey, Type>();
		}
		Func<object, object> CreateGetter(PropertyInfo info) {
			return (Func<object, object>)CreateMethodHandlerImpl(info.GetGetMethod(true), null, typeof(Func<object, object>), true);
		}
		Action<object, object> CreateSetter(PropertyInfo info) {
			if (!info.CanWrite)
				throw new NotSupportedException("no setter");
			return (Action<object, object>)CreateMethodHandlerImpl(info.GetSetMethod(true), null, typeof(Action<object, object>), true);
		}
		static object CreateMethodHandlerImpl(object instance, string methodName, BindingFlags bindingFlags, Type instanceType, Type delegateType, int? parametersCount, Type[] typeParameters, bool callVirtIfNeeded) {
			MethodInfo mi = null;
			if (instance != null)
				mi = GetMethod(instance.GetType(), methodName, bindingFlags, parametersCount, typeParameters);
			mi = mi ?? GetMethod(instanceType, methodName, bindingFlags, parametersCount, typeParameters);
			return CreateMethodHandlerImpl(mi, instanceType, delegateType, callVirtIfNeeded);
		}
		static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingFlags, int? parametersCount = null, Type[] typeParameters = null) {
			if (parametersCount != null) {
				return type.GetMethods(bindingFlags).Where(x => x.Name == methodName).First(x => x.GetParameters().Count() == parametersCount.Value);
			}
			if (typeParameters != null) {
				return type.GetMethods(bindingFlags).Where(x => x.Name == methodName).First(x => {
					int i = 0;
					foreach (var param in x.GetParameters()) {
						if (!typeParameters[i].IsAssignableFrom(param.ParameterType))
							return false;
						i++;
					}
					return true;
				});
			}
			return type.GetMethod(methodName, bindingFlags);
		}
		public static Action<object, Delegate, object, Delegate> CreatePushValueMethod(MethodInfo setValueDelegate, MethodInfo getValueDelegate) {
			DynamicMethod dm = new DynamicMethod(String.Empty, null, new Type[] { typeof(object), typeof(Delegate), typeof(object), typeof(Delegate) });
			var ig = dm.GetILGenerator();			
			ig.Emit(OpCodes.Ldarg, (short)1);
			ig.Emit(OpCodes.Castclass, setValueDelegate.DeclaringType);
			ig.Emit(OpCodes.Ldarg, (short)0);
			ig.Emit(OpCodes.Ldarg, (short)3);
			ig.Emit(OpCodes.Castclass, getValueDelegate.DeclaringType);
			ig.Emit(OpCodes.Ldarg, (short)2);
			ig.Emit(OpCodes.Callvirt, getValueDelegate);
			ig.Emit(OpCodes.Callvirt, setValueDelegate);
			ig.Emit(OpCodes.Ret);
			return (Action<object, Delegate, object, Delegate>)dm.CreateDelegate(typeof(Action<object, Delegate, object, Delegate>));
		}
 		static void CastClass(ILGenerator generator, Type sourceType, Type targetType) {
			if (Equals(null, targetType))
				return;
			if (sourceType == targetType)
				return;
			bool oneIsVoid = typeof(void) == sourceType || typeof(void) == targetType;
			bool sourceIsNull = Equals(null, sourceType);
			if (oneIsVoid && !sourceIsNull)
				throw new InvalidOperationException(string.Format("Cast from {0} to {1} is not supported", sourceType, targetType));			
			if (Equals(null, sourceType)) {
				if (targetType.IsClass)
					generator.Emit(OpCodes.Castclass, targetType);
				else
					generator.Emit(OpCodes.Unbox_Any, targetType);
			}
			if (sourceType.IsValueType && !targetType.IsValueType) {
				generator.Emit(OpCodes.Box, sourceType);
				generator.Emit(OpCodes.Castclass, targetType);
			}
			if (!sourceType.IsValueType && targetType.IsValueType)
				generator.Emit(OpCodes.Unbox_Any, targetType);
			if (Equals(sourceType.IsValueType, targetType.IsValueType) && !(sourceType == targetType))
				generator.Emit(OpCodes.Castclass, targetType);
		}			   
		static object CreateMethodHandlerImpl(MethodInfo mi, Type instanceType, Type delegateType, bool callVirtIfNeeded) {
			bool isStatic = mi.IsStatic;						
			var thisArgType = instanceType ?? mi.DeclaringType;
			var returnType = mi.ReturnType;
			Type[] delegateGenericArguments;
			bool skipArgumentLengthCheck = false;
			var sourceParametersTypes = mi.GetParameters().Select(x => x.ParameterType).ToArray();
			if (delegateType == null) {
				delegateType = MakeGenericDelegate(sourceParametersTypes, returnType, isStatic ? null : thisArgType);
				delegateGenericArguments = sourceParametersTypes;
				skipArgumentLengthCheck = true;
			} else {
				var invokeMethod = delegateType.GetMethod("Invoke");
				delegateGenericArguments = invokeMethod.GetParameters().Select(x => x.ParameterType).ToArray();
				if (!isStatic)
					thisArgType = delegateGenericArguments[0];
				returnType = invokeMethod.ReturnType;
			}
			if (!skipArgumentLengthCheck && delegateGenericArguments.Length != (isStatic ? sourceParametersTypes.Count() : sourceParametersTypes.Count() + 1))
				throw new ArgumentException("Invalid delegate arguments count");
			var resultParametersTypes = delegateGenericArguments.Skip(isStatic ? 0 : 1);
			var dynamicMethodParameterTypes = (isStatic ? resultParametersTypes : new Type[] { thisArgType }.Concat(resultParametersTypes)).ToArray();
			DynamicMethod dm;
			if (mi.IsVirtual && !callVirtIfNeeded)
				dm = new DynamicMethod(string.Empty, returnType, dynamicMethodParameterTypes, mi.DeclaringType, true);
			else
				dm = new DynamicMethod(string.Empty, returnType, dynamicMethodParameterTypes, true);
			var ig = dm.GetILGenerator();
			if (!isStatic) {
				var isValueType = mi.DeclaringType.IsValueType;
				if (isValueType) {
					ig.DeclareLocal(mi.DeclaringType);
				}
				ig.Emit(OpCodes.Ldarg_0);				
				CastClass(ig, thisArgType, mi.DeclaringType);
				if (isValueType) {
					ig.Emit(OpCodes.Stloc_0);
					ig.Emit(OpCodes.Ldloca_S, 0);
				}				
			}
			short argumentIndex = mi.IsStatic ? (short)0 : (short)1;
			for (int parameterIndex = 0; parameterIndex < sourceParametersTypes.Length; parameterIndex++) {
				ig.Emit(OpCodes.Ldarg, argumentIndex++);
				CastClass(ig, resultParametersTypes.ElementAt(parameterIndex), sourceParametersTypes[parameterIndex]);
			}
			if (mi.IsVirtual && callVirtIfNeeded)
				ig.Emit(OpCodes.Callvirt, mi);
			else
				ig.Emit(OpCodes.Call, mi);		
			CastClass(ig, mi.ReturnType, returnType);
			ig.Emit(OpCodes.Ret);
			return dm.CreateDelegate(delegateType);
		}
		static Type MakeGenericDelegate(Type[] parameterTypes, Type returnType, Type thisArgType) {
			Type resultType = null;
			bool hasReturnType = returnType != null && returnType != typeof(void);
			var parametersCount = parameterTypes.Length;
			if (thisArgType != null)
				parametersCount += 1;
			switch (parametersCount) {
				case 0:
					resultType = hasReturnType ? typeof(Func<>) : typeof(Action);
					break;
				case 1:
					resultType = hasReturnType ? typeof(Func<,>) : typeof(Action<>);
					break;
				case 2:
					resultType = hasReturnType ? typeof(Func<,,>) : typeof(Action<,>);
					break;
				default:
					resultType = hasReturnType ? typeof(Func<>).Assembly.GetType(string.Format("System.Func`{0}", parametersCount + 1)) : typeof(Func<>).Assembly.GetType(string.Format("System.Action`{0}", parametersCount));
					break;
			}
			var lst = new List<Type>();
			if (thisArgType != null)
				lst.Add(thisArgType);
			lst.AddRange(parameterTypes);
			if (hasReturnType)
				lst.Add(returnType);
			if (lst.Count == 0)
				return resultType;
			return resultType.MakeGenericType(lst.ToArray());
		}
		static Delegate CreateFieldGetterOrSetter<TElement, TField>(bool isGetter, Type delegateType, Type declaringType, string fieldName, BindingFlags bFlags) {
			FieldInfo fieldInfo = declaringType.GetField(fieldName, bFlags);
			bool isStatic = fieldInfo.IsStatic;
			DynamicMethod dm;
			if(isGetter)
				dm = new DynamicMethod(string.Empty, typeof(TField), new Type[] { typeof(TElement) }, true);
			else
				dm = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(TElement), typeof(TField) }, true);
			var ig = dm.GetILGenerator();
			short argIndex = 0;
			if (!isStatic) {
				ig.Emit(OpCodes.Ldarg, argIndex++);
				CastClass(ig, typeof(TElement), fieldInfo.DeclaringType);
			}
			if (!isGetter) {
				ig.Emit(OpCodes.Ldarg, argIndex++);
				CastClass(ig, typeof(TField), fieldInfo.FieldType);
				ig.Emit(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, fieldInfo);
			} else {
				ig.Emit(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fieldInfo);
				CastClass(ig, fieldInfo.FieldType, typeof(TField));
			}			
			ig.Emit(OpCodes.Ret);
			return dm.CreateDelegate(delegateType);
		}
		public static Func<TElement, TField> CreateFieldGetter<TElement,TField>(Type declaringType, string fieldName, BindingFlags bFlags = BindingFlags.Instance | BindingFlags.Public) {
			return (Func<TElement, TField>)CreateFieldGetterOrSetter<TElement, TField>(true, typeof(Func<TElement, TField>), declaringType, fieldName, bFlags);
		}
		public static Action<TElement, TField> CreateFieldSetter<TElement, TField>(Type declaringType, string fieldName, BindingFlags bFlags = BindingFlags.Instance | BindingFlags.Public) {
			return (Action<TElement, TField>)CreateFieldGetterOrSetter<TElement, TField>(false, typeof(Action<TElement, TField>), declaringType, fieldName, bFlags);
		}
		public static Delegate CreateInstanceMethodHandler(object instance, string methodName, BindingFlags bindingFlags, Type instanceType, int? parametersCount = null, Type[] typeParameters = null, bool callVirtIfNeeded = true) {
			return (Delegate)CreateMethodHandlerImpl(instance, methodName, bindingFlags, instanceType, null, parametersCount, typeParameters, callVirtIfNeeded);
		}		
		public static TDelegate CreateInstanceMethodHandler<TInstance, TDelegate>(TInstance entity, string methodName, BindingFlags bindingFlags, int? parametersCount = null, Type[] typeParameters = null, bool callVirtIfNeeded = true) {
			return (TDelegate)CreateMethodHandlerImpl(entity, methodName, bindingFlags, typeof(TInstance), typeof(TDelegate), parametersCount, typeParameters, callVirtIfNeeded);
		}
		public static TDelegate CreateInstanceMethodHandler<TDelegate>(object instance, string methodName, BindingFlags bindingFlags, Type instanceType, int? parametersCount = null, Type[] typeParameters = null, bool callVirtIfNeeded = true) {
			return (TDelegate)CreateMethodHandlerImpl(instance, methodName, bindingFlags, instanceType, typeof(TDelegate), parametersCount, null, callVirtIfNeeded);
		}
		public T GetStaticMethodHandler<T>(Type entityType, string methodName, BindingFlags bindingFlags) where T : class {
			object method;
			var key = new HelperKey(entityType, methodName, typeof(T), null, null, true);
			if (!InvokeInfo.TryGetValue(key, out method)) {
				method = CreateMethodHandlerImpl(null, methodName, bindingFlags, entityType, typeof(T), null, null, true);
				InvokeInfo[key] = method;
			}
			return (T)method;
		}
		public T GetInstanceMethodHandler<T>(object entity, string methodName, BindingFlags bindingFlags, Type instanceType, int? parametersCount = null, Type[] typeParameters = null, bool callVirtIfNeeded = true) {
			object method;
			var key = new HelperKey(instanceType, methodName, typeof(T), parametersCount, typeParameters, callVirtIfNeeded);
			if (!InvokeInfo.TryGetValue(key, out method)) {
				method = CreateInstanceMethodHandler<T>(entity, methodName, bindingFlags, instanceType, parametersCount, typeParameters, callVirtIfNeeded);
				InvokeInfo[key] = method;
			}
			return (T)method;
		}
		public T GetPropertyValue<T>(object entity, string propertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			return (T)GetPropertyValue(entity, propertyName, bindingFlags);
		}
		public object GetPropertyValue(object entity, string propertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			object getter;
			var type = entity.GetType();
			var key = new HelperKey(type, propertyName, typeof(Func<object, object>), null, null, true);
			if (!InvokeInfo.TryGetValue(key, out getter)) {
				var pi = type.GetProperty(propertyName, bindingFlags);
				getter = CreateGetter(pi);
				InvokeInfo[key] = getter;
			}
			var func = (Func<object, object>)getter;
			return func(entity);
		}
		public void SetPropertyValue(object entity, string propertyName, object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			object setter;
			var type = entity.GetType();
			var key = new HelperKey(type, propertyName, typeof(Action<object, object>), null, null, true);
			if (!InvokeInfo.TryGetValue(key, out setter)) {
				var pi = type.GetProperty(propertyName, bindingFlags);
				setter = CreateSetter(pi);
				InvokeInfo[key] = setter;
			}
			var del = (Action<object, object>)setter;
			del(entity, value);
		}
		public Type GetPropertyType(object entity, string propertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
			Type propertyType;
			Type type = entity.GetType();
			var key = new HelperKey(type, propertyName, null, null, null, true);
			if (!PropertyTypeInfo.TryGetValue(key, out propertyType)) {
				var pi = type.GetProperty(propertyName, bindingFlags);
				propertyType = pi.PropertyType;
			}
			return propertyType;
		}
	}
}
