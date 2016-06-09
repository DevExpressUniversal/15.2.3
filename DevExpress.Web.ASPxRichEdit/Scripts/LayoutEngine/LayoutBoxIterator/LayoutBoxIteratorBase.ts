module __aspxRichEdit {
    // interval any. But interval should not belong for table.
    //abstract
    export class LayoutBoxIteratorBase {
        protected layout: DocumentLayout;
        protected subDocument: SubDocument;
        protected lastModelPosition: number;

        public position: LayoutPosition;
        public intervalStart: number;
        public intervalEnd: number;

        protected endRowConflictFlags: LayoutPositionCreatorConflictFlags;
        protected middleRowConflictFlags: LayoutPositionCreatorConflictFlags;

        constructor(subDocument: SubDocument, layout: DocumentLayout, intervalStart: number, intervalEnd: number) {
            this.layout = layout;
            this.subDocument = subDocument;
            this.intervalStart = intervalStart;
            this.intervalEnd = intervalEnd;
            this.lastModelPosition = -1;
        }
        public isInitialized(): boolean {
            throw new Error(Errors.NotImplemented);
        }
        public resetToInterval(intervalStart: number, intervalEnd: number): boolean {
            this.intervalStart = intervalStart;
            this.intervalEnd = intervalEnd;
            return this.isInitialized();
        }
        public moveNext(endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags): boolean {
            this.endRowConflictFlags = endRowConflictFlags.clone();
            this.middleRowConflictFlags = middleRowConflictFlags.clone();

            if (this.lastModelPosition < 0) {
                this.position = this.getNewLayoutPosition(this.intervalStart, this.endRowConflictFlags, this.middleRowConflictFlags);
                if (!this.position && this.endRowConflictFlags.atLeastOneIsFalse())
                    this.position = this.getNewLayoutPosition(this.intervalStart, this.endRowConflictFlags.setDefault(true), this.middleRowConflictFlags);
                this.lastModelPosition = this.position.getLogPosition();
                return true;
            }

            if (this.lastModelPosition > this.intervalEnd)
                return false;

            if (this.lastModelPosition == this.intervalEnd)
                return this.setBoundPosition(this.intervalEnd, this.position, (boundPos: number) => boundPos < this.lastModelPosition);

            const prevPosition: LayoutPosition = this.position.clone();
            if (!this.advancePosition())
                return false;

            const currModelPos: number = this.position.getLogPosition();
            if (currModelPos >= this.intervalEnd)
                return this.setBoundPosition(this.intervalEnd, prevPosition, (boundPos: number) => boundPos < this.lastModelPosition);

            this.lastModelPosition = currModelPos;
            return true;
        }

        public movePrev(endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags): boolean {
            this.endRowConflictFlags = endRowConflictFlags.clone();
            this.middleRowConflictFlags = middleRowConflictFlags.clone();

            if (this.lastModelPosition < 0) {
                this.position = this.getNewLayoutPosition(this.intervalEnd, this.endRowConflictFlags, this.middleRowConflictFlags);
                if (!this.position && this.endRowConflictFlags.atLeastOneIsFalse())
                    this.position = this.getNewLayoutPosition(this.intervalEnd, this.endRowConflictFlags.setDefault(true), this.middleRowConflictFlags);
                this.lastModelPosition = this.position.getLogPosition();
                return true;
            }

            if (this.lastModelPosition < this.intervalStart)
                return false;

            if (this.lastModelPosition == this.intervalStart)
                return this.setBoundPosition(this.intervalStart, this.position, (boundPos: number) => boundPos > this.lastModelPosition);

            const prevPosition: LayoutPosition = this.position.clone();
            if (!this.advancePositionBack())
                return false;

            const currModelPos: number = this.position.getLogPosition();
            if (currModelPos <= this.intervalStart)
                return this.setBoundPosition(this.intervalStart, prevPosition, (boundPos: number) => boundPos > this.lastModelPosition);

            this.lastModelPosition = currModelPos;
            return true;
        }

        protected getNewLayoutPosition(position: number, endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags): LayoutPosition {
            throw new Error(Errors.NotImplemented);
        }

        private setBoundPosition(logPosition: number, prevPosition: LayoutPosition, boundFunc: (boundPos: number) => boolean): boolean {
            let layoutPosition: LayoutPosition = this.getNewLayoutPosition(logPosition, this.endRowConflictFlags, this.middleRowConflictFlags);
            if (!layoutPosition)
                layoutPosition = this.getNewLayoutPosition(logPosition, this.endRowConflictFlags.setDefault(true), this.middleRowConflictFlags);
            const modelPos: number = layoutPosition.getLogPosition();

            if (layoutPosition.equals(prevPosition) || boundFunc(modelPos)) {
                this.position = prevPosition;
                return false;
            }

            this.position = layoutPosition;
            this.lastModelPosition = modelPos;
            return true;
        }

        private advancePosition(): boolean {
            if (this.position.boxIndex + 1 < this.position.row.boxes.length) {
                this.position.boxIndex++;
                this.position.box = this.position.row.boxes[this.position.boxIndex];
                this.position.charOffset = 0;
                return true;
            }
            
            if (this.position.advanceToNextRow(this.layout)) {
                this.position.boxIndex = 0;
                this.position.box = this.position.row.boxes[0];
                this.position.charOffset = 0;
                return true;
            }

            if (this.position.charOffset != this.position.box.getLength()) {
                this.position.charOffset = this.position.box.getLength()
                return true;
            }
            return false;
        }

        private advancePositionBack() {
            if (this.position.charOffset != 0) {
                this.position.charOffset = 0;
                return true;
            }

            if (this.position.boxIndex > 0) {
                this.position.boxIndex--;
                this.position.box = this.position.row.boxes[this.position.boxIndex];
                this.position.charOffset = 0;
                return true;
            }

            if (this.position.advanceToPrevRow(this.layout)) {
                this.position.boxIndex = this.position.row.boxes.length - 1;
                this.position.box = this.position.row.boxes[this.position.boxIndex];
                this.position.charOffset = 0;
                return true;
            }

            return false;
        }
    }
}