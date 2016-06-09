(function(undefined) {
    ASPxClientSpreadsheet.SelectionState = function(spreadsheetControl) {
        var selectionHelper = spreadsheetControl.getSelectionHelper();

        var currentSelection = ASPxClientSpreadsheet.Selection.Default;

        var firstCell = {col: -1, row: -1};

        function setFirstCell(col, row) {
            firstCell.col = col;
            firstCell.row = row;
        }

        var lastCell = {col: -1, row: -1};

        function setLastCell(col, row) {
            lastCell.col = col;
            lastCell.row = row;
        }

        function moveActiveCellToLeftTop(selection) {
            selection.activeCellColIndex = Math.min(firstCell.col, lastCell.col);
            selection.activeCellRowIndex = Math.min(firstCell.row, lastCell.row);
        }

        this.bindToCell = function(cell) {
            setFirstCell(cell.col, cell.row);
            setLastCell(-1, -1);
            var range = new ASPxClientSpreadsheet.Range(cell.col, cell.row);
            var selection = new ASPxClientSpreadsheet.Selection(range);
            
            this.setSelection(selection);
        };
        this.bindToLastCell = function(cell) {
            if(firstCell) {
                if(lastCell.col == cell.col && lastCell.row == cell.row)
                    return false;
                setLastCell(cell.col, cell.row);
                if(ASPx.Browser.TouchUI)
                    moveActiveCellToLeftTop(currentSelection);
                currentSelection.range.set(firstCell.col, firstCell.row, lastCell.col, lastCell.row);
            }
            return true;
        };
        this.rebindFirstCellDiagonally = function(rightBottomSelection) {
            if(firstCell && lastCell.col != -1 && lastCell.row != -1) {
                var col = rightBottomSelection ? Math.min(firstCell.col, lastCell.col) : Math.max(firstCell.col, lastCell.col);
                var row = rightBottomSelection ? Math.min(firstCell.row, lastCell.row) : Math.max(firstCell.row, lastCell.row);
                setFirstCell(col, row);
            }
        };

        this.selectDrawingBoxIndex = function(drawingBoxElement, drawingBoxIndex) {
            var selection = ASPxClientSpreadsheet.Selection.createFromDrawingBoxIndex(drawingBoxElement, drawingBoxIndex);
            this.setSelection(selection);
        };

        /*region State Interfate implementation*/
        this.activate = function() {
            selectionHelper.calcActualCellSelection(currentSelection);
            selectionHelper.setSelection(currentSelection);
            spreadsheetControl.DoRaiseSelectionChanged(currentSelection);
        };
        this.finalize = function() {
            spreadsheetControl.getSelectionHelper().hideSelection();
        };
        /*endregion*/

        this.activateSilently = function() {
            selectionHelper.calcActualCellSelection(currentSelection);
            selectionHelper.setSelectionSilently(currentSelection);
        };
        this.getSelection = function() {
            return currentSelection;
        };
        this.setSelection = function(selection) {
            currentSelection = validateSelection(selection);
            if(firstCell.col === -1 || firstCell.row === -1)
                setFirstCell(selection.activeCellColIndex, selection.activeCellRowIndex);

        };
        this.isCellSelected = function(colIndex, rowIndex) {
            return currentSelection.isCellSelected(colIndex, rowIndex);
        };
		function validateSelection(selection) {
	        selection.correct();
	        return selection;
	    }
    };

    ASPxClientSpreadsheet.EditingState = function(spreadsheetControl) {
        var editMode = ASPxClientSpreadsheet.StateController.Modes.Ready;
        var cellLayoutInfo = null;

        this.bindToCellLayoutInfo = function(newCellLayoutInfo) {
            cellLayoutInfo = newCellLayoutInfo;
        };
        this.canStartEditing = function(newCellLayoutInfo) {
            var curCellLayoutInfo = newCellLayoutInfo || cellLayoutInfo;
            if(!curCellLayoutInfo)
                return false;

            return spreadsheetControl.CanEditDocument() && !spreadsheetControl.cellLocked_ByLayoutInfo(curCellLayoutInfo);
        };

        /*region State Interfate implementation*/
        this.activate = function(newEditMode) {
            if(this.canStartEditing()) {
                editMode = newEditMode;
                this.startEditing(newEditMode);
            }
            spreadsheetControl.getDynamicSelectionHelper().attach(spreadsheetControl.getEditingHelper().getEditorElement());
        };
        this.finalize = function() {
            this.apply();
        };
        /*endregion*/
        this.startEditing = function(newEditMode) { // TODO can make it private?
            if(this.canStartEditing()) {
                editMode = newEditMode;
                spreadsheetControl.getEditingHelper().startEditing(cellLayoutInfo, newEditMode);
            }
        };
        this.apply = function() {
            if(bound(this))
                spreadsheetControl.getEditingHelper().apply(cellLayoutInfo);
            onAfterEditingStoped();
            spreadsheetControl.getDynamicSelectionHelper().detach();
        };
        this.cancel = function() {
            spreadsheetControl.getEditingHelper().stopEditing();
            onAfterEditingStoped();
            spreadsheetControl.getDynamicSelectionHelper().detach();
        };

        this.getEditMode = function() {
            return editMode;
        };

        this.insertSpecificFunction = function(functionName) {//TODO union activate and insert
            var editor = spreadsheetControl.getEditingHelper().getEditorElement();

            functionName = functionName + '()';
            if(editor.value == '') {
                var text = '=' + functionName;

                this.activate(ASPxClientSpreadsheet.StateController.Modes.Enter);
                editor = spreadsheetControl.getEditingHelper().getEditorElement();
                spreadsheetControl.setElementsValue(text);
                spreadsheetControl.getEditingHelper().updateInplaceEditingCellElementSize();
                ASPx.Selection.SetCaretPosition(editor, text.length - 1);
                spreadsheetControl.displayCurrentFunctionArgumentsHint();
            } else {
                var startPos = 0,
                    endPos = 0;

                if(spreadsheetControl.focusedElementSelectionTextInfo) {
                    startPos = spreadsheetControl.focusedElementSelectionTextInfo.startPos;
                    endPos = spreadsheetControl.focusedElementSelectionTextInfo.endPos;
                }
                if(startPos == endPos) {
                    editor.value = editor.value.substring(0, startPos) + functionName + editor.value.substring(startPos, editor.value.length);
                } else {
                    editor.value = editor.value.substring(0, startPos) + functionName + editor.value.substring(endPos, editor.value.length);
                }

                ASPx.Selection.SetCaretPosition(editor, startPos + functionName.length - 1);
            }
        };

        function onAfterEditingStoped() {
            editMode = ASPxClientSpreadsheet.StateController.Modes.Ready;
            unbound();
        }

        function bound() {
            return !!cellLayoutInfo;
        }

        function unbound() {
            cellLayoutInfo = null;
        }
    };

    ASPxClientSpreadsheet.RangeEditState = function(spreadsheetControl) {
        var activated = false,
            dynamicSelectionHelper = spreadsheetControl.getDynamicSelectionHelper(),
            paneManager = spreadsheetControl.getPaneManager();

        /*region State Interfate implementation*/
        this.activate = function(cellLayoutInfo) {
            activated = true;
            cellLayoutInfo.colIndex = paneManager.convertVisibleIndexToModelIndex(cellLayoutInfo.colIndex, true);
            cellLayoutInfo.rowIndex = paneManager.convertVisibleIndexToModelIndex(cellLayoutInfo.rowIndex, false);
            dynamicSelectionHelper.activate(cellLayoutInfo);
        };
        // TODO: activate - deactivate, initialize - finalize.
        this.finalize = function() {
            activated = false;
            dynamicSelectionHelper.deactivate();
        };
        /*endregion*/

        this.continueSelection = function(cellLayoutInfo) {
            if(activated) {
                cellLayoutInfo.colIndex = paneManager.convertVisibleIndexToModelIndex(cellLayoutInfo.colIndex, true);
                cellLayoutInfo.rowIndex = paneManager.convertVisibleIndexToModelIndex(cellLayoutInfo.rowIndex, false);
                dynamicSelectionHelper.updateActiveSelection(cellLayoutInfo);
            }
        };

        this.onCellEditorChanged = function() {
            dynamicSelectionHelper.refresh()
        };

        this.isActivated = function() {
            return activated;
        };
    };

    ASPxClientSpreadsheet.DrawingBoxResizeSession = (function() {
        var minDrawingSize = 2;

        function getAbsoluteX(element) {
            var x = ASPx.GetAbsoluteX(element);
            if(ASPx.Browser.IE && ASPx.Browser.Version >= 10)
                x = Math.round(x);
            return x;
        }
        function getAbsoluteY(element) {
            var y = ASPx.GetAbsoluteY(element);
            if(ASPx.Browser.IE && ASPx.Browser.Version >= 10)
                y = Math.round(y);
            return y;
        }
        function getEventPoint(evt) {
            var x = ASPx.Evt.GetEventX(evt),
                y = ASPx.Evt.GetEventY(evt);
            if(ASPx.Browser.IE && ASPx.Browser.Version >= 10) {
                x = Math.round(x);
                y = Math.round(y);
            }
            return { x: x, y: y };
        }

        var DrawingBoxResizeSession = function(initMouseDownEvent, sourceElement) {
            if(ASPx.ElementHasCssClass(sourceElement, ASPx.SpreadsheetCssClasses.DrawingBoxSelectedElement)) {
                this.resizingPanel = sourceElement;
                this.movingHorizontalOffset = getAbsoluteX(this.resizingPanel) - ASPx.Evt.GetEventX(initMouseDownEvent);
                this.movingVerticalOffset = getAbsoluteY(this.resizingPanel) - ASPx.Evt.GetEventY(initMouseDownEvent);
                this.moving = true;
                this.resizing = false;
            } else if(ASPx.ElementHasCssClass(sourceElement.parentNode, ASPx.SpreadsheetCssClasses.DrawingBoxSelectedElement)) {
                this.resizingPanel = sourceElement.parentNode;

                var resizingDirection = ASPx.Attr.GetAttribute(sourceElement, "resizeDirection");
                this.lockH = resizingDirection === "s" || resizingDirection === "n";
                this.lockV = resizingDirection === "e" || resizingDirection === "w";
                this.sideH = resizingDirection.indexOf("e") > -1;
                this.sideV = resizingDirection.indexOf("s") > -1;

                this.startPoint = getEventPoint(initMouseDownEvent);

                this.moving = false;
                this.resizing = true;
            }

            this.initPosX = getAbsoluteX(this.resizingPanel);
            this.initPosY = getAbsoluteY(this.resizingPanel);

            this.initWidth = this.resizingPanel.offsetWidth;
            this.initHeight = this.resizingPanel.offsetHeight;
        };
        DrawingBoxResizeSession.prototype = {
            getChanges: function() {
                return {
                    offsetX: getAbsoluteX(this.resizingPanel) - this.initPosX,
                    offsetY: getAbsoluteY(this.resizingPanel) - this.initPosY,
                    width: this.resizingPanel.offsetWidth,
                    height: this.resizingPanel.offsetHeight
                }
            },
            hasChanges: function() {
                return (
                    this.initPosX != getAbsoluteX(this.resizingPanel) ||
                    this.initPosY != getAbsoluteY(this.resizingPanel) ||
                    this.initWidth != this.resizingPanel.offsetWidth ||
                    this.initHeight != this.resizingPanel.offsetHeight);
            },
            setResizePanelPositionAndDimensions: function(x, y, width, height) {
                ASPx.SetOffsetWidth(this.resizingPanel, width);
                ASPx.SetOffsetHeight(this.resizingPanel, height);
                ASPx.SetAbsoluteX(this.resizingPanel, x);
                ASPx.SetAbsoluteY(this.resizingPanel, y);
            },
            onMouseMove: function(evt, isChart) {
                if(ASPx.Browser.WebKitTouchUI)
                    evt.preventDefault();
                if(this.resizing)
                    this.onResize(evt, !isChart || evt.shiftKey);
                else if(this.moving)
                    this.onMove(evt);
            },
            onMouseUp: function(evt) {
            },
            onEscape: function() {
                this.setResizePanelPositionAndDimensions(this.initPosX, this.initPosY, this.initWidth, this.initHeight);
            },
            onResize: function(evt, proportional) {
                var newLeft = this.initPosX,
                    newTop = this.initPosY,
                    newWidth = this.initWidth,
                    newHeight = this.initHeight;
                
                var eventPoint = getEventPoint(evt),
                    deltaX = eventPoint.x - this.startPoint.x,
                    deltaY = eventPoint.y - this.startPoint.y;
                deltaX = !this.sideH && deltaX > 0 ? Math.min(this.initWidth + 1, deltaX) : deltaX;
                deltaY = !this.sideV && deltaY > 0 ? Math.min(this.initHeight + 1, deltaY) : deltaY;

                if(!this.lockH && !this.lockV) {
                    if(proportional) {
                        if(Math.abs(deltaX) > Math.abs(deltaY)) {
                            newWidth = this.sideH ? Math.max(minDrawingSize, this.initWidth + deltaX) : (this.initWidth - deltaX);
                            newHeight = this.initHeight * (newWidth / this.initWidth);
                        } else {
                            newHeight = this.sideV ? Math.max(minDrawingSize, this.initHeight + deltaY) : (this.initHeight - deltaY);
                            newWidth = this.initWidth * (newHeight / this.initHeight);
                        }
                    } else {
                        newHeight = this.sideV ? Math.max(minDrawingSize, this.initHeight + deltaY) : (this.initHeight - deltaY);
                        newWidth = this.sideH ? Math.max(minDrawingSize, this.initWidth + deltaX) : (this.initWidth - deltaX);
                    }
                } else {
                    deltaX = this.lockH ? 0 : deltaX;
                    deltaY = this.lockV ? 0 : deltaY;
                    newWidth = Math.max(minDrawingSize, this.sideH ? (this.initWidth + deltaX) : (this.initWidth - deltaX));
                    newHeight = Math.max(minDrawingSize, this.sideV ? (this.initHeight + deltaY) : (this.initHeight - deltaY));
                }

                newLeft += this.sideH ? 0 : this.initWidth - newWidth;
                newTop += this.sideV ? 0 : this.initHeight - newHeight;

                this.setResizePanelPositionAndDimensions(newLeft, newTop, newWidth, newHeight);
            },
            onMove: function(evt) {
                var coord = getEventPoint(evt);
                ASPx.SetAbsoluteX(this.resizingPanel, coord.x + this.movingHorizontalOffset);
                ASPx.SetAbsoluteY(this.resizingPanel, coord.y + this.movingVerticalOffset);
            }
        };
        return DrawingBoxResizeSession;
    })();

    ASPxClientSpreadsheet.DrawingBoxSelectionStateHelper = function(spreadsheetControl, selectionState) {
        var currentSelectionState = selectionState;
        this.drawingBoxElement = null;
        this.drawingBoxIndex = -1;
        this.drawingBoxMoveOrResizeSession = null;

        this.captureElement = function(evt, drawingBoxElement, drawingBoxIndex) {
            this.drawingBoxElement = drawingBoxElement;
            this.drawingBoxIndex = drawingBoxIndex;
        };
        this.onMouseDown = function(evt, element) {
            var preventScroll = function() { if(ASPx.Browser.WebKitTouchUI) evt.stopPropagation(); };
            preventScroll();
            if(spreadsheetControl.getIsDrawingBoxElement(element)) {
                ASPx.Evt.PreventElementDrag(element);
                element = spreadsheetControl.getSelectionHelper().getDrawingBoxSelectionElement();
            }
            this.drawingBoxMoveOrResizeSession = new ASPxClientSpreadsheet.DrawingBoxResizeSession(evt, element);
        };
        this.onMouseMove = function(evt) {
            if(this.drawingBoxMoveOrResizeSession) {
                var drowingBoxElement = spreadsheetControl.getSelectionInternal().drawingBoxElement,
                    isChart = spreadsheetControl.getIsChartElement(drowingBoxElement);
                this.drawingBoxMoveOrResizeSession.onMouseMove(evt, isChart);
            }
        };
        this.onMouseUp = function(evt, drawingBoxElement) {
            if(this.drawingBoxMoveOrResizeSession) {
                this.drawingBoxMoveOrResizeSession.onMouseUp(evt);
                var hasChanges = this.drawingBoxMoveOrResizeSession.hasChanges();
                var shapeDiff = this.drawingBoxMoveOrResizeSession.getChanges();
                this.drawingBoxMoveOrResizeSession = null;

                if(hasChanges) {
                    ASPxClientSpreadsheet.ServerCommands.ShapeMoveAndResize(spreadsheetControl,
                        {   shapeOffsetX: shapeDiff.offsetX,
                            shapeOffsetY: shapeDiff.offsetY,
                            shapeWidth: shapeDiff.width,
                            shapeHeight: shapeDiff.height
                        }
                    );
                }
            }
        };
        this.onEscape = function() {
            if(this.drawingBoxMoveOrResizeSession) {
                this.drawingBoxMoveOrResizeSession.onEscape();
                this.drawingBoxMoveOrResizeSession = null;
            }
        };
    };

    ASPxClientSpreadsheet.EntireRowAllColSelectionStateHelper = function(spreadsheetControl, selectionState) {
        var currentSelectionState = selectionState;

        var startIndex = -1,
            selectionMethod = "",
            curIsCol,
            activeCellRange = null;


        function setSelectionMethod(isCol) {
            selectionMethod = isCol ? "selectEntireColumns" : "selectEntireRows";
        }

        this.changeStartParams = function(toRightOrBottom) {
            var selection = currentSelectionState.getSelection();
            selection.resetVisible();
            if(selection.entireColsSelected) {
                startIndex = toRightOrBottom ? selection.range.leftColIndex : selection.range.rightColIndex;
                curIsCol = true;
            }
            else if(selection.entireRowsSelected) {
                startIndex = toRightOrBottom ? selection.range.topRowIndex : selection.range.bottomRowIndex;
                curIsCol = false;
            }
            setSelectionMethod(curIsCol);
        };
        this.onMouseDown = function(cellIndex, isCol) {
            startIndex = cellIndex;
            setSelectionMethod(isCol);
            curIsCol = isCol;

            var selection = currentSelectionState.getSelection(),
                selectionHelper = spreadsheetControl.getSelectionHelper();
            selection.resetVisible();
            activeCellRange = selectionHelper.findNoneMergedCell(isCol, cellIndex);
                           
            selection[selectionMethod](startIndex, cellIndex, activeCellRange.leftColIndex, activeCellRange.topRowIndex);

            currentSelectionState.activate();
        };
        this.onMouseMove = function(cellIndex, isCol) {
            if(curIsCol != isCol || startIndex == -1) return;

            var selection = currentSelectionState.getSelection();
            selection.resetVisible();
            if(!activeCellRange)
                findActiveCellRange(isCol, startIndex);
            moveActiveCellToRangeTouch(startIndex, cellIndex, isCol);

            selection[selectionMethod](startIndex, cellIndex, activeCellRange.leftColIndex, activeCellRange.topRowIndex);
            currentSelectionState.activate();
        };
        function findActiveCellRange(isCol, startIndex) {
            var selectionHelper = spreadsheetControl.getSelectionHelper();
            activeCellRange = selectionHelper.findNoneMergedCell(isCol, startIndex);
        }
        function moveActiveCellToRangeTouch(startIndex, cellIndex, isCol) {
            if(ASPx.Browser.TouchUI) {
                var minIndex = Math.min(startIndex, cellIndex);
                var maxIndex = Math.max(startIndex, cellIndex);
                if(isCol && activeCellRange.leftColIndex < minIndex || !isCol && activeCellRange.topRowIndex < minIndex)
                    findActiveCellRange(isCol, minIndex);
                else if(isCol && activeCellRange.leftColIndex > maxIndex || !isCol && activeCellRange.topRowIndex > maxIndex)
                    findActiveCellRange(isCol, maxIndex);
            }
        }
        this.onMouseUp = function(isCol) {
            startIndex = -1;
            selectionMethod = "";
            activeCellRange = null;
        };        
    };

    ASPxClientSpreadsheet.StateController = function(spreadsheetControl) {
        var paneManager = spreadsheetControl.getPaneManager();

        var states = {
            selected: new ASPxClientSpreadsheet.SelectionState(spreadsheetControl),
            editing: new ASPxClientSpreadsheet.EditingState(spreadsheetControl),
            rangeEditing: new ASPxClientSpreadsheet.RangeEditState(spreadsheetControl)
        };

        var drawingBoxSelectionStateHelper = new ASPxClientSpreadsheet.DrawingBoxSelectionStateHelper(spreadsheetControl, states.selected),
            entireRowAllColSelectionStateHelper = new ASPxClientSpreadsheet.EntireRowAllColSelectionStateHelper(spreadsheetControl, states.selected);

        var capturedEntityTypes = {
            Nothing:    0,
            Cell:       1,
            DrawingBox: 2,
            RowOrCol:   3,
            SelectionBorder: 4
        };

        var capturedEntity = capturedEntityTypes.Nothing;

        function captureCell() {
            capturedEntity = capturedEntityTypes.Cell;
        }
        function captureDrawingBox() {
            capturedEntity = capturedEntityTypes.DrawingBox;
        }
        function captureRowOrCol() {
            capturedEntity = capturedEntityTypes.RowOrCol;
        }
        function captureSelectionBorder() {
            capturedEntity = capturedEntityTypes.SelectionBorder;
        }
        function releaseCapturedEntity() {
            capturedEntity = capturedEntityTypes.Nothing;
        }
        function isCellCaptured() {
            return capturedEntity == capturedEntityTypes.Cell;
        }
        function isDrawingBoxCaptured() {
            return capturedEntity == capturedEntityTypes.DrawingBox;
        }
        function isRowOrColCaptured() {
            return capturedEntity == capturedEntityTypes.RowOrCol;
        }
        function isSelectionBorderCaptured() {
            return capturedEntity == capturedEntityTypes.SelectionBorder;
        }

        function cellSelectionStart(states, cellLayoutInfo) {
            captureCell();
            selectCell(states, cellLayoutInfo);
        }

        function selectCell(states, cellLayoutInfo) {
            states.selected.finalize();
            states.selected.bindToCell( { col: cellLayoutInfo.colIndex, row: cellLayoutInfo.rowIndex } );
            states.selected.activate();
            states.editing.finalize();
        }

        function cellSelectionContinue(states, cellLayoutInfo) {
            if(states.selected.bindToLastCell( { col: cellLayoutInfo.colIndex, row: cellLayoutInfo.rowIndex }))
                states.selected.activate();
        }

        function canStartEditing(cellLayoutInfo) {
            return states.editing.canStartEditing(cellLayoutInfo);
        }

        var isDialogOpened = false;

        this.notifyDialogOpen = function() {
            isDialogOpened = true;
        };

        this.notifyDialogClose = function() {
            isDialogOpened = false;
        };

        function canRangeEdit(evt) {
            if(isDialogOpened || ASPx.ElementHasCssClass(evt.target, "marker")) {
                return true;
            }

            var isEnterOrEditMode = spreadsheetControl.stateController.getEditMode() !== ASPxClientSpreadsheet.StateController.Modes.Ready;

            if(isEnterOrEditMode)
                return isCaretOnOperator();

            return false;
        }

        function isCaretOnOperator() {
            var editorElement = spreadsheetControl.getEditingHelper().getEditorElement(),
                editorContent = editorElement.value;

            if(!(editorContent && editorContent[0] === "="))
                return false;

            var trimmedContent = ASPx.Str.Trim(editorContent),
                caretPosition = ASPx.Selection.GetCaretPosition(editorElement),
                currentSymbol = trimmedContent[(caretPosition || trimmedContent.length) - 1],
                operators = ["+", "-", "*", "/", "(", "^", "="];

            return ASPx.Data.ArrayContains(operators, currentSymbol);
        }
        
        this.onDrawingBoxEvent = function(evt, drawingBoxElement, drawingBoxIndex) {
            if(evt.type == ASPx.TouchUIHelper.touchMouseDownEventName) {
                if(ASPx.Browser.TouchUI)
                    spreadsheetControl.getPaneManager().hideTouchSelectionElements(false);
                captureDrawingBox();
                if(drawingBoxIndex > -1) {
                    states.selected.finalize();
                    states.selected.selectDrawingBoxIndex(drawingBoxElement, drawingBoxIndex);

                    drawingBoxSelectionStateHelper.captureElement(evt, drawingBoxElement, drawingBoxIndex);
                    states.selected.activate();
                    states.editing.finalize();

                    drawingBoxSelectionStateHelper.onMouseDown(evt, drawingBoxElement);
                } else {
                    drawingBoxSelectionStateHelper.onMouseDown(evt, drawingBoxElement);
                }
            } else if(evt.type == ASPx.TouchUIHelper.touchMouseMoveEventName) {
                if(isDrawingBoxCaptured())
                    drawingBoxSelectionStateHelper.onMouseMove(evt, drawingBoxElement);
            } else if(evt.type == ASPx.TouchUIHelper.touchMouseUpEventName) {
                if(isDrawingBoxCaptured())
                    drawingBoxSelectionStateHelper.onMouseUp(evt, drawingBoxElement);
                releaseCapturedEntity();
            }
        };
        
        this.onCellEvent = function(evt, cellLayoutInfo, eventFromCellTextViewElement, paneType) {
            if(!cellLayoutInfo || (!cellLayoutInfo.valid && evt.type !== ASPx.TouchUIHelper.touchMouseUpEventName))
                return;

            var srcElement = ASPx.Evt.GetEventSource(evt);

            if(spreadsheetControl.getRenderProvider().getIsCellEditingElement(srcElement)) {
                return;
            }
            if(ASPx.Browser.MSTouchUI && spreadsheetControl.getRenderProvider().getGridContainer()['MSGestureTapStopPropagation']) {
                spreadsheetControl.getRenderProvider().getGridContainer()['MSGestureTapStopPropagation'] = false;
                return;
            }

            if(evt.type == ASPx.TouchUIHelper.touchMouseDownEventName) {
                onCellMouseDown(evt, cellLayoutInfo, srcElement, paneType);
            } else if(evt.type == "MSGestureTap" && !spreadsheetControl.getRenderProvider().getIsTouchSelectionElement(srcElement)) {
                onCellFastTap(cellLayoutInfo);
            } else if(evt.type == ASPx.TouchUIHelper.touchMouseMoveEventName && ASPx.Evt.IsLeftButtonPressed(evt)) {
                onCellLeftButtonMouseMove(evt, cellLayoutInfo, srcElement);
            } else if(dblClickEvent(evt, srcElement)) {
                startCellEditing(cellLayoutInfo);
            } else if(evt.type == ASPx.TouchUIHelper.touchMouseUpEventName) {
                onCellMouseUp(evt, srcElement);
            } else {
                var isFormulaBarStartEditing = eventFromCellTextViewElement
                    && (evt.type == "click" || evt.type == "keyup");
                if(isFormulaBarStartEditing)
                    startCellEditing(cellLayoutInfo, true);
            }
        };
        function dblClickEvent(evt, srcElement) {
            return (evt.type == "dblclick" || evt.type == "touchend" && evt[ASPx.TouchUIHelper.doubleTapEventName]) &&
                !spreadsheetControl.getRenderProvider().getIsDropDownButtonElement(srcElement) &&
                !ASPx.GetIsParent(spreadsheetControl.getValidationHelper().getDropDownPanel(), srcElement);
        }

        function onSelectionBorderMouseDown() {
            captureSelectionBorder();
        }
        function onSelectionBorderMouseMove(selection, cellLayoutInfo) {
            spreadsheetControl.getPaneManager().drawSelectionMovementRect(selection, cellLayoutInfo);
        }
        function onSelectionBorderMouseUp() {
            var paneManager = spreadsheetControl.getPaneManager();
            var rectLayoutInfo = paneManager.getSelectionMovementLayoutInfo();
            if(rectLayoutInfo) {
                var selection = getSelectionInternal();
                if(selection.range.leftColIndex !== rectLayoutInfo.leftTop.colIndex ||
                    selection.range.topRowIndex !== rectLayoutInfo.leftTop.rowIndex ||
                    selection.range.rightColIndex !== rectLayoutInfo.rightBottom.colIndex ||
                    selection.range.bottomRowIndex !== rectLayoutInfo.rightBottom.rowIndex) {
                        var leftTopModelPosition = paneManager.convertVisiblePositionToModelPosition(rectLayoutInfo.leftTop.colIndex, rectLayoutInfo.leftTop.rowIndex),
                            rightBottomModelPosition = paneManager.convertVisiblePositionToModelPosition(rectLayoutInfo.rightBottom.colIndex, rectLayoutInfo.rightBottom.rowIndex);

                        spreadsheetControl.onServerCommand(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("MoveRange").id, 
                            { Target: ASPx.Json.ToJson([leftTopModelPosition.colIndex, leftTopModelPosition.rowIndex, 
                                rightBottomModelPosition.colIndex, rightBottomModelPosition.rowIndex]) });
                }
            }
            spreadsheetControl.getPaneManager().hideSelectionMovementElement();
        }
        function onCellMouseDown(evt, cellLayoutInfo, srcElement, paneType) {
            if(spreadsheetControl.getRenderProvider().getIsSelectionMovementBorderElement(srcElement) && ASPx.Evt.IsLeftButtonPressed(evt) && evt.type !== "touchstart") {
                onSelectionBorderMouseDown();
            } else if(evt.shiftKey && ASPx.Evt.IsLeftButtonPressed(evt)) {
                cellSelectionContinue(states, cellLayoutInfo);
            } else if(canRangeEdit(evt)) {
                states.rangeEditing.activate(cellLayoutInfo);
            } else if(ASPx.Evt.IsLeftButtonPressed(evt) || !states.selected.isCellSelected(cellLayoutInfo.colIndex, cellLayoutInfo.rowIndex)) {
                if(ASPx.Browser.TouchUI) {
                    if(evt.touches && evt.touches.length > 1) {
                        return;
                    }
                    if(spreadsheetControl.getRenderProvider().getIsTouchSelectionElement(srcElement)) {
                        ASPx.Evt.PreventEventAndBubble(evt);
                        if(getSelectionInternal().entireColsSelected || getSelectionInternal().entireRowsSelected) {
                            captureRowOrCol();
                            entireRowAllColSelectionStateHelper.changeStartParams(spreadsheetControl.getPaneManager().isRightBottomTouchSelectionElement(srcElement, paneType));
                        } else {
                            captureCell();
                            states.selected.rebindFirstCellDiagonally(spreadsheetControl.getPaneManager().isRightBottomTouchSelectionElement(srcElement, paneType));
                        }
                        if(ASPx.Browser.TouchUI)
                            spreadsheetControl.getPaneManager().hideTouchSelectionElements(false);
                    } else if(ASPx.Browser.WebKitTouchUI) {
                        ASPx.TouchUIHelper.FastTapHelper.HandleFastTap(evt, function() {
                            onCellFastTap(cellLayoutInfo);
                        }, false);
                    }
                } else {
                    cellSelectionStart(states, cellLayoutInfo);
                    return ASPx.Evt.PreventEventAndBubble(evt);
                }
            }
        }
        function onCellMouseUp(evt, srcElement) {
            if(ASPx.GetIsParent(spreadsheetControl.getValidationHelper().getDropDownPanel(), srcElement)) {
                spreadsheetControl.getValidationHelper().onDropDownPanelMouseUp(srcElement);
            } else if(spreadsheetControl.getRenderProvider().getIsDropDownButtonElement(srcElement)) {
                spreadsheetControl.getValidationHelper().onDropDownButtonMouseUp();
            }
            if(states.rangeEditing.isActivated())
                states.rangeEditing.finalize();

            onSelectionBorderMouseUp();
            releaseCapturedEntity();
            
            if(ASPx.Browser.TouchUI)
                spreadsheetControl.getPaneManager().hideTouchSelectionElements(true);
        }
        function onCellLeftButtonMouseMove(evt, cellLayoutInfo, srcElement) {
            if(ASPx.Browser.TouchUI) {
                if(spreadsheetControl.getRenderProvider().getIsTouchSelectionElement(srcElement))
                    ASPx.Evt.PreventEventAndBubble(evt);
                if(isRowOrColCaptured()) {
                    var selection = getSelectionInternal();
                    if(selection.entireColsSelected) {
                        entireRowAllColSelectionStateHelper.onMouseMove(cellLayoutInfo.colIndex, true);
                        showTouchResizeElement(selection.range.rightColIndex, true);
                    }
                    if(selection.entireRowsSelected) {
                        entireRowAllColSelectionStateHelper.onMouseMove(cellLayoutInfo.rowIndex, false);
                        showTouchResizeElement(selection.range.bottomRowIndex, false);
                    }
                }
            }
            if(states.rangeEditing.isActivated()) {
                states.rangeEditing.continueSelection(cellLayoutInfo);
            } else if(isCellCaptured()) {
                cellSelectionContinue(states, cellLayoutInfo);
            } else if(isSelectionBorderCaptured()) {
                onSelectionBorderMouseMove(getSelectionInternal(), cellLayoutInfo);
			}
        }
        function onCellFastTap(cellLayoutInfo) {
            selectCell(states, cellLayoutInfo);
            if(ASPx.Browser.TouchUI) {
                var gridResizeHelper = spreadsheetControl.getGridResizingHelper();
                gridResizeHelper.hideTouchResizeElement();
                spreadsheetControl.getPaneManager().hideTouchSelectionElements(true);
            }
        }
        
        this.onHeaderEvent = function(evt, isCol, paneType) {
            var gridResizeHelper = spreadsheetControl.getGridResizingHelper(),
                canStartResizing = spreadsheetControl.CanChangeSheetStylesAndLayout() && gridResizeHelper.canStartResizing(evt, isCol, paneType)
                    && this.getEditMode() === ASPxClientSpreadsheet.StateController.Modes.Ready;

            if(evt.type == "mousemove")
                gridResizeHelper.updateCursor(canStartResizing, isCol, paneType);

            if(evt.type == "dblclick" && canStartResizing)
                gridResizeHelper.onDoubleClick(evt, isCol, paneType);
            else if(gridResizeHelper.isResizing() || canStartResizing)
                onHeaderResizingEvent(evt, isCol, gridResizeHelper, paneType);
            else
                onHeaderSelectionEvent(evt, isCol);
        };
        function onHeaderResizingEvent(evt, isCol, gridResizeHelper, paneType) {
            if(evt.type == ASPx.TouchUIHelper.touchMouseDownEventName) {
                gridResizeHelper.onMouseDown(evt, isCol, paneType)
            } else if(evt.type == ASPx.TouchUIHelper.touchMouseMoveEventName) {
                gridResizeHelper.onMouseMove(evt, isCol, paneType);
            }
        }
        function onHeaderSelectionEvent(evt, isCol) {
            var headerCellElement = ASPx.Evt.GetEventSource(evt);

            if(evt.type == "mouseup" && !ASPx.Browser.MSTouchUI)
                entireRowAllColSelectionStateHelper.onMouseUp(isCol);

            if(!ASPx.ElementHasCssClass(headerCellElement, ASPx.SpreadsheetCssClasses.HeaderCell))  return;
            var cellIndex = spreadsheetControl.getRenderProvider().getHeaderCellElementIndex(headerCellElement, isCol);

            if(evt.type == "mousedown" && !ASPx.Browser.MSTouchUI) {
                onHeaderSelectionMouseDown(evt, cellIndex, isCol);
            } else if(evt.type == "touchstart") {
                ASPx.TouchUIHelper.FastTapHelper.HandleFastTap(evt, function() {
                    onHeaderSelectionFastTap(evt, cellIndex, isCol);
                }, false);
            } else if(ASPx.Browser.MSTouchUI && evt.type == "click") {
                onHeaderSelectionFastTap(evt, cellIndex, isCol);
            } else if(evt.type == "mousemove" && !ASPx.Browser.MSTouchUI) {
                entireRowAllColSelectionStateHelper.onMouseMove(cellIndex, isCol);
            }
        }
        function onHeaderSelectionMouseDown(evt, cellIndex, isCol) {
            var leftMouseButton = ASPx.Evt.IsLeftButtonPressed(evt);
            var selection = getSelectionInternal();

            var cellInSelectedRange = selection.range.isCellInRangeCore(cellIndex, isCol);
            var columnIsAlreadySelected = isCol && selection.entireColsSelected && cellInSelectedRange;
            var rowIsAlreadySelected = !isCol && selection.entireRowsSelected && cellInSelectedRange;

            var changeSelection = leftMouseButton || !(columnIsAlreadySelected || rowIsAlreadySelected);

            if(changeSelection) {
                states.selected.finalize();
                states.editing.finalize();
                entireRowAllColSelectionStateHelper.onMouseDown(cellIndex, isCol);
            }
        }
        function onHeaderSelectionFastTap(evt, cellIndex, isCol) {
            onHeaderSelectionMouseDown(evt, cellIndex, isCol);
            showTouchResizeElement(cellIndex, isCol);
            spreadsheetControl.getPaneManager().hideTouchSelectionElements(true);
        }
        function showTouchResizeElement(cellIndex, isCol) {
            var gridResizeHelper = spreadsheetControl.getGridResizingHelper();
            gridResizeHelper.showTouchResizeElement(cellIndex, isCol);
        }
        
        this.getSelection = function() {
            return getSelectionInternal();
        };
        function getSelectionInternal() {
            return states.selected.getSelection();
        };
        this.getActiveCellModelPosition = function() {
            var selection = getSelectionInternal();
            var cellModelPosition = paneManager.convertVisiblePositionToModelPosition(selection.activeCellColIndex, selection.activeCellRowIndex);
            if(paneManager.isMergerCellContainsInCroppedRange(cellModelPosition))
                cellModelPosition = paneManager.getLeftTopModelCellPositionInCroppedRange(cellModelPosition);
            return cellModelPosition;
        };
        
        this.setSelection = function(selection) {
            states.selected.finalize();
            states.selected.setSelection(selection);
            states.selected.activate();
            states.editing.finalize();
        };

        this.setSelectionSilently = function(selection) {
            states.selected.setSelection(selection);
            states.selected.activateSilently();
            states.editing.finalize();
        };
        
        this.updateSelectionRender = function() {
            states.selected.activate();
        };
        
        this.getEditMode = function() {
            return states.editing.getEditMode();
        };
        
        this.setEditMode = function(editMode) {
            var selection = getSelectionInternal();
            var cellLayoutInfo = spreadsheetControl.getSelectionActiveCellLayoutInfoInternal(selection);
            if(!canStartEditing(cellLayoutInfo)) return;

            if(editMode === ASPxClientSpreadsheet.StateController.Modes.Enter) {
                startCellEntering(cellLayoutInfo);
            } else if(editMode === ASPxClientSpreadsheet.StateController.Modes.Edit) {
                startCellEditing(cellLayoutInfo);
            } else if(editMode === ASPxClientSpreadsheet.StateController.Modes.RangeEdit) {
                startRangeResizingInternal();
            }
        };
        function startCellEntering(cellLayoutInfo) {
            states.selected.bindToCell( {col: cellLayoutInfo.colIndex, row: cellLayoutInfo.rowIndex} );
            states.selected.activate();
            states.editing.bindToCellLayoutInfo(cellLayoutInfo);
            states.editing.activate(ASPxClientSpreadsheet.StateController.Modes.Enter);
        }
        function startCellEditing(cellLayoutInfo, isFormulaBarEditing) {
            var canStartEditing = states.editing.canStartEditing(cellLayoutInfo);
            if(isFormulaBarEditing && !canStartEditing)
                spreadsheetControl.cellTextViewElement.blur();
            if(ASPx.Browser.TouchUI && alreadyEditing(cellLayoutInfo) || !canStartEditing)
                return;
            if(!isFormulaBarEditing)
                states.selected.bindToCell( {col: cellLayoutInfo.colIndex, row: cellLayoutInfo.rowIndex} );
            states.selected.activate();
            states.editing.bindToCellLayoutInfo(cellLayoutInfo);
			var editMode = isFormulaBarEditing ?
                ASPxClientSpreadsheet.StateController.Modes.FormulaBarEdit
                : ASPxClientSpreadsheet.StateController.Modes.Edit;
            states.editing.activate(editMode);
        }
        function startRangeResizingInternal() {
            states.rangeEditing.activate();
        }
        function alreadyEditing(cellLayoutInfo) {
            return states.editing.cellLayoutInfo &&
                states.editing.cellLayoutInfo.colIndex == cellLayoutInfo.colIndex &&
                states.editing.cellLayoutInfo.rowIndex == cellLayoutInfo.rowIndex;
        }
        
        this.cancelEditing = function() {
            states.editing.cancel();
            states.selected.activate();
        };
        this.onEscape = function() {
            if(this.getEditMode() === ASPxClientSpreadsheet.StateController.Modes.Ready)
                drawingBoxSelectionStateHelper.onEscape();
            else
                this.cancelEditing();
            spreadsheetControl.updateCellTextViewElementValue();
        };
        this.onLeftTopRectMouseDown = function(evt) {
            var point = spreadsheetControl.getEventControlPoint(evt)
            var width = spreadsheetControl.getRenderProvider().getRowHeader().offsetWidth,
                height = spreadsheetControl.getRenderProvider().getColumnHeader().offsetHeight;
            if(point.x < width && point.y < height)
                this.selectAll();
        };
        this.selectAll = function() {
            if(this.getEditMode() !== ASPxClientSpreadsheet.StateController.Modes.Ready) return;
            // if(selection.isDrawingBoxSelection()) // TODO select all DrawingBoxes ()
            var selection = this.getSelection();
            selection.selectAll();
            this.setSelection(selection);
        }
        
        this.applyCurrentEdition = function() {
            states.editing.apply();
        };

        this.onCellEditorChanged = function() {
            states.rangeEditing.onCellEditorChanged();
        };
        
        this.insertSpecificFunction = function(cellLayoutInfo, name) {
            states.editing.bindToCellLayoutInfo(cellLayoutInfo);
            states.editing.insertSpecificFunction(name);
        };
        
        this.processDocumentResponse = function(responseSelection) {
            if(responseSelection) {
                var selection,
                    selectionHelper = spreadsheetControl.getSelectionHelper();
                if(responseSelection.allSelected) {
                    selection = new ASPxClientSpreadsheet.Selection();
                    selection.selectAll();
                } else if(responseSelection.drawingBoxIndexSelected) {
                    var drawingBoxElement = spreadsheetControl.getDrawingBoxByIndex(responseSelection.drawingBoxIndex);
                    selection = ASPxClientSpreadsheet.Selection.createFromDrawingBoxIndex(drawingBoxElement, responseSelection.drawingBoxIndex);
                } else if(responseSelection.entireColsSelected) {
                    selection = new ASPxClientSpreadsheet.Selection();
                    var activeCellRange = new ASPxClientSpreadsheet.Range(responseSelection.activeCol, responseSelection.activeRow, responseSelection.activeCol, responseSelection.activeRow); 
                    selection.selectEntireColumns(responseSelection.leftEntireColSelected, responseSelection.rightEntireColSelected, activeCellRange.leftColIndex, activeCellRange.topRowIndex);
                    if(ASPx.Browser.TouchUI)
                        showTouchResizeElement(responseSelection.rightEntireColSelected, true);
                } else if(responseSelection.entireRowsSelected) {
                    selection = new ASPxClientSpreadsheet.Selection();
                    var activeCellRange = new ASPxClientSpreadsheet.Range(responseSelection.activeCol, responseSelection.activeRow, responseSelection.activeCol, responseSelection.activeRow);
                    selection.selectEntireRows(responseSelection.topEntireRowSelected, responseSelection.bottomEntireRowSelected, activeCellRange.leftColIndex, activeCellRange.topRowIndex);
                    if(ASPx.Browser.TouchUI)
                        showTouchResizeElement(responseSelection.bottomEntireRowSelected, false);
                } else {
                    var range = new ASPxClientSpreadsheet.Range(responseSelection.left, responseSelection.top, responseSelection.right, responseSelection.bottom);
                    selection = new ASPxClientSpreadsheet.Selection(range, responseSelection.activeCol, responseSelection.activeRow);
                }

                selection = selection.getConvertedToVisibleIndices(paneManager);
                states.selected.setSelection(selection);
                states.selected.activate();
            }
        };
        
        this.isCellRangeSelectionInProcess = function() {
            return isCellCaptured();
        };

        this.isCellRangeEditingInProgress = function() {
            return states.rangeEditing.isActivated();
        };
        
        this.isDrawingBoxDragingOrResizingInProcess = function() {
            return isDrawingBoxCaptured();
        };

        this.isSelectionMovementInProcess = function() {
            return isSelectionBorderCaptured();
        };
        
        this.finializeProcessing = function() {
            releaseCapturedEntity();
        };
    };

    ASPxClientSpreadsheet.StateController.IsEditingMode = function(editMode) {
        return editMode === ASPxClientSpreadsheet.StateController.Modes.Edit
            || editMode === ASPxClientSpreadsheet.StateController.Modes.FormulaBarEdit;
    };

    ASPxClientSpreadsheet.StateController.Modes = {
        Ready: 0,
        Enter: 1,
        Edit: 2,
        RangeEdit: 3,
        FormulaBarEdit: 4
    };
})();