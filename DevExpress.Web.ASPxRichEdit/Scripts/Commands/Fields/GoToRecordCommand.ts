module __aspxRichEdit {
    export class GoToRecordCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields) && this.control.mailMergeOptions.isEnabled &&
                this.control.mailMergeOptions.viewMergedData && this.canNavigate();
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            if(!this.canNavigate())
                return false;
            this.control.mailMergeOptions.activeRecordIndex = this.calculateNextRecordIndex(this.control.mailMergeOptions.activeRecordIndex);
            this.control.commandManager.getCommand(RichEditClientCommand.UpdateAllFields).execute();
            return true;
        }

        protected canNavigate(): boolean {
            throw new Error(Errors.NotImplemented);
        }

        protected calculateNextRecordIndex(recordIndex: number): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class GoToFirstDataRecordCommand extends GoToRecordCommandBase {
        protected canNavigate(): boolean {
            return this.control.mailMergeOptions.activeRecordIndex > 0;
        }

        protected calculateNextRecordIndex(recordIndex: number): number {
            return 0;
        }
    }

    export class GoToPreviousDataRecordCommand extends GoToRecordCommandBase {
        protected canNavigate(): boolean {
            return this.control.mailMergeOptions.activeRecordIndex > 0;
        }

        protected calculateNextRecordIndex(recordIndex: number): number {
            return recordIndex - 1;
        }
    }

    export class GoToNextDataRecordCommand extends GoToRecordCommandBase {
        protected canNavigate(): boolean {
            return this.control.mailMergeOptions.activeRecordIndex < this.control.mailMergeOptions.recordCount - 1;
        }

        protected calculateNextRecordIndex(recordIndex: number): number {
            return recordIndex + 1;
        }
    }

    export class GoToLastDataRecordCommand extends GoToRecordCommandBase {
        protected canNavigate(): boolean {
            return this.control.mailMergeOptions.activeRecordIndex < this.control.mailMergeOptions.recordCount - 1;
        }

        protected calculateNextRecordIndex(recordIndex: number): number {
            return this.control.mailMergeOptions.recordCount - 1;
        }
    }
}