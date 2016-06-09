module __aspxRichEdit {
    export class DocumentLayout {
        public pages: LayoutPage[]; //The collection of the pages, which was previously calculated. All items is not null
        public validPageCount; //The number of valid pages in layout. All pages with pageIndex >= validPageCount are invalid and can't be used for drawing\editing\etc...
        public lastMaxNumPages; // Don't use it. It ONLY for {numpages} field.
        public isFullyFormatted: boolean; // mean documentFormatter now in state DocumentEnd
        public isShowTableGridLines: boolean;
        public pageColor: number;

        constructor() {
            this.setEmptyLayout(ColorHelper.NO_COLOR, false);
        }

        public setEmptyLayout(pageColor: number, isShowTableGridLines: boolean) {
            this.pages = [];
            this.validPageCount = 0;
            this.lastMaxNumPages = 0;
            this.isFullyFormatted = false;
            this.pageColor = pageColor;
            this.isShowTableGridLines = isShowTableGridLines;
        }

        public getLastValidPage(): LayoutPage {
            return this.pages[this.validPageCount - 1];
        }
    }
} 