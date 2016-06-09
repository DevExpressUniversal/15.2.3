module __aspxRichEdit {
    export class ToggleShowHorizontalRulerCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled(), this.control.horizontalRulerControl.getVisible());
        }
        executeCore(state: SimpleCommandState, parameter: boolean): boolean {
            this.control.horizontalRulerControl.setVisible(parameter);
            this.control.adjust();
            return true;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
} 