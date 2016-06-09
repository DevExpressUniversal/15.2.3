module __aspxRichEdit {
    export class CommandBase<T extends ICommandState> implements ICommand {
        control: IRichEditControl;

        constructor(control: IRichEditControl) {
            this.control = control;
        }

        getState(): T {
            throw new Error(Errors.NotImplemented);
        }
        execute(parameter?: any): boolean {
            var state = this.getState();
            var modifiedState = this.control.getModifiedState();
            if(!state.enabled)
                return false;
            this.control.beginUpdate();
            this.control.commandManager.beforeExecuting(this);
            var executed = this.executeCore(state, parameter);
            this.control.commandManager.afterExecuting();
            this.control.endUpdate();
            if(executed)
                this.updateControlState(modifiedState);
            return executed;
        }
        executeCore(state: T, parameter: any): boolean {
            throw new Error(Errors.NotImplemented);
        }
        isEnabled(): boolean {
            return this.control.readOnly === ReadOnlyMode.None || this.isEnabledInReadOnlyMode();
        }
        updateControlState(prevModifiedState: IsModified) {
            if(!this.lockInputPositionUpdating(prevModifiedState))
                this.control.inputPosition.reset();
            if(!this.lockUIUpdating(prevModifiedState)) {
                this.control.bars.updateItemsState();
                this.control.horizontalRulerControl.update();
            }
        }
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return false;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return false;
        }
        isEnabledInReadOnlyMode(): boolean {
            return false;
        }
    }
}  