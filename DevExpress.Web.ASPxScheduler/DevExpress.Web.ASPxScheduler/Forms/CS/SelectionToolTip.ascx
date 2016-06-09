<%@ Control Language="C#" AutoEventWireup="true" Inherits="SelectionToolTip" CodeFile="SelectionToolTip.ascx.cs" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web" TagPrefix="dx" %>

<div runat="server" id="buttonDiv">
    <dx:ASPxButton ID="btnShowMenu" runat="server" AutoPostBack="False" AllowFocus="False">
        <Border BorderWidth="0px" />
        <Paddings Padding="0px" />
        <FocusRectPaddings Padding="4px" />
        <FocusRectBorder BorderStyle="None" BorderWidth="0px" />
    </dx:ASPxButton>
</div>