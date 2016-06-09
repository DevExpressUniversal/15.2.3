module __aspxRichEdit {
    export class SectionPropertiesHistoryItemBase<T> extends IntervalBasedHistoryItem {
        oldState: HistoryItemState<HistoryItemSectionStateObject>;
        newValue: T;

        constructor(modelManipulator: ModelManipulator, boundSubDocument, interval: FixedInterval, newValue: T) {
            super(modelManipulator, boundSubDocument, interval);
            this.newValue = newValue;
        }

        public redo() {
            this.oldState = this.getPropertiesManipulator().setValue(this.boundSubDocument, this.interval, this.newValue);
        } 

        public undo() {
            this.getPropertiesManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }

        getPropertiesManipulator(): ISectionPropertyManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class SectionMarginLeftHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.marginLeft;
        }
    }

    export class SectionMarginTopHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.marginTop;
        }
    }

    export class SectionMarginRightHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.marginRight;
        }
    }

    export class SectionMarginBottomHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.marginBottom;
        }
    }

    export class SectionColumnCountHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.columnCount;
        }
    }

    export class SectionSpaceHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.space;
        }
    }

    export class SectionEqualWidthColumnsHistoryItem extends SectionPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): ISectionPropertyManipulator<boolean> {
            return this.modelManipulator.sectionProperties.equalWidthColumns;
        }
    }

    export class SectionColumnsInfoHistoryItem extends SectionPropertiesHistoryItemBase<SectionColumnProperties[]> {
        getPropertiesManipulator(): ISectionPropertyManipulator<SectionColumnProperties[]> {
            return this.modelManipulator.sectionProperties.columnsInfo;
        }
    }

    export class SectionPageWidthHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.pageWidth;
        }
    }

    export class SectionPageHeightHistoryItem extends SectionPropertiesHistoryItemBase<number> {
        getPropertiesManipulator(): ISectionPropertyManipulator<number> {
            return this.modelManipulator.sectionProperties.pageHeight;
        }
    }

    export class SectionStartTypeHistoryItem extends SectionPropertiesHistoryItemBase<SectionStartType> {
        getPropertiesManipulator(): ISectionPropertyManipulator<SectionStartType> {
            return this.modelManipulator.sectionProperties.startType;
        }
    }

    export class SectionLandscapeHistoryItem extends SectionPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): ISectionPropertyManipulator<boolean> {
            return this.modelManipulator.sectionProperties.landscape;
        }
    }
    export class SectionDifferentFirstPageHistoryItem extends SectionPropertiesHistoryItemBase<boolean> {
        getPropertiesManipulator(): ISectionPropertyManipulator<boolean> {
            return this.modelManipulator.sectionProperties.differentFirstPage;
        }
    }
}