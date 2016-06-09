module __aspxRichEdit {
    export class GoToNextPageCommandBase extends SelectionCommandBase {
        getPosition(): number {
            var selection: Selection = this.control.selection;
            var initPosition: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;
            var layoutPosition: LayoutPosition = new LayoutPositionMainSubDocumentCreator(this.control.layout, this.control.model.activeSubDocument, initPosition,
                DocumentLayoutDetailsLevel.Character)
                .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if(!layoutPosition)
                return -1;
            var charOffset: number = initPosition - layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Box);
            var x = layoutPosition.pageArea.x + layoutPosition.column.x + layoutPosition.row.x + layoutPosition.box.x + layoutPosition.box.getCharOffsetXInPixels(this.control.measurer, charOffset);
            var y = layoutPosition.pageArea.y + layoutPosition.column.y + layoutPosition.row.y + layoutPosition.box.y;

            var siblingPageIndex: number = layoutPosition.pageIndex + 1;
            var siblingPage: LayoutPage = this.control.forceFormatPage(siblingPageIndex);
            if(siblingPage) {
                var point: LayoutPoint = new LayoutPoint(siblingPageIndex, x, y);
                var htr: HitTestResult = this.control.hitTestManager.calculate(point, DocumentLayoutDetailsLevel.Character, this.control.model.activeSubDocument);
                if (!this.extendSelection())
                    htr.correctAsVisibleBox();
                return htr.getPosition();
            }
            else
                return this.control.model.activeSubDocument.getDocumentEndPosition() - 1;
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }

        isEnabled(): boolean {
            return super.isEnabled() && this.control.model.activeSubDocument.isMain();
        }
    }

    export class GoToNextPageCommand extends GoToNextPageCommandBase {
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

    export class ExtendGoToNextPageCommand extends GoToNextPageCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            var selection: Selection = this.control.selection;
            var position = this.getPosition();
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