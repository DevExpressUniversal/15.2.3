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
using System.Reflection.Emit;
namespace DevExpress.ExpressApp.Utils.Reflection {
	public static class AssemblyHelper {
		private static readonly Dictionary<Assembly, AssemblyData> cache;
		static AssemblyHelper() {
			cache = new Dictionary<Assembly, AssemblyData>();
		}
		private static AssemblyData GetData(Assembly assembly) {
			AssemblyData data;
			if(!cache.TryGetValue(assembly, out data)) {
				lock(cache) {
					if(!cache.TryGetValue(assembly, out data)) {
						data = new AssemblyData(assembly);
						cache.Add(assembly, data);
					}
				}
			}
			return data;
		}
		public static Type[] GetTypesFromAssembly(Assembly assembly) {
			return AssemblyData.GetTypesFromAssembly(assembly);
		}
		public static Type[] GetTypes(Assembly assembly) {
			return GetData(assembly).GetTypes();
		}
		public static Type[] GetTypes(Assembly assembly, Predicate<Type> filter) {
			Type[] types = GetTypes(assembly);
			List<Type> result = new List<Type>(types.Length);
			foreach(Type type in types) {
				if(filter(type)) {
					result.Add(type);
				}
			}
			return result.ToArray();
		}
		public static String GetName(Assembly assembly) {
			return GetData(assembly).Name;
		}
		public static Version GetVersion(Assembly assembly) {
			return GetData(assembly).Version;
		}
		public static Boolean IsDynamic(Assembly assembly) {
			return GetData(assembly).IsDynamic;
		}
		public static Boolean TryGetType(String assemblyName, String typeFullName, out Type type) {
			type = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies) {
				if(GetName(assembly) == assemblyName) {
					type = assembly.GetType(typeFullName);
					break;
				}
			}
			return (type != null);
		}
	}
	sealed class AssemblyData {
		private readonly Assembly assembly;
		private Type[] types;
		private readonly Object typesLocker;
		private String name;
		private Version version;
		private Boolean? isDynamic;
		public static Type[] GetTypesFromAssembly(Assembly assembly) {
			Type[] assemblyTypes;
			try {
				assemblyTypes = assembly.GetTypes();
			}
			catch(ReflectionTypeLoadException typeLoadException) {
				assemblyTypes = typeLoadException.Types;
			}
			List<Type> exportedTypes = new List<Type>(assemblyTypes.Length);
			foreach(Type type in assemblyTypes) {
				if(type != null && type.IsVisible) {
					exportedTypes.Add(type);
				}
			}
			return exportedTypes.ToArray();
		}
		public AssemblyData(Assembly assembly) {
			this.assembly = assembly;
			typesLocker = new Object();
		}
		public Type[] GetTypes() {
			if(types == null) {
				lock(typesLocker) {
					if(types == null) {
						types = AssemblyData.GetTypesFromAssembly(assembly);
					}
				}
			}
			Type[] result = new Type[types.Length];
			types.CopyTo(result, 0);
			return result;
		}
		public String Name {
			get {
				if(name == null) {
					name = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
				}
				return name;
			}
		}
		public Version Version {
			get {
				if(version == null) {
					String assemblyFullName = assembly.FullName;
					Int32 nameLength = assemblyFullName.IndexOf(',');
					Int32 startIndex = assemblyFullName.IndexOf('=', nameLength) + 1;
					Int32 endIndex = assemblyFullName.IndexOf(',', startIndex);
					String versionString = assemblyFullName.Substring(startIndex, endIndex - startIndex);
					version = new Version(versionString);
				}
				return version;
			}
		}
		public Boolean IsDynamic {
			get {
				if(!isDynamic.HasValue) {
					isDynamic = assembly is AssemblyBuilder || assembly.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder";
				}
				return isDynamic.Value;
			}
		}
	}
}
