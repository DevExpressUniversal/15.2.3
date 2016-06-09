module __aspxRichEdit {
    export enum JSONTableRowProperty {
        Height = 0,
        CellSpacing = 1,
        Header = 2,
        HideCellMark = 3,
        CantSplit = 4,
        TableRowAlignment = 5,
        UseValue = 6,
        GridAfter = 7,
        GridBefore = 8,
        WidthAfter = 9,
        WidthBefore = 10
    }

    export class JSONTableRowPropertiesConverter {
        static convertFromJSON(obj: any): TableRowProperties {
            var result = new TableRowProperties();
            result.cellSpacing = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableRowProperty.CellSpacing]);
            result.header = obj[JSONTableRowProperty.Header];
            result.hideCellMark = obj[JSONTableRowProperty.HideCellMark];
            result.cantSplit = obj[JSONTableRowProperty.CantSplit];
            result.tableRowAlignment = obj[JSONTableRowProperty.TableRowAlignment];
            result.mask = obj[JSONTableRowProperty.UseValue];
            return result;
        }

        static convertToJSON(source: TableRowProperties): any {
            var result = {};
            result[JSONTableRowProperty.CellSpacing] = JSONTableWidthUnitConverter.convertToJSON(source.cellSpacing);
            result[JSONTableRowProperty.Header] = source.header;
            result[JSONTableRowProperty.HideCellMark] = source.hideCellMark;
            result[JSONTableRowProperty.CantSplit] = source.cantSplit;
            result[JSONTableRowProperty.TableRowAlignment] = source.tableRowAlignment;
            result[JSONTableRowProperty.UseValue] = source.mask;
            return result;
        }
    }
} 