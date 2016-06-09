module __aspxRichEdit {
    export class LayoutFormatterMain extends LayoutFormatterBase implements ILayoutFormatter {
        public layout: DocumentLayout;
        private boundsCalculator: LayoutPageAreaBoundsCalculator;
        
        protected layoutFormatterPositionSaver: LayoutFormatterPositionSaver;
        
        public pageChanges: PageChange[] = [];
        protected section: Section; // current
        
        protected onLayoutChanged: EventDispatcher<ILayoutChangesListener> = new EventDispatcher<ILayoutChangesListener>();

        constructor(layout: DocumentLayout, iterator: ITextBoxIterator, unitConverter: IUnitConverter) {
            super(iterator, unitConverter);
            this.layout = layout;
            this.layoutFormatterPositionSaver = new LayoutFormatterPositionSaver(this);
            this.boundsCalculator = new LayoutPageAreaBoundsCalculator();

            this.state = LayoutFormatterState.DocumentStart;

            this.stateMap[LayoutFormatterState.DocumentStart] = this.processStateDocumentStart;
            this.stateMap[LayoutFormatterState.PageStart] = this.processStatePageStart;
            this.stateMap[LayoutFormatterState.PageAreaEnd] = this.processStatePageAreaEnd;
            this.stateMap[LayoutFormatterState.PageEnd] = this.processStatePageEnd;
            this.stateMap[LayoutFormatterState.DocumentEnd] = this.processStateDocumentEnd;
        }
        
        public formatNext(): boolean {
            if (this.isUpdateLocked())
                throw new Error("isUpdateLocked(). You can't call formatNext");
            return this.stateMap[this.state].call(this);
        }

        public formatPage(index: number): LayoutPage {
            //Logging.print(LogSource.LayoutFormatter, "formatPage", ["SubDocId", this.iterator.subDocument.id], ["index", index]);
            while (index >= this.layout.validPageCount && this.formatNext());
            //Logging.print(LogSource.LayoutFormatter, "formatPage(end)", ["ValidPageCount", this.layout.validPageCount]);
            return this.layout.pages[index];
        }

        public formatHeaderPageArea(headerSubDocumentInfo: HeaderSubDocumentInfo): FormatPageAreaResult {
            if(!headerSubDocumentInfo) {
                this.boundsCalculator.setHeaderBounds(0);
                return new FormatPageAreaResult(null, []);
            }

            this.boundsCalculator.setHeaderBounds(-1);
            const headerFormatter: LayoutFormatterBase = new LayoutFormatterBase(new TextBoxIterator(this.iterator.getMeasurer(),
                headerSubDocumentInfo.getSubDocument(this.iterator.subDocument.documentModel)), this.unitConverter);

            let pageIndex = this.layoutPosition.pageIndex;
            headerFormatter.formatPageArea(this.boundsCalculator.headerPageAreaBounds, [this.boundsCalculator.headerColumnBounds], pageIndex, this.pagesCount);
            //if (headerFormatter.state != LayoutFormatterState.PageAreaEnd) {
            // need chunks to complete the area formatting
            //}

            if(this.layoutDependentOtherPageAreasCache[pageIndex]) {
                let index = this.layoutDependentOtherPageAreasCache[pageIndex].indexOf(headerSubDocumentInfo.subDocumentId);
                if(index > -1)
                    this.layoutDependentOtherPageAreasCache[pageIndex].splice(index, 1);
            }
            if(headerFormatter.layoutDependentOtherPageAreasCache[pageIndex]) {
                if(!this.layoutDependentOtherPageAreasCache.hasOwnProperty(pageIndex.toString()))
                    this.layoutDependentOtherPageAreasCache[pageIndex] = [];
                let subDocumentId = headerSubDocumentInfo.subDocumentId;
                if(this.layoutDependentOtherPageAreasCache[pageIndex].indexOf(subDocumentId) < 0)
                    this.layoutDependentOtherPageAreasCache[pageIndex].push(subDocumentId);
            }

            const headerPageArea: LayoutPageArea = headerFormatter.layoutPosition.pageArea;
            if (headerPageArea) {
                this.boundsCalculator.setHeaderBounds(headerFormatter.currColumnHeight);
                const column: LayoutColumn = headerPageArea.columns[0];
                (<Rectangle>headerPageArea).copyFrom(this.boundsCalculator.headerPageAreaBounds);
                (<Rectangle>column).copyFrom(this.boundsCalculator.headerColumnBounds);
                this.reduceRowHeight(column);
            }
            else
                this.boundsCalculator.setHeaderBounds(0);

            return new FormatPageAreaResult(headerPageArea, headerFormatter.columnChanges);
        }
        public formatFooterPageArea(footerSubDocumentInfo: FooterSubDocumentInfo): FormatPageAreaResult {
            if(!footerSubDocumentInfo) {
                this.boundsCalculator.setFooterBounds(0);
                return new FormatPageAreaResult(null, []);
            }

            this.boundsCalculator.setFooterBounds(-1);
            const footerFormatter: LayoutFormatterBase = new LayoutFormatterBase(new TextBoxIterator(this.iterator.getMeasurer(), footerSubDocumentInfo.getSubDocument(this.iterator.subDocument.documentModel)), this.unitConverter);

            let pageIndex = this.layoutPosition.pageIndex;
            footerFormatter.formatPageArea(this.boundsCalculator.footerPageAreaBounds, [this.boundsCalculator.footerColumnBounds], pageIndex, this.pagesCount);
            //if (footerFormatter.state != LayoutFormatterState.PageAreaEnd) {
            // need chunks to complete the area formatting
            //}
            if(this.layoutDependentOtherPageAreasCache[pageIndex]) {
                let index = this.layoutDependentOtherPageAreasCache[this.layoutPosition.pageIndex].indexOf(footerSubDocumentInfo.subDocumentId);
                if(index > -1)
                    this.layoutDependentOtherPageAreasCache[this.layoutPosition.pageIndex].splice(index, 1);
            }
            if(footerFormatter.layoutDependentOtherPageAreasCache[pageIndex]) {
                if(!this.layoutDependentOtherPageAreasCache.hasOwnProperty(pageIndex.toString()))
                    this.layoutDependentOtherPageAreasCache[pageIndex] = [];
                let subDocumentId = footerSubDocumentInfo.subDocumentId;
                if(this.layoutDependentOtherPageAreasCache[pageIndex].indexOf(subDocumentId) < 0) 
                    this.layoutDependentOtherPageAreasCache[pageIndex].push(subDocumentId);
            }

            const footerPageArea: LayoutPageArea = footerFormatter.layoutPosition.pageArea;
            if (footerPageArea) {
                this.boundsCalculator.setFooterBounds(footerFormatter.currColumnHeight);
                const column: LayoutColumn = footerPageArea.columns[0];
                (<Rectangle>footerPageArea).copyFrom(this.boundsCalculator.footerPageAreaBounds);
                (<Rectangle>column).copyFrom(this.boundsCalculator.footerColumnBounds);
                this.reduceRowHeight(column);
            }
            else
                this.boundsCalculator.setFooterBounds(0);

            return new FormatPageAreaResult(footerPageArea, footerFormatter.columnChanges)
        }
        private reduceRowHeight(column: LayoutColumn) {
            if (!this.iterator.subDocument.documentModel.activeSubDocument.isMain())
                return;
            const colHeight: number = column.height;
            for (let row of column.rows) {
                if (row.y >= colHeight && !row.tableCellInfo) {
                    row.y = colHeight;
                    row.height = 0;
                }
            }
        }
    
        // redefined BatchUpdatableObject
        onUpdateUnlocked(occurredEvents: number) {
            this.layoutFormatterPositionSaver.restart();
        }

        processStateDocumentStart(): boolean {
            this.layoutPosition = new LayoutPosition(DocumentLayoutDetailsLevel.None);
            this.layoutPosition.pageIndex = 0;
            this.setNextExpectedTable(0);

            this.state = LayoutFormatterState.PageStart;

            this.iterator.setPosition(0, true);
            return true;
        }
        processStatePageStart(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStatePageStart", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.boundsCalculator.initWhenPageStart();
            if (!this.lastRow || this.lastRow.flags & LayoutRowStateFlags.SectionEnd) {
                this.section = this.iterator.subDocument.documentModel.sections[this.iterator.getSectionIndex()];
                this.boundsCalculator.init(this.unitConverter, this.section);
            }
            
            const isEvenPage: boolean = Utils.isEven(this.layoutPosition.pageIndex);
            const prevPage: LayoutPage = this.layout.pages[this.layoutPosition.pageIndex - 1];
            if(this.layoutPosition.pageIndex > 0 && prevPage) {
                let prevPageStartPosition = prevPage.contentIntervals[0].start;
                let prevPageSectionIndex = Utils.normedBinaryIndexOf(this.iterator.subDocument.documentModel.sections, s => s.startLogPosition.value - prevPageStartPosition);
                this.firstPageOfSection = this.iterator.getSectionIndex() != prevPageSectionIndex;
            }
            else
                this.firstPageOfSection = true;

            this.boundsCalculator.calculatePageBounds(prevPage ? (prevPage.y + prevPage.height) : 0);

            this.pageAreaChanges = [];
            if (!this.tryReusePage(this.boundsCalculator.pageBounds)) {
                this.createNextPage(this.boundsCalculator.pageBounds);

                const headerFormatResult: FormatPageAreaResult = this.formatHeaderPageArea(this.section.headers.getActualObject(this.firstPageOfSection, isEvenPage));
                this.addAndRegisterOtherPageAreaToCurrentPage(headerFormatResult.pageArea, headerFormatResult.columnChanges);

                const footerFormatResult: FormatPageAreaResult = this.formatFooterPageArea(this.section.footers.getActualObject(this.firstPageOfSection, isEvenPage));
                this.addAndRegisterOtherPageAreaToCurrentPage(footerFormatResult.pageArea, footerFormatResult.columnChanges);

                this.boundsCalculator.calculateMainPageAreaBounds(-1);
                this.boundsCalculator.calculateColumnBounds(this.boundsCalculator.mainPageAreasBounds[this.boundsCalculator.mainPageAreasBounds.length - 1]);

                this.pageAreaBounds = this.boundsCalculator.mainPageAreasBounds[this.boundsCalculator.mainPageAreasBounds.length - 1];
                this.columnBounds = this.boundsCalculator.mainColumnsBounds[this.boundsCalculator.mainColumnsBounds.length - 1];
            }
            
            this.pageChanges.push(new PageChange(LayoutChangeType.Updated, this.layoutPosition.pageIndex, this.layoutPosition.page, this.pageAreaChanges));
            return true;
        }
        processStatePageAreaEnd(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStatePageAreaEnd", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.finalizePageArea();
            this.addAndRegisterMainPageAreaToCurrentPage(this.layoutPosition.pageArea)

            // need habdle situation with sectionEnd type Continious
            this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Page;
            this.state = LayoutFormatterState.PageEnd;
            return true;
        }
        processStatePageEnd(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStatePageEnd", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.removeRedundantPageAreas(this.layoutPosition.pageAreaIndex + 1);
            const createdPage: LayoutPage = this.layoutPosition.page;

            createdPage.isContentValid = true;
            createdPage.isInnerContentValid = true;
            createdPage.index = this.layoutPosition.pageIndex;

            this.addAndRegisterPageToLayout(createdPage);
            this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.None;
            this.layoutPosition.pageIndex++;
            this.layoutPosition.page = null;

            // normalize offsets
            if (this.iterator.subDocument.isMain()) {
                const offsetFirstPageAreaFromPage: number = createdPage.mainSubDocumentPageAreas[0].pageOffset;
                if (offsetFirstPageAreaFromPage > 0) {
                    for (let pageArea of createdPage.mainSubDocumentPageAreas)
                        pageArea.pageOffset -= offsetFirstPageAreaFromPage;
                }
            }

            this.state = this.lastRow.flags & LayoutRowStateFlags.DocumentEnd ? LayoutFormatterState.DocumentEnd : LayoutFormatterState.PageStart;

            this.layout.validPageCount = this.layoutPosition.pageIndex;
            this.layout.lastMaxNumPages = Math.max(this.layout.lastMaxNumPages, this.layout.validPageCount);
            this.pagesCount = this.layout.lastMaxNumPages;

            if (!this.disableReuse)
                this.sendAllPageChanges();

            return true;
        }
        processStateDocumentEnd(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStateDocumentEnd", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.removeRedundantPage(this.layout.validPageCount);

            this.layout.isFullyFormatted = true;
            this.layout.lastMaxNumPages = this.layout.validPageCount;
            this.pagesCount = this.layout.lastMaxNumPages;

            let currentLayoutPosition = this.layoutPosition;
            for(let pageIndex in this.layoutDependentOtherPageAreasCache) {
                pageIndex = parseInt(pageIndex);
                let layoutPage = this.layout.pages[pageIndex];
                if(layoutPage) {
                    let subDocIds = this.layoutDependentOtherPageAreasCache[pageIndex];
                    for(let i = 0, subDocId: number; subDocId = subDocIds[i]; i++) {
                        let pageArea = layoutPage.otherPageAreas[subDocId];
                        if(pageArea) {
                            this.layoutPosition.pageIndex = pageIndex;
                            this.layoutPosition.page = layoutPage;
                            this.pageChanges.push(new PageChange(LayoutChangeType.Updated, pageIndex, layoutPage, this.pageAreaChanges));
                            const formatResult: FormatPageAreaResult = pageArea.subDocument.isHeader() ?
                                this.formatHeaderPageArea(<HeaderSubDocumentInfo>pageArea.subDocument.info) :
                                this.formatFooterPageArea(<FooterSubDocumentInfo>pageArea.subDocument.info);
                            this.addAndRegisterOtherPageAreaToCurrentPage(formatResult.pageArea, formatResult.columnChanges);
                            this.sendAllPageChanges();
                        }
                    }
                }
            }
            this.layoutPosition = currentLayoutPosition;
            
            for (let pageIndex: number = 0, page: LayoutPage; page = this.layout.pages[pageIndex]; pageIndex++)
                //Logging.print(LogSource.LayoutFormatter, "processStateDocumentEnd(end)", [`Pages[${pageIndex}].ContentIntervals`, LoggingToString.list, [page.contentIntervals, LoggingToString.fixedInterval, 1]]);
            if (Log.isEnabled)
                TEST_CLASS.checkLayout(this.iterator.subDocument.documentModel, this.layout);
            return false;
        }
        
        // correctOffsets
        public static correctPageOffsets(page: LayoutPage) {
            const pageAreas: LayoutPageArea[] = page.mainSubDocumentPageAreas;
            if (!pageAreas.length)
                return;
            const offsetFirstPageAreaFromPage: number = pageAreas[0].pageOffset;
            if (offsetFirstPageAreaFromPage > 0) {
                page.contentIntervals[0].start += offsetFirstPageAreaFromPage;
                page.contentIntervals[0].length -= offsetFirstPageAreaFromPage;
                for (let pageArea of pageAreas)
                    pageArea.pageOffset -= offsetFirstPageAreaFromPage;
            }
        }

        // remove rendundant
        private removeRedundantPage(firstRendundantPageIndex: number) {
            LayoutFormatterBase.removeRendundant(firstRendundantPageIndex, this.layout.pages,
                (index: number, elem: LayoutPage) => this.pageChanges.push(new PageChange(LayoutChangeType.Deleted, index, elem, [])));
        }
        private removeRedundantPageAreas(firstRendundantPageAreaIndex: number) {
            const pageAreas: LayoutPageArea[] = this.layoutPosition.page.mainSubDocumentPageAreas;
            for (let i: number = pageAreas.length - 1; i >= firstRendundantPageAreaIndex; i--)
                pageAreas.pop();
        }

        //try reuse
        //true mean reuse possible.
        // false - need create new object
        private tryReusePage(pageBounds: Rectangle): boolean {
            const page: LayoutPage = this.layout.pages[this.layoutPosition.pageIndex];
            if (!page || !page.isInnerContentValid || !(<Rectangle>page).equals(pageBounds))
                return false;

            this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Page;
            this.layoutPosition.page = page;

            const isPositionCorrect: boolean = page.contentIntervals[0].start == this.iterator.getPosition() ||
                this.iterator.nextVisibleBoxStartPositionEqualWith(page.contentIntervals[0].start);
            if (page.isContentValid && isPositionCorrect && !this.disableReuse) {
                this.layoutPosition.pageAreaIndex = page.mainSubDocumentPageAreas.length - 1;
                const pageArea: LayoutPageArea = page.getLastMainPageArea();
                const column: LayoutColumn = pageArea.getLastColumn();
                this.lastRow = column.getLastRow();
                this.lastRowStartPosition = page.getStartOffsetContentOfMainSubDocument() + pageArea.pageOffset + column.pageAreaOffset + this.lastRow.columnOffset;
                this.lastRowParagraphIndex = LayoutFormatterBase.NEED_RECALCULATE_PARAGRAPH_INDEX;
                this.getLastRowParagraphIndex();

                const pageEndPos: number = page.getEndPosition(this.iterator.subDocument);
                this.iterator.setPosition(pageEndPos, false);
                this.state = LayoutFormatterState.PageEnd;
            }
            else {
                page.contentIntervals = [new FixedInterval(page.getStartOffsetContentOfMainSubDocument(), 0)];
                this.layoutPosition.pageAreaIndex = 0;
                this.state = LayoutFormatterState.PageAreaStart;
            }
            return true; 
        }

        // registry
        addAndRegisterPageToLayout(page: LayoutPage) {
            page.isContentValid = true;
            page.isInnerContentValid = true;
            const pageIndex: number = this.layoutPosition.pageIndex;
            const existingPage: LayoutPage = this.layout.pages[pageIndex];
            let changeType: LayoutChangeType;
            if (!existingPage) {
                this.layout.pages.push(page);
                changeType = LayoutChangeType.Added;
            }
            else {
                if (existingPage == page)
                    changeType = LayoutChangeType.Updated;
                else {
                    changeType = LayoutChangeType.Replaced;
                    this.layout.pages[pageIndex] = page;
                }
            }
            this.pageChanges[this.pageChanges.length - 1].changeType = changeType;
        }
        addAndRegisterMainPageAreaToCurrentPage(pageArea: LayoutPageArea) {
            pageArea.isContentValid = true;
            const pageAreaIndex: number = this.layoutPosition.pageAreaIndex;
            const existingPageArea: LayoutPageArea = this.layoutPosition.page.mainSubDocumentPageAreas[pageAreaIndex];
            let changeType: LayoutChangeType;
            
            if (!existingPageArea) {
                this.layoutPosition.page.addPageArea(pageArea);
                changeType = LayoutChangeType.Added;
            }
            else if (existingPageArea == pageArea)
                changeType = LayoutChangeType.Updated;
            else {
                changeType = LayoutChangeType.Replaced;
                this.layoutPosition.page.mainSubDocumentPageAreas[pageAreaIndex] = pageArea;
            }
            this.pageAreaChanges[this.pageAreaChanges.length - 1].changeType = changeType;
        }
        addAndRegisterOtherPageAreaToCurrentPage(pageArea: LayoutPageArea, columnChanges: ColumnChange[]) {
            if (!pageArea)
                return;
            pageArea.isContentValid = true;
            const subDocId: number = pageArea.subDocument.id;
            const existingPageArea: LayoutPageArea = this.layoutPosition.page.otherPageAreas[subDocId];
            let changeType: LayoutChangeType;

            if (!existingPageArea) {
                if (pageArea.subDocument.isHeaderFooter()) {
                    const isHeader: boolean = pageArea.subDocument.isHeader();
                    for (let subDocumentId in this.layoutPosition.page.otherPageAreas) {
                        const otherPageArea: LayoutPageArea = this.layoutPosition.page.otherPageAreas[subDocumentId];
                        if ((isHeader && otherPageArea.subDocument.isHeader()) || !isHeader && otherPageArea.subDocument.isFooter()) {
                            this.layoutPosition.page.removePageArea(otherPageArea);
                            
                            this.pageAreaChanges.push(new PageAreaChange(LayoutChangeType.Deleted, 0, otherPageArea, []));
                            break;
                        }
                    }
                }
                changeType = LayoutChangeType.Added;
                this.layoutPosition.page.otherPageAreas[subDocId] = pageArea;
            }
            else if (existingPageArea == pageArea)
                changeType = LayoutChangeType.Updated;
            else {
                this.layoutPosition.page.otherPageAreas[subDocId] = pageArea;
                changeType = LayoutChangeType.Replaced;
            }
            this.pageAreaChanges.push(new PageAreaChange(changeType, 0, pageArea, columnChanges));
        }

        // others
        createNextPage(pageBounds: Rectangle) {
            const prevPage: LayoutPage = this.layout.pages[this.layoutPosition.pageIndex - 1];
            const newPage: LayoutPage = new LayoutPage();
            (<Rectangle>newPage).copyFrom(pageBounds);
            newPage.contentIntervals = [];
            this.state = LayoutFormatterState.PageAreaStart;

            this.layoutPosition.page = newPage;
            this.layoutPosition.pageAreaIndex = 0;
            this.layoutPosition.pageArea = null;
            this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Page;
        }

        // IDocumentFormatter
        public addLayoutChangedListener(listener: ILayoutChangesListener): void {
            this.onLayoutChanged.add(listener);
        }
        public removeLayoutChangedListener(listener: ILayoutChangesListener): void {
            this.onLayoutChanged.remove(listener);
        }

        // notify (see ILayoutChangesListener)
        private notifyPageInvalidated(firstInvalidPageIndex: number) {
            //Logging.print(LogSource.LayoutFormatter, "notifyPageInvalidated", ["firstInvalidPageIndex", firstInvalidPageIndex]);
            this.onLayoutChanged.raise("NotifyPagesInvalidated", firstInvalidPageIndex);
        }
        public sendAllPageChanges() {
            this.pageAreaChanges = [];
            this.columnChanges = [];
            this.rowChanges = [];
            this.tableChanges = [];


            for (let change: PageChange; change = this.pageChanges.shift();) {
                if (LayoutFormatterMain.pageChangeEmptyOrInvalid(change))
                    continue;
                //Logging.print(LogSource.LayoutFormatter, "notifyPageReady(1)", ["changes", LoggingToString.pageChange, [change]]);
                if (change.layoutElement) {
                    //Logging.print(LogSource.LayoutFormatter, "notifyPageReady(2)", ["contentIntervals", LoggingToString.list, [change.layoutElement.contentIntervals, LoggingToString.fixedInterval, 1]]);
                    1; // fake;
                }
                this.onLayoutChanged.raise("NotifyPageReady", change);
            }
        }

        private static pageChangeEmptyOrInvalid(change: PageChange): boolean {

            if (change.changeType != LayoutChangeType.Updated)
                return false;
            for (let pageAreaChange of change.pageAreaChanges) {
                if (pageAreaChange.changeType != LayoutChangeType.Updated)
                    return false;
                for (let columnChange of pageAreaChange.columnChanges) {
                    if (columnChange.changeType != LayoutChangeType.Updated || columnChange.rowChanges.length != 0 ||
                        columnChange.paragraphFrameChanges.length != 0 || columnChange.tableChanges.length != 0)
                        return false;
                }
            }
            return true;
        }

        public addPageChangeDeleted(pageIndex: number) {
            this.pageChanges.push(new PageChange(LayoutChangeType.Deleted, pageIndex, null, []));
        }

        // restart
        private restartCommonPart(pageIndexFromStartFormat: number, modelPosition: number) {
            // example 1st page starts with 76 position, and modelPosition == 76. (case hidden symbols or fields)
            var pageFromStartFormat: LayoutPage = this.layout.pages[pageIndexFromStartFormat];
            // start with position end prev page
            if (modelPosition == pageFromStartFormat.getStartOffsetContentOfMainSubDocument())
                modelPosition = pageIndexFromStartFormat ? this.layout.pages[pageIndexFromStartFormat - 1].getEndPosition(this.iterator.subDocument) : 0;

            if (this.iterator.getPosition() < modelPosition) {
                this.iterator.checkTableLevelsInfo();
                return false;
            }

            this.rowFormatter.pieceTableNumberingListCountersManager.reset();
            this.iterator.setPosition(modelPosition, true);
            const firstBox: LayoutBox = this.iterator.getNextBox();

            let nextTableIndex: number = Utils.normedBinaryIndexOf(this.iterator.subDocument.tables, (t: Table) => t.getStartPosition() - modelPosition);
            if (nextTableIndex < 0)
                nextTableIndex = 0;
            else if (modelPosition >= this.iterator.subDocument.tables[nextTableIndex].getEndPosition())
                nextTableIndex++;
            this.setNextExpectedTable(nextTableIndex);

            if (firstBox)
                this.iterator.setPosition(this.iterator.getPosition() - firstBox.getLength(), false); // get pos first show box
            return true;
        }
        public restartFromRow(subDocument: SubDocument, layoutPosition: LayoutPosition, modelPosition: number) {
            //Logging.print(LogSource.LayoutFormatter, "restartFromRow", ["SubDocId", subDocument.id], ["modelPosition", modelPosition], ["layoutPosition", LoggingToString.layoutPositionShort, [layoutPosition]]);
            if (modelPosition < 0)
                throw new Error(Errors.InternalException);
            this.layout.isFullyFormatted = false;
            if (this.isUpdateLocked()) {
                this.layoutFormatterPositionSaver.savePositionRestartFromRow(subDocument, layoutPosition, modelPosition);
                return;
            }
            
            if (!subDocument.isMain()) {
                this.pageAreaChanges = [];
                this.columnChanges = [];
                this.rowChanges = [];
                this.tableChanges = [];

                this.currColumnHeight = 0;
                this.layoutPosition = layoutPosition;
                this.lastRow = null;
                this.lastRowParagraphIndex = -1;

                this.pageChanges.push(new PageChange(LayoutChangeType.Updated, this.layoutPosition.pageIndex, this.layoutPosition.page, this.pageAreaChanges));

                const formatResult: FormatPageAreaResult = subDocument.isHeader() ?
                    this.formatHeaderPageArea(<HeaderSubDocumentInfo>subDocument.info) :
                    this.formatFooterPageArea(<FooterSubDocumentInfo>subDocument.info);
                this.addAndRegisterOtherPageAreaToCurrentPage(formatResult.pageArea, formatResult.columnChanges);

                const createdPageWithHeaderFooter: LayoutPage = this.layout.pages[this.layoutPosition.pageIndex];
                createdPageWithHeaderFooter.isContentValid = true;
                createdPageWithHeaderFooter.isInnerContentValid = true;
                createdPageWithHeaderFooter.contentIntervals[0] = FixedInterval.fromPositions(createdPageWithHeaderFooter.contentIntervals[0].start,
                    createdPageWithHeaderFooter.getEndPosition(this.iterator.subDocument.documentModel.mainSubDocument));
                createdPageWithHeaderFooter.index = this.layoutPosition.pageIndex;

                this.layout.validPageCount = this.layoutPosition.pageIndex + 1;
                this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.None;

                this.sendAllPageChanges();
                return; 
            }

            let shouldInvalidateWholePage = !layoutPosition.page.isInnerContentValid;
            if(shouldInvalidateWholePage) {
                this.restartFromPage(layoutPosition.pageIndex, modelPosition);
                return;
            }
            
            if (!this.restartCommonPart(layoutPosition.pageIndex, modelPosition))
                return;
            
            const lp: LayoutPosition = layoutPosition.clone();
            if (lp.advanceToPrevRow(this.layout)) {
                this.lastRowStartPosition = lp.getLogPosition(DocumentLayoutDetailsLevel.Row);
                this.lastRow = lp.row;
                this.lastRowParagraphIndex = LayoutFormatterBase.NEED_RECALCULATE_PARAGRAPH_INDEX;
                this.getLastRowParagraphIndex();

                if (layoutPosition.columnIndex == lp.columnIndex && layoutPosition.pageAreaIndex == lp.pageAreaIndex && layoutPosition.pageIndex == lp.pageIndex) {
                    const cellInfo: LayoutTableCellInfo = this.lastRow.tableCellInfo;
                    this.currColumnHeight = cellInfo ? cellInfo.bound.getBottomBoundPosition() : this.lastRow.getBottomBoundPosition();
                }
                else
                    this.currColumnHeight = 0;
            }
            else {
                this.currColumnHeight = 0;
                this.lastRow = null;
                this.lastRowParagraphIndex = -1;
            }

            this.currTableColumnInfoIndex = 0;
            const layoutTableColumnInfo: LayoutTableColumnInfo[] = layoutPosition.column.tablesInfo;
            for (let tblInfo: LayoutTableColumnInfo;
                (tblInfo = layoutTableColumnInfo[this.currTableColumnInfoIndex]) && tblInfo.logicInfo.grid.table.index < this.nextTableIndex;
                this.currTableColumnInfoIndex++);
            this.correctPageIntervals(layoutPosition, modelPosition);
            
            this.layoutPosition = layoutPosition;

            const sections: Section[] = this.iterator.subDocument.documentModel.sections;
            this.section = sections[Utils.normedBinaryIndexOf(sections, (s: Section) => s.startLogPosition.value - modelPosition)];

            this.boundsCalculator.init(this.unitConverter, this.section);
            const prevPage: LayoutPage = this.layout.pages[this.layoutPosition.pageIndex - 1];
            this.boundsCalculator.calculatePageBounds(prevPage ? (prevPage.y + prevPage.height) : 0);

            const headerFooterPageAreas: LayoutPageHeaderFooterPageAreas = this.layoutPosition.page.getHeaderFooterPageAreas();
            if (headerFooterPageAreas.headerPageArea)
                this.boundsCalculator.setHeaderBounds(headerFooterPageAreas.headerPageArea.columns[0].height);
            if (headerFooterPageAreas.footerPageArea)
                this.boundsCalculator.setFooterBounds(headerFooterPageAreas.footerPageArea.columns[0].height);

            this.boundsCalculator.calculateMainPageAreaBounds(-1);
            this.boundsCalculator.calculateColumnBounds(this.boundsCalculator.mainPageAreasBounds[this.boundsCalculator.mainPageAreasBounds.length - 1]);
            this.pageAreaBounds = this.boundsCalculator.mainPageAreasBounds[this.boundsCalculator.mainPageAreasBounds.length - 1];
            this.columnBounds = this.boundsCalculator.mainColumnsBounds[this.boundsCalculator.mainColumnsBounds.length - 1];

            this.state = LayoutFormatterState.RowFormatting;
            //Logging.print(LogSource.LayoutFormatter, "restartFromRow(end)", ["iterator.getPosition()", this.iterator.getPosition()], ["layoutPosition", LoggingToString.layoutPositionShort, [layoutPosition]], ["currTableColumnInfoIndex", this.currTableColumnInfoIndex]);

            this.pageAreaChanges = [];
            this.columnChanges = [];
            this.rowChanges = [];
            this.tableChanges = [];
            this.pageChanges.push(new PageChange(LayoutChangeType.Updated, this.layoutPosition.pageIndex, this.layoutPosition.page, this.pageAreaChanges));
            this.pageAreaChanges.push(new PageAreaChange(LayoutChangeType.Updated, this.layoutPosition.pageAreaIndex, this.layoutPosition.pageArea, this.columnChanges));
            this.columnChanges.push(new ColumnChange(LayoutChangeType.Updated, this.layoutPosition.columnIndex, this.layoutPosition.column, this.rowChanges, this.tableChanges, []));

            this.notifyPageInvalidated(this.layoutPosition.pageIndex);
        }
        
        private correctPageIntervals(layoutPosition: LayoutPosition, modelPosition: number) {
            const columns: LayoutColumn[] = layoutPosition.pageArea.columns;
            const pageContentIntervals: FixedInterval[] = layoutPosition.page.contentIntervals;
            const firstPagePos: number = pageContentIntervals[0].start;
            for (let contentIntervalIndex: number = 0, interval: FixedInterval; interval = pageContentIntervals[contentIntervalIndex]; contentIntervalIndex++) {
                if (modelPosition <= interval.start) {
                    pageContentIntervals.splice(contentIntervalIndex);
                    for (let colIndex: number = layoutPosition.columnIndex + 1, column: LayoutColumn; column = columns[colIndex]; colIndex++)
                        column.isContentValid = false;
                    break;
                }
                if (interval.containsPositionWithoutIntervalEnd(modelPosition)) {
                    interval.length = modelPosition - interval.start;
                    pageContentIntervals.splice(contentIntervalIndex + 1);
                    for (let colIndex: number = layoutPosition.columnIndex + 1, column: LayoutColumn; column = columns[colIndex]; colIndex++)
                        column.isContentValid = false;
                    break;
                }
            }
            if (!pageContentIntervals.length)
                pageContentIntervals.push(new FixedInterval(firstPagePos, 0))
        }

        // modelPosition - this must be start section
        public restartFromPage(pageIndex: number, modelPosition: number) {
            //Logging.print(LogSource.LayoutFormatter, "restartFromPage", ["pageIndex", pageIndex], ["modelPosition", modelPosition]);
            this.layout.isFullyFormatted = false;
            if (this.isUpdateLocked()) {
                this.layoutFormatterPositionSaver.savePositionRestartFromPage(pageIndex, modelPosition);
                return;
            }

            if (!this.restartCommonPart(pageIndex, modelPosition))
                return;

            this.layoutPosition = new LayoutPosition(DocumentLayoutDetailsLevel.None);
            this.layoutPosition.pageIndex = pageIndex;

            const sections: Section[] = this.iterator.subDocument.documentModel.sections;
            const sectionIndex: number = Utils.normedBinaryIndexOf(sections, (s: Section) => s.startLogPosition.value - modelPosition);
            this.section = sections[sectionIndex];
            
            this.state = LayoutFormatterState.PageStart;

            const prevPage: LayoutPage = this.layout.pages[pageIndex - 1];
            if (!prevPage) {
                this.lastRow = null;
                this.lastRowParagraphIndex = -1;
            }
            else {
                const pageArea: LayoutPageArea = prevPage.getLastMainPageArea();
                const column: LayoutColumn = pageArea.getLastColumn();
                this.lastRow = column.getLastRow();
                this.lastRowStartPosition = prevPage.getStartOffsetContentOfMainSubDocument() + pageArea.pageOffset + column.pageAreaOffset + this.lastRow.columnOffset;
                this.lastRowParagraphIndex = LayoutFormatterBase.NEED_RECALCULATE_PARAGRAPH_INDEX;
                this.getLastRowParagraphIndex();
            }

            //Logging.print(LogSource.LayoutFormatter, "restartFromPage(end)", ["iterator.getPosition()", this.iterator.getPosition()], ["layoutPosition", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.notifyPageInvalidated(this.layoutPosition.pageIndex);
        }
        public restartFormatingAllLayout() {
            //Logging.print(LogSource.LayoutFormatter, "restartFormatingAllLayout");
            for (let page: LayoutPage; page = this.layout.pages.shift();)
                this.addPageChangeDeleted(0);
            
            this.sendAllPageChanges();

            const subDocument: SubDocument = this.iterator.subDocument;
            this.layout.setEmptyLayout(subDocument.documentModel.pageBackColor, subDocument.documentModel.showTableGridLines);
            this.iterator.setPosition(0, true);
            this.state = LayoutFormatterState.DocumentStart;
            this.rowFormatter.pieceTableNumberingListCountersManager.reset();
            this.setNextExpectedTable(0);
            this.lastRow = null;
            this.lastRowParagraphIndex = -1;

            this.notifyPageInvalidated(0);
        }
    }

    // store position what need restart
    export class LayoutFormatterPositionSaver {
        private layoutFormatterMain: LayoutFormatterMain;

        private subDocument: SubDocument;
        private modelPosition: number;
        private layoutPosition: LayoutPosition;
        
        constructor(layoutFormatterMain: LayoutFormatterMain) {
            this.layoutFormatterMain = layoutFormatterMain;
            this.subDocument = null;
            this.layoutPosition = null;
            this.modelPosition = -1;
        }

        public savePositionRestartFromRow(subDocument: SubDocument, layoutPosition: LayoutPosition, modelPosition: number) {
            let compareWithPos: number = this.modelPosition < 0 ? this.layoutFormatterMain.iterator.getPosition() : this.modelPosition;
            if (modelPosition < compareWithPos || modelPosition == compareWithPos && (!this.layoutPosition || layoutPosition.detailsLevel < this.layoutPosition.detailsLevel)) {
                this.subDocument = subDocument;
                this.modelPosition = modelPosition;
                this.layoutPosition = layoutPosition;
                this.layoutFormatterMain.layout.validPageCount = layoutPosition.pageIndex;
            }
        }

        public savePositionRestartFromPage(pageIndex: number, modelPosition: number) {
            const lp: LayoutPosition = new LayoutPosition(DocumentLayoutDetailsLevel.None);
            lp.pageIndex = pageIndex;
            this.savePositionRestartFromRow(null, lp, modelPosition);
        }

        public restart() {
            if (this.modelPosition < 0)
                return;
            
            if (this.layoutPosition.detailsLevel >= DocumentLayoutDetailsLevel.Column)
                this.layoutFormatterMain.restartFromRow(this.subDocument, this.layoutPosition, this.modelPosition);
            else
                this.layoutFormatterMain.restartFromPage(this.layoutPosition.pageIndex, this.modelPosition);

            this.subDocument = null;
            this.layoutPosition = null;
            this.modelPosition = -1;
        }
    }

} 