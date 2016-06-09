module __aspxRichEdit {
    export class GoToLineAboveCommand extends GoToLineVerticallyCommandBase {
        public extendSelection(): boolean {
            return false;
        }

        public canAdvanceToNextRow(cellInterator: TableCellIterator): boolean {
            return cellInterator.tryAdvanceToAboveRow();
        }

        public getNewLayoutPositionRowLevel(oldLayoutPosition: LayoutPosition): LayoutPosition {
            var newLayoutPosition: LayoutPosition = oldLayoutPosition.clone();
            if (newLayoutPosition.advanceToPrevRow(this.control.layout))
                return newLayoutPosition;
            else
                return null;
        }

        public getInitialSelectionEndPosition(): number {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            if(this.extendSelection() && this.control.selection.forwardDirection)
                return this.control.selection.getLastSelectedInterval().end();
            else
                return this.control.selection.getLastSelectedInterval().start;
        }
    }

    export class ExtendGoToLineAboveCommand extends GoToLineAboveCommand {
        public extendSelection(): boolean {
            return true;
        }
        public canAdvanceToNextRow(cellInterator: TableCellIterator): boolean {
            return false;
        }
    }
} 