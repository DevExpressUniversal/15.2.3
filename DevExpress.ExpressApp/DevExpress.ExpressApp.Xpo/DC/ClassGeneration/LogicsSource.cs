#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Reflection;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	sealed class LogicsSource {
		const string ErrorDeclaringTypeIsNotAssignableFromInterfaceType = "The '{0}' is not assignable from '{1}'.";
		const string ErrorMultipleLogicsForMethod = "There are multiple logics for method '{0}' in '{1}'.";
		private readonly InterfaceInheritanceMap interfaceInheritanceMap;
		private readonly Dictionary<Type, List<Type>> autoLogics;
		private readonly Dictionary<Type, Type[]> logicsCache;
		private bool isInitialized;
		private CustomLogics customLogics;
		internal LogicsSource() {
			interfaceInheritanceMap = new InterfaceInheritanceMap();
			autoLogics = new Dictionary<Type, List<Type>>();
			logicsCache = new Dictionary<Type, Type[]>();
		}
		private void AddLogic(Type targetInterface, Type logic) {
			List<Type> list;
			if(!autoLogics.TryGetValue(targetInterface, out list)) {
				list = new List<Type>();
				autoLogics.Add(targetInterface, list);
			}
			list.Add(logic);
		}
		private Type[] GetLogicsForInterface(Type type) {
			Type[] result;
			if(!logicsCache.TryGetValue(type, out result)) {
				List<Type> logics = new List<Type>();
				List<Type> list;
				if(autoLogics.TryGetValue(type, out list)) {
					foreach(Type logic in list) {
						if(!customLogics.IsUnregisteredLogic(type, logic)) {
							logics.Add(logic);
						}
					}
				}
				foreach(Type logic in customLogics.GetRegisteredLogics(type)) {
					if(!logics.Contains(logic) && customLogics.IsRegisteredLogic(type, logic)) {
						logics.Add(logic);
					}
				}
				result = logics.ToArray();
				logicsCache.Add(type, result);
			}
			return (Type[])result.Clone();
		}
		private MethodLogic CreateMethodLogic(MethodInfo method, bool hasInstanceParameter, bool hasObjectSpaceParameter) {
			MethodLogic logic = new MethodLogic();
			logic.HasInstanceParameter = hasInstanceParameter;
			logic.HasObjectSpaceParameter = hasObjectSpaceParameter;
			logic.IsStatic = method.IsStatic;
			logic.Owner = method.ReflectedType;
			logic.Name = method.Name;
			logic.IsAccessor = method.IsSpecialName;
			return logic;
		}
		private void HandleMultipleLogicsError(MethodLogic[] logics, Type interfaceType, string methodName, Type[] parameterTypes) {
			StringBuilder builder = new StringBuilder();
			string[] parameters = new string[parameterTypes.Length];
			for(int i = 0; i < parameterTypes.Length; ++i) {
				parameters[i] = parameterTypes[i].FullName;
			}
			builder.AppendFormat("There are '{0}' logics found for '{1}({2})' for '{3}':", logics.Length, methodName, string.Join(", ", parameters), interfaceType.FullName).AppendLine();
			foreach(MethodLogic logic in logics) {
				builder.Append("  ").Append(logic.Owner.FullName).Append('.').Append(logic.Name);
				if(logic.HasInstanceParameter) {
					builder.Append(" with instance parameter");
				}
				if(logic.HasObjectSpaceParameter) {
					builder.Append(" and 'IObjectSpace' parameter");
				}
				builder.AppendLine();
			}
			throw new InvalidOperationException(builder.ToString());
		}
		private MethodInfo[] GetSuitableMethods(Type[] logics, string methodName, Type returnType, Type[] parameterTypes, Type[] additionalParameterTypes) {
			Type[] parameterTypesWithAdditional = parameterTypes;
			int additionalLength = additionalParameterTypes.Length;
			if(additionalLength != 0) {
				parameterTypesWithAdditional = new Type[parameterTypes.Length + additionalLength];
				additionalParameterTypes.CopyTo(parameterTypesWithAdditional, 0);
				parameterTypes.CopyTo(parameterTypesWithAdditional, additionalLength);
			}
			List<MethodInfo> candidateMethods = new List<MethodInfo>();
			foreach(Type logic in logics) {
				MethodInfo method = logic.GetMethod(methodName, parameterTypesWithAdditional);
				if(IsSuitableMethod(method, returnType, parameterTypes)) {
					bool isLeafClassMethod = true;
					for(int i = candidateMethods.Count - 1; i >= 0; i--) {
						MethodInfo checkMethod = candidateMethods[i];
						if(method.ReflectedType.IsSubclassOf(checkMethod.ReflectedType)) {
							candidateMethods.RemoveAt(i);
							break;
						}
						if(checkMethod.ReflectedType.IsSubclassOf(method.ReflectedType)) {
							isLeafClassMethod = false;
							break;
						}
					}
					if(isLeafClassMethod) {
						candidateMethods.Add(method);
					}
				}
			}
			return candidateMethods.ToArray();
		}
		private bool IsSuitableMethod(MethodInfo method, Type returnType, Type[] parameterTypes) {
			if(method == null) return false;
			if(method.DeclaringType == typeof(object)) return false;
			if(method.ReturnType == typeof(void) && returnType != typeof(void)) return false;
			if(!returnType.IsAssignableFrom(method.ReturnType)) return false;
			ParameterInfo[] methodParameters = method.GetParameters();
			int shiftValue = methodParameters.Length - parameterTypes.Length;
			for(int i = 0; i < parameterTypes.Length; i++) {
				if(methodParameters[i + shiftValue].ParameterType != parameterTypes[i]) return false;
			}
			return true;
		}
		private MethodLogic[] GetMethodLogics(Type interfaceType, Type declaringType, string methodName, Type returnType, Type[] parameterTypes) {
			MethodLogic[] result = GetMethodLogicsForInterface(interfaceType, methodName, returnType, parameterTypes);
			if(result.Length == 0) {
				List<MethodLogic> logics = new List<MethodLogic>();
				foreach(Type parentType in interfaceInheritanceMap.GetParentTypes(interfaceType)) {
					if(declaringType.IsAssignableFrom(parentType)) {
						foreach(MethodLogic logic in GetMethodLogics(parentType, declaringType, methodName, returnType, parameterTypes)) {
							logics.Add(logic);
						}
					}
				}
				for(int i = logics.Count - 1; i > 0; i--) {
					MethodLogic logicToCompare = logics[i];
					for(int j = 0; j < i; j++) {
						if(AreSameMethodLogic(logicToCompare, logics[j])) {
							logics.RemoveAt(i);
							break;
						}
					}
				}
				int logicsCount = logics.Count;
				if(logicsCount > 0) {
					result = new MethodLogic[logicsCount];
					logics.CopyTo(result, 0);
				}
			}
			return result;
		}
		private bool AreSameMethodLogic(MethodLogic logicA, MethodLogic logicB) {
			if(logicA == logicB) return true;
			return logicA.Name == logicB.Name
				&& logicA.Owner == logicB.Owner
				&& logicA.IsStatic == logicB.IsStatic
				&& logicA.IsAccessor == logicB.IsAccessor
				&& logicA.HasInstanceParameter == logicB.HasInstanceParameter
				&& logicA.HasObjectSpaceParameter == logicB.HasObjectSpaceParameter;
		}
		private void CheckIsInitialized() {
			if(!isInitialized) {
				throw new InvalidOperationException("The LogicsSource is not initialized.");
			}
		}
		internal void CollectLogics(Type[] forInterfaces, CustomLogics customLogics) {
			Guard.ArgumentNotNull(forInterfaces, "forInterfaces");
			Guard.ArgumentNotNull(customLogics, "customLogics");
			autoLogics.Clear();
			logicsCache.Clear();
			this.customLogics = customLogics;
			interfaceInheritanceMap.Build(forInterfaces);
			Dictionary<Assembly, object> checkedAssemblies = new Dictionary<Assembly, object>();
			foreach(Type referencedType in interfaceInheritanceMap.GetProcessedTypes()) {
				Assembly assembly = referencedType.Assembly;
				if(!checkedAssemblies.ContainsKey(assembly)) {
					checkedAssemblies.Add(assembly, null);
					foreach(Type exportedType in assembly.GetExportedTypes()) {
						object[] attributes = exportedType.GetCustomAttributes(typeof(DomainLogicAttribute), false);
						foreach(DomainLogicAttribute attribute in attributes) {
							Type interfaceType = attribute.InterfaceType;
							if(interfaceInheritanceMap.IsProcessedType(interfaceType)) {
								AddLogic(interfaceType, exportedType);
							}
						}
					}
				}
			}
			isInitialized = true;
		}
		internal MethodLogic[] GetMethodLogicsForInterface(Type interfaceType, string methodName, Type returnType, Type[] parameterTypes) {
			CheckIsInitialized();
			Type[] logics = GetLogicsForInterface(interfaceType);
			if(logics.Length == 0) return new MethodLogic[0];
			List<MethodLogic> result = new List<MethodLogic>();
			foreach(MethodInfo method in GetSuitableMethods(logics, methodName, returnType, parameterTypes, Type.EmptyTypes)) {
				result.Add(CreateMethodLogic(method, false, false));
			}
			foreach(MethodInfo method in GetSuitableMethods(logics, methodName, returnType, parameterTypes, new Type[] { interfaceType })) {
				result.Add(CreateMethodLogic(method, true, false));
			}
			foreach(MethodInfo method in GetSuitableMethods(logics, methodName, returnType, parameterTypes, new Type[] { interfaceType, typeof(IObjectSpace) })) {
				result.Add(CreateMethodLogic(method, true, true));
			}
			return result.ToArray();
		}
		internal MethodLogic GetMethodLogic(Type interfaceType, Type declaringType, string methodName, Type returnType, Type[] parameterTypes) {
			CheckIsInitialized();
			if(!declaringType.IsAssignableFrom(interfaceType)) {
				throw new ArgumentException(string.Format(ErrorDeclaringTypeIsNotAssignableFromInterfaceType, declaringType, interfaceType), "declaringType");
			}
			MethodLogic[] logics = GetMethodLogics(interfaceType, declaringType, methodName, returnType, parameterTypes);
			if(logics.Length > 1) {
				HandleMultipleLogicsError(logics, interfaceType, methodName, parameterTypes);
			}
			if(logics.Length == 1) {
				return logics[0];
			}
			return null;
		}
	}
	sealed class MethodLogic {
		internal MethodLogic() { }
		internal bool IsAccessor { get; set; }
		internal bool IsStatic { get; set; }
		internal bool HasInstanceParameter { get; set; }
		internal bool HasObjectSpaceParameter { get; set; }
		internal string Name { get; set; }
		internal Type Owner { get; set; }
	}
}
