module __aspxRichEdit {
    export class LayoutPageAreaBoundsCalculator {
        // all members here in pixels. All out sizes too
        private section: Section; // used ONLY in init function
        private equalWidthColumns: boolean;
        private columnCount: number;
        private columnsInfo: SectionColumnProperties[];
        private space: number;
        private marginTop: number;
        private marginRight: number;
        private marginBottom: number;
        private marginLeft: number;
        private pageHeight: number;
        private pageWidth: number;
        private headerOffset: number;
        private footerOffset: number;

        avaliablePageHeight: number;
        availablePageWidth: number;
        availableHeaderFooterWidth: number;

        headerPageAreaBounds: Rectangle;
        footerPageAreaBounds: Rectangle;

        mainPageAreasBounds: Rectangle[];
        mainColumnsBounds: Rectangle[][];
        pageBounds: Rectangle;
        headerColumnBounds: Rectangle;
        footerColumnBounds: Rectangle;

        public init(unitConverter: IUnitConverter, section: Section) {
            this.section = section;
            const sectionProperties: SectionProperties = this.section.sectionProperties;

            this.equalWidthColumns = sectionProperties.equalWidthColumns
            this.columnCount = sectionProperties.columnCount;
            this.space = unitConverter.toPixels(sectionProperties.space);
            this.pageHeight = unitConverter.toPixels(sectionProperties.pageHeight);

            this.marginTop = unitConverter.toPixels(sectionProperties.marginTop);
            this.marginRight = unitConverter.toPixels(sectionProperties.marginRight);
            this.marginBottom = unitConverter.toPixels(sectionProperties.marginBottom);
            this.marginLeft = unitConverter.toPixels(sectionProperties.marginLeft);
            this.pageWidth = unitConverter.toPixels(sectionProperties.pageWidth);
            this.pageHeight = unitConverter.toPixels(sectionProperties.pageHeight);
            this.headerOffset = unitConverter.toPixels(sectionProperties.headerOffset);
            this.footerOffset = unitConverter.toPixels(sectionProperties.footerOffset);
            
            this.columnsInfo = [];
            for (let info of sectionProperties.columnsInfo) {
                const cloneInfo = info.clone();
                cloneInfo.applyConverter(unitConverter);
                this.columnsInfo.push(cloneInfo);
            }

            this.avaliablePageHeight = this.pageHeight - (this.marginTop + this.marginBottom);
            this.availableHeaderFooterWidth = this.pageWidth - this.marginLeft - this.marginRight;
            this.availablePageWidth = this.availableHeaderFooterWidth - this.space * (this.columnCount - 1)
        }

        initWhenPageStart() {
            this.mainPageAreasBounds = [];
            this.mainColumnsBounds = [];
        }

        // 0 for set no header bounds. -1 for initial set. > 0 for final set
        public setHeaderBounds(currHeight: number) {
            if (currHeight == 0) {
                this.headerPageAreaBounds = null;
                this.headerColumnBounds = null;
                return;
            }

            const height: number = currHeight == -1 ? Number.MAX_VALUE :
                Math.max(this.marginTop - this.headerOffset, Math.min(Math.round(this.pageHeight / 3) - this.headerOffset, currHeight));
            this.headerPageAreaBounds = new Rectangle().init(this.marginLeft, this.headerOffset, this.availableHeaderFooterWidth, height);
            this.headerColumnBounds = new Rectangle().init(0, 0, this.headerPageAreaBounds.width, this.headerPageAreaBounds.height);
        }

        // 0 for set no footer bounds. -1 for initial set. > 0 for final set
        public setFooterBounds(currHeight: number) {
            if (currHeight == 0) {
                this.footerPageAreaBounds = null;
                this.footerColumnBounds = null;
                return;
            }

            if (currHeight == -1)
                this.footerPageAreaBounds = new Rectangle().init(this.marginLeft, this.pageHeight - this.footerOffset, this.availableHeaderFooterWidth, Number.MAX_VALUE);
            else {
                const height: number = Math.max(this.marginBottom - this.footerOffset, Math.min(Math.round(this.pageHeight / 3) - this.footerOffset, currHeight));
                this.footerPageAreaBounds = new Rectangle().init(this.marginLeft, this.pageHeight - height - this.footerOffset, this.availableHeaderFooterWidth, height);
            }
            
            this.footerColumnBounds = new Rectangle().init(0, 0, this.footerPageAreaBounds.width, this.footerPageAreaBounds.height);
        }

        public calculateMainPageAreaBounds(previousMainPageAreaHeight: number) {
            let y: number;
            if (previousMainPageAreaHeight > 0) {
                const previousPageAreaBounds = this.mainPageAreasBounds[this.mainPageAreasBounds.length - 1];
                for(let colBound of this.mainColumnsBounds[this.mainColumnsBounds.length - 1])
                    colBound.height = previousMainPageAreaHeight;
                previousPageAreaBounds.height = previousMainPageAreaHeight;
                y = previousPageAreaBounds.getBottomBoundPosition();
            }
            else
                y = Math.max(this.marginTop, this.headerPageAreaBounds ? this.headerPageAreaBounds.getBottomBoundPosition() : 0);
            let height: number = Math.min(this.pageHeight - this.marginBottom, this.footerPageAreaBounds ? this.footerPageAreaBounds.y : Number.MAX_VALUE) - y;
            this.mainPageAreasBounds.push(new Rectangle().init(this.marginLeft, y, this.availableHeaderFooterWidth, height));
        }
        
        public calculateColumnBounds(pageAreaBounds: Rectangle) {
            const currColBounds: Rectangle[] = [];
            this.mainColumnsBounds.push(currColBounds);

            if (this.equalWidthColumns) {
                const oneColumnWidth: number = Math.ceil(this.availablePageWidth / this.columnCount);
                const colWidthPlusSpace: number = oneColumnWidth + this.space;
                for (let colInd = 0; colInd < this.columnCount; colInd++)
                    currColBounds.push(new Rectangle().init(colWidthPlusSpace * colInd, 0, oneColumnWidth, pageAreaBounds.height));
            }
            else {
                let currXPos: number = 0;
                for (let colInd = 0; colInd < this.columnCount; colInd++) {
                    const currColumnWidth: number = this.columnsInfo[colInd].width;
                    currColBounds.push(new Rectangle().init(currXPos, 0, Math.max(1, currColumnWidth), pageAreaBounds.height));
                    currXPos += currColumnWidth;
                }
            }
        }

        public calculatePageBounds(y: number) {
            this.pageBounds = new Rectangle().init(0, y, this.pageWidth, this.pageHeight);
        }
    }
} 