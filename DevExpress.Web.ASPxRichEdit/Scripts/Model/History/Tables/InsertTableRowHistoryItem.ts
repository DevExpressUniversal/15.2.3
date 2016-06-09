module __aspxRichEdit {
    export class InsertTableRowHistoryItem extends HistoryItem {
        tableIndex: number;
        targetRowIndex: number;
        cellIntervals: FixedInterval[];
        patternRow: TableRow;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, patternRow: TableRow, targetRowIndex: number, cellIntervals: FixedInterval[]) {
            super(modelManipulator, boundSubDocument);
            this.tableIndex = tableIndex;
            this.targetRowIndex = targetRowIndex;
            this.cellIntervals = cellIntervals;
            this.patternRow = patternRow;
        }

        public redo() {
            this.modelManipulator.tables.insertRow(this.boundSubDocument, this.tableIndex, this.patternRow, this.targetRowIndex, this.cellIntervals);
        }
        public undo() {
            this.modelManipulator.tables.removeRow(this.boundSubDocument, this.tableIndex, this.targetRowIndex);
        }
    }
}