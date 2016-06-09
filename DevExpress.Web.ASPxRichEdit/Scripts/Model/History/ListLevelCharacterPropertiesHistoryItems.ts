module __aspxRichEdit {
    export class ListLevelCharacterPropertiesHistoryItemBase<T> extends HistoryItem {
        oldState: HistoryItemState<HistoryItemListLevelUseStateObject>;
        newValue: T;
        isAbstractList: boolean;
        listIndex: number;
        levelIndex: number;
        newUse: boolean;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, isAbstractList: boolean, listIndex: number, levelIndex: number, newValue: T, newUse: boolean) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
            this.isAbstractList = isAbstractList;
            this.listIndex = listIndex;
            this.levelIndex = levelIndex;
            this.newUse = newUse;
        }

        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument.documentModel, this.isAbstractList, this.listIndex, this.levelIndex, this.newValue, this.newUse);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument.documentModel, this.oldState);
        }

        getManipulator(): IListLevelPropertyWithUseManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class ListLevelFontBoldHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontBold;
        }
    }

    export class ListLevelFontCapsHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontCaps;
        }
    }

    export class ListLevelFontUnderlineTypeHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<UnderlineType> {
        getManipulator(): IListLevelPropertyWithUseManipulator<UnderlineType> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontUnderlineType;
        }
    }

    export class ListLevelFontForeColorHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.foreColor;
        }
    }

    export class ListLevelFontBackColorHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.backColor;
        }
    }

    export class ListLevelFontHiddenHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontHidden;
        }
    }

    export class ListLevelFontItalicHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontItalic;
        }
    }

    export class ListLevelFontNameHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<FontInfo> {
        getManipulator(): IListLevelPropertyWithUseManipulator<FontInfo> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontName;
        }
    }

    export class ListLevelFontScriptHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<CharacterFormattingScript> {
        getManipulator(): IListLevelPropertyWithUseManipulator<CharacterFormattingScript> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.script;
        }
    }

    export class ListLevelFontSizeHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontSize;
        }
    }

    export class ListLevelFontStrikeoutTypeHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<StrikeoutType> {
        getManipulator(): IListLevelPropertyWithUseManipulator<StrikeoutType> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontStrikeoutType;
        }
    }

    export class ListLevelFontStrikeoutWordsOnlyHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontStrikeoutWordsOnly;
        }
    }

    export class ListLevelFontStrikeoutColorHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontStrikeoutColor;
        }
    }

    export class ListLevelFontUnderlineColorHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontUnderlineColor;
        }
    }

    export class ListLevelFontUnderlineWordsOnlyHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontUnderlineWordsOnly;
        }
    }

    export class ListLevelFontNoProofHistoryItem extends ListLevelCharacterPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelCharacterPropertiesManipulator.fontNoProof;
        }
    }
} 