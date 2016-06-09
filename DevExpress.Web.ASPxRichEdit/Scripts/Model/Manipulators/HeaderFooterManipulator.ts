module __aspxRichEdit {
    export class HeaderFooterManipulatorBase<T extends HeaderFooterSubDocumentInfoBase> {
        protected manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }
        createObject(type: HeaderFooterType): number {
            let object = this.createObjectCore(type);
            object.headerFooterType = type;
            let objectIndex = this.getObjectsCache().push(object) - 1;
            this.manipulator.dispatcher.notifyHeaderFooterCreated(this.isHeader(), type, object);
            return objectIndex;
        }
        changeObjectIndex(sectionIndex: number, type: HeaderFooterType, objectIndex: number): number {
            let section = this.manipulator.model.sections[sectionIndex];
            let oldIndex = this.getContainer(section).getObjectIndex(type);
            this.getContainer(section).setObjectIndex(type, objectIndex);
            this.manipulator.dispatcher.notifyHeaderFooterIndexChanged(sectionIndex, this.isHeader(), type, objectIndex);
            return oldIndex;
        }
        protected createObjectCore(type: HeaderFooterType): T { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getObjectsCache(): T[] { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected isHeader(): boolean { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getContainer(section: Section): SectionHeadersFooters<T> { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }

    export class HeaderManipulator extends HeaderFooterManipulatorBase<HeaderSubDocumentInfo> {
        protected createObjectCore(type: HeaderFooterType): HeaderSubDocumentInfo {
            return <HeaderSubDocumentInfo>this.manipulator.model.createSubDocument(SubDocumentInfoType.Header).info;
        }
        protected getObjectsCache(): HeaderSubDocumentInfo[] {
            return this.manipulator.model.headers;
        }
        protected isHeader(): boolean {
            return true;
        }
        protected getContainer(section: Section): SectionHeadersFooters<HeaderSubDocumentInfo> {
            return section.headers;
        }
    }

    export class FooterManipulator extends HeaderFooterManipulatorBase<FooterSubDocumentInfo> {
        protected createObjectCore(type: HeaderFooterType): FooterSubDocumentInfo {
            return <FooterSubDocumentInfo>this.manipulator.model.createSubDocument(SubDocumentInfoType.Footer).info;
        }
        protected getObjectsCache(): FooterSubDocumentInfo[] {
            return this.manipulator.model.footers;
        }
        protected isHeader(): boolean {
            return false;
        }
        protected getContainer(section: Section): SectionHeadersFooters<FooterSubDocumentInfo> {
            return section.footers;
        }
    }
}