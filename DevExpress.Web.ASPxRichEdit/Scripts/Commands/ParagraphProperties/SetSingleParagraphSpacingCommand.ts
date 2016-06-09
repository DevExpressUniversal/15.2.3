module __aspxRichEdit {
    export class SetSingleParagraphSpacingCommand extends ChangeParagraphPropertiesCommandBase<ParagraphLineSpacingType> {
        executeCore(state: IntervalCommandStateEx): boolean {
            this.setNewValue(state.intervals, ParagraphLineSpacingType.Single);
            return true;
        }
        getActualValue(parameter: any, currentValue: ParagraphLineSpacingType): ParagraphLineSpacingType {
            return ParagraphLineSpacingType.Single;
        }
        getHistoryItem(interval: FixedInterval, newValue: ParagraphLineSpacingType): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new ParagraphLineSpacingTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: ParagraphLineSpacingType) {
            properties.lineSpacingType = value;
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): ParagraphLineSpacingType {
            return properties.lineSpacingType === ParagraphLineSpacingType.Single ? 1 : 0;
        }
    }
}  