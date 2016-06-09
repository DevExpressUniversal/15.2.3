(function() {

    var spellCheckerIDSuffix = "_SC";

    ASPx.HtmlEditorClasses.Controls.HtmlEditorSpellChecker = ASPx.CreateClass(ASPxClientSpellChecker,{
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this,name);
            this.htmlEditor=null;
        },
        CreateCallback: function (callbackString) {
            this.htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SpellCheckingCallbackPrefix, callbackString, false, this, null);
        },
        OnCallback: function(result) {
            ASPxClientSpellChecker.prototype.OnCallback.call(this,result.spellcheck?result.spellcheck:result);
        },
        OnEndCallback: function() {
            ASPxClientSpellChecker.prototype.DoEndCallback.apply(this,arguments);
        },
        HideDialog: function(result) {
            ASPxClientSpellChecker.prototype.HideDialog.call(this,result);
            if(ASPx.Browser.WebKitFamily)
                this.htmlEditor.GetSelection().SetFocusToDocumentStart();
            if(!this.preventHideEvents) // B188218
                this.htmlEditor.raiseSpellingChecked();
        },
        ShowFinishMessageBox: function() {
            ASPxClientSpellChecker.prototype.ShowFinishMessageBox.call(this);
            this.htmlEditor.raiseSpellingChecked();
        },
        RaiseEndCallback: function() {
            this.RaiseEndCallbackInternal();
        }
    });

    ASPx.HtmlEditorClasses.Controls.HtmlEditorSpellChecker.Helper = ASPx.CreateClass(null, {
        constructor: function(htmlEditor) {
            this.htmlEditor = htmlEditor;
            this.scInitialized = false;
            this.scStartOptions = null;
            this.scCallbackResult = "";
            this.scCallbackSpellCheckResult = "";
            this.areDictionariesAssigned = true; // for spellchecker
        },
        GetSpellCheckerClientInstance: function(checkExistance) {
            var collection = ASPx.GetControlCollection();
            var instanceName = this.htmlEditor.name + spellCheckerIDSuffix;
            var instance = collection.Get(instanceName);
            if(instance) {
                if(!checkExistance || ASPx.IsExistsElement(instance.GetStateHiddenField()))
                    return instance;
                collection.Remove(instance);
            }
            return null;
        },
        InitializeSpellChecker: function() {
            var spellChecker = this.GetSpellCheckerClientInstance();
            if(spellChecker && !this.scInitialized) {
                var he = this.htmlEditor;
                spellChecker.WordChanged.AddHandler(function(sender, args) {
                    ASPx.HESpellCheckerWordChanged(he.name, sender, args);
                });
                spellChecker.ReCheck = function(text) {
                    he.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SpellCheckingCallbackPrefix, spellChecker.CreateCallbackArgumentToCheckText(""), false);
                    spellChecker.ShowLoadingElements();
                }
                spellChecker.htmlEditor = he;
                this.scInitialized = true;
            }
        },
        CheckSpelling: function() {
            var spellChecker = this.GetSpellCheckerClientInstance(true);
            var htmlEditor = this.htmlEditor;
            if(!this.areDictionariesAssigned)
                alert("No dictionaries are specified for the built-in spell checker.");
            if(!spellChecker)
                htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SpellCheckingLoadControlCallbackPrefix, ASPxClientSpellChecker.GetCallbackArgumentToCheckText(""), true);
            else
                htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SpellCheckingCallbackPrefix, spellChecker.CreateCallbackArgumentToCheckText(""), true);
        },
        OnWordChanged: function(sender, args) {
            this.htmlEditor.ExecuteCommandInternal(ASPxClientCommandConsts.CheckSpellingCore_COMMAND, args.checkedText);
        },
        initSpellCheckerControl: function() {
            var spellChecker = this.GetSpellCheckerClientInstance();
            var he = this.htmlEditor;
            if(spellChecker) {
                this.InitializeSpellChecker();
                spellChecker.CheckByCallbackResult(this.scCallbackResult, this.scCallbackSpellCheckResult, he.GetMainElement());
                spellChecker.HideLoadingPanelCore();
                ASPx.ProcessScriptsAndLinks(spellChecker.name, true);
                he.removeFocus();
            }
        },
        OnHtmlEditorCallback: function(prefix, result, spellCheckResult, allowScripts, spellcheckerloadcontrol) {
            var he = this.htmlEditor;
            he.core.setAllowScripts(Boolean(allowScripts));
            switch (prefix) {
                case ASPx.HtmlEditorClasses.SpellCheckingLoadControlCallbackPrefix:
                    var scDiv = ASPx.CreateHtmlElementFromString("<div id='" + he.name + "_SpellCheckerContainer" + "'></div>");
                    he.GetMainElement().parentNode.appendChild(scDiv);
                    scDiv.innerHTML = spellcheckerloadcontrol;
                    this.UpdatePopupControlBackground(scDiv);
                    ASPx.ProcessScriptsAndLinks(he.name);
                    prefix = ASPx.HtmlEditorClasses.SpellCheckingCallbackPrefix;
                case ASPx.HtmlEditorClasses.SpellCheckingCallbackPrefix:
                    var wrapper = he.core.getActiveWrapper();
                    wrapper.commandManager.clearUndoHistory();
                    wrapper.setHtml(result);
                    this.scCallbackResult = result;
                    this.scCallbackSpellCheckResult = spellCheckResult;
                    var spellChecker = this.GetSpellCheckerClientInstance();
                    if(!spellChecker)
                        he.needInitSpellCheckerOnEndCallback = true;
                    else {
                        window.setTimeout(function() {
                            this.initSpellCheckerControl();
                        }.aspxBind(this), 0);
                    }
                    break; 
                case ASPx.HtmlEditorClasses.SpellCheckerOptionsCallbackPrefix:
                    var spellChecker = this.GetSpellCheckerClientInstance();
                    if(spellChecker) {
                        spellChecker.ProcessCallbackResult(spellCheckResult);
                        spellChecker.HideLoadingPanelCore();
                    }
                    break;
            }
        },
        UpdatePopupControlBackground: function(container) { // B207725
            var winContentElements = ASPx.GetNodes(container, function(el) {
                return el.id.indexOf("CLW-1") > -1;
            });
            for(var i = 0, wce; wce = winContentElements[i]; i++)
                wce.style.backgroundColor = "White";
        }
    });

})();