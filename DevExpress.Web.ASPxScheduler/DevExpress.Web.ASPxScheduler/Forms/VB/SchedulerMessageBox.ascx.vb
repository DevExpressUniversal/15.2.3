Imports System
Imports System.Web.UI
Imports DevExpress.XtraScheduler
Imports DevExpress.Web
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.Web.ASPxScheduler.Internal
Imports System.Collections
Imports System.Collections.Generic
Imports DevExpress.XtraScheduler.Localization
Imports DevExpress.Web.ASPxScheduler.Localization
Imports DevExpress.Utils
Imports System.Web.UI.WebControls

Partial Public Class SchedulerMessageBox
    Inherits SchedulerMessageBoxBase

    Public Overrides ReadOnly Property ClassName() As String
        Get
            Return "ASPxSchedulerMessageBox"
        End Get
    End Property

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        Localize()
    End Sub

    Private Sub Localize()
        btnOk.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonOk)
        btnCancel.Text = ASPxSchedulerLocalizer.GetString(ASPxSchedulerStringId.Form_ButtonCancel)
        btnOk.Wrap = DefaultBoolean.False
        btnCancel.Wrap = DefaultBoolean.False
    End Sub

    Protected Overrides Function GetChildEditors() As ASPxEditBase()
        Return New ASPxEditBase() {}
    End Function

    Protected Overrides Function GetChildButtons() As ASPxButton()
        Dim buttons() As ASPxButton = {btnOk, btnCancel}
        Return buttons
    End Function
    Protected Overrides Function GetChildControls() As Control()
        Return New Control() {root, lblMessage}
    End Function
    Protected Overrides Function GetDefaultButton() As WebControl
        Return btnOk
    End Function
End Class
