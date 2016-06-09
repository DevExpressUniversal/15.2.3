module __aspxRichEdit {
    export class ToggleParagraphAlignmentCommand extends ChangeParagraphPropertiesCommandBase<ParagraphAlignment> { // abstract
        executeCore(state: IntervalCommandStateEx): boolean {
            this.setNewValue(state.intervals, !state.value ? this.getParagraphAlignment() : ParagraphAlignment.Left);
            return true;
        }
        getActualValue(parameter: any, currentValue: ParagraphAlignment): ParagraphAlignment {
            return !currentValue ? this.getParagraphAlignment() : ParagraphAlignment.Left;
        }
        getHistoryItem(interval: FixedInterval, newValue: number): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new ParagraphAlignmentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue ? this.getParagraphAlignment() : ParagraphAlignment.Left, true);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: number) {
            properties.alignment = value;
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): number {
            return properties.alignment === this.getParagraphAlignment() ? 1 : 0;
        }
        getParagraphAlignment(): ParagraphAlignment {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class ToggleParagraphAlignmentLeftCommand extends ToggleParagraphAlignmentCommand {
        getParagraphAlignment(): ParagraphAlignment {
            return ParagraphAlignment.Left;
        }
    }

    export class ToggleParagraphAlignmentCenterCommand extends ToggleParagraphAlignmentCommand {
        getParagraphAlignment(): ParagraphAlignment {
            return ParagraphAlignment.Center;
        }
    }

    export class ToggleParagraphAlignmentRightCommand extends ToggleParagraphAlignmentCommand {
        getParagraphAlignment(): ParagraphAlignment {
            return ParagraphAlignment.Right;
        }
    }

    export class ToggleParagraphAlignmentJustifyCommand extends ToggleParagraphAlignmentCommand {
        getParagraphAlignment(): ParagraphAlignment {
            return ParagraphAlignment.Justify;
        }
    }
}   