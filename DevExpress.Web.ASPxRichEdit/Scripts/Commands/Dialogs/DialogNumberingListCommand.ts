module __aspxRichEdit {
    export class DialogNumberingListCommand extends ShowDialogCommandBase {
        isEnabled(): boolean {
            return super.isEnabled() && (ControlOptions.isEnabled(this.control.options.numberingBulleted) || ControlOptions.isEnabled(this.control.options.numberingMultiLevel) || ControlOptions.isEnabled(this.control.options.numberingSimple));
        }
        createParameters(abstractNumberingList: AbstractNumberingList): DialogNumberingListParameters {
            var parameters: DialogNumberingListParameters = new DialogNumberingListParameters();
            if(abstractNumberingList)
                parameters.selectedAbstractNumberingList = abstractNumberingList;
            else if(!this.areThereParagraphsInDifferentLists())
                    parameters.selectedAbstractNumberingList = this.getSelectedAbstractNumberingList();
            return parameters;
        }
        areThereParagraphsInDifferentLists(): boolean {
            var prevNumbListIndex = -1;
            var paragraphsIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            var paragraphsIndicesLength = paragraphsIndices.length;
            for(let i = 0; i < paragraphsIndicesLength; i++) {
                var paragraphIndex = paragraphsIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                if(!paragraph.isInList())
                    continue;
                if(prevNumbListIndex == -1) {
                    prevNumbListIndex = paragraph.numberingListIndex;
                    continue;
                }
                if(prevNumbListIndex != paragraph.numberingListIndex)
                    return true;
            }
            return false;
        }
        getSelectedAbstractNumberingList(): AbstractNumberingList {
            var listIndex = this.getFirstNumberingListIndex();
            if(listIndex == -1)
                return null;
            var list: NumberingList = this.control.model.numberingLists[listIndex];
            return list.getAbstractNumberingList();
        }
        getFirstNumberingListIndex(): number {
            var paragraphsIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            let paragraphsIndicesLength = paragraphsIndices.length;
            for(let i = 0; i < paragraphsIndicesLength; i++) {
                let paragraphIndex = paragraphsIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                if(paragraph.isInList())
                    return paragraph.numberingListIndex;
            }
            return -1;
        }
        applyParameters(state: IntervalCommandState, params: DialogNumberingListParameters) {
            if(params.selectedAbstractNumberingList)
                this.control.commandManager.getCommand(RichEditClientCommand.InsertNumerationToParagraphs).execute(params.selectedAbstractNumberingList);
            else if(params.selectedAbstractNumberingList === null)
                this.control.commandManager.getCommand(RichEditClientCommand.DeleteNumerationFromParagraphs).execute();
        }
        getDialogName() {
            return "NumberingList";
        }
    }

    export class DialogNumberingListParameters extends DialogParametersBase {
        selectedAbstractNumberingList: AbstractNumberingList;

        getNewInstance(): DialogParametersBase {
            return new DialogNumberingListParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }

        copyFrom(obj: DialogNumberingListParameters) {
            this.selectedAbstractNumberingList = obj.selectedAbstractNumberingList;
        }
    }
}   