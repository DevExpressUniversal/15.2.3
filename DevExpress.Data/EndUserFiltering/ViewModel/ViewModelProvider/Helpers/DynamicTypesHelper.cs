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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	public static class DynamicTypesHelper {
		static readonly string dynamicSuffix = "Dynamic_" + Guid.NewGuid().ToString();
#if DEBUGTEST
		static IDictionary<string, AssemblyBuilder> aCache = new Dictionary<string, AssemblyBuilder>();
#endif
		static IDictionary<string, ModuleBuilder> mCache = new Dictionary<string, ModuleBuilder>();
		public static TypeBuilder GetTypeBuilder(Type serviceType) {
			var moduleBuilder = GetModuleBuilder(serviceType.Assembly);
			return moduleBuilder.DefineType(GetDynamicTypeName(serviceType));
		}
		public static TypeBuilder GetTypeBuilder(Type serviceType, Type sourceType) {
			var moduleBuilder = GetModuleBuilder(serviceType.Assembly);
			return moduleBuilder.DefineType(GetDynamicTypeName(sourceType), TypeAttributes.NotPublic, sourceType);
		}
		public static ModuleBuilder GetModuleBuilder(Assembly assembly) {
			string strAssemblyName = assembly.GetName().Name;
			ModuleBuilder moduleBuilder;
			if(!mCache.TryGetValue(strAssemblyName, out moduleBuilder)) {
				var assemblyName = new AssemblyName(strAssemblyName + dynamicSuffix);
#if DEBUGTEST
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
				aCache.Add(strAssemblyName, assemblyBuilder);
#else
				var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
				moduleBuilder = assemblyBuilder.DefineDynamicModule(GetDynamicAssemblyName(strAssemblyName));
				mCache.Add(strAssemblyName, moduleBuilder);
			}
			return moduleBuilder;
		}
		public static string GetDynamicAssemblyName(string assemblyName) {
			return assemblyName + "." + dynamicSuffix + ".dll";
		}
		public static string GetDynamicTypeName(string typeName, string typeNameModifier) {
			if(string.IsNullOrEmpty(typeNameModifier))
				return typeName + "_" + dynamicSuffix;
			return typeName + "_" + typeNameModifier + "_" + dynamicSuffix;
		}
		public static string GetDynamicTypeName(Type type) {
			return GetDynamicTypeName(type, null);
		}
		public static string GetDynamicTypeName(Type type, string typeNameModifier) {
			if(string.IsNullOrEmpty(type.Namespace))
				return GetDynamicTypeName(GetTypeName(type), typeNameModifier);
			return GetDynamicTypeName(type.Namespace + "." + GetTypeName(type), typeNameModifier);
		}
		static string GetTypeName(Type type) {
			if(!type.IsGenericType)
				return type.Name;
			var sb = new System.Text.StringBuilder(type.Name);
			int argumentsPos = type.Name.IndexOf('`');
			sb.Remove(argumentsPos, type.Name.Length - argumentsPos);
			sb.Append("<");
			var genericArgs = type.GetGenericArguments();
			for(int i = 0; i < genericArgs.Length; i++) {
				sb.Append(GetTypeName(genericArgs[i]));
				if(i > 0) sb.Append(",");
			}
			sb.Append(">");
			return sb.ToString();
		}
#if DEBUGTEST
		public static void Save() {
			foreach(var item in aCache)
				item.Value.Save(GetDynamicAssemblyName(item.Key));
		}
#endif
	}
}
