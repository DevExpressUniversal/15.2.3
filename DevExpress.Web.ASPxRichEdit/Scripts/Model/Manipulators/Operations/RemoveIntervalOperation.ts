module __aspxRichEdit {
    export class RemoveIntervalOperation {
        manipulator: ModelManipulator;

        currentSectionIndex: number;
        currentChunkIndex: number;
        currentParagraphIndex: number;
        currentCellIndex: number = 0;

        position: number;
        subDocument: SubDocument;
        fieldIndexThatNeedDelete: number;
        shouldMergeParagraphs: boolean;

        cellsIterator: SelectedCellsIterator;

        constructor(manipulator: ModelManipulator, subDocument: SubDocument) {
            this.manipulator = manipulator;
            this.subDocument = subDocument;
        }

        execute(interval: FixedInterval, applyPropertiesToLeft: boolean, needHistory: boolean): RemoveIntervalOperationResult {
            if(interval.end() >= this.subDocument.getDocumentEndPosition())
                throw new Error("Interval.end() must be less than subDocument.getDocumentEndPosition()");
            this.cellsIterator = new SelectedCellsIterator(this.subDocument, interval);
            let result = new RemoveIntervalOperationResult(interval, this.cellsIterator);
            if(this.tryPackSelectionInOneRun(interval, result))
                return result;
            var iterator: RunIterator = this.subDocument.getRunIterator(interval);
            if(needHistory)
                this.fillResult(interval, result);
            this.initializeStartPositions(interval.start);
            this.executeCore(applyPropertiesToLeft, interval.start, iterator, result);
            return result;
        }

        private initializeStartPositions(intervalStart: number) {
            this.position = intervalStart;
            this.currentChunkIndex = Utils.normedBinaryIndexOf(this.subDocument.chunks, (c: Chunk) => c.startLogPosition.value - intervalStart);
            this.currentSectionIndex = Utils.normedBinaryIndexOf(this.subDocument.documentModel.sections, s => s.startLogPosition.value - intervalStart);
            this.currentParagraphIndex = Utils.normedBinaryIndexOf(this.subDocument.paragraphs, p => p.startLogPosition.value - intervalStart);
            this.fieldIndexThatNeedDelete = -1;
        }

        private executeCore(applyPropertiesToLeft: boolean, startPosition: number, iterator: RunIterator, result: RemoveIntervalOperationResult) {
            var subDocument = this.subDocument;
            var accumulatedInterval = new FixedInterval(this.position, 0);
            while(iterator.moveNext()) {
                if(iterator.currentChunk !== subDocument.chunks[this.currentChunkIndex]) {
                    this.removeAccumulatedInterval(accumulatedInterval, this.position, 0);
                    this.currentChunkIndex++;
                }
                if(subDocument.documentModel.sections[this.currentSectionIndex] !== iterator.currentSection)
                    this.currentSectionIndex++;
                var runIndex = Utils.normedBinaryIndexOf(subDocument.chunks[this.currentChunkIndex].textRuns, r => r.startOffset - iterator.currentRun.startOffset);
                if(iterator.currentRun.type == TextRunType.FieldCodeStartRun)
                    this.removeField(startPosition);
                if(iterator.currentRun.type == TextRunType.ParagraphRun) {
                    if(subDocument.paragraphs.length === 1) {
                        result.removeLastParagraphRun();
                        continue;
                    }
                }
                iterator.currentRun.paragraph.length -= iterator.currentRun.length;
                this.modifySectionLength(iterator.currentSection, -iterator.currentRun.length);
                var strategy = this.getStrategy(iterator, accumulatedInterval);
                strategy.call(this, accumulatedInterval, runIndex);
            }
            this.removeAccumulatedInterval(accumulatedInterval, -1, 0);
            if(this.shouldMergeParagraphs)
                this.tryMergeStartEndParagraphs(startPosition, applyPropertiesToLeft);
            this.cellsIterator.reset();
        }

        private modifySectionLength(section: Section, delta: number) {
            section.setLength(this.subDocument, section.getLength() + delta);
        }

        private getStrategy(iterator: RunIterator, accumulatedInterval: FixedInterval): (accumulatedInterval: FixedInterval, runIndex: number) => void {
            if(!this.canRemoveRun(iterator.currentRun, iterator.currentRun.startOffset + iterator.currentChunk.startLogPosition.value + accumulatedInterval.length, iterator.currentRun.paragraph.length === 0))
                return this.skipRunAndMoveToNextParagraph;
            else if(iterator.currentRun.type === TextRunType.SectionRun && iterator.currentSection.getLength() == 0)
                return this.removeWholeSection;
            else if(iterator.currentRun.type === TextRunType.ParagraphRun && iterator.currentRun.paragraph.length === 0)
                return this.removeWholeParagraph;
            else if(iterator.currentRun.type === TextRunType.SectionRun && this.shouldMergeParagraphs && iterator.currentRun.paragraph.length === 0)
                return this.mergePreviousParagraph;
            else if(iterator.currentRun.type !== TextRunType.ParagraphRun && iterator.currentRun.type !== TextRunType.SectionRun)
                return this.removeTextRun;
            else
                return this.skipParagraphRunAndMergeParagraphsAtTheEnd;
        }


        /* Strategies */
        private removeTextRun(accumulatedInterval: FixedInterval, runIndex: number) {
            accumulatedInterval.length += this.subDocument.chunks[this.currentChunkIndex].textRuns[runIndex].length;
            this.removeRunInternal(this.subDocument, runIndex);
        }

        private skipParagraphRunAndMergeParagraphsAtTheEnd(accumulatedInterval: FixedInterval, runIndex: number) {
            this.skipRunAndMoveToNextParagraph(accumulatedInterval, runIndex);
            this.shouldMergeParagraphs = true;
        }

        private skipRunAndMoveToNextParagraph(accumulatedInterval: FixedInterval, runIndex: number) {
            this.subDocument.paragraphs[this.currentParagraphIndex].length++;
            this.modifySectionLength(this.subDocument.documentModel.sections[this.currentSectionIndex], 1);
            this.position++;
            this.removeAccumulatedInterval(accumulatedInterval, this.position, 0);
            this.currentParagraphIndex++;
        }

        private mergePreviousParagraph(accumulatedInterval: FixedInterval, runIndex: number) {
            if(runIndex === 0)
                this.currentChunkIndex--;
            var paragraphIndex: number = this.currentParagraphIndex;
            this.subDocument.paragraphs[paragraphIndex].length++;
            this.removeAccumulatedInterval(accumulatedInterval, this.position, 0);
            this.mergeParagraphsInternal(this.subDocument, paragraphIndex - 1, false);
            this.manipulator.dispatcher.notifyParagraphMerged(this.subDocument, this.subDocument.paragraphs[paragraphIndex - 1].getEndPosition() - 1, false);
        }

        private removeWholeSection(accumulatedInterval: FixedInterval, runIndex: number) {
            var subDocument = this.subDocument;
            var currentSection = subDocument.documentModel.sections[this.currentSectionIndex];
            this.removeAccumulatedInterval(accumulatedInterval, this.position, -1);
            if(subDocument.documentModel.sections.length > 1) {
                subDocument.positionManager.unregisterPosition(currentSection.startLogPosition);
                subDocument.documentModel.sections.splice(this.currentSectionIndex, 1);
                subDocument.positionManager.unregisterPosition(subDocument.paragraphs[this.currentParagraphIndex].startLogPosition);
                subDocument.paragraphs.splice(this.currentParagraphIndex, 1);
                this.removeRunInternal(subDocument, runIndex);
                this.currentSectionIndex--;
                this.manipulator.dispatcher.notifySectionMerged(subDocument, this.position, false);
            }
        }

        private removeWholeParagraph(accumulatedInterval: FixedInterval, runIndex: number) {
            var subDocument = this.subDocument;
            this.removeAccumulatedInterval(accumulatedInterval, this.position, -1);
            if(subDocument.paragraphs.length > 1) {
                subDocument.positionManager.unregisterPosition(subDocument.paragraphs[this.currentParagraphIndex].startLogPosition);
                subDocument.paragraphs.splice(this.currentParagraphIndex, 1);
                this.removeRunInternal(subDocument, runIndex);
                this.manipulator.dispatcher.notifyParagraphMerged(subDocument, this.position, false);
            }
        }
        /* End Strategies */


        private tryMergeStartEndParagraphs(startPosition: number, applyPropertiesToLeft: boolean) {
            var subDocument = this.subDocument;
            var firstParagraphIndex = subDocument.getParagraphIndexByPosition(startPosition),
                firstParagraph = subDocument.paragraphs[firstParagraphIndex],
                lastParagraph = subDocument.paragraphs[firstParagraphIndex + 1];
            var firstSectionIndex = Utils.normedBinaryIndexOf(subDocument.documentModel.sections, s => s.startLogPosition.value - firstParagraph.startLogPosition.value),
                firstSection = subDocument.documentModel.sections[firstSectionIndex];
            if(lastParagraph) {
                this.modifySectionLength(firstSection, -1);
                if(this.subDocument.isMain() && firstSection.getEndPosition() === firstParagraph.getEndPosition() - 1 && firstSectionIndex < subDocument.documentModel.sections.length - 1) {
                    // merge sections
                    var lastSection = subDocument.documentModel.sections[firstSectionIndex + 1];
                    subDocument.positionManager.unregisterPosition(lastSection.startLogPosition);
                    lastSection.startLogPosition = subDocument.positionManager.registerPosition(firstSection.startLogPosition.value);
                    this.modifySectionLength(lastSection, firstSection.getLength());
                    subDocument.positionManager.unregisterPosition(firstSection.startLogPosition);
                    subDocument.documentModel.sections.splice(firstSectionIndex, 1);
                    this.mergeParagraphsInternal(subDocument, firstParagraphIndex, applyPropertiesToLeft);
                    this.manipulator.dispatcher.notifySectionMerged(subDocument, firstSection.getEndPosition() - 1, !!applyPropertiesToLeft);
                }
                else {
                    // merge paragraphs
                    this.mergeParagraphsInternal(subDocument, firstParagraphIndex, applyPropertiesToLeft);
                    this.manipulator.dispatcher.notifyParagraphMerged(subDocument, lastParagraph.startLogPosition.value - 1, !!applyPropertiesToLeft);
                }
            }
        }

        private removeField(startPosition: number) {
            if(this.fieldIndexThatNeedDelete < 0) {
                this.fieldIndexThatNeedDelete = Field.normedBinaryIndexOf(this.subDocument.fields, startPosition + 1);
                if(this.fieldIndexThatNeedDelete < 0 || startPosition > this.subDocument.fields[this.fieldIndexThatNeedDelete].getFieldStartPosition())
                    this.fieldIndexThatNeedDelete++;
            }
            Field.deleteFieldByIndex(this.subDocument, this.fieldIndexThatNeedDelete, this.manipulator);
        }

        private getPreviousRunIndex(runIndex: number): number {
            runIndex--;
            if(runIndex < 0) {
                var currentChunk = this.subDocument.chunks[--this.currentChunkIndex];
                runIndex = currentChunk.textRuns.length - 1;
            }
            return runIndex;
        }

        // frequent situation. Here we see situation where [{some number simbols > 0}{some number simbols > 0}{some number simbols > 0}] - run
        // and we need delete mid-block
        private tryPackSelectionInOneRun(interval: FixedInterval, result: RemoveIntervalOperationResult): boolean {
            const runInfo: FullChunkAndRunInfo = this.subDocument.getRunAndIndexesByPosition(interval.start);
            const runStartPosition: number = runInfo.chunk.startLogPosition.value + runInfo.run.startOffset;
            const runEndPosition: number = runStartPosition + runInfo.run.length;
            const selectionEndPosition: number = interval.end();
            const selectionStartPosition: number = interval.start;
            if((runStartPosition < selectionStartPosition) && (selectionEndPosition < runEndPosition)) {
                const chunkStartPosition: number = runInfo.chunk.startLogPosition.value;
                const offsetStartSelectionAtChunk: number = selectionStartPosition - chunkStartPosition;
                const offsetEndSelectionAtChunk: number = selectionEndPosition - chunkStartPosition;
                result.registerItem(new HistoryRun(runInfo.run.type, runInfo.run.characterStyle, selectionStartPosition,
                    runInfo.run.maskedCharacterProperties, runInfo.chunk.getTextInChunk(offsetStartSelectionAtChunk, interval.length)));

                //in this case we not need delete paragraphs, sections, runs. We only correct length
                runInfo.run.length -= interval.length;
                runInfo.chunk.textBuffer = [runInfo.chunk.textBuffer.substr(0, offsetStartSelectionAtChunk), runInfo.chunk.textBuffer.substr(offsetEndSelectionAtChunk)].join('');

                const paragraphIndex: number = Utils.normedBinaryIndexOf(this.subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - selectionStartPosition);
                this.subDocument.paragraphs[paragraphIndex].length -= interval.length;

                const sectionIndex: number = Utils.normedBinaryIndexOf(this.subDocument.documentModel.sections, (s: Section) => s.startLogPosition.value - selectionStartPosition);
                this.modifySectionLength(this.subDocument.documentModel.sections[sectionIndex], -interval.length);

                TextManipulator.moveRunsInChunk(runInfo.chunk, runInfo.runIndex + 1, -interval.length);
                this.subDocument.positionManager.advance(selectionStartPosition, -interval.length);

                this.manipulator.dispatcher.notifyIntervalRemoved(this.subDocument, interval.start, interval.length);
                return true;
            }
            return false;
        }
        private canRemoveRun(run: TextRun, absolutePosition: number, isLastRunInParagraph: boolean): boolean {
            if(run.type === TextRunType.ParagraphRun) {
                this.cellsIterator.moveTo(absolutePosition);
                let currentCell = this.cellsIterator.getCurrent();
                if(currentCell && absolutePosition === currentCell.endParagrapPosition.value - 1)
                    return false;
                let nextCell = this.cellsIterator.getNext();
                if(nextCell && nextCell.startParagraphPosition.value === absolutePosition + 1) {
                    if(!isLastRunInParagraph)
                        return false;
                    let prevCell = this.cellsIterator.getPrev();
                    if(prevCell && prevCell.parentRow.parentTable.getLastCell() === prevCell)
                        return false;
                }
            }
            return true;
        }

        private fillResult(interval: FixedInterval, result: RemoveIntervalOperationResult) {
            var iterator: RunIterator = this.subDocument.getRunIterator(interval);
            var isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean = undefined;
            var lastParagraphRemovingLength = 0;
            while(iterator.moveNext()) {
                var currentRun: TextRun = iterator.currentRun;
                var currentChunk: Chunk = iterator.currentChunk;
                var currentInterval = iterator.currentInterval();
                lastParagraphRemovingLength += currentInterval.length;
                if(!this.canRemoveRun(currentRun, currentInterval.start, iterator.currentRun.paragraph.length === lastParagraphRemovingLength)) {
                    if(currentRun.type === TextRunType.ParagraphRun || currentRun.type === TextRunType.SectionRun)
                        lastParagraphRemovingLength = 0;
                    continue;
                }
                switch(currentRun.type) {
                    case TextRunType.TextRun:
                        result.registerItem(new HistoryRun(currentRun.type, currentRun.characterStyle, currentInterval.start,
                            currentRun.maskedCharacterProperties, currentChunk.getRunText(currentRun)));
                        break;
                    case TextRunType.ParagraphRun:
                    case TextRunType.SectionRun:
                        // first paragraph selected fully, then store his properties. Other case store next paragraph properties
                        if(isInsertPropertiesAndStyleIndexToCurrentParagraph === undefined)
                            isInsertPropertiesAndStyleIndexToCurrentParagraph = currentRun.paragraph.startLogPosition.value == interval.start ? true : false;
                        var paragraph: Paragraph;
                        if(isInsertPropertiesAndStyleIndexToCurrentParagraph)
                            paragraph = currentRun.paragraph;
                        else {
                            var nextParagraphIndex: number =
                                Utils.normedBinaryIndexOf(this.subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - currentRun.paragraph.startLogPosition.value) + 1;
                            paragraph = this.subDocument.paragraphs[nextParagraphIndex];
                        }
                        if(currentRun.type == TextRunType.ParagraphRun) {
                            result.registerItem(new HistoryRunParagraph(currentRun.type, currentRun.characterStyle, currentInterval.start,
                                currentRun.maskedCharacterProperties, currentChunk.getRunText(currentRun), paragraph.paragraphStyle, paragraph.maskedParagraphProperties,
                                isInsertPropertiesAndStyleIndexToCurrentParagraph, paragraph.numberingListIndex, paragraph.listLevelIndex, paragraph.tabs.clone()));
                        }
                        else {
                            result.registerItem(new HistoryRunSection(currentRun, paragraph, iterator.currentSection, currentInterval.start,
                                isInsertPropertiesAndStyleIndexToCurrentParagraph));
                        }
                        lastParagraphRemovingLength = 0;
                        break;
                    case TextRunType.InlinePictureRun:
                        var currentPictureRun: InlinePictureRun = <InlinePictureRun>currentRun;
                        if(!(currentPictureRun instanceof InlinePictureRun))
                            throw new Error("In TexManipulator.getHistoryRunsFromInterval currentPictureRun not have type InlinePictureRun");
                        result.registerItem(new HistoryRunInlinePicture(currentPictureRun.characterStyle, currentInterval.start,
                            currentPictureRun.maskedCharacterProperties, currentPictureRun.id, currentPictureRun.originalWidth, currentPictureRun.originalHeight, currentPictureRun.scaleX,
                            currentPictureRun.scaleY, currentPictureRun.lockAspectRatio));
                        break;
                    case TextRunType.FieldCodeStartRun:
                        var globalOffset: number = currentInterval.start;
                        var fieldIndex: number = Field.normedBinaryIndexOf(this.subDocument.fields, globalOffset + 1);
                        var field: Field = this.subDocument.fields[fieldIndex];
                        result.registerItem(new HistoryRunFieldCodeStart(currentRun.type, currentRun.characterStyle, globalOffset, currentRun.maskedCharacterProperties,
                            currentChunk.getRunText(currentRun), field.showCode, field.getFieldStartPosition(), field.getSeparatorPosition(), field.getFieldEndPosition(),
                            field.getHyperlinkInfo() ? field.getHyperlinkInfo().clone() : undefined));
                        break;
                    case TextRunType.FieldCodeEndRun:
                        result.registerItem(new HistoryRunFieldCodeEnd(currentRun.type, currentRun.characterStyle, currentInterval.start,
                            currentRun.maskedCharacterProperties, currentChunk.getRunText(currentRun)));
                        break;
                    case TextRunType.FieldResultEndRun:
                        result.registerItem(new HistoryRunFieldResultEnd(currentRun.type, currentRun.characterStyle, currentInterval.start,
                            currentRun.maskedCharacterProperties, currentChunk.getRunText(currentRun)));
                        break;
                }
            }
            iterator.reset();
            this.cellsIterator.reset();
        }

        private removeAccumulatedInterval(removingInterval: FixedInterval, newPosition: number, advanceDelta: number) {
            advanceDelta -= removingInterval.length;
            if(Math.abs(advanceDelta) > 0)
                this.subDocument.positionManager.advance(removingInterval.start, advanceDelta);
            if(removingInterval.length)
                this.manipulator.dispatcher.notifyIntervalRemoved(this.subDocument, removingInterval.start, removingInterval.length);
            removingInterval.start = newPosition;
            removingInterval.length = 0;
        }

        private mergeParagraphsInternal(subDocument: SubDocument, paragraphIndex: number, setPropertiesSecondParagraph: boolean) {
            var firstParagraph = subDocument.paragraphs[paragraphIndex];
            var lastParagraph = subDocument.paragraphs[paragraphIndex + 1];
            var runInfo = subDocument.getRunAndIndexesByPosition(lastParagraph.startLogPosition.value - 1);
            if(setPropertiesSecondParagraph)
                this.copyPropertiesToParagraph(firstParagraph, lastParagraph);
            var runs: TextRun[] = subDocument.getRunsByInterval(new FixedInterval(lastParagraph.startLogPosition.value, lastParagraph.length));
            var currentChunkIndex = this.currentChunkIndex;
            var chunkIndexDelta = this.currentChunkIndex - runInfo.chunkIndex;
            this.currentChunkIndex = runInfo.chunkIndex;
            this.removeRunInternal(subDocument, runInfo.runIndex);
            this.currentChunkIndex += chunkIndexDelta
            firstParagraph.length--;
            for(var i: number = 0, run: TextRun; run = runs[i]; i++) {
                run.paragraph = firstParagraph;
                run.onCharacterPropertiesChanged();
                firstParagraph.length += run.length;
            }
            subDocument.positionManager.advance(lastParagraph.startLogPosition.value - 1, -1);
            subDocument.positionManager.unregisterPosition(lastParagraph.startLogPosition);
            subDocument.paragraphs.splice(paragraphIndex + 1, 1);
        }

        private removeRunInternal(subDocument: SubDocument, runIndex: number) {
            var currentChunk = subDocument.chunks[this.currentChunkIndex];
            var currentRun = currentChunk.textRuns[runIndex];
            currentChunk.textBuffer = currentChunk.textBuffer.substr(0, currentRun.startOffset) + currentChunk.textBuffer.substr(currentRun.startOffset + currentRun.length);
            currentChunk.textRuns.splice(runIndex, 1);
            TextManipulator.moveRunsInChunk(currentChunk, runIndex, -currentRun.length);
            if(currentChunk.textRuns.length === 0) {
                subDocument.positionManager.unregisterPosition(currentChunk.startLogPosition);
                subDocument.chunks.splice(this.currentChunkIndex--, 1);
            }
        }

        private copyPropertiesToParagraph(to: Paragraph, from: Paragraph) {
            to.paragraphStyle = from.paragraphStyle;
            to.setParagraphProperties(from.maskedParagraphProperties);
            if(from.hasParagraphMergedProperies())
                to.setParagraphMergedProperies(from.getParagraphMergedProperies());
            to.numberingListIndex = from.numberingListIndex;
            to.listLevelIndex = from.listLevelIndex;
            to.tabs = from.tabs.clone();
        }
    }

    export class RemoveIntervalOperationResult {
        historyRuns: HistoryRun[] = [];
        private nestingLevels: number[] = [];
        private cellsIterator: SelectedCellsIterator;
        private currentCellIndex = 0;
        private sourceInterval: FixedInterval;

        constructor(sourceInterval: FixedInterval, cellsIterator: SelectedCellsIterator) {
            this.sourceInterval = sourceInterval;
            this.cellsIterator = cellsIterator;
        }
        registerItem(historyRun: HistoryRun) {
            this.cellsIterator.moveTo(historyRun.offsetAtStartDocument);
            var cell = this.cellsIterator.getCurrent();
            this.registerItemCore(historyRun, cell ? cell.parentRow.parentTable.nestedLevel : -1);
        }
        registerItemCore(historyRun: HistoryRun, nestingLevel: number) {
            this.historyRuns.push(historyRun);
            this.nestingLevels.push(nestingLevel);
        }
        removeLastParagraphRun() {
            var lastIndex = this.historyRuns.length - 1;
            var lastHistoryRun = this.historyRuns[lastIndex];
            if(lastHistoryRun && lastHistoryRun.type == TextRunType.ParagraphRun) {
                this.historyRuns.splice(lastIndex, 1);
                this.nestingLevels.splice(lastIndex, 1);
            }
        }
        getIterator(): RemoveIntervalOperationResultIterator {
            return new RemoveIntervalOperationResultIterator(this.historyRuns, this.nestingLevels);
        }
    }

    export class SelectedCellsIterator {
        cells: TableCell[];
        private current: number = 0;
        private position: number;
        constructor(subDocument: SubDocument, interval: FixedInterval) {
            this.cells = SelectedCellsIterator.getCellsByInterval(subDocument, interval);
        }
        moveTo(position: number): boolean {
            if(position < this.position)
                this.reset();
            this.position = position;
            let cell: TableCell;
            while(cell = this.cells[this.current]) {
                if(position >= cell.endParagrapPosition.value)
                    this.current++;
                else
                    return true;
            }
            return false;
        }
        getCurrent(): TableCell {
            let cell = this.cells[this.current];
            return cell ? SelectedCellsIterator.correctCurrent(this.position, cell) : null;
        }
        getPrev() {
            let cell = this.cells[this.current];
            if(cell && this.position >= cell.endParagrapPosition.value)
                return cell;
            return this.cells[this.current - 1] || null;
        }
        getNext() {
            let cell = this.cells[this.current];
            if(cell && this.position < cell.startParagraphPosition.value)
                return cell;
            return this.cells[this.current + 1] || null;
        }
        reset() {
            this.current = 0;
            this.position = 0;
        }
        private static getCellsByInterval(subDocument: SubDocument, interval: FixedInterval): TableCell[] {
            if(subDocument.tables.length === 0)
                return [];
            let table = subDocument.tablesByLevels[0][Math.max(0, Utils.normedBinaryIndexOf(subDocument.tablesByLevels[0], t => t.getStartPosition() - interval.start))];
            let intervalEnd = interval.end();
            if(intervalEnd < table.getStartPosition())
                return [];
            let result: TableCell[] = [];
            this.collectCellsByIntervalCore(subDocument, result, table, interval.start, intervalEnd);
            return result.sort((c1, c2) => c1.endParagrapPosition.value - c2.endParagrapPosition.value);
        }
        private static collectCellsByIntervalCore(subDocument: SubDocument, result: TableCell[], table: Table, intervalStart: number, intervalEnd: number) {
            let nextTable = subDocument.tables[table.index + 1];
            if(nextTable && nextTable.getStartPosition() <= intervalEnd)
                this.collectCellsByIntervalCore(subDocument, result, nextTable, intervalStart, intervalEnd);
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++) {
                    if(intervalStart < cell.endParagrapPosition.value && intervalEnd > cell.startParagraphPosition.value)
                        result.push(cell);
                    else if(intervalStart === cell.endParagrapPosition.value && cell.parentRow.parentTable.getLastCell() === cell)
                        result.push(cell);
                    else if(cell.parentRow.parentTable.getFirstCell() === cell && cell.startParagraphPosition.value === intervalEnd)
                        result.push(cell);
                    else if(cell.startParagraphPosition.value > intervalEnd)
                        return;
                }
            }
        }
        private static correctCurrent(position, cell: TableCell): TableCell {
            if(position >= cell.startParagraphPosition.value && position < cell.endParagrapPosition.value)
                return cell;
            if(position < cell.startParagraphPosition.value && cell.parentRow.parentTable.parentCell)
                return this.correctCurrent(position, cell.parentRow.parentTable.parentCell);
            return null;
        }
    }

    export class RemoveIntervalOperationResultIterator {
        private historyRuns: HistoryRun[];
        private nestingLevels: number[];
        private position: number = -1;
        constructor(historyRuns: HistoryRun[], nestingLevels: number[]) {
            this.historyRuns = historyRuns;
            this.nestingLevels = nestingLevels;
        }

        currentHistoryRun: HistoryRun;
        currentNestingLevel: number;

        moveNext(): boolean {
            this.position++;
            this.currentHistoryRun = this.historyRuns[this.position];
            this.currentNestingLevel = this.nestingLevels[this.position];
            return !!this.currentHistoryRun;
        }
    }
} 