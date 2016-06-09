module __aspxRichEdit {
    export class ShowAllFieldCodesCommand extends CommandBase<IntervalCommandState> {
        getState(): IntervalCommandState {
            return new IntervalCommandState(this.isEnabled(), this.control.selection.getLastSelectedInterval());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields);
        }

        // undefined - calculate what position we start redraw
        executeCore(state: IntervalCommandState, parameter: any): boolean {
            var subDocumentsList: SubDocument[] = [this.control.model.activeSubDocument];
            var atLeastOneSubDocRecalculated: boolean = false;
            for (var subDocumentIndex: number = 0, currSubDoc: SubDocument; currSubDoc = subDocumentsList[subDocumentIndex]; subDocumentIndex++) {
                var fields: Field[] = currSubDoc.fields;
                var fieldIndexStartRecalculateLayout: number = null;
                for (var fieldIndex: number = 0, field: Field; field = fields[fieldIndex]; fieldIndex++) {
                    if (!field.showCode) {
                        if (fieldIndexStartRecalculateLayout == null)
                            fieldIndexStartRecalculateLayout = fieldIndex;
                        field.showCode = true;
                    }
                }

                if (fieldIndexStartRecalculateLayout != null) {
                    atLeastOneSubDocRecalculated = true;
                    // or here invalidate separately all fields
                    this.control.formatterOnIntervalChanged(FixedInterval.fromPositions(fields[fieldIndexStartRecalculateLayout].getFieldStartPosition(), fields[fields.length - 1].getFieldEndPosition()), currSubDoc);
                }
            }
            return atLeastOneSubDocRecalculated;
        }

        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
} 