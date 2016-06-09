module __aspxRichEdit {
    export class RemoveSpacingAfterParagraphCommand extends ChangeParagraphPropertiesCommandBase<number> {
        getActualValue(parameter: any, currentValue: number): number {
            return 0;
        }
        isEnabled(): boolean {
            return super.isEnabled() && this.control.inputPosition.getMergedParagraphPropertiesFull().spacingAfter > 0;
        }
        getHistoryItem(interval: FixedInterval, newValue: number): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new ParagraphSpacingAfterHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: number) {
            properties.spacingAfter = value;
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): number {
            return properties.spacingAfter;
        }
    }
} 