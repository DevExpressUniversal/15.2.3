module __aspxRichEdit {
    export class GoToPrevPageCommandBase extends SelectionCommandBase {
        getPosition(): number {
            var selection: Selection = this.control.selection;
            var initPosition = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;
            var layoutPosition: LayoutPosition = new LayoutPositionMainSubDocumentCreator(this.control.layout, this.control.model.activeSubDocument, initPosition, DocumentLayoutDetailsLevel.Box)
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
            if(!layoutPosition)
                return -1;
            var charOffset: number = initPosition - layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Box);
            var x = layoutPosition.pageArea.x + layoutPosition.column.x + layoutPosition.row.x + layoutPosition.box.x + layoutPosition.box.getCharOffsetXInPixels(this.control.measurer, charOffset);
            var y = layoutPosition.pageArea.y + layoutPosition.column.y + layoutPosition.row.y + layoutPosition.box.y;

            var siblingPageIndex = layoutPosition.pageIndex - 1;
            var siblingPage = this.control.layout.pages[siblingPageIndex];

            var position = initPosition;
            if(siblingPage) {
                var point = new LayoutPoint(siblingPageIndex, x, y);
                var htr = this.control.hitTestManager.calculate(point, DocumentLayoutDetailsLevel.Character, this.control.model.activeSubDocument);
                if (!this.extendSelection())
                    htr.correctAsVisibleBox();
                position = htr.getPosition();
            }
            else
                position = 0;
            return position;
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }

        isEnabled(): boolean {
            return super.isEnabled() && this.control.model.activeSubDocument.isMain();
        }
    }

    export class GoToPrevPageCommand extends GoToPrevPageCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var selection: Selection = this.control.selection;
            var position: number = this.getPosition();
            if(position < 0)
                return false;
            selection.setSelection(position, position, selection.endOfLine, selection.keepX, UpdateInputPositionProperties.Yes);
            return true;
        }

        extendSelection(): boolean {
            return false;
        }
    }

    export class ExtendGoToPrevPageCommand extends GoToPrevPageCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            var selection: Selection = this.control.selection;
            var position: number = this.getPosition();
            if(position < 0)
                return false;
            selection.extendLastSelection(position, selection.endOfLine, selection.keepX);
            return true;
        }

        extendSelection(): boolean {
            return true;
        }
    }
}