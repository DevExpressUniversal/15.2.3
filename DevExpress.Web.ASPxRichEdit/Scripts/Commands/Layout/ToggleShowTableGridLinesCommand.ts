module __aspxRichEdit {
    export class ToggleShowTableGridLinesCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled(), this.control.model.showTableGridLines);
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.control.selection.getSelectedCells().length > 0;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.model.showTableGridLines = !this.control.model.showTableGridLines;
            this.control.formatterResetAllLayout();
            return true;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
}