module __aspxRichEdit {
    export class Section {
        public startLogPosition: Position;
        private length: number;
        public sectionProperties: SectionProperties;
        public documentModel: DocumentModel;// remove
        public headers: SectionHeadersFooters<HeaderSubDocumentInfo>;
        public footers: SectionHeadersFooters<FooterSubDocumentInfo>;

        constructor(documentModel: DocumentModel, startLogPosition: Position, length: number, sectionProperties: SectionProperties) { 
            this.documentModel = documentModel;
            this.startLogPosition = startLogPosition;
            this.length = length;
            this.sectionProperties = sectionProperties;
            this.headers = new SectionHeaders(this);
            this.footers = new SectionFooters(this);
        }

        getLength(): number {
            return this.length;
        }

        // ????? ?? ???? ??? ????????? ?? ???????? ?????. ? ?????? ????????? ?? ????? ????.
        setLength(subDocument: SubDocument, newLength: number) {
            if(subDocument.isMain())
                this.length = newLength;
        }

        public getEndPosition(): number {
            return this.startLogPosition.value + this.length;
        }
    }
} 