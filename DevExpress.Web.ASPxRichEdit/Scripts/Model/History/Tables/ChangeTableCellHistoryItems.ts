module __aspxRichEdit {
    export class ShiftTableStartPositionToTheRightHistoryItem extends HistoryItem {
        table: Table;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, table: Table) {
            super(modelManipulator, boundSubDocument);
            this.table = table;
        }

        public redo() {
            this.modelManipulator.tables.shiftTableStartPositionToTheRight(this.boundSubDocument, this.table);
        }

        public undo() {
            this.modelManipulator.tables.restoreShiftedTableStartPositionToTheRight(this.boundSubDocument, this.table);
        }
    }

    export class TableRowConditionalFormattingHistoryItem extends HistoryItem {
        table: Table;
        formatting: ConditionalTableStyleFormatting;
        oldFormatting: ConditionalTableStyleFormatting;
        rowIndex: number;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, table: Table, rowIndex: number, formatting: ConditionalTableStyleFormatting) {
            super(modelManipulator, boundSubDocument);
            this.table = table;
            this.formatting = formatting;
            this.rowIndex = rowIndex;
        }

        public redo() {
            let row = this.table.rows[this.rowIndex];
            this.oldFormatting = row.conditionalFormatting;
            row.conditionalFormatting = this.formatting;
        }

        public undo() {
            this.table.rows[this.rowIndex].conditionalFormatting = this.oldFormatting;
        }
    }

    export class TableCellConditionalFormattingHistoryItem extends HistoryItem {
        table: Table;
        formatting: ConditionalTableStyleFormatting;
        oldFormatting: ConditionalTableStyleFormatting;
        rowIndex: number;
        cellIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, formatting: ConditionalTableStyleFormatting) {
            super(modelManipulator, boundSubDocument);
            this.table = table;
            this.formatting = formatting;
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
        }

        public redo() {
            let cell = this.table.rows[this.rowIndex].cells[this.cellIndex];
            this.oldFormatting = cell.conditionalFormatting;
            cell.conditionalFormatting = this.formatting;
        }

        public undo() {
            this.table.rows[this.rowIndex].cells[this.cellIndex].conditionalFormatting = this.oldFormatting;
        }
    }
}