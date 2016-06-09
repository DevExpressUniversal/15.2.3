(function() {
var TreeListTableHelper = ASPx.CreateClass(ASPx.TableHelperBase, { 
    constructor: function(control, contentTableIDSuffix, headerTableIDSuffix, footerTableIDSuffix, horzScroll, vertScroll) {
        this.constructor.prototype.constructor.call(this, control, contentTableIDSuffix, headerTableIDSuffix, footerTableIDSuffix, horzScroll, vertScroll);
        this.indentWidth = -1;
    },

    GetArmatureCellIndexByOtherCell: function(cell) {
        var tree = this.control;
        if(tree.GetHeaderRow() !== cell.parentNode)
            ASPx.TableHelperBase.prototype.GetArmatureCellIndexByOtherCell.call(this, cell);

        var columnIndex = this.GetColumnIndexById(cell.id);
        return this.GetArmatureCellIndex(columnIndex);
    },

    GetArmatureCellIndex: function(columnIndex) {
        var indices = this.GetColumnIndices();
        var index = indices.indexOf ? indices.indexOf(columnIndex) : ASPx.Data.ArrayIndexOf(indices, columnIndex);
        return this.GetIndentCount() + index;
    },

    GetIndentCount: function() {
        return this.control.GetIndentCount();
    },

    GetColumnIndices: function() {
        return this.control.GetVisibleColumnIndices();
    },

    GetColumnIndexById: function(id) {
        var tree = this.control;
        var index = tree.GetLastNumberOfString(id);
        var column = tree.GetColumnByIndex(index);
        return column ? column.index : -1;
    },

    GetTableBody: function(table) {
        if(table.tBodies.length > 0)
            return table.tBodies[0];
        return table;
    },

    UpdateArmCells: function(newCellCount) {
        var tables = [this.GetHeaderTable(), this.GetContentTable(), this.GetFooterTable()];
        for(var i = 0; i < tables.length; i++) {
            var table = tables[i];
            if(!table) continue;

            var addNew = newCellCount > 0;
            var armCells = this.GetArmatureCells(table);
            for(var j = 0; j < Math.abs(newCellCount); j++) {
                if(addNew)
                    ASPx.InsertElementAfter(this.CreateIndentCell(), armCells[0]);
                else
                    ASPx.RemoveElement(armCells[0]);
            }
        }
    },

    CreateIndentCell: function() {
        var cell = document.createElement("TD");
        if(this.indentWidth >= 0)
            cell.style.width = this.indentWidth + "px";
        return cell;
    },

    UpdateIndentCellWidths: function() {
        if(!this.control.IsDisplayed())
            return;
        this.indentWidth = this.GetIndentCellWidth();
        var indentCount = this.GetIndentCount();
        var tables = [this.GetHeaderTable(), this.GetContentTable(), this.GetFooterTable()];
        for(var i = 0; i < tables.length; i++) {
            var table = tables[i];
            if(!table) continue;
            var armCells = this.GetArmatureCells(table);
            for(var j = 0; j < armCells.length; j++) {
                if(j >= indentCount) 
                    break;
                var cell = armCells[j];
                if(this.GetStylePxWidth(cell) != this.indentWidth && this.indentWidth > 0)
                    cell.style.width = this.indentWidth + "px";
            }
        }
    },

    GetIndentCellWidth: function() {
        var width = -1;
        var buttonImage = this.FindExpandCollapseButtonImage();
        if(buttonImage)
            width = buttonImage.offsetWidth + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(ASPx.GetParentByTagName(buttonImage, "TD"));
        width = Math.max(width, this.GetSelectionCellWidth());
        var indent = this.FindFirstIndent();
        if(indent)
            width = Math.max(width, ASPx.GetLeftRightBordersAndPaddingsSummaryValue(indent));
        return width || -1;
    },

    GetSelectionCellWidth: function() {
        var treeList = this.control;
        var checkBoxInput = treeList.FindSelectionCheck(treeList.GetUpdatableCell()) || treeList.FindSelectAllCheckBox();
        if(!checkBoxInput) return -1;
        var cell = ASPx.GetParentByTagName(checkBoxInput, "TD") || ASPx.GetParentByTagName(checkBoxInput, "TH");
        var checkBox = ASPx.GetNodeByTagName(cell, "SPAN", 0);
        return checkBox.offsetWidth + ASPx.GetLeftRightMargins(checkBox) + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(cell);
    },

    FindExpandCollapseButtonImage: function() {
        var tree = this.control;
        var table = this.GetContentTable();
        if(table.querySelectorAll) {
            var query = "img." + tree.ExpandButtonClassName + ", img." + tree.CollapseButtonClassName;
            var list = table.querySelectorAll(query);
            if(list.length > 0)
                return list[0];
            return null;
        }
        return this.FindElementInFirstIndentCells(function(cell) {
            for(var i = 0; i < cell.childNodes.length; i++) {
                var child = cell.childNodes[i];
                if(!child || child.tagName != "IMG") continue;
                if(child.className.indexOf(tree.ExpandButtonClassName) > -1 || child.className.indexOf(tree.CollapseButtonClassName) > -1)
                    return child;
            }
            return null;
        });
    },

    FindFirstIndent: function() {
        var tbody = this.GetTableBody(this.GetContentTable());
        if(tbody.querySelectorAll) {
            var query = ".dxtl__IM, .dxtl__IE";
            var list = tbody.querySelectorAll(query);
            if(list.length > 0)
                return list[0];
            return null;
        }
        return this.FindElementInFirstIndentCells(function(cell) { return cell; });
    },

    FindElementInFirstIndentCells: function(finder) {
        var tree = this.control;
        var table = this.GetContentTable();
        for(var i = 0; i < table.rows.length; i++) {
            var row = table.rows[i];
            if(!row || !tree.IsElementDataRow(row)) continue;
            var cell = row.cells[0];
            if(cell && tree.IsElementIndentCell(cell)) {
                var element = finder(cell);
                if(element) return element;
            }
        }
        return null;
    },

    GetExtraCellCount: function() {
        return this.HasHorzScroll() ? 1 : 0;
    },

    GetExtraCellWidth: function() {
        if(this.GetExtraCellCount() > 0) {
            var row = this.GetContentTable().rows[0];
            return row.cells[row.cells.length - 1].offsetWidth;
        }
        return 0;
    },

    GetCellRealWidth: function(cell) {
        var width = this.GetStylePxWidth(this.GetArmatureCell(cell));
        if(width > -1)
            return width;

        width = cell.offsetWidth;
        var tree = this.control;
        if(tree.GetHeaderRow() !== cell.parentNode)
            return width;

        var columnIndex = this.GetColumnIndexById(cell.id);
        var indices = this.GetColumnIndices();
        var index = indices.indexOf ? indices.indexOf(columnIndex) : ASPx.Data.ArrayIndexOf(indices, columnIndex);
        if(index == 0 && cell.colSpan > 1)
            width -= this.indentWidth * (cell.colSpan - 1);

        return width;
    },

    IsRtl: function() { return this.control.rtl; }
});

var TreeListTableScrollHelper = ASPx.CreateClass(ASPx.TableScrollHelperBase, {
    constructor: function(tableHelper) {
        this.constructor.prototype.constructor.call(this, tableHelper);

        this.savedScrollTop = 0;
        this.verticalScrollHandler = null;
    },

    GetResizingHelper: function() { return this.control.GetResizingHelper(); },

    SetScrollState: function(state) {
        this.control.stateObject.scrollState = state;
    },
    GetScrollState: function() {
        return this.control.stateObject.scrollState;
    },

    Update: function() {
        if(this.HasVertScroll())
            this.UpdateVerticalScrollHanlder();
        ASPx.TableScrollHelperBase.prototype.Update.call(this);
    },

    UpdateVerticalScrollHanlder: function() {
        if(this.verticalScrollHandler == null)
            this.verticalScrollHandler = function() { this.OnVerticalScroll(); }.aspxBind(this);

        ASPx.Evt.DetachEventFromElement(this.GetVertScrollableControl(), "scroll", this.verticalScrollHandler);
        ASPx.Evt.AttachEventToElement(this.GetVertScrollableControl(), "scroll", this.verticalScrollHandler);
    },

    OnVerticalScroll: function() {
        var scrollableControl = this.GetVertScrollableControl();
        if(this.savedScrollTop === scrollableControl.scrollTop)
            return;
        this.savedScrollTop = scrollableControl.scrollTop;

        this.control.dragHelper.OnVerticalScroll();
    }

});

var TreeListTableResizingHelper = ASPx.CreateClass(ASPx.TableResizingHelperBase, {
    constructor: function(tableHelper) {
        this.constructor.prototype.constructor.call(this, tableHelper);
    },

    IsNextColumnResizable: function() {
        return this.control.columnResizeMode == ASPx.ColumnResizeMode.NextColumn;
    },
    GetScrollHelper: function() {
        return this.control.GetScrollHelper();
    },
    GetHeaderCell: function(columnIndex) {
        return this.control.GetHeaderCell(columnIndex);
    },
    GetColumnIndices: function() {
        return this.tableHelper.GetColumnIndices();
    },
    GetColumnIndexById: function(id) {
        return this.tableHelper.GetColumnIndexById(id);
    },
    GetColumnMinWidth: function(columnIndex) {
        return this.control.GetColumnByIndex(columnIndex).minWidth;
    },
    GetArmatureCellIndex: function(columnIndex) {
        return this.tableHelper.GetArmatureCellIndex(columnIndex);
    },
    NeedResizeFooterTable: function() {
        return true;
    },
    OnResized: function(columnIndex) {
        var tree = this.control;
        tree.RaiseColumnResized(tree.GetColumnByIndex(columnIndex));
        tree.AssignEllipsisToolTips();
    },
    GetControlDimensions: function() {
        return this.control.stateObject.resizingState;
    },
    SetControlDimensions: function(dimensions) {
        this.control.stateObject.resizingState = dimensions;
    }
});

ASPx.TreeListTableHelper = TreeListTableHelper;
ASPx.TreeListTableScrollHelper = TreeListTableScrollHelper;
ASPx.TreeListTableResizingHelper = TreeListTableResizingHelper;
})();