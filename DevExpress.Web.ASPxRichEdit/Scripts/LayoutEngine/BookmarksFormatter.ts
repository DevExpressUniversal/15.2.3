module __aspxRichEdit {
    export class BookmarksFormatter implements ILayoutChangesListener {
        private layout: DocumentLayout;
        private measurer: IBoxMeasurer;
        private layoutBoxes: LayoutBox[] = [];
        private existingPositions: any = { };
        private settings: any;
        private index: number;

        constructor(layout: DocumentLayout, measurer: IBoxMeasurer, settings: any) {
            this.layout = layout;
            this.measurer = measurer;
            this.settings = settings;
        }
        public NotifyPagesInvalidated(firstInvalidPageIndex: number): void {
            this.existingPositions = { };
        }
        public NotifyPageReady(pageChanges: PageChange) {
            if (pageChanges.changeType == LayoutChangeType.Deleted || this.settings.visibility != BookmarksVisibility.Visible)
                return;

            const layoutPage: LayoutPage = pageChanges.layoutElement;
            var bookmarks: Bookmark[] = [];
            this.index = pageChanges.index;

            for (var i: number = 0, pageArea: LayoutPageArea; pageArea = layoutPage.mainSubDocumentPageAreas[i]; i++)
                this.foreachBookmarks(pageArea.subDocument.bookmarks, pageArea, layoutPage.contentIntervals);
            for (var pageAreaIndex in layoutPage.otherPageAreas)
                this.foreachBookmarks(layoutPage.otherPageAreas[pageAreaIndex].subDocument.bookmarks, layoutPage.otherPageAreas[pageAreaIndex], layoutPage.contentIntervals);
        }

        private foreachBookmarks(bookmarks: Bookmark[], pageArea: LayoutPageArea, intervals: FixedInterval[]) {
            for (var i: number = 0, bm: Bookmark; bm = bookmarks[i]; i++) {
                if (bm.isHidden())
                    continue;

                for (var j: number = 0, interval: FixedInterval; interval = intervals[j]; j++) {
                    if (interval.contains(bm.start.value))
                        this.createLayoutBox(pageArea, bm.start, LayoutBoxType.BookmarkStartBox);
                    if (interval.contains(bm.end.value))
                        this.createLayoutBox(pageArea, bm.end, LayoutBoxType.BookmarkEndBox);
                }
            }
        }
        private getLayoutPosition(subDocument: SubDocument, position: number): LayoutPosition {
            var endRowConflictTags = new LayoutPositionCreatorConflictFlags().setDefault(false);
            var middleRowConflictTags = new LayoutPositionCreatorConflictFlags().setDefault(true);

            return subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(this.layout, subDocument, position, DocumentLayoutDetailsLevel.Max)
                .create(endRowConflictTags, middleRowConflictTags) :
                new LayoutPositionOtherSubDocumentCreator(this.layout, subDocument, position, this.index,
                    DocumentLayoutDetailsLevel.Max).create(endRowConflictTags, middleRowConflictTags);
        }

        private createLayoutBox(pageArea: LayoutPageArea, position: Position, boxType: LayoutBoxType) {
            var key = `${position.value}_${pageArea.subDocument.id}_${boxType}`;
            if (this.existingPositions[key])
                return;
            this.existingPositions[key] = true;
            var layoutPosition: LayoutPosition = this.getLayoutPosition(pageArea.subDocument, position.value);

            var box: BookmarkBox = new BookmarkBox(boxType);
            box.x = layoutPosition.box.x;
            if (boxType == LayoutBoxType.BookmarkEndBox)
                box.x = box.x - BookmarkBox.DEFAULT_BORDER_WIDTH;

            box.y = -layoutPosition.row.getSpacingBefore();
            box.width = BookmarkBox.DEFAULT_WIDTH;
            box.height = layoutPosition.row.height;
            box.color = this.settings.color;

            if (layoutPosition.charOffset > 0)
                box.x += layoutPosition.box.getCharOffsetXInPixels(this.measurer, layoutPosition.charOffset);

            var boxes = pageArea.columns[layoutPosition.columnIndex].rows[layoutPosition.rowIndex].bookmarkBoxes;

            var index = Utils.binaryIndexOf(boxes, (b: BookmarkBox) => b.x - box.x);
            if (index < 0) {
                index = ~index;
                boxes.splice(index, 0, box);
            }
        }
    }
}