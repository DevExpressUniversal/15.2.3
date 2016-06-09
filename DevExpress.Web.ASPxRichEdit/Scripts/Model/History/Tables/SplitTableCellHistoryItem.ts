module __aspxRichEdit {
    export class SplitTableCellToTheLeftHistoryItem extends HistoryItem {
        table: Table;
        rowIndex: number;
        cellIndex: number;
        copyProperties: boolean;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, copyProperties: boolean) {
            super(modelManipulator, boundSubDocument);
            this.table = table;
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
            this.copyProperties = copyProperties;
        }
        public redo() {
            this.modelManipulator.tables.splitTableCellHorizontally(this.boundSubDocument, this.table, this.rowIndex, this.cellIndex, false, this.copyProperties);
        }
        public undo() {
            this.modelManipulator.tables.restoreSplittedCellHorizontally(this.boundSubDocument, this.table, this.rowIndex, this.cellIndex + 1, false);
        }
    }

    export class SplitTableCellToTheRightHistoryItem extends HistoryItem {
        table: Table;
        rowIndex: number;
        cellIndex: number;
        copyProperties: boolean;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, copyProperties: boolean) {
            super(modelManipulator, boundSubDocument);
            this.table = table;
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
            this.copyProperties = copyProperties;
        }
        public redo() {
            this.modelManipulator.tables.splitTableCellHorizontally(this.boundSubDocument, this.table, this.rowIndex, this.cellIndex, true, this.copyProperties);
        }
        public undo() {
            this.modelManipulator.tables.restoreSplittedCellHorizontally(this.boundSubDocument, this.table, this.rowIndex, this.cellIndex, true);
        }
    }
}