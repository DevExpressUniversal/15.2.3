/// <reference path="..\..\DevExpress.Web\Scripts\_references.js"/>
/// <reference path="..\..\DevExpress.Web\Scripts\DialogUtils.js"/>
/// <reference path="Generated\Scripts\HtmlImport.js"/>
/// <reference path="Generated\Scripts\Commands\ShowFontDialogCommand.js"/>
/// <reference path="Generated\Scripts\Commands\ShowParagraphDialogCommand.js"/>
/// <reference path="Generated\Scripts\Commands\ShowOpenFileDialogCommand.js"/>
(function() {
    var constants = {
        SaveImageToServerCallbackPrefix: "REITS",
        SaveImageToServerErrorCallbackPrefix: "REISE",
        SaveImageToServerNewUrlCallbackPrefix: "REISU",
        ColumnsEditorCallbackPrefix: "RECE",
        SymbolListCallbackPrefix: "RESL",

        PreviewTextElementID: "_dxInsertImagePreviewText",
        PreviewImageElementID: "_dxInsertImagePreviewImage",

        TableBorderPreviewElementID: "_borderContainerPreview",

        WidthSpinElementName: "dxreDialog_SpnColumnsWidth",
        SpacingSpinElementName: "dxreDialog_SpnColumnsSpacing",
        ColumnsPresetElementName: "dxreDialog_BtnPreset",

        UrlAreaElementID: "_dxreUrlArea",
        EmailToAreaElementID: "_dxreEmailToArea",
        EmailSubjectAreaElementID: "_dxreEmailSubjectArea",
        
        AbstractNumberingListElementID: "_dxeAbstractNumberingList_",
        BulletedPresetElementName: "dxreDialog_BulletedPreset",

        DefaultLinkPrefix : "http://",
        MailToPrefex : "mailto:",
        SubjectPrefix : "?subject=",

        DialogListBoxClass: "dxreDlgListBox",
        ListSelectedClass: "dxreDlgListSelected",
        ListHoverClass: "dxreDlgListHover",
        NumberingListPreviewClass: "dxreDlgNumberingListPreview",

        BorderLineClass: "dxreDlgBorderLine",
        BorderVerticalLineClass: "dxreDlgVerticalLine",
        BorderHorizontalLineClass: "dxreDlgHorizontalLine",
        BorderControlLineClass: "dxreDlgBorderControlLine",
        BorderTopLineClass: "dxreDlgTopLine",
        BorderMiddleLineClass: "dxreDlgMiddleLine",
        BorderBottomLineClass: "dxreDlgBottomLine",
        BorderLeftLineClass: "dxreDlgLeftLine",
        BorderCenterLineClass: "dxreDlgCenterLine",
        BorderRightLineClass: "dxreDlgRightLine"
    };

    var ASPxRichEditDialog = ASPx.CreateClass(ASPx.Dialog, {
        Execute: function(ownerControl, parameters, callback, afterClosing, isModal) {
            this.richedit = ownerControl;
            this.callback = callback;
            this.parameters = parameters;
            this.afterClosing = afterClosing;
            this.SetDialogNameInput();
            this.isOnCallbackError = false;
            this.isModal = isModal;
            ASPx.Dialog.prototype.Execute.call(this, ownerControl);
        },
        OnCallbackError: function(result, data) {
            this.isOnCallbackError = true;
            this.ClearDialogNameInput();
            this.HideDialog(null, true);
            ASPx.Dialog.prototype.OnCallbackError.call(this, result);
        },
        OnClose: function() {
            if(this.richedit.isInFullScreenMode)
                this.richedit.hideBodyScroll();
            ASPx.Dialog.prototype.OnClose.call(this);
            this.ClearDialogNameInput();
            if(this.afterClosing)
                this.afterClosing();
        },

        OnComplete: function(result, params) {
            this.DoCustomAction(result, params);
            this.HideDialogPopup();
            this.ClearEditorValue();
        },
        OnInitComplete: function() {
            ASPx.Dialog.prototype.OnInitComplete.call(this);
            this.attachEvents();
            this.GetDialogPopup().UpdatePosition();
            this.RestoreEditorsState();
            var popupElement = this.GetDialogPopup().GetWindowElement(-1);
            if(popupElement && popupElement.style.width)
                ASPx.Attr.RemoveAttribute(popupElement.style, "width");
            if(!this.isModal) {
                this.GetDialogPopup().SetClientModality(false);
                this.richedit.core.captureFocus();
            }
        },

        onOkButtonClick: function() {
            if(this.isDialogValid())
                this.OnComplete(1);
        },
        onCancelButtonClick: function() {
            this.OnComplete(0);
        },

        SaveEditorsState: function() {
        },
        ClearEditorValue: function() {
        },
        RestoreEditorsState: function() {
        },

        ClearDialogNameInput: function() {
            this.richedit.UpdateStateObjectWithObject({ currentDialog: "" });
        },
        SetDialogNameInput: function() {
            this.richedit.UpdateStateObjectWithObject({ currentDialog: this.name });
        },

        SendCallback: function(callbackArgs) {
            this.richedit.createCallbackCore(callbackArgs, this);
            this.ShowLoadingPanelOverDialogPopup();
        },
        DoCustomAction: function(result) {
            if(result)
                this.callback(this.GetResultParameters());
            else
                this.callback(null);
        },
        ShowLoadingPanelOverDialogPopup: function() {
            var offsetElement = ASPx.GetParentByClassName(this.GetDialogPopup().GetWindowContentElement(-1), "dxreControlSys");
            this.richedit.CreateLoadingDiv(document.body, offsetElement);
            this.richedit.CreateLoadingPanelWithAbsolutePosition(document.body, offsetElement);
        },
        SendCallbackForDialogContent: function() {
            this.richedit.sendInternalServiceCallback(ASPx.dialogFormCallbackStatus, this.name, this);
        },
        GetInitInfoObject: function() {
            return this.parameters;
        },
        GetResultParameters: function() {
            return {};
        },
        toggleFullScreen: function() {
            this.richedit.SetFullscreenMode(!this.richedit.isInFullScreenMode);
            this.GetDialogPopup().UpdatePosition();
        },
        SetFocusInField: function() {
            var focusedElement = this.GetFocusedElement();
            if(focusedElement)
                focusedElement.SetFocus();
        },
        GetFocusedElement: function() {
            return null;
        },
        attachEvents: function() {
            executeIfExists("dxreDialog_BtnOk", function(element) {
                element.Click.AddHandler(this.onOkButtonClick.aspxBind(this));
            }.aspxBind(this));
            executeIfExists("dxreDialog_BtnCancel", function(element) {
                element.Click.AddHandler(this.onCancelButtonClick.aspxBind(this));
            }.aspxBind(this));
        },
        isDialogValid: function() {
            return true;
        }
    });

    var RESaveAsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        OnInitComplete: function() {
            ASPxRichEditDialog.prototype.OnInitComplete.apply(this, arguments);
            if(typeof(ASPxClientRichEditFolderManager) != "undefined")
                ASPxClientRichEditFolderManager.SetOwner(this.GetDialogPopup().GetContentContainer(-1), this.richedit);
            if(typeof(FileManager) != "undefined")
                setTimeout(function() { FileManager.Refresh(); }, 0);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            if(this.isFileSavedToServer()) {
                returnedObject.fileSavedToServer = this.isFileSavedToServer();
                returnedObject.folderPath = normolizeVirtualFolderPath(FileManager.GetCurrentFolderPath("\\", true));
                returnedObject.fileName = dxreDialog_TbxFileName.GetText();
                returnedObject.fileExtension = dxreDialog_CbxFileType.GetValue();
            }
            else {
                returnedObject.fileSavedToServer = false;
                returnedObject.folderPath = undefined;
                returnedObject.fileName = undefined;
                returnedObject.fileExtension = dxreDialog_CbxDownloadFileType.GetValue();
            }
            return returnedObject;
        },
        InitializeDialogFields: function(parameters) {
            this.toggleElements();
            if(typeof(FileManager) != "undefined") {
                parameters.folderPath = FileManager.GetCurrentFolderPath() + "\\" + parameters.folderPath;
                dxreDialog_TbxFolderPath.SetText(parameters.folderPath);
                dxreDialog_TbxFileName.SetText(parameters.fileName);
                this.InitializeFileExtensionComboBox(dxreDialog_CbxFileType, parameters.fileExtension);
            }
            if(typeof(dxreDialog_CbxDownloadFileType) != "undefined")
                this.InitializeFileExtensionComboBox(dxreDialog_CbxDownloadFileType, parameters.fileExtension);
        },
        InitializeFileExtensionComboBox: function(combobox, fileExtension) {
            var fileExtensionItem = combobox.FindItemByValue(fileExtension);
            if(fileExtensionItem)
                combobox.SetSelectedItem(fileExtensionItem);
            else
                combobox.SetSelectedIndex(0);
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            executeIfExists("dxreDialog_RblNavigation", function(element) {
                element.ValueChanged.AddHandler(this.toggleElements.aspxBind(this));
            }.aspxBind(this));
            executeIfExists("dxreDialog_TbxFolderPath", function(element) {
                element.ButtonClick.AddHandler(this.OnBrowserButtonClick.aspxBind(this));
            }.aspxBind(this));
            executeIfExists("BrowsePopup", function(element) {
                element.PopUp.AddHandler(this.OnBrowserPopupOpen.aspxBind(this));
                element.CloseUp.AddHandler(this.OnBrowserPopupClose.aspxBind(this));
            }.aspxBind(this));
            executeIfExists("dxreDialog_BtnSelect", function(element) {
                element.Click.AddHandler(this.SelectFolderForm_FolderSelected.aspxBind(this));
            }.aspxBind(this));
            executeIfExists("dxreDialog_BtnCancleSelect", function(element) {
                element.Click.AddHandler(BrowsePopup.Hide.aspxBind(BrowsePopup));
            }.aspxBind(this));
            executeIfExists("FileManager", function(element) {
                element.SelectedFileOpened.AddHandler(this.SelectFolderForm_FolderSelected.aspxBind(this));
            }.aspxBind(this));
            dxreDialog_BtnDownload.Click.AddHandler(this.onOkButtonClick.aspxBind(this));
        },
        GetFocusedElement: function() {
            return this.isFileSavedToServer() ? dxreDialog_TbxFileName : dxreDialog_CbxDownloadFileType;
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.SaveAsFile;
        },
        toggleElements: function() {
            var folderPath = dxreDialog_SaveFileContent.GetItemByName("TbxFolderPath");
            var fileName = dxreDialog_SaveFileContent.GetItemByName("TbxFileName");
            var fileType = dxreDialog_SaveFileContent.GetItemByName("CbxFileType");
            var downloadFileType = dxreDialog_SaveFileContent.GetItemByName("CbxDownloadFileType");
            isFileSavedToServer = this.isFileSavedToServer();

            if(downloadFileType)
                downloadFileType.SetVisible(!isFileSavedToServer);
            if(folderPath && fileName && fileType) {
                folderPath.SetVisible(isFileSavedToServer);
                fileName.SetVisible(isFileSavedToServer);
                fileType.SetVisible(isFileSavedToServer);
            }
            dxreDialog_BtnOk.SetVisible(isFileSavedToServer);
            dxreDialog_BtnDownload.SetVisible(!isFileSavedToServer);
        },
        OnBrowserButtonClick: function() {
            FileManager.browsePopup = BrowsePopup;
            BrowsePopup.Show();
            FileManager.Refresh();
            FileManager.Focus();
        },
        OnBrowserPopupOpen: function() {
            ASPx.Attr.ChangeAttribute(dxreDialog_LayoutWrapper.mainElement, "onkeypress", "");
        },
        OnBrowserPopupClose: function() {
            ASPx.Attr.RestoreAttribute(dxreDialog_LayoutWrapper.mainElement, "onkeypress");
            dxreDialog_TbxFolderPath.Focus();
        },
        SelectFolderForm_FolderSelected: function() {
            BrowsePopup.Hide();
            var documentUrl = FileManager.GetCurrentFolderPath() + "\\";
            dxreDialog_TbxFolderPath.SetText(documentUrl);
            if(FileManager.GetSelectedFile()) {
                documentUrl = documentUrl + FileManager.GetSelectedFile().name;
                var fileName = FileManager.GetSelectedFile().name.substr(0, FileManager.GetSelectedFile().name.lastIndexOf("."));
                var fileExt = FileManager.GetSelectedFile().name.substr(FileManager.GetSelectedFile().name.lastIndexOf("."), FileManager.GetSelectedFile().name.length - FileManager.GetSelectedFile().name.lastIndexOf("."));
                var comboBoxExtItem = dxreDialog_CbxFileType.FindItemByValue(fileExt);
                if(comboBoxExtItem) {
                    dxreDialog_CbxFileType.SetSelectedItem(comboBoxExtItem);
                }
                dxreDialog_TbxFileName.SetText(fileName);
            }
        },
        isFileSavedToServer: function() {
            if(typeof(dxreDialog_RblNavigation) != "undefined")
                return dxreDialog_RblNavigation.GetValue() ? false : true;
            else
                return typeof(dxreDialog_TbxFileName) != "undefined";
        },
        isDialogValid: function() {
            return this.isValidFields_SaveFileForm() && this.checkAndConfirmToRewriteExistingFile();
        },
        isValidFields_SaveFileForm: function() {
            return (typeof(dxreDialog_TbxFolderPath) != "undefined" && dxreDialog_TbxFolderPath.IsVisible()) ? ASPxClientEdit.ValidateGroup("_dxeTbxSaveFilePathGroup") : true;
        },
        checkAndConfirmToRewriteExistingFile: function() {
            return this.isSavedFileExist() ? confirm(dxreDialog_TbxFileName.GetText() + dxreDialog_CbxFileType.GetValue() + " " + "already exist. Do you want to replace it?") : true;
        },
        isSavedFileExist: function() {
            if(!this.isFileSavedToServer())
                return false;
            var fileExist = false;
            var foldedrPath = dxreDialog_TbxFolderPath.GetText() == "\\" ? "" : dxreDialog_TbxFolderPath.GetText();
            var fileName = dxreDialog_TbxFileName.GetText();
            var fileExtension = dxreDialog_CbxFileType.GetValue();
            if(typeof(FileManager) != "undefined") {
                FileManager.SetCurrentFolderPath(foldedrPath);
                var itemCollection = FileManager.GetItems();
                if(itemCollection) {
                    for(var i = 0; i < itemCollection.length; i++) {
                        if(itemCollection[i].name == (fileName + fileExtension)) {
                            fileExist = true;
                            break;
                        }
                    }
                }
            }
            return fileExist;
        }
    });

    var REOpenFileDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        OnInitComplete: function() {
            ASPxRichEditDialog.prototype.OnInitComplete.apply(this, arguments);
            if(typeof(ASPxClientRichEditFileManager) != "undefined")
                ASPxClientRichEditFileManager.SetOwner(this.GetDialogPopup().GetContentContainer(-1), this.richedit);
            if(FileManager)
                setTimeout(function() { FileManager.Refresh(); }, 0);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.src = normolizeVirtualFolderPath(FileManager.GetCurrentFolderPath("\\", true)) + FileManager.GetSelectedFile().name;
            return returnedObject;
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.OpenFile;
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            FileManager.SelectedFileOpened.AddHandler(function(s, e) {
                this.onOkButtonClick();
            }.aspxBind(this));
            FileManager.SelectedFileChanged.AddHandler(function(s, e) {
                dxreDialog_BtnOk.SetEnabled(!!e.file);
            }.aspxBind(this));
        }
    });

    var REFontDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Font;
        },
        InitializeDialogFields: function(parameters) {
            /// <param name="parameters" type="__aspxRichEdit.FontDialogParameters"></param>
            dxreDialog_CbxFontName.SetText(parameters.fontName);
            dxreDialog_CbxFontStyle.SetValue(parameters.fontStyle);
            dxreDialog_CbxFontSize.SetValue(parameters.fontSize < 0 ? null : parameters.fontSize);
            dxreDialog_CeFontColor.SetValue(parameters.fontColor);
            dxreDialog_CbxUnderlineStyle.SetValue(parameters.fontUnderlineType);
            dxreDialog_RblStrikeout.SetValue(parameters.fontStrikeoutType);
            dxreDialog_CbUnderlineWordsOnly.SetValue(parameters.underlineWordsOnly);
            dxreDialog_RblSubscript.SetValue(parameters.script);
            dxreDialog_ChkAllCaps.SetValue(parameters.allCaps);
            dxreDialog_ChkHidden.SetValue(parameters.hidden);
            dxreDialog_CeUnderlineColor.SetValue(parameters.fontUnderlineColor);
        },
        GetFocusedElement: function() {
            return dxreDialog_CbxFontName;
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.fontName = dxreDialog_CbxFontName.GetValue();
            returnedObject.fontStyle = dxreDialog_CbxFontStyle.GetValue();
            returnedObject.fontSize = dxreDialog_CbxFontSize.GetValue() === null ? -1 : dxreDialog_CbxFontSize.GetValue();
            returnedObject.fontColor = dxreDialog_CeFontColor.GetValue();
            returnedObject.fontUnderlineType = dxreDialog_CbxUnderlineStyle.GetValue();
            returnedObject.fontStrikeoutType = dxreDialog_RblStrikeout.GetValue();
            returnedObject.underlineWordsOnly = dxreDialog_CbUnderlineWordsOnly.GetValue();
            returnedObject.script = dxreDialog_RblSubscript.GetValue();
            returnedObject.allCaps = dxreDialog_ChkAllCaps.GetValue();
            returnedObject.hidden = dxreDialog_ChkHidden.GetValue();
            returnedObject.fontUnderlineColor = dxreDialog_CeUnderlineColor.GetValue();
            return returnedObject;
        }
    });

    var REParagraphDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        defaultFormatString: "",
        lineSpacing: 0,
        lineSpacingMultiple: 0,
        pageWidth: 0,
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Paragraph;
        },
        InitializeDialogFields: function(parameters) {
            /// <param name="parameters" type="__aspxRichEdit.ParagraphDialogParameters"></param>
            dxreDialog_CbxAlign.SetValue(parameters.alignment);
            dxreDialog_CbxOutlineLevel.SetValue(parameters.outlineLevel);
            dxreDialog_SpnLeft.SetValue(parameters.leftIndent);
            dxreDialog_SpnRight.SetValue(parameters.rightIndent);
            dxreDialog_CbxSpecial.SetValue(parameters.firstLineIndentType);
            dxreDialog_SpnBy.SetValue(parameters.firstLineIndent);
            dxreDialog_SpnBefore.SetValue(parameters.spacingBefore);
            dxreDialog_SpnAfter.SetValue(parameters.spacingAfter);
            dxreDialog_CbxLineSpacing.SetValue(parameters.lineSpacingType);
            dxreDialog_ChkNoSpace.SetValue(parameters.contextualSpacing);
            dxreDialog_ChkKLT.SetValue(parameters.keepLinesTogether);
            dxreDialog_ChkPBB.SetValue(parameters.pageBreakBefore);


            this.lineSpacing = parameters.lineSpacing;
            this.lineSpacingMultiple = parameters.lineSpacingMultiple;
            this.defaultFormatString = dxreDialog_SpnAt.displayFormat;
            this.pageWidth = parameters.pageWidth;

            switch(parameters.lineSpacingType) {
                case __aspxRichEdit.ParagraphLineSpacingType.Single:
                case __aspxRichEdit.ParagraphLineSpacingType.Sesquialteral:
                case __aspxRichEdit.ParagraphLineSpacingType.Double:
                    dxreDialog_SpnAt.displayFormat = null;
                    dxreDialog_SpnAt.SetValue(null);
                    break;
                case __aspxRichEdit.ParagraphLineSpacingType.Multiple:
                    dxreDialog_SpnAt.displayFormat = null;
                    dxreDialog_SpnAt.inc = 0.5;
                    dxreDialog_SpnAt.SetMinValue(1);
                    dxreDialog_SpnAt.SetValue(this.lineSpacingMultiple);
                    break;
                default:
                    dxreDialog_SpnAt.SetMinValue(0);
                    dxreDialog_SpnAt.inc = 0.01;
                    dxreDialog_SpnAt.SetValue(this.lineSpacing);
                    break;
            }
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_CbxLineSpacing.ValueChanged.AddHandler(function(s, e) {
                this.OnLineSpacingTypeValueChanged();
            }.aspxBind(this));
            dxreDialog_SpnAt.ValueChanged.AddHandler(function(s, e) {
                this.OnLineSpacingValueChanged();
            }.aspxBind(this));
            dxreDialog_BtnTabs.Click.AddHandler(function(s, e) {
                this.ShowTabsDialog();
            }.aspxBind(this));
        },
        GetFocusedElement: function() {
            return dxreDialog_CbxAlign;
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.alignment = dxreDialog_CbxAlign.GetValue();
            returnedObject.outlineLevel = dxreDialog_CbxOutlineLevel.GetValue();
            returnedObject.leftIndent = dxreDialog_SpnLeft.GetValue();
            returnedObject.rightIndent = dxreDialog_SpnRight.GetValue();
            returnedObject.firstLineIndentType = dxreDialog_CbxSpecial.GetValue();
            returnedObject.firstLineIndent = dxreDialog_SpnBy.GetValue();
            returnedObject.spacingBefore = dxreDialog_SpnBefore.GetValue();
            returnedObject.spacingAfter = dxreDialog_SpnAfter.GetValue();
            returnedObject.lineSpacingType = dxreDialog_CbxLineSpacing.GetValue();
            returnedObject.lineSpacing = dxreDialog_SpnAt.GetValue();
            returnedObject.lineSpacingMultiple = dxreDialog_SpnAt.GetValue();
            returnedObject.contextualSpacing = dxreDialog_ChkNoSpace.GetValue();
            returnedObject.keepLinesTogether = dxreDialog_ChkKLT.GetValue();
            returnedObject.pageBreakBefore = dxreDialog_ChkPBB.GetValue();
            return returnedObject;
        },
        ShowTabsDialog: function() {
            this.OnComplete(0);
            this.richedit.core.commandManager.getCommand(__aspxRichEdit.RichEditClientCommand.ShowTabsForm).execute();
        },
        OnLineSpacingTypeValueChanged: function() {
            switch(dxreDialog_CbxLineSpacing.GetValue()) {
                case __aspxRichEdit.ParagraphLineSpacingType.Single:
                case __aspxRichEdit.ParagraphLineSpacingType.Sesquialteral:
                case __aspxRichEdit.ParagraphLineSpacingType.Double:
                    dxreDialog_SpnAt.displayFormat = null;
                    dxreDialog_SpnAt.SetValue(null);
                    break;
                case __aspxRichEdit.ParagraphLineSpacingType.Multiple:
                    dxreDialog_SpnAt.displayFormat = null;
                    dxreDialog_SpnAt.inc = 0.5;
                    dxreDialog_SpnAt.SetMinValue(0.5);
                    dxreDialog_SpnAt.SetValue(this.lineSpacingMultiple);
                    break;
                default:
                    dxreDialog_SpnAt.displayFormat = this.defaultFormatString;
                    dxreDialog_SpnAt.inc = 0.01;
                    dxreDialog_SpnAt.SetMinValue(0);
                    dxreDialog_SpnAt.SetValue(this.lineSpacing);
                    break;
            }
        },
        OnLineSpacingValueChanged: function() {
            var type = dxreDialog_CbxLineSpacing.GetValue();
            if(type === __aspxRichEdit.ParagraphLineSpacingType.Single || type === __aspxRichEdit.ParagraphLineSpacingType.Sesquialteral || type === __aspxRichEdit.ParagraphLineSpacingType.Double)
                dxreDialog_CbxLineSpacing.SetValue(__aspxRichEdit.ParagraphLineSpacingType.Multiple);
        },
        onOkButtonClick: function() {
            var leftIndent = dxreDialog_SpnLeft.GetValue();
            if(dxreDialog_CbxSpecial.GetValue() === __aspxRichEdit.ParagraphFirstLineIndent.Indented)
                leftIndent += dxreDialog_SpnBy.GetValue();
            if(dxreDialog_SpnRight.GetValue() + leftIndent >= this.pageWidth)
                alert("The indent size is too large");
            else
                this.OnComplete(1);
        }
    });

    var REPageSetupDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.PageSetup;
        },
        InitializeDialogFields: function(parameters) {
            /// <param name="parameters" type="__aspxRichEdit.PageSetuphDialogParameters"></param>
            dxreDialog_SpnTop.SetValue(parameters.marginTop);
            dxreDialog_SpnBottom.SetValue(parameters.marginBottom);
            dxreDialog_SpnLeft.SetValue(parameters.marginLeft);
            dxreDialog_SpnRight.SetValue(parameters.marginRight);
            if(parameters.landscape === true)
                dxreDialog_RblOrientation.SetValue(1);
            else if(parameters.landscape === false)
                dxreDialog_RblOrientation.SetValue(0);

            dxreDialog_SpnWidth.SetValue(parameters.pageWidth);
            dxreDialog_SpnHeight.SetValue(parameters.pageHeight);
            if(parameters.pageWidth && parameters.pageHeight)
                this.OnPaperSizeChanged();

            dxreDialog_CbxSectionStart.SetValue(parameters.startType);
            dxreDialog_ChkDifferentOddAndEven.SetValue(parameters.headerDifferentOddAndEven);
            dxreDialog_ChkDifferentFirstPage.SetValue(parameters.headerDifferentFirstPage);

            dxreDialog_CbxApplyTo.SetValue(parameters.applyTo);

            dxreDialog_PageSetupPageControl.SetActiveTab(dxreDialog_PageSetupPageControl.GetTabByName(parameters.initialTab));
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_CbxPaperSize.ValueChanged.AddHandler(function(s, e) {
                this.OnPaperKindChanged();
            }.aspxBind(this));
            dxreDialog_SpnWidth.ValueChanged.AddHandler(function(s, e) {
                this.OnPaperSizeChanged();
            }.aspxBind(this));
            dxreDialog_SpnHeight.ValueChanged.AddHandler(function(s, e) {
                this.OnPaperSizeChanged();
            }.aspxBind(this));
            dxreDialog_RblOrientation.ValueChanged.AddHandler(function(s, e) {
                this.OnOrientationChanged();
            }.aspxBind(this));
        },
        GetFocusedElement: function() {
            return dxreDialog_SpnTop;
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            var orientation = dxreDialog_RblOrientation.GetValue();
            var landscape = undefined;
            if(orientation === 1)
                landscape = true;
            else if(orientation === 0)
                landscape = false;
            var marginTop = dxreDialog_SpnTop.GetValue();
            var marginBottom = dxreDialog_SpnBottom.GetValue();
            var marginLeft = dxreDialog_SpnLeft.GetValue();
            var marginRight = dxreDialog_SpnRight.GetValue();

            var pageWidth = dxreDialog_SpnWidth.GetValue();
            var pageHeight = dxreDialog_SpnHeight.GetValue();
            var kind = dxreDialog_CbxPaperSize.GetValue();
            if(kind > 0) {
                var size = __aspxRichEdit.PaperSizeConverter.calculatePaperSize(kind);
                var isNeedRotation = pageWidth > pageHeight && size.width < size.height;
                if(!isNeedRotation) {
                    pageWidth = this.richedit.core.units.twipsToUI(size.width);
                    pageHeight = this.richedit.core.units.twipsToUI(size.height);
                } else {
                    pageWidth = this.richedit.core.units.twipsToUI(size.height);
                    pageHeight = this.richedit.core.units.twipsToUI(size.width);
                }
            }

            returnedObject.marginTop = marginTop == null ? undefined : marginTop;
            returnedObject.marginBottom = marginBottom == null ? undefined : marginBottom;
            returnedObject.marginLeft = marginLeft == null ? undefined : marginLeft;
            returnedObject.marginRight = marginRight == null ? undefined : marginRight;
            returnedObject.landscape = landscape;

            returnedObject.pageWidth = pageWidth == null ? undefined : pageWidth;
            returnedObject.pageHeight = pageHeight == null ? undefined : pageHeight;

            returnedObject.startType = dxreDialog_CbxSectionStart.GetValue();
            returnedObject.headerDifferentOddAndEven = dxreDialog_ChkDifferentOddAndEven.GetValue();
            returnedObject.headerDifferentFirstPage = dxreDialog_ChkDifferentFirstPage.GetValue();

            returnedObject.applyTo = dxreDialog_CbxApplyTo.GetValue();

            return returnedObject;
        },
        OnPaperKindChanged: function() {
            var kind = dxreDialog_CbxPaperSize.GetValue();
            var size = __aspxRichEdit.PaperSizeConverter.calculatePaperSize(kind);
            dxreDialog_SpnWidth.SetValue(this.richedit.core.units.twipsToUI(size.width));
            dxreDialog_SpnHeight.SetValue(this.richedit.core.units.twipsToUI(size.height));
            this.updateOrientation(size.width > size.height);
        },
        OnPaperSizeChanged: function() {
            this.updatePaperKind(dxreDialog_SpnWidth.GetValue(), dxreDialog_SpnHeight.GetValue());
            this.updateOrientation(dxreDialog_SpnWidth.GetValue() > dxreDialog_SpnHeight.GetValue());
        },
        OnOrientationChanged: function() {
            var landscape = dxreDialog_RblOrientation.GetValue() === 1;
            var width = dxreDialog_SpnWidth.GetValue();
            var height = dxreDialog_SpnHeight.GetValue();
            if(landscape) {
                dxreDialog_SpnWidth.SetValue(Math.max(width, height));
                dxreDialog_SpnHeight.SetValue(Math.min(width, height));
                this.updatePaperKind(Math.max(width, height), Math.min(width, height));
            }
            else {
                dxreDialog_SpnWidth.SetValue(Math.min(width, height));
                dxreDialog_SpnHeight.SetValue(Math.max(width, height));
                this.updatePaperKind(Math.min(width, height), Math.max(width, height));
            }

        },
        updatePaperKind: function(width, height) {
            var size = { width: this.richedit.core.units.UIToTwips(width), height: this.richedit.core.units.UIToTwips(height) };
            var paperKind = __aspxRichEdit.PaperSizeConverter.calculatePaperKind(size, 0, this.richedit.core.units.UIToTwips(0.01));
            if(paperKind == 0)
                paperKind = __aspxRichEdit.PaperSizeConverter.calculatePaperKind({ width: size.height, height: size.width }, 0, this.richedit.core.units.UIToTwips(0.01));
            dxreDialog_CbxPaperSize.SetValue(paperKind >= 0 ? paperKind : null);
        },
        updateOrientation: function(landscape) {
            dxreDialog_RblOrientation.SetSelectedIndex(landscape ? 1 : 0);
        }
    });

    var REColumnsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        InitializeDialogFields: function(parameters) {
            this.currentColumnCount = 0;
            this.controller = new __aspxRichEdit.ColumnsEditorController(parameters);
            this.UpdateForm();
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            for(var i = 0; i < this.controller.presets.length; i++) {
                var presetButton = this.GetPresetElement(i);
                presetButton.Click.AddHandler(function(s) {
                    this.OnColumnsPresetChecked(this.GetIndexByElementName(s.name));
                }.aspxBind(this));
            }
            dxreDialog_SpnNumberOfColumns.ValueChanged.AddHandler(function(s) {
                this.OnColumnsCountChanged(s.GetValue());
            }.aspxBind(this));
            dxreDialog_ChkEqualWidth.CheckedChanged.AddHandler(function(s) {
                this.OnEqualColumnWidthChanged(s.GetChecked());
            }.aspxBind(this));
            dxreDialog_CbxApplyTo.ValueChanged.AddHandler(function(s) {
                this.OnApplyTypeChanged(s.GetValue());
            }.aspxBind(this));
        },
        initColumnsEditorHandlers: function() {
            for(var i = 0; i < this.currentColumnCount; i++) {
                var spacingSpinElement = this.GetSpacingSpinElement(i);
                var widthSpinElement = this.GetWidthSpinElement(i);
                spacingSpinElement.ValueChanged.AddHandler(function(s) {
                    this.OnColumnsEditorSpacingChanged(this.GetIndexByElementName(s.name), s.GetValue());
                }.aspxBind(this));
                widthSpinElement.ValueChanged.AddHandler(function(s) {
                    this.OnColumnsEditorWidthChanged(this.GetIndexByElementName(s.name), s.GetValue());
                }.aspxBind(this));
            }
        },
        GetFocusedElement: function() {
            return dxreDialog_SpnNumberOfColumns;
        },
        UpdateForm: function() {
            dxreDialog_SpnNumberOfColumns.SetValue(this.controller.columnsInfo.columnCount);
            dxreDialog_ChkEqualWidth.SetChecked(this.controller.columnsInfo.equalColumnWidth);
            dxreDialog_CbxApplyTo.SetValue(this.controller.columnsInfo.applyType);

            if(this.controller.columnsInfo.columnCount)
                this.UpdateColumnsEditor();
            this.UpdatePresetControls();
        },
        UpdateColumnsEditor: function() {
            if(this.currentColumnCount == this.controller.columnsInfo.columnCount)
                this.UpdateColumnsEditorFields();
            else {
                this.currentColumnCount = this.controller.columnsInfo.columnCount;
                this.SendCallbackForContent();
            }
        },
        UpdateColumnsEditorFields: function() {
            for(var i = 0, length = this.controller.columnsInfo.columnCount; i < length; i++) {
                var width = this.controller.getWidth(i);
                var spacing = this.controller.getSpacing(i);
                this.GetWidthSpinElement(i).SetValue(width);
                this.GetSpacingSpinElement(i).SetValue(spacing);
            }
            this.ApplyEditorsAvailability();
        },
        UpdatePresetControls: function() {
            var hasChecked = false;
            for(var i = 0; i < this.controller.presets.length; i++) {
                var checked = this.controller.matchPreset(i);
                if(checked) {
                    hasChecked = true;
                    this.GetPresetElement(i).SetChecked(true);
                    break;
                }
            }
            if(!hasChecked)
                this.GetPresetElement(0).SetChecked(false);
        },
        SendCallbackForContent: function() {
            var columnCount = this.controller.columnsInfo.columnCount;
            this.ShowLoadingPanel();
            this.SetEnabledForColumnsCountControl(false);
            this.SetStateObjectValue(columnCount);
            this.GetColumnsEditorCallbackHelper().SendCallback(constants.ColumnsEditorCallbackPrefix, columnCount);
        },
        SetStateObjectValue: function(value) {
            this.richedit.UpdateStateObjectWithObject({ columnsCount: value });
        },
        ShowLoadingPanel: function() {
            var offsetElement = this.GetColumnsEditorElement();
            this.richedit.CreateLoadingDiv(document.body, offsetElement);
            this.richedit.CreateLoadingPanelWithAbsolutePosition(document.body, offsetElement);
        },
        ApplyEditorsAvailability: function() {
            var lastIndex = this.controller.columnsInfo.columnCount - 1;
            this.SetEnableForColumnsEditors(0, lastIndex, false);
            if(this.controller.columnsInfo.equalColumnWidth && this.controller.columnsInfo.columnCount > 0)
                this.SetEnableForColumnsEditors(0, 0, true);
            else
                this.SetEnableForColumnsEditors(0, lastIndex, true);
            this.GetSpacingSpinElement(lastIndex).SetEnabled(false);
        },
        SetEnableForColumnsEditors: function(from, to, enabled) {
            for(var i = from; i <= to; i++) {
                this.GetWidthSpinElement(i).SetEnabled(enabled);
                this.GetSpacingSpinElement(i).SetEnabled(enabled);
            }
        },
        SetEnabledForColumnsCountControl: function(value) {
            dxreDialog_SpnNumberOfColumns.SetEnabled(value);
        },
        OnCallbackForContent: function(content) {
            ASPx.SetInnerHtml(this.GetColumnsEditorElement(), content);
        },
        OnEndCallbackForContent: function() {
            this.SetStateObjectValue(null);
            this.SetEnabledForColumnsCountControl(true);
            this.UpdateColumnsEditorFields();
            this.initColumnsEditorHandlers();
            this.SetFocusInField();
        },
        OnColumnsCountChanged: function(columnsCount) {
            this.controller.changeColumnCount(columnsCount);
            this.UpdateForm();
        },
        OnEqualColumnWidthChanged: function(value) {
            this.controller.setEqualColumnWidth(value);
            this.UpdateForm();
        },
        OnColumnsPresetChecked: function(presetIndex) {
            this.controller.applyPreset(presetIndex);
            this.UpdateForm();
        },
        OnColumnsEditorWidthChanged: function(index, value) {
            this.controller.setWidth(index, value);
            this.UpdateForm();
        },
        OnColumnsEditorSpacingChanged: function(index, value) {
            this.controller.setSpacing(index, value);
            this.UpdateForm();
        },
        OnApplyTypeChanged: function(value) {
            this.controller.columnsInfo.applyType = value;
        },
        GetColumnsEditorCallbackHelper: function() {
            if(!this.callbackHelper)
                this.callbackHelper = new CallbackHelper(this);
            return this.callbackHelper;
        },
        GetWidthSpinElement: function(index) {
            return window[constants.WidthSpinElementName + index];
        },
        GetSpacingSpinElement: function(index) {
            return window[constants.SpacingSpinElementName + index];
        },
        GetPresetElement: function(index) {
            return element = window[constants.ColumnsPresetElementName + index];
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Columns;
        },
        GetColumnsEditorElement: function() {
            var item = dxreDialog_WidthSpacingLayout.GetItemByName("ColumnsEditor");
            return dxreDialog_WidthSpacingLayout.GetHTMLElementByItem(item).firstChild;
        },
        GetIndexByElementName: function(name) {
            var nameParts = name.split(" ");
            return parseInt(nameParts[nameParts.length - 1]);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.columnsInfo = this.controller.columnsInfo;
            return returnedObject;
        }
    });

    var REInsertTableDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.InsertTable;
        },
        GetFocusedElement: function() {
            return dxreDialog_SpnColumnsNumber;
        },
        InitializeDialogFields: function(parameters) {
            dxreDialog_SpnRowsNumber.SetValue(parameters.rowCount);
            dxreDialog_SpnColumnsNumber.SetValue(parameters.columnCount);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.rowCount = dxreDialog_SpnRowsNumber.GetValue();
            returnedObject.columnCount = dxreDialog_SpnColumnsNumber.GetValue();
            return returnedObject;
        },
    });

    var REInsertTableCellsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.InsertTableCells;
        },
        InitializeDialogFields: function(parameters) {
            dxreDialog_RblCellOperation.SetValue(parameters.tableCellOperation);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.tableCellOperation = dxreDialog_RblCellOperation.GetValue();
            return returnedObject;
        }
    });

    var REDeleteTableCellsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.DeleteTableCells;
        },
        InitializeDialogFields: function(parameters) {
            dxreDialog_RblCellOperation.SetValue(parameters.tableCellOperation);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.tableCellOperation = dxreDialog_RblCellOperation.GetValue();
            return returnedObject;
        }
    });

    var RESplitTableCellsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.SplitTableCells;
        },
        InitializeDialogFields: function(parameters) {
            dxreDialog_SpnColumnsNumber.SetValue(parameters.columnCount);
            dxreDialog_SpnRowsNumber.SetValue(parameters.rowCount);
            dxreDialog_ChkMergeCells.SetChecked(parameters.isMergeBeforeSplit);
            dxreDialog_ChkMergeCells.SetEnabled(parameters.isMergeBeforeSplit);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.columnCount = dxreDialog_SpnColumnsNumber.GetValue();
            returnedObject.rowCount = dxreDialog_SpnRowsNumber.GetValue();
            returnedObject.isMergeBeforeSplit = dxreDialog_ChkMergeCells.GetValue();
            return returnedObject;
        }
    });

    var RETablePropertiesDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.TableProperties;
        },
        InitializeDialogFields: function(parameters) {
            if(parameters.useDefaultTableWidth !== null && !parameters.useDefaultTableWidth) {
                dxreDialog_ChkTablePrefWidth.SetChecked(true);
                this.SetEnabledForTablePreferredWidth(true);
            }
            dxreDialog_CbxTableMeasureIn.SetValue(parameters.tablePreferredWidth.type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ? parameters.tablePreferredWidth.type : __aspxRichEdit.TableWidthUnitType.ModelUnits);
            dxreDialog_SpnTablePrefWidth.displayFormat = this.getUnitFormatString(parameters.tablePreferredWidth.type);
            dxreDialog_SpnTablePrefWidth.SetValue(this.modelUnitToPercent(parameters.tablePreferredWidth.type, parameters.tablePreferredWidth.value));

            dxreDialog_RblAligment.SetValue(parameters.tableRowAlignment);
            dxreDialog_SpnIndentLeft.SetValue(parameters.tableIndent);
            dxreDialog_ChkAllowSpacing.SetValue(parameters.allowCellSpacing);
            dxreDialog_SpnSpacing.SetEnabled(parameters.allowCellSpacing);
            dxreDialog_SpnSpacing.SetValue(parameters.cellSpacing);
            dxreDialog_ChkAutoResize.SetValue(parameters.resizeToFitContent);
            dxreDialog_SpnTopMarginDefault.SetValue(parameters.defaultCellMarginTop);
            dxreDialog_SpnLeftMarginDefault.SetValue(parameters.defaultCellMarginLeft);
            dxreDialog_SpnRightMarginDefault.SetValue(parameters.defaultCellMarginRight);
            dxreDialog_SpnBottomMarginDefault.SetValue(parameters.defaultCellMarginBottom);

            if(parameters.useDefaultRowHeight !== null && !parameters.useDefaultRowHeight) {
                dxreDialog_ChkSpecifyHeight.SetChecked(true);
                this.SetEnabledForRowHeight(true);
            }
            dxreDialog_SpnRowHeight.SetValue(parameters.rowHeight.value);
            dxreDialog_CbxRowHeight.SetValue(parameters.rowHeight.type);

            if(parameters.useDefaultColumnWidth !== null && !parameters.useDefaultColumnWidth) {
                dxreDialog_ChkColumnPrefWidth.SetValue(true);
                this.SetEnabledForColumnPreferredWidth(true);
            }
            dxreDialog_CbxColumnMeasureIn.SetValue(parameters.columnPreferredWidth.type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ? parameters.columnPreferredWidth.type : __aspxRichEdit.TableWidthUnitType.ModelUnits);
            dxreDialog_SpnColumnPrefWidth.displayFormat = this.getUnitFormatString(parameters.columnPreferredWidth.type);
            dxreDialog_SpnColumnPrefWidth.SetValue(this.modelUnitToPercent(parameters.columnPreferredWidth.type, parameters.columnPreferredWidth.value));

            if(parameters.useDefaultCellWidth !== null && !parameters.useDefaultCellWidth) {
                dxreDialog_ChkCellPrefWidth.SetValue(true);
                this.SetEnabledForCellPreferredWidth(true);
            }
            dxreDialog_CbxCellMeasureIn.SetValue(parameters.cellPreferredWidth.type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ? parameters.cellPreferredWidth.type : __aspxRichEdit.TableWidthUnitType.ModelUnits);
            dxreDialog_SpnCellPrefWidth.displayFormat = this.getUnitFormatString(parameters.cellPreferredWidth.type);
            dxreDialog_SpnCellPrefWidth.SetValue(this.modelUnitToPercent(parameters.cellPreferredWidth.type, parameters.cellPreferredWidth.value));

            dxreDialog_RblVerticalAligment.SetValue(parameters.cellVerticalAlignment);
            dxreDialog_ChkWrapText.SetValue(parameters.cellNoWrap !== null ? !parameters.cellNoWrap : null);
            dxreDialog_ChkSameAsTable.SetValue(parameters.cellMarginsSameAsTable);
            dxreDialog_SpnTopMargin.SetValue(parameters.cellMarginTop);
            dxreDialog_SpnLeftMargin.SetValue(parameters.cellMarginLeft);
            dxreDialog_SpnRightMargin.SetValue(parameters.cellMarginRight);
            dxreDialog_SpnBottomMargin.SetValue(parameters.cellMarginBottom);
            this.SetEnabledForCellMargins(!parameters.cellMarginsSameAsTable);
            this.setMinMaxValuesForEdits();

            dxreDialog_TablePropertiesPageControl.SetActiveTab(dxreDialog_TablePropertiesPageControl.GetTabByName(parameters.initialTab));
        },
        setMinMaxValuesForEdits: function() {
            dxreDialog_SpnTablePrefWidth.SetMinValue(__aspxRichEdit.TablePropertiesDialogDefaults.MinTableWidthByDefault);
            dxreDialog_SpnTablePrefWidth.SetMaxValue(this.GetTableWidthMaxValueConsiderWidthUnitType(dxreDialog_CbxTableMeasureIn.GetValue()));
            dxreDialog_SpnColumnPrefWidth.SetMinValue(__aspxRichEdit.TablePropertiesDialogDefaults.MinColumnWidthByDefault);
            dxreDialog_SpnColumnPrefWidth.SetMaxValue(this.GetColumnWidthMaxValueConsiderWidthUnitType(dxreDialog_CbxColumnMeasureIn.GetValue()));
            dxreDialog_SpnCellPrefWidth.SetMinValue(__aspxRichEdit.TablePropertiesDialogDefaults.MinCellWidthByDefault);
            dxreDialog_SpnCellPrefWidth.SetMaxValue(this.GetCellWidthMaxValueConsiderWidthUnitType(dxreDialog_CbxCellMeasureIn.GetValue()));
            dxreDialog_SpnRowHeight.SetMinValue(this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MinRowHeightByDefault));
            dxreDialog_SpnRowHeight.SetMaxValue(this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MaxRowHeightByDefault));
            dxreDialog_SpnIndentLeft.SetMinValue(this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MinTableIndentByDefault));
            dxreDialog_SpnIndentLeft.SetMaxValue(this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MaxTableIndentByDefault));
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_ChkTablePrefWidth.CheckedChanged.AddHandler(function(s) {
                this.SetEnabledForTablePreferredWidth(s.GetValue());
            }.aspxBind(this));
            dxreDialog_CbxTableMeasureIn.ValueChanged.AddHandler(function(s) {
                dxreDialog_SpnTablePrefWidth.displayFormat = this.getUnitFormatString(s.GetValue());
                var value = this.switchMeasureIn(s.GetValue(), dxreDialog_SpnTablePrefWidth.GetValue(), this.parameters.maxTableWidth);
                dxreDialog_SpnTablePrefWidth.SetValue(value);
                dxreDialog_SpnTablePrefWidth.SetMaxValue(this.GetTableWidthMaxValueConsiderWidthUnitType(s.GetValue()));
            }.aspxBind(this));
            dxreDialog_RblAligment.ValueChanged.AddHandler(function(s) {
                dxreDialog_SpnIndentLeft.SetEnabled(s.GetValue() === __aspxRichEdit.HorizontalAlignMode.Left);
            });
            dxreDialog_ChkAllowSpacing.CheckedChanged.AddHandler(function(s) {
                dxreDialog_SpnSpacing.SetEnabled(s.GetValue());
            });
            dxreDialog_ChkSpecifyHeight.CheckedChanged.AddHandler(function(s) {
                this.SetEnabledForRowHeight(s.GetValue());
            }.aspxBind(this));
            dxreDialog_ChkColumnPrefWidth.CheckedChanged.AddHandler(function(s) {
                dxreDialog_SpnColumnPrefWidth.SetEnabled(s.GetValue());
                dxreDialog_CbxColumnMeasureIn.SetEnabled(s.GetValue());
            });
            dxreDialog_CbxColumnMeasureIn.ValueChanged.AddHandler(function(s) {
                dxreDialog_SpnColumnPrefWidth.displayFormat = this.getUnitFormatString(s.GetValue());
                var value = this.switchMeasureIn(s.GetValue(), dxreDialog_SpnColumnPrefWidth.GetValue(), this.getMaxCellAvailableWidth());
                dxreDialog_SpnColumnPrefWidth.SetValue(value);
                dxreDialog_SpnColumnPrefWidth.SetMaxValue(this.GetColumnWidthMaxValueConsiderWidthUnitType(s.GetValue()));              
            }.aspxBind(this));
            dxreDialog_ChkCellPrefWidth.CheckedChanged.AddHandler(function(s) {
                dxreDialog_SpnCellPrefWidth.SetEnabled(s.GetValue());
                dxreDialog_CbxCellMeasureIn.SetEnabled(s.GetValue());
            });
            dxreDialog_CbxCellMeasureIn.ValueChanged.AddHandler(function(s) {
                dxreDialog_SpnCellPrefWidth.displayFormat = this.getUnitFormatString(s.GetValue());
                var value = this.switchMeasureIn(s.GetValue(), dxreDialog_SpnCellPrefWidth.GetValue(), this.getMaxCellAvailableWidth());
                dxreDialog_SpnCellPrefWidth.SetValue(value);
                dxreDialog_SpnCellPrefWidth.SetMaxValue(this.GetCellWidthMaxValueConsiderWidthUnitType(s.GetValue()));              
            }.aspxBind(this));
            dxreDialog_ChkSameAsTable.CheckedChanged.AddHandler(function(s) {
                this.SetEnabledForCellMargins(!s.GetValue());
            }.aspxBind(this));
        },
        SetEnabledForTablePreferredWidth: function(value) {
            dxreDialog_SpnTablePrefWidth.SetEnabled(value);
            dxreDialog_CbxTableMeasureIn.SetEnabled(value);
        },
        SetEnabledForColumnPreferredWidth: function(value) {
            dxreDialog_SpnColumnPrefWidth.SetEnabled(value);
            dxreDialog_CbxColumnMeasureIn.SetEnabled(value);
        },
        SetEnabledForCellPreferredWidth: function(value) {
            dxreDialog_SpnCellPrefWidth.SetEnabled(value);
            dxreDialog_CbxCellMeasureIn.SetEnabled(value);
        },
        SetEnabledForRowHeight: function(value) {
            dxreDialog_SpnRowHeight.SetEnabled(value);
            dxreDialog_CbxRowHeight.SetEnabled(value);
        },
        SetEnabledForCellMargins: function(value) {
            dxreDialog_SpnTopMargin.SetEnabled(value);
            dxreDialog_SpnLeftMargin.SetEnabled(value);
            dxreDialog_SpnRightMargin.SetEnabled(value);
            dxreDialog_SpnBottomMargin.SetEnabled(value);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.tablePreferredWidth.type = dxreDialog_CbxTableMeasureIn.GetValue();
            returnedObject.tablePreferredWidth.value = this.percentToModelUnit(returnedObject.tablePreferredWidth.type, dxreDialog_SpnTablePrefWidth.GetValue());
            returnedObject.useDefaultTableWidth = dxreDialog_ChkTablePrefWidth.GetValue() !== null ? !dxreDialog_ChkTablePrefWidth.GetValue() : null;
            returnedObject.tableRowAlignment = dxreDialog_RblAligment.GetValue();
            returnedObject.tableIndent = dxreDialog_SpnIndentLeft.GetValue();
            returnedObject.allowCellSpacing = dxreDialog_ChkAllowSpacing.GetValue();
            returnedObject.cellSpacing = dxreDialog_SpnSpacing.GetValue();
            returnedObject.resizeToFitContent = dxreDialog_ChkAutoResize.GetValue();
            returnedObject.defaultCellMarginTop = dxreDialog_SpnTopMarginDefault.GetValue();
            returnedObject.defaultCellMarginLeft = dxreDialog_SpnLeftMarginDefault.GetValue();
            returnedObject.defaultCellMarginRight = dxreDialog_SpnRightMarginDefault.GetValue();
            returnedObject.defaultCellMarginBottom = dxreDialog_SpnBottomMarginDefault.GetValue();
            returnedObject.useDefaultRowHeight = dxreDialog_ChkSpecifyHeight.GetValue() !== null ? !dxreDialog_ChkSpecifyHeight.GetValue() : null;
            returnedObject.rowHeight.type = dxreDialog_CbxRowHeight.GetValue();
            returnedObject.rowHeight.value = dxreDialog_SpnRowHeight.GetValue();
            returnedObject.columnPreferredWidth.type = dxreDialog_CbxColumnMeasureIn.GetValue();
            if(this.checkValueChanged(returnedObject.columnPreferredWidth.value, this.percentToModelUnit(returnedObject.columnPreferredWidth.type, dxreDialog_SpnColumnPrefWidth.GetValue())))
                returnedObject.columnPreferredWidth.value = this.percentToModelUnit(returnedObject.columnPreferredWidth.type, dxreDialog_SpnColumnPrefWidth.GetValue());
            returnedObject.useDefaultColumnWidth = dxreDialog_ChkColumnPrefWidth.GetValue() !== null ? !dxreDialog_ChkColumnPrefWidth.GetValue() : null;
            returnedObject.cellPreferredWidth.type = dxreDialog_CbxCellMeasureIn.GetValue();
            if(this.checkValueChanged(returnedObject.cellPreferredWidth.value, this.percentToModelUnit(returnedObject.cellPreferredWidth.type, dxreDialog_SpnCellPrefWidth.GetValue())))
                returnedObject.cellPreferredWidth.value = this.percentToModelUnit(returnedObject.cellPreferredWidth.type, dxreDialog_SpnCellPrefWidth.GetValue());
            returnedObject.useDefaultCellWidth = dxreDialog_ChkCellPrefWidth.GetValue() !== null ? !dxreDialog_ChkCellPrefWidth.GetValue() : null;
            returnedObject.cellVerticalAlignment = dxreDialog_RblVerticalAligment.GetValue();
            returnedObject.cellNoWrap = dxreDialog_ChkWrapText.GetValue() !== null ? !dxreDialog_ChkWrapText.GetValue() : null;
            returnedObject.cellMarginsSameAsTable = dxreDialog_ChkSameAsTable.GetValue();
            returnedObject.cellMarginTop = dxreDialog_SpnTopMargin.GetValue();
            returnedObject.cellMarginRight = dxreDialog_SpnRightMargin.GetValue();
            returnedObject.cellMarginBottom = dxreDialog_SpnBottomMargin.GetValue();
            returnedObject.cellMarginLeft = dxreDialog_SpnLeftMargin.GetValue();
            return returnedObject;
        },
        switchMeasureIn: function(targetType, value, maxWidth) {
            if(targetType === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent)
                return value / maxWidth * 100;
            else
                return value / 100 * maxWidth;
        },
        percentToModelUnit: function(type, valueFromUI) {
            return type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ? Math.floor(valueFromUI * 50) : valueFromUI;
        },
        modelUnitToPercent: function(type, valueForUI) {
            return type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ? valueForUI / 50 : valueForUI;
        },
        getUnitFormatString: function(unitType) {
            var abbreviation = unitType === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ? this.richedit.unitAbbreviations.percent : this.richedit.unitAbbreviations.unit;
            return "{0}" + abbreviation;
        },
        GetTableWidthMaxValueConsiderWidthUnitType: function(type) {
            if(type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent)
                return __aspxRichEdit.TablePropertiesDialogDefaults.MaxTableWidthInPercentByDefault;
            return this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MaxTableWidthInModelUnitsByDefault);
        },
        GetColumnWidthMaxValueConsiderWidthUnitType: function(type) {
            if(type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent)
                return __aspxRichEdit.TablePropertiesDialogDefaults.MaxColumnWidthInPercentByDefault;
            return this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MaxColumnWidthInModelUnitsByDefault);
        },
        GetCellWidthMaxValueConsiderWidthUnitType: function(type) {
            if(type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent)
                return __aspxRichEdit.TablePropertiesDialogDefaults.MaxCellWidthInPercentByDefault;
            return this.richedit.core.units.twipsToUI(__aspxRichEdit.TablePropertiesDialogDefaults.MaxCellWidthInModelUnitsByDefault);
        },
        getMaxCellAvailableWidth: function() {
            var maxCellWidth = this.parameters.tablePreferredWidth.type === __aspxRichEdit.TableWidthUnitType.FiftiethsOfPercent ?
                this.switchMeasureIn(__aspxRichEdit.TableWidthUnitType.ModelUnits, this.modelUnitToPercent(this.parameters.tablePreferredWidth.type, this.parameters.tablePreferredWidth.value), this.parameters.maxTableWidth) :
                this.parameters.tablePreferredWidth.value;
            return maxCellWidth ? maxCellWidth : this.parameters.maxTableWidth;
        },
        checkValueChanged: function (oldValue, newValue) {
            return oldValue.toFixed(2) !== newValue.toFixed(2);
        },
        ShowBorderShadingForm: function(clientCommand) {
            this.OnComplete(0);
            this.richedit.core.commandManager.getCommand(__aspxRichEdit.RichEditClientCommand.ShowBorderShadingForm).execute(this.parameters);
        }
    });

    var REBorderShadingDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.BorderShading;
        },
        InitializeDialogFields: function(parameters) {
            var previewContainer = this.GetBorderPreviewElement();
            this.createTablePreview(previewContainer);
            dxreDialog_CeFillColor.SetValue(parameters.backgroundColor);
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_BtnPresetNone.Click.AddHandler(function(s) {
                this.topBorder.resetBorder();
                this.horizontalInBorder.resetBorder();
                this.bottomBorder.resetBorder();
                this.leftBorder.resetBorder();
                this.verticalInBorder.resetBorder();
                this.rightBorder.resetBorder();
            }.aspxBind(this));
            dxreDialog_BtnPresetBox.Click.AddHandler(function(s) {
                this.topBorder.setBorder();
                this.leftBorder.setBorder();
                this.bottomBorder.setBorder();
                this.rightBorder.setBorder();
                this.verticalInBorder.resetBorder();
                this.horizontalInBorder.resetBorder();
            }.aspxBind(this));
            dxreDialog_BtnPresetAll.Click.AddHandler(function(s) {
                this.topBorder.setBorder();
                this.leftBorder.setBorder();
                this.bottomBorder.setBorder();
                this.rightBorder.setBorder();
                this.verticalInBorder.setBorder();
                this.horizontalInBorder.setBorder();
            }.aspxBind(this));
            dxreDialog_BtnPresetGrid.Click.AddHandler(function(s) {
                this.topBorder.setBorder();
                this.leftBorder.setBorder();
                this.bottomBorder.setBorder();
                this.rightBorder.setBorder();
                this.verticalInBorder.setDefaultBorder();
                this.horizontalInBorder.setDefaultBorder();
            }.aspxBind(this));
            dxreDialog_CeFillColor.ValueChanged.AddHandler(function(s) {
                var previewElement = this.GetBorderPreviewElement();
                previewElement.style.backgroundColor = s.GetValue();
            }.aspxBind(this));
        },
        createTablePreview: function(container) {
            this.topBorder = this.createTablePreviewLine(container, constants.BorderTopLineClass, true, this.parameters.top, dxreDialog_BtnBorderTop);
            this.horizontalInBorder = this.createTablePreviewLine(container, constants.BorderMiddleLineClass, true, this.parameters.insideHorizontal, dxreDialog_BtnBorderInsideHorizontal);
            this.bottomBorder = this.createTablePreviewLine(container, constants.BorderBottomLineClass, true, this.parameters.bottom, dxreDialog_BtnBorderBottom);
            this.leftBorder = this.createTablePreviewLine(container, constants.BorderLeftLineClass, false, this.parameters.left, dxreDialog_BtnBorderLeft);
            this.verticalInBorder = this.createTablePreviewLine(container, constants.BorderCenterLineClass, false, this.parameters.insideVertical, dxreDialog_BtnBorderInsideVertical);
            this.rightBorder = this.createTablePreviewLine(container, constants.BorderRightLineClass, false, this.parameters.right, dxreDialog_BtnBorderRight);
        },
        createTablePreviewLine: function(container, cssClass, isHorizontalLine, borderInfo, controlBtn) {
            var line = ASPx.CreateHtmlElement();
            ASPx.AddClassNameToElement(line, constants.BorderLineClass);
            ASPx.AddClassNameToElement(line, isHorizontalLine ? constants.BorderHorizontalLineClass : constants.BorderVerticalLineClass);
            ASPx.AddClassNameToElement(line, cssClass);
            container.appendChild(line);
            var controlLine = ASPx.CreateHtmlElement();
            ASPx.AddClassNameToElement(controlLine, constants.BorderControlLineClass);
            ASPx.AddClassNameToElement(controlLine, isHorizontalLine ? constants.BorderHorizontalLineClass : constants.BorderVerticalLineClass);
            ASPx.AddClassNameToElement(controlLine, cssClass);
            container.appendChild(controlLine);
            return new DialogBorderLine(this, borderInfo, line, controlLine, controlBtn);
        },
        getBorderColorValue: function() {
            return dxreDialog_CeBorderColor.GetValue();
        },
        getBorderWidthValue: function() {
            return dxreDialog_CmbWidth.GetValue();
        },
        getBorderStyleValue: function() {
            return dxreDialog_CmbBorderStyle.GetValue();
        },
        GetBorderPreviewElement: function() {
            return ASPx.GetElementById(this.richedit.name + constants.TableBorderPreviewElementID);
        },
        setCustomPreset: function() {
            dxreDialog_BtnPresetCustom.SetChecked(true);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.backgroundColor = dxreDialog_CeFillColor.GetValue();
            returnedObject.top = this.topBorder.borderInfo;
            return returnedObject;
        },
    });

    var REInsertImageDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackCount = 0;
        },
        OnComplete: function(result, params) {
            this.insertImageParams = {};
            if(result) {
                if(!this.HasImageProductedFromUrl())
                    return this.GetImageUploader().UploadFile();
                else
                    return this.SaveImageToServerViaCallback(this.GetInsertImageUrlTextBox().GetText());
            }
            else
                this.HideLoadingPanelOverDialogPopup();
            ASPxRichEditDialog.prototype.OnComplete.call(this, result, params);
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            executeIfExists("dxreDialog_RblNavigation", function(element) {
                element.ValueChanged.AddHandler(this.OnImageFromTypeChanged.aspxBind(this));
            }.aspxBind(this));
            dxreDialog_TxbInsertImageUrl.TextChanged.AddHandler(function(s) {
                this.OnImageSrcChanged(s.GetText());
            }.aspxBind(this));
            executeIfExists("dxreDialog_UplImage", function(element) {
                element.FileUploadComplete.AddHandler(function(s, e) {
                    this.OnImageUploadComplete(e);
                }.aspxBind(this));
                element.FilesUploadComplete.AddHandler(function(s, e) {
                    this.OnImageUploadComplete(e);
                }.aspxBind(this));
                element.FilesUploadStart.AddHandler(function(s, e) {
                    this.OnImageUploadStart();
                }.aspxBind(this));
            }.aspxBind(this));
        },
        HasImageProductedFromUrl: function() {
            if(typeof(dxreDialog_RblNavigation) == "undefined")
                return true;
            return dxreDialog_RblNavigation.GetValue() ? false : true;
        },
        SaveImageToServerViaCallback: function(src) {
            this.ShowLoadingPanelOverDialogPopup();
            this.ownerControl.sendInternalServiceCallback(constants.SaveImageToServerCallbackPrefix, src, this);
            this.callbackCount++;
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.InsertImage;
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.id = this.insertImageParams.id;
            returnedObject.originalWidth = this.insertImageParams.originalWidth;
            returnedObject.originalHeight = this.insertImageParams.originalHeight;
            return returnedObject;
        },
        CheckImageExisting: function(checkingSrc) {
            if(document.images) {
                this.testImage = new Image();
                ASPx.Evt.AttachEventToElement(this.testImage, "load", new Function("aspxTestExistingImageOnLoad" + "('" + this.richedit.name + "');"));
                ASPx.Evt.AttachEventToElement(this.testImage, "error", new Function("aspxTestExistingImageOnError" + "('" + this.richedit.name + "');"));
                this.testImage.src = checkingSrc;
            }
        },
        SetPreviewImageSize: function(sourceWidth, sourceHeight, maxWidth, maxHeight) {
            var newWidth = sourceWidth;
            var newHeight = sourceHeight;
            if((sourceWidth > maxWidth) ||
                (sourceHeight > maxHeight)) {

                var cw = sourceWidth / maxWidth;
                var ch = sourceHeight / maxHeight;

                if(cw > ch) {
                    var cw = sourceWidth / maxWidth;
                    newWidth = Math.floor(sourceWidth / cw);
                    newHeight = Math.floor(sourceHeight / cw);
                }
                else {
                    newWidth = Math.floor(sourceWidth / ch);
                    newHeight = Math.floor(sourceHeight / ch);
                }
            }
            var previewImage = this.GetPreviewImageElement();
            previewImage.style.width = newWidth + "px";
            previewImage.style.height = newHeight + "px";
        },
        ShowErrorMessage: function(message) {
            var textBox = this.GetInsertImageUrlTextBox();
            textBox.isValid = false;
            textBox.errorText = message;
            textBox.UpdateErrorFrameAndFocus(false, true);
        },
        GetFocusedElement: function() {
            return dxreDialog_TxbInsertImageUrl;
        },
        OnCallback: function(result) {
            if(result.indexOf(constants.SaveImageToServerCallbackPrefix) == 0) {
                this.OnImageSavedToServer(result.substring(constants.SaveImageToServerCallbackPrefix.length + 1, result.length));
                if(this.callbackCount > 0)
                    this.callbackCount--;
                if(this.callbackCount <= 0)
                    this.ownerControl.clearOwnerControlCallback();
            }
            else
                ASPx.Dialog.prototype.OnCallback.call(this, result);
        },
        OnImageFromTypeChanged: function() {
            var url = dxreDialog_InsertImageFormLayout.GetItemByName("TxbInsertImageUrl");
            var previewArea = dxreDialog_InsertImageFormLayout.GetItemByName("ImagePreview");
            var uploadArea = dxreDialog_InsertImageFormLayout.GetItemByName("UploadControl");
            url.SetVisible(!dxreDialog_RblNavigation.GetValue());
            previewArea.SetVisible(!dxreDialog_RblNavigation.GetValue());
            uploadArea.SetVisible(dxreDialog_RblNavigation.GetValue());
        },
        OnLoadTestExistingImage: function() {
            ASPxClientEdit.ValidateGroup("_dxeTbxInsertImageUrlGroup");
            this.GetPreviewImageElement().src = this.testImage.src;
            var previewArea = this.GetPreviewAreaCell();

            var maxWidth = previewArea.clientWidth;
            var maxHeight = ASPx.Browser.WebKitFamily ? previewArea.offsetHeight : previewArea.clientHeight;

            this.SetPreviewImageSize(this.testImage.width, this.testImage.height, maxWidth, maxHeight);

            ASPx.SetElementDisplay(this.GetPreviewTextElement(), false);
            ASPx.SetElementDisplay(this.GetPreviewImageElement(), true);
            previewArea.style.borderStyle = "none";
        },
        OnErrorTestExistingImage: function() {
            ASPx.SetElementDisplay(this.GetPreviewTextElement(), true);
            ASPx.SetElementDisplay(this.GetPreviewImageElement(), false);

            var previewArea = this.GetPreviewAreaCell();
            previewArea.style.borderStyle = "";
        },
        OnImageSrcChanged: function(src) {
            this.CheckImageExisting(src);
        },
        OnImageSavedToServer: function(result) {
            if(result.indexOf(constants.SaveImageToServerErrorCallbackPrefix) > -1)
                this.ShowErrorMessage(result.substring(constants.SaveImageToServerErrorCallbackPrefix.length + 1, result.length));
            else {
                var result = eval(result.substr(constants.SaveImageToServerNewUrlCallbackPrefix.length + 1, result.length));
                this.insertImageParams.id = result.id;
                this.insertImageParams.originalWidth = result.originalWidth;
                this.insertImageParams.originalHeight = result.originalHeight;
                ASPxRichEditDialog.prototype.OnComplete.call(this, 1, this.insertImageParams);
            }
        },
        OnImageUploadComplete: function(args) {
            this.HideLoadingPanelOverDialogPopup();
            if(args.isValid) {
                var result = eval(args.callbackData);
                this.insertImageParams.id = result.id;
                this.insertImageParams.originalWidth = result.originalWidth;
                this.insertImageParams.originalHeight = result.originalHeight;
                ASPxRichEditDialog.prototype.OnComplete.call(this, 1, this.insertImageParams);
            }
        },
        OnImageUploadStart: function() {
            this.ShowLoadingPanelOverDialogPopup();
        },
        GetPreviewTextElement: function() {
            return ASPx.GetElementById(this.richedit.name + constants.PreviewTextElementID);
        },
        GetPreviewImageElement: function() {
            return ASPx.GetElementById(this.richedit.name + constants.PreviewImageElementID);
        },
        GetPreviewAreaCell: function() {
            return ASPx.GetParentByTagName(this.GetPreviewImageElement(), "table");
        },
        GetImageUploader: function() {
            return dxreDialog_UplImage;
        },
        GetInsertImageUrlTextBox: function() {
            return dxreDialog_TxbInsertImageUrl;
        },
        isDialogValid: function() {
            var ret = true;
            if(dxreDialog_TxbInsertImageUrl.IsVisible())
                ret = ASPxClientEdit.ValidateGroup("_dxeTbxInsertImageUrlGroup") && ret;
            else {
                ret = (dxreDialog_UplImage.GetText() != "") && ret;
                if(!ret)
                    dxreDialog_UplImage.UpdateCommonErrorDiv(dxreDialog_HiddenField.Get("RequiredFieldError"), true);
            }
            return ret;
        }
    });

    var REErrorDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Error;
        },
        InitializeDialogFields: function(parameters) {
            var errorText = ASPxRichEditDialogList.ErrorTexts[parameters.errorTextId];
            dxreDialog_ErrorText.SetValue(errorText);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            return returnedObject;
        },
    });

    var RENumberingListDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.BulletedAndNumbering;
        },
        InitializeDialogFields: function(parameters) {
            this.FillAllowedListType();
            this.FillAbstractNumberingListsArray();

            this.InitializeTabs();
            this.SetSelectedList(parameters.selectedAbstractNumberingList);
        },
        InitializeTabs: function() {
            this.listBoxes = [];
            this.forEachListType(function(listType) {
                this.InitializeListBoxes(listType);
                this.CreateItems(listType);
            }.aspxBind(this));
            this.lastListType = this.GetActiveTabType();
            this.initializeTabsComplited = true;
        },
        SetSelectedList: function(selectedList) {
            if(selectedList) {
                var listType = selectedList.getListType();
                if(this.IsAllowedListType(listType)) {
                    this.SetActiveTab(listType);
                    this.GetListBox(listType).SetSelectedValue(selectedList);
                }
            }
            if(selectedList === null)
                this.GetListBox(this.lastListType).SetSelectedValue(null);
        },
        InitializeListBoxes: function(listType) {
            this.listBoxes[listType] = new NumberingListBox(this, "numbering" + listType, this.GetMainElementByListType(listType));
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_BtnCustomize.Click.AddHandler(this.ShowCustomizeListDialog.aspxBind(this));
            dxreDialog_NumberingListPageControl.ActiveTabChanged.AddHandler(this.OnActiveTabChanged.aspxBind(this));
        },
        CreateItems: function(listType) {
            var listBox = this.GetListBox(listType);
            listBox.AddItem(null);
            for(var i = 0, list; list = this.abstractNumberingLists[i]; i++)
                if(list.getListType() == listType)
                    listBox.AddItem(list);
        },
        SetActiveTab: function(listType) {
            switch(listType) {
                case __aspxRichEdit.NumberingType.Bullet:
                    dxreDialog_NumberingListPageControl.SetActiveTabIndex(0);
                    break;
                case __aspxRichEdit.NumberingType.Simple:
                    dxreDialog_NumberingListPageControl.SetActiveTabIndex(1);
                    break;
                case __aspxRichEdit.NumberingType.MultiLevel:
                    dxreDialog_NumberingListPageControl.SetActiveTabIndex(2);
                    break;
            }
            this.lastListType = listType;
        },
        FillAbstractNumberingListsArray: function() {
            this.abstractNumberingLists = [];
            this.FillListsArrayFromModel(this.richedit.core.model.abstractNumberingLists);
            this.FillListsArrayFromModel(this.richedit.core.model.abstractNumberingListTemplates);
        },
        FillListsArrayFromModel: function(abstractNumberingLists) {
            for(var i = 0, list; list = abstractNumberingLists[i]; i++)
                if(!this.IsArrayContainsListWithSameProperties(list))
                    this.abstractNumberingLists.push(list);
        },
        IsArrayContainsListWithSameProperties: function(list) {
            for(var i = 0, currentList; currentList = this.abstractNumberingLists[i]; i++)
                if(currentList.externallyEquals(list))
                    return true;
            return false;
        },
        OnValueChanged: function(value) {
            this.SetEnabledForCustomizeButton(!!value);
            this.SetEnabledForOkButton(true);
        },
        OnActiveTabChanged: function() {
            if(!this.initializeTabsComplited)
                return;
            var selectedIndex = this.GetListBox(this.lastListType).GetSelectedIndex();
            this.lastListType = this.GetActiveTabType();
            if(selectedIndex > -1) {
                var listBox = this.GetListBox(this.lastListType);
                var newIndex = Math.min(selectedIndex, listBox.itemsCount - 1);
                listBox.SetSelectedIndex(newIndex);
            }
        },
        SetEnabledForCustomizeButton: function(value) {
            dxreDialog_BtnCustomize.SetEnabled(value);
        },
        SetEnabledForOkButton: function(value) {
            dxreDialog_BtnOk.SetEnabled(value);
        },
        IsBulletedTabSelected: function() {
            return dxreDialog_NumberingListPageControl.GetActiveTabIndex() == 0;
        },
        GetActiveTabType: function() {
            var activeTabIndex = dxreDialog_NumberingListPageControl.GetActiveTabIndex();
            switch(activeTabIndex) {
                case 0:
                    return __aspxRichEdit.NumberingType.Bullet;
                case 1:
                    return __aspxRichEdit.NumberingType.Simple;
                case 2:
                    return __aspxRichEdit.NumberingType.MultiLevel;
            }
            return null;
        },
        GetListBox: function(listType) {
            return this.listBoxes[listType];
        },
        GetMainElementByListType: function(listType) {
            return ASPx.GetElementById(this.richedit.name + constants.AbstractNumberingListElementID + listType);
        },
        forEachListType: function(action) {
            for(var i = 0; i < this.allowedListTypes.length; i++)
                action(this.allowedListTypes[i]);
        },
        IsAllowedListType: function(listType) {
            return ASPx.Data.ArrayContains(this.allowedListTypes, listType);
        },
        FillAllowedListType: function() {
            this.allowedListTypes = [];
            var options = this.richedit.core.options;
            var defaultValue = __aspxRichEdit.DocumentCapability.Default;
            var enabledValue = __aspxRichEdit.DocumentCapability.Enabled;

            if(options.numberingBulleted == defaultValue || options.numberingBulleted == enabledValue)
                this.allowedListTypes.push(__aspxRichEdit.NumberingType.Bullet);
            if(options.numberingSimple == defaultValue || options.numberingSimple == enabledValue)
                this.allowedListTypes.push(__aspxRichEdit.NumberingType.Simple);
            if(options.numberingMultiLevel == defaultValue || options.numberingMultiLevel == enabledValue)
                this.allowedListTypes.push(__aspxRichEdit.NumberingType.MultiLevel);
        },
        ShowCustomizeListDialog: function() {
            var clientCommand = __aspxRichEdit.RichEditClientCommand.ShowCustomNumberingListForm;
            var listBox = this.GetListBox(this.GetActiveTabType());
            var selectedAbstractList = listBox.GetValue();

            var params = new __aspxRichEdit.DialogCustomNumberingListParameters();
            params.init(selectedAbstractList, 0);

            this.OnComplete(0);
            this.richedit.core.commandManager.getCommand(clientCommand).execute(params);
        },
        OnClose: function() {
            ASPxRichEditDialog.prototype.OnClose.call(this);
            this.initializeTabsComplited = false;
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.selectedAbstractNumberingList = this.GetListBox(this.GetActiveTabType()).GetValue();
            return returnedObject;
        }
    });

    var RENumberingListDialogBase = ASPx.CreateClass(ASPxRichEditDialog, {
        onCancelButtonClick: function() {
            this.ShowNumberingListForm();
        },
        InitializeDialogFields: function(parameters) {
            this.parameters = parameters;
            this.UpdateForm();
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_BtnFont.Click.AddHandler(this.ShowServiceFontForm.aspxBind(this));
            dxreDialog_SpnAlignedAt.ValueChanged.AddHandler(function(s) {
                this.OnAlignedValueChanged();
            }.aspxBind(this));
            dxreDialog_SpnIndentAt.ValueChanged.AddHandler(function(s) {
                this.OnIndentValueChanged();
            }.aspxBind(this));
        },
        UpdateForm: function() {

        },
        GetFirstLineIndent: function() {
            var level = this.GetEditedLevel();
            if(level.firstLineIndentType == __aspxRichEdit.ParagraphFirstLineIndent.Hanging)
                return level.leftIndent - level.firstLineIndent;
            return level.firstLineIndent + level.leftIndent;
        },
        GetEditedLevel: function() {
            return this.parameters.levels[this.GetEditedLevelIndex()];
        },
        UpdateNumberFormat: function() {
            var items = [];
            for(var i = 0; i <= this.GetEditedLevelIndex() ; i++) {
                var currentLevel = this.parameters.levels[i];
                var converter = __aspxRichEdit.OrdinalBasedNumberConverter.createConverter(currentLevel.format);
                items[i] = converter.convertNumber(currentLevel.start);
            }
            var value = ASPx.Formatter.Format(this.GetEditedLevel().displayFormatString, items);
            this.SetNumberFormatValue(value);
        },
        OnAlignedValueChanged: function() {
            this.AssignIndentValues();
        },
        OnIndentValueChanged: function() {
            this.AssignIndentValues();
        },
        OnNumberFormatChanged: function() {
            var numberFormat = dxreDialog_TxbNumberFormat.GetValue();
            var startIndex = 0;
            var validResult = true;
            var resultFormat = "";
            for(var i = 0; i <= this.GetEditedLevelIndex() ; i++) {
                var currentLevel = this.parameters.levels[i];
                var converter = __aspxRichEdit.OrdinalBasedNumberConverter.createConverter(currentLevel.format);
                var item = converter.convertNumber(currentLevel.start);
                var index = numberFormat.indexOf(item, startIndex);
                if(index == -1) {
                    validResult = false;
                    break;
                }
                resultFormat += numberFormat.substring(startIndex, index) + "{" + i + "}";
                startIndex = index + item.length;
            }
            resultFormat += numberFormat.substring(startIndex, numberFormat.length);

            if(validResult)
                this.GetEditedLevel().displayFormatString = resultFormat;
            this.UpdateNumberFormat();
        },
        OnNumberStyleChanged: function() {
            this.GetEditedLevel().format = dxreDialog_CbxNumberStyle.GetValue();
            this.UpdateNumberFormat();
        },
        OnStartAtChanged: function() {
            this.GetEditedLevel().start = dxreDialog_SpnStartAt.GetValue();
            this.UpdateNumberFormat();
        },
        OnNumberPositionChanged: function() {
            this.GetEditedLevel().alignment = dxreDialog_CbxNumberPosition.GetValue();
        },
        AssignIndentValues: function() {
            var alignValue = this.richedit.core.units.UIToTwips(this.GetAlignedEditorValue());
            var indentValue = this.richedit.core.units.UIToTwips(this.GetIndentEditorValue());

            var level = this.GetEditedLevel();
            level.leftIndent = indentValue;
            var probableFirstLineIndent = indentValue - alignValue;
            if(probableFirstLineIndent > 0) {
                level.firstLineIndentType = __aspxRichEdit.ParagraphFirstLineIndent.Hanging
                level.firstLineIndent = probableFirstLineIndent;
            }
            else if(probableFirstLineIndent < 0) {
                level.firstLineIndentType = __aspxRichEdit.ParagraphFirstLineIndent.Indented;
                level.firstLineIndent = -probableFirstLineIndent;
            }
            else {
                level.firstLineIndentType = __aspxRichEdit.ParagraphFirstLineIndent.None;
                level.firstLineIndent = 0;
            }
        },
        SetNumberFormatValue: function(value) {
            dxreDialog_TxbNumberFormat.SetValue(value);
        },
        GetAlignedEditorValue: function() {
            return dxreDialog_SpnAlignedAt.GetValue();
        },
        GetIndentEditorValue: function() {
            return dxreDialog_SpnIndentAt.GetValue();
        },
        GetEditedLevelIndex: function() {
            return 0;
        },
        ShowNumberingListForm: function() {
            this.OnComplete(0);
            var clientCommand = __aspxRichEdit.RichEditClientCommand.ShowNumberingListForm;
            this.richedit.core.commandManager.getCommand(clientCommand).execute(this.parameters.initAbstractNumberingList);
        },
        ShowServiceFontForm: function() {
            this.ShowServiceForm(__aspxRichEdit.RichEditClientCommand.ShowServiceFontForm);
        },
        ShowServiceForm: function(clientCommand) {
            this.OnComplete(0);
            this.richedit.core.commandManager.getCommand(clientCommand).execute(this.parameters);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            return returnedObject;
        }
    });

    var RESimpleNumberingListDialog = ASPx.CreateClass(RENumberingListDialogBase, {
        attachEvents: function() {
            RENumberingListDialogBase.prototype.attachEvents.call(this);
            dxreDialog_TxbNumberFormat.LostFocus.AddHandler(this.OnNumberFormatChanged.aspxBind(this));
            dxreDialog_CbxNumberStyle.ValueChanged.AddHandler(this.OnNumberStyleChanged.aspxBind(this));
            dxreDialog_SpnStartAt.ValueChanged.AddHandler(this.OnStartAtChanged.aspxBind(this));
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.CustomizeNumberedList;
        },
        UpdateForm: function() {
            var level = this.GetEditedLevel();
            dxreDialog_CbxNumberStyle.SetValue(level.format);
            dxreDialog_SpnStartAt.SetValue(level.start);
            dxreDialog_CbxNumberPosition.SetValue(level.alignment);
            dxreDialog_SpnIndentAt.SetValue(this.richedit.core.units.twipsToUI(level.leftIndent));
            dxreDialog_SpnAlignedAt.SetValue(this.richedit.core.units.twipsToUI(this.GetFirstLineIndent()));
            this.UpdateNumberFormat();
        }
    });

    var REBulletedListDialog = ASPx.CreateClass(RENumberingListDialogBase, {
        attachEvents: function() {
            RENumberingListDialogBase.prototype.attachEvents.call(this);
            dxreDialog_BtnChar.Click.AddHandler(this.ShowServiceSymbolsForm.aspxBind(this));
            for(var i = 0; i < 6; i++) {
                var preset = this.GetPresetControl(i);
                preset.Click.AddHandler(function(s) {
                    this.OnPresetChecked(s);
                }.aspxBind(this));
            }
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.CustomizeBulletedList;
        },
        UpdateForm: function() {
            dxreDialog_SpnIndentAt.SetValue(this.richedit.core.units.twipsToUI(this.GetEditedLevel().leftIndent));
            dxreDialog_SpnAlignedAt.SetValue(this.richedit.core.units.twipsToUI(this.GetFirstLineIndent()));
            this.UpdateBulletCharacters();
        },
        UpdateBulletCharacters: function() {
            var fontName = this.GetEditedLevel().fontName;
            var symbol = this.GetEditedLevel().displayFormatString;
            var symbolIndex = this.GetActiveSymbolIndex(symbol, fontName);
            if(symbolIndex == -1) {
                symbolIndex = 0;
                this.SetPresetSymbolProperties(symbolIndex, symbol, fontName);
            }
            this.GetPresetControl(symbolIndex).SetChecked(true);
        },
        GetActiveSymbolIndex: function(symbol, fontName) {
            for(var i = 0; i < 6; i++) {
                var preset = this.GetPresetControl(i);
                if(preset.GetText() == symbol && this.GetPresetFontName(preset) == fontName)
                    return i;
            }
            return -1;
        },
        SetPresetSymbolProperties: function(index, symbol, fontName) {
            var preset = this.GetPresetControl(index);
            preset.SetText(symbol);
            ASPx.SetStyles(preset.GetTextContainer(), { "font-family": fontName });
        },
        OnPresetChecked: function(preset) {
            this.GetEditedLevel().fontName = this.GetPresetFontName(preset);
            this.GetEditedLevel().displayFormatString = preset.GetText();
        },
        GetPresetControl: function(index) {
            return element = window[constants.BulletedPresetElementName + index];
        },
        GetPresetFontName: function(presetControl) {
            return ASPx.GetCurrentStyle(presetControl.GetTextContainer()).fontFamily;
        },
        ShowServiceSymbolsForm: function() {
            this.ShowServiceForm(__aspxRichEdit.RichEditClientCommand.ShowServiceSymbolsForm);
        }
    });

    var REMultiLevelNumberingListDialog = ASPx.CreateClass(RENumberingListDialogBase, {
        attachEvents: function() {
            RENumberingListDialogBase.prototype.attachEvents.call(this);
            dxreDialog_LbLevel.SelectedIndexChanged.AddHandler(function(s) {
                this.SetEditedLevel(s.GetValue());
            }.aspxBind(this));
            dxreDialog_TxbNumberFormat.LostFocus.AddHandler(this.OnNumberFormatChanged.aspxBind(this));
            dxreDialog_CbxNumberStyle.ValueChanged.AddHandler(this.OnNumberStyleChanged.aspxBind(this));
            dxreDialog_SpnStartAt.ValueChanged.AddHandler(this.OnStartAtChanged.aspxBind(this));
            dxreDialog_CbxNumberPosition.ValueChanged.AddHandler(this.OnNumberPositionChanged.aspxBind(this));
        },
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.CustomizeOutlineNumbered;
        },
        UpdateForm: function() {
            dxreDialog_LbLevel.SetValue(this.parameters.currentLevel);
            var level = this.GetEditedLevel();
            dxreDialog_CbxNumberStyle.SetValue(level.format);
            dxreDialog_SpnStartAt.SetValue(level.start);
            dxreDialog_CbxNumberPosition.SetValue(level.alignment);
            dxreDialog_SpnIndentAt.SetValue(this.richedit.core.units.twipsToUI(level.leftIndent));
            dxreDialog_SpnAlignedAt.SetValue(this.richedit.core.units.twipsToUI(this.GetFirstLineIndent()));
            this.UpdateSeparator();
            this.UpdateNumberFormat();
        },
        UpdateSeparator: function() {
            var separator = this.GetEditedLevel().separator;
            var value = 2;
            switch(separator) {
                case __aspxRichEdit.Utils.specialCharacters.TabMark:
                    value = 0;
                    break;
                case ' ':
                    value = 1;
                    break;
            }
            dxreDialog_CbxFollowNumberWith.SetValue(value);
        },
        OnFollowNumberWithChanged: function() {
            this.GetEditedLevel().separator = dxreDialog_CbxFollowNumberWith.GetValue();
        },
        SetEditedLevel: function(value) {
            this.parameters.currentLevel = value;
            this.UpdateForm();
        },
        GetEditedLevelIndex: function() {
            return this.parameters.currentLevel;
        }
    });

    var REHyperlinkDialogEnum = {
        WebPage: 0,
        Bookmarks: 1,
        Mail: 2
    }
    var REHyperlinkDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Hyperlink;
        },
        InitializeDialogFields: function(parameters) {
            this.ClearFilels();
            this.InitializeFields(parameters);
            this.SetLinkToValue(parameters);
            this.SetOkBtnEnabled();
            this.updateFieldsVisibility();
        },
        ClearFilels: function() {
            dxreDialog_TxbURL.SetValue("");
            dxreDialog_TxbEmailTo.SetValue("");
            dxreDialog_TxbSubject.SetValue("");
        },
        SetLinkToValue: function(parameters) {
            if(parameters.anchor)
                dxreDialog_RblLinkTo.SetValue(REHyperlinkDialogEnum.Bookmarks);
            else if(this.IsLinkMailTo(parameters.url))
                dxreDialog_RblLinkTo.SetValue(REHyperlinkDialogEnum.Mail);
            else
                dxreDialog_RblLinkTo.SetValue(REHyperlinkDialogEnum.WebPage);
        },
        InitializeFields: function(parameters) {
            this.InitializeBookmarksField(parameters);
            if(this.IsLinkMailTo(parameters.url))
                this.InitializeEmailFields(parameters.url)
            else
                this.InitializeUrlFields(parameters);
            
            if(parameters.canChangeDisplayText)
                dxreDialog_TxbText.SetValue(parameters.text);
            else
                dxreDialog_TxbText.SetEnabled(false);
            dxreDialog_TxbToolTip.SetValue(parameters.tooltip);
        },
        InitializeBookmarksField: function(parameters) {
            dxreDialog_CbBookmarkNames.Clear();
            for(var i = 0, name; name = parameters.bookmarkNames[i]; i++)
                dxreDialog_CbBookmarkNames.AddItem(name);

            if(parameters.anchor)
                dxreDialog_CbBookmarkNames.SetValue(parameters.anchor);
            else if(parameters.bookmarkNames.length)
                dxreDialog_CbBookmarkNames.SetValue(parameters.bookmarkNames[0]);
        },
        InitializeUrlFields: function(parameters) {
            dxreDialog_TxbURL.SetValue(parameters.url && !parameters.anchor ? parameters.url : constants.DefaultLinkPrefix);
        },
        InitializeEmailFields: function(url) {
            var mailtoIndex = url.toLowerCase().indexOf(constants.MailToPrefex),
                subjectIndex = url.toLowerCase().indexOf(constants.SubjectPrefix),
                endIndex = subjectIndex > -1 ? subjectIndex : url.length,
                email = url.substring(mailtoIndex + constants.MailToPrefex.length, endIndex);

            subject = subjectIndex == -1 ? "" : unescape(url.substring(subjectIndex + constants.SubjectPrefix.length));
            dxreDialog_TxbEmailTo.SetValue(email);
            dxreDialog_TxbSubject.SetValue(subject);
        },

        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_TxbURL.KeyUp.AddHandler(this.SetOkBtnEnabled.aspxBind(this));
            dxreDialog_TxbEmailTo.KeyUp.AddHandler(this.SetOkBtnEnabled.aspxBind(this));
            dxreDialog_RblLinkTo.ValueChanged.AddHandler(function() { this.updateFieldsVisibility(); }.aspxBind(this));
        },
        SetOkBtnEnabled: function() {
            var isEmail = this.IsActiveEmailForm();
            var value = isEmail ? dxreDialog_TxbEmailTo.GetText().length > 0 : dxreDialog_TxbURL.GetText().length > 0
            dxreDialog_BtnOk.SetEnabled(value);
        },
        IsLinkMailTo: function(url) {
            return url.toLowerCase().indexOf(constants.MailToPrefex) > -1;
        },
        IsActiveEmailForm: function() {
            return dxreDialog_RblLinkTo.GetValue() == REHyperlinkDialogEnum.Mail;
        },
        GetFocusedElement: function() {
            return this.IsActiveEmailForm() ? dxreDialog_TxbEmailTo : dxreDialog_TxbURL;
        },

        GetUrl: function() {
            var url = dxreDialog_TxbURL.GetText();
            if(this.IsActiveEmailForm()) {
                url = constants.MailToPrefex + dxreDialog_TxbEmailTo.GetText();
                var subject = dxreDialog_TxbSubject.GetText();
                if(subject)
                    url += constants.SubjectPrefix + subject;
            }
            return url;
        },
        GetResultParameters: function() {
            var url = this.GetUrl(),
                text = dxreDialog_TxbText.GetText(),
                tooltip = dxreDialog_TxbToolTip.GetText(),
                anchor = dxreDialog_CbBookmarkNames.GetValue(),
                canChangeDisplayText = dxreDialog_TxbText.GetEnabled();

            var returnedObject = this.GetInitInfoObject();
            returnedObject.url = "";
            returnedObject.anchor = "";
            returnedObject.text = text;
            returnedObject.tooltip = tooltip;
            returnedObject.canChangeDisplayText = canChangeDisplayText;
            switch(dxreDialog_RblLinkTo.GetValue()) {
                case REHyperlinkDialogEnum.WebPage:
                case REHyperlinkDialogEnum.Mail:
                    returnedObject.url = url;
                    break;
                case REHyperlinkDialogEnum.Bookmarks:
                    returnedObject.anchor = anchor;
                    break;
            }
            return returnedObject;
        },
        isDialogValid: function() {
            if(typeof (dxreDialog_TxbEmailTo) != "undefined" && dxreDialog_TxbEmailTo.IsVisible())
                return ASPxClientEdit.ValidateGroup("_dxeTxbEmailToGroup");
            if(typeof (dxreDialog_TxbURL) != "undefined" && dxreDialog_TxbURL.IsVisible())
                return ASPxClientEdit.ValidateGroup("_dxeTxbURLGroup");
            return true;
        },

        updateFieldsVisibility: function() {
            var val = dxreDialog_RblLinkTo.GetValue();
            InsertHyperlinkContent.GetItemByName("TxbURL").SetVisible(val == REHyperlinkDialogEnum.WebPage);
            InsertHyperlinkContent.GetItemByName("CbBookmarkNames").SetVisible(val == REHyperlinkDialogEnum.Bookmarks);
            InsertHyperlinkContent.GetItemByName("TxbEmailTo").SetVisible(val == REHyperlinkDialogEnum.Mail);
            InsertHyperlinkContent.GetItemByName("TxbSubject").SetVisible(val == REHyperlinkDialogEnum.Mail);
        }
    });

    var RETabsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Tabs;
        },
        onOkButtonClick: function() {
            if(this.isDialogValid()) {
                this.SetTab();
                this.OnComplete(1);
            }
        },
        InitializeDialogFields: function(parameters) {
            this.tabProperties = parameters.tabProperties;
            this.tabsInfo = this.tabProperties.tabsInfo;
            this.deletedTabs = [];
            this.isClearAllHappend = false;

            dxreDialog_SpnDefaultTabStops.SetValue(parameters.defaultTabStop);
            this.UpdateForm();
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_LbTabStopPosition.SelectedIndexChanged.AddHandler(function(s, e) {
                this.OnSelectedTabIndexChanged(s.GetSelectedIndex());
            }.aspxBind(this));
            dxreDialog_TxbTabStopPosition.KeyUp.AddHandler(function(s, e) {
                this.SetButtonsEnabled();
            }.aspxBind(this));
            dxreDialog_BtnTabSet.Click.AddHandler(function(s, e) {
                if(this.isDialogValid())
                    this.SetTab();
            }.aspxBind(this));
            dxreDialog_BtnTabClear.Click.AddHandler(function(s, e) {
                this.ClearTab();
            }.aspxBind(this));
            dxreDialog_BtnTabClearAll.Click.AddHandler(function(s, e) {
                this.ClearAllTab();
            }.aspxBind(this));
            dxreDialog_RblTabsAlignment.ValueChanged.AddHandler(function(s, e) {
                this.SetAlignmentValue(s.GetValue());
            }.aspxBind(this));
            dxreDialog_RblTabsLeader.ValueChanged.AddHandler(function(s, e) {
                this.SetLeaderValue(s.GetValue());
            }.aspxBind(this));
        },
        UpdateForm: function() {
            dxreDialog_LbTabStopPosition.ClearItems();
            this.forEachTabsInfo(function(tabInfo) {
                var position = this.GetRoundedPositionByTwips(tabInfo.position);
                dxreDialog_LbTabStopPosition.AddItem(position + this.richedit.unitAbbreviations.unit, position);
            }.aspxBind(this));
            this.SetSelectedTabIndex(0);
            this.UpdateClearedTabsLabel();
        },
        GetFocusedElement: function() {
            return dxreDialog_TxbTabStopPosition;
        },
        forEachTabsInfo: function(action) {
            var tabsInfoCount = this.tabsInfo.length;
            for(var i = 0; i < tabsInfoCount; i++)
                action(this.tabsInfo[i]);
        },
        SetSelectedTabIndex: function(index) {
            dxreDialog_LbTabStopPosition.SetSelectedIndex(index);
            this.OnSelectedTabIndexChanged(index);
        },
        OnSelectedTabIndexChanged: function(index) {
            var tabInfo = this.tabsInfo[index];
            if(tabInfo) {
                this.SetTabPositionValue(tabInfo.position);
                dxreDialog_RblTabsAlignment.SetValue(tabInfo.alignment);
                dxreDialog_RblTabsLeader.SetValue(tabInfo.leader);
            }
            else {
                dxreDialog_TxbTabStopPosition.SetValue("");
                dxreDialog_RblTabsAlignment.SetValue(__aspxRichEdit.TabAlign.Left);
                dxreDialog_RblTabsLeader.SetValue(__aspxRichEdit.TabLeaderType.None);
            }
            this.SetButtonsEnabled();
        },
        SetTabPositionValue: function(value) {
            dxreDialog_TxbTabStopPosition.SetValue(this.GetRoundedPositionByTwips(value));
        },
        SetButtonsEnabled: function() {
            var text = this.GetTabPositionText() + "";
            var value = text.length > 0;
            dxreDialog_BtnTabSet.SetEnabled(value);
            dxreDialog_BtnTabClear.SetEnabled(value);
        },
        GetCurrentTabsInfoIndex: function() {
            var tabPositionText = this.GetTabPositionText();
            if(tabPositionText.length != 0) {
                var currentPosition = this.GetRoundedPosition(tabPositionText);
                for(var i = 0; i < this.tabsInfo.length; i++)
                    if(this.GetRoundedPositionByTwips(this.tabsInfo[i].position) == currentPosition)
                        return i;
            }
            return -1;
        },
        SetTab: function() {
            var currentTabsInfoIndex = this.GetCurrentTabsInfoIndex();
            if(currentTabsInfoIndex > -1)
                return;
            var tabPositionText = this.GetTabPositionText();
            if(tabPositionText.length == 0)
                return;
            var currentPosition = this.richedit.core.units.UIToTwips(tabPositionText);
            var currentTabStopAlign = dxreDialog_RblTabsAlignment.GetValue();
            var currentTabStopLeader = dxreDialog_RblTabsLeader.GetValue();
            var tabInfo = new __aspxRichEdit.TabInfo(currentPosition, currentTabStopAlign, currentTabStopLeader, false, false);
            this.tabsInfo.push(tabInfo);
            this.SortTabsInfo();
            this.UpdateForm();
        },
        SetAlignmentValue: function(value) {
            var currentTabsInfoIndex = this.GetCurrentTabsInfoIndex();
            if(currentTabsInfoIndex > -1)
                this.tabsInfo[currentTabsInfoIndex].alignment = value;
        },
        SetLeaderValue: function(value) {
            var currentTabsInfoIndex = this.GetCurrentTabsInfoIndex();
            if(currentTabsInfoIndex > -1)
                this.tabsInfo[currentTabsInfoIndex].leader = value;
        },
        SortTabsInfo: function() {
            this.tabsInfo.sort(function(i1, i2) {
                if(i1.position > i2.position)
                    return 1;
                else if(i1.position < i2.position)
                    return -1;
                return 0;
            });
        },
        ClearTab: function() {
            var currentTabsInfoIndex = this.GetCurrentTabsInfoIndex();
            if(currentTabsInfoIndex < 0)
                return;
            var currentTabStopPosition = this.GetRoundedPositionByTwips(this.tabsInfo[currentTabsInfoIndex].position);
            ASPx.Data.ArrayRemove(this.deletedTabs, currentTabStopPosition);
            this.deletedTabs.push(currentTabStopPosition);
            ASPx.Data.ArrayIntegerAscendingSort(this.deletedTabs);
            ASPx.Data.ArrayRemoveAt(this.tabsInfo, currentTabsInfoIndex);
            this.UpdateForm();
        },
        UpdateClearedTabsLabel: function() {
            var text = "";
            if(!this.isClearAllHappend)
                for(var i = 0; i < this.deletedTabs.length; i++) {
                    if(text.length)
                        text += "; ";
                    text += this.deletedTabs[i] + this.richedit.unitAbbreviations.unit;
                }
            else
                text = ASPxRichEditDialogList.OtherLabels.All;
            dxreDialog_LblToBeClearedList.SetText(text);
        },
        ClearAllTab: function() {
            ASPx.Data.ArrayClear(this.tabsInfo);
            this.isClearAllHappend = true;
            this.UpdateForm();
        },
        GetTabPositionText: function() {
            return dxreDialog_TxbTabStopPosition.GetText();
        },
        GetRoundedPosition: function(position) {
            return Math.round(position * 100) / 100;
        },
        GetRoundedPositionByTwips: function(position) {
            return this.GetRoundedPosition(this.richedit.core.units.twipsToUI(position));
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.tabProperties = this.tabProperties;
            returnedObject.defaultTabStop = dxreDialog_SpnDefaultTabStops.GetValue();
            return returnedObject;
        },
        isDialogValid: function() {
            return dxreDialog_TxbTabStopPosition.GetIsValid();
        }
    });

    var RESymbolsDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Symbols;
        },
        InitializeDialogFields: function(parameters) {
            dxreDialog_CbxFontName.SetText(parameters.fontName);
            this.CreateListBox();
            this.UpdateSymbolList(parameters.fontName);
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_CbxFontName.ValueChanged.AddHandler(function(s, e) {
                this.OnFontNameChanged(s.GetValue());
            }.aspxBind(this));
        },
        CreateListBox: function() {
            this.listBox = new DialogListBox(this, "symbol", this.GetSymbolListElement());
        },
        UpdateSymbolList: function(fontName) {
            this.SendCallbackForContent(fontName);
        },
        SendCallbackForContent: function(fontName) {
            this.ShowLoadingPanel();
            this.SetEnabledForComboBox(false);
            this.SetEnabledForSelectButton(false);
            this.GetCallbackHelper().SendCallback(constants.SymbolListCallbackPrefix, fontName);
        },
        ShowLoadingPanel: function() {
            var offsetElement = this.GetSymbolListElement();
            this.richedit.CreateLoadingDiv(document.body, offsetElement);
            this.richedit.CreateLoadingPanelWithAbsolutePosition(document.body, offsetElement);
        },
        SetEnabledForComboBox: function(value) {
            dxreDialog_CbxFontName.SetEnabled(value);
        },
        SetEnabledForSelectButton: function(value) {
            dxreDialog_BtnOk.SetEnabled(value);
        },
        CreateItems: function(symbolList) {
            this.listBox.ClearItems();
            this.listBox.SetStyles({ "font-family": this.GetCurrentFontName() });
            for(var i = 0; i < symbolList.length; i++)
                this.listBox.AddItem(symbolList[i]);
        },
        OnFontNameChanged: function(fontName) {
            this.UpdateSymbolList(fontName);
        },
        OnCallbackForContent: function(result) {
            var symbolList = eval(result);
            this.CreateItems(symbolList);
        },
        OnEndCallbackForContent: function() {
            this.SetEnabledForComboBox(true);
        },
        OnValueChanged: function(value) {
            this.SetEnabledForSelectButton(true);
        },
        GetCallbackHelper: function() {
            if(!this.callbackHelper)
                this.callbackHelper = new CallbackHelper(this);
            return this.callbackHelper;
        },
        GetSymbolListElement: function() {
            var item = dxreDialog_FormLayout.GetItemByName("SymbolList");
            return dxreDialog_FormLayout.GetHTMLElementByItem(item).firstChild;
        },
        GetCurrentSymbol: function() {
            return this.listBox.GetValue();
        },
        GetCurrentFontName: function() {
            return dxreDialog_CbxFontName.GetValue();
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.fontName = this.GetCurrentFontName();
            returnedObject.symbol = this.GetCurrentSymbol();
            return returnedObject;
        },
        GetFocusedElement: function() {
            return dxreDialog_CbxFontName;
        }
    });

    var REInsertMergeFieldDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.InsertMergeField;
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.fieldName = dxreDialog_LsbFields.GetValue();
            return returnedObject;
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_BtnInsert.Click.AddHandler(function() {
                this.DoCustomAction(true);
            }.aspxBind(this));
            dxreDialog_LsbFields.ItemDoubleClick.AddHandler(function() {
                this.DoCustomAction(true);
            }.aspxBind(this));
        }
    });

    var REFinishAndMergeDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.ExportRange;
        },
        InitializeDialogFields: function(parameters) {
            dxreDialog_RdBttnListSwitcher.SetValue(parameters.range);
            dxreDialog_SpnFrom.SetValue(parameters.exportFrom);
            dxreDialog_SpnCount.SetValue(parameters.exportRecordsCount);
            dxreDialog_CmbMergeMode.SetValue(parameters.mergeMode);

            this.toggleElements();
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_RdBttnListSwitcher.ValueChanged.AddHandler(this.toggleElements.aspxBind(this));
        },
        onOkButtonClick: function() {
            this.ShowSaveDialog();
        },
        ShowSaveDialog: function() {
            var clientCommand = __aspxRichEdit.RichEditClientCommand.ShowSaveMergedDocumentForm;
            var parameters = this.GetResultParameters();
            this.OnComplete(0);
            this.richedit.core.commandManager.getCommand(clientCommand).execute(parameters);
        },
        toggleElements: function() {
            var exportRange = this.GetExportRangeValue();
            dxreDialog_SpnFrom.SetEnabled(exportRange === __aspxRichEdit.MailMergeExportRange.Range);
            dxreDialog_SpnCount.SetEnabled(exportRange === __aspxRichEdit.MailMergeExportRange.Range);
            dxreDialog_CmbMergeMode.SetEnabled(exportRange !== __aspxRichEdit.MailMergeExportRange.CurrentRecord);
        },
        GetResultParameters: function() {
            var returnedObject = this.GetInitInfoObject();
            returnedObject.range = this.GetExportRangeValue();
            returnedObject.exportFrom = dxreDialog_SpnFrom.GetValue();
            returnedObject.exportRecordsCount = dxreDialog_SpnCount.GetValue();
            returnedObject.mergeMode = dxreDialog_CmbMergeMode.GetValue();
            return returnedObject;
        },
        GetExportRangeValue: function() {
            return dxreDialog_RdBttnListSwitcher.GetValue();
        }
    });

    var REBookmarksDialog = ASPx.CreateClass(ASPxRichEditDialog, {
        newBookmarkName: "",
        sortByName: false,

        bookmarks: [],

        GetDialogCaptionText: function() {
            return ASPxRichEditDialogList.Titles.Bookmark;
        },
        InitializeDialogFields: function(parameters) {
            this.clear();
            for(var i = 0, bookmark; bookmark = parameters.bookmarks[i]; i++) {
                bookmark.deleted = false;
                this.bookmarks.push(bookmark);
            }

            this.updateDataSource();
        },
        clear: function() {
            this.bookmarks = [];
            this.newBookmarkName = "";
        },

        GetFocusedElement: function() {
            if(dxreDialog_LsbBookmarkNames.GetItemCount() > 0)
                return dxreDialog_LsbBookmarkNames;
            return dxreDialog_TxbBookmarksName;
        },

        getDataSource: function() {
            var array = [];
            if(this.sortByName) {
                this.bookmarks = this.bookmarks.sort(function(b1, b2) {
                    if(b1.name > b2.name)
                        return 1;
                    else if(b1.name < b2.name)
                        return -1;
                    return 0;
                });

            }
            else {
                this.bookmarks = this.bookmarks.sort(function(b1, b2) {
                    if(b1.start > b2.start)
                        return 1;
                    else if(b1.start < b2.start)
                        return -1;
                    return 0;
                });
            }

            for(var i = 0, bookmark; bookmark = this.bookmarks[i]; i++)
                if(!bookmark.deleted)
                    array.push(bookmark.name);

            return array;
        },
        updateDataSource: function() {
            dxreDialog_LsbBookmarkNames.ClearItems();

            var dataSource = this.getDataSource();
            for(var i = 0, name; name = dataSource[i]; i++)
                dxreDialog_LsbBookmarkNames.AddItem(name);
            
            var lastIndex = dxreDialog_LsbBookmarkNames.GetItemCount() - 1;
            if(lastIndex > -1) {
                dxreDialog_LsbBookmarkNames.SetSelectedIndex(lastIndex);
                dxreDialog_TxbBookmarksName.SetValue(dxreDialog_LsbBookmarkNames.GetValue());
            }
            this.updateEnableState();
        },

        checkIsValidChar: function(char) {
            var punctuationChars = '`~!@#$%^&*()_+{}|:"<>?-=[]\;\'.\/,';
            if(/\d/.test(char))
                return false;
            if(punctuationChars.indexOf(char) > -1)
                return false;
            return true;
        },

        updateEnableState: function() {
            var value = dxreDialog_TxbBookmarksName.GetValue();
            var btnAddEnable = !!value;
            if (btnAddEnable)
                btnAddEnable = this.checkIsValidChar(value[0]);
            if (btnAddEnable)
                btnAddEnable = value.indexOf(' ') == -1;

            dxreDialog_BtnAdd.SetEnabled(btnAddEnable);

            var enable = false;
            for(var i = 0, listItem; listItem = dxreDialog_LsbBookmarkNames.GetItem(i); i++)
                if(value == listItem.value) {
                    enable = true;
                    break;
                }
            dxreDialog_BtnDelete.SetEnabled(enable);
            dxreDialog_BtnGoTo.SetEnabled(enable);
        },
        attachEvents: function() {
            ASPxRichEditDialog.prototype.attachEvents.call(this);
            dxreDialog_TxbBookmarksName.KeyUp.AddHandler(function() { this.updateEnableState(); }.aspxBind(this));
            dxreDialog_BtnAdd.Click.AddHandler(function() { this.onBtnAddClick(); }.aspxBind(this));
            dxreDialog_BtnDelete.Click.AddHandler(function() { this.onBtnDeleteClick(); }.aspxBind(this));
            dxreDialog_BtnCancel.Click.AddHandler(function() { this.OnComplete(1); }.aspxBind(this));
            dxreDialog_RblSortBy.ValueChanged.AddHandler(function(s, e) { this.onRblSortByValueChanged(s, e); }.aspxBind(this));
            dxreDialog_BtnGoTo.Click.AddHandler(function() { this.onBtnGoToClick(); }.aspxBind(this));
        },
        onBtnGoToClick: function() {
            var name = dxreDialog_LsbBookmarkNames.GetValue();
            this.richedit.core.commandManager.getCommand(__aspxRichEdit.RichEditClientCommand.GoToBookmark).execute(name);
        },
        onRblSortByValueChanged: function(s, e) {
            this.sortByName = !!!s.GetValue();
            this.updateDataSource();
        },
        onBtnAddClick: function() {
            this.newBookmarkName = dxreDialog_TxbBookmarksName.GetValue();
            this.OnComplete(1);
        },
        onBtnDeleteClick: function() {
            var name = dxreDialog_LsbBookmarkNames.GetValue();
            for(var i = 0, bookmark; bookmark = this.bookmarks[i]; i++)
                if(bookmark.name == name)
                    bookmark.deleted = true;
            this.updateDataSource();
        },

        GetResultParameters: function () {
            var returnedObject = this.GetInitInfoObject();
            var deletedBookmarkNames = [];

            for(var i = 0, bookmark; bookmark = this.bookmarks[i]; i++)
                if(bookmark.deleted)
                    deletedBookmarkNames.push(bookmark.name);

            returnedObject.newBookmarkName = this.newBookmarkName;
            returnedObject.deletedBookmarkNames = deletedBookmarkNames;
            return returnedObject;
        }
    });

    var CallbackHelper = ASPx.CreateClass(null, {
        constructor: function(owner) {
	        this.owner = owner;
            this.callbackCount = 0;
        },
        SendCallback: function(prefix, args) {
            var sendCallback = function() {
                this.owner.richedit.sendInternalServiceCallback(prefix, args, this);
                this.callbackCount++;
            }.aspxBind(this);
            window.setTimeout(sendCallback, 10);
        },
        OnCallback: function(result) {
            this.owner.OnCallbackForContent(result);
  	    },
        OnEndCallback: function() {
            if(this.callbackCount > 0)
                this.callbackCount--;
            if(this.callbackCount <= 0)
                this.owner.richedit.clearOwnerControlCallback();
            this.owner.OnEndCallbackForContent();
        }
    });

    var DialogListBox = ASPx.CreateClass(null, {
        constructor: function(owner, id, mainElement) {
            this.owner = owner;
            this.id = id;
            this.mainElement = mainElement;

            this.items = [];
            this.itemsCount = 0;
            this.selectedItem = null;

            this.Initialize();
        },
        Initialize: function() {
            this.InitializeEventHandlers();
            ASPx.AddClassNameToElement(this.GetMainElement(), constants.DialogListBoxClass);
        },
        InitializeEventHandlers: function() {
            ASPx.Evt.AttachEventToElement(this.GetMainElement(), "mousedown", function(evt) { this.OnMainContainerClick(evt); }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(this.GetMainElement(), "dblclick", function(evt) { this.OnMainContainerDblClick(evt); }.aspxBind(this));
        },
        AddItem: function(value) {
            var item = new DialogListBoxItem(this, this.itemsCount++, value);
            this.items[item.id] = item;
        },
        ClearItems: function() {
            this.forEachItem(function(item) {
                var itemId = item.id;
                ASPx.GetStateController().RemoveSelectedItem(itemId);
                ASPx.GetStateController().RemoveHoverItem(itemId);
            }.aspxBind(this));

            ASPx.SetInnerHtml(this.GetMainElement(), "");

            this.items = [];
            this.selectedItem = null;
            this.itemsCount = 0;
        },
        OnMainContainerClick: function(evt) {
            if(!ASPx.Evt.IsLeftButtonPressed(evt))
                return;
            var item = this.GetItemByEvent(evt);
            if(item)
                this.SetSelectedItem(item);
        },
        OnMainContainerDblClick: function(evt) {
            var item = this.GetItemByEvent(evt);
            if(item)
                ASPx.DialogComplete(1);
        },
        GetItemByEvent: function(evt) {
            var sourceElement = ASPx.GetParentByTagName(ASPx.Evt.GetEventSource(evt), "DIV");
            while(!sourceElement.id)
                sourceElement = sourceElement.parentNode;
            return this.items[sourceElement.id];
        },
        SetSelectedItem: function(item) {
            if(this.selectedItem)
                this.selectedItem.Unselect();
            this.selectedItem = item;
            item.Select();
            this.owner.OnValueChanged(this.GetValue());
        },
        forEachItem: function(action) {
            for(var item in this.items)
                if(this.items.hasOwnProperty(item) && action(this.items[item]))
                    return;
        },
        SetStyles: function(styles) {
            ASPx.SetStyles(this.GetMainElement(), styles);
        },
        SetSelectedIndex: function(index) {
            if(index < -1 && index >= this.itemsCount)
                return;
            if(index == -1 && this.selectedItem) {
                this.selectedItem = null;
                this.selectedItem.Unselect();
                return;
            }
            this.forEachItem(function(item) {
                if(item.index == index) {
                    this.SetSelectedItem(item);
                    return;
                }
            }.aspxBind(this));
        },
        SetSelectedValue: function(value) {
            this.forEachItem(function(item) {
                if(item.HasEqualValue(value)) {
                    this.SetSelectedItem(item);
                    return;
                }
            }.aspxBind(this));
        },
        GetSelectedItem: function() {
            return this.selectedItem;
        },
        GetValue: function() {
            return this.selectedItem ? this.selectedItem.GetValue() : undefined;
        },
        GetSelectedIndex: function() {
            return this.selectedItem ? this.selectedItem.index : -1;
        },
        GetMainElement: function() {
            return this.mainElement;
        }
    });

    var DialogListBoxItem = ASPx.CreateClass(null, {
        constructor: function(owner, index, value) {
            this.owner = owner;
            this.index = index;
            this.value = value;
            this.richedit = this.owner.owner.richedit;
            this.id = this.richedit.name + "_" + this.owner.id + "_" + index;

            this.CreateElement();
            this.PrepareElementState();
        },
        CreateElement: function() {
            var element = this.GetElementWithContent();
            element.id = this.id;
            var parentElement = this.owner.GetMainElement();
            parentElement.appendChild(element);
        },
        PrepareElementState: function() {
            ASPx.GetStateController().AddSelectedItem(
                this.id,
                [constants.ListSelectedClass],
                [""],
                null,
                null,
                null
            );
            ASPx.GetStateController().AddHoverItem(
                this.id,
                [constants.ListHoverClass],
                [""],
                null,
                null,
                null
            );
        },
        Select: function() {
            ASPx.GetStateController().SelectElementBySrcElement(this.GetElement());
        },
        Unselect: function() {
            ASPx.GetStateController().DeselectElementBySrcElement(this.GetElement());
        },
        HasEqualValue: function(value) {
            return this.GetValue() === value;
        },
        GetValue: function() {
            return this.value;
        },
        GetElementWithContent: function() {
            var element = ASPx.CreateHtmlElement();
            ASPx.SetInnerHtml(element, this.GetContent());
            return element;
        },
        GetContent: function() {
            return this.GetValue();
        },
        GetElement: function() {
            return ASPx.GetElementById(this.id);
        }
    });

    var NumberingListBox = ASPx.CreateClass(DialogListBox, {
        constructor: function(owner, id, mainElement) {
            this.constructor.prototype.constructor.call(this, owner, id, mainElement);
        },
        AddItem: function(value) {
            var item = new NumberingListBoxItem(this, this.itemsCount++, value);
            this.items[item.id] = item;
        }
    });

    var NumberingListBoxItem = ASPx.CreateClass(DialogListBoxItem, {
        GetElementWithContent: function() {
            return this.IsNoneItem() ? DialogListBoxItem.prototype.GetElementWithContent.apply(this) : this.GetPreview();
        },
        GetContent: function() {
            return ASPxRichEditDialogList.OtherLabels.None;
        },
        HasEqualValue: function(abstractNumberingList) {
            var currentValue = this.GetValue();
            return abstractNumberingList !== null && currentValue !== null ? abstractNumberingList.externallyEquals(currentValue) : abstractNumberingList === currentValue;
        },
        IsNoneItem: function() {
            return this.GetValue() == null;
        },
        GetPreview: function() {
            var previewElement = ASPx.CreateHtmlElement();
            var previewHelper = new __aspxRichEdit.NumberingListFormPreviewHelper(this.richedit.core, this.GetValue());
            var previewContentElement = previewHelper.createPreview();
            ASPx.AddClassNameToElement(previewContentElement, constants.NumberingListPreviewClass);
            previewElement.appendChild(previewContentElement);
            return previewElement;
        }
    });

    var DialogBorderLine = ASPx.CreateClass(null, {
        constructor: function(owner, borderInfo, line, controlLine, controlBtn) {
            this.owner = owner;
            this.borderInfo = borderInfo;
            this.line = line;
            this.controlLine = controlLine;
            this.controlBtn = controlBtn;
            this.Initialize();
        },
        Initialize: function() {
            this.InitializeEventHandlers();
            if(this.borderInfo !== null && this.borderInfo.style !== __aspxRichEdit.BorderLineStyle.None) {
                this.setBorderForLineElement();
                this.controlBtn.SetChecked(true);
            }
        },
        InitializeEventHandlers: function() {
            ASPx.Evt.AttachEventToElement(this.controlLine, "click", function() {
                if(!this.isIdenticalBorderInfo())
                    this.setBorder();
                else
                    this.resetBorder();
                this.owner.setCustomPreset();
            }.aspxBind(this));

            this.controlBtn.Click.AddHandler(function(s) {
                if(s.GetChecked())
                    this.setBorder();
                else
                    this.resetBorder();
            }.aspxBind(this));
        },
        updateBordersInfoValues: function() {
            this.borderInfo.color = this.owner.getBorderColorValue();
            this.borderInfo.width = this.owner.getBorderWidthValue();
            this.borderInfo.style = this.owner.getBorderStyleValue();
        },
        isIdenticalBorderInfo: function() {
            return this.borderInfo.color === this.owner.getBorderColorValue() &&
                this.borderInfo.width === this.owner.getBorderWidthValue() &&
                this.borderInfo.style === this.owner.getBorderStyleValue();
        },
        setBorder: function() {
            if(this.borderInfo === null)
                this.borderInfo = new __aspxRichEdit.DialogBorderInfo();
            this.updateBordersInfoValues();
            this.setBorderForLineElement();
            if(!this.controlBtn.GetChecked())
                this.controlBtn.SetChecked(true);
        },
        resetBorder: function() {
            this.borderInfo.style = __aspxRichEdit.BorderLineStyle.None;
            ASPx.Attr.RemoveAllStyles(this.line);
            if(this.controlBtn.GetChecked())
                this.controlBtn.SetChecked(false);
        },
        setDefaultBorder: function() {
            this.borderInfo.width = 0.75; //TODO!!
            this.borderInfo.style = __aspxRichEdit.BorderLineStyle.Single;
            this.borderInfo.color = this.owner.getBorderColorValue();
            this.setBorderForLineElement();
            this.controlBtn.SetChecked(true);
        },
        setBorderForLineElement: function() {
            var borderToUpdate = ASPx.ElementHasCssClass(this.line, constants.BorderVerticalLineClass) ? "borderRight" : "borderBottom";
            var style = ASPx.GetCurrentStyle(this.line);

            var oldWidth = ASPx.PxToInt(style[borderToUpdate + "Width"]);
            var newWidth = __aspxRichEdit.UnitConverter.pointsToPixels(this.borderInfo.width);
            newWidth = newWidth > 0 ? Math.floor(newWidth) : 1;

            this.line.style[borderToUpdate + "Width"] = newWidth + "px";
            this.line.style[borderToUpdate + "Color"] = this.borderInfo.color;
            this.line.style[borderToUpdate + "Style"] = this.getBorderStyleForHtml(this.borderInfo.style);

            var widthDifference = newWidth - oldWidth;
            var differenceToMove = widthDifference >= 0 ? Math.ceil(widthDifference / 2) : Math.floor(widthDifference / 2);
            var position;
            if(ASPx.ElementHasCssClass(this.line, constants.BorderCenterLineClass)) {
                position = ASPx.GetAbsolutePositionX(this.line);
                ASPx.SetAbsoluteX(this.line, position - differenceToMove);
            }
            if(ASPx.ElementHasCssClass(this.line, constants.BorderMiddleLineClass)) {
                position = ASPx.GetAbsolutePositionY(this.line);
                ASPx.SetAbsoluteY(this.line, position - differenceToMove);
            }
        },
        getBorderStyleForHtml: function(style) {
            switch(style) {
                case __aspxRichEdit.BorderLineStyle.Single:
                    return "solid";
                case __aspxRichEdit.BorderLineStyle.Dotted:
                    return "dotted";
                case __aspxRichEdit.BorderLineStyle.Dashed:
                    return "dashed";
                case __aspxRichEdit.BorderLineStyle.Double:
                    return "double";
            }
        }
    });

    function normolizeVirtualFolderPath(folderPath) {
        folderPath = folderPath.split("/").join("\\");
        if(folderPath.length == 0 || folderPath == "\\")
            return "";
        if(folderPath.substr(folderPath.length - 1, 1) != "\\")
            return folderPath + "\\";
        return folderPath
    }
    function aspxTestExistingImageOnLoad(name) {
        var richedit = ASPx.GetControlCollection().Get(name);
        var curDialog = richedit != null ? ASPx.Dialog.GetLastDialog(richedit) : null;
        if(curDialog != null) curDialog.OnLoadTestExistingImage();
    }
    function aspxTestExistingImageOnError(name) {
        var richedit = ASPx.GetControlCollection().Get(name);
        var curDialog = richedit != null ? ASPx.Dialog.GetLastDialog(richedit) : null;
        if(curDialog != null) curDialog.OnErrorTestExistingImage();
    }

    var ASPxRichEditDialogList = {};
    ASPxRichEditDialogList["FileSaveAs"] = new RESaveAsDialog("savefiledialog");
    ASPxRichEditDialogList["FileOpen"] = new REOpenFileDialog("openfiledialog");
    ASPxRichEditDialogList["EditFont"] = new REFontDialog("fontdialog");
    ASPxRichEditDialogList["EditParagraph"] = new REParagraphDialog("paragraphdialog");
    ASPxRichEditDialogList["PageSetup"] = new REPageSetupDialog("pagesetupdialog");
    ASPxRichEditDialogList["Columns"] = new REColumnsDialog("columnsdialog");
    ASPxRichEditDialogList["InsertTable"] = new REInsertTableDialog("inserttabledialog");
    ASPxRichEditDialogList["InsertImage"] = new REInsertImageDialog("insertimagedialog");
    ASPxRichEditDialogList["ErrorMessage"] = new REErrorDialog("errordialog");
    ASPxRichEditDialogList["NumberingList"] = new RENumberingListDialog("numberinglistdialog");
    ASPxRichEditDialogList["Hyperlink"] = new REHyperlinkDialog("hyperlinkdialog");
    ASPxRichEditDialogList["Tabs"] = new RETabsDialog("tabsdialog");
    ASPxRichEditDialogList["SimpleNumberingList"] = new RESimpleNumberingListDialog("simplenumberinglistdialog");
    ASPxRichEditDialogList["BulletedList"] = new REBulletedListDialog("bulletedlistdialog");
    ASPxRichEditDialogList["MultiLevelNumberingList"] = new REMultiLevelNumberingListDialog("multilevelnumberinglistdialog");
    ASPxRichEditDialogList["Symbols"] = new RESymbolsDialog("symbolsdialog");
    ASPxRichEditDialogList["InsertMergeField"] = new REInsertMergeFieldDialog("insertmergefielddialog");
    ASPxRichEditDialogList["FinishAndMerge"] = new REFinishAndMergeDialog("finishandmergedialog");
    ASPxRichEditDialogList["Bookmarks"] = new REBookmarksDialog("bookmarksdialog");
    ASPxRichEditDialogList["TableProperties"] = new RETablePropertiesDialog("tablepropertiesdialog");
    ASPxRichEditDialogList["InsertTableCells"] = new REInsertTableCellsDialog("inserttablecellsdialog");
    ASPxRichEditDialogList["DeleteTableCells"] = new REDeleteTableCellsDialog("deletetablecellsdialog");
    ASPxRichEditDialogList["SplitTableCells"] = new RESplitTableCellsDialog("splittablecellsdialog");
    ASPxRichEditDialogList["BorderShading"] = new REBorderShadingDialog("bordershadingdialog");

    ASPxClientRichEdit.ASPxRichEditDialogList = ASPxRichEditDialogList;
    ASPxClientRichEdit.getActiveDialog = function() {
        var richedit = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        return richedit != null ? ASPx.Dialog.GetLastDialog(richedit) : null;
    };

    function executeIfExists(name, execFunc) {
        var element = window[name];
        if(element && element.IsDOMInitialized())
            return execFunc(element);
        return false;
    };

    window.aspxTestExistingImageOnLoad = aspxTestExistingImageOnLoad;
    window.aspxTestExistingImageOnError = aspxTestExistingImageOnError;
})();