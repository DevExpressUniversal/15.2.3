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
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.XtraScheduler.UI;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Scheduler.Drawing;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler.UI {
	#region AppointmentResourcesEdit
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class AppointmentResourcesEdit : CheckedResourcesComboBoxControlBase {
		NotificationCollectionChangedListener<object> listener;
		protected internal new AppointmentResourcesEditResourceFilterController Controller { get { return (AppointmentResourcesEditResourceFilterController)base.Controller; } }
		protected internal new AppointmentResourcesEditSettings InnerSettings { get { return Settings as AppointmentResourcesEditSettings; } }
		#region ResourceIds
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentResourcesEditResourceIds")]
#endif
public AppointmentResourceIdCollection ResourceIds {
			get { return (AppointmentResourceIdCollection)GetValue(ResourceIdsProperty); }
			set { SetValue(ResourceIdsProperty, value); }
		}
		public static readonly DependencyProperty ResourceIdsProperty = CreateAppointmentResourceIdsProperty();
		static DependencyProperty CreateAppointmentResourceIdsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentResourcesEdit, AppointmentResourceIdCollection>("ResourceIds", null, (d, e) => d.OnResourceIdsChanged(e.OldValue, e.NewValue));
		}
		void OnResourceIdsChanged(AppointmentResourceIdCollection oldValue, AppointmentResourceIdCollection newValue) {
			BeginUpdate();
			try {
				if (listener != null)
					UnsubscribeResourceIdsEvents();
				if (newValue != null) {
					Controller.ResourceIds = newValue;
					this.listener = new NotificationCollectionChangedListener<object>(ResourceIds);
					UpdateSelectedItems();
					SubscribeResourceIdsEvents();
				}
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		protected override ResourceFilterController CreateFilterController() {
			return new AppointmentResourcesEditResourceFilterController(this);
		}
		protected override SchedulerBoundComboBoxEditSettings CreateSchedulerBasedEditorSettings() {
			return new AppointmentResourcesEditSettings();
		}
		protected internal override void UpdateSelectedItems() {
			if (SchedulerControl == null)
				return;
			if (ResourceIds == null)
				return;
			SelectedItems.Clear();
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
		protected internal override void SubscribeEditorEvents() {
			base.SubscribeEditorEvents();
			SubscribeResourceIdsEvents();
		}
		protected internal override void UnsubscribeEditorEvents() {
			base.UnsubscribeEditorEvents();
			UnsubscribeResourceIdsEvents();
		}
		protected internal virtual void SubscribeResourceIdsEvents() {
			if (listener == null)
				return;
			listener.Changed += new EventHandler(OnResourceIdsChanged);
		}
		protected internal virtual void UnsubscribeResourceIdsEvents() {
			if (listener == null)
				return;
			listener.Changed -= new EventHandler(OnResourceIdsChanged);
		}
		protected internal virtual void OnResourceIdsChanged(object sender, EventArgs e) {
			PopulateResourceItems();
		}
		protected override BaseEditStyleSettings CreateStyleSettings() {
			return new CheckedComboBoxStyleSettings();
		}
	}
	#endregion
	#region AppointmentResourcesEditSettings
	public class AppointmentResourcesEditSettings : ResourcesComboBoxControlBaseSettings {
		static AppointmentResourcesEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(AppointmentResourcesEdit), typeof(AppointmentResourcesEditSettings), delegate() { return new AppointmentResourcesEdit(); }, delegate() { return new AppointmentResourcesEditSettings(); });
		}
		public AppointmentResourcesEditSettings() {
			StyleSettings = new CheckedComboBoxStyleSettings();
		}
	}
	#endregion
}
