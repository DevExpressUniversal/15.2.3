//module __aspxRichEdit {
//    // iterate by cell paragraphs.
//    export enum NestedTableIteratorResult {
//        LevelNoChange,
//        LevelDown,
//        LevelUp,
//        NextCell,
//        NextRow,
//        End
//    }

//    export class NestedTableIterator {
//        private result: NestedTableIteratorResult;
//        subDocument: SubDocument;
//        currParagraphIndex: number;
//        currParagraph: Paragraph;
//        private needAdvanceParagraph: boolean;
//        localLevelIndex: number;
//        tablePositions: TablePosition[]; // tablePositions[i] parent cell for tablePositions[i + 1]
//        currTablePosition: TablePosition;
//        tableIndexesInLevels: number[];

//        constructor(subDocument: SubDocument, firstParagraphIndex: number, topLevelTablePosition: TablePosition) {
//            this.subDocument = subDocument;
//            this.localLevelIndex = 0;
//            this.currParagraphIndex = firstParagraphIndex;
//            this.currParagraph = subDocument.paragraphs[firstParagraphIndex];
//            this.currTablePosition = topLevelTablePosition.clone();
//            this.tablePositions = [this.currTablePosition];
//            this.init();
//            this.needAdvanceParagraph = false;
//        }

//        private init() {
//            const startPos: number = this.currTablePosition.cell.startParagraphPosition.value;
//            for (let level: number = this.currTablePosition.table.nestedLevel + 1; level < this.subDocument.tablesByLevels.length; level++) {
//                const tablesOnLevel: Table[] = this.subDocument.tablesByLevels[level];
//                const index: number = Math.max(0, Utils.normedBinaryIndexOf(tablesOnLevel, (t: Table) => t.getStartPosition() - startPos));
//                const table: Table = tablesOnLevel[index];
//                this.tableIndexesInLevels.push(level);
//                if (table.getStartPosition() >= startPos && startPos < table.getEndPosition())
//                    this.tablePositions.push(new TablePosition(table, 0, 0).init());
//                else
//                    break;
//            }
//        }

//        public moveNext(): NestedTableIteratorResult {
//            if (this.needAdvanceParagraph) {
//                this.currParagraphIndex++;
//                this.currParagraph = this.subDocument.paragraphs[this.currParagraphIndex];
//                this.needAdvanceParagraph = false;
//            }

//            if (this.levelDown())
//                return this.result;

//            if (this.levelUp())
//                return this.result;

//            this.simpleParagraph();
//            return this.result;
//        }

//        public skipCell() {
//            if (this.currTablePosition.moveToNextCell())
//                return;
//            if (this.currTablePosition.moveToNextRow()) {
//                this.currTablePosition.cellIndex = 0;
//                this.currTablePosition.cell = this.currTablePosition.row.cells[0];
//                return;
//            }
//            this.levelUp();
//        }

//        private levelDown(): boolean {
//            const lowLevelTablePosition: TablePosition = this.tablePositions[this.localLevelIndex + 1];
//            if (!lowLevelTablePosition || this.currParagraph.startLogPosition.value != lowLevelTablePosition.table.getStartPosition())
//                return false;

//            // need go level below
//            this.localLevelIndex++;
//            this.currTablePosition = lowLevelTablePosition;
//            this.result = NestedTableIteratorResult.LevelDown;
//            return true;
//        }

//        private levelUp(): boolean {
//            if (this.currParagraph.startLogPosition.value != this.currTablePosition.table.getEndPosition())
//                return false;

//            if (this.localLevelIndex == 0) {
//                this.result = NestedTableIteratorResult.End;
//                return true;
//            }

//            this.moveLevelTablePositionToNextTable();
            
//            this.localLevelIndex--;
//            this.currTablePosition = this.tablePositions[this.localLevelIndex];

//            this.result = NestedTableIteratorResult.LevelUp;
//            return true;
//        }

//        private simpleParagraph() {
//            if (this.currTablePosition.cell.endParagrapPosition.value == this.currParagraph.startLogPosition.value) {
//                if (this.localLevelIndex == 0)
//                    return this.result = NestedTableIteratorResult.End;

//                if (this.currTablePosition.moveToNextCell())
//                    return this.result = NestedTableIteratorResult.NextCell;

//                if (this.currTablePosition.moveToNextRow()) {
//                    this.currTablePosition.cellIndex = 0;
//                    this.currTablePosition.cell = this.currTablePosition.row.cells[0];
//                    this.result = NestedTableIteratorResult.NextRow;
//                    return;
//                }
//                throw new Error(Errors.NotImplemented);
//            }
//            this.result = NestedTableIteratorResult.LevelNoChange;
//            this.needAdvanceParagraph = true;
//        }

//        private moveLevelTablePositionToNextTable() {
//            const nextTableIndexOnThislevel: number = ++this.tableIndexesInLevels[this.localLevelIndex];
//            const tablesOnThisLevel: Table[] = this.subDocument.tablesByLevels[this.localLevelIndex + this.tablePositions[0].table.nestedLevel];
//            if (nextTableIndexOnThislevel < tablesOnThisLevel.length)
//                this.tablePositions[this.localLevelIndex] = new TablePosition(tablesOnThisLevel[nextTableIndexOnThislevel], 0, 0).init();
//            else
//                this.tablePositions.pop();
//        }
//    }
//}
