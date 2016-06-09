(function() {
    ASPxClientSpreadsheet.GridResizingHelper = ASPx.CreateClass(null, {
        constructor: function(spreadsheet) {
            this.spreadsheet = spreadsheet;
            this.tileSize = this.spreadsheet.getTileSize();

            this.resizeGrip = null;
            this.gripSize = 1;
            this.maximumOffset = ASPx.Browser.TouchUI ? 10 : 3;
            this.savedScrollAnchor = null;
            this.headerWidthRanges = {};
            this.headerHeightRanges = {};
        },

        // DOM containers and Helpers
        getRenderProvider: function() { return this.spreadsheet.getRenderProvider(); },
        getPaneManager: function() { return this.spreadsheet.getPaneManager(); },
        getColumnHeader: function(paneType) { return this.getRenderProvider().getColumnHeader(paneType); },
        getRowHeader: function(paneType) { return this.getRenderProvider().getRowHeader(paneType); },
        getHeadersContainer: function(isCol, paneType) { return isCol ? this.getColumnHeader(paneType) : this.getRowHeader(paneType); },

        getEventControlPoint: function(e, exceptHeaders, paneType) {
            var point = this.spreadsheet.getEventControlPoint(e);
            if(exceptHeaders) {
                var headerOffsetSize = this.getPaneManager().getHeaderOffsetSizeForMousePointer(paneType);
                point.x -= headerOffsetSize.width;
                point.y -= headerOffsetSize.height;
            }
            return point;
        },

        getScrollAnchorVisiblePosition: function() { return this.getPaneManager().getScrollAnchorVisiblePosition(); },
        
        // Resizing flags
        update: function() {
            this.lockResizing = false;
            this.headerWidthRanges = {};
            this.headerHeightRanges = {};

        },
        isResizing: function() { return ASPxClientSpreadsheet.activeResizingHelper === this; },
        canStartResizing: function(e, isCol, paneType) {
            return this.getResizeInfoByMouseEvent(e, isCol, paneType).canResize;
        },

        // Header Events
        onMouseDown: function(e, isCol, paneType) {
            if(this.lockResizing) return;
            var resizeInfo = this.getResizeInfoByMouseEvent(e, isCol, paneType);
            if(!resizeInfo.canResize)
                return;
            this.startResizing(e, isCol, resizeInfo.indexInHeaderRange, paneType);
            this.resize(e, isCol, true);
            this.spreadsheet.getStateController().finializeProcessing();
            ASPx.Evt.PreventEventAndBubble(e);
        },
        onMouseMove: function(e, isCol, paneType) {
            if(this.lockResizing) return;
            if(!this.isResizing()) {
                this.updateCursor(this.canStartResizing(e, isCol, paneType), isCol, paneType);
                return;
            }
            if(ASPx.Browser.TouchUI)
                e.preventDefault();
            ASPx.Selection.Clear();
            this.resize(e, this.headerResizeInfo.isCol);
        },
        onMouseUp: function(e) {
            this.endResizing(e);
        },

        onDoubleClick: function(e, isCol, paneType) {
            var resizeInfo = this.getResizeInfoByMouseEvent(e, isCol, paneType),
                cellIndexInHeaderRange = resizeInfo.indexInHeaderRange,
                headerResizeInfo = this.getHeaderResizeInfo(cellIndexInHeaderRange, isCol, paneType);

            index = this.getPaneManager().convertVisibleIndexToModelIndex(headerResizeInfo.cellIndex, isCol);
            this.spreadsheet.autoFitHeaderSize(isCol, index);
            ASPx.Evt.PreventEventAndBubble(e);
        },

        // Header actions
        startResizing: function(e, isCol, cellIndexInHeaderRange, paneType) {
            var point = this.getEventControlPoint(e, false, paneType);
            this.initialPos = isCol ? point.x : point.y;
            this.headerResizeInfo = this.getHeaderResizeInfo(cellIndexInHeaderRange, isCol, paneType);

            var headerOffsetSize = this.getPaneManager().getHeaderOffsetSizeForMousePointer(paneType);
            var gridOffsetSize = this.getRenderProvider().getGridOffsetSize(paneType);
            var headerOffset = isCol ? headerOffsetSize.width : headerOffsetSize.height;
            var gridOffset = isCol ? gridOffsetSize.width : gridOffsetSize.height;

            var sizeRanges = isCol ? this.headerWidthRanges[paneType] : this.headerHeightRanges[paneType];
            var gripCorrection = Math.floor(this.gripSize / 2);
            this.minPos = sizeRanges[cellIndexInHeaderRange] + headerOffset - gripCorrection;
            this.maxPos = gridOffset + headerOffset - gripCorrection; // TODO get max value from MODEL (DocumentModel.UnitConverter.TwipsToModelUnits(MaxHeightInTwips) for row and 255 for column)

            ASPxClientSpreadsheet.activeResizingHelper = this;
            ASPx.Selection.SetElementSelectionEnabled(document.body, false);

            this.changeResizeGripVisibility(isCol, true);
        },
        resize: function(e, isCol) {
            var point = this.getEventControlPoint(e, false),
                pos = isCol ? point.x : point.y,
                headerOffsetSize = this.getPaneManager().getHeaderOffsetSize(),
                resizeGrip = this.getResizeGrip();

            if(this.savedPos && this.savedPos === pos)
                return;
            if(pos < this.minPos || pos > this.maxPos) {
                if(this.savedPos === this.minPos || this.savedPos === this.maxPos)
                    return;
                pos = pos < this.minPos ? this.minPos : this.maxPos;
            }
            ASPx.SetStyles(resizeGrip, {
                left: isCol ? pos + "px" : headerOffsetSize.width,
                top: !isCol ? pos + "px" : headerOffsetSize.height
            });

            var movedHeaderElementsInfo = this.headerResizeInfo.movedHeaderElementsInfo;
            var diff = pos - this.initialPos;

            var sizeStyleName = isCol ? "width" : "height";
            var cellNewSize = this.headerResizeInfo.cellSize + diff;
            var headerNewSize = this.headerResizeInfo.headerTotalSize + diff;
            movedHeaderElementsInfo.resizedTextCell.style[sizeStyleName] = cellNewSize + "px";
            movedHeaderElementsInfo.headerElement.style[sizeStyleName] = headerNewSize + "px";

            var posStyleName = isCol ? "left" : "top";
            movedHeaderElementsInfo.gridLinesContainer.style[posStyleName] = diff + "px";
            movedHeaderElementsInfo.textCellsContainer.style[posStyleName] = diff + "px";
            this.savedPos = pos;
        },
        endResizing: function(e) {
            var isCol = this.headerResizeInfo.isCol;
            var point = this.getEventControlPoint(e, false);
            var pos = isCol ? point.x : point.y;
            if(pos < this.minPos)
                pos = this.minPos;
            if(pos > this.maxPos)
                pos = this.maxPos;
            var diff = Math.round(pos - this.initialPos);
            if(diff !== 0) {
                var visibleIndex = this.headerResizeInfo.cellIndex;
                var index = this.getPaneManager().convertVisibleIndexToModelIndex(visibleIndex, isCol);
                var size = this.headerResizeInfo.cellSize + diff;
                var resizeMethodName = isCol ? "resizeColumn" : "resizeRow";
                this.spreadsheet[resizeMethodName](index, size);
            } else
                this.restoreMovedElements(this.headerResizeInfo);
            this.changeResizeGripVisibility(this.headerResizeInfo.isCol, false);
            this.updateCursor(false, isCol, this.headerResizeInfo.paneType);
            ASPx.Selection.SetElementSelectionEnabled(document.body, true);

            this.initialPos = -1;
            this.minPos = -1;
            this.maxPos = -1;
            this.headerResizeInfo = null;
            ASPxClientSpreadsheet.activeResizingHelper = null;
            this.lockResizing = diff !== 0;
        },
        
        // Resizing Grip
        changeResizeGripVisibility: function(isCol, visible) {
            var resizeGrip = this.getResizeGrip();
            if(visible) {
                var gridOffsetSize = this.getPaneManager().getVisibleAreaSize(),
                    headerOffsetSize = this.getPaneManager().getHeaderOffsetSize(),
                    width = isCol ? this.gripSize : gridOffsetSize.width - headerOffsetSize.width,
                    height = isCol ? gridOffsetSize.height - headerOffsetSize.height : this.gripSize;

                ASPx.SetStyles(resizeGrip, {
                    width: width,
                    height: height,
                    cursor: this.getCursorValue(isCol)
                });
                resizeGrip.className = isCol ? "dxss-vrg" : "dxss-hrg";
            }
            if(ASPx.Browser.TouchUI && this.spreadsheet.touchResizeElemnt) {
                var headerCellRightOrBottomBorder = this.spreadsheet.touchResizeElemnt.parentNode;
                ASPx.SetElementDisplay(headerCellRightOrBottomBorder, !visible);
            }
            ASPx.SetElementDisplay(resizeGrip, visible);
        },
        getResizeGrip: function() {
            var id = this.getResizeGripID();
            var resizeGrip = ASPx.GetElementById(id);
            if(!resizeGrip) {
                resizeGrip = document.createElement("DIV");
                resizeGrip.id = id;
                this.getRenderProvider().getWorkbookControl().appendChild(resizeGrip);
            }
            return resizeGrip;
        },
        getResizeGripID: function() { return this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.resizeGrip; },
        getCursorValue: function(isCol) {
            return isCol ? "ew-resize" : "ns-resize";
        },

        // Touch device support
        getTouchResizeElement: function() {
            if(!this.spreadsheet.touchResizeElemnt) {
                this.spreadsheet.touchResizeElemnt = this.createTouchResizeElemnt();
            }
            return this.spreadsheet.touchResizeElemnt;
        },
        getIsTouchResizeElement: function(element) {
            return ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.TouchResizeElement);
        },
        createTouchResizeElemnt: function() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.TouchResizeElement;
            return element;
        },
        showTouchResizeElement: function(cellIndex, isCol) {
            var tileSize = isCol ? this.tileSize.col : this.tileSize.row;
            var lastLineIndex = tileSize - 1;
            var tileIndex = Math.floor(cellIndex / tileSize);
            var lineIndexInTile = cellIndex % tileSize + 1;
            if(lineIndexInTile > lastLineIndex) {
                tileIndex++;
                lineIndexInTile = 0;
            }
            var headerTile = this.getRenderProvider().getHeaderTileElementByCellIndex(cellIndex, tileIndex, isCol);
            if(!headerTile)
                return;
            var lines = ASPx.GetNodesByClassName(headerTile, ASPx.SpreadsheetCssClasses.getGridLine(isCol));
            var touchResizeDiv = lines[lineIndexInTile];
            var touchResizeElement = this.getTouchResizeElement();
            touchResizeDiv.appendChild(touchResizeElement);
            ASPx.SetElementDisplay(touchResizeElement, true);
        },
        hideTouchResizeElement: function() {
            if(this.spreadsheet.touchResizeElemnt)
                ASPx.SetElementDisplay(this.spreadsheet.touchResizeElemnt, false);
        },

        // Helpers
        getHeaderResizeInfo: function(indexInHeaderRange, isCol, paneType) {
            var paneManager = this.getPaneManager(),
                pane = paneManager.getPaneByType(paneType),
                scrollAnchor = paneManager.getPaneTopLeftCellVisiblePosition(paneType);

            var cellIndex = indexInHeaderRange + (isCol ? scrollAnchor.col : scrollAnchor.row);
            var tileSize = isCol ? this.tileSize.col : this.tileSize.row;
            var tileIndex = Math.floor(cellIndex / tileSize);
            var cellIndexInTile = cellIndex - tileIndex * tileSize;
            var cellSize = pane.getTileSizes(tileIndex, isCol)[cellIndexInTile];
            var cellPosInTile = pane.getTileIncrementalRanges(tileIndex, isCol)[cellIndexInTile];
            var headerTotalSize = pane.getTileTotalSize(tileIndex, isCol);
            var movedHeaderElementsInfo = this.getMovedHeaderElementsInfo(tileIndex, cellIndexInTile, isCol, paneType);

            return {
                isCol: isCol,
                cellIndex: cellIndex,
                tileIndex: tileIndex,
                cellIndexInTile: cellIndexInTile,
                cellSize: cellSize,
                headerTotalSize: headerTotalSize,
                movedHeaderElementsInfo: movedHeaderElementsInfo,
                paneType: paneType
            };
        },
        getMovedHeaderElementsInfo: function(tileIndex, cellIndexInTile, isCol, paneType) {
            var headerElement = this.getRenderProvider().getHeaderTileElement(tileIndex, isCol, paneType),
                gridLinesContainer = ASPx.GetChildByClassName(headerElement, "dxss-gl"),
                movedLinesContainer = document.createElement("DIV");

            movedLinesContainer.className = "dxss-rmc";
            gridLinesContainer.appendChild(movedLinesContainer);

            var lines = ASPx.GetChildNodesByClassName(gridLinesContainer, ASPx.SpreadsheetCssClasses.getGridLine(isCol));
            for(var i = cellIndexInTile + 1; i < lines.length; i++) {
                var line = lines[i];
                ASPx.RemoveElement(line);
                movedLinesContainer.appendChild(line);
            }

            var textCellsContainer = ASPx.GetChildNodesByClassName(headerElement, ASPx.SpreadsheetCssClasses.HeaderContainer)[0]; // TODO refactor it
            var movedTextCellsContainer = document.createElement("DIV");
            movedTextCellsContainer.className = "dxss-rmc";
            textCellsContainer.appendChild(movedTextCellsContainer);

            var textCells = ASPx.GetChildNodesByClassName(textCellsContainer, ASPx.SpreadsheetCssClasses.HeaderCell);
            for(var i = cellIndexInTile + 1; i < textCells.length; i++) {
                var textCell = textCells[i];
                ASPx.RemoveElement(textCell);
                movedTextCellsContainer.appendChild(textCell);
            }
            return {
                resizedTextCell: textCells[cellIndexInTile],
                gridLinesContainer: movedLinesContainer,
                textCellsContainer: movedTextCellsContainer,
                headerElement: headerElement
            };
        },
        restoreMovedElements: function(headerResizeInfo, isCol) {
            this.restoreGridLinesContainer(headerResizeInfo);
            this.restoreTextCellsContainer(headerResizeInfo);
        },
        restoreGridLinesContainer: function(headerResizeInfo) {
            var movedHeaderElementsInfo = headerResizeInfo.movedHeaderElementsInfo;
            var gridLinesContainer = ASPx.GetChildByClassName(movedHeaderElementsInfo.headerElement, "dxss-gl");
            var lines = ASPx.GetChildNodesByClassName(movedHeaderElementsInfo.gridLinesContainer, ASPx.SpreadsheetCssClasses.getGridLine(headerResizeInfo.isCol));
            for(var i = 0; i < lines.length; i++) {
                var line = lines[i];
                gridLinesContainer.appendChild(line);
            }
            ASPx.RemoveElement(movedHeaderElementsInfo.gridLinesContainer);
        },
        restoreTextCellsContainer: function(headerResizeInfo) {
            var movedHeaderElementsInfo = headerResizeInfo.movedHeaderElementsInfo;
            var textCellsContainer = ASPx.GetChildByClassName(movedHeaderElementsInfo.headerElement, ASPx.SpreadsheetCssClasses.HeaderContainer);
            var textCells = ASPx.GetChildNodesByClassName(movedHeaderElementsInfo.textCellsContainer, ASPx.SpreadsheetCssClasses.HeaderCell);
            for(var i = 0; i < textCells.length; i++) {
                var textCell = textCells[i];
                textCellsContainer.appendChild(textCell);
            }
            ASPx.RemoveElement(movedHeaderElementsInfo.textCellsContainer);
        },
        getResizeInfoByMouseEvent: function(e, isCol, paneType) {
            this.ensureCachedHeaderSizes();

            var coordValue = this.getResizeCoordinate(e, isCol, paneType);

            var sizeRanges = isCol ? this.headerWidthRanges[paneType] : this.headerHeightRanges[paneType];
            var indexInRange = ASPx.Data.ArrayBinarySearch(sizeRanges, coordValue, ASPx.Data.NearestLeftBinarySearchComparer);
            var indexFallInRange = indexInRange >= 0;

            var leftBound = sizeRanges[indexInRange] + this.maximumOffset;
            var rightBound = sizeRanges[indexInRange + 1] - this.maximumOffset - 1;

            var satisfyLeftBound = coordValue <= leftBound && indexInRange > 0;
            var satisfyRightBound = coordValue >= rightBound;

            var canResize = ASPx.Browser.TouchUI ? this.getIsTouchResizeElement(ASPx.Evt.GetEventSource(e))
                : indexFallInRange && (satisfyLeftBound || satisfyRightBound);
            return {
                canResize: canResize,
                indexInHeaderRange: coordValue >= rightBound ? indexInRange : indexInRange - 1
            }
        },
        getResizeCoordinate: function(e, isCol, paneType) {
            var point = this.getEventControlPoint(e, true, paneType);
            return isCol ? point.x : point.y;
        },
        ensureCachedHeaderSizes: function() {
            var scrollAnchor = this.getScrollAnchorVisiblePosition();
            if(this.savedScrollAnchor && this.savedScrollAnchor.col === scrollAnchor.col && this.savedScrollAnchor.row === scrollAnchor.row &&
                this.headerWidthRanges[ASPxClientSpreadsheet.PaneManager.PanesType.MainPane] && this.headerWidthRanges[ASPxClientSpreadsheet.PaneManager.PanesType.MainPane].length > 0 &&
                this.headerHeightRanges[ASPxClientSpreadsheet.PaneManager.PanesType.MainPane].length > 0)
                return;

            this.updateHeaderRanges();
        },
        updateHeaderRanges: function() {
            var viewBounds = this.getPaneManager().calculateHeaderViewBounds();
            for(var paneType in viewBounds) {
                if(viewBounds.hasOwnProperty(paneType) && viewBounds[paneType]) {
                    var viewBound = viewBounds[paneType];
                    this.headerWidthRanges[paneType] = this.calculateHeaderSizeRanges(viewBound, true, paneType);
                    this.headerHeightRanges[paneType] = this.calculateHeaderSizeRanges(viewBound, false, paneType);
                }
            }
            this.savedScrollAnchor = this.getScrollAnchorVisiblePosition();
        },
        calculateHeaderSizeRanges: function(viewBound, isCol, paneType) {
            var pane = this.getPaneManager().getPaneByType(paneType),
                startCellIndex = isCol ? viewBound.left : viewBound.top,
                endCellIndex = isCol ? viewBound.right : viewBound.bottom,
                tileSize = isCol ? this.tileSize.col : this.tileSize.row;

            var startTileIndex = Math.floor(startCellIndex / tileSize),
                endTileIndex = Math.floor(endCellIndex / tileSize);

            var startCellIndexInTile = startCellIndex - startTileIndex * tileSize,
                endCellIndexInTile = endCellIndex - endTileIndex * tileSize;

            var resultRanges = [];
            var totalSize = 0;
            resultRanges.push(totalSize);
            for(var i = startTileIndex; i <= endTileIndex; i++) {
                var start = i === startTileIndex ? startCellIndexInTile : 0,
                    end = i === endTileIndex ? endCellIndexInTile : tileSize - 1,
                    tileSizeArray = pane.getTileSizes(i, isCol);
                if(!tileSizeArray)
                    continue;
                var tileSizes = tileSizeArray.slice(start, end + 1);
                for(var j = 0; j < tileSizes.length; j++) {
                    totalSize += tileSizes[j];
                    resultRanges.push(totalSize);
                }
            }
            return resultRanges;
        },
        updateCursor: function(canResize, isCol, paneType) {
            var headerContainer = this.getHeadersContainer(isCol, paneType);
            var prevCanResize = headerContainer.dxPrevCanResize;
            if(!!prevCanResize === canResize)
                return;
            headerContainer.dxPrevCanResize = canResize;

            if(canResize)
                ASPx.AddClassNameToElement(headerContainer, "resizing");
            else
                ASPx.RemoveClassNameFromElement(headerContainer, "resizing");
        },
        generateRandomClassName: function() {
            return "dxss" + Math.floor((Math.random() + 1) * 100000).toString(36);
        }
    });

    ASPxClientSpreadsheet.activeResizingHelper = null;
    ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseMoveEventName, function(e) {
        if(ASPxClientSpreadsheet.activeResizingHelper != null && !(ASPx.Browser.WebKitTouchUI && ASPx.TouchUIHelper.isGesture)) {
            ASPxClientSpreadsheet.activeResizingHelper.onMouseMove(e);
            return true;
        }
    });
})();