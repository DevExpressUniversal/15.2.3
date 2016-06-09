(function() {
    ASPxClientSpreadsheet.PaneView = ASPx.CreateClass(null, {
        constructor: function(paneManager, renderProvider, paneType) {
            this.paneManager = paneManager;
            this.renderProvider = renderProvider;
            this.paneType = paneType;
        },
        getPaneManager: function() {
            return this.paneManager;
        },
        getRenderProvider: function() {
            return this.renderProvider;
        },

        // PaneType interface
        getPaneType: function() {
            return this.paneType;
        },
        getTileHelper: function() {
            if(!this.tileHelper)
                this.tileHelper = new ASPxClientSpreadsheet.TileHelper(this);
            return this.tileHelper;
        },

        // DOM worker 
        getRowHeaderContainer: function() {
            return this.getRenderProvider().getRowHeaderContainer(this.getPaneType());
        },
        getGridContainer: function() {
            return this.getRenderProvider().getGridContainer(this.getPaneType());
        },
        getColumnHeaderContainer: function() {
            return this.getRenderProvider().getColumnHeaderContainer(this.getPaneType());
        },
        getColumnHeaderTilesContainer: function() {
            return this.getRenderProvider().getColumnHeaderTilesContainer(this.getPaneType());
        },
        getColumnHeader: function() {
            return this.getRenderProvider().getColumnHeader(this.getPaneType());
        },
        getRowHeader: function() {
            return this.getRenderProvider().getRowHeader(this.getPaneType());
        },
        getRowHeaderTilesContainer: function() {
            return this.getRenderProvider().getRowHeaderTilesContainer(this.getPaneType());
        },
        getGridTilesContainer: function() {
            return this.getRenderProvider().getGridTilesContainer(this.getPaneType());
        },
        getWorkbookControl: function() {
            return this.getRenderProvider().getWorkbookControl();
        },

        processDocumentResponse: function(newTiles, visibleRange, outdatedTiles) {
            this.getTileHelper().processDocumentResponse(newTiles, visibleRange, outdatedTiles);
        },

        renderAutoFilterImage: function(autoFilterConfig) {
            var cellContainer = this.getCellElementByModelPosition(autoFilterConfig.colIndex, autoFilterConfig.rowIndex);
            var image = document.createElement("div");

            if(!cellContainer)
                return;

            image.className = ASPx.SpreadsheetCssClasses.AutoFilterImage + " " + this.getAutoFilterImagesClassNames(autoFilterConfig.imageName);

            ASPx.Evt.AttachEventToElement(image, "click", function() {
                this.onAutoFilterClick(image, autoFilterConfig.columnType, !autoFilterConfig.isDefault);
            }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(image, "contextmenu", ASPx.Evt.PreventEventAndBubble);
            cellContainer.appendChild(image);
        },

        adjustRootControls: function() {
            this.getPaneManager().adjustVisiblePanes();
        },
        getVisibleRangePaddings: function() {
            return this.getRenderProvider().getVisibleRangePaddings();
        },
        getVisibleRange: function() {
            return this.getTileHelper().getVisibleRange();
        },
        getTileSize: function(isCol) {
            return this.getRenderProvider().getTileSize(isCol);
        },
        getTileSizes: function(index, isCol) {
            return this.getTileHelper().getTileSizes(index, isCol);
        },
        getTileTotalSize: function(index, isCol) {
            return this.getTileHelper().getTileTotalSize(index, isCol);
        },
        getTileIncrementalRanges: function(index, isCol) {
            return this.getTileHelper().getTileIncrementalRanges(index, isCol);
        },
        calculateViewBoundRange: function(cell, viewBounds, useOnlyCachedTiles, horzFarAlign, vertFarAlign) {
            return this.getTileHelper().calculateViewBoundRange(cell, viewBounds, useOnlyCachedTiles, horzFarAlign, vertFarAlign);
        },
        getGridTileInfo: function(rowIndex, colIndex) {
            return this.getTileHelper().getGridTileInfo(rowIndex, colIndex);
        },
        getDefaultCellSize: function() {
            return this.getRenderProvider().getDefaultCellSize();
        },
        getChildControlsPrefix: function() {
            return this.getRenderProvider().getChildControlsPrefix();
        },
        onVisibleTileRangeChanged: function() {
            this.getPaneManager().onVisibleTileRangeChanged();
        },
        getScrollHelper: function() {
            return this.getPaneManager().getScrollHelper();
        },
        loadInvisibleTiles: function() {
            return this.getPaneManager().loadInvisibleTiles();
        },
        loadInvisibleTilesForFullScreen: function() {
            this.getPaneManager().loadInvisibleTilesForFullScreen();
        },
        getCellElementByModelPosition: function(columnModelIndex, rowModelIndex) {
            return this.getPaneManager().getCellElementByModelPosition(columnModelIndex, rowModelIndex);
        },
        getAutoFilterImagesClassNames: function(imageName) {
            return this.getPaneManager().getAutoFilterImagesClassNames(imageName);
        },
        onAutoFilterClick: function(element, columnType, hasFilter) {
            this.getPaneManager().onAutoFilterClick(element, columnType, hasFilter);
        },
        updateGridTileCache: function(serverVisibleRange) {
            this.getTileHelper().updateGridTileCache(serverVisibleRange);
        },
        removeAllTiles: function() {
            this.getTileHelper().removeAllTiles();
        },
        getCellLayoutInfo: function(modelColumnIndex, modelRowIndex) {
            return this.getTileHelper().getCellLayoutInfo(modelColumnIndex, modelRowIndex);
        },
        getHeaderInfo: function(index, isCol) {
            return this.getTileHelper().getHeaderInfo(index, isCol);
        },
        getCellByCoord: function(xPosition, yPosition) {
            return this.getTileHelper().getCellByCoord(xPosition, yPosition);
        },
        getHeaderOffsetSize: function() {
            return this.getPaneManager().getHeaderOffsetSize();
        },
        getHeaderOffsetSizeForMousePointer: function() {
            return this.getPaneManager().getHeaderOffsetSizeForMousePointer(this.getPaneType());
        },
        convertVisibleIndexToModelIndex: function(visibleIndex, isCol) {
            return this.getTileHelper().convertVisibleIndexToModelIndex(visibleIndex, isCol);
        },
        highlightHeaders: function(selection) {
            this.getTileHelper().highlightHeaders(selection);
        },
        convertModelIndexToVisibleIndex: function(modelIndex, isCol) {
            return this.getTileHelper().convertModelIndexToVisibleIndex(modelIndex, isCol);
        },
        convertModelIndicesRangeToVisibleIndices: function(modelTopLeftIndex, modelBottomRightIndex, isCol) {
            return this.getTileHelper().convertModelIndicesRangeToVisibleIndices(modelTopLeftIndex, modelBottomRightIndex, isCol);
        },
        canScrollToShowCell: function(colVisibleIndex, rowVisibleIndex) {
            var scrollBounds = this.getTileHelper().getScrollBounds(),
                inHorizontalScrollBounds = scrollBounds.left <= colVisibleIndex && colVisibleIndex <= scrollBounds.right,
                inVerticalScrollBounds = scrollBounds.top <= rowVisibleIndex && rowVisibleIndex <= scrollBounds.bottom;

            return inHorizontalScrollBounds && inVerticalScrollBounds;
        },
        getCellEditingText: function(colModelIndex, rowModelIndex) {
            return this.getTileHelper().getCellEditingText(colModelIndex, rowModelIndex);
        },
        getCellValue: function(colModelIndex, rowModelIndex) {
            return this.getTileHelper().getCellValue(colModelIndex, rowModelIndex);
        },
        isMergerCellContainsInCroppedRange: function(cellModelPosition) {
            return this.getTileHelper().isMergerCellContainsInCroppedRange(cellModelPosition);
        },
        getLeftTopModelCellPositionInCroppedRange: function(cellModelPosition) {
            return this.getTileHelper().getLeftTopModelCellPositionInCroppedRange(cellModelPosition);
        },
        getCellLayoutInfo_ByModelIndices: function(colModelIndex, rowModelIndex) {
            return this.getTileHelper().getCellLayoutInfo_ByModelIndices(colModelIndex, rowModelIndex);
        },
        serializeCachedTiles: function() {
            return this.getTileHelper().serializeCachedTiles();
        },
        loadTilesSoftly: function(visibleRange) {
            var tileHelper = this.getTileHelper();
            tileHelper.loadGridTiles(visibleRange);
            tileHelper.afterGridTilesLoaded(visibleRange);
            tileHelper.setVisibleRange(visibleRange);
        },
        getParentTileInfo: function(selection) {
            return this.getTileHelper().getParentTileInfo(selection);
        },
        convertModelRangeToVisible: function(range) {
            return this.getTileHelper().convertModelRangeToVisible(range);
        },
        getVisibleModelCellRange: function() {
            return this.getTileHelper().getVisibleModelCellRange();
        },
        isSelectionVisibleInPane: function(selection) {
            return this.getTileHelper().isSelectionVisible(selection);
        },
        getCellVisibleRange: function() {
            return this.getTileHelper().getCellVisibleRange();
        },
        putCellColIndexInVisibleRange: function(colIndex) {
            return this.getTileHelper().putCellColIndexInVisibleRange(colIndex);
        },
        putCellRowIndexInVisibleRange: function(rowIndex) {
            return this.getTileHelper().putCellRowIndexInVisibleRange(rowIndex);
        },
        getMergedCellRangesIntersectsRange: function(range) {
            return this.getTileHelper().getMergedCellRangesIntersectsRange(range);
        },
        getActiveCellParentTileInfo: function(selection) {
            return this.getTileHelper().getActiveCellParentTileInfo(selection);
        },
        getVisibleVisibleCellRange: function() {
            return this.getTileHelper().getVisibleVisibleCellRange();
        },
        getLastVisibleHeaderRowTileIndex: function() {
            return this.getTileHelper().getLastVisibleHeaderRowTileIndex();
        },
        setLastVisibleHeaderRowTileIndex: function(tileIndex) {
            return this.getTileHelper().setLastVisibleHeaderRowTileIndex(tileIndex);
        },
        searchCellInVisibleRange: function(cellModelPosition) {
            return this.getTileHelper().cellInVisibleRange(cellModelPosition);
        },
        getPaneTopLeftCellVisiblePosition: function() {
            return this.getPaneManager().getPaneTopLeftCellVisiblePosition(this.getPaneType());
        },
        getCellParentTileInfo: function(cell) {
            return this.getTileHelper().getCellParentTileInfo(cell);
        },

        // Real pane cell visible range
        setPaneCellVisibleRange: function(range){
            this.cellVisibleRange = range;
        },
        getPaneCellVisibleRange: function() {
            return this.cellVisibleRange;
        },
        isCellInPaneVisibleRange: function(cell) {
            return this.cellVisibleRange && this.cellVisibleRange.isCellInRange(cell.col, cell.row);
        },

        // Drawing selection elements

        drawPartSelection: function(selection, visibleBorders) {
            if(this.canDisplaySelection(selection)) {
                var topLeftCellLayoutInfoBase = this.getCellLayoutInfo(selection.range.leftColIndex, selection.range.topRowIndex),
                    bottomRightCellLayoutInfoBase = this.getCellLayoutInfo(selection.range.rightColIndex, selection.range.bottomRowIndex);
                if(topLeftCellLayoutInfoBase && bottomRightCellLayoutInfoBase) {
                    var rect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfoBase, bottomRightCellLayoutInfoBase, topLeftCellLayoutInfoBase.tileInfo);
                    var parentTileInfo = this.getCellParentTileInfo({ col: selection.range.leftColIndex, row: selection.range.topRowIndex });

                    this.correctSelectionRect(rect);

                    var leftBottomSelectionElement = this.getDrawingHelper().getLeftBottomSelectionElement();
                    ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(leftBottomSelectionElement, parentTileInfo.htmlElement);
                    ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(leftBottomSelectionElement, rect);
                    ASPx.SetElementDisplay(leftBottomSelectionElement, true);

                    if(visibleBorders.top) {
                        var topBorderElement = this.getDrawingHelper().getTopBorderElement();
                        ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(topBorderElement, parentTileInfo.htmlElement);
                        var borderTopRect = { x: rect.x, y: rect.y, width: rect.width, height: 0 };
                        ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(topBorderElement, borderTopRect);
                        ASPx.SetElementDisplay(topBorderElement, true);
                    }
                    if(visibleBorders.bottom) {
                        var bottomBorderElement = this.getDrawingHelper().getBottomBorderElement();
                        ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(bottomBorderElement, parentTileInfo.htmlElement);
                        var borderBottomRect = { x: rect.x, y: rect.y + rect.height, width: rect.width, height: 0 };
                        ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(bottomBorderElement, borderBottomRect);
                        ASPx.SetElementDisplay(bottomBorderElement, true);
                    }
                    if(visibleBorders.left) {
                        var leftBorderElement = this.getDrawingHelper().getLeftBorderElement();
                        ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(leftBorderElement, parentTileInfo.htmlElement);
                        var borderLeftRect = { x: rect.x, y: rect.y, width: 0, height: rect.height };
                        ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(leftBorderElement, borderLeftRect);
                        ASPx.SetElementDisplay(leftBorderElement, true);
                    }
                    if(visibleBorders.right) {
                        var rightBorderElement = this.getDrawingHelper().getRightBorderElement();
                        ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(rightBorderElement, parentTileInfo.htmlElement);
                        var borderRightRect = { x: rect.x + rect.width, y: rect.y, width: 0, height: rect.height + ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor };
                        ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(rightBorderElement, borderRightRect);
                        ASPx.SetElementDisplay(rightBorderElement, true);
                    }
                    if(ASPx.Browser.TouchUI)
                        this.setTouchSelectionElementsPosition(selection, parentTileInfo, borderTopRect, borderRightRect, borderBottomRect, borderLeftRect);
                }
            }
        },

        drawSelection: function(selection, keepOldSelection) {
            if(this.canDisplaySelection(selection)) {
                var activeCellRect = this.getActiveCellRect(selection),
                    selectionElementsRects = null;
                if(!selection.range.isSingleCell())
                    selectionElementsRects = this.getSelectionElementsRect(selection);
                this.drawSelectionBase(selection, activeCellRect, selectionElementsRects, keepOldSelection);
            }
        },
        getActiveCellRect: function(selection) {
            var activeCellRect = null;
            if(selection.isActiveCellMerged()) {
                var topLeftCellRect = this.getCellLayoutInfo(selection.activeCellRange.leftColIndex, selection.activeCellRange.topRowIndex).rect,
                    bottomRightCellRect = this.getCellLayoutInfo(selection.activeCellRange.rightColIndex, selection.activeCellRange.bottomRowIndex).rect;

                if(topLeftCellRect && bottomRightCellRect)
                    activeCellRect = ASPxClientSpreadsheet.RectHelper.mergeRects(topLeftCellRect, bottomRightCellRect);
            } else {
                activeCellRect = this.getCellLayoutInfo(selection.activeCellColIndex, selection.activeCellRowIndex).rect;
            }
            return activeCellRect;
        },
        getSelectionElementsRect: function(selection) {
            var selectionElementsRects = {
                leftTop: null,
                leftBottom: null,
                rightTop: null,
                rightBottom: null,
                selection: null
            };

            var topLeftCellLayoutInfoBase = this.getCellLayoutInfo(selection.range.leftColIndex, selection.range.topRowIndex),
                bottomRightCellLayoutInfoBase = this.getCellLayoutInfo(selection.range.rightColIndex, selection.range.bottomRowIndex);

            var rowIndex = selection.activeCellRange.topRowIndex > 0 ? this.putCellRowIndexInVisibleRange(selection.activeCellRange.topRowIndex - 1) : 0,
                topLeftCellLayoutInfo = this.getCellLayoutInfo(selection.range.leftColIndex, selection.range.topRowIndex),
                bottomRightCellLayoutInfo = this.getCellLayoutInfo(selection.activeCellRange.rightColIndex, rowIndex);
            selectionElementsRects.leftTop = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfo, bottomRightCellLayoutInfo, topLeftCellLayoutInfoBase.tileInfo);

            var colIndex = selection.activeCellRange.leftColIndex > 0 ? this.putCellColIndexInVisibleRange(selection.activeCellRange.leftColIndex - 1) : 0;
            topLeftCellLayoutInfo = this.getCellLayoutInfo(selection.range.leftColIndex, selection.activeCellRange.topRowIndex);
            bottomRightCellLayoutInfo = this.getCellLayoutInfo(colIndex, selection.range.bottomRowIndex);
            selectionElementsRects.leftBottom = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfo, bottomRightCellLayoutInfo, topLeftCellLayoutInfoBase.tileInfo);

            topLeftCellLayoutInfo = this.getCellLayoutInfo(this.putCellColIndexInVisibleRange(selection.activeCellRange.rightColIndex + 1), selection.range.topRowIndex);
            bottomRightCellLayoutInfo = this.getCellLayoutInfo(selection.range.rightColIndex, selection.activeCellRange.bottomRowIndex);
            selectionElementsRects.rightTop = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfo, bottomRightCellLayoutInfo, topLeftCellLayoutInfoBase.tileInfo);

            topLeftCellLayoutInfo = this.getCellLayoutInfo(selection.activeCellRange.leftColIndex, this.putCellRowIndexInVisibleRange(selection.activeCellRange.bottomRowIndex + 1));
            bottomRightCellLayoutInfo = this.getCellLayoutInfo(selection.range.rightColIndex, selection.range.bottomRowIndex);
            selectionElementsRects.rightBottom = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfo, bottomRightCellLayoutInfo, topLeftCellLayoutInfoBase.tileInfo);

            selectionElementsRects.selection = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfoBase, bottomRightCellLayoutInfoBase, topLeftCellLayoutInfoBase.tileInfo);

            return selectionElementsRects;
        },
        correctSelectionRect: function(rect) {
            rect.x -= 1;
            rect.y -= 1;
            rect.width += 1;
            rect.height += 1;
        },
        drawSelectionBase: function(selection, activeCellRect, selectionElementsRects, keepOldSelection) {
           if(activeCellRect) {
               var activeCellParentTileInfo = this.getCellParentTileInfo({ col: selection.activeCellColIndex, row: selection.activeCellRowIndex }),
                   activeCellSelectedElement = this.getDrawingHelper().getActiveSelectionElement();

                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(activeCellSelectedElement, activeCellParentTileInfo.htmlElement);                
                ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(activeCellSelectedElement, activeCellRect);
                ASPx.SetElementDisplay(activeCellSelectedElement, true);
            }

           if(!keepOldSelection)
               this.drawSelectionRect(selection, selectionElementsRects, activeCellRect);
        },
        drawSelectionRect: function(selection, selectionRectArray, activeCellRect) {
            var parentTileInfo = this.getCellParentTileInfo({ col: selection.range.leftColIndex, row: selection.range.topRowIndex });
            if(selectionRectArray && selectionRectArray.leftTop) {
                if(this.isSelectionElementVisible(selection.activeCellRange.topRowIndex, selection.range.topRowIndex, "top"))
                    selectionRectArray.leftTop.height = 0;

                var leftTopSelectionElement = this.getDrawingHelper().getLeftTopSelectionElement();
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(leftTopSelectionElement, parentTileInfo.htmlElement);
                ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(leftTopSelectionElement, selectionRectArray.leftTop);
                ASPx.SetElementDisplay(leftTopSelectionElement, true);
            }
            if(selectionRectArray && selectionRectArray.leftBottom) {
                if(this.isSelectionElementVisible(selection.activeCellRange.leftColIndex, selection.range.leftColIndex, "left"))
                    selectionRectArray.leftBottom.width = 0;

                var leftBottomSelectionElement = this.getDrawingHelper().getLeftBottomSelectionElement();
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(leftBottomSelectionElement, parentTileInfo.htmlElement);
                ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(leftBottomSelectionElement, selectionRectArray.leftBottom);
                ASPx.SetElementDisplay(leftBottomSelectionElement, true);
            }
            if(selectionRectArray && selectionRectArray.rightTop) {
                var rightTopSelectionElement = this.getDrawingHelper().getRightTopSelectionElement();
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(rightTopSelectionElement, parentTileInfo.htmlElement);
                ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(rightTopSelectionElement, selectionRectArray.rightTop);
                ASPx.SetElementDisplay(rightTopSelectionElement, true);
            }
            if(selectionRectArray && selectionRectArray.rightBottom) {
                var rightBottomSelectionElement = this.getDrawingHelper().getRightBottomSelectionElement();
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(rightBottomSelectionElement, parentTileInfo.htmlElement);
                ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(rightBottomSelectionElement, selectionRectArray.rightBottom);
                ASPx.SetElementDisplay(rightBottomSelectionElement, true);
            }
            
            var topBorderElement = this.getDrawingHelper().getTopBorderElement();
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(topBorderElement, parentTileInfo.htmlElement);
            var borderTopRect = this.getDrawingHelper().getTopBorderRect(activeCellRect, selectionRectArray, selection.range.isSingleCell());
            ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(topBorderElement, borderTopRect);
            ASPx.SetElementDisplay(topBorderElement, true);
            
            var rightBorderElement = this.getDrawingHelper().getRightBorderElement();
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(rightBorderElement, parentTileInfo.htmlElement);
            var borderRightRect = this.getDrawingHelper().getRightBorderRect(activeCellRect, selectionRectArray, selection.range.isSingleCell());
            ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(rightBorderElement, borderRightRect);
            ASPx.SetElementDisplay(rightBorderElement, true);

            var bottomBorderElement = this.getDrawingHelper().getBottomBorderElement();
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(bottomBorderElement, parentTileInfo.htmlElement);
            var borderBottomRect = this.getDrawingHelper().getBottomBorderRect(activeCellRect, selectionRectArray, selection.range.isSingleCell());
            ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(bottomBorderElement, borderBottomRect);
            ASPx.SetElementDisplay(bottomBorderElement, true);

            var leftBorderElement = this.getDrawingHelper().getLeftBorderElement();
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(leftBorderElement, parentTileInfo.htmlElement);
            var borderLeftRect = this.getDrawingHelper().getLeftBorderRect(activeCellRect, selectionRectArray, selection.range.isSingleCell());
            ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(leftBorderElement, borderLeftRect);
            ASPx.SetElementDisplay(leftBorderElement, true);

            if(ASPx.Browser.TouchUI)
                this.setTouchSelectionElementsPosition(selection, parentTileInfo, borderTopRect, borderRightRect, borderBottomRect, borderLeftRect);
        },

        setTouchSelectionElementsPosition: function(selection, parentTileInfo, borderTopRect, borderRightRect, borderBottomRect, borderLeftRect) {
            var leftTop = {},
                rightBottom = {};
            leftTop.x = borderTopRect ? borderTopRect.x : 
                (borderLeftRect ? borderLeftRect.x : borderBottomRect.x);
            leftTop.y = borderTopRect ? borderTopRect.y :
                (borderLeftRect ? borderLeftRect.y : borderRightRect.y);
            rightBottom.x = borderRightRect ? borderRightRect.x :
                (borderBottomRect ? borderBottomRect.x + borderBottomRect.width :
                (borderTopRect.x + borderTopRect.width));
            rightBottom.y = borderBottomRect ? borderBottomRect.y :
                (borderRightRect ? borderRightRect.y + borderRightRect.height :
                (borderLeftRect.y + borderLeftRect.height));
            var elements = this.getDrawingHelper().getTouchSelectionElements();
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(elements.leftTop, parentTileInfo.htmlElement);
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(elements.rightBottom, parentTileInfo.htmlElement);
            var coordinates;
            if(selection.entireColsSelected) {
                var y = this.getDrawingHelper().prepareEntireColsRowsSelectionElementPos(true);
                coordinates = this.getDrawingHelper().getTouchSelectionElementCoordinates(leftTop.x, y, rightBottom.x, y);
            } else if(selection.entireRowsSelected) {
                var x = this.getDrawingHelper().prepareEntireColsRowsSelectionElementPos(false);
                coordinates = this.getDrawingHelper().getTouchSelectionElementCoordinates(x, leftTop.y, x, rightBottom.y);
            } else {
                coordinates = this.getDrawingHelper().getTouchSelectionElementCoordinates(leftTop.x, leftTop.y, rightBottom.x, rightBottom.y);
            }
            elements.leftTop.style.left = coordinates.leftTopX;
            elements.leftTop.style.top = coordinates.leftTopY;
            elements.rightBottom.style.left = coordinates.rightBottomX;
            elements.rightBottom.style.top = coordinates.rightBottomY;
        },
        moveTouchSelectionElementsOnScroll: function(selection) {
            this.getDrawingHelper().moveTouchSelectionElementsOnScroll(selection);
        },
        hideTouchSelectionElements: function(display) {
            this.getDrawingHelper().hideTouchSelectionElements(display);
        },
        getMovementBoundaryCells: function(selection, cellLayoutInfo, referenceCell, firstTime) {
            var horzOffset = firstTime || selection.entireRowsSelected ? 0 : cellLayoutInfo.colIndex - referenceCell.colIndex,
                vertOffset = firstTime || selection.entireColsSelected ? 0 : cellLayoutInfo.rowIndex - referenceCell.rowIndex,
                leftColIndex = selection.range.leftColIndex + horzOffset,
                topRowIndex =  selection.range.topRowIndex + vertOffset;

            leftColIndex = leftColIndex < 0 ? 0 : leftColIndex;
            topRowIndex = topRowIndex < 0 ? 0 : topRowIndex;

            var rightColIndex = leftColIndex + selection.range.rightColIndex - selection.range.leftColIndex,
                bottomRowIndex = topRowIndex + selection.range.bottomRowIndex - selection.range.topRowIndex,
                topLeftCellLayoutInfoBase = this.getCellLayoutInfo(leftColIndex, topRowIndex),
                bottomRightCellLayoutInfo = this.getCellLayoutInfo(rightColIndex, bottomRowIndex);
            return { leftTop: topLeftCellLayoutInfoBase, rightBottom: bottomRightCellLayoutInfo };
        },
        drawSelectionMovementRect: function(selection, cellLayoutInfo) {
            var firstTime = false;
            if(!this.selectionMovementReferenceCell) {
                this.selectionMovementReferenceCell = {
                    rowIndex: this.getNearestNeighborCellIndexInsideSelection(selection, cellLayoutInfo.rowIndex, false),
                    colIndex: this.getNearestNeighborCellIndexInsideSelection(selection, cellLayoutInfo.colIndex, true)
                };
                firstTime = true;
            }
            var selectionMovementBorderElement = this.getDrawingHelper().getSelectionMovementBorderElement();
            var boundaryCells = this.getMovementBoundaryCells(selection, cellLayoutInfo, this.selectionMovementReferenceCell, firstTime);
            if(!boundaryCells.leftTop || !boundaryCells.rightBottom) {
                ASPx.SetElementDisplay(selectionMovementBorderElement, false);
                return;
            }

            var info = this.getSelectionMovementInfo(boundaryCells),
                leftTop = info.rectLayoutInfo.leftTop,
                parentTilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), leftTop.colIndex, leftTop.rowIndex),
                parentTileInfo = this.getGridTileInfo(parentTilePosition.row, parentTilePosition.col);

            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(selectionMovementBorderElement, parentTileInfo.htmlElement);
            this.selectionMovementLayoutInfo = info.rectLayoutInfo;
            ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(selectionMovementBorderElement, info.rect);
            ASPx.SetElementDisplay(selectionMovementBorderElement, true);
        },
        getNearestNeighborCellIndexInsideSelection: function(selection, cellIndex, isCol) {
            var index = cellIndex;
            if(!selection.range.isCellInRangeCore(cellIndex, isCol))
                index = selection.range.isCellInRangeCore(cellIndex + 1, isCol) ? cellIndex + 1 : cellIndex - 1;
            return index;
        },
        getSelectionMovementInfo: function(boundaryCells) {
            var rect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(boundaryCells.leftTop, boundaryCells.rightBottom, boundaryCells.leftTop.tileInfo);
            rect.height += ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor;    
            rect.width += ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor;
                
            return {
                rect: rect,
                rectLayoutInfo: {
                    leftTop: boundaryCells.leftTop,
                    rightBottom: boundaryCells.rightBottom
                }
            };
        },
        hideSelectionMovementElement: function() {
            this.selectionMovementReferenceCell = null;
            this.selectionMovementLayoutInfo = null;
            this.getDrawingHelper().hideSelectionMovementElement();
        },
        getSelectionMovementLayoutInfo: function() {
            return this.selectionMovementLayoutInfo;
        },
        isRightBottomTouchSelectionElement: function(element) {
            var touchElements = this.getDrawingHelper().getTouchSelectionElements();
            return touchElements.rightBottom == element;
        },

        isSelectionElementVisible: function(startInitialPosition, endInitialPosition, position) {
            var visibleCellRange = this.getVisibleVisibleCellRange();
            return startInitialPosition === visibleCellRange[position] && startInitialPosition === endInitialPosition;
        },
        canDisplaySelection: function(selection) {
            var parentTileInfo = this.getCellParentTileInfo({ col: selection.range.leftColIndex, row: selection.range.topRowIndex });
            return parentTileInfo && ASPx.IsValidElement(parentTileInfo.htmlElement);
        },
        hideSelection: function() {
            this.getDrawingHelper().hideSelection();
        },

        getDrawingHelper: function() {
            if(!this.drawingHelper)
                this.drawingHelper = new ASPxClientSpreadsheet.PaneView.DrawingHelper(this);
            return this.drawingHelper;
        }
    });

    ASPxClientSpreadsheet.PaneView.DrawingHelper = function(pane) {
        var pane = pane;

        // Public members
        this.hideSelection = function() {
            ASPx.SetElementDisplay(this.getActiveSelectionElement(), false);
            ASPx.SetElementDisplay(this.getRightBottomSelectionElement(), false);
            ASPx.SetElementDisplay(this.getRightTopSelectionElement(), false);
            ASPx.SetElementDisplay(this.getLeftBottomSelectionElement(), false);
            ASPx.SetElementDisplay(this.getLeftTopSelectionElement(), false);

            ASPx.SetElementDisplay(this.getBottomBorderElement(), false);
            ASPx.SetElementDisplay(this.getLeftBorderElement(), false);
            ASPx.SetElementDisplay(this.getTopBorderElement(), false);
            ASPx.SetElementDisplay(this.getRightBorderElement(), false);
        };
        this.getActiveSelectionElement = function() {
            if(!pane.selectElement)
                pane.selectElement = createActiveSelectionElement();
            return pane.selectElement;
        };
        this.getRightBottomSelectionElement = function() {
            if(!pane.rightBottomSelectionElement)
                pane.rightBottomSelectionElement = createRightBottomSelectionElement();
            return pane.rightBottomSelectionElement;
        };
        this.getRightTopSelectionElement = function() {
            if(!pane.rightTopSelectionElement)
                pane.rightTopSelectionElement = createRightTopSelectionElement();
            return pane.rightTopSelectionElement;
        };
        this.getLeftBottomSelectionElement = function() {
            if(!pane.leftBottomSelectionElement)
                pane.leftBottomSelectionElement = createLeftBottomSelectionElement();
            return pane.leftBottomSelectionElement;
        };
        this.getLeftTopSelectionElement = function() {
            if(!pane.leftTopSelectionElement)
                pane.leftTopSelectionElement = createLeftTopSelectionElement();
            return pane.leftTopSelectionElement;
        };
        this.getBottomBorderElement = function() {
            if(!pane.bottomBorderElement)
                pane.bottomBorderElement = createBottomBorderElement();
            return pane.bottomBorderElement;
        };
        this.getLeftBorderElement = function() {
            if(!pane.leftBorderElement)
                pane.leftBorderElement = createLeftBorderElement();
            return pane.leftBorderElement;
        };
        this.getTopBorderElement = function() {
            if(!pane.topBorderElement)
                pane.topBorderElement = createTopBorderElement();
            return pane.topBorderElement;
        };
        this.getRightBorderElement = function() {
            if(!pane.rightBorderElement)
                pane.rightBorderElement = createRightBorderElement();
            return pane.rightBorderElement;
        };
        this.getTouchSelectionElements = function() {
            if(!pane.touchSelectionElemnts)
                pane.touchSelectionElemnts = createTouchSelectionElements();
            return pane.touchSelectionElemnts;
        };
        this.getSelectionMovementBorderElement = function() {
            if(!pane.selectionMovementBorderElement)
                pane.selectionMovementBorderElement = createSelectionMovementBorderElement();
            return pane.selectionMovementBorderElement;
        };

        // Border Rect
        this.getTopBorderRect = function(activeCellRect, selectionRectArray, isSingleCell) {
            if(isSingleCell)
                return {
                    x: activeCellRect.x - 1,
                    y: activeCellRect.y - 1,
                    width: activeCellRect.width + 1,
                    height: 1
                };
            else return {
                x: selectionRectArray.leftTop.x - 1,
                y: selectionRectArray.leftTop.y - 1,
                width: selectionRectArray.leftTop.width + selectionRectArray.rightTop.width + 1,
                height: 1
            };
        };
        this.getRightBorderRect = function(activeCellRect, selectionRectArray, isSingleCell) {
            if(isSingleCell)
                return {
                    x: activeCellRect.x + activeCellRect.width - 1,
                    y: activeCellRect.y - 1,
                    width: 1,
                    height: activeCellRect.height + ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor + 1
                };
            else return {
                x: selectionRectArray.rightTop.x + selectionRectArray.rightTop.width - 1,
                y: selectionRectArray.rightTop.y - 1,
                width: 1,
                height: selectionRectArray.rightTop.height + selectionRectArray.rightBottom.height + ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor + 1
            };
        };
        this.getBottomBorderRect = function(activeCellRect, selectionRectArray, isSingleCell) {
            if(isSingleCell)
                return {
                    x: activeCellRect.x - 1,
                    y: activeCellRect.y + activeCellRect.height - 1,
                    width: activeCellRect.width + 1,
                    height: 1
                };
            else return {
                x: selectionRectArray.leftTop.x - 1,
                y: selectionRectArray.rightBottom.y + selectionRectArray.rightBottom.height - 1,
                width: selectionRectArray.leftTop.width + selectionRectArray.rightTop.width+ 1,
                height: 1
            };
        };
        this.getLeftBorderRect = function(activeCellRect, selectionRectArray, isSingleCell) {
            if(isSingleCell)
                return {
                    x: activeCellRect.x - 1,
                    y: activeCellRect.y - 1,
                    width: 1,
                    height: activeCellRect.height + 1
                };
            else return {
                x: selectionRectArray.leftTop.x - 1,
                y: selectionRectArray.rightTop.y - 1,
                width: 1,
                height: selectionRectArray.rightTop.height + selectionRectArray.rightBottom.height + 1
            };
        };

        // Touch Selection Radius
        this.getTouchSelectionElementCoordinates = function(leftTopX, leftTopY, rightBottomX, rightBottomY) {
            var offset = getTouchSelectionElementsRadius(this.getTouchSelectionElements());
            return {
                leftTopX: leftTopX - offset + "px",
                leftTopY: leftTopY - offset + "px",
                rightBottomX: rightBottomX - offset + "px",
                rightBottomY: rightBottomY - offset + "px"
            }
        };
        this.prepareEntireColsRowsSelectionElementPos = function(isColSelection) {
            var workbook = pane.getWorkbookControl();
            if(isColSelection) {
                var rightBorder = pane.getDrawingHelper().getRightBorderElement();
                var visibleBorder = ASPx.GetElementDisplay(rightBorder) ? rightBorder : pane.getDrawingHelper().getLeftBorderElement();
                return ASPx.PrepareClientPosForElement(ASPx.GetAbsoluteY(workbook) + workbook.offsetHeight / 2, visibleBorder, false);
            } else {
                var bottomBorder = pane.getDrawingHelper().getBottomBorderElement();
                var visibleBorder = ASPx.GetElementDisplay(bottomBorder) ? bottomBorder : pane.getDrawingHelper().getTopBorderElement();
                return ASPx.PrepareClientPosForElement(ASPx.GetAbsoluteX(workbook) + workbook.offsetWidth / 2, visibleBorder, true);
            }
        };
        this.moveTouchSelectionElementsOnScroll = function(selection) {
            var elements = this.getTouchSelectionElements();
            var pos = this.prepareEntireColsRowsSelectionElementPos(selection.entireColsSelected) - getTouchSelectionElementsRadius(this.getTouchSelectionElements()) + "px";
            if(selection.entireColsSelected) {
                elements.leftTop.style.top = pos;
                elements.rightBottom.style.top = pos;
            } else if(selection.entireRowsSelected) {
                elements.leftTop.style.left = pos;
                elements.rightBottom.style.left = pos;
            }
        };
        this.hideTouchSelectionElements = function(display) {
            var elements = this.getTouchSelectionElements();
            ASPx.SetElementDisplay(elements.leftTop, display);
            ASPx.SetElementDisplay(elements.rightBottom, display);
        };

        this.hideSelectionMovementElement = function() {
            if(pane.selectionMovementBorderElement)
                ASPx.SetElementDisplay(this.getSelectionMovementBorderElement(), false);
        };

        // Internal members
        function createActiveSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.CellActiveSelectedElement;
            return element;
        }

        function createLeftBottomSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.CellRangeSelectedElement;
            return element;
        }

        function createRightTopSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.CellRangeSelectedElement;
            return element;
        }

        function createRightBottomSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.CellRangeSelectedElement;
            return element;
        }

        function createLeftTopSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.CellRangeSelectedElement;
            return element;
        }

        function createLeftBorderElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.LeftRangeBorderElement;
            return element;
        }

        function createBottomBorderElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.BottomRangeBorderElement;
            return element;
        }

        function createTopBorderElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.TopRangeBorderElement;
            return element;
        }

        function createRightBorderElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.RightRangeBorderElement;
            return element;
        }

        function createTouchSelectionElements() {
            var elements = {};
            elements.leftTop = createTouchSelectionElement();
            elements.rightBottom = createTouchSelectionElement();
            return elements;
        }

        function createTouchSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.TouchSelectionElement;
            return element;
        }

        function getTouchSelectionElementsRadius(elements) {
            if(!elements.radius) {
                var style = window.getComputedStyle(elements.leftTop);
                elements.radius = Math.ceil(ASPx.PxToInt(style.getPropertyValue('width')) / 2);
            }
            return elements.radius;
        }

        function createSelectionMovementBorderElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.SelectionMovementBorderElement;
            return element;
        }
    };

    ASPxClientSpreadsheet.CellLayoutHelper = (function() { 
        function setElementPosAsElement(srcElement, destElement) {
            srcElement.style.left = destElement.style.left;
            srcElement.style.top = destElement.style.top;
            srcElement.style.width = destElement.style.width;
            srcElement.style.height = destElement.style.height;
        }
        function setElementRect(element, rect) {
            ASPxClientSpreadsheet.RectHelper.setElementRect(element, rect);
        }        
        function prepareCellRectToOutterRect(rect, shift, borderSize) {
            borderSize = borderSize - shift;
            rect.x -= shift;
            rect.y -= shift;
            rect.width -= borderSize;
            rect.height -= borderSize;
            return rect;
        }

        return {
            setElementPosAsElement: setElementPosAsElement,
            setElementRect: setElementRect,
            prepareCellRectToOutterRect: prepareCellRectToOutterRect
        };
    })();
})();