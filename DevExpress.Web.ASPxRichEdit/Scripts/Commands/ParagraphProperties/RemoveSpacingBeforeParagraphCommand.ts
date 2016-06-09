module __aspxRichEdit {
    export class RemoveSpacingBeforeParagraphCommand extends ChangeParagraphPropertiesCommandBase<number> {
        getActualValue(parameter: any, currentValue: number): number {
            return 0;
        }
        isEnabled(): boolean {
            return super.isEnabled() && this.control.inputPosition.getMergedParagraphPropertiesFull().spacingBefore > 0;
        }
        getHistoryItem(interval: FixedInterval, newValue: number): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new ParagraphSpacingBeforeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: number) {
            properties.spacingBefore = value;
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): number {
            return properties.spacingBefore;
        }
    }
}  