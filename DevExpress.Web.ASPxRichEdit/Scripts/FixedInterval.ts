module __aspxRichEdit {
    export class FixedInterval implements IEquatable<FixedInterval> {
        start: number;
        length: number;
        constructor(start: number, length: number) {
            this.start = start;
            this.length = length;
        }
        public equals(obj: FixedInterval): boolean {
            return obj && this.start === obj.start && this.length === obj.length;
        }
        public clone(): FixedInterval {
            return new FixedInterval(this.start, this.length);
        }
        public end(): number {
            return this.start + this.length;
        }
        public contains(val: number): boolean {
            return this.start <= val && this.end() >= val;
        }

        public containsPositionWithoutIntervalEnd(pos: number) {
            return this.start <= pos && pos < this.end();
        }

        public containsPositionWithoutIntervalEndAndStart(pos: number) {
            return this.start < pos && pos < this.end();
        }

        public static getIntersection(intervalA: FixedInterval, intervalB: FixedInterval): FixedInterval {
            const start: number = Math.max(intervalA.start, intervalB.start);
            const end: number = Math.min(intervalA.end(), intervalB.end());
            if (start > end)
                return null;
            return FixedInterval.fromPositions(start, end);
        }

        public expandInterval(interval: FixedInterval): FixedInterval {
            const end: number = Math.max(interval.end(), this.end());
            this.start = Math.min(interval.start, this.start);
            this.length = end - this.start;
            return this;
        }

        public containsInterval(startPosition: number, endPosition: number): boolean {
            return this.start <= startPosition && this.end() >= endPosition;
        }
        public static fromPositions(start: number, end: number) {
            return new FixedInterval(start, end - start);
        }
        public getLinkedInterval(positionManager: PositionManager): LinkedInterval {
            return new LinkedInterval(positionManager, this.start, this.end());
        }
    }
}