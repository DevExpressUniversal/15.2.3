module __aspxRichEdit {
    export class BookmarksManipulator {
        manipulator: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        public createBookmark(subDocument: SubDocument, start: number, end: number, name: string): HistoryItemState<HistoryItemBookmarkStateObject> {
            var state: HistoryItemState<HistoryItemBookmarkStateObject> = new HistoryItemState<HistoryItemBookmarkStateObject>();
            var newBookmark: Bookmark = new Bookmark();
            newBookmark.name = name;
            newBookmark.start = subDocument.positionManager.registerPosition(start);
            newBookmark.end = subDocument.positionManager.registerPosition(end);
            subDocument.bookmarks.push(newBookmark);

            subDocument.bookmarks = this.getSortedBookmarks(subDocument.bookmarks); //TODO

            state.register(new HistoryItemBookmarkStateObject(name, start, end));
            //newState.register(new HistoryItemBookmarkStateObject(name, start, end)); //TODO

            this.manipulator.dispatcher.notifyBookmarkCreated(subDocument, state);
            return state;
        }
        public restoreCreateBookmark(subDocument: SubDocument, state: HistoryItemState<HistoryItemBookmarkStateObject>) {
            if (state.isEmpty())
                return;

            var name: string = state.objects[0].name;
            var start: number = state.objects[0].start;
            var end: number = state.objects[0].end;
            var bookmark = this.findBookmark(name, subDocument.bookmarks);

            this.deleteBookmarkInternal(bookmark, subDocument);
            this.manipulator.dispatcher.notifyBookmarkDeleted(subDocument, state);
        }
        public deleteBookmark(subDocument: SubDocument, name: string): HistoryItemState<HistoryItemBookmarkStateObject> {
            var bm = this.findBookmark(name, subDocument.bookmarks);
            var state: HistoryItemState<HistoryItemBookmarkStateObject> = new HistoryItemState<HistoryItemBookmarkStateObject>();

            state.register(new HistoryItemBookmarkStateObject(name, bm.start.value, bm.end.value, true));
            this.deleteBookmarkInternal(bm, subDocument);
            this.manipulator.dispatcher.notifyBookmarkDeleted(subDocument, state);
            return state;
        }
        public restoreDeleteBookmark(subDocument: SubDocument, state: HistoryItemState<HistoryItemBookmarkStateObject>) {
            if (state.isEmpty())
                return;
            var name: string = state.objects[0].name;
            var start: number = state.objects[0].start;
            var end: number = state.objects[0].end;
            this.createBookmarkInternal(name, start, end, subDocument);

            var newState: HistoryItemState<HistoryItemBookmarkStateObject> = new HistoryItemState<HistoryItemBookmarkStateObject>();
            newState.register(new HistoryItemBookmarkStateObject(name, start, end));

            this.manipulator.dispatcher.notifyBookmarkCreated(subDocument, newState);
        }

        private getSortedBookmarks(bookmarks: Bookmark[]): Bookmark[] {
            return bookmarks.sort(function (b1, b2) {
                if (b1.start.value > b2.start.value)
                    return 1;
                else if (b1.start.value < b2.start.value)
                    return -1;
                return 0;
            });
        }

        private findBookmark(name: string, bookmarks: Bookmark[]): Bookmark {
            for (let bm of bookmarks)
                if (bm.name == name)
                    return bm;
            return null;
        }
        private getIndexByName(name: string, bookmarks: Bookmark[]): number {
            for (var i = 0; i < bookmarks.length; i++)
                if (bookmarks[i].name == name)
                    return i
            return -1;
        }
        private deleteBookmarkInternal(bookmark: Bookmark, subDocument: SubDocument) {
            var bookmarks: Bookmark[] = subDocument.bookmarks;
            var index = this.getIndexByName(bookmark.name, bookmarks);
            bookmarks.splice(index, 1);
            subDocument.positionManager.unregisterPosition(bookmark.start);
            subDocument.positionManager.unregisterPosition(bookmark.end);
        }
        private createBookmarkInternal(name: string, start: number, end: number, subDocument: SubDocument) {
            var bookmark: Bookmark = new Bookmark();
            bookmark.name = name;
            bookmark.start = subDocument.positionManager.registerPosition(start);
            bookmark.end = subDocument.positionManager.registerPosition(end);
            subDocument.bookmarks.push(bookmark);
        }
    }
}