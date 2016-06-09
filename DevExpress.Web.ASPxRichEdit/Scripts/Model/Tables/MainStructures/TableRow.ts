module __aspxRichEdit {
    export class TableRow {
        parentTable: Table;
        cells: TableCell[] = [];
        widthBefore: TableWidthUnit = TableWidthUnit.createDefault(); // (can be in percent)
        widthAfter: TableWidthUnit = TableWidthUnit.createDefault(); // (can be in percent)
        gridBefore: number = 0;
        gridAfter: number = 0;
        properties: TableRowProperties;
        height: TableHeightUnit = TableHeightUnit.createDefault();
        tablePropertiesException: TableProperties = new TableProperties(); // dont used. Here properties connected table (when between tables no parMark)

        // layout (don't synchronize with server)
        conditionalFormatting: ConditionalTableStyleFormatting = ConditionalTableStyleFormatting.WholeTable; // for styles table.style.conditionalStyles

        constructor(parentTable: Table, properties: TableRowProperties) {
            this.parentTable = parentTable;
            this.properties = properties;
        }

        destructor(positionManager: PositionManager) {
            for(var cellIndex: number = 0, cell: TableCell; cell = this.cells[cellIndex]; cellIndex++ ) 
                cell.destructor(positionManager);
        }

        getStartPosition(): number {
            return this.cells[0].startParagraphPosition.value;
        }

        getEndPosition() {
            return this.cells[this.cells.length - 1].endParagrapPosition.value;
        }

        public getCellColumnIndex(cellIndex: number): number {
            const cells: TableCell[] = this.cells;
            let columnIndex: number = this.gridBefore;
            for (let cIndex: number = 0; cIndex < cellIndex; cIndex++)
                columnIndex += cells[cIndex].columnSpan;
            return columnIndex;
        }

        public getTotalCellsInRowConsiderGrid() {
            let cells = this.gridBefore;
            for(let i = 0, cell: TableCell; cell = this.cells[i]; i++)
                cells += cell.columnSpan;
            cells += this.gridAfter;
            return cells;
        }
    }
}