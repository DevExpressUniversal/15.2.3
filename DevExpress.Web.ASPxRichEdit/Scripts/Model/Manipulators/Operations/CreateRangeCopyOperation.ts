module __aspxRichEdit {
    export class CreateRangeCopyOperation {
        private subDocument: SubDocument;
        private documentModel: DocumentModel;

        private oldCurrentChunk: Chunk;
        private oldCurrentParagraph: Paragraph;
        private oldCurrentSection: Section;
        private oldFieldStartIndex: number = -1;

        private newCurrentChunk: Chunk;
        private newCurrentSection: Section;
        private newCurrentParagraph: Paragraph;
        private newOffsetAtStartChunk: number = 0;

        private additionalParagraphRunPositions: { [pos: number]: boolean } = {};

        constructor(subDocument: SubDocument) {
            this.subDocument = subDocument;
            this.documentModel = subDocument.documentModel;
        }
        execute(intervals: FixedInterval[]): RangeCopy {
            let newDocumentModel = this.initNewDocumentModel();
            let newSubDocument = newDocumentModel.activeSubDocument;

            for(let i = 0, interval: FixedInterval; interval = intervals[i]; i++)
                this.copyMainContent(interval, newDocumentModel);
            newSubDocument.getLastChunk().isLast = true;
            this.copyTables(newSubDocument, intervals);
            var addedUselessParagraphMarkInEnd = this.tryAppendAdditionalParagraphRunInTheEnd(newSubDocument, false);
            return new RangeCopy(newDocumentModel, addedUselessParagraphMarkInEnd);
        }
        protected copyMainContent(interval: FixedInterval, newDocumentModel: DocumentModel) {
            let newSubDocument = newDocumentModel.activeSubDocument;
            var oldDocumentModel: DocumentModel = this.subDocument.documentModel,
                constRunIterator: RunIterator = this.subDocument.getConstRunIterator(interval),
                numberingListCache: { [key: number]: number } = {},
                abstractNumberingListCache: { [key: number]: number } = {};

            while(constRunIterator.moveNext()) {
                var oldCurrentRun: TextRun = constRunIterator.currentRun;
                if(this.oldCurrentSection != constRunIterator.currentSection) {
                    this.oldCurrentSection = constRunIterator.currentSection;
                    this.newCurrentSection = this.appendNewSection(newSubDocument);
                }

                if(this.oldCurrentParagraph != oldCurrentRun.paragraph) {
                    this.oldCurrentParagraph = oldCurrentRun.paragraph;
                    this.newCurrentParagraph = this.appendNewParagraph(newSubDocument, abstractNumberingListCache, numberingListCache);
                }

                if(this.oldCurrentChunk != constRunIterator.currentChunk) {
                    this.oldCurrentChunk = constRunIterator.currentChunk;
                    this.newCurrentChunk = this.appendNewChunk(newSubDocument);
                    this.newOffsetAtStartChunk = 0;
                }

                this.newCurrentParagraph.length += oldCurrentRun.length;
                this.newCurrentSection.setLength(newSubDocument, this.newCurrentSection.getLength() + oldCurrentRun.length);
                this.newCurrentChunk.textBuffer += this.oldCurrentChunk.getTextInChunk(oldCurrentRun.startOffset, oldCurrentRun.length);

                if(oldCurrentRun.type == TextRunType.FieldCodeStartRun)
                    this.appendField(newSubDocument, oldCurrentRun, interval);

                this.newCurrentChunk.textRuns.push(TextRun.create(this.newOffsetAtStartChunk, oldCurrentRun.length, oldCurrentRun.type, this.newCurrentParagraph,
                    newDocumentModel.stylesManager.addCharacterStyle(oldCurrentRun.characterStyle),
                    newDocumentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(oldCurrentRun.maskedCharacterProperties), oldCurrentRun));

                this.newOffsetAtStartChunk += oldCurrentRun.length;
            }
        }

        protected appendField(newSubDocument: SubDocument, oldCurrentRun: TextRun, interval: FixedInterval) {
            let oldFieldStartIndex = this.oldFieldStartIndex;
            let oldSubDocument = this.subDocument;
            if(oldFieldStartIndex < 0) {
                var oldFieldStartCodeRunOffset: number = this.oldCurrentChunk.startLogPosition.value + oldCurrentRun.startOffset;
                oldFieldStartIndex = Field.normedBinaryIndexOf(oldSubDocument.fields, oldFieldStartCodeRunOffset + 1);
            }
            else
                oldFieldStartIndex++;

            var oldField: Field = oldSubDocument.fields[oldFieldStartIndex];
            var newField: Field = new Field(newSubDocument.positionManager, newSubDocument.fields.length, oldField.getFieldStartPosition() - interval.start, oldField.getSeparatorPosition() - interval.start,
                oldField.getFieldEndPosition() - interval.start, oldField.showCode, oldField.getHyperlinkInfo() ? oldField.getHyperlinkInfo().clone() : undefined);
            newSubDocument.fields.push(newField);
            newField.initParent(newSubDocument.fields);
            this.oldFieldStartIndex = oldFieldStartIndex;
        }

        protected appendParagraphMarkInTheEnd(newSubDocument: SubDocument, lastRun: TextRun, sectionEnd: boolean) {
            this.newCurrentParagraph.length += 1;
            this.newCurrentSection.setLength(newSubDocument, this.newCurrentSection.getLength() + 1);
            let text = sectionEnd ? Utils.specialCharacters.SectionMark : Utils.specialCharacters.ParagraphMark;
            let type = sectionEnd ? TextRunType.SectionRun : TextRunType.ParagraphRun;
            this.newCurrentChunk.textBuffer += text;
            this.newCurrentChunk.textRuns.push(TextRun.create(lastRun.startOffset + lastRun.length, 1, type, this.newCurrentParagraph,
                newSubDocument.getLastRun().characterStyle, newSubDocument.getLastRun().maskedCharacterProperties));
            this.newOffsetAtStartChunk++;
        }

        protected appendNewChunk(newSubDocument: SubDocument): Chunk {
            let newCurrentChunkAbsolutePosition: number = newSubDocument.chunks.length ? newSubDocument.getLastChunk().getEndPosition() : 0,
                newCurrentChunkPosition: Position = newSubDocument.positionManager.registerPosition(newCurrentChunkAbsolutePosition),
                newCurrentChunk: Chunk = new Chunk(newCurrentChunkPosition, "", false);
            newSubDocument.chunks.push(newCurrentChunk);
            return newCurrentChunk;
        }

        protected appendNewSection(newSubDocument: SubDocument): Section {
            this.tryAppendAdditionalParagraphRunInTheEnd(newSubDocument, true);
            let lastSection = newSubDocument.documentModel.sections[newSubDocument.documentModel.sections.length - 1];
            let newCurrentSectionAbsolutePosition = lastSection ? lastSection.getEndPosition() : 0,
                newCurrentSectionPosition: Position = newSubDocument.positionManager.registerPosition(newCurrentSectionAbsolutePosition);
            let newCurrentSection: Section = new Section(newSubDocument.documentModel, newCurrentSectionPosition, 0, this.oldCurrentSection.sectionProperties.clone());
            newSubDocument.documentModel.sections.push(newCurrentSection);
            return newCurrentSection;
        }

        protected appendNewParagraph(newSubDocument: SubDocument, abstractNumberingListCache: { [key: number]: number }, numberingListCache: { [key: number]: number }): Paragraph {
            this.tryAppendAdditionalParagraphRunInTheEnd(newSubDocument, false);
            let oldCurrentParagraph = this.oldCurrentParagraph;
            let oldDocumentModel = oldCurrentParagraph.subDocument.documentModel;
            let lastParagraph = newSubDocument.paragraphs[newSubDocument.paragraphs.length - 1];
            var newCurrentParagraphAbsolutePosition: number = lastParagraph ? lastParagraph.getEndPosition() : 0;
            var newCurrentParagraphPosition: Position = newSubDocument.positionManager.registerPosition(newCurrentParagraphAbsolutePosition);
            var newCurrentParagraphStyle: ParagraphStyle = newSubDocument.documentModel.stylesManager.addParagraphStyle(oldCurrentParagraph.paragraphStyle);
            var newCurrentMaskedParagraphProperties: MaskedParagraphProperties = newSubDocument.documentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(oldCurrentParagraph.maskedParagraphProperties);
            let newCurrentParagraph = new Paragraph(newSubDocument, newCurrentParagraphPosition, 0, newCurrentParagraphStyle, newCurrentMaskedParagraphProperties);
            var newCurrentParagraphNumberingListIndex = -1,
                newAbstractNumberingListIndex = -1;
            if(oldCurrentParagraph.numberingListIndex >= 0) {
                var oldNumberingList = oldDocumentModel.numberingLists[oldCurrentParagraph.numberingListIndex];
                newCurrentParagraphNumberingListIndex = numberingListCache[oldNumberingList.getId()];
                if(newCurrentParagraphNumberingListIndex === undefined) {
                    var oldAbstractNumberingList = oldDocumentModel.abstractNumberingLists[oldNumberingList.abstractNumberingListIndex];
                    newAbstractNumberingListIndex = abstractNumberingListCache[oldAbstractNumberingList.getId()];
                    if(newAbstractNumberingListIndex === undefined) {
                        var newAbstractNumberingList = new AbstractNumberingList(newSubDocument.documentModel);
                        newAbstractNumberingList.copyFrom(oldAbstractNumberingList);
                        newAbstractNumberingListIndex = newSubDocument.documentModel.abstractNumberingLists.push(newAbstractNumberingList) - 1;
                        abstractNumberingListCache[oldAbstractNumberingList.getId()] = newAbstractNumberingListIndex;
                    }
                    var newNumberingList = new NumberingList(newSubDocument.documentModel, newAbstractNumberingListIndex);
                    newNumberingList.copyFrom(oldNumberingList);
                    newCurrentParagraphNumberingListIndex = newSubDocument.documentModel.numberingLists.push(newNumberingList) - 1;
                    numberingListCache[oldNumberingList.getId()] = newCurrentParagraphNumberingListIndex;
                }
            }
            newCurrentParagraph.listLevelIndex = oldCurrentParagraph.listLevelIndex;
            newCurrentParagraph.numberingListIndex = newCurrentParagraphNumberingListIndex;
            newSubDocument.paragraphs.push(newCurrentParagraph);
            return newCurrentParagraph;
        }

        protected tryAppendAdditionalParagraphRunInTheEnd(newSubDocument: SubDocument, sectionEnd: boolean): boolean {
            let lastChunk = newSubDocument.getLastChunk();
            if(!lastChunk)
                return;
            let lastRun = lastChunk.textRuns[lastChunk.textRuns.length - 1];
            if(!lastRun)
                return;
            if(lastRun.type !== TextRunType.ParagraphRun && lastRun.type !== TextRunType.SectionRun) {
                this.appendParagraphMarkInTheEnd(newSubDocument, lastRun, sectionEnd);
                this.additionalParagraphRunPositions[lastChunk.getEndPosition() - 1] = true;
                return true;
            }
            return false;
        }

        protected initNewDocumentModel(): DocumentModel {
            let newDocumentModel: DocumentModel = new DocumentModel(this.documentModel.options, 0);
            newDocumentModel.defaultCharacterProperties = newDocumentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(this.documentModel.defaultCharacterProperties);
            newDocumentModel.defaultParagraphProperties = newDocumentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(this.documentModel.defaultParagraphProperties);
            return newDocumentModel;
        }

        protected copyTables(newSubDocument: SubDocument, intervals: FixedInterval[]) {
            let oldSubDocument = this.subDocument;
            let startTableIndex = Utils.normedBinaryIndexOf(this.subDocument.tables, t => t.getStartPosition() - intervals[0].start);
            if(startTableIndex < 0)
                startTableIndex = 0;
            else if(this.subDocument.tables[startTableIndex].nestedLevel > 1) {
                while(this.subDocument.tables[startTableIndex].nestedLevel > 1)
                    startTableIndex--;
            }
            let nestedLevel = -1;
            let endSelectionPosition = intervals[intervals.length - 1].end();
            let prevLength = 0;
            for(let i = startTableIndex, table: Table; table = this.subDocument.tables[i]; i++) {
                if(table.nestedLevel > nestedLevel)
                    nestedLevel++;
                let tableStartPosition = table.getStartPosition();
                let tableEndPosition = table.getEndPosition();

                while(intervals.length > 0 && tableStartPosition >= intervals[0].end()) {
                    if(this.additionalParagraphRunPositions[intervals[0].end()])
                        prevLength++;
                    prevLength += intervals[0].length
                    intervals.shift();
                }
                if(!intervals.length)
                    break;

                let tableIntersection = FixedInterval.getIntersection(FixedInterval.fromPositions(tableStartPosition, tableEndPosition), intervals[0]);
                if(tableIntersection && tableIntersection.length) {
                    if(intervals[0].containsInterval(tableStartPosition, tableEndPosition)) {
                        this.appendWholeTable(newSubDocument, table, intervals[0].start - prevLength, nestedLevel);
                    }
                    else {
                        let selectedCellInfos = this.getSelectedCells(table, intervals.slice(0), prevLength);
                        if(this.canCopyParticallyTable(selectedCellInfos))
                            this.appendParticallyTable(newSubDocument, selectedCellInfos, nestedLevel);
                        else
                            nestedLevel = -1;
                    }
                }
                else if(tableStartPosition >= endSelectionPosition)
                    break;
            }
        }

        protected appendWholeTable(newSubDocument: SubDocument, table: Table, positionDelta: number, newNestedLevel: number) {
            let newTable = this.createTable(newSubDocument, table, newNestedLevel, positionDelta);
            newTable.preferredWidth = table.preferredWidth.clone();
            newTable.lookTypes = table.lookTypes;
            for(let i = 0, row: TableRow; row = table.rows[i]; i++) {
                let newRow = new TableRow(newTable, newSubDocument.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(row.properties.clone()));
                newTable.rows.push(newRow);
                newRow.height = row.height.clone();
                if(row.tablePropertiesException)
                    newRow.tablePropertiesException = row.tablePropertiesException.clone();
                newRow.gridBefore = row.gridBefore;
                newRow.gridAfter = row.gridAfter;
                newRow.widthAfter = row.widthAfter.clone();
                newRow.widthBefore = row.widthBefore.clone();
                for(let j = 0, cell: TableCell; cell = row.cells[j]; j++) {
                    let newCell = this.cloneTableCell(newSubDocument, newRow, cell);
                    newCell.verticalMerging = cell.verticalMerging;
                    newCell.startParagraphPosition = newSubDocument.positionManager.registerPosition(cell.startParagraphPosition.value - positionDelta);
                    newCell.endParagrapPosition = newSubDocument.positionManager.registerPosition(cell.endParagrapPosition.value - positionDelta);
                    newRow.cells.push(newCell);
                }
            }
        }

        private appendParticallyTable(newSubDocument: SubDocument, selectedCellInfos: TableCellInfo[][], newNestedLevel: number) {
            let minLeftColumnIndex = Number.MAX_VALUE;
            let maxRightColumnIndex = 0;

            let table = selectedCellInfos[0][0].cell.parentRow.parentTable;
            for(let i = 0, horCells: TableCellInfo[]; horCells = selectedCellInfos[i]; i++) {
                let leftColumnIndex = TableCellUtils.getStartColumnIndex(horCells[0].cell);
                let rightColumnIndex = TableCellUtils.getEndColumnIndex(horCells[horCells.length - 1].cell);
                minLeftColumnIndex = Math.min(minLeftColumnIndex, leftColumnIndex);
                maxRightColumnIndex = Math.max(maxRightColumnIndex, rightColumnIndex);
            }
            let newStartPosition = selectedCellInfos[0][0].cell.startParagraphPosition.value - selectedCellInfos[0][0].positionDelta;
            let newTable = this.createTable(newSubDocument, table, newNestedLevel, newStartPosition);
            newTable.preferredWidth = TableWidthUnit.create(0, TableWidthUnitType.Auto);
            for(let i = 0, horCellInfos: TableCellInfo[]; horCellInfos = selectedCellInfos[i]; i++) {
                let leftColumnIndex = TableCellUtils.getStartColumnIndex(horCellInfos[0].cell);
                let rightColumnIndex = TableCellUtils.getEndColumnIndex(horCellInfos[horCellInfos.length - 1].cell);
                let row = horCellInfos[0].cell.parentRow;
                let newRow = new TableRow(newTable, newSubDocument.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(row.properties.clone()));
                newTable.rows.push(newRow);
                newRow.height = row.height.clone();
                newRow.gridBefore = leftColumnIndex - minLeftColumnIndex;
                newRow.gridAfter = maxRightColumnIndex - rightColumnIndex;

                for(let j = 0, cellInfo: TableCellInfo; cellInfo = horCellInfos[j]; j++) {
                    let cellInterval = cellInfo.cell.getInterval();
                    let newCell = this.cloneTableCell(newSubDocument, newRow, cellInfo.cell);
                    newCell.startParagraphPosition = newSubDocument.positionManager.registerPosition(cellInfo.cell.startParagraphPosition.value - cellInfo.positionDelta);
                    newCell.endParagrapPosition = newSubDocument.positionManager.registerPosition(cellInfo.cell.endParagrapPosition.value - cellInfo.positionDelta);
                    newRow.cells.push(newCell);
                }
            }
            TablesManipulator.normalizeCellColumnSpansWithoutHistory(newTable, true);
        }

        private canCopyParticallyTable(selectedCellInfos: TableCellInfo[][]): boolean {
            if(selectedCellInfos.length === 0)
                return false;
            let prevRowEndPosition = selectedCellInfos[0][0].cell.parentRow.getEndPosition();
            let prevLeftColumnIndex = TableCellUtils.getStartColumnIndex(selectedCellInfos[0][0].cell);
            let prevRightColumnIndex = TableCellUtils.getEndColumnIndex(selectedCellInfos[0][selectedCellInfos[0].length - 1].cell);
            for(let i = 0, horCells: TableCellInfo[]; horCells = selectedCellInfos[i]; i++) {
                let prevCellEndPosition = horCells[0].cell.endParagrapPosition.value;
                for(let j = 1, cellInfo: TableCellInfo; cellInfo = horCells[j]; j++) {
                    if(cellInfo.cell.startParagraphPosition.value !== prevCellEndPosition)
                        return false;
                    prevCellEndPosition = cellInfo.cell.endParagrapPosition.value;
                }
                if(i > 0) {
                    if(horCells[0].cell.parentRow.getStartPosition() !== prevRowEndPosition)
                        return false;
                    let leftColumnIndex = TableCellUtils.getStartColumnIndex(horCells[0].cell);
                    let rightColumnIndex = TableCellUtils.getEndColumnIndex(horCells[horCells.length - 1].cell);
                    if(rightColumnIndex < prevLeftColumnIndex || leftColumnIndex > prevRightColumnIndex)
                        return false;
                    prevRowEndPosition = horCells[0].cell.parentRow.getEndPosition();
                }
            }
            return true;
        }

        private cloneTableCell(newSubDocument: SubDocument, newRow: TableRow, sourceCell: TableCell): TableCell {
            let newCell = new TableCell(newRow, newSubDocument.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(sourceCell.properties.clone()));
            newCell.columnSpan = sourceCell.columnSpan;
            newCell.conditionalFormatting = sourceCell.conditionalFormatting;
            newCell.preferredWidth = sourceCell.preferredWidth.clone();
            newCell.style = sourceCell.style;
            return newCell;
        }

        private createTable(newSubDocument: SubDocument, oldTable: Table, newNestedLevel: number, newDocumentStartPosition: number): Table {
            let newTableStyle = newSubDocument.documentModel.stylesManager.addTableStyle(oldTable.style);
            let newTable = new Table(oldTable.properties.clone(), newTableStyle);
            newTable.nestedLevel = newNestedLevel;
            if(newNestedLevel > 0) {
                let newStartPosition = oldTable.getStartPosition() - newDocumentStartPosition;
                newTable.parentCell = Table.getTableCellByPosition(newSubDocument.tables, newStartPosition);
            }
            newTable.index = newSubDocument.tables.push(newTable) - 1;
            (newSubDocument.tablesByLevels[newNestedLevel] || (newSubDocument.tablesByLevels[newNestedLevel] = [])).push(newTable);
            return newTable;
        }

        private getSelectedCells(table: Table, intervals: FixedInterval[], prevLength: number): TableCellInfo[][] {
            let currentIntervalIndex = 0;
            let maxIntervalIndex = intervals.length - 1;
            let selectedCellInfos: TableCellInfo[][] = [];
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let horCells: TableCellInfo[] = [];
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    while(currentIntervalIndex <= maxIntervalIndex && intervals[currentIntervalIndex].end() < cell.endParagrapPosition.value) {
                        if(this.additionalParagraphRunPositions[intervals[currentIntervalIndex].end()])
                            prevLength++;
                        prevLength += intervals[currentIntervalIndex].length;
                        currentIntervalIndex++;
                    }
                    if(currentIntervalIndex > maxIntervalIndex)
                        break;
                    if(intervals[currentIntervalIndex].containsInterval(cell.startParagraphPosition.value, cell.endParagrapPosition.value)) {
                        horCells.push(new TableCellInfo(cell, intervals[currentIntervalIndex].start - prevLength));
                    }
                }
                if(horCells.length)
                    selectedCellInfos.push(horCells);
                if(currentIntervalIndex > maxIntervalIndex)
                    break;
            }
            return selectedCellInfos;
        }
    }

    export class RangeCopy {
        model: DocumentModel;
        addedUselessParagraphMarkInEnd: boolean;

        constructor(model: DocumentModel, addedUselessParagraphMarkInEnd: boolean) {
            this.model = model;
            this.addedUselessParagraphMarkInEnd = addedUselessParagraphMarkInEnd;
        }
    }

    class TableCellInfo {
        cell: TableCell;
        positionDelta: number;
        constructor(cell: TableCell, positionDelta: number) {
            this.cell = cell;
            this.positionDelta = positionDelta;
        }
    }
}