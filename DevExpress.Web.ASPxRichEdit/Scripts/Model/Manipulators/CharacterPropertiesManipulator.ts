module __aspxRichEdit {
    export class CharacterPropertiesManipulator {
        fontBold: IIntervalPropertyWithUseValueManipulator<boolean>;
        fontItalic: IIntervalPropertyWithUseValueManipulator<boolean>;
        fontName: IIntervalPropertyWithUseValueManipulator<FontInfo>;
        fontSize: IIntervalPropertyWithUseValueManipulator<number>;
        fontCaps: IIntervalPropertyWithUseValueManipulator<boolean>;
        foreColor: IIntervalPropertyWithUseValueManipulator<number>;
        backColor: IIntervalPropertyWithUseValueManipulator<number>;
        fontStrikeoutType: IIntervalPropertyWithUseValueManipulator<StrikeoutType>;
        fontStrikeoutWordsOnly: IIntervalPropertyWithUseValueManipulator<boolean>;
        fontStrikeoutColor: IIntervalPropertyWithUseValueManipulator<number>;
        fontUnderlineColor: IIntervalPropertyWithUseValueManipulator<number>;
        fontUnderlineType: IIntervalPropertyWithUseValueManipulator<UnderlineType>;
        fontHidden: IIntervalPropertyWithUseValueManipulator<boolean>;
        script: IIntervalPropertyWithUseValueManipulator<CharacterFormattingScript>;
        fontUnderlineWordsOnly: IIntervalPropertyWithUseValueManipulator<boolean>;
        fontNoProof: IIntervalPropertyWithUseValueManipulator<boolean>;
        useValue: IIntervalPropertyManipulator<number>;

        constructor(manipulator: ModelManipulator) {
            this.fontBold = new CharacterPropertiesFontBoldManipulator(manipulator);
            this.fontItalic = new CharacterPropertiesFontItalicManipulator(manipulator);
            this.fontName = new CharacterPropertiesFontNameManipulator(manipulator);
            this.fontSize = new CharacterPropertiesFontSizeManipulator(manipulator);
            this.fontCaps = new CharacterPropertiesFontCapsManipulator(manipulator);
            this.foreColor = new CharacterPropertiesForeColorManipulator(manipulator);
            this.backColor = new CharacterPropertiesBackColorManipulator(manipulator);
            this.fontStrikeoutType = new CharacterPropertiesFontStrikeoutTypeManipulator(manipulator);
            this.fontStrikeoutWordsOnly = new CharacterPropertiesFontStrikeoutWordsOnlyManipulator(manipulator);
            this.fontStrikeoutColor = new CharacterPropertiesFontStrikeoutColorManipulator(manipulator);
            this.fontUnderlineColor = new CharacterPropertiesFontUnderlineColorManipulator(manipulator);
            this.fontUnderlineType = new CharacterPropertiesFontUnderlineTypeManipulator(manipulator);
            this.fontHidden = new CharacterPropertiesFontHiddenManipulator(manipulator);
            this.script = new CharacterPropertiesScriptManipulator(manipulator);
            this.fontUnderlineWordsOnly = new CharacterPropertiesFontUnderlineWordsOnlyManipulator(manipulator);
            this.fontNoProof = new CharacterPropertiesFontNoProof(manipulator);
            this.useValue = new CharacterPropertiesUseValueManipulator(manipulator);
        }
    }

    class CharacterPropertiesUseValueManipulator implements IIntervalPropertyManipulator<number> {
        manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }
        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: number): HistoryItemIntervalState<HistoryItemIntervalStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.characterFormatting))
                return oldState;
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var iterator = subDocument.getRunIterator(interval);
            while(iterator.moveNext()) {
                var run = iterator.currentRun;
                oldState.register(new HistoryItemIntervalStateObject(iterator.currentInterval(), run.maskedCharacterProperties.useValue));
                var properties = run.maskedCharacterProperties.clone();
                properties.useValue = newValue;
                run.setCharacterProperties(properties);
                run.onCharacterPropertiesChanged();
            }
            newState.register(new HistoryItemIntervalStateObject(interval, newValue));
            this.manipulator.dispatcher.notifyCharacterPropertyChanged(newState.interval(), JSONCharacterFormattingProperty.UseValue, newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.characterFormatting))
                return;
            for(var stateValue: HistoryItemIntervalStateObject, i = 0; stateValue = state.objects[i]; i++) {
                var iterator = subDocument.getRunIterator(stateValue.interval);
                while(iterator.moveNext()) {
                    var run = iterator.currentRun;
                    var properties = run.maskedCharacterProperties.clone();
                    properties.useValue = <number>stateValue.value;
                    run.setCharacterProperties(properties);
                    run.onCharacterPropertiesChanged();
                }
            }
            this.manipulator.dispatcher.notifyCharacterPropertyChanged(state.interval(), JSONCharacterFormattingProperty.UseValue, state, this.manipulator.model.activeSubDocument);
        }
    }

    class MaskedCharacterPropertiesManipulator<T> implements IIntervalPropertyWithUseValueManipulator<T> {
        manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: T, newUse: boolean): HistoryItemIntervalState<HistoryItemIntervalUseStateObject> {
            var oldState: HistoryItemIntervalState<HistoryItemIntervalUseStateObject> = new HistoryItemIntervalState<HistoryItemIntervalUseStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.characterFormatting))
                return oldState;
            var newState: HistoryItemIntervalState<HistoryItemIntervalUseStateObject> = new HistoryItemIntervalState<HistoryItemIntervalUseStateObject>();

            var mask = this.getPropertyMask();
            var iterator: RunIterator = subDocument.getRunIterator(interval);
            while(iterator.moveNext()) {
                var currentInterval = iterator.currentInterval();
                var properties = iterator.currentRun.maskedCharacterProperties.clone();

                newState.register(new HistoryItemIntervalUseStateObject(currentInterval, newValue, newUse));
                oldState.register(new HistoryItemIntervalUseStateObject(currentInterval, this.getPropertyValue(properties), properties.getUseValue(mask)));

                this.setPropertyValue(properties, newValue);
                properties.setUseValue(mask, newUse);
                iterator.currentRun.setCharacterProperties(properties);
                if (iterator.currentRun.hasCharacterMergedProperies() && newUse) {
                    var mergedProperties = iterator.currentRun.getCharacterMergedProperies().clone();
                    this.setPropertyValue(mergedProperties, newValue);
                    iterator.currentRun.setCharacterMergedProperies(mergedProperties);
                }
                else
                    iterator.currentRun.onCharacterPropertiesChanged();
            }
            this.manipulator.dispatcher.notifyCharacterPropertyChanged(interval, this.getJSONCharacterFormattingProperty(), newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }

        restoreValue(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalUseStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.characterFormatting))
                return;
            if(state.isEmpty()) return;
            for(var i = 0, stateItem: HistoryItemIntervalUseStateObject; stateItem = state.objects[i]; i++) {
                var iterator = subDocument.getRunIterator(stateItem.interval);
                while (iterator.moveNext()) {
                    var properties = iterator.currentRun.maskedCharacterProperties.clone();
                    this.setPropertyValue(properties, stateItem.value);
                    properties.setUseValue(this.getPropertyMask(), stateItem.use);
                    iterator.currentRun.setCharacterProperties(properties);
                    // have run maskedProps.color=black, style.color=blue, merge props=blue, then change color to blue, then maskedProps.color=blue and save to history masked!color=black OK!
                    // when press redo see block "if" below - there black applies to merged properties, what incorrect. For best perfomance need later save in history merged props too, and apply it

                    //if (iterator.currentRun.hasCharacterMergedProperies()) {
                    //    var mergedProperties = iterator.currentRun.getCharacterMergedProperies().clone();
                    //    this.setPropertyValue(mergedProperties, stateItem.value);
                    //    iterator.currentRun.setCharacterMergedProperies(mergedProperties);
                    //}
                    //else
                    //    iterator.currentRun.onCharacterPropertiesChanged();

                    iterator.currentRun.onCharacterPropertiesChanged();
                }
            }
            this.manipulator.dispatcher.notifyCharacterPropertyChanged(state.interval(), this.getJSONCharacterFormattingProperty(), state, this.manipulator.model.activeSubDocument);
        }

        getPropertyMask(): CharacterPropertiesMask {
            throw new Error(Errors.NotImplemented);
        }

        getPropertyValue(properties: CharacterProperties): T {
            throw new Error(Errors.NotImplemented);
        }

        setPropertyValue(properties: CharacterProperties, value: T) {
            throw new Error(Errors.NotImplemented);
        }

        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            throw new Error(Errors.NotImplemented);
        }
    }

    class CharacterPropertiesFontItalicManipulator extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontItalic;
        }
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.fontItalic;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.fontItalic = value;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.FontItalic;
        }
    }

    class CharacterPropertiesFontBoldManipulator extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.fontBold;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.fontBold = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontBold;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.FontBold;
        }
    }
    
    class CharacterPropertiesFontNameManipulator extends MaskedCharacterPropertiesManipulator<FontInfo> {
        getPropertyValue(properties: CharacterProperties): FontInfo {
            return properties.fontInfo;
        }
        setPropertyValue(properties: CharacterProperties, value: FontInfo) {
            properties.fontInfo = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontName;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.FontInfoIndex;
        }
    }

    class CharacterPropertiesFontSizeManipulator extends MaskedCharacterPropertiesManipulator<number> {
        getPropertyValue(properties: CharacterProperties): number {
            return properties.fontSize;
        }
        setPropertyValue(properties: CharacterProperties, value: number) {
            properties.fontSize = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseDoubleFontSize;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.FontSize;
        }
    }

    class CharacterPropertiesFontCapsManipulator extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.allCaps;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.allCaps = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseAllCaps;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.AllCaps;
        }
    }

    class CharacterPropertiesForeColorManipulator extends MaskedCharacterPropertiesManipulator<number> {
        getPropertyValue(properties: CharacterProperties): number {
            return properties.foreColor;
        }
        setPropertyValue(properties: CharacterProperties, value: number) {
            properties.foreColor = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseForeColor;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.ForeColor;
        }
    }

    class CharacterPropertiesBackColorManipulator extends MaskedCharacterPropertiesManipulator<number> {
        getPropertyValue(properties: CharacterProperties): number {
            return properties.backColor;
        }
        setPropertyValue(properties: CharacterProperties, value: number) {
            properties.backColor = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseBackColor;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.BackColor;
        }
    }

    class CharacterPropertiesFontStrikeoutTypeManipulator extends MaskedCharacterPropertiesManipulator<StrikeoutType> {
        getPropertyValue(properties: CharacterProperties): StrikeoutType {
            return properties.fontStrikeoutType;
        }
        setPropertyValue(properties: CharacterProperties, value: StrikeoutType) {
            properties.fontStrikeoutType = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontStrikeoutType;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.FontStrikeoutType;
        }
    }

    class CharacterPropertiesFontStrikeoutWordsOnlyManipulator extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.strikeoutWordsOnly;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.strikeoutWordsOnly = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseStrikeoutWordsOnly;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.StrikeoutWordsOnly;
        }
    }

    class CharacterPropertiesFontUnderlineColorManipulator extends MaskedCharacterPropertiesManipulator<number> {
        getPropertyValue(properties: CharacterProperties): number {
            return properties.underlineColor;
        }
        setPropertyValue(properties: CharacterProperties, value: number) {
            properties.underlineColor = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseUnderlineColor;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.UnderlineColor;
        }
    }

    class CharacterPropertiesFontUnderlineTypeManipulator extends MaskedCharacterPropertiesManipulator<UnderlineType> {
        getPropertyValue(properties: CharacterProperties): UnderlineType {
            return properties.fontUnderlineType;
        }
        setPropertyValue(properties: CharacterProperties, value: UnderlineType) {
            properties.fontUnderlineType = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseFontUnderlineType;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.FontUnderlineType;
        }
    }

    class CharacterPropertiesFontHiddenManipulator extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.hidden;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.hidden = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseHidden;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.Hidden;
        }
    }

    class CharacterPropertiesScriptManipulator extends MaskedCharacterPropertiesManipulator<CharacterFormattingScript> {
        getPropertyValue(properties: CharacterProperties): CharacterFormattingScript {
            return properties.script;
        }
        setPropertyValue(properties: CharacterProperties, value: CharacterFormattingScript) {
            properties.script = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseScript;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.Script;
        }
    }

    class CharacterPropertiesFontUnderlineWordsOnlyManipulator extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.underlineWordsOnly;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.underlineWordsOnly = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseUnderlineWordsOnly;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.UnderlineWordsOnly;
        }
    }

    class CharacterPropertiesFontNoProof extends MaskedCharacterPropertiesManipulator<boolean> {
        getPropertyValue(properties: CharacterProperties): boolean {
            return properties.noProof;
        }
        setPropertyValue(properties: CharacterProperties, value: boolean) {
            properties.noProof = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseNoProof;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.NoProof;
        }
    }

    class CharacterPropertiesFontStrikeoutColorManipulator extends MaskedCharacterPropertiesManipulator<number> {
        getPropertyValue(properties: CharacterProperties): number {
            return properties.strikeoutColor;
        }
        setPropertyValue(properties: CharacterProperties, value: number) {
            properties.strikeoutColor = value;
        }
        getPropertyMask(): CharacterPropertiesMask {
            return CharacterPropertiesMask.UseStrikeoutColor;
        }
        getJSONCharacterFormattingProperty(): JSONCharacterFormattingProperty {
            return JSONCharacterFormattingProperty.StrikeoutColor;
        }
    }
}