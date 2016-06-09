module __aspxRichEdit {
    export enum TableGridErrorType {
        Error_1,

    }

    export type TableGridColumnConstructor = new (width: number, colSpan: number, type: TableWidthUnitType) => TableGridColumn;
    export type TableGridConstructor = new () => TableGrid;

    // when create TableGrid, colSpan can be != 1. But after colSpan = 1
    export class TableGridColumn {
        public width: number; // final width
        public colSpan: number;
        public type: TableWidthUnitType;

        constructor(width: number, colSpan: number, type: TableWidthUnitType) {
            this.width = width;
            this.colSpan = colSpan;
            this.type = type;
        }

        public substract(b: TableGridColumn): TableGridColumn {
            const colSpan: number = this.colSpan - b.colSpan;
            if (this.type == b.type)
                return new (this.getConstructor())(Math.max(0, this.width - b.width), colSpan, this.type);
            switch (this.type) {
                case TableWidthUnitType.FiftiethsOfPercent: return new (this.getConstructor())(b.width, colSpan, b.type);
                case TableWidthUnitType.ModelUnits: return new (this.getConstructor())(this.width, colSpan, this.type);
                default:
                    const type: TableWidthUnitType = this.type == TableWidthUnitType.Auto || b.type == TableWidthUnitType.Auto ?
                        TableWidthUnitType.Auto : TableWidthUnitType.Nil;
                    return new (this.getConstructor())(0, colSpan, type);
            }
        }

        public getConstructor(): TableGridColumnConstructor {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class TableGridFixedTableColumn extends TableGridColumn {
        constructor(width: number, colSpan: number, type: TableWidthUnitType) {
            super(width, colSpan, type);
        }

        public getConstructor(): TableGridColumnConstructor {
            return TableGridFixedTableColumn;
        }
    }

    export class TableGridAutoFitTableColumn extends TableGridColumn {
        constructor(width: number, colSpan: number, type: TableWidthUnitType) {
            super(width, colSpan, type);
        }

        public getConstructor(): TableGridColumnConstructor {
            return TableGridAutoFitTableColumn;
        }
    }

    export class TableGrid {
        public static errorsMap: { [errNum: number]: string };
        public errorCode: TableGridErrorType = 0;
        public table: Table;
        public columns: TableGridColumn[] = [];

        public tableCellGridInfos: TableCellGridInfo[][]; // indexes in grid layout
        public tableCellInfos: TableCellGridInfo[][]; // indexes in model, object identical
        public columnsXPositions: number[] = []; //  length = grid.columns.length + 1

        protected numIntervalsInModelUnits: number = 0;
        protected numIntervalsInPercents: number = 0;
        protected numIntervalsAsAuto: number = 0;
        protected numIntervalsAsNil: number = 0;
        
        protected widthInModelUnits: number = 0;
        protected widthInModelPercent: number = 0;

        constructor() {
            if (!TableGrid.errorsMap) {
                TableGrid.errorsMap = {};
                TableGrid.errorsMap[TableGridErrorType.Error_1] = "";
            }
        }

        public getWidthSomeColumns(startColumnIndex: number, cellSpans: number) {
            let width: number = 0;
            for (; cellSpans > 0; cellSpans-- , startColumnIndex++)
                width += this.columns[startColumnIndex].width;
            return width;
        }

        public static getTableGridColumnConstructor(table: Table): TableGridColumnConstructor {
            return new TablePropertiesMergerLayoutType().getProperty(table.properties) == TableLayoutType.Autofit ?
                TableGridAutoFitTableColumn : TableGridFixedTableColumn;
        }

        public static getTableGridConstructor(table: Table): TableGridConstructor {
            return new TablePropertiesMergerLayoutType().getProperty(table.properties) == TableLayoutType.Autofit ?
                AutoFitTableGrid : FixedTableGrid;
        }

        public addInterval(col: TableGridColumn) {
            this.columns.push(col);
        }

        protected processGrid(avaliableSpacing: number, unitConverter: IUnitConverter) {
            throw new Error(Errors.NotImplemented);
        }

        private checkColumnSpans() {
            for (let column of this.columns)
                if (column.colSpan != 1)
                    throw new Error(Errors.NotImplemented); // TODO WHEN RELEASE - DELETE 
        }

        private fillMembers(unitConverter: IUnitConverter) {
            for (let col of this.columns)
                switch (col.type) {
                    case TableWidthUnitType.ModelUnits:
                        this.numIntervalsInModelUnits++;
                        col.width = unitConverter.toPixels(col.width);
                        if (col.width < 1)
                            col.width = 1;
                        this.widthInModelUnits += col.width;
                        break;
                    case TableWidthUnitType.FiftiethsOfPercent:
                        this.numIntervalsInPercents++;
                        if (col.width < 1)
                            col.width = 1;
                        this.widthInModelPercent += col.width;
                        break;
                    case TableWidthUnitType.Auto:
                        this.numIntervalsAsAuto++;
                        col.width = 1;
                        break;
                    case TableWidthUnitType.Nil:
                        this.numIntervalsAsNil++
                        col.width = 1;
                        break;
                }
        }

        public static getTableGrid(subDocument: SubDocument, tableIndex: number, avaliableSpacing: number, unitConverter: IUnitConverter): TableGrid {
            const model: DocumentModel = subDocument.documentModel;
            const table: Table = subDocument.tables[tableIndex];
            const leftIndent: number = new TablePropertiesMergerIndent()
                .getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties)
                .asNumberNoPercentType(unitConverter);
            avaliableSpacing = Math.max(0, avaliableSpacing - leftIndent);

            const grid: TableGrid = new ColumnIntervalWidthCalculator(table).getIntervals();
            grid.table = table;
            grid.checkColumnSpans();
            grid.fillMembers(unitConverter);
            grid.processGrid(avaliableSpacing, unitConverter);
            return grid;
        }

        public initTableCellGridInfo() {
            this.tableCellGridInfos = [];
            this.tableCellInfos = [];

            const rows: TableRow[] = this.table.rows;
            for (let rowIndex: number = 0, row: TableRow; row = rows[rowIndex]; rowIndex++) {
                const tableCellGridInfo: TableCellGridInfo[] = [];
                const tableCellInfo: TableCellGridInfo[] = [];
                this.tableCellGridInfos.push(tableCellGridInfo);
                this.tableCellInfos.push(tableCellInfo);
                let currColSpan: number = row.gridBefore;
                for (let spacingIndex: number = currColSpan; spacingIndex > 0; spacingIndex--)
                    tableCellGridInfo.push(null);
                
                for (let cellIndex: number = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    const currCellGridInfo: TableCellGridInfo = cell.verticalMerging != TableCellMergingState.Continue ?
                        new TableCellGridInfo(rowIndex, currColSpan, [cellIndex]) :
                        this.tableCellGridInfos[rowIndex - 1][currColSpan].addCellIndex(cellIndex);
                    tableCellInfo.push(currCellGridInfo);
                    for (let spacingIndex: number = cell.columnSpan; spacingIndex > 0; spacingIndex--)
                        tableCellGridInfo.push(currCellGridInfo);
                    currColSpan += cell.columnSpan;
                }
            }

            let currXPos: number = 0;
            for (let col of this.columns) {
                this.columnsXPositions.push(currXPos);
                currXPos += col.width;
            }
            this.columnsXPositions.push(currXPos);
        }

        public static getCellWidth(tablePosition: TablePosition, grid: TableGrid): number {
            const cells: TableCell[] = tablePosition.row.cells;
            let colIndex: number = tablePosition.row.gridBefore;
            for (let cellIndex: number = 0; cellIndex < tablePosition.cellIndex; cellIndex++)
                colIndex += cells[cellIndex].columnSpan;
            const endColIndex: number = colIndex + tablePosition.cell.columnSpan;

            let resultWidth: number = 0;
            for (; colIndex < endColIndex; colIndex++)
                resultWidth += grid.columns[colIndex].width;
            return resultWidth;
        }
    }

    export class FixedTableGrid extends TableGrid {
        constructor() {
            super();
        }

        protected processGrid(avaliableSpacing: number, unitConverter: IUnitConverter) {
            let tablePreferredWidth: TableWidthUnit = this.table.preferredWidth;
            const actualWidth: number = tablePreferredWidth ? tablePreferredWidth.asNumber(unitConverter, avaliableSpacing) : 0;
            if (actualWidth > 0)
                this.setActualModelUnitColumnsWidths(actualWidth, true);
            else
                this.setActualModelUnitColumnsWidths(avaliableSpacing, false);
        }

        private setActualModelUnitColumnsWidths(avaliableSpacing: number, isTablePreferredWidthSet: boolean) {
            // 1) set columns with percent. 2) rest between modelUnit columns. 3) last - auto
            let widthColumnsWithPercentAndModelUnitTypeInPixels: number = 0;
            if (this.numIntervalsInPercents > 0) {
                // handle columns with percent
                const muitiplierForPercentColumns: number = this.widthInModelPercent > TableWidthUnit.MAX_PERCENT_WIDTH ?
                    this.widthInModelPercent / TableWidthUnit.MAX_PERCENT_WIDTH : 1;

                for (let col of this.columns)
                    if (col.type == TableWidthUnitType.FiftiethsOfPercent) {
                        col.width = Math.floor(col.width * muitiplierForPercentColumns / TableWidthUnit.MAX_PERCENT_WIDTH * avaliableSpacing);
                        widthColumnsWithPercentAndModelUnitTypeInPixels += col.width;
                    }

                // handle modelUnits columns
                widthColumnsWithPercentAndModelUnitTypeInPixels = this.fillAvaliableWidthWithModelUnitsColumn(avaliableSpacing, widthColumnsWithPercentAndModelUnitTypeInPixels);
            }
            else {
                // handle modelUnits columns
                if (isTablePreferredWidthSet) // can't width > width set table
                    widthColumnsWithPercentAndModelUnitTypeInPixels = this.fillAvaliableWidthWithModelUnitsColumn(avaliableSpacing, widthColumnsWithPercentAndModelUnitTypeInPixels);
                else // here can
                    for (let col of this.columns)
                        if (col.type == TableWidthUnitType.ModelUnits)
                            widthColumnsWithPercentAndModelUnitTypeInPixels += col.width;
            }

            this.setAutoAndNilColumns(avaliableSpacing, widthColumnsWithPercentAndModelUnitTypeInPixels);
        }

        private fillAvaliableWidthWithModelUnitsColumn(avaliableSpacing: number, widthColumnsWithPercentAndModelUnitTypeInPixels: number): number {
            const avaliableSpaceForModelUnitsColumns: number = avaliableSpacing - widthColumnsWithPercentAndModelUnitTypeInPixels;
            const widthOneModelUnitColumn: number = Math.max(1, Math.floor(avaliableSpaceForModelUnitsColumns / this.numIntervalsInModelUnits));
            for (let col of this.columns)
                if (col.type == TableWidthUnitType.ModelUnits) {
                    col.width = Math.floor(col.width / this.widthInModelUnits * avaliableSpaceForModelUnitsColumns); // in proportion reduce
                    widthColumnsWithPercentAndModelUnitTypeInPixels += col.width;
                }
            return widthColumnsWithPercentAndModelUnitTypeInPixels;
        }

        private setAutoAndNilColumns(avaliableSpacing: number, widthColumnsWithPercentAndModelUnitTypeInPixels: number) {
            const avaliableSpaceForAutoColumns: number = Math.max(0, avaliableSpacing - widthColumnsWithPercentAndModelUnitTypeInPixels - this.numIntervalsAsNil);
            const widthOneAutoColumn: number = Math.max(1, Math.floor(avaliableSpaceForAutoColumns / this.numIntervalsAsAuto));
            for (let col of this.columns) {
                switch (col.type) {
                    case TableWidthUnitType.Auto:
                        col.width = widthOneAutoColumn;
                    case TableWidthUnitType.Nil: // by default above set in 1 pixel
                    case TableWidthUnitType.FiftiethsOfPercent:
                        col.type = TableWidthUnitType.ModelUnits;
                        break;
                }
            }
            this.widthInModelUnits = widthColumnsWithPercentAndModelUnitTypeInPixels + this.numIntervalsAsNil + this.numIntervalsAsAuto * widthOneAutoColumn;
        }
    }

    export class AutoFitTableGrid extends TableGrid {
        constructor() {
            super();
        }

        protected processGrid(avaliableSpacing: number, unitConverter: IUnitConverter) {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class TableCellGridInfo {
        protected rowIndex: number;
        protected gridCellIndex: number;
        protected cellIndexes: number[];

        constructor(rowIndex: number, gridCellIndex: number, cellIndexes: number[]) {
            this.rowIndex = rowIndex;
            this.gridCellIndex = gridCellIndex;
            this.cellIndexes = cellIndexes;
        }
        
        public getStartRowIndex(): number {
            return this.rowIndex;
        }

        public getCellIndex(rowIndexInCell: number): number {
            return this.cellIndexes[rowIndexInCell];
        }

        public getNumRowsInCell(): number {
            return this.cellIndexes.length;
        }

        public getGridCellIndex(): number {
            return this.gridCellIndex;
        }

        public addCellIndex(index: number): TableCellGridInfo {
            this.cellIndexes.push(index);
            return this;
        }
    }






    //export class TableGridColumn {
    //    width: number;
    //    minWidth: number;
    //    maxWidth: number;
    //    preferredWidth: number;
    //    totalHorizontalMargins: number;
    //    percentBased: boolean;

    //    public static createTableGrid(intervals: ColumnIntervalWidth[]): TableGridColumn[] {
    //        const result: TableGridColumn[] = [];
    //        for (let interval of intervals) {
    //            const col: TableGridColumn = new TableGridColumn();
    //            result.push(col);
    //            col.width = interval.width;
    //            col.percentBased = interval.type == ColumnIntervalWidthType.PercentBased;
    //        }
    //        return result;
    //    }
    //}

    //export class TableGridCalculator {
    //    public static calculateTableGrid(boxIterator: ITextBoxIterator, percentBaseWidth: number, subDocument: SubDocument, table: Table, unitConverter: IUnitConverter, simpleView: boolean,
    //        maxTableWidth: number, allowTablesToExtendIntoMargins: boolean):
    //        TableGridColumn[]{
    //        const tablePreferredWidth: TableWidthUnit = new TablePropertiesMergerPreferredWidth().getProperty(table.properties);
    //        const tableLayoutType: TableLayoutType = new TablePropertiesMergerLayoutType()
    //            .getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, subDocument.documentModel.defaultTableProperties);

    //        ParagraphWidthManipulator.calculateTableParagraphsWidth(boxIterator, subDocument.paragraphs, table);
    //        const intervals: ColumnIntervalWidth[] = ColumnIntervalWidthCalculator.getIntervals(subDocument.documentModel, table, unitConverter);
    //        if (tablePreferredWidth.type == TableWidthUnitType.Auto)
    //            percentBaseWidth = EstimatedTableWidthCalculator.calculateEstimatedTableWidth(subDocument, table.index, intervals, percentBaseWidth, unitConverter);
    //        const flex: boolean = EstimatedTableWidthCalculator.applyPercentWidth(intervals, percentBaseWidth);

    //        const grid: TableGridColumn[] = TableGridColumn.createTableGrid(intervals);
    //        EstimatedTableWidthCalculator.applyCellContentWidth(subDocument, unitConverter, grid, table, percentBaseWidth, simpleView);
    //        EstimatedTableWidthCalculator.autofitTable(subDocument, unitConverter, grid, table, percentBaseWidth, simpleView, maxTableWidth, allowTablesToExtendIntoMargins,
    //            flex);
            
    //        if (tablePreferredWidth.type == TableWidthUnitType.ModelUnits && tablePreferredWidth.value > 0 && tableLayoutType == TableLayoutType.Fixed)
    //            AutofitTableLayoutCalculator.compressTableGrid(grid, 0, grid.length - 1, tablePreferredWidth.asNumberNoPercentType(unitConverter));

    //        // wrong class
    //        //const compressHelper: TableCompressHelper = new TableCompressHelper(grid, table);
    //        //const margins: number[] = compressHelper.work();
    //        //compressHelper.compress(margins);

    //        //ReplaceCachedTableLayoutInfoIfNeeded(table, result, percentBaseWidth);
    //        return grid;
    //    }

        
    //}
}