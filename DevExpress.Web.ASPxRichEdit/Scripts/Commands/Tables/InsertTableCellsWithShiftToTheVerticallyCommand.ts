module __aspxRichEdit {
    export class InsertTableCellsWithShiftToTheVerticallyCommand extends CommandBase<SimpleCommandState> {
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
            let topRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[0][0].startParagraphPosition.value);
            let newCells: TableCell[] = [];
            for(let i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                let rowIndex = topRowIndex + i;
                newCells = newCells.concat(this.insertTableCellWithShiftToTheVertically(this.control.model.activeSubDocument, table, rowIndex, horCells));
            }
            let newCellIntervals: FixedInterval[] = [];
            for(let i = 0, newCell: TableCell; newCell = newCells[i]; i++)
                newCellIntervals.push(newCell.getInterval());
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, newCellIntervals, this.control.selection, UpdateInputPositionProperties.Yes, false));
            this.control.history.endTransaction();
            return true;
        }

        insertTableCellWithShiftToTheVertically(subDocument: SubDocument, table: Table, rowIndex: number, horCells: TableCell[]): TableCell[] {
            let row = horCells[0].parentRow;
            let startCellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - horCells[0].startParagraphPosition.value);
            let endCellIndex = startCellIndex + horCells.length - 1;
            let insertedRowsCount = this.calculateInsertedRowsCount(table, rowIndex, startCellIndex, endCellIndex);
            let newCells: TableCell[] = [];
            for(let i = 0; i < insertedRowsCount; i++) {
                TablesManipulator.insertRowBelow(this.control, subDocument, table, table.rows.length - 1);
                for(let cellIndex = endCellIndex; cellIndex >= startCellIndex; cellIndex--) {
                    newCells = newCells.concat(this.insertCellWithShiftToTheDown(subDocument, table, rowIndex + i, cellIndex));
                }
            }
            this.deleteTextInCell(subDocument, table, rowIndex, insertedRowsCount);
            return newCells;
        }

        calculateInsertedRowsCount(table: Table, rowIndex: number, startCellIndex: number, endCellIndex: number) {
            let row = table.rows[rowIndex];
            let result = Number.MAX_VALUE;
            for(let i = startCellIndex; i <= endCellIndex; i++) {
                let cell = row.cells[i];
                let columnIndex = TableCellUtils.getStartColumnIndex(cell);
                result = Math.min(result, TableCellUtils.getVerticalSpanCellPositions(TablePosition.createAndInit(table, rowIndex, i), columnIndex).length);
            }
            return result;
        }

        insertCellWithShiftToTheDown(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number): TableCell[] {
            let row = table.rows[rowIndex];
            let newCells: TableCell[] = [];
            for(let i = table.rows.length - 1; i > rowIndex; i--) {
                let currentRow = table.rows[i];
                let cellsCountInCurrentRow = currentRow.cells.length;
                let previousRow = table.rows[i - 1];
                if(cellIndex >= previousRow.cells.length)
                    continue;
                if(cellIndex >= cellsCountInCurrentRow) {
                    let previousRowLastCell = previousRow.cells[previousRow.cells.length - 1];
                    this.insertTableCells(subDocument, table, i, currentRow.cells.length - 1, cellIndex - cellsCountInCurrentRow + 1, previousRowLastCell.preferredWidth);
                }
                new InsertTableCellWithShiftToTheDownOperation(this.control, subDocument).execute(TablePosition.createAndInit(table, i, cellIndex), false);
                newCells.push(table.rows[i].cells[cellIndex]);
            }
            this.deleteContentInCell(subDocument, table, rowIndex, cellIndex);
            TablesManipulator.normalizeTableGrid(this.control, subDocument, table);
            TablesManipulator.normalizeCellColumnSpans(this.control, subDocument, table, true);
            return newCells;
        }

        insertTableCells(subDocument: SubDocument, table: Table, patternRowIndex: number, patternCellIndex: number, insertedCellsCount: number, preferredWidth: TableWidthUnit) {
            let row = table.rows[patternRowIndex];
            for(let i = 0; i < insertedCellsCount; i++) {
                let lastCellIndex = patternCellIndex + i;
                TablesManipulator.insertCellToTheRight(this.control, subDocument, table, patternRowIndex, lastCellIndex, false, true, false);
                this.control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(this.control.modelManipulator, subDocument, table.index, patternRowIndex, lastCellIndex + 1, preferredWidth.clone()));
            }
        }
        deleteTextInCell(subDocument: SubDocument, table: Table, rowIndex: number, rowsCount: number) {
            let startRowIndex = rowIndex + 1;
            let endRowIndex = startRowIndex + rowsCount;
            for(let i = startRowIndex; i < endRowIndex; i++) {
                let row = table.rows[i];
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    if(cell.verticalMerging === TableCellMergingState.Continue)
                        this.deleteContentInCell(subDocument, table, i, cellIndex);
                }
            }
        }
        deleteContentInCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            let cell = table.rows[rowIndex].cells[cellIndex];
            if(cell.endParagrapPosition.value - cell.startParagraphPosition.value > 1)
                ModelManipulator.removeInterval(this.control, subDocument, FixedInterval.fromPositions(cell.startParagraphPosition.value, cell.endParagrapPosition.value - 1), true);
        }
    }
}