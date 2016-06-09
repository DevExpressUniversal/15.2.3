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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design.Internal;
namespace DevExpress.Utils.Design {
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	public class DXCollectionEditorBase : UITypeEditor {
		#region Editor
		#region ctor
		public DXCollectionEditorBase(Type type) {
			this.collectionType = type;
		}
		#endregion
		#region Public Methods
		bool ignoreChangingEvents;
		bool ignoreChangedEvents;
		IList collectionCore;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(editorService != null) {
					this.currentContext = context;
					DXCollectionEditorBase.DXCollectionEditorBaseForm xtraCollectionForm = (DXCollectionEditorBaseForm)this.CreateCollectionForm();
					SetStorePath(context, xtraCollectionForm);
					ITypeDescriptorContext typeDescriptorContext = this.currentContext;
					xtraCollectionForm.EditValue = value;
					this.collectionCore = value as IList;
					this.ignoreChangingEvents = false;
					this.ignoreChangedEvents = false;
					DesignerTransaction designerTransaction = null;
					bool flag = true;
					IComponentChangeService componentChangeService = null;
					IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
					try {
						try {
							if(designerHost != null)
								designerTransaction = designerHost.CreateTransaction(GetFormatedString("CollectionEditorUndoBatchDesc", new object[] { this.CollectionItemType.Name }));
						}
						catch(CheckoutException ex) {
							if(ex == CheckoutException.Canceled)
								return value;
							throw ex;
						}
						componentChangeService = ((designerHost != null) ? ((IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService))) : null);
						if(componentChangeService != null) {
							componentChangeService.ComponentChanged += OnComponentChanged;
							componentChangeService.ComponentChanging += this.OnComponentChanging;
						}
						if(xtraCollectionForm.ShowEditorDialog(editorService) == DialogResult.OK)
							value = xtraCollectionForm.EditValue;
						else
							flag = false;
					}
					finally {
						xtraCollectionForm.EditValue = null;
						this.currentContext = typeDescriptorContext;
						if(designerTransaction != null) {
							if(flag)
								designerTransaction.Commit();
							else
								designerTransaction.Cancel();
						}
						if(componentChangeService != null) {
							componentChangeService.ComponentChanged -= OnComponentChanged;
							componentChangeService.ComponentChanging -= this.OnComponentChanging;
						}
						xtraCollectionForm.Dispose();
						ResetSettings();
					}
					return value;
				}
			}
			return value;
		}
		protected virtual void ResetSettings() {
			this.collectionItemType = null;
			this.newItemTypes = null;
			this.currentContext = null;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		#endregion
		#region Event Handlers
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(!this.ignoreChangedEvents && sender != this.Context.Instance) {
				this.ignoreChangedEvents = true;
				this.Context.OnComponentChanged();
			}
		}
		void OnComponentChanging(object sender, ComponentChangingEventArgs e) {
			if(!this.ignoreChangingEvents && sender != this.Context.Instance) {
				this.ignoreChangingEvents = true;
				this.Context.OnComponentChanging();
			}
		}
		#endregion
		#region Protected Properties
		Type collectionType;
		protected Type CollectionType {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get { return this.collectionType; }
		}
		Type collectionItemType;
		protected Type CollectionItemType {
			get {
				if(this.collectionItemType == null)
					this.collectionItemType = this.CreateCollectionItemType();
				return this.collectionItemType;
			}
		}
		Type[] newItemTypes;
		protected Type[] NewItemTypes {
			get {
				if(this.newItemTypes == null)
					this.newItemTypes = this.CreateNewItemTypes();
				return this.newItemTypes;
			}
		}
		ITypeDescriptorContext currentContext;
		protected ITypeDescriptorContext Context {
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get { return currentContext; }
		}
		protected object GetService(Type serviceType) {
			if(this.Context != null)
				return this.Context.GetService(serviceType);
			return null;
		}
		protected string GetFormatedString(string name, params object[] args) {
			string @string = name;
			if(args != null && args.Length != 0) {
				for(int i = 0; i < args.Length; i++) {
					string text = args[i] as string;
					if(text != null && text.Length > 1024)
						args[i] = text.Substring(0, 1021) + "...";
				}
				return string.Format(CultureInfo.CurrentCulture, @string, args);
			}
			return @string;
		}
		#endregion
		#region Virtual Methods
		protected virtual string HelpTopic {
			get { return GetHelpTopic(); }
		}
		string GetHelpTopic() {
			ComponentDesigner designer = GetInstanceComponentDesigner();
			if(designer == null) return "DXCollectionEditor";
			return designer.Component.GetType().Name;
		}
		protected virtual Type CreateCollectionItemType() {
			PropertyInfo[] properties = TypeDescriptor.GetReflectionType(this.CollectionType).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			for(int i = 0; i < properties.Length; i++) {
				if(properties[i].Name.Equals("Item") || properties[i].Name.Equals("Items"))
					return properties[i].PropertyType;
			}
			return typeof(object);
		}
		protected virtual Type[] CreateNewItemTypes() {
			return new Type[] { this.CollectionItemType };
		}
		protected virtual object CreateInstance(Type itemType) {
			object obj = null;
			IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if(typeof(IComponent).IsAssignableFrom(itemType) && host != null) {
				obj = host.CreateComponent(itemType, null);
				PerformAdd(obj as IComponent, host);
			}
			if(obj == null)
				obj = TypeDescriptor.CreateInstance(host, itemType, null, null);
			return obj;
		}
		protected virtual object CreateCustomInstance(Type itemType) {
			return null;
		}
		protected virtual void ShowHelp() {
			ComponentDesigner designer = GetInstanceComponentDesigner();
			if(designer == null) return;
			DesignerVerb verb = DXSmartTagsHelper.CreateSupportVerb(designer);
			if(verb == null) return;
			verb.Invoke();
		}
		protected virtual void CancelChanges() { }
		protected virtual bool CanRemoveInstance(object value) {
			IComponent component = value as IComponent;
			if(component != null) {
				InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
				if(inheritanceAttribute != null && inheritanceAttribute.InheritanceLevel != InheritanceLevel.NotInherited)
					return false;
			}
			return true;
		}
		protected virtual bool CanSelectMultipleInstances() {
			return true;
		}
		protected virtual void DestroyInstance(object instance) {
			IComponent component = instance as IComponent;
			if(component == null) {
				IDisposable disposable = instance as IDisposable;
				if(disposable != null)
					disposable.Dispose();
				return;
			}
			IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			if(designerHost != null) {
				designerHost.DestroyComponent(component);
				return;
			}
			component.Dispose();
		}
		protected virtual DXCollectionEditorBaseForm CreateCollectionForm() {
			return new DXCollectionEditorBaseForm(this);
		}
		[Obsolete("Unused")]
		protected virtual IList GetObjectsFromInstance(object instance) {
			return new ArrayList { instance };
		}
		protected virtual object[] GetItems(object editValue) {
			if(editValue != null && editValue is ICollection) {
				ArrayList arrayList = new ArrayList();
				ICollection collection = (ICollection)editValue;
				foreach(object current in collection) {
					arrayList.Add(current);
				}
				object[] array = new object[arrayList.Count];
				arrayList.CopyTo(array, 0);
				return array;
			}
			return new object[0];
		}
		protected virtual object SetItems(object editValue, object[] value) {
			if(editValue != null && editValue is IList && value != null) {
				IList list = (IList)editValue;
				list.Clear();
				for(int i = 0; i < value.Length; i++) {
					list.Add(value[i]);
				}
			}
			return editValue;
		}
		protected virtual string GetDisplayText(object value) {
			return GetDisplayTextByFieldName(value, "Name");
		}
		protected virtual void OnCollectionItemChanged(PropertyItemChangedEventArgs e) {
			e.UpdateVisibleInfo = true;
		}
		protected virtual void OnCollectionChanged(CollectionChangedEventArgs e) {
			ApplyLifeUpdateChange(e);
		}
		protected virtual void OnCollectionChanging(CollectionChangingEventArgs e) { }
		protected virtual UISettings GetCollectionEditorUISettings() {
			return new UISettings
			{
				AllowReordering = true,
				AllowSearch = true,
				ColumnHeaders = new DXCollectionEditorBase.ColumnHeader[] { new ColumnHeader { Caption = "Name", FieldName = "Name" } },
				ShowPreviewControlBorder = true,
				PreviewControl = null
			};
		}
		protected virtual string GetDisplayTextByFieldName(object value, string fieldName) {
			if(value == null)
				return string.Empty;
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(value)[fieldName];
			string text = String.Empty;
			if(propertyDescriptor != null) {
				text = propertyDescriptor.GetValue(value).ToString();
				if(text != null && text.Length > 0)
					return text;
			}
			propertyDescriptor = TypeDescriptor.GetDefaultProperty(this.CollectionType);
			if(propertyDescriptor != null) {
				text = propertyDescriptor.GetValue(value).ToString();
				if(text != null && text.Length > 0)
					return text;
			}
			if(text == null || text.Length == 0)
				text = value.GetType().Name;
			return text;
		}
		protected virtual ItemTypeInfo[] CreateNewItemTypeInfos() {
			return null;
		}
		protected virtual bool AllowLiveUpdates {
			get { return false; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual bool StandardCollectionEditorRemoveBehavior {
			get { return false; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void StandardCollectionEditorLiveUpdateRemoving(CollectionChangedEventArgs e) {
			StandardCollectionEditorRemoving(e.Item);
		}
		protected void StandardCollectionEditorRemoving(object item) {
			object[] array = new object[this.collectionCore.Count];
			this.collectionCore.CopyTo(array, 0);
			this.collectionCore.Clear();
			for(int i = 0; i < array.Length; i++)
				if(!array[i].Equals(item))
					this.collectionCore.Add(array[i]);
			if(this.Context != null)
				this.Context.OnComponentChanged();
		}
		#endregion
		void PerformAdd(IComponent component, IDesignerHost host) {
			if(component == null) return;
			if(host != null && host.Container != null) {
				host.Container.Add(component);
				IComponentInitializer componentInitializer = host.GetDesigner(component) as IComponentInitializer;
				if(componentInitializer != null)
					componentInitializer.InitializeNewComponent(null);
			}
		}
		void SetStorePath(ITypeDescriptorContext context, DXCollectionEditorBase.DXCollectionEditorBaseForm xtraCollectionForm) {
			if(context != null && context.Instance != null && context.PropertyDescriptor != null) {
				xtraCollectionForm.RegistryStorePath = context.Instance.GetType().FullName + "\\" + context.PropertyDescriptor.DisplayName;
			}
		}
		ComponentDesigner GetInstanceComponentDesigner() {
			if(Context == null || Context.Instance == null) return null;
			return DesignTimeHelper.GetDesignerObject(Context.Instance as IComponent) as ComponentDesigner;
		}
		protected virtual void ApplyLifeUpdateChange(CollectionChangedEventArgs e) {
			if(AllowLiveUpdates && collectionCore != null) {
				IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				OnComponentChanging(designerHost, null);
				switch(e.Action) {
					case Utils.Design.Internal.CollectionAction.Add:
						if(!this.collectionCore.Contains(e.Item))
							this.collectionCore.Add(e.Item);
						break;
					case Utils.Design.Internal.CollectionAction.Remove:
						if(this.collectionCore.Contains(e.Item)) {
							if(StandardCollectionEditorRemoveBehavior)
								StandardCollectionEditorLiveUpdateRemoving(e);
							else
								this.collectionCore.RemoveAt(this.collectionCore.IndexOf(e.Item));
						}
						break;
					case DevExpress.Utils.Design.Internal.CollectionAction.Reorder:
						if(e.TargetItem != null) {
							int elementIndex = collectionCore.IndexOf(e.Item);
							int targetElementIndex = collectionCore.IndexOf(e.TargetItem);
							if(StandardCollectionEditorRemoveBehavior)
								StandardCollectionEditorRemoving(e.Item);
							else
								collectionCore.RemoveAt(elementIndex);
							collectionCore.Insert(targetElementIndex, e.Item);
						}
						break;
					default:
						break;
				}
			}
		}
		#endregion
		#region Editor Form
		protected class DXCollectionEditorBaseForm : DXCollectionEditorForm {
			#region ctor and dispose
			public DXCollectionEditorBaseForm(DXCollectionEditorBase collectionEditor)
				: base() {
				this.collectionEditor = collectionEditor;
				RegisterEventsHandler();
			}
			protected override void Dispose(bool disposing) {
				base.Dispose(disposing);
				UnregisterEventsHandler();
			}
			#endregion
			DXCollectionEditorBase collectionEditor;
			#region Private Methods
			void RegisterEventsHandler() {
				this.HelpButtonClicked += XtraCollectionEditorForm_HelpButtonClicked;
				this.HelpRequested += XtraCollectionEditorForm_HelpRequested;
				this.FormClosed += XtraCollectionEditorForm_FormClosed;
			}
			void UnregisterEventsHandler() {
				this.HelpButtonClicked -= XtraCollectionEditorForm_HelpButtonClicked;
				this.HelpRequested -= XtraCollectionEditorForm_HelpRequested;
				this.FormClosed -= XtraCollectionEditorForm_FormClosed;
			}
			#endregion
			#region Events Handler
			void XtraCollectionEditorForm_HelpButtonClicked(object sender, CancelEventArgs e) {
				e.Cancel = true;
				collectionEditor.ShowHelp();
			}
			void XtraCollectionEditorForm_HelpRequested(object sender, HelpEventArgs hlpevent) {
				collectionEditor.ShowHelp();
			}
			void XtraCollectionEditorForm_FormClosed(object sender, FormClosedEventArgs e) {
				switch(this.DialogResult) {
					case System.Windows.Forms.DialogResult.Cancel:
						collectionEditor.CancelChanges();
						break;
					case System.Windows.Forms.DialogResult.OK:
						break;
					default:
						return;
				}
			}
			#endregion
			#region Public Methods
			public new object EditValue {
				get { return base.EditValue; }
				set {
					base.EditValue = value;
					this.OnEditValueChanged();
				}
			}
			#endregion
			#region Protected Methods
			protected virtual void OnEditValueChanged() { }
			protected Type[] NewItemTypes {
				get { return this.collectionEditor.NewItemTypes; }
			}
			[Obsolete("Unused")]
			protected Type CollectionItemType {
				get { return this.collectionEditor.CollectionItemType; }
			}
			protected ITypeDescriptorContext Context {
				get { return this.collectionEditor.Context; }
			}
			[Obsolete("Unused")]
			protected Type CollectionType {
				get { return this.collectionEditor.CollectionType; }
			}
			protected object CreateInstance(Type itemType) {
				object obj = this.collectionEditor.CreateCustomInstance(itemType);
				if(obj == null)
					return this.collectionEditor.CreateInstance(itemType);
				this.collectionEditor.PerformAdd(obj as IComponent, (IDesignerHost)this.GetService(typeof(IDesignerHost)));
				return obj;
			}
			protected override object GetService(Type serviceType) {
				if(collectionEditor == null)
					return base.GetService(serviceType);
				return this.collectionEditor.GetService(serviceType);
			}
			protected internal virtual DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc) {
				return edSvc.ShowDialog(this);
			}
			[Obsolete("Unused")]
			protected object[] Items {
				get { return this.collectionEditor.GetItems(this.EditValue); }
				set {
					bool flag = false;
					try {
						flag = this.Context.OnComponentChanging();
					}
					catch(Exception ex) {
						if(IsCriticalException(ex))
							throw;
						this.DisplayError(ex);
					}
					if(flag) {
						object obj = this.collectionEditor.SetItems(this.EditValue, value);
						if(obj != this.EditValue)
							this.EditValue = obj;
						this.Context.OnComponentChanged();
					}
				}
			}
			[Obsolete("TODO")]
			protected virtual bool CanSelectMultipleInstances() {
				return this.collectionEditor.CanSelectMultipleInstances();
			}
			protected virtual void DisplayError(Exception e) {
				IUIService iUIService = (IUIService)this.GetService(typeof(IUIService));
				if(iUIService != null) {
					iUIService.ShowError(e);
					return;
				}
				string text = e.Message;
				if(text == null || text.Length == 0)
					text = e.ToString();
				MessageBox.Show(null, text, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
			}
			#region Inherited From XtraCollectionForm
			protected override ItemTypeInfo[] CreateNewItemTypes() {
				ItemTypeInfo[] typeInfos = this.collectionEditor.CreateNewItemTypeInfos();
				if(typeInfos == null) return ConvertFromTypesToItemTypeInfos(this.NewItemTypes);
				return typeInfos;
			}
			protected override object GetDefaultElement(Type type) {
				try {
					return CreateInstance(type);
				}
				catch(Exception e) {
					this.DisplayError(e);
				}
				return null;
			}
			protected override UISettings GetUISettings() {
				return collectionEditor.GetCollectionEditorUISettings();
			}
			protected sealed override void OnCollectionItemChanged(PropertyItemChangedEventArgs e) {
				collectionEditor.OnCollectionItemChanged(e);
			}
			protected override string GetCustomDisplayText(object value, string fieldName) {
				if(fieldName.Equals("Name"))
					return collectionEditor.GetDisplayText(value);
				return collectionEditor.GetDisplayTextByFieldName(value, fieldName);
			}
			protected override string RegistryStorePrefix {
				get { return @"Software\Developer Express\Designer\XtraCollectionEditor"; }
			}
			protected override IServiceProvider GetServiceProvider() {
				return this.collectionEditor.Context;
			}
			protected override ITypeDescriptorContext GetEditContext() {
				return this.collectionEditor.Context;
			}
			protected sealed override void OnCollectionChanged(CollectionChangedEventArgs e) {
				switch(e.Action) {
					case CollectionAction.Add:
						break;
					case CollectionAction.Remove:
						if(e.IsCreatedItem)
							this.collectionEditor.DestroyInstance(e.Item);
						break;
					case CollectionAction.Reorder:
						break;
					default:
						return;
				}
				RaiseCollectionChanged(e);
			}
			protected sealed override void OnCollectionChanging(CollectionChangingEventArgs e) {
				switch(e.Action) {
					case CollectionAction.Add:
						break;
					case CollectionAction.Remove:
						e.Cancel = !collectionEditor.CanRemoveInstance(e.Item);
						break;
					case CollectionAction.Reorder:
						break;
					default:
						return;
				}
				RaiseCollectionChanging(e);
			}
			protected override void OnApprovedCollectionChanged() {
				this.Context.OnComponentChanged();
			}
			protected virtual void RaiseCollectionChanging(CollectionChangingEventArgs e) {
				if(this.collectionEditor.AllowLiveUpdates)
					this.collectionEditor.OnCollectionChanging(e);
			}
			protected virtual void RaiseCollectionChanged(CollectionChangedEventArgs e) {
				if(this.collectionEditor.AllowLiveUpdates)
					this.collectionEditor.OnCollectionChanged(e);
			}
			#endregion
			#endregion
			#region Helper Methods
			bool IsCriticalException(Exception ex) {
				return ex is NullReferenceException || ex is StackOverflowException || ex is OutOfMemoryException || ex is ThreadAbortException || ex is IndexOutOfRangeException || ex is AccessViolationException;
			}
			ItemTypeInfo[] ConvertFromTypesToItemTypeInfos(Type[] types) {
				if(types == null) return null;
				ItemTypeInfo[] array = new ItemTypeInfo[types.Length];
				for(int i = 0; i < types.Length; i++) {
					array[i] = new ItemTypeInfo { Type = types[i], Image = null };
				}
				return array;
			}
			#endregion
		}
		#endregion
		public class UISettings {
			bool isShowPreviewControlBorder = true;
			public DXCollectionEditorBase.ColumnHeader[] ColumnHeaders { get; set; }
			public bool AllowReordering { get; set; }
			public IDXCollectionEditorPreviewControl PreviewControl { get; set; }
			public bool AllowSearch { get; set; }
			public bool ShowPreviewControlBorder {
				get { return isShowPreviewControlBorder; }
				set { isShowPreviewControlBorder = value; }
			}
		}
		public class ColumnHeader {
			public string Caption { get; set; }
			public string FieldName { get; set; }
		}
		public class ItemTypeInfo {
			public Type Type { get; set; }
			public Image Image { get; set; }
		}
	}
}
