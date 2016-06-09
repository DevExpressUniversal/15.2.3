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
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using System.Reflection;
using DevExpress.Utils.Controls;
using System.Windows.Forms;
using System.Globalization;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraScheduler.Extensions;
using DevExpress.Utils;
using DevExpress.XtraSpellChecker;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.ribbonControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.btnSave")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.btnDelete")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.btnRecurrence")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.btnSpelling")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.btnClose")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.btnSaveAndClose")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.ribPageAppointment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.pgrActions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.pgrOptions")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.pgrProofing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.spellChecker")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.applicationMenu")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.barAndDockingController")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.chkReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.chkAllDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.edtEndDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.edtStartDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.edtStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.edtEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.tbSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.tbLocation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.edtResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.edtResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.tbDescription")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.cbReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.lblInfo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutCtrl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutControlGroup")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutLocation")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutSubject")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutEndDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutStartDate")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutDescription")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutStartTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutEndTime")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutChkReminder")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutAllDay")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutInfo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutReminderGroup")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutStartGroup")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutEndGroup")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutResourcesGroup")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.lblHorzSeparator1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutHorzSeparator1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.lblHorzSeparator2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.layoutHorzSeparator2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.emptySpaceItem1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.repItemAppointmentStatus")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.repItemAppointmentLabel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.barEditShowTimeAs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.UI.AppointmentFormRibbonStyle.barEditLabelAs")]
#endregion
namespace DevExpress.XtraScheduler.UI {
	[System.Runtime.InteropServices.ComVisible(false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1301:AvoidDuplicateAccelerators")]
	public partial class AppointmentFormRibbonStyle : RibbonForm {
		#region Fields
		bool openRecurrenceForm;
		bool readOnly;
		ISchedulerStorageBase storage;
		SchedulerControl control;
		Icon recurringIcon;
		Icon normalIcon;
		Image closeImage;
		Image saveImage;
		Image saveAndCloseImage;
		Image deleteImage;
		Image spellingImage;
		Image recurrenceImage;
		Image closeLargeImage;
		Image saveLargeImage;
		Image saveAndCloseLargeImage;
		Image deleteLargeImage;
		Image spellingLargeImage;
		Image recurrenceLargeImage;
		AppointmentFormController controller;
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentFormRibbonStyle()
			: base() {
			InitializeComponent();
		}
		public AppointmentFormRibbonStyle(SchedulerControl control, Appointment apt)
			: this(control, apt, false) {
		}
		public AppointmentFormRibbonStyle(SchedulerControl control, Appointment apt, bool openRecurrenceForm)
			: base() {
			if (control == null)
				Exceptions.ThrowArgumentException("control", control);
			if (control.DataStorage == null)
				Exceptions.ThrowArgumentException("control.Storage", control.DataStorage);
			if (apt == null)
				Exceptions.ThrowArgumentException("apt", apt);
			this.openRecurrenceForm = openRecurrenceForm;
			this.controller = CreateController(control, apt);
			InitializeComponent();
			SchedulerStorage storage = (SchedulerStorage)Controller.InnerControl.Storage;
			repItemAppointmentStatus.Storage = storage;
			repItemAppointmentLabel.Storage = storage;
			edtResource.SchedulerControl = control;
			edtResource.Storage = storage;
			edtResources.SchedulerControl = control;
			barAndDockingController.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.spellChecker.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			LoadIcons();
			LoadImages();
			this.control = control;
			this.storage = control.Storage;
		}
		#region Properties
		protected internal AppointmentFormController Controller { get { return controller; } }
		protected internal SchedulerControl Control { get { return control; } }
		protected internal ISchedulerStorageBase Storage { get { return storage; } }
		protected internal bool IsNewAppointment { get { return controller != null ? controller.IsNewAppointment : true; } }
		protected internal Icon RecurringIcon { get { return recurringIcon; } }
		protected internal Icon NormalIcon { get { return normalIcon; } }
		protected internal bool OpenRecurrenceForm { get { return openRecurrenceForm; } }
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if (readOnly == value)
					return;
				readOnly = value;
				UpdateForm();
			}
		}
		public SpellChecker Speller { get { return spellChecker; } }
		#endregion
		#region Events
		#region NotifyChangeCaption
		EventHandler onNotifyChangeCaption;
		protected event EventHandler NotifyChangeCaption { add { onNotifyChangeCaption += value; } remove { onNotifyChangeCaption -= value; } }
		protected internal virtual void RaiseNotifyChangeCaption() {
			if (onNotifyChangeCaption != null) {
				onNotifyChangeCaption(this, EventArgs.Empty);
			}
		}
		#endregion
		#endregion
		protected internal virtual AppointmentFormController CreateController(SchedulerControl control, Appointment apt) {
			return new AppointmentFormController(control, apt);
		}
		protected internal virtual void LoadIcons() {
			Assembly asm = typeof(SchedulerControl).Assembly;
			this.recurringIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm);
			this.normalIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm);
		}
		protected internal virtual void LoadImages() {
			Assembly asm = Assembly.GetExecutingAssembly();
			this.saveImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.Save, asm);
			this.saveLargeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.SaveLarge, asm);
			this.saveAndCloseImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.SaveAndClose, asm);
			this.saveAndCloseLargeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.SaveAndCloseLarge, asm);
			this.deleteImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.Delete, asm);
			this.deleteLargeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.DeleteLarge, asm);
			this.recurrenceImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.Recurrence, asm);
			this.recurrenceLargeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.RecurrenceLarge, asm);
			this.closeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.Close, asm);
			this.closeLargeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.CloseLarge, asm);
			this.spellingImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.Spelling, asm);
			this.spellingLargeImage = ResourceImageHelper.CreateImageFromResources(AppointmentFormImagesName.SpellingLarge, asm);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			UpdateControl();
			UpdateCustomFieldsControls();
			UpdateFormCaption();
			UpdateRibbonPageCaption();
			UpdateDeleteButton();
			UpdateRecurrenceButton();
			UpdateIcon();
		}
		protected internal virtual void UpdateCustomFieldsControls() {
		}
		protected internal virtual void UpdateFormCaption() {
			this.Text = FormatAppointmentFormCaption();
		}
		protected internal virtual string FormatAppointmentFormCaption() {
			string formCaptionPart = FormatAppointmentRibbonPageCaption();
			string formCaption = Controller.EditedAppointmentCopy.Subject;
			if (formCaption == null || formCaption.Length == 0)
				formCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_UntitledAppointment);
			formCaption = string.Format("{0} - {1}", formCaption, formCaptionPart);
			if (ReadOnly)
				formCaption += SchedulerLocalizer.GetString(SchedulerStringId.Caption_ReadOnly);
			return formCaption;
		}
		protected internal virtual string FormatAppointmentRibbonPageCaption() {
			bool recApt = Controller.EditedPattern == null ? false : true;
			bool allDayApt = Controller.AllDay;
			if (recApt && !allDayApt)
				return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_RecurringAppointment);
			if (recApt && allDayApt)
				return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_RecurringEvent);
			if (!recApt && !allDayApt)
				return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_Appointment);
			if (!recApt && allDayApt)
				return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_Event);
			return string.Empty;
		}
		protected internal virtual void UpdateRibbonPageCaption() {
			this.ribPageAppointment.Text = FormatAppointmentRibbonPageCaption();
		}
		protected internal virtual void UpdateDeleteButton() {
			btnDelete.Enabled = Controller.CanDeleteAppointment;
		}
		protected internal virtual void UpdateRecurrenceButton() {
			bool showRecurrenceButton = controller.ShouldShowRecurrenceButton;
			btnRecurrence.Enabled = showRecurrenceButton;
		}
		protected internal virtual void UpdateIcon() {
			if (controller.EditedAppointmentCopy.Type == AppointmentType.Pattern)
				this.Icon = recurringIcon;
			else
				this.Icon = normalIcon;
		}
		protected internal virtual void SubscribeControlsEvents() {
			NotifyChangeCaption += new EventHandler(aptFormCtrl_NotifyChangeCaption);
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			NotifyChangeCaption -= new EventHandler(aptFormCtrl_NotifyChangeCaption);
		}
		protected internal virtual void aptFormCtrl_NotifyChangeCaption(object sender, EventArgs e) {
			UpdateFormCaption();
			UpdateRibbonPageCaption();
		}
		protected internal virtual void UpdateImages() {
			this.btnSave.Glyph = saveImage;
			this.btnSave.LargeGlyph = saveLargeImage;
			this.btnSaveAndClose.Glyph = saveAndCloseImage;
			this.btnSaveAndClose.LargeGlyph = saveAndCloseLargeImage;
			this.btnRecurrence.Glyph = recurrenceImage;
			this.btnRecurrence.LargeGlyph = recurrenceLargeImage;
			this.btnDelete.Glyph = deleteImage;
			this.btnDelete.LargeGlyph = deleteLargeImage;
			this.btnSpelling.Glyph = spellingImage;
			this.btnSpelling.LargeGlyph = spellingLargeImage;
			this.btnClose.Glyph = closeImage;
			this.btnClose.LargeGlyph = closeLargeImage;
		}
		protected internal void btnDelete_Click(object sender, ItemClickEventArgs e) {
			OnDeleteButton();
		}
		protected internal virtual void OnDeleteButton() {
			if (IsNewAppointment)
				return;
			Controller.DeleteAppointment();
			DialogResult = DialogResult.Abort;
			Close();
		}
		protected internal void btnSave_Click(object sender, ItemClickEventArgs e) {
			OnSaveButton();
		}
		protected internal void btnSaveAndClose_Click(object sender, ItemClickEventArgs e) {
			OnSaveButton();
			this.DialogResult = DialogResult.OK;
		}
		protected internal virtual void OnSaveButton() {
			DoValidation();
			if (!controller.IsConflictResolved()) {
				ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			controller.ApplyChanges();
			UpdateDeleteButton();
		}
		protected internal void btnClose_Click(object sender, ItemClickEventArgs e) {
			DialogResult = DialogResult.Abort;
		}
		protected internal void btnSpelling_Click(object sender, ItemClickEventArgs e) {
			spellChecker.CheckContainer();
		}
		protected void btnRecurrence_Click(object sender, ItemClickEventArgs e) {
			OnRecurrenceButton();
		}
		protected internal virtual void OnRecurrenceButton() {
			if (!Controller.ShouldShowRecurrenceButton)
				return;
			DoValidation();
			Appointment patternCopy = controller.PrepareToRecurrenceEdit();
			DialogResult result;
			using (Form form = CreateAppointmentRecurrenceForm(patternCopy, control.OptionsView.FirstDayOfWeek)) {
				result = ShowRecurrenceForm(form);
			}
			if (result == DialogResult.Abort) {
				Controller.RemoveRecurrence();
				UpdateIntervalControls();
				UpdateIcon();
				UpdateFormCaption();
				UpdateRibbonPageCaption();
			}
			else if (result == DialogResult.OK) {
				Controller.ApplyRecurrence(patternCopy);
				UpdateForm();
			}
		}
		protected internal virtual Form CreateAppointmentRecurrenceForm(Appointment patternCopy, FirstDayOfWeek firstDayOfWeek) {
			AppointmentRecurrenceForm form = new AppointmentRecurrenceForm(patternCopy, firstDayOfWeek, Controller);
			form.SetMenuManager(this.ribbonControl);
			form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			form.ShowExceptionsRemoveMsgBox = controller.AreExceptionsPresent();
			return form;
		}
		protected virtual DialogResult ShowRecurrenceForm(Form form) {
			return form.ShowDialog(this);
		}
		protected internal void AppointmentFormBase_Activated(object sender, EventArgs e) {
			if (openRecurrenceForm) {
				openRecurrenceForm = false;
				OnRecurrenceButton();
			}
		}
		protected internal void AppointmentFormBase_Load(object sender, EventArgs e) {
			if (((AppointmentFormRibbonStyle)sender).Controller == null)
				return;
			spellChecker.Culture = CultureInfo.CurrentCulture;
			UpdateImages();
			UpdateForm();
			RestoreControlLayout(GetLayoutRegistryPath());
			SetFocus();
		}
		protected internal void AppointmentFormBase_FormClosing(object sender, FormClosingEventArgs e) {
			SaveControlLayout(GetLayoutRegistryPath());
		}
		protected internal virtual string GetLayoutRegistryPath() {
			return String.Format("{0}\\{1}_Layout", Application.ProductName + Application.ProductVersion, GetType().Name);
		}
		protected internal virtual DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return XtraMessageBox.Show(this, text, caption, buttons, icon);
		}
		protected virtual void DoValidation() {
			Validate();
		}
		#region AppointmentPageControls
		public virtual void UpdateControl() {
			UnsubscribeAppointmentPageControlsEvents();
			try {
				UpdateControlCore();
			}
			finally {
				SubscribeAppointmentPageControlsEvents();
			}
		}
		protected virtual void UpdateControlCore() {
			MakeControlsReadOnly(false);
			tbSubject.Text = Controller.Subject;
			tbLocation.Text = Controller.Location;
			tbDescription.Text = Controller.Description;
			barEditShowTimeAs.EditValue = Controller.GetStatus();
			edtResource.ResourceId = Controller.ResourceId;
			AppointmentResourceIdCollection resourceIds = edtResources.ResourceIds;
			resourceIds.BeginUpdate();
			try {
				resourceIds.Clear();
				resourceIds.AddRange(Controller.ResourceIds);
			}
			finally {
				resourceIds.EndUpdate();
			}
			SchedulerControl control = (SchedulerControl)Controller.InnerControl.Owner;
			barEditLabelAs.EditValue = Controller.GetLabel();
			bool remindersEnabled = control.RemindersEnabled;
			chkReminder.Enabled = remindersEnabled;
			chkReminder.Visible = remindersEnabled;
			chkReminder.Checked = Controller.HasReminder;
			UpdateReminderCombo();
			UpdateIntervalControlsCore();
			UpdateCustomFieldsControls();
			RaiseNotifyChangeCaption();
			bool resourceSharing = Controller.ResourceSharing;
			edtResource.Visible = !resourceSharing;
			edtResources.Visible = resourceSharing;
			if (resourceSharing) {
				layoutResources.Visibility = LayoutVisibility.Always;
				layoutResource.Visibility = LayoutVisibility.OnlyInCustomization;
			}
			else {
				layoutResources.Visibility = LayoutVisibility.OnlyInCustomization;
				layoutResource.Visibility = LayoutVisibility.Always;
			}
			bool canEditResource = Controller.CanEditResource;
			edtResource.Enabled = canEditResource;
			edtResources.Enabled = canEditResource;
			if (ReadOnly)
				MakeControlsReadOnly(ReadOnly);
		}
		protected internal virtual void MakeControlsReadOnly(bool readOnly) {
			tbLocation.Properties.ReadOnly = readOnly;
			chkAllDay.Properties.ReadOnly = readOnly;
			edtStartDate.Properties.ReadOnly = readOnly;
			edtEndDate.Properties.ReadOnly = readOnly;
			edtStartTime.Properties.ReadOnly = readOnly;
			edtEndTime.Properties.ReadOnly = readOnly;
			barEditLabelAs.Edit.ReadOnly = readOnly;
			barEditShowTimeAs.Edit.ReadOnly = readOnly;
			tbSubject.Properties.ReadOnly = readOnly;
			edtResource.Properties.ReadOnly = readOnly;
			edtResources.Properties.ReadOnly = readOnly;
			chkReminder.Properties.ReadOnly = readOnly;
			tbDescription.Properties.ReadOnly = readOnly;
			cbReminder.Properties.ReadOnly = readOnly;
		}
		protected internal virtual void SubscribeAppointmentPageControlsEvents() {
			tbSubject.EditValueChanged += new EventHandler(tbSubject_EditValueChanged);
			tbLocation.EditValueChanged += new EventHandler(tbLocation_EditValueChanged);
			chkAllDay.EditValueChanged += new EventHandler(chkAllDay_EditValueChanged);
			edtStartDate.Validated += new EventHandler(edtStartDate_Validated);
			edtEndDate.Validating += new CancelEventHandler(edtEndDate_Validating);
			edtEndDate.Validated += new EventHandler(edtEndDate_Validated);
			edtEndDate.InvalidValue += new InvalidValueExceptionEventHandler(edtEndDate_InvalidValue);
			edtStartTime.Validated += new EventHandler(edtStartTime_Validated);
			edtEndTime.Validating += new CancelEventHandler(edtEndTime_Validating);
			edtEndTime.Validated += new EventHandler(edtEndTime_Validated);
			edtEndTime.InvalidValue += new InvalidValueExceptionEventHandler(edtEndTime_InvalidValue);
			barEditLabelAs.Edit.EditValueChanged += new EventHandler(barEditLabelAs_EditValueChanged);
			barEditShowTimeAs.Edit.EditValueChanged += new EventHandler(barEditShowTimeAs_EditValueChanged);
			chkReminder.EditValueChanged += new EventHandler(chkReminder_EditValueChanged);
			tbDescription.EditValueChanged += new EventHandler(tbDescription_EditValueChanged);
			cbReminder.InvalidValue += new InvalidValueExceptionEventHandler(cbReminder_InvalidValue);
			cbReminder.Validating += new CancelEventHandler(cbReminder_Validating);
			cbReminder.Validated += new EventHandler(cbReminder_Validated);
			edtResource.EditValueChanged += new EventHandler(edtResource_EditValueChanged);
			edtResources.EditValueChanged += new EventHandler(edtResources_EditValueChanged);
		}
		protected internal virtual void UnsubscribeAppointmentPageControlsEvents() {
			tbSubject.EditValueChanged -= new EventHandler(tbSubject_EditValueChanged);
			tbLocation.EditValueChanged -= new EventHandler(tbLocation_EditValueChanged);
			chkAllDay.EditValueChanged -= new EventHandler(chkAllDay_EditValueChanged);
			edtStartDate.Validated -= new EventHandler(edtStartDate_Validated);
			edtEndDate.Validating -= new CancelEventHandler(edtEndDate_Validating);
			edtEndDate.Validated -= new EventHandler(edtEndDate_Validated);
			edtEndDate.InvalidValue -= new InvalidValueExceptionEventHandler(edtEndDate_InvalidValue);
			edtStartTime.Validated -= new EventHandler(edtStartTime_Validated);
			edtEndTime.Validating -= new CancelEventHandler(edtEndTime_Validating);
			edtEndTime.Validated -= new EventHandler(edtEndTime_Validated);
			edtEndTime.InvalidValue -= new InvalidValueExceptionEventHandler(edtEndTime_InvalidValue);
			barEditLabelAs.Edit.EditValueChanged += new EventHandler(barEditLabelAs_EditValueChanged);
			barEditShowTimeAs.Edit.EditValueChanged -= new EventHandler(barEditShowTimeAs_EditValueChanged);
			chkReminder.EditValueChanged -= new EventHandler(chkReminder_EditValueChanged);
			tbDescription.EditValueChanged -= new EventHandler(tbDescription_EditValueChanged);
			cbReminder.InvalidValue -= new InvalidValueExceptionEventHandler(cbReminder_InvalidValue);
			cbReminder.Validating -= new CancelEventHandler(cbReminder_Validating);
			cbReminder.Validated -= new EventHandler(cbReminder_Validated);
			edtResource.EditValueChanged -= new EventHandler(edtResource_EditValueChanged);
			edtResources.EditValueChanged -= new EventHandler(edtResources_EditValueChanged);
		}
		protected internal virtual void UpdateIntervalControls() {
			UnsubscribeAppointmentPageControlsEvents();
			try {
				UpdateIntervalControlsCore();
			}
			finally {
				SubscribeAppointmentPageControlsEvents();
			}
		}
		protected virtual void UpdateIntervalControlsCore() {
			edtStartDate.EditValue = Controller.DisplayStart.Date;
			edtEndDate.EditValue = Controller.DisplayEnd.Date;
			edtStartTime.EditValue = new DateTime(Controller.DisplayStart.TimeOfDay.Ticks);
			edtEndTime.EditValue = new DateTime(Controller.DisplayEnd.TimeOfDay.Ticks);
			chkAllDay.Checked = Controller.AllDay;
			Appointment editedAptCopy = Controller.EditedAppointmentCopy;
			bool showControls =  editedAptCopy.Type != AppointmentType.Pattern;
			edtStartDate.Enabled = showControls;
			edtEndDate.Enabled = showControls;
			bool enableTime = showControls && !Controller.AllDay;
			LayoutVisibility visibility = enableTime ? LayoutVisibility.Always : LayoutVisibility.Never;
			layoutStartTime.Visibility = visibility;
			edtStartTime.Enabled = enableTime;
			layoutEndTime.Visibility = visibility;
			edtEndTime.Enabled = enableTime;
			chkAllDay.Enabled = showControls;
			UpdateAppointmentInfo();
		}
		protected internal virtual void UpdateReminderCombo() {
			if (Controller.HasReminder)
				cbReminder.Duration = Controller.ReminderTimeBeforeStart;
			else
				cbReminder.Text = String.Empty;
			SchedulerControl control = (SchedulerControl)Controller.InnerControl.Owner;
			bool remindersEnabled = control.RemindersEnabled;
			cbReminder.Enabled = remindersEnabled && Controller.HasReminder;
			cbReminder.Visible = remindersEnabled;
		}
		public virtual void UpdateAppointmentInfo() {
			AppointmentInfoBuilder appointmentInfoBuilder = new AppointmentInfoBuilder();
			string info = appointmentInfoBuilder.GetAppointmentInfo(Controller);
			if (!string.IsNullOrEmpty(info)) {
				layoutInfo.Visibility = LayoutVisibility.Always;
				lblInfo.Text = info;
			}
			else
				layoutInfo.Visibility = LayoutVisibility.OnlyInCustomization;
		}
		protected internal virtual void edtStartDate_Validated(object sender, EventArgs e) {
			Controller.DisplayStart = edtStartDate.DateTime.Date + edtStartTime.Time.TimeOfDay;
			UpdateIntervalControls();
		}
		protected internal virtual void edtStartTime_Validated(object sender, EventArgs e) {
			Controller.DisplayStart = edtStartDate.DateTime.Date + edtStartTime.Time.TimeOfDay;
			UpdateIntervalControls();
		}
		protected internal virtual void edtEndDate_Validating(object sender, CancelEventArgs e) {
			e.Cancel = !IsValidInterval();
		}
		protected internal virtual void edtEndDate_Validated(object sender, System.EventArgs e) {
			Controller.DisplayEnd = edtEndDate.DateTime.Date + edtEndTime.Time.TimeOfDay;
			UpdateAppointmentInfo();
		}
		protected internal virtual void edtEndDate_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		protected internal virtual void edtEndTime_Validated(object sender, EventArgs e) {
			Controller.DisplayEnd = edtEndDate.DateTime.Date + edtEndTime.Time.TimeOfDay;
			UpdateAppointmentInfo();
		}
		protected internal virtual void edtEndTime_Validating(object sender, CancelEventArgs e) {
			e.Cancel = !IsValidInterval();
		}
		protected internal virtual void edtEndTime_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate);
		}
		protected internal virtual bool IsValidInterval() {
			return AppointmentFormControllerBase.ValidateInterval(edtStartDate.DateTime.Date, edtStartTime.Time.TimeOfDay, edtEndDate.DateTime.Date, edtEndTime.Time.TimeOfDay);
		}
		protected internal virtual void tbSubject_EditValueChanged(object sender, EventArgs e) {
			Controller.Subject = tbSubject.Text;
			RaiseNotifyChangeCaption();
		}
		protected internal virtual void tbLocation_EditValueChanged(object sender, EventArgs e) {
			Controller.Location = tbLocation.Text;
		}
		protected internal virtual void tbDescription_EditValueChanged(object sender, EventArgs e) {
			Controller.Description = tbDescription.Text;
		}
		protected internal virtual void edtResource_EditValueChanged(object sender, EventArgs e) {
			Controller.ResourceId = edtResource.ResourceId;
		}
		protected internal virtual void edtResources_EditValueChanged(object sender, EventArgs e) {
			AppointmentResourceIdCollection resourceIds = Controller.ResourceIds;
			resourceIds.BeginUpdate();
			try {
				resourceIds.Clear();
				resourceIds.AddRange(edtResources.ResourceIds);
			}
			finally {
				resourceIds.EndUpdate();
			}
		}
		protected internal virtual void chkAllDay_EditValueChanged(object sender, EventArgs e) {
			Controller.AllDay = chkAllDay.Checked;
			UpdateAppointmentStatus();
			UpdateIntervalControls();
			RaiseNotifyChangeCaption();
		}
		protected internal virtual void UpdateAppointmentStatus() {
			AppointmentStatus currentStatus = (AppointmentStatus)barEditShowTimeAs.EditValue;
			IAppointmentStatus newStatus = Controller.UpdateAppointmentStatus(currentStatus);
			if (newStatus != currentStatus)
				barEditShowTimeAs.EditValue = newStatus;
		}
		protected internal virtual void SetFocus() {
			if (String.IsNullOrEmpty(tbSubject.Text))
				tbSubject.Focus();
			else
				tbDescription.Focus();
		}
		protected internal virtual void chkReminder_EditValueChanged(object sender, EventArgs e) {
			Controller.HasReminder = chkReminder.Checked;
			UpdateReminderCombo();
		}
		protected internal virtual void cbReminder_Validated(object sender, EventArgs e) {
			Controller.ReminderTimeBeforeStart = cbReminder.Duration;
		}
		protected internal virtual void cbReminder_Validating(object sender, CancelEventArgs e) {
			TimeSpan span = cbReminder.Duration;
			e.Cancel = (span == TimeSpan.MinValue) || (span.Ticks < 0);
		}
		protected internal virtual void cbReminder_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidReminderTimeBeforeStart);
		}
		public virtual void RestoreControlLayout(string path) {
			if (!String.IsNullOrEmpty(path)) {
				layoutCtrl.RestoreLayoutFromRegistry(path);
				UpdateAppointmentInfo();
			}
		}
		public virtual void SaveControlLayout(string path) {
			if (!String.IsNullOrEmpty(path) && layoutCtrl.IsModified)
				layoutCtrl.SaveLayoutToRegistry(path);
		}
		#endregion
		protected internal virtual void barEditShowTimeAs_EditValueChanged(object sender, EventArgs e) {
			Controller.SetStatus(((AppointmentStatusEdit)sender).Status);
		}
		protected internal virtual void barEditLabelAs_EditValueChanged(object sender, EventArgs e) {
			Controller.SetLabel(((AppointmentLabelEdit)sender).Label);
		}
	}
}
