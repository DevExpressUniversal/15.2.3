(function() {
    var dragDropDelay = 100;
    
    var handlerTable = {};

    var EventManagerBase = ASPx.CreateClass(null, {
        constructor: function(owner) {
            this.owner = owner;
            this.attachEvents();
        },
        attachEvents: function() { },
        attachEventToElement: function(element, eventName, func, onlyBubbling) {
            handlerTable[func] = function(evt) {
                this.currentEvt = evt;
                func(evt);
                this.currentEvt = null;
            }.aspxBind(this);
            ASPx.Evt.AttachEventToElement(element, eventName, handlerTable[func], onlyBubbling);        
        },
        detachEventFromElement: function(element, eventName, func) {
            ASPx.Evt.DetachEventFromElement(element, eventName, handlerTable[func]);
            delete handlerTable[func];
        },
        executeCommand: function(commandName, parameter, saveToUndoHistory) {
            var cancel = this.owner.raiseCommandExecuting(commandName, parameter);
            if(cancel && this.currentEvt)
                ASPx.Evt.PreventEvent(this.currentEvt);
            return !cancel && this.owner.raiseExecuteCommand.apply(this.owner, arguments);
        }
    });

    ASPx.HtmlEditorClasses.Managers.ContextMenuEventManager = ASPx.CreateClass(EventManagerBase, {
        attachEvents: function() {
            var wrapper = this.owner.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Design);
            if(wrapper) {
                var doc = wrapper.getDocument();
                if(ASPx.Browser.Safari  || (ASPx.Browser.WebKitFamily && ASPx.Browser.MacOSPlatform)) { //B189872
                    this.attachEventToElement(doc, "contextmenu", function (evt) { evt.preventDefault(); });
                    this.attachEventToElement(doc, "mouseup", function(evt) {
                        if(evt.button == 2)
                            this.onContextMenu(evt, wrapper);
                    }.aspxBind(this));
                }
                else
                    this.attachEventToElement(doc, "contextmenu", function(evt) { this.onContextMenu(evt, wrapper); }.aspxBind(this));
            }
            var htmlViewWrapper = this.owner.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Html);
            if(htmlViewWrapper && !this.owner.htmlEditor.isSimpleHtmlEditingMode())
                this.attachEventToElement(htmlViewWrapper.sourceEditor.getWrapperElement(), "contextmenu", function(evt) { this.onContextMenu(evt, htmlViewWrapper, true); }.aspxBind(this));
        },
        onContextMenuItemClick: function(item) {
            var wrapper = this.owner.core.getActiveWrapper();
            if(wrapper.restoreSelectionForPopup)
                wrapper.restoreSelectionForPopup();
            if(ASPx.Browser.IE)
                wrapper.eventManager.isPreventKeyPressOnShowContextMenu = false;
            window.setTimeout(function() { // We should execute an action when the menu has been closed
                this.htmlEditor.ExecuteCommandInternal(item.name, null);
            }.aspxBind(this.owner), 0);
        },
        onContextMenuCloseUp: function() {
            var wrapper = this.owner.core.getActiveWrapper();
            if(wrapper.getName() == ASPx.HtmlEditorClasses.View.Design && wrapper.selectionManager.beforePopupSelection)
                wrapper.selectionManager.removeBeforePopupBookmark();
        },
        onContextMenu: function(evt, wrapper, useTimeOut) {
            if(!useTimeOut)
                this.owner.showContextMenu(evt, wrapper);
            else {
                setTimeout(function() {
                    this.owner.showContextMenu(evt, wrapper);
                }.aspxBind(this), 0);
            }
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9)
                evt.returnValue = false;
            else
                evt.preventDefault();
        }
    });

    ASPx.HtmlEditorClasses.Managers.BarDockEventManager = ASPx.CreateClass(EventManagerBase, {
        attachEvents: function() {
            var ribbon = this.owner.getRibbon(true);
            if(ribbon) {
                var htmlEditor = this.owner.htmlEditor;
                if(ribbon && ribbon.CommandExecuted) {
                    ribbon.CommandExecuted.removeHandlerByControlName(htmlEditor.name);
                    ribbon.CommandExecuted.AddHandler(function(s, e) { ASPx.HEToolbarCommand(htmlEditor.name, e.item, e.parameter); }, htmlEditor);
                }
                if(ribbon && ribbon.MinimizationStateChanged) {
                    ribbon.MinimizationStateChanged.removeHandlerByControlName(htmlEditor.name);
                    ribbon.MinimizationStateChanged.AddHandler(function(s, e) { ASPx.HERibbonMinimizationStateChanged(htmlEditor.name, e); }, htmlEditor);
                }
            }
        },
        onRibbonActiveTabChanged: function() {
            this.owner.saveRibbonClientState();
        }
    });

    ASPx.HtmlEditorClasses.Managers.LayoutEventManager = ASPx.CreateClass(EventManagerBase, {
        attachEvents: function() {
            var htmlEditor = this.owner.htmlEditor;
            if(htmlEditor.enabled && htmlEditor.allowResize) {
                var sizeGrip = htmlEditor.getSizeGrip();
                if(sizeGrip) {
                    ASPx.Evt.PreventElementDrag(sizeGrip);
                    this.attachEventToElement(sizeGrip, "mousedown", function(evt) {
                        this.onSizeGripMouseDown(evt);
                    }.aspxBind(this));
                    ASPx.Evt.AttachEventToDocument("mouseup", function(evt) {
                        this.onSizeGripMouseUp(evt);
                    }.aspxBind(this));
                    ASPx.Evt.AttachEventToDocument("mousemove", function(evt) {
                        this.onSizeGripMouseMove(evt);
                    }.aspxBind(this));
                }
            }
        },
        onSizeGripMouseDown: function(evt) {
            this.owner.resizeTempVars.isInResize = false;
            if(this.owner.htmlEditor.GetEnabled()) {
                this.owner.setResizingPanelVisibility(true);
                this.owner.saveCurrentSizeGripPosition(evt, true);
                this.owner.saveStartSize();
                ASPx.Attr.ChangeStyleAttribute(this.owner.resizingPanel, "cursor", this.owner.htmlEditor.rtl ? "ne-resize" : "se-resize");
                this.owner.resizeTempVars.isInMove = true;
            }
        },
        onSizeGripMouseUp: function(evt) {
            if(!this.owner.resizeTempVars.isInMove)
                return;
            this.owner.setResizingPanelVisibility(false);
            ASPx.Attr.RestoreStyleAttribute(this.owner.resizingPanel, "cursor");
            this.owner.saveCurrentSize(true, true, true, true);
            this.owner.resizeTempVars.isInMove = false;
            this.owner.resizeTempVars.isInResize = false;
            this.owner.resizeTempVars = {};
        },
        onSizeGripMouseMove: function(evt) {
            if(!this.owner.resizeTempVars.isInMove || this.owner.resizeTempVars.isInResize)
                return;
            if(ASPx.Browser.IE && !ASPx.Evt.IsLeftButtonPressed(evt)) {
                this.onSizeGripMouseUp(evt);
                return;
            }
            this.owner.isWidthDefinedInPercent = false;
            this.owner.resizeTempVars.isInResize = true;
            this.owner.saveCurrentSizeGripPosition(evt);
            this.owner.setDeltaSize();
            this.owner.resizeTempVars.isInResize = false;
        }
    });

    ASPx.HtmlEditorClasses.Managers.WrapperEventManager = ASPx.CreateClass(EventManagerBase, {
        attachEvents: function() {
            this.attachKeyDown();
        },
        attachKeyDown: function() {
        },
        onKeyDown: function(evt) {
            if(!this.owner.enabled)
                return false;
            var isAllowToPreventShortcut = false;
            var lastShortcut = this.owner.getShortcutCommand(evt);
            if(lastShortcut) {
                this.executeCommand(lastShortcut, null);
                isAllowToPreventShortcut = !this.owner.commandManager.isDefaultActionCommand(lastShortcut);  
            }
            else if(this.owner.isBrowserShortcut(evt))
                return ASPx.Evt.PreventEvent(evt);
            if(this.owner.getLastShortcutID() && (ASPx.Browser.IE || ASPx.Browser.WebKitFamily) && isAllowToPreventShortcut)
                this.preventEventIE(evt);
            return true;
        },
        preventEventIE: function(evt) {    
            evt.keyCode = 123;
            evt.returnValue = false;
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8)
                ASPx.Evt.PreventEvent(evt);
            return false;
        },
        onHtmlChanged: function() {
            this.owner.raiseHtmlChanged();
        },
        isUpdateLocked: function() {
            return false;
        },
        onFocus: function() {
            if(this.owner.isInFocus || !this.owner.enabled) 
                return;
            this.owner.isInFocus = true;
            this.owner.raiseFocus(this.owner);
        },
        onLostFocus: function() {
            if(!this.owner.isInFocus || !this.owner.enabled)
                return;
            this.owner.isInFocus = false;
            this.owner.raiseLostFocus(this.owner);
        }
    });

    ASPx.HtmlEditorClasses.Managers.PreviewEventManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.WrapperEventManager, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
        },
        attachKeyDown: function() {
            var doc = this.owner.getDocument();
            if(doc) {
                this.attachEventToElement(doc, "keydown", function(evt) { this.onKeyDown(evt); }.aspxBind(this));
                var win = this.owner.getWindow();
                this.attachEventToElement(win, "focus", function(evt) { this.onFocus(); }.aspxBind(this));
                this.attachEventToElement(win, "blur", function(evt) { this.onLostFocus(); }.aspxBind(this));
            }
        }
    });

    var equalCharCode = 187;
    var ltChar = 188;

    ASPx.HtmlEditorClasses.Managers.HtmlViewCMEventManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.WrapperEventManager, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
        },
        attachEvents: function() {
            ASPx.HtmlEditorClasses.Managers.WrapperEventManager.prototype.attachEvents.call(this);
            this.owner.sourceEditor.on("keyup", function(s, evt) { this.onKeyUp(evt); }.aspxBind(this));
            this.attachHtmlChangedEventToEditor();
        },
        attachKeyDown: function() {
            this.owner.sourceEditor.on("keydown", function(s, evt) { this.onKeyDown(evt); }.aspxBind(this));
        },
        attachHtmlChangedEventToEditor: function() {
            this.owner.sourceEditor.on("change", this.getHtmlChangedEventHandler());
        },
        detachHtmlChangedEventToEditor: function() {
            this.owner.sourceEditor.off("change", this.getHtmlChangedEventHandler());
        },
        onKeyUp: function(evt) {
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            if(this.owner.settings.enableAutoCompletion) {
                if(!this.owner.isIntelliSenseWindowShown) {
                    if((keyCode == ASPx.Key.Space || keyCode == equalCharCode)) {
                        var sourceEditor = this.owner.getSourceEditor(),
                            posState = this.owner.selectionManager.getPositionState();
                        if(posState == ASPx.HtmlEditorClasses.TokenType.AttributeState || posState == ASPx.HtmlEditorClasses.TokenType.AttributeEqual) {
                            if(posState == ASPx.HtmlEditorClasses.TokenType.AttributeEqual && keyCode == equalCharCode) {
                                sourceEditor.replaceSelection("\"\"");
                                sourceEditor.execCommand("goCharLeft");
                            }
                            this.executeCommand(ASPxClientCommandConsts.SHOWINTELLISENSE_COMMAND, null);
                        }
                    }
                    else if(evt.shiftKey && keyCode == ltChar)
                        this.executeCommand(ASPxClientCommandConsts.SHOWINTELLISENSE_COMMAND, null);
                }
            }
        },
        getHtmlChangedEventHandler: function() {
            if(!this.htmlChangedEventHandler)
                this.htmlChangedEventHandler = this.onHtmlChanged.aspxBind(this);
            return this.htmlChangedEventHandler;
        },
        onHtmlChanged: function() {
            this.owner.raiseHtmlChanged();
        }
    });

    ASPx.HtmlEditorClasses.Managers.HtmlViewMemoEventManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.WrapperEventManager, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
        },
        attachKeyDown: function() {
            this.owner.getHtmlViewEditControl().KeyDown.AddHandler(function(s, e) { this.onKeyDown(e.htmlEvent); }.aspxBind(this));
        }
    });
    
    ASPx.HtmlEditorClasses.Managers.DesignEventManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.WrapperEventManager, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
            this.selectedChangedTimerId = -1;

            this.isLocked = false;
            // for dragdrop operations
            this.isDraggingInsideEditor = false;
            this.isDropExternalContentExecuted = false;
            this.dragDropTimerID = null;
            this.suspendHCEvt = -1;
            this.isMouseDown = false;
        },
        attachEvents: function() {
            ASPx.HtmlEditorClasses.Managers.WrapperEventManager.prototype.attachEvents.call(this);
            var doc = this.owner.getDocument();

            this.attachEventToElement(doc.body, "paste", function (evt) {
                this.onPaste(evt)
             }.aspxBind(this));

            this.attachEventToElement(doc, "keyup", function(evt) { this.onKeyUp(evt); }.aspxBind(this));
            if(!ASPx.Browser.IE) // prevent built-in command shortcuts in Moz, Safari, Opera
                this.attachEventToElement(doc, "keypress", function(evt) { this.onKeyPress(evt); }.aspxBind(this));
            this.attachEventToElement(doc, "mousedown", function(evt) { this.onMouseDown(evt); }.aspxBind(this));

            this.attachEventToElement(doc, "mouseup", function(evt) { this.onMouseUp(evt); }.aspxBind(this));

            if(ASPx.Browser.WebKitTouchUI)
                this.eventEndlerTouchMouseUp = function(evt) { this.onTouchMouseUp(); }.aspxBind(this);
            if(ASPx.Browser.MSTouchUI || ASPx.Browser.WebKitTouchUI){
                this.attachEventToElement(doc, "selectionchange", function() {
                    ASPx.Timer.ClearTimer(this.selectedChangedTimerId);
                    this.selectedChangedTimerId = window.setTimeout(function() {
                        this.selectionChange();
                    }.aspxBind(this), 100);
                }.aspxBind(this));
            }
        
            var designViewWindow = this.owner.getWindow();
            this.attachEventToElement(designViewWindow, "focus", function(evt) { this.onFocus(); }.aspxBind(this));
            this.attachEventToElement(designViewWindow, "blur", function(evt) { this.onLostFocus(); }.aspxBind(this));
        
            if(!ASPx.Browser.Opera)
                this.attachEventToElement(doc, "dblclick", function(evt) { this.onDblClick(evt); }.aspxBind(this));
            if(ASPx.Browser.NetscapeFamily) {
                this.attachEventToElement(designViewWindow, "dragend", function(evt) { this.onObjectDragEnd(evt); }.aspxBind(this));
                this.attachEventToElement(designViewWindow, "dragover", function(evt) { return this.onObjectDragStart(evt); }.aspxBind(this));
                this.attachEventToElement(designViewWindow, "drop", function(evt) { this.onObjectDrop(evt); }.aspxBind(this));
            }
            else {
                this.attachEventToElement(doc.body, "resizestart", function(evt) { this.onObjectResizeStart(evt); }.aspxBind(this));
                this.attachEventToElement(doc.body, "resizeend", function(evt) { this.onObjectResizeEnd(evt); }.aspxBind(this));

                /* global document */
                this.attachEventToElement(document.body, "dragend", function(evt) { this.onDocumentObjectDragEnd(evt); }.aspxBind(this));
                this.attachEventToElement(doc.body, "dragstart", function(evt) { return this.onObjectDragStart(evt); }.aspxBind(this));
                this.attachEventToElement(doc.body, "drop", function(evt) { this.onObjectDrop(evt); }.aspxBind(this));

                this.attachEventToElement(doc.body, "dragend", function(evt) { this.onObjectDragEnd(); }.aspxBind(this));
            }
        },
        AttachEventTouchMouseUpToEditor: function() {
            if(!this.isAttachedEventTouchMouseUp) {
                this.attachEventToElement(this.owner.getDocument(), ASPx.TouchUIHelper.touchMouseUpEventName, this.eventEndlerTouchMouseUp);
                this.isAttachedEventTouchMouseUp = true;
            }
        },
        detachEventTouchMouseUpToEditor: function() {
            if(this.isAttachedEventTouchMouseUp) {
                this.detachEventFromElement(this.owner.getDocument(), ASPx.TouchUIHelper.touchMouseUpEventName, this.eventEndlerTouchMouseUp);
                this.isAttachedEventTouchMouseUp = false;
            }
        },
        attachKeyDown: function() {
            var doc = this.owner.getDocument();
            if(doc)
                this.attachEventToElement(doc, "keydown", function(evt) { this.onKeyDown(evt); }.aspxBind(this));
        },
        setLockUpdate: function(value) {
            this.isLocked = value;
        },
        isUpdateLocked: function() {
            return this.isLocked;
        },
        onHtmlChanged: function(saveSelectionAndHtml, preventEvent, hidePasteOptionsBar) {
            // performance optimization
            var isTextTyping = this.owner.commandManager.isTextTyping();
            if(this.suspendHCEvt > -1) {
                if(isTextTyping) {
                    this.suspendHCEvt = this.suspendHCEvt == 2 || saveSelectionAndHtml ? 2 : 1;
                    return;
                }
                else {
                    saveSelectionAndHtml = saveSelectionAndHtml || this.suspendHCEvt == 2;
                    this.suspendHCEvt = -1;
                }
            }
            this.onHtmlChangedInternal(saveSelectionAndHtml, preventEvent, hidePasteOptionsBar);
            // performance optimization
            if(isTextTyping) {
                this.suspendHCEvt = 0;
                setTimeout(function() {
                    if(this.suspendHCEvt > 0)
                        this.onHtmlChangedInternal(this.suspendHCEvt == 2, preventEvent, hidePasteOptionsBar);
                    this.suspendHCEvt = -1;
                }.aspxBind(this), 300);
            }
        },
        onHtmlChangedInternal: function(saveSelectionAndHtml, preventEvent, hidePasteOptionsBar) {
            if(saveSelectionAndHtml) {
                var commandManager = this.owner.commandManager;
                commandManager.updateLastRestoreSelectionAndHTML();
                if(commandManager.isTextTyping())
                    this.setLockUpdate(true);
            }
            this.owner.raiseHtmlChanged(saveSelectionAndHtml, preventEvent, hidePasteOptionsBar);
            this.onSelectionChanged(hidePasteOptionsBar);
        },
        onSelectionChanged: function(hidePasteOptionsBar) {
            this.owner.clearCachedSeletedElements();
            this.owner.raiseSelectionChanged(this.owner, hidePasteOptionsBar);
            ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() {
                if(!(ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 9) && !ASPx.Browser.WebKitFamily || this.owner.isInFocus) {
                    this.owner.saveSelection();
                    this.setLockUpdate(false);
                }
            }.aspxBind(this), "SaveSelection", ASPx.HtmlEditorClasses.DefaultSaveSelectionTimeoutValue, true);
        },
        callSuspendedHtmlChangedEvt: function() {
            if(this.suspendHCEvt > 0) {
                this.onHtmlChangedInternal(this.suspendHCEvt == 2, false, this.owner.settings.enablePasteOptions);
                this.suspendHCEvt = -1;
            }
        },
        onFocus: function() {
            if(!this.owner.enabled || this.preventFocus) return;
            ASPx.TouchUIHelper.removeDocumentTouchEventHandlers();
            if(ASPx.Browser.WebKitTouchUI) {
                ASPx.GesturesHelper.DetachEvents();
                this.detachEventTouchMouseUpToEditor();
                if(window.parent != window) {
                    if(window.parent.ASPx.GesturesHelper)
                        window.parent.ASPx.GesturesHelper.DetachEvents();
                    if(window.parent.ASPx.TouchUIHelper)
                        window.parent.ASPx.TouchUIHelper.removeDocumentTouchEventHandlers();
                }
            }
            if(this.owner.isInFocus) return;
            this.owner.isInFocus = true;
            if(!this.owner.isSelectionRestored()) {
                if(ASPx.Browser.WebKitFamily)
                    this.owner.restoreSelectionByTimer();
                else
                    this.owner.restoreSelection();
            }
            this.owner.raiseFocus(this.owner);
        },
        onLostFocus: function() {
            if(!this.owner.enabled) return;
            if(this.preventLostFocus || ASPx.HtmlEditorClasses.Utils.HasUnforcedFunction("PreventLostFocus")) {
                this.preventLostFocus = undefined;
                return;
            }
            ASPx.TouchUIHelper.restoreDocumentTouchEventHandlers();
            if(ASPx.Browser.WebKitTouchUI) {
                ASPx.GesturesHelper.AttachEvents();
                this.AttachEventTouchMouseUpToEditor();
                if(window.parent != window) {
                    if(window.parent.ASPx.GesturesHelper)
                        window.parent.ASPx.GesturesHelper.AttachEvents();
                    if(window.parent.ASPx.TouchUIHelper)
                        window.parent.ASPx.TouchUIHelper.restoreDocumentTouchEventHandlers();
                }
            }
            if(!this.owner.isInFocus)
                return;
            this.owner.isInFocus = false;
            this.owner.selectionManager.isSelectionRestored = false;
            this.owner.raiseLostFocus(this.owner);
        },
        processTabOnKeyDown: function(evt) {
            if(ASPx.Browser.IE && evt.keyCode == ASPx.Key.Tab) {
                try {
                    if(evt.shiftKey) {
                        this.setLockUpdate(true);
                        this.owner.focusLastToolbar(); 
                        ASPxClientHtmlEditor.PreventEventIE(evt);
                    }
                }
                catch (e) { }
            }
        },
        processingPlaceholderOnKeyDown: function(owner, evt, keyDownInfo, placeholder) {
            var doc = this.owner.getDocument();
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            if(keyDownInfo.isCursorMovingKey) {
                var selection = owner.getSelection();
                if(keyCode != ASPx.Key.Left && keyCode != ASPx.Key.Right) {
                    owner.placeholderManager.setAllowEditElement(placeholder, true);
                    var selection = owner.getSelection();
                    setTimeout(function() {
                        owner.getSelection().getSpecialSelection().removeHiddenContainer();
                        owner.placeholderManager.setAllowEditElement(placeholder, false);
                    }.aspxBind(this), 0);
                }
                else {
                    selection.getSpecialSelection().removeHiddenContainer();
                    var startMarker = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
                    var endMarker = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
                    if(keyCode == ASPx.Key.Left) {
                        placeholder.parentNode.insertBefore(endMarker, placeholder);
                        placeholder.parentNode.insertBefore(startMarker, placeholder);
                    }
                    else {
                        ASPx.InsertElementAfter(startMarker, placeholder);
                        ASPx.InsertElementAfter(endMarker, placeholder);
                    }
                    selection.clientSelection.SelectExtendedBookmark({ "startMarkerID": startMarker.id, "endMarkerID": endMarker.id });
                    ASPx.Evt.PreventEvent(evt);
                }
            }
            if(keyCode != ASPx.Key.Left && keyCode != ASPx.Key.Right)
                owner.getSelection().clientSelection.applySpecialSelectionToElement(placeholder, true);
        },
        onKeyDown: function(evt) {
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            if(!ASPx.Browser.WebKitTouchUI && !ASPx.Browser.MSTouchUI && (this.owner.commandManager.typeCommandProcessor.hasStyleCommand())) {
                var isSystemKey = ASPx.HtmlEditorClasses.Managers.KeyboardManager.IsSystemKey(keyCode) || keyCode == 0;
                if(!isSystemKey || keyCode == ASPx.Key.Space) {
                    setTimeout(function () {
                        ASPx.HtmlEditorClasses.Commands.Browser.ApplyStyle(this.owner.commandManager);
                    }.aspxBind(this), 0);
                } 
                else if(keyCode == ASPx.Key.Ctrl || keyCode == ASPx.Key.Shift || keyCode == ASPx.Key.Alt)
                    this.setLockUpdate(true);
                else {
                    this.setLockUpdate(false);
                    this.owner.commandManager.typeCommandProcessor.clearCommands();
                }
            }
            var doc = this.owner.getDocument();
            var lastShortcut = this.owner.getShortcutCommand(evt);
            var hiddenElement = doc.getElementById(ASPx.HtmlEditorClasses.SelectedHiddenContainerID);
            if(hiddenElement) {
                var keyDownInfo = this.owner.getKeyDownInfo(evt);
                if(!ASPx.HtmlEditorClasses.Managers.KeyboardManager.IsSystemKey(keyCode) && !lastShortcut && !this.owner.isBrowserShortcut(evt) || keyCode == ASPx.Key.Enter || keyCode == ASPx.Key.Space)
                    return ASPx.Evt.PreventEvent(evt);
                else if(keyDownInfo.isCursorMovingKey || lastShortcut || this.owner.isBrowserShortcut(evt))
                    this.processingPlaceholderOnKeyDown(this.owner, evt, keyDownInfo, this.owner.getSelection().GetSelectedElement());
                else if(keyDownInfo.isDeleteOrBackSpaceKey)
                    ASPx.Evt.PreventEvent(evt);
            }
            if(ASPx.Browser.IE) {
                if(this.isPreventKeyPressOnShowContextMenu) {
                    ASPx.Evt.PreventEventAndBubble(evt);
                    return false;
                }
                if(evt.shiftKey) {
                    this.setLockUpdate(true);
                    if(evt.keyCode == ASPx.Key.Tab)
                        this.owner.saveSelection();
                }
                if(ASPx.Browser.IE && lastShortcut == ASPxClientCommandConsts.KBPASTE_COMMAND) {
                    if(this.owner.settings.enablePasteOptions || this.owner.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.MergeFormatting)
                        this.owner.commandManager.getCommand(ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND).initStyleStateArray(this.owner);
                    ASPx.HtmlEditorClasses.Utils.createPasteContainer(this.owner);
                }
            }
            this.processTabOnKeyDown(evt); // accessibility, B34713
            this.isAllowToPreventShortcut = false;
            if(!lastShortcut) // Q411236
                this.owner.selectionManager.beforePopupSelection = null;
            if(!ASPx.Browser.IE && evt.keyCode == ASPx.Key.F10) { // accessibility
                 ASPx.HtmlEditorsCollection.Get().FocusActiveEditorToolbar();
                return ASPx.Evt.PreventEvent(evt);
            }
            else if(lastShortcut) {
                if(lastShortcut == ASPxClientCommandConsts.KBPASTE_COMMAND && this.owner.commandManager.getLastPasteFormattingHtml())
                    this.owner.commandManager.clearPasteOptions(false);
                ASPx.HtmlEditorClasses.Utils.ClearUnforcedFunctionByKey("SaveSelection");
                this.owner.saveSelection();
                this.executeCommand(lastShortcut, null);
                this.isAllowToPreventShortcut = !this.owner.commandManager.isDefaultActionCommand(lastShortcut);   
                if(lastShortcut == ASPxClientCommandConsts.KBCOPY_COMMAND || lastShortcut == ASPxClientCommandConsts.KBCUT_COMMAND)
                    ASPx.HtmlEditorClasses.Commands.Utils.processingElementsBeforeCope(this.owner.getSelection().GetElements());
            }
            else if(this.owner.isBrowserShortcut(evt))
                return ASPx.Evt.PreventEvent(evt);
            else {
                var keyDownInfo = this.owner.getKeyDownInfo(evt);
                if(keyDownInfo.isSystemKey && keyCode != ASPx.Key.Space) {
                    if(keyDownInfo.isDeleteOrBackSpaceKey) {
                        var selection = this.owner.getSelection();
                        var startListItem = ASPx.GetParentByTagName(selection.clientSelection.GetStartContainer(), "LI");
                        var endListItem = ASPx.GetParentByTagName(selection.clientSelection.GetEndContainer(), "LI");
                        if((ASPx.Browser.IE || ASPx.Browser.Opera) && !keyDownInfo.isBackSpaceKey && !selection.GetHtml() || endListItem && startListItem && endListItem != startListItem && (ASPx.GetParentByTagName(startListItem.parentNode, "LI") || ASPx.GetParentByTagName(endListItem.parentNode, "LI"))) {
                            this.executeCommand(ASPxClientCommandConsts.DELETE_COMMAND, null, true);
                            ASPx.Evt.PreventEvent(evt);
                        }
                        else {
                            this.executeCommand(ASPxClientCommandConsts.KBDELETE_COMMAND, null, true);
                            this.owner.commandManager.updateLastRestoreHtml();
                        }
                        if(ASPx.Browser.WebKitFamily && !this.owner.getHtml()) {
                            var doc = this.owner.getDocument();
                            doc.body.appendChild(doc.createTextNode("\xA0"));
                            ASPxClientHtmlEditorSelection.SelectElement(doc.body, this.owner.getWindow());
                        }
                    }
                }
                else if(!keyDownInfo.isCursorMovingKey && !this.owner.commandManager.isTextTyping())
                    this.executeCommand(ASPxClientCommandConsts.TEXTTYPE_COMMAND, null, true);
                else if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9){
                    ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() { 
                        this.owner.saveSelection();
                    }.aspxBind(this), "SaveSelection", ASPx.HtmlEditorClasses.DefaultSaveSelectionTimeoutValue, true);
                }
            }
            // Cancel event in IE, Edge and WebKitFamily
            if (this.owner.getLastShortcutID() && (ASPx.Browser.IE || ASPx.Browser.Edge || ASPx.Browser.WebKitFamily) && this.isAllowToPreventShortcut)
                this.preventEventIE(evt);
        },
        onKeyUp: function(evt) {
            if(!this.owner.enabled) return;
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            if(ASPx.Browser.IE) {
                if(this.isPreventKeyPressOnShowContextMenu) {
                    ASPx.Evt.PreventEventAndBubble(evt);
                    return false;
                }
                if(keyCode == ASPx.Key.Shift)
                    this.setLockUpdate(false);
            }
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            var keyDownInfo = this.owner.getKeyDownInfo(evt);
            var lastShortcutID = this.owner.getLastShortcutID();
            if(!lastShortcutID) {
                if(this.owner.commandManager.isDeleting()) {
                    var emptyDeleted = this.owner.commandManager.cleanEmptyRestoreHtml();
                    ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(this.owner.getBody());
                }
                if(!ASPx.HtmlEditorClasses.Managers.KeyboardManager.IsSystemKey(keyCode) || keyCode == ASPx.Key.Space || keyCode == 0)
                    this.setLockUpdate(true);
                if(this.owner.commandManager.isTextTyping() || (this.owner.commandManager.isDeleting() && !emptyDeleted) || this.owner.isSpacing())
                    this.onHtmlChanged(true, false, this.owner.settings.enablePasteOptions);
                else
                    this.onSelectionChanged(!keyDownInfo.isControlKey || keyCode != ASPx.Key.Ctrl || !this.owner.commandManager.getLastPasteFormattingHtml());
                if(this.owner.settings.enablePasteOptions && keyDownInfo.isControlKey && keyCode == ASPx.Key.Ctrl && this.owner.commandManager.getLastPasteFormattingHtml())
                    this.owner.showPasteOptionsBar();
                this.owner.clearKeyDownInfo();
            } else if(this.owner.commandManager.isDefaultActionCommand(lastShortcutID) && this.owner.commandManager.isHtmlChangeableCommand(lastShortcutID)) {
                // Don't change order !!!
                if(keyCode == ASPx.Key.Enter && !this.isAllowToPreventShortcut)
                    this.owner.commandManager.executeCommand(ASPxClientCommandConsts.SAVESTATEUNDOREDOSTACK_COMMAND, null, true);
                var pasteCommand = (lastShortcutID == ASPxClientCommandConsts.KBPASTE_COMMAND) || (lastShortcutID == ASPxClientCommandConsts.PASTEHTMLSOURCEFORMATTING_COMMAND) || (lastShortcutID == ASPxClientCommandConsts.PASTEHTMLPLAINTEXT_COMMAND) ||
                    (lastShortcutID == ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND);
                if(!pasteCommand)
                    this.onHtmlChanged(true);
            }
            else
                this.onSelectionChanged(true);
        },
        onKeyPress: function(evt) {
            if(!this.owner.enabled) return;
            if(ASPx.Browser.IE && this.isPreventKeyPressOnShowContextMenu) {
                ASPx.Evt.PreventEventAndBubble(evt);
                return false;
            }
            if(this.owner.getLastShortcutID() && this.isAllowToPreventShortcut) {
                this.owner.clearLastShortcut();
                return ASPx.Evt.PreventEvent(evt);
            }
        },
        onDblClick: function(evt) {
            if(!this.owner.enabled) return;
            if(ASPx.Browser.IE)
                this.isPreventKeyPressOnShowContextMenu = false;
            var source = ASPx.Evt.GetEventSource(evt);
            if(!source) return;
            if(source.tagName == "IMG") {
                // B32106 - it was impossible to open existent image editing dialog
                if(ASPx.Browser.WebKitFamily)
                    ASPxClientHtmlEditorSelection.SelectElement(source, this.owner.getWindow());
                this.owner.commandManager.executeCommand(this.getOnImageClickCommandName(source));
            }
            else { 
                var regex = new RegExp(ASPx.HtmlEditorClasses.PlaceholderCssClasseName + "|" + ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName + "|" + ASPx.HtmlEditorClasses.PlaceholderEndMarkCssClasseName + "|" + ASPx.HtmlEditorClasses.PlaceholderContentClasseName,"ig");
                if(source.className && regex.test(source.className))
                    this.owner.commandManager.executeCommand(ASPxClientCommandConsts.CHANGEPLACEHOLDER_DIALOG_COMMAND);
            }
        },
        getOnImageClickCommandName: function(img) {
            if(img.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Audio) > -1)
                return ASPxClientCommandConsts.CHANGEAUDIO_DIALOG_COMMAND;
            else if (img.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Video) > -1)
                return ASPxClientCommandConsts.CHANGEVIDEO_DIALOG_COMMAND;
            else if(img.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Flash) > -1 || img.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported) > -1)
                return ASPxClientCommandConsts.CHANGEFLASH_DIALOG_COMMAND;
            else if(img.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.YouTube) > -1)
                return ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_DIALOG_COMMAND;
            return ASPxClientCommandConsts.CHANGEIMAGE_DIALOG_COMMAND;
        },
        shouldLostFocusBePrevented: function(evt, mouseDown) {
            var source = ASPx.Evt.GetEventSource(evt);
            var isLeftButtonPressed = ASPx.Evt.IsLeftButtonPressed(evt) || ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 && evt.button == 0;
            return (ASPx.Browser.IE || ASPx.Browser.WebKitFamily && !this.owner.isInFocus) && source && source.tagName == "HTML" && (!mouseDown || isLeftButtonPressed);
        },
        setPreventEventFlag: function() {
            if(this.owner.isInFocus)
                this.setPreventLostFocusEventFlag();
            else
                this.setPreventFocusEventFlag();
        },
        setPreventLostFocusEventFlag: function() {
            this.preventLostFocus = true;
            if(ASPx.Browser.IE) {
                window.setTimeout(function() {
                    this.preventLostFocus = true;
                }.aspxBind(this), 100);
            }
        },
        setPreventFocusEventFlag: function() {
            this.preventFocus = true;
            window.setTimeout(function() {
                this.preventFocus = undefined;
            }.aspxBind(this), 100);
        },
        onMouseDown: function(evt) {
            if(!this.owner.enabled) return;
            if(this.shouldLostFocusBePrevented(evt, true)) {
                this.setPreventEventFlag();
                return ASPx.Browser.WebKitFamily ? false : ASPx.Evt.PreventEventAndBubble(evt);
            }
            if(ASPx.Browser.IE) {
                this.isPreventKeyPressOnShowContextMenu = false;
                if(!this.owner.settings.disableBrowserSpellCheck && this.owner.settings.allowContextMenu != "default")
                    this.owner.getBody().spellcheck = false;
            }
            this.owner.hideAllPopups();
            var source = ASPx.Evt.GetEventSource(evt);

            var needRestoreScrollPosition;
            if(!this.owner.isInFocus) {
                this.owner.saveScrollPosition();
                needRestoreScrollPosition = true;
            }
            var doc = this.owner.getDocument();
            var selection = this.owner.getSelection();
            if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(source)) {
                selection.getSpecialSelection().applySpecialSelectionToElement(source);
                if(ASPx.Browser.IE && this.owner.isInFocus)
                    this.setPreventEventFlag();
                ASPx.Evt.PreventEvent(evt);
                if(!this.owner.isInFocus)
                    this.owner.focus();
            }
            else if(this.owner.isInFocus && doc.getElementById(ASPx.HtmlEditorClasses.SelectedHiddenContainerID)) {
                selection.getSpecialSelection().removeHiddenContainer();
                if(ASPx.Browser.IE)
                    this.setPreventEventFlag();
            }
            if(ASPx.Browser.WebKitFamily) {
                var source = ASPx.Evt.GetEventSource(evt);
                if(source && source.tagName == "IMG")
                    ASPxClientHtmlEditorSelection.SelectElement(source, this.owner.getWindow());
            }
            if(needRestoreScrollPosition)
                this.owner.restoreScrollPosition();
            this.isMouseDown = true;
        },
        onTouchMouseUp: function() {
            if(!this.owner.isInFocus) {
                setTimeout(function() {
                    this.owner.focus();
                }.aspxBind(this), 0);
            }
        },
        selectionChange: function(){
            if(ASPx.Browser.WebKitTouchUI) {
                if(ASPx.HtmlEditorClasses.Utils.HasUnforcedFunction("selectionChange"))
                    return;
                var selection = this.owner.getSelection();
                var compare = function(el) { return ASPx.ElementHasCssClass(el, ASPx.HtmlEditorClasses.PlaceholderCssClasseName); };
                var startSelectedContainer = selection.clientSelection.GetStartContainer();
                if(startSelectedContainer.nodeType == 3)
                    startSelectedContainer = startSelectedContainer.parentNode;
                var needToUseSpecialSelection = ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(startSelectedContainer) || startSelectedContainer.id == ASPx.HtmlEditorClasses.SelectedHiddenContainerID || ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(startSelectedContainer, compare);
                var specialElement = selection.clientSelection.specialSelection.getSelectedElement();
                if(!specialElement && needToUseSpecialSelection) {
                    selection.getSpecialSelection().applySpecialSelectionToElement(ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(startSelectedContainer, compare));
                    ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() { }, "selectionChange", 300, true);
                }
                else if(specialElement) {
                    var el = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(startSelectedContainer, compare);
                    if(needToUseSpecialSelection && specialElement != el)
                        selection.getSpecialSelection().applySpecialSelectionToElement(el);
                    else
                        selection.getSpecialSelection().removeHiddenContainer();
                }
            }
            if(this.owner.isInFocus)
                this.onSelectionChanged(true);
        },
        onMouseUp: function(evt) {
            if(!this.owner.enabled) return;
            var preventLostFocus;
            if(ASPx.Browser.IE) {
                var selectedElement = this.owner.getSelection().GetSelectedElement();
                preventLostFocus = selectedElement.nodeName == "P" && selectedElement.style["width"];
            }
            var source = ASPx.Evt.GetEventSource(evt);
            if(ASPx.Browser.IE && ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(source)) {
                setTimeout(function() {
                    this.owner.getSelection().clientSelection.applySpecialSelectionToElement(source);
                }.aspxBind(this), 0);
                ASPx.Evt.PreventEvent(evt);
            }
            if(this.shouldLostFocusBePrevented(evt, false) || preventLostFocus) {
                this.setPreventEventFlag();
                if(!preventLostFocus)
                    return ASPx.Browser.WebKitFamily ? false : ASPx.Evt.PreventEventAndBubble(evt);
            }
            var typeCommandProcessor = this.owner.commandManager.typeCommandProcessor;
            if(typeCommandProcessor.hasStyleCommand()) {
                if(!this.isUpdateLocked())
                    typeCommandProcessor.clearCommands();
                this.setLockUpdate(false);
            }
            var hidePasteOptionsBar = !source || source && source.tagName != "HTML";
            if((ASPx.Browser.IE && evt.button != 2) || (ASPx.Browser.WebKitFamily && ASPx.Evt.IsLeftButtonPressed(evt))) //B187756
                window.setTimeout(function() { 
                    this.onSelectionChanged(hidePasteOptionsBar); 
                }.aspxBind(this), (ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 9) ? 100 : 0); // Q336995
            else
                this.onSelectionChanged(hidePasteOptionsBar);
            if(ASPx.Browser.IE && !this.owner.settings.disableBrowserSpellCheck && this.owner.settings.allowContextMenu != "default")
                this.owner.getBody().spellcheck = true;
            this.isMouseDown = false;
        },
        onPaste: function (e) {
            var commandManager = this.owner.commandManager;
            if (ASPx.Browser.IE) {
                if(!this.owner.getDocument().getElementById(ASPx.HtmlEditorClasses.PasteCatcherID)) {
                    _aspxPreventEvent(e);
                    commandManager.executeCommand(ASPxClientCommandConsts.PASTE_COMMAND);
                    return;
                }
                else
                    ASPx.HtmlEditorClasses.Utils.ClearUnforcedFunctionByKey("RemovePasteContainer");
            }
            if (ASPx.Browser.WebKitTouchUI) {
                setTimeout(function () {
                    commandManager.executeCommand(ASPxClientCommandConsts.KBPASTE_COMMAND);
                    this.onHtmlChanged(true);
                }.aspxBind(this), 0);
            }
            if(!ASPx.Browser.IE && (this.owner.settings.enablePasteOptions || this.owner.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.MergeFormatting))
                commandManager.getCommand(ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND).initStyleStateArray(this.owner);
            if(this.owner.settings.enablePasteOptions || this.owner.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.PlainText) {
                var text;
                if (e.clipboardData)
                    text = e.clipboardData.getData("text/plain");
                else if (window.clipboardData)
                    text = window.clipboardData.getData("Text");
                commandManager.getCommand(ASPxClientCommandConsts.PASTEHTMLPLAINTEXT_COMMAND).initPasteContent(this.owner, text);
            }
            if (e && e.clipboardData && e.clipboardData.getData) {
                if (ASPx.Data.ArrayIndexOf(e.clipboardData.types, "text/html") > -1) {
                    this.pasteToWrapper(this.owner, e.clipboardData.getData('text/html'));
                    e.stopImmediatePropagation();
                    e.preventDefault();
                    return;
                }
                if (/Files/.test(e.clipboardData.types) && this.owner.settings.pasteMode != ASPx.HtmlEditorClasses.PasteMode.PlainText) {
                    this.pasteImage(e);
                    return;
                }
            }
            this.pasteToContainer();
        },
        pasteToWrapper: function(wrapper, html) {
            wrapper.settings.allowPlaceholderReplacing = false;
            html = this.processHtmlBodyBeforeInsert(html, wrapper);
            html = wrapper.placeholderManager.replacePlaceholderElementToTextLabel(html);
            if(wrapper.settings.enablePasteOptions || wrapper.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.SourceFormatting)
                wrapper.commandManager.getCommand(ASPxClientCommandConsts.PASTEHTMLSOURCEFORMATTING_COMMAND).initPasteContent(wrapper, html);
            if(wrapper.settings.enablePasteOptions || wrapper.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.MergeFormatting)
                wrapper.commandManager.getCommand(ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND).initPasteContent(wrapper, html);
            wrapper.settings.allowPlaceholderReplacing = true;
            if(wrapper.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.SourceFormatting)
                this.executeCommand(ASPxClientCommandConsts.PASTEHTMLSOURCEFORMATTING_COMMAND);
            else if(wrapper.settings.pasteMode == ASPx.HtmlEditorClasses.PasteMode.PlainText)
                this.executeCommand(ASPxClientCommandConsts.PASTEHTMLPLAINTEXT_COMMAND);
            else
                this.executeCommand(ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND);
            if(!wrapper.settings.enablePasteOptions)
                wrapper.commandManager.clearPasteOptions(false);
        },
        pasteImage: function (e) {
            var items = e.clipboardData.items,
                blob;
            for (var i = 0; i < items.length; i++) {
                if (items[i].type.indexOf("image") === 0)
                    blob = items[i].getAsFile();
            }
            if (blob !== null) {
                var view = this.owner,
                    doc = view.getDocument(),
                    reader = new FileReader(),
                    image = doc.createElement("IMG");

                var imageId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                image.id = imageId;
                var html = view.raiseBeforePaste(ASPxClientCommandConsts.PASTEHTML_COMMAND, image.outerHTML);
                view.commandManager.executeCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, html);
                reader.onload = function (event) {
                        image = doc.getElementById(imageId),
                        result = event.target.result;

                    image.src = result;
                    image.id = "";
                };
                reader.readAsDataURL(blob);
            }
        },
        pasteToContainer: function() {
            var view = this.owner,
                doc = view.getDocument();
            var pasteCallback = function () {
                var element = doc.getElementById(ASPx.HtmlEditorClasses.PasteCatcherID);
                if(ASPx.Browser.WebKitFamily) {
                    for(var i = 0, child; child = element.childNodes[i]; i++) { 
                        if(child.nodeType == 1 && child.id == ASPx.HtmlEditorClasses.PasteCatcherID) {
                            ASPx.HtmlEditorClasses.Utils.clearPasteContainerStyle(child);
                            child.id = "";
                        }
                    }
                }
                var html = element.innerHTML;
                element.parentNode.removeChild(element);
                if(ASPx.Browser.IE) {
                    this.setPreventEventFlag();
                    view.getWindow().focus();
                }
                if(view.restoreFocus)
                    view.getSelection().clientSelection.SelectExtendedBookmark(view.restoreFocus);
                view.restoreFocus = undefined;
                if(ASPx.Str.Trim(html.replace(/<img[^>]*>/gi, "")))
                    this.pasteToWrapper(view, html);
                else if(view.settings.pasteMode != ASPx.HtmlEditorClasses.PasteMode.PlainText) {
                    html = view.raiseBeforePaste(ASPxClientCommandConsts.PASTEHTML_COMMAND, html);
                    view.commandManager.executeCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, html);
                    view.commandManager.clearPasteOptions(false);
                }
            }.aspxBind(this);
            setTimeout(pasteCallback, 0);

            var container = doc.getElementById(ASPx.HtmlEditorClasses.PasteCatcherID);
            if(!container)
                ASPx.HtmlEditorClasses.Utils.createPasteContainer(view);
        },
        processHtmlBodyBeforeInsert: function(html, wrapper) {
            var styleContents = html.match(/<style[^>]*>[\s\S]*?<\/style>/gi);
            var headHtml = "";
            if(ASPx.Browser.WebKitFamily && styleContents && styleContents.length > 0) {
                for(var i = 0, item; item = styleContents[i]; i++)
                    headHtml += item;
            }
            var pasteFragment = html.match(/<!--StartFragment-->[\s\S]*?<!--EndFragment-->/gi);
            html = pasteFragment && pasteFragment.length > 0 ? pasteFragment[0] : html;
            html = html.replace(/<span\s*class=["']Apple-converted-space["']>&nbsp;<\/span>/gi, "&nbsp;");
            html = html.replace(/<br\s*class=["']Apple-interchange-newline["']>/gi, "");
            
            if(headHtml)
                html = ASPx.HtmlEditorClasses.HtmlProcessor.convertStyleElementToInlineStyleElementByHtml(headHtml, html, wrapper.getDocument(), false);
            html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearWordFormatting(html, false, wrapper.getDocument(), false);
            
            html = wrapper.processHtmlBodyBeforeInsert(html);
            if(!ASPx.Browser.IE || ASPx.Browser.MajorVersion > 8)
                html = ASPx.HtmlEditorClasses.HtmlProcessor.clearExcessStyle(wrapper, html);
            html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearEmptySpans(html);
            return html;
        },

        /*region* * * * * * * * * * * * * * * * * *  Dragging  * * * * * * * * * * * * * * * * * */
        clearDragDropTimer: function() {
            if(this.dragDropTimerID)
                ASPx.Timer.ClearTimer(this.dragDropTimerID);
        },
        onAfterObjectDragEndWithDelay: function() {
            this.dragDropTimerID = window.setTimeout(function() { this.OnAfterObjectDragEnd(); }.aspxBind(this), dragDropDelay);
        },
        onAfterDocumentObjectDragEndCallWithDelay: function() {
            this.dragDropTimerID = window.setTimeout(function() { return this.onAfterDocumentObjectDragEnd(); }.aspxBind(this), dragDropDelay);
        },
        onAfterDocumentObjectDragEnd: function() {
            this.isDropExternalContentExecuted = false;
            this.onHtmlChanged(true);
            this.clearDragDropTimer();
        },
        OnAfterObjectDragEnd: function() {
            this.onHtmlChanged(true);
            this.clearDragDropTimer();
        },
        onDocumentObjectDragEnd: function() {
            if(this.isDropExternalContentExecuted)
                this.onAfterDocumentObjectDragEnd();
        },
        onObjectDragStart: function(evt) {
            if(!this.isDraggingInsideEditor && !ASPx.HtmlEditorClasses.IsDocumentDragOver /*firefox only*/) {
                this.isDraggingInsideEditor = true;
                this.owner.commandManager.executeCommand(ASPxClientCommandConsts.DRAGDROPOBJECT_COMMAND, null);
            }
        },
        onObjectDrop: function(evt) {
            if(!this.isDraggingInsideEditor) {
                this.isDropExternalContentExecuted = true;
                this.owner.commandManager.executeCommand(ASPxClientCommandConsts.DROPOBJECTFROMEXTERNAL_COMMAND, null);
                if(ASPx.HtmlEditorClasses.IsDocumentDragOver) { // for FireFox Only
                    ASPx.HtmlEditorClasses.IsDocumentDragOver = false;
                    this.onAfterDocumentObjectDragEndCallWithDelay();
                }
            }
            if(ASPx.Browser.NetscapeFamily)
                this.onObjectDragEnd();
            else if(!this.isDraggingInsideEditor)
                this.onDocumentObjectDragEnd();
        },
        onObjectDragEnd: function() {
            if(this.isDraggingInsideEditor) {
                this.isDraggingInsideEditor = false;
                this.onAfterObjectDragEndWithDelay();
            }
        },
        onObjectResizeStart: function (evt) {
            this.owner.commandManager.executeCommand(ASPxClientCommandConsts.RESIZEOBJECT_COMMAND, null);
        },
        onObjectResizeEnd: function (evt) {
            this.onHtmlChanged(true);
        }

    });
})();