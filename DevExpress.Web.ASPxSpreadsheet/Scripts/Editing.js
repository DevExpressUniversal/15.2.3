(function() {
    ASPxClientSpreadsheet.EditingHelper = function(spreadsheetControl) {
        this.spreadsheetControl = spreadsheetControl;

        var editingCellCache = {};

        this.startEditing = function(cellLayoutInfo, editMode) {
            var sizeParameters = calculateInplaceEditorSizeParameters(cellLayoutInfo),
                rect = sizeParameters.inplaceEditorRect,
                cellModelPosition = sizeParameters.inplaceEitorCellModelPosition;
            cellLayoutInfo = sizeParameters.inplaceEditorCellLayoutInfo;

            var isFormulaBarEditMode = editMode === ASPxClientSpreadsheet.StateController.Modes.FormulaBarEdit;
            var isEditingMode = ASPxClientSpreadsheet.StateController.IsEditingMode(editMode);
            var cellEditorElement = this.getEditorElement();
            var cellTextViewElement = this.spreadsheetControl.getCellTextViewElement();
            var inplaceEditingCellElement = isFormulaBarEditMode ? cellTextViewElement : cellEditorElement;
		    var inplaceFormulaBarElement  = isFormulaBarEditMode ? cellEditorElement : cellTextViewElement;
            var isEditingCellVisible = getIsEditingCellVisible(rect, cellLayoutInfo);
            var cellTextViewElementSelectionOnClick = isFormulaBarEditMode ? ASPx.Selection.GetInfo(cellTextViewElement) : null;

            if(isEditingCellVisible || isFormulaBarEditMode) {
                var rawValue = "";
                if(ASPx.Browser.IE && ASPx.Browser.Version < 9 && ASPx.IsExists(cellTextViewElement))
                    rawValue = cellTextViewElement.value;

                if(isEditingCellVisible)
                    moveElementToCell(inplaceEditingCellElement, cellLayoutInfo, rect, isFormulaBarEditMode);
                else
                    ASPx.SetElementDisplay(inplaceEditingCellElement, false);
                if(inplaceEditingCellElement === cellTextViewElement)
                    moveElementToFormulaBar(cellEditorElement);

                if(rawValue !== "")
                    cellTextViewElement.value = rawValue;

                if(isEditingMode) {
                    rawValue = ASPx.IsExists(cellTextViewElement) ? cellTextViewElement.value
                        : this.spreadsheetControl.getPaneManager().getCellEditingText(cellModelPosition.colIndex, cellModelPosition.rowIndex);
                    this.spreadsheetControl.setElementValue(cellEditorElement, rawValue);
                    if(isFormulaBarEditMode)
                        setSelectionToEditor(cellEditorElement, cellTextViewElementSelectionOnClick);
                    else if(ASPx.Browser.Safari)
                        ASPx.Selection.SetCaretPosition(cellEditorElement);
                }
                else if(editMode === ASPxClientSpreadsheet.StateController.Modes.Enter) {
                    var inputController = this.spreadsheetControl.getInputController();
                    if(inputController.editingStartedFromKeyPressEvent()) {
                        rawValue = String.fromCharCode(inputController.getMissingKeyCode());
                        this.spreadsheetControl.setElementsValue(rawValue);
                        if(ASPx.Browser.Safari) ASPx.Selection.SetCaretPosition(cellEditorElement);
                        inputController.setMissingKeyCode(-1);
                    }
                }

                editingCellCache.cellLayoutInfo = cellLayoutInfo;
                
                prepareInplaceCellElement(inplaceEditingCellElement, cellModelPosition);
                if(ASPx.IsExists(inplaceFormulaBarElement))
                    prepareInplaceFormulaBarElement(inplaceFormulaBarElement);
                onEditingStarted(cellEditorElement);

                editingCellCache.editorBorderWidth = editingCellCache.editorBorderWidth || (cellEditorElement.offsetWidth - cellEditorElement.clientWidth) / 2;

                spreadsheetControl.updateIntelliSenseElementsPosition();

            } else {
                this.stopEditing();
            }
        };

        this.apply = function(cellLayoutInfo) {
            var value = this.getEditorElement().value,
                rowModelIndex = cellLayoutInfo.rowModelIndex,
                colModelIndex = cellLayoutInfo.colModelIndex;
            if(editingCellCache.croppedRange) {
                if(rowModelIndex === editingCellCache.croppedRange.visibleModelPosition.rowIndex && editingCellCache.croppedRange.visibleModelPosition.colIndex == colModelIndex) {
                    colModelIndex = editingCellCache.croppedRange.realModelPosition.colIndex;
                    rowModelIndex = editingCellCache.croppedRange.realModelPosition.rowIndex;
                }
                editingCellCache.croppedRange = null;
            }

            var oldRawValue = this.spreadsheetControl.getPaneManager().getCellEditingText(colModelIndex, rowModelIndex);
            var isValueChanged = !((oldRawValue === value) || (oldRawValue == null && value == ""));
            if(isValueChanged) {
                this.spreadsheetControl.onCellValueChangedWithNewSelection(
                    {
                        Column: colModelIndex,
                        Row: rowModelIndex
                    }, value);
            }
            this.stopEditing();
        };

        this.onAltEnterEvent = function() {
            var editorElement = this.getEditorElement(),
                oldValue = editorElement.value,
                selectionInfo = ASPx.Selection.GetInfo(editorElement),
                newValue = [oldValue.slice(0, selectionInfo.startPos), "\r\n", oldValue.slice(selectionInfo.endPos)].join('');
            this.spreadsheetControl.setElementsValue(newValue);
            this.updateInplaceEditingCellElementSize();
            ASPx.Selection.SetCaretPosition(editorElement, selectionInfo.startPos + 1);
            if(!this.spreadsheetControl.isFormulaBarEditMode() && this.spreadsheetControl.showFormulaBar)
                scrollCellTextViewElementToNeighbourRow(1);
        };

        function scrollCellTextViewElementToNeighbourRow(step) {
            var cellTextViewElement = spreadsheetControl.getCellTextViewElement();
            var oldScrollTop = cellTextViewElement.scrollTop;
            var scrollTopDifference = cellTextViewElement.offsetHeight * step;
            cellTextViewElement.scrollTop += scrollTopDifference;
            var scrollTopHasExpectedValue = cellTextViewElement.scrollTop === oldScrollTop + scrollTopDifference;
            if(!scrollTopHasExpectedValue)
                cellTextViewElement.scrollTop = oldScrollTop;
        }

        this.stopEditing = function() {
            var cellEditorElement = this.getEditorElement();
            cellEditorElement.value = "";

            var hideEditorFunc = function() {
                ASPx.SetElementDisplay(cellEditorElement, false);
            };
            if(ASPx.Browser.Edge)
                window.setTimeout(hideEditorFunc, 0);
            else
                hideEditorFunc();

            this.spreadsheetControl.onEditingStopped();
            if(ASPx.Browser.Firefox || ASPx.Browser.Safari)
                window.setTimeout(function() { cellEditorElement.blur(); }, 0);
            editingCellCache = {};

            var cellTextViewElement = this.spreadsheetControl.getCellTextViewElement();
            if(ASPx.IsExists(cellTextViewElement)) {
                cellTextViewElement.scrollTop = 0;
                if(spreadsheetControl.isFormulaBarEditMode()) {
					moveElementToFormulaBar(cellTextViewElement);
                    prepareInplaceFormulaBarElement(cellTextViewElement);
                }
            }
            this.spreadsheetControl.hideIntelliSenseElements();
        };

        this.getEditorElement = function() {
            if(!spreadsheetControl.cellEditorElement)
                spreadsheetControl.cellEditorElement = createEditorElement(spreadsheetControl);
            return spreadsheetControl.cellEditorElement;
        };

        this.getInplaceEditingCellElement = function() {
            return spreadsheetControl.isFormulaBarEditMode() ? spreadsheetControl.getCellTextViewElement() : this.getEditorElement();
        };

        this.updateInplaceEditingCellElementSize = function() {
            var paneManager = spreadsheetControl.getPaneManager(),
                colIndex = editingCellCache.cellLayoutInfo.colIndex,
                rowIndex = editingCellCache.cellLayoutInfo.rowIndex,
                editorBorderWidth = editingCellCache.editorBorderWidth,
                sizeParameters;

            editingCellCache = {
                editorBorderWidth: editorBorderWidth,
                cellLayoutInfo: paneManager.getCellLayoutInfo(colIndex, rowIndex)
            };

            sizeParameters = calculateInplaceEditorSizeParameters(editingCellCache.cellLayoutInfo);

            var inplaceEditingCellElement = this.getInplaceEditingCellElement();
            moveElementToCell(inplaceEditingCellElement, editingCellCache.cellLayoutInfo, sizeParameters.inplaceEditorRect);
            adjustElementSize(inplaceEditingCellElement);
        };

        this.processDocumentResponse = function() {
            var editMode = spreadsheetControl.getStateController().getEditMode();
            if(ASPxClientSpreadsheet.StateController.IsEditingMode(editMode) || editMode === ASPxClientSpreadsheet.StateController.Modes.Enter)
                this.updateInplaceEditingCellElementSize();
        };

        this.createCellTextViewElement = function() {
            var cellTextViewElement = document.createElement("textarea");
            cellTextViewElement.className = ASPx.SpreadsheetCssClasses.CellTextViewElement;
            moveElementToFormulaBar(cellTextViewElement);
            prepareInplaceFormulaBarElement(cellTextViewElement);
            return cellTextViewElement;
        };

        function setSelectionToEditor(cellEditorElement, selection) {
            if(!selection) return;
            var setSelectionToEditorCore = function() {
                ASPx.Selection.Set(cellEditorElement, selection.startPos, selection.endPos);
            };
            if(ASPx.Browser.Opera || ASPx.Browser.TouchUI)
                setTimeout(function() { setSelectionToEditorCore(); }, 0);
            else
                setSelectionToEditorCore();
        }

		function getEditorCurrentRowIndex(editor) {
            var result = 0;
            var caretPosition = ASPx.Selection.GetCaretPosition(editor);
            for(var i = 0; i < caretPosition; i++)
                if(editor.value[i] === "\n")
                    result++;
            return result;
        }

        function onEditorElementValueChange(editor) {
            var cellTextViewElement = spreadsheetControl.getCellTextViewElement();
            var isFormulaBarEditMode = spreadsheetControl.isFormulaBarEditMode();
            if(ASPx.IsExists(cellTextViewElement)) {
                spreadsheetControl.setElementValue(cellTextViewElement, editor.value);
                if(!isFormulaBarEditMode)
                    cellTextViewElement.scrollTop = getEditorCurrentRowIndex(editor) * cellTextViewElement.offsetHeight;
            }
            window.setTimeout(function() { adjustElementSize(isFormulaBarEditMode ? cellTextViewElement : editor); }, 0);
        }

        function createEditorElement(spreadsheetControl) {
            var element = document.createElement("textarea");
            element.className = ASPx.SpreadsheetCssClasses.CellEditorElement;
            ASPx.Attr.SetAttribute(element, "spellcheck", "false");

            ASPx.Evt.AttachEventToElement(element, "input", function(evt) {
                onEditorElementValueChange(element);
            });

            if(ASPx.Browser.IE && ASPx.Browser.Version < 9) {
                ASPx.Evt.AttachEventToElement(element, "propertychange", function(evt) {
                    if(ASPx.GetElementDisplay(element))
                        onEditorElementValueChange(element);
                });
            }
            ASPx.Evt.AttachEventToElement(element, "keydown", function(evt) {
                spreadsheetControl.onKeyDown(evt, spreadsheetControl.getStateController().getEditMode());
                if(!spreadsheetControl.isFormulaBarEditMode() && spreadsheetControl.showFormulaBar && !spreadsheetControl.isFunctionsListBoxDisplayed()) {
                    if(ASPx.Evt.GetKeyCode(evt) === ASPx.Key.Up)
                        scrollCellTextViewElementToNeighbourRow(-1);
                    if(ASPx.Evt.GetKeyCode(evt) === ASPx.Key.Down)
                        scrollCellTextViewElementToNeighbourRow(1);
                }
            });
            ASPx.Evt.AttachEventToElement(element, "keyup", function(evt) {
                spreadsheetControl.getStateController().onCellEditorChanged();
            }, true);
            ASPx.Evt.AttachEventToElement(element, "focus", function(evt) {
                spreadsheetControl.onCellEditorElementFocused(element);
            });
            ASPx.Evt.AttachEventToElement(element, "blur", function(evt) {
                if(ASPx.Browser.IE) {
                    var isFunctionsListBoxFocused = ASPx.GetIsParent(
                        spreadsheetControl.getFunctionsListBox().GetMainElement(), ASPx.GetActiveElement());
                    if(isFunctionsListBoxFocused) {
                        setTimeout(function() { element.focus(); }, 300);
                        return;
                    }
                }
                if(!spreadsheetControl.getStateController().isCellRangeEditingInProgress()) {
                    spreadsheetControl.onCellEditorElementBlured(element);
                }
            });
            ASPx.Evt.AttachEventToElement(element, "click", function(evt) {
                spreadsheetControl.getStateController().onCellEditorChanged();
            });

            if(ASPx.Browser.WebKitTouchUI)
                ASPx.Evt.AttachEventToElement(element, ASPx.TouchUIHelper.touchMouseDownEventName, function(evt) { evt.stopPropagation(); });
            if(ASPx.Browser.MSTouchUI && ASPx.TouchUIHelper.msGestureEnabled)
                ASPx.Evt.AttachEventToElement(element, ASPx.TouchUIHelper.pointerDownEventName, function(evt) { spreadsheetControl.getRenderProvider().getGridContainer()["MSGestureTapStopPropagation"] = true; });
            spreadsheetControl.attachTooltipsToEditor(element);

            return element;
        }

        function getEditMergedCellsRange(colIndex, rowIndex) {
            var selectionHelper = spreadsheetControl.getSelectionHelper(),
                editingRange = new ASPxClientSpreadsheet.Range(colIndex, rowIndex, colIndex, rowIndex);
            return selectionHelper.ExpandRangeToMergedCellSize(editingRange);
        }

        function getElementAttributesList(skipPaddings, skipColorAndAlign) {
            var result = [
                    "fontWeight",
                    "fontFamily",
                    "fontSize",
                    "fontStyle",
                    "fontDecoration"
            ];

            if(!skipPaddings)
                result.push(
                    "padding",
                    "paddingLeft",
                    "paddingTop",
                    "paddingRight",
                    "paddingBottom"
                );
            
            if(!skipColorAndAlign)
                result.push(
                    "backgroundColor",
                    "color",
                    "verticalAlign",
                    "textAlign"
                );

            return result;
        }

        function getCorrectCellPosition() {
            var paneManager = spreadsheetControl.getPaneManager(),
                paneType = paneManager.getPaneTypeByCell(editingCellCache.activeCell),
                topLeftCell = paneManager.getPaneTopLeftCellVisiblePosition(paneType),
                activeCellInfo = editingCellCache.cellLayoutInfo,
                topLeftCellLayoutInfo = paneManager.getCellLayoutInfo(topLeftCell.col, topLeftCell.row),
                rect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCellLayoutInfo, editingCellCache.cellLayoutInfo, editingCellCache.cellLayoutInfo.tileInfo);

            return {
                x: rect.width - activeCellInfo.rect.width,
                y: rect.height - activeCellInfo.rect.height
            };
        }

        function getCorrectCellLayoutInfo(cellLayoutInfo) {
            var paneManager = spreadsheetControl.getPaneManager(),
                editingRange = getEditMergedCellsRange(cellLayoutInfo.colIndex, cellLayoutInfo.rowIndex),
                correctionWidth = 0;

            if(!editingRange.singleCell) {
                var topLeftCLI = paneManager.getCellLayoutInfo(editingRange.leftColIndex, editingRange.topRowIndex),
                    bottomRightCLI = paneManager.getCellLayoutInfo(editingRange.rightColIndex, editingRange.bottomRowIndex);

                if(topLeftCLI.paneType !== bottomRightCLI.paneType) {
                    var rightPaneTopLeftCLI = paneManager.getPaneTopLeftCellLayoutInfo(bottomRightCLI.paneType);

                    if(rightPaneTopLeftCLI.colIndex > bottomRightCLI.colIndex) { // bottom right cell is overlapped by a left pane
                        bottomRightCLI.colModelIndex = rightPaneTopLeftCLI.colModelIndex;
                        bottomRightCLI.rect.x = rightPaneTopLeftCLI.rect.x;
                        
                        correctionWidth = rightPaneTopLeftCLI.rect.width;
                    }
                }

                cellLayoutInfo.rect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(topLeftCLI, bottomRightCLI, topLeftCLI.tileInfo);

                cellLayoutInfo.rect.width -= correctionWidth;

                cellLayoutInfo.colIndex = editingCellCache.elementHasRightTextAlign ? editingRange.leftColIndex : editingRange.rightColIndex;
            }
            editingCellCache.editingRange = editingRange;

            return cellLayoutInfo;
        }

        function getWrapText(editingCellCache) {
            if(editingCellCache.wrapText === undefined) {
                var cellTextElement = spreadsheetControl.getRenderProvider().getCellTextElement(editingCellCache.cellLayoutInfo.colIndex, editingCellCache.cellLayoutInfo.rowIndex);
                editingCellCache.wrapText = ASPx.ElementHasCssClass(cellTextElement, "dxss-wrap");
            }

            return editingCellCache.wrapText;
        }

        function adjustElementSize(element) {
            if(!ASPx.IsElementVisible(element)) return;

            ensureEditingCellCache(element);

            adjustEditorWidth();
            adjustEditorLeftPosition();
            adjustEditorHeight();

            updateIntelliSenseElementsPosition();
        }

        function ensureEditingCellCache(element) {
            var renderProvider = spreadsheetControl.getRenderProvider();

            ensureActiveCell();

            editingCellCache.element = element;
            editingCellCache.elementHasRightTextAlign = ASPx.GetCurrentStyle(element).textAlign === "right";
            editingCellCache.cellLayoutInfo = getCorrectCellLayoutInfo(editingCellCache.cellLayoutInfo);
            editingCellCache.headerWidth = editingCellCache.headerWidth || renderProvider.getRowHeader().offsetWidth;
            editingCellCache.headerHeight = editingCellCache.headerHeight || renderProvider.getColumnHeader().offsetHeight;

            ensureBoundaries();

            editingCellCache.editorPosition = editingCellCache.editorPosition || getCorrectCellPosition();

            ensureCellWidth();
            ensureCellHeight();

            editingCellCache.wrapText = getWrapText(editingCellCache);
            editingCellCache.editorWidth = editingCellCache.cellWidth;
            editingCellCache.colIndex = editingCellCache.elementHasRightTextAlign ? editingCellCache.cellLayoutInfo.colIndex - 1 : editingCellCache.cellLayoutInfo.colIndex + 1;
            editingCellCache.rowIndex = editingCellCache.cellLayoutInfo.rowIndex + 1;
            editingCellCache.editorX = editingCellCache.editorPosition.x;

            var normalizedText = element.value.replace(/ /g, "'");

            editingCellCache.normalizedTextLines = normalizedText.split("\n");
            editingCellCache.fontSize = ASPx.PxToFloat(element.style.fontSize);
            editingCellCache.textSize = getMaxLineSize(editingCellCache.normalizedTextLines, element.style);
            editingCellCache.needAdjustHeight = editingCellCache.wrapText || false;
            editingCellCache.edgeReached = false;
        }

        function ensureBoundaries() {
            var cache = editingCellCache,
                renderProvider = spreadsheetControl.getRenderProvider(),
                paneManager = spreadsheetControl.getPaneManager();

            var topLeftCellLayoutInfo = paneManager.getCellLayoutInfo(cache.editingRange.leftColIndex, cache.editingRange.topRowIndex),
                topRightCellLayoutInfo = paneManager.getCellLayoutInfo(cache.editingRange.rightColIndex, cache.editingRange.topRowIndex),
                bottomRightCellLayoutInfo = paneManager.getCellLayoutInfo(cache.editingRange.rightColIndex, cache.editingRange.bottomRowIndex);

            cache.paneWidth = cache.paneWidth || renderProvider.getGridOffsetSize(topLeftCellLayoutInfo.paneType).width;
            if(topLeftCellLayoutInfo.paneType !== topRightCellLayoutInfo.paneType)
                cache.paneWidth += renderProvider.getGridOffsetSize(topRightCellLayoutInfo.paneType).width;

            cache.paneHeight = cache.paneHeight || renderProvider.getGridOffsetSize(topRightCellLayoutInfo.paneType).height;
            if(topRightCellLayoutInfo.paneType !== bottomRightCellLayoutInfo.paneType)
                cache.paneHeight += renderProvider.getGridOffsetSize(bottomRightCellLayoutInfo.paneType).height;
        }

        function ensureActiveCell() {
            if(!editingCellCache.activeCell) {
                editingCellCache.activeCell = {
                    col: editingCellCache.cellLayoutInfo.colIndex,
                    row: editingCellCache.cellLayoutInfo.rowIndex
                }
            }
        }
        function ensureCellWidth() {
            if(!editingCellCache.cellWidth)
                editingCellCache.cellWidth = calculateCellWidth();
        }
        function calculateCellWidth() {
            return Math.min(editingCellCache.cellLayoutInfo.rect.width, editingCellCache.paneWidth - editingCellCache.editorPosition.x) - editingCellCache.editorBorderWidth;
        }
        function ensureCellHeight() {
            if(!editingCellCache.cellHeight)
                editingCellCache.cellHeight = calculateCellHeight();
        }
        function calculateCellHeight() {
            return Math.min(editingCellCache.cellLayoutInfo.rect.height, editingCellCache.paneHeight - editingCellCache.editorPosition.y) - editingCellCache.editorBorderWidth;
        }

        function adjustEditorWidth() {
            var cache = editingCellCache;

            while(shouldAdjustWidth())
                adjustWidthCore();

            editingCellCache.editorWidth -= ASPx.GetLeftRightPaddings(cache.element);

            cache.element.style.width = cache.editorWidth + "px";
        }
        function shouldAdjustWidth() {
            return editingCellCache.editorWidth < editingCellCache.textSize.width && !editingCellCache.wrapText && !editingCellCache.edgeReached;
        }
        function adjustWidthCore() {
            var cache = editingCellCache,
                nextCell = getNextHorizontalCell(),
                remainingSpace = getRemainingSpace(),
                isNextCellPartiallyHidden = !nextCell.rect || remainingSpace <= nextCell.rect.width;

            if(isNextCellPartiallyHidden) {
                cache.editorWidth += remainingSpace;
                if(cache.elementHasRightTextAlign)
                    cache.editorX -= remainingSpace;

                cache.needAdjustHeight = true;
                cache.edgeReached = true;

                return;
            }
            cache.editorWidth += nextCell.rect.width;
            if(cache.elementHasRightTextAlign)
                cache.editorX -= nextCell.rect.width;

            cache.colIndex = cache.elementHasRightTextAlign ? nextCell.colIndex - 1 : nextCell.colIndex + 1;
        }

        function adjustEditorLeftPosition() {
            if(editingCellCache.elementHasRightTextAlign)
                adjustEditorLeftCore(editingCellCache.editorX - editingCellCache.editorPosition.x)
        }
        function adjustEditorLeftCore(difference) {
            var elementX = editingCellCache.cellLayoutInfo.rect.x;
            editingCellCache.tileXOffset = editingCellCache.tileXOffset || ASPxClientUtils.GetAbsoluteX(editingCellCache.cellLayoutInfo.tileInfo.htmlElement);

            ASPxClientUtils.SetAbsoluteX(editingCellCache.element, editingCellCache.tileXOffset + elementX + difference);
        }


        function adjustEditorHeight() {
            var cache = editingCellCache;
            var newLineAdded = cache.cellHeight < cache.textSize.height * cache.normalizedTextLines.length;
            cache.needAdjustHeight = cache.needAdjustHeight && cache.editorWidth <= cache.textSize.width || newLineAdded;

            if(cache.needAdjustHeight)
                adjustEditorHeightCore();
            else
                restoreEditorHeight();
        }

        function adjustEditorHeightCore() {
            if(isBottomReached())
                setEditorFixedHeight();
            else
                setEditorAutoHeight();
        }
        function restoreEditorHeight() {
            if(editingCellCache.textSize.height <= editingCellCache.cellLayoutInfo.rect.height)
                editingCellCache.element.style.height = editingCellCache.cellHeight + 'px';
        }
        function isBottomReached() {
            var cache = editingCellCache;
            cache.element.style.height = 'auto';
            return cache.editorPosition.y + calculateEditorHeight(cache.element.scrollHeight) + cache.editorBorderWidth >= cache.paneHeight;
        }
        function setEditorFixedHeight() {
            var cache = editingCellCache;
            cache.element.style.height = cache.paneHeight - cache.editorPosition.y - cache.editorBorderWidth - 1 + "px";
        }
        function setEditorAutoHeight() {
            var cache = editingCellCache,
                height;

            cache.element.style.height = 'auto';

            height = calculateEditorHeight(cache.element.scrollHeight);

            cache.element.style.height = height  + "px";
        }

        function calculateEditorHeight(currentHeight) {
            editingCellCache.nextRowIndex = editingCellCache.rowIndex;

            var height = editingCellCache.cellHeight,
                nextCell = getNextVerticalCell();

            while(currentHeight > height) {
                height += nextCell.rect.height;
                nextCell = getNextVerticalCell();
            }

            return height;
        }

        function getNextVerticalCell() {
            var colIndex = editingCellCache.cellLayoutInfo.colIndex,
                rowIndex = editingCellCache.nextRowIndex++;

            return getNextCellInfo(colIndex, rowIndex);
        }
        function getNextHorizontalCell() {
            var colIndex = editingCellCache.elementHasRightTextAlign ? editingCellCache.colIndex-- : editingCellCache.colIndex++,
                rowIndex = editingCellCache.cellLayoutInfo.rowIndex;

            return getNextCellInfo(colIndex, rowIndex);
        }

        function getNextCellInfo(colIndex, rowIndex) {
            var paneManager = spreadsheetControl.getPaneManager(),
                nextCellLayoutInfo = paneManager.getCellLayoutInfo(colIndex, rowIndex);

            return getCorrectCellLayoutInfo(nextCellLayoutInfo);
        }
        function getRemainingSpace() {
            var cache = editingCellCache,
                freezePaneBorder = 1,
                space;

            if(cache.elementHasRightTextAlign)
                space = cache.editorX;
            else
                space = cache.paneWidth - cache.editorPosition.x - cache.editorWidth - cache.editorBorderWidth - freezePaneBorder;

            return space;
        }

        function getMaxLineSize(normalizedTextLines, editorStyle) {
            var maxSize = ASPx.GetSizeOfText(normalizedTextLines[0] + "0", editorStyle);
            for(var i = 1; i < normalizedTextLines.length; i++) {
                var size = ASPx.GetSizeOfText(normalizedTextLines[i], editorStyle);
                if(maxSize.width < size.width)
                    maxSize = size;
            }

            if(normalizedTextLines.length === 1)
                maxSize.height = fitTextHeightToCell(maxSize.height);

            return maxSize;
        }

        function fitTextHeightToCell(textHeight) {
            var textSizeOverflow = textHeight - editingCellCache.cellHeight,
                fontSizeOverflow = editingCellCache.fontSize - editingCellCache.cellHeight;

            if(textSizeOverflow > 0 && (fontSizeOverflow < editingCellCache.fontSize / 8))
                textHeight = editingCellCache.cellHeight - 1;

            return textHeight;
        }

        function updateIntelliSenseElementsPosition() {
            if(spreadsheetControl.getStateController().getEditMode() !== ASPxClientSpreadsheet.StateController.Modes.FormulaBarEdit)
                spreadsheetControl.updateIntelliSenseElementsPosition();
        }

        function moveElementToFormulaBar(element) {
            ensureElementParent(element, true);
            spreadsheetControl.locateElementAboveFBPlaceholder(element);
        }

        function moveElementToCell(element, cellLayoutInfo, rect) {
            if(spreadsheetControl.isFormulaBarEditMode())
                correctCellTextViewElementPosition(rect);
            tileContainerElementPlacementApproach(element, cellLayoutInfo, rect);
        }
        function correctCellTextViewElementPosition(rect) {
            rect.x -= 1;
            rect.y -= 1;
            rect.width += 1;
            rect.height += 1;
        }

        function tileContainerElementPlacementApproach(element, cellLayoutInfo, rect) {
            ensureElementParent(element, false, cellLayoutInfo.paneType);
            ASPx.SetElementDisplay(element, true);
            ASPxClientSpreadsheet.RectHelper.setElementRectWithElementOffset(element, rect, cellLayoutInfo.tileInfo.htmlElement);
        }

        function ensureElementParent(element, shouldBeInFormulaBar, paneType) {
            var gridTilesContainer = spreadsheetControl.getRenderProvider().getGridTilesContainer(paneType);
            var requiredParentNode = shouldBeInFormulaBar ? spreadsheetControl.getRenderProvider().getFormulaBar() : gridTilesContainer;
            if(element.parentNode !== requiredParentNode)
                ASPxClientSpreadsheet.ElementPlacementHelper.appendChildWithCheck(element, requiredParentNode);
        }


        function calculateInplaceEditorSizeParameters(cellLayoutInfo) {
            var paneManager = spreadsheetControl.getPaneManager(),
                rect,
                editingRange = getEditMergedCellsRange(cellLayoutInfo.colIndex, cellLayoutInfo.rowIndex),
                cellModelPosition = paneManager.convertVisiblePositionToModelPosition(editingRange.leftColIndex, editingRange.topRowIndex);

            if(editingRange.singleCell) {
                rect = cellLayoutInfo.rect;
            } else {
                cellLayoutInfo.assignFrom(paneManager.getCellLayoutInfo(editingRange.leftColIndex, editingRange.topRowIndex));
                var farCellLayoutInfo = paneManager.getCellLayoutInfo(editingRange.rightColIndex, editingRange.bottomRowIndex);
                rect = ASPxClientSpreadsheet.TileHelper.getCellRangeRect(cellLayoutInfo, farCellLayoutInfo, cellLayoutInfo.tileInfo);
                if(paneManager.isMergerCellContainsInCroppedRange(cellModelPosition)) {
                    editingCellCache.croppedRange = {};
                    editingCellCache.croppedRange.visibleModelPosition = { colIndex: cellModelPosition.colIndex, rowIndex: cellModelPosition.rowIndex };
                    cellModelPosition = paneManager.getLeftTopModelCellPositionInCroppedRange(cellModelPosition);
                    editingCellCache.croppedRange.realModelPosition = { colIndex: cellModelPosition.colIndex, rowIndex: cellModelPosition.rowIndex };
                }
            }

            return { inplaceEditorRect: rect, inplaceEitorCellModelPosition: cellModelPosition, inplaceEditorCellLayoutInfo: cellLayoutInfo };
        }

        function ensureFontStyle(fbEditorPlaceHolder) {
            if(ASPx.Browser.IE && !this.fbEditorPlaceHolderFontIsActual) {
                var fbTextBoxStyle = ASPx.GetCurrentStyle(spreadsheetControl.getRenderProvider().getFormulaBarTextBoxElement());
                var fbTextBoxFontStyle = spreadsheetControl.getStyleSetByAttributes(fbTextBoxStyle, getElementAttributesList(true, true));
                ASPx.SetStyles(fbEditorPlaceHolder, fbTextBoxFontStyle);
                this.fbEditorPlaceHolderFontIsActual = true;
            }
        }

        function prepareInplaceFormulaBarElement(element) {
            var fbEditorPlaceHolder = spreadsheetControl.getRenderProvider().getFormulaBarEditorPlaceholder();
            ensureFontStyle(fbEditorPlaceHolder);
            var fbEditorPlaceHolderStyle = ASPx.GetCurrentStyle(fbEditorPlaceHolder);
            var elementStyle = spreadsheetControl.getStyleSetByAttributes(fbEditorPlaceHolderStyle, getElementAttributesList());
            ASPx.SetStyles(element, elementStyle);
            element.style.lineHeight = element.offsetHeight + "px";
        }

        function prepareInplaceCellElement(element, cellModelPosition) {
            var cellElementStyle = spreadsheetControl.getCellElementStyleByModelPosition(cellModelPosition.colIndex, cellModelPosition.rowIndex,
                    getElementAttributesList(true));

            if(element.value.indexOf('=') === 0)
                cellElementStyle.textAlign = "start";

            ASPx.SetStyles(element, cellElementStyle);
            element.style.lineHeight = "normal";

            editingCellCache.editorBorderWidth = editingCellCache.editorBorderWidth || (element.offsetWidth - element.clientWidth) / 2;

            adjustElementSize(element);

            /*Hide border on editing event*/
            if(spreadsheetControl.cellSelectionBorderElement)
                ASPx.SetElementDisplay(spreadsheetControl.cellSelectionBorderElement, false);
        }

        function getIsEditingCellVisible(rect, cellLayoutInfo) {
            var scrollHelper = spreadsheetControl.getPaneManager().getScrollHelper();

            var editorVisibleInfo = scrollHelper.isElementPartiallyVisible(rect, cellLayoutInfo);
            if(!editorVisibleInfo.fullInvisible) {
                if(editorVisibleInfo.verticalPartiallyVisible || editorVisibleInfo.horizontalPartiallyVisible)
                    scrollHelper.correctScrollPostitionsWhenEditingStarted(rect, editorVisibleInfo.horizontalPartiallyVisible, editorVisibleInfo.verticalPartiallyVisible);
            }
            return !editorVisibleInfo.fullInvisible;
        }

        function onEditingStarted(inplaceEditor) {
            spreadsheetControl.onEditingStarted();
            if(ASPx.Browser.MSTouchUI)
                window.setTimeout(function() { inplaceEditor.focus(); }, 0);
            else
                inplaceEditor.focus();
        }
    };
})();