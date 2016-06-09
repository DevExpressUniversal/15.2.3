module __aspxRichEdit {
    export class HeaderFooterCommandBase<T extends HeaderFooterSubDocumentInfoBase> extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled(), this.getValue());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.sections) && ControlOptions.isEnabled(this.control.options.headersFooters);
        }
        getValue(): any {
            return null;
        }
        static isFirstPageInSection(layoutPage: LayoutPage, section: Section): boolean {
            return layoutPage.getFirstPageInGroup().getStartOffsetContentOfMainSubDocument() <= section.startLogPosition.value;
        }
        static getSectionIndex(layoutPage: LayoutPage, model: DocumentModel) {
            let layoutPageStartPosition = layoutPage.contentIntervals[0].start;
            return Utils.normedBinaryIndexOf(model.sections, s => s.startLogPosition.value - layoutPageStartPosition);
        }
        protected getContainer(section: Section): SectionHeadersFooters<T> { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getManipulator(): HeaderFooterManipulatorBase<T> { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }

    export class SwitchHeaderFooterCommandBase<T extends HeaderFooterSubDocumentInfoBase> extends HeaderFooterCommandBase<T> {
        switchToHeaderFooter(sectionIndex: number, type: HeaderFooterType) {
            let section = this.control.model.sections[sectionIndex];
            var existedHeaderFooter = this.getContainer(section).getObject(type);
            if(existedHeaderFooter)
                return this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(existedHeaderFooter);
            else {
                this.control.history.beginTransaction();
                let newObjectIndex = this.createHeaderFooter(sectionIndex, type);
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(this.getObjectsCache()[newObjectIndex]);
                this.control.history.endTransaction();
                return true;
            }
        }
        createHeaderFooter(sectionIndex: number, type: HeaderFooterType): number {
            var objectIndex = this.getManipulator().createObject(type);
            this.changeHeaderFooterObjectIndex(sectionIndex, type, objectIndex);
            return objectIndex;
        }
        protected changeHeaderFooterObjectIndex(sectionIndex: number, type: HeaderFooterType, newIndex: number) { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getObjectsCache(): HeaderFooterSubDocumentInfoBase[] { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }
}