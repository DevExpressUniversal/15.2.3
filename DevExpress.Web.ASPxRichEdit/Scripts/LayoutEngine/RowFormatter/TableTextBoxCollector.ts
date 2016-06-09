module __aspxRichEdit {
    export enum TableTextBoxCollectorCellInfoType {
        Normal,
        Hidden, // here need set default box.
        Merged // merged with cell left ot top. not need render
    }

    export class TableTextBoxCollectorTableInfo {
        tableIndex: number;
        cells: TableTextBoxCollectorCellInfo[][] = []; // rowInd. cellInd. as modes structure

        constructor(tableIndex: number) {
            this.tableIndex = tableIndex;
        }
    }

    export class TableTextBoxCollectorCellInfo {
        cellType: TableTextBoxCollectorCellInfoType;
        paragraphsInfo: TableTextBoxCollectorCellParagraphInfo[]; // undefined alwas, except TableTextBoxIteratorCellBoxesType.Normal
    }

    export class TableTextBoxCollectorCellParagraphInfo {
        parIndex: number;
        boxes: LayoutBox[] = [];

        constructor(parIndex: number, box: LayoutBox) {
            this.parIndex = parIndex;
            this.boxes = [box];
        }

        public static addBox(infos: TableTextBoxCollectorCellParagraphInfo[], parIndex: number, box: LayoutBox) {
            const lastElem: TableTextBoxCollectorCellParagraphInfo = infos[infos.length - 1];
            if (lastElem && lastElem.parIndex == parIndex)
                lastElem.boxes.push(box);
            else
                infos.push(new TableTextBoxCollectorCellParagraphInfo(parIndex, box));
        }
    }

    export class TableTextBoxCollector {
        public tableInfos: TableTextBoxCollectorTableInfo[] = [];

        private tables: Table[];
        private textBoxIterator: ITextBoxIterator;

        private nextTablePosition: number; // when be in this pos, then go in next table
        private nextTableIndex: number;
        private currLayoutBox: LayoutBox;

        // set only tables absolutely TOP level!
        constructor(tables: Table[], tableIndex: number, textBoxIterator: ITextBoxIterator) {
            this.textBoxIterator = textBoxIterator;
            this.tables = tables;

            const currTablePosition: TablePosition = new TablePosition(this.tables[tableIndex], -1, -1);
            this.setNextTablePosition(tableIndex + 1);

            this.textBoxIterator.setPosition(currTablePosition.table.getStartPosition(), false);
            this.currLayoutBox = this.textBoxIterator.getNextBox();
            this.calcTableBoxes(currTablePosition);
        }

        private calcTableBoxes(currTablePosition: TablePosition) {
            const paragraphs: Paragraph[] = this.textBoxIterator.subDocument.paragraphs;
            const tableIndex: number = currTablePosition.table.index;
            const tableInfo: TableTextBoxCollectorTableInfo = new TableTextBoxCollectorTableInfo(tableIndex);
            this.tableInfos.push(tableInfo);

            while (currTablePosition.moveToNextRow()) {
                const currRowInfo: TableTextBoxCollectorCellInfo[] = [];
                tableInfo.cells.push(currRowInfo);
                currTablePosition.cellIndex = -1;
                let resetIteratorFromStartCell: boolean = false;
                while (currTablePosition.moveToNextCell()) {
                    if (resetIteratorFromStartCell) {
                        this.textBoxIterator.setPosition(currTablePosition.cell.startParagraphPosition.value, false);
                        this.currLayoutBox = this.textBoxIterator.getNextBox();
                        resetIteratorFromStartCell = false;
                    }
                    const info: TableTextBoxCollectorCellInfo = new TableTextBoxCollectorCellInfo();
                    currRowInfo.push(info);
                    if (currTablePosition.cell.verticalMerging == TableCellMergingState.Continue) {
                        info.cellType = TableTextBoxCollectorCellInfoType.Merged;
                        this.currLayoutBox = this.textBoxIterator.getNextBox(); // invisibe paragraph mark.
                        continue;
                    }
                    
                    // set default box here
                    if (this.currLayoutBox.rowOffset >= currTablePosition.cell.endParagrapPosition.value) {
                        info.cellType = TableTextBoxCollectorCellInfoType.Hidden;
                        resetIteratorFromStartCell = true;
                        continue;
                    }

                    info.cellType = TableTextBoxCollectorCellInfoType.Normal;
                    info.paragraphsInfo = [];
                    const parInfo: TableTextBoxCollectorCellParagraphInfo[] = info.paragraphsInfo;
                    do {
                        if (this.currLayoutBox.rowOffset >= this.nextTablePosition) { // to to nested table
                            this.setNextTablePosition(this.nextTableIndex + 1);
                            this.calcTableBoxes(new TablePosition(this.tables[this.nextTableIndex - 1], -1 ,-1));
                            TableTextBoxCollectorCellParagraphInfo.addBox(parInfo, -1, null); // set mark to understand laset that this nexted table
                        }
                        let parIndex: number = this.textBoxIterator.getParagraphIndex();
                        let par: Paragraph = paragraphs[parIndex];
                        while (!par || par.startLogPosition.value > this.currLayoutBox.rowOffset)
                            par = paragraphs[--parIndex];

                        TableTextBoxCollectorCellParagraphInfo.addBox(parInfo, parIndex, this.currLayoutBox);
                        this.currLayoutBox = this.textBoxIterator.getNextBox();
                    } while (this.currLayoutBox.rowOffset < currTablePosition.cell.endParagrapPosition.value);
                }
            }
        }

        private setNextTablePosition(nextTableIndex: number) {
            this.nextTableIndex = nextTableIndex;
            const nextTable: Table = this.tables[nextTableIndex];
            this.nextTablePosition = nextTable ? nextTable.getStartPosition() : Number.MAX_VALUE;
        }
    }
}