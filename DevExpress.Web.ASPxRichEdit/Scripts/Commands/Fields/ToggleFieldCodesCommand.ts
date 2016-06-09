module __aspxRichEdit {
    export class ToggleFieldCodesCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = FieldContextMenuHelper.showUpdateAndToogleCodeItems(this.control.model.activeSubDocument.fields, this.control.selection.intervals);
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields);
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            var selection: Selection = this.control.selection;
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var fields: Field[] = subDocument.fields;
            var intervals: FixedInterval[] = this.control.selection.intervals;

            var commandExecuted: boolean = false;
            for (var intervalIndex: number = 0, interval: FixedInterval; interval = intervals[intervalIndex]; intervalIndex++) {
                if (interval.length == 0) {
                    var fieldVisabilityInfo: FieldVisabilityInfo[] = FieldVisabilityInfo.getRelativeVisabilityInfo(interval.start, fields);
                    for (var infoIndex: number = fieldVisabilityInfo.length - 1, info: FieldVisabilityInfo; info = fieldVisabilityInfo[infoIndex]; infoIndex--) {
                        var field: Field = info.field;
                        var newSelectionStartPos: number = -1;
                        if (info.showResult) {
                            field.showCode = true;
                            newSelectionStartPos = field.getCodeStartPosition();
                        }
                        if (info.showCode) {
                            field.showCode = false;
                            newSelectionStartPos = field.getFieldStartPosition();
                        }
                        if (newSelectionStartPos >= 0) {
                            this.control.formatterOnIntervalChanged(field.getAllFieldInterval(), subDocument);
                            selection.setSelection(newSelectionStartPos, newSelectionStartPos, selection.endOfLine, -1, UpdateInputPositionProperties.Yes);
                            commandExecuted = true;
                            break;
                        }
                    }

                    // case when field.startPosition.value == interval.start
                    if (fieldVisabilityInfo.length == 0) {
                        var fieldIndex: number = Field.normedBinaryIndexOf(fields, interval.start + 1);
                        if (fieldIndex >= 0) {
                            var field: Field = fields[fieldIndex];
                            if (!field.parent && field.getFieldStartPosition() == interval.start) {
                                field.showCode = !field.showCode;
                                this.control.formatterOnIntervalChanged(field.getAllFieldInterval(), subDocument);
                                selection.setSelection(field.getFieldStartPosition(), field.getFieldStartPosition(), selection.endOfLine, -1, UpdateInputPositionProperties.Yes);
                                commandExecuted = true;
                            }
                        }
                    }
                }
                else {
                    var intervalEnd: number = interval.end();

                    var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(fields, interval.start + 1));
                    var field: Field = fields[fieldIndex];

                    while (field.parent) {
                        if (interval.start < field.getFieldStartPosition() || field.getFieldEndPosition() <= interval.start)
                            field = field.parent;
                        else
                            break;
                    }

                    fieldIndex = field.index;
                    commandExecuted = true;
                    var needSetState: boolean = !field.showCode;
                    field.showCode = needSetState;
                    this.control.formatterOnIntervalChanged(field.getAllFieldInterval(), subDocument);

                    if (field.getFieldStartPosition() != interval.start && field.getAllFieldInterval().contains(interval.start)) {
                        var newSelectionPos: number = needSetState ? field.getCodeStartPosition() : field.getFieldStartPosition();
                        selection.setSelection(newSelectionPos, newSelectionPos, selection.endOfLine, -1, UpdateInputPositionProperties.Yes);
                    }

                    for (fieldIndex++; field = fields[fieldIndex]; fieldIndex++) {
                        if (field.getFieldStartPosition() >= intervalEnd)
                            break;

                        if (field.getFieldEndPosition() <= interval.start)
                            continue;

                        field.showCode = needSetState;
                        this.control.formatterOnIntervalChanged(field.getAllFieldInterval(), subDocument);
                    }
                }
            }
            return commandExecuted;
        }

        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
} 