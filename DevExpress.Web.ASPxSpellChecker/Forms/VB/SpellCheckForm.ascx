<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxSpellChecker.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web.ASPxSpellChecker" TagPrefix="dx" %>

<%@ Control Language="vb" AutoEventWireup="true" CodeFile="SpellCheckForm.ascx.vb" Inherits="SpellCheckForm" %>

<table id="dxMainSpellCheckFormTable" <%=DevExpress.Web.Internal.RenderUtils.GetTableSpacings(Me, 0, 0)%> class="dxwscDlgMainSpellCheckFormTable"> 
	<tr>
		<td class="dxwscDlgContentSCFormContainer">
			<table id="dxSpellCheckForm" class="dxwscDlgSpellCheckForm" <%=DevExpress.Web.Internal.RenderUtils.GetTableSpacings(Me, 0, 0)%>>
				<tr>
					<td colspan="2">
						<% =DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerLocalizer.GetString(DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerStringId.NotInDictionary)%>
					</td>
				</tr>
				<tr>
					<td class="dxwscDlgCheckedDivContainer dx-valt">
						<asp:PlaceHolder ID="SCCheckedDivPlaceHolder" runat="server" />
					</td>
					<td class="dxwscDlgButtonTableContainer dx-valt">
						<table id="topButtonsTable" class="dxwscDlgButtonTable" <%=DevExpress.Web.Internal.RenderUtils.GetTableSpacings(Me, 0, 0)%>>
							<tr>
								<td>
									<dx:ASPxButton ID="btnIgnore" runat="server" CssClass="dxwscDlgSpellCheckBtn" AutoPostback="false" UseSubmitBehavior="false">
									<ClientSideEvents Click="function(s, e) {ASPx.SCIgnore();}"/>
									</dx:ASPxButton>
								</td>
							</tr>
							<tr>
								<td class="dxwscDlgVerticalSeparator dx-valt">
									<dx:ASPxButton ID="btnIgnoreAll" runat="server" CssClass="dxwscDlgSpellCheckBtn" AutoPostback="false" UseSubmitBehavior="false">
										<ClientSideEvents Click="function(s, e) {ASPx.SCIgnoreAll();}"/>
									</dx:ASPxButton>                    
								</td>
							</tr>
<%
							   If SettingsDialogFormElemets.ShowAddToDictionaryButton Then
%>
							<tr>
								<td class="dxwscDlgVerticalSeparator dx-valt">
									<dx:ASPxButton ID="btnAddToDictionary" runat="server" CssClass="dxwscDlgSpellCheckBtn" AutoPostback="false" UseSubmitBehavior="false">
										<ClientSideEvents Click="function(s, e) {ASPx.SCAddToDictionary();}"/>
									</dx:ASPxButton>
								</td>
							</tr>
<%
							   End If
%>
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="2" class="dxwscDlgChangeToText">
						<% =DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerLocalizer.GetString(DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerStringId.ChangeTo)%>
					</td>
				</tr>
				<tr>
					<td class="dxwscDlgChangeToPanelContainer">
						<dx:ASPxPanel ID="ChangeToPanel" runat="server" Width="100%" DefaultButton="btnChange">
							<PanelCollection>
								<dx:PanelContent ID="PanelContent1" runat="server">
									<table style="width:100%" <%=DevExpress.Web.Internal.RenderUtils.GetTableSpacings(Me, 0, 0)%>>
										<tr>
											<td style="width:100%" class="dxwscDlgVerticalSeparator dx-valt">
												<dx:ASPxTextBox ID="txtChangeTo" runat="server" Width="100%" ClientInstanceName="_dxeSCTxtChangeTo">
													<ClientSideEvents 
														KeyPress="function(s, e) {ASPx.SCTextBoxKeyPress(s, e);}"
														KeyDown="function(s, e) {ASPx.SCTextBoxKeyDown(s,e);}"
													/>
												</dx:ASPxTextBox>
											</td>
										</tr>
										<tr>
											<td class="dxwscDlgListBoxContainer dx-valt">
												<dx:ASPxListBox runat="server" ID="SCSuggestionsListBox" ClientInstanceName="_dxeSCSuggestionsListBox" width="100%" Height="100px">
													<ClientSideEvents 
													ItemDoubleClick="function(s, e) {ASPx.SCListBoxItemDoubleClick(s, e);}"
													SelectedIndexChanged="function(s, e) {ASPx.SCListBoxItemChanged(s, e);}"
													/>
												</dx:ASPxListBox> 
											</td>
										</tr>
									</table>                                
								</dx:PanelContent>
							</PanelCollection>
						</dx:ASPxPanel>
					</td>
					<td class="dxwscDlgButtonTableContainer dx-valt">
						<table id="bottomButtonsTable" class="dxwscDlgButtonTable" <%=DevExpress.Web.Internal.RenderUtils.GetTableSpacings(Me, 0, 0)%>>
							<tr>
								<td class="dx-valt">
									<dx:ASPxButton ID="btnChange" runat="server" CssClass="dxwscDlgSpellCheckBtn" ClientInstanceName="_dxeSCBtnChange" AutoPostback="false" UseSubmitBehavior="false">
										<ClientSideEvents Click="function(s, e) { ASPx.SCChange();}"/>
									</dx:ASPxButton>
								</td>
							</tr>
							<tr>
								<td class="dxwscDlgVerticalSeparator dx-valt">
									<dx:ASPxButton ID="btnChangeAll" runat="server" CssClass="dxwscDlgSpellCheckBtn" ClientInstanceName="_dxeSCBtnChangeAll" AutoPostback="false" UseSubmitBehavior="false">
										<ClientSideEvents Click="function(s, e) { ASPx.SCChangeAll();}"/>
									</dx:ASPxButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			</td>
	</tr>
	<tr>
		<td class="dxwscDlgFooter">
<%
			   If SettingsDialogFormElemets.ShowOptionsButton Then
%>
				<dx:ASPxButton ID="btnOptions" runat="server" CssClass="dxwscDlgFooterBtn" AutoPostback="false" UseSubmitBehavior="false">
					<ClientSideEvents Click="function(s, e) {ASPx.SCShowOptionsForm(true);}"/>
				</dx:ASPxButton>
<%
			   End If
%>
			<dx:ASPxButton ID="btnClose" runat="server" CssClass="dxwscDlgFooterBtn" AutoPostback="false" UseSubmitBehavior="false">
				<ClientSideEvents Click="function(s, e) {ASPx.SCDialogComplete(false);}"/>
			</dx:ASPxButton>
		 </td>
	</tr>
</table>