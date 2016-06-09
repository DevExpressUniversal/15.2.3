module __aspxRichEdit {
    export class DifferentFirstPageHeaderFooterCommand extends CommandBase<SimpleCommandState> {
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
            let section = this.control.model.sections[this.getSectionIndex()];
            return section.sectionProperties.differentFirstPage;
        }
        executeCore(state: IntervalCommandState, parameter?: any): boolean {
            let sectionIndex = this.getSectionIndex();
            let section = this.control.model.sections[sectionIndex];

            this.control.history.addAndRedo(new SectionDifferentFirstPageHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, new FixedInterval(section.startLogPosition.value, section.getLength()), !state.value));

            let pageIndex = this.control.selection.pageIndex;
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

        getSectionIndex(): number {
            let pageIndex = this.control.selection.pageIndex;
            let layoutPage = this.control.forceFormatPage(pageIndex);
            return HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model)
        }
    }
}