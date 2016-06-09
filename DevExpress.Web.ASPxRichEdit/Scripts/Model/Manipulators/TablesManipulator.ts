module __aspxRichEdit {
    export class TablesManipulator {
        manipulator: ModelManipulator;
        cellProperties: TableCellPropertiesManipulator;
        tableProperties: TablePropertiesManipulator;
        rowProperties: TableRowPropertiesManipulator;

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
            this.cellProperties = new TableCellPropertiesManipulator(manipulator);
            this.tableProperties = new TablePropertiesManipulator(manipulator);
            this.rowProperties = new TableRowPropertiesManipulator(manipulator);
        }

        /* Create/Remove Table */
        createTable(subDocument: SubDocument, firstParagraphIndex: number, rowCount: number, cellCount: number): Table {
            let paragraph = subDocument.paragraphs[firstParagraphIndex];
            let parentCell = Table.getTableCellByPosition(subDocument.tables, paragraph.startLogPosition.value);
            let newTable = new Table(new TableProperties(), subDocument.documentModel.getTableStyleByName(TableStyle.DEFAULT_STYLENAME));
            this.createTableStructure(subDocument, firstParagraphIndex, newTable, rowCount, cellCount);
            this.insertTableCore(newTable, subDocument.tables, subDocument.tablesByLevels, parentCell);
            this.manipulator.dispatcher.notifyTableCreated(subDocument, newTable);
            return newTable;
        }
        removeTable(subDocument: SubDocument, table: Table) {
            const startPosition = table.getStartPosition();
            const endPosition = table.getEndPosition();
            table.destructor(subDocument.positionManager);
            this.removeTableCore(table, subDocument.tables, subDocument.tablesByLevels);
            this.manipulator.dispatcher.notifyTableRemoved(subDocument, startPosition, endPosition, table.nestedLevel);
        }
        restoreRemovedTable(subDocument: SubDocument, table: Table, cellsRanges: FixedInterval[][]) {
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    let cellRange = cellsRanges[rowIndex][cellIndex];
                    cell.startParagraphPosition = subDocument.positionManager.registerPosition(cellRange.start);
                    cell.endParagrapPosition = subDocument.positionManager.registerPosition(cellRange.end());
                }
            }
            table.parentCell = Table.getTableCellByPosition(subDocument.tables, table.getStartPosition());
            this.insertTableCore(table, subDocument.tables, subDocument.tablesByLevels, table.parentCell);
            this.manipulator.dispatcher.notifyTableCreated(subDocument, table);
        }
        pasteTable(subDocument: SubDocument, patternTable: Table, position: number) {
            let patternCell = Table.getTableCellByPosition(subDocument.tables, position);
            let newTable = new Table(patternTable.properties, this.manipulator.model.stylesManager.addTableStyle(patternTable.style));
            newTable.preferredWidth = patternTable.preferredWidth.clone();
            newTable.lookTypes = patternTable.lookTypes;
            for(let rowIndex = 0, patternRow: TableRow; patternRow = patternTable.rows[rowIndex]; rowIndex++) {
                let newRow = new TableRow(newTable, subDocument.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(patternRow.properties.clone()));
                newRow.gridAfter = patternRow.gridAfter;
                newRow.gridBefore = patternRow.gridBefore;
                if(patternRow.tablePropertiesException)
                    newRow.tablePropertiesException = patternRow.tablePropertiesException.clone();
                newRow.widthAfter = patternRow.widthAfter.clone();
                newRow.widthBefore = patternRow.widthBefore.clone();
                newRow.height = patternRow.height.clone();
                if(patternRow.tablePropertiesException)
                    newRow.tablePropertiesException = patternRow.tablePropertiesException.clone();
                for(let cellIndex = 0, patternCell: TableCell; patternCell = patternRow.cells[cellIndex]; cellIndex++) {
                    let cellLength = patternCell.endParagrapPosition.value - patternCell.startParagraphPosition.value;
                    let newCell = new TableCell(newRow, subDocument.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(patternCell.properties.clone()));
                    newCell.startParagraphPosition = subDocument.positionManager.registerPosition(position);
                    position += cellLength;
                    newCell.endParagrapPosition = subDocument.positionManager.registerPosition(position);
                    newCell.columnSpan = patternCell.columnSpan;
                    newCell.conditionalFormatting = patternCell.conditionalFormatting;
                    newCell.verticalMerging = patternCell.verticalMerging;
                    newCell.preferredWidth = patternCell.preferredWidth.clone();
                    newRow.cells.push(newCell);
                }
                newTable.rows.push(newRow);
            }
            this.insertTableCore(newTable, subDocument.tables, subDocument.tablesByLevels, patternCell);
            this.manipulator.dispatcher.notifyTableCreated(subDocument, newTable);
        }

        insertRow(subDocument: SubDocument, tableIndex: number, patternRow: TableRow, targetRowIndex: number, cellIntervals: FixedInterval[]) {
            var table = subDocument.tables[tableIndex];
            var row = new TableRow(table, patternRow.properties);
            if(cellIntervals.length !== patternRow.cells.length)
                throw new Error("cellIntervals.length should be equal to patternRow.cells.length");
            row.gridAfter = patternRow.gridAfter;
            row.gridBefore = patternRow.gridBefore;
            row.height = patternRow.height.clone();
            row.properties = patternRow.properties;
            row.tablePropertiesException = patternRow.tablePropertiesException;
            row.widthAfter = patternRow.widthAfter.clone();
            row.widthBefore = patternRow.widthBefore.clone();
            for(let i = 0, interval: FixedInterval; interval = cellIntervals[i]; i++) {
                let patternCell = patternRow.cells[i];
                let cell = new TableCell(row, patternCell.properties);
                cell.startParagraphPosition = subDocument.positionManager.registerPosition(interval.start);
                cell.endParagrapPosition = subDocument.positionManager.registerPosition(interval.end());
                cell.columnSpan = patternCell.columnSpan;
                cell.conditionalFormatting = patternCell.conditionalFormatting;
                cell.preferredWidth = patternCell.preferredWidth.clone();
                cell.verticalMerging = patternCell.verticalMerging;
                cell.style = patternCell.style;
                row.cells.push(cell);
            }
            table.rows.splice(targetRowIndex, 0, row);
            let nextRow = table.rows[targetRowIndex + 1];
            if(nextRow) {
                subDocument.positionManager.unregisterPosition(nextRow.cells[0].startParagraphPosition);
                nextRow.cells[0].startParagraphPosition = subDocument.positionManager.registerPosition(row.getEndPosition());
            }
            this.manipulator.dispatcher.notifyTableRowInserted(subDocument, table, targetRowIndex);
        }
        removeRow(subDocument: SubDocument, tableIndex: number, rowIndex: number) {
            let table = subDocument.tables[tableIndex];
            let row = table.rows[rowIndex];
            row.destructor(subDocument.positionManager);
            var nextRow = table.rows[rowIndex + 1];
            if(nextRow) {
                var nextRowFirstCell = nextRow.cells[0];
                subDocument.positionManager.unregisterPosition(nextRowFirstCell.startParagraphPosition);
                nextRowFirstCell.startParagraphPosition = subDocument.positionManager.registerPosition(row.getStartPosition());
            }
            table.rows.splice(rowIndex, 1);
            this.manipulator.dispatcher.notifyTableRowRemoved(subDocument, table, rowIndex, true);
        }
        
        /* Remove Cell */
        removeCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            let row = table.rows[rowIndex];
            let cell = row.cells[cellIndex];
            let nextCell = row.cells[cellIndex + 1];
            if(!nextCell && table.rows.length > rowIndex + 1)
                nextCell = table.rows[rowIndex + 1].cells[0];
            cell.destructor(subDocument.positionManager);
            if(nextCell) {
                subDocument.positionManager.unregisterPosition(nextCell.startParagraphPosition);
                nextCell.startParagraphPosition = subDocument.positionManager.registerPosition(cell.startParagraphPosition.value);
            }
            row.cells.splice(cellIndex, 1);
            this.manipulator.dispatcher.notifyTableCellRemoved(subDocument, table, rowIndex, cellIndex);
        }
        insertCell(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, patternCell: TableCell, length: number) {
            let row = table.rows[rowIndex];
            var cell = new TableCell(row, patternCell.properties);
            cell.columnSpan = patternCell.columnSpan;
            cell.conditionalFormatting = patternCell.conditionalFormatting;
            cell.preferredWidth = patternCell.preferredWidth.clone();
            cell.style = patternCell.style;
            cell.verticalMerging = patternCell.verticalMerging;

            let startPosition = 0;
            if(cellIndex > 0)
                startPosition = row.cells[cellIndex - 1].endParagrapPosition.value;
            else if(rowIndex > 0)
                startPosition = table.rows[rowIndex - 1].getEndPosition();
            else
                startPosition = table.getStartPosition();
            cell.startParagraphPosition = subDocument.positionManager.registerPosition(startPosition);
            cell.endParagrapPosition = subDocument.positionManager.registerPosition(startPosition + length);
            row.cells.splice(cellIndex, 0, cell);
            let nextCell = row.cells[cellIndex + 1];
            if(!nextCell && table.rows.length > rowIndex + 1)
                nextCell = table.rows[rowIndex + 1].cells[0];
            if(nextCell) {
                subDocument.positionManager.unregisterPosition(nextCell.startParagraphPosition);
                nextCell.startParagraphPosition = subDocument.positionManager.registerPosition(cell.endParagrapPosition.value);
            }
            this.manipulator.dispatcher.notifyTableCellInserted(subDocument, table, rowIndex, cellIndex);
        }

        static insertParagraphToTheCellStartAndShiftContent(control: IRichEditControl, subDocument: SubDocument, cell: TableCell) {
            let table = cell.parentRow.parentTable;
            ModelManipulator.insertParagraph(control, subDocument, new FixedInterval(cell.startParagraphPosition.value, 0));
            let nextTable = subDocument.tables[table.index + 1];
            let shiftingTables: Table[] = [];
            while(nextTable && nextTable.nestedLevel > table.nestedLevel) {
                let nextTableStartPosition = nextTable.getStartPosition();
                if(nextTableStartPosition === cell.startParagraphPosition.value)
                    shiftingTables.push(nextTable);
                else if(nextTableStartPosition > cell.startParagraphPosition.value)
                    break;
                nextTable = subDocument.tables[nextTable.index + 1];
            }
            for(let i = shiftingTables.length - 1, shiftingTable: Table; shiftingTable = shiftingTables[i]; i--) {
                control.history.addAndRedo(new ShiftTableStartPositionToTheRightHistoryItem(control.modelManipulator, subDocument, shiftingTable));
            }
        }

        changeTableStartPosition(subDocument: SubDocument, table: Table, newPosition: number) {
            let cell = table.rows[0].cells[0];
            var oldPosition = cell.startParagraphPosition.value;
            subDocument.positionManager.unregisterPosition(cell.startParagraphPosition);
            cell.startParagraphPosition = subDocument.positionManager.registerPosition(newPosition);
            this.manipulator.dispatcher.notifyTableStartPositionShifted(subDocument, table, oldPosition, newPosition);
        }
        shiftTableStartPositionToTheRight(subDocument: SubDocument, table: Table) {
            this.changeTableStartPosition(subDocument, table, table.rows[0].cells[0].startParagraphPosition.value + 1);
        }
        restoreShiftedTableStartPositionToTheRight(subDocument: SubDocument, table: Table) {
            this.changeTableStartPosition(subDocument, table, table.rows[0].cells[0].startParagraphPosition.value - 1);
        }

        splitTableCellHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean, copyProperties: boolean) {
            let row = table.rows[rowIndex];
            let cell = row.cells[cellIndex];
            if(rightDirection)
                this.splitTableCellToTheRightCore(subDocument, row, cellIndex, copyProperties);
            else
                this.splitTableCellToTheLeftCore(subDocument, row, cellIndex, copyProperties);
            this.manipulator.dispatcher.notifyTableCellSplittedHorizontally(subDocument, table, rowIndex, rightDirection ? cellIndex : (cellIndex + 1), rightDirection);
        }

        restoreSplittedCellHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            let row = table.rows[rowIndex];
            let targetCell = row.cells[cellIndex];
            let removingCell = rightDirection ? row.cells[cellIndex + 1] : row.cells[cellIndex - 1];
            if(rightDirection) {
                subDocument.positionManager.unregisterPosition(targetCell.endParagrapPosition);
                targetCell.endParagrapPosition = subDocument.positionManager.registerPosition(removingCell.endParagrapPosition.value);
                removingCell.destructor(subDocument.positionManager);
                row.cells.splice(cellIndex + 1, 1);
            }
            else {
                subDocument.positionManager.unregisterPosition(targetCell.startParagraphPosition);
                targetCell.startParagraphPosition = subDocument.positionManager.registerPosition(removingCell.startParagraphPosition.value);
                removingCell.destructor(subDocument.positionManager);
                row.cells.splice(cellIndex - 1, 1);
            }
            this.manipulator.dispatcher.notifyTableCellMergedHorizontally(subDocument, table, rowIndex, rightDirection ? cellIndex : (cellIndex - 1), rightDirection);
        }


        private splitTableCellToTheLeftCore(subDocument: SubDocument, row: TableRow, splittingCellIndex: number, copyProperties: boolean) {
            if(splittingCellIndex < 0)
                throw new Error("splittingCellIndex should be > 0");
            let splittingCell = row.cells[splittingCellIndex];
            let movingParagraph = subDocument.getParagraphByPosition(splittingCell.startParagraphPosition.value);
            let newTableCell = new TableCell(row, copyProperties ? splittingCell.properties : subDocument.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(new TableCellProperties()));
            newTableCell.preferredWidth = splittingCell.preferredWidth.clone();
            row.cells.splice(splittingCellIndex, 0, newTableCell);
            newTableCell.startParagraphPosition = subDocument.positionManager.registerPosition(movingParagraph.startLogPosition.value);
            newTableCell.endParagrapPosition = subDocument.positionManager.registerPosition(movingParagraph.getEndPosition());
            subDocument.positionManager.unregisterPosition(splittingCell.startParagraphPosition);
            splittingCell.startParagraphPosition = subDocument.positionManager.registerPosition(movingParagraph.getEndPosition());
        }
        private splitTableCellToTheRightCore(subDocument: SubDocument, row: TableRow, splittingCellIndex: number, copyProperties: boolean) {
            if(splittingCellIndex < 0)
                throw new Error("splittingCellIndex should be > 0");
            let splittingCell = row.cells[splittingCellIndex];
            let movingParagraph = subDocument.getParagraphByPosition(splittingCell.endParagrapPosition.value - 1);
            let newTableCell = new TableCell(row, copyProperties ? splittingCell.properties : subDocument.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(new TableCellProperties()));
            newTableCell.preferredWidth = splittingCell.preferredWidth.clone();
            row.cells.splice(splittingCellIndex + 1, 0, newTableCell);
            newTableCell.startParagraphPosition = subDocument.positionManager.registerPosition(movingParagraph.startLogPosition.value);
            newTableCell.endParagrapPosition = subDocument.positionManager.registerPosition(movingParagraph.getEndPosition());

            subDocument.positionManager.unregisterPosition(splittingCell.endParagrapPosition);
            splittingCell.endParagrapPosition = subDocument.positionManager.registerPosition(movingParagraph.startLogPosition.value);
        }

        setTableStyle(subDocument: SubDocument, tableIndex: number, style: TableStyle) {
            let table = subDocument.tables[tableIndex];
            table.style = style;
            this.manipulator.dispatcher.notifyTableStyleChanged(subDocument, table, style);
        }

        static removeTableWithContent(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            this.removeNestedTablesByParentTable(control, subDocument, table);
            control.history.addAndRedo(new RemoveTableHistoryItem(control.modelManipulator, subDocument, table));
            control.history.addAndRedo(new RemoveIntervalHistoryItem(control.modelManipulator, subDocument, FixedInterval.fromPositions(table.getStartPosition(), table.getEndPosition()), false));
        }
        static removeTableCellWithContent(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            let cell = table.rows[rowIndex].cells[cellIndex];
            this.removeNestedTablesByParentCell(control, subDocument, cell);
            control.history.addAndRedo(new RemoveTableCellHistoryItem(control.modelManipulator, subDocument, table, rowIndex, cellIndex));
            control.history.addAndRedo(new RemoveIntervalHistoryItem(control.modelManipulator, subDocument, cell.getInterval(), false));
        }
        static removeTableRowWithContent(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number) {
            let row = table.rows[rowIndex];
            this.removeNestedTables(control, subDocument, row);
            this.updateVerticalMergingState(control, subDocument, table, rowIndex);
            control.history.addAndRedo(new RemoveTableRowHistoryItem(control.modelManipulator, subDocument, table, rowIndex));
            control.history.addAndRedo(new RemoveIntervalHistoryItem(control.modelManipulator, subDocument, FixedInterval.fromPositions(row.getStartPosition(), row.getEndPosition()), false));
        }

        private static updateVerticalMergingState(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number) {
            let row = table.rows[rowIndex];
            for(let cellIndex = row.cells.length - 1, cell: TableCell; cell = row.cells[cellIndex]; cellIndex--) {
                if(cell.verticalMerging !== TableCellMergingState.None) {
                    let columnIndex = TableCellUtils.getStartColumnIndex(cell);
                    let nextRow = table.rows[rowIndex + 1];
                    let nextRowCellIndex = nextRow ? TableCellUtils.getCellIndexByColumnIndex(nextRow, columnIndex) : -1;
                    let nextRowCell = nextRow ? nextRow.cells[nextRowCellIndex] : null;
                    if(cell.verticalMerging == TableCellMergingState.Restart) {
                        if(nextRowCell) {
                            let afterNextRow = table.rows[rowIndex + 2];
                            let afterNextRowCell = afterNextRow ? afterNextRow.cells[TableCellUtils.getCellIndexByEndColumnIndex(afterNextRow, columnIndex)] : null;
                            if(afterNextRowCell && afterNextRowCell.verticalMerging === TableCellMergingState.Continue)
                                control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex + 1, nextRowCellIndex, TableCellMergingState.Restart));
                            else
                                control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex + 1, nextRowCellIndex, TableCellMergingState.None));
                        }
                    }
                    else if(cell.verticalMerging == TableCellMergingState.Continue && rowIndex > 0) {
                        let prevRow = table.rows[rowIndex - 1];
                        let prevRowCellIndex = TableCellUtils.getCellIndexByColumnIndex(prevRow, columnIndex);
                        let prevRowCell = prevRow.cells[prevRowCellIndex];
                        if(prevRowCell && prevRowCell.verticalMerging === TableCellMergingState.Restart) {
                            if(!nextRowCell || nextRowCell.verticalMerging !== TableCellMergingState.Continue)
                                control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex - 1, prevRowCellIndex, TableCellMergingState.None));
                        }
                    }
                }
            }
        }
        private static removeNestedTables(control: IRichEditControl, subDocument: SubDocument, row: TableRow) {
            let nextNestedTable: Table = subDocument.tables[row.parentTable.index + 1];
            let nestedTables: Table[] = [];
            while(nextNestedTable && nextNestedTable.nestedLevel > row.parentTable.nestedLevel) {
                if(this.isNestedTableInRow(nextNestedTable, row))
                    nestedTables.push(nextNestedTable);
                nextNestedTable = subDocument.tables[nextNestedTable.index + 1];
            }
            for(let i = nestedTables.length - 1, nestedTable: Table; nestedTable = nestedTables[i]; i--)
                control.history.addAndRedo(new RemoveTableHistoryItem(control.modelManipulator, subDocument, nestedTable));
        }

        private static isNestedTableInRow(table: Table, parentRow: TableRow) {
            return table.parentCell && (table.parentCell.parentRow === parentRow || this.isNestedTableInRow(table.parentCell.parentRow.parentTable, parentRow));
        }

        static removeNestedTablesByParentTable(control: IRichEditControl, subDocument: SubDocument, parentTable: Table) {
            let nextNestedTable: Table = subDocument.tables[parentTable.index + 1];
            let nestedTables: Table[] = [];
            while(nextNestedTable && nextNestedTable.nestedLevel > parentTable.nestedLevel) {
                if(this.isNestedTableInTable(nextNestedTable, parentTable))
                    nestedTables.push(nextNestedTable);
                nextNestedTable = subDocument.tables[nextNestedTable.index + 1];
            }
            for(let i = nestedTables.length - 1, nestedTable: Table; nestedTable = nestedTables[i]; i--)
                control.history.addAndRedo(new RemoveTableHistoryItem(control.modelManipulator, subDocument, nestedTable));
        }

        static removeNestedTablesByParentCell(control: IRichEditControl, subDocument: SubDocument, parentCell: TableCell) {
            let nextNestedTable: Table = subDocument.tables[parentCell.parentRow.parentTable.index + 1];
            let nestedTables: Table[] = [];
            while(nextNestedTable && nextNestedTable.nestedLevel > parentCell.parentRow.parentTable.nestedLevel) {
                if(this.isNestedTableInCell(nextNestedTable, parentCell))
                    nestedTables.push(nextNestedTable);
                nextNestedTable = subDocument.tables[nextNestedTable.index + 1];
            }
            for(let i = nestedTables.length - 1, nestedTable: Table; nestedTable = nestedTables[i]; i--)
                control.history.addAndRedo(new RemoveTableHistoryItem(control.modelManipulator, subDocument, nestedTable));
        }
        private static isNestedTableInCell(table: Table, parentCell: TableCell) {
            return table.parentCell && (table.parentCell === parentCell || this.isNestedTableInCell(table.parentCell.parentRow.parentTable, parentCell));
        }
        private static isNestedTableInTable(table: Table, parentTable: Table) {
            return table.parentCell && (table.parentCell.parentRow.parentTable === parentTable || this.isNestedTableInTable(table.parentCell.parentRow.parentTable, parentTable));
        }
        static normalizeVerticalSpans(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            let rowCount = table.rows.length;
            if(rowCount === 1) {
                let row = table.rows[0];
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    if(cell.verticalMerging !== TableCellMergingState.None)
                        control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, 0, cellIndex, TableCellMergingState.None));
                }
                return;
            }
            for(let rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                let row = table.rows[rowIndex];
                let columnIndex = row.gridBefore;
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    switch(cell.verticalMerging) {
                        case TableCellMergingState.Restart:
                            if(rowIndex == rowCount - 1)
                                control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                            else {
                                let bottomCellIndex = TableCellUtils.getCellIndexByColumnIndex(table.rows[rowIndex + 1], columnIndex);
                                let bottomCell = table.rows[rowIndex + 1].cells[bottomCellIndex];
                                if(!bottomCell || bottomCell.verticalMerging !== TableCellMergingState.Continue)
                                    control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                            }
                            break;
                        case TableCellMergingState.Continue:
                            if(rowIndex === 0)
                                control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                            else {
                                let topCellIndex = TableCellUtils.getCellIndexByColumnIndex(table.rows[rowIndex - 1], columnIndex);
                                let topCell = table.rows[rowIndex - 1].cells[topCellIndex];
                                if(!topCell || topCell.verticalMerging === TableCellMergingState.None) {
                                    if(rowIndex == rowCount - 1)
                                        control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                                    else {
                                        let bottomCellIndex = TableCellUtils.getCellIndexByColumnIndex(table.rows[rowIndex + 1], columnIndex);
                                        let bottomCell = table.rows[rowIndex + 1].cells[bottomCellIndex];
                                        if(bottomCell && bottomCell.verticalMerging == TableCellMergingState.Continue)
                                            control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.Restart));
                                        else
                                            control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, TableCellMergingState.None));
                                    }
                                }
                            }
                            break;
                    }
                    columnIndex += cell.columnSpan;
                }
            }
        }

        static normalizeTableGrid(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            var maxEndColumnIndex = -1;
            for(let i = 0, row: TableRow; row = table.rows[i]; i++) {
                maxEndColumnIndex = Math.max(maxEndColumnIndex, row.getTotalCellsInRowConsiderGrid() - 1);
            }
            for(let i = 0, row: TableRow; row = table.rows[i]; i++) {
                let currentEndColumnIndex = row.getTotalCellsInRowConsiderGrid() - 1;
                let gridAfterDelta = maxEndColumnIndex - currentEndColumnIndex;
                if(gridAfterDelta != 0)
                    control.history.addAndRedo(new TableRowGridAfterHistoryItem(control.modelManipulator, subDocument, table.index, i, row.gridAfter + gridAfterDelta));
            }
        }

        static normalizeCellColumnSpans(control: IRichEditControl, subDocument: SubDocument, table: Table, canNormalizeWidthBeforeAndWidthAfter: boolean) {
            this.normalizeRowsGridBefore(table, canNormalizeWidthBeforeAndWidthAfter,
                (t, ri, val) => control.history.addAndRedo(new TableRowGridBeforeHistoryItem(control.modelManipulator, subDocument, t.index, ri, val)),
                (t, ri, val) => control.history.addAndRedo(new TableRowGridAfterHistoryItem(control.modelManipulator, subDocument, t.index, ri, val)),
                (t, ri, val) => control.history.addAndRedo(new TableRowWidthBeforeHistoryItem(control.modelManipulator, subDocument, t.index, ri, val)),
                (t, ri, val) => control.history.addAndRedo(new TableRowWidthAfterHistoryItem(control.modelManipulator, subDocument, t.index, ri, val))
            );
            let intervals = new ColumnIntervalWidthCalculator(table).getIntervals();
            for(let i = table.rows.length - 1; i >= 0; i--) {
                this.normalizeTableRow(table, i, intervals.columns.slice(0),
                    (t, ri, val) => control.history.addAndRedo(new TableRowGridBeforeHistoryItem(control.modelManipulator, subDocument, t.index, ri, val)),
                    (t, ri, val) => control.history.addAndRedo(new TableRowGridAfterHistoryItem(control.modelManipulator, subDocument, t.index, ri, val)),
                    (t, ri, ci, val) => control.history.addAndRedo(new TableCellColumnSpanHistoryItem(control.modelManipulator, subDocument, t.index, ri, ci, val)));
            }
        }
        static normalizeCellColumnSpansWithoutHistory(table: Table, canNormalizeWidthBeforeAndWidthAfter: boolean) {
            this.normalizeRowsGridBefore(table, canNormalizeWidthBeforeAndWidthAfter,
                (t, ri, val) => t.rows[ri].gridBefore = val,
                (t, ri, val) => t.rows[ri].gridAfter = val,
                (t, ri, val) => t.rows[ri].widthBefore = val,
                (t, ri, val) => t.rows[ri].widthAfter = val
            );
            let intervals = new ColumnIntervalWidthCalculator(table).getIntervals();
            for(let i = table.rows.length - 1; i >= 0; i--) {
                this.normalizeTableRow(table, i, intervals.columns.slice(0),
                    (t, ri, val) => t.rows[ri].gridBefore = val,
                    (t, ri, val) => t.rows[ri].gridAfter = val,
                    (t, ri, ci, val) => t.rows[ri].cells[ci].columnSpan = val
                );
            }
        }
        private static normalizeTableRow(table: Table, rowIndex: number, intervals: TableGridColumn[],
            setGridBefore: (table: Table, rowIndex: number, val: number) => void,
            setGridAfter: (table: Table, rowIndex: number, val: number) => void,
            setColumnSpan: (table: Table, rowIndex: number, cellIndex: number, val: number) => void) {
            let row = table.rows[rowIndex];
            let span = this.calculateNewSpan(row.gridBefore, intervals);;
            if(row.gridBefore != span)
                setGridBefore(table, rowIndex, span);
            for(let i = 0, cell: TableCell; cell = row.cells[i]; i++) {
                span = this.calculateNewSpan(cell.columnSpan, intervals);
                if(cell.columnSpan !== span)
                    setColumnSpan(table, rowIndex, i, span);
            }
            span = this.calculateNewSpan(row.gridAfter, intervals);
            if(row.gridAfter !== span)
                setGridAfter(table, rowIndex, span);
        }
        private static calculateNewSpan(oldSpan: number, intervals: TableGridColumn[]) {
            let result = 0;
            let totalSum = 0;
            while(totalSum < oldSpan) {
                totalSum += intervals[0].colSpan;
                result++;
                intervals.splice(0, 1);
            }
            return result;
        }
        private static normalizeRowsGridBefore(table: Table, canNormalizeWidthBeforeAndWidthAfter: boolean,
            setGridBefore: (table: Table, rowIndex: number, val: number) => void,
            setGridAfter: (table: Table, rowIndex: number, val: number) => void,
            setWidthBefore: (table: Table, rowIndex: number, val: TableWidthUnit) => void,
            setWidthAfter: (table: Table, rowIndex: number, val: TableWidthUnit) => void) {

            let minGridBefore = table.rows[0].gridBefore;
            let minGridAfter = table.rows[0].gridAfter;
            for(let i = 1, row: TableRow; row = table.rows[i]; i++) {
                minGridBefore = Math.min(minGridBefore, row.gridBefore);
                minGridAfter = Math.min(minGridAfter, row.gridAfter);
            }
            if(minGridBefore == 0 && minGridAfter == 0)
                return;
            for(let i = 0, row: TableRow; row = table.rows[i]; i++) {
                if(minGridBefore !== 0)
                    setGridBefore(table, i, row.gridBefore - minGridBefore);
                if(row.gridBefore === 0 && (row.widthBefore.type != TableWidthUnitType.Nil || row.widthBefore.value != 0) && canNormalizeWidthBeforeAndWidthAfter)
                    setWidthBefore(table, i, TableWidthUnit.createDefault());
                if(minGridAfter != 0)
                    setGridAfter(table, i, row.gridAfter - minGridAfter);
                if(row.gridAfter == 0 && (row.widthAfter.type != TableWidthUnitType.Nil || row.widthAfter.value != 0) && canNormalizeWidthBeforeAndWidthAfter)
                    setWidthAfter(table, i, TableWidthUnit.createDefault());
            }
        }
        static normalizeTableCellWidth(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            const maxWidth = 5000;
            for(let i = 0, row: TableRow; row = table.rows[i]; i++) {
                let totalWidth = 0;
                let tableCellPreferredWidths: TableWidthUnit[] = [];
                for (let j = 0, cell: TableCell; cell = row.cells[j]; j++) {
                    let tableCellPreferredWidth = cell.preferredWidth;
                    tableCellPreferredWidths.push(tableCellPreferredWidth);
                    if(tableCellPreferredWidth.type === TableWidthUnitType.FiftiethsOfPercent)
                        totalWidth += tableCellPreferredWidth.value;
                    else {
                        totalWidth = 0;
                        break;
                    }
                }
                if(totalWidth <= maxWidth)
                    continue;
                for(let j = 0, cell: TableCell; cell = row.cells[j]; j++) {
                    let newValue = tableCellPreferredWidths[j].value * maxWidth / totalWidth;
                    let newWidth = tableCellPreferredWidths[j].clone();
                    newWidth.value = newValue;
                    control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, i, j, newWidth));
                }
            }
        }

        private createTableStructure(subDocument: SubDocument, paragraphIndex: number, newTable: Table, rowCount: number, cellCount: number) {
            let paragraph = subDocument.paragraphs[paragraphIndex];
            for(let rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                let row = new TableRow(newTable, subDocument.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(new TableRowProperties()));
                newTable.rows.push(row);
                for(let cellIndex = 0; cellIndex < cellCount; cellIndex++) {
                    let cell = new TableCell(row, subDocument.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(new TableCellProperties()));
                    row.cells.push(cell);
                    cell.startParagraphPosition = subDocument.positionManager.registerPosition(paragraph.startLogPosition.value);
                    cell.endParagrapPosition = subDocument.positionManager.registerPosition(paragraph.getEndPosition());
                    paragraph = subDocument.paragraphs[++paragraphIndex];
                }
            }
        }

        static initializeColumnWidths(control: IRichEditControl, subDocument: SubDocument, table: Table, autoFitBehavior: TableAutoFitBehaviorType, fixedColumnWidths: number, outerColumnWidth: number) {
            switch(autoFitBehavior) {
                case TableAutoFitBehaviorType.AutoFitToContents:
                    TablesManipulator.forEachCell(table, (cell, ci, ri) => {
                        control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, ri, ci, TableWidthUnit.create(0, TableWidthUnitType.Auto)));
                    });
                    break;
                case TableAutoFitBehaviorType.AutoFitToWindow:
                    control.history.addAndRedo(new TablePreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, TableWidthUnit.create(5000, TableWidthUnitType.FiftiethsOfPercent)));
                    let columnsCount = this.findTotalColumnsCountInTable(table);
                    let columnWidthInFiftiethsOfPercent = Math.max(5000 / columnsCount, 1)
                    let lastColumnCorrection = 5000 % columnsCount;
                    this.forEachCell(table, (cell, ci, ri) => {
                        control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, ri, ci, TableWidthUnit.create(columnWidthInFiftiethsOfPercent, TableWidthUnitType.FiftiethsOfPercent)));
                    });
                    break;
                case TableAutoFitBehaviorType.FixedColumnWidth:
                    control.history.addAndRedo(new TableLayoutTypeHistoryItem(control.modelManipulator, subDocument, table.index, TableLayoutType.Fixed, true));
                    control.history.addAndRedo(new TablePreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, TableWidthUnit.create(0, TableWidthUnitType.Auto)));
                    if(fixedColumnWidths !== Number.MAX_VALUE && fixedColumnWidths !== Number.MIN_VALUE && fixedColumnWidths !== 0) {
                        TablesManipulator.forEachCell(table, (cell, ci, ri) => {
                            control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, ri, ci, TableWidthUnit.create(fixedColumnWidths, TableWidthUnitType.ModelUnits)));
                        });
                    }
                    else {
                        let totalColumnsInTable = this.findTotalColumnsCountInTable(table);
                        let widths = this.distributeWidthsToAllColumns(outerColumnWidth, totalColumnsInTable);
                        this.forEachCell(table, (cell, ci, ri) => {
                            let cellWidth = widths[ci];
                            control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(control.modelManipulator, subDocument, table.index, ri, ci, TableWidthUnit.create(cellWidth, TableWidthUnitType.ModelUnits)));
                        });
                    }
                    break;
            }
        }

        static validateTableIndent(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            let paragraphIndex = subDocument.getParagraphIndexByPosition(table.getStartPosition());
            let paragraph = subDocument.paragraphs[paragraphIndex];
            let leftIndent = paragraph.getParagraphMergedProperies().leftIndent;
            let firstLineIndent = paragraph.getParagraphMergedProperies().firstLineIndent;
            let firstLineIndentType = paragraph.getParagraphMergedProperies().firstLineIndentType;
            if(leftIndent === 0 && firstLineIndent === 0 && firstLineIndentType === ParagraphFirstLineIndent.None)
                return;
            //if(leftIndent != 0)
            //    control.history.addAndRedo(new TableIndentHistoryItem(control.modelManipulator, subDocument, table.index, TableWidthUnit.create(leftIndent, TableWidthUnitType.ModelUnits), true));
            let endParagraphIndex = subDocument.getParagraphIndexByPosition(table.getEndPosition() - 1);
            for(; paragraphIndex <= endParagraphIndex; paragraphIndex++) {
                paragraph = subDocument.paragraphs[paragraphIndex];
                control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(control.modelManipulator, subDocument, paragraph.getInterval(), 0, true));
                control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(control.modelManipulator, subDocument, paragraph.getInterval(), 0, true));
                control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(control.modelManipulator, subDocument, paragraph.getInterval(), ParagraphFirstLineIndent.None, true));
            }
        }

        static tryJoinTables(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            let paragraphIndex = subDocument.getParagraphIndexByPosition(table.getStartPosition());
            if(paragraphIndex === 0)
                return;
            let previousParagraph = subDocument.paragraphs[paragraphIndex - 1];
            let previousParagraphCell = Table.getTableCellByPosition(subDocument.tables, previousParagraph.startLogPosition.value);
            if(!previousParagraphCell || table.nestedLevel !== previousParagraphCell.parentRow.parentTable.nestedLevel)
                return;
            this.joinTablesCore(control, [previousParagraphCell.parentRow.parentTable, table]);
        }

        private static joinTablesCore(control: IRichEditControl, tables: Table[]) {
            if(tables.length < 2)
                throw new Error("tables.length should be > 2");

        }

        static forEachCell(table: Table, func: (cell: TableCell, cellIndex: number, rowIndex: number) => void) {
            for(let i = 0, row: TableRow; row = table.rows[i]; i++) {
                for(let j = 0, cell: TableCell; cell = row.cells[j]; j++) {
                    func(cell, j, i);
                }
            }
        }

        static distributeWidthsToAllColumns(width: number, count: number): number[] {
            let result = [];
            if(count == 0)
                return result;
            let rest = width;
            for(let i = 0; i < count; i++) {
                let cellWidth = Math.ceil(rest / (count - i));
                result[i] = Math.max(cellWidth, 1);
                rest -= cellWidth;
            }
            return result;
        }

        private static getTableWidth(subDocument: SubDocument, table: Table, columnWidth: number) {
            let leftMargin = this.getTableCellLeftMargin(table.rows[0].cells[0], subDocument.documentModel);
            let rightMargin = this.getTableCellRightMargin(table.rows[0].cells[table.rows[0].cells.length - 1], subDocument.documentModel);

            let leftMarginValue = leftMargin.type === TableWidthUnitType.ModelUnits ? leftMargin.value : 0;
            let rightMarginValue = rightMargin.type === TableWidthUnitType.ModelUnits ? rightMargin.value : 0;
            return columnWidth + leftMarginValue + rightMarginValue;
        }

        static findTotalColumnsCountInTable(table: Table): number {
            let result = 0;
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let cells = row.getTotalCellsInRowConsiderGrid();
                result = Math.max(result, cells);
            }
            return result;
        }

        private static getTableCellRightMargin(cell: TableCell, model: DocumentModel): TableWidthUnit {
            return new TableCellPropertiesMergerMarginRight(cell.parentRow.parentTable, model).getProperty(cell.properties, cell.parentRow.parentTable.style, cell.conditionalFormatting, model.defaultTableCellProperties);
        }
        private static getTableCellLeftMargin(cell: TableCell, model: DocumentModel): TableWidthUnit {
            return new TableCellPropertiesMergerMarginLeft(cell.parentRow.parentTable, model).getProperty(cell.properties, cell.parentRow.parentTable.style, cell.conditionalFormatting, model.defaultTableCellProperties);
        }

        private removeTableCore(table: Table, tables: Table[], tablesByLevels: Table[][]) {
            let position = table.getStartPosition();
            tablesByLevels[table.nestedLevel].splice(Utils.binaryIndexOf(tablesByLevels[table.nestedLevel], t => t.getStartPosition() - position), 1);
            tables.splice(table.index, 1);
            this.advanceTablesIndices(tables, table.index, -1);
        }
        private insertTableCore(table: Table, tables: Table[], tablesByLevels: Table[][], parentCell: TableCell) {
            table.nestedLevel = parentCell ? parentCell.parentRow.parentTable.nestedLevel + 1 : 0;
            table.parentCell = parentCell;
            let position = table.getStartPosition();
            if(!tablesByLevels[table.nestedLevel])
                tablesByLevels[table.nestedLevel] = [];
            let indexInNestedLevel = Utils.normedBinaryIndexOf(tablesByLevels[table.nestedLevel], t => t.getStartPosition() - position);
            tablesByLevels[table.nestedLevel].splice(indexInNestedLevel + 1, 0, table);

            if(!parentCell)
                table.index = Math.max(0, Utils.normedBinaryIndexOf(tables, t => t.getStartPosition() - position) + 1);
            else {
                let parentTable = parentCell.parentRow.parentTable;
                let prevTableInNestedLevel = tablesByLevels[table.nestedLevel][indexInNestedLevel];
                table.index = prevTableInNestedLevel && prevTableInNestedLevel.index > parentTable.index ? (prevTableInNestedLevel.index + 1) : (parentTable.index + 1);
            }
            tables.splice(table.index, 0, table);
            this.advanceTablesIndices(tables, table.index + 1, 1);
        }
        private advanceTablesIndices(tables: Table[], startIndex: number, shift: number) {
            for(var i = startIndex, table: Table; table = tables[i]; i++) {
                table.index += shift;
            }
        }

        static normalizeVerticalMerging(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            let row = table.rows[rowIndex];
            let cell = row.cells[cellIndex];
            for(let nextCellIndex = cellIndex + 1, nextCell: TableCell; nextCell = row.cells[nextCellIndex]; nextCellIndex++) {
                if(nextCell.verticalMerging === TableCellMergingState.None)
                    continue;
                let nextCellVerticalMerging = nextCell.verticalMerging;
                let firstCellInMergingGroupPosition = Table.getFirstCellPositionInVerticalMergingGroup(TablePosition.createAndInit(table, rowIndex, nextCellIndex).init());
                let startColumnIndex = TableCellUtils.getStartColumnIndex(firstCellInMergingGroupPosition.cell);
                let verticalSpanCellPositions = TableCellUtils.getVerticalSpanCellPositions(firstCellInMergingGroupPosition, startColumnIndex);
                control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(control.modelManipulator, subDocument, table.index, rowIndex, nextCellIndex, TableCellMergingState.None));
                if(nextCellVerticalMerging === TableCellMergingState.Restart) {
                    if(verticalSpanCellPositions.length > 2)
                        control.history.addAndRedo(TableCellVerticalMergingHistoryItem.fromPosition(control.modelManipulator, subDocument, verticalSpanCellPositions[1], TableCellMergingState.Restart));
                    else {

                        control.history.addAndRedo(TableCellVerticalMergingHistoryItem.fromPosition(control.modelManipulator, subDocument, verticalSpanCellPositions[0], TableCellMergingState.None));
                        control.history.addAndRedo(TableCellVerticalMergingHistoryItem.fromPosition(control.modelManipulator, subDocument, verticalSpanCellPositions[1], TableCellMergingState.None));
                    }
                }
                else {
                    let nextCellIndexInMergingGroup = TablePosition.indexOfCell(verticalSpanCellPositions, nextCell);
                    if(nextCellIndexInMergingGroup === 1)
                        control.history.addAndRedo(TableCellVerticalMergingHistoryItem.fromPosition(control.modelManipulator, subDocument, verticalSpanCellPositions[0], TableCellMergingState.None));
                    if(verticalSpanCellPositions.length - 2 === nextCellIndexInMergingGroup)
                        control.history.addAndRedo(TableCellVerticalMergingHistoryItem.fromPosition(control.modelManipulator, subDocument, verticalSpanCellPositions[verticalSpanCellPositions.length - 1], TableCellMergingState.None));
                    else if(verticalSpanCellPositions.length - 1 !== nextCellIndexInMergingGroup)
                        control.history.addAndRedo(TableCellVerticalMergingHistoryItem.fromPosition(control.modelManipulator, subDocument, verticalSpanCellPositions[nextCellIndexInMergingGroup + 1], TableCellMergingState.Restart));
                }
            }
        }

        static normalizeRows(control: IRichEditControl, subDocument: SubDocument, table: Table) {
            for(let i = table.rows.length - 1, row: TableRow; row = table.rows[i]; i--) {
                if (this.areAllCellsHasVerticalMerge(row)) {
                    let height = row.height;
                    if (height.type !== TableHeightUnitType.Auto && i > 0) {
                        let prevRowHeight = table.rows[i - 1].height;
                        control.history.addAndRedo(new TableRowHeightHistoryItem(control.modelManipulator, subDocument, table.index, i - 1, TableHeightUnit.create(prevRowHeight.value + height.value, height.type)))
                    }
                    this.removeTableRowWithContent(control, subDocument, table, i);
                }
            }
        }
        private static areAllCellsHasVerticalMerge(row: TableRow) {
            for(let i = 0, cell: TableCell; cell = row.cells[i]; i++) {
                if(cell.verticalMerging !== TableCellMergingState.Continue)
                    return false;
            }
            return true;
        }

        static mergeTwoTableCellsHorizontally(control: IRichEditControl, subDocument: SubDocument, cellPosition: TablePosition) {
            new MergeTwoTableCellsHorizontallyOperation(control, subDocument).execute(cellPosition, true);
        }

        static mergeTwoTableCellsVertically(control: IRichEditControl, subDocument: SubDocument, cellPosition: TablePosition) {
            new MergeTwoTableCellsVerticallyOperation(control, subDocument).execute(cellPosition, true);
        }

        static insertCellToTheRight(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, canNormalizeTable: boolean = true, canNormalizeVerticalMerging: boolean = true, canCopyProperties: boolean = true) {
            new InsertTableCellToTheRightOperation(control, subDocument).execute(table, rowIndex, cellIndex, canNormalizeTable, canNormalizeVerticalMerging, canCopyProperties);
        }
        static insertCellToTheLeft(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, canNormalizeTable: boolean = true, canNormalizeVerticalMerging: boolean = true, canCopyProperties: boolean = true) {
            new InsertTableCellToTheLeftOperation(control, subDocument).execute(table, rowIndex, cellIndex, canNormalizeTable, canNormalizeVerticalMerging, canCopyProperties);
        }

        static insertRowBelow(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number) {
            new InsertTableRowBelowOperation(control, subDocument).execute(table, rowIndex);
        }

        static insertRowAbove(control: IRichEditControl, subDocument: SubDocument, table: Table, rowIndex: number) {
            new InsertTableRowAboveOperation(control, subDocument).execute(table, rowIndex);
        }

        static insertTable(control: IRichEditControl, subDocument: SubDocument, rowCount: number, cellCount: number, position: number, availableWidth: number): Table {
            if(rowCount < 1 || cellCount < 1)
                throw new Error("rowCount and cellCount must be greater than 0");

            control.history.beginTransaction();
            let targetParagraphIndex = subDocument.getParagraphIndexByPosition(position);
            let targetParagraph = subDocument.paragraphs[targetParagraphIndex];
            if(targetParagraph.startLogPosition.value !== position || Utils.binaryIndexOf(subDocument.tables, t => t.getEndPosition() - position) >= 0) {
                ModelManipulator.insertParagraph(control, subDocument, new FixedInterval(position, 0));
                position++;
                targetParagraphIndex++;
            }

            let newParagraphCount = rowCount * cellCount;
            for(let i = 0; i < newParagraphCount; i++)
                ModelManipulator.insertParagraph(control, subDocument, new FixedInterval(position + i, 0));
            control.history.addAndRedo(new CreateTableHistoryItem(control.modelManipulator, subDocument, targetParagraphIndex, rowCount, cellCount));
            let table = Table.getTableByPosition(subDocument.tables, position, true);
            TablesManipulator.initializeColumnWidths(control, subDocument, table, TableAutoFitBehaviorType.FixedColumnWidth, Number.MIN_VALUE, availableWidth);
            TablesManipulator.validateTableIndent(control, subDocument, table);
            control.history.addAndRedo(new TableLookTypesHistoryItem(control.modelManipulator, subDocument, table.index, TableLookTypes.ApplyFirstRow | TableLookTypes.ApplyFirstColumn | TableLookTypes.DoNotApplyColumnBanding));
            TablesManipulator.tryJoinTables(control, subDocument, table);
            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.No, false)
            control.history.endTransaction();
            return table;
        }

        static removeTablesOnInterval(control: IRichEditControl, subDocument: SubDocument, interval: FixedInterval, removeTableIfItMatchesWithInterval: boolean) {
            let intervalEnd = interval.end();
            let startTableIndex = Math.max(Utils.normedBinaryIndexOf(subDocument.tables, t => t.getStartPosition() - interval.start), 0); // TODO: optimize - search on the root level only
            while(startTableIndex > 0 && subDocument.tables[startTableIndex].nestedLevel > 0)
                startTableIndex--;
            let tablesForRemoving: Table[] = [];
            for(let tableIndex = startTableIndex, table: Table; table = subDocument.tables[tableIndex]; tableIndex++) {
                let tableStartPosition = table.getStartPosition();
                if(intervalEnd <= tableStartPosition)
                    break;
                let tableEndPosition = table.getEndPosition()
                if(tableStartPosition === interval.start && tableEndPosition === intervalEnd) {
                    if(removeTableIfItMatchesWithInterval)
                        tablesForRemoving.push(table);
                }
                else if(interval.containsInterval(tableStartPosition, tableEndPosition))
                    tablesForRemoving.push(table);
            }

            for(let i = tablesForRemoving.length - 1; i >= 0; i--)
                control.history.addAndRedo(new RemoveTableHistoryItem(control.modelManipulator, subDocument, tablesForRemoving[i]));
        }
    }
}