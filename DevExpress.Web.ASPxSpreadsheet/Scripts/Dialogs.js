(function() {
    // TODO [Seleznyov] think about some common ASPxSpreadsheet prefix for the dialog controls (_dxTxbFindWhat -> dxssFindWhat) ?
    // TODO [Seleznyov] refactor to the new standard capitalization? (privateMethod, PublicMethod)
    
    var SpreadsheetDialogs = {
        InsertHyperlink             : { Command : "InsertHyperlink", Name : "inserthyperlinkdialog" },
        InsertPicture               : { Command : "InsertPicture", Name : "insertpicturedialog" },
        RenameSheet                 : { Command : "RenameSheet", Name : "renamesheetdialog"},
        EditHyperlink               : { Command : "EditHyperlink", Name : "inserthyperlinkdialog"},
        FileOpen                    : { Command : "FileOpen", Name : "openfiledialog"},
        FileSaveAs                  : { Command : "FileSaveAs", Name : "savefiledialog"},
        RowHeight                   : { Command : "FormatRowHeight", Name : "rowheightdialog"},
        ColumnWidth                 : { Command : "FormatColumnWidth", Name : "columnwidthdialog"},
        DefaultColumnWidth          : { Command : "FormatDefaultColumnWidth", Name : "defaultcolumnwidthdialog"},
        UnhideSheet                 : { Command : "UnhideSheet", Name : "unhidesheetdialog"},
        ChartType                   : { Command : "ChartChangeType", Name : "changecharttypedialog"},
        ChartData                   : { Command : "ChartSelectData", Name : "chartselectdatadialog"},
        ChartLayout                 : { Command : "ModifyChartLayout", Name : "modifychartlayoutdialog"},
        ChartTitle                  : { Command : "ChartChangeTitle", Name : "chartchangetitledialog"},
        ChartHorizontalAxisTitle    : { Command : "ChartChangeHorizontalAxisTitle", Name : "chartchangehorizontalaxistitledialog"},
        ChartVerticalAxisTitle      : { Command : "ChartChangeVerticalAxisTitle", Name : "chartchangeverticalaxistitledialog"},
        ChartStyle                  : { Command : "ModifyChartStyle", Name : "modifychartstyledialog"},
        FindAll                     : { Command : "FindAll", Name : "findandreplacedialog"},
        InsertTable                 : { Command : "InsertTable", Name: "tableselectdatadialog"},
        InsertTableWithStyle        : { Command : "InsertTableWithStyle", Name: "tableselectdatadialog"},
        DataFilterSimple            : { Command : "DataFilterSimple", Name: "datafiltersimpledialog" },
        CustomDataFilter            : { Command : "CustomDataFilter", Name: "customdatafilterdialog" },
        CustomDateTimeFilter        : { Command : "CustomDateTimeFilter", Name: "customdatetimefilterdialog" },
        DataFilterTop10             : { Command : "DataFilterTop10", Name: "datafiltertop10dialog" },
        DataValidation              : { Command : "ApplyDataValidation", Name: "datavalidationdialog" },
        ValidationConfirm           : { Name: "validationconfirmdialog" },
        MoveOrCopySheet             : { Command : "MoveOrCopySheet", Name: "moveorcopysheetdialog" },
        ModifyTableStyle            : { Command : "ModifyTableStyle", Name: "modifytablestyledialog" },
        FormatAsTable               : { Command : "FormatAsTable", Name: "modifytablestyledialog" },
        PageSetup                   : { Command : "PageSetup", Name: "pagesetupdialog" }
    };

    var SpreadsheetDialog = ASPx.CreateClass(ASPx.Dialog, {
        Execute: function(ownerControl, commandId) {
            this.spreadsheet = ownerControl;
            this.SetDialogAsCurrent();
            this.isOnCallbackError = false;
            this.commandId = commandId;

            ASPx.Dialog.prototype.Execute.call(this, ownerControl);
        },
        OnCallbackError: function(result, data) {
            this.isOnCallbackError = true;
            this.ClearCurrentDialog();
            this.HideDialog(null, true);
            ASPx.Dialog.prototype.OnCallbackError.call(this, result);
        },
        OnClose: function() {
            if(this.spreadsheet.isInFullScreenMode)
                this.spreadsheet.hideBodyScroll();
            ASPx.Dialog.prototype.OnClose.call(this);
            this.ClearCurrentDialog();
            this.focusKeyBoardSupportInput();
            if(this.canRemoveDialogHtmlAfterClose())
                setTimeout(function() { this.GetDialogPopup().SetContentHtml(""); }.aspxBind(this), 0);
        },
        canRemoveDialogHtmlAfterClose: function() {
            return true;
        },
        OnInitComplete: function() {
            ASPx.Dialog.prototype.OnInitComplete.call(this);
            this.GetDialogPopup().UpdatePosition();
            var popupElement = this.GetDialogPopup().GetWindowElement(-1);
            if(popupElement && popupElement.style.width)
                ASPx.Attr.RemoveAttribute(popupElement.style, "width");

            window.setTimeout(function() { this.SetClientModality(this.getDialogModal()); }.aspxBind(this), 100);            
        },
        ClearCurrentDialog: function() {
            this.spreadsheet.UpdateStateObjectWithObject({ currentDialog: "" });
        },
        SetDialogAsCurrent: function() {
            this.spreadsheet.UpdateStateObjectWithObject({ currentDialog: this.name });
        },
        SendCallback: function(callbackArgs) {
            this.spreadsheet.callbackOwner = this;
            this.spreadsheet.CreateCallback(callbackArgs, "", false);
            this.ShowLoadingPanelOverDialogPopup();
        },
        DoCustomAction: function(result, params) {
            if(result)
                this.DialogCompletedSuccessfully(params);
        },
        getControlInstance: function(name) {
            return window[this.spreadsheet.name + "_" + this.name + name];
        },
        //virtual 
        DialogCompletedSuccessfully: function(params) {
        },
        ShowLoadingPanelOverDialogPopup: function() {
            var offsetElement = this.spreadsheet.GetMainElement();
            this.spreadsheet.CreateLoadingDiv(document.body, offsetElement);
            this.spreadsheet.CreateLoadingPanelWithAbsolutePosition(document.body, offsetElement);
        },
        SendCallbackForDialogContent: function() {
            this.SendCallbackViaOwner(ASPx.dialogFormCallbackStatus, this.name, false);
        },
        SendCallbackViaOwner: function(callbackPrefix, arg, hideLoadingPanel) {
            this.spreadsheet.sendInternalServiceCallback(callbackPrefix, arg, this, hideLoadingPanel);
        },
        SetClientModality: function(isModal) {
            this.GetDialogPopup().SetClientModality(isModal);
        },
        GetDialogContent: function(name) {
            if(this.GetForceUpdateFlage()) {
                this.DropForceUpdateFlag();
                return null;
            }
            return ASPx.Dialog.prototype.GetDialogContent.call(this, name);
        },
        SetForceUpdateFlag: function() {
            this.GetForceUpdateCollection()[this.name] = true;
        },
        GetForceUpdateFlage: function() {
            return this.GetForceUpdateCollection()[this.name] || null;
        },
        DropForceUpdateFlag: function() {
            this.GetForceUpdateCollection()[this.name] = false;
        },
        GetForceUpdateCollection: function() {
            return this.spreadsheet.forceUpdateDialogCollection;
        },
        focusKeyBoardSupportInput: function() {
            var inputController = this.spreadsheet.getInputController();
            if(inputController)
                inputController.captureFocus();
        },
        getDialogModal: function() {
            return true;
        },
        switchRibbonEnabledMode: function(enabled) {
            var ribbonControl = this.spreadsheet.GetRibbon();
            if(ribbonControl)
                ribbonControl.SetEnabled(enabled);
            this.spreadsheet.onEditingStopped();
        },
        switchContextMenuEnableMode: function(enabled) {
            var contextMenu = this.spreadsheet.getPopupMenu();
            if(contextMenu)
                contextMenu.SetEnabled(enabled);
        },
        showLoadingPanel: function() {
            var dialogElement = this.GetDialogPopup().GetWindowContentElement(-1);
            var offsetElement = this.spreadsheet.GetMainElement();
            this.spreadsheet.CreateLoadingDiv(document.body, offsetElement);
            this.spreadsheet.CreateLoadingPanelWithAbsolutePosition(document.body, dialogElement);
        },
        beginInitializeInternalControls: function() {
            this.showLoadingPanel();
        },
        endInitializeInternalControls: function() {
            this.HideLoadingPanelOverDialogPopup();
        },
        getCommandIdByName: function(commandName) {
            return ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(commandName).id;
        },
        executeCommand: function(commandName, commandArgs) {
            this.spreadsheet.onServerCommand(this.getCommandIdByName(commandName), commandArgs);
        }, 
        clearParentControlCallbackOwner: function() {
            this.spreadsheet.callbackOwner = null;
        },
        initializeSubmitButton: function(submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function(cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function() {
            if(this.isDialogValid())
                this.OnComplete(1, this.getDialogData());
        },
        dialogComplete: function(result, params) {
            var curDialog = ASPx.Dialog.GetLastDialog(this.spreadsheet);
	        if(curDialog != null)
	            return curDialog.OnComplete(result, params);
        },
        closeDialog: function() {
            this.OnComplete(0, null);
        },
        // virtual
        isDialogValid: function() {
            return true;
        },
        // abstract
        getDialogData: function() {
        }
    });

    // ** RenameSheet **
    var RenameSheetDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(params) {
            if(params.newName != this.currentSheetName)
                this.executeCommand(SpreadsheetDialogs.RenameSheet.Command, { NewName: params.newName });
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.RenameSheet;
        },
        getSheetNameTextBox: function() {
            return this.getControlInstance("_dxTxbSheetName");
        },
        GetInitInfoObject: function() {
            this.currentSheetName = this.spreadsheet.getCurrentSheetName();
            return this.currentSheetName;
        },
        InitializeDialogFields: function(currentSheetName) {
            this.getSheetNameTextBox().SetValue(currentSheetName);
        },
        SetFocusInField: function() {
            var sheetNameTextBox = this.getSheetNameTextBox();
            if(sheetNameTextBox)
                _aspxSetFocusToElement(sheetNameTextBox.name);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxTxbSheetNameGroup");
        },
        getDialogData: function () {
            return {
                newName: this.getSheetNameTextBox().GetValue()
            };
        }
    });

    // ** InsertHyperlinkDialog **
    var URLConstants = {
        DefaultLinkPrefix : "http://",
        MailToPrefix : "mailto:",
        SubjectPrefix : "?subject="
    };

    var InsertHyperlinkDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function (params) {
            this.executeCommand(SpreadsheetDialogs.InsertHyperlink.Command, { HyperLinkDisplayText: params.text, HyperLinkScreenTip: params.title, HyperLinkUrlAddress: params.url });
        },
        GetDialogCaptionText: function() {
            return this.GetInitInfoObject().hasLink ? SpreadsheetDialog.Titles.ChangeLink : SpreadsheetDialog.Titles.InsertLink;
        },
        GetInitInfoObject: function() {
            return this.getHyperLinkInformation();
        },
        InitializeDialogFields: function(rangeInfo) {
            var btnEmail = this.getEmailRadioButton(),
                btnUrl = this.getUrlRadioButton(),
                btnChange = this.getChangeButton(),
                btnInsert = this.getInsertButton();

            btnChange.SetVisible(rangeInfo.hasLink);
            btnInsert.SetVisible(!rangeInfo.hasLink);

            if(rangeInfo.linkInfo.isLinkMailTo) {
                var txtEmail = this.getEmailToTextBox(),
                    txtSubject = this.getSubjectTextBox();

                if(txtEmail) {
                    txtEmail.SetValue(rangeInfo.linkInfo.href);
                    txtSubject.SetValue(rangeInfo.linkInfo.subject);
                    btnEmail.SetChecked(true);
                }
            } else {
                var txtUrl =  this.getUrlTextBox();
                btnUrl.SetChecked(true);                
                txtUrl.SetValue(rangeInfo.linkInfo.href);
            }

            this.InitializeDisplayPropertiesFields(rangeInfo.linkInfo);

            if(btnUrl)
                btnUrl.RaiseCheckedChanged();
        },
        InitializeDisplayPropertiesFields: function(linkInfo) {
            this.getToolTipTextBox().SetValue(linkInfo.title);
            this.getTextTextBox().SetValue(linkInfo.text);
        },
        SetFocusInField: function() {
            var btnUrl = this.getUrlRadioButton();
            if(btnUrl && btnUrl.GetChecked()) {
                var urlTextBox = this.getUrlTextBox();
                if(urlTextBox)
                    _aspxSetFocusToElement(urlTextBox.name);
            } else {
                var mailtoTextBox = this.getEmailToTextBox();
                if(mailtoTextBox)
                    _aspxSetFocusToElement(mailtoTextBox.name);
            }
        },
        typeChanged: function(s, e) {
            var mainTable = this.getMainTableElement(s.GetMainElement()),
                contentTable = ASPx.GetNodesByClassName(mainTable, "dxssDlgRoundPanelContent");
            if (contentTable && contentTable.length === 1) {
                contentTable = contentTable[0];
                var urlArea = contentTable.rows[0],
                    emailArea = contentTable.rows[1],
                    subjectArea = contentTable.rows[2];
                //TODO change for helper
                ASPx.SetElementDisplay(urlArea, this.getUrlRadioButton().GetChecked());
                ASPx.SetElementDisplay(emailArea, this.getEmailRadioButton().GetChecked());
                ASPx.SetElementDisplay(subjectArea, this.getEmailRadioButton().GetChecked());
            }
        },
        isDialogValid: function () {
            if(this.getEmailToTextBox() && this.getEmailToTextBox().IsVisible())
                return ASPxClientEdit.ValidateGroup("_dxeTxbEmailToGroup");
            if(this.getUrlTextBox() && this.getUrlTextBox().IsVisible())
                return ASPxClientEdit.ValidateGroup("_dxeTxbURLGroup");
            return true;
        },
        getDialogData: function () {
            var url = this.getResultUrl();
            return {
                url: url,
                text: this.getTextTextBox().GetText() || url,
                title: this.getToolTipTextBox().GetText()
            };
        },
        getResultUrl: function() {
            var isInsertEmailMode = this.getEmailRadioButton() && this.getEmailRadioButton().GetChecked();
            return isInsertEmailMode ? this.getEmailUrl() : this.getUrlTextBox().GetValue();
        },
        getEmailUrl: function() {
            return URLConstants.MailToPrefix + this.getEmailToTextBox().GetValue() + URLConstants.SubjectPrefix + this.getSubjectTextBox().GetValue();
        },
        // Helpers
        getHyperLinkInformation: function() {
            var selection = this.spreadsheet.getSelectionInternal(),
                searchResult = ASPxClientSpreadsheet.CellRenderHelper.getSelectedData(this.spreadsheet, selection);

            return {
                hasLink: ASPx.IsExists(searchResult.selectedLink),
                linkInfo: this.getLinkInfo(searchResult)
            };
        },
        getLinkInfo: function(searchResult) {
            var linkInfo = {
                isLinkMailTo:   false,
                href:           URLConstants.DefaultLinkPrefix,
                text:           "",
                title:          "",
                subject:        ""
            };
            if (ASPx.IsExists(searchResult.selectedLink)) {
                var url = ASPx.Attr.GetAttribute(searchResult.selectedLink, "_loc");
                if (url) {
                    var mailtoIndex = url.indexOf(URLConstants.MailToPrefix);
                    if (mailtoIndex >= 0) {
                        var subjectIndex = url.indexOf(URLConstants.SubjectPrefix);
                        var index = subjectIndex > -1 ? subjectIndex : url.length;
                        linkInfo.isLinkMailTo = true;
                        linkInfo.href = url.substring(mailtoIndex + URLConstants.MailToPrefix.length, index);
                        if (subjectIndex > -1)
                            linkInfo.subject = url.substring(subjectIndex + URLConstants.SubjectPrefix.length);
                    }
                    else linkInfo.href = url;
                }
	            linkInfo.text = searchResult.selectedLink.innerHTML;
	            linkInfo.title = ASPx.Attr.GetAttribute(searchResult.selectedLink, "title");
            }
            else if (searchResult.selectedText)
                linkInfo.text = searchResult.selectedText;

            return linkInfo;
        },
        getMainTableElement: function(childElement) {
            return this.mainTableElement = ASPx.GetParentByClassName(childElement, "dxssDlgInsertLinkForm");
        },
                
        // Child control getters
        getUrlRadioButton: function () {
            return this.getControlInstance("_dxeRblUrl");
        },
        getEmailRadioButton: function () {
            return this.getControlInstance("_dxeRblEmail");
        },
        getChangeButton: function () {
            return this.getControlInstance("_dxeBtnChange");
        },
        getEmailToTextBox: function () {
            return this.getControlInstance("_dxeTxbEmailTo");
        },
        getInsertButton: function () {
            return this.getControlInstance("_dxeBtnOk");
        },
        getTextTextBox: function () {
            return this.getControlInstance("_dxeTxbText");
        },
        getToolTipTextBox: function () {
            return this.getControlInstance("_dxeTxbToolTip");
        },
        getSubjectTextBox: function () {
            return this.getControlInstance("_dxeTxbSubject");
        },
        getUrlTextBox: function () {
            return this.getControlInstance("_dxeTxbURL");
        }
    });

    // ** InsertImageDialog ** 
    var InsertPictureDialog = ASPx.CreateClass(SpreadsheetDialog, {
        OnInitComplete: function() {
            SpreadsheetDialog.prototype.OnInitComplete.apply(this, arguments);
            if(typeof(ASPx.SpreadsheetFileManager) != "undefined")
                ASPx.SpreadsheetFileManager.SetOwner(this.GetDialogPopup().GetContentContainer(-1), this.spreadsheet);
        },
        DialogCompletedSuccessfully: function(params) {
            this.executeCommand(SpreadsheetDialogs.InsertPicture.Command, { PicturePath: params.src });
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.InsertImage;
        },

        // Child control getters
        GetImageUploader: function() {
            return this.getControlInstance("_dxeUplImage");
        },
        GetInsertImageUrlTextBox: function() {
            return this.getControlInstance("_dxeTbxInsertImageUrl");
        },
        getHiddenField: function() {
            return this.getControlInstance("_dxHiddenField");
        },
        GetPreviewTextElement: function () {            
            var popupContentElement = this.GetDialogPopup().GetWindowElement(-1),
            textElements = ASPx.GetNodesByClassName(popupContentElement, "dxssPreviewText");
            if(textElements && textElements.length > 0)
                return textElements[0];
            return null;
        },
        GetPreviewImageElement: function () {
            var popupContentElement = this.GetDialogPopup().GetWindowElement(-1),
                imageElement = ASPx.GetNodesByClassName(popupContentElement, "dxssPreviewImage");
            if(imageElement && imageElement.length > 0)
                return imageElement[0];
            return null;
        },
        GetPreviewAreaCell: function() {
            return ASPx.GetParentByTagName(this.GetPreviewImageElement(), "td");
        },
        CheckImageExisting: function(checkingSrc) {
            if(document.images) {
                this.testImage = new Image();
                ASPx.Evt.AttachEventToElement(this.testImage, "load", function() { ASPx.SSTestExistingImageOnLoad(this.spreadsheet.name); }.aspxBind(this));
                ASPx.Evt.AttachEventToElement(this.testImage, "error", function() { ASPx.SSTestExistingImageOnError(this.spreadsheet.name); }.aspxBind(this));
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
        HasImageProductedFromUrl: function() {            
            return this.GetInsertImageUrlTextBox().IsVisible();
        },
        HasImageProductedFromComputer: function() {
            var rblImageFromThisComputer = this.getControlInstance("_dxeRblImageFromThisComputer");
            return rblImageFromThisComputer ? rblImageFromThisComputer.GetChecked() : false;
        },
        OnCallback: function(result) {
            if(this.IsUploadImageCallbackResult(result))
                this.OnCallbackInternal(result);
            else
                ASPx.Dialog.prototype.OnCallback.call(this, result);
        },
        OnCallbackInternal: function(result) {
            this.OnImageSavedToServer(result.substr(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageCallbackPrefix.length + 1));
            this.clearParentControlCallbackOwner();
        },
        IsUploadImageCallbackResult: function(result) {
            return result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageCallbackPrefix) === 0;
        },
        OnComplete: function(result, params) {
            if(result) {
                if(this.HasImageProductedFromComputer()) {
                    var uploadControl = this.GetImageUploader();
                    if(uploadControl)
                        return uploadControl.Upload();
                }
                else if(this.HasImageProductedFromUrl()) {                    
                    return this.SaveImageToServerViaCallback(this.GetInsertImageUrlTextBox().GetText());
                }
            }
            SpreadsheetDialog.prototype.OnComplete.call(this, result, params);
        },
        //TODO to zhuravlev: merge with spreadsheet onCallback event
        OnImageSavedToServer: function(result) {
            if(result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageErrorCallbackPrefix) > -1)
                this.ShowErrorMessage(result.substr(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageErrorCallbackPrefix.length + 1));
            else {
                var callbackData = result.substr(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageSuccessCallbackPrefix.length + 1);
                this.spreadsheet.onResponseReceived(callbackData);

                this.HideDialogPopup();
                this.OnClose();
            }
        },
        ShowErrorMessage: function(message) {
            var textBox = this.GetInsertImageUrlTextBox();
            textBox.isValid = false;
            textBox.errorText = message;
            textBox.UpdateErrorFrameAndFocus(false, true);
        },
        OnImageUploadStart: function() {
            this.ShowLoadingPanelOverDialogPopup();
        },
        OnImageUploadComplete: function(args) {
            this.HideLoadingPanelOverDialogPopup();
            if(args.isValid) {
                if(args.callbackData && args.callbackData != "")
                    this.spreadsheet.onResponseReceived(args.callbackData);

                this.HideDialogPopup();
                this.OnClose();
            }
        },
        OnImageSrcChanged: function(src) {
            this.CheckImageExisting(src);
        },
        OnLoadTestExistingImage: function() {
            this.GetPreviewImageElement().src = this.testImage.src;
            var previewAreaTD = this.GetPreviewAreaCell();

            var maxWidth = previewAreaTD.clientWidth;
            var maxHeight = ASPx.Browser.WebKitFamily ? previewAreaTD.offsetHeight : previewAreaTD.clientHeight;

            this.SetPreviewImageSize(this.testImage.width, this.testImage.height, maxWidth, maxHeight);

            ASPx.SetElementDisplay(this.GetPreviewTextElement(), false);
            ASPx.SetElementDisplay(this.GetPreviewImageElement(), true);
            previewAreaTD.style.borderStyle = "none";
        },
        OnErrorTestExistingImage: function() {
            ASPx.SetElementDisplay(this.GetPreviewTextElement(), true);
            ASPx.SetElementDisplay(this.GetPreviewImageElement(), false);

            var previewAreaTD = this.GetPreviewAreaCell();
            previewAreaTD.style.borderStyle = "";
        },
        SaveImageToServerViaCallback: function(src) {
            this.SendCallbackViaOwner(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageCallbackPrefix, src);
        },
        SetFocusInField: function() {
            var inserImageTextBox =  this.GetInsertImageUrlTextBox();
            if(inserImageTextBox)
                _aspxSetFocusToElement(inserImageTextBox.name);
        },
        getMainTableElement: function (childElement) {
            return this.mainTableElement = ASPx.GetParentByClassName(childElement, "dxssDlgInsertImageForm");
        },
        // Helpers
        getFromWebRadioButton: function () {
            return this.getControlInstance("_dxeRblImageFromTheWeb");
        },        
        typeChanged: function(s, e) {
            var mainTable = this.getMainTableElement(s.GetMainElement()),
            contentTable = ASPx.GetNodesByClassName(mainTable, "dxssDlgRoundPanelContent");
            if (contentTable && contentTable.length === 1) {
                contentTable = contentTable[0];
                var urlArea = contentTable.rows[0],
                    previewArea = contentTable.rows[1],
                    uploadArea = contentTable.rows[2];

                ASPx.SetElementDisplay(urlArea, this.getFromWebRadioButton().GetChecked());
                ASPx.SetElementDisplay(previewArea, this.getFromWebRadioButton().GetChecked());
                ASPx.SetElementDisplay(uploadArea, !this.getFromWebRadioButton().GetChecked());
            }
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            var isValid = true;
            if(this.GetInsertImageUrlTextBox().IsVisible()) {
                isValid = ASPxClientEdit.ValidateGroup("_dxeTbxInsertImageUrlGroup")
            } else {
                isValid = this.GetImageUploader().GetText() !== "";
                if (!isValid)
                    this.GetImageUploader().viewManager.errorView.UpdateCommonErrorDiv(this.getHiddenField().Get("RequiredFieldError"), true);
            }
            return isValid;
        },
        getDialogData: function () {
            return {
                src: this.GetInsertImageUrlTextBox().GetText()
            };
        }
    });

    // ** OpenFileDialog **
    var OpenFileDialog = ASPx.CreateClass(SpreadsheetDialog, {
        OnInitComplete: function() {
            SpreadsheetDialog.prototype.OnInitComplete.apply(this, arguments);

            if(typeof(ASPx.SpreadsheetFileManager) != "undefined")
                ASPx.SpreadsheetFileManager.SetOwner(this.GetDialogPopup().GetContentContainer(-1), this.spreadsheet);
        },
        DialogCompletedSuccessfully: function(params) {
            this.SendCallbackViaOwner(ASPxClientSpreadsheet.CallbackPrefixes.OpenFileCallbackPrefix, params.filePath);
        },
        OnClose: function() {
            SpreadsheetDialog.prototype.OnClose.call(this);
            this.SetForceUpdateFlag();
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.OpenFile;
        },
        GetFileManager: function() {
            return this.getControlInstance("_dxFileManager");
        },
        GetSubmitButton: function () {
            return this.getControlInstance("_dxeBtnOk");
        },
        OnCallback: function(result) {
            if(this.IsOpenFileCallbackResult(result)) {
                this.OnCallbackInternal(result);
            } else {
                ASPx.Dialog.prototype.OnCallback.call(this, result);
            }
        },
        OnCallbackInternal: function(result) {
            this.clearParentControlCallbackOwner();
            this.spreadsheet.genarateClientInstanceGuid();
            this.spreadsheet.onResponseReceived(result.substr(ASPxClientSpreadsheet.CallbackPrefixes.OpenFileCallbackPrefix.length + 1));

            this.HideDialogPopup();
            this.OnClose();                      
        },
        IsOpenFileCallbackResult: function(result) {
            return result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.OpenFileCallbackPrefix) === 0;
        },
        SetFocusInField: function() {
            var fileManager =  this.GetFileManager();
            if(fileManager && fileManager.elements && fileManager.elements.filterElement)
                window.setTimeout(function() {
                            var edit = fileManager.elements.filterElement;
                            if(edit)
                                edit.focus();
            }, 300);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function() {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function() {
            this.dialogComplete(0, null);
        },
        isDialogValid: function() {
            return true;
        },
        getDialogData: function() {
            return {
                filePath: ASPx.SSGetFullFileName(this.GetFileManager().GetCurrentFolderPath("\\", true)) + this.GetFileManager().GetSelectedFile().name
            };
        },
        onSelectedFileChanged: function(s, e) {
            this.GetSubmitButton().SetEnabled(!!e.file);
        }
    });

    // TODO rename SaveFileDialog -> SaveAsFileDialog
    // ** SaveFileDialog **
    var SaveFileDialog = ASPx.CreateClass(SpreadsheetDialog, {
        OnInitComplete: function() {
            if(typeof(ASPx.SpreadsheetFolderManager) != "undefined")
                ASPx.SpreadsheetFolderManager.SetOwner(this.GetDialogPopup().GetContentContainer(-1), this.spreadsheet);

            SpreadsheetDialog.prototype.OnInitComplete.apply(this, arguments);
        },
        DialogCompletedSuccessfully: function(params) {
            if(!params.fileSavedToServer)
                this.executeCommand("DownLoadCopy", { FileFormat: params.fileFormat });
            else {
                this.SetForceUpdateFlag();
                this.SendCallbackViaOwner(ASPxClientSpreadsheet.CallbackPrefixes.SaveFileCallbackPrefix, params.filePath);
            }
        },
        OnCallback: function(result) {
            if(this.IsSaveFileCallbackResult(result)) {
                this.OnCallbackInternal(result);
            } else {
                ASPx.Dialog.prototype.OnCallback.call(this, result);
            }
        },
        OnCallbackInternal: function(result) {
            this.clearParentControlCallbackOwner();
            this.spreadsheet.onResponseReceived(result.substr(ASPxClientSpreadsheet.CallbackPrefixes.SaveFileCallbackPrefix.length + 1));
            
            this.HideDialogPopup();
            this.OnClose();
        },
        IsSaveFileCallbackResult: function(result) {
            return result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.SaveFileCallbackPrefix) === 0;
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.SaveFile;
        },
        InitializeDialogFields: function() {
            if(this.GetFolderPathTextBox() == null) {
                var mainTable = this.GetDialogPopup().GetWindowContentElement(-1).childNodes[0],
                contentTable = ASPx.GetNodesByClassName(mainTable, "dxssDlgRoundPanelContent");
                if(contentTable && contentTable.length === 1) {
                    contentTable = contentTable[0];
                    var downloadArea = contentTable.rows[0];
                    ASPx.SetElementDisplay(downloadArea, "block");
                    if(this.GetSaveButton()) this.GetSaveButton().SetVisible(false);
                    if(this.GetDownloadButton()) this.GetDownloadButton().SetVisible(true);
                }
            } else {
                this.setFileManagerRootFolder();
            }
        },
        GetFileManager: function() {
            var fileManager = null;
            var container = this.GetDialogPopup().GetContentContainer(-1);
            ASPx.GetControlCollection().ProcessControlsInContainer(
                container,
                function(control) {
                    if(control instanceof ASPx.SpreadsheetFolderManager)
                        fileManager = control;
                }
            );
            return fileManager;
        },
        getSaveAsRadioButton: function(){
            return this.getControlInstance("_dxRblFileSavedToServer");
        },
        GetFolderPathTextBox: function() {
            return this.getControlInstance("_dxTbxFolderPath");
        },
        GetFileNameTextBox: function() {
            return this.getControlInstance("_dxTbxFileName");
        },
        GetFileExtensionComboBox: function() {
            return this.getControlInstance("_dxCbxFileType");
        },
        GetSaveButton: function() {
            return this.getControlInstance("_dxeBtnOk");
        },
        GetDownloadButton: function() {
            return this.getControlInstance("_dxBtnDownload");
        },
        GetFormatComboBox: function() {
            return this.getControlInstance("_dxCbxDownloadFileType");
        },
        getHiddenField: function () {
            return this.getControlInstance("_dxHiddenField");
        },
        SetFocusInField: function() {
            if(this.GetFolderPathTextBox() == null) {
                var downloadButton =  this.GetDownloadButton();
                if(downloadButton)
                    _aspxSetFocusToElement(downloadButton.name);
            } else {
                var fileNameTextBox = this.GetFileNameTextBox();
                if(fileNameTextBox)
                    _aspxSetFocusToElement(fileNameTextBox.name);
            }
        },
        setFileManagerRootFolder: function() {
            var fileManager = this.GetFileManager();
            if(fileManager) {
                var subFolderPath = this.GetFolderPathTextBox().GetText(),
                    subFolderCount = subFolderPath.split("\\");
                if(subFolderCount && subFolderCount.length > 2) {
                    subFolderPath = subFolderPath.substr(subFolderPath.indexOf("\\") + 1);
                    fileManager.SetCurrentFolderPath(subFolderPath);
                }
            }
        },
        getMainTableElement: function (childElement) {
            return this.mainTableElement = ASPx.GetParentByClassName(childElement, "dxssDlgSaveFileAsForm");
        },
        typeChanged: function (s, e) {
            var mainTable = this.getMainTableElement(s.GetMainElement()),
                contentTable = ASPx.GetNodesByClassName(mainTable, "dxssDlgRoundPanelContent");
            if (contentTable && contentTable.length === 1) {
                contentTable = contentTable[0];
                var saveAsArea1 = contentTable.rows[0],
                    saveAsArea2 = contentTable.rows[1],
                    saveAsArea3 = contentTable.rows[2],
                    saveAsArea4 = contentTable.rows[3],
                    saveAsArea5 = contentTable.rows[4],
                    downloadArea = contentTable.rows[5];
                //TODO change for helper
                ASPx.SetElementDisplay(saveAsArea1, this.getSaveAsRadioButton().GetChecked());
                ASPx.SetElementDisplay(saveAsArea2, this.getSaveAsRadioButton().GetChecked());
                ASPx.SetElementDisplay(saveAsArea3, this.getSaveAsRadioButton().GetChecked());
                ASPx.SetElementDisplay(saveAsArea4, this.getSaveAsRadioButton().GetChecked());
                ASPx.SetElementDisplay(saveAsArea5, this.getSaveAsRadioButton().GetChecked());
                ASPx.SetElementDisplay(downloadArea, !this.getSaveAsRadioButton().GetChecked());

                this.GetSaveButton().SetVisible(this.getSaveAsRadioButton().GetChecked());
                this.GetDownloadButton().SetVisible(!this.getSaveAsRadioButton().GetChecked());                
            }
        },
        initializeSubmitButton: function(submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function(cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function() {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function() {
            this.dialogComplete(0, null);
        },
        isDialogValid: function() {
            var isValid = true;
            if(this.fileSavedToServer()) {
                isValid = ASPxClientEdit.ValidateGroup("_dxeTbxSaveFilePathGroup");

                var isFileExist = this.isSavedFileExist();
                if (isFileExist)
                    isValid = isValid && confirm(this.GetFileNameTextBox().GetText() + this.GetFileExtensionComboBox().GetValue() + " " + this.getHiddenField().Get("ConfirmMessage"));
            }

            return isValid;
        },
        getDialogData: function() {
            var result = {};
            if(this.fileSavedToServer()) {
                result.filePath = this.removeRootFolder(this.GetFolderPathTextBox().GetText() + this.GetFileNameTextBox().GetText() + this.GetFileExtensionComboBox().GetValue());
                result.fileSavedToServer = true;
            } else {
                result.fileFormat = this.GetFormatComboBox().GetValue();
                result.fileSavedToServer = false;
            }
            return result;
        },
        showSelector: function() {
            FileManager.browsePopup = BrowsePopup;
            BrowsePopup.Show();
            FileManager.Refresh();
            FileManager.Focus();
        },
        initializeFSSubmitButton: function(submitButton) {
            submitButton.Click.AddHandler(this.selectorCompleted.aspxBind(this));
        },
        initializeFSCancelButton: function(cancelButton) {
            cancelButton.Click.AddHandler(this.selectorCanceled.aspxBind(this));
        },
        selectorCompleted: function() {
            BrowsePopup.Hide();
            var documentUrl = FileManager.GetCurrentFolderPath() + "\\";
            this.GetFolderPathTextBox().SetText(documentUrl);
            if (FileManager.GetSelectedFile()) {
                var fileFullName = FileManager.GetSelectedFile().name;
                var fileExt = "." + fileFullName.split(".").pop();
                var fileName = fileFullName.substr(0, fileFullName.indexOf(fileExt));
                var comboBoxExtItem = this.GetFileExtensionComboBox().FindItemByValue(fileExt);
                if (comboBoxExtItem) {
                    this.GetFileExtensionComboBox().SetSelectedItem(comboBoxExtItem);
                }
                this.GetFileNameTextBox().SetText(fileName);
            }
        },
        selectorCanceled: function() {
            BrowsePopup.Hide();
        },
        selectorBeforeShow: function() {
            var panel = this.GetDialogPopup().GetWindowContentElement(-1).childNodes[0];
            ASPx.Attr.ChangeAttribute(panel, "onkeypress", "");
        },
        selectorAfterClose: function() {
            var panel = this.GetDialogPopup().GetWindowContentElement(-1).childNodes[0];
            ASPx.Attr.RestoreAttribute(panel, "onkeypress");
            this.GetFileNameTextBox().Focus();
        },
        isSavedFileExist: function() {
            var fileExist = false;
            var foldedrPath = this.GetFolderPathTextBox().GetText() == "\\" ? "" : this.GetFolderPathTextBox().GetText();
            var fileName = this.GetFileNameTextBox().GetText();
            var fileExtension = this.GetFileExtensionComboBox().GetValue();
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
            return fileExist
        },
        fileSavedToServer:function() {
            return this.GetFileNameTextBox() != null && this.GetFileNameTextBox().IsVisible();
        },
        removeRootFolder: function(filePath) {
            var rootFolderSeparatorPosition = filePath.indexOf("\\");
            if(rootFolderSeparatorPosition >= 0)
                filePath = filePath.substring(rootFolderSeparatorPosition + 1);
            return filePath;
        }
    });

    // ** Row Height **
    var RowHeightDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(dialogData.rowHeight != this.GetRowHeight()) {                    
                this.executeCommand(SpreadsheetDialogs.RowHeight.Command, { RowHeight: dialogData.rowHeight });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.RowHeight;
        },
        GetInitInfoObject: function() {
            return this.spreadsheet.getActiveCellSize(false);
        },
        InitializeDialogFields: function(rowHeight) {
            this.SaveDialogDataInternal(rowHeight);
            this.GetRowHeightSpinEdit().SetValue(rowHeight);
        },
        SaveDialogDataInternal: function(data) {
            this.SetRowHeight(data);
        },
        SetRowHeight: function(rowHeight) {
            this.rowHeight = rowHeight;
        },
        GetRowHeight: function() {
            return this.rowHeigh;
        },
        GetRowHeightSpinEdit: function() {
            return this.getControlInstance("_dxSpRowHeight");
        },
        SetFocusInField: function() {
            var sizeSpinEdit = this.GetRowHeightSpinEdit();
            if(sizeSpinEdit)
                _aspxSetFocusToElement(sizeSpinEdit.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxSpRowHeight");
        },
        getDialogData: function () {
            return {
                rowHeight: this.GetRowHeightSpinEdit().GetValue()
            };
        }
    });

    // ** Column Width **
    var ColumnWidthDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(dialogData.columnWidth != this.GetColumnWidth()) {                    
                this.executeCommand(SpreadsheetDialogs.ColumnWidth.Command, { ColumnWidth: dialogData.columnWidth });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ColumnWidth;
        },
        GetInitInfoObject: function() {
            return this.spreadsheet.getActiveCellSize(true);
        },
        InitializeDialogFields: function(columnWidth) {
            this.SaveDialogDataInternal(columnWidth);
            this.GetColumnWidthSpinEdit().SetValue(columnWidth);
        },
        SaveDialogDataInternal: function(data) {
            this.SetColumnWidth(data);
        },
        SetColumnWidth: function(columnWidth) {
            this.columnWidth = columnWidth;
        },
        GetColumnWidth: function() {
            return this.columnWidth;
        },
        GetColumnWidthSpinEdit: function() {
            return this.getControlInstance("_dxSpColumnWidth");
        },
        SetFocusInField: function() {
            var sizeSpinEdit = this.GetColumnWidthSpinEdit();
            if(sizeSpinEdit)
                _aspxSetFocusToElement(sizeSpinEdit.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxSpColumnWidth");
        },
        getDialogData: function () {
            return {
                columnWidth: this.GetColumnWidthSpinEdit().GetValue()
            };
        }
    });

    // ** Default Column Width **
    var DefaultColumnWidthDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(dialogData.defaultColumnWidth != this.GetDefaultColumnWidth()) {                    
                this.executeCommand(SpreadsheetDialogs.DefaultColumnWidth.Command, { DefaultColumnWidth: dialogData.defaultColumnWidth });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.DefaultColumnWidth;
        },
        GetInitInfoObject: function() {
            return this.spreadsheet.getDefaultCellSize().width;
        },
        InitializeDialogFields: function(defaultColumnWidth) {
            this.SaveDialogDataInternal(defaultColumnWidth);
            this.GetDefaultColumnWidthSpinEdit().SetValue(defaultColumnWidth);
        },
        SaveDialogDataInternal: function(data) {
            this.SetDefaultColumnWidth(data);
        },
        SetDefaultColumnWidth: function(defaultColumnWidth) {
            this.defaultColumnWidth = defaultColumnWidth;
        },
        GetDefaultColumnWidth: function() {
            return this.defaultColumnWidth;
        },
        GetDefaultColumnWidthSpinEdit: function() {
            return this.getControlInstance("_dxSpDefaultColumnWidth");
        },
        SetFocusInField: function() {
            var sizeSpinEdit = this.GetDefaultColumnWidthSpinEdit();
            if(sizeSpinEdit)
                _aspxSetFocusToElement(sizeSpinEdit.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxSpDefaultColumnWidth");
        },
        getDialogData: function () {
            return {
                defaultColumnWidth: this.GetDefaultColumnWidthSpinEdit().GetValue()
            };
        }
    });

    // ** Unhide sheet **
    var UnhideSheetDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            this.executeCommand(SpreadsheetDialogs.UnhideSheet.Command, { SheetName: dialogData.sheetName });
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.UnhideSheet;
        },
        GetInitInfoObject: function() {
            return this.spreadsheet.getTabControl().getHiddenSheets();
        },
        InitializeDialogFields: function(hiddenSheets) {
            var listBox = this.GetUnhideSheetListBox();
            if(listBox && hiddenSheets.length > 0) {
                for(var index = 0; index < hiddenSheets.length; index++) {
                    listBox.AddItem(hiddenSheets[index].Name, hiddenSheets[index].Name);
                }
                listBox.SetSelectedIndex(0);
            }
        },
        GetUnhideSheetListBox: function() {
            return this.getControlInstance("_dxlbUnhideSheet");
        },
        SetFocusInField: function() {
            var sheetListBox = this.GetUnhideSheetListBox();
            if(sheetListBox)
                _aspxSetFocusToElement(sheetListBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxSpUnhideSheet");
        },
        getDialogData: function () {
            return {
                sheetName: this.GetUnhideSheetListBox().GetValue()
            };
        }
    });

    var MoveOrCopySheetDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            this.executeCommand(SpreadsheetDialogs.MoveOrCopySheet.Command,
                {
                    BeforeVisibleSheetIndex: dialogData.BeforeVisibleSheetIndex,
                    CreateCopy: dialogData.CreateCopy
                });
        },
        OnClose: function() {
            this.SetForceUpdateFlag();
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.MoveOrCopySheet;
        },
        GetInitInfoObject: function() { },
        InitializeDialogFields: function() {
            this.getCreateCopyCheckbox().SetChecked(false);
        },
        getBeforeSheetListBox: function() {
            return this.getControlInstance("_lbxBeforeSheet");
        },
        getCreateCopyCheckbox: function() {
            return this.getControlInstance("_cbCreateCopy");
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return true;
        },
        getDialogData: function () {
            return {
                BeforeVisibleSheetIndex: this.getBeforeSheetListBox().GetSelectedIndex(),
                CreateCopy: this.getCreateCopyCheckbox().GetValue() === true
            };
        }
    });

    //Change chart type
    var ChangeChartTypeDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(ASPx.IsExists(dialogData.chartType)) {
                this.executeCommand(SpreadsheetDialogs.ChartType.Command, { ChartType: dialogData.chartType });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ChangeChartType;
        },
        GetDialogOkButton: function() {
            return this.getControlInstance("_dxeBtnOk");
        },
        getHiddenField: function () {
            return this.getControlInstance("_dxHiddenField");
        },
        SetFocusInField: function() {
            var okButton = this.GetDialogOkButton();
            if(okButton)
                _aspxSetFocusToElement(okButton.name);
        },
        setNewChartId: function(chartId) {
            this.getHiddenField().Set("chartType", chartId);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return true;
        },
        getDialogData: function () {
            return {
                chartType: this.getHiddenField().Get("chartType")
            };
        }
    });

    //Chart select data
    var ChartSelectDataDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            this.executeCommand(SpreadsheetDialogs.ChartData.Command, { SelectionRange: dialogData.selectionRange });
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ChartSelectData;
        },
        GetInitInfoObject: function() {
            var drawingBoxElement = this.spreadsheet.getSelectionInternal().drawingBoxElement;
            var chartIndex = ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi");

            return this.spreadsheet.getChartAttributeById(chartIndex, "Range");
        },
        InitializeDialogFields: function(range) {
            var chartRangeTextBox = this.GetChartRangeTextBox();

            if(chartRangeTextBox) {
                chartRangeTextBox.SetValue("=" + range);

                var that = this,
                    editorElement = chartRangeTextBox.GetInputElement();

                that.spreadsheet.getDynamicSelectionHelper().attach(editorElement, true);
                that.spreadsheet.getStateController().notifyDialogOpen();
                ASPx.Evt.AttachEventToElement(editorElement, "keyup", function() {
                    that.spreadsheet.getDynamicSelectionHelper().refresh();
                });
            }
        },
        GetChartRangeTextBox: function() {
            return this.getControlInstance("_dxTxbChartDataRange");
        },
        getDialogModal: function() {
            return false;
        },
        OnClose: function() {
            this.spreadsheet.getDynamicSelectionHelper().detach();
            this.spreadsheet.getStateController().notifyDialogClose();
            this.switchRibbonEnabledMode(true);
            this.switchContextMenuEnableMode(true);
            SpreadsheetDialog.prototype.OnClose.call(this);
        },
        OnShown: function() {
            this.switchContextMenuEnableMode(false);
            this.switchRibbonEnabledMode(false);
            SpreadsheetDialog.prototype.OnShown.call(this);
        },
        SetFocusInField: function() {
            var rangeTextBox = this.GetChartRangeTextBox();
            if(rangeTextBox)
                _aspxSetFocusToElement(rangeTextBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxTxbChartDataRangeValidationGroup");
        },
        getDialogData: function () {
            return {
                selectionRange: this.GetChartRangeTextBox().GetValue()
            };
        }
    });

    // **Chart Change Title Dialog**
    var ChartChangeTitleDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(dialogData.chartTitle != this.GetChartTitle()) {
                this.executeCommand(SpreadsheetDialogs.ChartTitle.Command, { ChartTitle: dialogData.chartTitle });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ChartChangeTitle;
        },
        GetInitInfoObject: function() {
            var drawingBoxElement = this.spreadsheet.getSelectionInternal().drawingBoxElement;
            var chartIndex = ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi");

            return this.spreadsheet.getChartAttributeById(chartIndex, "Title");
        },
        InitializeDialogFields: function(chartTitle) {
            this.SaveDialogDataInternal(chartTitle);
            this.GetChartTitleTextBox().SetValue(chartTitle);
        },
        SaveDialogDataInternal: function(data) {
            this.SetChartTitle(data);
        },
        SetChartTitle: function(chartTitle) {
            this.chartTitle = chartTitle;
        },
        GetChartTitle: function() {
            return this.chartTitle;
        },
        GetChartTitleTextBox: function() {
            return this.getControlInstance("_dxChartTitle");
        },
        SetFocusInField: function() {
            var titleTextBox = this.GetChartTitleTextBox();
            if(titleTextBox)
                _aspxSetFocusToElement(titleTextBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxChartChangeTitleGroup");
        },
        getDialogData: function () {
            return {
                chartTitle: this.GetChartTitleTextBox().GetValue()
            };
        }
    });

    // **Chart Change Horizontal Axis Title Dialog**
    var ChartChangeHorizontalAxisTitleDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(dialogData.chartHorizontalAxisTitle != this.GetChartHorizontalAxisTitle()) {
                this.executeCommand(SpreadsheetDialogs.ChartHorizontalAxisTitle.Command, { ChartHorizontalAxisTitle: dialogData.chartHorizontalAxisTitle });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ChartChangeHorizontalAxisTitle;
        },
        GetInitInfoObject: function() {
            var drawingBoxElement = this.spreadsheet.getSelectionInternal().drawingBoxElement;
            var chartIndex = ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi");

            return this.spreadsheet.getChartAttributeById(chartIndex, "HAxisTitle");
        },
        InitializeDialogFields: function(chartHorizontalAxisTitle) {
            this.SaveDialogDataInternal(chartHorizontalAxisTitle);
            this.GetChartHorizontalAxisTitleTextBox().SetValue(chartHorizontalAxisTitle);
        },
        SaveDialogDataInternal: function(data) {
            this.SetChartHorizontalAxisTitle(data);
        },
        SetChartHorizontalAxisTitle: function(chartHorizontalAxisTitle) {
            this.chartHorizontalAxisTitle = chartHorizontalAxisTitle;
        },
        GetChartHorizontalAxisTitle: function() {
            return this.chartHorizontalAxisTitle;
        },
        GetChartHorizontalAxisTitleTextBox: function() {
            return this.getControlInstance("_dxChartHorizontalAxisTitle");
        },
        SetFocusInField: function() {
            var titleTextBox = this.GetChartHorizontalAxisTitleTextBox();
            if(titleTextBox)
                _aspxSetFocusToElement(titleTextBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxChartChangeHorizontalAxisTitleGroup");
        },
        getDialogData: function () {
            return {
                chartHorizontalAxisTitle: this.GetChartHorizontalAxisTitleTextBox().GetValue()
            };
        }
    });

    // **Chart Change Vertical Axis Title Dialog**
    var ChartChangeVerticalAxisTitleDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(dialogData.chartVerticalAxisTitle != this.GetChartVerticalAxisTitle()) {
                this.executeCommand(SpreadsheetDialogs.ChartVerticalAxisTitle.Command, { ChartVerticalAxisTitle: dialogData.chartVerticalAxisTitle });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ChartChangeVerticalAxisTitle;
        },
        GetInitInfoObject: function() {
            var drawingBoxElement = this.spreadsheet.getSelectionInternal().drawingBoxElement;
            var chartIndex = ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi");

            return this.spreadsheet.getChartAttributeById(chartIndex, "VAxisTitle");
        },
        InitializeDialogFields: function(chartVerticalAxisTitle) {
            this.SaveDialogDataInternal(chartVerticalAxisTitle);
            this.GetChartVerticalAxisTitleTextBox().SetValue(chartVerticalAxisTitle);
        },
        SaveDialogDataInternal: function(data) {
            this.SetChartVerticalAxisTitle(data);
        },
        SetChartVerticalAxisTitle: function(chartVerticalAxisTitle) {
            this.chartVerticalAxisTitle = chartVerticalAxisTitle;
        },
        GetChartVerticalAxisTitle: function() {
            return this.chartVerticalAxisTitle;
        },
        GetChartVerticalAxisTitleTextBox: function() {
            return this.getControlInstance("_dxChartVerticalAxisTitle");
        },
        SetFocusInField: function() {
            var titleTextBox = this.GetChartVerticalAxisTitleTextBox();
            if(titleTextBox)
                _aspxSetFocusToElement(titleTextBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxChartChangeVerticalAxisTitleGroup");
        },
        getDialogData: function () {
            return {
                chartVerticalAxisTitle: this.GetChartVerticalAxisTitleTextBox().GetValue()
            };
        }
    });

    //Modify Chart Layout
    var ModifyChartLayoutDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            if(ASPx.IsExists(dialogData.chartLayoutPreset)) {
                this.executeCommand(SpreadsheetDialogs.ChartLayout.Command, { ChartLayoutPreset: dialogData.chartLayoutPreset });
            }
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ModifyChartLayout;
        },
        GetInitInfoObject: function() {
            var drawingBoxElement = this.spreadsheet.getSelectionInternal().drawingBoxElement;
            var chartIndex = ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi");

            return this.spreadsheet.getChartAttributeById(chartIndex, "View");
        },
        InitializeDialogFields: function(chartPresetView) {
            var panelContent = this.GetContentPanel();
            if(panelContent) {
                var presetsArray = ASPx.GetChildNodesByClassName(panelContent.GetMainElement(), "dxss-chartPresetContainer");
                if(presetsArray && presetsArray.length > 0) {
                    for(var i = 0; i < presetsArray.length; i++) {
                        if(ASPx.Attr.GetAttribute(presetsArray[i], "view") === chartPresetView)
                            ASPx.SetStylesCore(presetsArray[i], "display", "block");
                    }
                }
            }
        },
        GetContentPanel: function() {
            return this.getControlInstance("_dxPanelChartPresets");
        },
        GetOkButton: function() {
            return this.getControlInstance("_dxeBtnOk");
        },
        getHiddenField: function () {
            return this.getControlInstance("_dxHiddenField");
        },
        SetFocusInField: function() {
            var okButton = this.GetOkButton();
            if(okButton)
                _aspxSetFocusToElement(okButton.name);
        },
        setChartLayoutId: function (chartId) {
            this.getHiddenField().Set("presetImage", chartId);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return true;
        },
        getDialogData: function () {
            return {
                chartLayoutPreset: this.getHiddenField().Get("presetImage")
            };
        }
    });

    //Modify Chart Style
    var ModifyChartStyleDialog = ASPx.CreateClass(SpreadsheetDialog, {
        OnInitComplete: function () {
            this.presetsCount = 48;
            SpreadsheetDialog.prototype.OnInitComplete.apply(this, arguments);
        },
        DialogCompletedSuccessfully: function(dialogData) {
            if(ASPx.IsExists(dialogData.chartPresetStyle)) {
                this.executeCommand(SpreadsheetDialogs.ChartStyle.Command, { ChartPresetStyle: dialogData.chartPresetStyle });
            }
        },
        SendCallbackForDialogContent: function() {
            SpreadsheetDialog.prototype.SendCallbackForDialogContent.call(this);
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ModifyChartStyle;
        },
        GetOkButton: function() {
            return this.getControlInstance("_dxeBtnOk");
        },
        getHiddenField: function () {
            return this.getControlInstance("_dxHiddenField");
        },
        SetFocusInField: function() {
            var okButton = this.GetOkButton();
            if(okButton)
                _aspxSetFocusToElement(okButton.name);
        },
        OnClose: function() {
            this.SetForceUpdateFlag();
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return true;
        },
        getDialogData: function () {
            return {
                chartPresetStyle: this.getHiddenField().Get("presetStyle")
            };
        },
        setChartStyle: function(imageName) {
            this.getHiddenField().Set("presetStyle", imageName);
        },
        initializeChartStyle: function() {
            if (this.getHiddenField().initializeButtonCount) {
                this.getHiddenField().initializeButtonCount++;
            } else {
                ASPx.SSBeginInitializeInternalControls();
                this.getHiddenField().initializeButtonCount = 1;
            }
            if (this.getHiddenField().initializeButtonCount === this.presetsCount) {
                ASPx.SSEndInitializeInternalControls()
            }
        }
    });

    //Find And Replace
    var FindAndReplaceDialog = ASPx.CreateClass(SpreadsheetDialog, {
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.FindAndReplace;
        },
        OnFind: function(dialogData) {
            var resultListBox = this.GetSearchResultListBox();
            resultListBox.ClearItems();
            this.spreadsheet.findAllFromDialog(this, dialogData.findWhat, dialogData.matchCase, dialogData.matchCellContent, dialogData.searchBy, dialogData.lookIn);
        },
        OnSearchResultsReceived: function(findAllList, findNextCellModelPosition) {
            var resultListBox = this.GetSearchResultListBox();
            if(resultListBox) {
                for(var findResultCell in findAllList)
                    resultListBox.AddItem([findAllList[findResultCell].CellPosition, findAllList[findResultCell].DisplayText], findAllList[findResultCell].CellPosition);

                var nextCellFound = false;
                if(resultListBox.GetItemCount() > 0)
                    nextCellFound = this.selectNext(resultListBox, findNextCellModelPosition);

                if(!nextCellFound)
                    this.SetFocusInField();
            }
        },
        selectNext: function(resultListBox, findNextCellModelPosition) {
            var nextCellFound;

            if(findNextCellModelPosition)
                resultListBox.SetValue(findNextCellModelPosition);

            nextCellFound = resultListBox.GetSelectedIndex() > -1;
            if(!nextCellFound)
                resultListBox.SetSelectedIndex(0);

            nextCellFound = resultListBox.GetSelectedIndex() > -1;
            if(nextCellFound) {
                resultListBox.SetFocus();
                this.ScrollToCell();
            } 

            return nextCellFound;
        },
        GetSearchResultListBox: function() {
            return this.getControlInstance("_dxLbxSearchResults");
        },
        getFindWhatTextBox: function() {
            return this.getControlInstance("_dxTxbFindWhat");
        },
        getSearchParamsCheckBox: function() {
            return this.getControlInstance("_dxCbxSearchParams");
        },        
        getLookinCheckBox: function() {
            return this.getControlInstance("_dxCbxLookin");
        },
        getTextCaseComboBox: function () {
            return this.getControlInstance("_dxCbTextCase");
        },
        getMatchContentComboBox: function () {
            return this.getControlInstance("_dxCbMatchContent");
        },
        ScrollToCell: function() {
            var resultTextBox = this.GetSearchResultListBox();
            if(resultTextBox) {
                var searchValue = resultTextBox.GetValue();
                var searchCell = ASPxClientSpreadsheet.CellPositionConvertor.getCellModelPositionByStringRepresentation(searchValue);
                if(searchCell)
                    this.spreadsheet.getPaneManager().scrollTo(searchCell.col, searchCell.row, true);
            }
        },
        getDialogModal: function() {
            return false;
        },
        OnClose: function() {
            this.switchContextMenuEnableMode(true);
            this.switchRibbonEnabledMode(true);
            SpreadsheetDialog.prototype.OnClose.call(this);
        },
        OnShown: function() {
            this.switchContextMenuEnableMode(false);
            this.switchRibbonEnabledMode(false);
            SpreadsheetDialog.prototype.OnShown.call(this);
        },
        SetFocusInField: function() {
            var findWhatTextBox = this.getFindWhatTextBox();
            if(findWhatTextBox)
                _aspxSetFocusToElement(findWhatTextBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.OnFind(this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxTxbFindGroup");
        },
        getDialogData: function () {
            return {
                findWhat: this.getFindWhatTextBox().GetValue(),
                searchBy: this.getSearchParamsCheckBox().GetValue(),
                lookIn: this.getLookinCheckBox().GetValue(),
                matchCase: this.getTextCaseComboBox().GetChecked(),
                matchCellContent: this.getMatchContentComboBox().GetChecked()
            };
        }
    });

    var InsertTableDialog = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            this.executeCommand(SpreadsheetDialogs.InsertTable.Command,
                {
                    SelectedRange: dialogData.selectedRange,
                    HasHeaders: dialogData.hasHeaders
                });
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.TableSelectData;
        },
        GetInitInfoObject: function() {
            var range = this.spreadsheet.getSelectionInternal().range,
                paneManager = this.spreadsheet.getPaneManager();

            var rightColIndex = paneManager.convertVisibleIndexToModelIndex(range.rightColIndex, true),
                bottomRowIndex = paneManager.convertVisibleIndexToModelIndex(range.bottomRowIndex, false),
                leftColIndex = paneManager.convertVisibleIndexToModelIndex(range.leftColIndex, true),
                topRowIndex = paneManager.convertVisibleIndexToModelIndex(range.topRowIndex, false);
                
            return ASPxClientSpreadsheet.DynamicSelectionHelper.getRangeText(
                { colIndex: rightColIndex, rowIndex: bottomRowIndex },
                { colIndex: leftColIndex, rowIndex: topRowIndex }
            );
        },
        InitializeDialogFields: function(rangeText) {
            var tableRangeTextBox = this.getTableRangeTextBox();

            if(tableRangeTextBox) {
                tableRangeTextBox.SetValue("=" + rangeText);

                var that = this,
                    editorElement = tableRangeTextBox.GetInputElement();

                that.spreadsheet.getDynamicSelectionHelper().attach(editorElement, true);
                that.spreadsheet.getStateController().notifyDialogOpen();
                ASPx.Evt.AttachEventToElement(editorElement, "keyup", function() {
                    that.spreadsheet.getDynamicSelectionHelper().refresh();
                });
            }
        },
        getTableRangeTextBox: function() {
            return this.getControlInstance("_dxTxbTableDataRange");
        },
        getTableHasHeadersCheckbox: function() {
            return this.getControlInstance("_chbTableHasHeaders")
        },
        getDialogModal: function() {
            return false;
        },
        OnClose: function() {
            this.spreadsheet.getDynamicSelectionHelper().detach();
            this.spreadsheet.getStateController().notifyDialogClose();
            this.switchRibbonEnabledMode(true);
            this.switchContextMenuEnableMode(true);
            SpreadsheetDialog.prototype.OnClose.call(this);
        },
        OnShown: function() {
            this.switchContextMenuEnableMode(false);
            this.switchRibbonEnabledMode(false);
            SpreadsheetDialog.prototype.OnShown.call(this);
        },
        SetFocusInField: function() {
            var rangeTextBox = this.getTableRangeTextBox();
            if(rangeTextBox)
                _aspxSetFocusToElement(rangeTextBox.name);
        },
        initializeSubmitButton: function (submitButton) {
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
        },
        initializeCancelButton: function (cancelButton) {
            cancelButton.Click.AddHandler(this.closeDialog.aspxBind(this));
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxTxbTableDataRangeValidationGroup");
        },
        getDialogData: function () {
            return {
                selectedRange: this.getTableRangeTextBox().GetValue(),
                hasHeaders: this.getTableHasHeadersCheckbox().GetChecked()
            };
        }
    });

    var InsertTableWithStyleDialog = ASPx.CreateClass(InsertTableDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            this.executeCommand(SpreadsheetDialogs.InsertTableWithStyle.Command,
                {
                    SelectedRange: dialogData.selectedRange,
                    HasHeaders: dialogData.hasHeaders,
                    TableStyle: dialogData.tableStyle
                });
        },
        getDialogData: function () {
            var dialogData = InsertTableDialog.prototype.getDialogData.call(this);
            var tableStyle = this.spreadsheet._tableStyleValue || "";
            this.spreadsheet._tableStyleValue = null;

            dialogData.tableStyle = tableStyle;

            return dialogData;
        }
    });

    var ModifyTableStyleDialog = ASPx.CreateClass(SpreadsheetDialog, {
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.ModifyTableStyle;
        },
        DialogCompletedSuccessfully: function(dialogData) {
            if(ASPx.IsExists(dialogData.styleName)) {
                this.executeCommand(SpreadsheetDialogs.ModifyTableStyle.Command, { StyleName: dialogData.styleName });
            }
        },
        OnClose: function() {
            this.SetForceUpdateFlag();
        },
        submitDialog: function () {
            if(this.isDialogValid()) {
                ASPx.DialogComplete(1, this.getDialogData());
            }
        },
        closeDialog: function () {
            ASPx.DialogComplete(0, null);
        },
        isDialogValid: function () {
            return true;
        },
        getDialogData: function () {
            return {
                styleName: this.getTableStyle()
            };
        },
        initializeSubmitButton: function (submitButton) {
            SpreadsheetDialog.prototype.initializeSubmitButton.call(this, submitButton);

            submitButton.SetEnabled(false);
            this.submitButton = submitButton;
        },
        setTableStyle: function(styleName) {
            this.getHiddenField().Set("styleName", styleName);
            this.submitButton.SetEnabled(true);
        },
        getTableStyle: function() {
            return this.getHiddenField().Get("styleName");
        },
        getHiddenField: function () {
            return this.getControlInstance("_dxHiddenField");
        }
    });

    var FormatAsTableDialog = ASPx.CreateClass(ModifyTableStyleDialog, {
        DialogCompletedSuccessfully: function(dialogData) {
            this.spreadsheet._tableStyleValue = this.getTableStyle();

            if(this.selectionHasTable())
                ModifyTableStyleDialog.prototype.DialogCompletedSuccessfully.call(this, dialogData);
            else
                this.spreadsheet.onShortCutCommand(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("InsertTableWithStyle").id);
        },
        selectionHasTable: function() {
            return this.spreadsheet.getRibbonManager().isSelectionWithinAnyTable;
        }
    });

    var AutoFilterDialogBase = ASPx.CreateClass(SpreadsheetDialog, {
        DialogCompletedSuccessfully: function(params) {
            this.executeCommand("ApplyAutoFilter", {
                filterCommandId: params.filterCommandId,
                ViewModel: params.ViewModel
            });
        },
        getDialogData: function() {
            return {
                filterCommandId: this.commandId,
                ViewModel: ASPx.Json.ToJson(this.getViewModel())
            };
        },
        OnClose: function() {

        },
        populateEditors: function() {

        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.DataFilterSimple;
        },
        GetInitInfoObject: function() {

        },
        SaveDialogDataInternal: function(data) {

        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },
        submitDialog: function() {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        isDialogValid: function () {
            return true;
        }
    });

    var DataFilterSimpleDialog = ASPx.CreateClass(AutoFilterDialogBase, {
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.DataFilterSimple;
        },
        OnClose: function() {
            this.SetForceUpdateFlag();
        },
        closeDialog: function () {
            this.dialogComplete(0, null);
        },

        isDialogValid: function () {
            return true;
        },
        InitializeDialogFields: function() {
            this.getCheckAllButton().Click.AddHandler(this.checkAll.aspxBind(this));
            this.getUnCheckAllButton().Click.AddHandler(this.uncheckAll.aspxBind(this));
        },
        getValuesTreeView: function() {
            return this.getControlInstance("_twValues");
        },
        getCheckAllButton: function() {
            return this.getControlInstance("_btnCheckAll");
        },
        getUnCheckAllButton: function() {
            return this.getControlInstance("_btnUnCheckAll");
        },
        getSubmitButton: function () {
            return this.getControlInstance("_dxeBtnOk");
        },
        checkAll: function() {
            this.setAllItemsCheckedState(true);
        },
        uncheckAll: function() {
            this.setAllItemsCheckedState(false);
        },
        setAllItemsCheckedState: function(state) {
            this.setTreeViewNodesChecked(this.getValuesTreeView().GetRootNode().nodes, state);
            this.onValuesTreeViewCheckedChanged();
        },
        setTreeViewNodesChecked: function(nodes, checked) {
            ASPx.Data.ForEach(nodes, function(node) {
                node.SetChecked(checked);
            }.aspxBind(this));
        },
        getViewModel: function() {
            return this.getCheckedNodes();
        },
        getCheckedNodes: function() {
            this.itemsState = {};

            this.getCheckedChildren(this.getValuesTreeView().GetRootNode().nodes);

            var result = [false]; /*root node*/
            ASPx.Data.ForEach(ASPx.GetObjectKeys(this.itemsState), function(key) {
                result.push(this.itemsState[key]);
            }.aspxBind(this));

            return result;
        },
        getCheckedChildren: function(nodes) {
            var atLeastOneChecked = false;
            ASPx.Data.ForEach(nodes, function(node) {
                this.itemsState[node.name] = node.nodes.length ? this.getCheckedChildren(node.nodes) : node.GetChecked();

                atLeastOneChecked = (atLeastOneChecked || this.itemsState[node.name] || node.GetChecked());
            }.aspxBind(this));

            return atLeastOneChecked;
        },
        hasCheckedItems: function() {
            var checkedNodes = this.getCheckedNodes();

            for(var i = 0; i < checkedNodes.length; i++) {
                if(checkedNodes[i])
                    return true;
            }

            return false;
        },

        onValuesTreeViewCheckedChanged: function() {
            var submitAllowed = this.hasCheckedItems();
            this.getSubmitButton().SetEnabled(submitAllowed);
        }
    });

    var CustomDataFilterDialogBase = ASPx.CreateClass(AutoFilterDialogBase, {
        OnViewModelReceived: function(viewModel) {
            if(viewModel) {
                this.viewModel = viewModel;
                this.populateEditors();
            }
        },
        InitializeDialogFields: function() {
            this.spreadsheet.fetchAutoFilterViewModelFromDialog(this);
        },
        populateComboBox: function(editor, dataSource, selectedItem) {
            if(!(editor && dataSource))
                return;

            editor.ClearItems();
            ASPx.Data.ForEach(dataSource, function(item) {
                if(item.Text)
                    editor.AddItem(item.Text, item.Value);
                else
                    editor.AddItem(item, item);
            });

            if(selectedItem) {
                if(selectedItem.Value)
                    editor.SetSelectedItem(selectedItem);
                else
                    editor.SetValue(selectedItem);
            }
        },
        getFilterOperatorComboBox: function(index) {
            return this.getControlInstance("_cbFilterOperator" + index);
        },
        getAndOrRadioGroup: function() {
            return this.getControlInstance("_rblAndOr");
        }
    });

    var CustomDataFilterDialog = ASPx.CreateClass(CustomDataFilterDialogBase, {
        populateEditors: function() {
            this.populateComboBox(this.getFilterOperatorComboBox(1), this.viewModel["FilterOperatorsDataSource"], this.viewModel["FilterOperator"]);
            this.populateComboBox(this.getFilterValueComboBox(1), this.viewModel["UniqueFilterValues"], this.viewModel["FilterValue"]);

            this.populateComboBox(this.getFilterOperatorComboBox(2), this.viewModel["FilterOperatorsDataSource"], this.viewModel["SecondaryFilterOperator"]);
            this.populateComboBox(this.getFilterValueComboBox(2), this.viewModel["UniqueFilterValues"], this.viewModel["SecondaryFilterValue"]);

            var selectedIndex = this.viewModel["CriterionAnd"] === false ? 1 : 0; // when no filter is applied, CriterionAnd is undefined
            this.getAndOrRadioGroup().SetSelectedIndex(selectedIndex);
        },
        getFilterValueComboBox: function(index) {
            return this.getControlInstance("_cbFilterValue" + index);
        },
        getViewModel: function() {
            var viewModel = {};
            var firstFilterOperator = this.getFilterOperatorComboBox(1).GetValue(),
                secondaryFilterOperator = this.getFilterOperatorComboBox(2).GetValue(),
                firstFilterValue = this.getFilterValueComboBox(1).GetValue(),
                secondaryFilterValue = this.getFilterValueComboBox(2).GetValue();

            viewModel["OperatorAnd"] = this.getAndOrRadioGroup().GetSelectedIndex() === 0;
            if(firstFilterOperator)
                viewModel["FirstFilterOperator"] = firstFilterOperator;
            if(firstFilterValue)
                viewModel["FirstFilterValue"] = firstFilterValue;
            if(secondaryFilterOperator)
                viewModel["SecondaryFilterOperator"] = secondaryFilterOperator;
            if(secondaryFilterValue)
                viewModel["SecondaryFilterValue"] = secondaryFilterValue;

            return viewModel;
        }
    });

    var CustomDateTimeFilterDialog = ASPx.CreateClass(CustomDataFilterDialog, {
        InitializeDialogFields: function() {
            CustomDataFilterDialog.prototype.InitializeDialogFields.call(this);

            this.getDateEditor(1).DateChanged.AddHandler(function(s) { this.onDateChanged(s, 1); }.aspxBind(this));
            this.getDateEditor(2).DateChanged.AddHandler(function(s) { this.onDateChanged(s, 2); }.aspxBind(this));
        },
        getDateEditor: function(index) {
            return this.getControlInstance("_deFilterDate" + index);
        },
        onDateChanged: function(editor, index) {
            var date = new Date(editor.GetDate()),
                value = (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
            this.getFilterValueComboBox(index).SetValue(value);
        }
    });

    var DataFilterTop10Dialog = ASPx.CreateClass(CustomDataFilterDialogBase, {
        populateEditors: function() {
            this.populateComboBox(this.getFilterOrderComboBox(), this.viewModel["OrderDataSource"]);
            this.populateComboBox(this.getFilterTypeComboBox(), this.viewModel["TypeDataSource"]);

            this.getFilterOrderComboBox().SetSelectedIndex(this.viewModel["IsTop"] ? 0 : 1);
            this.getFilterTypeComboBox().SetSelectedIndex(this.viewModel["IsPercent"] ? 1 : 0);

            this.getFilterValueSpinEdit().SetValue(this.viewModel["Value"]);
        },
        getFilterOrderComboBox: function() {
            return this.getControlInstance("_cbFilterOrder");
        },
        getFilterTypeComboBox: function() {
            return this.getControlInstance("_cbFilterType");
        },
        getFilterValueSpinEdit: function() {
            return this.getControlInstance("_cbFilterValue");
        },
        getViewModel: function() {
            return {
                IsTop: this.getFilterOrderComboBox().GetSelectedIndex() === 0,
                IsPercent: this.getFilterTypeComboBox().GetSelectedIndex() === 1,
                Value: this.getFilterValueSpinEdit().GetValue()
            };
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxSeFilterTop10ValueValidationGroup");
        }
    });

    var ValidationConfirmDialog = ASPx.CreateClass(SpreadsheetDialog, {
        ConfirmTypes: {
            Stop: 0,
            Warning: 1,
            Information: 2
        },

        InitializeDialogFields: function() {
            this.selectCell(this.validationConfirm.cell);
            switch(this.validationConfirm.type) {
                case this.ConfirmTypes.Stop: {
                    this.GetYesOkButton().SetVisible(false);
                    this.GetRetryNoButton().SetVisible(true);
                    this.GetRetryNoButton().SetText("Retry");
                    this.GetRetryNoButton().Focus();
                    break;
                }
                case this.ConfirmTypes.Warning: {
                    this.GetYesOkButton().SetVisible(true);
                    this.GetRetryNoButton().SetVisible(true);
                    this.GetYesOkButton().SetText("Yes");
                    this.GetRetryNoButton().SetText("No");
                    this.GetYesOkButton().Focus();
                    break;
                }
                case this.ConfirmTypes.Information: {
                    this.GetYesOkButton().SetVisible(true);
                    this.GetRetryNoButton().SetVisible(false);
                    this.GetYesOkButton().SetText("OK");
                    this.GetYesOkButton().Focus();
                    break;
                }
            }
            this.getMessageDiv().innerHTML = this.validationConfirm.msg;
            this.GetDialogPopup().SetHeaderText(this.validationConfirm.title ? this.validationConfirm.title : "Spreadsheet");
            this.GetRetryNoButton().Click.AddHandler(function(s, e) { this.closeDialog(); this.retryInput(); }.aspxBind(this));
            this.GetYesOkButton().Click.AddHandler(function(s, e) { this.closeDialog(); this.confirmOperation(); }.aspxBind(this));
        },

        confirmOperation: function(confirmType) {
            ASPxClientSpreadsheet.ServerCommands.CellUpdate(this.spreadsheet, {
                CellPositionColumn: this.validationConfirm.cell.col, 
                CellPositionRow: this.validationConfirm.cell.row, 
                NewValue: ASPx.Str.EncodeHtml(this.validationConfirm.value),
                ReselectAfterCommand: true,
                ConfirmResult: this.validationConfirm.type === this.ConfirmTypes.Warning ? "Yes" : "OK"
            });
        },
        retryInput: function() {
            this.spreadsheet.getStateController().setEditMode(ASPxClientSpreadsheet.StateController.Modes.Edit);
            this.spreadsheet.setElementsValue(this.validationConfirm.value);
            ASPx.Selection.Set(this.spreadsheet.getEditingHelper().getEditorElement());
        },
        selectCell: function(cell) {
            var range = new ASPxClientSpreadsheet.Range(cell.col, cell.row, cell.col, cell.row),
                selection = new ASPxClientSpreadsheet.Selection(range);
            this.spreadsheet.getStateController().setSelection(selection);
        },


        setValidationConfirm: function(validationConfirm) {
            this.validationConfirm = validationConfirm;
        },

        getMessageDiv: function() {
            return document.getElementsByClassName("dxssValidationMessage")[0];
        },
        GetYesOkButton: function() {
            return this.getControlInstance("_dxeBtnOk");
        },
        GetRetryNoButton: function() {
            return this.getControlInstance("_dxBtnRetry");
        },
        GetCancelButton: function() {
            return this.getControlInstance("_dxBtnCancel");
        },
    });

    var DataValidationDialog = ASPx.CreateClass(SpreadsheetDialog, {
        Types: { 
            AnyValue: "0", 
            WholeNumber: "1",
            Decimal: "2",
            List: "3",
            Date: "4",
            Time: "5",
            TextLength: "6",
            Custom: "7"
        },
        Operators: {
            Between: "0",
            NotBetween: "1",
            EqualTo: "2",
            NotEqualTo: "3",
            GreaterThan: "4",
            LessThan: "5",
            GreaterThanOrEqualTo: "6",
            LessThanOrEqualTo: "7"
        },

        getTypeComboBox: function() {
            return this.getControlInstance("_dxCbxType");
        },
        getOperatorComboBox: function() {
            return this.getControlInstance("_dxCbxOperator");
        },
        getIgnoreBlankCheckBox: function() {
            return this.getControlInstance("_dxCbIgnoreBlank");
        },
        getInCellDropdownCheckBox: function() {
            return this.getControlInstance("_dxCbInCellDropdown");
        },
        getDataLabel: function() {
            return this.getControlInstance("_dxLblData");
        },
        getFormula1Label: function() {
            return this.getControlInstance("_dxLblFormula1");
        },
        getFormula1TextBox: function() {
            return this.getControlInstance("_dxTbFormula1");
        },
        getFormula2Label: function() {
            return this.getControlInstance("_dxLblFormula2");
        },
        getFormula2TextBox: function() {
            return this.getControlInstance("_dxTbFormula2");
        },
        getCanApplyChangesToAllCellsCheckBox: function() {
            return this.getControlInstance("_dxCbCanApplyChangesToAllCells");
        },
        getInputMessageTitleTextBox: function() {
            return this.getControlInstance("_dxTbTitle");
        },
        getInputMessageTextMemo: function() {
            return this.getControlInstance("_dxMInputMessage");
        },
        getShowInputMessageCheckBox: function() {
            return this.getControlInstance("_dxCbShowInputMessage");
        },
        getShowErrorMessageCheckBox: function() {
            return this.getControlInstance("_dxCbShowErrorMessage");
        },
        getErrorMessageTitleTextBox: function() {
            return this.getControlInstance("_dxTbErrorMessageTitle");
        },
        getErrorMessageMemo: function() {
            return this.getControlInstance("_dxMErrorMessage");
        },
        getErrorStyleComboBox: function() {
            return this.getControlInstance("_dxCbxStyle");
        },
        getPopupWindowElement: function() {
            return this.GetDialogPopup().GetWindowElement(-1);
        },

        InitializeDialogFields: function() {
            this.viewModel = {};
            this.initializeHandlers();
            this.spreadsheet.fetchDataValidationViewModelFromDialog(this);
            this.formulaLabelTexts = eval(this.getFormula1Label().cpTexts);
            this.getTypeComboBox().ValueChanged.AddHandler(function(s, e) { this.updateEditors(); }.aspxBind(this));
            this.getOperatorComboBox().ValueChanged.AddHandler(function(s, e) { this.updateEditors(); }.aspxBind(this));
            this.getCanApplyChangesToAllCellsCheckBox().CheckedChanged.AddHandler(function(s, e) { this.showDataValidationRanges(s.GetChecked()); }.aspxBind(this));
            this.getFormula1TextBox().GotFocus.AddHandler(function(s, e) { this.attachDynamicSelectionHelper(s.GetInputElement()); }.aspxBind(this));
            this.getFormula2TextBox().GotFocus.AddHandler(function(s, e) { this.attachDynamicSelectionHelper(s.GetInputElement()); }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(this.getPopupWindowElement(), "click", this.onPopupClick);
        },
        initializeHandlers: function() {
            this.onInputKeyUp = function() {
                this.spreadsheet.getDynamicSelectionHelper().refresh();
            }.aspxBind(this);
            this.onPopupClick = function(e) {
                var input1 = this.getFormula1TextBox().GetInputElement();
                var input2 = this.getFormula2TextBox().GetInputElement();
                var allowSelect = e.target === input1 || e.target === input2;
                if(!allowSelect) {
                    this.SetClientModality(true);
                    this.detachDynamicSelectionHelper();
                }
            }.aspxBind(this);
        },
        attachDynamicSelectionHelper: function(targetInput) {
            this.SetClientModality(false);
            this.detachDynamicSelectionHelper();
            
            this.spreadsheet.getDynamicSelectionHelper().attach(targetInput, false);
            ASPx.Evt.AttachEventToElement(targetInput, "keyup", this.onInputKeyUp);
            this.spreadsheet.getStateController().notifyDialogOpen();
        },
        detachDynamicSelectionHelper: function(input) {
            var input1 = this.getFormula1TextBox().GetInputElement();
            var input2 = this.getFormula2TextBox().GetInputElement();

            ASPx.Evt.DetachEventFromElement(input1, "keyup", this.onInputKeyUp);
            ASPx.Evt.DetachEventFromElement(input2, "keyup", this.onInputKeyUp);
            this.spreadsheet.getDynamicSelectionHelper().detach();
        },
        OnViewModelReceived: function(viewModel) {
            if(viewModel) {
                this.viewModel = viewModel;
                this.populateEditors();
                this.updateEditors();
            }
        },
        DialogCompletedSuccessfully: function(params) {
            if(this.getCanApplyChangesToAllCellsCheckBox().GetChecked())
                this.modifySelection();
            var selection = this.spreadsheet.getStateController().getSelection();
            this.spreadsheet.getValidationHelper().removeValidations(selection.range);
            this.executeCommand("ApplyDataValidation", { ViewModel: params.ViewModel });
        },
        OnShown: function(args) {
            this.switchContextMenuEnableMode(false);
            this.switchRibbonEnabledMode(false);
            SpreadsheetDialog.prototype.OnShown.call(this);
        },
        OnClose: function() {
            ASPx.Evt.DetachEventFromElement(this.getPopupWindowElement(), "click", this.onPopupClick);
            this.switchContextMenuEnableMode(true);
            this.switchRibbonEnabledMode(true);
            this.spreadsheet.getDynamicSelectionHelper().detach();
            this.spreadsheet.getStateController().notifyDialogClose();
            this.hideMultipleSelection();
            SpreadsheetDialog.prototype.OnClose.call(this);
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.DataValidation;
        },
        modifySelection: function() {
            if(this.viewModel.DataValidationRange.length === 0) return;
            var selection = this.spreadsheet.getStateController().getSelection();
            selection.multiSelection = true;
            selection.ranges = this.viewModel.DataValidationRange.slice(0);
        },
        getDialogData: function() {
            var result = {};
            result.ViewModel = ASPx.Json.ToJson({
                Type: this.getTypeComboBox().GetValue(),
                Operator: this.getOperatorComboBox().GetValue(),
                SuppressDropDown: false,
                IgnoreBlank: this.getIgnoreBlankCheckBox().GetChecked(),
                InCellDropDown: this.getInCellDropdownCheckBox().GetChecked(),
                Formula1: this.getFormula1TextBox().GetText(),
                Formula2: this.getFormula2TextBox().GetText(),
                ShowMessage: this.getShowInputMessageCheckBox().GetChecked(),
                MessageTitle: this.getInputMessageTitleTextBox().GetText(),
                Message: this.getInputMessageTextMemo().GetText(),
                ShowErrorMessage: this.getShowErrorMessageCheckBox().GetChecked(),
                ErrorTitle: this.getErrorMessageTitleTextBox().GetText(),
                ErrorMessage: this.getErrorMessageMemo().GetText(),
                ErrorStyle: this.getErrorStyleComboBox().GetValue()
            });
            return result;
        },
        populateEditors: function() {
            this.getOperatorComboBox().SetValue(this.viewModel.Operator);
            this.getTypeComboBox().SetValue(this.viewModel.Type);
            this.getIgnoreBlankCheckBox().SetChecked(this.viewModel.IgnoreBlank);
            this.getInCellDropdownCheckBox().SetChecked(this.viewModel.InCellDropDown);
            this.getFormula1TextBox().SetText(this.viewModel.Formula1);
            this.getFormula2TextBox().SetText(this.viewModel.Formula2);
            this.getCanApplyChangesToAllCellsCheckBox().SetEnabled(!!this.viewModel.DataValidationRange);
            this.getShowInputMessageCheckBox().SetChecked(this.viewModel.ShowMessage),
            this.getInputMessageTitleTextBox().SetText(this.viewModel.MessageTitle);
            this.getInputMessageTextMemo().SetText(this.viewModel.Message);
            this.getShowErrorMessageCheckBox().SetChecked(this.viewModel.ShowErrorMessage);
            this.getErrorMessageTitleTextBox().SetText(this.viewModel.ErrorTitle);
            this.getErrorMessageMemo().SetText(this.viewModel.ErrorMessage);
            this.getErrorStyleComboBox().SetValue(this.viewModel.ErrorStyle);
        },

        getDialogModal: function() {
            return true;
        },

        updateEditors: function() {
            var selectedType = this.getTypeComboBox().GetValue();
            this.getIgnoreBlankCheckBox().SetEnabled(selectedType !== this.Types.AnyValue);
            this.dataOperatorSetEnabled(selectedType);
            this.getInCellDropdownCheckBox().SetVisible(selectedType === this.Types.List);
            this.prepareFormulas();
        },
        dataOperatorSetEnabled: function(selectedType) {
            var enabled = selectedType !== this.Types.AnyValue &&
                selectedType !== this.Types.List && 
                selectedType !== this.Types.Custom;
            this.getOperatorComboBox().SetEnabled(enabled);
            this.getDataLabel().SetEnabled(enabled);
        },
        prepareFormulas: function() {
            var type = this.getTypeComboBox().GetValue(),
                operator = this.getOperatorComboBox().GetValue(),
                label1 = this.getFormula1Label(),
                label2 = this.getFormula2Label(),
                text1 = this.getFormula1TextBox(),
                text2 = this.getFormula2TextBox(),
                texts = this.formulaLabelTexts;

            var labels = { firstText: "", secondText: "", secondVisible: false };
            switch(type) {
                case this.Types.WholeNumber:
                case this.Types.Decimal:
                    labels = this.getFormulaLabelsByOperator(operator, texts.Minimum, texts.Value, texts.Maximum);
                    break;
                case this.Types.List:
                    labels.firstText = texts.Source;
                    break;
                case this.Types.Date:
                    labels = this.getFormulaLabelsByOperator(operator, texts.StartDate, texts.Date, texts.EndDate);
                    break;
                case this.Types.Time:
                    labels = this.getFormulaLabelsByOperator(operator, texts.StartTime, texts.Time, texts.EndTime);
                    break;
                case this.Types.TextLength:
                    labels = this.getFormulaLabelsByOperator(operator, texts.Minimum, texts.Length, texts.Maximum);
                    break;
                case this.Types.Custom:
                    labels.firstText = texts.Formula;
                    break;
            }
            label1.SetText(labels.firstText + ":");
            label2.SetText(labels.secondText + ":");
            label1.SetVisible(type !== this.Types.AnyValue);
            text1.SetVisible(type !== this.Types.AnyValue);
            label2.SetVisible(labels.secondVisible);
            text2.SetVisible(labels.secondVisible);
        },
        getFormulaLabelsByOperator: function(operator, min, equal, max) {
            var labels = { firstText: "", secondText: "", secondVisible: false };
            if(operator === this.Operators.Between ||
                operator === this.Operators.NotBetween) {
                labels.secondVisible = true;
                labels.firstText = min;
                labels.secondText = max;
            }
            else if(operator === this.Operators.GreaterThan ||
                operator === this.Operators.GreaterThanOrEqualTo)
                labels.firstText = min;
            else if(operator === this.Operators.EqualTo ||
                operator === this.Operators.NotEqualTo)
                labels.firstText = equal;
            else if(operator === this.Operators.LessThan ||
                operator === this.Operators.LessThanOrEqualTo)
                labels.firstText = max;
            return labels;
        },
        showDataValidationRanges: function(show) {
            if(show) {
                this.spreadsheet.getPaneManager().hideSelection();
                this.showMultipleSelection();
            } else {
                this.hideMultipleSelection();
                this.spreadsheet.getStateController().updateSelectionRender();
            }
        },
        showMultipleSelection: function() {
            this.multiSelection = [];
            for(var i = 0; i < this.viewModel.DataValidationRange.length; i++) {
                var validationRange = this.viewModel.DataValidationRange[i];
                var range = new ASPxClientSpreadsheet.Range(validationRange[0], validationRange[1], validationRange[2], validationRange[3]);
                var selection = new ASPxClientSpreadsheet.Selection(range);
                var selectionRect = new ASPxClientSpreadsheet.DynamicSelection(this.spreadsheet, 0);
                this.multiSelection.push(selectionRect);
                selectionRect.render(selection);
            }
        },
        hideMultipleSelection: function() {
            var selection;
            if(!this.multiSelection) return;
            while(selection = this.multiSelection.pop()) {
                selection.dispose();
            }
        }
    });

    var PageSetupDialog = ASPx.CreateClass(SpreadsheetDialog, {
        ClassNames: {
            MarginsPreviewTable: "dxssPreviewTable",
            MarginsPreviewArea: "dxssMPArea",
            VerticallyCentered: "dxssVCentered",
            HorizontallyCentered: "dxssHCentered",
            VerticalOrientation: "dxssVOrientation",
            HorizontalOrientation: "dxssHOrientation",
            HeaderPreview: "dxssPSHeader",
            FooterPreview: "dxssPSFooter",
            HFPreviewPart: "dxssHFP"
        },

        OnViewModelReceived: function(viewModel) {
            this.viewModel = viewModel;
            this.populateEditors();
        },
        InitializeDialogFields: function() {
            this.spreadsheet.fetchPageSetupViewModelFromDialog(this);
        },
        DialogCompletedSuccessfully: function(params) {
            this.executeCommand("ApplyPageSetupSettings", { pageSetupViewModel: params });
        },
        GetDialogCaptionText: function() {
            return SpreadsheetDialog.Titles.PageSetup;
        },
        submitDialog: function () {
            if (this.isDialogValid()) {
                this.dialogComplete(1, this.getDialogData());
            }
        },
        isDialogValid: function () {
            return ASPxClientEdit.ValidateGroup("_dxPageSetup_PrintArea");
        },
        getDialogData: function () {
            this.updateViewModel();

            return ASPx.Json.ToJson(this.viewModel);
        },

        updateViewModel: function() {
            for(var property in this.viewModel)
                this.updatePropertyValue(property);
        },
        updatePropertyValue: function(propertyName) {
            var editorType = propertyName.substr(0, 2),
                editor = this.getControlInstance("_" + propertyName),
                getter = this.getGetter(editorType);

            if(editor && editor[getter])
                this.viewModel[propertyName] = editor[getter]();
        },

        populateEditors: function() {
            for(var editorName in this.viewModel.DataSource)
                this.bindEditorToDataSource(editorName, this.viewModel.DataSource[editorName])

            for(var property in this.viewModel)
                this.initializeEditor(property, this.viewModel[property]);
        },
        bindEditorToDataSource: function(editorName, dataSource) {
            var editor = this.getControlInstance("_" + editorName);

            if(!(editor && dataSource))
                return;

            editor.ClearItems();
            ASPx.Data.ForEach(dataSource, function(item) {
                if(item.Text)
                    editor.AddItem(item.Text, item.Value);
                else
                    editor.AddItem(item, item);
            });
        },
        initializeEditor: function(propertyName, propertyValue) {
            var editorType = propertyName.substr(0, 2),
                editor = this.getControlInstance("_" + propertyName),
                setter = this.getSetter(editorType);

            if(editor && editor[setter]) {
                editor[setter](propertyValue);

                if(editor.OnValueChanged)
                    editor.OnValueChanged();
            }
        },
        getSetter: function(editorType) {
            var setter = "SetValue";

            if(editorType === "rl") // radioButtonList
                setter = "SetSelectedIndex";

            return setter;
        },
        getGetter: function(editorType) {
            return "GetValue";
        },

        getPopupWindowElement: function() {
            return this.GetDialogPopup().GetWindowElement(-1);
        },
        getChildElement: function(className) {
            return ASPx.GetNodesByPartialClassName(this.getPopupWindowElement(), className)[0];
        },
        getMarginsPreviewArea: function() {
            return this.getChildElement(this.ClassNames.MarginsPreviewArea);
        },
        getMarginsPreviewTable: function() {
            return this.getChildElement(this.ClassNames.MarginsPreviewTable);
        },
        getActiveBorderColorAttribute: function() {
            return "border-" + this.activeBorderLocation + "-color";
        },
        getHeaderPreview: function() {
            return this.getChildElement(this.ClassNames.HeaderPreview);
        },
        getFooterPreview: function() {
            return this.getChildElement(this.ClassNames.FooterPreview);
        },
        getPreviewParts: function(previewArea) {
            return ASPx.GetNodesByPartialClassName(previewArea, this.ClassNames.HFPreviewPart);
        },

        onMarginEditorFocused: function(boxClassName, borderLocation, isFocused) {
            this.activeBorderBox = this.getChildElement(boxClassName);
            this.activeBorderLocation = borderLocation;

            if(!this.activeBorderBox)
                return;

            if(isFocused)
                this.onSpinEditGotFocus();
            else
                this.onSpinEditLostFocus();
        },
        onSpinEditLostFocus: function() {
            ASPx.Attr.RemoveStyleAttribute(this.activeBorderBox, this.getActiveBorderColorAttribute());
        },
        onSpinEditGotFocus: function() {
            ASPx.Attr.ChangeStyleAttribute(this.activeBorderBox, this.getActiveBorderColorAttribute(), "red");
        },
        onCenterOnPageChanged: function(isVertical, isCentered) {
            var previewArea = this.getMarginsPreviewArea(),
                centerClassName = isVertical ? this.ClassNames.VerticallyCentered : this.ClassNames.HorizontallyCentered,
                changeClassMethod = isCentered ? ASPx.AddClassNameToElement : ASPx.RemoveClassNameFromElement;

            changeClassMethod(previewArea, centerClassName);
        },
        onOrientationChanged: function(isPortrait) {
            var previewTable = this.getMarginsPreviewTable(),
                existingClass = isPortrait ? this.ClassNames.HorizontalOrientation : this.ClassNames.VerticalOrientation,
                newClass = isPortrait ? this.ClassNames.VerticalOrientation : this.ClassNames.HorizontalOrientation;

            if(!previewTable) return;

            ASPx.RemoveClassNameFromElement(previewTable, existingClass);
            ASPx.AddClassNameToElement(previewTable, newClass);
        },
        onHFValueChanged: function(text, isHeader) {
            var previewArea = isHeader ? this.getHeaderPreview() : this.getFooterPreview();
            this.setHFTexts(previewArea, this.prepareHFTexts(text));
        },
        prepareHFTexts: function(text) {
            var nullTexts = ["", "", ""];

            if(text === "(none)")
                return nullTexts;

            var texts = text.split(";");
            if(texts.length < 2)
                texts.push("");
            if(texts.length < 3)
                texts.unshift("");

            return texts;
        },
        setHFTexts: function(previewArea, texts) {
            var previewParts = this.getPreviewParts(previewArea);

            for(var i = 0; i < 3; i++)
                previewParts[i].innerHTML = texts[i];
        },
        onPrintClicked: function() {
            this.submitDialog();
            this.executeCommand("Print");
        }

    });

    // Utils
    ASPx.SpreadsheetDialog = SpreadsheetDialog;
    ASPx.SpreadsheetDialog.SubmitButtonInit = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.initializeSubmitButton(s);
    };
    ASPx.SpreadsheetDialog.CancelButtonInit = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.initializeCancelButton(s);
    };
    ASPx.SpreadsheetDialog.SetChartType = function(chartId) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.setNewChartId(chartId);
    };
    ASPx.SpreadsheetDialog.SetChartLayoutId = function(chartLayoutId) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.setChartLayoutId(chartLayoutId);
    };
    ASPx.SpreadsheetDialog.SectionTypeChanged = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.typeChanged(s, e);
    };
    ASPx.SpreadsheetDialog.OpenDialogSelectedFileChanged = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.onSelectedFileChanged(s, e);
    };
    ASPx.SpreadsheetDialog.OpenDialogSelectedFileOpened = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.submitDialog();
    };
    ASPx.SpreadsheetDialog.ShowSelector = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.showSelector();
    };
    ASPx.SpreadsheetDialog.SelectorCompleted = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.initializeFSSubmitButton(s);
    };

    ASPx.SpreadsheetDialog.SelectorCanceled = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.initializeFSCancelButton(s);
    };
    ASPx.SpreadsheetDialog.SelectorBeforeShow = function (s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.selectorBeforeShow();
    };

    ASPx.SpreadsheetDialog.SelectorAfterClose = function (s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.selectorAfterClose();
    };
    ASPx.SpreadsheetDialog.SetChartStyle = function(imageName) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.setChartStyle(imageName);
    };
    ASPx.SpreadsheetDialog.InitializeChartStyle = function (s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog != null)
            curDialog.initializeChartStyle();
    };
    ASPx.SpreadsheetDialog.SetTableStyle = function(styleName) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.setTableStyle(styleName);
    };
    ASPx.SpreadsheetDialog.MarginEditorFocusChanged = function(boxClassName, borderLocation, isFocused) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.onMarginEditorFocused(boxClassName, borderLocation, isFocused);
    };
    ASPx.SpreadsheetDialog.CenterOnPageChanged = function(isVertical, isCentered) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.onCenterOnPageChanged(isVertical, isCentered);
    };
    ASPx.SpreadsheetDialog.OnOrientationChanged = function(isPortrait) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.onOrientationChanged(isPortrait);
    };
    ASPx.SpreadsheetDialog.HeaderFooterChanged = function(text, isHeader) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.onHFValueChanged(text, isHeader);
    };
    ASPx.SpreadsheetDialog.PrintClicked = function() {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.onPrintClicked();
    };
    ASPx.SpreadsheetDialog.ValuesTreeViewCheckedChanged = function() {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if (curDialog)
            curDialog.onValuesTreeViewCheckedChanged();
    };
    ASPx.SSInsertImageSrcValueChanged = function(src) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null) curDialog.OnImageSrcChanged(src);
    };
    /********Image Upload*********/
    ASPx.SSImageUploadStart = function(s, e) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null)
            return curDialog.OnImageUploadStart();
    };
    ASPx.SSImageUploadComplete = function(s, args) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null)
            return curDialog.OnImageUploadComplete(args);
    };
    /********File Upload*********/
    ASPx.SSFileUploadStart = function() {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null)
            return curDialog.OnFileUploadStart();
    };
    ASPx.SSFileUploadComplete = function(args) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null)
            return curDialog.OnFileUploadComplete(args);
    };
    ASPx.SSFileUploadTextChanged = function() {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
    };
    // Test image size
    ASPx.SSTestExistingImageOnLoad = function(name) {
        var spreadsheet = ASPx.GetControlCollection().Get(name);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null) curDialog.OnLoadTestExistingImage();
    };
    ASPx.SSTestExistingImageOnError = function(name) {
        var spreadsheet = ASPx.GetControlCollection().Get(name);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null) curDialog.OnErrorTestExistingImage();
    };
    //Find
    ASPx.SSFindAll = function(params) {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null)
            curDialog.OnFind(params);
    };
    ASPx.SSSearchItemClicked = function() {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
        if(curDialog != null)
            curDialog.ScrollToCell();
    };
    ASPx.SSSearchResultKeyDown = function(s, e) {
        if(e.htmlEvent.keyCode == 13) { 
            ASPx.Evt.PreventEventAndBubble(e.htmlEvent); 
            var handlerMethodName = e.htmlEvent.shiftKey ? "OnArrowUp" : "OnArrowDown";
            s[handlerMethodName](e.htmlEvent);
            ASPx.SSSearchResultScrollToItem(); 
        }
    };
    var searchItemSelectedIndexChangedTimerId = -1;
    ASPx.SSSearchResultScrollToItem = function(s, e) {
        ASPx.Timer.ClearTimer(searchItemSelectedIndexChangedTimerId);
        searchItemSelectedIndexChangedTimerId = window.setTimeout(ASPx.SSSearchItemClicked, 100);
    };
    // Focusing dialog elements
    function _aspxSetFocusToElement(name) {
        window.setTimeout(function() {
            var edit = ASPx.GetControlCollection().Get(name);
            if(!edit)
                return;            
            edit.SetFocus();
        }, 300);
    }
    // Initialization internal controls
    ASPx.SSBeginInitializeInternalControls = function() {
        var currentDialog = aspxGetCurrentDialog();
        if(currentDialog)
            currentDialog.beginInitializeInternalControls();
    };
    ASPx.SSEndInitializeInternalControls = function() {
        var currentDialog = aspxGetCurrentDialog();
        if(currentDialog)
            currentDialog.endInitializeInternalControls();
    };
    function aspxGetCurrentDialog() {
        var spreadsheet = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        return spreadsheet != null ? ASPx.Dialog.GetLastDialog(spreadsheet) : null;
    }
    // Helpers
    ASPx.SSGetFullFileName = function(folderPath) {
        var pathSeparator = "\\";
        folderPath += pathSeparator;
        return folderPath.replace(new RegExp("\\\\+|\/|^\.\.\\\\", "g"), pathSeparator);
    };

    // ** DialogList **
    var CreateDialogList = function() {
        var SpreadsheetDialogList = {};
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.InsertHyperlink.Command).id] = new InsertHyperlinkDialog(SpreadsheetDialogs.InsertHyperlink.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.InsertPicture.Command).id] = new InsertPictureDialog(SpreadsheetDialogs.InsertPicture.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.RenameSheet.Command).id] = new RenameSheetDialog(SpreadsheetDialogs.RenameSheet.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.EditHyperlink.Command).id] = new InsertHyperlinkDialog(SpreadsheetDialogs.EditHyperlink.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.FileOpen.Command).id] = new OpenFileDialog(SpreadsheetDialogs.FileOpen.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.FileSaveAs.Command).id] = new SaveFileDialog(SpreadsheetDialogs.FileSaveAs.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.RowHeight.Command).id] = new RowHeightDialog(SpreadsheetDialogs.RowHeight.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ColumnWidth.Command).id] = new ColumnWidthDialog(SpreadsheetDialogs.ColumnWidth.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.DefaultColumnWidth.Command).id] = new DefaultColumnWidthDialog(SpreadsheetDialogs.DefaultColumnWidth.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.UnhideSheet.Command).id] = new UnhideSheetDialog(SpreadsheetDialogs.UnhideSheet.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartType.Command).id] = new ChangeChartTypeDialog(SpreadsheetDialogs.ChartType.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartData.Command).id] = new ChartSelectDataDialog(SpreadsheetDialogs.ChartData.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartLayout.Command).id] = new ModifyChartLayoutDialog(SpreadsheetDialogs.ChartLayout.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartTitle.Command).id] = new ChartChangeTitleDialog(SpreadsheetDialogs.ChartTitle.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartHorizontalAxisTitle.Command).id] = new ChartChangeHorizontalAxisTitleDialog(SpreadsheetDialogs.ChartHorizontalAxisTitle.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartVerticalAxisTitle.Command).id] = new ChartChangeVerticalAxisTitleDialog(SpreadsheetDialogs.ChartVerticalAxisTitle.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ChartStyle.Command).id] = new ModifyChartStyleDialog(SpreadsheetDialogs.ChartStyle.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.FindAll.Command).id] = new FindAndReplaceDialog(SpreadsheetDialogs.FindAll.Name);

        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("InsertTable").id] = new InsertTableDialog(SpreadsheetDialogs.InsertTable.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("InsertTableWithStyle").id] = new InsertTableWithStyleDialog(SpreadsheetDialogs.InsertTable.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterSimple").id] = new DataFilterSimpleDialog(SpreadsheetDialogs.DataFilterSimple.Name);

        var customDataFilterDialog = new CustomDataFilterDialog(SpreadsheetDialogs.CustomDataFilter.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterEquals").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDoesNotEqual").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterGreaterThan").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterGreaterThanOrEqualTo").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterLessThan").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterLessThanOrEqualTo").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterBetween").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterBeginsWith").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterEndsWith").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterContains").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDoesNotContain").id] = customDataFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterCustom").id] = customDataFilterDialog;

        var customDateTimeFilterDialog = new CustomDateTimeFilterDialog(SpreadsheetDialogs.CustomDateTimeFilter.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDateEquals").id] = customDateTimeFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDateBefore").id] = customDateTimeFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDateAfter").id] = customDateTimeFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDateBetween").id] = customDateTimeFilterDialog;
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterDateCustom").id] = customDateTimeFilterDialog;

        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("DataFilterTop10").id] = new DataFilterTop10Dialog(SpreadsheetDialogs.DataFilterTop10.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.DataValidation.Command).id] = new DataValidationDialog(SpreadsheetDialogs.DataValidation.Name);
        SpreadsheetDialogList[SpreadsheetDialogs.ValidationConfirm.Name] = new ValidationConfirmDialog(SpreadsheetDialogs.ValidationConfirm.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.MoveOrCopySheet.Command).id] = new MoveOrCopySheetDialog(SpreadsheetDialogs.MoveOrCopySheet.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.ModifyTableStyle.Command).id] = new ModifyTableStyleDialog(SpreadsheetDialogs.ModifyTableStyle.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.FormatAsTable.Command).id] = new FormatAsTableDialog(SpreadsheetDialogs.ModifyTableStyle.Name);
        SpreadsheetDialogList[ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(SpreadsheetDialogs.PageSetup.Command).id] = new PageSetupDialog(SpreadsheetDialogs.PageSetup.Name);
        return SpreadsheetDialogList;
    };


    ASPxClientSpreadsheet.CreateDialogList = CreateDialogList;
})();