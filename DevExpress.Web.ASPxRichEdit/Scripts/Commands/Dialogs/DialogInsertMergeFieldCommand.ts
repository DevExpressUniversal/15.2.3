module __aspxRichEdit {
    export class DialogInsertMergeFieldCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields) && this.control.mailMergeOptions.isEnabled;
        }

        createParameters(): InsertMergeFieldDialogParameters {
            return new InsertMergeFieldDialogParameters();
        }

        applyParameters(state: IntervalCommandState, params: InsertMergeFieldDialogParameters) {
            if(params.fieldName != null)
                this.control.commandManager.getCommand(RichEditClientCommand.CreateMergeField).execute(params.fieldName);
        }

        getDialogName() {
            return "InsertMergeField";
        }

        isModal(): boolean {
            return false;
        }
    }

    export class InsertMergeFieldDialogParameters extends DialogParametersBase {
        fieldName: string;

        getNewInstance(): DialogParametersBase {
            return new InsertMergeFieldDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }

        copyFrom(obj: InsertMergeFieldDialogParameters) {
            this.fieldName = obj.fieldName;
        }
    }
}