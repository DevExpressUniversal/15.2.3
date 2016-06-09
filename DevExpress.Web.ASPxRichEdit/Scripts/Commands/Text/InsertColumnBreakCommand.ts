module __aspxRichEdit {
    export class InsertColumnBreakCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.sections);
        }

        // parameter must be null or undefined
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.modelManipulator.insertText(this.control, this.control.selection.getLastSelectedInterval().clone(), this.control.model.specChars.ColumnBreak, false);
            return true;
        }
    }
}     