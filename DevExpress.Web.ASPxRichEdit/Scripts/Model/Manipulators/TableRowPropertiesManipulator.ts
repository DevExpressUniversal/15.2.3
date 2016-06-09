module __aspxRichEdit {
    export class TableRowPropertiesManipulator {
        height: ITableRowPropertyManipulator<TableHeightUnit>;
        cellSpacing: ITableRowPropertyWithUseManipulator<TableWidthUnit>;
        cantSplit: ITableRowPropertyWithUseManipulator<boolean>;
        hideCellMark: ITableRowPropertyWithUseManipulator<boolean>;
        header: ITableRowPropertyWithUseManipulator<boolean>;
        tableRowAlignment: ITableRowPropertyWithUseManipulator<TableRowAlignment>;
        gridAfter: ITableRowPropertyManipulator<number>;
        gridBefore: ITableRowPropertyManipulator<number>;
        widthAfter: ITableRowPropertyManipulator<TableWidthUnit>;
        widthBefore: ITableRowPropertyManipulator<TableWidthUnit>;

        constructor(manipulator: ModelManipulator) {
            this.cellSpacing = new TableRowPropertiesWithUseManipulatorCore<TableWidthUnit>(manipulator, JSONTableRowProperty.CellSpacing, TableRowPropertiesMask.UseCellSpacing,
                (prop, val) => prop.cellSpacing = val, prop => prop.cellSpacing);
            this.cantSplit = new TableRowPropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableRowProperty.CantSplit, TableRowPropertiesMask.UseCantSplit,
                (prop, val) => prop.cantSplit = val, prop => prop.cantSplit);
            this.hideCellMark = new TableRowPropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableRowProperty.HideCellMark, TableRowPropertiesMask.UseHideCellMark,
                (prop, val) => prop.hideCellMark = val, prop => prop.hideCellMark);
            this.header = new TableRowPropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableRowProperty.Header, TableRowPropertiesMask.UseHeader,
                (prop, val) => prop.header = val, prop => prop.header);
            this.tableRowAlignment = new TableRowPropertiesWithUseManipulatorCore<TableRowAlignment>(manipulator, JSONTableRowProperty.TableRowAlignment, TableRowPropertiesMask.UseTableRowAlignment,
                (prop, val) => prop.tableRowAlignment = val, prop => prop.tableRowAlignment);
            this.height = new TableRowPropertiesManipulatorCore<TableHeightUnit>(manipulator, JSONTableRowProperty.Height,
                (row, val) => row.height = val, prop => prop.height);
            this.gridAfter = new TableRowPropertiesManipulatorCore<number>(manipulator, JSONTableRowProperty.GridAfter,
                (row, val) => row.gridAfter = val, row => row.gridAfter);
            this.gridBefore = new TableRowPropertiesManipulatorCore<number>(manipulator, JSONTableRowProperty.GridBefore,
                (row, val) => row.gridBefore = val, row => row.gridBefore);
            this.widthAfter = new TableRowPropertiesManipulatorCore<TableWidthUnit>(manipulator, JSONTableRowProperty.WidthAfter,
                (row, val) => row.widthAfter = val, row => row.widthAfter);
            this.widthBefore = new TableRowPropertiesManipulatorCore<TableWidthUnit>(manipulator, JSONTableRowProperty.WidthBefore,
                (row, val) => row.widthBefore = val, row => row.widthBefore);
        }
    }

    class TableRowPropertiesManipulatorCore<T> implements ITableRowPropertyManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableRowProperty: JSONTableRowProperty;
        setProperty: (row: TableRow, value: T) => void;
        getProperty: (row: TableRow) => T;
        constructor(manipulator: ModelManipulator, jsonTableCellProperty: JSONTableRowProperty, setProperty: (row: TableRow, value: T) => void, getProperty: (row: TableRow) => T) {
            this.manipulator = manipulator;
            this.jsonTableRowProperty = jsonTableCellProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, newValue: T): HistoryItemState<HistoryItemTableRowStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableRowStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableRowStateObject>();
            let row = table.rows[rowIndex];
            oldState.register(new HistoryItemTableRowStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, this.getProperty(row)));
            this.setProperty(row, newValue);
            newState.register(new HistoryItemTableRowStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, newValue));
            this.manipulator.dispatcher.notifyTableRowPropertyChanged(subDocument, this.jsonTableRowProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableRowUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            let row = table.rows[state.lastObject.rowIndex];
            this.setProperty(row, state.lastObject.value);
            this.manipulator.dispatcher.notifyTableRowPropertyChanged(subDocument, this.jsonTableRowProperty, state);
        }
    }

    class TableRowPropertiesWithUseManipulatorCore<T> implements ITableRowPropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableRowProperty: JSONTableRowProperty;
        tableRowPropertiesMask: TableRowPropertiesMask;
        setProperty: (properties: TableRowProperties, value: T) => void;
        getProperty: (properties: TableRowProperties) => T;

        constructor(manipulator: ModelManipulator, jsonTableCellProperty: JSONTableRowProperty, tableRowPropertiesMask: TableRowPropertiesMask, setProperty: (properties: TableRowProperties, value: T) => void, getProperty: (properties: TableRowProperties) => T) {
            this.manipulator = manipulator;
            this.tableRowPropertiesMask = tableRowPropertiesMask;
            this.jsonTableRowProperty = jsonTableCellProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }

        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemTableRowUseStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableRowUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableRowUseStateObject>();
            let row = table.rows[rowIndex];
            let properties = row.properties;

            oldState.register(new HistoryItemTableRowUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, this.getProperty(properties), properties.getUseValue(this.tableRowPropertiesMask)));
            this.setValueCore(subDocument.documentModel.cache, row, newValue, newUse);
            newState.register(new HistoryItemTableRowUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, newValue, newUse));
            this.manipulator.dispatcher.notifyTableRowPropertyChanged(subDocument, this.jsonTableRowProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableRowUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            let row = table.rows[state.lastObject.rowIndex];
            this.setValueCore(subDocument.documentModel.cache, row, state.lastObject.value, state.lastObject.use);
            this.manipulator.dispatcher.notifyTableRowPropertyChanged(subDocument, this.jsonTableRowProperty, state);
        }
        private setValueCore(cache: DocumentCache, row: TableRow, newValue: T, newUse: boolean) {
            var properties = row.properties.clone();
            this.setProperty(properties, newValue);
            properties.setUseValue(this.tableRowPropertiesMask, newUse);
            row.properties = cache.tableRowPropertiesCache.addItemIfNonExists(properties);
        }
    }
}