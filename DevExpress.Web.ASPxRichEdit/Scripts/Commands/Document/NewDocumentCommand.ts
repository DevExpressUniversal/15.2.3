module __aspxRichEdit {
    export class NewDocumentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.createNew !== DocumentCapability.Hidden;
            return state;
        }
        executeCore(state: SimpleCommandState): boolean {
            if(this.control.getModifiedState() && !this.control.confirmOnLosingChanges())
                return false;
            this.control.closeDocument();
            var requestParams = new RequestParams();
            requestParams.lockQueue = true;
            requestParams.immediateSend = true;
            this.control.serverDispatcher.pushRequest(CommandType.NewDocument, undefined, undefined, requestParams);
            return true;
        }
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.createNew);
        }
    }
}