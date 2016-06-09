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
using DevExpress.Xpf.Editors.Settings;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Utils;
#if WPF
using PlatformIndependentPropertyChangedCallback = System.Windows.PropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
using DevExpress.XtraScheduler.Native;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using PlatformIndependentPropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PlatformIndependentDependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Scheduler.UI {
	#region StorageBoundComboBoxEdit
	[DXToolboxBrowsable(false)]
	public abstract class StorageBoundComboBoxEdit : ComboBoxEdit {
		protected StorageBoundComboBoxEdit() {
			IsTextEditable = false;
		}
		#region Storage
		public SchedulerStorage Storage {
			get { return (SchedulerStorage)GetValue(StorageProperty); }
			set { SetValue(StorageProperty, value); }
		}
		public static readonly DependencyProperty StorageProperty = CreateStorageProperty();
		static DependencyProperty CreateStorageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<StorageBoundComboBoxEdit, SchedulerStorage>("Storage", null, (d, e) => d.OnStorageChanged(e.OldValue, e.NewValue));
		}
		protected internal virtual void OnStorageChanged(SchedulerStorage oldValue, SchedulerStorage newValue) {
			UnsubscribeEditorEvents();
			if (newValue != null) {
				SetupInnerSettingsBindings(InnerSettings);
			} else
				ClearInnerSettingsBindings();
			SubscribeEditorEvents();
		}
		#endregion
		StorageBoundComboBoxControlSettings InnerSettings { get { return Settings as StorageBoundComboBoxControlSettings; } }
		protected override BaseEditSettings CreateEditorSettings() {
			return CreateStorageBoundEditorSettings();
		}
		protected abstract StorageBoundComboBoxControlSettings CreateStorageBoundEditorSettings();
		protected virtual void SetupInnerSettingsBindings(BaseEditSettings innerSettings) {
			InnerSettings.SetupControlBindings(this);
		}
		private void ClearInnerSettingsBindings() {
			ClearValue(StorageProperty);
		}
		protected internal virtual void SubscribeEditorEvents() {
			EditValueChanged += new EditValueChangedEventHandler(HandleEditValueChanged);
		}
		protected internal virtual void UnsubscribeEditorEvents() {
			EditValueChanged -= new EditValueChangedEventHandler(HandleEditValueChanged);
		}
		protected internal virtual void OnBeforeStorageDispose(object sender, EventArgs e) {
			Storage = null;
		}
		protected virtual void HandleEditValueChanged(object sender, EditValueChangedEventArgs e) { 
		}
	}
	#endregion
	#region StorageBoundComboBoxControlSettings
	public abstract class StorageBoundComboBoxControlSettings : ComboBoxEditSettings {
		#region Storage
		public SchedulerStorage Storage {
			get { return (SchedulerStorage)GetValue(StorageProperty); }
			set { SetValue(StorageProperty, value); }
		}
		public static readonly DependencyProperty StorageProperty = CreateStorageProperty();
		static DependencyProperty CreateStorageProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<StorageBoundComboBoxControlSettings, SchedulerStorage>("Storage", null, (d, e) => d.OnStorageChanged(e.OldValue, e.NewValue));
		}
		protected internal virtual void OnStorageChanged(SchedulerStorage oldValue, SchedulerStorage newValue) {
			PopulateItems();
		}
		#endregion
		protected internal abstract void PopulateItems();
		protected virtual internal void SetupControlBindings(StorageBoundComboBoxEdit sourceControl) {
			SetBinding(StorageProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(sourceControl, "Storage"));
		}
	}
	#endregion
	#region UserInterfaceObjectEdit
	public abstract class UserInterfaceObjectEdit<T> : StorageBoundComboBoxEdit where T : IUserInterfaceObject {
	}
	#endregion
	#region UserInterfaceObjectEditSettings
	public abstract class UserInterfaceObjectEditSettings<T> : StorageBoundComboBoxControlSettings where T : IUserInterfaceObject {
		protected internal abstract UserInterfaceObjectCollection<T> GetItemSourceCollection();
		protected internal override void PopulateItems() {
			Items.Clear();
			if (Storage == null)
				return;
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			ItemsSource = GetItemSourceCollection();
		}
	}
	#endregion
	#region SchedulerBoundComboBoxEdit
	[DXToolboxBrowsable(false)]
	public abstract class SchedulerBoundComboBoxEdit : ComboBoxEdit, IBatchUpdateHandler {
		readonly BatchUpdateHelper batchUpdateHelper;
		protected SchedulerBoundComboBoxEdit() {
			IsTextEditable = false;
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region SchedulerControl
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerControlProperty = CreateSchedulerControlProperty();
		static DependencyProperty CreateSchedulerControlProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerBoundComboBoxEdit, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue));
		}
		protected internal virtual void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			BeginUpdate();
			try {
				OnSchedulerControlChangedCore();
				SynchronizeToInnerSettings(newValue);
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		SchedulerBoundComboBoxEditSettings InnerSettings { get { return Settings as SchedulerBoundComboBoxEditSettings; } }
		protected internal abstract void UpdateSelectedItems();
		protected override BaseEditSettings CreateEditorSettings() {
			SchedulerBoundComboBoxEditSettings settings = CreateSchedulerBasedEditorSettings();
			settings.OwnerSchedulerEdit = this;
			return settings;
		}
		protected abstract SchedulerBoundComboBoxEditSettings CreateSchedulerBasedEditorSettings();
		protected virtual void SynchronizeToInnerSettings(SchedulerControl control) {
			if (control != null) {
				SetupInnerSettingsBindings(InnerSettings);
			} else
				ClearInnerSettingsBindings();
		}
		protected internal virtual void OnSchedulerControlChangedCore() {
		}
		protected virtual void SetupInnerSettingsBindings(BaseEditSettings innerSettings) {
			InnerSettings.SetupControlBindings(this);
		}
		private void ClearInnerSettingsBindings() {
			ClearValue(SchedulerControlProperty);
		}
		protected internal virtual void SubscribeEditorEvents() {
		}
		protected internal virtual void UnsubscribeEditorEvents() {
		}
		public virtual void BeginUpdate() {
			this.batchUpdateHelper.BeginUpdate();
		}
		public virtual void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
		}
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			UnsubscribeEditorEvents();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			SubscribeEditorEvents();
		}
		#endregion
	}
	#endregion
	#region SchedulerBoundComboBoxEditSettings
	public abstract class SchedulerBoundComboBoxEditSettings : ComboBoxEditSettings {
		SchedulerBoundComboBoxEdit ownerSchedulerEdit;
		protected internal SchedulerBoundComboBoxEdit OwnerSchedulerEdit {
			get {
				return ownerSchedulerEdit;
			}
			set {
				if (ownerSchedulerEdit == value)
					return;
				ownerSchedulerEdit = value;
			}
		}
		#region SchedulerControlProperty
		public static readonly DependencyProperty SchedulerControlProperty = DependencyPropertyManager.Register("SchedulerControl", typeof(SchedulerControl), typeof(SchedulerBoundComboBoxEditSettings), new FrameworkPropertyMetadata(null, new PlatformIndependentPropertyChangedCallback(OnSchedulerControlChanged)));
		protected static void OnSchedulerControlChanged(DependencyObject d, PlatformIndependentDependencyPropertyChangedEventArgs e) {
			SchedulerBoundComboBoxEditSettings instance = d as SchedulerBoundComboBoxEditSettings;
			if (instance != null)
				instance.OnSchedulerControlChanged((SchedulerControl)e.OldValue, (SchedulerControl)e.NewValue);
		}
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		#endregion
		protected internal virtual void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			PopulateItems();
			if (OwnerSchedulerEdit != null) 
				OwnerSchedulerEdit.UpdateSelectedItems();
		}
		protected internal abstract void PopulateItems();
		protected virtual internal void SetupControlBindings(SchedulerBoundComboBoxEdit sourceControl) {
			SetBinding(SchedulerControlProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(sourceControl, "SchedulerControl"));
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			SchedulerBoundComboBoxEdit schedulerBoundComboBoxEdit = edit as SchedulerBoundComboBoxEdit;
			if(schedulerBoundComboBoxEdit == null)
				return;
			schedulerBoundComboBoxEdit.SchedulerControl = SchedulerControl;
		}
	}
	#endregion
}
