module __aspxRichEdit {
    //type MarginMergerConstructor = new (table: Table, model: DocumentModel) => TableCellPropertiesMergerMarginBase;
    class LayoutTableFormatterLayoutCell {
        layoutRow: LayoutRow[] = []; // if no rows then it vertMergingContinue, if null, then this - nested table
        cellStartXPosition: number;
        cellEndXPosition: number;
        cellAvaliableWidth: number;
        internalCellTable: { [layoutRowIndex: number]: LayoutTableFormatter } = {}; // layoutRowIndex -before row with this index - inner table
    }

    class TopAndBottomMarginsForRow {
        // because MS Word set max of top or bottom margin in row
        topMargin: number = 0;
        bottomMargin: number = 0;
        
        addCellTopMargin(topMargin: number) {
            if (this.topMargin < topMargin)
                this.topMargin = topMargin;
        }

        addCellBottomMargin(bottomMargin: number) {
            if (this.bottomMargin < bottomMargin)
                this.bottomMargin = bottomMargin;
        }
    }

    class TableFormatterRowIndexes {
        layoutRowIndexes: number[] = []; // get by cellIndex
        isRowRendered: boolean = false;
        numPages: number = 0; // on how many pages takes.
        numFullyFormattedCells: number = 0;
        constructor(cellNum: number) {
            for (; cellNum > 0; cellNum--)
                this.layoutRowIndexes.push(-1);
        }
    }
    
    class LayoutFormatterTableRowHeightInfo {
        public horizontalAlignment: TableRowAlignment; // can be null
        public preferredHeightValue: number;
        public preferredHeightType: TableHeightUnitType;
        public contentHeight: number;
        public cantSplit: boolean;

        constructor(cantSplit: boolean, height: TableHeightUnit, horizontalAlignment: TableRowAlignment, unitConverter: IUnitConverter) {
            this.preferredHeightValue = unitConverter.toPixels(height.value);
            this.preferredHeightType = height.type;
            this.contentHeight = 0;
            this.horizontalAlignment = horizontalAlignment;
        }
    }

    enum PushResult {
        AllObjectsPlaced = 0,
        PartObjectsPlaced = 1, // because content height too big
        NoObjectsPlaced = 2
    }

    export class LayoutTableFormatter {
        public tblPos: TablePosition;
        public cellStartContentXPosition: number;
        public cellEndContentXPosition: number;

        private subDocument: SubDocument;
        private layoutTableInfo: LayoutTableInfo;
        private unitConverter: IUnitConverter;
        private grid: TableGrid;
        private bordersHelper: LayoutTableBorderHelper;
        private tableXPositionStart: number;
        private rowCellSpacings: number[] = [];
        private rowsHeight: LayoutFormatterTableRowHeightInfo[] = []; // for logic rows
        private layoutRowsInfo: LayoutTableFormatterLayoutCell[][] = []; // collect rows on first step
        private verticalBorders: LayoutTableBorder[][][] = [];
        private topAndBottomMarginsForRow: TopAndBottomMarginsForRow[] = [];

        private avaliableColumnHeight: number;
        private columns: LayoutTableColumnInfo[];

        private currTableColumnInfo: LayoutTableColumnInfo;
        private currTableRowInfo: LayoutTableRowInfo;
        private currTableCellInfo: LayoutTableCellInfo;

        private currColumnHorizontalBorders: LayoutTableHorizontalLineBordersInfo[][];
        private currColumnVerticalBorders: LayoutTableBorder[][][];
        private currRowHorizontalBorders: LayoutTableHorizontalLineBordersInfo[];
        private layoutRowInCellIndexes: TableFormatterRowIndexes[]; // indexes current layout rows in layoutRowsInfo; first index - rowIndex

        private isThisColumnFirstInTable: boolean = true;

        public isFormattingEnd: boolean = false;
        private modelRowIndex: number = 0;
        private rowCellStartHeight: number;
        private rowCellEndHeight: number;

        private isPageAreaColumnEmpty: boolean;
        private prevIndexesState: number[];

        private tableMaxWidth: number;
        private tableHorizontalAlignment: TableRowAlignment;

        constructor(subDocument: SubDocument, tableIndex: number, tableMaxWidth: number, parentTableInfo: LayoutTableFormatter, unitConverter: IUnitConverter) {
            const table: Table = subDocument.tables[tableIndex];
            this.subDocument = subDocument;
            this.tableMaxWidth = tableMaxWidth;
            this.tblPos = new TablePosition(table, -1, -1);
            this.unitConverter = unitConverter;

            const model: DocumentModel = subDocument.documentModel;
            const tableStyle: TableStyle = table.style;
            const tblProps: TableProperties = table.properties;
            const defaultTblProps: TableProperties = model.defaultTableProperties;
            const defaultTblRowProps: TableRowProperties = model.defaultTableRowProperties;

            this.grid = TableGrid.getTableGrid(this.subDocument, tableIndex, tableMaxWidth, this.unitConverter);
            this.grid.initTableCellGridInfo();

            const tableIndent: number = new TablePropertiesMergerIndent()
                .getProperty(tblProps, tableStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps)
                .asNumberNoPercentType(this.unitConverter);
            this.tableXPositionStart = (parentTableInfo ? parentTableInfo.cellStartContentXPosition : 0) + tableIndent;

            const tableBackColor: number = new TablePropertiesMergerBackgroundColor().getProperty(tblProps, tableStyle, ConditionalTableStyleFormatting.WholeTable,
                defaultTblProps);
            this.layoutTableInfo = new LayoutTableInfo(new LayoutTableProperties(tableBackColor), this.grid);

            for (let rowIndex: number = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                const cantSplit: boolean = new TableRowPropertiesMergerCantSplit().getProperty(row.properties, tableStyle, row.conditionalFormatting, defaultTblRowProps);
                const horizontalAlignment: TableRowAlignment = new TableRowPropertiesMergerHorizontalAlignment()
                    .getProperty(row.properties, tableStyle, row.conditionalFormatting, defaultTblRowProps);
                this.rowsHeight.push(new LayoutFormatterTableRowHeightInfo(cantSplit, row.height, horizontalAlignment, this.unitConverter));
                this.rowCellSpacings.push(new TableRowPropertiesMergerCellSpacing(model, table).getProperty(row.properties, tableStyle, row.conditionalFormatting, defaultTblRowProps)
                    .asNumberNoPercentType(this.unitConverter));

                const rowTopAndBottomMargins: TopAndBottomMarginsForRow = new TopAndBottomMarginsForRow();
                this.topAndBottomMarginsForRow.push(rowTopAndBottomMargins);
                for (let cell of row.cells) {
                    if (cell.verticalMerging == TableCellMergingState.Continue)
                        continue;
                    rowTopAndBottomMargins.addCellTopMargin(this.getCellMargin(cell, new TableCellPropertiesMergerMarginTop(this.tblPos.table, model)));
                    rowTopAndBottomMargins.addCellBottomMargin(this.getCellMargin(cell, new TableCellPropertiesMergerMarginBottom(this.tblPos.table, model)));
                }
            }

            this.bordersHelper = new LayoutTableBorderHelper(this.grid, this.rowCellSpacings, model.defaultTableProperties, this.unitConverter);
            this.verticalBorders = this.bordersHelper.getVerticalBorders();

            this.layoutRowInCellIndexes = [];
            for (let row of this.grid.table.rows)
                this.layoutRowInCellIndexes.push(new TableFormatterRowIndexes(row.cells.length));

            this.tableHorizontalAlignment = new TablePropertiesMergerHorizontalAlignment()
                .getProperty(tblProps, tableStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);
        }

        private getCellMargin(cell: TableCell, marginMerger: TableCellPropertiesMergerMarginBase): number {
            const tableStyle: TableStyle = this.tblPos.table.style;
            const cellProps: TableCellProperties = cell.properties;
            const styleFormatting: ConditionalTableStyleFormatting = cell.conditionalFormatting;
            return marginMerger.getProperty(cellProps, tableStyle, styleFormatting, null).asNumberNoPercentType(this.unitConverter);
        }

        public initRowProps() {
            this.layoutRowsInfo.push([]);
            this.tblPos.cellIndex = -1;
        }

        public initCellProps() {
            const cell: TableCell = this.tblPos.cell;

            const rowIndex: number = this.tblPos.rowIndex;
            const cellIndex: number = this.tblPos.cellIndex;

            const layoutTableFormatterLayoutCell: LayoutTableFormatterLayoutCell = new LayoutTableFormatterLayoutCell();
            this.layoutRowsInfo[rowIndex].push(layoutTableFormatterLayoutCell);

            if (cell.verticalMerging == TableCellMergingState.Continue)
                return;

            const model: DocumentModel = this.subDocument.documentModel;

            const isFirstCellInRow: boolean = cellIndex == 0;
            const isLastCellInRow: boolean = this.tblPos.row.cells.length - 1 == cellIndex;

            const cellInfo: TableCellGridInfo = this.grid.tableCellInfos[rowIndex][cellIndex];
            const rowCellSpacing: number = this.rowCellSpacings[rowIndex];

            const cellMarginLeft: number = this.getCellMargin(cell, new TableCellPropertiesMergerMarginLeft(this.tblPos.table, model));
            const cellMarginRight: number = this.getCellMargin(cell, new TableCellPropertiesMergerMarginRight(this.tblPos.table, model));

            layoutTableFormatterLayoutCell.cellStartXPosition = this.tableXPositionStart + this.grid.columnsXPositions[cellInfo.getGridCellIndex()];
            layoutTableFormatterLayoutCell.cellEndXPosition = this.tableXPositionStart + this.grid.columnsXPositions[cellInfo.getGridCellIndex() + cell.columnSpan];

            const verticalBordersCurrCell: LayoutTableBorder[] = this.verticalBorders[rowIndex][cellIndex];
            const brdCellLeft: LayoutTableBorder = verticalBordersCurrCell[rowCellSpacing > 0 && isFirstCellInRow ? 1 : 0];

            this.cellStartContentXPosition = Math.max(layoutTableFormatterLayoutCell.cellStartXPosition + cellMarginLeft + rowCellSpacing * (isFirstCellInRow ? 2 : 1),
                brdCellLeft.borderInfo ? brdCellLeft.xPos + brdCellLeft.borderInfo.width : 0);

            let brdCellRight: LayoutTableBorder;
            if (rowCellSpacing > 0 || isLastCellInRow)
                brdCellRight = verticalBordersCurrCell[verticalBordersCurrCell.length - (rowCellSpacing > 0 && isLastCellInRow ? 2 : 1)];
            else
                brdCellRight = this.verticalBorders[rowIndex][cellIndex + 1][0];

            this.cellEndContentXPosition = Math.min(layoutTableFormatterLayoutCell.cellEndXPosition - (cellMarginRight + rowCellSpacing * (isLastCellInRow ? 2 : 1)),
                brdCellRight.borderInfo ? brdCellRight.xPos + this.tableXPositionStart : Number.MAX_VALUE);

            layoutTableFormatterLayoutCell.cellAvaliableWidth = this.cellEndContentXPosition - this.cellStartContentXPosition;
        }

        public addLayoutRow(row: LayoutRow) {
            this.layoutRowsInfo[this.tblPos.rowIndex][this.tblPos.cellIndex].layoutRow.push(row);
        }

        public addInternalCellTable(intervalTableInfo: LayoutTableFormatter) {
            const cellInfo: LayoutTableFormatterLayoutCell = this.layoutRowsInfo[this.tblPos.rowIndex][this.tblPos.cellIndex];
            cellInfo.internalCellTable[cellInfo.layoutRow.length] = intervalTableInfo;
            cellInfo.layoutRow.push(null);
        }   
        
        // return columnS (columnS because here also inner tables)
        public getTableColumns(avaliableColumnHeight: number, isPageAreaColumnEmpty: boolean, yPos: number, parentLayoutTableCellInfo: LayoutTableCellInfo): LayoutTableColumnInfo[] {
            if (this.isFormattingEnd)
                return [];

            this.currColumnHorizontalBorders = [];
            this.currColumnVerticalBorders = [];
            this.columns = [];
            this.isPageAreaColumnEmpty = isPageAreaColumnEmpty; // need write at least something
            this.avaliableColumnHeight = avaliableColumnHeight;
            this.currTableColumnInfo = new LayoutTableColumnInfo(parentLayoutTableCellInfo, this.layoutTableInfo,
                new Rectangle().init(this.tableXPositionStart, yPos, this.grid.columnsXPositions[this.grid.columnsXPositions.length - 1], 0));

            this.tblPos.rowIndex = -1;

            while (this.getNextRow()) {
                this.applyBottomHorizontalBordersChangesForPrevRow();

                this.currColumnHorizontalBorders.push(this.currRowHorizontalBorders);

                this.updatePrevRowsIndexes();

                this.currTableColumnInfo.tableRows.push(this.currTableRowInfo);
                this.setRowHeight();
                this.extendCellHeightToRow();

                this.layoutRowInCellIndexes[this.modelRowIndex].numPages++;
            }

            if (this.currTableColumnInfo.tableRows.length) {
                this.setColumnHorizontalBorders();
                this.setColumnVerticalBorders();

                this.columns.unshift(this.currTableColumnInfo);
                this.isThisColumnFirstInTable = false;

                this.currTableColumnInfo.height = this.currTableRowInfo.bound.getBottomBoundPosition() - this.currTableColumnInfo.y;

                this.setRowsVerticalBounds();
                this.setBackgroundInfos();

                this.currTableColumnInfo.horizontalBorders = LayoutTableFormatterBorderMerger.getFinalReducedHorizontalBorders(this.currColumnHorizontalBorders);
                this.currTableColumnInfo.verticalBorders = LayoutTableFormatterBorderMerger.getFinalReducedVerticalBorders(this.currColumnVerticalBorders);

                this.applyHorizontalAlignment();
                this.applyCellsVerticalAlignment()
            }

            this.isFormattingEnd = this.getFirstNotFullyFormattedRowIndex(0) == this.tblPos.table.rows.length;
            return this.columns;
        }
        
        private extendCellHeightToRow() {
            const rowHeight: number = this.currTableRowInfo.bound.height;
            for (let cellInfo of this.currTableRowInfo.rowCells)
                cellInfo.bound.height = Math.max(cellInfo.bound.height, rowHeight);
        }

        private updatePrevRowsIndexes() {
            for (let rowIndex: number = 0; rowIndex <= this.modelRowIndex; rowIndex++)
                if (this.getFirstNotFullyFormattedCellIndex(rowIndex, 0) == this.grid.table.rows[rowIndex].cells.length)
                    this.layoutRowInCellIndexes[rowIndex].isRowRendered = true;
        }

        private getFirstNotFullyFormattedRowIndex(startRowIndex: number): number {
            const rowLength: number = this.tblPos.table.rows.length;
            for (; startRowIndex < rowLength; startRowIndex++)
                if (!this.layoutRowInCellIndexes[startRowIndex].isRowRendered)
                    break;
            return startRowIndex;
        }

        private getFirstNotFullyFormattedCellIndex(rowIndex: number, cellIndex: number): number {
            const rowLayoutCellsIndexes: number[] = this.layoutRowInCellIndexes[rowIndex].layoutRowIndexes;
            const rowLayoutCells: LayoutTableFormatterLayoutCell[] = this.layoutRowsInfo[rowIndex];
            for (; cellIndex < rowLayoutCellsIndexes.length; cellIndex++)
                if (rowLayoutCellsIndexes[cellIndex] < rowLayoutCells[cellIndex].layoutRow.length)
                    return cellIndex;
            return cellIndex;
        }

        private getNextRow(): boolean {
            this.tblPos.setRow(this.getFirstNotFullyFormattedRowIndex(this.tblPos.rowIndex + 1));
            if (!this.tblPos.row)
                return false;

            const lastRowInColumn: LayoutTableRowInfo = this.currTableColumnInfo.getLastTableRow();
            const rowYPos: number = lastRowInColumn ? lastRowInColumn.bound.getBottomBoundPosition() : this.currTableColumnInfo.y;
            const prevValueModelRowIndex: number = this.modelRowIndex;
            if (this.tblPos.rowIndex > this.modelRowIndex)
                this.modelRowIndex = this.tblPos.rowIndex;
            this.currTableRowInfo = new LayoutTableRowInfo(this.currTableColumnInfo, new Rectangle().init(-1, rowYPos, 0, 0), this.modelRowIndex);

            if (this.layoutRowInCellIndexes[this.tblPos.rowIndex].layoutRowIndexes[0] == -1 &&
                this.rowsHeight[this.tblPos.rowIndex].preferredHeightType != TableHeightUnitType.Auto &&
                this.currTableColumnInfo.y + this.avaliableColumnHeight - rowYPos < this.rowsHeight[this.tblPos.rowIndex].preferredHeightValue &&
                (!this.isPageAreaColumnEmpty || this.currTableColumnInfo.tableRows.length > 0)) {
                this.modelRowIndex = prevValueModelRowIndex;
                return false; // low height for row, but set min height and column not empty or have at least one row
            }

            const rowCanBeSplit: boolean = !(this.rowsHeight[this.tblPos.rowIndex].cantSplit && !this.isPageAreaColumnEmpty);
            this.setRowHorizontalBorders(this.tblPos.rowIndex);

            this.tblPos.cellIndex = -1;
            this.storePrevIndexState();
            while (true) {
                this.tblPos.setCell(this.getFirstNotFullyFormattedCellIndex(this.tblPos.rowIndex, this.tblPos.cellIndex + 1));
                if (!this.tblPos.cell) {
                    this.tblPos.setRow(this.getFirstNotFullyFormattedRowIndex(this.tblPos.rowIndex + 1));
                    if (this.tblPos.rowIndex > this.modelRowIndex) {
                        this.tblPos.rowIndex = this.modelRowIndex;
                        return true;
                    }
                    this.storePrevIndexState();
                    this.tblPos.cellIndex = -1;
                    this.tblPos.setCell(this.getFirstNotFullyFormattedCellIndex(this.tblPos.rowIndex, this.tblPos.cellIndex + 1));
                }

                if (this.tblPos.cell.verticalMerging == TableCellMergingState.Continue) {
                    this.layoutRowInCellIndexes[this.tblPos.rowIndex].layoutRowIndexes[this.tblPos.cellIndex] = 0;
                    continue;
                }

                switch (this.getCell()) {
                    case PushResult.AllObjectsPlaced:
                        this.currTableRowInfo.rowCells.push(this.currTableCellInfo);
                        break;
                    case PushResult.PartObjectsPlaced:
                        if (rowCanBeSplit)
                            this.currTableRowInfo.rowCells.push(this.currTableCellInfo);
                        else {
                            this.layoutRowInCellIndexes[this.tblPos.rowIndex].layoutRowIndexes = this.prevIndexesState;
                            this.modelRowIndex = prevValueModelRowIndex;
                            return false;
                        }
                        break;
                    case PushResult.NoObjectsPlaced:
                        this.layoutRowInCellIndexes[this.tblPos.rowIndex].layoutRowIndexes = this.prevIndexesState;
                        this.modelRowIndex = prevValueModelRowIndex;
                        return false;
                }
            }
        }

        private storePrevIndexState() {
            this.prevIndexesState = [];
            for (let ind of this.layoutRowInCellIndexes[this.tblPos.rowIndex].layoutRowIndexes)
                this.prevIndexesState.push(ind);
        }

        private setRowHorizontalBorders(rowIndex: number) {
            const rowCellSpacing: number = this.rowCellSpacings[rowIndex];
            this.currRowHorizontalBorders = this.bordersHelper.getHorizontalBordersByRow(rowIndex, this.currTableColumnInfo.tableRows.length == 0, true);
            const isThisRowFirstInColumn: boolean = this.currTableColumnInfo.tableRows.length == 0;
            const isThisRowLastInTable: boolean = rowIndex == this.grid.table.rows.length - 1;
            const horBrdLastIndex: number = this.currRowHorizontalBorders.length - 1;
            this.rowCellStartHeight = this.currRowHorizontalBorders[0].maxWidth;
            if (rowCellSpacing > 0) {
                this.rowCellStartHeight += rowCellSpacing * (this.isThisColumnFirstInTable ? 2 : 1) + (isThisRowFirstInColumn ? this.currRowHorizontalBorders[1].maxWidth : 0);
                this.rowCellEndHeight = this.currRowHorizontalBorders[horBrdLastIndex].maxWidth + this.currRowHorizontalBorders[horBrdLastIndex - 1].maxWidth +
                (isThisRowLastInTable ? 2 : 1) * rowCellSpacing;
            }
            else
                this.rowCellEndHeight = this.currRowHorizontalBorders[horBrdLastIndex].maxWidth;
        }

        private getAvaliableSpaceForCellContent(): number {
            return this.avaliableColumnHeight + this.currTableColumnInfo.y - this.currTableCellInfo.bound.getBottomBoundPosition() - this.rowCellEndHeight;
        }

        private getCell(): PushResult {
            const rowIndex: number = this.tblPos.rowIndex;
            const cellIndex: number = this.tblPos.cellIndex;
            const currCellLayoutRowIndexes: number[] = this.layoutRowInCellIndexes[rowIndex].layoutRowIndexes;
            const isFirstRowInTable: boolean = rowIndex == 0;
            const cellInfo: LayoutTableFormatterLayoutCell = this.layoutRowsInfo[rowIndex][cellIndex];
            const cellGridInfo: TableCellGridInfo = this.grid.tableCellInfos[rowIndex][cellIndex];
            const rowMargins: TopAndBottomMarginsForRow = this.topAndBottomMarginsForRow[rowIndex];

            this.rowCellStartHeight += rowMargins.topMargin;
            this.rowCellEndHeight += rowMargins.bottomMargin;

            const layoutCellBounds: Rectangle = new Rectangle()
                .init(cellInfo.cellStartXPosition, this.currTableRowInfo.bound.y, cellInfo.cellEndXPosition - cellInfo.cellStartXPosition, this.rowCellStartHeight);

            this.currTableCellInfo = new LayoutTableCellInfo(this.currTableRowInfo, layoutCellBounds, cellGridInfo.getGridCellIndex(), cellInfo.cellAvaliableWidth);

            if (currCellLayoutRowIndexes[cellIndex] == -1)
                this.currTableCellInfo.cellBoundFlags |= TableCellBoundFlags.StartOnThisColumn;

            const pushResult: PushResult = this.pushLayoutRowsIntoCell();
            if (currCellLayoutRowIndexes[cellIndex] == cellInfo.layoutRow.length)
                this.currTableCellInfo.cellBoundFlags |= TableCellBoundFlags.EndOnThisColumn;

            this.currTableCellInfo.bound.height += this.rowCellEndHeight;

            this.rowCellStartHeight -= rowMargins.topMargin;
            this.rowCellEndHeight -= rowMargins.bottomMargin;

            return pushResult;
        }

        private pushLayoutRowsIntoCell(): PushResult {
            const rowIndex: number = this.tblPos.rowIndex;
            const cellIndex: number = this.tblPos.cellIndex;
            const currCellLayoutRowIndexes: number[] = this.layoutRowInCellIndexes[rowIndex].layoutRowIndexes;

            if (currCellLayoutRowIndexes[cellIndex] == -1)
                currCellLayoutRowIndexes[cellIndex]++;
            let thisCellHaveSomeContent: boolean = false;

            const cellLayoutRows: LayoutTableFormatterLayoutCell = this.layoutRowsInfo[rowIndex][cellIndex];
            for (let layoutRowInd: number; (layoutRowInd = currCellLayoutRowIndexes[cellIndex]) < cellLayoutRows.layoutRow.length; currCellLayoutRowIndexes[cellIndex]++) {
                const avalSpaceByColumn: number = this.getAvaliableSpaceForCellContent();
                if (cellLayoutRows.layoutRow[layoutRowInd] == null) { // mean on this row inner table
                    const tblFormatter: LayoutTableFormatter = cellLayoutRows.internalCellTable[layoutRowInd];
                    const cols: LayoutTableColumnInfo[] = tblFormatter.getTableColumns(avalSpaceByColumn, false, this.currTableCellInfo.bound.getBottomBoundPosition(), this.currTableCellInfo); // TODO can be not false

                    if (tblFormatter.isFormattingEnd) {
                        this.currTableCellInfo.bound.height += cols[0].height;
                    }
                    else {
                        if (!cols.length)
                            return thisCellHaveSomeContent ? PushResult.PartObjectsPlaced : PushResult.NoObjectsPlaced;
                        this.currTableCellInfo.bound.height += cols[0].height;
                    }
                    this.currTableCellInfo.internalTables[this.currTableCellInfo.layoutRows.length] = cols[0];
                    this.columns.push.apply(this.columns, cols);
                    thisCellHaveSomeContent = true;
                    if (!tblFormatter.isFormattingEnd)
                        return PushResult.PartObjectsPlaced;
                }
                else {
                    const layoutRow: LayoutRow = cellLayoutRows.layoutRow[layoutRowInd];
                    if (layoutRowInd == cellLayoutRows.layoutRow.length - 1) {
                        const lastRowIndex: number = this.currTableCellInfo.layoutRows.length;
                        if (this.currTableCellInfo.internalTables[lastRowIndex]) {
                            const boxes: LayoutBox[] = layoutRow.boxes;
                            if (boxes.length == 1 && boxes[0].getType() == LayoutBoxType.ParagraphMark)
                                layoutRow.height = 0;
                        }
                    }

                    if (layoutRow.height <= avalSpaceByColumn) {
                        this.currTableCellInfo.layoutRows.push(<LayoutRowWithIndex>layoutRow);
                        layoutRow.tableCellInfo = this.currTableCellInfo;
                        layoutRow.y = this.currTableCellInfo.bound.getBottomBoundPosition();
                        thisCellHaveSomeContent = true;
                        this.currTableCellInfo.bound.height += layoutRow.height;
                    }
                    else
                        return thisCellHaveSomeContent ? PushResult.PartObjectsPlaced : PushResult.NoObjectsPlaced;
                }
            }
            return PushResult.AllObjectsPlaced;
        }

        public setRowHeight() {
            const rowIndex: number = this.modelRowIndex;
            if (this.layoutRowInCellIndexes[rowIndex].numPages != 0) {
                let maxCellHeightStartAndEndOnThisRow: number = 0;
                for (let cell of this.currTableRowInfo.rowCells) {
                    const cellHeight: number = cell.bound.height;
                    if (maxCellHeightStartAndEndOnThisRow < cellHeight)
                        maxCellHeightStartAndEndOnThisRow = cellHeight;
                }
                this.currTableRowInfo.bound.height = maxCellHeightStartAndEndOnThisRow;
                return; // prev row height not matter
            }

            const height: LayoutFormatterTableRowHeightInfo = this.rowsHeight[rowIndex];
            const cells: TableCell[] = this.tblPos.table.rows[rowIndex].cells;

            let maxCellHeightStartAndEndOnThisRow: number = 0;

            for (let cellIndex: number = 0, cell: TableCell; cell = cells[cellIndex]; cellIndex++) {
                switch (cell.verticalMerging) {
                    case TableCellMergingState.Continue:
                        const cellGridInfo: TableCellGridInfo = this.grid.tableCellInfos[rowIndex][cellIndex];
                        const cellStartRowIndex: number = cellGridInfo.getStartRowIndex();
                        const cellLastRowIndex: number = cellStartRowIndex + cellGridInfo.getNumRowsInCell() - 1;
                        if (rowIndex == cellLastRowIndex) {
                            const startRowIndexInColumn: number = Math.max(0, this.currTableColumnInfo.tableRows.length - cellGridInfo.getNumRowsInCell());
                            const firstRowInThisCellInThisColumn: LayoutTableRowInfo = this.currTableColumnInfo.tableRows[startRowIndexInColumn];
                            let restCellHeight: number;
                            for (let layCellInfo of firstRowInThisCellInThisColumn.rowCells)
                                if (cellGridInfo.getGridCellIndex() == layCellInfo.cellGridIndex) {
                                    restCellHeight = layCellInfo.bound.height;
                                    break;
                                }

                            for (let heightIndex: number = firstRowInThisCellInThisColumn.rowIndex; heightIndex < this.currTableRowInfo.rowIndex; heightIndex++)
                                restCellHeight -= this.rowsHeight[heightIndex].contentHeight;

                            if (maxCellHeightStartAndEndOnThisRow < restCellHeight)
                                maxCellHeightStartAndEndOnThisRow = restCellHeight;
                        }
                        break;
                }
            }


            for (let layoutCellInfo of this.currTableRowInfo.rowCells) {
                const cellGridInfo: TableCellGridInfo = this.grid.tableCellGridInfos[rowIndex][layoutCellInfo.cellGridIndex];
                const cellStartRowIndex: number = cellGridInfo.getStartRowIndex();
                const cell: TableCell = cells[cellGridInfo.getCellIndex(rowIndex - cellStartRowIndex)];
                switch (cell.verticalMerging) {
                    case TableCellMergingState.None:
                        const cellHeight: number = layoutCellInfo.bound.height;
                        if (maxCellHeightStartAndEndOnThisRow < cellHeight)
                            maxCellHeightStartAndEndOnThisRow = cellHeight;
                        break;
                }
            }

            switch (height.preferredHeightType) {
                case TableHeightUnitType.Exact:
                    // think need cut some sells and set overflow: hidden
                    this.currTableRowInfo.bound.height = height.preferredHeightValue;
                    break;
                case TableHeightUnitType.Auto:
                    this.currTableRowInfo.bound.height = Math.max(maxCellHeightStartAndEndOnThisRow, height.contentHeight);
                    break;
                case TableHeightUnitType.Minimum:
                    this.currTableRowInfo.bound.height = Math.max(maxCellHeightStartAndEndOnThisRow, height.contentHeight, height.preferredHeightValue);
                    break;
            }

            height.contentHeight = this.currTableRowInfo.bound.height;
        }

        private applyBottomHorizontalBordersChangesForPrevRow() {
            if (this.currTableColumnInfo.tableRows.length == 0)
                return;

            const rowIndex: number = this.currTableRowInfo.rowIndex;
            const prevRowInfoRowIndex: number = this.currTableColumnInfo.getLastTableRow().rowIndex;
            const isPrevRowLastInTable: boolean = prevRowInfoRowIndex == this.grid.table.rows.length - 1;
            const cellSpacingPrevRow: number = this.rowCellSpacings[prevRowInfoRowIndex];
            const prevRowHorizBorders: LayoutTableHorizontalLineBordersInfo[] = this.currColumnHorizontalBorders[this.currColumnHorizontalBorders.length - 1];
            const horBrdLastIndex: number = prevRowHorizBorders.length - 1;

            let delta: number = 0;
            if (cellSpacingPrevRow > 0) {
                const lastBrd: LayoutTableHorizontalLineBordersInfo = prevRowHorizBorders.pop();
                const lastLastBrd: LayoutTableHorizontalLineBordersInfo = prevRowHorizBorders.pop();
                const newBrd: LayoutTableHorizontalLineBordersInfo =
                    this.bordersHelper.collectOneCellAndTableHorizontalBorders(rowIndex - 1, TableCellPropertiesMergerBorderBottom, rowIndex, this.bordersHelper.tableBorderHorizontal, true);
                prevRowHorizBorders.push(newBrd);
                delta = lastLastBrd.maxWidth + lastBrd.maxWidth - newBrd.maxWidth + (isPrevRowLastInTable ? cellSpacingPrevRow : 0);
            }
            else
                delta = prevRowHorizBorders.pop().maxWidth;

            if (delta == 0)
                return;

            this.currTableRowInfo.bound.height -= delta;
            for (let tblCell of this.currTableRowInfo.rowCells) {
                tblCell.bound.y -= delta;
                for (let row of tblCell.layoutRows)
                    row.y -= delta;
            }
        }

        private setColumnHorizontalBorders() {
            const tblRows: LayoutTableRowInfo[] = this.currTableColumnInfo.tableRows;
            const columnInfoYOffset: number = this.currTableColumnInfo.y;
            for (let tblRowIndex: number = 0, tblRow: LayoutTableRowInfo; tblRow = tblRows[tblRowIndex]; tblRowIndex++) {
                const isThisRowFirstInColumn: boolean = tblRowIndex == 0;
                const isThisRowLastInColumn: boolean = tblRowIndex == tblRows.length - 1;
                const rowCellSpacing: number = this.rowCellSpacings[tblRow.rowIndex];
                const rowBrd: LayoutTableHorizontalLineBordersInfo[] = this.currColumnHorizontalBorders[tblRowIndex];
                if (rowCellSpacing > 0) {
                    let endIndex: number;
                    if (isThisRowFirstInColumn) {
                        rowBrd[0].yPosition = tblRow.bound.y;
                        rowBrd[0].isOffsetFromTop = true;
                        rowBrd[1].yPosition = rowBrd[0].yPosition + rowBrd[0].maxWidth + rowCellSpacing * (this.isThisColumnFirstInTable ? 2 : 1) + rowBrd[1].maxWidth;
                        rowBrd[1].isOffsetFromTop = false;
                        endIndex = 3;
                    }
                    else {
                        rowBrd[0].yPosition = tblRow.bound.y + rowCellSpacing + rowBrd[0].maxWidth;
                        rowBrd[0].isOffsetFromTop = false;
                        endIndex = 2;
                    }

                    if (isThisRowLastInColumn) {
                        rowBrd[endIndex].yPosition = tblRow.bound.getBottomBoundPosition() - rowBrd[endIndex].maxWidth;
                        rowBrd[endIndex].isOffsetFromTop = true;
                        rowBrd[endIndex - 1].yPosition = rowBrd[endIndex].yPosition - rowCellSpacing * 2 - rowBrd[endIndex - 1].maxWidth;
                        rowBrd[endIndex - 1].isOffsetFromTop = true;
                    }
                    else {
                        rowBrd[endIndex - 1].yPosition = tblRow.bound.getBottomBoundPosition() - rowCellSpacing - rowBrd[endIndex - 1].maxWidth;
                        rowBrd[endIndex - 1].isOffsetFromTop = true;
                    }
                }
                else {
                    rowBrd[0].yPosition = tblRow.bound.y;
                    rowBrd[0].isOffsetFromTop = true;

                    if (isThisRowLastInColumn) {
                        rowBrd[1].yPosition = tblRow.bound.getBottomBoundPosition() - rowBrd[1].maxWidth;
                        rowBrd[1].isOffsetFromTop = true;
                    }
                }
                for (let brd of rowBrd)
                    brd.yPosition -= columnInfoYOffset;
            }
        }

        private setColumnVerticalBorders() {
            const columnInfoYOffset: number = this.currTableColumnInfo.y;
            const tblRows: LayoutTableRowInfo[] = this.currTableColumnInfo.tableRows;
            const rows: TableRow[] = this.grid.table.rows;
            for (let tblRowIndex: number = 0, tblRow: LayoutTableRowInfo; tblRow = tblRows[tblRowIndex]; tblRowIndex++) {
                const isRowFirstInColumn: boolean = tblRowIndex == 0;
                const isRowLastInColumn: boolean = tblRowIndex == tblRows.length - 1;

                const horRowBorders: LayoutTableHorizontalLineBordersInfo[] = this.currColumnHorizontalBorders[tblRowIndex];
                const nextHorRowBorders: LayoutTableHorizontalLineBordersInfo[] = this.currColumnHorizontalBorders[tblRowIndex + 1];
                const lastHorBorderLineIndex: number = horRowBorders.length - 1;
                const rowBorders: LayoutTableBorder[][] = [];
                this.currColumnVerticalBorders.push(rowBorders);

                let currLayoutCellIndex: number = 0;
                const rowIndex: number = tblRow.rowIndex;
                const cells: TableCell[] = rows[rowIndex].cells;
                const rowCellSpacing: number = this.rowCellSpacings[rowIndex];
                for (let cellIndex: number = 0, cell: TableCell; cell = cells[cellIndex]; cellIndex++) {
                    const isFirstCellInRow: boolean = cellIndex == 0;
                    const isLastCellInRow: boolean = cellIndex == cells.length - 1;

                    const cellGridInfo: TableCellGridInfo = this.grid.tableCellInfos[rowIndex][cellIndex];

                    const isThisCellMergedByTop: boolean = !isRowFirstInColumn && cellGridInfo.getStartRowIndex() != rowIndex;
                    const isThisCellMergedByBottom: boolean = !isRowLastInColumn && cellGridInfo.getStartRowIndex() + cellGridInfo.getNumRowsInCell() - 1 != rowIndex;

                    const cellBorders: LayoutTableBorder[] = [];
                    rowBorders.push(cellBorders);

                    for (let brd of this.verticalBorders[cellGridInfo.getStartRowIndex()][cellGridInfo.getCellIndex(0)])
                        cellBorders.push(brd.clone());

                    let topCellBordersLine: LayoutTableHorizontalLineBordersInfo;
                    let bottomCellBordersLine: LayoutTableHorizontalLineBordersInfo;

                    if (rowCellSpacing > 0) {
                        topCellBordersLine = horRowBorders[isRowFirstInColumn ? 1 : 0];
                        bottomCellBordersLine = horRowBorders[lastHorBorderLineIndex - (isRowLastInColumn ? 1 : 0)];
                    }
                    else {
                        topCellBordersLine = horRowBorders[0];
                        bottomCellBordersLine = isRowLastInColumn ? horRowBorders[lastHorBorderLineIndex] : nextHorRowBorders[0];
                    }

                    const yPos: number = topCellBordersLine.yPosition + (topCellBordersLine.isOffsetFromTop ? topCellBordersLine.maxWidth : 0);
                    const yEndPos: number = bottomCellBordersLine.yPosition - (bottomCellBordersLine.isOffsetFromTop ? 0 : bottomCellBordersLine.maxWidth);

                    for (let vertCellBorderIndex: number = 0, vertCellBorder: LayoutTableBorder; vertCellBorder = cellBorders[vertCellBorderIndex]; vertCellBorderIndex++) {
                        const isFirstBorderInCell: boolean = vertCellBorderIndex == 0;
                        const isLastBorderInCell: boolean = vertCellBorderIndex == cellBorders.length - 1;
                        const isBoundBorder: boolean = isFirstCellInRow && isFirstBorderInCell || isLastBorderInCell && isLastCellInRow;
                        vertCellBorder.yPos = yPos;
                        if (isBoundBorder || isThisCellMergedByTop)
                            vertCellBorder.yPos -= rowCellSpacing * (this.isThisColumnFirstInTable && isRowFirstInColumn ? 2 : 1) + topCellBordersLine.maxWidth;

                        vertCellBorder.length = yEndPos - vertCellBorder.yPos;
                        if (isBoundBorder || isThisCellMergedByBottom)
                            vertCellBorder.length += rowCellSpacing * (isRowLastInColumn ? 2 : 1) + bottomCellBordersLine.maxWidth;
                    }
                }
            }
        }

        private setRowsVerticalBounds() {
            const vertBrds: LayoutTableBorder[][][] = this.currColumnVerticalBorders;
            const rows: LayoutTableRowInfo[] = this.currTableColumnInfo.tableRows;
            for (let rowIndex: number = 0, row: LayoutTableRowInfo; row = rows[rowIndex]; rowIndex++) {
                const vertRowBrds: LayoutTableBorder[][] = vertBrds[rowIndex];
                const vertCellBrds: LayoutTableBorder[] = vertRowBrds[vertRowBrds.length - 1];
                const lastBrdInRow: LayoutTableBorder = vertCellBrds[vertCellBrds.length - 1];
                row.bound.x = vertRowBrds[0][0].xPos;
                row.bound.width = lastBrdInRow.xPos - row.bound.x;
            }
        }

        private setBackgroundInfos() {
            const defaultTblCellProps: TableCellProperties = this.subDocument.documentModel.defaultTableCellProperties;
            const tableStyle: TableStyle = this.grid.table.style;
            for (let rowIndexInColumn: number = 0, vertRowBrds: LayoutTableBorder[][]; vertRowBrds = this.currColumnVerticalBorders[rowIndexInColumn]; rowIndexInColumn++) {
                const tblRowInfo: LayoutTableRowInfo = this.currTableColumnInfo.tableRows[rowIndexInColumn];
                const modelRowIndex: number = tblRowInfo.rowIndex;
                const rowCellSpacing: number = this.rowCellSpacings[modelRowIndex];
                const cells: TableCell[] = this.grid.table.rows[modelRowIndex].cells;
                const rowHorizontalBorders: LayoutTableHorizontalLineBordersInfo[] = this.currColumnHorizontalBorders[rowIndexInColumn];
                const isRowFirstInColumn: boolean = rowIndexInColumn == 0;
                const isRowLastInColumn: boolean = rowIndexInColumn == this.currColumnVerticalBorders.length - 1;
                for (let cellIndex: number = 0, cellVerticalBorders: LayoutTableBorder[]; cellVerticalBorders = vertRowBrds[cellIndex]; cellIndex++) {
                    const cell: TableCell = cells[cellIndex];
                    if (cell.verticalMerging == TableCellMergingState.Continue && rowIndexInColumn != 0)
                        continue;

                    const isCellLastInRow: boolean = cellIndex == vertRowBrds.length - 1;

                    const cellGridInfo: TableCellGridInfo = this.grid.tableCellInfos[modelRowIndex][cellIndex];
                    const lastCellRowIndex: number = Math.min(cellGridInfo.getStartRowIndex() + cellGridInfo.getNumRowsInCell() - modelRowIndex + rowIndexInColumn, this.currColumnVerticalBorders.length) - 1;
                    const cellLastBrdsLines: LayoutTableHorizontalLineBordersInfo[] = this.currColumnHorizontalBorders[lastCellRowIndex];

                    const rect: Rectangle = new Rectangle();

                    if (rowCellSpacing) {
                        const isCellFirstInRow: boolean = cellIndex == 0;
                        const leftBrdIndex: number = isCellFirstInRow ? 1 : 0;
                        const rightBrdIndex: number = cellVerticalBorders.length - (isCellLastInRow ? 2 : 1);
                        rect.x = cellVerticalBorders[leftBrdIndex].xPos + cellVerticalBorders[leftBrdIndex].borderInfo.width;
                        rect.width = cellVerticalBorders[rightBrdIndex].xPos - rect.x;


                        const topHorBrdLine: LayoutTableHorizontalLineBordersInfo = rowHorizontalBorders[isRowFirstInColumn ? 1 : 0];
                        rect.y = topHorBrdLine.yPosition + topHorBrdLine.maxWidth;
                        rect.height = cellLastBrdsLines[cellLastBrdsLines.length - (isRowLastInColumn ? 2 : 1)].yPosition - rect.y;
                    }
                    else {
                        rect.x = cellVerticalBorders[0].xPos + cellVerticalBorders[0].borderInfo.width;
                        const rightBrd: LayoutTableBorder = isCellLastInRow ? cellVerticalBorders[1] : vertRowBrds[cellIndex + 1][0];
                        rect.width = rightBrd.xPos - rect.x;

                        const topHorBrdLine: LayoutTableHorizontalLineBordersInfo = rowHorizontalBorders[0];
                        const botHorBrdLine: LayoutTableHorizontalLineBordersInfo = lastCellRowIndex == this.currColumnVerticalBorders.length - 1 ?
                            this.currColumnHorizontalBorders[lastCellRowIndex][1] : this.currColumnHorizontalBorders[lastCellRowIndex + 1][0];
                        rect.y = topHorBrdLine.yPosition + topHorBrdLine.maxWidth;
                        rect.height = botHorBrdLine.yPosition - rect.y;
                    }
                    const cellBackgroundColor: number = new TableCellPropertiesMergerBackgroundColor().getProperty(cell.properties, tableStyle, cell.conditionalFormatting, defaultTblCellProps);
                    tblRowInfo.backgroundInfos.push(new LayoutTableCellBackgroundInfo(rect, cellBackgroundColor));
                }
            }
        }

        private applyHorizontalAlignment() {
            const leftBound: number = this.currTableColumnInfo.x;
            const rightBound: number = leftBound + this.tableMaxWidth;
            const tblRows: LayoutTableRowInfo[] = this.currTableColumnInfo.tableRows;
            const avalSpace: number = rightBound - this.currTableColumnInfo.getRightBoundPosition();
            let offset: number;
            switch (this.tableHorizontalAlignment) {
                case TableRowAlignment.Right: offset = avalSpace; break;
                case TableRowAlignment.Center: offset = Math.floor(avalSpace / 2); break;
                case TableRowAlignment.Left:
                default: offset = 0; break;
            }
            if (offset <= 0)
                return;

            LayoutTableFormatter.moveAllTable(this.currTableColumnInfo, (rect: Rectangle) => rect.x += offset);

            //for (let localColumnRowIndex: number = 0, tblRow: LayoutTableRowInfo; tblRow = tblRows[localColumnRowIndex]; localColumnRowIndex++) {
            //    const rowHorAlignment: TableRowAlignment = this.rowsHeight[tblRow.rowIndex].horizontalAlignment;
            //    // too hard dut to general borders two rows. miss it. miss rowHorAlignment
            //}
        }

        private applyCellsVerticalAlignment() {
            const defaultTableCellProps: TableCellProperties = this.subDocument.documentModel.defaultTableCellProperties;
            const table: Table = this.grid.table;
            const tableStyle: TableStyle = table.style;
            for (let tblRow of this.currTableColumnInfo.tableRows) {
                for (let tblCell of tblRow.rowCells) {
                    if (!(tblCell.cellBoundFlags & TableCellBoundFlags.StartOnThisColumn && tblCell.cellBoundFlags & TableCellBoundFlags.EndOnThisColumn))
                        continue;

                    const cellGridInfo: TableCellGridInfo = this.grid.tableCellGridInfos[tblRow.rowIndex][tblCell.cellGridIndex];
                    const cellStartRowIndex: number = cellGridInfo.getStartRowIndex();
                    const cellIndex: number = cellGridInfo.getCellIndex(0);
                    const cell: TableCell = table.rows[cellStartRowIndex].cells[cellIndex];
                    const verticalAlignmentType: TableCellVerticalAlignment = new TableCellVerticalAlignmentMerger()
                        .getProperty(cell.properties, tableStyle, cell.conditionalFormatting, defaultTableCellProps);

                    const topAndBottomMargins: TopAndBottomMarginsForRow = this.topAndBottomMarginsForRow[cellStartRowIndex];
                    const bottomBound: number = tblCell.bound.getBottomBoundPosition() - topAndBottomMargins.bottomMargin -
                        this.rowCellSpacings[tblRow.rowIndex] * (tblRow.rowIndex == table.rows.length - 1 ? 2 : 1);
                    const numLayoutRows: number = tblCell.layoutRows.length;
                    const lastInnerTable: LayoutTableColumnInfo = tblCell.internalTables[numLayoutRows];
                    const rowsBottomBound: number = lastInnerTable ?
                        lastInnerTable.getBottomBoundPosition() :
                        tblCell.layoutRows[numLayoutRows - 1].getBottomBoundPosition();
                    const avalSpace: number = bottomBound - rowsBottomBound;
                    var offset: number;
                    switch (verticalAlignmentType) {
                        case TableCellVerticalAlignment.Bottom: offset = avalSpace; break;
                        case TableCellVerticalAlignment.Center: offset = Math.floor(avalSpace / 2); break;
                        case TableCellVerticalAlignment.Top:
                        default: offset = 0; break;
                    }

                    if (offset <= 0)
                        continue;

                    for (let layoutRow of tblCell.layoutRows)
                        layoutRow.y += offset;

                    for (let key in tblCell.internalTables) {
                        if (!tblCell.internalTables.hasOwnProperty(key))
                            continue;
                        LayoutTableFormatter.moveAllTable(tblCell.internalTables[key], (rect: Rectangle) => rect.y += offset);
                    }
                }
            }
        }

        private static moveAllTable(tblCol: LayoutTableColumnInfo, mover: (rect: Rectangle) => void) {
            mover(tblCol);
            for (let tblRow of tblCol.tableRows) {
                mover(tblRow.bound);
                for (let tblCell of tblRow.rowCells) {
                    mover(tblCell.bound);
                    for (let key in tblCell.internalTables) {
                        if (!tblCell.internalTables.hasOwnProperty(key))
                            continue;
                        LayoutTableFormatter.moveAllTable(tblCell.internalTables[key], mover);
                    }
                    for (let layoutRow of tblCell.layoutRows)
                        mover(layoutRow);
                }
            }
        }
        
    }

    class LayoutTableFormatterBorderMerger {
        public static getFinalReducedVerticalBorders(vertBorders: LayoutTableBorder[][][]): LayoutTableBorder[] {
            const resultBorders: LayoutTableBorder[] = [];

            const allVerticalBorders: LayoutTableBorder[] = [];
            for (let rowBorders of vertBorders)
                for (let cellBorders of rowBorders)
                    for (let border of cellBorders)
                        allVerticalBorders.push(border);

            allVerticalBorders.sort((a: LayoutTableBorder, b: LayoutTableBorder) => {
                const xPosDiff: number = a.xPos - b.xPos;
                if (Math.abs(xPosDiff) > 1)
                    return xPosDiff;

                const yPosDiff: number = a.yPos - b.yPos;
                if (yPosDiff != 0)
                    return yPosDiff;

                return 0;
            });

            let prevBorder: LayoutTableBorder = allVerticalBorders[0];
            resultBorders.push(prevBorder);
            for (let brdIndex: number = 1, border: LayoutTableBorder; border = allVerticalBorders[brdIndex]; brdIndex++) {
                if (Math.abs(prevBorder.xPos - border.xPos) < 2 && Math.abs(prevBorder.yPos + prevBorder.length - border.yPos) < 2 && prevBorder.borderInfo.equals(border.borderInfo))
                    prevBorder.length = border.yPos + border.length - prevBorder.yPos;
                else {
                    prevBorder = border;
                    resultBorders.push(prevBorder);
                }
            }
            return resultBorders;
        }

        public static getFinalReducedHorizontalBorders(horBorders: LayoutTableHorizontalLineBordersInfo[][]): LayoutTableBorder[] {
            const resultBorders: LayoutTableBorder[] = [];
            for (let horLines of horBorders) {
                for (let line of horLines) {
                    const borders: LayoutTableHorizontalBorder[] = line.borders;
                    let prevBorder: LayoutTableBorder = borders[0].getLayoutTableBorder(line)
                    resultBorders.push(prevBorder);
                    for (let brdIndex: number = 1, border: LayoutTableHorizontalBorder; border = borders[brdIndex]; brdIndex++) {
                        if (Math.abs(prevBorder.xPos + prevBorder.length - border.xPosition) < 2 && prevBorder.borderInfo.equals(border.borderInfo))
                            prevBorder.length = border.xPosition + border.length - prevBorder.xPos;
                        else {
                            prevBorder = border.getLayoutTableBorder(line);
                            resultBorders.push(prevBorder);
                        }
                    }
                }
            }
            return resultBorders;
        }
    }
}