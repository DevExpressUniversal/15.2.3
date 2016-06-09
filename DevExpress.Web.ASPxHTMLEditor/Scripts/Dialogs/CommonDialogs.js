(function() {
    var InsertDialogBase = ASPx.HtmlEditorClasses.Dialogs.InsertDialogBase;
    var executeIfExists = ASPx.HtmlEditorClasses.Utils.executeIfExists;
    ASPx.HtmlEditorClasses.defaultLinkHref = "";

    var mailtToPrefix = "mailto:";
    var mailtToSubjectPrefix = "?subject=";
    var openInNewWindowTarget = "_blank";

    var InsertLinkDialog = ASPx.CreateClass(InsertDialogBase, {
        getResourceUrlPropertyName: function() {
            return "href";    
        },
        createCommandArgument: function(params) {
            var args = new ASPxClientHtmlEditorInsertLinkCommandArguments(this.htmlEditor);
            args.url = params.url;
            args.text = params.text;
            args.title = params.title;
            args.target = params.target;
            return args;    
        },
        hasEmailSection: function() {
            return !!this.getControl("RdBttnListSectionsSwitcher");        
        },
        isURLSectionActive: function() {
            return !this.hasEmailSection() || this.getSectionsSwitcher().GetValue() == "URLRadioButton";
        },
        getObjectSettings: function (settings) {
            var settings = InsertDialogBase.prototype.getObjectSettings.call(this, settings)
            this.prepareHrefSetting(settings);
            if (this.isTextEditorVisible)
                this.prepareTextSetting(settings);
            else
                settings.text = "";
            if (settings.target)
                settings.target = openInNewWindowTarget;
            return settings;
        },
        prepareTextSetting: function (settings) {
            if (!settings.text) {
                if (this.isURLSectionActive())
                    settings.text = this.getURLTextBox().GetValue();
                else
                    settings.text = this.getEmailTextBox().GetValue();
            }
        },
        prepareHrefSetting: function (settings) {
            if (this.isURLSectionActive())
                settings.url = this.getURLTextBox().GetValue();
            else {
                settings.url = mailtToPrefix + this.getEmailTextBox().GetValue();
                var subject = this.getSubjectTextBox().GetValue();
                if(subject)
                    settings.url += mailtToSubjectPrefix + subject;
            }
        },
        initializeMailSection: function (href) {
            this.getSectionsSwitcher().SetSelectedIndex(1);
            this.getSectionsSwitcher().SelectedIndexChanged.FireEvent(this);
            var mailAddress = href.split('?')[0].replace(mailtToPrefix, '');
            this.getEmailTextBox().SetValue(mailAddress);
            var hasMailSubject = href.indexOf(mailtToSubjectPrefix) > 0;
            if (hasMailSubject) {
                var mailSubject = href.split('?')[1].replace('subject=', '');
                mailSubject = (mailSubject == 'null') ? '' : mailSubject;
                this.getSubjectTextBox().SetValue(mailSubject);
            }
        },
        InitializeDialogFields: function (linkInfo) {
            InsertDialogBase.prototype.InitializeDialogFields.call(this, linkInfo);
            if (!!linkInfo.href) {
                var isMail = linkInfo.href.indexOf(mailtToPrefix) == 0 && this.hasEmailSection();
                if (isMail) {
                    this.initializeMailSection(linkInfo.href);
                    this.getURLTextBox().Clear();
                }
                else {
                    this.getURLTextBox().SetValue(linkInfo.href);
                }
            }
            this.isTextEditorVisible = linkInfo.isTextEditorVisible;

            executeIfExists(this.getMainFormLayout(), function (mainFormLayout) {
                var textItem = mainFormLayout.GetItemByName('TextItem');
                if (textItem && textItem.SetVisible)
                    textItem.SetVisible(linkInfo.isTextEditorVisible)
            }.aspxBind(this));
        },
        SetFocusInField: function () {
            this.isURLSectionActive() ? this.getURLTextBox().Focus() : this.getEmailTextBox().Focus();
        },
        GetDialogCaptionText: function () {
            return ASPx.HtmlEditorClasses.Utils.IsLink(this.getSelectedElement()) ? ASPx.HtmlEditorDialogSR.ChangeLink : ASPx.HtmlEditorDialogSR.InsertLink;
        },
        getDialogPropertiesMap: function () {
            return {
                "href": "SelectDocumentPopupButtonEdit",
                "title": "ToolTipItem",
                "target": "OpenInNewWindowCheckbox",
                "text": "TextItem"
            };
        },
        GetInitInfoObject: function () {
            var infoObject = {};
            var selectedElement = this.getSelectedElement();
            if (selectedElement) {
                var link = selectedElement.tagName == "A" ? selectedElement : ASPx.GetParentByTagName(selectedElement, "A");
                infoObject.isTextEditorVisible = true;
                if (link) {
                    var objectProperties = this.getDialogPropertiesMap();
                    for (var p in objectProperties)
                        infoObject[p] = ASPx.Attr.GetAttribute(link, p);
                    infoObject.isTextEditorVisible = this.containtTextNodeOnly(link) && !this.selectionInfo.isControl;
                }
                else {
                    infoObject.isTextEditorVisible = !ASPx.GetParentByTagName(selectedElement, "img") && 
                        !ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName) && this.canEditText();
                }
                infoObject.text = link ? link.innerHTML : this.selectionInfo.text;
                infoObject.href = infoObject.href || ASPx.HtmlEditorClasses.defaultLinkHref;
            }
            return infoObject;
        },
        canEditText: function () {
            var text = this.selectionInfo.text;
            if (text) text = this.selectionInfo.htmlText;
            return !ASPx.HtmlEditorClasses.Selection.IsHtml(ASPx.Str.Trim(text));
        },
        attachEvents: function () {
            var urlTextBox = this.getURLTextBox();
            urlTextBox.TextChanged.AddHandler(this.RaiseValueChanged.aspxBind(this));
            if (urlTextBox.GetButton(0)) {
                urlTextBox.ButtonClick.AddHandler(this.showSelectDocumentPopup.aspxBind(this));
                this.getSelectDocumentPopupCancelButton().Click.AddHandler(this.hideSelectDocumentPopup.aspxBind(this));
                this.getSelectDocumentPopupSelectButton().Click.AddHandler(this.selectDocument.aspxBind(this));
                this.getFileManager().SelectedFileOpened.AddHandler(this.selectDocument.aspxBind(this));
                this.getFileManager().SelectedFileChanged.AddHandler(this.setSelectButtonEnabling.aspxBind(this));
            }
            executeIfExists(this.getControl("EmailTextBox"), function (textBox) {
                textBox.TextChanged.AddHandler(this.RaiseValueChanged.aspxBind(this));
            }.aspxBind(this));
            executeIfExists(this.getControl("RdBttnListSectionsSwitcher"), function (rbl) {
                rbl.SelectedIndexChanged.AddHandler(this.switchSectionsVisibility.aspxBind(this));
            }.aspxBind(this));
        },
        isDialogValid: function (skipInnerValidation) {
            return skipInnerValidation || ASPxClientEdit.ValidateGroup("mediaDialogValidationGroup", false);
        },
        setSelectButtonEnabling: function (s, e) {
            this.getSelectDocumentPopupSelectButton().SetEnabled(!!e.file);
        },
        switchSectionsVisibility: function () {
            var flag = this.isURLSectionActive();
            var mainFormLayout = this.getMainFormLayout();
            mainFormLayout.GetItemByName('URLSection').SetVisible(flag);
            executeIfExists(this.getOpenInNewWindowCheckbox(), function (checkBox) {
                checkBox.SetVisible(flag);
            }.aspxBind(this));
            if(this.hasEmailSection()) {
                mainFormLayout.GetItemByName('EmailTextBox').SetVisible(!flag);
                mainFormLayout.GetItemByName('SubjectTextBox').SetVisible(!flag);
            }
        },
        selectDocument: function (s, e) {
            var documentUrl = this.getFileManager().cp_RootFolderRelativePath + this.getFileManager().GetSelectedFile().GetFullName("\/", true);
            documentUrl = ASPx.HtmlEditorClasses.GetPreparedUrl(this.htmlEditor, documentUrl);

            this.getURLTextBox().SetText(documentUrl);
            this.hideSelectDocumentPopup();
            this.getURLTextBox().TextChanged.FireEvent(this);
        },
        showSelectDocumentPopup: function () {
            this.getFileManager().setOwner(this.htmlEditor);
            this.getSelectDocumentPopup().Show();
            this.getFileManager().Refresh();
        },
        hideSelectDocumentPopup: function () {
            this.getSelectDocumentPopup().Hide();
        },
        getEditorsWithNullText: function() {
            return [ this.getURLTextBox() ];    
        },
        getEmailTextBox: function () {
            return this.getControl("EmailTextBox");
        },
        getSubjectTextBox: function () {
            return this.getControl("SubjectTextBox");
        },
        getURLTextBox: function () {
            return this.getControl("ShowingPopupButtonEdit");
        },
        getFileManager: function () {
            return this.getControl("FileManager");
        },
        getSelectDocumentPopupCancelButton: function () {
            return this.getControl("PopupCancelButton");
        },
        getSelectDocumentPopupSelectButton: function () {
            return this.getControl("PopupSelectButton");
        },
        getSelectDocumentPopup: function () {
            return this.getControl("SelectDocumentPopup");
        },
        getOpenInNewWindowCheckbox: function () {
            return this.getControl("OpenInNewWindowCheckbox");
        },
        getMainFormLayout: function () {
            return this.getControl("FormLayout");
        },
        getSectionsSwitcher: function () {
            return this.getControl("RdBttnListSectionsSwitcher");
        },
        containtTextNodeOnly : function(linkElement) {
            if(linkElement.childNodes.length > 0) {
                for(var i = 0; i < linkElement.childNodes.length; i++) {
                    if(linkElement.childNodes[i].nodeType != 3)
                        return false;
                }
            }
            return true;
        }
    });

    var InsertPlaceholderDialog = ASPx.CreateClass(InsertDialogBase, {
        getObjectSettings: function() {
            return this.getControl("PlaceholderName").GetValue();
        },
        GetInitInfoObject: function() {
            var selectedElement = this.selectionInfo.selectedElement;
            if(selectedElement && ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName)) {
                var innerText = ASPx.Str.Trim(ASPx.GetInnerText(selectedElement));
                return { selectedPlaceholderName: innerText.replace(/^{\s*([\s\S]*?)\s*}$/, "$1") };
            }
            return {};
        },
        InitializeDialogFields: function(objectInfo) {
            var wrapper = this.htmlEditor.getDesignViewWrapper();
            if(!wrapper || wrapper.settings.placeholders && wrapper.settings.placeholders.length == 0)
                return;
            var placholderNameControl = this.getControl("PlaceholderName");
            placholderNameControl.ValueChanged.AddHandler(this.RaiseValueChanged.aspxBind(this));
            placholderNameControl.ItemDoubleClick.AddHandler(this.submitDialog.aspxBind(this));
	        for(var i = 0, item; item = wrapper.settings.placeholders[i]; i++)
                placholderNameControl.AddItem(item, item);
            if(objectInfo.selectedPlaceholderName) {
                placholderNameControl.SetValue(objectInfo.selectedPlaceholderName);
                this.RaiseValueChanged();
            }
        },
        SetFocusInField: function() {
            this.getControl("PlaceholderName").Focus();
        },
        RestoreEditorsState: function() {
            this.getControl("PlaceholderName").Focus();
        },
        GetDialogCaptionText: function () {
            var selectedElement = this.getSelectedElement();
            return selectedElement.nodeType != 3 && ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName) ? ASPx.HtmlEditorDialogSR.ChangePlaceholder : ASPx.HtmlEditorDialogSR.InsertPlaceholder;
        },
        isDialogValid: function(skipInnerValidation) {
            return !!this.getControl("PlaceholderName").GetValue();    
        }
    });

    var ASPxCustomDialog = ASPx.CreateClass(ASPx.HtmlEditorDialog, {
        constructor: function(customDialogName, customDialogPublicName) {
            this.constructor.prototype.constructor.call(this, customDialogName);
            this.publicName = customDialogPublicName;
        },
        canRemoveDialogHtmlAfterClose: function() {
            return false;    
        },
        OnCallbackError: function(result, data) {
            ASPx.HtmlEditorDialog.prototype.OnCallbackError.call(this, result, data);
            this.htmlEditor.getCommandManager().getCommand(ASPxClientCommandConsts.CUSTOMDIALOG_COMMAND).DialogNotFound(this.name);
        },
        GetDialogCaptionText: function() {
            return this.htmlEditor.customDialogsCaptions[this.name] || "";
        },
        OnInitComplete: function() {
            this.RestoreDocumentScrollPosition();
            ASPx.HtmlEditorDialog.prototype.OnInitComplete.call(this);
            setTimeout(function() {
                this.htmlEditor.RaiseCustomDialogOpened(this.publicName);
            }.aspxBind(this), 0);
        },
        OnClosing: function(args) {
            ASPx.HtmlEditorDialog.prototype.OnClosing.call(this);
            if(args)
                args.cancel = true;
            var canceled = this.htmlEditor.RaiseCustomDialogClosing(this.publicName, "cancel");
            if(!canceled)
                ASPxClientHtmlEditor.CustomDialogComplete("cancel", null);
            return canceled;
        },
        DoCustomAction: function(result, data) {
            ASPx.HtmlEditorDialog.prototype.DoCustomAction.call(this);
            this.htmlEditor.RaiseCustomDialogClosed(this.publicName, result, data);
        }
    });
    
    var PasteFromWordDialog = ASPx.CreateClass(InsertDialogBase, {
        // cmdValue = { html, stripFontFamily };
        getObjectSettings: function() {
            var settings = InsertDialogBase.prototype.getObjectSettings.call(this);
            settings.html = ASPx.IFrameHelper.GetDocumentBody(this.GetPasteContainerIFrameName()).innerHTML;
            return settings;
        },
        releaseResources: function() {
            ASPx.RemoveElement(this.GetPasteContainerIFrame());
        },
        getDialogPropertiesMap: function() {
            return { "stripFontFamily": "StripFont" };    
        },
        isDialogValid: function(skipInnerValidation) {
            return true;    
        },
        attachEvents: function() {
            InsertDialogBase.prototype.attachEvents.call(this);
            ASPx.Evt.AttachEventToElement(this.GetPasteContainerIFrame().contentWindow.document, "keypress", function(evt) {
                this.OnPasteContainerKeyPress(evt);        
            }.aspxBind(this));
        },
        InitializeDialogFields: function() {
            // iframe
            var iframe = this.GetPasteContainerIFrame();
            if(!iframe.contentWindow) { // B156982
                var src = iframe.src;
                iframe.src = src;
            }
            var contentDocument = this.GetPasteContainerIFrame().contentWindow.document;
            
		    contentDocument.open();
		    contentDocument.write("<head><style></style></head><body></body>");
		    contentDocument.close();
		
		    if(ASPx.Browser.IE)
		        contentDocument.body.contentEditable = true;
		    else {
		        contentDocument.body.spellcheck = false;
		        contentDocument["designMode"] = "on";
		    }
		    
            ASPx.SetStyles(contentDocument.body, {
                margin: "0px",
                padding: "2px",
                border: "Solid 0px"
            });

            var designViewDoc = this.htmlEditor.GetDesignViewDocument();
            contentDocument.body.className = designViewDoc.body.className;
            var childNodes = ASPx.GetHeadElementOrCreateIfNotExist(designViewDoc).childNodes;
            var headElement = ASPx.GetHeadElementOrCreateIfNotExist(contentDocument);
            for(var i = 0, child; child = childNodes[i]; i++) {
                var nodeName = child.nodeName.toLowerCase();
                if((nodeName == "style" && child.innerHTML) || (nodeName == "link"))
                    headElement.appendChild(child.cloneNode(true));
            }
        },
        SetFocusInField: function() {
            window.setTimeout(function() {
                ASPx.IFrameHelper.GetWindow(this.GetPasteContainerIFrameName()).focus();
            }.aspxBind(this), 500);
        },
        GetDialogCaptionText: function() {
            return ASPx.HtmlEditorDialogSR.PasteFromWord;
        },
        // Event Handlers
        OnPasteContainerKeyPress: function(evt) {
            this.RaiseValueChanged();
            if(evt.keyCode == ASPx.Key.Esc)
                this.HideDialog(evt);
        },
        GetPasteContainerIFrame: function() {
            return ASPx.GetElementById(this.GetPasteContainerIFrameName());
        },
        GetPasteContainerIFrameName: function() {
            return this.htmlEditor.name + "_dxePasteFromWordContainer";
        }
    });

    var ChangeElementPropertiesDialog = ASPx.CreateClass(InsertDialogBase, {
        getDialogPropertiesMap: function() {
            return {
                "attributes": {
                    "id": "Id",
                    "title": "Title",
                    "direction": "Direction",
                    "value": "Value",
                    "tabindex": "TabIndex",
                    "disabled": "Disabled",
                    "type": "InputType",
                    "for": "For",
                    "name": "Name",
                    "method": "Method",
                    "action": "Action",
                    "checked": "Checked",
                    "maxlength": "MaxLength",
                    "size": "Size",
                    "readonly": "Readonly",
                    "src": "Src",
                    "accept": "Accept",
                    "alt": "Alt"
                },
                "styles": {
                    "className": "CssClassName",
                    "marginTop": "TopMargin",
                    "marginBottom": "BottomMargin",
                    "marginLeft": "LeftMargin",
                    "marginRight": "RightMargin",
                    "borderWidth": "BorderWidth",
                    "borderStyle": "BorderStyle",
                    "borderColor": "BorderColor"
                }
            };
        },
        getItemsVisibilityInfo: function () {
            return {
                "Value": ["INPUT", "SELECT", "LI"],
                "InputType": ["INPUT"],
                "Disabled": ["INPUT", "SELECT", "TEXTAREA"],
                "Action": ["FORM"],
                "Method": ["FORM"],
                "Name": ["FORM", "INPUT"],
                "For": ["LABEL"],
                "TabIndex": ["INPUT", "SELECT", "TEXTAREA"],
                "Checked": ["INPUT"],
                "MaxLength": ["INPUT"],
                "Size": ["INPUT"],
                "Readonly": ["INPUT"]
            };
        },
        attachEvents: function () {
            InsertDialogBase.prototype.attachEvents.call(this);
            executeIfExists(this.getControl("InputType"), function (InputTypeComboBox) {
                InputTypeComboBox.ValueChanged.AddHandler(this.setInputEditorsVisibility.aspxBind(this));
            }.aspxBind(this));
        },
        setInputEditorsVisibility: function () {
            if(this.getControl("InputType").IsVisible()) {
                var visibilityInfo = this.getInputEditorsVisibilityInfo();
                var inputType = this.getControl("InputType").GetValue();
                this.setEditorsVisibility(visibilityInfo, inputType);
            }
        },
        getInputEditorsVisibilityInfo: function () {
            return {
                "MaxLength": ["text", "password"],
                "Size": ["text", "password"],
                "Checked": ["checkbox", "radio"],
                "Readonly": ["text", "password"],
                "Alt": ["image"],
                "Src": ["image"],
                "Accept": ["file"],
                "hideEditors": {
                    "TabIndex": ["hidden"],
                    "Title": ["hidden"],
                    "Direction": ["hidden"],
                    "Disabled": ["hidden"],
                    "StyleProperties": ["hidden"]
                }
            }
        },
        removeDeafultStyles: function (settings) {
            for (var styleName in this.oldStyleInfoObject) {
                var styleChanged = this.oldStyleInfoObject[styleName] != settings.styles[styleName];
                //should always refresh className. if user remove className value will be 'null'. if not change value is 'undefined'.it unclear
                if (styleName != "className" && !styleChanged)
                    delete settings.styles[styleName];
            }
            return settings;
        },
        getObjectSettings: function (settings) {
            var settings = InsertDialogBase.prototype.getObjectSettings.call(this, settings);
            settings.selectedElement = this.getSelectedElement();
            settings = this.removeDeafultStyles(settings);
            settings.styles = ASPx.HtmlEditorClasses.Utils.RemoveStylesDuplicates(settings.styles, this.htmlEditor.GetDesignViewDocument());
            return settings;
        },
        GetInitInfoObject: function () {
            var infoObject = {
                attributes: {},
                styles: {}
            };
            var selectedElement = this.getSelectedElement();

            if (selectedElement) {
                var objectProperties = this.getDialogPropertiesMap();
                for (var p in objectProperties.attributes) {
                    infoObject.attributes[p] = ASPx.Attr.GetAttribute(selectedElement, p);
                }
                
                infoObject.styles = this.getStyleInfoObject(selectedElement);
                infoObject.styles.className = ASPx.Attr.GetAttribute(selectedElement, "class");
                
                for (var p in objectProperties.styles) {
                    var value = selectedElement.style[p];
                    if (ASPx.IsExists(value) && value != "")
                        infoObject.styles[p] = value;
                }
            }
            return infoObject;
        },
        setEditorsVisibility: function (visibilityInfo, regime) {
            for (var k in visibilityInfo) {
                var info = visibilityInfo[k];
                if (k == "hideEditors") {
                    for (var d in info) {
                        this.getFormLayout().GetItemByName(d).SetVisible(info[d].indexOf(regime) == -1);
                    }
                }
                else
                    this.getFormLayout().GetItemByName(k).SetVisible(ASPx.Data.ArrayIndexOf(info, regime) > -1);
            }
        },
        InitializeDialogFields: function (objectInfo) {
            InsertDialogBase.prototype.InitializeDialogFields.call(this, objectInfo);
            var selectedElement = this.getSelectedElement();
            if(!selectedElement)
                return;
            var tagName = selectedElement.tagName;
            var visibilityInfo = this.getItemsVisibilityInfo();
            this.setEditorsVisibility(visibilityInfo, tagName);
            this.setInputEditorsVisibility();
        },
        getStyleInfoObject: function (selectedElement) {
            var styleInfoObject = {
                "marginTop": "int",
                "marginRight": "int",
                "marginBottom": "int",
                "marginLeft": "int",
                "borderWidth": "int",
                "borderStyle": "string",
                "borderColor": "color"
            };
            styleInfoObject = ASPx.HtmlEditorClasses.Utils.GetSelectedElementComputedStyle(selectedElement, styleInfoObject);
            this.oldStyleInfoObject = styleInfoObject;
            return styleInfoObject;
        },
        GetDialogCaptionText: function() {
            return "Change Element Properties";    
        }
    });
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEELEMENTPROPERTIES_DIALOG_COMMAND] = new ChangeElementPropertiesDialog(ASPxClientCommandConsts.CHANGEELEMENTPROPERTIES_DIALOG_COMMAND);

    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.PASTEFROMWORDDIALOG_COMMAND] = new PasteFromWordDialog(ASPxClientCommandConsts.PASTEFROMWORDDIALOG_COMMAND);

    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTPLACEHOLDER_DIALOG_COMMAND] = new InsertPlaceholderDialog(ASPxClientCommandConsts.INSERTPLACEHOLDER_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEPLACEHOLDER_DIALOG_COMMAND] = new InsertPlaceholderDialog(ASPxClientCommandConsts.CHANGEPLACEHOLDER_DIALOG_COMMAND);
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTLINK_DIALOG_COMMAND] = new InsertLinkDialog(ASPxClientCommandConsts.INSERTLINK_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGELINK_DIALOG_COMMAND] = new InsertLinkDialog(ASPxClientCommandConsts.INSERTLINK_DIALOG_COMMAND);
    
    ASPx.HtmlEditorClasses.Dialogs.ASPxCustomDialog = ASPxCustomDialog;
    
})();