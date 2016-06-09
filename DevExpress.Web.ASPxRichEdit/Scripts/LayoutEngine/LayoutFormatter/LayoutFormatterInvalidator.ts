module __aspxRichEdit {
    export class LayoutFormatterInvalidator {
        private layout: DocumentLayout;
        private model: DocumentModel;
        private selection: Selection;
        private layoutFormatterMain: LayoutFormatterMain;

        constructor(layout: DocumentLayout, model: DocumentModel, selection: Selection, layoutFormatterMain: LayoutFormatterMain) {
            this.layout = layout;
            this.model = model;
            this.selection = selection;
            this.layoutFormatterMain = layoutFormatterMain;
        }
        // invalidate layout parts
        // length > 0 mean inserted length symbols in position logPosition
        // length < 0 mean deleted |length| symbols after position logPosition
        public onContentInserted(subDocument: SubDocument, logPosition: number, length: number, restartFromParagraphStart: boolean) {
            // recalculate possibly less elements, than onIntervalChanged
            const pages: LayoutPage[] = this.layout.pages;
            if (pages.length == 0)
                return;

            if(!subDocument.isMain()) {
                ////Logging.print(LogSource.LayoutFormatterInvalidator, `onContentInserted(header\footer). subDocument.id:${subDocument.id}, logPosition:${logPosition}, length:${length}, restartFromParagraphStart:${restartFromParagraphStart}`);
                return this.onHeaderFooterEdited(subDocument);
            }

            if (length > 0)
                return this.contentInserted(subDocument, logPosition, length, restartFromParagraphStart);
            if (length < 0)
                return this.contentDeleted(subDocument, logPosition, length, restartFromParagraphStart);
            throw new Error(Errors.InternalException);
        }
        public onIntervalChanged(subDocument: SubDocument, interval: FixedInterval, isCalculateTables: boolean = true) {
            if(!subDocument.isMain()) {
                //Logging.print(LogSource.LayoutFormatterInvalidator, `onIntervalChanged(header/footer). subDocument.id:${subDocument.id}, interval:${Logging.fixedIntervalToString(interval) }, isCalculateTables:${isCalculateTables}`);
                return this.onHeaderFooterEdited(subDocument);
            }

            //Logging.print(LogSource.LayoutFormatterInvalidator, `onIntervalChanged. subDocument.id:${subDocument.id}, interval:${Logging.fixedIntervalToString(interval) }, isCalculateTables:${isCalculateTables}`);
            if (isCalculateTables && subDocument.tables.length)
                interval = LayoutFormatterInvalidator.getExpandedByTableUpdateInterval(subDocument, interval);
            
            const startPosition: LayoutPosition = this.findLayoutPositionInAllLayout(subDocument, interval.start);
            const intervalEnd: number = interval.end();

            // invalidate interval
            for (const position: LayoutPosition = startPosition.clone(); position.getLogPosition(DocumentLayoutDetailsLevel.Row) <= intervalEnd;) {
                if (LayoutFormatterInvalidator.shouldInvalidateWholePage(position.page, interval)) {
                    position.page.isInnerContentValid = false;
                    if (!this.advanceToNextPage(position))
                        break;
                }
                else {
                    position.row.isContentValid = false;
                    position.column.isContentValid = false;
                    position.pageArea.isContentValid = false;
                    position.page.isContentValid = false;
                    if (!position.advanceToNextRow(this.layout))
                        break;
                }
            }
            startPosition.detailsLevel = DocumentLayoutDetailsLevel.Column;
            startPosition.row = null;

            if (interval.start < startPosition.page.getStartOffsetContentOfMainSubDocument()) {
                startPosition.detailsLevel = DocumentLayoutDetailsLevel.None;
                startPosition.page = null;
            }
            const pos: number = subDocument.isMain() ? startPosition.getLogPosition() + startPosition.column.rows[startPosition.rowIndex].columnOffset
                : this.layout.pages[startPosition.pageIndex].getStartOffsetContentOfMainSubDocument();
            this.layoutFormatterMain.restartFromRow(subDocument, startPosition, Math.max(pos, 0));
        }
        public onChangedSection(section: Section, sectionIndex: number) {
            //Logging.print(LogSource.LayoutFormatterInvalidator, `onChangedSection. sectionIndex:${sectionIndex}`);
            const pages: LayoutPage[] = this.layout.pages;
            const sectionStartLogPos: number = section.startLogPosition.value;
            const sectionEndLogPos: number = section.startLogPosition.value + section.getLength();

            let pageIndexStart: number = Math.max(0, Utils.normedBinaryIndexOf(pages, (p: LayoutPage) => p.getStartOffsetContentOfMainSubDocument() - sectionStartLogPos));
            pageIndexStart = pages[pageIndexStart].getFirstPageInGroup().index;

            let pageIndexEnd: number = Utils.normedBinaryIndexOf(pages, (p: LayoutPage) => p.getStartOffsetContentOfMainSubDocument() - sectionEndLogPos);
            pageIndexEnd = Math.min(pages[pageIndexEnd].getLastPageInGroup(pages).index + 1, this.layout.pages.length);

            const lastPage: LayoutPage = pages[pageIndexEnd];
            if (lastPage && lastPage.getStartOffsetContentOfMainSubDocument() == pageIndexEnd)
                pageIndexEnd++;

            for (let i: number = pageIndexStart; i < pageIndexEnd; i++)
                pages[i].isInnerContentValid = false;

            this.layoutFormatterMain.restartFromPage(pageIndexStart, sectionStartLogPos);
        }
        public onListLevelChanged(newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            //Logging.print(LogSource.LayoutFormatterInvalidator, `onListLevelChanged. newState:`, newState);
            for (let obj of newState.objects) {
                for (let subDocument of this.model.getSubDocumentsList()) {
                    const abstractNumberingListIndex: number = obj.isAbstractNumberingList ? obj.numberingListIndex :
                        subDocument.documentModel.numberingLists[obj.numberingListIndex].abstractNumberingListIndex;
                    const listLevelIndex: number = obj.listLevelIndex;
                    for (let paragraph of subDocument.paragraphs) {
                        if (paragraph.getAbstractNumberingListIndex() === abstractNumberingListIndex && paragraph.getListLevelIndex() === listLevelIndex)
                            this.onIntervalChanged(subDocument, paragraph.getInterval());
                    }
                }
            }
        }
        public onHeaderFooterIndexChanged(sectionIndex: number, isHeader: boolean, type: HeaderFooterType, newIndex: number) {
            if(this.selection.pageIndex > -1) {
                const layoutPage = this.layout.pages[this.selection.pageIndex];
                layoutPage.isInnerContentValid = false;
                this.layoutFormatterMain.restartFromPage(this.selection.pageIndex, layoutPage.getStartOffsetContentOfMainSubDocument());
            } else {
                let sectionStartLogPos = this.model.sections[sectionIndex].startLogPosition.value;
                let sectionEndLogPos = this.model.sections[sectionIndex].getEndPosition();

                let pages = this.layout.pages;
                let firstPageIndex: number = Math.max(0, Utils.normedBinaryIndexOf(pages, (p: LayoutPage) => p.getStartOffsetContentOfMainSubDocument() - sectionStartLogPos));
                firstPageIndex = pages[firstPageIndex].getFirstPageInGroup().index;

                let pageIndexStart = firstPageIndex;
                if(type !== HeaderFooterType.First) {
                    let isEvenHeaderFooter = type == HeaderFooterType.Even;
                    if(Utils.isEven(firstPageIndex) !== isEvenHeaderFooter)
                        pageIndexStart++;
                }

                let pageIndexEnd: number = Utils.normedBinaryIndexOf(pages, (p: LayoutPage) => p.getStartOffsetContentOfMainSubDocument() - sectionEndLogPos);
                pageIndexEnd = Math.min(pages[pageIndexEnd].getLastPageInGroup(pages).index + 1, this.layout.pages.length);

                const lastPage: LayoutPage = pages[pageIndexEnd];
                if(lastPage && lastPage.getStartOffsetContentOfMainSubDocument() == pageIndexEnd)
                    pageIndexEnd++;

                let modelPosition = pages[pageIndexStart].getStartOffsetContentOfMainSubDocument();
                for(let i: number = pageIndexStart; i < pageIndexEnd; i++)
                    pages[i].isInnerContentValid = false;

                this.layoutFormatterMain.restartFromPage(pageIndexStart, modelPosition);
            }
        }
        public onChangedAllLayout() {
            //Logging.print(LogSource.LayoutFormatterInvalidator, `onChangedAllLayout`);
            this.layoutFormatterMain.restartFormatingAllLayout();
        }

        // move
        // length > 0
        private onHeaderFooterEdited(subDocument: SubDocument) {
            if(this.selection.pageIndex > -1) {
                const lp: LayoutPosition = new LayoutPosition(DocumentLayoutDetailsLevel.Column);
                lp.pageIndex = this.selection.pageIndex;
                lp.page = this.layout.pages[lp.pageIndex];
                this.layout.pages[lp.pageIndex].isContentValid = false;

                return this.layoutFormatterMain.restartFromRow(subDocument, lp, 0);
            } else {
                //T313410
                for(let i = 0; i < this.layout.pages.length; i++) {
                    if(this.layout.pages[i].otherPageAreas[subDocument.id]) {
                        return this.onIntervalChanged(subDocument.documentModel.mainSubDocument, new FixedInterval(this.layout.pages[i].getStartOffsetContentOfMainSubDocument(),
                            subDocument.documentModel.mainSubDocument.getDocumentEndPosition()));
                    }
                }
            }
        }
        private contentInserted(subDocument: SubDocument, logPosition: number, length: number, restartFromParagraphStart: boolean) {
            //Logging.print(LogSource.LayoutFormatterInvalidator, `contentInserted. subDocument.id:${subDocument.id}, logPosition:${logPosition}, length:${length}, restartFromParagraphStart:${restartFromParagraphStart}`);
            const pages: LayoutPage[] = this.layout.pages;
            let layoutPosParagraphStart: LayoutPosition = null;
            if (restartFromParagraphStart) {
                const paragraphIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - logPosition);
                const paragraphStartPosition: number = subDocument.paragraphs[paragraphIndex].startLogPosition.value;

                layoutPosParagraphStart = this.findLayoutPositionInAllLayout(subDocument, paragraphStartPosition);
                LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPosParagraphStart);
            }

            let layoutPos: LayoutPosition = this.findLayoutPositionInAllLayout(subDocument, logPosition);
            LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPos);
            
            for (let pageIndex: number = layoutPos.pageIndex + 1, page: LayoutPage; page = pages[pageIndex]; pageIndex++)
                for (let interval of page.contentIntervals)
                    interval.start += length;

            this.moveRowsToRight(layoutPos, length);
            this.moveColumnsToRight(layoutPos, length);
            this.movePageAreasToRight(layoutPos, length);
            
            LayoutFormatterInvalidator.movePageIntervalsToRight(layoutPos.page.contentIntervals, logPosition, length);

            const oldLayoutPos: LayoutPosition = layoutPos.clone();
            layoutPos.advanceToPrevRow(this.layout);
            if (!oldLayoutPos.row.tableCellInfo && layoutPos.row.tableCellInfo)
                layoutPos = oldLayoutPos; // this for not format table above when this first row below table
            else
                LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPos);
            
            const lowerPositionInfo: LayoutAndModelPositions = LayoutFormatterInvalidator.getMinByModelPosition(
                new LayoutAndModelPositions(layoutPos, layoutPos.getLogPosition()),
                new LayoutAndModelPositions(layoutPosParagraphStart, layoutPosParagraphStart ? layoutPosParagraphStart.getLogPosition() : 0));

            const tables: Table[] = this.layoutFormatterMain.iterator.subDocument.tables;
            if (lowerPositionInfo.layoutPosition.row.tableCellInfo && tables.length) {
                const tableIndex: number = Math.max(0, Utils.normedBinaryIndexOf(tables, (t: Table) => t.getStartPosition() - lowerPositionInfo.modelPosition));
                lowerPositionInfo.modelPosition = tables[tableIndex].getTopLevelParent().getStartPosition();
                lowerPositionInfo.layoutPosition = this.findLayoutPositionInAllLayout(subDocument, lowerPositionInfo.modelPosition);
            }
            
            lowerPositionInfo.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Column;
            lowerPositionInfo.layoutPosition.row = null;
            this.layoutFormatterMain.restartFromRow(subDocument, lowerPositionInfo.layoutPosition, Math.min(logPosition, lowerPositionInfo.modelPosition));
        }
        // length < 0
        private contentDeleted(subDocument: SubDocument, logPosition: number, length: number, restartFromParagraphStart: boolean) {
            //Logging.print(LogSource.LayoutFormatterInvalidator, `contentDeleted. subDocument.id:${subDocument.id}, logPosition:${logPosition}, length:${length}, restartFromParagraphStart:${restartFromParagraphStart}`);
            const deletedInterval: FixedInterval = new FixedInterval(logPosition, -length);
            const pages: LayoutPage[] = this.layout.pages;
            
            let layoutPosParagraphStart: LayoutPosition = null;
            if (restartFromParagraphStart) {
                const paragraphIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - logPosition);
                const paragraphStartPosition: number = subDocument.paragraphs[paragraphIndex].startLogPosition.value;

                layoutPosParagraphStart = this.findLayoutPositionInAllLayout(subDocument, paragraphStartPosition);
                LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPosParagraphStart);
                layoutPosParagraphStart.advanceToPrevRow(this.layout);
                LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPosParagraphStart);
            }
            const modelPosParagraphStart: number = layoutPosParagraphStart ? layoutPosParagraphStart.getLogPosition() : 0;

            let layoutPos: LayoutPosition = this.findLayoutPositionInAllLayout(subDocument, logPosition);
            LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPos);

            let prevRowLayoutPos: LayoutPosition = layoutPos.clone();
            if (prevRowLayoutPos.advanceToPrevRow(this.layout)) {
                if (!layoutPos.row.tableCellInfo && prevRowLayoutPos.row.tableCellInfo)
                    prevRowLayoutPos = layoutPos.clone(); // this for not format table above when this first row below table
                else
                    LayoutFormatterInvalidator.invalidateLayoutPosition(prevRowLayoutPos);
            }
            const prevRowModelPos: number = prevRowLayoutPos.getLogPosition();

            for (let pageIndex: number = layoutPos.pageIndex, page: LayoutPage; page = pages[pageIndex]; pageIndex++) {
                const pageStartPos: number = page.getStartOffsetContentOfMainSubDocument();
                if (deletedInterval.containsInterval(pageStartPos, page.getLastContentInterval().end())) {
                    this.layoutFormatterMain.addPageChangeDeleted(pageIndex);
                    if (pages.length == 1)
                        throw new Error(Errors.InternalException);
                    pages.splice(pageIndex, 1);
                    
                    layoutPos.page = pages[pageIndex--];
                    layoutPos.pageAreaIndex = 0;
                    layoutPos.pageArea = layoutPos.page.mainSubDocumentPageAreas[0];
                    layoutPos.columnIndex = 0;
                    layoutPos.column = layoutPos.pageArea.columns[0];
                    layoutPos.rowIndex = 0;
                    layoutPos.row = layoutPos.column.rows[0];
                    
                    LayoutFormatterInvalidator.invalidateLayoutPosition(layoutPos);
                }
                else {
                    page.index = pageIndex;
                    if (pageStartPos < deletedInterval.end()) {
                        this.deletePartOfPageContent(layoutPos, deletedInterval);
                    }
                    else
                        for (let interval of page.contentIntervals)
                            interval.start += length;
                }
            }
            
            const lowerPositionInfo: LayoutAndModelPositions = LayoutFormatterInvalidator.getMinByModelPosition(
                new LayoutAndModelPositions(prevRowLayoutPos, prevRowModelPos),
                new LayoutAndModelPositions(layoutPosParagraphStart, modelPosParagraphStart));

            const tables: Table[] = this.layoutFormatterMain.iterator.subDocument.tables;
            if (lowerPositionInfo.layoutPosition.row.tableCellInfo && tables.length) {
                const tableIndex: number = Math.max(0, Utils.normedBinaryIndexOf(tables, (t: Table) => t.getStartPosition() - lowerPositionInfo.modelPosition));
                lowerPositionInfo.modelPosition = tables[tableIndex].getTopLevelParent().getStartPosition();
                lowerPositionInfo.layoutPosition = this.findLayoutPositionInAllLayout(subDocument, lowerPositionInfo.modelPosition);
            }

            if (lowerPositionInfo.layoutPosition.detailsLevel == DocumentLayoutDetailsLevel.Row) {
                lowerPositionInfo.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Column;
                lowerPositionInfo.layoutPosition.row = null;
            }
            else
                lowerPositionInfo.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.None;
            this.layoutFormatterMain.restartFromRow(subDocument, lowerPositionInfo.layoutPosition, Math.min(logPosition, lowerPositionInfo.modelPosition));
        }

        private deletePartOfPageContent(layoutPos: LayoutPosition, deletedInterval: FixedInterval) {
            const rowChanges: RowChange[] = this.moveRowsToLeft(layoutPos, deletedInterval);
            LayoutFormatterBase.correctRowOffsets(layoutPos.column);

            const columnsChanges: ColumnChange[] = this.moveColumnsToLeft(layoutPos, deletedInterval, rowChanges);
            LayoutFormatterBase.correctColumnOffsets(layoutPos.pageArea);

            const pageAreaChanges: PageAreaChange[] = this.movePageAreasToLeft(layoutPos, deletedInterval, columnsChanges);
            LayoutFormatterInvalidator.movePageIntervalsToLeft(layoutPos.page.contentIntervals, deletedInterval);
            LayoutFormatterMain.correctPageOffsets(layoutPos.page);

            const pageChange: PageChange = new PageChange(LayoutChangeType.Updated, layoutPos.page.index, layoutPos.page, pageAreaChanges);
            this.layoutFormatterMain.pageChanges.push(pageChange);
        }

        private moveRowsToRight(layoutPosition: LayoutPosition, offset: number) {
            const rows: LayoutRow[] = layoutPosition.column.rows;
            for (let rowIndex: number = layoutPosition.rowIndex + 1, row: LayoutRow; row = rows[rowIndex]; rowIndex++)
                row.columnOffset += offset;
        }
        private moveColumnsToRight(layoutPosition: LayoutPosition, offset: number) {
            const columns: LayoutColumn[] = layoutPosition.pageArea.columns;
            for (let columnIndex: number = layoutPosition.columnIndex + 1, column: LayoutColumn; column = columns[columnIndex]; columnIndex++)
                column.pageAreaOffset += offset;
        }
        private movePageAreasToRight(layoutPosition: LayoutPosition, offset: number) {
            const pageAreas: LayoutPageArea[] = layoutPosition.page.mainSubDocumentPageAreas;
            for (let pageAreaIndex: number = layoutPosition.pageAreaIndex + 1, pageArea: LayoutPageArea; pageArea = pageAreas[pageAreaIndex]; pageAreaIndex++)
                pageArea.pageOffset += offset;
        }
        
        private moveRowsToLeft(layoutPosition: LayoutPosition, constDeletedInterval: FixedInterval): RowChange[] {
            const rowChanges: RowChange[] = [];
            const rows: LayoutRow[] = layoutPosition.column.rows;
            const colPos: number = layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column);
            if (constDeletedInterval.end() <= colPos)
                return rowChanges;

            const deletedInterval: FixedInterval = constDeletedInterval.start < colPos ?
                FixedInterval.fromPositions(colPos, constDeletedInterval.end()) :
                constDeletedInterval.clone();
            
            for (let rowIndex: number = layoutPosition.rowIndex, row: LayoutRow; row = rows[rowIndex]; rowIndex++) {
                const rowInterval: FixedInterval = new FixedInterval(colPos + row.columnOffset, row.getLastBoxEndPositionInRow());
                const rowIntersection: FixedInterval = FixedInterval.getIntersection(rowInterval, deletedInterval);
                if (rowIntersection && rowIntersection.equals(rowInterval)) {
                    rows.splice(rowIndex, 1);
                    rowChanges.push(new RowChange(LayoutChangeType.Deleted, rowIndex, row));
                    rowIndex--;
                }
                else {
                    const deletedPrevIntervalLen: number = rowInterval.start - deletedInterval.start;
                    if (deletedPrevIntervalLen > 0)
                        row.columnOffset -= Math.min(deletedInterval.length, deletedPrevIntervalLen);

                    if (rowIntersection && rowIntersection.length > 0) {
                        const newRowLength: number = rowInterval.length - rowIntersection.length;
                        const fakeBox: LayoutTextBox = new LayoutTextBox(null, new Array(newRowLength + 1).join(" "));
                        fakeBox.rowOffset = 0;
                        row.boxes = [fakeBox];
                    }
                }
            }

            const firstChange: RowChange = rowChanges[0];
            if (firstChange && firstChange.layoutElement == layoutPosition.row) {
                layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Column;
                layoutPosition.row = null;
                layoutPosition.rowIndex = -1;
            }

            return rowChanges;
        }
        private moveColumnsToLeft(layoutPosition: LayoutPosition, constDeletedInterval: FixedInterval, rowChanges: RowChange[]): ColumnChange[]{
            const columnChanges: ColumnChange[] = [];
            const subDocument: SubDocument = layoutPosition.pageArea.subDocument;
            
            const pageAreaPos: number = layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.PageArea);
            const deletedInterval: FixedInterval = constDeletedInterval.start < pageAreaPos ?
                FixedInterval.fromPositions(pageAreaPos, constDeletedInterval.end()) :
                constDeletedInterval.clone();

            if (deletedInterval.end() > pageAreaPos) {
                const columns: LayoutColumn[] = layoutPosition.pageArea.columns;
                let columnIndex: number = layoutPosition.columnIndex;
                for (let column: LayoutColumn; (column = columns[columnIndex]) && columns.length > 1; columnIndex++) {
                    const lastRow: LayoutRow = column.getLastRow();
                    const columnStartPos: number = pageAreaPos + column.pageAreaOffset;
                    const columnEndPos: number = columnStartPos + (lastRow ? lastRow.getEndPosition() : 0);
                    if (deletedInterval.containsInterval(columnStartPos, columnEndPos)) {
                        columns.splice(columnIndex, 1);
                        columnChanges.push(new ColumnChange(LayoutChangeType.Deleted, columnIndex, null, [], [], []));
                        columnIndex--;
                    }
                    else {
                        const deletedPrevIntervalLen: number = columnStartPos - deletedInterval.start;
                        if (deletedPrevIntervalLen > 0)
                            column.pageAreaOffset -= Math.min(deletedInterval.length, deletedPrevIntervalLen);
                    }
                }
            }

            const firstChange: ColumnChange = columnChanges[0];
            if (firstChange && firstChange.layoutElement == layoutPosition.column) {
                layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.PageArea;
                layoutPosition.column = null;
                layoutPosition.columnIndex = -1;
            }
            else
                columnChanges.push(new ColumnChange(LayoutChangeType.Updated, layoutPosition.columnIndex, layoutPosition.column, rowChanges, [], []));
            
            return columnChanges;
        }
        private movePageAreasToLeft(layoutPosition: LayoutPosition, constDeletedInterval: FixedInterval, columnChanges: ColumnChange[]): PageAreaChange[]{
            const pageAreaChanges: PageAreaChange[] = [];
            const subDocument: SubDocument = layoutPosition.pageArea.subDocument;
            const pagePos: number = layoutPosition.page.getStartOffsetContentOfMainSubDocument();
            const delIntervalOffset: number = constDeletedInterval.start - pagePos;
            const pageAreas: LayoutPageArea[] = layoutPosition.page.mainSubDocumentPageAreas;
            let pageAreaIndex: number = layoutPosition.pageAreaIndex;
            const deletedInterval: FixedInterval = constDeletedInterval.start < pagePos ?
                FixedInterval.fromPositions(pagePos, constDeletedInterval.end()) :
                constDeletedInterval.clone();
            for (let pageArea: LayoutPageArea; (pageArea = pageAreas[pageAreaIndex]) && pageAreas.length > 1; pageAreaIndex++) {
                const lastColumn: LayoutColumn = pageArea.getLastColumn();
                const pageAreaStartPos: number = pagePos + pageArea.pageOffset;
                const pageAreaEndPos: number = pageAreaStartPos + (lastColumn ? lastColumn.getEndPosition() : 0);
                if (deletedInterval.containsInterval(pageAreaStartPos, pageAreaEndPos)) {
                    pageAreas.splice(pageAreaIndex, 1);
                    pageAreaChanges.push(new PageAreaChange(LayoutChangeType.Deleted, pageAreaIndex, null, []));
                    pageAreaIndex--;
                }
                else {
                    const deletedPrevIntervalLen: number = pageAreaStartPos - deletedInterval.start;
                    if (deletedPrevIntervalLen > 0)
                        pageArea.pageOffset -= Math.min(deletedInterval.length, deletedPrevIntervalLen);
                }
            }

            const firstChange: PageAreaChange = pageAreaChanges[0];
            if (firstChange && firstChange.layoutElement == layoutPosition.pageArea) {
                layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Page;
                layoutPosition.pageArea = null;
                layoutPosition.pageAreaIndex = -1;
            }
            else
                pageAreaChanges.push(new PageAreaChange(LayoutChangeType.Updated, layoutPosition.pageAreaIndex, layoutPosition.pageArea, columnChanges));
            return pageAreaChanges;
        }
        
        // others
        private static areAllRowsInPageInvalid(page: LayoutPage): boolean {
            if (!page.isInnerContentValid)
                return true;

            if (page.isContentValid)
                return false;

            for (let pageArea of page.mainSubDocumentPageAreas)
                if (LayoutFormatterInvalidator.pageAreaConsiderValidRow(pageArea))
                    return false;

            for (let subDocId in page.otherPageAreas) {
                if (!page.otherPageAreas.hasOwnProperty(subDocId))
                    continue;
                if (LayoutFormatterInvalidator.pageAreaConsiderValidRow(page.otherPageAreas[subDocId]))
                    return false;
            }
            return true;
        }
        private static shouldInvalidateWholePage(page: LayoutPage, interval: FixedInterval) {
            for (let internalContent of page.contentIntervals) {
                if (!interval.containsInterval(internalContent.start, internalContent.end()))
                    return false;
            }
            return true;
        }
        private static pageAreaConsiderValidRow(pageArea: LayoutPageArea): boolean {
            if (pageArea.isContentValid)
                return true;
            for (let column of pageArea.columns) {
                if (column.isContentValid)
                    return true;
                for (let row of column.rows)
                    if (row.isContentValid)
                        return true;
            }
            return false;
        }
        private restartFromZeroPosition(subDocument: SubDocument) {
            const lp: LayoutPosition = new LayoutPosition(DocumentLayoutDetailsLevel.None);
            lp.pageIndex = 0;
            this.layoutFormatterMain.restartFromRow(subDocument, lp, 0);
        }

        //determine model-layout positions
        //args[0] members must be non null
        private static getMinByModelPosition(...args: LayoutAndModelPositions[]): LayoutAndModelPositions {
            let minLayoutPos: LayoutPosition = args[0].layoutPosition;
            let minModelPos: number = args[0].modelPosition;
            for (let i: number = 1, currPositions: LayoutAndModelPositions; currPositions = args[i]; i++) {
                const currLayoutPosition: LayoutPosition = currPositions.layoutPosition;
                if (!currLayoutPosition)
                    continue;
                const currModelPosition: number = currPositions.modelPosition;
                if (currModelPosition < minModelPos || currModelPosition == minModelPos && currPositions.layoutPosition.detailsLevel < minLayoutPos.detailsLevel) {
                    minModelPos = currModelPosition;
                    minLayoutPos = currLayoutPosition;
                }
            }
            return new LayoutAndModelPositions(minLayoutPos, minModelPos);
        }

        public advanceToNextPage(layoutPosition: LayoutPosition): boolean {
            const subDocument: SubDocument = layoutPosition.pageArea.subDocument;
            if (!subDocument.isMain())
                return false;
            if (layoutPosition.pageIndex + 1 < this.layout.pages.length) {
                layoutPosition.pageIndex++;
                layoutPosition.page = this.layout.pages[layoutPosition.pageIndex];

                layoutPosition.pageAreaIndex = 0;
                layoutPosition.pageArea = layoutPosition.page.mainSubDocumentPageAreas[0];

                layoutPosition.columnIndex = 0;
                layoutPosition.column = layoutPosition.pageArea.columns[0];

                layoutPosition.rowIndex = 0;
                layoutPosition.row = layoutPosition.column.rows[0];

                return true;
            }
            else
                return false;
        }

        private static getExpandedByTableUpdateInterval(subDocument: SubDocument, allInterval: FixedInterval): FixedInterval {
            let resultInterval: FixedInterval = allInterval.clone();
            const endPos: number = resultInterval.end() - 1;
            const tblByLevelNil: Table[] = subDocument.tablesByLevels[0];
            const tableIndexStart: number = Utils.normedBinaryIndexOf(tblByLevelNil, (t: Table) => t.getStartPosition() - resultInterval.start);
            const tableIndexEnd: number = Utils.normedBinaryIndexOf(tblByLevelNil, (t: Table) => t.getStartPosition() - endPos);
            for (let i: number = tableIndexStart; i <= tableIndexEnd; i++) {
                const table: Table = tblByLevelNil[i];
                if (table) {
                    const tableInterval: FixedInterval = FixedInterval.fromPositions(table.getStartPosition(), table.getEndPosition());
                    if (FixedInterval.getIntersection(tableInterval, resultInterval))
                        resultInterval.expandInterval(tableInterval);
                }
            }
            return resultInterval;
        }

        private findLayoutPositionInAllLayout(subDocument: SubDocument, pos: number): LayoutPosition {
            const realValidPageCount: number = this.layout.validPageCount; // dirty hack for search by all pages
            const realIsFullyFormatted: boolean = this.layout.isFullyFormatted;
            this.layout.validPageCount = this.layout.pages.length;
            this.layout.isFullyFormatted = true;

            const res: LayoutPosition = subDocument.isMain() ?
                new LayoutPositionMainSubDocumentCreator(this.layout, subDocument, pos, DocumentLayoutDetailsLevel.Row)
                    .create(new LayoutPositionCreatorConflictFlags().setCustom(false, false, false, false), new LayoutPositionCreatorConflictFlags().setDefault(true)) :
                new LayoutPositionOtherSubDocumentCreator(this.layout, subDocument, pos, this.selection.pageIndex, DocumentLayoutDetailsLevel.Row)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(true));

            this.layout.validPageCount = realValidPageCount;
            this.layout.isFullyFormatted = realIsFullyFormatted;
            return res;
        }

        private static invalidateLayoutPosition(lp: LayoutPosition) {
            if (lp.pageArea.subDocument.isMain())
                lp.page.isContentValid = false;
            lp.pageArea.isContentValid = false;
            lp.column.isContentValid = false;
            lp.row.isContentValid = false;
        }

        private static movePageIntervalsToRight(intervals: FixedInterval[], pos: number, length: number) {
            let intervalIndex: number = Math.max(0, Utils.normedBinaryIndexOf(intervals, (i: FixedInterval) => i.start - pos));
            let interval: FixedInterval = intervals[intervalIndex];
            if (pos >= interval.end())
                return;

            if (pos >= interval.start) {
                interval.length += length;
                intervalIndex++;
            }

            for (; interval = intervals[intervalIndex]; intervalIndex++)
                interval.start += length;
        }
        // length < 0
        private static movePageIntervalsToLeft(intervals: FixedInterval[], deletedInterval: FixedInterval) {
            for (let intervalIndex: number = 0, interval: FixedInterval; interval = intervals[intervalIndex]; intervalIndex++) {
                if (interval.end() <= deletedInterval.start)
                    continue;

                if (interval.start >= deletedInterval.end()) {
                    interval.start -= deletedInterval.length;
                    continue;
                }

                if (deletedInterval.containsInterval(interval.start, interval.end())) {
                    intervals.splice(intervalIndex, 1);
                    intervalIndex--;
                    continue;
                }

                if (interval.start <= deletedInterval.start) {
                    interval.length -= Math.min(deletedInterval.length, interval.end() - deletedInterval.start);
                }
                else {
                    interval.length = interval.end() - deletedInterval.end();
                    interval.start = deletedInterval.start;
                }
            }
        }

    }
} 