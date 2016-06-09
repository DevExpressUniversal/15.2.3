/// <reference path="..\_references.js"/>

(function() {
var ColorTableIdPostfix = {
    ColorTable: "_CT",
    CustomColorTable: "_UCT",
    ColorPicker: "_CP",
    OkButton: "_OB",
    CancelButton: "_CB",
    ColorSelector: "_CS",
    ColorTables: "_CTS",
    CustomColorButton: "_CCB",
    AutomaticColorItem: "_ACI",
    AutomaticColorItemSelectionFrame: "_ACISF"
};

var ColorNestedControl = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.cookieName = "";
        this.color = null;
        this.enableCustomColors = true;
        this.enableAutomaticColorItem = false;
        this.automaticColor = null;
        this.automaticColorItemValue = null;
        this.isAutomaticColorSelected = false;
        this.ownerControl = null;
        this.ownerElementId = null;

        this.ColorChanged = new ASPxClientEvent();
        this.ShouldBeClosed = new ASPxClientEvent();
        this.CustomColorTableUpdated = new ASPxClientEvent();
    },
    Initialize: function() {
        this.InitializeColorTable();
        this.InitializeColorTableEvents();
        
        if(this.enableCustomColors) {
            this.InitializeCustomColorTable();
		    this.InitializeCustomColorEvents();
            this.SetColorSelectorDisplay(false);
        }
        if(this.enableAutomaticColorItem)
            this.InitializeAutomaticColorItemEvents();
        ASPxClientControl.prototype.Initialize.call(this);
    },
    InitializeColorTable: function() {
        var colorTable = this.GetColorTableControl();
        if(colorTable && this.color != null && !this.IsAutomaticColorSelected())
            colorTable.SetColor(this.color);
    },
    InitializeColorTableEvents: function() {
        this.GetColorTableControl().ColorChanged.AddHandler(this.OnInnerTableColorChanged.aspxBind(this));
        this.GetColorTableControl().ItemClick.AddHandler(this.OnColorTableCellClick.aspxBind(this));
    },
    InitializeCustomColorTable: function() {
        var customColorTable = this.GetCustomColorTableControl();
        if(customColorTable && this.color != null && !this.IsAutomaticColorSelected())
            customColorTable.SetColor(this.color);
    },
    InitializeCustomColorEvents: function() {
        ASPx.Evt.AttachEventToElement(this.GetOkButtonElement(), "click", this.OnOkButtonClick.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetCancelButtonElement(), "click", this.OnCancelButtonClick.aspxBind(this));
        ASPx.Evt.AttachEventToElement(this.GetCustomColorButtonElement(), "click", this.OnCustomColorButtonClick.aspxBind(this));
        var colorPickerControl = this.GetColorPickerControl();
        colorPickerControl.InputEsc.AddHandler(this.OnCancelButtonClick.aspxBind(this));
        colorPickerControl.InputEnter.AddHandler(this.OnOkButtonClick.aspxBind(this));
        var customColorTable = this.GetCustomColorTableControl();
        customColorTable.ColorChanged.AddHandler(this.OnInnerTableColorChanged.aspxBind(this));
        customColorTable.EmptyCellClick.AddHandler(this.OnCustomColorButtonClick.aspxBind(this));
        customColorTable.AddColorEvent.AddHandler(this.OnCustomColorTableAddColor.aspxBind(this));
        customColorTable.ItemClick.AddHandler(this.OnColorTableCellClick.aspxBind(this));
    },
    InitializeAutomaticColorItemEvents: function() {
        ASPx.Evt.AttachEventToElement(this.GetAutomaticColorItemElement(), "click", this.OnAutomaticColorItemClick.aspxBind(this));
    },
    GetMainElement: function() {
        return this.GetColorTablesElement();
    },
    GetColorTableControl: function() {
        return ASPx.GetControlCollection().Get(this.name + ColorTableIdPostfix.ColorTable);
    },
    GetCustomColorTableControl: function() {
        return ASPx.GetControlCollection().Get(this.name + ColorTableIdPostfix.CustomColorTable);
    },
    GetColorPickerControl: function() {
        return ASPx.GetControlCollection().Get(this.name + ColorTableIdPostfix.ColorPicker);
    },
    GetOkButtonElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.OkButton);
    },
    GetCancelButtonElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.CancelButton);
    },
    GetColorSelectorElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.ColorSelector);
    },
    GetColorTablesElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.ColorTables);
    },
    GetCustomColorButtonElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.CustomColorButton);
    },
    GetAutomaticColorItemElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.AutomaticColorItem);
    },
    GetAutomaticColorItemSelectionFrameElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIdPostfix.AutomaticColorItemSelectionFrame);
    },
    SetOwner: function(ownerControl) {
        this.ownerControl = ownerControl;
    },
    SetOwnerElementId: function(ownerElementId) {
        this.ownerElementId = ownerElementId;
    },
    OnInnerTableColorChanged: function(s, e) {
        this.ChangeColor(s.GetColor(), false);
    },
    OnAutomaticColorItemClick: function() {
        this.ChangeColor(this.GetAutomaticColor(), true);
        this.RaiseShouldBeClosed();
    },
    OnColorTableCellClick: function() {
        this.RaiseShouldBeClosed();
    },
    OnOkButtonClick: function() {
        var colorPicker = this.GetColorPickerControl();
        var colorTable = this.GetColorTableControl();
        var customColorTable = this.GetCustomColorTableControl();

        var color = colorPicker.GetColor();
        var colorIndex = colorTable.FindColorIndexByColor(color);
        if(colorIndex == -1) {
            colorIndex = customColorTable.FindColorIndexByColor(color);
            if(colorIndex == -1) {
                customColorTable.AddColor(color);
                colorIndex = customColorTable.FindColorIndexByColor(color);
            }
            customColorTable.ProcessItemClick(colorIndex);
        } else
            colorTable.ProcessItemClick(colorIndex);

        this.SetColorTablesDisplay(true);
        this.SetColorSelectorDisplay(false);
    },
    OnCancelButtonClick: function() {
        this.SetColorTablesDisplay(true);
        this.SetColorSelectorDisplay(false);
        this.RaiseShouldBeClosed();
    },
    OnCustomColorButtonClick: function() {
        this.SetColorTablesDisplay(false);
        this.SetColorSelectorDisplay(true);
        
        if(this.ownerControl && typeof (ASPxClientMenuItem) !== "undefined" && this.ownerControl instanceof ASPxClientMenuItem) 
            this.AlignOwnerMenuItem();

        if(this.ownerControl && typeof(ASPxClientPopupControl) !== "undefined" && this.ownerControl instanceof ASPxClientPopupControl)
            this.AlignOwnerPopup();
    },
    UpdateAutomaticColorItem: function() {
        var selectionFrameElement = this.GetAutomaticColorItemSelectionFrameElement();
        if(selectionFrameElement == null)
            return;
        var stateController = ASPx.GetStateController();
        if(this.IsAutomaticColorSelected())
            stateController.SelectElementBySrcElement(selectionFrameElement);
        else
            stateController.DeselectElementBySrcElement(selectionFrameElement);
    },
	AlignOwnerPopup: function() {
        this.ownerControl.UpdatePositionAtElement(document.getElementById(this.ownerElementId));
    },
    AlignOwnerMenuItem: function() {
	    var indexPath = this.ownerControl.indexPath,
            element = this.ownerControl.menu.GetMenuElement(indexPath),
            isVertical = this.ownerControl.menu.IsVertical(indexPath),
	        horizontalPopupPosition = this.ownerControl.menu.GetClientSubMenuPos(element, indexPath, ASPx.InvalidPosition, isVertical, true),
            verticalPopupPosition = this.ownerControl.menu.GetClientSubMenuPos(element, indexPath, ASPx.InvalidPosition, isVertical, false),
            clientX = horizontalPopupPosition.position,
            clientY = verticalPopupPosition.position;

	    ASPx.SetStyles(element, {
	        left: clientX, top: clientY
	    });
    },
    ChangeColor: function(newColor, isAutomaticColor) {
        var isNew = this.IsNew(newColor, isAutomaticColor);
        this.SetColor(newColor, isAutomaticColor);
        if(isNew)
            this.RaiseColorChanged();
    },
    IsNew: function(newColor, isAutomaticColor) {
        var isChangedItemType = isAutomaticColor != this.IsAutomaticColorSelected();
        var isChangedColor = newColor ? this.color != newColor : !!this.color
        return isChangedItemType || isChangedColor;
    },
    SetColorSelectorDisplay: function(display) {
        var colorSelector = this.GetColorSelectorElement();
        if(!colorSelector)
            return;

        ASPx.SetElementDisplay(colorSelector, display);
        if(display) {
            var color = this.GetColor();
            var colorPicker = this.GetColorPickerControl();

            colorPicker.InitializeSize();
            if(color) {
                colorPicker.SetColorByHexColor(color);
                colorPicker.SetSavedColor(color);
            } else
                colorPicker.SetDefaultState();
        }
    },
    SetColorTablesDisplay: function(display) {
        ASPx.SetElementDisplay(this.GetColorTablesElement(), display);
    },
    IsColorSelectorDisplayed: function() {
        var colorSelector = this.GetColorSelectorElement();
        return colorSelector ? ASPx.GetElementDisplay(colorSelector) : false;
    },

    GetState: function() {
        var customColorTable = this.GetCustomColorTableControl();
        return customColorTable.GetSerializedColors();
    },
    OnCustomColorTableAddColor: function() {
        this.RaiseCustomColorTableUpdated();
    },

    RaiseShouldBeClosed: function() {
        if(!this.ShouldBeClosed.IsEmpty()) {
            var args = new ASPxClientEventArgs();
            this.ShouldBeClosed.FireEvent(this, args);
        }
    },
    RaiseColorChanged: function(processOnServer) {
        if(!this.ColorChanged.IsEmpty()) {
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.ColorChanged.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    RaiseAutomaticColorItemClick: function() {
        if(!this.AutomaticColorItemClick.IsEmpty()){
            var args = new ASPxClientEventArgs();
            this.AutomaticColorItemClick.FireEvent(this, args);
        }
    },
    RaiseCustomColorTableUpdated: function(){
        if(!this.CustomColorTableUpdated.IsEmpty()){
            var args = new ASPxClientEventArgs();
            this.CustomColorTableUpdated.FireEvent(this, args);
        }
    },
    SetColor: function(color, isAutomaticColor) {
        this.color = color;
        this.isAutomaticColorSelected = !!isAutomaticColor;

        var tablesColor = isAutomaticColor ? "" : color;
        var colorTable = this.GetColorTableControl();
        if(colorTable)
            colorTable.SetColor(tablesColor)
        var customColorTable = this.GetCustomColorTableControl();
        if(customColorTable)
            customColorTable.SetColor(tablesColor);

        this.UpdateAutomaticColorItem();
    },
    GetColor: function() {
        return this.color ? this.color.toUpperCase() : "";
    },
    GetValue: function() {
        return this.IsAutomaticColorSelected() ? this.GetAutomaticColorItemValue() : this.GetColor();
    },
    GetAutomaticColorItemValue: function() {
        return this.automaticColorItemValue;
    },
    GetAutomaticColor: function() {
        return this.automaticColor;
    },
    IsAutomaticColorSelected: function() {
        return this.isAutomaticColorSelected;
    }
});

ASPx.ColorNestedControl = ColorNestedControl;
})();