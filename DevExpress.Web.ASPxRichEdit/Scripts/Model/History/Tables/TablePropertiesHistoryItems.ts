module __aspxRichEdit {
    export class TablePropertiesHistoryItemBase<T> extends HistoryItem {
        newValue: T;
        tableIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, newValue: T) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
            this.tableIndex = tableIndex;
        }
    }
    export class TablePropertiesUseHistoryItemBase<T> extends TablePropertiesHistoryItemBase<T> {
        oldState: HistoryItemState<HistoryItemTableUseStateObject>;
        newUse: boolean;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, newValues: T, newUse: boolean) {
            super(modelManipulator, boundSubDocument, tableIndex, newValues);
            this.newUse = newUse;
        }
        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument, this.tableIndex, this.newValue, this.newUse);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }
        getManipulator(): ITablePropertyWithUseManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }
    export class TablePropertiesComplexUseHistoryItemBase<T> extends TablePropertiesHistoryItemBase<Array<T>> {
        oldState: HistoryItemState<HistoryItemTableComplexUseStateObject>;
        newUses: boolean[];
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, newValues: T[], newUses: boolean[]) {
            super(modelManipulator, boundSubDocument, tableIndex, newValues);
            if(newValues.length !== newUses.length)
                throw new Error("newValues.length should be equal to newUses.length");
            this.newUses = newUses;
        }
        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument, this.tableIndex, this.newValue, this.newUses);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }
        getManipulator(): ITableComplexPropertyWithUseManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class TableCellMarginsHistoryItem extends TablePropertiesComplexUseHistoryItemBase<TableWidthUnit> {
        getManipulator(): ITableComplexPropertyWithUseManipulator<TableWidthUnit> {
            return this.modelManipulator.tables.tableProperties.cellMargins;
        }
    }

    export class TableCellSpacingHistoryItem extends TablePropertiesUseHistoryItemBase<TableWidthUnit> {
        getManipulator(): ITablePropertyWithUseManipulator<TableWidthUnit> {
            return this.modelManipulator.tables.tableProperties.cellSpacing;
        }
    }

    export class TableIndentHistoryItem extends TablePropertiesUseHistoryItemBase<TableWidthUnit> {
        getManipulator(): ITablePropertyWithUseManipulator<TableWidthUnit> {
            return this.modelManipulator.tables.tableProperties.indent;
        }
    }

    export class TablePreferredWidthHistoryItem extends TablePropertiesHistoryItemBase<TableWidthUnit> {
        oldState: HistoryItemState<HistoryItemTableStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.tableProperties.preferredWidth.setValue(this.boundSubDocument, this.tableIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.tableProperties.preferredWidth.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class TableBordersHistoryItem extends TablePropertiesComplexUseHistoryItemBase<BorderInfo> {
        getManipulator(): ITableComplexPropertyWithUseManipulator<BorderInfo> {
            return this.modelManipulator.tables.tableProperties.borders;
        }
    }

    export class TableTableStyleColumnBandSizeHistoryItem extends TablePropertiesUseHistoryItemBase<number> {
        getManipulator(): ITablePropertyWithUseManipulator<number> {
            return this.modelManipulator.tables.tableProperties.tableStyleColumnBandSize;
        }
    }

    export class TableTableStyleRowBandSizeHistoryItem extends TablePropertiesUseHistoryItemBase<number> {
        getManipulator(): ITablePropertyWithUseManipulator<number> {
            return this.modelManipulator.tables.tableProperties.tableStyleRowBandSize;
        }
    }

    export class TableAvoidDoubleBordersHistoryItem extends TablePropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITablePropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.tableProperties.avoidDoubleBorders;
        }
    }

    export class TableLayoutTypeHistoryItem extends TablePropertiesUseHistoryItemBase<TableLayoutType> {
        getManipulator(): ITablePropertyWithUseManipulator<TableLayoutType> {
            return this.modelManipulator.tables.tableProperties.layoutType;
        }
    }

    export class TableLookTypesHistoryItem extends TablePropertiesHistoryItemBase<TableLookTypes> {
        oldState: HistoryItemState<HistoryItemTableStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.tableProperties.lookTypes.setValue(this.boundSubDocument, this.tableIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.tableProperties.lookTypes.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class TableBackgroundColorHistoryItem extends TablePropertiesUseHistoryItemBase<number> {
        getManipulator(): ITablePropertyWithUseManipulator<number> {
            return this.modelManipulator.tables.tableProperties.backgroundColor;
        }
    }

    export class TableTableRowAlignmentHistoryItem extends TablePropertiesUseHistoryItemBase<TableRowAlignment> {
        getManipulator(): ITablePropertyWithUseManipulator<TableRowAlignment> {
            return this.modelManipulator.tables.tableProperties.tableRowAlignment;
        }
    }

    export class TableIsTableOverlapHistoryItem extends TablePropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITablePropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.tableProperties.isTableOverlap;
        }
    }
}