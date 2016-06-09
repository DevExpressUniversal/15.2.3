module __aspxRichEdit {
    export class DialogInsertTableCellsCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }
        createParameters(): TableCellsDialogParameters {
            var parameters = new TableCellsDialogParameters();
            parameters.tableCellOperation = TableCellOperation.ShiftToTheVertically;
            return parameters;
        }
        applyParameters(state: IntervalCommandState, params: TableCellsDialogParameters) {
            var command: RichEditClientCommand = RichEditClientCommand.None;
            switch(params.tableCellOperation) {
                case TableCellOperation.ShiftToTheHorizontally:
                    command = RichEditClientCommand.InsertTableCellWithShiftToTheLeft;
                    break;
                case TableCellOperation.ShiftToTheVertically:
                    command = RichEditClientCommand.InsertTableCellsWithShiftToTheVertically;
                    break;
                case TableCellOperation.RowOperation:
                    command = RichEditClientCommand.InsertTableRowAbove;
                    break;
                case TableCellOperation.ColumnOperation:
                    command = RichEditClientCommand.InsertTableColumnToTheLeft;
                    break;
            }
            return this.control.commandManager.getCommand(command).execute();
        }
        getDialogName() {
            return "InsertTableCells";
        }
    }

    export class DialogDeleteTableCellsCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }
        createParameters(): TableCellsDialogParameters {
            var parameters = new TableCellsDialogParameters();
            parameters.tableCellOperation = TableCellOperation.ShiftToTheHorizontally;
            return parameters;
        }
        applyParameters(state: IntervalCommandState, params: TableCellsDialogParameters) {
            var command: RichEditClientCommand = RichEditClientCommand.None;
            switch(params.tableCellOperation) {
                case TableCellOperation.ShiftToTheHorizontally:
                    command = RichEditClientCommand.DeleteTableCellsWithShiftToTheHorizontally;
                    break;
                case TableCellOperation.ShiftToTheVertically:
                    command = RichEditClientCommand.DeleteTableCellsWithShiftToTheVertically;
                    break;
                case TableCellOperation.RowOperation:
                    command = RichEditClientCommand.DeleteTableRows;
                    break;
                case TableCellOperation.ColumnOperation:
                    command = RichEditClientCommand.DeleteTableColumns;
                    break;
            }
            return this.control.commandManager.getCommand(command).execute();
        }
        getDialogName() {
            return "DeleteTableCells";
        }
    }


    export class TableCellsDialogParameters extends DialogParametersBase {
        tableCellOperation: TableCellOperation;

        copyFrom(obj: TableCellsDialogParameters) {
            this.tableCellOperation = obj.tableCellOperation;
        }

        getNewInstance(): DialogParametersBase {
            return new TableCellsDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }

    export enum TableCellOperation {
        ShiftToTheHorizontally = 0,
        ShiftToTheVertically = 1,
        RowOperation = 2,
        ColumnOperation = 3
    }
}