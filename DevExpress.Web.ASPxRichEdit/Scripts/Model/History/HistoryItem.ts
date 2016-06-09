module __aspxRichEdit {
    export class HistoryItem {
        modelManipulator: ModelManipulator;
        boundSubDocument: SubDocument;
        uniqueId: number = -1;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument) {
            this.modelManipulator = modelManipulator;
            this.boundSubDocument = boundSubDocument;
        }

        public changeModified(): boolean {
            return true;
        }

        public redo() { 
            throw new Error(Errors.NotImplemented);
        }

        public undo() {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class CompositionHistoryItem extends HistoryItem {
        historyItems: HistoryItem[] = [];

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument) {
            super(modelManipulator, boundSubDocument);
        }

        public changeModified(): boolean {
            var item: HistoryItem;
            for (var i = 0; item = this.historyItems[i]; i++) {
                if (item.changeModified())
                    return true;
            }
            return false;
        }

        public redo() {
            var item: HistoryItem;
            for (var i = 0; item = this.historyItems[i]; i++)
                item.redo();
        }

        public undo() {
            var item: HistoryItem;
            for (var i = this.historyItems.length - 1; item = this.historyItems[i]; i--)
                item.undo();
        }

        public add(historyItem: HistoryItem) {
            if (historyItem != null)
                this.historyItems.push(historyItem);
            else
                throw new Error(Errors.ValueCannotBeNull);
        }
    }
}