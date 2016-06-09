module __aspxRichEdit {
    export class DeleteTableRowsCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.getDeletingRowCount(this.control.selection.getSelectedCells()) > 0;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let subDocument = this.control.model.activeSubDocument;
            let table = selectedCells[0][0].parentRow.parentTable;
            let rowCount = this.getDeletingRowCount(selectedCells);
            if(rowCount === table.rows.length) {
                TablesManipulator.removeTableWithContent(this.control, subDocument, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.Yes, false);
            }
            else {
                let startPosition = selectedCells[0][0].startParagraphPosition.value;
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - startPosition);
                let subDocument = this.control.model.activeSubDocument;
                for(let i = 0; i < rowCount; i++)
                    TablesManipulator.removeTableRowWithContent(this.control, subDocument, table, rowIndex);
                TablesManipulator.normalizeCellColumnSpans(this.control, this.control.model.activeSubDocument, table, true);
                TableConditionalFormattingCalculator.updateTable(this.control, table);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(startPosition, 0), UpdateInputPositionProperties.Yes, false);
            }
            this.control.history.endTransaction();
            return true;
        }

        private getDeletingRowCount(selectedCells: TableCell[][]): number {
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
    }
}