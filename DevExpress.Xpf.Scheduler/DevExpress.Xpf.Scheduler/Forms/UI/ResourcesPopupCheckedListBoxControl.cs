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
using System.Linq;
using System.Windows;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.UI;
namespace DevExpress.Xpf.Scheduler.UI {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class ResourcesPopupCheckedListBoxControl : ComboBoxEdit, IResourceFilterControl {
		public static readonly DependencyProperty SchedulerControlProperty = DependencyProperty.Register("SchedulerControl", typeof(SchedulerControl), typeof(ResourcesPopupCheckedListBoxControl), new PropertyMetadata(null, OnSchedulerControlChanged));
		Locker processEditValueChangedLocker = new Locker();
		static void OnSchedulerControlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ResourcesPopupCheckedListBoxControl instance = o as ResourcesPopupCheckedListBoxControl;
			if (instance != null)
				instance.OnSchedulerControlChanged((SchedulerControl)e.OldValue, (SchedulerControl)e.NewValue);
		}
		static ResourcesPopupCheckedListBoxControl() {
			ResourcesPopupCheckedListBoxControlSettings.RegisterEditor();
		}
		public ResourcesPopupCheckedListBoxControl() {
			DefaultStyleKey = typeof(ResourcesPopupCheckedListBoxControl);
			Controller = new ResourceFilterController(this);
			EditValueChanged += OnEditValueChanged;
			Unloaded += OnUnloaded;
			Loaded += OnLoaded;
			IsActive = true;
		}
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		protected new ResourcesPopupCheckedListBoxControlSettings Settings {
			get { return (ResourcesPopupCheckedListBoxControlSettings)base.Settings; }
		}
		protected ResourceFilterController Controller { get; private set; }
		protected bool IsActive { get; private set; }
		public void ResetResourcesItems(ResourceBaseCollection resources) {
			if (!IsActive || SchedulerControl == null || Controller == null)
				return;
			Settings.ItemsSource = Controller.AvailableResources.Select(r => new NamedElement(r.Id, r.Caption)).ToList();
			UpdateSelectedItems();
		}
		public void ResourceVisibleChanged(ResourceBaseCollection resources) {
			UpdateSelectedItems();
		}
		protected virtual void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			if (!IsActive)
				return;
			Settings.SchedulerControl = newValue;
			Controller.InnerControlOwner = newValue;
		}
		protected void UpdateSelectedItems() {
			processEditValueChangedLocker.DoLockedActionIfNotLocked(UpdateSelectedItemsCore);
		}
		protected virtual void UpdateSelectedItemsCore() {
			SelectedItems.Clear();
			if (!IsActive || SchedulerControl == null || Controller == null)
				return;
			int count = ItemsProvider.Count;
			for (int i = 0; i < count; i++) {
				NamedElement item = ItemsProvider[i] as NamedElement;
				if (item != null) {
					Resource res = SchedulerControl.Storage.ResourceStorage.GetResourceById(item.Id);
					if (Controller.GetResourceVisible(res))
						SelectedItems.Add(item);
				}
			}
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			IsActive = false;
			Controller.Dispose();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			Controller = new ResourceFilterController(this);
			IsActive = true;
			Controller.InnerControlOwner = Settings.SchedulerControl;
		}
		void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			if (!IsActive || !IsLoaded || processEditValueChangedLocker.IsLocked)
				return;
			ProcessEditValue(e.NewValue);
		}
		void ProcessEditValue(object editValue) {
			List<object> value = editValue as List<object>;
			if (value == null) {
				Controller.SetAllResourcesVisible(false);
				return;
			}
			processEditValueChangedLocker.Lock();
			Controller.BeginUpdate();
			try {
				Controller.SetAllResourcesVisible(false);
				foreach (object id in value) {
					Resource res = SchedulerControl.Storage.ResourceStorage.GetResourceById(id);
					Controller.SetResourceVisible(res, true);
				}
			} finally {
				Controller.EndUpdate();
				processEditValueChangedLocker.Unlock();
			}
		}
	}
	public class ResourcesPopupCheckedListBoxControlSettings : ComboBoxEditSettings {
		public static readonly DependencyProperty SchedulerControlProperty = DependencyProperty.Register("SchedulerControl", typeof(SchedulerControl), typeof(ResourcesPopupCheckedListBoxControlSettings));
		static ResourcesPopupCheckedListBoxControlSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(ResourcesPopupCheckedListBoxControl), typeof(ResourcesPopupCheckedListBoxControlSettings), () => new ResourcesPopupCheckedListBoxControl(), () => new ResourcesPopupCheckedListBoxControlSettings());
		}
		public ResourcesPopupCheckedListBoxControlSettings() {
			StyleSettings = new CheckedComboBoxStyleSettings();
			DisplayMember = NamedElement.DisplayMember;
			ValueMember = NamedElement.ValueMember;
		}
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			ResourcesPopupCheckedListBoxControl editor = edit as ResourcesPopupCheckedListBoxControl;
			if (editor != null)
				SetValueFromSettings(SchedulerControlProperty, () => editor.SchedulerControl = SchedulerControl);
		}
	}
}
