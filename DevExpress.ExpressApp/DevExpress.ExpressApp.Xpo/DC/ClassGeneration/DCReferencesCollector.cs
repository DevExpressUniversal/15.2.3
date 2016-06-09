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
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	sealed class DCReferencesCollector {
		private readonly List<Type> typesToProcess;
		private readonly List<Assembly> assembliesToProcess;
		private readonly Dictionary<string, object> references;
		private readonly HashSet<Type> processedTypes;
		private readonly HashSet<string> processedAssemblies;
		private readonly Dictionary<string, Assembly> loadedAssembliesByFullName;
		public DCReferencesCollector() {
			typesToProcess = new List<Type>();
			assembliesToProcess = new List<Assembly>();
			references = new Dictionary<string, object>();
			processedTypes = new HashSet<Type>();
			processedAssemblies = new HashSet<string>();
			loadedAssembliesByFullName = new Dictionary<string, Assembly>();
		}
		private void FillLoadedAssembliesByFullName() {
			Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly loadedAssembly in loadedAssemblies) {
				string loadedAssemblyFullName = loadedAssembly.FullName;
				if(!loadedAssembliesByFullName.ContainsKey(loadedAssemblyFullName)) {
					loadedAssembliesByFullName.Add(loadedAssemblyFullName, loadedAssembly);
				}
			}
		}
		private void ProcessType(Type type) {
			ProcessUsingTypes(type, 3);
		}
		private void ProcessUsingTypes(Type type, int level) {
			if(level != 0 && !processedTypes.Contains(type)) {
				ProcessTypeCore(type);
				foreach(Type curInterface in type.GetInterfaces()) {
					ProcessUsingTypes(curInterface, level - 1);
				}
				foreach(PropertyInfo pi in type.GetProperties()) {
					ProcessUsingTypes(pi.PropertyType, level - 1);
				}
				foreach(MethodInfo mi in type.GetMethods()) {
					ProcessUsingTypes(mi.ReturnType, level - 1);
				}
			}
		}
		private void ProcessTypeCore(Type type) {
			if(!processedTypes.Contains(type)) {
				processedTypes.Add(type);
				ProcessAssembly(type.Assembly);
			}
		}
		private void ProcessAssembly(Assembly assembly) {
			if(!processedAssemblies.Contains(assembly.FullName)) {
				ProcessAssemblyCore(assembly);
				if(!IsMscorlib(assembly.GetName())) {
					foreach(AssemblyName referencedAssemblyName in assembly.GetReferencedAssemblies()) {
						Assembly referencedAssembly;
						if(!IsMscorlib(referencedAssemblyName) && TryGetLoadedAssembly(referencedAssemblyName.FullName, out referencedAssembly)) {
							ProcessAssembly(referencedAssembly);
						}
					}
				}
			}
		}
		private void ProcessAssemblyCore(Assembly assembly) {
			if(!processedAssemblies.Contains(assembly.FullName)) {
				CheckDynamicAssembly(assembly);
				processedAssemblies.Add(assembly.FullName);
				AddReference(assembly.Location);
			}
		}
		private void CheckDynamicAssembly(Assembly assembly) {
			if(AssemblyHelper.IsDynamic(assembly)) {
				throw new InvalidOperationException("Cannot reference Domain Components from a dynamic assembly.");
			}
		}
		private void AddReference(string reference) {
			if(!references.ContainsKey(reference)) {
				references.Add(reference, null);
			}
		}
		private bool IsMscorlib(AssemblyName assemblyName) {
			return assemblyName.Name == "mscorlib";
		}
		private bool TryGetLoadedAssembly(string fullName, out Assembly assembly) {
			return loadedAssembliesByFullName.TryGetValue(fullName, out assembly);
		}
		public string[] Collect() {
			FillLoadedAssembliesByFullName();
			foreach(Assembly assembly in assembliesToProcess) {
				ProcessAssembly(assembly);
			}
			foreach(Type type in typesToProcess) {
				ProcessType(type);
			}
			string[] result = new string[references.Count];
			references.Keys.CopyTo(result, 0);
			return result;
		}
		public void Add(Type type) {
			typesToProcess.Add(type);
		}
		public void AddRange(IEnumerable<Type> types) {
			typesToProcess.AddRange(types);
		}
		public void Add(Assembly assembly) {
			assembliesToProcess.Add(assembly);
		}
	}
}
