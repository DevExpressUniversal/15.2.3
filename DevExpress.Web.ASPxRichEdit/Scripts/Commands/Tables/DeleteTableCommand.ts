module __aspxRichEdit {
    export class DeleteTableCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.control.selection.getSelectedCells().length > 0;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let subDocument = this.control.model.activeSubDocument;
            let table = selectedCells[0][0].parentRow.parentTable;
            TablesManipulator.removeTableWithContent(this.control, subDocument, table);
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.Yes, false);
            this.control.history.endTransaction();
            return true;
        }
    }
}