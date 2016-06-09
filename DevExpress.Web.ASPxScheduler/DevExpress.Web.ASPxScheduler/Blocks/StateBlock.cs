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
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Schedule;
using System.Globalization;
using System.Collections;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public static class SchedulerClientStateProperties {
		public const string Selection = "selection";
		public const string FirstVisibleResource = "firstVisibleResource";
		public const string AppointmentSelection = "appointmentSelection";
		public const string FormType = "formType";
		public const string EditableAppointment = "editableAppointment";
		public const string ScrollState = "scrollState";
		public const string CheckSums = "checkSums";
		public const string Reminders = "reminders";
		public const string VisibleDays = "visibleDays";
		public const string DayViewTopRowTime = "dayViewTopRowTime";
		public const string WorkWeekViewTopRowTime = "workWeekViewTopRowTime";
		public const string FullWeekViewTopRowTime = "fullWeekViewTopRowTime";
		public const string WorkDays = "workDays";
	}
	public class StateBlock : ASPxSchedulerControlBlock {
		#region Fields
		SchedulerControlStateManager callbackStateManager;
		string[] appointmentIds = new string[] { };
		string reminderIds = String.Empty;
		string selectionStateString = String.Empty;
		string firstVisibleResourceString = String.Empty;
		bool postDataLoaded;
		bool isFirstTimeRendered;
		TopRowTimeState dayViewTopRowTimeState;
		TopRowTimeState workWeekViewTopRowTimeState;
		TopRowTimeState fullWeekViewTopRowTimeState;
		#endregion
		public StateBlock(ASPxScheduler control)
			: base(control) {
			this.callbackStateManager = new SchedulerControlStateManager(Owner);
			this.dayViewTopRowTimeState = new TopRowTimeState();
			this.workWeekViewTopRowTimeState = new TopRowTimeState();
			this.fullWeekViewTopRowTimeState = new TopRowTimeState();
		}
		#region Properties
		protected internal SchedulerControlStateManager CallbackStateManager { get { return callbackStateManager; } }
		public override string ContentControlID { get { return "stateBlock"; } }
		protected internal string SelectionStateString { get { return selectionStateString; } set { selectionStateString = value; } }
		public override ASPxSchedulerChangeAction RenderActions { get { return ASPxSchedulerChangeAction.RenderState; } }
		protected internal bool PostDataLoaded { get { return postDataLoaded; } }
		public bool IsFirstTimeRendered { get { return isFirstTimeRendered; } }
		internal TopRowTimeState DayViewTopRowTimeState { get { return dayViewTopRowTimeState; } }
		internal TopRowTimeState WorkWeekViewTopRowTimeState { get { return workWeekViewTopRowTimeState; } }
		internal TopRowTimeState FullWeekViewTopRowTimeState { get { return fullWeekViewTopRowTimeState; } }
		#endregion
		protected internal Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(ClientStateProperties.CallbackState, CallbackStateManager.CreateStateDiff());
			result.Add(SchedulerClientStateProperties.Selection, "");
			result.Add(SchedulerClientStateProperties.FirstVisibleResource, Owner.ActiveView.FirstVisibleResourceIndex);
			result.Add(SchedulerClientStateProperties.AppointmentSelection, CreateAppointmentSelectionIds(Owner.SelectedAppointments));
			result.Add(SchedulerClientStateProperties.FormType, (int)Owner.ActiveFormType);
			result.Add(SchedulerClientStateProperties.EditableAppointment, GetEditableAppointmentState());
			result.Add(SchedulerClientStateProperties.ScrollState, "");
			result.Add(SchedulerClientStateProperties.CheckSums, "");
			result.Add(SchedulerClientStateProperties.Reminders, Owner.ReminderState.ToString());
			result.Add(SchedulerClientStateProperties.VisibleDays, GetVisibleDaysValue());
			result.Add(SchedulerClientStateProperties.DayViewTopRowTime, "");
			result.Add(SchedulerClientStateProperties.WorkWeekViewTopRowTime, "");
			result.Add(SchedulerClientStateProperties.FullWeekViewTopRowTime, "");
			result.Add(SchedulerClientStateProperties.WorkDays, GetWorkDays());
			return result;
		}
		protected internal override void CreateControlHierarchyCore(Control parent) {
		}
		protected internal override void FinalizeCreateControlHierarchyCore(Control parent) {
		}
		protected internal override void PrepareControlHierarchyCore() {
			ContentControl.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
		string GetEditableAppointmentState() {
			if (Owner.EditableAppointment.Id != null)
				return String.Format("{0}|{1}", Owner.EditableAppointment.Type.ToString(), Owner.EditableAppointment.Id);
			return string.Empty;
		}
		string GetWorkDays() {
			return WorkDaysCollectionConverter.ToString(Owner.WorkDays);
		}
		protected internal virtual string GetVisibleDaysValue() {
			if (!(Owner.ActiveView is DayView))
				return String.Empty;
			DayIntervalCollection days = (DayIntervalCollection)Owner.ActiveView.GetVisibleIntervals();
			List<string> result = new List<string>();
			int count = days.Count;
			for (int i = 0; i < count; i++) {
				DateTime date = days[i].Start.Date;
				result.Add(date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString());
			}
			return String.Join(",", result.ToArray());
		}
		protected internal virtual void CreateInitialStateSnapshot() {
			CallbackStateManager.CreateInitialSnapShot();
		}
		protected internal virtual void ApplyCallbackState(string callbackState) {
			Owner.SuspendEvents();
			try {
				Owner.BeginUpdate();
				try {
					CallbackStateManager.ApplyStateDiff(callbackState);
					ApplySelectionState();
				} finally {
					Owner.EndUpdate();
				}
			} finally {
				Owner.ResumeEvents();
			}
		}
		public override void RenderPostbackScriptBegin(StringBuilder sb, string localVarName, string clientName) {
			sb.AppendFormat("{0}.BeginInit();\n", localVarName);
		}
		public override void RenderCallbackScriptBegin(StringBuilder sb, string localVarName, string clientName) {
			sb.AppendFormat("{0}.BeginInit();\n", localVarName);
			if (Owner.PropertyChangeTracker.IsVisibleIntervalChanged())
				sb.AppendFormat("{0}.privateRaiseVisibleIntervalChanged={1};\n", localVarName, HtmlConvertor.ToScript(true));
		}
		public override void RenderCallbackScriptEnd(StringBuilder sb, string localVarName, string clientName) {
			sb.AppendFormat("{0}.EndInit();\n", localVarName);
		}
		public override void RenderCommonScript(StringBuilder sb, string localVarName, string clientName) {
			GenerateSetSelectionScript(sb, localVarName);
			GenerateAppointmentSelectionScript(sb, localVarName);
			GenerateSetShowAllAppointmentsOnTimeCellsScript(sb, localVarName);
			GenerateSetActiveViewTypeScript(sb, localVarName);
			GenerateSetGroupTypeScript(sb, localVarName);
			GenerateSetDayViewProperties(sb, localVarName);
			GenerateDisableSnapToCellsScript(sb, localVarName);
			GenerateSetAppointmentHeightScript(sb, localVarName);
			GenerateSetFormsInitStateScript(sb, localVarName);
			GenerateEndUserRestrictions(sb, localVarName);
			GenerateTopRowTimeScript(sb, localVarName);
			GenerateCanShowDayTimeMarker(sb, localVarName);
		}
		void GenerateTopRowTimeScript(StringBuilder sb, string localVarName) {
			DayViewTopRowTimeState.Duration = Owner.DayView.TopRowTime;
			WorkWeekViewTopRowTimeState.Duration = Owner.WorkWeekView.TopRowTime;
			FullWeekViewTopRowTimeState.Duration = Owner.FullWeekView.TopRowTime;
			DayViewTopRowTimeState.Validate(Owner.DayView);
			WorkWeekViewTopRowTimeState.Validate(Owner.WorkWeekView);
			FullWeekViewTopRowTimeState.Validate(Owner.FullWeekView);
			sb.AppendFormat("{0}.SetTopRowTimeField(\"{1}\", \"{2}\", \"{3}\");\n", localVarName, DayViewTopRowTimeState.ToString(), WorkWeekViewTopRowTimeState.ToString(), FullWeekViewTopRowTimeState.ToString());
		}
		void GenerateEndUserRestrictions(StringBuilder sb, string localVarName) {
			sb.AppendFormat("{0}.privateAllowAppointmentMultiSelect={1};\n", localVarName, HtmlConvertor.ToScript(Owner.OptionsCustomization.AllowAppointmentMultiSelect));
		}
		protected internal virtual void GenerateSetShowAllAppointmentsOnTimeCellsScript(StringBuilder sb, string localVarName) {
			DayView dayView = Owner.ActiveView as DayView;
			bool allAppointmentsOnTimeCell = dayView == null ? true : dayView.ActualShowAllAppointmentsAtTimeCells;
			sb.AppendFormat("{0}.privateShowAllAppointmentsOnTimeCells={1};\n", localVarName, HtmlConvertor.ToScript(allAppointmentsOnTimeCell));
		}
		void GenerateSetFormsInitStateScript(StringBuilder sb, string localVarName) {
			ASPxSchedulerOptionsForms options = Owner.OptionsForms;
			sb.AppendFormat("{0}.SetFormsInitState(\"{1}\",\"{2}\",\"{3}\", \"{4}\");\n", localVarName, Owner.ActiveFormType,
				options.AppointmentFormVisibility, options.GotoDateFormVisibility, options.RecurrentAppointmentDeleteFormVisibility);
		}
		void GenerateCanShowDayTimeMarker(StringBuilder sb, string localVarName) {
			SchedulerViewType activeViewType = Owner.ActiveViewType;
			bool isSupportedView = activeViewType == SchedulerViewType.Day || activeViewType == SchedulerViewType.WorkWeek || activeViewType == SchedulerViewType.FullWeek;
			bool isOpenFormInFillMode = (Owner.OptionsForms.AppointmentFormVisibility == SchedulerFormVisibility.FillControlArea && Owner.ActiveFormType != SchedulerFormType.None);
			bool canShowDayTimeMarker = isSupportedView && !isOpenFormInFillMode;
			sb.AppendFormat("{0}.SetCanShowDayTimeMarker({1});\n", localVarName, HtmlConvertor.ToScript(canShowDayTimeMarker));
		}
		protected internal virtual void GenerateSetSelectionScript(StringBuilder sb, string localVarName) {
			SchedulerViewSelection selection = Owner.Selection;
			TimeInterval interval = selection.Interval;
			TimeInterval firstSelectedInterval = selection.FirstSelectedInterval;
			string start = HtmlConvertor.ToScript(interval.Start);
			string duration = HtmlConvertor.ToScript(interval.Duration.TotalMilliseconds);
			string firstSelectionStart = HtmlConvertor.ToScript(firstSelectedInterval.Start);
			string firstSelectionDuration = HtmlConvertor.ToScript(firstSelectedInterval.Duration.TotalMilliseconds);
			string resourceId = SchedulerIdHelper.GenerateScriptResourceId((XtraScheduler.Resource)selection.Resource);
			sb.AppendFormat("{0}.SetSelection({1}, {2}, {3}, {4}, {5});\n", localVarName, start, duration, resourceId, firstSelectionStart, firstSelectionDuration);
		}
		protected internal virtual void GenerateSetActiveViewTypeScript(StringBuilder sb, string localVarName) {
			sb.AppendFormat("{0}.PrivateSetActiveViewType(\"{1}\");\n", localVarName, Owner.ActiveViewType);
		}
		protected internal virtual void GenerateSetGroupTypeScript(StringBuilder sb, string localVarName) {
			SchedulerGroupType groupType = Owner.GroupType;
			int resourceCount = Owner.Storage.GetVisibleResources(true).Count;
			if (Owner.GroupType == SchedulerGroupType.Date && resourceCount == 0) 
				groupType = SchedulerGroupType.None;
			sb.AppendFormat("{0}.PrivateSetGroupType(\"{1}\");\n", localVarName, groupType);
			if (resourceCount == 0)
				sb.AppendFormat("{0}.PrivateSetActualGroupType(\"{1}\");\n", localVarName, SchedulerGroupType.None);
		}
		protected internal virtual void GenerateSetAppointmentHeightScript(StringBuilder sb, string localVarName) {
			AppointmentDisplayOptions displayOptions = Owner.ActiveView.AppointmentDisplayOptionsInternal;
			int height = displayOptions.AppointmentAutoHeight ? 0 : displayOptions.AppointmentHeight;
			sb.AppendFormat("{0}.PrivateSetAppointmentHeight(\"{1}\");\n", localVarName, height);
		}
		protected internal virtual void GenerateSetDayViewProperties(StringBuilder sb, string localVarName) {
			DayView dayView = Owner.ActiveView as DayView;
			if (dayView != null && !dayView.Styles.AllDayAreaHeight.IsEmpty)
				sb.AppendFormat("{0}.PrivateSetAllDayAreaHeight(\"{1}\");\n", localVarName, dayView.Styles.AllDayAreaHeight);
			if (dayView != null && !dayView.Styles.ScrollAreaHeight.IsEmpty)
				sb.AppendFormat("{0}.privateShowMoreButtonsOnEachColumn={1};\n", localVarName, HtmlConvertor.ToScript(dayView.ShowMoreButtonsOnEachColumn));
		}
		protected internal virtual void GenerateDisableSnapToCellsScript(StringBuilder sb, string localVarName) {
			bool disabled = Owner.ActiveView.AppointmentDisplayOptionsInternal.SnapToCellsMode == AppointmentSnapToCellsMode.Disabled;
			sb.AppendFormat("{0}.privateDisableSnapToCells={1};\n", localVarName, HtmlConvertor.ToScript(disabled));
		}
		protected internal virtual void GenerateAppointmentSelectionScript(StringBuilder sb, string localVarName) {
			string[] ids = CreateAppointmentSelectionIds(Owner.SelectedAppointments);
			if (ids.Length <= 0)
				return;
			sb.AppendFormat("{0}.SetSelectedAppointmentIds({1});\n", localVarName, HtmlConvertor.ToJSON(ids));
		}
		string[] CreateAppointmentSelectionIds(AppointmentBaseCollection selectedApts) {
			int count = selectedApts.Count;
			string[] ids = new string[count];
			for (int i = 0; i < count; i++)
				ids[i] = Owner.GetAppointmentClientId(selectedApts[i]);
			return ids;
		}
		protected internal virtual void ApplyFirstVisibleResourceState() {
			if (String.IsNullOrEmpty(this.firstVisibleResourceString))
				return;
			int indx = -1;
			if (!int.TryParse(this.firstVisibleResourceString, out indx))
				return;
			Owner.ActiveView.FirstVisibleResourceIndex = indx;
		}
		protected internal virtual void ApplySelectionState() {
			if (String.IsNullOrEmpty(this.selectionStateString))
				return;
			string[] parameters = selectionStateString.Split(',');
			if (parameters.Length != 5)
				Exceptions.ThrowInternalException();
			SchedulerViewSelection selection = Owner.Selection;
			selection.Interval = SchedulerWebUtils.ToTimeInterval(parameters[0], parameters[1]);
			selection.FirstSelectedInterval = SchedulerWebUtils.ToTimeInterval(parameters[2], parameters[3]);
			selection.Resource = Owner.LookupResourceByIdString(parameters[4]); 
		}
		protected internal virtual void ParseAppointmentSelectionState(string appointmentSelectionState) {
			if (String.IsNullOrEmpty(appointmentSelectionState))
				this.appointmentIds = new string[] { };
			else
				this.appointmentIds = appointmentSelectionState.Split(',');
		}
		protected internal virtual void ApplyAppointmentSelectionState() {
			int count = appointmentIds.Length;
			for (int i = 0; i < count; i++) {
				string id = appointmentIds[i];
				Appointment apt = Owner.LookupVisibleAppointmentByIdString(id); 
				if (apt != null)
					Owner.SelectedAppointments.Add(apt);
			}
		}
		protected internal virtual void ApplyEditableAppointmentState(string editedAppointmentState) {
			if (String.IsNullOrEmpty(editedAppointmentState))
				return;
			string[] parts = StringArgumentsHelper.SplitMasterSlaveArgumentPair(editedAppointmentState);
			string type = parts[0];
			string appointmentId = parts[1];
			AppointmentType aptType = (AppointmentType)Enum.Parse(typeof(AppointmentType), type);
			Owner.EditableAppointment.SetExistingReference(aptType, appointmentId);
		}
		protected internal virtual void ApplyScrollState(string scrollState) {
			Owner.ScrollState = scrollState;
		}
		protected internal virtual void ApplyCheckSums(string checkSums) {
			if (String.IsNullOrEmpty(checkSums))
				return;
			string[] parts = checkSums.Split(' ');
			int count = parts.Length;
			Dictionary<string, HashValueBase> blockCheckSums = Owner.BlockCheckSums;
			blockCheckSums.Clear();
			for (int i = 0; i < count; i += 2) {
				string name = parts[i];
				HashValueBase hash = HashValueBase.FromBase64String(parts[i + 1]);
				blockCheckSums.Add(name, hash);
			}
		}
		protected internal virtual void ParseRemindersState(string reminders) {
			this.reminderIds = reminders;
		}
		protected internal virtual void ApplyRemindersState() {
			Owner.ReminderState.LoadFromString(this.reminderIds);
		}
		protected internal virtual void ApplyTopRowTime(SchedulerViewType viewType, string topRowTimeString) {
			if (viewType == SchedulerViewType.Day) {
				this.dayViewTopRowTimeState = new TopRowTimeState(topRowTimeString);
				Owner.DayView.TopRowTime = DayViewTopRowTimeState.Duration;
			} else if (viewType == SchedulerViewType.WorkWeek) {
				this.workWeekViewTopRowTimeState = new TopRowTimeState(topRowTimeString);
				Owner.WorkWeekView.TopRowTime = WorkWeekViewTopRowTimeState.Duration;
			} else if (viewType == SchedulerViewType.FullWeek) {
				this.fullWeekViewTopRowTimeState = new TopRowTimeState(topRowTimeString);
				Owner.FullWeekView.TopRowTime = FullWeekViewTopRowTimeState.Duration;
			}
		}
		protected internal virtual void ApplyFormTypeState(string formTypeState) {
			if (String.IsNullOrEmpty(formTypeState))
				return;
			Owner.ActiveFormType = (SchedulerFormType)Enum.Parse(typeof(SchedulerFormType), formTypeState);
		}
		protected internal virtual void ApplyWorkDaysState(string workDaysState) {
			if (String.IsNullOrEmpty(workDaysState))
				return;
			WorkDaysCollectionConverter.FromString(Owner.WorkDays, workDaysState);
		}
		protected internal virtual void ApplyVisibleDaysState(string visibleDaysState) {
			DayView view = Owner.ActiveView as DayView;
			if (view == null || String.IsNullOrEmpty(visibleDaysState))
				return;
			List<string> dates = new List<string>(visibleDaysState.Split(new char[] { ',' }));
			int count = dates.Count;
			DayIntervalCollection days = new DayIntervalCollection();
			for (int i = 0; i < count; i++) {
				string[] values = dates[i].Split(new char[] { '/' });
				DateTime date = new DateTime(int.Parse(values[2]), int.Parse(values[1]), int.Parse(values[0]));
				days.Add(new TimeInterval(date, TimeSpan.FromDays(1)));
			}
			if (days.Count == view.DayCount && days[0].Start == view.Control.Start)
				Owner.ActiveView.SetVisibleIntervals(days);
		}
		public virtual bool LoadUserActionChangesPostData() {
			string workDaysState = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.WorkDays);
			ApplyWorkDaysState(workDaysState);
			string visibleDaysState = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.VisibleDays);
			ApplyVisibleDaysState(visibleDaysState);
			this.selectionStateString = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.Selection);
			ApplySelectionState();
			this.firstVisibleResourceString = Owner.GetClientObjectStateValueInternal<int>(SchedulerClientStateProperties.FirstVisibleResource).ToString();
			ApplyFirstVisibleResourceState();
			string appointmentSelectionState = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.AppointmentSelection);
			ParseAppointmentSelectionState(appointmentSelectionState);
			ApplyAppointmentSelectionState();
			string appointmentIdState = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.EditableAppointment);
			ApplyEditableAppointmentState(appointmentIdState);
			string scrollState = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.ScrollState);
			ApplyScrollState(scrollState);
			string checkSums = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.CheckSums);
			ApplyCheckSums(checkSums);
			string reminders = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.Reminders);
			ParseRemindersState(reminders);
			ApplyRemindersState();
			string dayViewTopRowTime = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.DayViewTopRowTime);
			ApplyTopRowTime(SchedulerViewType.Day, dayViewTopRowTime);
			string workWeekViewTopRowTime = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.WorkWeekViewTopRowTime);
			ApplyTopRowTime(SchedulerViewType.WorkWeek, workWeekViewTopRowTime);
			string fullWeekViewTopRowTime = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.FullWeekViewTopRowTime);
			ApplyTopRowTime(SchedulerViewType.FullWeek, fullWeekViewTopRowTime);
			this.postDataLoaded = true;
			return false;
		}
		public virtual bool LoadPreviousStatePostData() {
			string formTypeState = Owner.GetClientObjectStateValueStringInternal(SchedulerClientStateProperties.FormType);
			if (formTypeState == null) 
				this.isFirstTimeRendered = true;
			ApplyFormTypeState(formTypeState);
			string callbackState = Owner.GetClientObjectStateValueStringInternal(ClientStateProperties.CallbackState);
			ApplyCallbackState(callbackState);
			return false;
		}
		protected override void TrackViewState() {
			if (!Page.IsPostBack)
				CreateInitialStateSnapshot();
		}
	}
	public class SchedulerSnapShot {
		IXtraPropertyCollection snapShot;
		ASPxScheduler control;
		DateTime start;
		SchedulerViewType activeViewType;
		public SchedulerSnapShot(ASPxScheduler control) {
			this.control = control;
			Control.IsSnapShotCreating = true;
			SnapshotSerializeHelper helper = new SnapshotSerializeHelper();
			this.snapShot = helper.SerializeObject(control, OptionsLayoutBase.FullLayout);
			Control.IsSnapShotCreating = false;
			this.start = Control.Start;
			this.activeViewType = Control.ActiveViewType;
		}
		public IXtraPropertyCollection RawSnapShot { get { return snapShot; } }
		public DateTime Start { get { return start; } set { start = value; } }
		public XtraPropertyInfo StartPropertyInfo {
			get { return new XtraPropertyInfo("Start", typeof(DateTime), Control.Start); }
		}
		public SchedulerViewType ActiveViewType { get { return activeViewType; } set { activeViewType = value; } }
		public ASPxScheduler Control { get { return control; } set { control = value; } }
	}
	public class SchedulerControlStateManager : StateManager {
		#region Fields
		SchedulerSnapShot firstSnapShot;
		ASPxScheduler control;
		#endregion
		public SchedulerControlStateManager(ASPxScheduler control) {
			if (control == null)
				Exceptions.ThrowArgumentNullException("control");
			this.control = control;
		}
		#region Properties
		internal ASPxScheduler Control { get { return control; } }
		internal SchedulerSnapShot FirstSnapShot { get { return firstSnapShot; } }
		#endregion
		protected internal virtual SchedulerSnapShot CreateSnapShot() {
			return new SchedulerSnapShot(control);
		}
		protected internal virtual void CreateInitialSnapShot() {
			if (this.firstSnapShot == null) {
				this.firstSnapShot = CreateSnapShot();
			}
		}
		protected internal virtual string CreateStateDiff() {
			if (firstSnapShot == null)
				return null;
			SchedulerSnapShot secondSnapShot = CreateSnapShot();
			IXtraPropertyCollection diff = CalculateSnapShotDiff(this.firstSnapShot, secondSnapShot);
			if (diff.Count <= 0)
				return null;
			CompressedBase64XtraSerializer serializer = new CompressedBase64XtraSerializer();
			string state = serializer.Serialize(diff);
			return String.IsNullOrEmpty(state) ? null : state;
		}
		IXtraPropertyCollection CalculateSnapShotDiff(SchedulerSnapShot firstSnapShot, SchedulerSnapShot secondSnapShot) {
			IXtraPropertyCollection result = SerializationDiffCalculator.CalculateDiffCore(firstSnapShot.RawSnapShot, secondSnapShot.RawSnapShot);
			if (firstSnapShot.Start != secondSnapShot.Start || firstSnapShot.ActiveViewType != secondSnapShot.ActiveViewType)
				result.Add(secondSnapShot.StartPropertyInfo);
			return result;
		}
		protected internal virtual void ApplyStateDiff(string stateDiff) {
			if (!String.IsNullOrEmpty(stateDiff)) {
				CompressedBase64XtraSerializer serializer = new CompressedBase64XtraSerializer();
				serializer.Deserialize(control, stateDiff);
			}
		}
	}
	public class TopRowTimeState {
		int scrollOffset = -1;
		TimeSpan duration;
		public TopRowTimeState() {
		}
		public TopRowTimeState(string value) {
			if (String.IsNullOrEmpty(value))
				return;
			string[] values = value.Split(new char[] { ',' });
			double ms = double.Parse(values[0]);
			this.duration = TimeSpan.FromMilliseconds(ms);
			try {
				this.scrollOffset = (int)double.Parse(values[1], CultureInfo.InvariantCulture);
			} catch {
				this.scrollOffset = 0;
			}
		}
		public int ScrollOffset { get { return scrollOffset; } }
		public virtual TimeSpan Duration {
			get {
				return this.duration;
			}
			set {
				if (this.duration == value)
					return;
				this.duration = value;
				this.scrollOffset = -1;
			}
		}
		public void Validate(DayView view) {
			if (this.duration < view.ActualVisibleTime.Start)
				Duration = view.ActualVisibleTime.Start;
		}
		public override string ToString() {
			return String.Format("{0},{1}", Duration.TotalMilliseconds, ScrollOffset);
		}
	}
	public static class WorkDaysCollectionConverter {
		const string WeekDaysWorkDayItemMarker = "WD";
		public static string ToString(WorkDaysCollection workDays) {
			List<string> result = new List<string>();
			int count = workDays.Count;
			for (int i = 0; i < count; i++) {
				WorkDay workDay = workDays[i];
				WeekDaysWorkDay weekDaysWorkDay = workDay as WeekDaysWorkDay;
				if (weekDaysWorkDay != null) {
					string stringValue = String.Format("{0}:{1}", WeekDaysWorkDayItemMarker, (int)weekDaysWorkDay.WeekDays);
					result.Add(stringValue);
				}
			}
			return String.Join(",", result.ToArray());
		}
		public static void FromString(WorkDaysCollection workDays, string stringValue) {
			string[] stringItems = stringValue.Split(new char[] { ',' });
			workDays.BeginUpdate();
			int count = stringItems.Length;
			for (int i = 0; i < count; i++) {
				string stringItem = stringItems[i];
				if (stringItem.Substring(0, 2) != WeekDaysWorkDayItemMarker)
					continue;
				string weekDaysWorkDayStringValue = stringItem.Substring(3);
				int outWeekDaysValue = -1;
				bool isParseSuccess = int.TryParse(weekDaysWorkDayStringValue, out outWeekDaysValue);
				if (!isParseSuccess)
					continue;
				WeekDays weekDays = (WeekDays)outWeekDaysValue;
				workDays.Add(weekDays);
			}
			workDays.EndUpdate();
		}
	}
}
