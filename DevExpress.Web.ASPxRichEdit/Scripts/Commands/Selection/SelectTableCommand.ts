module __aspxRichEdit {
    export class SelectTableCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        addSelection(firstPos: number, lastPos: number, isFirstSelection: boolean) {
            if(isFirstSelection)
                this.control.selection.setSelection(firstPos, lastPos, false, -1, UpdateInputPositionProperties.Yes, true, false);
            else
                this.control.selection.addSelection(firstPos, lastPos, false, -1, false);
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }

    export class SelectTableCellCommand extends SelectTableCommandBase {
        executeCore(state: SimpleCommandState): boolean {
            let selectedCells = this.control.selection.getSelectedCells();
            for(let i: number = 0; i < selectedCells.length; i++)
                for(let j: number = 0; j < selectedCells[i].length; j++) {
                    let selectedCell: TableCell = selectedCells[i][j];
                    let firstPos = selectedCell.startParagraphPosition.value;
                    let lastPos = selectedCell.endParagrapPosition.value;
                    this.addSelection(firstPos, lastPos, i === 0 && j === 0);
                }
            return true;
        }
    }

    export class ExtendSelectTableCellCommand extends SelectTableCellCommand {
        addSelection(firstPos: number, lastPos: number, isFirstSelection: boolean) {
            this.control.selection.addSelection(firstPos, lastPos, false, -1);
        }
    }

    export class SelectTableColumnCommand extends SelectTableCommandBase {
        executeCore(state: SimpleCommandState, pararameter?: { table: Table; columnIndices: number[] }): boolean { // TODO Refactor it
            let table: Table;
            let columnIndices: number[];
            if(pararameter) {
                table = pararameter.table;
                columnIndices = pararameter.columnIndices;
            }
            else {
                columnIndices = [];
                let selectedCells = this.control.selection.getSelectedCells();
                table = selectedCells[0][0].parentRow.parentTable;
                let columnIndicesMap: { [index: number]: boolean } = {};
                for(let i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                    for(let j = 0, cell: TableCell; cell = horCells[j]; j++) {
                        let startColumnIndex = TableCellUtils.getStartColumnIndex(cell);
                        for(let span = 0; span < cell.columnSpan; span++) {
                            let columnIndex = startColumnIndex + span;
                            if(!columnIndicesMap[columnIndex]) {
                                columnIndices.push(columnIndex);
                                columnIndicesMap[columnIndex] = true;
                            }
                        }
                    }
                }
            }
            let isFirstItem = true;
            let prevAddedCell: TableCell = null;
            for(let i = 0, columnIndex: number; (columnIndex = columnIndices[i]) !== undefined; i++) {
                for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                    let cellIndex = TableCellUtils.getCellIndexByColumnIndex(row, columnIndex);
                    let cell = row.cells[cellIndex];
                    if(cell && prevAddedCell !== cell) {
                        let firstPos = cell.startParagraphPosition.value;
                        let lastPos = cell.endParagrapPosition.value;
                        this.addSelection(firstPos, lastPos, isFirstItem);
                        isFirstItem = false;
                    }
                    prevAddedCell = cell;
                }
            }
            return true;
        }
    }

    export class ExtendSelectTableColumnCommand extends SelectTableColumnCommand {
        addSelection(firstPos: number, lastPos: number, isFirstSelection: boolean) {
            this.control.selection.addSelection(firstPos, lastPos, false, -1);
        }
    }

    export class SelectTableRowCommand extends SelectTableCommandBase {
        executeCore(state: SimpleCommandState, pararameter?: { table: Table; rowIndices: number[], forwardDirection: boolean }): boolean {
            let table: Table;
            let rows: TableRow[] = [];
            let forwardDirection: boolean;
            if(pararameter) {
                table = pararameter.table;
                for(let i = 0, rowIndex: number; (rowIndex = pararameter.rowIndices[i]) !== undefined; i++)
                    rows.push(table.rows[rowIndex]);
                forwardDirection = pararameter.forwardDirection;
            }
            else {
                let selectedCells = this.control.selection.getSelectedCells();
                for(let i = 0; i < selectedCells.length; i++)
                    rows.push(selectedCells[i][0].parentRow);
                forwardDirection = this.control.selection.forwardDirection;
            }
            for(let i = 0, row: TableRow; row = rows[i]; i++)
                this.addSelection(forwardDirection ? row.getStartPosition() : row.getEndPosition(), forwardDirection ? row.getEndPosition() : row.getStartPosition(), i === 0);
            return true;
        }
    }

    export class ExtendSelectTableRowCommand extends SelectTableRowCommand {
        addSelection(firstPos: number, lastPos: number, isFirstSelection: boolean) {
            this.control.selection.addSelection(firstPos, lastPos, false, -1);
        }
    }

    export class SelectTableCommand extends SelectTableCommandBase {
        executeCore(state: SimpleCommandState): boolean {
            let table = Table.getTableByPosition(this.control.model.activeSubDocument.tables, this.control.selection.intervals[0].start, true);
            let firstPos = table.getFirstCell().startParagraphPosition.value;
            let lastPos = table.getLastCell().endParagrapPosition.value;
            this.addSelection(firstPos, lastPos, true);
            return true;
        }
    }

    export class ExtendSelectTableCommand extends SelectTableCommand {
        addSelection(firstPos: number, lastPos: number, isFirstSelection: boolean) {
            this.control.selection.addSelection(firstPos, lastPos, false, -1);
        }
    }

    export class SelectTableCellsRangeCommand extends SelectTableCommandBase {
        executeCore(state: SimpleCommandState, parameter: { firstCell: TableCell; lastCell: TableCell, extendSelection: boolean }): boolean {
            let forwardDirection = parameter.firstCell.startParagraphPosition.value <= parameter.lastCell.startParagraphPosition.value;
            if(parameter.lastCell.parentRow.parentTable !== parameter.firstCell.parentRow.parentTable)
                throw new Error("cells should be from the same table");
            let table = parameter.firstCell.parentRow.parentTable;

            let startColumnIndex = TableCellUtils.getStartColumnIndex(parameter.firstCell);
            let endColumnIndex = TableCellUtils.getStartColumnIndex(parameter.lastCell);

            let minColumnIndex = Math.min(startColumnIndex, endColumnIndex);
            let maxColumnIndex = Math.max(startColumnIndex + parameter.firstCell.columnSpan - 1, endColumnIndex + parameter.lastCell.columnSpan - 1);

            let startRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - parameter.firstCell.startParagraphPosition.value);
            let endRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - parameter.lastCell.startParagraphPosition.value);

            let minRowIndex = Math.min(startRowIndex, endRowIndex);
            let maxRowIndex = Math.max(startRowIndex, endRowIndex);
            let isFirstSelection = true;
            for(let rowIndex = minRowIndex; rowIndex <= maxRowIndex; rowIndex++) {
                let row = table.rows[rowIndex];
                let columnIndex = Math.max(row.gridBefore, minColumnIndex);
                if(columnIndex > maxColumnIndex)
                    continue;
                let cellIndex = TableCellUtils.getCellIndexByColumnIndex(row, columnIndex);
                while(columnIndex <= maxColumnIndex) {
                    let cell = row.cells[cellIndex];
                    if(!cell)
                        break;
                    this.addSelection(forwardDirection ? cell.startParagraphPosition.value : cell.endParagrapPosition.value, forwardDirection ? cell.endParagrapPosition.value : cell.startParagraphPosition.value, isFirstSelection && !parameter.extendSelection);
                    isFirstSelection = false;
                    columnIndex += cell.columnSpan;
                    cellIndex++;
                }
            }
            return true;
        }
    }
}