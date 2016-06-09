module __aspxRichEdit {
    export class DialogSplitTableCellsCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = TableCellUtils.areCellsSelectedInSeries(this.control.selection.getSelectedCells());
            return state;
        }

        createParameters(): SplitTableCellsDialogParameters {
            return this.getSplitTableCellsParameters();
        }

        applyParameters(state: IntervalCommandState, params: SplitTableCellsDialogParameters) {
            this.control.commandManager.getCommand(RichEditClientCommand.SplitTableCellsCommand).execute(params);
        }

        getDialogName() {
            return "SplitTableCells";
        }

        getSplitTableCellsParameters(): SplitTableCellsDialogParameters {
            let selectedCells: TableCell[][] = this.control.selection.getSelectedCells();
            let isSelectedCellsSquare = TableCellUtils.isSelectedSquare(selectedCells);
            let columnsCount = TableCellUtils.getColumnCountByCellsSeries(selectedCells[0]);
            let isMergeCellsBeforeSplit = isSelectedCellsSquare && (selectedCells.length > 1 || selectedCells[0].length > 0);
            let rowCountAfterMerge = -1;
            if(isMergeCellsBeforeSplit)
                rowCountAfterMerge = this.calculateRowCountAfterMerge(selectedCells);
            else
                rowCountAfterMerge = this.getActualRowsCount(selectedCells, isSelectedCellsSquare);
            let parameters = new SplitTableCellsDialogParameters();
            parameters.columnCount = columnsCount;
            parameters.rowCount = selectedCells.length;
            parameters.isMergeBeforeSplit = isMergeCellsBeforeSplit;
            return parameters;
        }
        calculateRowCountAfterMerge(selectedCells: TableCell[][]): number {
            let count = selectedCells.length;
            let result = 0;
            for(let i = 0, rowCells: TableCell[]; rowCells = selectedCells[i]; i++) {
                if(!this.shouldIgnoreRow(rowCells))
                    result++;
            }
            return Math.max(1, result);
        }
        getActualRowsCount(selectedCells: TableCell[][], isSelectedCellsSquare: boolean): number {
            if(isSelectedCellsSquare) {
                let table = selectedCells[0][0].parentRow.parentTable;
                let columnIndex = TableCellUtils.getStartColumnIndex(selectedCells[0][0]);
                let selectedCellsTable = TableCellUtils.getSelectedCellsTable(selectedCells);
                let startRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[0][0].startParagraphPosition.value);
                return TableCellUtils.getSelectedRowsCount(table, startRowIndex, startRowIndex + selectedCells.length - 1, columnIndex, selectedCellsTable);
            }
            return 0;
        }
        shouldIgnoreRow(selectedCellsInRow: TableCell[]) {
            let row = selectedCellsInRow[0].parentRow;
            let startCellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - selectedCellsInRow[0].startParagraphPosition.value);
            let endCellIndex = startCellIndex + selectedCellsInRow.length - 1;
            for(let i = 0, cell: TableCell; cell = row[i]; i++) {
                if(i >= startCellIndex && i <= endCellIndex)
                    continue;
                if(cell.verticalMerging !== TableCellMergingState.Continue)
                    return false;
            }
            return true;
        }
    }

    export class SplitTableCellsDialogParameters extends DialogParametersBase {
        rowCount: number;
        columnCount: number;
        isMergeBeforeSplit: boolean;

        copyFrom(obj: SplitTableCellsDialogParameters) {
            this.rowCount = obj.rowCount;
            this.columnCount = obj.columnCount;
            this.isMergeBeforeSplit = obj.isMergeBeforeSplit;
        }

        getNewInstance(): DialogParametersBase {
            return new SplitTableCellsDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}