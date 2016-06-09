module __aspxRichEdit {
    export class GoToPrevCharacterCommandBase extends SelectionCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            var position: number = this.getPosition();
            if (position == -1)
                return false;
            this.setSelection(position);
            return true;
        }

        getPosition(): number {
            var fields: Field[] = this.control.model.activeSubDocument.fields;
            var selection: Selection = this.control.selection;
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var initialModelPosition: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;
            var initialLayoutPosition: LayoutPosition = subDocument.isMain()
                ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, initialModelPosition, DocumentLayoutDetailsLevel.Character,
                    new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false))
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, initialModelPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
            var prevCharLayoutPosition: LayoutPosition;

            if (this.extendSelection()) {
                prevCharLayoutPosition = this.getPrevCharacterPosition(initialLayoutPosition);
                //var prevCharModelPosition: number = prevCharLayoutPosition.getLogPosition();
                //return prevCharModelPosition == initialModelPosition ? -1 : Field.correctWhenPositionInStartCode(fields, prevCharModelPosition);
            }
            else {
                if (!selection.isCollapsed()) {
                    if (selection.forwardDirection) {
                        var selectionEndPosition: number = selection.getLastSelectedInterval().start;
                        return (subDocument.isMain()
                            ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, selectionEndPosition,
                                DocumentLayoutDetailsLevel.Character, new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(true))
                            : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, selectionEndPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                                .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(true))).getLogPosition();
                    }
                    else
                        return initialLayoutPosition.getLogPosition();
                }

                prevCharLayoutPosition = this.getPrevCharacterPosition(initialLayoutPosition);
                if (!prevCharLayoutPosition.box.isVisible() && !prevCharLayoutPosition.isPositionBeforeFirstBoxInRow()) {
                    do {
                        var prevLP: LayoutPosition = prevCharLayoutPosition;
                        prevCharLayoutPosition = this.getPrevCharacterPosition(prevCharLayoutPosition);
                    } while (!prevCharLayoutPosition.box.isVisible() && prevLP !== prevCharLayoutPosition)
                    prevCharLayoutPosition = prevLP;
                }
            }
            var prevCharModelPosition: number = prevCharLayoutPosition.getLogPosition();
            return prevCharModelPosition == initialModelPosition ? -1 : prevCharModelPosition;
        }

        getPrevCharacterPosition(layoutPosition: LayoutPosition): LayoutPosition {
            var prevLayoutPosition: LayoutPosition = layoutPosition.clone();
            if (prevLayoutPosition.charOffset > 0) {
                prevLayoutPosition.charOffset--;
                return prevLayoutPosition;
            }
            if (prevLayoutPosition.boxIndex - 1 >= 0) {
                prevLayoutPosition.boxIndex--;
                prevLayoutPosition.box = prevLayoutPosition.row.boxes[prevLayoutPosition.boxIndex];
                prevLayoutPosition.charOffset = prevLayoutPosition.box.getLength() - 1;
                return prevLayoutPosition;
            }
            if (prevLayoutPosition.advanceToPrevRow(this.control.layout)) {
                prevLayoutPosition.boxIndex = prevLayoutPosition.row.boxes.length - 1;
                prevLayoutPosition.box = prevLayoutPosition.row.boxes[prevLayoutPosition.boxIndex];
                prevLayoutPosition.charOffset = prevLayoutPosition.box.getLength() - 1;
                return prevLayoutPosition;
            }
            else
                return layoutPosition;
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }

        setSelection(position: number) {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class GoToPrevCharacterCommand extends GoToPrevCharacterCommandBase {
        setSelection(position: number) {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            this.control.selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
        }

        extendSelection(): boolean {
            return false;
        }
    }

    export class ExtendGoToPrevCharacterCommand extends GoToPrevCharacterCommandBase {
        setSelection(position: number) {
            var selection: Selection = this.control.selection;
            selection.extendLastSelection(position, false, -1, UpdateInputPositionProperties.Yes, Field.jumpThroughFieldToLeft);
        }

        extendSelection(): boolean {
            return true;
        }
    }

}    