module __aspxRichEdit {
    export class LineDownCommand extends GoToLineVerticallyCommandBase {
        public extendSelection(): boolean {
            return false;
        }

        public canAdvanceToNextRow(cellInterator: TableCellIterator): boolean {
            return cellInterator.tryAdvanceToBelowRow();
        }

        public getNewLayoutPositionRowLevel(oldLayoutPosition: LayoutPosition): LayoutPosition {
            var newLayoutPosition: LayoutPosition = oldLayoutPosition.clone();
            if (newLayoutPosition.advanceToNextRow(this.control.layout))
                return newLayoutPosition;
            else
                return null;
        }

        public getInitialSelectionEndPosition(): number {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            if (this.extendSelection() && !this.control.selection.forwardDirection)
                return this.control.selection.getLastSelectedInterval().start;
            else
                return this.control.selection.getLastSelectedInterval().end();
        }
    }

    export class ExtendLineDownCommand extends LineDownCommand {
        public extendSelection(): boolean {
            return true;
        }
        public canAdvanceToNextRow(cellInterator: TableCellIterator): boolean {
            return false;
        }
    }
}