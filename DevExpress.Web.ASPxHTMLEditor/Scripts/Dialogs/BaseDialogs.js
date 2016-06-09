(function() {
    function getEditorValue(editor) {
        return (editor.GetNumber || editor.GetColor || editor.GetChecked || editor.GetValue).call(editor);
    };
    function setEditorValue(editor, value) {
        if(editor.SetColor)
            value = ASPx.Color.ColorToHexadecimal(value) || value;
        (editor.SetNumber || editor.SetColor || editor.SetChecked || editor.SetValue).call(editor, value);
    };
    var DialogViewModel = ASPx.CreateClass(null, {
        constructor: function(model, dialog) {
            this.Model = model;
            this.Dialog = dialog;
            this.injectReferences(model, null, 0);
        },
        injectReferences: function(model, ownerName, depth) {
            if(depth > 1)
                return;
            var valueChangedHandlers = [];
            for(var p in model) {
                var property = model[p];
                if(model.hasOwnProperty && model.hasOwnProperty(p) && this.processProperty(property, model, p, ownerName, valueChangedHandlers))
                    this.injectReferences(property, p, depth + 1);
                if(!ownerName)
                    this[p] = property;
            }
            if(valueChangedHandlers.length > 0)
                this.assignEventHandler(this.Dialog.getControl(ownerName), valueChangedHandlers);
        },
        assignEventHandler: function(editor, handlerInfoList) {
            editor.ValueChanged.AddHandler(function(s, e) {
                var isPristine = !s.isDirty;
                for(var i = 0; i < handlerInfoList.length; i++) {
                    var info = handlerInfoList[i];
                    this.processDirty(editor, function() { info.handler && info.handler(getEditorValue(s)); }, !info.checkDirty || isPristine);
                }
            }.aspxBind(this));
        },
        processProperty: function(property, model, propKey, ownerName, handlers) {
            if(typeof(property) === "function")
                this.processFunction(model, propKey, ownerName, handlers);
            return typeof(property) === "object";
        },
        processFunction: function(model, funcName, ownerName, handlers) {
            var editor = this.Dialog.getControl(ownerName);
            if(editor) {           
                model.Dialog = this.Dialog;
                model.Model = this.Model;
                model.Editor = editor;
                model[funcName] = this.getPreparedFunc(funcName, editor, handlers, model) || model[funcName];
            }
            return false;
        },
        getPreparedFunc: function(funcName, editor, valueChangedHandler, model) {
            var func = model[funcName].aspxBind(model);
            switch(funcName) {
                case "Get":
                    return function() {
                        return this.tryToRetieveValue(func, getEditorValue(editor));
                    }.aspxBind(this);
                case "Set":
                    return function(value) {
                        setEditorValue(editor, this.tryToRetieveValue(func, value));
                    }.aspxBind(this);
                case "SetOnce":
                    var result = function(value) {
                        this.processDirty(editor, function() { 
                            setEditorValue(editor, this.tryToRetieveValue(func, value)); 
                        }.aspxBind(this));
                    }.aspxBind(this);
                    return this.getFunc(result, valueChangedHandler, null, true, funcName);
                case "OnDirty":
                case "OnChanged":
                    return this.getFunc(func, valueChangedHandler, func, funcName == "OnDirty", funcName);
                default:
                    if(funcName.indexOf("On") == 0) {
                        var event = editor[funcName.substring(2, funcName.length)];
                        if(event) {
                            event.AddHandler(func); 
                            return func;
                        }
                    }
                    return null;
            }
        },
        getFunc: function(func, collection, handler, checkDirty, funcName) {
            if(handler || checkDirty)
                collection.push({ handler: handler, checkDirty: checkDirty, name: funcName  });
            return func;
        },
        processDirty: function(editor, func, ORstatement) {
            if(!editor.isDirty || ORstatement)
                func();
            editor.isDirty = true;
        },
        tryToRetieveValue: function(func, defaultValue) {
            var value = func(defaultValue);
            return ASPx.IsExists(value) ? value : defaultValue;
        }
    });

    var HtmlEditorDialog = ASPx.CreateClass(ASPx.Dialog, {
        Execute: function(ownerControl, cmdValue) {
            this.htmlEditor = ownerControl;
            this.SaveDocumentScrollPosition();
            this.CreateSelectionInfo(cmdValue);
            if(ASPx.Browser.IE)
                this.SaveScrollPosition();
            this.htmlEditor.removeFocus();
            if(ASPx.Browser.Opera || ASPx.Browser.Chrome)
                this.RestoreDocumentScrollPosition();
            this.SetDialogNameInput();
            this.isOnCallbackError = false;
            ASPx.Dialog.prototype.Execute.call(this, ownerControl);
        },
        ExecuteInternal: function(result) {
            ASPx.Dialog.prototype.ExecuteInternal.call(this, result);
            var nonEmptyTextPattern = /\S/;
            var contentContainer = this.GetDialogPopup().GetContentContainer(-1);
            if(contentContainer.lastChild && contentContainer.lastChild.nodeType == 3 && !nonEmptyTextPattern.test(contentContainer.lastChild.nodeValue))
                contentContainer.removeChild(contentContainer.lastChild);
        },
        OnCallbackError: function(result, data) {
            this.isOnCallbackError = true;
            this.ClearDialogNameInput();
            this.HideDialog(null, true);
            ASPx.Dialog.prototype.OnCallbackError.call(this, result);
        },    
        OnClose: function() {
            this.htmlEditor.core.getActiveWrapper().restoreLastSelection(this.selectionInfo.selection);
            this.htmlEditor.barDockManager.updateToolbar();
            
            if (this.htmlEditor.layoutManager.isFullscreenMode)
                this.htmlEditor.layoutManager.hideBodyScroll();
            ASPx.Dialog.prototype.OnClose.call(this);
            this.ClearDialogNameInput();
            if(!this.isOnCallbackError)
                this.SaveEditorsState();
            if(this.canRemoveDialogHtmlAfterClose())
                setTimeout(function() { this.GetDialogPopup().SetContentHtml(""); }.aspxBind(this), 0);
        },
        canRemoveDialogHtmlAfterClose: function() {
            return true;    
        },
  	    OnComplete: function(result, params) {
            this.releaseResources();
  	        this.HideDialogPopup();
  	    
  	        this.htmlEditor.core.getActiveWrapper().restoreLastSelection(this.selectionInfo.selection);
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8)
                this.htmlEditor.core.getActiveWrapper().restoreSelection();
	        this.DoCustomAction(result, params);
            this.ClearEditorValue();
	        if(!result)
	            this.htmlEditor.barDockManager.updateToolbar();
  	    },
        releaseResources: function() {
        },
        setFakeFocus: function () {
            if (!this.fakeFocusElement) {
                this.fakeFocusElement = document.createElement("A");
                this.fakeFocusElement.setAttribute("href", ASPx.AccessibilityEmptyUrl);
            }
            this.GetDialogPopup().GetContentContainer(-1).appendChild(this.fakeFocusElement);
            this.fakeFocusElement.focus();
        },
        getFocusField: function () {
            return null;
        },
        SetFocusInField: function () {
            var focusField = this.getFocusField();
            if (focusField && focusField.SetFocus)
                setTimeout(focusField.SetFocus.aspxBind(focusField), 100);
            else
                setTimeout(this.setFakeFocus.aspxBind(this), 100);
        },
        OnInitComplete: function () {
            ASPx.Dialog.prototype.OnInitComplete.call(this);
            this.GetDialogPopup().UpdatePosition();
            if(ASPx.Browser.IE)
                this.RestoreScrollPositionAndSelection();
            // clear fake input
            var htmlEditor = this.htmlEditor;
            htmlEditor.clearFocusInput(); 
            this.RestoreEditorsState();
            var popupElement = this.GetDialogPopup().GetWindowElement(-1);
	        if(popupElement && popupElement.style.width)
                ASPx.Attr.RemoveAttribute(popupElement.style, "width");
        },
    
        // Editors state
        SaveEditorsState: function() {
        },
        RestoreEditorsState: function() {
        },
    
        ClearEditorValue: function() {
        },
        CreateSelectionInfo: function(cmdValue) {
            var cmdValue = cmdValue || {};
            var wrapper = this.htmlEditor.core.getActiveWrapper();
            if(wrapper.selectionManager) {
                var selection = wrapper.getSelection();
                if(!selection.GetSelectedElement())
                    selection.SetFocusToDocumentStart();
                var curSelection = wrapper.selectionManager.createRestoreSelectionForDialog();
                this.selectionInfo = {
                    selection: curSelection,
                    isControl: curSelection.IsControl(),
                    text: curSelection.GetText(),
                    htmlText: curSelection.GetHtmlText(),
                    selectedElement: cmdValue.selectedElement || curSelection.GetParentElement(),
                    endSelectedElement: curSelection.GetEndContainer()
                };
            }
        },
        // Utils
        ClearDialogNameInput: function() {
            this.htmlEditor.stateObject.currentDialog = "";
        },
        SendCallback: function(callbackArgs) {
            this.ownerControl.sendCallbackViaQueue(callbackArgs, "", false, this);
            this.ShowLoadingPanelOverDialogPopup();
        },
        SetDialogNameInput: function() {
            this.htmlEditor.stateObject.currentDialog = this.name;
        },
        RestoreScrollPositionAndSelection: function() {
  	        this.htmlEditor.core.getActiveWrapper().restoreLastSelection(this.selectionInfo.selection);
  	        if(this.savedScrollLeft > 0)
  	            this.htmlEditor.GetDesignViewDocument().body.scrollLeft = this.savedScrollLeft;
  	        if(this.savedScrollTop > 0)  	        
  	            this.htmlEditor.GetDesignViewDocument().body.scrollTop = this.savedScrollTop;
        },
        RestoreDocumentScrollPosition: function() {
            var scrollElement = ASPx.Browser.IE || ASPx.Browser.Firefox ? document.documentElement : document.body;
            if(this.savedDocumentScrollLeft)
                scrollElement.scrollLeft = this.savedDocumentScrollLeft;
            if(this.savedDocumentScrollTop)
                scrollElement.scrollTop = this.savedDocumentScrollTop;
        },
        SaveScrollPosition: function() {
            this.savedScrollLeft = this.htmlEditor.GetDesignViewDocument().body.scrollLeft;
            this.savedScrollTop = this.htmlEditor.GetDesignViewDocument().body.scrollTop;
        },
        SaveDocumentScrollPosition: function() {
            this.savedDocumentScrollLeft = ASPx.GetDocumentScrollLeft();
            this.savedDocumentScrollTop = ASPx.GetDocumentScrollTop();
        },    
        IsEnabledEditor: function(editor) {
            return ASPx.GetElementVisibility(editor.GetMainElement());
        }
    });

    HtmlEditorDialog.ClientPath = {
        DirectorySeparatorChar: "\\",
        AltDirectorySeparatorChar: "/",
        VolumeSeparatorChar: ":",
        GetBaseUrlPath: function() {
		    return window.location.protocol + "//" +  window.location.host;
        },
        GetFileName: function(path) {
            if(path) {
                var length = path.length;
                var index = length;
                while (--index >= 0) {
                    var ch = path.charAt(index);
                    if(((ch == HtmlEditorDialog.ClientPath.DirectorySeparatorChar) || (ch == HtmlEditorDialog.ClientPath.AltDirectorySeparatorChar)) || 
                            (ch == HtmlEditorDialog.ClientPath.VolumeSeparatorChar))
                        return path.substr(index + 1, (length - index) - 1);
                }
            }
            return path || "";
        },
        GetFileNameWithoutExtension: function(path) {
            path = HtmlEditorDialog.ClientPath.GetFileName(path);
            if(!path)
                return "";
            var length = path.lastIndexOf('.');
            if(length == -1)
                return path;
            return path.substr(0, length);
        }
    };

    var InsertDialogBase = ASPx.CreateClass(HtmlEditorDialog, {
        constructor: function(cmdName) {
            this.constructor.prototype.constructor.call(this, cmdName);
            this.submitButton = null;
            this.cancelButton = null;
            this.editorPrefix = "dxheMediaDialog";
            this.viewModel = null;
            this.ValueChanged = new ASPxClientEvent();
        },
        getViewModel: function() {
            return this.viewModel;    
        },
        getModel: function() {
            return {
                BorderStyle: {
                    SetOnce: function(val) { }
                },
                BorderWidth: {
                    OnDirty: function(val) { this.Model.BorderStyle.SetOnce("solid"); }
                },
                BorderColor: {
                    OnDirty: function(val) { this.Model.BorderStyle.SetOnce("solid"); }
                }
            };
        },
        getStylesMap: function () {
            return {
                "marginTop": "int",
                "marginRight": "int",
                "marginBottom": "int",
                "marginLeft": "int",
                "borderWidth": "int",
                "borderStyle": "string",
                "borderColor": "color"
            };
        },
        CanUpdatePopupPosition: function() {
            return false;    
        },
        OnDialogPanelInitialized: function() {
            if(HtmlEditorDialog.prototype.CanUpdatePopupPosition.call(this))
                this.UpdatePopupPosition();    
        },
        InitializeDialogFields: function(settings) {
            this.viewModel = new DialogViewModel(this.getModel(), this);
            this.prepareEditorsWithNullText();
            this.attachEvents();
            if(settings) {
                this.iterateDialogEditors(settings, function(editor, settings, settingName) {
                    if(ASPx.IsExists(settings[settingName]))
                        setEditorValue(editor, settings[settingName]);
                }, null, true);
            }
        },
        prepareEditorsWithNullText: function() {
            var editors = this.getEditorsWithNullText();
            if(editors && editors.length) {
                var hideNullTextClassName = "dxhe-hideNullText";
                for(var i = 0; i < editors.length; i++) {
                    var editor = editors[i];
                    if(editor.focused)
                        ASPx.RemoveClassNameFromElement(editor.GetMainElement(), hideNullTextClassName);
                    else {
                        editor.GotFocus.AddHandler(function(s,e) { 
                            s.GotFocus.ClearHandlers();
                            ASPx.RemoveClassNameFromElement(s.GetMainElement(), hideNullTextClassName);
                        });
                    }
                }
            }    
        },
        getEditorsWithNullText: function() {
            return [];    
        },
        DoCustomAction: function(result, params) {
            if(result) {
                var propName = this.getResourceUrlPropertyName();
                if(propName && params) {
                    var resourceUrl = params[propName];
                    if(resourceUrl)
                        params[propName] = ASPx.HtmlEditorClasses.GetPreparedUrl(this.htmlEditor, resourceUrl);
                }
                this.htmlEditor.ExecuteCommandInternal(this.getCommandName(), this.createCommandArgument(params));
            }
        },
        getResourceUrlPropertyName: function() {
            return null;    
        },
        createCommandArgument: function(params) {
            return params;    
        },
        SaveEditorsState: function() {
        },
        RestoreEditorsState: function() {
        },
        getFormLayout: function() {
            return this.getControl("FormLayout");
        },
        getControlName: function(suffix) {
            return this.editorPrefix + suffix;    
        },
        getControl: function(suffix) {
            return executeIfExists(this.getControlName(suffix), function(control) { return control; }, null);    
        },
        getCacheKey: function() {
            return "dialog";    
        },
        getCache: function() {
            var cacheKey = "dialogCache";
            var htmlEditorCache = this.htmlEditor[cacheKey] || (this.htmlEditor[cacheKey] = {});
            var dialogKey = this.getCacheKey();
            return htmlEditorCache[dialogKey] || (htmlEditorCache[dialogKey] = {});
        },
        saveValue: function(key, value) {
            this.getCache()[key] = value;
        },
        loadValue: function(key) {
            return this.getCache()[key];
        },
        attachEvents: function() {
        },
        getCommandName: function() {
            return this.name.split("dialog").join("");
        },
        getSelectedElement: function() {
            return this.selectionInfo.selectedElement;
        },
        iterateDialogEditors: function(settings, editorProcessFunc, propertiesMap, skipAggregation) {
            propertiesMap = propertiesMap || this.getDialogPropertiesMap();
            var aggrFuncs = [];
            for(var settingName in propertiesMap) {
                var value = propertiesMap[settingName];
                var valueType = typeof value;

                if(valueType == "string" || (valueType == "function" && skipAggregation))
                    executeIfExists(this.editorPrefix + value, function(editor) { editorProcessFunc(editor, settings, settingName); });
                else if(valueType == "object")
                    this.iterateDialogEditors(settings[settingName] || (settings[settingName] = {}), editorProcessFunc, value);
                else if(valueType == "function")
                    aggrFuncs.push((function(key, func) {
                        return function() { settings[key] = func.aspxBind(settings)(); }
                    })(settingName, value));
            }
            for(var i = 0; i < aggrFuncs.length; i++)
                aggrFuncs[i]();
        },
        getDialogPropertiesMap: function() {
            return { };    
        },
        getObjectSettings: function(settings) {
            settings = settings || {};
            this.iterateDialogEditors(settings, function(editor, settings, settingName) {
                settings[settingName] = getEditorValue(editor);
            });
            return settings;
        },
        isChangeDialog: function() {
            return this.name.indexOf("change") > -1;
        },
        prepareSubmitButton: function(submitButton) {
            this.submitButton = submitButton;
            submitButton.SetEnabled(this.isDialogValid(true));
            submitButton.Click.AddHandler(this.submitDialog.aspxBind(this));
            this.ValueChanged.AddHandler(function(dialog) {
                submitButton.SetEnabled(dialog.isDialogValid(true));        
            });
        },
        submitDialog: function() {
            if(this.isDialogValid())
                ASPx.DialogComplete(1, this.getObjectSettings());    
        },
        isDialogValid: function(skipInnerValidation) {
            return true;    
        },
        prepareCancelButton: function(cancelButton) {
            this.cancelButton = cancelButton;
            cancelButton.Click.AddHandler(function() {
                this.onCancelButtonClick();
                ASPx.DialogComplete(0, null);
            }.aspxBind(this));
        },
        onCancelButtonClick: function() {
        },
        RaiseValueChanged: function() {
            if(!this.ValueChanged.IsEmpty())
                this.ValueChanged.FireEvent(this);
        }
    });

    function executeIfExists(prop, execFunc, defVal) {
        var element = (typeof prop == "string") ? window[prop] : prop;
        if(element && element.IsDOMInitialized())
            return execFunc(element);
        return defVal || false;
    };
    function executeOnCurrentDialog(action) {
        var htmlEdit = ASPx.GetControlCollection().Get(ASPx.currentControlNameInDialog);
        var curDialog = htmlEdit != null ? ASPx.Dialog.GetLastDialog(htmlEdit) : null;
	    if(curDialog != null) 
            return action(curDialog);
    }
    
    ASPx.HtmlEditorClasses.Utils.OnDialogSubmitButtonInit = function(s, e) {
        executeOnCurrentDialog(function(dialog) { dialog.prepareSubmitButton(s); });
    };
    ASPx.HtmlEditorClasses.Utils.OnDialogCloseButtonInit = function(s, e) {
        executeOnCurrentDialog(function(dialog) { dialog.prepareCancelButton(s); });
    };
    ASPx.HtmlEditorClasses.Utils.OnConstrainSizeClick = function(evt) {
        return executeOnCurrentDialog(function(dialog) {
            return dialog.onConstrainSizeClick(evt);
        });
    };
    ASPx.HtmlEditorClasses.Utils.OnDialogPanelInit = function(s, e) {
        setTimeout(function(){ 
            ASPx.RemoveClassNameFromElement(s.GetMainElement(), "dxhe-dialogPreparing");
            executeOnCurrentDialog(function(dialog) { dialog.OnDialogPanelInitialized(); });
        }, 0);
    };

    ASPx.HtmlEditorClasses.Utils.mergeFieldMaps = function mergeFieldMaps(fm1, fm2) {
        var result = {};
        for(var i in fm1)
            result[i] = fm1[i];
        for(var i in fm2)
            result[i] = fm2[i];
        return result;
    };
    ASPx.HtmlEditorClasses.Utils.executeIfExists = executeIfExists;

    ASPx.HtmlEditorClasses.Dialogs.InsertDialogBase = InsertDialogBase;

    ASPx.HtmlEditorClasses.Dialogs.DialogViewModel = DialogViewModel;

    ASPx.HtmlEditorDialog = HtmlEditorDialog;
    ASPx.HtmlEditorDialogList = {};
    ASPx.HtmlEditorDialogSR = {};
})();