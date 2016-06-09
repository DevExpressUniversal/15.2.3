module __aspxRichEdit {
    export class SubDocument implements IParagraphPropertiesProvider {
        static AUTOGENERATE_ID = -1;
        static MAIN_SUBDOCUMENT_ID = 0;
        id: number;
        documentModel: DocumentModel;
        chunks: Chunk[] = [];
        paragraphs: Paragraph[] = [];
        fields: Field[] = [];
        tables: Table[] = [];
        tablesByLevels: Table[][] = []; // first index - nestedLevel. Second num table in level. Sorted. Need keep actually
                                        // Need for exclude binary serch for boxIterator
        positionManager: PositionManager = new PositionManager();
        fieldsWaitingForUpdate: FieldsWaitingForUpdate = null;
        info: SubDocumentInfoBase;
        bookmarks: Bookmark[] = [];

        constructor(documentModel: DocumentModel, subDocumentInfo: SubDocumentInfoBase) {
            this.documentModel = documentModel;
            this.id = subDocumentInfo.subDocumentId;
            this.info = subDocumentInfo;
            documentModel.subDocuments[this.id] = this;
        }

        //************************************************************* GET CHUNK(S) *************************************************************
        public getLastChunk(): Chunk {
            return this.chunks[this.chunks.length - 1];
        }

        public getFirstChunk(): Chunk {
            return this.chunks[0];
        }

        public getText(interval: FixedInterval): string {
            var buffer = "";
            var chunkIndex = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - interval.start);
            var chunk: Chunk = this.chunks[chunkIndex];
            var remainLength = interval.length;
            var offset = interval.start - chunk.startLogPosition.value;
            while (chunk) {
                var length = Math.min(chunk.textBuffer.length - offset, remainLength);
                buffer += chunk.textBuffer.substr(offset, length);
                remainLength -= length;
                if (remainLength === 0)
                    break;
                chunk = this.chunks[++chunkIndex];
                offset = 0;
            }
            return buffer;
        }

        //************************************************************* GET RUN(S) *************************************************************

        // [run1][run2]
        //   ^ - split
        // [ ][ ][run3]
        public splitRun(position: number) {
            var info: FullChunkAndRunInfo = this.getRunAndIndexesByPosition(position);
            var offset: number = position - (info.chunk.startLogPosition.value + info.run.startOffset);
            if (offset > 0)
                info.chunk.splitRun(info.runIndex, offset);
        }

        public getLastRun(): TextRun {
            var lastChunk: Chunk = this.getLastChunk();
            return lastChunk.textRuns[lastChunk.textRuns.length - 1];
        }

        public getFirstRun(): TextRun {
            return this.chunks[0].textRuns[0];
        }

        public getRunIterator(interval: FixedInterval): RunIterator {
            return this.getRunIteratorInternal(interval, false);
        }

        //changes in any structure iterator NOT ALLOWED! Only read!
        public getConstRunIterator(interval: FixedInterval): RunIterator {
            return this.getRunIteratorInternal(interval, true);
        }

        // isConstRunIterator mean we dont touch document model
        private getRunIteratorInternal(interval: FixedInterval, isConstRunIterator: boolean): RunIterator {
            if (interval.length == 0)
                return new RunIterator(interval, [], [], [], [1], [1]);

            var runs: TextRun[] = [],
                chunks: Chunk[] = [],
                sections: Section[] = [],
                indexForChunks: number[] = [],
                indexForSections: number[] = [];

            var currentChunkIndex: number = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - interval.start),
                currentChunk: Chunk = this.chunks[currentChunkIndex];
            chunks.push(currentChunk);

            var currentRunIndex: number = Utils.normedBinaryIndexOf(currentChunk.textRuns, (r: TextRun) => currentChunk.startLogPosition.value + r.startOffset - interval.start),
                currentRun: TextRun = currentChunk.textRuns[currentRunIndex],
                remainIntervalLength: number = interval.length;

            if (currentChunk.startLogPosition.value + currentRun.startOffset < interval.start) {
                if (isConstRunIterator)
                    remainIntervalLength += interval.start - (currentChunk.startLogPosition.value + currentRun.startOffset);
                else {
                    currentChunk.splitRun(currentRunIndex, interval.start - (currentChunk.startLogPosition.value + currentRun.startOffset));
                    currentRunIndex++;
                }
            }

            var currentSectionIndex: number = Utils.normedBinaryIndexOf(this.documentModel.sections, (s: Section) => s.startLogPosition.value - interval.start);
            var currentSection: Section = this.documentModel.sections[currentSectionIndex];
            sections.push(currentSection);

            var runIndex: number = 0;
            var sectionIndex: number = 0;

            var remainSectionLength: number = currentSection.startLogPosition.value + currentSection.getLength() - interval.start;
            while (currentRun = currentChunk.textRuns[currentRunIndex]) {
                if (remainSectionLength === 0 && this.isMain()) {
                    currentSectionIndex++;
                    currentSection = this.documentModel.sections[currentSectionIndex];
                    sections.push(currentSection);
                    indexForSections.push(sectionIndex - 1);
                    remainSectionLength = currentSection.getLength();
                }

                if (currentRun.length > remainIntervalLength) {
                    if (isConstRunIterator) {
                        runs.push(TextRun.create(Math.max(currentRun.startOffset, interval.start - currentChunk.startLogPosition.value), Math.min(remainIntervalLength, interval.length),
                            currentRun.type, currentRun.paragraph, currentRun.characterStyle, currentRun.maskedCharacterProperties, currentRun));
                    }
                    else {
                        currentChunk.splitRun(currentRunIndex, remainIntervalLength);
                        runs.push(currentRun);
                    }
                    break;
                }

                if (runIndex == 0 && isConstRunIterator && currentRun.startOffset + currentChunk.startLogPosition.value < interval.start) {
                    var runEndPosition: number = currentChunk.startLogPosition.value + currentRun.startOffset + currentRun.length,
                        newRunLength: number = runEndPosition - interval.start;
                    runs.push(TextRun.create(interval.start - currentChunk.startLogPosition.value, newRunLength, currentRun.type, currentRun.paragraph,
                        currentRun.characterStyle, currentRun.maskedCharacterProperties, currentRun));
                    remainSectionLength -= newRunLength;
                }
                else {
                    runs.push(currentRun);
                    remainSectionLength -= currentRun.length;
                }
                remainIntervalLength -= currentRun.length;
                currentRunIndex++;

                if (!remainIntervalLength)
                    break;
                if (currentRunIndex == currentChunk.textRuns.length) {
                    currentChunkIndex++;
                    indexForChunks.push(runIndex);
                    currentChunk = this.chunks[currentChunkIndex];
                    chunks.push(currentChunk);
                    currentRunIndex = 0;
                }
                sectionIndex++;
                runIndex++;
            }
            indexForChunks.push(runIndex + 1);
            indexForSections.push(sectionIndex + 1);
            return new RunIterator(interval, runs, chunks, sections, indexForChunks, indexForSections);
        }

        // no split runs
        public getRunsByInterval(interval: FixedInterval): TextRun[] {
            var runs: TextRun[] = [];
            var chunkIndex: number = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - interval.start);
            var chunk: Chunk = this.chunks[chunkIndex];

            var runIndex: number = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => chunk.startLogPosition.value + r.startOffset - interval.start);
            var run: TextRun;
            var length: number = interval.length;
            var runOffset: number = interval.start - chunk.textRuns[runIndex].startOffset - chunk.startLogPosition.value;

            while (chunk && (run = chunk.textRuns[runIndex])) {
                runs.push(run);
                length -= (run.length - runOffset);
                if (length <= 0) break;
                runIndex++;
                if (runIndex >= chunk.textRuns.length) {
                    runIndex = 0;
                    chunkIndex++;
                    chunk = this.chunks[chunkIndex];
                }
                runOffset = 0;
            }
            return runs;
        }

        public getRunByPosition(position: number): TextRun {
            var chunkIndex: number = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - position);
            var chunk: Chunk = this.chunks[chunkIndex];

            var runIndex: number = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => chunk.startLogPosition.value + r.startOffset - position);
            return chunk.textRuns[runIndex];
        }
        
        public getRunAndIndexesByPosition(position: number): FullChunkAndRunInfo {
            var chunkIndex: number = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - position);
            var chunk: Chunk = this.chunks[chunkIndex];

            var runOffset: number = position - chunk.startLogPosition.value;
            var runIndex: number = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => r.startOffset - runOffset);
            var run: TextRun = chunk.textRuns[runIndex];
            return new FullChunkAndRunInfo(chunkIndex, chunk, runIndex, run);
        }

        public getInlinePictureRunById(id: number): InlinePictureRun {
            for(var i = 0, chunk: Chunk; chunk = this.chunks[i]; i++) {
                for(var j = 0, run: TextRun; run = chunk.textRuns[j]; j++) {
                    var pictureRun = <InlinePictureRun>run;
                    if(pictureRun instanceof InlinePictureRun && pictureRun.id == id)
                        return pictureRun;
                }
            }
            return null;
        }

        //************************************************************* GET SECTION(S) *************************************************************
        public getSectionByPosition(position: number): Section {
            return this.documentModel.sections[Utils.normedBinaryIndexOf(this.documentModel.sections, (s: Section) => s.startLogPosition.value - position) ];
        }

        //************************************************************* GET PARAGRAPHS(S) *************************************************************
        public getParagraphByPosition(position: number): Paragraph {
            return this.paragraphs[Utils.normedBinaryIndexOf(this.paragraphs, (p: Paragraph) => p.startLogPosition.value - position) ];
        }
        public getParagraphIndexByPosition(position: number): number {
            return Utils.normedBinaryIndexOf(this.paragraphs, (p: Paragraph) => p.startLogPosition.value - position);
        }

        public getParagraphsIndices(interval: FixedInterval): { start: number; end: number } {
            var endPosition = Math.max(interval.start, interval.end() - 1);
            var startParagraphIndex = Utils.normedBinaryIndexOf(this.paragraphs, paragraph => paragraph.startLogPosition.value - interval.start);
            var endParagraphIndex = endPosition > interval.start ? Utils.normedBinaryIndexOf(this.paragraphs, paragraph => paragraph.startLogPosition.value - endPosition) : startParagraphIndex;
            return {
                start: startParagraphIndex,
                end: endParagraphIndex
            };
        }

        public getParagraphsByInterval(interval: FixedInterval): Paragraph[] {
            var paragraphs: Paragraph[] = [],
                intervalEnd: number = interval.end(),
                paragraphIndex: number = Utils.normedBinaryIndexOf(this.paragraphs, (p: Paragraph) => p.startLogPosition.value - interval.start);
            paragraphs.push(this.paragraphs[paragraphIndex++]);
            for(var paragraph: Paragraph; paragraph = this.paragraphs[paragraphIndex]; paragraphIndex++) {
                if(paragraph.startLogPosition.value < intervalEnd)
                    paragraphs.push(paragraph);
                else
                    break;
            }
            return paragraphs;
        }

        public getParagraphIndicesByIntervals(intervals: FixedInterval[]): number[] {
            let result: number[] = [];
            for(var i = 0, interval: FixedInterval; interval = intervals[i]; i++) {
                let paragraphIndex: number = Utils.normedBinaryIndexOf(this.paragraphs, (p: Paragraph) => p.startLogPosition.value - interval.start);
                let intervalEnd: number = interval.end();
                result.push(paragraphIndex++);
                for(let paragraph: Paragraph; paragraph = this.paragraphs[paragraphIndex]; paragraphIndex++) {
                    if(paragraph.startLogPosition.value < intervalEnd)
                        result.push(paragraphIndex);
                    else
                        break;
                }
            }
            return Utils.sortAndDisctinctNumbers(result);
        }

        //************************************************************* GET SOME POSITIONS(S) *************************************************************
        public getDocumentEndPosition(): number {
            return this.info.getEndPosition(this.documentModel);
        }

        public getWholeWordInterval(position: number, checkRunProperties: boolean = false, includeBounds: boolean = false): FixedInterval {
            var interval = new FixedInterval(position, 0);
            if (!includeBounds && (position == this.getDocumentEndPosition() || position == 0))
                return interval;

            var chunkIndex = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - position);
            var runOffset = position - this.chunks[chunkIndex].startLogPosition.value;
            var runIndex = Utils.normedBinaryIndexOf(this.chunks[chunkIndex].textRuns, (r: TextRun) => r.startOffset - runOffset);

            var end = this.getWordEnd(position, checkRunProperties, chunkIndex, runIndex);
            if(end == position && !includeBounds)
                return interval;
            var start = this.getWordStart(position, checkRunProperties, this.chunks[chunkIndex].textRuns[runIndex]);
            if(start == position && !includeBounds)
                return interval;
            interval.start = start;
            interval.length = end - start;
            return interval;
        }

        private getWordStart(position: number, checkRunProperties: boolean, prevRun: TextRun): number {
            if(position <= 0)
                return position;

            var start = position;
            position--;

            var chunkIndex = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - position);
            var chunk = this.chunks[chunkIndex];
            var runIndex = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => r.startOffset - (position - chunk.startLogPosition.value));
            var run = chunk.textRuns[runIndex];

            while (chunk && run && run.type === TextRunType.TextRun && chunk.textBuffer[position - chunk.startLogPosition.value].match(Utils.isAlphanumeric) &&
                (!checkRunProperties || prevRun === run || prevRun.maskedCharacterProperties.equals(run.maskedCharacterProperties))) {
                start = position;
                position--;
                if(position - chunk.startLogPosition.value - run.startOffset < 0) {
                    prevRun = run;
                    runIndex--;
                    if(runIndex >= 0)
                        run = chunk.textRuns[runIndex];
                    else {
                        chunk = this.chunks[--chunkIndex];
                        if(!chunk)
                            break;
                        runIndex = chunk.textRuns.length - 1;
                        run = chunk.textRuns[runIndex];
                    }
                }
            }
            return start;
        }

        private getWordEnd(position: number, checkRunProperties: boolean, chunkIndex: number, runIndex: number): number {
            if(position == this.getDocumentEndPosition())
                return position;
            var chunk = this.chunks[chunkIndex];
            var run = chunk.textRuns[runIndex];
            var end = position;
            var prevRun = run;
            
            while (chunk && run && run.type === TextRunType.TextRun && chunk.textBuffer[position - chunk.startLogPosition.value].match(Utils.isAlphanumeric) &&
                (!checkRunProperties || prevRun === run || prevRun.maskedCharacterProperties.equals(run.maskedCharacterProperties))) {
                position++;
                end = position;

                if(position === chunk.startLogPosition.value + run.startOffset + run.length) {
                    prevRun = run;
                    run = chunk.textRuns[++runIndex];
                    if(!run) {
                        chunk = this.chunks[++chunkIndex];
                        if(chunk) {
                            runIndex = 0;
                            run = chunk.textRuns[runIndex];
                        }
                        else
                            break;
                    }
                }
            }
            return end;
        }

        resetMergedFormattingCache(type: ResetFormattingCacheType) {
            if(type & ResetFormattingCacheType.Paragraph) {
                for(var i = 0, paragraph: Paragraph; paragraph = this.paragraphs[i]; i++) {
                    paragraph.resetParagraphMergedProperties();
                }
            }
            if(type & ResetFormattingCacheType.Character) {
                for(var i = 0, chunk: Chunk; chunk = this.chunks[i]; i++) {
                    for(var j = 0, run: TextRun; run = chunk.textRuns[j]; j++) {
                        run.resetCharacterMergedProperties();
                    }
                }
            }
        }

        // for tests
        getParagraphProperties(paragraph: Paragraph): ParagraphProperties {
            return paragraph.getParagraphMergedProperies();
        }

        isMain(): boolean { return this.info.isMain; }
        isHeaderFooter(): boolean { return this.info.isHeaderFooter; }
        isFooter(): boolean { return this.info.isFooter; }
        isHeader(): boolean { return this.info.isHeader; }
        isNote(): boolean { return this.info.isNote; }
        isFootNote(): boolean { return this.info.isFootNote; }
        isEndNote(): boolean { return this.info.isEndNote; }
        isTextBox(): boolean { return this.info.isTextBox; }
        isComment(): boolean { return this.info.isComment; }
        isReferenced(): boolean { return this.info.isReferenced; }
    }
}