<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WebSearchDialog.ascx.cs" Inherits="WebSearchDialog" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4" Namespace="DevExpress.Web" TagPrefix="dx" %>

<dx:ASPxPopupControl ID="aspxPopupControl" runat="server" HeaderText="Search" AllowDragging="True"
  PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" AutoUpdatePosition="True" CloseAction="CloseButton" CssClass="dxxrpcSearch">
  <ContentCollection>
    <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
      <dx:ASPxPanel ID="ASPxPopupControl1" runat="server" DefaultButton="buttonFind">
        <PanelCollection>
          <dx:PanelContent ID="PanelContent1" runat="server">
            <table id="Table1" runat="server" style="width: 100%;">
              <tr>
                <td>
                  <table id="Table2" runat="server">
                    <tr>
                      <td style="width: 1%">
                        <dx:ASPxLabel ID="labelFindWhat" runat="server" Text="Find what:" AssociatedControlID="FindText" EncodeHtml="False" />
                      </td>
                      <td>
                          &nbsp;</td>
                      <td>
                        <dx:ASPxTextBox ID="textBoxFindText" runat="server" Text="" TabIndex="0" style="display: block;" />
                        <div style="height: 1px; width: 150px; border-width: 0px; overflow: hidden">
                        </div>
                      </td>
                    </tr>
                  </table>
                </td>
                <td style="width: 0px" rowspan="2">
                  &nbsp;</td>
                  <td class="dx-valt">
                    <dx:ASPxButton ID="buttonFind" runat="server" Text="Find Next" AutoPostBack="False"
                    TabIndex="4" style="display: block" EnableClientSideAPI="true" Wrap="False" EncodeHtml="False" />
                  </td>
              </tr>
              <tr>
                <td class="dx-valt">
                  <table id="Table3" runat="server" style="width: 100%;">
                    <tr>
                      <td>
                        <dx:ASPxCheckBox ID="checkMatchWholeWord" runat="server" TabIndex="1" Text="Match whole word only"
                          EncodeHtml="False" Wrap="False" />
                      </td>
                      <td rowspan="2">
                      </td>
                      <td>
                        <dx:ASPxRadioButton ID="radioUp" runat="server" TabIndex="3" Text="Up" 
                          GroupName="Direction" EncodeHtml="False" Wrap="False" />
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <dx:ASPxCheckBox ID="checkMatchCase" runat="server" TabIndex="2" Text="Match case"
                          EncodeHtml="False" Wrap="False" />
                      </td>
                      <td>
                        <dx:ASPxRadioButton ID="radioDown" runat="server" TabIndex="3" Text="Down"
                          Checked="true" GroupName="Direction" EncodeHtml="False" Wrap="False" />
                      </td>
                    </tr>
                  </table>
                </td>
                <td>
                    <dx:ASPxButton ID="buttonCancel" runat="server" Text="Cancel" AutoPostBack="False" TabIndex="5" style="display: block;" Wrap="False"/>
                </td>
              </tr>
            </table>
          </dx:PanelContent>
        </PanelCollection>
      </dx:ASPxPanel>
    </dx:PopupControlContentControl>
  </ContentCollection>
</dx:ASPxPopupControl>
