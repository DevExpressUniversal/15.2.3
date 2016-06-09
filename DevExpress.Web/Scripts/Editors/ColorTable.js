/// <reference path="..\_references.js"/>

(function() {
var ColorTableIDSuffix = {
    ColorTable: "_CT",
    ColorCell: "_C"
}
var colorTableCellCssClassName = "dxctCell";
var colorTableCellDivCssClassName = "dxctCellDiv";

var ColorTable = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.colorColCount = 8;
        this.colorValues = [];
        this.selectedIndex = -1;
        this.usedInDropDown = false;

        this.colorTableCellStyleCssText = "";
        this.colorTableCellDivStyleCssText = "";
        this.colorTableCellStyleCssClassName = "";
        this.colorTableCellDivStyleCssClassName = "";

        this.ColorChanged = new ASPxClientEvent();
        this.ItemClick = new ASPxClientEvent();
    },
    Initialize: function() {
        this.constructor.prototype.Initialize.call(this);

        this.InitializeDefaultColors();
        var mainElement = this.GetMainElement();
        mainElement.unselectable = "on";
        mainElement.rows[0].cells[0].unselectable = "on";
        this.InitializeColorTableCellStyle();
        this.InitializeColorsTable();
        if(this.usedInDropDown)
            this.InitializeColorsTableEvents();
    },
    InitializeDefaultColors: function() {
        if(this.colorValues.length == 0)
            this.colorValues = ["#000000", "#993300", "#333300", "#003300", "#003366", "#000080", "#333399", "#333333",
                                "#800000", "#FF6600", "#808000", "#008000", "#008080", "#0000FF", "#666699", "#808080",
                                "#FF0000", "#FF9900", "#99CC00", "#339966", "#33CCCC", "#3366FF", "#800080", "#999999",
                                "#FF00FF", "#FFCC00", "#FFFF00", "#00FF00", "#00FFFF", "#00CCFF", "#993366", "#C0C0C0",
                                "#FF99CC", "#FFCC99", "#FFFF99", "#CCFFCC", "#CCFFFF", "#99CCFF", "#CC99FF", "#FFFFFF"
                                ];
    },
    InitializeColorTableCellStyle: function() {
        var styleSheet = ASPx.GetCurrentStyleSheet();
        ASPx.AddStyleSheetRule(styleSheet,
                "." + colorTableCellCssClassName + "_" + this.name, this.colorTableCellStyleCssText);
        ASPx.AddStyleSheetRule(styleSheet,
                "." + colorTableCellDivCssClassName + "_" + this.name, this.colorTableCellDivStyleCssText);

        var colorTableElement = this.GetColorsTableElement();
        var trElements = ASPx.GetNodesByTagName(colorTableElement, "TR");

        for(var i = 0; i < trElements.length; i++) {
            var tdElements = ASPx.GetNodesByTagName(trElements[i], "TD");
            for(var j = 0; j < tdElements.length; j++) {
                if(this.colorTableCellStyleCssText != "")
                    tdElements[j].className += " " + colorTableCellCssClassName + "_" + this.name;
                if(this.colorTableCellStyleCssClassName != "")
                    tdElements[j].className += " " + this.colorTableCellStyleCssClassName;

                var tdDiv = ASPx.GetNodesByTagName(tdElements[j], "DIV");
                if(tdDiv[0] != null) {
                    tdDiv.className = "";
                    if(this.colorTableCellDivStyleCssClassName != "")
                        tdDiv[0].className += " " + this.colorTableCellDivStyleCssClassName;
                    if(this.colorTableCellDivStyleCssText != "")
                        tdDiv[0].className += " " + colorTableCellDivCssClassName + "_" + this.name;
                }
            }
        }
    },
    InitializeColorsTable: function() {
        var colorsTable = this.GetColorsTableElement();
        colorsTable.unselectable = "on";
        var colorIndex = 0;
        for(var i = 0; i < colorsTable.rows.length; i++) {
            for(var j = 0; j < colorsTable.rows[i].cells.length; j++) {
                var colorCell = colorsTable.rows[i].cells[j];
                colorCell.id = this.GetColorCellElementID(colorIndex);
                colorCell.unselectable = "on";
                if(ASPx.Browser.WebKitFamily)
                    colorCell.cellIndex_Safari = j;

                var colorDiv = colorCell.firstChild;
                if(colorDiv && colorDiv.tagName == "DIV") {
                    colorDiv.style.backgroundColor = this.colorValues[colorIndex];
                    colorDiv.unselectable = "on";
                    colorIndex++;
                }
            }
        }
    },
    InitializeColorsTableEvents: function(listTable, method){
        var colorsTable = this.GetColorsTableElement();
        colorsTable.ColorsTableId = this.name;
        ASPx.Evt.AttachEventToElement(colorsTable, "mouseup", aspxCTMouseUp);
    },
    FindColorIndexByColor: function(colorValue) {
        if(colorValue == "")
            return -1;
        if(colorValue)
            colorValue = colorValue.toLowerCase();
        for(var i = 0; i < this.colorValues.length; i++) {
            if(this.colorValues[i].toLowerCase() == colorValue)
                return i;
        }
        return -1;
    },
    GetColorsTableElement: function() {
        return ASPx.GetElementById(this.name + ColorTableIDSuffix.ColorTable);
    },
    GetColorCellElementID: function(colorIndex) {
        return this.name + ColorTableIDSuffix.ColorCell + colorIndex.toString();
    },
    GetColorCellElementByIndex: function(colorIndex) {
        return ASPx.GetElementById(this.GetColorCellElementID(colorIndex));
    },
    GetColorCellDivByIndex: function(colorIndex) {
        return this.GetColorCellElementByIndex(colorIndex).firstChild;
    },
    SelectColorByIndex: function(colorIndex, fireEvent) {
        if(this.isColorSelected(colorIndex))
            return;
        var stateController = ASPx.GetStateController();
        this.DeselectColorByIndex(this.selectedIndex);
        this.selectedIndex = colorIndex;

        element = this.GetColorCellElementByIndex(this.selectedIndex);
        if(element != null)
            stateController.SelectElementBySrcElement(element);

        if(fireEvent)
            this.RaiseColorChanged();
    },
    isColorSelected: function(colorIndex) {
        return this.selectedIndex == colorIndex;
    },
    DeselectColorByIndex: function(colorIndex) {
        var stateController = ASPx.GetStateController();
        var element = this.GetColorCellElementByIndex(colorIndex);
        if(element != null)
            stateController.DeselectElementBySrcElement(element);
        this.selectedIndex = -1;
    },
    ClearSelection: function() {
        if(this.selectedIndex != -1)
            this.DeselectColorByIndex(this.selectedIndex);
    },
    OnMouseUp: function (clickedElement, htmlEvent) {
        if (clickedElement) {
            if (clickedElement.tagName == "DIV")
                clickedElement = clickedElement.parentNode;
            if (clickedElement.tagName == "TD") {
                var colorIndex = this.GetColorIndexByTdElement(clickedElement);
                if (this.AllowProcessItemClick(colorIndex)) {
                    this.ProcessItemClick(colorIndex);
                } else
                    this.OnEmptyCellClick(htmlEvent);
            }
        }
    },

    GetColorIndexByTdElement: function(element) {
        var cellIndex = !ASPx.Browser.WebKitFamily ? element.cellIndex : element.cellIndex_Safari;
        var rowIndex = element.parentNode.rowIndex;
        var colorIndex = rowIndex * this.colorColCount + cellIndex;
        return colorIndex;
    },
    GetSerializedColors: function() {
        return this.colorValues + "";
    },
    AllowProcessItemClick: function(colorIndex) {
        return 0 <= colorIndex && colorIndex < this.colorValues.length;
    },
    ProcessItemClick: function(colorIndex) {
        this.SelectColorByIndex(colorIndex, true);
        this.RaiseItemClick();
    },
    OnEmptyCellClick: function(evt) {
    },
    // TODO API
    RaiseColorChanged: function() {
        if(!this.ColorChanged.IsEmpty()) {
            var args = new ASPxClientEventArgs(false); // [Seleznev] No argument
            this.ColorChanged.FireEvent(this, args);
        }
    },
    RaiseItemClick: function() {
        if(!this.ItemClick.IsEmpty()) {
            var args = new ASPxClientEventArgs(false);
            this.ItemClick.FireEvent(this, args);
        }
    },
    GetColor: function() {
        if(0 <= this.selectedIndex && this.selectedIndex < this.colorValues.length)
            return this.colorValues[this.selectedIndex].toUpperCase();
        return "";
    },
    SetColor: function(value) {
        var colorIndex = this.FindColorIndexByColor(value);
        this.SelectColorByIndex(colorIndex, false);
        return colorIndex;
    }
});

var CustomColorTable = ASPx.CreateClass(ColorTable, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.stateSeparator = ',';
        this.emptyColorValue = "";

        this.EmptyCellClick = new ASPxClientEvent();
        this.AddColorEvent = new ASPxClientEvent();
    },
    InitializeDefaultColors: function() {
    },
    AllowProcessItemClick: function(colorIndex) {
        return ColorTable.prototype.AllowProcessItemClick.call(this, colorIndex) && this.colorValues[colorIndex] != this.emptyColorValue;
    },
    isColorSelected: function(colorIndex) {
        return false;
    },
    OnEmptyCellClick: function(evt) {
        this.RaiseEmptyCellClick();
    },
    RaiseEmptyCellClick: function() {
        if(!this.EmptyCellClick.IsEmpty()) {
            var args = new ASPxClientEventArgs(false);
            this.EmptyCellClick.FireEvent(this, args);
        }
    },
    RaiseAddColorEvent: function() {
        if(!this.AddColorEvent.IsEmpty()) {
            var args = new ASPxClientEventArgs(false);
            this.AddColorEvent.FireEvent(this, args);
        }
    },
    AddColor: function(color) {
        var itemIndex;
        if(this.colorValues.length == this.colorColCount) {
            for(var i = this.colorValues.length - 1; i > 0; i--) {
                this.colorValues[i] = this.colorValues[i-1];
                this.GetColorCellDivByIndex(i).style.backgroundColor 
					= this.GetColorCellDivByIndex(i - 1).style.backgroundColor;
            }
            this.colorValues[0] = color;
            itemIndex = 0;
        }
        else {
            itemIndex = this.colorValues.length;
            this.colorValues.push(color);
        }
        this.GetColorCellDivByIndex(itemIndex).style.backgroundColor = color;
        this.RaiseAddColorEvent();
    }
});

function aspxCTMouseUp(evt) {
    var element = ASPx.Evt.GetEventSource(evt);
    while(element != null && element.tagName != "BODY") {
        if(element.tagName == "TR") {
            var table = element.offsetParent;
            if(table && table.ColorsTableId) {
                var ct = ASPx.GetControlCollection().Get(table.ColorsTableId);
                if(ct != null && ASPx.Evt.IsLeftButtonPressed(evt))
                    ct.OnMouseUp(ASPx.Evt.GetEventSource(evt), evt);
                break;
            }
        }
        element = element.parentNode;
    }
}

ASPx.ColorTable = ColorTable;
ASPx.CustomColorTable = CustomColorTable;
})();