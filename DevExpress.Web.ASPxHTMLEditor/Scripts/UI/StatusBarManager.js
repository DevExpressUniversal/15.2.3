/// <reference path="_references.js"/>
/// <reference path="HtmlEditorConstants.js"/>
/// <reference path="HtmlEditorCore.js"/>
/// <reference path="EventManager.js"/>
(function() {
    var tabControlIDSuffix = "_TC";
    
    ASPx.HtmlEditorClasses.Controls.StatusBarManager = ASPx.CreateClass(null, {
	    constructor: function(htmlEditor) {
            this.htmlEditor = htmlEditor;
            this.core = htmlEditor.core;
        },
        getActiveView: function() {
            var tabControl = this.getTabControl();
            if(tabControl)
                return tabControl.GetActiveTab().name;
            else if(this.htmlEditor.getDesignViewCell())
                return ASPx.HtmlEditorClasses.View.Design; 
            else 
                return this.htmlEditor.getHtmlViewEditCell() ? ASPx.HtmlEditorClasses.View.Html : ASPx.HtmlEditorClasses.View.Preview;
        },
        getTabControl: function() {
            var control = ASPx.GetControlCollection().Get(this.htmlEditor.name + tabControlIDSuffix);
            return (control && control.IsDOMInitialized()) ? control : null;
        },
        changeActiveView: function(activeView, skipCallback) {
            var currentActiveWrapper = this.core.getActiveWrapper();
            if(activeView == currentActiveWrapper.getName())
                return;
            if(activeView == ASPx.HtmlEditorClasses.View.Design) {
                var wrapper = this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Design);
                wrapper.clearCachedSeletedElements();
                wrapper.processingEmptyElements();
            }
            this.htmlEditor.layoutManager.saveCurrentSize(true, true, false);
            if(!skipCallback) {
                switch (activeView) {
                    case ASPx.HtmlEditorClasses.View.Design:
                        this.setDesignActiveView();
                        break;
                    case ASPx.HtmlEditorClasses.View.Html:
                        this.setHtmlActiveView();
                        break;
                    case ASPx.HtmlEditorClasses.View.Preview:
                        this.setPreviewActiveView();
                        break;
                }
            }
            if(this.htmlEditor.isHtmlViewAllowed() && !this.htmlEditor.isSimpleHtmlEditingMode() && currentActiveWrapper.getName() != ASPx.HtmlEditorClasses.View.Preview) {
                this.savedSelectionByBookmarkObject = currentActiveWrapper.saveSelectionByBookmark();
                this.htmlEditor.removeFocus();
            }
            this.core.setActiveWrapperByName(activeView);
        },
        setDesignActiveView: function() {
            this.htmlEditor.SetClientStateFieldValue("ActiveView", "Design");
            this.htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SwitchToDesignViewCallbackPrefix, "", true);
        },
        setHtmlActiveView: function() {
            this.htmlEditor.SetClientStateFieldValue("ActiveView", "Html");
            this.htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SwitchToHtmlViewCallbackPrefix, "", true);
        },
        setPreviewActiveView: function() {
            this.htmlEditor.SetClientStateFieldValue("ActiveView", "Preview");
            this.htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.SwitchToPreviewCallbackPrefix, "", true);
        },
        ensureActiveView: function(view) {
            if(this.getActiveView() != view) {
                this.changeActiveView(view, true);
                this.setActiveTabByName(this.getFullViewName(view));
            }
        },
        // Called directly with this.GetHtml() or through a callback with purged HTML
        switchToDesignViewCore: function(html) {
            this.ensureActiveView(ASPx.HtmlEditorClasses.View.Design);
            var wrapper = this.core.getActiveWrapper();
            var oldHtml = wrapper.getHtml();
            wrapper.selectionManager.selection = null;
            if(this.savedSelectionByBookmarkObject && this.savedSelectionByBookmarkObject.safeHtmlObject)
                html = wrapper.restoreBookmark(this.savedSelectionByBookmarkObject, html, this.core.getWrapperByName(ASPx.HtmlEditorClasses.View.Html).getSourceEditor());
            this.switchToViewCore(ASPx.HtmlEditorClasses.View.Design, html, true);
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) // B188634
                wrapper.cleanWrongSizeAttribute();
            if(ASPx.Browser.NetscapeFamily && this.htmlEditor.GetEnabled())
                wrapper.resetContentEditable();
            if(oldHtml != wrapper.getHtml())
                wrapper.eventManager.onHtmlChanged(false);
            wrapper.commandManager.clearLastExecutedCommand();
            wrapper.commandManager.clearUndoHistory();
        },
        switchToHtmlViewCore: function(html) {
            var wrapper = this.core.getActiveWrapper();
            if(this.savedSelectionByBookmarkObject && this.savedSelectionByBookmarkObject.safeHtmlObject)
                html = wrapper.restoreBookmark(this.savedSelectionByBookmarkObject, html);
            this.switchToViewCore(ASPx.HtmlEditorClasses.View.Html, html);
        },
        switchToPreviewCore: function(html) {
            this.switchToViewCore(ASPx.HtmlEditorClasses.View.Preview, html);
        },
        switchToViewCore: function(view, html, skipEnsureActiveView) {
            if(!skipEnsureActiveView)
                this.ensureActiveView(view);
            var wrapper = this.core.getActiveWrapper();
            if(view != ASPx.HtmlEditorClasses.View.Design && wrapper.settings.enablePasteOptions && wrapper.commandManager.getLastPasteFormattingHtml && wrapper.commandManager.getLastPasteFormattingHtml()) {
                wrapper.commandManager.clearPasteOptions(false);
                this.htmlEditor.pasteOptionsBarManager.hideBarImmediately();
            }
            var barDockManager = this.htmlEditor.barDockManager;
            var layoutManager = this.htmlEditor.layoutManager;
            barDockManager.setItemsEnabled(false);
            if(barDockManager.getRibbon(true) && layoutManager.isInFullscreen()) {
                if(view == ASPx.HtmlEditorClasses.View.Design)
                    barDockManager.setExternalRibbonPositionOnPageTop();
                else
                    barDockManager.restoreExternalRibbonPositionOnPage();
            } 
            else if(ASPx.Browser.WebKitFamily)
                layoutManager.saveCurrentSize(true, true, false);
            this.core.setActiveWrapperByName(view);
            if(this.core.isNeedReinitIFrame())
                this.htmlEditor.onIFrameLoad();
            layoutManager.updateLayout(view);
            layoutManager.correctSizeOnSwitchToView();

            wrapper.setHtml(html);
            wrapper.focus();
            this.raiseActiveTabChanged(wrapper);
        },
        raiseActiveTabChanged: function(wrapper) {
            this.restoreSelectionAfterSwitchView(wrapper);
            this.htmlEditor.RaiseActiveTabChanged(this.getActiveTabName());
            this.htmlEditor.barDockManager.updateRibbonContextTabs();
        },
        getFullViewName: function(shortName) {
            shortName = shortName.toUpperCase();
            switch (shortName) {
                case ASPx.HtmlEditorClasses.View.Design:
                    return "Design";
                case ASPx.HtmlEditorClasses.View.Html:
                    return "HTML";
                case ASPx.HtmlEditorClasses.View.Preview:
                    return "Preview";
            }
            return "";
        },
        rollbackActiveView: function() {
            var html = this.htmlEditor.GetHtml();
            if(this.core.isDesignViewAllowed()) {
                this.core.setActiveWrapperByName(ASPx.HtmlEditorClasses.View.Design);
                this.switchToDesignViewCore(html);
            }
            else if(this.core.isHtmlViewAllowed()) {
                this.core.setActiveWrapperByName(ASPx.HtmlEditorClasses.View.Html);
                this.switchToHtmlViewCore(html);
            }
            else if(this.core.isHtmlViewAllowed()) {
                this.core.setActiveWrapperByName(ASPx.HtmlEditorClasses.View.Preview);
                this.switchToPreviewCore(html);
            }
            var tabControl = this.getTabControl();
            if(tabControl != null)
                tabControl.SetActiveTab(tabControl.GetTab(0));
        },
        setActiveTabByName: function(name) {
            var equal = function(source, res) {
                source = source.toLowerCase();
                res = res.toLowerCase();
                return source == res || source == res.substr(0, 1);
            };
            if(name && typeof name == "string") {
                var tabControl = this.getTabControl();
                if(tabControl) {
                    var viewName = equal(name, this.getFullViewName(ASPx.HtmlEditorClasses.View.Design)) ? ASPx.HtmlEditorClasses.View.Design :
                                   equal(name, this.getFullViewName(ASPx.HtmlEditorClasses.View.Html)) ? ASPx.HtmlEditorClasses.View.Html :
                                   equal(name, this.getFullViewName(ASPx.HtmlEditorClasses.View.Preview)) ? ASPx.HtmlEditorClasses.View.Preview : "";
                    if(viewName) {
                        var tab = tabControl.GetTabByName(viewName);
                        if(tab)
                            tabControl.SetActiveTab(tab);
                    }
                }
            }
        },
        getActiveTabName: function() {
            return this.getFullViewName(this.getActiveView());
        },
        restoreSelectionAfterSwitchView: function(wrapper) {
            if(this.savedSelectionByBookmarkObject && wrapper.getName() != ASPx.HtmlEditorClasses.View.Preview) {
                wrapper.restoreSelectionByBookmark();
                this.savedSelectionByBookmarkObject = null;
            }
        }
    });
})();