module __aspxRichEdit {
    export class NumberingListManipulator {
        manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        addAbstractNumberingList(abstractNumberingList: AbstractNumberingList): number {
            var newIndex = this.manipulator.model.abstractNumberingLists.push(abstractNumberingList) - 1;
            abstractNumberingList.deleted = false;
            this.manipulator.dispatcher.notifyAbstractNumberingListAdded(newIndex);
            return newIndex;
        }

        deleteAbstractNumberingList(abstractNumberingListIndex: number) {
            this.manipulator.model.abstractNumberingLists[abstractNumberingListIndex].deleted = true;
            this.manipulator.model.abstractNumberingLists.splice(abstractNumberingListIndex, 1);
            this.manipulator.dispatcher.notifyAbstractNumberingListDeleted(abstractNumberingListIndex);
        }

        addNumberingList(numberingList: NumberingList): number {
            var newIndex = this.manipulator.model.numberingLists.push(numberingList) - 1;
            numberingList.deleted = false;
            this.manipulator.dispatcher.notifyNumberingListAdded(newIndex);
            return newIndex;
        }

        deleteNumberingList(numberingListIndex: number) {
            this.manipulator.model.numberingLists.splice(numberingListIndex, 1);
            this.manipulator.dispatcher.notifyNumberingListDeleted(numberingListIndex);
        }

        setIOverrideListLevelOverrideStart(numberingListIndex: number, levelIndex: number, overrideStart: boolean): HistoryItemState<HistoryItemListLevelStateObject> {
            var numberingList = this.manipulator.model.numberingLists[numberingListIndex];
            var listLevel = numberingList.levels[levelIndex];
            var newState = new HistoryItemState<HistoryItemListLevelStateObject>();
            var oldState = new HistoryItemState<HistoryItemListLevelStateObject>();
            oldState.register(new HistoryItemListLevelStateObject(false, numberingListIndex, levelIndex, listLevel.overrideStart));
            listLevel.overrideStart = overrideStart;
            newState.register(new HistoryItemListLevelStateObject(false, numberingListIndex, levelIndex, overrideStart));
            this.manipulator.dispatcher.notifyIOverrideListLevelChanged(JSONIOverrideListLevelProperty.OverrideStart, newState);
            return oldState;
        }
        restoreIOverrideListLevelOverrideStart(state: HistoryItemState<HistoryItemListLevelStateObject>) {
            var stateObject = state.objects[0];
            var numberingList = this.manipulator.model.numberingLists[stateObject.numberingListIndex];
            var listLevel = numberingList.levels[stateObject.listLevelIndex];
            listLevel.overrideStart = stateObject.value;
            this.manipulator.dispatcher.notifyIOverrideListLevelChanged(JSONIOverrideListLevelProperty.OverrideStart, stateObject.value);
        }

        setIOverrideListLevelNewStart(numberingListIndex: number, levelIndex: number, newStart: number): HistoryItemState<HistoryItemListLevelStateObject> {
            var numberingList = this.manipulator.model.numberingLists[numberingListIndex];
            var listLevel = numberingList.levels[levelIndex];

            var newState = new HistoryItemState<HistoryItemListLevelStateObject>();
            var oldState = new HistoryItemState<HistoryItemListLevelStateObject>();
            oldState.register(new HistoryItemListLevelStateObject(false, numberingListIndex, levelIndex, listLevel.getNewStart()));
            listLevel.setNewStart(newStart);
            newState.register(new HistoryItemListLevelStateObject(false, numberingListIndex, levelIndex, newStart));
            this.manipulator.dispatcher.notifyIOverrideListLevelChanged(JSONIOverrideListLevelProperty.NewStart, newState);
            return oldState;
        }

        restoreIOverrideListLevelNewStart(state: HistoryItemState<HistoryItemListLevelStateObject>) {
            var stateObject = state.objects[0];
            var numberingList = this.manipulator.model.numberingLists[stateObject.numberingListIndex];
            var listLevel = numberingList.levels[stateObject.listLevelIndex];
            listLevel.setNewStart(stateObject.value);
            this.manipulator.dispatcher.notifyIOverrideListLevelChanged(JSONIOverrideListLevelProperty.NewStart, stateObject.value);
        }

        // apply to paragraph

        setParagraphNumberingList(subDocument: SubDocument, paragraphIndex: number, numberingIndex: number, listLevelIndex: number): HistoryItemIntervalState<HistoryItemIntervalStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var paragraph = subDocument.paragraphs[paragraphIndex];
            var oldAbstractNumberingListIndex = paragraph.getAbstractNumberingListIndex();
            oldState.register(new HistoryItemIntervalStateObject(paragraph.getInterval(), [paragraph.numberingListIndex, paragraph.listLevelIndex]));
            newState.register(new HistoryItemIntervalStateObject(paragraph.getInterval(), [numberingIndex, listLevelIndex]));
            paragraph.numberingListIndex = numberingIndex;
            paragraph.listLevelIndex = listLevelIndex;
            paragraph.resetParagraphMergedProperties();
            this.manipulator.dispatcher.notifyParagraphNumberingListChanged(subDocument, newState, oldAbstractNumberingListIndex);
            return oldState;
        }

        removeNumberingListFromParagraph(subDocument: SubDocument, paragraphIndex: number): HistoryItemIntervalState<HistoryItemIntervalStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var paragraph = subDocument.paragraphs[paragraphIndex];
            var oldAbstractNumberingListIndex = paragraph.getAbstractNumberingListIndex();
            var newListIndex = NumberingList.NumberingListNotSettedIndex;
            if(paragraph.isInStyleList())
                newListIndex = NumberingList.NoNumberingListIndex;
            oldState.register(new HistoryItemIntervalStateObject(paragraph.getInterval(), [paragraph.numberingListIndex, paragraph.listLevelIndex]));
            paragraph.numberingListIndex = newListIndex;
            paragraph.listLevelIndex = -1;
            paragraph.resetParagraphMergedProperties();
            newState.register(new HistoryItemIntervalStateObject(paragraph.getInterval(), [newListIndex, -1]));
            this.manipulator.dispatcher.notifyParagraphNumberingListChanged(subDocument, newState, oldAbstractNumberingListIndex);
            return oldState;
        }

        restoreParagraphNumberingList(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            var paragraph = subDocument.getParagraphsByInterval(state.interval())[0];
            var oldAbstractNumberingListIndex = paragraph.getAbstractNumberingListIndex();
            paragraph.numberingListIndex = state.objects[0].value[0];
            paragraph.listLevelIndex = state.objects[0].value[1];
            paragraph.resetParagraphMergedProperties();
            this.manipulator.dispatcher.notifyParagraphNumberingListChanged(subDocument, state, oldAbstractNumberingListIndex);
        }
    }
} 