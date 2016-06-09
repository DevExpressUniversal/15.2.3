module __aspxRichEdit {
    export class ChangeFieldHyperlinkInfoHistoryItem extends HistoryItem {
        oldInfo: HyperlinkInfo;
        fieldIndex: number;
        newInfo: HyperlinkInfo;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, fieldIndex: number, newInfo: HyperlinkInfo) {
            super(modelManipulator, boundSubDocument);
            this.newInfo = newInfo;
            this.fieldIndex = fieldIndex;
        }

        public redo() {
            this.oldInfo = this.modelManipulator.fieldsManipulator.setHyperlinkInfo(this.boundSubDocument, this.fieldIndex, this.newInfo);
        }

        public undo() {
            this.modelManipulator.fieldsManipulator.setHyperlinkInfo(this.boundSubDocument, this.fieldIndex, this.oldInfo);
        }
    }
} 