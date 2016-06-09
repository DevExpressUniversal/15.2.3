module __aspxRichEdit {
    export interface IBatchUpdatableObject {
        beginUpdate();
        endUpdate();
    }

    export class BatchUpdatableObject implements IBatchUpdatableObject {
        private suspendUpdateCount: number = 0;
        private occurredEvents: number = 0;

        beginUpdate() {
            //console.log("beginUpdate");
            //console.dir(this);
            if (this.suspendUpdateCount === 0)
                this.onUpdateLocked();
            if (this.suspendUpdateCount < 0)
                this.suspendUpdateCount--;
            else
                this.suspendUpdateCount++;
        }
        endUpdate() {
            //console.log("endUpdate");
            //console.dir(this);
            if (this.suspendUpdateCount < 0)
                this.suspendUpdateCount++;
            else if (this.suspendUpdateCount > 0)
                this.suspendUpdateCount--;
            if (!this.isUpdateLocked()) {
                this.onUpdateUnlocked(this.occurredEvents);
                this.occurredEvents = 0;
            }
        }
        suspendUpdate() {
            //console.log("suspendUpdate");
            //console.dir(this);
            if (this.suspendUpdateCount > 0) {
                this.suspendUpdateCount *= -1;
                this.onUpdateUnlocked(this.occurredEvents);
                this.occurredEvents = 0;
            }
        }
        continueUpdate() {
            //console.log("continueUpdate");
            //console.dir(this);
            if (this.suspendUpdateCount < 0)
                this.suspendUpdateCount *= -1;
        }
        isUpdateLocked(): boolean {
            return this.suspendUpdateCount > 0;
        }
        onUpdateUnlocked(occurredEvents: number) { }
        onUpdateLocked() { }
        registerOccurredEvent(eventMask: number) {
            this.occurredEvents |= eventMask;
        }
    }
} 