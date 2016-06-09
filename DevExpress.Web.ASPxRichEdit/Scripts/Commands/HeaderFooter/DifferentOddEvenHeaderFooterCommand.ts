module __aspxRichEdit {
    export class DifferentOddEvenHeaderFooterCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let isEnabled = this.isEnabled();
            return new SimpleCommandState(isEnabled, isEnabled ? this.getValue() : false);
        }
        isEnabled(): boolean {
            return super.isEnabled() &&
                ControlOptions.isEnabled(this.control.options.sections) &&
                ControlOptions.isEnabled(this.control.options.headersFooters) &&
                this.control.model.activeSubDocument.isHeaderFooter();
        }
        getValue(): boolean {
            return this.control.model.differentOddAndEvenPages;
        }
        executeCore(state: IntervalCommandState, parameter?: any): boolean {
            this.control.history.addAndRedo(new DifferentOddAndEvenPagesHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, !state.value));

            let pageIndex = this.control.selection.pageIndex;
            let sectionIndex = HeaderFooterCommandBase.getSectionIndex(this.control.forceFormatPage(pageIndex), this.control.model);
            let section = this.control.model.sections[sectionIndex];
            let isHeader = this.control.model.activeSubDocument.isHeader();
            let sectionHeadersFooters = isHeader ? section.headers : section.footers;
            let type = SectionHeadersFooters.getActualObjectType(section,
                HeaderFooterCommandBase.isFirstPageInSection(this.control.layout.pages[pageIndex], section), Utils.isEven(pageIndex));
            let info = sectionHeadersFooters.getObject(type);
            if(!info) {
                let manipulator = isHeader ? this.control.modelManipulator.headerManipulator : this.control.modelManipulator.footerManipulator;
                manipulator.changeObjectIndex(sectionIndex, type, manipulator.createObject(type));
                info = sectionHeadersFooters.getObject(type);
            }
            if(info)
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(info);
            return true;
        }
    }
}