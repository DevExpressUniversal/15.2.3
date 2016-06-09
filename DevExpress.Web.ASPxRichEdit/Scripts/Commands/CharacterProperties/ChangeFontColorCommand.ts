module __aspxRichEdit {
    export class ChangeFontColorCommand extends ChangeCharacterPropertiesCommandBase<string> {
        getActualValue(parameter: any, currentValue: string): string {
            return parameter;
        }
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseForeColor;
        }
        getHistoryItem(interval: FixedInterval, newValue: string): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontForeColorHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getColor(newValue), true);
        }
        getColor(value: string): number {
            return value == ColorHelper.AUTOMATIC_COLOR.toString() ? ColorHelper.AUTOMATIC_COLOR : ColorHelper.hashToColor(value);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: string) {
            properties.foreColor = this.getColor(value);
        }
        getCharacterPropertiesValue(properties: CharacterProperties): string {
            return null;
        }
        isLockUpdateVaue(): boolean {
            return true;
        }
    }
} 