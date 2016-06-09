module __aspxRichEdit {
    export class FormatPageAreaResult {
        pageArea: LayoutPageArea;
        columnChanges: ColumnChange[];

        constructor(pageArea: LayoutPageArea, columnChanges: ColumnChange[]) {
            this.pageArea = pageArea;
            this.columnChanges = columnChanges;
        }
    }

    // format pageArea level and levels below
    export class LayoutFormatterBase extends BatchUpdatableObject { // BatchUpdatableObject not implement nothing in this class. Child implement this.
        protected static NEED_RECALCULATE_PARAGRAPH_INDEX: number = -2;
        protected unitConverter: IUnitConverter;
        protected stateMap: { [state: number]: any }; // LayoutFormatterState to function
        protected rowFormatter: ITextRowFormatter;
        public iterator: ITextBoxIterator; // here bound subDocument

        protected pageAreaBounds: Rectangle;
        protected columnBounds: Rectangle[];
        protected firstPageOfSection: boolean;

        protected lastRow: LayoutRow = null;
        protected lastRowStartPosition = 0; // not use. need for calculate parIndex
        protected lastRowParagraphIndex: number = -1;

        protected state: LayoutFormatterState;
        public layoutPosition: LayoutPosition; // current. It's very important

        protected pageAreaChanges: PageAreaChange[];
        public columnChanges: ColumnChange[];
        protected rowChanges: RowChange[];
        protected tableChanges: TableChange[];

        protected startRowPosition: number; // row formatting level only
        public currColumnHeight: number;

        public pagesCount: number = 1; //TODO
        public layoutDependentOtherPageAreasCache: { [pageIndex: number]: number[] } = {};

        protected bindNextPage: boolean = false; // when formatting tables
        private tableBoxCollectorIndex: number; // when formatting table
        private nextTablePosition: number; // which position we need switch to mode page formatting

        protected nextTableIndex: number;
        protected currTableColumnInfoIndex: number;
        protected disableReuse: boolean = false;

        constructor(iterator: ITextBoxIterator, unitConverter: IUnitConverter) {
            super();
            this.iterator = iterator;
            this.unitConverter = unitConverter;
            this.rowFormatter = new TextRowFormatter(iterator, this.unitConverter);
            this.state = LayoutFormatterState.PageAreaStart;
            this.lastRow = null;
            this.lastRowStartPosition = 0;
            this.lastRowParagraphIndex = -1;

            this.stateMap = {};
            this.stateMap[LayoutFormatterState.PageAreaStart] = this.processStatePageAreaStart;
            this.stateMap[LayoutFormatterState.ColumnStart] = this.processStateColumnStart;
            this.stateMap[LayoutFormatterState.RowFormatting] = this.processStateRowFormatting;
            this.stateMap[LayoutFormatterState.ColumnEnd] = this.processStateColumnEnd;
        }

        // format other pageArea
        public formatPageArea(pageAreaBounds: Rectangle, columnBounds: Rectangle[], pageIndex: number, pagesCount: number) { //TODO
            //Logging.print(LogSource.LayoutFormatter, "formatPageArea");
            this.pageAreaBounds = pageAreaBounds;
            this.columnBounds = columnBounds;
            //TODO
            this.pagesCount = pagesCount;

            if(!this.layoutPosition) {
                this.layoutPosition = new LayoutPosition(DocumentLayoutDetailsLevel.None);
                this.layoutPosition.page = new LayoutPage();
                this.layoutPosition.pageIndex = Math.max(0, pageIndex);
            }
            this.setNextExpectedTable(0);
            this.pageAreaChanges = [];
            while (this.stateMap[this.state].call(this) && this.state != LayoutFormatterState.PageAreaEnd);
            if (this.state == LayoutFormatterState.PageAreaEnd) {
                this.finalizePageArea();
            }
            else {
                // rollback all changes. Not enough chunks
            }
        }

        private processStatePageAreaStart(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStatePageAreaStart", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            if (!this.tryReusePageArea(this.pageAreaBounds)) {
                const subDocument: SubDocument = this.iterator.subDocument;
                const newPageArea: LayoutPageArea = new LayoutPageArea(subDocument);
                newPageArea.pageOffset = 0;
                //newPageArea.sectionIndex = this.iterator.getSectionIndex(); // TODO
                (<Rectangle>newPageArea).copyFrom(this.pageAreaBounds);

                this.layoutPosition.pageArea = newPageArea;
                this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.PageArea;
                this.layoutPosition.columnIndex = 0;
                this.layoutPosition.column = null;

                this.state = LayoutFormatterState.ColumnStart;
            }
            this.columnChanges = [];
            this.pageAreaChanges.push(new PageAreaChange(LayoutChangeType.Updated, this.layoutPosition.pageAreaIndex, this.layoutPosition.pageArea, this.columnChanges));
            return true;
        }
        private processStateColumnStart(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStateColumnStart", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            const columnBounds: Rectangle = this.columnBounds[this.layoutPosition.columnIndex];
            if (!columnBounds) {
                this.layoutPosition.columnIndex--;
                this.state = LayoutFormatterState.PageAreaEnd;
                return true;
            }
            
            this.currColumnHeight = 0;
            this.currTableColumnInfoIndex = 0;
            if (!this.tryReuseColumn(columnBounds)) {
                const subDocument: SubDocument = this.iterator.subDocument;
                const newColumn: LayoutColumn = new LayoutColumn();
                newColumn.pageAreaOffset = 0;
                (<Rectangle>newColumn).copyFrom(columnBounds);

                this.state = LayoutFormatterState.RowFormatting;

                this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Column;
                this.layoutPosition.column = newColumn;
                this.layoutPosition.rowIndex = 0;
            }
            this.rowChanges = [];
            this.tableChanges = [];
            // see setParagraphFrameChanges
            this.columnChanges.push(new ColumnChange(LayoutChangeType.Updated, this.layoutPosition.columnIndex, this.layoutPosition.column, this.rowChanges, this.tableChanges, []));
            return true;
        }
        private processStateRowFormatting(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStateRowFormatting", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.startRowPosition = this.iterator.getPosition();
            let row: LayoutRow = this.tryReuseRow();
            let rowParagraphIndex: number;
            if (!row) {
                row = this.createRow();

                rowParagraphIndex = this.rowFormatter.paragraphIndex;
                if (row.flags & LayoutRowStateFlags.NotEnoughChunks)
                    return false;
                // no finish paragraph mark
                if ((row.flags & LayoutRowStateFlags.DocumentEnd) && !(row.flags & LayoutRowStateFlags.ParagraphEnd)) {
                    rowParagraphIndex = 0;
                    this.addAndRegisterRowToCurrentColumn(LayoutFormatterBase.createEmptyRowWithParagraphBox(this.iterator));
                    this.state = LayoutFormatterState.ColumnEnd;
                    return true;
                }

                const firstBoxInRowOffset: number = row.boxes[0].rowOffset;
                if (this.layoutPosition.rowIndex == 0 && this.layoutPosition.columnIndex == 0 && this.layoutPosition.pageAreaIndex == 0 && this.iterator.subDocument.isMain())
                    this.layoutPosition.page.contentIntervals = [new FixedInterval(firstBoxInRowOffset, 0)];
                
                // normalize offsets (here boxes with absolute offset from document start)
                
                const pageOffset: number = this.iterator.subDocument.isMain() ? this.layoutPosition.page.getStartOffsetContentOfMainSubDocument() : 0;
                row.columnOffset = firstBoxInRowOffset - pageOffset - this.layoutPosition.pageArea.pageOffset - this.layoutPosition.column.pageAreaOffset;
                for (let box of row.boxes)
                    box.rowOffset -= firstBoxInRowOffset;
            }
            else {
                const rowOffset: number = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column) + row.columnOffset;
                rowParagraphIndex = Utils.normedBinaryIndexOf(this.iterator.subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - rowOffset);

                if (this.layoutPosition.rowIndex == 0 && this.iterator.subDocument.isMain())
                    this.layoutPosition.page.contentIntervals = [new FixedInterval(this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column) + row.columnOffset, 0)];
            }

            const paragraphs: Paragraph[] = this.iterator.subDocument.paragraphs;
            row.applySpacingBefore(rowParagraphIndex == this.getLastRowParagraphIndex() || !this.lastRow || (this.lastRow.flags & LayoutRowStateFlags.SectionEnd) ?
                0 : LayoutRow.getParagraphSpacingBefore(paragraphs[rowParagraphIndex], paragraphs[rowParagraphIndex - 1], this.unitConverter));
            row.applySpacingAfter(row.flags & LayoutRowStateFlags.ParagraphEnd ?
                LayoutRow.getParagraphSpacingAfter(paragraphs[rowParagraphIndex], paragraphs[rowParagraphIndex + 1], this.unitConverter) :
                0);

            row.y = this.currColumnHeight;
            if (this.layoutPosition.rowIndex != 0 && row.getBottomBoundPosition() > this.layoutPosition.column.height) {
                this.iterator.setPosition(this.startRowPosition, false);
                this.state = LayoutFormatterState.ColumnEnd;
                return true;
            }

            // update pageInterval
            if (this.iterator.subDocument.isMain()) {
                const contIntervals: FixedInterval[] = this.layoutPosition.page.contentIntervals;
                const lastInterval: FixedInterval = contIntervals[contIntervals.length - 1];
                const rowStartPos: number = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column) + row.columnOffset;
                const rowEndPos: number = rowStartPos + row.getLastBoxEndPositionInRow();
                if (this.lastRow && this.lastRow.tableCellInfo && lastInterval.end() != rowStartPos)
                    contIntervals.push(FixedInterval.fromPositions(rowStartPos, rowEndPos));
                else
                    lastInterval.length = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column) + row.getEndPosition() - lastInterval.start;
            }

            this.currColumnHeight = row.getBottomBoundPosition();

            this.addAndRegisterRowToCurrentColumn(row);
            this.layoutPosition.rowIndex++;

            if (row.flags &
                (LayoutRowStateFlags.ColumnEnd | LayoutRowStateFlags.DocumentEnd | LayoutRowStateFlags.SectionEnd | LayoutRowStateFlags.PageEnd))
                this.state = LayoutFormatterState.ColumnEnd;

            this.lastRow = row;
            this.lastRowParagraphIndex = rowParagraphIndex;

            return true;
        }
        private processStateColumnEnd(): boolean {
            //Logging.print(LogSource.LayoutFormatter, "processStateColumnEnd", ["SubDocId", this.iterator.subDocument.id], ["LayPos", LoggingToString.layoutPositionShort, [this.layoutPosition]]);
            this.removeRedundantRowsFromColumn(this.layoutPosition.rowIndex);
            this.removeRedundantTableInfosFromColumn();
            const createdColumn: LayoutColumn = this.layoutPosition.column;
            createdColumn.isContentValid = true;

            const lastRow: LayoutRow = createdColumn.getLastRow();
            lastRow.flags |= LayoutRowStateFlags.ColumnEnd;

            this.updateColumnParagraphFrames();

            LayoutFormatterBase.correctRowOffsets(createdColumn);

            this.addAndRegisterColumnToCurrentPageArea(createdColumn);

            this.firstPageOfSection = !!(lastRow.flags & LayoutRowStateFlags.SectionEnd);

            if (lastRow.flags &
                (LayoutRowStateFlags.DocumentEnd | LayoutRowStateFlags.PageEnd | LayoutRowStateFlags.SectionEnd)) {
                this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.PageArea;
                this.state = LayoutFormatterState.PageAreaEnd;
            }
            else {
                this.state = LayoutFormatterState.ColumnStart;

                this.layoutPosition.columnIndex++;
                this.layoutPosition.column = null;
            }
            return true;
        }
        
        // correctOffsets
        public static correctRowOffsets(column: LayoutColumn) {
            const rows: LayoutRow[] = column.rows;
            if (!rows.length)
                return;
            const offsetFirstRowFromColumn: number = rows[0].columnOffset;
            if (offsetFirstRowFromColumn > 0) {
                column.pageAreaOffset += offsetFirstRowFromColumn;
                for (let row of rows)
                    row.columnOffset -= offsetFirstRowFromColumn;
            }
        }
        public static correctColumnOffsets(pageArea: LayoutPageArea) {
            const columns: LayoutColumn[] = pageArea.columns;
            if (!columns.length)
                return;
            const offsetFirstColumnFromPageArea: number = columns[0].pageAreaOffset;
            if (offsetFirstColumnFromPageArea > 0) {
                pageArea.pageOffset += offsetFirstColumnFromPageArea;
                for (let column of columns)
                    column.pageAreaOffset -= offsetFirstColumnFromPageArea;
            }
        }

        //try reuse
        private tryReusePageArea(pageAreaBounds: Rectangle): boolean {
            const subDocument: SubDocument = this.iterator.subDocument;
            const pageArea: LayoutPageArea = subDocument.isMain() ?
                this.layoutPosition.page.mainSubDocumentPageAreas[this.layoutPosition.pageAreaIndex] :
                this.layoutPosition.page.otherPageAreas[subDocument.id];

            if (!pageArea || !this.layoutPosition.page.isInnerContentValid || !(<Rectangle>pageArea).equals(pageAreaBounds))
                return false;

            this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.PageArea;
            this.layoutPosition.pageArea = pageArea;

            const pageStartPos: number = subDocument.isMain() ? this.layoutPosition.page.getStartOffsetContentOfMainSubDocument() : 0;
            const pageAreaStartPos: number = pageStartPos + pageArea.pageOffset;
            const isPositionCorrect: boolean = pageAreaStartPos == this.iterator.getPosition() || this.iterator.nextVisibleBoxStartPositionEqualWith(pageAreaStartPos);
            if (pageArea.isContentValid && isPositionCorrect && !this.disableReuse) {
                const iteratorNewPosition: number = pageStartPos + pageArea.getEndPosition();
                this.iterator.setPosition(iteratorNewPosition, false);

                const lastCol: LayoutColumn = pageArea.getLastColumn();
                this.lastRow = lastCol.getLastRow();
                this.lastRowStartPosition = pageStartPos + pageArea.pageOffset + lastCol.pageAreaOffset + this.lastRow.columnOffset;
                this.lastRowParagraphIndex = LayoutFormatterBase.NEED_RECALCULATE_PARAGRAPH_INDEX;

                this.layoutPosition.columnIndex = pageArea.columns.length - 1;
                this.state = LayoutFormatterState.PageAreaEnd;
            }
            else {
                pageArea.pageOffset = 0;
                this.layoutPosition.columnIndex = 0;
                this.state = LayoutFormatterState.ColumnStart;
            }
            
            return true;
        }
        private tryReuseColumn(columnBounds: Rectangle): boolean {
            const column: LayoutColumn = this.layoutPosition.pageArea.columns[this.layoutPosition.columnIndex];
            if (!column || !this.layoutPosition.page.isInnerContentValid || !(<Rectangle>column).equals(columnBounds))
                return false;

            this.layoutPosition.detailsLevel = DocumentLayoutDetailsLevel.Column;
            this.layoutPosition.column = column;

            const currColumnStartPos: number = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.PageArea) + column.pageAreaOffset;
            const isPositionCorrect: boolean = currColumnStartPos == this.iterator.getPosition() || this.iterator.nextVisibleBoxStartPositionEqualWith(currColumnStartPos);

            if (column.isContentValid && isPositionCorrect && !this.disableReuse) {
                const iteratorNewPosition: number = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.PageArea) + column.getEndPosition();
                
                this.iterator.setPosition(iteratorNewPosition, false);

                this.lastRow = column.getLastRow();
                this.lastRowStartPosition = this.layoutPosition.page.getStartOffsetContentOfMainSubDocument() + this.layoutPosition.pageArea.pageOffset +
                    column.pageAreaOffset + this.lastRow.columnOffset;
                this.lastRowParagraphIndex = LayoutFormatterBase.NEED_RECALCULATE_PARAGRAPH_INDEX;

                this.layoutPosition.rowIndex = column.rows.length;
                this.state = LayoutFormatterState.ColumnEnd;
            }
            else {
                column.pageAreaOffset = 0;
                this.layoutPosition.rowIndex = 0;
                this.state = LayoutFormatterState.RowFormatting;
            }
            return true;
        }
        private tryReuseRow(): LayoutRow {
            const row: LayoutRow = this.layoutPosition.column.rows[this.layoutPosition.rowIndex];
            if (!row || !row.isContentValid || !this.layoutPosition.page.isInnerContentValid)
                return null;

            const rowStartPos: number = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column) + row.columnOffset;
            const isPositionCorrect: boolean = rowStartPos == this.iterator.getPosition() || this.iterator.nextVisibleBoxStartPositionEqualWith(rowStartPos);
            if (!isPositionCorrect || rowStartPos >= this.nextTablePosition)
                return null;
            
            // http://workservices01/OpenWiki/ow.asp?p=ASPxRichEdit_ChangesDescription#A1
            if (this.currColumnHeight != row.y)
                return null;

            this.iterator.setPosition(rowStartPos + row.getLastBoxEndPositionInRow(), false);
            row.isContentValid = true;
            
            return row;
        }

        // removeRedundant
        protected static removeRendundant<T>(fromIndex: number, list: T[], pushChange: (index: number, elem: T) => void) {
            for (let index: number = list.length - 1, elem: T; index >= fromIndex; index--)
                pushChange(index, list.pop());
        }
        private removeRedundantRowsFromColumn(firstRendundantRowIndex: number) {
            LayoutFormatterBase.removeRendundant(firstRendundantRowIndex, this.layoutPosition.column.rows,
                (index: number, elem: LayoutRow) => this.rowChanges.push(new RowChange(LayoutChangeType.Deleted, index, elem)));
        }
        private removeRedundantTableInfosFromColumn() {
            LayoutFormatterBase.removeRendundant(this.currTableColumnInfoIndex, this.layoutPosition.column.tablesInfo,
                (index: number, elem: LayoutTableColumnInfo) => this.tableChanges.push(new TableChange(LayoutChangeType.Deleted, index, elem)));
        }
        private removeRedundantColumnsFromArea(firstRedundantColumnIndex: number) {
            LayoutFormatterBase.removeRendundant(firstRedundantColumnIndex, this.layoutPosition.pageArea.columns,
                (index: number, elem: LayoutColumn) => this.columnChanges.push(new ColumnChange(LayoutChangeType.Deleted, index, elem, [], [], [])));
        }
        
        // addAndRegister
        private isTableRowEquivalent(rowA: LayoutRow, rowB: LayoutRow): boolean {
            if (!(!!rowA.tableCellInfo && !!rowB.tableCellInfo))
                return false;

            if (rowA.x != rowB.x || rowA.y != rowB.y || rowA.width != rowB.width || rowA.columnOffset != rowB.columnOffset ||
                rowA.height != rowB.height || rowA.baseLine != rowB.baseLine || rowA.boxes.length != rowB.boxes.length)
                return false;

            const numBoxes: number = rowA.boxes.length;
            for (let boxIndex: number = 0, box: LayoutBox; boxIndex < numBoxes; boxIndex++)
                if (!rowA.boxes[boxIndex].equals(rowB.boxes[boxIndex]))
                    return false;
            return true;
        }
        private addAndRegisterRowToCurrentColumn(row: LayoutRow) {
            row.isContentValid = true;
            const rowIndex: number = this.layoutPosition.rowIndex;
            let changeType: LayoutChangeType;
            const existingRow: LayoutRow = this.layoutPosition.column.rows[rowIndex];
            if (!existingRow) {
                changeType = LayoutChangeType.Added;
                this.layoutPosition.column.rows.push(row);
            }
            else {
                if (row.columnOffset < existingRow.columnOffset) {
                    changeType = LayoutChangeType.Inserted;
                    this.layoutPosition.column.rows.splice(rowIndex, 0, row);
                }
                else {
                    if (this.isTableRowEquivalent(row, existingRow))
                        return;
                    changeType = LayoutChangeType.Replaced;
                    this.layoutPosition.column.rows[rowIndex] = row;
                }
            }
            this.rowChanges.push(new RowChange(changeType, rowIndex, row));
        }
        private addAndRegisterTableColumnInfoToCurrentColumn(tblCol: LayoutTableColumnInfo) {
            const tblInfos: LayoutTableColumnInfo[] = this.layoutPosition.column.tablesInfo;
            const oldTblCol: LayoutTableColumnInfo = this.layoutPosition.column.tablesInfo[this.currTableColumnInfoIndex];
            let changeType: LayoutChangeType;
            if (!oldTblCol) {
                changeType = LayoutChangeType.Added;
                tblInfos.push(tblCol);
            }
            else {
                //(oldTblCol.logicInfo.grid.table.index == tblCol.logicInfo.grid.table.index) {
                changeType = LayoutChangeType.Replaced;
                this.layoutPosition.column.tablesInfo.splice(this.currTableColumnInfoIndex, 1, tblCol);
            }
            
            this.tableChanges.push(new TableChange(changeType, this.currTableColumnInfoIndex, tblCol));
            this.currTableColumnInfoIndex++;
        }
        private addAndRegisterColumnToCurrentPageArea(column: LayoutColumn) {
            column.isContentValid = true;
            const colIndex: number = this.layoutPosition.columnIndex;
            const existingColumn: LayoutColumn = this.layoutPosition.pageArea.columns[colIndex];
            let changeType: LayoutChangeType;
            if (!existingColumn) {
                changeType = LayoutChangeType.Added;
                this.layoutPosition.pageArea.columns.push(column);
            }
            else if (existingColumn == column)
                changeType = LayoutChangeType.Updated;
            else {
                changeType = LayoutChangeType.Replaced;
                this.layoutPosition.pageArea.columns[colIndex] = column;
            }
            this.columnChanges[this.columnChanges.length - 1].changeType = changeType;
        }
        
        // createEmpty\fake
        public static createEmptyRowWithParagraphBox(iterator: ITextBoxIterator): LayoutRow {
            const box: LayoutBox = LayoutFormatterBase.createParagraphMarkBoxForEmptyRow(iterator);
            const row: LayoutRow = new LayoutRow();
            row.boxes = [box];
            row.x = 0;
            row.y = 0;
            row.width = box.width;
            row.height = box.height;
            row.baseLine = box.getAscent();
            row.columnOffset = 0;
            row.isContentValid = true;
            row.numberingListBox = null;
            row.flags = LayoutRowStateFlags.DocumentEnd;

            return row;
        }
        public static createParagraphMarkBoxForEmptyRow(iterator: ITextBoxIterator): LayoutBox {
            const chunks: Chunk[] = iterator.subDocument.chunks;
            const lastChunk: Chunk = chunks[chunks.length - 1];
            const lastRun: TextRun = lastChunk.textRuns[lastChunk.textRuns.length - 1];
            const box: LayoutParagraphMarkBox = new LayoutParagraphMarkBox(lastRun.getCharacterMergedProperies());

            LayoutBox.initializeWithMeasurer([box], iterator.getMeasurer(), false);

            box.isContentValid = true;
            box.rowOffset = 0;
            box.x = 0
            box.y = 0
            return box;
        }
        
        // tableFormatting
        protected setNextExpectedTable(nextExpectedTableIndex: number) {
            this.nextTableIndex = nextExpectedTableIndex;
            const table: Table = this.iterator.subDocument.tables[nextExpectedTableIndex];
            this.nextTablePosition = table ? table.getStartPosition() : Number.MAX_VALUE;
        }
        private prepareTableFormat() {
            const tableBoxCollector: TableTextBoxCollector = new TableTextBoxCollector(this.iterator.subDocument.tables, this.nextTableIndex, this.iterator);
            const collectorInfo: TableTextBoxCollectorTableInfo[] = tableBoxCollector.tableInfos;

            const oldIterator: ITextBoxIterator = this.iterator;
            this.iterator = new TableBoxIterator(this.iterator.getMeasurer(), this.iterator.subDocument, collectorInfo, this.iterator.getSectionIndex());
            this.rowFormatter.setIterator(this.iterator);

            this.tableBoxCollectorIndex = 0;
            this.setTableColumnsInLayout(this.startTableFormat(collectorInfo, null));

            let lastTopLevelTable;
            const tables: Table[] = oldIterator.subDocument.tables;
            for (let collInfoIndex: number = collectorInfo.length - 1; collInfoIndex >= 0; collInfoIndex--) {
                lastTopLevelTable = collectorInfo[collInfoIndex].tableIndex;
                if (tables[lastTopLevelTable].nestedLevel == 0)
                    break;
            }

            const tblIndex: number = collectorInfo[collectorInfo.length - 1].tableIndex;

            oldIterator.setPosition(tables[lastTopLevelTable].getEndPosition(), false);
            this.iterator = oldIterator;
            this.rowFormatter.setIterator(oldIterator);
            this.setNextExpectedTable(tblIndex + 1);
        }
        private startTableFormat(collectorInfo: TableTextBoxCollectorTableInfo[], parentTableInfo: LayoutTableFormatter): LayoutTableFormatter {
            // in tables no break symbols exclude line break
            const collectorTableInfo: TableTextBoxCollectorTableInfo = collectorInfo[this.tableBoxCollectorIndex];
            const maxTableWidth: number = parentTableInfo ? parentTableInfo.cellEndContentXPosition - parentTableInfo.cellStartContentXPosition :
                LayoutColumn.findSectionColumnWithMinimumWidth(this.columnBounds);

            const tblInfo: LayoutTableFormatter = new LayoutTableFormatter(this.iterator.subDocument,
                collectorTableInfo.tableIndex, maxTableWidth, parentTableInfo, this.unitConverter);

            while (tblInfo.tblPos.moveToNextRow()) {
                const boxCollectorRow: TableTextBoxCollectorCellInfo[] = collectorTableInfo.cells[tblInfo.tblPos.rowIndex];
                tblInfo.initRowProps();
                while (tblInfo.tblPos.moveToNextCell()) {
                    const boxCollectorCell: TableTextBoxCollectorCellInfo = boxCollectorRow[tblInfo.tblPos.cellIndex];
                    tblInfo.initCellProps();
                    switch (boxCollectorCell.cellType) {
                        case TableTextBoxCollectorCellInfoType.Hidden: // set fake box
                            const box: LayoutBox = LayoutFormatterBase.createParagraphMarkBoxForEmptyRow(this.iterator);
                            box.rowOffset = tblInfo.tblPos.cell.startParagraphPosition.value;
                            var boxAbsolOffset: number = box.rowOffset;
                            const parIndex: number = Utils.normedBinaryIndexOf(this.iterator.subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - boxAbsolOffset);
                            boxCollectorCell.paragraphsInfo = [new TableTextBoxCollectorCellParagraphInfo(parIndex, box)];
                        case TableTextBoxCollectorCellInfoType.Merged:
                            continue; // draw as cell above
                    }
                    for (let parInfo of boxCollectorCell.paragraphsInfo) {
                        if (parInfo.parIndex < 0) {
                            this.tableBoxCollectorIndex++;
                            tblInfo.addInternalCellTable(this.startTableFormat(collectorInfo, tblInfo));
                        }
                        else {
                            this.iterator.setTableCellParagraphBoxes(parInfo.boxes, parInfo.parIndex);
                            while (this.createTableRow(tblInfo));
                        }
                    }
                }
            }
            tblInfo.tblPos = new TablePosition(tblInfo.tblPos.table, 0, 0).init();
            return tblInfo;
        }
        private setTableColumnsInLayout(tblFormatter: LayoutTableFormatter) {
            //Logging.print(LogSource.LayoutFormatter, "setTableColumnsInLayout", ["tableChanges", LoggingToString.list, [this.tableChanges, LoggingToString.tableChange, 1]]);
            const tables: Table[] = this.iterator.subDocument.tables;
            this.disableReuse = true;
            while (true) {
                const thisColumnEmpty: boolean = this.currColumnHeight == 0;
                const avaliableColumnHeight: number = this.columnBounds[this.layoutPosition.columnIndex].height - this.currColumnHeight;
                const tableColumns: LayoutTableColumnInfo[] = tblFormatter.getTableColumns(avaliableColumnHeight, thisColumnEmpty, this.currColumnHeight, null);

                if (!tableColumns.length && !tblFormatter.isFormattingEnd) {
                    this.goToStateRowFormatting();
                    this.layoutPosition.page.contentIntervals = [];
                    continue;
                }
                const pageIntervals: FixedInterval[] = this.layoutPosition.page.contentIntervals;
                this.extendPageIntervals(tableColumns[0], pageIntervals[pageIntervals.length - 1]);

                const layoutRows: LayoutRowWithIndex[] = [];
                for (let tblCol of tableColumns) {
                    for (let tblRow of tblCol.tableRows)
                        for (let tblCell of tblRow.rowCells) {
                            for (let layoutRowInfo of tblCell.layoutRows)
                                layoutRows.push(layoutRowInfo);
                        }
                    this.addAndRegisterTableColumnInfoToCurrentColumn(tblCol);
                }

                layoutRows.sort((a: LayoutRowWithIndex, b: LayoutRowWithIndex) => a.boxes[0].rowOffset - b.boxes[0].rowOffset);
               
                const pageOffset: number = this.iterator.subDocument.isMain() ? this.layoutPosition.page.getStartOffsetContentOfMainSubDocument() : 0;
                for (let rowInfo of layoutRows) {
                    const row: LayoutRow = rowInfo;
                    rowInfo.indexInColumn = this.layoutPosition.rowIndex;

                    const firstBoxInRowOffset: number = row.boxes[0].rowOffset;
                    row.columnOffset = firstBoxInRowOffset - pageOffset - this.layoutPosition.pageArea.pageOffset - this.layoutPosition.column.pageAreaOffset;
                    for (let box of row.boxes)
                        box.rowOffset -= firstBoxInRowOffset;

                    this.addAndRegisterRowToCurrentColumn(row);
                    this.layoutPosition.rowIndex++;
                }

                this.currColumnHeight = tableColumns[0].getBottomBoundPosition();

                const prevPage: LayoutPage = this.layoutPosition.page;
                if (tblFormatter.isFormattingEnd)
                    break;
                else
                    this.goToStateRowFormatting();
                this.layoutPosition.page.contentIntervals = [];
                if (this.bindNextPage) {
                    this.layoutPosition.page.firstPageInGroup = prevPage.firstPageInGroup;
                    this.bindNextPage = false;
                }
            }
            this.bindNextPage = false;
            this.disableReuse = false;
        }
        private extendPageIntervals(tableColumn: LayoutTableColumnInfo, currPageInterval: FixedInterval): FixedInterval {
            const pageIntervals: FixedInterval[] = this.layoutPosition.page.contentIntervals;
            for (let tblRowIndex: number = 0, tblRow: LayoutTableRowInfo; tblRow = tableColumn.tableRows[tblRowIndex]; tblRowIndex++) {
                for (let tblCellIndex: number = 0, tblCell: LayoutTableCellInfo; tblCell = tblRow.rowCells[tblCellIndex]; tblCellIndex++) {
                    const isStartOnPreviousColumns: boolean = !(tblCell.cellBoundFlags & TableCellBoundFlags.StartOnThisColumn);
                    const isEndOnNextColumns: boolean = !(tblCell.cellBoundFlags & TableCellBoundFlags.EndOnThisColumn);

                    const internalTableBeforeFirstLayoutRow: LayoutTableColumnInfo = tblCell.internalTables[0];
                    if (internalTableBeforeFirstLayoutRow)
                        currPageInterval = this.extendPageIntervals(internalTableBeforeFirstLayoutRow, currPageInterval);
                    else {
                        if (!currPageInterval || isStartOnPreviousColumns) {
                            currPageInterval = new FixedInterval(tblCell.layoutRows[0].boxes[0].rowOffset, 0);
                            pageIntervals.push(currPageInterval);
                        }
                    }
                    
                    const internalTableAfterLastLayoutRow: LayoutTableColumnInfo = tblCell.internalTables[tblCell.layoutRows.length];
                    if (internalTableAfterLastLayoutRow) {
                        if (internalTableAfterLastLayoutRow != internalTableBeforeFirstLayoutRow)
                            currPageInterval = this.extendPageIntervals(internalTableAfterLastLayoutRow, currPageInterval);
                    }
                    else {
                        if (isEndOnNextColumns || isStartOnPreviousColumns) {
                            const lastLayoutRowEndPos: number = tblCell.getLastLayoutRow().getLastBox().getEndPosition();
                            if (!currPageInterval)
                                currPageInterval = pageIntervals[pageIntervals.length - 1];
                            currPageInterval.length = lastLayoutRowEndPos - currPageInterval.start;
                            currPageInterval = null;
                            if (!this.bindNextPage) {
                                this.layoutPosition.page.firstPageInGroup = this.layoutPosition.page;
                                this.bindNextPage = true;
                            }
                        }
                    }
                }
            }
            if (tableColumn.logicInfo.grid.table.nestedLevel == 0) {
                let prevInterval: FixedInterval = pageIntervals[0];
                for (let intervalIndex: number = 1, interval: FixedInterval; interval = pageIntervals[intervalIndex]; intervalIndex++) {
                    if (prevInterval.end() == interval.start) {
                        pageIntervals.splice(intervalIndex, 1);
                        intervalIndex--;
                        prevInterval.length = interval.end() - prevInterval.start;
                    }
                    else
                        prevInterval = interval;
                }
                prevInterval.length = tableColumn.getLastTableRow().getLastRowCell().getEndPosition() - prevInterval.start;
            }
            return currPageInterval;
        }
        private goToStateRowFormatting() {
            this.state = LayoutFormatterState.ColumnEnd;
            while (this.stateMap[this.state].call(this) && this.state != LayoutFormatterState.RowFormatting);
        }
        private createTableRow(tblInfo: LayoutTableFormatter): boolean {
            this.rowFormatter.init(!this.lastRow || !!(this.lastRow.flags & LayoutRowStateFlags.ParagraphEnd), tblInfo.cellStartContentXPosition, tblInfo.cellEndContentXPosition);
            this.rowFormatter.formatRow();

            let row: LayoutRow = this.rowFormatter.row;
            this.copyRowBoxes(row);

            this.rowFormatter.applyRowHorizontalAlign();
            this.rowFormatter.applyLineSpacing();

            const paragraphIndex: number = this.rowFormatter.paragraphIndex;
            const paragraphs: Paragraph[] = this.iterator.subDocument.paragraphs;
            row.applySpacingBefore(LayoutRow.getParagraphSpacingBefore(paragraphs[paragraphIndex], paragraphs[paragraphIndex - 1], this.unitConverter));
            row.applySpacingAfter(row.flags & LayoutRowStateFlags.ParagraphEnd ?
                LayoutRow.getParagraphSpacingAfter(paragraphs[paragraphIndex], paragraphs[paragraphIndex + 1], this.unitConverter) :
                0);

            tblInfo.addLayoutRow(row);

            this.lastRow = row;
            this.lastRowParagraphIndex = paragraphIndex;

            return !(row.flags & LayoutRowStateFlags.CellTableEnd);
        }

        // others
        protected getLastRowParagraphIndex(): number {
            if (this.lastRowParagraphIndex == LayoutFormatterBase.NEED_RECALCULATE_PARAGRAPH_INDEX)
                this.lastRowParagraphIndex = Utils.normedBinaryIndexOf(this.iterator.subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - this.lastRowStartPosition);
            return this.lastRowParagraphIndex;
        }
        protected finalizePageArea() {
            this.removeRedundantColumnsFromArea(this.layoutPosition.columnIndex + 1);

            const createdPageArea: LayoutPageArea = this.layoutPosition.pageArea;
            createdPageArea.isContentValid = true;

            if (this.iterator.subDocument.isMain()) {
                // normalize offsets
                const offsetFirstColumnFromPageArea: number = createdPageArea.columns[0].pageAreaOffset;
                if (offsetFirstColumnFromPageArea > 0) {
                    createdPageArea.pageOffset += offsetFirstColumnFromPageArea;
                    for (let column of createdPageArea.columns)
                        column.pageAreaOffset -= offsetFirstColumnFromPageArea;
                }
            }
        }
        private createRow(): LayoutRow {
            const columnBounds: Rectangle = this.columnBounds[this.layoutPosition.columnIndex];
            this.rowFormatter.init(!this.lastRow || !!(this.lastRow.flags & LayoutRowStateFlags.ParagraphEnd), 0, columnBounds.width);
            this.rowFormatter.formatRow();
            let row: LayoutRow = this.rowFormatter.row;

            if (row.flags & LayoutRowStateFlags.NotEnoughChunks)
                return null;

            if ((row.flags & LayoutRowStateFlags.DocumentEnd) && !(row.flags & LayoutRowStateFlags.ParagraphEnd))
                return null;


            if (row.boxes[0].rowOffset >= this.nextTablePosition) {
                this.prepareTableFormat(); // here start format table mode
                this.startRowPosition = this.iterator.getPosition();
                return this.createRow();
            }
            else {
                this.copyRowBoxes(row);
                this.rowFormatter.applyRowHorizontalAlign();
                this.rowFormatter.applyLineSpacing();
                return row;
            }
        }
        private copyRowBoxes(row: LayoutRow) {
            for (let i = 0, box: LayoutBox; box = row.boxes[i]; i++) {
                if (box.getType() == LayoutBoxType.LayoutDependent) {
                    (<LayoutDependentTextBox>box).calculateText(this);
                    let oldWidth = box.width;
                    LayoutBox.initializeWithMeasurer([box], this.iterator.getMeasurer(), false);
                    let widthDiff = box.width - oldWidth;
                    row.width += widthDiff;
                    for (let j = i + 1, nextBox: LayoutBox; nextBox = row.boxes[j]; j++)
                        nextBox.x += widthDiff;
                }
            }

            // don't copy numbering boxes - there no rowOffset
            const boxesCopy: LayoutBox[] = [];
            for (let box of row.boxes)
                boxesCopy.push(box.clone());
            row.boxes = boxesCopy;
        }

        private updateColumnParagraphFrames() {
            const column: LayoutColumn = this.layoutPosition.column;
            const oldParFrames: ParagraphFrame[] = column.paragraphFrames;
            column.paragraphFrames = [];
            
            const rows: LayoutRow[] = column.rows;
            const columnPos: number = this.layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Column);
            const firstRowStartPos: number = columnPos + rows[0].columnOffset;
            const paragraphs: Paragraph[] = this.iterator.subDocument.paragraphs;
            let parIndex: number = Utils.normedBinaryIndexOf(paragraphs, (p: Paragraph) => p.startLogPosition.value - firstRowStartPos);
            let paragraph: Paragraph = paragraphs[parIndex];
            let parInterval: FixedInterval = paragraph.getInterval();
            let prevParIndex: number = parIndex - 1;
            let parProps: ParagraphProperties = paragraphs[parIndex].getParagraphMergedProperies();
            let parFrame: ParagraphFrame;
            for (let row of rows) {
                const rowPos: number = columnPos + row.columnOffset;
                while (true) {
                    if (parInterval.containsPositionWithoutIntervalEnd(rowPos)) {
                        if (parIndex != prevParIndex) {
                            parFrame = new ParagraphFrame();
                            column.paragraphFrames.push(parFrame);

                            parFrame.paragraphColor = parProps.backColor;
                            
                            parFrame.x = this.unitConverter.toPixels(parProps.leftIndent -
                                (parProps.firstLineIndentType == ParagraphFirstLineIndent.Hanging ? parProps.firstLineIndent : 0)) + row.x;
                            parFrame.width = row.getRightBoundPosition() - this.unitConverter.toPixels(parProps.rightIndent) - parFrame.x;

                            parFrame.y = row.y;
                            parFrame.height = row.height;
                        }
                        else
                            parFrame.height = row.getBottomBoundPosition() - parFrame.y;

                        prevParIndex = parIndex;
                        break;
                    }
                    else {
                        parIndex++;
                        paragraph = paragraphs[parIndex];
                        parInterval = paragraph.getInterval();
                        parProps = paragraph.getParagraphMergedProperies();
                    }
                }
            }
            this.setParagraphFrameChanges(oldParFrames, column.paragraphFrames);
        }

        private setParagraphFrameChanges(oldParFrames: ParagraphFrame[], newParFrames: ParagraphFrame[]) {
            const parFrameChanges: ParagraphFrameChange[] = [];
            for (let frameIndex: number = 0; frameIndex < newParFrames.length; frameIndex++) {
                const newParFrame: ParagraphFrame = newParFrames[frameIndex];
                const oldParFrame: ParagraphFrame = oldParFrames[frameIndex];
                if (!oldParFrames)
                    parFrameChanges.push(new ParagraphFrameChange(LayoutChangeType.Added, frameIndex, newParFrame));
                else {
                    if (!newParFrame.equals(oldParFrame))
                        parFrameChanges.push(new ParagraphFrameChange(LayoutChangeType.Replaced, frameIndex, newParFrame));
                }
            }
            for (let frameIndex: number = newParFrames.length; frameIndex < oldParFrames.length; frameIndex++)
                parFrameChanges.push(new ParagraphFrameChange(LayoutChangeType.Deleted, frameIndex, oldParFrames[frameIndex]));

            this.columnChanges[this.columnChanges.length - 1].paragraphFrameChanges = parFrameChanges;
        }
    }
} 