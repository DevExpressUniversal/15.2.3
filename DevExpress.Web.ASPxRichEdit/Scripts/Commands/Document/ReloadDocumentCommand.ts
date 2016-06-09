module __aspxRichEdit {
    export class ReloadDocumentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        executeCore(state: SimpleCommandState): boolean {
            this.control.closeDocument();
            var requestParams = new RequestParams();
            requestParams.immediateSend = true;
            requestParams.lockQueue = true;
            this.control.serverDispatcher.pushRequest(CommandType.ReloadDocument, undefined, undefined, requestParams);
            return true;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
}