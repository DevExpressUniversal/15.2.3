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
using System.Reflection;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.Serialization.Services {
	public class STypeResolutionService : ITypeResolutionService, IDisposable {
		static bool assemblyResolving = false;
		static bool IsExistsTypeInAssembly(Assembly assembly, string type, bool caseInsensitiveCache) {
			return assembly != null && assembly.GetType(type, false, caseInsensitiveCache) != null;
		}
		static int GetCommaIndex(string typeName) {
			int startIndex = typeName.LastIndexOf(']');
			startIndex = (startIndex == -1) ? 0 : startIndex;
			return typeName.IndexOf(',', startIndex);
		}
		IServiceProvider serviceProvider;
		Hashtable localTypeCache = new Hashtable();
		bool caseInsensitiveCache;
		ArrayList referencedAssemblies = new ArrayList();
		Assembly rootAssembly;
		Assembly ResolveType(string typeName) {
			Type type = GetCachedType(typeName);
			if(type != null)
				return type.Assembly;
			Assembly assembly = GetAssemblyInternal(typeName);
			if(assembly != null)
				SaveTypeInCache(typeName, assembly.GetType(typeName, false, caseInsensitiveCache));
			return assembly;
		}
		Assembly GetAssemblyInternal(string typeName) {
			int index = GetCommaIndex(typeName);
			if(index != -1) {
				string assemblyName = typeName.Substring(index + 1).Trim();
				return ResolveAssembly(assemblyName);
			} else {
				Assembly assembly = GetRootAssembly();
				if(IsExistsTypeInAssembly(assembly, typeName, caseInsensitiveCache))
					return assembly;
				AssemblyName[] assemblyNames = GetReferencedAssemblies();
				foreach(AssemblyName assemblyName in assemblyNames) {
					assembly = GetAssembly(assemblyName, false);
					if(IsExistsTypeInAssembly(assembly, typeName, caseInsensitiveCache))
						return assembly;
				}
				AssemblyName execAssemblyName = Assembly.GetExecutingAssembly().GetName();
				Assembly[] assembliesInDomain = AppDomain.CurrentDomain.GetAssemblies();
				foreach(Assembly assemblyInDomain in assembliesInDomain) {
					AssemblyName domainAssemblyName = assemblyInDomain.GetName();
					if((!domainAssemblyName.Name.StartsWith("DevExpress") || ComparePublicKeyTokenAndVersion(domainAssemblyName, execAssemblyName)) && 
						IsExistsTypeInAssembly(assemblyInDomain, typeName, caseInsensitiveCache)) {
						return assemblyInDomain;
					}
				}
			}
			return null;
		}
		bool ComparePublicKeyTokenAndVersion(AssemblyName left, AssemblyName right) {
			return ComparePublicKeyToken(left.GetPublicKeyToken(), right.GetPublicKeyToken()) && (left.Version == right.Version);
		}
		bool ComparePublicKeyToken(byte[] left, byte[] right) {
			if(left == null || right == null)
				return false;
			if(left.Length != right.Length)
				return false;
			for(int i = 0; i < left.Length; i++) {
				if(left[i] != right[i])
					return false;
			}
			return true;
		}
		Type GetCachedType(string typeName) {
			return localTypeCache != null ? (Type)localTypeCache[typeName] : null;
		}
		void SaveTypeInCache(string typeName, Type type) {
			if(type != null) {
				if(localTypeCache == null)
					localTypeCache = CreateTypeHash(caseInsensitiveCache);
				localTypeCache[typeName] = type;
			}
		}
		Hashtable CreateTypeHash(bool ignoreCase) {
			return ignoreCase ?
				new Hashtable(StringComparer.InvariantCultureIgnoreCase) :
				new Hashtable();
		}
		Assembly ResolveAssembly(string assemName) {
			return DevExpress.Data.Utils.AssemblyCache.LoadWithPartialName(assemName);
		}
		Assembly ResolveAssembly(AssemblyName assemName) {
			try {
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnAssemblyResolve);
				return DevExpress.Data.Utils.AssemblyCache.Load(assemName);
			} finally {
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(OnAssemblyResolve);
			}
		}
		Assembly GetRootAssembly() {
			IDesignerHost host = serviceProvider != null ? serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost : null;
			return host != null && host.RootComponent != null ? host.RootComponent.GetType().Assembly : rootAssembly;
		}
		AssemblyName[] GetReferencedAssemblies() {
			Assembly assembly = GetRootAssembly();
			ArrayList result = assembly != null ? new ArrayList(assembly.GetReferencedAssemblies()) : new ArrayList(new AssemblyName[] { });
			result.AddRange(referencedAssemblies);
			return (AssemblyName[])result.ToArray(typeof(AssemblyName));
		}
		Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
			if(assemblyResolving)
				return null;
			assemblyResolving = true;
			try {
				return DevExpress.Data.Utils.AssemblyCache.LoadWithPartialName(args.Name);
			} finally {
				assemblyResolving = false;
			}
		}
		public STypeResolutionService(IServiceProvider serviceProvider, Assembly rootAssembly) {
			this.serviceProvider = serviceProvider;
			this.rootAssembly = rootAssembly;
		}
		public void Dispose() {
		}
		public Assembly GetAssembly(AssemblyName name) {
			return GetAssembly(name, false);
		}
		public Assembly GetAssembly(AssemblyName name, bool throwOnError) {
			Assembly asm = null;
			try {
				asm = ResolveAssembly(name);
				return asm;
			} catch(Exception e) {
				if(throwOnError)
					throw e;
			}
			return null;
		}
		public string GetPathOfAssembly(AssemblyName name) {
			Assembly assembly = GetAssembly(name);
			if(assembly != null) {
				return assembly.Location;
			}
			return null;
		}
		Type ITypeResolutionService.GetType(string name) {
			return ((ITypeResolutionService)this).GetType(name, false, false);
		}
		Type ITypeResolutionService.GetType(string name, bool throwOnError) {
			return ((ITypeResolutionService)this).GetType(name, throwOnError, false);
		}
		Type ITypeResolutionService.GetType(string name, bool throwOnError, bool ignoreCase) {
			Type type = GetType(name, ignoreCase);
			if(type == null && throwOnError) {
				throw new TypeLoadException("Could not find type '" + name + "'");
			}
			return type;
		}
		public Type GetType(string typeName, bool ignoreCase) {
			if(typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			int idx = GetCommaIndex(typeName);
			string typeOnlyName = (idx != -1) ? typeName.Substring(0, idx).Trim() :
				typeName;
			if(ignoreCase != caseInsensitiveCache) {
				localTypeCache = null;
			}
			if(localTypeCache == null) {
				caseInsensitiveCache = ignoreCase;
				localTypeCache = CreateTypeHash(caseInsensitiveCache);
			}
			Type type = (Type)localTypeCache[typeOnlyName];
			Assembly assembly = null;
			if(type == null) {
				assembly = ResolveType(typeName);
				if(assembly == null) {
					type = Type.GetType(typeName, false, ignoreCase);
					if(type != null) localTypeCache[typeOnlyName] = type;
				} else {
					type = assembly.GetType(typeOnlyName, false, ignoreCase);
				}
			}
			return type;
		}
		public void ReferenceAssembly(AssemblyName name) {
			referencedAssemblies.Add(name);
		}
	}
	public class SReferenceService : IReferenceService, IDisposable {
		#region inner classes
		class ReferenceHolder {
			string trailingName;
			object reference;
			IComponent sitedComponent;
			string fullName = null;
			public ReferenceHolder(string trailingName, object reference, IComponent sitedComponent) {
				this.trailingName = trailingName;
				this.reference = reference;
				this.sitedComponent = sitedComponent;
				System.Diagnostics.Debug.Assert(trailingName != null);
				System.Diagnostics.Debug.Assert(reference != null);
			}
			public string Name {
				get {
					if(fullName == null) {
						fullName = sitedComponent.Site.Name + trailingName;
					}
					return fullName;
				}
			}
			public object Reference {
				get { return reference; }
			}
			public IComponent SitedComponent {
				get { return sitedComponent; }
			}
			public void ResetName() {
				this.fullName = null;
			}
		}
		#endregion
		bool initialized = false;
		ArrayList addedComponents = new ArrayList();
		ArrayList removedComponents = new ArrayList();
		ArrayList referenceList;
		Dictionary<object, ReferenceHolder> referenceDict;
		IDesignerHost designerHost;
		void RemoveReferences(IComponent component) {
			int size = referenceList.Count;
			for(int i = size - 1; i >= 0; i--) {
				ReferenceHolder referenceHolder = (ReferenceHolder)referenceList[i];
				if(referenceHolder.SitedComponent == component) {
					referenceList.RemoveAt(i);
					bool containsKey = false;
					try {
						containsKey = referenceDict.ContainsKey(referenceHolder.Reference);
					} catch { }
					if(containsKey)
						referenceDict.Remove(referenceHolder.Reference);
				}
			}
		}
		void CreateReferences(IComponent component) {
			CreateReferences(string.Empty, component, component);
		}
		void CreateReferences(string trailingName, object reference, IComponent sitedComponent) {
			if(reference == null)
				return;
			ReferenceHolder referenceHolder = new ReferenceHolder(trailingName, reference, sitedComponent);
			referenceList.Add(referenceHolder);
			if(!referenceDict.ContainsKey(reference))
				referenceDict.Add(reference, referenceHolder);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(reference, new Attribute[] { DesignerSerializationVisibilityAttribute.Content });
			for(int i = 0; i < properties.Count; i++) {
				if(properties[i].IsReadOnly) {
					try {
						string propertyName = properties[i].Name;
						CreateReferences(trailingName + "." + propertyName,
							properties[i].GetValue(reference),
							sitedComponent);
					} catch { }
				}
			}
		}
		void EnsureReferences() {
			if(!initialized) {
				ComponentCollection components = designerHost.Container.Components;
				foreach(IComponent comp in components) {
					CreateReferences(comp);
				}
				initialized = true;
			} else {
				if(this.addedComponents.Count > 0) {
					foreach(IComponent ic in addedComponents) {
						RemoveReferences(ic);
						CreateReferences(ic);
					}
					addedComponents.Clear();
				}
				if(this.removedComponents.Count > 0) {
					foreach(IComponent ic in removedComponents) {
						RemoveReferences(ic);
					}
					removedComponents.Clear();
				}
			}
		}
		void OnComponentAdd(object sender, ComponentEventArgs e) {
			if(initialized) {
				addedComponents.Add(e.Component);
				removedComponents.Remove(e.Component);
			}
		}
		void OnComponentRemove(object sender, ComponentEventArgs e) {
			if(initialized) {
				removedComponents.Add(e.Component);
				addedComponents.Remove(e.Component);
			}
		}
		void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			foreach(ReferenceHolder reference in this.referenceList) {
				if(reference.SitedComponent == e.Component) {
					reference.ResetName();
					return;
				}
			}
		}
		public SReferenceService(IDesignerHost designerHost) {
			this.designerHost = designerHost;
			referenceList = new ArrayList();
			referenceDict = new Dictionary<object, ReferenceHolder>();
			IComponentChangeService changeService = designerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdd);
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemove);
				changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		public void Dispose() {
			IComponentChangeService changeService = designerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(changeService != null) {
				changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdd);
				changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemove);
				changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
			}
			referenceList.Clear();
			referenceDict.Clear();
			addedComponents.Clear();
			removedComponents.Clear();
		}
		#region IReferenceService implementation
		IComponent IReferenceService.GetComponent(object reference) {
			EnsureReferences();
			ReferenceHolder referenceHolder;
			return referenceDict.TryGetValue(reference, out referenceHolder) ? referenceHolder.SitedComponent : null;
		}
		string IReferenceService.GetName(object reference) {
			EnsureReferences();
			ReferenceHolder referenceHolder;
			return referenceDict.TryGetValue(reference, out referenceHolder) ? referenceHolder.Name : null;
		}
		object IReferenceService.GetReference(string name) {
			EnsureReferences();
			int size = referenceList.Count;
			for(int i = 0; i < size; i++) {
				ReferenceHolder referenceHolder = (ReferenceHolder)referenceList[i];
				if(string.Compare(referenceHolder.Name, name, true, CultureInfo.InvariantCulture) == 0) {
					return referenceHolder.Reference;
				}
			}
			return null;
		}
		object[] IReferenceService.GetReferences() {
			EnsureReferences();
			int size = referenceList.Count;
			object[] references = new object[size];
			for(int i = 0; i < size; i++) {
				references[i] = ((ReferenceHolder)referenceList[i]).Reference;
			}
			return references;
		}
		object[] IReferenceService.GetReferences(Type baseType) {
			EnsureReferences();
			int size = referenceList.Count;
			ArrayList resultList = new ArrayList();
			for(int i = 0; i < size; i++) {
				object reference = ((ReferenceHolder)referenceList[i]).Reference;
				if(baseType.IsAssignableFrom(reference.GetType())) {
					resultList.Add(reference);
				}
			}
			object[] objectArray = new object[resultList.Count];
			resultList.CopyTo(objectArray, 0);
			return objectArray;
		}
		#endregion
	}
	public class SDictionaryService : IDictionaryService {
		Hashtable table = new Hashtable();
		#region System.ComponentModel.Design.IDictionaryService interface implementation
		public void SetValue(object key, object val) {
			if(key != null)
				table[key] = val;
		}
		public object GetValue(object key) {
			return key == null ? null : table[key];
		}
		public object GetKey(object val) {
			foreach(DictionaryEntry entry in table) {
				if(entry.Value == val)
					return entry.Key;
			}
			return null;
		}
		#endregion
	}
}
