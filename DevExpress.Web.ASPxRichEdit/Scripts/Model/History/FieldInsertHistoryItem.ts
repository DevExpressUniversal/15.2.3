module __aspxRichEdit {
    export class FieldInsertHistoryItem extends HistoryItem {
        control: IRichEditControl;
        startFieldPos: number;
        separatorPos: number;
        endPos: number;
        showCode: boolean;

        constructor(control: IRichEditControl, modelManipulator: ModelManipulator, boundSubDocument: SubDocument, startCodePos: number, codePartLength: number, resultPartLength: number, showCode: boolean) {
            super(modelManipulator, boundSubDocument);
            this.control = control;
            this.startFieldPos = startCodePos;
            this.separatorPos = this.startFieldPos + 1 + codePartLength;
            this.endPos = this.separatorPos + 1 + resultPartLength + 1;
            this.showCode = showCode;
        }

        public redo() {
            var specChars: SpecialCharacters = this.control.model.specChars;
            var textManipulator: TextManipulator = this.modelManipulator.text;

            var characterStyle: CharacterStyle = this.control.inputPosition.getCharacterStyle();
            var charProp: MaskedCharacterProperties = this.control.inputPosition.getMaskedCharacterProperties().clone();

            textManipulator.insertText(this.boundSubDocument, this.startFieldPos, specChars.FieldCodeStartRun, charProp, characterStyle, TextRunType.FieldCodeStartRun);
            textManipulator.insertText(this.boundSubDocument, this.separatorPos, specChars.FieldCodeEndRun, charProp, characterStyle, TextRunType.FieldCodeEndRun);
            textManipulator.insertText(this.boundSubDocument, this.endPos - 1, specChars.FieldResultEndRun, charProp, characterStyle, TextRunType.FieldResultEndRun);

            var fields: Field[] = this.boundSubDocument.fields;
            var fieldIndex: number = Field.normedBinaryIndexOf(fields, this.startFieldPos + 1);
            var newFieldIndex: number = fieldIndex + 1;

            var newField: Field = new Field(this.boundSubDocument.positionManager, newFieldIndex, this.startFieldPos, this.separatorPos, this.endPos, this.showCode, undefined);
            Field.addField(fields, newField);

            this.modelManipulator.dispatcher.notifyFieldInserted(this.boundSubDocument, this.startFieldPos, this.separatorPos, this.endPos);

            var selection: Selection = this.control.selection;
            selection.setSelection(this.startFieldPos + 1, this.startFieldPos + 1, true, -1, UpdateInputPositionProperties.No, false);
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.startFieldPos, 1), false);// here will deleted info about subDocument.fields and notify field deleted
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.endPos - 2, 1), false);
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, new FixedInterval(this.separatorPos - 1, 1), false);

            var selection: Selection = this.control.selection;
            selection.setSelection(this.startFieldPos, this.separatorPos - 1, true, -1, UpdateInputPositionProperties.Yes, false);
        }
    }
} 