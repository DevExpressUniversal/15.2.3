<%@ Control Language="vb" AutoEventWireup="true" Inherits="AppointmentToolTip" CodeFile="AppointmentToolTip.ascx.vb" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web"	TagPrefix="dx" %>

<div runat="server" id="buttonDiv">
	<dx:ASPxButton ID="btnShowMenu" runat="server" AutoPostBack="False" AllowFocus="False">
		<Border BorderWidth="0px" />
		<Paddings Padding="0px" />
		<FocusRectPaddings Padding="4px" />
		<FocusRectBorder BorderStyle="None" BorderWidth="0px" />
	</dx:ASPxButton>
</div>    

<script type="text/javascript" id="dxss_ASPxClientAppointmentToolTip">
	ASPxClientAppointmentToolTip = ASPx.CreateClass(ASPxClientToolTipBase, {
		Initialize: function () {
			ASPxClientUtils.AttachEventToElement(this.controls.buttonDiv, "click", ASPx.CreateDelegate(this.OnButtonDivClick, this));
		},
		OnButtonDivClick: function (s, e) {
			this.ShowAppointmentMenu(s);
		},
		CanShowToolTip: function (toolTipData) {
			return this.scheduler.CanShowAppointmentMenu(toolTipData.GetAppointment());
		}
	});    
</script>