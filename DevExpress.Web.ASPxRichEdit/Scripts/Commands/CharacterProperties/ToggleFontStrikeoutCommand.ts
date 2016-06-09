module __aspxRichEdit {
    export class ToggleFontStrikeoutCommand extends ChangeCharacterPropertiesCommandBase<StrikeoutType> {
        getActualValue(parameter: any, currentValue: StrikeoutType): StrikeoutType {
            return currentValue ? StrikeoutType.None : StrikeoutType.Single;
        }
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontStrikeoutType;
        }
        getHistoryItem(interval: FixedInterval, newValue: StrikeoutType): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontStrikeoutTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: StrikeoutType) {
            properties.fontStrikeoutType = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): StrikeoutType {
            return properties.fontStrikeoutType;
        }
    }
}   