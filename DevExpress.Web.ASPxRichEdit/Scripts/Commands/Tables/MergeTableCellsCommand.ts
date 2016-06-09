module __aspxRichEdit {
    export class MergeTableCellsCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && TableCellUtils.isSelectedSquare(this.control.selection.getSelectedCells());
        }

        executeCore(state: SimpleCommandState): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let table = selectedCells[0][0].parentRow.parentTable;
            this.mergeCellsHorizontally(this.control.model.activeSubDocument, table, selectedCells);

            let startCell = selectedCells[0][0];
            this.mergeCellsVertically(this.control.model.activeSubDocument, table, startCell, selectedCells.length);
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, startCell.getInterval(), UpdateInputPositionProperties.Yes, false);
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.endTransaction();
            return true;
        }

        mergeCellsVertically(subDocument: SubDocument, table: Table, startCell: TableCell, rowsCount: number) {
            let columnIndex = TableCellUtils.getStartColumnIndex(startCell);
            let restartRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - startCell.startParagraphPosition.value);
            let continionRowIndex = restartRowIndex + rowsCount - 2;
            for(let i = continionRowIndex; i >= restartRowIndex; i--) {
                let row = table.rows[i];
                let mergeCellIndex = TableCellUtils.getCellIndexByColumnIndex(row, columnIndex);
                TablesManipulator.mergeTwoTableCellsVertically(this.control, subDocument, TablePosition.createAndInit(table, i, mergeCellIndex));
            }
            TablesManipulator.normalizeRows(this.control, subDocument, table);
        }
        private mergeCellsHorizontally(subDocument: SubDocument, table: Table, selectedCells: TableCell[][]) {
            let startRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[0][0].startParagraphPosition.value);
            let endRowIndex = startRowIndex + selectedCells.length - 1;
            let count = endRowIndex - startRowIndex;
            for(let i = 0; i <= count; i++) {
                let rowIndex = startRowIndex + i;
                let columnSpan = this.getCellsColumnSpan(selectedCells[i]);
                let startCell = selectedCells[i][0];
                if(startCell.verticalMerging == TableCellMergingState.Restart)
                    i += this.mergeCellsHorizontallyCore(startCell, startRowIndex + i, columnSpan);
                this.mergeTableCellsHorizontally(table, rowIndex, startCell, columnSpan);
            }
        }

        mergeCellsHorizontallyCore(startCell: TableCell, rowIndex: number, mergedCellsColumnSpan: number): number {
            let columnIndex = TableCellUtils.getStartColumnIndex(startCell);
            let row = startCell.parentRow;
            let cellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - startCell.startParagraphPosition.value);
            let verticalCellPositions = TableCellUtils.getVerticalSpanCellPositions(TablePosition.createAndInit(startCell.parentRow.parentTable, rowIndex, cellIndex), columnIndex);
            for(let i = 1, position: TablePosition; position = verticalCellPositions[i]; i++)
                this.mergeTableCellsHorizontally(startCell.parentRow.parentTable, rowIndex + 1, position.cell, mergedCellsColumnSpan);
            return verticalCellPositions.length - 1;
        }
        mergeTableCellsHorizontally(table: Table, rowIndex: number, startCell: TableCell, columnSpan: number) {
            let row = table.rows[rowIndex];
            let startCellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - startCell.startParagraphPosition.value);
            let endCellIndex = this.calculateCellIndex(row, startCellIndex, columnSpan) - 1;

            for(let i = endCellIndex; i >= startCellIndex; i--)
                TablesManipulator.mergeTwoTableCellsHorizontally(this.control, this.control.model.activeSubDocument, TablePosition.createAndInit(table, rowIndex, i));
        }
        private calculateCellIndex(row: TableRow, startCellIndex: number, columnSpan: number): number {
            let result = startCellIndex;
            for(let cell: TableCell; cell = row.cells[result]; result++) {
                columnSpan -= cell.columnSpan;
                if(columnSpan <= 0)
                    break;
            }
            return result;
        }
        private getCellsColumnSpan(cells: TableCell[]): number {
            let result = 0;
            for(let i = 0, cell: TableCell; cell = cells[i]; i++)
                result += cell.columnSpan;
            return result;
        }

        
    }
}