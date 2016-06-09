module __aspxRichEdit {
    export class InsertLineBreakCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        // parameter must be null or undefined
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.modelManipulator.insertText(this.control, this.control.selection.getLastSelectedInterval().clone(), this.control.model.specChars.LineBreak, false);
            return true;
        }
    }
}   