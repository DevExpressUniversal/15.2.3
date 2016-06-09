module __aspxRichEdit {
    export class SelectionCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
}