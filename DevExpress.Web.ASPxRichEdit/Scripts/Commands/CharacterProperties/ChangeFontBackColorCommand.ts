module __aspxRichEdit {
    export class ChangeFontBackColorCommand extends ChangeCharacterPropertiesCommandBase<string> {
        getActualValue(parameter: any, currentValue: string): string {
            return parameter;
        }
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseBackColor;
        }
        getHistoryItem(interval: FixedInterval, newValue: string): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontBackColorHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getColor(newValue), true);
        }
        getColor(value: string): number {
            return value == ColorHelper.NO_COLOR.toString() ? ColorHelper.NO_COLOR : ColorHelper.hashToColor(value);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: string) {
            properties.backColor = this.getColor(value);
        }
        getCharacterPropertiesValue(properties: CharacterProperties): string {
            return null;
        }
        isLockUpdateVaue(): boolean {
            return true;
        }
    }
}  