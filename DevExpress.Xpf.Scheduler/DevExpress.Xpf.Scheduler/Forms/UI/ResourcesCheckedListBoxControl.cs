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
using DevExpress.Xpf.Editors.Settings;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.XtraScheduler.UI;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Scheduler.Drawing;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Scheduler.UI {
	#region ResourcesCheckedListBoxControl
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class ResourcesCheckedListBoxControl : ListBoxEdit, IResourceFilterControl, IBatchUpdateable, IBatchUpdateHandler {
		ResourceFilterController controller;
		BatchUpdateHelper batchUpdateHelper;
		static ResourcesCheckedListBoxControl() {
			ResourcesCheckedListBoxControlSettings.RegisterEditor();
		}
		public ResourcesCheckedListBoxControl() {
			DefaultStyleKey = typeof(ResourcesCheckedListBoxControl);
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.controller = new ResourceFilterController(this);
			StyleSettings = new CheckedListBoxEditStyleSettings();
			DisplayMember = NamedElement.DisplayMember;
			ValueMember = NamedElement.ValueMember;
		}
		#region Properties
		#region SchedulerControl
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourcesCheckedListBoxControlSchedulerControl")]
#endif
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerControlProperty = CreateSchedulerControlProperty();
		static DependencyProperty CreateSchedulerControlProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ResourcesCheckedListBoxControl, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue));
		}
		protected internal virtual void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			BeginUpdate();
			try {
				if(newValue != null) 
					SetupInnerSettingsBindings(InnerSettings);
				else
					ClearInnerSettingsBindings();
				Controller.InnerControlOwner = newValue;
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		protected internal ResourceFilterController Controller { get { return controller; } }
		protected internal ResourcesCheckedListBoxControlSettings InnerSettings { get { return Settings as ResourcesCheckedListBoxControlSettings; } }
		#endregion
		protected override  BaseEditSettings CreateEditorSettings() {
			return CreateResourcesCheckedListBoxControlSettings();
		}
		protected virtual ResourcesCheckedListBoxControlSettings CreateResourcesCheckedListBoxControlSettings() {
			return new ResourcesCheckedListBoxControlSettings();
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
			PopulateResourceItems(resources);
		}
		protected internal virtual void ResourceVisibleChangedCore(ResourceBaseCollection resources) {
			BeginInit();
			try {
				PopulateResourceItems(resources);
			} finally {
				EndInit();
			}
		}
		protected virtual void PopulateResourceItems(ResourceBaseCollection resources) {
			BeginUpdate();
			try {
				InnerSettings.PopulateItems();
				UpdateSelectedItems(resources);
			}
			finally {
			   EndUpdate();
			}
		}
		protected virtual void UpdateSelectedItems(ResourceBaseCollection resources) {
			SelectedItems.Clear();
			if (SchedulerControl == null)
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
		protected virtual void SetupInnerSettingsBindings(BaseEditSettings innerSettings) {
			InnerSettings.SetupControlBindings(this);
		}
		private void ClearInnerSettingsBindings() {
			ClearValue(SchedulerControlProperty);
		}
		protected internal virtual void SubscribeEditorEvents() {
			this.EditValueChanged += new EditValueChangedEventHandler(HandleEditValueChanged);
		}
		protected internal virtual void UnsubscribeEditorEvents() {
			this.EditValueChanged -= new EditValueChangedEventHandler(HandleEditValueChanged);
		}
		protected virtual void HandleEditValueChanged(object sender, EditValueChangedEventArgs e) {
			BeginUpdate();
			try {
				UpdateResourceVisible(e.NewValue);
				e.Handled = true;
			} finally {
				EndUpdate();
			}
		}
		protected virtual void UpdateResourceVisible(object item) {
			Controller.SetAllResourcesVisible(false);
			if (item == null)
				return;
			Controller.BeginUpdate();
			try {
				List<object> visibleResources = item as List<object>;
				int count = visibleResources.Count;
				for (int i = 0; i < count; i++) {
					Resource resource = visibleResources[i] as Resource;
					if (resource != null)
						Controller.SetResourceVisible(resource, true);
				}
			} finally {
				Controller.EndUpdate();
			}
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new SchedulerListBoxEditStrategy(this);
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			this.batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("ResourcesCheckedListBoxControlIsUpdateLocked")]
#endif
public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
			OnBeginUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			OnEndUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			OnCancelUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		protected virtual void OnFirstBeginUpdate() {
			UnsubscribeEditorEvents();
		}
		protected virtual void OnBeginUpdate() {
		}
		protected virtual void OnEndUpdate() {
		}
		protected virtual void OnLastEndUpdate() {
			SubscribeEditorEvents();
		}
		protected virtual void OnCancelUpdate() {
		}
		protected virtual void OnLastCancelUpdate() {
			SubscribeEditorEvents();
		}
		#endregion
	}
	#endregion
	#region ResourcesCheckedListBoxControlSetting
	public class ResourcesCheckedListBoxControlSettings : ListBoxEditSettings {
		static ResourcesCheckedListBoxControlSettings() {
			RegisterEditor();
		}
		public ResourcesCheckedListBoxControlSettings() {
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(ResourcesCheckedListBoxControl), typeof(ResourcesCheckedListBoxControlSettings), delegate() { return new ResourcesCheckedListBoxControl(); }, delegate() { return new ResourcesCheckedListBoxControlSettings(); });
		}
		#region SchedulerControlProperty
		public static readonly DependencyProperty SchedulerControlProperty = DependencyPropertyManager.Register("SchedulerControl", typeof(SchedulerControl), typeof(ResourcesCheckedListBoxControlSettings), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSchedulerControlChanged)));
		protected static void OnSchedulerControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ResourcesCheckedListBoxControlSettings instance = d as ResourcesCheckedListBoxControlSettings;
			if (instance != null)
				instance.OnSchedulerControlChanged((SchedulerControl)e.OldValue, (SchedulerControl)e.NewValue);
		}
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		protected internal virtual void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			PopulateItems();
		}
		#endregion
		protected virtual internal void SetupControlBindings(ResourcesCheckedListBoxControl sourceControl) {
			SetBinding(SchedulerControlProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(sourceControl, "SchedulerControl"));
		}
		protected internal virtual void PopulateItems() {
			Items.Clear();
			if (SchedulerControl == null)
				return;
			ResourceCollection resources = SchedulerControl.Storage.ResourceStorage.Items;
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				Items.Add(new NamedElement(resource, resource.Caption));
			}
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Native {
	public class SchedulerListBoxEditStrategy : ListBoxEditStrategy {
		EditorListBox ListBox { get { return (EditorListBox)Editor.EditCore; } }
		public SchedulerListBoxEditStrategy(ListBoxEdit editor)
			: base(editor) {
		}	   
	}
}
