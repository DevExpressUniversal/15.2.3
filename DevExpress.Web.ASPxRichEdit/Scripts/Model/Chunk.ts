module __aspxRichEdit {
    export class Chunk {
        public startLogPosition: Position;
        public textBuffer: string = "";
        public textRuns: TextRun[] = [];
        public isLast: boolean;

        constructor(startLogPosition: Position, textBuffer: string, isLast: boolean) {
            this.startLogPosition = startLogPosition;
            this.textBuffer = textBuffer;
            this.isLast = isLast;
        }

        public getEndPosition(): number {
            return this.startLogPosition.value + this.textBuffer.length;
        }

        public getRunText(run: TextRun) : string {
            return this.textBuffer.substr(run.startOffset, run.length);
        }
        public getTextInChunk(offsetAtStartChunk: number, length: number): string {
            return this.textBuffer.substr(offsetAtStartChunk, length);
        }
        public splitRun(runIndex: number, offset: number) {
            var run = this.textRuns[runIndex];
            if (!run)
                throw new Error("Undefined run");
            if (offset >= run.length)
                throw new Error("Offset >= run.length");
            var newRun = TextRun.create(run.startOffset + offset, run.length - offset, run.type, run.paragraph, run.characterStyle, run.maskedCharacterProperties, run);
            run.length = offset;
            this.textRuns.splice(runIndex + 1, 0, newRun);
        }
    }
}