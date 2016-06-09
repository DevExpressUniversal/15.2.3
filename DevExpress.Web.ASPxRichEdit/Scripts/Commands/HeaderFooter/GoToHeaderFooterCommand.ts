module __aspxRichEdit {
    export class GoToHeaderFooterCommandBase<T extends HeaderFooterSubDocumentInfoBase> extends SwitchHeaderFooterCommandBase<T> {
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            let headerFooterType = (<HeaderFooterSubDocumentInfoBase>this.control.model.activeSubDocument.info).headerFooterType;
            let layoutPage = this.control.forceFormatPage(this.control.selection.pageIndex);
            let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model);
            this.switchToHeaderFooter(sectionIndex, headerFooterType);
            return true;
        }
    }

    export class GoToHeaderCommand extends GoToHeaderFooterCommandBase<FooterSubDocumentInfo> {
        isEnabled(): boolean {
            return super.isEnabled() && this.control.model.activeSubDocument.isFooter();
        }
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[]{ // ABSTRACT
            return this.control.model.headers;
        }
        protected getContainer(section: Section): SectionHeadersFooters<FooterSubDocumentInfo> { // ABSTRACT
            return section.headers;
        }
        protected getManipulator(): HeaderFooterManipulatorBase<FooterSubDocumentInfo> { // ABSTRACT
            return this.control.modelManipulator.headerManipulator;
        }
        protected changeHeaderFooterObjectIndex(sectionIndex: number, type: HeaderFooterType, newIndex: number) {
            this.control.history.addAndRedo(new ChangeHeaderIndexHistoryItem(this.control.modelManipulator, sectionIndex, type, newIndex));
        }
    }

    export class GoToFooterCommand extends GoToHeaderFooterCommandBase<FooterSubDocumentInfo> {
        isEnabled(): boolean {
            return super.isEnabled() && this.control.model.activeSubDocument.isHeader();
        }
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[]{ // ABSTRACT
            return this.control.model.footers;
        }
        protected getContainer(section: Section): SectionHeadersFooters<FooterSubDocumentInfo> { // ABSTRACT
            return section.footers;
        }
        protected getManipulator(): HeaderFooterManipulatorBase<FooterSubDocumentInfo> { // ABSTRACT
            return this.control.modelManipulator.footerManipulator;
        }
        protected changeHeaderFooterObjectIndex(sectionIndex: number, type: HeaderFooterType, newIndex: number) {
            this.control.history.addAndRedo(new ChangeFooterIndexHistoryItem(this.control.modelManipulator, sectionIndex, type, newIndex));
        }
    }
}