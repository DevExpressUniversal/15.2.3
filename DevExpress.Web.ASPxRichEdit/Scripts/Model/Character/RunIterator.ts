module __aspxRichEdit {
    export class RunIterator {
        public currentRun: TextRun = null;
        public currentChunk: Chunk = null;
        public currentSection: Section = null;

        private runs: TextRun[] = [];
        private chunks: Chunk[] = [];
        private sections: Section[] = [];

        private currentRunIndex: number = 0;
        private currentChunkIndex: number = 0;
        private currentSectionIndex: number = 0;

        private indexForChunks: number[] = [];
        private indexForSections: number[] = [];

        private interval: FixedInterval;

        constructor(interval: FixedInterval, runs: TextRun[], chunks: Chunk[], sections: Section[], indexForChunks: number[], indexForSections: number[]) {
            this.runs = runs;
            this.chunks = chunks;
            this.sections = sections;
            this.interval = interval;
            this.indexForChunks = indexForChunks;
            this.indexForSections = indexForSections;
        }

        public moveNext(): boolean {
            this.currentRun = this.runs[this.currentRunIndex];
            if (this.currentRun) {
                this.currentChunk = this.chunks[this.currentChunkIndex];
                this.currentSection = this.sections[this.currentSectionIndex];

                if (this.currentRunIndex == this.indexForChunks[this.currentChunkIndex])
                    this.currentChunkIndex++;
                if (this.currentRunIndex == this.indexForSections[this.currentSectionIndex])
                    this.currentSectionIndex++;
                this.currentRunIndex++;
                return true;
            }
            else {
                this.currentChunk = undefined;
                this.currentSection = undefined;
                return false;
            }
        }

        public currentInterval(): FixedInterval {
            if (this.currentRun)
                return new FixedInterval(this.currentChunk.startLogPosition.value + this.currentRun.startOffset, this.currentRun.length);
            else
                return new FixedInterval(this.chunks[0].startLogPosition.value + this.runs[0].startOffset, this.runs[0].length);
        }

        public getFirstRun(): TextRun {
            return this.runs[0];
        }

        public getLastRun(): TextRun {
            return this.runs[this.runs.length - 1];
        }

        public getRunsCount(): number {
            return this.runs.length;
        }

        public reset() {
            this.currentRunIndex = 0;
            this.currentChunkIndex = 0;
            this.currentSectionIndex = 0;
        }
    }
}  