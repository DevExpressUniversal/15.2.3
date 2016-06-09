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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraScheduler.Internal;
using DevExpress.XtraScheduler.Compatibility;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.btnRemoveRecurrence")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.grpAptTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.grpRecurrencePattern")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.grpRecurrenceRange")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.lblRangeStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.edtRangeStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.lblRangeOccurrencesCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.edtRangeEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.edtStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.lblStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.lblEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.edtEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.spinRangeOccurrencesCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.dailyRecurrenceControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.weeklyRecurrenceControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.monthlyRecurrenceControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.yearlyRecurrenceControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.lblDuration")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.cbDuration")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkNoEndDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkEndByDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkDaily")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkWeekly")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkMonthly")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkYearly")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRecurrenceForm.chkEndAfterNumberOfOccurrences")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class AppointmentRecurrenceForm : DevExpress.XtraEditors.XtraForm {
		RecurrenceControlBase currentRecurrenceControl;
		SchedulerRecurrenceValidator validator;
		IRecurrenceInfo rinfo;
		Appointment pattern;
		RecurrenceType recurrenceType;
		RecurrenceRange recurrenceRange;
		AppointmentFormControllerBase controller;
		bool showExceptionsRemoveMsgBox;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentRecurrenceForm()
			: this(StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern), FirstDayOfWeek.System, null) {
		}
		[Obsolete("Please use the AppointmentRecurrenceForm(Appointment pattern, FirstDayOfWeek firstDayOfWeek, AppointmentFormController controller) instead.")]
		public AppointmentRecurrenceForm(Appointment pattern, FirstDayOfWeek firstDayOfWeek)
			: this(pattern, firstDayOfWeek, null) {
		}
		public AppointmentRecurrenceForm(Appointment pattern, FirstDayOfWeek firstDayOfWeek, AppointmentFormControllerBase controller) {
			if (pattern == null)
				Exceptions.ThrowArgumentException("pattern", pattern);
			if (pattern.Type != AppointmentType.Pattern)
				Exceptions.ThrowArgumentException("pattern", pattern);
			this.pattern = pattern;
			this.rinfo = pattern.RecurrenceInfo;
			this.controller = controller;
			this.validator = CreateRecurrenceValidator();
			InitializeComponent();
			UpdateIcon();
			ResetPatternAllDay();
			InitializeControls(firstDayOfWeek);
		}
		#region Properties
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentRecurrenceFormShowExceptionsRemoveMsgBox")]
#endif
		public bool ShowExceptionsRemoveMsgBox { get { return showExceptionsRemoveMsgBox; } set { showExceptionsRemoveMsgBox = value; } }
		protected internal Appointment Pattern { get { return pattern; } }
		protected internal IRecurrenceInfo RecurrenceInfo { get { return rinfo; } }
		protected internal RecurrenceControlBase CurrentRecurrenceControl { get { return currentRecurrenceControl; } set { currentRecurrenceControl = value; } }
		protected internal RecurrenceType RecurrenceType { get { return recurrenceType; } set { recurrenceType = value; } }
		protected internal RecurrenceRange RecurrenceRange { get { return recurrenceRange; } set { recurrenceRange = value; } }
		protected internal SchedulerRecurrenceValidator Validator { get { return validator; } }
		protected internal AppointmentFormControllerBase Controller { get { return controller; } }
		#endregion
		public virtual void SetMenuManager(Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
		}
		protected internal virtual DateTime ToClientTime(DateTime date) {
			if (Controller != null)
				return Controller.InnerControl.TimeZoneHelper.ToAppointmentTime(date, this.Pattern.TimeZoneId);
			else
				return date;
		}
		protected internal virtual DateTime FromClientTime(DateTime date) {
			if (Controller != null)
				return Controller.InnerControl.TimeZoneHelper.FromClientTime(date, Controller.TimeZoneId);
			else
				return date;
		}
		protected virtual SchedulerRecurrenceValidator CreateRecurrenceValidator() {
			return new SchedulerRecurrenceValidator();
		}
		protected virtual void UpdateIcon() {
			this.Icon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, Assembly.GetExecutingAssembly());
		}
		protected internal virtual void SubscribeRecurrenceTypeControlsEvents() {
			chkWeekly.EditValueChanged += new EventHandler(chkRecurrenceTypeChanged);
			chkMonthly.EditValueChanged += new EventHandler(chkRecurrenceTypeChanged);
			chkDaily.EditValueChanged += new EventHandler(chkRecurrenceTypeChanged);
			chkYearly.EditValueChanged += new EventHandler(chkRecurrenceTypeChanged);
		}
		protected internal virtual void UnsubscribeRecurrenceTypeControlsEvents() {
			chkWeekly.EditValueChanged -= new EventHandler(chkRecurrenceTypeChanged);
			chkMonthly.EditValueChanged -= new EventHandler(chkRecurrenceTypeChanged);
			chkDaily.EditValueChanged -= new EventHandler(chkRecurrenceTypeChanged);
			chkYearly.EditValueChanged -= new EventHandler(chkRecurrenceTypeChanged);
		}
		protected internal virtual void SubscribeRecurrenceRangeControlsEvents() {
			edtRangeStart.Validated += new EventHandler(edtRangeStart_Validated);
			spinRangeOccurrencesCount.EditValueChanged += new EventHandler(spinRangeOccurrencesCount_EditValueChanged);
			spinRangeOccurrencesCount.Validating += new CancelEventHandler(spinRangeOccurrencesCount_Validating);
			spinRangeOccurrencesCount.Validated += new EventHandler(spinRangeOccurrencesCount_Validated);
			spinRangeOccurrencesCount.InvalidValue += new InvalidValueExceptionEventHandler(spinRangeOccurrencesCount_InvalidValue);
			edtRangeEnd.EditValueChanged += new EventHandler(edtRangeEnd_EditValueChanged);
			edtRangeEnd.Validating += new CancelEventHandler(edtRangeEnd_Validating);
			edtRangeEnd.Validated += new EventHandler(edtRangeEnd_Validated);
			edtRangeEnd.InvalidValue += new InvalidValueExceptionEventHandler(edtRangeEnd_InvalidValue);
			chkNoEndDate.EditValueChanged += new EventHandler(chkEndTypeChanged);
			chkEndAfterNumberOfOccurrences.EditValueChanged += new EventHandler(chkEndTypeChanged);
			chkEndByDate.EditValueChanged += new EventHandler(chkEndTypeChanged);
		}
		protected internal virtual void UnsubscribeRecurrenceRangeControlsEvents() {
			edtRangeStart.Validated -= new EventHandler(edtRangeStart_Validated);
			spinRangeOccurrencesCount.EditValueChanged -= new EventHandler(spinRangeOccurrencesCount_EditValueChanged);
			spinRangeOccurrencesCount.Validating -= new CancelEventHandler(spinRangeOccurrencesCount_Validating);
			spinRangeOccurrencesCount.Validated -= new EventHandler(spinRangeOccurrencesCount_Validated);
			spinRangeOccurrencesCount.InvalidValue -= new InvalidValueExceptionEventHandler(spinRangeOccurrencesCount_InvalidValue);
			edtRangeEnd.EditValueChanged -= new EventHandler(edtRangeEnd_EditValueChanged);
			edtRangeEnd.Validating -= new CancelEventHandler(edtRangeEnd_Validating);
			edtRangeEnd.Validated -= new EventHandler(edtRangeEnd_Validated);
			edtRangeEnd.InvalidValue -= new InvalidValueExceptionEventHandler(edtRangeEnd_InvalidValue);
			chkNoEndDate.EditValueChanged -= new EventHandler(chkEndTypeChanged);
			chkEndAfterNumberOfOccurrences.EditValueChanged -= new EventHandler(chkEndTypeChanged);
			chkEndByDate.EditValueChanged -= new EventHandler(chkEndTypeChanged);
		}
		protected internal virtual void SubscribeAppointmentTimeControlsEvents() {
			edtStartTime.Validated += new EventHandler(edtStartTime_Validated);
			edtEndTime.Validated += new EventHandler(edtEndTime_Validated);
			cbDuration.InvalidValue += new InvalidValueExceptionEventHandler(cbDuration_InvalidValue);
			cbDuration.Validating += new CancelEventHandler(cbDuration_Validating);
			cbDuration.Validated += new EventHandler(cbDuration_Validated);
		}
		protected internal virtual void UnsubscribeAppointmentTimeControlsEvents() {
			edtStartTime.Validated -= new EventHandler(edtStartTime_Validated);
			edtEndTime.Validated -= new EventHandler(edtEndTime_Validated);
			cbDuration.InvalidValue -= new InvalidValueExceptionEventHandler(cbDuration_InvalidValue);
			cbDuration.Validating -= new CancelEventHandler(cbDuration_Validating);
			cbDuration.Validated -= new EventHandler(cbDuration_Validated);
		}
		protected virtual void SubscribeRecurrencePatternControlEvents() {
			CurrentRecurrenceControl.RecurrenceInfoChanged += new EventHandler(OnRecurrenceInfoChanged);
		}
		protected virtual void UnsubscribeRecurrencePatternControlEvents() {
			CurrentRecurrenceControl.RecurrenceInfoChanged -= new EventHandler(OnRecurrenceInfoChanged);
		}
		protected internal virtual void InitializeControls(FirstDayOfWeek firstDayOfWeek) {
			InitRecurrenceControls(firstDayOfWeek);
			this.RecurrenceType = RecurrenceInfo.Type;
			SetRecurrenceType(RecurrenceInfo.Type);	  
			ChangeCurrentRecurrenceControl();
			CurrentRecurrenceControl.RecurrenceInfo = RecurrenceInfo;
			UpdateRecurrenceInfoRange(RecurrenceInfo.Start, RecurrenceInfo.End, RecurrenceInfo.Range, RecurrenceInfo.OccurrenceCount);
			UnsubscribeRecurrencePatternControlEvents(); 
			edtStartTime.Time = ToClientTime(Pattern.Start);
			edtEndTime.Time = ToClientTime(Pattern.End);
			cbDuration.Duration = Pattern.Duration;
			UpdateRecurrenceRangeControlsCore(); 
			SubscribeAppointmentTimeControlsEvents();
			SubscribeRecurrenceTypeControlsEvents(); 
		}
		protected internal virtual void InitRecurrenceControls(FirstDayOfWeek firstDayOfWeek) {
			weeklyRecurrenceControl1.FirstDayOfWeek = firstDayOfWeek;
		}
		protected internal virtual RecurrenceType GetRecurrenceType() {
			return this.RecurrenceType;
		}
		protected internal virtual void SetRecurrenceType(RecurrenceType type) {
			switch (type) {
				default:
				case RecurrenceType.Daily:
					chkDaily.Checked = true;
					break;
				case RecurrenceType.Weekly:
					chkWeekly.Checked = true;
					break;
				case RecurrenceType.Monthly:
					chkMonthly.Checked = true;
					break;
				case RecurrenceType.Yearly:
					chkYearly.Checked = true;
					break;
			}
		}
		protected internal virtual void ResetRecurrenceInfo() {
			CurrentRecurrenceControl.UnsubscribeRecurrenceInfoEvents();
			try {
				RecurrenceType type = GetRecurrenceType();
				RecurrenceInfo.Reset(type);
			} finally {
				CurrentRecurrenceControl.SubscribeRecurrenceInfoEvents();
			}
		}
		protected internal virtual void UpdateRecurrenceInfoRange(DateTime start, DateTime end, RecurrenceRange rangeType, int occurrencesCount) {
			UnsubscribeRecurrencePatternControlEvents();
			try {
				((IInternalRecurrenceInfo)RecurrenceInfo).UpdateRange(start, end, rangeType, occurrencesCount, Pattern);
			} finally {
				SubscribeRecurrencePatternControlEvents();
			}
		}
		protected internal virtual void UpdateRecurrenceRangeControls() {
			RecurrenceRange rangeType = GetRangeType();
			int occurrencesCount = DevExpress.XtraScheduler.RecurrenceInfo.DefaultOccurrenceCount;
			if (rangeType == RecurrenceRange.OccurrenceCount) {
				try {
					occurrencesCount = Convert.ToInt32(spinRangeOccurrencesCount.EditValue);
				} catch {
				}
			}
			UpdateRecurrenceInfoRange(FromClientTime(edtRangeStart.DateTime), FromClientTime(edtRangeEnd.DateTime), rangeType, occurrencesCount);
			UpdateRecurrenceRangeControlsCore();
		}
		protected internal virtual void UpdateRecurrenceRangeControlsCore() {
			UnsubscribeRecurrenceRangeControlsEvents();
			try {
				edtRangeStart.DateTime = ToClientTime(RecurrenceInfo.Start);
				SetRangeType(RecurrenceInfo.Range);
				spinRangeOccurrencesCount.EditValue = RecurrenceInfo.OccurrenceCount;
				edtRangeEnd.DateTime = ToClientTime(RecurrenceInfo.End);
			} finally {
				SubscribeRecurrenceRangeControlsEvents();
			}
		}
		protected internal virtual void OnRecurrenceTypeEditValueChanged() {
			SuspendLayout();
			try {
				UnsubscribeRecurrencePatternControlEvents();
				try {
					if (CurrentRecurrenceControl != null)
						CurrentRecurrenceControl.RecurrenceInfo = null;
					ChangeCurrentRecurrenceControl();
					CurrentRecurrenceControl.RecurrenceInfo = RecurrenceInfo;
					UpdateRecurrenceRangeControls();
				} finally {
					SubscribeRecurrencePatternControlEvents();
				}
			} finally {
				ResumeLayout();
			}
		}
		protected internal virtual void chkEndTypeChanged(object sender, System.EventArgs e) {
			CheckEdit chkRecurrenceRangeType = (CheckEdit)sender;
			if (chkRecurrenceRangeType.Checked) {
				this.RecurrenceRange = (RecurrenceRange)chkRecurrenceRangeType.Tag;
				UpdateRecurrenceRangeControls();
			}
		}
		protected internal virtual void chkRecurrenceTypeChanged(object sender, System.EventArgs e) {
			CheckEdit chkRecurrenceType = (CheckEdit)sender;
			if (chkRecurrenceType.Checked) {
				this.RecurrenceType = (RecurrenceType)chkRecurrenceType.Tag;
				ResetRecurrenceInfo(); 
				OnRecurrenceTypeEditValueChanged();
			}
		}
		protected internal virtual void ChangeCurrentRecurrenceControl() {
			if (CurrentRecurrenceControl != null)
				CurrentRecurrenceControl.Visible = false;
			switch (GetRecurrenceType()) {
				case RecurrenceType.Daily:
					CurrentRecurrenceControl = dailyRecurrenceControl1;
					break;
				case RecurrenceType.Weekly:
					CurrentRecurrenceControl = weeklyRecurrenceControl1;
					break;
				case RecurrenceType.Monthly:
					CurrentRecurrenceControl = monthlyRecurrenceControl1;
					break;
				case RecurrenceType.Yearly:
				default:
					CurrentRecurrenceControl = yearlyRecurrenceControl1;
					break;
			}
			CurrentRecurrenceControl.Visible = true;
		}
		protected internal virtual void OnRecurrenceInfoChanged(object sender, EventArgs e) {
			UpdateRecurrenceRangeControls();
		}
		protected internal virtual void btnOk_Click(object sender, System.EventArgs e) {
			OnOkButton();
		}
		protected internal virtual DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return XtraMessageBox.Show(this, text, caption, buttons, icon);
		}
		protected internal virtual void OnOkButton() {
			if (ShowExceptionsRemoveMsgBox) {
				DialogResult result = ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_RecurrenceExceptionsWillBeLost), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (result == DialogResult.No)
					return;
			}
			ValidationArgs args = new ValidationArgs();
			ValidateValues(args);
			if (args.Valid) {
				args = new ValidationArgs();
				CurrentRecurrenceControl.CheckForWarnings(args);
				if (!args.Valid) {
					DialogResult answer = ShowMessageBox(args.ErrorMessage, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
					if (answer == DialogResult.OK)
						this.DialogResult = DialogResult.OK;
					else {
						FocusInvalidControl(args.Control as Control);
					}
				} else
					this.DialogResult = DialogResult.OK;
			} else {
				ShowMessageBox(args.ErrorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				FocusInvalidControl(args.Control as Control);
			}
		}
		protected virtual void FocusInvalidControl(Control control) {
			if (control != null)
				control.Focus();
		}
		#region Final validation
		protected internal virtual void ValidateValues(ValidationArgs args) {
			ValidateTimeAndDuration(args);
			if (!args.Valid)
				return;
			ValidateRecurrenceRange(args);
			if (!args.Valid)
				return;
			CurrentRecurrenceControl.ValidateValues(args);
		}
		protected internal virtual void ValidateTimeAndDuration(ValidationArgs args) {
		}
		protected internal virtual void ValidateRecurrenceRange(ValidationArgs args) {
		}
		#endregion
		protected internal virtual RecurrenceRange GetRangeType() {
			return this.RecurrenceRange;
		}
		protected internal virtual void SetRangeType(RecurrenceRange range) {
			this.RecurrenceRange = range;
			switch (range) {
				case RecurrenceRange.NoEndDate:
					chkNoEndDate.Checked = true;
					break;
				case RecurrenceRange.OccurrenceCount:
					chkEndAfterNumberOfOccurrences.Checked = true;
					break;
				case RecurrenceRange.EndByDate:
				default:
					chkEndByDate.Checked = true;
					break;
			}
		}
		protected internal virtual void OnStartModified() {
			UnsubscribeAppointmentTimeControlsEvents();
			try {
				CurrentRecurrenceControl.UnsubscribeRecurrenceInfoEvents();
				try {
					UnsubscribeRecurrenceRangeControlsEvents();
					try {
						DateTime start = edtRangeStart.DateTime.Date + edtStartTime.Time.TimeOfDay;
						DateTime end = start + Pattern.Duration;
						edtStartTime.Time = start;
						edtEndTime.Time = end;
						cbDuration.Duration = Pattern.Duration;
						Pattern.Start = FromClientTime(start);
						Pattern.RecurrenceInfo.Start = FromClientTime(start);
						edtRangeStart.DateTime = start;
						edtRangeEnd.DateTime = ToClientTime(Pattern.RecurrenceInfo.End);
					} finally {
						SubscribeRecurrenceRangeControlsEvents();
					}
				} finally {
					CurrentRecurrenceControl.SubscribeRecurrenceInfoEvents();
				}
			} finally {
				SubscribeAppointmentTimeControlsEvents();
			}
			UpdateRecurrenceRangeControls();
		}
		protected internal virtual void edtRangeStart_Validated(object sender, EventArgs e) {
			OnStartModified();
		}
		protected internal virtual void spinRangeOccurrencesCount_Validated(object sender, EventArgs e) {
			UpdateRecurrenceRangeControls();
		}
		protected internal virtual void edtRangeEnd_Validated(object sender, EventArgs e) {
			UpdateRecurrenceRangeControls();
		}
		protected internal virtual void edtRangeEnd_EditValueChanged(object sender, EventArgs e) {
			UnsubscribeRecurrenceRangeControlsEvents();
			try {
				SetRangeType(RecurrenceRange.EndByDate);
			} finally {
				SubscribeRecurrenceRangeControlsEvents();
			}
		}
		protected internal virtual void edtRangeEnd_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		protected internal virtual void edtRangeEnd_Validating(object sender, CancelEventArgs e) {
			e.Cancel = edtRangeEnd.DateTime < edtRangeStart.DateTime;
		}
		protected internal virtual void spinRangeOccurrencesCount_EditValueChanged(object sender, EventArgs e) {
			UnsubscribeRecurrenceRangeControlsEvents();
			try {
				SetRangeType(RecurrenceRange.OccurrenceCount);
			} finally {
				SubscribeRecurrenceRangeControlsEvents();
			}
		}
		protected internal virtual void spinRangeOccurrencesCount_Validating(object sender, CancelEventArgs e) {
			e.Cancel = !Validator.CheckPositiveValue(spinRangeOccurrencesCount.EditValue);
		}
		protected internal virtual void spinRangeOccurrencesCount_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = Validator.GetInvalidOccurrencesCountErrorMessage(spinRangeOccurrencesCount.EditValue);
		}
		protected internal virtual void edtStartTime_Validated(object sender, System.EventArgs e) {
			OnStartModified();
		}
		protected internal virtual void edtEndTime_Validated(object sender, System.EventArgs e) {
			DateTime endDateTime = edtEndTime.Time;
			if (endDateTime < edtStartTime.Time)
				endDateTime += DateTimeHelper.DaySpan;
			TimeSpan duration = endDateTime - edtStartTime.Time;
			UnsubscribeAppointmentTimeControlsEvents();
			try {
				CurrentRecurrenceControl.UnsubscribeRecurrenceInfoEvents();
				try {
					edtEndTime.Time = endDateTime;
					Pattern.Duration = duration;
					cbDuration.Duration = duration;
				} finally {
					CurrentRecurrenceControl.SubscribeRecurrenceInfoEvents();
				}
			} finally {
				SubscribeAppointmentTimeControlsEvents();
			}
		}
		protected internal virtual void cbDuration_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			TimeSpan span = cbDuration.Duration;
			e.Cancel = (span == TimeSpan.MinValue) || (span.Ticks < 0);
		}
		protected internal virtual void cbDuration_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidAppointmentDuration);
		}
		protected internal virtual void cbDuration_Validated(object sender, System.EventArgs e) {
			UnsubscribeAppointmentTimeControlsEvents();
			try {
				CurrentRecurrenceControl.UnsubscribeRecurrenceInfoEvents();
				try {
					TimeSpan duration = cbDuration.Duration;
					Pattern.Duration = duration;
					edtEndTime.Time = edtStartTime.Time + duration;
				} finally {
					CurrentRecurrenceControl.SubscribeRecurrenceInfoEvents();
				}
			} finally {
				SubscribeAppointmentTimeControlsEvents();
			}
		}
		void ResetPatternAllDay() {
			if (!Pattern.AllDay)
				return;
			Pattern.BeginUpdate();
			try {
				DateTime start = Pattern.Start;
				TimeSpan duration = Pattern.Duration;
				if (Pattern.AllDay && Controller != null)
					start = Controller.InnerControl.TimeZoneHelper.FromClientTime(start);
				Pattern.AllDay = false;
				Pattern.RecurrenceInfo.AllDay = false;
				Pattern.Start = start;
				Pattern.RecurrenceInfo.Start = start;
				Pattern.Duration = duration;
			} finally {
				Pattern.EndUpdate();
			}
		}
	}
}
