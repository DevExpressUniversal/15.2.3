module __aspxRichEdit {
    export class BoxIteratorTablePositionsManipulator {
        private textBoxIterator: TextBoxIterator;

        constructor(textBoxIterator: TextBoxIterator) {
            this.textBoxIterator = textBoxIterator;
        }

        public checkTableLevelsInfo() {
            // what for? Imagine the situation: We have three levels of nesting tables. Format the one of 100 pages. We have written to the iterator that have 3 levels of nesting.
            // We do not format all 100 pages, and we have added 5 page 4 level. That is inserted into the table. Since we have not yet reached 5 pages in the setPosition
            // Iterator will not be called. And we will remain 3 levels of nesting. That is not true.
            if (this.textBoxIterator.subDocument.tablesByLevels.length == this.textBoxIterator.currTableIndexInLevels.length)
                return;

            this.textBoxIterator.currTableIndexInLevels = [];
            BoxIteratorTablePositionsManipulator.calculateLevelIndexes(this.textBoxIterator.tablePositions, this.textBoxIterator.subDocument.tablesByLevels,
                this.textBoxIterator.currTableIndexInLevels, this.textBoxIterator.position);
        }

        public setTableIndexes() {
            const tables: Table[] = this.textBoxIterator.subDocument.tables;
            this.textBoxIterator.currTableIndexInLevels = [];
            if (!tables.length) {
                this.textBoxIterator.tablePositions = [];
                return;
            }
            const tablePositions: TablePosition[] = this.textBoxIterator.tablePositions;
            const tablesByLevels: Table[][] = this.textBoxIterator.subDocument.tablesByLevels;
            const currTableIndexInLevels: number[] = this.textBoxIterator.currTableIndexInLevels;

            const position: number = this.textBoxIterator.position;
            const lowlevelTableIndex: number = Math.max(0, Utils.normedBinaryIndexOf(tables, (t: Table) => t.getStartPosition() - position));
            const lowlevelTable: Table = Table.correctBoundTable(tables, lowlevelTableIndex, position, (index: number) => ++index);

            tablePositions.push.apply(tablePositions, BoxIteratorTablePositionsManipulator.pushParentTableIndexes(lowlevelTable, position, null));
            if (this.textBoxIterator.tablePositions.length) {
                BoxIteratorTablePositionsManipulator.calculateLevelIndexes(tablePositions, tablesByLevels, currTableIndexInLevels, position);
                return;
            }

            // if list not determined, then point in extern all tables
            // check that this not first table
            if (lowlevelTable.getTopLevelParent().index == 0) {
                const lowestTableStartPos: number = lowlevelTable.getStartPosition();
                if (position < lowestTableStartPos) {
                    tablePositions.push.apply(tablePositions,
                        BoxIteratorTablePositionsManipulator.setLastElementAsUnachieved(BoxIteratorTablePositionsManipulator.pushParentTableIndexes(lowlevelTable, lowestTableStartPos, null)));
                    BoxIteratorTablePositionsManipulator.calculateLevelIndexes(tablePositions, tablesByLevels, currTableIndexInLevels, lowestTableStartPos);
                    return;
                }
            }

            // if not first then it worked
            const nextTableIndex: number = lowlevelTable.index + 1;
            if (nextTableIndex >= tables.length) {
                this.textBoxIterator.tablePositions = [];
                return;
            }

            const nextLowLevelTable: Table = Table.correctBoundTable(tables, nextTableIndex, position, (index: number) => ++index);
            const nextLowLevelTableStartPos: number = nextLowLevelTable.getStartPosition();
            tablePositions.push.apply(tablePositions, BoxIteratorTablePositionsManipulator.setLastElementAsUnachieved(
                BoxIteratorTablePositionsManipulator.pushParentTableIndexes(nextLowLevelTable, nextLowLevelTableStartPos, null)));
            BoxIteratorTablePositionsManipulator.calculateLevelIndexes(tablePositions, tablesByLevels, currTableIndexInLevels, nextLowLevelTableStartPos);
        }

        public advanceTableIndexes(box: LayoutBox) {
            if (!this.textBoxIterator.tablePositions.length ||
                box.getType() != LayoutBoxType.ParagraphMark && box.getType() != LayoutBoxType.SectionMark)
                return;
            this.advanceTableIndexesInternal();
        }

        private advanceTableIndexesInternal() {
            const tablePositions: TablePosition[] = this.textBoxIterator.tablePositions;
            const position: number = this.textBoxIterator.position;
            const tables: Table[] = this.textBoxIterator.subDocument.tables;
            const lastTablePosition: TablePosition = tablePositions[tablePositions.length - 1];

            if (!lastTablePosition.cell) {
                if (position >= lastTablePosition.row.getStartPosition())
                    lastTablePosition.cell = lastTablePosition.row.cells[0];
                return;
            }

            BoxIteratorTablePositionsManipulator.processPositionAdvance(tables, tablePositions, position, this.textBoxIterator.subDocument.tablesByLevels, this.textBoxIterator.currTableIndexInLevels);
        }

        private static processPositionAdvance(tables: Table[], tablePositions: TablePosition[], position: number, tablesByLevels: Table[][], currTableIndexInLevels: number[]) {
            const lastTablePosition: TablePosition = tablePositions[tablePositions.length - 1];
            if (position >= lastTablePosition.cell.endParagrapPosition.value)
                BoxIteratorTablePositionsManipulator.toNextCell(tables, tablePositions, position, tablesByLevels, currTableIndexInLevels);
            else // in this cell
                BoxIteratorTablePositionsManipulator.addLowLevelTables(tables, tablePositions, tablesByLevels, currTableIndexInLevels, position);
        }

        private static toNextCell(tables: Table[], tablePositions: TablePosition[], position: number, tablesByLevels: Table[][], currTableIndexInLevels: number[]) {
            let currTablePositionIndex: number = tablePositions.length - 1;
            let currTablePosition: TablePosition = tablePositions[currTablePositionIndex];

            currTablePosition.cellIndex++;
            if (currTablePosition.cellIndex < currTablePosition.row.cells.length) {
                currTablePosition.cell = currTablePosition.row.cells[currTablePosition.cellIndex];
                BoxIteratorTablePositionsManipulator.addLowLevelTables(tables, tablePositions, tablesByLevels, currTableIndexInLevels, currTablePosition.cell.startParagraphPosition.value);
            }
            else {
                currTablePosition.rowIndex++;
                if (currTablePosition.rowIndex < currTablePosition.table.rows.length) {
                    currTablePosition.cellIndex = 0;
                    currTablePosition.init();
                    BoxIteratorTablePositionsManipulator.addLowLevelTables(tables, tablePositions, tablesByLevels, currTableIndexInLevels, currTablePosition.cell.startParagraphPosition.value);
                }
                else {
                    const lastTablePosition: TablePosition = tablePositions.pop();
                    if (tablePositions.length == 0) {
                        // to next level
                        // table1.getEndPosition() == table2.getStartPosition() - cant be this.
                        const nextTableIndex: number = lastTablePosition.table.index + 1;
                        const lowlevelTable: Table = Table.correctBoundTable(tables, nextTableIndex, position, (index: number) => ++index);
                        currTableIndexInLevels[lastTablePosition.table.nestedLevel]++;
                        tablePositions.push.apply(tablePositions, BoxIteratorTablePositionsManipulator.setLastElementAsUnachieved(
                            BoxIteratorTablePositionsManipulator.pushParentTableIndexes(lowlevelTable, lowlevelTable.getStartPosition(), null)));
                    }
                    else
                        BoxIteratorTablePositionsManipulator.processPositionAdvance(tables, tablePositions, position, tablesByLevels, currTableIndexInLevels); // can be recursion
                }
            }
        }

        // get last pos in list and if in this pos other tables lowest level just add it
        private static addLowLevelTables(tables: Table[], tablePositions: TablePosition[], tablesByLevels: Table[][], currTableIndexInLevels: number[], position: number) {
            const lastTablePosition: TablePosition = tablePositions[tablePositions.length - 1];
            const lastTablePositionTable: Table = lastTablePosition.table;
            const underNestedLevelIndex: number = lastTablePositionTable.nestedLevel + 1;
            if (underNestedLevelIndex < tablesByLevels.length) {
                const nextTableInLevelUnderIndex: number = currTableIndexInLevels[underNestedLevelIndex];
                const nextTableInLevelUnder: Table = tablesByLevels[underNestedLevelIndex][nextTableInLevelUnderIndex];
                if (nextTableInLevelUnder && nextTableInLevelUnder.getStartPosition() == position) {
                    const lowestLevelTable: Table = Table.correctBoundTable(tables, nextTableInLevelUnder.index, position, (index: number) => ++index);
                    const underLevelPositions: TablePosition[] = BoxIteratorTablePositionsManipulator.pushParentTableIndexes(lowestLevelTable, position, lastTablePositionTable);
                    for (let tblPos of underLevelPositions)
                        currTableIndexInLevels[tblPos.table.nestedLevel]++;
                    tablePositions.push.apply(tablePositions, underLevelPositions);
                }
            }
        }

        private static pushParentTableIndexes(lowestLevelTable: Table, position: number, notIncludedTopLevelTable: Table): TablePosition[] {
            const resultTablePositions: TablePosition[] = [];
            for (let currTable: Table = lowestLevelTable; currTable && (currTable != notIncludedTopLevelTable); currTable = currTable.getParentTable())
                if (position >= currTable.getStartPosition() && position < currTable.getEndPosition())
                    resultTablePositions.unshift(BoxIteratorTablePositionsManipulator.determineFullIndex(currTable, position));
            return resultTablePositions;
        }

        private static determineFullIndex(table: Table, position: number): TablePosition {
            const rowIndex: number = Math.max(0, Utils.normedBinaryIndexOf(table.rows, (r: TableRow) => r.getStartPosition() - position));
            const row: TableRow = table.rows[rowIndex];
            const cellIndex: number = Math.max(0, Utils.normedBinaryIndexOf(row.cells, (c: TableCell) => c.startParagraphPosition.value - position));
            return new TablePosition(table, rowIndex, cellIndex).init();
        }

        // set flag that table not reached
        private static setLastElementAsUnachieved(tablePositions: TablePosition[]): TablePosition[] {
            const last: TablePosition = tablePositions[tablePositions.length - 1];
            last.cell = null;
            return tablePositions;
        }

        private static calculateLevelIndexes(tablePositions: TablePosition[], tablesByLevels: Table[][], currTableIndexInLevels: number[], position: number) {
            for (let tblPos of tablePositions) {
                const tablesOnLevel: Table[] = tablesByLevels[tblPos.table.nestedLevel];
                var tblStartPos: number = tblPos.table.getStartPosition();
                currTableIndexInLevels.push(Utils.normedBinaryIndexOf(tablesOnLevel, (t: Table) => t.getStartPosition() - tblStartPos) + 1);
            }

            for (let levelIndex: number = tablePositions[tablePositions.length - 1].table.nestedLevel + 1, tablesoOnLevel: Table[];
                tablesoOnLevel = tablesByLevels[levelIndex]; levelIndex++)
                currTableIndexInLevels.push(Utils.normedBinaryIndexOf(tablesoOnLevel, (t: Table) => t.getStartPosition() - position) + 1);
        }
    }
}