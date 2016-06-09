module __aspxRichEdit {
    export class DeleteTableCellsWithShiftToTheHorizontallyCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && TableCellUtils.areCellsSelectedInSeries(this.control.selection.getSelectedCells());
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let subDocument = this.control.model.activeSubDocument;
            let table = selectedCells[0][0].parentRow.parentTable;
            if(this.control.selection.isSelectedEntireTable()) {
                TablesManipulator.removeTableWithContent(this.control, subDocument, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.Yes, false);
            }
            else {
                let startPosition = selectedCells[0][0].startParagraphPosition.value;
                for(var i = selectedCells.length - 1; i >= 0; i--) {
                    let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[i][0].startParagraphPosition.value);
                    let row = table.rows[rowIndex];
                    if(row.cells.length === selectedCells[i].length)
                        this.deleteEntireRow(subDocument, table, rowIndex);
                    else {
                        for(var j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                            let cellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - cell.startParagraphPosition.value);
                            if(cell.verticalMerging === TableCellMergingState.Continue)
                                continue;
                            this.deleteTableCell(subDocument, table, rowIndex, cellIndex);
                        }
                    }
                }
                TablesManipulator.normalizeVerticalSpans(this.control, subDocument, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(startPosition, 0), UpdateInputPositionProperties.Yes, false);
                TableConditionalFormattingCalculator.updateTable(this.control, table);
            }
            this.control.history.endTransaction();
            return true;
        }
        deleteEntireRow(subDocument: SubDocument, table: Table, rowIndex: number) {
            TablesManipulator.removeTableRowWithContent(this.control, subDocument, table, rowIndex);
            TablesManipulator.normalizeCellColumnSpans(this.control, subDocument, table, true);
        }
        deleteTableCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            TablesManipulator.removeTableCellWithContent(this.control, subDocument, table, rowIndex, cellIndex);
            TablesManipulator.normalizeTableGrid(this.control, subDocument, table);
            TablesManipulator.normalizeCellColumnSpans(this.control, subDocument, table, false);
        }
    }
}