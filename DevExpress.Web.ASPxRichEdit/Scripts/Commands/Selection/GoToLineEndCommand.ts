module __aspxRichEdit {
    export class GoToLineEndCommandBase extends SelectionCommandBase {
        endOfLine: boolean;
        executeCore(state: ICommandState, parameter: any): boolean {
            var selection: Selection = this.control.selection;
            var endSelection: number = this.getEndPosition();
            if(endSelection < 0)
                return false;
            this.setSelection(endSelection, -1);
            return true;
        }

        setSelection(pos: number, keepX: number) {
            throw new Error(Errors.NotImplemented);
        }

        getEndPosition(): number {
            var selection: Selection = this.control.selection;
            this.endOfLine = true;
            var position: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;    

            var subDocument = this.control.model.activeSubDocument;
            var layoutPosition: LayoutPosition = (subDocument.isMain() 
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, position, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, position, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if(!layoutPosition)
                return -1;
            return this.getEndPositionCore(layoutPosition);
        }

        getEndPositionCore(layoutPosition: LayoutPosition): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class GoToLineEndCommand extends GoToLineEndCommandBase {
        setSelection(pos: number, keepX: number) {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var selection: Selection = this.control.selection;
            selection.setSelection(pos, pos, this.endOfLine, keepX, UpdateInputPositionProperties.Yes);
        }
        getEndPositionCore(layoutPosition: LayoutPosition): number {
            var lastBoxInRow: LayoutBox = layoutPosition.row.getLastVisibleBox();
            if(!lastBoxInRow)
                this.endOfLine = false;
            return layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Row) + (lastBoxInRow ? lastBoxInRow.getEndPosition() : 0);
        }
    }

    export class ExtendGoToLineEndCommand extends GoToLineEndCommandBase {
        setSelection(pos: number, keepX: number) {
            var selection: Selection = this.control.selection;
            selection.extendLastSelection(pos, this.endOfLine, keepX);
        }
        getEndPositionCore(layoutPosition: LayoutPosition): number {
            return layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Row) + layoutPosition.row.getLastBoxEndPositionInRow();
        }
    }
} 