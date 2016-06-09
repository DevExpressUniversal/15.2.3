module __aspxRichEdit {
    export class ContinueNumberingListCommand extends NumberingListCommandBase {
        getState(): IntervalCommandStateEx {
            var state = new IntervalCommandStateEx(this.isEnabled(), this.control.selection.getIntervalsClone());
            state.visible = false;
            if(state.enabled) {
                var startParagraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, p => p.startLogPosition.value - state.intervals[0].start);
                var paragraph = this.control.model.activeSubDocument.paragraphs[startParagraphIndex];
                if(paragraph.isInList() && this.getTargetNumberingListIndex(startParagraphIndex, paragraph.getAbstractNumberingListIndex()) >= 0)
                    state.visible = true;
                state.enabled = state.visible;
            }
            return state;
        }

        executeCore(state: IntervalCommandStateEx, parameter: any): boolean {
            var startParagraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, p => p.startLogPosition.value - state.intervals[0].start);
            var startParagraphAbstractNumberingListIndex = this.control.model.activeSubDocument.paragraphs[startParagraphIndex].getAbstractNumberingListIndex();
            var targetNumberingListIndex = this.getTargetNumberingListIndex(startParagraphIndex, startParagraphAbstractNumberingListIndex);
            this.control.history.beginTransaction();
            for(var i = startParagraphIndex, paragraph: Paragraph; paragraph = this.control.model.activeSubDocument.paragraphs[i]; i++) {
                if(paragraph.getAbstractNumberingListIndex() === startParagraphAbstractNumberingListIndex)
                    this.control.history.addAndRedo(new AddParagraphToListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, i, targetNumberingListIndex, paragraph.getListLevelIndex()));
            }
            this.control.history.endTransaction();
            return true;
        }

        private getTargetNumberingListIndex(startParagraphIndex: number, currentAbstractNumberingListIndex: number): number {
            for(var i = startParagraphIndex - 1, prevParagraph: Paragraph; prevParagraph = this.control.model.activeSubDocument.paragraphs[i]; i--) {
                var prevParagraphAbstractNumberingListIndex = prevParagraph.getAbstractNumberingListIndex();
                if(prevParagraphAbstractNumberingListIndex === currentAbstractNumberingListIndex)
                    return -1;
                if(prevParagraphAbstractNumberingListIndex >= 0)
                    return prevParagraph.getNumberingListIndex();
            }
            return -1;
        }
    }
} 