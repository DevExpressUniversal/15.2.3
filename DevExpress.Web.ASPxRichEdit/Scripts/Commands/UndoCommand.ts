module __aspxRichEdit {
    export class UndoCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled() && this.control.history.canUndo());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.undo);
        }
        executeCore(state: SimpleCommandState): boolean {
            if (state.enabled) {
                this.control.history.undo();
                return true;
            }
            return false;
        }
    }
} 