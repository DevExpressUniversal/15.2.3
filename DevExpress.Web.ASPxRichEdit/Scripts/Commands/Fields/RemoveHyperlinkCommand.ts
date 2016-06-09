module __aspxRichEdit {
    export class RemoveHyperlinkCommand extends HyperlinkCommandBase {
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            var history: IHistory = this.control.history;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var selection: Selection = this.control.selection;
            var field: Field = <Field>state.value;
            var fieldStartPos: number = field.getFieldStartPosition();
            var resultLength: number = field.getResultInterval().length;

            history.beginTransaction();
            history.addAndRedo(new SetSelectionHistoryItem(modelManipulator, subDocument, [new FixedInterval(fieldStartPos, 0)], selection, UpdateInputPositionProperties.No, false));
            history.addAndRedo(new RemoveHyperlinkHistoryItem(modelManipulator, field));
            history.addAndRedo(new SetSelectionHistoryItem(modelManipulator, subDocument, [new FixedInterval(fieldStartPos + resultLength, 0)], selection, UpdateInputPositionProperties.No, false));
            history.endTransaction();
            return true;
        }
    }
} 