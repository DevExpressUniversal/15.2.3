module __aspxRichEdit {
    export class InsertNumerationToParagraphsCommand extends NumberingListCommandBase {
        abstractNumberingList: AbstractNumberingList;

        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), this.control.selection.getIntervalsClone());
        }
        executeCore(state: IntervalCommandStateEx, abstractNumberingList: AbstractNumberingList): boolean {
            this.abstractNumberingList = abstractNumberingList;
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(state.intervals);
            this.control.history.beginTransaction();
            var targetListIndex = this.createNewList(this.abstractNumberingList);
            var paragraphsLayoutPositions = this.getParagraphsLayoutPositions(paragraphIndices);
            var paragraphsLevelIndices = this.getParagraphsLevelIndices(paragraphIndices, paragraphsLayoutPositions, false, targetListIndex, -1);
            this.insertNumberingListCore(paragraphIndices, targetListIndex, paragraphsLevelIndices, paragraphsLayoutPositions);
            this.control.history.endTransaction();

            return true;
        }
        getNumberingListType(): NumberingType {
            return this.abstractNumberingList.getListType();
        }
    }
}