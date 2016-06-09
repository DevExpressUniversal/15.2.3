 module __aspxRichEdit {
     export class Table {
         index: number;
         style: TableStyle;
         nestedLevel: number;
         parentCell: TableCell;
         rows: TableRow[] = [];
         preferredWidth: TableWidthUnit = TableWidthUnit.createDefault();
         properties: TableProperties;
         lookTypes: TableLookTypes = TableLookTypes.None;

         constructor(properties: TableProperties, style: TableStyle) {
             this.properties = properties;
             this.style = style;
         }

         destructor(positionManager: PositionManager) {
             for (var rowIndex: number = 0, tableRow: TableRow; tableRow = this.rows[rowIndex]; rowIndex++)
                 tableRow.destructor(positionManager); 
         }

         getInterval(): FixedInterval {
             return FixedInterval.fromPositions(this.getStartPosition(), this.getEndPosition());
         }

         getParentTable(): Table {
             const parentCell: TableCell = this.parentCell;
             return parentCell ? this.parentCell.parentRow.parentTable : null;
         }

         getTopLevelParent(): Table {
             let currTable: Table = this;
             while (currTable.parentCell)
                 currTable = currTable.getParentTable();
             return currTable;
         }

         getStartPosition(): number {
             return this.rows[0].getStartPosition();
         }

         getEndPosition(): number {
             return this.rows[this.rows.length - 1].getEndPosition();
         }

         getLastCell(): TableCell {
             let lastRow = this.rows[this.rows.length - 1];
             return lastRow.cells[lastRow.cells.length - 1];
         }

         getFirstCell(): TableCell {
             return this.rows[0].cells[0];
         }

         public static sort(tables: Table[]) {
             tables.sort((a: Table, b: Table) => {
                 const res: number = a.getStartPosition() - b.getStartPosition();
                 return res == 0 ? a.nestedLevel - b.nestedLevel : res; 
             });
             for (let tableIndex: number = 0, table: Table; table = tables[tableIndex]; tableIndex++)
                 table.index = tableIndex;
             // in WinForms realisation no tables sort! There used binaty paragraph binary tree
         }

         public static getTableCellByPosition(tables: Table[], position: number): TableCell {
             let table = Table.getTableByPosition(tables, position, true);
             if(!table)
                 return null;
             let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - position);
             let row = table.rows[rowIndex];
             let cellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - position);
             return row.cells[cellIndex];
         }

         public static getTableByPosition(tables: Table[], position: number, maxNestedLevel: boolean): Table {
             let tableIndex: number = Utils.normedBinaryIndexOf(tables, (t: Table) => t.getStartPosition() - position);
             if (tableIndex < 0)
                 return null;
             let table = tables[tableIndex];
             while(position >= table.getEndPosition()) {
                 if(table.nestedLevel === 0)
                     return null; 
                 table = table.getParentTable();
             }
             return Table.correctBoundTable(tables, table.index, position, maxNestedLevel ? (index: number) => ++index : (index: number) => --index);
         }

         public static correctBoundTable(tables: Table[], tableIndex: number, position: number, indexIterator: (index: number) => number): Table {
             let table: Table = tables[tableIndex];
             let tablePos: number = table.getStartPosition();
             tableIndex = indexIterator(tableIndex);
             for (let neighborTable: Table; neighborTable = tables[tableIndex]; tableIndex = indexIterator(tableIndex)) {
                 const neighborTablePos: number = neighborTable.getStartPosition();
                 if(tablePos != neighborTablePos || position >= neighborTable.getEndPosition())
                     break;
                 tablePos = neighborTablePos;
                 table = neighborTable;
             }
             return table;
         }

         public static getFirstCellPositionInVerticalMergingGroup(tablePosition: TablePosition): TablePosition {
             if (tablePosition.cell.verticalMerging != TableCellMergingState.Continue)
                 return tablePosition;

             const tblPos: TablePosition = tablePosition.clone().init();
             const cellColumnIndex: number = tblPos.row.getCellColumnIndex(tblPos.cellIndex);
             while (tblPos.moveToPrevRow()) {
                 let columnIndex: number = tblPos.row.gridBefore;
                 tblPos.setCell(0);
                 do {
                     if(cellColumnIndex <= columnIndex) {
                         if(tblPos.cell.verticalMerging != TableCellMergingState.Continue || tblPos.rowIndex == 0)
                             return tblPos;
                         else
                             break;
                     }
                     columnIndex += tblPos.cell.columnSpan;
                 } while(tblPos.moveToNextCell());
             }
             return null;
         }
     }

     export class TablePosition implements ICloneable<TablePosition>, ISupportCopyFrom<TablePosition> {
         table: Table;

         rowIndex: number;
         row: TableRow;

         cellIndex: number;
         cell: TableCell;

         constructor(table: Table, rowIndex: number, cellIndex: number) {
             this.table = table;
             this.rowIndex = rowIndex;
             this.cellIndex = cellIndex;
         }

         init(): TablePosition {
             this.row = this.table.rows[this.rowIndex];
             this.cell = this.row.cells[this.cellIndex];
             return this;
         }

         setRow(rowIndex: number): TablePosition {
             this.rowIndex = rowIndex;
             this.row = this.table.rows[this.rowIndex];
             return this;
         }

         setCell(cellIndex: number) {
             this.cellIndex = cellIndex;
             this.cell = this.row.cells[cellIndex];
         }

         static createAndInit(table: Table, rowIndex: number, cellIndex: number): TablePosition {
             let position = new TablePosition(table, rowIndex, cellIndex);
             position.init();
             return position;
         }

         static indexOfCell(positions: TablePosition[], cell: TableCell): number {
             for(let i = 0, pos: TablePosition; pos = positions[i]; i++) {
                 if(pos.cell === cell)
                     return i;
             }
             return -1;
         }

         // after invalid cell info
         moveToPrevRow(): boolean {
             if (!this.rowIndex)
                 return false;
             this.rowIndex--;
             this.row = this.table.rows[this.rowIndex];
             return true;
         }

         moveToNextRow(): boolean {
             if (this.rowIndex == this.table.rows.length - 1)
                 return false;
             this.rowIndex++;
             this.row = this.table.rows[this.rowIndex];
             return true;
         }

         moveToNextCell(): boolean {
             if (this.cellIndex == this.row.cells.length - 1)
                 return false;
             this.cellIndex++;
             this.cell = this.row.cells[this.cellIndex];
             return true;
         }

         copyFrom(obj: TablePosition) {
             this.table = obj.table;
             this.rowIndex = obj.rowIndex;
             this.row = obj.row;
             this.cell = obj.cell;
             this.cellIndex = obj.cellIndex;
         }

         clone(): TablePosition{
             return new TablePosition(this.table, this.rowIndex, this.cellIndex);
         }
     }
}