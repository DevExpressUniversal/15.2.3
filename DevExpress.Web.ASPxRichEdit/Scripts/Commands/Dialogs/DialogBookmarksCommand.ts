module __aspxRichEdit {
    export class DialogBookmarksCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.bookmarks) && this.control.selection.intervals.length === 1;
        }

        createParameters(): BookmarksDialogParameters {
            var parameters = new BookmarksDialogParameters();
            parameters.bookmarks = []
            for (let sd of this.control.model.getSubDocumentsList()) {
                for (let b of sd.bookmarks)
                    if(!b.isHidden())
                        parameters.bookmarks.push(new BookmarkDioalogInfo(b.name, b.start.value));
            }
            return parameters;
        }

        applyParameters(state: SimpleCommandState, newParams: BookmarksDialogParameters) {
            if (newParams.newBookmarkName != "") {
                var start = this.control.selection.intervals[0].start;
                var end = this.control.selection.intervals[0].start + this.control.selection.intervals[0].length;
                this.control.commandManager.getCommand(RichEditClientCommand.CreateBookmark).execute({
                    name: newParams.newBookmarkName,
                    start: start, end: end
                });
            }
            else if (newParams.deletedBookmarkNames.length > 0)
                this.control.commandManager.getCommand(RichEditClientCommand.DeleteBookmarks).execute(newParams.deletedBookmarkNames);
        }

        getDialogName() {
            return "Bookmarks";
        }

        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }

    export class BookmarkDioalogInfo {
        name: string;
        start: number;

        constructor(name: string, start: number) {
            this.name = name;
            this.start = start;
        }
    }

    export class BookmarksDialogParameters extends DialogParametersBase {
        bookmarks: BookmarkDioalogInfo[];
        deletedBookmarkNames: string[];
        newBookmarkName: string;

        copyFrom(obj: BookmarksDialogParameters) {
            this.bookmarks = obj.bookmarks;
            this.deletedBookmarkNames = obj.deletedBookmarkNames;
            this.newBookmarkName = obj.newBookmarkName;
        }

        getNewInstance(): DialogParametersBase {
            return new BookmarksDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}