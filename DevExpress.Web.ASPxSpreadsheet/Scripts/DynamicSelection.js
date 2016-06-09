(function() {
    var RESIZE_MARKER_WIDTH = 3,
        RESIZE_MARKER_HEIGHT = 3,
        DYNAMIC_COLORS_COUNT = 7;

    ASPxClientSpreadsheet.DynamicSelection = ASPx.CreateClass(null, {
        constructor: function(spreadsheet, dynamicColorIndex, mouseDownHandler) {
            this.spreadsheet = spreadsheet;
            this.resizeMarkers = [ ];
            this.dynamicColorIndex = dynamicColorIndex;            
            this.paneManager = spreadsheet.getPaneManager();
            this.mouseDownHandler = mouseDownHandler;
        },

        render: function(selection) {
            this.selectionParts = [];
            this.resizeMarkers = [];

            this.range = selection.range;

            var visibleSelection = this.convertToVisible(selection);

            this.paneManager.correctSelection(visibleSelection);

            ASPx.Data.ForEach(this.getSelectionPanes(visibleSelection), function(paneType) {
                this.renderSelectionInPane(paneType, selection.clone());
            }.aspxBind(this));
        },
        convertToVisible: function(selection) {
            var convertedSelection = selection.clone();

            convertedSelection.range = this.paneManager.convertModelRangeToVisible(convertedSelection.range);

            return convertedSelection;
        },
        renderSelectionInPane: function(paneType, selection) {
            var pane = this.paneManager.getPaneByType(paneType);

            selection.range = this.paneManager.convertModelRangeToVisible(selection.range);

            var tileInfo = this.getTileInfoForRange(selection.range, pane);

            if(!tileInfo) return;

            this.correctSelectionRangeForPane(selection, paneType);

            var selectionElement = this.renderSelectionRectangle(selection);
            var selectionRect = this.getSelectionRect(selection, pane, tileInfo);

            this.drawElement(selectionElement, tileInfo.htmlElement, selectionRect);
            this.drawResizingMarkers(selection, tileInfo.htmlElement, selectionRect);

            this.selectionParts.push(selectionElement);
        },
        getTileInfoForRange: function(range, pane) {
            var paneType = pane.getPaneType(),
                cell;

            switch(paneType) {
                case ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane:
                    cell = { col: range.leftColIndex, row: range.topRowIndex };
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane:
                    cell = { col: range.rightColIndex, row: range.topRowIndex };
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane:
                    cell = { col: range.leftColIndex, row: range.bottomRowIndex };
                    break;
                case ASPxClientSpreadsheet.PaneManager.PanesType.MainPane:
                    cell = { col: range.rightColIndex, row: range.bottomRowIndex };
                    break;
            }

            return pane.getCellParentTileInfo(cell);
        },
        correctSelectionRangeForPane: function(selection, paneType) {
            selection.range.leftColIndex   = this.paneManager.putCellColIndexInVisibleRange(selection.range.leftColIndex, paneType);
            selection.range.topRowIndex    = this.paneManager.putCellRowIndexInVisibleRange(selection.range.topRowIndex, paneType);
            selection.range.rightColIndex  = this.paneManager.putCellColIndexInVisibleRange(selection.range.rightColIndex, paneType);
            selection.range.bottomRowIndex = this.paneManager.putCellRowIndexInVisibleRange(selection.range.bottomRowIndex, paneType);
        },
        getSelectionRect: function(selection, pane, parentTileInfo) {
            var topLeftLayoutInfo = pane.getCellLayoutInfo(selection.range.leftColIndex, selection.range.topRowIndex),
                bottomRightLayoutInfo = pane.getCellLayoutInfo(selection.range.rightColIndex, selection.range.bottomRowIndex);
            var selectionRect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftLayoutInfo, bottomRightLayoutInfo, parentTileInfo),
                borderCorrectionFactor = Math.floor(ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor / 2);

            selectionRect.width -= borderCorrectionFactor;
            selectionRect.height -= borderCorrectionFactor;
            selectionRect.x -= borderCorrectionFactor;
            selectionRect.y -= borderCorrectionFactor;

            return selectionRect;
        },
        renderSelectionRectangle: function(selection) {
            var isHighlighted = selection.range.isHighlighted;

            return createDynamicSelectionElement(this.dynamicColorIndex, isHighlighted ? "highlighted" : "");
        },
        getSelectionPanes: function(selection) {
            var startPaneType = selection.range.startPaneType,
                endPaneType = selection.range.endPaneType,
                startEndPanes = [startPaneType, endPaneType],
                panes = [];

            var inTopHalf =     ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane) ||
                                ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);

            var inBottomHalf =  ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane) ||
                                ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.MainPane);

            var inLeftHalf =    ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane) ||
                                ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);

            var inRightHalf =   ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane) ||
                                ASPx.Data.ArrayContains(startEndPanes, ASPxClientSpreadsheet.PaneManager.PanesType.MainPane);

            if(inTopHalf && inLeftHalf)
                panes.push(ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane);
            if(inTopHalf && inRightHalf)
                panes.push(ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane);
            if(inBottomHalf && inLeftHalf)
                panes.push(ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane);
            if(inBottomHalf && inRightHalf)
                panes.push(ASPxClientSpreadsheet.PaneManager.PanesType.MainPane);

            return panes;
        },
        setHighlighted: function(highlighted) {
            ASPx.Data.ForEach(this.selectionParts, function(selectionPart) {
                if(!!highlighted)
                    ASPx.AddClassNameToElement(selectionPart, "highlighted");
                else
                    ASPx.RemoveClassNameFromElement(selectionPart, "highlighted");
            });
        },

        drawResizingMarkers: function(selection, tileElement, selectionRect) {
            var locations = ["nw", "ne", "sw", "se"],
                that = this;

            ASPx.Data.ForEach(locations, function(location) {
                var marker = that.renderResizeMarker(selection, selectionRect, location);

                that.drawElement(marker.element, tileElement, marker.rect);
                that.resizeMarkers.push(marker.element);
            });
        },
        renderResizeMarker: function(selection, selectionRect, location) {
            var markerElement = createDynamicSelectionElement(this.dynamicColorIndex, "marker " + location),
                markerRect = this.getMarkerElementRect(selectionRect),
                that = this;

            ASPx.Evt.AttachEventToElement(markerElement, "mousedown", function() {
                that.mouseDownHandler(that);
            }, true);

            switch(location) {
                case "ne":
                    markerRect.x += selectionRect.width - RESIZE_MARKER_WIDTH;
                    break;
                case "sw":
                    markerRect.y += selectionRect.height - RESIZE_MARKER_HEIGHT;
                    break;
                case "se":
                    markerRect.x += selectionRect.width - RESIZE_MARKER_WIDTH;
                    markerRect.y += selectionRect.height - RESIZE_MARKER_HEIGHT;
                    break;
            }

            return { element: markerElement, rect: markerRect };
        },

        getMarkerElementRect: function(selectionRect) {
            return {
                x: selectionRect.x,
                y: selectionRect.y,
                width: RESIZE_MARKER_WIDTH,
                height: RESIZE_MARKER_HEIGHT
            };
        },

        drawElement: function(element, parentElement, selectionRect) {
            ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(element, parentElement);
            ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(element, selectionRect);
        },

        dispose: function() {
            ASPx.Data.ForEach(this.selectionParts, function(selectionPart) {
                ASPx.RemoveElement(selectionPart);
            }.aspxBind(this));

            ASPx.Data.ForEach(this.resizeMarkers, function(marker) {
                ASPx.RemoveElement(marker)
            });
        }
    });

    function createDynamicSelectionElement(colorNumber, customClass) {
        var element = document.createElement("DIV");
        element.className = ASPx.SpreadsheetCssClasses.CellDynamicSelectionElement + " color" + colorNumber + (
            customClass ? " " + customClass : ""
            );
        return element;
    }

    var originalEditorContentLeft,
        originalEditorContentRight,
        activeDynamicSelection,
        caretPosition,
        isMarkerEditing,
        currentCellLayoutInfo,
        activeRangeTextLength,
        startCellLayoutInfo,

        processingAllowed;

    ASPxClientSpreadsheet.DynamicSelectionHelper = ASPx.CreateClass(null, {
        constructor: function(spreadsheet) {
            this.dynamicSelections = [ ];
            this.spreadsheet = spreadsheet;
            this.selectionHelper = this.spreadsheet.getSelectionHelper();
        },

        attach: function(editorElement, sheetNameMode) {
            this.editorElement = editorElement;
            this.sheetNameMode = sheetNameMode;
            this.render();
        },

        detach: function() {
            this.sheetNameMode = false;
            this.clean();
        },

        activate: function(cellLayoutInfo) {
            var that = this;

            processingAllowed = false;

            setTimeout(function() {
                if(isMarkerEditing) {
                    startCellLayoutInfo = startCellLayoutInfo || cellLayoutInfo;
                    that.processMarkerMove(cellLayoutInfo);
                } else {
                    startCellLayoutInfo = cellLayoutInfo;
                }

                var textPosition = activeDynamicSelection ? activeDynamicSelection.range.textPosition : { start: getCursorPosition(that.editorElement), length: 0 };

                originalEditorContentLeft = that.editorElement.value.substr(0, textPosition.start);
                originalEditorContentRight = that.editorElement.value.substr(textPosition.start + textPosition.length);
                caretPosition = textPosition.start;

                processingAllowed = true;

                that.updateActiveSelection(cellLayoutInfo);
            }, 0);
        },

        deactivate: function() {
            ASPx.Selection.SetCaretPosition(this.editorElement, caretPosition + activeRangeTextLength);
            this.editorElement.focus();

            isMarkerEditing = false;
            caretPosition = 0;
            originalEditorContentLeft = "";
            originalEditorContentRight = "";
            activeRangeTextLength = 0;
            currentCellLayoutInfo = null;
        },

        render: function() {
            var selections = this.parseFormula(),
                dynamicSelection,
                colorIndex = 0,
                that = this;

            this.dynamicSelections = [ ];
            activeDynamicSelection = null;

            ASPx.Data.ForEach(selections, function(selection) {
                dynamicSelection = new ASPxClientSpreadsheet.DynamicSelection(that.spreadsheet, colorIndex++, markerMouseDownHandler);
                selection.range = that.selectionHelper.ExpandRangeToMergedCellSize(selection.range);

                dynamicSelection.render(selection);
                that.dynamicSelections.push(dynamicSelection);

                if(selection.range.isHighlighted) {
                    activeDynamicSelection = dynamicSelection;
                }

                if(colorIndex === DYNAMIC_COLORS_COUNT) {
                    colorIndex = 0;
                }
            });
        },

        refresh: function() {
            this.clean();
            this.render()
        },

        parseFormula: function() {
            this.editorElement = this.editorElement || this.spreadsheet.getEditingHelper().getEditorElement();
            var formulaCaretPosition = caretPosition || getCursorPosition(this.editorElement),
                formula = this.editorElement.value,
                selections = [ ];

            if(formula && (formula[0] === "=" || this.sheetNameMode)) {
                selections = ASPx.SpreadsheetFormulaParser.parse(formula, this.spreadsheet.currentActiveCell, this.spreadsheet.getCurrentSheetName(), formulaCaretPosition);
            }
            return selections;
        },

        updateActiveSelection: function(cellLayoutInfo) {
            this.processCellSelection(cellLayoutInfo);
        },

        processMarkerMove: function(cellLayoutInfo) {
            var modelRange = activeDynamicSelection.range;

            currentCellLayoutInfo = cellLayoutInfo;

            this.correctMarkerCellLayoutInfo(cellLayoutInfo, modelRange);
            this.correctMarkerStartCellLayoutInfo(cellLayoutInfo, modelRange);
        },
        correctMarkerCellLayoutInfo: function(cellLayoutInfo, modelRange) {
            if(cellLayoutInfo.colIndex < modelRange.leftColIndex)
                cellLayoutInfo.colIndex = modelRange.leftColIndex;
            if(cellLayoutInfo.colIndex > modelRange.rightColIndex)
                cellLayoutInfo.colIndex = modelRange.rightColIndex;

            if(cellLayoutInfo.rowIndex < modelRange.topRowIndex)
                cellLayoutInfo.rowIndex = modelRange.topRowIndex;
            if(cellLayoutInfo.rowIndex > modelRange.bottomRowIndex)
                cellLayoutInfo.rowIndex = modelRange.bottomRowIndex;
        },
        correctMarkerStartCellLayoutInfo: function(cellLayoutInfo, modelRange) {
            if(modelRange.leftColIndex === cellLayoutInfo.colIndex)
                startCellLayoutInfo.colIndex = modelRange.rightColIndex;
            else
                startCellLayoutInfo.colIndex = modelRange.leftColIndex;

            if(modelRange.topRowIndex === cellLayoutInfo.rowIndex)
                startCellLayoutInfo.rowIndex = modelRange.bottomRowIndex;
            else
                startCellLayoutInfo.rowIndex = modelRange.topRowIndex;
        },

        processCellSelection: function(cellLayoutInfo) {
            if(!startCellLayoutInfo || !processingAllowed) {
                return;
            }
            currentCellLayoutInfo = cellLayoutInfo;

            var rangeText = getRangeText(currentCellLayoutInfo, startCellLayoutInfo);

            if(this.sheetNameMode) {
                this.editorElement.value = originalEditorContentLeft + this.spreadsheet.getCurrentSheetName() + "!" + rangeText + originalEditorContentRight;
            } else {
                if(originalEditorContentLeft === "")
                    rangeText = "=" + rangeText;
                this.editorElement.value = originalEditorContentLeft + rangeText + originalEditorContentRight;
            }
            ASPx.Evt.DispatchEvent(this.editorElement, "input", false, false);

            activeRangeTextLength = rangeText.length;

            this.refresh();
        },

        clean: function() {
            var selection;

            while(selection = this.dynamicSelections.pop()) {
                selection.dispose();
            }
        }
    });

    function markerMouseDownHandler(dynamicSelection) {
        activeDynamicSelection && activeDynamicSelection.setHighlighted(false);
        activeDynamicSelection = dynamicSelection;
        activeDynamicSelection.setHighlighted(true);

        isMarkerEditing = true;
        processingAllowed = false;
    }

    // TODO [Seleznyov] isn't this function the same as ASPxClientSpreadsheet.CellPositionConvertor.getStringRepresentaion?
    function getColumnName(columnIndex) {
        var result = "",
            radix = 26,
            firstCharCode = 65;

        while(columnIndex >= 0) {
            result = String.fromCharCode(columnIndex % radix + firstCharCode) + result;
            columnIndex = parseInt(columnIndex / radix) - 1;
        }

        return result;
    }
    //TODO to Zhuravlev or Ushkal: same function in utils.js  - Selection.GetCaretPosition
    function getCursorPosition(element) {
        var pos = 0;

        if("selectionStart" in element) {
            pos = element.selectionStart;
        } else if ("selection" in document) {
            element.focus();
            var sel = document.selection.createRange(),
                selLength = document.selection.createRange().text.length;

            sel.moveStart("character", -element.value.length);
            pos = sel.text.length - selLength;
        }

        return pos;
    }

    function getRangeText(currentCellLayoutInfo, startCellLayoutInfo) {
        if(currentCellLayoutInfo.colIndex === startCellLayoutInfo.colIndex &&
            currentCellLayoutInfo.rowIndex === startCellLayoutInfo.rowIndex) {
            return getColumnName(currentCellLayoutInfo.colIndex) +
                (currentCellLayoutInfo.rowIndex + 1);
        }
        else {
            var indexes = getStartEndIndexes(currentCellLayoutInfo, startCellLayoutInfo);

            return getColumnName(indexes.startCol) +
                (indexes.startRow + 1) + ":" +
                getColumnName(indexes.endCol) +
                (indexes.endRow + 1);
        }
    }
    function getStartEndIndexes(currentCLI, startCLI) {
        return {
            startCol: startCLI.colIndex < currentCLI.colIndex ? startCLI.colIndex : currentCLI.colIndex,
            startRow: startCLI.rowIndex < currentCLI.rowIndex ? startCLI.rowIndex : currentCLI.rowIndex,
            endCol: startCLI.colIndex > currentCLI.colIndex ? startCLI.colIndex : currentCLI.colIndex,
            endRow: startCLI.rowIndex > currentCLI.rowIndex ? startCLI.rowIndex : currentCLI.rowIndex
        }
    }


    ASPxClientSpreadsheet.DynamicSelectionHelper.getRangeText = getRangeText;
})();