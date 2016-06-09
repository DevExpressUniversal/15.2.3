module __aspxRichEdit {
    export class ChangeNumberingIndentCommandBase extends ParagraphIndentCommandBase { // abstract
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState): boolean {
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            var firstParagraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[0]];
            if(!firstParagraph.isInList())
                return false;
            this.control.history.beginTransaction();
            if(this.hasPreviousParagraphsInList(paragraphIndices[0]))
                this.changeListLevelIndices(paragraphIndices);
            else
                this.changeListLevelIndents(paragraphIndices);
            this.control.history.endTransaction();
            return true;
        }

        changeListLevelIndices(paragraphIndices: number[]) {
            var paragraphIndicesLength = paragraphIndices.length;
            for(let i = 0; i < paragraphIndicesLength; i++) {
                var paragraphIndex = paragraphIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                var newListLevelIndex = this.getNewListLevelIndex(paragraph);
                if(newListLevelIndex !== paragraph.getListLevelIndex())
                    this.control.history.addAndRedo(new AddParagraphToListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphIndex, paragraph.numberingListIndex, newListLevelIndex));
            }
        }
        changeListLevelIndents(paragraphIndices: number[]) {
            var startParagraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[0]];
            var tabs = this.getTabs(paragraphIndices);
            var abstractNumberingListIndex = startParagraph.getAbstractNumberingListIndex();
            var abstractNumberingList = startParagraph.getAbstractNumberingList();
            var firstLevelProperties = abstractNumberingList.levels[0].getParagraphMergedProperies();
            var currentLeftIndent = this.getLeftIndentPosition(firstLevelProperties.leftIndent, firstLevelProperties.firstLineIndent, firstLevelProperties.firstLineIndentType);
            var nextListLevelIndent = this.getNextListLevelIndent(currentLeftIndent, tabs);
            this.assignNewIndent(abstractNumberingListIndex, nextListLevelIndent, tabs);
        }
        assignNewIndent(abstractNumberingListIndex: number, nextListLevelIndent: number, tabs: number[]) {
            var abstractNumberingList = this.control.model.abstractNumberingLists[abstractNumberingListIndex];
            var levels = abstractNumberingList.levels;
            var firstLevelProperties = abstractNumberingList.levels[0].getParagraphMergedProperies();
            var delta = this.calculateLeftIndentDelta(nextListLevelIndent, firstLevelProperties.leftIndent, firstLevelProperties.firstLineIndent, firstLevelProperties.firstLineIndentType);
            var levelCount = levels.length;
            for(var i = 0; i < levelCount; i++) {
                var level = levels[i];
                var levelProperties = level.getParagraphMergedProperies();
                var newLeftIndent = levelProperties.leftIndent + delta;
                if(newLeftIndent >= 0) {
                    if(levelProperties.firstLineIndentType == ParagraphFirstLineIndent.Hanging) {
                        var firstLineLeftIndent = newLeftIndent - levelProperties.firstLineIndent;
                        if(firstLineLeftIndent < 0)
                            newLeftIndent -= firstLineLeftIndent;
                    }
                    if(i == 0 && levelProperties.leftIndent == newLeftIndent)
                        break;
                    this.control.history.addAndRedo(new ListLevelParagraphLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, true, abstractNumberingListIndex, i, newLeftIndent, true));
                }
                else if(i == 0)
                    break;
            }
        }

        getNextListLevelIndent(currentLeftIndent: number, tabs: number[]): number {
            throw new Error(Errors.NotImplemented);
        }
        getNewListLevelIndex(paragraph: Paragraph): number {
            throw new Error(Errors.NotImplemented);
        }

        private calculateLeftIndentDelta(nextListLevelIndent: number, currentLeftIndent: number, firstLineIndent: number, firstLineIndentType: ParagraphFirstLineIndent): number {
            return nextListLevelIndent - this.getLeftIndentPosition(currentLeftIndent, firstLineIndent, firstLineIndentType);
        }
        private hasPreviousParagraphsInList(paragraphIndex: number): boolean {
            var abstractNumberingListIndex = this.control.model.activeSubDocument.paragraphs[paragraphIndex].getAbstractNumberingListIndex();
            for(var i = paragraphIndex - 1, prevParagraph: Paragraph; prevParagraph = this.control.model.activeSubDocument.paragraphs[i]; i--) {
                if(prevParagraph.getAbstractNumberingListIndex() === abstractNumberingListIndex)
                    return true;
            }
            return false;
        }
        private getLeftIndentPosition(currentLeftIndent: number, firstLineIndent: number, firstLineIndentType: ParagraphFirstLineIndent): number {
            return firstLineIndentType === ParagraphFirstLineIndent.Hanging ? (currentLeftIndent - firstLineIndent) : currentLeftIndent;
        }
    }

    export class IncrementNumberingIndentCommand extends ChangeNumberingIndentCommandBase {
        getNextListLevelIndent(currentLeftIndent: number, tabs: number[]): number {
            var nearestRightDefaultTab = this.getNearRightDefaultTab(currentLeftIndent);
            var nearestRightTab = this.getNearRightTab(currentLeftIndent, tabs);
            return (nearestRightDefaultTab < nearestRightTab || nearestRightTab == currentLeftIndent) ? nearestRightDefaultTab : nearestRightTab;
        }
        getNewListLevelIndex(paragraph: Paragraph): number {
            return Math.min(7, paragraph.getListLevelIndex() + 1);
        }
    }

    export class DecrementNumberingIndentCommand extends ChangeNumberingIndentCommandBase {
        getNextListLevelIndent(currentLeftIndent: number, tabs: number[]): number {
            var nearestLeftDefaultTab = this.getNearLeftDefaultTab(currentLeftIndent);
            var nearestLeftTab = this.getNearLeftTab(currentLeftIndent, tabs);
            return (nearestLeftDefaultTab > nearestLeftTab || nearestLeftTab == currentLeftIndent) ? nearestLeftDefaultTab : nearestLeftTab;
        }
        getNewListLevelIndex(paragraph: Paragraph): number {
            return Math.max(0, paragraph.getListLevelIndex() - 1);
        }
    }
}