<%@ Assembly Name="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Assembly Name="DevExpress.Data.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<%@ Assembly Name="DevExpress.Web.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<%@ Assembly Name="DevExpress.SpellChecker.v15.2.Core, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<%@ Assembly Name="DevExpress.Web.ASPxSpellChecker.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<%@ Assembly Name="DevExpress.Web.ASPxHtmlEditor.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<%@ Register Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"                    Namespace="Microsoft.SharePoint.WebControls"    TagPrefix="SharePoint" %>
<%@ Register Assembly="DevExpress.SharePoint.2013.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"        Namespace="DevExpress.SharePoint"               TagPrefix="spxhe" %>
<%@ Register Assembly="DevExpress.Web.ASPxHtmlEditor.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"     Namespace="DevExpress.Web.ASPxHtmlEditor"       TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"                    Namespace="DevExpress.Web"                      TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxSpellChecker.v15.2, Version=15.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"   Namespace="DevExpress.Web.ASPxSpellChecker"     TagPrefix="dx" %>
<SharePoint:RenderingTemplate ID="RichTextField" runat="server">
    <Template>
        <script type="text/javascript">
            function _spBodyOnLoad() {
                IsWikiPage();
            }
            function _spGetDescendantNodesByClassName(parent, className) {
                if (parent.querySelectorAll) {
                    var children = parent.querySelectorAll('.' + className);
                    return _spNodeListToArray(children);
                }
                return _spGetDescendantNodes(parent, function (elem) { return elem.className && _spElementHasCssClass(elem, className); });
            }
            function _spGetDescendantNodes(parent, predicate) {
                var c = parent.all || parent.getElementsByTagName('*');
                return _spRetrieveByPredicate(c, predicate);
            }
            function _spRetrieveByPredicate(scourceCollection, predicate) {
                var result = [];
                for (var i = 0; i < scourceCollection.length; i++) {
                    var element = scourceCollection[i];
                    if (!predicate || predicate(element))
                        result.push(element);
                }
                return result;
            }
            function _spNodeListToArray(nodeList, filter) {
                var result = [];
                for (var i = 0, element; element = nodeList[i]; i++) {
                    if (filter && !filter(element))
                        continue;
                    result.push(element);
                }
                return result;
            }
            function _spElementHasCssClass(element, className) {
                try {
                    return !!element.className.match("(^|\\s)" + className + "(\\s|$)");
                } catch (e) {
                    return false;
                }
            }
            function IsWikiPage() {
                if (!ASPx.IsExists(document.getElementById("_wikiPageMode"))) {
                    spnHtmlEditor.style.display = "";
                    spnStandartField.style.display = "none";
                    var reEditors = _spGetDescendantNodesByClassName(document.getElementById("s4-workspace"), 'ms-rtefield');
                    for (var i = 0; i < reEditors.length; i++) {
                        try {
                            if (_spElementHasCssClass(reEditors[i], 'ms-rtestate-field'))
                                reEditors[i].style.display = "none";
                        } catch (e) { }
                    }
                }
                else {
                    spnHtmlEditor.style.display = "none";
                    spnStandartField.style.display = "";
                }
            }
            function AddSPxTextField(s) {
                window.SPxTextFieldHelper = window.SPxTextField || (function () {
                    var controls = [];
                    var rlfiShowMoreSaved;
                    return {
                        OnrlfiShowMore: function () {
                            rlfiShowMoreSaved();
                            for (var i in controls)
                                controls[i].AdjustControl();
                        },
                        AddSPxTextField: function (control) {
                            if (window["rlfiShowMore"] && !rlfiShowMoreSaved) {
                                rlfiShowMoreSaved = rlfiShowMore;
                                rlfiShowMore = this.OnrlfiShowMore;
                            }
                            if (controls.indexOf(control) == -1)
                                controls.push(control);
                        }
                    };
                }());
                window.SPxTextFieldHelper.AddSPxTextField(s);
            }
        </script>
        <div id="spnHtmlEditor" style="display:none" class="ms-lines">
            <spxhe:SPxTextField ID="SPxTextField1" runat="server" Width="700px" Height="550px">
                <Toolbars>
                    <dx:HtmlEditorToolbar Name="StandardToolbar1">
                        <Items>
                            <dx:ToolbarCutButton>
                            </dx:ToolbarCutButton>
                            <dx:ToolbarCopyButton>
                            </dx:ToolbarCopyButton>
                            <dx:ToolbarPasteButton>
                            </dx:ToolbarPasteButton>
                            <dx:ToolbarUndoButton BeginGroup="True">
                            </dx:ToolbarUndoButton>
                            <dx:ToolbarRedoButton>
                            </dx:ToolbarRedoButton>
                            <dx:ToolbarRemoveFormatButton BeginGroup="True">
                            </dx:ToolbarRemoveFormatButton>
                            <dx:ToolbarSuperscriptButton BeginGroup="True">
                            </dx:ToolbarSuperscriptButton>
                            <dx:ToolbarSubscriptButton>
                            </dx:ToolbarSubscriptButton>
                            <dx:ToolbarInsertOrderedListButton BeginGroup="True">
                            </dx:ToolbarInsertOrderedListButton>
                            <dx:ToolbarInsertUnorderedListButton>
                            </dx:ToolbarInsertUnorderedListButton>
                            <dx:ToolbarIndentButton BeginGroup="True">
                            </dx:ToolbarIndentButton>
                            <dx:ToolbarOutdentButton>
                            </dx:ToolbarOutdentButton>
                            <dx:ToolbarInsertLinkDialogButton BeginGroup="True">
                            </dx:ToolbarInsertLinkDialogButton>
                            <dx:ToolbarUnlinkButton>
                            </dx:ToolbarUnlinkButton>
                            <dx:ToolbarInsertImageDialogButton>
                            </dx:ToolbarInsertImageDialogButton>
                            <dx:ToolbarCheckSpellingButton BeginGroup="True">
                            </dx:ToolbarCheckSpellingButton>
                            <dx:ToolbarTableOperationsDropDownButton BeginGroup="True">
                                <Items>
                                    <dx:ToolbarInsertTableDialogButton BeginGroup="True" ViewStyle="ImageAndText">
                                    </dx:ToolbarInsertTableDialogButton>
                                    <dx:ToolbarTablePropertiesDialogButton BeginGroup="True">
                                    </dx:ToolbarTablePropertiesDialogButton>
                                    <dx:ToolbarTableRowPropertiesDialogButton>
                                    </dx:ToolbarTableRowPropertiesDialogButton>
                                    <dx:ToolbarTableColumnPropertiesDialogButton>
                                    </dx:ToolbarTableColumnPropertiesDialogButton>
                                    <dx:ToolbarTableCellPropertiesDialogButton>
                                    </dx:ToolbarTableCellPropertiesDialogButton>
                                    <dx:ToolbarInsertTableRowAboveButton BeginGroup="True">
                                    </dx:ToolbarInsertTableRowAboveButton>
                                    <dx:ToolbarInsertTableRowBelowButton>
                                    </dx:ToolbarInsertTableRowBelowButton>
                                    <dx:ToolbarInsertTableColumnToLeftButton>
                                    </dx:ToolbarInsertTableColumnToLeftButton>
                                    <dx:ToolbarInsertTableColumnToRightButton>
                                    </dx:ToolbarInsertTableColumnToRightButton>
                                    <dx:ToolbarSplitTableCellHorizontallyButton BeginGroup="True">
                                    </dx:ToolbarSplitTableCellHorizontallyButton>
                                    <dx:ToolbarSplitTableCellVerticallyButton>
                                    </dx:ToolbarSplitTableCellVerticallyButton>
                                    <dx:ToolbarMergeTableCellRightButton>
                                    </dx:ToolbarMergeTableCellRightButton>
                                    <dx:ToolbarMergeTableCellDownButton>
                                    </dx:ToolbarMergeTableCellDownButton>
                                    <dx:ToolbarDeleteTableButton BeginGroup="True">
                                    </dx:ToolbarDeleteTableButton>
                                    <dx:ToolbarDeleteTableRowButton>
                                    </dx:ToolbarDeleteTableRowButton>
                                    <dx:ToolbarDeleteTableColumnButton>
                                    </dx:ToolbarDeleteTableColumnButton>
                                </Items>
                            </dx:ToolbarTableOperationsDropDownButton>
                        </Items>
                    </dx:HtmlEditorToolbar>
                    <dx:HtmlEditorToolbar Name="StandardToolbar2">
                        <Items>
                            <dx:ToolbarParagraphFormattingEdit Width="120px">
                                <Items>
                                    <dx:ToolbarListEditItem Text="Normal" Value="p" />
                                    <dx:ToolbarListEditItem Text="Heading  1" Value="h1" />
                                    <dx:ToolbarListEditItem Text="Heading  2" Value="h2" />
                                    <dx:ToolbarListEditItem Text="Heading  3" Value="h3" />
                                    <dx:ToolbarListEditItem Text="Heading  4" Value="h4" />
                                    <dx:ToolbarListEditItem Text="Heading  5" Value="h5" />
                                    <dx:ToolbarListEditItem Text="Heading  6" Value="h6" />
                                    <dx:ToolbarListEditItem Text="Address" Value="address" />
                                    <dx:ToolbarListEditItem Text="Normal (DIV)" Value="div" />
                                </Items>
                            </dx:ToolbarParagraphFormattingEdit>
                            <dx:ToolbarFontNameEdit>
                                <Items>
                                    <dx:ToolbarListEditItem Text="Times New Roman" Value="Times New Roman" />
                                    <dx:ToolbarListEditItem Text="Tahoma" Value="Tahoma" />
                                    <dx:ToolbarListEditItem Text="Verdana" Value="Verdana" />
                                    <dx:ToolbarListEditItem Text="Arial" Value="Arial" />
                                    <dx:ToolbarListEditItem Text="MS Sans Serif" Value="MS Sans Serif" />
                                    <dx:ToolbarListEditItem Text="Courier" Value="Courier" />
                                </Items>
                            </dx:ToolbarFontNameEdit>
                            <dx:ToolbarFontSizeEdit>
                                <Items>
                                    <dx:ToolbarListEditItem Text="1 (8pt)" Value="1" />
                                    <dx:ToolbarListEditItem Text="2 (10pt)" Value="2" />
                                    <dx:ToolbarListEditItem Text="3 (12pt)" Value="3" />
                                    <dx:ToolbarListEditItem Text="4 (14pt)" Value="4" />
                                    <dx:ToolbarListEditItem Text="5 (18pt)" Value="5" />
                                    <dx:ToolbarListEditItem Text="6 (24pt)" Value="6" />
                                    <dx:ToolbarListEditItem Text="7 (36pt)" Value="7" />
                                </Items>
                            </dx:ToolbarFontSizeEdit>
                            <dx:ToolbarBoldButton BeginGroup="True">
                            </dx:ToolbarBoldButton>
                            <dx:ToolbarItalicButton>
                            </dx:ToolbarItalicButton>
                            <dx:ToolbarUnderlineButton>
                            </dx:ToolbarUnderlineButton>
                            <dx:ToolbarStrikethroughButton>
                            </dx:ToolbarStrikethroughButton>
                            <dx:ToolbarJustifyLeftButton BeginGroup="True">
                            </dx:ToolbarJustifyLeftButton>
                            <dx:ToolbarJustifyCenterButton>
                            </dx:ToolbarJustifyCenterButton>
                            <dx:ToolbarJustifyRightButton>
                            </dx:ToolbarJustifyRightButton>
                            <dx:ToolbarJustifyFullButton>
                            </dx:ToolbarJustifyFullButton>
                            <dx:ToolbarBackColorButton BeginGroup="True">
                            </dx:ToolbarBackColorButton>
                            <dx:ToolbarFontColorButton>
                            </dx:ToolbarFontColorButton>
                        </Items>
                    </dx:HtmlEditorToolbar>
                </Toolbars>
                <ClientSideEvents Init="function(s, e){ AddSPxTextField(s); }" />
            </spxhe:SPxTextField>
        </div>
        <span id='spnStandartField' style="display:none">
            <span dir="<%$Resources:wss,multipages_direction_dir_value%>" id="nativeEditorContainer" runat="server">
                <asp:TextBox ID="TextField" TextMode="MultiLine" runat="server" />
                <input id="TextField_spSave" type="HIDDEN" name="TextField_spSave" runat="server" />
            </span>
        </span>
    </Template>
</SharePoint:RenderingTemplate>
