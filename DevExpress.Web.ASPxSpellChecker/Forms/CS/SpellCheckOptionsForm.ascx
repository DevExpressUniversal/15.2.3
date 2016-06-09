<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SpellCheckOptionsForm.ascx.cs" Inherits="SpellCheckOptionsForm" %>

<table id="dxMainSpellCheckOptionsFormTable" <%= DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) %> class="dxwscDlgMainSpellCheckOptionsFormTable">
    <tr>
        <td class="dxwscDlgContentSCOptionsFormContainer">
            <table id="dxOptionsForm" class="dxwscDlgOptionsForm" <%= DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) %> style="width:100%">
                <tr>
                    <td>
                        <dx:ASPxRoundPanel ID="pnlOptions" runat="server" Width="100%">
                            <PanelCollection>
                                <dx:PanelContent runat="server">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxCheckBox id="chkbUpperCase" ClientInstanceName="chkbUpperCase" runat="server"></dx:ASPxCheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dx:ASPxCheckBox id="chkbMixedCase" ClientInstanceName="chkbMixedCase" runat="server"></dx:ASPxCheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dx:ASPxCheckBox id="chkbNumbers" ClientInstanceName="chkbNumbers" runat="server"></dx:ASPxCheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dx:ASPxCheckBox id="chkbEmails" ClientInstanceName="chkbEmails" runat="server"></dx:ASPxCheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dx:ASPxCheckBox id="chkbUrls" ClientInstanceName="chkbUrls" runat="server"></dx:ASPxCheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                               <dx:ASPxCheckBox id="chkbTags" ClientInstanceName="chkbTags" runat="server"></dx:ASPxCheckBox>
                                            </td>
                                        </tr>
                                    </table>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxRoundPanel>
                    </td>
                </tr>
                <tr>
                    <td class="dxwscDlgLanguagePanel">
                        <dx:ASPxRoundPanel ID="pnlLanguageSelection" runat="server" Width="100%">
                            <PanelCollection>
                                <dx:PanelContent runat="server">
                                    <table style="width:100%;">
                                        <tr>
                                            <td colspan="2">
                                                <dx:ASPxLabel ID="lblChooseDictionary" runat="server"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="dxwscDlgLanguageLabelCell">
                                                <dx:ASPxLabel ID="lblLanguage" runat="server" AssociatedControlID="comboLanguage"></dx:ASPxLabel>
                                            </td>
                                            <td class="dxwscDlgLanguageComboCell">
                                                <dx:ASPxComboBox ID="comboLanguage" ClientInstanceName="comboLanguage" runat="server" Width="100%"></dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxRoundPanel>
                    </td>
                </tr>  
            </table>        
        </td>
    </tr>
    <tr>
        <td class="dxwscDlgFooter">
            <dx:ASPxButton id="btnOK" runat="server" AutoPostBack="false" CssClass="dxwscDlgFooterBtn" UseSubmitBehavior="false">
                <ClientSideEvents Click="function(s, e) {ASPx.SCDialogComplete(true)}"/>
            </dx:ASPxButton>
            <dx:ASPxButton id="btnCancel" runat="server" AutoPostBack="false" CssClass="dxwscDlgFooterBtn" UseSubmitBehavior="false">
                <ClientSideEvents Click="function(s, e) {ASPx.SCDialogComplete(false)}"/>                        
            </dx:ASPxButton>
        </td>
    </tr>
</table>

