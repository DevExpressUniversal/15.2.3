/// <reference path="_references.js"/>

(function() {
var TableScrollHelperCollection = {
    instances: { },
    add: function(helper){
        if(!helper || !helper.control)
            return;
        this.instances[helper.control.name] = helper;
    },
    onResize: function(){
        for(var key in this.instances)
            this.instances[key].OnWindowResize();
    }
}

var TableHelperBase = ASPx.CreateClass(null, {
    constructor: function(control, contentTableIDSuffix, headerTableIDSuffix, footerTableIDSuffix, horzScroll, vertScroll){
        this.control = control;
        
        this.horzScroll = horzScroll;
        this.vertScroll = vertScroll;
        this.contentTableIDSuffix = contentTableIDSuffix;
        this.headerTableIDSuffix = headerTableIDSuffix;
        this.footerTableIDSuffix = footerTableIDSuffix;
    },
    
    GetChildElement: function(idSuffix) {
        return this.control.GetChildElement(idSuffix);
    },
    GetTable: function(tableIDSuffix) {
        return this.GetChildElement(tableIDSuffix);
    },
    GetContentTable: function() {
        return this.GetTable(this.contentTableIDSuffix);
    },
    GetHeaderTable: function() {
        return this.GetTable(this.headerTableIDSuffix);
    },
    GetFooterTable: function() {
        return this.GetTable(this.footerTableIDSuffix);
    },

    HasHorzScroll: function() { return this.horzScroll != ASPx.ScrollBarMode.Hidden; }, 
    HasVertScroll: function() { return this.vertScroll != ASPx.ScrollBarMode.Hidden; },
    IsHorzAutoScroll: function() {
        return this.horzScroll == ASPx.ScrollBarMode.Auto;
    },
    IsVertAutoScroll: function() {
        return this.vertScroll == ASPx.ScrollBarMode.Auto;
    },

    IsRtl: function() { return false; },
    
    GetArmatureCells: function(table) {
        return table.rows[0].cells;
    },

    GetArmatureCell: function(cell) {
        var index = this.GetArmatureCellIndexByOtherCell(cell);
        return this.GetArmatureCells(ASPx.GetParentByTagName(cell, "TABLE"))[index];
    },

    GetArmatureCellIndexByOtherCell: function(cell) {
        return cell.cellIndex;
    },

    GetCellRealWidth: function(cell) {
        var width = this.GetStylePxWidth(this.GetArmatureCell(cell));
        return width > -1 ? width : cell.offsetWidth
    },

    GetStylePxWidth: function(element) {
        var width = element.style.width;
        if(width && width.indexOf("px") > -1) 
            return parseInt(width, 10);
        return -1;
    }

});

var TableHelperAdapter = ASPx.CreateClass(null, {
    constructor: function(tableHelper) {
        this.tableHelper = tableHelper;
        this.control = this.tableHelper.control;
    },
    GetContentTable: function() {
        return this.tableHelper.GetContentTable();
    },
    GetHeaderTable: function() {
        return this.tableHelper.GetHeaderTable();
    },
    GetFooterTable: function() {
        return this.tableHelper.GetFooterTable();
    },
    GetArmatureCells: function(table) {
        return this.tableHelper.GetArmatureCells(table);
    },
    GetArmatureCell: function(cell) {
        return this.tableHelper.GetArmatureCell(cell);
    },
    GetCellRealWidth: function(cell) {
        return this.tableHelper.GetCellRealWidth(cell);
    },
    GetStylePxWidth: function(element) {
        return this.tableHelper.GetStylePxWidth(element);
    },
    HasHorzScroll: function() {
        return this.tableHelper.HasHorzScroll();
    }, 
    HasVertScroll: function() {
        return this.tableHelper.HasVertScroll();
    },
    IsRtl: function() {
        return this.tableHelper.IsRtl();
    }
});

var TableScrollHelperBase = ASPx.CreateClass(TableHelperAdapter, {
    constructor: function(tableHelper) {
        this.constructor.prototype.constructor.call(this, tableHelper);

        this.touchUIScroller = null;
        this.savedScrollLeft = this.IsRtl() ? 0x1fffffff : -1;
        this.savedScrollTop = 0;
        this.posLoggerHandler = null;
        this.scrollableControlHeight = -1;
        this.activeElementSettings = [ ];
        this.containerScrollInfo = null;

        TableScrollHelperCollection.add(this);
    },
    
    Update: function() {
        this.AssignPosLoggerHandler(this.GetScrollDiv(this.GetContentTable()));
        if(this.HasHorzScroll()) {
            this.AssignScrollEvent(this.GetScrollDiv(this.GetHeaderTable()));
            this.AssignScrollEvent(this.GetScrollDiv(this.GetContentTable()));
        }
        if(ASPx.Browser.WebKitTouchUI)
            this.UpdateTouchUIScroller();
        this.UpdateScrollableControlsSize();

        if(this.tableHelper.control.resetScrollTop) {
            this.ResetScrollTop();
            this.tableHelper.control.resetScrollTop = false;
        }

        this.LoadScrollPosition();
        this.ApplyScrollPosition();
    },
    
    UpdateTouchUIScroller: function() {
        this.touchUIScroller = this.touchUIScroller || this.CreateTouchUIScroller();
        if(!ASPx.IsExistsElement(this.touchUIScroller.element))
            this.touchUIScroller.ChangeElement(this.GetScrollDiv(this.GetContentTable()));
    },
    
    CreateTouchUIScroller: function() {
        return ASPx.TouchUIHelper.MakeScrollable(this.GetScrollDiv(this.GetContentTable()), { 
            showHorizontalScrollbar: this.ShowTouchHorizontalScrollbar(),
            showVerticalScrollbar: this.ShowTouchVerticalScrollbar(),
            forceCustomScroll: this.UseTouchCustomScroll()
        });
    },
    
    ShowTouchHorizontalScrollbar: function() {
        return this.HasHorzScroll();
    },
    ShowTouchVerticalScrollbar: function() {
        return this.HasVertScroll();
    },
    UseTouchCustomScroll: function() {
        return false;
    },
    
    AssignScrollEvent: function(element) {
        if(!element || element.scrollHelper == this) 
            return;
        element.scrollHelper = this;
        ASPx.Evt.AttachEventToElement(element, "scroll", this.OnScroll);
    },
    
    OnScroll: function(event) {
        var src = ASPx.Evt.GetEventSource(event);
        var helper = src.scrollHelper;
        if(!helper) return;
        var scrollDivs = [ 
            helper.GetScrollDiv(helper.GetHeaderTable()),
            helper.GetScrollDiv(helper.GetContentTable()),
            helper.GetScrollDiv(helper.GetFooterTable())
        ];
        for(var i = 0; i < scrollDivs.length; i++) {
            var div = scrollDivs[i];
            if(!div || div == src)
                continue;
            if(ASPx.Browser.IE && ASPx.Browser.Version > 9 && i == 1)
                helper.SetScrollLeftPostponed(div, src);
            else
                helper.SetScrollLeft(div, src);
        }
    },
    
    SetScrollLeftPostponed: function(target, source) {
        if(!this.scrollUpdateTimerID) {
            this.scrollUpdateTimerID = window.setInterval(function() {
                this.SetScrollLeft(target, source);
            }.aspxBind(this), 0);
        }
        ASPx.Timer.ClearTimer(this.scrollUpdateTimerStopID);
        this.scrollUpdateTimerStopID = window.setTimeout(function() {
            ASPx.Timer.ClearInterval(this.scrollUpdateTimerID);
            delete this.scrollUpdateTimerID;
            delete this.scrollUpdateTimerStopID
        }.aspxBind(this), 500)
    },
    
    SetScrollLeft: function(target, source) {
        if(target.scrollLeft != source.scrollLeft)
            target.scrollLeft = source.scrollLeft;
    },

    AssignPosLoggerHandler: function(element) {
        if (!element) return;

        if(!this.posLoggerHandler)
            this.posLoggerHandler = function() { this.LogScrollPosition(); }.aspxBind(this);

        ASPx.Evt.DetachEventFromElement(element, "scroll", this.posLoggerHandler);
        ASPx.Evt.AttachEventToElement(element, "scroll", this.posLoggerHandler);
    },
    
    OnWindowResize: function() {
        if(!this.IsOriginalWidthPercentage())
            return;
        window.setTimeout(function() { 
            this.UpdateScrollableControlsSize(true);

            var resizingHelper = this.GetResizingHelper();
            if(this.IsOriginalWidthPercentage() && resizingHelper)
                resizingHelper.ValidateColumnWidths();
        }.aspxBind(this), 0);
    },
    UpdateScrollableControlsSize: function(onResize) {
        // this func is subject of: B134537, B135041, B140214, Q247230
        // if you change the code below, please check that these bugs don't appear again
        
        if(!this.control.GetMainElement()) 
            return;
        if(!this.control.IsDisplayed()) {
            this.control.ResetControlAdjustment();
            return;
        }

        this.SaveActiveElement();
        
        if(this.HasVertScroll() && this.scrollableControlHeight > -1)
            this.SetHeightCore(this.scrollableControlHeight);
        
        var mainTable = this.control.GetMainElement(),
            mainCell = mainTable.rows[0].cells[0],
            headerTable = this.GetHeaderTable(),
            contentTable = this.GetContentTable(),
            footerTable = this.GetFooterTable();
            
        var parts = [ headerTable, contentTable, footerTable ];
        var body = ASPx.Browser.WebKitFamily ? document.body : document.documentElement;

        var scrollTop = this.GetScrollDiv(contentTable).scrollTop,
            scrollLeft = this.GetScrollDiv(contentTable).scrollLeft,
            savedHeight = mainTable.style.height,
            bodyScrollLeft = body.scrollLeft,
            scrollContainerPadding = this.IsVerticalScrollBarShowed() ? ASPx.GetVerticalScrollBarWidth() : 0;

        this.containerScrollInfo = ASPx.GetOuterScrollPosition(mainTable);
        
        // determine the grid's desired width
        mainTable.style.height = mainCell.offsetHeight + "px";
        for(var i = 0; i < parts.length; i++) {
            if(!parts[i]) continue;

            parts[i].parentNode.style.display = "none";
            if(parts[i] != contentTable){
                var div = parts[i].parentNode;
                div.parentNode.style.paddingRight = "";
            }
        }
        ASPx.RestoreElementOriginalWidth(mainTable);
        var desiredTableWidth = mainTable.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainTable);
        var desiredPartWidth = desiredTableWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainCell);

        // set desired widths to the grid's parts
        for(var i = 0; i < parts.length; i++) {
            var part = parts[i];
            if(!part) continue;
            var padding = 0;
            var div = part.parentNode;
            if(part != contentTable) {
                padding = scrollContainerPadding;
                div.parentNode.style.paddingRight = padding + "px";
            }
            var borderStyle = ASPx.GetCurrentStyle(part == headerTable ? div.parentNode : div);
            padding += ASPx.PxToInt(borderStyle.borderRightWidth) + ASPx.PxToInt(borderStyle.borderLeftWidth);

            div.align = "left";
            div.style.width = "";
            div.style.display = "";

            if(!this.HasHorzScroll() && part == headerTable) {
                var actualWidth = part.offsetWidth + padding;
                if(actualWidth > desiredPartWidth)
                    desiredPartWidth = actualWidth;
            }

            if(this.RequireChangeScrollDivWidth())
                this.SetElementWidth(div, desiredPartWidth - padding);
        }
        
        ASPx.RestoreOuterScrollPosition(this.containerScrollInfo);

        // B147636
        if(this.IsContentTableEmpty() && this.HasHorzScroll() && headerTable)
            contentTable.style.width = headerTable.offsetWidth + "px";
        
        mainTable.style.width = desiredTableWidth + "px";
        mainTable.style.height = savedHeight;

        this.GetScrollDiv(contentTable).scrollTop = scrollTop;
        this.GetScrollDiv(contentTable).scrollLeft = scrollLeft;
        if(body.scrollLeft != bodyScrollLeft) // B188827
            body.scrollLeft = bodyScrollLeft;

        this.RestoreActiveElement();

        ASPx.GetControlCollection().AdjustControls(mainTable);
    },

    RequireChangeScrollDivWidth: function() { return true; },

    IsVerticalScrollBarShowed: function() {
        if(!this.HasVertScroll())
            return false;
        if(this.tableHelper.IsVertAutoScroll())
            return this.GetContentTableExcessHeight() > 0;
        return true;
    },

    GetContentTableExcessHeight: function() {
        var contentDiv = this.GetScrollDiv(this.GetContentTable());
        var scrollSize = ASPx.GetVerticalScrollBarWidth();
        var contentWidth = 0;
        var contentHeight = 0;
        for(var i = 0; i < contentDiv.childNodes.length; i++) {
            var child = contentDiv.childNodes[i];
            if(child.nodeType != 1) 
                continue;
            contentWidth = Math.max(contentWidth, child.offsetWidth);
            contentHeight += child.offsetHeight;
        }
        var divWidth = contentDiv.offsetWidth;
        var divHeight = contentDiv.offsetHeight;

        var divHasHorzScroll = contentWidth > divWidth;
        var divHasVertScroll = contentHeight > divHeight;

        if(!divHasHorzScroll && divHasVertScroll)
            divHasHorzScroll = contentWidth > (divWidth - scrollSize);
        if(divHasHorzScroll && !divHasVertScroll)
            divHasVertScroll = contentHeight > (divHeight - scrollSize);
        
        var result = contentHeight - divHeight;
        if(divHasVertScroll)
            result += scrollSize;
        return result;
    },
    
    IsContentTableEmpty: function() {
        return false;
    },
    
    SetElementWidth: function(element, width) {
        if(!element || width <= 0) 
            return;
        element.style.width = width + "px";
    },
    
    GetScrollDiv: function(childTable) {
        if(!childTable) return null;
        return childTable.parentNode;
    },
    
    MakeRowVisible: function(row, fromKbdHelper) {
        var div = this.GetScrollDiv(this.GetContentTable());
        if(div == null || !row || !this.HasVertScroll()) 
            return;
        var divTop = ASPx.GetAbsoluteY(div);
        var rowTop = ASPx.GetAbsoluteY(row);
        var topDiff = divTop - rowTop;
        if(topDiff > 0) {
            div.scrollTop -= topDiff;
            return; 
        }
        var divBottom = divTop + div.clientHeight + 1;
        var rowBottom = rowTop + row.offsetHeight;

        var bottomDiff = rowBottom - divBottom;
        if(bottomDiff <= 0) return;

        var diff = fromKbdHelper ? bottomDiff : topDiff * -1;
        div.scrollTop += diff;
    },

    GetHorzScrollableControl: function() {
        if(this.HasHorzScroll()) 
            return this.GetScrollDiv(this.GetContentTable());
        return null;
    },

    GetVertScrollableControl: function() {
        if(this.HasVertScroll()) 
            return this.GetScrollDiv(this.GetContentTable());
        return null;
    },

    LogScrollPosition: function() {
        if(!this.control.GetMainElement()) return;
        if(this.HasHorzScroll())
            this.savedScrollLeft = this.GetHorzScrollableControl().scrollLeft;
        if(this.HasVertScroll())
            this.savedScrollTop = this.GetVertScrollableControl().scrollTop;
        this.SaveScrollPosition();
    },

    SetScrollState: function(state) { },
    GetScrollState: function() { },

    SaveScrollPosition: function() {
        this.SetScrollState([this.savedScrollLeft, this.savedScrollTop]);
    },
    LoadScrollPosition: function() {
        var savedState = this.GetScrollState();
        if(savedState && savedState.length === 2) {
            this.savedScrollLeft = Number(savedState[0]);
            this.savedScrollTop = Number(savedState[1]);
        }
    },

    ApplyScrollPosition: function() {
        this.SetHorzScrollPosition(this.savedScrollLeft);
        this.SetVertScrollPosition(this.savedScrollTop);
    },

    SetHorzScrollPosition: function(pos) {
        if(!this.HasHorzScroll() || pos < 0) 
            return;
        this.SetScrollPositionCore(this.GetHorzScrollableControl(), pos, false);
    },

    SetVertScrollPosition: function(pos) {
        if(!this.HasVertScroll() || pos < 0)
            return;
        this.SetScrollPositionCore(this.GetVertScrollableControl(), pos, true);
    },

    SetScrollPositionCore: function(element, pos, isTop) {
        if(isTop) {
            if(element.scrollTop != pos)
                element.scrollTop = pos;
        } else if(element.scrollLeft != pos) {
            element.scrollLeft = pos;
        }
    },

    GetHorzScrollPosition: function() {
        if(!this.HasHorzScroll()) return 0;
        return this.GetScrollPositionCore(this.GetHorzScrollableControl(), false);
    },

    GetVertScrollPosition: function() {
        if(!this.HasVertScroll()) return 0;
        return this.GetScrollPositionCore(this.GetVertScrollableControl(), true);
    },

    GetScrollPositionCore: function(element, isTop) {
        return isTop ? element.scrollTop : element.scrollLeft;
    },

    ResetScrollTop: function() {
        this.savedScrollTop = 0;
        this.SaveScrollPosition();
    },

    SetHeight: function(height) {
        if(!this.HasVertScroll()) 
            return;
        if(this.scrollableControlHeight && this.scrollableControlHeight === height)
            return;
        this.scrollableControlHeight = height;
        this.SetHeightCore(height);
        this.Update();
    },

    SetHeightCore: function(height) {
        var mainElement = this.control.GetMainElement();
        var div = this.GetVertScrollableControl();
        var mainElementHeight = mainElement.offsetHeight;

        //B218392
        if (ASPx.Browser.IE && ASPx.Browser.Version > 8)
            mainElementHeight = ASPx.PxToFloat(window.getComputedStyle(mainElement, null).height);

        height = ASPx.PxToFloat(div.style.height) + (height - mainElementHeight);
        div.style.height = Math.max(height, 0) + "px";
    },

    OnSetWidth: function() {
        ASPx.DropElementOriginalWidth(this.control.GetMainElement());
    },

    IsOriginalWidthPercentage: function() {
        var mainElement = this.control.GetMainElement();
        var width = String(ASPx.GetElementOriginalWidth(mainElement));
        return ASPx.IsPercentageSize(width);
    },

    SaveActiveElement: function() {
        if(!ASPx.Browser.IE) return;
        var activeElement = ASPx.GetActiveElement();
        if(!activeElement || activeElement.tagName != "INPUT" && activeElement.tagName != "TEXTAREA") 
            return;
        if(!ASPx.GetIsParent(this.control.GetMainElement(), activeElement))
            return;
        var selInfo = ASPx.Selection.GetInfo(activeElement);
        this.activeElementSettings = [ activeElement, selInfo.startPos, selInfo.endPos ];
    },

    RestoreActiveElement: function() {
        if(!ASPx.Browser.IE || !this.activeElementSettings || this.activeElementSettings.length == 0)
            return;
        var oldElement = this.activeElementSettings[0];
        var currentElement = ASPx.GetActiveElement();
        if(!ASPx.IsExistsElement(oldElement) || currentElement === oldElement)
            return;
        oldElement.focus();
        ASPx.Selection.Set(oldElement, this.activeElementSettings[1], this.activeElementSettings[2]);
        this.activeElementSettings = null;
    }
    
});
TableScrollHelperBase.prevBodyWidth = -1;

ASPx.tableColumnResizing = null;
var TableResizingHelperBase = ASPx.CreateClass(TableHelperAdapter, {
    constructor: function(tableHelper) {
        this.constructor.prototype.constructor.call(this, tableHelper);
        
        this.maximumOffset = ASPx.Browser.TouchUI ? 10 : 3;
        this.defaultMinWidth = 16;

        this.prevX = 0;
        this.colInfo = { };
        this.nextColInfo = { };
    },

    IsResizing: function() { 
        return ASPx.tableColumnResizing == this;
    },

    CanStartResizing: function(e, headerCell) {
        this.prevX = ASPx.Evt.GetEventX(e);

        var left = ASPx.GetAbsoluteX(headerCell);
        var right = left + headerCell.offsetWidth - 1;

        var isLeftEdge = this.prevX - left < this.maximumOffset;
        var isRightEdge = right - this.prevX <= this.maximumOffset;

        if(!isLeftEdge && !isRightEdge)
            return false;

        var columnIndex = this.GetColumnIndexById(headerCell.id);
        var rtl = this.IsRtl();

        
        if(isLeftEdge && !rtl || isRightEdge && rtl)
            return !this.IsLeftmostColumn(columnIndex);
        
        if(isRightEdge && !rtl || isLeftEdge && rtl) {
            if(!this.IsNextColumnResizable())
                return true;
            return !this.IsRightmostColumn(columnIndex);
        }
    },

    GetResizingColumnIndex: function(e, headerCell) {
        var isLeft = ASPx.Evt.GetEventX(e) < ASPx.GetAbsoluteX(headerCell) + headerCell.offsetWidth / 2;
        if(this.IsRtl())
            isLeft = !isLeft;

        var columnIndex = this.GetColumnIndexById(headerCell.id);
        return this.GetResizingColumnIndexCore(columnIndex, isLeft);
    },

    GetResizingColumnIndexCore: function(columnIndex, isLeft) {
        if(isLeft)
            return this.GetColumnNeighbor(columnIndex, true);
        return columnIndex;
    },



    StartResizing: function(columnIndex) {
        this.colInfo = this.GetColumnInfo(columnIndex);
        if(this.IsNextColumnResizable())
            this.nextColInfo = this.GetColumnInfo(this.GetColumnNeighbor(columnIndex, false));

        ASPx.tableColumnResizing = this;
        ASPx.Selection.SetElementSelectionEnabled(document.body, false);
    },

    EndResizing: function() {
        if(!this.colInfo.headerCell)
            return;

        this.SaveControlDimensions();
        ASPx.GetControlCollection().AdjustControls(this.control.GetMainElement());

        this.OnResized(this.colInfo.index);
    },

    CancelResizing: function() {
        ASPx.tableColumnResizing = null;
        ASPx.Selection.SetElementSelectionEnabled(document.body, true);

        this.prevX = 0;
        this.colInfo = { };
        this.nextColInfo = { };

        var scrollHelper = this.GetScrollHelper();
        if(scrollHelper)
            scrollHelper.Update();
    },

    OnMouseMove: function(e) {
        if(ASPx.TouchUIHelper.isTouchEvent(e) && !ASPx.TouchUIHelper.isGesture)
            e.preventDefault();
        if(this.colInfo.headerCell.cellIndex < 0) { // B134111
            this.CancelResizing(e);
            return;
        }
        ASPx.Selection.Clear();
        this.Resize(e);
    },

    OnMouseUp: function(e) {
        this.EndResizing();
        this.CancelResizing();
    },

    Resize: function(e) {
        var newX = Math.round(ASPx.Evt.GetEventX(e));
        var delta = newX - this.prevX;
        if(this.IsRtl())
            delta = -delta;
        
        var newWidth = this.GetCellRealWidth(this.colInfo.headerCell) + delta;
        var minWidth = this.colInfo.minWidth > 0 ? this.colInfo.minWidth : this.defaultMinWidth;
        var nextColNewWidth = nextColMinWidth = 0;
        if(this.IsNextColumnResizable()) {
            nextColNewWidth = this.GetCellRealWidth(this.nextColInfo.headerCell) - delta;
            nextColMinWidth = this.nextColInfo.minWidth > 0 ? this.nextColInfo.minWidth : this.defaultMinWidth;
        }
        if(newWidth < minWidth || nextColNewWidth < nextColMinWidth)
            return;
        
        this.prevX = newX;
        var table = this.control.GetMainElement();
        var tableWidth = table.offsetWidth;

        this.ApplyColumnWidth(this.colInfo.index, newWidth + "px");
        this.colInfo.armCell.minWidthAssigned = false;

        if(this.IsNextColumnResizable()) {
            this.ApplyColumnWidth(this.nextColInfo.index, nextColNewWidth + "px");
            this.nextColInfo.armCell.minWidthAssigned = false;
        } else if(!this.HasHorzScroll()) {
            ASPx.DropElementOriginalWidth(table);
            table.style.width = (tableWidth + delta) + "px";
            if(this.HasVertScroll())
                this.GetScrollHelper().Update();
        }

        // B219948
        if(ASPx.Browser.IE && ASPx.Browser.Version == 9 && this.tableHelper.IsHorzAutoScroll())
            this.GetContentTable().parentNode.className = this.GenerateRandomClassName();
    },

    ApplyColumnWidth: function(columnIndex, width) {
        var cellIndex = this.GetArmatureCellIndex(columnIndex);

        this.ApplyCellWidth(this.GetHeaderTable(), cellIndex, width);
        this.ApplyCellWidth(this.GetContentTable(), cellIndex, width);
        if(this.NeedResizeFooterTable())
            this.ApplyCellWidth(this.GetFooterTable(), cellIndex, width);
    },

    ApplyCellWidth: function(table, cellIndex, width) {
        if(!table)
            return;
        this.GetArmatureCells(table)[cellIndex].style.width = width;
    },

    IsNextColumnResizable: function() { },
    GetColumnIndices: function() { },
    GetColumnIndexById: function(id) { },
    GetColumnMinWidth: function(columnIndex) { },
    GetHeaderCell: function(columnIndex) { },
    GetArmatureCellIndex: function(columnIndex) { },
    GetScrollHelper: function() { },
    OnResized: function(columnIndex) { },
    NeedResizeFooterTable: function() { },
    GetControlDimensions: function() { },
    SetControlDimensions: function(dimensions) { },

    GetColumnNeighbor: function(columnIndex, isLeft) {
        var indices = this.GetColumnIndices();
	    var index = -1;
	    for(var i = 0; i < indices.length; i++) {
		    if(indices[i] === columnIndex) {
			    index = isLeft ? i - 1 : i + 1;
			    break;
		    }
	    }
	    if(index < 0 || index >= indices.length)
		    return -1;
	    return indices[index];
    },

    IsLeftmostColumn: function(columnIndex) {
        return this.GetColumnIndices()[0] === columnIndex;
    },

    IsRightmostColumn: function(columnIndex) {
        var indices = this.GetColumnIndices();
        return indices[indices.length - 1] === columnIndex;
    },

    SaveControlDimensions: function(onlyControlWidth) {
        if(onlyControlWidth && this.GetControlDimensions())
            return;
        var state = { };
        var controlWidthPx = ASPx.GetElementOriginalWidth(this.control.GetMainElement());
        var controlWidth = ASPx.PxToInt(controlWidthPx);
        if(controlWidth > 0 && !this.control.widthValueSetInPercentage)
            state["ctrlWidth"] = controlWidth;
        if(!onlyControlWidth) {
            var indices = this.GetColumnIndices();
            for(var i = 0; i < indices.length; i++) {
                var index = indices[i];
                state[index] = this.GetCellRealWidth(this.GetHeaderCell(index));
            }
        }
        this.SetControlDimensions(ASPx.Json.ToJson(state));
    },

    UpdateCursor: function(e, headerCell) {
	    var changed = true;
	    if(this.IsResizing() || this.CanStartResizing(e, headerCell))
		    ASPx.Attr.ChangeStyleAttribute(headerCell, "cursor", "w-resize");
	    else
		    changed = ASPx.Attr.RestoreStyleAttribute(headerCell, "cursor");
	    // B230643
	    if(ASPx.Browser.IE && ASPx.Browser.Version == 9 && this.tableHelper.IsHorzAutoScroll() && changed)
		    this.GetContentTable().parentNode.className = this.GenerateRandomClassName();
    },

    GetColumnInfoList: function() {
        var list = [ ];
        var indices = this.GetColumnIndices();

        for(var i = 0; i < indices.length; i++)
            list.push(this.GetColumnInfo(indices[i]));
        
        list.sort(function(i1, i2) { // sort ascending
            var w1 = i1.minWidth;
            var w2 = i2.minWidth;
            if(w1 > w2)
                return 1;
            else if(w1 < w2)
                return -1;
            else
                return 0;
        });

        return list;
    },

    GetColumnInfo: function(columnIndex) {
        var headerCell = this.GetHeaderCell(columnIndex);
        if(!headerCell) return { };
        var armCell = this.GetArmatureCell(headerCell);
        return {
            index: columnIndex,
            headerCell: headerCell,
            armCell: armCell,
            minWidth: this.GetColumnMinWidth(columnIndex),
            pxWidth: this.GetStylePxWidth(armCell),
            minWidthAssigned: armCell.minWidthAssigned
        };
    },

    ValidateColumnWidths: function() {
        if(this.HasHorzScroll()) 
            return;
        
        var indices = this.GetColumnIndices();
        var validationRequired = false;
        for(var i = 0; i < indices.length; i++) {
            if(this.GetColumnMinWidth(indices[i]) > 0)
                validationRequired = true;
        }
        if(!validationRequired)
            return;

        var columnInfoList = this.GetColumnInfoList();
        var totalColumnWidth = 0;
        var allColumnsHasWidth = true;
        for(var i = 0; i < columnInfoList.length && allColumnsHasWidth; i++) {
            var info = columnInfoList[i];
            allColumnsHasWidth &= info.pxWidth > 0;
            totalColumnWidth += info.pxWidth;
        }
        var tablePxWidth = this.GetStylePxWidth(this.control.GetMainElement());
        if(allColumnsHasWidth && tablePxWidth > -1 && tablePxWidth <= totalColumnWidth)
            return;

        var processedList = [ ];
        for(var i = 0; i < columnInfoList.length; i++) {
            var info = columnInfoList[i];
            if(info.minWidth > 0 && (info.pxWidth <= 0 || info.minWidthAssigned))
                processedList.push(info);
        }

        this.ValidateColumnWidthsCore(processedList);
        if(this.HasVertScroll())
            this.GetScrollHelper().Update();
    },

    ValidateColumnWidthsCore: function(columnInfoList) {
        // two passes
        for(var i = 0; i < columnInfoList.length; i++) {
            var info = columnInfoList[i];
            if(info.minWidthAssigned)
                info.minWidthAssigned = !this.TryRestoreColumnWidth(info);
            else
                info.minWidthAssigned = this.TryCorrectColumnWidth(info);
        }
        for(var i = 0; i < columnInfoList.length; i++) {
            var info = columnInfoList[i];
            if(!info.minWidthAssigned)
                this.TryCorrectColumnWidth(info);
        }
    },

    TryRestoreColumnWidth: function(columnInfo) {
        var armCell = columnInfo.armCell;

        armCell.style.width = armCell.savedWidth;
        if(armCell.offsetWidth <= columnInfo.minWidth) {
            armCell.style.width = columnInfo.minWidth + "px";
            return false;
        }

        this.ApplyColumnWidth(columnInfo.index, armCell.savedWidth);
        armCell.minWidthAssigned = false;
        armCell.savedWidth = null;
        return true;
    },

    TryCorrectColumnWidth: function(columnInfo) {
        var armCell = columnInfo.armCell;
        if(armCell.offsetWidth >= columnInfo.minWidth)
            return false;

        armCell.savedWidth = armCell.style.width;
        armCell.minWidthAssigned = true;
        this.ApplyColumnWidth(columnInfo.index, columnInfo.minWidth + "px");
        return true;
    },

    ResetStretchedColumnWidth: function() {
        if(this.HasHorzScroll())
            return;
        var columnIndex = this.FindStretchedColumn();
        if(columnIndex > -1)
            this.ApplyColumnWidth(columnIndex, "");
    },

    FindStretchedColumn: function() {
        var columnInfoList = this.GetColumnInfoList();
        if(columnInfoList.length == 0)
            return -1;
        for(var i = 0; i < columnInfoList.length; i++) {
            if(columnInfoList[i].pxWidth < 0)
                return -1;
        }
        var indices = this.GetColumnIndices();
        return indices[indices.length - 1];
    },

    GenerateRandomClassName: function() {
        return "dx" + Math.floor((Math.random() + 1) * 100000).toString(36);
    }
});

ASPx.Evt.AttachEventToElement(window, "resize", function(e) {
    if(!document.body) return; //B187536
    var width = document.body.offsetWidth;
    if(width == TableScrollHelperBase.prevBodyWidth)
        return;
    TableScrollHelperBase.prevBodyWidth = width;
    TableScrollHelperCollection.onResize();
});

ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseMoveEventName, function(e) {
    if(ASPx.tableColumnResizing != null && !(ASPx.Browser.WebKitTouchUI && ASPx.TouchUIHelper.isGesture)) {
        ASPx.tableColumnResizing.OnMouseMove(e);
        return true;
    }
});
ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseUpEventName, function(e) {
    if(ASPx.tableColumnResizing != null) {
        ASPx.tableColumnResizing.OnMouseUp(e);
        return true;
    }
});

ASPx.TableHelperBase = TableHelperBase;
ASPx.TableHelperAdapter = TableHelperAdapter;
ASPx.TableScrollHelperBase = TableScrollHelperBase;
ASPx.TableResizingHelperBase = TableResizingHelperBase;
})();