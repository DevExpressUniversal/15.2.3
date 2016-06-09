module __aspxRichEdit {
    export class RemoveTableRowHistoryItem extends HistoryItem {
        cellCount: number;
        table: Table;
        rowIndex: number;
        oldRow: TableRow;
        cellIntervals: FixedInterval[];

        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, table: Table, rowIndex: number) {
            super(modelManipulator, subDocument);
            this.table = table;
            this.rowIndex = rowIndex;
        }
        public redo() {
            this.cellIntervals = [];
            this.oldRow = this.table.rows[this.rowIndex];
            for(let cellIndex = 0, cell: TableCell; cell = this.oldRow.cells[cellIndex]; cellIndex++) {
                this.cellIntervals.push(FixedInterval.fromPositions(cell.startParagraphPosition.value, cell.endParagrapPosition.value));
            }
            this.modelManipulator.tables.removeRow(this.boundSubDocument, this.table.index, this.rowIndex);
        }

        public undo() {
            this.modelManipulator.tables.insertRow(this.boundSubDocument, this.table.index, this.oldRow, this.rowIndex, this.cellIntervals);
        }
    }
}