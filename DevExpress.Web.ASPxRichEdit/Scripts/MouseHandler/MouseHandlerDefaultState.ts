module __aspxRichEdit {
    const COLUMN_SELECTION_TOPBORDER_OFFSET = 3;

    export class MouseHandlerDefaultState extends MouseHandlerStateBase {
        onMouseDoubleClick(evt: RichMouseEvent) {
            var htr: HitTestResult = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
            if (this.isLeftAreaOffset(htr, evt))
                this.handler.control.commandManager.getCommand(RichEditClientCommand.SelectParagraph).execute(htr.getPosition());
            else if(htr) {
                let activeSubDocument = this.handler.control.model.activeSubDocument;
                if(htr.exactlyDetailLevel < DocumentLayoutDetailsLevel.PageArea) {
                    if(activeSubDocument.isMain()) {
                        if (htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Top) {
                            this.handler.control.commandManager.getCommand(RichEditClientCommand.InsertHeader).execute(htr.pageIndex);
                            return;
                        }
                        else if(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Bottom) {
                            this.handler.control.commandManager.getCommand(RichEditClientCommand.InsertFooter).execute(htr.pageIndex);
                            return;
                        }
                    }
                    else if(this.shouldActivateMainArea(activeSubDocument, htr)) {
                        this.handler.control.selection.pageIndex = htr.pageIndex;
                        this.handler.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(this.handler.control.model.mainSubDocument.info);
                        this.handler.control.selection.pageIndex = -1;
                        return;
                    }
                }
                var position: number = htr.getRelatedSubDocumentPagePosition() + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.columnOffset + htr.box.rowOffset + htr.charOffset;
                if (htr.boxIndex == htr.row.boxes.length - 1 && htr.charOffset == htr.box.getLength())
                    position--; // ok, because htr.boxOffset > 0
                var startInterval: number = LayoutWordPositionHelper.getPrevWordEndPosition(this.handler.control.layout, this.handler.control.model.activeSubDocument, this.handler.control.selection, position);
                var endInterval: number = LayoutWordPositionHelper.getNextWordStartPosition(this.handler.control.layout, this.handler.control.model.activeSubDocument, this.handler.control.selection, position, true);
                if (endInterval - startInterval > 0)
                    this.handler.control.selection.setSelection(startInterval, endInterval, false, -1, UpdateInputPositionProperties.Yes);
            }
        }

        shouldActivateMainArea(activeSubDocument: SubDocument, htr: HitTestResult): boolean {
            return (activeSubDocument.isHeader() && !!(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Bottom)) ||
                (activeSubDocument.isFooter() && !!(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Top));
        }

        onMouseDown(evt: RichMouseEvent) {
            if (this.shouldProcessResizeBoxVisualizer(evt))
                this.beginResizeBoxMouseHandler(evt);
            else {
                var htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
                if(!htr)
                    return;
                if(htr.subDocument.isMain() && htr.exactlyDetailLevel < DocumentLayoutDetailsLevel.PageArea) {
                    if(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Top || htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Bottom)
                        this.handler.control.waitForDblClick();
                }
                if(evt.button === MouseButton.Right && this.handler.control.selection.getLastSelectedInterval().contains(htr.getPosition()))
                    return;
                if(this.shouldSelectEntireTableColumn(htr, evt))
                    this.beginSelectEntireTableColumn(htr, evt);
                else if(this.shouldSelectEntireTableRow(htr, evt))
                    this.beginSelectEntireTableRow(htr, evt);
                else if(this.shouldBeginDragExistingSelection(htr, evt))
                    this.beginDragExistingSelection(htr, true);
                else if(this.shouldSelectEntireTableCell(htr, evt))
                    this.beginSelectEntireTableCell(htr, evt);
                else if(this.isLeftAreaOffset(htr, evt)) {
                    if(evt.modifiers & KeyModifiers.Ctrl && this.handler.control.selection.isCollapsed())
                        this.handler.control.commandManager.getCommand(RichEditClientCommand.SelectAll).execute(htr.getPosition());
                    else
                        this.beginLineSelection(htr, evt);
                }
                else if (this.shouldSelectPicture(htr, !!(evt.modifiers & KeyModifiers.Ctrl)))
                    this.selectImage(htr);
                else if (htr.detailsLevel >= DocumentLayoutDetailsLevel.Character && evt.modifiers & KeyModifiers.Ctrl)
                    this.beginCharacterMultiSelection(htr, evt);
                else if (htr.detailsLevel >= DocumentLayoutDetailsLevel.Character || htr.deviations[DocumentLayoutDetailsLevel.Box] & HitTestDeviation.Right || evt.modifiers & KeyModifiers.Shift)
                    this.beginCharacterSelection(htr, !!(evt.modifiers & KeyModifiers.Shift));
            }
        }

        onMouseUp(evt: RichMouseEvent) {
            if(evt.button === MouseButton.Right && evt.point) {
                this.handler.control.showPopupMenu(evt.evtX, evt.evtY);
            }
            if(evt.modifiers & KeyModifiers.Ctrl) {
                var field: Field = this.handler.getHyperlinkFieldResult(evt);
                if(field)
                    this.handler.control.commandManager.getCommand(RichEditClientCommand.OpenHyperlink).execute(field);
            }
        }

        private shouldSelectEntireTableColumn(htr: HitTestResult, evt: RichMouseEvent): boolean {
            if(!htr.row)
                return false;
            if(!htr.row.tableCellInfo)
                return false;
            return !!(htr.deviations[DocumentLayoutDetailsLevel.TableCell] & HitTestDeviation.Top);
        }

        private shouldSelectEntireTableRow(htr: HitTestResult, evt: RichMouseEvent): boolean {
            if(!htr.row)
                return false;
            if(!htr.row.tableCellInfo)
                return false;
            return !!(htr.deviations[DocumentLayoutDetailsLevel.TableCell] & HitTestDeviation.Left);
        }
        private shouldSelectEntireTableCell(htr: HitTestResult, evt: RichMouseEvent): boolean {
            if(!htr.row)
                return false;
            if(!htr.row.tableCellInfo)
                return false;
            if(htr.deviations[DocumentLayoutDetailsLevel.TableCell])
                return false;
            if(evt.modifiers & KeyModifiers.Shift)
                return false;
            return true;
        }


        private beginSelectEntireTableColumn(htr: HitTestResult, evt: RichMouseEvent) {
            let shouldAddSelection = evt.modifiers & KeyModifiers.Ctrl;
            let cmd = this.handler.control.commandManager.getCommand(shouldAddSelection ? RichEditClientCommand.ExtendSelectTableColumn : RichEditClientCommand.SelectTableColumn);
            let table = htr.row.tableCellInfo.parentRow.parentTable.logicInfo.grid.table;
            let rowIndex = htr.row.tableCellInfo.parentRow.rowIndex;
            let cellGridIndex = htr.row.tableCellInfo.cellGridIndex;
            cmd.execute({ table: table, columnIndices: [cellGridIndex] });
            let selectionState = new MouseHandlerContinueSelectionByTableColumnsState(this.handler, htr.row.tableCellInfo.parentRow.parentTable, cellGridIndex, htr.column.x + htr.pageArea.x);
            let dragState = new MouseHandlerBeginDragHelperState(this.handler, selectionState);
            this.handler.switchState(dragState);
        }

        private beginSelectEntireTableRow(htr: HitTestResult, evt: RichMouseEvent) {
            let shouldAddSelection = evt.modifiers & KeyModifiers.Ctrl;
            let cmd = this.handler.control.commandManager.getCommand(shouldAddSelection ? RichEditClientCommand.ExtendSelectTableRow : RichEditClientCommand.SelectTableRow);
            let table = htr.row.tableCellInfo.parentRow.parentTable.logicInfo.grid.table;
            let rowIndex = htr.row.tableCellInfo.parentRow.rowIndex;
            cmd.execute({ table: table, rowIndices: [rowIndex] });
            let selectionState = new MouseHandlerContinueSelectionByTableRowsState(this.handler, htr.row.tableCellInfo.parentRow.parentTable, rowIndex);
            let dragState = new MouseHandlerBeginDragHelperState(this.handler, selectionState);
            this.handler.switchState(dragState);
        }

        private beginSelectEntireTableCell(htr: HitTestResult, evt: RichMouseEvent) {
            let shouldAddSelection = !!(evt.modifiers & KeyModifiers.Ctrl);
            var selection: Selection = this.handler.control.selection;
            var position = this.getPosition(htr);
            if(shouldAddSelection)
                this.handler.control.selection.addSelection(position, position, false, -1);
            else
                this.handler.control.selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
            let rowIndex = htr.row.tableCellInfo.parentRow.rowIndex;
            let gridCellIndex = htr.row.tableCellInfo.cellGridIndex;
            let selectionState = new MouseHandlerContinueSelectionByTableCellsState(this.handler, htr.row.tableCellInfo.parentRow.parentTable, rowIndex, gridCellIndex, this.getPosition(htr));
            let dragState = new MouseHandlerBeginDragHelperState(this.handler, selectionState);
            this.handler.switchState(dragState);
        }

        private beginCharacterMultiSelection(htr: HitTestResult, evt: RichMouseEvent) {
            var position = htr.getPosition();
            this.handler.switchState(new MouseHandlerBeginMultiselectionState(this.handler, position));
        }

        private isLeftAreaOffset(htr: HitTestResult, evt: RichMouseEvent): boolean {
            return htr && htr.exactlyDetailLevel == DocumentLayoutDetailsLevel.PageArea && htr.deviations[DocumentLayoutDetailsLevel.Column] & HitTestDeviation.Left &&
                evt.point.pageX <= htr.pageArea.x + htr.column.x - MouseHandler.LEFTAREA_COMMANDS_OFFSET;
        }

        private shouldProcessResizeBoxVisualizer(evt: RichMouseEvent): boolean {
            if(this.handler.resizeBoxVisualizer.shouldCapture(evt)) {
                this.beginResizeBoxMouseHandler(evt);
                return true;
            }
            else if(!evt.point.isEmpty())
                this.handler.resizeBoxVisualizer.hide();
        }

        private beginResizeBoxMouseHandler(evt: RichMouseEvent) {
            this.handler.switchState(new MouseHandlerResizeBoxState(this.handler));
            this.handler.state.onMouseDown(evt);
        }

        private beginCharacterSelection(htr: HitTestResult, extendSelection: boolean) {
            this.setStartSelection(htr, extendSelection);
            this.handler.switchState(new MouseHandlerBeginDragHelperState(this.handler, new MouseHandlerContinueSelectionByCharactersState(this.handler)));
        }

        private setStartSelection(htr: HitTestResult, extendSelection: boolean) {
            var selection: Selection = this.handler.control.selection;
            var position = this.getPosition(htr);
            var endOfLine = position === htr.getRelatedSubDocumentPagePosition() + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.getEndPosition();
            if(extendSelection)
                selection.extendLastSelection(position, endOfLine, -1, UpdateInputPositionProperties.No);
            else
                selection.setSelection(position, position, endOfLine, -1, UpdateInputPositionProperties.Yes);
        }
        private getPosition(htr: HitTestResult): number {
            var position = htr.getRelatedSubDocumentPagePosition() + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.columnOffset;
            if(htr.deviations[DocumentLayoutDetailsLevel.Box] & HitTestDeviation.Right) {
                var lastVisibleBoxInRow: LayoutBox = htr.row.getLastVisibleBox();
                if(lastVisibleBoxInRow)
                    position += lastVisibleBoxInRow.getEndPosition();
            }
            else {
                position += htr.box.rowOffset + htr.charOffset;
                if(htr.boxIndex == htr.row.boxes.length - 1 && htr.charOffset == htr.box.getLength() && !htr.box.isVisible())
                    position -= 1;
            }
            return position;
        }


        private beginLineSelection(htr: HitTestResult, evt: RichMouseEvent) {
            var lineStart = htr.getRelatedSubDocumentPagePosition() + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.columnOffset;
            if(evt.modifiers & KeyModifiers.Shift)
                this.handler.control.commandManager.getCommand(RichEditClientCommand.ExtendSelectLineNoUpdateControlState).execute(lineStart);
            else if(evt.modifiers & KeyModifiers.Ctrl)
                this.handler.control.commandManager.getCommand(RichEditClientCommand.AddSelectedLineCommandNoUpdateControlState).execute(lineStart);
            else
                this.handler.control.commandManager.getCommand(RichEditClientCommand.SelectLineNoUpdateControlState).execute(lineStart);
            this.handler.switchState(new MouseHandlerBeginDragHelperState(this.handler, new MouseHandlerContinueSelectionByLinesState(this.handler)));
        }

        private shouldSelectPicture(htr: HitTestResult, ctrlPressed: boolean): boolean {
            if(htr.detailsLevel < DocumentLayoutDetailsLevel.Box)
                return false;

            return htr.box.getType() == LayoutBoxType.Picture && !ctrlPressed;
        }

        private shouldBeginDragExistingSelection(htr: HitTestResult, evt: RichMouseEvent): boolean {
            if(!(evt.modifiers & KeyModifiers.Shift) && ControlOptions.isEnabled(this.handler.control.options.drag) && !this.handler.control.selection.isCollapsed()) {
                let position = htr.getPosition();
                for(let i = 0, interval: FixedInterval; interval = this.handler.control.selection.intervals[i]; i++) {
                    if(interval.contains(position))
                        return true;
                }
                return false;
            }
            return false;
        }

        private selectImage(htr: HitTestResult) {
            var position = htr.getPosition() - htr.charOffset;
            var selection: Selection = this.handler.control.selection;
            selection.setSelection(position, position + 1, false, -1, UpdateInputPositionProperties.Yes);
            if(ControlOptions.isEnabled(this.handler.control.options.drag))
                this.beginDragExistingSelection(htr, false);
        }

        private beginDragExistingSelection(htr: HitTestResult, resetSelectionOnMouseUp: boolean) {
            var dragState = new MouseHandlerDragContentState(this.handler);
            var state = new MouseHandlerBeginContentDragHelperState(this.handler, dragState);
            state.resetSelectionOnMouseUp = resetSelectionOnMouseUp;
            this.handler.switchState(state);
        }
    }
}  