module __aspxRichEdit {
    export class TableCellUtils {
        static getCellIndexByColumnIndex(row: TableRow, startColumnIndex: number): number {
            let columnIndex = row.gridBefore;
            for(let i = 0, cell: TableCell; cell = row.cells[i]; i++) {
                if(startColumnIndex >= columnIndex && startColumnIndex < columnIndex + cell.columnSpan)
                    return i;
                columnIndex += cell.columnSpan;
            }
            return -1;
        }
        static getCellIndexByEndColumnIndex(row: TableRow, endColumnIndex: number): number {
            let cellIndexByColumnIndex = this.getCellIndexByColumnIndex(row, endColumnIndex);
            if(cellIndexByColumnIndex < 0)
                return -1;
            let cellByColumnIndex = row.cells[cellIndexByColumnIndex];
            if(this.getStartColumnIndex(cellByColumnIndex) + cellByColumnIndex.columnSpan - 1 <= endColumnIndex)
                return cellIndexByColumnIndex;
            if(cellIndexByColumnIndex != 0)
                return cellIndexByColumnIndex - 1;
            return -1;
        }
        static getStartColumnIndex(cell: TableCell): number {
            let columnIndex = cell.parentRow.gridBefore;
            let cellIndex = 0;
            let row = cell.parentRow;
            for(let i = 0, currentCell: TableCell; currentCell = row.cells[i]; i++) {
                if(currentCell === cell)
                    break;
                columnIndex += currentCell.columnSpan;
            }
            return columnIndex;
        }
        static getEndColumnIndex(cell: TableCell): number {
            return this.getStartColumnIndex(cell) + cell.columnSpan - 1;
        }
        static getColumnCount(table: Table) {
            let row = table.rows[0];
            let result = row.gridBefore + row.gridAfter;
            for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                result += cell.columnSpan;
            }
            return result;
        }
        static getColumnCountByCellsSeries(cells: TableCell[]): number {
            if(!cells.length)
                return 0;
            let result = cells[0].columnSpan;;
            let prevEndPosition = cells[0].endParagrapPosition.value;
            for(let i = 1, cell: TableCell; cell = cells[i]; i++) {
                if(cell.startParagraphPosition.value !== prevEndPosition)
                    return 0;
                result += cell.columnSpan;
                prevEndPosition = cell.endParagrapPosition.value;
            }
            return result;
        }
        static getCellIndicesByColumnsRange(row: TableRow, startColumnIndex: number, endColumnIndex: number): number[]{
            let indices: number[] = [];
            while(startColumnIndex <= endColumnIndex) {
                let cellIndex = this.getCellIndexByColumnIndex(row, startColumnIndex);
                let cell = row.cells[cellIndex];
                if(!cell)
                    return indices;
                indices.push(cellIndex);
                let cellStartColumnIndex = this.getStartColumnIndex(cell);
                startColumnIndex += startColumnIndex - cellStartColumnIndex + cell.columnSpan;
            }
            return indices;
        }
        static getColumnsRangeBySelectedCells(selectedCells: TableCell[][]): { startColumnIndex: number, endColumnIndex: number } {
            let startColumnIndex: number = this.getStartColumnIndex(selectedCells[0][0]),
                endColumnIndex: number = this.getStartColumnIndex(selectedCells[0][selectedCells[0].length - 1]);
            for(let i = 1, cells: TableCell[]; cells = selectedCells[i]; i++) {
                startColumnIndex = Math.max(startColumnIndex, this.getStartColumnIndex(selectedCells[i][0]));
                let lastCell = selectedCells[i][selectedCells[i].length - 1];
                endColumnIndex = Math.min(endColumnIndex, this.getStartColumnIndex(lastCell) + lastCell.columnSpan - 1);
            }
            return {
                startColumnIndex: startColumnIndex,
                endColumnIndex: endColumnIndex
            };
        }

        static getAbsoluteCellIndexInRow(row: TableRow, columnIndex: number) {
            if(!row.cells.length)
                throw new Error("Empty row");
            columnIndex -= row.gridBefore;
            let cellIndex = 0;
            let cellsCount = row.cells.length;
            while(columnIndex > 0 && cellIndex < cellsCount) {
                let currentCell = row.cells[cellIndex];
                columnIndex -= currentCell.columnSpan;
                if(columnIndex >= 0)
                    cellIndex++;
            }
            return cellIndex;
        }
        static getVerticalSpanCellPositions(restartCellPosition: TablePosition, patternCellStartColumnIndex: number): TablePosition[]{
            let positions: TablePosition[] = [];
            positions.push(restartCellPosition);
            if(restartCellPosition.cell.verticalMerging !== TableCellMergingState.Restart)
                return positions;
            let table = restartCellPosition.table;
            for(let rowIndex = restartCellPosition.rowIndex + 1, nextRow: TableRow; nextRow = table.rows[rowIndex]; rowIndex++) {
                let nextRowCellIndex = this.getCellIndexByColumnIndex(nextRow, patternCellStartColumnIndex);
                let nextCell = nextRow.cells[nextRowCellIndex];
                if(nextCell && nextCell.verticalMerging === TableCellMergingState.Continue)
                    positions.push(TablePosition.createAndInit(table, rowIndex, nextRowCellIndex));
                else
                    break;
            }
            return positions;
        }

        static areCellsSelectedInSeries(selectedCells: TableCell[][]): boolean {
            if(selectedCells.length === 0)
                return false;
            let prevRowEndPosition = selectedCells[0][0].parentRow.getEndPosition();
            for(let i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                if(i > 0 && horCells[0].parentRow.getStartPosition() !== prevRowEndPosition)
                    return false;
                let prevCellEndPosition = horCells[0].endParagrapPosition.value;
                for(let j = 0, cell: TableCell; cell = horCells[j]; j++) {
                    if(j > 0 && cell.startParagraphPosition.value !== prevCellEndPosition)
                        return false;
                    prevCellEndPosition = cell.endParagrapPosition.value;
                }
                prevRowEndPosition = horCells[0].parentRow.getEndPosition();
            }
            return true;
        }

        static isSelectedSquare(selectedCells: TableCell[][]): boolean {
            if(selectedCells.length === 0)
                return false;
            if(selectedCells.length === 1 && selectedCells[0].length === 1)
                return false;
            let table = selectedCells[0][0].parentRow.parentTable;
            let selectedCellsTable = this.getSelectedCellsTable(selectedCells);
            let startColumnIndexInFirstInterval = TableCellUtils.getStartColumnIndex(selectedCells[0][0]);
            let endColumnIndexInFirstInterval = TableCellUtils.getEndColumnIndex(selectedCells[0][selectedCells[0].length - 1]);
            let startRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[0][0].startParagraphPosition.value);
            let endRowIndex = startRowIndex + selectedCells.length - 1;

            let selectedRowsCountInFirstColumn = this.getSelectedRowsCount(table, startRowIndex, endRowIndex, startColumnIndexInFirstInterval, selectedCellsTable);
            if(selectedRowsCountInFirstColumn < 0)
                return false;
            for(let columnIndex = startColumnIndexInFirstInterval + 1; columnIndex <= endColumnIndexInFirstInterval; columnIndex++) {
                let currentSelectedRowsCount = this.getSelectedRowsCount(table, startRowIndex, endRowIndex, columnIndex, selectedCellsTable);
                if(currentSelectedRowsCount < 0 || selectedRowsCountInFirstColumn != currentSelectedRowsCount)
                    return false;
            }
            return true;
        }

        static getSelectedCellsTable(selectedCells: TableCell[][]): { [position: number]: boolean } {
            let selectedCellsTable: { [position: number]: boolean } = { };
            for(let i = 0, cells: TableCell[]; cells = selectedCells[i]; i++) {
                for(let j = 0, cell: TableCell; cell = cells[j]; j++) {
                    selectedCellsTable[cell.startParagraphPosition.value] = true;
                }
            }
            return selectedCellsTable;
        }

        static getSelectedRowsCount(table: Table, startRowIndex: number, endRowIndex: number, columnIndex: number, selectedCellsTable: { [position: number]: boolean }): number {
            let result = 0;
            for(let i = startRowIndex; i <= endRowIndex; i++) {
                let currentRow = table.rows[i];
                let currentCellIndex = TableCellUtils.getCellIndexByColumnIndex(currentRow, columnIndex);
                let currentCell: TableCell = currentRow.cells[currentCellIndex];
                if(!currentCell)
                    continue;
                if(currentCell.verticalMerging !== TableCellMergingState.Continue && !selectedCellsTable[currentCell.startParagraphPosition.value])
                    return -1;
                result += this.getSelectedRowsCountCore(columnIndex, TablePosition.createAndInit(table, i, currentCellIndex));
            }
            return result;
        }

        static getSameTableCells(firstCell: TableCell, lastCell: TableCell): { firstCell: TableCell; lastCell: TableCell } {
            let topLevelCell = firstCell,
                lowLevelCell = lastCell;
            let rightOrder = true;
            if(topLevelCell.parentRow.parentTable.nestedLevel < lowLevelCell.parentRow.parentTable.nestedLevel) {
                topLevelCell = lastCell;
                lowLevelCell = firstCell;
                rightOrder = false;
            }
            while(topLevelCell.parentRow.parentTable.nestedLevel > lowLevelCell.parentRow.parentTable.nestedLevel)
                topLevelCell = topLevelCell.parentRow.parentTable.parentCell;
            while(topLevelCell && lowLevelCell && topLevelCell.parentRow.parentTable !== lowLevelCell.parentRow.parentTable) {
                topLevelCell = topLevelCell.parentRow.parentTable.parentCell;
                lowLevelCell = lowLevelCell.parentRow.parentTable.parentCell;
            }
            if(!topLevelCell || !lowLevelCell)
                return null;
            return {
                firstCell: rightOrder ? topLevelCell : lowLevelCell,
                lastCell: rightOrder ? lowLevelCell : topLevelCell
            };
        }

        private static getSelectedRowsCountCore(columnIndex: number, position: TablePosition): number {
            switch(position.cell.verticalMerging) {
                case TableCellMergingState.Restart:
                    return TableCellUtils.getVerticalSpanCellPositions(position, columnIndex).length;
                case TableCellMergingState.Continue:
                    return 0;
                default:
                    return 1;
            }
        }
    }

    export class TableConditionalFormattingCalculator {
        static updateTable(control: IRichEditControl, table: Table) {
            let tableStyleColumnBandSize = new TablePropertiesMergerStyleColumnBandSize().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, control.model.defaultTableProperties);
            let tableStyleRowBandSize = new TablePropertiesMergerStyleRowBandSize().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, control.model.defaultTableProperties);
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let rowConditionalFormatting = this.getRowConditionalFormatting(table.lookTypes, tableStyleRowBandSize, table, rowIndex);;
                if(row.conditionalFormatting !== rowConditionalFormatting)
                    control.history.addAndRedo(new TableRowConditionalFormattingHistoryItem(control.modelManipulator, control.model.activeSubDocument, table, rowIndex, rowConditionalFormatting));
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    let cellConditionalFormatting = rowConditionalFormatting | this.getCellConditionalFormatting(table.lookTypes, tableStyleColumnBandSize, table, rowIndex, cellIndex);
                    if(cell.conditionalFormatting !== cellConditionalFormatting)
                        control.history.addAndRedo(new TableCellConditionalFormattingHistoryItem(control.modelManipulator, control.model.activeSubDocument, table, rowIndex, cellIndex, cellConditionalFormatting));
                }
            }
        }
        static updateTableWithoutHistory(model: DocumentModel, table: Table) {
            let tableStyleColumnBandSize = new TablePropertiesMergerStyleColumnBandSize().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties);
            let tableStyleRowBandSize = new TablePropertiesMergerStyleRowBandSize().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties);
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                row.conditionalFormatting = this.getRowConditionalFormatting(table.lookTypes, tableStyleRowBandSize, table, rowIndex);;
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++)
                    cell.conditionalFormatting = row.conditionalFormatting | this.getCellConditionalFormatting(table.lookTypes, tableStyleColumnBandSize, table, rowIndex, cellIndex);
            }
        }
        private static getRowConditionalFormatting(tableLook: TableLookTypes, tableStyleRowBandSize: number, table: Table, rowIndex: number): ConditionalTableStyleFormatting {
            let result = ConditionalTableStyleFormatting.WholeTable;
            if(tableLook & TableLookTypes.ApplyFirstRow) {
                if(rowIndex === 0)
                    result |= ConditionalTableStyleFormatting.FirstRow;
            }
            if(tableLook & TableLookTypes.ApplyLastRow) {
                if(rowIndex === table.rows.length - 1)
                    result |= ConditionalTableStyleFormatting.LastRow;
            }
            if(!(tableLook & TableLookTypes.DoNotApplyRowBanding) && !(result & ConditionalTableStyleFormatting.FirstRow || result & ConditionalTableStyleFormatting.LastRow)) {
                if(tableLook & TableLookTypes.ApplyFirstRow)
                    rowIndex--;
                if(Math.floor(rowIndex / tableStyleRowBandSize) % 2 == 0)
                    result |= ConditionalTableStyleFormatting.OddRowBanding;
                else
                    result |= ConditionalTableStyleFormatting.EvenRowBanding;
            }
            return result;
        }
        private static getCellConditionalFormatting(tableLook: TableLookTypes, tableStyleColumnBandSize: number, table: Table, rowIndex: number, cellIndex: number): ConditionalTableStyleFormatting {
            let result = ConditionalTableStyleFormatting.WholeTable;
            let row = table.rows[rowIndex];
            if(tableLook & TableLookTypes.ApplyFirstColumn) {
                if(cellIndex === 0)
                    result |= ConditionalTableStyleFormatting.FirstColumn;
            }
            if(tableLook & TableLookTypes.ApplyLastColumn) {
                if(cellIndex === row.cells.length - 1)
                    result |= ConditionalTableStyleFormatting.LastColumn;
            }
            if(tableLook & TableLookTypes.ApplyFirstRow && rowIndex === 0) {
                if(tableLook & TableLookTypes.ApplyFirstColumn && cellIndex === 0)
                    result |= ConditionalTableStyleFormatting.TopLeftCell;
                if(tableLook & TableLookTypes.ApplyLastColumn && cellIndex === row.cells.length - 1)
                    result |= ConditionalTableStyleFormatting.TopRightCell;
            }
            else if(tableLook & TableLookTypes.ApplyLastRow && rowIndex === table.rows.length - 1) {
                if(tableLook & TableLookTypes.ApplyFirstColumn && cellIndex === 0)
                    result |= ConditionalTableStyleFormatting.BottomLeftCell;
                if(tableLook & TableLookTypes.ApplyLastColumn && cellIndex === row.cells.length - 1)
                    result |= ConditionalTableStyleFormatting.BottomRightCell;
            }
            if(!(tableLook & TableLookTypes.DoNotApplyColumnBanding) && !(result & ConditionalTableStyleFormatting.FirstColumn || result & ConditionalTableStyleFormatting.LastColumn)) {
                if(tableLook & TableLookTypes.ApplyFirstColumn)
                    cellIndex--;
                if(Math.floor(cellIndex / tableStyleColumnBandSize) % 2 == 0)
                    result |= ConditionalTableStyleFormatting.OddColumnBanding;
                else
                    result |= ConditionalTableStyleFormatting.EvenColumnBanding;
            }
            return result;
        }
    }
}