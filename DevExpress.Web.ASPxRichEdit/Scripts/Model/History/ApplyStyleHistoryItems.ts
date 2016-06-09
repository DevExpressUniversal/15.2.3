module __aspxRichEdit {
    export class ApplyCharacterStyleHistoryItem extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>;
        newStyle: CharacterStyle;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, style: CharacterStyle) {
            super(modelManipulator, boundSubDocument, interval);
            this.newStyle = style;
        }

        public redo() {
            this.oldState = this.modelManipulator.styles.setCharacterStyle(this.boundSubDocument, this.interval, this.newStyle);
        }

        public undo() {
            this.modelManipulator.styles.restoreCharacterStyle(this.boundSubDocument, this.oldState);
        }
    }

    export class ApplyParagraphStyleHistoryItem extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>;
        newStyle: ParagraphStyle;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, style: ParagraphStyle) {
            super(modelManipulator, boundSubDocument, interval);
            this.newStyle = style;
        }

        public redo() {
            this.oldState = this.modelManipulator.styles.setParagraphStyle(this.boundSubDocument, this.interval, this.newStyle);
        }

        public undo() {
            this.modelManipulator.styles.restoreParagraphStyle(this.boundSubDocument, this.oldState);
        }
    }

    export class ApplyTableStyleHistoryItem extends HistoryItem {
        oldStyle: TableStyle;
        newStyle: TableStyle;
        tableIndex: number;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, style: TableStyle) {
            super(modelManipulator, boundSubDocument);
            this.newStyle = style;
            this.tableIndex = tableIndex;
        }

        public redo() {
            this.oldStyle = this.boundSubDocument.tables[this.tableIndex].style;
            this.modelManipulator.tables.setTableStyle(this.boundSubDocument, this.tableIndex, this.newStyle);
        }

        public undo() {
            this.modelManipulator.tables.setTableStyle(this.boundSubDocument, this.tableIndex, this.oldStyle);
        }
    }
} 