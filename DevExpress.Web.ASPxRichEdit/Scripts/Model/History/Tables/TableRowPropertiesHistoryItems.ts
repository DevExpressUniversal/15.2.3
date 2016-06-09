module __aspxRichEdit {
    export class TableRowPropertiesHistoryItemBase<T> extends HistoryItem {
        newValue: T;
        tableIndex: number;
        rowIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, rowIndex: number, newValue: T) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
            this.tableIndex = tableIndex;
            this.rowIndex = rowIndex;
        }
    }

    export class TableRowGridAfterHistoryItem extends TableRowPropertiesHistoryItemBase<number> {
        oldState: HistoryItemState<HistoryItemTableRowStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.rowProperties.gridAfter.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.rowProperties.gridAfter.restoreValue(this.boundSubDocument, this.oldState);
        }
    }
    export class TableRowGridBeforeHistoryItem extends TableRowPropertiesHistoryItemBase<number> {
        oldState: HistoryItemState<HistoryItemTableRowStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.rowProperties.gridBefore.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.rowProperties.gridBefore.restoreValue(this.boundSubDocument, this.oldState);
        }
    }
    export class TableRowWidthAfterHistoryItem extends TableRowPropertiesHistoryItemBase<TableWidthUnit> {
        oldState: HistoryItemState<HistoryItemTableRowStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.rowProperties.widthAfter.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.rowProperties.widthAfter.restoreValue(this.boundSubDocument, this.oldState);
        }
    }
    export class TableRowWidthBeforeHistoryItem extends TableRowPropertiesHistoryItemBase<TableWidthUnit> {
        oldState: HistoryItemState<HistoryItemTableRowStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.rowProperties.widthBefore.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.rowProperties.widthBefore.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    /* Cache properties */
    export class TableRowPropertiesUseHistoryItemBase<T> extends TableRowPropertiesHistoryItemBase<T> {
        oldState: HistoryItemState<HistoryItemTableRowUseStateObject>;
        newUse: boolean;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, rowIndex: number, newValues: T, newUse: boolean) {
            super(modelManipulator, boundSubDocument, tableIndex, rowIndex, newValues);
            this.newUse = newUse;
        }
        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.newValue, this.newUse);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }
        getManipulator(): ITableRowPropertyWithUseManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class TableRowHeightHistoryItem extends TableRowPropertiesHistoryItemBase<TableHeightUnit> {
        oldState: HistoryItemState<HistoryItemTableRowStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.rowProperties.height.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.rowProperties.height.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class TableRowCellSpacingHistoryItem extends TableRowPropertiesUseHistoryItemBase<TableWidthUnit> {
        getManipulator(): ITableRowPropertyWithUseManipulator<TableWidthUnit> {
            return this.modelManipulator.tables.rowProperties.cellSpacing;
        }
    }

    export class TableRowCantSplitHistoryItem extends TableRowPropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITableRowPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.rowProperties.cantSplit;
        }
    }

    export class TableRowHideCellMarkHistoryItem extends TableRowPropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITableRowPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.rowProperties.hideCellMark;
        }
    }

    export class TableRowHeaderHistoryItem extends TableRowPropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITableRowPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.rowProperties.header;
        }
    }

    export class TableRowTableRowAlignmentHistoryItem extends TableRowPropertiesUseHistoryItemBase<TableRowAlignment> {
        getManipulator(): ITableRowPropertyWithUseManipulator<TableRowAlignment> {
            return this.modelManipulator.tables.rowProperties.tableRowAlignment;
        }
    }
}