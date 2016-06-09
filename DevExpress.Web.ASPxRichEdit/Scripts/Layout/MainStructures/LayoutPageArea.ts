module __aspxRichEdit {
    export class LayoutPageArea extends LayoutItemBase {
        subDocument: SubDocument;
        columns: LayoutColumn[] = [];
        pageOffset: number;
        //sectionIndex: number; // sectionIndex what start pageArea. first symbol of pageArea belongs to this section
        // For main - >= 0, for othersSubDocs = -1.
        // Can be situation when [.. sec1, sec2,...] and sec2.sectionIndex - sec1.sectionIndex  > 1

        constructor(subDocument: SubDocument) {
            super();
            this.subDocument = subDocument;
        }

        public getEndPosition(): number {
            return this.pageOffset + this.columns[this.columns.length - 1].getEndPosition();
        }

        public getLastColumn(): LayoutColumn {
            return this.columns[this.columns.length - 1];
        }
    }
}