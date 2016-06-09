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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Reflection;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.Serialization.Services;
using DevExpress.XtraReports.Wizards.Builder;
namespace DevExpress.XtraReports.Serialization {
	public class XRCodeDomDesignerSerializationManager : IDesignerSerializationManager, IServiceContainer, IDisposable {
		class XRTypeDescriptorFilterService : ITypeDescriptorFilterService {
			static void FilterDataSourceProperty(System.ComponentModel.IComponent component, System.Collections.IDictionary properties) {
				if(component is IDataContainer) {
					object dataSource = ((IDataContainer)component).GetSerializableDataSource();
					if(SerializationUtils.IsFakedComponent(dataSource))
						properties.Remove("DataSource");
				} else
					if(component is System.Windows.Forms.Control) {
						System.Windows.Forms.Control control = (System.Windows.Forms.Control)component;
						PropertyInfo dataSourcePi = control.GetType().GetProperty("DataSource");
						if(dataSourcePi != null && SerializationUtils.IsFakedComponent(dataSourcePi.GetValue(control, null)))
							properties.Remove("DataSource");
					}
			}
			IServiceProvider serviceProvider;
			public XRTypeDescriptorFilterService(IServiceProvider serviceProvider) {
				this.serviceProvider = serviceProvider;
			}
			#region System.ComponentModel.Design.ITypeDescriptorFilterService interface implementation
			public bool FilterProperties(System.ComponentModel.IComponent component, System.Collections.IDictionary properties) {
				ArrayList filteredProps = new ArrayList();
				if(component is XtraReport) {
					PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(DevExpress.XtraReports.UI.XtraReport));
					foreach(PropertyDescriptor property in properties.Values) {
						if(!props.Contains(property))
							filteredProps.Add(property);
					}
				}
				if(serviceProvider != null) {
					IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
					if(designerHost != null) {
						IDesignerFilter designerFilter = designerHost.GetDesigner(component) as IDesignerFilter;
						if(designerFilter != null) {
							designerFilter.PreFilterProperties(properties);
						}
					}
				}
				FilterDataSourceProperty(component, properties);
				foreach(PropertyDescriptor property in filteredProps) {
					properties.Remove(property.Name);
				}
				return false;
			}
			public bool FilterEvents(System.ComponentModel.IComponent component, System.Collections.IDictionary events) {
				return false;
			}
			public bool FilterAttributes(System.ComponentModel.IComponent component, System.Collections.IDictionary attributes) {
				return false;
			}
			#endregion
		}
		public static Type GetSerializationResourceManagerType() {
			return typeof(ComponentDesigner).Assembly.GetType("System.ComponentModel.Design.Serialization.ResourceCodeDomSerializer+SerializationResourceManager");
		}
		ContextStack context;
		Hashtable nameToObject = new Hashtable();
		Hashtable objectToName = new Hashtable();
		System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.PropertyDescriptorCollection.Empty;
		ArrayList providers = new ArrayList();
		IDesignerHost host;
		ServiceContainer services;
		ITypeResolutionService typeResolutionService;
		bool suppressTypeDescriptionProviderService;
		public XRCodeDomDesignerSerializationManager(IDesignerHost myHost)
			: this(myHost, false) {
		}
		public XRCodeDomDesignerSerializationManager(IDesignerHost myHost, bool suppressTypeDescriptionProviderService) {
			host = myHost;
			this.suppressTypeDescriptionProviderService = suppressTypeDescriptionProviderService;
			context = new ContextStack();
			services = new ServiceContainer(host);
		}
		protected ITypeDescriptorFilterService CreateTypeDescriptorFilterService(IServiceProvider serviceProvider) {
			return new XRTypeDescriptorFilterService(serviceProvider);
		}
		protected IResourceService CreateResourceService() {
			return new XRDesignerResourceService(this);
		}
		protected INameCreationService CreateNameCreationService(IDesignerHost host) {
			return new XRNameCreationService(host);
		}
		public void Initialize(IServiceProvider nativeServiceProvider, IServiceProvider rootNativeServiceProvider) {
			typeResolutionService = new STypeResolutionService(nativeServiceProvider, typeof(DevExpress.XtraReports.UI.XtraReport).Assembly);
			services.AddService(typeof(IDesignerSerializationManager), this);
			services.AddService(typeof(IServiceContainer), this);
			services.AddService(typeof(IDesignerHost), host);
			services.AddService(typeof(INameCreationService), CreateNameCreationService(host));
			services.AddService(typeof(ITypeDescriptorFilterService), CreateTypeDescriptorFilterService(nativeServiceProvider));
			services.AddService(typeof(IDictionaryService), new SDictionaryService());
			services.AddService(typeof(IReferenceService), new SReferenceService(host));
			services.AddService(typeof(IResourceService), CreateResourceService());
			services.AddService(typeof(IEventBindingService), new SEventBindingService(this));
			if(!suppressTypeDescriptionProviderService)
				AddServiceFromProvider(rootNativeServiceProvider, typeof(TypeDescriptionProviderService));
			AddServiceFromProvider(rootNativeServiceProvider, typeof(IResourceWriterBuilder));
			Activator.CreateInstance(GetSerializationResourceManagerType(), new object[] { this });
		}
		public void Dispose() {
			((IDisposable)typeResolutionService).Dispose();
			services.RemoveService(typeof(IDesignerSerializationManager));
			services.RemoveService(typeof(IServiceContainer));
			services.RemoveService(typeof(IDesignerHost));
			services.RemoveService(typeof(INameCreationService));
			services.RemoveService(typeof(ITypeDescriptorFilterService));
			services.RemoveService(typeof(IDictionaryService));
			services.RemoveService(typeof(IReferenceService));
			services.RemoveService(typeof(IResourceService));
			services.RemoveService(typeof(IEventBindingService));
			services.RemoveService(typeof(IResourceWriterBuilder));
			if(!suppressTypeDescriptionProviderService)
				RemoveService(typeof(TypeDescriptionProviderService));
		}
		public CodeDomSerializer GetRootSerializer(Type objectType) {
			Type type = typeof(CodeDomSerializer).Assembly.GetType("System.ComponentModel.Design.Serialization.RootCodeDomSerializer");
			return Activator.CreateInstance(type) as CodeDomSerializer;
		}
		public void OnSerializationComplete() {
			if(SerializationComplete != null) {
				SerializationComplete(this, EventArgs.Empty);
			}
		}
		Type AddServiceFromProvider(IServiceProvider nativeServiceProvider, string typeName) {
			if(nativeServiceProvider == null)
				return null;
			Type serviceType = Type.GetType(typeName);
			if(serviceType == null)
				return null;
			AddServiceFromProvider(nativeServiceProvider, serviceType);
			return serviceType;
		}
		void AddServiceFromProvider(IServiceProvider nativeServiceProvider, Type serviceType) {
			if(nativeServiceProvider == null || serviceType == null)
				return;
			object service = nativeServiceProvider.GetService(serviceType);
			if(service == null)
				return;
			services.AddService(serviceType, service);
		}
		void RemoveService(Type serviceType) {
			if(serviceType != null) {
				services.RemoveService(serviceType);
			}
		}
		object GetSerializerFromType(Type objectType, Type serializerType) {
			if(objectType == null)
				return null;
			DesignerSerializerAttribute serAtt = (DesignerSerializerAttribute)TypeDescriptor.GetAttributes(objectType)[typeof(DesignerSerializerAttribute)];
			if(serAtt != null) {
				if(serAtt.SerializerBaseTypeName.StartsWith(serializerType.FullName)) {
					Type type = GetType(serAtt.SerializerTypeName);
					if(type != null)
						return Activator.CreateInstance(type);
				}
			}
			return GetSerializerFromType(objectType.BaseType, serializerType);
		}
		object GetSerializerFromProvider(Type objectType, Type serializerType) {
			object currentSerializer = null;
			bool repeatFlag = true;
			for(int i = 0; repeatFlag && (i < this.providers.Count); i++) {
				repeatFlag = false;
				foreach(IDesignerSerializationProvider provider in this.providers) {
					object serializer = provider.GetSerializer(this, currentSerializer, objectType, serializerType);
					if(serializer != null) {
						repeatFlag = currentSerializer != serializer;
						currentSerializer = serializer;
					}
				}
			}
			if(currentSerializer != null)
				return currentSerializer;
			return GetSerializerFromProvider(objectType.BaseType, serializerType);
		}
		object OnResolveName(string name) {
			if(ResolveName == null) {
				return null;
			}
			ResolveNameEventArgs e = new ResolveNameEventArgs(name);
			ResolveName(this, e);
			return e.Value;
		}
		#region ISerivceProvider implementation
		public object GetService(System.Type serviceType) {
			return services.GetService(serviceType);
		}
		#endregion
		#region IServiceContainer implementation
		void IServiceContainer.RemoveService(Type serviceType) {
			services.RemoveService(serviceType);
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
			services.RemoveService(serviceType, promote);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			services.AddService(serviceType, serviceInstance);
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
			services.AddService(serviceType, callback);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			services.AddService(serviceType, serviceInstance, promote);
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			services.AddService(serviceType, callback, promote);
		}
		#endregion
		#region IDesignerSerializationManager implementation
		ContextStack IDesignerSerializationManager.Context {
			get { return context; }
		}
		System.ComponentModel.PropertyDescriptorCollection IDesignerSerializationManager.Properties {
			get { return props; }
		}
		public void AddSerializationProvider(IDesignerSerializationProvider provider) {
			providers.Add(provider);
		}
		object IDesignerSerializationManager.CreateInstance(Type type, ICollection arguments, string name, bool addToContainer) {
			return null;
		}
		string IDesignerSerializationManager.GetName(object value) {
			object name = objectToName[value];
			return (string)name;
		}
		object IDesignerSerializationManager.GetSerializer(Type objectType, Type serializerType) {
			object serializer = GetSerializerFromType(objectType, serializerType);
			if(serializer != null) {
				return serializer;
			}
			return GetSerializerFromProvider(objectType, serializerType);
		}
		public Type GetType(string typeName) {
			try {
				return typeResolutionService.GetType(typeName);
			} catch {
				return null;
			}
		}
		public void RemoveSerializationProvider(IDesignerSerializationProvider provider) {
			providers.Remove(provider);
		}
		void IDesignerSerializationManager.ReportError(object errorInformation) {
			System.Diagnostics.Debug.Assert(false, errorInformation.ToString());
		}
		object IDesignerSerializationManager.GetInstance(string name) {
			object obj = nameToObject[name];
			if(obj != null)
				return obj;
			obj = OnResolveName(name);
			if(obj != null) {
				nameToObject[name] = obj;
			}
			return obj;
		}
		public void SetName(object instance, string name) {
			object oldName = objectToName[instance];
			if(name == oldName as string) {
				return;
			}
			object oldInstance = nameToObject[name];
			if((oldInstance != null) & (oldInstance != instance)) {
				throw new Exception("An object with name already exists.");
			}
			if(oldName != null) {
				nameToObject.Remove(name);
			}
			objectToName[instance] = name;
			nameToObject[name] = instance;
			Component c = instance as Component;
			if(c != null && c.Site != null) {
				c.Site.Name = name;
			}
		}
		public event ResolveNameEventHandler ResolveName;
		public event EventHandler SerializationComplete;
		#endregion
	}
}
