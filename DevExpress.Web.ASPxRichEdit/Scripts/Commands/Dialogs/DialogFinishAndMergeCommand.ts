module __aspxRichEdit {
    export class DialogFinishAndMergeCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields) && this.control.mailMergeOptions.isEnabled;
        }

        createParameters(): FinishAndMergeDialogParameters {
            var parameters = new FinishAndMergeDialogParameters();
            parameters.range = MailMergeExportRange.AllRecord;
            parameters.exportFrom = 1;
            parameters.exportRecordsCount = this.control.mailMergeOptions.recordCount;
            parameters.mergeMode = MergeMode.NewParagraph;
            return parameters;
        }

        getDialogName() {
            return "FinishAndMerge";
        }
    }

    export class FinishAndMergeDialogParameters extends DialogParametersBase {
        range: MailMergeExportRange;
        exportFrom: number;
        exportRecordsCount: number;
        mergeMode: MergeMode;

        getNewInstance(): DialogParametersBase {
            return new FinishAndMergeDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }

        copyFrom(obj: FinishAndMergeDialogParameters) {
            this.range = obj.range;
            this.exportFrom = obj.exportFrom;
            this.exportRecordsCount = obj.exportRecordsCount;
            this.mergeMode = obj.mergeMode;
        }
    }

    export enum MergeMode {
        NewParagraph = 0,
        NewSection = 1,
        JoinTables = 2
    }

    export enum MailMergeExportRange {
        AllRecord = 0,
        CurrentRecord = 1,
        Range = 2
    }
}