module __aspxRichEdit {
    enum MarkType {
        Null,
        White,
        Black,
    }

    export class TableCompressHelper {
        private grid: TableGridColumn[];
        private rows: TableRow[];
        private spans: number[][];
        private marks: MarkType[][];

        constructor(grid: TableGridColumn[], table: Table) {
            this.grid = grid;
            this.rows = table.rows;
            this.initMatrices();
        }

        private initMatrices() {
            this.spans = [];
            for (let colIndex: number = 0; colIndex < this.grid.length; colIndex++) {
                const columnElements: number[] = [];
                for (let rowIndex: number = 0; rowIndex < this.rows.length; rowIndex++)
                    columnElements.push(MarkType.Null);
                this.spans.push(columnElements);
            }

            for (let rowIndex: number = 0; rowIndex < this.rows.length; rowIndex++) {
                let columnIndex: number = 0;

            }


            for (let rowIndex: number, row: TableRow; row = this.rows[rowIndex]; rowIndex++) {

            }
        }

        public work(): number[]{
            return [];
        }

        public compress(margins: number[]) {
            let totalDelta: number = 0;
            let availableToCompress: number = 0;
            for (let columnIndex: number = 0, gridColumn: TableGridColumn; gridColumn = this.grid[columnIndex]; columnIndex++) {
                const margin: number = margins[columnIndex];
                if (gridColumn.width < margin) {
                    totalDelta += margin - gridColumn.width;
                    gridColumn.width = margin;
                }
                else
                    availableToCompress += gridColumn.width - margin;
            }

            if (totalDelta > 0) {
                totalDelta = Math.min(totalDelta, availableToCompress);
                for (let columnIndex = 0, gridColumn: TableGridColumn; availableToCompress > 0 && (gridColumn = this.grid[columnIndex]); columnIndex++) {
                    const margin: number = margins[columnIndex];
                    if (gridColumn.width > margin) {
                        const diff: number = gridColumn.width - margin;
                        const delta: number = totalDelta * diff / availableToCompress;
                        availableToCompress -= diff;
                        totalDelta -= delta;
                        gridColumn.width -= delta;
                    }
                }
            }
        }
    }
}