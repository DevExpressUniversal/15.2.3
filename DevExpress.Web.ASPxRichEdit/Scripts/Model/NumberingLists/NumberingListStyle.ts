module __aspxRichEdit {
    export class NumberingListStyle extends StyleBase {
        numberingListIndex: number;
        parent: NumberingListStyle;

        constructor(styleName: string, localizedName: string, deleted: boolean, hidden: boolean, semihidden: boolean, isDefault: boolean, numberingListIndex: number) {
            super(styleName, localizedName, deleted, hidden, semihidden, isDefault);
            this.numberingListIndex = numberingListIndex;
        }
    }
} 