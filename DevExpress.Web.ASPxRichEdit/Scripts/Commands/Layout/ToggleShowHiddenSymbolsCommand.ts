module __aspxRichEdit {
    export class ToggleShowHiddenSymbolsCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled(), this.control.model.showHiddenSymbols);
        }

        executeCore(state: SimpleCommandState): boolean {
            this.control.model.showHiddenSymbols = !this.control.model.showHiddenSymbols;
            this.control.formatterResetAllLayout();
            return true;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
} 