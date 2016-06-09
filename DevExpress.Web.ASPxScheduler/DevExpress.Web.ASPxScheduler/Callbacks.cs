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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Commands;
using DevExpress.Web.ASPxScheduler.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using System.Runtime.Serialization;
using DevExpress.Web.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
using System.Web.UI.WebControls;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Web.ASPxScheduler {
	#region SchedulerCallbackCommandId
	public static class SchedulerCallbackCommandId {
		public const string SwitchView = "SAVT";
		public const string SwitchGroupType = "SVGT";
		public const string NavigationButton = "NVBTN";
		public const string MenuAppointment = "MNUAPT";
		public const string MenuView = "MNUVIEW";
		public const string AppointmentFormShow = "EDTFRMSHOW";
		public const string ShowAppointmentFormByServerId = "EDTFRMSHOWSID";
		public const string AppointmentSave = "APTSAVE";
		public const string AppointmentCancel = "APTCANCEL";
		public const string AppointmentDelete = "APTDEL";
		public const string AppointmentsChange = "APTSCHANGE";
		public const string AppointmentsCopy = "APTSCOPY";
		public const string AppointmentData = "APTDATA";
		public const string AppointmentDataEx = "APTDATAEX";
		public const string NewAppointmentViaInplaceEditor = "NEWAPTVIAINPLFRM";
		public const string ClientSideInsertAppointment = "INSRTAPT";
		public const string ClientSideUpdateAppointment = "UPDTAPT";
		public const string ClientSideDeleteAppointment = "DLTAPT";
		public const string InplaceEditorShow = "INPLACESHOW";
		public const string InplaceEditorSave = "INPLACESAVE";
		public const string InplaceEditorEditForm = "INPLACEFORM";
		public const string GotoDateForm = "GOTODATEFORM";
		public const string GotoDateFormCancel = "GOTODATEFORMCANCEL";
		public const string GotoDate = "GOTODATE";
		public const string RecurrentAppointmentDelete = "RECURAPTDELETE";
		public const string RecurrentAppointmentDeleteCancel = "RECURAPTDELETECANCEL";
		public const string RecurrentAppointmentEdit = "RECURAPTEDIT";
		public const string RecurrentAppointmentEditCancel = "RECURAPTEDITCANCEL";
		public const string MoreButton = "MOREBTN";
		public const string GotoToday = "GOTODAY";
		public const string NavigateBackward = "BACK";
		public const string NavigateForward = "FORWARD";
		public const string ChangeTimeZone = "TZI";
		public const string RaiseMoreButtonClickedEvent = "RAISEMOREBTN";
		public const string NavigateResourceFirst = "NAVRESFIRST";
		public const string NavigateResourcePrevPage = "NAVRESPREVPAGE";
		public const string NavigateResourcePrev = "NAVRESPREV";
		public const string NavigateResourceNext = "NAVRESNEXT";
		public const string NavigateResourceNextPage = "NAVRESNEXTPAGE";
		public const string NavigateResourceLast = "NAVRESLAST";
		public const string IncrementVisibleResourceCount = "RESINC";
		public const string DecrementVisibleResourceCount = "RESDEC";
		public const string NavigateSpecificResource = "NAVSPECRES";
		public const string Refresh = "REFRESH";
		public const string OffsetVisibleIntervals = "OFFSETVISI";
		public const string SetVisibleDays = "SETVISDAYS";
		public const string ProcessReminder = "PROCESSREMINDER";
		public const string DismissReminder = "DSMSREMINDER";
		public const string DismissAllReminders = "DSMSALLREMINDERS";
		public const string SnoozeReminders = "SNZREMINDERS";
		public const string SnoozeAllReminders = "SNZALLREMINDERS";
		public const string CloseRemindersForm = "CLSREMINDERSFRM";
		public const string CustomRemindersAction = "CSTREMINDERSACT";
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	public static class SchedulerCallbackSR {
		public const char CallbackDivider = '|';
		public const char ValueDivider = ';';
		public const char ParameterDivider = ',';
		public const string FunctionCallbackPrefix = "FB";
	}
	#region SchedulerCallbackCommandManager
	public class SchedulerCallbackCommandManager {
		const string CustomCommandId = "CUSTOMCALLBACK";
		ASPxScheduler control;
		SchedulerCallbackCommandCollection commands;
		public SchedulerCallbackCommandManager(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
			this.commands = CreateCallbackCommands();
		}
		protected internal ASPxScheduler Control { get { return control; } }
		public bool IsCommandEnabled(string callbackCommandId) {
			CallbackCommandInfo commandInfo = new CallbackCommandInfo(callbackCommandId, String.Empty);
			SchedulerCallbackCommand command = GetCallbackCommand(commandInfo);
			if (command != null) {
				DefaultCommandUIState state = new DefaultCommandUIState();
				command.UpdateUIState(state);
				return state.Enabled;
			} else
				return false;
		}
		public SchedulerCallbackCommand GetCallbackCommand(CallbackCommandInfo commandInfo) {
			return LookupCallbackCommand(this.commands, commandInfo);
		}
		protected internal virtual SchedulerCallbackCommand LookupCallbackCommand(SchedulerCallbackCommandCollection commands, CallbackCommandInfo commandInfo) {
			int count = commands.Count;
			for (int i = 0; i < count; i++) {
				if (String.Compare(commandInfo.Id, commands[i].Id, true) == 0)
					return commands[i];
			}
			return null;
		}
		protected internal virtual SchedulerCallbackCommandCollection CreateCallbackCommands() {
			SchedulerCallbackCommandCollection result = new SchedulerCallbackCommandCollection();
			result.AddRange(CreateEditableCallbackCommands());
			result.Add(new SwitchViewCallbackCommand(Control));
			result.Add(new SwitchGroupTypeCallbackCommand(Control));
			result.Add(new NavigationButtonCallbackCommand(Control));
			result.Add(new MenuViewCallbackCommand(Control));
			result.Add(CreateMenuAppointmentCallbackCommand());
			result.Add(new ShowAppointmentFormByClientIdCallbackCommand(Control));
			result.Add(new ShowAppointmentFormByServerIdCallbackCommand(Control));
			result.Add(new AppointmentCancelCallbackCommand(Control));
			result.Add(new AppointmentDataCallbackCommand(Control));
			result.Add(new AppointmentDataExCallbackCommand(Control));
			result.Add(new GotoDateCallbackCommand(Control));
			result.Add(new GotoDateFormCallbackCommand(Control));
			result.Add(new GotoDateCancelCallbackCommand(Control));
			result.Add(new GotoTodayCallbackCommand(Control));
			result.Add(CreateRecurrentAppointmentEditCallbackCommand());
			result.Add(new RecurrentAppointmentEditCancelCallbackCommand(Control));
			result.Add(new NewAppointmentViaInplaceEditor(Control));
			result.Add(new NavigateBackwardCallbackCommand(Control));
			result.Add(new NavigateForwardCallbackCommand(Control));
			result.Add(new MoreButtonCallbackCommand(Control));
			result.Add(new RaiseMoreButtonClickedEventCallbackCommand(Control));
			result.Add(new ChangeTimeZoneCallbackCommand(Control));
			result.Add(new AppointmentInplaceEditorShowCallbackCommand(Control));
			result.Add(CreateAppointmentInplaceEditorEditFormCallbackCommand());
			result.Add(new NavigateResourceFirstCallbackCommand(Control));
			result.Add(new NavigateResourcePrevPageCallbackCommand(Control));
			result.Add(new NavigateResourcePrevCallbackCommand(Control));
			result.Add(new NavigateResourceNextCallbackCommand(Control));
			result.Add(new NavigateResourceNextPageCallbackCommand(Control));
			result.Add(new NavigateResourceLastCallbackCommand(Control));
			result.Add(new IncrementVisibleResourceCountCallbackCommand(Control));
			result.Add(new DecrementVisibleResourceCountCallbackCommand(Control));
			result.Add(new NavigateSpecificResourceCallbackCommand(Control));
			result.Add(new RefreshCallbackCommand(control));
			result.Add(new OffsetVisibleIntervalsCallbackCommand(control));
			result.Add(new SetVisibleDaysCallbackCommand(control));
			result.Add(new ProcessRemindersCallbackCommand(control));
			return result;
		}
		protected virtual RecurrentAppointmentEditCallbackCommand CreateRecurrentAppointmentEditCallbackCommand() {
			return new RecurrentAppointmentEditCallbackCommand(Control);
		}
		protected virtual MenuAppointmentCallbackCommand CreateMenuAppointmentCallbackCommand() {
			return new MenuAppointmentCallbackCommand(Control);
		}
		protected virtual AppointmentInplaceEditorEditFormCallbackCommand CreateAppointmentInplaceEditorEditFormCallbackCommand() {
			return new AppointmentInplaceEditorEditFormCallbackCommand(Control);
		}
		protected virtual SchedulerCallbackCommandCollection CreateEditableCallbackCommands() {
			SchedulerCallbackCommandCollection commandCollection = new SchedulerCallbackCommandCollection();
			commandCollection.Add(new AppointmentInplaceEditorSaveCallbackCommand(Control));
			commandCollection.Add(new AppointmentDeleteCallbackCommand(Control));
			commandCollection.Add(new AppointmentsChangeCommand(Control));
			commandCollection.Add(new AppointmentFormSaveCallbackCommand(Control));
			commandCollection.Add(new RecurrentAppointmentDeleteCancelCallbackCommand(Control));
			commandCollection.Add(new RecurrentAppointmentDeleteCallbackCommand(Control));
			commandCollection.Add(new AppointmentClientSideInsertCallbackCommand(control));
			commandCollection.Add(new AppointmentClientSideUpdateCallbackCommand(control));
			commandCollection.Add(new AppointmentClientSideDeleteCallbackCommand(control));
			commandCollection.Add(new SnoozeReminderCallbackCommand(Control));
			commandCollection.Add(new DismissReminderCallbackCommand(Control));
			commandCollection.Add(new DismissAllRemindersCallbackCommand(Control));
			commandCollection.Add(new CloseRemindersFormCallbackCommand(control));
			return commandCollection;
		}
		public bool IsCustomCommand(CallbackCommandInfo commandInfo) {
			return commandInfo.Id == CustomCommandId;
		}
		protected void PerformParseParametersForEditableCommand(CallbackCommandInfo commandInfo, SchedulerCallbackCommand command) {
			AppointmentSaveCallbackCommand editableCommand = command as AppointmentSaveCallbackCommand;
			if (editableCommand != null)
				editableCommand.ParseParameters(commandInfo.Parameters);
		}
	}
	#endregion
	#region CallbackCommandInfo
	public class CallbackCommandInfo {
		#region Fields
		string id;
		string parameters;
		#endregion
		public CallbackCommandInfo(string commandId, string commandParameters) {
			if (String.IsNullOrEmpty(commandId))
				Exceptions.ThrowArgumentException("commandId", commandId);
			this.id = commandId;
			this.parameters = commandParameters;
		}
		#region Properties
		public string Id { get { return id; } }
		public string Parameters { get { return parameters; } }
		#endregion
	}
	#endregion
	#region SchedulerCallbackCommand
	public abstract class SchedulerCallbackCommand {
		#region Fields
		readonly ASPxScheduler control;
		#endregion
		protected SchedulerCallbackCommand(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		#region Properties
		public abstract string Id { get; }
		protected internal ASPxScheduler Control { get { return control; } }
		public virtual bool RequiresControlHierarchy { get { return false; } }
		#endregion
		public virtual void Execute(string parameters) {
			ParseParameters(parameters);
			if (CanContinueExecute())
				ExecuteCore();
			FinalizeExecute();
		}
		protected internal abstract void ParseParameters(string parameters);
		protected internal abstract void ExecuteCore();
		protected internal virtual bool CanContinueExecute() {
			return true;
		}
		protected internal virtual void FinalizeExecute() {
		}
		protected internal virtual void UpdateUIState(ICommandUIState state) {
		}
	}
	#endregion
	#region SchedulerCallbackCommandCollection
	public class SchedulerCallbackCommandCollection : List<SchedulerCallbackCommand> {
	}
	#endregion
	#region SwitchViewCallbackCommand
	public class SwitchViewCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		SchedulerViewType type;
		#endregion
		public SwitchViewCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.SwitchView; } }
		internal SchedulerViewType Type { get { return type; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			try {
				this.type = (SchedulerViewType)Enum.Parse(typeof(SchedulerViewType), parameters);
			}
			catch {
				this.type = Control.ActiveViewType;
			}
		}
		protected internal override void ExecuteCore() {
			Control.ActiveViewType = type;
		}
	}
	#endregion
	#region SwitchGroupTypeCallbackCommand
	public class SwitchGroupTypeCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		SchedulerGroupType type;
		#endregion
		public SwitchGroupTypeCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.SwitchGroupType; } }
		internal SchedulerGroupType Type { get { return type; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			try {
				this.type = (SchedulerGroupType)Enum.Parse(typeof(SchedulerGroupType), parameters);
			}
			catch {
				this.type = Control.GroupType;
			}
		}
		protected internal override void ExecuteCore() {
			Control.GroupType = type;
		}
	}
	#endregion
	#region NavigationButtonCallbackCommand
	public class NavigationButtonCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		TimeInterval interval = TimeInterval.Empty;
		string resourceId = SchedulerIdHelper.EmptyResourceId;
		#endregion
		public NavigationButtonCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigationButton; } }
		public TimeInterval Interval { get { return interval; } }
		public object ResourceId { get { return resourceId; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			try {
				string[] parts = parameters.Split(new Char[] { ',' });
				XtraSchedulerDebug.Assert(parts.Length == 3);
				this.interval = SchedulerWebUtils.ToTimeInterval(parts[0], parts[1]);
				this.interval.Start = Control.InnerControl.TimeZoneHelper.ToClientTime(this.interval.Start);
				this.resourceId = parts[2];
			}
			catch {
				this.interval = TimeInterval.Empty; 
				resourceId = SchedulerIdHelper.EmptyResourceId;
			}
		}
		protected internal override void ExecuteCore() {
			Control.InnerControl.BeginUpdate();
			XtraScheduler.Resource resource = Control.LookupResourceByIdString(this.resourceId);
			try {
				Control.ActiveView.SetSelectionCore(Interval, resource);
			} finally {
				Control.InnerControl.EndUpdate();
			}
			SelectAppointmentByInterval(interval, resource);
		}
		protected internal virtual void SelectAppointmentByInterval(TimeInterval interval, XtraScheduler.Resource resource) {
			SchedulerViewBase view = Control.ActiveView;
			Appointment apt = view.FilteredAppointments.FindAppointmentExact(interval);
			if (apt != null)
				view.SelectAppointment(apt, resource);
		}
	}
	#endregion
	public abstract class MoreButtonBaseCallbackCommand : SchedulerCallbackCommand {
		#region Fileds
		DateTime targetViewStart = DateTime.MinValue;
		TimeInterval interval = TimeInterval.Empty;
		XtraScheduler.Resource resource = ResourceBase.Empty;
		#endregion
		protected MoreButtonBaseCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Propereties
		public DateTime TargetViewStart { get { return targetViewStart; } set { targetViewStart = value; } }
		public TimeInterval Interval { get { return interval; } set { interval = value; } }
		public Resource Resource { get { return resource; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			try {
				string[] parts = parameters.Split(new Char[] { ',', ',', ',' });
				XtraSchedulerDebug.Assert(parts.Length == 4);
				this.targetViewStart = SchedulerWebUtils.ToDateTime(parts[0]);
				this.interval = SchedulerWebUtils.ToTimeInterval(parts[1], parts[2]);
				this.resource = Control.LookupResourceByIdString(parts[3]);
			}
			catch {
				this.targetViewStart = DateTime.Today;
			}
		}
	}
	public class MoreButtonCallbackCommand : MoreButtonBaseCallbackCommand {
		public MoreButtonCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Propereties
		public override string Id { get { return SchedulerCallbackCommandId.MoreButton; } }
		#endregion
		protected internal override void ExecuteCore() {
			NavigateMoreButtonCommand command = new ASPxNavigateMoreButtonCommand(Control, Interval, Resource, TargetViewStart);
			command.Execute();
		}
	}
	public class ASPxNavigateMoreButtonCommand : NavigateMoreButtonCommand {
		ASPxScheduler control;
		public ASPxNavigateMoreButtonCommand(ASPxScheduler control, TimeInterval interval, XtraScheduler.Resource resource, DateTime targetViewStart)
			: base(control.InnerControl, interval, resource, targetViewStart) {
			this.control = control;
		}
		protected internal ASPxScheduler SchedulerControl { get { return control; } }
		protected override void SetSelection() {
			this.SchedulerControl.ActiveView.SetSelection(Interval, (XtraScheduler.Resource)Resource);
		}
	}
	public class RaiseMoreButtonClickedEventCallbackCommand : MoreButtonBaseCallbackCommand {
		public RaiseMoreButtonClickedEventCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Propereties
		public override string Id { get { return SchedulerCallbackCommandId.RaiseMoreButtonClickedEvent; } }
		#endregion
		protected internal override void ExecuteCore() {
			MoreButtonClickedEventArgs args = new MoreButtonClickedEventArgs(TargetViewStart, Interval, Resource);
			Control.InnerControl.RaiseMoreButtonClicked(Control, args);
		}
	}
	#region RefreshCallbackCommand
	public class RefreshCallbackCommand : SchedulerCallbackCommand {
		[Flags]
		enum ASPxClientSchedulerRefreshAction { None = 0, VisibleIntervalChanged = 0x1, ActiveViewChanged = 0x2 }
		ASPxClientSchedulerRefreshAction refreshAction = ASPxClientSchedulerRefreshAction.None;
		public RefreshCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.Refresh; } }
		ASPxClientSchedulerRefreshAction RefreshAction { get { return refreshAction; } }
		protected internal override void ExecuteCore() {
			if ((RefreshAction & ASPxClientSchedulerRefreshAction.VisibleIntervalChanged) != 0)
				Control.ApplyChanges(ASPxSchedulerChangeAction.NotifyVisibleIntervalsChanged);
			if ((RefreshAction & ASPxClientSchedulerRefreshAction.ActiveViewChanged) != 0)
				Control.ApplyChanges(ASPxSchedulerChangeAction.NotifyActiveViewChanged);
		}
		protected internal override void ParseParameters(string parameters) {
			if (String.IsNullOrEmpty(parameters))
				return;
			this.refreshAction = ParseRefreshAction(parameters);
		}
		ASPxClientSchedulerRefreshAction ParseRefreshAction(string stringValue) {
			ASPxClientSchedulerRefreshAction result = ASPxClientSchedulerRefreshAction.None;
			int value = 0;
			bool isParsed = int.TryParse(stringValue, out value);
			if (!isParsed)
				return result;
			result = (ASPxClientSchedulerRefreshAction)value;
			return result;
		}
	}
	#endregion
	#region SchedulerMenuCallbackCommand
	public abstract class SchedulerMenuCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		ASPxMenuItemInfo info;
		SchedulerMenuItemId menuItemId = SchedulerMenuItemId.Custom;
		#endregion
		protected SchedulerMenuCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		protected internal SchedulerMenuItemId MenuItemId { get { return menuItemId; } }
		protected internal ASPxMenuItemInfo Info { get { return info; } }
		internal InnerSchedulerControl InnerControl { get { return Control.InnerControl; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			try {
				this.info = ASPxMenuItemHelper.ParseMenuItemName(parameters);
				this.menuItemId = (SchedulerMenuItemId)Enum.Parse(typeof(SchedulerMenuItemId), info.Name);
			}
			catch {
				this.menuItemId = SchedulerMenuItemId.Custom;
			}
		}
		protected internal override void ExecuteCore() {
			if (menuItemId == SchedulerMenuItemId.Custom)
				return;
			SchedulerCommand command = LookupCommandByMenuItemId(menuItemId, info);
			if (command != null) {
				XtraSchedulerDebug.Assert(command.MenuId == menuItemId);
				if (command.CanExecute())
					command.Execute();
			}
		}
		protected internal abstract SchedulerCommand LookupCommandByMenuItemId(SchedulerMenuItemId id, ASPxMenuItemInfo info);
	}
	#endregion
	#region MenuAppointmentCallbackCommand
	public class MenuAppointmentCallbackCommand : SchedulerMenuCallbackCommand {
		public MenuAppointmentCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.MenuAppointment; } }
		public override bool RequiresControlHierarchy { get { return true; } }
		#endregion
		protected internal override SchedulerCommand LookupCommandByMenuItemId(SchedulerMenuItemId id, ASPxMenuItemInfo info) {
			switch (id) {
			case SchedulerMenuItemId.OpenAppointment:
				return new WebEditAppointmentCommand(Control);
			case SchedulerMenuItemId.EditSeries:
				return new EditRecurrencePatternCommand(Control.InnerControl);
			case SchedulerMenuItemId.RestoreOccurrence:
				return CreateRestoreOccurrenceCommand();
			case SchedulerMenuItemId.LabelSubMenu:
				return CreateChangeAppointmentLabelCommand(info);
			case SchedulerMenuItemId.StatusSubMenu:
				return CreateChangeAppointmentStatusCommand(info);
			case SchedulerMenuItemId.DeleteAppointment:
				return CreateWebDeleteAppointmentsCommand();
			default:
				return null;
			}
		}
		protected internal virtual SchedulerCommand CreateChangeAppointmentLabelCommand(ASPxMenuItemInfo info) {
			int index = LookupIntegerParameter(info);
			return index >= 0 ? CreateInstanceChangeAppointmentLabelCommand(Control, Control.Storage.Appointments.Labels.GetByIndex(index), index) : null;
		}
		protected internal virtual SchedulerCommand CreateChangeAppointmentStatusCommand(ASPxMenuItemInfo info) {
			int index = LookupIntegerParameter(info);
			return index >= 0 ? CreateInstanseChangeAppointmentStatusCommand(Control, Control.Storage.Appointments.Statuses.GetByIndex(index), index) : null;
		}
		protected internal virtual int LookupIntegerParameter(ASPxMenuItemInfo info) {
			try {
				return Convert.ToInt32(info.Parameters);
			}
			catch {
				return -1;
			}
		}
		protected virtual SchedulerCommand CreateRestoreOccurrenceCommand() {
			return new RestoreOccurrenceCommand(Control.InnerControl);
		}
		protected virtual SchedulerCommand CreateWebDeleteAppointmentsCommand() {
			return new WebDeleteAppointmentsCommand(Control);
		}
		protected virtual SchedulerCommand CreateInstanceChangeAppointmentLabelCommand(ISchedulerCommandTarget target, AppointmentLabel label, int labelId) {
			return new ChangeAppointmentLabelCommand((ASPxScheduler)target, label, labelId);
		}
		protected virtual SchedulerCommand CreateInstanseChangeAppointmentStatusCommand(ISchedulerCommandTarget target, AppointmentStatus status, int statusId) {
			return new ChangeAppointmentStatusCommand((ASPxScheduler)target, status, statusId);
		}
	}
	#endregion
	#region ViewMenuCallbackCommand
	public class MenuViewCallbackCommand : SchedulerMenuCallbackCommand {
		public MenuViewCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.MenuView; } }
		#endregion
		protected internal override SchedulerCommand LookupCommandByMenuItemId(SchedulerMenuItemId id, ASPxMenuItemInfo info) {
			switch (id) {
			case SchedulerMenuItemId.SwitchToDayView:
				return new SwitchViewCommand(InnerControl, InnerControl.DayView);
			case SchedulerMenuItemId.SwitchToWorkWeekView:
				return new SwitchViewCommand(InnerControl, InnerControl.WorkWeekView);
			case SchedulerMenuItemId.SwitchToWeekView:
				return new SwitchViewCommand(InnerControl, InnerControl.WeekView);
			case SchedulerMenuItemId.SwitchToMonthView:
				return new SwitchViewCommand(InnerControl, InnerControl.MonthView);
			case SchedulerMenuItemId.SwitchToTimelineView:
				return new SwitchViewCommand(InnerControl, InnerControl.TimelineView);
			case SchedulerMenuItemId.SwitchToFullWeekView:
				return new SwitchViewCommand(InnerControl, InnerControl.FullWeekView);
			case SchedulerMenuItemId.GotoThisDay:
				return new GotoThisDayCommand(InnerControl, Control.Selection.Interval.Start.Date);
			case SchedulerMenuItemId.GotoToday:
				return new GotoTodayCommand(InnerControl);
			case SchedulerMenuItemId.TimeScaleEnable:
				return CreateTimeScaleEnableCommand(info);
			case SchedulerMenuItemId.TimeScaleVisible:
				return CreateTimeScaleVisibleCommand(info);
			case SchedulerMenuItemId.SwitchTimeScale:
				return CreateSwitchTimeScaleCommand(info);
			case SchedulerMenuItemId.NewAppointment:
				return new WebNewAppointmentCommand(Control);
			case SchedulerMenuItemId.NewAllDayEvent:
				return new NewAllDayAppointmentCommand(InnerControl);
			case SchedulerMenuItemId.NewRecurringAppointment:
				return new WebNewRecurringAppointmentCommand(Control);
			case SchedulerMenuItemId.NewRecurringEvent:
				return new NewRecurringAllDayAppointmentCommand(InnerControl);
			case SchedulerMenuItemId.GotoDate:
				return new WebGotoDateCommand(Control);
			default:
				return null;
			}
		}
		protected internal virtual SwitchTimeScaleCommand CreateSwitchTimeScaleCommand(ASPxMenuItemInfo info) {
			try {
				TimeSpan span = TimeSpan.Parse(info.Parameters);
				InnerDayView view = (InnerDayView)InnerControl.ActiveView;
				TimeSlot slot = view.TimeSlots.FindNearestTimeSlot(span);
				if (slot != null)
					return new SwitchTimeScaleCommand(InnerControl, view, slot);
				else
					return null;
			}
			catch {
				return null;
			}
		}
		protected internal virtual TimeScaleEnableCommand CreateTimeScaleEnableCommand(ASPxMenuItemInfo info) {
			TimeScale scale = LookupTimelineScale(info);
			if (scale != null)
				return new TimeScaleEnableCommand(InnerControl, scale);
			else
				return null;
		}
		protected internal virtual TimeScaleVisibleCommand CreateTimeScaleVisibleCommand(ASPxMenuItemInfo info) {
			TimeScale scale = LookupTimelineScale(info);
			if (scale != null)
				return new TimeScaleVisibleCommand(InnerControl, scale);
			else
				return null;
		}
		protected internal virtual TimeScale LookupTimelineScale(ASPxMenuItemInfo info) {
			try {
				int scaleIndex = Convert.ToInt32(info.Parameters);
				return InnerControl.TimelineView.Scales[scaleIndex];
			}
			catch {
				return null;
			}
		}
	}
	#endregion
	#region SchedulerFormCallbackCommand
	public abstract class SchedulerFormCallbackCommand : SchedulerCallbackCommand {
		bool canCloseForm;
		bool isFormDataError;
		FormCallbackCommandHelper formCallbackCommandHelper;
		protected SchedulerFormCallbackCommand(ASPxScheduler control)
			: base(control) {
			this.formCallbackCommandHelper = new FormCallbackCommandHelper(FormId);
		}
		#region Properties
		protected internal virtual bool CanCloseForm { get { return canCloseForm; } set { canCloseForm = value; } }
		protected internal abstract SchedulerFormVisibility FormVisibility { get; }
		protected internal bool IsFormDataError { get { return isFormDataError; } set { isFormDataError = value; } }
		protected abstract string FormId { get; }
		protected FormCallbackCommandHelper FormCallbackCommandHelper { get { return this.formCallbackCommandHelper; } }
		#endregion
		public override void Execute(string parameters) {
			CanCloseForm = true;
			Control.IgnoreFormPostData = true;
			base.Execute(parameters);
		}
		protected internal override void FinalizeExecute() {
			if (IsFormDataError) {
				CanCloseForm = false;
				Control.IsFormRecreated = true;
				Control.IgnoreFormPostData = false;
			}
			if (CanCloseForm) {
				Control.ActiveFormType = SchedulerFormType.None;
				if (FormVisibility == SchedulerFormVisibility.FillControlArea)
					Control.EnsureProcessToolbarBlocks();
			}
			Control.ProtectedResetControlHierarchy();
		}
		protected virtual Control FindControl(string controlId) {
			SchedulerFormTemplateContainer container = Control.FormBlock.CurrentFormTemplateContainer as SchedulerFormTemplateContainer;
			return container.FindControl(FormCallbackCommandHelper.GetDefaultInputPath(controlId, Control.ProtectedIdSeparator));
		}
	}
	#endregion
	#region AppointmentSaveCallbackCommand
	public abstract class AppointmentSaveCallbackCommand : SchedulerFormCallbackCommand {
		#region Fields
		AppointmentFormTemplateContainerBase templateContainer;
		AppointmentFormController controller;
		bool isLostAppointmentDetected;
		#endregion
		protected AppointmentSaveCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override bool RequiresControlHierarchy { get { return true; } }
		protected internal AppointmentFormController Controller { get { return controller; } }
		protected internal AppointmentFormTemplateContainerBase TemplateContainer {
			get {
				if (templateContainer == null)
					templateContainer = GetTemplateContainer();
				return templateContainer;
			}
		}
		protected virtual AppointmentFormTemplateContainerBase GetTemplateContainer() {
			return (AppointmentFormTemplateContainerBase)Control.FormBlock.CurrentFormTemplateContainer;
		}
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.AppointmentFormVisibility; } }
		protected internal bool IsLostAppointmentDetected { get { return isLostAppointmentDetected; } }
		protected bool IsNewAppointment { get { return Control.EditableAppointment.IsNewlyCreated; } }
		protected virtual string TimeZoneId { get; set; }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			Appointment apt = GetEditedAppointment();
			if (apt == null) {
				this.isLostAppointmentDetected = true;
				SchedulerStatusInfoHelper.AddWarning(Control, SchedulerLocalizer.GetString(SchedulerStringId.Msg_WarningAppointmentDeleted));
				return;
			}
			this.controller = CreateAppointmentFormController(apt);
			AssignControllerValues();
		}
		protected internal virtual Appointment GetEditedAppointment() {
			return Control.GetEditedAppointment();
		}
		protected internal override void ExecuteCore() {
			if (IsLostAppointmentDetected) {
				CanCloseForm = true;
				return;
			}
			if (!Controller.IsConflictResolved()) {
				IsFormDataError = true;
				SchedulerStatusInfoHelper.AddError(Control, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict));
				return;
			}
			ApplyChanges();
			IAppointmentSelectionService appointmentSelectionService = (IAppointmentSelectionService)Control.GetService(typeof(IAppointmentSelectionService));
			appointmentSelectionService.SelectAppointment(Controller.SourceAppointment);
		}
		protected internal virtual void ApplyChanges() {
			if (CanApplyChanges())
				Controller.ApplyChanges();
		}
		protected internal virtual bool CanApplyChanges() {
			return !IsFormDataError && !IsLostAppointmentDetected;
		}
		protected internal virtual Control FindControlByID(string id) {
			return TemplateContainer.FindControl(FormCallbackCommandHelper.GetDefaultInputPath(id, Control.ProtectedIdSeparator));
		}
		protected internal virtual DateTime FromClientTime(DateTime date) {
			return Control.InnerControl.TimeZoneHelper.FromClientTime(date, TimeZoneId);
		}
		protected virtual AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new AppointmentFormController(Control, apt);
		}
		protected internal abstract void AssignControllerValues();
		protected internal override void FinalizeExecute() {
			base.FinalizeExecute();
			if (CanApplyChanges())
				Control.SetEditableAppointment(Controller.SourceAppointment);
		}
		protected internal override bool CanContinueExecute() {
			if (Controller == null)
				return false;
			Appointment editedAppointment = Controller.EditedAppointmentCopy;
			if (editedAppointment == null || Control == null)
				return false;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			bool canContinue = (IsNewAppointment) ? helper.CanCreateAppointment(editedAppointment) : helper.CanEditAppointment(editedAppointment);
			if (!canContinue) {
				if (!Controller.IsConflictResolved()) {
					IsFormDataError = true;
					SchedulerStatusInfoHelper.AddError(Control, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict));
				}
			}
			return canContinue;
		}
	}
	#endregion
	#region AppointmentFormSaveCallbackCommand
	public class AppointmentFormSaveCallbackCommand : AppointmentSaveCallbackCommand {
		public AppointmentFormSaveCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentSave; } }
		protected override string FormId { get { return SchedulerFormNames.AppointmentForm; } }
		protected internal override void AssignControllerValues() {
			ASPxTextBox tbSubject = (ASPxTextBox)FindControlByID("tbSubject");
			ASPxTextBox tbLocation = (ASPxTextBox)FindControlByID("tbLocation");
			ASPxMemo tbDescription = (ASPxMemo)FindControlByID("tbDescription");
			ASPxDateEdit edtStartDate = (ASPxDateEdit)FindControlByID("edtStartDate");
			ASPxDateEdit edtEndDate = (ASPxDateEdit)FindControlByID("edtEndDate");
			ASPxCheckBox chkAllDay = (ASPxCheckBox)FindControlByID("chkAllDay");
			ASPxComboBox edtShowTimeAs = (ASPxComboBox)FindControlByID("edtStatus");
			ASPxComboBox edtLabel = (ASPxComboBox)FindControlByID("edtLabel");
			ASPxComboBox edtResource = (ASPxComboBox)FindControlByID("edtResource");
			ASPxCheckBox chkReminder = (ASPxCheckBox)FindControlByID("chkReminder");
			ASPxComboBox cbReminder = (ASPxComboBox)FindControlByID("cbReminder");
			ASPxDropDownEdit ddResource = (ASPxDropDownEdit)FindControlByID("ddResource");
			if (chkAllDay != null)
				Controller.AllDay = chkAllDay.Checked;
			DateTime clientStart = DateTime.MinValue;
			DateTime clientEnd = DateTime.MinValue;
			if (edtStartDate != null) {
				if (Controller.AllDay)
					clientStart = edtStartDate.Date;
				else
					clientStart = FromClientTime(edtStartDate.Date);
				Controller.EditedAppointmentCopy.Start = clientStart;
			}
			if (edtEndDate != null) {
				if (Controller.AllDay)
					clientEnd = edtEndDate.Date;
				else
					clientEnd = FromClientTime(edtEndDate.Date);
				Controller.EditedAppointmentCopy.End = clientEnd;
			}
			if (tbSubject != null)
				Controller.Subject = tbSubject.Text;
			if (tbLocation != null)
				Controller.Location = tbLocation.Text;
			if (tbDescription != null)
				Controller.Description = tbDescription.Text;
			if (edtShowTimeAs != null)
				Controller.StatusKey = Convert.ToInt32(edtShowTimeAs.Value);
			if (edtLabel != null)
				Controller.LabelKey = Convert.ToInt32(edtLabel.Value);
			AssignControllerResources(edtResource, ddResource);
			if (chkReminder != null) {
				if (chkReminder.Checked) {
					TimeSpan reminderTime = TimeSpan.Parse((string)cbReminder.Value);
					Controller.HasReminder = true;
					Controller.ReminderTimeBeforeStart = reminderTime;
				} else
					Controller.HasReminder = false;
			}
			AssignControllerRecurrenceValues(clientStart);
		}
		protected virtual void AssignControllerResources(ASPxComboBox edtResource, ASPxDropDownEdit ddResource) {
			if (Control.Storage.ResourceSharing) {
				if (ddResource != null) {
					ASPxListBox edtMultiResource = ddResource.FindControl("edtMultiResource") as ASPxListBox;
					if (edtMultiResource != null) {
						Controller.ResourceIds.Clear();
						foreach (ListEditItem item in edtMultiResource.SelectedItems) {
							string resourceIdString = item.Value.ToString();
							object resourceId = CalculateResouceId(resourceIdString);
							Controller.ResourceIds.Add(resourceId);
						}
					}
				}
			} else if (edtResource != null) {
				string resourceIdString = (string)edtResource.Value;
				Controller.ResourceId = CalculateResouceId(resourceIdString);
			}
		}
		protected internal virtual object CalculateResouceId(string resourceIdString) {
			Type type = SchedulerIdHelper.GetResourceIdType(Control.Storage.Resources);
			return SchedulerIdHelper.ConvertToResourceId(resourceIdString, type);
		}
		protected internal virtual void AssignControllerRecurrenceValues(DateTime clientStart) {
			AppointmentRecurrenceForm frm = FindRecurrenceFormByID();
			AppointmentRecurrenceControl recurrenceControl = GetAppointmentRecurrenceControlByForm(frm);
			if (recurrenceControl == null || !ShouldShowRecurrence())
				return;
			if (ShouldCreateRecurrence(frm) && IsRecurrenceValid(recurrenceControl, clientStart))
				ApplyRecurrence(recurrenceControl, clientStart);
			else
				Controller.RemoveRecurrence();
		}
		protected virtual AppointmentRecurrenceForm FindRecurrenceFormByID() {
			return FindControlByID("AppointmentRecurrenceForm1") as AppointmentRecurrenceForm;
		}
		protected virtual AppointmentRecurrenceControl FindRecurrenceControlByID() {
			return FindControlByID("RecurrencePanel" + Control.ProtectedIdSeparator + "RecurrenceControl1") as AppointmentRecurrenceControl;
		}
		protected virtual AppointmentRecurrenceControl GetAppointmentRecurrenceControlByForm(AppointmentRecurrenceForm form) {
			return (form == null) ? FindRecurrenceControlByID() : form.RecurrenceControl;
		}
		protected internal virtual bool ShouldShowRecurrence() {
			AppointmentFormTemplateContainer c = TemplateContainer as AppointmentFormTemplateContainer;
			return c != null ? c.ShouldShowRecurrence : false;
		}
		protected virtual bool ShouldCreateRecurrence(AppointmentRecurrenceForm frm) {
			if (frm != null)
				return frm.ClientIsRecurrence;
			ASPxCheckBox chkIsRecurrence = FindControlByID("chkRecurrence") as ASPxCheckBox;
			if (chkIsRecurrence == null)
				return false;
			return chkIsRecurrence.Checked;
		}
		[Obsolete("You should use the 'AssignRecurrenceInfoProperties(AppointmentRecurrenceControl recurrenceControl)' methods instead", false)]
		protected virtual bool IsRecurrenceValid(AppointmentRecurrenceForm frm, DateTime clientStart) {
			return IsRecurrenceValid(frm.RecurrenceControl, clientStart);
		}
		protected virtual bool IsRecurrenceValid(AppointmentRecurrenceControl recurrenceControl, DateTime clientStart) {
			DevExpress.XtraScheduler.UI.ValidationArgs args = new DevExpress.XtraScheduler.UI.ValidationArgs();
			ValidateValues(recurrenceControl, args, clientStart);
			if (!args.Valid) {
				IsFormDataError = true;
				SchedulerStatusInfoHelper.AddError(Control, args.ErrorMessage);
				return false;
			}
			args = new DevExpress.XtraScheduler.UI.ValidationArgs();
			CheckForWarnings(recurrenceControl, args);
			if (!args.Valid)
				SchedulerStatusInfoHelper.AddWarning(Control, args.ErrorMessage);
			return true;
		}
		[Obsolete("You should use the 'ApplyRecurrence(AppointmentRecurrenceControl recurrenceControl, DateTime clientStart)' methods instead", false)]
		protected virtual void ApplyRecurrence(AppointmentRecurrenceForm frm, DateTime clientStart) {
			Appointment patternCopy = Controller.PrepareToRecurrenceEdit();
			AssignRecurrenceInfoProperties(frm, patternCopy, patternCopy.RecurrenceInfo, clientStart);
			Controller.ApplyRecurrence(patternCopy);
		}
		protected virtual void ApplyRecurrence(AppointmentRecurrenceControl recurrenceControl, DateTime clientStart) {
			Appointment patternCopy = Controller.PrepareToRecurrenceEdit();
			AssignRecurrenceInfoProperties(Control.TimeZoneHelper, recurrenceControl, patternCopy, patternCopy.RecurrenceInfo, clientStart);
			Controller.ApplyRecurrence(patternCopy);
		}
		[Obsolete("You should use the 'AssignRecurrenceInfoProperties(AppointmentRecurrenceControl recurrenceControl, Appointment patternCopy, RecurrenceInfo rinfo, DateTime clientStart)' methods instead", false)]
		protected virtual void AssignRecurrenceInfoProperties(AppointmentRecurrenceForm frm, Appointment patternCopy, IRecurrenceInfo rinfo, DateTime clientStart) {
			AssignRecurrenceInfoProperties(Control.TimeZoneHelper, frm.RecurrenceControl, patternCopy, rinfo, clientStart);
		}
		protected internal static void AssignRecurrenceInfoProperties(TimeZoneHelper timeZoneHelper, AppointmentRecurrenceControl recurrenceControl, Appointment patternCopy, IRecurrenceInfo rinfo, DateTime clientStart) {
			rinfo.Type = recurrenceControl.ClientRecurrenceType;
			DateTime end = recurrenceControl.ClientEnd;
			if (!patternCopy.AllDay && timeZoneHelper != null)
				end = timeZoneHelper.FromClientTime(end);
			AssignRecurrenceInfoRangeProperties(rinfo, recurrenceControl.ClientRecurrenceRange, clientStart, end, recurrenceControl.ClientOccurrenceCount, patternCopy);
			rinfo.DayNumber = recurrenceControl.ClientDayNumber;
			rinfo.Periodicity = recurrenceControl.ClientPeriodicity;
			rinfo.Month = recurrenceControl.ClientMonth;
			rinfo.WeekDays = recurrenceControl.ClientWeekDays;
			rinfo.WeekOfMonth = recurrenceControl.ClientWeekOfMonth;
		}
		protected static void AssignRecurrenceInfoRangeProperties(IRecurrenceInfo rinfo, RecurrenceRange range, DateTime start, DateTime end, int occurrenceCount, Appointment patternCopy) {
			((IInternalRecurrenceInfo)rinfo).UpdateRange(start, end, range, occurrenceCount, patternCopy);
		}
		[Obsolete("You should use the 'ValidateValues(AppointmentRecurrenceControl recurrenceControl, DevExpress.XtraScheduler.UI.ValidationArgs args)' methods instead", false)]
		protected virtual void ValidateValues(AppointmentRecurrenceForm frm, DevExpress.XtraScheduler.UI.ValidationArgs args) {
			ValidateValues(frm.RecurrenceControl, args, DateTime.MinValue);
		}
		protected virtual void ValidateValues(AppointmentRecurrenceControl recurrenceControl, DevExpress.XtraScheduler.UI.ValidationArgs args, DateTime clientStart) {
			ISupportRecurrenceValidator controlWithValidator = recurrenceControl as ISupportRecurrenceValidator;
			if (controlWithValidator != null)
				controlWithValidator.Validator.Start = clientStart;
			recurrenceControl.ValidateValues(args);
		}
		[Obsolete("You should use the 'CheckForWarnings(AppointmentRecurrenceControl recurrenceControl, DevExpress.XtraScheduler.UI.ValidationArgs args)' methods instead", false)]
		protected virtual void CheckForWarnings(AppointmentRecurrenceForm frm, DevExpress.XtraScheduler.UI.ValidationArgs args) {
			CheckForWarnings(frm.RecurrenceControl, args);
		}
		protected virtual void CheckForWarnings(AppointmentRecurrenceControl recurrenceControl, DevExpress.XtraScheduler.UI.ValidationArgs args) {
			recurrenceControl.CheckForWarnings(args);
		}
	}
	#endregion
	#region ShowAppointmentFormByServerIdCallbackCommand
	public class ShowAppointmentFormByServerIdCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		string appointmentIdString;
		Appointment editedAppointment;
		#endregion
		public ShowAppointmentFormByServerIdCallbackCommand(ASPxScheduler control)
			: base(control) {
			this.appointmentIdString = SchedulerIdHelper.NewAppointmentId;
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.ShowAppointmentFormByServerId; } }
		public string AppointmentIdString { get { return appointmentIdString; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			this.appointmentIdString = parameters;
			this.editedAppointment = Control.LookupAppointmentByServerIdString(AppointmentIdString);
		}
		protected internal override void ExecuteCore() {
			if (this.editedAppointment == null)
				return; 
			WebEditAppointmentCommand editCommand = new WebEditAppointmentCommand(Control);
			editCommand.Execute(this.editedAppointment);
			Control.IgnoreFormPostData = true;
		}
		protected internal override bool CanContinueExecute() {
			if (this.editedAppointment == null)
				return false;
			WebEditAppointmentCommand editCommand = new WebEditAppointmentCommand(Control);
			return editCommand.ProtectedCanExecute(this.editedAppointment);
		}
	}
	#endregion
	#region ShowAppointmentFormByClientIdCallbackCommand
	public class ShowAppointmentFormByClientIdCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		string appointmentIdString;
		Appointment editedAppointment;
		#endregion
		public ShowAppointmentFormByClientIdCallbackCommand(ASPxScheduler control)
			: base(control) {
			this.appointmentIdString = SchedulerIdHelper.NewAppointmentId;
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentFormShow; } }
		public string AppointmentIdString { get { return appointmentIdString; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			this.appointmentIdString = parameters;
			this.editedAppointment = Control.LookupAppointmentByIdString(AppointmentIdString);
		}
		protected internal override void ExecuteCore() {
			if (this.editedAppointment == null)
				return; 
			WebEditAppointmentCommand editCommand = CreateEditCommand();
			editCommand.Execute(this.editedAppointment);
			Control.IgnoreFormPostData = true;
		}
		protected internal override bool CanContinueExecute() {
			if (this.editedAppointment == null)
				return false;
			WebEditAppointmentCommand command = CreateEditCommand();
			return command.ProtectedCanExecute(this.editedAppointment);
		}
		protected virtual WebEditAppointmentCommand CreateEditCommand() {
			return new WebEditAppointmentCommand(Control);
		}
	}
	#endregion
	#region SchedulerFormCloseCallbackCommand
	public abstract class SchedulerFormCloseCallbackCommand : SchedulerFormCallbackCommand {
		protected SchedulerFormCloseCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		protected internal override bool CanCloseForm { get { return true; } }
		protected internal override void ParseParameters(string parameters) {
		}
		protected internal override void ExecuteCore() {
		}
	}
	#endregion
	#region AppointmentCancelCallbackCommand
	public class AppointmentCancelCallbackCommand : SchedulerFormCloseCallbackCommand {
		public AppointmentCancelCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentCancel; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.AppointmentFormVisibility; } }
		protected override string FormId { get { return SchedulerFormNames.AppointmentForm; } }
	}
	#endregion
	#region AppointmentDeleteCallbackCommand
	public class AppointmentDeleteCallbackCommand : SchedulerFormCallbackCommand {
		AppointmentFormController controller;
		public AppointmentDeleteCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentDelete; } }
		protected internal AppointmentFormController Controller { get { return controller; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.AppointmentFormVisibility; } }
		protected override string FormId { get { return SchedulerFormNames.AppointmentForm; } }
		protected internal override void ParseParameters(string parameters) {
			Appointment apt = Control.GetEditedAppointment();
			if (apt != null)
				this.controller = CreateAppointmentFormController(apt);
		}
		protected virtual AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new AppointmentFormController(Control, apt);
		}
		protected internal override void ExecuteCore() {
			if (Controller != null)
				Controller.DeleteAppointment();
		}
		protected internal override bool CanContinueExecute() {
			if (Controller == null)
				return false;
			return controller.CanDeleteAppointment;
		}
	}
	#endregion
	#region AppointmentInplaceEditorShowCallbackCommand
	public class AppointmentInplaceEditorShowCallbackCommand : SchedulerCallbackCommand {
		public AppointmentInplaceEditorShowCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.InplaceEditorShow; } }
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
		}
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			if (Control.SelectedAppointments.Count == 1) {
				Appointment apt = Control.SelectedAppointments[0];
				Control.SetEditableAppointment(apt);
				Control.ActiveFormType = SchedulerFormType.AppointmentInplace;
			} else
				Control.ProtectedResetControlHierarchy();
		}
		#endregion
	}
	#endregion
	public interface IDefaultUserData {
		object GetProperty(string name);
	}
	public class DefaultUserData : IDefaultUserData {
		string subject;
		public DefaultUserData(string subject) {
			this.subject = subject;
		}
		public string Subject {
			get { return subject; }
			set { subject = value; }
		}
		public object GetProperty(string name) {
			if (name == "Subject")
				return Subject;
			return null;
		}
	}
	#region AppointmentInplaceEditorEditFormCallbackCommand
	public class AppointmentInplaceEditorEditFormCallbackCommand : AppointmentInplaceEditorSaveCallbackCommand {
		public AppointmentInplaceEditorEditFormCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.InplaceEditorEditForm; } }
		#endregion
		#region ApplyChanges
		protected internal override void ApplyChanges() {
		}
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			if (IsLostAppointmentDetected) {
				CanCloseForm = true;
				Control.ActiveFormType = SchedulerFormType.None;
				return;
			}
			if (Control.OptionsForms.AppointmentFormVisibility != SchedulerFormVisibility.None) {
				Control.ActiveFormType = SchedulerFormType.Appointment;
				if (IsNewAppointment) {
					Control.SetEditableAppointment(Controller.EditedAppointmentCopy);
				} else
					Control.SetEditableAppointment(Controller.SourceAppointment, new DefaultUserData(Controller.Subject));
			} else
				Control.ActiveFormType = SchedulerFormType.None;
		}
		#endregion
		#region FinalizeExecute
		protected internal override void FinalizeExecute() {
			if (Control.OptionsForms.AppointmentFormVisibility == SchedulerFormVisibility.FillControlArea)
				Control.EnsureProcessToolbarBlocks();
		}
		#endregion
	}
	#endregion
	#region AppointmentInplaceEditorSaveCallbackCommand
	public class AppointmentInplaceEditorSaveCallbackCommand : AppointmentSaveCallbackCommand {
		DateTime start;
		DateTime end;
		AppointmentResourceIdCollection resourceIds;
		protected const char ResourcesIdSeparator = ',';
		public AppointmentInplaceEditorSaveCallbackCommand(ASPxScheduler control)
			: base(control) {
			this.resourceIds = new AppointmentResourceIdCollection();
		}
		#region Properties
		public override bool RequiresControlHierarchy { get { return true; } }
		public override string Id { get { return SchedulerCallbackCommandId.InplaceEditorSave; } }
		public DateTime Start { get { return start; } }
		public DateTime End { get { return end; } }
		public AppointmentResourceIdCollection ResourceIds { get { return resourceIds; } }
		protected override string FormId { get { return SchedulerFormNames.InplaceEditorForm; } }
		#endregion
		#region AssignControllerValues
		protected internal override void AssignControllerValues() {
			ASPxMemo tbSubject = (ASPxMemo)FindControlByID("memSubject");
			if (tbSubject != null)
				Controller.Subject = tbSubject.Text;
		}
		#endregion
		#region GetEditedAppointment
		protected internal override Appointment GetEditedAppointment() {
			if (!IsNewAppointment)
				return base.GetEditedAppointment();
			ParseBehindAppointmentData();
			return CreateRestoredAppointment();
		}
		protected internal virtual void ParseBehindAppointmentData() {
			HiddenField startDateHiddenField = FindControlByID(InplaceEditorHiddenFieldId.StartDate) as HiddenField;
			HiddenField endDateHiddenField = FindControlByID(InplaceEditorHiddenFieldId.EndDate) as HiddenField;
			HiddenField resourceIdsHiddenField = FindControlByID(InplaceEditorHiddenFieldId.ResourceId) as HiddenField;
			if (startDateHiddenField != null && endDateHiddenField != null && resourceIdsHiddenField != null) {
				DateTime start = DateTime.Parse(startDateHiddenField.Value, CultureInfo.InvariantCulture);
				DateTime end = DateTime.Parse(endDateHiddenField.Value, CultureInfo.InvariantCulture);
				SetBehindAppointmentData(start, end, resourceIdsHiddenField.Value);
			}
		}
		protected void SetBehindAppointmentData(DateTime start, DateTime end, string resourceIdsString) {
			this.start = start;
			this.end = end;
			foreach (string resourceId in resourceIdsString.Split(ResourcesIdSeparator)) {
				XtraScheduler.Resource resource = Control.LookupResourceByIdString(resourceId);
				if (resource != null)
					ResourceIds.Add(resource.Id);
			}
		}
		protected virtual Appointment CreateRestoredAppointment() {
			Appointment appointment = Control.Storage.CreateAppointment(Control.EditableAppointment.Type);
			appointment.Start = Start;
			appointment.End = End;
			appointment.ResourceIds.AddRange(ResourceIds);
			return appointment;
		}
		#endregion
	}
	#endregion
	#region GotoDateCallbackCommand
	public class GotoDateCallbackCommand : SchedulerCallbackCommand {
		#region Fields
		internal DateTime newDate;
		#endregion
		public GotoDateCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override bool RequiresControlHierarchy { get { return true; } }
		public DateTime NewDate { get { return newDate; } }
		public override string Id { get { return SchedulerCallbackCommandId.GotoDate; } }
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
			this.newDate = SchedulerWebUtils.ToDateTime(parameters);
		}
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)Control.InnerControl.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(NewDate);
		}
		#endregion
	}
	#endregion
	#region GotoDateFormCallbackCommand
	public class GotoDateFormCallbackCommand : SchedulerFormCallbackCommand {
		#region Fields
		internal DateTime newDate;
		internal SchedulerViewType newViewType;
		#endregion
		public GotoDateFormCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override bool RequiresControlHierarchy { get { return true; } }
		public override string Id { get { return SchedulerCallbackCommandId.GotoDateForm; } }
		public DateTime NewDate { get { return newDate; } }
		public SchedulerViewType NewViewType { get { return newViewType; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.GotoDateFormVisibility; } }
		protected override string FormId { get { return SchedulerFormNames.GotoDateForm; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			ASPxDateEdit edtDate = FindControl("edtDate") as ASPxDateEdit;
			if (edtDate == null)
				return;
			this.newDate = edtDate.Date;
			this.newViewType = Control.ActiveViewType;
			ASPxComboBox cbView = FindControl("cbView") as ASPxComboBox;
			if (cbView != null) {
				string viewValue = (string)cbView.Value;
				this.newViewType = (SchedulerViewType)Enum.Parse(typeof(SchedulerViewType), viewValue);
			}
		}
		protected internal override void ExecuteCore() {
			IDateTimeNavigationService service = (IDateTimeNavigationService)Control.InnerControl.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(NewDate, NewViewType);
		}
	}
	#endregion
	#region NewAppointmentViaInplaceEditor
	public class NewAppointmentViaInplaceEditor : SchedulerCallbackCommand {
		DateTime start;
		DateTime end;
		XtraScheduler.Resource resource;
		public NewAppointmentViaInplaceEditor(ASPxScheduler control)
			: base(control) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.NewAppointmentViaInplaceEditor; } }
		public DateTime Start { get { return start; } }
		public DateTime End { get { return end; } }
		public Resource Resource { get { return resource; } }
		protected internal override void ParseParameters(string parameters) {
			string[] stringDates = parameters.Split(new char[] { ',' });
			if (stringDates.Length < 2)
				throw new ArgumentException("ParseParameters", "parameters");
			this.start = SchedulerWebUtils.ToDateTime(stringDates[0]);
			this.end = SchedulerWebUtils.ToDateTime(stringDates[1]);
			if (stringDates.Length > 2)
				this.resource = Control.LookupResourceByIdString(stringDates[2]);
			else
				this.resource = ResourceBase.Empty;
		}
		protected internal override void ExecuteCore() {
			ISelectionService selectionService = (ISelectionService)Control.GetService(typeof(ISelectionService));
			XtraScheduler.Resource newSelectedResource = (this.Resource == null) ? selectionService.SelectedResource : this.Resource;
			selectionService.SetSelection(new TimeInterval(Start, End), newSelectedResource);
			IAppointmentSelectionService appointmentSelectionService = (IAppointmentSelectionService)Control.GetService(typeof(IAppointmentSelectionService));
			appointmentSelectionService.ClearSelection();
			WebNewAppointmentViaInplaceEditorCommand command = new WebNewAppointmentViaInplaceEditorCommand(Control, new TimeInterval(Start, End), Resource);
			command.Execute();
		}
		protected internal override bool CanContinueExecute() {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			return helper.CanCreateAppointmentViaInplaceEditor(new TimeInterval(this.Start, this.End), (XtraScheduler.Resource)this.Resource);
		}
	}
	#endregion
	#region GotoDateCancelCallbackCommand
	public class GotoDateCancelCallbackCommand : SchedulerFormCloseCallbackCommand {
		public GotoDateCancelCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.GotoDateFormCancel; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.GotoDateFormVisibility; } }
		protected override string FormId { get { return SchedulerFormNames.GotoDateForm; } }
		#endregion
	}
	#endregion
	#region RecurrentAppointmentDeleteCallbackCommand
	public class RecurrentAppointmentDeleteCallbackCommand : SchedulerFormCallbackCommand {
		RecurrentAppointmentAction action;
		public RecurrentAppointmentDeleteCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		protected internal override bool CanCloseForm { get { return true; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.RecurrentAppointmentDeleteFormVisibility; } }
		public override string Id { get { return SchedulerCallbackCommandId.RecurrentAppointmentDelete; } }
		protected internal RecurrentAppointmentAction Action { get { return action; } set { action = value; } }
		protected override string FormId { get { return SchedulerFormNames.RecurrentAppointmentDeleteForm; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			RecurrentAppointmentDeleteFormTemplateContainer container = (RecurrentAppointmentDeleteFormTemplateContainer)Control.FormBlock.CurrentFormTemplateContainer;
			ASPxRadioButtonList rbList = (ASPxRadioButtonList)container.FindControl(FormCallbackCommandHelper.GetDefaultInputPath("rbAction", Control.ProtectedIdSeparator));
			if (rbList != null)
				Action = (RecurrentAppointmentAction)rbList.Value;
		}
		protected internal override void ExecuteCore() {
			Appointment apt = Control.GetEditedAppointment();
			if (apt != null)
				switch (Action) {
				case RecurrentAppointmentAction.Occurrence:
					apt.Delete();
					break;
				case RecurrentAppointmentAction.Series:
					Appointment pattern = apt.RecurrencePattern;
					if (pattern != null)
						pattern.Delete();
					break;
				default:
					break;
				}
		}
		protected internal override void FinalizeExecute() {
			base.FinalizeExecute();
			if (!CanContinueExecute(Control.SelectedAppointments))
				return;
			DeleteSelectedAppointments();
		}
		protected virtual void DeleteSelectedAppointments() {
			if (Control.SelectedAppointments.Count == 0)
				return;
			WebDeleteAppointmentsCommand deleteCommand = new WebDeleteAppointmentsCommand(Control);
			deleteCommand.Execute();
		}
		protected internal override bool CanContinueExecute() {
			Appointment editedAppointment = Control.GetEditedAppointment();
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			switch (Action) {
			case RecurrentAppointmentAction.Occurrence:
				return helper.CanDeleteAppointment(editedAppointment);
			case RecurrentAppointmentAction.Series:
				Appointment pattern = editedAppointment.RecurrencePattern;
				if (pattern == null)
					return false;
				return helper.CanDeleteAppointment(editedAppointment);
			default:
				return false;
			}
		}
		bool CanContinueExecute(AppointmentBaseCollection appointments) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			return helper.CanDeleteAppointments(appointments);
		}
	}
	#endregion
	#region RecurrentAppointmentDeleteCancelCallbackCommand
	public class RecurrentAppointmentDeleteCancelCallbackCommand : SchedulerFormCloseCallbackCommand {
		public RecurrentAppointmentDeleteCancelCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.RecurrentAppointmentDeleteCancel; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.RecurrentAppointmentDeleteFormVisibility; } }
		protected override string FormId { get { return SchedulerFormNames.RecurrentAppointmentDeleteForm; } }
		#endregion
	}
	#endregion
	#region RecurrentAppointmentEditCallbackCommand
	public class RecurrentAppointmentEditCallbackCommand : SchedulerFormCallbackCommand {
		RecurrentAppointmentAction action;
		public RecurrentAppointmentEditCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.RecurrentAppointmentEditFormVisibility; } }
		public override string Id { get { return SchedulerCallbackCommandId.RecurrentAppointmentEdit; } }
		protected internal RecurrentAppointmentAction Action { get { return action; } set { action = value; } }
		protected override string FormId { get { return SchedulerFormNames.RecurrentAppointmentEditForm; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			RecurrentAppointmentEditFormTemplateContainer container = (RecurrentAppointmentEditFormTemplateContainer)Control.FormBlock.CurrentFormTemplateContainer;
			ASPxRadioButtonList rbList = (ASPxRadioButtonList)container.FindControl(FormCallbackCommandHelper.GetDefaultInputPath("rbAction", Control.ProtectedIdSeparator));
			if (rbList != null)
				Action = (RecurrentAppointmentAction)rbList.Value;
		}
		protected internal override void ExecuteCore() {
			WebEditRecurrentAppointmentCommand command = CreateEditCommand();
			command.Execute();
			CanCloseForm = (Control.ActiveFormType == SchedulerFormType.RecurrentAppointmentEdit);
		}
		protected internal override bool CanContinueExecute() {
			WebEditRecurrentAppointmentCommand command = CreateEditCommand();
			return command.CanExecute();
		}
		protected virtual WebEditRecurrentAppointmentCommand CreateEditCommand() {
			return new WebEditRecurrentAppointmentCommand(Control, Action);
		}
	}
	#endregion
	#region RecurrentAppointmentEditCancelCallbackCommand
	public class RecurrentAppointmentEditCancelCallbackCommand : SchedulerFormCloseCallbackCommand {
		public RecurrentAppointmentEditCancelCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.RecurrentAppointmentEditCancel; } }
		protected internal override SchedulerFormVisibility FormVisibility { get { return Control.OptionsForms.RecurrentAppointmentEditFormVisibility; } }
		protected override string FormId { get { return SchedulerFormNames.RecurrentAppointmentEditForm; } }
		#endregion
	}
	#endregion
	#region ProcessRemindersCallbackCommand
	public class ProcessRemindersCallbackCommand : SchedulerCallbackCommand {
		public ProcessRemindersCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.ProcessReminder; } }
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
		}
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			ReminderState reminderState = Control.ReminderState;
			reminderState.LoadExpiredReminders();
			if (reminderState.ClientReminderIdCollection.Count > 0) {
				Control.ShowForm(SchedulerFormType.Reminder);
				reminderState.IsRequireSetClientTimer = false; 
			}
		}
		#endregion
	}
	#endregion
	#region RemindersFormCallbackCommandBase
	public abstract class RemindersFormCallbackCommandBase : SchedulerCallbackCommand {
		#region Fields
		RemindersFormTemplateContainer templateContainer;
		ReminderState reminderState;
		FormCallbackCommandHelper formCallbackCommandHelper;
		#endregion
		protected RemindersFormCallbackCommandBase(ASPxScheduler control)
			: base(control) {
			this.formCallbackCommandHelper = new FormCallbackCommandHelper(SchedulerFormNames.RemindersForm);
		}
		#region Properties
		#region TemplateContainer
		protected internal RemindersFormTemplateContainer TemplateContainer {
			get {
				if (templateContainer == null)
					templateContainer = (RemindersFormTemplateContainer)Control.FormBlock.CurrentFormTemplateContainer;
				return templateContainer;
			}
		}
		#endregion
		#region ReminderState
		public ReminderState ReminderState {
			get {
				if (reminderState == null) {
					reminderState = Control.ReminderState;
				}
				return reminderState;
			}
		}
		#endregion
		protected virtual FormCallbackCommandHelper FormCallbackCommandHelper { get { return formCallbackCommandHelper; } }
		#endregion
		#region FindControlByID
		protected internal virtual Control FindControlByID(string id) {
			return TemplateContainer.FindControl(FormCallbackCommandHelper.GetDefaultInputPath(id, Control.ProtectedIdSeparator));
		}
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			ProcessReminders();
			CloseIfEmpty();
		}
		#endregion
		#region CloseIfEmpty
		protected internal virtual void CloseIfEmpty() {
			if (IsCloseReminderForm()) {
				Control.ActiveFormType = SchedulerFormType.None;
				ReminderState.IsRequireSetClientTimer = true;
			}
		}
		protected virtual bool IsCloseReminderForm() {
			return ReminderState.ClientReminderIdCollection.Count <= 0;
		}
		#endregion
		abstract protected internal void ProcessReminders();
	}
	#endregion
	#region ItemSpecificRemindersFormCallbackCommandBase
	public abstract class ItemSpecificRemindersFormCallbackCommandBase : RemindersFormCallbackCommandBase {
		#region Fields
		ReminderCollection reminders;
		#endregion
		protected ItemSpecificRemindersFormCallbackCommandBase(ASPxScheduler control)
			: base(control) {
			this.reminders = new ReminderCollection();
		}
		#region Reminders
		public ReminderCollection Reminders { get { return reminders; } }
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
			Reminders.Clear();
			ParseSelectedReminders();
		}
		#endregion
		#region ParseSelectedReminders
		protected internal virtual void ParseSelectedReminders() {
			List<int> indexes = GetSelectedIndexes();
			int count = indexes.Count;
			for (int i = 0; i < count; i++) {
				int index = indexes[i];
				if (index == -1)
					continue;
				XtraSchedulerDebug.Assert(index >= 0 && index < ReminderState.ClientReminderIdCollection.Count);
				ReminderCollection reminders = ReminderState.GetExpiredReminders();
				Reminders.Add(reminders[index]);
			}
		}
		#endregion
		#region ProcessReminders
		protected internal override void ProcessReminders() {
			Reminders.ForEach(ProcessReminder);
			ReminderState.LoadExpiredReminders();
		}
		#endregion
		#region ProcessReminder
		protected internal virtual void ProcessReminder(Reminder reminder) {
			ReminderState.RemoveReminder(reminder);
			ProcessReminderCore(reminder);
		}
		#endregion
		#region GetSelectedIndexes
		protected virtual List<int> GetSelectedIndexes() {
			List<int> result = new List<int>();
			ASPxListBox lbItems = (ASPxListBox)FindControlByID("lbItems");
			int index = lbItems.SelectedIndex;
			result.Add(index);
			return result;
		}
		#endregion
		#region CanContinueExecute
		protected internal override bool CanContinueExecute() {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			int count = Reminders.Count;
			AppointmentBaseCollection appointments = new AppointmentBaseCollection();
			for (int i = 0; i < count; i++) {
				Appointment appointment = Reminders[i].Appointment;
				appointments.Add(appointment);
			}
			return helper.CanEditAppointments(appointments);
		}
		#endregion
		abstract protected internal void ProcessReminderCore(Reminder reminder);
	}
	#endregion
	#region DismissReminderCallbackCommand
	public class DismissReminderCallbackCommand : ItemSpecificRemindersFormCallbackCommandBase {
		public DismissReminderCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.DismissReminder; } }
		#endregion
		#region ProcessReminderCore
		protected internal override void ProcessReminderCore(Reminder reminder) {
			reminder.Dismiss();
		}
		#endregion
	}
	#endregion
	#region DismissAllRemindersCallbackCommand
	public class DismissAllRemindersCallbackCommand : DismissReminderCallbackCommand {
		public DismissAllRemindersCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.DismissAllReminders; } }
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
			Reminders.AddRange(ReminderState.GetExpiredReminders());
		}
		#endregion
	}
	#endregion
	#region SnoozeReminderCallbackCommand
	public class SnoozeReminderCallbackCommand : ItemSpecificRemindersFormCallbackCommandBase {
		TimeSpan snoozeSpan;
		DateTime now;
		public SnoozeReminderCallbackCommand(ASPxScheduler control)
			: base(control) {
			this.now = DateTime.Now;
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.SnoozeReminders; } }
		public TimeSpan SnoozeSpan { get { return snoozeSpan; } }
		public DateTime Now { get { return now; } }
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
			base.ParseParameters(parameters);
			ParseSnoozeSpanValue();
		}
		#endregion
		protected internal virtual void ParseSnoozeSpanValue() {
			this.snoozeSpan = GetSnoozeSpanValue();
		}
		protected internal virtual TimeSpan GetSnoozeSpanValue() {
			ASPxComboBox cbSnooze = (ASPxComboBox)FindControlByID("cbSnooze");
			XtraSchedulerDebug.Assert(cbSnooze.Value != null && cbSnooze.Value is string);
			return TimeSpan.Parse((string)cbSnooze.Value);
		}
		#region ProcessReminderCore
		protected internal override void ProcessReminderCore(Reminder reminder) {
			if (SnoozeSpan.Ticks < 0) {
				if (-SnoozeSpan.Ticks > reminder.Appointment.Start.Ticks)
					reminder.AlertTime = DateTime.MinValue;
				else
					reminder.AlertTime = reminder.Appointment.Start + SnoozeSpan.Negate();
			} else
				reminder.AlertTime = Now + SnoozeSpan;
			(reminder).Suspended = false;
		}
		#endregion
	}
	#endregion
	#region SnoozeAllRemindersCallbackCommand
	public class SnoozeAllRemindersCallbackCommand : SnoozeReminderCallbackCommand {
		public SnoozeAllRemindersCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.SnoozeAllReminders; } }
		#endregion
		#region ParseSelectedReminders
		protected internal override void ParseSelectedReminders() {
			Reminders.AddRange(ReminderState.GetExpiredReminders());
		}
		#endregion
	}
	#endregion
	#region CustomActionRemindersFormCallbackCommand
	public class CustomActionRemindersFormCallbackCommand : ItemSpecificRemindersFormCallbackCommandBase {
		public CustomActionRemindersFormCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.CustomRemindersAction; } }
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			ReminderAlertNotificationCollection alertNotifications = new ReminderAlertNotificationCollection();
			((IInternalSchedulerStorageBase)Control.Storage).RemindersEngine.FillAlertNotifications(alertNotifications, Reminders);
			RemindersFormDefaultActionEventArgs args = new RemindersFormDefaultActionEventArgs(alertNotifications);
			Control.InnerControl.RaiseRemindersFormDefaultAction(args);
			if (!args.Cancel) {
				Control.ActiveFormType = SchedulerFormType.None;
				ReminderState.IsRequireSetClientTimer = true;
			}
		}
		#endregion
		protected internal override void ProcessReminderCore(Reminder reminder) {
		}
	}
	#endregion
	#region CloseRemindersFormCallbackCommand
	public class CloseRemindersFormCallbackCommand : SchedulerCallbackCommand {
		SchedulerCallbackCommand command;
		public CloseRemindersFormCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.CloseRemindersForm; } }
		protected SchedulerCallbackCommand Command {
			get {
				if (command == null)
					command = CreateCloseReminderFormCommand();
				return command;
			}
		}
		protected virtual SchedulerCallbackCommand CreateCloseReminderFormCommand() {
			switch (Control.OptionsBehavior.RemindersFormDefaultAction) {
			case RemindersFormDefaultAction.DismissAll:
				return CreateDismissAllRemindersCallbackCommand();
			case RemindersFormDefaultAction.SnoozeAll:
				return CreateSnoozeAllRemindersCallbackCommand();
			case RemindersFormDefaultAction.Custom:
				return new CustomActionRemindersFormCallbackCommand(Control);
			}
			return null;
		}
		protected virtual DismissAllRemindersCallbackCommand CreateDismissAllRemindersCallbackCommand() {
			return new DismissAllRemindersCallbackCommand(Control);
		}
		protected virtual SnoozeAllRemindersCallbackCommand CreateSnoozeAllRemindersCallbackCommand() {
			return new SnoozeAllRemindersCallbackCommand(Control);
		}
		#endregion
		#region ParseParameters
		protected internal override void ParseParameters(string parameters) {
		}
		#endregion
		#region ExecuteCore
		protected internal override void ExecuteCore() {
			if (Command != null)
				Command.Execute(String.Empty);
		}
		#endregion
		protected internal override bool CanContinueExecute() {
			if (Command == null)
				return false;
			return Command.CanContinueExecute();
		}
	}
	#endregion
	#region AppointmentsChangeCommand
	public class AppointmentsChangeCommand : SchedulerCallbackCommand {
		class AppointmentChanges : Dictionary<string, string> {
		}
		class AppointmentsChanges : Dictionary<string, AppointmentChanges> {
		}
		AppointmentsChanges appointmentsChanges;
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentsChange; } }
		public AppointmentsChangeCommand(ASPxScheduler control)
			: base(control) {
			appointmentsChanges = new AppointmentsChanges();
		}
		protected internal override void ParseParameters(string parameters) {
			string[] changes = parameters.Split('!');
			int count = changes.Length;
			for (int i = 0; i < count; i++) {
				string appointmentId;
				AppointmentChanges appointmentChanges;
				if (ParseAppointmentChanges(changes[i], out appointmentId, out appointmentChanges))
					if (!appointmentsChanges.ContainsKey(appointmentId))
						appointmentsChanges.Add(appointmentId, appointmentChanges);
			}
		}
		bool ParseAppointmentChanges(string parameters, out string appointmentId, out AppointmentChanges appointmentChanges) {
			appointmentId = String.Empty;
			appointmentChanges = null;
			string[] changes = parameters.Split('?');
			int count = changes.Length;
			if (count < 1)
				return false;
			appointmentId = changes[0];
			appointmentChanges = new AppointmentChanges();
			for (int i = 1; i < count; i++) {
				string[] values = changes[i].Split('=');
				if (values.Length != 2)
					return false;
				string name = values[0];
				string value = values[1];
				appointmentChanges.Add(name, value);
			}
			return true;
		}
		protected virtual void ApplyChange(Appointment appointment, string name, string value) {
			if (name == "START") {
				ApplyStartChange(appointment, value);
				return;
			}
			if (name == "DURATION") {
				ApplyDurationChange(appointment, value);
				return;
			}
			if (name == "RESOURCES") {
				ApplyResourcesChange(appointment, value);
				return;
			}
			if (name == "COPY")
				return;
			if (name == "MADA") {
				ApplyAllDayChange(appointment, value);
				return;
			}
			Exceptions.ThrowArgumentException("name", name);
		}
		protected virtual void ApplyStartChange(Appointment appointment, string value) {
			TimeZoneHelper timeZoneEngine = Control.InnerControl.TimeZoneHelper;
			DateTime newStart = timeZoneEngine.FromClientTime(SchedulerWebUtils.ToDateTime(value), appointment.TimeZoneId);
			if (appointment.AllDay && newStart.Ticks % DateTimeHelper.DaySpan.Ticks != 0)
				appointment.AllDay = false;
			appointment.Start = timeZoneEngine.FromClientTime(SchedulerWebUtils.ToDateTime(value), appointment.TimeZoneId);
		}
		protected virtual void ApplyDurationChange(Appointment appointment, string value) {
			TimeSpan duration = SchedulerWebUtils.ToTimeSpan(value);
			if (appointment.AllDay && duration.Ticks % DateTimeHelper.DaySpan.Ticks != 0)
				appointment.AllDay = false;
			appointment.Duration = duration;
		}
		protected virtual void ApplyResourcesChange(Appointment appointment, string value) {
			string[] resourceIds = value.Split(',');
			appointment.ResourceIds.Clear();
			int count = resourceIds.Length;
			for (int i = 0; i < count; i++) {
				XtraScheduler.Resource resource = Control.LookupResourceByIdString(resourceIds[i]);
				appointment.ResourceIds.Add(resource.Id);
			}
		}
		protected virtual void ApplyAllDayChange(Appointment appointment, string value) {
			appointment.AllDay = Boolean.Parse(value);
		}
		protected internal override void ExecuteCore() {
			AppointmentBaseCollection conflictedAppointments = new AppointmentBaseCollection();
			try {
				foreach (string id in appointmentsChanges.Keys) {
					Appointment appointment = Control.LookupVisibleAppointmentByIdString(id);
					if (appointment == null)
						continue;
					AppointmentFormController controller = CreateAppointmentFormController(appointment);
					Appointment editedAppointmentCopy = controller.EditedAppointmentCopy;
					AppointmentChanges changes = appointmentsChanges[id];
					foreach (string name in changes.Keys)
						ApplyChange(editedAppointmentCopy, name, changes[name]);
					bool copy = changes.ContainsKey("COPY") && changes["COPY"] == "true";
					AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
					if (!IsConflictResolved(editedAppointmentCopy, appointment, copy))
						conflictedAppointments.Add(appointment);
					else {
						if (copy) {
							XtraScheduler.Resource resource = Control.InnerControl.Storage.GetResourceById(controller.ResourceId);
							if (helper.CanCreateAppointment(((IInternalAppointment)editedAppointmentCopy).GetInterval(), resource, appointment.IsRecurring))
								CopyAppointment(controller);
						} else
							if (helper.CanEditAppointment(editedAppointmentCopy))
								ChangeAppointment(controller);
					}
				}
			} finally {
			}
			if (conflictedAppointments.Count != 0)
				SchedulerStatusInfoHelper.AddWarning(Control, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict));
		}
		protected internal virtual AppointmentFormController CreateAppointmentFormController(Appointment appointment) {
			return new AppointmentFormController(Control, appointment);
		}
		bool IsConflictResolved(Appointment editedAptCopy, Appointment sourceApt, bool copy) {
			AppointmentOperationHelper helper = new AppointmentOperationHelper(Control.InnerControl);
			return helper.IsConflictResolved(editedAptCopy, sourceApt, copy);
		}
		protected virtual void ChangeAppointment(AppointmentFormController controller) {
			controller.ApplyChanges();
		}
		protected virtual void CopyAppointment(AppointmentFormController controller) {
			Appointment newAppointment = controller.SourceAppointment.Copy();
			((IInternalAppointment)controller.EditedAppointmentCopy).SetTypeCore(AppointmentType.Normal);
			newAppointment.Assign(controller.EditedAppointmentCopy);
			Control.Storage.Appointments.Add(newAppointment);
		}
	}
	#endregion
	public abstract class SchedulerCommandWrapperCallbackCommand : SchedulerCallbackCommand {
		protected SchedulerCommandWrapperCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			SchedulerCommand command = CreateCommand();
			if (command == null)
				ExecuteDefaultCommand();
			else
				command.Execute();
		}
		protected internal override void ParseParameters(string parameters) {
		}
		internal virtual void ExecuteDefaultCommand() {
		}
		protected internal override void UpdateUIState(ICommandUIState state) {
			SchedulerCommand command = CreateCommand();
			if (command == null)
				return;
			command.UpdateUIState(state);
		}
		protected internal abstract SchedulerCommand CreateCommand();
	}
	public class GotoTodayCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public GotoTodayCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.GotoToday; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new GotoTodayCommand(Control.InnerControl);
		}
	}
	public class NavigateBackwardCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateBackwardCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateBackward; } }
		#endregion
		internal override void ExecuteDefaultCommand() {
			IDateTimeNavigationService service = Control.GetService(typeof(IDateTimeNavigationService)) as IDateTimeNavigationService;
			if (service == null)
				return;
			service.NavigateBackward();
		}
		protected internal override SchedulerCommand CreateCommand() {
			return null;
		}
	}
	public class NavigateForwardCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateForwardCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateForward; } }
		#endregion
		internal override void ExecuteDefaultCommand() {
			IDateTimeNavigationService service = Control.GetService(typeof(IDateTimeNavigationService)) as IDateTimeNavigationService;
			if (service == null)
				return;
			service.NavigateForward();
		}
		protected internal override SchedulerCommand CreateCommand() {
			return null;
		}
	}
	public class ChangeTimeZoneCallbackCommand : SchedulerCallbackCommand {
		string timeZoneId;
		public ChangeTimeZoneCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.ChangeTimeZone; } }
		protected internal string TimeZoneId { get { return timeZoneId; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			this.timeZoneId = parameters;
		}
		protected internal override void ExecuteCore() {
			Control.OptionsBehavior.ClientTimeZoneId = timeZoneId;
		}
	}
	public class NavigateResourceFirstCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateResourceFirstCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateResourceFirst; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new NavigateFirstResourceCommand(Control.InnerControl);
		}
	}
	public class NavigateResourcePrevPageCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateResourcePrevPageCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateResourcePrevPage; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new NavigateResourcePageBackwardCommand(Control.InnerControl);
		}
	}
	public class NavigateResourcePrevCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateResourcePrevCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateResourcePrev; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new NavigatePrevResourceCommand(Control.InnerControl);
		}
	}
	public class NavigateResourceNextCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateResourceNextCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateResourceNext; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new NavigateNextResourceCommand(Control.InnerControl);
		}
	}
	public class NavigateResourceNextPageCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateResourceNextPageCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateResourceNextPage; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new NavigateResourcePageForwardCommand(Control.InnerControl);
		}
	}
	public class NavigateResourceLastCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public NavigateResourceLastCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateResourceLast; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new NavigateLastResourceCommand(Control.InnerControl);
		}
	}
	public class IncrementVisibleResourceCountCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public IncrementVisibleResourceCountCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.IncrementVisibleResourceCount; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new IncrementResourcePerPageCountCommand(Control.InnerControl);
		}
	}
	public class DecrementVisibleResourceCountCallbackCommand : SchedulerCommandWrapperCallbackCommand {
		public DecrementVisibleResourceCountCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.DecrementVisibleResourceCount; } }
		#endregion
		protected internal override SchedulerCommand CreateCommand() {
			return new DecrementResourcePerPageCountCommand(Control.InnerControl);
		}
	}
	public class NavigateSpecificResourceCallbackCommand : SchedulerCallbackCommand {
		int resourceIndex;
		public NavigateSpecificResourceCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.NavigateSpecificResource; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			this.resourceIndex = -1;
			int.TryParse(parameters, out this.resourceIndex);
		}
		protected internal override void ExecuteCore() {
			if (this.resourceIndex >= 0)
				Control.ActiveView.FirstVisibleResourceIndex = this.resourceIndex;
		}
	}
	public abstract class DateNavigatorCallbackCommandBase : SchedulerCallbackCommand, IDateNavigatorControllerOwner {
		protected DateNavigatorCallbackCommandBase(ASPxScheduler control)
			: base(control) {
		}
		#region IDateNavigatorControllerOwner Members
		public virtual DateTime EndDate {
			get { return DateTime.MinValue; }
		}
		public virtual DateTime GetFirstDayOfMonth(DateTime date) {
			return new DateTime(date.Year, date.Month, 1);
		}
		public virtual DayIntervalCollection GetSelection() {
			return new DayIntervalCollection();
		}
		public virtual DateTime SelectionEnd {
			get { return DateTime.MinValue; }
		}
		public virtual DateTime SelectionStart {
			get { return DateTime.MinValue; }
		}
		public virtual void SetSelection(DayIntervalCollection days) {
		}
		public virtual DateTime StartDate {
			get {
				return DateTime.MinValue;
			}
			set { }
		}
		#endregion
	}
	public class OffsetVisibleIntervalsCallbackCommand : DateNavigatorCallbackCommandBase {
		public OffsetVisibleIntervalsCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.OffsetVisibleIntervals; } }
		public DateTime FirstDate { get; private set; }
		public DateTime LastDate { get; private set; }
		public DayIntervalCollection Selection { get; private set; }
		public int Offset { get; private set; }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			try {
				Selection = new DayIntervalCollection();
				try {
					string[] dataItems = parameters.Split(',');
					int count = dataItems.Length;
					if (count < 3)
						return;
					Offset = int.Parse(dataItems[0], CultureInfo.InvariantCulture);
					FirstDate = DateTime.Parse(dataItems[1], CultureInfo.InvariantCulture);
					LastDate = DateTime.Parse(dataItems[2], CultureInfo.InvariantCulture);
					for (int i = 3; i < count; i++) {
						DateTime date = DateTime.Parse(dataItems[i], CultureInfo.InvariantCulture);
						Selection.Add(new TimeInterval(date, TimeSpan.Zero));
					}
				} catch {
				}
			}
			catch {
				Selection.Clear();
			}
		}
		protected internal override void ExecuteCore() {
			if (Selection.Count <= 0) 
				return;
			DateTime activeDate = DateTime.MinValue;
			if (Selection.Start < FirstDate) {
				Control.Start = Selection.Start.AddMonths(Offset);
				activeDate = FirstDate;
			} else if (Selection.End > LastDate) {
				Control.Start = Selection.Start.AddMonths(Offset);
				activeDate = DateTimeHelper.GetNextBeginOfMonth(LastDate);
			}
			if (activeDate != DateTime.MinValue) {
				if (Control.ActiveViewType == SchedulerViewType.Month) {
					if (!CheckWeekContainsDate(Control.Start, activeDate)) {
						if (Control.ActiveViewType == SchedulerViewType.Week || Control.ActiveViewType == SchedulerViewType.FullWeek) {
							Control.Start = Selection.Start.AddMonths(Offset + 1);
						} else {
							Control.Start = activeDate;
						}
					}
				} else if (Control.ActiveViewType == SchedulerViewType.Week || Control.ActiveViewType == SchedulerViewType.FullWeek) {
					if (Control.Start < DateTimeHelper.GetStartOfWeekUI(FirstDate, Control.FirstDayOfWeek, false)) {
						Control.Start = Selection.Start.AddMonths(Offset + 1);
					} else if (Control.Start > LastDate) {
						Control.Start = Selection.End.AddMonths(Offset - 1);
					}
				}
			}
		}
		bool CheckWeekContainsDate(DateTime weekStart, DateTime firstDate) {
			TimeInterval week = new TimeInterval(weekStart, weekStart.AddDays(7));
			return week.Contains(firstDate.AddTicks(1));
		}
	}
	public class SetVisibleDaysCallbackCommand : DateNavigatorCallbackCommandBase {
		#region Fields
		DayIntervalCollection selection;
		#endregion
		public SetVisibleDaysCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		public override string Id { get { return SchedulerCallbackCommandId.SetVisibleDays; } }
		internal DayIntervalCollection Selection { get { return selection; } }
		#endregion
		protected internal override void ParseParameters(string parameters) {
			selection = new DayIntervalCollection();
			try {
				string[] dates = parameters.Split(',');
				int count = dates.Length;
				for (int i = 0; i < count; i++) {
					DateTime date = DateTime.Parse(dates[i], CultureInfo.InvariantCulture);
					selection.Add(new TimeInterval(date, TimeSpan.Zero));
				}
			}
			catch {
			}
		}
		protected internal override void ExecuteCore() {
			DateNavigatorController controller = new DateNavigatorController(this);
			SubscribeDateNavigatorControllerEvents(controller);
			controller.BoldAppointmentDates = false;
			controller.InnerControl = Control.InnerControl;
			if (selection.IsContinuous()) {
				DateTime start = controller.AdjustSelectionStart(selection.Start, selection.End.AddDays(-1));
				DateTime end = controller.AdjustSelectionEnd(selection.Start, selection.End.AddDays(-1));
				selection.Add(new TimeInterval(start, end.AddDays(1)));
			}
			controller.SyncSelection();
			UnsubscribeDateNavigatorControllerEvents(controller);
		}
		protected virtual void SubscribeDateNavigatorControllerEvents(DateNavigatorController controller) {
			controller.DateNavigatorQueryActiveViewType += OnControllerDateNavigatorQueryActiveViewType;
		}
		protected virtual void UnsubscribeDateNavigatorControllerEvents(DateNavigatorController controller) {
			controller.DateNavigatorQueryActiveViewType -= OnControllerDateNavigatorQueryActiveViewType;
		}
		void OnControllerDateNavigatorQueryActiveViewType(object sender, DateNavigatorQueryActiveViewTypeEventArgs e) {
			if (Control == null)
				return;
			e.NewViewType = Control.RaiseDateNavigatorQueryActiveViewType(e.OldViewType, e.NewViewType, e.SelectedDays);
		}
		public override DayIntervalCollection GetSelection() {
			return selection;
		}
	}
	public abstract class SchedulerFunctionalCallbackCommand : SchedulerCallbackCommand {
		int callbackHandlerID = -1;
		string callbackResult = string.Empty;
		protected SchedulerFunctionalCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		protected internal int CallbackHandlerID { get { return callbackHandlerID; } }
		protected internal string CallbackResult {
			get { return callbackResult; }
			set { callbackResult = value; }
		}
		protected internal override void ParseParameters(string parameters) {
			try {
				int pos = parameters.IndexOf(SchedulerCallbackSR.ParameterDivider);
				if (pos <= 0)
					return;
				if (Int32.TryParse(parameters.Substring(0, pos), out callbackHandlerID) && callbackHandlerID >= 0)
					ParseContentParameters(parameters.Substring(pos + 1));
			}
			catch {
				callbackHandlerID = -1;
			}
		}
		protected internal abstract void ParseContentParameters(string paramenters);
		protected internal virtual string GetCallbackResult() {
			if (string.IsNullOrEmpty(CallbackResult))
				return string.Empty;
			return ASPxSchedulerScripts.GetCallbackHandlerFunction(CallbackHandlerID, CallbackResult);
		}
		protected internal override bool CanContinueExecute() { return CallbackHandlerID >= 0; }
	}
	public class AppointmentDataExCallbackCommand : SchedulerFunctionalCallbackCommand {
		Appointment appointment;
		string[] propertyNames = new string[0];
		public AppointmentDataExCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		protected Appointment Appointment { get { return appointment; } }
		protected string[] PropertyNames { get { return propertyNames; } }
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentDataEx; } }
		protected internal override void ParseContentParameters(string parameters) {
			XtraSchedulerDebug.Assert(CallbackHandlerID >= 0);
			try {
				string[] parts = parameters.Split(new Char[] { SchedulerCallbackSR.ParameterDivider });
				XtraSchedulerDebug.Assert(parts.Length == 2);
				this.appointment = Control.LookupVisibleAppointmentByIdString(parts[0]); 
				this.propertyNames = parts[1].Split(new Char[] { SchedulerCallbackSR.ValueDivider });
			}
			catch {
				appointment = null;
				propertyNames = new string[0];
			}
		}
		protected internal override bool CanContinueExecute() {
			return base.CanContinueExecute() && Appointment != null && PropertyNames.Length > 0;
		}
		protected internal override void ExecuteCore() {
			AppointmentDataReader reader = CreateAppointmentDataReader(Appointment);
			CallbackResult = reader.Read(PropertyNames);
		}
		protected virtual AppointmentDataReader CreateAppointmentDataReader(Appointment appointment) {
			return new AppointmentDataExReader(Control, appointment);
		}
	}
	public class AppointmentDataCallbackCommand : SchedulerFunctionalCallbackCommand {
		Appointment appointment;
		string[] propertyNames = new string[0];
		public AppointmentDataCallbackCommand(ASPxScheduler control)
			: base(control) {
		}
		protected Appointment Appointment { get { return appointment; } }
		protected string[] PropertyNames { get { return propertyNames; } }
		public override string Id { get { return SchedulerCallbackCommandId.AppointmentData; } }
		protected internal override void ParseContentParameters(string parameters) {
			XtraSchedulerDebug.Assert(CallbackHandlerID >= 0);
			try {
				string[] parts = parameters.Split(new Char[] { SchedulerCallbackSR.ParameterDivider });
				XtraSchedulerDebug.Assert(parts.Length == 2);
				this.appointment = Control.LookupVisibleAppointmentByIdString(parts[0]);
				this.propertyNames = parts[1].Split(new Char[] { SchedulerCallbackSR.ValueDivider });
			}
			catch {
				appointment = null;
				propertyNames = new string[0];
			}
		}
		protected internal override bool CanContinueExecute() {
			return base.CanContinueExecute() && Appointment != null && PropertyNames.Length > 0;
		}
		protected internal override void ExecuteCore() {
			AppointmentDataReader reader = CreateAppointmentDataReader(Appointment);
			CallbackResult = reader.Read(PropertyNames);
		}
		protected virtual AppointmentDataReader CreateAppointmentDataReader(Appointment appointment) {
			return new AppointmentDataReader(Control, appointment);
		}
	}
	public class AppointmentDataReader {
		TimeZoneHelper timeZoneEngine;
		Appointment appointment;
		ASPxScheduler scheduler;
		public AppointmentDataReader(ASPxScheduler scheduler, Appointment appointment) {
			Guard.ArgumentNotNull(scheduler, "scheduler");
			Guard.ArgumentNotNull(appointment, "appointment");
			this.appointment = appointment;
			this.scheduler = scheduler;
			this.timeZoneEngine = Scheduler.InnerControl.TimeZoneHelper;
		}
		protected Appointment Appointment { get { return appointment; } }
		protected TimeZoneHelper TimeZoneHelper { get { return timeZoneEngine; } }
		public ASPxScheduler Scheduler { get { return scheduler; } }
		public string Read(string[] propertyNames) {
			if (propertyNames == null || propertyNames.Length <= 0)
				return string.Empty;
			return ReadCore(propertyNames);
		}
		protected internal virtual string ReadCore(string[] propertyNames) {
			object[] result = new object[propertyNames.Length];
			for (int i = 0; i < propertyNames.Length; i++) {
				result[i] = ReadPropertyValue(propertyNames[i], propertyNames);
			}
			return HtmlConvertor.ToJSON(result);
		}
		protected internal virtual object ReadPropertyValue(string propertyName, string[] propertyNames) {
			if (propertyName == "AllDay")
				return Appointment.AllDay;
			if (propertyName == "Description")
				return Appointment.Description;
			if (propertyName == "Duration")
				return Appointment.Duration;
			if (propertyName == "End")
				return TimeZoneHelper.ToClientTime(Appointment.End, Appointment.TimeZoneId);
			if (propertyName == "HasExceptions")
				return Appointment.HasExceptions;
			if (propertyName == "Id")
				return Scheduler.GetAppointmentClientId(appointment);
			if (propertyName == "LabelId")
				return Appointment.LabelKey;
			if (propertyName == "LabelKey")
				return Appointment.LabelKey;
			if (propertyName == "Location")
				return Appointment.Location;
			if (propertyName == "RecurrenceIndex")
				return Appointment.RecurrenceIndex;
			if (propertyName == "RecurrenceInfo") {
				ClientRecurrenceInfoProperties clientRecurrenceInfo = new ClientRecurrenceInfoProperties(Appointment.RecurrenceInfo, Appointment.TimeZoneId);
				return clientRecurrenceInfo.ToPropertiesDictionary(TimeZoneHelper);
			}
			if (propertyName == "ResourceId")
				return Appointment.ResourceId;
			if (propertyName == "ResourceIds") {
				return (object)SchedulerIdHelper.GenerateResourceIds(Appointment.ResourceIds);
			}
			if (propertyName == "Start")
				return TimeZoneHelper.ToClientTime(Appointment.Start, Appointment.TimeZoneId);
			if (propertyName == "StatusId")
				return Appointment.StatusKey;
			if (propertyName == "StatusKey")
				return Appointment.StatusKey;
			if (propertyName == "Subject")
				return Appointment.Subject;
			if (propertyName == "Type")
				return Appointment.Type;
			return Appointment.CustomFields[propertyName];
		}
	}
	public class AppointmentDataExReader : AppointmentDataReader {
		static readonly Dictionary<String, String> ClientServerPropertyNameMap = new Dictionary<string, string>();
		static AppointmentDataExReader() {
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.AllDay, ClientSideAppointmentFieldNames.AllDay);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Description, ClientSideAppointmentFieldNames.Description);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Id, ClientSideAppointmentFieldNames.Id);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.LabelId, ClientSideAppointmentFieldNames.LabelId);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Location, ClientSideAppointmentFieldNames.Location);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.ResourceIds, ClientSideAppointmentFieldNames.ResourceIds);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.StatusId, ClientSideAppointmentFieldNames.StatusId);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Subject, ClientSideAppointmentFieldNames.Subject);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Type, ClientSideAppointmentFieldNames.AppointmentType);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Pattern, ClientSideAppointmentFieldNames.PatternType);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.Start, String.Empty);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.End, String.Empty);
			ClientServerPropertyNameMap.Add(ClientAppointmentPropertiesNames.RecurrenceInfo, ClientSideAppointmentFieldNames.RecurrenceInfo);
		}
		public AppointmentDataExReader(ASPxScheduler scheduler, Appointment appointment)
			: base(scheduler, appointment) {
		}
		protected Dictionary<String, Object> GetPropertiesDictionary() {
			List<String> propertyNames = new List<string>();
			foreach (KeyValuePair<String, String> item in ClientServerPropertyNameMap)
				propertyNames.Add(item.Key);
			return GetPropertiesDictionary(propertyNames.ToArray());
		}
		protected internal override object ReadPropertyValue(string propertyName, string[] propertyNames) {
			if (propertyName == ClientAppointmentPropertiesNames.Pattern) {
				if (Appointment.RecurrencePattern == null)
					return null;
				AppointmentDataExReader reader = new AppointmentDataExReader(Scheduler, Appointment.RecurrencePattern);
				string[] propertyNamesWithoutPattern = CalculatePatternProperties(propertyNames);
				return reader.GetPropertiesDictionary(propertyNamesWithoutPattern);
			}
			return base.ReadPropertyValue(propertyName, propertyNames);
		}
		string[] CalculatePatternProperties(string[] propertyNames) {
			List<string> result = new List<string>();
			foreach (KeyValuePair<String, String> item in ClientServerPropertyNameMap)
				result.Add(item.Key);
			propertyNames = EnsureIdPropertyExists(propertyNames);
			int count = propertyNames.Length;
			for (int i = 0; i < count; i++) {
				string propertyName = propertyNames[i];
				if (propertyName == ClientAppointmentPropertiesNames.Pattern || result.Contains(propertyName))
					continue;
				result.Add(propertyName);
			}
			return result.ToArray();
		}
		protected internal override string ReadCore(string[] propertyNames) {
			Dictionary<String, Object> result = GetPropertiesDictionary(propertyNames);
			return HtmlConvertor.ToJSON(result);
		}
		private Dictionary<String, Object> GetPropertiesDictionary(string[] propertyNames) {
			bool isIntervalProcessed = false;
			Dictionary<String, Object> result = new Dictionary<String, Object>();
			propertyNames = EnsureIdPropertyExists(propertyNames);
			for (int i = 0; i < propertyNames.Length; i++) {
				string propertyName = propertyNames[i];
				if (IsStartOrEndProperty(propertyName)) {
					if (!isIntervalProcessed) {
						isIntervalProcessed = true;
						WriteIntervalProperty(result);
					}
					continue;
				}
				WriteProperty(result, propertyName, propertyNames);
			}
			return result;
		}
		string[] EnsureIdPropertyExists(string[] propertyNames) {
			List<String> result = new List<string>(propertyNames);
			if (result.IndexOf("Id") == -1)
				result.Add("Id");
			return result.ToArray();
		}
		void WriteProperty(Dictionary<String, Object> result, string propertyName, string[] propertyNames) {
			string clientPropertyName = GetClientPropertyName(propertyName);
			object value = ReadPropertyValue(propertyName, propertyNames);
			if (value != null)
				result.Add(clientPropertyName, value);
		}
		void WriteIntervalProperty(Dictionary<string, object> result) {
			ClientIntervalProperties clientIntervalTransferObject = new ClientIntervalProperties(Appointment.Start, Appointment.End);
			result.Add(ClientAppointmentProperties.IntervalPropertyName, clientIntervalTransferObject.ToPropertiesDictionary(TimeZoneHelper, Appointment.TimeZoneId));
		}
		bool IsStartOrEndProperty(string propertyName) {
			return propertyName == "Start" || propertyName == "End";
		}
		string GetClientPropertyName(string propertyName) {
			if (ClientServerPropertyNameMap.ContainsKey(propertyName))
				return ClientServerPropertyNameMap[propertyName];
			return propertyName;
		}
	}
	public abstract class AppointmentClientSideCallbackCommandBase : SchedulerCallbackCommand {
		ClientAppointmentProperties clientAppointment;
		AppointmentFormController controller;
		protected AppointmentClientSideCallbackCommandBase(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public ClientAppointmentProperties ClientAppointment { get { return clientAppointment; } }
		public AppointmentFormController Controller { get { return controller; } }
		protected internal override void ParseParameters(string parameters) {
			ClientAppointmentJSONDeserializer deserializer = new ClientAppointmentJSONDeserializer(Control.InnerControl.TimeZoneHelper);
			this.clientAppointment = deserializer.DeserializeObject(parameters);
			Appointment apt = GetEditedAppointment(ClientAppointment);
			if (apt == null)
				throw new AppointmentNotFoundException(String.Format(ExceptionText.CannotFindAppointment, clientAppointment.Id));
			this.controller = CreateAppointmentFormController(apt);
		}
		protected virtual AppointmentFormController CreateAppointmentFormController(Appointment apt) {
			return new AppointmentFormController(Control, apt);
		}
		protected internal abstract Appointment GetEditedAppointment(ClientAppointmentProperties clientAppointment);
		protected internal override void ExecuteCore() {
			if (Controller == null)
				return;
			AssignControllerValues(Controller, ClientAppointment);
			if (!Controller.IsConflictResolved()) {
				SchedulerStatusInfoHelper.AddError(Control, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict));
				return;
			}
			Controller.ApplyChanges();
			IAppointmentSelectionService appointmentSelectionService = (IAppointmentSelectionService)Control.GetService(typeof(IAppointmentSelectionService));
			appointmentSelectionService.SelectAppointment(Controller.SourceAppointment);
		}
		private void AssignControllerValues(AppointmentFormController controller, ClientAppointmentProperties clientAppointment) {
			Appointment editedAppointment = controller.EditedAppointmentCopy;
			if (ClientAppointment.Start.HasValue)
				editedAppointment.Start = DateTimeHelper.TrimSeconds(clientAppointment.Start.Value);
			if (ClientAppointment.End.HasValue)
				editedAppointment.End = DateTimeHelper.TrimSeconds(clientAppointment.End.Value);
			if (ClientAppointment.ResourceIds != null) {
				editedAppointment.ResourceIds.Clear();
				editedAppointment.ResourceIds.AddRange(GetResourceIds(clientAppointment.ResourceIds));
			}
			if (ClientAppointment.LabelId.HasValue)
				editedAppointment.LabelKey = clientAppointment.LabelId.Value;
			if (ClientAppointment.StatusId.HasValue)
				editedAppointment.StatusKey = clientAppointment.StatusId.Value;
			if (ClientAppointment.Subject != null)
				editedAppointment.Subject = clientAppointment.Subject;
			if (ClientAppointment.Description != null)
				editedAppointment.Description = clientAppointment.Description;
			if (ClientAppointment.Location != null)
				editedAppointment.Location = clientAppointment.Location;
			if (ClientAppointment.AllDay.HasValue)
				editedAppointment.AllDay = clientAppointment.AllDay.Value;
			AssignControllerCustomFieldsValues(controller, clientAppointment);
			if (ClientAppointment.RecurrenceInfo != null) {
				Appointment patternCopy = controller.PrepareToRecurrenceEdit();
				CorrectRecurrenceInfo(clientAppointment.RecurrenceInfo, patternCopy);
				patternCopy.RecurrenceInfo.Assign(clientAppointment.RecurrenceInfo);
				controller.ApplyRecurrence(patternCopy);
			} else {
				if (editedAppointment.Type == AppointmentType.Pattern)
					controller.RemoveRecurrence();
			}
		}
		protected virtual void AssignControllerCustomFieldsValues(AppointmentFormController controller, ClientAppointmentProperties clientAppointment) {
		}
		void CorrectRecurrenceInfo(IRecurrenceInfo recurrenceInfo, Appointment pattern) {
			IRecurrenceInfo clientRecurrenceInfo = clientAppointment.RecurrenceInfo;
			DateTime recurrenceStart = pattern.Start;
			DateTime recurrenceEnd = recurrenceInfo.End;
			RecurrenceRange range = recurrenceInfo.Range;
			int occurrenceCount = recurrenceInfo.OccurrenceCount;
			((IInternalRecurrenceInfo)clientRecurrenceInfo).UpdateRange(recurrenceStart, recurrenceEnd, range, occurrenceCount, pattern);
		}
		ResourceIdCollection GetResourceIds(List<String> resourceIdsString) {
			ResourceIdCollection result = new ResourceIdCollection();
			int count = resourceIdsString.Count;
			for (int i = 0; i < count; i++) {
				XtraScheduler.Resource resource = Control.LookupResourceByIdString(resourceIdsString[i]);
				result.Add(resource.Id);
			}
			return result;
		}
	}
	public class AppointmentClientSideInsertCallbackCommand : AppointmentClientSideCallbackCommandBase {
		public AppointmentClientSideInsertCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.ClientSideInsertAppointment; } }
		protected internal override bool CanContinueExecute() {
			if (!ClientAppointment.AppointmentType.HasValue)
				return false;
			InnerSchedulerControl innerControl = Control.InnerControl;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(innerControl);
			Appointment editedAppointment = Controller.EditedAppointmentCopy;
			XtraScheduler.Resource resource = innerControl.Storage.GetResourceById(editedAppointment.ResourceId);
			TimeInterval aptInterval = TimeInterval.Empty;
			if (ClientAppointment.Start.HasValue && ClientAppointment.End.HasValue)
				aptInterval = new TimeInterval(ClientAppointment.Start.Value, ClientAppointment.End.Value);
			return helper.CanCreateAppointment(aptInterval, resource, editedAppointment.IsRecurring);
		}
		protected internal override Appointment GetEditedAppointment(ClientAppointmentProperties clientAppointment) {
			return Control.Storage.CreateAppointment(ClientAppointment.AppointmentType.Value);
		}
	}
	public class AppointmentClientSideUpdateCallbackCommand : AppointmentClientSideCallbackCommandBase {
		public AppointmentClientSideUpdateCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.ClientSideUpdateAppointment; } }
		protected internal override bool CanContinueExecute() {
			if (String.IsNullOrEmpty(ClientAppointment.Id))
				return false;
			InnerSchedulerControl innerControl = Control.InnerControl;
			AppointmentOperationHelper helper = new AppointmentOperationHelper(innerControl);
			return helper.CanEditAppointment(Controller.SourceAppointment);
		}
		protected internal override Appointment GetEditedAppointment(ClientAppointmentProperties clientAppointment) {
			return Control.LookupAppointmentByIdString(clientAppointment.Id);
		}
	}
	public class AppointmentClientSideDeleteCallbackCommand : AppointmentClientSideCallbackCommandBase {
		public AppointmentClientSideDeleteCallbackCommand(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		public override string Id { get { return SchedulerCallbackCommandId.ClientSideDeleteAppointment; } }
		protected internal override Appointment GetEditedAppointment(ClientAppointmentProperties clientAppointment) {
			return Control.LookupAppointmentByIdString(clientAppointment.Id);
		}
		protected internal override void ExecuteCore() {
			if (Controller != null)
				Controller.DeleteAppointment();
		}
		protected internal override bool CanContinueExecute() {
			if (Controller == null)
				return false;
			return Controller.CanDeleteAppointment;
		}
	}
}
