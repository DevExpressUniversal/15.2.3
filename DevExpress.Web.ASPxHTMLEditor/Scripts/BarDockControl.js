(function() {

    var BarDockIDSuffix = {
        Toolbar: "_T"
    };

    ASPx.HtmlEditorClasses.Utils.ToolbarUtils = {
        getTemplateEditorControl: function(item) {
            if(item.menu)
                return item.getTemplateControl();
            else { /* ribbon */
                if(item.getEditor) {
                    var editor = item.getEditor();
                    return window.ASPxClientComboBox && (editor instanceof ASPxClientComboBox) ? editor : null;
                }
                else
                    return this.getTemplateControlsFromContainer(item.getElement())[0];
            }
        },
        getTemplateControlsFromContainer: function(container) {
            var result = [];
            if(container) {
                ASPx.GetControlCollection().ProcessControlsInContainer(container, function(control) {
                    if(window.ASPxClientComboBox && (control instanceof ASPxClientComboBox) || window.ASPx.HtmlEditorClasses.Controls.ToolbarColorButton && (control instanceof ASPx.HtmlEditorClasses.Controls.ToolbarColorButton))
                        result.push(ASPx.GetControlCollection().Get(control.name));
                });
            }
            return result;
        },
        getToolbarItems: function(toolbarControl) {
            var result = [];
            if(window.ASPxClientRibbon && (toolbarControl instanceof ASPxClientRibbon)) {
                var tabCount = toolbarControl.GetTabCount();
                for(var i = 0, tab; tab = toolbarControl.GetTab(i); i++) {
                    for(var j = 0, group; group = tab.groups[j]; j++)
                        result = result.concat(group.items);
                }
            }
            else if(toolbarControl instanceof ASPxClientMenu) {
                var itemCount = toolbarControl.GetItemCount();
                for(var i = 0; i < itemCount; i++)
                    result.push(toolbarControl.GetItem(i));
            }
            return result;
        }
    };

    ASPx.HtmlEditorClasses.Controls.BarDockControl = ASPx.CreateClass(ASPxClientControl, {
	    constructor: function(name) {
		    this.constructor.prototype.constructor.call(this, name);
		    this.isLocked = false;

            this.innerToolbarsCount = 0;
            this.extToolbarID = "";

		    this.Command = new ASPxClientEvent();
		    this.DropDownItemBeforeFocus = new ASPxClientEvent();
		    this.DropDownItemCloseUp = new ASPxClientEvent();
		    this.CustomComboBoxInit = new ASPxClientEvent();
        },
        Initialize: function() {
            this.initializeToolbars();
            this.constructor.prototype.Initialize.call(this);
        },
        initializeToolbars: function() {
            this.toolbars = [];
            if(this.innerToolbarsCount) {
                for(var i = 0; i < this.innerToolbarsCount; i++)
                    this.toolbars.push(ASPx.GetControlCollection().Get(this.name + BarDockIDSuffix.Toolbar + i));
                if(this.isRibbonMode())
                    this.setUnselectable(this.toolbars[0].name);
            }
            else if(this.extToolbarID) {
                var toolbar = ASPx.GetControlCollection().Get(this.extToolbarID);
                if(toolbar)
                    this.toolbars.push(toolbar);
            }
        },
        Focus: function(){
            if(this.innerToolbarsCount) {
                var toolbarControl = this.getToolbarByIndex(0);
                if(toolbarControl && toolbarControl.TryFocusItem) {
                    var items = ASPx.HtmlEditorClasses.Utils.ToolbarUtils.getToolbarItems(toolbarControl);
                    for(var i = 0, item; item = items[i]; i++) {
                        if(item.GetEnabled()) {
                            toolbarControl.TryFocusItem(i);
                            return;
                        }
                    }
                }
            }
        },
        FocusLastToolbar: function(){
            if(this.innerToolbarsCount) {
                var toolbarControl = this.getToolbarByIndex(this.toolbars.length -1);
                if(toolbarControl && toolbarControl.FocusLastItem)
                    toolbarControl.FocusLastItem();
            }
        },
        OnCommand: function(toolbar, item, value){
            this.RaiseCommand(toolbar, item, value);
        },
        OnDropDownItemBeforeFocus: function(toolbar, item){
            this.RaiseDropDownItemBeforeFocus(toolbar, item);
        },
        OnDropDownItemCloseUp: function(toolbar, item){
            this.RaiseDropDownItemCloseUp(toolbar, item);
        },
        OnToolbarCustomComboBoxInit: function(toolbar, combobox) {
            this.RaiseToolbarCustomComboBoxInit(toolbar, combobox);
        },
        // API
        RaiseCommand: function(toolbar, item, value){
            if(!this.Command.IsEmpty()){
                var args = new ASPxClientBarDockControlEventArgs(false, toolbar, item, value);
                this.Command.FireEvent(this, args);
            }
        },
        RaiseDropDownItemBeforeFocus: function(toolbar, item){
            if(!this.DropDownItemBeforeFocus.IsEmpty()){
                var args = new ASPxClientBarDockControlEventArgs(false, toolbar, item, null);
                this.DropDownItemBeforeFocus.FireEvent(this, args);
            }
        },
        RaiseDropDownItemCloseUp: function(toolbar, item){
            if(!this.DropDownItemCloseUp.IsEmpty()){
                var args = new ASPxClientBarDockControlEventArgs(false, toolbar, item, null);
                this.DropDownItemCloseUp.FireEvent(this, args);
            }
        },
        RaiseToolbarCustomComboBoxInit: function(toolbar, item) {
            if(!this.CustomComboBoxInit.IsEmpty()){
                var args = new ASPxClientBarDockControlEventArgs(false, toolbar, item, null);
                this.CustomComboBoxInit.FireEvent(this, args);
            }
        },
        updateItems: function(commandsStateArray) {
            if(!this.toolbars.length || (!this.clientEnabled && this.innerToolbarsCount) || !commandsStateArray) return;
            for(var i = 0, commandState; commandState = commandsStateArray[i]; i++) {
                var itemsArray = this.getItemsByName(commandState.commandID);
                this.processingItemsState(itemsArray, commandState);
            }
        },
        updateRibbonContextTabs: function(ribbonContextTabsStateArray) {
            if(this.isRibbonMode() || this.isExternalRibbonMode()) {
                for(var i = 0, contextTabState; contextTabState = ribbonContextTabsStateArray[i]; i++) {
                    var ribbon = this.getRibbon(this.isExternalRibbonMode());
                    this.setContextTabCategoryVisibleByRibbon(ribbon, contextTabState.tabName, contextTabState.isVisible);
                }
            }
        },
        HideAllPopups: function(){
            if(window.ASPxClientRibbon) {
                for(var i = 0, toolbar; toolbar = this.getToolbarByIndex(i); i++) {
                    if(toolbar instanceof ASPxClientRibbon)
                        toolbar.hideAllPopups();
                }
            }
            ASPx.GetMenuCollection().DoHidePopupMenus(null, 0, null, false, "");
            if(ASPx.GetDropDownCollection)
                ASPx.GetDropDownCollection().OnDocumentMouseDown(null);
        },
        AdjustControls: function(){    
            if(this.innerToolbarsCount) {
                for(var i = 0, toolbar; toolbar = this.getToolbarByIndex(i); i++) {
                    toolbar.AdjustControl();
                }
            }
        },
        processingItemsState: function(itemsArray, commandState) {
            for(var j = 0, item; item = itemsArray[j]; j++) {
                var isChecked = commandState.isEnabled ? commandState.isChecked : false;
                item.SetEnabled(commandState.isEnabled);
                if(window.ASPxClientRibbonItem && (item instanceof ASPxClientRibbonItem)) {
                    var value = commandState.value == "true" || commandState.value == "false" ? null : commandState.value;
                    item.SetValue(value != null || value != undefined ? value : isChecked);
                }
                else {
                    item.SetChecked(isChecked);
                    if(item.setValue)
                        item.setValue(commandState.value);    
                }
            }
        },
        getItemTemplateValuesByName: function(name) {
            var result = [];
            var items = this.getItemsByName(name);
            for(var i = 0, item; item = items[i]; i++) {
                var templateControl = ASPx.HtmlEditorClasses.Utils.ToolbarUtils.getTemplateEditorControl(item);
                if(templateControl && templateControl.GetItem) {
                    for(var j = 0, item; item = templateControl.GetItem(j); j++) {
                        if(ASPx.Data.ArrayIndexOf(result, item.value) < 0)
                            result.push(item.value);
                    }
                }
            }
            return result;
        },
        getAllowCommandIDsArray: function() {
            var result = [];
            for (key in ASPxClientCommandConsts) {
                if(ASPxClientCommandConsts.hasOwnProperty(key)) {
                    var items = this.getItemsByName(ASPxClientCommandConsts[key]);
                    if(items.length > 0)
                        result.push(ASPxClientCommandConsts[key]);
                }
            }
            return result;
        },
        getItemsByName: function(name) {
            var result = [];
            for(var i = 0, toolbar; toolbar = this.getToolbarByIndex(i); i++) {
                var item = toolbar.GetItemByName(name);
                if(item)
                    result.push(item);
            }
            return result;
        },
        setUnselectable: function(toolbarName) {
            window.setTimeout(function() { ASPx.SetToolbarUnselectableByTimer(toolbarName); }.aspxBind(this), 0);
        },
        isRibbonMode: function() {
            var mainElement = this.GetMainElement();
            return this.innerToolbarsCount == 1 && mainElement && mainElement.className.indexOf("dxtbr") > -1;
        },
        isExternalRibbonMode: function() {
            return this.innerToolbarsCount == 0 && !!this.extToolbarID;
        },
        getRibbon: function(externalRibbon) {
            return !externalRibbon && this.isRibbonMode() || externalRibbon && this.isExternalRibbonMode() ? this.getToolbarByIndex(0) : null;
        },
        getToolbarByIndex: function(index) {
            return this.toolbars && this.toolbars.length > 0 ? this.toolbars[index] : null;
        },
        setExternalRibbonPositionOnPageTop: function(htmlEditor) {
            if(this.extToolbarID && this.toolbars.length > 0) {
                var externalRibbon = this.toolbars[0];
                var ribbonMainElement = externalRibbon.GetMainElement();
                if(ribbonMainElement.style.position == "fixed")
                    return;
                ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "position", "fixed");
                ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "top", "0px");
                ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "left", "0px");
                ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, ASPx.Browser.IE ? "zIndex" : "z-index", 10002);
                if(!ASPx.IsPercentageSize(ribbonMainElement.style.width)) {
                    this.saveRibbonWidth = externalRibbon.GetWidth();
                    ribbonMainElement.style.width = "100%";
                    externalRibbon.widthValueSetInPercentage = true;
                }
                externalRibbon.AdjustControl();
                var height = ribbonMainElement.offsetHeight;
                ASPx.Attr.ChangeStyleAttribute(htmlEditor.GetMainElement(), "top", height + "px");
                htmlEditor.layoutManager.currentHeight = ASPx.GetDocumentClientHeight() - height;
                htmlEditor.layoutManager.setHeightInternal(htmlEditor.layoutManager.currentHeight, false, false);
            }
        },
        restoreExternalRibbonPositionOnPage: function(htmlEditor) {
            if(this.extToolbarID && this.toolbars.length > 0) {
                var externalRibbon = this.toolbars[0];
                var ribbonMainElement = externalRibbon.GetMainElement();
                if(!ribbonMainElement.style.position)
                    return;
                ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "left");
                ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "top");
                ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "position");
                ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, ASPx.Browser.IE ? "zIndex" : "z-index");
                if(htmlEditor.layoutManager.isInFullscreen()) {
                    htmlEditor.layoutManager.currentHeight = ASPx.GetDocumentClientHeight();
                    htmlEditor.layoutManager.setHeightInternal(htmlEditor.layoutManager.currentHeight, false, false);
                    ASPx.Attr.ChangeStyleAttribute(htmlEditor.GetMainElement(), "top", "0px");
                }
                if(this.saveRibbonWidth) {
                    ribbonMainElement.style.width = this.saveRibbonWidth + "px";
                    this.saveRibbonWidth = undefined;
                }
                externalRibbon.AdjustControl();
            }
        },
        setItemsEnabled: function(enabled, listItemsNames) {
            for(var i = 0, itemName; itemName = listItemsNames[i]; i++) {
                var itemsArray = this.getItemsByName(itemName);
                for(var j = 0, item; item = itemsArray[j]; j++)
                    item.SetEnabled(enabled);
            }
        },
        setContextTabCategoryVisibleByRibbon: function(ribbon, categoryName, active) {
            ribbon.SetContextTabCategoryVisible(categoryName, active);
        }
    });
   
    var ASPxClientBarDockControlEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(processOnServer, toolbar, item, value){
            this.constructor.prototype.constructor.call(this);
            this.item = item;
            this.toolbar = toolbar;
            this.value = value;
        }
    });

    // TODO This is custom part therefore it need to be replaced into HTMLEditor script
    ASPx.HEToolbarCommand = function(heControlName, item, value){
        var heControl = ASPx.GetControlCollection().Get(heControlName);
        if(heControl) {
            if(ASPx.HtmlEditorsCollection.Get().IsLastActive(heControl) || !heControl.barDockManager.getRibbon(true)) {
                var templateControl = ASPx.HtmlEditorClasses.Utils.ToolbarUtils.getTemplateEditorControl(item);
                if(heControl.isDesignView() && ASPx.Browser.IE && templateControl && window.ASPxClientComboBox && (templateControl instanceof ASPxClientComboBox) && templateControl.droppedDown === false) {
                    var wrapper = heControl.getDesignViewWrapper();
                    wrapper.selectionManager.isSelectionRestored = false;
                    wrapper.eventManager.setLockUpdate(true);
                }
                heControl.ExecuteCommandInternal(item.name, (typeof value) != "boolean" ? value : null);
            }
        }
    }

    ASPx.SetToolbarUnselectableByTimer = function(name) {
        var toolbar = ASPx.GetControlCollection().Get(name);
        if(window.ASPxClientRibbon && (toolbar instanceof ASPxClientRibbon)) {
            var items = ASPx.HtmlEditorClasses.Utils.ToolbarUtils.getToolbarItems(toolbar);
            for(var i = 0, item; item = items[i]; i++) {
                if(item.getElement && item.type != 1) {
                    if(!item.getEditor)
                        ASPx.Selection.SetElementAsUnselectable(item.getElement(), true, true);
                    else
                        setComboBoxItemUnselectable(item.getEditor());
                }
            }
            var tabControl = toolbar.getTabControl();
            if(tabControl) {
                for(var i = 0; i < tabControl.GetTabCount(); i++)
                    ASPx.Selection.SetElementAsUnselectable(tabControl.GetTabElement(i), true, true);
            }
        } 
        else if(window.ASPx.HtmlEditorClasses.Controls.Toolbar && (toolbar instanceof ASPx.HtmlEditorClasses.Controls.Toolbar)) {
            for (var i = 0; i < toolbar.GetItemCount(); i++) {
                var curItem = toolbar.GetItem(i);
                ASPx.SetMenuItemUnselectable(toolbar, curItem.GetIndexPath());
                for (var j = 0; j < curItem.GetItemCount(); j++)
                    ASPx.SetMenuItemUnselectable(toolbar, curItem.GetItem(j).GetIndexPath());
            }
        }
    }
    function setComboBoxItemUnselectable(comboBox) {
        if(!comboBox || !window.ASPxClientComboBox || !(comboBox instanceof ASPxClientComboBox))
            return;
        var input = comboBox.GetInputElement();
        if(comboBox.ForceRefocusEditor)
            comboBox.ForceRefocusEditor = function() {};
        comboBox.GetMainElement().unselectable = "on";
        input.unselectable = "on";
        if(input.offsetParent)
            input.offsetParent.unselectable = "on";
        var ddbutton = comboBox.GetDropDownButton();
        if(ddbutton)
            ddbutton.unselectable = "on";
        var listBox = comboBox.GetListBoxControl();
        if(listBox) {
            var table = listBox.GetListTable();
            for(var i = 0, row; row = table.rows[i]; i++) {
                for(var j = 0, cell; cell = row.cells[j]; j++)
                    ASPx.Selection.SetElementAsUnselectable(cell, true);
            }
        }
    }
})();