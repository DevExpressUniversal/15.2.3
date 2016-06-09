module __aspxRichEdit {
    export class ColumnIntervalWidthIterator {
        tableGrid: TableGrid;
        intervalIndex: number;
        interval: TableGridColumn;

        constructor(tableGrid: TableGrid) {
            this.tableGrid = tableGrid;
            this.intervalIndex = 0;
            this.interval = tableGrid.columns[0];
        }

        endOfIntervals(): boolean {
            return !this.interval;
        }

        moveNext() {
            this.intervalIndex++;
            this.interval = this.tableGrid.columns[this.intervalIndex];
        }

        advance(interval: TableGridColumn) {
            if (this.interval.colSpan == interval.colSpan)
                this.moveNext();
            else
                this.interval = this.interval.substract(interval);
        }
    }
}