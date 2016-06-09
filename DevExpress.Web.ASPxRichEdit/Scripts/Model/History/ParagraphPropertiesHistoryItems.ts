module __aspxRichEdit {
    export class ParagraphUseValueHistoryItem extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemIntervalStateObject>
        newValue: number;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, newValue: number) {
            super(modelManipulator, boundSubDocument, interval);
            this.newValue = newValue;
        }
        public redo() {
            this.oldState = this.modelManipulator.paragraphPropertiesManipulator.useValue.setValue(this.boundSubDocument, this.interval, this.newValue);
        } 

        public undo() {
            this.modelManipulator.paragraphPropertiesManipulator.useValue.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class TabHistoryItemBase extends IntervalBasedHistoryItem {
        tabInfo: TabInfo;
        oldState: HistoryItemIntervalState<HistoryItemTabStateObject>;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, tabInfo: TabInfo) {
            super(modelManipulator, boundSubDocument, interval);
            this.tabInfo = tabInfo;
        }
    }
    export class InsertTabToParagraphHistoryItem extends TabHistoryItemBase {
        public redo(): void {
            this.oldState = this.modelManipulator.tabManipulator.insertTabToParagraph(this.boundSubDocument, this.interval, this.tabInfo);
        }
        public undo(): void {
            this.modelManipulator.tabManipulator.restoreInsertedTabToParagraph(this.boundSubDocument, this.oldState);
        }
    }
    export class DeleteTabAtParagraphHistoryItem extends TabHistoryItemBase {
        public redo(): void {
            this.oldState = this.modelManipulator.tabManipulator.deleteTabAtParagraph(this.boundSubDocument, this.interval, this.tabInfo);
        }
        public undo(): void {
            this.modelManipulator.tabManipulator.restoreDeletedTabAtParagraph(this.boundSubDocument, this.oldState);
        }
    }

    export class ParagraphPropertiesHistoryItemBase<T> extends IntervalBasedHistoryItem {
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

    export class ParagraphAlignmentHistoryItem extends ParagraphPropertiesHistoryItemBase<ParagraphAlignment> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<ParagraphAlignment> {
            return this.modelManipulator.paragraphPropertiesManipulator.align;
        }
    }
    export class ParagraphContextualSpacingHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.contextualSpacing;
        }
    }
    export class ParagraphAfterAutoSpacingHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.afterAutoSpacing;
        }
    }
    export class ParagraphBackColorHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.backColor;
        }
    }
    export class ParagraphBeforeAutoSpacingHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.beforeAutoSpacing;
        }
    }
    export class ParagraphFirstLineIndentHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.firstLineIndent;
        }
    }
    export class ParagraphFirstLineIndentTypeHistoryItem extends ParagraphPropertiesHistoryItemBase<ParagraphFirstLineIndent> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<ParagraphFirstLineIndent> {
            return this.modelManipulator.paragraphPropertiesManipulator.firstLineIndentType;
        }
    }
    export class ParagraphKeepLinesTogetherHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.keepLinesTogether;
        }
    }
    export class ParagraphLeftIndentHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.leftIndent;
        }
    }
    export class ParagraphLineSpacingHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.lineSpacing;
        }
    }
    export class ParagraphLineSpacingTypeHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.lineSpacingType;
        }
    }
    export class ParagraphOutlineLevelHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.outlineLevel;
        }
    }
    export class ParagraphPageBreakBeforeHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.pageBreakBefore;
        }
    }
    export class ParagraphRightIndentHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.rightIndent;
        }
    }
    export class ParagraphSpacingAfterHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.spacingAfter;
        }
    }
    export class ParagraphSpacingBeforeHistoryItem extends ParagraphPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<number> {
            return this.modelManipulator.paragraphPropertiesManipulator.spacingBefore;
        }
    }
    export class ParagraphSuppressHyphenationHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.suppressHyphenation;
        }
    }
    export class ParagraphSuppressLineNumbersHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.suppressLineNumbers;
        }
    }
    export class ParagraphWidowOrphanControlHistoryItem extends ParagraphPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): IIntervalPropertyWithUseValueManipulator<boolean> {
            return this.modelManipulator.paragraphPropertiesManipulator.widowOrphanControl;
        }
    }

    export class ParagraphPropertiesHistoryItem extends HistoryItem {
        paragraphProperties: MaskedParagraphProperties;
        style: ParagraphStyle;
        numberingListIndex: number;
        listLevelIndex: number;
        tabs: TabProperties;
        paragraphIndex: number;

        oldParagraphProperties: MaskedParagraphProperties;
        oldStyle: ParagraphStyle;
        oldNumberingListIndex: number;
        oldListLevelIndex: number;
        oldTabs: TabProperties;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, paragraphIndex: number, paragraphProperties: MaskedParagraphProperties, style: ParagraphStyle, numberingListIndex: number, listLevelIndex: number, tabs: TabProperties) {
            super(modelManipulator, boundSubDocument);
            this.paragraphIndex = paragraphIndex;
            this.style = style;
            this.numberingListIndex = numberingListIndex;
            this.listLevelIndex = listLevelIndex;
            this.paragraphProperties = paragraphProperties;
            this.tabs = tabs.clone();
        }
        public redo() {
            let paragraph = this.boundSubDocument.paragraphs[this.paragraphIndex];
            this.oldParagraphProperties = paragraph.maskedParagraphProperties;
            this.oldStyle = paragraph.paragraphStyle;
            this.oldNumberingListIndex = paragraph.numberingListIndex;
            this.oldListLevelIndex = paragraph.listLevelIndex;
            this.oldTabs = paragraph.tabs.clone();
            this.modelManipulator.paragraphPropertiesManipulator.changeAllProperties(this.boundSubDocument, this.paragraphIndex, this.paragraphProperties, this.style, this.tabs, this.numberingListIndex, this.listLevelIndex);
        }

        public undo() {
            this.modelManipulator.paragraphPropertiesManipulator.changeAllProperties(this.boundSubDocument, this.paragraphIndex, this.oldParagraphProperties, this.oldStyle, this.oldTabs, this.oldNumberingListIndex, this.oldListLevelIndex);
        }
    }
}  