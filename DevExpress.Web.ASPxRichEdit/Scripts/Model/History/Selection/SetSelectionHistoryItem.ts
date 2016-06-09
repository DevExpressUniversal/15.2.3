module __aspxRichEdit {
    export class SetSelectionHistoryItem extends HistoryItem {
        public selection: Selection;
        public forwardDirection: boolean;
        
        public upd: UpdateInputPositionProperties;
        public endOfLine: boolean;
        public intervals: FixedInterval[];

        public oldEndOfLine: boolean;
        public oldIntervals: FixedInterval[];

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, intervals: FixedInterval[], selection: Selection, upd: UpdateInputPositionProperties, endOfLine: boolean) {
            super(modelManipulator, boundSubDocument);
            this.selection = selection;
            this.upd = upd;
            this.endOfLine = endOfLine;
            this.forwardDirection = selection.forwardDirection;
            this.oldIntervals = selection.getIntervalsClone();
            this.oldEndOfLine = selection.endOfLine;
            this.intervals = intervals;
        }

        public redo() {
            this.setSelection(this.intervals);
        }

        public undo() {
            this.setSelection(this.oldIntervals);
        }

        private setSelection(intervals: FixedInterval[]) {
            for(var i = 0, interval: FixedInterval; interval = intervals[i]; i++) {
                var start = this.forwardDirection ? interval.start : interval.end();
                var end = this.forwardDirection ? interval.end() : interval.start;
                if(i === 0)
                    this.selection.setSelection(start, end, this.endOfLine, -1, this.upd);
                else
                    this.selection.addSelection(start, end, this.endOfLine, -1);
            }
            this.upd = UpdateInputPositionProperties.Yes;
        }
    }
}  