module __aspxRichEdit {
    export class DialogSymbolsCommandBase extends ShowDialogCommandBase {
        getDialogName() {
            return "Symbols";
        }
    }

    export class DialogSymbolsCommand extends DialogSymbolsCommandBase {
        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), this.control.selection.getIntervalsClone());
        }
        createParameters(): DialogSymbolsParameters {
            var charProps: CharacterProperties = this.control.inputPosition.getMergedCharacterPropertiesRaw();
            var parameters: DialogSymbolsParameters = new DialogSymbolsParameters();
            parameters.fontName = charProps.fontInfo ? charProps.fontInfo.name : this.control.model.defaultCharacterProperties.fontInfo.name;
            return parameters;
        }
        applyParameters(state: IntervalCommandStateEx, newParams: DialogSymbolsParameters) {
            this.control.history.beginTransaction();
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var maskedCharacterProperties = this.control.inputPosition.getMaskedCharacterProperties();
            var fontInfo = newParams.fontName ? this.control.model.cache.fontInfoCache.findItem((fi: FontInfo) => { return fi.name == newParams.fontName; }) : null;
            maskedCharacterProperties.fontInfo = fontInfo;
            maskedCharacterProperties.setUseValue(CharacterPropertiesMask.UseFontName, true);
            for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                if(interval.length > 0)
                    this.control.history.addAndRedo(new FontNameHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, fontInfo, true));
            }
            this.control.commandManager.getCommand(RichEditClientCommand.InsertText).execute(newParams.symbol);
            this.control.history.endTransaction();
        }
    }

    export class DialogServiceSymbolsCommand extends DialogSymbolsCommandBase {
        dialogCustomNumberingListParameters: DialogCustomNumberingListParameters;

        createParameters(dialogCustomNumberingListParameters: DialogCustomNumberingListParameters): DialogSymbolsParameters {
            this.dialogCustomNumberingListParameters = dialogCustomNumberingListParameters;
            
            var parameters = new DialogSymbolsParameters();
            parameters.fontName = dialogCustomNumberingListParameters.levels[dialogCustomNumberingListParameters.currentLevel].fontName;
            return parameters;
        }
        applyParameters(state: IntervalCommandState, newParams: DialogSymbolsParameters) {
            var level = this.dialogCustomNumberingListParameters.levels[this.dialogCustomNumberingListParameters.currentLevel];
            level.fontName = newParams.fontName;
            level.displayFormatString = newParams.symbol;
        }
        afterClosing() {
            var clientCommand: RichEditClientCommand = RichEditClientCommand.ShowCustomNumberingListForm;
            this.control.commandManager.getCommand(clientCommand).execute(this.dialogCustomNumberingListParameters);
        }
    }

    export class DialogSymbolsParameters extends DialogParametersBase {
        symbol: string;
        fontName: string;

        getNewInstance(): DialogParametersBase {
            return new DialogSymbolsParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }

        copyFrom(obj: DialogSymbolsParameters) {
            this.symbol = obj.symbol;
            this.fontName = obj.fontName;
        }
    }
} 