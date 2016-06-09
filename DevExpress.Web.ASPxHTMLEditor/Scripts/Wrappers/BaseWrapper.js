(function() {
    ASPx.HtmlEditorClasses.Wrappers.BaseWrapper = ASPx.CreateClass(ASPx.HtmlEditorClasses.HtmlProcessor, {
        constructor: function(id, settings, callbacks) {
            this.id = id;
            this.settings = settings;
            this.callbacks = callbacks;
            this.keyboardManager = null;
            this.eventManager = null;
            this.enabled = true;
            this.lockCount = 0;
        },
        initialize: function(inlineInit) {
        },
        initializeIfNeeded: function() {
        },
        initializeManagers: function() {
            this.commandManager = this.createCommandManager();
            this.keyboardManager = this.createKeyboardManager(this.settings.shortcuts, []);
        },
        createCommandManager: function() {
            return new ASPx.HtmlEditorClasses.Managers.CommandManagerBase(this);
        },
        createKeyboardManager: function (shortcuts, disabledShortcuts) {
            return new ASPx.HtmlEditorClasses.Managers.KeyboardManager(shortcuts, disabledShortcuts);
        },
        getSearchContainer: function() {
            return null;    
        },
        getSearchSelectedElement: function() {
            return null;    
        },
        getSearchSelectedText: function() {
            return "";    
        },
        lockAndPerformAction: function(action) {
            this.lockCount++;
            this.performLockedAction(action);
            this.lockCount--;
            if(this.lockCount === 0)
                this.onLockRelease();
        },
        onLockRelease: function() { },
        performLockedAction: function(action) {
            action();       
        },
        getName: function () {
            return "";
        },
        getHtml: function () {
            if (this.settings.allowEditFullDocument && this.getWholeHtml) 
                return this.getWholeHtml();
            else
                return this.getProcessedHtml(this.getRawHtml());
        },
        getRawHtml: function() {
            return "";
        },
        getProcessedHtml: function(html) {
            html = ASPx.HtmlEditorClasses.HtmlProcessor.clearDXElements(html);
            html = ASPx.HtmlEditorClasses.HtmlProcessor.removeSavedUrlsInHTML(html);
            if(this.settings.enterMode == ASPx.HtmlEditorClasses.EnterMode.BR) { // Q346731
                var regExp = new RegExp(ASPx.HEBogusSymbol, 'g');
                html = html.replace(regExp, ' ');
            }
            return html;
        },
        setHtml: function (html) {
            if (this.settings.allowEditFullDocument)
                this.setWholeHtml(html);
            else
                this.insertHtml(this.processHtmlBodyBeforeInsert(html));
        },
        insertHtml: function(html) {
        },
        processHtmlBodyBeforeInsert: function(html) {
            return ASPx.HtmlEditorClasses.HtmlProcessor.filteringByHtml(html, this.settings.tagFilter, this.settings.attrFilter, this.settings.styleAttrFilter);
        },
        setSpellCheckAttributeValue: function(value) {
        },
        getElement: function() {
            return null;
        },
        getMainElement: function() {
            return this.getElement();
        },
        isActive: function() {
            return ASPx.IsElementDisplayed(this.getMainElement());
        },
        focus: function() {
            this.updateToolbar(true);
        },
        itemIsFiltered: function(itemName, filterSettings) {
            if(!itemName || !filterSettings || filterSettings.list.length == 0 && filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList)
                return false;
            itemName = itemName.toLowerCase();
            var isContains = ASPx.Data.ArrayContains(filterSettings.list, itemName);
            return (filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList && isContains) || (filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.WhiteList && !isContains);
        },
        tagIsFiltered: function(tagName, element) {
            return this.itemIsFiltered(tagName, this.settings.tagFilter);
        },
        attributeIsFiltered: function(attrName, attrList, styleAttrList) {
            return this.itemIsFiltered(attrName, this.settings.attrFilter);
        },
        styleAttributeIsFiltered: function(styleAttrName, attrList, styleAttrList) {
            return this.itemIsFiltered(styleAttrName, this.settings.styleAttrFilter);
        },
        tagIsFilteredByConditional: function(tagName, element) {
            var result = true;
            var elementInfo = ASPx.HtmlEditorClasses.getElementInfo(element);
            var conditionalTagFilterSettings = ASPx.HtmlEditorClasses.getConditionalTagFilterSettings();
            for(var i = 0, filter; filter = conditionalTagFilterSettings[i]; i++) {
                if(filter.tagName == tagName) {
                    for(var j = 0, attr; attr = elementInfo.attrList[j]; j++) {
                        if(filter.isNotFiltered(attr))
                            return false;
                    }
                }
            }
            return result;
        },
        saveSelectionByBookmark: function(cm) {
        },
        restoreSelectionByBookmark: function() {
        },
        restoreBookmark: function() {
        },

        /* keyboard manager methods mapping */
        getKeyDownInfo: function(evt) {
            return this.keyboardManager && this.keyboardManager.getKeyDownInfo(evt);
        },
        getLastShortcutID: function() {
            return this.keyboardManager && this.keyboardManager.getLastShortcutID();
        },
        getShortcutCommand: function(evt) {
            return this.keyboardManager && this.keyboardManager.getShortcutCommand(evt);
        },
        isBrowserShortcut: function(evt) {
            return this.keyboardManager && this.keyboardManager.isBrowserShortcut(evt);
        },
        clearLastShortcut: function() {
            if(this.keyboardManager) 
                this.keyboardManager.clearLastShortcut();
        },
        getLastKeyDownInfo: function() {
            return this.keyboardManager && this.keyboardManager.getLastKeyDownInfo();
        },
        clearKeyDownInfo: function() {
            if(this.keyboardManager)
                this.keyboardManager.clearKeyDownInfo();
        },
        isSpacing: function() {
            return this.keyboardManager && this.keyboardManager.isSpacing();
        },

        /* Events */
        showQuickSearch: function() {
            this.tryInvoke(this.callbacks.onShowQuickSearch, arguments);
        },
        showAdvancedSearch: function() {
            this.tryInvoke(this.callbacks.onShowAdvancedSearch, arguments);
        },
        updateToolbar: function() {
            this.tryInvoke(this.callbacks.onUpdateToolbar, arguments);
        },
        raiseSelectionChanged: function(wrapper) {
            this.tryInvoke(this.callbacks.onRaiseSelectionChanged, arguments);
        },
        raiseBeforePaste: function(commandName, html) {
            return this.tryInvoke(this.callbacks.onRaiseBeforePaste, arguments) || html;
        },
        raiseHtmlChanged: function(saveSelectionAndHtml, preventEvent, hidePasteOptionsBar) {
            this.tryInvoke(this.callbacks.onRaiseHtmlChanged, arguments);
        },
        raiseFocus: function(wrapper) {
            this.tryInvoke(this.callbacks.onRaiseFocus, arguments);
        },
        raiseLostFocus: function(wrapper) {
            this.tryInvoke(this.callbacks.onRaiseLostFocus, arguments);
        },
        focusLastToolbar: function() {
            this.tryInvoke(this.callbacks.onFocusLastToolbar);
        },
        hideAllPopups: function() {
            this.tryInvoke(this.callbacks.onHideAllPopups);
        },
        setFullscreenMode: function(wrapper, enable) {
            this.tryInvoke(this.callbacks.onSetFullscreenMode, arguments);
        },
        checkSpelling: function() {
            this.tryInvoke(this.callbacks.onCheckSpelling);
        },
        raiseExport: function(format) {
            this.tryInvoke(this.callbacks.onExport, arguments);
        },
        executeDialog: function(dialog) {
            this.tryInvoke(this.callbacks.onExecuteDialog, arguments);        
        },
        raiseExecuteCommand: function(commandName, parameter, isSuccessfully) {
            this.tryInvoke(this.callbacks.raiseExecuteCommand, arguments);   
        },
        raiseCommandExecuting: function(commandName, parameter) {
            return this.tryInvoke(this.callbacks.raiseCommandExecuting, arguments);    
        },
        showPasteOptionsBar: function() {
            this.tryInvoke(this.callbacks.onShowPasteOptionsBar);
        },
        hidePasteOptionsBar: function() {
            this.tryInvoke(this.callbacks.onHidePasteOptionsBar);
        },
        removeFocus: function() {
            this.tryInvoke(this.callbacks.onRemoveFocus);
        },
        tryInvoke: function(func, args) {
            if(func)
                return func.apply(this, args || []);
        }
    });
})();