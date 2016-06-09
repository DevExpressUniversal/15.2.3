(function () {
    var barDockItemPickerButtonClassName = "dxhetipControl";
    
    var ToolbarConsts = {};
    ToolbarConsts.ClassNames = {
        CustomDropDownImageItem: "dxtb-cddi",
        CustomDropDownTextItem: "dxtb-cddt"
    };

    var ToolbarItem = ASPx.CreateClass(ASPxClientMenuItem, {
	    constructor: function(menu, parent, index, name){
	        this.constructor.prototype.constructor.call(this, menu, parent, index, name);
            this.itemTemplateControl = null;
            this.itemTextTemplateControl = null;
        },
        SetEnabled: function(value) {
            ASPxClientMenuItem.prototype.SetEnabled.call(this, value);
            var template = this.getTemplateControl();
            if(template)
                template.SetEnabled(value);
        },
	    setValue: function(value){
	        var templateControl = this.getTemplateControl();
	        if(templateControl)
	            templateControl.SetValue(value);
	    },
	    getTemplateControl: function(){
	        var itemTemplate = this.getItemTemplateControl();
	        if(itemTemplate)
	            return itemTemplate;
            return this.getItemTextTemplateControl();
        },
        getItemTemplateControl: function(){
    	    if(this.itemTemplateControl === null){
	            var templateContainer = this.menu.GetItemTemplateContainer(this.GetIndexPath());
	            this.itemTemplateControl = this.getControlFromParentElement(templateContainer);
	        }
    	    return this.itemTemplateControl;
        },
        getItemTextTemplateControl: function(){
    	    if(this.itemTextTemplateControl === null){
	            var templateContainer = this.menu.GetItemTextTemplateContainer(this.GetIndexPath());
	            this.itemTextTemplateControl = this.getControlFromParentElement(templateContainer);
	        }
    	    return this.itemTextTemplateControl;
        },
        getControlFromParentElement: function(element){
            if(element) {
                for(var i = 0, childNode; childNode = element.childNodes[i]; i ++) {
                    if(!childNode.id) continue;
                    var control = ASPx.GetControlCollection().Get(childNode.id);
                    if(control)
                        return control;
                }
            }
            return null;
        }
    });

    ASPx.HtmlEditorClasses.Controls.Toolbar = ASPx.CreateClass(ASPxClientMenu, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.barDockControlName = "";

            // Cache
            this.barDockControl = null;

            this.DropDownItemBeforeFocus = new ASPxClientEvent();
            this.DropDownItemCloseUp = new ASPxClientEvent();
            this.Command = new ASPxClientEvent();
            this.CustomComboBoxInit = new ASPxClientEvent();

            // CustomItems
            this.customDropDownItemsParams = {};
        },
        Initialize: function() {
            ASPxClientMenu.prototype.Initialize.call(this);

            this.SetMenuUnselectable();
            this.InitializeItemState();
        },
        InitializeItemState: function() {
            var element = this.GetItemContentElement(this.GetItem(0).GetIndexPath());

            this.SetHoverElement(element);
            this.SetHoverElement(null);
            if(this.itemCheckedGroups[0]) {
                this.SetItemChecked(this.itemCheckedGroups[0][0].toString(), true);
                this.SetItemChecked(this.itemCheckedGroups[0][0].toString(), false);
            }
        },
        SetMenuUnselectable: function() {
            if(ASPx.SetToolbarUnselectableByTimer)
                window.setTimeout(function() { ASPx.SetToolbarUnselectableByTimer(this.name) }.aspxBind(this), 0);
        },
        GetToolbarDockControl: function() {
            if(this.barDockControl == null)
                this.barDockControl = ASPx.GetControlCollection().Get(this.barDockControlName);
            return this.barDockControl;
        },
        GetToolbatItemTemplateID: function(indexPath) {
            return this.GetItemByIndexPath(indexPath).name;
        },
        GetToolbarItemTemplateId: function(indexPath) {
            var templateName = this.GetToolbatItemTemplateID(indexPath);
            return this.name + "_ITTCNT" + indexPath + "_" + templateName;
        },
        GetToolbarItemTemplateElement: function(indexPath) {
            return ASPx.GetElementById(this.GetToolbarItemTemplateId(indexPath));
        },
        GetToolbarItemTemplateControl: function(indexPath) {
            var templateElement = this.GetToolbarItemTemplateElement(indexPath);
            return templateElement ? ASPx.GetControlCollection().Get(this.GetToolbarItemTemplateId(indexPath)) : null;
        },
        GetClientItemType: function() {
            return ToolbarItem;
        },
        SetItemEnabled: function(indexPath, enabled) {
            ASPxClientMenu.prototype.SetItemEnabled.call(this, indexPath, enabled);
            var item = this.GetItemByIndexPath(indexPath);
            var templateControl = item.getTemplateControl();
            if(templateControl) {
                var templateElement = templateControl.GetMainElement();
                if(enabled)
                    ASPx.GetStateController().EnableElement(templateElement);
                else
                    ASPx.GetStateController().DisableElement(templateElement);
            }
        },

        OnTemplateItemValueChanged: function(itemName, itemValue, menuIndex) {
            var item = menuIndex ? this.GetItem(menuIndex) : this.GetItemByName(itemName);
            this.DoRaiseCommand(item.GetIndexPath(), itemValue);
        },
        OnTemplateItemClick: function(itemName, itemValue) {
            var item = this.GetItemByName(itemName);
            this.DoRaiseCommand(item.GetIndexPath(), itemValue);
        },
        OnDropDownItemBeforeFocus: function(itemName) {
            var item = this.GetItemByName(itemName);
            this.RaiseDropDownItemBeforeFocus(item);
        },
        OnDropDownItemCloseUp: function(itemName) {
            var item = this.GetItemByName(itemName);
            this.RaiseDropDownItemCloseUp(item);
        },
        DoRaiseCommand: function(indexPath, value) {
            var item = this.GetItemByIndexPath(indexPath);
            if(typeof(ASPx.HtmlEditorClasses.Controls.ToolbarItemPicker) != "undefined") { // Try define click on CustomItemPicker
                var itemPicker = ASPx.HtmlEditorClasses.Controls.ToolbarItemPicker.FindControlByMenuItem(item);
                if(itemPicker)
                    if(itemPicker.useItemPickerImageMode == ASPx.HtmlEditorClasses.ItemPickerImageMode.ExecuteSelectedItemAction) {
                        if(value == null) {
                            value = itemPicker.GetValue();
                            ASPx.GetMenuCollection().HideAll();
                            if(!value)
                                return;
                        }
                        else
                            this.UpdateMenuItem(item, itemPicker.GetText(), itemPicker.GetImage(), itemPicker.GetTooltip());
                    }
                    else if(itemPicker.useItemPickerImageMode == ASPx.HtmlEditorClasses.ItemPickerImageMode.ShowDropDown) {
                        if(value == null)
                            return;
                    }
            }
            if(value == null)
                var customMenuItemParams = this.TryGetCustomMenuItemParams(item); // Try define click on DDCustomMenuItem
            if(customMenuItemParams) {
                this.DoRaiseCustomDropDownMenuCommand(item, customMenuItemParams);
                return;
            }
            this.RaiseCommand(item, value);
        },
        DoRaiseCustomDropDownMenuCommand: function(item, customMenuItemParams) {
            if(this.IsFirstLevelItem(item)) {
                var valueItem = item.GetItem(customMenuItemParams.selectedItemIndex);
                if(customMenuItemParams.itemPickerMode == ASPx.HtmlEditorClasses.ItemPickerImageMode.ExecuteSelectedItemAction)
                    this.RaiseCommand(item, valueItem ? valueItem.name : null);
                else if(customMenuItemParams.itemPickerMode == ASPx.HtmlEditorClasses.ItemPickerImageMode.ExecuteAction)
                    this.RaiseCommand(item, null);
            }
            else {
                if(customMenuItemParams.itemPickerMode == ASPx.HtmlEditorClasses.ItemPickerImageMode.ExecuteSelectedItemAction) {
                    customMenuItemParams.selectedItemIndex = item.index;
                    this.UpdateMenuItem(item.parent, item.GetText(), item.GetImage(), item.menu.GetItemElement(item.GetIndexPath()).title);
                }
                this.RaiseCommand(item.parent, item.name);
            }
        },
        UpdateMenuItem: function(item, text, image, tooltip) {
            var itemElement = item.menu.GetItemElement(item.indexPath);
            var menuButtonCell = item.menu.GetItemContentElement(item.indexPath);
            if(!menuButtonCell) 
                return;
            var menuButtonCellChilds = ASPx.GetNodesByPartialClassName(menuButtonCell, barDockItemPickerButtonClassName);
            if(!menuButtonCellChilds || menuButtonCellChilds.length  < 1) 
                return;
            var menuButtonContainer = menuButtonCellChilds[0];
            menuButtonContainer.title = tooltip;
            var menuButtonImg = ASPx.GetNodeByTagName(menuButtonContainer, "IMG", 0);

            if(item.indexPath == "0")
                var itemStartHeight = menuButtonCell.offsetHeight;

            tooltip = tooltip ? tooltip : text;
            if(image && image.src && menuButtonImg) {
                menuButtonContainer.innerHTML = "";
                menuButtonContainer.appendChild(menuButtonImg);
                menuButtonImg.src = image.src;
                menuButtonImg.className = image && image.className || "";
                menuButtonImg.alt = tooltip;
                menuButtonImg.title = tooltip;
            
                this.UpdateCustomDropDownItemClassName(itemElement, ToolbarConsts.ClassNames.CustomDropDownImageItem);
                ASPx.SetElementDisplay(menuButtonImg, true);
            }
            else {
                menuButtonContainer.innerHTML = text;
                this.UpdateCustomDropDownItemClassName(itemElement, ToolbarConsts.ClassNames.CustomDropDownTextItem);
                if(menuButtonImg) {
                    menuButtonContainer.appendChild(menuButtonImg);
                    ASPx.SetElementDisplay(menuButtonImg, false);
                }
            }

            if(itemStartHeight && itemStartHeight != menuButtonCell.offsetHeight)
                item.menu.AdjustControl();
        },
        UpdateCustomDropDownItemClassName: function(element, className) {
            if(!ASPx.ElementContainsCssClass(element, className)) {
                var oppositeClassName = className == ToolbarConsts.ClassNames.CustomDropDownImageItem 
                    ? ToolbarConsts.ClassNames.CustomDropDownTextItem
                    : ToolbarConsts.ClassNames.CustomDropDownImageItem;
                element.className = element.className.replace(oppositeClassName, className);
            }
        },
        IsFirstLevelItem: function(item) {
            return (item.parent && item.parent.GetIndexPath() == "");
        },
        TryGetCustomMenuItemParams: function(item) {
            if(this.IsFirstLevelItem(item))
                return this.GetCustomMenuItemParams(item);
            else
                return this.GetCustomMenuItemParams(item.parent);
        },
        GetCustomMenuItemParams: function(item) {
            var indexPath = item.GetIndexPath();
            return this.customDropDownItemsParams[indexPath];
        },
        RaiseItemClick: function(indexPath, htmlEvent) {
            this.DoRaiseCommand(indexPath, null);
            if(ASPxClientMenu.prototype.RaiseItemClick)
                ASPxClientMenu.prototype.RaiseItemClick.call(this, indexPath, htmlEvent);
        },
        // API
        RaiseCommand: function(item, value) {
            if(!this.Command.IsEmpty()) {
                var args = new ASPx.HtmlEditorClasses.Controls.ToolbarEventArgs(item, value);
                this.Command.FireEvent(this, args);
            }
        },
        RaiseDropDownItemBeforeFocus: function(item) {
            if(!this.DropDownItemBeforeFocus.IsEmpty()) {
                var args = new ASPx.HtmlEditorClasses.Controls.ToolbarEventArgs(item, null);
                this.DropDownItemBeforeFocus.FireEvent(this, args);
            }
        },
        RaiseDropDownItemCloseUp: function(item) {
            if(!this.DropDownItemCloseUp.IsEmpty()) {
                var args = new ASPx.HtmlEditorClasses.Controls.ToolbarEventArgs(item, null);
                this.DropDownItemCloseUp.FireEvent(this, args);
            }
        },
        InitCustomComboBox: function(combobox) {
            this.RaiseCustomComboBoxInit(combobox);
        },
        RaiseCustomComboBoxInit: function(combobox) {
            var args = new ASPx.HtmlEditorClasses.Controls.ToolbarEventArgs(combobox, null);
            this.CustomComboBoxInit.FireEvent(this, args);
        }
    });

    ASPx.HtmlEditorClasses.Controls.ToolbarEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(item, value){
            this.constructor.prototype.constructor.call(this);
            this.item = item;
            this.value = value;
        }
    });

    ASPx.HtmlEditorClasses.Controls.ToolbarColorButton = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name){
		    this.constructor.prototype.constructor.call(this, name);
		
		    this.colorDiv = null;
		    this.colorPicker = null;
		    this.colorPickerName = "";
		    this.colorPickerLockCount = 0;
		    this.defaultColor = "";
		    this.ColorChanged = new ASPxClientEvent();
		    this.isEmptyColor = false;
	    },
	    Initialize: function(){
            this.constructor.prototype.Initialize.call(this);
            ASPx.Selection.SetElementAsUnselectable(this.GetColorDiv());
        },
    
        GetColorDiv: function(){
            if(this.colorDiv == null)
                this.colorDiv = ASPx.GetElementById(this.name + "_CD");
            return this.colorDiv;
        },
        ColorDegToHex: function(color){
            var str = color.toString(16);
            var length = str.length;
            for(var i = str.length; i < 6; i ++ )
                str = "0" + str;
            return "#" + str;
        },
        GetColor: function(){
            var colorDiv = this.GetColorDiv();
            var color = null;
            if(!this.isEmptyColor){
                var currStyle = ASPx.GetCurrentStyle(colorDiv);
                color = currStyle ? currStyle.backgroundColor : colorDiv.style.backgroundColor;
            }
            return ASPx.Color.ColorToHexadecimal(color);
        },
        SetColor: function(color){
            this.isEmptyColor = false;
            if(color == null) {
                this.isEmptyColor = true;
                color = this.defaultColor;
            }
        
            var colorDiv = this.GetColorDiv();
            colorDiv.style.backgroundColor = color;
            this.SetColorPickerColor();
        },
        SetColorPickerColor: function(){
            if(this.colorPickerLockCount == 0){
                var colorPicker = this.GetColorPicker();
                if(colorPicker)
                    colorPicker.SetColor(this.GetColor());
            }
        },
        GetValue: function(){
            return this.GetColor();
        },
        SetValue: function(color){
            this.SetColor(color);
        },
        GetColorPicker: function(){
            if(!this.colorPicker)
                this.colorPicker = ASPx.GetControlCollection().Get(this.colorPickerName);
            return this.colorPicker;
        },
    
        OnColorPickerInit: function (colorPicker){
            this.colorPicker = colorPicker;
            this.colorPickerName = colorPicker.name;
            colorPicker.SetColor(this.GetColor());

            colorPicker.ColorChanged.AddHandler(this.OnColorPickerColorChanged.aspxBind(this));
            colorPicker.ShouldBeClosed.AddHandler(function(){ 
                return aspxTBCPColorItemClick(this);
            }.aspxBind(this.name));
        },
        OnColorPickerColorItemClick: function(){
            ASPx.GetMenuCollection().HideAll();
            this.RaiseColorChanged();
        },
        OnColorPickerColorChanged: function (s, e){
            this.colorPickerLockCount ++;
            this.SetValue(s.GetColor());
            this.colorPickerLockCount --;
        },

        RaiseColorChanged: function(){
            if(!this.ColorChanged.IsEmpty()){
                var args = new ASPxClientEventArgs();
                this.ColorChanged.FireEvent(this, args);
            }
        }
    });

    ASPx.SetMenuItemUnselectable = function(parent, indexPath) {
        ASPx.Selection.SetElementAsUnselectable(parent.GetItemContentElement(indexPath), true, true);
        ASPx.Selection.SetElementAsUnselectable(parent.GetItemTemplateElement(indexPath));
        ASPx.Selection.SetElementAsUnselectable(parent.GetItemPopOutElement(indexPath));
        ASPx.Selection.SetElementAsUnselectable(parent.GetMenuBorderCorrectorElement(indexPath));
        ASPx.Selection.SetElementAsUnselectable(parent.GetItemIndentElement(indexPath), true, true);
        ASPx.Selection.SetElementAsUnselectable(parent.GetItemSeparatorElement(indexPath), true, true);
    }

    ASPx.ToolbarCommand = function(toolbar, item, value){
        var control = toolbar.GetToolbarDockControl();
        if(control)
            control.OnCommand(toolbar, item, value);
    }
    ASPx.ToolbarItemMouseOver = function(toolbar, item){
        var control = toolbar.GetToolbarItemTemplateControl(item.index.toString());
        if(control instanceof ASPx.HtmlEditorClasses.Controls.ToolbarColorButton) {
            var colorPicker = control.GetColorPicker();
                colorPicker.SetOwner(item);
            colorPicker.SetColorSelectorDisplay(false);
            colorPicker.SetColorTablesDisplay(true);
        }
    }
    ASPx.ToolbarDropDownItemBeforeFocus = function(toolbar, item){
        var control = toolbar.GetToolbarDockControl();
        if(control)
            control.OnDropDownItemBeforeFocus(toolbar, item);
    }
    ASPx.ToolbarDropDownItemCloseUp = function(toolbar, item){
        var control = toolbar.GetToolbarDockControl();
        if(control)
            control.OnDropDownItemCloseUp(toolbar, item);
    }
    ASPx.ToolbarCustomComboBoxInit = function(toolbar, e) {
        var control = toolbar.GetToolbarDockControl();
        if(control)
            control.OnToolbarCustomComboBoxInit(toolbar, e.item);
    }

    ASPx.TBCPInit = function(name, s){
        var control = ASPx.GetControlCollection().Get(name);
        if(control != null) control.OnColorPickerInit(s);
    }
    function aspxTBCPColorItemClick(name){
        var control = ASPx.GetControlCollection().Get(name);
        if(control != null) control.OnColorPickerColorItemClick();
    }

    ASPx.TBCBBeforeFocus = function(toolbarName, itemName){
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.OnDropDownItemBeforeFocus(itemName);
    }
    ASPx.TBCBCloseUp = function(toolbarName, itemName){
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.OnDropDownItemCloseUp(itemName);
    }
    ASPx.TBCBValueChanged = function(toolbarName, itemName, itemValue){
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.OnTemplateItemValueChanged(itemName, itemValue);
    }
    ASPx.TBCBItemClick = function(toolbarName, itemName, itemValue){
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.OnTemplateItemClick(itemName, itemValue);
    }
    ASPx.TBColorButtonValueChanged = function(toolbarName, itemName, itemValue){
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.OnTemplateItemValueChanged(itemName, itemValue);
    }
    ASPx.TBItemPickerItemClick = function(toolbarName, itemName, itemValue, menuIndex){
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.OnTemplateItemValueChanged(itemName, itemValue, menuIndex);
    }
    // Init custom combobox
    ASPx.TBCCBInit = function(toolbarName, combobox) {
        var bar = ASPx.GetControlCollection().Get(toolbarName);
        if(bar)
            bar.InitCustomComboBox(combobox);
    }
    ASPx.HEToolbarColorPickerCustomColorTableUpdated = function(heControlName, colorPicker, itemName){
        var heControl = ASPx.GetControlCollection().Get(heControlName);
        if(heControl != null)
            heControl.onToolbarColorPickerCustomColorTableUpdated(colorPicker, itemName);
    }
    ASPx.HEToolbarDropDownItemBeforeFocus = function(heControlName, toolbar, item){
        if(item == "") return;    
        var heControl = ASPx.GetControlCollection().Get(heControlName);
        if(heControl != null) {
            var wrapper = heControl.core.getActiveWrapper();
            if(wrapper.saveSelectionForPopup)
                wrapper.saveSelectionForPopup();
        }
    }
    ASPx.HEToolbarDropDownItemCloseUp = function(heControlName, toolbar, item){
        if(item == "") return;    
        var heControl = ASPx.GetControlCollection().Get(heControlName);
        if(heControl != null)
            heControl.onToolbarDropDownItemCloseUp();
    }
    ASPx.HEToolbarCustomComboBoxInit = function(heControlName, toolbar, item) {
        if(item == "") return;    
        var heControl = ASPx.GetControlCollection().Get(heControlName);
        if(heControl != null)
            heControl.onToolbarCustomComboBoxInit(item);
    }
})();