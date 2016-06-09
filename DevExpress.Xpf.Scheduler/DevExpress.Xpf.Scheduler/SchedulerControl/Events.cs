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

using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using System.ComponentModel.Design;
using System;
using System.Windows;
#if WPF
#else  
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
#endif
namespace DevExpress.Xpf.Scheduler {
	public partial class SchedulerControl {
		#region UpdateUI
		EventHandler onUpdateUI;
		public event EventHandler UpdateUI {
			add { onUpdateUI += value; }
			remove { onUpdateUI -= value; }
		}
		protected internal virtual void RaiseUpdateUI() {
			EventHandler handler = onUpdateUI;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region PopupMenuShowingEvent
		public static readonly RoutedEvent PopupMenuShowingEvent = CreatePopupMenuShowingEvent();
		static RoutedEvent CreatePopupMenuShowingEvent() {
			return EventManager.RegisterRoutedEvent("PopupMenuShowing", RoutingStrategy.Direct, typeof(SchedulerMenuEventHandler), typeof(SchedulerControl));
		}
		public event SchedulerMenuEventHandler PopupMenuShowing {
			add { AddHandler(PopupMenuShowingEvent, value); }
			remove { RemoveHandler(PopupMenuShowingEvent, value); }
		}
		#endregion
		#region RemindersFormShowing
		RemindersFormEventHandler onRemindersFormShowing;
		public event RemindersFormEventHandler RemindersFormShowing {
			add { onRemindersFormShowing += value; }
			remove { onRemindersFormShowing -= value; }
		}
		protected internal virtual void RaiseRemindersFormShowing(RemindersFormEventArgs e) {
			RemindersFormEventHandler handler = onRemindersFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InplaceEditorShowing
		InplaceEditorEventHandler onInplaceEditorShowing;
		public event InplaceEditorEventHandler InplaceEditorShowing { add { onInplaceEditorShowing += value; } remove { onInplaceEditorShowing -= value; } }
		protected internal virtual void RaiseInplaceEditorShowing(InplaceEditorEventArgs e) {
			InplaceEditorEventHandler handler = onInplaceEditorShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RemindersFormDefaultAction
		[Category(SRCategoryNames.Scheduler)]
		public event RemindersFormDefaultActionEventHandler RemindersFormDefaultAction {
			add {
				if (innerControl != null)
					innerControl.RemindersFormDefaultAction += value;
			}
			remove {
				if (innerControl != null)
					innerControl.RemindersFormDefaultAction -= value;
			}
		}
		#endregion
		#region EditAppointmentFormShowing
		AppointmentFormEventHandler onEditAppointmentFormShowing;
		public event AppointmentFormEventHandler EditAppointmentFormShowing {
			add { onEditAppointmentFormShowing += value; }
			remove { onEditAppointmentFormShowing -= value; }
		}
		protected internal virtual void RaiseEditAppointmentFormShowing(EditAppointmentFormEventArgs e) {
			AppointmentFormEventHandler handler = onEditAppointmentFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region RecurrenceFormShowing
		RecurrenceFormEventHandler onRecurrenceFormShowing;
		public event RecurrenceFormEventHandler RecurrenceFormShowing {
			add { onRecurrenceFormShowing += value; }
			remove { onRecurrenceFormShowing -= value; }
		}
		protected internal virtual void RaiseRecurrenceFormShowing(RecurrenceFormEventArgs e) {
			RecurrenceFormEventHandler handler = onRecurrenceFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region GotoDateFormShowing
		GotoDateFormEventHandler onGotoDateFormShowing;
		public event GotoDateFormEventHandler GotoDateFormShowing {
			add { onGotoDateFormShowing += value; }
			remove { onGotoDateFormShowing -= value; }
		}
		protected internal virtual void RaiseGotoDateFormShowing(GotoDateFormEventArgs e) {
			GotoDateFormEventHandler handler = onGotoDateFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region DeleteRecurrentAppointmentFormShowing
		DeleteRecurrentAppointmentFormEventHandler onDeleteRecurrentAppointmentFormShowing;
		public event DeleteRecurrentAppointmentFormEventHandler DeleteRecurrentAppointmentFormShowing {
			add { onDeleteRecurrentAppointmentFormShowing += value; }
			remove { onDeleteRecurrentAppointmentFormShowing -= value; }
		}
		protected internal virtual void RaiseDeleteRecurrentAppointmentFormShowing(DeleteRecurrentAppointmentFormEventArgs e) {
			DeleteRecurrentAppointmentFormEventHandler handler = onDeleteRecurrentAppointmentFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region EditRecurrentAppointmentFormShowing
		EditRecurrentAppointmentFormEventHandler onEditRecurrentAppointmentFormShowing;
		public event EditRecurrentAppointmentFormEventHandler EditRecurrentAppointmentFormShowing {
			add { onEditRecurrentAppointmentFormShowing += value; }
			remove { onEditRecurrentAppointmentFormShowing -= value; }
		}
		protected internal virtual void RaiseEditRecurrentAppointmentFormShowing(EditAppointmentFormEventArgs e) {
			EditRecurrentAppointmentFormEventHandler handler = onEditRecurrentAppointmentFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InitNewAppointment
		AppointmentEventHandler onInitNewAppointment;
		[Category(SRCategoryNames.Scheduler)]
		public event AppointmentEventHandler InitNewAppointment {
			add { onInitNewAppointment += value; }
			remove { onInitNewAppointment -= value; }
		}
		protected internal virtual void RaiseInitNewAppointment(AppointmentEventArgs args) {
			AppointmentEventHandler handler = onInitNewAppointment;
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region AppointmentViewInfoCustomizing
		AppointmentViewInfoCustomizingEventHandler onAppointmentViewInfoCustomizing;
		[Category(SRCategoryNames.Scheduler)]
		public event AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing {
			add { onAppointmentViewInfoCustomizing += value; }
			remove { onAppointmentViewInfoCustomizing -= value; }
		}
		protected internal virtual void RaiseAppointmentViewInfoCustomizing(AppointmentViewInfoCustomizingEventArgs e) {
			AppointmentViewInfoCustomizingEventHandler handler = onAppointmentViewInfoCustomizing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region AllowAppointmentDrag
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDrag {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDrag += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDrag -= value;
			}
		}
		#endregion
		#region AllowAppointmentDragBetweenResources
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDragBetweenResources {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDragBetweenResources += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDragBetweenResources -= value;
			}
		}
		#endregion
		#region AllowAppointmentResize
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentResize {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentResize += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentResize -= value;
			}
		}
		#endregion
		#region AllowAppointmentCopy
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCopy {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentCopy += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentCopy -= value;
			}
		}
		#endregion
		#region AllowAppointmentDelete
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentDelete {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentDelete += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentDelete -= value;
			}
		}
		#endregion
		#region AllowAppointmentCreate
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentCreate {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentCreate += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentCreate -= value;
			}
		}
		#endregion
		#region AllowAppointmentEdit
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowAppointmentEdit {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentEdit += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentEdit -= value;
			}
		}
		#endregion
		#region AllowInplaceEditor
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentOperationEventHandler AllowInplaceEditor {
			add {
				if (innerControl != null)
					innerControl.AllowInplaceEditor += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowInplaceEditor -= value;
			}
		}
		#endregion
		#region AppointmentDrag
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentDragEventHandler AppointmentDrag {
			add {
				if (innerControl != null)
					innerControl.AppointmentDrag += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentDrag -= value;
			}
		}
		#endregion
		#region AppointmentDrop
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentDragEventHandler AppointmentDrop {
			add {
				if (innerControl != null)
					innerControl.AppointmentDrop += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentDrop -= value;
			}
		}
		#endregion
		#region AppointmentResizing
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentResizeEventHandler AppointmentResizing {
			add {
				if (innerControl != null)
					innerControl.AppointmentResizing += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentResizing -= value;
			}
		}
		#endregion
		#region AppointmentResized
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentResizeEventHandler AppointmentResized {
			add {
				if (innerControl != null)
					innerControl.AppointmentResized += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AppointmentResized -= value;
			}
		}
		#endregion
		#region AllowAppointmentConflicts
		[Category(SRCategoryNames.OptionsCustomization)]
		public event AppointmentConflictEventHandler AllowAppointmentConflicts {
			add {
				if (innerControl != null)
					innerControl.AllowAppointmentConflicts += value;
			}
			remove {
				if (innerControl != null)
					innerControl.AllowAppointmentConflicts -= value;
			}
		}
		#endregion
		#region CustomizeTimeRulerFormShowing
		CustomizeTimeRulerFormEventHandler onCustomizeTimeRulerFormShowing;
		public event CustomizeTimeRulerFormEventHandler CustomizeTimeRulerFormShowing {
			add { onCustomizeTimeRulerFormShowing += value; }
			remove { onCustomizeTimeRulerFormShowing -= value; }
		}
		protected internal virtual void RaiseCustomizeTimeRulerFormShowing(CustomizeTimeRulerFormEventArgs e) {
			CustomizeTimeRulerFormEventHandler handler = onCustomizeTimeRulerFormShowing;
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region QueryWorkTime
		[Category(SRCategoryNames.Scheduler)]
		public event QueryWorkTimeEventHandler QueryWorkTime {
			add {
				if (innerControl != null)
					innerControl.QueryWorkTime += value;
			}
			remove {
				if (innerControl != null)
					innerControl.QueryWorkTime -= value;
			}
		}
		#endregion
		#region VisibleIntervalChanged
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler VisibleIntervalChanged {
			add {
				if (innerControl != null)
					innerControl.VisibleIntervalChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.VisibleIntervalChanged -= value;
			}
		}
		#endregion
		#region StorageChanged
		EventHandler onStorageChanged;
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler StorageChanged {
			add { onStorageChanged += value; }
			remove { onStorageChanged -= value; }
		}
		protected internal virtual void RaiseStorageChanged() {
			EventHandler handler = onStorageChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region SelectionChanged
		[Category(SRCategoryNames.Scheduler)]
		public event EventHandler SelectionChanged {
			add {
				if (innerControl != null)
					innerControl.SelectionChanged += value;
			}
			remove {
				if (innerControl != null)
					innerControl.SelectionChanged -= value;
			}
		}
		#endregion
		#region ActiveViewChanging
		ActiveViewChangingEventHandler activeViewChanging;
		[Category(SRCategoryNames.Scheduler)]
		public event ActiveViewChangingEventHandler ActiveViewChanging {
			add { activeViewChanging += value; }
			remove { activeViewChanging -= value; }
		}
		protected internal virtual bool RaiseActiveViewChanging(SchedulerViewBase newView) {
			ActiveViewChangingEventHandler handler = activeViewChanging;
			if (handler != null && IsInitialized) {
				ActiveViewChangingEventArgs args = new ActiveViewChangingEventArgs(ActiveView, newView);
				handler(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region ActiveViewChanged
		[Category(SRCategoryNames.Scheduler)]
		EventHandler onActiveViewChanged;
		public event EventHandler ActiveViewChanged {
			add {
				onActiveViewChanged += value;
			}
			remove {
				onActiveViewChanged -= value;
			}
		}
		protected internal virtual void RaiseActiveViewChanged() {
			this.deferredOnActiveViewChanged = true;
			if (!this.deferredOnActiveViewChanged)
				return;
			if (onActiveViewChanged != null)
				onActiveViewChanged(this, EventArgs.Empty);
			this.deferredOnActiveViewChanged = false;
		}
		#endregion
		#region ResourceScrollBarValueChanged
		EventHandler onResourceScrollBarValueChanged;
		internal event EventHandler ResourceScrollBarValueChanged {
			add { onResourceScrollBarValueChanged += value; }
			remove { onResourceScrollBarValueChanged -= value; }
		}
		protected internal virtual void RaiseResourceScrollBarValueChanged() {
			EventHandler handler = onResourceScrollBarValueChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region CustomizeVisualViewInfo
		CustomizeVisualViewInfoEventHandler onCustomizeVisualViewInfo;
		public event CustomizeVisualViewInfoEventHandler CustomizeVisualViewInfo {
			add { onCustomizeVisualViewInfo += value; }
			remove { onCustomizeVisualViewInfo -= value; }
		}
		protected internal virtual void RaiseCustomizeVisualViewInfo(VisualViewInfoBase visualViewInfo) {
			CustomizeVisualViewInfoEventHandler handler = onCustomizeVisualViewInfo;
			if (handler != null)
				handler(this, new CustomizeVisualViewInfoEventArgs(visualViewInfo));
		}
		#endregion
		#region QueryResourceColorSchema
		[Category(SRCategoryNames.Scheduler)]
		QueryResourceColorSchemaEventHandler onQueryResourceColorSchema;
		public event QueryResourceColorSchemaEventHandler QueryResourceColorSchema {
			add {
				onQueryResourceColorSchema += value;
			}
			remove {
				onQueryResourceColorSchema -= value;
			}
		}
		protected internal virtual void RaiseQueryResourceColorSchema(QueryResourceColorSchemaEventArgs e) {
			if (onQueryResourceColorSchema != null)
				onQueryResourceColorSchema(this, e);
		}
		#endregion
		#region DateNavigatorQueryActiveViewType
		public event DateNavigatorQueryActiveViewTypeHandler DateNavigatorQueryActiveViewType;
		protected internal virtual SchedulerViewType RaiseDateNavigatorQueryActiveViewType(SchedulerViewType oldViewType, SchedulerViewType newViewType, DayIntervalCollection selectedDays) {
			if (DateNavigatorQueryActiveViewType == null)
				return newViewType;
			DateNavigatorQueryActiveViewTypeEventArgs e = new DateNavigatorQueryActiveViewTypeEventArgs(oldViewType, newViewType, selectedDays);
			DateNavigatorQueryActiveViewType(this, e);
			return e.NewViewType;
		}
		#endregion
		#region RangeControlAutoAdjusting
		[Category(SRCategoryNames.Scheduler)]
		public event RangeControlAdjustEventHandler RangeControlAutoAdjusting;
		void RaiseRangeControlAutoAdjusting(RangeControlAdjustEventArgs e) {
			if (RangeControlAutoAdjusting == null)
				return;
			RangeControlAutoAdjusting(this, e);
		}
		#endregion
		#region VisibleResourcesChanged
		public event VisibleResourcesChangedEventHandler VisibleResourcesChanged {
			add {
				if (InnerControl != null)
					InnerControl.VisibleResourcesChanged += value; 
			}
			remove { 
				if (InnerControl != null)
					InnerControl.VisibleResourcesChanged -= value; 
			}
		}
		#endregion
	}
}
