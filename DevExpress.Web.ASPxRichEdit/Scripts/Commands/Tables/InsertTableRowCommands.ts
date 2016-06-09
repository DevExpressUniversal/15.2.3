module __aspxRichEdit {
    export class InsertTableRowCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.getInsertedRowCount(this.control.selection.getSelectedCells()) > 0;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let table = selectedCells[0][0].parentRow.parentTable;
            let rowCount = this.getInsertedRowCount(selectedCells);
            let patternRowIndex = this.getPatternRowIndex(selectedCells);
            let patternRow = table.rows[patternRowIndex];
            let newRows: TableRow[] = [];
            for(let i = 0; i < rowCount; i++)
                newRows.push(this.insertTableRowCore(this.control.model.activeSubDocument, table, patternRowIndex + i));
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, FixedInterval.fromPositions(newRows[0].getStartPosition(), newRows[newRows.length - 1].getEndPosition()), UpdateInputPositionProperties.Yes, false);
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.endTransaction();
            return true;
        }
        getInsertedRowCount(selectedCells: TableCell[][]): number {
            if(!selectedCells.length)
                return -1;
            let prevSelectedRowEndPosition = selectedCells[0][0].parentRow.getEndPosition();
            let result = 1;
            for(let i = 1, cells: TableCell[]; cells = selectedCells[i]; i++) {
                if(cells[0].parentRow.getStartPosition() !== prevSelectedRowEndPosition)
                    return 0;
                result++;
                prevSelectedRowEndPosition = cells[0].parentRow.getEndPosition();
            }
            return result;
        }
        protected insertTableRowCore(subDocument: SubDocument, table: Table, patternRowIndex: number): TableRow { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getPatternRowIndex(selectedCells: TableCell[][]): number {
            throw new Error(Errors.NotImplemented);
        }
    }
    export class InsertTableRowAboveCommand extends InsertTableRowCommandBase {
        protected insertTableRowCore(subDocument: SubDocument, table: Table, patternRowIndex: number): TableRow {
            TablesManipulator.insertRowAbove(this.control, subDocument, table, patternRowIndex);
            return table.rows[patternRowIndex];
        }
        protected getPatternRowIndex(selectedCells: TableCell[][]): number {
            let firstCell = selectedCells[0][0];
            return Utils.normedBinaryIndexOf(firstCell.parentRow.parentTable.rows, r => r.getStartPosition() - firstCell.startParagraphPosition.value);
        }
    }
    export class InsertTableRowBelowCommand extends InsertTableRowCommandBase {
        protected insertTableRowCore(subDocument: SubDocument, table: Table, patternRowIndex: number): TableRow {
            TablesManipulator.insertRowBelow(this.control, subDocument, table, patternRowIndex);
            return table.rows[patternRowIndex + 1];
        }
        protected getPatternRowIndex(selectedCells: TableCell[][]): number {
            let lastCell = selectedCells[selectedCells.length - 1][0];
            return Utils.normedBinaryIndexOf(lastCell.parentRow.parentTable.rows, r => r.getStartPosition() - lastCell.startParagraphPosition.value);
        }
    }
}