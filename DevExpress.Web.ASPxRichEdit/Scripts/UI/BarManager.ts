module __aspxRichEdit {
    export interface IBar {
        getCommandKeys(): RichEditClientCommand[];
        onChanged: EventDispatcher<IBarListener>;
        setItemValue(key: RichEditClientCommand, value: any);
        setItemEnabled(key: RichEditClientCommand, enabled: boolean);
        setItemVisible(key: RichEditClientCommand, visible: boolean);
        setItemSubItems(key: RichEditClientCommand, subItems: any[]);
        setEnabled(enabled: boolean);
        isVisible(): boolean;
        isContextMenu(): boolean;

        hasContextItems(): boolean;
        getContextKeys(): RichEditClientCommand[];
        setContextItemVisible(key: RichEditClientCommand, visible: boolean);
        activateContextItem(key: RichEditClientCommand);
    }

    export interface IBarListener {
        NotifyBarCommandExecuted(commandID: RichEditClientCommand, parameter: any);
        NotifyBarUpdateRequested();
    }

    export class BarManager extends BatchUpdatableObject implements IBarListener, ISelectionChangesListener {
        private bars: IBar[];
        private control: IRichEditControl;

        constructor(control: IRichEditControl, bars: IBar[]) {
            super();
            this.bars = bars;
            this.control = control;
            for(var i = 0, bar: IBar; bar = bars[i]; i++) {
                bar.onChanged.add(this);
            }
        }
        activateContextItem(clientCommand: RichEditClientCommand) {
            for(var i = 0, bar: IBar; bar = this.bars[i]; i++) {
                if(bar.hasContextItems()) {
                    bar.setContextItemVisible(clientCommand, true);
                    bar.activateContextItem(clientCommand);
                }
            }
        }
        updateContextMenu() {
            for(var i = 0, bar: IBar; bar = this.bars[i]; i++) {
                if(bar.isContextMenu()) {
                    var commandKeys = bar.getCommandKeys();
                    for(var j = 0, commandKey; commandKey = commandKeys[j]; j++) {
                        this.updateBarItem(bar, commandKey);
                    }
                }
            }
        }
        updateItemsState(queryCommands?: RichEditClientCommand[]) {
            if(this.isUpdateLocked()) return;
            var queryCommandsHash: { [key: number]: boolean; } = {};
            if(queryCommands) {
                for(var i = 0, queryCommandKey: number; (queryCommandKey = queryCommands[i]) !== undefined; i++) {
                    queryCommandsHash[queryCommandKey] = true;
                }
            }
            for(var i = 0, bar: IBar; bar = this.bars[i]; i++) {
                if(bar.isVisible()) {
                    let commandKeys = bar.getCommandKeys();
                    for(let j = 0, commandKey; commandKey = commandKeys[j]; j++) {
                        if(queryCommands && !queryCommandsHash[commandKey]) continue;
                        this.updateBarItem(bar, commandKey);
                    }
                }
                if(bar.hasContextItems()) {
                    let commandKeys = bar.getContextKeys();
                    for(let j = 0, commandKey; commandKey = commandKeys[j]; j++) {
                        if(queryCommands && !queryCommandsHash[commandKey]) continue;
                        this.updateContextItem(bar, commandKey);
                    }
                }
            }
        }
        private updateBarItem(bar: IBar, commandKey: RichEditClientCommand) {
            var command = this.control.commandManager.getCommand(commandKey);
            if(command) {
                var commandState = command.getState();
                bar.setItemVisible(commandKey, commandState.visible);
                if(commandState.visible) {
                    bar.setItemEnabled(commandKey, commandState.enabled);
                    if(!commandState.denyUpdateValue) {
                        var itemValue = this.getItemValue(commandState.value);
                        if(commandState.items)
                            bar.setItemSubItems(commandKey, commandState.items);
                        bar.setItemValue(commandKey, itemValue);
                    }
                }
            }
        }
        private updateContextItem(bar: IBar, commandKey: RichEditClientCommand) {
            var command = this.control.commandManager.getCommand(commandKey);
            if(command) {
                bar.setContextItemVisible(commandKey, command.getState().visible);
            }
        }

        setEnabled(enabled) {
            for(var i = 0, bar: IBar; bar = this.bars[i]; i++)
                bar.setEnabled(enabled);
        }

        //#region IBarListener
        NotifyBarCommandExecuted(commandID: RichEditClientCommand, parameter: any) {
            var command = this.control.commandManager.getCommand(commandID);
            if(command) {
                var executeResult = this.control.commandManager.executeCommand(command,parameter);
                if(!executeResult)
                    this.updateItemsState([commandID]);
            }
            if(!ASPx.Browser.TouchUI)
                setTimeout(() => {
                    this.control.commandManager.control.captureFocus();
                }, 100);
        }
        NotifyBarUpdateRequested() {
            this.updateItemsState();
        }
        //#endregion

        //#region ISelectionChangesListener
        NotifySelectionChanged(selection: Selection) {
            for (var i = 0, bar: IBar; bar = this.bars[i]; i++)
                this.updateItemsState();
        }
        NotifyFocusChanged(inFocus: boolean) {

        }
        //#endregion

        private getItemValue(value: any): any {
            if(value instanceof FontInfo) {
                if((<FontInfo>value).canBeSet)
                    return (<FontInfo>value).index;
                else
                    return (<FontInfo>value).name;
            }
            else
                return value;
        }
    }
}