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
using System.Collections;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using VSLangProj;
using EnvDTE;
using DevExpress.Utils.Design;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard {
	public class SourceItem {
		#region fields
		private TypeInfoDescriptor infoDescriptor;
		private IDataAccessTechnologyInfo dataAccessTechnologyInfo;
		private IList<MemberInfo> properties;
		private Type type;
		#endregion
		public TypeInfoDescriptor InfoDescriptor { get { return this.infoDescriptor; } }
		public IDataAccessTechnologyInfo DataAccessTechnologyInfo { get { return this.dataAccessTechnologyInfo; } }
		public IList<MemberInfo> Members { get { return this.properties; } }
		public Type Type { get { return type; } }
		public SourceItem(Type type) {
			this.type = type;
		}
		public SourceItem(Type type, TypeInfoDescriptor infoDescriptor)
			: this(type) {
			this.infoDescriptor = infoDescriptor;
			if(InfoDescriptor != null)
				this.properties = InfoDescriptor.FindMembers(type).ToList();
		}
		public SourceItem(Type type, TypeInfoDescriptor infoDescriptor, IDataAccessTechnologyInfo dataAccessTechnologyInfo)
			: this(type, infoDescriptor) {
			this.dataAccessTechnologyInfo = dataAccessTechnologyInfo;
		}
	}
	public static class TypeFinder {
		public static IList<SourceItem> SearchTypes(IDataAccessTechnologyInfo dataAccessTechnologyInfo) {
			List<SourceItem> result = new List<SourceItem>();
			TypeInfoDescriptor typeInfoDescriptor = null;
			foreach(Type type in AssemblyTypeIterator.Instance)
				if(TryFindTypeInBaseTypes(type, dataAccessTechnologyInfo, ref typeInfoDescriptor)) {
					SourceItem item = new SourceItem(type, typeInfoDescriptor, dataAccessTechnologyInfo);
					AddSafe(result, item);
				}
			return result;
		}
		internal static void AddSafe(List<SourceItem> result, SourceItem item) {
			if(result.Count(e => e.Type.FullName == item.Type.FullName) == 0)
				result.Add(item);
		}
		public static IList<SourceItem> SearchTypes(IDataAccessTechnologyCollectionProvider provider) {
			List<SourceItem> result = new List<SourceItem>();
			foreach(IDataAccessTechnologyInfo info in provider.DataAccessTechnologyCollection)
				result.AddRange(SearchTypes(info));
			return result;
		}
		public static IList<SourceItem> SearchTypes(TypeInfoDescriptor info) {
			List<SourceItem> result = new List<SourceItem>();
			foreach(Type type in AssemblyTypeIterator.Instance)
				if(info.Match(type)) {
					SourceItem item = new SourceItem(type, info);
					AddSafe(result, item);
				}
			return result;
		}
		private static bool TryFindTypeInBaseTypes(Type type, IDataAccessTechnologyInfo dataSourceInfo, ref TypeInfoDescriptor baseClassInfo) {
			baseClassInfo = dataSourceInfo.BaseClasses.FirstOrDefault(d => d.Match(type));
			return baseClassInfo != null;
		}
	}
	public class AssemblyTypeIterator : IEnumerable<Type> {
		private AssemblyTypeIterator() { }
		readonly static AssemblyTypeIterator instance = new AssemblyTypeIterator();
		public static AssemblyTypeIterator Instance { get { return instance; } }
		public IEnumerator<Type> GetEnumerator() {
			List<ReferenceItem> references = GetSolutionReferencedAssemblies();
			foreach(Assembly assembly in GetValidAssemblies(references)) {
				foreach(Type type in assembly.GetExportedTypes())
					if(Match(type))
						yield return type;
			}
		}
		private static IEnumerable<Assembly> GetValidAssemblies(List<ReferenceItem> references) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			return assemblies.Where(a => !a.IsDynamic && ReferenceItem.ContainsAssembly(references, a));
		}
		private static string GetAssemblyName(Project project) {
			foreach(Property prop in project.Properties) {
				if(prop.Name == "AssemblyName") return (string)prop.Value;
			}
			return project.Name;
		}
		private static List<ReferenceItem> GetSolutionReferencedAssemblies() {
			List<ReferenceItem> referenceNames = new List<ReferenceItem>();
			Project project = DTEHelper.GetCurrentProject();
			VSProject vsproject = (VSProject)project.Object;
			referenceNames.Add(new ReferenceItem() { Name = GetAssemblyName(project), IsSelfReference = true });
			using(new MessageFilter()) {
				foreach(Reference reference in vsproject.References)
					referenceNames.Add(new ReferenceItem() { Name = reference.Name, Version = reference.Version });
			}
			return referenceNames;
		}
		bool Match(Type type) {
			try {
				return !type.IsAbstract && !type.IsInterface && type.BaseType != null
					&& !type.GetCustomAttributes(false).Any(attr => attr.GetType().Name == "ItemsSourceWizardIgnoredAttribute");
			}
			catch(MethodAccessException) { return false; }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
	class ReferenceItem {
		public string Name { get; set; }
		public string Version { get; set; }
		public bool IsSelfReference { get; set; }
		public static bool ContainsAssembly(List<ReferenceItem> references, Assembly assembly) {
			return references.Any((r) => {
				AssemblyName assemblyName = assembly.GetName();
				if(!(r.Name == assemblyName.Name)) return false;
				return r.IsSelfReference || r.Version == assemblyName.Version.ToString();
			});
		}
	}
	public static class TypeExtensions {
		public static string GetFullName(this Type type) {
			if(type == null) return string.Empty;
			return type.Namespace + "." + type.Name;
		}
	}
	static class AssemblyExtensions {
		public static Type[] GetExportedTypesSafe(this Assembly assembly) {
			try {
				return assembly.GetExportedTypes();
			} catch {
				return new Type[0];
			}
		}
	}
}
