module __aspxRichEdit {
    export class TableCellPropertiesManipulator {
        cellMargins: ITableCellComplexPropertyWithUseManipulator<TableWidthUnit>;
        borders: ITableCellComplexPropertyWithUseManipulator<BorderInfo>;
        preferredWidth: ITableCellPropertyManipulator<TableWidthUnit>;
        hideCellMark: ITableCellPropertyWithUseManipulator<boolean>;
        noWrap: ITableCellPropertyWithUseManipulator<boolean>;
        fitText: ITableCellPropertyWithUseManipulator<boolean>;
        textDirection: ITableCellPropertyWithUseManipulator<TextDirection>;
        verticalAlignment: ITableCellPropertyWithUseManipulator<TableCellVerticalAlignment>;
        backgroundColor: ITableCellPropertyWithUseManipulator<number>;
        foreColor: ITableCellPropertyWithUseManipulator<number>;
        shading: ITableCellPropertyWithUseManipulator<ShadingPattern>;
        columnSpan: ITableCellPropertyManipulator<number>;
        verticalMerging: ITableCellPropertyManipulator<number>;

        constructor(manipulator: ModelManipulator) {
            this.cellMargins = new TableCellComplexPropertyWithUseManipulator<TableWidthUnit>(manipulator, JSONTableCellProperty.CellMargins,
                [TableCellPropertiesMask.UseTopMargin, TableCellPropertiesMask.UseRightMargin, TableCellPropertiesMask.UseBottomMargin, TableCellPropertiesMask.UseLeftMargin],
                (prop, vals) => { prop.cellMargins.top = vals[0]; prop.cellMargins.right = vals[1]; prop.cellMargins.bottom = vals[2]; prop.cellMargins.left = vals[3]; },
                (prop) => [prop.cellMargins.top, prop.cellMargins.right, prop.cellMargins.bottom, prop.cellMargins.left]);
            this.borders = new TableCellComplexPropertyWithUseManipulator<BorderInfo>(manipulator, JSONTableCellProperty.Borders,
                [TableCellPropertiesMask.UseTopBorder, TableCellPropertiesMask.UseRightBorder, TableCellPropertiesMask.UseBottomBorder, TableCellPropertiesMask.UseLeftBorder, TableCellPropertiesMask.UseTopLeftDiagonalBorder, TableCellPropertiesMask.UseTopRightDiagonalBorder],
                (prop, vals) => {
                    prop.borders.topBorder = vals[0] || prop.borders.topBorder;
                    prop.borders.rightBorder = vals[1] || prop.borders.rightBorder;
                    prop.borders.bottomBorder = vals[2] || prop.borders.bottomBorder;
                    prop.borders.leftBorder = vals[3] || prop.borders.leftBorder;
                    prop.borders.topLeftDiagonalBorder = vals[4] || prop.borders.topLeftDiagonalBorder;
                    prop.borders.topRightDiagonalBorder = vals[5] || prop.borders.topRightDiagonalBorder;
                },
                (prop) => [prop.borders.topBorder, prop.borders.rightBorder, prop.borders.bottomBorder, prop.borders.leftBorder, prop.borders.topLeftDiagonalBorder, prop.borders.topRightDiagonalBorder]);
            this.preferredWidth = new TableCellPropertiesManipulatorCore<TableWidthUnit>(manipulator, JSONTableCellProperty.PreferredWidth,
                (cell, val) => cell.preferredWidth = val,
                (cell) => cell.preferredWidth);
            this.hideCellMark = new TableCellPropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableCellProperty.HideCellMark, TableCellPropertiesMask.UseHideCellMark,
                (prop, val) => prop.hideCellMark = val,
                (prop) => prop.hideCellMark);
            this.noWrap = new TableCellPropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableCellProperty.NoWrap, TableCellPropertiesMask.UseNoWrap,
                (prop, val) => prop.noWrap = val,
                (prop) => prop.noWrap);
            this.fitText = new TableCellPropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableCellProperty.FitText, TableCellPropertiesMask.UseFitText,
                (prop, val) => prop.fitText = val,
                (prop) => prop.fitText);
            this.textDirection = new TableCellPropertiesWithUseManipulatorCore<TextDirection>(manipulator, JSONTableCellProperty.TextDirection, TableCellPropertiesMask.UseTextDirection,
                (prop, val) => prop.textDirection = val,
                (prop) => prop.textDirection);
            this.verticalAlignment = new TableCellPropertiesWithUseManipulatorCore<TableCellVerticalAlignment>(manipulator, JSONTableCellProperty.VerticalAlignment, TableCellPropertiesMask.UseVerticalAlignment,
                (prop, val) => prop.verticalAlignment = val,
                (prop) => prop.verticalAlignment);
            this.backgroundColor = new TableCellPropertiesWithUseManipulatorCore<number>(manipulator, JSONTableCellProperty.BackgroundColor, TableCellPropertiesMask.UseBackgroundColor,
                (prop, val) => prop.backgroundColor = val,
                (prop) => prop.backgroundColor);
            this.foreColor = new TableCellPropertiesWithUseManipulatorCore<number>(manipulator, JSONTableCellProperty.ForegroundColor, TableCellPropertiesMask.UseForegroundColor,
                (prop, val) => prop.foreColor = val,
                (prop) => prop.foreColor);
            this.shading = new TableCellPropertiesWithUseManipulatorCore<ShadingPattern>(manipulator, JSONTableCellProperty.Shading, TableCellPropertiesMask.UseShading,
                (prop, val) => prop.shading = val,
                (prop) => prop.shading);
            this.columnSpan = new TableCellPropertyManipulator<number>(manipulator, JSONTableCellProperty.ColumnSpan,
                (cell, val) => cell.columnSpan = val, cell => cell.columnSpan);
            this.verticalMerging = new TableCellPropertyManipulator<TableCellMergingState>(manipulator, JSONTableCellProperty.VerticalMerging,
                (cell, val) => cell.verticalMerging = val, cell => cell.verticalMerging);
        }
    }

    class TableCellPropertiesManipulatorCore<T> implements ITableCellPropertyManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableCellProperty: JSONTableCellProperty;
        setProperty: (row: TableCell, value: T) => void;
        getProperty: (row: TableCell) => T;
        constructor(manipulator: ModelManipulator, jsonTableCellProperty: JSONTableCellProperty, setProperty: (cell: TableCell, value: T) => void, getProperty: (cell: TableCell) => T) {
            this.manipulator = manipulator;
            this.jsonTableCellProperty = jsonTableCellProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }
        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValue: T): HistoryItemState<HistoryItemTableCellStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableCellStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableCellStateObject>();
            let row = table.rows[rowIndex];
            let cell = row.cells[cellIndex];
            oldState.register(new HistoryItemTableCellStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, this.getProperty(cell)));
            this.setProperty(cell, newValue);
            newState.register(new HistoryItemTableCellStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, newValue));
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            let row = table.rows[state.lastObject.rowIndex];
            let cell = row.cells[state.lastObject.cellIndex];
            this.setProperty(cell, state.lastObject.value);
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, state);
        }
    }

    class TableCellPropertyManipulator<T> implements ITableCellPropertyManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableCellProperty: JSONTableCellProperty;
        setProperty: (cell: TableCell, value: T) => void;
        getProperty: (cell: TableCell) => T;

        constructor(manipulator: ModelManipulator, jsonTableCellProperty: JSONTableCellProperty, setProperty: (cell: TableCell, value: T) => void, getProperty: (cell: TableCell) => T) {
            this.manipulator = manipulator;
            this.jsonTableCellProperty = jsonTableCellProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }

        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValue: T): HistoryItemState<HistoryItemTableCellStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableCellStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableCellStateObject>();
            let cell = table.rows[rowIndex].cells[cellIndex];

            oldState.register(new HistoryItemTableCellStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, this.getProperty(cell)));
            this.setProperty(cell, newValue);
            newState.register(new HistoryItemTableCellStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, newValue));
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            let cell = table.rows[state.lastObject.rowIndex].cells[state.lastObject.cellIndex];
            this.setProperty(cell, state.lastObject.value);
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, state);
        }
    }

    class TableCellComplexPropertyWithUseManipulator<T> implements ITableCellComplexPropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableCellProperty: JSONTableCellProperty;
        setProperties: (properties: TableCellProperties, values: T[]) => void;
        getProperties: (properties: TableCellProperties) => T[];
        masks: TableCellPropertiesMask[];

        constructor(manipulator: ModelManipulator, jsonTableCellProperty: JSONTableCellProperty, masks: TableCellPropertiesMask[], setProperties: (properties: TableCellProperties, values: T[]) => void, getProperties: (properties: TableCellProperties) => T[]) {
            this.manipulator = manipulator;
            this.jsonTableCellProperty = jsonTableCellProperty;
            this.setProperties = setProperties;
            this.getProperties = getProperties;
            this.masks = masks;
        }

        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValues: T[], newUses: boolean[]): HistoryItemState<HistoryItemTableCellComplexUseStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableCellComplexUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableCellComplexUseStateObject>();
            let cell = table.rows[rowIndex].cells[cellIndex];
            let properties = cell.properties;

            let oldUseValues = [];
            for(let mask of this.masks)
                oldUseValues.push(properties.getUseValue(mask));
            oldState.register(new HistoryItemTableCellComplexUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, this.getProperties(properties), oldUseValues));
            this.setValuesCore(subDocument.documentModel.cache, cell, newValues, newUses);
            newState.register(new HistoryItemTableCellComplexUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, newValues, newUses));
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellComplexUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            let cell = table.rows[state.lastObject.rowIndex].cells[state.lastObject.cellIndex];
            this.setValuesCore(subDocument.documentModel.cache, cell, state.lastObject.value, state.lastObject.uses);
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, state);
        }
        private setValuesCore(cache: DocumentCache, cell: TableCell, newValues: T[], newUses: boolean[]) {
            var properties = cell.properties.clone();
            this.setProperties(properties, newValues);
            for(let i = this.masks.length - 1; i >= 0; i--)
                properties.setUseValue(this.masks[i], newUses[i]);
            cell.properties = cache.tableCellPropertiesCache.addItemIfNonExists(properties);
        }
    }

    class TableCellPropertiesWithUseManipulatorCore<T> implements ITableCellPropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableCellProperty: JSONTableCellProperty;
        tableCellPropertiesMask: TableCellPropertiesMask;
        setProperty: (properties: TableCellProperties, value: T) => void;
        getProperty: (properties: TableCellProperties) => T;

        constructor(manipulator: ModelManipulator, jsonTableCellProperty: JSONTableCellProperty, tableCellPropertiesMask: TableCellPropertiesMask, setProperty: (properties: TableCellProperties, value: T) => void, getProperty: (properties: TableCellProperties) => T) {
            this.manipulator = manipulator;
            this.tableCellPropertiesMask = tableCellPropertiesMask;
            this.jsonTableCellProperty = jsonTableCellProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }

        setValue(subDocument: SubDocument, tableIndex: number, rowIndex: number, cellIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemTableCellUseStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableCellUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableCellUseStateObject>();
            let cell = table.rows[rowIndex].cells[cellIndex];
            let properties = cell.properties;

            oldState.register(new HistoryItemTableCellUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, this.getProperty(properties), properties.getUseValue(this.tableCellPropertiesMask)));
            this.setValueCore(subDocument.documentModel.cache, cell, newValue, newUse);
            newState.register(new HistoryItemTableCellUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, rowIndex, cellIndex, newValue, newUse));
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableCellUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            let cell = table.rows[state.lastObject.rowIndex].cells[state.lastObject.cellIndex];
            this.setValueCore(subDocument.documentModel.cache, cell, state.lastObject.value, state.lastObject.use);
            this.manipulator.dispatcher.notifyTableCellPropertyChanged(subDocument, this.jsonTableCellProperty, state);
        }
        private setValueCore(cache: DocumentCache, cell: TableCell, newValue: T, newUse: boolean) {
            var properties = cell.properties.clone();
            this.setProperty(properties, newValue);
            properties.setUseValue(this.tableCellPropertiesMask, newUse);
            cell.properties = cache.tableCellPropertiesCache.addItemIfNonExists(properties);
        }
    }
}