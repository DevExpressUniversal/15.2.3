/// <reference path="_references.js"/>

(function() {

    var HtmlEditorIDSuffix = {
        PopupDialogControl: "_DPP",

        DesignViewCell: "_DesignViewCell",
        HtmlViewEdit: "_HtmlViewEdit",
        PreviewCell: "_PreviewCell",

        DesignViewIFrame: "_DesignIFrame",
        PreviewIFrame: "_PreviewIFrame",
        
        ErrorFrame: "_EF",
        ErrorTextCell: "_ETC",
        ButtonImageIdPostfix: "Img",
        ErrorFrameCloseButton: "_EFCB",

        EditAreaCell: "_EdtCell",
        StatusBarCell: "_SBarCell",
        MainCell: "_MainCell",

        ToolbarRow: "_TBRow",

        SizeGrip: "_SG",

        TagInspectorWrapperElement: "_TI",
        SystemPopupControl: "_SPC",
        SourceEditorName: "_SourceEditor"
    };

    /* Callback prefixes */
    var fileManagerCallbackPrefix = "FileManager";
    var ASPxClientHtmlEditorCommandExecutingEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
        constructor: function(commandName, parameter) {
            this.commandName = commandName;
            this.parametr = parameter;
        }
    });
    var ASPxClientHtmlEditorCommandEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(commandName, parameter, isSuccessful) {
            this.constructor.prototype.constructor.call(this);
            this.commandName = commandName;
            this.parameter = parameter;
            this.isSuccessful = isSuccessful;
        }
    });
    var ASPxClientHtmlEditorCustomDialogEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this);
            this.name = name;
        }
    });
    var ASPxClientHtmlEditorCustomDialogCloseEventArgsBase = ASPx.CreateClass(ASPxClientHtmlEditorCustomDialogEventArgs, {
        constructor: function(name, status) {
            this.constructor.prototype.constructor.call(this, name);
            this.status = status;
        }
    });
    var ASPxClientHtmlEditorCustomDialogClosingEventArgs = ASPx.CreateClass(ASPxClientHtmlEditorCustomDialogCloseEventArgsBase, {
        constructor: function(name, status) {
            this.constructor.prototype.constructor.call(this, name, status);
            this.handled = false;
        }
    });
    var ASPxClientHtmlEditorCustomDialogClosedEventArgs = ASPx.CreateClass(ASPxClientHtmlEditorCustomDialogCloseEventArgsBase, {
        constructor: function(name, status, data) {
            this.constructor.prototype.constructor.call(this, name, status);
            this.data = data;
        }
    });
    var ASPxClientHtmlEditorValidationEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(html, isValid, errorText) {
            this.constructor.prototype.constructor.call(this);
            this.html = html;
            this.isValid = isValid;
            this.errorText = errorText;
        }
    });
    var ASPxClientHtmlEditorTabEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this);
            this.name = name;
        }
    });
    var ASPxClientHtmlEditorTabCancelEventArgs = ASPx.CreateClass(ASPxClientHtmlEditorTabEventArgs, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.cancel = false;
        }
    });
    var ASPxClientHtmlEditorBeforePasteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(commandName, html) {
            this.constructor.prototype.constructor.call(this);
            this.commandName = commandName;
            this.html = html;
        }
    });

    var HtmlEditorsCollection = ASPx.CreateClass(null, {
        constructor: function() {
            this.htmlEditors = {};
            this.lastActiveHtmlEditor = {};
        },
        Push: function(htmlEditor) {
            ASPx.GetPostHandler().Update(); // B213682
            this.htmlEditors[htmlEditor.name] = htmlEditor;
        },
        Get: function(name) {
            var instance = this.htmlEditors[name];
            if(instance) {
                if(instance.GetMainElement())
                    return instance;
                delete this.htmlEditors[name]; 
            }
            return null;
        },
        ForEach: function(action) {
            for(var name in this.htmlEditors) {
                var instance = this.Get(name);
                if(instance)
                    action(instance);
            }
        },
        FocusActiveEditorToolbar: function() {
            this.ForEach(function(instance) {
                if(instance.getDesignViewWrapper().isInFocus)
                    instance.FocusToolbar();
            });
        },
        AreEditorsValidInContainer: function(container, validationGroup, checkInvisibleEditors) {
            return this.ProcessingEditorsInContainer(function(editor) {
                       return editor.validationManager.getIsValid();
                   }, container, validationGroup, checkInvisibleEditors);
        },
        ClearEditorsInContainer: function(container, validationGroup, clearInvisibleEditors) {
            this.ProcessingEditorsInContainer(function(editor) {
                editor.SetHtml("");
                editor.SetIsValid(true);
                return true;
            }, container, validationGroup, clearInvisibleEditors);
        },
        ValidateEditorsInContainer: function(container, validationGroup, validateInvisibleEditors) {
            return this.ProcessingEditorsInContainer(function(editor) {
                       return editor.validationManager.onValidation(ASPx.ValidationType.MassValidation);
                   }, container, validationGroup, validateInvisibleEditors);
        },
        ProcessingEditorsInContainer: function(proc, container, validationGroup, validateInvisibleEditors) {
            var isSuccess = true;
            this.ForEach(function(instance) {
                if(container == null || ASPx.GetIsParent(container, instance.GetMainElement())) {
                    if((validationGroup == "" || instance.validationGroup == validationGroup)
                       && (validateInvisibleEditors || instance.IsVisible())) {
                        isSuccess = proc(instance) && isSuccess;
                    }
                }
            });
            return isSuccess;
        },
        IsLastActive: function(htmlEditor) {
            var externalRibbon = htmlEditor.barDockManager.getRibbon(true);
            if(!externalRibbon)
                return true;
            if(this.lastActiveHtmlEditor[externalRibbon.name] === undefined && htmlEditor.GetVisible())
                this.SetActive(htmlEditor);
            return this.lastActiveHtmlEditor[externalRibbon.name] == htmlEditor.name;
        },
        SetActive: function(htmlEditor) {
            var externalRibbon = htmlEditor.barDockManager.getRibbon(true);
            if(externalRibbon)
                this.lastActiveHtmlEditor[externalRibbon.name] = htmlEditor.name;
        }
    });
    HtmlEditorsCollection.Get = function() {
        if(!HtmlEditorsCollection.instance)
            HtmlEditorsCollection.instance = new HtmlEditorsCollection();
        return HtmlEditorsCollection.instance;
    };
    var ASPxClientHtmlEditor = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);

            //set from server
            this.enableAutoCompletion = false;
            this.documentType = ASPx.HtmlEditorClasses.DocumentType.XHTML;
            this.htmlViewAutoCompletionIcons = {};
            this.allowInsertDirectImageUrls = true;
            this.uploadImageFileDirectoryPath = "";
            this.validationGroup = "";
            this.htmlEditingMode = ASPx.HtmlEditorClasses.HtmlEditingMode.Advanced;
            this.allowContextMenu = "true";
            this.resourcePathMode = ASPx.HtmlEditorClasses.ResourcePathMode.NotSet;
            this.CommandExecuting = new ASPxClientEvent();
            this.CustomCommand = new ASPxClientEvent();
            this.CommandExecuted = new ASPxClientEvent();
            this.GotFocus = new ASPxClientEvent();
            this.LostFocus = new ASPxClientEvent();
            this.SelectionChanged = new ASPxClientEvent();
            this.HtmlChanged = new ASPxClientEvent();
            this.CustomDialogOpened = new ASPxClientEvent();
            this.CustomDialogClosing = new ASPxClientEvent();
            this.CustomDialogClosed = new ASPxClientEvent();
            this.Validation = new ASPxClientEvent();
            this.ContextMenuShowing = new ASPxClientEvent();
            this.SpellingChecked = new ASPxClientEvent();
            this.ActiveTabChanged = new ASPxClientEvent();
            this.ActiveTabChanging = new ASPxClientEvent();
            this.BeforePaste = new ASPxClientEvent();
            
            this.isErrorOnCallback = false;
            this.allowEditFullDocument = false;

            this.allowResize = false;
        
            this.percentResizeStarted = false;
            this.pasteContainer = null;

            this.cookieName = "";

            this.sizingConfig.correction = true;
            this.sizingConfig.adjustControl = true;

            this.enableTagInspector = false;
            this.tagInspector = null;

            this.systemPopupControl = null;
        
            // custom toolbars
            this.customComboBoxCollection = {};

            // spellchecker
            this.spellCheckerHelper = new ASPx.HtmlEditorClasses.Controls.HtmlEditorSpellChecker.Helper(this);

            // custom dialogs
            this.customDialogsCaptions = {};

            this.clientStateManager = new ASPx.HtmlEditorClasses.Managers.ClientStateManager(this);
            this.validationManager = new ASPx.HtmlEditorClasses.Managers.ValidationManager(this);
            this.layoutManager = new ASPx.HtmlEditorClasses.Managers.LayoutManager(this);

            this.canChangeContentDocument = true;
            this.canRaiseEvents = true;
            this.lastHtmlBackup = "";
            this.advancedSearchOfLocalization = "{0} of {1}";
        },

        /*region* * * * * * * * * * * * * * *  Initialization  * * * * * * * * * * * * * * * */
        useCallbackQueue: function () {
            return true;
        },
        beginInitialize: function() {
            this.canRaiseEvents = false;
            this.canChangeContentDocument = false;
            if(this.html)
                this.lastHtmlBackup = this.html;
            this.html = "";
        },
        endInitialize: function() {
            this.canChangeContentDocument = true;
            this.SetHtml(this.lastHtmlBackup);
            this.HideLoadingElements();
            this.canRaiseEvents = true;
        },
        InlineInitialize: function() {
            this.beginInitialize();

            ASPxClientControl.prototype.InlineInitialize.call(this);
            this.createCore();
            this.initializeUIManagers();
            this.barDockManager.setRibbonVisible(false);
            this.core.setActiveWrapperByName(this.statusBarManager.getActiveView());
            this.core.initAreas(true);
            var activeWrapper = this.getActiveWrapper();
            this.layoutManager.updateLayout(activeWrapper.getName(), false);
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9 && this.isDesignView(activeWrapper.getName())) // B188634 
                activeWrapper.cleanWrongSizeAttribute();
            var tabs = this.statusBarManager.getTabControl();
            if(this.enabled && tabs && tabs.IsDOMInitialized())
                tabs.SetEnabled(false);
            if(this.validationManager.isStandardValidationEnabled())
                this.SynchronizeForStandardValidation();
            HtmlEditorsCollection.Get().Push(this);
            this.ShowLoadingDivAndPanel();
        },
        Initialize: function() {
            ASPxClientControl.prototype.Initialize.call(this);
            this.core.reInitAreasIfNeeded();
            if(this.enableTagInspector) {
                this.tagInspector = new ASPx.HtmlEditorClasses.Controls.TagInspector(this);
                this.tagInspector.initialize();
            }
            this.barDockManager.initialize();
            this.barDockManager.setRibbonVisible(true);
            this.validationManager.initialize();
            this.core.setSpellCheckAttributeValue(!this.barDockManager.isCheckSpellingButtonExists());

            var isDesignViewAllowed = this.core.isDesignViewAllowed();
            if(!ASPx.Browser.Opera || !isDesignViewAllowed) {
                this.core.initializeManagers();
                this.contextMenuManager.initializeEventManager();
            }
            if(isDesignViewAllowed) {
                if(ASPx.Browser.Firefox)
                    this.attachOutsideClickEventHandler();
                if(ASPx.Browser.WebKitFamily && this.getDesignViewWrapper().getElement().loadUnhandled) //B147435
                    this.OnDesignViewIframeLoad();
            }
            var tabs = this.statusBarManager.getTabControl();
            if(this.enabled && tabs)
                tabs.SetEnabled(this.GetEnabled());
            if(this.core.isDesignViewAllowed() && this.barDockManager.isBarDockExists()) {
                this.barDockManager.setItemsEnabled(false);
                if(this.barDockManager.getBarDockControl().getToolbarByIndex(0))
                    this.getDesignViewWrapper().commandManager.initDefaultCommandValues(this.barDockManager.getBarDockControl());
                else
                    this.needInitDefaultCommandValues = true;
            }
            if(!this.clientEnabled)
                this.SetEnabled(false);
        },
        initializeUIManagers: function() {
            this.statusBarManager = new ASPx.HtmlEditorClasses.Controls.StatusBarManager(this);
            this.barDockManager = new ASPx.HtmlEditorClasses.Controls.BarDockManager(this);
            this.contextMenuManager = new ASPx.HtmlEditorClasses.Controls.ContextMenuManager(this);
            this.pasteOptionsBarManager = new ASPx.HtmlEditorClasses.Controls.PasteOptionsBarManager(this);
            this.layoutManager.initializeEventManager();
            if(this.enabled) {
                this.systemPopupControl = ASPxClientControl.GetControlCollection().Get(this.name + HtmlEditorIDSuffix.SystemPopupControl);
                this.systemPopupControl.initializeControl(this);
            }
        },
        AfterInitialize: function() {
            var initFunc = null;
            if(ASPx.Browser.Opera && !this.iframeLoadCalled) {
                initFunc = this.RaiseInit;
                this.needRaiseInit = true;
                this.RaiseInit = function() { };
            }
            this.constructor.prototype.AfterInitialize.call(this);
            if(initFunc)
                this.RaiseInit = initFunc;
            if(this.needInitDefaultCommandValues && this.core.isDesignViewAllowed() && this.barDockManager.isBarDockExists())
                this.getDesignViewWrapper().commandManager.initDefaultCommandValues(this.barDockManager.getBarDockControl());
            if(ASPx.Browser.IE)
                this.AdjustControl();
            if(this.validationManager.needUpdateErrorFrame)
                this.validationManager.updateErrorFrame();
            var wrapper = this.getDesignViewWrapper() || this.getPreviewWrapper();
            if(wrapper)
                ASPx.Evt.AttachEventToElement(wrapper.getElement(), "load", function(evt) { this.onIFrameLoad(); }.aspxBind(this));
		    if(this.core.isNeedReinitIFrame()) {
                setTimeout(function() {
                    this.onIFrameLoad();
                }.aspxBind(this), 0);
            }
            if(this.validationManager.isStandardValidationEnabled())
                this.validationManager.redirectStandardValidators();
            this.TryCallRaiseInit();
            if(this.isDesignViewAllowed()) {
                if(ASPx.Browser.IE)
                     this.getDesignViewWrapper().setDesignModeAttributeValue(this.GetEnabled());
            }
            if(!this.needInitDefaultCommandValues)
                this.updateToolbarAndMenu();
            window.setTimeout(function() {
                if(this.IsVisible() && !this.isLoaded)
                    this.AdjustControlCore();
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10 && this.core.htmlBackup != this.core.getHtml())
                    this.core.restoreHtml();
                if(this.needInitDefaultCommandValues) {
                    this.updateToolbarAndMenu();
                    this.needInitDefaultCommandValues = undefined;
                }
            }.aspxBind(this), 10);
        },
        getCommonWrapperSettings: function() {
            return {
                allowEditFullDocument: this.allowEditFullDocument,
                enterMode: (ASPx.IsExists(this.enterMode) ? this.enterMode : ASPx.HtmlEditorClasses.EnterMode.Default),
                tagFilter: this.tagFilterSettings,
                attrFilter: this.attributeFilterSettings,
                styleAttrFilter: this.styleAttributeFilterSettings,
                advancedSearchOfLocalization: this.advancedSearchOfLocalization
            };
        },
        getDesignWrapperSettings: function() {
            var iframeSettings = this.getIFrameWrapperSettings(this.designViewAreaStyleCssText);
            iframeSettings.updateDeprecatedElements = (ASPx.IsExists(this.updateDeprecatedElements) ? this.updateDeprecatedElements : true);
            iframeSettings.updateBoldItalic = (ASPx.IsExists(this.updateBoldItalic) ? this.updateBoldItalic : true);
            iframeSettings.pasteMode = (ASPx.IsExists(this.pasteMode) ? this.pasteMode : ASPx.HtmlEditorClasses.PasteMode.SourceFormatting);
            iframeSettings.enablePasteOptions = (ASPx.IsExists(this.enablePasteOptions) && !ASPx.Browser.WebKitTouchUI ? this.enablePasteOptions : false);
            iframeSettings.allowContextMenu = this.allowContextMenu;
            iframeSettings.placeholders = (ASPx.IsExists(this.placeholders) ? this.placeholders : []);
            iframeSettings.shortcuts = this.designViewShortcuts;
            return iframeSettings;
        },
        getHtmlViewCMWrapperSettings: function() {
            var settings = this.getCommonWrapperSettings();
            settings.showCollapseTagButtons = (ASPx.IsExists(this.htmlViewSettingsShowCollapseTagButtons) ? this.htmlViewSettingsShowCollapseTagButtons : false);
            settings.showLineNumbers = (ASPx.IsExists(this.htmlViewSettingsShowLineNumbers) ? this.htmlViewSettingsShowLineNumbers : false);
            settings.highlightActiveLine = (ASPx.IsExists(this.htmlViewSettingsHighlightActiveLine) ? this.htmlViewSettingsHighlightActiveLine : false);
            settings.highlightMatchingTags = (ASPx.IsExists(this.htmlViewSettingsHighlightMatchingTags) ? this.htmlViewSettingsHighlightMatchingTags : false);
            settings.enableTagAutoClosing = (ASPx.IsExists(this.htmlViewSettingsEnableTagAutoClosing) ? this.htmlViewSettingsEnableTagAutoClosing : true);
            settings.shortcuts = this.htmlViewShortcuts;
            settings.enableAutoCompletion = this.enableAutoCompletion;
            settings.documentType = this.documentType;
            settings.htmlViewAutoCompletionIcons = this.htmlViewAutoCompletionIcons;
            return settings;
        },
        getPreviewWrapperSettings: function() {
            var iframeSettings = this.getIFrameWrapperSettings(this.previewViewAreaStyleCssText);
            iframeSettings.isScriptExecutionAllowed = this.allowScriptExecution;
            iframeSettings.shortcuts = this.previewShortcuts;
            return iframeSettings;
        },
        getIFrameWrapperSettings: function (viewAreaStyleCssText) {
            var commonSettings = this.getCommonWrapperSettings();
            commonSettings.rtl = this.rtl;
            commonSettings.allowScripts = this.allowScripts;
            commonSettings.docStyleCssText = this.docStyleCssText;
            commonSettings.cssFileLinkArray = this.stateObject.cssFiles;
            commonSettings.viewAreaStyleCssText = viewAreaStyleCssText;
            commonSettings.allowHTML5MediaElements = (ASPx.IsExists(this.allowHTML5MediaElements) ? this.allowHTML5MediaElements : false);
            commonSettings.allowObjectAndEmbedElements = (ASPx.IsExists(this.allowObjectAndEmbedElements) ? this.allowObjectAndEmbedElements : false);
            commonSettings.allowYouTubeVideoIFrames = (ASPx.IsExists(this.allowYouTubeVideoIFrames) ? this.allowYouTubeVideoIFrames : false);
            return commonSettings;
        },
        getEventHandlers: function() {
            return {
                onShowQuickSearch: function(cmdValue) {
                    this.systemPopupControl.showQuickSearch(cmdValue);
                }.aspxBind(this),
                onShowAdvancedSearch: function(cmdValue) {
                    this.systemPopupControl.showAdvancedSearch(cmdValue);
                }.aspxBind(this),
                onSetFullscreenMode: function(wrapper, enable) {
                    var activeWrapper = this.getActiveWrapper();
                    if(wrapper.getName() == activeWrapper.getName())
                        this.layoutManager.setFullscreenMode(enable);
                }.aspxBind(this),
                onRaiseHtmlChanged: function(saveSelectionAndHtml, preventEvent, hidePasteOptionsBar) {
                    this.core.onHtmlChangedCore(saveSelectionAndHtml);
                    if(this.validationManager.isStandardValidationEnabled())
                        this.SynchronizeForStandardValidation(true);
                    if(!preventEvent)
                        this.RaiseHtmlChanged();
                    var wrapper = this.getActiveWrapper();
                    if(hidePasteOptionsBar && wrapper.settings.enablePasteOptions && wrapper.commandManager.getLastPasteFormattingHtml && wrapper.commandManager.getLastPasteFormattingHtml())
                        wrapper.commandManager.clearPasteOptions(true);
                }.aspxBind(this),
                raiseExecuteCommand: function(commandName, parameter, isSuccessfully) {
                    this.ExecuteCommand(commandName, parameter, isSuccessfully);
                }.aspxBind(this),
                raiseCommandExecuting: function(commandName, parameter) {
                    return this.RaiseCommandExecuting(commandName, parameter);
                }.aspxBind(this),
                onUpdateToolbar: function(isImmediately) {
                    var htmlEditor = HtmlEditorsCollection.Get().Get(this.name);
                    if(isImmediately) {
                        window.setTimeout(function() {
                            htmlEditor.barDockManager.updateToolbarImmediately();
                        }, 0);
                    }
                    else
                        htmlEditor.barDockManager.updateToolbar();
                }.aspxBind(this),
                onRaiseFocus: function(wrapper) {
                    this.setHtmlEditorActive();
                    if(this.isDesignView()) {
                        wrapper.eventManager.setLockUpdate(wrapper.commandManager.typeCommandProcessor.hasStyleCommand() || wrapper.eventManager.isUpdateLocked());
                        this.RaiseFocus();
                    }
                }.aspxBind(this),
                onRaiseLostFocus: function(wrapper) {
                    this.setHtmlEditorActive();
                    if(this.isDesignView())
                        this.RaiseLostFocus();
                }.aspxBind(this)
            };
        },
        getDesignWrapperEventHandlers: function() {
            var eventHandlers = this.getEventHandlers();
            eventHandlers.onRaiseBeforePaste = function(commandName, html) {
                return this.RaiseBeforePaste(commandName, html);
            }.aspxBind(this);
            eventHandlers.onFocusLastToolbar = function() {
                this.FocusLastToolbar();
            }.aspxBind(this);
            eventHandlers.onHideAllPopups = function() {
                this.barDockManager.hideAllPopups();
            }.aspxBind(this);
            eventHandlers.onCheckSpelling = function() {
                this.checkSpelling();
            }.aspxBind(this);
            eventHandlers.onExport = function(format) {
                this.Export(format);
            }.aspxBind(this);
            eventHandlers.onExecuteDialog = function (dialog, cmdValue) {
                dialog.Execute(this, cmdValue);
            }.aspxBind(this);
            eventHandlers.onShowPasteOptionsBar = function() {
                this.pasteOptionsBarManager.updatePasteFormattingItemsState();
                this.pasteOptionsBarManager.showBar();
            }.aspxBind(this);
            eventHandlers.onHidePasteOptionsBar = function() {
                this.pasteOptionsBarManager.hideBar();
            }.aspxBind(this);
            eventHandlers.onRemoveFocus = function() {
                this.removeFocus();
            }.aspxBind(this);
            eventHandlers.onRaiseSelectionChanged = function(wrapper, hidePasteOptionsBar) {
                if(wrapper.eventManager.isUpdateLocked())
                    wrapper.eventManager.setLockUpdate(false);
                else {
                    if(wrapper.isInFocus) {
                        this.barDockManager.updateToolbar();
                        this.contextMenuManager.updateContextMenu();
                    }
                    else
                        this.barDockManager.updateToolbarImmediately();
                    if(hidePasteOptionsBar && wrapper.settings.enablePasteOptions)
                        this.pasteOptionsBarManager.hideBar();
                }
                if(this.tagInspector)
                    this.tagInspector.updateElementTree();
                this.barDockManager.updateRibbonContextTabs();
                this.RaiseSelectionChanged();
            }.aspxBind(this);
            return eventHandlers;
        },
        getHtmlViewCMEventHandlers: function() {
            var eventHandlers = this.getEventHandlers();
            eventHandlers.onShowIntelliSenseWindow = function() {
                this.systemPopupControl.showIntelliSenseWindow();
            }.aspxBind(this);
            return eventHandlers;
        },
        createCore: function() {
            var wrappers = [];
            if(!!this.getDesignViewCell())
                wrappers.push(new ASPx.HtmlEditorClasses.Wrappers.DesignIFrameWrapper(this.name + HtmlEditorIDSuffix.DesignViewIFrame, this.getDesignWrapperSettings(), this.getDesignWrapperEventHandlers()));
            if(!!this.getHtmlViewEditCell())
                wrappers.push(this.createHtmlViewWrapper());
            if(!!this.getPreviewCell())
                wrappers.push(new ASPx.HtmlEditorClasses.Wrappers.PreviewIFrameWrapper(this.name + HtmlEditorIDSuffix.PreviewIFrame, this.getPreviewWrapperSettings(), this.getEventHandlers()));

            this.core = new ASPx.HtmlEditorClasses.HtmlEditorCore(wrappers);
            this.core.setEnabled(this.GetEnabled());
            if(ASPx.IsExists(this.html))
                this.core.saveHtmlToBackup(this.html);
            this.html = undefined;
        },
        createHtmlViewWrapper: function() {
            return this.htmlEditingMode == ASPx.HtmlEditorClasses.HtmlEditingMode.Advanced ? 
                new ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper(this.name + HtmlEditorIDSuffix.SourceEditorName, this.getHtmlViewCMWrapperSettings(), this.getHtmlViewCMEventHandlers()) : 
                new ASPx.HtmlEditorClasses.Wrappers.HtmlViewMemoWrapper(this.name + HtmlEditorIDSuffix.HtmlViewEdit, this.getCommonWrapperSettings(), this.getEventHandlers());
        },
        setHtmlEditorActive: function() {
             if(this.barDockManager.getRibbon(true))
                HtmlEditorsCollection.Get().SetActive(this);
        },
        updateToolbarAndMenu: function() {
            if(this.isDesignView() || this.isHtmlView()) {
                if(this.isDesignView())
                    this.barDockManager.updateToolbarImmediately();
                this.contextMenuManager.updateContextMenu();
            }
        },
        restoreFakeNode: function() {
            var doc = this.GetDesignViewDocument();
            if(ASPx.Browser.NetscapeFamily && doc && doc.body.childNodes.length == 0) {
                var newElement = doc.createElement("BR");
                doc.body.appendChild(newElement);
                setTimeout(function() {
                    newElement.parentElement.removeChild(newElement);
                }.aspxBind(this), 50);
            }
        },
        SetClientStateFieldValue: function(name, value, saveToCookie) {
            this.clientStateManager.setFieldValue(name, value, saveToCookie);
        },
        GetDesignViewDocument: function() {
            if(this.core.isDesignViewAllowed())
                return this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Design).getDocument();
            return null;
        },
        GetDialogPopupControl: function() {
            return ASPx.GetControlCollection().Get(this.name + HtmlEditorIDSuffix.PopupDialogControl);
        },
        GetPreviewDocument: function() {
            if(this.core.isPreviewAllowed())
                return this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Preview).getDocument();
            return null;
        },
        GetContextMenu: function() {
            return this.contextMenuManager.getContextMenuControl();
        },
        /* HTML Elements api*/
        getFakeFocusInputElement: function() {
            return ASPx.GetElementById(this.name + "_FFI");
        },
        getFakeFocusAnchorElement: function() {
            var element = ASPx.GetElementById(this.name + "_FFA");
            if(!element || !element.parentNode) {
                element = document.createElement("A");
                ASPx.Attr.SetAttribute(element, "href", "javascript:void('0')");
                ASPx.InsertElementAfter(element, this.GetMainElement());
            }
            return element;
        },
        getErrorFrameID: function(view) {
            return this.name + this.getViewCellID(view) + HtmlEditorIDSuffix.ErrorFrame;
        },
        getErrorTextCellID: function(view) {
            return this.name + this.getViewCellID(view) + HtmlEditorIDSuffix.ErrorTextCell;
        },
        getErrorFrameCloseButtonCellID: function(view) {
            return this.name + this.getViewCellID(view) + HtmlEditorIDSuffix.ErrorFrameCloseButton;
        },
        getTemplateErrorFrameID: function() {
            return this.name + HtmlEditorIDSuffix.ErrorFrame;
        },
        getErrorFrame: function() {
            return ASPx.GetElementById(this.getErrorFrameID());
        },
        getErrorTextCell: function() {
            return ASPx.GetElementById(this.getErrorTextCellID());
        },
        getTemplateErrorFrame: function() {
            return ASPx.GetElementById(this.getTemplateErrorFrameID());
        },
        getStandardValidationHiddenField: function() {
             var name = this.name + "SVHtml";
            return this.GetHiddenField(name, name, this.getMainCell());
        },
        getViewCellID: function(view) {
            view = view || this.statusBarManager.getActiveView();
            if(view == ASPx.HtmlEditorClasses.View.Design)
                return HtmlEditorIDSuffix.DesignViewCell;
            if(view == ASPx.HtmlEditorClasses.View.Html)
                return HtmlEditorIDSuffix.HtmlViewEdit;
            if(view == ASPx.HtmlEditorClasses.View.Preview)
                return HtmlEditorIDSuffix.PreviewCell;
            return "";
        },
        getStatusBarCell: function() {
            return ASPx.GetElementById(this.name + HtmlEditorIDSuffix.StatusBarCell);
        },
        getDesignViewCell: function() {
            return ASPx.GetElementById(this.name + HtmlEditorIDSuffix.DesignViewCell);
        },
        getDesignViewTable: function() {
            return ASPx.GetParentByTagName(this.getDesignViewCell(), "table");
        },
        getPreviewCell: function() {
            return ASPx.GetElementById(this.name + HtmlEditorIDSuffix.PreviewCell);
        },
        getPreviewTable: function() {
            return ASPx.GetParentByTagName(this.getPreviewCell(), "table");
        },
        getHtmlViewTable: function() {
            return ASPx.GetParentByTagName(this.getHtmlViewEditCell(), "table");
        },
        getHtmlViewEditCell: function() {
            var htmlViewEditTable = ASPx.GetElementById(this.name + HtmlEditorIDSuffix.HtmlViewEdit);
            return ASPx.GetNodeByTagName(htmlViewEditTable, "TD", 0);
        },
        getEditAreaCell: function() {
            return ASPx.GetElementById(this.name + HtmlEditorIDSuffix.EditAreaCell);
        },
        getMainCell: function() {
            return ASPx.GetElementById(this.name + HtmlEditorIDSuffix.MainCell);
        },
        getSizeGrip: function() {
            return ASPx.GetElementById(this.name + HtmlEditorIDSuffix.SizeGrip);
        },
        getToolbarRow: function() {
            return this.GetChildElement(HtmlEditorIDSuffix.ToolbarRow);
        },
        getToolbarTable: function() {
            return ASPx.GetParentByTagName(this.getToolbarRow(), "table");
        },
        getToolbarCell: function() {
            return ASPx.GetNodeByTagName(this.getToolbarRow(), "td", 0);
        },
        getTagInspectorWrapperElement: function() {
            return ASPx.GetElementById(this.name +  HtmlEditorIDSuffix.TagInspectorWrapperElement);        
        },

        AdjustControlCore: function() {
		    if(!ASPx.IsExistsElement(this.GetMainElement())) // B139769
                return;
            if(this.layoutManager.resizeTempVars && this.layoutManager.resizeTempVars.isInMove || this.isInitialized && !this.IsDisplayed() || this.layoutManager.adjustSizeInFullscreen()) 
                return;
            var activeWrapper = this.getActiveWrapper();
            var isDesignView = this.core.isDesignView(activeWrapper.getName());
            if(this.IsVisible() && (!this.isInitialized || !this.isLoaded)) {
                this.isLoaded = true;
                this.layoutManager.updateLayout(activeWrapper.getName(), true);
                if(isDesignView && this.GetEnabled())
                    activeWrapper.setDesignModeAttributeValue(this.GetEnabled());
                this.layoutManager.percentResizeStarted = false;
                if(this.delayedFocus) { // B188242
                    this.Focus();
                    this.delayedFocus = null;
                }
            }
            this.layoutManager.setPostponedSize();
            if(this.layoutManager.isWidthDefinedInPercent) {
                var needSaveAndRestoreSelection = (ASPx.Browser.IE && ASPx.Browser.MajorVersion == 10 || ASPx.Browser.NetscapeFamily) && activeWrapper.isInFocus && !this.layoutManager.percentResizeStarted;
                if(needSaveAndRestoreSelection && isDesignView) {
                    activeWrapper.saveSelection();
                    this.removeFocus();
                }
                this.layoutManager.adjustSizeInPercent();
                this.layoutManager.saveCurrentSize(true, true, false);
                if(needSaveAndRestoreSelection && !this.layoutManager.percentResizeStarted && isDesignView)
                    activeWrapper.restoreSelection();
            }
            if(isDesignView && ASPx.Browser.NetscapeFamily && this.GetEnabled())
                activeWrapper.resetContentEditable();
        },
        NeedCollapseControlCore: function() {
            return true;
        },
        ShowLoadingPanel: function() {
            this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement().parentNode, this.GetLoadingPanelOffsetElement(this.GetMainElement()));
        },
        ShowLoadingDiv: function () {
            this.CreateLoadingDiv(this.GetMainElement().parentNode, this.GetMainElement());
        },
        GetCallbackAnimationElement: function() {
            var result;
            this.core.forEachWrapper(function(w){
                if(w.isActive())
                    result = w.getElement();
            });
            return result;
        },
        UpdateAdjustmentFlags: function() {
		    var mainElement = this.GetMainElement();
	        if(mainElement) {
	            var mainElementStyle = ASPx.GetCurrentStyle(mainElement);
                var widths = [];
                if(this.layoutManager.isWidthDefinedInPercent)
                    widths.push(this.layoutManager.initialMainElementWidth);
                this.UpdatePercentSizeConfig(widths, [mainElementStyle.height, mainElement.style.height]);
	        }
	    },

        /*region* * * * * * * * * * * * * * *  Enable  * * * * * * * * * * * * * * * */
        GetEnabled: function(){
            return this.clientEnabled && this.enabled;
	    },
        SetEnabled: function(enabled) {
            ASPxClientControl.prototype.SetEnabled.call(this, enabled);
            this.setEnabledInternal(enabled);
        },
        setEnabledInternal: function(enabled) {
            if(!this.enabled)
                return;
            var wrapper = this.getDesignViewWrapper();
            if(wrapper) 
                wrapper.setDesignModeAttributeValue(enabled);
            this.core.setEnabled(enabled);
            if(enabled)
                this.barDockManager.updateToolbarImmediately();
            else if(this.barDockManager.getRibbon(true) && HtmlEditorsCollection.Get().IsLastActive(this))
                this.barDockManager.setItemsEnabled(enabled);
        },
        /*region* * * * * * * * * * * * * * *  Focus  * * * * * * * * * * * * * * * */
        Focus: function() {
            var wrapper = this.getActiveWrapper();
            if((this.isDesignView(wrapper.getName()) || this.isPreview(wrapper.getName())) && !this.IsIFrameReady())
                this.delayedFocus = true;
            else {
                wrapper.focus();
                if(this.isDesignView(wrapper.getName()) && !ASPx.Browser.MSTouchUI) {
                    setTimeout(function() {
                        this.updateToolbarAndMenu();
                    }.aspxBind(this), 0);
                }
            }
        },
        clearFocusInput: function() {
            var focusInputElement = this.getFakeFocusInputElement();
            if(focusInputElement)
                focusInputElement.value = "";
        },
        removeFocus: function(byAnchor) {
            var wrapper = this.getActiveWrapper();
            if(!this.isDesignView(wrapper.getName()))
                return;
            if(wrapper)
                wrapper.isInFocus = false; // sometimes onlostfocus raises too late
            var scrollTop = ASPx.GetDocumentScrollTop();
            if(byAnchor) {
                try {
                    this.getFakeFocusAnchorElement().focus();
                } catch(e) {}
                ASPx.SetDocumentScrollTop(scrollTop);
            }
            else {
                var inputElement = this.getFakeFocusInputElement();
                ASPx.Attr.RemoveAttribute(inputElement, "disabled");
                try {
                    inputElement.focus();
                }
                catch (e) { }

                if(ASPx.Browser.IE || (ASPx.Browser.WebKitFamily && !this.isInitialized)) {
                    try {
                        window.focus();
                    }
                    catch (e) { }
                }
                inputElement.disabled = "disabled";
            }
        },
        IsIFrameReady: function() {
            return this.isInitialized && this.isLoaded;
        },
        onIFrameLoad: function() {
            if(this.getCommandManager() && !(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) && !this.iframeLoading) { // loaded only after initalize managers
                this.iframeLoading = true;
                if(this.core.isNeedReinitIFrame()) {
                    this.core.initAreas();
                    this.core.initializeWrapperEventManagers();
                    this.contextMenuManager.initializeEventManager();
                }
                window.setTimeout(function() {
                    this.AdjustControl();
                    if(ASPx.Browser.Opera)
                        this.iframeLoading = false;
                    this.restoreFakeNode();  // Q478319
                }.aspxBind(this), 10);
                if(!ASPx.Browser.Opera)
                    this.iframeLoading = false;
            }
        },

        /*region* * * * * * * * * * * * * * *  Content HTML works  * * * * * * * * * * * * * * * */
        GetHtml: function() {
            return this.canChangeContentDocument ? this.core.getHtml() : this.lastHtmlBackup;
        },
        SetHtml: function(html, clearUndoHistory) {
            if(!this.canChangeContentDocument)
                return;
            html = html || "";
            if(this.isDesignView() && !this.getCommandManager()) {
                this.htmlToDelayedSet = {
                    "html": html,
                    "clearUndoHistory": clearUndoHistory
                };
            }
            else {
                clearUndoHistory = ASPx.IsExists(clearUndoHistory) ? clearUndoHistory : true;
                this.core.setHtml(html);
                var designViewWrapper = this.getDesignViewWrapper();
                if(clearUndoHistory && designViewWrapper) {
                    designViewWrapper.commandManager.clearUndoHistory();
                    designViewWrapper.commandManager.clearLastExecutedCommand();
                }
                var eventManager = this.getDesignEventManager() || this.getHtmlViewEventManager();
                if(eventManager && eventManager.onHtmlChanged)
                    eventManager.onHtmlChanged(false);
                if(!clearUndoHistory && designViewWrapper)
                    this.SaveToUndoHistory();
            }
        },
        ReplacePlaceholders: function(html, placeholders) {
            for(var i = 0, placeholder; placeholder = placeholders[i]; i++)
                html = html.replace(new RegExp("{((&nbsp;)*|\\s*)" + placeholder.value + "((&nbsp;)*|\\s*)}", "g"), (placeholder.value && placeholder.replacement != undefined ? placeholder.replacement : ""));
            return html;
        },

        UpdateStateObject: function() {
            if(!this.IsControlExist()) return; // TODO mode to base code

            var html = this.GetHtml();
            this.UpdateStateObjectWithObject({
                html: ASPx.Str.EncodeHtml(html)
            });
            this.syncronizedHtml = html; // HACK for SharePoint: B34387 fix
            this.clearFocusInput();
        },

        SynchronizeForStandardValidation: function(checkValidators) {
            var contentToValidate = this.getStandardValidationHiddenField();
            if(!checkValidators || contentToValidate.Validators)
                contentToValidate.value = ASPx.Str.EncodeHtml(this.GetHtml());
        },
        IsControlExist: function() {
            return ASPx.IsExistsElement(this.GetMainElement());
        },

        ExecuteCommandInternal: function(commandName, parameter, addToUndoHistory) {
            var cancel = this.RaiseCommandExecuting(commandName, parameter);
            return !cancel && this.ExecuteCommand(commandName, parameter, addToUndoHistory);    
        },
        ExecuteCommand: function(commandName, parameter, addToUndoHistory) {
            var commandManager = this.getCommandManager();
            if(!commandManager)
                return;
            addToUndoHistory = ASPx.IsExists(addToUndoHistory) ? addToUndoHistory : true;
            var isSuccessfully = false;

            if(commandName.indexOf(";") > -1) {
                var commandParts = commandName.split(";");
                commandName = commandParts[0];
                parameter = commandParts[1];
            }

            var cmd = commandManager.getCommand(commandName);
            var nonClientCommand = cmd && !cmd.IsClientCommand();
            if(nonClientCommand) {
                isSuccessfully = cmd.Execute(parameter, commandManager.wrapper);
                this.RaiseCommandExecuted(commandName, parameter, isSuccessfully);
            }
            else if(!this.GetEnabled() || !this.IsVisible())
                isSuccessfully = false;
            else if(commandName == ASPxClientCommandConsts.FULLSCREEN_COMMAND) {
                for(var i = 0, wrapper; wrapper = this.core.wrappersArray[i]; i++)
                    wrapper.commandManager.executeCommand(commandName, parameter, false);
                isSuccessfully = true;
                this.RaiseCommandExecuted(commandName, parameter, isSuccessfully);
            }
            else if(!this.isPreview())
                isSuccessfully = this.isHtmlView() ? this.executeCommandToHtmlView(commandManager, cmd, commandName, parameter, addToUndoHistory) : this.executeCommandToDesignView(commandManager, cmd, commandName, parameter, addToUndoHistory);
            else
                isSuccessfully = !this.CustomCommand.IsEmpty() ? this.RaiseCustomCommand(commandName, parameter) : !!this.showCommandNotFoundMessage();
            return isSuccessfully;
        },
        executeCommandToDesignView:function(commandManager, cmd, commandName, parameter, addToUndoHistory){
            var isSuccessfully = false;
            var wrapper = this.getDesignViewWrapper();
            var selectionManager = wrapper.selectionManager;
            selectionManager.restoreSelectionForPopup();
            wrapper.eventManager.callSuspendedHtmlChangedEvt();
            var contentHtml = wrapper.getHtml();
            if(cmd != null) {
                if(!wrapper.isInFocus)
                    selectionManager.restoreSelection();
                isSuccessfully = commandManager.executeCommand(commandName, parameter, addToUndoHistory);
                if(commandName == ASPxClientCommandConsts.BACKCOLOR_COMMAND || commandName == ASPxClientCommandConsts.FONTCOLOR_COMMAND)
                    this.barDockManager.saveRibbonClientState();
                if(!cmd.IsImmediateExecution())
                    commandManager.clearLastExecutedCommand();
            }
            else {
                if(this.isHtmlViewAllowed() && this.getHtmlViewWrapper().commandManager.getCommand(commandName))
                    isSuccessfully = false;
                else if(!this.CustomCommand.IsEmpty()) {
                    if(!ASPx.Browser.IE || (ASPx.Browser.MajorVersion >= 9 || !selectionManager.selection || !selectionManager.selection.GetIsControlSelected())) // B202777
                        selectionManager.restoreSelection();
                    isSuccessfully = this.RaiseCustomCommand(commandName, parameter);
                }
                else
                    return !!this.showCommandNotFoundMessage();
            }
            if(contentHtml != wrapper.getHtml()) {
                ASPx.HtmlEditorClasses.HtmlProcessor.filteringByDomElement(wrapper.getBody(), wrapper);
                if(contentHtml == wrapper.getHtml()) {
                    commandManager.updateLastRestoreSelectionAndHTML();
                    this.barDockManager.updateToolbarImmediately();
                }
                else {
                    var hidePasteOptionsBar = (commandName != ASPxClientCommandConsts.KBPASTE_COMMAND) && (commandName != ASPxClientCommandConsts.PASTEHTMLSOURCEFORMATTING_COMMAND) && (commandName != ASPxClientCommandConsts.PASTEHTMLPLAINTEXT_COMMAND) && 
                        (commandName != ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND); 
                    wrapper.eventManager.onHtmlChanged(true, false, hidePasteOptionsBar);
                }
            }
            else if(isSuccessfully && (!cmd || cmd.CanChangeSelection()))
                wrapper.eventManager.onSelectionChanged(commandName != ASPxClientCommandConsts.KBPASTE_COMMAND);
            this.RaiseCommandExecuted(commandName, parameter, isSuccessfully);
            if(!isSuccessfully)
                this.barDockManager.updateToolbarImmediately();
            return isSuccessfully;
        },
        executeCommandToHtmlView: function(commandManager, cmd, commandName, parameter, addToUndoHistory){
            var isSuccessfully = false;
            var wrapper = this.getHtmlViewWrapper();
            if(!wrapper.isInFocus)
                wrapper.focus();
            if(this.systemPopupControl && wrapper.isIntelliSenseWindowShown)
                this.systemPopupControl.hideIntelliSenseWindow();
            var contentHtml = wrapper.getHtml();
            if(cmd != null) 
                isSuccessfully = commandManager.executeCommand(commandName, parameter, addToUndoHistory);
            else {
                if(this.isDesignViewAllowed() && this.getDesignViewWrapper().commandManager.getCommand(commandName))
                    isSuccessfully = false;
                else if(!this.CustomCommand.IsEmpty())
                    isSuccessfully = this.RaiseCustomCommand(commandName, parameter);
                else 
                    return !!this.showCommandNotFoundMessage();
            }
            if(contentHtml != wrapper.getHtml()) 
                wrapper.eventManager.onHtmlChanged(true, false, false);
            else
                this.barDockManager.updateToolbarImmediately();
            this.RaiseCommandExecuted(commandName, parameter, isSuccessfully);
            return isSuccessfully;
        },
        showCommandNotFoundMessage: function() {
            var barDockControl = this.barDockManager.getBarDockControl();
            if(barDockControl && barDockControl.innerToolbarsCount)
                alert('Command not found');
        },

        isDesignView: function(view) {
            return this.core.isDesignView(view);
        },
        isHtmlView: function(view) {
            return this.core.isHtmlView(view);
        },
        isPreview: function(view) {
            return this.core.isPreview(view);
        },
        isDesignViewAllowed: function() {
            return this.core.isDesignViewAllowed();
        },
        isHtmlViewAllowed: function() {
            return this.core.isHtmlViewAllowed();
        },
        isPreviewAllowed: function() {
            return this.core.isPreviewAllowed();
        },
        getActiveWrapper: function() {
            return this.core.getActiveWrapper();
        },
        getCommandManager: function() {
            return this.getActiveWrapper().commandManager;
        },
        getDesignViewWrapper: function() {
            return this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Design);
        },
        getHtmlViewWrapper: function() {
            return this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Html);
        },
        getPreviewWrapper: function() {
            return this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Preview);
        },
        getDesignEventManager: function() {
            return this.core.isDesignViewAllowed() ? this.getDesignViewWrapper().eventManager : null;
        },
        getHtmlViewEventManager: function() {
            return this.core.isHtmlViewAllowed() ? this.getHtmlViewWrapper().eventManager : null;
        },
        isSimpleHtmlEditingMode: function() {
            return this.htmlEditingMode == ASPx.HtmlEditorClasses.HtmlEditingMode.Simple;
        },
        SaveToUndoHistory: function() {
            this.ExecuteCommandInternal(ASPxClientCommandConsts.SAVESTATEUNDOREDOSTACK_COMMAND, null);
        },
        checkSpelling: function() {
            this.spellCheckerHelper.CheckSpelling();
        },

        /*region* * * * * * * * * * * * * * * * * *  Selection  * * * * * * * * * * * * * * * * * */
        GetSelection: function() {
            return this.GetEnabled() && this.core.isDesignView() ? this.getActiveWrapper().getSelection() : null;
        },
        RestoreSelection: function() {
            var wrapper = this.getActiveWrapper();
            if(this.GetEnabled() && wrapper.restoreSelection)
                wrapper.restoreSelection();
        },
        SetToolbarComboBoxValue: function(commandName, value) {
            if(this.customComboBoxCollection[commandName])
                this.customComboBoxCollection[commandName].SetValue(value);
        },
        SetRibbonContextTabCategoryVisible: function(categoryName, active) {
            this.barDockManager.setContextTabCategoryVisible(categoryName, active);
        },

        /*region* * * * * * * * * * * * * * * * * *  Callback  * * * * * * * * * * * * * * * * * */
        DoEndCallback: function() {
            this.constructor.prototype.DoEndCallback.call(this);
            if(this.needInitSpellCheckerOnEndCallback) {
                this.spellCheckerHelper.initSpellCheckerControl();
                this.needInitSpellCheckerOnEndCallback = false;
            }
            this.isErrorOnCallback = false;
        },
        OnCallback: function (result) {
            this.core.setAllowScripts(Boolean(result.allowScripts));
            switch (result.action) {
                case ASPx.HtmlEditorClasses.SwitchToDesignViewCallbackPrefix:
                    this.statusBarManager.switchToDesignViewCore(result.html);
                    break;
                case ASPx.HtmlEditorClasses.SwitchToHtmlViewCallbackPrefix:
                    this.statusBarManager.switchToHtmlViewCore(result.html);
                    break;
                case ASPx.HtmlEditorClasses.SwitchToPreviewCallbackPrefix:
                    this.statusBarManager.switchToPreviewCore(result.html);
                    break;
                case ASPx.HtmlEditorClasses.SpellCheckingLoadControlCallbackPrefix:
                case ASPx.HtmlEditorClasses.SpellCheckingCallbackPrefix:
                case ASPx.HtmlEditorClasses.SpellCheckerOptionsCallbackPrefix:
                    this.spellCheckerHelper.OnHtmlEditorCallback(result.action, result.html, result.spellcheck, result.allowScripts, result.spellcheckerloadcontrol);
                    break;
                default:
                    throw "Unexpected callback prefix.";
                    break;
            }
        },

        /*region* * * * * * * * * * * * * * * * * *  Low Level Event Handlers  * * * * * * * * * * * * * * * * * */
        FocusToolbar: function() {
            this.barDockManager.getBarDockControl().Focus();
        },
        FocusLastToolbar: function() {
            this.barDockManager.getBarDockControl().FocusLastToolbar();
        },

        onToolbarCustomComboBoxInit: function(combobox) {
            this.customComboBoxCollection[combobox.commandName] = combobox;
        },
        onToolbarColorPickerCustomColorTableUpdated: function(colorPicker, name) {
            this.SetClientStateFieldValue(name.toLowerCase() == "forecolor" ? "ForeColorPalette" : "BackColorPalette", colorPicker.GetState(), true);
        },
        onToolbarDropDownItemCloseUp: function() {
            var wrapper = this.getActiveWrapper();
            if(this.core.isDesignView(wrapper.getName()))
                wrapper.selectionManager.restoreSelectionForPopup();
        },

        attachOutsideClickEventHandler: function() {
            ASPx.Evt.AttachEventToDocument("click", function(evt) {
                var wrapper = this.getDesignViewWrapper();    
                if(this.GetMainElement() != null && wrapper) { // B189164
                        var clickOutside = !ASPx.GetParentByPartialId(ASPx.Evt.GetEventSource(evt), this.GetMainElement().id);
                        setTimeout(function() {
                            if(clickOutside && wrapper.isInFocus && !this.IsVisible())
                                wrapper.eventManager.onLostFocus();
                        }.aspxBind(this), 0);
                    }
                }.aspxBind(this)
            );
        },

        OnErrorFrameCloseButtonClick: function() {
            this.layoutManager.saveCurrentSize(false, true);
            this.validationManager.setIsValid(true, true);
            this.Focus();
            this.layoutManager.restoreHeight();
        },
        OnDesignViewIframeLoad: function() {
            var wrapper = this.getDesignViewWrapper();
            if(!wrapper) return;
            var designViewIframe = wrapper.getElement();
            if(designViewIframe && !designViewIframe.isLoaded) {
                designViewIframe.isLoaded = true;
                this.core.initAreas();
                if(!this.IsHidden())
                    this.layoutManager.updateLayout();

                this.core.initializeManagers();
                this.contextMenuManager.initializeEventManager();
                if(!this.IsHidden())
                    this.AdjustControlCore();
                this.iframeLoadCalled = true;
                this.TryCallRaiseInit();
                if(ASPx.Browser.Opera)
                    this.barDockManager.updateToolbar();
            }
        },
        TryCallRaiseInit: function() {
            if(this.isInitialized && this.iframeLoadCalled && this.needRaiseInit) {
                this.needRaiseInit = false;
                this.RaiseInit();
            }
        },
        RaiseInit: function() {
            this.endInitialize();
            ASPxClientControl.prototype.RaiseInit.call(this);
        },

        /*region* * * * * * * * * * * * * * * * * *  Validating  * * * * * * * * * * * * * * * * * */
        GetIsValid: function(){
            return this.validationManager.getIsValid();
        },
        GetErrorText: function(){
            return this.validationManager.getErrorText();
        },
        SetIsValid: function(isValid, validating){
            this.validationManager.setIsValid(isValid, validating);
        },
        SetErrorText: function(errorText, validating){
            this.validationManager.setErrorText(errorText, validating);
        },
        Validate: function() {
            this.validationManager.validate();
        },
        /*region* * * * * * * * * * * * * * * * * *  API  * * * * * * * * * * * * * * * * * */
        RaiseBeforePaste: function(commandName, html) {
            if(!this.BeforePaste.IsEmpty()) {
                var args = new ASPxClientHtmlEditorBeforePasteEventArgs(commandName, html);
                this.BeforePaste.FireEvent(this, args);
                return args.html;
            }
            return html;
        },
        RaiseCommandExecuted: function(commandName, parameter, isSuccessful) {
            this.RaiseEventWithArgsInternal(
                "CommandExecuted",
                new ASPxClientHtmlEditorCommandEventArgs(commandName, parameter, isSuccessful, this)
            );
        },
        RaiseCommandExecuting: function(commandName, parameter) {
            return this.RaiseEventWithArgsInternal(
                "CommandExecuting",
                new ASPxClientHtmlEditorCommandExecutingEventArgs(commandName, parameter, this)
            ).cancel;
        },
        RaiseCustomCommand: function(commandName, parameter) {
            if(!this.CustomCommand.IsEmpty()) {
                var args = new ASPxClientHtmlEditorCommandEventArgs(commandName, parameter, true, this);
                this.CustomCommand.FireEvent(this, args);
                return args.isSuccessful;
            }
            return true;
        },
        RaiseEventWithArgsInternal: function(eventName, args) {
            var evt = this[eventName];
            if(!evt.IsEmpty())
                evt.FireEvent(this, args);
            return args;
        },
        RaiseEventInternal: function(eventName) {
            this.RaiseEventWithArgsInternal(eventName, new ASPxClientEventArgs());
        },
        RaiseFocus: function() {
            this.RaiseEventInternal("GotFocus");
        },
        RaiseLostFocus: function() {
            this.RaiseEventInternal("LostFocus");
        },
        RaiseHtmlChanged: function() {
            if(this.canRaiseEvents)
                this.RaiseEventInternal("HtmlChanged");
        },
        RaiseSelectionChanged: function() {
            if(this.canRaiseEvents)
                this.RaiseEventInternal("SelectionChanged");
        },
        RaiseCustomDialogOpened: function(name) {
            this.RaiseEventWithArgsInternal(
                "CustomDialogOpened",
                new ASPxClientHtmlEditorCustomDialogEventArgs(name)
            );
        },
        RaiseCustomDialogClosing: function(name, status) {
            return this.RaiseEventWithArgsInternal(
                "CustomDialogClosing",
                new ASPxClientHtmlEditorCustomDialogClosingEventArgs(name, status)
            ).handled;
        },
        RaiseCustomDialogClosed: function(name, status, data) {
            this.RaiseEventWithArgsInternal(
                "CustomDialogClosed",
                new ASPxClientHtmlEditorCustomDialogClosedEventArgs(name, status, data)
            );
        },
        RaiseValidation: function(html, isValid, errorText) {
            return this.RaiseEventWithArgsInternal(
                "Validation",
                new ASPxClientHtmlEditorValidationEventArgs(html, isValid, errorText)
            );
        },
        RaiseContextMenuShowing: function() {
            this.RaiseEventInternal("ContextMenuShowing");
        },
        raiseSpellingChecked: function() {
            this.RaiseEventInternal("SpellingChecked");
        },
        RaiseActiveTabChanged: function(name) {
            this.RaiseEventWithArgsInternal(
                "ActiveTabChanged",
                new ASPxClientHtmlEditorTabEventArgs(name)
            );
        },
        RaiseActiveTabChanging: function(arg) {
            this.RaiseEventWithArgsInternal("ActiveTabChanging", arg);
        },
        SetHeight: function(height) {
            this.layoutManager.setHeight(height);
        },
        SetWidth: function(width) {
            this.layoutManager.setWidth(width);
        },
        OnBrowserWindowResizeInternal: function(evt) {
            if(this.isInitialized)
                this.AdjustControlCore();
        },
        Export: function(format) {
            this.SendPostBack("Export_" + format);
        },
        SetActiveTabByName: function(name) {
           this.statusBarManager.setActiveTabByName(name);
        },
        GetActiveTabName: function() {
            return this.statusBarManager.getActiveTabName();
        },
        ReconnectToExternalRibbon: function() {
            this.barDockManager.reconnectToExternalRibbon();
        }
    });
    ASPxClientHtmlEditor.Cast = ASPxClientControl.Cast;

    ASPxClientHtmlEditor.OnCustomDialogClosing = function(sender) {
        var buttonName = sender.name.substr(sender.name.length - 2) == "Ok" ? "ok" : "cancel";
        var htmlEditor = ASPx.Dialog.GetOwnerControl();
        var dialog = ASPx.Dialog.GetCurrentDialog();
        if(htmlEditor && dialog) {
            if(!htmlEditor.RaiseCustomDialogClosing(dialog.publicName, buttonName))
                ASPxClientHtmlEditor.CustomDialogComplete(buttonName, null);
        }
    };
    ASPxClientHtmlEditor.CustomDialogComplete = function(status, data) {
        ASPx.DialogComplete(status, data);
    };
    ASPxClientHtmlEditor.AreEditorsValidInContainer = function(containerOrContainerId, validationGroup, checkInvisibleEditors) {
        var container = typeof(containerOrContainerId) == "string" ? document.getElementById(containerOrContainerId) : containerOrContainerId;
        return HtmlEditorsCollection.Get().AreEditorsValidInContainer(container, validationGroup, checkInvisibleEditors);
    }
    ASPxClientHtmlEditor.ClearEditorsInContainer = function(containerOrContainerId, validationGroup, clearInvisibleEditors) {
        var container = typeof(containerOrContainerId) == "string" ? document.getElementById(containerOrContainerId) : containerOrContainerId;
        return HtmlEditorsCollection.Get().ClearEditorsInContainer(container, validationGroup, clearInvisibleEditors);
    }
    ASPxClientHtmlEditor.ValidateEditorsInContainer = function(containerOrContainerId, validationGroup, validateInvisibleEditors) {
        var container = typeof (containerOrContainerId) == "string" ? document.getElementById(containerOrContainerId) : containerOrContainerId;
        var validationGroup = !!validationGroup == false ? "" : validationGroup;
        return HtmlEditorsCollection.Get().ValidateEditorsInContainer(container, validationGroup, validateInvisibleEditors);
    }

    ASPx.HEChangeActiveView = function(name, evt) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null)
            htmlEdit.statusBarManager.changeActiveView(evt.tab.name);
    }
    ASPx.HEChangingActiveView = function(name, evt) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null) {
            var arg = new ASPxClientHtmlEditorTabCancelEventArgs(htmlEdit.statusBarManager.getFullViewName(evt.tab.name));
            htmlEdit.RaiseActiveTabChanging(arg);
            evt.cancel = arg.cancel;
        }
    }

    // Editing Area event handlers
    window.FocusActiveEditorToolbar = function() {
        HtmlEditorsCollection.Get().FocusActiveEditorToolbar();
    }
    
    ASPx.HEEFCBClick = function(evt, name) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null) htmlEdit.OnErrorFrameCloseButtonClick();
    }

    ASPx.HESpellCheckerWordChanged = function(name, sender, args) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null)
            return htmlEdit.spellCheckerHelper.OnWordChanged(sender, args);
    }
    ASPx.HEDesignViewIframeOnLoad = function(name, iframe) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null)
            htmlEdit.OnDesignViewIframeLoad();
        iframe.loadUnhandled = !htmlEdit;
    }
    ASPx.HEHtmlViewHtmlChanged = function(name) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null)
            htmlEdit.getHtmlViewEventManager().onHtmlChanged();
    }
    ASPx.HERibbonMinimizationStateChanged = function(name, evt) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit) {
            var ribbon = htmlEdit.barDockManager.getRibbon(true);
            if(ribbon) {
                if(!htmlEdit.layoutManager.isFullscreenMode || !htmlEdit.core.isDesignView())
                    return;
                else {
                    var height = ribbon.GetMainElement().offsetHeight;
                    ASPx.Attr.ChangeStyleAttribute(htmlEdit.GetMainElement(), "top", height + "px");
                }
            }
            if(!htmlEdit.layoutManager.adjustSizeInFullscreen())
                htmlEdit.layoutManager.restoreHeight();
        }
    }
    ASPx.HERibbonActiveTabChanged = function(name, e) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit)
            htmlEdit.barDockManager.eventManager.onRibbonActiveTabChanged();
    }
    if(ASPx.Browser.NetscapeFamily)
        ASPx.Evt.AttachEventToDocument("dragover", function() { ASPx.HtmlEditorClasses.IsDocumentDragOver = true; });
    if(ASPx.Browser.NetscapeFamily)
        ASPx.Evt.AttachEventToDocument("dragdrop", function() { ASPx.HtmlEditorClasses.IsDocumentDragOver = false; });

    // PopupMenu
    ASPx.HEContextMenuCloseUp = function(name) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null) return htmlEdit.contextMenuManager.eventManager.onContextMenuCloseUp();
    }
    ASPx.HEContextMenuItemClick = function(name, args) {
        var htmlEdit = ASPx.GetControlCollection().Get(name);
        if(htmlEdit != null) return htmlEdit.contextMenuManager.eventManager.onContextMenuItemClick(args.item);
    }
    ASPx.HEPasteOptionsItemClick = function(name, args) {
        var htmlEditor = ASPx.GetControlCollection().Get(name);
        if(htmlEditor != null) {
            var wrapper = htmlEditor.core.getActiveWrapper();
            htmlEditor.ExecuteCommandInternal(args.item.name, null, false);
            htmlEditor.pasteOptionsBarManager.updatePasteFormattingItemsState();
        }
    }

    window.ASPxClientHtmlEditor = ASPxClientHtmlEditor;
    window.ASPxClientHtmlEditorCommandEventArgs = ASPxClientHtmlEditorCommandEventArgs;
    window.ASPxClientHtmlEditorCustomDialogEventArgs = ASPxClientHtmlEditorCustomDialogEventArgs;
    window.ASPxClientHtmlEditorCustomDialogCloseEventArgsBase = ASPxClientHtmlEditorCustomDialogCloseEventArgsBase;
    window.ASPxClientHtmlEditorCustomDialogClosingEventArgs = ASPxClientHtmlEditorCustomDialogClosingEventArgs;
    window.ASPxClientHtmlEditorCustomDialogClosedEventArgs = ASPxClientHtmlEditorCustomDialogClosedEventArgs;
    window.ASPxClientHtmlEditorValidationEventArgs = ASPxClientHtmlEditorValidationEventArgs;
    window.ASPxClientHtmlEditorTabEventArgs = ASPxClientHtmlEditorTabEventArgs;
    window.ASPxClientHtmlEditorTabCancelEventArgs = ASPxClientHtmlEditorTabCancelEventArgs;
    window.ASPxClientHtmlEditorCommandExecutingEventArgs = ASPxClientHtmlEditorCommandExecutingEventArgs;

    ASPx.HtmlEditorsCollection = HtmlEditorsCollection;
})();