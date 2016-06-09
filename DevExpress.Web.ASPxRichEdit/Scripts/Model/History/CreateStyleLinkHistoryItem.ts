module __aspxRichEdit {
    export class CreateStyleLinkHistoryItem extends HistoryItem {
        private characterStyle: CharacterStyle;
        private paragraphStyle: ParagraphStyle

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, characterStyle: CharacterStyle, paragraphStyle: ParagraphStyle) {
            super(modelManipulator, boundSubDocument);
            this.characterStyle = characterStyle;
            this.paragraphStyle = paragraphStyle;
        }

        public redo() {
            this.modelManipulator.styles.setLinkStyle(this.characterStyle, this.paragraphStyle);
        }

        public undo() {
            this.modelManipulator.styles.restoreLinkStyle(this.characterStyle, this.paragraphStyle);
        }
    }
}
