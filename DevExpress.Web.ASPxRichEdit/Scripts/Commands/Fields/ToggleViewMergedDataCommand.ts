module __aspxRichEdit {
    export class ToggleViewMergedDataCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.value = this.control.mailMergeOptions.viewMergedData;
            return state;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields) && this.control.mailMergeOptions.isEnabled;
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            var mailMergeOptions: MailMergeOptions = this.control.mailMergeOptions;
            mailMergeOptions.viewMergedData = !mailMergeOptions.viewMergedData;
            this.control.commandManager.getCommand(RichEditClientCommand.UpdateAllFields).execute();
            return true;
        }
    }
}