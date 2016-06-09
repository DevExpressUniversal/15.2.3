Imports Microsoft.VisualBasic
Imports System

Partial Public Class $safeitemname$
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType($safeitemname$))
        Dim superToolTip1 As New DevExpress.Utils.SuperToolTip()
        Dim toolTipTitleItem1 As New DevExpress.Utils.ToolTipTitleItem()
        Dim toolTipItem1 As New DevExpress.Utils.ToolTipItem()
        Dim superToolTip2 As New DevExpress.Utils.SuperToolTip()
        Dim toolTipTitleItem2 As New DevExpress.Utils.ToolTipTitleItem()
        Dim toolTipItem2 As New DevExpress.Utils.ToolTipItem()
        Dim superToolTip3 As New DevExpress.Utils.SuperToolTip()
        Dim toolTipTitleItem3 As New DevExpress.Utils.ToolTipTitleItem()
        Dim toolTipItem3 As New DevExpress.Utils.ToolTipItem()
        Dim superToolTip4 As New DevExpress.Utils.SuperToolTip()
        Dim toolTipTitleItem4 As New DevExpress.Utils.ToolTipTitleItem()
        Dim toolTipItem4 As New DevExpress.Utils.ToolTipItem()
        Dim optionsSpelling1 As New DevExpress.XtraSpellChecker.OptionsSpelling()
        Dim optionsSpelling2 As New DevExpress.XtraSpellChecker.OptionsSpelling()
        Dim optionsSpelling3 As New DevExpress.XtraSpellChecker.OptionsSpelling()
        Me.barManager = New DevExpress.XtraBars.BarManager(Me.components)
        Me.brTools = New DevExpress.XtraBars.Bar()
        Me.btnSaveAndClose = New DevExpress.XtraBars.BarButtonItem()
        Me.btnRecurrence = New DevExpress.XtraBars.BarButtonItem()
        Me.btnDelete = New DevExpress.XtraBars.BarButtonItem()
        Me.btnSpelling = New DevExpress.XtraBars.BarButtonItem()
        Me.brMainMenu = New DevExpress.XtraBars.Bar()
        Me.mActions = New DevExpress.XtraBars.BarSubItem()
        Me.btnSave = New DevExpress.XtraBars.BarButtonItem()
        Me.btnClose = New DevExpress.XtraBars.BarButtonItem()
        Me.barAndDockingController = New DevExpress.XtraBars.BarAndDockingController(Me.components)
        Me.barDockControlTop = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlBottom = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlLeft = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlRight = New DevExpress.XtraBars.BarDockControl()
        Me.spellChecker = New DevExpress.XtraSpellChecker.SpellChecker()
        Me.tbLocation = New DevExpress.XtraEditors.TextEdit()
        Me.layoutCtrl = New DevExpress.XtraLayout.LayoutControl()
        Me.edtResources = New DevExpress.XtraScheduler.UI.AppointmentResourcesEdit()
        Me.lblHorzSeparator1 = New DevExpress.XtraEditors.LabelControl()
        Me.lblInfo = New DevExpress.XtraEditors.LabelControl()
        Me.lblHorzSeparator2 = New DevExpress.XtraEditors.LabelControl()
        Me.tbSubject = New DevExpress.XtraEditors.TextEdit()
        Me.edtLabel = New DevExpress.XtraScheduler.UI.AppointmentLabelEdit()
        Me.edtStartDate = New DevExpress.XtraEditors.DateEdit()
        Me.cbReminder = New DevExpress.XtraScheduler.UI.DurationEdit()
        Me.edtResource = New DevExpress.XtraScheduler.UI.AppointmentResourceEdit()
        Me.edtShowTimeAs = New DevExpress.XtraScheduler.UI.AppointmentStatusEdit()
        Me.edtEndDate = New DevExpress.XtraEditors.DateEdit()
        Me.tbDescription = New DevExpress.XtraEditors.MemoEdit()
        Me.edtStartTime = New DevExpress.XtraScheduler.UI.SchedulerTimeEdit()
        Me.edtEndTime = New DevExpress.XtraScheduler.UI.SchedulerTimeEdit()
        Me.chkAllDay = New DevExpress.XtraEditors.CheckEdit()
        Me.chkReminder = New DevExpress.XtraEditors.CheckEdit()
        Me.layoutControlGroup = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.layoutLocation = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutSubject = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutLabel = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutDescription = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutShowTimeAs = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutAllDay = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutInfo = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutReminderGroup = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.layoutChkReminder = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutReminder = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutStartGroup = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.layoutStartDate = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutStartTime = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutEndGroup = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.layoutEndDate = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutEndTime = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutResourcesGroup = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.layoutResources = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutResource = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutHorzSeparator1 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.layoutHorzSeparator2 = New DevExpress.XtraLayout.LayoutControlItem()
        Me.emptySpaceItem1 = New DevExpress.XtraLayout.EmptySpaceItem()
        Me.tpAppointment = New DevExpress.XtraTab.XtraTabPage()
        Me.tabControl = New DevExpress.XtraTab.XtraTabControl()
        CType(Me.barManager, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.barAndDockingController, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbLocation.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutCtrl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.layoutCtrl.SuspendLayout()
        CType(Me.edtResources.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbSubject.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtLabel.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtStartDate.Properties.VistaTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtStartDate.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbReminder.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtResource.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtShowTimeAs.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtEndDate.Properties.VistaTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtEndDate.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbDescription.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtStartTime.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.edtEndTime.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.chkAllDay.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.chkReminder.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutControlGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutLocation, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutSubject, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutLabel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutDescription, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutShowTimeAs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutAllDay, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutInfo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutReminderGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutChkReminder, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutReminder, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutStartGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutStartDate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutStartTime, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutEndGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutEndDate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutEndTime, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutResourcesGroup, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutResources, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutResource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutHorzSeparator1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.layoutHorzSeparator2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.emptySpaceItem1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpAppointment.SuspendLayout()
        CType(Me.tabControl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabControl.SuspendLayout()
        Me.SuspendLayout()
        ' 
        ' barManager
        ' 
        Me.barManager.Bars.AddRange(New DevExpress.XtraBars.Bar() {Me.brTools, Me.brMainMenu})
        Me.barManager.Controller = Me.barAndDockingController
        Me.barManager.DockControls.Add(Me.barDockControlTop)
        Me.barManager.DockControls.Add(Me.barDockControlBottom)
        Me.barManager.DockControls.Add(Me.barDockControlLeft)
        Me.barManager.DockControls.Add(Me.barDockControlRight)
        Me.barManager.Form = Me
        Me.barManager.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.mActions, Me.btnSave, Me.btnClose, Me.btnSaveAndClose, Me.btnDelete, Me.btnRecurrence, Me.btnSpelling})
        Me.barManager.MainMenu = Me.brMainMenu
        Me.barManager.MaxItemId = 8
        ' 
        ' brTools
        ' 
        Me.brTools.BarName = "Tools"
        Me.brTools.DockCol = 0
        Me.brTools.DockRow = 1
        Me.brTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top
        Me.brTools.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.btnSaveAndClose, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.btnRecurrence, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.btnDelete, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.Standard), New DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, Me.btnSpelling, "", True, True, True, 0, Nothing, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)})
        resources.ApplyResources(Me.brTools, "brTools")
        ' 
        ' btnSaveAndClose
        ' 
        resources.ApplyResources(Me.btnSaveAndClose, "btnSaveAndClose")
        Me.btnSaveAndClose.Id = 4
        Me.btnSaveAndClose.Name = "btnSaveAndClose"
        resources.ApplyResources(toolTipTitleItem1, "toolTipTitleItem1")
        toolTipItem1.LeftIndent = 6
        resources.ApplyResources(toolTipItem1, "toolTipItem1")
        superToolTip1.Items.Add(toolTipTitleItem1)
        superToolTip1.Items.Add(toolTipItem1)
        Me.btnSaveAndClose.SuperTip = superToolTip1
        ' 
        ' btnRecurrence
        ' 
        resources.ApplyResources(Me.btnRecurrence, "btnRecurrence")
        Me.btnRecurrence.Id = 6
        Me.btnRecurrence.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.G))
        Me.btnRecurrence.Name = "btnRecurrence"
        resources.ApplyResources(toolTipTitleItem2, "toolTipTitleItem2")
        toolTipItem2.LeftIndent = 6
        resources.ApplyResources(toolTipItem2, "toolTipItem2")
        superToolTip2.Items.Add(toolTipTitleItem2)
        superToolTip2.Items.Add(toolTipItem2)
        Me.btnRecurrence.SuperTip = superToolTip2
        ' 
        ' btnDelete
        ' 
        resources.ApplyResources(Me.btnDelete, "btnDelete")
        Me.btnDelete.Id = 5
        Me.btnDelete.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D))
        Me.btnDelete.Name = "btnDelete"
        resources.ApplyResources(toolTipTitleItem3, "toolTipTitleItem3")
        toolTipItem3.LeftIndent = 6
        resources.ApplyResources(toolTipItem3, "toolTipItem3")
        superToolTip3.Items.Add(toolTipTitleItem3)
        superToolTip3.Items.Add(toolTipItem3)
        Me.btnDelete.SuperTip = superToolTip3
        ' 
        ' btnSpelling
        ' 
        resources.ApplyResources(Me.btnSpelling, "btnSpelling")
        Me.btnSpelling.Id = 7
        Me.btnSpelling.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F7)
        Me.btnSpelling.Name = "btnSpelling"
        resources.ApplyResources(toolTipTitleItem4, "toolTipTitleItem4")
        toolTipItem4.LeftIndent = 6
        resources.ApplyResources(toolTipItem4, "toolTipItem4")
        superToolTip4.Items.Add(toolTipTitleItem4)
        superToolTip4.Items.Add(toolTipItem4)
        Me.btnSpelling.SuperTip = superToolTip4
        ' 
        ' brMainMenu
        ' 
        Me.brMainMenu.BarName = "Main menu"
        Me.brMainMenu.DockCol = 0
        Me.brMainMenu.DockRow = 0
        Me.brMainMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top
        Me.brMainMenu.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.mActions)})
        Me.brMainMenu.OptionsBar.AllowQuickCustomization = False
        Me.brMainMenu.OptionsBar.MultiLine = True
        Me.brMainMenu.OptionsBar.UseWholeRow = True
        resources.ApplyResources(Me.brMainMenu, "brMainMenu")
        ' 
        ' mActions
        ' 
        resources.ApplyResources(Me.mActions, "mActions")
        Me.mActions.Id = 0
        Me.mActions.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.btnSave), New DevExpress.XtraBars.LinkPersistInfo(Me.btnDelete), New DevExpress.XtraBars.LinkPersistInfo(Me.btnRecurrence), New DevExpress.XtraBars.LinkPersistInfo(Me.btnSpelling), New DevExpress.XtraBars.LinkPersistInfo(Me.btnClose)})
        Me.mActions.Name = "mActions"
        ' 
        ' btnSave
        ' 
        resources.ApplyResources(Me.btnSave, "btnSave")
        Me.btnSave.Id = 2
        Me.btnSave.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S))
        Me.btnSave.Name = "btnSave"
        ' 
        ' btnClose
        ' 
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Id = 3
        Me.btnClose.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4))
        Me.btnClose.Name = "btnClose"
        ' 
        ' spellChecker
        ' 
        Me.spellChecker.CheckAsYouTypeOptions.CheckControlsInParentContainer = True
        Me.spellChecker.Culture = New System.Globalization.CultureInfo("en-US")
        Me.spellChecker.ParentContainer = Me
        Me.spellChecker.SpellCheckMode = DevExpress.XtraSpellChecker.SpellCheckMode.AsYouType
        ' 
        ' tbLocation
        ' 
        resources.ApplyResources(Me.tbLocation, "tbLocation")
        Me.tbLocation.Name = "tbLocation"
        Me.spellChecker.SetShowSpellCheckMenu(Me.tbLocation, True)
        Me.spellChecker.SetSpellCheckerOptions(Me.tbLocation, optionsSpelling1)
        Me.tbLocation.StyleController = Me.layoutCtrl
        ' 
        ' layoutCtrl
        ' 
        Me.layoutCtrl.Controls.Add(Me.edtResources)
        Me.layoutCtrl.Controls.Add(Me.tbLocation)
        Me.layoutCtrl.Controls.Add(Me.lblHorzSeparator1)
        Me.layoutCtrl.Controls.Add(Me.lblInfo)
        Me.layoutCtrl.Controls.Add(Me.lblHorzSeparator2)
        Me.layoutCtrl.Controls.Add(Me.tbSubject)
        Me.layoutCtrl.Controls.Add(Me.edtLabel)
        Me.layoutCtrl.Controls.Add(Me.edtStartDate)
        Me.layoutCtrl.Controls.Add(Me.cbReminder)
        Me.layoutCtrl.Controls.Add(Me.edtResource)
        Me.layoutCtrl.Controls.Add(Me.edtShowTimeAs)
        Me.layoutCtrl.Controls.Add(Me.edtEndDate)
        Me.layoutCtrl.Controls.Add(Me.tbDescription)
        Me.layoutCtrl.Controls.Add(Me.edtStartTime)
        Me.layoutCtrl.Controls.Add(Me.edtEndTime)
        Me.layoutCtrl.Controls.Add(Me.chkAllDay)
        Me.layoutCtrl.Controls.Add(Me.chkReminder)
        resources.ApplyResources(Me.layoutCtrl, "layoutCtrl")
        Me.layoutCtrl.Name = "layoutCtrl"
        Me.layoutCtrl.Root = Me.layoutControlGroup
        ' 
        ' edtResources
        ' 
        resources.ApplyResources(Me.edtResources, "edtResources")
        Me.edtResources.Name = "edtResources"
        Me.edtResources.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("edtResources.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.edtResources.StyleController = Me.layoutCtrl
        ' 
        ' lblHorzSeparator1
        ' 
        Me.lblHorzSeparator1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal
        Me.lblHorzSeparator1.LineVisible = True
        resources.ApplyResources(Me.lblHorzSeparator1, "lblHorzSeparator1")
        Me.lblHorzSeparator1.Name = "lblHorzSeparator1"
        Me.lblHorzSeparator1.StyleController = Me.layoutCtrl
        ' 
        ' lblInfo
        ' 
        Me.lblInfo.Appearance.BackColor = System.Drawing.Color.Silver
        Me.lblInfo.Appearance.Options.UseBackColor = True
        resources.ApplyResources(Me.lblInfo, "lblInfo")
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.StyleController = Me.layoutCtrl
        ' 
        ' lblHorzSeparator2
        ' 
        Me.lblHorzSeparator2.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal
        Me.lblHorzSeparator2.LineVisible = True
        resources.ApplyResources(Me.lblHorzSeparator2, "lblHorzSeparator2")
        Me.lblHorzSeparator2.Name = "lblHorzSeparator2"
        Me.lblHorzSeparator2.StyleController = Me.layoutCtrl
        ' 
        ' tbSubject
        ' 
        resources.ApplyResources(Me.tbSubject, "tbSubject")
        Me.tbSubject.Name = "tbSubject"
        Me.spellChecker.SetShowSpellCheckMenu(Me.tbSubject, True)
        Me.spellChecker.SetSpellCheckerOptions(Me.tbSubject, optionsSpelling2)
        Me.tbSubject.StyleController = Me.layoutCtrl
        ' 
        ' edtLabel
        ' 
        resources.ApplyResources(Me.edtLabel, "edtLabel")
        Me.edtLabel.Name = "edtLabel"
        Me.edtLabel.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("edtLabel.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.edtLabel.StyleController = Me.layoutCtrl
        ' 
        ' edtStartDate
        ' 
        resources.ApplyResources(Me.edtStartDate, "edtStartDate")
        Me.edtStartDate.Name = "edtStartDate"
        Me.edtStartDate.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("edtStartDate.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.edtStartDate.Properties.VistaTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.edtStartDate.StyleController = Me.layoutCtrl
        ' 
        ' cbReminder
        ' 
        resources.ApplyResources(Me.cbReminder, "cbReminder")
        Me.cbReminder.Name = "cbReminder"
        Me.cbReminder.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("cbReminder.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.cbReminder.StyleController = Me.layoutCtrl
        ' 
        ' edtResource
        ' 
        resources.ApplyResources(Me.edtResource, "edtResource")
        Me.edtResource.Name = "edtResource"
        Me.edtResource.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("edtResource.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.edtResource.StyleController = Me.layoutCtrl
        ' 
        ' edtShowTimeAs
        ' 
        resources.ApplyResources(Me.edtShowTimeAs, "edtShowTimeAs")
        Me.edtShowTimeAs.Name = "edtShowTimeAs"
        Me.edtShowTimeAs.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("edtShowTimeAs.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.edtShowTimeAs.StyleController = Me.layoutCtrl
        ' 
        ' edtEndDate
        ' 
        resources.ApplyResources(Me.edtEndDate, "edtEndDate")
        Me.edtEndDate.Name = "edtEndDate"
        Me.edtEndDate.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton((CType(resources.GetObject("edtEndDate.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines)))})
        Me.edtEndDate.Properties.VistaTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.edtEndDate.StyleController = Me.layoutCtrl
        ' 
        ' tbDescription
        ' 
        resources.ApplyResources(Me.tbDescription, "tbDescription")
        Me.tbDescription.Name = "tbDescription"
        Me.spellChecker.SetShowSpellCheckMenu(Me.tbDescription, True)
        Me.spellChecker.SetSpellCheckerOptions(Me.tbDescription, optionsSpelling3)
        Me.tbDescription.StyleController = Me.layoutCtrl
        ' 
        ' edtStartTime
        ' 
        resources.ApplyResources(Me.edtStartTime, "edtStartTime")
        Me.edtStartTime.Name = "edtStartTime"
        Me.edtStartTime.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.edtStartTime.StyleController = Me.layoutCtrl
        ' 
        ' edtEndTime
        ' 
        resources.ApplyResources(Me.edtEndTime, "edtEndTime")
        Me.edtEndTime.Name = "edtEndTime"
        Me.edtEndTime.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.edtEndTime.StyleController = Me.layoutCtrl
        ' 
        ' chkAllDay
        ' 
        resources.ApplyResources(Me.chkAllDay, "chkAllDay")
        Me.chkAllDay.Name = "chkAllDay"
        Me.chkAllDay.Properties.Caption = resources.GetString("chkAllDay.Properties.Caption")
        Me.chkAllDay.StyleController = Me.layoutCtrl
        ' 
        ' chkReminder
        ' 
        resources.ApplyResources(Me.chkReminder, "chkReminder")
        Me.chkReminder.Name = "chkReminder"
        Me.chkReminder.Properties.Caption = resources.GetString("chkReminder.Properties.Caption")
        Me.chkReminder.StyleController = Me.layoutCtrl
        ' 
        ' layoutControlGroup
        ' 
        resources.ApplyResources(Me.layoutControlGroup, "layoutControlGroup")
        Me.layoutControlGroup.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.layoutLocation, Me.layoutSubject, Me.layoutLabel, Me.layoutDescription, Me.layoutShowTimeAs, Me.layoutAllDay, Me.layoutInfo, Me.layoutReminderGroup, Me.layoutStartGroup, Me.layoutEndGroup, Me.layoutResourcesGroup, Me.layoutHorzSeparator1, Me.layoutHorzSeparator2, Me.emptySpaceItem1})
        Me.layoutControlGroup.Location = New System.Drawing.Point(0, 0)
        Me.layoutControlGroup.Name = "layoutControlGroup"
        Me.layoutControlGroup.Size = New System.Drawing.Size(543, 340)
        Me.layoutControlGroup.TextVisible = False
        ' 
        ' layoutLocation
        ' 
        Me.layoutLocation.Control = Me.tbLocation
        resources.ApplyResources(Me.layoutLocation, "layoutLocation")
        Me.layoutLocation.Location = New System.Drawing.Point(0, 50)
        Me.layoutLocation.Name = "layoutLocation"
        Me.layoutLocation.Size = New System.Drawing.Size(309, 31)
        Me.layoutLocation.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutLocation.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutSubject
        ' 
        Me.layoutSubject.Control = Me.tbSubject
        resources.ApplyResources(Me.layoutSubject, "layoutSubject")
        Me.layoutSubject.Location = New System.Drawing.Point(0, 19)
        Me.layoutSubject.Name = "layoutSubject"
        Me.layoutSubject.Size = New System.Drawing.Size(537, 31)
        Me.layoutSubject.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutSubject.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutLabel
        ' 
        Me.layoutLabel.Control = Me.edtLabel
        resources.ApplyResources(Me.layoutLabel, "layoutLabel")
        Me.layoutLabel.Location = New System.Drawing.Point(309, 50)
        Me.layoutLabel.Name = "layoutLabel"
        Me.layoutLabel.Size = New System.Drawing.Size(228, 31)
        Me.layoutLabel.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutLabel.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutDescription
        ' 
        Me.layoutDescription.Control = Me.tbDescription
        resources.ApplyResources(Me.layoutDescription, "layoutDescription")
        Me.layoutDescription.Location = New System.Drawing.Point(0, 253)
        Me.layoutDescription.Name = "layoutDescription"
        Me.layoutDescription.Size = New System.Drawing.Size(537, 81)
        Me.layoutDescription.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutDescription.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutDescription.TextToControlDistance = 0
        Me.layoutDescription.TextVisible = False
        ' 
        ' layoutShowTimeAs
        ' 
        Me.layoutShowTimeAs.Control = Me.edtShowTimeAs
        resources.ApplyResources(Me.layoutShowTimeAs, "layoutShowTimeAs")
        Me.layoutShowTimeAs.Location = New System.Drawing.Point(0, 222)
        Me.layoutShowTimeAs.Name = "layoutShowTimeAs"
        Me.layoutShowTimeAs.Size = New System.Drawing.Size(309, 31)
        Me.layoutShowTimeAs.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutShowTimeAs.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutAllDay
        ' 
        Me.layoutAllDay.Control = Me.chkAllDay
        resources.ApplyResources(Me.layoutAllDay, "layoutAllDay")
        Me.layoutAllDay.Location = New System.Drawing.Point(309, 167)
        Me.layoutAllDay.Name = "layoutAllDay"
        Me.layoutAllDay.Size = New System.Drawing.Size(104, 31)
        Me.layoutAllDay.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutAllDay.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutAllDay.TextToControlDistance = 0
        Me.layoutAllDay.TextVisible = False
        ' 
        ' layoutInfo
        ' 
        Me.layoutInfo.Control = Me.lblInfo
        resources.ApplyResources(Me.layoutInfo, "layoutInfo")
        Me.layoutInfo.Location = New System.Drawing.Point(0, 0)
        Me.layoutInfo.Name = "layoutInfo"
        Me.layoutInfo.Size = New System.Drawing.Size(537, 19)
        Me.layoutInfo.TextLocation = DevExpress.Utils.Locations.Top
        Me.layoutInfo.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutInfo.TextToControlDistance = 0
        Me.layoutInfo.TextVisible = False
        ' 
        ' layoutReminderGroup
        ' 
        resources.ApplyResources(Me.layoutReminderGroup, "layoutReminderGroup")
        Me.layoutReminderGroup.GroupBordersVisible = False
        Me.layoutReminderGroup.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.layoutChkReminder, Me.layoutReminder})
        Me.layoutReminderGroup.Location = New System.Drawing.Point(309, 222)
        Me.layoutReminderGroup.Name = "layoutReminderGroup"
        Me.layoutReminderGroup.Size = New System.Drawing.Size(228, 31)
        ' 
        ' layoutChkReminder
        ' 
        Me.layoutChkReminder.Control = Me.chkReminder
        resources.ApplyResources(Me.layoutChkReminder, "layoutChkReminder")
        Me.layoutChkReminder.Location = New System.Drawing.Point(0, 0)
        Me.layoutChkReminder.Name = "layoutChkReminder"
        Me.layoutChkReminder.Size = New System.Drawing.Size(86, 31)
        Me.layoutChkReminder.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutChkReminder.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutChkReminder.TextToControlDistance = 0
        Me.layoutChkReminder.TextVisible = False
        ' 
        ' layoutReminder
        ' 
        Me.layoutReminder.Control = Me.cbReminder
        resources.ApplyResources(Me.layoutReminder, "layoutReminder")
        Me.layoutReminder.Location = New System.Drawing.Point(86, 0)
        Me.layoutReminder.Name = "layoutReminder"
        Me.layoutReminder.Size = New System.Drawing.Size(142, 31)
        Me.layoutReminder.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutReminder.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutReminder.TextToControlDistance = 0
        Me.layoutReminder.TextVisible = False
        ' 
        ' layoutStartGroup
        ' 
        resources.ApplyResources(Me.layoutStartGroup, "layoutStartGroup")
        Me.layoutStartGroup.GroupBordersVisible = False
        Me.layoutStartGroup.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.layoutStartDate, Me.layoutStartTime})
        Me.layoutStartGroup.Location = New System.Drawing.Point(0, 105)
        Me.layoutStartGroup.Name = "layoutStartGroup"
        Me.layoutStartGroup.Size = New System.Drawing.Size(309, 62)
        ' 
        ' layoutStartDate
        ' 
        Me.layoutStartDate.Control = Me.edtStartDate
        resources.ApplyResources(Me.layoutStartDate, "layoutStartDate")
        Me.layoutStartDate.Location = New System.Drawing.Point(0, 0)
        Me.layoutStartDate.Name = "layoutStartDate"
        Me.layoutStartDate.Size = New System.Drawing.Size(210, 62)
        Me.layoutStartDate.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutStartDate.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutStartTime
        ' 
        Me.layoutStartTime.Control = Me.edtStartTime
        resources.ApplyResources(Me.layoutStartTime, "layoutStartTime")
        Me.layoutStartTime.Location = New System.Drawing.Point(210, 0)
        Me.layoutStartTime.Name = "layoutStartTime"
        Me.layoutStartTime.Size = New System.Drawing.Size(99, 62)
        Me.layoutStartTime.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutStartTime.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutStartTime.TextToControlDistance = 0
        Me.layoutStartTime.TextVisible = False
        ' 
        ' layoutEndGroup
        ' 
        resources.ApplyResources(Me.layoutEndGroup, "layoutEndGroup")
        Me.layoutEndGroup.GroupBordersVisible = False
        Me.layoutEndGroup.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.layoutEndDate, Me.layoutEndTime})
        Me.layoutEndGroup.Location = New System.Drawing.Point(0, 167)
        Me.layoutEndGroup.Name = "layoutEndGroup"
        Me.layoutEndGroup.Size = New System.Drawing.Size(309, 31)
        ' 
        ' layoutEndDate
        ' 
        Me.layoutEndDate.Control = Me.edtEndDate
        resources.ApplyResources(Me.layoutEndDate, "layoutEndDate")
        Me.layoutEndDate.Location = New System.Drawing.Point(0, 0)
        Me.layoutEndDate.Name = "layoutEndDate"
        Me.layoutEndDate.Size = New System.Drawing.Size(210, 31)
        Me.layoutEndDate.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutEndDate.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutEndTime
        ' 
        Me.layoutEndTime.Control = Me.edtEndTime
        resources.ApplyResources(Me.layoutEndTime, "layoutEndTime")
        Me.layoutEndTime.Location = New System.Drawing.Point(210, 0)
        Me.layoutEndTime.Name = "layoutEndTime"
        Me.layoutEndTime.Size = New System.Drawing.Size(99, 31)
        Me.layoutEndTime.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutEndTime.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutEndTime.TextToControlDistance = 0
        Me.layoutEndTime.TextVisible = False
        ' 
        ' layoutResourcesGroup
        ' 
        resources.ApplyResources(Me.layoutResourcesGroup, "layoutResourcesGroup")
        Me.layoutResourcesGroup.GroupBordersVisible = False
        Me.layoutResourcesGroup.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.layoutResources, Me.layoutResource})
        Me.layoutResourcesGroup.Location = New System.Drawing.Point(309, 105)
        Me.layoutResourcesGroup.Name = "layoutResourcesGroup"
        Me.layoutResourcesGroup.Size = New System.Drawing.Size(228, 62)
        ' 
        ' layoutResources
        ' 
        Me.layoutResources.Control = Me.edtResources
        resources.ApplyResources(Me.layoutResources, "layoutResources")
        Me.layoutResources.Location = New System.Drawing.Point(0, 0)
        Me.layoutResources.Name = "layoutResources"
        Me.layoutResources.Size = New System.Drawing.Size(228, 31)
        Me.layoutResources.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutResources.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutResource
        ' 
        Me.layoutResource.Control = Me.edtResource
        resources.ApplyResources(Me.layoutResource, "layoutResource")
        Me.layoutResource.Location = New System.Drawing.Point(0, 31)
        Me.layoutResource.Name = "layoutResource"
        Me.layoutResource.Size = New System.Drawing.Size(228, 31)
        Me.layoutResource.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutResource.TextSize = New System.Drawing.Size(67, 20)
        ' 
        ' layoutHorzSeparator1
        ' 
        Me.layoutHorzSeparator1.Control = Me.lblHorzSeparator1
        resources.ApplyResources(Me.layoutHorzSeparator1, "layoutHorzSeparator1")
        Me.layoutHorzSeparator1.Location = New System.Drawing.Point(0, 81)
        Me.layoutHorzSeparator1.Name = "layoutHorzSeparator1"
        Me.layoutHorzSeparator1.Size = New System.Drawing.Size(537, 24)
        Me.layoutHorzSeparator1.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutHorzSeparator1.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutHorzSeparator1.TextToControlDistance = 0
        Me.layoutHorzSeparator1.TextVisible = False
        ' 
        ' layoutHorzSeparator2
        ' 
        Me.layoutHorzSeparator2.Control = Me.lblHorzSeparator2
        resources.ApplyResources(Me.layoutHorzSeparator2, "layoutHorzSeparator2")
        Me.layoutHorzSeparator2.Location = New System.Drawing.Point(0, 198)
        Me.layoutHorzSeparator2.Name = "layoutHorzSeparator2"
        Me.layoutHorzSeparator2.Size = New System.Drawing.Size(537, 24)
        Me.layoutHorzSeparator2.TextLocation = DevExpress.Utils.Locations.Left
        Me.layoutHorzSeparator2.TextSize = New System.Drawing.Size(0, 0)
        Me.layoutHorzSeparator2.TextToControlDistance = 0
        Me.layoutHorzSeparator2.TextVisible = False
        ' 
        ' emptySpaceItem1
        ' 
        resources.ApplyResources(Me.emptySpaceItem1, "emptySpaceItem1")
        Me.emptySpaceItem1.Location = New System.Drawing.Point(413, 167)
        Me.emptySpaceItem1.Name = "emptySpaceItem1"
        Me.emptySpaceItem1.Size = New System.Drawing.Size(124, 31)
        Me.emptySpaceItem1.TextSize = New System.Drawing.Size(0, 0)
        ' 
        ' tpAppointment
        ' 
        Me.tpAppointment.Controls.Add(Me.layoutCtrl)
        Me.tpAppointment.Name = "tpAppointment"
        resources.ApplyResources(Me.tpAppointment, "tpAppointment")
        ' 
        ' tabControl
        ' 
        resources.ApplyResources(Me.tabControl, "tabControl")
        Me.tabControl.Name = "tabControl"
        Me.tabControl.SelectedTabPage = Me.tpAppointment
        Me.tabControl.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.tpAppointment})
        ' 
        ' $safeitemname$
        ' 
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.tabControl)
        Me.Controls.Add(Me.barDockControlLeft)
        Me.Controls.Add(Me.barDockControlRight)
        Me.Controls.Add(Me.barDockControlBottom)
        Me.Controls.Add(Me.barDockControlTop)
        Me.Name = "$safeitemname$"
        Me.ShowInTaskbar = False
        CType(Me.barManager, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.barAndDockingController, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbLocation.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutCtrl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.layoutCtrl.ResumeLayout(False)
        CType(Me.edtResources.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbSubject.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtLabel.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtStartDate.Properties.VistaTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtStartDate.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbReminder.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtResource.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtShowTimeAs.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtEndDate.Properties.VistaTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtEndDate.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbDescription.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtStartTime.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.edtEndTime.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.chkAllDay.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.chkReminder.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutControlGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutLocation, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutSubject, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutLabel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutDescription, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutShowTimeAs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutAllDay, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutInfo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutReminderGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutChkReminder, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutReminder, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutStartGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutStartDate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutStartTime, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutEndGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutEndDate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutEndTime, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutResourcesGroup, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutResources, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutResource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutHorzSeparator1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.layoutHorzSeparator2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.emptySpaceItem1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpAppointment.ResumeLayout(False)
        CType(Me.tabControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabControl.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Protected spellChecker As DevExpress.XtraSpellChecker.SpellChecker
    Protected tabControl As DevExpress.XtraTab.XtraTabControl
    Protected tpAppointment As DevExpress.XtraTab.XtraTabPage
    Protected Friend barAndDockingController As DevExpress.XtraBars.BarAndDockingController
    Protected chkReminder As DevExpress.XtraEditors.CheckEdit
    Protected chkAllDay As DevExpress.XtraEditors.CheckEdit
    Protected edtEndDate As DevExpress.XtraEditors.DateEdit
    Protected edtStartDate As DevExpress.XtraEditors.DateEdit
    Protected edtStartTime As DevExpress.XtraScheduler.UI.SchedulerTimeEdit
    Protected edtEndTime As DevExpress.XtraScheduler.UI.SchedulerTimeEdit
    Protected edtLabel As DevExpress.XtraScheduler.UI.AppointmentLabelEdit
    Protected edtShowTimeAs As DevExpress.XtraScheduler.UI.AppointmentStatusEdit
    Protected tbSubject As DevExpress.XtraEditors.TextEdit
    Protected tbLocation As DevExpress.XtraEditors.TextEdit
    Protected edtResources As DevExpress.XtraScheduler.UI.AppointmentResourcesEdit
    Protected edtResource As DevExpress.XtraScheduler.UI.AppointmentResourceEdit
    Protected tbDescription As DevExpress.XtraEditors.MemoEdit
    Protected cbReminder As DevExpress.XtraScheduler.UI.DurationEdit
    Protected lblInfo As DevExpress.XtraEditors.LabelControl
    Protected layoutCtrl As DevExpress.XtraLayout.LayoutControl
    Protected layoutControlGroup As DevExpress.XtraLayout.LayoutControlGroup
    Protected layoutResources As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutLocation As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutSubject As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutLabel As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutEndDate As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutStartDate As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutReminder As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutDescription As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutShowTimeAs As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutStartTime As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutEndTime As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutChkReminder As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutAllDay As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutInfo As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutResource As DevExpress.XtraLayout.LayoutControlItem
    Protected layoutReminderGroup As DevExpress.XtraLayout.LayoutControlGroup
    Protected layoutStartGroup As DevExpress.XtraLayout.LayoutControlGroup
    Protected layoutEndGroup As DevExpress.XtraLayout.LayoutControlGroup
    Protected layoutResourcesGroup As DevExpress.XtraLayout.LayoutControlGroup
    Protected lblHorzSeparator1 As DevExpress.XtraEditors.LabelControl
    Protected layoutHorzSeparator1 As DevExpress.XtraLayout.LayoutControlItem
    Protected lblHorzSeparator2 As DevExpress.XtraEditors.LabelControl
    Protected layoutHorzSeparator2 As DevExpress.XtraLayout.LayoutControlItem
    Protected emptySpaceItem1 As DevExpress.XtraLayout.EmptySpaceItem
    Protected barManager As DevExpress.XtraBars.BarManager
    Protected brMainMenu As DevExpress.XtraBars.Bar
    Protected barDockControlTop As DevExpress.XtraBars.BarDockControl
    Protected barDockControlBottom As DevExpress.XtraBars.BarDockControl
    Protected barDockControlLeft As DevExpress.XtraBars.BarDockControl
    Protected barDockControlRight As DevExpress.XtraBars.BarDockControl
    Protected brTools As DevExpress.XtraBars.Bar
    Protected mActions As DevExpress.XtraBars.BarSubItem
    Protected btnSave As DevExpress.XtraBars.BarButtonItem
    Protected btnClose As DevExpress.XtraBars.BarButtonItem
    Protected btnSaveAndClose As DevExpress.XtraBars.BarButtonItem
    Protected btnDelete As DevExpress.XtraBars.BarButtonItem
    Protected btnRecurrence As DevExpress.XtraBars.BarButtonItem
    Protected btnSpelling As DevExpress.XtraBars.BarButtonItem
End Class
