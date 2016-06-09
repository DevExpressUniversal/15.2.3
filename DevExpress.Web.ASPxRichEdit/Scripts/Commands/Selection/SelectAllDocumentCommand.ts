module __aspxRichEdit {
    export class SelectAllDocumentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState): boolean {
            if(!this.control.layout.isFullyFormatted)
                this.control.formatSync();

            const selection: Selection = this.control.selection;
            const firstPos: number = selection.pageIndex < 0 ? this.control.layout.pages[0].contentIntervals[0].start : 0;
            const subDocument = this.control.model.activeSubDocument;
            const lastPos: number = (subDocument.isMain() ? this.control.layout.getLastValidPage() : this.control.layout.pages[selection.pageIndex]).getEndPosition(subDocument);
            selection.setSelection(firstPos, lastPos, true, -1, UpdateInputPositionProperties.Yes);
            this.control.captureFocus();
            return true;
        }

        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
}  