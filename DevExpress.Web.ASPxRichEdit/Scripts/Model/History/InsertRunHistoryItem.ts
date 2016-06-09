//module __aspxRichEdit {
//    export class InsertRunHistoryItem extends HistoryItem {
//        position: number;
//        text: string;
//        runType: TextRunType;
//        maskedCharacterProperties: MaskedCharacterProperties;
//        characterStyle: CharacterStyle;

//        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: number, text: string, runType: TextRunType, characterStyle: CharacterStyle,
//            maskedCharacterProperties: MaskedCharacterProperties) {
//            super(modelManipulator, boundSubDocument);
//            this.position = position;
//            this.text = text;
//            this.runType = runType;
//            this.maskedCharacterProperties = maskedCharacterProperties;
//            this.characterStyle = characterStyle;
//        }

//        public redo() {
//            this.modelManipulator.text.insertText(this.boundSubDocument, this.position, this.text, this.maskedCharacterProperties, this.characterStyle, this.runType);
//        }

//        public undo() {
//            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.position, this.text.length), false);
//        }
//    }
//} 