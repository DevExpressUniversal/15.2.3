module __aspxRichEdit {
    export class InputPosition {
        // IMPORTANT!!!
        // Always clone all this objects if you want add it to cache
        // "raw": if some interval have different property value, then here property have UNDEFINED value
        // "full": ---------------------------------------------------------------------- DEFAULT ------- 

        private model: DocumentModel;

        private selection: Selection;
        private intervals: FixedInterval[];
        private subDocument: SubDocument;

        private sourceRun: TextRun;

        private characterStyle: CharacterStyle;
        private maskedCharacterProperties: MaskedCharacterProperties; 

        private mergedCharacterPropertiesRaw: CharacterProperties;
        private mergedCharacterPropertiesFull: CharacterProperties;

        private mergedParagraphPropertiesRaw: ParagraphProperties;
        private mergedParagraphPropertiesFull: ParagraphProperties;

        private mergedSectionPropertiesRaw: SectionProperties;
        private mergedSectionPropertiesFull: SectionProperties;

        constructor(selection: Selection, model: DocumentModel) {
            this.model = model;

            this.selection = selection;
            this.intervals = selection.getIntervalsClone();
            this.subDocument = model.activeSubDocument;
            
            this.sourceRun = null;

            this.characterStyle = null;
            this.maskedCharacterProperties = new MaskedCharacterProperties();

            this.mergedCharacterPropertiesRaw = new CharacterProperties();
            this.mergedCharacterPropertiesFull = this.mergedCharacterPropertiesRaw.clone();

            this.mergedParagraphPropertiesRaw = new ParagraphProperties();
            this.mergedParagraphPropertiesFull = this.mergedParagraphPropertiesRaw.clone();

            this.mergedSectionPropertiesRaw = new SectionProperties();
            this.mergedSectionPropertiesFull = this.mergedSectionPropertiesRaw.clone();
        }

        public reset() {
            this.intervals = this.selection.getIntervalsClone();
            this.subDocument = this.model.activeSubDocument;
            this.resetReturnValues();
        }

        public resetSectionMergedProperties() {
            this.mergedSectionPropertiesRaw = null;
            this.mergedSectionPropertiesFull = null;
        }

        public resetParagraphMergedProperties() {
            this.mergedParagraphPropertiesRaw = null;
            this.mergedParagraphPropertiesFull = null;
        }

        public setPropertiesFromPosition(subDocument: SubDocument, position: number) {
            this.intervals = [new FixedInterval(position, 0)];
            this.subDocument = subDocument;
            this.resetReturnValues();
        }

        public getCharacterStyle(): CharacterStyle {
            if (!this.characterStyle)
                this.characterStyle = this.getCharacterStyleInternal();
            return this.characterStyle;
        }

        public setCharacterStyle(characterStyle: CharacterStyle) {
            this.characterStyle = characterStyle;
        }

        public getMaskedCharacterProperties(): MaskedCharacterProperties {
            if (!this.maskedCharacterProperties) {
                this.setSourceRun();
                this.maskedCharacterProperties = this.sourceRun.maskedCharacterProperties.clone();
            }
            return this.maskedCharacterProperties;
        }

        public getMergedCharacterPropertiesRaw(): CharacterProperties {
            if (!this.mergedCharacterPropertiesRaw)
                this.setMergedCharacterAndParagraphPropertiesRaw();
            return this.mergedCharacterPropertiesRaw;
        }

        public getMergedCharacterPropertiesFull(): CharacterProperties {
            if (!this.mergedCharacterPropertiesFull)
                this.mergedCharacterPropertiesFull = InputPosition.mergePropertiesFull(this.getMergedCharacterPropertiesRaw(), CharacterProperties.fieldsInfoToCompare);
            return this.mergedCharacterPropertiesFull;
        }

        public getMergedParagraphPropertiesRaw(): ParagraphProperties {
            if (!this.mergedParagraphPropertiesRaw)
                this.setMergedCharacterAndParagraphPropertiesRaw();
            return this.mergedParagraphPropertiesRaw;
        }

        public getMergedParagraphPropertiesFull(): ParagraphProperties {
            if (!this.mergedParagraphPropertiesFull)
                this.mergedParagraphPropertiesFull = InputPosition.mergePropertiesFull(this.getMergedParagraphPropertiesRaw(), ParagraphProperties.fieldsInfoToCompare);
            return this.mergedParagraphPropertiesFull;
        }

        public getMergedSectionPropertiesRaw(): SectionProperties {
            if (!this.mergedSectionPropertiesRaw) {
                var interval: FixedInterval = this.getLastInterval();
                var intervalStartPosition: number = interval.start;
                var intervalEndPosition: number = interval.end();
                var sections: Section[] = this.model.sections;
                var sectionIndex: number = Utils.normedBinaryIndexOf(sections, (s: Section) => s.startLogPosition.value - intervalStartPosition);

                this.mergedSectionPropertiesRaw = sections[sectionIndex++].sectionProperties.clone();
                for (var section: Section; (section = sections[sectionIndex]) && (section.startLogPosition.value < intervalEndPosition); sectionIndex++)
                    InputPosition.mergePropertiesRaw(this.mergedSectionPropertiesRaw, section.sectionProperties, SectionProperties.fieldsInfoToCompare);
            }
            return this.mergedSectionPropertiesRaw;
        }

        public getMergedSectionPropertiesFull(): SectionProperties {
            if (!this.mergedSectionPropertiesFull)
                this.mergedSectionPropertiesFull = InputPosition.mergePropertiesFull(this.getMergedSectionPropertiesRaw(), SectionProperties.fieldsInfoToCompare);
            return this.mergedSectionPropertiesFull;
        }

        private setMergedCharacterAndParagraphPropertiesRaw() {
            this.setSourceRun();

            this.mergedCharacterPropertiesRaw = this.sourceRun.getCharacterMergedProperies().clone();
            this.mergedParagraphPropertiesRaw = this.sourceRun.paragraph.getParagraphMergedProperies().clone();

            var interval: FixedInterval = this.getLastInterval();
            var intervalStartPosition: number = interval.start;
            var intervalEndPosition: number = interval.end();

            var chunks: Chunk[] = this.subDocument.chunks;
            var chunkIndex: number = Utils.normedBinaryIndexOf(chunks, (c: Chunk) => c.startLogPosition.value - intervalStartPosition);
            var chunk: Chunk = chunks[chunkIndex];
            var runIndex: number = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => chunk.startLogPosition.value + r.startOffset - intervalStartPosition);
            var prevParagraph: Paragraph = this.sourceRun.paragraph;

            exitBothLoops:
            for (; chunk = chunks[chunkIndex]; chunkIndex++) {
                for (var run: TextRun; run = chunk.textRuns[runIndex]; runIndex++) {
                    if (chunk.startLogPosition.value + run.startOffset >= intervalEndPosition)
                        break exitBothLoops;
                    if (run !== this.sourceRun) {
                        if (run.type === TextRunType.TextRun)
                            InputPosition.mergePropertiesRaw(this.mergedCharacterPropertiesRaw, run.getCharacterMergedProperies(), CharacterProperties.fieldsInfoToCompare);
                        // simple merge all paragraphs
                        if (prevParagraph !== run.paragraph) {
                            InputPosition.mergePropertiesRaw(this.mergedParagraphPropertiesRaw, run.paragraph.getParagraphMergedProperies(), ParagraphProperties.fieldsInfoToCompare);
                            prevParagraph = run.paragraph;
                        }
                    }
                }
            }
        }

        // for merge Character, Paragraph, Section and other properties
        private static mergePropertiesRaw(sourceProps: any, otherProps: any, cmpInfos: InputPositionCompareInfo[]) {
            for (var i: number = 0, cmpInfo: InputPositionCompareInfo; cmpInfo = cmpInfos[i]; i++) {
                var propName: string = cmpInfo.name;
                if (!cmpInfo.comparer(sourceProps[propName], otherProps[propName]))
                    sourceProps[propName] = undefined;
            }
        }

        // change undefined property values to default
        private static mergePropertiesFull(sourceProps: any, cmpInfos: InputPositionCompareInfo[]): any {
            var result: any = sourceProps.clone();
            for (var i: number = 0, cmpInfo: InputPositionCompareInfo; cmpInfo = cmpInfos[i]; i++) {
                var propName: string = cmpInfo.name;
                if (result[propName] === undefined)
                    result[propName] = cmpInfo.defaultValue;
            }
            return result;
        }

        private getCharacterStyleInternal(): CharacterStyle {
            var interval: FixedInterval = this.getLastInterval();
            if (interval.length == 0)
                return this.getCharacterStyleCollapsedIntervalInternal(interval.start);

            var chunks: Chunk[] = this.subDocument.chunks;
            var intervalStartPosition: number = interval.start;
            var intervalEndPosition: number = interval.end();

            var firstRun: FullChunkAndRunInfo = this.subDocument.getRunAndIndexesByPosition(intervalStartPosition);
            if (intervalStartPosition == firstRun.run.paragraph.startLogPosition.value && firstRun.run.paragraph.length > 1)
                return firstRun.run.characterStyle;

            if (intervalStartPosition > 0) {
                var prevFirstRun: FullChunkAndRunInfo = this.subDocument.getRunAndIndexesByPosition(intervalStartPosition - 1);
                if (prevFirstRun.run.type == TextRunType.TextRun)
                    return prevFirstRun.run.characterStyle;
            }

            for (var chunk: Chunk, chunkIndex: number = firstRun.chunkIndex; chunk = chunks[chunkIndex]; chunkIndex++) {
                for (var run: TextRun, runIndex: number = firstRun.runIndex; run = chunk.textRuns[runIndex]; runIndex++) {
                    if (chunk.startLogPosition.value + run.startOffset >= intervalEndPosition)
                        return firstRun.run.characterStyle;
                    if (run.type == TextRunType.TextRun)
                        return run.characterStyle;
                }
            }
            return firstRun.run.characterStyle;
        }

        private getCharacterStyleCollapsedIntervalInternal(intervalStartPosition: number): CharacterStyle {
            var chunks: Chunk[] = this.subDocument.chunks;

            if (intervalStartPosition == 0)
                return chunks[0].textRuns[0].characterStyle; // run type not matter

            var prevFirstRun: { chunkIndex: number; runIndex: number; chunk: Chunk; run: TextRun } = this.subDocument.getRunAndIndexesByPosition(intervalStartPosition - 1);
            if (prevFirstRun.run.type != TextRunType.ParagraphRun && prevFirstRun.run.type != TextRunType.SectionRun)
                return prevFirstRun.run.characterStyle;

            // take properties next run, not matter what type he have
            var indexNextRun: number = prevFirstRun.runIndex + 1;
            if (indexNextRun < prevFirstRun.chunk.textRuns.length)
                return prevFirstRun.chunk.textRuns[indexNextRun].characterStyle;
            if (prevFirstRun.chunkIndex + 1 < chunks.length)
                return chunks[prevFirstRun.chunkIndex + 1].textRuns[0].characterStyle;
            return prevFirstRun.run.characterStyle;
        }

        private setSourceRun() {
            if (this.sourceRun)
                return;

            var interval: FixedInterval = this.getLastInterval();

            if (interval.length == 0) {
                var chunks: Chunk[] = this.subDocument.chunks;
                if (interval.start == 0) {
                    this.sourceRun = chunks[0] ? chunks[0].textRuns[0].clone() : null;
                    return;
                }

                var prevAbsolutePosition: number = interval.start - 1; // take properties prev run if it type == TextRun

                var chunkIndex: number = Utils.normedBinaryIndexOf(chunks, (c: Chunk) => c.startLogPosition.value - prevAbsolutePosition),
                    chunk: Chunk = chunks[chunkIndex];

                var runIndex: number = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => chunk.startLogPosition.value + r.startOffset - prevAbsolutePosition),
                    firstRunInInterval: TextRun = chunk.textRuns[runIndex];

                if (firstRunInInterval.type == TextRunType.TextRun) {
                    this.sourceRun = firstRunInInterval.clone();
                    return;
                }

                // take properties next run, not matter what type he have
                var indexNextRun: number = runIndex + 1;
                if (indexNextRun < chunk.textRuns.length) {
                    this.sourceRun = chunk.textRuns[indexNextRun].clone();
                    return;
                }
                if (chunkIndex + 1 < chunks.length) {
                    this.sourceRun = chunks[chunkIndex + 1].textRuns[0].clone();
                    return;
                }
                this.sourceRun = chunk.textRuns[chunk.textRuns.length - 1].clone();
                return;
            }
            else {
                // if 1st run - textRun, then get it
                // else if last run - textRun - get it (only exclude last paragraph in document)
                // else get 1st TextRun
                var firstRunInInterval: TextRun = this.subDocument.getRunByPosition(interval.start);
                if (firstRunInInterval.type == TextRunType.TextRun) {
                    this.sourceRun = firstRunInInterval.clone();
                    return;
                }
                var intervalEnd: number = interval.end();
                var lastRunInInterval: TextRun = this.subDocument.getRunByPosition(intervalEnd - 1);

                var lastSection: Section = this.model.sections[this.model.sections.length - 1];
                if (intervalEnd == lastSection.startLogPosition.value + lastSection.getLength() && intervalEnd > 2) // ignore last paragraphRun
                    lastRunInInterval = this.subDocument.getRunByPosition(intervalEnd - 2);

                if (lastRunInInterval.type == TextRunType.TextRun) {
                    this.sourceRun = lastRunInInterval.clone();
                    return;
                }
                this.sourceRun = firstRunInInterval.clone();
                return;
            }
        }

        private getLastInterval(): FixedInterval {
            return this.intervals[this.intervals.length - 1];
        }

        private resetReturnValues() {
            this.sourceRun = null;

            this.characterStyle = null;
            this.maskedCharacterProperties = null;

            this.mergedCharacterPropertiesRaw = null;
            this.mergedCharacterPropertiesFull = null;

            this.mergedParagraphPropertiesRaw = null;
            this.mergedParagraphPropertiesFull = null;

            this.mergedSectionPropertiesRaw = null;
            this.mergedSectionPropertiesFull = null;
        }
    }
}