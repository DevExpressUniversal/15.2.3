module __aspxRichEdit {
    export class GoToDocumentStartCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            if(this.control.layout.validPageCount < 1)
                this.control.runFormatting(0);
            this.setSelection(this.control.layout.pages[0].contentIntervals[0].start);
            return true;
        }

        setSelection(position: number) {
            throw new Error(Errors.NotImplemented);
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
    
    export class GoToDocumentStartCommand extends GoToDocumentStartCommandBase {
        setSelection(position: number) {
            this.control.selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
        }
    }

    export class ExtendGoToDocumentStartCommand extends GoToDocumentStartCommandBase {
        setSelection(position: number) {
            this.control.selection.extendLastSelection(position, false, -1, UpdateInputPositionProperties.Yes);
        }
    }
}  