module __aspxRichEdit {
    export class RemoveTableHistoryItem extends HistoryItem {
        cellCount: number;
        table: Table;
        cellsRanges: FixedInterval[][];

        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, table: Table) {
            super(modelManipulator, subDocument);
            this.table = table;
        }
        public redo() {
            this.cellsRanges = [];
            for(let rowIndex = 0, row: TableRow; row = this.table.rows[rowIndex]; rowIndex++) {
                let rowCellsRanges: FixedInterval[] = [];
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    rowCellsRanges.push(cell.getInterval());
                }
                this.cellsRanges.push(rowCellsRanges);
            }
            this.modelManipulator.tables.removeTable(this.boundSubDocument, this.table);
        }

        public undo() {
            this.modelManipulator.tables.restoreRemovedTable(this.boundSubDocument, this.table, this.cellsRanges);
        }
    }
}