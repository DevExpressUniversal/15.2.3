module __aspxRichEdit {
    export class InsertParagraphHistoryItem extends HistoryItem {
        position: number;
        maskedCharacterProperties: MaskedCharacterProperties;
        characterStyle: CharacterStyle;
        maskedParagraphProperties: MaskedParagraphProperties;
        paragraphStyle: ParagraphStyle;
        numberingListIndex: number;
        listLevelIndex: number;
        tabs: TabProperties;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: number, maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle,
            maskedParagraphProperties: MaskedParagraphProperties, paragraphStyle: ParagraphStyle, numberingListIndex: number, listLevelIndex: number, tabs: TabProperties) {
            super(modelManipulator, boundSubDocument); 
            this.position = position;
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.characterStyle = characterStyle;
            this.maskedParagraphProperties = maskedParagraphProperties;
            this.paragraphStyle = paragraphStyle;
            this.numberingListIndex = numberingListIndex;
            this.listLevelIndex = listLevelIndex;
            this.tabs = tabs;
        }

        public redo() {
            this.modelManipulator.text.insertParagraph(this.boundSubDocument, this.position, this.maskedCharacterProperties, this.characterStyle,
                this.maskedParagraphProperties, this.paragraphStyle, true, this.numberingListIndex, this.listLevelIndex, this.tabs);
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.position, this.modelManipulator.model.specChars.ParagraphMark.length), true);
        }
    }
} 