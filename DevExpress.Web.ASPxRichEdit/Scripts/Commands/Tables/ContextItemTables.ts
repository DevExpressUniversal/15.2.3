module __aspxRichEdit {
    export class ContextItemTables extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            return false;
        }
    }
}