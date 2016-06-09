module __aspxRichEdit {
    // read before get layoutPosition
    // http://workservices01/OpenWiki/ow.asp?p=ASPxRichEdit_HiddenRuns#getLayoutPositionProblem

    class PageIndexAndInterval {
        interval: FixedInterval;
        pageIndex: number;

        constructor(pageIndex: number, interval: FixedInterval) {
            this.interval = interval;
            this.pageIndex = pageIndex;
        }
    }

    export class LayoutPositionCreatorConflictFlags implements ISupportCopyFrom<LayoutPositionCreatorConflictFlags>, ICloneable<LayoutPositionCreatorConflictFlags>{
        left: boolean;
        middle: boolean;
        right: boolean;
        simple: boolean;

        setDefault(defaultVal: boolean): LayoutPositionCreatorConflictFlags {
            this.left = defaultVal;
            this.middle = defaultVal;
            this.right = defaultVal;
            this.simple = defaultVal;
            return this;
        }

        setCustom(left: boolean, middle: boolean, right: boolean, simple: boolean): LayoutPositionCreatorConflictFlags {
            this.left = left;
            this.middle = middle;
            this.right = right;
            this.simple = simple;
            return this;
        }

        allIsTrue(): boolean {
            return this.left && this.middle && this.right && this.simple;
        }

        atLeastOneIsTrue(): boolean {
            return this.left || this.middle || this.right || this.simple;
        }

        atLeastOneIsFalse(): boolean {
            return !(this.left && this.middle && this.right && this.simple);
        }

        copyFrom(obj: LayoutPositionCreatorConflictFlags): LayoutPositionCreatorConflictFlags {
            this.left = obj.left;
            this.middle = obj.middle;
            this.right = obj.right;
            this.simple = obj.simple
            return this;
        }

        clone(): LayoutPositionCreatorConflictFlags {
            return new LayoutPositionCreatorConflictFlags().copyFrom(this);
        }
    }

    export class LayoutPositionCreator {
        protected layout: DocumentLayout;
        protected subDocument: SubDocument;
        public result: LayoutPosition;
        public startPosition: number; // const

        protected endRowConflictFlags: LayoutPositionCreatorConflictFlags;
        protected middleRowConflictFlags: LayoutPositionCreatorConflictFlags;
        
        protected position: number;
        protected detailsLevel: DocumentLayoutDetailsLevel;

        // subDocument always set!
        constructor(documentLayout: DocumentLayout, subDocument: SubDocument, logPosition: number, detailsLevel: DocumentLayoutDetailsLevel) {
            this.layout = documentLayout;
            this.subDocument = subDocument;
            this.position = logPosition;
            this.startPosition = logPosition;
            this.detailsLevel = detailsLevel;
            this.result = new LayoutPosition(detailsLevel);
        }

        public create(endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags) {
            this.endRowConflictFlags = endRowConflictFlags;
            this.middleRowConflictFlags = middleRowConflictFlags;
        }

        // get reduced info
        public static createLightLayoutPosition(documentLayout: DocumentLayout, subDocumen: SubDocument, logPosition: number, pageIndex: number, detailsLevel: DocumentLayoutDetailsLevel,
            endOfLine: boolean, closerToTheRightEdgeHiddenBox: boolean): LayoutRowPosition {
            return <LayoutRowPosition>(subDocumen.isMain() ?
                new LayoutPositionMainSubDocumentCreator(documentLayout, subDocumen, logPosition, detailsLevel)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(closerToTheRightEdgeHiddenBox)) :
                new LayoutPositionOtherSubDocumentCreator(documentLayout, subDocumen, logPosition, pageIndex, detailsLevel)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(closerToTheRightEdgeHiddenBox)));
        }

        public updateRowInfo() {
            var rows: LayoutRow[] = this.result.column.rows;
            const rowIndex: number = Utils.normedBinaryIndexOf(rows, (r: LayoutRow) => r.columnOffset - this.position);
            const row: LayoutRow = rows[rowIndex];
            [this.result.row, this.result.rowIndex] = LayoutPositionCreator.conflictResolver(this.position, this.endRowConflictFlags, rows, row, rowIndex,
                (obj: LayoutRow) => obj.columnOffset, (obj: LayoutRow) => obj.getEndPosition());

            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Row) {
                this.position = Math.max(0, this.position - this.result.row.columnOffset);
                this.updateBoxInfo();
            }
        }

        public updateBoxInfo() {
            const boxes: LayoutBox[] = this.result.row.boxes;
            const boxIndex: number = Math.max(0, Utils.normedBinaryIndexOf(boxes, (b: LayoutBox) => b.rowOffset - this.position));
            const box: LayoutBox = boxes[boxIndex];

            [this.result.box, this.result.boxIndex] = LayoutPositionCreator.conflictResolver(this.position, this.middleRowConflictFlags, boxes, box, boxIndex,
                (obj: LayoutBox) => obj.rowOffset, (obj: LayoutBox) => obj.getEndPosition());

            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Box) {
                this.position = Math.max(0, this.position - this.result.box.rowOffset);
                const boxLength: number = this.result.box.getLength();
                this.result.charOffset = this.position < boxLength ? this.position : boxLength;
            }
        }

        protected static conflictResolver<T>(position: number, conflictFlags: LayoutPositionCreatorConflictFlags, objects: T[], object: T, objectIndex: number,
            getObjectStartPos: (obj: T) => number, getObjectEndPos: (obj: T) => number): [T, number]{
            const prevObject: T = objects[objectIndex - 1];
            const objectStartPos: number = getObjectStartPos(object);
            if (prevObject && objectStartPos == position) {
                const prevObjectEndPos: number = getObjectEndPos(prevObject);
                if (objectStartPos == prevObjectEndPos)
                    return conflictFlags.simple ? [prevObject, objectIndex - 1] : [object, objectIndex];
                else
                    return conflictFlags.right ? [prevObject, objectIndex - 1] : [object, objectIndex];
            }

            const nextObject: T = objects[objectIndex + 1];
            if (nextObject) {
                const objectEndPos: number = getObjectEndPos(object);
                if (position >= objectEndPos) {
                    const nextObjectStartPos: number = getObjectStartPos(nextObject);
                    if (objectEndPos == nextObjectStartPos)
                        return conflictFlags.simple ? [object, objectIndex] : [nextObject, objectIndex + 1];
                    if (position == objectEndPos)
                        return conflictFlags.left ? [object, objectIndex] : [nextObject, objectIndex + 1];
                    return conflictFlags.middle ? [object, objectIndex] : [nextObject, objectIndex + 1];
                }
            }
            return [object, objectIndex];
        }
    }

    export class LayoutPositionMainSubDocumentCreator extends LayoutPositionCreator {
        constructor(documentLayout: DocumentLayout, subDocument: SubDocument, logPosition: number, detailsLevel: DocumentLayoutDetailsLevel) {
            super(documentLayout, subDocument, logPosition, detailsLevel);

            if (!this.subDocument.isMain())
                throw new Error("LayoutPositionMainSubDocumentCreator need set here main sub document");
        }

        public create(endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags) {
            super.create(endRowConflictFlags, middleRowConflictFlags);
            this.updatePageInfo();
            return this.result;
        }

        public static ensureLayoutPosition(formatterController: IFormatterController, documentLayout: DocumentLayout, subDocument: SubDocument, logPosition: number,
            detailsLevel: DocumentLayoutDetailsLevel, endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags): LayoutPosition {
            while (true) {
                var layoutPosition: LayoutPosition = new LayoutPositionMainSubDocumentCreator(documentLayout, subDocument, logPosition, detailsLevel)
                    .create(endRowConflictFlags, middleRowConflictFlags);
                if (layoutPosition)
                    break;
                formatterController.forceFormatPage(documentLayout.validPageCount);
            }
            return layoutPosition;
        }

        private updatePageInfoInterval(validPageCount: number, pages: LayoutPage[], foundPage: LayoutPage): LayoutPage {
            // http://workservices01/OpenWiki/ow.asp?ASPxRichEdit_ChangesDescription#A2
            if (foundPage.index == 0 && this.position <= foundPage.contentIntervals[0].start)
                return validPageCount > 0 ? foundPage : null;

            const intervalsList: PageIndexAndInterval[] = [];
            for (let page of [pages[foundPage.index - 1], foundPage, pages[foundPage.index + 1]]) {
                if (page && page.index < validPageCount) {
                    for (let interval of page.contentIntervals) {
                        if (interval.containsPositionWithoutIntervalEndAndStart(this.position))
                            return page;
                        intervalsList.push(new PageIndexAndInterval(page.index, interval));
                    }
                }
            }
            if (!intervalsList.length)
                return null;
            intervalsList.sort((a, b) => a.interval.start - b.interval.start);

            let currInfo: PageIndexAndInterval = intervalsList[0];
            let nextInfo: PageIndexAndInterval;
            for (let nextInfoIndex: number = 1; nextInfo = intervalsList[nextInfoIndex]; nextInfoIndex++) {
                if (FixedInterval.fromPositions(currInfo.interval.end(), nextInfo.interval.start).contains(this.position))
                    break;
                currInfo = nextInfo;
            }

            const firstPageIndex: number = currInfo.pageIndex;
            const secondPageIndex: number = nextInfo ? nextInfo.pageIndex : firstPageIndex + 1;
            if (firstPageIndex == secondPageIndex)
                return pages[firstPageIndex];

            if (secondPageIndex >= validPageCount) {
                if (this.layout.isFullyFormatted)
                    return pages[firstPageIndex];
                else
                    return this.endRowConflictFlags.allIsTrue() ? pages[firstPageIndex] : null;
            }

            return this.getPageResolvedFlag(currInfo.interval.end(), nextInfo.interval.start) ? pages[firstPageIndex] : pages[secondPageIndex];
        }

        private getPageResolvedFlag(posA: number, posB: number): boolean {
            return posA == posB ? this.endRowConflictFlags.simple :
                (posA == this.position ? this.endRowConflictFlags.left :
                    (posB == this.position ? this.endRowConflictFlags.right :
                        this.endRowConflictFlags.middle));
        }

        private updatePageInfo() {
            var validPageCount: number = this.layout.validPageCount;
            var pages: LayoutPage[] = this.layout.pages;
            var foundPageIndex: number = Math.max(0, Utils.normedBinaryIndexOf(pages, (p: LayoutPage) => p.getStartOffsetContentOfMainSubDocument() - this.position, 0, validPageCount - 1));
            var foundPage: LayoutPage = pages[foundPageIndex];

            if (foundPage)
                this.result.page = this.updatePageInfoInterval(validPageCount, pages, foundPage);

            if (!this.result.page) {
                this.result = null;
                return;
            }

            this.result.pageIndex = this.result.page.index;

            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Page) {
                this.position = Math.max(0, this.position - this.result.page.getStartOffsetContentOfMainSubDocument());
                this.updatePageAreaInfo();
            }
        }

        private updatePageAreaInfo() {
            var areas: LayoutPageArea[] = this.result.page.mainSubDocumentPageAreas;
            if (areas.length > 1) {
                const pageAreaIndex: number = Math.max(0, Utils.normedBinaryIndexOf(areas, (a: LayoutPageArea) => a.pageOffset - this.position));
                const pageArea: LayoutPageArea = areas[pageAreaIndex];
                [this.result.pageArea, this.result.pageAreaIndex] = LayoutPositionCreator.conflictResolver(this.position, this.endRowConflictFlags, areas, pageArea, pageAreaIndex,
                    (obj: LayoutPageArea) => obj.pageOffset, (obj: LayoutPageArea) => obj.getEndPosition());
            }
            else {
                this.result.pageAreaIndex = 0;
                this.result.pageArea = areas[0];
            }
            

            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.PageArea) {
                this.position = Math.max(0, this.position - this.result.pageArea.pageOffset);
                this.updateColumnInfo();
            }
        }

        private updateColumnInfo() {
            var columns: LayoutColumn[] = this.result.pageArea.columns;
            if (columns.length > 1) {
                const columnIndex: number = Utils.normedBinaryIndexOf(columns, (c: LayoutColumn) => c.pageAreaOffset - this.position);
                const column: LayoutColumn = columns[columnIndex];
                [this.result.column, this.result.columnIndex] = LayoutPositionCreator.conflictResolver(this.position, this.endRowConflictFlags, columns, column, columnIndex,
                    (obj: LayoutColumn) => obj.pageAreaOffset, (obj: LayoutColumn) => obj.getEndPosition());
            }
            else {
                this.result.columnIndex = 0;
                this.result.column = columns[0];
            }

            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Column) {
                this.position = Math.max(0, this.position - this.result.column.pageAreaOffset);
                this.updateRowInfo();
            }
        }
    }

    export class LayoutPositionOtherSubDocumentCreator extends LayoutPositionCreator {
        constructor(documentLayout: DocumentLayout, subDocument: SubDocument, logPosition: number, pageIndex: number, detailsLevel: DocumentLayoutDetailsLevel) {
            super(documentLayout, subDocument, logPosition, detailsLevel);

            if (this.subDocument.isMain())
                throw new Error("LayoutPositionMainSubDocumentCreator need set here not main sub document");

            this.result.page = this.layout.pages[pageIndex];
            this.result.pageIndex = pageIndex;
        }

        public create(endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags) {
            super.create(endRowConflictFlags, middleRowConflictFlags);

            this.result.pageArea = this.result.page.otherPageAreas[this.subDocument.id];
            if(!this.result.pageArea) {
                this.result = null;
            }
            else {
                this.result.pageAreaIndex = 0;
                this.position = Math.max(0, this.position - this.result.pageArea.pageOffset); // here always 0, but "just in case"
                this.result.column = this.result.pageArea.columns[0];
                this.result.columnIndex = 0;
                this.position = Math.max(0, this.position - this.result.column.pageAreaOffset); // here can be number > 0
                this.updateRowInfo();
            }
            return this.result;
        }
    }
}