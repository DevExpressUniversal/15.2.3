module __aspxRichEdit {
    export class ChangeTableLookCommandBase extends CommandBase<SimpleCommandState> {
        option: TableLookTypes;

        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            let selectedCells = this.control.selection.getSelectedCells();
            state.enabled = this.isEnabled() && selectedCells.length > 0;
            if(state.enabled) {
                let table = selectedCells[0][0].parentRow.parentTable;
                state.value = !!(table.lookTypes & this.option);
            }
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        executeCore(state: SimpleCommandState, parameter: boolean): boolean {
            let table = this.control.selection.getSelectedCells()[0][0].parentRow.parentTable;
            this.control.history.beginTransaction();
            let value = table.lookTypes;
            if(parameter)
                value |= this.option;
            else
                value &= ~this.option;
            this.control.history.addAndRedo(new TableLookTypesHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, table.index, value));
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.endTransaction();
            return true;
        }
    }

    export class ToggleFirstRowCommand extends ChangeTableLookCommandBase {
        option: TableLookTypes = TableLookTypes.ApplyFirstRow;
    }

    export class ToggleLastRowCommand extends ChangeTableLookCommandBase {
        option: TableLookTypes = TableLookTypes.ApplyLastRow;
    }

    export class ToggleFirstColumnCommand extends ChangeTableLookCommandBase {
        option: TableLookTypes = TableLookTypes.ApplyFirstColumn;
    }

    export class ToggleLastColumnCommand extends ChangeTableLookCommandBase {
        option: TableLookTypes = TableLookTypes.ApplyLastColumn;
    }

    export class ToggleBandedRowsCommand extends ChangeTableLookCommandBase {
        option: TableLookTypes = TableLookTypes.DoNotApplyRowBanding;
    }

    export class ToggleBandedColumnCommand extends ChangeTableLookCommandBase {
        option: TableLookTypes = TableLookTypes.DoNotApplyColumnBanding;
    }
}