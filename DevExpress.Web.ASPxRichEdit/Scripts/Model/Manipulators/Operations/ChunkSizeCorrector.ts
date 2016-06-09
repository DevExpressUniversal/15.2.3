module __aspxRichEdit {
    export class ChunkSizeCorrector {
        maxChunkSize: number = 4096;
        maxRunSizeCoeff: number = 0.25;
        maxRunSize: number = Math.floor(this.maxRunSizeCoeff * this.maxChunkSize);

        subDocument: SubDocument;
        chunks: Chunk[];

        originChunk: Chunk;
        originChunkIndex: number;
        originChunkRuns: TextRun[];

        needMoveLength: number;

        correctChunkSizeAtChunkIndex(subDocument: SubDocument, chunkIndex: number) {
            this.subDocument = subDocument;
            this.chunks = subDocument.chunks;
            this.originChunk = this.chunks[chunkIndex];
            this.originChunkIndex = chunkIndex;
            this.originChunkRuns = this.originChunk.textRuns;
            this.startCorrect();
        }

        correctChunkSizeAtInsertPosition(subDocument: SubDocument, insertPosition: number) {
            this.subDocument = subDocument;
            this.chunks = subDocument.chunks;
            const originRunInfo: { chunkIndex: number; runIndex: number; chunk: Chunk; run: TextRun } = this.subDocument.getRunAndIndexesByPosition(insertPosition);
            this.originChunk = originRunInfo.chunk;
            this.originChunkRuns = originRunInfo.chunk.textRuns;
            this.originChunkIndex = originRunInfo.chunkIndex;
            this.startCorrect();
        }

        startCorrect() {
            if (this.originChunk.textBuffer.length <= this.maxChunkSize)
                return;
            this.needMoveLength = this.originChunk.textBuffer.length - this.maxChunkSize;
            if (this.needMoveLength < this.maxChunkSize) {
                if (this.isMoveToPrevChunk())
                    return;
                if (this.isMoveToNextChunk())
                    return;
            }
            this.moveToNewNextChunks();
        }

        isMoveToPrevChunk(): boolean {
            const prevChunk: Chunk = this.chunks[this.originChunkIndex - 1];
            if (!prevChunk)
                return false;

            let runIndexFrom: number = 0;
            let run: TextRun;
            let totallyMoveLength: number = 0;
            for (; run = this.originChunkRuns[runIndexFrom]; runIndexFrom++) {
                if (run.length > this.maxRunSize) {
                    this.originChunk.splitRun(runIndexFrom, this.maxRunSize);
                }
                totallyMoveLength += run.length;
                if (totallyMoveLength >= this.needMoveLength)
                    break;
            }

            if (totallyMoveLength + prevChunk.textBuffer.length > this.maxChunkSize)
                return false;

            // move [0, runIndexFrom]
            let prevChunkOffset: number = prevChunk.textBuffer.length;
            for (; runIndexFrom >= 0; runIndexFrom--) {
                run = this.originChunkRuns.shift();
                prevChunk.textRuns.push(run);
                run.startOffset = prevChunkOffset;
                prevChunkOffset += run.length;
            }
            prevChunk.textBuffer += this.originChunk.textBuffer.substr(0, totallyMoveLength);
            this.originChunk.textBuffer = this.originChunk.textBuffer.substr(totallyMoveLength);

            this.subDocument.positionManager.unregisterPosition(this.originChunk.startLogPosition);
            this.originChunk.startLogPosition = this.subDocument.positionManager.registerPosition(this.originChunk.startLogPosition.value + totallyMoveLength);

            TextManipulator.moveRunsInChunk(this.originChunk, 0, -totallyMoveLength);

            return true;
        }

        isMoveToNextChunk(): boolean {
            const nextChunk: Chunk = this.chunks[this.originChunkIndex + 1];
            if (!nextChunk)
                return false;

            let runIndexFrom: number = this.originChunkRuns.length - 1;
            let run: TextRun;
            let totallyMoveLength: number = 0;
            for (; run = this.originChunkRuns[runIndexFrom]; runIndexFrom--) {
                while (run.length > this.maxRunSize) {
                    this.originChunk.splitRun(runIndexFrom, this.maxRunSize);
                    run = this.originChunkRuns[++runIndexFrom];
                }
                totallyMoveLength += run.length;
                if (totallyMoveLength >= this.needMoveLength)
                    break;
            }

            if (totallyMoveLength + nextChunk.textBuffer.length > this.maxChunkSize)
                return false;

            // move [runIndexFrom, .. end]
            TextManipulator.moveRunsInChunk(nextChunk, 0, totallyMoveLength);
            let offsetFirstRun: number = totallyMoveLength;
            for (runIndexFrom = this.originChunkRuns.length - runIndexFrom; runIndexFrom > 0; runIndexFrom--) {
                run = this.originChunkRuns.pop();
                nextChunk.textRuns.unshift(run);
                offsetFirstRun -= run.length;
                run.startOffset = offsetFirstRun;
            }
            const startMovedPosition: number = this.originChunk.textBuffer.length - totallyMoveLength;
            nextChunk.textBuffer = this.originChunk.textBuffer.substring(startMovedPosition) + nextChunk.textBuffer;
            this.originChunk.textBuffer = this.originChunk.textBuffer.substring(0, startMovedPosition);

            this.subDocument.positionManager.unregisterPosition(nextChunk.startLogPosition);
            nextChunk.startLogPosition = this.subDocument.positionManager.registerPosition(nextChunk.startLogPosition.value - totallyMoveLength);

            return true;
        }

        moveToNewNextChunks() {
            let runIndexFrom: number = this.originChunkRuns.length - 1;
            let run: TextRun;
            let totallyMoveLength: number = 0;
            for (; run = this.originChunkRuns[runIndexFrom]; runIndexFrom--) {
                while (run.length > this.maxRunSize) {
                    this.originChunk.splitRun(runIndexFrom, this.maxRunSize);
                    run = this.originChunkRuns[++runIndexFrom];
                }
                totallyMoveLength += run.length;
                if (totallyMoveLength >= this.needMoveLength)
                    break;
            }

            let newChunk: Chunk = undefined;
            let runOffset: number = 0;
            let indexInsertNewChunk: number = this.originChunkIndex + 1;
            let chunkStartPosition: number = this.originChunk.startLogPosition.value + this.originChunkRuns[runIndexFrom].startOffset;
            for (let currRunIndex: number = runIndexFrom; run = this.originChunkRuns[currRunIndex]; currRunIndex++) {
                if (newChunk == undefined || runOffset + run.length > this.maxChunkSize) {
                    // (1.0) line below have better perfomanse then newChunk.textBuffer += this.originChunk.getRunText(run);
                    if (newChunk)
                        newChunk.textBuffer = this.originChunk.textBuffer.substr(newChunk.startLogPosition.value - this.originChunk.startLogPosition.value, runOffset);
                    newChunk = new Chunk(this.subDocument.positionManager.registerPosition(chunkStartPosition), "", false);
                    this.chunks.splice(indexInsertNewChunk, 0, newChunk);
                    indexInsertNewChunk++;
                    runOffset = 0;
                }
                newChunk.textRuns.push(run);
                run.startOffset = runOffset;

                chunkStartPosition += run.length;
                runOffset += run.length;
            }
            // (1.1)for one last chunk
            newChunk.textBuffer = this.originChunk.textBuffer.substr(newChunk.startLogPosition.value - this.originChunk.startLogPosition.value, runOffset);

            const originPrevRun: TextRun = this.originChunkRuns[runIndexFrom - 1]; // prev because  runIndexFrom have incorrect startOffset
            this.originChunk.textBuffer = this.originChunk.textBuffer.substring(0, originPrevRun.startOffset + originPrevRun.length);
            this.originChunkRuns.splice(runIndexFrom);

            this.originChunk.isLast = false;
            this.chunks[this.chunks.length - 1].isLast = true;
        }
    }
}