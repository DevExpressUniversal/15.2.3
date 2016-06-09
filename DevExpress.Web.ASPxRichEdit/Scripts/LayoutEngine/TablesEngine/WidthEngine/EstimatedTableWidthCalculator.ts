module __aspxRichEdit {
    export class WidthBounds implements ICloneable<WidthBounds> {
        min: number;
        max: number;

        constructor(min: number, max: number) {
            this.min = min;
            this.max = max;
        }

        clone(): WidthBounds {
            return new WidthBounds(this.min, this.max);
        }

        choiseMaxWidthBounds(widthBounds: WidthBounds) {
            if (this.min < widthBounds.min)
                this.min = widthBounds.min;
            if (this.max < widthBounds.max)
                this.max = widthBounds.max;
        }
    }

//    export class EstimatedTableWidthCalculator {
//        // IMPORTANT!!! CALL BEFORE ParagraphWidthManipulator.calculateTableParagraphsWidth(boxIterator, paragraphs)
//        public static calculateEstimatedTableWidth(subDocument: SubDocument, tableIndex: number, gridIntervals: ColumnIntervalWidth[], percentBaseWidth: number,
//            unitConverter: IUnitConverter): number {
//            let totalWidthInModelUnits: number = 0;
//            let totalWidthInPercents: number = 0;

//            for (let interval of gridIntervals)
//                if (interval.type == ColumnIntervalWidthType.PercentBased)
//                    totalWidthInPercents += interval.width;
//                else
//                    totalWidthInModelUnits += interval.width;

//            if (totalWidthInPercents == 0)
//                return Math.min(totalWidthInModelUnits, percentBaseWidth);

//            if (totalWidthInModelUnits > 0) {
//                const restWidthInPercent: number = TableWidthUnit.MAX_PERCENT_WIDTH - totalWidthInPercents;
//                if (restWidthInPercent <= 0)
//                    return percentBaseWidth;
//                return Math.min(totalWidthInModelUnits / restWidthInPercent * TableWidthUnit.MAX_PERCENT_WIDTH, percentBaseWidth);
//            }

//            // here always percentWidth > 0, modelUnitWidth == 0, some intervals can be NotSet
//            let estimatedTableWidth: number = 0;
//            const table: Table = subDocument.tables[tableIndex];
//            const rows: TableRow[] = table.rows;
//            for (let rowIndex: number = 0, row: TableRow; row = rows[rowIndex]; rowIndex++) {
//                let columnIndex: number = row.gridBefore;
//                const cells: TableCell[] = row.cells;
//                for (let cellIndex: number = 0, cell: TableCell; cell = cells[cellIndex]; cellIndex++) {
//                    const interval: ColumnIntervalWidth = gridIntervals[columnIndex];
//                    if (interval.type == ColumnIntervalWidthType.PercentBased) {
//                        const topLevelTablePos: TablePosition = new TablePosition(table, rowIndex, cellIndex).init();
//                        var cellStartPos: number = cell.startParagraphPosition.value;
//                        const firstParIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - cellStartPos);
//                        const nestedTableIterator: NestedTableIterator = new NestedTableIterator(subDocument, firstParIndex, topLevelTablePos);
//                        const widthInfo: CellWidthBoundsWithHorizMargins = CellWidthBoundsCalculator.calculateCellWidthsBound(
//                            percentBaseWidth, false, unitConverter, nestedTableIterator);
//                        // wtf ...
//                        estimatedTableWidth = Math.max(estimatedTableWidth, widthInfo.max); // todo why max?? totalWidthInPercents > 0 always.
//                        if (interval.width > 0) // ?? % == 0 ???
//                            estimatedTableWidth = Math.max(widthInfo.min * TableWidthUnit.MAX_PERCENT_WIDTH / interval.width, estimatedTableWidth); // here min
//                    }
//                    columnIndex += cell.columnSpan;
//                }
//            }
//            // TODO think here need be return Math.min(estimatedTableWidth / totalWidthInPercents * 5000, percentBaseWidth);
//            return Math.min(estimatedTableWidth, percentBaseWidth) * TableWidthUnit.MAX_PERCENT_WIDTH / totalWidthInPercents;
//        }

//        public static getNestedTableWidth(percentBaseWidth: number, simpleView: boolean, unitConverter: IUnitConverter, nestedTableIterator: NestedTableIterator): WidthBounds {
//            const table: Table = nestedTableIterator.currTablePosition.table;
//            const cellWidthCollector: CellWidthBoundsCollector = CellWidthBoundsCollector.getCollector(table);
//            for (let rowIndex: number = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
//                const cells: TableCell[] = row.cells;
//                let columnIndex: number = row.gridBefore;
//                for (let cellIndex: number = 0, cell: TableCell; cell = cells[cellIndex]; cellIndex++) {
//                    const cellWidthsInfo: WidthBounds = CellWidthBoundsCalculator.calculateCellWidthsBound(0, simpleView, unitConverter, nestedTableIterator);
//                    const endColumnCellColumnIndex: number = columnIndex + cell.columnSpan;
//                    cellWidthCollector.addInfo(new CellWidthBounds(columnIndex, endColumnCellColumnIndex, cellWidthsInfo.min, cellWidthsInfo.max));
//                    columnIndex = endColumnCellColumnIndex;
//                }
//            }
//            let result: WidthBounds = cellWidthCollector.calculateTableWidth();
//            const tablePreferredWidth: TableWidthUnit = new TablePropertiesMergerPreferredWidth().getProperty(table.properties)
//            if (tablePreferredWidth.type == TableWidthUnitType.ModelUnits && tablePreferredWidth.type == TableWidthUnitType.FiftiethsOfPercent) {
//                const tableWidth: number = tablePreferredWidth.asNumber(unitConverter, 0);
//                result = new WidthBounds(Math.max(result.min, tableWidth), Math.max(result.min, tableWidth));
//            }
//            return result;
//        }

//        // return flex or not
//        public static applyPercentWidth(intervals: ColumnIntervalWidth[], percentBaseWidth: number): boolean {
//            let totalPercentWidth: number = 0;
//            let totalUnitWidth: number = 0;
//            let unsetCount: number = 0;
//            for (let interval of intervals) {
//                switch (interval.type) {
//                    case ColumnIntervalWidthType.PercentBased:
//                        totalPercentWidth += interval.width;
//                        if (totalPercentWidth > TableWidthUnit.MAX_PERCENT_WIDTH)
//                            interval.width = Math.max(0, TableWidthUnit.MAX_PERCENT_WIDTH - totalPercentWidth + interval.width);
//                        break;
//                    case ColumnIntervalWidthType.ModelUnitBased:
//                        totalUnitWidth += interval.width;
//                        break;
//                    case ColumnIntervalWidthType.NotSet:
//                        unsetCount++;
//                        break;
//                }
//            }

//            const flex: boolean = (totalUnitWidth == 0);
//            if (totalPercentWidth == 0)
//                return flex;

//            let restUnitWidth: number = Math.max(0, percentBaseWidth - totalUnitWidth);
//            if (restUnitWidth > 0 && unsetCount > 0) {
//                let restPercentWidth: number = TableWidthUnit.MAX_PERCENT_WIDTH - totalPercentWidth;
//                if (restPercentWidth > 0) {
//                    for (let interval of intervals) {
//                        if (unsetCount <= 0)
//                            break;

//                        if (interval.type != ColumnIntervalWidthType.NotSet)
//                            continue;
//                        interval.type = ColumnIntervalWidthType.PercentBased;
//                        let percentWidth: number = restPercentWidth / unsetCount;
//                        interval.width = percentWidth;
//                        unsetCount--;
//                        restPercentWidth -= percentWidth;
//                    }
//                    totalPercentWidth = TableWidthUnit.MAX_PERCENT_WIDTH;
//                }
//            }

//            for (let interval of intervals) {
//                if (interval.type != ColumnIntervalWidthType.PercentBased)
//                    continue;
//                interval.type = ColumnIntervalWidthType.ModelUnitBased;
//                const newWidth: number = totalPercentWidth > 0 ? restUnitWidth * interval.width / totalPercentWidth : 0;
//                totalPercentWidth -= interval.width;
//                interval.width = Math.max(1, newWidth);
//                restUnitWidth -= newWidth;
//            }
//            return flex;
//        }
        
//        public static applyCellContentWidth(subDocument: SubDocument, unitConverter: IUnitConverter, grid: TableGridColumn[], table: Table, percentBaseWidth: number,
//            simpleView: boolean) {
//            let tablePosition: TablePosition = new TablePosition(table, -1, 0);
//            while (tablePosition.moveToNextRow()) {
//                tablePosition.cellIndex = -1;
//                let columnIndex: number = tablePosition.row.gridBefore;
//                while (tablePosition.moveToNextCell()) {
//                    if (tablePosition.cell.columnSpan == 1)
//                        EstimatedTableWidthCalculator.applyCellContentWidthWithoutSpan(subDocument, unitConverter, grid, tablePosition, columnIndex, percentBaseWidth, simpleView);
//                    columnIndex += tablePosition.cell.columnSpan;
//                }
//            }

//            for (let col of grid)
//                if (col.minWidth == 0 && col.maxWidth == 0)
//                    col.maxWidth = col.minWidth = col.width;
         
//            tablePosition = new TablePosition(table, -1, 0);
//            while (tablePosition.moveToNextRow()) {
//                tablePosition.cellIndex = -1;
//                let columnIndex: number = tablePosition.row.gridBefore;
//                while (tablePosition.moveToNextCell()) {
//                    if (tablePosition.cell.columnSpan == 1)
//                        EstimatedTableWidthCalculator.applyCellContentWidthWithSpan(subDocument, unitConverter, grid, tablePosition, columnIndex, percentBaseWidth, simpleView);
//                    columnIndex += tablePosition.cell.columnSpan;
//                }
//            }  

//            for (let col of grid) {
//                col.maxWidth = Math.max(col.maxWidth, 1);
//                col.width = Math.max(col.maxWidth, 1); // TODO???
//            }
//        }

//        public static applyCellContentWidthWithoutSpan(subDocument: SubDocument, unitConverter: IUnitConverter, grid: TableGridColumn[], tablePosition: TablePosition,
//            columnIndex: number, percentBaseWidth: number, simpleView: boolean) {
//            const cell: TableCell = tablePosition.cell;
//            const table: Table = tablePosition.table;
//            const cellStartPos: number = cell.startParagraphPosition.value;
//            const firstParIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - cellStartPos);
//            const nestedTableIterator: NestedTableIterator = new NestedTableIterator(subDocument, firstParIndex, tablePosition);
//            const info: CellWidthBoundsWithHorizMargins = CellWidthBoundsCalculator.calculateCellWidthsBound(percentBaseWidth, simpleView, unitConverter, nestedTableIterator);

//            const cellMinWidth: number = Math.max(1, info.min);
//            let cellMaxWidth: number = Math.max(1, info.max);
//            if (grid[columnIndex].width > 0)
//                cellMaxWidth = Math.max(cellMinWidth, grid[columnIndex].width); //actually, it's grid[columnIndex].Width, except the case, when info.MinWidth > grid[columnIndex].Width

//            grid[columnIndex].minWidth = Math.max(cellMinWidth, grid[columnIndex].minWidth);
//            grid[columnIndex].maxWidth = Math.max(cellMaxWidth, grid[columnIndex].maxWidth);
//            grid[columnIndex].totalHorizontalMargins = Math.max(grid[columnIndex].totalHorizontalMargins, info.totalHorizontalMargins);

//            const noWrap: boolean = new TableCellPropertiesMergerNoWrap()
//                .getProperty(cell.properties, table.style, cell.conditionalFormatting, subDocument.documentModel.defaultTableCellProperties);

//            let preferredWidth: number = new TableCellPropertiesMergerPreferredWidth().getProperty(cell.properties, table.style,
//                cell.conditionalFormatting, subDocument.documentModel.defaultTableCellProperties).asNumber(unitConverter, percentBaseWidth);
//            if (preferredWidth <= 0 && noWrap)
//                preferredWidth = grid[columnIndex].maxWidth;


//            if (preferredWidth > 0) {
//                if (!noWrap)
//                    preferredWidth = Math.max(preferredWidth, cellMinWidth);
//                grid[columnIndex].preferredWidth = Math.max(grid[columnIndex].preferredWidth, preferredWidth);
//                grid[columnIndex].maxWidth = grid[columnIndex].preferredWidth;
//            }
//            else {
//                const totalColumnsMinWidth: number = grid[columnIndex].minWidth;
//                if (cellMinWidth > totalColumnsMinWidth) {
//                    const gridColumn: TableGridColumn = grid[columnIndex]
//                    let rest: number = cellMinWidth - totalColumnsMinWidth;
//                    let availableWidth: number = gridColumn.maxWidth - gridColumn.minWidth;
//                    let totalRestMaxWidth: number = gridColumn.maxWidth;

//                    if (availableWidth < rest) {
//                        gridColumn.maxWidth = gridColumn.maxWidth + gridColumn.maxWidth * rest / totalRestMaxWidth;
//                        gridColumn.minWidth = gridColumn.maxWidth;
//                    }
//                    else
//                        gridColumn.minWidth += (gridColumn.maxWidth - gridColumn.minWidth) * rest / availableWidth;
//                }
//            }
//        }

//        private static applyCellContentWidthWithSpan(subDocument: SubDocument, unitConverter: IUnitConverter, grid: TableGridColumn[], tablePosition: TablePosition,
//            startColumnIndex: number, percentBaseWidth: number, simpleView: boolean) {
//            const cell: TableCell = tablePosition.cell;
//            const table: Table = tablePosition.table;
//            const cellStartPos: number = cell.startParagraphPosition.value;
//            const firstParIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - cellStartPos);
//            const nestedTableIterator: NestedTableIterator = new NestedTableIterator(subDocument, firstParIndex, tablePosition);
//            const info: CellWidthBoundsWithHorizMargins = CellWidthBoundsCalculator.calculateCellWidthsBound(percentBaseWidth, simpleView, unitConverter, nestedTableIterator);

//            const endColumnIndex: number = startColumnIndex + cell.columnSpan - 1;
//            let gridWidth: number = 0;
//            for (let columnIndex: number = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
//                gridWidth += grid[columnIndex].width;

//            const cellMinWidth: number = Math.max(1, info.min);
//            let cellMaxWidth: number = Math.max(1, info.max);
//            if (gridWidth > 0)
//                cellMaxWidth = Math.max(cellMinWidth, gridWidth); //actually, it's grid[columnIndex].Width, except the case, when info.MinWidth > grid[columnIndex].Width

//            let preferredWidth: number = new TableCellPropertiesMergerPreferredWidth().getProperty(cell.properties, table.style,
//                cell.conditionalFormatting, subDocument.documentModel.defaultTableCellProperties).asNumber(unitConverter, percentBaseWidth);
//            if (preferredWidth > 0) {
//                preferredWidth = Math.max(cellMinWidth, preferredWidth);
//                cellMaxWidth = preferredWidth;
//            }

//            let gridMinWidth = 0;
//            for (let columnIndex: number = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
//                gridMinWidth += grid[columnIndex].minWidth;

//            if (cellMinWidth > gridMinWidth)
//                EstimatedTableWidthCalculator.enlargeColumnsMinWidth(grid, startColumnIndex, endColumnIndex, cellMinWidth);

//            let gridMaxWidth: number = 0;
//            for (let columnIndex: number = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
//                gridMaxWidth += grid[columnIndex].maxWidth;

//            if (cellMaxWidth > gridMaxWidth)
//                EstimatedTableWidthCalculator.enlargeColumnsMaxWidth(grid, startColumnIndex, endColumnIndex, gridMaxWidth, cellMaxWidth);

//            let gridTotalMargins: number = 0;
//            for (let columnIndex: number = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
//                gridTotalMargins += grid[columnIndex].totalHorizontalMargins;

//            if (info.totalHorizontalMargins > gridTotalMargins)
//                EstimatedTableWidthCalculator.enlargeColumnsHorizontalMargins(grid, startColumnIndex, endColumnIndex, gridTotalMargins, info.totalHorizontalMargins);
//        }

//        private static enlargeColumnsMinWidth(grid: TableGridColumn[], startColumnIndex: number, endColumnIndex: number, newWidth: number) {
//            const hasColumnsWithoutPreferredWidth: boolean = EstimatedTableWidthCalculator.hasColumnsWithoutPreferredWidth(grid, startColumnIndex, endColumnIndex);
//            let zeroMinWidthCount: number = 0;
//            let existingMinWidth: number = 0;
//            for (let columnIndex: number = endColumnIndex; columnIndex >= startColumnIndex; columnIndex--) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                if (gridColumn.minWidth == 0 && gridColumn.maxWidth == 0)
//                    zeroMinWidthCount++;
//                else
//                    existingMinWidth += gridColumn.minWidth;
//            }

//            let rest: number = 0;
//            for (let columnIndex: number = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                rest += gridColumn.maxWidth + gridColumn.minWidth;
//            }

//            const equalSpace: boolean = rest == 0;

//            if (equalSpace || zeroMinWidthCount > 0) {
//                rest = endColumnIndex - startColumnIndex + 1;
//                newWidth -= existingMinWidth;
//            }

//            for (let columnIndex: number = endColumnIndex; columnIndex >= startColumnIndex; columnIndex--) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                if (!hasColumnsWithoutPreferredWidth || gridColumn.preferredWidth == 0) {
//                    if (zeroMinWidthCount > 0 && (gridColumn.minWidth > 0 || gridColumn.maxWidth > 0))
//                        continue;
//                    const factor: number = (equalSpace || zeroMinWidthCount > 0) ? 1 : (gridColumn.minWidth + gridColumn.maxWidth);
//                    const newMinWidth: number = factor * newWidth / rest;
//                    rest -= factor;
//                    newWidth -= newMinWidth;
//                    gridColumn.minWidth = Math.max(newMinWidth, gridColumn.minWidth);
//                    gridColumn.maxWidth = Math.max(gridColumn.minWidth, gridColumn.maxWidth);
//                }
//            }
//        }
        
//        private static enlargeColumnsMaxWidth(grid: TableGridColumn[], startColumnIndex: number, endColumnIndex: number, oldWidth: number, newWidth: number) {
//            const hasColumnsWithoutPreferredWidth: boolean = EstimatedTableWidthCalculator.hasColumnsWithoutPreferredWidth(grid, startColumnIndex, endColumnIndex);
//            let rest: number = oldWidth;
//            for (let columnIndex: number = endColumnIndex; columnIndex >= startColumnIndex; columnIndex--) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                if (!hasColumnsWithoutPreferredWidth || gridColumn.preferredWidth == 0) {
//                    const newMaxWidth: number = rest != 0 ? gridColumn.maxWidth * newWidth / rest : 0;
//                    rest -= gridColumn.maxWidth;
//                    newWidth -= newMaxWidth;
//                    if (rest < 0)
//                        rest = 0;
//                    if (newWidth < 0)
//                        newWidth = 0;
//                    gridColumn.maxWidth = Math.max(1, newMaxWidth);
//                }
//            }
//        }

//        private static hasColumnsWithoutPreferredWidth(grid: TableGridColumn[], startColumnIndex: number, endColumnIndex: number): boolean {
//            for (let columnIndex = startColumnIndex; columnIndex <= endColumnIndex; columnIndex++)
//                if (grid[columnIndex].preferredWidth == 0)
//                    return true;
//            return false;
//        }

//        private static enlargeColumnsHorizontalMargins(grid: TableGridColumn[], startColumnIndex: number, endColumnIndex: number, oldWidth: number, newWidth: number) {
//            let totalDelta: number = newWidth - oldWidth;
//            const equalSpace: boolean = oldWidth == 0;
//            let totalCount: number = endColumnIndex - startColumnIndex + 1;
//            for (let columnIndex: number = endColumnIndex; columnIndex >= startColumnIndex && totalDelta > 0; columnIndex--) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                const delta: number = equalSpace ? totalDelta / totalCount : totalDelta * gridColumn.totalHorizontalMargins / oldWidth;
//                totalDelta -= delta;
//                oldWidth -= gridColumn.totalHorizontalMargins;
//                gridColumn.totalHorizontalMargins += delta;
//                totalCount--;
//            }
//        }

//        public static autofitTable(subDocument: SubDocument, unitConverter: IUnitConverter, grid: TableGridColumn[], table: Table, percentBaseWidth: number, simpleView: boolean,
//            maxTableWidth: number, allowTablesToExtendIntoMargins: boolean, flex: boolean) {
//            let width: number = 0;
//            for (let gridColumn of grid)
//                width += gridColumn.width;

//            let minWidth: number = 0;
//            for (let gridColumn of grid)
//                width += gridColumn.minWidth;

//            const tablePreferredWidth: TableWidthUnit = new TablePropertiesMergerPreferredWidth().getProperty(table.properties);

//            if (tablePreferredWidth.type == TableWidthUnitType.ModelUnits || tablePreferredWidth.type == TableWidthUnitType.FiftiethsOfPercent) {
//                const preferredTableWidth: number = tablePreferredWidth.asNumber(unitConverter, percentBaseWidth);
//                if (width >= preferredTableWidth) {
//                    if (minWidth < maxTableWidth || preferredTableWidth > maxTableWidth)
//                        EstimatedTableWidthCalculator.compressTableGridToPreferredWidth(grid, width, preferredTableWidth);
//                    else
//                        EstimatedTableWidthCalculator.compressTableGridToColumnWidth(grid, width, maxTableWidth, allowTablesToExtendIntoMargins);
//                }
//                else if (width < preferredTableWidth)
//                    EstimatedTableWidthCalculator.enlargeTableGridToPreferredWidth(grid, width, preferredTableWidth);
//            }
//            else {
//                const tableLayout: TableLayoutType = new TablePropertiesMergerLayoutType()
//                    .getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, subDocument.documentModel.defaultTableProperties);
//                if (width > maxTableWidth && (tableLayout == TableLayoutType.Autofit || simpleView))
//                    EstimatedTableWidthCalculator.compressTableGridToColumnWidth(grid, width, maxTableWidth, allowTablesToExtendIntoMargins);
//                else
//                    EstimatedTableWidthCalculator.compressRelativelySizedTable(grid, table, subDocument, flex);
//            }
//        }

//        private static compressTableGridToPreferredWidth(grid: TableGridColumn[], oldWidth: number, newWidth: number) {
//            let totalMinWidth: number = 0;
//            let totalMaxWidth: number = 0;
//            let totalPreferredWidth: number = 0;
//            let totalMinWidthForNonPreferredWidthColumn: number = 0;

//            for (let gridColumn of grid) {
//                totalMinWidth += gridColumn.minWidth;
//                totalMaxWidth += gridColumn.maxWidth;
//                totalPreferredWidth += gridColumn.preferredWidth;
//                if (gridColumn.preferredWidth == 0)
//                    totalMinWidthForNonPreferredWidthColumn += gridColumn.minWidth;
//            }

//            let totalMaxWidthForNonPreferredWidthColumn: number = totalMaxWidth - totalPreferredWidth;
//            if (newWidth <= totalMinWidth) {
//                for (let gridColumn of grid)
//                    gridColumn.width = gridColumn.minWidth;
//                return;
//            }

//            for (let gridColumn of grid) {
//                if (gridColumn.maxWidth < gridColumn.minWidth) {
//                    const minWidth: number = gridColumn.minWidth;
//                    gridColumn.minWidth = gridColumn.maxWidth;
//                    gridColumn.maxWidth = minWidth;
//                    gridColumn.width = gridColumn.maxWidth;
//                }
//            }

//            if (newWidth - totalMinWidthForNonPreferredWidthColumn < totalPreferredWidth) {
//                let totalDelta: number = oldWidth - newWidth;
//                for (let gridColumn of grid) {
//                    if (gridColumn.preferredWidth == 0) {
//                        totalDelta -= gridColumn.width - gridColumn.minWidth;
//                        gridColumn.width = gridColumn.minWidth;
//                    }
//                }

//                let rest: number = totalPreferredWidth - (totalMinWidth - totalMinWidthForNonPreferredWidthColumn);
//                for (let columnIndex: number = grid.length - 1, gridColumn: TableGridColumn; (totalDelta > 0) && (gridColumn = grid[columnIndex]); columnIndex--) {
//                    if (gridColumn.preferredWidth > 0) {
//                        const delta: number = (gridColumn.preferredWidth - gridColumn.minWidth) / (rest) * totalDelta;
//                        gridColumn.width = Math.max(gridColumn.width - delta, 1);
//                        rest -= gridColumn.preferredWidth - gridColumn.minWidth;
//                        totalDelta -= delta;
//                    }
//                }
//                return;
//            }
            
//            let totalDelta: number = oldWidth - newWidth;
//            let rest: number = totalMaxWidthForNonPreferredWidthColumn - totalMinWidthForNonPreferredWidthColumn;
//            for (let columnIndex: number = grid.length - 1, gridColumn: TableGridColumn; gridColumn = grid[columnIndex]; columnIndex--) {
//                if (gridColumn.preferredWidth == 0) {
//                    const delta: number = Math.floor((gridColumn.maxWidth - gridColumn.minWidth) * totalDelta / rest);
//                    gridColumn.width = gridColumn.width - delta;
//                    rest -= gridColumn.maxWidth - gridColumn.minWidth;
//                    totalDelta -= delta;
//                }
//            }
//        }

//        private static compressTableGridToColumnWidth(grid: TableGridColumn[], oldWidth: number, newWidth: number, allowTablesToExtendIntoMargins: boolean) {
//            let totalMinWidth: number = 0;
//            for (let gridColumn of grid)
//                totalMinWidth += gridColumn.minWidth;

//            if (totalMinWidth <= newWidth) {
//                EstimatedTableWidthCalculator.compressTableGridToPreferredWidth(grid, oldWidth, newWidth);
//                return;
//            }
//            if (allowTablesToExtendIntoMargins) {
//                EstimatedTableWidthCalculator.compressTableGridToPreferredWidth(grid, oldWidth, totalMinWidth);
//                return;
//            }

//            let totalWidth: number = newWidth;
//            let rest: number = totalMinWidth;
//            for (let gridColumn of grid) {
//                const newColumnWidth: number = rest > 0 ? Math.max(gridColumn.minWidth * totalWidth / rest, 1) : 1;
//                gridColumn.width = newColumnWidth;
//                totalWidth -= newColumnWidth;
//                rest -= gridColumn.minWidth;

//            }
//        }

//        private static enlargeTableGridToPreferredWidth(grid: TableGridColumn[], oldWidth: number, newWidth: number) {
//            let totalMaxWidth: number = 0;
//            for (let gridColumn of grid)
//                totalMaxWidth += gridColumn.maxWidth;

//            let totalPreferredWidth: number = 0;
//            for (let gridColumn of grid)
//                totalMaxWidth += gridColumn.preferredWidth;

            
//            let totalDelta: number = newWidth - oldWidth;
//            let rest: number = totalMaxWidth;
//            let hasColumnsWithoutPreferredWidth: boolean = EstimatedTableWidthCalculator.hasColumnsWithoutPreferredWidth(grid, 0, grid.length - 1);
//            if (hasColumnsWithoutPreferredWidth)
//                rest -= totalPreferredWidth;
//            for (let columnIndex: number = grid.length - 1, gridColumn: TableGridColumn; gridColumn = grid[columnIndex]; columnIndex--) {
//                if (!hasColumnsWithoutPreferredWidth || gridColumn.preferredWidth == 0) {
//                    const delta: number = gridColumn.maxWidth * totalDelta / rest;
//                    gridColumn.width = gridColumn.width + delta;
//                    rest -= gridColumn.maxWidth;
//                    totalDelta -= delta;
//                }
//            }
//            return;
//        }

//        private static compressRelativelySizedTable(grid: TableGridColumn[], table: Table, subDocument: SubDocument, flex: boolean) {
//            const tablePreferredWidth: TableWidthUnit = new TablePropertiesMergerPreferredWidth().getProperty(table.properties);
//            const tableLayoutType: TableLayoutType = new TablePropertiesMergerLayoutType()
//                .getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, subDocument.documentModel.defaultTableProperties);
//            if (!(tablePreferredWidth.type == TableWidthUnitType.Auto && tableLayoutType == TableLayoutType.Autofit && flex))
//                return;

//            let ratio: number = 0.0;
//            for (let row of table.rows) {
//                for (let cell of row.cells) {
//                    if (cell.verticalMerging == TableCellMergingState.Continue)
//                        continue;
//                    //if (cell.LayoutProperties.ContentWidthsInfo.MaxWidth >= cell.LayoutProperties.ContainerWidthsInfo.MaxWidth)
//                    //    return;
//                    //const q: number = (double) cell.LayoutProperties.ContentWidthsInfo.MaxWidth / (double) cell.LayoutProperties.ContainerWidthsInfo.MaxWidth;
//                    //if (q > ratio)
//                    //    ratio = q;
//                }
//            }

//            for (let gridColumn of grid)
//                gridColumn.width = Math.max(gridColumn.width * ratio, gridColumn.minWidth);
//        }
//    }
}