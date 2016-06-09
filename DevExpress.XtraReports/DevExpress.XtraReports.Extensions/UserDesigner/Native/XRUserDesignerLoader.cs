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
using System.Collections;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design;
using System.Reflection;
using DevExpress.XtraReports.Serialization;
using DevExpress.Serialization.Services;
using DevExpress.DataAccess;
namespace DevExpress.XtraReports.Native {
	public class ComponentLoadHelper {
		#region static
		public static string GetRootComponentName(IComponent component, IDesignerHost designerHost) {
			string name = GetNameProperty(component);
			if(string.IsNullOrEmpty(name))
				name = component.GetType().Name;
			return GetValidName(component, name, designerHost);
		}
		static public string GetComponentName(IComponent component, string componentName, IDesignerHost designerHost) {
			string name = componentName != null ? componentName : GetNameProperty(component);
			return GetValidName(component, name, designerHost);
		}
		static string GetValidName(IComponent component, string componentName, IDesignerHost designerHost) {
			INameCreationService ncs = (INameCreationService)designerHost.GetService(typeof(System.ComponentModel.Design.Serialization.INameCreationService));
			bool isValidName = false;
			try {
				isValidName = ncs.IsValidName(componentName);
			} catch {
			}
			return isValidName ? componentName : ncs.CreateName(designerHost.Container, component.GetType());
		}
		static public string GetNameProperty(IComponent component) {
			System.Reflection.PropertyInfo propertyInfo = PropInfoAccessor.GetProperty(component, "Name", false);
			return propertyInfo != null ? (string)propertyInfo.GetValue(component, null) : string.Empty;
		}
		static void FilterSitedComponents(Hashtable components) {
			ArrayList sitedComponents = new ArrayList();
			foreach(IComponent component in components.Keys)
				if(component != null && component.Site != null)
					sitedComponents.Add(component);
			foreach(IComponent component in sitedComponents)
				components.Remove(component);
		}
		#endregion
		public virtual Hashtable GetComponentsFromReport(XtraReport report) {
			Hashtable hostedComponents = new Hashtable();
			IList components = report.GetAssociatedComponents();
			Hashtable componentNamesHT = report.GetComponentNamesHT(components);
			foreach(IComponent component in components) {
				if(report.ComponentStorage.Contains(component) && component.GetType().GetConstructor(new Type[] { typeof(IContainer) }) != null) {
					TryRemoveFromContainer(component);
				}
				if(IsEnabledType(component.GetType()) && !hostedComponents.ContainsKey(component)) {
					hostedComponents.Add(component, componentNamesHT[component]);
				}
			}
			FilterSitedComponents(hostedComponents);
			return hostedComponents;
		}
		public Hashtable GetComponentsFromFields(XtraReport source) {
			Hashtable hostedComponents = GetFieldComponents(source);
			foreach(IComponent component in hostedComponents.Keys) {
				if(source.ComponentStorage.Contains(component) && component.GetType().GetConstructor(new Type[] { typeof(IContainer) }) != null) {
					TryRemoveFromContainer(component);
				}
			}
			FilterSitedComponents(hostedComponents);
			return hostedComponents;
		}
		static bool TryRemoveFromContainer(IComponent component) {
			if(component.Site != null && !component.Site.DesignMode) {
				IContainer container = component.Site.GetService(typeof(IContainer)) as IContainer;
				if(container != null) {
					container.Remove(component);
					return true;
				}
			}
			return false;
		}
		Hashtable GetFieldComponents(XtraReport report) {
			Hashtable components = new Hashtable();
			if(report != null) {
				System.Reflection.FieldInfo[] fInfos = report.GetType().GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fInfos) {
					if (IsEnabledType(fieldInfo.FieldType) && !typeof(XtraReport).IsAssignableFrom(fieldInfo.FieldType)) {
						try {
							object val = fieldInfo.GetValue(report);
							if (val != null)
								components[val] = fieldInfo.Name;
						} catch { }
					}
				}
			}
			return components;
		}
		protected virtual bool IsEnabledType(Type type) {
			return typeof(IComponent).IsAssignableFrom(type) && !typeof(System.Data.IDbCommand).IsAssignableFrom(type) && !typeof(System.Data.IDbConnection).IsAssignableFrom(type); 
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner.Native
{
	public class XRDesignerLoader : DesignerLoader
	{
		bool fLoading;
		IDesignerLoaderHost designerHost;
		public XRDesignerLoader() {
		}
		public override bool Loading { get { return fLoading; }
		}
		public IDesignerLoaderHost DesignerLoaderHost {
			get { return designerHost; }
		}
		#region System.ComponentModel.Design.Serialization.DesignerLoader abstract class implementation
		public override void Dispose() {
		}
		public override void BeginLoad(System.ComponentModel.Design.Serialization.IDesignerLoaderHost host) {
			fLoading = true;
			designerHost = host; 
		}
		public void LoadReport(XtraReport report, XtraReport source) {
			ComponentLoadHelper componentLoader = new ComponentLoadHelper();
			designerHost.Container.Add(report, ComponentLoadHelper.GetRootComponentName(report, designerHost));
			bool loadDesignTimeProperties = report.GetType() != typeof(XtraReport);
			System.Resources.ResourceSet resourceSet = null;
			if(loadDesignTimeProperties) {
				System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager(report.GetType());
				resourceSet = ResLoader.GetResourceSet(resourceManager);
				ResLoader.LoadDesignTimeProperties(report, designerHost, resourceSet);
			}
			Hashtable components = source != null ? componentLoader.GetComponentsFromFields(source) : 
				componentLoader.GetComponentsFromReport(report);
			foreach(IComponent component in components.Keys) {
				string name = ComponentLoadHelper.GetComponentName(component, (string)components[component], designerHost);
				designerHost.Container.Add(component, name);
				if(component is XRControl && loadDesignTimeProperties)
					ResLoader.LoadDesignTimeProperties((XRControl)component, designerHost, resourceSet);
			}
		}
		public virtual void EndLoad() {
			fLoading = false;
			designerHost.EndLoad("DesignerLoader", false, null);
		}
		#endregion
		public override void Flush() {
			base.Flush();
		}
	}
}
