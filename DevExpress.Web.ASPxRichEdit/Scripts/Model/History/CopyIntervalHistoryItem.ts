module __aspxRichEdit {
    //export class CopyIntervalHistoryItem extends HistoryItem {
    //    toPosition: number;
    //    intervals: FixedInterval[];

    //    constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, fromIntervals: FixedInterval[], toPosition: number) {
    //        super(modelManipulator, boundSubDocument);
    //        this.intervals = fromIntervals;
    //        this.toPosition = toPosition;
    //    }

    //    public redo() {
    //        var info = this.modelManipulator.text.createModelPartCopy(this.boundSubDocument, this.intervals);
    //        this.modelManipulator.text.insertPartSubDocumentInOtherSubDocument(info.model.activeSubDocument, this.boundSubDocument,
    //            new FixedInterval(0, info.model.activeSubDocument.getDocumentEndPosition() - (info.addedUselessParagraphMarkInEnd ? 1 : 0)), 
    //            this.toPosition);
    //    }

    //    public undo() {
    //        this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.toPosition, this.intervals[this.intervals.length - 1].end() - this.intervals[0].start), true);
    //    }
    //}
} 