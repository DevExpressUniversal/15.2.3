module __aspxRichEdit {
    export class LayoutTableCellBackgroundInfo {
        bound: Rectangle;
        color: number;

        constructor(bound: Rectangle, color: number) {
            this.bound = bound;
            this.color = color;
        }
    }
    export class LayoutTableRowInfo {
        public parentTable: LayoutTableColumnInfo;
        public rowIndex: number; // in model
        public bound: Rectangle;
        public rowCells: LayoutTableCellInfo[]; // sorted by "x" 
        public backgroundInfos: LayoutTableCellBackgroundInfo[];

        constructor(parentTable: LayoutTableColumnInfo, bound: Rectangle, rowIndex: number) {
            this.parentTable = parentTable;
            this.bound = bound;
            this.rowCells = [];
            this.backgroundInfos = [];
            this.rowIndex = rowIndex;
        }

        public getLastRowCell(): LayoutTableCellInfo {
            return this.rowCells[this.rowCells.length - 1];
        }
    }

    
}