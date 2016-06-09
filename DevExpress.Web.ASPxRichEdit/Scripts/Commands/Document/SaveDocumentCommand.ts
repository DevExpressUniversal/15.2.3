module __aspxRichEdit {
    export class SaveDocumentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.save !== DocumentCapability.Hidden;
            return state;
        }
        executeCore(): boolean {
            if(this.isDocumentSaved()) {
                var requestParams = new RequestParams();
                requestParams.immediateSend = true;
                requestParams.processOnCallback = true;
                this.control.serverDispatcher.pushRequest(CommandType.SaveDocument, undefined, { "historyId": this.control.history.getCurrentItemId() }, requestParams);
                return true;
            } else
                return this.executeSaveAsCommand();
        }
        isEnabled(): boolean {
            return super.isEnabled() && this.control.getModifiedState() === IsModified.True && ControlOptions.isEnabled(this.control.options.save);
        }
        isDocumentSaved(): boolean {
            return this.control.getFileName() !== "";
        }
        executeSaveAsCommand(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.FileSaveAs).execute();
        }
    }
}