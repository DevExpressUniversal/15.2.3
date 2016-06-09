Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports DevExpress.Utils
Imports DevExpress.Utils.Internal
Imports DevExpress.Utils.Menu
Imports DevExpress.XtraBars
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Native
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.iCalendar
Imports DevExpress.XtraScheduler.Localization
Imports DevExpress.XtraScheduler.Native
Imports DevExpress.XtraScheduler.Printing
Imports DevExpress.XtraScheduler.Printing.Native
Imports DevExpress.XtraScheduler.UI

Partial Public Class $safeitemname$
    Inherits DevExpress.XtraBars.Ribbon.RibbonForm
    Implements IDXManagerPopupMenu
#Region "Fields"
    Private m_openRecurrenceForm As Boolean
    ReadOnly m_storage As ISchedulerStorageBase
    ReadOnly m_control As SchedulerControl
    Private m_recurringIcon As Icon
    Private m_normalIcon As Icon
    ReadOnly m_controller As AppointmentFormController
    Private m_menuManager As IDXMenuManager
    Private m_supressCancelCore As Boolean
#End Region

    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub New(control As DevExpress.XtraScheduler.SchedulerControl, apt As Appointment)
        Me.New(control, apt, False)
    End Sub
    Public Sub New(control As DevExpress.XtraScheduler.SchedulerControl, apt As Appointment, openRecurrenceForm As Boolean)
        Guard.ArgumentNotNull(control, "control")
        Guard.ArgumentNotNull(control.Storage, "control.Storage")
        Guard.ArgumentNotNull(apt, "apt")

        Me.m_openRecurrenceForm = openRecurrenceForm
        Me.m_controller = CreateController(control, apt)
        '
        ' Required for Windows Form Designer support
        '
        InitializeComponent()
        SetupPredefinedConstraints()

        LoadIcons()

        Me.m_control = control
        Me.m_storage = control.Storage

        Me.edtResource.SchedulerControl = control
        Me.edtResource.Storage = m_storage
        Me.edtResources.SchedulerControl = control

        Me.riAppointmentResource.SchedulerControl = control
        Me.riAppointmentResource.Storage = m_storage
        Me.riAppointmentStatus.Storage = m_storage

        Me.riAppointmentLabel.Storage = m_storage

        BindControllerToControls()

        Me.m_supressCancelCore = False
    End Sub
#Region "Properties"
    Public Property MenuManager() As IDXMenuManager
        Get
            Return m_menuManager
        End Get
        Private Set(value As IDXMenuManager)
            m_menuManager = value
        End Set
    End Property
    Protected Friend ReadOnly Property Controller() As AppointmentFormController
        Get
            Return m_controller
        End Get
    End Property
    Protected Friend ReadOnly Property Control() As SchedulerControl
        Get
            Return m_control
        End Get
    End Property
    Protected Friend ReadOnly Property Storage() As ISchedulerStorageBase
        Get
            Return m_storage
        End Get
    End Property
    Protected Friend ReadOnly Property IsNewAppointment() As Boolean
        Get
            Return If(m_controller IsNot Nothing, m_controller.IsNewAppointment, True)
        End Get
    End Property
    Protected Friend ReadOnly Property RecurringIcon() As Icon
        Get
            Return m_recurringIcon
        End Get
    End Property
    Protected Friend ReadOnly Property NormalIcon() As Icon
        Get
            Return m_normalIcon
        End Get
    End Property
    Protected Friend ReadOnly Property OpenRecurrenceForm() As Boolean
        Get
            Return m_openRecurrenceForm
        End Get
    End Property
    Public Property [ReadOnly]() As Boolean
        Get
            Return Controller.[ReadOnly]
        End Get
        Set(value As Boolean)
            If Controller.[ReadOnly] = value Then
                Return
            End If
            Controller.[ReadOnly] = value
        End Set
    End Property
    Protected Overrides ReadOnly Property ShowMode() As FormShowMode
        Get
            Return DevExpress.XtraEditors.FormShowMode.AfterInitialization
        End Get
    End Property
#End Region

    Public Overridable Sub LoadFormData(appointment As Appointment)
        'do nothing
    End Sub
    Public Overridable Function SaveFormData(appointment As Appointment) As Boolean
        Return True
    End Function
    Public Overridable Function IsAppointmentChanged(appointment As Appointment) As Boolean
        Return False
    End Function
    Public Overridable Sub SetMenuManager(menuManager As DevExpress.Utils.Menu.IDXMenuManager)
        MenuManagerUtils.SetMenuManager(Controls, menuManager)
        Me.m_menuManager = menuManager
    End Sub

    Protected Friend Overridable Sub SetupPredefinedConstraints()
        Me.tbProgress.Properties.Minimum = AppointmentProcessValues.Min
        Me.tbProgress.Properties.Maximum = AppointmentProcessValues.Max
        Me.tbProgress.Properties.SmallChange = AppointmentProcessValues.[Step]
        Me.edtResources.Visible = True
    End Sub
    Protected Overridable Sub BindControllerToControls()
        Me.DataBindings.Add("Text", Controller, "Caption")
        BindControllerToIcon()
        BindProperties(Me.tbSubject, "Text", "Subject")
        BindProperties(Me.tbLocation, "Text", "Location")
        BindProperties(Me.tbDescription, "Text", "Description")
        BindProperties(Me.edtStartDate, "EditValue", "DisplayStartDate")
        BindProperties(Me.edtStartDate, "Enabled", "IsDateTimeEditable")
        BindProperties(Me.edtStartTime, "EditValue", "DisplayStartTime")
        BindProperties(Me.edtStartTime, "Enabled", "IsTimeEnabled")
        BindProperties(Me.edtEndDate, "EditValue", "DisplayEndDate", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndDate, "Enabled", "IsDateTimeEditable", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndTime, "EditValue", "DisplayEndTime", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndTime, "Enabled", "IsTimeEnabled", DataSourceUpdateMode.Never)
        BindProperties(Me.chkAllDay, "Checked", "AllDay")
        BindProperties(Me.chkAllDay, "Enabled", "IsDateTimeEditable")

        BindProperties(Me.lblResource, "Enabled", "CanEditResource")

        BindProperties(Me.edtResources, "ResourceIds", "ResourceIds")
        BindProperties(Me.edtResources, "Visible", "ResourceSharing")
        BindProperties(Me.edtResources, "Enabled", "CanEditResource")

        BindProperties(Me.edtResource, "ResourceId", "ResourceId")
        BindProperties(Me.edtResource, "Enabled", "CanEditResource")
        BindToBoolPropertyAndInvert(Me.edtResource, "Visible", "ResourceSharing")

        BindProperties(Me.barLabel, "EditValue", "Label")

        BindProperties(Me.barStatus, "EditValue", "Status")

        BindBoolToVisibility(Me.barReminder, "Visibility", "ReminderVisible")
        BindProperties(Me.barReminder, "Editvalue", "ReminderTimeBeforeStart")

        BindProperties(Me.tbProgress, "Value", "PercentComplete")
        BindProperties(Me.lblPercentCompleteValue, "Text", "PercentComplete", AddressOf ObjectToStringConverter)
        BindProperties(Me.progressPanel, "Visible", "ShouldEditTaskProgress")
        BindProperties(Me.btnDelete, "Enabled", "CanDeleteAppointment")

        BindBoolToVisibility(Me.btnRecurrence, "Visibility", "ShouldShowRecurrenceButton")

        BindToBoolPropertyAndInvert(Me.ribbonControl1, "Enabled", "ReadOnly")

        BindProperties(Me.edtTimeZone, "Visible", "TimeZoneVisible")
        BindProperties(Me.edtTimeZone, "EditValue", "TimeZoneId")
        BindProperties(Me.edtTimeZone, "Enabled", "TimeZoneEnabled")

        BindBoolToVisibility(Me.btnTimeZones, "Visibility", "TimeZonesEnabled")
        BindProperties(Me.btnTimeZones, "Down", "TimeZoneVisible")
    End Sub

    Protected Overridable Sub BindControllerToIcon()
        Dim binding As New Binding("Icon", Controller, "AppointmentType")
        AddHandler binding.Format, AddressOf AppointmentTypeToIconConverter
        DataBindings.Add(binding)
    End Sub
    Protected Overridable Sub ObjectToStringConverter(o As Object, e As ConvertEventArgs)
        e.Value = e.Value.ToString()
    End Sub
    Protected Overridable Sub AppointmentTypeToIconConverter(o As Object, e As ConvertEventArgs)
        Dim type As AppointmentType = DirectCast(e.Value, AppointmentType)
        If type = AppointmentType.Pattern Then
            e.Value = RecurringIcon
        Else
            e.Value = NormalIcon
        End If
    End Sub
    Protected Overridable Sub BindProperties(target As Control, targetProperty As String, sourceProperty As String)
        BindProperties(target, targetProperty, sourceProperty, DataSourceUpdateMode.OnPropertyChanged)
    End Sub
    Protected Overridable Sub BindProperties(target As Control, targetProperty As String, sourceProperty As String, updateMode As DataSourceUpdateMode)
        target.DataBindings.Add(targetProperty, Controller, sourceProperty, True, updateMode)
        BindToIsReadOnly(target, updateMode)
    End Sub
    Protected Overridable Sub BindProperties(target As Control, targetProperty As String, sourceProperty As String, objectToStringConverter As ConvertEventHandler)
        Dim binding As New Binding(targetProperty, Controller, sourceProperty, True)
        AddHandler binding.Format, objectToStringConverter
        target.DataBindings.Add(binding)
    End Sub
    Protected Overridable Sub BindToBoolPropertyAndInvert(target As Control, targetProperty As String, sourceProperty As String)
        target.DataBindings.Add(New BoolInvertBinding(targetProperty, Controller, sourceProperty))
        BindToIsReadOnly(target)
    End Sub
    Protected Overridable Sub BindToIsReadOnly(control As Control)
        BindToIsReadOnly(control, DataSourceUpdateMode.OnPropertyChanged)
    End Sub
    Protected Overridable Sub BindToIsReadOnly(control As Control, updateMode As DataSourceUpdateMode)
        If (Not (TypeOf control Is BaseEdit)) OrElse control.DataBindings("ReadOnly") IsNot Nothing Then
            Return
        End If
        control.DataBindings.Add("ReadOnly", Controller, "ReadOnly", True, updateMode)
    End Sub

    Protected Overridable Sub BindProperties(target As DevExpress.XtraBars.BarItem, targetProperty As String, sourceProperty As String)
        BindProperties(target, targetProperty, sourceProperty, DataSourceUpdateMode.OnPropertyChanged)
    End Sub
    Protected Overridable Sub BindProperties(target As DevExpress.XtraBars.BarItem, targetProperty As String, sourceProperty As String, updateMode As DataSourceUpdateMode)
        target.DataBindings.Add(targetProperty, Controller, sourceProperty, True, updateMode)
    End Sub
    Protected Overridable Sub BindProperties(target As DevExpress.XtraBars.BarItem, targetProperty As String, sourceProperty As String, objectToStringConverter As ConvertEventHandler)
        Dim binding As New Binding(targetProperty, Controller, sourceProperty, True)
        AddHandler binding.Format, objectToStringConverter
        target.DataBindings.Add(binding)
    End Sub
    Protected Overridable Sub BindToBoolPropertyAndInvert(target As DevExpress.XtraBars.BarItem, targetProperty As String, sourceProperty As String)
        target.DataBindings.Add(New BoolInvertBinding(targetProperty, Controller, sourceProperty))
    End Sub
    Protected Overridable Sub BindBoolToVisibility(target As DevExpress.XtraBars.BarItem, targetProperty As String, sourceProperty As String, Optional invert As Boolean = False)
        target.DataBindings.Add(New BoolToVisibilityBinding(targetProperty, Controller, sourceProperty, invert))
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        If Controller Is Nothing Then
            Return
        End If
        SubscribeControlsEvents()
        LoadFormData(Controller.EditedAppointmentCopy)
    End Sub
    Protected Overridable Function CreateController(control As SchedulerControl, apt As Appointment) As AppointmentFormController
        Return New AppointmentFormController(control, apt)
    End Function
    Protected Friend Overridable Sub LoadIcons()
        Dim asm As Assembly = GetType(SchedulerControl).Assembly
        Me.m_recurringIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm)
        Me.m_normalIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm)
    End Sub
    Protected Friend Overridable Sub SubscribeControlsEvents()
        AddHandler Me.edtEndDate.Validating, AddressOf OnEdtEndDateValidating
        AddHandler Me.edtEndDate.InvalidValue, AddressOf OnEdtEndDateInvalidValue
        AddHandler Me.edtEndTime.Validating, AddressOf OnEdtEndTimeValidating
        AddHandler Me.edtEndTime.InvalidValue, AddressOf OnEdtEndTimeInvalidValue
        AddHandler Me.riDuration.Validating, AddressOf OnCbReminderValidating
    End Sub
    Protected Friend Overridable Sub UnsubscribeControlsEvents()
        RemoveHandler Me.edtEndDate.Validating, AddressOf OnEdtEndDateValidating
        RemoveHandler Me.edtEndDate.InvalidValue, AddressOf OnEdtEndDateInvalidValue
        RemoveHandler Me.edtEndTime.Validating, AddressOf OnEdtEndTimeValidating
        RemoveHandler Me.edtEndTime.InvalidValue, AddressOf OnEdtEndTimeInvalidValue
        RemoveHandler Me.riDuration.Validating, AddressOf OnCbReminderValidating
    End Sub
    Protected Friend Overridable Sub OnEdtEndDateValidating(sender As Object, e As CancelEventArgs)
        e.Cancel = Not IsValidInterval()
        If Not e.Cancel Then
            Me.edtEndDate.DataBindings("EditValue").WriteValue()
        End If
    End Sub
    Protected Friend Overridable Sub OnEdtEndDateInvalidValue(sender As Object, e As InvalidValueExceptionEventArgs)
        e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate)
    End Sub
    Protected Friend Overridable Sub OnEdtEndTimeValidating(sender As Object, e As CancelEventArgs)
        e.Cancel = Not IsValidInterval()
        If Not e.Cancel Then
            Me.edtEndTime.DataBindings("EditValue").WriteValue()
        End If
    End Sub
    Protected Friend Overridable Sub OnEdtEndTimeInvalidValue(sender As Object, e As InvalidValueExceptionEventArgs)
        e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidEndDate)
    End Sub
    Protected Friend Overridable Function IsValidInterval() As Boolean
        Return AppointmentFormControllerBase.ValidateInterval(edtStartDate.DateTime.[Date], edtStartTime.Time.TimeOfDay, edtEndDate.DateTime.[Date], edtEndTime.Time.TimeOfDay)
    End Function
    Protected Friend Overridable Sub OnOkButton()
        Save(True)
    End Sub
    Protected Overridable Sub OnSaveButton()
        Save(False)
    End Sub
    Private Sub Save(closeAfterSave As Boolean)
        If Not ValidateEndDateAndTime() Then
            Return
        End If
        If Not SaveFormData(Controller.EditedAppointmentCopy) Then
            Return
        End If
        If Not Controller.IsConflictResolved() Then
            ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_Conflict), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        If Controller.IsAppointmentChanged() OrElse Controller.IsNewAppointment OrElse IsAppointmentChanged(Controller.EditedAppointmentCopy) Then
            Controller.ApplyChanges()
        End If
        If closeAfterSave Then
            DialogResult = System.Windows.Forms.DialogResult.OK
        End If
    End Sub
    Private Function ValidateEndDateAndTime() As Boolean
        Me.edtEndDate.DoValidate()
        Me.edtEndTime.DoValidate()

        Return [String].IsNullOrEmpty(Me.edtEndTime.ErrorText) AndAlso [String].IsNullOrEmpty(Me.edtEndDate.ErrorText)
    End Function
    Protected Overridable Sub OnSaveAsButton()
        Dim fileDialog As New SaveFileDialog()
        fileDialog.Filter = "iCalendar files (*.ics)|*.ics"
        fileDialog.FilterIndex = 1
        If fileDialog.ShowDialog() <> DialogResult.OK Then
            Return
        End If
        Try
            Using stream As Stream = fileDialog.OpenFile()
                ExportAppointment(stream)
            End Using
        Catch
            ShowMessageBox("Error: could not export appointments", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub ExportAppointment(stream As Stream)
        If stream Is Nothing Then
            Return
        End If

        Dim aptsToExport As New AppointmentBaseCollection()
        aptsToExport.Add(Controller.EditedAppointmentCopy)
        Dim exporter As New iCalendarExporter(Me.m_storage, aptsToExport)

        exporter.ProductIdentifier = "-//Developer Express Inc."
        exporter.Export(stream)
    End Sub
    Protected Friend Overridable Function ShowMessageBox(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Return XtraMessageBox.Show(Me, text, caption, buttons, icon)
    End Function
    Protected Friend Overridable Sub OnDeleteButton()
        If IsNewAppointment Then
            Return
        End If

        Controller.DeleteAppointment()

        DialogResult = DialogResult.Abort
        Close()
    End Sub
    Protected Friend Overridable Sub OnRecurrenceButton()
        If Not Controller.ShouldShowRecurrenceButton Then
            Return
        End If

        Dim patternCopy As Appointment = Controller.PrepareToRecurrenceEdit()

        Dim result As DialogResult
        Using form As Form = CreateAppointmentRecurrenceForm(patternCopy, Control.OptionsView.FirstDayOfWeek)
            result = ShowRecurrenceForm(form)
        End Using

        If result = DialogResult.Abort Then
            Controller.RemoveRecurrence()
        ElseIf result = DialogResult.OK Then
            Controller.ApplyRecurrence(patternCopy)
        End If

        Me.btnRecurrence.Down = Controller.IsRecurrentAppointment
    End Sub
    Protected Overridable Sub OnCloseButton()
        Me.Close()
    End Sub

    Private Function CancelCore() As Boolean
        Dim result As Boolean = True

        If DialogResult <> System.Windows.Forms.DialogResult.Abort AndAlso Controller IsNot Nothing AndAlso Controller.IsAppointmentChanged() AndAlso Not Me.m_supressCancelCore Then
            Dim dialogResult__1 As DialogResult = ShowMessageBox(SchedulerLocalizer.GetString(SchedulerStringId.Msg_SaveBeforeClose), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

            If dialogResult__1 = System.Windows.Forms.DialogResult.Cancel Then
                result = False
            ElseIf dialogResult__1 = System.Windows.Forms.DialogResult.Yes Then
                Save(True)
            End If
        End If

        Return result
    End Function

    Protected Overridable Function ShowRecurrenceForm(form As Form) As DialogResult
        Return FormTouchUIAdapter.ShowDialog(form, Me)
    End Function
    Protected Friend Overridable Function CreateAppointmentRecurrenceForm(patternCopy As Appointment, firstDayOfWeek As FirstDayOfWeek) As Form
        Dim form As New AppointmentRecurrenceForm(patternCopy, firstDayOfWeek, Controller)
        form.SetMenuManager(MenuManager)
        form.LookAndFeel.ParentLookAndFeel = LookAndFeel
        form.ShowExceptionsRemoveMsgBox = m_controller.AreExceptionsPresent()
        Return form
    End Function
    Friend Sub OnAppointmentFormActivated(sender As Object, e As EventArgs) Handles MyBase.Activated
        If m_openRecurrenceForm Then
            m_openRecurrenceForm = False
            OnRecurrenceButton()
        End If
    End Sub
    Protected Friend Overridable Sub OnCbReminderValidating(sender As Object, e As CancelEventArgs)
        Dim edit As DurationEdit = TryCast(sender, DurationEdit)
        Dim span As TimeSpan = DirectCast(barReminder.EditValue, TimeSpan)
        e.Cancel = span.Ticks < 0 AndAlso span <> TimeSpan.MinValue
        If Not e.Cancel Then
            Me.barReminder.DataBindings("EditValue").WriteValue()
        End If
    End Sub

    Protected Friend Overridable Sub OnNextButton()
        If CancelCore() Then
            Me.m_supressCancelCore = True

            Control.SelectNextAppointment()
            If Control.SelectedAppointments.Count > 0 Then
                Control.ShowAnotherEditAppointmentForm = True
            End If

            Me.Close()
        End If
    End Sub

    Protected Friend Overridable Sub OnPreviousButton()
        If CancelCore() Then
            Me.m_supressCancelCore = True

            Control.SelectPrevAppointment()
            If Control.SelectedAppointments.Count > 0 Then
                Control.ShowAnotherEditAppointmentForm = True
            End If

            Me.Close()
        End If
    End Sub

    Protected Friend Overridable Sub OnTimeZonesButton()
        Controller.TimeZoneVisible = Not Controller.TimeZoneVisible
    End Sub

    Protected Overridable Sub OnApplicationButtonClick()
        Me.dvInfo.Document = Control.GetPrintPreviewDocument(New MemoPrintStyle())
        Me.dvInfo.ExecCommand(DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToWholePage)
    End Sub

    Private Sub btnSaveAndClose_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnSaveAndClose.ItemClick
        OnOkButton()
    End Sub

    Private Sub barButtonDelete_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles btnDelete.ItemClick
        OnDeleteButton()
    End Sub

    Private Sub barRecurrence_ItemClick(sender As Object, e As ItemClickEventArgs) Handles btnRecurrence.ItemClick
        OnRecurrenceButton()
    End Sub

    Private Sub bvbSave_ItemClick(sender As Object, e As DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs) Handles bvbSave.ItemClick
        OnSaveButton()
    End Sub

    Private Sub bvbSaveAs_ItemClick(sender As Object, e As DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs) Handles bvbSaveAs.ItemClick
        OnSaveAsButton()
    End Sub

    Private Sub bvbClose_ItemClick(sender As Object, e As DevExpress.XtraBars.Ribbon.BackstageViewItemEventArgs) Handles bvbClose.ItemClick
        OnCloseButton()
    End Sub

    Private Sub btnSave_ItemClick(sender As Object, e As ItemClickEventArgs) Handles btnSave.ItemClick
        OnSaveButton()
    End Sub

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        e.Cancel = Not CancelCore()
        MyBase.OnClosing(e)
    End Sub

    Private Sub btnNext_ItemClick(sender As Object, e As ItemClickEventArgs) Handles btnNext.ItemClick
        OnNextButton()
    End Sub

    Private Sub btnPrevious_ItemClick(sender As Object, e As ItemClickEventArgs) Handles btnPrevious.ItemClick
        OnPreviousButton()
    End Sub

    Private Sub btnTimeZones_ItemClick(sender As Object, e As ItemClickEventArgs) Handles btnTimeZones.ItemClick
        OnTimeZonesButton()
    End Sub

    Private Sub ribbonControl1_ApplicationButtonClick(sender As Object, e As EventArgs) Handles ribbonControl1.ApplicationButtonClick
        OnApplicationButtonClick()
    End Sub
End Class

