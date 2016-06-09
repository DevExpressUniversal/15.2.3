(function() {
    ASPxClientSpreadsheet.PaneManager = ASPx.CreateClass(null, {

        constructor: function(spreadsheetControl) {
            this.spreadsheetControl = spreadsheetControl;
            this.renderProvider = this.spreadsheetControl.getRenderProvider();
            this.activePanes = {};
            this.headerOffsetHeight = 0;
            this.headerOffsetWidths = [];
            this.frozenPaneSettings = { isFrozen: false, width: 0, height: 0 };
        },
        getHeaderOffsetSize: function() {
            var rowIndex = (this.getLastVisibleHeaderRowTileIndex() + 1) * this.getTileSize().row;
            return {
                width: this.headerOffsetWidths[rowIndex.toString().length - 1],
                height: this.headerOffsetHeight
            };
        },
        setHeaderOffsetSizes: function(response) {
            if(response.headerOffsetHeight) {
                this.headerOffsetHeight = response.headerOffsetHeight;
                this.headerOffsetWidths = response.headerOffsetWidths;
            }
        },
        getLastVisibleHeaderRowTileIndex: function() {
            var maxVisibleIndex = 0;
            for(var paneType in this.activePanes)
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType])
                    maxVisibleIndex = Math.max(this.activePanes[paneType].getLastVisibleHeaderRowTileIndex(), maxVisibleIndex);
            return maxVisibleIndex;
        },

        updateGridTileCache: function(response) {
            if(response.clearGridCache) {
                for(var paneType in this.activePanes)
                    if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                        var pane = this.activePanes[paneType],
                            serverVisibleRange = response.visibleRange[paneType];
                        pane.updateGridTileCache(serverVisibleRange);
                    }
            }
        },
        processDocumentResponse: function(response) {
            this.preparePaneManager(response);
            this.getScrollHelper().processDocumentResponse(response);
            if(response.autoFilters)
                this.renderAutoFilterImages(response.autoFilters)
        },
        preparePaneManager: function(response) {
            this.getRenderProvider().hideFreezeBorders();

            this.setHeaderOffsetSizes(response);
            this.applyFrozenPaneSettings(response);
            this.calculatePaneCellVisibleRange();

            this.updateVisibleRanges(response.visibleRange);

            this.adjustVisiblePanes();

            this.getRenderProvider().showFreezeBorders(this.getFrozenPaneSettings());

            for(var paneType in response.newTiles) {
                if(response.newTiles.hasOwnProperty(paneType) && response.newTiles[paneType]) {
                    var pane = this.getPaneByType(paneType),
                        newTiles = response.newTiles[paneType],
                        visibleRange = response.visibleRange[paneType],
                        outdatedTiles = response.outdatedTiles;
                    pane.processDocumentResponse(newTiles, visibleRange, outdatedTiles);
                }
            }
        },
        renderAutoFilterImages: function(autoFiltersConfig) {
            ASPx.Data.ForEach(autoFiltersConfig, function(rawConfig) {
                var autoFilterConfig = this.formatAutoFilterConfig(rawConfig);
                if(autoFilterConfig.imageName !== "None")
                    this.renderAutoFilterImage(autoFilterConfig);
            }.aspxBind(this));
        },
        renderAutoFilterImage: function(autoFilterConfig) {
            var cell = { col: autoFilterConfig.colIndex, row: autoFilterConfig.rowIndex },
                pane = this.getPaneByCell(cell);

            pane.renderAutoFilterImage(autoFilterConfig);
        },
        formatAutoFilterConfig: function(rawConfig) {
            return {
                colIndex: rawConfig[0],
                rowIndex: rawConfig[1],
                columnType: rawConfig[2],
                imageName: rawConfig[3],
                isDefault: rawConfig[4]
            };
        },
        applyFrozenPaneSettings: function(response) {
            var frozenModeChanged = !this.frozePaneSettingsAreEqual(this.getFrozenPaneSettings(), response.fp);
            if(frozenModeChanged || this.getOwnerControl().isNewSheetLoaded(response)) {
                this.setFrozenPaneSettings(response.fp);
                this.actualizeDOMElements(response.newTiles, response.visibleRange);
                this.getScrollHelper().onFrozenPaneSettingsChanged();
            }
        },
        getFrozenPaneSettings: function() {
            return this.frozenPaneSettings;
        },
        setFrozenPaneSettings: function(settings) {
            if(settings) {
                this.frozenPaneSettings = settings;
                this.frozenPaneSettings.isFrozen = !!settings.frozenCell;
            } else this.frozenPaneSettings = { isFrozen: false, width: 0, height: 0 };
        },
        frozePaneSettingsAreEqual: function(oldSettings, newSettings) {
            var changed = newSettings && !oldSettings || !newSettings && oldSettings;
            if(!changed && oldSettings && newSettings)
                changed = !(oldSettings.mode === newSettings.mode &&
                    oldSettings.topLeftCell.col === newSettings.topLeftCell.col &&
                    oldSettings.topLeftCell.row === newSettings.topLeftCell.row &&
                    oldSettings.frozenCell.col === newSettings.frozenCell.col &&
                    oldSettings.frozenCell.row === newSettings.frozenCell.row &&
                    oldSettings.width === newSettings.width &&
                    oldSettings.height === newSettings.height);
            return !changed;
        },
        actualizeDOMElements: function(newTiles) {
            var panes = [ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane, ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane, ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane],
                len = panes.length,
                paneType;
            for(var i = 0; i < len, paneType = panes[i]; i++) {
                if(newTiles[paneType]) {
                    var pane = this.getPaneByType(paneType);
                    if(!pane) {
                        pane = this.createPaneByType(paneType);
                        this.attachEventsToPaneElements(paneType);
                    }
                } else {
                    this.activePanes[paneType] = null;
                    this.removePaneByType(paneType);
                }
            }
        },
        updateVisibleRanges: function(visibleRanges) {
            for(var paneType in visibleRanges) {
                if(visibleRanges.hasOwnProperty(paneType) && visibleRanges[paneType]) {
                    var pane = this.getPaneByType(paneType),
                        visibleRange = visibleRanges[paneType];
                    pane.setLastVisibleHeaderRowTileIndex(visibleRange.bottom);
                }
            }
        },
        // TODO to zhuravlev: refactor 
        calculatePaneCellVisibleRange: function() {
            var settings = this.getFrozenPaneSettings(),
                mode = settings.mode,
                mainPane = this.getPaneByType();

            switch(mode) {
                case 0: var topRightPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
                    topRightPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(0, 0, ASPxClientSpreadsheet.Range.MAX_COL_INDEX, settings.frozenCell.row - 1));
                    mainPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(0, settings.frozenCell.row, ASPxClientSpreadsheet.Range.MAX_COL_INDEX, ASPxClientSpreadsheet.Range.MAX_ROW_INDEX));
                    break;
                case 1:
                    var bottomLeftPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);
                    bottomLeftPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(0, 0, settings.frozenCell.col - 1, ASPxClientSpreadsheet.Range.MAX_ROW_INDEX));
                    mainPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(settings.frozenCell.col, 0, ASPxClientSpreadsheet.Range.MAX_COL_INDEX, ASPxClientSpreadsheet.Range.MAX_ROW_INDEX));
                    break;
                case 2:
                    var topRightPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane),
                        bottomLeftPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane),
                        frozenPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane);

                    frozenPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(0, 0, settings.frozenCell.col - 1, settings.frozenCell.row - 1));
                    topRightPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(settings.frozenCell.col, 0, ASPxClientSpreadsheet.Range.MAX_COL_INDEX, settings.frozenCell.row - 1));
                    bottomLeftPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(0, settings.frozenCell.row, settings.frozenCell.col - 1, ASPxClientSpreadsheet.Range.MAX_ROW_INDEX));
                    mainPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(settings.frozenCell.col, settings.frozenCell.row, ASPxClientSpreadsheet.Range.MAX_COL_INDEX, ASPxClientSpreadsheet.Range.MAX_ROW_INDEX));
                    break;
                default:
                    mainPane.setPaneCellVisibleRange(new ASPxClientSpreadsheet.Range(0, 0, ASPxClientSpreadsheet.Range.MAX_COL_INDEX, ASPxClientSpreadsheet.Range.MAX_ROW_INDEX));
                    break;
            }
        },

        // Working with pane infrastructure
        getPaneByType: function(paneType) {
            paneType = paneType ? paneType : ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
            var pane = this.activePanes[paneType];
            if(!pane && paneType === ASPxClientSpreadsheet.PaneManager.PanesType.MainPane)
                pane = this.createPaneByType(paneType);

            return pane;
        },
        getPaneByCell: function(cellModelPosition) {
            for(var paneType in this.activePanes)
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var pane = this.activePanes[paneType];
                    if(pane.isCellInPaneVisibleRange(cellModelPosition))
                        return pane;
                }
            return null;
        },
        getPaneTypeByCell: function(cellModelPosition) {
            if(this.getFrozenPaneSettings().isFrozen) {
                var pane = this.getPaneByCell(cellModelPosition);
                return pane ? pane.getPaneType() : null;
            }
            return ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
        },
        getPaneTypeByEvent: function(evt) {
            var point = this.getOwnerControl().getEventControlPoint(evt);
            return this.getPaneTypeByControlPoint(point);
        },
        getPaneTypeByControlPoint: function(point) {
            var settings = this.getFrozenPaneSettings(),
                headerOffsetSize = this.getHeaderOffsetSize();
            if(point.x > settings.width + headerOffsetSize.width)
                return point.y > settings.height + headerOffsetSize.height ? 
                    ASPxClientSpreadsheet.PaneManager.PanesType.MainPane : ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane;
            return point.y > settings.height + headerOffsetSize.height ? ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane : ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane;
        },
        createPaneByType: function(paneType) {
            var renderProvider = this.getRenderProvider(),
                pane = new ASPxClientSpreadsheet.PaneView(this, renderProvider, paneType);
            this.activePanes[paneType] = pane;

            renderProvider.prepareControlHierarchy(paneType);

            return pane;
        },
        removePaneByType: function(paneType){
            var workbook = this.getRenderProvider().getWorkbookControl(),
                gridContainer = this.getRenderProvider().getGridContainer(paneType),
                columnContainer = this.getRenderProvider().getColumnHeader(paneType),
                rowContainer = this.getRenderProvider().getRowHeader(paneType);
            if(gridContainer) {
                if(ASPx.Browser.IE && ASPx.Browser.Version < 9) {
                    var gridContainerEvents = ["click", "dblclick", "mousedown", "mouseup", "mousemove"],
                        handler = function(evt) { this["dispatchEvent"](evt, paneType); }.aspxBind(this.getOwnerControl());
                    for(var i = 0; i < gridContainerEvents.length; i++)
                        ASPx.Evt.DetachEventFromElement(gridContainer, gridContainerEvents[i], handler);
                }
                workbook.removeChild(gridContainer);
                this.getRenderProvider().resetGridTilesContainer(paneType);
            }
            if(columnContainer) {
                workbook.removeChild(columnContainer);
                this.getRenderProvider().resetColumnHeaderTileContainer(paneType);
            }
            if(rowContainer) {
                workbook.removeChild(rowContainer);
                this.getRenderProvider().resetRowHeaderTilesContainer(paneType);
            }
        },
        getOwnerControl: function() {
            return this.spreadsheetControl;
        },
        getRenderProvider: function() {
            return this.renderProvider;
        },
        getTileHelper: function() {
            return this.getPaneByType().getTileHelper();
        },
        getStateController: function() {
            return this.getOwnerControl().getStateController();
        },
        getSelectionHelper: function() {
            return this.getOwnerControl().getSelectionHelper();
        },
        getGridResizingHelper: function() {
            return this.getOwnerControl().getGridResizingHelper();
        },
        
        onVisibleTileRangeChanged: function() {
            this.getOwnerControl().onVisibleTileRangeChanged();
        },

        adjustVisiblePanes: function() {
            this.getOwnerControl().adjustRootControls();
        },
        getScrollHelper: function() {
            if(!this.scrollHelper)
                this.scrollHelper = new ASPxClientSpreadsheet.ScrollHelper(this);
            return this.scrollHelper;
        },
        loadInvisibleTiles: function() {
             this.getOwnerControl().loadInvisibleTiles();
        },
        loadInvisibleTilesForFullScreen: function() {
            this.getOwnerControl().loadInvisibleTilesForFullScreen();
        },
        getCellElementByModelPosition: function(columnModelIndex, rowModelIndex) {
            return this.getRenderProvider().getCellElementByModelPosition(columnModelIndex, rowModelIndex);
        },
        getAutoFilterImagesClassNames: function(imageName) {
            return this.getOwnerControl().autoFilterImagesClassNames[imageName];
        },
        onAutoFilterClick: function(element, columnType, hasFilter) {
            this.getOwnerControl().onAutoFilterClick(element, columnType, hasFilter);
        },
        restoreInitialState: function() {
            this.executeMethodForEachPane("removeAllTiles");
        },

        // ForEach Panes API
        executeMethodForEachPane: function() {
            var methodName = arguments[0];
            if(!methodName)
                return;
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    this.activePanes[paneType][methodName](arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
                }
            }
        },
        getObjectValueSearchInEachPane: function() {
            var methodName = arguments[0];
            if(!methodName)
                return;
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var objectValue = this.activePanes[paneType][methodName](arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
                    if(objectValue)
                        return objectValue;
                }
            }
            return null;
        },
        //TODO to zhuravlev - try to refactor
        getNumericValueSearchInEachPane: function() {
            var methodName = arguments[0];
            if(!methodName)
                return;
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var numericValue = this.activePanes[paneType][methodName](arguments[1], arguments[2], arguments[3]);
                    if(numericValue !== null && numericValue >= 0)
                        return numericValue;
                }
            }
            return -1;
        },

        getCellLayoutInfo: function(colVisibleIndex, rowVisibleIndex) {
            var colModelIndex = this.convertVisibleIndexToModelIndex(colVisibleIndex, true),
                rowModelIndex = this.convertVisibleIndexToModelIndex(rowVisibleIndex, false);

            var pane = this.getPaneByCell({ col: colModelIndex, row: rowModelIndex });
            if(pane)
                return pane.getCellLayoutInfo(colVisibleIndex, rowVisibleIndex);
            return this.getObjectValueSearchInEachPane("getCellLayoutInfo", colVisibleIndex, rowVisibleIndex);
        },
        
        getCellByCoord: function(xPosition, yPosition, paneType) {
            var activePane = this.getPaneByType(paneType);
            if(activePane)
                return activePane.getCellByCoord(xPosition, yPosition);
            return this.getObjectValueSearchInEachPane("getCellByCoord", xPosition, yPosition);
        },
        convertVisibleIndexToModelIndex: function(visibleIndex, isCol) {
            return this.getNumericValueSearchInEachPane("convertVisibleIndexToModelIndex", visibleIndex, isCol)
        },
        convertVisiblePositionToModelPosition: function(colVisibleIndex, rowVisibleIndex) {
            var colIndex = this.convertVisibleIndexToModelIndex(colVisibleIndex, true),
                rowIndex = this.convertVisibleIndexToModelIndex(rowVisibleIndex, false);
            return {
                colIndex: colIndex,
                rowIndex: rowIndex
            };
        },
        convertModelIndicesRangeToVisibleIndices: function(modelTopLeftIndex, modelBottomRightIndex, isCol) {
            if(modelTopLeftIndex < 0 || modelBottomRightIndex < 0)
                return { visibleTopLeftIndex: modelTopLeftIndex, visibleBottomRightIndex: modelBottomRightIndex };

            var bestSearchValue = null;
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var visibleRange = this.activePanes[paneType].convertModelIndicesRangeToVisibleIndices(modelTopLeftIndex, modelBottomRightIndex, isCol);
                    if(visibleRange.rangeExist.topLeft && visibleRange.rangeExist.topLeft)
                        return visibleRange;
                    else if(visibleRange.rangeExist.topLeft || visibleRange.rangeExist.topLeft)
                        bestSearchValue = visibleRange;
                    else if(bestSearchValue == null && paneType == ASPxClientSpreadsheet.PaneManager.PanesType.MainPane)
                        bestSearchValue = visibleRange;
                }
            }
            return bestSearchValue;
        },
        convertModelIndexToVisibleIndex: function(modelIndex, isCol) {
            return this.getNumericValueSearchInEachPane("convertModelIndexToVisibleIndex", modelIndex, isCol);
        },
        highlightHeaders: function(selection) {
            if(selection.visible)
                this.executeMethodForEachPane("highlightHeaders", selection);
        },
        getCellEditingText: function(colModelIndex, rowModelIndex) {
            return this.getObjectValueSearchInEachPane("getCellEditingText", colModelIndex, rowModelIndex);
        },
        getCellValue: function(colModelIndex, rowModelIndex) {
            return this.getObjectValueSearchInEachPane("getCellValue", colModelIndex, rowModelIndex);
        },
        getHeaderInfo: function(index, isCol) {
            return this.getObjectValueSearchInEachPane("getHeaderInfo", index, isCol);
        },
        // Model params
        getVisibleRangePaddings: function () {
            return this.getOwnerControl().getVisibleRangePaddings();
        },
        getTileSize: function() {
            return this.getOwnerControl().getTileSize();
        },
        getTileSizes: function(index, isCol) {
            return this.getObjectValueSearchInEachPane("getTileSizes", index, isCol);
        },
        getCellSize: function(uniqueCellIndex, isCol) {
            var tileSize = isCol ? this.getTileSize().col : this.getTileSize().row,
                tileIndex = Math.floor(uniqueCellIndex / tileSize),
                cellIndexInTile = uniqueCellIndex - tileIndex * tileSize
            return this.getTileSizes(tileIndex, isCol)[cellIndexInTile];
        },
        getTileTotalSize: function(index, isCol) {
            return this.getNumericValueSearchInEachPane("getTileTotalSize", index, isCol);
        },
        getGridTileInfo: function(rowIndex, colIndex) {
            return this.getObjectValueSearchInEachPane("getGridTileInfo", rowIndex, colIndex);
        },
        getActiveCellParentTileInfo: function(selection) {
            return this.getObjectValueSearchInEachPane("getActiveCellParentTileInfo", selection);
        },
        getDefaultCellSize: function() {
            return this.getOwnerControl().getDefaultCellSize();
        },
        getLastFilledCell: function() {
            return this.getOwnerControl().getLastFilledCell(); 
        },
        isSheetLoaded: function(){
            return this.getOwnerControl().isSheetLoaded();
        },
        
        isMergerCellContainsInCroppedRange: function(cellModelPosition) {
            return this.getObjectValueSearchInEachPane("isMergerCellContainsInCroppedRange", cellModelPosition);
        },
        getLeftTopModelCellPositionInCroppedRange: function(cellModelPosition) {
            return this.getObjectValueSearchInEachPane("getLeftTopModelCellPositionInCroppedRange", cellModelPosition);
        },
        getCellLayoutInfo_ByModelIndices: function(colModelIndex, rowModelIndex) {
            var pane = this.getPaneByCell({ col: colModelIndex, row: rowModelIndex });
            if(pane)
                return pane.getCellLayoutInfo_ByModelIndices(colModelIndex, rowModelIndex);
            return this.getObjectValueSearchInEachPane("getCellLayoutInfo_ByModelIndices", colModelIndex, rowModelIndex);
        },
        // TODO remove duplicates
        serializeCachedTiles: function() {
            var cachedTiles = "";
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType])
                    var paneCachedTiles = this.activePanes[paneType].serializeCachedTiles();
                if(paneCachedTiles)
                    cachedTiles += (cachedTiles ? ";" + paneCachedTiles : paneCachedTiles);
            }
            return cachedTiles;
        },
        getCellsGridAbsolutePosition: function() {
            var mode = this.getFrozenPaneSettings().mode,
                mainPaneContainer = this.getRenderProvider().getGridContainer(ASPxClientSpreadsheet.PaneManager.PanesType.MainPane),
                mainPaneSize = this.getRenderProvider().getGridOffsetSize(ASPxClientSpreadsheet.PaneManager.PanesType.MainPane),
                mainPaneX = ASPx.GetAbsolutePositionX(mainPaneContainer),
                mainPaneY = ASPx.GetAbsolutePositionY(mainPaneContainer),
                leftPaneContainer,
                topPaneContainer;

            switch(mode) {
                case 0:
                    topPaneContainer = this.getRenderProvider().getGridContainer(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
                    break;
                case 1:
                    leftPaneContainer = this.getRenderProvider().getGridContainer(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);
                    break;
                case 2:
                    leftPaneContainer = this.getRenderProvider().getGridContainer(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);
                    topPaneContainer = this.getRenderProvider().getGridContainer(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
                    break;
            }

            var left = leftPaneContainer ? ASPx.GetAbsolutePositionX(leftPaneContainer) : mainPaneX,
                top = topPaneContainer ? ASPx.GetAbsolutePositionY(topPaneContainer) : mainPaneY,
                right = mainPaneX + mainPaneSize.width,
                bottom = mainPaneY + mainPaneSize.height;

            return {
                left: left,
                top: top,
                right: right,
                bottom: bottom
            };
        },

        // Scrolling and Navigation
        canScrollToShowCell: function(colVisibleIndex, rowVisibleIndex) {
            return this.getObjectValueSearchInEachPane("canScrollToShowCell", colVisibleIndex, rowVisibleIndex);            
        },
        navigateTo: function(stateController, selection) {
            var cellScrollVisible = false,
                colModelIndex = selection.activeCellColIndex,
                rowModelIndex = selection.activeCellRowIndex,
                activePane = this.getPaneByCell({ col: colModelIndex, row: rowModelIndex }),
                mainPane = this.getPaneByType();

            if(!activePane)
                return;
            var visibleCellRange = this.getDisplayedCellVisibleRange(activePane);

            if(visibleCellRange.isCellInRange(colModelIndex, rowModelIndex)) {
                cellScrollVisible = true;
            } else {
                var shiftedScrollAnchor = this.getScrollAnchorToMakeCellVisible(activePane, visibleCellRange, colModelIndex, rowModelIndex);
                var canScrollToNewAnchor = mainPane.canScrollToShowCell(shiftedScrollAnchor.col, shiftedScrollAnchor.row);
                var canShowNewSelectedCell = activePane.canScrollToShowCell(colModelIndex, rowModelIndex);
                if(canScrollToNewAnchor && canShowNewSelectedCell) {
                    var scrollHelper = this.getScrollHelper();

                    scrollHelper.applyScrollAnchor(shiftedScrollAnchor);            
                    scrollHelper.increaseScrollableAreaProportionally();                
                    cellScrollVisible = true;
                } else {
                    stateController.setSelectionSilently(selection);
                    this.getOwnerControl().scrollToViaServer(shiftedScrollAnchor.col, shiftedScrollAnchor.row);
                }
            }
            return cellScrollVisible;
        },
        scrollTo: function(colModelIndex, rowModelIndex, selectAfterScroll) {
            var scrollHelper = this.getScrollHelper(),
                activePane = this.getPaneByCell({ col: colModelIndex, row: rowModelIndex }),
                colVisibleIndex = this.convertModelIndexToVisibleIndex(colModelIndex, true),
                rowVisibleIndex = this.convertModelIndexToVisibleIndex(rowModelIndex, false),
                visibleCellRange = this.getDisplayedCellVisibleRange(activePane),
                cellIsScrollVisible = false;

            if(visibleCellRange.isCellInRange(colVisibleIndex, rowVisibleIndex))
                cellIsScrollVisible = true;
            else if(this.canScrollToShowCell(colVisibleIndex, rowVisibleIndex)) {
                scrollHelper.applyScrollAnchor({ col: colVisibleIndex, row: rowVisibleIndex });
                cellIsScrollVisible = true;
            } else {
                this.getOwnerControl().scrollToViaServer(colModelIndex, rowModelIndex, selectAfterScroll);
            }

            if(cellIsScrollVisible && selectAfterScroll) {
                var range = new ASPxClientSpreadsheet.Range(colVisibleIndex, rowVisibleIndex, colVisibleIndex, rowVisibleIndex);
                var selection = new ASPxClientSpreadsheet.Selection(range);
                this.getStateController().setSelection(selection);
            }
        },
        //TODO to Zhuravlev: try to optimize convertion
        getDisplayedCellVisibleRange: function(activePane) {
            var gridContainer = activePane.getGridContainer(),
               scrollHelper = this.getScrollHelper(),
               scrollContainer = this.getRenderProvider().getScrollContainer(),
               spacer = scrollHelper.getSpacer(),
               scrollLeft = 0,
               scrollTop = 0,
               width = gridContainer.offsetWidth,
               height = gridContainer.offsetHeight,
               settings = this.getFrozenPaneSettings();

            switch(activePane.getPaneType()) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:
                    scrollTop = scrollHelper.getScrollAreaHeightSizeByIndex(this.convertModelIndexToVisibleIndex(settings.topLeftCell.row, false));
                    scrollLeft = scrollHelper.getScrollAreaWidthSizeByIndex(this.convertModelIndexToVisibleIndex(settings.topLeftCell.col, true));
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    scrollTop = scrollHelper.getScrollAreaHeightSizeByIndex(this.convertModelIndexToVisibleIndex(settings.topLeftCell.row, false));
                    scrollLeft = scrollContainer.scrollLeft + scrollHelper.getMinimumScrollLeftPosition();
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    scrollTop = scrollContainer.scrollTop + scrollHelper.getMinimumScrollTopPosition();
                    scrollLeft = scrollHelper.getScrollAreaWidthSizeByIndex(this.convertModelIndexToVisibleIndex(settings.topLeftCell.col, true));
                    break;
                default: //ASPxClientSpreadsheet.PaneManager.PanesType.MainPane
                    scrollTop = scrollContainer.scrollTop + scrollHelper.getMinimumScrollTopPosition();
                    scrollLeft = scrollContainer.scrollLeft + scrollHelper.getMinimumScrollLeftPosition();
                    break;
            }
            return this.getVisibleCellRangeByScrollSize(scrollLeft, scrollTop, width, height);
        },
        getScrollAnchorToMakeCellVisible: function(activePane, visibleCellRange, colModelIndex, rowModelIndex) {
            var scrollAnchor = this.getScrollHelper().getScrollAnchorModelPosition();
            switch(activePane.getPaneType()) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.MainPane:
                    if(colModelIndex < visibleCellRange.leftColIndex)
                        scrollAnchor.col = colModelIndex;
                    if(colModelIndex > visibleCellRange.rightColIndex)
                        scrollAnchor.col += colModelIndex - visibleCellRange.rightColIndex;
                    if(rowModelIndex < visibleCellRange.topRowIndex)
                        scrollAnchor.row = rowModelIndex;
                    if(rowModelIndex > visibleCellRange.bottomRowIndex)
                        scrollAnchor.row += rowModelIndex - visibleCellRange.bottomRowIndex;
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    if(colModelIndex < visibleCellRange.leftColIndex)
                        scrollAnchor.col = colModelIndex;
                    if(colModelIndex > visibleCellRange.rightColIndex)
                        scrollAnchor.col += colModelIndex - visibleCellRange.rightColIndex;
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    if(rowModelIndex < visibleCellRange.topRowIndex)
                        scrollAnchor.row = rowModelIndex;
                    if(rowModelIndex > visibleCellRange.bottomRowIndex)
                        scrollAnchor.row += rowModelIndex - visibleCellRange.bottomRowIndex;
                    break;
            }
            if(this.getPaneByCell(scrollAnchor))
                return scrollAnchor;

            return this.getScrollHelper().getScrollAnchorModelPosition();
        },
        getVisibleCellRangeByScrollSize: function(scrollLeft, scrollTop, width, height) {
            var scrollHelper = this.getScrollHelper();
            return new ASPxClientSpreadsheet.Range(scrollHelper.getColCountByWidth(scrollLeft),scrollHelper.getRowCountByHeight(scrollTop),
                            scrollHelper.getColCountByWidth(scrollLeft + width) - 1, scrollHelper.getRowCountByHeight(scrollTop + height) - 1);
        },

        getTopLeftCellVisiblePosition: function() {
            var settings = this.getFrozenPaneSettings();
            if(settings.isFrozen)
                return { col: this.convertModelIndexToVisibleIndex(settings.topLeftCell.col, true), row: this.convertModelIndexToVisibleIndex(settings.topLeftCell.row, false) };
            return this.getScrollAnchorVisiblePosition();
        },

        // Load cached tiles
        tryLoadTilesFromCache: function() {
            var tileHelper = this.getTileHelper(),
                loadingTilesResult = tileHelper.tryLoadTilesFromCache(),
                settings = this.getFrozenPaneSettings();

            if(loadingTilesResult && settings.isFrozen) {
                var visibleRange = tileHelper.getVisibleRange(),
                    mode = settings.mode,
                    newVisibleRange;
                if(mode !== 1) {
                    var topRightPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane),
                        topRightVisibleRange = topRightPane.getVisibleRange();
                    if(this.isRangeChangedInOneDirection(visibleRange, topRightVisibleRange, true)) {
                        newVisibleRange = { left: visibleRange.left, right: visibleRange.right, top: topRightVisibleRange.top, bottom: topRightVisibleRange.bottom };
                        topRightPane.loadTilesSoftly(newVisibleRange);
                    }
                }
                if(mode !== 0) {
                    var bottomLeftPane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane),
                        bottomLeftVisibleRange = bottomLeftPane.getVisibleRange();
                    if(this.isRangeChangedInOneDirection(visibleRange, bottomLeftVisibleRange, false)) {
                        newVisibleRange = { left: bottomLeftVisibleRange.left, right: bottomLeftVisibleRange.right, top: visibleRange.top, bottom: visibleRange.bottom };
                        bottomLeftPane.loadTilesSoftly(newVisibleRange);
                    }
                }
            }
            return loadingTilesResult;
        },
        isRangeChangedInOneDirection: function(visibleRange, paneVisibleRange, isCol) {
            if(isCol)
                return visibleRange.left !== paneVisibleRange.left || visibleRange.right !== paneVisibleRange.right;

            return visibleRange.top !== paneVisibleRange.top || visibleRange.bottom !== paneVisibleRange.bottom
        },

        getWorkbookVisibleCellRange: function() {
            var settings = this.getFrozenPaneSettings(),
                mode = settings.mode,
                visibleRange = this.getCellVisibleRange();

            switch(mode) {
                case 0:var topRightVisibleRange = this.getCellVisibleRange(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
                    return { top: topRightVisibleRange.top, bottom: visibleRange.bottom, right: visibleRange.right, left: visibleRange.left };                    
                case 1:
                    var bottomLeftVisibleRange = this.getCellVisibleRange(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);
                    return { top: visibleRange.top, bottom: visibleRange.bottom, right: visibleRange.right, left: bottomLeftVisibleRange.left };
                case 2:
                    var frozenVisibleRange = this.getCellVisibleRange(ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane);
                    return { top: frozenVisibleRange.top, bottom: visibleRange.bottom, right: visibleRange.right, left: frozenVisibleRange.left };

                default:
                    return visibleRange;
            }            
        },
        putSelectionCellIndexToVisibleDocumentRange: function(index, isCol) {
            var cellVisibleRange = this.getWorkbookVisibleCellRange();
            if(isCol) {
                if(index < cellVisibleRange.left)
                    index = cellVisibleRange.left;

                if(index > cellVisibleRange.right)
                    index = cellVisibleRange.right;
            } else {
                if(index < cellVisibleRange.top)
                    index = cellVisibleRange.top;

                if(index > cellVisibleRange.bottom)
                    index = cellVisibleRange.bottom;
            }
            return index;
        },

        getVisibleModelCellRange: function(paneType) {
            var pane = this.getPaneByType(paneType);
            return pane ? pane.getVisibleModelCellRange() : null;
        },
        getCellVisibleRange: function(paneType) {
            var pane = this.getPaneByType(paneType);
            return pane ? pane.getCellVisibleRange() : null;
        },
        putCellColIndexInVisibleRange: function(colIndex, paneType) {
            var pane = this.getPaneByType(paneType);
            return pane ? pane.putCellColIndexInVisibleRange(colIndex) : colIndex;
        },
        putCellRowIndexInVisibleRange: function(rowIndex, paneType) {
            var pane = this.getPaneByType(paneType);
            return pane ? pane.putCellRowIndexInVisibleRange(rowIndex) : rowIndex;
        },
        getMergedCellRangesIntersectsRange: function(range) {
            range.correct();
            if(this.rangeLocatedInSinglePane(range)) {
                var pane = this.getPaneByCell({ col: range.leftColIndex, row: range.topRowIndex });
                return pane.getMergedCellRangesIntersectsRange(range);
            }

            var complexboxes =  [];
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var pane = this.activePanes[paneType];
                    complexboxes = complexboxes.concat(pane.getMergedCellRangesIntersectsRange(range));
                }
            }
            return complexboxes;
        },
        rangeLocatedInSinglePane: function(range) {
            return range.isSingleCell() || this.getPaneTypeByCell({ col: range.leftColIndex, row: range.topRowIndex }) === this.getPaneTypeByCell({ col: range.rightColIndex, row: range.bottomRowIndex });
        },
        getVisibleVisibleCellRange: function(paneType) {
            var pane = this.getPaneByType(paneType);
            return pane ? pane.getVisibleVisibleCellRange() : { top: 0, right: 0, bottom: 0, left: 0 };
        },
        loadTilesForNewVisibleRange: function(scrollAnchorPosition) {
            var visibleRange = this.getTileHelper().calculateNewVisibleRange(scrollAnchorPosition);
            if(visibleRange) {
                this.getScrollHelper().setScrollAnchor(scrollAnchorPosition);
                this.getTileHelper().addVisibleRangeToWiatingForLoad(visibleRange);
                this.loadInvisibleTilesForFullScreen();
            }
        },
        changeScrollableArea: function(ribbonState) {
            if(ribbonState === ASPxClientRibbonState.Minimized) {
                this.getScrollHelper().changeScrollableArea(ASPxClientSpreadsheet.Constants.RibbonHeightInRows, false);
                this.getGridResizingHelper().updateHeaderRanges();
            } else {
                this.getScrollHelper().changeScrollableArea(-ASPxClientSpreadsheet.Constants.RibbonHeightInRows, false);
            }
        },
        calculateHeaderViewBounds: function() {
            var viewBounds = {};
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var pane = this.activePanes[paneType],
                        topLeftCell = this.getPaneTopLeftCellVisiblePosition(paneType);

                    viewBounds[paneType] = pane.calculateViewBoundRange(topLeftCell, this.getRenderProvider().getGridOffsetSize(paneType));
                }
            }
            return viewBounds
        },
        getVisibleAreaSize: function() {
            var gridOffsetSize = this.getRenderProvider().getGridOffsetSize(),
                headerOffsetSize = this.getHeaderOffsetSize(),
                width = gridOffsetSize.width + headerOffsetSize.width,
                height = gridOffsetSize.height + headerOffsetSize.height,
                pane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);
            if(pane)
                width += this.getRenderProvider().getGridOffsetSize(pane.getPaneType()).width;
            pane = this.getPaneByType(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
            if(pane)
                height += this.getRenderProvider().getGridOffsetSize(pane.getPaneType()).height;
            return { width: width, height: height };
        },

        getScrollAnchorModelPosition: function() {
            return this.getScrollHelper().getScrollAnchorModelPosition();
        },
        getScrollAnchorVisiblePosition: function() {
            return this.getScrollHelper().getScrollAnchorVisiblePosition();
        },
        restoreScrollPositionRequired: function() {
            return this.getScrollHelper().restoreScrollPositionRequired();
        },
        updateScrollPosition: function() {
            this.getScrollHelper().updateScrollPosition();
        },
        createTouchUIScroller: function() {
            this.getScrollHelper().createTouchUIScroller();
        },
        getParentTileInfo: function(selection) {
            return this.getObjectValueSearchInEachPane("getParentTileInfo", selection);
        },
        convertModelRangeToVisible: function(range) {
            var bestSearchValue = null;
            for(var paneType in this.activePanes) {
                if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                    var visibleRange = this.activePanes[paneType].convertModelRangeToVisible(range);
                    if(visibleRange.isVisible)
                        return visibleRange;
                    else if(bestSearchValue == null && paneType == ASPxClientSpreadsheet.PaneManager.PanesType.MainPane)
                        bestSearchValue = visibleRange;
                }
            }
            return bestSearchValue;
        },// TODO rework it if selected 1+ panes
        isSelectionVisible: function(selection) {
            return this.getObjectValueSearchInEachPane("isSelectionVisibleInPane", selection);
        },
        getTileHelperByCellPostition: function(cellModelPosition) {
            var tileHelper = this.getObjectValueSearchInEachPane("searchCellInVisibleRange", cellModelPosition);
            if(!tileHelper)
                tileHelper = this.getTileHelper();
            return tileHelper;
        },
        setScrollPositionOnHoldenPanes: function(scrolledCellColumnInfo, scrolledCellRowInfo, minimumScrollLeftPosition, minimumScrollTopPosition) {
            var paneType = ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane,
                gridContainer_BL = this.getRenderProvider().getGridContainer(paneType);
            if(gridContainer_BL) {
                gridContainer_BL.scrollLeft = minimumScrollLeftPosition;
                if(scrolledCellRowInfo)
                    gridContainer_BL.scrollTop = scrolledCellRowInfo.gridScrollValue;
                this.getRenderProvider().getColumnHeader(paneType).scrollLeft = minimumScrollLeftPosition;
            }
            paneType = ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane;
            var gridContainer_TR = this.getRenderProvider().getGridContainer(paneType);
            if(gridContainer_TR) {
                if(scrolledCellColumnInfo)
                    gridContainer_TR.scrollLeft = scrolledCellColumnInfo.gridScrollValue;
                gridContainer_TR.scrollTop = minimumScrollTopPosition;
                this.getRenderProvider().getRowHeader(paneType).scrollTop = minimumScrollTopPosition;
            }
            paneType = ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane;
            var gridContainer_FP = this.getRenderProvider().getGridContainer(paneType);
            if(gridContainer_FP) {
                gridContainer_FP.scrollLeft = minimumScrollLeftPosition;
                gridContainer_FP.scrollTop = minimumScrollTopPosition;
            }
        },
        
        getHeaderOffsetSizeForMousePointer: function(paneType) {
            var headerOffsetSize = this.getHeaderOffsetSize(),
                frozenParams = this.getFrozenPaneSettings();

            switch(paneType) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:
                    return headerOffsetSize;
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    return {
                        width: headerOffsetSize.width,
                        height: headerOffsetSize.height + frozenParams.height
                    };
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    return {
                        width: headerOffsetSize.width + frozenParams.width,
                        height: headerOffsetSize.height
                    };
                default:
                    return {
                        width: headerOffsetSize.width + frozenParams.width,
                        height: headerOffsetSize.height + frozenParams.height
                    }
            }
        },
        // Attach Events
        attachEventsToPaneElements: function(paneType) {
            this.attachEventsToGridContainer(paneType);
            this.attachEventsToHeaders(paneType);
            this.attachContextMenuEvents(paneType);
        },

        attachEventToMainElement: function() {
            this.initializeHandlersCore(this.getRenderProvider().GetMainElement(), [ASPx.TouchUIHelper.touchMouseDownEventName], "activateControl");
        },
        attachEventsToGridContainer: function(paneType) {
            paneType = paneType ? paneType : ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
            var gridContainer = this.getRenderProvider().getGridContainer(paneType);
            if(gridContainer) {
                if(ASPx.Browser.WebKitTouchUI) {
                    var gridContainerEvents = ["touchstart", "touchend", "touchmove"];
                } else if(ASPx.Browser.MSTouchUI) {
                    gridContainerEvents = ["mousedown", "mousemove", "dblclick"];
                    var spreadsheet = this.getOwnerControl();
                    gridContainer.dxMsTouchGesture = ASPx.TouchUIHelper.msTouchCreateGesturesWrapper(gridContainer, spreadsheet.dispatchEvent.aspxBind(spreadsheet));
                } else
                    gridContainerEvents = ["click", "dblclick", "mousedown", "mouseup", "mousemove"];
                this.initializeHandlersCore(gridContainer, gridContainerEvents, "dispatchEvent", paneType);
            }
        },
        attachEventsToHeaders: function(paneType) {
            paneType = paneType ? paneType : ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
            var rowHeader = this.getRenderProvider().getRowHeader(paneType),
                colHeader = this.getRenderProvider().getColumnHeader(paneType),
                headerEvents = ASPx.Browser.WebKitTouchUI ? ["touchstart"] : ["click", "mousedown", "mouseup", "mousemove", "dblclick"];

            if(rowHeader)
                this.initializeHandlersCore(rowHeader, headerEvents, "dispatchRowHeaderEvent", paneType);
            if(colHeader)
                this.initializeHandlersCore(colHeader, headerEvents, "dispatchColHeaderEvent", paneType);
        },
        attachEventsToFormulaBarElements: function() {
            this.initializeHandlersCore(this.getOwnerControl().getCellTextViewElement(), ["click", "keyup"], "dispatchEvent");
            var formulaBarMenu = this.getOwnerControl().getFormulaBarMenu();
            var formulaBarMenuElement = formulaBarMenu.GetMainElement();
            ASPx.Evt.AttachEventToElement(formulaBarMenuElement, "mousedown", ASPx.Evt.PreventEventAndBubble);
            ASPx.Evt.AttachEventToElement(formulaBarMenuElement, "mouseup", ASPx.Evt.PreventEventAndBubble);
        },

        initializeHandlersCore: function(element, eventNames, methodName, paneType) {
            var handler = function(evt) { this[methodName](evt, paneType); }.aspxBind(this.getOwnerControl());
            for(var i = 0; i < eventNames.length; i++)
                ASPx.Evt.AttachEventToElement(element, eventNames[i], handler);
            var isIEBrowserVersionUnder9 = ASPx.Browser.IE && ASPx.Browser.Version < 9;
            if(!ASPx.Browser.WebKitFamily)
                ASPx.Evt.PreventElementDragAndSelect(element, isIEBrowserVersionUnder9, isIEBrowserVersionUnder9);
        },

        attachContextMenuEvents: function(paneType) {
            paneType = paneType ? paneType : ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
            var columnHeader = this.getRenderProvider().getColumnHeader(paneType),
                rowHeader = this.getRenderProvider().getRowHeader(paneType),
                grid = this.getRenderProvider().getGridContainer(paneType);

            if(columnHeader) ASPx.Evt.AttachEventToElement(columnHeader, "contextmenu", this.onColumnHeaderContextMenu.aspxBind(this), true);
            if(rowHeader) ASPx.Evt.AttachEventToElement(rowHeader, "contextmenu", this.onRowHeaderContextMenu.aspxBind(this), true);
            ASPx.Evt.AttachEventToElement(grid, "contextmenu", this.onGridContextMenu.aspxBind(this), true);
        },
        onColumnHeaderContextMenu: function(e) {
            this.onContextMenuCore(e, { isColumnHeader: true });
        },
        onRowHeaderContextMenu: function(e) {
            this.onContextMenuCore(e, { isRowHeader: true });
        },
        onGridContextMenu: function(e) {
            this.onContextMenuCore(e, {});
        },
        onContextMenuCore: function(e, context) {
            ASPx.Evt.PreventEventAndBubble(e);

            this.getOwnerControl().showPopupMenu(e, context);
        },

        getPaneTopLeftCellVisiblePosition: function(paneType) {
            var settings = this.getFrozenPaneSettings(),
                topLeftCell = settings.topLeftCell,
                scrolledCell = this.getScrollHelper().getScrollAnchorVisiblePosition();

            switch(paneType) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:       
                    return { col: this.convertModelIndexToVisibleIndex(topLeftCell.col, true), row: this.convertModelIndexToVisibleIndex(topLeftCell.row, false) };

                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    return { col: this.convertModelIndexToVisibleIndex(topLeftCell.col, true), row: scrolledCell.row };

                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    return { col: scrolledCell.col, row: this.convertModelIndexToVisibleIndex(topLeftCell.row, false) };

                default:
                    return scrolledCell;
            }
        },
        getPaneTopLeftCellLayoutInfo: function(paneType) {
            var topLeftCell = this.getPaneTopLeftCellVisiblePosition(paneType);
            if(topLeftCell)
                return this.getPaneByType(paneType).getCellLayoutInfo(topLeftCell.col, topLeftCell.row);
            return null;
        },
        getCellParentTileInfo: function(cell, paneType) {
            var pane = this.getPaneByType(paneType);
            if(pane)
                return pane.getCellParentTileInfo(cell);
            return this.getPaneByType().getCellParentTileInfo(cell);
        },
        // Drawing selection elements
        drawSelection: function(selection, keepOldSelection) {
            if(!keepOldSelection)
                this.hideSelection();
            this.correctSelection(selection);

            if(selection.range.isSingleCell() || selection.range.startPaneType === selection.range.endPaneType) {
                this.drawSelectionInSinglePane(selection, keepOldSelection);
            } else {
                this.drawSelectionInMultyPanes(selection, keepOldSelection);
            }
        },
        correctSelection: function(selection) {
            var settings = this.getFrozenPaneSettings();
            if(!selection.range.startPaneType) {
                var startPaneType = ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
                if(settings.isFrozen) {
                    var cellModelPosition = this.convertVisiblePositionToModelPosition(selection.range.leftColIndex, selection.range.topRowIndex);
                    startPaneType = this.getPaneTypeByCell({ col: cellModelPosition.colIndex, row: cellModelPosition.rowIndex });
                }
                selection.range.setStartPaneType(startPaneType);
            }
            
            if(!selection.range.endPaneType) {
                var endPaneType = ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
                if(settings.isFrozen) {
                    var cellModelPosition = this.convertVisiblePositionToModelPosition(selection.range.rightColIndex, selection.range.bottomRowIndex);
                    endPaneType = this.getPaneTypeByCell({ col: cellModelPosition.colIndex, row: cellModelPosition.rowIndex });
                }
                selection.range.setEndPaneType(endPaneType);
            }
        },
        drawSelectionInSinglePane: function(selection, keepOldSelection) {
            var activePane = this.getPaneByType(selection.range.startPaneType);
            if(activePane)
                activePane.drawSelection(selection, keepOldSelection);
        },
        drawSelectionInMultyPanes: function(selection, keepOldSelection) {
            selection.correctPaneSelection();

            if(selection.range.diagonalSelection) {
                for(var paneType in this.activePanes) {
                    if(this.activePanes.hasOwnProperty(paneType) && this.activePanes[paneType]) {
                        var pane = this.activePanes[paneType],
                            paneSelection = this.getDiagonalSelectionByPaneType(pane, selection);
                        if(paneSelection.isActiveCellInRange)
                            pane.drawSelection(paneSelection, keepOldSelection);
                        else 
                            pane.drawPartSelection(paneSelection, this.getVisibleBordersForDiagonalSelection(paneType));
                    }
                }
            } else {
                var startPane = this.getPaneByType(selection.range.startPaneType),
                    endPane = this.getPaneByType(selection.range.endPaneType),
                    isHorizontalCrossing = this.isHorizontalCrossing(selection.range.startPaneType, selection.range.endPaneType),
                    startCrossSelection = this.getCrossSelection(startPane, selection, true, isHorizontalCrossing),
                    endCrossSelection = this.getCrossSelection(endPane, selection, false, isHorizontalCrossing);
                
                if(startCrossSelection.isActiveCellInRange)
                    startPane.drawSelection(startCrossSelection, keepOldSelection);
                else
                    startPane.drawPartSelection(startCrossSelection, this.getVisibleBordersForCrossSelection(true, isHorizontalCrossing));

                if(endCrossSelection.isActiveCellInRange)
                    endPane.drawSelection(endCrossSelection, keepOldSelection);
                else
                    endPane.drawPartSelection(endCrossSelection, this.getVisibleBordersForCrossSelection(false, isHorizontalCrossing));
                
            }
        },
        getDiagonalSelectionByPaneType: function(pane, sheetSelection) {
            var selection = sheetSelection.clone(),
                visibleRange = pane.getCellVisibleRange(),
                range = null;

            switch(pane.getPaneType()) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:
                    range = new ASPxClientSpreadsheet.Range(selection.range.leftColIndex, selection.range.topRowIndex, visibleRange.right, visibleRange.bottom);
                    selection.isActiveCellInRange = pane.isCellInPaneVisibleRange({ col: sheetSelection.activeCellRange.leftColIndex, row: sheetSelection.activeCellRange.topRowIndex });
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.MainPane:
                    range = new ASPxClientSpreadsheet.Range(visibleRange.left, visibleRange.top, selection.range.rightColIndex, selection.range.bottomRowIndex);
                    selection.isActiveCellInRange = pane.isCellInPaneVisibleRange({ col: sheetSelection.activeCellRange.rightColIndex, row: sheetSelection.activeCellRange.bottomRowIndex });
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    range = new ASPxClientSpreadsheet.Range(selection.range.leftColIndex, visibleRange.top, visibleRange.right, selection.range.bottomRowIndex);
                    selection.isActiveCellInRange = pane.isCellInPaneVisibleRange({ col: sheetSelection.activeCellRange.leftColIndex, row: sheetSelection.activeCellRange.bottomRowIndex });
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    range = new ASPxClientSpreadsheet.Range(visibleRange.left, selection.range.topRowIndex, selection.range.rightColIndex, visibleRange.bottom);
                    selection.isActiveCellInRange = pane.isCellInPaneVisibleRange({ col: sheetSelection.activeCellRange.rightColIndex, row: sheetSelection.activeCellRange.topRowIndex });
                    break;
            }
            selection.setRange(range);
            return selection;
        },
        getCrossSelection: function(pane, sheetSelection, startPane, isHorizontalCrossing) {
            var selection = sheetSelection.clone(),
                visibleRange = pane.getCellVisibleRange(),
                range = null;

            if(isHorizontalCrossing)
                range = startPane ? new ASPxClientSpreadsheet.Range(selection.range.leftColIndex, selection.range.topRowIndex, visibleRange.right, selection.range.bottomRowIndex) :
                        new ASPxClientSpreadsheet.Range(visibleRange.left, selection.range.topRowIndex, selection.range.rightColIndex, selection.range.bottomRowIndex);
            else
                range = startPane ? new ASPxClientSpreadsheet.Range(selection.range.leftColIndex, selection.range.topRowIndex, selection.range.rightColIndex, visibleRange.bottom) :
                        new ASPxClientSpreadsheet.Range(selection.range.leftColIndex, visibleRange.top, selection.range.rightColIndex, selection.range.bottomRowIndex);
            selection.setRange(range);
            selection.isActiveCellInRange = pane.isCellInPaneVisibleRange({ col: sheetSelection.activeCellRange.leftColIndex, row: sheetSelection.activeCellRange.topRowIndex }) ||
                                            pane.isCellInPaneVisibleRange({ col: sheetSelection.activeCellRange.rightColIndex, row: sheetSelection.activeCellRange.bottomRowIndex });
            return selection
        },
        isHorizontalCrossing: function(startPaneType, endPaneType) {
            return (startPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane && endPaneType == ASPxClientSpreadsheet.PaneManager.PanesType.MainPane) ||
                (startPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane && endPaneType == ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
        },
        
        getVisibleBordersForDiagonalSelection: function(paneType) {
            switch(paneType) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:
                    return { left: true, top: true };
                case ASPxClientSpreadsheet.PaneManager.PanesType.MainPane:
                    return { right: true, bottom: true };
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    return { left: true, bottom: true };
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    return { right: true, top: true };
            }
        },
        getVisibleBordersForCrossSelection: function(showOnTopSide, isHorizontalCrossing) {
            return {
                left: showOnTopSide || !isHorizontalCrossing,
                right: !showOnTopSide || !isHorizontalCrossing,
                top: showOnTopSide || isHorizontalCrossing,
                bottom: !showOnTopSide || isHorizontalCrossing
            };
        },

        hideSelection: function() {
            this.executeMethodForEachPane("hideSelection");
        },
        hideTouchSelectionElements: function(display) {
            this.executeMethodForEachPane("hideTouchSelectionElements", false);
            if(display) {
                var selection = this.getStateController().getSelection(),
                    range = selection.range,
                    leftTopPane = this.getPaneByCell({ col: range.leftColIndex, row: range.topRowIndex }),
                    rightBottomPane = this.getPaneByCell({ col: range.rightColIndex, row: range.bottomRowIndex });
                leftTopPane.hideTouchSelectionElements(true);
                rightBottomPane.hideTouchSelectionElements(true);
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    var rightTopPane = this.getPaneByCell({ col: range.rightColIndex, row: range.topRowIndex }),
                        leftBottomPane = this.getPaneByCell({ col: range.leftColIndex, row: range.bottomRowIndex });
                    rightTopPane.hideTouchSelectionElements(true);
                    leftBottomPane.hideTouchSelectionElements(true);
                }
            }
        },
        moveDataValidationElementsOnScroll: function() {
            this.getOwnerControl().getValidationHelper().onScroll();
        },
        moveTouchSelectionElementsOnScroll: function(selection) {
            this.executeMethodForEachPane("moveTouchSelectionElementsOnScroll", selection);
        },
        drawSelectionMovementRect: function(selection, cellLayoutInfo) {
            this.executeMethodForEachPane("drawSelectionMovementRect", selection, cellLayoutInfo);
        },
        hideSelectionMovementElement: function() {
            this.executeMethodForEachPane("hideSelectionMovementElement");
        },
        getSelectionMovementLayoutInfo: function() {
            return this.getObjectValueSearchInEachPane("getSelectionMovementLayoutInfo");
        },
        isRightBottomTouchSelectionElement: function(element, paneType) {
            var activePane = this.getPaneByType(paneType);
            return activePane.isRightBottomTouchSelectionElement(element);
        }
    });

    ASPxClientSpreadsheet.PaneManager.PanesType = {
        MainPane: "MainPane",
        TopRightPane: "TopRightPane",
        BottomLeftPane: "BottomLeftPane",
        FrozenPane: "FrozenPane"
    };
})();