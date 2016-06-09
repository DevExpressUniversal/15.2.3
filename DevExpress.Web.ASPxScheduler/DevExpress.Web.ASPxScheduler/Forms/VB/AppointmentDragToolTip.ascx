<%@ Control Language="vb" AutoEventWireup="true" Inherits="AppointmentDragToolTip" CodeFile="AppointmentDragToolTip.ascx.vb" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web"	TagPrefix="dx" %>

<div style="white-space:nowrap;">
	<dx:ASPxLabel ID="lblInterval" runat="server" Text="CustomDragAppointmentTooltip" EnableClientSideAPI="true">
		</dx:ASPxLabel>
	<br />
	<dx:ASPxLabel ID="lblInfo" runat="server" EnableClientSideAPI="true"></dx:ASPxLabel>
</div>

<script id="dxss_ASPxClientAppointmentDragTooltip" type="text/javascript"><!--
	ASPxClientAppointmentDragTooltip = ASPx.CreateClass(ASPxClientToolTipBase, {
		CalculatePosition: function(bounds) {
			return new ASPxClientPoint(bounds.GetLeft(), bounds.GetTop() - bounds.GetHeight());
		},
		Update: function (toolTipData) {
			var stringInterval = this.GetToolTipContent(toolTipData);
			var oldText = this.controls.lblInterval.GetText();
			if (oldText != stringInterval)
				this.controls.lblInterval.SetText(stringInterval);
		},
		GetToolTipContent: function(toolTipData) {    
			var interval = toolTipData.GetInterval();
			return this.ConvertIntervalToString(interval);
		}
	});
//--></script>