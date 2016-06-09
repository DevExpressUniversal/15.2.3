module __aspxRichEdit {

    /* Interval */
    export interface IIntervalPropertyWithUseValueManipulator<T> {
        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: T, newUse: boolean): HistoryItemIntervalState<HistoryItemIntervalUseStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalUseStateObject>);
    }
    export interface IIntervalPropertyManipulator<T> {
        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: T): HistoryItemIntervalState<HistoryItemIntervalStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStateObject>);
    }

    /* List Level */
    export interface IListLevelPropertyManipulator<T> {
        setValue(model: DocumentModel, isAbstractList: boolean, listIndex: number, listLevelIndex: number, newValue: T): HistoryItemState<HistoryItemListLevelStateObject>;
        restoreValue(model: DocumentModel, state: HistoryItemState<HistoryItemListLevelStateObject>);
    }
    export interface IListLevelPropertyWithUseManipulator<T> {
        setValue(model: DocumentModel, isAbstractList: boolean, listIndex: number, listLevelIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemListLevelUseStateObject>;
        restoreValue(model: DocumentModel, state: HistoryItemState<HistoryItemListLevelUseStateObject>);
    }

    /* Section */
    export interface ISectionPropertyManipulator<T> {
        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: T): HistoryItemState<HistoryItemSectionStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemSectionStateObject>);
    }

    /* Table Cell */
    export interface ITableCellPropertyWithUseManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemTableCellUseStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellUseStateObject>);
    }
    export interface ITableCellComplexPropertyWithUseManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValues: T[], newUses: boolean[]): HistoryItemState<HistoryItemTableCellComplexUseStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellComplexUseStateObject>);
    }
    export interface ITableCellPropertyManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValue: T): HistoryItemState<HistoryItemTableCellStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellStateObject>);
    }

    /* Table */
    export interface ITablePropertyManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, newValue: T): HistoryItemState<HistoryItemTableStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableStateObject>);
    }
    export interface ITablePropertyWithUseManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemTableUseStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableUseStateObject>);
    }
    export interface ITableComplexPropertyWithUseManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, newValues: T[], newUses: boolean[]): HistoryItemState<HistoryItemTableComplexUseStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableComplexUseStateObject>);
    }

    /* Table Row */
    export interface ITableRowPropertyWithUseManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemTableRowUseStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableRowUseStateObject>);
    }
    export interface ITableRowPropertyManipulator<T> {
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, newValue: T): HistoryItemState<HistoryItemTableRowStateObject>;
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableRowStateObject>);
    }
}