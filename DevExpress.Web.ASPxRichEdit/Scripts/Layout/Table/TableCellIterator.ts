module __aspxRichEdit {
    export class ParentLinkTableCell {
        public parentCell: LayoutTableCellInfo = null;
        public parentLink: ParentLinkTableCell = null;
        public layoutRowIndexInParentCell: number = -1;

        public createParentLink(): ParentLinkTableCell {
            return this.createParentLinkInternal(this);
        }

        private createParentLinkInternal(link: ParentLinkTableCell): ParentLinkTableCell {
            if (link.parentLink)
                return this.createParentLinkInternal(link.parentLink);
            var newLink = new ParentLinkTableCell();
            link.parentLink = newLink;
            return newLink
        }
    }

    export class TableCellIterator {
        private position: LayoutPosition;
        private layout: DocumentLayout
        private measurer: IBoxMeasurer;

        private tableIndex: number = -1
        private tableInfo: LayoutTableColumnInfo;

        private tableRowIndex: number = -1;
        private tableRowInfo: LayoutTableRowInfo;

        private tableCellIndex: number = -1;
        private tableCellInfo: LayoutTableCellInfo;

        private layoutRowIndexInCell: number = -1;
        private layoutRowIndexInLayout: number = -1;
        private layoutRowIndexInLayoutInitial = -1;

        private pageIndex: number = 0;
        private pageAreaIndex: number = 0;
        private columnIndex: number = 0;

        private isDown: boolean;
        private parentLink: ParentLinkTableCell;

        constructor(position: LayoutPosition, layout: DocumentLayout, measurer: IBoxMeasurer) {
            this.position = position;
            this.layout = layout;
            this.measurer = measurer;

            this.setPositionProperties();
            if (this.readTables(this.position.column.tablesInfo)) {
                for (let tableIndex: number = 0, table: LayoutTableColumnInfo; table = this.position.column.tablesInfo[tableIndex]; tableIndex++)
                    if (table == this.tableInfo) {
                        this.tableIndex = tableIndex;
                        break;
                    }
            }
        }

        private readTables(tables: LayoutTableColumnInfo[], parentCell: LayoutTableCellInfo = null, layountIndex: number = -1): boolean {
            for (let tableIndex: number = 0, table: LayoutTableColumnInfo; table = tables[tableIndex]; tableIndex++) {
                for (let rowIndex: number = 0, row: LayoutTableRowInfo; row = table.tableRows[rowIndex]; rowIndex++) {
                    for (let cellIndex: number = 0, cell: LayoutTableCellInfo; cell = table.tableRows[rowIndex].rowCells[cellIndex]; cellIndex++) {

                        for (var layountRowIndex in cell.internalTables) {
                            if (this.readTables(this.getConverttedObjectToArray(cell.internalTables), cell, parseInt(layountRowIndex))) {
                                var link = this.parentLink.createParentLink();
                                link.parentCell = parentCell;
                                link.layoutRowIndexInParentCell = layountIndex;
                                return true;
                            }
                        }

                        var index = Utils.binaryIndexOf(cell.layoutRows, (row: LayoutRowWithIndex) => row.indexInColumn - this.position.rowIndex);
                        if (index >= 0) {
                            this.tableInfo = table;

                            this.tableRowIndex = rowIndex;
                            this.tableRowInfo = row;

                            this.tableCellIndex = cellIndex;
                            this.tableCellInfo = cell;

                            this.layoutRowIndexInCell = index;
                            this.layoutRowIndexInLayout = cell.layoutRows[index].indexInColumn;
                            this.layoutRowIndexInLayoutInitial = this.layoutRowIndexInLayout;

                            this.parentLink = new ParentLinkTableCell();
                            this.parentLink.parentCell = parentCell;
                            this.parentLink.layoutRowIndexInParentCell = layountIndex;

                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private getConverttedObjectToArray(obj: { [rowIndex: number]: LayoutTableColumnInfo }): LayoutTableColumnInfo[] { //TODO rename
            var tables: LayoutTableColumnInfo[] = [];
            for (var index in obj)
                tables.push(obj[index]);
            return tables;
        }
        private setPositionProperties() {
            this.pageIndex = this.position.pageIndex;
            this.pageAreaIndex = this.position.pageAreaIndex;
            this.columnIndex = this.position.columnIndex;
        }

        isMainSubDocument(): boolean {
            return this.position.pageArea.subDocument.isMain();
        }
        //private readTable(): boolean {
        //    this.tableIndex++;
        //    this.tableInfo = this.position.column.tablesInfo[this.tableIndex];
        //    this.tableRowIndex = -1;
        //    return !!this.tableInfo;
        //}
        //private readTableRow(): boolean {
        //    this.tableRowIndex++;
        //    this.tableRowInfo = this.tableInfo.tableRows[this.tableRowIndex];
        //    this.tableCellIndex = -1;
        //    return !!this.tableRowInfo;
        //}

        //private readTableCell(): boolean {
        //    this.tableCellIndex++;
        //    this.tableCellInfo = this.tableRowInfo.rowCells[this.tableCellIndex];
        //    return !!this.tableCellInfo;
        //}

        //public advanceToRight(): boolean {
        //    this.tableCellIndex++
        //    if (this.tableCellIndex > this.tableRowInfo.rowCells.length) {
        //        this.tableCellIndex = 0;
        //        this.tableRowIndex++;
        //        if (this.tableRowIndex > this.tableInfo.tableRows.length) {
        //            this.tableRowIndex = 0;
        //            this.tableIndex = 0;
        //            this.columnIndex++;
        //            if (this.columnIndex > this.position.pageArea.columns.length) {
        //                this.columnIndex = 0;
        //                this.pageIndex++;
        //                if (this.pageIndex > this.layout.validPageCount)
        //                    return false;
        //            }
        //        }
        //    }

        //    var tables = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[0].columns[this.columnIndex].tablesInfo;
        //    if (tables.length > 0 && tables[this.tableIndex].logicInfo.grid == this.tableInfo.logicInfo.grid) {
        //        this.tableInfo = tables[this.tableIndex];
        //        this.tableRowInfo = this.tableInfo.tableRows[this.tableRowIndex];
        //        this.tableCellInfo = this.tableRowInfo.rowCells[this.tableCellIndex];
        //        return true;
        //    }
        //    return false;
        //}
        //public advanceToLeft(): boolean {
        //    var reCalculateCellIndex = false;
        //    var reCalculateRowIndex = false;
        //    var reCalculateColumnIndex = false;

        //    this.tableCellIndex--;
        //    if (this.tableCellIndex < 0) {
        //        reCalculateCellIndex = true;
        //        this.tableRowIndex--;
        //        if (this.tableRowIndex < 0) {
        //            reCalculateRowIndex = true;
        //            this.columnIndex--;
        //            if (this.columnIndex < 0) {
        //                reCalculateColumnIndex = true;
        //                this.pageIndex--;
        //                if (this.pageIndex < 0)
        //                    return false;
        //            }
        //        }
        //    }
        //    if(reCalculateColumnIndex)
        //        this.columnIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[0].columns.length - 1;
        //    if (reCalculateRowIndex) {
        //        this.tableIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[0].columns[this.columnIndex].tablesInfo.length - 1;
        //        this.tableRowIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[0].columns[this.columnIndex].tablesInfo[this.tableIndex].tableRows.length - 1;
        //    }
        //    if (reCalculateCellIndex) //TODO rafact
        //        this.tableCellIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[0].columns[this.columnIndex].tablesInfo[this.tableIndex].tableRows[this.tableRowIndex].rowCells.length - 1;

        //    var tables = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[0].columns[this.columnIndex].tablesInfo; //TODO nedd refactoring
        //    if (tables.length > 0 && tables[this.tableIndex].logicInfo.grid == this.tableInfo.logicInfo.grid) {
        //        var oldTableRowInfo = this.tableRowInfo;
        //        var oldTableCellIndex = this.tableCellIndex;

        //        this.tableInfo = tables[this.tableIndex];
        //        this.tableRowInfo = this.tableInfo.tableRows[this.tableRowIndex];
        //        this.tableCellIndex = this.findNextCellIndex(oldTableRowInfo, oldTableCellIndex, this.tableRowInfo);
        //        this.tableCellInfo = this.tableRowInfo.rowCells[this.tableCellIndex];

        //        return true;
        //    }
        //    return true;
        //}

        public getModifyPosition(): LayoutPosition {
            var newPosition = this.position.clone();

            if (this.isMainSubDocument()) {
                newPosition.pageIndex = this.pageIndex;
                newPosition.pageAreaIndex = this.pageAreaIndex;
                newPosition.columnIndex = this.columnIndex;
                newPosition.page = this.layout.pages[newPosition.pageIndex];
                newPosition.pageArea = newPosition.page.mainSubDocumentPageAreas[newPosition.pageAreaIndex];
                newPosition.column = newPosition.pageArea.columns[newPosition.columnIndex];
            }
            newPosition.rowIndex = this.layoutRowIndexInLayout;
            newPosition.row = newPosition.column.rows[newPosition.rowIndex];

            return newPosition;
        }

        public tryAdvanceToBelowRow(): boolean {
            if (!this.canAdvanceToBelowRow())
                return false;
            this.isDown = true;

            this.tableRowIndex++
            if (this.tableRowIndex >= this.tableInfo.tableRows.length) {

                this.tableRowIndex = 0;
                this.tableIndex = 0;

                if (!this.isMainSubDocument())
                    return this.tryAdvanceToLayoutRowBelowTable();

                this.columnIndex++;
                if (this.columnIndex >= this.position.pageArea.columns.length) {
                    this.columnIndex = 0;

                    this.pageAreaIndex++;
                    if (this.pageAreaIndex >= this.position.page.mainSubDocumentPageAreas.length) {
                        this.pageAreaIndex = 0;

                        this.pageIndex++;
                        if (this.pageIndex >= this.layout.validPageCount)
                            return this.tryAdvanceToLayoutRowBelowTable();
                    }
                }
            }

            var tables = this.getTables();
            if (tables.length > 0 && this.isEqualTables(tables[this.tableIndex], this.tableInfo)) {
                this.tableInfo = tables[this.tableIndex];
                this.tableRowInfo = this.tableInfo.tableRows[this.tableRowIndex];
                this.tableCellIndex = this.findNextCellIndex(this.tableRowInfo);
                this.tableCellInfo = this.tableRowInfo.rowCells[this.tableCellIndex];
                this.layoutRowIndexInLayout = this.tableCellInfo.layoutRows[0].indexInColumn;

                this.goToInternalTableIfExistInFirstBelowRow();
                return true;
            }

            return this.tryAdvanceToLayoutRowBelowTable();
        }
        public tryAdvanceToAboveRow(): boolean {
            var rowIndexReset = false,
                columnIndexReset = false,
                pageAreaIndexReset = false;

            if (!this.canAdvanceToAboveRow())
                return false;
            this.isDown = false;

            if (this.tableCellInfo.layoutRows.length == 1 && this.tableCellInfo.internalTables[this.layoutRowIndexInCell]) {
                if (this.goToInternalTableIfExistInLastAboveRow())
                    return true;
            }

            var tables = [];
            this.tableRowIndex--
            if (this.tableRowIndex < 0) {

                if (!this.isMainSubDocument())
                    return this.tryAdvanceToLayoutRowAboveTable();

                rowIndexReset = true;
                this.columnIndex--;
                if (this.columnIndex < 0) {

                    columnIndexReset = true;
                    this.pageAreaIndex--;
                    if (this.pageAreaIndex < 0) {

                        pageAreaIndexReset = true;
                        this.pageIndex--;
                        if (this.pageIndex < 0)
                            return this.tryAdvanceToLayoutRowAboveTable();
                    }
                }

                if (pageAreaIndexReset)
                    this.pageAreaIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas.length - 1;
                if (columnIndexReset)
                    this.columnIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[this.pageAreaIndex].columns.length - 1;
                if (rowIndexReset) {
                    tables = this.getTables();
                    if (tables.length == 0) return false; //TODO

                    this.tableIndex = tables.length - 1;
                    this.tableRowIndex = tables[this.tableIndex].tableRows.length - 1;
                }
            }

            tables = this.getTables();
            if (tables.length > 0 && this.isEqualTables(tables[this.tableIndex], this.tableInfo)) {
                this.tableInfo = tables[this.tableIndex];
                this.tableRowInfo = this.tableInfo.tableRows[this.tableRowIndex];
                this.tableCellIndex = this.findNextCellIndex(this.tableRowInfo);
                //
                this.tableRowInfo = this.tableInfo.tableRows[this.tableRowIndex]; //TODO
                //
                this.tableCellInfo = this.tableRowInfo.rowCells[this.tableCellIndex];
                this.layoutRowIndexInLayout = this.tableCellInfo.layoutRows[this.tableCellInfo.layoutRows.length - 1].indexInColumn;
                this.goToInternalTableIfExistInLastAboveRow();
                return true;
            }
            return this.tryAdvanceToLayoutRowAboveTable();
        }

        private tryAdvanceToLayoutRowBelowTable(): boolean {
            this.setPositionProperties();

            var row = this.tableInfo.getLastTableRow().getLastRowCell().getLastLayoutRow();
            var column = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[this.pageAreaIndex].columns[this.columnIndex];
            this.layoutRowIndexInLayout = row.indexInColumn + 1;

            if (this.layoutRowIndexInLayout >= column.rows.length) {
                this.columnIndex++;
                if (this.columnIndex >= this.position.pageArea.columns.length) {
                    this.columnIndex = 0;

                    this.pageAreaIndex++;
                    if (this.pageAreaIndex >= this.position.page.mainSubDocumentPageAreas.length) {
                        this.pageAreaIndex = 0;

                        this.pageIndex++;
                        if (this.pageIndex >= this.layout.validPageCount) {
                            this.setPositionProperties();
                            this.resetLayoutRowIndex();
                            return true;
                        }
                    }
                }
                this.layoutRowIndexInLayout = 0;
            }
            return true;
        }

        private isAboveLayoutRowOuterTable(): boolean {
            return this.parentLink && this.parentLink.parentCell && this.parentLink.layoutRowIndexInParentCell == 0;
        }
        private tryAdvanceToLayoutRowOuterTable(parentLink: ParentLinkTableCell): boolean {
            var parentRow = parentLink.parentCell.parentRow;
            var parentTable = parentRow.parentTable;

            if (parentRow.rowIndex == 0 && parentLink.parentLink.parentCell)
                return this.tryAdvanceToLayoutRowOuterTable(parentLink.parentLink);

            if (parentRow.rowIndex > 0) {
                var index = this.findNextCellIndex(parentTable.tableRows[parentRow.rowIndex - 1], true); //TODO
                this.layoutRowIndexInLayout = parentTable.tableRows[parentRow.rowIndex - 1].rowCells[index].getLastLayoutRow().indexInColumn;
                return true;
            }
            return false;
        }

        private tryAdvanceToLayoutRowAboveTable(): boolean {
            this.setPositionProperties();

            if (this.isAboveLayoutRowOuterTable())
                return this.tryAdvanceToLayoutRowOuterTable(this.parentLink);

            var columnIndexReset = false, pageAreaIndexReset = false;
            var row = this.tableInfo.tableRows[0].rowCells[0].layoutRows[0];
            this.layoutRowIndexInLayout = row.indexInColumn - 1;

            if (this.layoutRowIndexInLayout < 0) {
                this.columnIndex--;
                if (this.columnIndex < 0) {

                    columnIndexReset = true;
                    this.pageAreaIndex--;
                    if (this.pageAreaIndex < 0) {

                        pageAreaIndexReset = true;
                        this.pageIndex--;
                        if (this.pageIndex < 0) {
                            this.setPositionProperties();
                            this.resetLayoutRowIndex();
                            return true;
                        }
                    }
                }
                if (pageAreaIndexReset)
                    this.pageAreaIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas.length - 1;
                if (columnIndexReset)
                    this.columnIndex = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[this.pageAreaIndex].columns.length - 1;

                var rows = this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[this.pageAreaIndex].columns[this.columnIndex].rows;
                this.layoutRowIndexInLayout = rows.length - 1;
            }
            return true;
        }

        private goToInternalTableIfExistInFirstBelowRow(): boolean {
            if (this.tableCellInfo.internalTables && this.tableCellInfo.internalTables[0]) {
                var internalTable = this.tableCellInfo.internalTables[0];
                this.tableCellInfo = internalTable.tableRows[0].rowCells[this.findNextCellIndex(internalTable.tableRows[0])];

                if (this.goToInternalTableIfExistInFirstBelowRow())
                    return true;

                this.layoutRowIndexInLayout = this.tableCellInfo.layoutRows[0].indexInColumn;
                return true;
            }
            return false;
        }
        private goToInternalTableIfExistInLastAboveRow(): boolean {
            var lastLayoutRowIndexInInternalTable = this.tableCellInfo.layoutRows.length - 1;
            if (this.tableCellInfo.internalTables && this.tableCellInfo.internalTables[lastLayoutRowIndexInInternalTable]) {
                var internalTable = this.tableCellInfo.internalTables[lastLayoutRowIndexInInternalTable];
                var lastTableRowIndex = internalTable.tableRows.length - 1;
                var cellIndexInInternalTable = this.findNextCellIndex(internalTable.tableRows[lastTableRowIndex]);
                var lastLayoutRowIndex = internalTable.tableRows[lastTableRowIndex].rowCells[cellIndexInInternalTable].layoutRows.length - 1;

                this.layoutRowIndexInLayout = internalTable.tableRows[lastTableRowIndex].rowCells[cellIndexInInternalTable].layoutRows[lastLayoutRowIndex].indexInColumn;
                return true;
            }
            return false;
        }

        private isInTable(): boolean {
            return this.layoutRowIndexInCell > -1;
        }
        private canAdvanceToBelowRow(): boolean {
            return this.isInTable() && this.layoutRowIndexInCell == this.tableCellInfo.layoutRows.length - 1;
        }
        private canAdvanceToAboveRow(): boolean {
            return this.isInTable() && this.layoutRowIndexInCell == 0;
        }

        private resetLayoutRowIndex() {
            this.layoutRowIndexInLayout = this.layoutRowIndexInLayoutInitial;
        }
        private isEqualTables(table1: LayoutTableColumnInfo, table2: LayoutTableColumnInfo): boolean {
            return table1.logicInfo.grid == table2.logicInfo.grid;
        }
        private getTables(): LayoutTableColumnInfo[]{
            if (this.isMainSubDocument())
                return this.layout.pages[this.pageIndex].mainSubDocumentPageAreas[this.pageAreaIndex].columns[this.columnIndex].tablesInfo;
            return this.position.column.tablesInfo;
        }
        private findNextCellIndex(newRow: LayoutTableRowInfo, skipSeachCellInAllTable: Boolean = false): number {
            var x = this.position.box.getCharOffsetXInPixels(this.measurer, this.position.charOffset) + this.position.box.x + this.position.row.x;

            if (!this.isDown && !skipSeachCellInAllTable) { //TODO
                for (let tableRowInfoIndex = this.tableRowIndex; tableRowInfoIndex >= 0; tableRowInfoIndex--) { //TODO copy from HitTestManager
                    const tableRowInfo: LayoutTableRowInfo = this.tableInfo.tableRows[tableRowInfoIndex];
                    const tableCellInfoIndex: number = Math.max(0, Utils.normedBinaryIndexOf(tableRowInfo.rowCells, (c: LayoutTableCellInfo) => c.bound.x - x));
                    var tableCellInfo = tableRowInfo.rowCells[tableCellInfoIndex];

                    if (x >= tableCellInfo.bound.x && x <= (tableCellInfo.bound.x + tableCellInfo.bound.width)) {
                        this.tableRowIndex = tableRowInfoIndex;
                        return tableCellInfoIndex;
                    }
                }
            }
            
            for (var i = 0; i < newRow.rowCells.length; i++) { //TODO refact
                var cell = newRow.rowCells[i];
                if (x >= cell.bound.x && x <= (cell.bound.x + cell.bound.width))
                    return i;
            }
            return 0;
        }
    }
}