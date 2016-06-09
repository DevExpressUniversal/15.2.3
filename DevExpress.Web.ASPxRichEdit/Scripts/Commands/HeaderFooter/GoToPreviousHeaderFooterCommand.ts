module __aspxRichEdit {
    export class GoToPreviousHeaderFooterCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() &&
                ControlOptions.isEnabled(this.control.options.sections) &&
                ControlOptions.isEnabled(this.control.options.headersFooters) &&
                this.control.model.activeSubDocument.isHeaderFooter();
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            let pageIndex = this.control.selection.pageIndex - 1;
            let isHeader = this.control.model.activeSubDocument.isHeader();
            let activeSubDocument = this.control.model.activeSubDocument;
            let layoutPage: LayoutPage;
            while(pageIndex >= 0 && (layoutPage = this.control.forceFormatPage(pageIndex))) {
                for(let subDocumentId in layoutPage.otherPageAreas) {
                    if(!layoutPage.otherPageAreas.hasOwnProperty(subDocumentId)) continue;
                    let pageArea = layoutPage.otherPageAreas[subDocumentId];
                    if((isHeader && pageArea.subDocument.isHeader()) || !isHeader && pageArea.subDocument.isFooter()) {
                        if(activeSubDocument === pageArea.subDocument)
                            continue;
                        this.control.selection.pageIndex = pageIndex;
                        this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(pageArea.subDocument.info);
                        return true;
                    }
                }
                pageIndex--;
            }
            return false;
        }
    }
}