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
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class XRDesignerHostBase : XRDesignComponentContainer, IDesignerLoaderHost, IServiceContainer {
		#region static
		static string rootFullName;
		static public void SetRootFullName(string name) {
			rootFullName = name;
		}
		static IDesigner CreateDesigner(IComponent component, Type designerBaseType) {
			IDesigner designer = CreateComponentDesigner(component, designerBaseType);
			if(component is DevExpress.XtraReports.UI.XRControl)
				System.Diagnostics.Debug.Assert(designer != null, "Incorrect XRControlDesigner of " + component.GetType().Name);
			if(designer == null) {
				return component is System.Windows.Forms.Control ? new ControlDesigner() : new ComponentDesigner();
			}
			return designer;
		}
		static IDesigner CreateComponentDesigner(IComponent component, Type designerBaseType) {
			Type designerType = GetDesignerType(component, designerBaseType.FullName);
			return designerType == null ? null :
				(IDesigner)Activator.CreateInstance(designerType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
		}
		static Type GetDesignerType(IComponent component,string designerBaseTypeName) {
			if(component is System.Data.DataSet)
				return typeof(DevExpress.XtraReports.UserDesigner.Native.DataSetDesigner);
			if(DevExpress.Data.Native.BindingHelper.IsDataAdapter(component))
				return typeof(DevExpress.XtraReports.UserDesigner.Native.DataAdapterDesigner);
			AttributeCollection attributes = TypeDescriptor.GetAttributes(component);
			string designerTypeName = null;
			for(int i = 0; i < attributes.Count; i++) {
				if(attributes[i] is XRDesignerAttribute && TryGetTypeName((XRDesignerAttribute)attributes[i], designerBaseTypeName, ref designerTypeName))
					 break;
				if(string.IsNullOrEmpty(designerTypeName) && attributes[i] is DesignerAttribute)
					 TryGetTypeName((DesignerAttribute)attributes[i], designerBaseTypeName, ref designerTypeName);
			}
			return !string.IsNullOrEmpty(designerTypeName) ? GetType(component.Site, designerTypeName) : null;
		}
		static bool TryGetTypeName(DesignerAttribute attr, string baseTypeName, ref string value) {
			string attrBaseTypeName = GetTypeName(attr.DesignerBaseTypeName);
			if(attrBaseTypeName.Equals(baseTypeName)) {
				value = attr.DesignerTypeName;
				return true;
			}
			return false;
		}
		static bool TryGetTypeName(XRDesignerAttribute attr, string baseTypeName, ref string value) {
			string attrBaseTypeName = GetTypeName(attr.DesignerBaseTypeName);
			if(attrBaseTypeName.Equals(baseTypeName)) {
				value = attr.DesignerTypeName;
				return true;
			}
			return false;
		}
		static string GetTypeName(string assemblyQualifiedName) {
			string typeName;
			string assemblyName;
			DevExpress.XtraReports.Native.TypeResolver.SplitTypeName(assemblyQualifiedName, out typeName, out assemblyName);
			return typeName;
		}
		static Type GetType(IServiceProvider serviceProvider, string typeName) {
			ITypeResolutionService tr = (ITypeResolutionService)serviceProvider.GetService(typeof(ITypeResolutionService));
			try {
				return tr != null ? tr.GetType(typeName) : Type.GetType(typeName);
			} catch {
				return null;
			}
		}
		static void DisposeItems(IList items, IDisposable exclusiveItem) {
			foreach(IDisposable item in items)
				if(item != exclusiveItem)
					try {
						item.Dispose();
					} catch { }
			if(exclusiveItem != null)
				try {
					exclusiveItem.Dispose();
				} catch { }
		}
		#endregion
		Stack fTransactions = new Stack();
		XRDesignerLoader fLoader = new XRDesignerLoader();
		XRServiceContainer serviceContainer;
		Hashtable fDesigners = new Hashtable();
		IComponent fRootComponent;
		bool isDisposing = false;
		public XRDesignerHostBase(IServiceProvider parentServiceProvider) {
			serviceContainer = new XRServiceContainer(parentServiceProvider);
			Initialize();
		}
		private void Initialize() {
			Reload();
		}
		public IDictionary Designers {
			get { return fDesigners; }
		}
		#region Designer loading
		public XRDesignerLoader DesignerLoader {
			get { return this.fLoader; }
		}
		public virtual bool Loading {
			get { return this.fLoader.Loading; }
		}
		public void Activate() {
			OnActivated(EventArgs.Empty);
		}
		public void Deactivate() {
			OnDeactivated(EventArgs.Empty);
		}
		public void EndLoad(string baseClassName, bool successful, ICollection errorCollection) {
			OnLoadComplete(EventArgs.Empty);
		}
		public void Reload() {
			this.fLoader.BeginLoad(this);
			OnLoadComplete(EventArgs.Empty);
		}
		protected virtual void OnActivated(EventArgs e) {
			if(Activated != null)
				Activated(this, e);
		}
		protected virtual void OnDeactivated(EventArgs e) {
			if(Deactivated != null)
				Deactivated(this, e);
		}
		protected virtual void OnLoadComplete(EventArgs e) {
			if(LoadComplete != null)
				LoadComplete(this, e);
		}
		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler LoadComplete;
		#endregion
		#region Designer component management
		public IContainer Container {
			get { return this; }
		}
		public string RootComponentClassName {
			get { return RootComponent == null ? null : RootComponent.GetType().FullName; }
		}
		public override void Add(IComponent component) {
			INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
			string name = (nameCreationService != null) ? nameCreationService.CreateName(this, component.GetType()) : null;
			Add(component, name);
		}
		public override void Add(IComponent component, string name) {
			INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
			if(String.IsNullOrEmpty(name)) {
				name = (nameCreationService != null) ? nameCreationService.CreateName(this, component.GetType()) : null;
			}
			if(!nameCreationService.IsValidName(name))
				throw new ArgumentException("name");
			XRComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as XRComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.OnComponentAdding(component);
			base.Add(component, name);
			CreateDesigner(component);
			if(component is IExtenderProvider) {
				IExtenderProviderService extenderProviderService = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
				extenderProviderService.AddExtenderProvider((IExtenderProvider)component);
			}
			if(componentChangeService != null)
				componentChangeService.OnComponentAdded(component);
		}
		void CreateDesigner(IComponent component) {
			IDesigner designer = null;
			if(fRootComponent == null) {
				designer = CreateDesigner(component, typeof(IRootDesigner));
				fRootComponent = component;
			} else
				designer = CreateDesigner(component, typeof(IDesigner));
			if(designer != null) {
				fDesigners[component] = designer;
				try {
					designer.Initialize(component);
				} catch {
					if(designer is DevExpress.XtraReports.Design.XRComponentDesignerBase)
						throw;
				}
			}
		}
		public override void Remove(IComponent component) {
			if((component.Site == null) || (component.Site.Container != Container)) 
				return;
			XRComponentChangeService componentChangeService = componentChangeService = GetService(typeof(IComponentChangeService)) as XRComponentChangeService;
			if(componentChangeService != null && !isDisposing)
				componentChangeService.OnComponentRemoving(component);
			RemoveFormTray(component);
			RemoveDesigner(component);
			base.Remove(component);
			if(componentChangeService != null && !isDisposing)
				componentChangeService.OnComponentRemoved(component);
			component.Site = null;
			if(component is IExtenderProvider) {
				IExtenderProviderService extenderProviderService = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
				extenderProviderService.RemoveExtenderProvider((IExtenderProvider)component);
			}
		}
		void RemoveDesigner(IComponent component) {
			IDesigner designer = fDesigners[component] as IDesigner;
			if(designer != null) {
				fDesigners.Remove(component);
				try {
					designer.Dispose();
				} catch {
				}
			}
		}
		void RemoveFormTray(IComponent component) {
			ComponentTray tray = GetService(typeof(ComponentTray)) as ComponentTray;
			if(tray != null)
				tray.RemoveComponent(component);
		}
		public IComponent CreateComponent(Type componentClass, string name) {
			if(rootFullName == componentClass.FullName)
				return null;
			Type t = GetType(componentClass.FullName);
			IComponent component = t.Assembly.CreateInstance(t.FullName) as IComponent;
			if(component == null) {
				throw new ArgumentException("componentClass");
			}
			INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
			if(!nameCreationService.IsValidName(name))
				name = nameCreationService.CreateName(this, componentClass);
			this.Add(component, name);
			return component;
		}
		public IComponent CreateComponent(Type componentClass) {
			INameCreationService nameCreationService = (INameCreationService)GetService(typeof(INameCreationService));
			return this.CreateComponent(componentClass, nameCreationService.CreateName(this, componentClass));
		}
		public void DestroyComponent(IComponent component) {
			if(component.Site == null) return;
			if(InTransaction) {
				RemoveAndDispose(component);
				return;
			}
			DesignerTransaction t = CreateTransaction(String.Format("Delete {0}", component.Site.Name));
			try {
				RemoveAndDispose(component);
			} finally {
				t.Commit();
			}
		}
		void RemoveAndDispose(IComponent component) {
			Container.Remove(component);
			component.Dispose();
		}
		public IDesigner GetDesigner(IComponent component) {
			if(component == null)
				return null;
			return (IDesigner)Designers[component];
		}
		public Type GetType(string typeName) {
			ITypeResolutionService typeResolutionService = (ITypeResolutionService)GetService(typeof(ITypeResolutionService));
			if(typeResolutionService != null)
				return typeResolutionService.GetType(typeName);
			return DevExpress.XtraReports.Native.TypeResolver.GetType(typeName);
		}
		#endregion
		#region Designer transaction management
		public bool InTransaction {
			get { return this.fTransactions.Count > 0; }
		}
		public IComponent RootComponent {
			get { return fRootComponent; }
		}
		public string TransactionDescription {
			get {
				if(InTransaction) {
					DesignerTransaction trans = (DesignerTransaction)fTransactions.Peek();
					return trans.Description;
				}
				return null;
			}
		}
		public DesignerTransaction CreateTransaction(string description) {
			OnTransactionOpening(EventArgs.Empty);
			DesignerTransaction transaction = null;
			transaction = description == null ? new XRDesignerTransaction(this) : new XRDesignerTransaction(this, description);
			fTransactions.Push(transaction);
			OnTransactionOpened(EventArgs.Empty);
			return transaction;
		}
		public DesignerTransaction CreateTransaction() {
			return this.CreateTransaction(null);
		}
		internal void FireTransactionClosing(bool commit) {
			OnTransactionClosing(new DesignerTransactionCloseEventArgs(commit, fTransactions.Count == 1));
		}
		internal void FireTransactionClosed(bool commit) {
			this.fTransactions.Pop();
			OnTransactionClosed(new DesignerTransactionCloseEventArgs(commit, fTransactions.Count == 0));
		}
		protected virtual void OnTransactionOpening(EventArgs e) {
			if(TransactionOpening != null)
				TransactionOpening(this, e);
		}
		protected virtual void OnTransactionOpened(EventArgs e) {
			if(TransactionOpened != null)
				TransactionOpened(this, e);
		}
		protected virtual void OnTransactionClosing(DesignerTransactionCloseEventArgs e) {
			if(TransactionClosing != null)
				TransactionClosing(this, e);
		}
		protected virtual void OnTransactionClosed(DesignerTransactionCloseEventArgs e) {
			if(TransactionClosed != null)
				TransactionClosed(this, e);
		}
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			serviceContainer.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			serviceContainer.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceContainer.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			serviceContainer.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			serviceContainer.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			serviceContainer.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		protected override object GetService(Type serviceType) {
			object service = serviceContainer.GetService(serviceType);
			if(service == null) {
				INestedServiceProvider nestedServProvider = serviceContainer.GetService(typeof(INestedServiceProvider)) as INestedServiceProvider;
				if(nestedServProvider != null)
					return nestedServProvider.GetService(serviceType);
			}
			return service;
		}
		#endregion
		protected override void Dispose(bool dispose) {
			if(dispose) {
				isDisposing = true;
				Remove(RootComponent);
				WinControlContainer[] winControlContainers = CollectWinControlContainers();
				foreach(WinControlContainer winControlContainer in winControlContainers) 
					Remove(winControlContainer);
				base.Dispose(dispose);
				ClearDesigners();
				serviceContainer.Dispose();
			} else
				base.Dispose(dispose);
		}
		WinControlContainer[] CollectWinControlContainers() {
			List<WinControlContainer> result = new List<WinControlContainer>();
			foreach(IComponent component in Container.Components) {
				if(component is WinControlContainer)
					result.Add((WinControlContainer)component);
			}
			return result.ToArray();
		}
		void ClearDesigners() {
			IDesigner rootDesigner = (IDesigner)fDesigners[fRootComponent];
			ArrayList items = new ArrayList(fDesigners.Values);
			DisposeItems(items, rootDesigner);
			fDesigners.Clear();
		}
	}
}
