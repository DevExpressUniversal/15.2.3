module __aspxRichEdit {
    export class DeleteNumerationFromParagraphsCommand extends NumberingListCommandBase {
        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), this.control.selection.getIntervalsClone());
        }
        executeCore(state: IntervalCommandStateEx): boolean {
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(state.intervals);
            this.control.history.beginTransaction();
            this.deleteNumberingList(paragraphIndices);
            this.control.history.endTransaction();

            return true;
        }
        getNumberingListType(): NumberingType {
            return null;
        }
    }
}