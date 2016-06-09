module __aspxRichEdit {
    // If you are not sure that you need it this class. Don't use it. Better use FixedInterval. To be careful with destructor. Call it manually
    export class LinkedInterval {
        start: Position;
        end: Position;

        constructor(manager: PositionManager, intervalStart: number, intervalEnd: number) {
            this.start = manager.registerPosition(intervalStart);
            this.end = manager.registerPosition(intervalEnd);
        }

        destructor(manager: PositionManager) {
            manager.unregisterPosition(this.start);
            manager.unregisterPosition(this.end);
        }

        getLength(): number {
            return this.end.value - this.start.value;
        }

        getFixedInterval(): FixedInterval {
            return FixedInterval.fromPositions(this.start.value, this.end.value);
        }
    }
}