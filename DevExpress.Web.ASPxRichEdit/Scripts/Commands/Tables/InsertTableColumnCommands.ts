module __aspxRichEdit {
    export class InsertTableColumnCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && this.getInsertedColumnCount() > 0;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let patternCell = this.getPatternCell();
            let columnIndex = this.getColumnIndex(patternCell);
            let table = patternCell.parentRow.parentTable;
            let columnCellIndices = this.getColumnCellIndices(table, columnIndex);
            let columnCount = this.getInsertedColumnCount();
            let newCells: TableCell[] = [];
            let subDocument = this.control.model.activeSubDocument;

            let availableWidth = this.getAvailableWidth(subDocument, table);

            for(let i = 0; i < columnCount; i++) {
                for(let rowIndex = columnCellIndices.length - 1; rowIndex >= 0; rowIndex--) {
                    newCells.push(this.splitTableCellCore(subDocument, table, rowIndex, columnCellIndices[rowIndex], patternCell));
                }
            }
            TablesManipulator.normalizeTableGrid(this.control, subDocument, table);
            TablesManipulator.normalizeCellColumnSpans(this.control, subDocument, table, false);
            TablesManipulator.normalizeTableCellWidth(this.control, subDocument, table);
            this.normalizeTableWidth(subDocument, table, availableWidth); // REMOVE after implementation of AutoSize mode in tables
            let newCellIntervals: FixedInterval[] = [];
            for(let i = 0, newCell: TableCell; newCell = newCells[i]; i++)
                newCellIntervals.push(newCell.getInterval());
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.endTransaction();
            return true;
        }

        private getAvailableWidth(subDocument: SubDocument, table: Table): number {
            let lp: LayoutPosition = subDocument.isMain() ?
                new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, table.getEndPosition(), DocumentLayoutDetailsLevel.Row)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false)) :
                new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, table.getEndPosition(), this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Row)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if(!lp)
                return -1;
            return table.parentCell && lp.row.tableCellInfo ? lp.row.tableCellInfo.avaliableContentWidth : LayoutColumn.findSectionColumnWithMinimumWidth(lp.pageArea.columns);
        }

        private normalizeTableWidth(subDocument: SubDocument, table: Table, availableWidth: number) {
            if(availableWidth < 0)
                return;
            let grid = TableGrid.getTableGrid(subDocument, table.index, availableWidth, new TwipsUnitConverter());
            grid.initTableCellGridInfo();
            let tableWidth = grid.columnsXPositions[grid.columnsXPositions.length - 1] - grid.columnsXPositions[0];
            if(tableWidth > availableWidth) {
                availableWidth = UnitConverter.pixelsToTwipsF(availableWidth);
                this.control.history.addAndRedo(new TablePreferredWidthHistoryItem(this.control.modelManipulator, subDocument, table.index, TableWidthUnit.create(availableWidth, TableWidthUnitType.ModelUnits)));
            }
        }

        private getInsertedColumnCount(): number {
            let selectedCells = this.control.selection.getSelectedCells();
            if(selectedCells.length === 0)
                return -1;
            let prevStartColumnIndex = TableCellUtils.getStartColumnIndex(selectedCells[0][0]);
            let columnCount = TableCellUtils.getColumnCountByCellsSeries(selectedCells[0]);
            if(columnCount === 0)
                return -1;
            for(let i = 1, cells: TableCell[]; cells = selectedCells[i]; i++) {
                if(i > 0 && prevStartColumnIndex !== TableCellUtils.getStartColumnIndex(cells[0]))
                    return -1;
                columnCount = Math.min(columnCount, TableCellUtils.getColumnCountByCellsSeries(cells));
            }
            return columnCount;
        }
        private sumColumnCount(cells: TableCell[]): number {
            let result = 0;
            for(let i = 0, cell: TableCell; cell = cells[i]; i++)
                result += cell.columnSpan;
            return result;
        }
        private getColumnCellIndices(table: Table, columnIndex: number): number[]{
            let indices: number[] = [];
            for(let i = 0, currentRow: TableRow; currentRow = table.rows[i]; i++) {
                indices.push(this.getCurrentCellIndex(columnIndex, currentRow));
            }
            return indices;
        }

        protected insertParagraphToTheLeft(subDocument: SubDocument, currentCell: TableCell) {
            let sourceRun = subDocument.getRunByPosition(currentCell.startParagraphPosition.value);
            let sourceParagraph = subDocument.getParagraphByPosition(currentCell.startParagraphPosition.value);
            this.control.history.addAndRedo(new InsertParagraphHistoryItem(this.control.modelManipulator, subDocument, currentCell.startParagraphPosition.value,
                sourceRun.maskedCharacterProperties, sourceRun.characterStyle,
                sourceParagraph.maskedParagraphProperties, sourceParagraph.paragraphStyle, sourceParagraph.numberingListIndex, sourceParagraph.listLevelIndex, sourceParagraph.tabs.clone()));
        }
        protected insertParagraphToTheRight(subDocument: SubDocument, currentCell: TableCell) {
            let sourceRun = subDocument.getRunByPosition(currentCell.endParagrapPosition.value - 1);
            let sourceParagraph = subDocument.getParagraphByPosition(currentCell.endParagrapPosition.value - 1);
            this.control.history.addAndRedo(new InsertParagraphHistoryItem(this.control.modelManipulator, subDocument, currentCell.endParagrapPosition.value - 1,
                sourceRun.maskedCharacterProperties, sourceRun.characterStyle,
                sourceParagraph.maskedParagraphProperties, sourceParagraph.paragraphStyle, sourceParagraph.numberingListIndex, sourceParagraph.listLevelIndex, sourceParagraph.tabs.clone()));
        }

        protected splitTableCellCore(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, patternCell: TableCell): TableCell { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getPatternCell(): TableCell { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getColumnIndex(patternCell: TableCell): number { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getCurrentCellIndex(relativeColumnIndex: number, currentRow: TableRow): number { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }

    export class InsertTableColumnToTheLeftCommand extends InsertTableColumnCommandBase {
        protected splitTableCellCore(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, patternCell: TableCell): TableCell {
            if(cellIndex < 0)
                cellIndex = table.rows[rowIndex].cells.length - 1;
            this.insertParagraphToTheLeft(subDocument, table.rows[rowIndex].cells[cellIndex]);
            this.control.history.addAndRedo(new SplitTableCellToTheLeftHistoryItem(this.control.modelManipulator, subDocument, table, rowIndex, cellIndex, true));
            this.control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, patternCell.preferredWidth.clone()));
            return table.rows[rowIndex].cells[cellIndex];
        }
        protected getPatternCell(): TableCell {
            return this.control.selection.getSelectedCells()[0][0];
        }
        protected getColumnIndex(patternCell: TableCell): number {
            return TableCellUtils.getStartColumnIndex(patternCell);
        }
        protected getCurrentCellIndex(relativeColumnIndex: number, currentRow: TableRow): number {
            return TableCellUtils.getCellIndexByColumnIndex(currentRow, relativeColumnIndex);
        }
    }

    export class InsertTableColumnToTheRightCommand extends InsertTableColumnCommandBase {
        protected splitTableCellCore(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, patternCell: TableCell): TableCell {
            if(cellIndex < 0) {
                this.insertParagraphToTheLeft(subDocument, table.rows[rowIndex].cells[0]);
                this.control.history.addAndRedo(new SplitTableCellToTheLeftHistoryItem(this.control.modelManipulator, subDocument, table, rowIndex, 0, true));
            }
            else {
                this.insertParagraphToTheRight(subDocument, table.rows[rowIndex].cells[cellIndex]);
                this.control.history.addAndRedo(new SplitTableCellToTheRightHistoryItem(this.control.modelManipulator, subDocument, table, rowIndex, cellIndex, true));
            }
            this.control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, Math.min(0, cellIndex), patternCell.preferredWidth.clone()));
            return table.rows[rowIndex].cells[cellIndex + 1];
        }
        protected getPatternCell(): TableCell {
            let selectedCells = this.control.selection.getSelectedCells();
            let lastSelectedRowCells = selectedCells[selectedCells.length - 1];
            return lastSelectedRowCells[lastSelectedRowCells.length - 1];
        }
        protected getColumnIndex(patternCell: TableCell): number {
            return TableCellUtils.getStartColumnIndex(patternCell) + patternCell.columnSpan - 1;
        }
        protected getCurrentCellIndex(relativeColumnIndex: number, currentRow: TableRow): number {
            return TableCellUtils.getCellIndexByEndColumnIndex(currentRow, relativeColumnIndex);
        }
    }
}