module __aspxRichEdit {
    export class RedoCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled() && this.control.history.canRedo());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.undo);
        }
        executeCore(state: SimpleCommandState): boolean {
            if (state.enabled) {
                this.control.history.redo();
                return true;
            }
            return false;
        }
    }
}  