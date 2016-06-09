
(function() {
    ASPx.HtmlEditorClasses.Managers.SelectionManager = ASPx.CreateClass(null, {
        constructor: function(iframeWrapper) {
            this.iframeWrapper = iframeWrapper;
            this.inRestoreSelectionProcess = false;
            this.isSelectionRestored = false;
            this.selection = null;
            this.beforePopupSelection = null;
            this.selectionBookmark = null;
            // table command updating optimization
            this.cachedElementsManager = new ASPx.HtmlEditorClasses.Managers.CachedElementsManager();
        },
        createSelection: function() {
            var selection = new ASPxClientHtmlEditorSelection(this.iframeWrapper, this);
            var selectedElement = selection.GetSelectedElement();
            if(!selectedElement || selectedElement.nodeName == "HTML") {
                this.restoreSelection();
                selection = new ASPxClientHtmlEditorSelection(this.iframeWrapper, this);
            }
            return selection;
        },
        restoreSelection: function() {
            if((ASPx.Browser.IE && this.inRestoreSelectionProcess) || (ASPx.Browser.Opera && this.iframeWrapper.isInFocus))
                return true;
            if(this.preventSelectionProcessing())
                return false;
            this.isSelectionRestored = true;
            this.iframeWrapper.focus();
            var doc = this.iframeWrapper.getDocument();
            var hiddenElement = doc.getElementById(ASPx.HtmlEditorClasses.SelectedHiddenContainerID);
            if(hiddenElement) {
                var selection = new ASPxClientHtmlEditorSelection(this.iframeWrapper, this);
                selection.getSpecialSelection().restoreSpecialSelection();
            }
            else {
                this.inRestoreSelectionProcess = true; // Q336932
                setTimeout(function() {
                    this.inRestoreSelectionProcess = false;
                }.aspxBind(this), 100);
                try {
                    this.setSelection(this.selection);
                    return true;
                }
                catch(e) {
                    return false;
                }
            }
        },
        saveSelection: function() { 
            try {
                if(!this.selection)
                    this.isSelectionRestored = true;
                var selection = this.createSelection();
                if(ASPx.Browser.WebKitFamily && !selection.GetSelectedElement()) {
                    this.selection = null;
                    return;
                }
                else if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8) { // B185720
                    if(!this.preventSelectionProcessing(selection))
                        this.selection = selection;
                }
                else
                    this.selection = selection;
            } catch(e) { }
        },
        preventSelectionProcessing: function(selection) { // B185720, Q351325
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8) {
                selection = !selection ? this.createSelection() : selection;
                var element = selection.GetSelectedElement();
                while(element && element.ownerDocument == this.iframeWrapper.getDocument() && element.tagName != "BODY") {
                    if(element.tagName == "DIV") {
                        var divStyle = ASPx.GetCurrentStyle(element);
                        if(divStyle.float != "none" || divStyle.width != "auto" || divStyle.height != "auto" || divStyle.overflow == "hidden")
                            return true;
                    }
                    element = element.parentNode;
                }
            }
            return false;
        },

        setSelection: function(selection) {
            if(ASPx.Browser.NetscapeFamily)
                this.iframeWrapper.focus();
            if(selection)
                selection.Apply();
        },

        GetSelectedElement: function(name) {
            return this.cachedElementsManager.GetSeletedElement(name);
        },
        SetSelectedElement: function(name, element) {
            return this.cachedElementsManager.SetSelectedElement(name, element);
        },
        restoreLastSelection: function(selectionObj) {
            if(selectionObj != null) {
                this.iframeWrapper.focus();
                selectionObj.Restore();
            }
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9)
                this.saveSelection();
        },
        saveLastSelection: function() {
            var selectionObj = ASPxClientHtmlEditorSelection.Create(this.iframeWrapper.getWindow());
            selectionObj.Save();
            return selectionObj;
        },

        createRestoreSelectionForDialog: function() {
            var selectionObj = ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 ? new ASPx.HtmlEditorClasses.DialogSelectionIE(this.iframeWrapper.getWindow()) :
                                new ASPx.HtmlEditorClasses.SelectionNSOpera(this.iframeWrapper.getWindow());
            selectionObj.Save();
            return selectionObj;
        },

        saveSelectionForPopup: function() {
            this.beforePopupSelection = this.createSelection().clientSelection;
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11)
                this.selectionBookmark = this.beforePopupSelection.GetExtendedBookmark();
            else
                this.beforePopupSelection.Save();
        },
        removeBeforePopupBookmark: function() {
            this.selectionBookmark = this.beforePopupSelection.RemoveExtendedBookmark(this.selectionBookmark);
        },
        restoreSelectionForPopup: function() {
            if(!ASPx.Browser.Opera) {
                if(this.beforePopupSelection) {
                    if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 && this.selectionBookmark) {
                        this.beforePopupSelection.SelectExtendedBookmark(this.selectionBookmark);
                        this.selectionBookmark = null;
                    }
                    else
                        this.beforePopupSelection.Restore();
                    this.beforePopupSelection = null;
                }
            }
        },
        saveSelectionByBookmark: function(cm) {
            this.restoreSelection();
            var selection = this.createSelection();
            if(selection.GetSelectedElement()) {
                var restoreHtml = this.iframeWrapper.getRawHtml();
                var selectedHtml = selection.GetHtml();
                var doc = this.iframeWrapper.getDocument();
                var startEl, endEl;
                if(!selectedHtml) {
                    startEl = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
                    endEl = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
                    this.iframeWrapper.commandManager.executeCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, startEl.outerHTML + endEl.outerHTML, false);
                    startEl = doc.getElementById(startEl.id);
                    endEl = doc.getElementById(endEl.id);
                }
                else {
                    var bm = selection.clientSelection.GetExtendedBookmark();
                    startEl = doc.getElementById(bm.startMarkerID);
                    endEl = doc.getElementById(bm.endMarkerID);
                    var moveBmElementToChild = function(bmElement, getSiblingNode, insertChildNode) {
                        bmElement = ASPx.ReplaceTagName(bmElement, "span");
                        var siblingNode = getSiblingNode(bmElement);
                        if(siblingNode && /^(table|tbody|tfoot|thead|tr)$/i.test(siblingNode.nodeName)) {
                            var childNodes = ASPx.GetNodesByTagName(siblingNode, "TD");
                            if(childNodes.length > 0)
                                insertChildNode(bmElement, childNodes);
                        }
                    }
                    moveBmElementToChild(startEl, function(el) { return el.nextSibling; }, function(elem, childNodes) { return childNodes[0].insertBefore(elem, childNodes[0].firstChild); });
                    moveBmElementToChild(endEl, function(el) { return el.previousSibling; }, function(elem, childNodes) { return ASPx.InsertElementAfter(elem, childNodes[childNodes.length - 1].lastChild); });
                }

                var bodyContent = this.iframeWrapper.getHtml();
                if(startEl)
                    bodyContent = bodyContent.replace(new RegExp("<span[^>]*" + startEl.id + "[^>]*><\\/span>", "gi"), ASPx.HtmlEditorClasses.StartSelectionPosMarkName);
                if(endEl)
                    bodyContent = bodyContent.replace(new RegExp("<span[^>]*" + endEl.id + "[^>]*><\\/span>", "gi"), ASPx.HtmlEditorClasses.EndSelectionPosMarkName);
                this.selection = null;
                this.iframeWrapper.setInnerHtmlToBody(restoreHtml);
                this.createSelection().SetFocusToDocumentStart();
                if(this.iframeWrapper.settings.allowEditFullDocument)
                    bodyContent = ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent(bodyContent);
                bodyContent = ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent(bodyContent);
                var safeHtmlObject = ASPx.HtmlEditorClasses.HtmlProcessor.safeHtml(bodyContent);
            }
            return { safeHtmlObject: safeHtmlObject, startTagName: null, endTagName: null };
        },
        restoreBookmark: function(savedSelectionByBookmarkObject, html) {
            var getTagName = function(tagName) { return !tagName || !/^(tbody|tfoot|thead|tr|td)$/i.test(tagName) ? "span" : tagName; };
            var getTagHtml = function(tagName, id) { return "<" + getTagName(tagName) + " style='display: none;' id='" + id + "'></" + getTagName(tagName) + ">" };
            
            var bodyContent = this.iframeWrapper.settings.allowEditFullDocument ? ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent(html) : html;
            bodyContent = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.restoreMarkPositionBySafeHtml(savedSelectionByBookmarkObject.safeHtmlObject, bodyContent);
            bodyContent = bodyContent.replace(ASPx.HtmlEditorClasses.StartSelectionPosMarkName, getTagHtml(savedSelectionByBookmarkObject.startTagName, ASPx.HtmlEditorClasses.StartSelectionPosMarkID));
            bodyContent = bodyContent.replace(ASPx.HtmlEditorClasses.EndSelectionPosMarkName, getTagHtml(savedSelectionByBookmarkObject.endTagName, ASPx.HtmlEditorClasses.EndSelectionPosMarkID));
            return this.iframeWrapper.settings.allowEditFullDocument ? ASPx.HtmlEditorClasses.HtmlProcessor.replaceBodyContent(html, bodyContent) : bodyContent;
        },
        restoreSelectionByBookmark: function(doc, startMarkerID, endMarkerID) {
            var moveBmElementToChild = function(bmElement, getSiblingNode, insertChildNode) {
                bmElement = ASPx.ReplaceTagName(bmElement, "span");
                var siblingNode = getSiblingNode(bmElement);
                if(siblingNode && /^(table|tbody|tfoot|thead|tr)$/i.test(siblingNode.nodeName)) {
                    var childNodes = ASPx.GetNodesByTagName(siblingNode, "TD");
                    if(childNodes.length > 0)
                        insertChildNode(bmElement, childNodes);
                }
            }
            var startEl = doc.getElementById(startMarkerID),
                endEl = doc.getElementById(endMarkerID);
            if(startEl && endEl) {
                if(startEl.nextSibling == endEl) {
                    startEl.id = endMarkerID;
                    endEl.id = startMarkerID;
                }
                else {
                    moveBmElementToChild(startEl, function(el) { return el.nextSibling; }, function(elem, childNodes) { return childNodes[0].insertBefore(elem, childNodes[0].firstChild); });
                    moveBmElementToChild(endEl, function(el) { return el.previousSibling; }, function(elem, childNodes) { return ASPx.InsertElementAfter(elem, childNodes[childNodes.length - 1].lastChild); });
                }
                this.iframeWrapper.resetScrollPosAfterInsertHtml = false;
                if(!endEl.parentNode)
                    endEl = doc.getElementById(endMarkerID);
                endEl.style.display = "";
                endEl.innerHTML = "&nbsp;"
                ASPx.HtmlEditorClasses.scrollHelper.scrollTo(endEl, 20, ASPx.HtmlEditorClasses.DisplayPositon.Bottom);
                endEl.innerHTML = "";
                this.createSelection().clientSelection.SelectExtendedBookmark({ "startMarkerID": startMarkerID, "endMarkerID": endMarkerID});
                this.saveSelection();
            }
        }
    });

    ASPx.HtmlEditorClasses.Managers.HtmlViewCMSelectionManager = ASPx.CreateClass(null, {
        constructor: function(wrapper) {
            this.wrapper = wrapper;
            this.codeMirror = wrapper.getSourceEditor();
        },
        getHtml: function() {
            return this.codeMirror.getSelection();
        },
        setHtml: function(html) {
            this.codeMirror.replaceSelection(html);
        },
        getSelectedTag: function() {
            var range = this.getRange();
            var matchingTag;
            if(this.isCollapsed())
                matchingTag = this.getMatchingTag(range.from());
            return !matchingTag ? CodeMirror.findEnclosingTag(this.codeMirror, range.from()) : matchingTag;
        },
        isCollapsed: function() {
            return !this.codeMirror.somethingSelected();
        },
        isScriptBlockSelected: function() {
            var tag = this.getSelectedTag();
            return tag ? tag.open.tag == "script" : false;
        },
        getMatchingTag: function(pos) {
            if(!pos)
                pos = this.getCursorPos();
            return CodeMirror.findMatchingTag(this.codeMirror, pos);
        },
        setSelection: function(from, to) {
            this.codeMirror.setSelection(from, to);
        },
        getRange: function() {
            return this.codeMirror.listSelections()[0];
        },
        getCursorPos: function() {
            return this.isCollapsed() ? this.codeMirror.getCursor() : null;
        },
        getCurrentToken: function() {
            var pos = this.getCursorPos();
            return pos ? this.codeMirror.getTokenAt(pos) : null;
        },
        getPositionState: function() {
            if(!this.isCollapsed())
                return null;
            var token = this.getCurrentToken(),
                state = CodeMirror.innerMode(this.codeMirror.getMode(), token.state).state;
            var result = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.getTokenType(state, token.type, token.string);
            if(token.string == "</")
                return "";
            else if(result == ASPx.HtmlEditorClasses.TokenType.TagName) {
                var matchingTag = this.getMatchingTag();
                return matchingTag && matchingTag.at == "close" ? "" : result;
            }
            return result;
        },

        saveSelectionByBookmark: function() {
            var html = this.wrapper.settings.allowEditFullDocument ? ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent(this.wrapper.getRawHtml()) : this.wrapper.getRawHtml();
            if(!html)
                return null;
            this.wrapper.eventManager.detachHtmlChangedEventToEditor();
            var marksPos = this.setBookmarksBySelection();
            html = this.wrapper.settings.allowEditFullDocument ? ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent(this.wrapper.getRawHtml()) : this.wrapper.getRawHtml();
            var safeHtmlObject;
            if(marksPos) {
                if(html.indexOf(ASPx.HtmlEditorClasses.StartSelectionPosMarkName) > -1 && html.indexOf(ASPx.HtmlEditorClasses.EndSelectionPosMarkName) > -1) {
                    html = ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent(html);
                    safeHtmlObject = ASPx.HtmlEditorClasses.HtmlProcessor.safeHtml(html);
                }
                this.removeMark(this.codeMirror, marksPos.start.pos, marksPos.end.pos);
                this.wrapper.eventManager.attachHtmlChangedEventToEditor();
                return { safeHtmlObject: safeHtmlObject, startTagName: marksPos.start.tagName, endTagName: marksPos.end.tagName };
            }
            return null;
        },
        removeMark: function(cm, startPos, endPos) {
            cm.replaceRange("", endPos, CodeMirror.Pos(endPos.line, endPos.ch + ASPx.HtmlEditorClasses.EndSelectionPosMarkName.length));
            cm.replaceRange("", startPos, CodeMirror.Pos(startPos.line, startPos.ch + ASPx.HtmlEditorClasses.StartSelectionPosMarkName.length));
            if(startPos.line == endPos.line)
                endPos.ch -= ASPx.HtmlEditorClasses.StartSelectionPosMarkName.length;
        },
        restoreSelectionByBookmark: function(startMarkerID, endMarkerID) {
            this.wrapper.eventManager.detachHtmlChangedEventToEditor();
            var lineCount = this.codeMirror.lineCount();
            var startPos, endPos;
            for(var i = 0; i < lineCount; i++) {
                var tokens = this.codeMirror.getLineTokens(i);
                for(var j = 0, token; token = tokens[j]; j++) {
                    if(token.string.indexOf(startMarkerID) > -1)
                        startPos = CodeMirror.Pos(i, token.start  + token.string.indexOf(startMarkerID));
                    if(token.string.indexOf(endMarkerID) > -1) {
                        endPos = CodeMirror.Pos(i, token.start  + token.string.indexOf(endMarkerID));
                        this.removeMark(this.codeMirror, startPos, endPos);
                        this.codeMirror.setSelection(startPos, endPos);
                        this.codeMirror.clearHistory();
                    }
                }
            }
            this.wrapper.eventManager.attachHtmlChangedEventToEditor();
        },
        restoreBookmark: function(savedSelectionByBookmarkObject, html) {
            var bodyContent = this.wrapper.settings.allowEditFullDocument ? ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent(html) : html;
            bodyContent = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.restoreMarkPositionBySafeHtml(savedSelectionByBookmarkObject.safeHtmlObject, bodyContent);
            return this.wrapper.settings.allowEditFullDocument ? ASPx.HtmlEditorClasses.HtmlProcessor.replaceBodyContent(html, bodyContent) : bodyContent;
        },
        setBookmarksBySelection: function() {
            var startPos, endPos;
            var startTagName, endTagName;
            ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.correctSelection(this.wrapper);
            var pos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.getPositionToBookmarks(this.wrapper);
            if(pos.start.pos && pos.end.pos) {
                this.codeMirror.setCursor(pos.start.pos);
                this.wrapper.pasteHtml(ASPx.HtmlEditorClasses.StartSelectionPosMarkName);
                if(pos.start.pos.line == pos.end.pos.line)
                    pos.end.pos.ch += ASPx.HtmlEditorClasses.StartSelectionPosMarkName.length;

                this.codeMirror.setCursor(pos.end.pos);
                this.wrapper.pasteHtml(ASPx.HtmlEditorClasses.EndSelectionPosMarkName);

                return pos;
            }
            return null;
        }
    });
})();