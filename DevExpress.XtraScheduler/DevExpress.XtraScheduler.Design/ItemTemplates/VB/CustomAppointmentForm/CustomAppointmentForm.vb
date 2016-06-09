Imports System.Drawing
Imports System.ComponentModel
Imports System.Reflection
Imports System.Windows.Forms
Imports DevExpress.Utils.Controls
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Localization
Imports DevExpress.XtraScheduler.Native
Imports DevExpress.XtraScheduler.UI
Imports DevExpress.Utils
Imports DevExpress.Utils.Menu
Imports DevExpress.XtraEditors.Native
Imports DevExpress.Utils.Internal

Partial Public Class $safeitemname$
    Inherits DevExpress.XtraEditors.XtraForm
    Implements IDXManagerPopupMenu
#Region "Fields"
    Private m_openRecurrenceForm As Boolean
    ReadOnly m_storage As SchedulerStorage
    ReadOnly m_control As SchedulerControl
    Private m_recurringIcon As Icon
    Private m_normalIcon As Icon
    ReadOnly m_controller As AppointmentFormController
    Private m_menuManager As IDXMenuManager
#End Region

    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Sub New()
        InitializeComponent()
    End Sub
    Public Sub New(control As DevExpress.XtraScheduler.SchedulerControl, apt As DevExpress.XtraScheduler.Appointment)
        Me.New(control, apt, False)
    End Sub
    Public Sub New(control As DevExpress.XtraScheduler.SchedulerControl, apt As DevExpress.XtraScheduler.Appointment, openRecurrenceForm As Boolean)
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

        Me.edtShowTimeAs.Storage = Me.m_storage
        Me.edtLabel.Storage = m_storage
        Me.edtResource.SchedulerControl = control
        Me.edtResource.Storage = m_storage
        Me.edtResources.SchedulerControl = control

        SubscribeControllerEvents(Controller)
        BindControllerToControls()
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
    Protected Friend ReadOnly Property Storage() As SchedulerStorage
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
            Return Controller IsNot Nothing AndAlso Controller.[ReadOnly]
        End Get
        Set(value As Boolean)
            If Controller.[ReadOnly] = value Then
                Return
            End If
            Controller.[ReadOnly] = value
        End Set
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
        BindControllerToIcon()
        BindProperties(Me.tbSubject, "Text", "Subject")
        BindProperties(Me.tbLocation, "Text", "Location")
        BindProperties(Me.tbDescription, "Text", "Description")
        BindProperties(Me.edtShowTimeAs, "Status", "Status")
        BindProperties(Me.edtStartDate, "EditValue", "DisplayStartDate")
        BindProperties(Me.edtStartDate, "Enabled", "IsDateTimeEditable")
        BindProperties(Me.edtStartTime, "EditValue", "DisplayStartTime")
        BindProperties(Me.edtStartTime, "Visible", "IsTimeVisible")
        BindProperties(Me.edtStartTime, "Enabled", "IsTimeVisible")
        BindProperties(Me.edtEndDate, "EditValue", "DisplayEndDate", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndDate, "Enabled", "IsDateTimeEditable", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndTime, "EditValue", "DisplayEndTime", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndTime, "Visible", "IsTimeVisible", DataSourceUpdateMode.Never)
        BindProperties(Me.edtEndTime, "Enabled", "IsTimeVisible", DataSourceUpdateMode.Never)
        BindProperties(Me.chkAllDay, "Checked", "AllDay")
        BindProperties(Me.chkAllDay, "Enabled", "IsDateTimeEditable")

        BindProperties(Me.edtResource, "ResourceId", "ResourceId")
        BindProperties(Me.edtResource, "Enabled", "CanEditResource")
        BindToBoolPropertyAndInvert(Me.edtResource, "Visible", "ResourceSharing")

        BindProperties(Me.edtResources, "ResourceIds", "ResourceIds")
        BindProperties(Me.edtResources, "Visible", "ResourceSharing")
        BindProperties(Me.edtResources, "Enabled", "CanEditResource")
        BindProperties(Me.lblResource, "Enabled", "CanEditResource")

        BindProperties(Me.edtLabel, "Label", "Label")
        BindProperties(Me.chkReminder, "Enabled", "ReminderVisible")
        BindProperties(Me.chkReminder, "Visible", "ReminderVisible")
        BindProperties(Me.chkReminder, "Checked", "HasReminder")
        BindProperties(Me.cbReminder, "Enabled", "HasReminder")
        BindProperties(Me.cbReminder, "Visible", "ReminderVisible")
        BindProperties(Me.cbReminder, "Duration", "ReminderTimeBeforeStart")

        BindProperties(Me.tbProgress, "Value", "PercentComplete")
        BindProperties(Me.lblPercentCompleteValue, "Text", "PercentComplete", AddressOf ObjectToStringConverter)
        BindProperties(Me.progressPanel, "Visible", "ShouldEditTaskProgress")
        BindToBoolPropertyAndInvert(Me.btnOk, "Enabled", "ReadOnly")
        BindToBoolPropertyAndInvert(Me.btnRecurrence, "Enabled", "ReadOnly")
        BindProperties(Me.btnDelete, "Enabled", "CanDeleteAppointment")
        BindProperties(Me.btnRecurrence, "Visible", "ShouldShowRecurrenceButton")
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
    End Sub
    Protected Overridable Sub BindProperties(target As Control, targetProperty As String, sourceProperty As String, objectToStringConverter As ConvertEventHandler)
        Dim binding As New Binding(targetProperty, Controller, sourceProperty, True)
        AddHandler binding.Format, objectToStringConverter
        target.DataBindings.Add(binding)
    End Sub
    Protected Overridable Sub BindToBoolPropertyAndInvert(target As Control, targetProperty As String, sourceProperty As String)
        target.DataBindings.Add(New BoolInvertBinding(targetProperty, Controller, sourceProperty))
    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        If Controller Is Nothing Then
            Return
        End If
        Me.DataBindings.Add("Text", Controller, "Caption")
        SubscribeControlsEvents()
        LoadFormData(Controller.EditedAppointmentCopy)
        RecalculateLayoutOfControlsAffectedByProgressPanel()
    End Sub
    Protected Overridable Function CreateController(control As SchedulerControl, apt As Appointment) As AppointmentFormController
        Return New AppointmentFormController(control, apt)
    End Function
    Private Sub SubscribeControllerEvents(ByVal controller As AppointmentFormController)
        If controller Is Nothing Then
            Return
        End If
        AddHandler controller.PropertyChanged, AddressOf OnControllerPropertyChanged
    End Sub
    Private Sub OnControllerPropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        If e.PropertyName = "ReadOnly" Then
            UpdateReadonly()
        End If
    End Sub
    Protected Overridable Sub UpdateReadonly()
        If Controller Is Nothing Then
            Return
        End If
        Dim controls As IList(Of Control) = GetAllControls(Me)

        For Each control_Renamed As Control In controls
            Dim editor As BaseEdit = TryCast(control_Renamed, BaseEdit)
            If editor Is Nothing Then
                Continue For
            End If
            editor.ReadOnly = Controller.ReadOnly
        Next control_Renamed
        Me.btnOk.Enabled = Not Controller.ReadOnly
        Me.btnRecurrence.Enabled = Not Controller.ReadOnly
    End Sub

    Private Function GetAllControls(ByVal rootControl As Control) As List(Of Control)
        Dim result As New List(Of Control)()

        For Each control_Renamed As Control In rootControl.Controls
            result.Add(control_Renamed)
            Dim childControls As IList(Of Control) = GetAllControls(control_Renamed)
            result.AddRange(childControls)
        Next control_Renamed
        Return result
    End Function
    Protected Friend Overridable Sub LoadIcons()
        Dim asm As Assembly = GetType(SchedulerControl).Assembly
        m_recurringIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.RecurringAppointment, asm)
        m_normalIcon = ResourceImageHelper.CreateIconFromResources(SchedulerIconNames.Appointment, asm)
    End Sub
    Protected Friend Overridable Sub SubscribeControlsEvents()
        AddHandler Me.edtEndDate.Validating, AddressOf OnEdtEndDateValidating
        AddHandler Me.edtEndDate.InvalidValue, AddressOf OnEdtEndDateInvalidValue
        AddHandler Me.edtEndTime.Validating, AddressOf OnEdtEndTimeValidating
        AddHandler Me.edtEndTime.InvalidValue, AddressOf OnEdtEndTimeInvalidValue
        AddHandler Me.cbReminder.InvalidValue, AddressOf OnCbReminderInvalidValue
        AddHandler Me.cbReminder.Validating, AddressOf OnCbReminderValidating
    End Sub
    Protected Friend Overridable Sub UnsubscribeControlsEvents()
        RemoveHandler Me.edtEndDate.Validating, AddressOf OnEdtEndDateValidating
        RemoveHandler Me.edtEndDate.InvalidValue, AddressOf OnEdtEndDateInvalidValue
        RemoveHandler Me.edtEndTime.Validating, AddressOf OnEdtEndTimeValidating
        RemoveHandler Me.edtEndTime.InvalidValue, AddressOf OnEdtEndTimeInvalidValue
        RemoveHandler Me.cbReminder.InvalidValue, AddressOf OnCbReminderInvalidValue
        RemoveHandler Me.cbReminder.Validating, AddressOf OnCbReminderValidating
    End Sub
    Private Sub OnBtnOkClick(sender As Object, e As System.EventArgs) Handles btnOk.Click
        OnOkButton()
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

        Me.DialogResult = DialogResult.OK
    End Sub
    Protected Friend Overridable Function ShowMessageBox(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Return XtraMessageBox.Show(Me, text, caption, buttons, icon)
    End Function
    Private Sub OnBtnDeleteClick(sender As Object, e As System.EventArgs) Handles btnDelete.Click
        OnDeleteButton()
    End Sub
    Protected Friend Overridable Sub OnDeleteButton()
        If IsNewAppointment Then
            Return
        End If

        Controller.DeleteAppointment()

        DialogResult = DialogResult.Abort
        Close()
    End Sub
    Private Sub OnBtnRecurrenceClick(sender As Object, e As System.EventArgs) Handles btnRecurrence.Click
        OnRecurrenceButton()
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
    End Sub
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
        Dim span As TimeSpan = cbReminder.Duration
        e.Cancel = (span = TimeSpan.MinValue) OrElse (span.Ticks < 0)
        If Not e.Cancel Then
            Me.cbReminder.DataBindings("Duration").WriteValue()
        End If
    End Sub
    Protected Friend Overridable Sub OnCbReminderInvalidValue(sender As Object, e As InvalidValueExceptionEventArgs)
        e.ErrorText = SchedulerLocalizer.GetString(SchedulerStringId.Msg_InvalidReminderTimeBeforeStart)
    End Sub
    Protected Friend Overridable Sub RecalculateLayoutOfControlsAffectedByProgressPanel()
        If progressPanel.Visible Then
            Return
        End If
        Dim intDeltaY As Integer = progressPanel.Height
        tbDescription.Location = New Point(tbDescription.Location.X, tbDescription.Location.Y - intDeltaY)
        tbDescription.Size = New Size(tbDescription.Size.Width, tbDescription.Size.Height + intDeltaY)
    End Sub
End Class
