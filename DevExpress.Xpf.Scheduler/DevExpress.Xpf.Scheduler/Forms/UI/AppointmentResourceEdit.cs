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
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler.Drawing;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.UI {
	#region AppointmentResourceEdit
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfScheduling)]
	public class AppointmentResourceEdit : SchedulerBoundComboBoxEdit {
		SchedulerColorSchemaCollection resourceColors;
		NotificationCollectionChangedListener<SchedulerColorSchema> resourceColorsListener;
		static AppointmentResourceEdit() {
			AppointmentResourceEditSettings.RegisterEditor();
		}
		public AppointmentResourceEdit() {
			DefaultStyleKey = typeof(AppointmentResourceEdit);
			ValueMember = NamedElement.ValueMember;
			this.resourceColors = new SchedulerColorSchemaCollection();
			this.resourceColors.LoadDefaults();
			SubscribeResourceColorsEvents();
		}
		#region ShowEmptyResource
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("AppointmentResourceEditShowEmptyResource")]
#endif
		public bool ShowEmptyResource {
			get { return (bool)GetValue(ShowEmptyResourceProperty); }
			set { SetValue(ShowEmptyResourceProperty, value); }
		}
		public static readonly DependencyProperty ShowEmptyResourceProperty = CreateShowEmptyResourceProperty();
		static DependencyProperty CreateShowEmptyResourceProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentResourceEdit, bool>("ShowEmptyResource", true, (d, e) => d.OnShowEmptyResourceChanged(e.OldValue, e.NewValue));
		}
		protected void OnShowEmptyResourceChanged(bool oldValue, bool newValue) {
		}
		#endregion
		protected internal override void UpdateSelectedItems() {
		}
		protected internal AppointmentResourceEditSettings InnerSettings { get { return Settings as AppointmentResourceEditSettings; } }
		protected internal virtual Color CalculateResourceItemColor(Resource resource, int resourceIndex) {
			if (resource.GetColor() != ColorExtension.Empty)
				return resource.GetColor();
			else {
				if (resourceIndex < 0)
					resourceIndex = 0;
				return resourceColors.GetSchema(resourceIndex).CellLight;
			}
		}
		protected internal override void SubscribeEditorEvents() {
			base.SubscribeEditorEvents();
			SubscribeResourceColorsEvents();
		}
		private void SubscribeResourceColorsEvents() {
			this.resourceColorsListener = new NotificationCollectionChangedListener<SchedulerColorSchema>(resourceColors);
		}
		protected internal override void UnsubscribeEditorEvents() {
			base.UnsubscribeEditorEvents();
			UnsubscribeResourceColorsEvents();
		}
		private void UnsubscribeResourceColorsEvents() {
			this.resourceColorsListener.Changed -= new EventHandler(OnResourceColorsChanged);
		}
		protected internal virtual void OnResourceColorsChanged(object sender, EventArgs args) {
			InnerSettings.PopulateItems();
		}
		protected override SchedulerBoundComboBoxEditSettings CreateSchedulerBasedEditorSettings() {
			return new AppointmentResourceEditSettings(this);
		}
		protected internal override void OnSchedulerControlChangedCore() {
			if (resourceColorsListener != null) {
				resourceColorsListener.Dispose();
				resourceColorsListener = null;
			}
			if (SchedulerControl != null)
				resourceColors = SchedulerControl.ResourceColorSchemas;
			else
				resourceColors = new SchedulerColorSchemaCollection();
			resourceColorsListener = new NotificationCollectionChangedListener<SchedulerColorSchema>(resourceColors);
		}
	}
	#endregion
	#region AppointmentResourceEditSettings
	public class AppointmentResourceEditSettings : ResourcesComboBoxControlBaseSettings {
		static AppointmentResourceEditSettings() {
			RegisterEditor();
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(AppointmentResourceEdit), typeof(AppointmentResourceEditSettings), delegate() { return new AppointmentResourceEdit(); }, delegate() { return new AppointmentResourceEditSettings(); });
		}
		public AppointmentResourceEditSettings() {
		}
		public AppointmentResourceEditSettings(SchedulerBoundComboBoxEdit ownerSchedulerEdit) {
			OwnerSchedulerEdit = ownerSchedulerEdit;
		}
		#region ShowEmptyResource
		public bool ShowEmptyResource {
			get { return (bool)GetValue(ShowEmptyResourceProperty); }
			set { SetValue(ShowEmptyResourceProperty, value); }
		}
		public static readonly DependencyProperty ShowEmptyResourceProperty = CreateShowEmptyResourceProperty();
		static DependencyProperty CreateShowEmptyResourceProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentResourceEditSettings, bool>("ShowEmptyResource", true, (d, e) => d.OnShowEmptyResourceChanged(e.OldValue, e.NewValue));
		}
		protected void OnShowEmptyResourceChanged(bool oldValue, bool newValue) {
			PopulateItems();
		}
		#endregion
		protected internal override void SetupControlBindings(SchedulerBoundComboBoxEdit sourceControl) {
			base.SetupControlBindings(sourceControl);
			SetBinding(ShowEmptyResourceProperty, InnerBindingHelper.CreateTwoWayPropertyBinding(sourceControl, "ShowEmptyResource"));
		}
		protected internal override void PopulateExtendedItems(NamedElementList items) {
			if (ShowEmptyResource)
				items.Add(CreateItem(ResourceBase.Empty, -1));
		}
		protected override NamedElement CreateItem(Resource resource, int resourceIndex) {
			AppointmentResourceEdit owner = OwnerSchedulerEdit as AppointmentResourceEdit;
			Color color = owner != null ? owner.CalculateResourceItemColor(resource, resourceIndex) : ColorExtension.Empty;
			return new ColoredNamedElement(resource.Id, resource.Caption, color);
		}
	}
	#endregion
}
