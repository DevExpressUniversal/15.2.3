module __aspxRichEdit {
    export class LayoutSelection {
        pages: { [pageIndex: number]: LayoutSelectionItem[] } = {};
        scrollPosition: LayoutPosition;
        subDocumentInfo: SubDocumentInfoBase;
        pageIndex: number;

        firstItem: LayoutSelectionItem;
        lastItem: LayoutSelectionItem;

        constructor(subDocumentInfo: SubDocumentInfoBase) {
            this.subDocumentInfo = subDocumentInfo;
        }

        public registerItem(item: LayoutSelectionItem) {
            if(!this.pages[item.pageIndex])
                this.pages[item.pageIndex] = [];
            this.pages[item.pageIndex].push(item);
        }
    }

    export class LayoutSelectionItem extends Rectangle implements IEquatable<LayoutSelectionItem> {
        public pageIndex: number;
        public pageAreaIndex: number;
        public columnIndex: number;
        public subDocumentID: number;

        constructor(subDocumentID: number, pageIndex: number, pageAreaIndex: number, columnIndex: number) {
            super();
            this.subDocumentID = subDocumentID;
            this.pageIndex = pageIndex;
            this.pageAreaIndex = pageAreaIndex;
            this.columnIndex = columnIndex;
        }

        equals(obj: LayoutSelectionItem): boolean {
            return this.equalsLocation(obj) && this.x == obj.x && this.width === obj.width && this.height === obj.height;
        }
        equalsLocation(obj: LayoutSelectionItem) {
            return this.subDocumentID == obj.subDocumentID && this.pageIndex == obj.pageIndex && this.pageAreaIndex == obj.pageAreaIndex && this.columnIndex == obj.columnIndex;
        }
        getHash(): string {
            return this.getLocationHash() + "_" + this.x + "_" + this.y + "_" + this.width + "_" + this.height;
        }
        getLocationHash(): string {
            return "" + this.subDocumentID + "_" + this.pageIndex + "_" + this.pageAreaIndex + "_" + this.columnIndex;
        }
    }

    export class LayoutSelectionCursor extends LayoutSelectionItem {
        getLocationHash(): string {
            return super.getLocationHash() + "_" + "C";
        }
    }
}