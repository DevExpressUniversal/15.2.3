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
using System.Globalization;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Localization;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using System.Reflection;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Internal;
#if !SL
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraScheduler.Native {
	#region ISupportAppointmentEdit
	public interface ISupportAppointmentEdit {
		void ShowAppointmentForm(Appointment apt, bool openRecurrenceForm, bool readOnly, CommandSourceType commandSourceType);
		void SelectNewAppointment(Appointment apt);
		void BeginEditNewAppointment(Appointment apt);
		void RaiseInitNewAppointmentEvent(Appointment apt);
	}
	#endregion
	#region ISupportAppointmentDependencyEdit
	public interface ISupportAppointmentDependencyEdit {
		void ShowAppointmentDependencyForm(AppointmentDependency dependency, bool readOnly, CommandSourceType commandSourceType);
	}
	#endregion
	public static class ResourceImageLoader {
		public static System.IO.Stream LoadSmallImageStream(string imageName, Assembly asm) {
			return LoadImageStream(GetSmallImageName(imageName), asm);
		}
		public static System.IO.Stream LoadLargeImageStream(string imageName, Assembly asm) {
			return LoadImageStream(GetSmallImageName(imageName), asm);
		}
		static System.IO.Stream LoadImageStream(string imageName, Assembly asm) {
			return asm.GetManifestResourceStream(imageName);
		}
		public static Image LoadSmallImage(string imageName, Assembly asm) {
			return CreateBitmapFromResources(GetSmallImageName(imageName), asm);
		}
		public static Image LoadLargeImage(string imageName, Assembly asm) {
			return CreateBitmapFromResources(GetLargeImageName(imageName), asm);
		}
		internal static Image CreateBitmapFromResources(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = asm.GetManifestResourceStream(name);
			Image image = SchedulerImageHelper.CreateImageFromStream(stream);
			return image;
		}
		internal static string GetSmallImageName(string imageName) {
			return GetImageName(imageName, "16x16");
		}
		internal static string GetLargeImageName(string imageName) {
			return GetImageName(imageName, "32x32");
		}
		public static string GetImageName(string imageName, string size) {
			return String.Format("DevExpress.XtraScheduler.Images.{0}_{1}.png", imageName, size);
		}
	}
}
namespace DevExpress.XtraScheduler.Commands {
	#region ISchedulerCommandTarget
	public interface ISchedulerCommandTarget {
	}
	#endregion
	#region IInnerSchedulerCommandTarget
	internal interface IInnerSchedulerCommandTarget {
		InnerSchedulerControl InnerSchedulerControl { get; }
	}
	#endregion
	#region SchedulerCommand (abstract class)
	public abstract class SchedulerCommand : ControlCommand<InnerSchedulerControl, SchedulerCommandId, SchedulerStringId> {
		protected SchedulerCommand(ISchedulerCommandTarget target)
			: base(GetInnerSchedulerControl(target)) {
		}
		#region Properties
		protected internal InnerSchedulerControl InnerControl { get { return Control; } }
		public abstract SchedulerMenuItemId MenuId { get; }
		protected override XtraLocalizer<SchedulerStringId> Localizer { get { return SchedulerLocalizer.Active; } }
		public override SchedulerCommandId Id { get { return SchedulerCommandId.None; } }
		protected internal virtual string Parameters { get { return String.Empty; } }
		protected override Assembly ImageResourceAssembly { get { return Assembly.GetExecutingAssembly(); } }
		protected override string ImageResourcePrefix { get { return "DevExpress.XtraScheduler.Images"; } }
		#endregion
		static InnerSchedulerControl GetInnerSchedulerControl(ISchedulerCommandTarget target) {
			Guard.ArgumentNotNull(target, "target");
			IInnerSchedulerCommandTarget actualTarget = target as IInnerSchedulerCommandTarget;
			if (actualTarget == null)
				Exceptions.ThrowArgumentException("actualTarget", actualTarget);
			InnerSchedulerControl innerControl = actualTarget.InnerSchedulerControl;
			if (innerControl == null)
				Exceptions.ThrowArgumentException("control", innerControl);
			return innerControl;
		}
		public virtual Stream LoadImageStream() {
			return CommandResourceImageLoader.LoadSmallImageStream(ImageResourcePrefix, ImageName, ImageResourceAssembly);
		}
	}
	#endregion
	#region SchedulerMenuItemSimpleCommand (abstract class)
	public abstract class SchedulerMenuItemSimpleCommand : SchedulerCommand {
		protected SchedulerMenuItemSimpleCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override void ForceExecute(ICommandUIState state) {
			NotifyBeginCommandExecution(state);
			try {
				ExecuteCore();
			} finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal abstract void ExecuteCore();
	}
	#endregion
	#region SwitchViewCommand
	public class SwitchViewCommand : SchedulerMenuItemSimpleCommand {
		InnerSchedulerViewBase view;
		public SwitchViewCommand(ISchedulerCommandTarget target, InnerSchedulerViewBase view)
			: base(target) {
			if (view == null)
				Exceptions.ThrowArgumentException("view", view);
			this.view = view;
		}
		#region Properties
		protected internal InnerSchedulerViewBase View { get { return view; } }
		public override SchedulerMenuItemId MenuId { get { return view.MenuItemId; } }
		public override string MenuCaption { get { return view.MenuCaption; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_SwitchViewMenu; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override string ImageName { get { return View.GetImageName(); } }
		#endregion
		protected internal override void ExecuteCore() {
			InnerControl.ActiveViewType = View.Type;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = View.Enabled;
			state.Visible = View.Enabled;
			state.Checked = (View.Type == InnerControl.ActiveViewType);
		}
	}
	#endregion
	#region SwitchToDayViewCommand
	public class SwitchToDayViewCommand : SwitchViewCommand {
		public SwitchToDayViewCommand(ISchedulerCommandTarget target)
			: base(target, ((InnerSchedulerControl)target).DayView) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToDayView; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_DayViewDescription; } }
	}
	#endregion
	#region SwitchToWorkWeekViewCommand
	public class SwitchToWorkWeekViewCommand : SwitchViewCommand {
		public SwitchToWorkWeekViewCommand(ISchedulerCommandTarget target)
			: base(target, ((InnerSchedulerControl)target).WorkWeekView) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToWorkWeekView; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_WorkWeekViewDescription; } }
	}
	#endregion
	#region SwitchToWeekViewCommand
	public class SwitchToWeekViewCommand : SwitchViewCommand {
		public SwitchToWeekViewCommand(ISchedulerCommandTarget target)
			: base(target, ((InnerSchedulerControl)target).WeekView) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToWeekView; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_WeekViewDescription; } }
	}
	#endregion
	#region SwitchToMonthViewCommand
	public class SwitchToMonthViewCommand : SwitchViewCommand {
		public SwitchToMonthViewCommand(ISchedulerCommandTarget target)
			: base(target, ((InnerSchedulerControl)target).MonthView) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToMonthView; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_MonthViewDescription; } }
	}
	#endregion
	#region SwitchToTimelineViewCommand
	public class SwitchToTimelineViewCommand : SwitchViewCommand {
		public SwitchToTimelineViewCommand(ISchedulerCommandTarget target)
			: base(target, ((InnerSchedulerControl)target).TimelineView) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.SwitchToTimelineView; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_TimelineViewDescription; } }
	}
	#endregion
	#region SwitchToGanttViewCommand
	public class SwitchToGanttViewCommand : SwitchViewCommand {
		public SwitchToGanttViewCommand(ISchedulerCommandTarget target)
			: base(target, ((InnerSchedulerControl)target).GanttView) {
		}
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_GanttViewDescription; } }
	}
	#endregion
	#region GotoDayCommand
	public class GotoDayCommand : SchedulerMenuItemSimpleCommand {
		DateTime date;
		public GotoDayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.GotoDay; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.GotoThisDay; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_GotoThisDay; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public DateTime Date { get { return date; } set { date = value; } }
		#endregion
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)InnerControl.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date, SchedulerViewType.Day);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			SchedulerViewType activeViewType = InnerControl.ActiveViewType;
			state.Visible = true;
			state.Enabled = InnerControl.DayView.Enabled && (activeViewType != SchedulerViewType.WorkWeek || activeViewType != SchedulerViewType.Day);
		}
	}
	#endregion
	#region GotoThisDayCommand
	public class GotoThisDayCommand : SchedulerMenuItemSimpleCommand {
		DateTime date;
		public GotoThisDayCommand(ISchedulerCommandTarget target)
			: base(target) {
			Initialize(InnerControl.Selection.FirstSelectedInterval.Start.Date);
		}
		public GotoThisDayCommand(ISchedulerCommandTarget target, DateTime date)
			: base(target) {
			Initialize(date);
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.GotoThisDay; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_GotoThisDay; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal DateTime Date { get { return date; } }
		#endregion
		protected internal virtual void Initialize(DateTime date) {
			this.date = date.Date;
		}
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)InnerControl.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date, SchedulerViewType.Day);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			SchedulerViewType activeViewType = InnerControl.ActiveViewType;
			state.Visible = InnerControl.DayView.Enabled && (activeViewType == SchedulerViewType.Week || activeViewType == SchedulerViewType.Month);
			state.Enabled = (InnerControl.Selection.Interval.Duration == DateTimeHelper.DaySpan);
		}
	}
	#endregion
	#region GotoTodayCommand
	public class GotoTodayCommand : SchedulerMenuItemSimpleCommand {
		public GotoTodayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.GotoToday; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.GotoToday; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_GotoToday; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_GotoToday; } }
		#endregion
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)InnerControl.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToToday();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
		}
	}
	#endregion
	#region SwitchTimeScaleCommand
	public class SwitchTimeScaleCommand : SchedulerMenuItemSimpleCommand {
		#region Fields
		InnerDayView view;
		TimeSlot slot;
		#endregion
		public SwitchTimeScaleCommand(ISchedulerCommandTarget target, InnerDayView view, TimeSlot slot)
			: base(target) {
			if (view == null)
				Exceptions.ThrowArgumentException("view", view);
			if (slot == null)
				Exceptions.ThrowArgumentException("slot", slot);
			this.view = view;
			this.slot = slot;
		}
		#region Properties
		protected internal InnerDayView View { get { return view; } }
		protected internal TimeSlot Slot { get { return slot; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.SwitchTimeScale; } }
		public override string MenuCaption { get { return slot.MenuCaption; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_SwitchViewMenu; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override string Parameters { get { return String.Format(CultureInfo.InvariantCulture, "{0}", slot.Value); } }
		#endregion
		protected internal override void ExecuteCore() {
			View.TimeScale = slot.Value;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = (View.TimeScale == slot.Value);
		}
	}
	#endregion
	#region TimeScaleCommandBase (abstract class)
	public abstract class TimeScaleCommandBase : SchedulerMenuItemSimpleCommand {
		TimeScale scale;
		protected TimeScaleCommandBase(ISchedulerCommandTarget target, TimeScale scale)
			: base(target) {
			if (scale == null)
				Exceptions.ThrowArgumentException("scale", scale);
			this.scale = scale;
		}
		#region Properties
		protected internal TimeScale Scale { get { return scale; } }
		public override string MenuCaption { get { return Scale.MenuCaption; } }
		protected internal override string Parameters {
			get {
				TimeScaleCollection innerControlGanttViewScales = InnerControl.GetViewScales();
				int scaleIndex = innerControlGanttViewScales.IndexOf(Scale);
				return scaleIndex.ToString(CultureInfo.InvariantCulture);
			}
		}
		#endregion
	}
	#endregion
	#region TimeScaleEnableCommand
	public class TimeScaleEnableCommand : TimeScaleCommandBase {
		public TimeScaleEnableCommand(ISchedulerCommandTarget target, TimeScale scale)
			: base(target, scale) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.TimeScaleEnable; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_TimeScalesMenu; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
		protected internal override void ExecuteCore() {
			Scale.Enabled = !Scale.Enabled;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
			state.Checked = Scale.Enabled;
		}
	}
	#endregion
	#region TimeScaleVisibleCommand
	public class TimeScaleVisibleCommand : TimeScaleCommandBase {
		public TimeScaleVisibleCommand(ISchedulerCommandTarget target, TimeScale scale)
			: base(target, scale) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.TimeScaleVisible; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_TimeScaleCaptionsMenu; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
		protected internal override void ExecuteCore() {
			Scale.Visible = !Scale.Visible;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = Scale.Enabled;
			state.Visible = true;
			state.Checked = Scale.Enabled && Scale.Visible;
		}
	}
	#endregion
	#region EditAppointmentCommandBase
	public abstract class EditAppointmentCommandBase : SchedulerMenuItemSimpleCommand {
		protected EditAppointmentCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal ISupportAppointmentEdit SupportAppointmentEdit { get { return InnerControl; } }
	}
	#endregion
	#region EditAppointmentCommandBase
	public abstract class EditExistingAppointmentCommandBase : EditAppointmentCommandBase {
		protected EditExistingAppointmentCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override bool CanExecute() {
			if (!base.CanExecute())
				return false;
			Appointment apt = ObtainEditedAppointment();
			return CanExecute(apt);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = false;
			state.Visible = true;
			if (!CheckInitalConditions()) {
				state.Visible = false;
				return;
			}
			ISetSchedulerStateService setStateService = InnerControl.GetService<ISetSchedulerStateService>();
			bool inplaceOpened = false;
			if (setStateService != null)
				inplaceOpened = setStateService.IsInplaceEditorOpened;
			Appointment editAppointment = ObtainEditedAppointment();
			bool canEditAppointment = editAppointment != null && (CanEditAppointment(editAppointment) || CanViewAppointmentAsReadOnly(editAppointment));
			state.Enabled = canEditAppointment & !inplaceOpened;
		}
		protected internal override void ExecuteCore() {
			Appointment apt = ObtainEditedAppointment();
			if (apt != null)
				Execute(apt);
		}
		protected virtual Appointment ObtainEditedAppointment() {
			AppointmentBaseCollection appointments = InnerControl.SelectedAppointments;
			if (appointments.Count == 1)
				return appointments[0];
			return null;
		}
		protected internal virtual bool CanEditAppointment(Appointment apt) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanEditAppointment(apt);
		}
		protected internal virtual bool CanViewAppointmentAsReadOnly(Appointment apt) {
			return InnerControl.OptionsCustomization.AllowDisplayAppointmentForm == AllowDisplayAppointmentForm.Always;
		}
		protected virtual bool CheckInitalConditions() {
			return true;
		}
		protected virtual bool CanExecute(Appointment apt) {
			return true;
		}
		public abstract void Execute(Appointment apt);
	}
	#endregion
	#region NewAppointmentCommandBase (abstract class)
	public abstract class NewAppointmentCommandBase : EditAppointmentCommandBase {
		protected NewAppointmentCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		protected internal abstract bool AllDay { get; }
		protected internal abstract bool Recurring { get; }
		protected internal virtual bool ConvertSelectionIntervalFromClientTime { get { return false; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (!CanCreateAppointment())
				return;
			Appointment apt = CreateNewAppointment();
			ShowEditAppointmentForm(apt, Recurring);
			if (!IsNewAppointment(apt) && apt.Type != AppointmentType.Pattern)
				SupportAppointmentEdit.SelectNewAppointment(apt);
		}
		protected internal virtual bool IsNewAppointment(Appointment apt) {
			if (InnerControl.Storage == null)
				return true;
			if (InnerControl.Storage.Appointments == null)
				return true;
			return InnerControl.Storage.Appointments.IsNewAppointment(apt);
		}
		protected internal virtual Appointment CreateNewAppointment() {
			AppointmentType type = CalculateNewAppointmentType();
			Appointment apt = CreateAppointmentForSelectedInterval(type);
			TimeInterval interval = InnerControl.TimeZoneHelper.ToClientTime(((IInternalAppointment)apt).GetInterval());
			bool appointmentAllDay = CalculateAppointmentAllDayValue(interval);
			if (AllDay != appointmentAllDay && appointmentAllDay) {
				apt.Start = interval.Start;
				apt.End = interval.End;
			}
			apt.AllDay = appointmentAllDay;
			if (apt.RecurrenceInfo != null) {
				apt.RecurrenceInfo.AllDay = appointmentAllDay;
				apt.RecurrenceInfo.FirstDayOfWeek = InnerControl.FirstDayOfWeek;
			}
			apt.StatusKey = CalculateAppointmentStatusId(apt.AllDay);
			RaiseInitNewAppointment(apt);
			return apt;
		}
		protected internal virtual Appointment CreateAppointmentForSelectedInterval(AppointmentType type) {
			TimeInterval selectionInterval = GetActualSelectedInterval();
			Appointment apt = InnerControl.Storage.CreateAppointment(type);
			apt.Start = selectionInterval.Start;
			apt.End = selectionInterval.End;
			apt.ResourceId = GetActualSelectedResourceId();
			IRecurrenceInfo recurrenceInfo = apt.RecurrenceInfo;
			if (recurrenceInfo != null) {
				recurrenceInfo.Start = apt.Start;
				recurrenceInfo.OccurrenceCount = 10;
			}
			return apt;
		}
		protected internal virtual object GetActualSelectedResourceId() {
			return InnerControl.Selection.Resource.Id;
		}
		protected internal virtual TimeInterval GetActualSelectedInterval() {
			if (ConvertSelectionIntervalFromClientTime)
				return InnerControl.TimeZoneHelper.FromClientTime(InnerControl.Selection.Interval);
			else
				return InnerControl.Selection.Interval;
		}
		protected internal virtual AppointmentType CalculateNewAppointmentType() {
			return Recurring ? AppointmentType.Pattern : AppointmentType.Normal;
		}
		protected internal virtual bool CalculateAppointmentAllDayValue(TimeInterval interval) {
			if (!typeof(InnerDayView).IsAssignableFrom(InnerControl.ActiveView.GetType()) || AllDay)
				return AllDay;
			return interval.Start.TimeOfDay == TimeSpan.Zero && interval.End.TimeOfDay == TimeSpan.Zero && interval.Duration.Ticks > 0;
		}
		protected internal virtual object CalculateAppointmentStatusId(bool allDay) {
			AppointmentStatusType type = allDay ? AppointmentStatusType.Free : AppointmentStatusType.Busy;
			IAppointmentStatus status = InnerControl.Storage.Appointments.Statuses.GetByType(type);
			if (status.Id == null)
				status = InnerControl.Storage.Appointments.Statuses.GetByIndex(0);
			return status.Id;
		}
		protected internal virtual void RaiseInitNewAppointment(Appointment apt) {
			SupportAppointmentEdit.RaiseInitNewAppointmentEvent(apt);
		}
		protected internal virtual void ShowEditAppointmentForm(Appointment apt, bool recurring) {
			SupportAppointmentEdit.ShowAppointmentForm(apt, recurring, false, CommandSourceType);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool canCreateAppointment = CanCreateAppointment();
			state.Enabled = canCreateAppointment;
			state.Visible = canCreateAppointment;
		}
		protected internal virtual bool CanCreateAppointment() {
			if (InnerControl.Storage == null)
				return false;
			AppointmentOperationHelper operationHelper = new AppointmentOperationHelper(InnerControl);
			Resource selectedResource = InnerControl.Storage.GetResourceById(GetActualSelectedResourceId());
			return operationHelper.CanCreateAppointment(GetActualSelectedInterval(), selectedResource, Recurring, false);
		}
	}
	#endregion
	#region NewAppointmentCommand
	public class NewAppointmentCommand : NewAppointmentCommandBase {
		public NewAppointmentCommand(InnerSchedulerControl control)
			: base(control) {
		}
		public NewAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.NewAppointment; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.NewAppointment; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_NewAppointment; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_NewAppointment; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.Appointment; } }
		protected internal override bool AllDay { get { return false; } }
		protected internal override bool Recurring { get { return false; } }
		protected internal override bool ConvertSelectionIntervalFromClientTime { get { return true; } }
		#endregion
	}
	#endregion
	#region NewRecurringAppointmentCommand
	public class NewRecurringAppointmentCommand : NewAppointmentCommandBase {
		public NewRecurringAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.NewRecurringAppointment; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_NewRecurringAppointment; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_NewRecurringAppointment; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.RecurringAppointment; } }
		protected internal override bool AllDay { get { return false; } }
		protected internal override bool Recurring { get { return true; } }
		protected internal override bool ConvertSelectionIntervalFromClientTime { get { return true; } }
		#endregion
	}
	#endregion
	#region NewAllDayAppointmentCommand
	public class NewAllDayAppointmentCommand : NewAppointmentCommandBase {
		public NewAllDayAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.NewAllDayEvent; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_NewAllDayEvent; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override bool AllDay { get { return true; } }
		protected internal override bool Recurring { get { return false; } }
		#endregion
		protected internal override bool CalculateAppointmentAllDayValue(TimeInterval interval) {
			return AllDay;
		}
	}
	#endregion
	#region NewRecurringAllDayAppointmentCommand
	public class NewRecurringAllDayAppointmentCommand : NewAppointmentCommandBase {
		public NewRecurringAllDayAppointmentCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.NewRecurringEvent; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_NewRecurringEvent; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override bool AllDay { get { return true; } }
		protected internal override bool Recurring { get { return true; } }
		#endregion
		protected internal override bool CalculateAppointmentAllDayValue(TimeInterval interval) {
			return AllDay;
		}
	}
	#endregion
	#region NewAppointmentViaInplaceEditorCommand
	public class NewAppointmentViaInplaceEditorCommand : NewAppointmentCommand {
		string subject = String.Empty;
		public NewAppointmentViaInplaceEditorCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		internal NewAppointmentViaInplaceEditorCommand(ISchedulerCommandTarget target, string subject)
			: base(target) {
			this.subject = subject;
		}
		public string Subject { get { return subject; } set { subject = value; } }
		protected internal override Appointment CreateAppointmentForSelectedInterval(AppointmentType type) {
			Appointment apt = base.CreateAppointmentForSelectedInterval(type);
			if (!String.IsNullOrEmpty(Subject))
				apt.Subject = this.Subject;
			return apt;
		}
		protected internal override bool CanCreateAppointment() {
			if (InnerControl.SelectedAppointments.Count > 0)
				return false;
			if (InnerControl.SelectedDependencies.Count > 0)
				return false;
			if (InnerControl.Storage == null)
				return false;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanCreateAppointmentViaInplaceEditor(InnerControl.Selection.Interval, InnerControl.Selection.Resource);
		}
		protected internal override void ShowEditAppointmentForm(Appointment apt, bool recurring) {
			SupportAppointmentEdit.BeginEditNewAppointment(apt);
		}
		protected internal override bool CalculateAppointmentAllDayValue(TimeInterval interval) {
			if (AllDay)
				return true;
			return interval.Start.TimeOfDay == TimeSpan.Zero && interval.End.TimeOfDay == TimeSpan.Zero && interval.Duration.Ticks > 0;
		}
	}
	#endregion
	#region EditAppointmentOrNewAppointmentViaInplaceEditorCommand
	public class EditAppointmentOrNewAppointmentViaInplaceEditorCommand : MultiCommand {
		public EditAppointmentOrNewAppointmentViaInplaceEditorCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.EditAppointmentOrNewAppointmentViaInplaceEditor; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.AppointmentLabel_Business; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
		protected internal override void CreateCommands() {
			Commands.Add(new EditAppointmentQueryCommand(InnerControl));
			Commands.Add(new NewAppointmentViaInplaceEditorCommand(InnerControl));
		}
	}
	#endregion
	#region ChangeAppointmentIntegerPropertyCommand (abstract class)
	public abstract class ChangeAppointmentObjectPropertyCommand : SchedulerMenuItemSimpleCommand {
		object newValue;
		protected ChangeAppointmentObjectPropertyCommand(ISchedulerCommandTarget target, object newValue)
			: base(target) {
			this.newValue = newValue;
		}
		protected internal virtual object NewValue { get { return newValue; } }
		protected internal override string Parameters { get { return newValue.ToString(); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image Image {
			get { return base.Image; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image LargeImage {
			get { return base.LargeImage; }
		}
		protected internal virtual bool CanEditAppointment(Appointment apt) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanEditAppointment(apt);
		}
		protected internal virtual bool CanEditAppointments(AppointmentBaseCollection appointments) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanEditAppointments(appointments);
		}
		protected internal virtual bool AreAllAppointmentsHaveSamePropertyValue(AppointmentBaseCollection appointments) {
			int count = appointments.Count;
			if (count <= 0)
				return false;
			object first = GetPropertyValue(appointments[0]);
			for (int i = 1; i < count; i++) {
				if (!Object.Equals(GetPropertyValue(appointments[i]), first))
					return false;
			}
			return true;
		}
		protected internal override void ExecuteCore() {
			InnerControl.BeginUpdate();
			StartAppointmentsTransaction();
			try {
				ChangeAppointmentsValues();
			} finally {
				CommitAppointmentsTransaction();
				InnerControl.EndUpdate();
			}
		}
		protected internal void ChangeAppointmentsValues() {
			AppointmentBaseCollection appointments = GetEditAppointments();
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = appointments[i];
				if (CanEditAppointment(apt))
					SetPropertyValue(apt, newValue);
			}
		}
		protected virtual AppointmentBaseCollection GetEditAppointments() {
			return InnerControl.SelectedAppointments;
		}
		protected virtual void StartAppointmentsTransaction() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if (storage == null)
				return;
			ISupportsAppointmentTransaction transaction = storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalStartAppointmentsTransaction();
		}
		protected virtual void CommitAppointmentsTransaction() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if (storage == null)
				return;
			ISupportsAppointmentTransaction transaction = storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalCommitAppointmentsTransaction(InnerControl.SelectedAppointments, AppointmentsTransactionType.Update);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			AppointmentBaseCollection appointments = InnerControl.SelectedAppointments;
			if (CanEditAppointments(appointments)) {
				state.Enabled = true;
				if (AreAllAppointmentsHaveSamePropertyValue(appointments))
					state.Checked = (Object.Equals(GetPropertyValue(appointments[0]), newValue));
				else
					state.Checked = false;
			} else
				state.Enabled = false;
		}
		protected internal abstract object GetPropertyValue(Appointment apt);
		protected internal abstract void SetPropertyValue(Appointment apt, object newValue);
	}
	#endregion
	public abstract class ChangeAppointmentPropertyImageCommandCore<T> : ChangeAppointmentObjectPropertyCommand {
		T image;
		T largeImage;
		protected ChangeAppointmentPropertyImageCommandCore(ISchedulerCommandTarget target, object newValue)
			: base(target, newValue) {
		}
		public new T Image {
			get { return image; }
		}
		public new T LargeImage {
			get { return largeImage; }
		}
		protected virtual void InitImages() {
			image = GetImage();
			largeImage = GetLargeImage();
		}
		protected abstract T GetImage();
		protected abstract T GetLargeImage();
	}
	#region ChangeAppointmentStatusCommand
	#endregion
	#region ChangeAppointmentLabelCommand
	#endregion
	#region DeleteAppointmentsCommandBase (abstract class)
	public abstract class DeleteAppointmentsCommandBase : SchedulerMenuItemSimpleCommand {
		#region Fields
		AppointmentBaseCollection appointments = new AppointmentBaseCollection();
		#endregion
		protected DeleteAppointmentsCommandBase(ISchedulerCommandTarget target)
			: base(target) {
			Initialize(InnerControl.SelectedAppointments);
		}
		protected DeleteAppointmentsCommandBase(ISchedulerCommandTarget target, Appointment appointment)
			: base(target) {
			Guard.ArgumentNotNull(appointment, "appointment");
			Initialize(appointment);
		}
		protected DeleteAppointmentsCommandBase(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target) {
			if (appointments == null)
				Exceptions.ThrowArgumentException("appointments", appointments);
			Initialize(appointments);
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.DeleteAppointment; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_DeleteAppointment; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_DeleteAppointment; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.Delete; } }
		public AppointmentBaseCollection Appointments { get { return appointments; } }
		#endregion
		protected internal virtual void Initialize(Appointment appointment) {
			this.appointments.Add(appointment);
		}
		protected internal virtual void Initialize(AppointmentBaseCollection appointments) {
			this.appointments.AddRange(appointments);
		}
		protected internal override void ExecuteCore() {
			InnerControl.BeginUpdate();
			StartAppointmentsTransaction();
			try {
				Modify();
			} finally {
				InnerControl.EndUpdate();
				if (CanCommitAppointmentTransaction)
					CommitAppointmentsTransaction();
				else
					CancelAppointmentsTransaction();
			}
		}
		protected virtual void StartAppointmentsTransaction() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if (storage == null)
				return;
			ISupportsAppointmentTransaction transaction = storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalStartAppointmentsTransaction();
		}
		protected virtual void CancelAppointmentsTransaction() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if (storage == null)
				return;
			ISupportsAppointmentTransaction transaction = storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalCancelAppointmentsTransaction();
		}
		protected virtual void CommitAppointmentsTransaction() {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if (storage == null)
				return;
			ISupportsAppointmentTransaction transaction = storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalCommitAppointmentsTransaction(appointments, AppointmentsTransactionType.Delete);
		}
		protected abstract void Modify();
		protected internal virtual void DeleteAppointmentAndDependencies(Appointment appointment) {
			ISchedulerStorageBase storage = InnerControl.Storage;
			if (storage == null) {
				appointment.Delete();
				return;
			}
			IInternalAppointmentDependencyStorage internalDependencyStorageImpl = storage.AppointmentDependencies as IInternalAppointmentDependencyStorage;
			DXCollection<AppointmentDependency> dependencies = internalDependencyStorageImpl.GetAllDependenciesById(appointment.Id);
			appointment.Delete();
			if (!appointment.IsDisposed)
				return;
			int count = dependencies.Count;
			for (int i = 0; i < count; i++) {
				dependencies[i].Delete();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool canDelete = CanDeleteAppointments(Appointments);
			ISetSchedulerStateService setStateService = InnerControl.GetService<ISetSchedulerStateService>();
			bool inplaceOpened = false;
			if (setStateService != null)
				inplaceOpened = setStateService.IsInplaceEditorOpened;
			state.Visible = IsVisible(canDelete, inplaceOpened);
			state.Enabled = canDelete & !inplaceOpened;
		}
		protected virtual bool IsVisible(bool canDelete, bool inplaceOpened) {
			return true;
		}
		protected internal virtual bool CanDeleteAppointment(Appointment apt) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanDeleteAppointment(apt);
		}
		protected internal virtual bool CanDeleteAppointments(AppointmentBaseCollection appointments) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanDeleteAppointments(appointments);
		}
		protected virtual bool CanCommitAppointmentTransaction { get { return true; } }
	}
	#endregion
	#region DeleteAppointmentsSimpleCommand
	public class DeleteAppointmentsSimpleCommand : DeleteAppointmentsCommandBase {
		public DeleteAppointmentsSimpleCommand(ISchedulerCommandTarget target, Appointment appointment)
			: base(target, appointment) {
		}
		protected override void Modify() {
			XtraSchedulerDebug.Assert(Appointments.Count == 1);
			DeleteAppointmentAndDependencies(Appointments[0]);
		}
	}
	#endregion
	#region DeleteAppointmentsCommand (abstract class)
	public abstract class DeleteAppointmentsCommand : DeleteAppointmentsCommandBase {
		protected DeleteAppointmentsCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected DeleteAppointmentsCommand(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target, appointments) {
		}
		protected override void Modify() {
			DeleteAppointmentsCommandFilter filter = new DeleteAppointmentsCommandFilter(this);
			filter.Process(Appointments);
			DeleteNonRecurringAppointments(filter.NonRecurringAppointments);
			DeleteRecurringAppointments(filter.RecurringAppointments);
		}
		protected internal virtual void DeleteNonRecurringAppointments(AppointmentBaseCollection nonRecurringAppointments) {
			int count = nonRecurringAppointments.Count;
			for (int i = 0; i < count; i++)
				DeleteAppointmentAndDependencies(nonRecurringAppointments[i]);
		}
		protected internal abstract void DeleteRecurringAppointments(AppointmentBaseCollection recurringAppointments);
	}
	#endregion
	#region DeleteAppointmentsQueryOrDependenciesCommand
	public class DeleteAppointmentsQueryOrDependenciesCommand : MultiCommand {
		public DeleteAppointmentsQueryOrDependenciesCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.DeleteAppointmentsQueryOrDependencies; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Msg_InternalError; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Msg_InternalError; } }
		#endregion
		protected internal override void CreateCommands() {
			Commands.Add(new DeleteAppointmentsQueryCommand(InnerControl));
			Commands.Add(new DeleteAppointmentDependenciesCommand(InnerControl));
		}
	}
	#endregion
	#region DeleteAppointmentsQueryCommand
	public class DeleteAppointmentsQueryCommand : DeleteAppointmentsCommand {
		public DeleteAppointmentsQueryCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public DeleteAppointmentsQueryCommand(ISchedulerCommandTarget target, AppointmentBaseCollection appointments)
			: base(target, appointments) {
		}
		protected internal virtual RecurrentAppointmentAction RecurrentAppointmentDeleteAction { get { return InnerControl.OptionsBehavior.RecurrentAppointmentDeleteAction; } }
		protected internal virtual bool QueryDeleteEachRecurringAppointment { get { return InnerControl.Owner.QueryDeleteForEachRecurringAppointment; } }
#if SL
		protected override bool CanCommitAppointmentTransaction { 
			get {
				return ShouldDeleteNormalAppointments() || InnerControl.OptionsBehavior.RecurrentAppointmentDeleteAction != RecurrentAppointmentAction.Ask; 
			}
		}
		protected bool ShouldDeleteNormalAppointments() {
			DeleteAppointmentsCommandFilter filter = new DeleteAppointmentsCommandFilter(this);
			filter.Process(Appointments);
			return filter.NonRecurringAppointments.Count > 0;
		}
#endif
		protected internal virtual RecurrentAppointmentAction QueryDeleteAppointment(Appointment apt) {
			RecurrentAppointmentAction deleteAction = InnerControl.OptionsBehavior.RecurrentAppointmentDeleteAction;
			if (deleteAction == RecurrentAppointmentAction.Ask)
				return QueryDeleteAppointmentCore(apt);
			else
				return deleteAction;
		}
		protected internal override void DeleteRecurringAppointments(AppointmentBaseCollection apts) {
			if (apts.Count <= 0)
				return;
			if (QueryDeleteEachRecurringAppointment)
				DeleteRecurringAppointmentsQueryEach(apts);
			else
				DeleteRecurringAppointmentsSingleQuery(apts);
		}
		protected internal virtual void DeleteRecurringAppointmentsQueryEach(AppointmentBaseCollection apts) {
			while (apts.Count > 0) {
				Appointment apt = apts[0];
				apts.RemoveAt(0);
				DeleteRecurringAppointment(apts, apt);
			}
		}
		protected internal virtual void DeleteRecurringAppointmentsSingleQuery(AppointmentBaseCollection apts) {
			RecurrentAppointmentAction action = RecurrentAppointmentDeleteAction;
			if (action == RecurrentAppointmentAction.Ask)
				action = InnerControl.Owner.ShowDeleteRecurrentAppointmentsForm(apts);
			DeleteRecurringAppointmentsSilently(apts, action);
		}
		protected internal virtual void DeleteRecurringAppointmentsSilently(AppointmentBaseCollection apts, RecurrentAppointmentAction action) {
			switch (action) {
				case RecurrentAppointmentAction.Cancel:
					return;
				case RecurrentAppointmentAction.Ask:
					return;
				case RecurrentAppointmentAction.Occurrence:
					DeleteOccurrences(apts);
					break;
				case RecurrentAppointmentAction.Series:
					DeleteSeries(apts);
					break;
			}
		}
		internal virtual void DeleteRecurringAppointment(AppointmentBaseCollection apts, Appointment apt) {
			Appointment pattern = apt.RecurrencePattern;
			if (pattern == null)
				return;
			switch (QueryDeleteAppointment(apt)) {
				case RecurrentAppointmentAction.Series:
					DeleteAppointmentAndDependencies(pattern);
					RemoveAppointmentsRelatedToPattern(apts, pattern);
					break;
				case RecurrentAppointmentAction.Occurrence:
					DeleteAppointmentAndDependencies(apt);
					break;
			}
		}
		internal virtual void DeleteOccurrences(AppointmentBaseCollection apts) {
			int count = apts.Count;
			for (int i = 0; i < count; i++)
				DeleteAppointmentAndDependencies(apts[i]);
		}
		internal virtual void DeleteSeries(AppointmentBaseCollection apts) {
			while (apts.Count > 0) {
				Appointment pattern = apts[0].RecurrencePattern;
				apts.RemoveAt(0);
				if (pattern != null) {
					DeleteAppointmentAndDependencies(pattern);
					RemoveAppointmentsRelatedToPattern(apts, pattern);
				}
			}
		}
		protected internal virtual void RemoveAppointmentsRelatedToPattern(AppointmentBaseCollection appointments, Appointment pattern) {
			int count = appointments.Count;
			for (int i = count - 1; i >= 0; i--)
				if (appointments[i].RecurrencePattern == pattern)
					appointments.RemoveAt(i);
		}
		protected internal virtual RecurrentAppointmentAction QueryDeleteAppointmentCore(Appointment apt) {
			return InnerControl.Owner.ShowDeleteRecurrentAppointmentForm(apt);
		}
	}
	#endregion
	#region RestoreOccurrenceCommand
	public class RestoreOccurrenceCommand : SchedulerMenuItemSimpleCommand {
		public RestoreOccurrenceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.RestoreOccurrence; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_RestoreOccurrence; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
		protected internal virtual bool CanEditAppointment(Appointment apt) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.CanEditAppointment(apt) && IsConflictResolved(apt);
		}
		protected internal override void ExecuteCore() {
			Appointment selectedAppointment = GetSelectedAppointment();
			if (IsConflictResolved(selectedAppointment))
			{
				InnerControl.BeginUpdate();
				InternalStartAppointmentsTransaction();
				try {
					selectedAppointment.RestoreOccurrence();
				} finally {
					InternalCommitAppointmentsTransaction(new Appointment[] { selectedAppointment }, AppointmentsTransactionType.RestoreOccurrence);
					InnerControl.EndUpdate();
				}
			}
		}
		void InternalCommitAppointmentsTransaction(Appointment[] appointment, AppointmentsTransactionType appointmentsTransactionType) {
			ISupportsAppointmentTransaction transaction = InnerControl.Storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalCommitAppointmentsTransaction(appointment, appointmentsTransactionType);
		}
		void InternalStartAppointmentsTransaction() {
			ISupportsAppointmentTransaction transaction = InnerControl.Storage.Appointments as ISupportsAppointmentTransaction;
			if (transaction == null)
				return;
			transaction.InternalStartAppointmentsTransaction();
		}
		protected Appointment GetSelectedAppointment() {
			return InnerControl.SelectedAppointments[0];
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (InnerControl.SupportsRecurrence) {
				AppointmentBaseCollection appointments = InnerControl.SelectedAppointments;
				if (appointments.Count == 1 && appointments[0].Type == AppointmentType.ChangedOccurrence) {
					state.Visible = true;
					state.Enabled = CanEditAppointment(appointments[0]);
					return;
				}
			}
			state.Enabled = false;
			state.Visible = false;
		}
		bool IsConflictResolved(Appointment appointment) {
			Appointment pattern = appointment.RecurrencePattern;
			OccurrenceCalculator calculator = OccurrenceCalculator.CreateInstance(pattern.RecurrenceInfo);
			Appointment occurrence = calculator.CalcOccurrenceByIndex(appointment.RecurrenceIndex, pattern);
			AppointmentOperationHelper helper = new AppointmentOperationHelper(InnerControl);
			return helper.IsConflictResolved(occurrence, appointment, false);
		}
	}
	#endregion
	#region NavigateViewForwardBackwardCommandBase
	public abstract class NavigateViewForwardBackwardCommandBase : SchedulerMenuItemSimpleCommand {
		protected NavigateViewForwardBackwardCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		#endregion
	}
	#endregion
	#region NavigateViewBackwardCommand
	public class NavigateViewBackwardCommand : NavigateViewForwardBackwardCommandBase {
		public NavigateViewBackwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.NavigateViewBackward; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			TimeIntervalCollection visibleIntervals = InnerControl.ActiveView.InnerVisibleIntervals;
			state.Enabled = (visibleIntervals.Start > InnerControl.LimitInterval.Start);
		}
		protected internal override void ExecuteCore() {
			InnerControl.Start = InnerControl.ActiveView.CalculateNewStartDateBackward();
		}
	}
	#endregion
	#region NavigateViewForwardCommand
	public class NavigateViewForwardCommand : NavigateViewForwardBackwardCommandBase {
		public NavigateViewForwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.NavigateViewForward; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			TimeIntervalCollection visibleIntervals = InnerControl.ActiveView.InnerVisibleIntervals;
			state.Enabled = (visibleIntervals.End < InnerControl.LimitInterval.End);
		}
		protected internal override void ExecuteCore() {
			InnerControl.Start = InnerControl.ActiveView.CalculateNewStartDateForward();
		}
	}
	#endregion
	#region SchedulerCommandCollection
	public class SchedulerCommandCollection : List<SchedulerCommand> {
	}
	#endregion
	#region MultiCommand (abstract class)
	public abstract class MultiCommand : SchedulerMenuItemSimpleCommand {
		SchedulerCommandCollection commands = new SchedulerCommandCollection();
		protected MultiCommand(ISchedulerCommandTarget target)
			: base(target) {
			CreateCommands();
		}
		#region Properties
		protected internal SchedulerCommandCollection Commands { get { return commands; } }
		public override CommandSourceType CommandSourceType {
			get { return base.CommandSourceType; }
			set {
				if (base.CommandSourceType == value)
					return;
				base.CommandSourceType = value;
				UpdateNestedCommandsSourceType();
			}
		}
		#endregion
		protected internal virtual void UpdateNestedCommandsSourceType() {
			int count = Commands.Count;
			for (int i = 0; i < count; i++)
				Commands[i].CommandSourceType = this.CommandSourceType;
		}
		public override void UpdateUIState(ICommandUIState state) {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				DefaultCommandUIState commandState = new DefaultCommandUIState();
				Commands[i].UpdateUIState(commandState);
				if (commandState.Enabled && commandState.Visible) {
					state.Enabled = commandState.Enabled;
					state.Visible = commandState.Visible;
					state.Checked = commandState.Checked;
					return;
				}
			}
			state.Enabled = false;
			state.Visible = false;
			state.Checked = false;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		public override void ForceExecute(ICommandUIState state) {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				DefaultCommandUIState commandState = new DefaultCommandUIState();
				SchedulerCommand command = Commands[i];
				command.UpdateUIState(commandState);
				if (commandState.Enabled && commandState.Visible) {
					command.ForceExecute(state);
					return;
				}
			}
		}
		protected internal override void ExecuteCore() {
		}
		protected internal abstract void CreateCommands();
	}
	#endregion
	#region ResourceNavigationCommandBase (abstract class)
	public abstract class ResourceNavigationCommandBase : SchedulerMenuItemSimpleCommand {
		protected ResourceNavigationCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_AppointmentLabelVacation; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal InnerSchedulerViewBase View { get { return InnerControl.ActiveView; } }
		public virtual int FirstResourceIndex { get { return View.ActualFirstVisibleResourceIndex; } set { View.FirstVisibleResourceIndex = value; } }
		public virtual int ResourcesPerPage { get { return View.ActualResourcesPerPage; } set { View.ResourcesPerPage = value; } }
		public virtual int ResourceCount { get { return View.FilteredResources.Count; } }
		#endregion
	}
	#endregion
	#region NavigateResourceBackwardCommand
	public abstract class NavigateResourceBackwardCommand : ResourceNavigationCommandBase {
		protected NavigateResourceBackwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal virtual bool CanScrollBackward() {
			if (ResourcesPerPage <= 0 || ResourceCount <= 0)
				return false;
			return FirstResourceIndex > 0;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = CanScrollBackward();
		}
	}
	#endregion
	#region NavigateResourceForwardCommand
	public abstract class NavigateResourceForwardCommand : ResourceNavigationCommandBase {
		protected NavigateResourceForwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal virtual bool CanScrollForward() {
			int resourceCount = ResourceCount;
			if (ResourcesPerPage <= 0 || resourceCount <= 0)
				return false;
			return FirstResourceIndex + ResourcesPerPage < resourceCount;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = CanScrollForward();
		}
	}
	#endregion
	#region NavigateFirstResourceCommand
	public class NavigateFirstResourceCommand : NavigateResourceBackwardCommand {
		public NavigateFirstResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			FirstResourceIndex = 0;
		}
	}
	#endregion
	#region NavigateResourcePageBackwardCommand
	public class NavigateResourcePageBackwardCommand : NavigateResourceBackwardCommand {
		public NavigateResourcePageBackwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			FirstResourceIndex = Math.Max(0, FirstResourceIndex - ResourcesPerPage);
		}
	}
	#endregion
	#region NavigatePrevResourceCommand
	public class NavigatePrevResourceCommand : NavigateResourceBackwardCommand {
		public NavigatePrevResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			FirstResourceIndex--;
		}
	}
	#endregion
	#region NavigateNextResourceCommand
	public class NavigateNextResourceCommand : NavigateResourceForwardCommand {
		public NavigateNextResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			FirstResourceIndex++;
		}
	}
	#endregion
	#region NavigateResourcePageForwardCommand
	public class NavigateResourcePageForwardCommand : NavigateResourceForwardCommand {
		public NavigateResourcePageForwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			FirstResourceIndex = Math.Min(ResourceCount - ResourcesPerPage, FirstResourceIndex + ResourcesPerPage);
		}
	}
	#endregion
	#region NavigateLastResourceCommand
	public class NavigateLastResourceCommand : NavigateResourceForwardCommand {
		public NavigateLastResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			FirstResourceIndex = ResourceCount - ResourcesPerPage;
		}
	}
	#endregion
	#region IncrementResourcePerPageCountCommand
	public class IncrementResourcePerPageCountCommand : ResourceNavigationCommandBase {
		public IncrementResourcePerPageCountCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal virtual bool CanIncrementResourcesPerPage() {
			int resourceCount = ResourceCount;
			if (resourceCount <= 0 || ResourcesPerPage <= 0)
				return false;
			return ResourcesPerPage < resourceCount;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = CanIncrementResourcesPerPage();
		}
		protected internal override void ExecuteCore() {
			ResourcesPerPage++;
			if (FirstResourceIndex + ResourcesPerPage > ResourceCount)
				FirstResourceIndex--;
		}
	}
	#endregion
	#region DecrementResourcePerPageCountCommand
	public class DecrementResourcePerPageCountCommand : ResourceNavigationCommandBase {
		public DecrementResourcePerPageCountCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal virtual bool CanDecrementResourcesPerPage() {
			return ResourceCount > 0 && ResourcesPerPage != 1;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = CanDecrementResourcesPerPage();
		}
		protected internal override void ExecuteCore() {
			ResourcesPerPage--;
		}
	}
	#endregion
	#region ViewZoomInCommand
	public class ViewZoomInCommand : SchedulerMenuItemSimpleCommand {
		public ViewZoomInCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.ViewZoomIn; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_ViewZoomIn; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_ViewZoomIn; } }
		public override string ImageName { get { return "ZoomIn"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = InnerControl.ActiveView.CanZoomIn();
			state.Checked = false;
		}
		protected internal override void ExecuteCore() {
			InnerControl.ActiveView.ZoomIn();
		}
	}
	#endregion
	#region ViewZoomOutCommand
	public class ViewZoomOutCommand : SchedulerMenuItemSimpleCommand {
		public ViewZoomOutCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerCommandId Id { get { return SchedulerCommandId.ViewZoomOut; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_ViewZoomOut; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_ViewZoomOut; } }
		public override string ImageName { get { return "ZoomOut"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = InnerControl.ActiveView.CanZoomOut();
			state.Checked = false;
		}
		protected internal override void ExecuteCore() {
			InnerControl.ActiveView.ZoomOut();
		}
	}
	#endregion
	#region NavigateMoreButtonCommand (abstract class)
	public abstract class NavigateMoreButtonCommand : SchedulerMenuItemSimpleCommand {
		#region Fields
		TimeInterval interval;
		Resource resource;
		DateTime targetViewStart;
		#endregion
		protected NavigateMoreButtonCommand(ISchedulerCommandTarget target, TimeInterval interval, Resource resource, DateTime targetViewStart)
			: base(target) {
			if (interval == null)
				Exceptions.ThrowArgumentNullException("interval");
			if (resource == null)
				Exceptions.ThrowArgumentNullException("resource");
			this.interval = interval;
			this.resource = resource;
			this.targetViewStart = targetViewStart;
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.Custom; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Msg_InternalError; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		public DateTime TargetViewStart { get { return targetViewStart; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = true;
		}
		protected internal override void ExecuteCore() {
			InnerControl.BeginUpdate();
			try {
				MoreButtonClickedEventArgs args = new MoreButtonClickedEventArgs(TargetViewStart, Interval, Resource);
				if (!InnerControl.RaiseMoreButtonClicked(InnerControl.Owner, args))
					PerformNavigation();
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected internal virtual void PerformNavigation() {
			SetSelection();
			InnerControl.ActiveViewType = SchedulerViewType.Day;
			InnerDayView dayView = InnerControl.ActiveView as InnerDayView;
			if (dayView == null)
				return;
			SetDayViewProperties(TargetViewStart);
		}
		protected abstract void SetSelection();
		protected internal virtual bool ShouldSetPreviousDay(TimeOfDayInterval visibleTime, TimeSpan targetTimeOfDay) {
			return (targetTimeOfDay < visibleTime.Start) && (targetTimeOfDay + TimeSpan.FromDays(1) < visibleTime.End);
		}
		protected internal virtual DateTime CalcActualDate(InnerDayView dayView, DateTime targetDateTime) {
			DateTime targetDate = targetDateTime.Date;
			TimeSpan targetTimeOfDay = targetDateTime.TimeOfDay;
			if (ShouldSetPreviousDay(dayView.VisibleTime, targetTimeOfDay))
				targetDate -= TimeSpan.FromDays(1);
			return targetDate;
		}
		protected internal virtual void SetDayViewProperties(DateTime targetDateTime) {
			InnerDayView innerDayView = (InnerDayView)InnerControl.ActiveView;
			DateTime actualDate = CalcActualDate(innerDayView, targetDateTime);
			innerDayView.DayCount = 1;
			innerDayView.SetStart(actualDate, InnerControl.Selection);
		}
	}
	#endregion
	#region ResourcesCommandBase
	public abstract class ResourceCommandBase : SchedulerCommand {
		Resource resource;
		protected ResourceCommandBase(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal abstract SchedulerControlChangeType ChangeType { get; }
		public Resource Resource { get { return resource; } set { resource = value; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool resourceIsVisible = true;
			bool isGanttView = InnerControl.ActiveViewType == SchedulerViewType.Gantt
				|| InnerControl.ActiveViewType == SchedulerViewType.Timeline;
			state.Visible = true;
			state.Enabled = Resource != null && isGanttView ? resourceIsVisible : false;
			state.Checked = false;
		}
		protected internal abstract void ModifyResource();
		public override void ForceExecute(ICommandUIState state) {
			ModifyResource();
			if (IsResourceVisible(Resource))
				InnerControl.ApplyChanges(ChangeType);
		}
		bool IsResourceVisible(Resource resource) {
			return InnerControl.ActiveView.VisibleResources.Contains(resource);
		}
	}
	#endregion
	#region CollapseResourceCommand
	public class CollapseResourceCommand : ResourceCommandBase {
		public CollapseResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.CollapseResource; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.CollapseResource; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Msg_InternalError; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override SchedulerControlChangeType ChangeType { get { return SchedulerControlChangeType.ResourceExpandedChanged; } }
		protected internal override void ModifyResource() {
			((IInternalResource)Resource).IsExpanded = false;
		}
	}
	#endregion
	#region ExpandResourceCommand
	public class ExpandResourceCommand : ResourceCommandBase {
		public ExpandResourceCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override SchedulerCommandId Id { get { return SchedulerCommandId.ExpandResource; } }
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.ExpandResource; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.Msg_InternalError; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.Caption_EmptyResource; } }
		protected internal override SchedulerControlChangeType ChangeType { get { return SchedulerControlChangeType.ResourceExpandedChanged; } }
		protected internal override void ModifyResource() {
			((IInternalResource)Resource).IsExpanded = true;
		}
	}
	#endregion
	public class EditAppointmentDependencyCommand : SchedulerMenuItemSimpleCommand {
		public EditAppointmentDependencyCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.OpenAppointmentDependency; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_EditAppointmentDependency; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_EditAppointmentDependency; } }
		protected internal ISupportAppointmentDependencyEdit SupportDependencyEdit { get { return InnerControl; } }
		#endregion
		protected internal virtual bool CanEditDependency(AppointmentDependency dep) {
			bool helperCanEditAppointment = true;
			return helperCanEditAppointment && InnerControl.OptionsCustomization.AllowDisplayAppointmentDependencyForm != AllowDisplayAppointmentDependencyForm.Never;
		}
		protected internal virtual bool CanViewAppointmentAsReadOnly() {
			return InnerControl.OptionsCustomization.AllowDisplayAppointmentDependencyForm == AllowDisplayAppointmentDependencyForm.Always;
		}
		protected internal override void ExecuteCore() {
			AppointmentDependency dep = InnerControl.SelectedDependencies[0];
			Execute(dep);
		}
		public virtual void Execute(AppointmentDependency dep) {
			if (CanEditDependency(dep))
				ShowEditFormCore(dep, false);
			else if (CanViewAppointmentAsReadOnly())
				ShowEditFormCore(dep, true);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			AppointmentDependencyBaseCollection appointments = InnerControl.SelectedDependencies;
			state.Enabled = appointments.Count == 1 && (CanEditDependency(appointments[0]) || CanViewAppointmentAsReadOnly());
		}
		protected internal virtual void ShowEditFormCore(AppointmentDependency dep, bool readOnly) {
			SupportDependencyEdit.ShowAppointmentDependencyForm(dep, readOnly, CommandSourceType);
		}
	}
	#region DeleteAppointmentDependenciesCommand
	public class DeleteAppointmentDependenciesCommand : SchedulerMenuItemSimpleCommand {
		#region Fields
		AppointmentDependencyBaseCollection dependencies;
		#endregion
		public DeleteAppointmentDependenciesCommand(ISchedulerCommandTarget target)
			: base(target) {
			Initialize(InnerControl.SelectedDependencies);
		}
		public DeleteAppointmentDependenciesCommand(ISchedulerCommandTarget target, AppointmentDependency dependency)
			: base(target) {
			Guard.ArgumentNotNull(dependency, "dependency");
			Initialize(dependency);
		}
		public DeleteAppointmentDependenciesCommand(ISchedulerCommandTarget target, AppointmentDependencyBaseCollection dependencies)
			: base(target) {
			Guard.ArgumentNotNull(dependencies, "dependencies");
			Initialize(dependencies);
		}
		#region Properties
		public override SchedulerMenuItemId MenuId { get { return SchedulerMenuItemId.DeleteAppointmentDependency; } }
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_DeleteAppointmentDependency; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_DeleteAppointmentDependency; } }
		public override string ImageName { get { return SchedulerCommandImagesNames.Delete; } }
		#endregion
		protected internal virtual void Initialize(AppointmentDependency dependency) {
			InitializeCore();
			this.dependencies.Add(dependency);
		}
		protected internal virtual void Initialize(AppointmentDependencyBaseCollection dependencies) {
			InitializeCore();
			this.dependencies.AddRange(dependencies);
		}
		protected internal virtual void InitializeCore() {
			this.dependencies = new AppointmentDependencyBaseCollection();
		}
		protected internal override void ExecuteCore() {
			InnerControl.BeginUpdate();
			try {
				DeleteDependencies();
			} finally {
				InnerControl.EndUpdate();
			}
		}
		protected internal virtual void DeleteDependencies() {
			int count = dependencies.Count;
			for (int i = 0; i < count; i++) {
				dependencies[i].Delete();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = false;
			state.Enabled = true; 
			state.Visible = true;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Commands.Internal {
	#region ServiceNavigateViewBackwardCommand
	public class ServiceNavigateViewBackwardCommand : NavigateViewBackwardCommand {
		#region Properties
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_NavigateBackward; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_NavigateBackward; } }
		public override string ImageName { get { return "Back"; } }
		#endregion
		public ServiceNavigateViewBackwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = InnerControl.GetService(typeof(IDateTimeNavigationService)) as IDateTimeNavigationService;
			if (service != null)
				service.NavigateBackward();
		}
	}
	#endregion
	#region ServiceNavigateViewForwardCommand
	public class ServiceNavigateViewForwardCommand : NavigateViewForwardCommand {
		#region Properties
		public override SchedulerStringId MenuCaptionStringId { get { return SchedulerStringId.MenuCmd_NavigateForward; } }
		public override SchedulerStringId DescriptionStringId { get { return SchedulerStringId.DescCmd_NavigateForward; } }
		public override string ImageName { get { return "Forward"; } }
		#endregion
		public ServiceNavigateViewForwardCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = InnerControl.GetService(typeof(IDateTimeNavigationService)) as IDateTimeNavigationService;
			if (service != null)
				service.NavigateForward();
		}
	}
	#endregion
	#region ServiceGotoTodayCommand
	public class ServiceGotoTodayCommand : GotoTodayCommand {
		public ServiceGotoTodayCommand(ISchedulerCommandTarget target)
			: base(target) {
		}
		public override string ImageName { get { return "Today"; } }
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = InnerControl.GetService(typeof(IDateTimeNavigationService)) as IDateTimeNavigationService;
			if (service != null)
				service.GoToToday();
		}
	}
	#endregion
}
