module __aspxRichEdit {
    export class LayoutBoxIteratorMainSubDocument extends LayoutBoxIteratorBase {
        constructor(subDocument: SubDocument, layout: DocumentLayout, intervalStart: number, intervalEnd: number) {
            super(subDocument, layout, intervalStart, intervalEnd);
        }
        public isInitialized(): boolean {
            if(!this.layout.isFullyFormatted) {
                const lastValidPage: LayoutPage = this.layout.getLastValidPage();
                if(!lastValidPage || this.intervalEnd > lastValidPage.contentIntervals[0].end())
                    return false;
            }
            return true;
        }
        protected getNewLayoutPosition(position: number, endRowConflictFlags: LayoutPositionCreatorConflictFlags, middleRowConflictFlags: LayoutPositionCreatorConflictFlags): LayoutPosition {
            return new LayoutPositionMainSubDocumentCreator(this.layout, this.subDocument, position, DocumentLayoutDetailsLevel.Character)
                .create(endRowConflictFlags, middleRowConflictFlags);
        }
    }
}