module __aspxRichEdit {
    export class ListLevelCharacterPropertiesManipulator {
        fontBold: IListLevelPropertyWithUseManipulator<boolean>;
        fontItalic: IListLevelPropertyWithUseManipulator<boolean>;
        fontName: IListLevelPropertyWithUseManipulator<FontInfo>;
        fontSize: IListLevelPropertyWithUseManipulator<number>;
        fontCaps: IListLevelPropertyWithUseManipulator<boolean>;
        foreColor: IListLevelPropertyWithUseManipulator<number>;
        backColor: IListLevelPropertyWithUseManipulator<number>;
        fontStrikeoutType: IListLevelPropertyWithUseManipulator<StrikeoutType>;
        fontStrikeoutWordsOnly: IListLevelPropertyWithUseManipulator<boolean>;
        fontStrikeoutColor: IListLevelPropertyWithUseManipulator<number>;
        fontUnderlineColor: IListLevelPropertyWithUseManipulator<number>;
        fontUnderlineType: IListLevelPropertyWithUseManipulator<UnderlineType>;
        fontHidden: IListLevelPropertyWithUseManipulator<boolean>;
        script: IListLevelPropertyWithUseManipulator<CharacterFormattingScript>;
        fontUnderlineWordsOnly: IListLevelPropertyWithUseManipulator<boolean>;
        fontNoProof: IListLevelPropertyWithUseManipulator<boolean>;

        constructor(manipulator: ModelManipulator) {
            this.fontBold = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.FontBold,
                CharacterPropertiesMask.UseFontBold,
                (properties: MaskedCharacterProperties, value: boolean) => properties.fontBold = value,
                properties => properties.fontBold);
            this.fontItalic = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.FontItalic,
                CharacterPropertiesMask.UseFontItalic,
                (properties: MaskedCharacterProperties, value: boolean) => properties.fontItalic = value,
                properties => properties.fontItalic);
            this.fontName = new CharacterPropertiesManipulator<FontInfo>(manipulator,
                JSONCharacterFormattingProperty.FontInfoIndex,
                CharacterPropertiesMask.UseFontName,
                (properties: MaskedCharacterProperties, value: FontInfo) => properties.fontInfo = value,
                properties => properties.fontInfo);
            this.fontSize = new CharacterPropertiesManipulator<number>(manipulator,
                JSONCharacterFormattingProperty.FontSize,
                CharacterPropertiesMask.UseDoubleFontSize,
                (properties: MaskedCharacterProperties, value: number) => properties.fontSize = value,
                properties => properties.fontSize);
            this.fontCaps = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.AllCaps,
                CharacterPropertiesMask.UseAllCaps,
                (properties: MaskedCharacterProperties, value: boolean) => properties.allCaps = value,
                properties => properties.allCaps);
            this.foreColor = new CharacterPropertiesManipulator<number>(manipulator,
                JSONCharacterFormattingProperty.ForeColor,
                CharacterPropertiesMask.UseForeColor,
                (properties: MaskedCharacterProperties, value: number) => properties.foreColor = value,
                properties => properties.foreColor);
            this.backColor = new CharacterPropertiesManipulator<number>(manipulator,
                JSONCharacterFormattingProperty.BackColor,
                CharacterPropertiesMask.UseBackColor,
                (properties: MaskedCharacterProperties, value: number) => properties.backColor = value,
                properties => properties.backColor);
            this.fontStrikeoutType = new CharacterPropertiesManipulator<StrikeoutType>(manipulator,
                JSONCharacterFormattingProperty.FontStrikeoutType,
                CharacterPropertiesMask.UseFontStrikeoutType,
                (properties: MaskedCharacterProperties, value: StrikeoutType) => properties.fontStrikeoutType = value,
                properties => properties.fontStrikeoutType);
            this.fontStrikeoutWordsOnly = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.StrikeoutWordsOnly,
                CharacterPropertiesMask.UseStrikeoutWordsOnly,
                (properties: MaskedCharacterProperties, value: boolean) => properties.strikeoutWordsOnly = value,
                properties => properties.strikeoutWordsOnly);
            this.fontStrikeoutColor = new CharacterPropertiesManipulator<number>(manipulator,
                JSONCharacterFormattingProperty.StrikeoutColor,
                CharacterPropertiesMask.UseStrikeoutColor,
                (properties: MaskedCharacterProperties, value: number) => properties.strikeoutColor = value,
                properties => properties.strikeoutColor);
            this.fontUnderlineColor = new CharacterPropertiesManipulator<number>(manipulator,
                JSONCharacterFormattingProperty.UnderlineColor,
                CharacterPropertiesMask.UseUnderlineColor,
                (properties: MaskedCharacterProperties, value: number) => properties.underlineColor = value,
                properties => properties.underlineColor);
            this.fontUnderlineType = new CharacterPropertiesManipulator<UnderlineType>(manipulator,
                JSONCharacterFormattingProperty.FontUnderlineType,
                CharacterPropertiesMask.UseFontUnderlineType,
                (properties: MaskedCharacterProperties, value: UnderlineType) => properties.fontUnderlineType = value,
                properties => properties.fontUnderlineType);
            this.fontHidden = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.Hidden,
                CharacterPropertiesMask.UseHidden,
                (properties: MaskedCharacterProperties, value: boolean) => properties.hidden = value,
                properties => properties.hidden);
            this.script = new CharacterPropertiesManipulator<CharacterFormattingScript>(manipulator,
                JSONCharacterFormattingProperty.Script,
                CharacterPropertiesMask.UseScript,
                (properties: MaskedCharacterProperties, value: CharacterFormattingScript) => properties.script = value,
                properties => properties.script);
            this.fontUnderlineWordsOnly = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.UnderlineWordsOnly,
                CharacterPropertiesMask.UseUnderlineWordsOnly,
                (properties: MaskedCharacterProperties, value: boolean) => properties.underlineWordsOnly = value,
                properties => properties.underlineWordsOnly);
            this.fontNoProof = new CharacterPropertiesManipulator<boolean>(manipulator,
                JSONCharacterFormattingProperty.NoProof,
                CharacterPropertiesMask.UseNoProof,
                (properties: MaskedCharacterProperties, value: boolean) => properties.noProof = value,
                properties => properties.noProof);
        }
    }

    class CharacterPropertiesManipulator<T> implements IListLevelPropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonCharacterFormattingProperty: JSONCharacterFormattingProperty;
        characterPropertiesMask: CharacterPropertiesMask;
        setProperty: (properties: MaskedCharacterProperties, value: T) => void;
        getProperty: (properties: MaskedCharacterProperties) => T;

        constructor(manipulator: ModelManipulator, jsonCharacterFormattingProperty: JSONCharacterFormattingProperty, characterPropertiesMask: CharacterPropertiesMask, setProperty: (properties: MaskedCharacterProperties, value: T) => void, getProperty: (properties: MaskedCharacterProperties) => T) {
            this.manipulator = manipulator;
            this.characterPropertiesMask = characterPropertiesMask;
            this.jsonCharacterFormattingProperty = jsonCharacterFormattingProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }

        setValue(model: DocumentModel, isAbstractList: boolean, listIndex: number, listLevelIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemListLevelUseStateObject> {
            var newState = new HistoryItemState<HistoryItemListLevelUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemListLevelUseStateObject>();

            var numberingList: NumberingListBase<IListLevel> = isAbstractList ? model.abstractNumberingLists[listIndex] : model.numberingLists[listIndex];
            var listLevel: IListLevel = numberingList.levels[listLevelIndex];

            var properties = listLevel.getCharacterProperties();

            if(listLevel instanceof NumberingListReferenceLevel) {
                var abstractNumberingListIndex = (<NumberingList>numberingList).abstractNumberingListIndex;
                oldState.register(new HistoryItemListLevelUseStateObject(true, abstractNumberingListIndex, listLevelIndex, this.getProperty(properties), properties.getUseValue(this.characterPropertiesMask)));
                this.setValueCore(listLevel, newValue, newUse);
                newState.register(new HistoryItemListLevelUseStateObject(true, abstractNumberingListIndex, listLevelIndex, newValue, newUse));
            }
            else {
                oldState.register(new HistoryItemListLevelUseStateObject(isAbstractList, listIndex, listLevelIndex, this.getProperty(properties), properties.getUseValue(this.characterPropertiesMask)));
                this.setValueCore(listLevel, newValue, newUse);
                newState.register(new HistoryItemListLevelUseStateObject(isAbstractList, listIndex, listLevelIndex, newValue, newUse));
            }
            this.manipulator.model.resetMergedFormattingCache(ResetFormattingCacheType.Character);
            this.manipulator.dispatcher.notifyListLevelCharacterPropertyChanged(this.jsonCharacterFormattingProperty, newState);
            return oldState;
        }
        restoreValue(model: DocumentModel, state: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            var stateObject = state.objects[0];
            var numberingList: NumberingListBase<IListLevel> = stateObject.isAbstractNumberingList ? model.abstractNumberingLists[stateObject.numberingListIndex] : model.numberingLists[stateObject.numberingListIndex];
            var listLevel: IListLevel = numberingList.levels[stateObject.listLevelIndex];
            this.setValueCore(listLevel, stateObject.value, stateObject.use);
            this.manipulator.model.resetMergedFormattingCache(ResetFormattingCacheType.Character);
            this.manipulator.dispatcher.notifyListLevelCharacterPropertyChanged(this.jsonCharacterFormattingProperty, state);
        }

        private setValueCore(level: IListLevel, newValue: T, newUse: boolean) {
            var properties = level.getCharacterProperties().clone();
            this.setProperty(properties, newValue);
            properties.setUseValue(this.characterPropertiesMask, newUse);
            level.setCharacterProperties(properties);
            level.onCharacterPropertiesChanged();
        }
    }
} 