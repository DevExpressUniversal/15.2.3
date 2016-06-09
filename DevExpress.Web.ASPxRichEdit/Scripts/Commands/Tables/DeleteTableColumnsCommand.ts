module __aspxRichEdit {
    export class DeleteTableColumnsCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.canDeleteTableColumns(this.control.selection.getSelectedCells());
        }
        canDeleteTableColumns(selectedCells: TableCell[][]): boolean {
            if(!selectedCells.length)
                return false;
            for(let i = selectedCells.length - 1; i >= 0; i--) {
                let prevCellEndPosition = selectedCells[i][0].endParagrapPosition.value;
                for(let j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                    if(j > 0 && cell.startParagraphPosition.value > prevCellEndPosition)
                        return false;
                    prevCellEndPosition = selectedCells[i][j].endParagrapPosition.value;
                }
            }
            return true;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let table = selectedCells[0][0].parentRow.parentTable;
            let subDocument = this.control.model.activeSubDocument;
            if(this.isSelectedEntireTable(table)) {
                TablesManipulator.removeTableWithContent(this.control, subDocument, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.Yes, false);
            }
            else {
                let selectedCells = this.control.selection.getSelectedCells();
                let startPosition = selectedCells[0][0].startParagraphPosition.value;
                let columnsRange = TableCellUtils.getColumnsRangeBySelectedCells(selectedCells);
                for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                    let cellIndices = TableCellUtils.getCellIndicesByColumnsRange(row, columnsRange.startColumnIndex, columnsRange.endColumnIndex);
                    if(cellIndices.length === row.cells.length) {
                        TablesManipulator.removeTableRowWithContent(this.control, subDocument, table, rowIndex);
                        rowIndex--;
                    }
                    else {
                        for(let i = cellIndices.length - 1; i >= 0; i--)
                            TablesManipulator.removeTableCellWithContent(this.control, subDocument, table, rowIndex, cellIndices[i]);
                    }
                    TablesManipulator.normalizeTableGrid(this.control, subDocument, table);
                }
                this.normalizeCellVerticalMerging(subDocument, table);
                TablesManipulator.normalizeCellColumnSpans(this.control, subDocument, table, true);
                TableConditionalFormattingCalculator.updateTable(this.control, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(startPosition, 0), UpdateInputPositionProperties.Yes, false);
            }
            this.control.history.endTransaction();
            return true;
        }
        isSelectedEntireTable(table: Table): boolean {
            if(this.control.selection.isSelectedEntireTable())
                return true;
            let deletingRange = TableCellUtils.getColumnsRangeBySelectedCells(this.control.selection.getSelectedCells());
            return deletingRange.startColumnIndex === 0 && deletingRange.endColumnIndex === TableCellUtils.getColumnCount(table) - 1;
        }
        normalizeCellVerticalMerging(subDocument: SubDocument, table: Table) {
            for(let rowIndex = table.rows.length - 1, row: TableRow; row = table.rows[rowIndex]; rowIndex--) {
                //if(this.areAllCellsVerticalMergingContinue(subDocument, table, rowIndex))
                //    continue;
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    if(cell.verticalMerging === TableCellMergingState.None)
                        continue;
                    let columnIndex = TableCellUtils.getStartColumnIndex(cell);
                    let nextRow = table.rows[rowIndex + 1];
                    if(cell.verticalMerging === TableCellMergingState.Restart) {
                        if(rowIndex === table.rows.length - 1) {
                            this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                            continue;
                        }
                        let nextRowCell = nextRow.cells[TableCellUtils.getCellIndexByColumnIndex(nextRow, columnIndex)];
                        if(!nextRowCell || nextRowCell.verticalMerging !== TableCellMergingState.Continue)
                            this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                    }
                    else {
                        let prevRow = table.rows[rowIndex - 1];
                        let prevCell = prevRow ? prevRow.cells[TableCellUtils.getCellIndexByColumnIndex(prevRow, columnIndex)] : null;
                        let nextRow = table.rows[rowIndex + 1];
                        let nextCell = nextRow ? nextRow.cells[TableCellUtils.getCellIndexByColumnIndex(nextRow, columnIndex)] : null;
                        if(!prevCell || prevCell.verticalMerging === TableCellMergingState.None) {
                            if(!nextCell || nextCell.verticalMerging !== TableCellMergingState.Continue)
                                this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                            else if(nextCell && nextCell.verticalMerging === TableCellMergingState.Continue)
                                this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.Restart));
                        }
                    }
                }
            }
        }
        areAllCellsVerticalMergingContinue(subDocument: SubDocument, table: Table, rowIndex: number): boolean {
            let row = table.rows[rowIndex];
            for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                if(cell.verticalMerging != TableCellMergingState.Continue)
                    return false;
            }
            TablesManipulator.removeTableRowWithContent(this.control, subDocument, table, rowIndex);
            return true;
        }
    }
}