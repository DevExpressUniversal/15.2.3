module __aspxRichEdit {
    export class ModelIterator implements ICloneable<ModelIterator> {
        // const
        public subDocument: SubDocument;
        public chunks: Chunk[];
        
        // you can reset it directly anytime
        public ignoreHiddenRuns: boolean;

        // next fields only for readonly mode. Don't reset it
        public chunk: Chunk;
        public chunkIndex: number;
        
        public runs: TextRun[];
        public run: TextRun;
        public runIndex: number;
        
        public charOffset: number;

        constructor(subDocument: SubDocument, ignoreHiddenRuns: boolean) {
            this.subDocument = subDocument;
            this.chunks = subDocument.chunks;
            this.ignoreHiddenRuns = ignoreHiddenRuns;
        }

        public setPosition(pos: number) {
            if (this.run && this.getCurrectPosition() == pos)
                return;

            this.chunkIndex = Utils.normedBinaryIndexOf(this.chunks, (c: Chunk) => c.startLogPosition.value - pos);
            this.chunk = this.chunks[this.chunkIndex];

            var runOffset: number = pos - this.chunk.startLogPosition.value;
            this.runs = this.chunk.textRuns;
            this.runIndex = Utils.normedBinaryIndexOf(this.runs, (r: TextRun) => r.startOffset - runOffset);
            this.run = this.runs[this.runIndex];

            this.charOffset = runOffset - this.run.startOffset;

            if (this.ignoreHiddenRuns && this.run.getCharacterMergedProperies().hidden)
                this.moveToNextRun();
        }

        getCurrectPosition(): number {
            return this.chunk.startLogPosition.value + this.run.startOffset + this.charOffset;
        }

        getCurrentChar(): string {
            return this.chunk.textBuffer[this.run.startOffset + this.charOffset];
        }

        moveToNextChar(): boolean {
            if (this.charOffset + 1 < this.run.length) {
                this.charOffset++;
                return true;
            }

            return this.moveToNextRun();
        }

        moveToPrevChar(): boolean {
            if (this.charOffset > 0) {
                this.charOffset--;
                return true;
            }

            return this.moveToPrevRun();
        }

        public moveToNextRun(): boolean {
            if (this.runIndex + 1 < this.runs.length) {
                this.charOffset = 0;
                this.runIndex++;
                this.run = this.runs[this.runIndex];
                if (this.ignoreHiddenRuns && this.run.getCharacterMergedProperies().hidden)
                    return this.moveToNextRun();
                return true;
            }

            if (this.chunkIndex + 1 < this.chunks.length) {
                this.charOffset = 0;
                this.runIndex = 0;
                this.chunkIndex++;
                this.chunk = this.chunks[this.chunkIndex];
                this.runs = this.chunk.textRuns;
                this.run = this.runs[this.runIndex];
                if (this.ignoreHiddenRuns && this.run.getCharacterMergedProperies().hidden)
                    return this.moveToNextRun();
                return true;
            }

            return false;
        }

        public moveToPrevRun(): boolean {
            if (this.runIndex > 0) {
                this.runIndex--;
                this.run = this.runs[this.runIndex];
                this.charOffset = this.run.length - 1;
                if (this.ignoreHiddenRuns && this.run.getCharacterMergedProperies().hidden)
                    return this.moveToPrevRun();
                return true;
            }

            if (this.chunkIndex > 0) {
                this.chunkIndex--;
                this.chunk = this.chunks[this.chunkIndex];
                this.runs = this.chunk.textRuns;
                this.runIndex = this.runs.length - 1;
                this.run = this.runs[this.runIndex];
                this.charOffset = this.run.length - 1;
                if (this.ignoreHiddenRuns && this.run.getCharacterMergedProperies().hidden)
                    return this.moveToPrevRun();
                return true;
            }

            return false;
        }

        public clone(): ModelIterator {
            var newIterator: ModelIterator = new ModelIterator(this.subDocument, this.ignoreHiddenRuns);
            newIterator.chunks = this.chunks;
            newIterator.chunk = this.chunk;
            newIterator.chunkIndex = this.chunkIndex;
            newIterator.runs = this.runs;
            newIterator.run = this.run;
            newIterator.runIndex = this.runIndex;
            newIterator.charOffset = this.charOffset;
            return newIterator;
        }
    }
}