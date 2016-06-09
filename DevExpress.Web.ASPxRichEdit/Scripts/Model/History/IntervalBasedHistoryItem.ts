module __aspxRichEdit {
    export class IntervalBasedHistoryItem extends HistoryItem {
        interval: FixedInterval;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval) {
            super(modelManipulator, boundSubDocument);
            this.interval = interval;
        }
    }
} 