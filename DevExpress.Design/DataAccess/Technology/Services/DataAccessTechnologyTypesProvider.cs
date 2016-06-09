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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	public abstract class BaseDataAccessTechnologyTypesProvider : IDataAccessTechnologyTypesProvider {
		protected IEnumerable<Type> types;
		protected BaseDataAccessTechnologyTypesProvider(IEnumerable<Type> dtTypes) {
			this.types = dtTypes;
		}
		#region IEnumerable Members
		protected virtual IEnumerator<Type> GetEnumeratorCore() {
			return types.GetEnumerator();
		}
		IEnumerator<Type> IEnumerable<Type>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
		#endregion
		protected internal static bool ValidForDataAccessTechnologyConstraints(Type type) {
			return !type.IsAbstract && !type.IsInterface && (type.BaseType != null) &&
				!IsStatic(type) && !IsRelatedToEvents(type) && !IsRelatedToAttributes(type);
		}
		internal static bool IsStatic(Type type) {
			return type.IsSealed && type.IsAbstract;
		}
		internal static bool IsRelatedToAttributes(Type type) {
			return
				typeof(Attribute).IsAssignableFrom(type);
		}
		internal static bool IsRelatedToEvents(Type type) {
			return
				typeof(Delegate).IsAssignableFrom(type) ||
				typeof(EventArgs).IsAssignableFrom(type);
		}
		internal static bool IsRelatedToUI(Type type) {
			return
				typeof(System.Windows.Forms.Control).IsAssignableFrom(type) ||
				typeof(System.Windows.Controls.Control).IsAssignableFrom(type) ||
				typeof(System.Web.UI.Control).IsAssignableFrom(type);
		}
	}
	public sealed class TypeDiscoveryServiceTypesProvider : IDataAccessTechnologyTypesProvider {
		HashSet<Type> types;
		public TypeDiscoveryServiceTypesProvider(System.IServiceProvider serviceProvider) {
			types = new HashSet<Type>();
			var dteTypes = DevExpress.Design.UI.Platform.GetTypes();
			if(dteTypes != null) {
				for(int i = 0; i < dteTypes.Length; i++)
					types.Add(dteTypes[i]);
			}
			var dataSourceProviderServiceTypes = GetTypesViaDataSourceProviderService(serviceProvider);
			foreach(Type dataSourceProviderServiceType in dataSourceProviderServiceTypes)
				types.Add(dataSourceProviderServiceType);
			System.Reflection.Assembly rootTypeAssembly = null;
			var typeResolutionService = GetService<System.ComponentModel.Design.ITypeResolutionService>(serviceProvider);
			if(typeResolutionService != null) {
				var host = GetService<System.ComponentModel.Design.IDesignerHost>(serviceProvider);
				if(host != null && !string.IsNullOrEmpty(host.RootComponentClassName)) {
					var rootType = typeResolutionService.GetType(host.RootComponentClassName);
					if(rootType != null) {
						rootTypeAssembly = rootType.Assembly;
						LoadAssemblyTypes(rootType.Assembly);
					}
				}
			}
			var projectAssemblies = DevExpress.Design.UI.Platform.GetProjectAssemblies();
			foreach(var assembly in projectAssemblies) {
				if(assembly != null && assembly != rootTypeAssembly)
					LoadAssemblyTypes(assembly);
			}
		}
		void LoadAssemblyTypes(System.Reflection.Assembly assembly) {
			try {
				var assemblyTypes = assembly.GetTypes();
				for(int i = 0; i < assemblyTypes.Length; i++)
					types.Add(assemblyTypes[i]);
			}
			catch(System.Reflection.ReflectionTypeLoadException e) {
				var loadedTypes = e.Types;
				for(int i = 0; i < loadedTypes.Length; i++) {
					if(loadedTypes[i] != null)
						types.Add(loadedTypes[i]);
				}
			}
		}
		static IEnumerable<Type> GetTypesViaDataSourceProviderService(System.IServiceProvider serviceProvider) {
			var designerHost = GetService<System.ComponentModel.Design.IDesignerHost>(serviceProvider);
			var dsProviderService = GetService<System.ComponentModel.Design.Data.DataSourceProviderService>(serviceProvider);
			if(dsProviderService != null && designerHost != null) {
				var dataSources = Safe(() => dsProviderService.GetDataSources());
				if(dataSources != null) {
					foreach(System.ComponentModel.Design.Data.DataSourceGroup dsGroup in dataSources) {
						foreach(System.ComponentModel.Design.Data.DataSourceDescriptor dsDescriptor in dsGroup.DataSources) {
							Type dsType = Safe(() => designerHost.GetType(dsDescriptor.TypeName));
							if(dsType != null)
								yield return dsType;
						}
					}
				}
			}
		}
		static T Safe<T>(Func<T> f) {
			try { return f(); }
			catch { return default(T); }
		}
		static T GetService<T>(System.IServiceProvider serviceProvider) where T : class {
			return serviceProvider.GetService(typeof(T)) as T;
		}
		#region IEnumerable Members
		IEnumerator<Type> GetEnumeratorCore() {
			foreach(Type type in types) {
				if(BaseDataAccessTechnologyTypesProvider.ValidForDataAccessTechnologyConstraints(type))
					yield return type;
			}
		}
		IEnumerator<Type> IEnumerable<Type>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
		#endregion
	}
	public sealed class DesignerHostComponentTypesProvider : IDataAccessTechnologyComponentsProvider {
		System.ComponentModel.Design.IDesignerHost designerHost;
		public DesignerHostComponentTypesProvider(System.ComponentModel.Design.IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		#region IEnumerable Members
		IEnumerator<System.ComponentModel.IComponent> GetEnumeratorCore() {
			if(designerHost != null && designerHost.Container != null) {
				foreach(System.ComponentModel.IComponent component in designerHost.Container.Components)
					yield return component;
			}
		}
		IEnumerator<System.ComponentModel.IComponent> IEnumerable<System.ComponentModel.IComponent>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
		#endregion
		object IDataAccessTechnologyComponentsProvider.CreateComponent(string componentTypeName, IDictionary defaultValues) {
			if(designerHost == null)
				return null;
			Type componentType = designerHost.GetType(componentTypeName);
			if(componentType == null)
				return null;
			using(var createComponent = designerHost.CreateTransaction(DataAccessLocalizer.GetString(DataAccessLocalizerStringId.CreateSqlDataSourceTransactionName))) {
				var component = designerHost.CreateComponent(componentType);
				if(component != null) {
					var componentDesigner = designerHost.GetDesigner(component) as System.ComponentModel.Design.ComponentDesigner;
					if(componentDesigner != null)
						componentDesigner.InitializeNewComponent(defaultValues);
				}
				createComponent.Commit();
				return component;
			}
		}
	}
	public sealed class DefaultDataAccessTechnologyTypesProvider : BaseDataAccessTechnologyTypesProvider {
		public DefaultDataAccessTechnologyTypesProvider(IEnumerable<Type> types)
			: base(types) {
		}
		public static IEnumerable<Type> GetAllAvailableTypes() {
			return Metadata.AvailableTypes.All(ValidForDataAccessTechnologyConstraints);
		}
	}
	public sealed class DefaultDataAccessTechnologyLocalTypesProvider : BaseDataAccessTechnologyTypesProvider {
		public DefaultDataAccessTechnologyLocalTypesProvider()
			: base(Metadata.AvailableTypes.Local(ValidForDataAccessTechnologyConstraints)) {
		}
	}
	public sealed class DefaultDataAccessTechnologyComponentsProvider : IDataAccessTechnologyComponentsProvider {
		IEnumerable<System.ComponentModel.IComponent> components;
		public DefaultDataAccessTechnologyComponentsProvider(IEnumerable<System.ComponentModel.IComponent> components) {
			this.components = components;
		}
		IEnumerator<System.ComponentModel.IComponent> IEnumerable<System.ComponentModel.IComponent>.GetEnumerator() {
			return components.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return components.GetEnumerator();
		}
		object IDataAccessTechnologyComponentsProvider.CreateComponent(string componentTypeName, IDictionary defaultValues) { return null; }
	}
}
namespace DevExpress.Design.DataAccess.Win {
	public sealed class DataAccessTechnologyTypesProviderFactory : IDataAccessTechnologyTypesProviderFactory {
		public IDataAccessTechnologyTypesProvider Create(System.IServiceProvider serviceProvider) {
			if(serviceProvider is System.ComponentModel.Design.IDesignerHost)
				return new TypeDiscoveryServiceTypesProvider(serviceProvider);
			return new DefaultDataAccessTechnologyLocalTypesProvider();
		}
	}
	public sealed class DataAccessTechnologyComponentsProviderFactory : IDataAccessTechnologyComponentsProviderFactory {
		public IDataAccessTechnologyComponentsProvider Create(System.IServiceProvider serviceProvider) {
			var designerHost = serviceProvider as System.ComponentModel.Design.IDesignerHost ??
				serviceProvider.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) as System.ComponentModel.Design.IDesignerHost;
			return new DesignerHostComponentTypesProvider(designerHost);
		}
	}
}
