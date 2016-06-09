<%@ Control Language="vb" AutoEventWireup="true" Inherits="SchedulerMessageBox" CodeFile="SchedulerMessageBox.ascx.vb" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web.ASPxScheduler.Controls" TagPrefix="dxsc" %>
<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web.ASPxScheduler" TagPrefix="dxwschs" %>

<div runat="server" id="root">
    <dx:ASPxLabel runat="server" EnableClientSideAPI="true" Width="100%" ID="lblMessage" Wrap="True"  Text=""></dx:ASPxLabel>
    <table <%=DevExpress.Web.Internal.RenderUtils.GetTableSpacings(Me, 0, 0)%> style="width: 100%; height: 35px;">
        <tr>
            <td class="dx-ac" style="width: 100%; height: 100%;" <%=DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(Me, "center", Nothing)%>>
                <table class="dxscButtonTable" style="height: 100%">
                    <tr>
                        <td class="dxscCellWithPadding">
                            <dx:ASPxButton runat="server" ID="btnOk" UseSubmitBehavior="false" AutoPostBack="false"
                                EnableViewState="false" Width="91px" EnableClientSideAPI="true" />
                        </td>
                        <td class="dxscCellWithPadding">
                            <dx:ASPxButton runat="server" ID="btnCancel" UseSubmitBehavior="false" AutoPostBack="false" EnableViewState="false"
                                Width="91px" CausesValidation="False" EnableClientSideAPI="true" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>

<script id="dxss_ASPxSchedulerMessageBox" type="text/javascript">
    ASPxSchedulerMessageBox = ASPx.CreateClass(ASPxSchedulerMessageBoxBase, {
        Initialize: function () {
            this.controls.btnOk.Click.AddHandler(ASPx.CreateDelegate(this.OnBtnOk, this));
            this.controls.btnCancel.Click.AddHandler(ASPx.CreateDelegate(this.OnBtnCancel, this));
        },
        UpdateMessage: function(message) { // from base
            this.controls.lblMessage.SetText(message);
        },
        CalculateDesiredWidth: function () { // from base
            var measureDiv = this.CreateMeasureDiv(this.controls.lblMessage.GetMainElement());
            measureDiv.innerHTML = this.controls.lblMessage.GetText();
            var width = measureDiv.clientWidth;
            if (width + 50 > window.innerWidth) 
                width = window.innerWidth - 50;
            ASPx.SchedulerGlobals.RemoveChildFromParent(document.body, measureDiv);
            return width;
        },
        OnBtnOk: function (s, e) {
            this.Ok();
        },
        OnBtnCancel: function(s, e) {
            this.Cancel();
        },
        CreateMeasureDiv: function (element) {
            var result = document.createElement("div");
            document.body.appendChild(result);
            result.style.cssText = element.style.cssText;
            result.style.position = "absolute";
            result.style.whiteSpace = "nowrap";
            result.style.left = "0px";
            result.style.top = "-100px";
            result.style.width = "";
            result.className = element.className;
            return result;
        }
    });
</script>