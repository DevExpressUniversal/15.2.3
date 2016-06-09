module __aspxRichEdit {
    export class ChangeFontNameCommand extends ChangeCharacterPropertiesCommandBase<FontInfo> {
        getActualValue(parameter: any, currentValue: FontInfo): FontInfo {
            var fontInfo = this.control.model.cache.fontInfoCache.getItem(<number>parameter);
            if(!fontInfo)
                throw "Unknown fontInfo key";
            return fontInfo;
        }
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontName;
        }
        getHistoryItem(interval: FixedInterval, newValue: FontInfo): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new FontNameHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue, true);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: FontInfo) {
            properties.fontInfo = value;
        }
        getCharacterPropertiesValue(properties: CharacterProperties): FontInfo {
            return properties.fontInfo;
        }
    }
}