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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.iCalendar;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Printing.Native;
using DevExpress.XtraScheduler.UI;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblLocation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblShowTimeAs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.chkAllDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.btnDelete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.btnRecurrence")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtStartDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtEndDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtShowTimeAs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.tbSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.edtResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.chkReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.tbDescription")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.cbReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.tbLocation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.panel1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.progressPanel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.tbProgress")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblPercentComplete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentRibbonForm.lblPercentCompleteValue")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class AppointmentRibbonForm : DevExpress.XtraBars.Ribbon.RibbonForm, IDXManagerPopupMenu {
		#region Fields
		bool openRecurrenceForm;
		readonly ISchedulerStorage storage;
		readonly SchedulerControl control;
		Icon recurringIcon;
		Icon normalIcon;
		readonly AppointmentFormController controller;
		IDXMenuManager menuManager;
		bool supressCancelCore;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentRibbonForm() {
			InitializeComponent();
		}
		public AppointmentRibbonForm(DevExpress.XtraScheduler.SchedulerControl control, Appointment apt)
			: this(control, apt, false) {
		}
		public AppointmentRibbonForm(DevExpress.XtraScheduler.SchedulerControl control, Appointment apt, bool openRecurrenceForm) {
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
			this.edtResource.SchedulerControl = control;
			this.edtResource.Storage = storage;
			this.edtResources.SchedulerControl = control;
			this.riAppointmentResource.SchedulerControl = control;
			this.riAppointmentResource.Storage = storage;
			this.riAppointmentStatus.Storage = storage;
			this.riAppointmentLabel.Storage = storage;
			BindControllerToControls();
			this.supressCancelCore = false;
		}
		#region Properties
		public IDXMenuManager MenuManager { get { return menuManager; } private set { menuManager = value; } }
		protected internal AppointmentFormController Controller { get { return controller; } }
		protected internal SchedulerControl Control { get { return control; } }
		protected internal ISchedulerStorage Storage { get { return storage; } }
		protected internal bool IsNewAppointment { get { return controller != null ? controller.IsNewAppointment : true; } }
		protected internal Icon RecurringIcon { get { return recurringIcon; } }
		protected internal Icon NormalIcon { get { return normalIcon; } }
		protected internal bool OpenRecurrenceForm { get { return openRecurrenceForm; } }
		public bool ReadOnly {
			get { return Controller.ReadOnly; }
			set {
				if (Controller.ReadOnly == value)
					return;
				Controller.ReadOnly = value;
			}
		}
		protected override FormShowMode ShowMode { get { return DevExpress.XtraEditors.FormShowMode.AfterInitialization; } }
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
			this.DataBindings.Add("Text", Controller, "Caption");
			BindControllerToIcon();
			BindProperties(this.tbSubject, "Text", "Subject");
			BindProperties(this.tbLocation, "Text", "Location");
			BindProperties(this.tbDescription, "Text", "Description");
			BindProperties(this.edtStartDate, "EditValue", "DisplayStartDate");
			BindProperties(this.edtStartDate, "Enabled", "IsDateTimeEditable");
			BindProperties(this.edtStartTime, "EditValue", "DisplayStartTime");
			BindProperties(this.edtStartTime, "Enabled", "IsTimeEnabled");
			BindProperties(this.edtEndDate, "EditValue", "DisplayEndDate", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndDate, "Enabled", "IsDateTimeEditable", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndTime, "EditValue", "DisplayEndTime", DataSourceUpdateMode.Never);
			BindProperties(this.edtEndTime, "Enabled", "IsTimeEnabled", DataSourceUpdateMode.Never);
			BindProperties(this.chkAllDay, "Checked", "AllDay");
			BindProperties(this.chkAllDay, "Enabled", "IsDateTimeEditable");
			BindProperties(this.lblResource, "Enabled", "CanEditResource");
			BindProperties(this.edtResources, "ResourceIds", "ResourceIds");
			BindProperties(this.edtResources, "Visible", "ResourceSharing");
			BindProperties(this.edtResources, "Enabled", "CanEditResource");
			BindProperties(this.edtResource, "ResourceId", "ResourceId");
			BindProperties(this.edtResource, "Enabled", "CanEditResource");
			BindToBoolPropertyAndInvert(this.edtResource, "Visible", "ResourceSharing");
			BindProperties(this.barLabel, "EditValue", "Label");
			BindProperties(this.barStatus, "EditValue", "Status");
			BindBoolToVisibility(this.barReminder, "Visibility", "ReminderVisible");
			BindProperties(this.barReminder, "Editvalue", "ReminderTimeBeforeStart");
			BindProperties(this.tbProgress, "Value", "PercentComplete");
			BindProperties(this.lblPercentCompleteValue, "Text", "PercentComplete", ObjectToStringConverter);
			BindProperties(this.progressPanel, "Visible", "ShouldEditTaskProgress");
			BindProperties(this.btnDelete, "Enabled", "CanDeleteAppointment");
			BindBoolToVisibility(this.btnRecurrence, "Visibility", "ShouldShowRecurrenceButton");
			BindToBoolPropertyAndInvert(this.ribbonControl1, "Enabled", "ReadOnly");
			BindProperties(this.edtTimeZone, "Visible", "TimeZoneVisible");
			BindProperties(this.edtTimeZone, "EditValue", "TimeZoneId");
			BindProperties(this.edtTimeZone, "Enabled", "TimeZoneEnabled");
			BindBoolToVisibility(this.btnTimeZones, "Visibility", "TimeZonesEnabled");
			BindProperties(this.btnTimeZones, "Down", "TimeZoneVisible");
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
			BindToIsReadOnly(target, updateMode);
		}
		protected virtual void BindProperties(Control target, string targetProperty, string sourceProperty, ConvertEventHandler objectToStringConverter) {
			Binding binding = new Binding(targetProperty, Controller, sourceProperty, true);
			binding.Format += objectToStringConverter;
			target.DataBindings.Add(binding);
		}
		protected virtual void BindToBoolPropertyAndInvert(Control target, string targetProperty, string sourceProperty) {
			target.DataBindings.Add(new BoolInvertBinding(targetProperty, Controller, sourceProperty));
			BindToIsReadOnly(target);
		}
		protected virtual void BindToIsReadOnly(Control control) {
			BindToIsReadOnly(control, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected virtual void BindToIsReadOnly(Control control, DataSourceUpdateMode updateMode) {
			if ((!(control is BaseEdit)) || control.DataBindings["ReadOnly"] != null)
				return;
			control.DataBindings.Add("ReadOnly", Controller, "ReadOnly", true, updateMode);
		}
		protected virtual void BindProperties(DevExpress.XtraBars.BarItem target, string targetProperty, string sourceProperty) {
			BindProperties(target, targetProperty, sourceProperty, DataSourceUpdateMode.OnPropertyChanged);
		}
		protected virtual void BindProperties(DevExpress.XtraBars.BarItem target, string targetProperty, string sourceProperty, DataSourceUpdateMode updateMode) {
			target.DataBindings.Add(targetProperty, Controller, sourceProperty, true, updateMode);
		}
		protected virtual void BindProperties(DevExpress.XtraBars.BarItem target, string targetProperty, string sourceProperty, ConvertEventHandler objectToStringConverter) {
			Binding binding = new Binding(targetProperty, Controller, sourceProperty, true);
			binding.Format += objectToStringConverter;
			target.DataBindings.Add(binding);
		}
		protected virtual void BindToBoolPropertyAndInvert(DevExpress.XtraBars.BarItem target, string targetProperty, string sourceProperty) {
			target.DataBindings.Add(new BoolInvertBinding(targetProperty, Controller, sourceProperty));
		}
		protected virtual void BindBoolToVisibility(DevExpress.XtraBars.BarItem target, string targetProperty, string sourceProperty) {
			target.DataBindings.Add(new BoolToVisibilityBinding(targetProperty, Controller, sourceProperty, false));
		}
		protected virtual void BindBoolToVisibility(DevExpress.XtraBars.BarItem target, string targetProperty, string sourceProperty, bool invert) {
			target.DataBindings.Add(new BoolToVisibilityBinding(targetProperty, Controller, sourceProperty, invert));
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (Controller == null)
				return;
			SubscribeControlsEvents();
			LoadFormData(Controller.EditedAppointmentCopy);
		}
		protected virtual AppointmentFormController CreateController(SchedulerControl control, Appointment apt) {
			return new AppointmentFormController(control, apt);
		}
		protected internal virtual void LoadIcons() {
			Assembly asm = typeof(SchedulerControl).Assembly;
			this.recurringIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm);
			this.normalIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm);
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.edtEndDate.Validating += new CancelEventHandler(OnEdtEndDateValidating);
			this.edtEndDate.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtEndDateInvalidValue);
			this.edtEndTime.Validating += new CancelEventHandler(OnEdtEndTimeValidating);
			this.edtEndTime.InvalidValue += new InvalidValueExceptionEventHandler(OnEdtEndTimeInvalidValue);
			this.riDuration.Validating += new CancelEventHandler(OnCbReminderValidating);
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.edtEndDate.Validating -= new CancelEventHandler(OnEdtEndDateValidating);
			this.edtEndDate.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtEndDateInvalidValue);
			this.edtEndTime.Validating -= new CancelEventHandler(OnEdtEndTimeValidating);
			this.edtEndTime.InvalidValue -= new InvalidValueExceptionEventHandler(OnEdtEndTimeInvalidValue);
			this.riDuration.Validating -= new CancelEventHandler(OnCbReminderValidating);
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
			Save(true);
		}
		protected virtual void OnSaveButton() {
			Save(false);
		}
		private void Save(bool closeAfterSave) {
			if (!ValidateEndDateAndTime())
				return;
			if (!SaveFormData(Controller.EditedAppointmentCopy))
				return;
			if (!Controller.IsConflictResolved()) {
				ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (Controller.IsAppointmentChanged() || Controller.IsNewAppointment || IsAppointmentChanged(Controller.EditedAppointmentCopy))
				Controller.ApplyChanges();
			if (closeAfterSave)
				DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		private bool ValidateEndDateAndTime() {
			this.edtEndDate.DoValidate();
			this.edtEndTime.DoValidate();
			return String.IsNullOrEmpty(this.edtEndTime.ErrorText) && String.IsNullOrEmpty(this.edtEndDate.ErrorText);
		}
		protected virtual void OnSaveAsButton() {
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filter = "iCalendar files (*.ics)|*.ics";
			fileDialog.FilterIndex = 1;
			if (fileDialog.ShowDialog() != DialogResult.OK)
				return;
			try {
				using (Stream stream = fileDialog.OpenFile())
					ExportAppointment(stream);
			} catch {
				ShowMessageBox("Error: could not export appointments", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			} 
		}
		void ExportAppointment(Stream stream) {
			if (stream == null)
				return;
			AppointmentBaseCollection aptsToExport = new AppointmentBaseCollection();
			aptsToExport.Add(Controller.EditedAppointmentCopy);
			iCalendarExporter exporter = new iCalendarExporter(this.storage, aptsToExport);
			exporter.ProductIdentifier = "-//Developer Express Inc.";
			exporter.Export(stream);
		}
		protected internal virtual DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return XtraMessageBox.Show(this, text, caption, buttons, icon);
		}
		protected internal virtual void OnDeleteButton() {
			if (IsNewAppointment)
				return;
			Controller.DeleteAppointment();
			DialogResult = DialogResult.Abort;
			Close();
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
			this.btnRecurrence.Down = Controller.IsRecurrentAppointment;
		}
		protected virtual void OnCloseButton() {
			this.Close();
		}
		private bool CancelCore() {
			bool result = true;
			if (DialogResult != System.Windows.Forms.DialogResult.Abort && Controller != null && Controller.IsAppointmentChanged() && !this.supressCancelCore) {
				DialogResult dialogResult = ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_SaveBeforeClose), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
					result = false;
				else if (dialogResult == System.Windows.Forms.DialogResult.Yes) 
					Save(true);
			}
			return result;
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
			TimeSpan span = (TimeSpan)barReminder.EditValue;
			e.Cancel = span.Ticks < 0 && span != TimeSpan.MinValue;
			if (!e.Cancel)
				this.barReminder.DataBindings["EditValue"].WriteValue();
		}
		protected internal virtual void OnNextButton() {
			if (CancelCore()) {
				this.supressCancelCore = true;
				Control.SelectNextAppointment();
				if (Control.SelectedAppointments.Count > 0)
					Control.ShowAnotherEditAppointmentForm = true;
				this.Close();
			}
		}
		protected internal virtual void OnPreviousButton() {
			if (CancelCore()) {
				this.supressCancelCore = true;
				Control.SelectPrevAppointment();
				if (Control.SelectedAppointments.Count > 0)
					Control.ShowAnotherEditAppointmentForm = true;
				this.Close();
			}
		}
		protected internal virtual void OnTimeZonesButton() {
			Controller.TimeZoneVisible = !Controller.TimeZoneVisible;
		}
		protected virtual void OnApplicationButtonClick() {
			this.dvInfo.Document = Control.GetPrintPreviewDocument(new MemoPrintStyle());
			this.dvInfo.ExecCommand(DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToWholePage);
		}
		private void btnSaveAndClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			OnOkButton();
		}
		private void barButtonDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			OnDeleteButton();
		}
		private void barRecurrence_ItemClick(object sender, ItemClickEventArgs e) {
			OnRecurrenceButton();
		}
		private void bvbSave_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e) {
			OnSaveButton();
		}
		private void bvbSaveAs_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e) {
			OnSaveAsButton();
		}
		private void bvbClose_ItemClick(object sender, DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs e) {
			OnCloseButton();
		}
		private void btnSave_ItemClick(object sender, ItemClickEventArgs e) {
			OnSaveButton();
		}
		protected override void OnClosing(CancelEventArgs e) {
			e.Cancel = !CancelCore();
			base.OnClosing(e);
		}
		private void btnNext_ItemClick(object sender, ItemClickEventArgs e) {
			OnNextButton();
		}
		private void btnPrevious_ItemClick(object sender, ItemClickEventArgs e) {
			OnPreviousButton();
		}
		private void btnTimeZones_ItemClick(object sender, ItemClickEventArgs e) {
			OnTimeZonesButton();
		}
		private void ribbonControl1_ApplicationButtonClick(object sender, EventArgs e) {
			OnApplicationButtonClick();
		}
	}
	public class BoolToVisibilityBinding {
		public static implicit operator Binding(BoolToVisibilityBinding boolToVisibilityBinding) {
			Binding binding = new Binding(boolToVisibilityBinding.PropertyName, boolToVisibilityBinding.DataSource, boolToVisibilityBinding.DataMember);
			if (boolToVisibilityBinding.Invert) {
				binding.Format += ConvertWithInvert;
				binding.Parse += ConvertWithInvert;
			} else {
				binding.Format += Convert;
				binding.Parse += Convert;
			}
			return binding;
		}
		static void Convert(object sender, ConvertEventArgs e) {
			e.Value = BoolToVisibilityConverter.Convert((bool)e.Value);
		}
		static void ConvertWithInvert(object sender, ConvertEventArgs e) {
			e.Value = BoolToVisibilityConverter.Convert(!(bool)e.Value);
		}
		public BoolToVisibilityBinding(string propertyName, Object dataSource, string dataMember, bool invert) {
			PropertyName = propertyName;
			DataSource = dataSource;
			DataMember = dataMember;
			Invert = invert;
		}
		public string PropertyName { get; private set; }
		public Object DataSource { get; private set; }
		public string DataMember { get; private set; }
		public bool Invert { get; private set; }
	}
	public static class BoolToVisibilityConverter {
		public static BarItemVisibility Convert(bool value){
			if (value)
				return BarItemVisibility.Always;
			else
				return BarItemVisibility.Never;
		}
	}
}
