module __aspxRichEdit {
    export class InsertTableCellOperationBase {
        control: IRichEditControl;
        subDocument: SubDocument;

        constructor(control: IRichEditControl, subDocument: SubDocument) {
            this.control = control;
            this.subDocument = subDocument;
        }

        execute(table: Table, rowIndex: number, cellIndex: number, canNormalizeTable: boolean, canNormalizeVerticalMerging: boolean, canCopyProperties: boolean) {
            let row = table.rows[rowIndex];
            let cell = row.cells[cellIndex];
            let patternCellStartColumnIndex = TableCellUtils.getStartColumnIndex(cell);
            let restartCellPosition = Table.getFirstCellPositionInVerticalMergingGroup(TablePosition.createAndInit(table, rowIndex, cellIndex));
            let verticalSpanCellsPositions = TableCellUtils.getVerticalSpanCellPositions(restartCellPosition, patternCellStartColumnIndex);
            for(let i = verticalSpanCellsPositions.length - 1, pos: TablePosition; pos = verticalSpanCellsPositions[i]; i--) {
                if(canNormalizeVerticalMerging)
                    TablesManipulator.normalizeVerticalMerging(this.control, this.subDocument, table, pos.rowIndex, pos.cellIndex);
                this.insertTableCellCore(this.subDocument, pos, canCopyProperties);
            }
            if(canNormalizeTable) {
                this.normalizeTableGridAfter(table);
                TablesManipulator.normalizeCellColumnSpans(this.control, this.subDocument, table, true);
            }
        }

        normalizeTableGridAfter(table: Table) {
            let maxEndColumnIndex = 0;
            let endColumnIndices: number[] = [];
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let lastCell = row.cells[row.cells.length - 1];
                let currentEndColumnIndex = TableCellUtils.getEndColumnIndex(lastCell) + row.gridAfter;
                endColumnIndices.push(currentEndColumnIndex);
                maxEndColumnIndex = Math.max(maxEndColumnIndex, currentEndColumnIndex);
            }
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let currentEndColumnIndex = endColumnIndices[rowIndex];
                let delta = maxEndColumnIndex - currentEndColumnIndex;
                if(delta > 0)
                    this.control.history.addAndRedo(new TableRowGridAfterHistoryItem(this.control.modelManipulator, this.subDocument, table.index, rowIndex, row.gridAfter + delta));
            }
        }

        insertTableCellCore(subDocument: SubDocument, pos: TablePosition, copyProperties: boolean) {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class InsertTableCellToTheLeftOperation extends InsertTableCellOperationBase {
        insertTableCellCore(subDocument: SubDocument, pos: TablePosition, copyProperties: boolean) {
            TablesManipulator.insertParagraphToTheCellStartAndShiftContent(this.control, subDocument, pos.cell);
            this.control.history.addAndRedo(new SplitTableCellToTheLeftHistoryItem(this.control.modelManipulator, subDocument, pos.table, pos.rowIndex, pos.cellIndex, copyProperties));
            if(pos.cell.verticalMerging !== TableCellMergingState.None)
                this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, pos.table.index, pos.rowIndex, pos.cellIndex, pos.cell.verticalMerging));
        }
    }

    export class InsertTableCellToTheRightOperation extends InsertTableCellOperationBase {
        insertTableCellCore(subDocument: SubDocument, pos: TablePosition, copyProperties: boolean) {
            ModelManipulator.insertParagraph(this.control, subDocument, new FixedInterval(pos.cell.endParagrapPosition.value - 1, 0));
            this.control.history.addAndRedo(new SplitTableCellToTheRightHistoryItem(this.control.modelManipulator, subDocument, pos.table, pos.rowIndex, pos.cellIndex, copyProperties));
            if(pos.cell.verticalMerging !== TableCellMergingState.None)
                this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, pos.table.index, pos.rowIndex, pos.cellIndex + 1, pos.cell.verticalMerging));
        }
    }
}