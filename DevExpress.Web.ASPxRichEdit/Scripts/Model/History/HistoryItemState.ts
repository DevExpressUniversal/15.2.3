module __aspxRichEdit {
    export class HistoryItemState<T extends IHistoryItemStateObject> {
        objects: T[] = [];
        lastObject: T;

        constructor(...values: T[]) {
            for(var value, i = 0; value = values[i]; i++)
                this.register(value);
        }
        register(object: T) {
            if(this.lastObject && this.lastObject.canMerge(object))
                this.lastObject.merge(object);
            else {
                this.objects.push(object);
                this.lastObject = object;
            }
        }
        toJSON(): any {
            var result = [];
            for(var object: T, i = 0; object = this.objects[i]; i++)
                result.push(object.toJSON());
            return result;
        }
        isEmpty(): boolean {
            return !this.lastObject;
        }
    }

    export class HistoryItemIntervalState<T extends HistoryItemIntervalStateObject> extends HistoryItemState<T> { // TODO: rename it to HistoryItemIntervalState
        interval(): FixedInterval {
            if(this.lastObject)
                return FixedInterval.fromPositions(this.objects[0].interval.start, this.lastObject.interval.end());
            return null;
        }
    }
}