module __aspxRichEdit {
    export class BookmarkHistoryItemBase extends HistoryItem {
        state: HistoryItemState<HistoryItemBookmarkStateObject>;
        name: string;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, name: string) {
            super(modelManipulator, boundSubDocument);
            this.name = name;
        }
    }

    export class CreateBookmarkHistoryItem extends BookmarkHistoryItemBase {
        start: number;
        end: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, start: number, end: number, name: string) {
            super(modelManipulator, boundSubDocument, name);
            this.start = start;
            this.end = end;
        }

        public redo(): void {
            this.state = this.modelManipulator.bookmarksManipulator.createBookmark(this.boundSubDocument, this.start, this.end, this.name);
        }
        public undo(): void {
            this.modelManipulator.bookmarksManipulator.restoreCreateBookmark(this.boundSubDocument, this.state);
        }
    }
    export class DeleteBookmarkHistoryItem extends BookmarkHistoryItemBase {
        public redo(): void {
            this.state = this.modelManipulator.bookmarksManipulator.deleteBookmark(this.boundSubDocument, this.name);
        }
        public undo(): void {
            this.modelManipulator.bookmarksManipulator.restoreDeleteBookmark(this.boundSubDocument, this.state);
        }
    }
} 