module __aspxRichEdit {
    export class History implements IHistory {
        modelManipulator: ModelManipulator;
        historyItems: HistoryItem[] = [];
        currentIndex: number = -1;
        transaction: CompositionHistoryItem = null;

        private incrementalId: number = -1;
        private transactionLevel: number = -1;
        private unmodifiedIndex: number = -1;
        private options: ControlOptions

        constructor(modelManipulator: ModelManipulator, options: ControlOptions) {
            this.modelManipulator = modelManipulator;
            this.options = options;
        }

        public isModified(): boolean { 
            if (this.unmodifiedIndex == this.currentIndex)
                return false;
            var startIndex = Math.min(this.unmodifiedIndex, this.currentIndex);
            var endIndex = Math.max(this.unmodifiedIndex, this.currentIndex);
            for (var i = startIndex + 1; i <= endIndex; i++) {
                if (this.historyItems[i].changeModified())
                    return true;
            }
            return false;
        }
        public undo() {
            if (!this.canUndo())
                return;

            this.historyItems[this.currentIndex].undo();
            this.currentIndex--;
        }
        public redo() {
            if (!this.canRedo())
                return;

            this.currentIndex++;
            this.historyItems[this.currentIndex].redo();
        }
        public canUndo(): boolean {
            return this.currentIndex >= 0;
        }
        public canRedo(): boolean {
            return this.currentIndex < this.historyItems.length - 1;
        }
        public beginTransaction() {
            this.transactionLevel++;
            if (this.transactionLevel == 0)
                this.transaction = new CompositionHistoryItem(this.modelManipulator, this.modelManipulator.model.activeSubDocument);
        }
        public endTransaction() {
            if (this.transactionLevel < 0)
                return;

            this.transactionLevel--;
            if (this.transactionLevel < 0) {
                var transactionLength = this.transaction.historyItems.length;
                if (transactionLength > 1)
                    this.addInternal(this.transaction);
                else
                    if (transactionLength == 1)
                        this.addInternal(this.transaction.historyItems.pop());
                this.transaction = null;
            }
        }
        public addAndRedo(historyItem: HistoryItem) {
            this.add(historyItem);
            historyItem.redo();
        }
        public add(historyItem: HistoryItem) {
            if (this.transactionLevel >= 0)
                this.transaction.add(historyItem);
            else
                this.addInternal(historyItem);
        }
        private addInternal(historyItem: HistoryItem) {
            if(!ControlOptions.isEnabled(this.options.undo)) return;
            if (this.currentIndex < this.historyItems.length - 1) {
                this.historyItems.splice(this.currentIndex + 1);
                this.unmodifiedIndex = Math.min(this.unmodifiedIndex, this.currentIndex);
            }
            this.historyItems.push(historyItem);
            this.currentIndex++;
        }
        public getNextId() {
            this.incrementalId++;
            return this.incrementalId;
        }
        public clear() {
            this.currentIndex = -1;
            this.unmodifiedIndex = -1;
            this.incrementalId = -1;
            this.historyItems = [];
        }
        public resetModified() {
            this.unmodifiedIndex = this.currentIndex;
        }
        public getCurrentItemId(): number {
            if (this.currentIndex == -1)
                return -1;
            var currentItem = this.historyItems[this.currentIndex];
            if (currentItem.uniqueId == -1)
                currentItem.uniqueId = this.getNextId();
            return currentItem.uniqueId;
        }
    }
} 