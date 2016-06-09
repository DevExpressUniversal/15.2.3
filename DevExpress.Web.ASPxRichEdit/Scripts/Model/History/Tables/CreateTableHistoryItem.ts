module __aspxRichEdit {
    export class CreateTableHistoryItem extends HistoryItem {
        firstParagraphIndex: number;
        rowCount: number;
        cellCount: number;
        table: Table;

        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, firstParagraphIndex: number, rowCount: number, cellCount: number) {
            super(modelManipulator, subDocument);
            this.firstParagraphIndex = firstParagraphIndex;
            this.rowCount = rowCount;
            this.cellCount = cellCount;
        }
        public redo() {
            this.table = this.modelManipulator.tables.createTable(this.boundSubDocument, this.firstParagraphIndex, this.rowCount, this.cellCount);
        }

        public undo() {
            this.modelManipulator.tables.removeTable(this.boundSubDocument, this.table);
        }
    }
}