module __aspxRichEdit {
    export class InsertPageBreakCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        // parameter must be null or undefined
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            var selection: Selection = this.control.selection;
            this.control.history.beginTransaction();
            if(this.control.options.pageBreakInsertMode === PageBreakInsertMode.NewLine)
                ModelManipulator.insertParagraph(this.control, this.control.model.activeSubDocument, selection.getLastSelectedInterval().clone());
            this.control.modelManipulator.insertText(this.control, selection.getLastSelectedInterval().clone(), this.control.model.specChars.PageBreak, false);
            ModelManipulator.insertParagraph(this.control, this.control.model.activeSubDocument, selection.getLastSelectedInterval().clone());
            this.control.history.endTransaction();

            return true;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphs);
        }
    }
}   