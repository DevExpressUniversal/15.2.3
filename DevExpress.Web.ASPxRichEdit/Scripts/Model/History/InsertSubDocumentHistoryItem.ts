module __aspxRichEdit {
    // it must be the only point of execution of the insertPartSubDocumentInOtherSubDocument()
    export class InsertSubDocumentHistoryItem extends HistoryItem {
        sourceSubDocument: SubDocument;
        sourceInterval: FixedInterval;
        targetPosition: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, targetPosition: number, sourceSubDocument: SubDocument, sourceInterval: FixedInterval) {
            super(modelManipulator, boundSubDocument);
            this.sourceInterval = sourceInterval;
            this.sourceSubDocument = sourceSubDocument;
            this.targetPosition = targetPosition;
        }

        public redo() {
            this.modelManipulator.text.insertSubDocumentInOtherSubDocument(this.boundSubDocument, this.targetPosition, this.sourceSubDocument, this.sourceInterval);
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.targetPosition, this.sourceInterval.length), true);
        }
    }
} 