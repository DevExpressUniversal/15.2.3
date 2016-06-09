module __aspxRichEdit {
    export class ToggleFontBoldCommand extends ToggleCharacterPropertiesCommandBase {
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontBold;
        }
        getHistoryItem(interval: FixedInterval, newValue: boolean): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontBoldHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: boolean) {
            properties.fontBold = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): boolean {
            return properties.fontBold;
        }
    }
} 