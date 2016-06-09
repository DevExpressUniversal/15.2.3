/// <reference path="_references.js"/>
/// <reference path="HtmlEditorConstants.js"/>
/// <reference path="HtmlEditorCore.js"/>
/// <reference path="EventManager.js"/>
(function() {

    var BarDockNameSuffix = "_TD";
    ASPx.HtmlEditorClasses.Controls.BarDockManager = ASPx.CreateClass(null, {
        constructor: function(htmlEditor) {
            this.core = htmlEditor.core;
            this.htmlEditor = htmlEditor;
            this.barDock = this.getBarDockControl();
            this.allowCommandIDsArray = null;
            this.allowDesignViewCommandList = null;
            this.allowHtmlViewCommandList = null;
            this.allowPreviewCommandList = null;
            this.isLocked = false;
            this.eventManager = null;
        },
        initialize: function() {
            this.eventManager = new ASPx.HtmlEditorClasses.Managers.BarDockEventManager(this);
        },
        isBarDockExists: function(){
            return !!(this.barDock && (this.barDock.GetMainElement() || this.barDock.isExternalRibbonMode()));
        },
        setRibbonVisible: function(value) {
            if(this.barDock && this.barDock.isRibbonMode())
                this.barDock.SetVisible(value);
        },
        getAllowCommandIDsArray: function(){
            if(!this.allowCommandIDsArray)
                this.allowCommandIDsArray = this.barDock ? this.barDock.getAllowCommandIDsArray() : [];
            return this.allowCommandIDsArray;
        },
        getAllowDesignViewCommandList: function() {
            if(!this.allowDesignViewCommandList)
                this.allowDesignViewCommandList = this.getAllowCommandList(this.htmlEditor.getDesignViewWrapper());
            return this.allowDesignViewCommandList;
        },
        getAllowHtmlViewCommandList: function() {
            if(!this.allowHtmlViewCommandList)
                this.allowHtmlViewCommandList = this.getAllowCommandList(this.htmlEditor.getHtmlViewWrapper());
            return this.allowHtmlViewCommandList;
        },
        getAllowPreviewCommandList: function() {
            if(!this.allowPreviewCommandList)
                this.allowPreviewCommandList = this.getAllowCommandList(this.htmlEditor.getPreviewWrapper());
            return this.allowPreviewCommandList;
        },
        getAllowCommandList: function(wrapper) {
            var commandList = [];
            var commandIDsArray = this.getAllowCommandIDsArray();
            for(var i = 0, commandID; commandID = commandIDsArray[i]; i++) {
                var command = wrapper.commandManager.getCommand(commandID);
                if(command)
                    commandList.push(command);
            }
            return commandList;
        },
        getAllowCommandListByActiveView: function(wrapper) {
            if(!wrapper)
                wrapper = this.core.getActiveWrapper();
            if(wrapper.getName() == ASPx.HtmlEditorClasses.View.Design)
                return this.getAllowDesignViewCommandList();
            else if(wrapper.getName() == ASPx.HtmlEditorClasses.View.Html)
                return this.getAllowHtmlViewCommandList();
            else if(wrapper.getName() == ASPx.HtmlEditorClasses.View.Preview)
                return this.getAllowPreviewCommandList(); 
            return [];
        },
        getDefaultRibbonContextTabNames: function() {
            return [ASPxClientCommandConsts.TABLETOOLSSTATE_COMMAND];
        },
        updateToolbar: function() {
            var wrapper = this.core.getActiveWrapper();
            if(this.isBarDockExists() && this.htmlEditor.GetEnabled() && !wrapper.eventManager.isUpdateLocked() && wrapper.isInFocus) {
                ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() {
                    this.barDock.updateItems(this.getAllowCommandsState(true, wrapper));
                }.aspxBind(this), "UpdateToolbar", 300, true);
            }
        },
        updateToolbarImmediately: function() {
            var wrapper = this.core.getActiveWrapper();
            if(this.isBarDockExists() && !wrapper.eventManager.isUpdateLocked() && this.htmlEditor.GetEnabled()){
                this.barDock.AdjustControls();
                if(!this.htmlEditor.clientEnabled)
                    this.barDock.SetEnabled(false);
                this.barDock.updateItems(this.getAllowCommandsState(wrapper.isInFocus, wrapper));
            }
        },
        updateRibbonContextTabs: function() {
            var wrapper = this.core.getActiveWrapper();
            if(this.isBarDockExists() && this.htmlEditor.GetEnabled() && (this.barDock.isRibbonMode() || this.barDock.isExternalRibbonMode()))
                this.barDock.updateRibbonContextTabs(this.getRibbonContextTabsState(wrapper));
        },
        getAllowCommandsState: function(useSelection, wrapper) {
            var result = [];
            var commandList = this.getAllowCommandListByActiveView(wrapper);
            if(commandList.length > 0) {
                var selection, selectedElements;
                if(this.core.isDesignView() && useSelection) {
                    if(!wrapper.isSelectionRestored() && !ASPx.Browser.Opera)
                        wrapper.restoreSelection();
                    selection = wrapper.getSelection();
                    if(!ASPx.Browser.WebKitTouchUI && !ASPx.Browser.MSTouchUI && !selection.IsCollapsed() && 
                        selection.GetHtml().length > 1 && !selection.GetIsControlSelected()) {
                            selectedElements = selection.GetElements(true);
                            wrapper.saveSelection();
                    }
                }
                for(var i = 0, command; command = commandList[i]; i++) {
                    result.push({ 
                        "commandID": command.commandID, 
                        "value": command.GetValue(wrapper, selection, selectedElements), 
                        "isChecked": command.GetState(wrapper, selection, selectedElements), 
                        "isEnabled" : command.isEnabled(wrapper, selection)
                    });
                }
            }
            return result;
        },
        getRibbonContextTabsState: function(wrapper) {
            var result = [],
                contextTabNames = this.getDefaultRibbonContextTabNames(),
                isDesignView = this.core.isDesignView(wrapper.getName()),
                selection;
            if(isDesignView && wrapper.isInFocus)
                selection = wrapper.getSelection();
            for(var i = 0, name; name = contextTabNames[i]; i++) {
                var command = wrapper.commandManager.getCommand(name);
                result.push({ 
                    "tabName": name, 
                    "isVisible": (command && isDesignView ? command.GetState(wrapper, selection) : false)
                });
            }
            return result;
        },
        saveRibbonClientState: function() {
            var ribbon = this.getRibbon();
            if(ribbon)
                this.htmlEditor.SetClientStateFieldValue("Ribbon", ASPx.Json.ToJson(ribbon.stateObject), true);
        },
        getBarDockControl: function() {
            if(!this.isBarDockExists())
                this.barDock = ASPx.GetControlCollection().Get(this.htmlEditor.name + BarDockNameSuffix);
            return this.barDock;
        },
        getRibbon: function(externalRibbon) {
            return this.isBarDockExists() && this.barDock.getRibbon(externalRibbon);
        },
        setItemsEnabled: function(enabled) {
            if(this.isBarDockExists())
                this.barDock.setItemsEnabled(enabled, this.getAllowCommandIDsArray());
        },
        hideAllPopups: function() {
            if(this.isBarDockExists())
                this.barDock.HideAllPopups();
        },
        setExternalRibbonPositionOnPageTop: function() {
            if(this.isBarDockExists())
                this.barDock.setExternalRibbonPositionOnPageTop(this.htmlEditor);
        },
        restoreExternalRibbonPositionOnPage: function() {
            if(this.isBarDockExists())
                this.barDock.restoreExternalRibbonPositionOnPage(this.htmlEditor);
        },
        isCheckSpellingButtonExists: function() {
            var barDockControl = this.getBarDockControl();
            if(barDockControl) {
                var items = barDockControl.getItemsByName(ASPxClientCommandConsts.CHECKSPELLING_COMMAND);
                return items && items.length > 0;
            }
            return false;
        },
        reconnectToExternalRibbon: function() {
            var barDockControl = this.getBarDockControl();
            if(barDockControl) {
                barDockControl.initializeToolbars();
                this.eventManager.attachEvents();
                this.allowCommandIDsArray = null;
                this.allowDesignViewCommandList = null;
                this.allowHtmlViewCommandList = null;
            }
        },
        setContextTabCategoryVisible: function(categoryName, active) {
            var barDockControl = this.getBarDockControl();
            if(barDockControl) {
                var ribbon = barDockControl.getRibbon(barDockControl.isExternalRibbonMode());
                barDockControl.setContextTabCategoryVisibleByRibbon(ribbon, categoryName, active);
            }
        }
    });
})();