module __aspxRichEdit {
    export class FieldsManipulator {
        manipulator: ModelManipulator;

        constructor(dispatcher: ModelManipulator) {
            this.manipulator = dispatcher;
        }

        setHyperlinkInfo(subDocument: SubDocument, fieldIndex: number, newHyperlinkInfo: HyperlinkInfo): HyperlinkInfo {
            var field: Field = subDocument.fields[fieldIndex];
            var oldValue: HyperlinkInfo = field.getHyperlinkInfo();
            field.setNewHyperlinkInfo(newHyperlinkInfo);
            this.manipulator.dispatcher.notifyHyperlinkInfoChanged(subDocument, field.getResultInterval(), newHyperlinkInfo);
            return oldValue;
        }

        continueUpdateFields(control: IRichEditControl, responce: any) {
            var subDocument: SubDocument = control.model.subDocuments[responce["subDocID"]];
            if (subDocument && subDocument.fieldsWaitingForUpdate)
                subDocument.fieldsWaitingForUpdate.update(responce["info"]);
        }
    }
}  