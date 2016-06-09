module __aspxRichEdit {
    export class SectionHeadersFooters<T extends HeaderFooterSubDocumentInfoBase> implements ISupportCopyFrom<SectionHeadersFooters<T>>, ICloneable<SectionHeadersFooters<T>> {
        static INVALID_INDEX = -1;

        section: Section;
        private indices: { [type: number]: number } = {};

        constructor(section: Section) {
            this.section = section;
            this.setObjectIndex(HeaderFooterType.Even, SectionHeadersFooters.INVALID_INDEX);
            this.setObjectIndex(HeaderFooterType.Odd, SectionHeadersFooters.INVALID_INDEX);
            this.setObjectIndex(HeaderFooterType.First, SectionHeadersFooters.INVALID_INDEX);
        }

        getObject(type: HeaderFooterType): T {
            let index = this.getObjectIndex(type);
            return this.getObjectsCache()[index];
        }
        getObjectIndex(type: HeaderFooterType): number {
            return this.indices[type];
        }
        setObjectIndex(type: HeaderFooterType, objectIndex: number) {
            this.indices[type] = objectIndex;
        }
        getActualObject(firstPageOfSection: boolean, isEvenPage: boolean): T {
            let type = SectionHeadersFooters.getActualObjectType(this.section, firstPageOfSection, isEvenPage);
            let index = this.getObjectIndex(type);
            return this.getObjectsCache()[index];
        }
        
        copyFrom(source: SectionHeadersFooters<T>) {
            this.indices = {};
            this.setObjectIndex(HeaderFooterType.Even, source.getObjectIndex(HeaderFooterType.Even));
            this.setObjectIndex(HeaderFooterType.Odd, source.getObjectIndex(HeaderFooterType.Odd));
            this.setObjectIndex(HeaderFooterType.First, source.getObjectIndex(HeaderFooterType.First));
        }
        clone(): SectionHeadersFooters<T> {
            let clone = new SectionHeadersFooters<T>(this.section);
            clone.copyFrom(this);
            return clone;
        }
        isLinkedToPrevious(type: HeaderFooterType): boolean {
            let previousSection: Section = this.section.documentModel.getPreviousSection(this.section);
            return previousSection && this.getContainer(previousSection).getObjectIndex(type) === this.getObjectIndex(type);
        }
        canLinkToPrevious(): boolean {
            return !!this.section.documentModel.getPreviousSection(this.section);
        }

        static getActualObjectType(section: Section, firstPageOfSection: boolean, isEvenPage: boolean): HeaderFooterType {
            if(firstPageOfSection && section.sectionProperties.differentFirstPage)
                return HeaderFooterType.First;
            if(isEvenPage) {
                if(section.documentModel.differentOddAndEvenPages)
                    return HeaderFooterType.Even;
                return HeaderFooterType.Odd;
            }
            return HeaderFooterType.Odd;
        }

        protected getObjectsCache(): T[] { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getContainer(section: Section): SectionHeadersFooters<T> { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }

    export class SectionHeaders extends SectionHeadersFooters<HeaderSubDocumentInfo> {
        protected getContainer(section: Section): SectionHeadersFooters<HeaderSubDocumentInfo> {
            return section.headers;
        }
        protected getObjectsCache(): HeaderSubDocumentInfo[] {
            return this.section.documentModel.headers;
        }
    }

    export class SectionFooters extends SectionHeadersFooters<FooterSubDocumentInfo> {
        protected getContainer(section: Section): SectionHeadersFooters<FooterSubDocumentInfo> {
            return section.footers;
        }
        protected getObjectsCache(): FooterSubDocumentInfo[]{
            return this.section.documentModel.footers;
        }
    }
}