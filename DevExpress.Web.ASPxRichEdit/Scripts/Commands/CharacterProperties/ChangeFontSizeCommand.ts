module __aspxRichEdit {
    export class ChangeFontSizeCommand extends ChangeCharacterPropertiesCommandBase<number> {
        getActualValue(parameter: any, currentValue: number): number {
            return parseFloat(parameter);
        }
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseDoubleFontSize;
        }
        getHistoryItem(interval: FixedInterval, newValue: number): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontSizeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: number) {
            properties.fontSize = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): number {
            return properties.fontSize;
        }
    }
}