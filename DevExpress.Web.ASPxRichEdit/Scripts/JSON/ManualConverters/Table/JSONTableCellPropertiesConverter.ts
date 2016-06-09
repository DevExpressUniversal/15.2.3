module __aspxRichEdit {
    export enum JSONTableCellProperty {
        CellMargins = 0,
        PreferredWidth = 1, 
        Borders = 2,
        HideCellMark = 3,
        NoWrap = 4,
        FitText = 5,
        TextDirection = 6,
        VerticalAlignment = 7,
        BackgroundColor = 8,
        ForegroundColor = 9,
        Shading = 10,
        UseValue = 11,
        ColumnSpan = 12,
        VerticalMerging = 13
    }
    
    export class JSONTableCellPropertiesConverter {
        static convertFromJSON(obj: any): TableCellProperties {
            var result = new TableCellProperties();
            result.cellMargins = JSONTableCellMarginsConverter.convertFromJSON(obj[JSONTableCellProperty.CellMargins]);
            result.borders = JSONTableCellBordersConverter.convertFromJSON(obj[JSONTableCellProperty.Borders]);
            result.hideCellMark = obj[JSONTableCellProperty.HideCellMark];
            result.noWrap = obj[JSONTableCellProperty.NoWrap];
            result.fitText = obj[JSONTableCellProperty.FitText];
            result.textDirection = obj[JSONTableCellProperty.TextDirection];
            result.verticalAlignment = obj[JSONTableCellProperty.VerticalAlignment];
            result.backgroundColor = obj[JSONTableCellProperty.BackgroundColor];
            result.foreColor = obj[JSONTableCellProperty.ForegroundColor];
            result.shading = obj[JSONTableCellProperty.Shading];
            result.mask = obj[JSONTableCellProperty.UseValue];
            return result;
        }

        static convertToJSON(source: TableCellProperties): any {
            var result = {};
            result[JSONTableCellProperty.CellMargins] = JSONTableCellMarginsConverter.convertToJSON(source.cellMargins);
            result[JSONTableCellProperty.Borders] = JSONTableCellBordersConverter.convertToJSON(source.borders);
            result[JSONTableCellProperty.HideCellMark] = source.hideCellMark;
            result[JSONTableCellProperty.NoWrap] = source.noWrap;
            result[JSONTableCellProperty.FitText] = source.fitText;
            result[JSONTableCellProperty.TextDirection] = source.textDirection;
            result[JSONTableCellProperty.VerticalAlignment] = source.verticalAlignment;
            result[JSONTableCellProperty.BackgroundColor] = source.backgroundColor;
            result[JSONTableCellProperty.ForegroundColor] = source.foreColor;
            result[JSONTableCellProperty.Shading] = source.shading;
            result[JSONTableCellProperty.UseValue] = source.mask;
            return result;
        }
    }
} 