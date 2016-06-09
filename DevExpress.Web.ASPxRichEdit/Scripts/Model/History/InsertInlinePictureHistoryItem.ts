module __aspxRichEdit {
    export class InsertInlinePictureHistoryItem extends HistoryItem {
        position: number;
        id: number;
        originalWidth: number;
        originalHeight: number;
        scaleX: number;
        scaleY: number;
        maskedCharacterProperties: MaskedCharacterProperties;
        characterStyle: CharacterStyle;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: number, id: number, originalWidth: number, originalHeight: number, scaleX: number, scaleY: number,
                    maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle) {
            super(modelManipulator, boundSubDocument);
            this.position = position;
            this.id = id;
            this.originalWidth = originalWidth;
            this.originalHeight = originalHeight;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.characterStyle = characterStyle;
        }

        public redo() {
            this.modelManipulator.text.insertInlinePicture(this.boundSubDocument, this.position, this.id, this.originalWidth, this.originalHeight, this.scaleX, this.scaleY,
                false, this.maskedCharacterProperties, this.characterStyle);
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.position, 1), false);
        }
    }
} 