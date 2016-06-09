module __aspxRichEdit {
    export class DialogFontCommandBase extends ShowDialogCommandBase {
        getDialogName() {
            return "EditFont";
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.characterFormatting);
        }
    }

    export class DialogFontCommand extends DialogFontCommandBase {
        getState(): ICommandState {
            return new IntervalCommandStateEx(this.isEnabled(), this.getActualIntervals());
        }
        getActualIntervals(): FixedInterval[] {
            if (this.control.selection.isCollapsed())
                return [this.control.model.activeSubDocument.getWholeWordInterval(this.control.selection.intervals[0].start)];
            return this.control.selection.getIntervalsClone();
        }
        createParameters(): FontDialogParameters {
            var parameters: FontDialogParameters = new FontDialogParameters();
            parameters.init(this.control.inputPosition.getMergedCharacterPropertiesRaw());
            return parameters;
        }
        applyParameters(state: IntervalCommandStateEx, newParams: FontDialogParameters) {
            var initParams: FontDialogParameters = <FontDialogParameters>this.initParams;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var history: IHistory = this.control.history;

            var maskedCharProps: MaskedCharacterProperties = this.control.inputPosition.getMaskedCharacterProperties();
            var charPropsRaw: CharacterProperties = this.control.inputPosition.getMergedCharacterPropertiesRaw();
            var charPropsFull: CharacterProperties = this.control.inputPosition.getMergedCharacterPropertiesFull();
            
            history.beginTransaction();
            if (newParams.allCaps !== initParams.allCaps) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseAllCaps, (prop: CharacterProperties) => { prop.allCaps = newParams.allCaps; });
                this.addHistoryItem(state.intervals, interval => new FontCapsHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.allCaps, true));
            }
            if (newParams.fontColor !== initParams.fontColor) {
                var color = newParams.fontColor == "Auto" ? ColorHelper.AUTOMATIC_COLOR : ColorHelper.hashToColor(newParams.fontColor);
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseForeColor, (prop: CharacterProperties) => { prop.foreColor = color; });
                this.addHistoryItem(state.intervals, interval => new FontForeColorHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, color, true));
            }
            if (newParams.fontName !== initParams.fontName) {
                var fontInfo = newParams.fontName ? this.control.model.cache.fontInfoCache.findItem((fi: FontInfo) => { return fi.name == newParams.fontName; }) : null;
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseFontName, (prop: CharacterProperties) => { prop.fontInfo = fontInfo; });
                this.addHistoryItem(state.intervals, interval => new FontNameHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, fontInfo, true));
            }
            if (newParams.fontSize !== initParams.fontSize) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseDoubleFontSize, (prop: CharacterProperties) => { prop.fontSize = newParams.fontSize; });
                this.addHistoryItem(state.intervals, interval => new FontSizeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.fontSize, true));
            }
            if (newParams.fontStrikeoutType !== initParams.fontStrikeoutType) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseFontStrikeoutType, (prop: CharacterProperties) => prop.fontStrikeoutType = newParams.fontStrikeoutType);
                this.addHistoryItem(state.intervals, interval => new FontStrikeoutTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.fontStrikeoutType, true));
            }
            if (newParams.fontStyle !== initParams.fontStyle) {
                if ((newParams.fontStyle & 1) !== (initParams.fontStyle & 1)) {
                    this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseFontBold, (prop: CharacterProperties) => prop.fontBold = !!(newParams.fontStyle & 1));
                    this.addHistoryItem(state.intervals, interval => new FontBoldHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, !!(newParams.fontStyle & 1), true));
                }
                if ((newParams.fontStyle & 2) !== (initParams.fontStyle & 2)) {
                    this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseFontItalic, (prop: CharacterProperties) => prop.fontItalic = !!(newParams.fontStyle & 2));
                    this.addHistoryItem(state.intervals, interval => new FontItalicHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, !!(newParams.fontStyle & 2), true));
                }
            }
            if (newParams.fontUnderlineColor !== initParams.fontUnderlineColor) {
                var fontUnderlineColor = newParams.fontUnderlineColor == "Auto" ? ColorHelper.AUTOMATIC_COLOR : ColorHelper.hashToColor(newParams.fontUnderlineColor);
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseUnderlineColor, (prop: CharacterProperties) => prop.underlineColor = fontUnderlineColor);
                this.addHistoryItem(state.intervals, interval => new FontUnderlineColorHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, fontUnderlineColor, true));
            }
            if (newParams.fontUnderlineType !== initParams.fontUnderlineType) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseFontUnderlineType, (prop: CharacterProperties) => prop.fontUnderlineType = newParams.fontUnderlineType);
                this.addHistoryItem(state.intervals, interval => new FontUnderlineTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.fontUnderlineType, true));
            }
            if (newParams.hidden !== initParams.hidden) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseHidden, (prop: CharacterProperties) => prop.hidden = newParams.hidden);
                this.addHistoryItem(state.intervals, interval => new FontHiddenHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.hidden, true));
            }
            if (newParams.script !== initParams.script) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseScript, (prop: CharacterProperties) => prop.script = newParams.script);
                this.addHistoryItem(state.intervals, interval => new FontScriptHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.script, true));
            }
            if (newParams.underlineWordsOnly !== initParams.underlineWordsOnly) {
                this.applyParameter(maskedCharProps, charPropsRaw, charPropsFull, CharacterPropertiesMask.UseUnderlineWordsOnly, (prop: CharacterProperties) => prop.underlineWordsOnly = newParams.underlineWordsOnly);
                this.addHistoryItem(state.intervals, interval => new FontUnderlineWordsOnlyHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.underlineWordsOnly, true));
            }
            history.endTransaction();
        }

        private addHistoryItem(intervals: FixedInterval[], getHistoryItemAction: (interval: FixedInterval) => HistoryItem) {
            if(intervals.length > 1 || intervals[0].length > 0) {
                for(let i = 0, interval: FixedInterval; interval = intervals[i]; i++) {
                    this.control.history.addAndRedo(getHistoryItemAction(interval));
                }
            }
        }

        applyParameter(maskedCharProps: MaskedCharacterProperties, charPropsRaw: CharacterProperties, charPropsFull: CharacterProperties,
            mask: CharacterPropertiesMask, setProperties: (properties) => void ) {
            setProperties(maskedCharProps);
            setProperties(charPropsRaw);
            setProperties(charPropsFull);
            maskedCharProps.setUseValue(mask, true);
        }
    }

    export class DialogServiceFontCommand extends DialogFontCommandBase {
        dialogCustomNumberingListParameters: DialogCustomNumberingListParameters;

        createParameters(dialogCustomNumberingListParameters: DialogCustomNumberingListParameters): FontDialogParameters {
            this.dialogCustomNumberingListParameters = dialogCustomNumberingListParameters;
            var level: CustomListlevel = this.dialogCustomNumberingListParameters.levels[this.dialogCustomNumberingListParameters.currentLevel];
            var parameters: FontDialogParameters = new FontDialogParameters();
            parameters.initServicePart(level);
            return parameters;
        }
        applyParameters(state: IntervalCommandState, params: FontDialogParameters) {
            var level = this.dialogCustomNumberingListParameters.levels[this.dialogCustomNumberingListParameters.currentLevel];
            level.fontColor = params.fontColor;
            level.fontName = params.fontName;
            level.fontSize = params.fontSize;
            level.fontStyle = params.fontStyle;
        }
        afterClosing() {
            var clientCommand: RichEditClientCommand = RichEditClientCommand.ShowCustomNumberingListForm;
            this.control.commandManager.getCommand(clientCommand).execute(this.dialogCustomNumberingListParameters);
        }
    }

    export class FontDialogParameters extends DialogParametersBase {
        fontName: string = null;
        fontStyle: number = null;
        fontSize: number = null;
        fontColor: string = null;
        fontUnderlineType: UnderlineType = null;
        fontUnderlineColor: string = null;

        fontStrikeoutType: StrikeoutType = null;
        underlineWordsOnly: boolean = null;
        script: CharacterFormattingScript = null;

        allCaps: boolean = null;
        hidden: boolean = null;

        init(rawCharProps: CharacterProperties) {
            this.allCaps = rawCharProps.allCaps;
            this.fontColor = this.getColor(rawCharProps.foreColor);
            this.fontName = rawCharProps.fontInfo ? rawCharProps.fontInfo.name : null;
            this.fontSize = rawCharProps.fontSize;
            this.fontStrikeoutType = rawCharProps.fontStrikeoutType;
            this.fontStyle = (rawCharProps.fontBold ? 1 : 0) | (rawCharProps.fontItalic ? 2 : 0);
            this.fontUnderlineColor = this.getColor(rawCharProps.underlineColor);
            this.fontUnderlineType = rawCharProps.fontUnderlineType;
            this.hidden = rawCharProps.hidden;
            this.script = rawCharProps.script;
            this.underlineWordsOnly = rawCharProps.underlineWordsOnly;
        }

        getColor(color: number) {
            if(color == ColorHelper.AUTOMATIC_COLOR)
                return "Auto";
            if(color != undefined)
                return ColorHelper.colorToHash(color).toUpperCase();
            else
                return undefined;
        }

        initServicePart(level: CustomListlevel) {
            this.fontColor = level.fontColor;
            this.fontName = level.fontName;
            this.fontSize = level.fontSize;
            this.fontStyle = level.fontStyle;
        }

        copyFrom(obj: FontDialogParameters) {
            this.allCaps = obj.allCaps;
            this.fontColor = obj.fontColor;
            this.fontName = obj.fontName;
            this.fontSize = obj.fontSize;
            this.fontStrikeoutType = obj.fontStrikeoutType;
            this.fontStyle = obj.fontStyle;
            this.fontUnderlineColor = obj.fontUnderlineColor;
            this.fontUnderlineType = obj.fontUnderlineType
            this.hidden = obj.hidden;
            this.script = obj.script;
            this.underlineWordsOnly = obj.underlineWordsOnly;
        }

        getNewInstance(): DialogParametersBase {
            return new FontDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}  