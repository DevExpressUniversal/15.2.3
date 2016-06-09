module __aspxRichEdit {
    export class RemoveIntervalHistoryItem extends IntervalBasedHistoryItem {
        result: RemoveIntervalOperationResult;
        setPropertiesSecondParagraph: boolean;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, setPropertiesSecondParagraph: boolean) {
            super(modelManipulator, boundSubDocument, interval);
            this.setPropertiesSecondParagraph = setPropertiesSecondParagraph;
        }

        public redo() {
            this.result = this.modelManipulator.text.removeInterval(this.boundSubDocument, this.interval, this.setPropertiesSecondParagraph);
        }

        public undo() {
            this.modelManipulator.text.restoreRemovedInterval(this.boundSubDocument, this.result);
        }
    }
}  