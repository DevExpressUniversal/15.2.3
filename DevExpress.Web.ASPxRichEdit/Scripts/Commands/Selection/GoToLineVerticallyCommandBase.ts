module __aspxRichEdit {
    export class GoToLineVerticallyCommandBase extends SelectionCommandBase {
        public executeCore(state: ICommandState): boolean {
            var selection: Selection = this.control.selection;
            var layout: DocumentLayout = this.control.layout;
            var initialSelectionEndPosition: number = this.getInitialSelectionEndPosition();
            var subDocument = this.control.model.activeSubDocument;
            var initialLayoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, initialSelectionEndPosition, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, initialSelectionEndPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if (!initialLayoutPosition)
                return false;

            var keepX: number = selection.keepX;
            if (keepX < 0)
                keepX = initialLayoutPosition.page.x + initialLayoutPosition.pageArea.x + initialLayoutPosition.column.x + initialLayoutPosition.row.x + initialLayoutPosition.box.x +
                        initialLayoutPosition.box.getCharOffsetXInPixels(this.control.measurer, initialLayoutPosition.charOffset);

            var newLayoutPosition: LayoutPosition;
            var cellInterator: TableCellIterator = new TableCellIterator(initialLayoutPosition, layout, this.control.measurer);
            if (this.canAdvanceToNextRow(cellInterator))
                newLayoutPosition = cellInterator.getModifyPosition();
            else {
                newLayoutPosition = this.getNewLayoutPositionRowLevel(initialLayoutPosition);
                if (!newLayoutPosition)
                    return false;
            }

            var xOffsetBoxLevel: number = Math.max(0, keepX - (newLayoutPosition.page.x + newLayoutPosition.pageArea.x + newLayoutPosition.column.x + newLayoutPosition.row.x));
            var boxIndex = Utils.normedBinaryIndexOf(newLayoutPosition.row.boxes, (b: LayoutBox) => b.x - xOffsetBoxLevel);
            if(boxIndex < 0)
                boxIndex = 0;

            newLayoutPosition.box = newLayoutPosition.row.boxes[boxIndex];
            if (!newLayoutPosition.box.isVisible()) {
                var lastVisibleBoxIndexInRow: number = newLayoutPosition.row.getLastVisibleBoxIndex();
                if (lastVisibleBoxIndexInRow < 0) {
                    lastVisibleBoxIndexInRow = 0;
                    var isNoVisibleBoxesInRow: boolean = true;
                }
                newLayoutPosition.boxIndex = lastVisibleBoxIndexInRow;
                newLayoutPosition.box = newLayoutPosition.row.boxes[lastVisibleBoxIndexInRow];
            }
            
            var xOffsetCharLevel: number = isNoVisibleBoxesInRow ? 0 : xOffsetBoxLevel - newLayoutPosition.box.x;
            
            var newLogPosition: number = newLayoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Box) + newLayoutPosition.box.calculateCharOffsetByPointX(this.control.measurer, xOffsetCharLevel);

            if (newLogPosition == initialLayoutPosition.getLogPosition())
                return false;

            var endOfLine: boolean = newLogPosition === newLayoutPosition.getLogPosition(DocumentLayoutDetailsLevel.Row) + newLayoutPosition.row.getLastBoxEndPositionInRow();

            if (this.extendSelection())
                selection.extendLastSelection(newLogPosition, endOfLine, keepX, UpdateInputPositionProperties.Yes);
            else
                selection.setSelection(newLogPosition, newLogPosition, endOfLine, keepX, UpdateInputPositionProperties.Yes);

            return true;
        }

        public canAdvanceToNextRow(cellInterator: TableCellIterator): boolean {
            throw new Error(Errors.NotImplemented);
        }

        public extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }
        
        public getNewLayoutPositionRowLevel(oldLayoutPosition: LayoutPosition): LayoutPosition {
            throw new Error(Errors.NotImplemented);
        }

        public getInitialSelectionEndPosition(): number {
            throw new Error(Errors.NotImplemented);
        }

        // Down + Forward  +          = End
        //      + Forward  + Extended = End
        //      + Backward + -        = End
        //      +          + Extended = Start
        // Up   + Forward  + -        = Start
        //      +          + Extended = End
        //      + Backward + -        = Start
        //      +          + Extended = Start
    }
}  
