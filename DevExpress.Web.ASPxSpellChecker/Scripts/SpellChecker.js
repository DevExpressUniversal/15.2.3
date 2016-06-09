

(function() {
    ASPx.activeSpellChecker = null;

    var formPostFix = ":SCFPR:"; //TODO
    var optionsPrefix = "Options:";
    var errorMarker = "^^^DXError^^^";
    var callbackArgumentToGetSpellCheckOptionsFormContent = "DialogForm:SpellCheckOptionsForm";
    var callbackArgumentToGetSpellCheckFormContent = "DialogForm:SpellCheckForm";
    var ASPxClientSpellChecker = ASPx.CreateClass(ASPxClientControl, {
    
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackError = false;

            this.checkingText = null;
            this.anchorElement = null;
            this.editorElement = null;
            this.checkedContainer = null;
            this.checkedText = "";
        
            this.editorHelper = new ASPxEditorHelper();
            this.containerBrowser  = new ASPxContainerBrowser();
            this.formHandler = null;
        
            this.spellCheckForm = null;
            this.optionsForm = null;
            this.isOptionsForm = false;
            this.preventHideEvents = false;
                
            // from server
            this.checkedElementID = "";
            this.showOneWordInTextPreview = false;
            this.finishSpellChecking = "";
            this.spellCheckFormCaption = "";
            this.optionsFormCaption = "";
            this.noSuggestionsText = "";
            this.BeforeCheck = new ASPxClientEvent();                
            this.CheckCompleteFormShowing = new ASPxClientEvent();
            this.AfterCheck = new ASPxClientEvent();
            this.WordChanged = new ASPxClientEvent();
        },
        InlineInitialize: function() {
            this.constructor.prototype.InlineInitialize.call(this);

            this.finishSpellChecking = unescape(this.finishSpellChecking);
            this.spellCheckFormCaption = unescape(this.spellCheckFormCaption);
            this.optionsFormCaption = unescape(this.optionsFormCaption);
            this.noSuggestionsText = unescape(this.noSuggestionsText);
        },
        IsDOMDisposed: function() {
            var spellCheckFormPopup = this.GetSpellCheckForm().GetDialogPopup();
            return !spellCheckFormPopup || spellCheckFormPopup.IsDOMDisposed();
        },
        Check: function() {        
            this.CheckElementById(this.checkedElementID);
        },
        CheckElement: function(element) {    
            if(!element) return;
        
            var inputElement = this.FindDxEditorInputElement(element);
            if (inputElement != null)
                element = inputElement;
            if(this.CanCheckControl(element.id))
                this.CheckElementCore(element);
            else
                this.ContinueOrFinishCheck(element);
        },
        CheckElementById: function(id) {
            var element = this.FindDxEditorInputElementById(id);
            if(!element)
                element = ASPx.GetElementById(id);
            this.CheckElement(element);
        },
        CheckElementsInContainer: function(containerElement) {
            if(containerElement) {
                this.checkedContainer = containerElement;
                var editor = this.FindNextEditorInContainer(containerElement, null);
                if(editor)
                    this.CheckElement(editor);
                else
                    this.ShowFinishMessageBox();
            }        
        },
        CheckElementsInContainerById: function(containerId) {
            var container = ASPx.GetElementById(containerId);
            this.CheckElementsInContainer(container);
        },
        CheckText: function(text, anchorElement) { 
            this.CheckCore(text, anchorElement, null, null);
        },    
    
        /*region* * * * * * * * * * * * * * * * * *  Get/Set  * * * * * * * * * * * * * * * * * */    
    
        FindDxEditorInputElement: function(obj) {
            if(ASPx.Ident.IsASPxClientEdit(obj))
                return obj.GetInputElement();
            else if(ASPx.IsExistsElement(obj) && obj.id)
                return ASPx.GetChildById(obj, obj.id + "_I"); // for DX editors without a client object
            return null;
        },
        FindDxEditorInputElementById: function(id) {
            var control = ASPx.GetControlCollection().Get(id);
            if(control != null && ASPx.Ident.IsASPxClientEdit(control))
                return control.GetInputElement();
            return null;
        },    
        GetEditorID: function(element) {
            var id = "";
            if (element) {
                var editor = this.editorHelper.GetDXEditorByInput(element);
                id = editor ? editor.name : element.id;
            }
            return id;    
        },    
        GetDialogPopupControl: function(name) {
            return ASPx.GetControlCollection().Get(this.name + "_" + name);
	    },
        GetSpellCheckForm: function() {
            if(!this.spellCheckForm)
                this.spellCheckForm = new ASPx.SpellCheckForm("SpellCheckForm", this);
            return this.spellCheckForm;
        },
        GetOptionsForm: function() {
            if(!this.optionsForm)
                this.optionsForm = new ASPx.SpellCheckOptionsForm("SpellCheckOptionsForm", this);
            return this.optionsForm;
        },    
        GetStateHiddenFieldOrigin: function() {
            return this.GetSpellCheckForm().GetDialogPopup().GetWindowContentElement(-1);
        },


        /*region* * * * * * * * * * * * * * * * * *  Checking  * * * * * * * * * * * * * * * * * */
    
        CanCheckControl: function(id) {
            return this.RaiseBeforeCheck(id);
        },      
     
        ContinueCheckContainer: function(nextEditor) {
            if (this.editorElement)
                this.RaiseAfterCheck(this.GetEditorID(this.editorElement), this.checkingText);
            this.checkedText = null;
            this.CheckElement(nextEditor);
        },
        ContinueOrFinishCheck: function(currentElement) {
            if(this.checkedContainer){        
                var editor = this.FindNextEditorInContainer(this.checkedContainer, currentElement);
                if(editor){
                    this.ContinueCheckContainer(editor);
                    return;
                }
                else 
                    this.RaiseAfterCheck(this.GetEditorID(currentElement), this.checkingText);
            }
            var spellCheckForm = this.GetSpellCheckForm();
            if(spellCheckForm.IsVisible() != null)
                spellCheckForm.GetDialogPopup().Hide();
            if(!this.checkedContainer)
                this.RaiseAfterCheck(this.GetEditorID(currentElement), this.checkingText);
            this.ShowFinishMessageBox();
            ASPx.activeSpellChecker = null;
        },
    
        CheckElementCore: function(element) { 
            if (ASPxEditorHelper.IsEditableEditNode(element))
                this.CheckCore(element.value, element, element, null);
            else
                this.ContinueOrFinishCheck(element);
        },
        CheckByCallbackResult: function(text, result, anchorElement) {     
            this.CheckCore(text, anchorElement, null, result);
        },    
        CheckCore: function(text, anchorElement, editorElement, callResult) { 
            ASPx.activeSpellChecker = this;
            this.checkingText = text;
            this.anchorElement = anchorElement || document.body;
            this.editorElement = editorElement;
            ASPx.currentControlNameInDialog = this.name;
        
            if(this.formHandler == null)
                this.formHandler = new ASPxSpellCheckerFormHandler(this);        
        
            //if(callResult != null)
            //    this.OnCallback(callResult);
            //else if(text != "")
            //    this.StartCheck();
            //else 
            //    this.ContinueOrFinishCheck(this.editorElement);
            if (!this.StartCheck(callResult))
                this.ContinueOrFinishCheck(this.editorElement);
        },
        StartCheck: function(callResult) {
            this.checkComplete = false;
            this.checkedText = this.checkingText;
            if (callResult) {
                this.OnCallback(callResult);
                return true;
            }
            else if (this.checkingText && this.checkingText != "") {
                this.SendCheckCallback(this.checkingText);
                return true;
            }
            else
                return false;
        },
        CheckNext: function(startIndex) {
            this.SendCheckCallback(this.checkedText, startIndex);
        },
        SendCheckCallback: function (text, startIndex) {
            var arguments = this.CreateCallbackArgumentToCheckText(text);
            if (startIndex)
                arguments += ":StartIndex:" + startIndex;
            this.SendCallback(arguments);
        },
        ReCheck: function(text) {
            //var callbackParam = this.CreateCallbackArgumentToCheckText(text);
            this.checkingText = text;
            //this.SendCallback(callbackParam);
            this.StartCheck();
        },

        CreateCallbackArgumentToCheckText: function(text) {
            return ASPxClientSpellChecker.GetCallbackArgumentToCheckText(text);
        },
        CreateCallbackArgumentToGetSpellCheckFormContent: function() {
            return callbackArgumentToGetSpellCheckFormContent;
        },
        CreateCallbackArgumentToGetSpellCheckOptionsFormContent: function() {
            return callbackArgumentToGetSpellCheckOptionsFormContent;
        },
        CreateCallbackArgumentToAddWord: function(word) {
            return "Word(" + word.length + "):" + word;
        },
            	            
        ShowFinishMessageBox: function() {
            var needShowAlert = this.formHandler == null || !this.formHandler.HasErrors();
            if(!needShowAlert)
                return;
            needShowAlert = this.RaiseCheckCompleteFormShowing();
            if(needShowAlert)
                alert(this.finishSpellChecking);    
        },
    
        
        /*region* * * * * * * * * * * * * * * * * *  Dialogs - OptionsForm  * * * * * * * * * * * * * * * * * */
        IsDialogContentAvailable: function(name) {
            return this.dialogContentHashTable[name] != null;
        },
        HideDialog: function(result) { 
            if(this.preventHideEvents) return;
        
            var optionsForm = this.GetOptionsForm();
            if(optionsForm.IsVisible())
                this.OnFinishOptionsEditing(result);
            else
                this.OnFinishSpellChecking(result);
        },
        ShowOptionsForm: function(dialogContent) {
            this.isOptionsForm = true;
            var dialog = this.GetOptionsForm();
            if(!this.IsDialogContentAvailable(dialog.name)  && !dialogContent){
                this.SendCallback(this.CreateCallbackArgumentToGetSpellCheckOptionsFormContent());
            }
            else
                this.ShowOptionsFormCore(dialogContent);
        },
        
        ShowOptionsFormCore: function(dialogContent) { 
            var dialog = this.GetOptionsForm();
            var spellCheckForm = this.GetSpellCheckForm();
        
            var popupElement = this.anchorElement;
            if(spellCheckForm.IsVisible())
                popupElement = spellCheckForm.GetDialogPopup().GetWindowContentElement(-1);
                
            dialog.Show(popupElement);
        
            if(dialogContent)
                dialog.SetDialogContent(dialogContent);
        },
    
        /*region* * * * * * * * * * * * * * * * * *  LoadingPanel  * * * * * * * * * * * * * * * * * */    
    
        ShowLoadingPanel: function() {
            var offsetElement = null;
            var spellCheckForm = this.GetSpellCheckForm();
            if(spellCheckForm.IsVisible())
                spellCheckForm.ShowLoadingPanelOverDialogPopup();
            else {
                offsetElement = this.checkedContainer ? this.checkedContainer : this.anchorElement;
                this.ShowLoadingPanelCore(document.body, offsetElement);            
            }
        
        },
        HideLoadingPanelCore: function() {
            var spellCheckForm = this.GetSpellCheckForm();    
            if(spellCheckForm.IsVisible())
                spellCheckForm.HideLoadingPanelOverDialogPopup();
        },
    
        ShowLoadingPanelCore: function(parentElement, offsetElement) { 
            this.CreateLoadingPanelWithAbsolutePosition(parentElement, offsetElement);        
        },
        
        /*region* * * * * * * * * * * * * * * * * *  Callback  * * * * * * * * * * * * * * * * * */
        SendCallback: function(argument) {
            this.BeforeSendCallback();
            this.CreateCallback(argument);   
        },
        OnCallback: function(result) {
		    this.HideLoadingPanelCore();
		    this.ProcessCallbackResult(result);
        },
        IsNeedSpellCheckFormShow: function() {
            var errors = this.formHandler.errors;
            if(!errors || !errors.length)
                return false;
            for(var i = 0; i < errors.length; i++) {
                var word = errors[i].word;
                if(!this.formHandler.FindWord(this.formHandler.ignoreAllList, word))
                    return true;
            }
            return false;
        },
        ProcessCallbackResult: function (resultObj) {
            // state
            if (ASPx.IsExists(resultObj.settings))
                this.UpdateStateObjectWithObject({ settings: resultObj.settings });
            if (!this.isOptionsForm) {
                var errorCount = resultObj.errorCount || 0;
                this.checkComplete = resultObj.checkComplete || false;
                if (errorCount > 0) {
                    this.formHandler.Initialize(errorCount, resultObj.startErrorWordPositionArray, resultObj.wrongWordLengthArray, resultObj.suggestionsArray);
                    //this.formHandler.BeforeShowSpellCheckerForm();
                    if (this.IsNeedSpellCheckFormShow()) {
                        this.GetSpellCheckForm().Show(this.anchorElement);
                        if (ASPx.IsExists(resultObj.dialogContent))
                            this.GetSpellCheckForm().SetDialogContent(resultObj.dialogContent);
                    }
                }
                else
                    if (!this.checkedContainer)
                        this.OnFinishSpellChecking(false);
            }
            else
                this.ShowOptionsForm(resultObj.dialogContent);
        },

        DoEndCallback: function () {
            if (this.endCallbackAnimationProcessing)
                return;
            this.constructor.prototype.DoEndCallback.call(this);
            if (this.callbackError) {
                this.callbackError = false;
                return;
            }
            if (!this.isOptionsForm) {
                if (this.formHandler.errorCount > 0) {
                    this.GetSpellCheckForm().OnEndCallback();
                    this.formHandler.ShowSpellCheckerForm();
                }
                else if (this.checkedContainer) {
                    var editor = this.FindNextEditorInContainer(this.checkedContainer, this.editorElement);
                    if (editor)
                        this.ContinueCheckContainer(editor);
                    else {
                        this.OnFinishSpellChecking(false);
                        this.ShowFinishMessageBox();
                    }
                }
                else
                    this.ShowFinishMessageBox();
            }
            else
                this.InitializeOptionsDialogFields();
        },
        OnCallbackError: function(result, data){
            this.callbackError = true;
            this.constructor.prototype.OnCallbackError.call(this, result, data);
        },
        BeforeSendCallback: function() {
            this.ShowLoadingElements();
        },
    
        /*region* * * * * * * * * * * * * * * * * *  Action  * * * * * * * * * * * * * * * * * */
    
        AddToDictionary: function() {
            this.formHandler.AddToDictionaryClick();
        },
        AddWord: function(word) {
            this.formHandler.ClearErrorSelection();
            var text = this.checkedText;
            var callbackParam = this.CreateCallbackArgumentToCheckText(text);
            callbackParam = callbackParam.concat(":", this.CreateCallbackArgumentToAddWord(word));
            this.checkingText = text;        
            this.SendCallback(callbackParam);
        },    
        Change: function() {
            this.formHandler.ChangeClick();        
        },
        ChangeAll: function() {
            this.formHandler.ChangeAllClick();
        },
        Ignore: function() {
            this.formHandler.IgnoreClick();
        },    
        IgnoreAll: function() { 
            this.formHandler.IgnoreAllClick();
        },    
        Cancel: function() {
            this.formHandler.Cancel();
        },
        
        /*region* * * * * * * * * * * * * * * * * *  Event Handlers  * * * * * * * * * * * * * * * * * */    
    
        ListBoxItemChanged: function(listBox, e) {
            this.formHandler.ListBoxItemChanged(listBox);
        },    
        TextBoxKeyPress: function(evt) {
            this.formHandler.TextBoxKeyPress(ASPx.Evt.GetKeyCode(evt));
        },    
        TextBoxKeyDown: function(evt) {
            this.formHandler.TextBoxKeyDown(ASPx.Evt.GetKeyCode(evt));
        },
        SCListBoxItemDoubleClick: function() {
            this.formHandler.SCListBoxItemDoubleClick();
        },
          
        OnWordChanged: function() {
            if(this.editorElement)
                this.editorHelper.SetText(this.editorElement, this.checkedText);
            this.RaiseWordChanged(this.GetEditorID(this.editorElement), this.checkedText);
        },
    
        OnFinishSpellCheckingCore: function(change) {
            var checkedText = this.checkingText;
            this.formHandler.ClearErrorSelection();
            if(change) {
                checkedText = this.checkedText;
                if(this.editorElement)
                    this.editorHelper.SetText(this.editorElement, checkedText);                
            }
            this.formHandler.ClearInternalFields();
        
            this.RaiseAfterCheck(this.GetEditorID(this.editorElement), checkedText);
        
            this.checkingText = null;
            this.anchorElement = null;
            this.editorElement = null;
            this.checkedText = null;
        },
        OnFinishSpellChecking: function(change) {
            this.preventHideEvents = true;
            if(change && this.checkedContainer){
                var editor = this.FindNextEditorInContainer(this.checkedContainer, this.editorElement);
                if(editor){
                    this.OnFinishSpellCheckingCore(change);
                    this.ContinueCheckContainer(editor);
                    this.preventHideEvents = false;
                    return;
                }
            }
            var spellCheckForm = this.GetSpellCheckForm();
            if(spellCheckForm.IsVisible() != null)
                spellCheckForm.GetDialogPopup().Hide();
            
            this.OnFinishSpellCheckingCore(change);
            if(change) 
                this.ShowFinishMessageBox();
            ASPx.activeSpellChecker = null;
            this.preventHideEvents = false;
        },    
        OnFinishOptionsEditing: function(change) {
            this.preventHideEvents = true;
            var optionsForm = this.GetOptionsForm();
            if(optionsForm.IsVisible() != null)
                optionsForm.GetDialogPopup().Hide();

            this.isOptionsForm = false;
            if(change) {
                this.stateObject.settings = this.CreateSettingsObject();
                this.ReCheck(this.checkedText);
            }
            else {
                var textBox = this.formHandler && this.formHandler.GetSCFormChangeTextBox();
                if(textBox)
                    textBox.Focus();
            }
            this.preventHideEvents = false;
        },
        FindNextEditorInContainer: function(container, currentEditor) {
            var editor = this.containerBrowser.FindNextEditor(container, currentEditor);
            if(editor && editor.id && editor.id.indexOf(this.name + "_") == 0)
                editor = null;
            return editor;
        },
   
        // uppercase
        // mixedcase
        // numbers
        // emails
        // urls
        CreateSettingsObject: function() {
            var formHandler = this.formHandler;
            var oldSettings = this.stateObject.settings;
            return {
                ignoreUpperCaseWords: formHandler.GetUpperCaseCheckBox() ? formHandler.GetUpperCaseCheckBox().GetChecked() : oldSettings.ignoreUpperCaseWords,
                ignoreMixedCaseWords: formHandler.GetMixedCaseCheckBox() ? formHandler.GetMixedCaseCheckBox().GetChecked() : oldSettings.ignoreMixedCaseWords,
                ignoreWordsWithNumber: formHandler.GetNumbersCheckBox() ? formHandler.GetNumbersCheckBox().GetChecked() : oldSettings.ignoreWordsWithNumber,
                ignoreEmails: formHandler.GetEmailsCheckBox() ? formHandler.GetEmailsCheckBox().GetChecked() : oldSettings.ignoreEmails,
                ignoreUrls: formHandler.GetUrlsCheckBox() ? formHandler.GetUrlsCheckBox().GetChecked() : oldSettings.ignoreUrls,
                ignoreMarkupTags: formHandler.GetTagsCheckBox() ? formHandler.GetTagsCheckBox().GetChecked() : oldSettings.ignoreMarkupTags,
                culture: formHandler.GetLanguageComboBox() ? formHandler.GetLanguageComboBox().GetValue() : oldSettings.culture
            };
        },
        InitializeOptionsDialogFields: function() {
            this.UpdateSettings(this.stateObject.settings);
        },
        UpdateSettings: function(settingsObj) {
            var formHandler = this.formHandler;
            if(formHandler.GetUpperCaseCheckBox())
                formHandler.GetUpperCaseCheckBox().SetChecked(settingsObj.ignoreUpperCaseWords);
            if(formHandler.GetMixedCaseCheckBox())
                formHandler.GetMixedCaseCheckBox().SetChecked(settingsObj.ignoreMixedCaseWords);
            if(formHandler.GetNumbersCheckBox())
                formHandler.GetNumbersCheckBox().SetChecked(settingsObj.ignoreWordsWithNumber);
            if(formHandler.GetEmailsCheckBox())
                formHandler.GetEmailsCheckBox().SetChecked(settingsObj.ignoreEmails);
            if(formHandler.GetUrlsCheckBox())
                formHandler.GetUrlsCheckBox().SetChecked(settingsObj.ignoreUrls);
            if(formHandler.GetTagsCheckBox())
                formHandler.GetTagsCheckBox().SetChecked(settingsObj.ignoreMarkupTags);
            if(formHandler.GetLanguageComboBox())
                formHandler.GetLanguageComboBox().SetValue(settingsObj.culture);    
        }
    }); 
    ASPxClientSpellChecker.Cast = ASPxClientControl.Cast;

    ASPxClientSpellChecker.GetCallbackArgumentToCheckText = function(text) {
        var argument = "Check(" + text.length + "):" + text;
        argument = argument.concat(":", callbackArgumentToGetSpellCheckFormContent);
        return argument;
    }

    var ASPxSpellCheckerFormHandler = ASPx.CreateClass(null, {
        constructor: function(spellChecker) {
            this.spellChecker = spellChecker;
            this.scForm = spellChecker.GetSpellCheckForm();
            this.errorCount = -1;
            this.errors = [];
            this.currentError = null;
            this.ignoreAllList = [];
            this.changeAllList = { };
            this.delta = 0;
        
            this.checkedTextFakeElement = null;
            this.previewTextElement = null;
            this.sampleErrorElement = null;
        },
    
        ClearInternalFields: function () {
            this.delta = 0;
            this.errorCount = -1;
            this.errors = [];
            this.currentError = null;
        },
        Initialize: function(errorCount, startErrorWordPositionArray, wrongWordLengthArray, suggestionsArray) {
            this.ClearInternalFields();

            this.wrongWordLengthArray = wrongWordLengthArray;
            this.startErrorWordPositionArray = startErrorWordPositionArray;
            this.suggestionsArray = suggestionsArray;
            this.errorCount = errorCount;
            this.InitializeErrorArray();
        },
        InitializeErrorArray: function() {
            ASPx.Data.ArrayClear(this.errors);
            for (var i =0; i < this.errorCount; i++) {
                var error = {};
                error.wordStart = this.startErrorWordPositionArray[i];
                error.wordLength = this.wrongWordLengthArray[i];
                error.suggestions = this.suggestionsArray[i];
                error.suggestionCount = this.suggestionsArray[i].length;                
                error.word = this.GetWordByError(error);
            
                this.AddErrorToErrorsList(error);
            }
        },
        AddErrorToErrorsList: function(error) {
            if(this.currentError == null)
                this.errors.push(error);
            else {
                if(error.wordStart > this.currentError.wordStart)
                    this.errors.push(error);
                else
                    this.errorCount --;
            }
        },
        /*region* * * * * * * * * * * * * * * * * *  Get/Set  * * * * * * * * * * * * * * * * * */    
        GetSCForm: function() {
            return this.scForm;
        },
        GetSCFormListBox: function (form) {
            if(typeof(_dxeSCSuggestionsListBox) != "undefined" && _dxeSCSuggestionsListBox && ASPx.IsExistsElement(_dxeSCSuggestionsListBox.GetMainElement()))
                return _dxeSCSuggestionsListBox;
            return null;
        },    
        GetSCFormChangeButton: function(form) { 
            if(typeof(_dxeSCBtnChange) != "undefined" && _dxeSCBtnChange && ASPx.IsExistsElement(_dxeSCBtnChange.GetMainElement()))
                return _dxeSCBtnChange;
            return null;
        },
        GetSCFormChangeAllButton: function(form) {
            if(typeof(_dxeSCBtnChangeAll) != "undefined" && _dxeSCBtnChangeAll && ASPx.IsExistsElement(_dxeSCBtnChangeAll.GetMainElement()))
                return _dxeSCBtnChangeAll;
            return null;
        },
        GetSCFormChangeTextBox: function() { 
            if(typeof(_dxeSCTxtChangeTo) != "undefined" && _dxeSCTxtChangeTo && ASPx.IsExistsElement(_dxeSCTxtChangeTo.GetMainElement()))
                return _dxeSCTxtChangeTo;
            return null;
        },
        GetUpperCaseCheckBox: function() { 
            if(typeof(chkbUpperCase) != "undefined" && chkbUpperCase && ASPx.IsExistsElement(chkbUpperCase.GetMainElement()))
                return chkbUpperCase;
            return null;
        },
        GetMixedCaseCheckBox: function() { 
            if(typeof(chkbMixedCase) != "undefined" && chkbMixedCase && ASPx.IsExistsElement(chkbMixedCase.GetMainElement()))
                return chkbMixedCase;
            return null;
        },
        GetNumbersCheckBox: function() { 
            if(typeof(chkbNumbers) != "undefined" && chkbNumbers && ASPx.IsExistsElement(chkbNumbers.GetMainElement()))
                return chkbNumbers;
            return null;
        },
        GetEmailsCheckBox: function() { 
            if(typeof(chkbEmails) != "undefined" && chkbEmails && ASPx.IsExistsElement(chkbEmails.GetMainElement()))
                return chkbEmails;
            return null;
        },
        GetUrlsCheckBox: function() { 
            if(typeof(chkbUrls) != "undefined" && chkbUrls && ASPx.IsExistsElement(chkbUrls.GetMainElement()))
                return chkbUrls;
            return null;
        },
        GetTagsCheckBox: function() { 
            if(typeof(chkbTags) != "undefined" && chkbTags && ASPx.IsExistsElement(chkbTags.GetMainElement()))
                return chkbTags;
            return null;
        },
        GetLanguageComboBox: function() { 
            if(typeof(comboLanguage) != "undefined" && comboLanguage && ASPx.IsExistsElement(comboLanguage.GetMainElement()))
                return comboLanguage;
            return null;
        },
        GetCheckedTextFakeElement: function() {
            if(!ASPx.IsExistsElement(this.checkedTextFakeElement))
                this.checkedTextFakeElement = document.createElement("DIV");
            return this.checkedTextFakeElement;
        },
        GetSampleErrorElement: function() {
            return document.getElementById(this.spellChecker.name + "_SpellCheckSpan");        
        },
        GetCachedSampleErrorElement: function() {
            if(!ASPx.IsExistsElement(this.sampleErrorElement))
                this.sampleErrorElement = this.GetSampleErrorElement();
            return this.sampleErrorElement;
        },            
        GetPreviewTextElement: function() {
            if(!ASPx.IsExistsElement(this.previewTextElement))
                this.previewTextElement = document.getElementById(this.spellChecker.name + "_" + this.GetSCForm().name +"_SCCheckedDiv"); 
            return this.previewTextElement;
        },
    
        ClearListBoxItems: function() {
            var listBox = this.GetSCFormListBox(this.GetSCForm());
            if(listBox) 
                listBox.ClearItems();
        },
        CanPerformChangeAction: function() {
            return this.currentError.suggestionCount > 0;
        },    
        HasErrors: function() {
            return this.errors.length > 0  || this.currentError != null;
        },
        ProcessCurrentError: function() {
            this.ClearErrorSelection();
            this.AdjustFormButtons();
            this.SelectError(this.currentError);
            this.PrepareFormControlsByError(this.currentError);
        },
        PrepareFormControlsByError: function(error) { 
            this.PopulateSuggestionsListBox(error);
            this.PrepareTextBox(error);
            this.SelectFirstSuggestion();    
        },
        SetChangeButtonsEnabled: function(enabled) {
            var btnChange = this.GetSCFormChangeButton(this.GetSCForm());
            var btnChangeAll = this.GetSCFormChangeAllButton(this.GetSCForm());
            if(btnChange)
                btnChange.SetEnabled(enabled);
            if(btnChangeAll)
                btnChangeAll.SetEnabled(enabled);    
        },
        SelectError: function(error) {
            var startIndex = this.GetWordStartIndex(error);
            var finishIndex = this.GetWordFinishIndex(error);   
            var text = this.spellChecker.checkedText;
        
            text = ASPx.Str.Insert(text, errorMarker, finishIndex);
            text = ASPx.Str.Insert(text, errorMarker, startIndex);
        
            if(this.spellChecker.editorElement != null)
                text = ASPxSpellCheckerFormHandler.PreparePreviewText(text);
            else
                text = SpellCheckerHtmlFilter.PreparePreviewHTML(text);
            
            startIndex = text.indexOf(errorMarker);
            finishIndex = text.lastIndexOf(errorMarker) + errorMarker.length;
            var leftText = text.slice(0, startIndex);
            var rightText = text.slice(finishIndex);
        
            var element = this.GetPreviewTextElement();
            if(!element) return;
            
            var errorSpan = this.GetCachedSampleErrorElement().cloneNode(true);
            ASPx.SetElementDisplay(errorSpan, true);
            errorSpan.innerHTML = error.word;
            element.appendChild(errorSpan);
            element.innerHTML = leftText + element.innerHTML + rightText;
        
            this.UpdatePreviewElementScroll();
        
        },
        UpdatePreviewElementScroll: function() {
            var previewElement = this.GetPreviewTextElement();
            var sampleErrorElement = this.GetSampleErrorElement();
        
            var previewElementClientHeight = ASPx.GetClearClientHeight(previewElement);

            var errorElementOffsetTop = this.getElementOffsetTop(sampleErrorElement, previewElement);

            var sampleErrorElementScrollTop = errorElementOffsetTop + sampleErrorElement.offsetHeight;
            var delta = sampleErrorElementScrollTop - previewElement.scrollTop;
                
            if (delta >= previewElementClientHeight || delta <=0)
                previewElement.scrollTop = errorElementOffsetTop - 10; // 10 - PreviewCellPaddingTop
        },

        getElementOffsetTop: function(element, scrollContainer) {
            var elementParent = element.parentNode;
            var offsetTop = element.offsetTop;
            while(elementParent && elementParent != scrollContainer) {
                if(elementParent.tagName == "TABLE" || elementParent.tagName == "TR")
                    offsetTop += elementParent.offsetTop;
                elementParent = elementParent.parentNode;
            }
            return offsetTop;
        },
    
        PopulateSuggestionsListBox: function(error) {
            var listBox = this.GetSCFormListBox(this.GetSCForm());
            if(!listBox)
                return;
            listBox.BeginUpdate();
            listBox.ClearItems();
            if(error.suggestions.length > 0) {
                for(var i = 0; i < error.suggestions.length; i++)
                    listBox.AddItem(error.suggestions[i]);
            }
            else {
                listBox.AddItem(this.spellChecker.noSuggestionsText);
                // disable the listBox here
            }
            listBox.EndUpdate();
        },
        
        SelectFirstSuggestion: function() {
            var listBox = this.GetSCFormListBox(this.GetSCForm());
            if(listBox && listBox.GetItemCount() > 0)
                listBox.SetSelectedIndex(0);    
        },

        ShowSpellCheckerFormCore: function() {
            var previewTextElement = this.GetPreviewTextElement();
            if(previewTextElement)
                previewTextElement.scrollTop = 0;
            this.StartProcessError();
        },
    
        ShowSpellCheckerForm: function() {
            if(this.errorCount > 0)
                this.ShowSpellCheckerFormCore();
            else
                this.FinishSpellChecking();
        
        },

        /*region* * * * * * * * * * * * * * * * * *  Actions  * * * * * * * * * * * * * * * * * */
        AddToDictionaryClick: function() {
            this.spellChecker.AddWord(this.currentError.word);
        },
        CloseSpellCheckForm: function() {
            this.GetSCForm().OnComplete(0, 0);
        },    
        FindWord: function(wordArray, word) { 
            for(var i = 0; i < wordArray.length; i++)
                if(word == wordArray[i])
                    return true;
            return false;
        },
        FindWordInChangeAllList: function(word) {
            return this.changeAllList[word] != null;
        },
        FinishSpellChecking: function() {
            this.spellChecker.OnFinishSpellChecking(true);
        },   
        //StartProcessError: function (formShown) {
        //    this.currentError = this.GetNextError();
        //    if(!formShown)
        //        this.spellChecker.OnWordChanged();
        //    if(this.currentError)
        //        this.ProcessCurrentError();
        //    else
        //        this.FinishSpellChecking();
        //},
        StartProcessError: function () {
            //if (!formShown)
            //    this.spellChecker.OnWordChanged();
            var currentErrorEndIndex;
            if (this.currentError)
                currentErrorEndIndex = this.GetWordFinishIndex(this.currentError);
            if (!this.ProcessNextError()) {
                if (this.spellChecker.checkComplete)
                    this.FinishSpellChecking();
                else
                    this.spellChecker.CheckNext(currentErrorEndIndex);
            }
        },
        ProcessNextError: function() {
            this.currentError = this.GetNextError();
            if (this.currentError) {
                this.ProcessCurrentError();
                return true;
            }
            else
                return false;
        },
    
        /*region* * * * * * * * * * * * * * * * * *  Events  * * * * * * * * * * * * * * * * * */
    
        //BeforeShowSpellCheckerForm: function() {
        //    this.delta = 0;
        //    this.spellChecker.checkedText = this.spellChecker.checkingText;
        //    this.InitializeErrorArray();
        //},
        ChangeClick: function() {
            var suggestion = this.GetSuggestion();
            this.ChangeCore(suggestion);        
            this.spellChecker.OnWordChanged();
            this.StartProcessError();
        },
        IgnoreClick: function() {
            this.StartProcessError();
        },
        IgnoreAllClick: function() {        
            this.ignoreAllList.push(this.currentError.word);
            this.StartProcessError();
        },    
        ChangeAllClick: function() {
            this.changeAllList[this.currentError.word] = this.GetSuggestion();
            this.ChangeClick();
        },       
        ListBoxItemChanged: function(listBox) { 
            var suggestion = listBox.GetSelectedItem().text;
            var textBox = this.GetSCFormChangeTextBox();
            textBox.SetValue(suggestion);
        },
        SCListBoxItemDoubleClick: function() {
            if(this.currentError.suggestionCount > 0)
                this.ChangeClick();
        },    
        TextBoxKeyPress: function(keyCode) {
            if (keyCode > 0 && keyCode != ASPx.Key.Enter && keyCode != ASPx.Key.Esc)
                this.SetChangeButtonsEnabled(true);
        },    
        TextBoxKeyDown: function(keyCode) {
            if(keyCode == ASPx.Key.Backspace || keyCode == ASPx.Key.Delete)
                this.SetChangeButtonsEnabled(true);    
        },
        
        ChangeCore: function(suggestion) { 
            this.DoChangeWord(suggestion);
            if(suggestion)
                this.delta += suggestion.length - this.currentError.wordLength;
            else
                this.delta -= this.currentError.wordLength;
        },    
        GetWord: function(startIndex, endIndex) {
            var text = this.spellChecker.checkedText;
            return text.substring(startIndex, endIndex);
        },
        GetText: function(tdElement) {
            var text = "";
            var textNodes = [ ];
            ASPx.GetTextNodes(tdElement, textNodes);
            for(var i = 0; i < textNodes.length; i++)
                text += textNodes[i].nodeValue;
            return text;
        },    
        GetWordByError: function(error) {
            var startIndex = this.GetWordStartIndex(error);
            var finishIndex = this.GetWordFinishIndex(error);
            return this.GetWord(startIndex, finishIndex);
        },    
        GetWordStartIndex: function(error) {
            return error.wordStart + this.delta;
        },    
        GetWordFinishIndex: function(error) {
            return error.wordStart + error.wordLength + this.delta;
        },
        
        DoChangeWord: function(suggestion) {
            var startIndex = this.GetWordStartIndex(this.currentError);
            var finishIndex = this.GetWordFinishIndex(this.currentError);
            var text = this.spellChecker.checkedText;
            this.spellChecker.checkedText = ASPx.Str.InsertEx(text, suggestion, startIndex, finishIndex);
        },
    
        GetNextError: function() { 
            while(this.errors.length > 0) { 
                var error = this.errors.shift();
                var word = error.word;
            
                if(this.FindWordInChangeAllList(word)) { 
                    this.currentError = error;
                    this.ClearErrorSelection();
                    this.ChangeCore(this.changeAllList[word]);
                    continue;
                }
                if(!this.FindWord(this.ignoreAllList, word))
                    return error;
            }
            return null;
        },    
        GetSuggestion: function() {
            var changeTextBox = this.GetSCFormChangeTextBox();
            return changeTextBox ? changeTextBox.GetText() : '';
        },     
    
        AdjustFormButtons: function() {
            this.SetChangeButtonsEnabled(this.CanPerformChangeAction());
        },
        ClearErrorSelection: function() {
            var element = this.GetPreviewTextElement();      
            if(element) 
                element.innerHTML = "";
        },
        PrepareTextBox: function(error) {
            var textBox = this.GetSCFormChangeTextBox(); 
            if(!textBox) return;

            if(error.suggestions.length == 0) {
                textBox.SetValue(error.word);
                textBox.Focus();
                textBox.SelectAll();
            }
            else {
                textBox.SetValue(error.suggestions[0]);
                textBox.Focus();
            }
        }    
    });
    ASPxSpellCheckerFormHandler.PreparePreviewText = function(text) {
        var regExp = new RegExp("\\r\\n|\\n|\\n\\v", "g");
        text = ASPx.Str.ApplyReplacement(text, [ [ /</g, '&lt;' ], [ />/g, '&gt;' ] ]);
        text = text.replace(regExp, "<br/>");
        return text;
    }

    // SpellCheckerHtmlFilter
    var textFormattingElementTagNames = [ "B", "I", "U", "S" ,"STRONG", "SMALL", "BIG", "BASEFONT", "TT", "STRIKE"];
    var textFormattingElementTagNameHashTable = ASPx.Data.CreateHashTableFromArray(textFormattingElementTagNames);

    var blockElementTagNames = [ "H1", "H2", "H3", "H4" ,"H5", "H6", "CENTER" ];
    var blockElementTagNamesHashTable = ASPx.Data.CreateHashTableFromArray(blockElementTagNames);

    var forbiddenElementTagNames = [ "OBJECT", "APPLET", "IMG", "MAP" ,"IFRAME", "BODY", "HEAD", "SCRIPT", "LINK" ];
    var forbiddenElementTagNamesHashTable = ASPx.Data.CreateHashTableFromArray(forbiddenElementTagNames);

    var SpellCheckerHtmlFilter = {};
    SpellCheckerHtmlFilter.PreparePreviewHTML = function(html) {
        var fakeElement = document.createElement("DIV");
        ASPx.SetInnerHtml(fakeElement, html);
        SpellCheckerHtmlFilter.FilterElements(fakeElement);
        return fakeElement.innerHTML;
    }
    SpellCheckerHtmlFilter.FilterElements = function(parentNode) {
        for (var i = parentNode.childNodes.length - 1; i >= 0 ; i--)
            SpellCheckerHtmlFilter.FilterElements(parentNode.childNodes[i]);
        SpellCheckerHtmlFilter.CleanElement(parentNode);
    }
    SpellCheckerHtmlFilter.CleanElement = function(element) {
        if (element.nodeType == 1) {
            var tagName = element.tagName;
            if (SpellCheckerHtmlFilter.IsTextFormattingElementTagName(tagName))
                ASPx.RemoveOuterTags(element);
            else if (SpellCheckerHtmlFilter.IsBlockElementTagName(tagName))
                ASPx.ReplaceTagName(element, "P");
            else if (SpellCheckerHtmlFilter.IsForbiddenElementTagName(tagName))
                ASPx.RemoveElement(element);
            else {
                ASPx.Attr.RemoveAllAttributes(element);
                ASPx.Attr.RemoveAllStyles(element);
                if (SpellCheckerHtmlFilter.IsLinkElementTagName(tagName))
                    ASPx.Attr.SetAttribute(element, "href", "javascript:void('0')");
            }
        }
    }
    SpellCheckerHtmlFilter.IsTextFormattingElementTagName = function(tagName) {
        return ASPx.IsExists(textFormattingElementTagNameHashTable[tagName]);
    }
    SpellCheckerHtmlFilter.IsBlockElementTagName = function(tagName) {
        return ASPx.IsExists(blockElementTagNamesHashTable[tagName]);
    }
    SpellCheckerHtmlFilter.IsForbiddenElementTagName = function(tagName) {
        return ASPx.IsExists(forbiddenElementTagNamesHashTable[tagName]);
    }
    SpellCheckerHtmlFilter.IsLinkElementTagName = function(tagName) {
        return tagName == "A";
    }

    var ASPxContainerBrowser = ASPx.CreateClass(null, {
        constructor: function() {
            this.currentEditor = null;
            this.nextEditor = null;
            this.isCurrentEditorFound = false;
        },
    
        BeforeFindNextEditor: function(currentEditor) { 
            this.nextEditor = null;
            this.isCurrentEditorFound = false;        
            this.currentEditor = currentEditor;
        },
    
        FindNextEditor: function(container, currentEditor) { 
            this.BeforeFindNextEditor(currentEditor);
            this.FindNextEditorCore(container);
            return this.nextEditor;
        },
    
        FindNextEditorCore: function(container) { 
            if(this.nextEditor != null)
                return;
            for(var i = 0; i < container.childNodes.length; i++) {
                var curNode = container.childNodes[i];
                if(ASPxEditorHelper.IsEditableEditNode(curNode))
                    this.ProcessEditableEditNode(curNode);
                else
                    this.FindNextEditorCore(curNode);
            }
        },
    
        ProcessEditableEditNode: function(editNode) {
            if(this.nextEditor != null)
                return;
            if(editNode == this.currentEditor) { 
                this.isCurrentEditorFound = true;
                return;
            }
            if(this.isCurrentEditorFound || !this.currentEditor) {
                this.nextEditor = editNode;
                return;
            }
        }
    });

    var ASPxSpellCheckerBaseEditorHelper = ASPx.CreateClass(null, {
        constructor: function() {},

        GetText: function(editor) {
            return editor.value;    
        },
    
        SetText: function(editor, value) {
            editor.value = value;
        },
        IsEditable: function(editor) {
            return !editor.disabled && !editor.readOnly;
        }
    });

    var ASPxSpellCheckerASPxEditorHelper = ASPx.CreateClass(ASPxSpellCheckerBaseEditorHelper, {
        constructor: function() { 
            this.constructor.prototype.constructor.call(this);
        },

        GetText: function(editor) {
            return editor.GetValue();    
        },
        SetText: function(editor, value) {
            // B39869
            if(editor.SetText)
                editor.SetText(value);
            else
                editor.SetValue(value);
        },
        IsEditable: function(editor) {
            return editor.GetEnabled() && 
                ASPxSpellCheckerBaseEditorHelper.prototype.IsEditable(editor.GetInputElement());
        }
    });

    var ASPxEditorHelper = ASPx.CreateClass(null, {
        constructor: function() {
            this.standardEditorHelper = new ASPxSpellCheckerBaseEditorHelper();
            this.DXEditorHelper = new ASPxSpellCheckerASPxEditorHelper();
        },
        GetDXEditorByInput: function(inputElement) {
            var result = null;
            ASPx.GetControlCollection().ForEachControl(function(control) {
                if(ASPx.Ident.IsASPxClientEdit(control) && control.GetInputElement() == inputElement) {
                    result = control;
                    return true;
                }
            });
            return result;
        },
        SetText: function(inputElement, value) {
            if(this.IsDXEditor(inputElement)) {
                this.GetDXEditorHelper().SetText(this.GetDXEditorByInput(inputElement), value);
                this.GetDXEditorByInput(inputElement).RaiseTextChanged();
            }
            else
                this.GetStandardEditorHelper().SetText(inputElement, value);
        },
    
        GetText: function(inputElement) {
            if(this.IsDXEditor(inputElement))
                return this.GetDXEditorHelper().GetText(this.GetDXEditorByInput(inputElement));
            else
                return this.GetStandardEditorHelper().GetText(inputElement);
        },
        IsDXEditor: function(inputElement) {
            return this.GetDXEditorByInput(inputElement) != null;    
        },
    
        IsEditable: function(inputElement) {
            if(this.IsDXEditor(inputElement))
                return this.GetDXEditorHelper().IsEditable(this.GetDXEditorByInput(inputElement));
            else
                return this.GetStandardEditorHelper().IsEditable(inputElement);
        },
        GetStandardEditorHelper: function() {
            return this.standardEditorHelper;
        },
    
        GetDXEditorHelper: function() {
            return this.DXEditorHelper;
        }
    });
    ASPxEditorHelper.IsEditableEditNode = function(element) {
        if(!element.tagName)
            return false;
        var tagName = element.tagName;
        if((tagName == "INPUT" && element.type == "text") || tagName == "TEXTAREA")        
            return ASPxEditorHelper.CanEditNode(element);
        return false;
    }
    ASPxEditorHelper.CanEditNode = function(element) {
        return !element.readOnly && !element.disabled;
    }

    // events
    ASPxClientSpellChecker.prototype.RaiseBeforeCheck = function(controlId) {
        if(!this.BeforeCheck.IsEmpty()) {
            var args = new ASPxClientSpellCheckerBeforeCheckEventArgs(controlId);
            this.BeforeCheck.FireEvent(this, args);
            return !args.cancel;
        } 
        return true;
    }
    var ASPxClientSpellCheckerBeforeCheckEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
	    constructor: function(controlId){
	        this.constructor.prototype.constructor.call(this);
            this.controlId = controlId;
        }
    });

    ASPxClientSpellChecker.prototype.RaiseCheckCompleteFormShowing = function() {
        if(!this.CheckCompleteFormShowing.IsEmpty()) {
            var args = new ASPxClientCancelEventArgs();
            this.CheckCompleteFormShowing.FireEvent(this, args);
            return !args.cancel;
        }
        return true;
    }

    ASPxClientSpellChecker.prototype.RaiseAfterCheck = function(controlId, checkedText) {
        if(!this.AfterCheck.IsEmpty()) {
            var args = new ASPxClientSpellCheckerAfterCheckEventArgs(controlId, checkedText);
            this.AfterCheck.FireEvent(this, args);
        }
    }
    ASPxClientSpellChecker.prototype.RaiseWordChanged = function(controlId, checkedText) {
        if(!this.WordChanged.IsEmpty()) {
            var args = new ASPxClientSpellCheckerAfterCheckEventArgs(controlId, checkedText);
            this.WordChanged.FireEvent(this, args);
        }
    }
    var ASPxClientSpellCheckerAfterCheckEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	    constructor: function(controlId, checkedText){
	        this.constructor.prototype.constructor.call(this, null);	    
            this.controlId = controlId;	    
            this.checkedText = checkedText;
        }
    });

    ASPx.SCIgnore = function(s, e) {
        ASPx.activeSpellChecker.Ignore();
    }
    ASPx.SCIgnoreAll = function(s, e) {
        ASPx.activeSpellChecker.IgnoreAll();
    }
    ASPx.SCAddToDictionary = function(s, e) {
        ASPx.activeSpellChecker.AddToDictionary();
    }
    ASPx.SCChange = function(s, e) {
        ASPx.activeSpellChecker.Change();
    }
    ASPx.SCChangeAll = function(s, e) {
        ASPx.activeSpellChecker.ChangeAll();
    }
    ASPx.SCShowOptionsForm = function(s, e) {
        ASPx.activeSpellChecker.ShowOptionsForm();
    }
    ASPx.SCListBoxItemChanged = function(s, e) {
        ASPx.activeSpellChecker.ListBoxItemChanged(s, e);
    }
    ASPx.SCTextBoxKeyPress = function(s, e) {
        if (ASPx.activeSpellChecker != null)
            ASPx.activeSpellChecker.TextBoxKeyPress(e.htmlEvent);
    }
    ASPx.SCTextBoxKeyDown = function(s, e) { 
        if (ASPx.activeSpellChecker != null)
            ASPx.activeSpellChecker.TextBoxKeyDown(e.htmlEvent);
    }

    ASPx.SCListBoxItemDoubleClick = function(s, e) { 
        if (ASPx.activeSpellChecker != null)
            ASPx.activeSpellChecker.SCListBoxItemDoubleClick();
    }

    ASPx.SpellCheckerHtmlFilter = SpellCheckerHtmlFilter;

    window.ASPxClientSpellChecker = ASPxClientSpellChecker;
    window.ASPxClientSpellCheckerBeforeCheckEventArgs = ASPxClientSpellCheckerBeforeCheckEventArgs;
    window.ASPxClientSpellCheckerAfterCheckEventArgs = ASPxClientSpellCheckerAfterCheckEventArgs;
})();