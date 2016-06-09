/// <reference path="..\_references.js"/>
(function() {
    var constants = {
        InsertTableControlPostfix: "_ITC",
        InsertTableControlTablePostfix: "_ITCT",
        InsertTableControlCaptionPostfix: "_ITCC",

        InsertTableItemClass: "dxitcItem",
        InsertTableItemHoverClass: "dxitcItemHover"
    };

    var InsertTableControlTableInsertedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(rowCount, columnCount) {
            this.constructor.prototype.constructor.call(this);
            this.rowCount = rowCount;
            this.columnCount = columnCount;
        }
    });

    var InsertTableControl = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);

            this.columnCount = null;
            this.rowCount = null;
            this.insertTableText = null;

            this.cells = [];
            this.currentCell = null;
            this.currentX = 0;
            this.currentY = 0;

            this.TableInserted = new ASPxClientEvent();
        },
        Initialize: function() {
            this.InitializeTable();
            this.InitializeEventHandlers();
            this.ChangeCaption();
            ASPxClientControl.prototype.Initialize.call(this);
        },
        InitializeTable: function() {
            var tableElement = this.GetTableElement();
            var table = ASPx.CreateHtmlElement("table");
            tableElement.appendChild(table);
            ASPx.GetStateController().AddHoverItem(tableElement.id, [""], [""], [""], null, null, false);

            for(var i = 0; i < this.rowCount; i++) {
                var row = ASPx.CreateHtmlElement("tr");
                table.appendChild(row);
                for(var j = 0; j < this.columnCount; j++) {
                    var cellContainer = ASPx.CreateHtmlElement("td");
                    row.appendChild(cellContainer);
                    this.AddCell(cellContainer, i, j);
                }
            }
        },
        InitializeEventHandlers: function() {
            ASPx.Evt.AttachEventToElement(this.GetTableElement(), "mouseup", function(evt) { this.OnTableMouseUp(evt); }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(this.GetTableElement(), "mousemove", function(evt) { this.OnTableMouseMove(evt); }.aspxBind(this));
            ASPx.AddAfterClearHoverState(function(s, e) { this.OnTableMouseOut(e); }.aspxBind(this))
        },
        OnTableMouseUp: function(evt) {
            if(!ASPx.Evt.IsLeftButtonPressed(evt))
                return;
            var cell = this.GetCellByEvent(evt);
            if(cell)
                this.RaiseTableInserted(cell.GetX(), cell.GetY());
        },
        OnTableMouseMove: function(evt) {
            var cell = this.GetCellByEvent(evt);
            if(cell != this.currentCell) {
                this.currentCell = cell;
                this.ChangeSelection();
            }
        },
        OnTableMouseOut: function(evt) {
            if(evt.element == this.GetTableElement() && this.currentCell) {
                this.currentCell = null;
                this.ChangeSelection();
            }
        },
        RaiseTableInserted: function(rowCount, columnCount) {
            if(!this.TableInserted.IsEmpty()) {
                var args = new InsertTableControlTableInsertedEventArgs(rowCount, columnCount);
                this.TableInserted.FireEvent(this, args);
            }
        },
        RaiseRibbonExecCommand: function(ribbonName, ribbonItemName, rowCount, columnCount) {
            var params = { rowCount: rowCount, cellCount: columnCount };
            var ribbon = ASPx.GetControlCollection().GetByName(ribbonName);
            var item = ribbon.GetItemByName(ribbonItemName);
            item.hidePopup();
            ribbon.onExecCommand(item, params);
        },
        AddCell: function(parentElement, x, y) {
            var cell = new TableCell(this, parentElement, x, y);
            this.cells[cell.id] = cell;
        },
        ChangeSelection: function() {
            var x = this.currentCell ? this.currentCell.GetX() : 0;
            var y = this.currentCell ? this.currentCell.GetY() : 0;

            var minX = Math.min(this.currentX, x);
            var maxX = Math.max(this.currentX, x);
            var minY = Math.min(this.currentY, y);
            var maxY = Math.max(this.currentY, y);

            for(var i = 0; i < minX; i++)
                for(var j = minY; j < maxY; j++)
                    this.ChangeCellClass(i, j, i < x && j < y);
            for(var i = minX; i < maxX; i++)
                for(var j = 0; j < minY; j++)
                    this.ChangeCellClass(i, j, i < x && j < y);
            for(var i = minX; i < maxX; i++)
                for(var j = minY; j < maxY; j++)
                    this.ChangeCellClass(i, j, i < x && j < y);

            this.currentX = x;
            this.currentY = y;

            this.ChangeCaption();
        },
        ChangeCaption: function() {
            var captionElement = this.GetCaptionElement();
            var text = this.currentX > 0 ? this.currentX + " x " + this.currentY : this.insertTableText;
            ASPx.SetInnerHtml(captionElement, text);
        },
        ChangeCellClass: function(rowIndex, columnIndex, isHover) {
            var element = ASPx.GetElementById(this.name + "_" + rowIndex + "_" + columnIndex);
            if(isHover)
                ASPx.AddClassNameToElement(element, constants.InsertTableItemHoverClass);
            else
                ASPx.RemoveClassNameFromElement(element, constants.InsertTableItemHoverClass);
        },
        GetCellByEvent: function(evt) {
            var srcElement = ASPx.Evt.GetEventSource(evt);
            if(!srcElement.id)
                srcElement = ASPx.GetChildByTagName(srcElement, "DIV", 0);
            return srcElement ? this.cells[srcElement.id] : null;
        },
        GetMainElement: function() {
            return ASPx.GetElementById(this.name + constants.InsertTableControlPostfix);
        },
        GetTableElement: function() {
            return ASPx.GetElementById(this.name + constants.InsertTableControlTablePostfix);
        },
        GetCaptionElement: function() {
            return ASPx.GetElementById(this.name + constants.InsertTableControlCaptionPostfix);
        },

        getEnabledCore: function() {
            return true;
        }
    });

    var TableCell = ASPx.CreateClass(null, {
        constructor: function(owner, parentElement, rowIndex, columnIndex) {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.id = owner.name + "_" + rowIndex + "_" + columnIndex;

            this.CreateElement(parentElement);
        },
        CreateElement: function(parentElement) {
            var element = ASPx.CreateHtmlElement();
            element.id = this.id;
            ASPx.AddClassNameToElement(element, constants.InsertTableItemClass);
            parentElement.appendChild(element);
        },
        GetX: function() {
            return this.rowIndex + 1;
        },
        GetY: function() {
            return this.columnIndex + 1;
        }
    });

    ASPx.InsertTableControl = InsertTableControl;
})();