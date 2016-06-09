module __aspxRichEdit {
    export class RemoveHyperlinkHistoryItem extends HistoryItem {
        fieldIndex: number;
        startPos: number;
        separatorPos: number;
        endPos: number;
        hyperlinkInfo: HyperlinkInfo;

        resultEndRunHistoryItem: RemoveIntervalHistoryItem;
        separatorRunHistoryItem: RemoveIntervalHistoryItem;
        startCodeRunRunHistoryItem: RemoveIntervalHistoryItem;

        removeOperationResults: RemoveIntervalOperationResult[] = [];
        styleHistory: HistoryItem;

        constructor(modelManipulator: ModelManipulator, field: Field) {
            super(modelManipulator, modelManipulator.model.activeSubDocument);
            this.fieldIndex = field.index;
            this.startPos = field.getFieldStartPosition();
            this.separatorPos = field.getSeparatorPosition();
            this.endPos = field.getFieldEndPosition();
            this.hyperlinkInfo = field.getHyperlinkInfo().clone(); // if here exception, this mean incorrect use this history item
        }

        public redo() {
            var codeInterval: FixedInterval = FixedInterval.fromPositions(this.startPos, this.separatorPos + 1);
            var endRunNewPos: number = this.endPos - 1 - codeInterval.length;

            this.removeOperationResults = [
                this.modelManipulator.text.removeInterval(this.boundSubDocument, codeInterval, false),
                this.modelManipulator.text.removeInterval(this.boundSubDocument, new FixedInterval(endRunNewPos, 1), false)
                ];
            this.removeOperationResults[1].historyRuns[this.removeOperationResults[1].historyRuns.length - 1].offsetAtStartDocument = this.endPos - 1;
            var charDefaultStyle: CharacterStyle = this.modelManipulator.model.getCharacterStyleByName("Default Paragraph Font");
            if (!this.styleHistory)
                this.styleHistory = new ApplyCharacterStyleHistoryItem(this.modelManipulator, this.boundSubDocument, FixedInterval.fromPositions(this.startPos, endRunNewPos), charDefaultStyle);
            this.styleHistory.redo();
        }

        public undo() {
            this.styleHistory.undo();
            this.modelManipulator.text.restoreRemovedInterval(this.boundSubDocument, this.removeOperationResults[1]);
            this.modelManipulator.text.restoreRemovedInterval(this.boundSubDocument, this.removeOperationResults[0]);
        }
    }
} 