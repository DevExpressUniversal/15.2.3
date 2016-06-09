module __aspxRichEdit {
    export class ToggleTableCellsBorderCommandBase extends CommandBase<SimpleCommandState> {
        affectNeighbours: boolean = true;
        affectInner: boolean = true;
        affectOuter: boolean = true;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.All;

        getState(): SimpleCommandState {
            let selectedCells = this.control.selection.getSelectedCells();
            let enabled = this.isEnabled() && TableCellUtils.areCellsSelectedInSeries(selectedCells);
            let state = new SimpleCommandState(enabled);
            if(state.enabled)
                state.value = this.isChecked(selectedCells);
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        isChecked(selectedCells: TableCell[][]): boolean {
            let table = selectedCells[0][0].parentRow.parentTable;
            let spacing = new TablePropertiesMergerCellSpacing().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties);
            let hasSpacing = spacing.value > 0;
            if(this.control.selection.isSelectedEntireTable())
                return this.checkBorderInEntireTable(this.getPatternBorderInfo(), table);
            let bordersGrid = this.createCellBorderGrid(table, selectedCells, hasSpacing);
            return this.isCheckedInParticallyTableSelection(table, bordersGrid, hasSpacing);
        }
        executeCore(state: SimpleCommandState, parameters: boolean): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let table = selectedCells[0][0].parentRow.parentTable;
            let hasSpacing = new TablePropertiesMergerCellSpacing().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties).value > 0;
            let patternBorder = this.getPatternBorderInfo();
            let subDocument = this.control.model.activeSubDocument;
            if(this.control.selection.isSelectedEntireTable())
                this.applyBorderToTable(subDocument, table, state.value ? null : patternBorder);
            let grid = this.createCellBorderGrid(table, selectedCells, hasSpacing);
            for(let i = 0, gridRow: TableBorderGridCell[]; gridRow = grid[i]; i++) {
                let cellIndex = -1;
                let prevCell: TableCell = null;
                for(let j = 0, gridCell: TableBorderGridCell; gridCell = gridRow[j]; j++) {
                    if(gridCell.cell != prevCell)
                        cellIndex++;
                    prevCell = gridCell.cell;
                    if(!gridCell.cell || gridCell.cell.verticalMerging === TableCellMergingState.Continue)
                        continue;
                    this.applyBorderToCell(subDocument, table, gridCell, i, cellIndex, state.value ? null : patternBorder.clone());
                }
            }
            this.control.history.endTransaction();
            return true;
        }
        private isCheckedInParticallyTableSelection(table: Table, grid: TableBorderGridCell[][], hasSpacing: boolean): boolean {
            let hasAffectedCells = false;
            let leftTableBorder = !hasSpacing && this.affectOuter ? new TablePropertiesMergerBorderLeft().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties) : null;
            let rightTableBorder = !hasSpacing && this.affectOuter ? new TablePropertiesMergerBorderRight().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties) : null;
            let bottomTableBorder = !hasSpacing && this.affectOuter ? new TablePropertiesMergerBorderBottom().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties) : null;
            let topTableBorder = !hasSpacing && this.affectOuter ? new TablePropertiesMergerBorderTop().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties) : null;
            let patternBorder = this.getPatternBorderInfo();
            for(let i = 0, gridRow: TableBorderGridCell[]; gridRow = grid[i]; i++) {
                for(let j = 0, gridCell: TableBorderGridCell; gridCell = gridRow[j]; j++) {
                    if(!gridCell.cell || gridCell.cell.verticalMerging === TableCellMergingState.Continue || gridCell.info === TableBorderGridCellInfo.None)
                        continue;
                    if(!(gridCell.info & this.affectOnStateFlags))
                        continue;
                    hasAffectedCells = true;
                    if(!this.checkBorderInParticallyTableSelection(patternBorder, table, grid, gridCell, i, j, topTableBorder, rightTableBorder, bottomTableBorder, leftTableBorder))
                        return false;
                }
            }
            return hasAffectedCells;
        }
        createCellBorderGrid(table: Table, selectedCells: TableCell[][], hasSpacing: boolean): TableBorderGridCell[][] {
            let grid = this.createCellBorderGridCore(table, selectedCells);
            let rowsCount = grid.length;
            let columnsCount = grid[0].length;
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let columnIndex = row.gridBefore;
                let isLastRow = rowIndex + 1 === rowsCount;
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    let isLastCell = columnIndex + cell.columnSpan === columnsCount;
                    let gridCell = grid[rowIndex][columnIndex];
                    let position = TableBorderGridCellInfo.None;
                    if(cell.verticalMerging === TableCellMergingState.Continue) {
                        if(isLastRow && grid[rowIndex - 1][columnIndex].info & TableBorderGridCellInfo.BottomOuter) {
                            let prevRowIndex = rowIndex;
                            do {
                                prevRowIndex--;
                                grid[prevRowIndex][columnIndex].info |= TableBorderGridCellInfo.BottomTableOuter;
                            } while(grid[prevRowIndex][columnIndex].cell.verticalMerging !== TableCellMergingState.Restart)
                        }
                        gridCell.info = grid[rowIndex - 1][columnIndex].info;
                    }
                    else if(!gridCell.selected) {
                        if(!hasSpacing && this.affectNeighbours) {
                            if(!isLastRow && this.checkBottomSibling(grid, rowIndex, columnIndex, ToggleTableCellsBorderCommandBase.checkVSiblingSelected))
                                gridCell.info |= TableBorderGridCellInfo.TopNeighbour;
                            if(this.checkLeftRightSiblingSelected(cell, rowIndex, columnIndex, columnIndex - 1, TableBorderGridCellInfo.RightNeighbour, grid))
                                gridCell.info |= TableBorderGridCellInfo.RightNeighbour;
                            if(ToggleTableCellsBorderCommandBase.checkVSiblingSelected(cell.columnSpan, rowIndex - 1, columnIndex, grid))
                                gridCell.info |= TableBorderGridCellInfo.BottomNeighbour;
                            if(!isLastCell && this.checkLeftRightSiblingSelected(cell, rowIndex, columnIndex, columnIndex + cell.columnSpan, TableBorderGridCellInfo.LeftNeighbour, grid))
                                gridCell.info |= TableBorderGridCellInfo.LeftNeighbour;
                        }
                    }
                    else {
                        if(this.affectInner) {
                            if(ToggleTableCellsBorderCommandBase.checkVSiblingSelected(cell.columnSpan, rowIndex - 1, columnIndex, grid))
                                gridCell.info |= TableBorderGridCellInfo.TopInner;
                            if(!isLastRow && this.checkBottomSibling(grid, rowIndex, columnIndex, ToggleTableCellsBorderCommandBase.checkVSiblingSelected))
                                gridCell.info |= TableBorderGridCellInfo.BottomInner;
                            if(this.checkLeftRightSiblingSelected(cell, rowIndex, columnIndex, columnIndex - 1, TableBorderGridCellInfo.LeftInner, grid))
                                gridCell.info |= TableBorderGridCellInfo.LeftInner;
                            if(!isLastCell && this.checkLeftRightSiblingSelected(cell, rowIndex, columnIndex, columnIndex + cell.columnSpan, TableBorderGridCellInfo.RightInner, grid))
                                gridCell.info |= TableBorderGridCellInfo.RightInner;
                        }
                        if(this.affectOuter) {
                            if(ToggleTableCellsBorderCommandBase.checkVSiblingUnselected(cell.columnSpan, rowIndex - 1, columnIndex, grid))
                                gridCell.info |= TableBorderGridCellInfo.TopOuter;
                            if(isLastRow || this.checkBottomSibling(grid, rowIndex, columnIndex, ToggleTableCellsBorderCommandBase.checkVSiblingUnselected))
                                gridCell.info |= TableBorderGridCellInfo.BottomOuter;
                            if(!this.checkLeftRightSiblingSelected(cell, rowIndex, columnIndex, columnIndex - 1, TableBorderGridCellInfo.LeftInner, grid))
                                gridCell.info |= TableBorderGridCellInfo.LeftOuter;
                            if(isLastCell || !this.checkLeftRightSiblingSelected(cell, rowIndex, columnIndex, columnIndex + cell.columnSpan, TableBorderGridCellInfo.RightInner, grid))
                                gridCell.info |= TableBorderGridCellInfo.RightOuter;
                            if(columnIndex === 0)
                                gridCell.info |= TableBorderGridCellInfo.LeftTableOuter;
                            if(columnIndex + cell.columnSpan === columnsCount)
                                gridCell.info |= TableBorderGridCellInfo.RightTableOuter;
                            if(rowIndex === 0)
                                gridCell.info |= TableBorderGridCellInfo.TopTableOuter;
                            if(rowIndex === rowsCount - 1)
                                gridCell.info |= TableBorderGridCellInfo.BottomTableOuter;
                        }
                    }
                    columnIndex += cell.columnSpan;
                }
            }
            return grid;
        }
        private createCellBorderGridCore(table: Table, selectedCells: TableCell[][]): TableBorderGridCell[][] {
            let grid: TableBorderGridCell[][] = [];
            let selectedCellsVIndex = 0;
            let selectedCellsHIndex = -1;
            let horSelectedCells: TableCell[];
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                horSelectedCells = selectedCells[selectedCellsVIndex];
                if(horSelectedCells && horSelectedCells[0].parentRow === row) {
                    selectedCellsVIndex++;
                    selectedCellsHIndex = 0;
                }
                else
                    selectedCellsHIndex = -1;
                grid.push([]);
                for(let i = 0; i < row.gridBefore; i++)
                    grid[rowIndex].push({ cell: null, info: TableBorderGridCellInfo.None, selected: false });
                let columnIndex = row.gridBefore;
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    let borderGridCell = { cell: cell, info: TableBorderGridCellInfo.None, selected: false };
                    if(cell.verticalMerging === TableCellMergingState.Continue && grid[rowIndex - 1][columnIndex].selected) {
                        if(grid[rowIndex - 1][columnIndex].selected)
                            borderGridCell.selected = true;
                    }
                    if(selectedCellsHIndex > -1 && horSelectedCells[selectedCellsHIndex] === cell) {
                        borderGridCell.selected = true;
                        selectedCellsHIndex++;
                    }
                    for(let i = 0; i < cell.columnSpan; i++)
                        grid[rowIndex].push(borderGridCell);
                    columnIndex += cell.columnSpan;
                }
                for(let i = 0; i < row.gridAfter; i++)
                    grid[rowIndex].push({ cell: null, info: TableBorderGridCellInfo.None, selected: false });
            }
            return grid;
        }
        private checkBottomSibling(grid: TableBorderGridCell[][], rowIndex: number, columnIndex: number, testFunc: (columnSpan: number, sibRowIndex: number, columnIndex: number, grid: TableBorderGridCell[][]) => boolean): boolean {
            let gridCell = grid[rowIndex][columnIndex];
            if(gridCell.cell.verticalMerging === TableCellMergingState.Restart) {
                while(grid[rowIndex]) {
                    rowIndex++;
                    if(!grid[rowIndex] || grid[rowIndex][columnIndex].cell.verticalMerging !== TableCellMergingState.Continue)
                        break;
                }
            }
            else
                rowIndex++;
            return testFunc(gridCell.cell.columnSpan, rowIndex, columnIndex, grid);
        }
        private static checkVSiblingSelected(columnSpan: number, checkingRowIndex: number, columnIndex: number, grid: TableBorderGridCell[][]): boolean {
            if(checkingRowIndex < 0)
                return false;
            if(!grid[checkingRowIndex])
                return false;
            for(let i = 0; i < columnSpan; i++) {
                if(!grid[checkingRowIndex][columnIndex + i].selected)
                    return false;
            }
            return true;
        }
        private static checkVSiblingUnselected(columnSpan: number, checkingRowIndex: number, columnIndex: number, grid: TableBorderGridCell[][]): boolean {
            if(checkingRowIndex < 0)
                return true;
            if(!grid[checkingRowIndex])
                return true;
            for(let i = 0; i < columnSpan; i++) {
                if(grid[checkingRowIndex][columnIndex + i].selected)
                    return false;
            }
            return true;
        }
        private checkLeftRightSiblingSelected(cell: TableCell, rowIndex: number, columnIndex: number, sibColumnIndex: number, testingPosition: TableBorderGridCellInfo, grid: TableBorderGridCell[][]): boolean {
            if(sibColumnIndex < 0)
                return false;
            if(!grid[rowIndex][sibColumnIndex].selected)
                return false;
            if(cell.verticalMerging === TableCellMergingState.Restart) {
                while(true) {
                    rowIndex++;
                    if(!grid[rowIndex] || !grid[rowIndex][columnIndex].cell || grid[rowIndex][columnIndex].cell.verticalMerging !== TableCellMergingState.Continue)
                        break;
                    if(!grid[rowIndex][sibColumnIndex].selected)
                        return false;
                }
            }
            else if(cell.verticalMerging === TableCellMergingState.Continue) {
                while(rowIndex > 0) {
                    rowIndex--;
                    if(grid[rowIndex][columnIndex].cell.verticalMerging === TableCellMergingState.Restart) {
                        if(!(grid[rowIndex][columnIndex].info & testingPosition))
                            return false;
                        break;
                    }
                }
            }
            return true;
        }
        protected getPatternBorderInfo(): BorderInfo {
            return this.control.model.repositoryBorderItem;
        }
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            throw new Error(Errors.NotImplemented);
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            throw new Error(Errors.NotImplemented);
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            throw new Error(Errors.NotImplemented);
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            throw new Error(Errors.NotImplemented);
        }

        protected getActualLeftBorder(grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, leftTableBorder: BorderInfo, table: Table, rowIndex: number, columnIndex: number): BorderInfo {
            let border = new TableCellPropertiesMergerBorderLeft().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties);
            let leftBorder: BorderInfo;
            if(currentCell.info & TableBorderGridCellInfo.LeftTableOuter)
                leftBorder = leftTableBorder;
            else {
                let leftCell = grid[rowIndex][columnIndex - 1];
                leftBorder = leftCell ? new TableCellPropertiesMergerBorderRight().getProperty(leftCell.cell.properties, table.style, leftCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties) : null;
            }
            return leftBorder ? TableBorderCalculator.getPowerfulBorder(border, leftBorder) : border;
        }

        protected getActualRightBorder(grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rightTableBorder: BorderInfo, table: Table, rowIndex: number, columnIndex: number): BorderInfo {
            let border = new TableCellPropertiesMergerBorderRight().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties);
            let rightBorder: BorderInfo = null;
            if(currentCell.info & TableBorderGridCellInfo.RightTableOuter)
                rightBorder = rightTableBorder;
            else {
                let rightCell = grid[rowIndex][columnIndex + 1];
                rightBorder = rightCell.cell ? new TableCellPropertiesMergerBorderLeft().getProperty(rightCell.cell.properties, table.style, rightCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties) : null;
            }
            return rightBorder ? TableBorderCalculator.getPowerfulBorder(border, rightBorder) : border;
        }

        protected getActualTopBorder(grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, topTableBorder: BorderInfo, table: Table, rowIndex: number, columnIndex: number): BorderInfo {
            let border = new TableCellPropertiesMergerBorderTop().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties);
            let topBorder: BorderInfo = null;
            if(currentCell.info & TableBorderGridCellInfo.TopTableOuter)
                topBorder = topTableBorder;
            else {
                let topCell = grid[rowIndex - 1][columnIndex];
                topBorder = topCell.cell ? new TableCellPropertiesMergerBorderBottom().getProperty(topCell.cell.properties, table.style, topCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties) : null;
            }
            return topBorder ? TableBorderCalculator.getPowerfulBorder(border, topBorder) : border;
        }

        protected getActualBottomBorder(grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, bottomTableBorder: BorderInfo, table: Table, rowIndex: number, columnIndex: number): BorderInfo {
            let border = new TableCellPropertiesMergerBorderBottom().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties);
            let bottomBorder: BorderInfo = null;
            if(currentCell.info & TableBorderGridCellInfo.BottomTableOuter)
                bottomBorder = bottomTableBorder;
            else {
                let bottomCell = grid[rowIndex + 1][columnIndex];
                bottomBorder = bottomCell.cell ? new TableCellPropertiesMergerBorderTop().getProperty(bottomCell.cell.properties, table.style, bottomCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties) : null;
            }
            return bottomBorder ? TableBorderCalculator.getPowerfulBorder(border, bottomBorder) : border;
        }
    }

    export class ToggleTableCellsTopBorderCommand extends ToggleTableCellsBorderCommandBase {
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.TopOuter;
        affectInner: boolean = false;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            return patternBorder.equals(this.getActualTopBorder(grid, currentCell, topTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderTop().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            if(!borderInfo) {
                borderInfo = table.properties.borders.topBorder.clone();
                borderInfo.width = 0;
            }
            else
                borderInfo = borderInfo.clone();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, table.index,
                [borderInfo, undefined, undefined, undefined, undefined, undefined],
                [true, undefined, undefined, undefined, undefined, undefined]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            if(gridCell.info & TableBorderGridCellInfo.TopOuter) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.topBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [borderInfo, undefined, undefined, undefined, undefined, undefined],
                    [true, undefined, undefined, undefined, undefined, undefined]));
            }
            if(gridCell.info & TableBorderGridCellInfo.TopNeighbour) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.bottomBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, undefined, borderInfo, undefined, undefined, undefined],
                    [undefined, undefined, true, undefined, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellsRightBorderCommand extends ToggleTableCellsBorderCommandBase {
        affectInner: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.RightOuter;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            return patternBorder.equals(this.getActualRightBorder(grid, currentCell, rightTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderRight().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            if(!borderInfo) {
                borderInfo = table.properties.borders.rightBorder.clone();
                borderInfo.width = 0;
            }
            else
                borderInfo = borderInfo.clone();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, table.index,
                [undefined, borderInfo, undefined, undefined, undefined, undefined],
                [undefined, true, undefined, undefined, undefined, undefined]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            if(gridCell.info & TableBorderGridCellInfo.RightOuter) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.rightBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, borderInfo, undefined, undefined, undefined, undefined],
                    [undefined, true, undefined, undefined, undefined, undefined]));
            }
            if(gridCell.info & TableBorderGridCellInfo.RightNeighbour) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.leftBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, undefined, undefined, borderInfo, undefined, undefined],
                    [undefined, undefined, undefined, true, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellsBottomBorderCommand extends ToggleTableCellsBorderCommandBase {
        affectInner: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.BottomOuter;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            return patternBorder.equals(this.getActualBottomBorder(grid, currentCell, bottomTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderBottom().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            if(!borderInfo) {
                borderInfo = table.properties.borders.bottomBorder.clone();
                borderInfo.width = 0;
            }
            else
                borderInfo = borderInfo.clone();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, table.index,
                [undefined, undefined, borderInfo, undefined, undefined, undefined],
                [undefined, undefined, true, undefined, undefined, undefined]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            if(gridCell.info & TableBorderGridCellInfo.BottomOuter) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.bottomBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, undefined, borderInfo, undefined, undefined, undefined],
                    [undefined, undefined, true, undefined, undefined, undefined]));
            }
            if(gridCell.info & TableBorderGridCellInfo.BottomNeighbour) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.topBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [borderInfo, undefined, undefined, undefined, undefined, undefined],
                    [true, undefined, undefined, undefined, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellsLeftBorderCommand extends ToggleTableCellsBorderCommandBase {
        affectInner: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.LeftOuter;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            return patternBorder.equals(this.getActualLeftBorder(grid, currentCell, leftTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderLeft().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            if(!borderInfo) {
                borderInfo = table.properties.borders.leftBorder.clone();
                borderInfo.width = 0;
            }
            else
                borderInfo = borderInfo.clone();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, table.index,
                [undefined, undefined, undefined, borderInfo, undefined, undefined],
                [undefined, undefined, undefined, true, undefined, undefined]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            if(gridCell.info & TableBorderGridCellInfo.LeftOuter) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.leftBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, undefined, undefined, borderInfo, undefined, undefined],
                    [undefined, undefined, undefined, true, undefined, undefined]));
            }
            if(gridCell.info & TableBorderGridCellInfo.LeftNeighbour) {
                if(!borderInfo) {
                    borderInfo = gridCell.cell.properties.borders.rightBorder.clone();
                    borderInfo.width = 0;
                }
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, borderInfo, undefined, undefined, undefined, undefined],
                    [undefined, true, undefined, undefined, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellNoBorderCommand extends ToggleTableCellsBorderCommandBase {
        isChecked(selectedCells: TableCell[][]): boolean {
            return false;
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index,
                [new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo()],
                [true, true, true, true, true, true])
            );
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            let borders = new BorderInfo();
            this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                [borders.clone(), borders.clone(), borders.clone(), borders.clone(), borders.clone(), borders.clone()],
                [false, false, false, false, false, false])
            );
        }
    }

    export class ToggleTableCellAllBordersCommand extends ToggleTableCellsBorderCommandBase {
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.BottomInner | TableBorderGridCellInfo.BottomOuter | TableBorderGridCellInfo.LeftInner | TableBorderGridCellInfo.LeftOuter | TableBorderGridCellInfo.RightInner | TableBorderGridCellInfo.RightOuter | TableBorderGridCellInfo.TopInner | TableBorderGridCellInfo.TopOuter;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            return patternBorder.equals(new TableCellPropertiesMergerBorderTop().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties)) &&
                patternBorder.equals(new TableCellPropertiesMergerBorderRight().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties)) &&
                patternBorder.equals(new TableCellPropertiesMergerBorderBottom().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties)) &&
                patternBorder.equals(new TableCellPropertiesMergerBorderLeft().getProperty(currentCell.cell.properties, table.style, currentCell.cell.conditionalFormatting, this.control.model.defaultTableCellProperties));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderTop().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderRight().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderBottom().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderLeft().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderHorizontal().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderVertical().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            borderInfo = borderInfo || new BorderInfo();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, [
                borderInfo.clone(), borderInfo.clone(), borderInfo.clone(), borderInfo.clone(), borderInfo.clone(), borderInfo.clone()
            ], [true, true, true, true, true, true]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            let topBorder: boolean,
                rightBorder: boolean,
                bottomBorder: boolean,
                leftBorder: boolean;
            if(gridCell.info & TableBorderGridCellInfo.TopOuter || gridCell.info & TableBorderGridCellInfo.TopInner || gridCell.info & TableBorderGridCellInfo.BottomNeighbour)
                topBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.RightOuter || gridCell.info & TableBorderGridCellInfo.RightInner || gridCell.info & TableBorderGridCellInfo.LeftNeighbour)
                rightBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.BottomOuter || gridCell.info & TableBorderGridCellInfo.BottomInner || gridCell.info & TableBorderGridCellInfo.TopNeighbour)
                bottomBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.LeftOuter || gridCell.info & TableBorderGridCellInfo.LeftInner || gridCell.info & TableBorderGridCellInfo.RightNeighbour)
                leftBorder = true;

            borderInfo = borderInfo || new BorderInfo();
            if(topBorder || rightBorder || bottomBorder || leftBorder) {
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [topBorder ? borderInfo.clone() : undefined, rightBorder ? borderInfo.clone() : undefined, bottomBorder ? borderInfo.clone() : undefined, leftBorder ? borderInfo.clone() : undefined, undefined, undefined],
                    [topBorder, rightBorder, bottomBorder, leftBorder, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellInsideBordersCommand extends ToggleTableCellsBorderCommandBase {
        affectOuter: boolean = false;
        affectNeighbours: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.BottomInner | TableBorderGridCellInfo.LeftInner | TableBorderGridCellInfo.RightInner | TableBorderGridCellInfo.TopInner;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            if(currentCell.info & TableBorderGridCellInfo.TopInner)
                return patternBorder.equals(this.getActualTopBorder(grid, currentCell, topTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.RightInner)
                return patternBorder.equals(this.getActualRightBorder(grid, currentCell, rightTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.BottomInner)
                return patternBorder.equals(this.getActualBottomBorder(grid, currentCell, bottomTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.LeftInner)
                return patternBorder.equals(this.getActualLeftBorder(grid, currentCell, leftTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderHorizontal().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderVertical().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            borderInfo = borderInfo || new BorderInfo();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, [
                undefined, undefined, undefined, undefined, borderInfo.clone(), borderInfo.clone()
            ], [undefined, undefined, undefined, undefined, true, true]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            let topBorder: boolean,
                rightBorder: boolean,
                bottomBorder: boolean,
                leftBorder: boolean;
            if(gridCell.info & TableBorderGridCellInfo.TopInner)
                topBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.RightInner)
                rightBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.BottomInner)
                bottomBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.LeftInner)
                leftBorder = true;
            borderInfo = borderInfo || new BorderInfo();
            if(topBorder || rightBorder || bottomBorder || leftBorder) {
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [topBorder ? borderInfo.clone() : undefined, rightBorder ? borderInfo.clone() : undefined, bottomBorder ? borderInfo.clone() : undefined, leftBorder ? borderInfo.clone() : undefined, undefined, undefined],
                    [topBorder, rightBorder, bottomBorder, leftBorder, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellInsideHorizontalBordersCommand extends ToggleTableCellsBorderCommandBase {
        affectOuter: boolean = false;
        affectNeighbours: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.BottomInner | TableBorderGridCellInfo.TopInner;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            if(currentCell.info & TableBorderGridCellInfo.TopInner)
                return patternBorder.equals(this.getActualTopBorder(grid, currentCell, topTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.BottomInner)
                return patternBorder.equals(this.getActualBottomBorder(grid, currentCell, bottomTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderHorizontal().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            borderInfo = borderInfo || new BorderInfo();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, [
                undefined, undefined, undefined, undefined, borderInfo.clone(), undefined
            ], [undefined, undefined, undefined, undefined, true, undefined]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            let topBorder: boolean,
                bottomBorder: boolean;
            if(gridCell.info & TableBorderGridCellInfo.TopInner)
                topBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.BottomInner)
                bottomBorder = true;
            borderInfo = borderInfo || new BorderInfo();
            if(topBorder || bottomBorder) {
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [topBorder ? borderInfo.clone() : undefined, undefined, bottomBorder ? borderInfo.clone() : undefined, undefined, undefined, undefined],
                    [topBorder, undefined, bottomBorder, undefined, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellInsideVerticalBordersCommand extends ToggleTableCellsBorderCommandBase {
        affectOuter: boolean = false;
        affectNeighbours: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.LeftInner | TableBorderGridCellInfo.RightInner;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            if(currentCell.info & TableBorderGridCellInfo.LeftInner)
                return patternBorder.equals(this.getActualLeftBorder(grid, currentCell, leftTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.RightInner)
                return patternBorder.equals(this.getActualRightBorder(grid, currentCell, rightTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderVertical().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            borderInfo = borderInfo || new BorderInfo();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, [
                undefined, undefined, undefined, undefined, undefined, borderInfo.clone()
            ], [undefined, undefined, undefined, undefined, undefined, true]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            let leftBorder: boolean,
                rightBorder: boolean;
            if(gridCell.info & TableBorderGridCellInfo.LeftInner)
                leftBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.RightInner)
                rightBorder = true;
            borderInfo = borderInfo || new BorderInfo();
            if(rightBorder || leftBorder) {
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [undefined, rightBorder ? borderInfo.clone() : undefined, undefined, leftBorder ? borderInfo.clone() : undefined, undefined, undefined],
                    [undefined, rightBorder, undefined, leftBorder, undefined, undefined]));
            }
        }
    }

    export class ToggleTableCellOutsideBordersCommand extends ToggleTableCellsBorderCommandBase {
        affectInner: boolean = false;
        affectOnStateFlags: TableBorderGridCellInfo = TableBorderGridCellInfo.LeftOuter | TableBorderGridCellInfo.RightOuter | TableBorderGridCellInfo.TopOuter | TableBorderGridCellInfo.BottomOuter;
        protected checkBorderInParticallyTableSelection(patternBorder: BorderInfo, table: Table, grid: TableBorderGridCell[][], currentCell: TableBorderGridCell, rowIndex: number, columnIndex: number, topTableBorder: BorderInfo, rightTableBorder: BorderInfo, bottomTableBorder: BorderInfo, leftTableBorder: BorderInfo): boolean {
            if(currentCell.info & TableBorderGridCellInfo.TopOuter)
                return patternBorder.equals(this.getActualTopBorder(grid, currentCell, topTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.RightOuter)
                return patternBorder.equals(this.getActualRightBorder(grid, currentCell, rightTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.BottomOuter)
                return patternBorder.equals(this.getActualBottomBorder(grid, currentCell, bottomTableBorder, table, rowIndex, columnIndex));
            if(currentCell.info & TableBorderGridCellInfo.LeftOuter)
                return patternBorder.equals(this.getActualLeftBorder(grid, currentCell, leftTableBorder, table, rowIndex, columnIndex));
        }
        protected checkBorderInEntireTable(patternBorder: BorderInfo, table: Table): boolean {
            return patternBorder.equals(new TablePropertiesMergerBorderTop().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderRight().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderBottom().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties)) &&
                patternBorder.equals(new TablePropertiesMergerBorderLeft().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, this.control.model.defaultTableProperties));
        }
        protected applyBorderToTable(subDocument: SubDocument, table: Table, borderInfo: BorderInfo) {
            borderInfo = borderInfo || new BorderInfo();
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, [
                borderInfo.clone(), borderInfo.clone(), borderInfo.clone(), borderInfo.clone(), undefined, undefined
            ], [true, true, true, true, undefined, undefined]));
        }
        protected applyBorderToCell(subDocument: SubDocument, table: Table, gridCell: TableBorderGridCell, rowIndex: number, cellIndex: number, borderInfo: BorderInfo) {
            let topBorder: boolean,
                rightBorder: boolean,
                bottomBorder: boolean,
                leftBorder: boolean;
            if(gridCell.info & TableBorderGridCellInfo.TopOuter || gridCell.info & TableBorderGridCellInfo.BottomNeighbour)
                topBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.RightOuter || gridCell.info & TableBorderGridCellInfo.LeftNeighbour)
                rightBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.BottomOuter || gridCell.info & TableBorderGridCellInfo.TopNeighbour)
                bottomBorder = true;
            if(gridCell.info & TableBorderGridCellInfo.LeftOuter || gridCell.info & TableBorderGridCellInfo.RightNeighbour)
                leftBorder = true;
            borderInfo = borderInfo || new BorderInfo();
            if(topBorder || rightBorder || bottomBorder || leftBorder) {
                this.control.history.addAndRedo(new TableCellBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex,
                    [topBorder ? borderInfo.clone() : undefined, rightBorder ? borderInfo.clone() : undefined, bottomBorder ? borderInfo.clone() : undefined, leftBorder ? borderInfo.clone() : undefined, undefined, undefined],
                    [topBorder, rightBorder, bottomBorder, leftBorder, undefined, undefined]));
            }
        }
    }

    export interface TableBorderGridCell {
        cell: TableCell;
        info: TableBorderGridCellInfo;
        selected: boolean;
    }
    export enum TableBorderGridCellInfo {
        None = 0,
        LeftOuter = 1 << 0,
        RightOuter = 1 << 1,
        BottomOuter = 1 << 2,
        TopOuter = 1 << 3,
        RightNeighbour = 1 << 4,
        TopNeighbour = 1 << 5,
        LeftNeighbour = 1 << 6,
        BottomNeighbour = 1 << 7,
        RightInner = 1 << 8,
        TopInner = 1 << 9,
        LeftInner = 1 << 10,
        BottomInner = 1 << 11,
        LeftTableOuter = 1 << 12,
        RightTableOuter = 1 << 13,
        TopTableOuter = 1 << 14,
        BottomTableOuter = 1 << 15,
        All = ~0
    }
}