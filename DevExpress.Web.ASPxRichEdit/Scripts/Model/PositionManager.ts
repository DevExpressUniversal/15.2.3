module __aspxRichEdit {
    export class Position {
        public value: number;
        private refCount: number;

        constructor(value: number) {
            this.value = value;
            this.refCount = 0;
        }

        incRefCount() {
            ++this.refCount;
        }

        decRefCount() {
            --this.refCount;
        }

        hasReference(): boolean {
            return this.refCount > 0;
        }
    }

    export class PositionManager implements IPositionManager {
        positions: Position[] = [];

        registerPosition(position: number): Position {
            var index: number = Utils.binaryIndexOf(this.positions, (p: Position) => p.value - position);
            if (index >= 0) {
                var findedPosition: Position = this.positions[index];
                findedPosition.incRefCount();
                return findedPosition; // give any of the appropriate (it can be more then one Position with Position.value==position)
            }

            var indexWhereInsert: number = ~index; // = -index - 1
            var newPosition: Position = new Position(position);
            newPosition.incRefCount();
            this.positions.splice(indexWhereInsert, 0, newPosition);
            return newPosition;
        }

        unregisterPosition(position: Position) {
            var exactIndex: number = this.findPosition(position);
            if (exactIndex != null) {
                var findedPosition: Position = this.positions[exactIndex];
                findedPosition.decRefCount();
                if (!findedPosition.hasReference())
                    this.positions.splice(exactIndex, 1);
            }
            else
                throw new Error("PositionManager unregisterPosition: unregister nonexisted Position");
        }

        //find Position, where findPosition == position, not findExactPosition.value === Position.value
        private findPosition(position: Position): number {
            var index: number = Utils.binaryIndexOf(this.positions, (p: Position) => p.value - position.value);
            if (index >= 0) {
                var exactIndex: number;
                // looking up 
                for (exactIndex = index; exactIndex >= 0 && this.positions[exactIndex].value == position.value; exactIndex--) {
                    if (this.positions[exactIndex] == position)
                        return exactIndex;
                }
                // looking after
                var positionsLength: number = this.positions.length;
                for (exactIndex = index + 1; exactIndex < positionsLength && this.positions[exactIndex].value == position.value; exactIndex++) {
                    if (this.positions[exactIndex] == position)
                        return exactIndex;
                }
            }
            return null;
        }

        reset() {
            this.positions = [];
        }

        advance(position: number, delta: number) {
            var index: number = Utils.binaryIndexOf(this.positions, (p: Position) => p.value - position);
            var advanceIndex: number = index >= 0 ? index : ~index;
            var positionsLength: number = this.positions.length;
            // here advanceIndex >= 0
            if (advanceIndex >= positionsLength)
                return;

            if (index >= 0)
                advanceIndex = this.correctPositionIndex(position, delta, advanceIndex);

            var i: number;
            for (i = advanceIndex; i < positionsLength; i++)
                this.positions[i].value += delta;

            // correcting situation (2 4 7 7 7 7 9 13 16) with call  "advance(4, -5)" => ( 2 4 4 4 4 4 4 8 11). Not (2 4 2 2 2 2 4 8 11)
            if (delta < 0 && advanceIndex < positionsLength && advanceIndex >= 0) {
                for (i = advanceIndex; i < positionsLength && this.positions[i].value < position; i++)
                    this.positions[i].value = position;
            }
        }

        private correctPositionIndex(position: number, delta:number, corrIndex: number): number {
            if (delta < 0) {
                // correcting situation ( 2 4 7 7 >7< 7 9 13) => (2 4 7 7 7 7 >9< 13) advance(7, -n)
                while (corrIndex < this.positions.length && this.positions[corrIndex].value == position)
                    corrIndex++;
            }
            else {
                // correcting situation (2 4 7 7 >7< 7 9) => ( 2 4 >7< 7 7 7 9) advance(7, +n)
                while (corrIndex > 0 && this.positions[corrIndex - 1].value == this.positions[corrIndex].value)
                    corrIndex--;
            }
            return corrIndex;
        }

    }
}