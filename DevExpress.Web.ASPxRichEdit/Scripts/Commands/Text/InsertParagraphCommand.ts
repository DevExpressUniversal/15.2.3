module __aspxRichEdit {
    // tests in \Tests\Scripts\Commands\Insert_CommandTests.ts
    export class InsertParagraphCommand extends CommandBase<SimpleCommandState> {
        lock: boolean;

        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphs);
        }

         // parameter must be null or undefined
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            var interval: FixedInterval = this.control.selection.getLastSelectedInterval();
            this.lock = interval.length == 0;

            if(this.tryInsertParagraphBeforeTable(interval))
                return true;

            if(interval.length === 0 || interval.start === 1) {
                var paragraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, p => p.startLogPosition.value - interval.start);
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                if(paragraph.startLogPosition.value === interval.start && paragraph.isInList() && paragraph.length === 1) {
                    this.control.history.addAndRedo(new RemoveParagraphFromListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphIndex));
                    return true;
                }
            }
            ModelManipulator.insertParagraph(this.control, this.control.model.activeSubDocument, interval.clone());
            return true;
        }

        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return this.lock;
        }

        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return this.lock;
        }

        tryInsertParagraphBeforeTable(interval: FixedInterval) {
            if(interval.start > 0 || interval.length > 0)
                return false;
            let subDocument = this.control.model.activeSubDocument;
            let firstTable = subDocument.tables[0];
            if(firstTable && firstTable.getStartPosition() === 0) {
                this.control.history.beginTransaction();
                TablesManipulator.insertParagraphToTheCellStartAndShiftContent(this.control, subDocument, firstTable.rows[0].cells[0]);
                this.control.history.addAndRedo(new ShiftTableStartPositionToTheRightHistoryItem(this.control.modelManipulator, subDocument, firstTable));
                ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(0, 0), UpdateInputPositionProperties.Yes, false);
                this.control.history.endTransaction();
                return true;
            }
        }
    }
}    