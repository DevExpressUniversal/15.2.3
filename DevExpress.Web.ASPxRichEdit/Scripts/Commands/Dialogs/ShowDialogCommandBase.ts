module __aspxRichEdit {
    export class ShowDialogCommandBase extends CommandBase<ICommandState> {
        initParams: DialogParametersBase;

        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: ICommandState, parameter?: any): boolean {
            var params: DialogParametersBase = this.createParameters(parameter);
            this.initParams = params.clone();
            params.toAnotherMeasuringSystem(this.control.units.twipsToUI);

            if(this.isModal())
                this.control.bars.beginUpdate();
            var prevModifiedState = this.control.getModifiedState();
            this.control.showDialog(this.getDialogName(), params, (result: DialogParametersBase) => {
                if (result) {
                    result.toAnotherMeasuringSystem(this.control.units.UIToTwips);
                    this.control.beginUpdate();
                    this.applyParameters(state, result);
                    this.control.endUpdate();
                }
                if(this.isModal())
                    this.control.bars.endUpdate();
                if(result)
                    this.updateControlState(prevModifiedState);
                if(!ASPx.Browser.TouchUI && this.isModal())
                    this.control.captureFocus();
            },() => {
                if(!ASPx.Browser.TouchUI)
                    this.control.captureFocus();
                this.afterClosing();
            }, this.isModal());
            return true;
        }

        createParameters(commandsParameter: any): DialogParametersBase { return null; }
        applyParameters(state: ICommandState, newParameters: DialogParametersBase) { }
        getDialogName(): string { return null; }
        afterClosing() { }

        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }

        isModal(): boolean {
            return true;
        }
    }

    export class DialogParametersBase implements ICloneable<DialogParametersBase>, ISupportCopyFrom<DialogParametersBase>, ISupportTranslateToAnotherMeasuringSystem {
        clone(): DialogParametersBase {
            var newInstance: DialogParametersBase = this.getNewInstance();
            newInstance.copyFrom(this);
            return newInstance;
        }

        getNewInstance(): DialogParametersBase {
            throw new Error(Errors.NotImplemented);
        }

        copyFrom(obj: DialogParametersBase) {
            throw new Error(Errors.NotImplemented);
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
            throw new Error(Errors.NotImplemented);
        }
    }

    export enum DialogTitleText {
        SaveAsFile = 0,
        OpenFile = 1,
        Font = 2,
        Paragraph = 3,
        PageSetup = 4,
        Columns = 5,
        InsertImage = 6,
        Error = 7
    }
}