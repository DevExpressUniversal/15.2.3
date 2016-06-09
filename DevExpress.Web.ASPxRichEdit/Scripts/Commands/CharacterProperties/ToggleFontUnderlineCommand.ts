module __aspxRichEdit {
    export class ToggleFontUnderlineCommand extends ChangeCharacterPropertiesCommandBase<UnderlineType> { // abstract
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontUnderlineType;
        }
        getHistoryItem(interval: FixedInterval, newValue: UnderlineType): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator; 
            return new FontUnderlineTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: UnderlineType) {
            properties.fontUnderlineType = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): UnderlineType {
            return properties.fontUnderlineType;
        }
    }

    export class ToggleFontSingleUnderlineCommand extends ToggleFontUnderlineCommand {
        getActualValue(parameter: any, currentValue: UnderlineType): UnderlineType {
            return currentValue ? UnderlineType.None : UnderlineType.Single;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): UnderlineType {
            return properties.fontUnderlineType == UnderlineType.Single ? UnderlineType.Single : 0;
        }
    }

    export class ToggleFontDoubleUnderlineCommand extends ToggleFontUnderlineCommand {
        getActualValue(parameter: any, currentValue: UnderlineType): UnderlineType {
            return currentValue ? UnderlineType.None : UnderlineType.Double;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): UnderlineType {
            return properties.fontUnderlineType == UnderlineType.Double ? UnderlineType.Double : 0;
        }
    }
}   