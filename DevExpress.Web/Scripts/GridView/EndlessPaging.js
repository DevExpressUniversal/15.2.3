/// <reference path="..\_references.js"/>

(function() {
var GridBaseEndlessPagingHelper = ASPx.CreateClass(null, {
    constructor: function(grid) {
        this.grid = grid;
        this.showLoadingPanelAtBottom = false;
        this.focusedRowIndexInfo =  { 
            beforeCallback: -1,
            underCallback: -1,
            afterCallback: -1
        };
        this.endlessCallbackComplete = false;
    },

    GetItemsContainer: function(){ return this.grid.GetMainTable(); },
    GetItemElements: function(){ 
        var container = this.GetItemsContainer();
        return container.tagName == "TABLE" ? container.rows : container.childNodes; 
    },

    OnBeforeCallback: function(command) {
        this.endlessCallbackComplete = false;
        this.showLoadingPanelAtBottom = command === ASPxClientGridViewCallbackCommand.NextPage;

        this.focusedRowIndexInfo.beforeCallback = this.focusedRowIndexInfo.underCallback = this.GetFocusedItemIndex();
        this.focusedRowIndexInfo.afterCallback = -1;
    },
    OnAfterCallback: function() {
        var grid = this.grid;
        var lpContainer = grid.GetEndlessPagingLPContainer();
        if(lpContainer)
            ASPx.SetElementDisplay(lpContainer, grid.pageIndex + 1 != grid.pageCount);
        this.CorrectItemIDs();

        this.focusedRowIndexInfo.afterCallback = this.GetFocusedItemIndex();
        this.CheckFocusedItemIndexChangedOnCallback();
    },
    GetFocusedItemIndex: function(){
        return -1;
    },

    CheckFocusedItemIndexChangedOnCallback: function() {
        var info = this.focusedRowIndexInfo;
        if(info.beforeCallback === info.underCallback && info.underCallback === info.afterCallback)
            return;

        if(info.beforeCallback !== info.afterCallback) {
            this.grid.ChangeFocusedItemStyle(info.beforeCallback, false);
            this.grid.ChangeFocusedItemStyle(info.underCallback, false);
        } else if(info.beforeCallback !== info.underCallback) {
            this.grid.ChangeFocusedItemStyle(info.beforeCallback, false);
            this.grid.ChangeFocusedItemStyle(info.underCallback, true);

            this.grid.focusedRowIndex = info.underCallback;
        }
        this.grid._setFocusedItemInputValue();
    },

    OnGridFocusedItemChanged: function() { //TODO remove
        if(this.grid.InCallback())
            this.focusedRowIndexInfo.underCallback = this.GetFocusedItemIndex();
    },

    CorrectItemIDs: function() {
        var grid = this.grid;
        var itemElements = this.GetItemElements();
        if(itemElements.length == 0) return;
        var index = -1;

        var editingRow = grid.GetEditingRow();
        for(var i = 0; i < itemElements.length; i++) {
            var item = itemElements[i];
            var id = item.id;
            if(!id) continue;

            if(editingRow && item == editingRow && grid.IsEditRowHasDisplayedDataRow() && !grid.IsNewItemEditing()) {
                index++;
                continue;
            }

            var regEx = grid.GetItemVisibleIndexRegExp();
            var matches = regEx.exec(id);
            if(matches && matches.length == 3) {
                if(this.IsDataItemID(id))
                    index++;
                var visibleIndex = parseInt(matches[2]);
                if(visibleIndex != index)
                    item.id = id.replace(regEx, "$1" + index);
            }
        }
    },
    IsDataItemID: function(id){
        return false;
    },
    
    NeedShowLoadingPanelAtBottom: function(){
        return this.showLoadingPanelAtBottom && this.grid.GetEndlessPagingLPContainer();
    },

    OnCallback: function(str) {
        if(!this.grid.GetMainElement())
            return;
        var result = eval(str.slice(str.indexOf("|") + 1));

        this.UpdateKeyValues(result.pageKeys);
        this.UpdateGroupState(result.groupState);

        if(result.removeEditForm)
            this.RemoveEditFormRow();
        if(result.html)
            ASPx.SetInnerHtml(this.grid.GetEndlessPagingUpdatableContainer(), result.html);
        
        this.UpdateDataItems(result.dataTableInfo);

        this.grid.cButtonIDs = result.cButtonIDs;
        this.endlessCallbackComplete = true;

        this.grid.UpdateItemsStyle();
    },

    UpdateDataItems: function(updateInfo) {
        if(!updateInfo || updateInfo.length == 0)
            return;
        for(var i = 0; i < updateInfo.length; i++)
            this.UpdateDataItemsCore(updateInfo[i]);
    },

    UpdateDataItemsCore: function(updateInfo) {
        if(!updateInfo) 
            return;
        var removeIndex = updateInfo[0];
        var removeCount = updateInfo[1];
        var addIndex = updateInfo[2];
        var html = updateInfo[3];

        this.RemoveDataItems(removeIndex, removeCount);
        if(addIndex < 0 || !html)
            return;

        var startItem = null;
        var visibleIndex = addIndex;
        do {
            startItem = this.grid.GetItem(visibleIndex);
        } while(!startItem && ++visibleIndex < this.grid.pageRowCount)

        var itemElements = this.GetItemElements();
        var startRowIndex = startItem ? ASPx.Data.ArrayIndexOf(itemElements, startItem) : itemElements.length;
        this.InsertItems(this.GetItemsContainer(), html, startRowIndex);
    },

    RemoveDataItems: function(removeIndex, removeCount) {
        if(removeIndex < 0 || removeCount <= 0)
            return;
        var startItem = this.grid.GetItem(removeIndex);
        if(!startItem) {
            startItem = this.grid.GetItem(++removeIndex);
            removeCount--;
        }
        var endItem = null;
        var visibleIndex = removeIndex + removeCount;
        do {
            endItem = this.grid.GetItem(visibleIndex);
        } while(!endItem && ++visibleIndex < this.grid.pageRowCount)

        var itemElements = this.GetItemElements();
        var startRowIndex = ASPx.Data.ArrayIndexOf(itemElements, startItem);
        var endRowIndex = endItem ? ASPx.Data.ArrayIndexOf(itemElements, endItem) : itemElements.length;

        for(var i = endRowIndex - 1; i >= startRowIndex && startRowIndex >= 0; i--)
            ASPx.RemoveElement(itemElements[i]);
    },

    InsertItems: function(container, rowsHtml, index) {
        if(!container || container.tagName != "TABLE")
            return;
        this.InsertRows(container, rowsHtml, index);
    },

    InsertRows: function(table, rowsHtml, index) {
        if(ASPx.Browser.IE && ASPx.Browser.Version < 10) {
            this.InsertRows_IE(table, rowsHtml, index);
            return;
        }
        var row;
        if(index >= 0 && index < table.rows.length) { 
            row = table.rows[index];
        } else if(table.tBodies.length > 0) {
            row = document.createElement("TR");
            table.tBodies[0].appendChild(row);
            row.shouldRemove = true;
        }
        if(row) {
            row.insertAdjacentHTML("beforeBegin", rowsHtml);
            if(row.shouldRemove)
                ASPx.RemoveElement(row);
        }
    },
    InsertRows_IE: function(table, rowsHtml, index) {
        var row = document.createElement("TR");
        var cell = document.createElement("TD");
        cell.innerHTML = "<table><tbody>" + rowsHtml + "</tbody></table>";

        var tbody = table.tBodies[0];
        tbody.appendChild(row);
        row.appendChild(cell);
        
        var newTable = ASPx.GetNodeByTagName(cell, "TABLE", 0);
        var rowCount = newTable.rows.length;

        var nextRow = null;
        if(index >= 0 && index < table.rows.length)
            nextRow = table.rows[index];

        for(var i = rowCount - 1; i >= 0; i--) {
            var newRow = newTable.rows[i];
            if(nextRow == null)
                tbody.appendChild(newRow);
            else
                tbody.insertBefore(newRow, nextRow);
            nextRow = newRow;
        }
        ASPx.RemoveElement(row);
    },

    RemoveEditFormRow: function() {
        ASPx.RemoveElement(this.grid.GetEditingRow());
        ASPx.RemoveElement(this.grid.GetEditingErrorRow());
    },

    UpdateKeyValues: function(updateInfo) {
        this.UpdateArray(this.grid.keys, updateInfo);
        this.grid.stateObject.keys = this.grid.keys;
        this.grid.EnsureRowKeys();
    },

    UpdateGroupState: function(updateInfo) {
        this.UpdateArray(this.grid.stateObject.endlessPagingGroupState, updateInfo);
    },

    UpdateArray: function(array, updateInfo) {
        if(!updateInfo)
            return;
        var removeIndex = updateInfo[0];
        var removeCount = updateInfo[1];
        var addIndex = updateInfo[2];
        var newArray = updateInfo[3];

        for(var i = 0; i < removeCount; i++)
            ASPx.Data.ArrayRemoveAt(array, removeIndex);
        for(var i = 0; i < newArray.length; i++)
            ASPx.Data.ArrayInsert(array, newArray[i], addIndex + i);
    }
});

var CardViewEndlessPagingHelper = ASPx.CreateClass(GridBaseEndlessPagingHelper, {
    constructor: function(grid){
        this.constructor.prototype.constructor.call(this, grid);
        grid.FocusedCardChanged.AddHandler(function() { this.OnGridFocusedItemChanged(); }.aspxBind(this));
    },

    IsFlowLayout: function(){
        var mainElement = this.grid.GetMainTable();
        return mainElement && ASPx.ElementHasCssClass(mainElement, "dxcvFT");
    },
    GetItemsContainer: function(){
        if(this.IsFlowLayout())
            return this.grid.GetMainTable().rows[0].cells[0];
        return GridBaseEndlessPagingHelper.prototype.GetItemsContainer.call(this);
    },
    InsertItems: function(container, itemsHtml, index) {
        if(!this.IsFlowLayout()){
            GridBaseEndlessPagingHelper.prototype.InsertItems.call(this, container, itemsHtml, index);
            return;
        }

        var item;
        if(index >= 0 && index < container.childNodes.length)
            item = container.childNodes[index];
        item && item.insertAdjacentHTML("beforeBegin", itemsHtml) || container.insertAdjacentHTML("beforeEnd", itemsHtml);
    },
    IsDataItemID: function(id){
        return id.indexOf(this.grid.CardID) > 0;
    },
    GetFocusedItemIndex: function(){
        return this.grid.GetFocusedCardIndex();
    }
});
var GridViewEndlessPagingHelper = ASPx.CreateClass(GridBaseEndlessPagingHelper, {
    constructor: function(grid){
        this.constructor.prototype.constructor.call(this, grid);
        grid.FocusedRowChanged.AddHandler(function() { this.OnGridFocusedItemChanged(); }.aspxBind(this));
    },
    GetFocusedItemIndex: function(){
        return this.grid.GetFocusedRowIndex();
    },
    IsDataItemID: function(id){
        return id.indexOf(this.grid.GroupRowID) > 0 || id.indexOf(this.grid.DataRowID) > 0;
    }
});

ASPx.GridViewEndlessPagingHelper = GridViewEndlessPagingHelper;
ASPx.CardViewEndlessPagingHelper = CardViewEndlessPagingHelper;
})();