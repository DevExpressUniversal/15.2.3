module __aspxRichEdit {
    export class ChangeParagraphBackColorCommand extends ChangeParagraphPropertiesCommandBase<string> {
        getActualValue(parameter: any, currentValue: string): string {
            return parameter;
        }
        getHistoryItem(interval: FixedInterval, newValue: string): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new ParagraphBackColorHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getColor(newValue), true);
        }
        getColor(value: string): number {
            return value == ColorHelper.NO_COLOR.toString() ? ColorHelper.NO_COLOR : ColorHelper.hashToColor(value);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: string) {
            properties.backColor = this.getColor(value);
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): string {
            return null;
        }
        isLockUpdateVaue(): boolean {
            return true;
        }
    }
}  