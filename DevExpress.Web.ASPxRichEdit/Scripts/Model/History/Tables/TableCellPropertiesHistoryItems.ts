module __aspxRichEdit {
    export class TableCellPropertiesHistoryItemBase<T> extends HistoryItem {
        newValue: T;
        tableIndex: number;
        rowIndex: number;
        cellIndex: number;

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValue: T) {
            super(modelManipulator, boundSubDocument);
            this.newValue = newValue;
            this.tableIndex = tableIndex;
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
        }
    }

    export class TableCellColumnSpanHistoryItem extends TableCellPropertiesHistoryItemBase<number> {
        oldState: HistoryItemState<HistoryItemTableCellStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.cellProperties.columnSpan.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.cellIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.cellProperties.columnSpan.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class TableCellVerticalMergingHistoryItem extends TableCellPropertiesHistoryItemBase<TableCellMergingState> {
        oldState: HistoryItemState<HistoryItemTableCellStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.cellProperties.verticalMerging.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.cellIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.cellProperties.verticalMerging.restoreValue(this.boundSubDocument, this.oldState);
        }

        static fromPosition(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, position: TablePosition, value: TableCellMergingState) {
            return new TableCellVerticalMergingHistoryItem(modelManipulator, boundSubDocument, position.table.index, position.rowIndex, position.cellIndex, value);
        }
    }


    export class TableCellPropertiesUseHistoryItemBase<T> extends TableCellPropertiesHistoryItemBase<T> {
        oldState: HistoryItemState<HistoryItemTableCellUseStateObject>;
        newUse: boolean;
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValues: T, newUse: boolean) {
            super(modelManipulator, boundSubDocument, tableIndex, rowIndex, cellIndex, newValues);
            this.newUse = newUse;
        }
        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.cellIndex, this.newValue, this.newUse);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }
        getManipulator(): ITableCellPropertyWithUseManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }
    export class TableCellPropertiesComplexUseHistoryItemBase<T> extends TableCellPropertiesHistoryItemBase<Array<T>> {
        oldState: HistoryItemState<HistoryItemTableCellComplexUseStateObject>;
        newUses: boolean[];
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValues: T[], newUses: boolean[]) {
            super(modelManipulator, boundSubDocument, tableIndex, rowIndex, cellIndex, newValues);
            if(newValues.length !== newUses.length)
                throw new Error("newValues.length should be equal to newUses.length");
            this.newUses = newUses;
        }
        public redo() {
            this.oldState = this.getManipulator().setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.cellIndex, this.newValue, this.newUses);
        }

        public undo() {
            this.getManipulator().restoreValue(this.boundSubDocument, this.oldState);
        }
        getManipulator(): ITableCellComplexPropertyWithUseManipulator<T> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class TableCellCellMarginsHistoryItem extends TableCellPropertiesComplexUseHistoryItemBase<TableWidthUnit> {
        getManipulator(): ITableCellComplexPropertyWithUseManipulator<TableWidthUnit> {
            return this.modelManipulator.tables.cellProperties.cellMargins;
        }
    }

    export class TableCellBordersHistoryItem extends TableCellPropertiesComplexUseHistoryItemBase<BorderInfo> {
        getManipulator(): ITableCellComplexPropertyWithUseManipulator<BorderInfo> {
            return this.modelManipulator.tables.cellProperties.borders;
        }
    }

    export class TableCellPreferredWidthHistoryItem extends TableCellPropertiesHistoryItemBase<TableWidthUnit> {
        oldState: HistoryItemState<HistoryItemTableCellStateObject>;
        public redo() {
            this.oldState = this.modelManipulator.tables.cellProperties.preferredWidth.setValue(this.boundSubDocument, this.tableIndex, this.rowIndex, this.cellIndex, this.newValue);
        }

        public undo() {
            this.modelManipulator.tables.cellProperties.preferredWidth.restoreValue(this.boundSubDocument, this.oldState);
        }
    }

    export class TableCellHideCellMarkHistoryItem extends TableCellPropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITableCellPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.cellProperties.hideCellMark;
        }
    }

    export class TableCellNoWrapHistoryItem extends TableCellPropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITableCellPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.cellProperties.noWrap;
        }
    }

    export class TableCellFitTextHistoryItem extends TableCellPropertiesUseHistoryItemBase<boolean> {
        getManipulator(): ITableCellPropertyWithUseManipulator<boolean> {
            return this.modelManipulator.tables.cellProperties.fitText;
        }
    }

    export class TableCellTextDirectionHistoryItem extends TableCellPropertiesUseHistoryItemBase<TextDirection> {
        getManipulator(): ITableCellPropertyWithUseManipulator<TextDirection> {
            return this.modelManipulator.tables.cellProperties.textDirection;
        }
    }

    export class TableCellVerticalAlignmentHistoryItem extends TableCellPropertiesUseHistoryItemBase<TableCellVerticalAlignment> {
        getManipulator(): ITableCellPropertyWithUseManipulator<TableCellVerticalAlignment> {
            return this.modelManipulator.tables.cellProperties.verticalAlignment;
        }
    }

    export class TableCellBackgroundColorHistoryItem extends TableCellPropertiesUseHistoryItemBase<number> {
        getManipulator(): ITableCellPropertyWithUseManipulator<number> {
            return this.modelManipulator.tables.cellProperties.backgroundColor;
        }
    }

    export class TableCellForeColorHistoryItem extends TableCellPropertiesUseHistoryItemBase<number> {
        getManipulator(): ITableCellPropertyWithUseManipulator<number> {
            return this.modelManipulator.tables.cellProperties.foreColor;
        }
    }

    export class TableCellShadingHistoryItem extends TableCellPropertiesUseHistoryItemBase<ShadingPattern> {
        getManipulator(): ITableCellPropertyWithUseManipulator<ShadingPattern> {
            return this.modelManipulator.tables.cellProperties.shading;
        }
    }
}