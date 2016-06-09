module __aspxRichEdit {
    export class CreateFieldCommandBase extends CommandBase<IntervalCommandState> {
        getState(): IntervalCommandState {
            return new IntervalCommandState(this.isEnabled(), this.control.selection.getLastSelectedInterval());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields);
        }

        executeCore(state: IntervalCommandState, parameter: any): boolean {
            throw new Error(Errors.NotImplemented);
        }

        public static getIntervalWithoutLastParagraphMark(interval: FixedInterval, subDocument: SubDocument): FixedInterval {
            interval = interval.clone();

            var lastDocPos: number = subDocument.getDocumentEndPosition();
            if (interval.end() == lastDocPos) {
                interval.length--;
                if (interval.length < 0)
                    interval = new FixedInterval(lastDocPos - 1, 0);
            }
            return interval;
        }
    }

    export class CreateFieldCommand extends CreateFieldCommandBase {
        executeCore(state: IntervalCommandState, parameter: any): boolean {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var interval: FixedInterval = CreateFieldCommandBase.getIntervalWithoutLastParagraphMark(this.control.selection.getLastSelectedInterval(), subDocument);

            this.control.history.addAndRedo(new FieldInsertHistoryItem(this.control, this.control.modelManipulator, subDocument, interval.start, interval.length, 0, true));
            Field.DEBUG_FIELDS_CHECKS(subDocument);
            return true;
        }
    }


    export class CreatePredefinedFieldCommand extends CreateFieldCommandBase {
        executeCore(state: IntervalCommandState, parameter: any): boolean {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var selection: Selection = this.control.selection;
            var interval: FixedInterval = CreateFieldCommandBase.getIntervalWithoutLastParagraphMark(selection.getLastSelectedInterval(), subDocument);
            var history: IHistory = this.control.history;

            history.beginTransaction();
            history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, subDocument, [new FixedInterval(interval.start, 0)], selection, UpdateInputPositionProperties.No, false));
            if (interval.length > 0)
                history.addAndRedo(new RemoveIntervalHistoryItem(this.control.modelManipulator, subDocument, interval, false));
            history.addAndRedo(new FieldInsertHistoryItem(this.control, this.control.modelManipulator, subDocument, interval.start, 0, 0, false));
            var run: TextRun = subDocument.getRunByPosition(interval.start);
            var insertedText: string = this.getInsertedText(parameter);
            history.addAndRedo(new InsertTextHistoryItem(this.control.modelManipulator, subDocument, interval.start + 1, insertedText, run.maskedCharacterProperties, run.characterStyle));
            var fieldInterval: FixedInterval = FixedInterval.fromPositions(interval.start, interval.start + insertedText.length + 3);
            history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, subDocument, [fieldInterval], selection, UpdateInputPositionProperties.No, false));

            (new FieldsWaitingForUpdate(this.control, () => {
                var fieldEndPos: number = subDocument.fields[Field.normedBinaryIndexOf(subDocument.fields, interval.start + 1)].getFieldEndPosition();
                history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, subDocument, [new FixedInterval(fieldEndPos, 0)], selection, UpdateInputPositionProperties.No, false));
                history.endTransaction();
            })).update(null);
            //history.endTransaction();

            return true;
        }

        public getInsertedText(parameter: any): string {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class CreatePageFieldCommand extends CreatePredefinedFieldCommand {
        public getInsertedText(parameter: any): string {
            return "PAGE";
        }
    }

    export class CreatePageCountFieldCommand extends CreatePredefinedFieldCommand {
        public getInsertedText(parameter: any): string {
            return "NUMPAGES";
        }
    }

    export class CreateDateFieldCommand extends CreatePredefinedFieldCommand {
        public getInsertedText(parameter: any): string {
            return "DATE \\@ \"M/d/yyyy\"";
        }
    }

    export class CreateTimeFieldCommand extends CreatePredefinedFieldCommand {
        public getInsertedText(parameter: any): string {
            return "TIME \\@ \"h:mm am/pm\"";
        }
    }

    export class CreateMergeFieldCommand extends CreatePredefinedFieldCommand {
        public getInsertedText(parameter: string): string {
            return "MERGEFIELD " + parameter;
        }
    }
}