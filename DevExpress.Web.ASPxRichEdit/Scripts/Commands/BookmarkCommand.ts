module __aspxRichEdit {
    export class BookmarkCommandBase extends CommandBase<SimpleCommandState> {
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.bookmarks);
        }

        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            if (state.enabled) {
                this.executeCoreIfEnabled(state, parameter);
                return true;
            }
            return false;;
        }

        executeCoreIfEnabled(state: SimpleCommandState, parameter: any) {
        }

        findBookmark(name: string): { bookmark: Bookmark, subDocument: SubDocument } {
            for (let sd of this.control.model.getSubDocumentsList()) {
                for (let bm of sd.bookmarks)
                    if (bm.name == name)
                        return { bookmark: bm, subDocument: sd };
            }
            return null;
        }

        getSubDocument(): SubDocument {
            return this.control.modelManipulator.model.activeSubDocument;
        }
    }

    export class CreateBookmarkCommand extends BookmarkCommandBase {
        executeCoreIfEnabled(state: SimpleCommandState, parameter: any) {
            var bookmarkInfo = <{ name: string; start: number, end: number }>parameter;
            var subDocument: SubDocument = this.getSubDocument();
            this.control.history.beginTransaction();
            if (this.findBookmark(bookmarkInfo.name))
                this.control.history.addAndRedo(new DeleteBookmarkHistoryItem(this.control.modelManipulator, subDocument, bookmarkInfo.name));
            this.control.history.addAndRedo(this.createHistoryItem(this.control.modelManipulator, subDocument, bookmarkInfo.start, bookmarkInfo.end, bookmarkInfo.name));
            this.control.history.endTransaction();
        }
        createHistoryItem(modelManipulator: ModelManipulator, subDocument: SubDocument, start: number, end: number, name: string): HistoryItem {
            return new CreateBookmarkHistoryItem(this.control.modelManipulator, subDocument, start, end, name);
        }
    }
    export class DeleteBookmarksCommand extends BookmarkCommandBase {
        executeCoreIfEnabled(state: SimpleCommandState, parameter: any){
            var bookmarkNames = <string[]>parameter;
            this.control.history.beginTransaction();
            for (var i = 0, name; name = bookmarkNames[i]; i++) {
                var bookmarkInfo = this.findBookmark(name);
                this.control.history.addAndRedo(new DeleteBookmarkHistoryItem(this.control.modelManipulator, bookmarkInfo.subDocument, bookmarkInfo.bookmark.name));
            }
            this.control.history.endTransaction();
        }
    }

    export class GoToBookmarkCommand extends BookmarkCommandBase {
        getSectionIndex(): number {
            let pageIndex = this.control.selection.pageIndex;
            let layoutPage = this.control.forceFormatPage(pageIndex);
            return HeaderFooterCommandBase.getSectionIndex(layoutPage, this.control.model);
        }

        executeCoreIfEnabled(state: SimpleCommandState, parameter: any) {
            var name = <string>parameter;
            var obj = this.findBookmark(name);
            if (obj) {
                var bookmark = obj.bookmark;
                var subDocument = obj.subDocument;

                this.changeSubDocument(subDocument);
                this.goTo(bookmark, subDocument, this.control.getViewElement());
            }
        }
        goTo(bookmark: Bookmark, subDocument: SubDocument, viewElement: HTMLDivElement): void {
            var startY = this.getY(this.getPosition(bookmark.start.value, subDocument));
            var endY = this.getY(this.getPosition(bookmark.end.value, subDocument));
            var scrollTop = startY;

            if (startY != endY)
                scrollTop = startY + (endY - startY);
            viewElement.scrollTop = scrollTop - viewElement.offsetHeight / 2;
            this.control.selection.setSelection(bookmark.start.value, bookmark.end.value, false, this.control.selection.keepX, UpdateInputPositionProperties.No, false);
        }
        changeSubDocument(subDocument: SubDocument): void {
            if (this.control.model.activeSubDocument == subDocument)
                return;
            if (subDocument.isMain())
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(this.control.model.mainSubDocument.info);
            else {
                let section = this.control.model.sections[this.getSectionIndex()];
                let isEvenPage = Utils.isEven(this.control.selection.pageIndex);
                let firstPageOfSection = this.control.layout.pages[this.control.selection.pageIndex].getStartOffsetContentOfMainSubDocument() === section.startLogPosition.value;
                let info = (subDocument.isHeader() ? section.headers : section.footers).getActualObject(firstPageOfSection, isEvenPage);
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(info);
            }
        }
        getY(pos: LayoutPosition): number {
            var y = this.control.getCanvasSizeInfo().getPageOffsetY(this.control.layout.pages[pos.pageIndex]);
            y += pos.pageArea.y;
            y += pos.column.y;
            y += pos.row.y;
            return y;
        }

        getPosition(pos: number, subDocument: SubDocument): LayoutPosition {
            var endRowConflictTags = new LayoutPositionCreatorConflictFlags().setDefault(false);
            var middleRowConflictTags = new LayoutPositionCreatorConflictFlags().setDefault(true);
            return subDocument.isMain()
                ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, pos,
                    DocumentLayoutDetailsLevel.Character, endRowConflictTags, middleRowConflictTags)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, pos, this.control.selection.pageIndex,
                    DocumentLayoutDetailsLevel.Character).create(endRowConflictTags, middleRowConflictTags);
        }
    }
}