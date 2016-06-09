module __aspxRichEdit {
    export class LayoutBoxIteratorOtherSubDocument extends LayoutBoxIteratorBase {
        private pageIndex: number;
        constructor(subDocument: SubDocument, layout: DocumentLayout, intervalStart: number, intervalEnd: number, pageIndex: number) {
            super(subDocument, layout, intervalStart, intervalEnd);
            this.pageIndex = pageIndex;
        }
        public isInitialized(): boolean {
            const page: LayoutPage = this.layout.pages[this.pageIndex];
            if(!page || page.otherPageAreas[this.subDocument.id].getEndPosition() < this.intervalEnd)
                return false;
            return true;
        }
        protected getNewLayoutPosition(position: number, endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags): LayoutPosition {
            return new LayoutPositionOtherSubDocumentCreator(this.layout, this.subDocument, position, this.pageIndex, DocumentLayoutDetailsLevel.Character)
                .create(endRowConflictFlags, middleRowConflictFlags);
        }
    }
}