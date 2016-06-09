module __aspxRichEdit {
    export enum TableCellBoundFlags {
        StartAndEndOnOtherColumns = 0,
        StartOnThisColumn = 1,
        EndOnThisColumn = 2
    }

    export class LayoutTableCellInfo {
        public cellGridIndex: number;
        public parentRow: LayoutTableRowInfo; // in what row top logic cell of this render cell. Need know that cell can be in some logic rows
        public bound: Rectangle;
        public avaliableContentWidth: number;

        public layoutRows: LayoutRowWithIndex[] = []; // sorted by "y". Here only rows what belongs this table. No rows what belongs low level tables(inner tables)
        public cellBoundFlags: TableCellBoundFlags;
        public internalTables: { [rowIndex: number]: LayoutTableColumnInfo }; // index - before rowIndex(in layoutRows) placed inner table. can be == layoutRows.length

        constructor(parentRow: LayoutTableRowInfo, bound: Rectangle, cellGridIndex: number, avaliableContentWidth: number) {
            this.parentRow = parentRow;
            this.bound = bound;
            this.layoutRows = [];
            this.cellGridIndex = cellGridIndex;
            this.cellBoundFlags = TableCellBoundFlags.StartAndEndOnOtherColumns;
            this.internalTables = {};
            this.avaliableContentWidth = avaliableContentWidth;
        }

        public getLastLayoutRow(): LayoutRowWithIndex {
            return this.layoutRows[this.layoutRows.length - 1];
        }

        // use ONLY for formatter
        public getEndPosition(): number {
            const internalTableAfterLastRow: LayoutTableColumnInfo = this.internalTables[this.layoutRows.length];
            return internalTableAfterLastRow ?
                internalTableAfterLastRow.getLastTableRow().getLastRowCell().getEndPosition() :
                this.getLastLayoutRow().getLastBoxEndPositionInRow();
        }
    }
}