module __aspxRichEdit {
    export class GoToParagraphStartCommandBase extends SelectionCommandBase {
        executeCore(state: ICommandState): boolean {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var position: number = this.getPositionStartParagraph();
            if (position < 0)
                return false;
            var selection: Selection = this.control.selection;
            if (this.extendSelection())
                selection.extendLastSelection(position, false, selection.keepX);
            else
                selection.setSelection(position, position, false, selection.keepX, UpdateInputPositionProperties.Yes);
            return true;
        }

        getPositionStartParagraph(): number {
            const selection: Selection = this.control.selection;
            const layout: DocumentLayout = this.control.layout;
            const subDocument: SubDocument = this.control.model.activeSubDocument;
            var firstPageStartPos: number = layout.pages[0].contentIntervals[0].start;
            var currentPos: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;

            let boxIterator: LayoutBoxIteratorBase = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, layout, firstPageStartPos, currentPos) : new LayoutBoxIteratorOtherSubDocument(subDocument, layout, firstPageStartPos, currentPos, this.control.selection.pageIndex);
            if(!boxIterator.isInitialized())
                return -1;

            boxIterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true)); // init first position

            if (boxIterator.position.getLogPosition() == (this.control.model.activeSubDocument.isMain() ? layout.getLastValidPage() : layout.pages[selection.pageIndex]).getEndPosition(this.control.model.activeSubDocument)) { // for case when pos >= LastValidPage().getEndPosition
                if (boxIterator.position.box.getType() == LayoutBoxType.ParagraphMark || boxIterator.position.box.getType() == LayoutBoxType.SectionMark)
                    return boxIterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box);
                return boxIterator.position.getLogPosition();
            }

            if (boxIterator.position.charOffset == 0)
                boxIterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
            while (boxIterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true))) {
                if (boxIterator.position.box.getType() == LayoutBoxType.ParagraphMark || boxIterator.position.box.getType() == LayoutBoxType.SectionMark) {
                    boxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
                    return boxIterator.position.getLogPosition();
                }
            }

            return firstPageStartPos;
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class GoToParagraphStartCommand extends GoToParagraphStartCommandBase {
        extendSelection(): boolean {
            return false;
        }
    }

    export class ExtendGoToParagraphStartCommand extends GoToParagraphStartCommandBase {
        extendSelection(): boolean {
            return true;
        }
    }
} 