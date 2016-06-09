(function() {
    ASPxClientSpreadsheet.RenderProvider = ASPx.CreateClass(null, {
        constructor: function(ownerControl) {
            this.ownerControl = ownerControl;
            this.columnHeaderTile = [];
            this.rowHeaderTile = [];
            this.gridTileContainer = [];
        },
        getOwnerControl: function() {
            return this.ownerControl;
        },
        getPaneManager: function() {
            return this.getOwnerControl().getPaneManager();
        },
        getChildControlsPrefix: function(withOutSeparator) {
            return this.getOwnerControl().name + (withOutSeparator ? "" : "_");
        },

        // Model params
        getVisibleRangePaddings: function() {
            return this.getOwnerControl().getVisibleRangePaddings();
        },
        getTileSize: function() {
            return this.getOwnerControl().getTileSize();
        },
        getDefaultCellSize: function() {
            return this.getOwnerControl().getDefaultCellSize();
        },
        adjustFreezePanes: function() {
            var scrollContainer = this.getScrollContainer();

            var gridContainer = this.getGridContainer(),
                columnHeader = this.getColumnHeader(),
                rowHeader = this.getRowHeader(),
                frozenParams = this.getPaneManager().getFrozenPaneSettings();

            var paneType = ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane,
                gridContainer_BL = this.getGridContainer(paneType),
                columnHeader_BL = this.getColumnHeader(paneType);

            paneType = ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane;
            var gridContainer_TR = this.getGridContainer(paneType),
                rowHeader_TR = this.getRowHeader(paneType);

            paneType = ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane;
            var gridContainer_FP = this.getGridContainer(paneType);

            var frozenColumn = !!gridContainer_BL,
                frozenRow = !!gridContainer_TR;

            var gridLeft = ASPx.PxToInt(gridContainer.style.left);
            var gridTop = ASPx.PxToInt(gridContainer.style.top);

            var columnHeaderLeft = ASPx.PxToInt(columnHeader.style.left);
            var rowHeaderTop = ASPx.PxToInt(rowHeader.style.top);

            var headerOffset = this.getPaneManager().getHeaderOffsetSize();

            var offsetWidth = headerOffset.width;
            var offsetHeight = headerOffset.height;

            var headerSizeChanged = columnHeaderLeft !== offsetWidth || rowHeaderTop !== offsetHeight || gridLeft !== offsetWidth || gridTop !== offsetHeight;
            var scrollContainerChanged = scrollContainer.offsetHeight != scrollContainer.parentNode.offsetHeight || scrollContainer.offsetWidth != scrollContainer.parentNode.offsetWidth;
            if(headerSizeChanged || scrollContainerChanged) {
                var workbook = this.getWorkbookControl();
                if(workbook.style.height != "") {
                    var actualHeight = this.getOwnerControl().getMainElementHeightByServerValue();
                    if(actualHeight > 0)
                        workbook.style.height = actualHeight + "px";
                }

                var scrollSize = ASPx.GetVerticalScrollBarWidth(),
                    width = workbook.offsetWidth,
                    height = workbook.offsetHeight,
                    leftRightBorderPaddingsSize = ASPx.GetLeftRightBordersAndPaddingsSummaryValue(workbook);

                if(frozenColumn && frozenRow) {
                    ASPx.SetStyles(gridContainer_FP, {
                        left: this.correctSizeValue(offsetWidth),
                        top: this.correctSizeValue(offsetHeight),
                        width: this.correctSizeValue(frozenParams.width),
                        height: this.correctSizeValue(frozenParams.height)
                    });
                }
                if(frozenColumn) {
                    ASPx.SetStyles(columnHeader_BL, {
                        left: this.correctSizeValue(offsetWidth) - 1,
                        top: 0,
                        width: this.correctSizeValue(frozenParams.width),
                        height: this.correctSizeValue(offsetHeight - 1)
                    });
                    ASPx.SetStyles(gridContainer_BL, {
                        left: this.correctSizeValue(offsetWidth),
                        top: this.correctSizeValue(offsetHeight) + frozenParams.height,
                        width: this.correctSizeValue(frozenParams.width),
                        height: this.correctSizeValue(height - offsetHeight - scrollSize - frozenParams.height)
                    });
                }
                if(frozenRow) {
                    ASPx.SetStyles(gridContainer_TR, {
                        left: this.correctSizeValue(offsetWidth) + frozenParams.width,
                        top: this.correctSizeValue(offsetHeight),
                        width: this.correctSizeValue(width - offsetWidth - scrollSize - leftRightBorderPaddingsSize - frozenParams.width),
                        height: this.correctSizeValue(frozenParams.height)
                    });
                    ASPx.SetStyles(rowHeader_TR, {
                        left: 0,
                        top: this.correctSizeValue(offsetHeight) - 1,
                        width: this.correctSizeValue(offsetWidth - 1),
                        height: this.correctSizeValue(frozenParams.height)
                    });
                }

                ASPx.SetStyles(columnHeader, {
                    left: this.correctSizeValue(offsetWidth + frozenParams.width) - 1,
                    top: 0,
                    width: this.correctSizeValue(width - offsetWidth - scrollSize - leftRightBorderPaddingsSize - frozenParams.width),
                    height: this.correctSizeValue(offsetHeight - 1)
                });
                ASPx.SetStyles(rowHeader, {
                    left: 0,
                    top: this.correctSizeValue(offsetHeight + frozenParams.height) - 1,
                    width: this.correctSizeValue(offsetWidth - 1),
                    height: this.correctSizeValue(height - offsetHeight - scrollSize - frozenParams.height)
                });

                ASPx.SetStyles(gridContainer, {
                    left: this.correctSizeValue(offsetWidth + frozenParams.width),
                    top: this.correctSizeValue(offsetHeight + frozenParams.height),
                    width: this.correctSizeValue(width - offsetWidth - scrollSize - leftRightBorderPaddingsSize - frozenParams.width),
                    height: this.correctSizeValue(height - offsetHeight - scrollSize - frozenParams.height)
                });
                ASPx.SetStyles(scrollContainer, {
                    width: this.correctSizeValue(width - offsetWidth - leftRightBorderPaddingsSize) + this.correctSizeValue(offsetWidth),
                    height: this.correctSizeValue(height - offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(workbook)) + this.correctSizeValue(offsetHeight)
                });
                this.showFreezeBorders(frozenParams);
            }
        },
        adjustControl: function() {
            var scrollContainer = this.getScrollContainer();
            var gridContainer = this.getGridContainer();
            var columnHeader = this.getColumnHeader();
            var rowHeader = this.getRowHeader();

            var gridLeft = ASPx.PxToInt(gridContainer.style.left);
            var gridTop = ASPx.PxToInt(gridContainer.style.top);
            var columnHeaderLeft = ASPx.PxToInt(columnHeader.style.left);
            var rowHeaderTop = ASPx.PxToInt(rowHeader.style.top);

            var headerOffset = this.getPaneManager().getHeaderOffsetSize();

            var offsetWidth = headerOffset.width;
            var offsetHeight = headerOffset.height;

            var headerSizeChanged = columnHeaderLeft !== offsetWidth || rowHeaderTop !== offsetHeight || gridLeft !== offsetWidth || gridTop !== offsetHeight;
            var scrollContainerChanged = scrollContainer.offsetHeight != scrollContainer.parentNode.offsetHeight || scrollContainer.offsetWidth != scrollContainer.parentNode.offsetWidth;
            if(headerSizeChanged || scrollContainerChanged) {
                var mainElement = this.getWorkbookControl();
                if(mainElement.style.height != "") {
                    var actualHeight = this.getOwnerControl().getMainElementHeightByServerValue();
                    if(actualHeight > 0)
                        mainElement.style.height = actualHeight + "px";
                }

                var scrollSize = ASPx.GetVerticalScrollBarWidth();
                var width = mainElement.offsetWidth;
                var height = mainElement.offsetHeight;

                ASPx.SetStyles(columnHeader, {
                    left: this.correctSizeValue(offsetWidth) - 1,
                    top: 0,
                    width: this.correctSizeValue(width - offsetWidth - scrollSize - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement)),
                    height: this.correctSizeValue(offsetHeight - 1)
                });
                ASPx.SetStyles(rowHeader, {
                    left: 0,
                    top: this.correctSizeValue(offsetHeight) - 1,
                    width: this.correctSizeValue(offsetWidth - 1),
                    height: this.correctSizeValue(height - offsetHeight - scrollSize)
                });
                ASPx.SetStyles(gridContainer, {
                    left: this.correctSizeValue(offsetWidth),
                    top: this.correctSizeValue(offsetHeight),
                    width: this.correctSizeValue(width - offsetWidth - scrollSize - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement)),
                    height: this.correctSizeValue(height - offsetHeight - scrollSize)
                });
                ASPx.SetStyles(scrollContainer, {
                    width: this.correctSizeValue(width - offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement)) + this.correctSizeValue(offsetWidth),
                    height: this.correctSizeValue(height - offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainElement)) + this.correctSizeValue(offsetHeight)
                });
            }
        },

        correctSizeValue: function(value) {
            return value > 0 ? value : 0;
        },

        // Freeze Borders Elements
        hideFreezeBorders: function() {
            ASPx.SetElementDisplay(this.getHorizontalFreezeBorder(), false);
            ASPx.SetElementDisplay(this.getVerticalFreezeBorder(), false);
        },
        showFreezeBorders: function(settings) {
            var mode = settings.mode,
                width = settings.width,
                height = settings.height,
                hbe = this.getHorizontalFreezeBorder(),
                vbe = this.getVerticalFreezeBorder(),
                sizes = this.getPaneManager().getVisibleAreaSize(),
                headerOffsetSize = this.getPaneManager().getHeaderOffsetSize();

            if(mode === 0 || mode === 2) {
                hbe.style.width = sizes.width + "px";
                hbe.style.left = "0px";
                hbe.style.top = height + headerOffsetSize.height - 1 + "px";
                hbe.style.height = "1px";
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(hbe, this.getWorkbookControl());
                ASPx.SetElementDisplay(hbe, true);
            }
            if(mode === 1 || mode === 2) {
                vbe.style.height = sizes.height + "px";
                vbe.style.left = width + headerOffsetSize.width - 1 + "px";
                vbe.style.top = "0px";
                vbe.style.width = "1px";
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(vbe, this.getWorkbookControl());
                ASPx.SetElementDisplay(vbe, true);
            }
        },
        getVerticalFreezeBorder: function() {
            if(!this.verticalFreezeBorder)
                this.verticalFreezeBorder = this.createFreezeBorderElement(true);
            return this.verticalFreezeBorder;
        },
        getHorizontalFreezeBorder: function() {
            if(!this.horizontalFreezeBorder)
                this.horizontalFreezeBorder = this.createFreezeBorderElement(false);
            return this.horizontalFreezeBorder;
        },
        createFreezeBorderElement: function(isVertical) {
            var element = document.createElement("DIV");
            element.className = isVertical ? "dxss-vfbe" : "dxss-hfbe";
            return element;
        },

        // Child controls
        GetMainElement: function() {
            return this.getOwnerControl().GetMainElement();
        },
        // TODO rename to CONTROL
        getWorkbookControl: function() {
            return this.getOwnerControl().GetChildElement(ASPxClientSpreadsheet.ChildControlIdPostfixes.workbookPostfix);
        },
        getColumnHeader: function(paneType) {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.columnHeaderDivPostfix + this.getPanePostfixByType(paneType));
        },
        getRowHeader: function(paneType) {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.rowHeaderDivPostfix + this.getPanePostfixByType(paneType));
        },
        getGridContainer: function(paneType) {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.gridContainerDivPostfix + this.getPanePostfixByType(paneType));
        },
        getScrollContainer: function() {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.scrollContainerDivPostfix);
        },
        getScrollContent: function() {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.scrollContentDivPostfix);
        },
        getColumnHeaderTilesContainer: function(paneType) {
            if(!this.columnHeaderTile[paneType])
                this.columnHeaderTile[paneType] = ASPx.GetNodeByClassName(this.getColumnHeader(paneType), ASPx.SpreadsheetCssClasses.TileContainer);
            return this.columnHeaderTile[paneType];
        },
        getHeaderTileElementByCellIndex: function(cellIndex, tileIndex, isCol) {
            var paneType = this.getPaneManager().getPaneTypeByCell(isCol ? { col: cellIndex, row: 0 } : { col: 0, row: cellIndex });
            if(paneType === ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane)
                paneType = isCol ? ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane : ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane;
            else
                paneType = ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
            return this.getHeaderTileElement(tileIndex, isCol, paneType);
        },
        resetColumnHeaderTileContainer: function(paneType) {
            this.columnHeaderTile[paneType] = null;
        },
        getRowHeaderTilesContainer: function(paneType) {
            if(!this.rowHeaderTile[paneType])
                this.rowHeaderTile[paneType] = ASPx.GetNodeByClassName(this.getRowHeader(paneType), ASPx.SpreadsheetCssClasses.TileContainer);
            return this.rowHeaderTile[paneType];
        },
        resetRowHeaderTilesContainer: function(paneType) {
            this.rowHeaderTile[paneType] = null;
        },
        getGridTilesContainer: function(paneType) {
            if(!this.gridTileContainer[paneType])
                this.gridTileContainer[paneType] = ASPx.GetNodeByClassName(this.getGridContainer(paneType), ASPx.SpreadsheetCssClasses.TileContainer);
            return this.gridTileContainer[paneType];
        },
        resetGridTilesContainer: function(paneType) {
            this.gridTileContainer[paneType] = null;
        },
        getFormulaBar: function() {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.formulaBar);
        },
        getFormulaBarTextBoxElement: function() {
            return ASPx.GetElementById(this.getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.formulaBarTextBox);
        },
        getFormulaBarEditorPlaceholder: function() {
            var formulaBarTextBoxElement = this.getFormulaBarTextBoxElement();
            return ASPx.IsExists(formulaBarTextBoxElement) ? ASPx.GetNodeByTagName(formulaBarTextBoxElement, "INPUT") : null;
            // TODO - probably it is better to find input by class name
        },

        getCellElementByModelPosition: function(colModelIndex, rowModelIndex) {
            var documentContainer = this.getWorkbookControl(),
                paneType = this.getPaneManager().getPaneTypeByCell({ col: colModelIndex, row: rowModelIndex });
            return documentContainer ? ASPx.GetChildById(documentContainer, this.getChildControlsPrefix(true) + this.getPanePostfixByType(paneType) + '_ctb.' + colModelIndex + '.' + rowModelIndex) : null;
        },
        getCellElementByVisiblePosition: function(colVisibleIndex, rowVisibleIndex) {
            var cellModelPosition = this.getPaneManager().convertVisiblePositionToModelPosition(colVisibleIndex, rowVisibleIndex);
            return this.getCellElementByModelPosition(cellModelPosition.colIndex, cellModelPosition.rowIndex);
        },
        getCellTextElement: function(colVisibleIndex, rowVisibleIndex) {
            var cellContainer = this.getCellElementByVisiblePosition(colVisibleIndex, rowVisibleIndex);
            return cellContainer ? ASPx.GetNodeByClassName(cellContainer, ASPx.SpreadsheetCssClasses.TextBoxContent) : null;
        },
        getCellTextElementByModelPosition: function(colModelIndex, rowModelIndex) {
            var cellContainer = this.getCellElementByModelPosition(colModelIndex, rowModelIndex);
            return cellContainer ? ASPx.GetNodeByClassName(cellContainer, ASPx.SpreadsheetCssClasses.TextBoxContent) : null;
        },
        getActiveCellElement: function() {
            var cellModelPosition = this.getOwnerControl().getActiveCellModelPosition();
            return this.getCellElementByModelPosition(cellModelPosition.colIndex, cellModelPosition.rowIndex);
        },
        getIsTouchSelectionElement: function(element) {
            return ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.TouchSelectionElement);
        },
        getIsSelectionMovementBorderElement: function(element) {
            return ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.SelectionMovementBorderElement) ||
                ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.TopRangeBorderElement) ||
                ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.RightRangeBorderElement) ||
                ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.BottomRangeBorderElement) ||
                ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.LeftRangeBorderElement);
        },
        getIsCellEditingElement: function(element) {
            return ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.CellEditorElement);
        },
        getIsDropDownButtonElement: function(element) {
            return ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.DropDownButtonImage);
        },
        isLoadTilesRequired: function(cachedScrollLeft, cachedScrollTop) {
            var scrollDiv = this.getScrollContainer();
            if(cachedScrollTop > scrollDiv.scrollHeight - ASPx.GetDocumentClientHeight() || cachedScrollLeft > scrollDiv.scrollWidth - ASPx.GetDocumentClientWidth())
                return true;
            return !this.getPaneManager().tryLoadTilesFromCache();
        },
        getGridOffsetSize: function(paneType) {  // TODO use cached value from spreadsheet control
            var grid = this.getGridContainer(paneType);
            return { width: ASPx.PxToInt(grid.style.width), height: ASPx.PxToInt(grid.style.height) };
        },

        getHeaderTileElementId: function(index, isColumn, paneType) { return this.getChildControlsPrefix(true) + this.getPanePostfixByType(paneType) + "_h" + (isColumn ? "c" : "r") + index; },
        getHeaderTileElement: function(index, isColumn, paneType) { return ASPx.GetElementById(this.getHeaderTileElementId(index, isColumn, paneType)); },

        getGridTileElementId: function(rowIndex, colIndex, paneType) { return this.getChildControlsPrefix(true) + this.getPanePostfixByType(paneType) + "_r" + rowIndex + "c" + colIndex; },
        getGridTileElement: function(rowIndex, colIndex, paneType) { return ASPx.GetElementById(this.getGridTileElementId(rowIndex, colIndex, paneType)); },

        getGridTileRowElementId: function(rowIndex, paneType) { return this.getChildControlsPrefix(true) + this.getPanePostfixByType(paneType) + "_r" + rowIndex; },
        getGridTileRowElement: function(rowIndex, paneType) { return ASPx.GetElementById(this.getGridTileRowElementId(rowIndex, paneType)); },

        getHeaderCellElementIndex: function(headerCellElement, isCol) {
            var cellContainer = ASPx.GetParentByClassName(headerCellElement, ASPx.SpreadsheetCssClasses.HeaderContainer);
            var tileIndex = parseInt(cellContainer.getAttribute(ASPxClientSpreadsheet.TileHelper.DataAttributes.HeaderContainerTileIndex));

            var cellIndex = parseInt(headerCellElement.getAttribute(ASPxClientSpreadsheet.TileHelper.DataAttributes.HeaderCellIndex));
            var tileSize = isCol ? this.getTileSize().col : this.getTileSize().row;
            return tileIndex * tileSize + cellIndex;
        },

        prepareControlHierarchy: function(paneType) {
            if(paneType === ASPxClientSpreadsheet.PaneManager.PanesType.MainPane)
                return;

            if(!this.getGridContainer(paneType))
                this.createChildControls(this.getPanePostfixByType(paneType), paneType);
        },
        createChildControls: function(postfix, paneType) {
            var columnHeader = this.getColumnHeader(),
               rowHeader = this.getRowHeader(),
               gridContainer = this.getGridContainer(),
               workbook = this.getWorkbookControl();

            if(paneType === ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane) {
                var clonedColumnHeader = columnHeader.cloneNode(true);
                clonedColumnHeader.id = clonedColumnHeader.id + postfix;
                clonedColumnHeader.style.borderRight = "none"
                this.removeChilds(clonedColumnHeader);
                workbook.insertBefore(clonedColumnHeader, columnHeader);
            }

            if(paneType === ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane) {
                var clonedRowHeader = rowHeader.cloneNode(true);
                clonedRowHeader.id = clonedRowHeader.id + postfix;
                clonedRowHeader.style.borderBottom = "none";
                this.removeChilds(clonedRowHeader);
                workbook.insertBefore(clonedRowHeader, columnHeader);
            }

            var clonedGridContainer = gridContainer.cloneNode(true);
            clonedGridContainer.id = clonedGridContainer.id + postfix;
            this.prepareStyles(clonedGridContainer, paneType);
            this.removeChilds(clonedGridContainer);
            workbook.insertBefore(clonedGridContainer, columnHeader);
        },
        removeChilds: function(parentContainer) {
            var tileContainer = ASPx.GetNodeByClassName(parentContainer, ASPx.SpreadsheetCssClasses.TileContainer);
            if(tileContainer)
                while(tileContainer.firstChild)
                    tileContainer.removeChild(tileContainer.firstChild);
        },
        prepareStyles: function(container, paneType) {
            if(paneType === ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane || paneType === ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane)
                container.style.borderRight = "none";
            if(paneType === ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane || paneType === ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane)
                container.style.borderBottom = "none";
        },
        getPanePostfixByType: function(paneType) {
            var panePostfix = "";
            switch(paneType) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    panePostfix = "_" + ASPxClientSpreadsheet.ChildControlIdPostfixes.topRightPanePostfix;
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    panePostfix = "_" + ASPxClientSpreadsheet.ChildControlIdPostfixes.bottomLeftPanePostfix;
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:
                    panePostfix = "_" + ASPxClientSpreadsheet.ChildControlIdPostfixes.frozenPanePostfix;
                    break;
            }
            return panePostfix;
        },

        getInputTargetElement: function() {
            if(ASPx.Browser.Firefox) {
                this.removeIFrameElement(ASPxClientSpreadsheet.ChildControlIdPostfixes.inputTargetElement);
                this.inputTargetElement = null;
            }

            if(!this.inputTargetElement)
                this.inputTargetElement = this.createInputTargetFrameElement();

            return this.inputTargetElement;
        },
        createInputTargetFrameElement: function() {
            var frameElement = document.createElement("iframe");
            frameElement.id = this.getOwnerControl().name + ASPxClientSpreadsheet.ChildControlIdPostfixes.inputTargetElement;
            frameElement.className = ASPx.SpreadsheetCssClasses.InputTargetFrameElement;
            frameElement.src = "javascript:false;";
            if(ASPx.Browser.WebKitFamily) frameElement.scrolling = "no";

            frameElement.name = this.getOwnerControl().name + ASPxClientSpreadsheet.ChildControlIdPostfixes.inputTargetElement;
            this.getOwnerControl().GetMainElement().appendChild(frameElement);
            return this.getOwnerControl().GetChildElement(ASPxClientSpreadsheet.ChildControlIdPostfixes.inputTargetElement);
        },

        getSupportFrameElement: function() {
            if(ASPx.Browser.Firefox) {
                this.removeIFrameElement(ASPxClientSpreadsheet.ChildControlIdPostfixes.supportFrame);
                this.supportFrameElement = null;
            }

            if(!this.supportFrameElement)
                this.supportFrameElement = this.createSupportFrameElement();

            return this.supportFrameElement;
        },
        createSupportFrameElement: function() {
            var supportFrame = document.createElement("iframe");
            var frameSize = ASPx.Browser.Safari ? "1px" : "0px";
            supportFrame.style.width = frameSize;
            supportFrame.style.height = frameSize;
            supportFrame.name = this.getOwnerControl().name + ASPxClientSpreadsheet.ChildControlIdPostfixes.supportFrame;
            supportFrame.id = this.getOwnerControl().name + ASPxClientSpreadsheet.ChildControlIdPostfixes.supportFrame;
            supportFrame.className = ASPx.SpreadsheetCssClasses.SupportFrameElement;
            this.getOwnerControl().GetMainElement().appendChild(supportFrame);
            return window.frames[supportFrame.name];
        },

        removeIFrameElement: function(iframeName) {
            var frameElement = this.getOwnerControl().GetChildElement(iframeName);
            if(frameElement)
                this.getOwnerControl().GetMainElement().removeChild(frameElement);

            try {
                delete window.frames[this.getOwnerControl().name + iframeName];
            } catch(e) { }
        }
    });
})();