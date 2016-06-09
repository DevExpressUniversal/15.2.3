module __aspxRichEdit {
    export class ColumnIntervalWidthCalculator {
        private table: Table;
        private tableGridColumnConstructor: TableGridColumnConstructor;
        private tableGridConstructor: TableGridConstructor;

        constructor(table: Table) {
            this.table = table;
            // it's right
            //this.tableGridColumnConstructor = TableGrid.getTableGridColumnConstructor(table);
            //this.tableGridConstructor = TableGrid.getTableGridConstructor(table);
            // it's temporary
            this.tableGridColumnConstructor = TableGridFixedTableColumn;
            this.tableGridConstructor = FixedTableGrid;
        }

        // return intervals in model units no in pixels
        public getIntervals(): TableGrid{
            const rows: TableRow[] = this.table.rows;
            let currRowTableGrid: TableGrid = this.createOneRowTableGrid(rows[0]);
            for (let nextRowIndex: number = 1, nextRow: TableRow; nextRow = rows[nextRowIndex]; nextRowIndex++) {
                const nextRowTableGrid: TableGrid = this.createOneRowTableGrid(nextRow)
                currRowTableGrid = this.mergeRowColumnIntervals(currRowTableGrid, nextRowTableGrid);
            }
            return currRowTableGrid;
        }

        private createOneRowTableGrid(row: TableRow): TableGrid {
            const grid: TableGrid = new this.tableGridConstructor();
            if (row.gridBefore > 0)
                grid.addInterval(new this.tableGridColumnConstructor(row.widthBefore.value, row.gridBefore, row.widthBefore.type));

            for (let cell of row.cells) {
                const cellPreferredWidth: TableWidthUnit = cell.preferredWidth;
                grid.addInterval(new this.tableGridColumnConstructor(cellPreferredWidth.value, cell.columnSpan, cellPreferredWidth.type));
            }

            if (row.gridAfter > 0)
                grid.addInterval(new this.tableGridColumnConstructor(row.widthAfter.value, row.gridAfter, row.widthAfter.type));
            return grid;
        }

        private mergeRowColumnIntervals(currRowTableGrid: TableGrid, nextRowTableGrid: TableGrid): TableGrid {
            const mergedTableGrid: TableGrid = new this.tableGridConstructor();
            const currRowIterator: ColumnIntervalWidthIterator = new ColumnIntervalWidthIterator(currRowTableGrid);
            const nextRowIterator: ColumnIntervalWidthIterator = new ColumnIntervalWidthIterator(nextRowTableGrid);
            while (!currRowIterator.endOfIntervals() && !nextRowIterator.endOfIntervals()) {
                const currIntervalSpan: number = currRowIterator.interval.colSpan;
                const nextIntervalSpan: number = nextRowIterator.interval.colSpan;
                if (currIntervalSpan > nextIntervalSpan)
                    this.processDependedIntervals(currRowIterator, nextRowIterator, mergedTableGrid);
                else if (nextIntervalSpan > currIntervalSpan)
                    this.processDependedIntervals(nextRowIterator, currRowIterator, mergedTableGrid);
                else {
                    const newInterval: TableGridColumn = this.mergeIntervalsDifferentRows(currRowIterator.interval, nextRowIterator.interval);
                    mergedTableGrid.addInterval(newInterval);
                    currRowIterator.advance(newInterval);
                    nextRowIterator.advance(newInterval);
                }
            }
            this.copyRestIntervals(currRowIterator, mergedTableGrid); // only in one terator can be intervals
            this.copyRestIntervals(nextRowIterator, mergedTableGrid);
            return mergedTableGrid;
        }

        // first place interval with right spanNums
        private mergeIntervalsDifferentRows(currRowTableGridColumn: TableGridColumn, nextRowTableGridColumn: TableGridColumn): TableGridColumn {
            const colSpan: number = currRowTableGridColumn.colSpan;
            if (currRowTableGridColumn.type == nextRowTableGridColumn.type)
                return new this.tableGridColumnConstructor(Math.max(currRowTableGridColumn.width, nextRowTableGridColumn.width), colSpan, currRowTableGridColumn.type);

            if (currRowTableGridColumn.type == TableWidthUnitType.FiftiethsOfPercent) // (PercentBased + ModelUnitBased), (PercentBased + notSet)
                return this.mergeIntervalsDifferentRowsDifferentTypes(currRowTableGridColumn, nextRowTableGridColumn, colSpan);

            if (nextRowTableGridColumn.type == TableWidthUnitType.FiftiethsOfPercent) // (ModelUnitBased + PercentBased), (notSet + PercentBased)
                return this.mergeIntervalsDifferentRowsDifferentTypes(nextRowTableGridColumn, currRowTableGridColumn, colSpan);

            if (currRowTableGridColumn.type == TableWidthUnitType.ModelUnits) // (ModelUnitBased + notSet)
                return new this.tableGridColumnConstructor(currRowTableGridColumn.width, colSpan, TableWidthUnitType.ModelUnits);

            // (notSet + modelUnit)
            return new this.tableGridColumnConstructor(nextRowTableGridColumn.width, colSpan, TableWidthUnitType.ModelUnits);
        }

        private mergeIntervalsDifferentRowsDifferentTypes(percentBasedInterval: TableGridColumn, anotherBasedInterval: TableGridColumn, colSpan: number): TableGridColumn {
            return anotherBasedInterval.width > 0 ?
                new this.tableGridColumnConstructor(anotherBasedInterval.width, colSpan, anotherBasedInterval.type) :
                new this.tableGridColumnConstructor(percentBasedInterval.width, colSpan, percentBasedInterval.type);
        }

        private copyRestIntervals(iterator: ColumnIntervalWidthIterator, to: TableGrid) {
            while (!iterator.endOfIntervals()) {
                to.addInterval(iterator.interval);
                iterator.advance(iterator.interval);
            }
        }

        private processDependedIntervals(masterIterator: ColumnIntervalWidthIterator, slaveIterator: ColumnIntervalWidthIterator,
            mergedRowsIntervals: TableGrid) {
            // TODO some strange
            const deferredTableGrid: TableGrid = new this.tableGridConstructor();
            let autoSizeIntervalsCount: number = 0;
            let masterInterval: TableGridColumn = masterIterator.interval;
            do {
                const slaveInterval: TableGridColumn = slaveIterator.interval;
                if (slaveInterval.type == TableWidthUnitType.Auto || slaveInterval.type == TableWidthUnitType.Nil)
                    autoSizeIntervalsCount++;

                deferredTableGrid.addInterval(slaveInterval);
                slaveIterator.advance(slaveInterval);
                masterInterval = masterInterval.substract(slaveInterval);
            } while (masterInterval.colSpan > 0 && !slaveIterator.endOfIntervals() && slaveIterator.interval.colSpan <= masterInterval.colSpan);

            const calculateNotSetIntervals: boolean = masterInterval.type == TableWidthUnitType.ModelUnits && autoSizeIntervalsCount > 0;
            const newWidth: number = calculateNotSetIntervals ? masterInterval.width / autoSizeIntervalsCount : 0;

            for (let interval of deferredTableGrid.columns) {
                if (calculateNotSetIntervals && (interval.type == TableWidthUnitType.Auto || interval.type == TableWidthUnitType.Nil)) {
                    interval.type = TableWidthUnitType.ModelUnits;
                    interval.width = newWidth;
                }
                mergedRowsIntervals.addInterval(masterIterator.interval.colSpan > 1 ? interval : this.mergeIntervalsDifferentRows(masterIterator.interval, interval));
                masterIterator.advance(interval);
            }
        }
    }
}