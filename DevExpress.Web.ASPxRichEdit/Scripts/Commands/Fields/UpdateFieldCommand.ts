module __aspxRichEdit {
    export class UpdateFieldCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = FieldContextMenuHelper.showUpdateAndToogleCodeItems(this.control.model.activeSubDocument.fields, this.control.selection.intervals);
            return state;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields);
        }

        executeCore(state: SimpleCommandState, callbackFunc: any): boolean {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            if (subDocument.fieldsWaitingForUpdate)
                return false;
            Field.DEBUG_FIELDS_CHECKS(subDocument);
            (new FieldsWaitingForUpdate(this.control, callbackFunc)).update(null);
            return true;    
        }
    }
}