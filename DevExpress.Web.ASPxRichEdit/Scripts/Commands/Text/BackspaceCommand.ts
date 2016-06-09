module __aspxRichEdit {
    export class BackspaceCommand extends CommandBase<SimpleCommandState> {
        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        // parameter must be null or undefined
        executeCore(state: ICommandState, parameter: string): boolean {
            var interval: FixedInterval = this.control.selection.getLastSelectedInterval().clone();
            
            if(this.tryChangeParagraphIndent(interval))
                return true;
            var isIntervalCollapsed: boolean = interval.length == 0;

            //let selectedCells = this.get

            var selection: Selection = this.control.selection;
            if (isIntervalCollapsed) {
                let subDocument = this.control.model.activeSubDocument;
                var layoutPosition: LayoutPosition = subDocument.isMain()
                    ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, interval.start,
                        DocumentLayoutDetailsLevel.Character, new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine),
                        new LayoutPositionCreatorConflictFlags().setDefault(false))
                    : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, interval.start, selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                        .create(new LayoutPositionCreatorConflictFlags().setDefault(selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
                layoutPosition.switchToEndPrevBoxInRow();
                interval.start = layoutPosition.getLogPosition() - 1;

                if(interval.start < this.control.layout.pages[0].contentIntervals[0].start)
                    return false;
                interval.length = 1;
            }

            if(interval.end() == this.control.model.activeSubDocument.getDocumentEndPosition() && interval.length === 1)
                return false;

            if (isIntervalCollapsed) {
                var intervalEnd: number = interval.end();
                var fields: Field[] = this.control.model.activeSubDocument.fields;
                var fieldIndex: number = Field.normedBinaryIndexOf(fields, intervalEnd + 1);
                var field: Field = fields[fieldIndex];

                while (field) {
                    if ((field.showCode ? field.getResultStartPosition() : field.getResultEndPosition()) == intervalEnd ||
                        (field.showCode ? field.getFieldStartPosition() : field.getSeparatorPosition()) == interval.start) {
                        interval = field.getAllFieldInterval();
                        selection.setSelection(interval.start, interval.end(), selection.endOfLine, selection.keepX, UpdateInputPositionProperties.Yes);
                        return true;
                    }
                    field = field.parent;
                }
            }
            this.control.history.beginTransaction();
            ModelManipulator.removeInterval(this.control, this.control.model.activeSubDocument, interval, false);
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.Yes, this.control.selection.endOfLine);
            this.control.history.endTransaction();
            return true;
        }

        tryChangeParagraphIndent(interval: FixedInterval): boolean {
            if(interval.length > 0)
                return false;
            var paragraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, p => p.startLogPosition.value - interval.start);
            var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];

            if(interval.start !== paragraph.startLogPosition.value)
                return false;
            if(paragraph.isInList()) {
                this.control.history.addAndRedo(new RemoveParagraphFromListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphIndex));
                return true;
            }
            this.control.history.beginTransaction();
            var paragraphInterval = paragraph.getInterval();
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            var indentsChanged = false;
            if(paragraphProperties.leftIndent > 0) {
                this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphInterval, 0, true));
                indentsChanged = true;
            }
            if(paragraphProperties.firstLineIndent > 0) {
                this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphInterval, 0, true));
                indentsChanged = true;
            }
            if(paragraphProperties.firstLineIndentType !== ParagraphFirstLineIndent.None) {
                this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphInterval, ParagraphFirstLineIndent.None, true));
                indentsChanged = true;
            }
            this.control.history.endTransaction();
            if(indentsChanged)
                return true;
        }
    }
}    