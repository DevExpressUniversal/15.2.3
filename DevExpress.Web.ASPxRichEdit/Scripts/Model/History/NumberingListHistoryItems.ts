module __aspxRichEdit {
    export class AddAbstractNumberingListHistoryItem extends HistoryItem {
        abstractNumberingList: AbstractNumberingList;
        abstractNumberingListIndex: number;
        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, abstractNumberingList: AbstractNumberingList) {
            super(modelManipulator, subDocument);
            this.abstractNumberingList = abstractNumberingList;
        }

        public redo() {
            this.abstractNumberingListIndex = this.modelManipulator.numberingListManipulator.addAbstractNumberingList(this.abstractNumberingList);
        }

        public undo() {
            this.modelManipulator.numberingListManipulator.deleteAbstractNumberingList(this.abstractNumberingListIndex);
        }
    }

    export class AddNumberingListHistoryItem extends HistoryItem {
        numberingList: NumberingList;
        numberingListIndex: number;
        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, numberingList: NumberingList) {
            super(modelManipulator, subDocument);
            this.numberingList = numberingList;
        }

        public redo() {
            this.numberingListIndex = this.modelManipulator.numberingListManipulator.addNumberingList(this.numberingList);
        }

        public undo() {
            this.modelManipulator.numberingListManipulator.deleteNumberingList(this.numberingListIndex);
        }
    }

    export class AddParagraphToListHistoryItem extends HistoryItem {
        paragraphIndex: number;
        numberingListIndex: number;
        listLevelIndex: number;
        state: HistoryItemIntervalState<HistoryItemIntervalStateObject>;

        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, paragraphIndex: number, numberingListIndex: number, listLevelIndex: number) {
            super(modelManipulator, subDocument);
            this.paragraphIndex = paragraphIndex;
            this.numberingListIndex = numberingListIndex;
            this.listLevelIndex = listLevelIndex;
        }

        public redo() {
            this.state = this.modelManipulator.numberingListManipulator.setParagraphNumberingList(this.boundSubDocument, this.paragraphIndex, this.numberingListIndex, this.listLevelIndex);
        }

        public undo() {
            this.modelManipulator.numberingListManipulator.restoreParagraphNumberingList(this.boundSubDocument, this.state);
        }
    }

    export class RemoveParagraphFromListHistoryItem extends HistoryItem {
        paragraphIndex: number;
        state: HistoryItemIntervalState<HistoryItemIntervalStateObject>;

        constructor(modelManipulator: ModelManipulator, subDocument: SubDocument, paragraphIndex: number) {
            super(modelManipulator, subDocument);
            this.paragraphIndex = paragraphIndex;
        }

        public redo() {
            this.state = this.modelManipulator.numberingListManipulator.removeNumberingListFromParagraph(this.boundSubDocument, this.paragraphIndex);         }

        public undo() {
            this.modelManipulator.numberingListManipulator.restoreParagraphNumberingList(this.boundSubDocument, this.state);
        }
    }

    export class ListLevelNewStartHistoryItem extends HistoryItem {
        oldState: HistoryItemState<HistoryItemListLevelStateObject>;
        newValue: number;
        listIndex: number;
        levelIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, listIndex: number, levelIndex: number, newValue: number) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
            this.listIndex = listIndex;
            this.levelIndex = levelIndex;
        }
        public redo() {
            this.oldState = this.modelManipulator.numberingListManipulator.setIOverrideListLevelNewStart(this.listIndex, this.levelIndex, this.newValue);
        }
        public undo() {
            this.modelManipulator.numberingListManipulator.restoreIOverrideListLevelNewStart(this.oldState);
        }
    }

    export class ListLevelOverrideStartHistoryItem extends HistoryItem {
        oldState: HistoryItemState<HistoryItemListLevelStateObject>;
        newValue: boolean;
        listIndex: number;
        levelIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, listIndex: number, levelIndex: number, overrideStart: boolean) {
            super(modelManipulator, boundSubDocument);
            this.newValue = overrideStart;
            this.listIndex = listIndex;
            this.levelIndex = levelIndex;
        }
        public redo() {
            this.oldState = this.modelManipulator.numberingListManipulator.setIOverrideListLevelOverrideStart(this.listIndex, this.levelIndex, this.newValue);
        }
        public undo() {
            this.modelManipulator.numberingListManipulator.restoreIOverrideListLevelOverrideStart(this.oldState);
        }
    }
}