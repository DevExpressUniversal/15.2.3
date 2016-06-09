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
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Native;
using DevExpress.Utils.Internal;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblLocation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblShowTimeAs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.chkAllDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.btnDelete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.btnRecurrence")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtStartDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtEndDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtShowTimeAs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.tbSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.edtResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.chkReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.tbDescription")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.cbReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.tbLocation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.panel1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.progressPanel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.tbProgress")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblPercentComplete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentForm.lblPercentCompleteValue")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class AppointmentForm : DevExpress.XtraEditors.XtraForm, IDXManagerPopupMenu {
		#region Fields
		bool openRecurrenceForm;
		readonly ISchedulerStorage storage;
		readonly SchedulerControl control;
		Icon recurringIcon;
		Icon normalIcon;
		readonly AppointmentFormController controller;
		IDXMenuManager menuManager;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentForm() {
			InitializeComponent();
		}
		public AppointmentForm(SchedulerControl control, Appointment apt)
			: this(control, apt, false) {
		}
		public AppointmentForm(SchedulerControl control, Appointment apt, bool openRecurrenceForm) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control.Storage, "control.Storage");
			Guard.ArgumentNotNull(apt, "apt");
			this.openRecurrenceForm = openRecurrenceForm;
			this.controller = CreateController(control, apt);
			InitializeComponent();
			SetupPredefinedConstraints();
			LoadIcons();
			this.control = control;
			this.storage = control.Storage;
			this.edtShowTimeAs.Storage = this.storage;
			this.edtLabel.Storage = storage;
			this.edtResource.SchedulerControl = control;
			this.edtResource.Storage = storage;
			this.edtResources.SchedulerControl = control;
			SubscribeControllerEvents(Controller);
			BindControllerToControls();
		}
		#region Properties
		protected override FormShowMode ShowMode { get { return DevExpress.XtraEditors.FormShowMode.AfterInitialization; } }
		public IDXMenuManager MenuManager { get { return menuManager; } private set { menuManager = value; } }
		protected internal AppointmentFormController Controller { get { return controller; } }
		protected internal SchedulerControl Control { get { return control; } }
		protected internal ISchedulerStorage Storage { get { return storage; } }
		protected internal bool IsNewAppointment { get { return controller != null ? controller.IsNewAppointment : true; } }
		protected internal Icon RecurringIcon { get { return recurringIcon; } }
		protected internal Icon NormalIcon { get { return normalIcon; } }
		protected internal bool OpenRecurrenceForm { get { return openRecurrenceForm; } }
		public bool ReadOnly {
			get { return Controller != null && Controller.ReadOnly; }
			set {
				if (Controller.ReadOnly == value)
					return;
				Controller.ReadOnly = value;
			}
		}
		#endregion
		public virtual void LoadFormData(Appointment appointment) {
		}
		public virtual bool SaveFormData(Appointment appointment) {
			return true;
		}
		public virtual bool IsAppointmentChanged(Appointment appointment) {
			return false;
		}
		public virtual void SetMenuManager(DevExpress.Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
			this.menuManager = menuManager;
		}
		protected internal virtual void SetupPredefinedConstraints() {
			this.tbProgress.Properties.Minimum = AppointmentProcessValues.Min;
			this.tbProgress.Properties.Maximum = AppointmentProcessValues.Max;
			this.tbProgress.Properties.SmallChange = AppointmentProcessValues.Step;
			this.edtResources.Visible = true;
		}
		protected virtual void BindControllerToControls() {
			BindControllerToIcon();
			BindProperties(this.tbSubject, "Text", "Subject");
			BindProperties(this.tbLocation, "Text", "Location");
			BindProperties(this.tbDescription, "Text", "Description");
			BindProperties(this.edtShowTimeAs, "Status", "Status");
			BindProperties(this.edtStartDate, "EditValue", "DisplayStartDate");
			BindProperties(this.edtStartDate, "Enabled", "IsDateTimeEditable");
			BindProperties(this.edtStartTime, "EditValue", "DisplayStartTime");
			BindProperties(this.edtStartTime, "Visible", "IsTimeVisible");
			BindProperties(this.edtStartTime, "Enabled", "IsTimeVisible");
			BindProperties(this.edtEndDate, "EditValue", "DisplayEndDate", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndDate, "Enabled", "IsDateTimeEditable", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndTime, "EditValue", "DisplayEndTime", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndTime, "Visible", "IsTimeVisible", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndTime, "Enabled", "IsTimeVisible", DataSourceUpdateMode.Never);
			BindProperties(this.chkAllDay, "Checked", "AllDay");
			BindProperties(this.chkAllDay, "Enabled", "IsDateTimeEditable");
			BindProperties(this.edtResource, "ResourceId", "ResourceId");
			BindProperties(this.edtResource, "Enabled", "CanEditResource");
			BindToBoolPropertyAndInvert(this.edtResource, "Visible", "ResourceSharing");
			BindProperties(this.edtResources, "ResourceIds", "ResourceIds");
			BindProperties(this.edtResources, "Visible", "ResourceSharing");
			BindProperties(this.edtResources, "Enabled", "CanEditResource");
			BindProperties(this.lblResource, "Enabled", "CanEditResource");
			BindProperties(this.edtLabel, "Label", "Label");
			BindProperties(this.chkReminder, "Enabled", "ReminderVisible");
			BindProperties(this.chkReminder, "Visible", "ReminderVisible");
			BindProperties(this.chkReminder, "Checked", "HasReminder");
			BindProperties(this.cbReminder, "Enabled", "HasReminder");
			BindProperties(this.cbReminder, "Visible", "ReminderVisible");
			BindProperties(this.cbReminder, "Duration", "ReminderTimeBeforeStart");
			BindProperties(this.tbProgress, "Value", "PercentComplete");
			BindProperties(this.lblPercentCompleteValue, "Text", "PercentComplete", ObjectToStringConverter);
			BindProperties(this.progressPanel, "Visible", "ShouldEditTaskProgress");
			BindToBoolPropertyAndInvert(this.btnOk, "Enabled", "ReadOnly");
			BindToBoolPropertyAndInvert(this.btnRecurrence, "Enabled", "ReadOnly");
			BindProperties(this.btnDelete, "Enabled", "CanDeleteAppointment");
			BindProperties(this.btnRecurrence, "Visible", "ShouldShowRecurrenceButton");
		}
		protected virtual void BindControllerToIcon() {
			Binding binding = new Binding("Icon", Controller, "AppointmentType");
			binding.Format += AppointmentTypeToIconConverter;
			DataBindings.Add(binding);
		}
		protected virtual void ObjectToStringConverter(object o, ConvertEventArgs e) {
			e.Value = e.Value.ToString();
		}
		protected virtual void AppointmentTypeToIconConverter(object o, ConvertEventArgs e) {
			AppointmentType type = (AppointmentType)e.Value;
			if (type == AppointmentType.Pattern)
				e.Value = RecurringIcon;
			else
				e.Value = NormalIcon;
		}
		protected virtual void BindProperties(Control target, string targetProperty, string sourceProperty) {
			BindProperties(target, targetProperty, sourceProperty, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected virtual void BindProperties(Control target, string targetProperty, string sourceProperty, DataSourceUpdateMode updateMode) {
			target.DataBindings.Add(targetProperty, Controller, sourceProperty, true, updateMode);
		}
		protected virtual void BindProperties(Control target, string targetProperty, string sourceProperty, ConvertEventHandler objectToStringConverter) {
			Binding binding = new Binding(targetProperty, Controller, sourceProperty, true);
			binding.Format += objectToStringConverter;
			target.DataBindings.Add(binding);
		}
		protected virtual void BindToBoolPropertyAndInvert(Control target, string targetProperty, string sourceProperty) {
			target.DataBindings.Add(new BoolInvertBinding(targetProperty, Controller, sourceProperty));
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (Controller == null)
				return;
			this.DataBindings.Add("Text", Controller, "Caption");
			SubscribeControlsEvents();
			LoadFormData(Controller.EditedAppointmentCopy);
			RecalculateLayoutOfControlsAffectedByProgressPanel();
		}
		protected virtual AppointmentFormController CreateController(SchedulerControl control, Appointment apt) {
			return new AppointmentFormController(control, apt);
		}
		void SubscribeControllerEvents(AppointmentFormController controller) {
			if (controller == null)
				return;
			controller.PropertyChanged += OnControllerPropertyChanged;
		}
		void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "ReadOnly")
				UpdateReadonly();
		}
		protected virtual void UpdateReadonly() {
			if (Controller == null)
				return;
			IList<Control> controls = GetAllControls(this);
			foreach (Control control in controls) {
				BaseEdit editor = control as BaseEdit;
				if (editor == null)
					continue;
				editor.ReadOnly = Controller.ReadOnly;
			}
			this.btnOk.Enabled = !Controller.ReadOnly;
			this.btnRecurrence.Enabled = !Controller.ReadOnly;
		}
		List<Control> GetAllControls(Control rootControl) {
			List<Control> result = new List<Control>();
			foreach (Control control in rootControl.Controls) {
				result.Add(control);
				IList<Control> childControls = GetAllControls(control);
				result.AddRange(childControls);
			}
			return result;
		}
		protected internal virtual void LoadIcons() {
			Assembly asm = typeof(SchedulerControl).Assembly;
			recurringIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm);
			normalIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm);
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.edtEndDate.Validating += new CancelEventHandler(OnEdtEndDateValidating);
			this.edtEndDate.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtEndDateInvalidValue);
			this.edtEndTime.Validating += new CancelEventHandler(OnEdtEndTimeValidating);
			this.edtEndTime.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtEndTimeInvalidValue);
			this.cbReminder.InvalidValue += new InvalidValueExceptionEventHandler(OnCbReminderInvalidValue);
			this.cbReminder.Validating += new CancelEventHandler(OnCbReminderValidating);
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.edtEndDate.Validating -= new CancelEventHandler(OnEdtEndDateValidating);
			this.edtEndDate.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtEndDateInvalidValue);
			this.edtEndTime.Validating -= new CancelEventHandler(OnEdtEndTimeValidating);
			this.edtEndTime.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtEndTimeInvalidValue);
			this.cbReminder.InvalidValue -= new InvalidValueExceptionEventHandler(OnCbReminderInvalidValue);
			this.cbReminder.Validating -= new CancelEventHandler(OnCbReminderValidating);
		}
		void OnBtnOkClick(object sender, System.EventArgs e) {
			OnOkButton();
		}
		protected internal virtual void OnEdtEndDateValidating(object sender, CancelEventArgs e) {
			e.Cancel = !IsValidInterval();
			if (!e.Cancel)
				this.edtEndDate.DataBindings["EditValue"].WriteValue();
		}
		protected internal virtual void OnEdtEndDateInvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		protected internal virtual void OnEdtEndTimeValidating(object sender, CancelEventArgs e) {
			e.Cancel = !IsValidInterval();
			if (!e.Cancel)
				this.edtEndTime.DataBindings["EditValue"].WriteValue();
		}
		protected internal virtual void OnEdtEndTimeInvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		protected internal virtual bool IsValidInterval() {
			return AppointmentFormControllerBase.ValidateInterval(edtStartDate.DateTime.Date, edtStartTime.Time.TimeOfDay, edtEndDate.DateTime.Date, edtEndTime.Time.TimeOfDay);
		}
		protected internal virtual void OnOkButton() {
			if (!SaveFormData(Controller.EditedAppointmentCopy))
				return;
			if (!Controller.IsConflictResolved()) {
				ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (Controller.IsAppointmentChanged() || Controller.IsNewAppointment || IsAppointmentChanged(Controller.EditedAppointmentCopy))
				Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		protected internal virtual DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return XtraMessageBox.Show(this, text, caption, buttons, icon);
		}
		void OnBtnDeleteClick(object sender, System.EventArgs e) {
			OnDeleteButton();
		}
		protected internal virtual void OnDeleteButton() {
			if (IsNewAppointment)
				return;
			Controller.DeleteAppointment();
			DialogResult = DialogResult.Abort;
			Close();
		}
		void OnBtnRecurrenceClick(object sender, System.EventArgs e) {
			OnRecurrenceButton();
		}
		protected internal virtual void OnRecurrenceButton() {
			if (!Controller.ShouldShowRecurrenceButton)
				return;
			Appointment patternCopy = Controller.PrepareToRecurrenceEdit();
			DialogResult result;
			using (Form form = CreateAppointmentRecurrenceForm(patternCopy, Control.OptionsView.FirstDayOfWeek)) {
				result = ShowRecurrenceForm(form);
			}
			if (result == DialogResult.Abort) {
				Controller.RemoveRecurrence();
			} else if (result == DialogResult.OK) {
				Controller.ApplyRecurrence(patternCopy);
			}
		}
		protected virtual DialogResult ShowRecurrenceForm(Form form) {
			return FormTouchUIAdapter.ShowDialog(form, this);
		}
		protected internal virtual Form CreateAppointmentRecurrenceForm(Appointment patternCopy, FirstDayOfWeek firstDayOfWeek) {
			AppointmentRecurrenceForm form = new AppointmentRecurrenceForm(patternCopy, firstDayOfWeek, Controller);
			form.SetMenuManager(MenuManager);
			form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			form.ShowExceptionsRemoveMsgBox = controller.AreExceptionsPresent();
			return form;
		}
		internal void OnAppointmentFormActivated(object sender, EventArgs e) {
			if (openRecurrenceForm) {
				openRecurrenceForm = false;
				OnRecurrenceButton();
			}
		}
		protected internal virtual void OnCbReminderValidating(object sender, CancelEventArgs e) {
			TimeSpan span = cbReminder.Duration;
			e.Cancel = (span == TimeSpan.MinValue) || (span.Ticks < 0);
			if (!e.Cancel)
				this.cbReminder.DataBindings["Duration"].WriteValue();
		}
		protected internal virtual void OnCbReminderInvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidReminderTimeBeforeStart);
		}
		protected internal virtual void RecalculateLayoutOfControlsAffectedByProgressPanel() {
			if (progressPanel.Visible)
				return;
			int intDeltaY = progressPanel.Height;
			tbDescription.Location = new Point(tbDescription.Location.X, tbDescription.Location.Y - intDeltaY);
			tbDescription.Size = new Size(tbDescription.Size.Width, tbDescription.Size.Height + intDeltaY);
		}
	}
	public class AppointmentFormController : AppointmentFormControllerBase {
		readonly SchedulerControl control;
		public AppointmentFormController(SchedulerControl control, Appointment apt)
			: base(control.InnerControl, apt) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.PropertyChanged += OnAppointmentFormControllerPropertyChanged;
		}
		protected internal SchedulerControl Control { get { return control; } }
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentFormControllerShouldEditTaskProgress")]
#endif
		public virtual bool ShouldEditTaskProgress {
			get { return Control.ActiveViewType == SchedulerViewType.Gantt; }
		}
		#region PercentComplete
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("AppointmentFormControllerPercentComplete")]
#endif
		public int PercentComplete {
			get { return EditedAppointmentCopy.PercentComplete; }
			set {
				if (EditedAppointmentCopy.PercentComplete == value)
					return;
				EditedAppointmentCopy.PercentComplete = value;
				OnPercentCompleteChanged();
			}
		}
		protected virtual void OnPercentCompleteChanged() {
			NotifyPropertyChanged("PercentComplete");
		}
		#endregion
		#region Label
		public AppointmentLabel Label {
			get { return GetLabel(); }
			set { SetLabel(value); }
		}
		public AppointmentLabel GetLabel() {
			return (AppointmentLabel)base.GetInnerLabel();
		}
		#endregion
		#region Status
		public AppointmentStatus Status {
			get { return GetStatus(); }
			set { SetStatus(value); }
		}
		public AppointmentStatus GetStatus() {
			return (AppointmentStatus)base.GetInnerStatus();
		}
		public AppointmentStatus UpdateAppointmentStatus(AppointmentStatus currentStatus) {
			return base.UpdateAppointmentStatusCore(currentStatus) as AppointmentStatus;
		}
		#endregion
		#region DisplayStartDate
		public DateTime DisplayStartDate {
			get { return DisplayStart.Date; }
			set {
				if (DisplayStart.Date == value)
					return;
				DisplayStart = value + DisplayStartTime.TimeOfDay;
				NotifyPropertyChanged("DisplayStartDate");
			}
		}
		#endregion
		#region DisplayStartTime
		public DateTime DisplayStartTime {
			get { return new DateTime(DisplayStart.TimeOfDay.Ticks); }
			set {
				if (DisplayStart.TimeOfDay == value.TimeOfDay)
					return;
				DisplayStart = DisplayStartDate + value.TimeOfDay;
				NotifyPropertyChanged("DisplayStartTime");
			}
		}
		#endregion
		#region DisplayEndDate
		public DateTime DisplayEndDate {
			get { return DisplayEnd.Date; }
			set {
				DateTime newDate = value.Date;
				if (DisplayEnd.Date == newDate)
					return;
				DisplayEnd = newDate + DisplayEndTime.TimeOfDay;
				NotifyPropertyChanged("DisplayEndDate");
			}
		}
		#endregion
		#region DisplayEndTime
		public DateTime DisplayEndTime {
			get { return new DateTime(DisplayEnd.TimeOfDay.Ticks); }
			set {
				TimeSpan newTime = value.TimeOfDay;
				if (DisplayEnd.TimeOfDay == newTime)
					return;
				DisplayEnd = DisplayEndDate + newTime;
				NotifyPropertyChanged("DisplayEndTime");
			}
		}
		#endregion
		public bool IsTimeVisible {
			get { return IsDateTimeEditable; }
		}
		public bool ReminderVisible { get { return Control.RemindersEnabled; } }
		public override void ApplyChanges() {
			ForceSyncMode();
			base.ApplyChanges();
			ResetForceSyncMode();
		}
		protected void ForceSyncMode() {
			IViewAsyncSupport viewAsyncSupport = Control.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null || Control.ActiveView.ViewInfo == null)
				return;
			Control.ActiveView.ThreadManager.WaitForAllThreads();
			viewAsyncSupport.ForceSyncMode();
		}
		protected void ResetForceSyncMode() {
			IViewAsyncSupport viewAsyncSupport = Control.ActiveView as IViewAsyncSupport;
			if (viewAsyncSupport == null || Control.ActiveView.ViewInfo == null)
				return;
			Control.ActiveView.ThreadManager.WaitForAllThreads();
			viewAsyncSupport.ResetForceSyncMode();
		}
		void OnAppointmentFormControllerPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "AppointmentType" || e.PropertyName == "AllDay")
				NotifyPropertyChanged("IsTimeVisible");
			else if (e.PropertyName == "AppointmentType")
				NotifyPropertyChanged("IsDateTimeEditable");
			else if (e.PropertyName == "Start") {
				NotifyPropertyChanged("DisplayStartDate");
				NotifyPropertyChanged("DisplayStartTime");
			} else if (e.PropertyName == "End") {
				NotifyPropertyChanged("DisplayEndDate");
				NotifyPropertyChanged("DisplayEndTime");
			}
		}
#if DEBUGTEST
		public void NotifyAll() {
			NotifyPropertyChanged("Start");
			NotifyPropertyChanged("End");
			NotifyPropertyChanged("ShouldShowRecurrenceButton");
			NotifyPropertyChanged("CanDeleteAppointment");
			NotifyPropertyChanged("ReadOnly");
		}
#endif
	}
	public class BoolInvertBinding {
		public static implicit operator Binding(BoolInvertBinding invertBinding) {
			Binding binding = new Binding(invertBinding.PropertyName, invertBinding.DataSource, invertBinding.DataMember);
			binding.Format += Convert;
			binding.Parse += Convert;
			return binding;
		}
		static void Convert(object sender, ConvertEventArgs e) {
			e.Value = !(bool)e.Value;
		}
		public BoolInvertBinding(string propertyName, Object dataSource, string dataMember) {
			PropertyName = propertyName;
			DataSource = dataSource;
			DataMember = dataMember;
		}
		public string PropertyName { get; private set; }
		public Object DataSource { get; private set; }
		public string DataMember { get; private set; }
	}
}
