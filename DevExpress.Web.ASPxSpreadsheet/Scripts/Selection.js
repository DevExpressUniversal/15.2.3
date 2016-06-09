(function() {
    ASPxClientSpreadsheet.Range = (function() {
        var Range = function(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex) {
            this.set(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex);
        };
        Range.prototype = {
            set: function(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex) {
                if(typeof(rightColIndex) == "undefined") {
                    this.leftColIndex = this.rightColIndex = leftColIndex;
                    this.topRowIndex = this.bottomRowIndex = topRowIndex;
                    this.singleCell = true;
                } else {
                    if(leftColIndex < rightColIndex) {
                        this.leftColIndex = leftColIndex;
                        this.rightColIndex = rightColIndex;
                    } else {
                        this.leftColIndex = rightColIndex;
                        this.rightColIndex = leftColIndex;
                    }

                    if(topRowIndex < bottomRowIndex) {
                        this.topRowIndex = topRowIndex;
                        this.bottomRowIndex = bottomRowIndex;
                    } else {
                        this.topRowIndex = bottomRowIndex;
                        this.bottomRowIndex = topRowIndex;
                    }
                    this.updateIsSingleCell();
                }
            },
            clone: function() {
                var clone = new ASPxClientSpreadsheet.Range(this.leftColIndex, this.topRowIndex, this.rightColIndex, this.bottomRowIndex);

                clone.isHighlighted = this.isHighlighted;
                clone.textPosition = this.textPosition;
                clone.startPaneType = this.startPaneType;
                clone.endPaneType = this.endPaneType;

                return clone;
            },
            equals: function(range) {
                return this.leftColIndex == range.leftColIndex &&
                    this.topRowIndex == range.topRowIndex &&
                    this.rightColIndex == range.rightColIndex &&
                    this.bottomRowIndex == range.bottomRowIndex;
            },
            updateIsSingleCell: function() {
                this.singleCell = this.leftColIndex == this.rightColIndex && this.topRowIndex == this.bottomRowIndex;
            },
            isSingleCell: function() {
                return this.singleCell;
            },
            isCellInRange: function(colIndex, rowIndex) {
                return this.isCellInRangeCore(colIndex, true) &&
                    this.isCellInRangeCore(rowIndex, false);
            },
            isCellInRangeCore: function(cellIndex, isCol) {
                if(isCol)
                    return this.leftColIndex <= cellIndex && cellIndex <= this.rightColIndex;
                return this.topRowIndex <= cellIndex && cellIndex <= this.bottomRowIndex;
            },
            containsRange: function(range) {
                return range.leftColIndex >= this.leftColIndex && range.rightColIndex <= this.rightColIndex &&
                        range.topRowIndex >= this.topRowIndex  && range.bottomRowIndex <= this.bottomRowIndex;
            },
            intersectsRange: function(range) {
                return range.topRowIndex <= this.bottomRowIndex && range.bottomRowIndex >= this.topRowIndex &&
                       range.leftColIndex <= this.rightColIndex && range.rightColIndex >= this.leftColIndex;
            },
            convertToVisibleIndices: function(paneManager) {
                var modelColRange = paneManager.convertModelIndicesRangeToVisibleIndices(
                    this.leftColIndex,
                    this.rightColIndex,
                    true);

                var modelRowRange = paneManager.convertModelIndicesRangeToVisibleIndices(
                    this.topRowIndex,
                    this.bottomRowIndex,
                    false);

                this.leftColIndex = modelColRange.visibleTopLeftIndex;
                this.rightColIndex = modelColRange.visibleBottomRightIndex;
                this.topRowIndex = modelRowRange.visibleTopLeftIndex;
                this.bottomRowIndex = modelRowRange.visibleBottomRightIndex;
            },
            getConvertedCore: function(converter) {
                var convertedRange = this.clone();

                convertedRange.leftColIndex = converter(convertedRange.leftColIndex, true);
                convertedRange.topRowIndex = converter(convertedRange.topRowIndex, false);

                if(convertedRange.isSingleCell()) {
                    convertedRange.rightColIndex = convertedRange.leftColIndex;
                    convertedRange.bottomRowIndex = convertedRange.topRowIndex;
                } else {
                    convertedRange.rightColIndex = converter(convertedRange.rightColIndex, true);
                    convertedRange.bottomRowIndex = converter(convertedRange.bottomRowIndex, false);
                }

                return convertedRange;
            },
            correct: function() {
                if(this.leftColIndex < 0)
                    this.leftColIndex = 0;
                if(this.topRowIndex < 0)
                    this.topRowIndex = 0;
                if(this.rightColIndex < 0)
                    this.rightColIndex = 0;
                if(this.bottomRowIndex < 0)
                    this.bottomRowIndex = 0;
            },
            correctPanes: function() {
                if((this.endPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane) ||
                    (this.endPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane && this.startPaneType !== ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane) ||
                    (this.endPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane && this.startPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.MainPane)) {
                    var buffer = this.endPaneType;
                    this.endPaneType = this.startPaneType;
                    this.startPaneType = buffer;
                }
                if((this.endPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane && this.startPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane) ||
                    (this.endPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.BottomLeftPane && this.startPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.TopRightPane)) {
                    this.endPaneType = ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
                    this.startPaneType = ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane;
                }
                if(this.endPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.MainPane && this.startPaneType === ASPxClientSpreadsheet.PaneManager.PanesType.FrozenPane)
                    this.diagonalSelection = true;
            },
            setStartPaneType: function(startPaneType) {
                this.startPaneType = startPaneType;
            },
            setEndPaneType: function(endPaneType) {
                this.endPaneType = endPaneType;
            }
        };
        return Range;
    })();

    var DYNAMIC_COLORS_COUNT = 7;
    //experimental value obtained from MSExcel!
    ASPxClientSpreadsheet.Range.MAX_ROW_COUNT = 1048576;
    ASPxClientSpreadsheet.Range.MAX_COL_COUNT = 16384;
    ASPxClientSpreadsheet.Range.MIN_ROW_INDEX = 0;
    ASPxClientSpreadsheet.Range.MIN_COL_INDEX = 0;
    ASPxClientSpreadsheet.Range.MAX_ROW_INDEX = ASPxClientSpreadsheet.Range.MAX_ROW_COUNT - 1;
    ASPxClientSpreadsheet.Range.MAX_COL_INDEX = ASPxClientSpreadsheet.Range.MAX_COL_COUNT - 1;

    ASPxClientSpreadsheet.Range.Default = new ASPxClientSpreadsheet.Range(-1, -1);
    ASPxClientSpreadsheet.Range.Maximum = new ASPxClientSpreadsheet.Range(0, 0, 256, 65536); // TODO Why it was made so?
    //ASPxClientSpreadsheet.Range.Maximum = new ASPxClientSpreadsheet.Range(0, 0, ASPxClientSpreadsheet.Range.MAX_COL_COUNT, ASPxClientSpreadsheet.Range.MAX_ROW_COUNT);

    ASPxClientSpreadsheet.Selection = (function() {
        var Selection = function(range, activeCellColIndex, activeCellRowIndex) {
            // TODO ???
            /*this.activeCellColIndex;
             this.activeCellRowIndex;
             this.drawingBoxElement;
             this.drawingBoxIndex;*/
            this.allSelected = false;
            this.visible = true;

            this.setRange(range, activeCellColIndex, activeCellRowIndex);
        };

        function setActiveCellRangeCore(selection, activeCellRange) {
            selection.allSelected = false;
            selection.activeCellRange = activeCellRange || ASPxClientSpreadsheet.Range.Default.clone();
        }

        function setActiveCore(selection, colIndex, rowIndex) {
            selection.allSelected = false;
            selection.activeCellColIndex = colIndex;
            selection.activeCellRowIndex = rowIndex;
        }

        Selection.prototype = {
            selectDrawingBox: function(drawingBoxElement, drawingBoxIndex) {
                this.reset();
                this.drawingBoxIndex = drawingBoxIndex;
                this.drawingBoxElement = drawingBoxElement;
            },
            isDrawingBoxSelection: function() {
                return this.drawingBoxIndex >= 0;
            },
            isAllSelected: function() {
                return this.allSelected;
            },
            isCellSelected: function(colIndex, rowIndex) {
                var isActiveCell = this.activeCellColIndex == colIndex && this.activeCellRowIndex == rowIndex;
                var isInRange = this.range && this.range.isCellInRange(colIndex, rowIndex);
                return this.isAllSelected() || isActiveCell || isInRange;
            },
            setRange: function(range, activeCellColIndex, activeCellRowIndex) {
                if(range) {
                    this.range = range;
                    this.activeCellColIndex = typeof (activeCellColIndex) != "undefined" ? activeCellColIndex : range.leftColIndex;
                    this.activeCellRowIndex = typeof (activeCellRowIndex) != "undefined" ? activeCellRowIndex : range.topRowIndex;
                } else
                    this.range = ASPxClientSpreadsheet.Range.Default.clone();
            },
            setActiveCellRange: function(activeCellRange) {
                this.allSelected = false;
                setActiveCellRangeCore(this, activeCellRange);
                setActiveCore(this, activeCellRange.leftColIndex, activeCellRange.topRowIndex);
            },
            setActiveCell: function(colIndex, rowIndex) {
                this.allSelected = false;
                setActiveCellRangeCore(this, null);
                setActiveCore(this, colIndex, rowIndex);
            },
            activeCellExists: function() {
                return this.activeCellColIndex > -1 && this.activeCellRowIndex > -1;
            },
            clone: function() {
                var clone = new ASPxClientSpreadsheet.Selection(this.range.clone(), this.activeCellColIndex, this.activeCellRowIndex);
                clone.allSelected = this.allSelected;
                clone.visible = this.visible;
                clone.entireColsSelected = this.entireColsSelected;
                clone.entireRowsSelected = this.entireRowsSelected;
                if(this.activeCellRange)
                    clone.activeCellRange = this.activeCellRange.clone();
                clone.drawingBoxElement = this.drawingBoxElement;
                clone.drawingBoxIndex = this.drawingBoxIndex;
                clone.multiSelection = this.multiSelection;
                if(this.multiSelection)
                    clone.ranges = this.ranges.slice(0);
                return clone;
            },
            equals: function(selection) {
                return this.activeCellColIndex == selection.activeCellColIndex &&
                    this.activeCellRowIndex == selection.activeCellRowIndex &&
                    this.range.equals(selection.range);
            },
            isSingleCell: function() {
                return this.range.isSingleCell();
            },
            selectAll: function() {
                this.reset(true);
                this.allSelected = true;
            },
            reset: function(keepActiveCellRange) {
                this.allSelected = false;
                this.entireColsSelected = false;
                this.entireRowsSelected = false;
                
                this.setRange(null);
                if(!keepActiveCellRange) {                    
                    this.setActiveCell(-1, -1);
                    setActiveCellRangeCore(this, null);
                    setActiveCore(this, null, null);
                }

                this.drawingBoxElement = -1;
                this.drawingBoxIndex = -1;
            },
            isActiveCellMerged: function() {
                return this.activeCellRange && !this.activeCellRange.isSingleCell();
            },

            selectEntireColumns: function(leftColumnIndex, rightColumnIndex, activeCellColIndex, activeCellRowIndex) {
                this.reset();
                this.setRange(new ASPxClientSpreadsheet.Range(
                    leftColumnIndex,
                    ASPxClientSpreadsheet.Range.Maximum.topRowIndex,
                    rightColumnIndex,
                    ASPxClientSpreadsheet.Range.Maximum.bottomRowIndex), activeCellColIndex, activeCellRowIndex);
                this.entireColsSelected = true;
            },
            selectEntireRows: function(topRowIndex, bottomRowIndex, activeCellColIndex, activeCellRowIndex) {
                this.reset();
                this.setRange(new ASPxClientSpreadsheet.Range(
                    ASPxClientSpreadsheet.Range.Maximum.leftColIndex,
                    topRowIndex,
                    ASPxClientSpreadsheet.Range.Maximum.rightColIndex,
                    bottomRowIndex), activeCellColIndex, activeCellRowIndex);
                this.entireRowsSelected = true;
            },

            getConvertedToModelIndices: function(paneManager) {
                return this.getConvertedCore(function(visibleIndex, isCol) {
                    return paneManager.convertVisibleIndexToModelIndex(visibleIndex, isCol);
                });
            },
            getConvertedToVisibleIndices: function(paneManager) {
                var convertedSelection = this.clone();

                //NOTE: (T223707) indeces can be negative if
                //not all range (some tiles in range) has on client side
                var modelColIndex = paneManager.convertModelIndexToVisibleIndex(convertedSelection.activeCellColIndex, true),
                    modelRowIndex = paneManager.convertModelIndexToVisibleIndex(convertedSelection.activeCellRowIndex, false)

                convertedSelection.visible = modelColIndex >= 0 && modelRowIndex >= 0;

                convertedSelection.activeCellColIndex = modelColIndex >= 0 ? modelColIndex : convertedSelection.activeCellColIndex;
                convertedSelection.activeCellRowIndex = modelRowIndex >= 0 ? modelRowIndex : convertedSelection.activeCellRowIndex;

                convertedSelection.range.convertToVisibleIndices(paneManager);

                return convertedSelection;
            },
            getConvertedCore: function(converter) {
                var convertedSelection = this.clone();

                convertedSelection.activeCellColIndex = converter(convertedSelection.activeCellColIndex, true);
                convertedSelection.activeCellRowIndex = converter(convertedSelection.activeCellRowIndex, false);

                convertedSelection.range = convertedSelection.range.getConvertedCore(converter);

                return convertedSelection;
            },
            expandToAllSheetIfRequired: function() {
                var isAllSelected = this.isAllSelected();
                if(this.entireColsSelected || isAllSelected) {
                    this.range.topRowIndex = ASPxClientSpreadsheet.Range.MIN_ROW_INDEX;
                    this.range.bottomRowIndex = ASPxClientSpreadsheet.Range.MAX_ROW_INDEX;
                }

                if(this.entireRowsSelected || isAllSelected) {
                    this.range.leftColIndex = ASPxClientSpreadsheet.Range.MIN_COL_INDEX;
                    this.range.rightColIndex = ASPxClientSpreadsheet.Range.MAX_COL_INDEX;
                }
            },
            isValuable: function() {
                return this != ASPxClientSpreadsheet.Selection.Default;
            },
            correct: function() {
                if(this.activeCellColIndex < 0)
                    this.activeCellColIndex = 0;
                if(this.activeCellRowIndex < 0)
                    this.activeCellRowIndex = 0;
                if(this.range)
                    this.range.correct();
            },
            correctPaneSelection: function() {
                this.correct();
                if(this.range)
                    this.range.correctPanes();
            },
            resetVisible: function() {
                this.visible = true;
            }
        };
        return Selection;
    })();

    ASPxClientSpreadsheet.Selection.Default = new ASPxClientSpreadsheet.Selection(ASPxClientSpreadsheet.Range.Default.clone(), 0, 0);

    ASPxClientSpreadsheet.Selection.createFromDrawingBoxIndex = function(drawingBoxElement, drawingBoxIndex) {
        var selection = new ASPxClientSpreadsheet.Selection();
        selection.selectDrawingBox(drawingBoxElement, drawingBoxIndex);
        return selection;
    };

    ASPxClientSpreadsheet.SelectionKeyboardUtils = {
        createSingleCellSelection: function(selectionHelper, colIndex, rowIndex) {
            colIndex = this.verifyCellPosition(colIndex);
            rowIndex = this.verifyCellPosition(rowIndex);
            var activePaneType = selectionHelper.getPaneTypeByCell({ col: colIndex, row: rowIndex }),
                range = new ASPxClientSpreadsheet.Range(colIndex, rowIndex);

            return new ASPxClientSpreadsheet.Selection(range, colIndex, rowIndex);
        },
        verifyCellPosition: function(position) {
            if(position < 0)
                position = 0;
            return position;
        },
        moveRight: function(selection, selectionHelper) {
            selectionHelper.calcActualActiveCellRange(selection);
            return this.createSingleCellSelection(selectionHelper, selection.activeCellRange.rightColIndex + 1, selection.activeCellRowIndex);
        },
        moveLeft: function(selection, selectionHelper) {
            return this.createSingleCellSelection(selectionHelper, selection.activeCellColIndex - 1, selection.activeCellRowIndex);
        },
        moveUp: function(selection, selectionHelper) {
            return this.createSingleCellSelection(selectionHelper, selection.activeCellColIndex, selection.activeCellRowIndex - 1);
        },
        moveDown: function(selection, selectionHelper) {
            selectionHelper.calcActualActiveCellRange(selection);
            return this.createSingleCellSelection(selectionHelper, selection.activeCellColIndex, selection.activeCellRange.bottomRowIndex + 1);
        },

        shrinkCore: function(selection, selectionHelper, methodName, horizontalShrink) {
            var originSelection = selection.clone();
            var range;
            var collapsed;

            do {
                selection = this[methodName](selection);
                if(selection.equals(originSelection)) break;

                collapsed = horizontalShrink ? selection.range.leftColIndex == selection.range.rightColIndex :
                    selection.range.topRowIndex == selection.range.bottomRowIndex;
                range = selectionHelper.ExpandRangeToMergedCellSize(selection.range);
            } while(range.equals(originSelection.range) && !collapsed);

            selection.range = range;
            return selection;
        },

        // KB SHIFT+LEFT
        expandLeft: function(selection, selectionHelper) {
            if(selection.activeCellColIndex != selection.range.leftColIndex || selection.range.leftColIndex == selection.range.rightColIndex)
                return this.extendLeft(selection);
            else
                return this.shrinkRight(selection, selectionHelper);
        },
        extendLeft: function(selection) {
            var leftColIndex = this.verifyCellPosition(selection.range.leftColIndex - 1);
            // TODO range = ExpandRangeToMergedCellSize(range);
            selection.range.set(leftColIndex, selection.range.topRowIndex, selection.range.rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },
        shrinkRight: function(selection, selectionHelper) {
            return this.shrinkCore(selection, selectionHelper, "shrinkRightCore", true);
        },
        shrinkRightCore: function(selection) {
            var rightColIndex = selection.range.rightColIndex - 1;
            if(rightColIndex >= selection.range.leftColIndex)
                selection.range.set(selection.range.leftColIndex, selection.range.topRowIndex, rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },

        // KB SHIFT+RIGHT
        expandRight: function(selection, selectionHelper) {
            if(selection.activeCellColIndex != selection.range.rightColIndex || selection.range.leftColIndex == selection.range.rightColIndex)
                return this.extendRight(selection);
            else
                return this.shrinkLeft(selection, selectionHelper);
        },
        extendRight: function(selection) {
            var rightColIndex = this.verifyCellPosition(selection.range.rightColIndex + 1);
            // TODO range = ExpandRangeToMergedCellSize(range);
            selection.range.set(selection.range.leftColIndex, selection.range.topRowIndex, rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },
        shrinkLeft: function(selection, selectionHelper) {
            return this.shrinkCore(selection, selectionHelper, "shrinkLeftCore", true);
        },
        shrinkLeftCore: function(selection, selectionHelper) {
            var leftColIndex = selection.range.leftColIndex + 1;
            if(leftColIndex <= selection.range.rightColIndex)
                selection.range.set(leftColIndex, selection.range.topRowIndex, selection.range.rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },

        // KB SHIFT+UP
        expandUp: function(selection, selectionHelper) {
            if(selection.activeCellRowIndex != selection.range.topRowIndex || selection.range.topRowIndex == selection.range.bottomRowIndex)
                return this.extendUp(selection);
            else
                return this.shrinkDown(selection, selectionHelper);
        },
        extendUp: function(selection) {
            var topRowIndex = this.verifyCellPosition(selection.range.topRowIndex - 1);
            // TODO range = ExpandRangeToMergedCellSize(range);
            selection.range.set(selection.range.leftColIndex, topRowIndex, selection.range.rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },
        shrinkDown: function(selection, selectionHelper) {
            return this.shrinkCore(selection, selectionHelper, "shrinkDownCore", false);
        },
        shrinkDownCore: function(selection, selectionHelper) {
            var bottomRowIndex = selection.range.bottomRowIndex - 1;
            if(bottomRowIndex >= selection.range.topRowIndex)
                selection.range.set(selection.range.leftColIndex, selection.range.topRowIndex, selection.range.rightColIndex, bottomRowIndex);
            return selection;
        },

        // KB SHIFT+DOWN
        expandDown: function(selection, selectionHelper) {
            if(selection.activeCellRowIndex != selection.range.bottomRowIndex || selection.range.topRowIndex == selection.range.bottomRowIndex)
                return this.extendDown(selection);
            else
                return this.shrinkUp(selection, selectionHelper);
        },
        extendDown: function(selection) {
            var bottomRowIndex = this.verifyCellPosition(selection.range.bottomRowIndex + 1);
            // TODO range = ExpandRangeToMergedCellSize(range);
            selection.range.set(selection.range.leftColIndex, selection.range.topRowIndex, selection.range.rightColIndex, bottomRowIndex);
            return selection;
        },
        shrinkUp: function(selection, selectionHelper) {
            return this.shrinkCore(selection, selectionHelper, "shrinkUpCore", false);
        },
        shrinkUpCore: function(selection, selectionHelper) {
            var topRowIndex = selection.range.topRowIndex + 1;
            if(topRowIndex <= selection.range.bottomRowIndex)
                selection.range.set(selection.range.leftColIndex, topRowIndex, selection.range.rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },

        // SHITF_TAB
        moveActiveCellLeft: function(selection, selectionHelper) {
            if(this.isSingleCellSelected(selection))
                return this.moveLeft(selection, selectionHelper);

            if(selection.activeCellRange.leftColIndex > selection.range.leftColIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.activeCellRange.leftColIndex, selection.activeCellRange.topRowIndex, new Array(-1, 0), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellLeft);
                } else {
                    selection.setActiveCell(selection.activeCellRange.leftColIndex - 1, selection.activeCellRange.topRowIndex);
                }
                return selection;
            }
            if(selection.activeCellRange.topRowIndex > selection.range.topRowIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.range.rightColIndex, selection.activeCellRange.topRowIndex, new Array(0, -1), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellLeft);
                } else {
                    selection.setActiveCell(selection.range.rightColIndex, selection.activeCellRange.topRowIndex - 1);
                }
                return selection;
            }

            selection.setActiveCell(selection.range.rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },
        // TAB
        moveActiveCellRight: function(selection, selectionHelper) {
            if(this.isSingleCellSelected(selection))
                return this.moveRight(selection, selectionHelper);

            if(selection.activeCellRange.rightColIndex < selection.range.rightColIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.activeCellRange.rightColIndex, selection.activeCellRange.topRowIndex, new Array(1, 0), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellRight);
                } else {
                    selection.setActiveCell(selection.activeCellRange.rightColIndex + 1, selection.activeCellRange.topRowIndex);
                }
                return selection;
            }
            if(selection.activeCellRange.bottomRowIndex < selection.range.bottomRowIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.range.leftColIndex, selection.activeCellRange.topRowIndex, new Array(0, 1), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellRight);
                } else {
                    selection.setActiveCell(selection.range.leftColIndex, selection.activeCellRange.topRowIndex + 1);
                }
                return selection;
            }

            selection.setActiveCell(selection.range.leftColIndex, selection.range.topRowIndex);
            return selection;
        },
        // SHIFT+ENTER
        moveActiveCellUp: function(selection, selectionHelper) {
            if(this.isSingleCellSelected(selection))
                return this.moveUp(selection, selectionHelper);

            if(selection.activeCellRange.topRowIndex > selection.range.topRowIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.activeCellRange.leftColIndex, selection.activeCellRange.topRowIndex, new Array(0, -1), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellUp);
                } else {
                    selection.setActiveCell(selection.activeCellRange.leftColIndex, selection.activeCellRange.topRowIndex - 1);
                }
                return selection;
            }
            if(selection.activeCellRange.leftColIndex > selection.range.leftColIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.activeCellRange.leftColIndex, selection.range.bottomRowIndex, new Array(-1, 0), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellUp);
                } else {
                    selection.setActiveCell(selection.activeCellRange.leftColIndex - 1, selection.range.bottomRowIndex);
                }
                return selection;
            }

            selection.setActiveCell(selection.range.rightColIndex, selection.range.bottomRowIndex);
            return selection;
        },
        // ENTER
        moveActiveCellDown: function(selection, selectionHelper) {
            if(this.isSingleCellSelected(selection))
                return this.moveDown(selection, selectionHelper);

            if(selection.activeCellRange.bottomRowIndex < selection.range.bottomRowIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.activeCellRange.leftColIndex, selection.activeCellRange.bottomRowIndex, new Array(0, 1), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellDown);
                } else {
                    selection.setActiveCell(selection.activeCellRange.leftColIndex, selection.activeCellRange.bottomRowIndex + 1);
                }
                return selection;
            }
            if(selection.activeCellRange.rightColIndex < selection.range.rightColIndex) {
                if(selection.entireColsSelected || selection.entireRowsSelected) {
                    this.moveActiveCellInColOrRowSelectionMode(selection, selectionHelper, selection.activeCellRange.rightColIndex, selection.range.topRowIndex, new Array(1, 0), ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellDown);
                } else {
                    selection.setActiveCell(selection.activeCellRange.rightColIndex + 1, selection.range.topRowIndex);
                }
                return selection;
            }

            selection.setActiveCell(selection.range.leftColIndex, selection.range.topRowIndex);
            return selection;
        },
        // Healpers
        isSingleCellSelected: function(selection) {
            return selection.isSingleCell() || this.isMergedCellSelected(selection);
        },
        isMergedCellSelected: function(selection) {
            return !selection.activeCellRange.singleCell &&
                selection.range.topRowIndex === selection.activeCellRange.topRowIndex && selection.range.bottomRowIndex === selection.activeCellRange.bottomRowIndex &&
                selection.range.leftColIndex === selection.activeCellRange.leftColIndex && selection.range.rightColIndex === selection.activeCellRange.rightColIndex;
        },

        moveActiveCellInColOrRowSelectionMode: function(selection, selectionHelper, activeCellColIndex, activeCellRowIndex, positionMultiplier, method) {
            var cellFullyInSelectionRange = false;
            var activeCellRange;

            while(!cellFullyInSelectionRange) {
                activeCellColIndex = activeCellColIndex + positionMultiplier[0];
                activeCellRowIndex = activeCellRowIndex + positionMultiplier[1];

                activeCellRange = this.createActiveCellRange(selectionHelper, activeCellColIndex, activeCellRowIndex);

                if(!activeCellRange.singleCell)
                    cellFullyInSelectionRange = selection.range.topRowIndex <= activeCellRange.topRowIndex && selection.range.bottomRowIndex >= activeCellRange.bottomRowIndex &&
                                            selection.range.leftColIndex <= activeCellRange.leftColIndex && selection.range.rightColIndex >= activeCellRange.rightColIndex;
                else cellFullyInSelectionRange = activeCellRange.singleCell;

                if(!cellFullyInSelectionRange && this.isCellOnEndOfRange(selection, activeCellRange, method)) {
                    selection.activeCellRange = activeCellRange;
                    return this[method].call(this, selection, selectionHelper);
                }
            }
            selection.setActiveCellRange(activeCellRange);
        },
        createActiveCellRange: function(selectionHelper, activeCellColIndex, activeCellRowIndex) {
            var activeCellRange = new ASPxClientSpreadsheet.Range(activeCellColIndex, activeCellRowIndex, activeCellColIndex, activeCellRowIndex);
            activeCellRange = selectionHelper.ExpandRangeToMergedCellSize(activeCellRange);
            return activeCellRange;
        },
        isCellOnEndOfRange: function(selection, activeCellRange, method) {
            var initialCellPosition = this.checkCellPosition(selection, selection.activeCellRange);
            var cellPosition = this.checkCellPosition(selection, activeCellRange);

            if(method === ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellRight)
                return cellPosition.right || initialCellPosition.right;
            else if(method === ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellLeft)
                return cellPosition.left || initialCellPosition.left;
            else if(method === ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellUp || method === ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions.MoveActiveCellDown)
                return cellPosition.top || initialCellPosition.top || cellPosition.bottom || initialCellPosition.bottom;

            return false;
        },
        checkCellPosition: function(selection, activeCellRange) {
            var cellPosition = { top: false, bottom: false, left: false, right: false };
            cellPosition.right = selection.range.rightColIndex <= activeCellRange.rightColIndex;
            cellPosition.left = selection.range.leftColIndex >= activeCellRange.leftColIndex;
            cellPosition.top = selection.range.topRowIndex >= activeCellRange.topRowIndex;
            cellPosition.bottom = selection.range.bottomRowIndex <= activeCellRange.bottomRowIndex;
            return cellPosition;
        }
    };

    ASPxClientSpreadsheet.SelectionKeyboardUtils.Actions = {
        MoveActiveCellDown: "moveActiveCellDown",
        MoveActiveCellUp: "moveActiveCellUp",
        MoveActiveCellRight: "moveActiveCellRight",
        MoveActiveCellLeft: "moveActiveCellLeft"
    };

    ASPxClientSpreadsheet.SelectionHelper = function(spreadsheet) {
        var spreadsheet = spreadsheet;

        this.dynamicColorIndex = 0;
        this.selectionMovementLayoutInfo = null;

        this.getPaneTypeByCell = function(cell) {
            return spreadsheet.getPaneManager().getPaneTypeByCell(cell);
        };
        this.setSelectionCells = function(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex) {
            var range = new ASPxClientSpreadsheet.Range(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex);
            this.setSelectionRangeInternal(range);
        }.aspxBind(this);
        this.setSelectionRangeInternal = function(range) {
            var selection = new ASPxClientSpreadsheet.Selection(range);
            this.setSelectionInternal(selection);
        };
        this.setSelection = function(selection, keepOldSelection) {
            if(!isSelectionVisible(selection)) return;

            selection = selection.clone();
            this.setSelectionInternal(selection, keepOldSelection);
        }.aspxBind(this);
        this.setSelectionSilently = function(selection) {
            selection = selection.clone();
            calcActualCellSelectionInternal(selection);
        };

        function isSelectionVisible(selection) {
            return spreadsheet.getPaneManager().isSelectionVisible(selection);
        }

        this.setSelectionInternal = function(selection, keepOldSelection) {
            if(selection.isDrawingBoxSelection())
                setDrawingBoxSelection(selection);
            else if(selection.isAllSelected())
                this.setSheetSelection(selection);
            else
                this.setCellSelection(selection, keepOldSelection);
            spreadsheet.onSelectionChanged();
        };
        function setDrawingBoxSelection(selection) {
            hideSelectionInternal();
            showDrawingBoxSelection(selection);
        }

        this.setSheetSelection = function(selection) {
            selection.setRange(ASPxClientSpreadsheet.Range.Maximum.clone()); // TODO move to selection
            this.setSelectionBase(selection);
        };
        this.setCellSelection = function(selection, keepOldSelection) {
            calcActualCellSelectionInternal(selection);
            this.setSelectionBase(selection, keepOldSelection);
        };
        this.setSelectionBase = function(selection, keepOldSelection) {
            this.correctSelectionToVisibleRange(selection);
            if(selection.visible)
                spreadsheet.getPaneManager().drawSelection(selection, keepOldSelection);
        };
        function expandEntireRowsAndColumns(selection) {
            var cellVisibleRange = spreadsheet.getPaneManager().getWorkbookVisibleCellRange();
            if(selection.entireColsSelected) {
                selection.range.topRowIndex = cellVisibleRange.top;
                selection.range.bottomRowIndex = cellVisibleRange.bottom;
            }

            if(selection.entireRowsSelected) {
                selection.range.leftColIndex = cellVisibleRange.left;
                selection.range.rightColIndex = cellVisibleRange.right;
            }
        }

        this.correctSelectionToVisibleRange = function(selection) {
            var paneManager = spreadsheet.getPaneManager();

            selection.activeCellColIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.activeCellColIndex, true);
            selection.activeCellRowIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.activeCellRowIndex, false);

            selection.range.leftColIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.range.leftColIndex, true);
            selection.range.topRowIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.range.topRowIndex, false);
            selection.range.rightColIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.range.rightColIndex, true);
            selection.range.bottomRowIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.range.bottomRowIndex, false);

            selection.activeCellRange.leftColIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.activeCellRange.leftColIndex, true);
            selection.activeCellRange.topRowIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.activeCellRange.topRowIndex, false);
            selection.activeCellRange.rightColIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.activeCellRange.rightColIndex, true);
            selection.activeCellRange.bottomRowIndex = paneManager.putSelectionCellIndexToVisibleDocumentRange(selection.activeCellRange.bottomRowIndex, false);
        };

        this.correctRangeToVisiblePosition = function(range) {
            var result = range.clone(),
                paneManager = spreadsheet.getPaneManager();

            result.leftColIndex = paneManager.putCellColIndexInVisibleRange(range.leftColIndex);
            result.topRowIndex = paneManager.putCellRowIndexInVisibleRange(range.topRowIndex);
            result.rightColIndex = paneManager.putCellColIndexInVisibleRange(range.rightColIndex);
            result.bottomRowIndex = paneManager.putCellRowIndexInVisibleRange(range.bottomRowIndex);

            return result;
        };

        this.calcActualCellSelection = function(selection) {
            calcActualCellSelectionInternal(selection);
        }.aspxBind(this);
        function saveNotActualizableData(selection) {
            var notActualizableData = {
                allSelected: selection.allSelected
            };
            return notActualizableData;
        }
        function restoreNotActualizableData(selection, notActualizableData) {
            selection.allSelected = notActualizableData.allSelected;
        }
        function calcActualCellSelectionInternal(selection) {
            var notActualizableData = saveNotActualizableData(selection);

            expandEntireRowsAndColumns(selection);
            if(!selection.entireColsSelected && !selection.entireRowsSelected)
                calcActualRange(selection);
            calcActualActiveCellRangeInternal(selection);

            restoreNotActualizableData(selection, notActualizableData);
        }

        function calcActualRange(selection) {
            selection.range = ExpandRangeToMergedCellSizeInternal(selection.range);
        }

        this.calcActualActiveCellRange = function(selection) {
            calcActualActiveCellRangeInternal(selection);
        }.aspxBind(this);
        function calcActualActiveCellRangeInternal(selection) {
            var activeCellRange = new ASPxClientSpreadsheet.Range(selection.activeCellColIndex, selection.activeCellRowIndex, selection.activeCellColIndex, selection.activeCellRowIndex);
            activeCellRange = ExpandActiveCellToMergedRange(activeCellRange);
            selection.setActiveCellRange(activeCellRange);
        }

        this.ExpandRangeToMergedCellSize = function(rangeDesired) {
            return ExpandRangeToMergedCellSizeInternal(rangeDesired);
        }.aspxBind(this);
        function ExpandRangeToMergedCellSizeInternal(rangeDesired) {
            var range = rangeDesired.clone();
            var paneManager = spreadsheet.getPaneManager();

            var changed = true;
            while(changed) {
                changed = false;

                var complexBoxes = paneManager.getMergedCellRangesIntersectsRange(range);

                for(var i = 0; i < complexBoxes.length; i++) {
                    var complexBox = complexBoxes[i];
                    if(range.leftColIndex > complexBox[0]) {
                        range.leftColIndex = complexBox[0];
                        changed = true;
                    }
                    if(range.topRowIndex > complexBox[1]) {
                        range.topRowIndex = complexBox[1];
                        changed = true;
                    }

                    if(range.rightColIndex < complexBox[2]) {
                        range.rightColIndex = complexBox[2];
                        changed = true;
                    }
                    if(range.bottomRowIndex < complexBox[3]) {
                        range.bottomRowIndex = complexBox[3];
                        changed = true;
                    }
                }
                if(changed)
                    range.singleCell = false;
            }

            return range;
        }
        function ExpandActiveCellToMergedRange(rangeDesired) {
            var range = rangeDesired.clone(),
                paneManager = spreadsheet.getPaneManager(),
                complexBoxes = paneManager.getMergedCellRangesIntersectsRange(range);

                for(var i = 0; i < complexBoxes.length; i++) {
                    var complexBox = complexBoxes[i];
                    if(range.leftColIndex >= complexBox[0] && range.rightColIndex <= complexBox[2] &&
                        range.topRowIndex >= complexBox[1] && range.bottomRowIndex <= complexBox[3]) {
                        
                        range.leftColIndex = complexBox[0];
                        range.topRowIndex = complexBox[1];
                        range.rightColIndex = complexBox[2];
                        range.bottomRowIndex = complexBox[3];

                        range.singleCell = false;
                    }
                }
            return range;
        }
        
        this.findNoneMergedCell = function(isCol, startIndex) {
            var topLeftCell = spreadsheet.getPaneManager().getTopLeftCellVisiblePosition(),
                cellSearchIndex = isCol ? topLeftCell.row : topLeftCell.col,
                isMergedCell = true,
                activeCellRange = isCol ? new ASPxClientSpreadsheet.Range(startIndex, cellSearchIndex, startIndex, cellSearchIndex) :
                                        new ASPxClientSpreadsheet.Range(cellSearchIndex, startIndex, cellSearchIndex, startIndex);
            while(isMergedCell) {
                var newActiveCellRange = ExpandRangeToMergedCellSizeInternal(activeCellRange);
                if(activeCellRange.bottomRowIndex == newActiveCellRange.bottomRowIndex && activeCellRange.leftColIndex == newActiveCellRange.leftColIndex &&
                    activeCellRange.rightColIndex == newActiveCellRange.rightColIndex && activeCellRange.topRowIndex == newActiveCellRange.topRowIndex) {
                    return newActiveCellRange;
                } else {
                    cellSearchIndex++;
                    activeCellRange = isCol ? new ASPxClientSpreadsheet.Range(startIndex, cellSearchIndex, startIndex, cellSearchIndex) : new ASPxClientSpreadsheet.Range(cellSearchIndex, startIndex, cellSearchIndex, startIndex);
                }
            }
        };

        this.getDrawingBoxSelectionElement = function() {
            return getDrawingBoxSelectionElementInternal();
        }.aspxBind(this);
        function showDrawingBoxSelection(selection) {
            var selectionElement = getDrawingBoxSelectionElementInternal();
            var drawingBoxElement = selection.drawingBoxElement;
            if(drawingBoxElement) {
                var parentTileElement = drawingBoxElement.parentNode;
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(selectionElement, parentTileElement);
                var rect = {
                    x: ASPx.PxToInt(drawingBoxElement.style.left),
                    y: ASPx.PxToInt(drawingBoxElement.style.top),
                    width: ASPx.PxToInt(drawingBoxElement.style.width) - ASPx.GetHorizontalBordersWidth(selectionElement),
                    height: ASPx.PxToInt(drawingBoxElement.style.height) - ASPx.GetHorizontalBordersWidth(selectionElement)
                };
                ASPxClientSpreadsheet.CellLayoutHelper.setElementRect(selectionElement, rect);
                ASPx.SetElementDisplay(selectionElement, true);
            }
        }

        this.hideSelection = function() {
            hideSelectionInternal();
        }.aspxBind(this);

        this.hideDynamicSelection = function() {
            var selection;

            while(selection = this.dynamicSelection.pop()) {
                selection.dispose();
            }

            this.dynamicColorIndex = 0;
        }.aspxBind(this);

        function hideSelectionInternal() {
            ASPx.SetElementDisplay(getDrawingBoxSelectionElementInternal(), false);
        }

        this.dynamicSelection = [];

        this.drawDynamicSelectionRect = function(selection, selectionRectArray, parentTileInfo) {
            var selectionRect = new ASPxClientSpreadsheet.DynamicSelection(spreadsheet, this.dynamicColorIndex++);

            selectionRect.render(selection, selectionRectArray, parentTileInfo);
            this.dynamicSelection.push(selectionRect);
            if(this.dynamicColorIndex === DYNAMIC_COLORS_COUNT) {
                this.dynamicColorIndex = 0;
            }
        }.aspxBind(this);

        this.parseFormula = function(inputElement, cursorPosition) {
            var that = this;

            spreadsheet.stateController.resetActiveSelection();
            if(inputElement.value) {
                ASPx.Data.ForEach(ASPx.SpreadsheetFormulaParser.parse(inputElement.value, spreadsheet.currentActiveCell, spreadsheet.getCurrentSheetName(), cursorPosition), function(selection) {
                    if(selection.range.isHighlighted) {
                        spreadsheet.stateController.setActiveSelection(selection);
                    }
                    that.setSelection(selection, true);
                });
                spreadsheet.stateController.setCursorPosition(cursorPosition);
            }
        }.aspxBind(this);

        function getDrawingBoxSelectionElementInternal() {
            if(!spreadsheet.drawingBoxSelectElement)
                spreadsheet.drawingBoxSelectElement = createDrawingBoxSelectionElement();
            return spreadsheet.drawingBoxSelectElement;
        }

        function createDrawingBoxSelectionElement() {
            var element = document.createElement("DIV");
            element.className = ASPx.SpreadsheetCssClasses.DrawingBoxSelectedElement;

            var corner = document.createElement("DIV");
            corner.className = "corner nw";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "nw");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner ne";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "ne");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner se";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "se");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner sw";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "sw");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner n";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "n");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner e";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "e");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner s";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "s");
            element.appendChild(corner);

            corner = document.createElement("DIV");
            corner.className = "corner w";
            addTouchClassForCorner(corner);
            ASPx.Attr.SetAttribute(corner, "resizeDirection", "w");
            element.appendChild(corner);

            return element;
        }

        function addTouchClassForCorner(corner) {
            if(ASPx.Browser.TouchUI)
                corner.className += " cornerTouch";
        }
    };
    ASPxClientSpreadsheet.SelectionHelper.BorderCorrectionFactor = 2;
})();