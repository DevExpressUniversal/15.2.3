module __aspxRichEdit {
    export class DeleteTableCellsWithShiftToTheVerticallyCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && TableCellUtils.areCellsSelectedInSeries(this.control.selection.getSelectedCells());
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let table = selectedCells[0][0].parentRow.parentTable;
            if(this.control.selection.isSelectedEntireTable()) {
                TablesManipulator.removeTableWithContent(this.control, this.control.model.activeSubDocument, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.Yes, false);
            }
            else {
                let startPosition = selectedCells[0][0].startParagraphPosition.value;
                let startRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[0][0].startParagraphPosition.value);
                for(var i = selectedCells.length - 1, horCells: TableCell[]; horCells = selectedCells[i]; i--) {
                    let rowIndex = startRowIndex + i;
                    let leftCellIndex = Utils.normedBinaryIndexOf(horCells[0].parentRow.cells, c => c.startParagraphPosition.value - horCells[0].startParagraphPosition.value);
                    for(let i = horCells.length - 1, cell: TableCell; cell = horCells[i]; i--) {
                        this.removeTableCell(this.control.model.activeSubDocument, table, rowIndex, leftCellIndex + i);
                    }
                }
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(startPosition, 0), UpdateInputPositionProperties.Yes, false);
                TableConditionalFormattingCalculator.updateTable(this.control, table);
            }
            this.control.history.endTransaction();
            return true;
        }
        removeTableCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            let cell = table.rows[rowIndex].cells[cellIndex];
            let columnIndex = TableCellUtils.getStartColumnIndex(cell);
            let rowsCount = table.rows.length;
            for(let i = rowIndex; i < rowsCount; i++) {
                let row = table.rows[i];
                let cellIndex = TableCellUtils.getCellIndexByColumnIndex(table.rows[rowIndex], columnIndex);
                if(row.cells[cellIndex])
                    new DeleteOneTableCellWithShiftToTheUpOperation(this.control, subDocument).execute(TablePosition.createAndInit(table, i, cellIndex), false);
            }
        }
    }
}