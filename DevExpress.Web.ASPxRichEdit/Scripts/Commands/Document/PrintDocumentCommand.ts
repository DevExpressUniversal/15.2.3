module __aspxRichEdit {
    export class PrintDocumentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.printing !== DocumentCapability.Hidden;
            return state;
        }
        executeCore(state: SimpleCommandState): boolean {
            this.control.commandManager.getCommand(RichEditClientCommand.UpdateAllFields).execute(() => {
                if(this.control.serverDispatcher.hasQueue()) {
                    var requestParams = new RequestParams();
                    requestParams.immediateSend = true;
                    this.control.serverDispatcher.pushRequest(CommandType.DelayedPrint, undefined, undefined, requestParams);
                } else
                    this.control.sendDownloadRequest(DownloadRequestType.PrintCurrentDocument);
            });
            return true;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.printing);
        }
    }

    export enum DownloadRequestType {
        PrintCurrentDocument = 0,
        DownloadCurrentDocument = 1,
        DownloadMergedDocument = 2
    }
}