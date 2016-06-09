module __aspxRichEdit {
    export class InsertTableCellWithShiftToTheHorizontallyCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.control.selection.getSelectedCells().length > 0;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let table = selectedCells[0][0].parentRow.parentTable;
            let subDocument = this.control.model.activeSubDocument;
            let newCells: TableCell[] = [];
            for(var i = selectedCells.length - 1; i >= 0; i--) {
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[i][0].startParagraphPosition.value);
                let row = table.rows[rowIndex];
                for(var j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                    let cellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - cell.startParagraphPosition.value);
                    newCells.push(this.insertTableCell(subDocument, table, rowIndex, cellIndex));
                }
            }
            TablesManipulator.normalizeVerticalSpans(this.control, subDocument, table);
            let newCellIntervals: FixedInterval[] = [];
            for(let i = 0, newCell: TableCell; newCell = newCells[i]; i++)
                newCellIntervals.push(newCell.getInterval());
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, subDocument, newCellIntervals, this.control.selection, UpdateInputPositionProperties.Yes, false));
            this.control.history.endTransaction();
            return true;
        }
        insertTableCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number): TableCell {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class InsertTableCellWithShiftToTheLeftCommand extends InsertTableCellWithShiftToTheHorizontallyCommandBase {
        insertTableCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number): TableCell {
            TablesManipulator.insertCellToTheLeft(this.control, this.control.model.activeSubDocument, table, rowIndex, cellIndex);
            return table.rows[rowIndex].cells[cellIndex];
        }
    }
}