Imports System
Imports System.ComponentModel
Imports DevExpress.XtraScheduler.UI
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Ribbon
Imports System.Drawing
Imports DevExpress.XtraScheduler.Native
Imports System.Reflection
Imports DevExpress.Utils.Controls
Imports System.Windows.Forms
Imports System.Globalization
Imports DevExpress.XtraScheduler.Localization
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraLayout.Utils
Imports DevExpress.XtraScheduler.Extensions
Imports DevExpress.Utils
Imports DevExpress.XtraSpellChecker
Imports DevExpress.XtraScheduler

Partial Public Class $safeitemname$
    Inherits RibbonForm
#Region "Fields"
    Private openRecurrenceForm_Renamed As Boolean
    Private readOnly_Renamed As Boolean
    Private storage_Renamed As SchedulerStorage
    Private control_Renamed As SchedulerControl
    Private recurringIcon_Renamed As Icon
    Private normalIcon_Renamed As Icon
    Private closeImage As Image
    Private saveImage As Image
    Private saveAndCloseImage As Image
    Private deleteImage As Image
    Private spellingImage As Image
    Private recurrenceImage As Image
    Private closeLargeImage As Image
    Private saveLargeImage As Image
    Private saveAndCloseLargeImage As Image
    Private deleteLargeImage As Image
    Private spellingLargeImage As Image
    Private recurrenceLargeImage As Image
    Private controller_Renamed As AppointmentFormController
    Private sourceAppointment As DevExpress.XtraScheduler.Appointment
#End Region
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub
    Public Sub New(ByVal control As DevExpress.XtraScheduler.SchedulerControl, ByVal apt As DevExpress.XtraScheduler.Appointment)
        Me.New(control, apt, False)
    End Sub
    Public Sub New(ByVal control As DevExpress.XtraScheduler.SchedulerControl, ByVal apt As DevExpress.XtraScheduler.Appointment, ByVal openRecurrenceForm As Boolean)
        MyBase.New()
        If control Is Nothing Then
            Exceptions.ThrowArgumentException("control", control)
        End If
        If control.Storage Is Nothing Then
            Exceptions.ThrowArgumentException("control.Storage", control.Storage)
        End If
        If apt Is Nothing Then
            Exceptions.ThrowArgumentException("apt", apt)
        End If

        Me.openRecurrenceForm_Renamed = openRecurrenceForm
        Me.controller_Renamed = CreateController(control, apt)
        Me.sourceAppointment = apt
        '
        ' Required for Windows Form Designer support
        '
        InitializeComponent()

        Dim storage As SchedulerStorage = CType(control.Storage, SchedulerStorage)
        repItemAppointmentStatus.Storage = storage
        repItemAppointmentLabel.Storage = storage
        edtResource.SchedulerControl = control
        edtResource.Storage = storage
        edtResources.SchedulerControl = control

        barAndDockingController.LookAndFeel.ParentLookAndFeel = LookAndFeel
        Me.spellChecker.LookAndFeel.ParentLookAndFeel = Me.LookAndFeel
        LoadIcons()
        LoadImages()

        Me.control_Renamed = control
        Me.storage_Renamed = control.Storage
    End Sub

#Region "Properties"
    Protected Friend ReadOnly Property Controller() As AppointmentFormController
        Get
            Return controller_Renamed
        End Get
    End Property
    Protected Friend ReadOnly Property Control() As DevExpress.XtraScheduler.SchedulerControl
        Get
            Return control_Renamed
        End Get
    End Property
    Protected Friend ReadOnly Property Storage() As DevExpress.XtraScheduler.SchedulerStorage
        Get
            Return storage_Renamed
        End Get
    End Property
    Protected Friend ReadOnly Property IsNewAppointment() As Boolean
        Get
            Return If(controller_Renamed IsNot Nothing, controller_Renamed.IsNewAppointment, True)
        End Get
    End Property
    Protected Friend ReadOnly Property RecurringIcon() As Icon
        Get
            Return recurringIcon_Renamed
        End Get
    End Property
    Protected Friend ReadOnly Property NormalIcon() As Icon
        Get
            Return normalIcon_Renamed
        End Get
    End Property

    Protected Friend ReadOnly Property OpenRecurrenceForm() As Boolean
        Get
            Return openRecurrenceForm_Renamed
        End Get
    End Property
    Public Property [ReadOnly]() As Boolean
        Get
            Return readOnly_Renamed
        End Get
        Set(ByVal value As Boolean)
            If readOnly_Renamed = value Then
                Return
            End If
            readOnly_Renamed = value
            UpdateForm()
        End Set
    End Property
    Public ReadOnly Property Speller() As SpellChecker
        Get
            Return spellChecker
        End Get
    End Property
#End Region
#Region "Events"
#Region "NotifyChangeCaption"
    Private Event onNotifyChangeCaption As EventHandler
    Protected Custom Event NotifyChangeCaption As EventHandler
        AddHandler(ByVal value As EventHandler)
            AddHandler onNotifyChangeCaption, value
        End AddHandler
        RemoveHandler(ByVal value As EventHandler)
            RemoveHandler onNotifyChangeCaption, value
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
        End RaiseEvent
    End Event
    Protected Friend Overridable Sub RaiseNotifyChangeCaption()
        RaiseEvent onNotifyChangeCaption(Me, EventArgs.Empty)
    End Sub
#End Region
#End Region

    ''' <summary>
    ''' Add your code to obtain a custom field value and fill the editor with data.
    ''' </summary>
    Private Sub LoadFormData(ByVal appointment As Appointment)
    End Sub
    ''' <summary>
    ''' Add your code to retrieve a value from the editor and set the custom appointment field.
    ''' </summary>
    Private Function SaveFormData(ByVal appointment As Appointment) As Boolean
        Return True
    End Function
    Private Sub PrepareSpellChecker()
        'To check the spelling in a different language,  specify the SpellChecker culture setting and 
        'add the dictionaries required for the selected culture.
        'The form already contains en-US dictionary loaded by default.
        'For more information on supported dictionaries, review the Dictionaries article in XtraSpellChecker Help, 
        'available online at http://documentation.devexpress.com/#WindowsForms/CustomDocument8581
        'The commented lines below can be used to load the OpenOffce dictionary for Engilsh (US). You 
        'can uncomment them and modify file names, paths and dictionary type as required. 
    End Sub
    Protected Friend Overridable Function CreateController(ByVal control As DevExpress.XtraScheduler.SchedulerControl, ByVal apt As DevExpress.XtraScheduler.Appointment) As AppointmentFormController
        Return New AppointmentFormController(control, apt)
    End Function
    Protected Friend Overridable Sub LoadIcons()
        Dim asm As System.Reflection.Assembly = GetType(SchedulerControl).Assembly
        Me.recurringIcon_Renamed = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm)
        Me.normalIcon_Renamed = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm)
    End Sub
    Protected Friend Overridable Sub LoadImages()
        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Me.saveImage = LoadImageFromResource($safeitemname$ImagesName.Save)
            Me.saveLargeImage = LoadImageFromResource($safeitemname$ImagesName.SaveLarge)
            Me.saveAndCloseImage = LoadImageFromResource($safeitemname$ImagesName.SaveAndClose)
            Me.saveAndCloseLargeImage = LoadImageFromResource($safeitemname$ImagesName.SaveAndCloseLarge)
            Me.deleteImage = LoadImageFromResource($safeitemname$ImagesName.Delete)
            Me.deleteLargeImage = LoadImageFromResource($safeitemname$ImagesName.DeleteLarge)
            Me.recurrenceImage = LoadImageFromResource($safeitemname$ImagesName.Recurrence)
            Me.recurrenceLargeImage = LoadImageFromResource($safeitemname$ImagesName.RecurrenceLarge)
            Me.closeImage = LoadImageFromResource($safeitemname$ImagesName.Close)
            Me.closeLargeImage = LoadImageFromResource($safeitemname$ImagesName.CloseLarge)
            Me.spellingImage = LoadImageFromResource($safeitemname$ImagesName.Spelling)
            Me.spellingLargeImage = LoadImageFromResource($safeitemname$ImagesName.SpellingLarge)
    End Sub
    Private Function LoadImageFromResource(ByVal name As String) As Image
			Dim resources As New System.ComponentModel.ComponentResourceManager(GetType($safeitemname$))
        Return (CType(resources.GetObject(name), System.Drawing.Image))
    End Function
    Protected Friend Overridable Sub UpdateForm()
        UnsubscribeControlsEvents()
        Try
            UpdateFormCore()
        Finally
            SubscribeControlsEvents()
        End Try
    End Sub
    Protected Friend Overridable Sub UpdateFormCore()
        UpdateControl()
        UpdateCustomFieldsControls()
        UpdateFormCaption()
        UpdateRibbonPageCaption()
        UpdateDeleteButton()
        UpdateRecurrenceButton()
        UpdateIcon()
    End Sub
    Protected Friend Overridable Sub UpdateCustomFieldsControls()
    End Sub
    Protected Friend Overridable Sub UpdateFormCaption()
        Me.Text = FormatAppointmentFormCaption()
    End Sub
    Protected Friend Overridable Function FormatAppointmentFormCaption() As String
        Dim formCaptionPart As String = FormatAppointmentRibbonPageCaption()
        Dim formCaption As String = Controller.EditedAppointmentCopy.Subject
        If formCaption Is Nothing OrElse formCaption.Length = 0 Then
            formCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_UntitledAppointment)
        End If
        formCaption = String.Format("{0} - {1}", formCaption, formCaptionPart)
        If [ReadOnly] Then
            formCaption &= SchedulerLocalizer.GetString(SchedulerStringId.Caption_ReadOnly)
        End If
        Return formCaption
    End Function
    Protected Friend Overridable Function FormatAppointmentRibbonPageCaption() As String
        Dim recApt As Boolean = If(Controller.EditedPattern Is Nothing, False, True)
        Dim allDayApt As Boolean = Controller.AllDay
        If recApt AndAlso (Not allDayApt) Then
            Return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_RecurringAppointment)
        End If
        If recApt AndAlso allDayApt Then
            Return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_RecurringEvent)
        End If
        If (Not recApt) AndAlso (Not allDayApt) Then
            Return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_Appointment)
        End If
        If (Not recApt) AndAlso allDayApt Then
            Return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_Event)
        End If
        Return String.Empty
    End Function
    Protected Friend Overridable Sub UpdateRibbonPageCaption()
        Me.ribPageAppointment.Text = FormatAppointmentRibbonPageCaption()
    End Sub
    Protected Friend Overridable Sub UpdateDeleteButton()
        btnDelete.Enabled = Controller.CanDeleteAppointment
    End Sub
    Protected Friend Overridable Sub UpdateRecurrenceButton()
        Dim showRecurrenceButton As Boolean = controller_Renamed.ShouldShowRecurrenceButton
        btnRecurrence.Enabled = showRecurrenceButton
    End Sub
    Protected Friend Overridable Sub UpdateIcon()
        If controller_Renamed.EditedAppointmentCopy.Type = AppointmentType.Pattern Then
            Me.Icon = recurringIcon_Renamed
        Else
            Me.Icon = normalIcon_Renamed
        End If
    End Sub
    Protected Friend Overridable Sub SubscribeControlsEvents()
        AddHandler NotifyChangeCaption, AddressOf aptFormCtrl_NotifyChangeCaption
    End Sub
    Protected Friend Overridable Sub UnsubscribeControlsEvents()
        RemoveHandler NotifyChangeCaption, AddressOf aptFormCtrl_NotifyChangeCaption
    End Sub
    Protected Friend Overridable Sub aptFormCtrl_NotifyChangeCaption(ByVal sender As Object, ByVal e As EventArgs)
        UpdateFormCaption()
        UpdateRibbonPageCaption()
    End Sub
    Protected Friend Overridable Sub UpdateImages()
        Me.btnSave.Glyph = saveImage
        Me.btnSave.LargeGlyph = saveLargeImage
        Me.btnSaveAndClose.Glyph = saveAndCloseImage
        Me.btnSaveAndClose.LargeGlyph = saveAndCloseLargeImage
        Me.btnRecurrence.Glyph = recurrenceImage
        Me.btnRecurrence.LargeGlyph = recurrenceLargeImage
        Me.btnDelete.Glyph = deleteImage
        Me.btnDelete.LargeGlyph = deleteLargeImage
        Me.btnSpelling.Glyph = spellingImage
        Me.btnSpelling.LargeGlyph = spellingLargeImage
        Me.btnClose.Glyph = closeImage
        Me.btnClose.LargeGlyph = closeLargeImage
    End Sub
    Protected Friend Sub btnDelete_Click(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles btnDelete.ItemClick
        OnDeleteButton()
    End Sub
    Protected Friend Overridable Sub OnDeleteButton()
        If IsNewAppointment Then
            Return
        End If
        Controller.DeleteAppointment()

        DialogResult = System.Windows.Forms.DialogResult.Abort
        Close()
    End Sub
    Protected Friend Sub btnSave_Click(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles btnSave.ItemClick
        OnSaveButton()
    End Sub
    Protected Friend Sub btnSaveAndClose_Click(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles btnSaveAndClose.ItemClick
        OnSaveButton()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub
    Protected Friend Overridable Sub OnSaveButton()
        DoValidation()
        If (Not controller_Renamed.IsConflictResolved()) Then
            ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If
        If (Not Me.SaveFormData(Me.sourceAppointment)) Then
            Return
        End If
        controller_Renamed.ApplyChanges()
        UpdateDeleteButton()
    End Sub
    Protected Friend Sub btnClose_Click(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles btnClose.ItemClick
        DialogResult = System.Windows.Forms.DialogResult.Abort
    End Sub
    Protected Friend Sub btnSpelling_Click(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles btnSpelling.ItemClick
        spellChecker.CheckContainer()
    End Sub
    Protected Sub btnRecurrence_Click(ByVal sender As Object, ByVal e As ItemClickEventArgs) Handles btnRecurrence.ItemClick
        OnRecurrenceButton()
    End Sub
    Protected Friend Overridable Sub OnRecurrenceButton()
        If (Not Controller.ShouldShowRecurrenceButton) Then
            Return
        End If

        DoValidation()
        Dim patternCopy As Appointment = controller_Renamed.PrepareToRecurrenceEdit()

        Dim result As DialogResult
        Using form As Form = CreateAppointmentRecurrenceForm(patternCopy, control_Renamed.OptionsView.FirstDayOfWeek)
            result = ShowRecurrenceForm(form)
        End Using

        If result = System.Windows.Forms.DialogResult.Abort Then
            Controller.RemoveRecurrence()
            UpdateIntervalControls()
            UpdateIcon()
            UpdateFormCaption()
            UpdateRibbonPageCaption()
        ElseIf result = System.Windows.Forms.DialogResult.OK Then
            Controller.ApplyRecurrence(patternCopy)
            UpdateForm()
        End If
    End Sub
    Protected Friend Overridable Function CreateAppointmentRecurrenceForm(ByVal patternCopy As DevExpress.XtraScheduler.Appointment, ByVal firstDayOfWeek As DevExpress.XtraScheduler.FirstDayOfWeek) As Form
        Dim form As New AppointmentRecurrenceForm(patternCopy, firstDayOfWeek, Controller)
        form.LookAndFeel.ParentLookAndFeel = LookAndFeel
        form.ShowExceptionsRemoveMsgBox = controller_Renamed.AreExceptionsPresent()
        Return form
    End Function
    Protected Overridable Function ShowRecurrenceForm(ByVal form As Form) As DialogResult
        Return form.ShowDialog(Me)
    End Function
    Protected Friend Sub AppointmentFormBase_Activated(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Activated
        If openRecurrenceForm_Renamed Then
            openRecurrenceForm_Renamed = False
            OnRecurrenceButton()
        End If
    End Sub
    Protected Friend Sub OnFormLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			If (CType(sender, $safeitemname$)).Controller Is Nothing Then
            Return
        End If
        PrepareSpellChecker()
        UpdateImages()
        UpdateForm()
        LoadFormData(Me.sourceAppointment)
        RestoreControlLayout(GetLayoutRegistryPath())
        SetFocus()
    End Sub
    Protected Friend Sub AppointmentFormBase_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        SaveControlLayout(GetLayoutRegistryPath())
    End Sub
    Protected Friend Overridable Function GetLayoutRegistryPath() As String
        Return String.Format("{0}\{1}_Layout", Application.ProductName + Application.ProductVersion, Me.GetType().Name)
    End Function
    Protected Friend Overridable Function ShowMessageBox(ByVal text As String, ByVal caption As String, ByVal buttons As MessageBoxButtons, ByVal icon As MessageBoxIcon) As DialogResult
        Return XtraMessageBox.Show(Me, text, caption, buttons, icon)
    End Function
    Protected Overridable Sub DoValidation()
        Validate()
    End Sub

#Region "AppointmentPageControls"
    Public Overridable Sub UpdateControl()
        UnsubscribeAppointmentPageControlsEvents()
        Try
            UpdateControlCore()
        Finally
            SubscribeAppointmentPageControlsEvents()
        End Try
    End Sub
    Protected Overridable Sub UpdateControlCore()
        MakeControlsReadOnly(False)

        tbSubject.Text = Controller.Subject
        tbLocation.Text = Controller.Location
        tbDescription.Text = Controller.Description
        barEditShowTimeAs.EditValue = Controller.GetStatus()

        edtResource.ResourceId = Controller.ResourceId

        Dim resourceIds As AppointmentResourceIdCollection = edtResources.ResourceIds
        resourceIds.BeginUpdate()
        Try
            resourceIds.Clear()
            resourceIds.AddRange(Controller.ResourceIds)
        Finally
            resourceIds.EndUpdate()
        End Try

        barEditLabelAs.EditValue = Controller.GetLabel()
        Dim remindersEnabled As Boolean = Control.RemindersEnabled
        chkReminder.Enabled = remindersEnabled
        chkReminder.Visible = remindersEnabled
        chkReminder.Checked = Controller.HasReminder
        UpdateReminderCombo()
        UpdateIntervalControlsCore()
        UpdateCustomFieldsControls()
        RaiseNotifyChangeCaption()

        Dim resourceSharing As Boolean = Controller.ResourceSharing
        edtResource.Visible = Not resourceSharing
        edtResources.Visible = resourceSharing
        If resourceSharing Then
            layoutResources.Visibility = LayoutVisibility.Always
            layoutResource.Visibility = LayoutVisibility.OnlyInCustomization
        Else
            layoutResources.Visibility = LayoutVisibility.OnlyInCustomization
            layoutResource.Visibility = LayoutVisibility.Always
        End If
        Dim canEditResource As Boolean = Controller.CanEditResource
        edtResource.Enabled = canEditResource
        edtResources.Enabled = canEditResource
        If [ReadOnly] Then
            MakeControlsReadOnly([ReadOnly])
        End If
    End Sub

    Protected Friend Overridable Sub MakeControlsReadOnly(ByVal [readOnly] As Boolean)
        tbLocation.Properties.ReadOnly = [readOnly]
        chkAllDay.Properties.ReadOnly = [readOnly]
        edtStartDate.Properties.ReadOnly = [readOnly]
        edtEndDate.Properties.ReadOnly = [readOnly]
        edtStartTime.Properties.ReadOnly = [readOnly]
        edtEndTime.Properties.ReadOnly = [readOnly]
        barEditLabelAs.Edit.ReadOnly = [readOnly]
        barEditShowTimeAs.Edit.ReadOnly = [readOnly]
        tbSubject.Properties.ReadOnly = [readOnly]
        edtResource.Properties.ReadOnly = [readOnly]
        edtResources.Properties.ReadOnly = [readOnly]
        chkReminder.Properties.ReadOnly = [readOnly]
        tbDescription.Properties.ReadOnly = [readOnly]
        cbReminder.Properties.ReadOnly = [readOnly]
    End Sub

    Protected Friend Overridable Sub SubscribeAppointmentPageControlsEvents()
        AddHandler tbSubject.EditValueChanged, AddressOf tbSubject_EditValueChanged
        AddHandler tbLocation.EditValueChanged, AddressOf tbLocation_EditValueChanged
        AddHandler chkAllDay.EditValueChanged, AddressOf chkAllDay_EditValueChanged
        AddHandler edtStartDate.Validated, AddressOf edtStartDate_Validated
        AddHandler edtEndDate.Validating, AddressOf edtEndDate_Validating
        AddHandler edtEndDate.Validated, AddressOf edtEndDate_Validated
        AddHandler edtEndDate.InvalidValue, AddressOf edtEndDate_InvalidValue
        AddHandler edtStartTime.Validated, AddressOf edtStartTime_Validated
        AddHandler edtEndTime.Validating, AddressOf edtEndTime_Validating
        AddHandler edtEndTime.Validated, AddressOf edtEndTime_Validated
        AddHandler edtEndTime.InvalidValue, AddressOf edtEndTime_InvalidValue
        AddHandler barEditLabelAs.Edit.EditValueChanged, AddressOf barEditLabelAs_EditValueChanged
        AddHandler barEditShowTimeAs.Edit.EditValueChanged, AddressOf barEditShowTimeAs_EditValueChanged
        AddHandler chkReminder.EditValueChanged, AddressOf chkReminder_EditValueChanged
        AddHandler tbDescription.EditValueChanged, AddressOf tbDescription_EditValueChanged
        AddHandler cbReminder.InvalidValue, AddressOf cbReminder_InvalidValue
        AddHandler cbReminder.Validating, AddressOf cbReminder_Validating
        AddHandler cbReminder.Validated, AddressOf cbReminder_Validated
        AddHandler edtResource.EditValueChanged, AddressOf edtResource_EditValueChanged
        AddHandler edtResources.EditValueChanged, AddressOf edtResources_EditValueChanged
    End Sub
    Protected Friend Overridable Sub UnsubscribeAppointmentPageControlsEvents()
        RemoveHandler tbSubject.EditValueChanged, AddressOf tbSubject_EditValueChanged
        RemoveHandler tbLocation.EditValueChanged, AddressOf tbLocation_EditValueChanged
        RemoveHandler chkAllDay.EditValueChanged, AddressOf chkAllDay_EditValueChanged
        RemoveHandler edtStartDate.Validated, AddressOf edtStartDate_Validated
        RemoveHandler edtEndDate.Validating, AddressOf edtEndDate_Validating
        RemoveHandler edtEndDate.Validated, AddressOf edtEndDate_Validated
        RemoveHandler edtEndDate.InvalidValue, AddressOf edtEndDate_InvalidValue
        RemoveHandler edtStartTime.Validated, AddressOf edtStartTime_Validated
        RemoveHandler edtEndTime.Validating, AddressOf edtEndTime_Validating
        RemoveHandler edtEndTime.Validated, AddressOf edtEndTime_Validated
        RemoveHandler edtEndTime.InvalidValue, AddressOf edtEndTime_InvalidValue
        AddHandler barEditLabelAs.Edit.EditValueChanged, AddressOf barEditLabelAs_EditValueChanged
        RemoveHandler barEditShowTimeAs.Edit.EditValueChanged, AddressOf barEditShowTimeAs_EditValueChanged
        RemoveHandler chkReminder.EditValueChanged, AddressOf chkReminder_EditValueChanged
        RemoveHandler tbDescription.EditValueChanged, AddressOf tbDescription_EditValueChanged
        RemoveHandler cbReminder.InvalidValue, AddressOf cbReminder_InvalidValue
        RemoveHandler cbReminder.Validating, AddressOf cbReminder_Validating
        RemoveHandler cbReminder.Validated, AddressOf cbReminder_Validated
        RemoveHandler edtResource.EditValueChanged, AddressOf edtResource_EditValueChanged
        RemoveHandler edtResources.EditValueChanged, AddressOf edtResources_EditValueChanged
    End Sub
    Protected Friend Overridable Sub UpdateIntervalControls()
        UnsubscribeAppointmentPageControlsEvents()
        Try
            UpdateIntervalControlsCore()
        Finally
            SubscribeAppointmentPageControlsEvents()
        End Try
    End Sub
    Protected Overridable Sub UpdateIntervalControlsCore()
        edtStartDate.EditValue = Controller.DisplayStart.Date
        edtEndDate.EditValue = Controller.DisplayEnd.Date
        edtStartTime.EditValue = New DateTime(Controller.DisplayStart.TimeOfDay.Ticks)
        edtEndTime.EditValue = New DateTime(Controller.DisplayEnd.TimeOfDay.Ticks)
        chkAllDay.Checked = Controller.AllDay

        Dim editedAptCopy As Appointment = Controller.EditedAppointmentCopy
        'INSTANT VB NOTE: Embedded comments are not maintained by Instant VB
        'ORIGINAL LINE: bool showControls = /*Controller.IsNewAppointment ||*/ editedAptCopy.Type != AppointmentType.Pattern;
        Dim showControls As Boolean = editedAptCopy.Type <> AppointmentType.Pattern
        edtStartDate.Enabled = showControls
        edtEndDate.Enabled = showControls
        Dim enableTime As Boolean = showControls AndAlso Not Controller.AllDay
        Dim visibility As LayoutVisibility = If(enableTime, LayoutVisibility.Always, LayoutVisibility.Never)
        layoutStartTime.Visibility = visibility
        edtStartTime.Enabled = enableTime
        layoutEndTime.Visibility = visibility
        edtEndTime.Enabled = enableTime
        chkAllDay.Enabled = showControls
        UpdateAppointmentInfo()
    End Sub
    Protected Friend Overridable Sub UpdateReminderCombo()
        If Controller.HasReminder Then
            cbReminder.Duration = Controller.ReminderTimeBeforeStart
        Else
            cbReminder.Text = String.Empty
        End If
        Dim remindersEnabled As Boolean = Control.RemindersEnabled
        cbReminder.Enabled = remindersEnabled AndAlso Controller.HasReminder
        cbReminder.Visible = remindersEnabled
    End Sub

    Public Overridable Sub UpdateAppointmentInfo()
        Dim appointmentInfoBuilder As New AppointmentInfoBuilder()
        Dim info As String = appointmentInfoBuilder.GetAppointmentInfo(Controller)
        If (Not String.IsNullOrEmpty(info)) Then
            layoutInfo.Visibility = LayoutVisibility.Always
            lblInfo.Text = info
        Else
            layoutInfo.Visibility = LayoutVisibility.OnlyInCustomization
        End If
    End Sub
    Protected Friend Overridable Sub edtStartDate_Validated(ByVal sender As Object, ByVal e As EventArgs)
        Controller.DisplayStart = edtStartDate.DateTime.Date + edtStartTime.Time.TimeOfDay
        UpdateIntervalControls()
    End Sub
    Protected Friend Overridable Sub edtStartTime_Validated(ByVal sender As Object, ByVal e As EventArgs)
        Controller.DisplayStart = edtStartDate.DateTime.Date + edtStartTime.Time.TimeOfDay
        UpdateIntervalControls()
    End Sub
    Protected Friend Overridable Sub edtEndDate_Validating(ByVal sender As Object, ByVal e As CancelEventArgs)
        e.Cancel = Not IsValidInterval()
    End Sub
    Protected Friend Overridable Sub edtEndDate_Validated(ByVal sender As Object, ByVal e As System.EventArgs)
        Controller.DisplayEnd = edtEndDate.DateTime.Date + edtEndTime.Time.TimeOfDay
        UpdateAppointmentInfo()
    End Sub
    Protected Friend Overridable Sub edtEndDate_InvalidValue(ByVal sender As Object, ByVal e As InvalidValueExceptionEventArgs)
        e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate)
    End Sub
    Protected Friend Overridable Sub edtEndTime_Validated(ByVal sender As Object, ByVal e As EventArgs)
        Controller.DisplayEnd = edtEndDate.DateTime.Date + edtEndTime.Time.TimeOfDay
        UpdateAppointmentInfo()
    End Sub
    Protected Friend Overridable Sub edtEndTime_Validating(ByVal sender As Object, ByVal e As CancelEventArgs)
        e.Cancel = Not IsValidInterval()
    End Sub
    Protected Friend Overridable Sub edtEndTime_InvalidValue(ByVal sender As Object, ByVal e As InvalidValueExceptionEventArgs)
        e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate)
    End Sub
    Protected Friend Overridable Function IsValidInterval() As Boolean
        Return AppointmentFormControllerBase.ValidateInterval(edtStartDate.DateTime.Date, edtStartTime.Time.TimeOfDay, edtEndDate.DateTime.Date, edtEndTime.Time.TimeOfDay)
    End Function

    Protected Friend Overridable Sub tbSubject_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.Subject = tbSubject.Text
        RaiseNotifyChangeCaption()
    End Sub
    Protected Friend Overridable Sub tbLocation_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.Location = tbLocation.Text
    End Sub
    Protected Friend Overridable Sub tbDescription_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.Description = tbDescription.Text
    End Sub
    Protected Friend Overridable Sub edtResource_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.ResourceId = edtResource.ResourceId
    End Sub
    Protected Friend Overridable Sub edtResources_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim resourceIds As AppointmentResourceIdCollection = Controller.ResourceIds
        resourceIds.BeginUpdate()
        Try
            resourceIds.Clear()
            resourceIds.AddRange(edtResources.ResourceIds)
        Finally
            resourceIds.EndUpdate()
        End Try
    End Sub
    Protected Friend Overridable Sub chkAllDay_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.AllDay = chkAllDay.Checked
        UpdateAppointmentStatus()
        UpdateIntervalControls()
        RaiseNotifyChangeCaption()
    End Sub
    Protected Friend Overridable Sub UpdateAppointmentStatus()
        Dim currentStatus As AppointmentStatus = CType(barEditShowTimeAs.EditValue, AppointmentStatus)
        Dim newStatus As AppointmentStatus = Controller.UpdateAppointmentStatus(currentStatus)
        If newStatus IsNot currentStatus Then
            barEditShowTimeAs.EditValue = newStatus
        End If
    End Sub
    Protected Friend Overridable Sub SetFocus()
        If String.IsNullOrEmpty(tbSubject.Text) Then
            tbSubject.Focus()
        Else
            tbDescription.Focus()
        End If
    End Sub
    Protected Friend Overridable Sub chkReminder_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.HasReminder = chkReminder.Checked
        UpdateReminderCombo()
    End Sub
    Protected Friend Overridable Sub cbReminder_Validated(ByVal sender As Object, ByVal e As EventArgs)
        Controller.ReminderTimeBeforeStart = cbReminder.Duration
    End Sub
    Protected Friend Overridable Sub cbReminder_Validating(ByVal sender As Object, ByVal e As CancelEventArgs)
        Dim span As TimeSpan = cbReminder.Duration
        e.Cancel = (span = TimeSpan.MinValue) OrElse (span.Ticks < 0)
    End Sub
    Protected Friend Overridable Sub cbReminder_InvalidValue(ByVal sender As Object, ByVal e As InvalidValueExceptionEventArgs)
        e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidReminderTimeBeforeStart)
    End Sub
    Public Overridable Sub RestoreControlLayout(ByVal path As String)
        If (Not String.IsNullOrEmpty(path)) Then
            layoutCtrl.RestoreLayoutFromRegistry(path)
            UpdateAppointmentInfo()
        End If
    End Sub
    Public Overridable Sub SaveControlLayout(ByVal path As String)
        If (Not String.IsNullOrEmpty(path)) AndAlso layoutCtrl.IsModified Then
            layoutCtrl.SaveLayoutToRegistry(path)
        End If
    End Sub
#End Region

    Protected Friend Overridable Sub barEditShowTimeAs_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.SetStatus((CType(sender, AppointmentStatusEdit)).Status)
    End Sub

    Protected Friend Overridable Sub barEditLabelAs_EditValueChanged(ByVal sender As Object, ByVal e As EventArgs)
        Controller.SetLabel((CType(sender, AppointmentLabelEdit)).Label)
    End Sub
End Class

    Public NotInheritable Class $safeitemname$ImagesName
    Public Const Close As String = "Close_16x16"
    Public Const CloseLarge As String = "Close_32x32"
    Public Const Delete As String = "Delete_16x16"
    Public Const DeleteLarge As String = "Delete_32x32"
    Public Const Recurrence As String = "Recurrence_16x16"
    Public Const RecurrenceLarge As String = "Recurrence_32x32"
    Public Const Save As String = "Save_16x16"
    Public Const SaveAndClose As String = "SaveAndClose_16x16"
    Public Const SaveAndCloseLarge As String = "SaveAndClose_32x32"
    Public Const SaveLarge As String = "Save_32x32"
    Public Const Spelling As String = "SpellCheck_16x16"
    Public Const SpellingLarge As String = "SpellCheck_32x32"
End Class
