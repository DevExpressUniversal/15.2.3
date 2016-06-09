(function() {
    ASPxClientSpreadsheet.ScrollHelper = ASPx.CreateClass(null, {
        constructor: function(paneManager) {
            this.paneManager = paneManager;
            this.tileSize = this.paneManager.getTileSize();
            this.scrollAnchor = { col: -1, row: -1 };

            this.spacer = 50000 + 1; // header border

            this.lastVisibleCell = { col: 0, row: 0 };

            this.loadTilesTimerID = -1;
            this.loadTilesTimeout = 50;

            this.savedScrollLeft = -1;
            this.savedScrollTop = -1;
            this.contentScrollPosition = { left: -1, top: -1 };
            this.lastScrollingCell = { col: -1, row: -1 };
            this.scrollAnchorLayoutInfo = null;
            this.minimumScrollTopPosition = 0;
            this.minimumScrollLeftPosition = 0;

            this.widthBetweenFrozenCellAndScrollAnchor = 0;
            this.heightBetweenFrozenCellAndScrollAnchor = 0;
        },
        getDefaultCellSize: function() { return this.paneManager.getDefaultCellSize(); },
        getRenderProvider: function() { return this.paneManager.getRenderProvider(); },
        getScrollDiv: function() { return this.getRenderProvider().getScrollContainer(); },
        getScrollContent: function() { return this.getRenderProvider().getScrollContent(); },
        getGridContainer: function() { return this.getRenderProvider().getGridContainer(); },
        getColumnHeader: function() { return this.getRenderProvider().getColumnHeader(); },
        getRowHeader: function() { return this.getRenderProvider().getRowHeader(); },
        getTileHelper: function() { return this.paneManager.getTileHelper(); },
        getVisibleTileRange: function() { return this.getTileHelper().getVisibleRange(); },
        getSpacer: function() {
            return this.spacer;
        },

        getScrollAnchorVisiblePosition: function() { return this.scrollAnchor; },
        getScrollAnchorModelPosition: function() {
            var scrollAnchorVisiblePosition = this.getScrollAnchorVisiblePosition();
            return {
                col: this.paneManager.convertVisibleIndexToModelIndex(scrollAnchorVisiblePosition.col, true),
                row: this.paneManager.convertVisibleIndexToModelIndex(scrollAnchorVisiblePosition.row, false)
            };
        },
        setScrollAnchor: function(anchor, force) {
            if(anchor && (force || (this.scrollAnchor.col !== anchor.col || this.scrollAnchor.row !== anchor.row))) {
                this.scrollAnchor = anchor;
                this.onScrollAnchorChanged();
            }
        },
        getScrollAnchorLayoutInfo: function() {
            return this.scrollAnchorLayoutInfo;
        },
        updateScrollAnchorLayoutInfo: function() {
            var anchor = this.getScrollAnchorVisiblePosition();
            this.scrollAnchorLayoutInfo = this.paneManager.getCellLayoutInfo(anchor.col, anchor.row);
        },
        getUpdatedScrollAnchorLayoutInfoIfRequired: function() {
            var cellLayoutInfo = this.getScrollAnchorLayoutInfo();
            var scrollAnchor = this.getScrollAnchorVisiblePosition();
            if(cellLayoutInfo === null || cellLayoutInfo.colIndex !== scrollAnchor.col || cellLayoutInfo.rowIndex !== scrollAnchor.row) {
                this.updateScrollAnchorLayoutInfo();
                return this.getScrollAnchorLayoutInfo();
            }
            return cellLayoutInfo;
        },

        getLastVisibleCell: function() {
            var lastFilledCell = this.paneManager.getLastFilledCell();
            return {
                col: Math.max(this.lastVisibleCell.col, lastFilledCell.col),
                row: Math.max(this.lastVisibleCell.row, lastFilledCell.row)
            };
        },

        createTouchUIScroller: function() {
            if(ASPx.Browser.WebKitTouchUI)
                this.touchUIScroller = new ASPx.TouchUIHelper.ScrollExtender(this.getScrollDiv(),
                        { forceCustomScroll: true, touchEventHandlersElement: this.getGridContainer(), acceleration: 0 });
            if(ASPx.Browser.MSTouchUI) {
                var grid = this.getGridContainer();
                var scrollElement = this.getScrollDiv();
                var onMsTouchMouseDown = function(e) {
                    if(this.paneManager.getRenderProvider().getIsTouchSelectionElement(ASPx.Evt.GetEventSource(e)) ||
                        this.paneManager.getStateController().isDrawingBoxDragingOrResizingInProcess() ||
                        this.paneManager.getRenderProvider().getIsCellEditingElement(ASPx.Evt.GetEventSource(e)))
                        return;
                    this.initMSScrollLeft = scrollElement.scrollLeft;
                    this.initMSScrollTop = scrollElement.scrollTop;
                    this.initMSPageX = e.pageX;
                    this.initMSPageY = e.pageY;
                    ASPx.Evt.AttachEventToElement(grid, "mousemove", onMsTouchMouseMove);
                }.aspxBind(this);
                var onMsTouchMouseMove = function(e) {
                    var newX = this.initMSScrollLeft + (this.initMSPageX - e.pageX);
                    var newY = this.initMSScrollTop + (this.initMSPageY - e.pageY);
                    scrollElement.scrollLeft = newX;
                    scrollElement.scrollTop = newY;
                }.aspxBind(this);
                ASPx.Evt.AttachEventToElement(grid, "mousedown", onMsTouchMouseDown);
                ASPx.Evt.AttachEventToElement(grid, "mouseup", function(e) {
                    ASPx.Evt.DetachEventFromElement(grid, "mousemove", onMsTouchMouseMove);
                });
            }
        },

        //TODO to zhuravlev - think about recalculate optimization
        getMinimumScrollTopPosition: function() {
            var settings = this.paneManager.getFrozenPaneSettings(),
                frozenCell = settings.frozenCell;

            this.heightBetweenFrozenCellAndScrollAnchor = frozenCell ? this.getScrollAreaHeightSizeByIndex(this.paneManager.convertModelIndexToVisibleIndex(frozenCell.row, false)) : 0;
            return this.heightBetweenFrozenCellAndScrollAnchor;
        },
        getMinimumScrollLeftPosition: function() {
            var settings = this.paneManager.getFrozenPaneSettings(),
                frozenCell = settings.frozenCell;

            this.widthBetweenFrozenCellAndScrollAnchor = frozenCell ? this.getScrollAreaWidthSizeByIndex(this.paneManager.convertModelIndexToVisibleIndex(frozenCell.col, true)) : 0;
            return this.widthBetweenFrozenCellAndScrollAnchor;
        },

        onScrollAnchorChanged: function() {
            if(ASPx.Browser.TouchUI)
                return;
            var scrollAnchor = this.getScrollAnchorVisiblePosition(),
                scrollDiv = this.getScrollDiv();

            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                ASPx.Attr.ChangeStyleAttribute(scrollDiv, "visibility", "hidden");

            scrollDiv.scrollLeft = this.getScrollAreaWidthSizeByIndex(scrollAnchor.col) - this.getMinimumScrollLeftPosition();
            scrollDiv.scrollTop = this.getScrollAreaHeightSizeByIndex(scrollAnchor.row) - this.getMinimumScrollTopPosition();

            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                ASPx.Attr.RestoreStyleAttribute(scrollDiv, "visibility");
        },

        updateScrollPosition: function() {
            this.ensureScrollPosition(true);
        },

        //use only when get data from server
        processDocumentResponse: function(response) {
            var scrollAnchorInVisibleRange = response.scrollAnchor,
                force = response.clearGridCache;

            if(response.scrollAnchor) {
                scrollAnchorInVisibleRange = {
                    col: this.paneManager.convertModelIndexToVisibleIndex(response.scrollAnchor.col, true),
                    row: this.paneManager.convertModelIndexToVisibleIndex(response.scrollAnchor.row, false)
                };
            } else { //scroll anchor visible position was changed - in freeze panes mode
                var settings = this.paneManager.getFrozenPaneSettings();
                if(settings.isFrozen) {
                    var scrollAnchorVisiblePosition = this.getScrollAnchorVisiblePosition(),
                        scrollAnchorModelPosition = this.getScrollAnchorModelPosition(),
                        updatedScrollAnchorCol = this.paneManager.convertModelIndexToVisibleIndex(scrollAnchorModelPosition.col, true),
                        updatedScrollAnchorRow = this.paneManager.convertModelIndexToVisibleIndex(scrollAnchorModelPosition.row, false);
                    if(scrollAnchorVisiblePosition.row !== updatedScrollAnchorRow || scrollAnchorVisiblePosition.col !== updatedScrollAnchorCol) {
                        scrollAnchorInVisibleRange = {
                            col: updatedScrollAnchorCol,
                            row: updatedScrollAnchorRow
                        }
                    }
                }
            }
            this.applyScrollAnchor(scrollAnchorInVisibleRange, force);
        },
        applyScrollAnchor: function(scrollAnchor, force) {
            this.assignHandlers();
            this.updateScrollSize(true);
            this.setScrollAnchor(scrollAnchor, force);
            this.updateScrollPosition();
        },
        assignHandlers: function() {
            var scrollDiv = this.getScrollDiv();
            if(!scrollDiv.dxScrollHandlerAssigned) {
                ASPx.Evt.AttachEventToElement(scrollDiv, "scroll", function(e) { this.onScroll(e); }.aspxBind(this));
                ASPx.Evt.AttachEventToElement(scrollDiv, "mousedown", function(e) { this.onMouseDown(e); }.aspxBind(this));
                scrollDiv.dxScrollHandlerAssigned = true;
            }
            var mainDiv = this.paneManager.getRenderProvider().getWorkbookControl();
            if(ASPx.IsExists(mainDiv) && !mainDiv.dxWheelHandlerAssigned) {
                ASPx.Evt.AttachEventToElement(mainDiv, ASPx.Browser.Firefox ? "wheel" : "mousewheel", function(e) { this.onMouseWheel(e); }.aspxBind(this));
                mainDiv.dxWheelHandlerAssigned = true;
            }

            if(ASPx.Browser.IE)
                ASPx.Evt.AttachEventToElement(this.getGridContainer(), "scroll", function(e) { this.onGridScrollIE(e); }.aspxBind(this));
        },

        onGridScrollIE: function(e) {
            if(this.restoreScrollPositionRequired()) {
                var div = this.getScrollDiv(),
                    col = this.getColCountByWidth(div.scrollLeft + this.getMinimumScrollLeftPosition()),
                    row = this.getRowCountByHeight(div.scrollTop + this.getMinimumScrollTopPosition());

                if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                    ASPx.Attr.ChangeStyleAttribute(this.getGridContainer(), "visibility", "hidden");

                var scrollInfo = this.getScrollInfo(this.getTileHelper(), col, true);
                this.getGridContainer().scrollLeft = scrollInfo.gridScrollValue;

                scrollInfo = this.getScrollInfo(this.getTileHelper(), row, false);
                this.getGridContainer().scrollTop = scrollInfo.gridScrollValue;

                if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                    ASPx.Attr.RestoreStyleAttribute(this.getGridContainer(), "visibility");
            }
        },

        onScroll: function(e) {
            if(!this.paneManager.isSheetLoaded()) return; // WORKAROUND for IE9-
            this.ensureScrollPosition();
            if(ASPx.Browser.TouchUI) {
                this.increaseScrollableAreaProportionally();
                var selection = this.paneManager.getStateController().getSelection();
                if(selection.entireColsSelected || selection.entireRowsSelected)
                    this.paneManager.moveTouchSelectionElementsOnScroll(selection);
            }
            this.paneManager.moveDataValidationElementsOnScroll(selection);
        },
        onMouseWheel: function(e) {
            if(!e.ctrlKey) {
                if(ASPx.Browser.Chrome || ASPx.Browser.Safari)
                    this.setScrollDependingOnDirection(e);
                else
                    this.setScroll(ASPx.Browser.Firefox ? -e.deltaY : ASPx.Evt.GetWheelDelta(e), this.getDefaultCellSize().height, false);
                return ASPx.Evt.PreventEvent(e);
            }
        },
        setScrollDependingOnDirection: function(e) {
            if(Math.abs(e.wheelDeltaY) >= Math.abs(e.wheelDeltaX))
                this.setScroll(e.wheelDeltaY, this.getDefaultCellSize().height, false);
            else
                this.setScroll(e.wheelDeltaX, this.getDefaultCellSize().width, true);
        },
        setScroll: function(wheelDelta, defaultCellSize, isHorz) {
            var deltaDivider = ASPx.Browser.NetscapeFamily ? 3 : 30;
            var div = this.getScrollDiv();
            var absDelta = Math.ceil(Math.abs(wheelDelta / deltaDivider));
            var delta = wheelDelta > 0 ? absDelta : -absDelta;
            var scrollDelta = -1 * delta * ASPxClientSpreadsheet.ScrollHelper.Constants.ScrollTopRowStep * defaultCellSize;

            this.increaseScrollableAreaIfNeeded(isHorz);

            isHorz ? div.scrollLeft += scrollDelta : div.scrollTop += scrollDelta;
        },
        onMouseDown: function(e) {
            var x = ASPx.Evt.GetEventX(e);
            var y = ASPx.Evt.GetEventY(e);

            var scrollDiv = this.getScrollDiv();
            var scrollDivX = ASPx.GetAbsoluteX(scrollDiv);
            var scrollDivY = ASPx.GetAbsoluteY(scrollDiv);
            var scrollDivWidth = ASPx.PxToInt(scrollDiv.style.width);
            var scrollDivHeight = ASPx.PxToInt(scrollDiv.style.height);

            var scrollSize = ASPx.GetVerticalScrollBarWidth();

            var xOffset = scrollDivWidth - x + scrollDivX;
            var yOffset = scrollDivHeight - y + scrollDivY;
            var isBottomButton = xOffset <= scrollSize && yOffset <= 2 * scrollSize && yOffset >= scrollSize;
            var isRightButton = yOffset <= scrollSize && xOffset <= 2 * scrollSize && xOffset >= scrollSize;

            if(!isBottomButton && !isRightButton) {
                this.paneManager.getStateController().onLeftTopRectMouseDown(e);
                return;
            }

            if(isRightButton)
                this.increaseScrollableAreaIfNeeded(true);
            if(isBottomButton)
                this.increaseScrollableAreaIfNeeded(false);
        },
        increaseScrollableAreaProportionally: function() {
            this.increaseScrollableAreaIfNeeded(true);
            this.increaseScrollableAreaIfNeeded(false);
        },
        increaseScrollableAreaIfNeeded: function(horz) {
            if(horz && this.getHorzScrollRightSpace() === 0)
                this.changeScrollableArea(this.tileSize.col, horz);
            if(!horz && this.getVertScrollBottomSpace() === 0)
                this.changeScrollableArea(this.tileSize.row, horz);
        },
        changeScrollableArea: function(colOrRowSize, isHorz) {
            var lastVisibleCell = this.getLastVisibleCell();
            if(isHorz)
                lastVisibleCell.col += colOrRowSize;
            else
                lastVisibleCell.row += colOrRowSize;
            this.lastVisibleCell = lastVisibleCell;
            this.updateScrollSize();
        },
        ensureScrollPosition: function(forceChanged) {
            var div = this.getScrollDiv();
            var left = Math.round(div.scrollLeft + this.getMinimumScrollLeftPosition());
            var top = Math.round(div.scrollTop + this.getMinimumScrollTopPosition());

            if(this.restoreScrollPositionRequired())
                div.scrollLeft = left = this.contentScrollPosition.left;
            if(this.restoreScrollPositionRequired())
                div.scrollTop = top = this.contentScrollPosition.top;

            var col = this.getColCountByWidth(left);
            var row = this.getRowCountByHeight(top);
            var scrollAnchor = this.getScrollAnchorVisiblePosition();
            var colChanged = col !== scrollAnchor.col || forceChanged;
            var rowChanged = row !== scrollAnchor.row || forceChanged;
            var scrollToLeft = col < scrollAnchor.col;
            var scrollToTop = row < scrollAnchor.row;

            if(!colChanged && !rowChanged)
                return;
            this.setScrollAnchor({ col: col, row: row });
            this.ensureScrolledTiles(col, row, scrollToLeft, scrollToTop);
            var scrolledCellColumnInfo = null;

            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                ASPx.Attr.ChangeStyleAttribute(this.getGridContainer(), "visibility", "hidden");

            if(colChanged) {
                scrolledCellColumnInfo = this.getScrollInfo(this.getTileHelper(), col, true);
                this.getGridContainer().scrollLeft = scrolledCellColumnInfo.gridScrollValue;
                this.getColumnHeader().scrollLeft = scrolledCellColumnInfo.headerScrollValue;

                this.saveScrollLeftPosition(this.getGridContainer().scrollLeft);
            }

            var scrolledCellRowInfo = null;
            if(rowChanged) {
                scrolledCellRowInfo = this.getScrollInfo(this.getTileHelper(), row, false);
                this.getGridContainer().scrollTop = scrolledCellRowInfo.gridScrollValue;
                this.getRowHeader().scrollTop = scrolledCellRowInfo.headerScrollValue;

                this.saveScrollTopPosition(this.getGridContainer().scrollTop);
            }

            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                ASPx.Attr.RestoreStyleAttribute(this.getGridContainer(), "visibility");

            var settings = this.paneManager.getFrozenPaneSettings();
            if(settings && settings.topLeftCell) {
                this.updateScrollPositionsOnHoldenPanes(settings);
                this.paneManager.setScrollPositionOnHoldenPanes(scrolledCellColumnInfo, scrolledCellRowInfo, this.minimumScrollLeftPosition, this.minimumScrollTopPosition);
            }

            this.saveContentScrollPosition(div);
        },
        onFrozenPaneSettingsChanged: function() {
            this.updateScrollAfterMainPane = true;
        },
        updateScrollPositionsOnHoldenPanes: function(settings) {
            if(this.updateScrollAfterMainPane || this.topLeftCellPositionChanged(settings)) {
                this.updateScrollAfterMainPane = false;
                if(settings && settings.topLeftCell) {
                    var topLeftCell = settings.topLeftCell,
                        mode = settings.mode,
                        tileHelper = this.paneManager.getTileHelperByCellPostition(topLeftCell),
                        col = this.paneManager.convertModelIndexToVisibleIndex(topLeftCell.col, true),
                        row = this.paneManager.convertModelIndexToVisibleIndex(topLeftCell.row, false),
                        scrollColumnInfo = this.getScrollInfo(tileHelper, col, true),
                        scrollRowInfo = this.getScrollInfo(tileHelper, row, false);

                    this.topLeftCellVisiblePosition = { col: col, row: row };
                    this.minimumScrollTopPosition = mode !== 1 ? scrollRowInfo.gridScrollValue : 0;
                    this.minimumScrollLeftPosition = mode !== 0 ? scrollColumnInfo.gridScrollValue : 0;
                } else {
                    this.minimumScrollTopPosition = 0;
                    this.minimumScrollLeftPosition = 0;
                }
            }
        },
        topLeftCellPositionChanged: function(settings) {
            var currentTopLeftCell = this.topLeftCellVisiblePosition,
                newTopLeftCellVisiblePosition = {
                    col: this.paneManager.convertModelIndexToVisibleIndex(settings.topLeftCell.col, true),
                    row: this.paneManager.convertModelIndexToVisibleIndex(settings.topLeftCell.row, false)
                };
            return currentTopLeftCell && (currentTopLeftCell.col !== newTopLeftCellVisiblePosition.col || currentTopLeftCell.row !== newTopLeftCellVisiblePosition.row);
        },

        getScrollContentAreaWidthSize: function() {
            var lastVisibleCell = this.getLastVisibleCell();
            var colIndex = lastVisibleCell.col;
            var tileIndex = Math.floor(colIndex / this.tileSize.col);

            var size = 0;
            var counterIndex = 0;
            for(counterIndex; counterIndex < tileIndex; counterIndex++) {
                var headerInfo = this.paneManager.getHeaderInfo(counterIndex, true);
                if(headerInfo)
                    size += headerInfo.totalWidth;
                else break;
            }

            var headerWidth = this.paneManager.getHeaderInfo(counterIndex, true);
            var lastColInRange = 0;
            if(headerWidth) {
                lastColInRange = colIndex - counterIndex * this.tileSize.col;
                lastColInRange = lastColInRange + ASPxClientSpreadsheet.ScrollHelper.Constants.AdditionalColToView > headerWidth.incrementalWidths.length - 1 ?
                            headerWidth.incrementalWidths.length - 1 : lastColInRange + ASPxClientSpreadsheet.ScrollHelper.Constants.AdditionalColToView;
                size += headerWidth.incrementalWidths[lastColInRange];
            }
            if((lastColInRange + counterIndex * this.tileSize.col) != colIndex) {
                colIndex -= (lastColInRange + counterIndex * this.tileSize.col - ASPxClientSpreadsheet.ScrollHelper.Constants.AdditionalColToView);
                if(colIndex > 0)
                    size += this.getDefaultCellSize().width * (colIndex + 1);
            }
            var controlVisibleWidth = this.getGridContainer().offsetWidth;
            if(controlVisibleWidth < size) {
                size = size + controlVisibleWidth - this.getDefaultCellSize().width;
            }
            return size;
        },
        getScrollContentAreaHeightSize: function() {
            var lastVisibleCell = this.getLastVisibleCell();
            var rowIndex = lastVisibleCell.row;
            var tileIndex = Math.floor(rowIndex / this.tileSize.row);

            var size = 0;
            var counterIndex = 0;
            for(counterIndex; counterIndex < tileIndex; counterIndex++) {
                var headerInfo = this.paneManager.getHeaderInfo(counterIndex, false);
                if(headerInfo)
                    size += headerInfo.totalHeight;
                else break;
            }

            var headerWidth = this.paneManager.getHeaderInfo(counterIndex, false);
            var lastRowInRange = 0;
            if(headerWidth) {
                lastRowInRange = rowIndex - counterIndex * this.tileSize.row;
                lastRowInRange = lastRowInRange + ASPxClientSpreadsheet.ScrollHelper.Constants.AdditionalRowToView >= headerWidth.incrementalHeights.length - 1 ?
                            headerWidth.incrementalHeights.length - 1 : lastRowInRange + ASPxClientSpreadsheet.ScrollHelper.Constants.AdditionalRowToView;
                size += headerWidth.incrementalHeights[lastRowInRange];
            }
            if((lastRowInRange + counterIndex * this.tileSize.row) != rowIndex) {
                rowIndex = rowIndex - (lastRowInRange + counterIndex * this.tileSize.row - ASPxClientSpreadsheet.ScrollHelper.Constants.AdditionalRowToView);
                if(rowIndex > 0)
                    size += this.getDefaultCellSize().height * (rowIndex + 1);
            }
            var controlVisibleHeight = this.getGridContainer().offsetHeight;
            if(controlVisibleHeight < size) {
                size = size + controlVisibleHeight - this.getDefaultCellSize().height;
            }
            return size;
        },

        getScrollAreaWidthSizeByIndex: function(colIndex) {
            var tileIndex = Math.floor(colIndex / this.tileSize.col);

            var size = 0;
            var counterIndex = 0;
            for(counterIndex; counterIndex < tileIndex; counterIndex++) {
                var headerInfo = this.paneManager.getHeaderInfo(counterIndex, true);
                if(headerInfo)
                    size += headerInfo.totalWidth;
                else break;
            }

            var headerWidth = this.paneManager.getHeaderInfo(counterIndex, true);
            var lastColInRange = 0;
            if(headerWidth) {
                lastColInRange = colIndex - counterIndex * this.tileSize.col;
                lastColInRange = lastColInRange > headerWidth.incrementalWidths.length ? headerWidth.incrementalWidths.length - 1 : lastColInRange;
                size += headerWidth.incrementalWidths[lastColInRange];
            }
            if((lastColInRange + counterIndex * this.tileSize.col) != colIndex) {
                colIndex -= (lastColInRange + counterIndex * this.tileSize.col);
                if(colIndex > 0)
                    size += this.getDefaultCellSize().width * colIndex;
            }
            return size;
        },
        getScrollAreaHeightSizeByIndex: function(rowIndex) {
            var tileIndex = Math.floor(rowIndex / this.tileSize.row);

            var size = 0;
            var counterIndex = 0;
            for(counterIndex; counterIndex < tileIndex; counterIndex++) {
                var headerInfo = this.paneManager.getHeaderInfo(counterIndex, false);
                if(headerInfo)
                    size += headerInfo.totalHeight;
                else break;
            }

            var headerWidth = this.paneManager.getHeaderInfo(counterIndex, false);
            var lastRowInRange = 0;
            if(headerWidth) {
                lastRowInRange = rowIndex - counterIndex * this.tileSize.row;
                lastRowInRange = lastRowInRange > headerWidth.incrementalHeights.length ? headerWidth.incrementalHeights.length - 1 : lastRowInRange;
                size += headerWidth.incrementalHeights[lastRowInRange];
            }
            if((lastRowInRange + counterIndex * this.tileSize.row) != rowIndex) {
                rowIndex = rowIndex - (lastRowInRange + counterIndex * this.tileSize.row);
                if(rowIndex > 0)
                    size += this.getDefaultCellSize().height * rowIndex;
            }
            return size;
        },

        getColCountByWidth: function(width) {
            var summarySize = 0,
                counterIndex = 0,
                colIndex = 0;
            while(summarySize < width) {
                var headerInfo = this.paneManager.getHeaderInfo(counterIndex, true);
                if(headerInfo) {
                    if(width < headerInfo.totalWidth) { //col in this tile
                        for(var i = 0; i < headerInfo.widths.length; i++) {
                            summarySize += headerInfo.widths[i];
                            if(width < summarySize) {
                                colIndex += i;
                                break;
                            }
                        }
                    } else {
                        counterIndex++;
                        width -= headerInfo.totalWidth;
                        colIndex += this.tileSize.col;
                    }
                } else break;
            }
            if(width - summarySize > 0)
                colIndex += Math.floor(width / this.getDefaultCellSize().width);
            return colIndex;
        },
        getRowCountByHeight: function(height) {
            var summarySize = 0,
                counterIndex = 0,
                colIndex = 0;
            while(summarySize < height) {
                var headerInfo = this.paneManager.getHeaderInfo(counterIndex, false);
                if(headerInfo) {
                    if(height < headerInfo.totalHeight) { //col in this tile
                        for(var i = 0; i < headerInfo.heights.length; i++) {
                            summarySize += headerInfo.heights[i];
                            if(height < summarySize) {
                                colIndex += i;
                                break;
                            }
                        }
                    } else {
                        counterIndex++;
                        height -= headerInfo.totalHeight;
                        colIndex += this.tileSize.col;
                    }
                } else break;
            }
            if(height - summarySize > 0)
                colIndex += Math.floor(height / this.getDefaultCellSize().height);
            return colIndex;
        },

        restoreScrollPositionRequired: function() {
            if(this.savedScrollTop > -1 && this.savedScrollTop !== this.getVerticalScrollPosition())
                return true;
            if(this.savedScrollLeft > -1 && this.savedScrollLeft !== this.getHorizontalScrollPosition())
                return true;
            return false;
        },
        saveScrollLeftPosition: function(value) {
            this.savedScrollLeft = value;
        },
        saveScrollTopPosition: function(value) {
            this.savedScrollTop = value;
        },
        saveContentScrollPosition: function(divElement) {
            this.contentScrollPosition.top = divElement.scrollTop;
            this.contentScrollPosition.left = divElement.scrollLeft
        },
        getVerticalScrollPosition: function() {
            var gridContainer = this.getGridContainer()
            if(gridContainer)
                return gridContainer.scrollTop;
            return 0;
        },
        getHorizontalScrollPosition: function() {
            var gridContainer = this.getGridContainer()
            if(gridContainer)
                return gridContainer.scrollLeft;
            return 0;
        },

        updateScrollSize: function(recalculate) {
            var contentDiv = this.getScrollContent();
            var scrollDiv = this.getScrollDiv();

            var minWidth = scrollDiv.offsetWidth + this.getDefaultCellSize().width;
            var minHeight = scrollDiv.offsetHeight + this.getDefaultCellSize().height;

            var lastVisibleCell = this.getLastVisibleCell();
            if(this.getLastScrollingCell().col == lastVisibleCell.col && this.getLastScrollingCell().row == lastVisibleCell.row && !recalculate)
                return;

            var calcWidth = this.getScrollContentAreaWidthSize() - this.getMinimumScrollLeftPosition();
            var calcHeight = this.getScrollContentAreaHeightSize() - this.getMinimumScrollTopPosition();

            this.setLastScrollingCell(this.getLastVisibleCell())

            var maxWidth = Math.max(minWidth, calcWidth),
                maxHeight = Math.max(minHeight, calcHeight);

            ASPx.SetStyles(contentDiv, {
                width: maxWidth + "px",
                height: maxHeight + "px"
            });
            if(ASPx.Browser.IE)
                this.extendScrollableAreaForIE(maxWidth, maxHeight);
        },
        extendScrollableAreaForIE: function(maxWidth, maxHeight) {
            this.increaseScrollableAreaSize(maxWidth, true);
            this.increaseScrollableAreaSize(maxHeight, false);
        },
        increaseScrollableAreaSize: function(size, isCol) {
            var maxElementSize = 1100000,
                maxIEElementSize = 1533917;
            if(size > maxElementSize) {
                var scrollDiv = this.getScrollDiv(),
                    cssClassName = isCol ? "dxss-hea" : "dxss-vea",
                    styleKey = isCol ? "width" : "height";

                this.removeExtendedElements(ASPx.GetNodesByClassName(scrollDiv, cssClassName));
                size -= maxIEElementSize;
                while(size > 0) {
                    var extendedElement = document.createElement("div");
                    var elementSize = size >= maxElementSize ? maxElementSize : size;

                    extendedElement.style[styleKey] = elementSize + "px";
                    extendedElement.className = cssClassName;
                    size -= elementSize;
                    scrollDiv.appendChild(extendedElement);
                }
            }
        },
        removeExtendedElements: function(elements) {
            if(elements.length > 0) {
                var scrollDiv = this.getScrollDiv();
                for(var index = 0; index < elements.length; index++)
                    scrollDiv.removeChild(elements[index]);
            }
        },

        setLastScrollingCell: function(cell) {
            this.lastScrollingCell = cell;
        },
        getLastScrollingCell: function() {
            return this.lastScrollingCell
        },
        ensureScrolledTiles: function(col, row, scrollToLeft, scrollToTop) {
            var tileHelper = this.getTileHelper();
            var gridScrollBounds = tileHelper.getScrollBounds();
            if(this.isColFitToScrollBounds(col, scrollToLeft, gridScrollBounds) && this.isRowFitToScrollBounds(row, scrollToTop, gridScrollBounds))
                return;
            if(this.paneManager.tryLoadTilesFromCache()) {
                this.paneManager.getStateController().updateSelectionRender();
                return;
            }
            var headerScrollBounds = tileHelper.getHeaderScrollBounds();
            if(!this.isColFitToScrollBounds(col, scrollToLeft, headerScrollBounds) || !this.isRowFitToScrollBounds(row, scrollToTop, headerScrollBounds))
                tileHelper.loadHeaderTiles(); //TODO only scrolledTile can be there
            this.loadScrolledTiles();
        },

        loadScrolledTiles: function() {
            this.loadTilesTimerID = ASPx.Timer.ClearTimer(this.loadTilesTimerID);
            this.loadTilesTimerID = window.setTimeout(function() {
                this.getTileHelper().loadTilesAccordingToScrollAnchor();
            }.aspxBind(this), this.loadTilesTimeout)
        },
        getScrollInfo: function(tileHelper, index, isCol) {
            var gridVisibleRange = tileHelper.getVisibleRange(),
                headerVisibleRange = tileHelper.getHeaderVisibleRange(),
                gridScrollValue,
                headerScrollValue;

            gridScrollValue = headerScrollValue = this.calculateScrollPos(tileHelper, index, gridVisibleRange, isCol);
            if(!this.areRangesEqualByOneDirection(gridVisibleRange, headerVisibleRange, isCol))
                headerScrollValue = this.calculateScrollPos(tileHelper, index, headerVisibleRange, isCol);

            return { gridScrollValue: gridScrollValue, headerScrollValue: headerScrollValue };
        },
        calculateScrollPos: function(tileHelper, index, visibleRange, isCol) {
            var cellRange = this.getCellRange(visibleRange, isCol);

            var outsideLeft = cellRange.start - index > 0;
            var startCellIndex = outsideLeft ? index : cellRange.start;
            var endCellIndex = outsideLeft ? cellRange.start : index;
            var cellSize = this.getTileHelper().calculateCellSize(startCellIndex, endCellIndex - 1, isCol);

            return this.spacer + (outsideLeft ? -1 : 1) * cellSize;
        },
        areRangesEqualByOneDirection: function(r1, r2, isCol) { // TODO rename -> areRangesEqualByOneDirection
            if(isCol && (r1.left !== r2.left || r1.right !== r2.right))
                return false;
            if(!isCol && (r1.top !== r2.top || r1.bottom !== r2.bottom))
                return false;
            return true;
        },
        getCellRange: function(tileRange, isCol) {
            var left = isCol ? tileRange.left : tileRange.top;
            var right = isCol ? tileRange.right : tileRange.bottom;
            var tileSize = isCol ? this.tileSize.col : this.tileSize.row;
            return { start: left * tileSize, end: (right + 1) * tileSize - 1 };
        },

        isColFitToScrollBounds: function(col, scrollToLeft, scrollBounds) {
            return scrollToLeft && col >= scrollBounds.left || !scrollToLeft && col <= scrollBounds.right;
        },
        isRowFitToScrollBounds: function(row, scrollToTop, scrollBounds) {
            return scrollToTop && row >= scrollBounds.top || !scrollToTop && row <= scrollBounds.bottom;
        },

        getHorzScrollRightSpace: function() {
            var scrollDiv = this.getScrollDiv();
            return scrollDiv.scrollWidth - scrollDiv.clientWidth - scrollDiv.scrollLeft;
        },
        getVertScrollBottomSpace: function() {
            var scrollDiv = this.getScrollDiv();
            return scrollDiv.scrollHeight - scrollDiv.clientHeight - scrollDiv.scrollTop;
        },
        correctScrollPostitionsWhenEditingStarted: function(elementSizeRect, horizontalScrollCorrectRequired, verticalScrollCorrectRequired) {
            var colCount = 0,
                rowCount = 0;
            if(horizontalScrollCorrectRequired) {
                colCount = this.getCorrectionCellCount(elementSizeRect.width, true);
            }
            if(verticalScrollCorrectRequired) {
                rowCount = this.getCorrectionCellCount(elementSizeRect.height, false);
            }
            if(colCount > 0 || rowCount > 0) {
                if(colCount > 0)
                    this.increaseScrollableAreaIfNeeded(true);
                if(rowCount > 0)
                    this.increaseScrollableAreaIfNeeded(false)
                var firstVisibleCell = this.getScrollAnchorVisiblePosition();
                var anchor = { row: firstVisibleCell.row + rowCount, col: firstVisibleCell.col + colCount };
                this.applyScrollAnchor(anchor);
            }
        },
        getCorrectionCellCount: function(elementSize, isCol) {
            var firstVisibleCell = this.getScrollAnchorVisiblePosition();
            var currentSelection = this.paneManager.getStateController().getSelection();
            var cellCount = 0,
                totalSize = 0;

            var beginCellIndex = isCol ? firstVisibleCell.col : firstVisibleCell.row
            var endCellIndex = isCol ? currentSelection.activeCellColIndex : currentSelection.activeCellRowIndex;

            for(var cellIndex = beginCellIndex; cellIndex < endCellIndex; cellIndex++) {
                var sizeSize = this.paneManager.getCellSize(cellIndex, isCol);
                totalSize += sizeSize;
                cellCount++;
                if(totalSize >= elementSize)
                    break;
            }
            return cellCount;
        },

        isElementPartiallyVisible: function(elementRect, cellLayoutInfo) {
            var tileInfo = cellLayoutInfo.tileInfo,
                result = { verticalPartiallyVisible: false, horizontalPartiallyVisible: false, fullInvisible: false };

            var gridContainer = this.getRenderProvider().getGridContainer(cellLayoutInfo.paneType);

            var anchorCellLayoutInfo = this.paneManager.getPaneTopLeftCellLayoutInfo(cellLayoutInfo.paneType);
            var elementCorrectWidth = elementRect.x,
                elementCorrectHeight = elementRect.y;
            if(anchorCellLayoutInfo.tileInfo !== tileInfo) {
                if(anchorCellLayoutInfo.tileInfo.colIndex != tileInfo.colIndex)
                    elementCorrectWidth += this.getAdditionalCorrectSize(anchorCellLayoutInfo.tileInfo, tileInfo, true);
                if(anchorCellLayoutInfo.tileInfo.rowIndex != tileInfo.rowIndex)
                    elementCorrectHeight += this.getAdditionalCorrectSize(anchorCellLayoutInfo.tileInfo, tileInfo, false);
            }

            result.horizontalPartiallyVisible = elementCorrectWidth - anchorCellLayoutInfo.rect.x + elementRect.width > gridContainer.clientWidth;
            result.verticalPartiallyVisible = elementCorrectHeight - anchorCellLayoutInfo.rect.y + elementRect.height > gridContainer.clientHeight;
            result.fullInvisible = anchorCellLayoutInfo.rect.y > elementCorrectHeight || anchorCellLayoutInfo.rect.y + gridContainer.clientHeight < elementCorrectHeight ||
                                   anchorCellLayoutInfo.rect.x > elementCorrectWidth || anchorCellLayoutInfo.rect.x + gridContainer.clientWidth < elementCorrectWidth;

            return result;
        },
        getAdditionalCorrectSize: function(anchorTileInfo, cellTileInfo, isCol) {
            var startCellIndex = isCol ? anchorTileInfo.colIndex : anchorTileInfo.rowIndex,
                endCellIndex = isCol ? cellTileInfo.colIndex : cellTileInfo.rowIndex,
                correctSize = 0;
            for(var cellIndex = startCellIndex; cellIndex < endCellIndex; cellIndex++)
                correctSize += this.paneManager.getTileTotalSize(cellIndex, isCol);

            return correctSize
        }
    });

    ASPxClientSpreadsheet.ScrollHelper.Constants = {
        ScrollTopRowStep: 1,
        ScrollLeftRowStep: 1,
        AdditionalRowToView: 3,
        AdditionalColToView: 2
    };
})();