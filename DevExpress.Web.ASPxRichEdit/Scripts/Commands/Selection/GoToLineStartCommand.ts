module __aspxRichEdit {
    export class GoToLineStartCommandBase extends SelectionCommandBase {
        getStartPosition(): number {
            var selection: Selection = this.control.selection;
            var position: number = selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;
            var subDocument = this.control.model.activeSubDocument;
            var layoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, position, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, position, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(true));
            if(!layoutPosition)
                return -1;
            return layoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Row);
        }
    }

    export class GoToLineStartCommand extends GoToLineStartCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var pos: number = this.getStartPosition();
            if(pos < 0)
                return false;
            this.control.selection.setSelection(pos, pos, false, -1, UpdateInputPositionProperties.Yes);
            return true;
        }
    }

    export class ExtendGoToLineStartCommand extends GoToLineStartCommandBase {
        executeCore(state: ICommandState, parameter: any): boolean {
            var selection: Selection = this.control.selection;
            var pos: number = this.getStartPosition();
            if(pos < 0)
                return false;
            selection.extendLastSelection(pos, false, -1);
            return true;
        }
    }
}