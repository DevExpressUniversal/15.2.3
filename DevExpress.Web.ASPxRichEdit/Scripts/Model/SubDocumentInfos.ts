module __aspxRichEdit {
    export class SubDocumentInfoBase {
        subDocumentId: number;

        isMain: boolean = true;
        isHeaderFooter: boolean = false;
        isFooter: boolean = false;
        isHeader: boolean = false;
        isNote: boolean = false;
        isFootNote: boolean = false;
        isEndNote: boolean = false;
        isTextBox: boolean = false;
        isComment: boolean = false;
        isReferenced: boolean = true;

        constructor(subDocumentId: number) {
            this.subDocumentId = subDocumentId;
        }

        getEndPosition(documentModel: DocumentModel) {
            return documentModel.subDocuments[this.subDocumentId].getLastChunk().getEndPosition();
        }
        getSubDocument(documentModel: DocumentModel) {
            return documentModel.subDocuments[this.subDocumentId];
        }
        getType(): SubDocumentInfoType {
            throw new Error(Errors.NotImplemented);
        }

        static create(type: SubDocumentInfoType, subDocumentId: number): SubDocumentInfoBase {
            switch(type) {
                case SubDocumentInfoType.Main:
                    return new MainSubDocumentInfo(subDocumentId);
                case SubDocumentInfoType.Header:
                    return new HeaderSubDocumentInfo(subDocumentId);
                case SubDocumentInfoType.Footer:
                    return new FooterSubDocumentInfo(subDocumentId);
            }
            throw new Error(Errors.NotImplemented);
        }
    }

    export class MainSubDocumentInfo extends SubDocumentInfoBase {
        getType(): SubDocumentInfoType {
            return SubDocumentInfoType.Main;
        }
        getEndPosition(documentModel: DocumentModel) {
            var sections = documentModel.sections;
            var lastSection: Section = sections[sections.length - 1];
            return lastSection.startLogPosition.value + lastSection.getLength();
        }
    }

    export class HeaderFooterSubDocumentInfoBase extends SubDocumentInfoBase {
        headerFooterType: HeaderFooterType = HeaderFooterType.Odd;
        isMain: boolean = false;
        isHeaderFooter: boolean = true;
    }

    export class HeaderSubDocumentInfo extends HeaderFooterSubDocumentInfoBase {
        getType(): SubDocumentInfoType {
            return SubDocumentInfoType.Header;
        }
        isHeader: boolean = true;
    }
    export class FooterSubDocumentInfo extends HeaderFooterSubDocumentInfoBase {
        getType(): SubDocumentInfoType {
            return SubDocumentInfoType.Footer;
        }
        isFooter: boolean = true;
    }

    export class FullChunkAndRunInfo {
        chunkIndex: number;
        runIndex: number;
        chunk: Chunk;
        run: TextRun;

        constructor(chunkIndex: number, chunk: Chunk, runIndex: number, run: TextRun) {
            this.chunkIndex = chunkIndex;
            this.chunk = chunk;
            this.runIndex = runIndex;
            this.run = run;
        }

        public getAbsoluteRunPosition(): number {
            return this.chunk.startLogPosition.value + this.run.startOffset;
        }
    }

    export enum SubDocumentInfoType {
        Main = 0,
        Header = 1,
        Footer = 2
    }
}