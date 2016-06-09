module __aspxRichEdit {
    export class TEST_CLASS {
        public static clearAllRunMergedProperties(control: IRichEditControl) {
            var chunks: Chunk[] = control.model.activeSubDocument.chunks;
            for (var chunkIndex: number = 0, chunk: Chunk; chunk = chunks[chunkIndex]; chunkIndex++) {
                for (var runIndex: number = 0, textRun: TextRun; textRun = chunk.textRuns[runIndex]; runIndex++) {
                    textRun.resetCharacterMergedProperties();
                }
            }

            control.formatterOnIntervalChanged(new FixedInterval(0, control.model.activeSubDocument.getDocumentEndPosition()), control.model.activeSubDocument);
            while (!control.layout.isFullyFormatted)
                control.forceFormatPage(control.layout.validPageCount);
        }

        public static maskToString(objEnum: any, mask: number, splitBy: string = ", ") {
            let res: string[] = [];
            for (let key in objEnum) {
                if (!objEnum.hasOwnProperty(key))
                    continue;
                const keyNum: number = parseInt(key);
                if (!isNaN(keyNum) && (keyNum & mask) == keyNum)
                    res.push(objEnum[key]);
            }
            return res.join(splitBy);
        }

        public static checkLayout(model: DocumentModel, layout: DocumentLayout) {
            const it: ModelIterator = new ModelIterator(model.mainSubDocument, true);
            it.setPosition(0);

            const pages: LayoutPage[] = layout.pages;
            for (let pageIndex: number = 0, page: LayoutPage; page = pages[pageIndex]; pageIndex++) {
                const pagePos: number = page.getStartOffsetContentOfMainSubDocument();
                const pageIntervals: FixedInterval[] = page.contentIntervals;
                let collectedIntervals: FixedInterval[] = [];
                const pageAreas: LayoutPageArea[] = page.mainSubDocumentPageAreas;
                if (pageAreas[0].pageOffset != 0)
                    console.log(pageIndex, page, "First page area offset != 0");
                for (let pageAreaIndex: number = 0, pageArea: LayoutPageArea; pageArea = pageAreas[pageAreaIndex]; pageAreaIndex++) {
                    const pageAreaPos: number = pagePos + pageArea.pageOffset;
                    const columns: LayoutColumn[] = pageArea.columns;
                    if (columns[0].pageAreaOffset != 0)
                        console.log(pageIndex, page, pageAreaIndex, pageArea, "First column offset != 0");
                    for (let columnIndex: number = 0, column: LayoutColumn; column = columns[columnIndex]; columnIndex++) {
                        const columnPos: number = pageAreaPos + column.pageAreaOffset;
                        const rows: LayoutRow[] = column.rows;
                        if (rows[0].columnOffset != 0)
                            console.log(pageIndex, page, pageAreaIndex, pageArea, columnIndex, column, "First row offset != 0");

                        for (let rowIndex: number = 0, row: LayoutRow; row = rows[rowIndex]; rowIndex++) {
                            const rowPos: number = columnPos + row.columnOffset;
                            collectedIntervals.push(new FixedInterval(rowPos, row.getLastBoxEndPositionInRow()));
                            const boxes: LayoutBox[] = row.boxes;
                            if (boxes[0].rowOffset != 0)
                                console.log(pageIndex, page, pageAreaIndex, pageArea, columnIndex, column, row, rowIndex, "First box offset != 0");


                            for (let boxIndex: number = 0, box: LayoutBox; box = boxes[boxIndex]; boxIndex++) {
                                //switch (box.getType()) {
                                //    case LayoutBoxType.Text:
                                //    case LayoutBoxType.Dash:
                                //    case LayoutBoxType.Space:
                                //    case LayoutBoxType.ParagraphMark:
                                //    default: it.advanceBy(box.getLength());

                                //}
                                //for (let charOffset: number = 0, char: string; char = box.ge
                                //if(box)
                            }
                        }
                    }
                }
                let lastEndPos: number = 0;
                for (let interval of collectedIntervals) {
                    if (interval.start < lastEndPos)
                        console.log("Intervals of row not sorted", page, collectedIntervals);
                    lastEndPos = interval.end();
                }

                const mergedIntervals: FixedInterval[] = Utils.getMergedIntervals(collectedIntervals, true);

                let pageIntervalIndex: number = 0;
                let currPageInterval: FixedInterval = pageIntervals[pageIntervalIndex];
                for (let mergedIntervalIndex: number = 0, mgInt: FixedInterval; mgInt = mergedIntervals[mergedIntervalIndex]; mergedIntervalIndex++) {
                    if (!currPageInterval.containsInterval(mgInt.start, mgInt.end())) {
                        pageIntervalIndex++;
                        currPageInterval = pageIntervals[pageIntervalIndex];
                        if (!currPageInterval || !currPageInterval.containsInterval(mgInt.start, mgInt.end()))
                            console.log("current page contentIntervals not consider some row intervals", [mgInt.start, mgInt.end()], mergedIntervals, pageIntervals, collectedIntervals, page);
                    }
                }
            }
        }

        public static checkModel(model: DocumentModel) {
            for(let sid in model.subDocuments) {
                let subDocument = model.subDocuments[sid];
                let prevParagraphEnd = 0;
                for(let pIndex = 0, paragraph: Paragraph; paragraph = subDocument.paragraphs[pIndex]; pIndex++) {
                    if(paragraph.startLogPosition.value !== prevParagraphEnd) {
                        console.log(`paragraphs[${pIndex}].length !== prevParagraphEnd`)
                    }
                    prevParagraphEnd = paragraph.getEndPosition();
                    if(paragraph.length === 0) {
                        console.log(`paragraphs[${pIndex}].length == 0`);
                        continue;
                    }
                    let endParRun = subDocument.getRunByPosition(paragraph.getEndPosition() - 1);
                    if(endParRun.type !== TextRunType.ParagraphRun) {
                        console.log(`The last run of paragraph ${pIndex} is not ParagraphRun`);
                        continue;
                    }
                }
                if(subDocument.paragraphs[subDocument.paragraphs.length - 1].getEndPosition() !== subDocument.getLastChunk().getEndPosition())
                    console.log(`paragraphs.length !== chunks.length in sid=${sid}`);

                let prevTableStartPosition = -1;
                for(let tIndex = 0, table: Table; table = subDocument.tables[tIndex]; tIndex++) {
                    if(table.getStartPosition() < prevTableStartPosition)
                        console.log(`tables are not sorted. tables[${tIndex}].getStartPosition() < prevTableStartPosition in sid=${sid}`);
                    prevTableStartPosition = table.getStartPosition();
                    if(table.index !== tIndex)
                        console.log(`subDocument.tables[${tIndex}] !== subDocument.tables[${tIndex}].index`);
                    if(table.nestedLevel == 0 && table.parentCell)
                        console.log(`subDocument.tables[${tIndex}].parentCell exists but nestedLevel===0`);
                    else if(table.nestedLevel > 0 && !table.parentCell)
                        console.log(`subDocument.tables[${tIndex}].parentCell doesn't exist but nestedLevel>0`);
                    else if(table.parentCell && table.parentCell.parentRow.parentTable.index >= table.index)
                        console.log(`subDocument.tables[${tIndex}].parentCell.parentRow.parentTable.index >= table.index`);
                    let prevRowEndPosition = -1;
                    let prevColumnsCount = -1;
                    for(let rIndex = 0, row: TableRow; row = table.rows[rIndex]; rIndex++) {
                        let currentColumnsCount = row.gridAfter + row.gridBefore;
                        if(row.cells.length === 0)
                            console.log(`tables[${tIndex}].rows.length === 0`);
                        if(prevRowEndPosition >= 0 && prevRowEndPosition !== row.getStartPosition())
                            console.log(`tables[${tIndex}].rows[${rIndex}].getStartPosition() != prevRowEndPosition`);
                        if(row.parentTable !== table)
                            console.log(`tables[${tIndex}].rows[${rIndex}].parentTable != table`);
                        prevRowEndPosition = row.getEndPosition();
                        for(let cIndex = 0, cell: TableCell; cell = row.cells[cIndex]; cIndex++) {
                            currentColumnsCount += cell.columnSpan;
                            if(cell.startParagraphPosition.value >= cell.endParagrapPosition.value)
                                console.log(`tables[${tIndex}].rows[${rIndex}].cells[${cIndex}].startParagraphPosition.value >= cell.endParagrapPosition.value`);
                            let startParagraph = subDocument.getParagraphByPosition(cell.startParagraphPosition.value);
                            let endParagraph = subDocument.getParagraphByPosition(cell.endParagrapPosition.value - 1);
                            if(cell.startParagraphPosition.value !== startParagraph.startLogPosition.value)
                                console.log(`tables[${tIndex}].rows[${rIndex}].cells[${cIndex}] doesn't start with paragraph`);
                            if(cell.endParagrapPosition.value !== endParagraph.getEndPosition())
                                console.log(`tables[${tIndex}].rows[${rIndex}].cells[${cIndex}] doesn't end with paragraph`);
                        }
                        if(rIndex > 0 && currentColumnsCount !== prevColumnsCount)
                            console.log(`tables[${tIndex }].rows[${rIndex}].columnsCount(${currentColumnsCount}) != prevColumnsCount(${prevColumnsCount})`);
                        prevColumnsCount = currentColumnsCount;
                    }
                }
            }
        }

        public static writeTableStructure(subDocument: SubDocument, index: number) {
            console.log(this.getTableStructure(subDocument, index, ""));
        }

        private static getTableStructure(subDocument: SubDocument, index: number, result: string): string {
            let table = subDocument.tables[index];
            result += "<";
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                if(rowIndex > 0)
                    result += "\n";
                result += "\t";
                result += Array(row.gridBefore + 1).join("→ ");
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    result += ` [${cell.startParagraphPosition.value} `;
                    if(cell.columnSpan > 1)
                        result += Array(cell.columnSpan).join("→");
                    if(cell.verticalMerging === TableCellMergingState.Restart)
                        result += "↓";
                    if(cell.verticalMerging === TableCellMergingState.Continue)
                        result += "↑";

                    let nextTable = subDocument.tables[index + 1];
                    if(nextTable && nextTable.getStartPosition() == cell.startParagraphPosition.value)
                        this.getTableStructure(subDocument, index + 1, result);
                    result += ` ${cell.endParagrapPosition.value}] `;
                }
                result += Array(row.gridAfter + 1).join("← ");
            }
            result += ">";
            return result;
        }
    }
}