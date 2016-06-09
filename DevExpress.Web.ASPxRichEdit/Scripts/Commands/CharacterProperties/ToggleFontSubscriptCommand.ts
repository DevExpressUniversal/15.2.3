module __aspxRichEdit {
    export class ToggleFontSubscriptCommand extends ChangeCharacterPropertiesCommandBase<CharacterFormattingScript> {
        getActualValue(parameter: any, currentValue: CharacterFormattingScript): CharacterFormattingScript {
            return currentValue ? CharacterFormattingScript.Normal : CharacterFormattingScript.Subscript;
        }
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseScript;
        }
        getHistoryItem(interval: FixedInterval, newValue: CharacterFormattingScript): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontScriptHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: CharacterFormattingScript) {
            properties.script = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): CharacterFormattingScript {
            return properties.script == CharacterFormattingScript.Subscript ? CharacterFormattingScript.Subscript : CharacterFormattingScript.Normal;
        }
    }
}   