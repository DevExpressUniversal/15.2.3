module __aspxRichEdit {
    export class GoToNextCharacterCommandBase extends SelectionCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            var position: number = this.getPosition();
            if (position == -1)
                return false;
            this.setSelection(position);
            return true;
        }

        setSelection(position: number) {
            throw new Error(Errors.NotImplemented);  
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }
        
        private getPosition(): number {
            var selection: Selection = this.control.selection;
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var initialModelPosition: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;
            var initialLayoutPosition: LayoutPosition = subDocument.isMain()
                ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, initialModelPosition, DocumentLayoutDetailsLevel.Character,
                    new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setCustom(true, true, false, false))
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, initialModelPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setCustom(true, true, false, false));

            var nextCharLayoutPosition: LayoutPosition;
            if (this.extendSelection()) {
                nextCharLayoutPosition = initialLayoutPosition;
                if (nextCharLayoutPosition.getLogPosition() == initialModelPosition) {
                    nextCharLayoutPosition = this.getNextCharacterPosition(nextCharLayoutPosition);
                    if (nextCharLayoutPosition.isPositionBeforeFirstBoxInRow())
                        nextCharLayoutPosition = this.getNextCharacterPosition(nextCharLayoutPosition);
                }
                else
                    if (nextCharLayoutPosition.isPositionBeforeFirstBoxInRow())
                        nextCharLayoutPosition = this.getNextCharacterPosition(nextCharLayoutPosition);
            }
            else {
                if (!selection.isCollapsed()) {
                    var pos: number;
                    if (selection.forwardDirection) {
                        pos = initialLayoutPosition.getLogPosition();
                    }
                    else {
                        var selectionEndPosition: number = selection.getLastSelectedInterval().end();
                        pos = (subDocument.isMain()
                            ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, selectionEndPosition, DocumentLayoutDetailsLevel.Character,
                                new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(true))
                            : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, selectionEndPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                                .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(true))).getLogPosition();
                    }
                    return pos < subDocument.getDocumentEndPosition() ? pos : pos - 1;
                }
                nextCharLayoutPosition = initialLayoutPosition;

                var needFindPosition: boolean = nextCharLayoutPosition.getLogPosition() == initialModelPosition;
                if (!needFindPosition)
                    needFindPosition = Field.binaryIndexOf(subDocument.fields, initialModelPosition + 1) >= 0;

                if (needFindPosition) {
                    if (nextCharLayoutPosition.box.getLength() == nextCharLayoutPosition.charOffset)
                        nextCharLayoutPosition = this.getNextCharacterPosition(nextCharLayoutPosition);
                    do {
                        var prevLP: LayoutPosition = nextCharLayoutPosition;
                        nextCharLayoutPosition = this.getNextCharacterPosition(prevLP);
                        if (prevLP === nextCharLayoutPosition) { // as reference
                            nextCharLayoutPosition = initialLayoutPosition;
                            break;
                        }
                    } while(!(nextCharLayoutPosition.box.isVisible() || nextCharLayoutPosition.isPositionBeforeFirstBoxInRow()))
                }
            }
            var nextCharModelPosition: number = nextCharLayoutPosition.getLogPosition();
            return nextCharModelPosition == initialModelPosition ? -1 : nextCharModelPosition;
        }

        private getNextCharacterPosition(layoutPosition: LayoutPosition): LayoutPosition {
            var nextLayoutPosition: LayoutPosition = layoutPosition.clone();

            if (nextLayoutPosition.charOffset + 1 <= nextLayoutPosition.box.getLength()) {
                nextLayoutPosition.charOffset++;
                return nextLayoutPosition;
            }
            nextLayoutPosition.charOffset = 0;

            if (nextLayoutPosition.boxIndex + 1 < nextLayoutPosition.row.boxes.length) {
                nextLayoutPosition.boxIndex++;
                nextLayoutPosition.box = nextLayoutPosition.row.boxes[nextLayoutPosition.boxIndex];
                return nextLayoutPosition;
            }
            nextLayoutPosition.boxIndex = 0;

            if (nextLayoutPosition.advanceToNextRow(this.control.layout)) {
                nextLayoutPosition.box = nextLayoutPosition.row.boxes[0];
                return nextLayoutPosition;
            }
            else
                return layoutPosition;
        }
    }

    export class GoToNextCharacterCommand extends GoToNextCharacterCommandBase {
        setSelection(position: number) {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            this.control.selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
        }

        extendSelection(): boolean {
            return false;
        }
    }

    export class ExtendGoToNextCharacterCommand extends GoToNextCharacterCommandBase {
        setSelection(position: number) {
            var selection: Selection = this.control.selection;
            selection.extendLastSelection(position, false, -1, UpdateInputPositionProperties.Yes, Field.jumpThroughFieldToRight);
        }

        extendSelection(): boolean {
            return true;
        }
    }
}   