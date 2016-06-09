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
using System.Windows;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Core;
using System.Collections;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.UI {
	#region ResourcesComboBoxControlBase
	public abstract class ResourcesComboBoxControlBase : SchedulerBoundComboBoxEdit, IResourceFilterControl {
		ResourceFilterController controller;
		protected ResourcesComboBoxControlBase() {
			DisplayMember = NamedElement.DisplayMember;
			ValueMember = NamedElement.ValueMember;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			this.controller = CreateFilterController();
		}
		protected internal new bool IsLoaded { get; set; }
		protected internal ResourceFilterController Controller { get { return controller; } }
		protected internal ResourcesComboBoxControlBaseSettings InnerSettings { get { return Settings as ResourcesComboBoxControlBaseSettings; } }
		protected abstract ResourceFilterController CreateFilterController();
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if (this.controller == null) {
				this.controller = CreateFilterController();
				OnSchedulerControlChangedCore();
			}
			this.controller.ReloadAvailableResources();
			IsLoaded = true;
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			if (!IsLoaded)
				return;
			UnsubscribeEditorEvents();
			if (this.controller != null) {
				this.controller.Dispose();
				this.controller = null;
			}
			IsLoaded = false;
		}
		protected internal override void UnsubscribeEditorEvents() {
			EditValueChanged -= new EditValueChangedEventHandler(HandleEditValueChanged);
		}
		protected internal override void SubscribeEditorEvents() {
			EditValueChanged += new EditValueChangedEventHandler(HandleEditValueChanged);
		}
		protected virtual void HandleEditValueChanged(object sender, EditValueChangedEventArgs e) {
			BeginUpdate();
			try {
				UpdateResourceVisible(e.NewValue);
				InnerSettings.UpdateVisibleResources();
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void UpdateResourceVisible(object item) {
			if (item == null || Controller == null)
				return;
			Controller.BeginUpdate();
			try {
				bool allResourcesVisible = Object.ReferenceEquals(item, ResourcesComboBoxControlSettings.AllItemObject);
				Controller.SetAllResourcesVisible(allResourcesVisible);
				if (!allResourcesVisible) {
					Resource resource = item as Resource;
					if (resource != null)
						Controller.SetResourceVisible(resource, true);
				}
			}
			finally {
				Controller.EndUpdate();
			}
		}
		#region IResourceFilterControl Members
		void IResourceFilterControl.ResetResourcesItems(ResourceBaseCollection resources) {
			ResetResourcesItemsCore(resources);
		}
		void IResourceFilterControl.ResourceVisibleChanged(ResourceBaseCollection resources) {
			ResourceVisibleChangedCore(resources);
		}
		#endregion
		protected internal virtual void ResetResourcesItemsCore(ResourceBaseCollection resources) {
			PopulateResourceItems();
		}
		protected internal virtual void ResourceVisibleChangedCore(ResourceBaseCollection resources) {
			PopulateResourceItems();
		}
		protected internal virtual void PopulateResourceItems() {
			BeginUpdate();
			try {
				InnerSettings.PopulateItems();
				UpdateSelectedItemsCore();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void UpdateSelectedItems() {
			BeginUpdate();
			try {
				UpdateSelectedItemsCore();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void UpdateSelectedItemsCore() {
			SelectedItems.Clear();
			if (SchedulerControl == null || Controller == null)
				return;
			int count = ItemsProvider.GetCount(ItemsProvider.CurrentDataViewHandle);
			for (int i = 0; i < count; i++) {
				NamedElement item = ItemsProvider.GetItemByControllerIndex(i, ItemsProvider.CurrentDataViewHandle) as NamedElement;
				if (item != null) {
					Resource resource = item.Id as Resource;
					if (resource != null && Controller.GetResourceVisible(resource))
						SelectedItems.Add(item);
				}
			}
		}
		protected internal virtual void SetNullEditValue(string nullValueText) {
			NullText = nullValueText;
			EditValue = null;
		}
		protected internal override void OnSchedulerControlChangedCore() {
			Controller.InnerControlOwner = SchedulerControl;
		}
	}
	#endregion
	#region ResourcesComboBoxControlBaseSettings
	public abstract class ResourcesComboBoxControlBaseSettings : SchedulerBoundComboBoxEditSettings {
		List<Resource> visibleResources = new List<Resource>();
		protected ResourcesComboBoxControlBaseSettings() {
			NullText = SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone);
			NullValue = null;
			DisplayMember = NamedElement.DisplayMember;
			ValueMember = NamedElement.ValueMember;
		}
		public List<Resource> VisibleResources { get { return visibleResources; } }
		protected internal override void PopulateItems() {
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			if (SchedulerControl == null || SchedulerControl.Storage == null || OwnerSchedulerEdit == null)
				return;
			NamedElementList itemSource = new NamedElementList();
			PopulateExtendedItems(itemSource);
			PopulateItemsCore(itemSource, GetResources());
			UpdateVisibleResources();
			ItemsSource = itemSource;
		}
		ResourceBaseCollection GetResources() {
			return ((IInternalSchedulerStorageBase)SchedulerControl.Storage.InnerStorage).GetFilteredResources(true);
		}
		protected internal virtual void PopulateExtendedItems(NamedElementList items) {
		}
		protected internal virtual void PopulateItemsCore(NamedElementList items, ResourceBaseCollection resources) {
			int count = resources.Count;
			for (int i = 0; i < count; i++)
				items.Add(CreateItem(resources[i], i));
		}
		protected internal virtual void UpdateVisibleResources(Resource visibleResource) {
			VisibleResources.Clear();
			VisibleResources.Add(visibleResource);
		}
		protected internal virtual void UpdateVisibleResources() {
			VisibleResources.Clear();
			ResourceBaseCollection allResources = GetResources();
			ResourceBaseCollection resources = allResources.FindAll((resouce) => { return resouce.Visible; });
			VisibleResources.AddRange(resources);
		}
		protected virtual NamedElement CreateItem(Resource resource, int resourceIndex) {
			return new NamedElement(resource, resource.Caption);
		}
		protected internal virtual NamedElement FindItemById(object id) {
			int count = ItemsProvider.GetCount(ItemsProvider.CurrentDataViewHandle);
			for (int i = 0; i < count; i++) {
				NamedElement item = ItemsProvider.GetItemByControllerIndex(i, ItemsProvider.CurrentDataViewHandle) as NamedElement;
				if (item != null && Object.ReferenceEquals(id, item.Id))
					return item;
			}
			return null;
		}
		protected internal bool IsSeveralResourcesVisible() {
			if (SchedulerControl == null || SchedulerControl.Storage == null)
				return false;
			int visibleResourceCount = VisibleResources.Count;
			if (visibleResourceCount <= 1)
				return false;
			if (visibleResourceCount == SchedulerControl.Storage.ResourceStorage.Count)
				return false;
			return true;
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			SchedulerBoundComboBoxEdit schedulerComboBoxEdit = edit as SchedulerBoundComboBoxEdit;
			if (OwnerSchedulerEdit == null && schedulerComboBoxEdit != null) {
				OwnerSchedulerEdit = schedulerComboBoxEdit;
				PopulateItems();
			}
		}
	}
	#endregion
	#region CheckedResourcesComboBoxControlBase
	public abstract class CheckedResourcesComboBoxControlBase : ResourcesComboBoxControlBase {
		protected CheckedResourcesComboBoxControlBase() {
		}
		protected override void UpdateResourceVisible(object item) {
			if (item == null)
				return;
			Controller.BeginUpdate();
			try {
				Controller.SetAllResourcesVisible(false);
				IEnumerable visibleResources = item as IEnumerable;
				foreach (object resourceObject in visibleResources) {
					Resource resource = resourceObject as Resource;
					if (resource != null)
						Controller.SetResourceVisible(resource, true);
				}
			}
			finally {
				Controller.EndUpdate();
			}
		}
		protected override string GetCustomDisplayText(object editValue, string displayText) {
			if(SelectedItems.Count == 0 && EditValue != null)
				return  SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone);
			return displayText;
		}
	}
	#endregion
	#region ResourcesComboBoxControl
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class ResourcesComboBoxControl : ResourcesComboBoxControlBase, IResourceFilterControl {
		const bool defaultShowAllResourcesItem = true;
		const bool defaultShowNoneResourcesItem = true;
		static ResourcesComboBoxControl() {
			ResourcesComboBoxControlSettings.RegisterEditor();
		}
		public ResourcesComboBoxControl() {
			DefaultStyleKey = typeof(ResourcesComboBoxControl);
		}
		protected override ResourceFilterController CreateFilterController() {
			return new ResourceFilterController(this);
		}
		protected internal new ResourcesComboBoxControlSettings InnerSettings { get { return Settings as ResourcesComboBoxControlSettings; } }
		#region ShowAllResourcesItem
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourcesComboBoxControlShowAllResourcesItem")]
#endif
		public bool ShowAllResourcesItem {
			get { return (bool)GetValue(ShowAllResourcesItemProperty); }
			set { SetValue(ShowAllResourcesItemProperty, value); }
		}
		public static readonly DependencyProperty ShowAllResourcesItemProperty = CreateShowAllResourcesItemProperty();
		static DependencyProperty CreateShowAllResourcesItemProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourcesComboBoxControl, bool>("ShowAllResourcesItem", defaultShowAllResourcesItem, (d, e) => d.OnShowAllResourcesItemChanged(e.OldValue, e.NewValue));
		}
		protected void OnShowAllResourcesItemChanged(bool oldValue, bool newValue) {
		}
		#endregion
		#region ShowNoneResourcesItem
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourcesComboBoxControlShowNoneResourcesItem")]
#endif
		public bool ShowNoneResourcesItem {
			get { return (bool)GetValue(ShowNoneResourcesItemProperty); }
			set { SetValue(ShowNoneResourcesItemProperty, value); }
		}
		public static readonly DependencyProperty ShowNoneResourcesItemProperty = CreateShowNoneResourcesItemProperty();
		static DependencyProperty CreateShowNoneResourcesItemProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourcesComboBoxControl, bool>("ShowNoneResourcesItem", defaultShowNoneResourcesItem, (d, e) => d.OnShowNoneResourcesItemChanged(e.OldValue, e.NewValue));
		}
		protected void OnShowNoneResourcesItemChanged(bool oldValue, bool newValue) {
		}
		#endregion
		protected override SchedulerBoundComboBoxEditSettings CreateSchedulerBasedEditorSettings() {
			return new ResourcesComboBoxControlSettings();
		}
		protected internal override void UpdateSelectedItemsCore() {
			int availableResourceCount = (SchedulerControl != null && SchedulerControl.Storage != null) ? SchedulerControl.Storage.ResourceStorage.Count : 0;
			int visibleResourceCount = InnerSettings.VisibleResources.Count;
			if (visibleResourceCount == 1) {
				EditValue = InnerSettings.VisibleResources[0];
			}
			else if (visibleResourceCount == 0) {
				if (ShowNoneResourcesItem)
					SelectedItem = InnerSettings.FindItemById(ResourcesComboBoxControlSettings.NoneItemObject);
				else
					SetNullEditValue(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone));
			}
			else if (visibleResourceCount == availableResourceCount) {
				if (ShowAllResourcesItem)
					SelectedItem = InnerSettings.FindItemById(ResourcesComboBoxControlSettings.AllItemObject);
				else
					SetNullEditValue(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceAll));
			}
			else {
				SetNullEditValue(null);
			}
		}
		protected override string GetCustomDisplayText(object editValue, string displayText) {
			if (InnerSettings.IsSeveralResourcesVisible()) {
				string text = string.Empty;
				for (int i = 0; i < InnerSettings.VisibleResources.Count; i++) {
					Resource resource = InnerSettings.VisibleResources[i];
					text += i > 0 ? SeparatorString : String.Empty;
					text += resource.Caption;
				}
				return text;
			}
			return displayText;
		}
	}
	#endregion
	#region ResourcesComboBoxControlSettings
	public class ResourcesComboBoxControlSettings : ResourcesComboBoxControlBaseSettings {
		internal static readonly object noneItemObject = new Object();
		internal static readonly object allItemObject = new Object();
		public static object NoneItemObject { get { return noneItemObject; } }
		public static object AllItemObject { get { return allItemObject; } } 
		static ResourcesComboBoxControlSettings() {
			RegisterEditor();
		}
		public ResourcesComboBoxControlSettings() {
		}
		#region ShowAllResourcesItem
		public bool ShowAllResourcesItem {
			get { return (bool)GetValue(ShowAllResourcesItemProperty); }
			set { SetValue(ShowAllResourcesItemProperty, value); }
		}
		public static readonly DependencyProperty ShowAllResourcesItemProperty = CreateShowAllResourcesItemProperty();
		static DependencyProperty CreateShowAllResourcesItemProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourcesComboBoxControlSettings, bool>("ShowAllResourcesItem", true, (d, e) => d.OnShowAllResourcesItemChanged(e.OldValue, e.NewValue));
		}
		protected void OnShowAllResourcesItemChanged(bool oldValue, bool newValue) {
			PopulateItems();
		}
		#endregion
		#region ShowNoneResourcesItem
		public bool ShowNoneResourcesItem {
			get { return (bool)GetValue(ShowNoneResourcesItemProperty); }
			set { SetValue(ShowNoneResourcesItemProperty, value); }
		}
		public static readonly DependencyProperty ShowNoneResourcesItemProperty = CreateShowNoneResourcesItemProperty();
		static DependencyProperty CreateShowNoneResourcesItemProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourcesComboBoxControlSettings, bool>("ShowNoneResourcesItem", true, (d, e) => d.OnShowNoneResourcesItemChanged(e.OldValue, e.NewValue));
		}
		protected void OnShowNoneResourcesItemChanged(bool oldValue, bool newValue) {
			PopulateItems();
		}
		#endregion
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(ResourcesComboBoxControl), typeof(ResourcesComboBoxControlSettings), delegate() { return new ResourcesComboBoxControl(); }, delegate() { return new ResourcesComboBoxControlSettings(); });
		}
		protected internal override void SetupControlBindings(SchedulerBoundComboBoxEdit sourceControl) {
			base.SetupControlBindings(sourceControl);
			SetBinding(ShowAllResourcesItemProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(sourceControl, "ShowAllResourcesItem"));
			SetBinding(ShowNoneResourcesItemProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(sourceControl, "ShowNoneResourcesItem"));
		}
		protected internal override void PopulateExtendedItems(NamedElementList items) {
			if (ShowAllResourcesItem)
				items.Add(new NamedElement(AllItemObject, SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceAll)));
			if (ShowNoneResourcesItem)
				items.Add(new NamedElement(NoneItemObject, SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceNone)));
		}
	}
	#endregion
}
