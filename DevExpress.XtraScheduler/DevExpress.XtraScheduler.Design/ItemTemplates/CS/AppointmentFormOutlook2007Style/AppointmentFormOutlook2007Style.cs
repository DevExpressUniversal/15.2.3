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
using DevExpress.XtraScheduler;

namespace $rootnamespace$ {
    public partial class $safeitemname$ : RibbonForm {
        #region Fields
        bool openRecurrenceForm;
        bool readOnly;
        SchedulerStorage storage;
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
        DevExpress.XtraScheduler.Appointment sourceAppointment;
        #endregion
        [EditorBrowsable(EditorBrowsableState.Never)]
        public $safeitemname$()
            : base() {
            InitializeComponent();
        }
        public $safeitemname$(DevExpress.XtraScheduler.SchedulerControl control, DevExpress.XtraScheduler.Appointment apt)
            : this(control, apt, false) {
        }
        public $safeitemname$(DevExpress.XtraScheduler.SchedulerControl control, DevExpress.XtraScheduler.Appointment apt, bool openRecurrenceForm)
            : base() {
            if (control == null)
                Exceptions.ThrowArgumentException("control", control);
            if (control.Storage == null)
                Exceptions.ThrowArgumentException("control.Storage", control.Storage);
            if (apt == null)
                Exceptions.ThrowArgumentException("apt", apt);

            this.openRecurrenceForm = openRecurrenceForm;
            this.controller = CreateController(control, apt);
            this.sourceAppointment = apt;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            SchedulerStorage storage = (SchedulerStorage)control.Storage;
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
        protected internal DevExpress.XtraScheduler.SchedulerControl Control { get { return control; } }
        protected internal DevExpress.XtraScheduler.SchedulerStorage Storage { get { return storage; } }
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

        /// <summary>
        /// Add your code to obtain a custom field value and fill the editor with data.
        /// </summary>
        void LoadFormData(Appointment appointment) {
        }
        /// <summary>
        /// Add your code to retrieve a value from the editor and set the custom appointment field.
        /// </summary>
        bool SaveFormData(Appointment appointment) {
            return true;
        }
        void PrepareSpellChecker() {
            //To check the spelling in a different language,  specify the SpellChecker culture setting and 
            //add the dictionaries required for the selected culture.
            //The form already contains en-US dictionary loaded by default.
            //For more information on supported dictionaries, review the Dictionaries article in XtraSpellChecker Help, 
            //available online at http://documentation.devexpress.com/#WindowsForms/CustomDocument8581
            //The commented lines below can be used to load the OpenOffce dictionary for Engilsh (US). You 
            //can uncomment them and modify file names, paths and dictionary type as required. 
        }
        protected internal virtual AppointmentFormController CreateController(DevExpress.XtraScheduler.SchedulerControl control, DevExpress.XtraScheduler.Appointment apt) {
            return new AppointmentFormController(control, apt);
        }
        protected internal virtual void LoadIcons() {
            Assembly asm = typeof(SchedulerControl).Assembly;
            this.recurringIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm);
            this.normalIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm);
        }
        protected internal virtual void LoadImages() {
            Assembly asm = Assembly.GetExecutingAssembly();
            this.saveImage = LoadImageFromResource($safeitemname$ImagesName.Save);
            this.saveLargeImage = LoadImageFromResource($safeitemname$ImagesName.SaveLarge);
            this.saveAndCloseImage = LoadImageFromResource($safeitemname$ImagesName.SaveAndClose);
            this.saveAndCloseLargeImage = LoadImageFromResource($safeitemname$ImagesName.SaveAndCloseLarge);
            this.deleteImage = LoadImageFromResource($safeitemname$ImagesName.Delete);
            this.deleteLargeImage = LoadImageFromResource($safeitemname$ImagesName.DeleteLarge);
            this.recurrenceImage = LoadImageFromResource($safeitemname$ImagesName.Recurrence);
            this.recurrenceLargeImage = LoadImageFromResource($safeitemname$ImagesName.RecurrenceLarge);
            this.closeImage = LoadImageFromResource($safeitemname$ImagesName.Close);
            this.closeLargeImage = LoadImageFromResource($safeitemname$ImagesName.CloseLarge);
            this.spellingImage = LoadImageFromResource($safeitemname$ImagesName.Spelling);
            this.spellingLargeImage = LoadImageFromResource($safeitemname$ImagesName.SpellingLarge);
        }
        Image LoadImageFromResource(string name) {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof($safeitemname$));
            return ((System.Drawing.Image)(resources.GetObject(name)));
        }
        protected internal virtual void UpdateForm() {
            UnsubscribeControlsEvents();
            try {
                UpdateFormCore();
            } finally {
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
            if (!this.SaveFormData(this.sourceAppointment))
                return;
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
            } else if (result == DialogResult.OK) {
                Controller.ApplyRecurrence(patternCopy);
                UpdateForm();
            }
        }
        protected internal virtual Form CreateAppointmentRecurrenceForm(DevExpress.XtraScheduler.Appointment patternCopy, FirstDayOfWeek firstDayOfWeek) {
            AppointmentRecurrenceForm form = new AppointmentRecurrenceForm(patternCopy, firstDayOfWeek, Controller);
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
        protected internal void OnFormLoad(object sender, EventArgs e) {
            if ((($safeitemname$)sender).Controller == null)
                return;
            PrepareSpellChecker();
            UpdateImages();
            UpdateForm();
            LoadFormData(this.sourceAppointment);
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
            } finally {
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
            } finally {
                resourceIds.EndUpdate();
            }

            barEditLabelAs.EditValue = Controller.GetLabel();
            bool remindersEnabled = Control.RemindersEnabled;
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
            } else {
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
            } finally {
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
            bool showControls = /*Controller.IsNewAppointment ||*/ editedAptCopy.Type != AppointmentType.Pattern;
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
            bool remindersEnabled = Control.RemindersEnabled;
            cbReminder.Enabled = remindersEnabled && Controller.HasReminder;
            cbReminder.Visible = remindersEnabled;
        }

        public virtual void UpdateAppointmentInfo() {
            AppointmentInfoBuilder appointmentInfoBuilder = new AppointmentInfoBuilder();
            string info = appointmentInfoBuilder.GetAppointmentInfo(Controller);
            if (!string.IsNullOrEmpty(info)) {
                layoutInfo.Visibility = LayoutVisibility.Always;
                lblInfo.Text = info;
            } else
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
            } finally {
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
            AppointmentStatus newStatus = Controller.UpdateAppointmentStatus(currentStatus);
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

    public static class $safeitemname$ImagesName {
        public const string Close = "Close_16x16";
        public const string CloseLarge = "Close_32x32";
        public const string Delete = "Delete_16x16";
        public const string DeleteLarge = "Delete_32x32";
        public const string Recurrence = "Recurrence_16x16";
        public const string RecurrenceLarge = "Recurrence_32x32";
        public const string Save = "Save_16x16";
        public const string SaveAndClose = "SaveAndClose_16x16";
        public const string SaveAndCloseLarge = "SaveAndClose_32x32";
        public const string SaveLarge = "Save_32x32";
        public const string Spelling = "SpellCheck_16x16";
        public const string SpellingLarge = "SpellCheck_32x32";
    }
}