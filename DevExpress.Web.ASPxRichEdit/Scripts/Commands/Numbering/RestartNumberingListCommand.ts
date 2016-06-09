module __aspxRichEdit {
    export class RestartNumberingListCommand extends NumberingListCommandBase {
        getState(): IntervalCommandStateEx {
            var state = new IntervalCommandStateEx(this.isEnabled(), this.control.selection.getIntervalsClone());
            state.visible = false;
            if(state.enabled) {
                var startParagraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, p => p.startLogPosition.value - state.intervals[0].start);
                var paragraph = this.control.model.activeSubDocument.paragraphs[startParagraphIndex];
                if(paragraph.isInList()) {
                    var abstractNumberingListIndex = paragraph.getAbstractNumberingListIndex();
                    for(var i = startParagraphIndex - 1, prevParagraph: Paragraph; prevParagraph = this.control.model.activeSubDocument.paragraphs[i]; i--) {
                        if(prevParagraph.getAbstractNumberingListIndex() === abstractNumberingListIndex && prevParagraph.listLevelIndex === paragraph.listLevelIndex) {
                            state.visible = true;
                            break;
                        }
                    }
                }
                state.enabled = state.visible;
            }
            return state;
        }

        executeCore(state: IntervalCommandStateEx, parameter: any): boolean {
            var startParagraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, paragraph => paragraph.startLogPosition.value - state.intervals[0].start);

            this.control.history.beginTransaction();
            var firstParagraph = this.control.model.activeSubDocument.paragraphs[startParagraphIndex];
            var listType = firstParagraph.getNumberingList().getListType();
            var templateIndex = this.getNumberingListTemplateIndex(listType);
            var newListIndex = this.createNewList(this.control.model.abstractNumberingListTemplates[templateIndex]);
            var startParagraphAbstractNumberingListIndex = firstParagraph.getAbstractNumberingListIndex();

            for(var i = startParagraphIndex, paragraph: Paragraph; paragraph = this.control.model.activeSubDocument.paragraphs[i]; i++) {
                if(paragraph.getAbstractNumberingListIndex() === startParagraphAbstractNumberingListIndex)
                    this.control.history.addAndRedo(new AddParagraphToListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, i, newListIndex, paragraph.getListLevelIndex()));
            }
            this.control.history.endTransaction();
            return true;
        }
    }
}   