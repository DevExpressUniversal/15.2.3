module __aspxRichEdit {
    export class ToggleFullScreenCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.value = this.control.isFullScreenMode();
            return state;
        }
        executeCore(state: SimpleCommandState): boolean {
            this.control.toggleFullScreenMode();
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fullScreen);
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
}