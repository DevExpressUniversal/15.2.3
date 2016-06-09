module __aspxRichEdit {
    export class DialogInsertTableCommand extends ShowDialogCommandBase {
        createParameters(): InsertTableDialogParameters {
            var parameters = new InsertTableDialogParameters();
            parameters.rowCount = 2;
            parameters.columnCount = 5;
            return parameters;
        }

        applyParameters(state: SimpleCommandState, params: InsertTableDialogParameters) {
            if(params.rowCount > 0 && params.columnCount > 0)
                this.control.commandManager.getCommand(RichEditClientCommand.InsertTableCore).execute({ rowCount: params.rowCount, cellCount: params.columnCount });
        }

        getDialogName() {
            return "InsertTable";
        }
    }

    export class InsertTableDialogParameters extends DialogParametersBase {
        rowCount: number;
        columnCount: number;

        copyFrom(obj: InsertTableDialogParameters) {
            this.rowCount = obj.rowCount;
            this.columnCount = obj.columnCount;
        }

        getNewInstance(): DialogParametersBase {
            return new InsertTableDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}