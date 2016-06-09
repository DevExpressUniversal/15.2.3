module __aspxRichEdit {
    export class ChangeTableCellShadingCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.denyUpdateValue = true;
            return state;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.control.selection.getSelectedCells().length > 0;
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let color = this.getColor(parameter);
            let selectedCells = this.control.selection.getSelectedCells();
            let subDocument = this.control.model.activeSubDocument;
            let table = selectedCells[0][0].parentRow.parentTable;
            for(var i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - horCells[0].startParagraphPosition.value);
                for(var j = 0, cell: TableCell; cell = horCells[j]; j++) {
                    if(cell.properties.backgroundColor !== color || !cell.properties.getUseValue(TableCellPropertiesMask.UseBackgroundColor)) {
                        let cellIndex = Utils.normedBinaryIndexOf(table.rows[rowIndex].cells, c => c.startParagraphPosition.value - cell.startParagraphPosition.value);
                        this.control.history.addAndRedo(new TableCellBackgroundColorHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, color, true));
                    }
                }
            }
            this.control.history.endTransaction();
            return true;
        }

        getColor(value: string): number {
            return value == ColorHelper.NO_COLOR.toString() ? ColorHelper.NO_COLOR : ColorHelper.hashToColor(value);
        }
    }
}