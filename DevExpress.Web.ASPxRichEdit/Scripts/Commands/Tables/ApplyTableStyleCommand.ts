module __aspxRichEdit {
    export class ApplyTableStyleCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            let selectedCells = this.control.selection.getSelectedCells();
            state.enabled = this.isEnabled() && selectedCells.length > 0;
            let items = [];
            for(let i = 0, style: TableStyle; style = this.control.model.tableStyles[i]; i++) {
                if(!style.hidden && !style.semihidden && !style.deleted)
                    items.push({ value: style.styleName, text: style.localizedName });
            }
            state.items = items;
            if(state.enabled) {
                let table = selectedCells[0][0].parentRow.parentTable;
                state.value = table.style ? table.style.styleName : null;
            }
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && ControlOptions.isEnabled(this.control.options.tableStyle);
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let style = this.control.model.getTableStyleByName(parameter);
            let table = this.control.selection.getSelectedCells()[0][0].parentRow.parentTable;
            this.control.history.addAndRedo(new ApplyTableStyleHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, table.index, style));
            this.control.history.endTransaction();
            return true;
        }
    }
}