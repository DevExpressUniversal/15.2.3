module __aspxRichEdit {
    export class SetSesquialteralParagraphSpacingCommand extends ChangeParagraphPropertiesCommandBase<ParagraphLineSpacingType> {
        executeCore(state: IntervalCommandStateEx): boolean {
            this.setNewValue(state.intervals, ParagraphLineSpacingType.Sesquialteral);
            return true;
        }
        getActualValue(parameter: any, currentValue: ParagraphLineSpacingType): ParagraphLineSpacingType {
            return ParagraphLineSpacingType.Sesquialteral;
        }
        getHistoryItem(interval: FixedInterval, newValue: ParagraphLineSpacingType): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new ParagraphLineSpacingTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: ParagraphLineSpacingType) {
            properties.lineSpacingType = value;
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): ParagraphLineSpacingType {
            return properties.lineSpacingType === ParagraphLineSpacingType.Sesquialteral ? 1 : 0;
        }
    }
}  