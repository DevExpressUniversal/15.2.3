module __aspxRichEdit {
    export interface ICommand {
        control: IRichEditControl;
        getState(): ICommandState;
        execute(parameter?: any): boolean;
    }

    export interface ICommandState {
        enabled: boolean;
        value: any;
        visible: boolean;
        denyUpdateValue: boolean;
        items: any[];
    }
}