module __aspxRichEdit {
    export class DeleteCommand extends CommandBase<SimpleCommandState> {
        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        // parameter must be null or undefined
        executeCore(state: ICommandState, parameter: string): boolean {
            this.control.history.beginTransaction();
            var selectionState = this.control.selection.getSelectionState();
            for(let i = 0, selectionInterval: LinkedInterval; selectionInterval = selectionState.intervals[i]; i++) {
                var removingInterval = selectionInterval.getFixedInterval();
                var isIntervalCollapsed: boolean = selectionInterval.getLength() == 0;
                if(isIntervalCollapsed) {
                    var subDocument = this.control.model.activeSubDocument;
                    var layoutPosition: LayoutPosition = (subDocument.isMain()
                        ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, selectionInterval.start.value, DocumentLayoutDetailsLevel.Character)
                        : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, selectionInterval.start.value, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                        .create(new LayoutPositionCreatorConflictFlags().setDefault(this.control.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
                    layoutPosition.switchToStartNextBoxInRow();
                    removingInterval.start = layoutPosition.getLogPosition();
                    removingInterval.length = 1;
                }

                if(selectionInterval.end.value === this.control.model.activeSubDocument.getDocumentEndPosition() && removingInterval.length === 1)
                    continue;

                if(isIntervalCollapsed) {
                    var fields: Field[] = this.control.model.activeSubDocument.fields;
                    var fieldIndex: number = Field.normedBinaryIndexOf(fields, removingInterval.start + 1);
                    var field: Field = fields[fieldIndex];

                    while(field) {
                        if((field.showCode ? field.getFieldStartPosition() : field.getResultStartPosition()) == removingInterval.start ||
                            (field.showCode ? field.getSeparatorPosition() : field.getResultEndPosition()) == removingInterval.start) {
                            removingInterval = field.getAllFieldInterval();
                            this.control.selection.setSelection(removingInterval.start, removingInterval.end(), this.control.selection.endOfLine, this.control.selection.keepX, UpdateInputPositionProperties.Yes);
                            break;
                        }
                        field = field.parent;
                    }
                }

                ModelManipulator.removeInterval(this.control, this.control.model.activeSubDocument, removingInterval, false);
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(removingInterval.start, 0), UpdateInputPositionProperties.Yes, this.control.selection.endOfLine);
            }
            selectionState.destructor();
            this.control.history.endTransaction();
            return true;
        }
    }
}     