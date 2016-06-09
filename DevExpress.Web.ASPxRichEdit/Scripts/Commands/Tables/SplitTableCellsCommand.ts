module __aspxRichEdit {
    export class SplitTableCellsCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables) && TableCellUtils.areCellsSelectedInSeries(this.control.selection.getSelectedCells());
        }

        executeCore(state: SimpleCommandState, parameters: SplitTableCellsDialogParameters): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let firstCell = selectedCells[0][0];
            if(parameters.isMergeBeforeSplit) {
                this.control.commandManager.getCommand(RichEditClientCommand.MergeTableCells).execute();
                selectedCells = this.filterRemovedCells(selectedCells);
            }
            this.splitTableCellsHorizontally(this.control.model.activeSubDocument, selectedCells, parameters);
            this.splitTableCellsVertically(this.control.model.activeSubDocument, selectedCells, parameters);
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(selectedCells[0][0].startParagraphPosition.value, 0), UpdateInputPositionProperties.Yes, false);
            TableConditionalFormattingCalculator.updateTable(this.control, firstCell.parentRow.parentTable);
            this.control.history.endTransaction();
            return true;
        }
        
        splitTableCellsHorizontally(subDocument: SubDocument, selectedCells: TableCell[][], parameters: SplitTableCellsDialogParameters) {
            let startCell = selectedCells[0][0];
            let table = startCell.parentRow.parentTable;
            if(parameters.isMergeBeforeSplit) {
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - startCell.startParagraphPosition.value);
                let cellIndex = Utils.normedBinaryIndexOf(table.rows[rowIndex].cells, c => c.startParagraphPosition.value - startCell.startParagraphPosition.value);
                this.splitTableCellsHorizontallyCore(subDocument, TablePosition.createAndInit(table, rowIndex, cellIndex), parameters.columnCount);
                return;
            }

            let topRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - startCell.startParagraphPosition.value);
            for(var i = selectedCells.length - 1; i >= 0; i--) {
                let rowIndex = i + topRowIndex;
                let row = table.rows[rowIndex];
                let startCellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - selectedCells[i][0].startParagraphPosition.value);
                for(let j = selectedCells[i].length - 1; j >= 0; j--) {
                    let cellIndex = startCellIndex + j;
                    let cell = row.cells[cellIndex];
                    if(cell.verticalMerging === TableCellMergingState.Continue)
                        continue;
                    this.splitTableCellsHorizontallyCore(subDocument, TablePosition.createAndInit(table, rowIndex, cellIndex), parameters.columnCount);
                }
            }
        }
        splitTableCellsVertically(subDocument: SubDocument, selectedCells: TableCell[][], parameters: SplitTableCellsDialogParameters) {
            if(parameters.rowCount === 1)
                return;
            let columnCount = this.getColumnsCountForSplitVertically(selectedCells[0], parameters);
            let startCell = selectedCells[0][0];
            let topRowIndex = Utils.normedBinaryIndexOf(startCell.parentRow.parentTable.rows, r => r.getStartPosition() - startCell.startParagraphPosition.value);
            let startCellIndex = Utils.normedBinaryIndexOf(startCell.parentRow.cells, c => c.startParagraphPosition.value - startCell.startParagraphPosition.value);
            this.splitTableCellsVerticallyCore(subDocument, TablePosition.createAndInit(startCell.parentRow.parentTable, topRowIndex, startCellIndex), parameters.rowCount, columnCount);
        }
        splitTableCellsVerticallyCore(subDocument: SubDocument, position: TablePosition, rowsCount: number, columnsCount: number) {
            if(position.cell.verticalMerging === TableCellMergingState.Restart) {
                this.splitMergedCellsVertically(subDocument, position, columnsCount, rowsCount);
                return;
            }
            let table = position.table;
            this.insertRows(subDocument, position, rowsCount);
            let startIndex = position.cellIndex;
            let endIndex = position.cellIndex + columnsCount - 1;
            for(let i = 0, cell: TableCell; cell = position.row.cells[i]; i++) {
                if(i < startIndex || i > endIndex) {
                    let columnIndex = TableCellUtils.getStartColumnIndex(cell);
                    let mergeCellPosition = TableCellUtils.getVerticalSpanCellPositions(TablePosition.createAndInit(table, position.rowIndex, i), columnIndex)[0];
                    let restartRowIndex = mergeCellPosition.rowIndex;
                    let continionRowIndex = rowsCount + position.rowIndex - 2;
                    for(let i = continionRowIndex; i >= restartRowIndex; i--) {
                        let row = table.rows[i];
                        let mergeCellIndex = TableCellUtils.getCellIndexByColumnIndex(row, columnIndex);
                        TablesManipulator.mergeTwoTableCellsVertically(this.control, subDocument, TablePosition.createAndInit(table, i, mergeCellIndex));
                    }
                    TablesManipulator.normalizeRows(this.control, subDocument, table);
                }
            }
        }
        insertRows(subDocument: SubDocument, position: TablePosition, rowsCount: number) {
            let rowHeight = position.row.height;
            this.control.history.addAndRedo(new TableRowHeightHistoryItem(this.control.modelManipulator, subDocument, position.table.index, position.rowIndex, TableHeightUnit.create(rowHeight.value / rowsCount, rowHeight.type)));
            for(let i = 1; i < rowsCount; i++) {
                TablesManipulator.insertRowBelow(this.control, subDocument, position.table, position.rowIndex);
            }
        }
        splitMergedCellsVertically(subDocument: SubDocument, position: TablePosition, columnsCount: number, rowsCount: number) {
            let endIndex = position.cellIndex + columnsCount - 1;
            for(let cellIndex = position.cellIndex; cellIndex <= endIndex; cellIndex++) {
                this.splitMergedCellsVerticallyCore(subDocument, TablePosition.createAndInit(position.table, position.rowIndex, cellIndex), rowsCount);
            }
        }
        splitMergedCellsVerticallyCore(subDocument: SubDocument, position: TablePosition, rowsCount: number) {
            let columnIndex = TableCellUtils.getStartColumnIndex(position.cell);
            let mergedCellsPositions = TableCellUtils.getVerticalSpanCellPositions(position, columnIndex);
            if(mergedCellsPositions.length === rowsCount) {
                for(let i = 0, mergedCellsPosition: TablePosition; mergedCellsPosition = mergedCellsPositions[i]; i++) {
                    this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, position.table.index, mergedCellsPosition.rowIndex, mergedCellsPosition.cellIndex, TableCellMergingState.None));
                }
                return;
            }
            let totalRowsCount = mergedCellsPositions.length / rowsCount;
            for(let i = 0, mergedCellsPosition: TablePosition; mergedCellsPosition = mergedCellsPositions[i]; i++) {
                if(i % totalRowsCount == 0)
                    this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, position.table.index, mergedCellsPosition.rowIndex, mergedCellsPosition.cellIndex, TableCellMergingState.Restart));
            }
        }

        splitTableCellsHorizontallyCore(subDocument: SubDocument, position: TablePosition, columnsCount: number) {
            let columnIndex = TableCellUtils.getStartColumnIndex(position.cell);
            let verticalSpanPositions = TableCellUtils.getVerticalSpanCellPositions(position, columnIndex);
            let spanDelta = columnsCount - position.cell.columnSpan;
            let oldPatternCellWidth = position.cell.preferredWidth;
            if(oldPatternCellWidth.type !== TableWidthUnitType.Nil && oldPatternCellWidth.type !== TableWidthUnitType.Auto) {
                for(let i = verticalSpanPositions.length - 1; i >= 0; i--) {
                    let cellPosition = verticalSpanPositions[i];
                    let cellWidth = cellPosition.cell.preferredWidth;
                    if(cellWidth.type !== TableWidthUnitType.Nil && cellWidth.type !== TableWidthUnitType.Auto)
                        this.control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(this.control.modelManipulator, subDocument, cellPosition.table.index, cellPosition.rowIndex, cellPosition.cellIndex, TableWidthUnit.create(cellWidth.value / columnsCount, cellWidth.type)));
                    if(cellPosition.cell.columnSpan > 1)
                        this.control.history.addAndRedo(new TableCellColumnSpanHistoryItem(this.control.modelManipulator, subDocument, cellPosition.table.index, cellPosition.rowIndex, cellPosition.cellIndex, Math.max(1, cellPosition.cell.columnSpan - (columnsCount - 1))));
                }
            }
            for(let i = 1; i < columnsCount; i++)
                TablesManipulator.insertCellToTheRight(this.control, subDocument, position.table, position.rowIndex, position.cellIndex, false, false);
            if(spanDelta > 0)
                this.normalizeColumnSpansAfterSplitHorizontally(subDocument, position, columnIndex, spanDelta);
            if(oldPatternCellWidth.type !== TableWidthUnitType.Auto && oldPatternCellWidth.type !== TableWidthUnitType.Nil)
                this.normalizeTableCellsWidth(subDocument, oldPatternCellWidth, position, columnsCount);
        }

        normalizeColumnSpansAfterSplitHorizontally(subDocument: SubDocument, position: TablePosition, columnIndex: number, newColumnsCount: number) {
            for(let rowIndex = 0, row: TableRow; row = position.table.rows[rowIndex]; rowIndex++) {
                if(rowIndex === position.rowIndex)
                    continue;
                let cellIndex = TableCellUtils.getCellIndexByColumnIndex(row, columnIndex);
                let cell = row.cells[cellIndex];
                if(!cell) {
                    if(row.gridBefore >= columnIndex)
                        this.control.history.addAndRedo(new TableRowGridBeforeHistoryItem(this.control.modelManipulator, subDocument, position.table.index, rowIndex, row.gridBefore + newColumnsCount));
                    else
                        this.control.history.addAndRedo(new TableRowGridAfterHistoryItem(this.control.modelManipulator, subDocument, position.table.index, rowIndex, row.gridAfter + newColumnsCount));
                }
                else
                    this.control.history.addAndRedo(new TableCellColumnSpanHistoryItem(this.control.modelManipulator, subDocument, position.table.index, rowIndex, cellIndex, cell.columnSpan + newColumnsCount));
            }
        }

        private normalizeTableCellsWidth(subDocument: SubDocument, width: TableWidthUnit, position: TablePosition, columnsCount: number) {
            //let widthAllNewCells = 0; // TODO
            //let startIndex = position.cellIndex;
            //let endIndex = startIndex + columnsCount - 1;
            //for(let cellIndex = startIndex; cellIndex <= endIndex; cellIndex++) {
            //    let cell = position.row.cells[cellIndex];
            //    widthAllNewCells += cell.preferredWidth.value;
            //}
            //let delta = width.value - widthAllNewCells;
            //endIndex = startIndex + delta; // TODO: looks strange, copied from WF
            //for(let cellIndex = startIndex; cellIndex < endIndex; cellIndex++) {
            //    let cell = position.row.cells[cellIndex];
            //    let cellWidth = cell.preferredWidth;
            //    this.control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(this.control.modelManipulator, subDocument, position.table.index, position.rowIndex, cellIndex, TableWidthUnit.create(cellWidth.value + 1, cellWidth.type)));
            //}
        }

        private getColumnsCountForSplitVertically(selectedCells: TableCell[], parameters: SplitTableCellsDialogParameters): number {
            if(parameters.isMergeBeforeSplit)
                return parameters.columnCount;
            return (selectedCells.length + 1) * parameters.columnCount;
        }

        private filterRemovedCells(selectedCells: TableCell[][]): TableCell[][] {
            let result: TableCell[][] = [];
            let table = selectedCells[0][0].parentRow.parentTable;
            for(let i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                let row = horCells[0].parentRow;
                if(Utils.indexOf(table.rows, row) < 0)
                    continue;
                result.push([horCells[0]]);
            }
            return result;
        }
    }
}