module __aspxRichEdit {
    export class RemoveHyperlinksCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.hyperlinks);
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var fields: Field[] = subDocument.fields;
            if (fields.length < 1)
                return false;

            var history: IHistory = this.control.history;
            var interval: FixedInterval = this.control.selection.getLastSelectedInterval();
            var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(fields, interval.start));
            var field: Field = fields[fieldIndex].getAbsolutelyTopLevelField(fields);

            var linkedInterval: LinkedInterval = interval.getLinkedInterval(subDocument.positionManager);
            history.beginTransaction();
            var selection: Selection = this.control.selection;

            history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, subDocument, [new FixedInterval(0, 0)], this.control.selection, UpdateInputPositionProperties.No,
                this.control.selection.endOfLine));// need for keep right selection
            for (fieldIndex = field.index; (field = fields[fieldIndex]) && field.getFieldStartPosition() < linkedInterval.end.value;) {
                if (FixedInterval.getIntersection(FixedInterval.fromPositions(field.getCodeStartPosition(), field.getResultEndPosition()), interval))
                    history.addAndRedo(new RemoveHyperlinkHistoryItem(this.control.modelManipulator, field));
                else
                    fieldIndex++;
            }
            history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, subDocument, [linkedInterval.getFixedInterval()], this.control.selection, UpdateInputPositionProperties.Yes,
                this.control.selection.endOfLine));
            history.endTransaction();
            linkedInterval.destructor(subDocument.positionManager);
            return true;
        }
    }
} 