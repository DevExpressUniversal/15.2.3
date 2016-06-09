module __aspxRichEdit {
    export class MouseHandlerContinueSelectionByRangesState extends MouseHandlerStateBase {
        onMouseMove(evt: RichMouseEvent) {
            if((evt.button & MouseButton.Left) === 0)
                this.stopProcessing();
            var htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
            if(htr)
                this.continueSelection(htr);
        }
        onMouseUp(evt: RichMouseEvent) {
            this.stopProcessing();
        }

        start() {
            this.handler.control.bars.beginUpdate();
            this.handler.control.horizontalRulerControl.beginUpdate();
        }

        finish() {
            this.handler.control.inputPosition.reset();
            this.handler.control.bars.endUpdate();
            this.handler.control.horizontalRulerControl.endUpdate();
            this.handler.control.bars.updateItemsState();
            this.handler.control.horizontalRulerControl.update();
        }

        stopProcessing() {
            this.handler.switchToDefaultState();
        }

        continueSelection(htr: HitTestResult) {
            var command = this.getExtendSelectionCommand();
            var parameter = this.getExtendSelectionCommandParameter(htr);
            command.execute(parameter);
        }

        getExtendSelectionCommand(): ICommand {
            throw new Error(Errors.NotImplemented);
        }
        getExtendSelectionCommandParameter(htr: HitTestResult): any {
            return undefined;
        }
    }

    // TODO: i'll rename it later
    export class MouseHandlerBeginMultiselectionState extends MouseHandlerStateBase {
        startPosition: number;
        constructor(handler: MouseHandler, startPosition: number) {
            super(handler);
            this.startPosition = startPosition;
        }
        onMouseUp(evt: RichMouseEvent) {
            this.handler.switchToDefaultState();
            this.handler.state.onMouseUp(evt);
        }
        onMouseMove(evt: RichMouseEvent) {
            this.updateSelection(evt);
        }
        onMouseWheel(evt: RichMouseEvent) {
            this.updateSelection(evt);
        }
        updateSelection(evt: RichMouseEvent) {
            var htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
            if(htr) {
                var newPosition = htr.getPosition();
                if(newPosition !== this.startPosition) {
                    this.handler.control.selection.addSelection(this.startPosition, newPosition, false, -1);
                    this.handler.switchState(new MouseHandlerContinueSelectionByCharactersState(this.handler));
                }
            }
        }
    }

    export class MouseHandlerContinueSelectionByCharactersState extends MouseHandlerContinueSelectionByRangesState {
        continueSelection(htr: HitTestResult) {
            this.handler.control.selection.extendLastSelection(htr.getPosition(), false, -1, UpdateInputPositionProperties.No);
        }
    }

    export class MouseHandlerContinueSelectionByLinesState extends MouseHandlerContinueSelectionByRangesState {
        getExtendSelectionCommand(): ICommand {
            return this.handler.control.commandManager.getCommand(RichEditClientCommand.ExtendSelectLineNoUpdateControlState);
        }
        getExtendSelectionCommandParameter(htr: HitTestResult): any {
            return htr.getRelatedSubDocumentPagePosition() + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.columnOffset;
        }
    }

    export class MouseHandlerContinueSelectionByTableColumnsState extends MouseHandlerContinueSelectionByRangesState {
        table: LayoutTableColumnInfo;
        startColumnIndex: number;
        columnOffsetX: number;
        lastColumnIndex: number;
        constructor(handler: MouseHandler, table: LayoutTableColumnInfo, startColumnIndex: number, columnOffsetX: number) {
            super(handler);
            this.table = table;
            this.startColumnIndex = startColumnIndex;
            this.lastColumnIndex = startColumnIndex;
            this.columnOffsetX = columnOffsetX;
        }
        onMouseMove(evt: RichMouseEvent) {
            if(evt.point.isEmpty())
                return;
            let relativeX = evt.point.pageX - this.columnOffsetX - this.table.x;
            let columnIndex = Math.max(0, Utils.normedBinaryIndexOf(this.table.logicInfo.grid.columnsXPositions, posX => posX - relativeX));
            if(columnIndex !== this.lastColumnIndex) {
                let shouldExtend = evt.modifiers & KeyModifiers.Ctrl;
                let cmd = this.handler.control.commandManager.getCommand(shouldExtend ? RichEditClientCommand.ExtendSelectTableColumn : RichEditClientCommand.SelectTableColumn);
                let columnIndices: number[] = [];
                let startColumnIndex = Math.min(this.startColumnIndex, columnIndex);
                let endColumnIndex = Math.max(this.startColumnIndex, columnIndex);
                for(let i = startColumnIndex; i <= endColumnIndex; i++)
                    columnIndices.push(i);
                cmd.execute({ table: this.table.logicInfo.grid.table, columnIndices: columnIndices });
                this.lastColumnIndex = columnIndex;
            }
        }
    }

    export class MouseHandlerContinueSelectionByTableRowsState extends MouseHandlerContinueSelectionByCharactersState {
        table: LayoutTableColumnInfo;
        startRowIndex: number;
        lastRowIndex: number;
        constructor(handler: MouseHandler, table: LayoutTableColumnInfo, startRowIndex: number) {
            super(handler);
            this.table = table;
            this.startRowIndex = startRowIndex;
            this.lastRowIndex = startRowIndex;
        }

        onMouseMove(evt: RichMouseEvent) {
            let htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
            if(!htr)
                return;
            let shouldExtend = evt.modifiers & KeyModifiers.Ctrl;
            let position = htr.getPosition();
            let modelTable = this.table.logicInfo.grid.table;
            let rowIndex: number;
            let shouldContinue = false;
            let forwardDirection: boolean;
            if(position < modelTable.getStartPosition()) {
                rowIndex = 0;
                shouldContinue = true;
                forwardDirection = false;
            }
            else if(position > modelTable.getEndPosition()) {
                rowIndex = modelTable.rows.length - 1;
                shouldContinue = true;
                forwardDirection = true;
            }
            else {
                rowIndex = Utils.normedBinaryIndexOf(modelTable.rows, r => r.getStartPosition() - position);
                forwardDirection = rowIndex >= this.startRowIndex;
            }
            let selection = this.handler.control.selection;
            if(this.lastRowIndex !== rowIndex || (!shouldContinue && (selection.intervals[0].start < modelTable.getStartPosition() || selection.intervals[selection.intervals.length - 1].end() > modelTable.getEndPosition()))) {
                let cmd = this.handler.control.commandManager.getCommand(shouldExtend ? RichEditClientCommand.ExtendSelectTableRow : RichEditClientCommand.SelectTableRow);
                let rowIndices: number[] = [];
                let startRowIndex = Math.min(this.startRowIndex, rowIndex);
                let endRowIndex = Math.max(this.startRowIndex, rowIndex);
                for(let i = startRowIndex; i <= endRowIndex; i++)
                    rowIndices.push(i);
                if(rowIndex < this.startRowIndex)
                    rowIndices = rowIndices.reverse();
                cmd.execute({ table: this.table.logicInfo.grid.table, rowIndices: rowIndices, forwardDirection: forwardDirection });
                this.lastRowIndex = rowIndex;
            }
            if(shouldContinue)
                this.continueSelection(htr);
        }
    }


    export class MouseHandlerContinueSelectionByTableCellsState extends MouseHandlerContinueSelectionByCharactersState {
        startCell: TableCell;
        lastCell: TableCell;
        startParentCell: TableCell;
        startPosition: number;

        constructor(handler: MouseHandler, startTable: LayoutTableColumnInfo, startRowIndex: number, startGridCellIndex: number, startPosition: number) {
            super(handler);
            let modelRow = startTable.logicInfo.grid.table.rows[startRowIndex];
            this.startCell = modelRow.cells[TableCellUtils.getCellIndexByColumnIndex(modelRow, startGridCellIndex)];
            this.lastCell = this.startCell;
            this.startParentCell = this.startCell;
            this.startPosition = startPosition;
            while(this.startParentCell.parentRow.parentTable.parentCell)
                this.startParentCell = this.startParentCell.parentRow.parentTable.parentCell;
        }

        onMouseMove(evt: RichMouseEvent) {
            let htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
            if(!htr)
                return;
            let selection = this.handler.control.selection;
            let extendSelection = !!(evt.modifiers & KeyModifiers.Ctrl);
            if(!htr.row.tableCellInfo) {
                this.selectWholeInterval(htr, extendSelection);
                return;
            }
            let modelTable = htr.row.tableCellInfo.parentRow.parentTable.logicInfo.grid.table;
            let modelRow = modelTable.rows[htr.row.tableCellInfo.parentRow.rowIndex];
            let modelCell = modelRow.cells[TableCellUtils.getCellIndexByColumnIndex(modelRow, htr.row.tableCellInfo.cellGridIndex)];
            let sameTableCells = TableCellUtils.getSameTableCells(this.startCell, modelCell);
            if(!sameTableCells) {
                this.selectWholeInterval(htr, extendSelection);
                return;
            }
            if(this.startCell === sameTableCells.lastCell) {
                if(!extendSelection && (selection.intervals.length !== 1 || (selection.intervals[0].start !== this.startPosition && selection.intervals[0].end() !== this.startPosition)))
                    this.handler.control.selection.setSelection(this.startPosition, htr.getPosition(), false, -1, UpdateInputPositionProperties.Yes);
                else
                    this.continueSelection(htr);
                return;
            }
            if(this.lastCell === sameTableCells.lastCell)
                return;
            let commandParameters = {
                firstCell: sameTableCells.firstCell,
                lastCell: sameTableCells.lastCell,
                extendSelection: extendSelection
            };
            this.handler.control.commandManager.getCommand(RichEditClientCommand.SelectTableCellsRange).execute(commandParameters);
            this.lastCell = sameTableCells.lastCell;
        }

        selectWholeInterval(htr: HitTestResult, extendSelection: boolean) {
            let selection = this.handler.control.selection;
            let position = htr.getPosition();
            let startPosition = position >= this.startCell.endParagrapPosition.value ? this.startParentCell.parentRow.getStartPosition() : this.startParentCell.parentRow.getEndPosition();
            if(selection.intervals.length > 1) {
                if(extendSelection)
                    selection.addSelection(startPosition, position, true, -1);
                else
                    selection.setSelection(startPosition, position, true, -1, UpdateInputPositionProperties.Yes);
            }
            else
                this.continueSelection(htr);
        }
    }
} 