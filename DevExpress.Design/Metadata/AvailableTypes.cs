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

namespace DevExpress.Design.Metadata {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	public static class AvailableTypes {
		public static IEnumerable<Type> All(Predicate<Type> match) {
			var dteTypes = DevExpress.Design.UI.Platform.GetTypes();
			if(dteTypes != null && dteTypes.Length > 0)
				return All(dteTypes, match);
			return All(GetAllAssemblies(), match);
		}
		public static IEnumerable<Type> Local(Predicate<Type> match) {
			var dteTypes = DevExpress.Design.UI.Platform.GetTypes();
			if(dteTypes != null && dteTypes.Length > 0)
				return All(dteTypes, match);
			return All(GetLocalAssemblies(), match);
		}
		static Assembly[] GetAllAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies();
		}
		internal static IList<Assembly> assemblyCache;
#if DEBUGTEST
		internal static IEnumerable<Assembly> LocalAssembliesForTests;
#endif
		static IEnumerable<Assembly> GetLocalAssemblies() {
#if DEBUGTEST
			if(LocalAssembliesForTests != null) {
				foreach(Assembly testAssembly in LocalAssembliesForTests)
					yield return testAssembly;
				yield break;
			}
#endif
			if(assemblyCache != null) {
				foreach(Assembly assembly in assemblyCache)
					yield return assembly;
				yield break;
			}
			else assemblyCache = new List<Assembly>();
			Assembly[] allAssemblies = GetAllAssemblies();
			for(int i = 0; i < allAssemblies.Length; i++) {
				if(DevExpress.Design.UI.Platform.IsProjectAssembly(allAssemblies[i].GetName())) {
					assemblyCache.Add(allAssemblies[i]);
					yield return allAssemblies[i];
				}
			}
		}
		public static IEnumerable<Type> All(IEnumerable<Assembly> assemblies, Predicate<Type> match) {
			AssertionException.IsNotNull(match);
			foreach(Assembly assembly in assemblies) {
				Type[] assemblyTypes = null;
				try {
					assemblyTypes = assembly.GetTypes();
				}
				catch(ReflectionTypeLoadException) { continue; }
				foreach(Type type in assemblyTypes) {
					if(match(type))
						yield return type;
				}
			}
		}
		public static IEnumerable<Type> All(IEnumerable<Type> types, Predicate<Type> match) {
			foreach(Type type in types) {
				if(match(type))
					yield return type;
			}
		}
		public static IEnumerable<T> Unique<T>(IEnumerable<T> elements, Converter<T, Type> convert) {
			AssertionException.IsNotNull(convert);
			HashSet<string> uniqueTypes = new HashSet<string>();
			foreach(T element in elements) {
				Type type = convert(element);
				if(type != null && !uniqueTypes.Contains(type.FullName)) {
					uniqueTypes.Add(type.FullName);
					yield return element;
				}
			}
		}
	}
}
