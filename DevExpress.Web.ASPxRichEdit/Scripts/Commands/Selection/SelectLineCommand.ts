module __aspxRichEdit {
    export class SelectLineCommand extends SelectionCommandBase {
        executeCore(state: ICommandState, clickPosition: number): boolean {
            var selection: Selection = this.control.selection;
            var subDocument = this.control.model.activeSubDocument;
            var clickLayoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, clickPosition, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, clickPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
            if(!clickLayoutPosition)
                return false;
            var clickRowEndBox: LayoutBox = clickLayoutPosition.row.boxes[clickLayoutPosition.row.boxes.length - 1];

            var clickRowStartPosition = clickLayoutPosition.getRelatedSubDocumentPagePosition() + clickLayoutPosition.pageArea.pageOffset + clickLayoutPosition.column.pageAreaOffset + clickLayoutPosition.row.columnOffset;
            var clickRowEndPosition = clickRowStartPosition + clickRowEndBox.rowOffset + clickRowEndBox.getLength();
            this.setSelection(clickRowStartPosition, clickRowEndPosition);
            return true;
        }

        setSelection(startPosition: number, endPosition: number) {
            this.control.selection.setSelection(startPosition, endPosition, false, -1, UpdateInputPositionProperties.Yes);
        }
    }

    export class AddSelectedLineCommandNoUpdateControlState extends SelectLineCommand {
        setSelection(startPosition: number, endPosition: number) {
            this.control.selection.addSelection(startPosition, endPosition, false, -1);
        }
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
    }

    export class ExtendSelectLineCommand extends SelectionCommandBase {
        executeCore(state: ICommandState, clickPosition: number): boolean {
            var selection: Selection = this.control.selection;

            var currentInterval: FixedInterval = selection.getLastSelectedInterval();
            var intervalEnd: number = currentInterval.end();
            if (intervalEnd == this.control.model.activeSubDocument.getDocumentEndPosition()) // or endSelectionRowStartLayoutPosition will be null
                intervalEnd--;

            var subDocument = this.control.model.activeSubDocument;
            var startSelectionRowStartLayoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, currentInterval.start, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, currentInterval.start, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if(!startSelectionRowStartLayoutPosition)
                return false;
            var startSelectionEndBox: LayoutBox = startSelectionRowStartLayoutPosition.row.boxes[startSelectionRowStartLayoutPosition.row.boxes.length - 1];

            var endSelectionRowStartLayoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, intervalEnd, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, intervalEnd, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if(!endSelectionRowStartLayoutPosition)
                return false;
            var endSelectionEndBox: LayoutBox = endSelectionRowStartLayoutPosition.row.boxes[endSelectionRowStartLayoutPosition.row.boxes.length - 1];

            var clickRowStartLayoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, clickPosition, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, clickPosition, selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if(!clickRowStartLayoutPosition)
                return false;
            var clickEndBox: LayoutBox = clickRowStartLayoutPosition.row.boxes[clickRowStartLayoutPosition.row.boxes.length - 1];

            var startSelectionRowStartPosition: number = startSelectionRowStartLayoutPosition.getRelatedSubDocumentPagePosition() + startSelectionRowStartLayoutPosition.pageArea.pageOffset +
                                                         startSelectionRowStartLayoutPosition.column.pageAreaOffset + startSelectionRowStartLayoutPosition.row.columnOffset;
            var startSelectionRowEndPosition: number = startSelectionRowStartPosition + startSelectionEndBox.rowOffset + startSelectionEndBox.getLength();

            var endSelectionRowStartPosition: number = endSelectionRowStartLayoutPosition.getRelatedSubDocumentPagePosition() + endSelectionRowStartLayoutPosition.pageArea.pageOffset +
                                                       endSelectionRowStartLayoutPosition.column.pageAreaOffset + endSelectionRowStartLayoutPosition.row.columnOffset;
            var endSelectionRowEndPosition: number = endSelectionRowStartPosition + endSelectionEndBox.rowOffset + endSelectionEndBox.getLength();
            
            var clickStartRowPosition: number = clickRowStartLayoutPosition.getRelatedSubDocumentPagePosition() + clickRowStartLayoutPosition.pageArea.pageOffset +
                                                clickRowStartLayoutPosition.column.pageAreaOffset + clickRowStartLayoutPosition.row.columnOffset;
            var clickEndRowPosition: number = clickStartRowPosition + clickEndBox.rowOffset + clickEndBox.getLength();

            if (currentInterval.start == startSelectionRowStartPosition && currentInterval.end() == endSelectionRowEndPosition) { // if selected some number FULL rows
                if (selection.forwardDirection) {
                    if (clickStartRowPosition < startSelectionRowStartPosition)
                        selection.setSelection(startSelectionRowEndPosition, clickStartRowPosition, false, -1, UpdateInputPositionProperties.Yes);
                    else
                        selection.setSelection(startSelectionRowStartPosition, clickEndRowPosition, false, -1, UpdateInputPositionProperties.Yes);
                }
                else {
                    if (clickStartRowPosition >= endSelectionRowStartPosition)
                        selection.setSelection(endSelectionRowStartPosition, clickEndRowPosition, false, -1, UpdateInputPositionProperties.Yes);
                    else
                        selection.setSelection(endSelectionRowEndPosition, clickStartRowPosition, false, -1, UpdateInputPositionProperties.Yes);
                }
            }
            else { // if selected some number NOT FULL rows
                if (selection.forwardDirection) {
                    var selectionEnd: number = clickStartRowPosition <= currentInterval.start ? Math.min(clickStartRowPosition, startSelectionRowStartPosition) : clickStartRowPosition;
                    selection.setSelection(currentInterval.start, selectionEnd, false, -1, UpdateInputPositionProperties.Yes);
                }
                else
                    selection.setSelection(intervalEnd, clickStartRowPosition, false, -1, UpdateInputPositionProperties.Yes);
            }
            return true;
        }
    }

    export class SelectLineCommandNoUpdateControlState extends SelectLineCommand {
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
    }

    export class ExtendSelectLineCommandNoUpdateControlState extends ExtendSelectLineCommand {
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
    }
} 