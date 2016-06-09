module __aspxRichEdit {
    export class DialogOpenFileCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.open !== DocumentCapability.Hidden;
            return state;
        }
        createParameters(): OpenFileDialogParameters {
            var parameters = new OpenFileDialogParameters();
            parameters.src = null;
            return parameters;
        }
        executeCore(state: ICommandState): boolean {
            if(this.control.hasWorkDirectory())
                return super.executeCore(state, null);
            else
                return this.executeShowErrorMessageCommand();
        }
        applyParameters(state: IntervalCommandState, params: OpenFileDialogParameters) {
            if(this.control.getModifiedState() && !this.control.confirmOnLosingChanges())
                return false;
            this.control.closeDocument();
            var requestParams = new RequestParams();
            requestParams.lockQueue = true;
            requestParams.immediateSend = true;
            requestParams.processOnCallback = true;
            this.control.serverDispatcher.pushRequest(CommandType.OpenDocument, { "src": params.src }, undefined, requestParams);
        }
        getDialogName() {
            return "FileOpen";
        }
        executeShowErrorMessageCommand(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorOpeningAndOverstoreImpossibleMessageCommand).execute();
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.open);
        }
    }

    export class OpenFileDialogParameters extends DialogParametersBase {
        src: string = null;

        clone(): OpenFileDialogParameters {
            var obj = new OpenFileDialogParameters();
            obj.copyFrom(this);
            return obj;
        }
        copyFrom(obj: OpenFileDialogParameters) {
            this.src = obj.src;
        }
        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}