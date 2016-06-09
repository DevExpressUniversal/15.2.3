module __aspxRichEdit {
    export class ListLevelParagraphPropertiesHistoryItemBase<T> extends HistoryItem {
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

    export class ListLevelParagraphAlignmentHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<ParagraphAlignment> {
        getManipulator(): IListLevelPropertyWithUseManipulator<ParagraphAlignment> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.align;
        }
    }
    export class ListLevelParagraphContextualSpacingHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.contextualSpacing;
        }
    }
    export class ListLevelParagraphAfterAutoSpacingHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.afterAutoSpacing;
        }
    }
    export class ListLevelParagraphBackColorHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.backColor;
        }
    }
    export class ListLevelParagraphBeforeAutoSpacingHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.beforeAutoSpacing;
        }
    }
    export class ListLevelParagraphFirstLineIndentHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.firstLineIndent;
        }
    }
    export class ListLevelParagraphFirstLineIndentTypeHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<ParagraphFirstLineIndent> {
        getManipulator(): IListLevelPropertyWithUseManipulator<ParagraphFirstLineIndent> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.firstLineIndentType;
        }
    }
    export class ListLevelParagraphKeepLinesTogetherHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.keepLinesTogether;
        }
    }
    export class ListLevelParagraphLeftIndentHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.leftIndent;
        }
    }
    export class ListLevelParagraphLineSpacingHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.lineSpacing;
        }
    }
    export class ListLevelParagraphLineSpacingTypeHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.lineSpacingType;
        }
    }
    export class ListLevelParagraphOutlineLevelHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.outlineLevel;
        }
    }
    export class ListLevelParagraphPageBreakBeforeHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.pageBreakBefore;
        }
    }
    export class ListLevelParagraphRightIndentHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.rightIndent;
        }
    }
    export class ListLevelParagraphSpacingAfterHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.spacingAfter;
        }
    }
    export class ListLevelParagraphSpacingBeforeHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<number> {
        getManipulator(): IListLevelPropertyWithUseManipulator<number> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.spacingBefore;
        }
    }
    export class ListLevelParagraphSuppressHyphenationHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.suppressHyphenation;
        }
    }
    export class ListLevelParagraphSuppressLineNumbersHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.suppressLineNumbers;
        }
    }
    export class ListLevelParagraphWidowOrphanControlHistoryItem extends ListLevelParagraphPropertiesHistoryItemBase<boolean> {
        getManipulator(): IListLevelPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.listLevelParagraphPropertiesManipulator.widowOrphanControl;
        }
    }
}