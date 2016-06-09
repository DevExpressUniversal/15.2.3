module __aspxRichEdit {
    export class ListLevelPropertiesHistoryItemBase<T> extends HistoryItem {
        oldState: HistoryItemState<HistoryItemListLevelStateObject>;
        newValue: T;
        isAbstractList: boolean;
        listIndex: number;
        levelIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, isAbstractList: boolean, listIndex: number, levelIndex: number, newValue: T) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
            this.isAbstractList = isAbstractList;
            this.listIndex = listIndex;
            this.levelIndex = levelIndex;
        }

        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument.documentModel, this.isAbstractList, this.listIndex, this.levelIndex, this.newValue);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument.documentModel, this.oldState);
        }

        getManipulator(): IListLevelPropertyManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class ListLevelStartHistoryItem extends ListLevelPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyManipulator<number> {
            return this.modelManipulator.listLevelPropertiesManipulator.start;
        }
    }

    export class ListLevelAlignmentHistoryItem extends ListLevelPropertiesHistoryItemBase<ListNumberAlignment> {
        getManipulator(): IListLevelPropertyManipulator<ListNumberAlignment> {
            return this.modelManipulator.listLevelPropertiesManipulator.alignment;
        }
    }

    export class ListLevelConvertPreviousLevelNumberingToDecimalHistoryItem extends ListLevelPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyManipulator<boolean> {
            return this.modelManipulator.listLevelPropertiesManipulator.convertPreviousLevelNumberingToDecimal;
        }
    }

    export class ListLevelDisplayFormatStringHistoryItem extends ListLevelPropertiesHistoryItemBase<string> {
        getManipulator(): IListLevelPropertyManipulator<string> {
            return this.modelManipulator.listLevelPropertiesManipulator.displayFormatString;
        }
    }

    export class ListLevelFormatHistoryItem extends ListLevelPropertiesHistoryItemBase<NumberingFormat> {
        getManipulator(): IListLevelPropertyManipulator<NumberingFormat> {
            return this.modelManipulator.listLevelPropertiesManipulator.format;
        }
    }

    export class ListLevelLegacyHistoryItem extends ListLevelPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyManipulator<boolean> {
            return this.modelManipulator.listLevelPropertiesManipulator.legacy;
        }
    }

    export class ListLevelLegacyIndentHistoryItem extends ListLevelPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyManipulator<number> {
            return this.modelManipulator.listLevelPropertiesManipulator.legacyIndent;
        }
    }

    export class ListLevelLegacySpaceHistoryItem extends ListLevelPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyManipulator<number> {
            return this.modelManipulator.listLevelPropertiesManipulator.legacySpace;
        }
    }

    export class ListLevelOriginalLeftIndentHistoryItem extends ListLevelPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyManipulator<number> {
            return this.modelManipulator.listLevelPropertiesManipulator.originalLeftIndent;
        }
    }

    export class ListLevelRelativeRestartLevelHistoryItem extends ListLevelPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyManipulator<number> {
            return this.modelManipulator.listLevelPropertiesManipulator.relativeRestartLevel;
        }
    }

    export class ListLevelSeparatorHistoryItem extends ListLevelPropertiesHistoryItemBase<string> {
        getManipulator(): IListLevelPropertyManipulator<string> {
            return this.modelManipulator.listLevelPropertiesManipulator.separator;
        }
    }

    export class ListLevelSuppressBulletResizeHistoryItem extends ListLevelPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyManipulator<boolean> {
            return this.modelManipulator.listLevelPropertiesManipulator.suppressBulletResize;
        }
    }

    export class ListLevelSuppressRestartHistoryItem extends ListLevelPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyManipulator<boolean> {
            return this.modelManipulator.listLevelPropertiesManipulator.suppressRestart;
        }
    }

    export class ListLevelTemplateCodeHistoryItem extends ListLevelPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyManipulator<number> {
            return this.modelManipulator.listLevelPropertiesManipulator.templateCode;
        }
    }
} 