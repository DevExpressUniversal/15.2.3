(function () {
    var F_KEY_CODE = 70,
        SEARCH_FIELD_INDICATOR_BUTTON_INDEX = 0;

    var SystemPopupConsts = {
        QuickSearchWindowName: "quickSearch",
        AdvanceSearchWindowName: "advancedSearch",
        IntelliSenseWindowName: "intelliSense",
        IntelliSenseListBoxId: "ListBox",
        SearchButtonEditId: "SrchBtnEdit",
        AdvancedSearchDialogButtonId: "advancedSearchDlgBtn",
        NextButtonId: "NextButton",
        PrevButtonId: "PrevButton",
        WinCssPrefix: "dxhe-",
        WinCssSuffix: "SysWin"
    };

    var ManagerBase = ASPx.CreateClass(null, {
        constructor: function(popup) {
            this.popup = popup;
            this.afterCreate();
            this.setWindowCssClassName();
        },
        setWindowCssClassName: function() {
            ASPx.AddClassNameToElement(this.popup.GetWindowElement(this.getWindow().index), SystemPopupConsts.WinCssPrefix + this.getWindowName() + SystemPopupConsts.WinCssSuffix);        
        },
        initialize: function() { },
        clear: function() { },
        updateResults: function() { },
        afterCreate: function() { },
        getWindowName: function() { },
        hide: function() {
            this.popup.HideWindow(this.getWindow());
        },
        getWindow: function() {
            return this.popup.GetWindowByName(this.getWindowName());   
        },
        getControl: function (id) {
            var id = this.popup.htmlEditor.name + "_" + this.getWindowName() + "_" + id;
            if(!this[id])
                this[id] = ASPxClientControl.GetControlCollection().GetByName(id);
            return this[id];
        }
    });
    
    var equalCharCode = 187;
    var ltChar = 188;
    var listBoxRowCount = 7;
    var listBoxPageKeyStepValue = 9;

    var IntelliSenseWindowManager = ASPx.CreateClass(ManagerBase, {
	    afterCreate: function() {
            var htmlEditor = this.popup.htmlEditor;
	        this.htmlEditor = htmlEditor;
            this.core = htmlEditor.core;
            this.wrapper = htmlEditor.getHtmlViewWrapper();
            this.intelliSenseManager = new ASPx.HtmlEditorClasses.Controls.IntelliSenseManager(this.wrapper);

            var listBox = this.getListBoxControl();
            listBox.ItemDoubleClick.AddHandler(this.onItemDoubleClick.aspxBind(this));
            listBox.GotFocus.AddHandler(this.setFocusToSourceEditor.aspxBind(this));
            
            var sourceEditor = this.wrapper.getSourceEditor();
            sourceEditor.on("keydown", this.onSourceEditorKeyDown.aspxBind(this));
            sourceEditor.on("keyup", this.onSourceEditorKeyUp.aspxBind(this));
        },
        getPopupControl: function() {
            return this.popup;    
        },
        getWindowName: function() { 
            return SystemPopupConsts.IntelliSenseWindowName;
        },
        getListBoxControl: function() {
            return this.getControl(SystemPopupConsts.IntelliSenseListBoxId);
        },
        initialize: function() {
            if(!this.wrapper.selectionManager.isCollapsed())
                return;
            var items = this.intelliSenseManager.getItems();
            this.saveToken = this.wrapper.selectionManager.getCurrentToken();
            if(!items || items.length == 0)
                this.hide();
            else if(items.length == 1 && !this.isIntelliSenseWindowVisible())
                this.insertSelectedItem(items[0].name);
            else {
                var listBox = this.getListBoxControl();
                listBox.ClearItems();
                listBox.BeginUpdate();
                for(var i = 0, element; element = items[i]; i++)
                    this.addListBoxItem(listBox, element, this.getIconClassNameByType(element.iconType));
                listBox.EndUpdate();
                listBox.SetSelectedIndex(0);
                var visibleElementCount = items.length > listBoxRowCount ? listBoxRowCount : items.length;
                listBox.SetHeight(visibleElementCount * listBox.GetItemHeight(0) + (listBox.GetMainElement().offsetHeight - listBox.GetScrollDivElement().offsetHeight));

                if(!this.isIntelliSenseWindowVisible()) {
                    var pos = this.wrapper.getSourceEditor().cursorCoords();
                    this.getPopupControl().ShowWindowAtPos(this.getWindow(), pos.left, pos.top + 15);
                    this.wrapper.isIntelliSenseWindowShown = true;
                }
            }
        },
        clear: function() {
            this.wrapper.isIntelliSenseWindowShown = false; 
        },
        isIntelliSenseWindowVisible: function() {
            if(this.wrapper.isIntelliSenseWindowShown && !this.getListBoxControl().IsVisible())
                this.wrapper.isIntelliSenseWindowShown = false;
            return this.wrapper.isIntelliSenseWindowShown;
        },
        selectItemByIndex: function(index) {
            var listBox = this.getListBoxControl();
            var itemCount = listBox.GetItemCount();
            if(index < 0)
                index = 0;
            else if(index > itemCount - 1)
                index = itemCount - 1;
            listBox.SetSelectedIndex(index);
        },
        insertSelectedItem: function(value) {
            var listBox = this.getListBoxControl();
            if(!value)
                value = listBox.GetValue();
            this.intelliSenseManager.insertItem(value);
            this.hide();
            this.setFocusToSourceEditor();
        },
        getSelectedIndex: function() {
            return this.getListBoxControl().GetSelectedIndex();
        },
        setFocusToSourceEditor: function() {
            var sourceEditor = this.wrapper.getSourceEditor();
            if(!sourceEditor.state.focused) {
                setTimeout(function() {
                    sourceEditor.focus();
                }, 0);
            }    
        },
        addListBoxItem: function(listBox, element) {
            var index = listBox.AddItem(element.name, element.name, ASPx.EmptyImageUrl);
            var child = listBox.GetItemElement(index).childNodes[0];    
            if(child.nodeType == 1 && child.nodeName == "IMG")
                ASPx.AddClassNameToElement(child, this.getIconClassNameByType(element.iconType));
        },
        getIconClassNameByType: function(iconType) {
            if(iconType == ASPx.HtmlEditorClasses.IconType.XmlItem)
                return this.wrapper.settings.htmlViewAutoCompletionIcons.xmlItemIconClassName;
            else if(iconType == ASPx.HtmlEditorClasses.IconType.Field)
                return this.wrapper.settings.htmlViewAutoCompletionIcons.fieldIconClassName;
            else if(iconType == ASPx.HtmlEditorClasses.IconType.Enum)
                return this.wrapper.settings.htmlViewAutoCompletionIcons.enumIconClassName;
            else if(iconType == ASPx.HtmlEditorClasses.IconType.Event)
                return this.wrapper.settings.htmlViewAutoCompletionIcons.eventIconClassName;
        },
        isEditingTokenChenged: function() {
            var token = this.wrapper.selectionManager.getCurrentToken();
            if(!this.saveToken || !ASPx.Str.Trim(this.saveToken.string) && !ASPx.Str.Trim(token.string))
                return false;
            return this.saveToken.string != token.string || this.saveToken.start != token.start || this.saveToken.end != token.end;
        },
        isControlKey: function(keyCode) {
            return keyCode == ASPx.Key.Down || keyCode == ASPx.Key.Up || keyCode == ASPx.Key.Enter || keyCode == ASPx.Key.Tab || keyCode == ASPx.Key.PageDown || keyCode == ASPx.Key.PageUp;
        },
        needHideOnKeyDown: function(keyCode) {
            return keyCode == ASPx.Key.Home || keyCode == ASPx.Key.End || keyCode == ASPx.Key.Insert || (keyCode >= ASPx.Key.F1 && keyCode <= ASPx.Key.F12);
        },
        selectSiblingItem: function(offset) {
            var listBox = this.getListBoxControl();
            var itemCount = listBox.GetItemCount();
            var index = listBox.GetSelectedIndex();
            var newIndex = Math.min(itemCount - 1, Math.max(0, index + offset));
            if(!listBox.IsItemVisible(newIndex) && offset > 0) {
                listBox.SetSelectedIndex(newIndex);
                listBox.ScrollToItemVisible(Math.max(0, newIndex - (listBoxRowCount - 1)));
            } else
                listBox.SetSelectedIndex(newIndex);
        },

        // Events
        onSourceEditorKeyUp: function(s, evt) {
            if(this.isIntelliSenseWindowVisible()) {
                var keyCode = ASPx.Evt.GetKeyCode(evt);
                if((keyCode == ASPx.Key.Left || keyCode == ASPx.Key.Right || keyCode == ASPx.Key.Space) && this.isEditingTokenChenged())
                    this.hide();
                else if(!this.isControlKey(keyCode))
                    this.initialize();
            }
        },
        onSourceEditorKeyDown: function(s, evt) {
            if(this.isIntelliSenseWindowVisible()) {
                var keyCode = ASPx.Evt.GetKeyCode(evt);
                if(keyCode == ASPx.Key.Down)
                    this.selectSiblingItem(1);
                else if(keyCode == ASPx.Key.Up)
                    this.selectSiblingItem(-1);
                else if(keyCode == ASPx.Key.Enter || keyCode == ASPx.Key.Tab)
                    this.insertSelectedItem();
                else if(keyCode == ASPx.Key.PageDown)
                    this.selectItemByIndex(this.getSelectedIndex() + listBoxPageKeyStepValue);
                else if(keyCode == ASPx.Key.PageUp)
                    this.selectItemByIndex(this.getSelectedIndex() - listBoxPageKeyStepValue);
                else {
                    var curToken = this.wrapper.selectionManager.getCurrentToken();
                    if(keyCode == ASPx.Key.Backspace && /^(<|\s*)$/.test(curToken.string) || this.needHideOnKeyDown(keyCode))
                        this.hide();
                }
                if(this.isControlKey(keyCode))
                    ASPx.Evt.PreventEvent(evt);
            }
            return true;
        },
        onItemDoubleClick: function() {
            this.insertSelectedItem();
        }
    });

    var SearchManager = ASPx.CreateClass(ManagerBase, {
        constructor: function(searchPopup) {
            this.constructor.prototype.constructor.call(this, searchPopup);
            this.searchPopup = searchPopup;
            this.currentSearch = null;
            this.attachEvents();
        },
        initialize: function (text) {
            this.setSearchText(ASPx.IsExists(text) ? text : this.getSelectedText());
            this.setFocus();
            this.search();
            var windowIndex = this.searchPopup.GetWindowByName(this.getWindowName()).index;
            this.searchPopup.GetWindowElement(windowIndex).addEventListener("keydown", function (e) { this.onSearchPopupKeyDown(e); }.aspxBind(this));
        },
        searchThrottled: function () {
            ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(this.search.aspxBind(this), "SearchManager.search", 0, true);
        },
        checkShortcut: function (event) {
            var shortcutName = this.getWrapper().getShortcutCommand(event);
            return {
                isQuickSearchShortcut: (shortcutName == ASPxClientCommandConsts.SHOWSEARCHPANEL_COMMAND),
                isAdvanceSearchShortcut: (shortcutName == ASPxClientCommandConsts.FINDANDREPLACE_DIALOG_COMMAND)
            }
        },
        onSearchPopupKeyDown: function (event) {
            var isAdvanceSearchActive = this.getWindowName() === SystemPopupConsts.AdvanceSearchWindowName;
            var shortcutType = this.checkShortcut(event);
            var searchText = this.getSearchText();

            if (shortcutType.isQuickSearchShortcut) {
                if (isAdvanceSearchActive) {
                    this.searchPopup.hideAdvancedSearch();
                    this.searchPopup.showQuickSearch({ selectedText: searchText });
                }
                else {
                    this.getSearchField().SelectAll();
                }
            }
            else
                if (shortcutType.isAdvanceSearchShortcut) {
                    if (isAdvanceSearchActive) {
                        this.getSearchField().SelectAll();
                    }
                    else {
                        this.searchPopup.hideQuickSearch();
                        this.searchPopup.showAdvancedSearch({ selectedText: searchText });
                    }
                }
            if (shortcutType.isAdvanceSearchShortcut || shortcutType.isQuickSearchShortcut) {
                ASPx.Evt.PreventEvent(event);
            }
        },
        setSearchText: function(selectedText) {
            this.getSearchField().SetText(selectedText);    
        },
        getSearchText: function() {
            return this.getSearchField().GetText();    
        },
        setFocus: function() { 
            this.getWrapper().isInFocus = false;
            this.getSearchField().SetFocus();
        },
        getSearchField: function() { 
            return this.getControl(SystemPopupConsts.SearchButtonEditId);
        },
        search: function() {
            if ((ASPx.Browser.IE || ASPx.Browser.Firefox) && this.getWrapper().turnOffSpellChecking) {
                this.getWrapper().turnOffSpellChecking();
            }
        },
        getSelectedText: function() { 
            return this.getWrapper().getSearchSelectedText();    
        },
        attachEvents: function() { 
            var searchField = this.getSearchField();

            // TODO: Hack - can't detect clear click or value changed (editor still in focus)
            var clearFunc = searchField.OnClear.aspxBind(searchField);
            searchField.OnClear = function() {
                clearFunc();
                this.onSearchFieldKeyup(searchField, null);
            }.aspxBind(this);

            searchField.KeyUp.AddHandler(this.onSearchFieldKeyup, this);
            searchField.KeyDown.AddHandler(this.onSearchGroupInputsKeydown, this);
            this.getControl(SystemPopupConsts.PrevButtonId).Click.AddHandler(function(s, e) { this.updateSearchInfo(this.findPrev()); }, this);
            this.getControl(SystemPopupConsts.NextButtonId).Click.AddHandler(function (s, e) { this.updateSearchInfo(this.findNext()); }, this);
            this.attachEventsInternal();
        },
        attachEventsInternal: function() { },
        clear: function() {
            if(this.currentSearch) {
                this.currentSearch.setSelectionAccordingCurrentEntry();
                this.currentSearch.endSearch();
                this.currentSearch = null;
            }
            if ((ASPx.Browser.IE || ASPx.Browser.Firefox) && this.getWrapper().restoreSpellChecking) {
                this.getWrapper().restoreSpellChecking();
            }
        },
        performSearch: function(text) {
            if(this.currentSearch)
                this.currentSearch.endSearch();
            var params = { 
                wrapper: this.getWrapper(), 
                text: text, 
                options: this.getSearchOptions() 
            };
            this.currentSearch = new ASPx.HtmlEditorClasses.Search(params);
            this.currentSearch.beginSearch();
        },
        getCursorPositionBookmarks: function (selection) {
            var doc = selection.focusNode.ownerDocument;
            var textNodeType = 3;
            var focusNode = selection.focusNode;
            var startMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
            var endMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);

            var document = selection.focusNode.ownerDocument;
            if (focusNode.nodeType == textNodeType) {
                var beforeFocusPart = document.createTextNode(focusNode.textContent.substr(0, selection.focusOffset));
                var afterFocusPart = document.createTextNode(focusNode.textContent.substr(selection.focusOffset));
                ASPx.InsertElementAfter(afterFocusPart, focusNode);
                ASPx.InsertElementAfter(beforeFocusPart, focusNode);
                ASPx.RemoveElement(focusNode);
                ASPx.InsertElementAfter(endMarkElement, beforeFocusPart);
                ASPx.InsertElementAfter(startMarkElement, endMarkElement);
                return { "startMarkerID": startMarkElement.id, "endMarkerID": endMarkElement.id };
            }
            return false;
        },
        restoreCursorByBookmarks: function (bookmarks) {
            var selection = this.getWrapper().getWindow().getSelection();
            var bmElements = ASPx.HtmlEditorClasses.Selection.GetBookmarkElements(bookmarks, this.getWrapper().getDocument());
            if (!bmElements.startMarker || !bmElements.endMarker)
                return;
            this.getWrapper().getSelection().clientSelection.SelectExtendedBookmark(bookmarks, true);

            var isSystemSearchBackgroundElement = selection.focusNode.parentNode && ASPx.ElementHasCssClass(selection.focusNode.parentNode, ASPx.HtmlEditorClasses.Search.CssClasses.Highlight);
            if (isSystemSearchBackgroundElement) { // isSystemSearchBackgroundElement display careet so, if careet located at SearchWrapper 0 position 
                var range = this.getWrapper().getDocument().createRange(); 
                var oldFocusNode = selection.focusNode;
                var newFocusNode = this.getNewFocusNode(selection.focusNode);
                var newFocusOffset = (oldFocusNode.parentNode == newFocusNode) ? oldFocusNode.parentNode.textContent.length : 0;

                range.setStart(newFocusNode, newFocusOffset);
                range.setEnd(newFocusNode, newFocusOffset);
                selection.removeAllRanges();
                selection.addRange(range);
            }
        },
        saveCursor: function () {
            try {
                var isHtmlView = this.getWrapper().getName() === ASPx.HtmlEditorClasses.View.Html;
                if (isHtmlView) 
                    return this.getWrapper().getSourceEditor().getCursor();
            
                var selection = this.getWrapper().getWindow().getSelection();
                return {
                    bookmarks: this.getCursorPositionBookmarks(selection)
                }
            } catch(e) { 
                return null;
            }
        },
        restoreCursor: function (cursor) {
            if (cursor.bookmarks)
                this.restoreCursorByBookmarks(cursor.bookmarks);
            else
                if (this.getWrapper().getSourceEditor)
                    this.getWrapper().getSourceEditor().setCursor(cursor);
        },
        updateResults: function (text) {
            if (this.currentSearch) {
                var cursor = this.saveCursor();

                this.currentSearch.endSearch();
                this.currentSearch.beginSearch();
                this.findByIndex(this.currentSearch.currentIndex);
                this.populateSearchResult();
                this.updateSearchInfo(this.currentSearch.currentIndex);

                if(cursor)
                    this.restoreCursor(cursor);
            }
        },
        populateSearchResult: function() { },
        getNewFocusNode: function (el) {
            if (el.innerText == "") {
                if (el.nextElementSibling)
                    return this.getNewFocusNode(el.nextElementSibling);
                else
                    if (el.parentElement && el.parentElement.tagName == "body")
                        return el.parentElement;
                    else
                        return this.getNewFocusNode(el.parentElement);
            }
            else
                return el;
        },
        findByIndex: function(index) {
            this.currentSearch && this.currentSearch.findByIndex(index);    
        },
        findNext: function() {
            return this.currentSearch ? this.currentSearch.findNext() : -1;    
        },
        findPrev: function() {
            return this.currentSearch ? this.currentSearch.findPrev() : -1;
        },
        getEntryCount: function() {
            return this.currentSearch ? this.currentSearch.getEntryCount() : 0;    
        },
        getWrapper: function() {
            return this.searchPopup.htmlEditor.getActiveWrapper();    
        },
        getSearchOptions: function() {
            return null;
        },
        getControl: function (id) {
            var id = this.searchPopup.htmlEditor.name + "_" + this.getWindowName() + "_" + id;
            if(!this[id])
                this[id] = ASPxClientControl.GetControlCollection().GetByName(id);
            return this[id];
        },
        getWindowName: function() { },
        updateSearchInfo: function (index) {
            var entryCount = this.currentSearch.getEntryCount();
            var resultText = "";
            var newIndex = ((index + 1) > entryCount) ? entryCount : (index + 1);

            if (entryCount > 0)
                resultText = ASPx.Str.ApplyReplacement(this.getWrapper().settings.advancedSearchOfLocalization, [["{0}", newIndex], ["{1}", entryCount]]);

            this.getResultLabel().innerHTML = resultText;
        },
        getResultLabel: function () { },
        onSearchGroupInputsKeydown: function (s, evt) {
            var e = evt.htmlEvent;
            var shortcutType = this.checkShortcut(e);
            var isNavigationShortcut = (ASPx.Evt.GetKeyCode(e) == ASPx.Key.Enter) || (ASPx.Evt.GetKeyCode(e) == ASPx.Key.F3);
            var isSearchGroupInput = (this.getSearchField().inputElement == e.target) || (this.getReplaceTextBox().inputElement == e.target);

            if (shortcutType.isQuickSearchShortcut || shortcutType.isAdvanceSearchShortcut) {
                ASPx.Evt.PreventEvent(e);
            }
            if (isSearchGroupInput && isNavigationShortcut) {
                var move = !!e.shiftKey ? this.findPrev : this.findNext;
                this.updateSearchInfo(move.call(this));
                ASPx.Evt.PreventEvent(e);
            }
        },
        onSearchFieldKeyup: function (s, e) {
            var newText = s.GetText();
            if(newText !== s.prevSavedText) {
                s.prevSavedText = newText;
                this.searchThrottled();
            }
        }
    });

    var InfoFrame = ASPx.CreateClass(null, {
        constructor: function(managerBase) {
            this.root = null;
            this.msgTemplates = [];
            this.manager = managerBase;
        },
        initialize: function() {
            this.root = ASPx.GetNodeByClassName(this.manager.popup.GetWindowElement(this.manager.getWindow().index), "dxhe-searchWarning", 0);
            for(var i = 0; i < this.root.childNodes.length; i++) {
                if(this.root.childNodes[i].innerHTML)
                    this.msgTemplates.push(this.root.childNodes[i].innerHTML);
            }
            var styles = ASPx.GetCurrentStyle(this.root);
            ASPx.SetStyles(this.root, { 
                marginTop: -(ASPx.PxToInt(styles.paddingTop) + ASPx.PxToInt(styles.paddingBottom)), 
                height: styles.height
            });
            this.root.innerHTML = "";
        },
        show: function(templateIndex, templateData, closeTimeout) {
            closeTimeout = closeTimeout || 0;
            var rootEl = this.root;
            rootEl.innerHTML = ASPx.Str.ApplyReplacement(this.msgTemplates[templateIndex], templateData);
            var handler = function() { };
            if(closeTimeout > 0) {
                handler = function() {
                    ASPx.AnimationHelper.fadeOut(rootEl, this.close.aspxBind(this));
                }.aspxBind(this);
            }
            ASPx.SetElementVisibility(rootEl, true);
            ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(handler, "advancedSearchInfoFrameShow", closeTimeout, true);
        },
        close: function() {
            ASPx.SetElementVisibility(this.root, false);
            ASPx.SetElementOpacity(this.root, 1);
        }
    });

    var AdvancedSearchManager = ASPx.CreateClass(SearchManager, {
        initialize: function(text) {
            SearchManager.prototype.initialize.call(this, text);
            this.getInfoFrame().initialize();
            this.getReplaceTextBox().KeyUp.AddHandler(this.onReplaceTextBoxKeyUp, this);
            this.updateReplaceButtons();
            var width = this.getReplaceAllBtn().GetWidth();
            this.getReplaceBtn().SetWidth(width);
            this.getControl(SystemPopupConsts.PrevButtonId).SetWidth(width);
            this.getControl(SystemPopupConsts.NextButtonId).SetWidth(width);
            this.searchPopup.UpdateWindowPosition(this.getWindow());
            this.getResultList().encodeHtml = false;
        },
        getInfoFrame: function() {
            if(!this.advanceSearchInfoFrame)
                this.advanceSearchInfoFrame = new InfoFrame(this);
            return this.advanceSearchInfoFrame;
        },
        getWindowName: function() {
            return SystemPopupConsts.AdvanceSearchWindowName;    
        },
        getSearchOptions: function() {
            return {
                direction: ASPx.HtmlEditorClasses.SearchDirection.Downward,
                matchCase: this.getMatchCaseCheckBox().GetChecked()
            };
        },
        search: function(index) {
            this.performSearch(this.getSearchText());
            this.populateSearchResult();
            if(index)
                this.findByIndex(index);
            this.updateSearchInfo(index || this.findNext());
            SearchManager.prototype.search.call(this);
        },
        populateSearchResult: function() {
            var resultListBox = this.getResultList();
            resultListBox.ClearItems();
            if(this.currentSearch) {
                var maxItemCount = ASPx.HtmlEditorClasses.Search.SearchConstants.MaxEntryCount;
                resultListBox.BeginUpdate();
                var entryCount = this.getEntryCount();
                var length = Math.min(entryCount, maxItemCount);
                for(var i = 0; i < length; i++) {
                    var entry = this.currentSearch.entries[i];
                    resultListBox.AddItem(entry.getContextHtml(), i);
                }
                if(entryCount > maxItemCount) {
                    var narrowSearchMsg = ASPx.Str.ApplyReplacement(resultListBox.cpNarrowSearchWarning, [["{0}", maxItemCount]]);
                    var itemIndex = resultListBox.AddItem(narrowSearchMsg, "none");
                    var itemElement = resultListBox.GetItemElement(itemIndex);
                    ASPx.AddClassNameToElement(itemElement, "dxhe-srchCountWarning");
                }
                resultListBox.EndUpdate();
            }
        },

        updateSearchInfo: function (index) {
            if (this.getEntryCount() == 0)
                this.getResultList().ClearItems();
            else
                this.getResultList().SetSelectedIndex(index);

            this.updateReplaceButtons();
            SearchManager.prototype.updateSearchInfo.call(this, index);
        },
        getResultLabel: function() {
            if(!this.resultLabel) {
                var win = this.searchPopup.getAdvancesSearchWindow();
                var resultLabelCell = ASPx.GetNodeByClassName(this.searchPopup.GetWindowElement(win.index), "dxhe-searchResultInfo", 0);
                this.resultLabel = document.createElement("SPAN");
                resultLabelCell.appendChild(this.resultLabel);
                resultLabelCell.style.whiteSpace = "normal";
                this.resultLabel.className = "dxhe-searchEntryCount";
            }
            return this.resultLabel;
        },
        clear: function() {
            SearchManager.prototype.clear.call(this);
            this.getResultList().ClearItems();
        },
        attachEventsInternal: function() {
            this.getResultList().SelectedIndexChanged.AddHandler(function (s, e) {
                var index = this.getResultList().GetValue();
                if(index != "none") {
                    this.findByIndex(index);
                    this.updateSearchInfo(index);
                }
            }.aspxBind(this));
            this.getMatchCaseCheckBox().CheckedChanged.AddHandler(this.onMatchCaseChanged.aspxBind(this));
            this.getReplaceBtn().Click.AddHandler(this.onReplace.aspxBind(this));
            this.getReplaceAllBtn().Click.AddHandler(this.onReplaceAll.aspxBind(this));

            // TODO: Hack cannnot prevent base event
            var onArrowDownBase = this.getResultList().OnArrowDown.aspxBind(this.getResultList());
            this.getResultList().OnArrowDown = function (evt) {
                this.onResultListKeyDown(evt);
                var isPrevented = evt.defaultPrevented === true || evt.returnValue === false;
                if (!isPrevented)
                    onArrowDownBase.call(this, evt);
            }.aspxBind(this);
            var onArrowUpBase = this.getResultList().OnArrowUp.aspxBind(this.getResultList());
            this.getResultList().OnArrowUp = function (evt) {
                this.onResultListKeyDown(evt);
                var isPrevented = evt.defaultPrevented === true || evt.returnValue === false;
                if (!isPrevented)
                    onArrowUpBase.call(this, evt);
            }.aspxBind(this);
            this.getReplaceTextBox().KeyDown.AddHandler(this.onSearchGroupInputsKeydown, this);
        },
        onResultListKeyDown: function (evt) {
            var DownKeyCode = 40;
            var UpKeyCode = 38;
            var resultList = this.getResultList();

            var isDownKey = DownKeyCode == evt.keyCode;
            var isUpKey = UpKeyCode == evt.keyCode;
            var isStartItem = resultList.GetSelectedIndex() == 0;
            var endIndex = resultList.itemsValue.length - 1;
            var isEndItem = endIndex == resultList.GetSelectedIndex();
            if (isDownKey && isEndItem) {
                resultList.SelectIndexSilentAndMakeVisible(0);
                resultList.OnValueChanged();
                ASPx.Evt.PreventEvent(evt);
                return false;
            }
            else
                if (isUpKey && isStartItem) {
                    resultList.SelectIndexSilentAndMakeVisible(endIndex);
                    resultList.OnValueChanged();
                    ASPx.Evt.PreventEvent(evt);
                    return false;
                }
            return true;
        },
        getReplaceText: function() {
            return this.getReplaceTextBox().GetText();    
        },
        updateReplaceButtons: function() {
            this.getReplaceBtn().SetEnabled(this.canReplace());
            this.getReplaceAllBtn().SetEnabled(this.canReplace());
        },
        canReplace: function () {
            var hasResults = this.getEntryCount() > 0;
            return hasResults && this.getReplaceText() !== this.getSearchText();
        },
        onReplaceTextBoxKeyUp: function(s, e) {
            var newText = s.GetText();
            if(newText !== s.prevSavedText) {
                s.prevSavedText = newText;
                this.updateReplaceButtons();
            }    
        },
        onReplace: function(s, e) {
            var text = this.getReplaceText();
            var index = this.getResultList().GetSelectedIndex();
            var selectedItem = this.currentSearch.entries[index];
            var setLastItemSelected = index == this.getResultList().GetItemCount() - 1;

            selectedItem.replace(text);
            if (setLastItemSelected) {
                this.getResultList().SetSelectedIndex(this.getResultList().GetItemCount() - 1);
            }
        },
        onReplaceAll: function(s, e) {
            var text = this.getReplaceText();
            var count = this.getEntryCount();
            this.currentSearch.replaceAll(text, {
                onBatchEnd: function(endIndex) {
                    this.popup.htmlEditor.SetEnabled(false);
                    this.popup.SetEnabled(false);
                    this.getInfoFrame().show(1, [["{0}", endIndex ], ["{1}", count]]);
                }.aspxBind(this),
                onEnd: function() {
                    this.getInfoFrame().show(0, [["{0}", count]], 3000);
                    this.popup.htmlEditor.SetEnabled(true);
                    this.popup.SetEnabled(true);
                }.aspxBind(this)
            });
        },
        onMatchCaseChanged: function (s, e) {
            var newValue = s.GetChecked();
            if(newValue != this.prevSavedMatchCase) {
                this.prevSavedMatchCase = newValue;
                this.searchThrottled();
            }
        },
        getReplaceTextBox: function() {
            return this.getControl("ReplaceTb");
        },
        getResultList: function() {
            return this.getControl("ResultList");
        },
        getMatchCaseCheckBox: function() {
            return this.getControl("MatchCase");
        },
        getReplaceAllBtn: function() {
            return this.getControl("ReplaceAllBtn");
        },
        getReplaceBtn: function() {
            return this.getControl("ReplaceBtn");
        }
    });

    var QuickSearchManager = ASPx.CreateClass(SearchManager, { 
        getWindowName: function() {
            return SystemPopupConsts.QuickSearchWindowName;    
        },
        search: function () {
            this.performSearch(this.getSearchText());
            this.updateSearchInfo(this.findNext());
            SearchManager.prototype.search.call(this);
        },
        getResultLabel: function () {
            if(!this.resultLabel) {
                var inputElement = this.getSearchField().GetInputElement();
                var tdContainer = inputElement.parentNode;
                var span = document.createElement("SPAN");
                tdContainer.appendChild(span);
                ASPx.SetStyles(span, { lineHeight: tdContainer.offsetHeight });
                this.resultLabel = span;
            }
            return this.resultLabel;
        }
    });
    
    ASPx.HtmlEditorClasses.Controls.SystemPopupControl = ASPx.CreateClass(window.ASPxClientPopupControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.htmlEditor = null;
            this.currentWinName = "";
        },
        initializeControl: function(htmlEditor) {
            this.htmlEditor = htmlEditor;
            this.Shown.AddHandler(this.onShown.aspxBind(this));
            this.CloseUp.AddHandler(this.onCloseUp.aspxBind(this));
            htmlEditor.ActiveTabChanging.AddHandler(this.onActiveTabChanging.aspxBind(this));
            htmlEditor.HtmlChanged.AddHandler(this.onHtmlChanged.aspxBind(this));
            htmlEditor.BeginCallback.AddHandler(this.onBeginCallback.aspxBind(this));
        },
        AssignWindowElementsEvents: function(index, element) {
            var win = this.GetWindow(index);
            if((win.name == SystemPopupConsts.QuickSearchWindowName || win.name == SystemPopupConsts.IntelliSenseWindowName) && this.allowDragging) {
                this.allowDragging = false;
                ASPxClientPopupControl.prototype.AssignWindowElementsEvents.call(this, index, element); 
                this.allowDragging = true;
            } else 
                ASPxClientPopupControl.prototype.AssignWindowElementsEvents.call(this, index, element);
        },
        OnCollapseButtonClick: function(index) {
            switch(this.GetWindow(index).name) {
                case SystemPopupConsts.QuickSearchWindowName:
                    var searchText = this.getQuickSearchManager().getSearchText();
                    this.hideQuickSearch();
                    this.showAdvancedSearch({ selectedText: searchText });
                    break;
                default:
                    this.constructor.prototype.OnCollapseButtonClick.call(this, index);
                    break;
            }
        },
        hasManager: function() {
            return !!this.getManager();    
        },
        getManager: function (managerName) {
            var managerName = managerName || this.currentWinName;
            switch (managerName) {
                case SystemPopupConsts.QuickSearchWindowName:
                    return this.getQuickSearchManager();
                case SystemPopupConsts.AdvanceSearchWindowName:
                    return this.getAdvancedSearchManager();
                case SystemPopupConsts.IntelliSenseWindowName:
                    return this.getIntelliSenseWindowManager();
                default:
                    return null;
            }
        },
        getQuickSearchManager: function() {
            if(!this.quickSearchManager)
                this.quickSearchManager = new QuickSearchManager(this);
            return this.quickSearchManager;   
        },
        getAdvancedSearchManager: function() {
            if(!this.advancedSearchManager)
                this.advancedSearchManager = new AdvancedSearchManager(this);
            return this.advancedSearchManager;   
        },
        getIntelliSenseWindowManager: function() {
            if(!this.intelliSenseWindowManager)
                this.intelliSenseWindowManager = new IntelliSenseWindowManager(this);
            return this.intelliSenseWindowManager; 
        },
        showQuickSearch: function(parms) {
            this.hideAllManagerWindows();
            this.showSearchWindow(this.getQuickSearchWindow(), parms);
            this.dockQuickSearchWindow();
        },
        hideQuickSearch: function() {
            this.HideWindow(this.getQuickSearchWindow());    
        },
        getQuickSearchWindow: function() {
            return this.GetWindowByName(SystemPopupConsts.QuickSearchWindowName);
        },
        showSearchWindow: function(sWindow, parms) {
            var parms = parms || { selectedText: undefined };
            if (parms.selectedText === undefined) {
                var curManager = this.getManager(sWindow.name);
                if (curManager && curManager.getSelectedText)
                    parms.selectedText = this.getManager(sWindow.name).getSelectedText();
            }
            else if (parms.selectedText === null) {
                parms.selectedText = "";
            }

            sWindow.predefinedText = parms.selectedText;
            this.ShowWindow(sWindow, 0);
        },
        dockQuickSearchWindow: function() {
            var quickSearchWindowIndex = this.getQuickSearchWindow().index;
            var dockElement = this.GetPopupElement(quickSearchWindowIndex, 0);
            if(dockElement) {
                var windowElement = this.GetWindowElement(quickSearchWindowIndex);
                var dockElementStyle = ASPx.GetCurrentStyle(dockElement);

                ASPx.SetStyles(windowElement, {
                    top: ASPx.PxToInt(windowElement.style.top) + ASPx.PxToInt(dockElementStyle.paddingTop),
                    left: ASPx.PxToInt(windowElement.style.left) - ASPx.PxToInt(dockElementStyle.paddingRight) - ASPx.GetVerticalScrollBarWidth()
                });
            }    
        },
        hideAllManagerWindows: function () {
            this.hideAdvancedSearch();
            this.hideIntelliSenseWindow();
            this.hideQuickSearch();
        },
        showAdvancedSearch: function (parms) {
            this.hideAllManagerWindows();
            this.showSearchWindow(this.getAdvancesSearchWindow(), parms);
        },
        hideAdvancedSearch: function() {
            this.HideWindow(this.getAdvancesSearchWindow(), 0);
        },
        getAdvancesSearchWindow: function() {
            return this.GetWindowByName(SystemPopupConsts.AdvanceSearchWindowName);        
        },
        hideIntelliSenseWindow: function() {
            this.HideWindow(this.getIntelliSenseWindow(), 0);
        },
        showIntelliSenseWindow: function() {
            this.hideAllManagerWindows();
            this.getIntelliSenseWindowManager().initialize();
        },
        getIntelliSenseWindow: function() {
            return this.GetWindowByName(SystemPopupConsts.IntelliSenseWindowName);  
        },
        onCloseUp: function(s, e) {
            this.htmlEditor.Focus();
            if (this.hasManager())
                this.getManager().clear();
            this.currentWinName = "";
        },
        onShown: function(s, e) {
            this.currentWinName = e.window.name;
            this.getManager().initialize(e.window.predefinedText);
            e.window.predefinedText = null;
        },
        onHtmlChanged: function(s, e) {
            if(this.hasManager())
                this.getManager().updateResults();
        },
        onActiveTabChanging: function(s, e) {
            if (this.getManager() && this.getManager().getSearchField()) {
                this.getManager().getSearchField().prevSavedText = "";
            }
            this.hideAllManagerWindows();
        },
        onBeginCallback: function(s, e) {

        }
    });

    ASPx.HtmlEditorClasses.Controls.QuickSearchManager = QuickSearchManager;
})();