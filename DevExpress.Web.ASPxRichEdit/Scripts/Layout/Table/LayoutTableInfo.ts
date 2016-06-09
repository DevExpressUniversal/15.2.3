module __aspxRichEdit {
    export class LayoutTableInfo {
        constructor(properties: LayoutTableProperties, grid: TableGrid) {
            this.properties = properties;
            this.grid = grid;
        }
        
        public properties: LayoutTableProperties;
        public grid: TableGrid;
    }

    export class LayoutTableColumnInfo extends Rectangle {
        public logicInfo: LayoutTableInfo;
        public horizontalBorders: LayoutTableBorder[] = []; // LayoutTableBorder relatively <Rectangle>this
        public verticalBorders: LayoutTableBorder[] = []; // LayoutTableBorder relatively <Rectangle>this
        public parentCell: LayoutTableCellInfo;
        public tableRows: LayoutTableRowInfo[] = []; // sorted by "y"

        constructor(parentCell: LayoutTableCellInfo, logicInfo: LayoutTableInfo, bound: Rectangle) {
            super();
            this.parentCell = parentCell;
            this.logicInfo = logicInfo;
            (<Rectangle>this).copyFrom(bound);
        }

        getLastTableRow(): LayoutTableRowInfo {
            return this.tableRows[this.tableRows.length - 1];
        }
    }
}