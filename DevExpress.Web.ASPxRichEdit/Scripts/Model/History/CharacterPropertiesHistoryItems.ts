module __aspxRichEdit {
    export class FontUseValueHistoryItem extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemIntervalStateObject>;
        newValue: number;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, newValue: number) {
            super(modelManipulator, boundSubDocument, interval);
            this.newValue = newValue;
        }
        public redo() {
            this.oldState = this.modelManipulator.characterPropertiesManipulator.useValue.setValue(this.boundSubDocument, this.interval, this.newValue);
        }

        public undo() {
            this.modelManipulator.characterPropertiesManipulator.useValue.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class CharacterPropertiesHistoryItemBase<T> extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemIntervalUseStateObject>;
        newValue: T;
        newUse: boolean;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, newValue: T, newUse: boolean) {
            super(modelManipulator, boundSubDocument, interval);
            this.newValue = newValue;
            this.newUse = newUse;
        }

        public redo() {
            this.oldState = this.getPropertiesManipulator().setValue(this.boundSubDocument, this.interval, this.newValue, this.newUse);
        }

        public undo() {
            this.getPropertiesManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }

        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class FontBoldHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontBold;
        }
    }

    export class FontCapsHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontCaps;
        }
    }

    export class FontUnderlineTypeHistoryItem extends CharacterPropertiesHistoryItemBase<UnderlineType> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<UnderlineType> {
            return this.modelManipulator.characterPropertiesManipulator.fontUnderlineType;
        }
    }

    export class FontForeColorHistoryItem extends CharacterPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.characterPropertiesManipulator.foreColor;
        }
    }

    export class FontBackColorHistoryItem extends CharacterPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.characterPropertiesManipulator.backColor;
        }
    }

    export class FontHiddenHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontHidden;
        }
    }

    export class FontItalicHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontItalic;
        }
    }

    export class FontNameHistoryItem extends CharacterPropertiesHistoryItemBase<FontInfo> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<FontInfo> {
            return this.modelManipulator.characterPropertiesManipulator.fontName;
        }
    }

    export class FontScriptHistoryItem extends CharacterPropertiesHistoryItemBase<CharacterFormattingScript> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<CharacterFormattingScript> {
            return this.modelManipulator.characterPropertiesManipulator.script;
        }
    }

    export class FontSizeHistoryItem extends CharacterPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.characterPropertiesManipulator.fontSize;
        }
    }

    export class FontStrikeoutTypeHistoryItem extends CharacterPropertiesHistoryItemBase<StrikeoutType> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<StrikeoutType> {
            return this.modelManipulator.characterPropertiesManipulator.fontStrikeoutType;
        }
    }

    export class FontStrikeoutWordsOnlyHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontStrikeoutWordsOnly;
        }
    }

    export class FontStrikeoutColorHistoryItem extends CharacterPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.characterPropertiesManipulator.fontStrikeoutColor;
        }
    }

    export class FontUnderlineColorHistoryItem extends CharacterPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.characterPropertiesManipulator.fontUnderlineColor;
        }
    }

    export class FontUnderlineWordsOnlyHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontUnderlineWordsOnly;
        }
    }

    export class FontNoProofHistoryItem extends CharacterPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.characterPropertiesManipulator.fontNoProof;
        }
    }

    export class ResetCharacterPropertiesUseValuesHistoryItem extends IntervalBasedHistoryItem {
        state: HistoryItemIntervalState<HistoryItemIntervalStateObject>;

        public redo() {
            this.state = this.modelManipulator.characterPropertiesManipulator.useValue.setValue(this.boundSubDocument, this.interval, 0);
        }

        public undo() {
            this.modelManipulator.characterPropertiesManipulator.useValue.restoreValue(this.boundSubDocument, this.state);
        }
    }

    // CARE! Don't move it class. Or historyItemType will undefined
    export class PropertiesWhatNeedSetWhenCreateHyperlinkField {
        public static info: { historyItemType: any; propertyName: string }[] = [
            { historyItemType: FontSizeHistoryItem, propertyName: "fontSize" },
            { historyItemType: FontBoldHistoryItem, propertyName: "fontBold" },
            { historyItemType: FontItalicHistoryItem, propertyName: "fontItalic" },
            { historyItemType: FontNameHistoryItem, propertyName: "fontInfo" },
            { historyItemType: FontScriptHistoryItem, propertyName: "script" },
            { historyItemType: FontStrikeoutTypeHistoryItem, propertyName: "fontStrikeoutType" },
            //{ historyItemType: FontUnderlineTypeHistoryItem, propertyName: "fontUnderlineType" },
            { historyItemType: FontCapsHistoryItem, propertyName: "allCaps" },
            { historyItemType: FontUnderlineWordsOnlyHistoryItem, propertyName: "underlineWordsOnly" },
            { historyItemType: FontStrikeoutWordsOnlyHistoryItem, propertyName: "strikeoutWordsOnly" },
            { historyItemType: FontNoProofHistoryItem, propertyName: "noProof" },
            { historyItemType: FontHiddenHistoryItem, propertyName: "hidden" },
            //{ historyItemType: FontForeColorHistoryItem, propertyName: "foreColor" },
            { historyItemType: FontBackColorHistoryItem, propertyName: "backColor" },
            { historyItemType: FontStrikeoutColorHistoryItem, propertyName: "strikeoutColor" },
            { historyItemType: FontUnderlineColorHistoryItem, propertyName: "underlineColor" },
        ];
    }
} 