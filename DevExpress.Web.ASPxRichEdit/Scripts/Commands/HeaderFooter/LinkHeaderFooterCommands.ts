module __aspxRichEdit {
    export class LinkHeaderFooterToPreviousCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            if(this.isEnabled()) {
                if(this.control.model.activeSubDocument.isHeader())
                    return this.control.commandManager.getCommand(RichEditClientCommand.LinkHeader).getState();
                else
                    return this.control.commandManager.getCommand(RichEditClientCommand.LinkFooter).getState();
            }
            else
                return new SimpleCommandState(this.isEnabled(), false);
        }
        executeCore(state: SimpleCommandState, parameter?: any): boolean {
            if(this.control.model.activeSubDocument.isHeader())
                return this.control.commandManager.getCommand(RichEditClientCommand.LinkHeader).execute();
            else
                return this.control.commandManager.getCommand(RichEditClientCommand.LinkFooter).execute();
        }
        isEnabled(): boolean {
            return super.isEnabled() &&
                ControlOptions.isEnabled(this.control.options.sections) &&
                ControlOptions.isEnabled(this.control.options.headersFooters) &&
                this.control.model.activeSubDocument.isHeaderFooter() &&
                this.control.selection.pageIndex > 0 &&
                this.control.model.sections.length > 1;
        }
    }

    export class LinkHeaderFooterCommandBase<T extends HeaderFooterSubDocumentInfoBase> extends HeaderFooterCommandBase<T> {
        executeCore(state: SimpleCommandState, parameter?: any): boolean {
            let pageIndex = this.control.selection.pageIndex;
            let layoutPage = this.control.forceFormatPage(pageIndex);
            let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model);
            let section = this.control.model.sections[sectionIndex];
            let type = (<HeaderFooterSubDocumentInfoBase>this.control.model.activeSubDocument.info).headerFooterType;
            this.control.history.beginTransaction();
            if(state.value)
                this.unlinkFromPrevious(sectionIndex, type);
            else
                this.linkToPrevious(sectionIndex, type);
            this.control.history.endTransaction();
            return true;
        }
        isEnabled(): boolean {
            if(super.isEnabled() && this.control.model.activeSubDocument.isHeaderFooter()) {
                let pageIndex = this.control.selection.pageIndex;
                if(super.isEnabled()) {
                    let layoutPage = this.control.forceFormatPage(pageIndex);
                    let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model);
                    return super.isEnabled() && sectionIndex > 0;
                }
            }
            return false;
        }
        getValue(): any {
            if(this.isEnabled()) {
                let pageIndex = this.control.selection.pageIndex;
                let layoutPage = this.control.forceFormatPage(pageIndex);
                let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model);
                let section = this.control.model.sections[sectionIndex];
                let firstPageInSection = HeaderFooterCommandBase.isFirstPageInSection(this.control.layout.pages[pageIndex], section);
                let type = SectionHeadersFooters.getActualObjectType(section, firstPageInSection, Utils.isEven(pageIndex));
                return this.getContainer(this.control.model.sections[sectionIndex]).isLinkedToPrevious(type);
            }
            return null;
        }
        protected linkToPrevious(sectionIndex: number, type: HeaderFooterType) {
            let section = this.control.model.sections[sectionIndex];
            this.performLinkSectionToPrevious(sectionIndex, type, (previousSectionIndex: number) => {
                let previousSection = section.documentModel.sections[previousSectionIndex];
                let prevObjectIndex = this.getContainer(previousSection).getObjectIndex(type);
                if(prevObjectIndex === -1) {
                    prevObjectIndex = this.getManipulator().createObject(type);
                    this.control.history.addAndRedo(this.createChangeObjectIndexHistoryItem(previousSectionIndex, type, prevObjectIndex));
                }
                this.control.history.addAndRedo(this.createChangeObjectIndexHistoryItem(sectionIndex, type, prevObjectIndex));
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(this.getObjectsCache()[prevObjectIndex]);
            });
        }
        protected unlinkFromPrevious(sectionIndex: number, type: HeaderFooterType) {
            let section = this.control.model.sections[sectionIndex];
            this.performLinkSectionToPrevious(sectionIndex, type, (previousSectionIndex: number) => {
                let previousSection = section.documentModel.sections[previousSectionIndex];
                let previousObject = this.getContainer(previousSection).getObject(type);
                let endPosition = previousObject.getEndPosition(section.documentModel);
                let previousObjectCopyInfo = endPosition > 1 ? ModelManipulator.createRangeCopy(previousObject.getSubDocument(section.documentModel), [new FixedInterval(0, endPosition - 1)]) : null;
                let newObjectIndex = this.getManipulator().createObject(type);
                let newObject = this.getObjectsCache()[newObjectIndex];
                let newObjectSubDocument = newObject.getSubDocument(this.control.model);
                this.control.history.addAndRedo(this.createChangeObjectIndexHistoryItem(sectionIndex, type, newObjectIndex));
                if(previousObjectCopyInfo)
                    ModelManipulator.pasteRangeCopy(this.control, newObjectSubDocument, FixedInterval.fromPositions(0, newObjectSubDocument.getDocumentEndPosition()), previousObjectCopyInfo);
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(newObject);
            });
        }
        protected performLinkSectionToPrevious(sectionIndex: number, type: HeaderFooterType, linkAction: (previousSectionIndex: number) => void) {
            let nextSection = this.control.model.sections[sectionIndex + 1];
            let section = this.control.model.sections[sectionIndex];
            let shouldRelinkNextSection = nextSection && this.areSectionsLinked(section, nextSection, type);
            linkAction(sectionIndex - 1);
            if(shouldRelinkNextSection)
                this.linkToPrevious(sectionIndex + 1, type);
        }
        protected areSectionsLinked(section1: Section, section2: Section, type: HeaderFooterType): boolean {
            return this.getContainer(section2).getObject(type) === this.getContainer(section1).getObject(type)
        }
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[] {
            throw new Error(Errors.NotImplemented);
        }
        protected createChangeObjectIndexHistoryItem(sectionIndex: number, type: HeaderFooterType, newIndex: number): ChangeHeaderFooterIndexHistoryItemBase<T> { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }
    export class LinkHeaderCommand extends LinkHeaderFooterCommandBase<HeaderSubDocumentInfo> {
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[]{
            return this.control.model.headers;
        }
        protected getContainer(section: Section): SectionHeadersFooters<HeaderSubDocumentInfo> {
            return section.headers;
        }
        protected getManipulator(): HeaderFooterManipulatorBase<HeaderSubDocumentInfo> { // ABSTRACT
            return this.control.modelManipulator.headerManipulator;
        }
        protected createChangeObjectIndexHistoryItem(sectionIndex: number, type: HeaderFooterType, newIndex: number): ChangeHeaderFooterIndexHistoryItemBase<HeaderSubDocumentInfo> {
            return new ChangeHeaderIndexHistoryItem(this.control.modelManipulator, sectionIndex, type, newIndex);
        }
    }
    export class LinkFooterCommand extends LinkHeaderFooterCommandBase<FooterSubDocumentInfo> {
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[]{
            return this.control.model.footers;
        }
        protected getContainer(section: Section): SectionHeadersFooters<FooterSubDocumentInfo> {
            return section.footers;
        }
        protected getManipulator(): HeaderFooterManipulatorBase<HeaderSubDocumentInfo> {
            return this.control.modelManipulator.footerManipulator;
        }
        protected createChangeObjectIndexHistoryItem(sectionIndex: number, type: HeaderFooterType, newIndex: number): ChangeHeaderFooterIndexHistoryItemBase<HeaderSubDocumentInfo> {
            return new ChangeFooterIndexHistoryItem(this.control.modelManipulator, sectionIndex, type, newIndex);
        }
    }
}