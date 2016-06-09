module __aspxRichEdit {
    export class ToggleNumberingListCommand extends NumberingListCommandBase {
        getNumberingListType(): NumberingType {
            return NumberingType.Simple;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.numberingSimple);
        }

        getParagraphsLevelIndices(paragraphIndices: number[], layoutPositions: LayoutPosition[], continueNumberingList: boolean, listIndex: number, listLevelIndex: number): number[]{
            if(listLevelIndex >= 0 || !this.equalLeftIndent(paragraphIndices, layoutPositions, listIndex))
                return super.getParagraphsLevelIndices(paragraphIndices, layoutPositions, continueNumberingList, listIndex, listLevelIndex);
            else {
                var result: number[] = [];
                for(var i = paragraphIndices.length - 1; i >= 0; i--)
                    result.push(0);
                if(!continueNumberingList)
                    this.assignLevelsIndents(paragraphIndices[0], listIndex);
                return result;
            }
        }

        equalLeftIndent(paragraphIndices: number[], layoutPositions: LayoutPosition[], listIndex: number): boolean {
            if(paragraphIndices.length === 1)
                return true;
            var minLeftIndent = Number.MAX_VALUE;
            var maxLeftIndent = Number.MIN_VALUE;

            let paragraphIndicesLength = paragraphIndices.length;
            for(let i = 0; i < paragraphIndicesLength; i++) {
                var layoutPosition = layoutPositions[i];
                var startBox = layoutPosition.row.numberingListBox ? layoutPosition.row.numberingListBox : this.getStartBox(layoutPosition.row.boxes);
                var boxLeft = layoutPosition.row.x + startBox.x;
                minLeftIndent = Math.min(boxLeft, minLeftIndent);
                maxLeftIndent = Math.max(boxLeft, maxLeftIndent);
            }
            var numberingList = this.control.model.numberingLists[listIndex];
            var leftIndent = numberingList.levels[1].getParagraphMergedProperies().leftIndent - numberingList.levels[0].getParagraphMergedProperies().leftIndent;
            return maxLeftIndent - minLeftIndent < leftIndent;
        }
        assignLevelsIndents(paragraphIndex: number, listIndex: number) {
            var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
            var numberingList = this.control.model.numberingLists[listIndex];
            var firstLevelLeftIndent = numberingList.levels[0].getParagraphMergedProperies().leftIndent;
            if(paragraph.getParagraphMergedProperies().leftIndent !== firstLevelLeftIndent) {
                this.assignLevelsIndentsCore(paragraphIndex, listIndex, numberingList.levels);
            }
        }
        assignLevelsIndentsCore(paragraphIndex: number, listIndex: number, listLevels: IListLevel[]) {
            var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
            for(var i = 0, listLevel: IListLevel; listLevel = listLevels[i]; i++) {
                var listLevelMergedParagraphProperties = listLevel.getParagraphMergedProperies();
                this.control.history.addAndRedo(new ListLevelOriginalLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, false, listIndex, i, listLevelMergedParagraphProperties.leftIndent));
                this.control.history.addAndRedo(new ListLevelParagraphLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, false, listIndex, i, listLevelMergedParagraphProperties.leftIndent + paragraph.getParagraphMergedProperies().leftIndent, true));
            }
        }
    }
}   