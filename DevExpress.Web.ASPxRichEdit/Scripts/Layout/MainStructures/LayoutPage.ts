module __aspxRichEdit {
    export class LayoutPageHeaderFooterPageAreas {
        headerPageArea: LayoutPageArea;
        footerPageArea: LayoutPageArea;

        constructor(headerPageArea: LayoutPageArea, footerPageArea: LayoutPageArea) {
            this.headerPageArea = headerPageArea;
            this.footerPageArea = footerPageArea;
        }
    }

    export class LayoutPage extends LayoutItemBase {
        public mainSubDocumentPageAreas: LayoutPageArea[] = [];
        public otherPageAreas: { [subDocumentId: number]: LayoutPageArea } = {};

        // For tables. When one cell on several pages
        public contentIntervals: FixedInterval[] = [];
        public firstPageInGroup: LayoutPage; // don't initialize
        public isNeedFullyRender: boolean = false; // old flag isComplete.
        public index: number;
        public isInnerContentValid: boolean = false; //true - the inner content (areas, columns, rows) can be reused, even if isContentValid = false. false - the inner content can't be reused.

        public getEndPosition(subDocument: SubDocument): number {
            if (subDocument.isMain())
                return this.getStartOffsetContentOfMainSubDocument() + this.mainSubDocumentPageAreas[this.mainSubDocumentPageAreas.length - 1].getEndPosition();
            else
                return this.otherPageAreas[subDocument.id].getEndPosition();
        }

        advanceContentIntervals(length: number) {
            for (let interval of this.contentIntervals)
                interval.start += length;
        }

        public getHeaderFooterPageAreas(): LayoutPageHeaderFooterPageAreas {
            let headerPageArea: LayoutPageArea;
            let footerPageArea: LayoutPageArea;
            for (let key in this.otherPageAreas) {
                if (!this.otherPageAreas.hasOwnProperty(key))
                    continue;
                const pageArea: LayoutPageArea = this.otherPageAreas[key];
                if (pageArea.subDocument.isHeader())
                    headerPageArea = pageArea;
                else if (pageArea.subDocument.isFooter())
                    footerPageArea = pageArea;
            }
            return new LayoutPageHeaderFooterPageAreas(headerPageArea, footerPageArea);
        }

        getLastContentInterval(): FixedInterval {
            return this.contentIntervals[this.contentIntervals.length - 1];
        }

        public getLastMainPageArea(): LayoutPageArea {
            return this.mainSubDocumentPageAreas[this.mainSubDocumentPageAreas.length - 1];
        }

        //public getValidContentLength(): number {
        //    let validPageCntentLength: number = 0;
        //    if (this.isInnerContentValid) {
        //        for (let subDocId in this.otherPageAreas) {
        //            if (!this.otherPageAreas.hasOwnProperty(subDocId))
        //                continue;
        //            const pageArea: LayoutPageArea = this.otherPageAreas[subDocId];
        //            for (let column of pageArea.columns) {
        //                for (let row of column.rows) {
        //                    for (let box of row.boxes) {
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    this.mainSubDocumentPageAreas
        //}

        public getFirstPageInGroup() {
            return this.firstPageInGroup ? this.firstPageInGroup : this;
        }

        public getLastPageInGroup(pages: LayoutPage[]): LayoutPage {
            if (!this.firstPageInGroup)
                return this;

            for (let pageIndex: number = this.index + 1, page: LayoutPage; page = pages[pageIndex]; pageIndex++) {
                if (!page.firstPageInGroup || page.firstPageInGroup != this.firstPageInGroup)
                    return pages[pageIndex - 1];
            }
            return pages[pages.length - 1];
        }

        public getStartOffsetContentOfMainSubDocument(): number {
            return this.contentIntervals[0].start;
        }

        // BE CAREFUL. Here need store orderly!!!

        public addPageArea(pageArea: LayoutPageArea) {
            if (pageArea.subDocument.isMain()) {
                const pageOffset: number = pageArea.pageOffset;
                const subDocsCount: number = this.mainSubDocumentPageAreas.length;
                if (subDocsCount > 0 && pageOffset > this.mainSubDocumentPageAreas[subDocsCount - 1].pageOffset)
                    this.mainSubDocumentPageAreas.push(pageArea);
                else {
                    const pageAreaIndex: number = Math.max(0, Utils.normedBinaryIndexOf(this.mainSubDocumentPageAreas, (a: LayoutPageArea) => a.pageOffset - pageOffset));
                    this.mainSubDocumentPageAreas.splice(pageAreaIndex, 0, pageArea);
                }
            }
            else
                this.otherPageAreas[pageArea.subDocument.id] = pageArea;
        }

        public removePageArea(pageArea: LayoutPageArea) {
            if (pageArea.subDocument.isMain())
                this.deletePageAreaFromList(this.mainSubDocumentPageAreas, pageArea);
            else
                delete this.otherPageAreas[pageArea.subDocument.id];
        }

        private deletePageAreaFromList(list: LayoutPageArea[], pageArea: LayoutPageArea) {
            for (let i = 0, area: LayoutPageArea; area = list[i]; i++)
                if (area == pageArea) {
                    list.splice(i, 1);
                    break;
                }
        }

        // if need, then write function removeLastAddedPageArea, etc
    }
} 