module __aspxRichEdit {
    export class ContextItemHeadersFooters extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            state.visible = this.control.model.activeSubDocument.isHeaderFooter();
            return state;
        }
    }
}