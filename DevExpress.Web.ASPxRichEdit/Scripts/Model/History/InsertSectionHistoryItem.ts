module __aspxRichEdit {
    export class InsertSectionHistoryItem extends HistoryItem {
        position: number;
        maskedCharacterProperties: MaskedCharacterProperties;
        characterStyle: CharacterStyle;
        sectionProperties: SectionProperties;
        isInsertPropertiesToCurrentSection: boolean;
        paragraphStyle: ParagraphStyle;
        paragraphMaskedProperties: MaskedParagraphProperties;
        isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean;
        numberingListIndex: number;
        listLevelIndex: number;
        tabs: TabProperties;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: number, maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle,
            sectionProperties: SectionProperties, isInsertPropertiesToCurrentSection,
            paragraphStyle: ParagraphStyle, paragraphMaskedProperties: MaskedParagraphProperties, isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean, numberingListIndex: number, listLevelIndex: number, tabs: TabProperties) {
            super(modelManipulator, boundSubDocument);
            this.position = position;
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.characterStyle = characterStyle;

            this.sectionProperties = sectionProperties;
            this.isInsertPropertiesToCurrentSection = isInsertPropertiesToCurrentSection; 

            this.paragraphStyle = paragraphStyle;
            this.paragraphMaskedProperties = paragraphMaskedProperties;
            this.isInsertPropertiesAndStyleIndexToCurrentParagraph = isInsertPropertiesAndStyleIndexToCurrentParagraph;
            this.tabs = tabs;
        }

        public redo() {
            this.modelManipulator.text.insertSection(this.boundSubDocument, this.position, this.maskedCharacterProperties, this.characterStyle, this.sectionProperties,
                this.isInsertPropertiesToCurrentSection, this.paragraphStyle, this.paragraphMaskedProperties, this.isInsertPropertiesAndStyleIndexToCurrentParagraph, this.numberingListIndex, this.listLevelIndex, this.tabs);
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.position, this.modelManipulator.model.specChars.SectionMark.length), false);
        }
    }
}