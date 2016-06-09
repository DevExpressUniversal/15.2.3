module __aspxRichEdit {
    export class InsertTextHistoryItemBase extends HistoryItem {
        position: number;
        text: string;
        maskedCharacterProperties: MaskedCharacterProperties;
        characterStyle: CharacterStyle;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: number, text: string, maskedCharacterProperties: MaskedCharacterProperties,
            characterStyle: CharacterStyle) {
            super(modelManipulator, boundSubDocument);
            this.position = position;
            this.text = text;
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.characterStyle = characterStyle;
        } 

        public redo() {
            this.modelManipulator.text.insertText(this.boundSubDocument, this.position, this.text, this.maskedCharacterProperties, this.characterStyle, this.getTextRunType());
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.position, this.text.length), false);
        }

        public getTextRunType(): TextRunType {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class InsertTextHistoryItem extends InsertTextHistoryItemBase {
        public getTextRunType(): TextRunType {
            return TextRunType.TextRun;
        }
    }

    export class InsertLayoutDependentTextItem extends InsertTextHistoryItemBase {
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: number, maskedCharacterProperties: MaskedCharacterProperties,
            characterStyle: CharacterStyle) {
            super(modelManipulator, boundSubDocument, position, Utils.specialCharacters.LayoutDependentText, maskedCharacterProperties, characterStyle);
        }

        public getTextRunType(): TextRunType {
            return TextRunType.LayoutDependentTextRun;
        }
    }
} 