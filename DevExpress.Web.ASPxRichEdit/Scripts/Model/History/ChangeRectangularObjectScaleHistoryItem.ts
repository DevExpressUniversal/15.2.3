module __aspxRichEdit {
    export class ChangeRectangularObjectScaleHistoryItem extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemIntervalStateObject>;
        scaleX: number;
        scaleY: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, scaleX: number, scaleY: number) {
            super(modelManipulator, boundSubDocument, interval);
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }

        public redo() { 
            this.oldState = this.modelManipulator.inlineObjectManipulator.setScale(this.boundSubDocument, this.interval, this.scaleX, this.scaleY);
        }

        public undo() {
            this.modelManipulator.inlineObjectManipulator.restoreScale(this.boundSubDocument, this.oldState);
        }
    }
} 