module __aspxRichEdit {
    export class CloseHeaderFooterCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() &&
                ControlOptions.isEnabled(this.control.options.sections) &&
                ControlOptions.isEnabled(this.control.options.headersFooters) &&
                this.control.model.activeSubDocument.isHeaderFooter();
        }
        executeCore(state: SimpleCommandState, parameter?: any): boolean {
            const res: boolean = this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(this.control.model.mainSubDocument.info);
            this.control.selection.pageIndex = -1;
            return res;
        }
    }
}