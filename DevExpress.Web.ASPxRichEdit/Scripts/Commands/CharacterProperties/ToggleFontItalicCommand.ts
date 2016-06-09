module __aspxRichEdit {
    export class ToggleFontItalicCommand extends ToggleCharacterPropertiesCommandBase {
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontItalic;
        }
        getHistoryItem(interval: FixedInterval, newValue: boolean): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontItalicHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: boolean) {
            properties.fontItalic = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): boolean {
            return properties.fontItalic;
        }
    }
}  