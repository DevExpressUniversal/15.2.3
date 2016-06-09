module __aspxRichEdit {
    export type getCellBorderConstructor = new () => TableCellPropertiesMergerBorderBase;

    export class LayoutTableHorizontalBorder implements ICloneable<LayoutTableHorizontalBorder>, ISupportCopyFrom<LayoutTableHorizontalBorder> {
        xPosition: number;
        length: number;
        borderInfo: BorderInfo;

        clone(): LayoutTableHorizontalBorder {
            const result: LayoutTableHorizontalBorder = new LayoutTableHorizontalBorder();
            result.copyFrom(this);
            return result;
        }

        copyFrom(obj: LayoutTableHorizontalBorder) {
            this.length = obj.length;
            this.xPosition = obj.xPosition;
            this.borderInfo = obj.borderInfo.clone();
        }

        getLayoutTableBorder(line: LayoutTableHorizontalLineBordersInfo): LayoutTableBorder {
            return new LayoutTableBorder(this.xPosition, line.yPosition - (line.isOffsetFromTop ? 0 : this.borderInfo.width), this.length, this.borderInfo);
        }
    }

    export class LayoutTableHorizontalLineBordersInfo implements ICloneable<LayoutTableHorizontalLineBordersInfo>, ISupportCopyFrom<LayoutTableHorizontalLineBordersInfo> {
        constructor(isOffsetFromTop: boolean) {
            this.isOffsetFromTop = isOffsetFromTop;
        }

        isOffsetFromTop: boolean; // true mean mainPosition is top bound border. False - bottom bound (when for row set cellSpacing)
        yPosition: number;
        borders: LayoutTableHorizontalBorder[] = [];

        maxWidth: number = 0; // max border width in this line

        updateWidth(width: number) {
            if (width > this.maxWidth)
                this.maxWidth = width;
        }

        clone(): LayoutTableHorizontalLineBordersInfo {
            const result: LayoutTableHorizontalLineBordersInfo = new LayoutTableHorizontalLineBordersInfo(this.isOffsetFromTop);
            result.copyFrom(this);
            return result;
        }

        copyFrom(obj: LayoutTableHorizontalLineBordersInfo) {
            this.isOffsetFromTop = obj.isOffsetFromTop;
            this.yPosition = obj.yPosition;
            this.maxWidth = obj.maxWidth;

            this.borders = [];
            for (let brd of this.borders)
                this.borders.push(brd.clone());
        }
    }

    export class LayoutTableBorder implements ICloneable<LayoutTableBorder>, ISupportCopyFrom<LayoutTableBorder> {
        xPos: number;
        yPos: number;
        length: number;
        borderInfo: BorderInfo;

        constructor(xPos: number, yPos: number, length: number, borderInfo: BorderInfo) {
            this.xPos = xPos;
            this.yPos = yPos;
            this.length = length;
            this.borderInfo = borderInfo;
        }

        clone(): LayoutTableBorder {
            return new LayoutTableBorder(this.xPos, this.yPos, this.length, this.borderInfo.clone());
        }

        copyFrom(obj: LayoutTableBorder) {
            this.xPos = obj.xPos;
            this.yPos = obj.yPos;
            this.length = obj.length;
            this.borderInfo = obj.borderInfo.clone();
        }
    }

    export class LayoutTableBorderHelper {
        private rowCellSpacings: number[];
        private grid: TableGrid;
        private tblStyle: TableStyle;

        private tableBorderLeft: BorderInfo;
        private tableBorderRight: BorderInfo;
        private tableBorderVertical: BorderInfo;

        private tableBorderTop: BorderInfo;
        private tableBorderBottom: BorderInfo;
        public tableBorderHorizontal: BorderInfo;

        private unitConverter: IUnitConverter;

        constructor(grid: TableGrid, rowCellSpacings: number[], defaultTblProps: TableProperties, unitConverter: IUnitConverter) {
            this.grid = grid;
            this.rowCellSpacings = rowCellSpacings;
            this.unitConverter = unitConverter;

            this.tblStyle = this.grid.table.style;
            const tblProps: TableProperties = this.grid.table.properties;

            this.tableBorderLeft = new TablePropertiesMergerBorderLeft().getProperty(tblProps, this.tblStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);
            this.tableBorderRight = new TablePropertiesMergerBorderRight().getProperty(tblProps, this.tblStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);
            this.tableBorderTop = new TablePropertiesMergerBorderTop().getProperty(tblProps, this.tblStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);
            this.tableBorderBottom = new TablePropertiesMergerBorderBottom().getProperty(tblProps, this.tblStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);
            this.tableBorderHorizontal = new TablePropertiesMergerBorderHorizontal().getProperty(tblProps, this.tblStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);
            this.tableBorderVertical = new TablePropertiesMergerBorderVertical().getProperty(tblProps, this.tblStyle, ConditionalTableStyleFormatting.WholeTable, defaultTblProps);

            this.tableBorderLeft = this.borderConvertToPixels(this.tableBorderLeft);
            this.tableBorderRight = this.borderConvertToPixels(this.tableBorderRight);
            this.tableBorderTop = this.borderConvertToPixels(this.tableBorderTop);
            this.tableBorderBottom = this.borderConvertToPixels(this.tableBorderBottom);
            this.tableBorderHorizontal = this.borderConvertToPixels(this.tableBorderHorizontal);
            this.tableBorderVertical = this.borderConvertToPixels(this.tableBorderVertical);
        }

        private borderConvertToPixels(brdInfo: BorderInfo): BorderInfo {
            if (!brdInfo)
                return null;
            const newBrd: BorderInfo = brdInfo.clone();
            newBrd.width = this.unitConverter.toPixels(newBrd.width);
            return newBrd;
        }

        public getVerticalBorders(): LayoutTableBorder[][][] {
            const verticalBorders: LayoutTableBorder[][][] = [];
            const rows: TableRow[] = this.grid.table.rows;
            const tblInfos: TableCellGridInfo[][] = this.grid.tableCellInfos;
            for (var rowIndex: number = 0, row: TableRow; row = rows[rowIndex]; rowIndex++) {
                var cellSpacing: number = this.rowCellSpacings[rowIndex];
                const rowBorders: LayoutTableBorder[][] = [];
                const lastRowCellIndex: number = row.cells.length - 1;
                var cells: TableCell[] = row.cells;
                verticalBorders.push(rowBorders);
                for (var cellIndex: number = 0, cell: TableCell; cell = cells[cellIndex]; cellIndex++) {
                    const cellBorders: LayoutTableBorder[] = [];
                    rowBorders.push(cellBorders);
                    var cellInfo: TableCellGridInfo = tblInfos[rowIndex][cellIndex];

                    const cellForBorders: TableCell = cellInfo.getStartRowIndex() != rowIndex ? rows[cellInfo.getStartRowIndex()].cells[cellInfo.getCellIndex(0)] : cell;

                    if (cellSpacing > 0) {
                        if (cellIndex == 0) {
                            cellBorders.push(this.mergeVerticalBorders(null, null, null, null, this.tableBorderLeft, (borderWidth: number) =>
                                Math.floor(this.grid.columnsXPositions[cellInfo.getGridCellIndex()] - borderWidth / 2)));

                            cellBorders.push(this.mergeVerticalBorders(cellForBorders, TableCellPropertiesMergerBorderLeft, null, null, this.tableBorderVertical, (borderWidth: number) =>
                                Math.floor(this.grid.columnsXPositions[cellInfo.getGridCellIndex()] + cellSpacing * 2)));
                        }
                        else
                            cellBorders.push(this.mergeVerticalBorders(cellForBorders, TableCellPropertiesMergerBorderLeft, null, null, this.tableBorderVertical, (borderWidth: number) =>
                                Math.floor(this.grid.columnsXPositions[cellInfo.getGridCellIndex()] + cellSpacing)));

                        cellBorders.push(this.mergeVerticalBorders(cellForBorders, TableCellPropertiesMergerBorderRight, null, null, this.tableBorderVertical, (borderWidth: number) =>
                            Math.floor(this.grid.columnsXPositions[cellInfo.getGridCellIndex() + cell.columnSpan] - cellSpacing * (cellIndex == cells.length - 1 ? 2 : 1) - borderWidth)));

                        if (cellIndex == lastRowCellIndex)
                            cellBorders.push(this.mergeVerticalBorders(null, null, null, null, this.tableBorderRight, (borderWidth: number) =>
                                Math.floor(this.getCellEndGridPosition(rowIndex, cellIndex) - borderWidth / 2)));
                    }
                    else {
                        if (cellIndex == 0)
                            cellBorders.push(this.mergeVerticalBorders(cellForBorders, TableCellPropertiesMergerBorderLeft, null, null, this.tableBorderLeft, (borderWidth: number) =>
                                Math.floor(this.grid.columnsXPositions[cellInfo.getGridCellIndex()] - borderWidth / 2)));
                        else {
                            const leftCell: TableCell = cells[cellIndex - 1];
                            let leftCellForBorders: TableCell;
                            if (leftCell.verticalMerging != TableCellMergingState.Continue)
                                leftCellForBorders = leftCell;
                            else {
                                const leftCellInfo: TableCellGridInfo = tblInfos[rowIndex][cellIndex - 1];
                                leftCellForBorders = rows[leftCellInfo.getStartRowIndex()].cells[leftCellInfo.getCellIndex(0)]
                            }
                            
                            cellBorders.push(this.mergeVerticalBorders(cellForBorders, TableCellPropertiesMergerBorderLeft,
                                leftCellForBorders, TableCellPropertiesMergerBorderRight, this.tableBorderVertical, (borderWidth: number) =>
                                    Math.floor(this.grid.columnsXPositions[cellInfo.getGridCellIndex()])));
                        }

                        if (cellIndex == lastRowCellIndex)
                            cellBorders.push(this.mergeVerticalBorders(cellForBorders, TableCellPropertiesMergerBorderRight, null, null, this.tableBorderRight, (borderWidth: number) =>
                                Math.floor(this.getCellEndGridPosition(rowIndex, cellIndex) - borderWidth / 2)));
                    }
                }
            }
            return verticalBorders;
        }

        private mergeVerticalBorders(cellA: TableCell, mergerCellA: getCellBorderConstructor, cellB: TableCell, mergerCellB: getCellBorderConstructor, tableBorderInfo: BorderInfo,
            getX: (borderWidth: number) => number): LayoutTableBorder {
            const tableVerticalBorder: LayoutTableBorder = new LayoutTableBorder(0, 0, 0, this.mergeThreeBorders(cellA, mergerCellA, cellB, mergerCellB, tableBorderInfo));
            if (tableVerticalBorder.borderInfo)
                tableVerticalBorder.xPos = getX(tableVerticalBorder.borderInfo.width);
            return tableVerticalBorder;
        }

        public getHorizontalBordersByRow(rowIndex: number, isRowFirstInLayoutColumn: boolean, isRowLastInLayoutColumn: boolean): LayoutTableHorizontalLineBordersInfo[] {
            const linesInRow: LayoutTableHorizontalLineBordersInfo[] = [];

            if (this.rowCellSpacings[rowIndex] > 0) {
                if (isRowFirstInLayoutColumn)
                    linesInRow.push(this.collectTableHorizontalBorders(this.tableBorderTop, rowIndex));

                linesInRow.push(this.collectOneCellAndTableHorizontalBorders(rowIndex, TableCellPropertiesMergerBorderTop, rowIndex - 1, this.tableBorderHorizontal, !isRowFirstInLayoutColumn));
                linesInRow.push(this.collectOneCellAndTableHorizontalBorders(rowIndex, TableCellPropertiesMergerBorderBottom, rowIndex + 1, this.tableBorderHorizontal, !isRowLastInLayoutColumn));

                if (isRowLastInLayoutColumn)
                    linesInRow.push(this.collectTableHorizontalBorders(this.tableBorderBottom, rowIndex));
            }
            else {
                if (isRowFirstInLayoutColumn)
                    linesInRow.push(this.collectOneCellAndTableHorizontalBorders(rowIndex, TableCellPropertiesMergerBorderTop, rowIndex - 1, this.tableBorderTop, false));
                else
                    linesInRow.push(this.collectThreeBorders(rowIndex, TableCellPropertiesMergerBorderTop, TableCellPropertiesMergerBorderBottom, this.tableBorderHorizontal));

                if (isRowLastInLayoutColumn)
                    linesInRow.push(this.collectOneCellAndTableHorizontalBorders(rowIndex, TableCellPropertiesMergerBorderBottom, rowIndex + 1, this.tableBorderBottom, false));
            }
            return linesInRow;
        }

        private collectTableHorizontalBorders(tableBorderInfo: BorderInfo, rowIndex: number): LayoutTableHorizontalLineBordersInfo {
            const cells: TableCell[] = this.grid.table.rows[rowIndex].cells;
            const lastCellIndex: number = cells.length - 1;
            const lastCell: TableCell = cells[lastCellIndex];
            const firstCellGridInfo: TableCellGridInfo = this.grid.tableCellInfos[rowIndex][0];
            const lastCellGridInfo: TableCellGridInfo = this.grid.tableCellInfos[rowIndex][lastCellIndex];
            
            const horizBorder: LayoutTableHorizontalBorder = new LayoutTableHorizontalBorder();
            horizBorder.borderInfo = tableBorderInfo;
            horizBorder.xPosition = Math.floor(this.grid.columnsXPositions[firstCellGridInfo.getGridCellIndex()]);
            horizBorder.length = Math.ceil(this.grid.columnsXPositions[lastCellGridInfo.getGridCellIndex() + lastCell.columnSpan]) - horizBorder.xPosition;

            const horizBordersInfo: LayoutTableHorizontalLineBordersInfo = new LayoutTableHorizontalLineBordersInfo(true);
            horizBordersInfo.borders.push(horizBorder);
            horizBordersInfo.updateWidth(tableBorderInfo ? tableBorderInfo.width : 0);
            return horizBordersInfo;
        }

        // merge cell and table border. tableBorderRowIndex - index row what below or above and what consider table border
        public collectOneCellAndTableHorizontalBorders(cellBorderRowIndex: number, getCurrCellBorderMerger: getCellBorderConstructor,
            tableBorderRowIndex: number, tableBorderInfo: BorderInfo, isTableBorderRowIndexValid: boolean): LayoutTableHorizontalLineBordersInfo {
            const rows: TableRow[] = this.grid.table.rows;
            const cells: TableCell[] = rows[cellBorderRowIndex].cells;
            const rowCellSpacing: number = this.rowCellSpacings[cellBorderRowIndex];
            const horizBordersInfo: LayoutTableHorizontalLineBordersInfo = new LayoutTableHorizontalLineBordersInfo(true);
            for (let cellIndex: number = 0, cell: TableCell; cell = cells[cellIndex]; cellIndex++) {
                const currCellGridTableInfo: TableCellGridInfo = this.grid.tableCellInfos[cellBorderRowIndex][cellIndex];
                const currCellGridStartIndex: number = currCellGridTableInfo.getGridCellIndex();
                const tableCellGridTableInfo: TableCellGridInfo = isTableBorderRowIndexValid ?
                    this.grid.tableCellGridInfos[tableBorderRowIndex][currCellGridStartIndex] :
                    null;
                if (currCellGridTableInfo == tableCellGridTableInfo)
                    continue;
                
                const currCellXPosition: number = Math.floor(this.grid.columnsXPositions[currCellGridStartIndex] + rowCellSpacing * (cellIndex == 0 ? 2 : 1));
                const currCellEndGridColumn: number =
                    Math.ceil(this.grid.columnsXPositions[currCellGridStartIndex + cell.columnSpan] - rowCellSpacing * (cellIndex == cells.length - 1 ? 2 : 1));

                const currCellBorder: BorderInfo = getCurrCellBorderMerger ?
                    this.borderConvertToPixels((new getCurrCellBorderMerger).getProperty(cell.properties, this.tblStyle, cell.conditionalFormatting, null)) :
                    null;

                const horizBorder: LayoutTableHorizontalBorder = new LayoutTableHorizontalBorder();
                horizBorder.xPosition = currCellXPosition;
                horizBorder.length = currCellEndGridColumn - currCellXPosition;
                horizBorder.borderInfo = currCellBorder ? currCellBorder : tableBorderInfo;
                
                horizBordersInfo.borders.push(horizBorder);
                horizBordersInfo.updateWidth(horizBorder.borderInfo ? horizBorder.borderInfo.width : 0);
            }
            return horizBordersInfo;
        }

        public collectThreeBorders(rowIndex: number, getCurrCellBorderMerger: getCellBorderConstructor, getTopCellBorderMerger: getCellBorderConstructor,
            tableBorderInfo: BorderInfo): LayoutTableHorizontalLineBordersInfo {
            const horizBordersInfo: LayoutTableHorizontalLineBordersInfo = new LayoutTableHorizontalLineBordersInfo(true);

            const rows: TableRow[] = this.grid.table.rows;
            const cells: TableCell[] = rows[rowIndex].cells;
            const gridColumnsNum: number = this.grid.columns.length;
            const prevRowIndex: number = rowIndex - 1;
            
            const gridInfosPrevRow: TableCellGridInfo[] = this.grid.tableCellGridInfos[prevRowIndex];
            const gridInfosCurrRow: TableCellGridInfo[] = this.grid.tableCellGridInfos[rowIndex];
            const prevRowCells: TableCell[] = this.grid.table.rows[prevRowIndex].cells;
            const currRowCells: TableCell[] = this.grid.table.rows[rowIndex].cells;
            for (let cellGridIndex: number = 0; cellGridIndex < gridColumnsNum; cellGridIndex++) {
                const prevCellInfo: TableCellGridInfo = gridInfosPrevRow[cellGridIndex];
                const currCellInfo: TableCellGridInfo = gridInfosCurrRow[cellGridIndex];
                
                if (!prevCellInfo && !currCellInfo ||
                    prevCellInfo && prevRowIndex != prevCellInfo.getStartRowIndex() + prevCellInfo.getNumRowsInCell() - 1)
                    continue;

                let prevCell: TableCell = prevCellInfo ? prevRowCells[prevCellInfo.getCellIndex(prevRowIndex - prevCellInfo.getStartRowIndex())] : null;
                let currCell: TableCell = currCellInfo ? currRowCells[currCellInfo.getCellIndex(rowIndex - currCellInfo.getStartRowIndex())] : null;

                const horizBorder: LayoutTableHorizontalBorder = new LayoutTableHorizontalBorder();
                horizBorder.xPosition = Math.floor(this.grid.columnsXPositions[cellGridIndex]);
                horizBorder.length = Math.ceil(this.grid.columns[cellGridIndex].width);
                horizBorder.borderInfo = this.mergeThreeBorders(currCell, getCurrCellBorderMerger, prevCell, getTopCellBorderMerger, tableBorderInfo);

                horizBordersInfo.updateWidth(horizBorder.borderInfo ? horizBorder.borderInfo.width : 0);
                horizBordersInfo.borders.push(horizBorder);
            }
            return horizBordersInfo;
        }

        private getCellEndGridPosition(rowIndex: number, cellIndex: number): number {
            const cellGridStartPos: number = this.grid.tableCellInfos[rowIndex][cellIndex].getGridCellIndex();
            const cellGridColumnEndIndex: number = cellGridStartPos + this.grid.table.rows[rowIndex].cells[cellIndex].columnSpan;
            return Math.ceil(this.grid.columnsXPositions[cellGridColumnEndIndex]);
        }

        private mergeThreeBorders(cellA: TableCell, mergerCellA: getCellBorderConstructor, cellB: TableCell, mergerCellB: getCellBorderConstructor, tableBorderInfo: BorderInfo): BorderInfo {
            let tblBrd: BorderInfo;
            if (cellA && cellB) {
                tblBrd = TableBorderCalculator.getPowerfulBorder(
                    this.borderConvertToPixels((new mergerCellA).getProperty(cellA.properties, this.tblStyle, cellA.conditionalFormatting, null)),
                    this.borderConvertToPixels((new mergerCellB).getProperty(cellB.properties, this.tblStyle, cellB.conditionalFormatting, null)));
            }
            else {
                const cell: TableCell = cellA || cellB;
                if (cell)
                    tblBrd = this.borderConvertToPixels((new (mergerCellA || mergerCellB)).getProperty(cell.properties, this.tblStyle, cell.conditionalFormatting, null));
            }
            if (!tblBrd)
                tblBrd = tableBorderInfo;
            return tblBrd;
        }
    }
}