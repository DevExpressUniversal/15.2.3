module __aspxRichEdit {
    export class GoToParagraphEndCommandBase extends SelectionCommandBase {
        executeCore(state: ICommandState): boolean {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var selection: Selection = this.control.selection;
            var position: number = this.getPositionEndParagraph();
            if (position < 0)
                return false;

            if (this.extendSelection())
                selection.extendLastSelection(position, false, selection.keepX);
            else
                selection.setSelection(position, position, false, selection.keepX, UpdateInputPositionProperties.Yes);
            return true;
        }

        getPositionEndParagraph(): number {
            var selection: Selection = this.control.selection;
            const subDocument: SubDocument = this.control.model.activeSubDocument;
            var layout: DocumentLayout = this.control.layout;
            var lastValidPage = subDocument.isMain() ? layout.getLastValidPage() : layout.pages[selection.pageIndex];
            if(!lastValidPage)
                return -1;
            var lastPageEndPos: number = lastValidPage.getEndPosition(this.control.model.activeSubDocument);
            var currentPos: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;

            let boxIterator: LayoutBoxIteratorBase = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, layout, currentPos, lastPageEndPos) : new LayoutBoxIteratorOtherSubDocument(subDocument, layout, currentPos, lastPageEndPos, this.control.selection.pageIndex);
            if(!boxIterator.isInitialized())
                return -1;
            
            while (boxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true))) {
                if (boxIterator.position.box.getType() == LayoutBoxType.ParagraphMark || boxIterator.position.box.getType() == LayoutBoxType.SectionMark) {
                    boxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
                    if (boxIterator.position.getLogPosition() == lastPageEndPos)
                        return boxIterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box);
                    return boxIterator.position.getLogPosition();
                }
            }

            if (boxIterator.position.box.getType() == LayoutBoxType.ParagraphMark || boxIterator.position.box.getType() == LayoutBoxType.SectionMark)
                return boxIterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box);
            return boxIterator.position.getLogPosition();
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class GoToParagraphEndCommand extends GoToParagraphEndCommandBase {
        extendSelection(): boolean {
            return false;
        }
    }

    export class ExtendGoToParagraphEndCommand extends GoToParagraphEndCommandBase {
        extendSelection(): boolean {
            return true;
        }
    }
} 