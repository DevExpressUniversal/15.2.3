/// <reference path="..\_references.js"/>

(function() {
var GridViewTableHelper = ASPx.CreateClass(ASPx.TableHelperBase, {
    GridDetailIndentClassName: "dxgvDI",
    GridGroupIndentClassName: "dxgvGI",
    constructor: function(control, contentTableIDSuffix, headerTableIDSuffix, footerTableIDSuffix, horzScroll, vertScroll) {
        this.constructor.prototype.constructor.call(this, control, contentTableIDSuffix, headerTableIDSuffix, footerTableIDSuffix, horzScroll, vertScroll);
    },

    GetArmatureCellIndexByOtherCell: function(cell) {
        var grid = this.control;
        if(!grid.IsHeaderRow(cell.parentNode))
            ASPx.TableHelperBase.prototype.GetArmatureCellIndexByOtherCell.call(this, cell);

        var columnIndex = grid.getColumnIndex(cell.id);
        return this.GetArmatureCellIndex(columnIndex);
    },

    GetArmatureCellIndex: function(columnIndex) {
        var grid = this.control;
        return grid.indentColumnCount + grid.GetHeaderMatrix().GetLeafIndex(columnIndex);
    },

    UpdateIndentCellWidths: function() {
        var indentCount = this.control.indentColumnCount;
        if(indentCount == 0)
            return;
        var info = this.GetIndentsWidth(indentCount);
        if(info.group <= 0 && info.detail <= 0)
            return;
        var tables = [this.GetHeaderTable(), this.GetContentTable(), this.GetFooterTable()];
        for(var i = 0; i < tables.length; i++) {
            var table = tables[i];
            if(!table) continue;
            var armCells = this.GetArmatureCells(table);
            for(var j = 0; j < armCells.length; j++) {
                if(j >= indentCount) 
                    break;
                var width = j == indentCount - 1 && info.detail > 0 ? info.detail : info.group;
                var cell = armCells[j];
                if(this.GetStylePxWidth(cell) != width)
                    cell.style.width = width + "px";
            }
        }
    },

    GetIndentsWidth: function(indentCount) {
        var grid = this.control;

        var headerRow = grid.GetHeaderRow(0);
        var groupRow = this.FindFirstVisibleRow(true);
        var dataRow = this.FindFirstVisibleRow();

        var groupButton = this.FindExpandCollapseButton(headerRow, 0) || this.FindExpandCollapseButton(dataRow, 0);
        var detailButton = this.FindExpandCollapseButton(headerRow, indentCount - 1, true) || this.FindExpandCollapseButton(dataRow, indentCount - 1, true);

        return { group : this.GetButtonWidth(groupButton), detail : this.GetButtonWidth(detailButton) };
    },

    FindExpandCollapseButton: function(row, cellIndex, isDetail) {
        if(!row || row.cells.length === 0) return;
        var rowCells = row.cells;
        var armCells = this.GetArmatureCells(row.parentNode.parentNode);
        return this.FindExpandCollapseButtonCore(rowCells[cellIndex], armCells[cellIndex], isDetail);
    },

    FindExpandCollapseButtonCore: function(cell, armCell, isDetail) {
        if(!cell || !armCell) return;

        var hasDetail = armCell.className.indexOf(this.GridDetailIndentClassName) >= 0 && isDetail;
        var hasGroup = armCell.className.indexOf(this.GridGroupIndentClassName) >= 0 && !isDetail;
        if(!hasDetail && !hasGroup) return;

        return ASPx.GetChildByTagName(cell, "IMG");
    },

    FindFirstVisibleRow: function(isGroup) {
        var grid = this.control;
        var start = grid.GetTopVisibleIndex();
        var end = start + grid.GetVisibleRowsOnPage();
        for(var i = start; i < end; i++) {
            var row = isGroup ? grid.GetGroupRow(i) : grid.GetDataRow(i);
            if(row) return row;
        }
    },

    GetButtonWidth: function(button) {
        if(!button) return -1;
        return button.offsetWidth + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(button.parentNode);
    },

    IsRtl: function() { return this.control.rtl; }

});

var virtualScrollingReserveSizeFactor = 0.5;
var GridViewTableResizingHelper = ASPx.CreateClass(ASPx.TableResizingHelperBase, {
    constructor: function(tableHelper) {
        this.constructor.prototype.constructor.call(this, tableHelper);
    },

    IsNextColumnResizable: function() { 
        return this.control.columnResizeMode == ASPx.ColumnResizeMode.NextColumn;
    },
    IsRightmostColumn: function(columnIndex) {
        return !ASPx.IsExists(this.control.GetHeaderMatrix().GetRightNeighbor(columnIndex, true));
    },
    GetScrollHelper: function() { 
        return this.control.GetScrollHelper(); 
    },
    GetHeaderCell: function(columnIndex) { 
        return this.control.GetHeader(columnIndex); 
    },
    GetColumnIndices: function() { 
        return this.control.GetHeaderMatrix().GetLeafIndices(); 
    },
    GetColumnIndexById: function(id) { 
        return this.control.getColumnIndex(id); 
    },
    GetColumnMinWidth: function(columnIndex) {
        return this.control.columns[columnIndex].minWidth;
    },
    GetArmatureCellIndex: function(columnIndex) {
        return this.tableHelper.GetArmatureCellIndex(columnIndex);
    },
    GetColumnNeighbor: function(columnIndex, isLeft) {
        var matrix = this.control.GetHeaderMatrix();
        return matrix.GetLeaf(columnIndex, isLeft, true);
    },
    GetResizingColumnIndexCore: function(columnIndex, isLeft) {
        var matrix = this.control.GetHeaderMatrix();
        return matrix.GetLeaf(columnIndex, isLeft, isLeft);
    },
    NeedResizeFooterTable: function() {
        return !this.control.hasFooterRowTemplate;
    },
    OnResized: function(columnIndex) {
        this.control.RaiseColumnResized(this.control.columns[columnIndex]);
        this.control.AssignEllipsisToolTips();
    },
    GetControlDimensions: function() {
        return this.control.stateObject.resizingState;
    },
    SetControlDimensions: function(dimensions) {
        this.control.stateObject.resizingState = dimensions;
    },
	CanStartResizing: function(e, headerCell) {
        if(this.control.adaptivityMode === 2 && this.control.hasAdaptiveElements)
            return false;

        var dragHelper = this.control.GetDragHelper();
        return ASPx.TableResizingHelperBase.prototype.CanStartResizing.call(this, e, headerCell) && dragHelper.IsDataHeaderTarget(headerCell);
    }
});

var GridViewTableScrollHelper = ASPx.CreateClass(ASPx.TableScrollHelperBase, {
    constructor: function(tableHelper) {
        this.constructor.prototype.constructor.call(this, tableHelper);

        this.isVirtualScrolling = this.control.isVirtualScrolling;
        this.isVirtualSmoothScrolling = this.control.isVirtualSmoothScrolling;
        this.requireRetainVerticalScrollPosition = false;
        this.lockVirtualScrolling = false;
        this.virtualScrollRowHeight = -1;
        this.virtualScrollPrevPos = -1;
        this.virtualScrollTimerID = -1;
        this.virtualScrollHandler = null;

        this.useEndlessPaging = this.control.useEndlessPaging;
        this.allowFixedGroups = this.control.allowFixedGroups;
    },

    Update: function() {
        if(this.isVirtualScrolling)
            this.UpdateVirtualScrolling();

        if(this.useEndlessPaging)
            this.UpdateEndlessPaging();

        if(this.allowFixedGroups)
            this.UpdateFixedGroups();
        ASPx.TableScrollHelperBase.prototype.Update.call(this);
    },
    
    UpdateScrollableControlsSize: function(onResize) {
        var hasFixedColumns = this.HasFixedColumns();
        if(onResize && hasFixedColumns)
            return;
        if(hasFixedColumns)
            this.CollapseFixedDivs();
        ASPx.TableScrollHelperBase.prototype.UpdateScrollableControlsSize.call(this, onResize);
        if(hasFixedColumns)
            this.RestoreFixedDivSize();
    },

    RequireChangeScrollDivWidth: function() { return this.control.adaptivityMode !== 1; },

    GetFixedDivs: function() {
        var fixedHelper = this.control.GetFixedColumnsHelper();
        if(!fixedHelper)
            return [];
        var fixedDiv = fixedHelper.GetFixedDiv();
        var divs = [fixedDiv, this.GetScrollDiv(this.GetContentTable())];
        return divs;
    },

    CollapseFixedDivs: function() {
        var divs = this.GetFixedDivs();
        for(var i = 0; i < divs.length; i++)
            this.SetElementWidth(divs[i], 1);

    },

    RestoreFixedDivSize: function() {
        var fixedHelper = this.control.GetFixedColumnsHelper();
        if(!fixedHelper) return;
        var fixedDiv = fixedHelper.GetFixedDiv();
        var mainTable = this.control.GetMainElement();
        var divs = this.GetFixedDivs();
        if(this.IsOriginalWidthPercentage(mainTable))
            ASPx.RestoreElementOriginalWidth(mainTable);

        var width = mainTable.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainTable);
        if(width < 0) 
            width = 0;
        for(var i = 0; i < divs.length; i++)
            this.SetElementWidth(divs[i], width);
        
        fixedHelper.Update();

        if(ASPx.Browser.IE && ASPx.Browser.Version <= 9) // IE doesn't raise scroll event on scrollable div width changing
            fixedHelper.UpdateCellsVisibility(fixedHelper.GetColumnsToHide(fixedDiv.scrollLeft));
    },

    SetHeight: function(val) {
        if(this.useEndlessPaging)
            this.CheckEndlessPagingLoadNextPage();
        ASPx.TableScrollHelperBase.prototype.SetHeight.call(this, val);
    },
    
    HasFixedColumns: function() {
        return this.control.fixedColumnCount > 0 && this.control.GetFixedColumnsDiv();
    },
    
    IsContentTableEmpty: function() {
        return this.control.pageRowCount < 1;
    },
    ShowTouchHorizontalScrollbar: function() {
        return this.HasHorzScroll() && !this.HasFixedColumns();
    },
    UseTouchCustomScroll: function() {
        return this.HasFixedColumns();
    },

    SetScrollState: function(position) {
        this.control.stateObject.scrollState = position;
    },
    GetScrollState: function() {
        return this.control.stateObject.scrollState;
    },

    AssignPosLoggerHandler: function(element) {
        if(this.HasVertScroll() || !this.HasFixedColumns())
            ASPx.TableScrollHelperBase.prototype.AssignPosLoggerHandler.call(this, element);
        if(this.HasFixedColumns())
            ASPx.TableScrollHelperBase.prototype.AssignPosLoggerHandler.call(this, this.control.GetFixedColumnsDiv());
    },

    GetHorzScrollableControl: function() {
        if(this.HasFixedColumns())
            return this.control.GetFixedColumnsDiv();
        return ASPx.TableScrollHelperBase.prototype.GetHorzScrollableControl.call(this);
    },

    IsRestoreScrollPosition: function(){
        var grid = this.control;
        if(this.savedScrollTop > -1 && this.savedScrollTop !== grid.GetVerticalScrollPosition())
            return true;
        if(this.savedScrollLeft > -1 && this.savedScrollLeft !== grid.GetHorizontalScrollPosition())
            return true;
        return false;
    },

    LoadScrollPosition: function() {
        ASPx.TableScrollHelperBase.prototype.LoadScrollPosition.call(this);

        var grid = this.control;
        var newRowVertScrollPos = this.GetNewRowVertScrollPos();
        if(newRowVertScrollPos > -1)
            this.savedScrollTop = newRowVertScrollPos;
        if(grid.rtl && this.HasFixedColumns() && this.savedScrollLeft == 0)
            this.savedScrollLeft = 1;
    },

    ApplyScrollPosition: function() {
        if(!this.isVirtualScrolling && this.savedScrollTop > -1)
            this.SetVertScrollPosition(this.savedScrollTop);
        if(this.savedScrollLeft > -1)
            window.setTimeout(function() { this.SetHorzScrollPosition(this.savedScrollLeft); }.aspxBind(this), 0);
    },

    SetScrollPositionCore: function(element, pos, isTop) {
        this.lockVirtualScrolling = true;
        try {
            ASPx.TableScrollHelperBase.prototype.SetScrollPositionCore.call(this, element, pos, isTop);
            if(isTop) {
                var popup = this.control.GetPopupEditForm();
                if(popup && popup.GetVisible())
                    popup.UpdatePosition();
            }
        } finally {
            this.lockVirtualScrolling = false;
        }
    },

    GetNewRowVertScrollPos: function() {
        var grid = this.control;
        if(grid.IsNewRowEditing()) {
            var row = grid.GetEditingRow();
            if(row)
                return !grid.IsNewRowAtBottom() ? 0 : 0x1fffffff;
        }
        return -1;
    },

    GetResizingHelper: function() { return this.control.GetResizingHelper(); },

    // Virtual Scrolling
    UpdateVirtualScrolling: function() {
        this.UpdateVirtualScrollMargins();
        if(!this.virtualScrollHandler) 
            this.virtualScrollHandler = function() { this.OnVirtualScroll(); }.aspxBind(this);
        window.setTimeout(function() { 
            ASPx.Evt.AttachEventToElement(this.GetVertScrollableControl(), "scroll", this.virtualScrollHandler); 
        }.aspxBind(this), 0);
    },

    UpdateVirtualScrollMargins: function() {
        var grid = this.control;
        if(grid.pageCount < 1)
            return;

        var table = this.GetContentTable();
        var topDiv, bottomDiv;

        var prev;
        for(var i = 0; i < table.parentNode.childNodes.length; i++) {
            var child = table.parentNode.childNodes[i];
            if(child.nodeType != 1)
                continue;
            if(child == table && prev)
                topDiv = prev;
            if(prev == table) {
                bottomDiv = child;
                break;
            }
            prev = child;
        }

        var rowHeight = this.GetVirtualScrollRowHeight();

        var topDivHeight = grid.visibleStartIndex * rowHeight;
        var bottomDivHeight = (grid.pageRowSize * (grid.pageCount - grid.pageIndex - 1)) * rowHeight;
        if(this.isVirtualSmoothScrolling) {
            topDivHeight -= this.GetTopReserveRowsCount() * rowHeight;
            bottomDivHeight -= this.GetBottomReserveRowsCount() * rowHeight;
            if(grid.IsLastPage())
                topDivHeight -= (grid.pageRowSize - grid.pageRowCount) * rowHeight;
        } else {
            bottomDivHeight += (grid.pageRowSize - grid.pageRowCount) * rowHeight;
        }

        this.PrepareVirtualScrollMarginDiv(topDiv, topDivHeight);
        this.PrepareVirtualScrollMarginDiv(bottomDiv, bottomDivHeight);

        var newVertScrollPosition;
        if(!this.isVirtualSmoothScrolling) 
            newVertScrollPosition = topDivHeight;
        else if(this.requireRetainVerticalScrollPosition)
            newVertScrollPosition = this.virtualScrollPrevPos;
        else if(grid.IsLastPage())
            newVertScrollPosition = topDivHeight + (this.GetTopReserveRowsCount() + grid.pageRowSize) * rowHeight;
        else
            newVertScrollPosition = topDivHeight + this.GetTopReserveRowsCount() * rowHeight;
        window.setTimeout(function() {
            this.SetVertScrollPosition(newVertScrollPosition);
            this.virtualScrollPrevPos = newVertScrollPosition;
            this.requireRetainVerticalScrollPosition = false;
        }.aspxBind(this), 0);
    },
    GetTopReserveRowsCount: function() {
        return this.control.pageIndex === 0 ? 0 : this.GetCommonReserveRowsCount();
    },
    GetBottomReserveRowsCount: function() {
        return this.control.pageIndex === this.control.pageCount - 1 ? 0 : this.GetCommonReserveRowsCount();
    },
    GetCommonReserveRowsCount: function() {
        return Math.round(this.control.pageRowSize * virtualScrollingReserveSizeFactor);
    },

    PrepareVirtualScrollMarginDiv: function(div, height) {
        if(!div) return;
        
        var maxPieceHeight = 1100000;
        if(height <= maxPieceHeight) {
            div.style.height = height + "px";
        } else {
            while(height > 0) {
                var pieceHeight = height >= maxPieceHeight ? maxPieceHeight : height;
                height -= maxPieceHeight;
                var pieceDiv = document.createElement("DIV");
                pieceDiv.style.height = pieceHeight + "px";
                div.appendChild(pieceDiv);
            } 
        }
    },

    OnVirtualScroll: function() {
        this.ClearVirtualScrollTimer();
          
        var deferredHandler = function() { this.HandleVirtualScroll(); }.aspxBind(this);

        this.virtualScrollTimerID = window.setTimeout(deferredHandler, this.control.virtualScrollingDelay);
    },

    HandleVirtualScroll: function() {
        var pos = this.GetVertScrollPosition();
        if(pos == this.virtualScrollPrevPos)
            return;

        this.virtualScrollPrevPos = pos;

        if(this.lockVirtualScrolling) 
            return;

        var grid = this.control;
        var index = this.GetPageIndexForVirtualScrollPos(pos);
        if(index != grid.pageIndex) {
            ASPx.Evt.DetachEventFromElement(this.GetVertScrollableControl(), "scroll", this.virtualScrollHandler);
            grid.PreventCallbackAnimation();
            this.requireRetainVerticalScrollPosition = this.isVirtualSmoothScrolling;
            grid.GotoPage(index);
        }
    },

    GetPageIndexForVirtualScrollPos: function(pos) {
        var grid = this.control;
        var table = this.GetContentTable();
        var container = table.parentNode;

        var index;
        if(this.isVirtualSmoothScrolling) {
            index = Math.round(grid.pageCount * pos / container.scrollHeight);
        } else {
            index = Math.floor(grid.pageCount * pos / container.scrollHeight);

            var tableTop = ASPx.GetAbsoluteY(table);
            var containerTop = ASPx.GetAbsoluteY(container);
            var tableBottom = tableTop + table.offsetHeight;
            var containerBottom = containerTop + container.clientHeight;

            if(tableTop < containerTop && tableBottom > containerTop && tableBottom <= containerBottom)
                index++;
        }

        return Math.min(grid.pageCount - 1, index);
    },

    ClearVirtualScrollTimer: function() {
        this.virtualScrollTimerID = ASPx.Timer.ClearTimer(this.virtualScrollTimerID);
    },

    GetVirtualScrollRowHeight: function() {
        var grid = this.control;
        if(this.virtualScrollRowHeight < 0) {
            var dataRow = grid.GetDataRow(grid.visibleStartIndex);
            var previewRow = grid.GetPreviewRow(grid.visibleStartIndex);

            var height = dataRow ? dataRow.offsetHeight : 20;
            if(previewRow)
                height += previewRow.offsetHeight;

            this.virtualScrollRowHeight = height;
        }
        return this.virtualScrollRowHeight;
    },

    // Endless Paging

    UpdateEndlessPaging: function() {
        var container = this.GetVertScrollableControl();
        if(container.dxEndlessPaging)
            return;
        ASPx.Evt.AttachEventToElement(container, "scroll", function(e) { this.OnEndlessPagingScroll(e); }.aspxBind(this)); 
        ASPx.Evt.AttachEventToElement(container, ASPx.Evt.GetMouseWheelEventName(), function(e) { this.OnEndlessPagingMouseWheel(e); }.aspxBind(this));
        container.dxEndlessPaging = true;
    },
    OnEndlessPagingMouseWheel: function(e) {
        if(!this.control.InCallback()) return;

        var canScrollBottom = this.CalculateBottomExcess(true) > 0;
        var isScrollingToBottom = ASPx.Evt.GetWheelDelta(e) < 0;
        if(!canScrollBottom && isScrollingToBottom)
            return ASPx.Evt.PreventEvent(e);
    },
    OnEndlessPagingScroll: function(e) {
        this.CheckEndlessPagingLoadNextPage();
    },

    CheckEndlessPagingLoadNextPage: function() {
        var grid = this.control;
        if(grid.pageIndex + 1 === grid.pageCount || grid.pageCount === 0 || grid.InCallback())
            return;

        var avgPageHeight = this.CalculateAveragePageHeight();
        var bottomExcess = this.CalculateBottomExcess();
        if(bottomExcess < avgPageHeight / 3) {
            grid.NextPage();
        }
    },
    CalculateBottomExcess: function(withLoadingPanel) {
        var table = this.GetContentTable();
        var scrollDiv = this.GetScrollDiv(table);
        var contentHeight = table.offsetHeight;
        var result = contentHeight - scrollDiv.clientHeight - scrollDiv.scrollTop;
        if(withLoadingPanel){
            var lpContainer = this.control.GetEndlessPagingLPContainer();
            lpContainer && (result += lpContainer.clientHeight);
        }
        return result;
    },
    CalculateAveragePageHeight: function() {
        var grid = this.control;
        var contentHeight = this.GetContentTable().offsetHeight;
        return grid.pageRowSize * contentHeight / grid.pageRowCount;
    },

    // AllowFixedGroups
    UpdateFixedGroups: function(){
        var container = this.GetVertScrollableControl();
        if(container.dxFixedGroups)
            return;
        
        var helper = this.control.GetFixedGroupsHelper();
        if(helper)
            helper.PopulateRowsHeight();
        ASPx.Evt.AttachEventToElement(container, "scroll", function(e) { this.OnUpdateFixedGroups(e); }.aspxBind(this)); 
        container.dxFixedGroups = true;
    },
    OnUpdateFixedGroups: function(e){
        var helper = this.control.GetFixedGroupsHelper();
        if(helper)
            helper.UpdateFixedGroups();
    }
});

var GridViewFixedGroupsHelper = ASPx.CreateClass(ASPx.TableHelperAdapter, {
    constructor: function(tableHelper){
        this.tableHelper = tableHelper;
        this.control = this.tableHelper.control;
        
        this.cachedTopVisibleIndex = -1;
        this.fixedGroupsIndexes = [ ];
        this.rowsHeight = [ ];
    },

    OnDocumentScroll: function(){
        this.UpdateFixedGroupStyleCore(false);
    },
    UpdateFixedGroups: function(){
        var topVisibleIndex = this.GetTopRowVisibleIndex();
        var newFixedGroupIndexes = this.FindFixedGroupIndexes(topVisibleIndex);
        this.ClearFixedGroupStyle(newFixedGroupIndexes);
        this.UpdateFixedGroupStyle(newFixedGroupIndexes, topVisibleIndex);
    },
    GetTopRowVisibleIndex: function(){
        var topVisibleIndex = this.control.visibleStartIndex - this.GetContinuousGroupCount() - 1;
        var hiddenRowsHeight = 0;
        var scrollDiv = this.GetScrollDiv(this.GetContentTable());
        for(var i = 0; i < this.rowsHeight.length && scrollDiv.scrollTop > hiddenRowsHeight; i++){
            hiddenRowsHeight += this.rowsHeight[i];
            topVisibleIndex++;
        }
        return topVisibleIndex;
    },
    GetContinuousGroupCount: function(){
        var grid = this.control;
        if(grid.useEndlessPaging || grid.pageIndex <= 0)
            return 0;
        
        var count = 0;
        var mainTable = grid.GetMainTable();
        for(var rowIndex = grid.GetItem(grid.visibleStartIndex).rowIndex - 1; rowIndex > -1; rowIndex--){
            var row = mainTable.rows[rowIndex];
            if(row && row.id && row.id.indexOf(grid.GroupRowID) > -1)
                count++
        }
        return count;
    },
    ClearFixedGroupStyle: function(newfixedGroupsIndexes){
        for(var i = 0; i < this.fixedGroupsIndexes.length; i++){
            var groupRow = this.control.GetGroupRow(this.fixedGroupsIndexes[i]);
            if(!groupRow || newfixedGroupsIndexes && newfixedGroupsIndexes.indexOf(this.fixedGroupsIndexes[i]) > -1) continue;
            groupRow.style.position = "";
            groupRow.style.top = "";
            groupRow.style.width = "";
            groupRow.style.zIndex = "";

            var groupMoreRows = this.control.GetGroupMoreRows(this.fixedGroupsIndexes[i]);
            if(groupMoreRows)
                ASPx.SetElementDisplay(groupMoreRows, false);
        }
    },
    UpdateFixedGroupStyle: function(newfixedGroupsIndexes, topVisibleIndex){
        this.fixedGroupsIndexes = newfixedGroupsIndexes;
        this.cachedTopVisibleIndex = topVisibleIndex;
        
        if(!this.control.isVirtualScrolling)
            this.UpdateContentTableTopPadding();
        this.UpdateFixedGroupStyleCore(true);
        this.UpdateGroupMoreRowsIndicators();
    },
    UpdateFixedGroupStyleCore: function(isFixedPostion){
        var fixedGroupsHeight = 0;
        var count = this.fixedGroupsIndexes.length;
        
        var table = this.GetContentTable();
        var scrollDiv = this.GetScrollDiv(table);

        for(var groupLevel = 0; groupLevel < count; groupLevel++){
            var groupIndex = this.fixedGroupsIndexes[groupLevel];
            var group = this.control.GetGroupRow(groupIndex);
            var isPartiallyVisible = this.IsPartiallyVisibleGroup(groupLevel);

            if(isPartiallyVisible)
                isFixedPostion = false;

            group.style.position = isFixedPostion ? "fixed" : "absolute";
            group.style.zIndex = count - groupLevel;
            
            var groupTopPosition = 0;
            if(isPartiallyVisible){
                var rowIndexPosition = this.GetFixedGroupMaxIndexPosition(groupIndex, groupLevel);
                groupTopPosition = this.GetRowBottomPosition(rowIndexPosition);
            } else {
                var firstFixedGroupTopPosition = isFixedPostion ? (ASPx.GetAbsoluteY(scrollDiv) - ASPx.GetDocumentScrollTop()) : scrollDiv.scrollTop;
                groupTopPosition = fixedGroupsHeight + firstFixedGroupTopPosition;
            }

            group.style.top = groupTopPosition + "px";
            group.style.left = isFixedPostion ? ASPx.GetAbsoluteX(scrollDiv) - ASPx.GetDocumentScrollLeft() + "px" : "";
            
            this.SetGroupRowCellsWidth(group, groupLevel);
            fixedGroupsHeight += this.GetRowsHeight(groupIndex);
        }
    },
    GetRowBottomPosition: function(rowIndex){
        var groupCount = this.fixedGroupsIndexes.length;

        var visibleStartIndex = this.control.visibleStartIndex;
        if(visibleStartIndex < rowIndex)
            return this.GetRowsHeight(visibleStartIndex, rowIndex - visibleStartIndex + this.GetContinuousGroupCount());
        
        var count = visibleStartIndex - rowIndex;
        var bottom = 0
        for(var i = 0; i < count; i++){
            var vi = this.fixedGroupsIndexes[i];
            bottom += this.GetRowHeight(vi);
        }
        return bottom;
    },
    GetFixedGroupMaxIndexPosition: function(groupIndex, groupLevel){
        var grid = this.control;
        var groupCount = this.fixedGroupsIndexes.length;
        var maxSearcedVI = this.cachedTopVisibleIndex + groupCount;
        for(var vi = this.cachedTopVisibleIndex; vi <= maxSearcedVI; vi++){
            if(!this.IsLastVisibleIndexInGroup(vi)) continue;
            
            if(grid.GetGroupLevel(vi + 1) <= groupLevel)
                return vi - (groupCount - groupLevel);
        }
        return -1;
    },
    IsPartiallyVisibleGroup: function(fixedGroupLevel){
        var bottomFixedGroupIndex = this.cachedTopVisibleIndex + this.fixedGroupsIndexes.length + 1;
        if(!this.IsCollapsedGroupRow(bottomFixedGroupIndex - 1) && this.control.GetGroupLevel(bottomFixedGroupIndex) == 0)
            return true;

        var visibleIndex = this.cachedTopVisibleIndex + fixedGroupLevel + 1;
        var groupLevel = this.control.GetGroupLevel(visibleIndex);
        if(groupLevel < 0)
            return this.IsLastVisibleIndexInGroup(visibleIndex) && this.control.GetGroupLevel(visibleIndex + 1) <= fixedGroupLevel;

        if(this.IsCollapsedGroupRow(visibleIndex))
            return this.IsLastVisibleIndexInGroup(visibleIndex) || this.control.GetGroupLevel(visibleIndex + 1) <= fixedGroupLevel;

        return groupLevel <= fixedGroupLevel;
    },
    GetGroupTopPosition: function(isFixedPostion, isPartiallyVisible, scrollDiv){
        if(isPartiallyVisible){
            var firstVisibleRowIndex = this.cachedTopVisibleIndex + this.fixedGroupsIndexes.length - 1;
            return this.GetRowsHeight(0, firstVisibleRowIndex);
        }

        var firstFixedGroupTopPosition = isFixedPostion ? (ASPx.GetAbsoluteY(scrollDiv) - ASPx.GetDocumentScrollTop()) : scrollDiv.scrollTop;
        return fixedGroupsHeight + firstFixedGroupTopPosition;
    },
    UpdateGroupMoreRowsIndicators: function(){
        var count = this.fixedGroupsIndexes.length;
        for(var groupLevel = 0; groupLevel < count; groupLevel++){
            var groupIndex = this.fixedGroupsIndexes[groupLevel];
            var groupMoreRows = this.control.GetGroupMoreRows(groupIndex);
            if(groupMoreRows){
                var showGroupMoreRows = groupLevel > 0 || (groupIndex + 1) < this.fixedGroupsIndexes[groupLevel + 1] || count == 1;
                if(!showGroupMoreRows && groupLevel + 1 < count){
                    var nextFixedRow = this.control.GetItem(this.fixedGroupsIndexes[groupLevel + 1]);
                    showGroupMoreRows = nextFixedRow && nextFixedRow.style && nextFixedRow.style.position == "absolute";
                }
                ASPx.SetElementDisplay(groupMoreRows, showGroupMoreRows);
            }
        }
    },
    UpdateContentTableTopPadding: function(){
        var fixedGroupsHeight = 0;
        for(var i = 0; i < this.fixedGroupsIndexes.length; i++)
            fixedGroupsHeight += this.GetRowsHeight(this.fixedGroupsIndexes[i]);
        
        var table = this.GetContentTable();
        table.style.paddingTop = fixedGroupsHeight + "px";
    },
    FindFixedGroupIndexes: function(vi){
        if(vi < 0)
            return [ ];

        var grid = this.control;

        var collapsedGroupCount = 0;
        var startSearchVI = vi;
        while((this.IsLastVisibleIndexInGroup(startSearchVI) || grid.IsGroupRowExpanded(startSearchVI)) && grid.GetGroupLevel(startSearchVI + 1) != 0)
            startSearchVI++;
        var fixedGroupIndexes = this.FindParentGroupIndexes(startSearchVI);

        startSearchVI = vi + fixedGroupIndexes.length;
        while(grid.IsGroupRowExpanded(startSearchVI) && grid.GetGroupLevel(startSearchVI) == fixedGroupIndexes.length)
            fixedGroupIndexes.push(startSearchVI++);

        return fixedGroupIndexes;
    },
    IsCollapsedGroupRow: function(vi){
        return this.control.IsGroupRow(vi) && !this.control.IsGroupRowExpanded(vi);
    },
    FindParentGroupIndexes: function(visibleIndex){
        var grid = this.control;
        var parentGroupIndexes = [ ];
        var lastLevel = grid.GetGroupLevel(visibleIndex);
        while(lastLevel != 0 && visibleIndex-- > -1){
            var newLevel = grid.GetGroupLevel(visibleIndex);
            if(newLevel > -1 && (newLevel < lastLevel || lastLevel == -1)){
                parentGroupIndexes.push(visibleIndex);
                lastLevel = grid.GetGroupLevel(visibleIndex);
            }
        }
        parentGroupIndexes.reverse();
        return parentGroupIndexes;
    },
    SetGroupRowCellsWidth: function(groupRow, level) {
        var armCells = this.GetArmatureCells(this.GetHeaderTable() || this.GetContentTable());
        var lastGroupServiceIndex = Math.min(level, groupRow.cells.length - 2);
        for(var i = 0; i <= lastGroupServiceIndex; i++){
            var cell = groupRow.cells[i];
            var cellWidth = armCells[i].offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(cell);
            cell.style.width = cellWidth + "px";
        }

        var totalWidth = 0;
        for(var i = lastGroupServiceIndex + 1; i < armCells.length; i++)
            totalWidth += armCells[i].offsetWidth;
        groupRow.cells[lastGroupServiceIndex + 1].style.width = totalWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(groupRow.cells[lastGroupServiceIndex + 1]) + "px";
    },
    IsLastVisibleIndexInGroup: function(vi){
        var grid = this.control;
        return grid.IsGroupRow(vi + 1) && (!grid.IsGroupRow(vi) || grid.GetGroupLevel(vi) > grid.GetGroupLevel(vi + 1));
    },
    GetContentTable: function(){
        return this.tableHelper.GetContentTable();
    },
    GetScrollDiv: function(table){
        var scrollHelper = this.control.GetScrollHelper();
        return scrollHelper ? scrollHelper.GetScrollDiv(table) : null;
    },

    PopulateRowsHeight: function(){
        this.ResetRowsHeight();
        var startIndex = this.control.visibleStartIndex;
        for(var i = 0; i < this.control.pageRowCount; i++){
            var height = this.GetRowHeight(startIndex + i);
            this.rowsHeight.push(height);
        }
    },
    GetRowHeight: function(visibleIndex){
        var row = this.control.GetItem(visibleIndex);
        if(!row)
            return 0;
        if(row.getBoundingClientRect)
            return row.getBoundingClientRect().height;
        return row.outerHeight;
    },
    ResetRowsHeight: function(){
        this.rowsHeight = [ ];
    },
    GetRowsHeight: function(startVisibleIndex, count){
        var startIndex = startVisibleIndex - this.control.visibleStartIndex;
        var totalHeight = 0;
        var count = count || 1;
        for(var i = 0; i < count; i++){
            var arrIndex = startIndex + i;
            if(arrIndex > -1 && arrIndex < this.rowsHeight.length)
                totalHeight += this.rowsHeight[arrIndex];
            else
                totalHeight += this.GetRowHeight(startVisibleIndex + i);
        }
        return totalHeight;
    }
});

var TableFixedColumnsHelper = ASPx.CreateClass(ASPx.TableHelperAdapter, {
    constructor: function(tableHelper, fixedDivName, fixedContentDivName, fixedColumnCount){
        this.constructor.prototype.constructor.call(this, tableHelper);
        this.FixedDivName = fixedDivName;
        this.FixedContentDivName = fixedContentDivName;
        this.FixedColumnCount = fixedColumnCount; 
        this.hiddenColumnCount = 0;
        this.touchUIScroller = null;
        this.mouseScroller = null;

        this.savedScrollPos = -1;
    },
    GetFixedDiv: function() { return this.tableHelper.GetChildElement(this.FixedDivName); },
    GetFixedContentDiv: function() { return this.tableHelper.GetChildElement(this.FixedContentDivName); },
    
    Update: function() {
        var contentDiv = this.GetContentTable().parentNode;
        this.AttacheEvent(this.GetFixedDiv(), "scroll", this.OnScroll);
        this.AttacheEvent(contentDiv, "scroll", function() { 
            var expectedLeft = this.control.rtl ? 0x1fffffff : 0;
            if(contentDiv.scrollLeft != expectedLeft)
                contentDiv.scrollLeft = expectedLeft;
        }.aspxBind(this));

        if(ASPx.Browser.TouchUI)
            this.UpdateMouseScroller();
        if(ASPx.Browser.WebKitTouchUI)
            this.UpdateTouchUIScroller();
        if(ASPx.Browser.MacOSPlatform && ASPx.Browser.WebKitFamily) // Q395192
            this.UpdateWheelScroller();
        this.UpdateFixedDivSize();
    },

    UpdateFixedDivSize: function() {
        var fixedDiv = this.GetFixedDiv();
        if((ASPx.Browser.WebKitFamily || ASPx.Browser.IE) && fixedDiv.scrollLeft == 0 && this.savedScrollPos > 0) // Q408680
            fixedDiv.scrollLeft = this.savedScrollPos;

        var fixedContentDiv = this.GetFixedContentDiv();
        var contentDivStyleWidth = fixedContentDiv.style.width;
        var contentDivStyleHeight = fixedContentDiv.style.height;
        var contentDivWidth = this.GetFixedContentDivWidth() + "px";
        var contentDivHeight = this.GetFixedContentDivHeight() + "px";

        if(contentDivStyleWidth !== contentDivWidth)
            fixedContentDiv.style.width = contentDivWidth;
        if(contentDivStyleHeight !== contentDivHeight || fixedDiv.style.height !== contentDivHeight)
            fixedContentDiv.style.height = fixedDiv.style.height = contentDivHeight;
        this.UpdateRowHeights();
    },

    GetFixedContentDivWidth: function() {
        var armCells = this.GetArmatureCells(this.GetHeaderTable() || this.GetContentTable());
        var totalWidth = 0;
        for(var i = 0; i < armCells.length; i++) {
            var width = this.GetStylePxWidth(armCells[i]);
            totalWidth += width > -1 ? width : 0;
        }
        return totalWidth;
    },
    GetFixedContentDivHeight: function() {
        var scrollHeigth = ASPx.GetVerticalScrollBarWidth() + 1; // extra pixel added for IE, see B142305
        if(ASPx.Browser.MacOSPlatform && ASPx.Browser.WebKitFamily && ASPx.GetVerticalScrollBarWidth() == 0) // Q395192
            scrollHeigth = 12;
        return scrollHeigth;
    },

    OnScroll: function(event) {
        var src = ASPx.Evt.GetEventSource(event);
        var helper = src.fixedHelper;
        helper.savedScrollPos = src.scrollLeft;
        helper.UpdateCellsVisibility(helper.GetColumnsToHide(src.scrollLeft));
    },
    
    AttacheEvent: function(element, eventName, eventHandler) {
        if(!element) return;
        var eventMarker = "gvfh" + eventName;
        if(element[eventMarker])
            return;
        ASPx.Evt.AttachEventToElement(element, eventName, eventHandler);
        element[eventMarker] = true;
        element.fixedHelper = this;
    },
    
    DetacheEvent: function(element, eventName, eventHandler) {
        if(!element) return;
        ASPx.Evt.DetachEventFromElement(element, eventName, eventHandler);
        var eventMarker = "gvfh" + eventName;
        element[eventMarker] = false;
        element.fixedHelper = null;
    },
    
    SetScrollLeft: function(target, scrollLeft) {
        if(target.scrollLeft != scrollLeft)
            target.scrollLeft = scrollLeft;
    },
    
    // Touch
    UpdateMouseScroller: function() {
        var getContentDiv = function() { return this.GetContentTable().parentNode; }.aspxBind(this);
        if(!this.mouseScroller)
            this.mouseScroller = ASPx.MouseScroller.Create(
                getContentDiv,
                function() { return this.GetFixedDiv(); }.aspxBind(this),
                getContentDiv,
                function(element) { return this.control.IsHeaderChild(element); }.aspxBind(this),
                true, 
                function(e) { this.UpdateTouchScrollBars(); }.aspxBind(this),
                function(e) { this.UpdateTouchScrollBars(); }.aspxBind(this),
                function(e) { this.UpdateTouchScrollBars(true); }.aspxBind(this)
            );
        else
            this.mouseScroller.update();
    },
    UpdateTouchUIScroller: function() {
        this.touchUIScroller = this.touchUIScroller || this.CreateTouchUIScroller();
        if(!ASPx.IsExistsElement(this.touchUIScroller.element))
            this.touchUIScroller.ChangeElement(this.GetFixedDiv());
        this.touchUIScroller.updateInitData();
    },
    
    CreateTouchUIScroller: function() {
        return ASPx.TouchUIHelper.MakeScrollable(this.GetFixedDiv(), { 
            showHorizontalScrollbar: true, 
            showVerticalScrollbar: false,
            forceCustomScroll: true
        });
    },

    UpdateTouchScrollBars: function(hide) {
        if(!this.touchUIScroller) 
            return;
        if(hide) {
            this.touchUIScroller.hideScrollBars();
        } else {
            this.touchUIScroller.updateScrollHandles();
            this.touchUIScroller.showScrollBars();
        }
    },

    UpdateWheelScroller: function() {
        var contentDiv = this.GetContentTable().parentNode;
        if(this.wheelHandler)
            ASPx.Evt.DetachEventFromElement(contentDiv, "mousewheel", this.wheelHandler);

        this.wheelHandler = this.wheelHandler || this.CreateWheelHandler();
        ASPx.Evt.AttachEventToElement(contentDiv, "mousewheel", this.wheelHandler);
    },

    CreateWheelHandler: function() {
        return function(e) {
            if(!e.wheelDeltaX) 
                return;

            var fixedDiv = this.GetFixedDiv();
            var delta = e.wheelDeltaX / 120;
            var leftDirection = delta > 0;

            var isLeftmost = fixedDiv.scrollLeft == 0;
            var isRightmost = fixedDiv.scrollLeft + fixedDiv.offsetWidth == fixedDiv.scrollWidth

            if(leftDirection && isLeftmost || !leftDirection && isRightmost)
                return;

            fixedDiv.scrollLeft += -1 * delta * 100;
            return ASPx.Evt.PreventEvent(e);
        }.aspxBind(this);
    },
    
    GetColumnsToHide: function(pos) {
        var grid = this.control;
        var indices = grid.GetHeaderMatrix().GetLeafIndices();

        if(grid.rtl) {
            pos = this.GetFixedContentDiv().scrollWidth - pos - grid.GetMainElement().offsetWidth;
            if(grid.HasVertScroll())
                pos += ASPx.GetVerticalScrollBarWidth();
        }

        var i;
        var width = 0;
        for(i = this.FixedColumnCount; i < indices.length; i ++) {
            if(width >= pos) break;       
            width += this.GetCellRealWidth(grid.GetHeader(indices[i]));
        }

        return i - this.FixedColumnCount;
    },

    UpdateCellsVisibility: function(columnsToHide) {
        if(this.hiddenColumnCount == columnsToHide)
            return;
        var prevHiddenColumnCount = this.hiddenColumnCount;
        this.hiddenColumnCount = columnsToHide;

        var startIndex = Math.min(prevHiddenColumnCount, this.hiddenColumnCount) + this.FixedColumnCount;
        var endIndex = Math.max(prevHiddenColumnCount, this.hiddenColumnCount) + this.FixedColumnCount;
        var display = this.hiddenColumnCount - prevHiddenColumnCount > 0 ? "none" : "";

        var tables = [ this.GetHeaderTable(), this.GetContentTable(), this.GetFooterTable() ];

        for(var tableIndex = 0; tableIndex < tables.length; tableIndex++) {
            var table = tables[tableIndex];
            if(!table)
                continue;

            var armCells = this.GetArmatureCells(table);
            for(var i = startIndex; i < endIndex; i++)
                armCells[i].style.display = display;

            for(var rowIndex = 0; rowIndex < table.rows.length; rowIndex++) {
                var row = table.rows[rowIndex];
                if(this.control.IsHeaderRow(row))
                    continue;
                this.ChangeCellsVisibility(row, startIndex, endIndex - 1, display);
            }
        }

        this.UpdateHeadersVisibility();
    },

    ChangeCellsVisibility: function(row, startIndex, endIndex, display) { // Don't remove, it used in the E970
        for(var i = startIndex; i <= endIndex; i++) {
            if(!row.cells[i])
                break;
            row.cells[i].style.display = display;
        }
    },

    UpdateHeadersVisibility: function() {
        var grid = this.control;
        var matrix = grid.GetHeaderMatrix();

        var totalSpans = { };
        var hiddenSpans = { };
        for(var rowIndex = 0; rowIndex < matrix.GetRowCount(); rowIndex++) {
            var indices = matrix.GetRowIndices(rowIndex);
            for(var i = this.FixedColumnCount; i < indices.length; i++) {
                var columnIndex = indices[i];

                if(isNaN(totalSpans[columnIndex])) totalSpans[columnIndex] = 0;
                if(isNaN(hiddenSpans[columnIndex])) hiddenSpans[columnIndex] = 0;

                totalSpans[columnIndex]++;
                if(i < this.FixedColumnCount + this.hiddenColumnCount)
                    hiddenSpans[columnIndex]++;
            }
        }        

        for(var i = 0; i < grid.columns.length; i++) {
            var columnIndex = grid.columns[i].index;
            if(isNaN(totalSpans[columnIndex]))
                continue;

            var visible = totalSpans[columnIndex] > hiddenSpans[columnIndex];
            var header = grid.GetHeader(columnIndex);
            header.style.display = visible ? "" : "none";
            if(visible && matrix.GetRowCount() > 1)
                header.colSpan = Math.max(1, (totalSpans[columnIndex] - hiddenSpans[columnIndex]) / matrix.GetRowSpan(columnIndex));
        }
    },
    
    UpdateRowHeights: function() {
        var rows = this.GetRowsForHeightCorrection();
        var skipCorrection = true;
        for(var i = 0; i < rows.length; i++)
            skipCorrection &= !!rows[i].heightCorrected;
        if(skipCorrection)
            return;
        var savedColumnsToHide = this.hiddenColumnCount;
        if(savedColumnsToHide !== 0)
            this.UpdateCellsVisibility(0);

        this.UpdateRowHeightsCore(rows);

        if(savedColumnsToHide !== 0)
            this.UpdateCellsVisibility(savedColumnsToHide);

        for(var i = 0; i < rows.length; i++)
            rows[i].heightCorrected = true;
    },

    UpdateRowHeightsCore: function(rows) {
        GridViewFixedColumnsStyleSheetHelper.Instance.BeginUpdate();

        var styleSheetRules = this.GetStyleSheetRulesForHeightCorrection(rows);
        GridViewFixedColumnsStyleSheetHelper.Instance.ChangeRules(this.control, styleSheetRules);

        GridViewFixedColumnsStyleSheetHelper.Instance.EndUpdate();
    },

    GetRowsForHeightCorrection: function() {
        var tables = [ this.GetHeaderTable(), this.GetContentTable(), this.GetFooterTable() ];
        var rows = [ ];
        for(var tableIndex = 0; tableIndex < tables.length; tableIndex++) {
            var table = tables[tableIndex];
            if(!table) continue;
            for(var i = 0; i < table.rows.length; i++)
                rows.push(table.rows[i]);
        }
        return rows;
    },

    GetStyleSheetRulesForHeightCorrection: function(rows) {
        var selectors = { };
        var selectorMask = [ "#", "id", " > ", "td", ".dxgvHEC" ];
        for(var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if(row.cells.length < 1 || !row.id)
                continue;
            var cell = row.cells[row.cells.length - 1];
            var h = cell.offsetHeight;
            if(h <= 0) continue;
            if(!selectors[h])
                selectors[h] = [ ];
            var selectorArgs = selectorMask.slice(0);
            selectorArgs[1] = row.id;
            selectors[h].push(selectorArgs.join(""));
        }
        var rules = [ ];
        for(var height in selectors)
            rules.push({ 
                selector: selectors[height].join(", "), 
                cssText: "height:" + height + "px"
            });
        return rules;
    },

    TryShowColumn: function(columnIndex, showFullRect) {
        if(!ASPx.IsExists(columnIndex) || columnIndex < 0) 
            return false;
        var grid = this.control;
        var matrix = grid.GetHeaderMatrix();

        var index =  matrix.GetLeafIndex(columnIndex);
        if(index < this.FixedColumnCount)
            return false;

        var hiddenColumnCount = this.CalcHiddenColumnCount(columnIndex, showFullRect);
        if(this.hiddenColumnCount === hiddenColumnCount)
            return false;

        var columnsWidth = this.GetHiddenColumnsTotalWidth(hiddenColumnCount);
        var scrollLeft = columnsWidth > 10 ? columnsWidth - 10 : 0;
        this.UpdateCellsVisibility(hiddenColumnCount);
        this.control.SetHorizontalScrollPosition(scrollLeft);
        return true;
    },

    CalcHiddenColumnCount: function(visibleColumnIndex, showFullRect) {
        var grid = this.control;
        var matrix = grid.GetHeaderMatrix();
        var columnIndices = matrix.GetLeafIndices();
        
        var scrollDivWidth = this.GetContentTable().parentNode.offsetWidth;
        var fixedWidth = 0;
        var columnWidths = [ ];
        for(var i = 0; i < columnIndices.length; i++) {
            var width = this.GetCellRealWidth(grid.GetHeader(columnIndices[i]));
            columnWidths[i] = width;
            if(i < this.FixedColumnCount)
                fixedWidth += width;
        }

        var scrolledPartWidth = scrollDivWidth - fixedWidth;
        var start = this.FixedColumnCount + this.hiddenColumnCount;
        var end = this.GetLastColumnIndexFromBound(columnWidths, scrolledPartWidth, start, showFullRect);

        var index =  matrix.GetLeafIndex(visibleColumnIndex);
        var inc = index > start ? 1 : -1;
        while(index < start || index > end) {
            start += inc;
            end = this.GetLastColumnIndexFromBound(columnWidths, scrolledPartWidth, start, showFullRect);
            if(end === columnIndices.length - 1)
                break;
        }
        return start - this.FixedColumnCount;
    },

    GetLastColumnIndexFromBound: function(columnWidths, rectWidth, start, allowRightBound) {
        for(var i = start; i < columnWidths.length; i++) {
            rectWidth -= columnWidths[i];
            if(rectWidth > 0) continue;
            if(allowRightBound && rectWidth !== 0) 
                i--
            return i;
        }
        return columnWidths.length - 1;
    },

    GetHiddenColumnsTotalWidth: function(hiddenColumnCount) {
        var totalWidth = 0;
        var grid = this.control;
        var indices = grid.GetHeaderMatrix().GetLeafIndices();
        for(var i = 0; i < hiddenColumnCount; i++)
            totalWidth += this.GetCellRealWidth(grid.GetHeader(indices[i + this.FixedColumnCount]));
        return totalWidth;
    },
    
    SaveCallbackSettings: function() {
        this.savedScrollPos = -1;
    },
    RestoreCallbackSettings: function() {
        if(this.control.IsLastCallbackProcessedAsEndless())
            return;
        this.FixedColumnCount = this.control.fixedColumnCount;
        this.hiddenColumnCount = 0;
    },
    HideColumnsRelyOnScrollPosition: function() {
        var scrollHelper = this.control.GetScrollHelper();
        var fixedDiv = this.GetFixedDiv();
        var maxScrollLeft = fixedDiv.scrollWidth - fixedDiv.offsetWidth;
        var scrollLeft = Math.min(scrollHelper.savedScrollLeft, maxScrollLeft);
        var columnsToHide = this.GetColumnsToHide(scrollLeft);
        this.UpdateCellsVisibility(columnsToHide);
    },
    GetHiddenColumns: function() {
        var result = [ ];
        var indices = this.control.GetHeaderMatrix().GetLeafIndices();
        var start = this.FixedColumnCount;
        var end = start + this.hiddenColumnCount - 1;
        for(var i = start; i <= end; i++)
            result.push(indices[i]);
        return result;
    }
});

var GridViewFixedColumnsStyleSheetHelper = ASPx.CreateClass(null, {
    constructor: function() {
        this.styleSheet = null;
        this.rules = { };
        this.updateLock = 0;
    },

    ChangeRules: function(control, rules) {
        if(control.name && rules)
            this.rules[control.name] = rules
    },

    BeginUpdate: function() {
        this.updateLock++;
        if(this.styleSheet)
            ASPx.RemoveElement(this.styleSheet);
    },

    EndUpdate: function() {
        this.updateLock--;
        if(this.updateLock !== 0)
            return;

        var styleArgs = [ ];
        for(var key in this.rules) {
            var controlRules = this.rules[key];
            for(var i = 0; i < controlRules.length; i++) {
                var rule = controlRules[i];
                styleArgs.push(rule.selector + " { " + rule.cssText + " } ");
            }
        }
        this.styleSheet = this.CreateStyleSheet(styleArgs.join(""));
    },

    CreateStyleSheet: function(cssText) {
        var container = document.createElement("DIV");
        ASPx.SetInnerHtml(container, "<style type='text/css'>" + cssText + "</style>");
        
        styleSheet = ASPx.GetNodeByTagName(container, "style", 0);
        if(styleSheet) 
            ASPx.GetNodeByTagName(document, "HEAD", 0).appendChild(styleSheet);
        return styleSheet;
    }
});

GridViewFixedColumnsStyleSheetHelper.Instance = new GridViewFixedColumnsStyleSheetHelper();

ASPx.GridViewTableHelper = GridViewTableHelper;
ASPx.GridViewTableResizingHelper = GridViewTableResizingHelper;
ASPx.GridViewTableScrollHelper = GridViewTableScrollHelper;
ASPx.TableFixedColumnsHelper = TableFixedColumnsHelper;
ASPx.GridViewFixedGroupsHelper = GridViewFixedGroupsHelper;
})();