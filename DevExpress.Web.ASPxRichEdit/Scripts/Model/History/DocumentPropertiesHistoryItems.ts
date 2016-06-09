module __aspxRichEdit {
    export class DocumentDefaultTabWidthHistoryItem extends HistoryItem {
        oldDefaultTabWidth: number;
        newDefaultTabWidth: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument, newDefaultTabWidth: number) {
            super(modelManipulator, boundSubDocument);
            this.newDefaultTabWidth = newDefaultTabWidth;
        }

        public redo() {
            this.oldDefaultTabWidth = this.modelManipulator.documentPropertiesManipulator.setDefaultTabWidth(this.boundSubDocument, this.newDefaultTabWidth);
        }

        public undo() {
            this.modelManipulator.documentPropertiesManipulator.setDefaultTabWidth(this.boundSubDocument, this.oldDefaultTabWidth);
        }
    }

    export class PageColorHistoryItem extends HistoryItem {
        oldPageColor: number;
        newPageColor: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument, newPageColor: number) {
            super(modelManipulator, boundSubDocument);
            this.newPageColor = newPageColor;
        }

        public redo() {
            this.oldPageColor = this.modelManipulator.documentPropertiesManipulator.changePageColor(this.boundSubDocument, this.newPageColor);
        }

        public undo() {
            this.modelManipulator.documentPropertiesManipulator.changePageColor(this.boundSubDocument, this.oldPageColor);
        }
    }

    export class DifferentOddAndEvenPagesHistoryItem extends HistoryItem {
        newValue: boolean;
        oldValue: boolean;

        constructor(modelManipulator: ModelManipulator, boundSubDocument, newValue: boolean) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
        }

        public redo() {
            this.oldValue = this.modelManipulator.documentPropertiesManipulator.changeDifferentOddAndEvenPages(this.boundSubDocument.documentModel, this.newValue);
        }

        public undo() {
            this.modelManipulator.documentPropertiesManipulator.changeDifferentOddAndEvenPages(this.boundSubDocument.documentModel, this.oldValue);
        }
    }
} 