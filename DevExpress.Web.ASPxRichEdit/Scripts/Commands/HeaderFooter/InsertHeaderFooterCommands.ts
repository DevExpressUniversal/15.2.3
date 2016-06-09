module __aspxRichEdit {
    export class InsertHeaderFooterCommandBase<T extends HeaderFooterSubDocumentInfoBase> extends SwitchHeaderFooterCommandBase<T> {
        executeCore(state: SimpleCommandState, pageIndex: number): boolean {
            // MouseHandler send pageIndex
            // pageIndex === null mean press ribbon button, then get selection pageIndex
            if (pageIndex === null) {
                const subDocument: SubDocument = this.control.model.activeSubDocument;
                if (!subDocument.isMain())
                    throw new Error("Where it calls?");
                const cursorPos: LayoutPosition = LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout,
                    subDocument, this.control.selection.intervals[0].start, DocumentLayoutDetailsLevel.Page,
                    new LayoutPositionCreatorConflictFlags().setDefault(this.control.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
                pageIndex = cursorPos.pageIndex;
            }
            this.control.selection.pageIndex = pageIndex;
            let layoutPage = this.control.forceFormatPage(pageIndex);
            let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model);
            let section = this.control.model.sections[sectionIndex];

            let firstPageInSection = HeaderFooterCommandBase.isFirstPageInSection(layoutPage, section);
            var type = SectionHeadersFooters.getActualObjectType(section, firstPageInSection, Utils.isEven(pageIndex));
            this.switchToHeaderFooter(sectionIndex, type);
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && !this.control.model.activeSubDocument.isHeaderFooter();
        }
    }

    export class InsertHeaderCommand extends InsertHeaderFooterCommandBase<HeaderSubDocumentInfo> {
        protected getContainer(section: Section): SectionHeadersFooters<HeaderSubDocumentInfo> {
            return section.headers;
        }
        protected changeHeaderFooterObjectIndex(sectionIndex: number, type: HeaderFooterType, newIndex: number) {
            this.control.history.addAndRedo(new ChangeHeaderIndexHistoryItem(this.control.modelManipulator, sectionIndex, type, newIndex));
        }
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[]{
            return this.control.model.headers;
        }
        protected getManipulator(): HeaderFooterManipulatorBase<HeaderSubDocumentInfo> { // ABSTRACT
            return this.control.modelManipulator.headerManipulator;
        }
    }

    export class InsertFooterCommand extends InsertHeaderFooterCommandBase<FooterSubDocumentInfo> {
        protected getContainer(section: Section): SectionHeadersFooters<FooterSubDocumentInfo> {
            return section.footers;
        }
        protected changeHeaderFooterObjectIndex(sectionIndex: number, type: HeaderFooterType, newIndex: number) {
            this.control.history.addAndRedo(new ChangeFooterIndexHistoryItem(this.control.modelManipulator, sectionIndex, type, newIndex));
        }
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[]{
            return this.control.model.footers;
        }
        protected getManipulator(): HeaderFooterManipulatorBase<FooterSubDocumentInfo> { // ABSTRACT
            return this.control.modelManipulator.footerManipulator;
        }
    }
}