module __aspxRichEdit {
    export class ChangeActiveSubDocumentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        executeCore(state: ICommandState, info: SubDocumentInfoBase): boolean {
            if(!(info instanceof SubDocumentInfoBase))
                throw new Error("Parameter should be instance of SubDocumentInfoBase");
            var previousActiveSubDocumentInfo = this.control.model.activeSubDocument.info;
            this.control.model.activeSubDocument = info.getSubDocument(this.control.model);
            this.control.selection.intervals = [];
            if(info.isMain) {
                let layoutPage = this.control.forceFormatPage(this.control.selection.pageIndex);
                this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument,
                    [new FixedInterval(layoutPage.getStartOffsetContentOfMainSubDocument(), 0)], this.control.selection, UpdateInputPositionProperties.Yes, false));
                if(previousActiveSubDocumentInfo.isHeaderFooter)
                    this.forceFormatPagesWithHeaderFooter(<HeaderFooterSubDocumentInfoBase>previousActiveSubDocumentInfo);
            }
            else {
                this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, [new FixedInterval(0, 0)],
                    this.control.selection, UpdateInputPositionProperties.Yes, false));
            }
            if(info.isHeaderFooter)
                this.control.bars.activateContextItem(RichEditClientCommand.ContextItem_HeadersFooters);
            return true;
        }

        private forceFormatPagesWithHeaderFooter(headerFooterSubDocumentInfo: HeaderFooterSubDocumentInfoBase) {
            let pageIndex = this.control.selection.pageIndex;
            let currentPage = this.control.layout.pages[this.control.selection.pageIndex];
            if(currentPage) {
                let sectionIndex = HeaderFooterCommandBase.getSectionIndex(currentPage, this.control.model);
                let section = this.control.model.sections[sectionIndex];

                let type = headerFooterSubDocumentInfo.headerFooterType;
                let isLinkedToPrevious = section.headers.isLinkedToPrevious(type) || section.footers.isLinkedToPrevious(type);
                let isFirstPageInSection = HeaderFooterCommandBase.isFirstPageInSection(currentPage, section);

                if(isLinkedToPrevious && sectionIndex > 0) {
                    sectionIndex--;
                    let currentSectionIndex = sectionIndex;
                    for(let i = currentSectionIndex; i >= 0; i--) {
                        let currentSection = this.control.model.sections[i];
                        if(sectionIndex === 0 || !(currentSection.headers.isLinkedToPrevious(type) || currentSection.footers.isLinkedToPrevious(type)))
                            break;
                        sectionIndex--;
                    }
                    section = this.control.model.sections[sectionIndex];
                }
                
                let firstPageInSectionIndex = 0;
                if(!isLinkedToPrevious && isFirstPageInSection)
                    firstPageInSectionIndex = pageIndex;
                else {
                    for(let i = pageIndex; i >= 0; i--) {
                        if(HeaderFooterCommandBase.isFirstPageInSection(this.control.layout.pages[i], section)) {
                            firstPageInSectionIndex = i;
                            break;
                        }
                    }
                }

                let startPageIndex = pageIndex;
                if(section.sectionProperties.differentFirstPage && isFirstPageInSection)
                    startPageIndex = firstPageInSectionIndex;
                else if(this.control.model.differentOddAndEvenPages) {
                    if(isLinkedToPrevious) {
                        let isEvenPage = Utils.isEven(pageIndex);
                        let isEvenFirstPage = Utils.isEven(firstPageInSectionIndex);
                        startPageIndex = isEvenPage === isEvenFirstPage ? firstPageInSectionIndex : firstPageInSectionIndex + 1;
                    } else {
                        let currentPageIndex = startPageIndex;
                        for(let i = currentPageIndex; i >= 0; i -= 2) {
                            if(this.control.layout.pages[i].getStartOffsetContentOfMainSubDocument() < section.startLogPosition.value)
                                break;
                            startPageIndex = i;
                        }
                    }
                } else
                    startPageIndex = firstPageInSectionIndex;
                if(section.sectionProperties.differentFirstPage && !isFirstPageInSection && startPageIndex === firstPageInSectionIndex)
                    startPageIndex += this.control.model.differentOddAndEvenPages ? 2 : 1;

                let intervalStart = this.control.layout.pages[startPageIndex].getStartOffsetContentOfMainSubDocument();
                let intervalLength = this.control.model.mainSubDocument.getDocumentEndPosition() - intervalStart;
                this.control.formatterOnIntervalChanged(new FixedInterval(intervalStart, intervalLength), this.control.model.mainSubDocument);
            }
        }
    }
} 