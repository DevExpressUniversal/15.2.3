(function() {
    ASPxClientSpreadsheet.TileHelper = ASPx.CreateClass(null, {
        constructor: function(parentPane) {
            this.parentPane = parentPane;
            this.tileSize = this.parentPane.getTileSize();
            this.visibleRangePadding = this.parentPane.getVisibleRangePaddings();
            this.clientVisibleRangePadding = {
                row: Math.floor(this.visibleRangePadding.row / 3),
                col: Math.floor(this.visibleRangePadding.col / 3)
            };
            this.tileMatrix = null;
            this.etalonGridTileRow = null;
            this.etalonGridTileRowHtml = "<div class='dxss-fc'></div>";

            this.visibleRangesWaitingForLoad = [];

            this.visibleRange = { top: -1, right: -1, bottom: -1, left: -1 }; // Tile indices
            this.scrollBounds = { top: 0, right: 0, bottom: 0, left: 0 }; // Cell indices

            this.headerVisibleRange = { top: 0, right: 0, bottom: 0, left: 0 }; // Tile indices
            this.headerScrollBounds = { top: 0, right: 0, bottom: 0, left: 0 }; // Cell indices

            this.headerOffsetHeight = 0;
            this.headerOffsetWidths = [];
            this.lastVisibleHeaderRowTileIndex = 0;

            this.maxRowCount = 1000000; // todo get it from document
            this.maxColCount = 1000000;
            this.highlightHeadersArray = [];

            this.croppedComplexBoxes = [];
            this.thereAreHiddenElementsBetweenTiles = false;
        },
        getDefaultCellSize: function() { return this.parentPane.getDefaultCellSize(); },
        getPaneType: function() { return this.parentPane.getPaneType(); },
        getRenderProvider: function() { return this.parentPane.getRenderProvider(); },
        getGridContainer: function() { return this.parentPane.getGridContainer(); },
        getGridTilesContainer: function() { return this.parentPane.getGridTilesContainer(); },
        getColumnHeaderTilesContainer: function() { return this.parentPane.getColumnHeaderTilesContainer(); },
        getRowHeaderTilesContainer: function() { return this.parentPane.getRowHeaderTilesContainer(); },
        getPaneTopLeftCellVisiblePosition: function() {
            return this.parentPane.getPaneTopLeftCellVisiblePosition();
        },
                
        getScrollHelper: function() { return this.parentPane.getScrollHelper(); },
        getScrollAnchorVisiblePosition: function() { return this.getScrollHelper().getScrollAnchorVisiblePosition(); },
        getLastVisibleHeaderRowTileIndex: function() { return this.lastVisibleHeaderRowTileIndex; },
        setLastVisibleHeaderRowTileIndex: function(tileIndex) { this.lastVisibleHeaderRowTileIndex = tileIndex; },
        getTileMatrix: function() {
            if(!this.tileMatrix)
                this.tileMatrix = new ASPxClientSpreadsheet.TileMatrix();
            return this.tileMatrix;
        },

        // Receive data from server side
        processDocumentResponse: function(newTiles, visibleRange, outdatedTiles/*callbackResult*/) {
            ASPxClientSpreadsheet.Log.BeginUpdate();
            ASPxClientSpreadsheet.Log.Write("processCallback:");

            this.removeOutdatedTiles(outdatedTiles);
            this.insertNewTiles(newTiles, visibleRange);

            this.setVisibleRange(visibleRange);
            this.afterGridTilesLoaded(visibleRange);

            var size = this.getVisiblePartSize();
            ASPx.SetStyles(this.getGridTilesContainer(), { width: size.width, height: size.height });

            ASPxClientSpreadsheet.Log.EndUpdate();
        },

        adjustControls: function() {
            // TODO set row headers width, col headers height and remove it from assignHeaderContentSizes function
            this.parentPane.adjustRootControls();
        },
        loadInvisibleTiles: function() { this.parentPane.loadInvisibleTiles(); },

        getVisibleRange: function() { return this.visibleRange; },
        setVisibleRange: function(range) {
            var changed = this.visibleRange.top !== range.top || this.visibleRange.right !== range.right || this.visibleRange.bottom !== range.bottom || this.visibleRange.left !== range.left;
            if(changed) {
                this.visibleRange = range;
                this.onVisibleRangeChanged();
            }
        },

        getVisibleModelCellRange: function() {
            var visibleTileRange = this.getVisibleRange();
            var top, right, bottom, left;

            top = this.getHeaderInfo(visibleTileRange.top, false).modelIndices[0];
            left = this.getHeaderInfo(visibleTileRange.left, true).modelIndices[0];

            bottom = this.getHeaderInfo(visibleTileRange.bottom, false).modelIndices[this.tileSize.col - 1];
            right = this.getHeaderInfo(visibleTileRange.right, true).modelIndices[this.tileSize.row - 1];

            return { top: top, right: right, bottom: bottom, left: left };
        },
        getVisibleVisibleCellRange: function() {
            var visibleTileRange = this.getVisibleRange();
            var top = 0, right = 0, bottom = 0, left = 0;

            if(visibleTileRange.top > 0)
                top = visibleTileRange.top * this.tileSize.row;
            if(visibleTileRange.left > 0)
                left = visibleTileRange.left * this.tileSize.col;

            bottom = (visibleTileRange.bottom + 1) * this.tileSize.row - 1;
            right = (visibleTileRange.right + 1) * this.tileSize.col - 1;

            return { top: top, right: right, bottom: bottom, left: left };
        },

        onVisibleRangeChanged: function() {
            var visibleRange = this.getVisibleRange();
            this.removeInvisibleTiles(visibleRange);
            this.scrollBounds = this.calculateScrollBounds(visibleRange);
            this.invalidateVisibleRangesWaitingForLoad();

            this.parentPane.onVisibleTileRangeChanged();
        },

        serializeCachedTiles: function() {
            var result = [];
            var cachedTileInfoArray = this.getTileMatrix().getGridTileInfoArray();
            for(var i = 0; i < cachedTileInfoArray.length; i++) {
                var tileInfo = cachedTileInfoArray[i];
                result.push(tileInfo.rowIndex + "|" + tileInfo.colIndex);
            }
            return result.join(";");
        },
        
        // Parent tile information
        getCellParentTileInfo: function(cell) {
            var tilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), cell.col, cell.row);
            return this.getGridTileInfo(tilePosition.row, tilePosition.col);
        },
        getParentTileInfo: function(selection) {
            var parentTilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), selection.range.leftColIndex, selection.range.topRowIndex);
            return this.getGridTileInfo(parentTilePosition.row, parentTilePosition.col);
        },
        getActiveCellParentTileInfo: function(selection) {
            var activeCellParentTilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), selection.activeCellColIndex, selection.activeCellRowIndex);
            return this.getGridTileInfo(activeCellParentTilePosition.row, activeCellParentTilePosition.col);
        },
        updateGridTileCache: function(serverVisibleRange) {
            var clientVisibleRange = this.getVisibleRange();

            if(serverVisibleRange && clientVisibleRange.top < serverVisibleRange.top ||
                clientVisibleRange.bottom > serverVisibleRange.bottom ||
                clientVisibleRange.left < serverVisibleRange.left ||
                clientVisibleRange.right > serverVisibleRange.right) {
                var matrix = this.getTileMatrix();

                for(var i = clientVisibleRange.top; i <= clientVisibleRange.bottom; i++) {
                    for(var k = clientVisibleRange.left; k <= clientVisibleRange.right; k++) {
                        var rowInRange = (serverVisibleRange.top <= i) && (i <= serverVisibleRange.bottom),
                            columnInRange = (serverVisibleRange.left <= k) && (k <= serverVisibleRange.right)
                        if(rowInRange && columnInRange) {
                            continue;
                        } else {
                            this.removeGridTileSmart(i, k);
                            if(!rowInRange)
                                ASPx.RemoveElement(this.getRenderProvider().getHeaderTileElement(i, false, this.getPaneType()));
                            if(!columnInRange)
                                ASPx.RemoveElement(this.getRenderProvider().getHeaderTileElement(k, true, this.getPaneType()));
                        }
                    }
                }
            }
            this.getTileMatrix().clearCache();
        },

        removeGridTileSmart: function(rowIndex, colIndex) {
            var gridTileElement = this.getRenderProvider().getGridTileElement(rowIndex, colIndex, this.getPaneType());
            if(gridTileElement) {
                var tileContainer = gridTileElement.parentNode;
                ASPx.RemoveElement(gridTileElement);
                if(tileContainer && tileContainer.childNodes.length === 0 && ASPx.ElementContainsCssClass(tileContainer, "dxss-fc"))
                    ASPx.RemoveElement(tileContainer);
            }
        },

        removeOutdatedTiles: function(outdatedTiles) {
            var matrix = this.getTileMatrix();
            for(var i = 0; i < outdatedTiles.length; i++) {
                var tileInfo = outdatedTiles[i];
                if(!tileInfo) continue;
                this.removeTile(matrix, outdatedTiles.rowIndex, outdatedTiles.colIndex);
            }
            ASPxClientSpreadsheet.Log.Write("outdatedTiles count = " + outdatedTiles.length);
            for(var i = 0; i < outdatedTiles.length; i++)
                ASPxClientSpreadsheet.Log.Write("rowIndex = " + outdatedTiles[i].rowIndex + " colIndex = " + outdatedTiles[i].colIndex);
        },
        removeTile: function(matrix, rowIndex, colIndex) {
            matrix.removeGridTileInfo(rowIndex, colIndex);
            ASPx.RemoveElement(this.getRenderProvider().getGridTileElement(rowIndex, colIndex, this.getPaneType()));
        },

        insertNewTiles: function(newTiles, visibleRange) {
            var grid = newTiles.grid;
            var columnHeader = newTiles.columnHeader;
            var rowHeader = newTiles.rowHeader;

            var matrix = this.getTileMatrix();

            for(var i = 0; i < grid.length; i++)
                matrix.insertGridTileInfo(grid[i]);
            for(var i = 0; i < columnHeader.length; i++)
                matrix.insertHeaderInfo(columnHeader[i], true);
            for(var i = 0; i < rowHeader.length; i++)
                matrix.insertHeaderInfo(rowHeader[i], false);

            this.ensureNewTiles(grid, columnHeader, rowHeader);

            this.loadGridTiles(visibleRange);
            this.loadHeaderTiles(visibleRange);

            ASPxClientSpreadsheet.Log.Write("newTiles count = " + grid.length);
            for(var i = 0; i < grid.length; i++)
                ASPxClientSpreadsheet.Log.Write("rowIndex = " + grid[i].rowIndex + " colIndex = " + grid[i].colIndex);

            ASPxClientSpreadsheet.Log.Write("newHeaderColTiles count = " + columnHeader.length);
            for(var i = 0; i < columnHeader.length; i++)
                ASPxClientSpreadsheet.Log.Write("index = " + columnHeader[i].index);

            ASPxClientSpreadsheet.Log.Write("newHeaderRowTiles count = " + rowHeader.length);
            for(var i = 0; i < rowHeader.length; i++)
                ASPxClientSpreadsheet.Log.Write("index = " + rowHeader[i].index);
        },
        forceRedrawAppearance: function(element) {
            ASPx.Attr.ChangeStyleAttribute(element, "width", "0px");
            var dummy = element.offsetWidth;
            ASPx.Attr.RestoreStyleAttribute(element, "width");
        },

        removeInvisibleTiles: function(visibleRange, onlyHeaders) {
            var trash = [],
                matrix = this.getTileMatrix();

            for(var k = 0; k < 2; k++) {
                var isCol = k == 0;
                var headerInfoArray = matrix.getHeaderInfoArray(isCol);
                for(var i = 0; i < headerInfoArray.length; i++) {
                    var index = headerInfoArray[i].index;
                    if(isCol && (index < visibleRange.left || index > visibleRange.right) || !isCol && (index < visibleRange.top || index > visibleRange.bottom))
                        trash.push(this.getRenderProvider().getHeaderTileElement(index, isCol, this.getPaneType()));
                }
            }
            var gridTileInfoArray = matrix.getGridTileInfoArray();
            if(!onlyHeaders) {
                for(var i = 0; i < gridTileInfoArray.length; i++) {
                    var tileInfo = gridTileInfoArray[i];
                    if(tileInfo.rowIndex < visibleRange.top || tileInfo.rowIndex > visibleRange.bottom)
                        trash.push(this.getRenderProvider().getGridTileRowElement(tileInfo.rowIndex, this.getPaneType()));
                    else if(tileInfo.colIndex < visibleRange.left || tileInfo.colIndex > visibleRange.right)
                        trash.push(this.getRenderProvider().getGridTileElement(tileInfo.rowIndex, tileInfo.colIndex, this.getPaneType()));
                }
            }
            for(var i = 0; i < trash.length; i++)
                ASPx.RemoveElement(trash[i]);
        },

        removeAllTiles: function() {
            var visibleRange = {
                left: -1,
                top: -1,
                right: -1,
                bottom: -1
            };
            this.removeInvisibleTiles(visibleRange); // TODO refactor it;
            this.getTileMatrix().clearCache();
            this.resetCroppedComplexBoxes();
        },

        getScrollBounds: function() { return this.scrollBounds; },

        calculateScrollBounds: function(visibleRange) {
            var top, right, bottom, left;
            top = right = bottom = left = 0;

            if(visibleRange.top > 0)
                top = visibleRange.top * this.tileSize.row + this.clientVisibleRangePadding.row;
            if(visibleRange.left > 0)
                left = visibleRange.left * this.tileSize.col + this.clientVisibleRangePadding.col;

            var anchor = {
                row: (visibleRange.bottom + 1) * this.tileSize.row - 1 - this.clientVisibleRangePadding.row,
                col: (visibleRange.right + 1) * this.tileSize.col - 1 - this.clientVisibleRangePadding.col
            };
            var maxScrollViewBound = this.calculateViewBoundRange(anchor, this.getRenderProvider().getGridOffsetSize(this.getPaneType()), false, true, true);
            if(maxScrollViewBound) {
                bottom = maxScrollViewBound.top;
                right = maxScrollViewBound.left;
            }
            return { top: top, right: right, bottom: bottom, left: left };
        },

        loadTilesAccordingToScrollAnchor: function() {
            var newVisibleRange = this.calculateNewVisibleRange(this.getScrollAnchorVisiblePosition());
            if(newVisibleRange) {
                this.loadInvisibleTiles();
                this.addVisibleRangeToWiatingForLoad(newVisibleRange);
            }
        },
        addVisibleRangeToWiatingForLoad: function(visibleRange) {
            this.visibleRangesWaitingForLoad.push(visibleRange);
        },
        calculateNewVisibleRange: function(scrollAnchor) {
            var viewBoundRange = this.calculateViewBoundRange(scrollAnchor, this.getRenderProvider().getGridOffsetSize(), false);
            if(!viewBoundRange) return; // WORKAROUND for IE9-

            var newVisibleRange = this.calculateVisibleRange(viewBoundRange);
            if(this.checkIsVisibleRangeAlreadyWaitingForLoad(newVisibleRange))
                return;

            return newVisibleRange;
        },
        // attention: use this method only through PaneManager
        tryLoadTilesFromCache: function() {
            var viewBoundRange = this.calculateViewBoundRange(this.getScrollAnchorVisiblePosition(), this.getRenderProvider().getGridOffsetSize(), true);
            if(!viewBoundRange)
                return;
            var newVisibleRange = this.calculateVisibleRange(viewBoundRange);
            var matrix = this.getTileMatrix();
            if(!matrix.containsRange(newVisibleRange))
                return;

            this.loadGridTiles(newVisibleRange);
            this.loadHeaderTiles(newVisibleRange);
            this.afterGridTilesLoaded(newVisibleRange);

            this.setVisibleRange(newVisibleRange);
            return true;
        },

        calculateVisibleRange: function(viewBoundRange) {
            var top = Math.max(0, viewBoundRange.top - this.visibleRangePadding.row);
            var left = Math.max(0, viewBoundRange.left - this.visibleRangePadding.col);
            var bottom = Math.min(this.maxRowCount, viewBoundRange.bottom + this.visibleRangePadding.row);
            var right = Math.min(this.maxColCount, viewBoundRange.right + this.visibleRangePadding.col);
            return {
                top: Math.floor(top / this.tileSize.row),
                right: Math.floor(right / this.tileSize.col),
                bottom: Math.floor(bottom / this.tileSize.row),
                left: Math.floor(left / this.tileSize.col)
            };
        },
        calculateViewBoundRange: function(cell, viewBounds, useOnlyCachedTiles, horzFarAlign, vertFarAlign) {
            var top = vertFarAlign ? this.calculateVisibleBoundIndex(cell, false, viewBounds.height, false, useOnlyCachedTiles) : cell.row;
            var right = !horzFarAlign ? this.calculateVisibleBoundIndex(cell, true, viewBounds.width, true, useOnlyCachedTiles) : cell.col;
            var bottom = !vertFarAlign ? this.calculateVisibleBoundIndex(cell, false, viewBounds.height, true, useOnlyCachedTiles) : cell.row;
            var left = horzFarAlign ? this.calculateVisibleBoundIndex(cell, true, viewBounds.width, false, useOnlyCachedTiles) : cell.col;

            if(top > -1 && right > -1 && bottom > -1 && left > -1 && right >= left && bottom >= top)
                return { top: top, right: right, bottom: bottom, left: left };
        },

        calculateVisibleBoundIndex: function(cell, isCol, minSize, isRightDir, useOnlyCachedTiles) {
            var cellIndex = isCol ? cell.col : cell.row,
                tileSize = isCol ? this.tileSize.col : this.tileSize.row,
                tileIndex = Math.floor(cellIndex / tileSize),
                cellPositionInTile = cellIndex - tileIndex * tileSize;

            var headerInfo = this.getHeaderInfo(tileIndex, isCol);
            if(!headerInfo && useOnlyCachedTiles)
                return -1;

            var boundIndexInfo = this.findBoundIndexInTilePart(headerInfo, isCol, minSize, cellPositionInTile, isRightDir);
            if(!boundIndexInfo.found)
                return -1;
            if(boundIndexInfo.index >= 0)
                return tileIndex * tileSize + boundIndexInfo.index;

            var inc = isRightDir ? 1 : -1;
            cellIndex += boundIndexInfo.partLength * inc;
            tileIndex += inc;

            var defaultTileInfo = this.getDefaultTileInfo();
            var totalSizeName = isCol ? "totalWidth" : "totalHeight";
            var diff = minSize - boundIndexInfo.partTotalSize;
            var i = 0;
            var prevDiff;
            do {
                headerInfo = this.getHeaderInfo(tileIndex, isCol);
                if(!headerInfo && useOnlyCachedTiles)
                    return -1;
                var tileTotalSize = headerInfo ? headerInfo[totalSizeName] : defaultTileInfo[totalSizeName];
                prevDiff = diff;
                diff -= tileTotalSize;
                i++;
                tileIndex += inc;

            } while(diff > 0)
            cellIndex += (i - 1) * tileSize * inc;
            tileIndex -= inc;

            headerInfo = this.getHeaderInfo(tileIndex, isCol);
            if(!headerInfo && useOnlyCachedTiles)
                return -1;
            cellPositionInTile = isRightDir ? 0 : tileSize - 1;
            boundIndexInfo = this.findBoundIndexInTilePart(headerInfo, isCol, prevDiff, cellPositionInTile, isRightDir);
            return tileIndex * tileSize + boundIndexInfo.index;
        },
        findBoundIndexInTilePart: function(headerInfo, isCol, bound, startIndex, isRightDir) {
            var defaultTileInfo = this.getDefaultTileInfo();
            var sizeName = isCol ? "widths" : "heights";
            var tileSizes = headerInfo ? headerInfo[sizeName] : defaultTileInfo[sizeName];

            var partSizes = isRightDir ? tileSizes.slice(startIndex) : tileSizes.slice(0, startIndex + 1).reverse();
            var partSizeRanges = _aspxGetIncrementalRangeArray(partSizes);
            var partTotalSize = partSizeRanges[partSizeRanges.length - 1];

            if(bound === partTotalSize)
                return { index: !isRightDir ? 0 : tileSizes.length - 1, found: true };
            if(bound < partTotalSize) {
                var indexInPart = ASPx.Data.ArrayBinarySearch(partSizeRanges, bound, ASPx.Data.NearestLeftBinarySearchComparer);
                var index = startIndex + indexInPart * (isRightDir ? 1 : -1);
                return { index: index, found: index >= 0 };
            }
            return { index: -1, partTotalSize: partTotalSize, partLength: partSizes.length, found: true };
        },
        getDefaultTileInfo: function() {
            if(!this.defaultTileInfo) {
                var totalWidth = this.tileSize.col * this.getDefaultCellSize().width;
                var totalHeight = this.tileSize.row * this.getDefaultCellSize().height;
                var widths = [];
                var heights = [];
                var iterationCount = Math.max(this.tileSize.col, this.tileSize.row);
                for(var i = 0; i < iterationCount; i++) {
                    if(i < this.tileSize.col)
                        widths.push(this.getDefaultCellSize().width);
                    if(i < this.tileSize.row)
                        heights.push(this.getDefaultCellSize().height);
                }
                this.defaultTileInfo = { totalWidth: totalWidth, totalHeight: totalHeight, widths: widths, heights: heights };
            }
            return this.defaultTileInfo;
        },

        getVisiblePartSize: function() {
            var startCellRowIndex = this.visibleRange.top * this.tileSize.row;
            var endCellRowIndex = (this.visibleRange.bottom + 1) * this.tileSize.row - 1;
            var startCellColIndex = this.visibleRange.left * this.tileSize.col;
            var endCellColIndex = (this.visibleRange.right + 1) * this.tileSize.col - 1;
            return {
                width: this.calculateCellSize(startCellColIndex, endCellColIndex, true, true),
                height: this.calculateCellSize(startCellRowIndex, endCellRowIndex, false, true)
            };
        },

        ensureNewTiles: function(gridTiles, columnHeaderTiles, rowHeaderTiles) {
            var colHeaderGridLines = {};
            var rowHeaderGridLines = {};
            for(var i = 0; i < columnHeaderTiles.length; i++) {
                var tileHeaderInfo = columnHeaderTiles[i];
                this.hasHiddenElementsBetweenTiles(i, columnHeaderTiles);
                tileHeaderInfo.htmlElement = this.createHeaderTileHtmlElement(tileHeaderInfo, true);
                colHeaderGridLines[tileHeaderInfo.index] = ASPx.GetChildByClassName(tileHeaderInfo.htmlElement, "dxss-gl");
            }
            for(var i = 0; i < rowHeaderTiles.length; i++) {
                var tileHeaderInfo = rowHeaderTiles[i];
                this.hasHiddenElementsBetweenTiles(i, rowHeaderTiles);
                tileHeaderInfo.htmlElement = this.createHeaderTileHtmlElement(tileHeaderInfo, false);
                rowHeaderGridLines[tileHeaderInfo.index] = ASPx.GetChildByClassName(tileHeaderInfo.htmlElement, "dxss-gl");
            }
            for(var i = 0; i < gridTiles.length; i++) {
                var tileInfo = gridTiles[i];
                tileInfo.htmlElement = ASPx.GetChildByTagName(this.createFragmentFromHtml(tileInfo.html), "DIV");
                if(colHeaderGridLines[tileInfo.colIndex])
                    tileInfo.htmlElement.appendChild(colHeaderGridLines[tileInfo.colIndex].cloneNode(true));
                if(rowHeaderGridLines[tileInfo.rowIndex])
                    tileInfo.htmlElement.appendChild(rowHeaderGridLines[tileInfo.rowIndex].cloneNode(true));
            }
        },
        hasHiddenElementsBetweenTiles: function(tileIndex, tiles) {
            var firstTile = tiles[tileIndex - 1],
                secondTile = tiles[tileIndex];
            if(firstTile && secondTile && firstTile.modelIndices && secondTile.modelIndices) {
                if(secondTile.modelIndices[0] - firstTile.modelIndices[firstTile.modelIndices.length - 1] > 1)
                    this.thereAreHiddenElementsBetweenTiles = true;
            }
        },
        createHeaderTileHtmlElement: function(headerInfo, isCol) {
            var headerElement = this.getHeaderTemplate(headerInfo.index, isCol).cloneNode(true);
            headerElement.id = this.getRenderProvider().getHeaderTileElementId(headerInfo.index, isCol, this.getPaneType()); // TODO rework id to data attribute

            var cellContainer = this.getHeaderCellContainer(headerElement);

            this.assignHeaderContentSizes(headerElement, headerInfo, isCol);
            this.populateHeaderTextCells(headerElement, headerInfo, isCol, cellContainer);
            this.assignHeaderTileIndex(headerInfo.index, cellContainer);
            return headerElement;
        },
        getHeaderCellContainer: function(headerElement) {
            return ASPx.GetChildByClassName(headerElement, ASPx.SpreadsheetCssClasses.HeaderContainer);
        },
        assignHeaderContentSizes: function(headerElement, headerInfo, isCol) {
            var headerOffsetSize = this.parentPane.getHeaderOffsetSize();
            var headerOffsetWidth = headerOffsetSize.width;
            var headerOffsetHeight = headerOffsetSize.height; // TODO get header width and all header heigths from server and use it here (or create css rule for all header elements)
            //var headerOffset = this.parentPane.headerOffset; 
            var hWidth = isCol ? headerInfo.totalWidth : headerOffsetWidth;
            var hHeight = isCol ? headerOffsetHeight : headerInfo.totalHeight;
            if(isCol)
                headerElement.style.width = hWidth + "px";
            headerElement.style.height = hHeight + "px";
            if(isCol)
                headerElement.style.lineHeight = hHeight + "px";

            var gridLineContainer, cellContainer;
            for(var i = 0; i < headerElement.childNodes.length; i++) {
                var child = headerElement.childNodes[i];
                if(child.className === "dxss-gl")
                    gridLineContainer = child;
                else if(child.className === ASPx.SpreadsheetCssClasses.HeaderContainer)
                    cellContainer = child;
            }
            var tileSizeInfo = this.ensureNewHeaderTileSizes(headerInfo, isCol);
            var stylePosName = isCol ? "left" : "top";
            var index = 0;
            for(var i = 0; i < gridLineContainer.childNodes.length; i++) {
                var line = gridLineContainer.childNodes[i];
                if(line.tagName !== "DIV") continue;

                if(headerInfo.modelIndices) {
                    if(i >= 1)
                        if(headerInfo.modelIndices[i] - headerInfo.modelIndices[i - 1] > 1)
                            line.className += " db";
                    if((i === 0) && this.thereAreHiddenElementsBetweenTiles) {
                        line.className += " db";
                        this.thereAreHiddenElementsBetweenTiles = false;
                    }
                }

                line.style[stylePosName] = tileSizeInfo.incrementalSizeRanges[index] + "px";
                index++
            }
            var styleSizeName = isCol ? "width" : "height";
            index = 0;
            for(var i = 0; i < cellContainer.childNodes.length; i++) {
                var cell = cellContainer.childNodes[i];
                if(cell.tagName !== "DIV") continue;
                cell.style[stylePosName] = tileSizeInfo.incrementalSizeRanges[index] + "px";
                cell.style[styleSizeName] = tileSizeInfo.sizes[index] + "px";
                if(!isCol)
                    cell.style.lineHeight = tileSizeInfo.sizes[index] + "px";
                index++
            }
        },
        populateHeaderTextCells: function(headerElement, headerInfo, isCol, cellContainer) {
            cellContainer = cellContainer || this.getHeaderCellContainer(headerElement);
            if(headerInfo.modelIndices)
                this.populateHeaderTextCellsByModelIndices(headerElement, headerInfo, isCol, cellContainer);
            else
                this.populateHeaderTextCellsByOrder(headerElement, headerInfo, isCol, cellContainer);
        },
        populateHeaderTextCellsByOrder: function(headerElement, headerInfo, isCol, cellContainer) {
            var index = 0;
            var tileSize = isCol ? this.tileSize.col : this.tileSize.row;
            var startCellIndexInTile = headerInfo.index * tileSize;
            if(!isCol)
                startCellIndexInTile++;
            for(var i = 0; i < cellContainer.childNodes.length; i++) {
                var cell = cellContainer.childNodes[i];
                if(cell.tagName !== "DIV") continue;
                cell.innerHTML = this.getHeaderCellTextByOrder(startCellIndexInTile + index, isCol);
                index++
            }
        },
        populateHeaderTextCellsByModelIndices: function(headerElement, headerInfo, isCol, cellContainer) {
            for(var i = 0; i < cellContainer.childNodes.length; i++) {
                var cell = cellContainer.childNodes[i];
                if(cell.tagName !== "DIV") continue;
                cell.innerHTML = this.getHeaderCellTextByOrder(headerInfo.modelIndices[i] + (isCol ? 0 : 1), isCol);
            }
        },
        assignHeaderTileIndex: function(tileIndex, cellContainer) {
            cellContainer.setAttribute(ASPxClientSpreadsheet.TileHelper.DataAttributes.HeaderContainerTileIndex, tileIndex);
        },
        ensureNewHeaderTileSizes: function(headerInfo, isCol) { // TODO move to TileMatrix
            var sizeName = isCol ? "widths" : "heights";
            var sizeRangeName = isCol ? "incrementalWidths" : "incrementalHeights";
            headerInfo[sizeRangeName] = _aspxGetIncrementalRangeArray(headerInfo[sizeName]);
            return { sizes: headerInfo[sizeName], incrementalSizeRanges: headerInfo[sizeRangeName] };
        },
        getHeaderCellTextByOrder: function(cellIndex, isCol) {
            return isCol ? this.getHeaderCellIndexStringRepresentation(cellIndex) : cellIndex;
        },

        getHeaderCellIndexStringRepresentation: function(index) {
            var str = ASPxClientSpreadsheet.CellPositionConvertor.getStringRepresentaion(index);
            return str.split("").reverse().join("");
        },
        getHeaderTemplate: function(tileIndex, isCol) {
            var template = isCol ? this.colHeaderTemplate : this.rowHeaderTemplate;
            if(!template) {
                template = this.createHeaderTemplate(tileIndex, isCol);
                if(isCol)
                    this.colHeaderTemplate = template;
                else
                    this.rowHeaderTemplate = template;
            }
            return template;
        },
        createHeaderTemplate: function(tileIndex, isCol) {
            var mainDiv = document.createElement("DIV");
            mainDiv.className = "dxss-ht";
            var cellContainer = document.createElement("DIV");
            cellContainer.className = ASPx.SpreadsheetCssClasses.HeaderContainer;
            cellContainer.setAttribute(ASPxClientSpreadsheet.TileHelper.DataAttributes.HeaderContainerTileIndex, tileIndex);
            mainDiv.appendChild(cellContainer);
            var gridLinesContainer = document.createElement("DIV");
            gridLinesContainer.className = "dxss-gl";
            mainDiv.appendChild(gridLinesContainer);
            var gridLineClass = ASPx.SpreadsheetCssClasses.getGridLine(isCol);
            var cellCount = isCol ? this.tileSize.col : this.tileSize.row;
            for(var i = 0; i < cellCount; i++) {
                var cell = document.createElement("DIV");
                cell.className = ASPx.SpreadsheetCssClasses.HeaderCell;
                cell.setAttribute(ASPxClientSpreadsheet.TileHelper.DataAttributes.HeaderCellIndex, i);
                cellContainer.appendChild(cell);
                var gl = document.createElement("DIV");
                gl.className = gridLineClass;
                gridLinesContainer.appendChild(gl);
            }
            return mainDiv;
        },

        insertGridTile: function(tileInfo) {
            if(!tileInfo) return;

            var newTile = tileInfo.htmlElement;

            var oldTile = this.getRenderProvider().getGridTileElement(tileInfo.rowIndex, tileInfo.colIndex, this.getPaneType());
            if(oldTile) {
                oldTile.parentNode.replaceChild(newTile, oldTile);
                return;
            }
            var row = this.getRenderProvider().getGridTileRowElement(tileInfo.rowIndex, this.getPaneType());
            if(!row)
                row = this.createGridTileRow(tileInfo.rowIndex);
            var refTile = this.findReferenceElementForTileInsertion(row, tileInfo.colIndex);
            if(refTile)
                row.insertBefore(newTile, refTile);
            else
                row.appendChild(newTile);
        },
        insertHeaderTile: function(headerTileInfo, isCol) {
            if(!headerTileInfo) return;
            var newHeaderTileElement = headerTileInfo.htmlElement;
            var oldHeaderTileElement = this.getRenderProvider().getHeaderTileElement(headerTileInfo.index, isCol, this.getPaneType());
            if(oldHeaderTileElement) {
                oldHeaderTileElement.parentNode.replaceChild(newHeaderTileElement, oldHeaderTileElement);
            } else {
                var container = isCol ? this.getColumnHeaderTilesContainer() : this.getRowHeaderTilesContainer();
                var refTile = this.findReferenceElementForTileInsertion(container, headerTileInfo.index);
                if(refTile)
                    container.insertBefore(newHeaderTileElement, refTile);
                else
                    container.appendChild(newHeaderTileElement);
            }
        },
        findReferenceElementForTileInsertion: function(container, index) {
            var siblings = ASPx.GetChildNodesByTagName(container, "DIV");
            for(var i = 0; i < siblings.length; i++) {
                var sibling = siblings[i];
                var siblingIndex = this.getNumberFromEndOfString(sibling.id);
                if(siblingIndex > index)
                    return sibling;
            }
            return null;
        },

        createGridTileRow: function(rowIndex) {
            if(!this.etalonGridTileRow) {
                var fragment = this.createFragmentFromHtml(this.etalonGridTileRowHtml);
                this.etalonGridTileRow = ASPx.GetChildByTagName(fragment, "DIV");
            }
            var row = this.etalonGridTileRow.cloneNode(true);
            row.id = this.getRenderProvider().getGridTileRowElementId(rowIndex, this.getPaneType(), this.getPaneType());
            this.insertGridTileRow(row, rowIndex);
            return row;
        },
        insertGridTileRow: function(row, rowIndex) {
            var container = this.getGridTilesContainer();
            var siblings = ASPx.GetChildNodesByTagName(container, "DIV");
            for(var i = 0; i < siblings.length; i++) {
                var sibling = siblings[i];
                var siblingRowIndex = this.getNumberFromEndOfString(sibling.id);
                if(siblingRowIndex > rowIndex) {
                    container.insertBefore(row, sibling);
                    return;
                }
            }
            container.appendChild(row);
        },

        calculateCellSize: function(startIndex, endIndex, isCol, useOnlyCachedTiles) {
            if(startIndex < 0 || endIndex < 0)
                return 0;
            var count = endIndex - startIndex + 1;

            var tileSize = isCol ? this.tileSize.col : this.tileSize.row;
            var tileIndex = Math.floor(startIndex / tileSize);
            var headerInfo = this.getHeaderInfo(tileIndex, isCol);
            if(!headerInfo && useOnlyCachedTiles)
                return 0;

            var startIndexInTile = startIndex - tileIndex * tileSize;
            var endIndexInTile = endIndex - tileIndex * tileSize;
            if(endIndexInTile >= tileSize)
                endIndexInTile = tileSize - 1;

            var size = this.calculateCellSizeInTile(headerInfo, startIndexInTile, endIndexInTile, isCol);
            count -= endIndexInTile - startIndexInTile + 1;
            tileIndex++;
            if(count === 0)
                return size;

            var defaultTileInfo = this.getDefaultTileInfo();
            var totalSizeName = isCol ? "totalWidth" : "totalHeight";
            var lastTileIndex = tileIndex + Math.floor(count / tileSize);
            for(var i = tileIndex; i < lastTileIndex; i++) {
                headerInfo = this.getHeaderInfo(i, isCol);
                if(!headerInfo && useOnlyCachedTiles)
                    return 0;
                size += headerInfo ? headerInfo[totalSizeName] : defaultTileInfo[totalSizeName];
                count -= tileSize;
            }
            headerInfo = this.getHeaderInfo(lastTileIndex, isCol);
            if(!headerInfo && useOnlyCachedTiles)
                return 0;
            size += this.calculateCellSizeInTile(headerInfo, 0, count - 1, isCol);
            return size;
        },
        calculateCellSizeInTile: function(headerInfo, startIndex, endIndex, isCol) {
            if(!headerInfo) {
                var size = isCol ? this.getDefaultCellSize().width : this.getDefaultCellSize().height;
                return (endIndex - startIndex + 1) * size;
            }
            var incSizeName = isCol ? "incrementalWidths" : "incrementalHeights";
            var incrementSizeRanges = headerInfo[incSizeName];
            var partSizeRanges = incrementSizeRanges.slice(startIndex, endIndex + 2);
            return partSizeRanges[partSizeRanges.length - 1] - partSizeRanges[0];
        },

        invalidateVisibleRangesWaitingForLoad: function() { // use callbackIDs for invalidating
            var newRanges = [];
            for(var i = 0; i < this.visibleRangesWaitingForLoad.length; i++) {
                var range = this.visibleRangesWaitingForLoad[i];
                if(!this.isRangeContainsOther(this.visibleRange, range))
                    newRanges.push(range);
            }
            this.visibleRangesWaitingForLoad = newRanges;
        },

        checkIsVisibleRangeAlreadyWaitingForLoad: function(target) {
            for(var i = 0; i < this.visibleRangesWaitingForLoad.length; i++)
                if(this.isRangeContainsOther(this.visibleRangesWaitingForLoad[i], target))
                    return true;
            return false;
        },

        isRangeContainsOther: function(source, target) { // TODO create client rect object
            return target.top >= source.top && target.left >= source.left && target.bottom <= source.bottom && target.right <= source.right;
        },

        patchIDs: function(html) {
            var renderProvider = this.getRenderProvider(),
                panePostfix = renderProvider.getPanePostfixByType(this.getPaneType()),
                postfix = panePostfix != "" ? panePostfix + "_" : panePostfix + "_";

            var newIdPrefix = "id=\"" + renderProvider.getChildControlsPrefix(true) + postfix
            return String(html).replace(/id="/g, newIdPrefix);
        },
        createFragmentFromHtml: function(html) {
            html = this.patchIDs(html);
            var container = document.createElement("DIV");
            container.innerHTML = html;
            var doc = document.createDocumentFragment();
            var first = null;
            for(var i = container.childNodes.length - 1; i >= 0; i--) {
                var el = container.childNodes[i];
                if(first)
                    doc.insertBefore(el, first);
                else
                    doc.appendChild(el);
                first = el;
            }
            return doc;
        },

        getNumberFromEndOfString: function(str) { // TODO use regExp
            var value = -1;
            var n = str.length - 1;
            while(parseInt(str.substr(n), 10) >= 0) {
                value = parseInt(str.substr(n), 10);
                n--;
            }
            return value;
        },

        getCellByCoord: function(x, y) {
            var cellVisiblePosition = this.getCellVisiblePositionByCoords(x, y);

            var cellLayoutInfo = this.getCellLayoutInfo(cellVisiblePosition.colIndex, cellVisiblePosition.rowIndex);
            return cellLayoutInfo;
        },
        convertVisiblePositionToModelPosition: function(colIndex, rowIndex) {
            return {
                colIndex: this.convertVisibleIndexToModelIndex(colIndex, true),
                rowIndex: this.convertVisibleIndexToModelIndex(rowIndex, false)
            };
        },

        getTileSize: function(isCol) {
            return isCol ? this.tileSize.col : this.tileSize.row;
        },
        findCellPositionInTileInfo: function(visibleIndex, isCol) {
            var tileSize = this.getTileSize(isCol);
            var tileIndex = Math.floor(visibleIndex / tileSize);
            var cellIndexInTile = visibleIndex - tileIndex * tileSize;
            return {
                tileSize: tileSize,
                tileIndex: tileIndex,
                cellIndexInTile: cellIndexInTile
            };
        },

        convertVisibleIndexToModelIndex: function(visibleIndex, isCol) {
            var cellPosInTile = this.findCellPositionInTileInfo(visibleIndex, isCol);
            var headerInfo = this.getHeaderInfo(cellPosInTile.tileIndex, isCol);
            if(!headerInfo || this.getHeaderTileIsClientTemporal(headerInfo))
                return -1;

            return headerInfo.modelIndices[cellPosInTile.cellIndexInTile];
        },
        convertFormulaRangeToVisibleRange: function(modelTopLeftIndex, modelBottomRightIndex, isCol) {
            var visibleTopLeftIndex = this.convertModelIndexToVisibleIndex(modelTopLeftIndex, isCol);
            var visibleBottomRightIndex = this.convertModelIndexToVisibleIndex(modelBottomRightIndex, isCol);
            if(visibleBottomRightIndex < 0 && visibleTopLeftIndex < 0)
                return { visibleTopLeftIndex: -1, visibleBottomRightIndex: -1 };

            if(visibleTopLeftIndex < 0)
                visibleTopLeftIndex = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(modelTopLeftIndex, isCol);
            if(visibleBottomRightIndex < 0)
                visibleBottomRightIndex = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(modelBottomRightIndex, isCol, true);

            if(visibleTopLeftIndex == -1 || visibleBottomRightIndex == -1) {
                var modelRangeIntersectsVisibleRange = this.getIsModelRangeIntersectsVisibleRange(modelTopLeftIndex, modelBottomRightIndex, isCol);
                if(modelRangeIntersectsVisibleRange) {
                    var visibleVisibleCellRange = this.getVisibleVisibleCellRange();
                    if(visibleTopLeftIndex == -1)
                        visibleTopLeftIndex = isCol ? visibleVisibleCellRange.top : visibleVisibleCellRange.left;
                    if(visibleBottomRightIndex == -1)
                        visibleBottomRightIndex = isCol ? visibleVisibleCellRange.bottom : visibleVisibleCellRange.right;
                }
            }
            return { visibleTopLeftIndex: visibleTopLeftIndex, visibleBottomRightIndex: visibleBottomRightIndex };
        },
        convertModelIndicesRangeToVisibleIndices: function(modelTopLeftIndex, modelBottomRightIndex, isCol) {
            var visibleTopLeftIndex = this.convertModelIndexToVisibleIndex(modelTopLeftIndex, isCol),
                rangeExistOnClient = { topLeft : true, bottomRight : true };
            if(visibleTopLeftIndex < 0)
                visibleTopLeftIndex = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(modelTopLeftIndex, isCol);

            var visibleBottomRightIndex = this.convertModelIndexToVisibleIndex(modelBottomRightIndex, isCol);
            if(visibleBottomRightIndex < 0)
                visibleBottomRightIndex = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(modelBottomRightIndex, isCol);

            //NOTE: (T223707) indeces can be negative if
            //not all range (some tiles in range) has on client side
            if(visibleTopLeftIndex < 0) {
                visibleTopLeftIndex = modelTopLeftIndex;
                rangeExistOnClient.topLeft = false;
            }
            if(visibleBottomRightIndex < 0) {
                visibleBottomRightIndex = modelBottomRightIndex;
                rangeExistOnClient.bottomRight = false;
            }

            return { visibleTopLeftIndex: visibleTopLeftIndex, visibleBottomRightIndex: visibleBottomRightIndex, rangeExist: rangeExistOnClient };
        },
        getIsModelRangeIntersectsVisibleRange: function(modelTopLeftIndex, modelBottomRightIndex, isCol) {
            var visibleModelCellRange = this.getVisibleModelCellRange();
            if(isCol)
                return modelTopLeftIndex <= visibleModelCellRange.bottom && modelBottomRightIndex >= visibleModelCellRange.top;
            return modelTopLeftIndex <= visibleModelCellRange.right && modelBottomRightIndex >= visibleModelCellRange.left;
        },
        convertModelIndexToVisibleIndex: function(modelIndex, isCol) {
            var headerInfo = this.findHeaderInfoByCellModelIndex(modelIndex, isCol);
            if(!headerInfo || this.getHeaderTileIsClientTemporal(headerInfo))
                return -1;

            var tileSize = this.getTileSize(isCol);
            for(var i = 0; i < tileSize; i++) {
                if(headerInfo.modelIndices[i] == modelIndex) {
                    var cellVisibleIndexInTile = i;
                    var cellBeforeCurrentTile = tileSize * headerInfo.index;
                    var cellVisibleIndex = cellBeforeCurrentTile + cellVisibleIndexInTile;
                    return cellVisibleIndex;
                }
            }
            return -1;
        },
        convertModelRangeToVisible: function(range) {
            var convertedRange = range.clone();
            convertedRange.isVisible = true;
            var modelColRange = this.convertFormulaRangeToVisibleRange(
                    convertedRange.leftColIndex,
                    convertedRange.rightColIndex,
                    true);

            var modelRowRange = this.convertFormulaRangeToVisibleRange(
                    convertedRange.topRowIndex,
                    convertedRange.bottomRowIndex,
                    false);

            if(modelColRange.visibleTopLeftIndex < 0 && modelColRange.visibleBottomRightIndex < 0 ||
                modelRowRange.visibleTopLeftIndex < 0 && modelRowRange.visibleBottomRightIndex < 0)
                convertedRange.isVisible = false;

            convertedRange.leftColIndex = modelColRange.visibleTopLeftIndex;
            convertedRange.rightColIndex = modelColRange.visibleBottomRightIndex;
            convertedRange.topRowIndex = modelRowRange.visibleTopLeftIndex;
            convertedRange.bottomRowIndex = modelRowRange.visibleBottomRightIndex;
            return convertedRange;
        },
        findHeaderInfoByCellModelIndex: function(modelIndex, isCol) {
            var tileSize = this.getTileSize(isCol);
            var headerInfo = null;

            var matrix = this.getTileMatrix();
            matrix.findHeaderInfo(isCol, function(curHeaderInfo) {
                if(this.getHeaderTileIsClientTemporal(curHeaderInfo)) return;
                if(curHeaderInfo.modelIndices[0] <= modelIndex && modelIndex <= curHeaderInfo.modelIndices[tileSize - 1]) {
                    headerInfo = curHeaderInfo;
                    return true;
                }
            }.aspxBind(this));
            return headerInfo;
        },
        getCellVisiblePositionByCoords: function(x, y) {
            var headerOffsetSize = this.parentPane.getHeaderOffsetSizeForMousePointer();

            x -= headerOffsetSize.width;
            y -= headerOffsetSize.height;

            var anchor = this.getPaneTopLeftCellVisiblePosition();
            var colIndex = this.calculateVisibleBoundIndex(anchor, true, x, true, true);
            var rowIndex = this.calculateVisibleBoundIndex(anchor, false, y, true, true);
            return {
                colIndex: colIndex,
                rowIndex: rowIndex
            }
        },
        getCellLayoutInfo_ByModelIndices: function(colModelIndex, rowModelIndex) {
            var colVisibleIndex = this.convertModelIndexToVisibleIndex(colModelIndex, true);
            var rowVisibleIndex = this.convertModelIndexToVisibleIndex(rowModelIndex, false);
            return this.getCellLayoutInfo(colVisibleIndex, rowVisibleIndex);
        },
        getCellLayoutInfo: function(colVisibleIndex, rowVisibleIndex) {
            if(colVisibleIndex < 0 || rowVisibleIndex < 0)
                return new ASPxClientSpreadsheet.TileHelper.CellLayoutInfo(null, colVisibleIndex, rowVisibleIndex);

            var cellModelPosition = this.convertVisiblePositionToModelPosition(colVisibleIndex, rowVisibleIndex);

            var tilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), colVisibleIndex, rowVisibleIndex);

            var tileInfo = this.getGridTileInfo(tilePosition.row, tilePosition.col);

            var cellPositionInTile = this.getCellPositionInTile(tilePosition, colVisibleIndex, rowVisibleIndex);
            var rectangle = this.getCellRectInsideTile(tilePosition, cellPositionInTile);
            if(rectangle && cellModelPosition.colIndex >= 0 && cellModelPosition.rowIndex >= 0)
                return new ASPxClientSpreadsheet.TileHelper.CellLayoutInfo(tileInfo,
                                                                            colVisibleIndex, rowVisibleIndex,
                                                                            cellModelPosition.colIndex, cellModelPosition.rowIndex,
                                                                            rectangle, this.getPaneType());
            return null;
        },
        // NOTE: can return NULL if there is not such a tile in cache
        getCellRectInsideTile: function(tilePosition, cellPositionInTile) {
            var colHeaderInfo = this.getHeaderInfo(tilePosition.col, true);
            var rowHeaderInfo = this.getHeaderInfo(tilePosition.row, false);
            if(!colHeaderInfo || !rowHeaderInfo)
                return null;

            return {
                x: colHeaderInfo.incrementalWidths[cellPositionInTile.colIndex],
                y: rowHeaderInfo.incrementalHeights[cellPositionInTile.rowIndex],
                width: this.getTileSizes(tilePosition.col, true)[cellPositionInTile.colIndex],
                height: this.getTileSizes(tilePosition.row, false)[cellPositionInTile.rowIndex]
            };
        },
        getCellPositionInTile: function(tilePosition, colIndex, rowIndex) {
            var colIndexInTile = colIndex - tilePosition.col * this.tileSize.col;
            var rowIndexInTile = rowIndex - tilePosition.row * this.tileSize.row;
            return { colIndex: colIndexInTile, rowIndex: rowIndexInTile };
        },
        getCellEditingText: function(colModelIndex, rowModelIndex) {
            var cellEditingText = this.getCellEditingTextCore(colModelIndex, rowModelIndex);
            if(ASPx.IsExists(cellEditingText))
                return cellEditingText;
            return this.getCellValue(colModelIndex, rowModelIndex);
        },
        getCellEditingTextCore: function(colModelIndex, rowModelIndex) {
            var cellTileInfo = this.getCellTileInfo_ByModelIndex(colModelIndex, rowModelIndex);
            return cellTileInfo ? cellTileInfo.editTexts[colModelIndex + "|" + rowModelIndex] : null;
        },
        getCellValue: function(colModelIndex, rowModelIndex) {
            var variantValue = this.getCellVariantValue(colModelIndex, rowModelIndex);
            return variantValue == null ? null : variantValue.value;
        },
        getCellVariantValue: function(colModelIndex, rowModelIndex) {
            var cellTileInfo = this.getCellTileInfo_ByModelIndex(colModelIndex, rowModelIndex);
            var strVariantValue = cellTileInfo ? cellTileInfo.values[colModelIndex + "|" + rowModelIndex] : cellTileInfo;
            if(strVariantValue) {
                var variantValue = strVariantValue.split("|");
                return {
                    type: variantValue[0],
                    value: strVariantValue.substr(variantValue[0].length + 1)
                }
            }
            return null;
        },
        getCellTileInfo_ByModelIndex: function(colModelIndex, rowModelIndex) {
            var colVisibleIndex = this.convertModelIndexToVisibleIndex(colModelIndex, true);
            if(colVisibleIndex < 0)
                colVisibleIndex = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(colModelIndex, true);
            var rowVisibleIndex = this.convertModelIndexToVisibleIndex(rowModelIndex, false);
            if(rowVisibleIndex < 0)
                rowVisibleIndex = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(rowModelIndex, false);

            var tilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), colVisibleIndex, rowVisibleIndex);

            return this.getGridTileInfo(tilePosition.row, tilePosition.col);
        },
        //getCellTileInfo_ByVisibleIndex: function(colVisibleIndex, rowVisibleIndex){
        //    var tilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this, colVisibleIndex, rowVisibleIndex);
        //    return this.getGridTileInfo(tilePosition.row, tilePosition.col);
        //},

        /* Complex Box helper */
        getTileCache: function(cellTileInfo) {
            if(!cellTileInfo.cache)
                cellTileInfo.cache = {};
            return cellTileInfo.cache;
        },
        getTileCache_ComplexBoxes: function(cellTileInfo) {
            var tileCache = this.getTileCache(cellTileInfo);
            if(!tileCache.complexBoxes)
                tileCache.complexBoxes = this.getComplexBoxes(cellTileInfo);
            return tileCache.complexBoxes;
        },

        // Complex boxes
        getComplesBoxesPrepared: function(cellTileInfo) {
            var complexes = cellTileInfo.complexes;
            if(!complexes.prepared) {
                var tileColOffset = cellTileInfo.colIndex * this.tileSize.col;
                var tileRowOffset = cellTileInfo.rowIndex * this.tileSize.row;

                for(var i = 0; i < complexes.length; i++) {
                    var complexBox = complexes[i];

                    var cloneComplexBox = this.cloneComplexBoxes(complexes[i]);
                    var complexBoxCropped = false;

                    complexBox[0] = this.convertModelIndexToVisibleIndex(cloneComplexBox.leftColIndex, true);
                    if(complexBox[0] < 0) {
                        complexBox[0] = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(cloneComplexBox.leftColIndex, true);
                        complexBoxCropped = true;
                    }

                    complexBox[1] = this.convertModelIndexToVisibleIndex(cloneComplexBox.topRowIndex, false);
                    if(complexBox[1] < 0) {
                        complexBox[1] = this.findExistVisibleIndexForComplexBoxesOrFormulaRange(cloneComplexBox.topRowIndex, false);
                        complexBoxCropped = true;
                    }

                    if(complexBoxCropped)
                        this.croppedComplexBoxes.push(cloneComplexBox);

                    complexBox[2] = this.convertModelIndexToVisibleIndex(complexBox[2], true);
                    complexBox[3] = this.convertModelIndexToVisibleIndex(complexBox[3], false);

                    complexBox[0] -= tileColOffset;
                    complexBox[1] -= tileRowOffset;
                    complexBox[2] -= tileColOffset;
                    complexBox[3] -= tileRowOffset;
                }
                complexes.prepared = true;
            }
            return complexes;
        },
        cloneComplexBoxes: function(complexBox) {
            var initialModelIndex = {};
            initialModelIndex.leftColIndex = complexBox[0];
            initialModelIndex.topRowIndex = complexBox[1];
            initialModelIndex.rightColIndex = complexBox[2];
            initialModelIndex.bottomRowIndex = complexBox[3];
            return initialModelIndex;
        },
        findExistVisibleIndexForComplexBoxesOrFormulaRange: function(modelIndex, isCol, findLess) {
            var headerInfo = this.findHeaderInfoByCellModelIndex(modelIndex, isCol),
                tileSize = this.getTileSize(isCol);
            if(!headerInfo || this.getHeaderTileIsClientTemporal(headerInfo))
                return this.tryToFindExistVisibleIndexBetweenTiles(modelIndex, isCol);

            for(var i = 0; i < tileSize; i++) {
                if(headerInfo.modelIndices[i] >= modelIndex) {
                    var cellVisibleIndexInTile = findLess ? i - 1 : i;
                    var cellBeforeCurrentTile = tileSize * headerInfo.index;
                    var cellVisibleIndex = cellBeforeCurrentTile + cellVisibleIndexInTile;
                    return cellVisibleIndex;
                }
            }
            throw new Error("Can't find visible index");
        },
        tryToFindExistVisibleIndexBetweenTiles: function(modelIndex, isCol) {
            var matrix = this.getTileMatrix(),
                boundaryArray = matrix.getModelIndecesBoundaryPositions(isCol),
                len = boundaryArray.length;
            for(var i = 0; i < len; i += 2) {
                if((boundaryArray[i + 1] - boundaryArray[i] > 1) && boundaryArray[i] < modelIndex && boundaryArray[i + 1] > modelIndex)
                    return  boundaryArray[i + 1] - 1;
            }
            return -1;
        },
        getComplexBoxes: function(cellTileInfo) {
            var complexBoxes = [];
            var complexes = this.getComplesBoxesPrepared(cellTileInfo);

            for(var i = 0; i < complexes.length; i++) {
                var complexBox = complexes[i];

                for(var c = complexBox[0]; c <= complexBox[2]; c++) {
                    for(var r = complexBox[1]; r <= complexBox[3]; r++) {
                        if(!complexBoxes[c])
                            complexBoxes[c] = [];
                        complexBoxes[c][r] = complexBox;
                    }
                }
            }
            return complexBoxes;
        },
        getMergedCellRangesIntersectsRange: function(range) {
            var leftTopTilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), range.leftColIndex, range.topRowIndex);
            var rightBottomTilePosition = ASPxClientSpreadsheet.CellHelper.getTilePosition(this.getRenderProvider(), range.rightColIndex, range.bottomRowIndex);

            var leftTopCellPositionInTile = this.getCellPositionInTile(leftTopTilePosition, range.leftColIndex, range.topRowIndex);
            var rightBottomCellPositionInTile = this.getCellPositionInTile(rightBottomTilePosition, range.rightColIndex, range.bottomRowIndex);

            var matrix = this.getTileMatrix();

            var leftTopTileInfo = matrix.getGridTileInfo(leftTopTilePosition.row, leftTopTilePosition.col);
            var rightBottomTileInfo = matrix.getGridTileInfo(rightBottomTilePosition.row, rightBottomTilePosition.col);

            var complexBoxes = [];
            if(!leftTopTileInfo && !rightBottomTileInfo) return complexBoxes;

            this.scanComplexBoxCacheCol(matrix, leftTopTilePosition.col, leftTopCellPositionInTile.colIndex, leftTopTilePosition, rightBottomTilePosition, leftTopCellPositionInTile, rightBottomCellPositionInTile, complexBoxes);
            this.scanComplexBoxCacheCol(matrix, rightBottomTilePosition.col, rightBottomCellPositionInTile.colIndex, leftTopTilePosition, rightBottomTilePosition, leftTopCellPositionInTile, rightBottomCellPositionInTile, complexBoxes);
            this.scanComplexBoxCacheRow(matrix, leftTopTilePosition.row, leftTopCellPositionInTile.rowIndex, leftTopTilePosition, rightBottomTilePosition, leftTopCellPositionInTile, rightBottomCellPositionInTile, complexBoxes);
            this.scanComplexBoxCacheRow(matrix, rightBottomTilePosition.row, rightBottomCellPositionInTile.rowIndex, leftTopTilePosition, rightBottomTilePosition, leftTopCellPositionInTile, rightBottomCellPositionInTile, complexBoxes);

            return complexBoxes;
        },
        scanComplexBoxCacheCol: function(matrix, tileCol, colIndexInTile, leftTopTilePosition, rightBottomTilePosition, leftTopCellPositionInTile, rightBottomCellPositionInTile, complexBoxes) {
            var visibleTileCol = this.putTileColIndexInVisibleRange(tileCol);
            var tileColOffset = visibleTileCol * this.tileSize.col;

            for(var tileRow = leftTopTilePosition.row; tileRow <= rightBottomTilePosition.row; tileRow++) {
                var visibleTileRow = this.putTileRowIndexInVisibleRange(tileRow);

                var cellTileInfo = matrix.getGridTileInfo(visibleTileRow, visibleTileCol);
                var tileComplexBoxes = this.getTileCache_ComplexBoxes(cellTileInfo);
                if(tileComplexBoxes.length == 0) return;

                var tileRowOffset = visibleTileRow * this.tileSize.row;

                var tileComplexBoxLine = tileComplexBoxes[colIndexInTile];
                if(tileComplexBoxLine) {
                    var firstTile = visibleTileRow == leftTopTilePosition.row;
                    var lastTile = visibleTileRow == rightBottomTilePosition.row;
                    var beginR = firstTile ? leftTopCellPositionInTile.rowIndex : 0;
                    var endR = lastTile ? rightBottomCellPositionInTile.rowIndex : tileComplexBoxLine.length;

                    for(var r = beginR; r <= endR; r++) {
                        if(tileComplexBoxLine[r]) {
                            complexBoxes.push([
                                tileComplexBoxLine[r][0] + tileColOffset,
                                tileComplexBoxLine[r][1] + tileRowOffset,
                                tileComplexBoxLine[r][2] + tileColOffset,
                                tileComplexBoxLine[r][3] + tileRowOffset,
                            ]);

                            r = tileComplexBoxLine[r][3];
                            continue;
                        }
                    }
                }
            }
        },
        scanComplexBoxCacheRow: function(matrix, tileRow, rowIndexInTile, leftTopTilePosition, rightBottomTilePosition, leftTopCellPositionInTile, rightBottomCellPositionInTile, complexBoxes) {
            var visibleTileRow = this.putTileRowIndexInVisibleRange(tileRow);
            var tileRowOffset = visibleTileRow * this.tileSize.row;

            for(var tileCol = leftTopTilePosition.col; tileCol <= rightBottomTilePosition.col; tileCol++) {
                var visibleTileCol = this.putTileColIndexInVisibleRange(tileCol);

                var cellTileInfo = matrix.getGridTileInfo(visibleTileRow, visibleTileCol);
                var tileComplexBoxes = this.getTileCache_ComplexBoxes(cellTileInfo);

                if(tileComplexBoxes.length == 0) return;

                var tileColOffset = visibleTileCol * this.tileSize.col;

                var firstTile = visibleTileCol == leftTopTilePosition.col;
                var lastTile = visibleTileCol == rightBottomTilePosition.col;
                var beginCol = firstTile ? leftTopCellPositionInTile.colIndex : 0;
                var endCol = lastTile ? rightBottomCellPositionInTile.colIndex : this.tileSize.col;

                for(var c = beginCol; c <= endCol; c++) {
                    var tileComplexBoxLine = tileComplexBoxes[c];
                    if(tileComplexBoxLine && tileComplexBoxLine[rowIndexInTile]) {

                        complexBoxes.push([
                            tileComplexBoxLine[rowIndexInTile][0] + tileColOffset,
                            tileComplexBoxLine[rowIndexInTile][1] + tileRowOffset,
                            tileComplexBoxLine[rowIndexInTile][2] + tileColOffset,
                            tileComplexBoxLine[rowIndexInTile][3] + tileRowOffset,
                        ]);

                        c = tileComplexBoxLine[rowIndexInTile][2];
                        continue;
                    }
                }
            }
        },
        isInvalidComblexBox: function(complexBox) {
            for(var i = 0; i < complexBox.length; i++)
                if(complexBox[i] < 0) return true;
            return false;
        },
        resetCroppedComplexBoxes: function() {
            this.croppedComplexBoxes = [];
        },
        loadMergedCellRanges: function(tileInfo, mergedCells) {
            this.resetCellVisibleRange();

            var tileColOffset = tileInfo.colIndex * this.tileSize.col;
            var tileRowOffset = tileInfo.rowIndex * this.tileSize.row;
            var complexes = this.getComplesBoxesPrepared(tileInfo);

            for(var i = 0; i < complexes.length; i++) {
                var complexBox = complexes[i];
                if(this.isInvalidComblexBox(complexBox)) continue;

                var topLeftCellLayoutInfo = this.getCellLayoutInfo(
                    this.putCellColIndexInVisibleRange(complexBox[0] + tileColOffset),
                    this.putCellRowIndexInVisibleRange(complexBox[1] + tileRowOffset));

                var bottomRightCellLayoutInfo = this.getCellLayoutInfo(
                    this.putCellColIndexInVisibleRange(complexBox[2] + tileColOffset),
                    this.putCellRowIndexInVisibleRange(complexBox[3] + tileRowOffset));

                var rect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfo, bottomRightCellLayoutInfo, tileInfo);
                rect = this.correctRectForGridLine(rect);

                var element = document.createElement("DIV");
                element.className = "dxss-mc";
                ASPxClientSpreadsheet.RectHelper.setElementRect(element, rect);
                tileInfo.htmlElement.appendChild(element);
            }
        },
        isMergerCellContainsInCroppedRange: function(visibleCellPosition) {
            var croppedRangesLenght = this.croppedComplexBoxes.length;
            if(croppedRangesLenght > 0) {
                for(var i = 0; i < croppedRangesLenght; i++) {
                    var croppedRange = this.croppedComplexBoxes[i];
                    if(croppedRange.leftColIndex <= visibleCellPosition.colIndex &&
                        croppedRange.topRowIndex <= visibleCellPosition.rowIndex &&
                        croppedRange.rightColIndex >= visibleCellPosition.colIndex &&
                        croppedRange.bottomRowIndex >= visibleCellPosition.rowIndex)
                        return true;
                }
            }
            return false;
        },
        getLeftTopModelCellPositionInCroppedRange: function(visibleCellPosition) {
            var croppedRangesLenght = this.croppedComplexBoxes.length;
            if(croppedRangesLenght > 0) {
                for(var i = 0; i < croppedRangesLenght; i++) {
                    var croppedRange = this.croppedComplexBoxes[i];
                    if(croppedRange.leftColIndex <= visibleCellPosition.colIndex &&
                        croppedRange.topRowIndex <= visibleCellPosition.rowIndex &&
                        croppedRange.rightColIndex >= visibleCellPosition.colIndex &&
                        croppedRange.bottomRowIndex >= visibleCellPosition.rowIndex)
                        return { colIndex: croppedRange.leftColIndex, rowIndex: croppedRange.topRowIndex };
                }
            }
            return null;
        },

        // Header visible range
        getHeaderScrollBounds: function() { return this.headerScrollBounds; },
        getHeaderVisibleRange: function() { return this.headerVisibleRange; },
        setHeaderVisibleRange: function(range) {
            var changed = this.headerVisibleRange.top !== range.top || this.headerVisibleRange.right !== range.right || this.headerVisibleRange.bottom !== range.bottom || this.headerVisibleRange.left !== range.left;
            if(changed) {
                this.headerVisibleRange = range;
                this.onHeaderVisibleRangeChanged();
            }
        },
        onHeaderVisibleRangeChanged: function() {
            var headerVisibleRange = this.getHeaderVisibleRange();
            this.setLastVisibleHeaderRowTileIndex(headerVisibleRange.bottom);
            this.adjustControls();

            this.removeInvisibleTiles(headerVisibleRange, true);
            this.headerScrollBounds = this.calculateScrollBounds(headerVisibleRange);
        },

        loadGridTiles: function(visibleRange) {
            var tilInfo;
            for(var i = visibleRange.top; i <= visibleRange.bottom; i++) {
                for(var j = visibleRange.left; j <= visibleRange.right; j++) {
                    tilInfo = this.getGridTileInfo(i, j)
                    this.insertGridTile(tilInfo);
                }
            }
            if(ASPx.Browser.IE && ASPx.Browser.Version > 9)
                this.forceRedrawAppearance(this.getGridTilesContainer());
        },
        afterGridTilesLoaded: function(visibleRange) {
            this.resetCroppedComplexBoxes();

            var tilInfo;
            for(var i = visibleRange.top; i <= visibleRange.bottom; i++) {
                for(var j = visibleRange.left; j <= visibleRange.right; j++) {
                    tilInfo = this.getGridTileInfo(i, j)
                    this.loadMergedCellRanges(tilInfo, tilInfo.complexes);
                }
            }
        },
        
        resetCellVisibleRange: function() {
            this.cellVisibleRange = null;
        },

        createCellVisibleRange: function(visibleRange) {
            return {
                left: (visibleRange.left) * this.tileSize.col,
                top: (visibleRange.top) * this.tileSize.row,
                right: (visibleRange.right + 1) * this.tileSize.col - 1,
                bottom: (visibleRange.bottom + 1) * this.tileSize.row - 1
            };
        },
        getCellVisibleRange: function() {
            if(!this.cellVisibleRange)
                this.cellVisibleRange = this.createCellVisibleRange(this.getVisibleRange());
            return this.cellVisibleRange;
        },
        getCellHeaderVisibleRange: function() {
            if(!this.cellHeaderVisibleRange)
                this.cellHeaderVisibleRange = this.createCellVisibleRange(this.getHeaderVisibleRange());
            return this.cellHeaderVisibleRange;
        },
        putCellColIndexInVisibleRange: function(colIndex) {
            var cellVisibleRange = this.getCellVisibleRange();
            if(colIndex < cellVisibleRange.left) {
                colIndex = cellVisibleRange.left;
            }
            if(colIndex > cellVisibleRange.right) {
                colIndex = cellVisibleRange.right;
            }
            return colIndex;
        },
        putCellRowIndexInVisibleRange: function(rowIndex) {
            var cellVisibleRange = this.getCellVisibleRange();
            if(rowIndex < cellVisibleRange.top) {
                rowIndex = cellVisibleRange.top;
            }
            if(rowIndex > cellVisibleRange.bottom) {
                rowIndex = cellVisibleRange.bottom;
            }
            return rowIndex;
        },
        putTileColIndexInVisibleRange: function(colIndex) {
            if(colIndex < this.visibleRange.left) {
                colIndex = this.visibleRange.left;
            }
            if(colIndex > this.visibleRange.right) {
                colIndex = this.visibleRange.right;
            }
            return colIndex;
        },
        putTileRowIndexInVisibleRange: function(rowIndex) {
            if(rowIndex < this.visibleRange.top) {
                rowIndex = this.visibleRange.top;
            }
            if(rowIndex > this.visibleRange.bottom) {
                rowIndex = this.visibleRange.bottom;
            }
            return rowIndex;
        },
        
        correctRectForGridLine: function(rect) {
            var gridLineSize = 1;
            rect.x += gridLineSize;
            rect.y += gridLineSize;
            rect.width -= gridLineSize;
            rect.height -= gridLineSize;
            return rect;
        },
        loadHeaderTiles: function(newVisibleRange) {
            if(!newVisibleRange) {
                var viewBoundRange = this.calculateViewBoundRange(this.getScrollAnchorVisiblePosition(), this.getRenderProvider().getGridOffsetSize());
                if(!viewBoundRange) return; // WORKAROUND for IE9-

                newVisibleRange = this.calculateVisibleRange(viewBoundRange);
            }
            var matrix = this.getTileMatrix();
            if(this.getRowHeaderTilesContainer()) {
                for(var i = newVisibleRange.top; i <= newVisibleRange.bottom; i++) {
                    var headerInfo = matrix.getHeaderInfo(i, false);
                    if(!headerInfo) {
                        headerInfo = this.getDefaultHeaderInfo(i, false);
                        matrix.insertHeaderInfo(headerInfo, false);
                    }
                    this.insertHeaderTile(headerInfo, false);
                }
            }
            if(this.getColumnHeaderTilesContainer()) {
                for(var j = newVisibleRange.left; j <= newVisibleRange.right; j++) {
                    var headerInfo = matrix.getHeaderInfo(j, true);
                    if(!headerInfo) {
                        headerInfo = this.getDefaultHeaderInfo(j, true);
                        matrix.insertHeaderInfo(headerInfo, true);
                    }
                    this.insertHeaderTile(headerInfo, true);
                }
            }
            this.setHeaderVisibleRange(newVisibleRange);
        },
        getDefaultHeaderInfo: function(tileIndex, isCol) {
            var headerInfo = this.cloneDefaultHeaderInfo(tileIndex, isCol);
            headerInfo.index = tileIndex;
            headerInfo.htmlElement.id = this.getRenderProvider().getHeaderTileElementId(tileIndex, isCol, this.getPaneType());
            this.populateHeaderTextCells(headerInfo.htmlElement, headerInfo, isCol);
            return headerInfo;
        },
        cloneDefaultHeaderInfo: function(tileIndex, isCol) {
            var info = ASPx.CloneObject(this.getDefaultHeaderInfoTemplate(tileIndex, isCol));
            info.htmlElement = info.htmlElement.cloneNode(true);
            return info;
        },
        getDefaultHeaderInfoTemplate: function(tileIndex, isCol) {
            var headerInfo = isCol ? this.defaultColHeaderInfo : this.defaultRowHeaderInfo;
            if(!headerInfo) {
                headerInfo = this.createDefaultHeaderInfo(tileIndex, isCol);
                if(isCol)
                    this.defaultColHeaderInfo = headerInfo;
                else
                    this.defaultRowHeaderInfo = headerInfo;
            }
            return headerInfo;
        },
        createDefaultHeaderInfo: function(tileIndex, isCol) {
            var sizeName = isCol ? "widths" : "heights";
            var totalSizeName = isCol ? "totalWidth" : "totalHeight";
            var cellSize = isCol ? this.getDefaultCellSize().width : this.getDefaultCellSize().height;
            var tileSize = isCol ? this.tileSize.col : this.tileSize.row;
            var sizes = [];
            for(var i = 0; i < tileSize; i++)
                sizes.push(cellSize);
            var headerInfo = {};
            headerInfo[sizeName] = sizes;
            headerInfo[totalSizeName] = cellSize * tileSize;
            headerInfo.index = -1;
            headerInfo.htmlElement = this.createHeaderTemplate(tileIndex, isCol);
            this.assignHeaderContentSizes(headerInfo.htmlElement, headerInfo, isCol);
            return headerInfo;
        },
        getHeaderTileIsClientTemporal: function(headerInfo) {
            return !ASPx.IsExists(headerInfo.modelIndices);
        },

        // HighLight selection
        highlightHeaders: function(selection) {
            this.clearHeaderSelection();

            this.highlightHeadersCore(selection.range.leftColIndex, selection.range.rightColIndex, true);
            this.highlightHeadersCore(selection.range.topRowIndex, selection.range.bottomRowIndex, false)
        },
        clearHeaderSelection: function() {
            while(this.highlightHeadersArray.length > 0) {
                var headerCell = this.highlightHeadersArray.pop();
                ASPx.RemoveClassNameFromElement(headerCell, "dxss-hlh");
            }
        },
        highlightHeadersCore: function(startIndex, endIndex, isCol) {
            for(var i = startIndex; i <= endIndex; i++) {
                var headerCell = this.getHeaderCellByIndex(i, isCol);
                if(headerCell && !ASPx.ElementContainsCssClass(headerCell, "dxss-hlh")) {
                    ASPx.AddClassNameToElement(headerCell, "dxss-hlh");
                    this.highlightHeadersArray.push(headerCell);
                }
            }
        },
        getHeaderCellByIndex: function(index, isCol) {
            var columnContainer = isCol ? this.getRenderProvider().getColumnHeader(this.getPaneType()) : this.getRenderProvider().getRowHeader(this.getPaneType());
            if(columnContainer) {
                var headers = ASPx.GetNodesByClassName(columnContainer, ASPx.SpreadsheetCssClasses.HeaderContainer),
                    tileSize = isCol ? this.tileSize.col : this.tileSize.row,
                    tileIndex = Math.floor(index / tileSize),
                    cellContainer = this.getCellContainer(headers, tileIndex);
                if(cellContainer)
                    return cellContainer.childNodes[index - tileIndex * tileSize];
            }
            return null;
        },
        getCellContainer: function(headersArray, tileIndex) {
            for(var i in headersArray) {
                if(ASPx.Attr.GetAttribute(headersArray[i], ASPxClientSpreadsheet.TileHelper.DataAttributes.HeaderContainerTileIndex) == tileIndex)
                    return headersArray[i];
            }
            return null;
        },
        
        isSelectionVisible: function(selection) {
            if(selection.isAllSelected) return true;

            var cellVisibleRange = this.getCellVisibleRange();

            if(selection.range.bottomRowIndex < cellVisibleRange.top) return false;
            if(selection.range.topRowIndex > cellVisibleRange.bottom) return false;

            if(selection.range.rightColIndex < cellVisibleRange.left) return false;
            return selection.range.leftColIndex <= cellVisibleRange.right;
        },

        // Multy panes working
        cellInVisibleRange: function(cellModelPosition) {
            var cellVisibleRange = this.getCellVisibleRange();
            if(cellModelPosition.col >= cellVisibleRange.left && cellModelPosition.col <= cellVisibleRange.right &&
               cellModelPosition.row >= cellVisibleRange.top && cellModelPosition.row <= cellVisibleRange.bottom)
                return this;
            return null;
        },

        // TileMatrix worker API
        getTileTotalSize: function(index, isCol) {
            return this.getTileMatrix().getTileTotalSize(index, isCol);
        },
        getTileSizes: function(index, isCol) {
            return this.getTileMatrix().getTileSizes(index, isCol);
        },
        getTileIncrementalRanges: function(index, isCol) {
            return this.getTileMatrix().getTileIncrementalRanges(index, isCol);
        },
        getGridTileInfo: function(rowIndex, colIndex) {
            return this.getTileMatrix().getGridTileInfo(rowIndex, colIndex);
        },
        getHeaderInfo: function(index, isCol) {
            return this.getTileMatrix().getHeaderInfo(index, isCol);
        }
    });

    ASPxClientSpreadsheet.TileHelper.DataAttributes = {
        HeaderContainerTileIndex: "data-tile-index",
        HeaderCellIndex: "data-header-cell-index"
    }

    ASPxClientSpreadsheetLog = ASPx.CreateClass(null, { // INFO debug only
        constructor: function() {
            this.lockWrite = 0;
            this.cachedLog = "";
        },
        Write: function(text) {
            this.cachedLog += text + "\n";
            this.writeCore();
        },
        writeCore: function() {
            if(this.lockWrite === 0 && this.cachedLog !== "") {
                this.cachedLog = "";
            }
        },
        BeginUpdate: function() {
            this.lockWrite++;
        },
        EndUpdate: function() {
            this.lockWrite--;
            this.writeCore();
        }
    });
    
    ASPxClientSpreadsheet.Log = new ASPxClientSpreadsheetLog();
    
    function _aspxGetIncrementalRangeArray(source) {
        if(!source) return [];
        var result = [];
        var inc = 0;
        result.push(inc);
        for(var i = 0; i < source.length; i++) {
            inc += source[i];
            result.push(inc);
        }
        return result;
    }
    
    ASPxClientSpreadsheet.CellHelper = {
        getTilePosition: function(renderProvider, cellCol, cellRow) {
            return {
                col: Math.floor(cellCol / renderProvider.getTileSize().col),
                row: Math.floor(cellRow / renderProvider.getTileSize().row)
            }
        }
    }

    ASPxClientSpreadsheet.CellRenderHelper = (function() {
        function getSelectedData(spreadsheet, selection) {
            var activeCellElement = spreadsheet.getRenderProvider().getActiveCellElement();

            var selectionData = selection.range.singleCell ?
               getSingleSelectionData(spreadsheet, activeCellElement, selection) :
               getRangeSelectionData(spreadsheet, activeCellElement, selection);
            return selectionData;
        }

        function createDefaultSelectionData() {
            var selectionData = {
                selectedLink: null,
                selectedText: ""
            };
            return selectionData;
        }
        function getSingleSelectionData(spreadsheet, activeCellElement, selection) {
            var selectionData = createDefaultSelectionData();

            if(activeCellElement) {
                selectionData.selectedLink = getCellAnchorElement(activeCellElement);
                selectionData.selectedText = getCellText(activeCellElement);
            }

            return selectionData;
        }
        function getRangeSelectionData(spreadsheet, activeCellElement, selection) {
            var selectionData = createDefaultSelectionData();

            selectionData.selectedLink = findLinkElementInRangeSelection(spreadsheet, selection);
            selectionData.selectedText = getCellText(activeCellElement);
            return selectionData;
        }

        function findLinkElementInRangeSelection(spreadsheet, selection) {
            var linkElement;
            for(var i = selection.range.leftColIndex; i <= selection.range.rightColIndex; i++) {
                for(var k = selection.range.topRowIndex; k <= selection.range.bottomRowIndex; k++) {
                    var cellElement = spreadsheet.getRenderProvider().getCellElementByVisiblePosition(i, k);
                    if(cellElement) {
                        linkElement = getCellAnchorElement(cellElement);
                        if(linkElement)
                            return linkElement;
                    }
                }
            }
            return linkElement;
        }

        function getCellAnchorElement(cellElement) {
            return ASPx.GetNodeByTagName(cellElement, "a", 0);
        }
        function getCellTextElement(cellElement) {
            return ASPx.GetNodeByTagName(cellElement, "div", 0);
        }
        function getCellText(cellElement) {
            var cellTextElement = getCellTextElement(cellElement);
            return cellTextElement ? ASPx.GetInnerText(cellTextElement) : "";
        }

        return {
            getSelectedData: getSelectedData
        }
    })();

    ASPxClientSpreadsheet.RectHelper = (function() {

        function mergeRects(leftTopRect, rightBottomRect) {
            return {
                x: leftTopRect.x,
                y: leftTopRect.y,
                width: rightBottomRect.x + rightBottomRect.width - leftTopRect.x,
                height: rightBottomRect.y + rightBottomRect.height - leftTopRect.y
            }
        }

        function setElementRect(element, rect) {
            // INFO never use _aspxSetAbsolutePosition here!
            element.style.left = rect.x + "px";
            element.style.top = rect.y + "px";
            element.style.width = rect.width + "px";
            element.style.height = rect.height + "px";
        }
        function setElementRectWithElementOffset(element, rect, elementOffset) {
            element.style.width = rect.width + "px";
            element.style.height = rect.height + "px";

            var xOffset = ASPxClientUtils.GetAbsoluteX(elementOffset);
            var yOffset = ASPxClientUtils.GetAbsoluteY(elementOffset);

            if(ASPx.Browser.IE && ASPx.Browser.Version >= 10) {
                xOffset = Math.round(xOffset);
                yOffset = Math.round(yOffset);
            }

            ASPxClientUtils.SetAbsoluteX(element, rect.x + xOffset);
            ASPxClientUtils.SetAbsoluteY(element, rect.y + yOffset);
        }

        return {
            mergeRects: mergeRects,
            setElementRect: setElementRect,
            setElementRectWithElementOffset: setElementRectWithElementOffset
        };

    })();

    ASPxClientSpreadsheet.ElementPlacementHelper = (function() {
        function attachElementToTile(element, tileElement) {
            appendChildWithCheck(element, tileElement);
        }
        function attachElementToTileContainer(element, spreadsheetControl) {
            var gridContainer = spreadsheetControl.getRenderProvider().getGridTilesContainer();
            appendChildWithCheck(element, gridContainer);
        }
        function appendChildWithCheck(child, parent) {
            if(child.parentNode != parent) {
                if(child.parentNode)
                    child.parentNode.removeChild(child);
                parent.appendChild(child);
            }
        }

        return {
            attachElementToTile: attachElementToTile,
            attachElementToTileContainer: attachElementToTileContainer,
            appendChildWithCheck: appendChildWithCheck
        }
    })();

    ASPxClientSpreadsheet.TileHelper.getCellRangeRect = function(topLeftCellLayoutInfo, bottomRightCellLayoutInfo, parentTileInfo) {
        var relativeOffsetLeftTopCornerX = ASPx.GetAbsoluteX(bottomRightCellLayoutInfo.tileInfo.htmlElement) - ASPx.GetAbsoluteX(parentTileInfo.htmlElement);
        var relativeOffsetLeftTopCornerY = ASPx.GetAbsoluteY(bottomRightCellLayoutInfo.tileInfo.htmlElement) - ASPx.GetAbsoluteY(parentTileInfo.htmlElement);

        var relativeOffsetRightBottomCornerX = ASPx.GetAbsoluteX(topLeftCellLayoutInfo.tileInfo.htmlElement) - ASPx.GetAbsoluteX(parentTileInfo.htmlElement);
        var relativeOffsetRightBottomCornerY = ASPx.GetAbsoluteY(topLeftCellLayoutInfo.tileInfo.htmlElement) - ASPx.GetAbsoluteY(parentTileInfo.htmlElement);

        var bottomRightCellRect = bottomRightCellLayoutInfo.rect;
        bottomRightCellRect.x += relativeOffsetLeftTopCornerX;
        bottomRightCellRect.y += relativeOffsetLeftTopCornerY;

        var topLeftCellRect = topLeftCellLayoutInfo.rect;
        topLeftCellRect.x += relativeOffsetRightBottomCornerX;
        topLeftCellRect.y += relativeOffsetRightBottomCornerY;

        return ASPxClientSpreadsheet.RectHelper.mergeRects(topLeftCellRect, bottomRightCellRect);
    }

    ASPxClientSpreadsheet.TileHelper.CellLayoutInfo = function(tileInfo, colVisibleIndex, rowVisibleIndex, colModelIndex, rowModelIndex, rect, paneType) {
        this.tileInfo = tileInfo;
        this.colIndex = colVisibleIndex;
        this.rowIndex = rowVisibleIndex;
        this.colModelIndex = colModelIndex;
        this.rowModelIndex = rowModelIndex;
        this.rect = rect;
        this.paneType = paneType

        validate(this);

        this.assignFrom = function(cellLayoutInfo) {
            this.tileInfo = cellLayoutInfo.tileInfo;
            this.colIndex = cellLayoutInfo.colIndex;
            this.rowIndex = cellLayoutInfo.rowIndex;
            this.colModelIndex = cellLayoutInfo.colModelIndex;
            this.rowModelIndex = cellLayoutInfo.rowModelIndex;
            this.rect = cellLayoutInfo.rect;
            this.paneType = cellLayoutInfo.paneType;
            validate(this);
        }

        function validate(cellLayoutInfo) {
            cellLayoutInfo.valid = cellLayoutInfo.colIndex >= 0 && cellLayoutInfo.rowIndex >= 0;
        }
    }

    ASPxClientSpreadsheet.ProtectionResolver = (function() {
        function cellLocked_ByLayoutInfoCore(sheetLocked, cellLayoutInfo) {
            if(!sheetLocked) return false;

            if(argumentIsCellayoutInfo(cellLayoutInfo)) {
                var cellEditable = cellLayoutInfo.tileInfo.cellEditable;
                return !cellEditPermitted(cellEditable, cellLayoutInfo.colModelIndex, cellLayoutInfo.rowModelIndex);
            }
            return false;
        }
        function cellLocked_ByModelIndicesCore(tileHelper, sheetLocked, colModelIndex, rowModelIndex) {
            if(!sheetLocked) return false;

            var cellLayoutInfo = tileHelper.getCellLayoutInfo_ByModelIndices(colModelIndex, rowModelIndex);
            return cellLocked_ByLayoutInfoCore(cellLayoutInfo);
        }

        function cellEditPermitted(cellEditable, colModelIndex, rowModelIndex) {
            var noPermission = !cellEditable;
            if(noPermission) return false;

            var cellModelPosition = [colModelIndex, rowModelIndex];
            var comparer = function(el1, el2) { return el1[0] == el2[0] && el1[1] == el2[1]; };
            return ASPx.Data.ArrayIndexOf(cellEditable, cellModelPosition, comparer) > -1;
        }
        function argumentIsCellayoutInfo(cellLayoutInfo) {
            return cellLayoutInfo && cellLayoutInfo.tileInfo;
        }

        return {
            cellLocked_ByLayoutInfo: function(sheetLocked, cellLayoutInfo) {
                return cellLocked_ByLayoutInfoCore(sheetLocked, cellLayoutInfo);
            },
            cellLocked_ByModelIndices: function(tileHelper, sheetLocked, colModelIndex, rowModelIndex) {
                return cellLocked_ByModelIndicesCore(tileHelper, sheetLocked, colModelIndex, rowModelIndex);
            }
        }
    })();

    ASPxClientSpreadsheet.StylesheetManager = function() {
        this.rules = {};
        this.styleSheet = null;
                
        this.update = function() {
            if(this.styleSheet)
                ASPx.RemoveElement(this.styleSheet);
            var styleArgs = [];
            for(var key in this.rules) {
                var controlRules = this.rules[key];
                for(var controlRuleKey in controlRules) {
                    var styleRules = controlRules[controlRuleKey];
                    for(var i = 0; i < styleRules.length; i++) {
                        var rule = styleRules[i];
                        styleArgs.push(rule.selector + " { " + rule.cssText + " } ");
                    }
                }
            }
            this.styleSheet = this.createStyleSheet(styleArgs.join(""));
        };
        this.createStyleSheet = function(cssText) {
            var container = document.createElement("DIV");
            ASPx.SetInnerHtml(container, "<style type='text/css'>" + cssText + "</style>");

            styleSheet = ASPx.GetNodeByTagName(container, "style", 0);
            if(styleSheet)
                ASPx.GetNodeByTagName(document, "HEAD", 0).appendChild(styleSheet);
            return styleSheet;
        };
    };

    ASPxClientSpreadsheet.CellPositionConvertor = (function() {
        function getStringRepresentaion(modelIndex) {
            if(modelIndex < 0) return "";
            var letter = String.fromCharCode(65 + modelIndex % 26);
            modelIndex = Math.floor(modelIndex / 26) - 1;
            return letter + this.getStringRepresentaion(modelIndex);
        }
        function getCellModelPositionByStringRepresentation(cellName) {
            var rowIndex = -1,
                colIndex = -1;

            var row = /(\$)?(\d+)/.exec(cellName);
            if(row && row.length > 0)
                rowIndex = parseInt(row[0]) - 1;

            var col = /(\$)?([A-Z]+)/.exec(cellName.toUpperCase());
            if(col && col.length > 0)
                colIndex = getColumnIndex(col[0]);

            return { col: colIndex, row: rowIndex };
        }
        function getColumnIndex(columnName) {
            var result = 0;

            for(var i = 0; i < columnName.length; i++) {
                result += columnName.charCodeAt(i) - 64 + result * 25;
            }
            return result - 1;
        }
        return {
            getStringRepresentaion: getStringRepresentaion,
            getCellModelPositionByStringRepresentation: getCellModelPositionByStringRepresentation
        };
    })();

    ASPxClientSpreadsheet.StylesheetManager.Instance = new ASPxClientSpreadsheet.StylesheetManager();
})();