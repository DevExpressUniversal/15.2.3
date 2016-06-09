module __aspxRichEdit {
    export class HyperlinkCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            Field.DEBUG_FIELDS_CHECKS(subDocument);

            var state = new SimpleCommandState(this.isEnabled());
            state.value = FieldContextMenuHelper.showHyperlinkItems(subDocument.fields, this.control.selection.getLastSelectedInterval());
            state.visible = state.value !== null;
            
            return state;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.hyperlinks);
        }
    }
} 