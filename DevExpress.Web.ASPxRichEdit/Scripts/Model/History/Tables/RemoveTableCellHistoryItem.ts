module __aspxRichEdit {
    export class RemoveTableCellHistoryItem extends HistoryItem {
        cellCount: number;
        table: Table;
        rowIndex: number;
        cellIndex: number;
        cell: TableCell;
        length: number;

        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            super(modelManipulator, subDocument);
            this.table = table;
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
            this.cell = table.rows[rowIndex].cells[cellIndex];
            this.length = this.cell.getInterval().length;
        }
        public redo() {
            this.modelManipulator.tables.removeCell(this.boundSubDocument, this.table, this.rowIndex, this.cellIndex);
        }

        public undo() {
            this.modelManipulator.tables.insertCell(this.boundSubDocument, this.table, this.rowIndex, this.cellIndex, this.cell, this.length);
        }
    }
}