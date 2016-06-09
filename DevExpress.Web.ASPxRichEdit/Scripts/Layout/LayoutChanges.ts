module __aspxRichEdit {
    export class LayoutChangeBase<LayoutElementType extends Rectangle> {
        public changeType: LayoutChangeType;
        public index: number;
        public layoutElement: LayoutElementType;

        constructor(changeType: LayoutChangeType, index: number, layoutElement: LayoutElementType) {
            this.changeType = changeType;
            this.index = index;
            this.layoutElement = layoutElement;
        }
    }

    export class PageChange extends LayoutChangeBase<LayoutPage> {
        public pageAreaChanges: PageAreaChange[];

        constructor(changeType: LayoutChangeType, index: number, layoutElement: LayoutPage, pageAreaChanges: PageAreaChange[]) {
            super(changeType, index, layoutElement);
            this.pageAreaChanges = pageAreaChanges;
        }
    }

    export class PageAreaChange extends LayoutChangeBase<LayoutPageArea> {
        public columnChanges: ColumnChange[];

        constructor(changeType: LayoutChangeType, index: number, layoutElement: LayoutPageArea, columnChanges: ColumnChange[]) {
            super(changeType, index, layoutElement);
            this.columnChanges = columnChanges;
        }
    }

    export class ColumnChange extends LayoutChangeBase<LayoutColumn> {
        rowChanges: RowChange[];
        tableChanges: TableChange[]; // here maybe will more nested levels
        paragraphFrameChanges: ParagraphFrameChange[];

        constructor(changeType: LayoutChangeType, index: number, layoutElement: LayoutColumn,
            rowChanges: RowChange[], tableChanges: TableChange[], paragraphFrameChanges: ParagraphFrameChange[]) {
            super(changeType, index, layoutElement);
            this.rowChanges = rowChanges;
            this.tableChanges = tableChanges;
            this.paragraphFrameChanges = paragraphFrameChanges;
        }
    }

    export class RowChange extends LayoutChangeBase<LayoutRow> {
    }

    export class TableChange extends LayoutChangeBase<LayoutTableColumnInfo> {
    }

    export class ParagraphFrameChange extends LayoutChangeBase<ParagraphFrame> {
    }

    export enum LayoutChangeType {
        Added,
        Deleted,
        Replaced,
        Updated,
        Inserted
    }
}