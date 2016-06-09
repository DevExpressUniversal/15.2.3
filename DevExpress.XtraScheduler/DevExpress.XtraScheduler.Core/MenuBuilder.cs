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
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Drawing {
	#region SchedulerHitTest
	public enum SchedulerHitTest {
		None = 0x00000001,
		Cell = 0x00000002,
		ResourceHeader = 0x00000004,
		DayHeader = 0x00000008,
		AllDayArea = 0x00000010,
		AppointmentResizingLeftEdge = 0x00000020,
		AppointmentResizingRightEdge = 0x00000040,
		AppointmentResizingTopEdge = 0x00000080,
		AppointmentResizingBottomEdge = 0x00000100,
		AppointmentMoveEdge = 0x00000200,
		AppointmentContent = 0x00000400,
		MoreButton = 0x00000800,
		DayOfWeekHeader = 0x00001000,
		GroupSeparator = 0x00002000,
		UpperLeftCorner = 0x00004000,
		DayViewColumn = 0x00008000,
		SingleWeek = 0x00010000,
		Timeline = 0x00020000,
		SelectionBar = 0x00040000,
		SelectionBarCell = 0x00080000,
		TimeScaleHeader = 0x00100000,
		Ruler = 0x00200000,
		NavigationButton = 0x00400000,
		ScrollMoreButton = 0x00800000,
		AppointmentDependency = 0x01000000,
		Undefined = 0x02000000,
		TimeIndicator = 0x04000000
	};
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region SchedulerPopupMenuBuilder (abstract class)
	public abstract class SchedulerPopupMenuBuilder : CommandBasedPopupMenuBuilder<SchedulerCommand, SchedulerMenuItemId> {
		readonly InnerSchedulerControl innerControl;
		protected SchedulerPopupMenuBuilder(InnerSchedulerControl innerControl, IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory)
			: base(uiFactory) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
		}
		public InnerSchedulerControl InnerControl { get { return innerControl; } }
	}
	#endregion
	#region SchedulerDefaultPopupMenuBuilder (abstract class)
	public abstract class SchedulerDefaultPopupMenuBuilder : SchedulerPopupMenuBuilder {
		readonly ISchedulerHitInfo hitInfo;
		protected SchedulerDefaultPopupMenuBuilder(InnerSchedulerControl innerControl, IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory, ISchedulerHitInfo hitInfo)
			: base(innerControl, uiFactory) {
			Guard.ArgumentNotNull(hitInfo, "hitInfo");
			this.hitInfo = hitInfo;
		}
		protected internal ISchedulerHitInfo HitInfo { get { return hitInfo; } }
		protected internal SchedulerHitTest HitTest { get { return HitInfo.HitTest; } }
		protected internal virtual IComparer<InnerSchedulerViewBase> ViewsComparer { get { return null; } }
		public override void PopulatePopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			switch (HitTest) {
				case SchedulerHitTest.AppointmentContent:
				case SchedulerHitTest.AppointmentMoveEdge:
				case SchedulerHitTest.AppointmentResizingBottomEdge:
				case SchedulerHitTest.AppointmentResizingLeftEdge:
				case SchedulerHitTest.AppointmentResizingRightEdge:
				case SchedulerHitTest.AppointmentResizingTopEdge:
					PopulateAppointmentPopupMenu(menu);
					break;
				case SchedulerHitTest.Ruler:
					PopulateTimeRulerPopupMenu(menu);
					break;
				case SchedulerHitTest.AppointmentDependency:
					PopulateAppointmentDependencyPopupMenu(menu);
					break;
				default:
					PopulateDefaultPopupMenu(menu);
					break;
			}
		}
		protected internal virtual SchedulerCommand CreateEditAppointmentCommand() {
			return new EditAppointmentQueryCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateGotoThisDayCommand() {
			ISchedulerHitInfo layoutHitInfo = HitInfo.FindFirstLayoutHitInfo();
			DateTime date = layoutHitInfo.ViewInfo.Interval.Start.Date;
			return new GotoThisDayCommand(InnerControl, date);
		}
		protected internal virtual SchedulerCommand CreateGotoDateCommand() {
			ISchedulerHitInfo layoutHitInfo = HitInfo.FindFirstLayoutHitInfo();
			DateTime date = layoutHitInfo.ViewInfo.Interval.Start.Date;
			return new GotoDateCommand(InnerControl, date);
		}
		protected internal virtual SchedulerCommand CreateDeleteAppointmentsCommand() {
			return new DeleteAppointmentsQueryCommand(InnerControl, InnerControl.SelectedAppointments);
		}
		protected internal abstract SchedulerCommand CreateCustomizeTimeRulerCommand();
		protected internal virtual SchedulerCommand CreateNewAppointmentCommand() {
			return new NewAppointmentCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateNewAllDayAppointmentCommand() {
			return new NewAllDayAppointmentCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateNewRecurringAppointmentCommand() {
			return new NewRecurringAppointmentCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateNewRecurringAllDayAppointmentCommand() {
			return new NewRecurringAllDayAppointmentCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateEditRecurrencePatternCommand() {
			return new EditRecurrencePatternCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateSwitchTimeScaleCommand(InnerDayView view, TimeSlot slot) {
			return new SwitchTimeScaleCommand(InnerControl, view, slot);
		}
		protected internal virtual SchedulerCommand CreateTimeScaleEnableCommand(TimeScale scale) {
			return new TimeScaleEnableCommand(InnerControl, scale);
		}
		protected internal virtual SchedulerCommand CreateTimeScaleVisibleCommand(TimeScale scale) {
			return new TimeScaleVisibleCommand(InnerControl, scale);
		}
		protected internal virtual SchedulerCommand CreateGotoTodayCommand() {
			return new GotoTodayCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateSwitchViewCommand(InnerSchedulerViewBase view) {
			return new SwitchViewCommand(InnerControl, view);
		}
		protected internal virtual SchedulerCommand CreateRestoreOccurrenceCommand() {
			return new RestoreOccurrenceCommand(InnerControl);
		}
		protected internal abstract SchedulerCommand CreateChangeAppointmentStatusCommand(IAppointmentStatus status, int statusIndex);
		protected internal abstract SchedulerCommand CreateChangeAppointmentLabelCommand(IAppointmentLabel label, int labelIndex);
		protected internal virtual void PopulateAppointmentPopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			menu.Id = SchedulerMenuItemId.AppointmentMenu;
			AddMenuItem(menu, CreateEditAppointmentCommand());
			AddMenuItem(menu, CreateEditRecurrencePatternCommand()).BeginGroup = true;
			AddMenuItem(menu, CreateRestoreOccurrenceCommand()).BeginGroup = true;
			AppendSubmenu(menu, CreateShowTimeAsSubMenu(), true);
			AppendSubmenu(menu, CreateLabelAsSubMenu(), false);
			AddMenuItem(menu, CreateDeleteAppointmentsCommand()).BeginGroup = true;
			if (InnerControl.ActiveViewType == SchedulerViewType.Gantt)
				AddMenuItem(menu, CreateAppointmentDependencyCreatingOperationCommand()).BeginGroup = true;
		}
		protected internal abstract SchedulerCommand CreateAppointmentDependencyCreatingOperationCommand();
		protected internal virtual void PopulateAppointmentDependencyPopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			menu.Id = SchedulerMenuItemId.AppointmentDependencyMenu;
			AddMenuItem(menu, CreateEditAppointmentDependencyCommand());
			AddMenuItem(menu, CreateDeleteAppointmentDependenciesCommand()).BeginGroup = true;
		}
		protected internal virtual SchedulerCommand CreateEditAppointmentDependencyCommand() {
			return new EditAppointmentDependencyCommand(InnerControl);
		}
		protected internal virtual SchedulerCommand CreateDeleteAppointmentDependenciesCommand() {
			return new DeleteAppointmentDependenciesCommand(InnerControl);
		}
		protected internal virtual void PopulateDefaultPopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			menu.Id = SchedulerMenuItemId.DefaultMenu;
			AppendNewAppointmentMenuItems(menu);
			SchedulerCommand gotoThisDayCommand = CreateGotoThisDayCommand();
			DefaultCommandUIState state = new DefaultCommandUIState();
			gotoThisDayCommand.UpdateUIState(state);
			if (state.Visible) {
				AddMenuItem(menu, gotoThisDayCommand).BeginGroup = true;
				AddMenuItem(menu, CreateGotoTodayCommand());
			} else
				AddMenuItem(menu, CreateGotoTodayCommand()).BeginGroup = true;
			AddMenuItem(menu, CreateGotoDateCommand());
			SchedulerViewType viewType = InnerControl.ActiveViewType;
			if (viewType == SchedulerViewType.Timeline || viewType == SchedulerViewType.Gantt) {
				AppendTimeScalesMenuItems(menu);
			}
			IDXPopupMenu<SchedulerMenuItemId> changeViewSubMenu = CreateSwitchViewSubMenu();
			if (changeViewSubMenu.ItemsCount > 1)
				AppendSubmenu(menu, changeViewSubMenu, true);
		}
		protected internal virtual void PopulateTimeRulerPopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			menu.Id = SchedulerMenuItemId.RulerMenu;
			AppendNewAppointmentMenuItems(menu);
			IDXPopupMenu<SchedulerMenuItemId> changeViewSubMenu = CreateSwitchViewSubMenu();
			if (changeViewSubMenu.ItemsCount > 1)
				AppendSubmenu(menu, changeViewSubMenu, true);
			AddMenuItem(menu, CreateCustomizeTimeRulerCommand()).BeginGroup = true;
			AppendTimeSlotsMenuItems(menu, (InnerDayView)InnerControl.ActiveView);
		}
		protected internal virtual void AppendTimeScalesMenuItems(IDXPopupMenu<SchedulerMenuItemId> menu) {
			AppendSubmenu(menu, CreateTimeScalesSubMenu(), true);
			AppendSubmenu(menu, CreateTimeScaleCaptionsSubMenu(), false);
		}
		protected internal virtual IDXPopupMenu<SchedulerMenuItemId> CreateTimeScalesSubMenu() {
			TimeScaleCollection scales = InnerControl.GetViewScales();
			int count = scales.Count;
			if (count <= 0)
				return null;
			IDXPopupMenu<SchedulerMenuItemId> menu = UiFactory.CreatePopupMenu();
			menu.Id = SchedulerMenuItemId.TimeScaleEnable;
			menu.Caption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScalesMenu);
			for (int i = 0; i < count; i++)
				AddMenuCheckItem(menu, CreateTimeScaleEnableCommand(scales[i]), String.Format("TSE{0}", i));
			return menu;
		}
		protected internal virtual IDXPopupMenu<SchedulerMenuItemId> CreateTimeScaleCaptionsSubMenu() {
			TimeScaleCollection scales = InnerControl.GetViewScales();
			int count = scales.Count;
			if (count <= 0)
				return null;
			IDXPopupMenu<SchedulerMenuItemId> menu = UiFactory.CreatePopupMenu();
			menu.Id = SchedulerMenuItemId.TimeScaleVisible;
			menu.Caption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_TimeScaleCaptionsMenu);
			for (int i = 0; i < count; i++)
				AddMenuCheckItem(menu, CreateTimeScaleVisibleCommand(scales[i]), String.Format("TSV{0}", i));
			return menu;
		}
		protected internal virtual void AppendNewAppointmentMenuItems(IDXPopupMenu<SchedulerMenuItemId> menu) {
			AddMenuItemIfCommandVisible(menu, CreateNewAppointmentCommand());
			AddMenuItemIfCommandVisible(menu, CreateNewAllDayAppointmentCommand());
			AddMenuItemIfCommandVisible(menu, CreateNewRecurringAppointmentCommand(), true);
			AddMenuItemIfCommandVisible(menu, CreateNewRecurringAllDayAppointmentCommand());
		}
		public virtual IDXPopupMenu<SchedulerMenuItemId> CreateShowTimeAsSubMenu() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if ( storage == null )
				return null;
			IAppointmentStatusStorage statuses = storage.Appointments.Statuses;
			int count = statuses.Count;
			if ( count <= 0 )
				return null;
			IDXPopupMenu<SchedulerMenuItemId> menu = UiFactory.CreatePopupMenu();
			menu.Id = SchedulerMenuItemId.StatusSubMenu;
			menu.Caption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_ShowTimeAs);
			for ( int i = 0; i < count; i++ ) {
				IAppointmentStatus status = statuses.GetByIndex(i);
				AddMenuCheckItem(menu, CreateChangeAppointmentStatusCommand(status, i), "STA");
			}
			return menu;
		}
		protected internal virtual void AppendTimeSlotsMenuItems(IDXPopupMenu<SchedulerMenuItemId> menu, InnerDayView view) {
			TimeSlotCollection slots = view.TimeSlots;
			int count = slots.Count;
			for (int i = 0; i < count; i++)
				AddMenuCheckItem(menu, CreateSwitchTimeScaleCommand(view, slots[i]), "TSL").BeginGroup = (i == 0);
		}
		public virtual IDXPopupMenu<SchedulerMenuItemId> CreateLabelAsSubMenu() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if ( storage == null )
				return null;
			IAppointmentLabelStorage labels = storage.Appointments.Labels;
			int count = labels.Count;
			if ( count <= 0 )
				return null;
			IDXPopupMenu<SchedulerMenuItemId> menu = UiFactory.CreatePopupMenu();
			menu.Id = SchedulerMenuItemId.LabelSubMenu;
			menu.Caption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_LabelAs);
			for ( int i = 0; i < count; i++ ) {
				IAppointmentLabel label = labels.GetByIndex(i);
				AddMenuCheckItem(menu, CreateChangeAppointmentLabelCommand(label, i), "LBL");
			}
			return menu;
		}
		public virtual IDXPopupMenu<SchedulerMenuItemId> CreateSwitchViewSubMenu() {
			int count = InnerControl.Views.Count;
			if (count <= 0)
				return null;
			IDXPopupMenu<SchedulerMenuItemId> menu = UiFactory.CreatePopupMenu();
			menu.Id = SchedulerMenuItemId.SwitchViewMenu;
			menu.Caption = SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_SwitchViewMenu);
			List<InnerSchedulerViewBase> views = new List<InnerSchedulerViewBase>();
			for(int i = 0; i < count; i++) 
				views.Add(InnerControl.Views.GetInnerView(i));
			if(ViewsComparer != null) 
				views.Sort(ViewsComparer);
			for(int i = 0; i < count; i++)
				AddMenuCheckItemIfCommandVisible(menu, CreateSwitchViewCommand(views[i]), "VW");
			return menu;
		}
	}
	#endregion
	#region SchedulerAppointmentDragPopupMenuBuilder
	public class SchedulerAppointmentDragPopupMenuBuilder : SchedulerPopupMenuBuilder {
		readonly SchedulerDragData dragData;
		readonly AppointmentBaseCollection changedAppointments;
		public SchedulerAppointmentDragPopupMenuBuilder(IMenuBuilderUIFactory<SchedulerCommand, SchedulerMenuItemId> uiFactory, InnerSchedulerControl control, SchedulerDragData dragData, AppointmentBaseCollection changedAppointments)
			: base(control, uiFactory) {
			this.dragData = dragData;
			this.changedAppointments = new AppointmentBaseCollection();
			this.changedAppointments.AddRange(changedAppointments);
		}
		public override void PopulatePopupMenu(IDXPopupMenu<SchedulerMenuItemId> menu) {
			menu.Id = SchedulerMenuItemId.AppointmentDragMenu;
			AddMenuItem(menu, new AppointmentDragMoveCommand(InnerControl, dragData, changedAppointments));
			AddMenuItem(menu, new AppointmentDragCopyCommand(InnerControl, dragData, changedAppointments));
			AddMenuItem(menu, new AppointmentDragCancelCommand(InnerControl, dragData, changedAppointments));
		}
	}
	#endregion
}
