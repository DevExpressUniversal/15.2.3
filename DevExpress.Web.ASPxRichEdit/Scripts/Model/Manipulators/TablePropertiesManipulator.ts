module __aspxRichEdit {
    export class TablePropertiesManipulator {
        cellMargins: ITableComplexPropertyWithUseManipulator<TableWidthUnit>;
        cellSpacing: ITablePropertyWithUseManipulator<TableWidthUnit>;
        indent: ITablePropertyWithUseManipulator<TableWidthUnit>;
        preferredWidth: ITablePropertyManipulator<TableWidthUnit>;
        borders: ITableComplexPropertyWithUseManipulator<BorderInfo>;

        tableStyleColumnBandSize: ITablePropertyWithUseManipulator<number>;
        tableStyleRowBandSize: ITablePropertyWithUseManipulator<number>;
        avoidDoubleBorders: ITablePropertyWithUseManipulator<boolean>;
        layoutType: ITablePropertyWithUseManipulator<TableLayoutType>;
        lookTypes: ITablePropertyManipulator<TableLookTypes>;
        backgroundColor: ITablePropertyWithUseManipulator<number>;
        tableRowAlignment: ITablePropertyWithUseManipulator<TableRowAlignment>;

        isTableOverlap: ITablePropertyWithUseManipulator<boolean>;

        constructor(manipulator: ModelManipulator) {
            this.cellMargins = new TableComplexPropertyWithUseManipulator<TableWidthUnit>(manipulator, JSONTableProperty.CellMargins,
                [TablePropertiesMask.UseTopMargin, TablePropertiesMask.UseRightMargin, TablePropertiesMask.UseBottomMargin, TablePropertiesMask.UseLeftMargin],
                (prop, vals) => {
                    prop.cellMargins.top = vals[0];
                    prop.cellMargins.right = vals[1];
                    prop.cellMargins.bottom = vals[2];
                    prop.cellMargins.left = vals[3];
                },
                prop => [prop.cellMargins.top, prop.cellMargins.right, prop.cellMargins.bottom, prop.cellMargins.left]);
            this.cellSpacing = new TablePropertiesWithUseManipulatorCore<TableWidthUnit>(manipulator, JSONTableProperty.CellSpacing,
                TablePropertiesMask.UseCellSpacing,
                (prop, val) => prop.cellSpacing = val,
                (prop) => prop.cellSpacing);
            this.indent = new TablePropertiesWithUseManipulatorCore<TableWidthUnit>(manipulator, JSONTableProperty.Indent,
                TablePropertiesMask.UseTableIndent,
                (prop, val) => prop.indent = val,
                (prop) => prop.indent);
            this.preferredWidth = new TablePropertiesManipulatorCore<TableWidthUnit>(manipulator, JSONTableProperty.PreferredWidth,
                (table, val) => table.preferredWidth = val,
                (table) => table.preferredWidth);
            this.lookTypes = new TablePropertiesManipulatorCore<TableLookTypes>(manipulator, JSONTableProperty.TableLookTypes,
                (table, val) => table.lookTypes = val,
                (table) => table.lookTypes);
            this.borders = new TableComplexPropertyWithUseManipulator<BorderInfo>(manipulator, JSONTableProperty.Borders,
                [TablePropertiesMask.UseTopBorder, TablePropertiesMask.UseRightBorder, TablePropertiesMask.UseBottomBorder, TablePropertiesMask.UseLeftBorder, TablePropertiesMask.UseInsideHorizontalBorder, TablePropertiesMask.UseInsideVerticalBorder],
                (prop, vals) => {
                    prop.borders.topBorder = vals[0] || prop.borders.topBorder;
                    prop.borders.rightBorder = vals[1] || prop.borders.rightBorder;
                    prop.borders.bottomBorder = vals[2] || prop.borders.bottomBorder;
                    prop.borders.leftBorder = vals[3] || prop.borders.leftBorder;
                    prop.borders.insideHorizontalBorder = vals[4] || prop.borders.insideHorizontalBorder;
                    prop.borders.insideVerticalBorder = vals[5] || prop.borders.insideVerticalBorder;
                },
                prop => [prop.borders.topBorder, prop.borders.rightBorder, prop.borders.bottomBorder, prop.borders.leftBorder, prop.borders.insideHorizontalBorder, prop.borders.insideVerticalBorder]);
            this.tableStyleColumnBandSize = new TablePropertiesWithUseManipulatorCore<number>(manipulator, JSONTableProperty.TableStyleColBandSize,
                TablePropertiesMask.UseTableStyleColBandSize,
                (prop, val) => prop.tableStyleColumnBandSize = val,
                (prop) => prop.tableStyleColumnBandSize);
            this.tableStyleRowBandSize = new TablePropertiesWithUseManipulatorCore<number>(manipulator, JSONTableProperty.TableStyleRowBandSize,
                TablePropertiesMask.UseTableStyleRowBandSize,
                (prop, val) => prop.tableStyleRowBandSize = val,
                (prop) => prop.tableStyleRowBandSize);
            this.avoidDoubleBorders = new TablePropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableProperty.AvoidDoubleBorders,
                TablePropertiesMask.UseAvoidDoubleBorders,
                (prop, val) => prop.avoidDoubleBorders = val,
                (prop) => prop.avoidDoubleBorders);
            this.layoutType = new TablePropertiesWithUseManipulatorCore<TableLayoutType>(manipulator, JSONTableProperty.LayoutType,
                TablePropertiesMask.UseTableLayout,
                (prop, val) => prop.layoutType = val,
                (prop) => prop.layoutType);           
            this.backgroundColor = new TablePropertiesWithUseManipulatorCore<number>(manipulator, JSONTableProperty.BackgroundColor,
                TablePropertiesMask.UseBackgroundColor,
                (prop, val) => prop.backgroundColor = val,
                (prop) => prop.backgroundColor);
            this.tableRowAlignment = new TablePropertiesWithUseManipulatorCore<number>(manipulator, JSONTableProperty.TableRowAlignment,
                TablePropertiesMask.UseTableAlignment,
                (prop, val) => prop.tableRowAlignment = val,
                (prop) => prop.tableRowAlignment);
            this.isTableOverlap = new TablePropertiesWithUseManipulatorCore<boolean>(manipulator, JSONTableProperty.IsTableOverlap,
                TablePropertiesMask.UseIsTableOverlap,
                (prop, val) => prop.isTableOverlap = val,
                (prop) => prop.isTableOverlap);
        }
    }

    class TablePropertiesManipulatorCore<T> implements ITablePropertyManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableProperty: JSONTableProperty;
        setProperty: (row: Table, value: T) => void;
        getProperty: (row: Table) => T;
        constructor(manipulator: ModelManipulator, jsonTableProperty: JSONTableProperty, setProperty: (table: Table, value: T) => void, getProperty: (table: Table) => T) {
            this.manipulator = manipulator;
            this.jsonTableProperty = jsonTableProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }
        setValue(subDocument: SubDocument, tableIndex: number, newValue: T): HistoryItemState<HistoryItemTableStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableStateObject>();
            oldState.register(new HistoryItemTableStateObject(tableStartPosition, table.nestedLevel, tableIndex, this.getProperty(table)));
            this.setProperty(table, newValue);
            newState.register(new HistoryItemTableStateObject(tableStartPosition, table.nestedLevel, tableIndex, newValue));
            this.manipulator.dispatcher.notifyTablePropertyChanged(subDocument, this.jsonTableProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            this.setProperty(table, state.lastObject.value);
            this.manipulator.dispatcher.notifyTablePropertyChanged(subDocument, this.jsonTableProperty, state);
        }
    }

    class TableComplexPropertyWithUseManipulator<T> implements ITableComplexPropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableProperty: JSONTableProperty;
        setProperties: (properties: TableProperties, values: T[]) => void;
        getProperties: (properties: TableProperties) => T[];
        masks: TablePropertiesMask[];

        constructor(manipulator: ModelManipulator, jsonTableProperty: JSONTableProperty, masks: TablePropertiesMask[], setProperties: (properties: TableProperties, values: T[]) => void, getProperties: (properties: TableProperties) => T[]) {
            this.manipulator = manipulator;
            this.jsonTableProperty = jsonTableProperty;
            this.setProperties = setProperties;
            this.getProperties = getProperties;
            this.masks = masks;
        }

        setValue(subDocument: SubDocument, tableIndex: number, newValues: T[], newUses: boolean[]): HistoryItemState<HistoryItemTableComplexUseStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableComplexUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableComplexUseStateObject>();
            let properties = table.properties;

            let oldUseValues = [];
            for(let mask of this.masks)
                oldUseValues.push(properties.getUseValue(mask));
            oldState.register(new HistoryItemTableComplexUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, this.getProperties(properties), oldUseValues));
            this.setValuesCore(subDocument.documentModel.cache, table, newValues, newUses);
            newState.register(new HistoryItemTableComplexUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, newValues, newUses));
            this.manipulator.dispatcher.notifyTablePropertyChanged(subDocument, this.jsonTableProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableComplexUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            this.setValuesCore(subDocument.documentModel.cache, table, state.lastObject.value, state.lastObject.uses);
            this.manipulator.dispatcher.notifyTablePropertyChanged(subDocument, this.jsonTableProperty, state);
        }
        private setValuesCore(cache: DocumentCache, table: Table, newValues: T[], newUses: boolean[]) {
            var properties = table.properties.clone();
            this.setProperties(properties, newValues);
            for(let i = this.masks.length - 1; i >= 0; i--)
                properties.setUseValue(this.masks[i], newUses[i]);
            table.properties = properties;
        }
    }

    class TablePropertiesWithUseManipulatorCore<T> implements ITablePropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonTableProperty: JSONTableProperty;
        tablePropertiesMask: TablePropertiesMask;
        setProperty: (properties: TableProperties, value: T) => void;
        getProperty: (properties: TableProperties) => T;

        constructor(manipulator: ModelManipulator, jsonTableProperty: JSONTableProperty, tablePropertiesMask: TablePropertiesMask, setProperty: (properties: TableProperties, value: T) => void, getProperty: (properties: TableProperties) => T) {
            this.manipulator = manipulator;
            this.tablePropertiesMask = tablePropertiesMask;
            this.jsonTableProperty = jsonTableProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }

        setValue(subDocument: SubDocument, tableIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemTableUseStateObject> {
            let table = subDocument.tables[tableIndex];
            let tableStartPosition = table.getStartPosition();
            var newState = new HistoryItemState<HistoryItemTableUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemTableUseStateObject>();
            let properties = table.properties;

            oldState.register(new HistoryItemTableUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, this.getProperty(properties), properties.getUseValue(this.tablePropertiesMask)));
            this.setValueCore(subDocument.documentModel.cache, table, newValue, newUse);
            newState.register(new HistoryItemTableUseStateObject(tableStartPosition, table.nestedLevel, tableIndex, newValue, newUse));
            this.manipulator.dispatcher.notifyTablePropertyChanged(subDocument, this.jsonTableProperty, newState);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemTableUseStateObject>) {
            let table = subDocument.tables[state.lastObject.tableIndex];
            this.setValueCore(subDocument.documentModel.cache, table, state.lastObject.value, state.lastObject.use);
            this.manipulator.dispatcher.notifyTablePropertyChanged(subDocument, this.jsonTableProperty, state);
        }
        private setValueCore(cache: DocumentCache, table: Table, newValue: T, newUse: boolean) {
            var properties = table.properties.clone();
            this.setProperty(properties, newValue);
            properties.setUseValue(this.tablePropertiesMask, newUse);
            table.properties = properties;
        }
    }
}