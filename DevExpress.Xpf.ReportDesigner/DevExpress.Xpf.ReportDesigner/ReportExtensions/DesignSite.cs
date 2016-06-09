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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions {
	public class DesignSite : Mvvm.BindableBase, ISite, IDesigner, IXRControlDesigner {
		public static void DoWithDesignMode(XtraReport report, Action<XtraReport> action) {
			if(report.Site is DesignSite) {
				DoWithRuntimeProperties(report, action);
				return;
			}
			EnableDesignMode(report, true);
			try {
				action(report);
			} finally {
				DisableDesignMode(report, false);
			}
		}
		public static void DoWithRuntimeProperties<T>(IEnumerable<T> components, Action<IEnumerable<T>> action) where T : IComponent {
			var componentsAndSites = components.Select(x => new { component = x, site = x.Site as DesignSite });
			foreach(var componentAndSite in componentsAndSites) {
				if(componentAndSite.site == null)
					throw new ArgumentException("", "components");
				componentAndSite.site.RestoreRuntimeProperties();
			}
			try {
				action(componentsAndSites.Select(x => x.component));
			} finally {
				foreach(var componentAndSite in componentsAndSites)
					componentAndSite.site.ResetRuntimeProperties();
			}
		}
		public static IDesignerHost EnableDesignMode(XtraReport report) {
			return EnableDesignMode(report, false);
		}
		public static void EnableDesignMode(IComponent component, bool componentIsNew, IDesignerHost designerHost) {
			var designerHostInstance = Guard.ArgumentMatchType<DesignerHost>(designerHost, "designerHost");
			EnableDesignMode(component, componentIsNew, designerHostInstance, false);
		}
		public static void DisableDesignMode(XtraReport report) {
			DisableDesignMode(report, true);
		}
		static void DoWithRuntimeProperties(XtraReport report, Action<XtraReport> action) {
			DoForAllControls(report, x => ((DesignSite)x.Site).RestoreRuntimeProperties());
			try {
				action(report);
			} finally {
				DoForAllControls(report, x => ((DesignSite)x.Site).ResetRuntimeProperties());
			}
		}
		static DesignerHost EnableDesignMode(XtraReport report, bool saveRuntimeProperties) {
			var designerHost = new DesignerHost(report);
			report.PrintingSystem.AddService(typeof(IBandLogic), new BandLogic());
			EnableDesignMode(report, false, designerHost, saveRuntimeProperties);
			return designerHost;
		}
		static void EnableDesignMode(IComponent component, bool componentIsNew, DesignerHost designerHost, bool saveRuntimeProperties) {
			DoForAllControls(component, x => EnableDesignModeCore(x, componentIsNew, designerHost, saveRuntimeProperties));
		}
		static void DisableDesignMode(XtraReport report, bool restoreRuntimeProperties) {
			DoForAllControls(report, x => DisableDesignModeCore(x, restoreRuntimeProperties));
			report.PrintingSystem.RemoveService(typeof(IBandLogic));
		}
		static void EnableDesignModeCore(IComponent component, bool componentIsNew, DesignerHost designerHost, bool saveRuntimeProperties) {
			var site = component is XRControl ? new XRControlDesignSite(component, componentIsNew, designerHost) : new DesignSite(component, componentIsNew, designerHost);
			component.Site = site;
			if(!saveRuntimeProperties)
				site.ResetRuntimeProperties();
		}
		static void DisableDesignModeCore(IComponent component, bool restoreRuntimeProperties) {
			if(restoreRuntimeProperties)
				((DesignSite)component.Site).RestoreRuntimeProperties();
			component.Site = null;
		}
		static void DoForAllControls(IComponent root, Action<IComponent> action) {
			action(root);
			var control = root as XRControl;
			if(control == null)
				return;
			foreach(var child in control.AllControls<XRControl>())
				action(child);
		}
		[CLSCompliant(false)]
		protected readonly IComponent component;
		readonly DesignerHost designerHost;
		readonly bool componentIsNew;
		string name;
		protected DesignSite(IComponent component, bool componentIsNew, DesignerHost designerHost) {
			Guard.ArgumentNotNull(component, "component");
			Guard.ArgumentNotNull(designerHost, "rootComponent");
			this.component = component;
			this.designerHost = designerHost;
			this.componentIsNew = componentIsNew;
			this.name = (string)component.GetType().GetProperty("Name").Return(x => x.GetValue(component, null), () => string.Empty);
		}
		public void OnAddToDesignerHost() {
			this.name = designerHost.CoerceName(component, name, componentIsNew);
		}
		public void OnRemoveFromDesignerHost() {
			designerHost.RemoveName(name);
		}
		protected virtual void RestoreRuntimeProperties() { }
		protected virtual void ResetRuntimeProperties() { }
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType.IsAssignableFrom(typeof(IDesignerHost))) return designerHost;
			return null;
		}
		IComponent ISite.Component { get { return component; } }
		IContainer ISite.Container { get { return null; } }
		bool ISite.DesignMode { get { return true; } }
		string ISite.Name {
			get { return name; }
			set {
				name = designerHost.ChangeName(component, false, name, value);
				Tracker.Set(component, "Name", name);
			}
		}
		IComponent IDesigner.Component { get { return component; } }
		DesignerVerbCollection IDesigner.Verbs { get { throw new NotSupportedException(); } }
		void IDesigner.DoDefaultAction() { }
		void IDesigner.Initialize(IComponent component) { }
		void IDisposable.Dispose() { }
		sealed class BandLogic : IBandLogic {
			float IBandLogic.GetMaxControlBottom(Band band, bool ignoreAnchoring) {
				return 0.0f;
			}
		}
		public sealed class DesignerHost : IDesignerHost {
			readonly IComponent rootComponent;
			readonly HashSet<string> existingNames = new HashSet<string>();
			public DesignerHost(IComponent rootComponent) {
				this.rootComponent = rootComponent;
			}
			public void RemoveName(string name) {
				existingNames.Remove(name);
			}
			public string CoerceName(IComponent component, string componentName, bool componentIsNew) {
				if(string.IsNullOrEmpty(componentName) || existingNames.Contains(componentName)) {
					var baseNameProvider = component as IXRControlBaseNameProvider;
					string baseName = baseNameProvider == null ? component.GetDefaultName() : baseNameProvider.GetBaseName();
					componentName = Enumerable.Range(1, int.MaxValue)
						.Select(x => baseName + x)
						.Where(x => !existingNames.Contains(x) && !XRNameCreationService.RootComponentHasMember(rootComponent, x))
						.First();
					ChangeTextIfNeeded(component, componentIsNew, null, componentName);
				}
				existingNames.Add(componentName);
				return componentName;
			}
			public string ChangeName(IComponent component, bool componentIsNew, string oldName, string newName) {
				if(!string.IsNullOrEmpty(oldName))
					existingNames.Remove(oldName);
				if(!string.IsNullOrEmpty(newName) && existingNames.Contains(newName))
					newName = oldName;
				if(!string.IsNullOrEmpty(newName))
					existingNames.Add(newName);
				ChangeTextIfNeeded(component, componentIsNew, oldName, newName);
				return newName;
			}
			void ChangeTextIfNeeded(IComponent component, bool componentIsNew, string oldText, string newText) {
				var label = component as XRLabel;
				if(label == null) return;
				if(ShouldChangeText(componentIsNew, label.Text, oldText, newText))
					Tracker.Set(label, x => x.Text, newText);
			}
			bool ShouldChangeText(bool newComponent, string currentControlText, string oldText, string newText) {
				if(string.IsNullOrEmpty(oldText)) {
					return newComponent && string.IsNullOrEmpty(currentControlText);
				} else {
					return currentControlText == oldText;
				}
			}
			event EventHandler IDesignerHost.Activated { add { } remove { } }
			event EventHandler IDesignerHost.Deactivated { add { } remove { } }
			event EventHandler IDesignerHost.LoadComplete { add { } remove { } }
			event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosed { add { } remove { } }
			event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosing { add { } remove { } }
			event EventHandler IDesignerHost.TransactionOpened { add { } remove { } }
			event EventHandler IDesignerHost.TransactionOpening { add { } remove { } }
			bool IDesignerHost.Loading { get { return false; } }
			bool IDesignerHost.InTransaction { get { return false; } }
			IContainer IDesignerHost.Container { get { return null; } }
			IComponent IDesignerHost.RootComponent { get { return rootComponent; } }
			string IDesignerHost.RootComponentClassName { get { return rootComponent.GetType().FullName; } }
			string IDesignerHost.TransactionDescription { get { return null; } }
			void IDesignerHost.Activate() { throw new NotSupportedException(); }
			IComponent IDesignerHost.CreateComponent(Type componentClass) { throw new NotSupportedException(); }
			IComponent IDesignerHost.CreateComponent(Type componentClass, string name) { throw new NotSupportedException(); }
			DesignerTransaction IDesignerHost.CreateTransaction() { throw new NotSupportedException(); }
			DesignerTransaction IDesignerHost.CreateTransaction(string description) { throw new NotSupportedException(); }
			void IDesignerHost.DestroyComponent(IComponent component) { throw new NotSupportedException(); }
			IDesigner IDesignerHost.GetDesigner(IComponent component) { return (DesignSite)component.Site; }
			Type IDesignerHost.GetType(string typeName) { throw new NotSupportedException(); }
			void IServiceContainer.AddService(Type serviceType, object serviceInstance) { throw new NotSupportedException(); }
			void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) { throw new NotSupportedException(); }
			void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) { throw new NotSupportedException(); }
			void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) { throw new NotSupportedException(); }
			void IServiceContainer.RemoveService(Type serviceType) { throw new NotSupportedException(); }
			void IServiceContainer.RemoveService(Type serviceType, bool promote) { throw new NotSupportedException(); }
			object IServiceProvider.GetService(Type serviceType) {
				if(serviceType.IsAssignableFrom(typeof(IDesignerHost))) return this;
				return null;
			}
		}
		#region Bindings
		PropertyDescriptorCollection IXRControlDesigner.GetBindableProperties(Type winFormsConverterType) {
			PropertyDescriptorCollection designProperties = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component, null);
			Attribute[] designBindingAttributes = new Attribute[] { new NotifyParentPropertyAttribute(true) };
			foreach(PropertyDescriptor property in properties) {
				var bindableAttribute = (BindableAttribute)property.Attributes[typeof(BindableAttribute)];
				if(bindableAttribute != null && bindableAttribute.Bindable && !SkipBindableProperty(property))
					designProperties.Add(new DataBindingPropertyDescriptor(property, designBindingAttributes));
			}
			return designProperties;
		}
		bool SkipBindableProperty(PropertyDescriptor property) {
			var barCode = component as XRBarCode;
			if(barCode == null) return false;
			return barCode.Symbology is XtraPrinting.BarCode.BarCode2DGenerator && property.Name == ExpressionHelper.GetPropertyName((XRBarCode x) => x.BinaryData);
		}
		class DataBindingPropertyDescriptor : PropertyDescriptor {
			public DataBindingPropertyDescriptor(PropertyDescriptor propertyDescriptor, Attribute[] attrs) : base(propertyDescriptor, attrs) { }
			public override Type ComponentType { get { return typeof(XRBindingCollection); } }
			public override Type PropertyType { get { return typeof(BindingSettings); } }
			public override bool IsReadOnly { get { return false; } }
			public override bool ShouldSerializeValue(object component) { return false; }
			public override TypeConverter Converter { get { return TypeDescriptor.GetConverter(PropertyType); } }
			public override bool CanResetValue(object component) {
				return GetBinding(component).Data != null;
			}
			public override void ResetValue(object component) {
				GetBinding(component).Data = null;
			}
			public override object GetValue(object component) {
				return GetBinding(component);
			}
			public override void SetValue(object component, object value) {
				throw new InvalidOperationException();
			}
			BindingSettings GetBinding(object component) {
				return new BindingSettings((XRBindingCollection)component, Name);
			}
		}
		#endregion
	}
	public interface IXRControlBaseNameProvider {
		string GetBaseName();
	}
}
